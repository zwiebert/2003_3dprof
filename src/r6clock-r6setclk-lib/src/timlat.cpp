#include "setclk_inc.h"


// -----------------------------------------
//  Name: setclk_getmlat
//  Desc: Get RADEON memory latency timers
// -----------------------------------------
void  __cdecl c_setclk_setmlat(int latidx)
{
  unsigned latency = 0;

  switch(latidx) {
  case 0: latency = LATENCY_FAST; break;
  case 1: latency = LATENCY_MEDIUM; break;
  default: return;
  }

  r6probe_writemmr(RADEON_MEM_INIT_LAT_TIMER, 0xffffffff, latency);
}


int   __cdecl c_setclk_getmlat( void )
{
  unsigned data;
  data = r6probe_readmmr(RADEON_MEM_INIT_LAT_TIMER);

  switch(data) {
  case LATENCY_FAST: return 0;
  case LATENCY_MEDIUM: return 1;
  default: return data;
  }
}




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


void  __cdecl c_setclk_setmtim(int timidx)
{
  unsigned timing = 0;

  switch(timidx) {
  case 0: timing = MEMTIM_SLOW;   break;
  case 1: timing = MEMTIM_MEDIUM; break;
  case 2: timing = MEMTIM_FAST;   break;
  default: return;
  }

  r6probe_writemmr(RADEON_MEM_TIMINGS, MEMTIM_MASK, timing);
}
