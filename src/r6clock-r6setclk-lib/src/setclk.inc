; ############################################################################
; $Id: setclk.inc,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; Include file for setclk library
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
; $Log: setclk.inc,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################

BIOSPLLINFO     STRUCT
                dw      04h dup (?)
xclk            dw      ?
                dw      02h dup (?)
reference_freq  dw  ?
reference_div   dw  ?
min_freq        dw  ?
                dw  ?
max_freq        dw  ?
BIOSPLLINFO     ENDS

; ############################################################################

PLLDATA         STRUCT
reference_freq  REAL4   ?
reference_div   dd      ?
min_freq        REAL4   ?
max_freq        REAL4   ?
xclk            REAL4   ?
PLLDATA         ENDS
PPLLDATA        TYPEDEF ptr PLLDATA

; ############################################################################

LATENCY_FAST    equ 07ffff1ffh
LATENCY_MEDIUM  equ 044334244h

MEMTIM_SLOW     equ 01405357fh
MEMTIM_MEDIUM   equ 01405356ah
MEMTIM_FAST     equ 01405256ah
MEMTIM_MASK     equ 00000f0ffh

; ############################################################################

externdef       setclk_PLLData: PLLDATA

asm_setclk_open     PROTO c
asm_setclk_close    PROTO c
asm_setclk_setclock PROTO c coreclk :real4, memclk :real4, locked: dword
asm_setclk_getclock PROTO c coreclk :dword, memclk :dword
