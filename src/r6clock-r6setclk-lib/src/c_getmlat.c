#include "setclk_inc.h"


// -----------------------------------------
//  Name: setclk_setmlat
//  Desc: Set RADEON memory latency timers
// -----------------------------------------
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
