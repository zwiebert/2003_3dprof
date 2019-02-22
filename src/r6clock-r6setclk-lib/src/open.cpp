#include "setclk_inc.h"
// ############################################################################

BIOSPLLINFO bioshdr;

// --------------------------------------
//  Name: r6probe_open
//  Desc: Opens connection to r6probe
// --------------------------------------
int   __cdecl c_setclk_open()
{

  
  unsigned data;
  unsigned addr;

  if (!r6probe_open ())
    return 0;


  data = r6probe_readbios(R6_BIOS_HEADER_LOC);
  addr = (data & 0xffff) + 0x30;  // offset to pll info ptr
  data = r6probe_readbios(addr);
  addr = (data & 0xffff);
  addr = r6probe_readbiosblk(&bioshdr, addr, sizeof bioshdr);

  setclk_PLLData.reference_freq = bioshdr.reference_freq * 0.01f;
  setclk_PLLData.xclk           = bioshdr.xclk * 0.01f;
  setclk_PLLData.min_freq       = bioshdr.min_freq * 0.01f;
  setclk_PLLData.max_freq       = bioshdr.max_freq * 0.01f;
  setclk_PLLData.reference_div  = bioshdr.reference_div;

  hack_bios_ref_freq();

  return 1;
}



void  __cdecl c_setclk_close()
{
  r6probe_close();
}
