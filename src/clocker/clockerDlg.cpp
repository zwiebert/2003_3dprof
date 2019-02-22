// clockerDlg.cpp : implementation file
//

#include "stdafx.h"
#include "clocker.h"
#include "clockerDlg.h"
#include <string.h>
#include <stdio.h>

#include "r6clock-dll.h"
#include ".\clockerdlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif



#define pref2khz(khz) ((khz) * 1000)
#define khz2pref(khz) ((khz) * 1000)
#define p2s(khz) ((khz) * 1000)      // pref to slider
#define i2a(nmb) (itoa((nmb), buf, 10))
char buf[80];



// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
  CAboutDlg();

  // Dialog Data
  enum { IDD = IDD_ABOUTBOX };

protected:
  virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

  // Implementation
protected:
  DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
  CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CclockerDlg dialog



CclockerDlg::CclockerDlg(CWnd* pParent /*=NULL*/)
: CDialog(CclockerDlg::IDD, pParent)
, sliderVal_core(0)
, core_mhz(_T(""))
, mem_mhz(_T(""))
, sliderVal_mem(0)
, text_min_core(0)
{
  m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CclockerDlg::DoDataExchange(CDataExchange* pDX)
{
  CDialog::DoDataExchange(pDX);
  DDX_Slider(pDX, IDC_SLIDER_CORE, sliderVal_core);
  DDX_Text(pDX, IDC_STATIC_CORE, core_mhz);
  DDV_MaxChars(pDX, core_mhz, 10);
  DDX_Text(pDX, IDC_STATIC_MEM, mem_mhz);
  DDX_Slider(pDX, IDC_SLIDER_MEM, sliderVal_mem);
  DDX_Control(pDX, IDC_SLIDER_CORE, sliderCtrl_core);
  DDX_Control(pDX, IDC_STATIC_CORE, m_TextCoreMHz);
  DDX_Control(pDX, IDC_SLIDER_MEM, sliderCtrl_mem);
  DDX_Control(pDX, IDC_STATIC_MEM, textCtrl_mem_mhz);
  DDX_Control(pDX, IDC_EDIT_LIM_MIN_CORE, textCtrl_min_core);
  DDX_Control(pDX, IDC_EDIT_LIM_MIN_MEM, textCtrl_min_mem);
  DDX_Control(pDX, IDC_EDIT_LIM_MAX_CORE, textCtrl_max_core);
  DDX_Control(pDX, IDC_EDIT_LIM_MAX_MEM, textCtrl_max_mem);
  DDX_Control(pDX, IDC_EDIT_PRE_SLOW_CORE, textCtrl_preSlow_core);
  DDX_Control(pDX, IDC_EDIT_PRE_NORMAL_CORE, textCtrl_preNormal_core);
  DDX_Control(pDX, IDC_EDIT_PRE_FAST_CORE, textCtrl_preFast_core);
  DDX_Control(pDX, IDC_EDIT_PRE_ULTRA_CORE, textCtrl_preUltra_core);
  DDX_Control(pDX, IDC_EDIT_PRE_SLOW_MEM, textCtrl_preSlow_mem);
  DDX_Control(pDX, IDC_EDIT_PRE_NORMAL_MEM, textCtrl_preNormal_mem);
  DDX_Control(pDX, IDC_EDIT_PRE_FAST_MEM, textCtrl_preFast_mem);
  DDX_Control(pDX, IDC_EDIT_PRE_ULTRA_MEM, textCtrl_preUltra_mem);
  DDX_Control(pDX, IDC_STATIC_DEVID_VALUE, textCtrl_devId);
  DDX_Control(pDX, IDC_STATIC_VENID_VALUE, textCtrl_venId);
  DDX_Control(pDX, IDC_STATIC_MESSAGE, textCtrl_message);
  DDX_Control(pDX, IDC_SPIN_CORE_MAX, spinCtrl_lim_min_core);
  DDX_Control(pDX, IDC_SPIN_LIM_MIN_MEM, spinCtrl_lim_min_mem);
  DDX_Control(pDX, IDC_SPIN_PRE_SLOW_CORE, spinCtrl_pre_slow_core);
  DDX_Control(pDX, IDC_SPIN_PRE_SLOW_MEM, spinCtrl_pre_slow_mem);
  DDX_Control(pDX, IDC_SPIN_PRE_NORMAL_CORE, spinCtrl_pre_normal_core);
  DDX_Control(pDX, IDC_SPIN_PRE_NORMAL_MEM, spinCtrl_pre_normal_mem);
  DDX_Control(pDX, IDC_SPIN_PRE_FAST_CORE, spinCtrl_pre_fast_core);
  DDX_Control(pDX, IDC_SPIN_PRE_FAST_MEM, spinCtrl_pre_fast_mem);
  DDX_Control(pDX, IDC_SPIN_PRE_ULTRA_CORE, spinCtrl_pre_ultra_core);
  DDX_Control(pDX, IDC_SPIN_PRE_ULTRA_MEM, spinCtrl_pre_ultra_mem);
  DDX_Control(pDX, IDC_SPIN_LIM_MAX_CORE, spinCtrl_lim_max_core);
  DDX_Control(pDX, IDC_SPIN_LIM_MAX_MEM, spinCtrl_lim_max_mem);
  DDX_Control(pDX, IDC_BUTTON_SET, buttonCtrl_setClock);
}

BEGIN_MESSAGE_MAP(CclockerDlg, CDialog)
  ON_WM_SYSCOMMAND()
  ON_WM_PAINT()
  ON_WM_QUERYDRAGICON()
  //}}AFX_MSG_MAP
  ON_NOTIFY(NM_CUSTOMDRAW, IDC_SLIDER_CORE, OnNMCustomdrawSliderCore)
  ON_BN_CLICKED(IDC_BUTTON_GET, OnBnClickedButtonGet)
  ON_EN_CHANGE(IDC_EDIT_LIM_MIN_MEM, OnEnChangeEditMemMin)
  ON_NOTIFY(NM_CUSTOMDRAW, IDC_SLIDER_MEM, OnNMCustomdrawSliderMem)
  ON_BN_CLICKED(IDC_BUTTON_SET, OnBnClickedButtonSet)
  ON_BN_CLICKED(IDC_BUTTON_PRE_SLOW, OnBnClickedButtonPreSlow)
  ON_BN_CLICKED(IDC_BUTTON_PRE_NORMAL, OnBnClickedButtonPreNormal)
  ON_BN_CLICKED(IDC_BUTTON_PRE_FAST, OnBnClickedButtonPreFast)
  ON_BN_CLICKED(IDC_BUTTON_PRE_ULTRA, OnBnClickedButtonPreUltra)
  ON_BN_CLICKED(IDC_BUTTON_PREF_CANCEL, OnBnClickedButtonPrefCancel)
  ON_BN_CLICKED(IDC_BUTTON_PREF_OK, OnBnClickedButtonPrefOk)
  ON_BN_CLICKED(IDC_BUTTON_PRE_SET_MIN, OnBnClickedButtonPreSetMin)
  ON_BN_CLICKED(IDC_BUTTON_PRE_SET_SLOW, OnBnClickedButtonPreSetSlow)
  ON_BN_CLICKED(IDC_BUTTON_PRE_SET_NORMAL, OnBnClickedButtonPreSetNormal)
  ON_BN_CLICKED(IDC_BUTTON_PRE_SET_FAST, OnBnClickedButtonPreSetFast)
  ON_BN_CLICKED(IDC_BUTTON_PRE_SET_ULTRA, OnBnClickedButtonPreSetUltra)
  ON_BN_CLICKED(IDC_BUTTON_PRE_SET_MAX, OnBnClickedButtonPreSetMax)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_CORE_MAX, OnDeltaposSpinCoreMax)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_LIM_MIN_MEM, OnDeltaposSpinLimMinMem)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_PRE_SLOW_CORE, OnDeltaposSpinPreSlowCore)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_PRE_SLOW_MEM, OnDeltaposSpinPreSlowMem)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_PRE_NORMAL_CORE, OnDeltaposSpinPreNormalCore)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_PRE_NORMAL_MEM, OnDeltaposSpinPreNormalMem)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_PRE_FAST_CORE, OnDeltaposSpinPreFastCore)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_PRE_FAST_MEM, OnDeltaposSpinPreFastMem)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_PRE_ULTRA_CORE, OnDeltaposSpinPreUltraCore)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_PRE_ULTRA_MEM, OnDeltaposSpinPreUltraMem)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_LIM_MAX_CORE, OnDeltaposSpinLimMaxCore)
  ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN_LIM_MAX_MEM, OnDeltaposSpinLimMaxMem)
  ON_BN_CLICKED(IDC_BUTTON_DIAG_PRINT, OnBnClickedButtonDiagPrint)
