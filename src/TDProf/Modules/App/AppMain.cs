#define NEW_RUN
#define CR_APPLY
#define TEST_RESTORE
#define NO_AUTO_SAFE_MODE
//#define NOTRY
//#define TEST_NOLF
//#define TEST_PRE_COMMANDS
//#define TEST_GC

using System;
using System.IO;
using System.Diagnostics;
using TDProf.DriverSettings;
using TDProf.Profiles;
using TDProf.Util;
using TDProf.App;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Threading;
using MessageBox = System.Windows.Forms.MessageBox;

namespace TDProf {
  class option_x {
    public readonly GameProfileData.Parms gp_parm;
    public option_x(GameProfileData.Parms a_gp_parm) { gp_parm = a_gp_parm; }
  }
  class option_3d : option_x {
    public readonly ConfigRecord.EMode cr_mode;
    public readonly int idx;
    // public option_3d(ConfigRecord.EMode a_cr_mode, GameProfileData.Parms a_gp_parm) { idx = 0; cr_mode = a_cr_mode; gp_parm = a_gp_parm; }
    public option_3d(int a_idx) : base(G.gp_parms[a_idx]) { idx = a_idx; cr_mode = G.cr_modes[a_idx]; ;  }
  }

  class option_clock : option_x {
    public enum EKind { CORE, MEM, PRESET };
    public readonly EKind kind;
    public option_clock(EKind a_kind)
      : base (a_kind == EKind.CORE ? GameProfileData.Parms.CLOCKING_CORE_CLK
      : a_kind == EKind.MEM ? GameProfileData.Parms.CLOCKING_MEM_CLK
      : GameProfileData.Parms.CLOCKING_KIND) {
      kind = a_kind; }
  }

}

namespace TDProf {
  public class FatalError : System.Exception {
    new public string Message;
    public FatalError(string msg) { Message = msg; }
    protected FatalError() {}
  }

  public class ParseError : FatalError {
    public ParseError(string msg) { Message = "Parse error: " + msg; }
  }
}

class AppVersion {
  public static int compare_version_strings(string lhs, string rhs) {
    string[] lhs_parts = lhs.Split(new char[] {'.'});
    string[] rhs_parts = rhs.Split(new char[] {'.'});
    for(int i=0; i < lhs_parts.Length && i < rhs_parts.Length; ++i) {
      int diff = Convert.ToInt32(lhs_parts[i]) - Convert.ToInt32(rhs_parts[i]);
      if (diff != 0)
        return diff;
    }
    return lhs_parts.Length - rhs_parts.Length;
  }
};

internal class G {
  public static string ManualDirectory            { get { return (Directory.Exists(@".\manual") ? @".\manual" :  @"..\..\TDProf\manual"); } }
  public static string ManualIndexHTML {
    get {
      string dir = ManualDirectory;
      string lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
      string index_localized = dir + @"\all_" + lang + ".html";
      string index_default = dir + @"\all.html";

      return File.Exists(index_localized) ? index_localized : index_default;
    }
  }

  public static string AppDirectory             { get { return app_install_directory; } }
  public static string SpecfileDirectory        { get { return (Directory.Exists(@".\specfiles") ? @".\specfiles" :  @"..\..\TDProf\specfiles"); } }

  public static TDProf.GUI.FormMain root_form = null;
  public static bool is_gui_loaded              { get { return (root_form != null); } }
  public static readonly string app_name = "3DProf";
  public static AppContext ax;
  public static string spec_name = "";
  public static bool is_vendor_ati              { get { return spec_name == "ati" || spec_name.StartsWith("ati-"); } }
  public static bool is_vendor_nvidia           { get { return spec_name == "nvidia" || spec_name.StartsWith("nvidia-"); } }
  public static string spec_vendor = "";
  public static int prof_change_count = 0;
  public static TDProf.DisplayInfo di = null;
  public static bool m_config_in_isolated_storage = false; // where to put config files
  public static bool config_in_isolated_storage {
    get { return m_config_in_isolated_storage; }
    set { 
      if (m_config_in_isolated_storage != value) {
        ax.ac.save_config();
        m_config_in_isolated_storage = value;
        ax.ac = new AppConfig(false);
        ax.reinit_gp_from_file();
      }
    }
  }

