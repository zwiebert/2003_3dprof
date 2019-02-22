// clocker.h : main header file for the PROJECT_NAME application
//

#pragma once

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols


// CclockerApp:
// See clocker.cpp for the implementation of this class
//

class CclockerApp : public CWinApp
{
public:
	CclockerApp();

// Overrides
	public:
	virtual BOOL InitInstance();

// Implementation

	DECLARE_MESSAGE_MAP()
};

extern CclockerApp theApp;



struct ClockerConfig {
  int min_core,         min_mem;
  int max_core,         max_mem;
  int pre_slow_core,    pre_slow_mem;
  int pre_normal_core,  pre_normal_mem;
  int pre_fast_core,    pre_fast_mem;
  int pre_ultra_core,   pre_ultra_mem;

  bool parse(FILE *in, const char *id);
  bool save(FILE *out, const char *id);
};

extern struct ClockerConfig cfg;
extern char *pci_id_string;
