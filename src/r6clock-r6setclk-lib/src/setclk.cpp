#include "setclk_inc.h"
#include <Windows.h>

#define MAX_FB_DIV 150   // upto 255
#define MIN_FB_DIV 2     // at least 2
#define MAX_REF_DIV 150  // upto 255 
#define MIN_REF_DIV 4    // at least 2

#define MAX_KHZ_DIFF 8000  // clocks have to be higher than DESIRED_KHZ - MAX_KHZ_DIFF


//#define EXP_FORCED_REF_FREQ // force reference freq to 2700
#define EXP_MOB  // enable code to deal with RAM-less cards
//#define EXP_SIM_MOB // fake a card with no local RAM



static void  __cdecl imp_c_setclk_setclock(float desired_vpuclk, float desired_memclk, int locked);

static int getclock(ULONG *vpuclk, ULONG *memclk);
static int getclock(float* vpuclk, float* memclk);
static BOOL calc_optimal_dividers_for_given_clock(unsigned desired_vpu_khz, unsigned desired_mem_khz,
                                                   unsigned vpu_post_divider, unsigned mem_post_divider,
                                                   unsigned *out_reference_divider,
                                                   unsigned *out_vpu_feedback_divider,
                                                   unsigned *out_mem_feedback_divider);
static void imp_setclock(float desired_vpuclk, float desired_memclk, int locked);


struct dividers {
  unsigned reference,
    vpu_feedback, vpu_post,
    mem_feedback, mem_post;
  BOOL is_locked;
};



int post_divider_lut[8] = {
  /*0:*/0, /*1:*/1, /*2:*/2, /*3:*/4, /*4:*/8, /*5:*/3, /*6:*/6, /*7:*/12
};
int post_divider_set_lut[13] = {
  /*0:*/0, /*1:*/1, /*2:*/2, /*3:*/5, /*4:*/3, /*5:*/0,
  /*6:*/6, /*7:*/0, /*8:*/4,  /*9:*/0, /*10:*/0, /*11:*/0, /*12:*/7
};


inline unsigned get_ref_khz()
{
  return (unsigned)(setclk_PLLData.reference_freq * 1000.0); 
}


void get_dividers(struct dividers *divs)
{
  unsigned hw_reg;

  hw_reg = r6probe_readpll(RADEON_XCLK_CNTL);
  divs->is_locked    = ((hw_reg & 0x7) == 0x7);
  divs->vpu_post     = GET_VPU_POST_DIV(hw_reg);

  hw_reg = r6probe_readpll(RADEON_MCLK_CNTL);
  divs->mem_post     = GET_MEM_POST_DIV(hw_reg);

  hw_reg = r6probe_readpll(RADEON_X_MPLL_REF_FB_DIV);
  divs->reference    = GET_REF_DIV(hw_reg);
  divs->vpu_feedback = GET_VPU_FB_DIV(hw_reg);
  divs->mem_feedback = GET_MEM_FB_DIV(hw_reg);


#ifdef EXP_SIM_MOB
  divs->mem_feedback = 0;
  divs->mem_post     = 0;
#endif
}

void set_dividers(struct dividers *divs)
{
#ifdef EXP_SIM_MOB
  return;
#endif
  unsigned hw_reg;
    // build new register content
  SET_REF_DIV(hw_reg,    divs->reference);
  SET_MEM_FB_DIV(hw_reg, divs->mem_feedback);
  SET_VPU_FB_DIV(hw_reg, divs->vpu_feedback);

  // write register
  r6probe_writepll(RADEON_X_MPLL_REF_FB_DIV, 0x00ffffff, hw_reg);

#if 0
  // writing locked state to R300 may crash (?)
  r6probe_writepll(RADEON_XCLK_CNTL, 7, (divs->is_locked ? 7 : 2));
#endif
}



