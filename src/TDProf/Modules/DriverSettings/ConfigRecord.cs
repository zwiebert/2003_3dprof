#define OBSOLETE

using System;
using System.IO;
using ArrayList=System.Collections.ArrayList;
using System.Text.RegularExpressions;
using System.Collections;
using Microsoft.Win32;
using TDProf.Profiles;



/*
 
 ConfigRecord cr;  // All data for a given hardware (like "nvidia-nv30"):
 IMode iMode = ConfigRecord[mode]; // Data section for a specific mode (like "Ansio")
 IModeVal iModeVal = iMode[n]; // Data section for a specific mode-option (like "2x")
 
TODO: the naming (and other things) sucks

*/

namespace TDProf.DriverSettings {

  /// <summary>
  /// Provides a list of values for each mode.
  /// Each value is addressed by a mode identifier and an index.
  /// For example mode=EMode.D3D_ANISO and index=2 may address D3D-Aniso-EMode 2x.
  /// All data will be parsed from a config file at program start by the nested
  /// ConfigParser class.
  /// </summary>
  public class ConfigRecord { 
  #region Private Data
    enum ModeClass { D3D, OGL, UNKNOWN };

    private CMode[] m_cfg_store    = new CMode[(int)EMode.NONE]; // filled by parser
    private DisplayInfo   m_display_info = null;
    int m_ati_apply_count_of_3d_changes = 0;

    // A LUT to lookup the class of a given index
    static ModeClass[] modeClass
      = {
          ModeClass.D3D, ModeClass.OGL,
          ModeClass.D3D, ModeClass.OGL,
          ModeClass.D3D, ModeClass.OGL,
          ModeClass.D3D, ModeClass.OGL,
          ModeClass.D3D, ModeClass.OGL,
          ModeClass.D3D, ModeClass.OGL,
          ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D,
          ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, 
          ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, ModeClass.D3D, 
          ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL, ModeClass.OGL,
          ModeClass.UNKNOWN
        };
    int    count_of_d3d_changes = 0;
    int    count_of_ogl_changes = 0;

  #endregion

  #region ctor / init
    private ConfigRecord(string file_name, DisplayInfo displayInfo) {
      G.applog("** entering ConfigRecord.ConfigRecord(...)");

      m_display_info = displayInfo != null ? displayInfo : new DisplayInfo(null, null);
      driver_regkey    = m_display_info.get_driver_key();

      try {
        new ConfigParser(file_name, this);
      }
      catch (System.IO.FileNotFoundException e) {
        
        System.Windows.Forms.MessageBox.Show("Chip config file not found: \"" + e.FileName + "\"", G.app_name);
        throw new FatalError("Important file missing from installation");
      }

      // fill empty slots with empty objects to avoid test for null in methods (uefull?)
      for (int i=0; i < m_cfg_store.Length; ++i)
        if (m_cfg_store[i] == null)
          m_cfg_store[i] = new CMode(0);
		

      G.applog("** leaving ConfigRecord.ConfigRecord(...)");
    }
    /// <summary>
    /// Create new ConfigRecord according to given config file
    /// </summary>
    /// <param name="file_name">name of text file holding the config</param>
    /// <returns></returns>
    public static ConfigRecord create_from_file(string file_name, DisplayInfo di) { return new ConfigRecord(file_name, di); }
  #endregion

  #region Public Interface
 
    #region STATIC
    // A LUT to provide a symbolic name for each EMode.
    public enum EMode {
      D3D_FSAA,  OGL_FSAA,
      D3D_ANISO, OGL_ANISO,
      D3D_VSYNC, OGL_VSYNC,
      D3D_QE, OGL_QE,
      D3D_LOD, OGL_LOD,
      D3D_PRE, OGL_PRE,
      D3D_EXTRA_1, D3D_EXTRA_2, D3D_EXTRA_3, D3D_EXTRA_4, D3D_EXTRA_5, D3D_EXTRA_6, D3D_EXTRA_7, D3D_EXTRA_8, D3D_EXTRA_9, D3D_EXTRA_10,
      OGL_EXTRA_1, OGL_EXTRA_2, OGL_EXTRA_3, OGL_EXTRA_4, OGL_EXTRA_5, OGL_EXTRA_6, OGL_EXTRA_7, OGL_EXTRA_8, OGL_EXTRA_9, OGL_EXTRA_10,
      D3D_EXTRA2_1, D3D_EXTRA2_2, D3D_EXTRA2_3, D3D_EXTRA2_4, D3D_EXTRA2_5, D3D_EXTRA2_6, D3D_EXTRA2_7, D3D_EXTRA2_8, D3D_EXTRA2_9, D3D_EXTRA2_10,
      OGL_EXTRA2_1, OGL_EXTRA2_2, OGL_EXTRA2_3, OGL_EXTRA2_4, OGL_EXTRA2_5, OGL_EXTRA2_6, OGL_EXTRA2_7, OGL_EXTRA2_8, OGL_EXTRA2_9, OGL_EXTRA2_10,
      NONE
    };

    static public bool is_mode_d3d(EMode m) { return modeClass[(int)m] == ModeClass.D3D; }
    static public bool is_mode_ogl(EMode m) { return modeClass[(int)m] == ModeClass.OGL; }

    #endregion
    /// <summary>
    /// If true, registry changes are impossible.
    /// Purpose: to allow testing without risking damages to registry. 
    /// Implemetation: By passing a read only RegistryKey and catch the Exception.
    /// </summary>
    public bool   flag_registry_readonly = true;
    public string driver_regkey;
    public int    specfile_checksum = 0; // checksum of currently loaded file


    #region Mode related
    /// <summary>
    /// test if this EMode is defined in used specfile
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    public bool   mode_exists(EMode label) {
      return ! m_cfg_store[(int)label].is_empty;
    }

    public IMode this[EMode m] {
      get { return m_cfg_store[(int)m]; }
    }
    
    #endregion
    #region ModeVal related
    public bool     modeval_name_or_alias_exists(EMode label, int idx, string name_or_alias) {
      string name = this[label].get_name_by_index(idx);
      if (name == name_or_alias)
        return true;
      return this[label].is_alias(name, name_or_alias);
    }

    /// <summary>
    /// Returns index of the  value currently enabled in registry.
    /// Example: (label=D3D_ANISO) returns index=2
    /// </summary>
    /// <param name="label">points to a (registry) name</param>
    /// <returns>index associated with a registry value</returns>
    public int      get_modeval_index_of_enabled_item(EMode label) {
      try {
        using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(driver_regkey)) {
          return m_cfg_store[(int)label].get_index_of_item_enabled_in_registry(regkey);
        }
      } catch {
        return -1;
      }
    }

    /// <summary>
    /// Call the "write-to-registry" method of the CModeVal object
    /// specified by IDX parameter. The CModeVal was configured by
    /// ConfigParser to know what changes it has to made to registry 
    /// to enable itself. btw: It does not know how to disable itself.
    /// If you need to change registry when a object becomes disabled,
    ///  you have to tell every other value to do this change when becomes enabled itself.
    /// </summary>
    /// <param name="label">points to a (registry) name</param>
    /// <param name="idx">points to a (regisry) value</param>
    public void     enable_modeval_by_index(EMode label, int idx) {
      try {
        using (RegistryKey regkey = (flag_registry_readonly
                 ? Registry.LocalMachine.OpenSubKey(driver_regkey)
                 : Registry.LocalMachine.CreateSubKey(driver_regkey))) {
          if (regkey != null) {
            if (m_cfg_store[(int)label].enable_item_in_registry(regkey, idx)) {
              if (is_mode_d3d(label))
                ++count_of_d3d_changes;
              if (is_mode_ogl(label))
                ++count_of_ogl_changes;
            }
          }
        }
      } catch {
        return;
      }
    }

    #endregion
    #region Profile Apply
    public void    apply_prof(string name, bool restore) {
      GameProfileData gpd = G.ax.gp.get_profile(name);
      if (gpd == null)
        return;
      apply_prof(gpd, restore);
    }

