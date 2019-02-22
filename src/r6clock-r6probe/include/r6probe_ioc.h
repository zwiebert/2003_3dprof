/* $Id: r6probe_ioc.h,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
 *
 * Header file for r6probe I/O control functions
 *
 * ----------------------------------------------------------------------------
 * LICENSE
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License (GPL) as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 *
 * To read the license please visit http://www.gnu.org/copyleft/gpl.html
 * ----------------------------------------------------------------------------
 * $Log: r6probe_ioc.h,v $
 * Revision 1.1.1.1  2002/01/25 22:14:25  VSi
 * Imported to CVS
 *
 * ----------------------------------------------------------------------------
 *                    Copyright (C) 2001-2002 Vahur Sinij„rv
 * ----------------------------------------------------------------------------
 */
#ifndef R6PROBE_IOC_H
#define R6PROBE_IOC_H

//
// Macro definition for defining IOCTL and FSCTL function control codes.  Note
// that function codes 0-2047 are reserved for Microsoft Corporation, and
// 2048-4095 are reserved for customers.
//

#define CTL_CODE( DeviceType, Function, Method, Access ) (                 \
    ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method) \
)

//
// Macro to extract device type out of the device io control code
//
#ifndef   DEVICE_TYPE_FROM_CTL_CODE
#  define DEVICE_TYPE_FROM_CTL_CODE(ctrlCode)     (((ULONG)(ctrlCode & 0xffff0000)) >> 16)
#endif
//
// Define the method codes for how buffers are passed for I/O and FS controls
//

#define METHOD_BUFFERED                 0
#define METHOD_IN_DIRECT                1
#define METHOD_OUT_DIRECT               2
#define METHOD_NEITHER                  3

//
// Define the access check value for any access
//
//
// The FILE_READ_ACCESS and FILE_WRITE_ACCESS constants are also defined in
// ntioapi.h as FILE_READ_DATA and FILE_WRITE_DATA. The values for these
// constants *MUST* always be in sync.
//
//
// FILE_SPECIAL_ACCESS is checked by the NT I/O system the same as FILE_ANY_ACCESS.
// The file systems, however, may add additional access checks for I/O and FS controls
// that use this value.
//


#define FILE_ANY_ACCESS                 0
#define FILE_SPECIAL_ACCESS    (FILE_ANY_ACCESS)
#define FILE_READ_ACCESS          ( 0x0001 )    // file & pipe
#define FILE_WRITE_ACCESS         ( 0x0002 )    // file & pipe

#define R6_TYPE 40000

#define IOCTL_R6_READ_PCI \
    CTL_CODE( R6_TYPE, 0x900, METHOD_BUFFERED, FILE_READ_ACCESS )

#define IOCTL_R6_READ_MMR \
    CTL_CODE( R6_TYPE, 0x901, METHOD_BUFFERED, FILE_READ_ACCESS )

#define IOCTL_R6_WRITE_MMR \
    CTL_CODE( R6_TYPE, 0x902, METHOD_BUFFERED, FILE_READ_ACCESS )

#define IOCTL_R6_READ_PLL \
    CTL_CODE( R6_TYPE, 0x903, METHOD_BUFFERED, FILE_READ_ACCESS )

#define IOCTL_R6_WRITE_PLL \
    CTL_CODE( R6_TYPE, 0x904, METHOD_BUFFERED, FILE_READ_ACCESS )

#define IOCTL_R6_READ_BIOS \
    CTL_CODE( R6_TYPE, 0x905, METHOD_BUFFERED, FILE_READ_ACCESS )

#define IOCTL_R6_GET_ADAPTER_INFO \
    CTL_CODE( R6_TYPE, 0x906, METHOD_BUFFERED, FILE_READ_ACCESS )

#define IOCTL_R6_GET_VERSION \
    CTL_CODE( R6_TYPE, 0x907, METHOD_BUFFERED, FILE_READ_ACCESS )

typedef struct
{
    DWORD addr;
    DWORD mask;
    DWORD data;
} MASKED_WRITE_PARAMS;

#endif