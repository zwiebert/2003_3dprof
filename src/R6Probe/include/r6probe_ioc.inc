; ############################################################################
; $Id: r6probe_ioc.inc,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; Include file for R6PROBE's IOCTLs
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
; $Log: r6probe_ioc.inc,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################

                    include nt\ctl.inc

R6_TYPE             EQU 40000

CTL_CODE IOCTL_R6_READ_PCI,         R6_TYPE, 0900h, METHOD_BUFFERED, FILE_READ_ACCESS
CTL_CODE IOCTL_R6_READ_MMR,         R6_TYPE, 0901h, METHOD_BUFFERED, FILE_READ_ACCESS
CTL_CODE IOCTL_R6_WRITE_MMR,        R6_TYPE, 0902h, METHOD_BUFFERED, FILE_READ_ACCESS
CTL_CODE IOCTL_R6_READ_PLL,         R6_TYPE, 0903h, METHOD_BUFFERED, FILE_READ_ACCESS
CTL_CODE IOCTL_R6_WRITE_PLL,        R6_TYPE, 0904h, METHOD_BUFFERED, FILE_READ_ACCESS
CTL_CODE IOCTL_R6_READ_BIOS,        R6_TYPE, 0905h, METHOD_BUFFERED, FILE_READ_ACCESS
CTL_CODE IOCTL_R6_GET_ADAPTER_INFO, R6_TYPE, 0906h, METHOD_BUFFERED, FILE_READ_ACCESS
CTL_CODE IOCTL_R6_GET_VERSION,      R6_TYPE, 0907h, METHOD_BUFFERED, FILE_READ_ACCESS

MASKED_WRITE_PARAMS     STRUCT
_addr                   dd  ?
_mask                   dd  ?
_data                   dd  ?
MASKED_WRITE_PARAMS     ENDS
PMASKED_WRITE_PARAMS    TYPEDEF ptr MASKED_WRITE_PARAMS