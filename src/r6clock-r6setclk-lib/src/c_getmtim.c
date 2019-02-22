#include "setclk_inc.h"


// -----------------------------------------
//  Name: setclk_getmlat
//  Desc: Get RADEON memory latency timers
// -----------------------------------------
int   __cdecl c_setclk_getmtim( void )
{
  unsigned data;
  data = r6probe_readmmr(RADEON_MEM_TIMINGS);

  switch(data & MEMTIM_MASK) {
  case (MEMTIM_SLOW & MEMTIM_MASK):   return 0;
  case (MEMTIM_MEDIUM & MEMTIM_MASK): return 1;
  case (MEMTIM_FAST & MEMTIM_MASK):   return 2;
  default: return data;
  }
}
