#include "stdafx.h"
#include "config.h"
#include <algorithm>

//#define EXP_FORCED_REF_FREQ // force reference freq to 2700
#define EXP_MOB  // enable code to deal with RAM-less cards
//#define EXP_SIM_MOB // fake a card with no local RAM

#ifdef USE_DRV_WIO

#include "radcard.h"


#include "WinIO.h"
#include "r6.h"
#define RADEON_BIOS_HEADER_LOC  0x48
#include "r6reg.h"
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <errno.h>



#define MAX_FB_DIV 150   // upto 255
#define MIN_FB_DIV 2     // at least 2
#define MAX_REF_DIV 150  // upto 255 
#define MIN_REF_DIV 4    // at least 2

#define MAX_KHZ_DIFF 8000  // clocks have to be higher than DESIRED_KHZ - MAX_KHZ_DIFF



#define PCI_HDR_GET_DEV(p) ((p)->Slot >> 3)
#define PCI_HDR_GET_FN(p) ((p)->Slot & 0x7)

#define D(x)  //(backend_debug && (1, x))
#define UD(x) //(backend_debug && (1, x)) // user debug info
#define V(x)  //(backend_debug && (1, x))

#define GET_REF_DIV(hw_reg) ((hw_reg) & 0xff)
#define GET_MEM_FB_DIV(hw_reg) (((hw_reg) >> 8) & 0xff)
#define GET_VPU_FB_DIV(hw_reg) (((hw_reg) >> 16) & 0xff)
#define GET_VPU_POST_DIV(hw_reg) (post_divider_lut[(hw_reg) & 0x7] + 0)
#define GET_MEM_POST_DIV(hw_reg) GET_VPU_POST_DIV((hw_reg))

#define SET_REF_DIV(hw_reg, val) ((hw_reg) = (((hw_reg) & ~0xff)) | ((val) & 0xff))
#define SET_MEM_FB_DIV(hw_reg, val) ((hw_reg) = (((hw_reg) & ~(0xff << 8)) | (((val) & 0xff) << 8)))
#define SET_VPU_FB_DIV(hw_reg, val) ((hw_reg) = (((hw_reg) & ~(0xff << 16)) | (((val) & 0xff) << 16)))
#define SET_VPU_POST_DIV(hw_reg, val) ((hw_reg) = (((hw_reg) & ~0x7) | post_divider_set_lut[(val)]))
#define SET_MEM_POST_DIV(hw_reg, val) SET_VPU_POST_DIV((hw_reg), (val))

//#define CALC_VPU_CLOCK(ref_freq, ref_div, post_div, fb_div)  ((2.0f * (float)(fb_div) * (ref_freq)) / ((ref_div) * (post_div)))
#define CALC_VPU_CLOCK(ref_freq, ref_div, post_div, fb_div)  ((2 * (fb_div) * (ref_freq)) / ((ref_div) * (post_div)))
#define CALC_MEM_CLOCK(ref_freq, ref_div, post_div, fb_div) CALC_VPU_CLOCK((ref_freq), (ref_div), (post_div), (fb_div))
extern int post_divider_lut[8];
extern int post_divider_set_lut[13];


// === local functions ===
static int IsVideoCard(struct winio_pci_common_header *pci_hdr);
static void set_default_speeds(int num);
static void dummy_unsigned(unsigned n) { (void)n; } 


char *strdup(const char *s1)
{
  char *s2 = (char*)malloc(strlen(s1) + 1);
  if (s2 != NULL)
    strcpy(s2, s1);
  return s2;
}

int backend_debug;




int post_divider_lut[8] = {
  /*0:*/0, /*1:*/1, /*2:*/2, /*3:*/4, /*4:*/8, /*5:*/3, /*6:*/6, /*7:*/12
};
int post_divider_set_lut[13] = {
  /*0:*/0, /*1:*/1, /*2:*/2, /*3:*/5, /*4:*/3, /*5:*/0,
  /*6:*/6, /*7:*/0, /*8:*/4,  /*9:*/0, /*10:*/0, /*11:*/0, /*12:*/7
};

void radcard::hack_bios_ref_freq() {
  ULONG test_vpuclk = 0;
  ULONG min_khz = 100 * 1000;
  ULONG max_khz = 999 * 1000;
  getclock(&test_vpuclk, 0);
  if (test_vpuclk < min_khz || test_vpuclk > max_khz) {
    USHORT old_ref_freq = 2700;

    std::swap (old_ref_freq, bioshdr.reference_freq);
    getclock(&test_vpuclk, 0);
    if (test_vpuclk < min_khz || test_vpuclk > max_khz) {
      std::swap (old_ref_freq, bioshdr.reference_freq);
    }
  }
}

