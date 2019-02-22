#include <windows.h>
#include <stdio.h>
#include "winio.h"

PVOID inst;
int main()
{
  DWORD dwPortVal;
  DWORD dwMemVal;
  bool bResult;
  HANDLE hPhysicalMemory;
  PBYTE pbLinAddr=0;

  // Call InitializeWinIo to initialize the WinIo library.

  bResult = (inst = InitializeWinIo());

  if (bResult)
  {
    // Under Windows NT/2000/XP, after calling InitializeWinIo,
    // you can call _inp/_outp instead of using GetPortVal/SetPortVal

    //GetPortVal(0x378, &dwPortVal, 4);

    //SetPortVal(0x378, 10, 4);

    // Map physical addresses 0xA0000 - 0xAFFFF into the linear address space
    // of the application. The value returned from the call to MapPhysToLin is
    // a linear address corresponding to physical address 0xA0000. In case of
    // an error, the return value is NULL.

#if 0
    pbLinAddr = MapPhysToLin((PBYTE)0xA0002, 4097, &hPhysicalMemory);
#elif 0
    pbLinAddr = MapPhysToLin((PBYTE)0xDE000000, 128, &hPhysicalMemory);
#else
    void *handle;
    void *addr = MapIO(inst, 0xDE000004, 128);
    if (addr) {
      printf("Mapping Success (virt_uaddr=0x%08lx)\n", addr);
      UnMapIO(inst, addr);
    } else {
      printf("Mapping Failure\n");
    }
#endif

    printf("pbLinAddr = 0x%08lx\n", pbLinAddr);
    if (pbLinAddr)
    {
      // Now we can use pbLinAddr to access physical address 0xA0000

      *pbLinAddr = 10;

      // When you're done with pbLinAddr, call UnmapPhysicalMemory

      UnmapPhysicalMemory(hPhysicalMemory, pbLinAddr);
    }

    // Instead of using MapPhysToLin, we can use GetPhysLong/SetPhysLong

    GetPhysLong((PBYTE)0xA0000, &dwMemVal);

    SetPhysLong((PBYTE)0xA0000, 10);

    // When you're done using WinIo, call ShutdownWinIo

    ShutdownWinIo(inst);
  }
  else
  {
    printf("Error during initialization of WinIo.\n");
    return 1;
  }
  getc(stdin);
  return 0;
}

