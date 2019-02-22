using System;
using System.Collections;
using System.Windows.Forms;
using Win32;

// Module for registering HotKeys with Windows API.
//
// Required data for each hotkey: KeyData, Description-Text, Callback-Delegate, Callback-Data.
// Notification for each hotkey event: Calling the provided callback with provided data.
namespace Hotkeys {

  public class SystemHotkey : IDisposable {
    #region private data member
    DummyWindowWithEvent m_Window          = new DummyWindowWithEvent();	//window for WM_Hotkey Messages
    Hashtable            m_hotKeysByKey    = new Hashtable();
    Hashtable            m_hotKeysByID     = new Hashtable();
    #endregion

    #region public interface 
    public SystemHotkey() {
      m_Window.ProcessMessage += new MessageEventHandler(MessageEvent);
    }


    public delegate void callback_function(object user_data);
    public void Dispose() {
      m_Window.Dispose();
      foreach(DictionaryEntry den in m_hotKeysByKey) {
        HotKeyData hkd = (HotKeyData)den.Value;
        hkd.Dispose();
      }
      GC.SuppressFinalize(this);
    }
    /// <summary>
    /// Remove with AddHotKey() added key.
    /// </summary>
    /// <param name="key">key used previously in AddHotKey() call</param>
    public void RemoveHotKey(Keys key) {
      HotKeyData hkd = (HotKeyData)m_hotKeysByKey[key];
      if (hkd == null)
        return;
      m_hotKeysByKey.Remove(hkd.m_key);
      m_hotKeysByID.Remove(hkd.m_id);
      hkd.Dispose();
    }

    /// <summary>
    /// Register global hotkey and install callback function
    /// </summary>
    /// <param name="key">KeyCode | Modifiers</param>
    /// <param name="description">Unique description for this callback. If used twice the previous hotkey will be removed first.</param>
    /// <param name="user_data">Used as callback argument</param>
    /// <param name="callback">Called if hotkey is pressed.</param>
    public bool AddHotKey(Keys key, string description, callback_function callback, object user_data) {

      HotKeyData hkd = new HotKeyData(m_Window.Handle, key, description, user_data, callback);

      if (m_hotKeysByKey.ContainsKey(hkd.m_key) || m_hotKeysByID.ContainsKey(hkd.m_id)) {
        RemoveHotKey(hkd.m_key);
      }

      if (hkd.register_key()) {
        m_hotKeysByKey[hkd.m_key] = hkd;
        m_hotKeysByID[hkd.m_id]   = hkd;
        return true;
      }

      hkd.Dispose();
      return false;
    }

    #endregion

    #region private nested class
    class HotKeyData : IDisposable {
      private readonly IntPtr             m_window_handle;
      public  readonly Keys               m_key;
      public  readonly string             m_description;
      private readonly object             m_user_data;
      private readonly callback_function  m_callback;
      public  readonly int                m_id;
      private bool                        m_registered = false;

      public HotKeyData(IntPtr window_handle, Keys key, string description, object user_data, callback_function callback) {
        m_window_handle = window_handle;
        m_key           = key;
        m_description   = description;
        m_user_data     = user_data;
        m_callback      = callback;
        m_id            = Win32.Kernel32.GlobalAddAtom(description);
      }

      ~HotKeyData() {
        Dispose();
      }

      public bool register_key() {
        bool success = false;
        if (!m_registered) {
          int key_code  = (int)m_key & ~(int)Keys.Modifiers;
          int modifiers = net_to_win32modifiers(m_key);
          success = m_registered = Win32.User32.RegisterHotKey(m_window_handle, m_id, modifiers, key_code);
        }
        return success;
      }

      public void do_callback() {
        m_callback(m_user_data);
      }

      public void Dispose() {
        Win32.User32.UnregisterHotKey(m_window_handle, m_id);
        Win32.Kernel32.GlobalDeleteAtom(m_id);
        GC.SuppressFinalize(this);
      }

      static int net_to_win32modifiers(Keys key) {
        int modifiers = 0;
        if (((int)key & (int)Keys.Modifiers) != 0) {
          if (((int)key & (int)Keys.Alt) != 0)
            modifiers |= (int)Modifiers.MOD_ALT;
          if (((int)key & (int)Keys.Control) != 0)
            modifiers |= (int)Modifiers.MOD_CONTROL;
          if (((int)key & (int)Keys.Shift) != 0)
            modifiers |= (int)Modifiers.MOD_SHIFT;
        }
        return modifiers;
      }


    }

    #endregion

    #region private methods
    /// <summary>
    /// Handle WM_Hotkey event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="m"></param>
    /// <param name="Handled"></param>
    void MessageEvent(object sender, ref Message m, ref bool Handled) {	
      if ((m.Msg==(int)Win32.Msgs.WM_HOTKEY) && m_hotKeysByID.ContainsKey((int)m.WParam)) {
        HotKeyData hkd = (HotKeyData)m_hotKeysByID[(int)m.WParam];
        hkd.do_callback();
      }  
    }
    ~SystemHotkey() {
      Dispose();
    }
    #endregion
  }    
}


