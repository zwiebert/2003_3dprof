; ############################################################################
; $Id: chiplist.inc,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; PCI ID's of RADEON
;
; ----------------------------------------------------------------------------
; LICENSE
;
; This program is free software; you can redistribute it and/or modify it
; under the terms of the GNU General Public License (GPL) as published by the
; Free Software Foundation; either version 2 of the License, or (at your
; option) any later version.
;
; This program is distributed in the hope that it will be useful, but WITHOUT
; ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
; FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
; more details.
;
; To read the license please visit http://www.gnu.org/copyleft/gpl.html
; ----------------------------------------------------------------------------
; $Log: chiplist.inc,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################


;; EXPERIMENTAL {
;; some magic numbers to help locating the PCI device ID tables in the driver binary in order to display or patch them
;; TODO: binary patching may require recalculation of a checksum in the binary
MAGIC_BEG EQU 04213h
MAGIC_MID EQU 01299h
MAGIC_END EQU 06963h
pciIDs_start dw MAGIC_BEG, MAGIC_BEG, MAGIC_BEG, MAGIC_BEG, MAGIC_BEG, MAGIC_BEG, MAGIC_BEG, MAGIC_BEG
;; } EXPERIMENTAL


;; #############################################################################
;; PCI device ID table to recognize supported cards in main.asm of vxd+sys
;;
;; Updating:
;;  - to add a new device ID: Append a new device ID to any of the tables
;;  - to add a new table: Make a new table and add its length to "devids_len" below.
;;
;; Notes:
;;  - The reason for having more than one table: MASM32 will complain about too complex statements and too long lines)
;;  - Order of IDs does not matter (no priority): For each found PCI device the entire table is looked up.

devids dw  03150h,    ; MOBILITY_X600
	       03E50h,    ; X600_SERIES
	       03E54h,    ; FireGL_V3200
	       04136h,    ; IGP_320M
	       04137h,    ; IGP_340
	       04144h,    ; R9500
	       04147h,    ; FireGL_Z1_AGP_Pro
	       04148h,    ; R9800_AIW
	       04150h,    ; R9600
	       04151h,    ; R9600_2
	       04152h,    ; R9600_3
	       04153h,    ; 9550
	       04154h,    ; FireGL_T2
	       
devids2 dw 04242h,    ; R200_BB
	       04336h,    ; IGP_320M_SERIES	       
           04337h,    ; IGP_340M_SERIES
           04966h,    ; 9000_AIW
	       04A48h,    ; X800_SERIES_2
	       04A4Ah,    ; X800_SE
	       04A4Ch,    ; X800_SERIES
           04A4Dh,    ; FireGL_X3_256
	       04A4Eh,    ; MOBILITY_9800
	       04C57h,    ; MOBILITY_7500
	       
devids3 dw 04C58h,    ; MOBILITY_FIRE_GL7800
	       04C59h,    ; RADEON_LY
	       04C5Ah,    ; RADEON_LZ
	       04C64h,    ; FGL9000_MOBILE
	       04E47h,    ; FireGL_X1_AGP_Pro
	       04E4Bh,    ; FireGL_X2_256_X2_256t
	       04E54h,    ; MOBILITY_FIRE_GL_T2	       
           04a49h,    ; RX800_PRO
	       04a4bh,    ; RX800
	       04a50h,    ; RX800_XT
	       
devids4 dw 04c66h,    ; R9000_MOBILE
	       04e44h,    ; R9700_PRO
	       04e45h,    ; R9500_PRO, R9700
	       04e46h,    ; R9600_XT
	       04e48h,    ; R9800_PRO
	       04e49h,    ; R9800
	       04e4ah,    ; R9800_XT
	       04e50h,    ; R9600_MOBILE
	       05144h,    ; 7200_SERIES
	       05145h,    ; RADEON_QE
	       05146h,    ; RADEON_QF
	       
devids5 dw 05147h,    ; RADEON_QG
	       05148h,    ; FireGL_8800
	       0514Ch,    ; 8500_SERIES
	       0514Eh,    ; R200_QN
	       0514Fh,    ; R200_QO
	       0514dh,    ; R9100
	       05157h,    ; 7500_SERIES
	       05159h,    ; 7000_SERIES_2
	       0515Ah,    ; 7000_SERIES
	       0516Ch,    ; R200_Ql
	       
devids6 dw 05460h,    ; MOBILITY_X300
	       05464h,    ; MIBILITY_M22_GL
	       05548h,    ; X880_SERIES
	       05549h,    ; X880_PRO
	       0554Bh,    ; X880_SE
	       05551h,    ; FireGL_V7200
	       05552h,    ; FireGL_V5100
	       05554h,    ; FireGL_V7100
	       05834h,    ; R9100_IGP
	       05960h,    ; 9250_SERIES
	       05961h,    ; R9200
	       05964h,    ; R9200_2
	       
devids7 dw 05B60h,    ; X300_SERIES
	       05B62h,    ; X600_SERIES_2
	       05B64h,    ; FireGL_V1100_V3100
	       05B65h,    ; FireGL_D1100
	       05C63h,    ; MOBILITY_9200
	       05D48h,    ; MOBILITY_M28
	       05D57h,    ; X880_XT
	       05c61h,    ; R9200_MOBILE
	       07834h,    ; 9000_9100_PRO_IGP_SERIES
	       0C599h,    ; MOBILITY



;; ### Add new tables above this line and add its length to devids_len below ###
;; #############################################################################


;; EXPERIMENTAL {
pciIDs_mid dw MAGIC_MID, MAGIC_MID, MAGIC_MID, MAGIC_MID, MAGIC_MID, MAGIC_MID, MAGIC_MID, MAGIC_MID
pciIDsX dw  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
pciIDs_end dw MAGIC_END, MAGIC_END, MAGIC_END, MAGIC_END, MAGIC_END, MAGIC_END, MAGIC_END, MAGIC_END
pciIDs_csdiff dw 0
;; } EXPERIMENTAL



;; ### Append "lenghof" for new tables here	###
devids_len dd lengthof devids + lengthof devids2 + lengthof devids3 + lengthof devids4 + lengthof devids5 + lengthof devids6 + lengthof devids7



           

