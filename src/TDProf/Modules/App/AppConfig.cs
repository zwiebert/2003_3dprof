using System;
using System.IO;
using System.Text;
using ArrayList=System.Collections.ArrayList;
using System.Text.RegularExpressions;
using System.IO.IsolatedStorage;
using System.Collections;
using System.Windows.Forms;
using EClockKinds=TDProf.Profiles.GameProfileData.EClockKinds;

namespace TDProf.App {

  /// <summary>
  /// Class for storing application settings permanently.
  /// 
  /// Add new options here. Append your new option to all of Opts, names and vals.
  ///  Try to group the options by topic, too keep this code maintainable and the
  ///   configfile readable..
  ///  A value in vals will be used as default value, if no option was defined in
  ///  config file.
  /// </summary>
  public class AppConfig {
    #region Option Definitions, Options Properties. Add New Options Here

    #region Option Definitions + Default values
    public enum Opts { 
      DAEMON_EXE, RUN_AND_QUIT, GUI_MOVER_FEEDBACK, GUI_SHOW_TOOLTIPS, GUI_FILTER_BY_SPECNAME, GUI_MENU_ICONS, GUI_3D_CHECKBOXES, GUI_SHOW_TRAY_ICON, GUI_HIDE_ON_CLOSEBOX, GUI_COMPACT, GUI_SIZE_CLOCKER_SPLIT,
      APP_LANG, APP_VERSION, CFG_GUID,
      REG_READONLY, WARN_SAFE_MODE, RUN_AUTO_RESTORE, RUNGUI_AUTO_RESTORE, AUTO_RESTORE_TO_DEFAULT_PROF,
      SF_CHECKSUM,
      TIMER_UPDATE_CR,
      PROF_LAST_SELECTED, PROF_DEFAULT, PROF_QUALITY, PROF_PERFORMANCE,
      SL_NAME_PREFIX, SL_NAME_SUFFIX,
      COMMANDS_PRE_EXE_RUN, COMMANDS_POST_EXE_EXIT,
      FEATURE_CLOCKING, EXP_RADEON_SMALL_CLOCKSTEPS, EXP_CLOCKING_DLL, CLOCKING_RESTORE_KIND,
      HOTKEY_MINIMIZE_TRAY_TOGGLE, HOTKEY_GUI_FOCUS, HOTKEY_CLOCKING_PRE_SLOW, HOTKEY_CLOCKING_PRE_NORMAL, HOTKEY_CLOCKING_PRE_FAST, HOTKEY_CLOCKING_PRE_ULTRA,
    };

    static readonly string[] names 
      = { "daemon_exe", "run_and_quit", "gui_mover_feedback", "gui_show_tooltips", "filter_by_videocard", "gui_show_menu_icons", "gui_3d_checkboxes", "gui_show_tray_icon", "gui_hide_on_closebox", "gui_compact", "gui_size_clocker_split_ratio",
          "language", "app_version", "config_guid",
          "registry_readonly", "warn_about_safe_mode", "auto_restore", "auto_restore_gui", "auto_restore_to_default_profile",
          "specfile_checksum",
          "update_driver_timer_ms",
          "last_selected_profile", "default_profile", "quality_profile", "performance_profile",
          "shell_link_name_prefix", "shell_link_name_suffix",
          "commands_pre_exe_run", "commands_post_exe_exit", 
          "feature_clocking", "clocking_experimental_radeon_small_steps", "clocking_experimental_use_dll", "clocking_restore_kind",
          "hotkey_toggle_minimize_tray", "hotkey_gui_focus", "hotkey_clk_pre_slow", "hotkey_clk_pre_normal", "hotkey_clk_pre_fast", "hotkey_clk_pre_ultra", 
    };

    string[] vals
      = { "C:\\Programme\\D-Tools\\daemon.exe", "False", "True", "True", "False", "True", "True", "True", "False", "False", "50",
          "Auto", "0.0.0", "",
          "True", "True", "True", "True", "False",
          "0",
          "1500",
          "", "", "", "",
          "", " Profile",
          "", "",
          "True", "True", "False", EClockKinds.PARENT.ToString(),
          "0", "0", "0", "0", "0", "0",
    };

    #endregion

    #region Option Properties

    public Keys hotkey_clk_pre_slow {
      get { return (Keys)int.Parse(vals[(int)Opts.HOTKEY_CLOCKING_PRE_SLOW]); }
      set { vals[(int)Opts.HOTKEY_CLOCKING_PRE_SLOW] = ((int)value).ToString();; }
    }

