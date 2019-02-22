// ---------------------------------------------------- //
//                      WinIo v2.0                      //
//  Direct Hardware Access Under Windows 9x/NT/2000/XP  //
//           Copyright 1998-2002 Yariv Kaplan           //
//               http://www.internals.com               //
// ---------------------------------------------------- //

#include <windows.h>
#include <winioctl.h>
#include <winio_nt.h>
#include "WinIO.h"
#include "WinIO_private.h"

BOOL   _stdcall winio_get_pci_header(PVOID winio_handle, struct winio_pci_common_header *buf)
{
  DWORD dwBytesReturned;
  if (!IsWinIoInitialized)
    return FALSE;
  if (IsNT)
  {

    if (!DeviceIoControl(hDriver, IOCTL_WINIO_GETPCICOMMHDR,
      buf, sizeof *buf,
      buf, sizeof *buf,
      &dwBytesReturned, NULL))
      return FALSE;

    if (dwBytesReturned == 0
      || buf->Bus == 0xffff)
      return FALSE;

    return TRUE;
  }
  return FALSE;
}
