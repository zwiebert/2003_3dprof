using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms; 
using TDProf.Util;
using System.IO.IsolatedStorage;
using System.Collections;

namespace TDProf.Profiles {
  /// <summary>
  /// How to add a new config option: If you want for example add an option "game_ini_path",
  /// just append GAME_INI_PATH to GameProfileData.Parms. Now append "game_ini_path" to GameProfileData.names.
  /// Now append an string to GameProfileData.vals. The string can be empty or can contain a default
  /// value prefixed by string "default:". This default setting will shown in config file for User
  /// info, but will never read in.
  /// 
  /// Thats it. Next time you save a config file, you will see the new option in each profile with value set to default
  /// value, if not already changed in GUI.
  /// 
  /// You can use set_mode_value() and get_mode_value() to access your new added option.
  /// You may want to add an property to acces your option. Maybe because its not a string,
  /// but a different datatype. You can do this like this:
  /// bool some_option {
  ///  get { return bool.Parse(get_mode_value (Parms.SOME_OPTION)); }
  ///  set { set_mode_value(Parms.SOME_OPTION, value.ToString()); }
  /// }
  /// </summary>
  public class GameProfileData : IComparable {

    #region Add New Options Here
    public enum Parms {
      D3D_FSAA, D3D_ANISO, D3D_VSYNC, D3D_QE, D3D_LOD, D3D_PRE,
      OGL_FSAA, OGL_ANISO, OGL_VSYNC, OGL_QE, OGL_LOD, OGL_PRE,
      EXE_PATH, EXE_ARGS, EXE_PROC_PRIO, EXE_FREE_MEM,
      IMG_PATH, IMG_DRIVE_NMB, IMG_PATH_2, IMG_DRIVE_NMB_2, IMG_PATH_3, IMG_DRIVE_NMB_3, IMG_PATH_4, IMG_DRIVE_NMB_4,
      GAME_INI_PATH,
      SPEC_NAME,
      CLOCKING_KIND, CLOCKING_RESTORE_KIND, CLOCKING_MEM_CLK, CLOCKING_CORE_CLK,
      AUTORESTORE_MODE, AUTORESTORE_FORCE_DISABLE, AUTORESTORE_FORCE_DIALOG, AUTORESTORE_DISABLE_DIALOG,
      COMMANDS_PRE_EXE_RUN, COMMANDS_POST_EXE_EXIT, COMMANDS_PRE_EXE_RUN_GLOB, COMMANDS_POST_EXE_EXIT_GLOB,
      INCLUDE_OTHER_PROFILE,
      D3D_EXTRA_1, D3D_EXTRA_2, D3D_EXTRA_3, D3D_EXTRA_4, D3D_EXTRA_5, D3D_EXTRA_6, D3D_EXTRA_7, D3D_EXTRA_8, D3D_EXTRA_9, D3D_EXTRA_10,
      OGL_EXTRA_1, OGL_EXTRA_2, OGL_EXTRA_3, OGL_EXTRA_4, OGL_EXTRA_5, OGL_EXTRA_6, OGL_EXTRA_7, OGL_EXTRA_8, OGL_EXTRA_9, OGL_EXTRA_10,
      D3D_EXTRA2_1, D3D_EXTRA2_2, D3D_EXTRA2_3, D3D_EXTRA2_4, D3D_EXTRA2_5, D3D_EXTRA2_6, D3D_EXTRA2_7, D3D_EXTRA2_8, D3D_EXTRA2_9, D3D_EXTRA2_10,
      OGL_EXTRA2_1, OGL_EXTRA2_2, OGL_EXTRA2_3, OGL_EXTRA2_4, OGL_EXTRA2_5, OGL_EXTRA2_6, OGL_EXTRA2_7, OGL_EXTRA2_8, OGL_EXTRA2_9, OGL_EXTRA2_10,
    };
    
    public static void rename_option(Parms parm, string new_name) {
      int idx = (int)parm;
      m_names[idx] = new_name;
      m_lc_names[idx] = new_name.ToLower();
    }

