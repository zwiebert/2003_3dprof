// clockerDlg.h : header file
//

#pragma once
#include "afxcmn.h"
#include "afxwin.h"


// CclockerDlg dialog
class CclockerDlg : public CDialog
{
// Construction
public:
	CclockerDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_CLOCKER_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
  afx_msg void OnNMCustomdrawSliderCore(NMHDR *pNMHDR, LRESULT *pResult);
  int sliderVal_core;
  afx_msg void OnBnClickedButtonGet();
  CString core_mhz;
  CString mem_mhz;
  int sliderVal_mem;
  afx_msg void OnEnChangeEditMemMin();
  CSliderCtrl sliderCtrl_core;
  CStatic m_TextCoreMHz;
  CSliderCtrl sliderCtrl_mem;
  afx_msg void OnNMCustomdrawSliderMem(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnBnClickedButtonSet();
  CStatic textCtrl_mem_mhz;
private:
  void get_clock();
  bool set_clock(int core_khz, int mem_khz);
  void preferences_to_gui();
  void gui_to_preferences();
  void output_message(const char *msg);
public:
  unsigned int text_min_core;
  CEdit textCtrl_min_core;
  CEdit textCtrl_min_mem;
  CEdit textCtrl_max_core;
  CEdit textCtrl_max_mem;
  CEdit textCtrl_preSlow_core;
  CEdit textCtrl_preNormal_core;
  CEdit textCtrl_preFast_core;
  CEdit textCtrl_preUltra_core;
  CEdit textCtrl_preSlow_mem;
  CEdit textCtrl_preNormal_mem;
  CEdit textCtrl_preFast_mem;
  CEdit textCtrl_preUltra_mem;
  afx_msg void OnBnClickedButtonPreSlow();
  afx_msg void OnBnClickedButtonPreNormal();
  afx_msg void OnBnClickedButtonPreFast();
  afx_msg void OnBnClickedButtonPreUltra();
  afx_msg void OnBnClickedButtonPrefCancel();
  afx_msg void OnBnClickedButtonPrefOk();
  CStatic textCtrl_devId;
  CStatic textCtrl_venId;
  CStatic textCtrl_message;
  afx_msg void OnBnClickedButtonPreSetMin();
  afx_msg void OnBnClickedButtonPreSetSlow();
  afx_msg void OnBnClickedButtonPreSetNormal();
  afx_msg void OnBnClickedButtonPreSetFast();
  afx_msg void OnBnClickedButtonPreSetUltra();
  afx_msg void OnBnClickedButtonPreSetMax();
  afx_msg void OnDeltaposSpin1(NMHDR *pNMHDR, LRESULT *pResult);
  CSpinButtonCtrl spinCtrl_lim_min_core;
  CSpinButtonCtrl spinCtrl_lim_min_mem;
  CSpinButtonCtrl spinCtrl_pre_slow_core;
  CSpinButtonCtrl spinCtrl_pre_slow_mem;
  CSpinButtonCtrl spinCtrl_pre_normal_core;
  CSpinButtonCtrl spinCtrl_pre_normal_mem;
  CSpinButtonCtrl spinCtrl_pre_fast_core;
  CSpinButtonCtrl spinCtrl_pre_fast_mem;
  CSpinButtonCtrl spinCtrl_pre_ultra_core;
  CSpinButtonCtrl spinCtrl_pre_ultra_mem;
  CSpinButtonCtrl spinCtrl_lim_max_core;
  CSpinButtonCtrl spinCtrl_lim_max_mem;
  afx_msg void OnDeltaposSpinCoreMax(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinLimMinMem(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinPreSlowCore(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinPreSlowMem(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinPreNormalCore(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinPreNormalMem(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinPreFastCore(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinPreFastMem(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinPreUltraCore(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinPreUltraMem(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinLimMaxCore(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnDeltaposSpinLimMaxMem(NMHDR *pNMHDR, LRESULT *pResult);
  afx_msg void OnBnClickedButtonDiagPrint();
  CButton buttonCtrl_setClock;
};
