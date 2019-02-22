
#include <ntddk.h>
#include <string.h>
#include "winio_nt.h"
#include <stdio.h>

#define D(x) //x
#define OutputDebugString(txt) D(DbgPrint(txt))

#define DEVICE_NAME L"\\Device\\NVC_WinIO"
#define DOS_DEVICE_NAME L"\\DosDevices\\NVC_WinIO"
void test_pci_scan(void);


// Function definition section
// -----------------------------------------------------------------
NTSTATUS WinIoDispatch(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp);
void WinIoUnload(IN PDRIVER_OBJECT DriverObject);
NTSTATUS UnmapPhysicalMemory(HANDLE PhysicalMemoryHandle, PVOID pPhysMemLin);
NTSTATUS MapPhysicalMemoryToLinearSpace(PVOID pPhysAddress,
                                        ULONG PhysMemSizeInBytes,
                                        PVOID *ppPhysMemLin,
                                        HANDLE *pPhysicalMemoryHandle);

LONG SstReportEvent(IN PVOID  pIoObject, IN NTSTATUS  MsgCode, IN CHAR *p_c_ArgString);



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
