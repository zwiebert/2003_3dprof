; ############################################################################
; $Id: IOCTL.ASM,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; IOControl routines
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
; $Log: IOCTL.ASM,v $
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
                include hal.inc
                include ntoskrnl.inc
                include ddk.inc
                include r6.inc
                include r6reg.inc
                include r6probe_ioc.inc
                include driver.inc
                .list
                .nocref

; ############################################################################

                ; --------------------------------------
                ;  Publics
                ; --------------------------------------
                public IoControl

; ############################################################################

                ; --------------------------------------
                ;  Constants
                ; --------------------------------------

R6CTL_FUNC_MASK EQU     03fch       ; normal ctl_function_mask - 8 upper bits
DIOCParams      TEXTEQU <Parameters.DeviceIoControl>

; ############################################################################
.data
                ; --------------------------------------
                ;  Module globals
                ; --------------------------------------

; ############################################################################
.code
                ; --------------------------------------
                ;  Name: readPCI
                ;  Desc: read from PCI configuration
                ; --------------------------------------
readPCI:
                assume  esi: PCARD
                assume  edx: PIO_STACK_LOCATION
                assume  ebx: PNIRP
                assume  edi: ptr dword

                xor     eax, eax
                mov     al, 4               ; size dword
                xchg    edi, eax
                ; check parameters
                cmp     [edx].DIOCParams.InputBufferLength, edi
                jne     paramerror
                cmp     [edx].DIOCParams.OutputBufferLength, edi
                jne     paramerror

                push    edi                 ; size dword
                push    [eax]               ; offset
                push    eax                 ; buffer
                mov     eax, [esi].card_dev
                push    eax                 ; slotnr
                mov     eax, [esi].card_bus
                push    eax                 ; bus
                push    edi                 ; PCIConfiguration = 4
                call    HalGetBusDataByOffset

                mov     [ebx].IoStatus.Information, edi
                xor     eax, eax
                ret

; ############################################################################
                
                ; --------------------------------------
                ;  Name: readMMR
                ;  Desc: read from memory-mapped reg
                ; --------------------------------------
readMMR:
                assume  esi: PCARD
                assume  edx: PIO_STACK_LOCATION
                assume  ebx: PNIRP
                assume  edi: ptr dword

                ; check parameters
                cmp     [edx].DIOCParams.InputBufferLength, size dword
                jne     paramerror
                cmp     [edx].DIOCParams.OutputBufferLength, size dword
                jne     paramerror

                mov     eax, [esi].card_mmr
                mov     ecx, [edi]
                and     ecx, R6_MMR_SIZE - 1
                mov     eax, [eax + ecx]
                mov     [edi], eax
                mov     [ebx].IoStatus.Information, size dword
                xor     eax, eax
                ret

; ############################################################################
                
                ; --------------------------------------
                ;  Name: writeMMR
                ;  Desc: write to memory-mapped reg
                ; --------------------------------------
writeMMR:
                assume  esi: PCARD
                assume  edx: PIO_STACK_LOCATION
                assume  ebx: PNIRP
                assume  edi: PMASKED_WRITE_PARAMS

                ; check parameters
                cmp     [edx].DIOCParams.InputBufferLength, size MASKED_WRITE_PARAMS
                jne     paramerror
                cmp     [edx].DIOCParams.OutputBufferLength, 0
                jne     paramerror

                mov     eax, [esi].card_mmr     ; get mmr base
                mov     ecx, [edi]._addr        ; get addr to write to
                and     ecx, R6_MMR_SIZE - 1    ; make sure the address is valid
                lea     eax, [eax + ecx]        ; make final ptr
                mov     edx, [edi]._mask        ; load mask
                mov     esi, [edi]._data        ; load dest data
                mov     ecx, [eax]              ; read register
                and     esi, edx                ; and user data with mask
                not     edx                     ; invert mask
                and     ecx, edx                ; and register data with mask
                or      ecx, esi                ; or both together
                mov     [eax], ecx              ; and write back to card
                xor     eax, eax
                mov     [ebx].IoStatus.Information, eax
                ret

; ############################################################################
paramerror:
                xor     eax, eax
                mov     [ebx].IoStatus.Information, eax
                mov     eax, STATUS_INVALID_PARAMETER
                ret

; ############################################################################
                
                ; --------------------------------------
                ;  Name: readPLL
                ;  Desc: read from PLL register
                ; --------------------------------------
readPLL:
                assume  esi: PCARD
                assume  edx: PIO_STACK_LOCATION
                assume  ebx: PNIRP
                assume  edi: ptr dword

                ; check parameters
                cmp     [edx].DIOCParams.InputBufferLength, size dword
                jne     paramerror
                cmp     [edx].DIOCParams.OutputBufferLength, size dword
                jne     paramerror

                mov     eax, [esi].card_mmr
                lea     eax, [eax + RADEON_CLOCK_CNTL_INDEX]
                mov     ecx, dword ptr [edi]
                and     cl, 1fh
@@:
                mov     [eax], cl
                mov     ch, [eax]
                and     ch, 9fh
                cmp     cl, ch
                jne     @b

                mov     eax, [eax + 4]  ; get data from RADEON_CLOCK_CNTL_DATA
                mov     [edi], eax      ; write to user buffer
                mov     [ebx].IoStatus.Information, size dword
                xor     eax, eax
                ret

; ############################################################################
                
                ; --------------------------------------
                ;  Name: writePLL
                ;  Desc: write to PLL register
                ; --------------------------------------
