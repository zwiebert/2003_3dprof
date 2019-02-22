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

#include <math.h>
#include <stdio.h>

typedef enum
{
    SET_MEMORY,
    SET_DDR_MEMORY,
    SET_GPU
} Action;

int CalcSpeed(int base_freq, int m, int n, int p)
{
    return (int)((float)n/(float)m * base_freq) >> p;
}

float calc_pll(int pll, int base_freq, int device_id, short debug)
{
   extern int CalcSpeed_nv30(int base_freq, int m1, int m2, int n1, int n2, int p);
    /* NV3X */
    if((device_id & 0xf00) == 0x300)
    {
	/* If bit 7 is set use the new algorithm */
	if(nv_card.PRAMDAC[0x504/4] & 0x80)
	{
	    int m1, m2, n1, n2, p;
    	    m1 = NV30_PLL_M1(pll);
	    m2 = NV30_PLL_M2(pll);
    	    n1 = NV30_PLL_N1(pll);
	    n2 = NV30_PLL_N2(pll);
    	    p = NV30_PLL_P(pll);
	    
	    if(debug == 1)
		printf("m1=%d m2=%d n1=%d n2=%d p=%d\n", m1, m2, n1, n2, p);
	    
	    return (float)CalcSpeed_nv30(base_freq, m1, m2, n1, n2, p)/1000;
	}
	else
	{
	    int m, n, p;
    	    m = NV30_PLL_M1(pll);
    	    n = NV30_PLL_N1(pll);
    	    p = NV30_PLL_P(pll);

	    if(debug == 1)
		printf("m=%d n=%d p=%d\n", m, n, p);
	    
	    return (float)CalcSpeed(base_freq, m, n, p)/1000;
	}
    }
    else
    {
	int m, n, p;

	m = pll & 0xff;
	n = (pll >> 8) & 0xff;
	p = (pll >> 16) & 0x0f;
	
	if(debug == 1)
	    printf("m=%d n=%d p=%d\n", m, n, p);

	return (float)CalcSpeed(base_freq, m, n, p)/1000;
    }
}

/* Calculate the requested speed. */
static void ClockSelect(int base_freq, int clockIn, int *PLL, Action action)
{
    int m, n, p, bestm, bestn, bestp;
    int diff, diffOld, mlow, mhigh, nlow, nhigh, plow, phigh;
    int level = 0;
    diffOld = clockIn;


    if(base_freq == 14318)
    {
	mlow = 7;
	mhigh = 14;
	nlow = 14;
	nhigh = 255;
    }
    else
    {
	mlow = 6;
	mhigh = 13;
	nlow = 14;
	nhigh = 255;
    }

    if(clockIn > 250000)
    {
        mlow = 1;
        mhigh = 6;
        nlow = 14;
        nhigh = 93;
    }
    if(clockIn > 340000)
    {
    /*
	When DDR memory is used we should perhaps force mhigh to 1, since
	on some cards the framebuffer needs to be reinitialized and image corruption
	can occur.
    */
        mlow = 1;
        mhigh = 2;
        nlow = 14;
        nhigh = 93;
    }
    
    /* 
	postdivider locking to improve stability.
	in the near future we will provide some tuning options for the
	overclocking algorithm which will extend this.
    */    
    plow = (*PLL >> 16) & 0x0f;
    phigh = (*PLL >> 16) & 0x0f;
    
    /*
	Calculate the m and n values. There are a lot of values which give the same speed;
	We choose the speed for which the difference with the request speed is as small as possible.
    */
    for(p = plow; p <= phigh; p++)
    {
	for(m = mlow; m <= mhigh; m++)
	{
	    for(n = nlow; n <= nhigh; n++)
	    {
		diff = abs((int)(clockIn - CalcSpeed(base_freq, m, n, p)));

		/* When the new difference is smaller than the old one, use this one */
		if(diff < diffOld)
		{
		    diffOld = diff;
		    bestm = m;
		    bestn = n;
		    bestp = p;

#if 0
		    /* When the difference is 0 or less than .5% accept the speed */		    
		    if(((diff == 0) || ((float)diff/(float)clockIn <= 0.005)))
		    {
			*PLL = ((int)bestp << 16) + ((int)bestn << 8) + bestm;
			return;
		    }
#endif
		}	    
	    }
	}
    }


    *PLL = ((int)bestp << 16) + ((int)bestn << 8) + bestm;
    return;
}


