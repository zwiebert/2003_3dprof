// ---------------------------------------------------- //
//                      WinIo v2.0                      //
//  Direct Hardware Access Under Windows 9x/NT/2000/XP  //
//           Copyright 1998-2002 Yariv Kaplan           //
//               http://www.internals.com               //
// ---------------------------------------------------- //

#include <ntddk.h>
#include <string.h>
#include "winio_nt.h"
#include <stdio.h>

#define D(x) //x
#define OutputDebugString(txt) D(DbgPrint(txt))

void test_pci_scan(void);

#define IOPM_SIZE 0x2000
typedef char IOPM[IOPM_SIZE];
IOPM *pIOPM = NULL;

// Function definition section
// -----------------------------------------------------------------
NTSTATUS WinIoDispatch(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp);
void WinIoUnload(IN PDRIVER_OBJECT DriverObject);
NTSTATUS UnmapPhysicalMemory(HANDLE PhysicalMemoryHandle, PVOID pPhysMemLin);
NTSTATUS MapPhysicalMemoryToLinearSpace(PVOID pPhysAddress,
                                        ULONG PhysMemSizeInBytes,
                                        PVOID *ppPhysMemLin,
                                        HANDLE *pPhysicalMemoryHandle);

void Ke386SetIoAccessMap(int, IOPM *);
void Ke386QueryIoAccessMap(int, IOPM *);
void Ke386IoSetAccessProcess(PEPROCESS, int);


/**********************************************************
*
* Function   :  SstReportEvent
*
* Description:  Uses the input parameters to send a 
message to the Event Log
*               found in the Administrative Tools Section 
of Windows NT.
*
*******************************************************/

#define EVENTLOG_MSG_ARR_SIZE 40
LONG
SstReportEvent(
               IN PVOID  pIoObject,
               IN NTSTATUS  MsgCode,
               IN CHAR        *p_c_ArgString
               )
{
  PIO_ERROR_LOG_PACKET  pPacket;
  UCHAR    uc_Size;
  WCHAR    arr_wd_EventLogMsg
    [EVENTLOG_MSG_ARR_SIZE]; 

  /* this EVENTLOG_MSG_ARR_SIZE equals 41 */

  if (pIoObject == NULL ||
    p_c_ArgString == NULL)
    return -1;

  if ( strlen(p_c_ArgString) <= 0   ||
    strlen(p_c_ArgString) >= 
    EVENTLOG_MSG_ARR_SIZE )
    return -2;

  swprintf(
    arr_wd_EventLogMsg,
    L"%S",
    p_c_ArgString
    );

  uc_Size = 
    sizeof(IO_ERROR_LOG_PACKET) + 
    ((wcslen(arr_wd_EventLogMsg) + 1) 
    * sizeof(WCHAR));

  if (uc_Size >= ERROR_LOG_MAXIMUM_SIZE)
    return -3;


  // Try to allocate the packet
  pPacket = IoAllocateErrorLogEntry(
    pIoObject,
    uc_Size
    );

  if (pPacket == NULL)
    return -4;

  // Fill in standard parts of the packet
  pPacket->MajorFunctionCode  = 0;
  pPacket->RetryCount    = 0;
  pPacket->DumpDataSize   = 0;

  pPacket->EventCategory   = 0;
  pPacket->ErrorCode    = MsgCode;
  pPacket->UniqueErrorValue  = 0;
  pPacket->FinalStatus   = 
    STATUS_SUCCESS;
  pPacket->SequenceNumber   = 0;
  pPacket->IoControlCode   = 0;
  pPacket->DeviceOffset.QuadPart = 0;

  pPacket->NumberOfStrings  = 1;
  pPacket->StringOffset   = 
    FIELD_OFFSET(IO_ERROR_LOG_PACKET, DumpData) ;

  RtlCopyMemory(
    (PWSTR) ( &pPacket->DumpData[0] ),
    arr_wd_EventLogMsg,
    uc_Size - sizeof(IO_ERROR_LOG_PACKET)
    );


  // Log the message
  IoWriteErrorLogEntry(
    pPacket
    );

  return 0;

} // Closing : "SstReportEvent(..)"
// -----------------------------------------------------------------

