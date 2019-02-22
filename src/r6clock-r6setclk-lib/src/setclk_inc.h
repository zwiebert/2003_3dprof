#ifndef SETCLK_INC_H_
#define SETCLK_INC_H_


#include <r6probe.h>
#include <r6_inc.h>
#include <r6reg.h>
#include <stdio.h>
#include "setclk.h"


#ifdef __cplusplus
extern "C"
{
#endif

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

// ############################################################################

// ############################################################################

#define LATENCY_FAST   0x7ffff1ff
#define LATENCY_MEDIUM 0x44334244

#define MEMTIM_SLOW    0x1405357f
#define MEMTIM_MEDIUM  0x1405356a
#define MEMTIM_FAST    0x1405256a
#define MEMTIM_MASK    0x0000f0ff

// ############################################################################

extern  PLLDATA setclk_PLLData;
extern  BIOSPLLINFO bioshdr;

// following was added by Tom Servo
/*
Variable explanation:

ref_div - reference divider, which is the divider circuit between
reference (XTAL) oscillator and memory/core phase difference
detector (PDF) inputs.

mem_fb_div - memory-clock feedback divider, wich is the divider
circuit between voltage controlled oscillator (VCO) of memory clock
and memclk-PDF input.

core_fb_div - core-clock feedback divider, wich is the divider
circuit between VCO of core clock and memclk-PDF input.
*/

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





int   __cdecl setclk_open();
void  __cdecl setclk_close();
void  __cdecl setclk_setclock( float coreclk, float memclk, int locked );
int   __cdecl setclk_getclock( float* coreclk, float* memclk );
void  __cdecl setclk_setmlat( int latency );
int   __cdecl setclk_getmlat( void );
void  __cdecl setclk_setmtim( int timing );
int   __cdecl setclk_getmtim( void );
float __cdecl setclk_getclockstep( void );

int   __cdecl c_setclk_open();
void  __cdecl c_setclk_close();
void  __cdecl c_setclk_setclock( float coreclk, float memclk, int locked );
int   __cdecl c_setclk_getclock( float* coreclk, float* memclk );
void  __cdecl c_setclk_setmlat( int latency );
int   __cdecl c_setclk_getmlat( void );
void  __cdecl c_setclk_setmtim( int timing );
int   __cdecl c_setclk_getmtim( void );
float __cdecl c_setclk_getclockstep( void );


void hack_bios_ref_freq();

#ifdef __cplusplus
}
#endif

#endif /* SETCLK_INC_H_ */