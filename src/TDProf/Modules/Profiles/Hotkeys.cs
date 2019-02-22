using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using Formatter=System.Runtime.Serialization.Formatters.Soap.SoapFormatter;
using System.IO.IsolatedStorage;
using Hotkeys;


namespace TDProf.Profiles
{
	/// <summary>
	/// Summary description for ProfileHotkeys.
	/// </summary>

	public class ProfileHotkeys
	{
    #region private data
	[Serializable()]
    class DataStore {
      public Hashtable m_hotkeys = new Hashtable();
    public void store_to_file(string file_name) {
      Stream stream = null;
      try {
        stream = (!G.config_in_isolated_storage
        ? new FileStream(file_name, FileMode.Create)
        : new IsolatedStorageFileStream(file_name, FileMode.Create));



        Formatter formatter = new Formatter();
        formatter.Serialize(stream, this);
        stream.Close();
      } finally {
        stream.Close();
      }
    }

    public static DataStore create_from_file(string file_name) {
      DataStore obj = null;
      Stream stream = null;
      try {
        stream = //File.Open(file_name, FileMode.Open);
          (!G.config_in_isolated_storage
          ? new FileStream(file_name, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read)
          : new IsolatedStorageFileStream(file_name, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read));
        Formatter formatter = new Formatter();
        obj = (DataStore)formatter.Deserialize(stream);
      } finally {
        stream.Close();
      }
      return obj;
    }

    }

    Hashtable m_hotkeys = new Hashtable();
    SystemHotkey m_sysHotkey;
    SystemHotkey.callback_function m_sysHotkeyCallback;
    #endregion
    #region public interface
		public ProfileHotkeys(SystemHotkey sysHotkey, SystemHotkey.callback_function sysHotkeyCallback)
		{
      m_sysHotkey         = sysHotkey;
      m_sysHotkeyCallback = sysHotkeyCallback;
		}

    public void AddHotkey(string profile_name, Keys key) {
      if (m_hotkeys.ContainsKey(profile_name))
        RemoveHotkey(profile_name);
      if (key == Keys.None)
        return;
      
      m_hotkeys[profile_name] = key;
      m_sysHotkey.AddHotKey(key, "RunProfile." + profile_name, m_sysHotkeyCallback, profile_name);
    }

    public Keys RetrieveHotkey(string profile_name) {
      if (! m_hotkeys.ContainsKey(profile_name))
        return Keys.None;

      return (Keys)m_hotkeys[profile_name];
    }

    public void RemoveHotkey(string profile_name) {
      if (!m_hotkeys.ContainsKey(profile_name))
        return;
      Keys key = (Keys)m_hotkeys[profile_name];
      m_sysHotkey.RemoveHotKey(key);
      m_hotkeys.Remove(profile_name);
    }

    public void RemoveAllHotkeys() {
      foreach(DictionaryEntry den in m_hotkeys) {
        Keys key = (Keys)den.Value;
        m_sysHotkey.RemoveHotKey(key);
      }
      m_hotkeys.Clear();
    }

    public void Save(string file) {
      DataStore ds = new DataStore();
      ds.m_hotkeys = m_hotkeys;
      ds.store_to_file(file);
    }
 
    public void Load(string file) {
      G.applog("** entering ProfileHotkeys.Load(string file)");
      DataStore ds = DataStore.create_from_file(file);
      m_hotkeys = (ds != null) ? ds.m_hotkeys : new Hashtable();
      G.applog("** leaving ProfileHotkeys.Load(string file)");
    }

    public void ActivateHotkeys() {
      foreach(DictionaryEntry den in m_hotkeys) {
        string profile_name = (string)den.Key;
        Keys   key          = (Keys)den.Value;
        m_sysHotkey.AddHotKey(key, "RunProfile." + profile_name, m_sysHotkeyCallback, profile_name);
      }
    }

    public void DeactivateHotkeys() {
      foreach(DictionaryEntry den in m_hotkeys) {
        string profile_name = (string)den.Key;
        Keys   key          = (Keys)den.Value;
        m_sysHotkey.RemoveHotKey(key);
      }
    }

    
    #endregion
	}
}