// Installable driver initialization entry point.
// This entry point is called directly by the I/O system.

NTSTATUS DriverEntry (IN PDRIVER_OBJECT DriverObject,
                      IN PUNICODE_STRING RegistryPath)
{
  UNICODE_STRING  DeviceNameUnicodeString;
  UNICODE_STRING  DeviceLinkUnicodeString;
  NTSTATUS        ntStatus;
  PDEVICE_OBJECT  DeviceObject = NULL;

  OutputDebugString ("Entering DriverEntry");

  RtlInitUnicodeString (&DeviceNameUnicodeString, L"\\Device\\WinIo");

  // Create an EXCLUSIVE device object (only 1 thread at a time
  // can make requests to this device).

  ntStatus = IoCreateDevice (DriverObject,
    0,
    &DeviceNameUnicodeString,
    FILE_DEVICE_WINIO,
    0,
    TRUE,
    &DeviceObject);

  if (NT_SUCCESS(ntStatus))
  {
    // Create dispatch points for device control, create, close.

    DriverObject->MajorFunction[IRP_MJ_CREATE]         =
      DriverObject->MajorFunction[IRP_MJ_CLOSE]          =
      DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = WinIoDispatch;
    DriverObject->DriverUnload                         = WinIoUnload;

    // Create a symbolic link, e.g. a name that a Win32 app can specify
    // to open the device.

    RtlInitUnicodeString (&DeviceLinkUnicodeString, L"\\DosDevices\\WinIo");

    ntStatus = IoCreateSymbolicLink (&DeviceLinkUnicodeString,
      &DeviceNameUnicodeString);

    if (!NT_SUCCESS(ntStatus))
    {
      // Symbolic link creation failed- note this & then delete the
      // device object (it's useless if a Win32 app can't get at it).

      OutputDebugString ("ERROR: IoCreateSymbolicLink failed");

      IoDeleteDevice (DeviceObject);
    }

  }
  else
  {
    OutputDebugString ("ERROR: IoCreateDevice failed");
  }

  OutputDebugString ("Leaving DriverEntry");
  test_pci_scan(); //TODO:XXX
  return ntStatus;
}


// Process the IRPs sent to this device

