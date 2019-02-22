Naming:


VendorID[-DeviceID]-WinVer.cfg

 VendorID: 4-digit hex number (e.g. 1002) or a name (e.g. ati)
 DeviceID: 4-digit hex number (e.g. 4e44) or a chip type (e.g. r300)
 WinVer: win4 or win5

 Examples: 1002-4e44-win5.cfg, nvidia-nv30-win5.cfg, nvidia-win4.cfg

Note: Vendor names have precedence over VendorIDs. DeviceIDs have
precedence over ChipTypes.

--------------------------------------------------------------------------------
Syntax:

 if(condition)  -OR- if.LABEL(condition)
 elif(condition) -OR- elif.LABEL(condition)
 else -OR- else.LABEL
 endif -OR- endif.LABEL

   Conditional scanning statements. Used to comment out unused
   blocks/items according to driver version.

   Valid conditions are:

      condition && condition
      condition || condition
      true
      false
      Win32NT
      driver_version COMPARE_OPERATOR version_string
      driver_short_version COMPARE_OPERATOR version_number

      Short alias names exist for the following left hand side parameter:
        driver_version => dv   (the driver dll version string e.g. 6.14.10.4520) 
        driver_short_version => dsv (number of version string behind last ".", e.g. 4520)

      COMPARE_OPERATOR can be one of the following:
       equal: = -OR- ==
       not equal: <> -OR- !=
       greater than: >
       lower than: <
       equal or greater than: >=
       lower or equal than: <=

      && means the boolean AND operator (AND has precedence over OR)
      || means the boolean OR operator (OR has lower priority than AND)
      
        

   Labels can be used to enhance error messages in case of mismatched
   statements.


 include="FILE"

   Note: FILE is relative to TDProf.exe, so it has to be prefixed with
   "specfiles/"

 [SOME_LABEL]

   Starts a new list of items. All following lines are interpreted as items.


 name="...", rv("REGVALUE"=REGDATA)
 name="...", rv("REGVALUE":=REGDATA)   ;; := operator makes the value WriteOnly

  Defines a new item. The name will appear in combo boxes and saved
  profiles. The registry stuff determines both how to enable an item and
  how to recognize if its already enabled. WriteOnly values are just
  used to enable but not to recognice a mode.

  If an items has to use more than one registry value, one can append
  up to 7 additional regvalue/regdata pairs.
  
  REGDATA := STRING_DATA | DWORD_DATA | BINARY_DATA | undef
  STRING_DATA := "xxxxxx"
  DWORD_DATA := dword:HEXNUMBER | dword:HEXNUMBER_DATA#HEXNUMBER_MASK
  BINARY_DATA := hex:HEX_BYTE_LIST | hex:HEX_BYTE_LIST_DATA#HEX_BYTE_LIST_MASK
  
  A REGVALUE can be prefixed by registry keys. Registry keys can be absolute or
  relative. Relative keys use the driver key as their base key.
  Absolute keys have to start with a backslash followed by one of the
  following hive abbrevations: HKLM, HKU, HKCU, HKCC, HKCR.
  Expamples:
   absolute key: rv("\HKLM\Software\test_key\test_key2\test_value"=dword:1)
   relative key: rv("OpenGL\FSAA"=dword:1)  ;; relative to driver-key
   no key at all:  rv("FSAA"=dword:1)       ;; value is located in driver-key
   
  A mask is a bitmask to configure which bits of the data is used in
  comparing and setting. If an regvalue exists in registry,
  only bits set in mask are changed. If no regvalue exist the mask 
  will be ignored and the dataword is set as whole.

 alias="..."
  define an alias which can be used in profiles instead of name


 flags=(comma_separated_flag_name_list)
    hidden - does not show the name in GUI controls (Purpose: some
    kind of 'boolean or')
 parent="item_name"
   If the item matches, the parent will be reported as match to the
   user interface. (Purpose: some kind of 'boolean or')
   

 id="..."

  This is often used with labels like OGL_Extra_1. It defines a
  proper name which will show up in saved profiles.


 gui_label="...", gui_tooltip="...", gui_width_mult=N

  Defines a label for combo boxes and a help tooltip. gui_width_mult
  can be used to make an extra slot appear wider in the GUI. It is
  required to leave some slots unused to make room. e.g. if N=2, One
  slot on the right hand side of this slot cannot be used. If N=3, two
  slots on the right hand side must be left alone, and so on.

 Syntax examples:

 Using if/elif/else/endif with labels to enhance parse error messages:

 if.abc(driver_short_version < 3380)
   ....
 elif.abc(driver_short_version <= 4403)
   ...
 elif.abc(driver_version < 6.14.10.4520)
   ...
 else.abc
   ...
 endif.abc

Operator && has precedence over ||:

if(true && false && true || false && true && true || true && true)

Here the same statement written in a different way to show the precedence: 
if( true&&false&&true   ||   false&&true&&true   ||   true&&true )
  


--------------------------------------------------------------------------------
