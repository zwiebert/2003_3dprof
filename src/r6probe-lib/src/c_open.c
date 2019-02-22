/*
Welcome to R6Probe.

What does this do? Well, r6probe allows you reading and writing to
some registers of a Radeon graphic board. The R6Probe library acts
as a wrapper around the R6Probe driver. It handles opening/closing
of the driver and provides wrapper functions for device-IO controls.

The BIOS ROM on the card can also be read through this
library. Writing to BIOS ROM is not possible of course.

Which programs can use this software? There is SetClk library, which
implements an API to read and modify clock freqencies on Radon cards.

SetClk library is used by SetClk.exe, which is a commandline
clocking utility.

R6Probe driver, R6Probe library, SetClk library and SetClk.exe are
all included in this package.

*/

#include <r6probe.h>
#include <windows.h>

#define REMOVE_DRIVER 0     // if 1, demand loaded service is removed by close()
#define ALLOW_SELFINSTALL 1 // if 1, a new demand loaded service is installed if driver cannot be opened

#define DRV_SERVICE_NAME "R6ProbeDemand"
#define DRV_SERVICE_DISPLAY "R6ProbeDemand"
#define DRV_DEVICE_NAME "\\\\.\\" "r6probe" // must be the same string as in driver source
#define DRV_BINARY_NAME "r6probe.sys"


// exported procedures
int __cdecl r6probe_open(void);
void __cdecl r6probe_close(void);

// local procedures
static BOOL _stdcall remove_driver(void);
static BOOL _stdcall install_driver(const char *driver_path, BOOL IsDemandLoaded);
static BOOL _stdcall stop_driver(void);
static BOOL _stdcall start_driver(void);
static BOOL IsWinNT(void);
static BOOL isWinNT;
static BOOL driver_was_installed_by_us = FALSE;
static const char *build_driver_path(void);
static void print_error();

// local data
static const char *r6probe_path;

#define CREATE_FILE(name) CreateFile((name), GENERIC_READ, 0, 0, OPEN_EXISTING, 0, 0)
// implementation
int __cdecl r6probe_open()
{
  extern HANDLE r6probe_hProbe;
  BOOL terminate = FALSE;
  DWORD last_error = 0;

  isWinNT = IsWinNT();

  while (1) {
    r6probe_hProbe = CREATE_FILE(isWinNT ? DRV_DEVICE_NAME ".vxd" : DRV_DEVICE_NAME ".vxd");

#if !ALLOW_SELFINSTALL
    break;
#endif
    if ((r6probe_hProbe != INVALID_HANDLE_VALUE) || terminate || !isWinNT)
      break;

    // try to install driver by ourself
    r6probe_path = build_driver_path();

    if (!install_driver(r6probe_path, TRUE))
      break;
    if (!start_driver()) {
      break;
    }
    terminate = TRUE;
  }

  if (r6probe_hProbe == INVALID_HANDLE_VALUE) {
    last_error = GetLastError();
    print_error();
    return 0;
  }

  return 1;
}


void __cdecl r6probe_close()
{
  extern HANDLE r6probe_hProbe;

  if (r6probe_hProbe == INVALID_HANDLE_VALUE)
    return;

  CloseHandle(r6probe_hProbe);
#if REMOVE_DRIVER
  remove_driver();
#endif
}


static BOOL IsWinNT()
{
  OSVERSIONINFO OSVersionInfo;

  OSVersionInfo.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
  GetVersionEx(&OSVersionInfo);
  return OSVersionInfo.dwPlatformId == VER_PLATFORM_WIN32_NT;
}


static BOOL _stdcall install_driver(const char *driver_path, BOOL IsDemandLoaded)
{
  SC_HANDLE hSCManager;
  SC_HANDLE hService;

  // Remove any previous instance of the driver

  remove_driver();

  hSCManager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);

  if (hSCManager) {
    // Install the driver

    hService = CreateService(hSCManager,
      DRV_SERVICE_NAME,
      DRV_SERVICE_DISPLAY,
      SERVICE_ALL_ACCESS,
      SERVICE_KERNEL_DRIVER,
      (IsDemandLoaded ? SERVICE_DEMAND_START :
    SERVICE_SYSTEM_START),
      SERVICE_ERROR_NORMAL, driver_path, NULL, NULL, NULL, NULL, NULL);

    CloseServiceHandle(hSCManager);

    if (hService == NULL)
      return FALSE;
  } else
    return FALSE;

  CloseServiceHandle(hService);

  return TRUE;
}


static BOOL _stdcall remove_driver()
{
  SC_HANDLE hSCManager;
  SC_HANDLE hService;
  BOOL bResult;

  stop_driver();

  hSCManager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);

  if (hSCManager) {
    hService = OpenService(hSCManager, DRV_SERVICE_NAME, SERVICE_ALL_ACCESS);
    CloseServiceHandle(hSCManager);

    if (hService) {
      bResult = DeleteService(hService);
      CloseServiceHandle(hService);
    } else
      return FALSE;
  } else
    return FALSE;

  return bResult;
}


static BOOL _stdcall start_driver()
{
  SC_HANDLE hSCManager;
  SC_HANDLE hService;
  BOOL bResult;

  hSCManager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);

  if (hSCManager) {
    hService = OpenService(hSCManager, DRV_SERVICE_NAME, SERVICE_ALL_ACCESS);
    CloseServiceHandle(hSCManager);
    if (hService) {
      bResult = StartService(hService, 0, NULL)
        || GetLastError() == ERROR_SERVICE_ALREADY_RUNNING;

      CloseServiceHandle(hService);
    } else
      return FALSE;
  } else
    return FALSE;

  return bResult;
}


static BOOL _stdcall stop_driver()
{
  SC_HANDLE hSCManager;
  SC_HANDLE hService;
  SERVICE_STATUS ServiceStatus;
  BOOL bResult;

  hSCManager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);

  if (hSCManager) {
    hService = OpenService(hSCManager, DRV_SERVICE_NAME, SERVICE_ALL_ACCESS);

    CloseServiceHandle(hSCManager);

    if (hService) {
      bResult = ControlService(hService, SERVICE_CONTROL_STOP, &ServiceStatus);

      CloseServiceHandle(hService);
    } else
      return FALSE;
  } else
    return FALSE;

  return bResult;
}


static const char *build_driver_path()
{
  static char buf[256];

  PSTR Slash;

  if (!GetModuleFileName(GetModuleHandle(NULL), buf, sizeof buf))
    return 0;

  if (!(Slash = strrchr(buf, '\\')))
    return 0;

  Slash[1] = 0;
  strcat(buf, DRV_BINARY_NAME);

  return buf;
}

#include <stdio.h>
static void print_error()
{
  LPSTR lpMsgBuf;
  FormatMessage( 
    FORMAT_MESSAGE_ALLOCATE_BUFFER | 
    FORMAT_MESSAGE_FROM_SYSTEM | 
    FORMAT_MESSAGE_IGNORE_INSERTS,
    NULL,
    GetLastError(),
    MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
    &lpMsgBuf,
    0,
    NULL 
    );
  fprintf (stderr, "r6probe.lib: error: %s\n", lpMsgBuf);
  LocalFree( lpMsgBuf );
}
