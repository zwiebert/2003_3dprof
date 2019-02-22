/* NVClock 0.7 - Linux overclocker for NVIDIA cards
* 
* Copyright(C) 2001-2003 Roderick Colenbrander
*
* site: http://NVClock.sourceforge.net
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

//#include <unistd.h>
#include <getopt.h>
#include <stdio.h>
#include <stdlib.h>
#include <stdarg.h>
#include <string.h>
#include "nvclock.h"

static struct option long_options[] = {
  {"memclk", 1, 0, 'm'},
  {"nvclk", 1, 0, 'n'},
  {"card", 1, 0, 'c'},
  {"force", 0, 0, 'f'},
  {"info", 0, 0, 'i'}, 
  {"reset", 0, 0, 'r'},
  {"speeds", 0, 0, 's'},
  {"debug", 0, 0, 'd'},
  {"help", 0, 0, 'h'},
  {"no-ddr-doubling", 0, 0, '1'}, 
  {0, 0, 0, 0}
};


int usage() 
{
  printf("Using NVClock you can overclock your Nvidia videocard under Linux and FreeBSD.\nUse this program at your own risk, because it can damage your system!\n\n");
  printf("Usage: ./NVClock [options]\n\n");
  printf("Overclock options:\n");
  printf("   -c  --card number\t\tNumber of the card to overclock\n");
  printf("   -1  --no-ddr-doubling\tUse real memory clock for in- and output\n");
  printf("   -m  --memclk speed\t\tMemory speed in MHz\n");
  printf("   -n  --nvclk speed\t\tCore speed in MHz\n\n");
  printf("   -r  --reset\t\t\tRestore the original speeds\n\n");
  printf("Other options:\n");	
  printf("   -d  --debug\t\t\tEnable/Disable debug info\n");
  printf("   -f  --force\t\t\tForce a speed, NVClock won't check min/max speeds\n");
  printf("   -h  --help\t\t\tShow this help info\n");
  printf("   -i  --info\t\t\tPrint detailed card info\n");
  printf("   -s  --speeds\t\t\tPrint current speeds in MHz\n\n");
  return 0;
}

static void print_info_line_memclk(int no_ddr_doubling, int variant) {
  float curr_memclk;

  curr_memclk = nv_card.get_memory_speed();

  if(no_ddr_doubling && (strcmp("DDR", nv_card.get_memory_type()) == 0)) {
    curr_memclk *= 0.5f;
  }
  switch (variant) {
    case 1: printf("Speed: \t\t%0.3f MHz\n", curr_memclk); break;
    case 0: default: printf("Memory speed: \t%0.3f MHz\n", curr_memclk); break;
  }
}

static void print_info_line_memclk_range(int no_ddr_doubling) {
  float curr_memclk, min_memclk, max_memclk;

  return; // this function does not work, because speed ranges in pci_ids seems to be obsolete

  curr_memclk = nv_card.get_memory_speed();
  min_memclk = nv_card.memclk_min;
  max_memclk = nv_card.memclk_max;

  if(no_ddr_doubling && (strcmp("DDR", nv_card.get_memory_type()) == 0)) {
    curr_memclk *= 0.5f;
    min_memclk *= 0.5f;
    max_memclk *= 0.5f;
  }
  printf("Memory clocks:\tmin=%0.3f max=%0.3f curr=%0.3f\n", min_memclk, max_memclk, curr_memclk);     
}

/*
This function should be in the "client"
We receive an error_type code describing the seriousness of the problem:
-Error: fatal error
-INFO: unimplemented stuff (like nv30 overclocking..)
-Warning: non-fatal error
*/
void error(error_type code, const char *format, ...)
{
  va_list arg;
  va_start(arg, format);

  switch(code)
  {
  case ERROR:
    fprintf(stderr, "Error: ");
    vfprintf(stderr, format, arg);	
    fprintf(stderr, "\n");	
    exit(EXIT_FAILURE);
    break;
  case INFO:
    fprintf(stderr, "Info: ");	
    vfprintf(stderr, format, arg);	
    fprintf(stderr, "\n");	
    break;
  case WARNING:
    vfprintf(stderr, format, arg);	
    fprintf(stderr, "\n");	
    break;
  }
  va_end(arg);
}

