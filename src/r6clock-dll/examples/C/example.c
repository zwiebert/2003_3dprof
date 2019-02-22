#include "windows.h"
#include "../../r6clock-dll.h"                         // r6clock-dll library header to include
//#pragma comment (lib, "../../Debug/r6clock-dll.lib")   // r6clock-dll library object to link

#include <stdio.h>


void debug_print_clocks(const char *msg, struct r6clock_clocks clocks)
{
  fprintf(stderr, "%sCore = %lu kHz, RAM = %lu kHz\n", msg, clocks.core_khz, clocks.ram_khz);
}


int main(void)
{
  struct r6clock_clocks clocks = {0, 0};
  unsigned short device_id, vendor_id;
  unsigned long pci_id;

  /* get current clocks */
  clocks = r6clock_get_clock();
  debug_print_clocks("r6clock_get_clock() => ", clocks);


  /* set clocks to clocks.core_khz and clocks.ram_khz */
  clocks = r6clock_set_clock(clocks.core_khz, clocks.ram_khz);
  debug_print_clocks("r6clock_set_clock(n, m) => ", clocks);



  /* set core clock only */
  clocks = r6clock_set_clock(clocks.core_khz, 0);
  debug_print_clocks("r6clock_set_clock(n, 0) => ", clocks);



  /* set clocks to clocks.core_khz and clocks.ram_khz */
  if (r6clock_set_get_clock(&clocks.core_khz, &clocks.ram_khz) == -1) {
    /* ...error handling... */
  }
  debug_print_clocks("r6clock_set_get_clock(&p, &q) => ", clocks);



  /* get default clock */
  clocks = r6clock_get_default_clock();
  debug_print_clocks("r6clock_get_default_clock() => ", clocks);



  /* get PCI ID */
  pci_id = r6clock_get_id();
  vendor_id = R6CLOCK_VEN_ID(pci_id);
  device_id = R6CLOCK_DEV_ID(pci_id);
  fprintf(stderr, "r6clock_get_id() => vendor_id = %04x, device_id = %04x\n", (int)vendor_id, (int)device_id);

  fprintf(stderr, "\n*** Press Return To Continue! ***\n"), getchar();
  return 0;
}