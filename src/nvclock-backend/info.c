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


#include "backend.h"
#include "nvclock.h"

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

/* This list isn't used much for the speed ranges anymore */
/* Mainly mobile gpu speeds are missing */
const static struct pci_ids ids[] = 
{ { 0x20, "nVidia Riva TnT", 75, 135, 75, 135, NORMAL },
{ 0x28, "nVidia Riva TnT 2 Pro", 125, 200, 125, 175, NORMAL },
{ 0x2a, "nVidia Riva TnT 2", 100, 175, 100, 150, NORMAL },
{ 0x2b, "nVidia Riva TnT 2", 100, 175, 100, 175, NORMAL },
{ 0xa0, "nVidia Riva TnT 2 Aladdin (Integrated)", 100, 175, 100, 175, NORMAL }, /* Integrated card but with real videomemory */
{ 0x2c, "nVidia Riva TnT 2 VANTA", 100, 175, 100, 175, NORMAL },
{ 0x2d, "nVidia Riva TnT 2 M64", 100, 175, 100, 175, NORMAL },
{ 0x2e, "nVidia Riva TnT 2 VANTA", 100, 175, 100, 175, NORMAL },
{ 0x2f, "nVidia Riva TnT 2 VANTA", 100, 175, 100, 175, NORMAL },	    
{ 0x29, "nVidia Riva TnT 2 Ultra", 150, 225, 150, 225, NORMAL },
{ 0x100, "nVidia Geforce 256 SDR", 150, 200, 100, 150, NORMAL },
{ 0x101, "nVidia Geforce 256 DDR", 250, 350, 100, 150, NORMAL },
{ 0x103, "nVidia Quadro", 250, 350, 100, 150, NORMAL },
{ 0x110, "nVidia Geforce 2 MX/MX400", 150, 250, 150, 250, NORMAL },
{ 0x111, "nVidia Geforce 2 MX 100/200", 150, 250, 125, 250, NORMAL },
{ 0x112, "nVidia Geforce 2 GO", 0, 0, 0, 0, MOBILE },
{ 0x113, "nVidia Quadro 2 MXR/EX/GO", 0, 0, 0, 0, MOBILE }, // what should we do with this one ? MOBILE or NORMAL?
{ 0x1a0, "nVidia Geforce 2 MX integrated", 0, 0, 150, 250, NFORCE },
{ 0x150, "nVidia Geforce 2 GTS/PRO", 250, 450, 175, 250, NORMAL },
{ 0x151, "nVidia Geforce 2 Ti", 300, 500, 175, 275, NORMAL },
{ 0x152, "nVidia Geforce 2 Ultra", 400, 550, 200, 275, NORMAL },
{ 0x153, "nVidia Quadro 2 Pro", 250, 400, 150, 250, NORMAL },
{ 0x170, "nVidia Geforce 4 MX460", 450, 600, 275, 350, NORMAL },
{ 0x171, "nVidia Geforce 4 MX440", 350, 500, 250, 325, NORMAL },
{ 0x172, "nVidia Geforce 4 MX420", 150, 250, 225, 300, NORMAL },
{ 0x173, "nVidia Geforce 4 MX440 SE", 250, 400, 225, 300, NORMAL }, //DDR based MX420? or is there a SDR too?
{ 0x174, "nVidia Geforce 4 440 Go", 0, 0, 0, 0, MOBILE },
{ 0x175, "nVidia Geforce 4 420 Go", 0, 0, 0, 0, MOBILE },
{ 0x176, "nVidia Geforce 4 420 Go 32M", 0, 0, 0, 0, MOBILE },
{ 0x177, "nVidia Geforce 4 460 Go", 0, 0, 0, 0, MOBILE },
{ 0x178, "nVidia Quadro 4 500 XGL", 150, 600, 150, 350, NORMAL }, // needs to be adjusted when the real speeds are known
{ 0x179, "nVidia Geforce 4 440 Go 64M", 0, 0, 0, 0, MOBILE },
{ 0x17a, "nVidia Quadro 4 200/400 NVS", 150, 250, 225, 300, NORMAL }, // needs to be adjusted when the real speeds are known
{ 0x17b, "nVidia Quadro 4 550 XGL", 375, 500, 250, 325, NORMAL },
{ 0x17c, "nVidia Quadro 4 GoGL", 0, 0, 0, 0, MOBILE },
{ 0x17d, "nVidia Geforce 410 Go", 0, 0, 0, 0, MOBILE },
{ 0x180, "nVidia Geforce 4 MX440 8X", 450, 700, 200, 400, NORMAL },
{ 0x181, "nVidia Geforce 4 MX440 8X", 450, 700, 200, 400, NORMAL },
{ 0x182, "nVidia Geforce 4 MX440SE 8X", 150, 300, 150, 400, NORMAL }, // needs to be adjusted when the real speeds are known
{ 0x183, "nVidia Geforce 4 MX420 8X", 150, 250, 225, 300, NORMAL },
{ 0x186, "nVidia NV18", 300, 600, 150, 400, NORMAL }, // needs to be adjusted when the real speeds are known
{ 0x187, "nVidia Geforce 4 488 Go", 300, 600, 150, 400, MOBILE }, // needs to be adjusted when the real speeds are known
{ 0x188, "nVidia Quadro 4 580 XGL", 300, 600, 200, 400, NORMAL }, // needs to be adjusted when the real speeds are known
{ 0x18a, "nVidia Quadro 4 280 NVS", 300, 600, 150, 400, NORMAL }, // needs to be adjusted when the real speeds are known
{ 0x18b, "nVidia Quadro 4 380 XGL", 300, 600, 200, 400, NORMAL }, // needs to be adjusted when the real speeds are known
{ 0x1f0, "nVidia Geforce 4 MX integrated",0, 0, 225, 300, NFORCE },
{ 0x200, "nVidia Geforce 3", 350, 550, 175, 275, NORMAL },
{ 0x201, "nVidia Geforce 3 Titanium 200", 350, 500, 175, 275, NORMAL },
{ 0x202, "nVidia Geforce 3 Titanium 500", 450, 600, 200, 300, NORMAL },
{ 0x203, "nVidia Quadro DCC", 350, 550, 175, 275, NORMAL },
{ 0x250, "nVidia Geforce 4 Ti 4600", 625, 750, 300, 400, NORMAL },
{ 0x251, "nVidia Geforce 4 Ti 4400", 450, 600, 275, 375, NORMAL },
{ 0x253, "nVidia Geforce 4 Ti 4200", 350, 600, 225, 325, NORMAL },
{ 0x258, "nVidia Quadro 4 900 XGL", 550, 700, 275, 350, NORMAL },
{ 0x259, "nVidia Quadro 4 750 XGL", 525, 650, 250, 325, NORMAL },
{ 0x25a, "nVidia Quadro 4 600 XGL", 525, 650, 250, 325, NORMAL },
{ 0x25b, "nVidia Quadro 4 700 XGL", 450, 600, 200, 300, NORMAL },
{ 0x280, "nVidia Geforce 4 Ti 4800", 625, 750, 300, 400, NORMAL }, /* Ti4600 with AGP 8x */
{ 0x281, "nVidia Geforce 4 Ti 4200 8X", 350, 600, 225, 325, NORMAL },
{ 0x282, "nVidia Geforce 4 Ti 4800SE", 450, 600, 200, 375, NORMAL }, /* Ti4400 with AGP 8x */
{ 0x286, "nVidia Geforce 4 4000 GO", 0, 0, 0, 0, MOBILE },
{ 0x288, "nVidia Quadro 4 980 XGL", 400, 900, 250, 450, NORMAL },
{ 0x289, "nVidia Quadro 4 780 XGL", 350, 600, 250, 375, NORMAL },
{ 0x28c, "nVidia Quadro 4 700 GoGL", 0, 0, 0, 0, MOBILE },
{ 0x300, "nVidia GeforceFX 5800", 500, 1000, 250, 500, NORMAL },
{ 0x301, "nVidia GeforceFX 5800 Ultra", 500, 1100, 250, 550, NORMAL }, /* 600/300 and 1000/500 */
{ 0x302, "nVidia GeforceFX 5800", 500, 1000, 250, 500, NORMAL }, /* 600/300 and 800/400 */
{ 0x308, "nVidia QuadroFX 2000", 500, 1100, 250, 550, NORMAL }, /* 600/300 and 800/400 */
{ 0x309, "nVidia QuadroFX 1000", 500, 800, 250, 400, NORMAL }, /* 600/300 */
{ 0x311, "nVidia GeforceFX 5600 Ultra", 600, 900, 300, 450, NORMAL }, /* 700/350 */
{ 0x312, "nVidia GeforceFX 5600", 450, 700, 300, 450, NORMAL }, /* 550/325 */
{ 0x314, "nVidia GeforceFX 5600SE", 450, 700, 300, 450, NORMAL }, /* 550/325 */
{ 0x318, "nVidia NV31GL", 750, 1000, 250, 500, NORMAL },
{ 0x319, "nVidia NV31GL", 750, 1000, 250, 500, NORMAL },
{ 0x31a, "nVidia GeforceFX Go 5600", 0, 0, 0, 0, MOBILE },
{ 0x31b, "nVidia GeforceFX Go 5650", 0, 0, 0, 0, MOBILE },
{ 0x31c, "nVidia QuadroFX Go700", 0, 0, 0, 0, MOBILE },
{ 0x321, "nVidia GeforceFX 5200 Ultra", 550, 900, 275, 450, NORMAL }, /* 650/325 */
{ 0x322, "nVidia GeforceFX 5200", 350, 500, 200, 350, NORMAL }, /* 400/250 */
{ 0x323, "nVidia NV34", 750, 1000, 250, 500, NORMAL },
{ 0x324, "nVidia GeforceFX Go 5200", 0, 0, 0, 0, MOBILE },
{ 0x325, "nVidia GeforceFX Go 5250", 0, 0, 0, 0, MOBILE },
{ 0x32a, "nVidia NV34GL", 750, 1100, 250, 550, NORMAL },
{ 0x32b, "nVidia QuadroFX 500", 300, 600, 200, 400, NORMAL }, /* needs adjustments */
{ 0x32f, "nVidia NV34GL", 750, 1100, 250, 550, NORMAL },
{ 0x330, "nVidia GeforceFX 5900 Ultra", 750, 1000, 250, 500, NORMAL },
{ 0x331, "nVidia GeforceFX 5900", 750, 1000, 250, 500, NORMAL },
{ 0x338, "nVidia GeforceFX 3000", 750, 1000, 250, 500, NORMAL },
{ 0x2a0, "nVidia Xbox GPU", 0, 0, 0, 0, NFORCE },
{ 0, NULL, 0, 0, 0, 0, 0 }
};


