/* NVClock 0.7 - MS Windows overclocker for NVIDIA cards
 *
 * Site: http://nvclock.sourceforge.net
 *
 * Copyright(C) 2001-2003 Roderick Colenbrander
 * Portions Copyright(C) 2003 Bert Winkelmann <bertw@gmx.net>
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


/* Problem: FindAllCards() returns 0 if found 1 card and nvlock.c depends on it.
It would be better to return 0 if no card was found, and a negative value for an error. 
-bw
*/


#include "win32nt_get_io.h" /* currently unused */

#define nvc_error error
#include "nvclock.h"
#include "backend.h"
#include "nvidia_regs.h"

#include <windows.h>
#include "WinIO.h"
#define PCI_HDR_GET_DEV(p) ((p)->Slot >> 3)
#define PCI_HDR_GET_FN(p) ((p)->Slot & 0x7)
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <errno.h>


// === exported data ===
Card nv_card;
struct card_list card[MAX_CARDS];

// === exported functions ===
int FindAllCards(void);
void set_card(int number);
long pciReadLong(short dev, short bus, short function, long offset);

// === local data ===


// === local functions ===
static int IsVideoCard(struct winio_pci_common_header *pci_hdr);
static void set_default_speeds(int num);
static int map_mem(const char* dev_name);
static void unmap_mem(void);
static void *map_dev_mem (unsigned long base, unsigned long size);
static void unmap_dev_mem(volatile void *base);
static void dummy_unsigned(unsigned n) { (void)n; } 


/*  Detect all nVidia cards using ioctl's or using /proc when the 
closed source nVidia drivers aren't installed/loaded.

Return codes:
>=   0 nvclock found 1 or more nvidia videocards
-1 nvclock couldn't find any nvidia cards
-2 the NVdriver isn't loaded and the user isn't root.
Only root users can have access to the card in this case since we
we need access to /dev/mem. 
*/
#if USE_SETUPAPI_DLL
int FindAllCards()
{
  int dev = 0, dum = 0,  i = -1;
  unsigned short devbusfn = 0;

  i++;
  card[i].device_id = get_device_id();
  card[i].number = 0;
  card[i].card_name = (char*)get_card_name(card[i].device_id, &card[i].gpu);
  card[i].bus_id = (char*)malloc(8 * sizeof(char));
  sprintf(card[i].bus_id, "%02d:%02d.%d", devbusfn >> 8, (devbusfn & 0xff) >> 3, (devbusfn & 0xff) & 0x7);
  card[i].reg_address = get_mem_base(0);

  set_default_speeds(i);

  return i;
}

