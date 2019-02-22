; ############################################################################
; $Id: getclkstep.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; setclk_getclock function
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
; $Log: getclkstep.asm,v $
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
                include r6reg.inc
                include r6probe.inc
                include setclk.inc

                .list
                .nocref

; ############################################################################
.code
                ; --------------------------------------
                ;  Name: setclk_getclock
                ;  Desc: Get RADEON clock settings
                ; --------------------------------------

asm_setclk_getclockstep proc c
                public  asm_setclk_getclockstep

                local   dwRead: dword

                invoke  r6probe_readpll, RADEON_X_MPLL_REF_FB_DIV

                ; Calculate clock step
                ; = basefreq / divisor
                fld     setclk_PLLData.reference_freq
                movzx   eax, al
                mov     dwRead, eax
                fild    dwRead
                fdivp   st(1), st

                ret
asm_setclk_getclockstep endp

; ############################################################################

                endfile