selectPllReg:
                mov     ch, [eax]
                and     ch, 0bfh
                cmp     cl, ch
                je      @f
                mov     [eax], cl
                jmp     selectPllReg
@@:
                ret

writePLL:
                assume  esi: PCARD
                assume  edx: PIO_STACK_LOCATION
                assume  ebx: PNIRP
                assume  edi: PMASKED_WRITE_PARAMS

                ; check parameters
                cmp     [edx].DIOCParams.InputBufferLength, size MASKED_WRITE_PARAMS
                jne     paramerror
                cmp     [edx].DIOCParams.OutputBufferLength, 0
                jne     paramerror

                mov     eax, [esi].card_mmr
                lea     eax, [eax + RADEON_CLOCK_CNTL_INDEX]
                mov     ecx, [edi]._addr
                mov     edx, [edi]._mask    ; load mask
                mov     esi, [edi]._data    ; load data
                and     esi, edx            ; and user data with mask
                not     edx                 ; invert mask
                and     cl, 3fh
                or      cl, RADEON_PLL_WR_EN
   
                call    selectPllReg
                mov     edi, [eax + 4]      ; get data from RADEON_CLOCK_CNTL_DATA
                and     edi, edx            ; and reg data with mask
                or      edi, esi            ; set data to selected bits
@@:
                call    selectPllReg
                mov     [eax + 4], edi      ; output to RADEON_CLOCK_CNTL_DATA
                call    selectPllReg
                cmp     edi, [eax + 4]
                jnz     @b

                xor     eax, eax
                mov     [ebx].IoStatus.Information, eax
                ret

; ############################################################################

                ; --------------------------------------
                ;  Name: readBIOS
                ;  Desc: read from memory-mapped reg
                ; --------------------------------------
readBIOS:
                assume  esi: PCARD
                assume  edx: PIO_STACK_LOCATION
                assume  ebx: PNIRP
                assume  edi: ptr dword

                mov     esi, [esi].card_bios
readMem:
                ; check parameters
                cmp     [edx].DIOCParams.InputBufferLength, size dword
                jne     paramerror

                mov     ecx, [edx].DIOCParams.OutputBufferLength
                push    ecx

                mov     eax, [edi]
                add     esi, eax

                push    ecx
                shr     ecx, 2
                rep movsd
                pop     ecx
                and     ecx, 3
                rep movsb

                pop     [ebx].IoStatus.Information
                xor     eax, eax
                ret

; ############################################################################

                ; ---------------------------------------------
                ;  Name: getAdapterInfo
                ;  Desc: Copy internal card structure to client
                ; ---------------------------------------------
getAdapterInfo:
                assume  esi: PCARD
                assume  edx: PIO_STACK_LOCATION
                assume  ebx: PNIRP
                assume  edi: ptr dword

                ; check parameters
                cmp     [edx].DIOCParams.InputBufferLength, 0
                jne     paramerror

                mov     ecx, [edx].DIOCParams.OutputBufferLength
                cmp     ecx, size CARD
                jne     paramerror

                push    ecx
                shr     ecx, 2
                rep movsd

                pop     [ebx].IoStatus.Information
                xor     eax, eax
                ret

; ############################################################################

                ; --------------------------------------
                ;  Name: getVersion
                ;  Desc: return version number
                ; --------------------------------------
getVersion:
                assume  esi: PCARD
                assume  edx: PIO_STACK_LOCATION
                assume  ebx: PNIRP
                assume  edi: ptr dword

                ; check parameters
                cmp     [edx].DIOCParams.InputBufferLength, 0
                jne     paramerror

                cmp     [edx].DIOCParams.OutputBufferLength, size dword
                jne     paramerror

                mov     [edi], R6PROBE_VERSION
                mov     [ebx].IoStatus.Information, size dword
                xor     eax, eax
                ret

; ############################################################################

                ; --------------------------------------
                ;  Name: IoControl
                ;  Desc: I/O Control Dispatcher
                ;  In:   ECX - pointer to IRP
                ;  Out:  EAX - status code
                ; --------------------------------------

IoControl:                 
                push    ebx
                push    esi
                push    edi

                mov     ebx, ecx           ; Get pIrp
                assume  ebx: PNIRP
                mov     edx, [ebx].Tail.Overlay.CurrentStackLocation
                assume  edx: PIO_STACK_LOCATION
                mov     ecx, [edx].Parameters.DeviceIoControl.IoControlCode

                ;
                ; Optimized part:
                ; Assumes CTL_FUNCTION_SHIFT = 2 and function is
                ; in bits 13-2 and that our ioctl code is 8 bits
                ;
                xor     ecx, IOCTL_R6_READ_PCI
                mov     eax, ecx
                and     eax, NOT R6CTL_FUNC_MASK
                jnz     @fail

                cmp     ecx, lengthof dispatchTable * 4
                jae     @fail

                mov     edi, [ebx].AssociatedIrp.SystemBuffer
                mov     esi, [edx].DeviceObject
                assume  esi: PDEVICE_OBJECT
                mov     esi, [esi].DeviceExtension
                call    [dispatchTable + ecx]
                jmp     @end
@fail:
                xor     eax, eax
                mov     [ebx].IoStatus.Information, eax
                mov     eax, STATUS_NOT_IMPLEMENTED
@end:
                pop     edi
                pop     esi
                pop     ebx
                ret

; ############################################################################
.data
dispatchTable   dd      readPCI, readMMR, writeMMR, readPLL, writePLL, readBIOS,
                        getAdapterInfo, getVersion

                endfile