#endif /* USE_SETUPAPI_DLL */
/* Set the card object to the requested card */
void set_card(int number)
{
  /* Some simple check to see if we are using nv_card for the first time or not */
  if(nv_card.device_id != 0)
  {
    unmap_mem();
  }

  nv_card.card_name = card[number].card_name;
  nv_card.device_id = card[number].device_id;
  nv_card.bus_id = strdup(card[number].bus_id);
  nv_card.reg_address = card[number].reg_address;
  nv_card.gpu = card[number].gpu;

  if(map_mem(card[number].dev_name) == -1)
#ifdef NBW /* -bw BUGFIX */
    return; /* the client will take care of the rest using error ... */
#else
    /* fixed code */
    error(ERROR, "Cannot access GPU register space"); // no error handling in nvclock.c
#endif

  /* Set the speed range */
  set_speed_range();

  /* Set the default pll's */
  nv_card.mpll = card[number].mpll;
  nv_card.nvpll = card[number].nvpll;

  /* Find out what memory is being used */
  nv_card.mem_type = (nv_card.PFB[0x200/4] & 0x01) ? DDR : SDR;

  nv_card.get_bus_type = get_bus_type;

  if(strcmp(nv_card.get_bus_type(), "AGP") == 0)
  {
    nv_card.get_agp_rate = get_agp_rate;
    nv_card.get_agp_status = get_agp_status;
    nv_card.get_fw_status = get_fw_status;
    nv_card.get_sba_status = get_sba_status;
    nv_card.get_supported_agp_rates = get_supported_agp_rates;
  }
  else
  {
    nv_card.get_agp_rate = get_agp_rate;
    nv_card.get_agp_status = dummy_str;
    nv_card.get_fw_status = dummy_str;
    nv_card.get_sba_status = dummy_str;
    nv_card.get_supported_agp_rates = dummy_str;
  }


  nv_card.get_memory_size = get_memory_size;
  nv_card.get_memory_type = get_memory_type;
  nv_card.get_memory_width = get_memory_width;

  /* Depending on the gpu type, we provide some functions */
  /* We do this since we don't want overclocking on all types of cards */
  switch(card[number].gpu)
  {
  case NORMAL:
    /* Use different clocking functions for nv30 based cards */
    if(((nv_card.device_id & 0xf00) == 0x300) && (nv_card.device_id != 0x322))
    {
      nvc_error(INFO, "Overclocking of GeforceFX hardware is not supported at the moment");
      nv_card.get_gpu_speed = get_nv30_gpu_speed;
      nv_card.get_memory_speed = get_nv30_memory_speed;
      nv_card.set_memory_speed = set_nv30_memory_speed;
      nv_card.set_gpu_speed = set_nv30_gpu_speed;
      nv_card.reset_speeds = reset_speeds;
    }
    else
    {
      nv_card.get_gpu_speed = get_gpu_speed;
      nv_card.set_gpu_speed = set_gpu_speed;
      nv_card.get_memory_speed = get_memory_speed;
      nv_card.set_memory_speed = set_memory_speed;
      nv_card.reset_speeds = reset_speeds;
    }
    break;

  case NFORCE:
    nvc_error(INFO, "Memory Overclocking isn't supported on Nforce cards");
    nv_card.get_gpu_speed = get_gpu_speed;
    nv_card.set_gpu_speed = set_gpu_speed;
    nv_card.get_memory_speed = get_nforce_memory_speed;
    nv_card.set_memory_speed = dummy_unsigned;
    nv_card.reset_speeds = reset_speeds;
    break;

  case MOBILE:
    nvc_error(INFO, "Overclocking isn't supported on mobile GPUs");
    nv_card.get_gpu_speed = get_gpu_speed;
    nv_card.set_gpu_speed = dummy_unsigned;
    nv_card.get_memory_speed = get_memory_speed;
    nv_card.set_memory_speed = dummy_unsigned;
    nv_card.reset_speeds = dummy;
    break;
  }
}

/*
Some new hacky function to add a way to restore
the card to its default speeds. Since bios parsing
is way too complicated, we fall back to a sort of
config file that will hold the speeds till the user
reboots.
*/
#define def_speed_file "def_clocks.txt"
static void set_default_speeds(int num)
{
  int i, eof;
  char buffer[100];
  volatile unsigned int *PEXTDEV;
  volatile unsigned int *PRAMDAC;
  FILE *fp;

  if((fp = fopen(def_speed_file, "r")) != NULL)
  {
    /* Read the file .. */
    for(eof = fscanf(fp, "%s",&buffer); eof != EOF; eof = fscanf(fp, "%s", &buffer))
    {
      if( strstr(buffer, "card=") != 0)
      {
        int number, mpll, nvpll;
        sscanf(buffer, "card=%x", &number);

        fscanf(fp, "%s", &buffer);
        sscanf(buffer, "mpll=%x", &mpll);
        card[number].mpll = mpll;

        fscanf(fp, "%s", &buffer);
        sscanf(buffer, "nvpll=%x", &nvpll);
        card[number].nvpll = nvpll;
      }
      continue;    
    }
    fclose(fp);
  }
  else
  {
    /* Write the config file */
    fp = fopen(def_speed_file, "w+");
    fprintf(fp, "#This file is used by nvclock to be able to restore the card's speeds\n#Do not edit this file!\n");

    for(i = 0; i <= num; i++)
    {
      /* Only map the registers we need */
      PEXTDEV = (unsigned *)map_dev_mem(card[i].reg_address + 0x101000, 0xf);
      PRAMDAC = (unsigned *)map_dev_mem(card[i].reg_address + 0x680500, 0xf);

      fprintf(fp, "card=%d\n", i);
      fprintf(fp, "mpll=%08x\n", PRAMDAC[0x4/4]);
      fprintf(fp, "nvpll=%08x\n", PRAMDAC[0x0/4]);

      card[i].nvpll = PRAMDAC[0x0/4];
      card[i].mpll = PRAMDAC[0x4/4];
      unmap_dev_mem(PEXTDEV);
      unmap_dev_mem(PRAMDAC);
    }
    fclose(fp);
  }
}

/* Check if the device is a videocard */
static int IsVideoCard(struct winio_pci_common_header *pci_hdr)
{
  return pci_hdr->BaseClass == 0x03;
}


#if 1
#include "WinIO.h"
static void g() {
  struct winio_pci_common_header pci_hdr = {0, };
  while (winio_get_pci_header(0, &pci_hdr)) {
    printf("%hx\n", pci_hdr.DeviceID);
  }

}
#endif


