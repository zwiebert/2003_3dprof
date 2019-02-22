// ---------------------------------------------------- //
//                      WinIo v2.0                      //
//  Direct Hardware Access Under Windows 9x/NT/2000/XP  //
//           Copyright 1998-2002 Yariv Kaplan           //
//               http://www.internals.com               //
// ---------------------------------------------------- //

#include <windows.h>
#include "WinIO.h"
#include "WinIO_private.h"

BOOL _stdcall InstallWinIoDriver(PSTR pszWinIoDriverPath, BOOL IsDemandLoaded)
{
  SC_HANDLE hSCManager;
  SC_HANDLE hService;

  // Remove any previous instance of the driver

  RemoveWinIoDriver();

  hSCManager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);

  if (hSCManager)
  {
    // Install the driver
    
    hService = CreateService(hSCManager,
                             DRV_SERVICE_NAME,
                             DRV_SERVICE_DISPLAY,
                             SERVICE_ALL_ACCESS,
                             SERVICE_KERNEL_DRIVER,
                             (IsDemandLoaded ? SERVICE_DEMAND_START : SERVICE_SYSTEM_START),
                             SERVICE_ERROR_NORMAL,
                             pszWinIoDriverPath,
                             NULL,
                             NULL,
                             NULL,
                             NULL,
                             NULL);

    CloseServiceHandle(hSCManager);

    if (hService == NULL)
      return false;
  }
  else
    return false;

  CloseServiceHandle(hService);
  
  return true;
}


BOOL _stdcall RemoveWinIoDriver()
{
  SC_HANDLE hSCManager;
  SC_HANDLE hService;
  BOOL bResult;

  StopWinIoDriver();

  hSCManager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);

  if (hSCManager)
  {
    hService = OpenService(hSCManager, DRV_SERVICE_NAME, SERVICE_ALL_ACCESS);

    CloseServiceHandle(hSCManager);

    if (hService)
    {
      bResult = DeleteService(hService);
      
      CloseServiceHandle(hService);
    }
    else
      return false;
  }
  else
    return false;

  return bResult;
}


BOOL _stdcall StartWinIoDriver()
{
  SC_HANDLE hSCManager;
  SC_HANDLE hService;
  bool bResult;

  hSCManager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);

  if (hSCManager)
  {
    hService = OpenService(hSCManager, DRV_SERVICE_NAME, SERVICE_ALL_ACCESS);

    CloseServiceHandle(hSCManager);

    if (hService)
    {
      bResult = StartService(hService, 0, NULL) || GetLastError() == ERROR_SERVICE_ALREADY_RUNNING;

      CloseServiceHandle(hService);
    }
    else
      return false;
  }
  else
    return false;

  return bResult;
}


BOOL _stdcall StopWinIoDriver()
{
  SC_HANDLE hSCManager;
  SC_HANDLE hService;
  SERVICE_STATUS ServiceStatus;
  BOOL bResult;

  hSCManager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);

  if (hSCManager)
  {
    hService = OpenService(hSCManager, DRV_SERVICE_NAME, SERVICE_ALL_ACCESS);

    CloseServiceHandle(hSCManager);

    if (hService)
    {
      bResult = ControlService(hService, SERVICE_CONTROL_STOP, &ServiceStatus);

      CloseServiceHandle(hService);
    }
    else
      return false;
  }
  else
    return false;

  return bResult;
}


