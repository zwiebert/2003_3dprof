{ Unit: WinIO
  Version: 1.00
  translated from file WinIO.H
  raw translation using H2Pas v.1.21, (c) Peter Sawatzki
  fine tuned by:
    (fill in)
 
  revision history:
  Date:    Ver: Author: Mod:
  xx/xx/94 1.00 <name>  <modification>
}
Unit WinIO;
Interface
Uses
  WinTypes,
  WinProcs;
#ifndef WINIO_H_
#define WINIO_H_

#ifdef WINIO_DLL
  Function _declspec (_declspec(dllimport: dllexport) #else # define WINIO_API): # define WINIO_API; 

#ifdef __cplusplus
extern "C" {
#endif

  { install / uninstall driver to system }
  Function InstallWinIoDriver (pszWinIoDriverPath: PSTR;IsDemandLoaded: Bool): WINIO_API BOOL _stdcall;
  Function RemoveWinIoDriver (): WINIO_API BOOL _stdcall;

  { open / close driver }
  Function InitializeWinIo (): WINIO_API WinIO _stdcall;
  Function ShutdownWinIo (winio_handle: WinIO): WINIO_API VOID _stdcall;

  { map / unmap physical RAM into user space (broken ?) }
  Function MapPhysToLin (pbPhysAddr: PBYTE;dwPhysSize: LongInt;pPhysicalMemoryHandle: HANDLE *): WINIO_API PBYTE _stdcall;
  Function UnmapPhysicalMemory (PhysicalMemoryHandle: tHandle;pbLinAddr: PBYTE): WINIO_API BOOL _stdcall;

  { read / write single DWORD from/to a physical address }
  Function GetPhysLong (pbPhysAddr: PBYTE;pdwPhysVal: PDWORD): WINIO_API BOOL _stdcall;
  Function SetPhysLong (pbPhysAddr: PBYTE;dwPhysVal: LongInt): WINIO_API BOOL _stdcall;

  { read / write single BYTE from/to a physical address }
  Function GetPortVal (wPortAddr: Word;pdwPortVal: PDWORD;bSize: Byte): WINIO_API BOOL _stdcall;
  Function SetPortVal (wPortAddr: Word;dwPortVal: LongInt;bSize: Byte): WINIO_API BOOL _stdcall;


  { Map/UnMap physical RAM to user space.   (Replacement for possible broken MapPhysToLin/UnmapPhysicalMemory pair)  Note: Both functions do nothing on Win9x, because they aren't implemented there yet.   PHYS_ADDR  Points to physical memory address. No alinging to pagesize is required.  SIZE       Byte size of memory space  VIRT_UADDR Start address to user space mapped memory (result of MapIO()). Its safe  to pass NULL here, which causes UnMapIO() to do nothing.  MapIO()    returns user space address.  UnMapIO()  returns nothing  }
  Function MapIO (winio_handle: WinIO;phys_addr,size: LongInt): WINIO_API PVOID _stdcall; 
  Function UnMapIO (winio_handle: WinIO;virt_uaddr: PVOID): WINIO_API VOID _stdcall;

#ifdef __cplusplus
}
#endif


{ recent changes to this file:-bw/26-Aug-2003===============- private stuff moved to new header file WinIO_private.h- type of WinIO no longer visible to user. Is a generic (PVOID) pointer now.- 2 new functions: MapIO, UnMapIO. Do the same as MapPhysToLin/UnmapPhysicalMemory butwork with addresses the old functions cannot map. The userinterface ist more easy too,so UnMapIO() only needs the base addres returned by MapIO().}
#endif { WINIO_H_ }
Implementation
  Function GetPhysLong;                   External '?'        Index    ?;
  Function GetPortVal;                    External '?'        Index    ?;
  Function InitializeWinIo;               External '?'        Index    ?;
  Function InstallWinIoDriver;            External '?'        Index    ?;
  Function MapIO;                         External '?'        Index    ?;
  Function MapPhysToLin;                  External '?'        Index    ?;
  Function RemoveWinIoDriver;             External '?'        Index    ?;
  Function SetPhysLong;                   External '?'        Index    ?;
  Function SetPortVal;                    External '?'        Index    ?;
  Function ShutdownWinIo;                 External '?'        Index    ?;
  Function UnMapIO;                       External '?'        Index    ?;
  Function UnmapPhysicalMemory;           External '?'        Index    ?;
  Function _declspec;                     External '?'        Index    ?;
End.

--- snip --- snip --- snip ---


{Pascal verification program for WinIO }
Program VeriP;
Uses
  WinIO;

Procedure Veri (aStr: pChar; aSize: Integer);
Begin
  WriteLn('Size of ',aStr,'= ',aSize:5);
End;

Begin
  WriteLn('verification of WinIO for Pascal:');
End.



/* C verification program for WinIO */
#include <stdio.h>
#include "WinIO.h"
void veri (char *aStr, int aSize)
{ printf("Size of %s= %5i\n",aStr,aSize); }

void main (void)
{ printf("verification of WinIO for C:\n");
}