  public static string app_install_directory = "";
  public static TDProf.AppLocale loc = TDProf.AppLocale.create();
  public static bool exp_radeon_setclk_minimize_step = false;
  public static string app_version = "1.3.0";
  public static Mutex gui_mutex;
  #region CR2GP_MAP
  public static readonly int cr_modes_extra1_start = (2 * 6);
  public static readonly int cr_modes_extra2_start = (2 * 6  +  2 * 8);
  /// <summary>
  /// This lookup table maps an index of combos_[prof_]modes to related ConfigRecord.EMode value
  /// </summary>
  public static readonly ConfigRecord.EMode[] cr_modes
    = {
        ConfigRecord.EMode.D3D_FSAA,
        ConfigRecord.EMode.OGL_FSAA,
        ConfigRecord.EMode.D3D_ANISO,
        ConfigRecord.EMode.OGL_ANISO,
        ConfigRecord.EMode.D3D_VSYNC,
        ConfigRecord.EMode.OGL_VSYNC,
        ConfigRecord.EMode.D3D_QE,
        ConfigRecord.EMode.OGL_QE,
        ConfigRecord.EMode.D3D_LOD,
        ConfigRecord.EMode.OGL_LOD,
        ConfigRecord.EMode.D3D_PRE,
        ConfigRecord.EMode.OGL_PRE,
        ConfigRecord.EMode.D3D_EXTRA_1, ConfigRecord.EMode.OGL_EXTRA_1,
        ConfigRecord.EMode.D3D_EXTRA_2, ConfigRecord.EMode.OGL_EXTRA_2,
        ConfigRecord.EMode.D3D_EXTRA_3, ConfigRecord.EMode.OGL_EXTRA_3,
        ConfigRecord.EMode.D3D_EXTRA_4, ConfigRecord.EMode.OGL_EXTRA_4,
        ConfigRecord.EMode.D3D_EXTRA_5, ConfigRecord.EMode.OGL_EXTRA_5,
        ConfigRecord.EMode.D3D_EXTRA_6, ConfigRecord.EMode.OGL_EXTRA_6,
        ConfigRecord.EMode.D3D_EXTRA_7, ConfigRecord.EMode.OGL_EXTRA_7,
        ConfigRecord.EMode.D3D_EXTRA_8, ConfigRecord.EMode.OGL_EXTRA_8,
        ConfigRecord.EMode.D3D_EXTRA_9, ConfigRecord.EMode.OGL_EXTRA_9,
        ConfigRecord.EMode.D3D_EXTRA_10, ConfigRecord.EMode.OGL_EXTRA_10,

        ConfigRecord.EMode.D3D_EXTRA2_1, ConfigRecord.EMode.OGL_EXTRA2_1,
        ConfigRecord.EMode.D3D_EXTRA2_2, ConfigRecord.EMode.OGL_EXTRA2_2,
        ConfigRecord.EMode.D3D_EXTRA2_3, ConfigRecord.EMode.OGL_EXTRA2_3,
        ConfigRecord.EMode.D3D_EXTRA2_4, ConfigRecord.EMode.OGL_EXTRA2_4,
        ConfigRecord.EMode.D3D_EXTRA2_5, ConfigRecord.EMode.OGL_EXTRA2_5,
        ConfigRecord.EMode.D3D_EXTRA2_6, ConfigRecord.EMode.OGL_EXTRA2_6,
        ConfigRecord.EMode.D3D_EXTRA2_7, ConfigRecord.EMode.OGL_EXTRA2_7,
        ConfigRecord.EMode.D3D_EXTRA2_8, ConfigRecord.EMode.OGL_EXTRA2_8,
        ConfigRecord.EMode.D3D_EXTRA2_9, ConfigRecord.EMode.OGL_EXTRA2_9,
        ConfigRecord.EMode.D3D_EXTRA2_10, ConfigRecord.EMode.OGL_EXTRA2_10,
  };
  /// <summary>
  /// This lookup table maps an index of combos_[prof_]modes to related GameProfileData.Parms value
  /// </summary>
  public static GameProfileData.Parms[] gp_parms
    = {
        GameProfileData.Parms.D3D_FSAA,
        GameProfileData.Parms.OGL_FSAA,
        GameProfileData.Parms.D3D_ANISO,
        GameProfileData.Parms.OGL_ANISO,
        GameProfileData.Parms.D3D_VSYNC,
        GameProfileData.Parms.OGL_VSYNC,
        GameProfileData.Parms.D3D_QE,
        GameProfileData.Parms.OGL_QE,
        GameProfileData.Parms.D3D_LOD,
        GameProfileData.Parms.OGL_LOD,
        GameProfileData.Parms.D3D_PRE,
        GameProfileData.Parms.OGL_PRE,
        GameProfileData.Parms.D3D_EXTRA_1, GameProfileData.Parms.OGL_EXTRA_1,
        GameProfileData.Parms.D3D_EXTRA_2, GameProfileData.Parms.OGL_EXTRA_2,
        GameProfileData.Parms.D3D_EXTRA_3, GameProfileData.Parms.OGL_EXTRA_3,
        GameProfileData.Parms.D3D_EXTRA_4, GameProfileData.Parms.OGL_EXTRA_4,
        GameProfileData.Parms.D3D_EXTRA_5, GameProfileData.Parms.OGL_EXTRA_5,
        GameProfileData.Parms.D3D_EXTRA_6, GameProfileData.Parms.OGL_EXTRA_6,
        GameProfileData.Parms.D3D_EXTRA_7, GameProfileData.Parms.OGL_EXTRA_7,
        GameProfileData.Parms.D3D_EXTRA_8, GameProfileData.Parms.OGL_EXTRA_8,
        GameProfileData.Parms.D3D_EXTRA_9, GameProfileData.Parms.OGL_EXTRA_9,
        GameProfileData.Parms.D3D_EXTRA_10, GameProfileData.Parms.OGL_EXTRA_10,

        GameProfileData.Parms.D3D_EXTRA2_1, GameProfileData.Parms.OGL_EXTRA2_1,
        GameProfileData.Parms.D3D_EXTRA2_2, GameProfileData.Parms.OGL_EXTRA2_2,
        GameProfileData.Parms.D3D_EXTRA2_3, GameProfileData.Parms.OGL_EXTRA2_3,
        GameProfileData.Parms.D3D_EXTRA2_4, GameProfileData.Parms.OGL_EXTRA2_4,
        GameProfileData.Parms.D3D_EXTRA2_5, GameProfileData.Parms.OGL_EXTRA2_5,
        GameProfileData.Parms.D3D_EXTRA2_6, GameProfileData.Parms.OGL_EXTRA2_6,
        GameProfileData.Parms.D3D_EXTRA2_7, GameProfileData.Parms.OGL_EXTRA2_7,
        GameProfileData.Parms.D3D_EXTRA2_8, GameProfileData.Parms.OGL_EXTRA2_8,
        GameProfileData.Parms.D3D_EXTRA2_9, GameProfileData.Parms.OGL_EXTRA2_9,
        GameProfileData.Parms.D3D_EXTRA2_10, GameProfileData.Parms.OGL_EXTRA2_10,


  };
    #endregion CR2GP_MAP