END_MESSAGE_MAP()


// CclockerDlg message handlers

BOOL CclockerDlg::OnInitDialog()
{
  CDialog::OnInitDialog();

  // Add "About..." menu item to system menu.

  // IDM_ABOUTBOX must be in the system command range.
  ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
  ASSERT(IDM_ABOUTBOX < 0xF000);

  CMenu* pSysMenu = GetSystemMenu(FALSE);
  if (pSysMenu != NULL)
  {
    CString strAboutMenu;
    strAboutMenu.LoadString(IDS_ABOUTBOX);
    if (!strAboutMenu.IsEmpty())
    {
      pSysMenu->AppendMenu(MF_SEPARATOR);
      pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
    }
  }

  // Set the icon for this dialog.  The framework does this automatically
  //  when the application's main window is not a dialog
  SetIcon(m_hIcon, TRUE);			// Set big icon
  SetIcon(m_hIcon, FALSE);		// Set small icon


  unsigned pci_id = r6clock_get_id();
  sprintf(buf, "%x", R6CLOCK_VEN_ID(pci_id));  textCtrl_venId.SetWindowText(buf);
  sprintf(buf, "%x", R6CLOCK_DEV_ID(pci_id));  textCtrl_devId.SetWindowText(buf);

  preferences_to_gui();
  get_clock();
  UpdateData(FALSE);

  return TRUE;  // return TRUE  unless you set the focus to a control
}

