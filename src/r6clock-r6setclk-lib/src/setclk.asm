; ############################################################################
; $Id: setclk.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; setclk_setclock function
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
; $Log: setclk.asm,v $
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
                ;  Name: r6probe_setclk
                ;  Desc: Set RADEON clock freqs
                ; --------------------------------------

asm_setclk_setclock proc c  coreclk: real4, memclk: real4, locked: dword
                public  asm_setclk_setclock

                local   dwRead: dword
                local   cw:  word
                local   cw2: word

                invoke  r6probe_readpll, RADEON_X_MPLL_REF_FB_DIV

                ; Calculate clock step
                ; = basefreq / divisor
                fld     setclk_PLLData.reference_freq
                movzx   eax, al
                mov     dwRead, eax
                fild    dwRead
                fdivrp  st(1), st           ; st(0) = divisor / basefreq

                mov     eax, locked
                test    eax, eax
                jz      @notlocked          ; not locked, jump fwd
                mov     eax, coreclk
                mov     memclk, eax
                mov     al, 7
                jmp     @set
@notlocked:
                mov     al, 2
@set:
                ;
                ; Prepare FPU
                ;
                fstcw   cw
                mov     cx, cw
                and     ch, 0f3h         ; mask rounding bits
                or      ch, 004h         ; set round to lower
                mov     cw2, cx

                ; Calculate multiplier
                fld     coreclk
                fmul    st, st(1)
                fldcw   cw2              ; set round to lower
                fistp   coreclk
                fldcw   cw               ; restore rounding
                fld     memclk
                fmulp   st(1),st
                mov     ch, byte ptr coreclk
                fldcw   cw2              ; set round to lower
                fistp   memclk
                fldcw   cw               ; restore rounding
                mov     cl, byte ptr memclk
                shl     ecx, 8
                push    eax
                invoke  r6probe_writepll, RADEON_X_MPLL_REF_FB_DIV, 0ffff00h, ecx
                pop     eax
                invoke  r6probe_writepll, RADEON_XCLK_CNTL, 07h, eax
                ret

asm_setclk_setclock endp

; ############################################################################

                endfile
