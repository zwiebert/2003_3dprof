// r6clock-dll.cpp : Defines the entry point for the DLL application.
//
#define EXP2 1


#include "stdafx.h"
#include "r6clock-dll.h"
#if !EXP2
#include "setclk.h"
#include "r6probe.h"
#endif

#define EXP1 0
int nmb_openers;


int FindAllCards(void);


BOOL
APIENTRY DllMain( HANDLE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
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
#if EXP2
    FindAllCards();
#endif
    break;

  case DLL_THREAD_DETACH:
  case DLL_PROCESS_DETACH:
#if EXP1
    if (--nmb_openers == 0) {
      setclk_close();
    }
#endif
    break;

  }
  return TRUE;
}


#if ! EXP2
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
#endif

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


R6CLOCKDLL_API unsigned r6clock_get_id(void)
{

  if (nmb_openers == 0)
    if ( !setclk_open() )
      return 0;

  unsigned pci_id = r6probe_readpci(0);

  if (nmb_openers == 0)
    setclk_close();
  return pci_id;
}



/////////////////////////////////////////////////////////////////////////////////////////////////////////
#if EXP2

int setclk_feature_divopt = 1;

#include "WinIO.h"
#include "r6.h"
#define RADEON_BIOS_HEADER_LOC  0x48
#include "r6reg.h"
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <errno.h>

#define ERR_STREAM stderr
#define DBG_STREAM stderr
#define NO_WINIO "driver not fount"
#define HAVE_WINIO "driver found"

#define PCI_HDR_GET_DEV(p) ((p)->Slot >> 3)
#define PCI_HDR_GET_FN(p) ((p)->Slot & 0x7)

#define D(x)  x //(backend_debug && (1, x))
#define UD(x) x //(backend_debug && (1, x)) // user debug info
#define V(x)  x //(backend_debug && (1, x))

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
static int map_mem(const char* dev_name);
static void unmap_mem(void);
static void *map_dev_mem (unsigned long base, unsigned long size);
static void unmap_dev_mem(volatile void *base);
static void dummy_unsigned(unsigned n) { (void)n; } 

char *strdup(const char *s1)
{
  char *s2 = (char*)malloc(strlen(s1) + 1);
  if (s2 != NULL)
    strcpy(s2, s1);
  return s2;
}




typedef struct {
  unsigned short x[4],
    xclk,
    xx[2],
    reference_freq,
    reference_div,
    min_freq,
    xxx,
    max_freq;
} BIOSPLLINFO, *PBIOSPLLINFO;

typedef struct
{
    float    reference_freq;
    unsigned reference_div;
    float    min_freq;
    float    max_freq;
    float    xclk;
} PLLData;

class radcard {
private:
  static ULONG inreg(void *base, int offset) { return ((ULONG*)((UCHAR *)base + offset))[0]; }  
  static UCHAR inreg8(void *base, int offset) { return ((UCHAR*)((UCHAR *)base + offset))[0]; }  
  static void outreg(void *base, int offset, ULONG val) { ((ULONG*)((UCHAR *)base + offset))[0] = val; }  
  static void outreg8(void *base, int offset, UCHAR val) { ((UCHAR*)((UCHAR *)base + offset))[0] = val; }

  ULONG inpll(int offset)
  {
    ULONG data;

    outreg8(mmr, RADEON_CLOCK_CNTL_INDEX, offset & 0x3f);
    data = inreg(mmr, RADEON_CLOCK_CNTL_DATA);

    return data;
  }

  void outpll(ULONG offset, UCHAR val) {
    UCHAR	tmp;

    tmp = (offset & 0x3f) | RADEON_PLL_WR_EN;
    outreg8(mmr, RADEON_CLOCK_CNTL_INDEX, tmp);
    outreg(mmr, RADEON_CLOCK_CNTL_DATA, val);
  }
  void select_pll_reg(unsigned addr) {
     ULONG curr_addr;
     curr_addr = inreg8(mmr, RADEON_CLOCK_CNTL_INDEX, tmp) & 0xbf;
  }