void CclockerDlg::preferences_to_gui()
{
#define limPre_pref2rng(ctrl, pref_min, pref_max)  (void)((ctrl).SetRange(pref_min * 1000, pref_max * 1000))
  sliderCtrl_core.SetRange(p2s(cfg.min_core), p2s(cfg.max_core), FALSE);
  sliderCtrl_mem.SetRange(p2s(cfg.min_mem), p2s(cfg.max_mem), FALSE);

#define limPre_pref2gui(ctrl, spinCtrl, pref) (ctrl).SetWindowText(i2a((pref))), ((spinCtrl.SetRange(0, 1000))), ((spinCtrl.SetPos(pref)))

  limPre_pref2gui(textCtrl_min_core,       spinCtrl_lim_min_core,      cfg.min_core);
  limPre_pref2gui(textCtrl_min_mem,        spinCtrl_lim_min_mem,       cfg.min_mem);
  limPre_pref2gui(textCtrl_max_core,       spinCtrl_lim_max_core,      cfg.max_core);
  limPre_pref2gui(textCtrl_max_mem,        spinCtrl_lim_max_mem,       cfg.max_mem);
  limPre_pref2gui(textCtrl_preSlow_core,   spinCtrl_pre_slow_core,     cfg.pre_slow_core);
  limPre_pref2gui(textCtrl_preSlow_mem,    spinCtrl_pre_slow_mem,      cfg.pre_slow_mem);
  limPre_pref2gui(textCtrl_preNormal_core, spinCtrl_pre_normal_core,   cfg.pre_normal_core);
  limPre_pref2gui(textCtrl_preNormal_mem,  spinCtrl_pre_normal_mem,    cfg.pre_normal_mem);
  limPre_pref2gui(textCtrl_preFast_core,   spinCtrl_pre_fast_core,     cfg.pre_fast_core);
  limPre_pref2gui(textCtrl_preFast_mem,    spinCtrl_pre_fast_mem,      cfg.pre_fast_mem);
  limPre_pref2gui(textCtrl_preUltra_core,  spinCtrl_pre_ultra_core,    cfg.pre_ultra_core);
  limPre_pref2gui(textCtrl_preUltra_mem,   spinCtrl_pre_ultra_mem,     cfg.pre_ultra_mem);
}