    public Keys hotkey_clk_pre_normal {
      get { return (Keys)int.Parse(vals[(int)Opts.HOTKEY_CLOCKING_PRE_NORMAL]); }
      set { vals[(int)Opts.HOTKEY_CLOCKING_PRE_NORMAL] = ((int)value).ToString();; }
    }

    public Keys hotkey_clk_pre_fast {
      get { return (Keys)int.Parse(vals[(int)Opts.HOTKEY_CLOCKING_PRE_FAST]); }
      set { vals[(int)Opts.HOTKEY_CLOCKING_PRE_FAST] = ((int)value).ToString();; }
    }

    public Keys hotkey_clk_pre_ultra {
      get { return (Keys)int.Parse(vals[(int)Opts.HOTKEY_CLOCKING_PRE_ULTRA]); }
      set { vals[(int)Opts.HOTKEY_CLOCKING_PRE_ULTRA] = ((int)value).ToString();; }
    }

    public Keys hotkey_gui_focus {
      get { return (Keys)int.Parse(vals[(int)Opts.HOTKEY_GUI_FOCUS]); }
      set { vals[(int)Opts.HOTKEY_GUI_FOCUS] = ((int)value).ToString();; }
    }

    public Keys hotkey_minimize_tray_toggle {
      get { return (Keys)int.Parse(vals[(int)Opts.HOTKEY_MINIMIZE_TRAY_TOGGLE]); }
      set { vals[(int)Opts.HOTKEY_MINIMIZE_TRAY_TOGGLE] = ((int)value).ToString();; }
    }

    public string config_guid {
      get { return vals[(int)Opts.CFG_GUID]; }
      set { vals[(int)Opts.CFG_GUID] = value; }
    }

    public string app_version {
      get { return vals[(int)Opts.APP_VERSION]; }
    }

    public EClockKinds clocking_restore_kind {
      get { return (EClockKinds)EClockKinds.Parse(typeof(EClockKinds), vals[(int)Opts.CLOCKING_RESTORE_KIND], true); } 
      set { vals[(int)Opts.CLOCKING_RESTORE_KIND] = value.ToString(); }
    }

    public bool exp_radeon_small_clocksteps {
      get { return bool.Parse(vals[(int)Opts.EXP_RADEON_SMALL_CLOCKSTEPS]); }
      set { vals[(int)Opts.EXP_RADEON_SMALL_CLOCKSTEPS] = value.ToString();; }
    }

    public bool exp_clocking_dll {
      get { return bool.Parse(vals[(int)Opts.EXP_CLOCKING_DLL]); }
      set { vals[(int)Opts.EXP_CLOCKING_DLL] = value.ToString();; }
    }

    public bool gui_hide_on_closeBox {
      get { return bool.Parse(vals[(int)Opts.GUI_HIDE_ON_CLOSEBOX]); }
      set { vals[(int)Opts.GUI_HIDE_ON_CLOSEBOX] = value.ToString();; }
    }
    public bool gui_show_tray_icon {
      get { return bool.Parse(vals[(int)Opts.GUI_SHOW_TRAY_ICON]); }
      set { vals[(int)Opts.GUI_SHOW_TRAY_ICON] = value.ToString();; }
    }

    public bool gui_3d_checkboxes {
      get { return bool.Parse(vals[(int)Opts.GUI_3D_CHECKBOXES]); }
      set { vals[(int)Opts.GUI_3D_CHECKBOXES] = value.ToString();; }
    }

    public bool gui_show_menu_icons {
      get { return bool.Parse(vals[(int)Opts.GUI_MENU_ICONS]); }
      set { vals[(int)Opts.GUI_MENU_ICONS] = value.ToString();; }
    }

    public bool gui_compact {
      get { return bool.Parse(vals[(int)Opts.GUI_COMPACT]); }
      set { vals[(int)Opts.GUI_COMPACT] = value.ToString();; }
    }

