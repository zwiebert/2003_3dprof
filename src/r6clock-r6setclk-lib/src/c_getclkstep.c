#include "setclk_inc.h"

/*
; --------------------------------------
;  Name: setclk_getclock
;  Desc: Get RADEON clock settings
; --------------------------------------
*/
float __cdecl c_setclk_getclockstep(void)
{
  unsigned hw_reg, reference_divider;
  float clock_step;

  hw_reg = r6probe_readpll(RADEON_X_MPLL_REF_FB_DIV);
  reference_divider = GET_REF_DIV(hw_reg);

  clock_step = setclk_PLLData.reference_freq / reference_divider;

  return clock_step;
}