const char *get_card_name(int device_id, gpu_type *gpu)
{
    struct pci_ids *nv_ids = (struct pci_ids*)ids;
    
    while(nv_ids->id != 0)
    {
	if(nv_ids->id == device_id)
	{
	    *gpu = nv_ids->gpu;
	    return nv_ids->name;
	}

	nv_ids++;
    }	

    /* if !found */
    *gpu = UNKNOWN;
    return "Unknown Nvidia card";
}

/* Set the speed ranges */
void set_speed_range()
{
    int found = 0;
    float memclk, nvclk;

    /* Find out the reference frequency */    
    if(nv_card.device_id >= 0x100)
    {
	/* The Geforce4/GeforceFX use a base frequency of 27MHz */
	nv_card.base_freq = (nv_card.PEXTDEV[0x0000/4] &(1<<6) ) ? 14318 : (nv_card.PEXTDEV[0x0000/4] & (1<<22)) ? 27000 : 13500;
    }
    else
    {
	nv_card.base_freq = (nv_card.PEXTDEV[0x0000/4] & 0x40) ? 14318 : 13500;
    }

    memclk = calc_pll(nv_card.PRAMDAC[0x504/4], nv_card.base_freq, nv_card.device_id, 0);
    nvclk = calc_pll(nv_card.PRAMDAC[0x500/4], nv_card.base_freq, nv_card.device_id, 0);

    nv_card.memclk_min = (int)(memclk * .75);
    nv_card.memclk_max = (int)(memclk * 1.25);
    nv_card.nvclk_min = (int)(nvclk * .75);
    nv_card.nvclk_max = (int)(nvclk * 1.25);

    /* 
	Hack.
	Nvidia was so nice to ship support both DDR and SDR memory on some gf2mx and gf4mx cards :(
	Because of this the speed ranges of the memory speed can be different.
	Check if the card is a gf2mx/gf4mx using SDR and if the speed is "too" high.
	Then adjust the speed range.
    */ 
    if((nv_card.device_id == 0x110 || nv_card.device_id == 0x111 \
     || nv_card.device_id == 0x172 || nv_card.device_id == 0x182 \
     || nv_card.device_id == 0x183) && ((nv_card.PFB[0x200/4] & 0x1) == SDR) && memclk > 280)
    {
	nv_card.memclk_min /= 2;
	nv_card.memclk_max /= 2;
    }
}