void  c_setclk_setclock(float desired_vpuclk, float desired_memclk, int locked)
{
  float vpuclk, memclk;


  // do clocking in step-by-step to avoid screen corruption
  if (!locked && !getclock(&vpuclk, &memclk))
  {

#ifdef EXP_MOB
    if (memclk == 0)
      desired_memclk = 0.0;
#endif
    for (;;) {
      int vpu_increment = 0;
      int mem_increment = 0;
#define CLK_INC 8
#define SLEEP_MS 15

      if (desired_vpuclk != 0.0 && abs((int)(vpuclk - desired_vpuclk)) > CLK_INC)
        vpu_increment = (vpuclk < desired_vpuclk ? CLK_INC: -CLK_INC); 
      if (desired_memclk != 0.0 && abs((int)(memclk - desired_memclk)) > CLK_INC)
        mem_increment = (memclk < desired_memclk ? CLK_INC : -CLK_INC);
      if (vpu_increment == 0 && mem_increment == 0)
        break;

      imp_setclock(vpuclk += vpu_increment, memclk += mem_increment, 0);
      Sleep(SLEEP_MS);
    }
  }


  imp_setclock(desired_vpuclk, desired_memclk, locked);
#undef CLK_INC
#undef SLEEP_MS  
}

void imp_setclock(float desired_vpuclk, float desired_memclk, int locked)
{
  // automatic variables
  struct dividers divs = {0, };
  float vpuclk, memclk;


  get_dividers(&divs);

#ifdef EXP_MOB
  if (divs.mem_feedback == 0 && divs.mem_post == 0) {
    desired_memclk = 0.0;
  }
#endif

  // initilize variables
  if (locked) desired_memclk = desired_vpuclk;
  vpuclk = desired_vpuclk;
  memclk = desired_memclk;


  if (setclk_feature_divopt > 0) {
    unsigned vpu_khz = (unsigned)(vpuclk * 1000.0);
    unsigned mem_khz = (unsigned)(memclk * 1000.0);
    if (!calc_optimal_dividers_for_given_clock(vpu_khz, mem_khz, divs.vpu_post, divs.mem_post, &divs.reference, &divs.vpu_feedback, &divs.mem_feedback))
      return; // FAILURE
  } else {
    divs.mem_feedback = (unsigned)((divs.reference * divs.mem_post * memclk) / (2.0f * setclk_PLLData.reference_freq));
    divs.vpu_feedback = (unsigned)((divs.reference * divs.vpu_post * vpuclk) / (2.0f * setclk_PLLData.reference_freq));
  }


#if 0 // sanity checks
  {
    float dbg_memclk = CALC_MEM_CLOCK(setclk_PLLData.reference_freq, divs.reference, divs.mem_post, divs.mem_feedback);
    float dbg_vpuclk = CALC_VPU_CLOCK(setclk_PLLData.reference_freq, divs.reference, divs.vpu_post, divs.vpu_feedback);
    printf("debug_info: ref_div=%u, mem_fb_div=%u, vpu_fb_div=%u\n", divs.reference, divs.mem_feedback, divs.vpu_feedback);
    if (!(190 <= dbg_memclk && dbg_memclk < 500)) {
      puts("######## error in program: dbg_memclk out of range");
      return;
    }
    if (!(190 <= dbg_vpuclk && dbg_vpuclk < 500)) {
      puts("######### error in program: dbg_vpuclk out of range");
      return;
    }
  }
#endif // end sanity checks


  set_dividers(&divs);

}





