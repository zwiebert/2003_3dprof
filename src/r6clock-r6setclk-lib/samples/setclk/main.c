/* $Id: main.c,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
*
* Command-line Radeon clocks setting utility.
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
* $Log: main.c,v $
* Revision 1.1.1.1  2002/01/25 22:14:25  VSi
* Imported to CVS
*
* ----------------------------------------------------------------------------
*                    Copyright (C) 2001-2002 Vahur Sinij„rv
* ----------------------------------------------------------------------------
*/

#define OPT_DEFAULT_BROKEN 1 // XXX-bw: --default crashes with my R9700.
#define OPT_DEFAULT_BROKEN_DEFCLK_CORE 276.75
#define OPT_DEFAULT_BROKEN_DEFCLK_MEM 270.00
#if 1
#  define IS_VALID_MEMCLK(mhz) (200.0 <= (mhz) && (mhz) <= 600.0)
#  define IS_VALID_CORECLK(mhz) (200.0 <= (mhz) && (mhz) <= 800.0)
#else
#  define IS_VALID_MEMCLK(mhz) (setclk_PLLData.min_freq <= (mhz) && (mhz) <= setclk_PLLData.max_freq)
#  define IS_VALID_CORECLK(mhz) (setclk_PLLData.min_freq <= (mhz) && (mhz) <= setclk_PLLData.max_freq)
#endif

#ifdef DEBUG
//#define EXPERIMENTAL
#endif

#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include "options.h"
#include "setclk.h"

void usage( void );
void paramTiming( int );
void paramLatency( int );
void showRadeonInfo();
void setDefaultClock();
void setSafeSettings();

const char logo[] =
"\n"
"---------------------------------------------------------------\n"
" Radeon (tm) clock setting utility by Vahur Sinijarv 1.01.0001 \n"
"---------------------------------------------------------------";

const char bar[] = "----------------------------------------------";

typedef struct
{
  int scrupd;
  int memLatency;
  int memTiming;
} PARAMS;

PARAMS params =
{
  0,     /* scrupd:     update screen  :def: do not update */
    -1,     /* memLatency: latency timing :def: do not change */
    -1      /* memTiming:  memory timing  :def: do not change */
};

extern int setclk_feature_divopt;

OPTDEF optdefs[] =
{
  { 'd', "default", OPT_EXEC,    setDefaultClock,    0 },
  { 'h', "help",    OPT_EXEC,    usage,              0 },
  { 'i', "info",    OPT_EXEC,    showRadeonInfo,     0 },
  { 'l', "memlat",  OPT_EXECINT, paramLatency,       0 },
  { 'm', "memory",  OPT_EXECINT, paramTiming,        0 },
  { 's', "safe",    OPT_EXEC,    setSafeSettings,    0 },
  { 'u', "updscr",  OPT_FLAG,    &params.scrupd,     1 },
  { 'o', "min_step",OPT_FLAG,    &setclk_feature_divopt, 1 },
  { '?', "usage",   OPT_EXEC,    usage,              0 },
};

#define NUMOPTS ( sizeof( optdefs ) / sizeof( optdefs[0] ) )

static void updateScreen()
{
  DEVMODE dm;

  if ( params.scrupd )
  {
    EnumDisplaySettings( NULL, ENUM_CURRENT_SETTINGS, &dm );
    ChangeDisplaySettings( &dm, 0 );
  }
}

static void end( int val )
{
  setclk_close();
  exit( val );
}

static void usage( void )
{
  puts( logo );
  puts( "\n"
    "Usage: setclk [switches] [core clock] [memory clock]\n"\
    "Switches:\n"
    "\t-d      --default  Set clocks to card defaults\n"
    "\t-h      --help     This message\n"
    "\t-i      --info     Print detailed info about card\n"
    "\t-l 0-1  --memlat   Set memory latency timing.\n"
    "\t                   0 = medium, 1 = fast\n"
    "\t-m 0-2  --memory   Set memory timing.\n"
    "\t                   0 = slow, 1 = medium, 2 = fast\n"
    "\t-s      --safe     Safe ( and slow ) settings\n"
    "\t                   Updates also memory timings\n"
    "\t-o      --min_step Minimizes intervals between frequency steps (EXP)\n"
    "\t-u      --updscr   Update screen\n"
    "\t-?      --usage    This message"
    );

  end( 0 );
}

void paramTiming( int timing )
{
  if ( timing > MTIM_LAST || timing < 0 )
  {
    printf( "Memory timing parameter out of range: %d\n", timing );
    end( -1 );
  }
  params.memTiming = timing;
}

void paramLatency( int latency )
{
  if ( latency > MLAT_LAST || latency < 0 )
  {
    printf( "Memory latency parameter out of range: %d\n", latency );
    end( -1 );
  }
  params.memLatency = latency ^ 1;
}

