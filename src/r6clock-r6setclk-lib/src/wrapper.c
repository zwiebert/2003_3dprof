/*

This file is for migration of setclk library from ASM to C. It defines all API functions as wrappers
around prefixed (c_, asm_) implementations.

*/

#include "setclk_inc.h"

PLLDATA setclk_PLLData;

int   __cdecl asm_setclk_open();
void  __cdecl asm_setclk_close();
void  __cdecl asm_setclk_setclock( float coreclk, float memclk, int locked );
int   __cdecl asm_setclk_getclock( float* coreclk, float* memclk );
void  __cdecl asm_setclk_setmlat( int latency );
int   __cdecl asm_setclk_getmlat( void );
void  __cdecl asm_setclk_setmtim( int timing );
int   __cdecl asm_setclk_getmtim( void );
float __cdecl asm_setclk_getclockstep( void );

#define C_PREF(base) c_setclk_##base
#define A_PREF(base) asm_setclk_##base
#define X_PREF(base) C_PREF(base)

// Available in ASM only

// available in both C and ASM
int   __cdecl setclk_open() { return X_PREF(open)(); }
void  __cdecl setclk_close() { X_PREF(close)(); }
void  __cdecl setclk_setclock(float coreclk, float memclk, int locked) {X_PREF(setclock) (coreclk, memclk, locked);}
float __cdecl setclk_getclockstep(void) { return X_PREF(getclockstep)(); }
int   __cdecl setclk_getclock(float* coreclk, float* memclk )  { return X_PREF(getclock)(coreclk, memclk); }
void  __cdecl setclk_setmlat(int latency) { X_PREF(setmlat)(latency); }
int   __cdecl setclk_getmlat(void) { return X_PREF(getmlat)(); }
void  __cdecl setclk_setmtim(int timing) { X_PREF(setmtim)(timing); }
int   __cdecl setclk_getmtim(void) { return X_PREF(getmtim)(); }

// available in C only


// enable / disable some features

int setclk_feature_divopt;
void setclk_divopt(int enable) { setclk_feature_divopt = enable; }