    // Note: you have to keep Parms and m_names in the same order		
    private static string[] m_names
      =  {
           "d3d_fsaa", "d3d_aniso", "d3d_vsync", "d3d_qe", "d3d_lod", "d3d_pre_frames",
           "ogl_fsaa", "ogl_aniso", "ogl_vsync", "ogl_qe", "ogl_lod", "ogl_pre_frames",
           "exe_path", "exe_args", "exe_process_priority", "mbytes_ram_to_free_before_run",
           "img_path", "img_drive_number", "img_path_2", "img_drive_number_2", "img_path_3", "img_drive_number_3", "img_path_4", "img_drive_number_4",
           "game_ini_path",
           "spec_name",
           "clock_kind", "clock_restore_kind", "clock_memory", "clock_core",
           "auto_restore_mode", "autorestore_force_disable", "autorestore_force_dialog", "autorestore_disable_dialog",
           "commands_pre_exe_run", "commands_post_exe_exit", "commands_pre_exe_run_glob", "commands_post_exe_exit_glob",
           "include_other_profile",
           "_d3d_extra_1",
           "_d3d_extra_2",
           "_d3d_extra_3",
           "_d3d_extra_4",
           "_d3d_extra_5",
           "_d3d_extra_6",
           "_d3d_extra_7",
           "_d3d_extra_8",
           "_d3d_extra_9",
           "_d3d_extra_10",
           "_ogl_extra_1",
           "_ogl_extra_2",
           "_ogl_extra_3",
           "_ogl_extra_4",
           "_ogl_extra_5",
           "_ogl_extra_6",
           "_ogl_extra_7",
           "_ogl_extra_8",
           "_ogl_extra_9",
           "_ogl_extra_10",

           "_d3d_extra2_1",
           "_d3d_extra2_2",
           "_d3d_extra2_3",
           "_d3d_extra2_4",
           "_d3d_extra2_5",
           "_d3d_extra2_6",
           "_d3d_extra2_7",
           "_d3d_extra2_8",
           "_d3d_extra2_9",
           "_d3d_extra2_10",
           "_ogl_extra2_1",
           "_ogl_extra2_2",
           "_ogl_extra2_3",
           "_ogl_extra2_4",
           "_ogl_extra2_5",
           "_ogl_extra2_6",
           "_ogl_extra2_7",
           "_ogl_extra2_8",
           "_ogl_extra2_9",
           "_ogl_extra2_10",
    };
    static string[] m_lc_names = new string[m_names.Length];
    static bool m_lc_names_initialized = false;
    static public void init_lc_names() {
      if (m_lc_names_initialized)
        return;

      for (int i=0; i < m_lc_names.Length; ++i)
        if (m_lc_names[i] == null)
          m_lc_names[i] = m_names[i].ToLower();
      m_lc_names_initialized = true;
    }

    public static string get_name(Parms parm, bool lowerCase) {
      return (lowerCase ? m_lc_names[(int)parm] : m_names[(int)parm]);
    }

    /// <summary>
    /// contains values defined by m_names in this.m_names and this.Parms
    /// The Initializer contains default values. As long a profile does
    ///  not contain a value, the default is used.
    /// </summary>
    private string[] m_vals
      = {
          "", "", "", "", "", "", 
          "", "", "", "", "", "", 
          "", "", "default:normal", "default:0",
          "", "default:0", "", "default:1", "", "default:2", "", "default:3",
          "",
          "", // spec_name
          EClockKinds.PARENT.ToString(), EClockKinds.PARENT.ToString(), "disabled:0", "disabled:0", // clocking
          "default:normal", "default:False", "default:False", "default:False", // autorestore_*
          "", "", "default:True", "default:True", // commands_*
          "", // include_profile
          "", "", "", "", "", "", "", "", "", "", 
          "", "", "", "", "", "", "", "", "", "", 
          "", "", "", "", "", "", "", "", "", "", 
          "", "", "", "", "", "", "", "", "", "", 
    };

    public enum EClockKinds { PARENT, PRE_SLOW, PRE_NORM, PRE_FAST, PRE_ULTRA };
    #endregion ADD_NEW_ITEMS_HERE
    private string m_unknown_text = ""; // this currently holds unknown options lines separated by "\r\n" ready to print.
    string[] imgs_to_mount = null;
    #region Value Interface Class
    public Value val(Parms parm) { return m_vh[(int)parm]; }

    private Value[] m_vh; // value handles

    // this type allows use of properties instead of get_xxx(Parms id) set_xxx(Parms id, T v)
    public class Value {
      public Value(GameProfileData a_gp, int index) { gp = a_gp; idx = index; } 

      public string Data { 
        get {
          string s = raw_data;
          return s.StartsWith("default:") ? s.Substring(8)
            :    s.StartsWith("disabled:") ? s.Substring(9)
            :    (s == "$dont_print_this_entry$") ? " ~ "
            :    s;
        }
        set { raw_data = (Enabled ? value : "disabled:" + value); }
      }

      public bool Enabled { 
        get {
          return ! raw_data.ToLower().StartsWith("disabled:");
        }
        set {
          if (raw_data.ToLower().StartsWith("disabled:") == !value)
            return;
          if (!value)
            raw_data = "disabled:" + raw_data;
          else
            raw_data = raw_data.Substring(9); // remove disabled: prefix
        }
 
      }

