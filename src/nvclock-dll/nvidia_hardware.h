#ifdef NVCLOCKDLL_EXPORTS
#define NVC_API __declspec(dllexport)
#else
#define NVC_API __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* Example:
...
nvc_init(); // init
nvc_set_card(0);     // set current card to the first one
...now use other functions from this header...
*/

/* init / card select */
NVC_API int  nvc_init(void);            /* init and returns number of found cards */
NVC_API void nvc_set_card(int number);  /* choose current card. */
NVC_API void nvc_leave(void);           /* free all used resources */

/* Memory info */
NVC_API char *   nvc_get_memory_type(void);  /* Memory type: SDR/DDR */
NVC_API unsigned nvc_get_memory_width(void); /* Memory width 64bit or 128bit */
NVC_API unsigned nvc_get_memory_size(void);  /* Amount of memory between 4 and 128 MB */

/* AGP info */
NVC_API char *   nvc_get_bus_type(void);            /* Bus type: AGP/PCI */
NVC_API unsigned nvc_get_agp_rate(void);            /* Current AGP rate: 1, 2, 4  or 8*/
NVC_API char *   nvc_get_agp_status(void);          /* Current AGP status: Enabled/Disabled */
NVC_API char *   nvc_get_fw_status(void);           /* Current FW status: Enabled/Disabled */
NVC_API char *   nvc_get_sba_status(void);          /* Current SBA status: Enabled/Disabled */
NVC_API char *   nvc_get_supported_agp_rates(void); /* Supported AGP rates */

/* Overclocking */
NVC_API float nvc_get_gpu_speed(void);
NVC_API void  nvc_set_gpu_speed(unsigned nvclk);
NVC_API float nvc_get_memory_speed(void);
NVC_API void  nvc_set_memory_speed(unsigned memclk);
NVC_API void  nvc_reset_speeds(void);


/* Register Space */
NVC_API unsigned *nvc_get_regspace_extdev(void);
NVC_API unsigned *nvc_get_regspace_fb(void);
NVC_API unsigned *nvc_get_regspace_mc(void);
NVC_API unsigned *nvc_get_regspace_ramdac(void);

#ifdef __cplusplus
}
#endif

