#ifdef R6CLOCKDLL_EXPORTS
#define R6CLOCKDLL_API __declspec(dllexport)
#else
#define R6CLOCKDLL_API __declspec(dllimport)
#endif

struct r6clock_clocks {
  ULONG core_khz;
  ULONG ram_khz;
};

#ifdef __cplusplus
extern "C" {
#endif


  /*
   NAME
     r6clock_get_clock - Get current clocks

   SYNOPSIS
     struct r6clock_clocks r6clock_get_clock(void)

   RESULT
     Current clocks in kilohertz.
  */


  /*
   NAME
     r6clock_set_clock - Set and get current clocks

   SYNOPSIS
     struct r6clock_clocks   r6clock_set_clock(ULONG core_khz, ULONG ram_khz)

   PARAMETERS
     [IN] core_khz (kilohertz) - Desired VPU clock to be set. If 0, current VPU clock will be used. 
     [IN] ram_khz  (kilohertz) - Desired RAM clock to be set. If 0, current RAM clock will be used.

     If both parameters are 0, then it just returs the current clocks like r6clock_get_clock() does.

   RESULT
    Current clocks after setting is done. These clocks may differ from the requested clocks because of PLL limitations.
  */

  /*
    NAME
      r6clock_set_get_clock - Set and get current clocks (alternative version)

    SYNOPSIS
      LONG  r6clock_set_get_clock(ULONG *core_khz, ULONG *ram_khz)

    PARAMETERS
      [IN,OUT] core_khz (kilohertz) - Pointer to desired VPU clock to be set. If clock is 0, current VPU clock will be used.
                                      At return, *core_khz contains the current clock after setting is done.
      [IN,OUT] ram_khz  (kilohertz) - Pointer to desired RAM clock to be set. If clock is 0, current RAM clock will be used.
                                      At return, *core_khz contains the current clock after setting is done.
    RESULT
      0 for success; -1 for failure      
  */

  /*
  NAME
    r6clock_get_default_clock - Get default clocks defined in VGA-BIOS if possible

  SYNOPSIS
    struct r6clock_clocks r6clock_get_default_clock(void);

  RESULT
    Default clock of the current card

  BUGS
    Currently not fully implemented. It always returns {0,0} 
  */

  /*
  NAME
    r6clock_get_id - Get device and vendor ID from PCI header

  SYNOPSIS
    ULONG  r6clock_get_id(void)

  RESULT
    32bit value which contains both IDs. Use macros R6CLOCK_VEN_ID and R6CLOCK_DEV_ID to extract.
  */


/* set */
R6CLOCKDLL_API struct r6clock_clocks    r6clock_set_clock(ULONG core_khz, ULONG ram_khz);
R6CLOCKDLL_API LONG                     r6clock_set_get_clock(ULONG *core_khz, ULONG *ram_khz);

/* get */
R6CLOCKDLL_API struct r6clock_clocks    r6clock_get_clock(void);
R6CLOCKDLL_API struct r6clock_clocks    r6clock_get_default_clock(void);
R6CLOCKDLL_API ULONG                    r6clock_get_id(void);
#define R6CLOCK_VEN_ID(id) ((USHORT)((id) & 0xffff))
#define R6CLOCK_DEV_ID(id) ((USHORT)((id) >> 16))

/* diag */
R6CLOCKDLL_API LONG                     r6clockdiag_get_info(char *buf, ULONG buflen);

#ifdef __cplusplus
}
#endif
