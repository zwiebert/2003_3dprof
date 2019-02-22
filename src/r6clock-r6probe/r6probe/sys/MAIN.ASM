; ############################################################################
; $Id: MAIN.ASM,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
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
; $Log: MAIN.ASM,v $
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
                include driver.inc
                .list
                .nocref

; ############################################################################

                ; --------------------------------------
                ;  Publics
                ; --------------------------------------
                public DriverEntry

; ############################################################################

                ; --------------------------------------
                ;  Constants
                ; --------------------------------------
DEVICE_NAME EQU <'r','6','p','r','o','b','e','.','v','x','d'>

; ############################################################################
.data
                ; --------------------------------------
                ;  Module globals
                ; --------------------------------------

cNameString dw  '\','D','e','v','i','c','e','\', DEVICE_NAME, 0
cLinkString dw  '\','D','o','s','D','e','v','i','c','e','s','\', DEVICE_NAME, 0

                include ..\chiplist.inc
                


; ############################################################################
.code
                ; --------------------------------------
                ;  Name: findR6
                ;  Desc: Searches all buses for radeon
                ;  Out:  ZF = 1 when found
                ;        ECX - bus #
                ;        EDX - device & function
                ; --------------------------------------

findR6          PROC

                local   bus: dword
                local   dev: dword
                local   id: dword

                push    edi
                xor     ecx, ecx
@nextbus:
                xor     eax, eax
                mov     bus, ecx
@nextdev:
                mov     dev, eax

                invoke  HalGetBusData, PCIConfiguration, bus, dev,\
                            addr id, sizeof id

                cmp     eax, sizeof id
                jnz     @notfound

                mov     eax, [id]
                cmp     ax, PCI_VENDOR_ID_ATI
                jnz     @notfound

                shr     eax, 16 ; shift device ID to ax
                
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
                mov     ecx, bus
                mov     edx, dev
@end:
                pop     edi
                ret
findR6          ENDP

; ############################################################################
.code
                ; --------------------------------------
                ;  Name: unloadDriver
                ;  Desc: Called when driver is unloading
                ; --------------------------------------

unloadDriver    PROC USES esi driverObject: PDRIVER_OBJECT

                local   linkString: UNICODE_STRING

                assume  esi: PDRIVER_OBJECT
                mov     esi, driverObject
                mov     esi, [esi].DeviceObject
                assume  esi: PDEVICE_OBJECT
                push    esi
                mov     esi, [esi].DeviceExtension
                assume  esi: PCARD
                mov     eax, [esi].card_mmr
                assume  eax: nothing
                test    eax, eax
                jz      @f

                invoke  MmUnmapIoSpace, eax, R6_MMR_SIZE
@@:
                mov     eax, [esi].card_bios
                assume  eax: nothing
                test    eax, eax
                jz      @f

                invoke  MmUnmapIoSpace, eax, R6_BIOS_SIZE
@@:
                mov     ecx, [esi].card_fbsize
                mov     eax, [esi].card_fb
                assume  eax: nothing
                test    eax, eax
                jz      @f

                invoke  MmUnmapIoSpace, eax, ecx
@@:
                call    IoDeleteDevice              ; Driver object is already in stack
                invoke  RtlInitUnicodeString, addr linkString, addr cLinkString
                invoke  IoDeleteSymbolicLink, addr linkString

                ret
unloadDriver    ENDP

; ############################################################################
.code
                ; --------------------------------------
                ;  Name: mainDispatch
                ;  Desc: Dispatcher
                ; --------------------------------------

mainDispatch    PROC deviceObject: dword, pIrp: dword

                mov     ecx, pIrp
                assume  ecx: PNIRP
                mov     eax, [ecx].Tail.Overlay.CurrentStackLocation
                assume  eax: PIO_STACK_LOCATION
                movzx   eax, [eax].MajorFunction

                .if eax == IRP_MJ_CREATE || eax == IRP_MJ_CLOSE

                    xor     eax, eax
                    mov     [ecx].IoStatus.Information, eax
                    jmp     @end2

                .elseif eax == IRP_MJ_DEVICE_CONTROL

                    call    IoControl
                    jmp     @end1

                .endif

                mov     eax, STATUS_NOT_IMPLEMENTED
@end1:
                mov     ecx, pIrp
@end2:
                assume  ecx: PNIRP
                mov     [ecx].IoStatus.Status, eax
                push    eax
                invoke  IoCompleteRequest, ecx, 0
                pop     eax
                ret
mainDispatch    ENDP