    public void    apply_prof_clocking_only(GameProfileData gpd, bool restore) {
      if (G.ax.ac.feature_clocking && Clocking.clocking_ability()) {
        float clk_core = 0, clk_mem = 0;
        GameProfileData.EClockKinds clocking_kind;

#if ResClkKindByProf
        clocking_kind = ((!restore) ? gpd.clocking_kind : gpd.clocking_restore_kind);
#else
        clocking_kind = ((!restore) ? gpd.clocking_kind : G.ax.ac.clocking_restore_kind);
#endif
        if (clocking_kind == GameProfileData.EClockKinds.PARENT) {
          if (gpd.val(GameProfileData.Parms.CLOCKING_MEM_CLK).Enabled)
            clk_core = gpd.clocking_core_clock;
          if (gpd.val(GameProfileData.Parms.CLOCKING_CORE_CLK).Enabled)
            clk_mem = gpd.clocking_mem_clock;

        } else {
          int[] clocks = {0, 0};
          if (clocking_kind == GameProfileData.EClockKinds.PRE_SLOW) {
            clocks = G.ax.ac.clocking_preset_slow;
          } else if (clocking_kind == GameProfileData.EClockKinds.PRE_NORM) {
            clocks = G.ax.ac.clocking_preset_normal;
          } else if (clocking_kind == GameProfileData.EClockKinds.PRE_FAST) {
            clocks = G.ax.ac.clocking_preset_fast;
          } else if (clocking_kind == GameProfileData.EClockKinds.PRE_ULTRA) {
            clocks = G.ax.ac.clocking_preset_ultra;
          }
          clk_core = (float)clocks[0];
          clk_mem  = (float)clocks[1];
        }
        Clocking.clocking_set_clock(clk_core, clk_mem, true);
      }
    }

    public void    apply_prof(GameProfileData gpd, bool restore) {
      // apply included profile recursively
      if (gpd.include_other_profile != "") 
        apply_prof(gpd.include_other_profile, false);

      for (int i=0; i < G.gp_parms.Length; ++i) {
        GameProfileData.Value gpd_val = gpd.val(G.gp_parms[i]);
        // set modes in registry if enabled and valid
        if (gpd_val.Enabled && gpd_val.Data != "") {
          ConfigRecord.EMode cr_mode = G.cr_modes[i];
          G.ax.cr.enable_modeval_by_index(cr_mode, G.ax.cr[cr_mode].get_index_by_name(gpd_val.Data));
        }
      }
      ati_apply_d3d(false);

      apply_prof_clocking_only(gpd, restore);

      if (!restore)
        G.ax.di.save_resolution();
      else
        G.ax.di.restore_resolution();
    }

    public void    ati_apply_d3d(bool force) {
      if (force || (m_ati_apply_count_of_3d_changes != G.ax.cr.count_of_d3d_changes)) {
        G.ax.di.ati_apply_d3d(false);
        m_ati_apply_count_of_3d_changes = G.ax.cr.count_of_d3d_changes;
      }
    }

    #endregion
    #region Reports
    public string[][][] hmr_format() {
      string[][][] result = new string[m_cfg_store.Length][][];
      for (int i=0; i < m_cfg_store.Length; ++i) {
        result[i] = m_cfg_store[i].hmr_format();
        if (result[i][0][0] == "") {
          result[i][0][0] = ((EMode)i).ToString() + " unused";
        }
      }
      return result;
    }
 
    #endregion

    public interface IMode {
      string gui_label { get; }
      string gui_tooltip { get; }
      int    gui_width_mult { get; }
      void   add_gui_property(string key, Object data);
 
      bool is_empty { get; }

      void insert_item(IModeVal item, int idx);
      void append_item(IModeVal item);
      void append_items(System.Collections.ICollection items);

      void add_alias(string name, string alias);
      bool is_alias(string name, string alias);

      string[] names { get; }
      string[] texts { get; }


      int      get_index_by_name(string name);
      string   get_name_by_index(int idx);
      string   get_text_by_index(int idx);

      IModeVal get_item_by_index(int idx);
      IModeVal this[int idx] { get; }
      IModeVal get_item_by_name(string name);
      IModeVal this[string name] { get; }



      int  get_index_of_item_enabled_in_registry(RegistryKey regkey);
      bool enable_item_in_registry(RegistryKey regkey, int idx);

      string[][] hmr_format();
    }

    public interface IModeVal {

      /// <summary>
      /// Test if this object ist the one, which is currently activated in registry.
      /// In other words, test if this.enable_in_registry() would change anything there.
      /// </summary>
      /// <param name="regkey">readable RegistryKey</param>
      /// <returns></returns>
      bool is_enabled_in_registry(RegistryKey regkey);


      /// <summary>
      /// Change registry values according to our stored data sets.
      /// </summary>
      /// <param name="regkey">writable RegistryKey</param>
      void enable_in_registry(RegistryKey regkey);

      /// <summary>
      /// Make human and machine readable string representation of this object
      /// </summary>
      /// <returns></returns>
      string[] hmr_format();
    }



  #endregion PUBLIC INTERFACE

    #region Private Nested Classes
    /*
     class CModeVal
     class CMode  
     class ConfigRecord  
    */ 
   

    /// <summary>
    /// holds data for a single line of config file, e.g. name="App" rv("xyz"="0")
    /// </summary>
    class CModeVal : IModeVal {
      const int array_size = 8; // max number of registry value per item
      public enum RegType { NONE, REG_SZ, REG_DWORD, REG_BINARY, REG_NOT_EXIST };

      public static System.Type
        type_reg_sz     = System.Type.GetType("System.String"),
        type_reg_dword  = System.Type.GetType("System.Int32"),
        type_reg_binary = System.Type.GetType("System.Byte[]");

      private string m_name;
      public string parent_item = null; // to inherit texts from

      private System.Collections.Hashtable m_text = null; // different language versions of m_name. idices are "default", "en", "de", ...
      public readonly bool is_hidden;
      rv[] m_rv = new rv[array_size];


      class rv {
        #region private data
        private string  m_regval  = null;           // Registry Value Name
        private string  m_regdata = null;           // Registry Data for Value
        private string  m_regdmsk = null;           // Bitmask for Data
        private string  m_regkey  = null;           // Registry Key (if null, then use key provided by caller)
        private RegType m_regtype = RegType.NONE;   // Registry Type for Value/Data
        private bool    m_wrtonly = false;          // If true, then ignore this RV in is_enabled_in_registry()
        #endregion
        /// <summary>
        /// If true, a undefined registry value matches too. (useful if e.g. value="0" means
        ///  the same as a not existing value)
        /// </summary>
        public bool    m_default_val = false;

        #region properties

       public string RegValueName {
          get { return expand_vars(m_regval); }
        }
        public object RegDataObject {
          get { return convert_string_to_object(m_regdata, m_regtype); }
        } 
        public object RegDataMaskObject {
          get { object obj = convert_string_to_object(m_regdmsk, m_regtype);
                return  (obj == null && m_regtype == RegType.REG_DWORD) ? (Int32)~0 : obj;
          }
        }
        public bool RegValueNotExist {
          get { return m_regtype == RegType.REG_NOT_EXIST; }
        }
        public bool WriteOnly {
          get { return m_wrtonly; }
        }
        #endregion

        #region ctors

        public rv() {
        }

        public rv(rv other) {
          m_regval      = other.m_regval;
          m_regtype     = other.m_regtype;
          m_regdata     = other.m_regdata;
          m_regdmsk     = other.m_regdmsk;
          m_wrtonly     = other.m_wrtonly;
          m_default_val = other.m_default_val;
        }

        public rv(string regval, RegType regtype, string regdata, string regdmsk, bool wrtonly, bool default_value) {
          m_regval      = regval;
          m_regtype     = regtype;
          m_regdata     = regdata;
          m_regdmsk     = regdmsk;
          m_wrtonly     = wrtonly;
          m_default_val = default_value;
        }

        #endregion

        static private object convert_string_to_object(string regdata_string, RegType object_type) {
          if (regdata_string == null)
            return null;

          if (object_type == RegType.REG_SZ) {
            return regdata_string;

          } else if (object_type == RegType.REG_BINARY) {
            int byte_count = regdata_string.Length / 2;
            byte[] obj = new byte[byte_count];
            for (int i=0; i < byte_count; ++i) {
              string tem = regdata_string.Substring(i*2, 2);
              byte tem2 = byte.Parse(tem, System.Globalization.NumberStyles.HexNumber);
              obj[i] = tem2;
            }
            return obj;

          } else if (object_type == RegType.REG_DWORD) {
            Int32 obj = Int32.Parse(regdata_string, System.Globalization.NumberStyles.HexNumber);
            return obj;
          }
          return null;
        }

        static private string expand_vars(string s) {
          if (s.IndexOf("$") != -1) {
            s = s.Replace("$GUID$", G.ax.di.get_guid());
          }
          return s;
        }

