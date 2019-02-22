using System;
using System.IO;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;

using TDProf.DriverSettings;

namespace TDProf.DriverSettings {
  /// <summary>
  /// Save / Restore 3D Settings at runtime and between sessions.
  /// </summary>
  [Serializable()]
  public class AutoRestore {
    int[] cr_saved = new int[(int)TDProf.DriverSettings.ConfigRecord.EMode.NONE];
    float[] clocks;

    public AutoRestore() { 
      for (int i=0; i < cr_saved.Length; ++i)
        cr_saved[i] = -2; //TODO: -1 may be better?
   }
    /// <summary>
    /// save this object to file
    /// </summary>
    /// <param name="file_name">file name to open for write/create</param>
    public void store_to_file(string file_name) {
      Stream stream = null;
      try {
        stream = File.Open(file_name, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, this);
        stream.Close();
      } finally {
        stream.Close();
      }
    }
    /// <summary>
    /// create new object from file
    /// </summary>
    /// <param name="file_name">file name to open for read</param>
    /// <returns>new created object</returns>
    public static AutoRestore create_from_file(string file_name) {
      AutoRestore obj = null;
      Stream stream = null;
      try {
        stream = File.Open(file_name, FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();
        obj = (AutoRestore)formatter.Deserialize(stream);
      } finally {
        stream.Close();
      }
      return obj;
    }

    /// <summary>
    /// Save 3D Settings for later restore
    /// </summary>
    /// <param name="cr"></param>
    public void save_state(ConfigRecord cr) {
      lock (this) {
        for (int i=0; i < cr_saved.Length; ++i) {
          cr_saved[i] = cr.get_modeval_index_of_enabled_item((ConfigRecord.EMode)i);
        }
        clocks = Clocking.clocking_get_clock(true);
      }
    }

 
    public void restore_state(ConfigRecord cr) {
      lock (this) {
        for (int i=0; i < cr_saved.Length; ++i) {
          if (cr_saved[i] != -2)
            cr.enable_modeval_by_index((TDProf.DriverSettings.ConfigRecord.EMode)i, cr_saved[i]);       
        }
        if (clocks != null)
          clocks = Clocking.clocking_set_clock(clocks[0], clocks[1], true);
      }
    }
    //TODO: clock freqs
    public bool is_unchanged(ConfigRecord cr, ConfigRecord.EMode m) {
      return (cr_saved[(int)m] == cr.get_modeval_index_of_enabled_item(m));
  }
}
}
