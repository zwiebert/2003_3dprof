(* $Id: setclk.pas,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
 *
 * setclk unit for Delphi
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
 * $Log: setclk.pas,v $
 * Revision 1.1.1.1  2002/01/25 22:14:25  VSi
 * Imported to CVS
 *
 * ----------------------------------------------------------------------------
 *                    Copyright (C) 2001-2002 Vahur Sinij„rv
 * ----------------------------------------------------------------------------
 *)
unit setclk;

{$L getclk.obj}
{$L setclk.obj}
{$L getmlat.obj}
{$L getmtim.obj}
{$L setmlat.obj}
{$L setmtim.obj}
{$L getclkstep.obj}
{$L close.obj}
{$L open.obj}

interface

uses Windows;

const
    MLAT_FAST   = 0;
    MLAT_MEDIUM = 1;

    MTIM_SLOW   = 0;
    MTIM_MEDIUM = 1;
    MTIM_FAST   = 2;

type
    TPLLData = record
        reference_freq: single;
        reference_div:  dword;
        min_freq:       single;
        max_freq:       single;
        xclk:           single;
    end;

var
    _setclk_PLLData: TPLLData;

    function  _setclk_open: boolean; cdecl; external;
    procedure _setclk_close; cdecl; external;
    function  _setclk_getclock( var coreclk, memclk: single ): boolean; cdecl; external;
    procedure _setclk_setclock( coreclk, memclk: single; locked: boolean ); cdecl; external;
    function  _setclk_getmlat: dword; cdecl; external;
    function  _setclk_getmtim: dword; cdecl; external;
    procedure _setclk_setmlat( latidx: dword ); cdecl; external;
    procedure _setclk_setmtim( timidx: dword ); cdecl; external;
    function  _setclk_getclockstep: single; cdecl; external;

implementation

uses r6probe;

end.