; ############################################################################
; $Id: r6.inc,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; Include file for Radeon specific stuff
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
; $Log: r6.inc,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################

PCI_VENDOR_ID_ATI   equ 01002h

R6_MMR_SIZE         equ 04000h

R6_BIOS_BASE        equ 0c0000h
R6_BIOS_SIZE        equ 008000h
R6_BIOS_HEADER_LOC  equ 0048h


R6_WaitForFifo      macro   mmrbase, tmpreg, slots
@@:
                    mov     tmpreg, byte ptr [mmrbase + RADEON_RBBM_STATUS]
                    and     tmpreg, RADEON_RBBM_FIFOCNT_MASK
                    cmp     tmpreg, slots
                    jb      @b

                    endm

RADEON_MEM_TIMINGS  equ     000000144h