NTSTATUS WinIoDispatch(IN PDEVICE_OBJECT DeviceObject,
                       IN PIRP Irp)
{
  PIO_STACK_LOCATION IrpStack;
  ULONG              dwInputBufferLength;
  ULONG              dwOutputBufferLength;
  ULONG              dwIoControlCode;
  PVOID              pvIOBuffer;
  NTSTATUS           ntStatus;
  struct             tagPhys32Struct Phys32Struct;

  OutputDebugString ("Entering WinIoDispatch");

  // Init to default settings

  Irp->IoStatus.Status      = STATUS_SUCCESS;
  Irp->IoStatus.Information = 0;

  IrpStack = IoGetCurrentIrpStackLocation(Irp);

  // Get the pointer to the input/output buffer and it's length

  pvIOBuffer           = Irp->AssociatedIrp.SystemBuffer;
  dwInputBufferLength  = IrpStack->Parameters.DeviceIoControl.InputBufferLength;
  dwOutputBufferLength = IrpStack->Parameters.DeviceIoControl.OutputBufferLength;

  switch (IrpStack->MajorFunction)
  {
  case IRP_MJ_CREATE:

    OutputDebugString("IRP_MJ_CREATE");

    break;

  case IRP_MJ_CLOSE:

    OutputDebugString("IRP_MJ_CLOSE");

    break;

  case IRP_MJ_DEVICE_CONTROL:

    OutputDebugString("IRP_MJ_DEVICE_CONTROL");

    dwIoControlCode = IrpStack->Parameters.DeviceIoControl.IoControlCode;

    switch (dwIoControlCode)
    {
    case IOCTL_WINIO_ENABLEDIRECTIO:

      OutputDebugString("IOCTL_WINIO_ENABLEDIRECTIO");

      pIOPM = MmAllocateNonCachedMemory(sizeof(IOPM));

      if (pIOPM)
      {
        RtlZeroMemory(pIOPM, sizeof(IOPM));

        Ke386IoSetAccessProcess(PsGetCurrentProcess(), 1);
        Ke386SetIoAccessMap(1, pIOPM);
      }
      else
        Irp->IoStatus.Status = STATUS_INSUFFICIENT_RESOURCES;

      break;

    case IOCTL_WINIO_DISABLEDIRECTIO:

      OutputDebugString("IOCTL_WINIO_DISABLEDIRECTIO");

      if (pIOPM)
      {
        Ke386IoSetAccessProcess(PsGetCurrentProcess(), 0);
        Ke386SetIoAccessMap(1, pIOPM);

        MmFreeNonCachedMemory(pIOPM, sizeof(IOPM));
        pIOPM = NULL;
      }

      break;

    case IOCTL_WINIO_UNMAPIO:
      OutputDebugString("IOCTL_WINIO_UNMAPIO");

      if (!dwInputBufferLength) {
        Irp->IoStatus.Status = STATUS_INVALID_PARAMETER;
        break;
      } else {
        struct winio_map_io_data data;

        memcpy (&data, pvIOBuffer, dwInputBufferLength);

        if (data.virt_uaddr)
          MmUnmapLockedPages(data.virt_uaddr, data.mdl);
        if (data.mdl)
          IoFreeMdl (data.mdl);
        if (data.virt_kaddr)
          MmUnmapIoSpace(data.virt_kaddr, data.size_requested);
      }
      Irp->IoStatus.Status = STATUS_SUCCESS;
      break;

    case IOCTL_WINIO_MAPIO:
      OutputDebugString("IOCTL_WINIO_MAPIO New!");

      if (!dwInputBufferLength) {
        Irp->IoStatus.Status = STATUS_INVALID_PARAMETER;
        break;
      } else {
        struct winio_map_io_data data;
        PHYSICAL_ADDRESS pa;
        ULONG size;
        PVOID virt_kaddr=0, virt_uaddr=0;
        PMDL mdl=0;
        unsigned i;

        memcpy (&data, pvIOBuffer, dwInputBufferLength);

        pa.QuadPart = (ULONGLONG)data.phys_addr_requested;
        size = data.size_requested;

        if ((virt_kaddr = MmMapIoSpace(pa, size, MmNonCached))) {
          if ((mdl = IoAllocateMdl(virt_kaddr, size, FALSE, FALSE, NULL))) {
            MmBuildMdlForNonPagedPool(mdl);
            if ((virt_uaddr = MmMapLockedPagesSpecifyCache(mdl, UserMode, MmNonCached,
              NULL, FALSE, NormalPagePriority)))
            {
              data.virt_uaddr = virt_uaddr;
              data.virt_kaddr = virt_kaddr;
              data.mdl = mdl;
              memcpy (pvIOBuffer, &data, dwInputBufferLength);
              Irp->IoStatus.Information = dwInputBufferLength;
              Irp->IoStatus.Status = STATUS_SUCCESS;
              break;
            }
          }
        }

        D(DbgPrint("virt_kaddr=0x%lx\n", virt_kaddr));
        D(DbgPrint("mdl=0x%lx\n", mdl));
        D(DbgPrint("virt_uaddr=0x%lx\n", virt_uaddr));

        if (virt_uaddr)
          MmUnmapLockedPages(virt_uaddr, mdl);
        if (mdl)
          IoFreeMdl (mdl);
        if (virt_kaddr)
          MmUnmapIoSpace(virt_kaddr, size);
      }
      Irp->IoStatus.Status = STATUS_INVALID_PARAMETER; // TODO
      break;

    case IOCTL_WINIO_GETPCICOMMHDR:
      OutputDebugString("IOCTL_WINIO_GETPCICOMMHDR");

      if (dwInputBufferLength != sizeof (struct winio_pci_common_header_)) {
        Irp->IoStatus.Status = STATUS_INVALID_PARAMETER;
        break;
      } else {
        USHORT bus, dev, fn;
        struct winio_pci_common_header_ data;
        PPCI_COMMON_CONFIG  pci_data;
        ULONG result;

        memcpy (&data, pvIOBuffer, dwInputBufferLength);

        pci_data = (PPCI_COMMON_CONFIG)&data.VendorID;

        if (data.priv_iter_slot >= PCI_MAX_DEVICES) {
          data.priv_iter_slot = 0;
          ++data.priv_iter_bus;
        }

        for (bus=data.priv_iter_bus; bus < 8; ++bus) {
          for (dev=data.priv_iter_slot; dev < PCI_MAX_DEVICES; ++dev) {
            for (fn=0; fn == 0; ++fn) { //TODO: multifunction
              PCI_SLOT_NUMBER slot_nmb = {0};

              if (fn == 1 && !(data.HeaderType & PCI_MULTIFUNCTION))
                break; /* skipping functions 1-7 if not multi function is required */

              slot_nmb.u.bits.DeviceNumber = dev;
              slot_nmb.u.bits.FunctionNumber = 0;

              result = HalGetBusData(PCIConfiguration, bus, slot_nmb.u.AsULONG, pci_data,
                (sizeof data) - FIELD_OFFSET(struct winio_pci_common_header_, VendorID));
              if (result == 0)
                goto no_more_busses; // bus does not exist
              else if (result == 2 && pci_data->VendorID == PCI_INVALID_VENDORID)
                continue; // bus exists, but no device exists at this virtual slot number
              else // device found
              {
                D(DbgPrint("#bus=%d, dev=%d, fn=%d, vendor_id=0x%04lx, device_id=0x%04lx, sub_class=%02x, base_class=%02x, base_addr=0x%08lx\n",
                  (int)bus, (int)dev, (int)fn,
                  (int)pci_data->VendorID,
                  (int)pci_data->DeviceID,
                  (int)pci_data->BaseClass,
                  (int)pci_data->SubClass,
                  pci_data->u.type0.BaseAddresses[0]));
                data.Bus = bus;
                data.Slot = ((dev << 3) | fn);
                data.priv_iter_bus = bus;
                data.priv_iter_slot = dev + 1;
                goto found_pci_hdr;
              }
            }
          }
        }
no_more_busses:
          data.Bus = ~0;
found_pci_hdr:
          memcpy (pvIOBuffer, &data, dwInputBufferLength);
          Irp->IoStatus.Information = dwInputBufferLength;
          Irp->IoStatus.Status = STATUS_SUCCESS;
      }
      OutputDebugString("Leaving IOCTL_WINIO_GETPCICOMMHDR");


      break;

    case IOCTL_WINIO_MAPPHYSTOLIN:

      OutputDebugString("IOCTL_WINIO_MAPPHYSTOLIN");

      if (dwInputBufferLength)
      {
        memcpy (&Phys32Struct, pvIOBuffer, dwInputBufferLength);
        ntStatus = MapPhysicalMemoryToLinearSpace(Phys32Struct.pvPhysAddress,
          Phys32Struct.dwPhysMemSizeInBytes,
          &Phys32Struct.pvPhysMemLin,
          &Phys32Struct.PhysicalMemoryHandle);

        if (NT_SUCCESS(ntStatus))
        {
          memcpy (pvIOBuffer, &Phys32Struct, dwInputBufferLength);
          Irp->IoStatus.Information = dwInputBufferLength;
        } else {
          SstReportEvent(DeviceObject, ntStatus, "MapPhysicalMemoryToLinearSpace failed");
        }

        Irp->IoStatus.Status = ntStatus;
      }
      else
        Irp->IoStatus.Status = STATUS_INVALID_PARAMETER;

      break;

    case IOCTL_WINIO_UNMAPPHYSADDR:

      OutputDebugString("IOCTL_WINIO_UNMAPPHYSADDR");

      if (dwInputBufferLength)
      {
        memcpy (&Phys32Struct, pvIOBuffer, dwInputBufferLength);

        ntStatus = UnmapPhysicalMemory(Phys32Struct.PhysicalMemoryHandle, Phys32Struct.pvPhysMemLin);

        Irp->IoStatus.Status = ntStatus;
      }
      else
        Irp->IoStatus.Status = STATUS_INVALID_PARAMETER;

      break;

    default:

      OutputDebugString("ERROR: Unknown IRP_MJ_DEVICE_CONTROL");

      Irp->IoStatus.Status = STATUS_INVALID_PARAMETER;

      break;
    }

    break;
  }

  // DON'T get cute and try to use the status field of the irp in the
  // return status.  That IRP IS GONE as soon as you call IoCompleteRequest.

  ntStatus = Irp->IoStatus.Status;

  IoCompleteRequest (Irp, IO_NO_INCREMENT);

  // We never have pending operation so always return the status code.

  OutputDebugString("Leaving WinIoDispatch");

  return ntStatus;
}