  void r6probe_writepll(unsigned addr, unsigned mask, unsigned data)
  {
    UCHAR	tmp = (offset & 0x3f) | RADEON_PLL_WR_EN;
    select_pll_reg(tmp);
    ULONG user_data = data & mask;
    ULONG curr_data = inreg(mmr, RADEON_CLOCK_CNTL_DATA);
    curr_data &= ~mask;
    curr_data |= usr_data;
    select_pll_reg(tmp);

    do {
      outreg(mmr, RADEON_CLOCK_CNTL_DATA, curr_data);
    } while(inreg(mmr, RADEON_CLOCK_CNTL_DATA) != curr_data);

  }

  void init_bioshdr() {
    ULONG data, addr;

    data = inreg(bios, RADEON_BIOS_HEADER_LOC);
    addr = (data & 0xffff) + 0x30;  // offset to pll info ptr
    data = inreg(bios, addr);
    addr = (data & 0xffff);
    memcpy(&bioshdr, (void*)((UCHAR*)bios + addr), sizeof bioshdr);

    plldata.reference_freq = bioshdr.reference_freq * 0.01f;
    plldata.xclk           = bioshdr.xclk * 0.01f;
    plldata.min_freq       = bioshdr.min_freq * 0.01f;
    plldata.max_freq       = bioshdr.max_freq * 0.01f;
    plldata.reference_div  = bioshdr.reference_div;
  }
public:
#define r6probe_readpll(off) inpll(off)
//#define r6probe_writepll(off, val) outpll(off, val)
#define setclk_PLLData plldata




public:
  int getclock(float* vpuclk, float* memclk)
  {
    ULONG vpu_clock, mem_clock;
    int is_locked;

    is_locked = getclock(&vpu_clock, &mem_clock);

    if (!is_locked)
      *vpuclk = (float)vpu_clock * .001f;
    *memclk = (float)mem_clock * .001f;

    return is_locked;
  }

  // get clocks in kHz
  int getclock(ULONG *vpuclk, ULONG *memclk)
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




#define MAX_FB_DIV 150   // upto 255
#define MIN_FB_DIV 2     // at least 2
#define MAX_REF_DIV 150  // upto 255 
#define MIN_REF_DIV 4    // at least 2


