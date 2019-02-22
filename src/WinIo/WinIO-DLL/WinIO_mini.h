
 /* install / uninstall driver to system */
  _declspec(dllimport) BOOL  _stdcall  InstallWinIoDriver(PSTR pszWinIoDriverPath, BOOL IsDemandLoaded);
  _declspec(dllimport) BOOL  _stdcall  RemoveWinIoDriver();

  /* open / close driver */
  _declspec(dllimport)  PVOID _stdcall  InitializeWinIo();
  _declspec(dllimport)  VOID  _stdcall  ShutdownWinIo(PVOID winio_handle);

  /* map / unmap physical memory to user space */
  _declspec(dllimport)  PVOID _stdcall MapIO(PVOID winio_handle, ULONG phys_addr, ULONG size); 
  _declspec(dllimport)  VOID  _stdcall UnMapIO(PVOID winio_handle, PVOID virt_uaddr);
