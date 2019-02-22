// r6clock-gui.h : main header file for the r6clock-gui application
//
#pragma once

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"       // main symbols


// Cr6clockguiApp:
// See r6clock-gui.cpp for the implementation of this class
//

class Cr6clockguiApp : public CWinApp
{
public:
	Cr6clockguiApp();


// Overrides
public:
	virtual BOOL InitInstance();

// Implementation
	afx_msg void OnAppAbout();
	DECLARE_MESSAGE_MAP()
};

extern Cr6clockguiApp theApp;