    public bool feature_clocking {
      get { return bool.Parse(vals[(int)Opts.FEATURE_CLOCKING]); }
      set { vals[(int)Opts.FEATURE_CLOCKING] = value.ToString();; }
    }
    static readonly string LINE_SEPARATOR = ";-=NEWLINE=-;";
    public string command_pre_exe_run {
      get { return vals[(int)Opts.COMMANDS_PRE_EXE_RUN].Replace(LINE_SEPARATOR, Environment.NewLine).Replace("\\\"", "\""); }
      set { vals[(int)Opts.COMMANDS_PRE_EXE_RUN] = value.Replace(Environment.NewLine, LINE_SEPARATOR).Replace("\"", "\\\""); }
    }
    public string command_post_exe_exit {
      get { return vals[(int)Opts.COMMANDS_POST_EXE_EXIT]; }
      set { vals[(int)Opts.COMMANDS_POST_EXE_EXIT] = value; }
    }

    public string prof_last_selected {
      get { return vals[(int)Opts.PROF_LAST_SELECTED]; }
      set { vals[(int)Opts.PROF_LAST_SELECTED] = value; }
    }
    public string prof_default {
      get { return vals[(int)Opts.PROF_DEFAULT]; }
      set { vals[(int)Opts.PROF_DEFAULT] = value; }
    }
    public string prof_quality {
      get { return vals[(int)Opts.PROF_QUALITY]; }
      set { vals[(int)Opts.PROF_QUALITY] = value; }
    }
    public string prof_performance {
      get { return vals[(int)Opts.PROF_PERFORMANCE]; }
      set { vals[(int)Opts.PROF_PERFORMANCE] = value; }
    }
    public int cr_timer_update {
      get { return int.Parse(vals[(int)Opts.TIMER_UPDATE_CR]); }
      set { vals[(int)Opts.TIMER_UPDATE_CR] = value.ToString();; }
    }
    public bool gui_filter_by_spec_name {
      get { return bool.Parse(vals[(int)Opts.GUI_FILTER_BY_SPECNAME]); }
      set { vals[(int)Opts.GUI_FILTER_BY_SPECNAME] = value.ToString();; }
    }
    public string sl_name_prefix {
      get { return vals[(int)Opts.SL_NAME_PREFIX]; }
      set { vals[(int)Opts.SL_NAME_PREFIX] = value; }
    }
    public string sl_name_suffix {
      get { return vals[(int)Opts.SL_NAME_SUFFIX]; }
      set { vals[(int)Opts.SL_NAME_SUFFIX] = value; }
    }

    public int app_specfile_checksum {
      get { return int.Parse(vals[(int)Opts.SF_CHECKSUM]); }
      set { vals[(int)Opts.SF_CHECKSUM] = value.ToString();; }
    }
    public bool gui_mover_feedback {
      get { return bool.Parse(vals[(int)Opts.GUI_MOVER_FEEDBACK]); }
      set { vals[(int)Opts.GUI_MOVER_FEEDBACK] = value.ToString();; }
    }
    public bool gui_show_tooltips {
      get { return bool.Parse(vals[(int)Opts.GUI_SHOW_TOOLTIPS]); }
      set { vals[(int)Opts.GUI_SHOW_TOOLTIPS] = value.ToString();; }
    }

    public int gui_clock_tab_split_ratio {
      get { return int.Parse(vals[(int)Opts.GUI_SIZE_CLOCKER_SPLIT]); }
      set { vals[(int)Opts.GUI_SIZE_CLOCKER_SPLIT] = value.ToString();; }
    }

    public string img_daemon_exe_path {
      get { return vals[(int)Opts.DAEMON_EXE]; }
      set { vals[(int)Opts.DAEMON_EXE] = value; }
    }
    public bool run_and_quit {
      get { return bool.Parse(vals[(int)Opts.RUN_AND_QUIT]); }
      set { vals[(int)Opts.RUN_AND_QUIT] = value.ToString();; }
    }
    public string app_lang {
      get { return vals[(int)Opts.APP_LANG]; }
      set { vals[(int)Opts.APP_LANG] = value; }
    }
    public bool reg_readonly {
      get { return bool.Parse(vals[(int)Opts.REG_READONLY]); }
      set { vals[(int)Opts.REG_READONLY] = value.ToString();; }
    }
    public bool warn_about_safe_mode {
      get { return bool.Parse(vals[(int)Opts.WARN_SAFE_MODE]); }
      set { vals[(int)Opts.WARN_SAFE_MODE] = value.ToString();; }
    }
    public bool auto_restore_after_exit {
      get { return bool.Parse(vals[(int)Opts.RUN_AUTO_RESTORE]); }
      set { vals[(int)Opts.RUN_AUTO_RESTORE] = value.ToString();; }
    }
    public bool auto_restore_after_exit_in_gui {
      get { return bool.Parse(vals[(int)Opts.RUNGUI_AUTO_RESTORE]); }
      set { vals[(int)Opts.RUNGUI_AUTO_RESTORE] = value.ToString();; }
    }
    public bool auto_restore_to_default_profile {
      get { return bool.Parse(vals[(int)Opts.AUTO_RESTORE_TO_DEFAULT_PROF]); }
      set { vals[(int)Opts.AUTO_RESTORE_TO_DEFAULT_PROF] = value.ToString();; }
    }

