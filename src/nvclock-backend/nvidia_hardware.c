#include "nvidia_hardware.h"

#include "nvclock.h"

/* init / card select */
int  nvc_init(void) { int n = FindAllCards(); return (n < 0) ? 0 : n+1; }
void nvc_set_card(int number) { set_card(number); }
void nvc_leave(void) { /* TODO */ } 


/* Memory info */
char *nvc_get_memory_type(void) { return nv_card.get_memory_type(); }
unsigned  nvc_get_memory_width(void) { return (unsigned)nv_card.get_memory_width(); }
unsigned  nvc_get_memory_size(void) { return (unsigned)nv_card.get_memory_size(); }

/* AGP info */
char *nvc_get_bus_type(void) { return nv_card.get_bus_type(); }
unsigned   nvc_get_agp_rate(void) { return (unsigned)nv_card.get_agp_rate(); }
char *nvc_get_agp_status(void) { return nv_card.get_agp_status(); }
char *nvc_get_fw_status(void) { return nv_card.get_fw_status(); }
char *nvc_get_sba_status(void) { return nv_card.get_sba_status(); }
char *nvc_get_supported_agp_rates(void) { return nv_card.get_supported_agp_rates(); }

/* Overclocking */
float nvc_get_gpu_speed(void) { return nv_card.get_gpu_speed(); }
void  nvc_set_gpu_speed(unsigned nvclk) { nv_card.set_gpu_speed(nvclk); }
float nvc_get_memory_speed(void) { return nv_card.get_memory_speed(); }
void  nvc_set_memory_speed(unsigned memclk) { nv_card.set_memory_speed(memclk); }
void  nvc_reset_speeds(void) { nv_card.reset_speeds(); }

/* Register Space */
unsigned *nvc_get_regspace_extdev() { return (unsigned *)nv_card.PEXTDEV; }
unsigned *nvc_get_regspace_fb() { return (unsigned *)nv_card.PFB; }
unsigned *nvc_get_regspace_mc() { return (unsigned *)nv_card.PMC; }
unsigned *nvc_get_regspace_ramdac() { return (unsigned *)nv_card.PRAMDAC; }
