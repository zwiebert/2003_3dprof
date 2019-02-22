/* NVClock 0.7 - Linux overclocker for NVIDIA cards
 *
 * site: http://nvclock.sourceforge.net
 *
 * Copyright(C) 2001-2003 Roderick Colenbrander
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA
 */

#include "nvclock.h"
#include "backend.h"
#include <stdio.h>

int CalcSpeed_nv30(int base_freq, int m1, int m2, int n1, int n2, int p)
{
    return (int)((float)(n1*n2)/(m1*m2) * base_freq) >> p;
}

float get_nv30_gpu_speed(float *memclk, float *nvclk)
{
    /* Unlock the programmable NVPLL/MPLL */
    nv_card.PRAMDAC[0x50c/4] |= 0x500;

    if(nv_card.debug == 1)
    {
        printf("NVPLL_COEFF=%08x\n",  nv_card.PRAMDAC[0x500/4]);
    }

    return (float)calc_pll(nv_card.PRAMDAC[0x500/4], 27000, 0x300, nv_card.debug);
}

float get_nv30_memory_speed()
{
    /* Unlock the programmable NVPLL/MPLL */
    nv_card.PRAMDAC[0x50c/4] |= 0x504;

    if(nv_card.debug == 1)
    {
        printf("MPLL_COEFF=%08x\n",  nv_card.PRAMDAC[0x504/4]);
    }

    return (float)calc_pll(nv_card.PRAMDAC[0x504/4], 27000, 0x300, nv_card.debug);
}

void set_nv30_gpu_speed(unsigned int nvclk)
{
}

void set_nv30_memory_speed(unsigned int memclk)
{
}