BOOL calc_optimal_dividers_for_given_clock(unsigned desired_vpu_khz, unsigned desired_mem_khz,
                                                     unsigned vpu_post_divider, unsigned mem_post_divider,
                                                     unsigned *out_reference_divider,
                                                     unsigned *out_vpu_feedback_divider,
                                                     unsigned *out_mem_feedback_divider)
{
  unsigned i_ref_div,    i_vfb_div,    i_mfb_div;
  unsigned best_ref_div, best_vfb_div, best_mfb_div;
  unsigned base_clock  = get_ref_khz();
  double best_vpu_offness = 99999.9;
  double best_mem_offness = 99999.9;

  int best_found = FALSE;

  for (i_ref_div      = MAX_REF_DIV; i_ref_div  >= MIN_REF_DIV; --i_ref_div) {
    for (i_vfb_div    = MIN_FB_DIV;  i_vfb_div  <= MAX_FB_DIV;  ++i_vfb_div) {

      unsigned tem_vpu_khz  = CALC_VPU_CLOCK(base_clock, i_ref_div, vpu_post_divider, i_vfb_div);
      if (tem_vpu_khz > desired_vpu_khz)
        break;
      if (tem_vpu_khz < desired_vpu_khz - MAX_KHZ_DIFF)
        continue; // optimization
      double tem_vpu_offness = (double)(desired_vpu_khz - tem_vpu_khz) / (double)desired_vpu_khz;

#ifdef EXP_MOB
      if (mem_post_divider == 0) {
        if (best_vpu_offness > tem_vpu_offness) {
          best_found = TRUE;
          best_ref_div = i_ref_div;
          best_vfb_div = i_vfb_div;
          best_mfb_div = 0;
          best_vpu_offness = tem_vpu_offness;
          best_mem_offness = 0.0;
        }
        continue;
      }
#endif

        for (i_mfb_div  = MIN_FB_DIV;  i_mfb_div  <= MAX_FB_DIV;  ++i_mfb_div) {
          unsigned tem_mem_khz  = CALC_MEM_CLOCK(base_clock, i_ref_div, mem_post_divider, i_mfb_div);
          if (tem_mem_khz > desired_mem_khz)
            break;
          if (tem_mem_khz < desired_mem_khz - MAX_KHZ_DIFF)
            continue; // optimization
          double tem_mem_offness = (double)(desired_mem_khz - tem_mem_khz) / (double)desired_mem_khz;

          if ((best_vpu_offness + best_mem_offness) > (tem_vpu_offness + tem_mem_offness)) {
            best_found = TRUE;
            best_ref_div = i_ref_div;
            best_vfb_div = i_vfb_div;
            best_mfb_div = i_mfb_div;
            best_vpu_offness = tem_vpu_offness;
            best_mem_offness = tem_mem_offness;
          }


      }}}

  if (best_found) {
    *out_reference_divider    = best_ref_div;
    *out_vpu_feedback_divider = best_vfb_div;
    *out_mem_feedback_divider = best_mfb_div;
  }

  return (best_found != 0);
}



// --------------------------------------
//  Name: setclk_getclock
//  Desc: Get RADEON clock settings
// --------------------------------------

// get clocks in kHz
int getclock(ULONG *vpuclk, ULONG *memclk)
{
  struct dividers divs;
  unsigned mem_clock=0, vpu_clock=0;

  get_dividers(&divs);

  printf("debug_info: ref_div=%u, mem_fb_div=%u, vpu_fb_div=%u\n", divs.reference, divs.mem_feedback, divs.vpu_feedback);


  if ((divs.reference * divs.mem_post) != 0)
    mem_clock =  CALC_MEM_CLOCK(10 * bioshdr.reference_freq, divs.reference, divs.mem_post, divs.mem_feedback);
  if ((divs.reference * divs.vpu_post) != 0)
    vpu_clock =  CALC_VPU_CLOCK(10 * bioshdr.reference_freq, divs.reference, divs.vpu_post, divs.vpu_feedback);

  if (memclk != NULL)
    *memclk = mem_clock;
  if (vpuclk != NULL && !divs.is_locked)
    *vpuclk = vpu_clock;

  return divs.is_locked;

}

int getclock(float* vpuclk, float* memclk)
{
  ULONG vpu_clock, mem_clock;
  int is_locked;

  is_locked = getclock(&vpu_clock, &mem_clock);

  if (vpuclk != NULL && !is_locked)
    *vpuclk = (float)vpu_clock * .001f;

  if (memclk != NULL)
    *memclk = (float)mem_clock * .001f;

  return is_locked;
}

int c_setclk_getclock(float* vpuclk, float* memclk) { return getclock (vpuclk, memclk); }


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

#include <algorithm>

/*
bios.reference_freq is sometimes read wrong from BIOS. Espacially on X800.
In that case, we provide a hardcoded value instead.
*/
void hack_bios_ref_freq() {
  ULONG test_vpuclk = 0;
  ULONG min_khz = 100 * 1000;
  ULONG max_khz = 999 * 1000;
  getclock(&test_vpuclk, 0);
  if (test_vpuclk < min_khz || test_vpuclk > max_khz) {
    USHORT old_ref_freq = 2700; // 2700 seems to be always (?) used 

    std::swap (old_ref_freq, bioshdr.reference_freq);
    getclock(&test_vpuclk, 0);
    if (test_vpuclk < min_khz || test_vpuclk > max_khz) {
      std::swap (old_ref_freq, bioshdr.reference_freq);
    }
  }
}