// radcard member functions

radcard::radcard(struct winio_pci_common_header &pci_hdr)
: mmr(0), bios(0)
{
  if (!(winio_instance = InitializeWinIo()))
    throw "Driver not found";

  try {

  device_id = pci_hdr.DeviceID;
  bus_id    = (char*)malloc(8 * sizeof(char));
  sprintf(bus_id, "%02d:%02d.%d", pci_hdr.Bus, PCI_HDR_GET_DEV(&pci_hdr), PCI_HDR_GET_FN(&pci_hdr));

  fb_phys  =  pci_hdr.BaseAddresses[0] & 0xfffffff0;
  mmr_phys =  pci_hdr.BaseAddresses[2] & 0xfffffff0;

  mmr             = MapIO(winio_instance, mmr_phys, RADEON_MMR_SIZE);
  ULONG bios_phys = (inreg32(mmr, RADEON_BIOS_1_SCRATCH) & 0xffff) << 4;
  bios            = MapIO(winio_instance, bios_phys, RADEON_BIOS_SIZE);
  init_bioshdr();

  hack_bios_ref_freq();

  } catch (...) {
    ShutdownWinIo(winio_instance);
    throw;
  }
}

radcard::~radcard() {
  UnMapIO(winio_instance, mmr);
  UnMapIO(winio_instance, bios);
  ShutdownWinIo(winio_instance);
}

void radcard::init_bioshdr() {

  // load PLL info
  USHORT bios_header              = inreg16(bios, RADEON_BIOS_HEADER_LOC);
  USHORT ptr_to_pll_info_ptr      = bios_header + 0x30;
  USHORT pll_info_ptr             = inreg16(bios, ptr_to_pll_info_ptr); 
  memcpy(&bioshdr, (void*)((UCHAR*)bios + pll_info_ptr), sizeof bioshdr);


#ifdef  EXP_FORCED_REF_FREQ
  // PLL info is no longer there on X800
  bioshdr.reference_freq = 2700;
#endif
  plldata.reference_freq = bioshdr.reference_freq * 0.01f;
  plldata.xclk           = bioshdr.xclk * 0.01f;
  plldata.min_freq       = bioshdr.min_freq * 0.01f;
  plldata.max_freq       = bioshdr.max_freq * 0.01f;
  plldata.reference_div  = bioshdr.reference_div;
}

#define r6probe_readpll(off) inpll(off)
//#define r6probe_writepll(off, val) outpll(off, val)
#define setclk_PLLData plldata


void  radcard::setclock(float desired_vpuclk, float desired_memclk, int locked)
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

void radcard::imp_setclock(float desired_vpuclk, float desired_memclk, int locked)
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