        public string hmr_format_regdata() {

          switch(m_regtype) {

            case RegType.REG_SZ:
              return "\"" + m_regdata + "\"";

            case RegType.REG_BINARY: {
              byte[] data = (byte[])this.RegDataObject;
              byte[] mask = this.RegDataMaskObject as byte[];
              string s = "hex:";
              foreach(byte b in data) {
                s += string.Format("{0:x2},", b);
              }
              s = s.Substring(0, s.Length - 1);
              if (mask != null) {
                s = s += "#";
                foreach(byte b in mask) {
                  s += string.Format("{0:x2},", b);
                }         
                s = s.Substring(0, s.Length - 1) + " (data#bitmask)";
              }
              return s;
            }

            case RegType.REG_DWORD: {
              Int32 data = (Int32)this.RegDataObject;
              Int32 mask = (Int32)this.RegDataMaskObject;

              if (mask == ~0)
                return string.Format("dword:{0:x8}", data);
              else
                return string.Format("dword:{0:x8}#{1:x8}", data, mask);
            }

            default:
              return "undef:dummy";
          }
        }

      };



      static private string convert_binary_to_string(byte[] obj) {
        string s = "";
        for (int i=0; i < obj.Length; ++i) {
          //			if (i > 0)
          //				s += ", ";
          s += string.Format("{0:x2}", obj[i]);
        }
        return s;
      }

      /// <summary>
      /// Test if OBJ holds the same value as V of type T.
      /// </summary>
      /// <param name="obj">some object of unknown type</param>
      /// <param name="t">type used for parsing V</param>
      /// <param name="v">string representation of a object</param>
      /// <returns></returns>

      static private bool compare_typed_value(object lhs, object rhs, object regdata_mask) {
        if (lhs == null || rhs == null || lhs.GetType() != rhs.GetType())
          return false;
      
        System.Type type = lhs.GetType();

        if (type == type_reg_sz) {
          return ((string)lhs).ToLower() == ((string)rhs).ToLower();
        }

        if (type == type_reg_binary)  {
          byte[] ba = (byte[])lhs;
          byte[] ba_msk = (byte[])regdata_mask;
          byte[] ba_v = (byte[])rhs;

          // extend length of binary data for benefit of ATI CCC (e.g. hex:30,00,00,00 == hex:30,00)
          if (ba.Length > ba_v.Length) {
            byte[] tem = new byte[ba.Length];
            ba_v.CopyTo(tem, 0);
            ba_v = tem;
          }

          if (ba.Length != ba_v.Length) {
            return false;
          }
          for(int i=0; i < ba.Length; ++i) {
            byte mask = (ba_msk == null) ? (byte)0xff : ba_msk[i];
            if ((ba[i] & mask) != (ba_v[i] & mask))
              return false;
          }
          return true;
        }

        if (type == type_reg_dword) {
          if (regdata_mask == null)
            return (System.Int32)lhs ==  (System.Int32)rhs;
          else
            return (((System.Int32)lhs & (System.Int32)regdata_mask)
              ==  ((System.Int32)rhs & (System.Int32)regdata_mask));
        }

        return false;

      }
 
      /// <summary>
      /// Test if N is a valid name for this object. Note: Names are case independent.
      /// </summary>
      /// <param name="n">Name</param>
      /// <returns></returns>
      public bool compare_name(string n) { return n != null && n.ToLower() == m_name.ToLower(); }
      public string name { get { return m_name; } }
      public string text {
        get {
          if (m_text == null)
            return name;
          string l = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
          string result = null;
          if ((result = (string)m_text[l]) != null)
            return result;
          if ((result = (string)m_text["default"]) != null)
            return result;
          return name;
        }
      }
      // constructing
      public CModeVal(string name, bool isHidden) {
        m_name = name;
        is_hidden = isHidden;
      }
      public bool is_usable() { return m_rv[0] != null; }
      /// <summary>
      /// append an additionally set of RegistryValue, Type, Value to this object.
      /// Currently, up to array_size sets can be hold by the object.
      /// </summary>
      /// <param name="regval">name we later use to read/write to registry</param>
      /// <param name="type">type we later use to write to registry</param>
      /// <param name="regdata_string">value we later write to registry</param>
      /// <returns></returns>
      public bool add(string regval, RegType type, string regdata_string, string mask, bool def, bool is_wrtonly) {
        for (int i=0; i < array_size; ++i)
          if (m_rv[i] == null) {
            if (type == RegType.REG_BINARY) {
              regdata_string = regdata_string.Replace(",", "");
              regdata_string = regdata_string.Replace(" ", "");
              if (mask != null) {
                mask = mask.Replace(",", "");
                mask = mask.Replace(" ", "");
              }
            }
            m_rv[i] = new rv(regval, type, regdata_string, mask, is_wrtonly, def);
            return true;
          }
        return false;
      }
      /// <summary>
      /// Overload ussing a string representation for RegType.
      /// </summary>
      /// <param name="regval"></param>
      /// <param name="type">character string representation of RegType</param>
      /// <param name="regdata_string"></param>
      /// <returns></returns>
      public bool add(string regval, string type, string regdata_string, string mask, bool def, bool is_wrtonly) {
        RegType t;
        if (type == null || type == "") return add(regval, regdata_string, mask, def, is_wrtonly); 
        else if (type == "REG_SZ") t = RegType.REG_SZ;
        else if (type == "REG_BINARY") t = RegType.REG_BINARY;
        else if (type == "REG_DWORD") t = RegType.REG_DWORD;
        else if (type == "REG_NOT_EXIST") t = RegType.REG_NOT_EXIST;
        else return false;

        return add(regval, t, regdata_string, mask, def, is_wrtonly);
      }

      public bool add(string regval, string regdata_string, string mask, bool def, bool is_wrtonly) {
        if (regdata_string.ToLower().StartsWith("hex:"))
          return add(regval, RegType.REG_BINARY, regdata_string.Substring(4), mask, def, is_wrtonly);
        else if (regdata_string.ToLower().StartsWith("dword:"))
          return add(regval, RegType.REG_DWORD, regdata_string.Substring(6), mask, def, is_wrtonly);
        else if (regdata_string.ToLower().StartsWith("undef:"))
          return add(regval, RegType.REG_NOT_EXIST, regdata_string.Substring(6), mask, def, is_wrtonly);
        else
          return add(regval, RegType.REG_SZ, regdata_string, mask, def, is_wrtonly);
      }

      public void add_text(string language, string text) {
        if (m_text == null) m_text = new Hashtable(10);
        m_text.Add(((language != null) ? language : "default"), text);
      }

      public void retrofit(CModeVal other) {
        if (other.m_text != null) {
          if (m_text == null) m_text = new Hashtable(10);
          foreach (DictionaryEntry den in other.m_text)
            m_text[den.Key] = den.Value;
        }

        for (int i=0; i < array_size; ++i) {
          if (other.m_rv[i] == null)
            break;
          for (int k=0; k < array_size; ++k) {
             if (this.m_rv[k] != null)
               continue;
            m_rv[k] = new rv(other.m_rv[i]);
            break;
          }

        }
      }

