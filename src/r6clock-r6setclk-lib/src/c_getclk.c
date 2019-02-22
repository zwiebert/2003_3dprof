#include "setclk_inc.h"



// --------------------------------------
//  Name: setclk_getclock
//  Desc: Get RADEON clock settings
// --------------------------------------

int post_divider_lut[8] = {
  /*0:*/0, /*1:*/1, /*2:*/2, /*3:*/4, /*4:*/8, /*5:*/3, /*6:*/6, /*7:*/12
};
int post_divider_set_lut[13] = {
  /*0:*/0, /*1:*/1, /*2:*/2, /*3:*/5, /*4:*/3, /*5:*/0,
  /*6:*/6, /*7:*/0, /*8:*/4,  /*9:*/0, /*10:*/0, /*11:*/0, /*12:*/7
};

int __cdecl c_setclk_getclock(float* vpuclk, float* memclk)
{
  unsigned vpu_clock, mem_clock;
  int is_locked;

  is_locked = r6clock_getclock(&vpu_clock, &mem_clock);

  if (!is_locked)
    *vpuclk = (float)vpu_clock * .001f;
  *memclk = (float)mem_clock * .001f;

  return is_locked;
}

// get clocks in kHz
int __cdecl
r6clock_getclock(unsigned *vpuclk, unsigned *memclk)
{
  unsigned hw_reg,
    reference_divider,
    vpu_feedback_divider, vpu_post_divider,
    mem_feedback_divider, mem_post_divider;
  unsigned char is_locked; // if mem and vpu clock are coupled
  unsigned mem_clock=0, vpu_clock=0;

  hw_reg = r6probe_readpll(RADEON_XCLK_CNTL);
  is_locked        = ((hw_reg & 0x7) == 0x7);
  vpu_post_divider = GET_VPU_POST_DIV(hw_reg);

  hw_reg = r6probe_readpll(RADEON_MCLK_CNTL);
  mem_post_divider = GET_MEM_POST_DIV(hw_reg);

  hw_reg = r6probe_readpll(RADEON_X_MPLL_REF_FB_DIV);
  reference_divider    = GET_REF_DIV(hw_reg);
  vpu_feedback_divider = GET_VPU_FB_DIV(hw_reg);
  mem_feedback_divider = GET_MEM_FB_DIV(hw_reg);

  printf("debug_info: ref_div=%u, mem_fb_div=%u, vpu_fb_div=%u\n", reference_divider, mem_feedback_divider, vpu_feedback_divider);


  if ((reference_divider * mem_post_divider) != 0)
    mem_clock =  CALC_MEM_CLOCK(10 * bioshdr.reference_freq, reference_divider, mem_post_divider, mem_feedback_divider);
  if ((reference_divider * vpu_post_divider) != 0)
    vpu_clock =  CALC_VPU_CLOCK(10 * bioshdr.reference_freq, reference_divider, vpu_post_divider, vpu_feedback_divider);

  *memclk = mem_clock;
  if (!is_locked)
    *vpuclk = vpu_clock;

  return is_locked;

}