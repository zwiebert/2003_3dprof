/* $Id: options.h,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
 *
 * Header file for command-line options parser
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
 * $Log: options.h,v $
 * Revision 1.1.1.1  2002/01/25 22:14:25  VSi
 * Imported to CVS
 *
 * ----------------------------------------------------------------------------
 *                    Copyright (C) 1997-2002 Vahur Sinij�rv
 * ----------------------------------------------------------------------------
 */
#ifndef OPTIONS_H
#define OPTIONS_H

typedef struct {
    char          shortname;
    char*         longname;
    unsigned char param;
    void*         var;
    int           mask;
} OPTDEF;

/* Parameter types */
#define OPT_FLAG    0x00
#define OPT_EXEC    0x01
#define OPT_INT     0x81
#define OPT_STR     0x82
#define OPT_EXECINT 0x83
#define OPT_EXECSTR 0x84

#define OPT_PMASK   0x80

extern char *opt_errmsg;

int opt_parse( int, char**, OPTDEF*, int );

#endif
                                                        