      RegistryKey get_base_key(ref string regval, RegistryKey default_key) {
        if (regval == null || !regval.StartsWith(@"\"))
          return default_key;
        RegistryKey rk = default_key;

        if (regval.StartsWith(@"\HKLM\")) {
          rk = Registry.LocalMachine;
          regval = regval.Substring(6);
        } else if (regval.StartsWith(@"\HKCU\")) {
          rk = Registry.CurrentUser;
          regval = regval.Substring(6);
        } else if (regval.StartsWith(@"\HKU\")) {
          rk = Registry.Users;
          regval = regval.Substring(5);
        } else if (regval.StartsWith(@"\HKCC\")) {
          rk = Registry.CurrentConfig;
          regval = regval.Substring(6);
        } else if (regval.StartsWith(@"\HKCR\")) {
          rk = Registry.ClassesRoot;
          regval = regval.Substring(6);
        } else {
          throw new FatalError("invalid base key in registry value: " + regval);
        }
        return rk;
      }

      /// <summary>
      /// Test if this object ist the one, which is currently activated in registry.
      /// In other words, test if this.enable_in_registry() would change anything there.
      /// </summary>
      /// <param name="regkey">readable RegistryKey</param>
      /// <returns></returns>
      public bool is_enabled_in_registry(RegistryKey regkey) {

        for (int i=0; i < array_size; ++i) {
          if (m_rv[i] == null)
            continue;
          if (m_rv[i].WriteOnly)
            continue;

          bool subkey = false;
          string regval = m_rv[i].RegValueName;
          // get a base key (no need to close it!)
          RegistryKey rk = get_base_key(ref regval, regkey);

          int rv_sep = regval.LastIndexOf(@"\");
          if (1 <= rv_sep) {
            rk = rk.OpenSubKey(regval.Substring(0, rv_sep));
            subkey = true;
            regval = regval.Substring(rv_sep + 1);
          }

          if (rk == null) {
            if (this.m_rv[i].RegValueNotExist || this.m_rv[i].m_default_val == true)
              continue;
            return false;
          }

          Object obj = rk.GetValue(regval);
          if (subkey)
            rk.Close();
          if (obj == null && (this.m_rv[i].RegValueNotExist || this.m_rv[i].m_default_val == true))
            continue;
          else if (obj == null || !compare_typed_value(obj, m_rv[i].RegDataObject, m_rv[i].RegDataMaskObject))
            return false;                 
        }


        return m_rv[0] != null;
      }

      /// <summary>
      /// Change registry values according to our stored data sets.
      /// </summary>
      /// <param name="regkey">writable RegistryKey</param>
      public void enable_in_registry(RegistryKey regkey) {
        for (int i=0; i < array_size; ++i) {
          if (m_rv[i] == null)
            continue;

          bool is_subkey = false;
          string regval = m_rv[i].RegValueName;
          RegistryKey rk = get_base_key(ref regval, regkey);
          int rv_sep = regval.LastIndexOf(@"\");
          if (rk == null)
            continue;

          // if path contains keys then open them
          if (1 <= rv_sep) {
            rk = rk.CreateSubKey(regval.Substring(0, rv_sep));
            is_subkey = true;
            regval = regval.Substring(rv_sep + 1);
            if (rk == null)
              continue;
          }

          if (this.m_rv[i].RegValueNotExist) {
            rk.DeleteValue(regval, false);
            continue;
          }
 
          object new_data = m_rv[i].RegDataObject;
          object msk_data = m_rv[i].RegDataMaskObject;
          if (msk_data != null) {
            object old_data = rk.GetValue(regval);
            if (old_data != null) {
              System.Type type = new_data.GetType();
              if (type == type_reg_sz) {
                // TODO: handle numbers in strings like for Radeon
              }
              if (type == type_reg_binary)  {
                byte[] ba_old = (byte[])old_data;
                byte[] ba_msk = (byte[])msk_data;
                byte[] ba_new = (byte[])new_data;
                for(int k=0; k < ba_new.Length; ++k) {
                  ba_new[k] = (byte)((int)(ba_new[k] & ba_msk[k]) | (int)(ba_old[k] & ~ba_msk[k]));
                }
              }
              if (type == type_reg_dword) {
                new_data = ((System.Int32)new_data & (System.Int32)msk_data)
                  | ((System.Int32)old_data & ~(System.Int32)msk_data);
              }
            }
          }             
          rk.SetValue(regval, new_data);
          if (is_subkey)
            rk.Close();
          
        }
      }

      /// <summary>
      /// Make human and machine readable string representation of this object
      /// </summary>
      /// <returns></returns>
      public string[] hmr_format() {
        int count=0;
        for (int i=0; i < array_size; ++i)
          if (m_rv[i] != null)
            ++count;


        string[] result = new string[count + 1];
        result[0] = m_name;
        for (int i=0; i < array_size; ++i)
          if (m_rv[i] != null) {
            result[i+1] = string.Format(@"""{0}""{1}{2}",
              m_rv[i].RegValueName,
              ((m_rv[i].WriteOnly) ? ":=" : "="),
              m_rv[i].hmr_format_regdata());
          }
        return result;
      }

    }

    /// <summary>
    /// holds data for a labelled block in config file
    /// </summary>
    class CMode : IMode {
      private string m_name = "";
      private Hashtable m_items;
      private ArrayList m_hidden_items = new ArrayList(10);
      private Hashtable m_aliases;

      private class CModeValWithIndex {
        public readonly int index;
        public readonly CModeVal configItem;
        public CModeValWithIndex(CModeVal item, int idx) { configItem = item; index = idx; }
      }

      // store all language versions of some gui texts
      private Hashtable m_gui = new Hashtable(); 

      private string get_gui_text(string key) {
        string result = (string)m_gui[key + "." + System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName];
        if (result != null)
          return result;
        return (string)m_gui[key];
      }

      public string gui_label       { get { return get_gui_text("gui_label"); } }
      public string gui_tooltip     { get { return get_gui_text("gui_tooltip"); } }
      public int    gui_width_mult  { get { return (m_gui.ContainsKey("gui_width_mult") ? (int)m_gui["gui_width_mult"] : 0); } }

      public void add_gui_property(string key, Object data) {
        m_gui[key] = data;
      }
 
      //constructing
      public CMode(int size) {
        m_items = new Hashtable(size);

      }
      public CMode(string name, int size) {
        m_name = name;
        m_items = new Hashtable(size);
      }

      public void insert_item(IModeVal if_item, int idx) {
        CModeVal item = (CModeVal)if_item;
        if (!item.is_hidden)
          m_items.Add(item.name, new CModeValWithIndex(item, idx));
        else
          m_hidden_items[idx] = item;      
      }

      public void append_item(IModeVal if_item) {
        CModeVal item = (CModeVal)if_item;
        if (!item.is_hidden)
          m_items.Add(item.name, new CModeValWithIndex(item, m_items.Count));      
        else
          m_hidden_items.Add(item);      
      }
 
      public void append_items(System.Collections.ICollection items) {
        foreach(object item in items)
          append_item((CModeVal)item);
      }
 
      public void add_alias(string name, string alias) {
        if (m_aliases == null)
          m_aliases = new Hashtable();
        m_aliases[alias] = name;
      }
 
      public bool is_alias(string name, string alias) {
        return m_aliases != null && (string)m_aliases[alias] == name;
      }

      public int get_index_by_name(string name) {
        if (m_aliases != null && m_aliases.ContainsKey(name))
          name = (string)m_aliases[name];
        CModeValWithIndex item = (CModeValWithIndex)m_items[name];
        if (item != null)
          return item.index;

        return -1; // not existing
      }

      public bool is_empty { get { return m_items.Count == 0; } }

      public IModeVal get_item_by_index(int idx) {
        if (0 <= idx) { 
          foreach (DictionaryEntry obj in m_items)
            if (((CModeValWithIndex)obj.Value).index == idx)
              return ((CModeValWithIndex)obj.Value).configItem;
        } else if (idx <= -2) {
          idx = -1 * (idx + 2);
          return (CModeVal)m_hidden_items[idx];
        }

        return null;
      }

      public IModeVal get_item_by_name(string name) {
        if (m_aliases != null && m_aliases.ContainsKey(name))
          name = (string)m_aliases[name];
        CModeValWithIndex ciwi = (CModeValWithIndex)m_items[name];
        if (ciwi == null)
          return null;
        return ciwi.configItem;
      }

      public IModeVal this[int idx]     { get { return get_item_by_index(idx); } }
      public IModeVal this[string name] { get { return get_item_by_name(name); } }

      public string get_name_by_index(int idx) {
        CModeVal item = (CModeVal)get_item_by_index(idx);
        return (item == null) ? null : item.name;
      }

      public string[] names {
        get {
          string[] result = new string[m_items.Count];
          foreach (DictionaryEntry obj in m_items) {
            CModeValWithIndex cii =  (CModeValWithIndex)obj.Value;
            result[cii.index] = cii.configItem.name;
          }
          return result;
        }        
      }

      public string[] texts {
        get {
          string[] result = new string[m_items.Count];
          foreach (DictionaryEntry obj in m_items) {
            CModeValWithIndex cii =  (CModeValWithIndex)obj.Value;
            result[cii.index] = cii.configItem.text;
          }
          return result;
        }        
      }


      public string get_text_by_index(int idx) {
        CModeVal item = (CModeVal)get_item_by_index(idx);
        if (item == null)
          return null;
        if (item.parent_item != null) {
          CModeVal parent_item = (CModeVal)get_item_by_name(item.parent_item);
          if (parent_item != null)
            return parent_item.text;
        }
        return item.text;
      }

      public int get_index_of_item_enabled_in_registry(RegistryKey regkey) {
        foreach (DictionaryEntry obj in m_items) {
          CModeValWithIndex cii =  (CModeValWithIndex)obj.Value;
          if (cii.configItem.is_enabled_in_registry(regkey))
            return cii.index;
        }
        for(int i=0, e=m_hidden_items.Count; i < e; ++i) {
          if (((CModeVal)m_hidden_items[i]).is_enabled_in_registry(regkey))
            return (-1 * (i+2));
        }
        return -1;
      }

      public bool enable_item_in_registry(RegistryKey regkey, int idx) {
        if (get_index_of_item_enabled_in_registry(regkey) == idx)
          return false; // already enabled
        this[idx].enable_in_registry(regkey);
        return true;
      }

      public string[][] hmr_format() {
        string [][] result = new string[m_items.Count + 1][];
        result[0] = new string[] { m_name.Replace("_Values", ""), null };
        for (int i=0; i < m_items.Count; ++i) {
          result[i+1] = get_item_by_index(i).hmr_format();
        }
        return result;
      }



    }

 		#region FILEPARSER
    /// <summary>
    /// file parser as nested class
    /// implementation note: we wait for making a new CMode objects until
    /// all related items are parsed, so we know the number of values.
    /// </summary>
    class ConfigParser {
      #region Private Data

      config_stream         cs                    = null;
      ArrayList             current_items         = new ArrayList(200);
      ConfigRecord          m_cfg;
      ConfigRecord.EMode     current_label         = ConfigRecord.EMode.NONE;
      string                current_label_string  = "";
      int                   current_label_idx     = -1;
      Hashtable             current_gui_props     = null;
      bool                  append_mode           = false;
      Hashtable             dict                  = new Hashtable();          // translating stadard words like Yes, No, On, Off
      Hashtable             m_macros              = new Hashtable();          // preprocessor macros
      Hashtable             m_aliases             = new Hashtable();          // alias => name pairs for a single label block

        #region RegExp
      // Record naming
      readonly Regex reg_label  = new Regex(@"^(?<append>\[\+\])?\[(?<name>[a-zA-Z0-9_]+)\]");
      readonly Regex reg_id     = new Regex("\\bid=\"([^\"]*)\"");

      // Item line patterns (different syntaxes)
      readonly Regex reg_name      = new Regex("\\bname=\"([^\":]+)\"");
      readonly Regex reg_alias     = new Regex("\\balias=\"([^\":]+)\"");
      readonly Regex reg_parent    = new Regex("\\bparent=\"([^\":]+)\"");
      readonly Regex reg_regval    = new Regex("\\bregval=\"([^\"]+)\"");
      readonly Regex reg_type      = new Regex("\\btype=([A-Z_]+)");
      readonly Regex reg_regdata   = new Regex("\\bvalue=\"([^\"]+)\"");
      readonly Regex reg_regval_2  = new Regex("\\bregval_[234]=\"([^\"]+)\"");
      readonly Regex reg_type_2    = new Regex("\\btype_[234]=([A-Z_]+)");
      readonly Regex reg_regdata_2 = new Regex(@"\bvalue_[234]=""([^""]+)""");
      readonly Regex reg_rv        = new Regex(@"\brv\(""([^""]+)""\s*[,=]\s*""?([^""]+)""?\s*(\|\s*default)?\)"); // new rv() syntax
      readonly Regex reg_rv2       = new Regex(@"\brv\((?:""(?<value>[^""]+)""\s*(?<assign>:)?[=]\s*(?:(?:dword:(?<dword>[0-9a-fA-F]{1,8})(?:#(?<dword_msk>[0-9a-fA-F]{1,8}))?)|(?:hex:(?<hex>[0-9a-fA-F]{2}(?:,[0-9a-fA-F]{2})*)(?:#(?<hex_msk>[0-9a-fA-F]{2}(?:,[0-9a-fA-F]{2})*))?)|(?<undef>undef:\w*)|(?:""(?<string>[^""]*)"")))\)");
      readonly Regex reg_text      = new Regex(@"\btext(\.(?<lang>[a-z]{2}))?=""(?<text>[^""]+)""");
      readonly Regex reg_flags     = new Regex(@"\bflags=\((?<flags>[^\]]+)\)");

      // GUI modification
      readonly Regex reg_gui_label      = new Regex(@"\b(?<key>gui_label(?:\.[a-z]{2})?)=""(?<data>[^""]*)""");
      readonly Regex reg_gui_tooltip    = new Regex(@"\b(?<key>gui_tooltip(?:\.[a-z]{2})?)=""(?<data>[^""]*)""");
      readonly Regex reg_gui_width_mult = new Regex("\\bgui_width_mult=(\\d)");

      // Control commands
      readonly Regex reg_inc         = new Regex("^include[=(]\"(.*)\"\\)?");
      readonly Regex reg_if          = new Regex(@"^if(\..*)?\((.*)\)$");
      readonly Regex reg_elif        = new Regex(@"^if(\..*)?\((.*)\)$");
      readonly Regex reg_else        = new Regex(@"^else(\..*)?\b");
      readonly Regex reg_endif       = new Regex(@"^endif(\..*)?\b");
      readonly Regex reg_define      = new Regex(@"^define\s+([A-Za-z0-9_]+)\s*(.*)\s*$");
      readonly Regex reg_define_test = new Regex(@"^define_test\s+([A-Za-z0-9_]+)\s*(.*)\s*$");
      readonly Regex reg_rv_test     = new Regex(@"\brv\(""(?<value>[^""]+)""\s*(?<not>!)?[=]\s*(?<type>REG_BINARY|REG_DWORD|REG_SZ)");

      #endregion

        #region Tables

      // parse registry values
      string[] m_file_label_names
        = { "d3d_fsaa_mode_values",
            "ogl_fsaa_mode_values",
            "d3d_aniso_mode_values",
            "ogl_aniso_mode_values",
            "d3d_vsync_mode_values",
            "ogl_vsync_mode_values",
            "d3d_qualityenhancements_values",
            "ogl_qualityenhancements_values",
            "d3d_texture_lod_bias_values",
            "ogl_texture_lod_bias_values",
            "d3d_prerender_frames_values",
            "ogl_prerender_frames_values",
            "d3d_extra_1_values",
            "d3d_extra_2_values",
            "d3d_extra_3_values",
            "d3d_extra_4_values",
            "d3d_extra_5_values",
            "d3d_extra_6_values",
            "d3d_extra_7_values",
            "d3d_extra_8_values",
            "d3d_extra_9_values",
            "d3d_extra_10_values",
            "ogl_extra_1_values",
            "ogl_extra_2_values",
            "ogl_extra_3_values",
            "ogl_extra_4_values",
            "ogl_extra_5_values",
            "ogl_extra_6_values",
            "ogl_extra_7_values",
            "ogl_extra_8_values",
            "ogl_extra_9_values",
            "ogl_extra_10_values",

            "d3d_extra2_1_values",
            "d3d_extra2_2_values",
            "d3d_extra2_3_values",
            "d3d_extra2_4_values",
            "d3d_extra2_5_values",
            "d3d_extra2_6_values",
            "d3d_extra2_7_values",
            "d3d_extra2_8_values",
            "d3d_extra2_9_values",
            "d3d_extra2_10_values",
            "ogl_extra2_1_values",
            "ogl_extra2_2_values",
            "ogl_extra2_3_values",
            "ogl_extra2_4_values",
            "ogl_extra2_5_values",
            "ogl_extra2_6_values",
            "ogl_extra2_7_values",
            "ogl_extra2_8_values",
            "ogl_extra2_9_values",
            "ogl_extra2_10_values",

      };

      ConfigRecord.EMode[] config_label_ids 
        = { ConfigRecord.EMode.D3D_FSAA, ConfigRecord.EMode.OGL_FSAA,
            ConfigRecord.EMode.D3D_ANISO, ConfigRecord.EMode.OGL_ANISO,
            ConfigRecord.EMode.D3D_VSYNC, ConfigRecord.EMode.OGL_VSYNC,
            ConfigRecord.EMode.D3D_QE, ConfigRecord.EMode.OGL_QE,
            ConfigRecord.EMode.D3D_LOD, ConfigRecord.EMode.OGL_LOD,
            ConfigRecord.EMode.D3D_PRE, ConfigRecord.EMode.OGL_PRE,
            ConfigRecord.EMode.D3D_EXTRA_1, ConfigRecord.EMode.D3D_EXTRA_2, ConfigRecord.EMode.D3D_EXTRA_3, ConfigRecord.EMode.D3D_EXTRA_4, 
            ConfigRecord.EMode.D3D_EXTRA_5, ConfigRecord.EMode.D3D_EXTRA_6, ConfigRecord.EMode.D3D_EXTRA_7, ConfigRecord.EMode.D3D_EXTRA_8,
            ConfigRecord.EMode.D3D_EXTRA_9, ConfigRecord.EMode.D3D_EXTRA_10, 
            ConfigRecord.EMode.OGL_EXTRA_1, ConfigRecord.EMode.OGL_EXTRA_2, ConfigRecord.EMode.OGL_EXTRA_3, ConfigRecord.EMode.OGL_EXTRA_4, 
            ConfigRecord.EMode.OGL_EXTRA_5, ConfigRecord.EMode.OGL_EXTRA_6, ConfigRecord.EMode.OGL_EXTRA_7, ConfigRecord.EMode.OGL_EXTRA_8,
            ConfigRecord.EMode.OGL_EXTRA_9, ConfigRecord.EMode.OGL_EXTRA_10,

            ConfigRecord.EMode.D3D_EXTRA2_1, ConfigRecord.EMode.D3D_EXTRA2_2, ConfigRecord.EMode.D3D_EXTRA2_3, ConfigRecord.EMode.D3D_EXTRA2_4, 
            ConfigRecord.EMode.D3D_EXTRA2_5, ConfigRecord.EMode.D3D_EXTRA2_6, ConfigRecord.EMode.D3D_EXTRA2_7, ConfigRecord.EMode.D3D_EXTRA2_8,
            ConfigRecord.EMode.D3D_EXTRA2_9, ConfigRecord.EMode.D3D_EXTRA2_10, 
            ConfigRecord.EMode.OGL_EXTRA2_1, ConfigRecord.EMode.OGL_EXTRA2_2, ConfigRecord.EMode.OGL_EXTRA2_3, ConfigRecord.EMode.OGL_EXTRA2_4, 
            ConfigRecord.EMode.OGL_EXTRA2_5, ConfigRecord.EMode.OGL_EXTRA2_6, ConfigRecord.EMode.OGL_EXTRA2_7, ConfigRecord.EMode.OGL_EXTRA2_8,
        ConfigRecord.EMode.OGL_EXTRA2_9, ConfigRecord.EMode.OGL_EXTRA2_10, 
 
      };

      int[] config_label_idx = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11,
                                 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
                                 22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
                                 32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
                                 42, 43, 44, 45, 46, 47, 48, 49, 50, 51,

      };
      #endregion Tables

      #endregion

      #region "If" Statement
      class if_block {
        public string label;
        public bool p;
        public bool p_inherited = true;
        public bool else_reached_p;
        public bool true_block_passed_p;
        public if_block() { label = null; else_reached_p = false; true_block_passed_p = false; }
      
      };

      // define operator functions for if statement

      bool ifop_cmp_eq(string lhs, string rhs) { return lhs == rhs; }
      bool ifop_cmp_ne(string lhs, string rhs) { return lhs != rhs; }
      bool ifop_cmp_lt(string lhs, string rhs) {
        if (lhs == rhs)
          return false;
        string[] lhs_split = lhs.Split('.');
        string[] rhs_split = rhs.Split('.');
        for (int i=0; i < lhs_split.Length && i < rhs_split.Length; ++i) {
          if (int.Parse(lhs_split[i]) > int.Parse(rhs_split[i]))
            return false;
          if (int.Parse(lhs_split[i]) < int.Parse(rhs_split[i]))
            return true;
        }
        return false;
      }
      bool ifop_cmp_gt(string lhs, string rhs) {
        if (lhs == rhs)
          return false;
        string[] lhs_split = lhs.Split('.');
        string[] rhs_split = rhs.Split('.');
        for (int i=0; i < lhs_split.Length && i < rhs_split.Length; ++i) {
          if (int.Parse(lhs_split[i]) < int.Parse(rhs_split[i]))
            return false;
          if (int.Parse(lhs_split[i]) > int.Parse(rhs_split[i]))
            return true;
        }
        return false;
      }
      bool ifop_cmp_lteq(string lhs, string rhs) {
        return ifop_cmp_eq(lhs, rhs) || ifop_cmp_lt(lhs, rhs);
      }
      bool ifop_cmp_gteq(string lhs, string rhs) {
        return ifop_cmp_eq(lhs, rhs) || ifop_cmp_gt(lhs, rhs);
      }

      delegate bool if_comparison(string lhs, string rhs); // pointer to operation
      /// <summary>
      /// parse if statement comparison operator. returns the delegate that operation.  
      /// </summary>
      if_comparison parse_if_op(string op) {
        if (op == "==" || op == "=")
          return new if_comparison(ifop_cmp_eq);
        if (op == "!=" || op == "<>")
          return new if_comparison(ifop_cmp_ne);
        if (op == ">")
          return new if_comparison(ifop_cmp_gt);
        if (op == "<")
          return new if_comparison(ifop_cmp_lt);
        if (op == ">=")
          return new if_comparison(ifop_cmp_gteq);
        if (op == "<=")
          return new if_comparison(ifop_cmp_lteq);

        cs.throw_parse_error("invalid operator \"" + op + "\" in if or elif statement");
        return null; // never reached
      }

      private bool test_if_condition(string cond) {
        Regex reg_cond = new Regex(@"\s*(?<lhs>\$?[a-zA-Z_0-9]+\$?)\s*(?<op>[!<>=]{1,2})?\s*(?:(?:(?:""(?<rhs_string>[^\""]*)"")|(?<rhs_misc>[0-9.a-z]+)))?\s*(?<and_or>[&|]{0,2})");
        MatchCollection mat_conds = reg_cond.Matches(cond);
        bool result = true;
        bool and_run = false;

        foreach (Match m in mat_conds) {
          string and_or = m.Groups["and_or"].Value;
          string lhs = m.Groups["lhs"].Value;
          string op = m.Groups["op"].Value;
          string rhs = (m.Groups["rhs_misc"].Success ? m.Groups["rhs_misc"].Value 
            : (m.Groups["rhs_string"].Success ? m.Groups["rhs_string"].Value
            : null));

          bool res = test_if_condition(lhs, op, rhs);

          if (and_run) {
            result = result && res;
          } else {
            result = res;
          }
          
          and_run = (and_or == "&&");


          if (and_or == "||" && res && result)
            return true;
        }
        return result;
      }
      private bool test_if_condition(string lhs, string op, string rhs) {
        if (lhs.StartsWith("$") && lhs.EndsWith("$"))
          lhs = (string)m_macros[lhs.Substring(1, lhs.Length - 2)];
        if (lhs == "true")
          return true;
        if (lhs == "false")
          return false;
        if (lhs == "Win32NT")
          return (System.Environment.OSVersion.Platform == System.PlatformID.Win32NT);

        if (lhs == "driver_version" || lhs == "dv") {
          if_comparison ifop_cmp = parse_if_op(op);
          return ifop_cmp(G.di.get_driver_version(), rhs);
        }

        if (lhs == "driver_short_version" || lhs == "dsv") {
          if_comparison ifop_cmp = parse_if_op(op);
          string dv = G.di.get_driver_version();
          int last_point = dv.LastIndexOf(".");
          if (last_point > -1) dv = dv.Substring(last_point + 1);
          return ifop_cmp(dv, rhs);
        }
        if (lhs == "lang") {
          if_comparison ifop_cmp = parse_if_op(op);
          return ifop_cmp(System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, rhs);
        }
        if (lhs == "spec_name") {
          if_comparison ifop_cmp = parse_if_op(op);
          bool result = ifop_cmp(G.spec_name, rhs);
          return result;
        }

        return false;
      }

#endregion "If" Statement

      class config_stream {
        public readonly string       file_name;
        public readonly StreamReader sr;
        public int                   current_line_number = 0; // current line number
        public string                current_line = ""; // current line

        public config_stream(string fileName) {
          file_name = fileName;
          System.Text.Encoding enc = System.Text.Encoding.ASCII; // default;
          try { 
            enc =  System.Text.Encoding.GetEncoding("iso-8859-1"); //TODO: make charset configurable or use unicode in files
          } catch {
          }

          sr = new StreamReader (fileName, enc);
        }
        public string read_line_skip_comments() {
          while (true) {
            string line = read_line();
            if (line == null || !is_comment)
              return line;
          }
        }

        public string read_line() {
          current_line = null;

          while (true) {

            string line = sr.ReadLine();
            current_line = null;

            if (line == null)
              return current_line;
            if (current_line == null)
              current_line = "";

            ++current_line_number;

            if (line.EndsWith(@" \")) {  //FIXME: the space allows lines (e.g. defines) ending with backslash
               current_line += line.Substring(0, line.Length - 1).Trim();
               continue;
            }

            current_line += line.Trim();
            return current_line;
          }
        }


        public bool is_comment { get { return (current_line.StartsWith(";") || current_line == ""); } }
        public void throw_parse_error(string msg) {
          throw new ParseError(msg
            +"\r\n"
            +"\r\nFile Name:\t" + file_name
            +"\r\nLine Number:\t" + current_line_number.ToString()
            +"\r\nLine Content:\t" + current_line); }
      };

      // constructor starts end ends all parsing
      public ConfigParser(string file_name, ConfigRecord config) {
        bool error = false;
        //string err_msg = null;
        m_cfg = config;
        Stack srs = new Stack(); // Stack of StreamReaders required by include command
        Stack ifs = new Stack(); // Stack of if statements (???-bw: What about having labels for if/else/endif to validate correct nesting?)
        dict.Add("On",  new string[] {"de.Ein"});
        dict.Add("Off", new string[] {"de.Aus"});
        dict.Add("Yes", new string[] {"de.Ja"});
        dict.Add("No",  new string[] {"de.Nein"});
        dict.Add("App", new string[] {"de.Anw"});

        try {
          string text;

          Match mat_name, mat_parent, mat_flags, mat_regval, mat_regdata, mat_type,
            mat_label, mat_id,
            mat_gui_label, mat_gui_tooltip, mat_gui_width_mult,
            mat_inc, mat_if, mat_else, mat_endif, mat_elif, mat_define, mat_define_test;

          Util.CheckSum chs = new Util.CheckSum();

          for (srs.Push(new config_stream (file_name)); srs.Count > 0; srs.Pop()) {
            cs=(config_stream)srs.Peek();

            while ((text = cs.read_line_skip_comments()) != null) {

              // accumulate a checksum for this file
              chs.push_bytes(text);

              #region CONDITIONAL SCANNING

              if ((mat_if = reg_if.Match(text)).Success) {  // "if" statement
                if_block ib = new if_block();
                ib.label = mat_if.Groups[1].Value;
                string cond = mat_if.Groups[2].Value;
                ib.true_block_passed_p = ib.p = test_if_condition(cond);

                if (ifs.Count > 0 
                  && (!((if_block)ifs.Peek()).p || !((if_block)ifs.Peek()).p_inherited)
                  )
                  ib.p_inherited = false;

                ifs.Push(ib);

                continue; // line done
                     
              } else if ((mat_elif = reg_elif.Match(text)).Success) { // elif statement
                if_block ib = (if_block)ifs.Peek();
                string elif_label = mat_elif.Groups[1].Value;
                if (ib.label != elif_label)
                  cs.throw_parse_error("Non matching if / elif labels (if" + ib.label + " / elif" + elif_label + ")");
                if (ib.else_reached_p)
                  cs.throw_parse_error("using elif after else else is not allowed"
                    + ((elif_label == null) ? "" : " (elif." + elif_label + ")"));

                if (ib.true_block_passed_p) {
                  ib.p = false;
                  continue; // there was already a true condition in this "if" block.
                }

                string cond = mat_if.Groups[2].Value;

                if (test_if_condition(cond)) {
                  ib.true_block_passed_p = ib.p = true;
                }

                continue; // line done

              } else if ((mat_endif = reg_endif.Match(text)).Success) { // endif statement
                if_block ib = (if_block)ifs.Pop();
                string endif_label = mat_endif.Groups[1].Value;
                if (ib.label != endif_label)
                  cs.throw_parse_error("Non matching if / endif label");

                continue; // line done
                           
              } else if ((mat_else = reg_else.Match(text)).Success) { // else statement
                if_block ib = (if_block)ifs.Peek();
                string else_label = mat_else.Groups[1].Value;
                if (ib.label != else_label)
                  cs.throw_parse_error("Non matching if / else labels (if" + ib.label + " / else" + else_label + ")");
 
                ib.p ^= true;

                if (ib.p) {
                  ib.true_block_passed_p = true;
                }

                continue; // line done

              }
              
              // skip false "if" blocks 
              if (ifs.Count > 0 && ( !((if_block)ifs.Peek()).p || ! ((if_block)ifs.Peek()).p_inherited))
                continue;
              #endregion CONDITIONAL SCANNING

              // macro defining
              if((mat_define = reg_define.Match(text)).Success) {
                m_macros[mat_define.Groups[1].Value] = mat_define.Groups[2].Value;
                continue;
              }
                
              // test-macro defining
              if((mat_define_test = reg_define_test.Match(text)).Success) {
                bool result = false;
                string lhs = mat_define_test.Groups[1].Value;
                string rhs = mat_define_test.Groups[2].Value;
                if ((mat_regval = reg_rv_test.Match(text)).Success) {
                  string val_name = mat_regval.Groups["value"].Value; 
                  string dat_type = mat_regval.Groups["type"].Value;
                  string key = m_cfg.driver_regkey;

                  // expand preprocessor macros //FIXME: move code upwards
                  if (val_name.IndexOf("$") != -1) {
                      foreach (DictionaryEntry den in m_macros)
                          val_name = val_name.Replace("$" + (string)den.Key + "$", (string)den.Value);
                  }

 
                    
                    string val = val_name;

                  int sepIdx = val_name.LastIndexOf(@"\");
                  if (sepIdx != -1) {
                      key += @"\" + val_name.Substring(0, sepIdx);
                      val = val_name.Substring(sepIdx + 1);
                  }

                   
                  using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(key)) {
                    object obj = regkey.GetValue(val);
                    System.Type obj_type = obj.GetType();
                    if (dat_type == "REG_BINARY" && obj_type == (CModeVal.type_reg_binary))
                      result = true;
                    else if (dat_type == "REG_SZ" && obj_type == (CModeVal.type_reg_sz))
                      result = true;
                    else if (dat_type == "REG_DWORD" && obj_type == (CModeVal.type_reg_dword))
                      result = true;
                  }
                }
                m_macros[lhs] = result.ToString().ToLower();
                continue;
              }


              // include command
              if ((mat_inc = reg_inc.Match(text)).Success && srs.Count < 50 /* avoid recursive includes */) {
                file_name = mat_inc.Groups[1].Value;
                srs.Push(cs = new config_stream(file_name));

                continue;
              }


              // Label
              if ((mat_label = reg_label.Match(text)).Success) {
                append_mode = mat_label.Groups["append"].Success; 
                string res_label = mat_label.Groups["name"].ToString();
                interpret_new_label(res_label);
                current_gui_props = new Hashtable(10);

                continue;
              }

              mat_name   = reg_name.Match(text);
              mat_parent = reg_parent.Match(text);
              mat_flags  = reg_flags.Match(text);
              if (mat_name.Success) {
                bool is_hidden = false;

                if (mat_flags.Success) {
                  is_hidden = mat_flags.Groups["flags"].Value.IndexOf("hidden") != -1;
                }
                // Item line with new rv() syntax, strict syntax
                string name = mat_name.Groups[1].Value;
                string parent = (mat_parent.Success ? mat_parent.Groups[1].Value : null);
                MatchCollection mats_text = reg_text.Matches(text);
                MatchCollection mat_rv2 = reg_rv2.Matches(text);
                MatchCollection mats_alias = reg_alias.Matches(text);

                // Naming
                Hashtable hash = new Hashtable(mats_text.Count);
                string[] at_texts = (string[])dict[name];
                if (at_texts != null) {
                  foreach (string s in at_texts) {
                    hash[s.Substring(0,2)] = s.Substring(3);
                  }
                }

                if (mats_text.Count > 0) {
                  foreach(Match mat_text in mats_text) {
                    string lang = (mat_text.Groups["lang"].Success) ? mat_text.Groups["lang"].Value : "default";
                    hash[lang] = mat_text.Groups["text"].Value;
                  }
                }
                if (hash.Count > 0) {
                  interpret_new_item(name, parent, null, null, null, null, false, hash, is_hidden, false);
                  name = null;
                }

                if (mats_alias.Count > 0) {
                  foreach(Match mat_alias in mats_alias) {
                    m_aliases[mat_alias.Groups[1].Value] = name;
                  }
                }
                
                // Registry Values
                if (mat_rv2.Count > 0) {
                  foreach (Match m in mat_rv2) {
                    string rv = null, rd = null, rt = null, rm = null;

                    rv = m.Groups["value"].Value;

                    if (m.Groups["string"].Success) {
                      rd = m.Groups["string"].Value;
                      rt = "REG_SZ";
                    } else if (m.Groups["dword"].Success) {
                      rd = m.Groups["dword"].Value;
                      rt = "REG_DWORD";
                      rm = (m.Groups["dword_msk"].Success) ? m.Groups["dword_msk"].Value : null;
                    } else if (m.Groups["hex"].Success) {
                      rd = m.Groups["hex"].Value;
                      rt = "REG_BINARY";
                      rm = (m.Groups["hex_msk"].Success) ? m.Groups["hex_msk"].Value : null;
                    } else if (m.Groups["undef"].Success) {
                      rd = m.Groups["undef"].Value;
                      rt = "REG_NOT_EXIST";
                    }
                  #region OBSOLETE
#if OBSOLETE
                    /// Support old quoted syntax
                    if (rt == "REG_SZ") {
                      if (rd.StartsWith("dword:")) {
                        rd = rd.Substring(6);
                        rt = "REG_DWORD";                    
                      } else if (rd.StartsWith("hex:")) {
                        rd = rd.Substring(4);
                        rt = "REG_BINARY";
                      } else if (rd.StartsWith("undef:")) {
                        rd = rd.Substring(7);
                        rt = "REG_NOT_EXIST";                    
                      }
                    }
#endif
                  #endregion
                
                    bool is_wrtonly = false;
                    if (m.Groups["assign"].Success)
                      is_wrtonly = true;

                    if (!interpret_new_item(name, parent, rv, rt, rd, rm, false, null, is_hidden, is_wrtonly))
                      break;
                    name = null; // if null for subsequent registry values
                  } // foreach in mat_rv2


                  continue;
                }
              


              #region OBSOLETE  
#if OBSOLETE
                // Item line with new rv() syntax
                //mat_name = reg_name.Match(text);
                MatchCollection mat_rv = reg_rv.Matches(text);
                       
                if (mat_name.Success && mat_rv.Count > 0) {
                  foreach (Match m in mat_rv) {
                    string regval = m.Groups[1].Value;
                    string regdata = m.Groups[2].Value;
                    bool default_value = m.Groups[3].Success;

                    if (!interpret_new_item(name, parent, regval, null, regdata, null, default_value, null, is_hidden, false))
                      break;
                    name = null;
                  }

                  continue;
                }
#endif 
              #endregion
                // Item line with old syntax
                if (mat_name.Success
                  && (mat_regval = reg_regval.Match(text)).Success
                  && (mat_regdata = reg_regdata.Match(text)).Success) {
                  string res_name = mat_name.Groups[1].ToString();
                  string res_value = mat_regdata.Groups[1].ToString();
                  string res_regval = mat_regval.Groups[1].ToString();
                  mat_type = reg_type.Match(text);
                  string res_type = mat_type.Groups[1].ToString();
                  if (!interpret_new_item(res_name, parent, res_regval, res_type, res_value, null, false, null, is_hidden, false))
                    error = true;
                  if (!error) {
                    mat_regval = reg_regval_2.Match(text);
                    mat_regdata = reg_regdata_2.Match(text);
                    mat_type = reg_type_2.Match(text);
                    while (mat_regval.Success && mat_regdata.Success && !error) {
                      res_value = mat_regdata.Groups[1].ToString();
                      res_regval = mat_regval.Groups[1].ToString();
                      res_type = mat_type.Groups[1].ToString();
                      if (!interpret_new_item(null, parent, res_regval, res_type, res_value, null, false, null, is_hidden, false))
                        error = true;

                      mat_regval.NextMatch();
                      mat_regdata.NextMatch();
                      mat_type.NextMatch();
                    }
                  }

                  continue;
                }
              }

              // tags not related to a special line

              if ((mat_gui_label = reg_gui_label.Match(text)).Success)
                current_gui_props[mat_gui_label.Groups["key"].Value] = mat_gui_label.Groups["data"].Value;

              if ((mat_gui_tooltip = reg_gui_tooltip.Match(text)).Success)
                current_gui_props[mat_gui_tooltip.Groups["key"].Value] = mat_gui_tooltip.Groups["data"].Value.Replace(@"\n", "\r\n");

              if ((mat_gui_width_mult = reg_gui_width_mult.Match(text)).Success)
                current_gui_props["gui_width_mult"] = int.Parse(mat_gui_width_mult.Groups[1].Value);

              if ((mat_id = reg_id.Match(text)).Success)
                for (int i=0; i < G.cr_modes.Length; ++i)
                  if (G.cr_modes[i] == current_label) {
                    Profiles.GameProfileData.rename_option(G.gp_parms[i], mat_id.Groups[1].Value);
                    current_label_string = mat_id.Groups[1].Value + "_Values"; //TODO:XXX
                  }

            } // while text
            m_cfg.specfile_checksum = chs.get_checksum();
            interpret_new_label(null);
          } // while srs

        } // catch
        finally {
          if (cs != null && cs.sr != null) cs.sr.Close();
        }
      }
      private bool append_mode_previous = false;
      /// <summary>
      /// Do all pending actions before switching to a new storage object (Label) or EOF.
      /// </summary>
      private void interpret_commit() {
        int idx = -1;
        idx = (current_label_idx == -1) ? -1 : idx = config_label_idx[current_label_idx];

        if (0 <= idx) {
          if (!append_mode_previous) {
            m_cfg.m_cfg_store[idx] = new CMode(current_label_string, current_items.Count);
            m_cfg.m_cfg_store[idx].append_items(current_items);           
          } else {
            foreach(object cit in current_items) {
              CModeVal new_cit = (CModeVal)cit;
              CMode cits = m_cfg.m_cfg_store[idx];
              CModeVal old_cit = (CModeVal)cits.get_item_by_name(new_cit.name);
              if (old_cit == null) {
                if (new_cit.is_usable())
                  cits.append_item(new_cit);
              } else {
                old_cit.retrofit(new_cit);
              }
            }
 
          }

          if (m_aliases.Count > 0) {
            foreach(DictionaryEntry den in m_aliases)
              m_cfg.m_cfg_store[idx].add_alias((string)den.Value, (string)den.Key);
            m_aliases.Clear();
          }

        }

        if (current_gui_props != null) {
          foreach(DictionaryEntry den in current_gui_props)
            m_cfg.m_cfg_store[idx].add_gui_property((string)den.Key, den.Value);
          current_gui_props = null;
        }

        current_items.Clear();
        current_label = ConfigRecord.EMode.NONE;
        current_label_idx = -1;
        append_mode_previous = append_mode;
      }

      /// <summary>
      /// Starts a new data storage object
      /// </summary>
      /// <param name="label"></param>
      /// <returns></returns>
      private bool interpret_new_label(string label) {
        interpret_commit();
        if (label == null)
          return true;

        string label_lowercase = label.ToLower();

        for (int i=0; i < m_file_label_names.Length; ++i) {
          if (label_lowercase == m_file_label_names[i]) {
            current_label = config_label_ids[i];
            current_label_string = label;
            current_label_idx = i;
          }
        }

        return current_label_idx != -1; 
      }

      /// <summary>
      /// Store parsed data
      /// </summary>
      /// <param name="name">If non-null make new object with this name, otherwise append to current item object</param>
      /// <param name="rval">data</param>
      /// <param name="type">data</param>
      /// <param name="regdata_string">data</param>
      /// <returns></returns>
      private bool interpret_new_item(string name, string parent, string rval, string type, string regdata_string, string mask, bool def, Hashtable hash, bool is_hidden, bool is_wrtonly) {
        CModeVal cit; 
        // create new item if needed
        if (name != null) {
          cit = new CModeVal(name, is_hidden);
          cit.parent_item = parent;
          current_items.Add(cit);
        } else {
          cit = (CModeVal)current_items[current_items.Count - 1];
        }

        // expand preprocessor macros
        if (rval != null && rval.IndexOf("$") != -1) {
          foreach(DictionaryEntry den in m_macros)
            rval = rval.Replace("$" + (string)den.Key + "$", (string)den.Value);   
        }

      
        // add registry value to to current config item
        if (rval != null) {
          if (!cit.add(rval, type, regdata_string, mask, def, is_wrtonly)) {
            if (name != null)
              current_items.Remove(cit);   
            return false;
          }
        }

        // add text to current config item
        if (hash != null)
          foreach(DictionaryEntry den in hash)
            cit.add_text((string)den.Key, (string)den.Value);

        return true;

      }

    }

		#endregion FILEPARSER
  #endregion
  }

}