  private static Hotkeys.SystemHotkey m_systemHotkey = null;

  public static Hotkeys.SystemHotkey sys_hotkeys {
    get {
      if (m_systemHotkey == null)
        m_systemHotkey = new Hotkeys.SystemHotkey();
      return m_systemHotkey;
    }
  }

  public static void init_globals() {
    try {
      StreamReader sr = new StreamReader("etc/version.txt");
      app_version = sr.ReadLine();
      sr.Close();
    } catch {}

  }

  static bool m_applog_firstuse = true;
  public static void applog(string msg) {
    System.DateTime dat = System.DateTime.Now;
    if (m_applog_firstuse) {
      m_applog_firstuse = false;
      
      FileInfo fi = new FileInfo("app.log");
      if (fi.Exists && fi.Length > 1 * 1024 * 1024) {
        fi.Delete();
      }

      //File.Delete("app.log");
    }
#if true
    AppEventLog.log_app(msg);
#else
    try {
      StreamWriter stw = new StreamWriter("app.log", true, System.Text.Encoding.ASCII);
      stw.WriteLine(dat.ToLongTimeString() + "," + dat.Millisecond.ToString() + ": " + msg);
      stw.Close();
    } catch {}
#endif
  }
}

namespace TDProf.App {

  public class AppContext {
    public readonly TDProf.DriverSettings.ConfigRecord cr;
    public TDProf.Profiles.GameProfiles gp;
    public AppConfig ac;
    public readonly DisplayInfo di;
    public readonly AppMain.CommandLine cl;
    public readonly bool forbid_registry_writes = false; // force readonly on entire session
    public readonly string name_gp_cfg_file;
    public readonly string name_cr_cfg_file;
    public StreamWriter log_registry_matching_sw = null;