    public int[] clocking_limits {
      set { spec_vals[G.spec_name + ".clocking_limits"] = value.Clone(); }
      get {
        int[] limits = (int[])spec_vals[G.spec_name + ".clocking_limits"];
        if (limits == null)
          limits  = (int[])spec_vals["clocking_limits"];
        if (limits == null)
          limits = new int[] { 200, 350, 200, 350};

        return limits;
      }
    }

    // clocking presets
    int[] get_clocking_preset(string id) {
      int[] limits = (int[])spec_vals[G.spec_name + "." + id];
      if (limits == null)
        limits  = (int[])spec_vals[id];
      if (limits == null)
        limits = new int[] {0, 0};

      return limits;
    }

    void set_clocking_presets(string id, int[] val) {
        spec_vals[G.spec_name + "." + id] = val.Clone();
    }

    public int[] clocking_preset_slow {
      set { set_clocking_presets("clocking_preset_slow", value); }
      get { return get_clocking_preset("clocking_preset_slow"); }
    }

    public int[] clocking_preset_normal {
      set { set_clocking_presets("clocking_preset_normal", value); }
      get { return get_clocking_preset("clocking_preset_normal"); }
    }

    public int[] clocking_preset_fast {
      set { set_clocking_presets("clocking_preset_fast", value); }
      get { return get_clocking_preset("clocking_preset_fast"); }
    }

    public int[] clocking_preset_ultra {
      set { set_clocking_presets("clocking_preset_ultra", value); }
      get { return get_clocking_preset("clocking_preset_ultra"); }
    }

    #endregion

   #endregion

    #region Private Data
    Hashtable     spec_vals     = new Hashtable(); // keep videocard dependent data; e.g. clocking limits
    bool          m_read_only;
    string        m_file_name   = "options.cfg";
    #endregion

    #region Public Interface
    public AppConfig(bool read_only) {
      G.applog("** entering AppConfig.AppConfig(bool read_only)");
      m_read_only = read_only;
      try {
        new ConfigParser(m_file_name, this);
      } 
      catch (FileNotFoundException) {
        // do nothing
      }
      G.applog("** leaving AppConfig.AppConfig(bool read_only)");
    }

    ~AppConfig() {
      if (!m_read_only) save_config();
    }

