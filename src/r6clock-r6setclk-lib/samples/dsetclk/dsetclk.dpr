program dsetclk;

uses
  Forms,
  fmain in 'fmain.pas' {MainForm};

{$R *.RES}

begin
  Application.Initialize;
  Application.Title := 'Setclk Library Sample';
  Application.CreateForm(TMainForm, MainForm);
  Application.Run;
end.
