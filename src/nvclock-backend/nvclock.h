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

#ifndef NVCLOCK_H
#define NVCLOCK_H

#define MAX_CARDS 4

typedef enum
{
    ERROR=0, NVC_ERROR=0,
    INFO,
    WARNING
} error_type;

typedef enum
{
    SDR,
    DDR
} mem_type;

typedef enum
{
    AGP,
    PCI
} bus_type;

typedef enum
{
    UNKNOWN,
    NORMAL,
    NFORCE,
    MOBILE
} gpu_type;

struct pci_ids {
    short id;
    const char *name;
    short mem_min;
    short mem_max;
    short nv_min;
    short nv_max;
    gpu_type gpu;
};

struct card_list
{
    short device_id;
    unsigned int reg_address;
    short number; /* internal card number */
    char *bus_id; /* will be used to identify cards, since it is different for each card */
    gpu_type gpu;
    char *card_name; /* Name of the card */
    char *dev_name; /* file in /dev to open */
    volatile unsigned int mpll; /* default memory speed */
    volatile unsigned int nvpll; /* default gpu speed */
};

typedef struct Card {
    char *card_name; /* Name of the card */
    short supported;
    short device_id;
    int reg_address;
    char *bus_id;
    short base_freq;
    gpu_type gpu; /* Tells what type of gpu is used: mobile, nforce .. */
    short debug; /* Enable/Disable debug information */

    /* card registers */
    volatile unsigned int *PFB;
    volatile unsigned int *PBUS;
    volatile unsigned int *PMC;
    volatile unsigned int *PRAMDAC;
    volatile unsigned int *PEXTDEV;
    
    /* Overclock range of speeds */
    short nvclk_min; 
    short nvclk_max; 
    short memclk_min; 
    short memclk_max; 

    /* Memory info */
    int mem_type; /* needs to replace memory_type ?? */
    char* (*get_memory_type)(); /* Memory type: SDR/DDR */
    short (*get_memory_width)(); /* Memory width 64bit or 128bit */
    short (*get_memory_size)(); /* Amount of memory between 4 and 128 MB */

    /* AGP info */
    char* (*get_bus_type)(); /* Bus type: AGP/PCI */
    short (*get_agp_rate)(); /* Current AGP rate: 1, 2, 4  or 8*/
    char* (*get_agp_status)(); /* Current AGP status: Enabled/Disabled */
    char* (*get_fw_status)(); /* Current FW status: Enabled/Disabled */
    char* (*get_sba_status)(); /* Current SBA status: Enabled/Disabled */
    char* (*get_supported_agp_rates)(); /* Supported AGP rates */


    /* Overclocking */
    volatile unsigned int mpll; /* default memory speed */
    volatile unsigned int nvpll; /* default gpu speed */
    float (*get_gpu_speed)();
    void (*set_gpu_speed)(unsigned int nvclk);
    float (*get_memory_speed)();
    void (*set_memory_speed)(unsigned int memclk);
    void (*reset_speeds)();

} Card, *CardPtr;

extern Card nv_card;
extern struct card_list card[MAX_CARDS];

#ifdef __cplusplus
extern "C" {
#endif

int FindAllCards();
void set_card(int number);
void error(error_type code, const char *format, ...);

#ifdef __cplusplus    
};
#endif

#endif /* NVCLOCK_H */
