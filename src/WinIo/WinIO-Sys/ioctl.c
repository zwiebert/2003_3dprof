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

#define IOPM_SIZE 0x2000
typedef char IOPM[IOPM_SIZE];
IOPM *pIOPM = NULL;

void      Ke386SetIoAccessMap(int, IOPM *);
void      Ke386QueryIoAccessMap(int, IOPM *);
void      Ke386IoSetAccessProcess(PEPROCESS, int);
NTSTATUS  WinIoDispatch(IN PDEVICE_OBJECT DeviceObject, IN PIRP irp);
void      WinIoUnload(IN PDRIVER_OBJECT DriverObject);
LONG      SstReportEvent(IN PVOID  pIoObject, IN NTSTATUS  MsgCode, IN CHAR *p_c_ArgString);


void unmapio(struct winio_map_io_data *datap)
{
#define data (*datap)
  OutputDebugString("IOCTL_WINIO_UNMAPIO");

  if (data.virt_uaddr)
      MmUnmapLockedPages(data.virt_uaddr, data.mdl);
    if (data.mdl)
      IoFreeMdl (data.mdl);
    if (data.virt_kaddr)
      MmUnmapIoSpace(data.virt_kaddr, data.size_requested);

#undef data
}



// Process the IRPs sent to this device

NTSTATUS
WinIoDispatch(IN PDEVICE_OBJECT DeviceObject, IN PIRP irp)
{
  PIO_STACK_LOCATION      irpStack;
  unsigned                inBufSize;
  unsigned                outBufSize;
  ULONG                   ioCtrlCode;
  PVOID                   ioBuf;
  NTSTATUS                ntStatus;

  OutputDebugString ("Entering WinIoDispatch");

  // Init to default settings

  irp->IoStatus.Status      = STATUS_SUCCESS;
  irp->IoStatus.Information = 0;
  irpStack                  = IoGetCurrentIrpStackLocation(irp);


  // Get the pointer to the input/output buffer and it's length

  ioBuf      = irp->AssociatedIrp.SystemBuffer;
  inBufSize  = irpStack->Parameters.DeviceIoControl.InputBufferLength;
  outBufSize = irpStack->Parameters.DeviceIoControl.OutputBufferLength;

  switch (irpStack->MajorFunction)
  {

  case IRP_MJ_DEVICE_CONTROL:
    OutputDebugString("IRP_MJ_DEVICE_CONTROL");
    ioCtrlCode = irpStack->Parameters.DeviceIoControl.IoControlCode;

    switch (ioCtrlCode) {


    case IOCTL_WINIO_UNMAPIO:
      OutputDebugString("IOCTL_WINIO_UNMAPIO");

      if (inBufSize == 0) {
        irp->IoStatus.Status = STATUS_INVALID_PARAMETER;
        break;
      }
      
      {
        struct winio_map_io_data data;

        memcpy (&data, ioBuf, inBufSize);

        if (data.virt_uaddr)    MmUnmapLockedPages(data.virt_uaddr, data.mdl);
        if (data.mdl)           IoFreeMdl(data.mdl);
        if (data.virt_kaddr)    MmUnmapIoSpace(data.virt_kaddr, data.size_requested);
      }
      irp->IoStatus.Status = STATUS_SUCCESS;
      OutputDebugString("Leaving IOCTL_WINIO_UNMAPIO");
      break;



    case IOCTL_WINIO_MAPIO:
      OutputDebugString("IOCTL_WINIO_MAPIO");

      if (inBufSize == 0) {
        irp->IoStatus.Status = STATUS_INVALID_PARAMETER;
        break;
      }
      
      {
        struct winio_map_io_data  data;
        PHYSICAL_ADDRESS          pa;
        ULONG                     size;
        PVOID                     virt_kaddr = NULL;
        PVOID                     virt_uaddr = NULL;
        PMDL                      mdl        = 0;
        unsigned i;

        memcpy(&data, ioBuf, inBufSize);

        pa.QuadPart = (ULONGLONG)data.phys_addr_requested;
        size = data.size_requested;

        if ((virt_kaddr = MmMapIoSpace(pa, size, MmNonCached))) {
          if ((mdl = IoAllocateMdl(virt_kaddr, size, FALSE, FALSE, NULL))) {
            MmBuildMdlForNonPagedPool(mdl);
            if ((virt_uaddr = MmMapLockedPagesSpecifyCache(mdl, UserMode, MmNonCached, NULL, FALSE, NormalPagePriority))) {
              // Success.
              data.virt_uaddr = virt_uaddr;
              data.virt_kaddr = virt_kaddr;
              data.mdl        = mdl;

              memcpy (ioBuf, &data, inBufSize);

              irp->IoStatus.Information = inBufSize;
              irp->IoStatus.Status      = STATUS_SUCCESS;
              OutputDebugString("Leaving IOCTL_WINIO_MAPIO");
              break;
            }
          }
        }

        // Failure. Cleanup the mess.
        if (virt_uaddr)  MmUnmapLockedPages(virt_uaddr, mdl);
        if (mdl)         IoFreeMdl(mdl);
        if (virt_kaddr)  MmUnmapIoSpace(virt_kaddr, size);
      }
      irp->IoStatus.Status = STATUS_INVALID_PARAMETER; // TODO
      break;



    case IOCTL_WINIO_GETPCICOMMHDR:
      OutputDebugString("IOCTL_WINIO_GETPCICOMMHDR");

      if (inBufSize != sizeof (struct winio_pci_common_header_)) {
        irp->IoStatus.Status = STATUS_INVALID_PARAMETER;
        break;
      }
      
      {
        struct winio_pci_common_header_ pci_hdr;
        enum {                          pci_data_size = ((sizeof pci_hdr) - FIELD_OFFSET(struct winio_pci_common_header_, VendorID)) };
        PPCI_COMMON_CONFIG              pci_data;

        memcpy(&pci_hdr, ioBuf, min(sizeof pci_hdr, inBufSize));
        pci_data = (PPCI_COMMON_CONFIG)&pci_hdr.VendorID;

        // If this is not first call, continue iteration at same place where we left last time
        if (pci_data->VendorID != 0)                                            goto next_fn; // -*- jump into loop -*-

        for (pci_hdr.Bus    = 0;  pci_hdr.Bus < 8;                 ++pci_hdr.Bus) {
          for (pci_hdr.Dev  = 0;  pci_hdr.Dev < PCI_MAX_DEVICES;   ++pci_hdr.Dev) {
            for (pci_hdr.Fn = 0;  pci_hdr.Fn  < PCI_MAX_FUNCTION;  ++pci_hdr.Fn)  {

              ULONG           result;
              PCI_SLOT_NUMBER slot_number = {0};

              slot_number.u.bits.DeviceNumber   = pci_hdr.Dev;
              slot_number.u.bits.FunctionNumber = pci_hdr.Fn;


              // skip functions above 0 for single function devices
              if (pci_hdr.Fn > 0 && !(pci_hdr.HeaderType & PCI_MULTIFUNCTION))  goto next_dev; // no more functions

              result = HalGetBusData(PCIConfiguration, pci_hdr.Bus, slot_number.u.AsULONG, pci_data, pci_data_size);

              if (result == 2 && pci_data->VendorID == PCI_INVALID_VENDORID)    goto next_fn;  // device or function does not exist
              else if (result == 0)                                             goto next_bus; // bus does not exist
              else                                                              goto found_fn; // Device found

next_fn:  continue; } // fn
next_dev: continue; } // dev
next_bus: continue; } // bus


        pci_hdr.Bus = ~0;  // no more busses. done.
found_fn:
        memcpy(ioBuf, &pci_hdr, min(sizeof pci_hdr, inBufSize));
        irp->IoStatus.Information = inBufSize;
        irp->IoStatus.Status      = STATUS_SUCCESS;
      }

      OutputDebugString("Leaving IOCTL_WINIO_GETPCICOMMHDR");
      break;


    default:
      OutputDebugString("ERROR: Unknown IRP_MJ_DEVICE_CONTROL");
      irp->IoStatus.Status = STATUS_INVALID_PARAMETER;
      break;
    }

    break; // IRP_MJ_DEVICE_CONTROL

    
  case IRP_MJ_CREATE:
    OutputDebugString("IRP_MJ_CREATE");
    break;


  case IRP_MJ_CLOSE:
    OutputDebugString("IRP_MJ_CLOSE");
    break;

  }

  // save status before releasing irp by IoCompleteRequest()
  ntStatus = irp->IoStatus.Status;

  IoCompleteRequest (irp, IO_NO_INCREMENT);
  OutputDebugString("Leaving WinIoDispatch");

  // We never have pending operation so always return the status code.
  return ntStatus;
}