    public void reinit_gp_from_file() {
      gp = GameProfiles.create_from_file(name_gp_cfg_file);
    }

    public AppContext(DisplayInfo a_di, AppMain.CommandLine a_cl,
      string gp_cfg, string cr_cfg) {
      name_gp_cfg_file = gp_cfg;
      name_cr_cfg_file = cr_cfg;

      cl = a_cl;
      ac = new AppConfig(cl.optarg_run_profile != null); // do not save config on non-gui

      if (cl.optarg_lang != null) {
        app_change_lang(cl.optarg_lang);
      } else if (ac.app_lang != "" && ac.app_lang.ToLower() != "auto") {
        app_change_lang(ac.app_lang);
      }


      di = a_di; 
      cr = TDProf.DriverSettings.ConfigRecord.create_from_file(cr_cfg, a_di);
      gp = GameProfiles.create_from_file(gp_cfg);
      cr.flag_registry_readonly = ac.reg_readonly;
      GC.Collect();


      bool specfile_changed = (ac.app_specfile_checksum != 0 && ac.app_specfile_checksum != cr.specfile_checksum);
      
      ac.app_specfile_checksum = cr.specfile_checksum;

      if (a_cl.optarg_specfile != null) {
        cr.flag_registry_readonly = forbid_registry_writes = true;
      } else if (specfile_changed) {
#if AUTO_SAFE_MODE
        ac.reg_readonly = cr.flag_registry_readonly = true;
        MessageBox.Show(G.loc.msgbox_txt_safe_mode_reenabled, G.loc.msgbox_title_safe_mode_reenabled);
#endif
      }
#if false
      string log_file = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) + @"\tdprof-rmlog.txt";
      log_registry_matching_sw = new StreamWriter(log_file, true);
#endif
    }
    
  
    #region Localization    
    static System.Globalization.CultureInfo default_cultureInfo = Thread.CurrentThread.CurrentUICulture;

