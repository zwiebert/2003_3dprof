; ############################################################################
; $Id: ioctl.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
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
; $Log: ioctl.asm,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################
                .586
                .mmx
                option proc: private

; #####################################################################

                ; --------------------------------------
                ;  Includes
                ; --------------------------------------
                .xlist
                include vmm.inc
                include vxdldr.inc
                include vwin32.inc
                include pci.inc
                include winerror.inc
                .list
                option casemap: none

                include r6.inc
                include r6reg.inc
                include r6probe_ioc.inc
                include driver.inc

; #####################################################################
                ; --------------------------------------
                ;  Constants
                ; --------------------------------------

R6CTL_FUNC_MASK EQU     03fch       ; normal ctl_function_mask - 8 upper bits

; #####################################################################
VxD_LOCKED_CODE_SEG

                ; --------------------------------------------------
                ;  Name: R6_WaitForIdle
                ;  Desc: Wait for Radeon to be in idle state
                ;  Mod : EDX, EAX
                ; --------------------------------------------------

R6_WaitForIdle  PROC

                mov     edx, card.card_mmr
                
                R6_WaitForFifo edx, al, 64

                mov     eax, dword ptr [edx + RADEON_RBBM_STATUS]
                test    eax, RADEON_RBBM_ACTIVE
                jnz     wfi_done

                lea     edx, [edx + RADEON_RB2D_DSTCACHE_CTLSTAT]
                or      dword ptr [edx], RADEON_RB2D_DC_FLUSH_ALL
@@:
                test    dword ptr [edx], RADEON_RB2D_DC_BUSY
                jnz     @b
wfi_done:
                ret

R6_WaitForIdle  ENDP

; #####################################################################

                ; --------------------------------------
                ;  Name: R6IOC_ReadPCI
                ;  Desc: Open first card in system
                ;  In  : EDX - hDevice
                ;        ESI - lpDIOCParams
                ; --------------------------------------

R6IOC_ReadPCI   PROC

                mov     eax, [esi.lpvInBuffer]
                push    [eax]
                push    card.card_dev
                push    card.card_bus
                VxDCall _PCI_Read_Config
                add     esp, 3*4

                mov     ebx, [esi.lpvOutBuffer]
                mov     [ebx], eax
                xor     eax, eax
                ret

R6IOC_ReadPCI   ENDP

; #####################################################################

                ; --------------------------------------
                ;  Name: R6IOC_ReadMMR
                ;  Desc: Open first card in system
                ;  In  : EDX - hDevice
                ;        ESI - lpDIOCParams
                ; --------------------------------------

R6IOC_ReadMMR   PROC

                mov     eax, [esi.lpvInBuffer]
                mov     ebx, [esi.lpvOutBuffer]
                mov     eax, [eax]
                and     eax, R6_MMR_SIZE - 1
                add     eax, card.card_mmr
                mov     eax, [eax]
                mov     [ebx], eax
                xor     eax, eax
                ret

R6IOC_ReadMMR   ENDP

; #####################################################################

                ; --------------------------------------
                ;  Name: R6IOC_ReadPLL
                ;  Desc: Open first card in system
                ;  In  : EDX - hDevice
                ;        ESI - lpDIOCParams
                ; --------------------------------------

R6IOC_ReadPLL   PROC

                mov     eax, [esi.lpvInBuffer]
                mov     eax, [eax]
                mov     ebx, card.card_mmr
                and     al, 1fh
@@:
                mov     [ebx + RADEON_CLOCK_CNTL_INDEX], al
                mov     ah, [ebx + RADEON_CLOCK_CNTL_INDEX]
                and     ah, 9fh
                cmp     al, ah
                jnz     @b

                mov     eax, [esi.lpvOutBuffer]
                mov     ebx, [ebx + RADEON_CLOCK_CNTL_DATA]
                mov     [eax], ebx
                xor     eax, eax
                ret

R6IOC_ReadPLL   ENDP

; #####################################################################

                ; --------------------------------------
                ;  Name: Copy memory
                ;  Desc: Copy memory to user buffer
                ;  In  : EDX - hDevice
                ;        ESI - lpDIOCParams
                ;        EAX - base address
                ; --------------------------------------

CopyMemory:
                mov     edi, [esi.lpvOutBuffer]
                mov     ecx, [esi.cbOutBuffer]
                mov     esi, [esi.lpvInBuffer]
                mov     esi, [esi]
                add     esi, eax
CopyMemory2:
                mov     eax, 16
                push    ecx
                shr     ecx, 4
                jz      smallblock
@@:
                movq    MM0, [esi]
                movq    MM1, [esi + 8]
                movq    [edi], MM0
                movq    [edi + 8], MM1
                add     esi, eax
                add     edi, eax
                loop    @b
                emms
smallblock:
                pop     ecx
                and     ecx, 15
                rep movsb

                xor     eax, eax
                ret

; #####################################################################

                ; --------------------------------------
                ;  Name: R6IOC_ReadBIOS
                ;  Desc: Read from Video BIOS
                ;  In  : EDX - hDevice
                ;        ESI - lpDIOCParams
                ; --------------------------------------

R6IOC_ReadBIOS:
                mov     eax, card.card_bios
                jmp     CopyMemory

; #####################################################################

                ; --------------------------------------
                ;  Name: R6IOC_WritePLL
                ;  Desc: Write dword to PLL with mask
                ;  In  : EDX - hDevice
                ;        ESI - lpDIOCParams
                ; --------------------------------------

