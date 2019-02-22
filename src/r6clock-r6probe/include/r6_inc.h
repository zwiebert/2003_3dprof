// ############################################################################
// $Id: r6.inc,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
//
// Include file for Radeon specific stuff
//
// ----------------------------------------------------------------------------
// LICENSE
//
// This program is free software// you can redistribute it and/or modify it
// under the terms of the GNU General Public License (GPL) as published by the
// Free Software Foundation// either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY// without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
// more details.
//
// To read the license please visit http://www.gnu.org/copyleft/gpl.html
// ----------------------------------------------------------------------------
// $Log: r6.inc,v $
// Revision 1.1.1.1  2002/01/25 22:14:25  VSi
// Imported to CVS
//
// ----------------------------------------------------------------------------
//                    Copyright (C) 2001-2002 Vahur Sinij„rv
// ############################################################################

#define PCI_VENDOR_ID_ATI           0x1002

#define PCI_DEVICE_ID_RADEON_LW     0x4C57
#define PCI_DEVICE_ID_RADEON_LX     0x4C58
#define PCI_DEVICE_ID_RADEON_LY     0x4C59
#define PCI_DEVICE_ID_RADEON_LZ     0x4C5A
#define PCI_DEVICE_ID_RADEON_QD     0x5144
#define PCI_DEVICE_ID_RADEON_QE     0x5145
#define PCI_DEVICE_ID_RADEON_QF     0x5146
#define PCI_DEVICE_ID_RADEON_QG     0x5147
#define PCI_DEVICE_ID_RADEON_QY     0x5159
#define PCI_DEVICE_ID_RADEON_QZ     0x515A
#define PCI_DEVICE_ID_R200_BB       0x4242
#define PCI_DEVICE_ID_R200_QL       0x514C
#define PCI_DEVICE_ID_R200_QN       0x514E
#define PCI_DEVICE_ID_R200_QO       0x514F
#define PCI_DEVICE_ID_R200_Ql       0x516C
#define PCI_DEVICE_ID_RV200_QW      0x5157
#define PCI_DEVICE_ID_R300          0x4E45
#define PCI_DEVICE_ID_R300_PRO      0x4E44
#define PCI_DEVICE_ID_R350          0x4E49
#define PCI_DEVICE_ID_R350_PRO      0x4E48
#define PCI_DEVICE_ID_RV350         0x4150
#define PCI_DEVICE_ID_R360          0x4E4A

#define R6_MMR_SIZE                 0x4000

#define R6_BIOS_BASE                0xc0000
#define R6_BIOS_SIZE                0x08000
#define R6_BIOS_HEADER_LOC          0x048

#define RADEON_MEM_TIMINGS          0x00000144
