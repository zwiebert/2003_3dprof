using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using Formatter=System.Runtime.Serialization.Formatters.Soap.SoapFormatter;
using Hotkeys;

namespace TDProf.App
{
	/// <summary>
	/// Summary description for AppHotkeys.
	/// </summary>
	public class AppHotkeys
	{
    #region Private Data

    SystemHotkey.callback_function m_sysHotkeyCallback;
    SystemHotkey m_sysHotkey;

    #endregion

    #region Public Interface

    public enum EAppKeys {
      Iconify, Focus,
      ClockingPresetSLow, ClockingPresetNormal, ClockingPresetFast, ClockingPresetUltra,
      // don't add enums behind count
      count };
    Keys[]     m_keys = new Keys[(int)EAppKeys.count];

    public AppHotkeys(SystemHotkey sysHotkey, SystemHotkey.callback_function sysHotkeyCallback) {
      m_sysHotkey         = sysHotkey;
      m_sysHotkeyCallback = sysHotkeyCallback;
    }

    public void SetHotkey(EAppKeys id, Keys key) {
      if (m_keys[(int)id] != Keys.None)
        m_sysHotkey.RemoveHotKey(key);
      m_keys[(int)id] = key;
      if (key != Keys.None)
        m_sysHotkey.AddHotKey(key, "App." + id.ToString(), m_sysHotkeyCallback, id);
    }

    public Keys GetHotkey(EAppKeys id) {
      return m_keys[(int)id];
    }


    public void Load(AppConfig ac) {
      m_keys[(int)EAppKeys.Iconify] = ac.hotkey_minimize_tray_toggle;
      m_keys[(int)EAppKeys.Focus]   = ac.hotkey_gui_focus;
      m_keys[(int)EAppKeys.ClockingPresetSLow]   = ac.hotkey_clk_pre_slow;
      m_keys[(int)EAppKeys.ClockingPresetNormal] = ac.hotkey_clk_pre_normal;
      m_keys[(int)EAppKeys.ClockingPresetFast]   = ac.hotkey_clk_pre_fast;
      m_keys[(int)EAppKeys.ClockingPresetUltra]  = ac.hotkey_clk_pre_ultra;
    }

    public void Save(AppConfig ac) {
      ac.hotkey_minimize_tray_toggle = m_keys[(int)EAppKeys.Iconify];
      ac.hotkey_gui_focus            = m_keys[(int)EAppKeys.Focus];
      ac.hotkey_clk_pre_slow   = m_keys[(int)EAppKeys.ClockingPresetSLow];
      ac.hotkey_clk_pre_normal = m_keys[(int)EAppKeys.ClockingPresetNormal];
      ac.hotkey_clk_pre_fast   = m_keys[(int)EAppKeys.ClockingPresetFast];
      ac.hotkey_clk_pre_ultra  = m_keys[(int)EAppKeys.ClockingPresetUltra];
      ac.save_config();
    }


    public void ActivateHotkeys() {
      for (int i=0, e=m_keys.Length; i < e; ++i) {
        EAppKeys id = (EAppKeys)i;
        Keys   key  = m_keys[i];
        if (key != Keys.None)
          m_sysHotkey.AddHotKey(key, "App." + id.ToString(), m_sysHotkeyCallback, id);
      }
    }

    public void DeactivateHotkeys() {
      for (int i=0, e=m_keys.Length; i < e; ++i) {
        Keys   key  = m_keys[i];
        m_sysHotkey.RemoveHotKey(key);
      }
    }

    #endregion

	}
}