// Delete the associated device and return

void WinIoUnload(IN PDRIVER_OBJECT DriverObject)
{
  UNICODE_STRING DeviceLinkUnicodeString;
  NTSTATUS ntStatus;

  OutputDebugString ("Entering WinIoUnload");

  RtlInitUnicodeString (&DeviceLinkUnicodeString, L"\\DosDevices\\WinIo");

  ntStatus = IoDeleteSymbolicLink (&DeviceLinkUnicodeString);

  if (NT_SUCCESS(ntStatus))
  {
    IoDeleteDevice (DriverObject->DeviceObject);
  }
  else
  {
    OutputDebugString ("ERROR: IoDeleteSymbolicLink");
  }

  OutputDebugString ("Leaving WinIoUnload...");
}


NTSTATUS MapPhysicalMemoryToLinearSpace(PVOID pPhysAddress,
                                        ULONG PhysMemSizeInBytes,
                                        PVOID *ppPhysMemLin,
                                        HANDLE *pPhysicalMemoryHandle)
{
  UNICODE_STRING     PhysicalMemoryUnicodeString;
  PVOID              PhysicalMemorySection = NULL;
  OBJECT_ATTRIBUTES  ObjectAttributes;
  PHYSICAL_ADDRESS   ViewBase;
  NTSTATUS           ntStatus;
  PHYSICAL_ADDRESS   pStartPhysAddress;
  PHYSICAL_ADDRESS   pEndPhysAddress;
  PHYSICAL_ADDRESS   MappingLength;
  BOOLEAN            Result1, Result2;
  ULONG              IsIOSpace;
  unsigned char     *pbPhysMemLin = NULL;

  OutputDebugString ("Entering MapPhysicalMemoryToLinearSpace");

  RtlInitUnicodeString (&PhysicalMemoryUnicodeString,
    L"\\Device\\PhysicalMemory");

  InitializeObjectAttributes (&ObjectAttributes,
    &PhysicalMemoryUnicodeString,
    OBJ_CASE_INSENSITIVE,
    (HANDLE) NULL,
    (PSECURITY_DESCRIPTOR) NULL);

  *pPhysicalMemoryHandle = NULL;

  ntStatus = ZwOpenSection (pPhysicalMemoryHandle,
    SECTION_ALL_ACCESS,
    &ObjectAttributes);

  if (NT_SUCCESS(ntStatus))
  {

    ntStatus = ObReferenceObjectByHandle (*pPhysicalMemoryHandle,
      SECTION_ALL_ACCESS,
      (POBJECT_TYPE) NULL,
      KernelMode,
      &PhysicalMemorySection,
      (POBJECT_HANDLE_INFORMATION) NULL);

    if (NT_SUCCESS(ntStatus))
    {

      pStartPhysAddress.QuadPart = (ULONGLONG)pPhysAddress;

      pEndPhysAddress = RtlLargeIntegerAdd (pStartPhysAddress,
        RtlConvertUlongToLargeInteger(PhysMemSizeInBytes));
#if 1
      IsIOSpace = 0;

      Result1 = HalTranslateBusAddress (1, 0, pStartPhysAddress, &IsIOSpace, &pStartPhysAddress);

      IsIOSpace = 0;

      Result2 = HalTranslateBusAddress (1, 0, pEndPhysAddress, &IsIOSpace, &pEndPhysAddress);

      if (Result1 && Result2)
#else
      if(1)
#endif
      {
        DbgPrint("start phys address: 0x%08lx, size: %d IsIOSpace: 0x%lx\n", (ULONG)pStartPhysAddress.LowPart, (int)PhysMemSizeInBytes, IsIOSpace);
        MappingLength = RtlLargeIntegerSubtract (pEndPhysAddress, pStartPhysAddress);

        if (MappingLength.LowPart)
        {

          // Let ZwMapViewOfSection pick a linear address

          PhysMemSizeInBytes = 0; //MappingLength.LowPart;

          ViewBase = pStartPhysAddress;

          ntStatus = ZwMapViewOfSection (*pPhysicalMemoryHandle,
            (HANDLE) -1,
            &pbPhysMemLin,
            0L,
            PhysMemSizeInBytes,
            &ViewBase,
            &PhysMemSizeInBytes,
            ViewShare,
            0,
            PAGE_READWRITE | PAGE_NOCACHE);

          if (!NT_SUCCESS(ntStatus)) {
            D(DbgPrint("ERROR: ZwMapViewOfSection failed (ntStatus=0x%lx (%s))\n",
              (long)ntStatus,
              ((ntStatus == STATUS_CONFLICTING_ADDRESSES) ? "STATUS_CONFLICTING_ADDRESSES"
              : ((ntStatus == STATUS_INVALID_PAGE_PROTECTION) ? "STATUS_INVALID_PAGE_PROTECTION"
              : ((ntStatus == STATUS_INVALID_VIEW_SIZE) ? "STATUS_INVALID_VIEW_SIZE"
              : ((ntStatus == STATUS_SECTION_PROTECTION) ? "STATUS_SECTION_PROTECTION" : "unkown"))))

              ));
          }
          else
          {
            pbPhysMemLin += (ULONG)pStartPhysAddress.LowPart - (ULONG)ViewBase.LowPart;
            *ppPhysMemLin = pbPhysMemLin;
          }  
        }
        else
          OutputDebugString ("ERROR: RtlLargeIntegerSubtract failed");
      }
      else
        OutputDebugString ("ERROR: MappingLength = 0");
    }
    else
      OutputDebugString ("ERROR: ObReferenceObjectByHandle failed");
  }
  else
    OutputDebugString ("ERROR: ZwOpenSection failed");

  if (!NT_SUCCESS(ntStatus))
    ZwClose(*pPhysicalMemoryHandle);

  OutputDebugString ("Leaving MapPhysicalMemoryToLinearSpace");

  return ntStatus;
}


