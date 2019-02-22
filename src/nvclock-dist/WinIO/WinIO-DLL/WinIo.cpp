// ---------------------------------------------------- //
//                      WinIo v2.0                      //
//  Direct Hardware Access Under Windows 9x/NT/2000/XP  //
//           Copyright 1998-2002 Yariv Kaplan           //
//               http://www.internals.com               //
// ---------------------------------------------------- //

#include <windows.h>
#include <winioctl.h>
#include "k32call.h"
#include "phys32.h"
#include <winio_nt.h>
#include "WinIO.h"
#include "WinIO_private.h"


#define ENABLE_DIRECTIO 0 // 0 disables, 1 enables: _inp/_oup on NT for this process


HANDLE hDriver = INVALID_HANDLE_VALUE;
BOOL IsNT;
BOOL IsWinIoInitialized = false;
char szWinIoDriverPath[MAX_PATH];


bool IsWinNT()
{
  OSVERSIONINFO OSVersionInfo;

  OSVersionInfo.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
  GetVersionEx(&OSVersionInfo);
  return OSVersionInfo.dwPlatformId == VER_PLATFORM_WIN32_NT;
}


bool GetDriverPath()
{
  PSTR pszSlash;

  if (!GetModuleFileName(GetModuleHandle(NULL), szWinIoDriverPath, sizeof(szWinIoDriverPath)))
    return false;

  pszSlash = strrchr(szWinIoDriverPath, '\\');
  if (pszSlash)
    pszSlash[1] = 0;
  else
    return false;

  strcat(szWinIoDriverPath, "winio.sys");

  return true;
}



HANDLE _stdcall InitializeWinIo()
{
  IsNT = IsWinNT();

  if (IsNT)
  {
    hDriver = CreateFile("\\\\.\\WINIO",
                         GENERIC_READ | GENERIC_WRITE,
                         0,
                         NULL,
                         OPEN_EXISTING,
                         FILE_ATTRIBUTE_NORMAL,
                         NULL);

    // If the driver is not running, install it
    if (hDriver == INVALID_HANDLE_VALUE)
    {
      GetDriverPath();

      if (!InstallWinIoDriver(szWinIoDriverPath, TRUE))
        return NULL;
      if (!StartWinIoDriver())
        return NULL;


      hDriver = CreateFile("\\\\.\\WINIO",
                           GENERIC_READ | GENERIC_WRITE,
                           0,
                           NULL,
                           OPEN_EXISTING,
                           FILE_ATTRIBUTE_NORMAL,
                           NULL);

      if (hDriver == INVALID_HANDLE_VALUE)
        return NULL;
    }

#if ENABLE_DIRECTIO
    // Enable I/O port access for this process
    if (!DeviceIoControl(hDriver, IOCTL_WINIO_ENABLEDIRECTIO, NULL,
                         0, NULL, 0, &dwBytesReturned, NULL))
      return NULL;
#endif

  }
  else
  {
    VxDCall = (DWORD (WINAPI *)(DWORD,DWORD,DWORD))GetK32ProcAddress(1);

    hDriver = CreateFile("\\\\.\\WINIO.VXD", 0, 0, 0, CREATE_NEW, FILE_FLAG_DELETE_ON_CLOSE, 0);

    if (hDriver == INVALID_HANDLE_VALUE)
      return NULL;
  }

  IsWinIoInitialized = true;

  return (WinIO)calloc(sizeof(struct winio_instance), 1);
}


void _stdcall ShutdownWinIo(WinIO vp_inst)
{
  if (IsNT)
  {
    if (hDriver != INVALID_HANDLE_VALUE)
    {
#if ENABLE_DIRECTIO
      // Disable I/O port access
      DeviceIoControl(hDriver, IOCTL_WINIO_DISABLEDIRECTIO, NULL,
                      0, NULL, 0, &dwBytesReturned, NULL);
#endif
      CloseHandle(hDriver);
    }

    RemoveWinIoDriver();
  }
  else
    CloseHandle(hDriver);

  IsWinIoInitialized = false;
  free(vp_inst);
}
