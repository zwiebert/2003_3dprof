; ############################################################################
; $Id: open.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; setclk_open function
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
; $Log: open.asm,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################
                ; --------------------------------------
                ;  Includes
                ; --------------------------------------
                .xlist
                include macros.inc
                include r6.inc
                include r6probe.inc
                include setclk.inc
                .list
                .nocref

; ############################################################################

                ; --------------------------------------
                ;  Publics
                ; --------------------------------------
                public asm_setclk_open

; ############################################################################
.data?
                ; --------------------------------------
                ;  Module globals
                ; --------------------------------------

ifndef DELPHI
setclk_PLLData  PLLDATA<?>
endif

; ############################################################################
.const
flt_0_01        real4   0.01

.code
                ; --------------------------------------
                ;  Name: r6probe_open
                ;  Desc: Opens connection to r6probe
                ; --------------------------------------

asm_setclk_open     proc    c
                public  asm_setclk_open

                local   bioshdr: BIOSPLLINFO
                local   tmp: dword

                invoke  r6probe_open
                test    eax, eax
                jz      @end

                invoke  r6probe_readbios, R6_BIOS_HEADER_LOC

                movzx   eax, ax
                add     eax, 030h                    ; offset to pll info ptr
                invoke  r6probe_readbios, eax

                movzx   eax, ax
                invoke  r6probe_readbiosblk, addr bioshdr, eax, sizeof bioshdr

                fld     flt_0_01                     ; reference_freq *= 0.01
                movzx   eax, bioshdr.reference_freq
                mov     tmp, eax
                fild    tmp
                fmul    st, st(1)
                fstp    setclk_PLLData.reference_freq

                movzx   eax, bioshdr.xclk            ; xclk *= 0.01
                mov     tmp, eax
                fild    tmp
                fmul    st, st(1)
                fstp    setclk_PLLData.xclk

                movzx   eax, bioshdr.min_freq        ; min_freq *= 0.01
                mov     tmp, eax
                fild    tmp
                fmul    st, st(1)
                fstp    setclk_PLLData.min_freq

                movzx   eax, bioshdr.max_freq        ; max_freq *= 0.01
                mov     tmp, eax
                fild    tmp
                fmulp   st(1), st
                fstp    setclk_PLLData.max_freq

                xor     eax, eax
                mov     ax, bioshdr.reference_div
                mov     setclk_PLLData.reference_div, eax
                mov     eax, 1
@end:
                ret
asm_setclk_open     endp

; ############################################################################

                endfile
