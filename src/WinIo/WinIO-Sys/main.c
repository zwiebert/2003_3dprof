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

#define DEVICE_NAME L"\\Device\\NVC_WinIO"
#define DOS_DEVICE_NAME L"\\DosDevices\\NVC_WinIO"
void test_pci_scan(void);


// Function definition section

NTSTATUS WinIoDispatch(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp);
void     WinIoUnload(IN PDRIVER_OBJECT DriverObject);
LONG     SstReportEvent(IN PVOID  pIoObject, IN NTSTATUS  MsgCode, IN CHAR *p_c_ArgString);



// Installable driver initialization entry point.
// This entry point is called directly by the I/O system.

NTSTATUS
DriverEntry (IN PDRIVER_OBJECT DriverObject, IN PUNICODE_STRING RegistryPath)
{
  UNICODE_STRING  DeviceNameUnicodeString;
  UNICODE_STRING  DeviceLinkUnicodeString;
  NTSTATUS        ntStatus;
  PDEVICE_OBJECT  DeviceObject = NULL;

  OutputDebugString("Entering DriverEntry");

  RtlInitUnicodeString(&DeviceNameUnicodeString, DEVICE_NAME);

  // Create an EXCLUSIVE device object (only 1 thread at a time
  // can make requests to this device).

  ntStatus = IoCreateDevice(DriverObject, 0, &DeviceNameUnicodeString, FILE_DEVICE_WINIO, 0, TRUE, &DeviceObject);

  if (NT_SUCCESS(ntStatus)) {
    // Create dispatch points for device control, create, close.

    DriverObject->MajorFunction[IRP_MJ_CREATE]
    = DriverObject->MajorFunction[IRP_MJ_CLOSE]
    = DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL]
    = WinIoDispatch;
    DriverObject->DriverUnload = WinIoUnload;

    // Create a symbolic link, e.g. a name that a Win32 app can specify
    // to open the device.

    RtlInitUnicodeString (&DeviceLinkUnicodeString, DOS_DEVICE_NAME);

    ntStatus = IoCreateSymbolicLink(&DeviceLinkUnicodeString, &DeviceNameUnicodeString);

    if (!NT_SUCCESS(ntStatus)) {
      // Symbolic link creation failed- note this & then delete the
      // device object (it's useless if a Win32 app can't get at it).

      OutputDebugString ("ERROR: IoCreateSymbolicLink failed");
      IoDeleteDevice (DeviceObject);
    }
  } else {
    OutputDebugString ("ERROR: IoCreateDevice failed");
  }

  OutputDebugString ("Leaving DriverEntry");
  return ntStatus;
}



// Delete the associated device and return

void
WinIoUnload(IN PDRIVER_OBJECT DriverObject)
{
  UNICODE_STRING DeviceLinkUnicodeString;
  NTSTATUS ntStatus;

  OutputDebugString ("Entering WinIoUnload");

  RtlInitUnicodeString (&DeviceLinkUnicodeString, DOS_DEVICE_NAME);

  ntStatus = IoDeleteSymbolicLink (&DeviceLinkUnicodeString);

  if (NT_SUCCESS(ntStatus)) {
    IoDeleteDevice (DriverObject->DeviceObject);
  } else {
    OutputDebugString ("ERROR: IoDeleteSymbolicLink");
  }

  OutputDebugString ("Leaving WinIoUnload...");
}

