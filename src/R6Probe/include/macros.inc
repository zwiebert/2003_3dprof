; ############################################################################
; $Id: macros.inc,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; Commonly used macros for modules written in assembly.
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
; $Log: macros.inc,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################
                    .686
                    option language: stdcall
                    option nokeyword: <.code .data .data? .const>
                    option dotname
                    option casemap: none
                    option proc: private

;---------------------------------------------------------------
; Segment setup
;---------------------------------------------------------------
endseg              macro
                    IFNB @CurSeg
@CurSeg             ends
                    ENDIF
                    endm

endfile             macro
                    endseg
                    end
                    endm

startseg            macro   segname
                    endseg
segname             segment
                    endm

.code               macro
                    startseg    _TEXT
                    endm

.const              macro
                    startseg    CONST
                    endm

.data               macro
                    startseg    _DATA
                    endm
                                
.data?              macro
                    startseg    _BSS
                    endm

_DATA               segment dword public flat 'DATA'
_DATA               ends

_BSS                segment dword public flat 'BSS'
_BSS                ends

_TEXT               segment dword public flat 'CODE'
_TEXT               ends

CONST               segment dword public flat 'CONST'
CONST               ends

                    assume  cs: flat, ds: flat, es: flat, ss: flat