; ############################################################################
.code
                ; --------------------------------------
                ;  Name: InitDevice
                ;  Desc: Initialize device
                ; --------------------------------------

InitDevice      PROC USES esi device: PDEVICE_OBJECT
                

                local   pcidata: PCI_COMMON_CONFIG

                mov     esi, device
                assume  esi: PDEVICE_OBJECT
                mov     esi, [esi].DeviceExtension
                assume  esi: PCARD
                xor     eax, eax
                mov     [esi].card_mmr, eax
                mov     [esi].card_bios, eax
                mov     [esi].card_fb, eax

                invoke  findR6
                jz      @f

                mov     eax, STATUS_DEVICE_DOES_NOT_EXIST
                jmp     @end
@@:
                mov     [esi].card_bus, ecx
                mov     [esi].card_dev, edx
                invoke  HalGetBusData, PCIConfiguration, ecx, edx, addr pcidata, sizeof pcidata
                mov     eax, pcidata.u.type0.BaseAddresses[ 0 ]
                and     al, 0f0h
                mov     [esi].card_fbPhy, eax
                mov     eax, pcidata.u.type0.BaseAddresses[ 2 * 4 ]
                and     al, 0f0h
                mov     [esi].card_mmrPhy, eax

                invoke  MmMapIoSpace, eax, 0, R6_MMR_SIZE, MmNonCached
                test    eax, eax
                jz      @nomem
                mov     [esi].card_mmr, eax

                ;
                ; Map BIOS from physical address space to linear
                ; BIOS segment can be found in lower word of RADEON_BIOS_1_SCRATCH
                ;
                mov     edx, dword ptr [eax + RADEON_BIOS_1_SCRATCH]
                movzx   edx, dx
                shl     edx, 4
                invoke  MmMapIoSpace, edx, 0, R6_BIOS_SIZE, MmNonCached
                test    eax, eax
                jz      @nomem
                mov     [esi].card_bios, eax

                ;
                ; Map frame buffer from physical address space to linear
                ;
                mov     edx, [esi].card_fbPhy
                mov     ecx, [esi].card_mmr
                mov     ecx, dword ptr [ecx + RADEON_CONFIG_MEMSIZE]
                mov     [esi].card_fbsize, ecx
                invoke  MmMapIoSpace, edx, 0, ecx, MmNonCached
                test    eax, eax
                jz      @nomem
                mov     [esi].card_fb, eax

                xor     eax, eax
@end:
                ret
@nomem:
                mov     eax, STATUS_NO_MEMORY
                jmp     @end

InitDevice      ENDP

; ############################################################################
.code
                ; --------------------------------------
                ;  Name: DriverEntry
                ;  Desc: Entry point
                ; --------------------------------------

DriverEntry     PROC driverObject: PDRIVER_OBJECT, registryPath: dword

                local   nameString  : UNICODE_STRING
                local   linkString  : UNICODE_STRING
                local   deviceObject: dword

                ;
                ; Create device object first
                ;
                invoke  RtlInitUnicodeString, addr nameString, addr cNameString
                invoke  IoCreateDevice, driverObject, sizeof CARD, addr nameString,\
                            FILE_DEVICE_UNKNOWN, 0, TRUE, addr deviceObject
                test    eax, eax        ; check if successful
                jnz     @end            ; if not, return status

                invoke  RtlInitUnicodeString, addr linkString, addr cLinkString
                invoke  IoCreateSymbolicLink, addr linkString, addr nameString
                test    eax, eax        ; check if successful
                jz      @f              ; if it is, proceed with init

                push    eax             ; delete device object
                invoke  IoDeleteDevice, deviceObject
                pop     eax
                jmp     @end
@@:
                ;
                ; Try initializing device
                ;
                invoke  InitDevice, deviceObject
                test    eax, eax
                jz      @f

                push    eax
                invoke  unloadDriver, driverObject
                pop     eax
                jmp     @end
@@:
                ;
                ; Setup fields in driver object
                ;
                assume  eax: PDRIVER_OBJECT
                mov     eax, driverObject
                mov     [eax].DriverUnload, offset unloadDriver
                mov     ecx, offset mainDispatch
                mov     [eax].MajorFunction + IRP_MJ_CREATE * 4, ecx
                mov     [eax].MajorFunction + IRP_MJ_CLOSE * 4, ecx
                mov     [eax].MajorFunction + IRP_MJ_DEVICE_CONTROL * 4, ecx
                assume  eax: nothing
                xor     eax, eax        ; return STATUS_SUCCESS
@end:
                ret
DriverEntry     ENDP

; ############################################################################

                endfile
