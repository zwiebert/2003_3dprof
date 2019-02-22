
/* Example:
...
FindCallCards(); // init
set_card(0);     // set current card to the first one
...now use other functions from this header...
*/

/* init / card select */
int  nvc_init(void);            /* init and returns number of found cards */
void nvc_set_card(int number);  /* choose current card. */
void nvc_leave(void);           /* free all used resources */

/* Memory info */
char *   nvc_get_memory_type(void);  /* Memory type: SDR/DDR */
unsigned nvc_get_memory_width(void); /* Memory width 64bit or 128bit */
unsigned nvc_get_memory_size(void);  /* Amount of memory between 4 and 128 MB */

/* AGP info */
char *   nvc_get_bus_type(void);            /* Bus type: AGP/PCI */
unsigned nvc_get_agp_rate(void);            /* Current AGP rate: 1, 2, 4  or 8*/
char *   nvc_get_agp_status(void);          /* Current AGP status: Enabled/Disabled */
char *   nvc_get_fw_status(void);           /* Current FW status: Enabled/Disabled */
char *   nvc_get_sba_status(void);          /* Current SBA status: Enabled/Disabled */
char *   nvc_get_supported_agp_rates(void); /* Supported AGP rates */

/* Overclocking */
float nvc_get_gpu_speed(void);
void  nvc_set_gpu_speed(unsigned nvclk);
float nvc_get_memory_speed(void);
void  nvc_set_memory_speed(unsigned memclk);
void  nvc_reset_speeds(void);


/* Register Space */
unsigned *nvc_get_regspace_extdev();
unsigned *nvc_get_regspace_fb();
unsigned *nvc_get_regspace_mc();
unsigned *nvc_get_regspace_ramdac();


