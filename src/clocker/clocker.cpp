// clocker.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "clocker.h"
#include "clockerDlg.h"
#include <stdio.h>
#include <r6clock-dll.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CclockerApp

BEGIN_MESSAGE_MAP(CclockerApp, CWinApp)
	ON_COMMAND(ID_HELP, CWinApp::OnHelp)
END_MESSAGE_MAP()


// CclockerApp construction

CclockerApp::CclockerApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only CclockerApp object

CclockerApp theApp;


// CclockerApp initialization
char *pci_id_string;


BOOL CclockerApp::InitInstance()
{
  static char buf[9];
  pci_id_string = buf;
  sprintf(pci_id_string, "%x", r6clock_get_id()); 
  FILE *cfg_file = fopen("preferences.cfg", "r");
  if (cfg_file == NULL || !cfg.parse(cfg_file, pci_id_string)) {
    if (cfg_file != NULL)
      fclose(cfg_file);
    cfg_file = fopen("defaults.cfg", "r");
    if (cfg_file)
      cfg.parse(cfg_file, pci_id_string);
    if (cfg_file != NULL)
      fclose(cfg_file);
  }




	// InitCommonControls() is required on Windows XP if an application
	// manifest specifies use of ComCtl32.dll version 6 or later to enable
	// visual styles.  Otherwise, any window creation will fail.
	InitCommonControls();

	CWinApp::InitInstance();

	AfxEnableControlContainer();


	CclockerDlg dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO: Place code here to handle when the dialog is
		//  dismissed with OK
	}
	else if (nResponse == IDCANCEL)
	{
		// TODO: Place code here to handle when the dialog is
		//  dismissed with Cancel
	}

	// Since the dialog has been closed, return FALSE so that we exit the
	//  application, rather than start the application's message pump.
	return FALSE;
}



struct ClockerConfig cfg = {
  200, 200,
    350, 350,
    200, 200,
    278, 270,
    300, 280,
    345, 295,
};


bool ClockerConfig::parse(FILE *in, const char *id)
{
  static char read_id[80] = "";
  int res;
  while (EOF != (res = fscanf(in, "%s %d %d %d %d %d %d %d %d %d %d %d %d",
    read_id,
    &min_core,         &min_mem,
    &max_core,         &max_mem,
    &pre_slow_core,    &pre_slow_mem,
    &pre_normal_core,  &pre_normal_mem,
    &pre_fast_core,    &pre_fast_mem,
    &pre_ultra_core,   &pre_ultra_mem))) {
      if (res == 0)
        continue;
      if (stricmp(id, read_id) == 0)
        return true;
    }

    return false;
}

bool ClockerConfig::save(FILE *out, const char *id)
{
  fprintf(out, "%s %d %d %d %d %d %d %d %d %d %d %d %d\n",
    id,
    min_core,         min_mem,
    max_core,         max_mem,
    pre_slow_core,    pre_slow_mem,
    pre_normal_core,  pre_normal_mem,
    pre_fast_core,    pre_fast_mem,
    pre_ultra_core,   pre_ultra_mem);
  return true;
}