void set_gpu_speed(unsigned int clk)
{
    int PLL = nv_card.PRAMDAC[0x500/4];

    /* MHz -> KHz */
    clk *= 1000;

    /* HERE the new clocks are selected (in KHz). */
    ClockSelect(nv_card.base_freq, clk, &PLL, SET_GPU);

    /* Unlock the programmable NVPLL/MPLL */
    nv_card.PRAMDAC[0x50c/4] |= 0x500;

    /* Overclock */
    nv_card.PRAMDAC[0x500/4] = PLL;
}

void set_memory_speed(unsigned int clk)
{
    int PLL;

    /* MHz -> KHz */
    clk *= 1000;
    /* This is a workaround meant for some Geforce2 MX/Geforce4 MX cards
    *  using SDR memory. Gf2MX/Gf4MX cards use 4x16 SDR memory report
    *  twice as high clockspeeds. I call that "fake ddr".
    *  By detecting the memory type, pci id and clockspeed we check
    *  if this occurs. It is a workaround.
    */
    if(nv_card.mem_type == SDR && ( nv_card.device_id == 0x110 || nv_card.device_id == 0x111 
	|| nv_card.device_id == 0x172 || nv_card.device_id == 0x17a))
    {
	if(calc_pll(nv_card.PRAMDAC[0x504/4], nv_card.base_freq, nv_card.device_id, 0) > 280000) clk *= 2;
    }

    PLL = nv_card.PRAMDAC[0x504/4];

    /* HERE the new clocks are selected (in KHz). */
    if(nv_card.mem_type == DDR)
        ClockSelect(nv_card.base_freq, clk, &PLL, SET_DDR_MEMORY);
    else
        ClockSelect(nv_card.base_freq, clk, &PLL, SET_MEMORY);

    /* Unlock the programmable NVPLL/MPLL */
    nv_card.PRAMDAC[0x50c/4] |= 0x500;

    /* Overclock */
    nv_card.PRAMDAC[0x504/4] = PLL;
}

float get_gpu_speed()
{
    /* Unlock the programmable NVPLL/MPLL */
    nv_card.PRAMDAC[0x50c/4] |= 0x500;

    if(nv_card.debug == 1)
    {
	printf("NVPLL_COEFF=%08x\n", nv_card.PRAMDAC[0x500/4]);
    }
    
    return ((float)calc_pll(nv_card.PRAMDAC[0x500/4], nv_card.base_freq, nv_card.device_id, nv_card.debug));
}

float get_memory_speed()
{
    int factor = 1;

    /* Unlock the programmable NVPLL/MPLL */
    nv_card.PRAMDAC[0x50c/4] |= 0x500;
    
    /* This is a workaround meant for some Geforce2 MX/Geforce4 MX cards
    *  using SDR memory. Gf2MX/Gf4MX cards use 4x16 SDR memory report
    *  twice as high clockspeeds. I call that "fake ddr".
    *  By detecting the memory type, pci id and clockspeed we check
    *  if this occurs. It is a workaround. We divide the memclk later by 2.
    */
    if(nv_card.mem_type == SDR && ( nv_card.device_id == 0x110 || nv_card.device_id == 0x111 ||
    nv_card.device_id == 0x172 || nv_card.device_id == 0x17a || nv_card.device_id == 0x182 \
    || nv_card.device_id == 0x183))
    {
	if(calc_pll(nv_card.PRAMDAC[0x504/4], nv_card.base_freq, nv_card.device_id, 0) > 280)
	{
	    factor = 2;
	}
    }

    if(nv_card.debug == 1)
    {
	printf("MPLL_COEFF=%08x\n", nv_card.PRAMDAC[0x504/4]);
    }

    return ((float)calc_pll(nv_card.PRAMDAC[0x504/4], nv_card.base_freq, nv_card.device_id, nv_card.debug)) / factor;
}

float get_nforce_memory_speed()
{
    unsigned short p = (unsigned short)((pciReadLong(0, 0, 3, 0x6c) >> 8) & 0xf);
    if(!p) p = 4;
    return 400.0f / (float)p;
}

void reset_speeds()
{
    /* Unlock the programmable NVPLL/MPLL */
    nv_card.PRAMDAC[0x50c/4] |= 0x500;
    
    /* Don't support this on nv3x cards yet */
    if((nv_card.device_id & 0xf00) == 0x300)
	return;
	
    nv_card.PRAMDAC[0x500/4] = nv_card.nvpll;

    /* Don't overclock the memory of integrated GPUs */
    if(nv_card.gpu == NFORCE)
	return;
	
    nv_card.PRAMDAC[0x504/4] = nv_card.mpll;
}