/* Needs better bus checks .. return a string ?*/
short get_agp_rate()
{
    int agp_rate, agp_status;
    
    /* If the card is an AGP card */
    if(((nv_card.PEXTDEV[0x0/4]  >> 14) & 0x1) == 0x1)
    {
	agp_status = nv_card.PMC[0x1848/4];
	agp_rate = nv_card.PMC[0x184c/4] & 0x7;

	/* If true, the user has AGP8x support */
	if(agp_status & 0x8)
	{
	    agp_rate <<= 2;
	}
	return agp_rate;
    }
    else
    {
	return 0;
    }
}

char* get_bus_type()
{
    return ((nv_card.PEXTDEV[0x0/4]  >> 14) & 0x1) ? "AGP" : "PCI";
}

char* get_agp_status()
{
    return ((nv_card.PMC[0x184c/4] >> 8) & 0x1) ? "Enabled" : "Disabled";
}

char* get_fw_status()
{
    /* Check if Fast Writes is supported by the hostbridge */
    if(((nv_card.PMC[0x1848/4] >> 4) & 0x1) == 1)
	return ((nv_card.PMC[0x184c/4] >> 4) & 0x1) ? "Enabled" : "Disabled";
    else
	return "Unsupported";
}

char* get_sba_status()
{
    /* Check if Fast Writes is supported by the hostbridge */
    if(((nv_card.PMC[0x1848/4] >> 9) & 0x1) == 1)
        return ((nv_card.PMC[0x184c/4] >> 9) & 0x1) ? "Enabled" : "Disabled";
    else
	return "Unsupported";
}