static void showClockInfo( void )
{
  float    coreclk;
  float    memclk;
  unsigned locked;
  char*    s;

  locked = (unsigned)setclk_getclock( &coreclk, &memclk );

  puts( bar );
  printf( "Radeon @ %.2f/%.2f (core/mem)\n"
    "Core & memory clocks %slocked\n",
    coreclk,
    memclk,
    locked ? "" : "un" );
  puts( bar );

  /*
  * Memory timing
  */
  switch( locked = setclk_getmtim() )
  {
  case MTIM_FAST:
    s = "fast";
    break;

  case MTIM_MEDIUM:
    s = "medium";
    break;

  case MTIM_SLOW:
    s = "slow";
    break;

  default:
    s = NULL;
  }

  printf( "Memory timing : " );
  if ( s )
    puts( s );
  else
    printf( "other (%08x)\n", locked );

  /*
  * Memory latency
  */
  switch( locked = setclk_getmlat() )
  {
  case MLAT_FAST:
    s = "fast";
    break;

  case MLAT_MEDIUM:
    s = "medium";
    break;

  default:
    s = NULL;
  }

  printf( "Memory latency: " );
  if ( s )
    puts( s );
  else
    printf( "other (%08x)\n", locked );
}

static int getClock( char* s, char* desc, float* d, int is_memclk )
{
  float clock = (float)atof( s );
#ifndef NBW // allow zero as special value representing the keep current clock flag
  if  (clock >= 1.0)
#endif
    if (!(is_memclk && IS_VALID_MEMCLK(clock) || (!is_memclk && IS_VALID_CORECLK(clock))))			
    {
      printf( "Invalid %s clock freq: %.2f MHz\n"\
        "PLL accepts clock freqs in range %.2f-%.2f MHz\n",
        desc,
        clock,
        setclk_PLLData.min_freq,
        setclk_PLLData.max_freq
        );

      return 0;
    }

    *d = clock;
    return 1;
}

static void setDefaultClock( void )
{
#if ! OPT_DEFAULT_BROKEN
  setclk_setclock( setclk_PLLData.xclk, setclk_PLLData.xclk, 1 );
#else
  setclk_setclock(OPT_DEFAULT_BROKEN_DEFCLK_CORE, OPT_DEFAULT_BROKEN_DEFCLK_MEM, 0);
#endif
  updateScreen();
  showClockInfo();
  end( 0 );
}

static void setSafeSettings( void )
{
  params.scrupd = 1;
  setclk_setmtim( MTIM_SLOW );
  setclk_setmlat( MLAT_MEDIUM );
  setDefaultClock();
}

static void showRadeonInfo()
{
  showClockInfo();

  puts( "\nPLL Data" );
  puts( bar );

  printf( "Default xclk      : %.2f MHz\n"
    "PLL reference freq: %.2f MHz\n"
    "PLL reference div : %d\n" 
    "PLL freq limits   : %.2f-%.2f MHz\n",
    setclk_PLLData.xclk,
    setclk_PLLData.reference_freq,
    setclk_PLLData.reference_div,
    setclk_PLLData.min_freq, setclk_PLLData.max_freq
    );

  end( 0 );
}

int main( int argc, char** argv )
{
  int   nopt;
  float coreClock;
  float memClock;
  int   locked;


  if ( !setclk_open() )
  {
    puts( "Cannot connect to r6probe" );
    return -1;
  }

  opt_errmsg = "Use -h or --help to get help";
  nopt = opt_parse( argc, argv, optdefs, NUMOPTS );



  /*
  * If we have leftover command-line arguments, assume they
  * are clocks.
  */
  switch( argc -= nopt )
  {
  case 0:
    break;

  case 1:
    if ( !getClock( argv[ nopt ], "synchronous", &coreClock, 0 ) )
    {
      end( -1 );
    }
    locked = 1;
    break;

  case 2:
    if ( !getClock( argv[ nopt ], "core", &coreClock, 0 ) )
    {
      end( -1 );
    }

    if ( !getClock( argv[ nopt + 1 ], "memory", &memClock, 1 ) )
    {
      end( -1 );
    }
    locked = 0;
    break;

  default:
    puts( "Too many arguments" );
    puts( opt_errmsg );
    end( -1 );
  }

  if ( argc )
  {
#ifndef NBW /* -bw: allow keeping current core or mem clock by passing zero */
    float currCoreClock, currMemClock;
    setclk_getclock(&currCoreClock, &currMemClock);
    if (coreClock < 1.0)
      coreClock = currCoreClock;
    if (memClock < 1.0)
      memClock = currMemClock;

#endif /* BW */
    setclk_setclock( coreClock, memClock, locked );
  }

  /*
  * Memory latency
  */
  if ( params.memLatency >= 0 )
  {
    setclk_setmlat( params.memLatency );
  }

  /*
  * Memory timings
  */
  if ( params.memTiming >= 0 )
  {
    setclk_setmtim( params.memTiming );
  }

  updateScreen();
  showClockInfo();
  end( 0 );
}