static int map_mem(const char *dev_name)
{
  (void)dev_name;


  /* Map the registers of the nVidia chip */
  nv_card.PEXTDEV = (unsigned *)map_dev_mem((nv_card.reg_address
    + NVIDIA_EXTDEV_BYTE_OFFSET), NVIDIA_EXTDEV_BYTE_SIZE);
  nv_card.PFB     = (unsigned *)map_dev_mem((nv_card.reg_address
    + NVIDIA_FB_BYTE_OFFSET), NVIDIA_FB_BYTE_SIZE);
  nv_card.PMC     = (unsigned *)map_dev_mem((nv_card.reg_address
    + NVIDIA_MC_BYTE_OFFSET), NVIDIA_MC_BYTE_SIZE);
  nv_card.PRAMDAC = (unsigned *)map_dev_mem((nv_card.reg_address
    + NVIDIA_RAMDAC_BYTE_OFFSET), NVIDIA_RAMDAC_BYTE_SIZE);

  if (!nv_card.PEXTDEV || !nv_card.PFB || !nv_card.PMC || !nv_card.PRAMDAC) {
    unmap_mem();
    return -1;
  }
  return 0;
}

static void unmap_mem()
{
  unmap_dev_mem(nv_card.PEXTDEV);
  unmap_dev_mem(nv_card.PFB);
  unmap_dev_mem(nv_card.PMC);
  unmap_dev_mem(nv_card.PRAMDAC);
}


/* Map/UnMap wrapper */

static BOOL module_winio_init(void);
static WinIO winio_instance;

static void *map_dev_mem (unsigned long base, unsigned long size)
{
  if (!module_winio_init())
    return NULL;
  return MapIO(winio_instance, base, size);
}

static void unmap_dev_mem(volatile unsigned *base)
{
  if (!module_winio_init())
    return;
  UnMapIO((void *)winio_instance, base);
}

static void module_winio_exit(void)
{
  if (winio_instance) {
    ShutdownWinIo(winio_instance);
    winio_instance = NULL;
  }
}

static BOOL module_winio_init()
{
  if (!winio_instance) {
    if (!(winio_instance = InitializeWinIo())) {
      InstallWinIoDriver("WinIO.sys", FALSE);
      winio_instance = InitializeWinIo();
    }
    if (winio_instance)
      atexit(module_winio_exit);
  }

  return (winio_instance != NULL);
}

#if !USE_SETUPAPI_DLL

int FindAllCards()
{
  	int i = -1;

    struct winio_pci_common_header buf = {0, };
  if (!module_winio_init())
    return -1;

  while (winio_get_pci_header(winio_instance, &buf)) 
  {

    /* Check if the card contains an Nvidia chipset */	
    if(buf.VendorID == 0x10de)
    {
      /*
      When we enter this block of code we know that the device contains some 
      chip designed by Nvidia. In the past Nvidia only produced videochips but
      now they also make various other devices. Because of this we need to find
      out if the device is a videocard or not. There are two ways to do this. We can
      create a list of all Nvidia videochips or we can check the pci header of the device.
      We will read the pci header from /proc/bus/pci/(bus)/(function).(device). When
      the card is in our card database we report the name of the card and else we say
      it is an unknown card.
      */

      if(!IsVideoCard(&buf))
        continue;

      i++;
      card[i].device_id = buf.DeviceID;
      card[i].number = i;
      card[i].card_name = (char*)get_card_name(card[i].device_id, &card[i].gpu);
      card[i].bus_id = (char*)malloc(8 * sizeof(char));
      sprintf(card[i].bus_id, "%02d:%02d.%d", buf.Bus, PCI_HDR_GET_DEV(&buf), PCI_HDR_GET_FN(&buf));
      card[i].reg_address = buf.BaseAddresses[0];
    }
  }
  /* new function that writes/reads the defaults speeds from a file */
  set_default_speeds(i);

  return i;
}
#endif /* not USE_SETUPAPI_DLL */


long pciReadLong(short dev, short bus, short function, long offset)
{
#ifdef NOT_IMPLEMENTED
    char file[25];
    FILE *device;

    snprintf(file, sizeof(file), "/proc/bus/pci/%02x/%02x.%x", dev, bus, function);
    if((device = fopen(file, "r")) != NULL)
    {
	long buffer;
	fseek(device, offset, SEEK_SET); 	    			    
	fread(&buffer, sizeof(long), 1, device);
	fclose(device);

	return buffer;
    }
#endif
    return -1;
}