    public void app_change_lang (string lang) {
      string l = lang.ToLower();

      if (l == "de" || l == "deutsch") {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
        ac.app_lang = "Deutsch";

      } else if (l == "en" || l == "english") {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        ac.app_lang = "English";

      } else if (l == "auto" || l == "default") {
        Thread.CurrentThread.CurrentUICulture = default_cultureInfo;
        ac.app_lang = "Auto";

      }
      G.loc = TDProf.AppLocale.create();
    }
   #endregion 
  }

  /// <summary>
  /// Summary description for AppMain.
  /// </summary>
  public class AppMain {
    /// <summary>
    /// m_ax will be mainly constructed by run() method. This should be changed.
    /// </summary>
    private AppContext m_ax;
    string m_gp_config_file = "profiles.cfg";

    #region Command Line Parameters
    /// <summary>
    /// parse commandline. provide parsed options as public member.
    /// </summary>
    public class CommandLine {
      public string optarg_device_regkey = null;
      public string optarg_driver_regkey = null;
      public string optarg_profile = null;
      public string optarg_run_profile = null;
      public string optarg_alt_exe = null;
      public string optarg_specfile = null;
      public bool opt_force_ati = false;
      public string optarg_lang = null;
      public bool opt_run_restore = true; // restore 3D Settings after game ends (nongui only)
      public bool opt_restore_now = false;
      public string unkown_args = "";
      public bool opt_run_and_pass = false;
      public bool opt_iconic = false;
      public bool opt_tray = false;
      public bool opt_screenshots = false;
      public bool opt_selftest = false;

      public CommandLine(string[] args) {
        for (int i=0; i < args.Length; ++i) {
          if (args[i].ToLower() == "-driver_key" && args.Length > i+1)
            optarg_driver_regkey = args[++i];
          else if (args[i].ToLower() == "-device_key" && args.Length > i+1)
            optarg_device_regkey = args[++i];
          else if (args[i].ToLower() == "-profile" && args.Length > i+1)
            optarg_profile = args[++i];
          else if (args[i].ToLower() == "-run" && args.Length > i+1)
            optarg_run_profile = args[++i];
          else if (args[i].ToLower() == "-run_and_pass" && args.Length > i+1) {
            optarg_run_profile = args[++i];
            opt_run_and_pass = true;
          }
          else if (args[i].ToLower() == "-alt_exe" && args.Length > i+1)
            optarg_alt_exe = args[++i];
          else if (args[i].ToLower() == "-specfile" && args.Length > i+1)
            optarg_specfile = args[++i];
          else if (args[i].ToLower() == "-lang" && args.Length > i+1)
            optarg_lang = args[i+1].ToLower();
          else if (args[i].ToLower() == "-quit")
            return;
          else if (args[i].ToLower() == "-no_restore")
            opt_run_restore = false;
          else if (args[i].ToLower() == "-force_ati")
            opt_force_ati = true;
          else if (args[i].ToLower() == "-restore_now")
            opt_restore_now = true;
          else if (args[i].ToLower() == "-iconic")
            opt_iconic = true;
          else if (args[i].ToLower() == "-tray")
            opt_tray = true;
          else if (args[i].ToLower() == "-screenshots")
            opt_screenshots = true;
          else if (args[i].ToLower() == "-selftest")
            opt_selftest = true;
          else if (args[i].ToLower() == "-debug-throw")
             ((string)null).ToLower();
          else if (args[i] == "--") {
            while (++i < args.Length) {
              opt_run_and_pass = true;
              if (!args[i].StartsWith("\"") && args[i].IndexOf(" ") != -1)
                unkown_args += " \"" + args[i] + "\"";
              else
                unkown_args += " " + args[i];
            }
          } else {
            if (unkown_args.Length > 0)
              unkown_args += " ";
            unkown_args += args[i];
          }
        }
      }
    }
    #endregion

    /// <summary>
    /// Make sure we have a valid specfile to load. If not, throw a FatalError containing info about specfiles
    /// </summary>
    /// <param name="cl">name of forced_specfile</param>
    /// <param name="di_">data to tell the user which specfile name he needs for his chip</param>
    /// <param name="sf_">name of best suitable existing specfile or null</param>
    private void check_if_specfile_exists(CommandLine cl, DisplayInfo di_, SpecFile sf_) {
      if (cl.optarg_specfile != null && Utils.file_exists(cl.optarg_specfile))
        return;

      if (sf_.config_file_name == null)
        throw new FatalError(string.Format(G.loc.em_matching_specfile_does_not_found,
          di_.get_vendor_id(),
          di_.get_device_id(),
          Environment.OSVersion.Version.Major.ToString()));
    }

    /// <summary>
    /// Contains the entry point Main().
    /// Does all the ini stuff. Contains the the non-GUI version (needed for starting links).
    /// Starts GUI if necessary and pass to it the created AppContext.
    /// </summary>
    /// <param name="args"></param>
    public AppMain(string[] args) {

      G.init_globals();

      CommandLine cl_ = new CommandLine(args);
      DisplayInfo di_ = new DisplayInfo(cl_.optarg_driver_regkey, cl_.optarg_device_regkey);
      G.di = di_;
      SpecFile sf_ = new SpecFile(di_);

      if (cl_.optarg_specfile != null && !Utils.file_exists(cl_.optarg_specfile)) {
        MessageBox.Show(string.Format(G.loc.em_fmt_provided_specfile_0_not_exists, cl_.optarg_specfile),
          G.loc.msgbox_title_user_error);
        throw new FatalError(G.loc.em_file_not_found);
      }
      check_if_specfile_exists(cl_, di_, sf_); // throws FatalError

      G.spec_name = sf_.spec_name.Replace("usr-", "");
      // XXX
      if (cl_.optarg_specfile != null) {
        G.spec_name = cl_.optarg_specfile.Replace("-win4.cfg", "").Replace("-win5.cfg", "").Replace("-win.cfg", "").Replace("specfiles/", "").Replace("usr-specfiles/", "");
      }
      G.ax = m_ax = new AppContext(di_, cl_, m_gp_config_file,
        Utils.alt_string(cl_.optarg_specfile, sf_.config_file_name));


    }


    void restore_now() {
      GameProfileData gpd;
      GameProfileData def_gpd;

      gpd = m_ax.gp.get_profile(m_ax.cl.optarg_profile);

      
      if (m_ax.ac.auto_restore_to_default_profile
        && (def_gpd = m_ax.gp.get_profile(m_ax.ac.prof_default)) != null) {
        m_ax.cr.apply_prof(def_gpd, false);

      } else {
        AutoRestore ar = AutoRestore.create_from_file("3d_settings_saved_by_run.bin");
        ar.restore_state(m_ax.cr);
      }

#if ResClkKindByProf
      if (gpd.clocking_restore_kind != GameProfileData.EClockKinds.PARENT)
#else
      if (m_ax.ac.clocking_restore_kind != GameProfileData.EClockKinds.PARENT)
#endif
        m_ax.cr.apply_prof_clocking_only(gpd, true);

      PrePostCommands.run_post_commands(gpd);
    }

    /// <summary>
    /// this code path starts the app without any GUI.
    ///  It usually executes a single profile (Apply/Img/Run) and quits.
    /// </summary>
    void run_nogui() {
      GameProfileData gpd = null;

      // warn user about Safe Mode
      if (m_ax.cr.flag_registry_readonly)
        MessageBox.Show(G.loc.em_safe_mode_enabled_nongui, G.app_name + "");

      // restore and quit
      if (m_ax.cl.opt_restore_now) {
        restore_now();
        return;
      }

      // get profile by name provided by user
      if ((gpd = m_ax.gp.get_profile(m_ax.cl.optarg_run_profile)) == null)
        throw new FatalError(string.Format(G.loc.em_profile_does_not_exist, m_ax.cl.optarg_run_profile));

      App.AppRunProfile arp = new App.AppRunProfile(gpd, null);
      arp.apply_profile();
      arp.run_profile();
    }

    /// <summary>
    /// Creates new AppMain object and calls its run() method.
    /// Catches the app specifig FatalError Exception. This exception is just an exit
    /// if a known fatal errors occurs, like a missing config file.
    /// Unkown exceptions just fall through, so the user can attach a debugger.
    /// </summary>
    [STAThread]
    static int Main(string[] args) {

      // Change current directory to install directory if not required
#if false
        System.Reflection.Assembly ass = System.Reflection.Assembly.GetCallingAssembly();
      G.app_install_directory = Utils.extract_directory(ass.Location);
#else
      string arg0 = Environment.GetCommandLineArgs()[0];
      G.app_install_directory = Utils.extract_directory(arg0);
#endif

      if (!File.Exists("tdprof.exe"))
        Directory.SetCurrentDirectory(G.app_install_directory);

      G.m_config_in_isolated_storage = File.Exists("multi_user_config");


#if ! NOTRY      
      try {
#endif
        AppMain app = new AppMain(args);
        return app.run(args);
#if ! NOTRY  
      }
      catch (FatalError ex) {
        TDProf.MessageForm.open_dialog(G.loc.em_cant_continue
          + (ex.Message == null ? "" : ": " + ex.Message),
          G.loc.msgbox_title_unrecoverable_error).resize(640, 320).ShowDialog();
        Environment.ExitCode = 1;
        return 1;
      }
#if ! DEBUG
      catch (Exception ex) {
        TDProf.MessageForm.open_dialog(
          string.Format(G.loc.em_fmt_unhandled_execption_0_occured, ex.Message)
          + "\r\nTargetSite:\r\n" + ex.TargetSite + "\r\n"
          + "\r\nStackTrace:\r\n" + ex.StackTrace
          ,
          G.loc.msgbox_title_unhandled_exception).resize(640, 320).ShowDialog();    
      }
      return 0;
#endif // DEBUG
#endif // NOTRY
    }


    /// <summary>
    /// creates configuration objects and forks either to GUI or non-GUI path.
    /// </summary>
    /// <param name="args">The commandline args from Main()</param>
    /// <returns>0 for success. 1 for failure (Currently alway returns 0. Exceptions are used for failures)</returns>
    int run(string[] args) {

      // overide registry key by user option
      if (m_ax.cl.optarg_driver_regkey != null && !m_ax.cl.optarg_driver_regkey.StartsWith("-"))
        m_ax.cr.driver_regkey = m_ax.cl.optarg_driver_regkey; //TODO

      // decide between gui/non-gui version by user option
      bool use_gui = !(m_ax.cl.optarg_run_profile != null || m_ax.cl.opt_restore_now);
      bool protect_cfg_by_mutex = !m_ax.cl.opt_screenshots;

      if (use_gui) {
        string guid = G.ax.ac.config_guid;
        if (guid == "") {
          guid = G.ax.ac.config_guid = System.Guid.NewGuid().ToString();
          G.ax.ac.save_config();
        }
        bool createdNew = true;
        if(protect_cfg_by_mutex)
          G.gui_mutex = new Mutex(true, "3DProf Config:" + guid, out createdNew);
        if (createdNew)
          GUI.FormMain.start_gui(m_ax);
        else
          MessageBox.Show("3DProf is already running.", "3DProf - Cannot Start");

        // close gui_mutex in case explicitly. If not, a waiter thread could keep it open.
        if (G.gui_mutex != null)
           G.gui_mutex.Close();

      } else {
        run_nogui();
      }

      return 0;
    }


  }
}

namespace TDProf {

  class  swap {

    /// <summary>
    /// minimize current usage of physical RAM by our process (like swapping it out)
    /// This is the same like Windows explorer does at minimizing
    /// </summary>
    static public void swap_me_out() {
      if (Environment.OSVersion.Platform == System.PlatformID.Win32NT) //TODO: ignores NT3/4
        Win32.Kernel32.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
    }

  };
}