void CclockerDlg::gui_to_preferences()
{
#define limPre_rng2pref(ctrl, pref_min, pref_max) (void)(((pref_min) = (ctrl).GetRangeMin() / 1000), ((pref_max) = (ctrl).GetRangeMax() / 1000))
#define limPre_gui2pref(ctrl, spinCtrl, pref) (void)((ctrl).GetWindowText(buf, 4) > 1 && (pref = atoi(buf)))

  limPre_rng2pref(sliderCtrl_core, cfg.min_core, cfg.max_core);
  limPre_rng2pref(sliderCtrl_mem,  cfg.min_mem,  cfg.max_mem);

  limPre_gui2pref(textCtrl_min_core,       spinCtrl_lim_min_core,      cfg.min_core);
  limPre_gui2pref(textCtrl_min_mem,        spinCtrl_lim_min_mem,       cfg.min_mem);
  limPre_gui2pref(textCtrl_max_core,       spinCtrl_lim_max_core,      cfg.max_core);
  limPre_gui2pref(textCtrl_max_mem,        spinCtrl_lim_max_mem,       cfg.max_mem);
  limPre_gui2pref(textCtrl_preSlow_core,   spinCtrl_pre_slow_core,     cfg.pre_slow_core);
  limPre_gui2pref(textCtrl_preSlow_mem,    spinCtrl_pre_slow_mem,      cfg.pre_slow_mem);
  limPre_gui2pref(textCtrl_preNormal_core, spinCtrl_pre_normal_core,   cfg.pre_normal_core);
  limPre_gui2pref(textCtrl_preNormal_mem,  spinCtrl_pre_normal_mem,    cfg.pre_normal_mem);
  limPre_gui2pref(textCtrl_preFast_core,   spinCtrl_pre_fast_core,     cfg.pre_fast_core);
  limPre_gui2pref(textCtrl_preFast_mem,    spinCtrl_pre_fast_mem,      cfg.pre_fast_mem);
  limPre_gui2pref(textCtrl_preUltra_core,  spinCtrl_pre_ultra_core,    cfg.pre_ultra_core);
  limPre_gui2pref(textCtrl_preUltra_mem,   spinCtrl_pre_ultra_mem,     cfg.pre_ultra_mem);

}

void CclockerDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
  if ((nID & 0xFFF0) == IDM_ABOUTBOX)
  {
    CAboutDlg dlgAbout;
    dlgAbout.DoModal();
  }
  else
  {
    CDialog::OnSysCommand(nID, lParam);
  }
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CclockerDlg::OnPaint() 
{
  if (IsIconic())
  {
    CPaintDC dc(this); // device context for painting

    SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

    // Center icon in client rectangle
    int cxIcon = GetSystemMetrics(SM_CXICON);
    int cyIcon = GetSystemMetrics(SM_CYICON);
    CRect rect;
    GetClientRect(&rect);
    int x = (rect.Width() - cxIcon + 1) / 2;
    int y = (rect.Height() - cyIcon + 1) / 2;

    // Draw the icon
    dc.DrawIcon(x, y, m_hIcon);
  }
  else
  {
    CDialog::OnPaint();
  }
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CclockerDlg::OnQueryDragIcon()
{
  return static_cast<HCURSOR>(m_hIcon);
}

void CclockerDlg::OnNMCustomdrawSliderCore(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMCUSTOMDRAW pNMCD = reinterpret_cast<LPNMCUSTOMDRAW>(pNMHDR);
  // TODO: Add your control notification handler code here
  int pos = sliderCtrl_core.GetPos();
  core_mhz.Format("%3.2f", pos * 0.001f);
  m_TextCoreMHz.SetWindowText((pos != 0 && sliderCtrl_core.IsWindowEnabled()) ? core_mhz : "---.--");

  *pResult = 0;
}

void CclockerDlg::OnBnClickedButtonGet()
{
  get_clock();
  UpdateData(FALSE);

  // TODO: Add your control notification handler code here
}

void CclockerDlg::OnEnChangeEditMemMin()
{
  // TODO:  If this is a RICHEDIT control, the control will not
  // send this notification unless you override the CDialog::OnInitDialog()
  // function and call CRichEditCtrl().SetEventMask()
  // with the ENM_CHANGE flag ORed into the mask.

  // TODO:  Add your control notification handler code here
}

void CclockerDlg::OnNMCustomdrawSliderMem(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMCUSTOMDRAW pNMCD = reinterpret_cast<LPNMCUSTOMDRAW>(pNMHDR);
  // TODO: Add your control notification handler code here

  int pos = sliderCtrl_mem.GetPos();
  mem_mhz.Format("%3.2f", pos * 0.001f);
  textCtrl_mem_mhz.SetWindowText((pos != 0 && sliderCtrl_mem.IsWindowEnabled()) ? mem_mhz : "---.--");


  *pResult = 0;
}

void CclockerDlg::OnBnClickedButtonSet()
{
  set_clock(sliderCtrl_core.GetPos(), sliderCtrl_mem.GetPos());
  UpdateData(FALSE);
}


void CclockerDlg::get_clock()
{
  struct r6clock_clocks clocks = r6clock_get_clock();
  bool error = (clocks.core_khz == 0 && clocks.ram_khz == 0);

  if (!error) {
    core_mhz.Format("%3.2f", clocks.core_khz * 0.001f);
    mem_mhz.Format("%3.2f", clocks.ram_khz * 0.001f);
    sliderVal_core = clocks.core_khz;
    sliderVal_mem = clocks.ram_khz;
  } else {
    core_mhz = mem_mhz = "---.--";
  }
  unsigned id = r6clock_get_id();
  output_message((clocks.core_khz == 0 && clocks.ram_khz == 0)
    ? "Error: Could not read clock from hardware"
    : "Last Action: Current clocks were read from hardware.");

  sliderCtrl_mem.EnableWindow(clocks.ram_khz != 0);
  sliderCtrl_core.EnableWindow(clocks.core_khz != 0);
  buttonCtrl_setClock.EnableWindow(clocks.core_khz != 0 || clocks.ram_khz != 0);
}

void CclockerDlg::output_message(const char *msg)
{
  textCtrl_message.SetWindowText(msg);
}

#define clamp(n, n_min, n_max) (void)(((n < n_min) && (n=n_min)) || ((n_max < n) && (n=n_max)))

bool CclockerDlg::set_clock(int core_khz, int mem_khz)
{
  bool error = false;
  clamp(core_khz, pref2khz(cfg.min_core), pref2khz(cfg.max_core));
  clamp(mem_khz,  pref2khz(cfg.min_mem),  pref2khz(cfg.max_mem));

  struct r6clock_clocks clocks = r6clock_set_clock(core_khz, mem_khz);
  error = (clocks.core_khz == 0 && clocks.ram_khz == 0);

  if (!error) {
    core_mhz.Format("%3.2f", clocks.core_khz * 0.001f);
    mem_mhz.Format("%3.2f", clocks.ram_khz * 0.001f);
    sliderVal_core = clocks.core_khz;
    sliderVal_mem = clocks.ram_khz;

  } else {
    core_mhz = mem_mhz = "---.--";
  }

  output_message(error
    ? "Error: Could not write clock to hardware"
    : "Last Action: Adjustet clocks were written to hardware.");

  sliderCtrl_mem.EnableWindow(clocks.ram_khz != 0);
  sliderCtrl_core.EnableWindow(clocks.core_khz != 0);
  buttonCtrl_setClock.EnableWindow(clocks.core_khz != 0 || clocks.ram_khz != 0);

  return !error;
}

void CclockerDlg::OnBnClickedButtonPreSlow()
{
  if (set_clock(cfg.pre_slow_core * 1000, cfg.pre_slow_mem * 1000))
    output_message("Last Action: Clocks were set to preset Slow");
  UpdateData(FALSE);
}

void CclockerDlg::OnBnClickedButtonPreNormal()
{
  if (set_clock(cfg.pre_normal_core * 1000, cfg.pre_normal_mem * 1000))
    output_message("Last Action: Clocks were set to preset Normal");
  UpdateData(FALSE);
}

void CclockerDlg::OnBnClickedButtonPreFast()
{
  if (set_clock(cfg.pre_fast_core * 1000, cfg.pre_fast_mem * 1000))
    output_message("Last Action: Clocks were set to preset Fast");
  UpdateData(FALSE);
}

void CclockerDlg::OnBnClickedButtonPreUltra()
{
  if (set_clock(cfg.pre_ultra_core * 1000, cfg.pre_ultra_mem * 1000))
    output_message("Last Action: Clocks were set to preset Ultra");
  UpdateData(FALSE);
}

void CclockerDlg::OnBnClickedButtonPrefCancel()
{
  preferences_to_gui();
  UpdateData(FALSE);
  output_message("Last Action: All changes discarded. Old values recalled.");
}

void CclockerDlg::OnBnClickedButtonPrefOk()
{
  gui_to_preferences();
  preferences_to_gui();
  UpdateData(FALSE);
  FILE *cfg_out = fopen("preferences.cfg", "w");
  if (cfg_out != NULL) {
    cfg.save(cfg_out, pci_id_string);
    fclose(cfg_out);
  }
  output_message("Last Action: All changes applied and saved.");
}



#define limPre_curr2pref(ctrl, pref) (void)((pref).SetWindowText(itoa((ctrl).GetPos() / 1000, buf, 10)))

void CclockerDlg::OnBnClickedButtonPreSetMin()
{
  limPre_curr2pref(sliderCtrl_core, textCtrl_min_core);
  limPre_curr2pref(sliderCtrl_mem,  textCtrl_min_mem);
  output_message("Last Action: Current clocks are used as lower limit (Min)");
}

void CclockerDlg::OnBnClickedButtonPreSetSlow()
{
  limPre_curr2pref(sliderCtrl_core, textCtrl_preSlow_core);
  limPre_curr2pref(sliderCtrl_mem,  textCtrl_preSlow_mem);
  output_message("Last Action: Current clocks are used as Slow preset");
}

void CclockerDlg::OnBnClickedButtonPreSetNormal()
{
  limPre_curr2pref(sliderCtrl_core, textCtrl_preNormal_core);
  limPre_curr2pref(sliderCtrl_mem,  textCtrl_preNormal_mem);
  output_message("Last Action: Current clocks are used as Normal preset");
}

void CclockerDlg::OnBnClickedButtonPreSetFast()
{
  limPre_curr2pref(sliderCtrl_core, textCtrl_preFast_core);
  limPre_curr2pref(sliderCtrl_mem,  textCtrl_preFast_mem);
  output_message("Last Action: Current clocks are used as Fast preset");
}

void CclockerDlg::OnBnClickedButtonPreSetUltra()
{
  limPre_curr2pref(sliderCtrl_core, textCtrl_preUltra_core);
  limPre_curr2pref(sliderCtrl_mem,  textCtrl_preUltra_mem);
  output_message("Last Action: Current clocks are used as Ultra preset");
}

void CclockerDlg::OnBnClickedButtonPreSetMax()
{  
  limPre_curr2pref(sliderCtrl_core, textCtrl_max_core);
  limPre_curr2pref(sliderCtrl_mem,  textCtrl_max_mem);
  output_message("Last Action: Current clocks are used as upper limit (Max)");
}

#define limPre_spin2text(text, updown) (text).SetWindowText(i2a(updown->iPos + updown->iDelta))
void CclockerDlg::OnDeltaposSpinCoreMax(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_min_core, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinLimMinMem(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_min_mem, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinPreSlowCore(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_preSlow_core, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinPreSlowMem(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_preSlow_mem, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinPreNormalCore(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_preNormal_core, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinPreNormalMem(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_preNormal_mem, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinPreFastCore(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_preFast_core, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinPreFastMem(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_preFast_mem, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinPreUltraCore(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_preUltra_core, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinPreUltraMem(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_preUltra_mem, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinLimMaxCore(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_max_core, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnDeltaposSpinLimMaxMem(NMHDR *pNMHDR, LRESULT *pResult)
{
  LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
  limPre_spin2text(textCtrl_max_mem, pNMUpDown);
  *pResult = 0;
}

void CclockerDlg::OnBnClickedButtonDiagPrint()
{
  const int buf_len = 1024;
  char *buf = new char[buf_len];
  if (r6clockdiag_get_info(buf, buf_len) == 0) {
    if (MessageBox(buf, "Clocker - Diagnostic: Write this to file \"clocker_diag.txt\"?", MB_OKCANCEL) == IDOK) {
      FILE *sout = fopen("clocker_diag.log", "w");
      if (sout != NULL) {
        fprintf(sout, "%s\n", buf);
        fclose(sout);
      }

    }
      return;
  }
}
