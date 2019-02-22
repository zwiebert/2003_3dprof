/* $Id: R6PROBE.H,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
 *
 * Header file for r6probe helper library
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
 * $Log: R6PROBE.H,v $
 * Revision 1.1.1.1  2002/01/25 22:14:25  VSi
 * Imported to CVS
 *
 * ----------------------------------------------------------------------------
 *                    Copyright (C) 2001-2002 Vahur Sinij„rv
 * ----------------------------------------------------------------------------
 */
#ifndef R6PROBE_H
#define R6PROBE_H

#ifdef __cplusplus
extern "C"
{
#endif

/*
 * R6 probe is a low level Windows driver, which connects to Radeon
 * and makes it's registers and memory accessible to ring 3 applications.
 * Here are functions, which help you to access the driver.
 *
 * To use the library, first open it, by calling r6probe_open() and check
 * the return value. If it's 0, the library could not connect to low level
 * driver, otherwise open succeeded. Other functions are pretty clear I
 * guess. If you want to set radeon clocks, look at the setclk.h instead.
 */

typedef struct
{
    unsigned bus;
    unsigned dev;
    unsigned fbPhy;
    unsigned fb;
    unsigned fbSize;
    unsigned mmrPhy;
    unsigned mmr;
    unsigned bios;
} R6ADAPTER_INFO;

int      __cdecl r6probe_open(void);
void     __cdecl r6probe_close(void);
unsigned __cdecl r6probe_readmmr( unsigned addr );
unsigned __cdecl r6probe_readpll( unsigned addr );
unsigned __cdecl r6probe_readpci( unsigned addr );
unsigned __cdecl r6probe_readbios( unsigned addr );
unsigned __cdecl r6probe_readbiosblk( void* dest, unsigned addr, unsigned count );
void     __cdecl r6probe_getadapterinfo( R6ADAPTER_INFO* );
unsigned __cdecl r6probe_getversion(void);
void     __cdecl r6probe_writemmr(unsigned addr, unsigned mask, unsigned data);
void     __cdecl r6probe_writepll(unsigned addr, unsigned mask, unsigned data);
#ifdef __cplusplus
}
#endif

#endif