char* get_supported_agp_rates()
{
    int agp_rates, agp_status, i;
    static char *rate;

    agp_status = nv_card.PMC[0x1848/4];
    agp_rates = nv_card.PMC[0x1848/4] & 0x7;

    /* If true, the user has AGP8x support */
    if(agp_status & 0x8)
    {
        agp_rates <<= 2;
    }

    rate = (char*)calloc(1, sizeof(char));
	
    for(i=1; i <= 8; i*=2)
    {
        if(agp_rates & i)
        {
    	    char *temp = (char*)malloc(4 * sizeof(char));
    	    sprintf(temp, "%dX ", i);
    	    rate = (char*)realloc(rate, strlen(rate)+4); //+3
    	    rate = strcat(rate, temp);
    	    free(temp);
	}
    }

    return rate;
}

short get_memory_width()
{
    /* Nforce / Nforce2 */
    if((nv_card.device_id == 0x1a0) || (nv_card.device_id == 0x1f0))
	return 64;
    else
    /* If nv3x should we assume 128==256 and 64==128?? */
	return (nv_card.PEXTDEV[0x0/4] & 0x17) ? 128 : 64;
}

char* get_memory_type()
{
    /* Nforce / Nforce2 */
    if((nv_card.device_id == 0x1a0) || (nv_card.device_id == 0x1f0))
	return ((pciReadLong(0, 0, 1, 0x7c) >> 12) & 0x1) ? "DDR" : "SDR";
    else    
	return (nv_card.PFB[0x200/4] & 0x01) ? "DDR" : "SDR";
}

short get_memory_size()
{
    short memory_size;
    
    /* If the card is something TNT based the calculation of the memory is different. */
    if(nv_card.device_id < 0x100)
    {
	if(nv_card.PFB[0x0/4] & 0x100)
	    memory_size = ((nv_card.PFB[0x0/4] >> 12) & 0xf)*2+2;
	else
	{
	    switch(nv_card.PFB[0x0/4] & 0x3)
	    {
		case 0:
		    memory_size = 32;
		    break;
		case 1:
		    memory_size = 4;
		    break;
		case 2:
		    memory_size = 8;
		    break;
		case 3:
		    memory_size = 16;
		    break;
		default:
		    memory_size = 16;
		    break;    	    
	    }
	}
    }
    /* Nforce 1 */
    else if(nv_card.device_id == 0x1a0)
    {
	long temp = pciReadLong(0, 0, 1, 0x7c);
	memory_size = (short)(((temp >> 6) & 0x31) + 1);
    }
    /* Nforce2 */
    else if(nv_card.device_id == 0x1f0)
    {
	long temp = pciReadLong(0, 0, 1, 0x84);
	memory_size = (short)(((temp >> 4) & 0x127) + 1);
    }
    /* Memory calculation for geforce cards or better. */
    else
    {
	/* 
	    Atleast nv3x boards are available with 256MB memory.
	    Not sure if previous boards were available with that much
	    else make this so for those cards too.
	*/
	if((nv_card.device_id & 0xf00) == 0x300)
	    memory_size = (nv_card.PFB[0x20c/4] >> 20) & 0xfff;
	else
	    memory_size = (nv_card.PFB[0x20c/4] >> 20) & 0xff;
    }

    return memory_size;
}

/* Temp hack */
/* Use this function for pci cards when the client wants AGP info */
char* dummy_str()
{
    return "-";
}

void dummy(unsigned n)
{
  (void)n;
}

void dummy_void(void)
{
}