BOOL radcard::calc_optimal_dividers_for_given_clock(unsigned desired_vpu_khz, unsigned desired_mem_khz,
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





int radcard::getclock(float* vpuclk, float* memclk)
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


void radcard::get_dividers(struct dividers *divs)
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

void radcard::set_dividers(struct dividers *divs)
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

int radcard::getclock(ULONG *vpuclk, ULONG *memclk)
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

ULONG radcard::inpll(int offset)
{
  ULONG data;

  outreg8(mmr, RADEON_CLOCK_CNTL_INDEX, offset & 0x3f);
  data = inreg32(mmr, RADEON_CLOCK_CNTL_DATA);

  return data;
}

void radcard::outpll(int offset, UCHAR val) {
  UCHAR	tmp;

  tmp = (UCHAR)((offset & 0x3f) | RADEON_PLL_WR_EN);
  outreg8(mmr, RADEON_CLOCK_CNTL_INDEX, tmp);
  outreg32(mmr, RADEON_CLOCK_CNTL_DATA, val);
}

void radcard::select_pll_reg(unsigned addr) {
  ULONG curr_addr;
  curr_addr = inreg8(mmr, RADEON_CLOCK_CNTL_INDEX) & 0xbf;
  if (curr_addr != addr)
    outreg8(mmr, RADEON_CLOCK_CNTL_INDEX, addr);
}


void radcard::r6probe_writepll(unsigned addr, unsigned mask, unsigned data)
{
  UCHAR	tmp = (addr & 0x3f) | RADEON_PLL_WR_EN;
  select_pll_reg(tmp);
  ULONG usr_data = data & mask;
  ULONG curr_data = inreg32(mmr, RADEON_CLOCK_CNTL_DATA);
  curr_data &= ~mask;
  curr_data |= usr_data;
  select_pll_reg(tmp);

  do {
    outreg32(mmr, RADEON_CLOCK_CNTL_DATA, curr_data);
  } while(inreg32(mmr, RADEON_CLOCK_CNTL_DATA) != curr_data);

}

#undef r6probe_readpll
#undef r6probe_writepll


const USHORT PCI_VENDOR_ID_ATI = 0x1002;

USHORT dev_ids[] = 
{
  0x3150,    // MOBILITY_X600
  0x3e50,    // X600_SERIES
  0x3e54,    // FireGL_V3200
  0x4136,    // IGP_320M
  0x4137,    // IGP_340
  0x4144,    // R9500
  0x4147,    // FireGL_Z1_AGP_Pro
  0x4148,    // R9800_AIW
  0x4150,    // R9600
  0x4151,    // R9600_2
  0x4152,    // R9600_3
  0x4153,    // 9550
  0x4154,    // FireGL_T2
  0x4242,    // R200_BB
  0x4336,    // IGP_320M_SERIES	       
  0x4337,    // IGP_340M_SERIES
  0x4966,    // 9000_AIW
  0x4a48,    // X800_SERIES_2
  0x4a49,    // RX800_PRO
  0x4a4a,    // X800_SE
  0x4a4b,    // RX800
  0x4a4c,    // X800_SERIES
  0x4a4d,    // FireGL_X3_256
  0x4a4e,    // MOBILITY_9800
  0x4a50,    // RX800_XT
  0x4c57,    // MOBILITY_7500
  0x4c58,    // MOBILITY_FIRE_GL7800
  0x4c59,    // RADEON_LY
  0x4c5a,    // RADEON_LZ
  0x4c64,    // FGL9000_MOBILE
  0x4c66,    // R9000_MOBILE
  0x4e44,    // R9700_PRO
  0x4e45,    // R9500_PRO, R9700
  0x4e46,    // R9600_XT
  0x4e47,    // FireGL_X1_AGP_Pro
  0x4e48,    // R9800_PRO
  0x4e49,    // R9800
  0x4e4a,    // R9800_XT
  0x4e4b,    // FireGL_X2_256_X2_256t
  0x4e50,    // R9600_MOBILE
  0x4e54,    // MOBILITY_FIRE_GL_T2	       
  0x5144,    // 7200_SERIES
  0x5145,    // RADEON_QE
  0x5146,    // RADEON_QF
  0x5147,    // RADEON_QG
  0x5148,    // FireGL_8800
  0x514c,    // 8500_SERIES
  0x514d,    // R9100
  0x514e,    // R200_QN
  0x514f,    // R200_QO
  0x5157,    // 7500_SERIES
  0x5159,    // 7000_SERIES_2
  0x515a,    // 7000_SERIES
  0x516c,    // R200_Ql
  0x5460,    // MOBILITY_X300
  0x5464,    // MIBILITY_M22_GL
  0x5548,    // X880_SERIES
  0x5549,    // X880_PRO
  0x554b,    // X880_SE
  0x5551,    // FireGL_V7200
  0x5552,    // FireGL_V5100
  0x5554,    // FireGL_V7100
  0x5834,    // R9100_IGP
  0x5960,    // 9250_SERIES
  0x5961,    // R9200
  0x5964,    // R9200_2
  0x5b60,    // X300_SERIES
  0x5b62,    // X600_SERIES_2
  0x5b64,    // FireGL_V1100_V3100
  0x5b65,    // FireGL_D1100
  0x5c61,    // R9200_MOBILE
  0x5c63,    // MOBILITY_9200
  0x5d48,    // MOBILITY_M28
  0x5d57,    // X880_XT
  0x7834,    // 9000_9100_PRO_IGP_SERIES
  0xc599,    // MOBILITY

};

bool radcard::is_supported(USHORT ven_id, USHORT dev_id)
{
  if (ven_id != PCI_VENDOR_ID_ATI)
    return false;

  for(int i=0; dev_ids[i] != 0; ++i) {
    if (dev_id == dev_ids[i])
      return true;
  }

  return false;
}

#endif // USE_DRV_WIO