NTSTATUS UnmapPhysicalMemory(HANDLE PhysicalMemoryHandle, PVOID pPhysMemLin)
{
  NTSTATUS ntStatus;

  OutputDebugString ("Entering UnmapPhysicalMemory");

  ntStatus = ZwUnmapViewOfSection((HANDLE)-1, pPhysMemLin);

  if (!NT_SUCCESS(ntStatus))
    OutputDebugString ("ERROR: UnmapViewOfSection failed");

  ZwClose(PhysicalMemoryHandle);

  OutputDebugString ("Leaving UnmapPhysicalMemory");

  return ntStatus;
}

//--------------------------------------------------------------------
void test_pci_scan()
{
  ULONG bus, dev, id;
  PPCI_COMMON_CONFIG  pci_data;
  UCHAR               buffer[PCI_COMMON_HDR_LENGTH];
  ULONG result;

  return;

  pci_data = (PPCI_COMMON_CONFIG)buffer;


  for (bus=0; bus < 8; ++bus) // TODO:XXX
    for (dev=0; dev < PCI_MAX_DEVICES; ++dev) {
      result = HalGetBusData(PCIConfiguration, bus, dev, pci_data, PCI_COMMON_HDR_LENGTH);
      if (result == 0)
        break; // bus does not exist
      else if (result == 2 && pci_data->VendorID == PCI_INVALID_VENDORID)
        continue; // bus exists, but no device exists at this virtual slot number
      else // device found
      {
        DbgPrint("res=%d, bus=%d, slot=%d, vendor_id=0x%04lx, device_id=0x%04lx, sub_class=%02x, base_class=%02x, base_addr=0x%08lx\n",
          (int)result, bus, dev,
          (int)pci_data->VendorID,
          (int)pci_data->DeviceID,
          (int)pci_data->BaseClass,
          (int)pci_data->SubClass,
          pci_data->u.type0.BaseAddresses[0]);
      }
    }

}