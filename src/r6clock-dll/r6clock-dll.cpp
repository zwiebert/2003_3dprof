// r6clock-dll.cpp : Defines the entry point for the DLL application.
//
#include "stdafx.h"
#include "config.h"
#include "r6clock-dll.h"
#include <stdio.h>


#define EXP1 0

#ifdef USE_DRV_WIO
  #include "radcard.h"
  #include "WinIO.h"
#endif

#ifdef USE_DRV_R6P
  #include "setclk.h"
  #include "r6probe.h"
  //#pragma comment (lib, "..\\r6clock-r6setclk-lib\\Debug\\setclk.lib")
  //#pragma comment (lib, "..\\r6clock-r6probe\\Debug\\r6probe.lib")
#endif


#define DEFAULT_CORE_KHZ (0)
#define DEFAULT_RAM_KHZ (0)


int nmb_openers;


int FindAllCards(void);


BOOL
APIENTRY DllMain( HANDLE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
#ifdef USE_DRV_R6P
setclk_feature_divopt = 1;
#endif

  switch (ul_reason_for_call)
  {

  case DLL_PROCESS_ATTACH:
  case DLL_THREAD_ATTACH:
#if EXP1
    if (nmb_openers++ == 0) {
      if ( !setclk_open() ) {
        return -1;
      }
    }
#endif
    break;

  case DLL_THREAD_DETACH:
  case DLL_PROCESS_DETACH:
#if EXP1
    if (--nmb_openers == 0) {
      setclk_close();
    }
#endif
#ifdef USE_DRV_WIO
   RemoveWinIoDriver();
#endif
    break;

  }
  return TRUE;
}




R6CLOCKDLL_API r6clock_clocks r6clock_set_clock(ULONG core_khz, ULONG ram_khz)
{
  r6clock_set_get_clock(&core_khz, &ram_khz);
  struct r6clock_clocks clocks = {core_khz, ram_khz };
  return clocks;
}

R6CLOCKDLL_API r6clock_clocks r6clock_get_clock(void)
{
  struct r6clock_clocks clocks = {0, 0};
  r6clock_set_get_clock(&clocks.core_khz, &clocks.ram_khz);
  return clocks;
}

R6CLOCKDLL_API r6clock_clocks r6clock_get_default_clock(void)
{
  struct r6clock_clocks clocks = {0, 0};
  clocks.core_khz = DEFAULT_CORE_KHZ; //TODO: IMPLEMENT DEFAULT CLOCKS
  clocks.ram_khz = DEFAULT_RAM_KHZ;
  return clocks;
}




#ifdef USE_DRV_R6P


R6CLOCKDLL_API ULONG r6clock_get_id(void)
{

  if (nmb_openers == 0)
    if ( !setclk_open() )
      return 0;

  unsigned pci_id = r6probe_readpci(0);

  if (nmb_openers == 0)
    setclk_close();
  return pci_id;
}


R6CLOCKDLL_API LONG r6clock_set_get_clock(ULONG *core_khz, ULONG *ram_khz)
{
  if (nmb_openers == 0)
    if ( !setclk_open() )
    {
      return -1;
    }

    float core_mhz = *core_khz * 0.001f;
    float ram_mhz  = *ram_khz  * 0.001f;

    if (!(*core_khz == 0 && *ram_khz == 0))
      setclk_setclock(core_mhz, ram_mhz, 0);

    int locked = setclk_getclock(&core_mhz, &ram_mhz);
    *core_khz = (int) (core_mhz * 1000.0f);
    *ram_khz  = (int) (ram_mhz  * 1000.0f);
    if (locked)
      *core_khz = *ram_khz;

    if (nmb_openers == 0)
      setclk_close();
    return 0;
}

R6CLOCKDLL_API LONG  r6clockdiag_get_info(char *buf, ULONG buflen)
{
  sprintf(buf, "%s\n", "r6clockdiag_get_info() is not implemented for R6Probe config.");
  return -1;
}
#endif // USE_DRV_R6P



#ifdef USE_DRV_WIO


#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <errno.h>

#define D(x)  //(backend_debug && (1, x))
#define UD(x) //(backend_debug && (1, x)) // user debug info
#define V(x)  //(backend_debug && (1, x))

#define ERR_STREAM stderr
#define DBG_STREAM stderr
#define NO_WINIO "driver not fount"
#define HAVE_WINIO "driver found"
int backend_remove_driver;


radcard *card[10];


