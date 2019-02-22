; ############################################################################
; $Id: DRIVER.INC,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; Definitions and declarations for driver
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
; $Log: DRIVER.INC,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################

R6PROBE_VERSION equ 000010002h

CARD            STRUCT

card_bus        dd  ?
card_dev        dd  ?
card_fbPhy      dd  ?
card_fb         dd  ?
card_fbsize     dd  ?
card_mmrPhy     dd  ?
card_mmr        dd  ?
card_bios       dd  ?

CARD            ENDS
PCARD           TYPEDEF ptr CARD

;
; Functions
;
externdef       IoControl: proc
