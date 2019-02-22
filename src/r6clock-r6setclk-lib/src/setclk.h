/* $Id: setclk.h,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
 *
 * Header file for setclk library
 *
 * ----------------------------------------------------------------------------
 * LICENSE
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License (GPL) as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 *
 * To read the license please visit http://www.gnu.org/copyleft/gpl.html
 * ----------------------------------------------------------------------------
 * $Log: setclk.h,v $
 * Revision 1.1.1.1  2002/01/25 22:14:25  VSi
 * Imported to CVS
 *
 * ----------------------------------------------------------------------------
 *                    Copyright (C) 2001-2002 Vahur Sinij„rv
 * ----------------------------------------------------------------------------
 */
#ifndef SETCLK_H
#define SETCLK_H

#ifdef __cplusplus
extern "C"
{
#endif

/*
 * setclk library operates on top of r6probe lib ( which provides a
 * connection to low level hw access driver - r6probe ), to simplify
 * certain tasks as setting radeon clocks and memory settings. If
 * you use this library, do not call r6probe_open() - setclk_open()
 * will take care of that among other things. I think other aspects
 * need no further explanation - look at the function names :)
 */

#define MLAT_FAST       0
#define MLAT_MEDIUM     1
#define MLAT_LAST       MLAT_MEDIUM

#define MTIM_SLOW       0
#define MTIM_MEDIUM     1
#define MTIM_FAST       2
#define MTIM_LAST       MTIM_FAST

int   __cdecl setclk_open();
void  __cdecl setclk_close();
void  __cdecl setclk_setclock( float coreclk, float memclk, int locked );
int   __cdecl setclk_getclock( float* coreclk, float* memclk );
void  __cdecl setclk_setmlat( int latency );
int   __cdecl setclk_getmlat( void );
void  __cdecl setclk_setmtim( int timing );
int   __cdecl setclk_getmtim( void );
float __cdecl setclk_getclockstep( void );

void __cdecl setclk_divopt(int enable);
extern int setclk_feature_divopt;


typedef struct
{
  float    reference_freq;
  unsigned reference_div;
  float    min_freq;
  float    max_freq;
  float    xclk;
} PLLDATA, *PPLLDATA;

/*
 * Use this structure after you have called setclk_open to get
 * minimum and maximum PLL frequencies.
 */
extern PLLDATA setclk_PLLData;

#ifdef __cplusplus
}
#endif

#endif