    public void save_config() {
      if (m_read_only)
        return;

      vals[(int)Opts.APP_VERSION] = G.app_version;

      StreamWriter sw = new StreamWriter(
        (!G.config_in_isolated_storage
        ? new FileStream(G.app_install_directory + @"\" + m_file_name, FileMode.Create)
        : new IsolatedStorageFileStream(m_file_name, FileMode.Create)),
        System.Text.Encoding.ASCII);
      StringBuilder line = new StringBuilder(256);

      for (int i=0; i < names.Length; ++i) {
        line.Remove(0, line.Length);
        line.AppendFormat(@"name=""{0}"", value=""{1}""", names[i], vals[i]);

        sw.WriteLine(line.ToString());
      }
#if true
      foreach(DictionaryEntry den in spec_vals) {
        line.Remove(0, line.Length);
        string key = (string)den.Key;
        ICollection val_collection = den.Value as ICollection;
        string val_string = den.Value as string;

        if (val_collection == null && val_string == null) {
          // XXX internal error
          continue;
        }

        int idx;
        if ((idx = key.LastIndexOf(".")) != -1) {
          string spec = key.Substring(0, idx);
          string name = key.Substring(idx + 1);
          line.AppendFormat(@"name=""{0}"", spec_name=""{1}"", ", name, spec);
        } else {
          line.AppendFormat(@"name=""{0}"", ", key);
        }   

        if (val_collection != null) {
          line.Append("values=[");

          bool first_item = true;
          foreach(Object item in val_collection) {
            if (first_item) first_item = false;
            else line.Append(", ");
            if (item is string) {
              line.AppendFormat(@"""{0}""", (string)item);
            } else {
              line.Append(item.ToString());
            }
          }
          line.Append("]");
          sw.WriteLine(line.ToString());
        }

      }
#endif
      sw.Close();
    }

    #endregion

    #region File Parser
    /// <summary>
    /// parse the configuration file
    /// </summary>
    class ConfigParser {
      #region Private Data
      AppConfig cfg;
      Regex reg_name      = new Regex(@"\bname=""([^""]+)""");
      Regex reg_spec_name = new Regex(@"\bspec_name=""([^""]+)""");
      Regex reg_value     = new Regex(@"\bvalue=(?:(?<empty_value>"""")|(?:""(?<nonempty_value>.*[^\\])""))");
      Regex reg_values    = new Regex(@"\bvalues=\[(?<val_array>[0-9, ]*)\]");
      Regex reg_label     = new Regex(@"^\[(.+)\]");
      #endregion

      // constructor starts end ends all parsing
      public ConfigParser(string file_name, AppConfig config) {
        cfg = config;
        string text;

        StreamReader sr = new StreamReader(
          (!G.config_in_isolated_storage
          ? new FileStream(G.app_install_directory + @"\" + file_name, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read)
          : new IsolatedStorageFileStream(file_name, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read)),
          System.Text.Encoding.ASCII);

        string res_name;
        string res_value;
        string res_label;

        while ((text = sr.ReadLine()) != null) {
          if (text.Trim().StartsWith(";"))
            continue;
          bool error = false;
          if (!error) {
            Match mat_name = reg_name.Match(text);
            Match mat_spec_name = reg_spec_name.Match(text);
            Match mat_value = reg_value.Match(text);
            Match mat_label = reg_label.Match(text);
            Match mat_values = reg_values.Match(text);

            if (mat_label.Success) {
              res_label = mat_label.Groups[1].ToString();
              interpret_new_label(res_label);
            }
            else if (mat_name.Success &&  mat_value.Success) {
              res_name = mat_name.Groups[1].Value;
              if (mat_value.Groups["empty_value"].Success)
                res_value = "";
              else if (mat_value.Groups["nonempty_value"].Success)
                res_value = mat_value.Groups["nonempty_value"].Value;
              else
                res_value = "";

              if (!interpret_new_item(res_name, res_value))
                error = true;
            }
            else if (mat_name.Success &&  mat_values.Success) {
              res_name = mat_name.Groups[1].Value;
              string val_array = mat_values.Groups["val_array"].Value;
              string[] val_strings = val_array.Split(new char[] {',',});
              int[] vals = new int[val_strings.Length];
              for(int i=0; i < vals.Length; ++i)
                vals[i] = int.Parse(val_strings[i]); // XXX: integer only atm

              string spec_name = (mat_spec_name.Success) ? mat_spec_name.Groups[1].Value : null;

              if (!interpret_new_item(res_name, spec_name, vals))
                error = true;
            }

          }
        }
        interpret_new_label(null);
        sr.Close();
      }
      /// <summary>
      /// Do all pending actions before switching to a new storage object (Label) or EOF.
      /// </summary>
      private void interpret_commit() {
        /*
                    if (current_data != null)
                        cfg.add_prof(current_data);
                    current_data = null;
                    */
      }

      /// <summary>
      /// Starts a new data storage object
      /// </summary>
      /// <param name="label"></param>
      /// <returns></returns>
      private bool interpret_new_label(string label) {
        /*
                    interpret_commit();
                    if (label == null)
                        return true;

                    current_data = new GameProfileData();
                    current_data.profile_descriptor = label;
                    */
        return true;	
      }

      /// <summary>
      /// Store parsed data.
      /// </summary>
      /// <param name="name"></param>
      /// <param name="val"></param>
      /// <returns></returns>
      private bool interpret_new_item(string name, string val) {

        // find arrray slot for named option and store data
        string lc_name = name.ToLower();
        for(int i = 0; i < AppConfig.names.Length; ++i) {
          if (lc_name == AppConfig.names[i]) {
            cfg.vals[i] = val;
            return true;
          }
        }
        return false;
      }

      private bool interpret_new_item(string name, string spec_name, ICollection vals) {
        cfg.spec_vals[(spec_name == null) ? name : spec_name + "." + name] = vals;
        return true;
      }

    }
    #endregion
  }
}
