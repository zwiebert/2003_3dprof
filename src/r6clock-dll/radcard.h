#include "WinIO.h"

class radcard {
 public:
  static bool    is_supported(USHORT ven_id, USHORT dev_id);
public:
  radcard(struct winio_pci_common_header &pci_hdr);
  ~radcard();
public:
  int     getclock(float* vpuclk, float* memclk); // get clocks in MHz
  int     getclock(ULONG *vpuclk, ULONG *memclk); // get clocks in kHz
  void    setclock(float desired_vpuclk, float desired_memclk, int locked); // set clocks in MHz

  unsigned get_ref_khz() { return this->bioshdr.reference_freq * 10; }

private:
  friend LONG r6clockdiag_get_info(char *buf, ULONG buflen);
  struct dividers {
    unsigned reference,
      vpu_feedback, vpu_post,
      mem_feedback, mem_post;
    BOOL is_locked;
  };

  void get_dividers(struct dividers *divs);
  void set_dividers(struct dividers *divs);
  void radcard::hack_bios_ref_freq();

private:
  void          init_bioshdr();
private:
  static UCHAR  inreg8  (void *base, int offset)             { return ((UCHAR*) ((UCHAR *)base + offset))[0]; }  
  static USHORT inreg16 (void *base, int offset)             { return ((USHORT*)((UCHAR *)base + offset))[0]; }  
  static ULONG  inreg32 (void *base, int offset)             { return ((ULONG*) ((UCHAR *)base + offset))[0]; }  
  static void   outreg8 (void *base, int offset, UCHAR  val) { ((UCHAR*)  ((UCHAR *)base + offset))[0] = val; }
  static void   outreg16(void *base, int offset, USHORT val) { ((USHORT*) ((UCHAR *)base + offset))[0] = val; }
  static void   outreg32(void *base, int offset, ULONG  val) { ((ULONG*)  ((UCHAR *)base + offset))[0] = val; } 
  ULONG    inpll (int offset);
  void     outpll(int offset, UCHAR val);
private:
  void          r6probe_writepll(unsigned addr, unsigned mask, unsigned data);
  void          select_pll_reg(unsigned addr);
private:
  void imp_setclock(float desired_vpuclk, float desired_memclk, int locked);

  BOOL calc_optimal_dividers_for_given_clock(
    unsigned  desired_vpu_khz,          unsigned desired_mem_khz,
    unsigned  vpu_post_divider,         unsigned mem_post_divider,
    unsigned *out_reference_divider,
    unsigned *out_vpu_feedback_divider, unsigned *out_mem_feedback_divider);

private:
  struct BIOS_PLL_Info
  {
    unsigned short x[4],
      xclk,
      xx[2],
      reference_freq,
      reference_div,
      min_freq,
      xxx,
      max_freq;
  };

  struct PLLData
  {
    float    reference_freq;
    unsigned reference_div;
    float    min_freq;
    float    max_freq;
    float    xclk;
  };

private:
  const static int setclk_feature_divopt = 1;
  ULONG fb_phys;          // pyhs FB space 
  ULONG mmr_phys;         // phys MMR space 
  void *mmr;              // user mapped MMR space
  void *bios;             // user mapped BIOS space
  BIOS_PLL_Info bioshdr;
  PLLData plldata;

  int number;
  char *bus_id;
  unsigned fb_size;
  short device_id;
  WinIO winio_instance;
};