selectPllReg:
                mov     ah, [ebx + RADEON_CLOCK_CNTL_INDEX]
                and     ah, 0bfh
                cmp     al, ah
                jz      @f
                mov     [ebx + RADEON_CLOCK_CNTL_INDEX], al
                jmp     selectPllReg
@@:
                ret

R6IOC_WritePLL  PROC

                mov     ecx, [esi.lpvInBuffer]
                assume  ecx: PMASKED_WRITE_PARAMS
                mov     eax, [ecx]._addr
                mov     edi, [ecx]._mask            ; mask
                mov     ecx, [ecx]._data            ; data
                and     ecx, edi
                not     edi
                mov     ebx, card.card_mmr
                and     al, 3fh
                or      al, RADEON_PLL_WR_EN

                call    selectPllReg
                mov     edx, [ebx + RADEON_CLOCK_CNTL_DATA]
                and     edx, edi                             ; zero selected bits in dest
                or      edx, ecx                             ; or values together
wpll_again:
                call    selectPllReg
                mov     [ebx + RADEON_CLOCK_CNTL_DATA], edx  ; send to PLL data reg
                call    selectPllReg
                cmp     edx, [ebx + RADEON_CLOCK_CNTL_DATA]  ; read back
                jnz     wpll_again                           ; if the value did not stay, repeat
                
                xor     eax, eax
                ret

R6IOC_WritePLL  ENDP

; #####################################################################

                ; --------------------------------------
                ;  Name: R6IOC_WriteMMR
                ;  Desc: Write dword to MMR with mask
                ;  In  : EDX - hDevice
                ;        ESI - lpDIOCParams
                ; --------------------------------------

R6IOC_WriteMMR  PROC

                mov     ecx, [esi.lpvInBuffer]
                assume  ecx: PMASKED_WRITE_PARAMS
                mov     ebx, [ecx]._addr
                and     ebx, R6_MMR_SIZE - 1
                add     ebx, card.card_mmr

                mov     eax, [ebx]
                
                mov     edx, [ecx]._mask            ; mask
                mov     ecx, [ecx]._data            ; data
                and     ecx, edx                    ; and data with mask
                not     edx                         ; invert mask
                and     eax, edx                    ; zero selected bits in dest
                or      eax, ecx                    ; or values together
                mov     [ebx], eax                  ; send to MMR
                
                xor     eax, eax
                ret

R6IOC_WriteMMR  ENDP

; #####################################################################

                ; --------------------------------------
                ;  Name: R6IOC_GetVersion
                ;  Desc: Return minport version
                ;  In  : EDX - hDevice
                ;        ESI - lpDIOCParams
                ; --------------------------------------

R6IOC_GetVersion proc

                cmp     [esi.cbOutBuffer], sizeof dword
                jnz     @f
                mov     eax, [esi.lpvOutBuffer]
                mov     dword ptr [eax], R6PROBE_VERSION
                xor     eax, eax
                ret
@@:
                mov     eax, ERROR_INVALID_PARAMETER
                ret
R6IOC_GetVersion  ENDP

; #####################################################################

                ; --------------------------------------
                ;  Name: R6IOC_GetAdapterInfo
                ;  Desc: Return card information block
                ;  In  : EDX - hDevice
                ;        ESI - lpDIOCParams
                ; --------------------------------------

R6IOC_GetAdapterInfo proc

                cmp     [esi.cbOutBuffer], sizeof CARD
                jnz     @f
                mov     eax, [esi.lpvOutBuffer]
                mov     ecx, sizeof CARD / 4
                mov     edi, [esi.lpvOutBuffer]
                mov     esi, offset card
                rep     movsd
                xor     eax, eax
                ret
@@:
                mov     eax, ERROR_INVALID_PARAMETER
                ret

R6IOC_GetAdapterInfo endp

; #####################################################################

                ; --------------------------------------
                ;  Name: IOControl
                ;  Desc: Dispatches IO control requests
                ;
                ;  In  : ECX - function id
                ;        EBX - dwDDB
                ;        EDX - hDevice
                ;        ESI - lpDIOCParams
                ; --------------------------------------

IoControl       PROC
                public  IoControl

                cmp     ecx, DIOC_OPEN
                jnz     @f
                xor     eax, eax
                ret
@@:
                cmp     ecx, DIOC_CLOSEHANDLE
                jnz     @f

                mov     bx, UNDEFINED_DEVICE_ID
                lea     edx, vxd_name
                VxDCall VXDLDR_UnloadMe
                xor     eax, eax
                ret
@@:
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

                call    [ dispatchTable + ecx ]
                clc
                ret
@fail:
                mov     eax, ERROR_NOT_SUPPORTED
                clc
                ret

IoControl       ENDP

VxD_LOCKED_CODE_ENDS
; #####################################################################
VxD_LOCKED_DATA_SEG

dispatchTable   dd  R6IOC_ReadPCI, R6IOC_ReadMMR,  R6IOC_WriteMMR,
                    R6IOC_ReadPLL, R6IOC_WritePLL, R6IOC_ReadBIOS,
                    R6IOC_GetAdapterInfo, R6IOC_GetVersion

vxd_name        db  "R6PROBE",0

VxD_LOCKED_DATA_ENDS
; #####################################################################

END