(* $Id: r6probe.pas,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
 *
 * r6probe unit for Delphi
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
 * $Log: r6probe.pas,v $
 * Revision 1.1.1.1  2002/01/25 22:14:25  VSi
 * Imported to CVS
 *
 * ----------------------------------------------------------------------------
 *                    Copyright (C) 2001-2002 Vahur Sinij„rv
 * ----------------------------------------------------------------------------
 *)
unit r6probe;

{$L getver.obj}
{$L globals.obj}
{$L open.obj}
{$L close.obj}
{$L readbblk.obj}
{$L readbios.obj}
{$L readmmr.obj}
{$L readpll.obj}
{$L writemmr.obj}
{$L writepll.obj}

interface

uses Windows;

function  _r6probe_getversion: dword; cdecl; external;
function  _r6probe_open: boolean; cdecl; external;
procedure _r6probe_close; cdecl; external;
function  _r6probe_readmmr( address: dword ): dword; cdecl; external;
function  _r6probe_readpll( address: dword ): dword; cdecl; external;
function  _r6probe_readbios( address: dword ): dword; cdecl; external;
function  _r6probe_readbiosblk( buf: pointer; address, count: dword ): dword; cdecl; external;
procedure _r6probe_writemmr( address, mask, data: dword ); cdecl; external;
procedure _r6probe_writepll( address, mask, data: dword ); cdecl; external;

implementation

end.