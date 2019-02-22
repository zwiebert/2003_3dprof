; ############################################################################
; $Id: main.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; Driver entry point and management routines.
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
; $Log: main.asm,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################
                .586
                option proc: private

; ############################################################################
                ; --------------------------------------
                ;  Includes
                ; --------------------------------------
                .xlist
                include vmm.inc
                include vxdldr.inc
                include vwin32.inc
                include pci.inc
                .list
                option casemap: none

                include r6.inc
                include r6reg.inc
                include r6probe_ioc.inc
                include driver.inc
; ############################################################################

                ; --------------------------------------
                ;  Virtual device declaration
                ; --------------------------------------

Declare_Virtual_Device  R6PROBE, 1, 0,                \
                        Control, UNDEFINED_DEVICE_ID, \
                        UNDEFINED_INIT_ORDER

; ############################################################################
VxD_LOCKED_CODE_SEG

                ; --------------------------------------
                ;  Name: findR6
                ;  Desc: Search for a device on PCI bus
                ;  Out:  ZF = 1 when found
                ;        EAX - bus #
                ;        EDX - device & function
                ; --------------------------------------

findR6          PROC

                local   bus: DWORD
                local   dev: DWORD

                xor     ecx, ecx
                push    edi
@nextbus:
                xor     eax, eax
                mov     bus, ecx
@nextdev:
                mov     dev, eax

                push    0
                push    eax
                push    bus
                VxDCall _PCI_Read_Config
                add     esp, 3*4
                cmp     ax, PCI_VENDOR_ID_ATI
                jnz     @notfound

                shr     eax, 16  ; shift device id to ax

                push    ecx
                mov     edi, offset devids
                mov     ecx, devids_len
                repne   scasw
                pop     ecx
                jz      @found

                push    ecx
                mov     edi, offset pciIDsX
                mov     ecx, lengthof pciIDsX
                repne   scasw
                pop     ecx
                jz      @found
        
@notfound:
                mov     eax, dev
                inc     eax
                cmp     eax, 64 * 4
                jb      @nextdev

                mov     ecx, bus
                inc     ecx
                cmp     ecx, 8
                jb      @nextbus
                or      ecx, ecx
                jmp     @end
@found:
                mov     eax, bus
                mov     edx, dev
@end:
                pop     edi
                ret
findR6          ENDP

; ############################################################################

                ; --------------------------------------
                ;  Name: InitDevice
                ;  Desc: Initialize device
                ;  In  : EDX - hDevice
                ;        ESI - lpDIOCParams
                ; --------------------------------------

DeviceInit:
                call    findR6
                jz      @f

                stc
                jmp     @endInit
@@:
                mov     card.card_bus, eax
                mov     card.card_dev, edx

                push    RADEON_REG_BASE
                push    edx
                push    eax
                VxDCall _PCI_Read_Config
                add     esp, 3*4
                and     al, 0f0h
                mov     card.card_mmrphy, eax

                VMMCall _MapPhysToLinear, <eax, R6_MMR_SIZE, 0>
                mov     card.card_mmr, eax

                mov     edx, dword ptr [eax + RADEON_BIOS_1_SCRATCH]
                movzx   edx, dx
                shl     edx, 4
                mov     card.card_bios, edx

                mov     edx, dword ptr [eax + RADEON_CONFIG_MEMSIZE]
                mov     card.card_fbsize, edx
                mov     eax, dword ptr [eax + RADEON_MEM_BASE]
                and     al, 0f0h
                VMMCall _MapPhysToLinear, <eax, edx, 0>
                mov     card.card_fb, eax

                clc
@endInit:
                ret

; ############################################################################

                ; --------------------------------------
                ;  Name: DeviceExit
                ;  Desc: Called when VXD is unloading
                ; --------------------------------------
DeviceExit:
                clc
                ret

; ############################################################################

                ; --------------------------------------
                ;  Name: Control
                ;  Desc: VxD control dispatcher
                ; --------------------------------------
Control:
                cmp eax, W32_DEVICEIOCONTROL
                je  IoControl

                cmp eax, SYS_DYNAMIC_DEVICE_INIT
                je  DeviceInit

                cmp eax, SYS_DYNAMIC_DEVICE_EXIT
                je  DeviceExit

                clc
                ret

VxD_LOCKED_CODE_ENDS
; ############################################################################
VxD_LOCKED_DATA_SEG

card            CARD<0>

                include ..\chiplist.inc

VxD_LOCKED_DATA_ENDS
; ############################################################################

END