int main(int argc, char *argv[])
{
  int card_number, total_cards, opt;
  float memclk, nvclk;
  short card_opt, force_opt, info_opt, reset_opt, speeds_opt, no_ddr_doubling_opt;

  card_opt = 0;
  force_opt = 0;
  info_opt = 0;
  reset_opt = 0;
  speeds_opt = 0;
  no_ddr_doubling_opt = 0;

  card_number = 0;
  total_cards = -1;
  memclk = 0;
  nvclk = 0;

  printf("NVClock v0.7\n\n");

  /* If no options are given. */
  if (argc == 1) {	
    usage();
    return 0;
  }

  total_cards = FindAllCards();

  switch(total_cards)
  {
    /* If no cards found, exit. */
  case -1:
    printf("No nVidia card found in your system!\n");
    exit(1);
    break;
  case -2:
    printf("You don't have permissions to access the registers of your Nvidia card.\n");
    printf("Try to run NVClock as root to get access to /dev/mem or install the Nvidia 3d drivers to use /dev/nv* instead.\n");
    exit(1);
    break;
  default:
    break;
  }


  /* If no cards found, exit. */
  if(total_cards < 0)
  {
    fprintf(stderr, "No nVidia card found in your system!\n");
    return 0;
  }

  while ( ( opt = getopt_long (argc, argv, "m:M:n:c:1lfidrsSh", long_options, NULL)) != -1 )
  {
    switch (opt)
    {
    case 'c':
      card_number = strtol(optarg, (char **)NULL, 10)-1;
      /* If the user only the card number. */
      if(argc == 3)
      {
        fprintf(stderr, "Error: You only used the -c option\n");
        return 0;    
      }

      /* Check if the card number is valid. */
      if(((card_number+1) <= 0) || card_number > total_cards)
      {
        fprintf(stderr, "Error: You entered an invalid card number!\nUse the -s option to show all card numbers\n\n");
        return 0;
      }

      card_opt = 1;
      break;

    case 'd':
      /* If the user only entered the -d option */
      if(argc == 2)
      {
        fprintf(stderr, "Error: You only used the -d option\n");
        return 0;    
      }
      nv_card.debug = 1; 
      break;

    case 'f':
      /* If the user only entered the -f option */
      if(argc == 2)
      {
        fprintf(stderr, "Error: You only used the -f option\n");
        return 0;    
      }
      force_opt = 1; 
      break;

    case 'i':
      info_opt = 1;
      break;

    case 'm':
      memclk = (float)strtol(optarg, (char **)NULL, 10);

      if (memclk < 0)
      {
        fprintf(stderr, "Wrong value for memclk: %d\n", memclk);
        return 1;
      }
      break;

    case 'n':
      nvclk = (float)strtol(optarg, (char **)NULL, 10);

      if (nvclk < 0)
      {
        fprintf(stderr, "Wrong value of nvclk: %d\n", nvclk);
        return 1;
      }
      break;

    case 'r':
      reset_opt = 1;
      break;

    case 's':
      speeds_opt = 1;
      break;

    case '1':
      no_ddr_doubling_opt = 1;
      break;

    case 'h':
      usage();
      break;
    default:
      return 0;
    }
  }

  /* set the card object to the requested card */
  set_card(card_number);		

  /* Make NVClock work on unsupported cards and access higher speeds as requested by the user */
  if(force_opt == 1)
    card[card_number].gpu = NORMAL;


  /* Check if the card is supported, if not print a message. */
  if((nv_card.gpu == UNKNOWN) && (force_opt == 0))
  {
    fprintf(stderr, "It seems your card isn't officialy supported in NVClock yet.\n");
    fprintf(stderr, "The reason can be that your card is too new.\n");
    fprintf(stderr, "If you want to try it anyhow [DANGEROUS], use the option -f to force the setting(s).\n");
    fprintf(stderr, "NVClock will then assume your card is a 'normal', it might be dangerous on other cards.\n");
    fprintf(stderr, "Also please email the author the pci_id of the card for further investigation.\n[Get that value using the -i option].\n\n");
    return 0;
  }

  if (no_ddr_doubling_opt) {
    memclk = ((strcmp("DDR", nv_card.get_memory_type()) == 0) ? memclk * 2 : memclk);
  }

  /* Check if the user used the -s option, if so show the requested info. */
  /* Detect all cards */
  if(speeds_opt == 1)
  {
    int i;
    for(i=card_number; i<=total_cards; i++)
    {
      set_card(i);

      printf("Card: \t\t%s\n", nv_card.card_name);
      printf("Card number: \t%d\n", i+1);

      print_info_line_memclk(no_ddr_doubling_opt, 0);
      print_info_line_memclk_range(no_ddr_doubling_opt);
      printf("Core speed: \t%0.3f MHz\n\n", nv_card.get_gpu_speed()); 

      /* Detect only the requested card */
      if(card_opt == 1)
        break;
    }
    return 0;
  }


  /* Get the card information for the currently selected card */
  if(info_opt == 1)
  {
    printf("-- General info --\n");
    printf("Card: \t\t%s\n", nv_card.card_name);
    printf("PCI id: \t0x%x\n", nv_card.device_id ); 
    printf("GPU speed: \t%0.3f MHz\n", nv_card.get_gpu_speed()); 
    printf("Bustype: \t%s\n\n", nv_card.get_bus_type());

    printf("-- Memory info --\n");
    printf("Amount: \t%d MB\n", nv_card.get_memory_size());
    printf("Type: \t\t%d bit %s\n", nv_card.get_memory_width(), nv_card.get_memory_type());
    print_info_line_memclk(no_ddr_doubling_opt, 1);

    if(strstr(nv_card.get_agp_status(), "Enabled") != 0)
    {
      printf("-- AGP info --\n");
      printf("Status: \t%s\n", nv_card.get_agp_status());
      printf("Rate: \t\t%dX\n", nv_card.get_agp_rate());
      printf("AGP rates: \t%s\n", nv_card.get_supported_agp_rates());
      printf("Fast Writes: \t%s\n", nv_card.get_fw_status());
      printf("SBA: \t\t%s\n\n", nv_card.get_sba_status());
    }

    return 0;
  }

  if(reset_opt)
  {
    nv_card.reset_speeds();
    printf("Your %s has been restored to its original speeds\n", nv_card.card_name);
    print_info_line_memclk(no_ddr_doubling_opt, 0);
    print_info_line_memclk_range(no_ddr_doubling_opt);
    printf("Core speed: \t%0.3f MHz\n\n", nv_card.get_gpu_speed()); 
    return 0;
  }

  /* Check if the gpu speed is higher than NVClock's max speed (+25%), if not print a message. */
  if( (nvclk >= nv_card.nvclk_max) && force_opt == 0)
  {
    fprintf(stderr, "Warning!\n");
    fprintf(stderr, "You entered a core speed of %.3f MHz and NVClock believes %d.000 MHz is the maximum!\n", nvclk, nv_card.nvclk_max);
    fprintf(stderr, "This error appears when the entered speed is 25%% higher than the default speed.\n");
    fprintf(stderr, "If you really want to use this speed, use the option -f to force it.\n\n");
    return 0;
  }

  /* Check if the memory speed is higher than NVClock's max speed (+25%), if not print a message. */
  if( (memclk >= nv_card.memclk_max || nvclk >= nv_card.nvclk_max) && force_opt == 0)
  {
    fprintf(stderr, "Warning!\n");
    fprintf(stderr, "You entered a memory speed of %.3f MHz and NVClock believes %d.000 MHz is the maximum!\n", memclk, nv_card.memclk_max);
    fprintf(stderr, "This error appears when the entered speed is 25%% higher than the default speed.\n");
    fprintf(stderr, "If you really want to use this speed, use the option -f to force it.\n\n");
    return 0;
  }

  if( memclk != 0)
  {
    printf("Requested memory speed: \t%0.3f MHz\n", memclk);
    nv_card.set_memory_speed((unsigned)memclk);
  }

  if (nvclk != 0)
  {
    printf("Requested core speed: \t\t%0.3f MHz\n", nvclk);    
    nv_card.set_gpu_speed((unsigned)nvclk);
  }

  printf("%s\n", nv_card.card_name);
  print_info_line_memclk(no_ddr_doubling_opt, 0);
  print_info_line_memclk_range(no_ddr_doubling_opt);

  printf("Core speed: \t%0.3f MHz\n\n", nv_card.get_gpu_speed()); 

  return 0;
}