      private string raw_data { 
        get { return gp.m_vals[idx]; }
        set {
          bool has_changed = (gp.m_vals[idx] != value);
          gp.m_vals[idx] = value;
          if (has_changed) {
            GameProfiles.changed_done_to_gpd();
          }
        }
      }
      private Parms m_parm { get { return (Parms)idx; } }
      // the only real data member
      public int idx;
      private GameProfileData gp; // where the actual values are stored
    }
    #endregion

    #region Public Interface
    public EClockKinds clocking_kind {
      get { return (EClockKinds)EClockKinds.Parse(typeof(EClockKinds), val(Parms.CLOCKING_KIND).Data, true); } 
      set { val(Parms.CLOCKING_KIND).Data = value.ToString(); }
    }

    public EClockKinds clocking_restore_kind {
      get { return (EClockKinds)EClockKinds.Parse(typeof(EClockKinds), val(Parms.CLOCKING_RESTORE_KIND).Data, true); } 
      set { val(Parms.CLOCKING_RESTORE_KIND).Data = value.ToString(); }
    }

    public string name = "unamed";
    public int flags = 0;
    public float clocking_mem_clock {
      get { return float.Parse(val(Parms.CLOCKING_MEM_CLK).Data, System.Globalization.NumberFormatInfo.InvariantInfo); }
      set { val(Parms.CLOCKING_MEM_CLK).Data = value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo); }
    }

