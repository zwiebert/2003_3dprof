(* $Id: dclkinfo.pas,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
 *
 * Console mode program to display RADEON clocks and PLL data.
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
 * $Log: dclkinfo.pas,v $
 * Revision 1.1.1.1  2002/01/25 22:14:25  VSi
 * Imported to CVS
 *
 * ----------------------------------------------------------------------------
 *                    Copyright (C) 2001-2002 Vahur Sinij„rv
 * ----------------------------------------------------------------------------
 *)
program dsetclk;

uses Windows, setclk;

(*
 * Writes a separator to console
 *)
procedure writeSeparator;
begin
    writeln( '----------------------------------------------' );
end;

(*
 * Dumps PLL Info
 *)
procedure writePLLInfo;
begin
    writeln( 'PLL Data' );
    writeSeparator;
    writeln( 'Reference frequency: ', _setclk_PLLData.reference_freq:3:2, ' MHz' );
    writeln( 'Reference divisor  : ', _setclk_PLLData.reference_div );
    writeln( 'Minimum frequency  : ', _setclk_PLLData.min_freq:3:2, ' MHz' );
    writeln( 'Maximum frequency  : ', _setclk_PLLData.max_freq:3:2, ' MHz' );
    writeln( 'Default frequency  : ', _setclk_PLLData.xclk:3:2, ' MHz' );
end;

(*
 * Writes clocks to console
 *)
procedure writeClocks;
var
    coreclk: single;
    memclk:  single;
    locked:  boolean;
begin
    locked := _setclk_getclock( coreclk, memclk );

    writeSeparator;
    writeln( 'Radeon @ ', coreclk:3:2, '/', memclk:3:2, ' (core/mem)' );
    write( 'Core & memory clocks ' );

    if not locked then
    begin
        write( 'not ' );
    end;

    writeln( 'locked' );
    writeSeparator;
end;

(*
 * Writes memory information to console
 *)
procedure writeMem;
var
    dw: dword;
    s:  string;
begin
    dw := _setclk_getmtim;

    case dw of
    MTIM_SLOW:
        s := 'slow';
    MTIM_MEDIUM:
        s := 'medium';
    MTIM_FAST:
        s := 'fast';
    else
        Str( dw, s );
    end;

    writeln( 'Memory timing : ', s );
    dw := _setclk_getmlat;
        
    case dw of
    MLAT_MEDIUM:
        s := 'medium';
    MLAT_FAST:
        s := 'fast';
    else
        Str( dw, s );
    end;
    writeln( 'Memory latency: ', s );
end;

(*
 * Main
 *)
begin
    if _setclk_open then
    begin
        writeClocks;
        writeMem;
        writeln;
        writePLLInfo;
    end else
    begin
        writeln( 'Cannot connect to r6probe' );
    end;

    _setclk_close;
end.