/* Check if the device is a videocard */
static int IsVideoCard(struct winio_pci_common_header *pci_hdr)
{
  return pci_hdr->BaseClass == 0x03;
}

bool FindCard(struct winio_pci_common_header &buf)
{
  WinIO winio_instance  = InitializeWinIo();

  if (winio_instance) {

    while (winio_get_pci_header(winio_instance, &buf))  {
      if(!IsVideoCard(&buf))
        continue; 
      if (!radcard::is_supported(buf.VendorID, buf.DeviceID))
        continue;

      ShutdownWinIo(winio_instance);
      return true;
    }
  }
  return false;
}


R6CLOCKDLL_API LONG r6clock_set_get_clock(ULONG *core_khz, ULONG *ram_khz)
{
  struct winio_pci_common_header buf = {0, };

  if (!FindCard(buf))
    return -1;

  try {
    radcard *card = new radcard(buf);

    float core_mhz = *core_khz * 0.001f;
    float ram_mhz  = *ram_khz  * 0.001f;

    if (!(*core_khz == 0 && *ram_khz == 0))
      card->setclock(core_mhz, ram_mhz, 0);

    int locked = card->getclock(&core_mhz, &ram_mhz);
    *core_khz = (int) (core_mhz * 1000.0f);
    *ram_khz  = (int) (ram_mhz  * 1000.0f);
    if (locked)
      *core_khz = *ram_khz;

    delete card;

    return 0;

  } catch (...) {
    *ram_khz = *core_khz = 0;
    return -1;
  }

}

R6CLOCKDLL_API ULONG r6clock_get_id(void)
{
  struct winio_pci_common_header buf = {0, };

  if (!FindCard(buf))
    return 0;

  unsigned pci_id = (buf.VendorID) | (buf.DeviceID << 16);
  return pci_id;
}




R6CLOCKDLL_API LONG  r6clockdiag_get_info(char *buf, ULONG buflen)
{
  struct winio_pci_common_header pci = {0, };

  if (!FindCard(pci))
    return -1;

    try {
    radcard card(pci);
    struct radcard::dividers divs;
   
    card.get_dividers(&divs);

    sprintf(buf, //buflen,
    "pci.Bus = %hu\n"
    "pci.Slot = %hu\n"
    "pci.Dev = %hu\n"
    "pci.Fn = %hu\n"
    "pci.VendorID = 0x%04hx\n"
    "pci.DeviceID = 0x%04hx\n"
    "pci.BaseAdresses = { 0x%08x, 0x%08x, 0x%08x, 0x%08x, 0x%08x, 0x%08x }\n"
    "pci.ROMBaseAddress = 0x%08x\n"
    "card.bus_id = %s\n"
    "card.bioshdr.xclk = %u\n"
    "card.bioshdr.reference_freq = %u\n"
    "card.bioshdr.reference_div = %u\n"
    "card.bioshdr.x = { 0x%04x, 0x%04x, 0x%04x, 0x%04x }\n"
    "card.bioshdr.xx = { 0x%04x, 0x%04x }\n"
    "card.bioshdr.xxx = 0x%04x\n"
    "card.get_dividers() = { %u, %u, %u, %u, %u, %u } (ref,vfb,vpost,mfb,mpost,isl)\n"
    ,(USHORT)pci.Bus
    ,(USHORT)pci.Slot
    ,(USHORT)pci.Dev
    ,(USHORT)pci.Fn
    ,(USHORT)pci.VendorID
    ,(USHORT)pci.DeviceID
    ,pci.BaseAddresses[0], pci.BaseAddresses[1], pci.BaseAddresses[2], pci.BaseAddresses[3], pci.BaseAddresses[4], pci.BaseAddresses[5]
    ,pci.ROMBaseAddress
    ,card.bus_id
    ,(LONG)card.bioshdr.xclk
    ,(LONG)card.bioshdr.reference_freq
    ,(LONG)card.bioshdr.reference_div
    ,(LONG)card.bioshdr.x[0], (LONG)card.bioshdr.x[1], (LONG)card.bioshdr.x[2], (LONG)card.bioshdr.x[3] 
    ,(LONG)card.bioshdr.xx[0], (LONG)card.bioshdr.xx[1]
    ,(LONG)card.bioshdr.xxx
      ,divs.reference, divs.vpu_feedback, divs.vpu_post, divs.mem_feedback, divs.mem_post, (unsigned)divs.is_locked
    );
    return 0;

  } catch (...) {
    return -1;
  }




}


#endif // USE_DRV_WIO
