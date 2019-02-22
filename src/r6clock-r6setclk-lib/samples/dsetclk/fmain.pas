(* $Id: fmain.pas,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
 *
 * Simple GUI mode RADEON overclocker.
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
 * $Log: fmain.pas,v $
 * Revision 1.1.1.1  2002/01/25 22:14:25  VSi
 * Imported to CVS
 *
 * ----------------------------------------------------------------------------
 *                    Copyright (C) 2001-2002 Vahur Sinij„rv
 * ----------------------------------------------------------------------------
 *)
unit fmain;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs, setclk,
  StdCtrls, ComCtrls;

type
  TMainForm = class(TForm)
    _btnApply: TButton;
    GroupBox1: TGroupBox;
    Label1: TLabel;
    _trkCore: TTrackBar;
    _lblCore: TLabel;
    _lblMem: TLabel;
    _trkMem: TTrackBar;
    Label2: TLabel;
    _btnReset: TButton;
    _chkLocked: TCheckBox;
    _btnDefault: TButton;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure updateAll;
    procedure _trkCoreChange(Sender: TObject);
    procedure _trkMemChange(Sender: TObject);
    procedure _btnResetClick(Sender: TObject);
    procedure _chkLockedClick(Sender: TObject);
    procedure _btnApplyClick(Sender: TObject);
    procedure _btnDefaultClick(Sender: TObject);
  private
    { Private declarations }
    _coreClk:  single;
    _memClk:   single;
    _clkStep:  single;
    _locked:   boolean;
    _modified: boolean;

    procedure setFreqLabel( freq: Integer; lbl: TLabel );
    procedure setModified( flag: Boolean );
    procedure freqChanged( orig, other: TTrackBar; lorig, lother: TLabel );
  public
    { Public declarations }
  end;

var
  MainForm: TMainForm;

implementation

{$R *.DFM}

(*
 * Name: TMainForm.updateAll
 * Desc: Executed when form is created.
 *)
procedure TMainForm.FormCreate(Sender: TObject);
begin
     _modified := false;

     if not _setclk_open then
     begin
          MessageBox( 0, 'Cannot connect to r6probe', nil, MB_OK );
          Exit;
     end;

     updateAll;
end;

(*
 * Name: TMainForm.updateAll
 * Desc: Executed when form is about to be destroyed
 *)
procedure TMainForm.FormDestroy(Sender: TObject);
begin
     _setclk_close;
end;

(*
 * Name: TMainForm.updateAll
 * Desc: Updates all controls and internal variables from hw.
 *)
procedure TMainForm.updateAll;
var
   s: string;
   max: Integer;
   min: Integer;
begin
     _locked  := _setclk_getclock( _coreClk, _memClk );
     _clkStep := _setclk_getclockstep;

     Str( _coreClk:3:2, s );
     _lblCore.Caption := s + ' MHz';

     max := trunc( _setclk_PLLData.max_freq / _clkStep );
     min := trunc( _setclk_PLLData.min_freq / _clkStep );

     if max * _clkStep > _setclk_PLLData.max_freq then
     begin
          dec( max );
     end;

     if min * _clkStep < _setclk_PLLData.min_freq then
     begin
          inc( min );
     end;

     _trkCore.Max       := max;
     _trkCore.Min       := min;
     _trkCore.Frequency := (_trkCore.Max - _trkCore.Min) div 10;
     _trkCore.Position  := round( _coreClk / _clkStep );

     Str( _memClk:3:2, s );
     _lblMem.Caption := s + ' MHz';

     _trkMem.Max       := _trkCore.Max;
     _trkMem.Min       := _trkCore.Min;
     _trkMem.LineSize  := _trkCore.LineSize;
     _trkMem.PageSize  := _trkCore.PageSize;
     _trkMem.Frequency := _trkCore.Frequency;
     _trkMem.Position  := round( _memClk / _clkStep );

     _chkLocked.Checked := _locked;
end;

(*
 * Name: TMainForm.setFreqLabel
 * Desc: Sets label's caption to 'freq' in Mhz.
 *)
procedure TMainForm.setFreqLabel( freq: Integer; lbl: TLabel );
var
   s: string;
   f: single;
begin
   f := freq * _clkStep;
   Str( f:3:2, s );
   lbl.Caption := s + ' MHz';
end;

(*
 * Name: TMainForm.freqChanged
 * Desc: Common response to changes in clock trackbars
 *)
procedure TMainForm.freqChanged( orig, other: TTrackBar; lorig, lother: TLabel );
begin
    setFreqLabel( orig.Position, lorig );

    if _chkLocked.Checked then
    begin
        other.Position := orig.Position;
        setFreqLabel( other.Position, lother );
    end;

    setModified( true );
end;

(*
 * Name: TMainForm._trkCoreChange
 * Desc: Executes when core clock trackbar is changed
 *)
procedure TMainForm._trkCoreChange(Sender: TObject);
begin
    freqChanged( _trkCore, _trkMem, _lblCore, _lblMem );
end;

(*
 * Name: TMainForm._trkMemChange
 * Desc: Executes when memory clock trackbar is changed
 *)
procedure TMainForm._trkMemChange(Sender: TObject);
begin
    freqChanged( _trkMem, _trkCore, _lblMem, _lblCore );
end;

(*
 * Name: TMainForm._btnResetClick
 * Desc: Executes when "Reset" button is clicked
 *)
procedure TMainForm._btnResetClick(Sender: TObject);
begin
    updateAll;
    setModified( false );
end;

(*
 * Name: TMainForm.setModified
 * Desc: Sets or clears modified flag.
 *)
procedure TMainForm.setModified( flag: Boolean );
begin
    if _modified <> flag then
    begin
        _modified := flag;
        _btnApply.Enabled := flag;
    end;
end;

(*
 * Name: TMainForm._chkLockedClick
 * Desc: Executes when "locked" checkbox is clicked
 *)
procedure TMainForm._chkLockedClick(Sender: TObject);
begin
    if _chkLocked.Checked then
    begin
         if _trkMem.Position < _trkCore.Position then
         begin
           _trkCore.Position := _trkMem.Position;
           _trkCoreChange( self );
         end else
         begin
           _trkMem.Position := _trkCore.Position;
           _trkMemChange( self );
         end;
    end;
end;

(*
 * Name: TMainForm._btnApplyClick
 * Desc: Executes when "Apply" button is clicked
 *)
procedure TMainForm._btnApplyClick(Sender: TObject);
var
   coreClk: single;
   memClk:  single;
begin
    coreClk := _trkCore.Position * _clkStep;
    memClk  := _trkMem.Position * _clkStep;
    _setclk_setClock( coreClk, memClk, _chkLocked.Checked );
    setModified( false );
    updateAll;
end;

(*
 * Name: TMainForm._btnDefaultClick
 * Desc: Executes when "Default" button is clicked
 *)
procedure TMainForm._btnDefaultClick(Sender: TObject);
begin
    _trkCore.Position := trunc( _setclk_PLLData.xclk / _clkStep );
    _trkMem.Position  := trunc( _setclk_PLLData.xclk / _clkStep );
    _trkCoreChange( self );
    _trkMemChange( self );
end;

end.
