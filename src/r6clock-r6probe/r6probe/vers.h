/* $Id: vers.h,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
 *
 * Header file for version information
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
 * $Log: vers.h,v $
 * Revision 1.1.1.1  2002/01/25 22:14:25  VSi
 * Imported to CVS
 *
 * ----------------------------------------------------------------------------
 *                    Copyright (C) 2001-2002 Vahur Sinij„rv
 * ----------------------------------------------------------------------------
 */

#ifndef R6VER_H
#define R6VER_H

#define VER_PRODUCTBUILD               1
#define VER_PRODUCTBUILD_QFE           1
#define VER_PRODUCTVERSION             1,04,VER_PRODUCTBUILD
#define VER_PRODUCTVERSION_STRING      "1.05"

#if     ( VER_PRODUCTBUILD < 10 )
#define VER_BPAD "000"
#elif   ( VER_PRODUCTBUILD < 100 )
#define VER_BPAD "00"
#elif   ( VER_PRODUCTBUILD < 1000 )
#define VER_BPAD "0"
#else
#define VER_BPAD
#endif

#if DEBUG
#define VER_DEBUG                   VS_FF_DEBUG
#else
#define VER_DEBUG                   0
#endif

/* default is prerelease */
#if BETA
#define VER_PRERELEASE              VS_FF_PRERELEASE
#else
#define VER_PRERELEASE              0
#endif

#if OFFICIAL_BUILD
#define VER_PRIVATE                 0
#else
#define VER_PRIVATE                 VS_FF_PRIVATEBUILD
#endif

#define VER_FILEFLAGSMASK           VS_FFI_FILEFLAGSMASK
#define VER_FILEOS                  VOS_NT_WINDOWS32
#define VER_FILEFLAGS               (VER_PRERELEASE|VER_DEBUG|VER_PRIVATE)

#define VER_PRODUCTVERSION_STR2(x,y) VER_PRODUCTVERSION_STRING "." VER_BPAD #x "." #y
#define VER_PRODUCTVERSION_STR1(x,y) VER_PRODUCTVERSION_STR2(x, y)
#define VER_PRODUCTVERSION_STR       VER_PRODUCTVERSION_STR1(VER_PRODUCTBUILD, VER_PRODUCTBUILD_QFE)

#define VER_COMPANYNAME_STR         "Vahur Sinijarv"
#define VER_PRODUCTNAME_STR         "Radeon Probe"
#define VER_LEGALTRADEMARKS_STR     "Radeon (tm) is a registered trademark of ATI Technologies Inc."

#endif
