/* NVClock 0.7 - Linux overclocker for NVIDIA cards
 * 
 * Copyright(C) 2001-2003 Roderick Colenbrander
 *
 * site: http://nvclock.sourceforge.net
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

/* Thanks to Alexey Nicolaychuk (Unwinder), the author of Rivatuner, for providing
   these macros to get nv30 clock detection working.
*/
#define NV30_PLL_M1(PLL) ( ( PLL ) & 0x0f )
#define NV30_PLL_M2(PLL) ( ( ( PLL )  >>  4 ) & 0x07 )
#define NV30_PLL_N1(PLL) ( ( ( PLL ) >> 8 ) & 0xff )
#define NV30_PLL_N2(PLL) ( ( ( ( PLL ) >> 19 ) & 0x07 ) | ( ( ( PLL ) >> 21 ) & 0x18 ) )
#define NV30_PLL_P(PLL) ( ( ( PLL ) >> 16 ) & 0x07 )


/* Set the card object to the requested card */
void set_card(int number);
float calc_pll(int pll, int base_freq, int device_id, short debug);

/* AGP related functions */
short get_agp_rate();
char* get_agp_status();
char* get_bus_type();
char* get_sba_status();
char* get_sba_support();
char* get_fw_status();
char* get_fw_support();
char* get_supported_agp_rates();

/* Memory related functions */
short get_memory_size();
char* get_memory_type();
short get_memory_width();

/* Some internally needed functions */
const char* get_card_name(int device_id, gpu_type *gpu);
void set_speed_range();
char* dummy_str();
void dummy();
long pciReadLong(short dev, short bus, short function, long offset);

/* Overclocking related functions */
float get_gpu_speed();
float get_nv30_gpu_speed();
float get_memory_speed();
float get_nforce_memory_speed();
float get_nv30_memory_speed();
void set_gpu_speed(unsigned int nvclk);
void set_nv30_gpu_speed(unsigned int nvclk);
void set_memory_speed(unsigned int memclk);
void set_nv30_memory_speed(unsigned int memclk);
void reset_speeds();
