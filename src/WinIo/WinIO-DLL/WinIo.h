#ifndef WINIO_H_
#define WINIO_H_

#ifdef WINIO_DLL
# define WINIO_API _declspec(dllexport)
#else
# define WINIO_API _declspec(dllimport)
#endif

typedef PVOID WinIO;

#ifdef __cplusplus
extern "C" {
#endif

  /* install / uninstall driver to system */
  WINIO_API BOOL   _stdcall  InstallWinIoDriver(PSTR pszWinIoDriverPath, BOOL IsDemandLoaded);
  WINIO_API BOOL   _stdcall  RemoveWinIoDriver();

  /* open / close driver */
  WINIO_API  PVOID  _stdcall  InitializeWinIo();
  WINIO_API  VOID   _stdcall  ShutdownWinIo(PVOID winio_handle);

  /* map / unmap physical RAM into user space (broken ?) */
  WINIO_API  PBYTE  _stdcall  MapPhysToLin(PBYTE pbPhysAddr, DWORD dwPhysSize, HANDLE *pPhysicalMemoryHandle);
  WINIO_API  BOOL   _stdcall  UnmapPhysicalMemory(HANDLE PhysicalMemoryHandle, PBYTE pbLinAddr);

  /* read / write single DWORD from/to a physical address */
  WINIO_API  BOOL   _stdcall  GetPhysLong(PBYTE pbPhysAddr, PDWORD pdwPhysVal);
  WINIO_API  BOOL   _stdcall  SetPhysLong(PBYTE pbPhysAddr, DWORD dwPhysVal);
 
  /* read / write single BYTE from/to a physical address */
  WINIO_API  BOOL   _stdcall  GetPortVal(WORD wPortAddr, PDWORD pdwPortVal, BYTE bSize);
  WINIO_API  BOOL   _stdcall  SetPortVal(WORD wPortAddr, DWORD dwPortVal, BYTE bSize);


  /* Map/UnMap physical RAM to user space. 
  (Replacement for possible broken MapPhysToLin/UnmapPhysicalMemory pair)

  Note: Both functions do nothing on Win9x, because they aren't implemented there yet. 

  PHYS_ADDR  Points to physical memory address. No alinging to pagesize is required.
  SIZE       Byte size of memory space
  VIRT_UADDR Start address to user space mapped memory (result of MapIO()). Its safe
  to pass NULL here, which causes UnMapIO() to do nothing.

  MapIO()    returns user space address.
  UnMapIO()  returns nothing
  */
  WINIO_API  PVOID  _stdcall MapIO(PVOID winio_handle, ULONG phys_addr, ULONG size); 
  WINIO_API  VOID   _stdcall UnMapIO(PVOID winio_handle, PVOID virt_uaddr);



  struct winio_pci_common_header {
    USHORT  Bus;   //Bus number
    USHORT  Slot;  //Dev+Fn=virtual slot number
    USHORT  Dev;
    USHORT  Fn;
    USHORT  VendorID;                   // (ro)
    USHORT  DeviceID;                   // (ro)
    USHORT  Command;                    // Device control
    USHORT  Status;
    UCHAR   RevisionID;                 // (ro)
    UCHAR   ProgIf;                     // (ro)
    UCHAR   SubClass;                   // (ro)
    UCHAR   BaseClass;                  // (ro)
    UCHAR   CacheLineSize;              // (ro+)
    UCHAR   LatencyTimer;               // (ro+)
    UCHAR   HeaderType;                 // (ro)
    UCHAR   BIST;                       // Built in self test

    ULONG   BaseAddresses[6];
    ULONG   CIS;
    USHORT  SubVendorID;
    USHORT  SubSystemID;
    ULONG   ROMBaseAddress;
    UCHAR   CapabilitiesPtr;
    UCHAR   Reserved1[3];
    ULONG   Reserved2;
    UCHAR   InterruptLine;      //
    UCHAR   InterruptPin;       // (ro)
    UCHAR   MinimumGrant;       // (ro)
    UCHAR   MaximumLatency;     // (ro)
  };

  WINIO_API  BOOL   _stdcall winio_get_pci_header(PVOID winio_handle, struct winio_pci_common_header *buf);


#ifdef __cplusplus
}
#endif


/* recent changes to this file:

-bw/26-Aug-2003
===============
- private stuff moved to new header file WinIO_private.h
- type of WinIO no longer visible to user. Is a generic (PVOID) pointer now.
- 2 new functions: MapIO, UnMapIO. Do the same as MapPhysToLin/UnmapPhysicalMemory but
work with addresses the old functions cannot map. The userinterface ist more easy too,
so UnMapIO() only needs the base addres returned by MapIO().

*/
#endif /* WINIO_H_ */