  // --------------------------------------
  //  Name: r6probe_setclk
  //  Desc: Set RADEON clock freqs
  // --------------------------------------
  void  setclock(float desired_vpuclk, float desired_memclk, int locked)
  {
    float vpuclk, memclk;


    // do clokcing in step-by-step to avoid screen corruption
    if (!locked && !getclock(&vpuclk, &memclk))
    {
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
    // external options
    extern int setclk_feature_divopt;
    // automatic variables
    unsigned hw_reg, reference_divider,
      vpu_feedback_divider, vpu_post_divider,
      mem_feedback_divider, mem_post_divider;
    unsigned char is_locked; // if mem and vpu clock are coupled
    float vpuclk, memclk, maxclk;


    // get hardware registers first
    hw_reg = r6probe_readpll(RADEON_XCLK_CNTL);
    is_locked        = ((hw_reg & 0x7) == 0x7);
    vpu_post_divider = GET_VPU_POST_DIV(hw_reg);

    hw_reg = r6probe_readpll(RADEON_MCLK_CNTL);
    mem_post_divider = GET_MEM_POST_DIV(hw_reg);

    hw_reg = r6probe_readpll(RADEON_X_MPLL_REF_FB_DIV);
    reference_divider = GET_REF_DIV(hw_reg);

    // initilize variables
    if (locked) desired_memclk = desired_vpuclk;
    vpuclk = desired_vpuclk;
    memclk = desired_memclk;
    maxclk = ((vpuclk > memclk) ? vpuclk : memclk) + 1.0f;

    // experimental: try to minimize gap between desired clocks and actual clock by changing reference_divider
    if (setclk_feature_divopt > 0) {
      vpuclk = calc_optimal_dividers_for_given_clock(maxclk, vpuclk, mem_post_divider, &reference_divider, &vpu_feedback_divider);
      memclk = calc_optimal_dividers_for_given_clock(maxclk, memclk, vpu_post_divider, &reference_divider, &mem_feedback_divider);

      if (desired_vpuclk < vpuclk || (desired_vpuclk - vpuclk) > 14.0)
        return;
      if (desired_memclk < memclk || (desired_memclk - memclk) > 14.0)
        return;
    }

    // calulate new feedback dividers
    mem_feedback_divider = (unsigned)((reference_divider * mem_post_divider * memclk) / (2.0f * setclk_PLLData.reference_freq));
    vpu_feedback_divider = (unsigned)((reference_divider * vpu_post_divider * vpuclk) / (2.0f * setclk_PLLData.reference_freq));

#if 1
    // sanity checks
    {
      float dbg_memclk = CALC_MEM_CLOCK(setclk_PLLData.reference_freq, reference_divider, mem_post_divider, mem_feedback_divider);
      float dbg_vpuclk = CALC_VPU_CLOCK(setclk_PLLData.reference_freq, reference_divider, vpu_post_divider, vpu_feedback_divider);
      printf("debug_info: ref_div=%u, mem_fb_div=%u, vpu_fb_div=%u\n", reference_divider, mem_feedback_divider, vpu_feedback_divider);
      if (!(190 <= dbg_memclk && dbg_memclk < 500)) {
        puts("######## error in program: dbg_memclk out of range");
        return;
      }
      if (!(190 <= dbg_vpuclk && dbg_vpuclk < 500)) {
        puts("######### error in program: dbg_vpuclk out of range");
        return;
      }
    }


#endif

    // build new register content
    SET_REF_DIV(hw_reg, reference_divider);
    SET_MEM_FB_DIV(hw_reg, mem_feedback_divider);
    SET_VPU_FB_DIV(hw_reg, vpu_feedback_divider);

    // write register
    r6probe_writepll(RADEON_X_MPLL_REF_FB_DIV, 0x00ffffff, hw_reg);

#if 0
    // writing locked state to R300 may crash (?)
    r6probe_writepll(RADEON_XCLK_CNTL, 7, (locked ? 7 : 2));
#endif
  }

  // The problem is: mem and vpu share the same reverence_divider
  // So better rewrite this function to take both clock freqencies as parameters
  float calc_optimal_dividers_for_given_clock(const float max_clock, float desired_clock, unsigned post_divider, unsigned *out_reference_divider, unsigned *out_feedback_divider)
  {
    unsigned i_ref_div, i_fb_div;
    double best_clock = -1;
    double best_clock_diff = desired_clock;
    unsigned best_ref_div, best_fb_div;
    double base_clock = setclk_PLLData.reference_freq;
    double clock, clock_diff;
    double multiplier;
    int best_found = 0;

    for (i_ref_div = MAX_REF_DIV; i_ref_div >= MIN_REF_DIV; --i_ref_div) {
      for (i_fb_div = MIN_FB_DIV; i_fb_div <= MAX_FB_DIV; ++i_fb_div) {
        multiplier = (2.0f *(float)i_fb_div) / ((float)i_ref_div * (float)post_divider);
        clock = base_clock * multiplier;
        if (clock < 0.0 || clock > 999000.0) continue;
        if (clock > desired_clock) continue;
        clock_diff = desired_clock - clock;
        if (best_clock_diff < (clock_diff + 1.5)) continue;
        if (max_clock >= CALC_VPU_CLOCK(base_clock, i_ref_div, post_divider, MAX_FB_DIV)) continue;

        best_clock      = clock;
        best_clock_diff = clock_diff;
        best_ref_div    = i_ref_div;
        best_fb_div     = i_fb_div;
        best_found      = 1;


      }
    }

    if (best_found) {
      *out_reference_divider = best_ref_div;
      *out_feedback_divider = best_fb_div;
    }

    return (float)best_clock;
  }
#undef r6probe_readpll
#undef r6probe_writepll

public:
  radcard(struct winio_pci_common_header &pci_hdr)
    : mmr(0), bios(0)
  {
    device_id = pci_hdr.DeviceID;
    bus_id    = (char*)malloc(8 * sizeof(char));
    sprintf(bus_id, "%02d:%02d.%d", pci_hdr.Bus, PCI_HDR_GET_DEV(&pci_hdr), PCI_HDR_GET_FN(&pci_hdr));
    reg_address = pci_hdr.BaseAddresses[0];

    fb_phys  =  pci_hdr.BaseAddresses[0] & 0xfffffff0;
    mmr_phys =  pci_hdr.BaseAddresses[2] & 0xfffffff0;

    mmr = map_dev_mem(mmr_phys, RADEON_MMR_SIZE);
    ULONG bios_phys = (inreg(mmr, RADEON_BIOS_1_SCRATCH) & 0xffff) << 4;
    bios = map_dev_mem(bios_phys, RADEON_BIOS_SIZE);
    init_bioshdr();
  }

  ~radcard() {
    unmap_dev_mem(mmr);
    unmap_dev_mem(bios);
  }

private:
  ULONG fb_phys;
  ULONG mmr_phys;
  int number;
  char *bus_id;
  void *fb;
  unsigned fb_size;
  void *mmr;
  void *bios;
  ULONG reg_address;
  short device_id;
  BIOSPLLINFO bioshdr;
  PLLData plldata;
};

radcard *card[10];


static WinIO winio_instance;
int backend_debug;
int backend_remove_driver;

/* Check if the device is a videocard */
static int IsVideoCard(struct winio_pci_common_header *pci_hdr)
{
  return pci_hdr->BaseClass == 0x03;
}

static void module_winio_exit(void)
{
  if (winio_instance) {
    ShutdownWinIo(winio_instance);
    if (backend_remove_driver) {
      BOOL success;
      success = RemoveWinIoDriver();
      V(fprintf(DBG_STREAM, "nvclock: backend: Calling RemoveWinIoDriver() => %s\n", (success ? "succeeded" : "failed"))); 
    }
    winio_instance = NULL;
  }
}

static BOOL module_winio_init()
{
  if (!winio_instance) {
    winio_instance = InitializeWinIo();

    if (winio_instance) {
      atexit(module_winio_exit);
      V(fprintf(DBG_STREAM, "nvclock: %s\n", HAVE_WINIO));
    } else {
      fprintf(ERR_STREAM, "nvclock: %s\n", NO_WINIO);
      exit(1);
    }
  }

  return (winio_instance != NULL);
}

static void *map_dev_mem (unsigned long base, unsigned long size)
{
  if (!module_winio_init())
    return NULL;
  return MapIO(winio_instance, base, size);
}

static void unmap_dev_mem(volatile void *base)
{
  if (!module_winio_init())
    return;
  UnMapIO((void *)winio_instance, (void*)base);
}




int FindAllCards()
{
  int card_idx = -1;
  int count_pci_devices = 0;

  struct winio_pci_common_header buf = {0, };
  if (!module_winio_init())
    return -1;

  UD(fprintf(DBG_STREAM, "back_w32: %s\n", "Searching for PCI devices\n"));
  while (winio_get_pci_header(winio_instance, &buf)) 
  {
    UD(fprintf(DBG_STREAM, "PCI: bus=%d, dev=%2d, fn=%2d,\t VenID=0x%04x, DevID=0x%04x, BaseAddr[0]=0x%08x, BaseClass=0x%x\n",
      (int)buf.Bus, (int)(buf.Dev), (int)(buf.Fn), (unsigned)buf.VendorID, (unsigned)buf.DeviceID,
      (unsigned)buf.BaseAddresses[0], (unsigned)buf.BaseClass)
      );
    ++count_pci_devices;

    /* Check if the card contains an ATIa chipset */	
    if(buf.VendorID != 0x1002)
      continue;
    if(!IsVideoCard(&buf))
      continue;

    card_idx++;
    card[card_idx] = new radcard(buf);
    break; // TODO
  }

  if (count_pci_devices < 3) {
    fprintf(stderr, "nvclock: backend: Number of PCI devices found on this system: %d.\n", count_pci_devices);
  }

  return card_idx;
}




int post_divider_lut[8] = {
  /*0:*/0, /*1:*/1, /*2:*/2, /*3:*/4, /*4:*/8, /*5:*/3, /*6:*/6, /*7:*/12
};
int post_divider_set_lut[13] = {
  /*0:*/0, /*1:*/1, /*2:*/2, /*3:*/5, /*4:*/3, /*5:*/0,
  /*6:*/6, /*7:*/0, /*8:*/4,  /*9:*/0, /*10:*/0, /*11:*/0, /*12:*/7
};



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
      card[0]->setclock(core_mhz, ram_mhz, 0);

    int locked = card[0]->getclock(&core_mhz, &ram_mhz);
    *core_khz = (int) (core_mhz * 1000.0f);
    *ram_khz  = (int) (ram_mhz  * 1000.0f);
    if (locked)
      *core_khz = *ram_khz;

    if (nmb_openers == 0)
      setclk_close();
    return 0;
}

#endif