    public float clocking_core_clock {
      get { return float.Parse(val(Parms.CLOCKING_CORE_CLK).Data, System.Globalization.NumberFormatInfo.InvariantInfo); }
      set { val(Parms.CLOCKING_CORE_CLK).Data = value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo); }
    }

    public enum AutoRestoreMode { NORMAL, OFF, FORCE_DIALOG, NO_DIALOG, }; 
    public AutoRestoreMode auto_restore_mode {
      get { return (AutoRestoreMode)AutoRestoreMode.Parse(typeof(AutoRestoreMode), val(Parms.AUTORESTORE_MODE).Data, true); } 
      set { val(Parms.AUTORESTORE_MODE).Data = value.ToString().ToLower(); }
    }

    public string spec_name { get { return val(Parms.SPEC_NAME).Data; } set { val(Parms.SPEC_NAME).Data = value; } }
    public string exe_path { get { return val(Parms.EXE_PATH).Data; } set { val(Parms.EXE_PATH).Data = value; } }
    public string exe_args { get { return val(Parms.EXE_ARGS).Data; } set { val(Parms.EXE_ARGS).Data = value; } }
    public int exe_free_mem {
      get { return int.Parse(val(Parms.EXE_FREE_MEM).Data); }
      set { val(Parms.EXE_FREE_MEM).Data = value.ToString(); }
    }

    static readonly string LINE_SEPARATOR = ";-=NEWLINE=-;";
    public string command_pre_exe_run {
      get { return val(Parms.COMMANDS_PRE_EXE_RUN).Data.Replace(LINE_SEPARATOR, Environment.NewLine).Replace("\\\"", "\""); }
      set { val(Parms.COMMANDS_PRE_EXE_RUN).Data = value.Replace(Environment.NewLine, LINE_SEPARATOR).Replace("\"", "\\\""); }
    }
    public string command_post_exe_exit {
      get { return val(Parms.COMMANDS_POST_EXE_EXIT).Data.Replace(LINE_SEPARATOR, Environment.NewLine).Replace("\\\"", "\""); }
      set { val(Parms.COMMANDS_POST_EXE_EXIT).Data = value.Replace(Environment.NewLine, LINE_SEPARATOR).Replace("\"", "\\\""); }
    }

    public string include_other_profile { get { return val(Parms.INCLUDE_OTHER_PROFILE).Data; } set { val(Parms.INCLUDE_OTHER_PROFILE).Data = value; } }

    // begin obsolete
    public bool autorestore_force_disable {
      get { return (auto_restore_mode == AutoRestoreMode.OFF); } 
    }

    public bool autorestore_force_dialog {
      get { return (auto_restore_mode == AutoRestoreMode.FORCE_DIALOG); } 
    }

    public bool autorestore_disable_dialog {
      get { return (auto_restore_mode == AutoRestoreMode.NO_DIALOG); } 
    }
    // end obsolete

    public bool command_pre_exe_run_glob {
      get { return bool.Parse(val(Parms.COMMANDS_PRE_EXE_RUN_GLOB).Data); }
      set { val(Parms.COMMANDS_PRE_EXE_RUN_GLOB).Data = value.ToString(); }
    }

    public bool command_post_exe_exit_glob {
      get { return bool.Parse(val(Parms.COMMANDS_POST_EXE_EXIT_GLOB).Data); }
      set { val(Parms.COMMANDS_POST_EXE_EXIT_GLOB).Data = value.ToString(); }
    }

    #region Process Priorities
    static readonly string[] proc_prio_names = { "Idle", "BelowNormal", "Normal", "AboveNormal", "High" };
    static readonly string[] proc_prio_names_lc = { "idle", "belownormal", "normal", "abovenormal", "high" };
    static readonly ProcessPriorityClass[] proc_prios
      = { ProcessPriorityClass.Idle, ProcessPriorityClass.BelowNormal, ProcessPriorityClass.Normal,
          ProcessPriorityClass.AboveNormal, ProcessPriorityClass.High };
    public ProcessPriorityClass exe_process_prio {
      get {
        string prio_name_lc = val(Parms.EXE_PROC_PRIO).Data.ToLower();
        for (int i=0; i < proc_prios.Length; ++i)
          if (prio_name_lc == proc_prio_names_lc[i])
            return proc_prios[i];
        return ProcessPriorityClass.Normal; // not found, fall back to default
      }
      set {
        for (int i=0; i < proc_prios.Length; ++i)
          if (value == proc_prios[i])
            val(Parms.EXE_PROC_PRIO).Data  = proc_prio_names[i]; 
      }
    }
    #endregion
        
    public string[] img_path {
      get {
        int i;
        string[] data = { val(Parms.IMG_PATH).Data, val(Parms.IMG_PATH_2).Data,
                          val(Parms.IMG_PATH_3).Data, val(Parms.IMG_PATH_4).Data };
        int count = 0;
        for (i=0; i < data.Length; ++i)
          if (data[i] != "")
            ++count;
        string[] result = new String[count];
        i = 0;
        foreach (string s in data)
          if (s != "")
            result[i++] = s;

        return result; 
      }

      set {
        val(Parms.IMG_PATH).Data = (value.Length > 0) ? value[0] : "";
        val(Parms.IMG_PATH_2).Data = (value.Length > 1) ? value[1] : "";
        val(Parms.IMG_PATH_3).Data = (value.Length > 2) ? value[2] : "";
        val(Parms.IMG_PATH_4).Data = (value.Length > 3) ? value[3] : "";
      }
    }

    public int[] img_drive_number {
      get {
        int i;
        string[] data = { val(Parms.IMG_DRIVE_NMB).Data, val(Parms.IMG_DRIVE_NMB_2).Data,
                          val(Parms.IMG_DRIVE_NMB_3).Data, val(Parms.IMG_DRIVE_NMB_4).Data };
        int count = 0;
        for (i=0; i < data.Length; ++i)
          if (data[i] != "")
            ++count;
        int[] result = new int[count];
        i = 0;
        foreach (string s in data)
          if (s != "")
            result[i++] = int.Parse(s);

        return result; 
      }

      set {
#if false
        val(Parms.IMG_DRIVE_NMB).Data = (value.Length > 0) ? value[0] : "";
        val(Parms.IMG_DRIVE_NMB_2).Data = (value.Length > 1) ? value[1] : "";
        val(Parms.IMG_DRIVE_NMB_3).Data = (value.Length > 2) ? value[2] : "";
        val(Parms.IMG_DRIVE_NMB_4).Data = (value.Length > 3) ? value[3] : "";
#else
        if (value.Length > 0) val(Parms.IMG_DRIVE_NMB).Data = value[0].ToString();
        if (value.Length > 1) val(Parms.IMG_DRIVE_NMB_2).Data = value[1].ToString();
        if (value.Length > 2) val(Parms.IMG_DRIVE_NMB_3).Data = value[2].ToString();
        if (value.Length > 3) val(Parms.IMG_DRIVE_NMB_4).Data = value[3].ToString();
#endif
      }
    }

    public string game_ini_path { get { return val(Parms.GAME_INI_PATH).Data; } set { val(Parms.GAME_INI_PATH).Data = value; } }
  
    public GameProfileData() {
      Debug.Assert(m_vals.Length == m_names.Length);
      m_vh = new Value[m_vals.Length];
      for (int i=0; i < m_vh.Length; ++i) {
        m_vh[i] = new Value(this, i);
      }
    }

    /// <summary>
    /// copy constructor (clone)
    /// </summary>
    /// <param name="other">object to clone</param>
    public GameProfileData(GameProfileData other) {
      m_vh = new Value[m_vals.Length];
      for (int i=0; i < m_vh.Length; ++i) {
        m_vh[i] = new Value(this, i);
      }
      if (other == null)
        return;

#if false
      //TODO: really needed to use Copy? As long we do not use things like += ...
      this.spec_name = string.Copy(other.spec_name);
      this.exe_path = string.Copy(other.exe_path);
      this.exe_args = string.Copy(other.exe_args);
      this.img_drive_number = (int[])other.img_drive_number.Clone();
      this.img_path = (string[])img_path.Clone(); //TODO:image
      this.game_ini_path = string.Copy(other.game_ini_path);
#endif
      this.flags = other.flags;
      this.name = string.Copy(other.name);


      for (int i=0; i < m_vals.Length; ++i) {
        this.m_vals[i] = string.Copy(other.m_vals[i]);
      }
    }



    /// <summary>
    /// Tells external virtual CD drive software (daemon.exe) to mount given image.
    /// </summary>
    /// <param name="daemon_exe_path">path to CDROM image readable by daemon.exe</param>
    public bool mount_img(string daemon_exe_path) {
      bool success = true;
      if (img_path.Length == 0)
        return true;

      if (img_path.Length > 0 && daemon_exe_path != "") {
        int nmb_imgs = img_path.Length;

        if (!Utils.file_exists(daemon_exe_path)) {
          System.Windows.Forms.MessageBox.Show("Daemon.exe could not be found at \""
            + daemon_exe_path
            + "\".\r\n"
            + "\r\nHint: Configure the correct path in menu Options=>Settings and try again.",
            G.app_name + " - User Error");
          success = false;
        }

        foreach (string img in img_path)
          if (!Utils.file_exists(img)) {
            System.Windows.Forms.MessageBox.Show("CDROM image could not be found at \""
              + img
              + "\".\r\n"
              + "\r\nHint: Configure the correct path in Game Profile \""
              + name
              + "\" and try again.",
              G.app_name + " - User Error");
            success = false;
          }

        if (!success)
          return false;

        string exe = unquote_string(daemon_exe_path);
        string[][] imgs_by_drive_nmb = { null, null, null, null };
        for (int dn=0; dn < 4; ++dn) {
          int img_count = 0;
          for (int i=0; i < nmb_imgs; ++i)
            if (dn == img_drive_number[i])
              ++img_count;
          if (img_count == 0)
            continue;

          imgs_by_drive_nmb[dn] = new string[img_count];
          int img_idx = 0;

          for (int i=0; i < nmb_imgs; ++i)
            if (dn == img_drive_number[i]) {
              string img = img_path[i];
              imgs_by_drive_nmb[dn][img_idx++] = img;
            }
        }

        bool ask_user = false;
        foreach(string[] sa in imgs_by_drive_nmb)
          if (sa != null && sa.Length > 1) {
            ask_user = true;
            break;
          }

        imgs_to_mount = new string[] { null, null, null, null };
        if (!ask_user) {
          for (int i=0; i < 4; ++i)
            if (imgs_by_drive_nmb[i] != null)
              imgs_to_mount[i] = imgs_by_drive_nmb[i][0];

        } else if (ask_user /* && G.root_form == null */) {
          FormImageChooser frm = new FormImageChooser(imgs_to_mount, imgs_by_drive_nmb);
          frm.ShowDialog();
          frm.Dispose();
        }

        for(int i=0; i < 4; ++i) {
          if (imgs_to_mount[i] == null)
            continue;

          string args = "-mount " + i + "," + "\"" + imgs_to_mount[i] + "\"";
          System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
          psi.FileName = exe;
          psi.Arguments = args;
          psi.CreateNoWindow = true;
          System.Diagnostics.Process process = new System.Diagnostics.Process();
          process.StartInfo = psi;
          if (process.Start()) {
            process.WaitForExit(10 * 1000); //TODO: make timeout configurable (?)
            if (!process.HasExited) {
              return false; // usual daemon.exe complains about non existing image file and exits 0 (success) anyway.
              // ask user: kill or wait?
            }
          }
        }
        return true;
      }
      return false;
    }

    public void unmount_img(string daemon_exe_path) {
      if (imgs_to_mount == null)
        return;
      for(int i=0, e=imgs_to_mount.Length; i < e; ++i) {
        if (imgs_to_mount[i] == null)
          continue;
        Process.Start(daemon_exe_path, "-unmount " + i.ToString());
      }
    }

    class ram_free_mbox_thread {
      Process proc;
      MessageForm mf = null;

      public ram_free_mbox_thread(Process p) {
        proc = p;
      }

      public void start() {
        mf = MessageForm.open_dialog(
          "Please wait until enough RAM has been free'd.\r\n"
          + "You can also hit \"Abort\" to stop freeing RAM now.\r\n"
          + "\r\nHint: This function can be configured by menu Profile => Free RAM" 
          ,
          "Free RAM");
        mf.ShowDialog();
        proc.Kill();
      }
    }

    public void page_out_physical_mem(int mb) {
      if (mb == 0)
        return;
      Process p = new Process();
      p.StartInfo.FileName = "etc/free_ram.exe";
      p.StartInfo.Arguments = ((mb != -1) ? mb : 1024).ToString();
      p.StartInfo.UseShellExecute = false;
      //p.StartInfo.RedirectStandardOutput = true;
      //p.StartInfo.RedirectStandardError = true;
      //p.StartInfo.RedirectStandardInput = true;
      p.StartInfo.CreateNoWindow = true;
      p.Start();
      ram_free_mbox_thread tc = new ram_free_mbox_thread (p);
      Thread t = new Thread(new ThreadStart(tc.start));
      t.Start();

      p.WaitForExit();
      t.Abort();
    }

    /// <summary>
    /// Start a Game using exe path and args stored in this profile
    /// </summary>
    public System.Diagnostics.Process run_exec() {

      if (exe_path == null || exe_path == "") // this really sucks, not our business
        return null;

      page_out_physical_mem(exe_free_mem);

      string exe = unquote_string(exe_path);
      Match match = Regex.Match(exe, @"^(.+)\\");
      string wd = match.Groups[1].ToString();

      System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
      psi.FileName = exe;
      psi.WorkingDirectory = wd;
      psi.Arguments = this.exe_args;


      System.Diagnostics.Process process = new System.Diagnostics.Process();
      process.StartInfo = psi;
      try {
        if (process.Start()) {
          if (val(Parms.EXE_PROC_PRIO).Enabled)
            process.PriorityClass = this.exe_process_prio;
          swap.swap_me_out();
          return process;
        }
      } catch (Exception e) {
        System.Windows.Forms.MessageBox.Show("Could not start game!\r\n"
          + "\r\nExe File: " + psi.FileName + "\r\n"
          + "Error Message: " + e.Message, G.app_name + " - External Error");
      }
  
      return null;
    }


    /// <summary>
    /// Creates a config file section from this profile.
    /// That section starts with a label in brackets followed
    /// by multiple lines of name/value pairs.  
    /// </summary>
    /// <returns>multiline config file section</returns>
    public string format() {
      string s = "";
      s += "[" + name + "]" + Environment.NewLine;
      for (int i = 0; i < m_names.Length; ++i) {
        if (m_names[i].StartsWith("_"))
          continue;
        if (m_vals[i] == "$dont_print_this_entry$")
          continue;
        s += "name=\"" + m_names[i] + "\", " + "value=\"" + m_vals[i] + "\"" + Environment.NewLine;
      }
      s += this.m_unknown_text;
      return s;
    }
    /// <summary>
    /// Set an option according to name/value pair in config file format.
    ///  Called by parser class.
    /// </summary>
    /// <param name="name">An option name from this.m_names (case insensitive)</param>
    /// <param name="val">option value</param>
    /// <returns>True for success. Will fail if name is unknown (not in this.m_names)</returns>
    public bool add_tag(string name, string val) {
      string lc_name = name.ToLower();
      if (lc_name.StartsWith("default:"))
        return true; // default values are shown in config file. But when loading, current values will be kept.
      bool alien_chip = (this.spec_name != "" && this.spec_name != G.spec_name);
      int names_length = alien_chip ? m_names.Length - 40 : m_names.Length; //TODO: hardcoded value
      
      for (int i = 0; i < m_names.Length; ++i) {
        if (lc_name == m_lc_names[i]) { 
          if (alien_chip && i >= names_length) {
            m_vals[i] = "$dont_print_this_entry$";
            return false; // do not store current vidcard value into old vidcard profile
          }
          m_vals[i] = val;
          return true;
        }
      }

      if (this.spec_name == "" || this.spec_name != G.spec_name)
        this.m_unknown_text 
          += String.Format("name=\"{0}\", value=\"{1}\"{2}", name, val,Environment.NewLine);
      return false;
    }
	


    #endregion

    #region Utils
    /// <summary>
    /// Expand ENV variables, parse quotes like \" \'
    /// TODO: implement me
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private string unquote_string(string s) { 
      string r = s.Trim(new char[] {'\"'});
      r = Environment.ExpandEnvironmentVariables(r);
      return r;
    }
    public int CompareTo(Object other) {
      GameProfileData o = (GameProfileData)other;

      // compare by group spec_name
      if (this.spec_name == G.spec_name) {
        if (o.spec_name != G.spec_name)
          return -1;
      } else if (o.spec_name == G.spec_name) {
        return 1;
      } else if (this.spec_name == "") {
        return -1;
      } else if (o.spec_name == "") {
        return 1;
      }

      // compare by name
      string this_s = spec_name + name;
      string other_s = o.spec_name + o.name;
      int res = this_s.CompareTo(other_s);
      return res;
    }

    #endregion
  }

  public class GameProfiles {
    public ArrayList profs = new ArrayList(); //TODO:private
 
    private GameProfiles() { GameProfileData.init_lc_names(); }




    // counting unsaved changes 
    public static int change_count { get { return G.prof_change_count; } }


    internal static void changed_done_to_gp() {
      ++G.prof_change_count;
      if (Unsaved != null)
        Unsaved(null, null);
      if (ListChanged != null)
        ListChanged(null, null);
    }

    internal static void changed_done_to_gpd() {
      ++G.prof_change_count;
      if (Unsaved != null)
        Unsaved(null, null);
      if (DataChanged != null)
        DataChanged(null, null);
    }

    #region Public Interface

    /// <summary>
    /// A change to a profile or to the list of profiles happened.
    /// </summary>
    static public event System.EventHandler Unsaved = null;
    /// <summary>
    /// Data inside a profile has changed
    /// </summary>
    static public event System.EventHandler DataChanged = null;
    /// <summary>
    /// A profile has been added, removed, renamed or changed his place in the list 
    /// </summary>
    static public event System.EventHandler ListChanged = null;


    public int nmb_of_profiles { get { return profs.Count; } }
    public GameProfileData get_profile(int idx) { return (GameProfileData)profs[idx]; }
    public GameProfileData get_profile(string name) {
      int idx = get_profile_index(name);
      if (idx == -1)
        return null;
      return get_profile(idx);
    }
      
    public void remove_profile(int idx) {
      profs.RemoveAt(idx);
      changed_done_to_gp();
    }

    public int add_prof(GameProfileData obj) {
      profs.Add(obj);
      profs.Sort();
      changed_done_to_gp();
      return profs.Count - 1;
    }
    public int get_profile_index(string name) {
      int result = -1;
      for (int i=0; i < nmb_of_profiles; ++i) {

        GameProfileData data = get_profile(i);
        if (name == data.name) {
          if (data.spec_name == G.spec_name)
            return i; // 100% match
          else
            result = i; // TODO: find best match if no 100% match exist
        }
      }
      return result;
    }

    public int rename_profile(string old_name, string new_name) {
      int count = 0;
      foreach (object obj in profs) {
         GameProfileData gpd = (GameProfileData)obj;
        if (gpd.name == old_name) {
          foreach (object obj2 in profs) {
            GameProfileData gpd2 = (GameProfileData)obj2;
            if (gpd2.name == new_name && gpd2.spec_name == gpd.spec_name)
              goto name_exists;
          }
          gpd.name = new_name;
          ++count;
        name_exists: continue;
        }
          
      }
      if (count > 0)
        changed_done_to_gp();

      return count;
    }


    // spec_name renaming

    // get each unique spec_name occuring in the game profiles
    public string[] get_spec_names() {
      Hashtable ht = new Hashtable();
      foreach (object obj in profs) {
        ht[((GameProfileData)obj).spec_name] = 0;
      }
      string[] result = new string[ht.Count];
      ht.Keys.CopyTo(result, 0);
      return result;
    }

    // Rename all spec_name attributes matching old_name to new_name. Return number of succesfull renamings.
    int rename_spec_names(string old_name, string new_name) {
      int count = 0;
      foreach (object obj in profs) {
        GameProfileData gpd = (GameProfileData)obj;
        if (gpd.spec_name == old_name) {
          gpd.spec_name = new_name;
          ++ count;
        }
      }
      return count;
    }

    public string guess_spec_rename(string spec_name) {
      int count_match = 0;
      int count_vendor = 0;

      int idx_1st_dash = spec_name.IndexOf("-");
      if (idx_1st_dash == -1)
        return null;

      string spec_vendor = spec_name.Substring(0, idx_1st_dash);

      foreach (object obj in profs) {
        GameProfileData gpd = (GameProfileData)obj;
        if (gpd.spec_name == spec_name)
          ++count_match;
        else if (gpd.spec_name == spec_vendor)
          ++count_vendor;
      }

      if (count_match > 0)
        return null;
      if (count_vendor > 0)
        return spec_vendor;

      return null;
    }

    public int check_if_spec_rename_required() {
      string old_spec_name = guess_spec_rename(G.spec_name);
      if (old_spec_name == null)
        return 0;

      DialogResult dr = MessageBox.Show("No profile for the current card was found. Do you want to move existing profiles atttached to graphic adapter \""
        + old_spec_name + "\" to the current adapter \""
        + G.spec_name + "\"?"
        +"\r\n\r\nHint: This usually happens if support for your card was newly added to 3DProf. If this is true, just click Yes.",
        "3DProf - ...",
        MessageBoxButtons.YesNo);
      if (dr == System.Windows.Forms.DialogResult.Yes) {
        int count = rename_spec_names(old_spec_name, G.spec_name);
        profs.Sort();
        return count;
      }

       return 0;
    }

    /// <summary>
    /// Save all profiles to a file. Old content of that file will be lost.
    /// </summary>
    /// <param name="name">path to a writeable file</param>
    public void save_profiles(string file_name) {
      StreamWriter sw = new StreamWriter(
        (!G.config_in_isolated_storage
        ? new FileStream(file_name, FileMode.Create)
        : new IsolatedStorageFileStream(file_name, FileMode.Create)),
        System.Text.Encoding.ASCII);

      for (int i=0; i < profs.Count; ++i) {
        string s = get_profile(i).format();
        sw.Write(s);
        sw.WriteLine("");
      }
      sw.Close();
      G.prof_change_count = 0;


    }


    public void append_from_file(string name) {
      if (Utils.file_exists (name))
        new ConfigParser(name, this);
    }
    /// <summary>
    /// Only way to create a new object of this class.
    /// </summary>
    /// <param name="name">config file name to read profiles from</param>
    /// <returns>new object holding all profiles</returns>
    public static GameProfiles create_from_file(string name) {
			
      GameProfiles obj = new GameProfiles();
      try {
        new ConfigParser(name, obj);
      } catch (FileNotFoundException) {
        // do nothing
      }

      obj.profs.Sort();

      G.prof_change_count = 0;

      // kludge
     
      int count = obj.check_if_spec_rename_required();
      if (count > 0) {
        DialogResult dr = MessageBox.Show("" + count + " changes made. Do you want to keep/save these these changes?", "3DProf - Moving Complete", MessageBoxButtons.YesNo);
        if (dr == System.Windows.Forms.DialogResult.Yes) {
          obj.save_profiles(name);
        }
      }

      return obj;
    }


    public GameProfileData this[int index] {
      get {
        return get_profile(index);
      }
    }

    public IEnumerator GetEnumerator() {
      return new Enumerator(this);
    }
    #endregion


    class Enumerator : IEnumerator {
      private int m_nCurr = -1;
      private GameProfiles m_gps;

      public Enumerator(GameProfiles gps) {
        m_gps = gps;
      }

      public void Reset() {
        m_nCurr = -1;
      }

      public bool MoveNext() {
        if (++m_nCurr >= m_gps.profs.Count)
          return false;
        return true;
      }

      public object Current {
        get {
          if (m_nCurr < 0)
            throw new InvalidOperationException();
          if (m_nCurr >= m_gps.profs.Count)
            throw new InvalidOperationException();
          return m_gps.profs[m_nCurr];
        }
      }

    }
  }

  #region Config File Parser
  class ConfigParser {
    private GameProfiles cfg;
    private GameProfileData current_data;

    // constructor starts end ends all parsing
    public ConfigParser(string file_name, GameProfiles config) {
      cfg = config;
      string text;

      StreamReader sr = new StreamReader(
        (!G.config_in_isolated_storage
        ? new FileStream(file_name, FileMode.Open)
        : new IsolatedStorageFileStream(file_name, FileMode.Open)),
        System.Text.Encoding.ASCII);


      Regex reg_name = new Regex("\\bname=\"([^\"]+)\"");
      Regex reg_value = new Regex(@"\bvalue=(?:(?<empty_value>"""")|(?:""(?<nonempty_value>.*[^\\])""))");
      Regex reg_label = new Regex(@"^\[(.+)\]");

      string res_name;
      string res_value;
      string res_label;

      while ((text = sr.ReadLine()) != null) {
        if (text.Trim().StartsWith(";"))
          continue;
        bool error = false;
        if (!error) {
          Match mat_name = reg_name.Match(text);
          Match mat_value = reg_value.Match(text);
          Match mat_label = reg_label.Match(text);

          if (mat_label.Success) {
            res_label = mat_label.Groups[1].ToString();
            interpret_new_label(res_label);
          }
          else if (mat_name.Success &&  mat_value.Success) {
            res_name = mat_name.Groups[1].ToString();
            if (mat_value.Groups["empty_value"].Success)
              res_value = "";
            else if (mat_value.Groups["nonempty_value"].Success)
              res_value = mat_value.Groups["nonempty_value"].Value;
            else
              res_value = "";

            if (!interpret_new_item(res_name, res_value))
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
      if (current_data != null)
        cfg.add_prof(current_data);
      current_data = null;
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

      current_data = new GameProfileData();
      current_data.name = label;
      return true;	
    }

    /// <summary>
    /// Store parsed data.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    private bool interpret_new_item(string name, string val) {
      if (current_data == null)
        return false;

      // XXX: translate obsolete options, this sucks
      if (G.is_vendor_ati && name == "D3D_DisableHyperZ") { //bw-15-Nov-03
        val = val.Replace("Yes", "Off").Replace("No", "On");
        name = "D3D_EnableHyperZ";        
      }

      return current_data.add_tag(name, val);
    }

  }

	#endregion FILEPARSER

}

