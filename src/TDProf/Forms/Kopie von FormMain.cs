#define GC_COLLECT
#define TAB_EXP
#define TRAY_ON_CLOSEBUTTON

using System;
using System.Drawing;
using System.Net;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using TDProf.DriverSettings;
using TDProf.Profiles;
using TDProf.Util;
using TDProf.App;
using Microsoft.Win32;

namespace TDProf.GUI {

  /// <summary>
  /// Summary description for FormMain.
  /// </summary>
  public class FormMain : System.Windows.Forms.Form {
    #region Private Data
    readonly public AppContext ax;
    AutoRestore ar_apply = new AutoRestore();
    //Form_Settings form_opts;
    readonly System.Windows.Forms.Label[] labels_modes;
    FM_GameProfiles m_gps;
    ProfileHotkeys m_profHotkeys = null;
    App.AppHotkeys m_appHotkeys  = null;
    bool m_iconify_me = false;
    bool m_send_me_to_tray = false;
    bool m_ignore_close_just_hide = true;
    bool m_tray_menu_updated = false; // if false, tray menu will be initialized at popup
    bool m_main_menu_updated = false;
    bool m_textbox_focus = false; // if a textbox has focus
    bool m_shift_pressed = false; // used to recognize a Shift-Click on CloseBox
    readonly int TRAY_ICON_MS_BEFORE_HIDE = 500;

    GameProfileData m_gpd = null;
    int m_gpi             = -1;
    GameProfileData sel_gpd { get { return m_gpd; } }
    int sel_gpi { get { return m_gpi; } }

    class combo_user_data {
      public enum Kind { CURR, PROF };
      public int idx;
      public Kind kind;
      public bool disable_updating = false;
      public Label row_label, column_label;
      public ConfigRecord.Mode cr_mode;
      //public combo_user_data(int index) { idx = index; row_label = null; column_label = null; }
      public combo_user_data(Kind k, int index, Label row, Label column, ConfigRecord.Mode crm) {
        kind = k; idx = index; row_label = row; column_label = column; cr_mode = crm;}
    }; 

    struct data_save_tooltips {
      public string
        combo_prof_img,
        text_prof_exe_path,
        text_prof_exe_args;
      
      public data_save_tooltips(FormMain frm) {
        combo_prof_img = frm.toolTip.GetToolTip(frm.combo_prof_img);
        text_prof_exe_path = frm.toolTip.GetToolTip(frm.text_prof_exe_path);
        text_prof_exe_args = frm.toolTip.GetToolTip(frm.text_prof_exe_args);
      }
    };
    /// <summary>
    /// A place to save some tooltips created by designer.
    /// This is used for some initially empty text fields to
    /// have the original tooltip if empty and show the content in tooltip if non-empty.
    /// Showing the content is useful, if it does not fit in the text control.
    /// </summary>
    data_save_tooltips toolTip_orgs;
    readonly System.Windows.Forms.ComboBox[] combos_curr_modes;
    readonly System.Windows.Forms.ComboBox[] combos_prof_modes;
    CheckBox[] checks_curr_modes;
    private System.Windows.Forms.MenuItem menuItem24;
    private System.Windows.Forms.MenuItem menu_profs_templates_turnTo2D;
    private System.Windows.Forms.MenuItem menu_profs_templates_clockOnly;
    private System.Windows.Forms.TabPage tab_files;
    private System.Windows.Forms.Button button_clocking_disable;
    private System.Windows.Forms.MenuItem menu_profs_exim;
    private System.Windows.Forms.Splitter splitter_prof_gameExe;
    private System.Windows.Forms.Splitter splitter_clocking;
    private System.Windows.Forms.GroupBox group_extra_ogl_2;
    private System.Windows.Forms.GroupBox group_extra_d3d_2;
    private System.Windows.Forms.ComboBox combo_clocking_prof_presets;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button button_clocking_curr_preSlow;
    private System.Windows.Forms.Button button_clocking_curr_preNormal;
    private System.Windows.Forms.Button button_clocking_curr_preUltra;
    private System.Windows.Forms.Button button_clocking_curr_preFast;
    private System.Windows.Forms.MenuItem menuItem26;
    private System.Windows.Forms.MenuItem menu_tray_clocking_pre_slow;
    private System.Windows.Forms.MenuItem menu_tray_clocking_pre_fast;
    private System.Windows.Forms.MenuItem menu_tray_clocking_pre_normal;
    private System.Windows.Forms.MenuItem menu_tray_clocking_pre_ultra;
    private System.Windows.Forms.MenuItem menu_opts_hotkeys;
    private System.Windows.Forms.MenuItem menuItem27;
    private System.Windows.Forms.MenuItem menu_help_manual;
    private System.Windows.Forms.ToolBar toolBar1;
    private System.Windows.Forms.ToolBarButton toolButton_settings;
    private System.Windows.Forms.ToolBarButton toolButton_hotkeys;
    private System.Windows.Forms.ToolBarButton toolButton_editGameCfg;
    private System.Windows.Forms.ToolBarButton toolButton_exploreGameFolder;
    private System.Windows.Forms.ToolBarButton toolButton_prof_commands;
    private System.Windows.Forms.ToolBarButton toolBarButton1;
    private System.Windows.Forms.ToolBarButton toolBarButton2;
    private System.Windows.Forms.ToolBarButton toolBarButton3;
    private System.Windows.Forms.ToolBarButton toolButton_tools_regEdit;
    private System.Windows.Forms.ToolBarButton toolBarButton4;
    private System.Windows.Forms.ToolBarButton toolButton_help_onlineManual;
    private System.Windows.Forms.Button button_prof_restore;
    private System.Windows.Forms.MenuItem menuItem28;
    private System.Windows.Forms.ToolBarButton toolBarButton5;
    private System.Windows.Forms.ToolBarButton toolButton_compact;
    private System.Windows.Forms.CheckBox check_prof_shellLink;
    private System.Windows.Forms.TabPage tab_exp;
    private System.Windows.Forms.ColumnHeader columnHeader_name;
    private System.Windows.Forms.ColumnHeader columnHeader_profile;
    private System.Windows.Forms.ColumnHeader columnHeader_driver;
    private System.Windows.Forms.ListView list_3d;
    private System.Windows.Forms.ComboBox combo_ind_modeVal;
    private System.Windows.Forms.Label label_ind_mode;
    private System.Windows.Forms.CheckBox check_ind_d3d;
    private System.Windows.Forms.CheckBox check_ind_ogl;
    private System.Windows.Forms.PictureBox picture_ind_d3d;
    private System.Windows.Forms.PictureBox picture_ind_ogl;
    private System.Windows.Forms.MenuItem menuItem25;
    private System.Windows.Forms.Panel panel_clocking_prof_clocks;
    private System.Windows.Forms.MenuItem menu_help_log;
    private System.Windows.Forms.MenuItem menu_nvCoolBits;
    private System.Windows.Forms.MenuItem menu_nvCoolBits_clocking;
    private System.Windows.Forms.GroupBox group_clocking_current_presets;
    private System.Windows.Forms.MenuItem menu_file_debugThrow;
    private System.Windows.Forms.Button button_prof_discard;
    private System.Windows.Forms.Button button_prof_rename;
    CheckBox[] checks_prof_modes;
    #endregion

    #region INIT

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appContext"></param>
    public FormMain(AppContext appContext) {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      ax           = appContext;
      m_gps        = new FM_GameProfiles(this, ax);
      // save original content of dynamic tooltips
      toolTip_orgs = new data_save_tooltips(this);      

      // Hide unsupported features
      menu_tools_openRegedit.Visible = (Environment.OSVersion.Platform == System.PlatformID.Win32NT);


      try {
        //TODO: Security exceptions can occur on Win9x
        this.dialog_prof_choose_exec.RestoreDirectory = true;
      } catch {}

      // register callback for profile changes
      GameProfiles.Unsaved += new System.EventHandler(game_profile_has_changed);
 
      init_3d_combos(ref combos_curr_modes, ref combos_prof_modes, ref labels_modes);
      init_clocking();
      init_nvidia_specific();
      init_ati_radeon_specific();
      init_hotkeys();

      m_iconify_me = ax.cl.opt_iconic;
      m_send_me_to_tray = ax.cl.opt_tray;

    }

    /// <summary>
    /// initialize lookup tables and event handlers for combo boxes
    /// </summary>
    /// <param name="combos_curr">requried to acess readonly member array from outside the ctor</param>
    /// <param name="combos_prof">requried to acess readonly member array from outside the ctor</param>
    /// <param name="labels">requried to acess readonly member array from outside the ctor</param>
    void init_3d_combos(ref ComboBox[] combos_curr, ref ComboBox[] combos_prof, ref Label[] labels) {
      #region init lookup tables
      // we keep references to objects in arrays, so we can iterate and access related objects 
      // on same indices
      combos_curr
        = new ComboBox[] {
                           combo_d3d_fsaa_mode,
                           combo_ogl_fsaa_mode,
                           combo_d3d_aniso_mode,
                           combo_ogl_aniso_mode,
                           combo_d3d_vsync_mode,
                           combo_ogl_vsync_mode,
                           combo_d3d_qe_mode,
                           combo_ogl_qe_mode,
                           combo_d3d_lod_bias,
                           combo_ogl_lod_bias,
                           combo_d3d_prerender_frames,
                           combo_ogl_prerender_frames,
                           combo_extra_curr_d3d_1,
                           combo_extra_curr_ogl_1,
                           combo_extra_curr_d3d_2,
                           combo_extra_curr_ogl_2,
                           combo_extra_curr_d3d_3,
                           combo_extra_curr_ogl_3,
                           combo_extra_curr_d3d_4,
                           combo_extra_curr_ogl_4,
                           combo_extra_curr_d3d_5,
                           combo_extra_curr_ogl_5,
                           combo_extra_curr_d3d_6,
                           combo_extra_curr_ogl_6,
                           combo_extra_curr_d3d_7,
                           combo_extra_curr_ogl_7,
                           combo_extra_curr_d3d_8,
                           combo_extra_curr_ogl_8,


                           combo_extra2_curr_d3d_1,
                           combo_extra2_curr_ogl_1,
                           combo_extra2_curr_d3d_2,
                           combo_extra2_curr_ogl_2,
                           combo_extra2_curr_d3d_3,
                           combo_extra2_curr_ogl_3,
                           combo_extra2_curr_d3d_4,
                           combo_extra2_curr_ogl_4,
                           combo_extra2_curr_d3d_5,
                           combo_extra2_curr_ogl_5,
                           combo_extra2_curr_d3d_6,
                           combo_extra2_curr_ogl_6,
                           combo_extra2_curr_d3d_7,
                           combo_extra2_curr_ogl_7,
                           combo_extra2_curr_d3d_8,
                           combo_extra2_curr_ogl_8,
      };

      combos_prof
        = new ComboBox[] {
                           combo_prof_d3d_fsaa_mode,
                           combo_prof_ogl_fsaa_mode,
                           combo_prof_d3d_aniso_mode,
                           combo_prof_ogl_aniso_mode,
                           combo_prof_d3d_vsync_mode,
                           combo_prof_ogl_vsync_mode,
                           combo_prof_d3d_qe_mode,
                           combo_prof_ogl_qe_mode,
                           combo_prof_d3d_lod_bias,
                           combo_prof_ogl_lod_bias,
                           combo_prof_d3d_prerender_frames,
                           combo_prof_ogl_prerender_frames,
                           combo_extra_prof_d3d_1,
                           combo_extra_prof_ogl_1,
                           combo_extra_prof_d3d_2,
                           combo_extra_prof_ogl_2,
                           combo_extra_prof_d3d_3,
                           combo_extra_prof_ogl_3,
                           combo_extra_prof_d3d_4,
                           combo_extra_prof_ogl_4,
                           combo_extra_prof_d3d_5,
                           combo_extra_prof_ogl_5,
                           combo_extra_prof_d3d_6,
                           combo_extra_prof_ogl_6,
                           combo_extra_prof_d3d_7,
                           combo_extra_prof_ogl_7,
                           combo_extra_prof_d3d_8,
                           combo_extra_prof_ogl_8,

                           combo_extra2_prof_d3d_1,
                           combo_extra2_prof_ogl_1,
                           combo_extra2_prof_d3d_2,
                           combo_extra2_prof_ogl_2,
                           combo_extra2_prof_d3d_3,
                           combo_extra2_prof_ogl_3,
                           combo_extra2_prof_d3d_4,
                           combo_extra2_prof_ogl_4,
                           combo_extra2_prof_d3d_5,
                           combo_extra2_prof_ogl_5,
                           combo_extra2_prof_d3d_6,
                           combo_extra2_prof_ogl_6,
                           combo_extra2_prof_d3d_7,
                           combo_extra2_prof_ogl_7,
                           combo_extra2_prof_d3d_8,
                           combo_extra2_prof_ogl_8,
      };
      labels = new Label[] {
                             label_fsaa_mode,
                             label_fsaa_mode,
                             label_aniso_mode,
                             label_aniso_mode,
                             label_vsync_mode,
                             label_vsync_mode,
                             label_quality,
                             label_quality,
                             label_lod_bias,
                             label_lod_bias,
                             label_prerender_frames,
                             label_prerender_frames,
                             label_extra_combo_d3d_1,
                             label_extra_combo_ogl_1,
                             label_extra_combo_d3d_2,
                             label_extra_combo_ogl_2,
                             label_extra_combo_d3d_3,
                             label_extra_combo_ogl_3,
                             label_extra_combo_d3d_4,
                             label_extra_combo_ogl_4,
                             label_extra_combo_d3d_5,
                             label_extra_combo_ogl_5,
                             label_extra_combo_d3d_6,
                             label_extra_combo_ogl_6,
                             label_extra_combo_d3d_7,
                             label_extra_combo_ogl_7,
                             label_extra_combo_d3d_8,
                             label_extra_combo_ogl_8,

                             label_extra2_combo_d3d_1,
                             label_extra2_combo_ogl_1,
                             label_extra2_combo_d3d_2,
                             label_extra2_combo_ogl_2,
                             label_extra2_combo_d3d_3,
                             label_extra2_combo_ogl_3,
                             label_extra2_combo_d3d_4,
                             label_extra2_combo_ogl_4,
                             label_extra2_combo_d3d_5,
                             label_extra2_combo_ogl_5,
                             label_extra2_combo_d3d_6,
                             label_extra2_combo_ogl_6,
                             label_extra2_combo_d3d_7,
                             label_extra2_combo_ogl_7,
                             label_extra2_combo_d3d_8,
                             label_extra2_combo_ogl_8,


      };
      #endregion

      // set common eventhandler for ComboBoxes

      foreach (ComboBox cb in combos_curr_modes) {
        cb.MouseEnter += new System.EventHandler(this.combos_mouse_enter);
        cb.MouseLeave += new System.EventHandler(this.combos_mouse_leaves);
        cb.DropDown   += new System.EventHandler(this.combos_DropDown);
        cb.SelectedIndexChanged += new System.EventHandler(this.combos_SelectedIndexChanged);
      }

      foreach (ComboBox cb in combos_prof_modes) {
        cb.MouseEnter += new System.EventHandler(this.combos_mouse_enter);
        cb.MouseLeave += new System.EventHandler(this.combos_mouse_leaves);
        cb.DropDown   += new System.EventHandler(this.combos_DropDown);
        cb.SelectedIndexChanged += new System.EventHandler(this.combos_prof_modes_SelectedIndexChanged);
        cb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.combos_prof_mouse_down);
        cb.EnabledChanged += new System.EventHandler(this.combos_prof_modes_EnabledChanged);
      }
    }

    private void combos_DropDown(object sender, System.EventArgs e) {
      if (ax.ac.gui_mover_feedback) {
        Control ctrl = (Control)sender;
        combo_user_data cud = ((combo_user_data)ctrl.Tag);

        highlight_3d_label(cud.column_label, false);
        highlight_3d_label(cud.row_label, false);
      }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if (components != null) {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }


    public static void start_gui(AppContext appContext) {
      G.applog("** entering FormMain.start_gui(...)");
      FormMain form = new FormMain(appContext);

      //form.form_opts = new Form_Settings(form, form.ax);
      if (form.ax.forbid_registry_writes)
        System.Windows.Forms.MessageBox
          .Show(string.Format(G.loc.em_safe_mode_enabled_gui_forced_specfile, "-specfile"), G.app_name);
      else if (form.ax.cr.flag_registry_readonly && form.ax.ac.warn_about_safe_mode) {
        System.Windows.Forms.DialogResult dr =
          System.Windows.Forms.MessageBox
          .Show(G.loc.em_safe_mode_enabled_gui_new, G.app_name, System.Windows.Forms.MessageBoxButtons.YesNoCancel);
        if (dr == System.Windows.Forms.DialogResult.Yes) {
          form.ax.ac.reg_readonly = false;
          form.regreadonly_change_for_session(false);
        } else if (dr == System.Windows.Forms.DialogResult.No) {
          form.ax.ac.warn_about_safe_mode = false;
        } else if (dr == System.Windows.Forms.DialogResult.Cancel) {
        }
      }
#if GC_COLLECT
      GC.Collect();
#endif
      G.root_form = form;
      G.applog("*** in FormMain.start_gui(...): Application.Run()");
      Application.Run(form);
    }
					
    private void remove_menuImage_recursively(Menu.MenuItemCollection mic, bool ownerDraw) {
      if (mic == null)
        return;
      foreach(MenuItem mi in mic) {
        mi.OwnerDraw = ownerDraw;
        if (mic != null)
          remove_menuImage_recursively(mi.MenuItems, ownerDraw); // recursion
      }
    }


    #region form loading time
    private void FormMain_Load(object sender, System.EventArgs e) {

      string env_debug = Environment.GetEnvironmentVariable("TDPROF_DEBUG");
      if (env_debug != null) {
        this.menu_file_debugCrashMe.Visible = true;
        this.menu_exp.Visible = true;
      }

      this.menu_help_manual.Enabled = this.menu_help_manual.Visible = File.Exists(G.ManualIndexHTML);

      if (ax.forbid_registry_writes)
        menu_opts_regreadonly.Enabled = false;

      this.toolTip.Active
        = this.menu_help_tooltips.Checked 
        = ax.ac.gui_show_tooltips;
      init_from_cr(false);
#if TAB_EXP
      init_tab_exp();
#else
      this.tabCtrl.Controls.Remove(tab_exp);
      this.tabCtrl.SelectedIndex = 0;
#endif
      init_from_gp();
      update_current_combos_from_cr();

      
      this.button_prof_set_enable_state();

#if GC_COLLECT
      GC.Collect();
#endif
      swap.swap_me_out();
      regreadonly_change_for_session(ax.cr.flag_registry_readonly);
      button_prof_save.Enabled = button_prof_discard.Enabled = button_prof_discard.Enabled = false;
      menu_opts_multiUser.Checked = G.config_in_isolated_storage;
      menu_opts_autoStart.Checked = auto_start(true, false);

      if (ax.ac.gui_3d_checkboxes)
        checkboxing_combos();

      menu_tray_stayInTray.Checked     = ax.ac.gui_show_tray_icon;
      menu_tray_hideOnCloseBox.Checked = m_ignore_close_just_hide = ax.ac.gui_hide_on_closeBox;
      menu_opt_3DCheckBoxes.Checked    = ax.ac.gui_3d_checkboxes;
      menu_opts_menuIcons.Checked      = ax.ac.gui_show_menu_icons;
      bool menuIcons = menu_opts_menuIcons.Checked = ax.ac.gui_show_menu_icons;
      foreach(MenuItem mi in mainMenu.MenuItems)
        remove_menuImage_recursively(mi.MenuItems, menuIcons);


      check_lsc(true);
      check_mw(true);

      int[] clock_limits = ax.ac.clocking_limits; //Clocking.get_limits();
      track_clocking_curr_mem.Minimum = (int)clock_limits[0];
      track_clocking_curr_mem.Maximum = (int)clock_limits[1];
      track_clocking_curr_core.Minimum = (int)clock_limits[2];
      track_clocking_curr_core.Maximum = (int)clock_limits[3];


#if !DEBUG
      this.menu_profs_exim.Visible = false;    
#endif

      //  form_iconify_or_deiconify((m_iconify_me || m_send_me_to_tray), m_send_me_to_tray);
      gui_compact(toolButton_compact.Pushed = ax.ac.gui_compact);
    }

    #endregion

    /// <summary>
    /// Set a profile ComboBox to reflect given mode.
    /// 
    /// Note: if a mode is not known by video card, only the text will be displayed
    /// </summary>
    /// <param name="i">index of combo box in combos_prof_modes array</param>
    /// <param name="mode_idx">index of item in combo box, or -1</param>
    /// <param name="gp_mode_name">text showed in combo box if mode_idx is -1</param>
    private void set_index_or_text_in_profile_comboBox(int i, int mode_idx, string gp_mode_name) {
      if (0 <= mode_idx) {
        combos_prof_modes[i].DropDownStyle = ComboBoxStyle.DropDownList;
        combos_prof_modes[i].SelectedIndex = mode_idx;
        combos_prof_modes[i].ForeColor     = System.Drawing.SystemColors.WindowText;

      } else if (gp_mode_name != null) {
        combos_prof_modes[i].SelectedIndex = -1;
        combos_prof_modes[i].DropDownStyle = ComboBoxStyle.DropDown;
        combos_prof_modes[i].ForeColor     = System.Drawing.Color.Red;
        combos_prof_modes[i].Text          = gp_mode_name; //TODO: TEST
      }
    }

    private void init_update_timer(bool force_off) {
      int ms = ax.ac.cr_timer_update;
      timer_updateCr.Enabled = (ms > 0 && !force_off);
      timer_updateCr.Interval = (ms < 0) ? ms * -1 : ms;
    }


    #endregion INIT

    #region MISC

    string old_group_3d_text = null;
    internal void regreadonly_change_for_session(bool readOnly) {
      if (old_group_3d_text == null)
        old_group_3d_text = group_main_d3d.Text;
      ax.cr.flag_registry_readonly
        = this.menu_opts_regreadonly.Checked
        = readOnly;

      foreach(ComboBox box in combos_curr_modes)
        box.Enabled = !readOnly;

      group_main_d3d.Text = (!readOnly ? old_group_3d_text : old_group_3d_text + " - Safe Mode!");
      group_extra_d3d.Text= (!readOnly ? "" : "Safe Mode!");
      group_extra_ogl.Text= (!readOnly ? "" : "Safe Mode!");
#if false
      group_main_d3d.ForeColor
        = group_extra_d3d.ForeColor
        = group_main_ogl.ForeColor
        = group_extra_ogl.ForeColor
        = (readOnly
        ? System.Drawing.Color.Red
        : System.Drawing.SystemColors.ControlText);
#endif
#if false
      label_d3d.ForeColor
        = label_ogl.ForeColor
        = label_extra_curr_d3d.ForeColor
        = label_extra2_curr_d3d.ForeColor
        = label_extra_curr_ogl.ForeColor
        = label_extra2_curr_ogl.ForeColor
        = (readOnly
        ? System.Drawing.Color.Red
        : System.Drawing.SystemColors.ControlText);
      // avoid inheriting red color on safe mode
      label_d3d.ForeColor = label_ogl.ForeColor = System.Drawing.SystemColors.ControlText;
#endif

    }
    /// <summary>
    /// Set Enabled property of many buttons and menu items and other widgets according to current state
    /// </summary>
    private void button_prof_set_enable_state() {
      string s = combo_prof_names.Text;
      int prof_idx = ax.gp.get_profile_index(s);
      bool prof_selected = combo_prof_names.SelectedIndex != -1 // this does not work immediatly 
        ;//&& prof_idx != -1; //TODO: this is a hack
      bool prof_new_text = !prof_selected && s.Length > 0;
      bool prof_new_text_valid = prof_new_text 
        && (prof_idx == -1 || ax.gp.get_profile(prof_idx).spec_name != G.spec_name);

      bool alien_profile = (prof_idx != -1 && ax.gp.get_profile(prof_idx).spec_name != G.spec_name);

      button_prof_make_link.Enabled
        = menu_profile.Enabled
        = menu_profile_ini_edit.Enabled
        = button_prof_delete.Enabled = button_prof_rename.Enabled
        = prof_selected;

      // enable / disable menu Explore Game Exe Folder
      if (prof_idx != -1) {
        string exe_path = ax.gp.get_profile(prof_idx).exe_path;
        menu_prof_exploreExePath.Enabled
          = menu_prof_autoRestore.Enabled
          = menu_prof_prio.Enabled
          = menu_prof_freeMem.Enabled
          = menu_prof_detectAPI.Enabled
          = menu_prof_tdprofGD.Enabled
          = (exe_path != null && exe_path.Length > 0);

      }

      button_prof_save.Enabled = button_prof_discard.Enabled = button_prof_discard.Enabled = (GameProfiles.change_count > 0);

      //XXX//button_prof_add.Enabled = prof_new_text_valid || alien_profile;
      //XXX// button_prof_add.Text = alien_profile ? "Clone" : "Add";
    }

    /// <summary>
    /// Flag to neutralize callback of all combo_prof_ while set combos from a stored profile
    /// </summary>
    private bool combos_prof_update_in_progress = false;
    private void util_prof_enable_combo(int idx, bool enabled) {
      combos_prof_modes[idx].Enabled = enabled;
      if (sel_gpd != null)
        sel_gpd.val(G.gp_parms[idx]).Enabled = enabled;
    }

 
    /// <summary>
    ///  true if a app was stated from tray. No deiconify will be done.
    /// </summary>
    bool executed_from_tray = false;

    #endregion MISC

    #region GUI Update/Size/Feedback
    /// <summary>
    /// fill summary tab with info text about settings of current profile.
    /// Useful to check settings configured by menu items.
    /// </summary>
    private void summary_update() {
      RichTextBox tb = text_summary;
      if (sel_gpd == null)
        return;
      string s = "";
      string endl = "\r\n";
      //string dq = "\"";

      // heading
      if (sel_gpd.spec_name == "")
        s += string.Format(G.loc.fmt_summary_of_profile_0, sel_gpd.name);
      else
        s += string.Format(G.loc.fmt_summary_of_profile_0_attached_to_display_adapter_1, sel_gpd.name, sel_gpd.spec_name);
      s += endl;

      // master profile
      if (sel_gpd.include_other_profile != "") {
        s+="Master Profile: " + sel_gpd.include_other_profile + endl;
      }

      s += endl;

      Keys hotkey = m_profHotkeys.RetrieveHotkey(sel_gpd.name);
      if (hotkey != Keys.None)
        s+= "Hotkey: " + hotkey.ToString() + endl;

      // FSAA/Aniso/VSync/Quality/LOD
      s +="3D Settings (D3D / OGL)" + endl;
      s +="    Antialiasing = "
        + (sel_gpd.val(GameProfileData.Parms.D3D_FSAA).Enabled ? sel_gpd.val(GameProfileData.Parms.D3D_FSAA).Data : "--")
        + " / "
        + (sel_gpd.val(GameProfileData.Parms.OGL_FSAA).Enabled ? sel_gpd.val(GameProfileData.Parms.OGL_FSAA).Data : "--") + ", ";
      s +="    Anisotropic = "
        + (sel_gpd.val(GameProfileData.Parms.D3D_ANISO).Enabled ? sel_gpd.val(GameProfileData.Parms.D3D_ANISO).Data : "--")
        + " / "
        + (sel_gpd.val(GameProfileData.Parms.OGL_ANISO).Enabled ? sel_gpd.val(GameProfileData.Parms.OGL_ANISO).Data : "--") + endl;
      s +="    VSync = "
        + (sel_gpd.val(GameProfileData.Parms.D3D_VSYNC).Enabled ? sel_gpd.val(GameProfileData.Parms.D3D_VSYNC).Data : "--")
        + " / "
        + (sel_gpd.val(GameProfileData.Parms.OGL_VSYNC).Enabled ? sel_gpd.val(GameProfileData.Parms.OGL_VSYNC).Data : "--") + ", ";
      s +="    Quality = "
        + (sel_gpd.val(GameProfileData.Parms.D3D_QE).Enabled ? sel_gpd.val(GameProfileData.Parms.D3D_QE).Data : "--")
        + " / "
        + (sel_gpd.val(GameProfileData.Parms.OGL_QE).Enabled ? sel_gpd.val(GameProfileData.Parms.OGL_QE).Data  : "--")+ endl;
      s +="    LOD = "
        + (sel_gpd.val(GameProfileData.Parms.D3D_LOD).Enabled ? sel_gpd.val(GameProfileData.Parms.D3D_LOD).Data : "--")
        + " / "
        + (sel_gpd.val(GameProfileData.Parms.OGL_LOD).Enabled ? sel_gpd.val(GameProfileData.Parms.OGL_LOD).Data : "--") + endl;
      bool has_core_clk = sel_gpd.val(GameProfileData.Parms.CLOCKING_CORE_CLK).Enabled;
      bool has_mem_clk = sel_gpd.val(GameProfileData.Parms.CLOCKING_MEM_CLK).Enabled;

      // clocking
      if (has_core_clk || has_mem_clk || sel_gpd.clocking_kind != GameProfileData.EClockKinds.PARENT)
        s +="Clocking: "
          + ((sel_gpd.clocking_kind == GameProfileData.EClockKinds.PARENT)
          ? (has_core_clk ? "Core = " + sel_gpd.clocking_core_clock + " MHz" : "")
          + (has_core_clk && has_mem_clk ? ", " : "")
          + (has_mem_clk ? "Mem Clock = " + sel_gpd.clocking_mem_clock + " MHz" : "")
          : "Preset " + sel_gpd.clocking_kind.ToString().ToLower())
          + endl;

      // executable / args / ini file / prio / RAM to free
      if (sel_gpd.exe_path != "" || sel_gpd.game_ini_path.Length != 0) {
        s +="Game" + endl;
        if (sel_gpd.exe_path.Length != 0)
          s +="    Executable = " + sel_gpd.exe_path + endl;
        if (sel_gpd.exe_args != "")
          s +="    Exe-Args = " + sel_gpd.exe_args + endl;     
        if (sel_gpd.game_ini_path.Length != 0)
          s +="    Config-File = " + sel_gpd.game_ini_path + endl;
        if (sel_gpd.exe_process_prio != ProcessPriorityClass.Normal)
          s +="    Process Priority = " + sel_gpd.exe_process_prio.ToString() + endl;
        if (sel_gpd.exe_free_mem != 0)
          s +="    RAM to free = " + sel_gpd.exe_free_mem.ToString() + " MB" + endl;      
      }

      // image-files to mount
      if (sel_gpd.img_path.Length > 0) {
        Debug.Assert (sel_gpd.img_path.Length <= sel_gpd.img_drive_number.Length);
        for (int i=0; i < sel_gpd.img_path.Length; ++i) {
          s +="    Image at drive number " + sel_gpd.img_drive_number[i].ToString()
            + " = "
            + sel_gpd.img_path[i]
            + endl;
        }
      }


      tb.Text = s;
    }

    private void gui_update() {
      // updating GUI
      update_current_combos_from_cr();
      if (tabCtrl.SelectedTab == tab_clocking)
        clocking_get_clock(false);
      // end updating GUI
    }

    void close_me() {
      m_ignore_close_just_hide = false;
      this.Close();
    }

    void toggle_minimize_tray() {
      bool is_iconic = (WindowState == System.Windows.Forms.FormWindowState.Minimized) || !this.Visible;
      form_iconify_or_deiconify(!is_iconic, true);
    }


    private void button_color_effect(Button but, bool mover) {
      if (but.Enabled) {
        but.ForeColor = mover ? System.Drawing.Color.Blue : System.Drawing.SystemColors.WindowText;
      }		
    }
    private void button_mover_effect(Button but, bool mover) {
      if (but.Enabled) {
        //but.ForeColor = mover ? System.Drawing.Color.Blue : System.Drawing.SystemColors.WindowText;
        //but.FlatStyle = mover ? FlatStyle.System : FlatStyle.Standard;
      }		
    }

    private void form_iconify_or_deiconify(bool iconify, bool tray_icon) {
      init_update_timer(iconify);      
      if (!iconify) {
        Show();
        WindowState = System.Windows.Forms.FormWindowState.Normal;
        if (!ax.ac.gui_show_tray_icon) {
          tray_icon_visibility(false, TRAY_ICON_MS_BEFORE_HIDE);
        } else {
          tray_icon_visibility(true, 0);
        }
      } else {
        if (tray_icon) {
          Hide();
          tray_icon_visibility(true, 0);
          WindowState = System.Windows.Forms.FormWindowState.Minimized;
        } else {
          WindowState = System.Windows.Forms.FormWindowState.Minimized;
          tray_icon_visibility(ax.ac.gui_show_tray_icon, 0);
        }
#if GC_COLLECT
        GC.Collect();
#endif
        swap.swap_me_out();
      }

    }


    #endregion GUI

    #region Clocking

    void init_clocking() {
      if (!ax.ac.feature_clocking || !Clocking.clocking_ability()) {
        tabCtrl.Controls.Remove(tab_clocking);
        track_clocking_curr_core.Enabled = false;
        track_clocking_curr_mem.Enabled = false;
        tabCtrl.SelectedTab = tab_main;
      }

      // MouseWheel Trackbar Support
      this.track_clocking_prof_core.MouseWheel += new MouseEventHandler(this.track_clocking_prof_core_MouseWheel);
      this.track_clocking_prof_mem.MouseWheel += new MouseEventHandler(this.track_clocking_prof_mem_MouseWheel);
      this.track_clocking_curr_core.MouseWheel += new MouseEventHandler(this.track_clocking_curr_core_MouseWheel);
      this.track_clocking_curr_mem.MouseWheel += new MouseEventHandler(this.track_clocking_curr_mem_MouseWheel);

      button_clocking_curr_preSlow.Tag = GameProfileData.EClockKinds.PRE_SLOW;
      button_clocking_curr_preNormal.Tag = GameProfileData.EClockKinds.PRE_NORM;
      button_clocking_curr_preFast.Tag = GameProfileData.EClockKinds.PRE_FAST;
      button_clocking_curr_preUltra.Tag = GameProfileData.EClockKinds.PRE_ULTRA;
    }

    /// <summary>
    /// enable/disable core clocking widgets of this profile according to profile data.
    /// </summary>
    /// <param name="enable">true to enable, false to disable</param>
    private void clocking_enable_prof_core(bool enable) {
      if (sel_gpd == null)
        return;

      sel_gpd.val(GameProfileData.Parms.CLOCKING_CORE_CLK).Enabled
        = text_clocking_prof_core.Enabled
        = track_clocking_prof_core.Enabled
        = check_clocking_prof_core.Checked
        = enable;
    }

    /// <summary>
    /// enable/disable RAM clocking widgets of this profile according to profile data.
    /// </summary>
    /// <param name="enable">true to enable, false to disable</param>
    private void clocking_enable_prof_mem(bool enable) {
      if (sel_gpd == null)
        return;

      sel_gpd.val(GameProfileData.Parms.CLOCKING_MEM_CLK).Enabled
        = text_clocking_prof_mem.Enabled
        = track_clocking_prof_mem.Enabled
        = check_clocking_prof_mem.Checked
        = enable;
    }

    /// <summary>
    /// Get current clock from hardware and update text boxes 
    /// </summary>
    /// <param name="ignore_errors">if "false", errors like missing clocking program will cause a message box to pop up</param>
    private void clocking_get_clock(bool ignore_errors) {
      if (!ax.ac.feature_clocking || !Clocking.clocking_ability())
        return;
      float[] clocks = Clocking.clocking_get_clock(ignore_errors);
      if (clocks != null) {
        text_clocking_curr_core.Text = clocks[0].ToString();
        text_clocking_curr_mem.Text = clocks[1].ToString();
      } else {
        text_clocking_curr_core.Text = text_clocking_curr_mem.Text = "";
      }
    }

    /// <summary>
    /// Set current clock according to parameters. After setting is done, text boxes will be updated to current clock. 
    /// </summary>
    /// <param name="core_clock">Core clock in MHz</param>
    /// <param name="mem_clock">RAM clock in MHz</param>
    /// <param name="ignore_errors">if "false", errors like missing clocking program will cause a message box to pop up</param>
    private void clocking_set_clock(float core_clock, float mem_clock, bool ignore_errors) {
      if (!ax.ac.feature_clocking || !Clocking.clocking_ability())
        return;
      float[] clocks = Clocking.clocking_set_clock(core_clock, mem_clock, ignore_errors);
      if (clocks != null) {
        text_clocking_curr_core.Text = clocks[0].ToString();
        text_clocking_curr_mem.Text = clocks[1].ToString();
      } else {
        text_clocking_curr_core.Text = text_clocking_curr_mem.Text = "";
      }
    }


    #endregion

    #region Newly Added (Experimental) Code
    private void group_current_Enter(object sender, System.EventArgs e) {
      update_current_combos_from_cr();
    }

    enum ProfNameState { Choose, New, Rename };
    ProfNameState m_profNameState = ProfNameState.Choose; // current state of combined control for choosing, adding, renaming profiles

    private void state_enter_new_profile(ProfNameState state) {
      m_profNameState = state;
      text_prof_name.Visible
        = button_prof_ok.Visible
        = button_prof_cancel.Visible = (state != ProfNameState.Choose);

      menu_profile.Enabled
        = menu_profs.Enabled
        = panel_prof_apply.Enabled
        = tabCtrl.Enabled
        = combo_prof_names.Visible
        = button_prof_new.Visible
        = button_prof_clone.Visible
        = button_prof_rename.Visible
        = button_prof_delete.Visible
        = (state == ProfNameState.Choose);

      if (state != ProfNameState.Choose)
        text_prof_name.Focus();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true for cancel</returns>
    private bool showDialog_save_profiles() {
      System.Windows.Forms.DialogResult dr =
        System.Windows.Forms.MessageBox
        .Show("Do you want to save profiles?", G.app_name, System.Windows.Forms.MessageBoxButtons.YesNoCancel);
      if (dr == System.Windows.Forms.DialogResult.Yes) {
        if (sel_gpi >= 0)
          store_gui_profile(ax.gp.get_profile(sel_gpi));
        ax.gp.save_profiles("profiles.cfg");        
      } else if (dr == System.Windows.Forms.DialogResult.No) {

      } else if (dr == System.Windows.Forms.DialogResult.Cancel) {
        return true;
      }
      return false;
    }

    private void ToggleBold(RichTextBox richTextBox1) {
      if (richTextBox1.SelectionFont != null) {
        System.Drawing.Font currentFont = richTextBox1.SelectionFont;
        System.Drawing.FontStyle newFontStyle;

        if (richTextBox1.SelectionFont.Bold == true) {
          newFontStyle = FontStyle.Regular;
        }
        else {
          newFontStyle = FontStyle.Bold;
        }

        richTextBox1.SelectionFont = new Font(
          currentFont.FontFamily, 
          currentFont.Size, 
          newFontStyle
          );
      }
    }

    #endregion

    #region Starting This App With Windows
    /// <summary>
    /// Add or remove this program to windows startup
    /// </summary>
    /// <param name="test">do nothing, just test if a startup registry value for this program exists.</param>
    /// <param name="val">if TEST is set false, then add an entry if VAL is true, or remove it, if VAL is false</param>
    /// <returns>true, if an 3DProf entry exists in startup</returns>
    private bool auto_start(bool test, bool val) {
      string tdprof_full_path = G.app_install_directory + @"\tdprof.exe";
      string data = null;
      bool auto_start_enabled = false;

      if (test) {
        using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run")) {
          if (rk != null) {
            data = rk.GetValue("3DProf") as string;
            auto_start_enabled = data != null && data.StartsWith(tdprof_full_path);
          }
        }
      
        // We used to use -iconic option in autostart in older versions.
#if false // I have seen some exceptions in version compare. Disable it for now and use a version independent text match
        if (auto_start_enabled && AppVersion.compare_version_strings(ax.ac.app_version, "1.2.40") < 0) {
#else
        if (auto_start_enabled && data != null && data.EndsWith("-iconic")) {
#endif
          auto_start(false, true); // recursion!
        }
        return auto_start_enabled;
      }
       
      using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run")) {
        if (rk == null)
          return false;

        if (val)
          rk.SetValue("3DProf", tdprof_full_path + " -tray");
        else 
          rk.DeleteValue("3DProf");
        return val;
      }
    }

    #endregion

    #region CDROM Image Mounting
    /// <summary>
    ///Try to make daemon.exe availabe by asking user if the current path to it is invalid.  
    /// </summary>
    /// <returns>false if daemon.exe is not availabe. true if a file ist availabe (but it's still
    /// possibe the user has given us a wrong file</returns>
    private bool check_daemon_exe() {
      if (! Utils.file_exists(ax.ac.img_daemon_exe_path)) {
        MessageBox.Show("Before image mounting is possible, you have to configure the path to daemon.exe", G.app_name);
        //form_opts.ShowDialog();
        menu_opts_settings_Click(null, null);
        return Utils.file_exists(ax.ac.img_daemon_exe_path);
      }
      return true;
    }

    void img_file_remove(int idx) {
      combo_prof_img.Items.RemoveAt(idx);
      if (combo_prof_img.Items.Count == 1) {
        combo_prof_img.DropDownStyle = ComboBoxStyle.Simple;
        combo_prof_img.Items[0] = combo_prof_img.Items[0].ToString().Substring(3);
      }
      if (combo_prof_img.Items.Count > 0)
        combo_prof_img.SelectedIndex = ((idx > 0) ? idx -1 : 0);
    }

    private void img_file_browse(bool replace, bool all) {
      Debug.Assert(sel_gpd != null);
      if (sel_gpd == null)
        return;

      dialog_prof_choose_exec.FileName = sel_gpd.img_path.Length > 0 ? sel_gpd.img_path[0] : "";

      dialog_prof_choose_exec.Filter = "All Images (cue,iso,ccd,bwt,mds,cdi)|*.cue;*.iso;*.ccd;*.bwt;*.mds;*.cdi|All Files|*.*";
      dialog_prof_choose_exec.ReadOnlyChecked = true;
      dialog_prof_choose_exec.Multiselect = all;
      dialog_prof_choose_exec.Title = (replace ? "Choose new image file" : "Choose image file") + (all ? "(s)" : "");
      dialog_prof_choose_exec.ShowDialog();
      dialog_prof_choose_exec.Filter = "";
      dialog_prof_choose_exec.Multiselect = false;
    }

    #endregion

    #region Developer Maintanance Support

    void dev_testing_make_new_profile(string name) {
      button_prof_new.PerformClick();
      text_prof_name.Text = name;
      button_prof_ok.PerformClick();
      Application.DoEvents();
    }

    void dev_testing_profile_creating_and_deleting() {
      combo_prof_names.SelectedItem = "non existing profile";
      Application.DoEvents();
      dev_testing_make_new_profile("testing a");
      button_prof_delete.PerformClick();
      dev_testing_make_new_profile("testing a");
      dev_testing_make_new_profile("testing b");
      button_prof_delete.PerformClick();
      Application.DoEvents();
      combo_prof_names.SelectedItem = "testing a";
      Application.DoEvents();
      button_prof_delete.PerformClick();
      Application.DoEvents();
    }

    void dev_testing_self() {
      dev_testing_profile_creating_and_deleting();
    }

    void dev_make_screenshots() {
      string screenshots_path = G.ManualDirectory + @"\img\" + Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
      Directory.CreateDirectory(screenshots_path);
      string prefix = screenshots_path + @"\tdprof_" + G.spec_name.Substring(G.spec_name.LastIndexOf("-") + 1) + "_shot_";
      string prefix_man = screenshots_path + @"\" + "tdprof_shot_";

      tabCtrl.SelectedTab = tab_main;
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix     + "main.png");
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix_man + "main.png");
      Dev.ScreenShots.dev_make_and_save_screenshot(this.group_main_d3d, prefix_man + "main_group3d.png");
      Dev.ScreenShots.dev_make_and_save_screenshot(this.group_prof,     prefix_man + "main_groupProf.png");
      button_prof_new.PerformClick();
      Dev.ScreenShots.dev_make_and_save_screenshot(this.group_prof,     prefix_man + "main_groupProf_new.png");
      button_prof_cancel.PerformClick();

      tabCtrl.SelectedTab = tab_files;
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix     + "file_tab.png");
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix_man + "file_tab.png");
 
      tabCtrl.SelectedTab = tab_extra_d3d;
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix     + "extra_d3d.png");
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix_man + "extra_d3d.png");

      tabCtrl.SelectedTab = tab_extra_ogl;
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix     + "extra_ogl.png");
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix_man + "extra_ogl.png");

      tabCtrl.SelectedTab = tab_summary;
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix     + "extra_summ.png");
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix_man + "extra_summ.png");

      tabCtrl.SelectedTab = tab_clocking;
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix     + "extra_clock.png");
      Dev.ScreenShots.dev_make_and_save_screenshot(this,                prefix_man + "extra_clock.png");
      Dev.ScreenShots.dev_make_and_save_screenshot(group_clocking_curr, prefix_man + "clock_groupCurr.png");
      Dev.ScreenShots.dev_make_and_save_screenshot(group_clocking_prof, prefix_man + "clock_groupProf.png");

      Form_Settings fs = new Form_Settings(this, ax, Form_Settings.TabEn.Main);
      fs.Show();
      fs.dev_make_screenshots();
      fs.Close();
    }

    #endregion

    #region Hotkeys
    void init_hotkeys() {
      // XXX: enable hotkey support (should be configurable!)
      m_profHotkeys = new ProfileHotkeys(G.sys_hotkeys, new Hotkeys.SystemHotkey.callback_function(hotkey_callback_run_profile)); //
      try { m_profHotkeys.Load("hotkeys.xml"); } catch {}
      m_profHotkeys.ActivateHotkeys();

      m_appHotkeys  = new App.AppHotkeys(G.sys_hotkeys, new Hotkeys.SystemHotkey.callback_function(hotkey_callback_app));
      m_appHotkeys.Load(ax.ac);
      m_appHotkeys.ActivateHotkeys();
    }

    void hotkey_callback(object user_data) {
      string msg = user_data as string;

      text_summary.Text = (msg == null) ? "hotkey_callback fired" : msg;
    }

    void hotkey_callback_app(object user_data) {

      switch ((App.AppHotkeys.EAppKeys)user_data) {

        case App.AppHotkeys.EAppKeys.Iconify:
          toggle_minimize_tray();
          break;

        case App.AppHotkeys.EAppKeys.Focus:
          form_iconify_or_deiconify(false, false);
          this.Activate();
          break;
      
        case App.AppHotkeys.EAppKeys.ClockingPresetSLow:
          clocking_pre_slow();
          break;
      
        case App.AppHotkeys.EAppKeys.ClockingPresetNormal:
          clocking_pre_normal();
          break;
      
        case App.AppHotkeys.EAppKeys.ClockingPresetFast:
          clocking_pre_fast();
          break;
      
        case App.AppHotkeys.EAppKeys.ClockingPresetUltra:
          clocking_pre_ultra();
          break;
      

        default:
          MessageBox.Show("hotkey_callback_app: invalid id", "3DProf - Internal Error");
          break;
      }
    }

    void hotkey_callback_run_profile(object user_data) {
      string profile_name = user_data as string;
      if (profile_name == null)
        return;
      
      int idx = ax.gp.get_profile_index(profile_name);
      if (idx == -1)
        return;


      m_gps.run_profile(idx, true);
    }

    #endregion

    #region nVidia Specific
    void init_nvidia_specific() {
      string env_debug = Environment.GetEnvironmentVariable("TDPROF_DEBUG");
      if (env_debug == null) env_debug = "";

      if (!G.is_vendor_nvidia) {
        menu_tools_nvclock.Visible = false;
        menu_nvCoolBits.Visible    = false;
      }
    }
    #endregion

    #region ATI Specific
    void init_ati_radeon_specific() {
      string env_debug = Environment.GetEnvironmentVariable("TDPROF_DEBUG");
      if (env_debug == null) env_debug = "";

      // Hide unneeded features
      menu_ati_D3DApply.Visible = G.di.is_ati_driver_needing_apply_d3d();

      if (!File.Exists(m_ati_cpl_cmd))
        m_ati_cpl_cmd = m_ati_cpl_cmd2;

      menu_ati_open_cpl.Visible
        = menu_ati_open_oldCpl.Visible
        = (G.is_vendor_ati && File.Exists(m_ati_cpl_cmd));

      menu_tools_regdiff_ati.Visible = (G.is_vendor_ati && Environment.OSVersion.Platform == System.PlatformID.Win32NT);

      if (!G.is_vendor_ati) {
        menu_help.MenuItems.Remove(menu_help_ati_visit_radeonFAQ);
      }

    }

    #region Experimental: Radeon Specific Tab
    private void set_checkbox_by_combo(CheckBox chb, ComboBox cob) {
      combo_user_data cud = (combo_user_data)cob.Tag;
      ConfigRecord.Mode cr_mode = G.cr_modes[cud.idx];
      string[] names = ax.cr.get_modeval_names(cr_mode);
      int idx = cob.SelectedIndex;
      if (idx == -1)
        return; // ???
      switch (names[idx].ToLower()) {
        case "unset": chb.CheckState = CheckState.Indeterminate; break; //???
        case "on":    chb.CheckState = CheckState.Checked;       break; //???
        case "off":   chb.CheckState = CheckState.Unchecked;     break; //???

        case "yes": goto case "on";
        case "no": goto case "off";
      }
    }
    private void set_combo_by_checkbox(CheckBox chb, ComboBox cob) {
      combo_user_data cud = (combo_user_data)cob.Tag;
      ConfigRecord.Mode cr_mode = G.cr_modes[cud.idx];
      string[] names = ax.cr.get_modeval_names(cr_mode);

      for(int idx = 0, end = names.Length; idx < end; ++idx) {
        string s = names[idx].ToLower();
        if (chb.CheckState == CheckState.Checked &&  (s == "on" || s == "yes")) {
          cob.SelectedIndex = idx; break;
        } else if (chb.CheckState == CheckState.Unchecked &&  (s == "off" || s == "no")) {
          cob.SelectedIndex = idx; break;
        } else if (chb.CheckState == CheckState.Indeterminate &&  (s == "unset")) {
          cob.SelectedIndex = idx; break;
        }
      }
#if false
      switch (cob.Text.ToLower()) {
        case "unset": chb.CheckState = CheckState.Indeterminate; break; //???
        case "on":  chb.Checked = true; break; //???
        case "off":  chb.Checked = false; break; //???

        case "yes": goto case "on";
        case "no": goto case "off";
      }
#endif
    }
    #endregion

    string m_ati_cpl_cmd = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + @"\ATIPRBXX.EXE";
    string m_ati_cpl_cmd2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles) + @"\ATI Technologies\ATI Control Panel\atiprbxx.exe";
    void ati_open_cpl(bool old) {
      string guid = ax.di.get_guid();
      if (guid != null) {
        using (RegistryKey rk = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\ATI Technologies\Desktop\" + guid)) {
          if (rk != null)
            rk.SetValue("3D", (old ? new byte[] { 0, 0, 0, 0 } : new byte[] { 1, 0, 0, 0 }));
        }
        Process.Start( m_ati_cpl_cmd, "/a" );
      }
    }
    #endregion

    #region CheckBoxing

    /// <summary>
    /// Create all CheckBoxes: initialize checkbox arrays, add event handlers to each checkbox.
    /// </summary>
    void checkboxing_combos() {
      checkboxing_combos_2(ref checks_curr_modes, combos_curr_modes);
      checkboxing_combos_2(ref checks_prof_modes, combos_prof_modes);
      // add handler for right-click-disabling profile controls
      foreach(CheckBox chb in checks_prof_modes)
        if (chb != null) {
          chb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.combos_prof_mouse_down);
        }
      foreach(ComboBox cob in combos_prof_modes)
        cob.ForeColorChanged += new System.EventHandler(combos_update_checkbox_ForeColorChanged);
    }

    private bool contains(string[] a, string s) {
      foreach(string am in a)
        if (am == s) return true;
      return false;
    }

    /// <summary>
    /// Create a checkbox for each combo box containing checkbox like content like Yes/No, On/Off
    /// </summary>
    /// <param name="chbs">returns new array of checkboxes. May contain null for items not checkbox-able</param>
    /// <param name="cobs">combo boxes used as source. Only checkbox-able items are used, others are ignored.</param>
    void checkboxing_combos_2(ref CheckBox[] chbs, ComboBox[] cobs) {

      chbs = new CheckBox[cobs.Length];

      for(int i=0, e = cobs.Length; i < e; ++i) {
        ComboBox cob = cobs[i];
        combo_user_data cud = (combo_user_data)cob.Tag;
        ConfigRecord.Mode cr_mode = G.cr_modes[cud.idx];
        string[] names = ax.cr.get_modeval_names(cr_mode);

        if ((names.Length == 2
          && ((contains(names, "Yes") && contains(names, "No"))
          || (contains(names, "On") && contains(names, "Off"))))
          || (cob.Items.Count == 3
          && ((contains(names, "Yes") && contains(names, "No") && contains(names, "unset"))
          || (contains(names, "On") && contains(names, "Off") && contains(names, "unset"))))) {
          CheckBox chb = chbs[i] = new CheckBox();
          //chb.Visible = cob.Visible;
          chb.Enabled = cob.Enabled;
          chb.Anchor  = cob.Anchor;
          chb.Parent  = cob.Parent;
#if true
          chb.FlatStyle = FlatStyle.System;
          chb.Location = new System.Drawing.Point(cob.Location.X + cob.Width / 2 - 8, cob.Location.Y + cob.Height / 2 - 8);
          chb.CheckAlign = ContentAlignment.TopLeft;
#else
          chb.CheckAlign = ContentAlignment.MiddleCenter;
          chb.Location = cob.Location;
#endif
          chb.Tag = cob.Tag;
          chb.Size = cob.Size;
          chb.ThreeState = cob.Items.Count == 3;
          set_checkbox_by_combo(chb, cob);
          cob.SelectedIndexChanged += new System.EventHandler(combos_update_checkbox_SelectedIndexChanged);
          //         cob.TextChanged += new System.EventHandler(combos_update_checkbox_SelectedIndexChanged);
          cob.EnabledChanged += new System.EventHandler(combos_update_checkbox_EnabledChanged);
          chb.CheckStateChanged += new System.EventHandler(checks_update_combo_CheckStateChanged);
          chb.MouseEnter += new System.EventHandler(combos_mouse_enter);
          chb.MouseLeave += new System.EventHandler(combos_mouse_leaves);

          cob.Visible = false;
          cob.Parent.Controls.Add(chb);
        }
      }
    }
 
    #endregion
 
    #region Tools: Windows Tweaks
    void check_lsc(bool init) {
      using (RegistryKey rek = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management")) {
        if (init) {
          menu_winTweaks_Separator.Visible
            = menu_winTweaks.Visible
            = (rek != null) && (Environment.OSVersion.Platform == System.PlatformID.Win32NT);
        }

        if (rek != null && menu_winTweaks.Visible) {
          Object tem = rek.GetValue("LargeSystemCache");
          bool lsc_on = ((tem == null) ? false : ((Int32)tem) != 0);
          tem = rek.GetValue("DisablePagingExecutive");
          bool pagExec = ((tem == null) ? false : ((Int32)tem) != 0);
          tem = rek.GetValue("SystemPages");
          Int32 sysPages = ((tem == null) ? 0x183000 : (Int32)tem); 

          menu_ati_lsc.Checked = lsc_on && (uint)sysPages >= 0xefffffff; //Int32.MaxValue;
          menu_winTweak_disablePageExecutive.Checked = pagExec;
        }
      }
    }

    void check_mw(bool init) {   
      using (RegistryKey rek = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Logitech\MouseWare\CurrentVersion\GamingCompatibility")) { 
        if (init) {
          menu_prof_mouseWare.Visible = (rek != null);
          return;
        }
        bool found = false;

        if (rek == null || sel_gpd == null)
          return;
        string exe_file = System.IO.Path.GetFileName(sel_gpd.exe_path).ToLower();

        foreach(string val in rek.GetValueNames()) {
          string data = rek.GetValue(val) as string;
          if (data == null || data.ToLower() != exe_file)
            continue;
          found = true;
          break;
        }
        menu_prof_mouseWare_noAccel.Checked = found;
      }
    }

    #endregion
    #region Tools: nVidia Coolbits
    #endregion

    #region Logitec MouseWare Support
    void mouseWare_restart_emExec() {
      Process[] procs = Process.GetProcessesByName("Em_Exec");
      foreach(Process proc in procs) {
        string file_name = proc.MainModule.FileName;
        proc.Kill();
        Process.Start(file_name);
      }
    }

    #endregion

    #region Help Reports
    string report(string cmd, string args) {
      Process p = new Process();
      p.StartInfo.FileName = cmd;
      p.StartInfo.Arguments = args;
      p.StartInfo.UseShellExecute = false;
      p.StartInfo.RedirectStandardOutput = true;
      p.StartInfo.RedirectStandardError = true;
      //p.StartInfo.RedirectStandardInput = true;
      p.StartInfo.CreateNoWindow = true;
      p.Start();
      p.WaitForExit();
      return
        " ==== Standard Output Stream: ====\r\n\r\n"
        + p.StandardOutput.ReadToEnd()
        + "\r\n\r\n === Standard Error Stream: ===\r\n\r\n"
        + p.StandardError.ReadToEnd() + "\r\n";
    }

    string report_nvclock() {
      return report("etc/nvclock.exe", "-d -1 --speeds --remove-driver");
    }
    string report_r6clock() {
      System.Text.StringBuilder buf = new System.Text.StringBuilder(1024);
      return report("etc/radeon_setclk.exe", "--info") 
        + ((Win32.R6Clock.r6clockdiag_get_info(buf, 1024) == 0)
        ?  "=== R6Clock.dll:r6clockdiag_get_info(): ===\r\n" + buf.Replace("\n", "\r\n")
        : "");
    }

    #endregion

    #region DEFER HIDING OF TRAY ICON
    Thread m_thread_defer_icon_visibility = null;

    delegate void ti_visibility(bool visible, int defer_milliseconds);
    void thread_tray_icon_visibility() {
      System.Threading.Thread.Sleep(tray_icon_visibility_wait_ms);
      this.Invoke(new ti_visibility(tray_icon_visibility), new object[] { tray_icon_is_visible, 0 });
    }
    bool tray_icon_is_visible = true;
    int  tray_icon_visibility_wait_ms = 0;
    void tray_icon_visibility(bool visible, int defer_milliseconds) {
      tray_icon_is_visible = visible;
      tray_icon_visibility_wait_ms = defer_milliseconds;

      if (m_thread_defer_icon_visibility != null) {
        m_thread_defer_icon_visibility.Abort();
        m_thread_defer_icon_visibility = null;
      }

      if (defer_milliseconds == 0) {
        notifyIcon_tray.Visible = tray_icon_is_visible ;
        return;
      }

      m_thread_defer_icon_visibility = new Thread(new ThreadStart(thread_tray_icon_visibility));
      m_thread_defer_icon_visibility.Start();
    }
    #endregion DEFER HIDING OF TRAY ICON

    #region Card Config

    #region init / update from a ConfigRecord object  
    private void init_from_cr(bool re_init) {
      // init D3D+OGL combo boxes
      for (int i=0; i < G.cr_modes.Length; ++i) {
        init_combo(combos_curr_modes[i], G.cr_modes[i]);
        init_combo(combos_prof_modes[i], G.cr_modes[i]);
        if (!re_init) {
          // creating user data objects which are accesible through ComboBox.Tag member
          Label row_label = null, row_prof_label = null;
          if (i < G.cr_modes_extra1_start) {
            row_label = ((i%2) == 0) ? label_d3d : label_ogl;
            row_prof_label = ((i%2) == 0) ? label_prof_d3d : label_prof_ogl;
          } else if (i < G.cr_modes_extra2_start) {
            row_label = ((i%2) == 0) ? label_extra_curr_d3d : label_extra_curr_ogl;
            row_prof_label = ((i%2) == 0) ? label_extra_prof_d3d : label_extra_prof_ogl;
          } else  {
            row_label = ((i%2) == 0) ? label_extra2_curr_d3d : label_extra2_curr_ogl;
            row_prof_label = ((i%2) == 0) ? label_extra2_prof_d3d : label_extra2_prof_ogl;
          }

          combos_curr_modes[i].Tag = new combo_user_data(combo_user_data.Kind.CURR, i, row_label, labels_modes[i], G.cr_modes[i]);
          combos_prof_modes[i].Tag = new combo_user_data(combo_user_data.Kind.PROF, i, row_prof_label, labels_modes[i], G.cr_modes[i]);
 

          // Init profile settings from current settings
          ((combo_user_data)combos_curr_modes[i].Tag).disable_updating = true;
          //combos_curr_modes[i].SelectedIndex = combos_prof_modes[i].SelectedIndex = ax.cr.get_modeval_index_of_name(G.cr_modes[i]);
          ((combo_user_data)combos_curr_modes[i].Tag).disable_updating = false;
        }
      }


      // init Row Label Tags
      for (int i=0; i < labels_modes.Length; ++i) {
        labels_modes[i].Tag = i;
      }

      check_prof_quit.Checked = ax.ac.run_and_quit;
      menu_profs_filterBySpecName.Checked = ax.ac.gui_filter_by_spec_name;

      switch (ax.ac.app_lang) {
        case "Auto": menu_opt_lang_auto.Checked = true; break;
        case "Deutsch": menu_opt_lang_de.Checked = true; break;
        default: menu_opt_lang_en.Checked = true; break;
      }
      regreadonly_change_for_session(ax.cr.flag_registry_readonly);

      for (int i=0; i < G.cr_modes.Length; ++i) {
        // hide ComboBoxes which are not set in specfile
        combos_prof_modes[i].Visible
          = combos_curr_modes[i].Visible
          = ax.cr.is_defined(G.cr_modes[i]);

        // set combo labels and tooltypes by cr (defined in specfile)
        string label = ax.cr.get_gui_label(G.cr_modes[i]);
        string tooltip = ax.cr.get_gui_tooltip(G.cr_modes[i]);
        int width_mult = ax.cr.get_gui_width_mult(G.cr_modes[i]);
        if (label != null)
          labels_modes[i].Text = label;
        int width = 55; //XXX: literal
        if (tooltip != null)
          toolTip.SetToolTip(labels_modes[i], tooltip);
        if (width_mult > 0) {
          labels_modes[i].Width = width * width_mult;
          combos_curr_modes[i].Width = width * width_mult;
          combos_prof_modes[i].Width = width * width_mult;
          for (int ii=1; ii < width_mult; ++ii) {
            labels_modes[i + ii * 2].Visible 
              = combos_curr_modes[i + ii * 2].Visible 
              = combos_prof_modes[i + ii * 2].Visible
              = false;
          }
        }
      }
      // hide empty labels to avoid mouseover effects
      foreach (Label l in labels_modes)
        l.Visible = (l.Text.Length > 0);

    }

 
    private void update_current_combos_from_cr() {
      for (int i=0; i < G.cr_modes.Length; ++i) {
        ComboBox cob = combos_curr_modes[i];
        if (cob.Tag == null) continue;
        combo_user_data cud = ((combo_user_data)cob.Tag);

        cud.disable_updating = true;
        // update box if not currently opened by user
        if (!cob.DroppedDown) {
          int idx = ax.cr.get_modeval_index_of_enabled_item(cud.cr_mode);
          if (0 <= idx) {
            cob.SelectedIndex = idx;
            cob.DropDownStyle = ComboBoxStyle.DropDownList;
          } else {
            cob.DropDownStyle = ComboBoxStyle.DropDown;
            cob.Text = ax.cr.get_modeval_text(cud.cr_mode, idx);
          }
        }
        cud.disable_updating = false;
      }
#if TAB_EXP
      tab_exp_update_from_cr();
#endif
    }

    /// <summary>
    /// Create items of all 3D setting combo boxes. Each ComboBox represents a single 3D Option.
    /// Its Items represent all possible arguments defined for that option.
    /// </summary>
    /// <param name="box">An empty box we want create items for</param>
    /// <param name="label">The option for which all defined arguments are retrieved</param>
    private void init_combo(ComboBox box, ConfigRecord.Mode label) {
      box.BeginUpdate();
      string[] new_items = ax.cr.get_modeval_texts(label);

      if (box.Items.Count == new_items.Length) {
        for(int i=0, end = new_items.Length; i < end; ++i)
          box.Items[i] = new_items[i];

      } else {
        box.Items.Clear();
        box.Items.AddRange(ax.cr.get_modeval_texts(label));
      }
      box.EndUpdate();
    }
    #endregion

    #endregion

    #region Game Profiles

    #region init / update from a GameProfile object 

    private void init_from_gp() {

      combo_prof_names.BeginUpdate();
      combo_prof_names.Items.Clear();

      // filtering works by sorting gpd (done by ctor) and don't showing the nonmatching high indexes
      //  menu items are filtered out by just make them invisible. This way we can retrieve
      // the related profile directly by menu index.
      bool filter_by_videocard = ax.ac.gui_filter_by_spec_name;

      foreach (GameProfileData gpd in ax.gp) {
        if (filter_by_videocard && gpd.spec_name != "" && gpd.spec_name != G.spec_name)
          break;
        combo_prof_names.Items.Add(gpd.name);
      }
      // load last selected profile
      int idx = ax.gp.get_profile_index(ax.cl.optarg_profile == null ? ax.ac.prof_last_selected : ax.cl.optarg_profile);
      if (0 <= idx && idx < combo_prof_names.Items.Count)
        combo_prof_names.SelectedIndex = idx;

      combo_prof_names.EndUpdate();
      m_tray_menu_updated = m_main_menu_updated = false;

    }

    private void init_tray_menu_from_gp() {
      MenuItem[] parentMenus = { menu_tray_apply_exe,
                                 menu_tray_applyExe,
                                 menu_tray_apply,
                                 menu_tray_profs_makeLink,
                                 menu_tray_profs_editGameIni,
                                 menu_tray_img_mountAnImgAtD0,
      };

      
      foreach (MenuItem mi in parentMenus) {
        mi.MenuItems.Clear();
        mi.Enabled = false;
      }


      menu_tray_apply.MenuItems.Add(0, menu_tray_applyExe);
      int menu_tray_apply_firstIndex = 1;
      // filtering works by sorting gpd (done by ctor) and don't showing the nonmatching high indexes
      //  menu items are filtered out by just make them invisible. This way we can retrieve
      // the related profile directly by menu index.
      bool filter_by_videocard = ax.ac.gui_filter_by_spec_name;

      for (int i = 0, e = ax.gp.nmb_of_profiles; i < e;  ++i) {

        GameProfileData gpd = ax.gp.get_profile(i);

        if (filter_by_videocard && gpd.spec_name != G.spec_name)
          break;

        menu_tray_apply_exe.MenuItems.Add(i, new MenuItem(gpd.name, new System.EventHandler(menu_tray_apply_exe_Any_Click)));
        menu_tray_apply_exe.MenuItems[i].Visible = (gpd.exe_path != "");

        menu_tray_apply.MenuItems.Add(i + menu_tray_apply_firstIndex, new MenuItem(gpd.name, new System.EventHandler(menu_tray_apply_Any_Click)));
        menu_tray_apply.MenuItems[i + menu_tray_apply_firstIndex].Visible = (gpd.exe_path == "");

        menu_tray_applyExe.MenuItems.Add(i, new MenuItem(gpd.name, new System.EventHandler(menu_tray_apply_Any_Click)));
        menu_tray_applyExe.MenuItems[i].Visible = (gpd.exe_path != "");

        menu_tray_profs_makeLink.MenuItems.Add(i, new MenuItem(gpd.name, new System.EventHandler(menu_tray_profs_makeLink_Any_Click)));
        menu_tray_profs_makeLink.MenuItems[i].Checked = Link.exists_link(gpd, ax.ac.sl_name_prefix, ax.ac.sl_name_suffix);

        menu_tray_profs_editGameIni.MenuItems.Add(i, new MenuItem(gpd.name, new System.EventHandler(menu_tray_profs_editGameIni_Any_Click)));
        menu_tray_profs_editGameIni.MenuItems[i].Visible = (gpd.game_ini_path != "");

        foreach(string img in gpd.img_path) {
          foreach(MenuItem mi in menu_tray_img_mountAnImgAtD0.MenuItems)
            if (mi.Text == img)
              goto skip;              
          menu_tray_img_mountAnImgAtD0.MenuItems.Add(new MenuItem(img, new System.EventHandler(menu_tray_img_mountAnImgAtD0_Any_Click)));
        skip:;
        }
      } // for

      menu_tray_img_mountAnImgAtD0.Enabled =  menu_tray_img_mountAnImgAtD0.MenuItems.Count > 0;

      foreach (MenuItem mi in parentMenus) {
        mi.Enabled = mi.MenuItems.Count > 0;
      }
      remove_menuImage_recursively(context_tray.MenuItems, false);
    }

    private void init_main_menu_from_gp() {
      string s;
      MenuItem[] parentMenus = { menu_prof_importProfile,
      };
   
      foreach (MenuItem mi in parentMenus) {
        mi.MenuItems.Clear();
        mi.Enabled = false;
      }

      // combo-box filtering works by sorting gp (done by ctor) and don't add the nonmatching high indexes
      //  menu items are filtered out by just make them invisible. This way we can retrieve
      // the related profile directly by menu index.
      bool filter_by_videocard = ax.ac.gui_filter_by_spec_name;

      for (int i = 0, e = ax.gp.nmb_of_profiles; i < e;  ++i) {
        GameProfileData gpd = ax.gp.get_profile(i);
        if (filter_by_videocard && gpd.spec_name != G.spec_name)
          break;

        menu_prof_importProfile.MenuItems.Add(i, new MenuItem(gpd.name, new System.EventHandler(menu_prof_importProfile_Any_Click)));
        menu_prof_importProfile.MenuItems[i].Visible = (gpd.exe_path == "");
        menu_prof_importProfile.MenuItems[i].RadioCheck = true;
        if ((s = gpd.include_other_profile) != "" && s == gpd.name) {
          menu_prof_importProfile.Checked = true;
        }


        foreach (MenuItem mi in parentMenus) {
          mi.Enabled = mi.MenuItems.Count > 0;
        }
      }
    }

    /// <summary>
    /// Update profile widgets states by GameProfileData
    /// </summary>
    /// <param name="gpd">profile data</param>
    private void update_from_gpd(GameProfileData gpd) {
      if (gpd == null)
        return; // XXX
      if (combos_prof_update_in_progress)
        return;

      // update 3D profile combo boxes (let them show an item or write text directly if no item matches)
      combos_prof_update_in_progress = true;

      for (int i=0, l=G.cr_modes.Length; i < l; ++i) {
        string gp_mode_name = gpd.val(G.gp_parms[i]).Data;
        int mode_idx        = ax.cr.get_modeval_index_of_name(G.cr_modes[i], gp_mode_name);
        bool disabled       = !gpd.val(G.gp_parms[i]).Enabled;

        set_index_or_text_in_profile_comboBox(i, mode_idx, gp_mode_name);
        combos_prof_modes[i].Enabled = !disabled;
      }
      combos_prof_update_in_progress = false;


      // update menu item "bound to card"
      menu_prof_setSpecName.Checked = (gpd.spec_name == G.spec_name);

      // update exe file/args controls
      text_prof_exe_path.Text           = gpd.exe_path;
      text_prof_exe_path.SelectionStart = text_prof_exe_path.Text.Length; // show end of text
      text_prof_exe_args.Text           = gpd.exe_args;
      text_prof_exe_args.SelectionStart = text_prof_exe_args.Text.Length; // show end of text

      // image combo box
      combo_prof_img.Items.Clear();
      combo_prof_img.DropDownStyle = (gpd.img_path.Length > 1) ? ComboBoxStyle.DropDown : ComboBoxStyle.Simple;

      int dn_idx=0;
      if (gpd.img_path.Length == 1) {
        combo_prof_img.Items.AddRange(gpd.img_path);
      } else
        foreach(string img in gpd.img_path) {
          combo_prof_img.Items.Add(gpd.img_drive_number[dn_idx++] + ": " + img);
        }


      if (combo_prof_img.Items.Count > 0) {
        combo_prof_img.SelectedIndex = 0;
        // daemon.exe
        int idx = combo_prof_img.SelectedIndex;
        menu_prof_daemon_drive_nmb_0.Checked = (gpd.img_drive_number[idx] == 0);
        menu_prof_daemon_drive_nmb_1.Checked = (gpd.img_drive_number[idx] == 1);
        menu_prof_daemon_drive_nmb_2.Checked = (gpd.img_drive_number[idx] == 2);
        menu_prof_daemon_drive_nmb_3.Checked = (gpd.img_drive_number[idx] == 3);
      } else {
        combo_prof_img.Text = "";
      }
      //combo_prof_img.SelectionStart = combo_prof_img.Text.Length; // show text end



      // server browser / TDProfGD.exe menu
      menu_prof_tdprofGD_enable(gpd);

      // clocking
      text_clocking_prof_core.Text = gpd.clocking_core_clock.ToString();
      text_clocking_prof_mem.Text = gpd.clocking_mem_clock.ToString();
      combo_clocking_prof_presets.SelectedIndex = (int)gpd.clocking_kind;

      //clocking_get_clock(true);

      clocking_enable_prof_core(gpd.val(GameProfileData.Parms.CLOCKING_CORE_CLK).Enabled);
      clocking_enable_prof_mem(gpd.val(GameProfileData.Parms.CLOCKING_MEM_CLK).Enabled);

      // include master profile
      foreach (MenuItem mi in menu_prof_importProfile.MenuItems) {
        mi.Checked = (mi.Text == gpd.include_other_profile);
      }
        
      // some special profiles cannot have a game exe
      // but on the other hand: a game exe can be a ClockGen file to change FSB
#if false
      text_prof_exe_path.Enabled = (ax.ac.prof_default != gpd.name);
#endif


      check_prof_shellLink.Enabled = false;
      check_prof_shellLink.Checked = Link.exists_link(gpd, ax.ac.sl_name_prefix, ax.ac.sl_name_suffix);
      check_prof_shellLink.Enabled = true;

#if TAB_EXP
      tab_exp_update_from_gpd(gpd);
#endif

    }

    #endregion
    void game_profile_has_changed(Object sender, EventArgs e) {
      button_prof_set_enable_state ();
      if (text_summary.Visible)
        summary_update();
      if (sel_gpd != null) 
        tab_exp_update_from_gpd(sel_gpd);
    }


      #region Auto Restore

    class FM_GameProfiles {
      FormMain m_frm;
      AppContext ax;
      public GameProfileData running_gpd = null;
      delegate void notify_me();


      public FM_GameProfiles(FormMain a_frm, AppContext a_ax) {
        m_frm = a_frm;
        ax    = a_ax;
      }

      public App.AppRunProfile m_arp;
      void auto_restore_now_invoke() {
        m_frm.Invoke(new notify_me(m_frm.auto_restore_now));
      }

      void do_nothing () {}


      public void run_profile(int idx, bool ignore_quit) {
        if (idx >= m_frm.combo_prof_names.Items.Count)
          return;

        m_frm.combo_prof_names.SelectedIndex = idx;

        m_frm.ar_apply.save_state(ax.cr);
        m_frm.menu_tools_undoApply.Enabled = m_frm.button_prof_restore.Enabled = true;

        // check if mounting is required, if yes check if mounting program is configured
        if (m_frm.sel_gpd.img_path.Length > 0)
          if (!m_frm.check_daemon_exe())
            return;

        m_arp = new App.AppRunProfile(m_frm.sel_gpd, new App.AppRunProfile.notify_me(auto_restore_now_invoke));
        m_arp.apply_profile();
        m_frm.gui_update();
                
        if (m_frm.text_prof_exe_path.Text.Length > 0)
          run_exec(m_frm.sel_gpd, false, m_arp);

        // Quit App
        if (m_frm.check_prof_quit.Checked && !ignore_quit) {
          m_frm.close_me();
        }
      }

      public void run_exec(GameProfileData gpd, bool from_tray, App.AppRunProfile a_arp) {
        m_frm.executed_from_tray = from_tray;
        m_frm.ar_apply.store_to_file("3d_settings_saved_by_run_exec.bin");

        running_gpd = gpd;

        if (!ax.ac.run_and_quit && ax.ac.auto_restore_after_exit_in_gui && !gpd.autorestore_force_disable) {
          App.AppRunProfile arp = (a_arp != null) ? a_arp : new App.AppRunProfile(gpd, new App.AppRunProfile.notify_me(do_nothing));
          arp.run_profile();
        } else {
          PrePostCommands.run_pre_commands(gpd);
          Process proc = gpd.run_exec();
        }


        //Minimize Window
        m_frm.form_iconify_or_deiconify(true, true);
      }

      public string find_ini_file(bool keep_existing_file) {
        GameProfileData sel_gpd = m_frm.sel_gpd;
        Debug.Assert(sel_gpd != null);
        if (sel_gpd == null)
          return null;


        if (!keep_existing_file || sel_gpd.game_ini_path.Length == 0) {
          // set directory to previous or the one in exe_path, which likely contains the config file too
          if (sel_gpd.game_ini_path.Length > 0)
            m_frm.dialog_prof_choose_exec.FileName = sel_gpd.game_ini_path;
          else if (sel_gpd.exe_path.Length > 0) {
            m_frm.dialog_prof_choose_exec.FileName = "";
            m_frm.dialog_prof_choose_exec.InitialDirectory = System.IO.Path.GetDirectoryName(sel_gpd.exe_path);
          }
          m_frm.dialog_prof_choose_exec.Filter = "All Config Files (ini,cfg,exe)|*.cfg;*.ini;*.exe|All Files|*.*";
          m_frm.dialog_prof_choose_exec.ReadOnlyChecked = true;
          m_frm.dialog_prof_choose_exec.Title = "Choose game config file";
          m_frm.dialog_prof_choose_exec.ShowDialog();
          m_frm.dialog_prof_choose_exec.Filter = "";
        }
        if (Utils.file_exists(sel_gpd.game_ini_path))
          return sel_gpd.game_ini_path;
        else
          return null;
      }

      private void store_settings(string file_name) {
        AutoRestore ar = new AutoRestore();
        ar.save_state(ax.cr);
        ar.store_to_file(file_name);
      }


    }


    /// <summary>
    /// invoked by the wait for exit thread
    /// </summary>
    public void auto_restore_now() {
      if (m_gps.m_arp == null)
        return;

      m_gps.m_arp.restore_now();

      if (!executed_from_tray)
        form_iconify_or_deiconify(false, false);
      else {
        executed_from_tray = false; 
        swap.swap_me_out();
      }

      m_gps.running_gpd = null;
      m_gps.m_arp = null;
      gui_update();
    }

    private void auto_restore_to_before_nongui_run() {
      try {
        AutoRestore ar = AutoRestore.create_from_file("3d_settings_saved_by_run.bin");
        ar.restore_state(ax.cr);
      } catch {
      }
    }

    private void auto_restore_to_before_run() {
      try {
        ar_apply.restore_state(ax.cr);
        update_current_combos_from_cr();
        clocking_get_clock(true);
        menu_tools_undoApply.Enabled = button_prof_restore.Enabled = false;        
      } catch {
      }
    }

    #endregion
      #region Profile Templates

    void templates_turnTo2D() {
      if (sel_gpd == null)
        return;

      sel_gpd.auto_restore_mode = GameProfileData.AutoRestoreMode.OFF;

      for (int i=0; i < combos_prof_modes.Length; ++i) {
        util_prof_enable_combo(i, false);
      }

      prof_set_spec_name(true);
      clocking_enable_prof_mem(false);
      clocking_enable_prof_core(false);
    }

    void templates_turnClockOnly() {
      if (sel_gpd == null)
        return;

      sel_gpd.auto_restore_mode = GameProfileData.AutoRestoreMode.OFF;

      for (int i=0; i < combos_prof_modes.Length; ++i) {
        util_prof_enable_combo(i, false);
      }

      //prof_set_spec_name(true);
      clocking_enable_prof_mem(true);
      clocking_enable_prof_core(true);
    }


    #endregion

    /// <summary>
    /// Transfer data represented by widget states to the profile GPD
    /// </summary>
    /// <param name="gpd">Profile we want to update</param>
    private void store_gui_profile(GameProfileData gpd) {
      for (int i=0; i < G.cr_modes.Length; ++i) {
        if (combos_prof_modes[i].SelectedIndex >= 0) {
          // lookup config-name for selected index in the config
          string modeval_name = ax.cr.get_modeval_name(G.cr_modes[i], combos_prof_modes[i].SelectedIndex);
          // store the config-name in the related profile field
          gpd.val(G.gp_parms[i]).Data = modeval_name;
        }
      }
    }


    private GameProfileData detect_game_process() {
      Process[] procs = Process.GetProcesses();

      string[] prof_files = new string [ax.gp.nmb_of_profiles];
      GameProfileData[] prof_gpds = new GameProfileData [ax.gp.nmb_of_profiles];

      int count_exe_profs = 0;
      foreach(GameProfileData gpd in ax.gp.profs) {
        string path_sep = @"/\";
        if (gpd.exe_path != "") {
          int idx = gpd.exe_path.LastIndexOfAny(path_sep.ToCharArray());
          string file_name = (idx >= 0) ? gpd.exe_path.Substring(idx + 1) : gpd.exe_path;
          prof_files[count_exe_profs] = file_name;
          prof_gpds[count_exe_profs++] = gpd;
        }
      }


      foreach(Process proc in procs) {
        string proc_name = proc.ProcessName + ".exe";

        for (int i=0; i < count_exe_profs; ++i) {
          if (proc_name == prof_files[i]) {
            //MessageBox.Show("Found process: " + prof_files[i], G.app_name + " - Experimental");
            return prof_gpds[i];
          }
        }
      }
      return null;
    }

    private void apply_prof() {
      if (sel_gpd == null)
        return;
      // apply profile (incl. clocking)
      ax.cr.apply_prof(sel_gpd, false);
    }

    private void prof_add(string name, bool init_default) {
      GameProfileData gpd = new GameProfileData((init_default ? null : sel_gpd));

      gpd.spec_name = G.spec_name;
      gpd.name = name;

      if (!init_default) {
#if false
        for (int i=0; i < G.cr_modes.Length; ++i) {
          if (combos_prof_modes[i].SelectedIndex >= 0) {
            string modeval_name = ax.cr.get_modeval_name(G.cr_modes[i], combos_prof_modes[i].SelectedIndex);
            gpd.val(G.gp_parms[i]).Data = modeval_name;
          }
        }
#endif
      } else {
        for (int i=0; i < G.cr_modes.Length; ++i) {
          if (combos_curr_modes[i].SelectedIndex >= 0) {
            string modeval_name = ax.cr.get_modeval_name(G.cr_modes[i], combos_curr_modes[i].SelectedIndex);
            gpd.val(G.gp_parms[i]).Data = modeval_name;
          }
        }
      }
     
      ax.gp.add_prof(gpd);

      init_from_gp();
      combo_prof_names.SelectedIndex = ax.gp.get_profile_index(gpd.name);
    }

    private void prof_set_spec_name(bool clear) {
      if (sel_gpd == null)
        return;

      sel_gpd.spec_name = (clear ? "" : G.spec_name);
    }


    #endregion Game Profiles

    #region TAB_EXP
    
    Font font_lvsi_bold;
    Font font_lvsi_regular;

  
    void init_tab_exp() {

      // Direct3D
      for (int i=0, e=G.cr_modes.Length; i < e; ++i) {

        ConfigRecord.Mode cr_mode = G.cr_modes[i];
        GameProfileData.Parms gp_parm = G.gp_parms[i];

        if (!ax.cr.is_defined(cr_mode))
          continue;
        if (!ConfigRecord.is_mode_d3d(cr_mode))
          continue;

        ListViewItem lvi = list_3d.Items.Add(ax.cr.get_gui_label(cr_mode).Replace("&&", "&"));
        lvi.Tag = new option_3d(i);
        lvi.UseItemStyleForSubItems = false; // color support for sub items
        lvi.SubItems.Add("");
        lvi.SubItems.Add("");
      }

      // OpenGL
      for (int i=0, e=G.cr_modes.Length; i < e; ++i) {

        ConfigRecord.Mode cr_mode = G.cr_modes[i];
        GameProfileData.Parms gp_parm = G.gp_parms[i];

        if (!ax.cr.is_defined(cr_mode))
          continue;
        if (!ConfigRecord.is_mode_ogl(cr_mode))
          continue;

        ListViewItem lvi = list_3d.Items.Add("[ogl]  " + ax.cr.get_gui_label(cr_mode).Replace("&&", "&"));
        lvi.Tag = new option_3d(i);
        lvi.UseItemStyleForSubItems = false; // color support for sub items
        lvi.SubItems.Add(""); 
        lvi.SubItems.Add("");
      }

      // Clocking
      if (true) {
        ListViewItem lvi = list_3d.Items.Add("[clk]  " + "Chip");;
        lvi.Tag = new option_clock(option_clock.EKind.CORE);
        lvi.UseItemStyleForSubItems = false; // color support for sub items
        lvi.SubItems.Add(""); 
        lvi.SubItems.Add("-");
        lvi              = list_3d.Items.Add("[clk]  " + "RAM");;
        lvi.Tag = new option_clock(option_clock.EKind.MEM);
        lvi.UseItemStyleForSubItems = false; // color support for sub items
        lvi.SubItems.Add(""); 
        lvi.SubItems.Add("-");
        lvi              = list_3d.Items.Add("[clk]  " + "Preset");;
        lvi.Tag = new option_clock(option_clock.EKind.PRESET);
        lvi.UseItemStyleForSubItems = false; // color support for sub items
        lvi.SubItems.Add(""); 
        lvi.SubItems.Add("-");
      }


      Font font = list_3d.Items[0].SubItems[1].Font;
      font_lvsi_bold    = new Font(font.FontFamily, font.Size, FontStyle.Bold);      
      font_lvsi_regular = new Font(font.FontFamily, font.Size, FontStyle.Regular);      

      tab_exp_update_from_cr();
    }

    void tab_exp_update_lvsi(ListViewItem.ListViewSubItem lvsi, string text, bool is_default, bool is_alien) {
      Color fore_color = is_alien   ? System.Drawing.Color.Red : list_3d.Items[0].ForeColor;
      Color back_color = is_alien   ? System.Drawing.Color.White : list_3d.Items[0].BackColor;
      Font font        = is_default ? font_lvsi_regular : font_lvsi_bold;
      if (lvsi.Text != text)            lvsi.Text      = text;
      if (lvsi.ForeColor != fore_color) lvsi.ForeColor = fore_color;
      if (lvsi.BackColor != back_color) lvsi.BackColor = back_color;
      if (lvsi.Font != font)            lvsi.Font      = font;   
    }

    void tab_exp_update_from_cr() {

      foreach(ListViewItem lvi in list_3d.Items) {
        option_3d o3d    = lvi.Tag as option_3d;
        option_clock ock = lvi.Tag as option_clock;

        if (o3d != null) {
          int idx = ax.cr.get_modeval_index_of_enabled_item(o3d.cr_mode);
          string text = ax.cr.get_modeval_text(o3d.cr_mode, idx);
          tab_exp_update_lvsi(lvi.SubItems[2], text,  (idx == 0), false);
        }
#if false
        if (ock != null) {
          float[] clocks = Clocking.clocking_get_clock(true);
          lvi.SubItems[2].Text = (ock.kind == option_clock.EKind.CORE)
             ? clocks[0].ToString()
             : clocks[1].ToString();
        }
#endif
      }
    }

    void tab_exp_update_from_gpd(GameProfileData gpd) {
      Debug.Assert (gpd != null);
      int nmb_d3d = 0, nmb_d3d_enabled = 0;
      int nmb_ogl = 0, nmb_ogl_enabled = 0;


      list_3d.BeginUpdate();
      list_3d.Tag = null;

      foreach(ListViewItem lvi in list_3d.Items) {
        option_3d o3d    = lvi.Tag as option_3d;
        option_clock ock = lvi.Tag as option_clock;

        if (o3d != null) {
          string txt = ax.cr.get_modeval_text(o3d.cr_mode, ax.cr.get_modeval_index_of_name(o3d.cr_mode, gpd.val(o3d.gp_parm).Data));

          bool is_default = gpd.val(o3d.gp_parm).Data == ax.cr.get_modeval_name(o3d.cr_mode, 0);

          if (txt != null) {
            tab_exp_update_lvsi(lvi.SubItems[1], txt, is_default, false);
          } else {
            tab_exp_update_lvsi(lvi.SubItems[1], gpd.val(o3d.gp_parm).Data, true, true);
          }

          bool enabled = lvi.Checked = gpd.val(o3d.gp_parm).Enabled;
          if (ConfigRecord.is_mode_d3d(o3d.cr_mode)) {
            ++nmb_d3d; if (enabled) ++nmb_d3d_enabled;
          } else {
            ++nmb_ogl; if (enabled) ++nmb_ogl_enabled;
          }
        }

        if (ock != null) {
          if (ock.kind == option_clock.EKind.PRESET) {
            lvi.SubItems[1].Text = gpd.val(ock.gp_parm).Data;
            lvi.Checked          = gpd.val(ock.gp_parm).Enabled;
            lvi.SubItems[1].Font = (lvi.SubItems[1].Text == "0") ? font_lvsi_regular : font_lvsi_bold;

          } else {
            lvi.SubItems[1].Text = float.Parse(gpd.val(ock.gp_parm).Data, System.Globalization.NumberFormatInfo.InvariantInfo).ToString();
            lvi.Checked          = gpd.val(ock.gp_parm).Enabled;
            lvi.SubItems[1].Font = (lvi.SubItems[1].Text == "0") ? font_lvsi_regular : font_lvsi_bold;
          }
        }
      }

      ListView.SelectedListViewItemCollection lvis = list_3d.SelectedItems;
      ComboBox cbx = combo_ind_modeVal;
      if (cbx.Enabled = (lvis.Count > 0)) {
        cbx.Enabled = lvis[0].Checked;
      }


      check_ind_d3d.CheckState = (nmb_d3d_enabled == 0) ? CheckState.Unchecked
        : ((nmb_d3d - nmb_d3d_enabled) == 0) ? CheckState.Checked
        : CheckState.Indeterminate;
      check_ind_ogl.CheckState = (nmb_ogl_enabled == 0) ? CheckState.Unchecked
        : ((nmb_ogl - nmb_ogl_enabled) == 0) ? CheckState.Checked
        : CheckState.Indeterminate;  
    

      list_3d.EndUpdate();
      list_3d.Tag = gpd;
      list_3d_SelectedIndexChanged(list_3d, null);

    }

    private void list_3d_SelectedIndexChanged(object sender, System.EventArgs e) {
      ListView.SelectedListViewItemCollection lvis = list_3d.SelectedItems;
      if (lvis.Count == 0)
        return;
      ListViewItem lvi              = lvis[0];
      option_3d o3d                 = lvi.Tag as option_3d;
      option_clock ock              = lvi.Tag as option_clock;
      GameProfileData gpd           = list_3d.Tag as GameProfileData;


      if (o3d != null) {

        label_ind_mode.Text = ax.cr.get_gui_label(o3d.cr_mode);
        toolTip.SetToolTip(label_ind_mode, ax.cr.get_gui_tooltip(o3d.cr_mode));

        ComboBox cbx = combo_ind_modeVal;
        cbx.Items.Clear();
        cbx.Items.AddRange(ax.cr.get_modeval_texts(o3d.cr_mode));
        cbx.Tag = null;
     
        if (gpd != null) cbx.Enabled = gpd.val(o3d.gp_parm).Enabled;
        picture_ind_d3d.Visible = ConfigRecord.is_mode_d3d(o3d.cr_mode);
        picture_ind_ogl.Visible = ConfigRecord.is_mode_ogl(o3d.cr_mode);
      
        if (gpd != null) {
          string gp_mode_name = gpd.val(o3d.gp_parm).Data;
          int mode_idx        = ax.cr.get_modeval_index_of_name(o3d.cr_mode, gp_mode_name);
          if (mode_idx != -1)
            cbx.SelectedIndex = mode_idx;
        }
        cbx.Tag = lvi.Tag;
      } else {
        // combo_ind_modeVal.Enabled = combo_ind_modeVal.Visible = false;
      }
    }

    private void list_3d_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e) {
      if (list_3d.Tag == null)
        return;

      ListViewItem lvi              = list_3d.Items[e.Index];
      GameProfileData gpd           = list_3d.Tag as GameProfileData;
      option_3d o3d    = lvi.Tag as option_3d;
      option_clock ock = lvi.Tag as option_clock;

      if (o3d != null) {
        gpd.val(o3d.gp_parm).Enabled = (e.NewValue == CheckState.Checked);
        update_from_gpd(gpd);
      }
    }

    private void combo_ind_modeVal_SelectedIndexChanged(object sender, System.EventArgs e) {
      ComboBox cbx                  = sender as ComboBox;
      if (cbx.Tag == null)
        return; // event caused by program code
      GameProfileData gpd           = list_3d.Tag as GameProfileData;
      option_3d o3d    = cbx.Tag as option_3d;
      option_clock ock = cbx.Tag as option_clock;

      int idx = cbx.SelectedIndex;
      if (idx == -1)
        return;
      if (gpd == null)
        return;

      gpd.val(o3d.gp_parm).Data = ax.cr.get_modeval_name(o3d.cr_mode, idx);
    }

    private void check_ind_d3d_CheckedChanged(object sender, System.EventArgs e) {
      if (list_3d.Tag == null)
        return; // event caused by program code
      GameProfileData gpd = list_3d.Tag as GameProfileData;

      bool enabled = (check_ind_d3d.CheckState == CheckState.Checked);

      foreach(ListViewItem lvi in list_3d.Items) {
        option_3d o3d = lvi.Tag as option_3d;
        if (o3d != null && ConfigRecord.is_mode_d3d(o3d.cr_mode)) {
          gpd.val(o3d.gp_parm).Enabled = enabled;
        }
      }

      update_from_gpd(gpd);
    }

    private void check_ind_ogl_CheckedChanged(object sender, System.EventArgs e) {
      if (list_3d.Tag == null)
        return;
      GameProfileData gpd = list_3d.Tag as GameProfileData;

      bool enabled = (check_ind_ogl.CheckState == CheckState.Checked);

      foreach(ListViewItem lvi in list_3d.Items) {
        option_3d o3d = lvi.Tag as option_3d;
        if (o3d != null && ConfigRecord.is_mode_ogl(o3d.cr_mode)) {
          gpd.val(o3d.gp_parm).Enabled = enabled;
        }
      }

      update_from_gpd(gpd);
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////
    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormMain));
      this.button_prof_apply = new System.Windows.Forms.Button();
      this.button_prof_apply_and_run = new System.Windows.Forms.Button();
      this.button_prof_choose_exe = new System.Windows.Forms.Button();
      this.button_prof_choose_img = new System.Windows.Forms.Button();
      this.button_prof_delete = new System.Windows.Forms.Button();
      this.button_prof_make_link = new System.Windows.Forms.Button();
      this.button_prof_mount_img = new System.Windows.Forms.Button();
      this.button_prof_run_exe = new System.Windows.Forms.Button();
      this.button_prof_save = new System.Windows.Forms.Button();
      this.check_prof_quit = new System.Windows.Forms.CheckBox();
      this.combo_d3d_aniso_mode = new System.Windows.Forms.ComboBox();
      this.combo_d3d_fsaa_mode = new System.Windows.Forms.ComboBox();
      this.combo_d3d_lod_bias = new System.Windows.Forms.ComboBox();
      this.combo_d3d_prerender_frames = new System.Windows.Forms.ComboBox();
      this.combo_d3d_qe_mode = new System.Windows.Forms.ComboBox();
      this.combo_d3d_vsync_mode = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_d3d_1 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_d3d_2 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_d3d_3 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_d3d_4 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_d3d_5 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_ogl_1 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_ogl_2 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_ogl_3 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_ogl_4 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_ogl_5 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_curr_ogl_6 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_d3d_1 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_d3d_2 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_d3d_3 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_d3d_4 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_d3d_5 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_d3d_6 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_ogl_1 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_ogl_2 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_ogl_3 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_ogl_4 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_ogl_5 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_ogl_6 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_d3d_1 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_d3d_2 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_d3d_3 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_d3d_4 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_d3d_5 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_d3d_6 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_ogl_1 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_ogl_2 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_ogl_3 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_ogl_4 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_ogl_5 = new System.Windows.Forms.ComboBox();
      this.combo_extra_curr_ogl_6 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_d3d_1 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_d3d_2 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_d3d_3 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_d3d_4 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_d3d_5 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_d3d_6 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_ogl_1 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_ogl_2 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_ogl_3 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_ogl_4 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_ogl_5 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_ogl_6 = new System.Windows.Forms.ComboBox();
      this.combo_ogl_aniso_mode = new System.Windows.Forms.ComboBox();
      this.combo_ogl_fsaa_mode = new System.Windows.Forms.ComboBox();
      this.combo_ogl_lod_bias = new System.Windows.Forms.ComboBox();
      this.combo_ogl_prerender_frames = new System.Windows.Forms.ComboBox();
      this.combo_ogl_qe_mode = new System.Windows.Forms.ComboBox();
      this.combo_ogl_vsync_mode = new System.Windows.Forms.ComboBox();
      this.combo_prof_d3d_aniso_mode = new System.Windows.Forms.ComboBox();
      this.combo_prof_d3d_fsaa_mode = new System.Windows.Forms.ComboBox();
      this.combo_prof_d3d_lod_bias = new System.Windows.Forms.ComboBox();
      this.combo_prof_d3d_prerender_frames = new System.Windows.Forms.ComboBox();
      this.combo_prof_d3d_qe_mode = new System.Windows.Forms.ComboBox();
      this.combo_prof_d3d_vsync_mode = new System.Windows.Forms.ComboBox();
      this.combo_prof_names = new System.Windows.Forms.ComboBox();
      this.combo_prof_ogl_aniso_mode = new System.Windows.Forms.ComboBox();
      this.combo_prof_ogl_fsaa_mode = new System.Windows.Forms.ComboBox();
      this.combo_prof_ogl_lod_bias = new System.Windows.Forms.ComboBox();
      this.combo_prof_ogl_prerender_frames = new System.Windows.Forms.ComboBox();
      this.combo_prof_ogl_qe_mode = new System.Windows.Forms.ComboBox();
      this.combo_prof_ogl_vsync_mode = new System.Windows.Forms.ComboBox();
      this.dialog_prof_choose_exec = new System.Windows.Forms.OpenFileDialog();
      this.label_extra2_prof_ogl = new System.Windows.Forms.Label();
      this.label_extra2_prof_d3d = new System.Windows.Forms.Label();
      this.combo_extra2_curr_d3d_6 = new System.Windows.Forms.ComboBox();
      this.label_extra2_curr_ogl = new System.Windows.Forms.Label();
      this.label_extra2_curr_d3d = new System.Windows.Forms.Label();
      this.group_main_d3d = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label_d3d = new System.Windows.Forms.Label();
      this.label_prof_d3d = new System.Windows.Forms.Label();
      this.label_prerender_frames = new System.Windows.Forms.Label();
      this.label_fsaa_mode = new System.Windows.Forms.Label();
      this.label_vsync_mode = new System.Windows.Forms.Label();
      this.label_lod_bias = new System.Windows.Forms.Label();
      this.label_aniso_mode = new System.Windows.Forms.Label();
      this.label_quality = new System.Windows.Forms.Label();
      this.label_ogl = new System.Windows.Forms.Label();
      this.label_prof_ogl = new System.Windows.Forms.Label();
      this.group_extra_d3d = new System.Windows.Forms.GroupBox();
      this.label_extra_combo_d3d_8 = new System.Windows.Forms.Label();
      this.combo_extra_curr_d3d_8 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_d3d_8 = new System.Windows.Forms.ComboBox();
      this.label_extra_combo_d3d_7 = new System.Windows.Forms.Label();
      this.combo_extra_curr_d3d_7 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_d3d_7 = new System.Windows.Forms.ComboBox();
      this.label_extra_combo_d3d_1 = new System.Windows.Forms.Label();
      this.label_extra_combo_d3d_4 = new System.Windows.Forms.Label();
      this.label_extra_combo_d3d_5 = new System.Windows.Forms.Label();
      this.label_extra_combo_d3d_3 = new System.Windows.Forms.Label();
      this.label_extra_combo_d3d_2 = new System.Windows.Forms.Label();
      this.label_extra_combo_d3d_6 = new System.Windows.Forms.Label();
      this.label_extra_curr_d3d = new System.Windows.Forms.Label();
      this.label_extra_prof_d3d = new System.Windows.Forms.Label();
      this.combo_extra2_curr_d3d_8 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_d3d_8 = new System.Windows.Forms.ComboBox();
      this.label_extra2_combo_d3d_8 = new System.Windows.Forms.Label();
      this.combo_extra2_curr_d3d_7 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_d3d_7 = new System.Windows.Forms.ComboBox();
      this.label_extra2_combo_d3d_7 = new System.Windows.Forms.Label();
      this.label_extra2_combo_d3d_6 = new System.Windows.Forms.Label();
      this.label_extra2_combo_d3d_5 = new System.Windows.Forms.Label();
      this.label_extra2_combo_d3d_2 = new System.Windows.Forms.Label();
      this.label_extra2_combo_d3d_3 = new System.Windows.Forms.Label();
      this.label_extra2_combo_d3d_4 = new System.Windows.Forms.Label();
      this.label_extra2_combo_d3d_1 = new System.Windows.Forms.Label();
      this.label_extra_curr_ogl = new System.Windows.Forms.Label();
      this.group_extra_ogl = new System.Windows.Forms.GroupBox();
      this.combo_extra_prof_ogl_8 = new System.Windows.Forms.ComboBox();
      this.label_extra_combo_ogl_8 = new System.Windows.Forms.Label();
      this.combo_extra_curr_ogl_8 = new System.Windows.Forms.ComboBox();
      this.combo_extra_prof_ogl_7 = new System.Windows.Forms.ComboBox();
      this.label_extra_combo_ogl_7 = new System.Windows.Forms.Label();
      this.combo_extra_curr_ogl_7 = new System.Windows.Forms.ComboBox();
      this.label_extra_prof_ogl = new System.Windows.Forms.Label();
      this.label_extra_combo_ogl_1 = new System.Windows.Forms.Label();
      this.label_extra_combo_ogl_4 = new System.Windows.Forms.Label();
      this.label_extra_combo_ogl_5 = new System.Windows.Forms.Label();
      this.label_extra_combo_ogl_3 = new System.Windows.Forms.Label();
      this.label_extra_combo_ogl_2 = new System.Windows.Forms.Label();
      this.label_extra_combo_ogl_6 = new System.Windows.Forms.Label();
      this.label_extra2_combo_ogl_8 = new System.Windows.Forms.Label();
      this.combo_extra2_curr_ogl_8 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_ogl_8 = new System.Windows.Forms.ComboBox();
      this.label_extra2_combo_ogl_7 = new System.Windows.Forms.Label();
      this.combo_extra2_curr_ogl_7 = new System.Windows.Forms.ComboBox();
      this.combo_extra2_prof_ogl_7 = new System.Windows.Forms.ComboBox();
      this.label_extra2_combo_ogl_1 = new System.Windows.Forms.Label();
      this.label_extra2_combo_ogl_4 = new System.Windows.Forms.Label();
      this.label_extra2_combo_ogl_5 = new System.Windows.Forms.Label();
      this.label_extra2_combo_ogl_3 = new System.Windows.Forms.Label();
      this.label_extra2_combo_ogl_2 = new System.Windows.Forms.Label();
      this.label_extra2_combo_ogl_6 = new System.Windows.Forms.Label();
      this.combo_prof_img = new System.Windows.Forms.ComboBox();
      this.text_prof_exe_args = new System.Windows.Forms.TextBox();
      this.text_prof_exe_path = new System.Windows.Forms.TextBox();
      this.mainMenu = new System.Windows.Forms.MainMenu();
      this.menu_file = new System.Windows.Forms.MenuItem();
      this.menu_file_loadprofs = new System.Windows.Forms.MenuItem();
      this.menu_file_reloadCurrDriverSettings = new System.Windows.Forms.MenuItem();
      this.menuItem16 = new System.Windows.Forms.MenuItem();
      this.menu_file_iconifyTray = new System.Windows.Forms.MenuItem();
      this.menu_file_quit = new System.Windows.Forms.MenuItem();
      this.menu_options = new System.Windows.Forms.MenuItem();
      this.menu_opts_lang = new System.Windows.Forms.MenuItem();
      this.menu_opt_lang_auto = new System.Windows.Forms.MenuItem();
      this.menu_opt_lang_en = new System.Windows.Forms.MenuItem();
      this.menu_opt_lang_de = new System.Windows.Forms.MenuItem();
      this.menuItem1 = new System.Windows.Forms.MenuItem();
      this.menu_opts_regreadonly = new System.Windows.Forms.MenuItem();
      this.menuItem17 = new System.Windows.Forms.MenuItem();
      this.menu_opts_menuIcons = new System.Windows.Forms.MenuItem();
      this.menu_opt_3DCheckBoxes = new System.Windows.Forms.MenuItem();
      this.menuItem8 = new System.Windows.Forms.MenuItem();
      this.menu_opts_multiUser = new System.Windows.Forms.MenuItem();
      this.menu_opts_autoStart = new System.Windows.Forms.MenuItem();
      this.menuItem15 = new System.Windows.Forms.MenuItem();
      this.menu_opts_hotkeys = new System.Windows.Forms.MenuItem();
      this.menuItem27 = new System.Windows.Forms.MenuItem();
      this.menu_opts_settings = new System.Windows.Forms.MenuItem();
      this.menu_tools = new System.Windows.Forms.MenuItem();
      this.menu_tools_openRegedit = new System.Windows.Forms.MenuItem();
      this.menu_exp_testRdKey = new System.Windows.Forms.MenuItem();
      this.menu_tools_regdiff_ati = new System.Windows.Forms.MenuItem();
      this.menuItem5 = new System.Windows.Forms.MenuItem();
      this.menu_tools_nvclock = new System.Windows.Forms.MenuItem();
      this.menu_tools_nvclock_log = new System.Windows.Forms.MenuItem();
      this.menuItem19 = new System.Windows.Forms.MenuItem();
      this.menuItem20 = new System.Windows.Forms.MenuItem();
      this.menu_nvCoolBits = new System.Windows.Forms.MenuItem();
      this.menu_nvCoolBits_clocking = new System.Windows.Forms.MenuItem();
      this.menu_ati_D3DApply = new System.Windows.Forms.MenuItem();
      this.menu_ati_open_cpl = new System.Windows.Forms.MenuItem();
      this.menu_ati_open_oldCpl = new System.Windows.Forms.MenuItem();
      this.menu_winTweaks_Separator = new System.Windows.Forms.MenuItem();
      this.menu_winTweaks = new System.Windows.Forms.MenuItem();
      this.menu_winTweak_disablePageExecutive = new System.Windows.Forms.MenuItem();
      this.menu_ati_lsc = new System.Windows.Forms.MenuItem();
      this.menuItem21 = new System.Windows.Forms.MenuItem();
      this.menu_tools_undoApply = new System.Windows.Forms.MenuItem();
      this.menu_profile = new System.Windows.Forms.MenuItem();
      this.menuItem14 = new System.Windows.Forms.MenuItem();
      this.menu_prof_exploreExePath = new System.Windows.Forms.MenuItem();
      this.menu_profile_ini = new System.Windows.Forms.MenuItem();
      this.menu_profile_ini_edit = new System.Windows.Forms.MenuItem();
      this.menu_profile_ini_find = new System.Windows.Forms.MenuItem();
      this.menuItem3 = new System.Windows.Forms.MenuItem();
      this.menuItem12 = new System.Windows.Forms.MenuItem();
      this.menu_prof_daemon_drive_nmb = new System.Windows.Forms.MenuItem();
      this.menu_prof_daemon_drive_nmb_0 = new System.Windows.Forms.MenuItem();
      this.menu_prof_daemon_drive_nmb_1 = new System.Windows.Forms.MenuItem();
      this.menu_prof_daemon_drive_nmb_2 = new System.Windows.Forms.MenuItem();
      this.menu_prof_daemon_drive_nmb_3 = new System.Windows.Forms.MenuItem();
      this.menu_prof_imageFiles = new System.Windows.Forms.MenuItem();
      this.menu_prof_img_file_replace = new System.Windows.Forms.MenuItem();
      this.menu_prof_img_file_replaceAll = new System.Windows.Forms.MenuItem();
      this.menu_prof_img_file_add = new System.Windows.Forms.MenuItem();
      this.menu_prof_img_file_remove = new System.Windows.Forms.MenuItem();
      this.menu_prof_img_file_removeAll = new System.Windows.Forms.MenuItem();
      this.menu_prof_prio = new System.Windows.Forms.MenuItem();
      this.menu_prof_prio_high = new System.Windows.Forms.MenuItem();
      this.menu_prof_prio_aboveNormal = new System.Windows.Forms.MenuItem();
      this.menu_prof_prio_normal = new System.Windows.Forms.MenuItem();
      this.menu_prof_prio_belowNormal = new System.Windows.Forms.MenuItem();
      this.menu_prof_prio_idle = new System.Windows.Forms.MenuItem();
      this.menu_prof_freeMem = new System.Windows.Forms.MenuItem();
      this.menu_prof_freeMem_none = new System.Windows.Forms.MenuItem();
      this.menu_prof_freeMem_64mb = new System.Windows.Forms.MenuItem();
      this.menu_prof_freeMem_128mb = new System.Windows.Forms.MenuItem();
      this.menu_prof_freeMem_256mb = new System.Windows.Forms.MenuItem();
      this.menu_prof_freeMem_384mb = new System.Windows.Forms.MenuItem();
      this.menu_prof_freeMem_512mb = new System.Windows.Forms.MenuItem();
      this.menu_prof_freeMem_max = new System.Windows.Forms.MenuItem();
      this.menu_prof_autoRestore = new System.Windows.Forms.MenuItem();
      this.menu_prof_autoRestore_default = new System.Windows.Forms.MenuItem();
      this.menu_prof_autoRestore_forceOff = new System.Windows.Forms.MenuItem();
      this.menu_prof_autoRestore_forceDialog = new System.Windows.Forms.MenuItem();
      this.menu_prof_autoRestore_disableDialog = new System.Windows.Forms.MenuItem();
      this.menuItem6 = new System.Windows.Forms.MenuItem();
      this.menu_prof_detectAPI = new System.Windows.Forms.MenuItem();
      this.menu_prof_tdprofGD = new System.Windows.Forms.MenuItem();
      this.menu_prof_tdprofGD_create = new System.Windows.Forms.MenuItem();
      this.menu_prof_tdprofGD_remove = new System.Windows.Forms.MenuItem();
      this.menu_prof_tdprofGD_help = new System.Windows.Forms.MenuItem();
      this.menu_prof_mouseWare = new System.Windows.Forms.MenuItem();
      this.menu_prof_mouseWare_noAccel = new System.Windows.Forms.MenuItem();
      this.menuItem9 = new System.Windows.Forms.MenuItem();
      this.menu_prof_setSpecName = new System.Windows.Forms.MenuItem();
      this.menu_prof_importProfile = new System.Windows.Forms.MenuItem();
      this.menuItem24 = new System.Windows.Forms.MenuItem();
      this.menu_profs_templates_turnTo2D = new System.Windows.Forms.MenuItem();
      this.menu_profs_templates_clockOnly = new System.Windows.Forms.MenuItem();
      this.menuItem28 = new System.Windows.Forms.MenuItem();
      this.menu_profs = new System.Windows.Forms.MenuItem();
      this.menu_profs_filterBySpecName = new System.Windows.Forms.MenuItem();
      this.menu_profs_cmds = new System.Windows.Forms.MenuItem();
      this.menu_profs_hotkeys = new System.Windows.Forms.MenuItem();
      this.menu_profs_exim = new System.Windows.Forms.MenuItem();
      this.menu_help = new System.Windows.Forms.MenuItem();
      this.menu_help_tooltips = new System.Windows.Forms.MenuItem();
      this.menuItem4 = new System.Windows.Forms.MenuItem();
      this.menu_help_intro = new System.Windows.Forms.MenuItem();
      this.menu_help_news = new System.Windows.Forms.MenuItem();
      this.menu_help_todo = new System.Windows.Forms.MenuItem();
      this.menu_help_manual = new System.Windows.Forms.MenuItem();
      this.menuItem18 = new System.Windows.Forms.MenuItem();
      this.menu_help_visit_home = new System.Windows.Forms.MenuItem();
      this.menu_help_visit_thread = new System.Windows.Forms.MenuItem();
      this.menu_help_visit_thread_3DC = new System.Windows.Forms.MenuItem();
      this.menu_help_visit_thread_R3D = new System.Windows.Forms.MenuItem();
      this.menu_help_ati_visit_radeonFAQ = new System.Windows.Forms.MenuItem();
      this.menu_help_mailto_author = new System.Windows.Forms.MenuItem();
      this.menuItem10 = new System.Windows.Forms.MenuItem();
      this.menu_help_about = new System.Windows.Forms.MenuItem();
      this.menu_help_showLicense = new System.Windows.Forms.MenuItem();
      this.menu_help_nonWarranty = new System.Windows.Forms.MenuItem();
      this.menuItem2 = new System.Windows.Forms.MenuItem();
      this.menu_help_report = new System.Windows.Forms.MenuItem();
      this.menu_help_report_nvclockDebug = new System.Windows.Forms.MenuItem();
      this.menu_help_report_r6clockDebug = new System.Windows.Forms.MenuItem();
      this.menu_help_report_displayInfo = new System.Windows.Forms.MenuItem();
      this.menu_help_log = new System.Windows.Forms.MenuItem();
      this.menu_help_sysInfo = new System.Windows.Forms.MenuItem();
      this.menu_sfEdit = new System.Windows.Forms.MenuItem();
      this.menu_exp = new System.Windows.Forms.MenuItem();
      this.menuI_exp_findProc = new System.Windows.Forms.MenuItem();
      this.menu_help_helpButton = new System.Windows.Forms.MenuItem();
      this.menu_file_debugThrow = new System.Windows.Forms.MenuItem();
      this.menu_file_debugCrashMe = new System.Windows.Forms.MenuItem();
      this.menu_file_restore3d = new System.Windows.Forms.MenuItem();
      this.menu_file_restore3d_saveToFile = new System.Windows.Forms.MenuItem();
      this.menu_file_restore3d_loadLastSaved = new System.Windows.Forms.MenuItem();
      this.menu_file_restore3d_loadSavedByRun = new System.Windows.Forms.MenuItem();
      this.menu_file_restore3d_loadAutoSaved = new System.Windows.Forms.MenuItem();
      this.menu_exp_test1 = new System.Windows.Forms.MenuItem();
      this.menuItem13 = new System.Windows.Forms.MenuItem();
      this.menuItem22 = new System.Windows.Forms.MenuItem();
      this.tabCtrl = new System.Windows.Forms.TabControl();
      this.tab_main = new System.Windows.Forms.TabPage();
      this.tab_files = new System.Windows.Forms.TabPage();
      this.panel_prof_files = new System.Windows.Forms.Panel();
      this.panel_gameExe = new System.Windows.Forms.Panel();
      this.splitter_prof_gameExe = new System.Windows.Forms.Splitter();
      this.check_prof_shellLink = new System.Windows.Forms.CheckBox();
      this.tab_extra_d3d = new System.Windows.Forms.TabPage();
      this.group_extra_d3d_2 = new System.Windows.Forms.GroupBox();
      this.tab_extra_ogl = new System.Windows.Forms.TabPage();
      this.group_extra_ogl_2 = new System.Windows.Forms.GroupBox();
      this.tab_summary = new System.Windows.Forms.TabPage();
      this.text_summary = new System.Windows.Forms.RichTextBox();
      this.tab_clocking = new System.Windows.Forms.TabPage();
      this.group_clocking_curr = new System.Windows.Forms.GroupBox();
      this.button_clocking_set = new System.Windows.Forms.Button();
      this.button_clocking_reset = new System.Windows.Forms.Button();
      this.text_clocking_curr_core = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.track_clocking_curr_mem = new System.Windows.Forms.TrackBar();
      this.track_clocking_curr_core = new System.Windows.Forms.TrackBar();
      this.label1 = new System.Windows.Forms.Label();
      this.text_clocking_curr_mem = new System.Windows.Forms.TextBox();
      this.button_clocking_refresh = new System.Windows.Forms.Button();
      this.group_clocking_current_presets = new System.Windows.Forms.GroupBox();
      this.button_clocking_curr_preFast = new System.Windows.Forms.Button();
      this.button_clocking_curr_preUltra = new System.Windows.Forms.Button();
      this.button_clocking_curr_preNormal = new System.Windows.Forms.Button();
      this.button_clocking_curr_preSlow = new System.Windows.Forms.Button();
      this.splitter_clocking = new System.Windows.Forms.Splitter();
      this.group_clocking_prof = new System.Windows.Forms.GroupBox();
      this.panel_clocking_prof_clocks = new System.Windows.Forms.Panel();
      this.check_clocking_prof_mem = new System.Windows.Forms.CheckBox();
      this.text_clocking_prof_mem = new System.Windows.Forms.TextBox();
      this.track_clocking_prof_mem = new System.Windows.Forms.TrackBar();
      this.text_clocking_prof_core = new System.Windows.Forms.TextBox();
      this.label_clocking_prof_mem = new System.Windows.Forms.Label();
      this.track_clocking_prof_core = new System.Windows.Forms.TrackBar();
      this.check_clocking_prof_core = new System.Windows.Forms.CheckBox();
      this.button_clocking_disable = new System.Windows.Forms.Button();
      this.label_clocking_prof_core = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.combo_clocking_prof_presets = new System.Windows.Forms.ComboBox();
      this.tab_exp = new System.Windows.Forms.TabPage();
      this.picture_ind_d3d = new System.Windows.Forms.PictureBox();
      this.check_ind_ogl = new System.Windows.Forms.CheckBox();
      this.check_ind_d3d = new System.Windows.Forms.CheckBox();
      this.label_ind_mode = new System.Windows.Forms.Label();
      this.combo_ind_modeVal = new System.Windows.Forms.ComboBox();
      this.list_3d = new System.Windows.Forms.ListView();
      this.columnHeader_name = new System.Windows.Forms.ColumnHeader();
      this.columnHeader_profile = new System.Windows.Forms.ColumnHeader();
      this.columnHeader_driver = new System.Windows.Forms.ColumnHeader();
      this.picture_ind_ogl = new System.Windows.Forms.PictureBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.button_prof_new = new System.Windows.Forms.Button();
      this.button_prof_clone = new System.Windows.Forms.Button();
      this.text_prof_name = new System.Windows.Forms.TextBox();
      this.button_prof_cancel = new System.Windows.Forms.Button();
      this.button_prof_ok = new System.Windows.Forms.Button();
      this.panel_prof_apply = new System.Windows.Forms.Panel();
      this.button_prof_restore = new System.Windows.Forms.Button();
      this.group_prof = new System.Windows.Forms.Panel();
      this.button_prof_discard = new System.Windows.Forms.Button();
      this.button_prof_rename = new System.Windows.Forms.Button();
      this.toolBar1 = new System.Windows.Forms.ToolBar();
      this.toolButton_exploreGameFolder = new System.Windows.Forms.ToolBarButton();
      this.toolButton_editGameCfg = new System.Windows.Forms.ToolBarButton();
      this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
      this.toolButton_tools_regEdit = new System.Windows.Forms.ToolBarButton();
      this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
      this.toolButton_prof_commands = new System.Windows.Forms.ToolBarButton();
      this.toolButton_hotkeys = new System.Windows.Forms.ToolBarButton();
      this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
      this.toolButton_settings = new System.Windows.Forms.ToolBarButton();
      this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
      this.toolButton_help_onlineManual = new System.Windows.Forms.ToolBarButton();
      this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
      this.toolButton_compact = new System.Windows.Forms.ToolBarButton();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.timer_updateCr = new System.Windows.Forms.Timer(this.components);
      this.notifyIcon_tray = new System.Windows.Forms.NotifyIcon(this.components);
      this.context_tray = new System.Windows.Forms.ContextMenu();
      this.menu_tray_profs = new System.Windows.Forms.MenuItem();
      this.menu_tray_apply_exe = new System.Windows.Forms.MenuItem();
      this.menu_tray_apply = new System.Windows.Forms.MenuItem();
      this.menu_tray_applyExe = new System.Windows.Forms.MenuItem();
      this.menu_tray_profs_editGameIni = new System.Windows.Forms.MenuItem();
      this.menuItem7 = new System.Windows.Forms.MenuItem();
      this.menu_tray_profs_makeLink = new System.Windows.Forms.MenuItem();
      this.menuItem26 = new System.Windows.Forms.MenuItem();
      this.menu_tray_clocking_pre_slow = new System.Windows.Forms.MenuItem();
      this.menu_tray_clocking_pre_normal = new System.Windows.Forms.MenuItem();
      this.menu_tray_clocking_pre_fast = new System.Windows.Forms.MenuItem();
      this.menu_tray_clocking_pre_ultra = new System.Windows.Forms.MenuItem();
      this.menuItem11 = new System.Windows.Forms.MenuItem();
      this.menu_tray_img_mountCurr = new System.Windows.Forms.MenuItem();
      this.menu_tray_img_mountAnImgAtD0 = new System.Windows.Forms.MenuItem();
      this.menuItem25 = new System.Windows.Forms.MenuItem();
      this.menu_tray_tools = new System.Windows.Forms.MenuItem();
      this.menu_tray_tools_regEdit = new System.Windows.Forms.MenuItem();
      this.menu_tray_tools_regDiff = new System.Windows.Forms.MenuItem();
      this.menuItem23 = new System.Windows.Forms.MenuItem();
      this.menu_tray_stayInTray = new System.Windows.Forms.MenuItem();
      this.menu_tray_hideOnCloseBox = new System.Windows.Forms.MenuItem();
      this.menu_tray_sep = new System.Windows.Forms.MenuItem();
      this.menu_tray_quit = new System.Windows.Forms.MenuItem();
      this.group_main_d3d.SuspendLayout();
      this.group_extra_d3d.SuspendLayout();
      this.group_extra_ogl.SuspendLayout();
      this.tabCtrl.SuspendLayout();
      this.tab_main.SuspendLayout();
      this.tab_files.SuspendLayout();
      this.panel_prof_files.SuspendLayout();
      this.panel_gameExe.SuspendLayout();
      this.tab_extra_d3d.SuspendLayout();
      this.group_extra_d3d_2.SuspendLayout();
      this.tab_extra_ogl.SuspendLayout();
      this.group_extra_ogl_2.SuspendLayout();
      this.tab_summary.SuspendLayout();
      this.tab_clocking.SuspendLayout();
      this.group_clocking_curr.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.track_clocking_curr_mem)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.track_clocking_curr_core)).BeginInit();
      this.group_clocking_current_presets.SuspendLayout();
      this.group_clocking_prof.SuspendLayout();
      this.panel_clocking_prof_clocks.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.track_clocking_prof_mem)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.track_clocking_prof_core)).BeginInit();
      this.tab_exp.SuspendLayout();
      this.panel_prof_apply.SuspendLayout();
      this.group_prof.SuspendLayout();
      this.SuspendLayout();
      // 
      // button_prof_apply
      // 
      this.button_prof_apply.AccessibleDescription = resources.GetString("button_prof_apply.AccessibleDescription");
      this.button_prof_apply.AccessibleName = resources.GetString("button_prof_apply.AccessibleName");
      this.button_prof_apply.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_apply.Anchor")));
      this.button_prof_apply.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_apply.BackgroundImage")));
      this.button_prof_apply.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_apply.Dock")));
      this.button_prof_apply.Enabled = ((bool)(resources.GetObject("button_prof_apply.Enabled")));
      this.button_prof_apply.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_apply.FlatStyle")));
      this.button_prof_apply.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_apply.Font")));
      this.button_prof_apply.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_apply.Image")));
      this.button_prof_apply.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_apply.ImageAlign")));
      this.button_prof_apply.ImageIndex = ((int)(resources.GetObject("button_prof_apply.ImageIndex")));
      this.button_prof_apply.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_apply.ImeMode")));
      this.button_prof_apply.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_apply.Location")));
      this.button_prof_apply.Name = "button_prof_apply";
      this.button_prof_apply.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_apply.RightToLeft")));
      this.button_prof_apply.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_apply.Size")));
      this.button_prof_apply.TabIndex = ((int)(resources.GetObject("button_prof_apply.TabIndex")));
      this.button_prof_apply.Text = resources.GetString("button_prof_apply.Text");
      this.button_prof_apply.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_apply.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_apply, resources.GetString("button_prof_apply.ToolTip"));
      this.button_prof_apply.Visible = ((bool)(resources.GetObject("button_prof_apply.Visible")));
      this.button_prof_apply.Click += new System.EventHandler(this.button_prof_apply_Click);
      this.button_prof_apply.MouseEnter += new System.EventHandler(this.button_MouseEnter);
      this.button_prof_apply.MouseLeave += new System.EventHandler(this.button_MouseLeave);
      // 
      // button_prof_apply_and_run
      // 
      this.button_prof_apply_and_run.AccessibleDescription = resources.GetString("button_prof_apply_and_run.AccessibleDescription");
      this.button_prof_apply_and_run.AccessibleName = resources.GetString("button_prof_apply_and_run.AccessibleName");
      this.button_prof_apply_and_run.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_apply_and_run.Anchor")));
      this.button_prof_apply_and_run.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_apply_and_run.BackgroundImage")));
      this.button_prof_apply_and_run.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_apply_and_run.Dock")));
      this.button_prof_apply_and_run.Enabled = ((bool)(resources.GetObject("button_prof_apply_and_run.Enabled")));
      this.button_prof_apply_and_run.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_apply_and_run.FlatStyle")));
      this.button_prof_apply_and_run.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_apply_and_run.Font")));
      this.button_prof_apply_and_run.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_apply_and_run.Image")));
      this.button_prof_apply_and_run.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_apply_and_run.ImageAlign")));
      this.button_prof_apply_and_run.ImageIndex = ((int)(resources.GetObject("button_prof_apply_and_run.ImageIndex")));
      this.button_prof_apply_and_run.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_apply_and_run.ImeMode")));
      this.button_prof_apply_and_run.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_apply_and_run.Location")));
      this.button_prof_apply_and_run.Name = "button_prof_apply_and_run";
      this.button_prof_apply_and_run.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_apply_and_run.RightToLeft")));
      this.button_prof_apply_and_run.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_apply_and_run.Size")));
      this.button_prof_apply_and_run.TabIndex = ((int)(resources.GetObject("button_prof_apply_and_run.TabIndex")));
      this.button_prof_apply_and_run.Text = resources.GetString("button_prof_apply_and_run.Text");
      this.button_prof_apply_and_run.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_apply_and_run.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_apply_and_run, resources.GetString("button_prof_apply_and_run.ToolTip"));
      this.button_prof_apply_and_run.Visible = ((bool)(resources.GetObject("button_prof_apply_and_run.Visible")));
      this.button_prof_apply_and_run.Click += new System.EventHandler(this.button_prof_apply_and_run_Click);
      this.button_prof_apply_and_run.MouseEnter += new System.EventHandler(this.button_prof_apply_and_run_MouseEnter);
      this.button_prof_apply_and_run.MouseLeave += new System.EventHandler(this.button_prof_apply_and_run_MouseLeave);
      // 
      // button_prof_choose_exe
      // 
      this.button_prof_choose_exe.AccessibleDescription = resources.GetString("button_prof_choose_exe.AccessibleDescription");
      this.button_prof_choose_exe.AccessibleName = resources.GetString("button_prof_choose_exe.AccessibleName");
      this.button_prof_choose_exe.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_choose_exe.Anchor")));
      this.button_prof_choose_exe.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_choose_exe.BackgroundImage")));
      this.button_prof_choose_exe.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_choose_exe.Dock")));
      this.button_prof_choose_exe.Enabled = ((bool)(resources.GetObject("button_prof_choose_exe.Enabled")));
      this.button_prof_choose_exe.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_choose_exe.FlatStyle")));
      this.button_prof_choose_exe.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_choose_exe.Font")));
      this.button_prof_choose_exe.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_choose_exe.Image")));
      this.button_prof_choose_exe.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_choose_exe.ImageAlign")));
      this.button_prof_choose_exe.ImageIndex = ((int)(resources.GetObject("button_prof_choose_exe.ImageIndex")));
      this.button_prof_choose_exe.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_choose_exe.ImeMode")));
      this.button_prof_choose_exe.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_choose_exe.Location")));
      this.button_prof_choose_exe.Name = "button_prof_choose_exe";
      this.button_prof_choose_exe.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_choose_exe.RightToLeft")));
      this.button_prof_choose_exe.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_choose_exe.Size")));
      this.button_prof_choose_exe.TabIndex = ((int)(resources.GetObject("button_prof_choose_exe.TabIndex")));
      this.button_prof_choose_exe.Text = resources.GetString("button_prof_choose_exe.Text");
      this.button_prof_choose_exe.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_choose_exe.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_choose_exe, resources.GetString("button_prof_choose_exe.ToolTip"));
      this.button_prof_choose_exe.Visible = ((bool)(resources.GetObject("button_prof_choose_exe.Visible")));
      this.button_prof_choose_exe.Click += new System.EventHandler(this.button_prof_choose_exe_Click);
      // 
      // button_prof_choose_img
      // 
      this.button_prof_choose_img.AccessibleDescription = resources.GetString("button_prof_choose_img.AccessibleDescription");
      this.button_prof_choose_img.AccessibleName = resources.GetString("button_prof_choose_img.AccessibleName");
      this.button_prof_choose_img.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_choose_img.Anchor")));
      this.button_prof_choose_img.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_choose_img.BackgroundImage")));
      this.button_prof_choose_img.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_choose_img.Dock")));
      this.button_prof_choose_img.Enabled = ((bool)(resources.GetObject("button_prof_choose_img.Enabled")));
      this.button_prof_choose_img.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_choose_img.FlatStyle")));
      this.button_prof_choose_img.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_choose_img.Font")));
      this.button_prof_choose_img.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_choose_img.Image")));
      this.button_prof_choose_img.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_choose_img.ImageAlign")));
      this.button_prof_choose_img.ImageIndex = ((int)(resources.GetObject("button_prof_choose_img.ImageIndex")));
      this.button_prof_choose_img.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_choose_img.ImeMode")));
      this.button_prof_choose_img.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_choose_img.Location")));
      this.button_prof_choose_img.Name = "button_prof_choose_img";
      this.button_prof_choose_img.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_choose_img.RightToLeft")));
      this.button_prof_choose_img.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_choose_img.Size")));
      this.button_prof_choose_img.TabIndex = ((int)(resources.GetObject("button_prof_choose_img.TabIndex")));
      this.button_prof_choose_img.Text = resources.GetString("button_prof_choose_img.Text");
      this.button_prof_choose_img.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_choose_img.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_choose_img, resources.GetString("button_prof_choose_img.ToolTip"));
      this.button_prof_choose_img.Visible = ((bool)(resources.GetObject("button_prof_choose_img.Visible")));
      this.button_prof_choose_img.Click += new System.EventHandler(this.button_prof_choose_img_Click);
      // 
      // button_prof_delete
      // 
      this.button_prof_delete.AccessibleDescription = resources.GetString("button_prof_delete.AccessibleDescription");
      this.button_prof_delete.AccessibleName = resources.GetString("button_prof_delete.AccessibleName");
      this.button_prof_delete.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_delete.Anchor")));
      this.button_prof_delete.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_delete.BackgroundImage")));
      this.button_prof_delete.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_delete.Dock")));
      this.button_prof_delete.Enabled = ((bool)(resources.GetObject("button_prof_delete.Enabled")));
      this.button_prof_delete.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_delete.FlatStyle")));
      this.button_prof_delete.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_delete.Font")));
      this.button_prof_delete.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_delete.Image")));
      this.button_prof_delete.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_delete.ImageAlign")));
      this.button_prof_delete.ImageIndex = ((int)(resources.GetObject("button_prof_delete.ImageIndex")));
      this.button_prof_delete.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_delete.ImeMode")));
      this.button_prof_delete.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_delete.Location")));
      this.button_prof_delete.Name = "button_prof_delete";
      this.button_prof_delete.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_delete.RightToLeft")));
      this.button_prof_delete.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_delete.Size")));
      this.button_prof_delete.TabIndex = ((int)(resources.GetObject("button_prof_delete.TabIndex")));
      this.button_prof_delete.Text = resources.GetString("button_prof_delete.Text");
      this.button_prof_delete.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_delete.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_delete, resources.GetString("button_prof_delete.ToolTip"));
      this.button_prof_delete.Visible = ((bool)(resources.GetObject("button_prof_delete.Visible")));
      this.button_prof_delete.Click += new System.EventHandler(this.button_prof_delete_Click);
      this.button_prof_delete.MouseEnter += new System.EventHandler(this.button_MouseEnter);
      this.button_prof_delete.MouseLeave += new System.EventHandler(this.button_MouseLeave);
      // 
      // button_prof_make_link
      // 
      this.button_prof_make_link.AccessibleDescription = resources.GetString("button_prof_make_link.AccessibleDescription");
      this.button_prof_make_link.AccessibleName = resources.GetString("button_prof_make_link.AccessibleName");
      this.button_prof_make_link.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_make_link.Anchor")));
      this.button_prof_make_link.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_make_link.BackgroundImage")));
      this.button_prof_make_link.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_make_link.Dock")));
      this.button_prof_make_link.Enabled = ((bool)(resources.GetObject("button_prof_make_link.Enabled")));
      this.button_prof_make_link.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_make_link.FlatStyle")));
      this.button_prof_make_link.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_make_link.Font")));
      this.button_prof_make_link.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_make_link.Image")));
      this.button_prof_make_link.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_make_link.ImageAlign")));
      this.button_prof_make_link.ImageIndex = ((int)(resources.GetObject("button_prof_make_link.ImageIndex")));
      this.button_prof_make_link.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_make_link.ImeMode")));
      this.button_prof_make_link.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_make_link.Location")));
      this.button_prof_make_link.Name = "button_prof_make_link";
      this.button_prof_make_link.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_make_link.RightToLeft")));
      this.button_prof_make_link.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_make_link.Size")));
      this.button_prof_make_link.TabIndex = ((int)(resources.GetObject("button_prof_make_link.TabIndex")));
      this.button_prof_make_link.Text = resources.GetString("button_prof_make_link.Text");
      this.button_prof_make_link.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_make_link.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_make_link, resources.GetString("button_prof_make_link.ToolTip"));
      this.button_prof_make_link.Visible = ((bool)(resources.GetObject("button_prof_make_link.Visible")));
      this.button_prof_make_link.Click += new System.EventHandler(this.button_prof_make_link_Click);
      this.button_prof_make_link.MouseEnter += new System.EventHandler(this.button_MouseEnter);
      this.button_prof_make_link.MouseLeave += new System.EventHandler(this.button_MouseLeave);
      // 
      // button_prof_mount_img
      // 
      this.button_prof_mount_img.AccessibleDescription = resources.GetString("button_prof_mount_img.AccessibleDescription");
      this.button_prof_mount_img.AccessibleName = resources.GetString("button_prof_mount_img.AccessibleName");
      this.button_prof_mount_img.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_mount_img.Anchor")));
      this.button_prof_mount_img.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_mount_img.BackgroundImage")));
      this.button_prof_mount_img.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_mount_img.Dock")));
      this.button_prof_mount_img.Enabled = ((bool)(resources.GetObject("button_prof_mount_img.Enabled")));
      this.button_prof_mount_img.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_mount_img.FlatStyle")));
      this.button_prof_mount_img.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_mount_img.Font")));
      this.button_prof_mount_img.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_mount_img.Image")));
      this.button_prof_mount_img.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_mount_img.ImageAlign")));
      this.button_prof_mount_img.ImageIndex = ((int)(resources.GetObject("button_prof_mount_img.ImageIndex")));
      this.button_prof_mount_img.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_mount_img.ImeMode")));
      this.button_prof_mount_img.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_mount_img.Location")));
      this.button_prof_mount_img.Name = "button_prof_mount_img";
      this.button_prof_mount_img.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_mount_img.RightToLeft")));
      this.button_prof_mount_img.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_mount_img.Size")));
      this.button_prof_mount_img.TabIndex = ((int)(resources.GetObject("button_prof_mount_img.TabIndex")));
      this.button_prof_mount_img.Text = resources.GetString("button_prof_mount_img.Text");
      this.button_prof_mount_img.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_mount_img.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_mount_img, resources.GetString("button_prof_mount_img.ToolTip"));
      this.button_prof_mount_img.Visible = ((bool)(resources.GetObject("button_prof_mount_img.Visible")));
      this.button_prof_mount_img.Click += new System.EventHandler(this.button_prof_mount_img_Click);
      this.button_prof_mount_img.MouseEnter += new System.EventHandler(this.button_MouseEnter);
      this.button_prof_mount_img.MouseLeave += new System.EventHandler(this.button_MouseLeave);
      // 
      // button_prof_run_exe
      // 
      this.button_prof_run_exe.AccessibleDescription = resources.GetString("button_prof_run_exe.AccessibleDescription");
      this.button_prof_run_exe.AccessibleName = resources.GetString("button_prof_run_exe.AccessibleName");
      this.button_prof_run_exe.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_run_exe.Anchor")));
      this.button_prof_run_exe.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_run_exe.BackgroundImage")));
      this.button_prof_run_exe.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_run_exe.Dock")));
      this.button_prof_run_exe.Enabled = ((bool)(resources.GetObject("button_prof_run_exe.Enabled")));
      this.button_prof_run_exe.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_run_exe.FlatStyle")));
      this.button_prof_run_exe.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_run_exe.Font")));
      this.button_prof_run_exe.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_run_exe.Image")));
      this.button_prof_run_exe.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_run_exe.ImageAlign")));
      this.button_prof_run_exe.ImageIndex = ((int)(resources.GetObject("button_prof_run_exe.ImageIndex")));
      this.button_prof_run_exe.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_run_exe.ImeMode")));
      this.button_prof_run_exe.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_run_exe.Location")));
      this.button_prof_run_exe.Name = "button_prof_run_exe";
      this.button_prof_run_exe.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_run_exe.RightToLeft")));
      this.button_prof_run_exe.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_run_exe.Size")));
      this.button_prof_run_exe.TabIndex = ((int)(resources.GetObject("button_prof_run_exe.TabIndex")));
      this.button_prof_run_exe.Text = resources.GetString("button_prof_run_exe.Text");
      this.button_prof_run_exe.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_run_exe.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_run_exe, resources.GetString("button_prof_run_exe.ToolTip"));
      this.button_prof_run_exe.Visible = ((bool)(resources.GetObject("button_prof_run_exe.Visible")));
      this.button_prof_run_exe.Click += new System.EventHandler(this.button_prof_run_exe_Click);
      this.button_prof_run_exe.MouseEnter += new System.EventHandler(this.button_MouseEnter);
      this.button_prof_run_exe.MouseLeave += new System.EventHandler(this.button_MouseLeave);
      // 
      // button_prof_save
      // 
      this.button_prof_save.AccessibleDescription = resources.GetString("button_prof_save.AccessibleDescription");
      this.button_prof_save.AccessibleName = resources.GetString("button_prof_save.AccessibleName");
      this.button_prof_save.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_save.Anchor")));
      this.button_prof_save.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_save.BackgroundImage")));
      this.button_prof_save.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_save.Dock")));
      this.button_prof_save.Enabled = ((bool)(resources.GetObject("button_prof_save.Enabled")));
      this.button_prof_save.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_save.FlatStyle")));
      this.button_prof_save.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_save.Font")));
      this.button_prof_save.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_save.Image")));
      this.button_prof_save.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_save.ImageAlign")));
      this.button_prof_save.ImageIndex = ((int)(resources.GetObject("button_prof_save.ImageIndex")));
      this.button_prof_save.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_save.ImeMode")));
      this.button_prof_save.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_save.Location")));
      this.button_prof_save.Name = "button_prof_save";
      this.button_prof_save.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_save.RightToLeft")));
      this.button_prof_save.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_save.Size")));
      this.button_prof_save.TabIndex = ((int)(resources.GetObject("button_prof_save.TabIndex")));
      this.button_prof_save.Text = resources.GetString("button_prof_save.Text");
      this.button_prof_save.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_save.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_save, resources.GetString("button_prof_save.ToolTip"));
      this.button_prof_save.Visible = ((bool)(resources.GetObject("button_prof_save.Visible")));
      this.button_prof_save.Click += new System.EventHandler(this.button_prof_save_Click);
      this.button_prof_save.MouseEnter += new System.EventHandler(this.button_MouseEnter);
      this.button_prof_save.MouseLeave += new System.EventHandler(this.button_MouseLeave);
      // 
      // check_prof_quit
      // 
      this.check_prof_quit.AccessibleDescription = resources.GetString("check_prof_quit.AccessibleDescription");
      this.check_prof_quit.AccessibleName = resources.GetString("check_prof_quit.AccessibleName");
      this.check_prof_quit.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_prof_quit.Anchor")));
      this.check_prof_quit.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_prof_quit.Appearance")));
      this.check_prof_quit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_prof_quit.BackgroundImage")));
      this.check_prof_quit.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_quit.CheckAlign")));
      this.check_prof_quit.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_prof_quit.Dock")));
      this.check_prof_quit.Enabled = ((bool)(resources.GetObject("check_prof_quit.Enabled")));
      this.check_prof_quit.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_prof_quit.FlatStyle")));
      this.check_prof_quit.Font = ((System.Drawing.Font)(resources.GetObject("check_prof_quit.Font")));
      this.check_prof_quit.Image = ((System.Drawing.Image)(resources.GetObject("check_prof_quit.Image")));
      this.check_prof_quit.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_quit.ImageAlign")));
      this.check_prof_quit.ImageIndex = ((int)(resources.GetObject("check_prof_quit.ImageIndex")));
      this.check_prof_quit.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_prof_quit.ImeMode")));
      this.check_prof_quit.Location = ((System.Drawing.Point)(resources.GetObject("check_prof_quit.Location")));
      this.check_prof_quit.Name = "check_prof_quit";
      this.check_prof_quit.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_prof_quit.RightToLeft")));
      this.check_prof_quit.Size = ((System.Drawing.Size)(resources.GetObject("check_prof_quit.Size")));
      this.check_prof_quit.TabIndex = ((int)(resources.GetObject("check_prof_quit.TabIndex")));
      this.check_prof_quit.Text = resources.GetString("check_prof_quit.Text");
      this.check_prof_quit.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_quit.TextAlign")));
      this.toolTip.SetToolTip(this.check_prof_quit, resources.GetString("check_prof_quit.ToolTip"));
      this.check_prof_quit.Visible = ((bool)(resources.GetObject("check_prof_quit.Visible")));
      this.check_prof_quit.CheckedChanged += new System.EventHandler(this.check_prof_quit_CheckedChanged);
      // 
      // combo_d3d_aniso_mode
      // 
      this.combo_d3d_aniso_mode.AccessibleDescription = resources.GetString("combo_d3d_aniso_mode.AccessibleDescription");
      this.combo_d3d_aniso_mode.AccessibleName = resources.GetString("combo_d3d_aniso_mode.AccessibleName");
      this.combo_d3d_aniso_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_d3d_aniso_mode.Anchor")));
      this.combo_d3d_aniso_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_d3d_aniso_mode.BackgroundImage")));
      this.combo_d3d_aniso_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_d3d_aniso_mode.Dock")));
      this.combo_d3d_aniso_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_d3d_aniso_mode.Enabled = ((bool)(resources.GetObject("combo_d3d_aniso_mode.Enabled")));
      this.combo_d3d_aniso_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_d3d_aniso_mode.Font")));
      this.combo_d3d_aniso_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_d3d_aniso_mode.ImeMode")));
      this.combo_d3d_aniso_mode.IntegralHeight = ((bool)(resources.GetObject("combo_d3d_aniso_mode.IntegralHeight")));
      this.combo_d3d_aniso_mode.ItemHeight = ((int)(resources.GetObject("combo_d3d_aniso_mode.ItemHeight")));
      this.combo_d3d_aniso_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_d3d_aniso_mode.Location")));
      this.combo_d3d_aniso_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_d3d_aniso_mode.MaxDropDownItems")));
      this.combo_d3d_aniso_mode.MaxLength = ((int)(resources.GetObject("combo_d3d_aniso_mode.MaxLength")));
      this.combo_d3d_aniso_mode.Name = "combo_d3d_aniso_mode";
      this.combo_d3d_aniso_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_d3d_aniso_mode.RightToLeft")));
      this.combo_d3d_aniso_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_d3d_aniso_mode.Size")));
      this.combo_d3d_aniso_mode.TabIndex = ((int)(resources.GetObject("combo_d3d_aniso_mode.TabIndex")));
      this.combo_d3d_aniso_mode.Text = resources.GetString("combo_d3d_aniso_mode.Text");
      this.toolTip.SetToolTip(this.combo_d3d_aniso_mode, resources.GetString("combo_d3d_aniso_mode.ToolTip"));
      this.combo_d3d_aniso_mode.Visible = ((bool)(resources.GetObject("combo_d3d_aniso_mode.Visible")));
      // 
      // combo_d3d_fsaa_mode
      // 
      this.combo_d3d_fsaa_mode.AccessibleDescription = resources.GetString("combo_d3d_fsaa_mode.AccessibleDescription");
      this.combo_d3d_fsaa_mode.AccessibleName = resources.GetString("combo_d3d_fsaa_mode.AccessibleName");
      this.combo_d3d_fsaa_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_d3d_fsaa_mode.Anchor")));
      this.combo_d3d_fsaa_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_d3d_fsaa_mode.BackgroundImage")));
      this.combo_d3d_fsaa_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_d3d_fsaa_mode.Dock")));
      this.combo_d3d_fsaa_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_d3d_fsaa_mode.Enabled = ((bool)(resources.GetObject("combo_d3d_fsaa_mode.Enabled")));
      this.combo_d3d_fsaa_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_d3d_fsaa_mode.Font")));
      this.combo_d3d_fsaa_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_d3d_fsaa_mode.ImeMode")));
      this.combo_d3d_fsaa_mode.IntegralHeight = ((bool)(resources.GetObject("combo_d3d_fsaa_mode.IntegralHeight")));
      this.combo_d3d_fsaa_mode.ItemHeight = ((int)(resources.GetObject("combo_d3d_fsaa_mode.ItemHeight")));
      this.combo_d3d_fsaa_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_d3d_fsaa_mode.Location")));
      this.combo_d3d_fsaa_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_d3d_fsaa_mode.MaxDropDownItems")));
      this.combo_d3d_fsaa_mode.MaxLength = ((int)(resources.GetObject("combo_d3d_fsaa_mode.MaxLength")));
      this.combo_d3d_fsaa_mode.Name = "combo_d3d_fsaa_mode";
      this.combo_d3d_fsaa_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_d3d_fsaa_mode.RightToLeft")));
      this.combo_d3d_fsaa_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_d3d_fsaa_mode.Size")));
      this.combo_d3d_fsaa_mode.TabIndex = ((int)(resources.GetObject("combo_d3d_fsaa_mode.TabIndex")));
      this.combo_d3d_fsaa_mode.Text = resources.GetString("combo_d3d_fsaa_mode.Text");
      this.toolTip.SetToolTip(this.combo_d3d_fsaa_mode, resources.GetString("combo_d3d_fsaa_mode.ToolTip"));
      this.combo_d3d_fsaa_mode.Visible = ((bool)(resources.GetObject("combo_d3d_fsaa_mode.Visible")));
      // 
      // combo_d3d_lod_bias
      // 
      this.combo_d3d_lod_bias.AccessibleDescription = resources.GetString("combo_d3d_lod_bias.AccessibleDescription");
      this.combo_d3d_lod_bias.AccessibleName = resources.GetString("combo_d3d_lod_bias.AccessibleName");
      this.combo_d3d_lod_bias.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_d3d_lod_bias.Anchor")));
      this.combo_d3d_lod_bias.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_d3d_lod_bias.BackgroundImage")));
      this.combo_d3d_lod_bias.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_d3d_lod_bias.Dock")));
      this.combo_d3d_lod_bias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_d3d_lod_bias.Enabled = ((bool)(resources.GetObject("combo_d3d_lod_bias.Enabled")));
      this.combo_d3d_lod_bias.Font = ((System.Drawing.Font)(resources.GetObject("combo_d3d_lod_bias.Font")));
      this.combo_d3d_lod_bias.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_d3d_lod_bias.ImeMode")));
      this.combo_d3d_lod_bias.IntegralHeight = ((bool)(resources.GetObject("combo_d3d_lod_bias.IntegralHeight")));
      this.combo_d3d_lod_bias.ItemHeight = ((int)(resources.GetObject("combo_d3d_lod_bias.ItemHeight")));
      this.combo_d3d_lod_bias.Location = ((System.Drawing.Point)(resources.GetObject("combo_d3d_lod_bias.Location")));
      this.combo_d3d_lod_bias.MaxDropDownItems = ((int)(resources.GetObject("combo_d3d_lod_bias.MaxDropDownItems")));
      this.combo_d3d_lod_bias.MaxLength = ((int)(resources.GetObject("combo_d3d_lod_bias.MaxLength")));
      this.combo_d3d_lod_bias.Name = "combo_d3d_lod_bias";
      this.combo_d3d_lod_bias.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_d3d_lod_bias.RightToLeft")));
      this.combo_d3d_lod_bias.Size = ((System.Drawing.Size)(resources.GetObject("combo_d3d_lod_bias.Size")));
      this.combo_d3d_lod_bias.TabIndex = ((int)(resources.GetObject("combo_d3d_lod_bias.TabIndex")));
      this.combo_d3d_lod_bias.Text = resources.GetString("combo_d3d_lod_bias.Text");
      this.toolTip.SetToolTip(this.combo_d3d_lod_bias, resources.GetString("combo_d3d_lod_bias.ToolTip"));
      this.combo_d3d_lod_bias.Visible = ((bool)(resources.GetObject("combo_d3d_lod_bias.Visible")));
      // 
      // combo_d3d_prerender_frames
      // 
      this.combo_d3d_prerender_frames.AccessibleDescription = resources.GetString("combo_d3d_prerender_frames.AccessibleDescription");
      this.combo_d3d_prerender_frames.AccessibleName = resources.GetString("combo_d3d_prerender_frames.AccessibleName");
      this.combo_d3d_prerender_frames.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_d3d_prerender_frames.Anchor")));
      this.combo_d3d_prerender_frames.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_d3d_prerender_frames.BackgroundImage")));
      this.combo_d3d_prerender_frames.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_d3d_prerender_frames.Dock")));
      this.combo_d3d_prerender_frames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_d3d_prerender_frames.Enabled = ((bool)(resources.GetObject("combo_d3d_prerender_frames.Enabled")));
      this.combo_d3d_prerender_frames.Font = ((System.Drawing.Font)(resources.GetObject("combo_d3d_prerender_frames.Font")));
      this.combo_d3d_prerender_frames.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_d3d_prerender_frames.ImeMode")));
      this.combo_d3d_prerender_frames.IntegralHeight = ((bool)(resources.GetObject("combo_d3d_prerender_frames.IntegralHeight")));
      this.combo_d3d_prerender_frames.ItemHeight = ((int)(resources.GetObject("combo_d3d_prerender_frames.ItemHeight")));
      this.combo_d3d_prerender_frames.Location = ((System.Drawing.Point)(resources.GetObject("combo_d3d_prerender_frames.Location")));
      this.combo_d3d_prerender_frames.MaxDropDownItems = ((int)(resources.GetObject("combo_d3d_prerender_frames.MaxDropDownItems")));
      this.combo_d3d_prerender_frames.MaxLength = ((int)(resources.GetObject("combo_d3d_prerender_frames.MaxLength")));
      this.combo_d3d_prerender_frames.Name = "combo_d3d_prerender_frames";
      this.combo_d3d_prerender_frames.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_d3d_prerender_frames.RightToLeft")));
      this.combo_d3d_prerender_frames.Size = ((System.Drawing.Size)(resources.GetObject("combo_d3d_prerender_frames.Size")));
      this.combo_d3d_prerender_frames.TabIndex = ((int)(resources.GetObject("combo_d3d_prerender_frames.TabIndex")));
      this.combo_d3d_prerender_frames.Text = resources.GetString("combo_d3d_prerender_frames.Text");
      this.toolTip.SetToolTip(this.combo_d3d_prerender_frames, resources.GetString("combo_d3d_prerender_frames.ToolTip"));
      this.combo_d3d_prerender_frames.Visible = ((bool)(resources.GetObject("combo_d3d_prerender_frames.Visible")));
      // 
      // combo_d3d_qe_mode
      // 
      this.combo_d3d_qe_mode.AccessibleDescription = resources.GetString("combo_d3d_qe_mode.AccessibleDescription");
      this.combo_d3d_qe_mode.AccessibleName = resources.GetString("combo_d3d_qe_mode.AccessibleName");
      this.combo_d3d_qe_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_d3d_qe_mode.Anchor")));
      this.combo_d3d_qe_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_d3d_qe_mode.BackgroundImage")));
      this.combo_d3d_qe_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_d3d_qe_mode.Dock")));
      this.combo_d3d_qe_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_d3d_qe_mode.Enabled = ((bool)(resources.GetObject("combo_d3d_qe_mode.Enabled")));
      this.combo_d3d_qe_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_d3d_qe_mode.Font")));
      this.combo_d3d_qe_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_d3d_qe_mode.ImeMode")));
      this.combo_d3d_qe_mode.IntegralHeight = ((bool)(resources.GetObject("combo_d3d_qe_mode.IntegralHeight")));
      this.combo_d3d_qe_mode.ItemHeight = ((int)(resources.GetObject("combo_d3d_qe_mode.ItemHeight")));
      this.combo_d3d_qe_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_d3d_qe_mode.Location")));
      this.combo_d3d_qe_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_d3d_qe_mode.MaxDropDownItems")));
      this.combo_d3d_qe_mode.MaxLength = ((int)(resources.GetObject("combo_d3d_qe_mode.MaxLength")));
      this.combo_d3d_qe_mode.Name = "combo_d3d_qe_mode";
      this.combo_d3d_qe_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_d3d_qe_mode.RightToLeft")));
      this.combo_d3d_qe_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_d3d_qe_mode.Size")));
      this.combo_d3d_qe_mode.TabIndex = ((int)(resources.GetObject("combo_d3d_qe_mode.TabIndex")));
      this.combo_d3d_qe_mode.Text = resources.GetString("combo_d3d_qe_mode.Text");
      this.toolTip.SetToolTip(this.combo_d3d_qe_mode, resources.GetString("combo_d3d_qe_mode.ToolTip"));
      this.combo_d3d_qe_mode.Visible = ((bool)(resources.GetObject("combo_d3d_qe_mode.Visible")));
      // 
      // combo_d3d_vsync_mode
      // 
      this.combo_d3d_vsync_mode.AccessibleDescription = resources.GetString("combo_d3d_vsync_mode.AccessibleDescription");
      this.combo_d3d_vsync_mode.AccessibleName = resources.GetString("combo_d3d_vsync_mode.AccessibleName");
      this.combo_d3d_vsync_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_d3d_vsync_mode.Anchor")));
      this.combo_d3d_vsync_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_d3d_vsync_mode.BackgroundImage")));
      this.combo_d3d_vsync_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_d3d_vsync_mode.Dock")));
      this.combo_d3d_vsync_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_d3d_vsync_mode.Enabled = ((bool)(resources.GetObject("combo_d3d_vsync_mode.Enabled")));
      this.combo_d3d_vsync_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_d3d_vsync_mode.Font")));
      this.combo_d3d_vsync_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_d3d_vsync_mode.ImeMode")));
      this.combo_d3d_vsync_mode.IntegralHeight = ((bool)(resources.GetObject("combo_d3d_vsync_mode.IntegralHeight")));
      this.combo_d3d_vsync_mode.ItemHeight = ((int)(resources.GetObject("combo_d3d_vsync_mode.ItemHeight")));
      this.combo_d3d_vsync_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_d3d_vsync_mode.Location")));
      this.combo_d3d_vsync_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_d3d_vsync_mode.MaxDropDownItems")));
      this.combo_d3d_vsync_mode.MaxLength = ((int)(resources.GetObject("combo_d3d_vsync_mode.MaxLength")));
      this.combo_d3d_vsync_mode.Name = "combo_d3d_vsync_mode";
      this.combo_d3d_vsync_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_d3d_vsync_mode.RightToLeft")));
      this.combo_d3d_vsync_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_d3d_vsync_mode.Size")));
      this.combo_d3d_vsync_mode.TabIndex = ((int)(resources.GetObject("combo_d3d_vsync_mode.TabIndex")));
      this.combo_d3d_vsync_mode.Text = resources.GetString("combo_d3d_vsync_mode.Text");
      this.toolTip.SetToolTip(this.combo_d3d_vsync_mode, resources.GetString("combo_d3d_vsync_mode.ToolTip"));
      this.combo_d3d_vsync_mode.Visible = ((bool)(resources.GetObject("combo_d3d_vsync_mode.Visible")));
      // 
      // combo_extra2_curr_d3d_1
      // 
      this.combo_extra2_curr_d3d_1.AccessibleDescription = resources.GetString("combo_extra2_curr_d3d_1.AccessibleDescription");
      this.combo_extra2_curr_d3d_1.AccessibleName = resources.GetString("combo_extra2_curr_d3d_1.AccessibleName");
      this.combo_extra2_curr_d3d_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_d3d_1.Anchor")));
      this.combo_extra2_curr_d3d_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_d3d_1.BackgroundImage")));
      this.combo_extra2_curr_d3d_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_d3d_1.Dock")));
      this.combo_extra2_curr_d3d_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_d3d_1.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_d3d_1.Enabled")));
      this.combo_extra2_curr_d3d_1.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_d3d_1.Font")));
      this.combo_extra2_curr_d3d_1.ForeColor = System.Drawing.SystemColors.WindowText;
      this.combo_extra2_curr_d3d_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_d3d_1.ImeMode")));
      this.combo_extra2_curr_d3d_1.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_d3d_1.IntegralHeight")));
      this.combo_extra2_curr_d3d_1.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_d3d_1.ItemHeight")));
      this.combo_extra2_curr_d3d_1.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_d3d_1.Location")));
      this.combo_extra2_curr_d3d_1.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_d3d_1.MaxDropDownItems")));
      this.combo_extra2_curr_d3d_1.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_d3d_1.MaxLength")));
      this.combo_extra2_curr_d3d_1.Name = "combo_extra2_curr_d3d_1";
      this.combo_extra2_curr_d3d_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_d3d_1.RightToLeft")));
      this.combo_extra2_curr_d3d_1.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_d3d_1.Size")));
      this.combo_extra2_curr_d3d_1.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_d3d_1.TabIndex")));
      this.combo_extra2_curr_d3d_1.Text = resources.GetString("combo_extra2_curr_d3d_1.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_d3d_1, resources.GetString("combo_extra2_curr_d3d_1.ToolTip"));
      this.combo_extra2_curr_d3d_1.Visible = ((bool)(resources.GetObject("combo_extra2_curr_d3d_1.Visible")));
      // 
      // combo_extra2_curr_d3d_2
      // 
      this.combo_extra2_curr_d3d_2.AccessibleDescription = resources.GetString("combo_extra2_curr_d3d_2.AccessibleDescription");
      this.combo_extra2_curr_d3d_2.AccessibleName = resources.GetString("combo_extra2_curr_d3d_2.AccessibleName");
      this.combo_extra2_curr_d3d_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_d3d_2.Anchor")));
      this.combo_extra2_curr_d3d_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_d3d_2.BackgroundImage")));
      this.combo_extra2_curr_d3d_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_d3d_2.Dock")));
      this.combo_extra2_curr_d3d_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_d3d_2.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_d3d_2.Enabled")));
      this.combo_extra2_curr_d3d_2.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_d3d_2.Font")));
      this.combo_extra2_curr_d3d_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_d3d_2.ImeMode")));
      this.combo_extra2_curr_d3d_2.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_d3d_2.IntegralHeight")));
      this.combo_extra2_curr_d3d_2.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_d3d_2.ItemHeight")));
      this.combo_extra2_curr_d3d_2.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_d3d_2.Location")));
      this.combo_extra2_curr_d3d_2.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_d3d_2.MaxDropDownItems")));
      this.combo_extra2_curr_d3d_2.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_d3d_2.MaxLength")));
      this.combo_extra2_curr_d3d_2.Name = "combo_extra2_curr_d3d_2";
      this.combo_extra2_curr_d3d_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_d3d_2.RightToLeft")));
      this.combo_extra2_curr_d3d_2.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_d3d_2.Size")));
      this.combo_extra2_curr_d3d_2.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_d3d_2.TabIndex")));
      this.combo_extra2_curr_d3d_2.Text = resources.GetString("combo_extra2_curr_d3d_2.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_d3d_2, resources.GetString("combo_extra2_curr_d3d_2.ToolTip"));
      this.combo_extra2_curr_d3d_2.Visible = ((bool)(resources.GetObject("combo_extra2_curr_d3d_2.Visible")));
      // 
      // combo_extra2_curr_d3d_3
      // 
      this.combo_extra2_curr_d3d_3.AccessibleDescription = resources.GetString("combo_extra2_curr_d3d_3.AccessibleDescription");
      this.combo_extra2_curr_d3d_3.AccessibleName = resources.GetString("combo_extra2_curr_d3d_3.AccessibleName");
      this.combo_extra2_curr_d3d_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_d3d_3.Anchor")));
      this.combo_extra2_curr_d3d_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_d3d_3.BackgroundImage")));
      this.combo_extra2_curr_d3d_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_d3d_3.Dock")));
      this.combo_extra2_curr_d3d_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_d3d_3.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_d3d_3.Enabled")));
      this.combo_extra2_curr_d3d_3.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_d3d_3.Font")));
      this.combo_extra2_curr_d3d_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_d3d_3.ImeMode")));
      this.combo_extra2_curr_d3d_3.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_d3d_3.IntegralHeight")));
      this.combo_extra2_curr_d3d_3.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_d3d_3.ItemHeight")));
      this.combo_extra2_curr_d3d_3.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_d3d_3.Location")));
      this.combo_extra2_curr_d3d_3.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_d3d_3.MaxDropDownItems")));
      this.combo_extra2_curr_d3d_3.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_d3d_3.MaxLength")));
      this.combo_extra2_curr_d3d_3.Name = "combo_extra2_curr_d3d_3";
      this.combo_extra2_curr_d3d_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_d3d_3.RightToLeft")));
      this.combo_extra2_curr_d3d_3.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_d3d_3.Size")));
      this.combo_extra2_curr_d3d_3.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_d3d_3.TabIndex")));
      this.combo_extra2_curr_d3d_3.Text = resources.GetString("combo_extra2_curr_d3d_3.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_d3d_3, resources.GetString("combo_extra2_curr_d3d_3.ToolTip"));
      this.combo_extra2_curr_d3d_3.Visible = ((bool)(resources.GetObject("combo_extra2_curr_d3d_3.Visible")));
      // 
      // combo_extra2_curr_d3d_4
      // 
      this.combo_extra2_curr_d3d_4.AccessibleDescription = resources.GetString("combo_extra2_curr_d3d_4.AccessibleDescription");
      this.combo_extra2_curr_d3d_4.AccessibleName = resources.GetString("combo_extra2_curr_d3d_4.AccessibleName");
      this.combo_extra2_curr_d3d_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_d3d_4.Anchor")));
      this.combo_extra2_curr_d3d_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_d3d_4.BackgroundImage")));
      this.combo_extra2_curr_d3d_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_d3d_4.Dock")));
      this.combo_extra2_curr_d3d_4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_d3d_4.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_d3d_4.Enabled")));
      this.combo_extra2_curr_d3d_4.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_d3d_4.Font")));
      this.combo_extra2_curr_d3d_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_d3d_4.ImeMode")));
      this.combo_extra2_curr_d3d_4.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_d3d_4.IntegralHeight")));
      this.combo_extra2_curr_d3d_4.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_d3d_4.ItemHeight")));
      this.combo_extra2_curr_d3d_4.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_d3d_4.Location")));
      this.combo_extra2_curr_d3d_4.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_d3d_4.MaxDropDownItems")));
      this.combo_extra2_curr_d3d_4.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_d3d_4.MaxLength")));
      this.combo_extra2_curr_d3d_4.Name = "combo_extra2_curr_d3d_4";
      this.combo_extra2_curr_d3d_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_d3d_4.RightToLeft")));
      this.combo_extra2_curr_d3d_4.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_d3d_4.Size")));
      this.combo_extra2_curr_d3d_4.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_d3d_4.TabIndex")));
      this.combo_extra2_curr_d3d_4.Text = resources.GetString("combo_extra2_curr_d3d_4.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_d3d_4, resources.GetString("combo_extra2_curr_d3d_4.ToolTip"));
      this.combo_extra2_curr_d3d_4.Visible = ((bool)(resources.GetObject("combo_extra2_curr_d3d_4.Visible")));
      // 
      // combo_extra2_curr_d3d_5
      // 
      this.combo_extra2_curr_d3d_5.AccessibleDescription = resources.GetString("combo_extra2_curr_d3d_5.AccessibleDescription");
      this.combo_extra2_curr_d3d_5.AccessibleName = resources.GetString("combo_extra2_curr_d3d_5.AccessibleName");
      this.combo_extra2_curr_d3d_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_d3d_5.Anchor")));
      this.combo_extra2_curr_d3d_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_d3d_5.BackgroundImage")));
      this.combo_extra2_curr_d3d_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_d3d_5.Dock")));
      this.combo_extra2_curr_d3d_5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_d3d_5.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_d3d_5.Enabled")));
      this.combo_extra2_curr_d3d_5.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_d3d_5.Font")));
      this.combo_extra2_curr_d3d_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_d3d_5.ImeMode")));
      this.combo_extra2_curr_d3d_5.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_d3d_5.IntegralHeight")));
      this.combo_extra2_curr_d3d_5.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_d3d_5.ItemHeight")));
      this.combo_extra2_curr_d3d_5.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_d3d_5.Location")));
      this.combo_extra2_curr_d3d_5.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_d3d_5.MaxDropDownItems")));
      this.combo_extra2_curr_d3d_5.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_d3d_5.MaxLength")));
      this.combo_extra2_curr_d3d_5.Name = "combo_extra2_curr_d3d_5";
      this.combo_extra2_curr_d3d_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_d3d_5.RightToLeft")));
      this.combo_extra2_curr_d3d_5.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_d3d_5.Size")));
      this.combo_extra2_curr_d3d_5.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_d3d_5.TabIndex")));
      this.combo_extra2_curr_d3d_5.Text = resources.GetString("combo_extra2_curr_d3d_5.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_d3d_5, resources.GetString("combo_extra2_curr_d3d_5.ToolTip"));
      this.combo_extra2_curr_d3d_5.Visible = ((bool)(resources.GetObject("combo_extra2_curr_d3d_5.Visible")));
      // 
      // combo_extra2_curr_ogl_1
      // 
      this.combo_extra2_curr_ogl_1.AccessibleDescription = resources.GetString("combo_extra2_curr_ogl_1.AccessibleDescription");
      this.combo_extra2_curr_ogl_1.AccessibleName = resources.GetString("combo_extra2_curr_ogl_1.AccessibleName");
      this.combo_extra2_curr_ogl_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_ogl_1.Anchor")));
      this.combo_extra2_curr_ogl_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_ogl_1.BackgroundImage")));
      this.combo_extra2_curr_ogl_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_ogl_1.Dock")));
      this.combo_extra2_curr_ogl_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_ogl_1.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_ogl_1.Enabled")));
      this.combo_extra2_curr_ogl_1.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_ogl_1.Font")));
      this.combo_extra2_curr_ogl_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_ogl_1.ImeMode")));
      this.combo_extra2_curr_ogl_1.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_ogl_1.IntegralHeight")));
      this.combo_extra2_curr_ogl_1.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_ogl_1.ItemHeight")));
      this.combo_extra2_curr_ogl_1.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_ogl_1.Location")));
      this.combo_extra2_curr_ogl_1.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_ogl_1.MaxDropDownItems")));
      this.combo_extra2_curr_ogl_1.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_ogl_1.MaxLength")));
      this.combo_extra2_curr_ogl_1.Name = "combo_extra2_curr_ogl_1";
      this.combo_extra2_curr_ogl_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_ogl_1.RightToLeft")));
      this.combo_extra2_curr_ogl_1.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_ogl_1.Size")));
      this.combo_extra2_curr_ogl_1.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_ogl_1.TabIndex")));
      this.combo_extra2_curr_ogl_1.Text = resources.GetString("combo_extra2_curr_ogl_1.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_ogl_1, resources.GetString("combo_extra2_curr_ogl_1.ToolTip"));
      this.combo_extra2_curr_ogl_1.Visible = ((bool)(resources.GetObject("combo_extra2_curr_ogl_1.Visible")));
      // 
      // combo_extra2_curr_ogl_2
      // 
      this.combo_extra2_curr_ogl_2.AccessibleDescription = resources.GetString("combo_extra2_curr_ogl_2.AccessibleDescription");
      this.combo_extra2_curr_ogl_2.AccessibleName = resources.GetString("combo_extra2_curr_ogl_2.AccessibleName");
      this.combo_extra2_curr_ogl_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_ogl_2.Anchor")));
      this.combo_extra2_curr_ogl_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_ogl_2.BackgroundImage")));
      this.combo_extra2_curr_ogl_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_ogl_2.Dock")));
      this.combo_extra2_curr_ogl_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_ogl_2.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_ogl_2.Enabled")));
      this.combo_extra2_curr_ogl_2.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_ogl_2.Font")));
      this.combo_extra2_curr_ogl_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_ogl_2.ImeMode")));
      this.combo_extra2_curr_ogl_2.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_ogl_2.IntegralHeight")));
      this.combo_extra2_curr_ogl_2.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_ogl_2.ItemHeight")));
      this.combo_extra2_curr_ogl_2.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_ogl_2.Location")));
      this.combo_extra2_curr_ogl_2.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_ogl_2.MaxDropDownItems")));
      this.combo_extra2_curr_ogl_2.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_ogl_2.MaxLength")));
      this.combo_extra2_curr_ogl_2.Name = "combo_extra2_curr_ogl_2";
      this.combo_extra2_curr_ogl_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_ogl_2.RightToLeft")));
      this.combo_extra2_curr_ogl_2.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_ogl_2.Size")));
      this.combo_extra2_curr_ogl_2.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_ogl_2.TabIndex")));
      this.combo_extra2_curr_ogl_2.Text = resources.GetString("combo_extra2_curr_ogl_2.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_ogl_2, resources.GetString("combo_extra2_curr_ogl_2.ToolTip"));
      this.combo_extra2_curr_ogl_2.Visible = ((bool)(resources.GetObject("combo_extra2_curr_ogl_2.Visible")));
      // 
      // combo_extra2_curr_ogl_3
      // 
      this.combo_extra2_curr_ogl_3.AccessibleDescription = resources.GetString("combo_extra2_curr_ogl_3.AccessibleDescription");
      this.combo_extra2_curr_ogl_3.AccessibleName = resources.GetString("combo_extra2_curr_ogl_3.AccessibleName");
      this.combo_extra2_curr_ogl_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_ogl_3.Anchor")));
      this.combo_extra2_curr_ogl_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_ogl_3.BackgroundImage")));
      this.combo_extra2_curr_ogl_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_ogl_3.Dock")));
      this.combo_extra2_curr_ogl_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_ogl_3.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_ogl_3.Enabled")));
      this.combo_extra2_curr_ogl_3.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_ogl_3.Font")));
      this.combo_extra2_curr_ogl_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_ogl_3.ImeMode")));
      this.combo_extra2_curr_ogl_3.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_ogl_3.IntegralHeight")));
      this.combo_extra2_curr_ogl_3.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_ogl_3.ItemHeight")));
      this.combo_extra2_curr_ogl_3.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_ogl_3.Location")));
      this.combo_extra2_curr_ogl_3.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_ogl_3.MaxDropDownItems")));
      this.combo_extra2_curr_ogl_3.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_ogl_3.MaxLength")));
      this.combo_extra2_curr_ogl_3.Name = "combo_extra2_curr_ogl_3";
      this.combo_extra2_curr_ogl_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_ogl_3.RightToLeft")));
      this.combo_extra2_curr_ogl_3.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_ogl_3.Size")));
      this.combo_extra2_curr_ogl_3.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_ogl_3.TabIndex")));
      this.combo_extra2_curr_ogl_3.Text = resources.GetString("combo_extra2_curr_ogl_3.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_ogl_3, resources.GetString("combo_extra2_curr_ogl_3.ToolTip"));
      this.combo_extra2_curr_ogl_3.Visible = ((bool)(resources.GetObject("combo_extra2_curr_ogl_3.Visible")));
      // 
      // combo_extra2_curr_ogl_4
      // 
      this.combo_extra2_curr_ogl_4.AccessibleDescription = resources.GetString("combo_extra2_curr_ogl_4.AccessibleDescription");
      this.combo_extra2_curr_ogl_4.AccessibleName = resources.GetString("combo_extra2_curr_ogl_4.AccessibleName");
      this.combo_extra2_curr_ogl_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_ogl_4.Anchor")));
      this.combo_extra2_curr_ogl_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_ogl_4.BackgroundImage")));
      this.combo_extra2_curr_ogl_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_ogl_4.Dock")));
      this.combo_extra2_curr_ogl_4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_ogl_4.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_ogl_4.Enabled")));
      this.combo_extra2_curr_ogl_4.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_ogl_4.Font")));
      this.combo_extra2_curr_ogl_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_ogl_4.ImeMode")));
      this.combo_extra2_curr_ogl_4.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_ogl_4.IntegralHeight")));
      this.combo_extra2_curr_ogl_4.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_ogl_4.ItemHeight")));
      this.combo_extra2_curr_ogl_4.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_ogl_4.Location")));
      this.combo_extra2_curr_ogl_4.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_ogl_4.MaxDropDownItems")));
      this.combo_extra2_curr_ogl_4.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_ogl_4.MaxLength")));
      this.combo_extra2_curr_ogl_4.Name = "combo_extra2_curr_ogl_4";
      this.combo_extra2_curr_ogl_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_ogl_4.RightToLeft")));
      this.combo_extra2_curr_ogl_4.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_ogl_4.Size")));
      this.combo_extra2_curr_ogl_4.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_ogl_4.TabIndex")));
      this.combo_extra2_curr_ogl_4.Text = resources.GetString("combo_extra2_curr_ogl_4.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_ogl_4, resources.GetString("combo_extra2_curr_ogl_4.ToolTip"));
      this.combo_extra2_curr_ogl_4.Visible = ((bool)(resources.GetObject("combo_extra2_curr_ogl_4.Visible")));
      // 
      // combo_extra2_curr_ogl_5
      // 
      this.combo_extra2_curr_ogl_5.AccessibleDescription = resources.GetString("combo_extra2_curr_ogl_5.AccessibleDescription");
      this.combo_extra2_curr_ogl_5.AccessibleName = resources.GetString("combo_extra2_curr_ogl_5.AccessibleName");
      this.combo_extra2_curr_ogl_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_ogl_5.Anchor")));
      this.combo_extra2_curr_ogl_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_ogl_5.BackgroundImage")));
      this.combo_extra2_curr_ogl_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_ogl_5.Dock")));
      this.combo_extra2_curr_ogl_5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_ogl_5.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_ogl_5.Enabled")));
      this.combo_extra2_curr_ogl_5.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_ogl_5.Font")));
      this.combo_extra2_curr_ogl_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_ogl_5.ImeMode")));
      this.combo_extra2_curr_ogl_5.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_ogl_5.IntegralHeight")));
      this.combo_extra2_curr_ogl_5.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_ogl_5.ItemHeight")));
      this.combo_extra2_curr_ogl_5.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_ogl_5.Location")));
      this.combo_extra2_curr_ogl_5.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_ogl_5.MaxDropDownItems")));
      this.combo_extra2_curr_ogl_5.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_ogl_5.MaxLength")));
      this.combo_extra2_curr_ogl_5.Name = "combo_extra2_curr_ogl_5";
      this.combo_extra2_curr_ogl_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_ogl_5.RightToLeft")));
      this.combo_extra2_curr_ogl_5.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_ogl_5.Size")));
      this.combo_extra2_curr_ogl_5.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_ogl_5.TabIndex")));
      this.combo_extra2_curr_ogl_5.Text = resources.GetString("combo_extra2_curr_ogl_5.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_ogl_5, resources.GetString("combo_extra2_curr_ogl_5.ToolTip"));
      this.combo_extra2_curr_ogl_5.Visible = ((bool)(resources.GetObject("combo_extra2_curr_ogl_5.Visible")));
      // 
      // combo_extra2_curr_ogl_6
      // 
      this.combo_extra2_curr_ogl_6.AccessibleDescription = resources.GetString("combo_extra2_curr_ogl_6.AccessibleDescription");
      this.combo_extra2_curr_ogl_6.AccessibleName = resources.GetString("combo_extra2_curr_ogl_6.AccessibleName");
      this.combo_extra2_curr_ogl_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_ogl_6.Anchor")));
      this.combo_extra2_curr_ogl_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_ogl_6.BackgroundImage")));
      this.combo_extra2_curr_ogl_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_ogl_6.Dock")));
      this.combo_extra2_curr_ogl_6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_ogl_6.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_ogl_6.Enabled")));
      this.combo_extra2_curr_ogl_6.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_ogl_6.Font")));
      this.combo_extra2_curr_ogl_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_ogl_6.ImeMode")));
      this.combo_extra2_curr_ogl_6.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_ogl_6.IntegralHeight")));
      this.combo_extra2_curr_ogl_6.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_ogl_6.ItemHeight")));
      this.combo_extra2_curr_ogl_6.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_ogl_6.Location")));
      this.combo_extra2_curr_ogl_6.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_ogl_6.MaxDropDownItems")));
      this.combo_extra2_curr_ogl_6.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_ogl_6.MaxLength")));
      this.combo_extra2_curr_ogl_6.Name = "combo_extra2_curr_ogl_6";
      this.combo_extra2_curr_ogl_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_ogl_6.RightToLeft")));
      this.combo_extra2_curr_ogl_6.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_ogl_6.Size")));
      this.combo_extra2_curr_ogl_6.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_ogl_6.TabIndex")));
      this.combo_extra2_curr_ogl_6.Text = resources.GetString("combo_extra2_curr_ogl_6.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_ogl_6, resources.GetString("combo_extra2_curr_ogl_6.ToolTip"));
      this.combo_extra2_curr_ogl_6.Visible = ((bool)(resources.GetObject("combo_extra2_curr_ogl_6.Visible")));
      // 
      // combo_extra2_prof_d3d_1
      // 
      this.combo_extra2_prof_d3d_1.AccessibleDescription = resources.GetString("combo_extra2_prof_d3d_1.AccessibleDescription");
      this.combo_extra2_prof_d3d_1.AccessibleName = resources.GetString("combo_extra2_prof_d3d_1.AccessibleName");
      this.combo_extra2_prof_d3d_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_d3d_1.Anchor")));
      this.combo_extra2_prof_d3d_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_d3d_1.BackgroundImage")));
      this.combo_extra2_prof_d3d_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_d3d_1.Dock")));
      this.combo_extra2_prof_d3d_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_d3d_1.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_d3d_1.Enabled")));
      this.combo_extra2_prof_d3d_1.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_d3d_1.Font")));
      this.combo_extra2_prof_d3d_1.ForeColor = System.Drawing.SystemColors.WindowText;
      this.combo_extra2_prof_d3d_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_d3d_1.ImeMode")));
      this.combo_extra2_prof_d3d_1.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_d3d_1.IntegralHeight")));
      this.combo_extra2_prof_d3d_1.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_d3d_1.ItemHeight")));
      this.combo_extra2_prof_d3d_1.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_d3d_1.Location")));
      this.combo_extra2_prof_d3d_1.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_d3d_1.MaxDropDownItems")));
      this.combo_extra2_prof_d3d_1.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_d3d_1.MaxLength")));
      this.combo_extra2_prof_d3d_1.Name = "combo_extra2_prof_d3d_1";
      this.combo_extra2_prof_d3d_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_d3d_1.RightToLeft")));
      this.combo_extra2_prof_d3d_1.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_d3d_1.Size")));
      this.combo_extra2_prof_d3d_1.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_d3d_1.TabIndex")));
      this.combo_extra2_prof_d3d_1.Text = resources.GetString("combo_extra2_prof_d3d_1.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_d3d_1, resources.GetString("combo_extra2_prof_d3d_1.ToolTip"));
      this.combo_extra2_prof_d3d_1.Visible = ((bool)(resources.GetObject("combo_extra2_prof_d3d_1.Visible")));
      // 
      // combo_extra2_prof_d3d_2
      // 
      this.combo_extra2_prof_d3d_2.AccessibleDescription = resources.GetString("combo_extra2_prof_d3d_2.AccessibleDescription");
      this.combo_extra2_prof_d3d_2.AccessibleName = resources.GetString("combo_extra2_prof_d3d_2.AccessibleName");
      this.combo_extra2_prof_d3d_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_d3d_2.Anchor")));
      this.combo_extra2_prof_d3d_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_d3d_2.BackgroundImage")));
      this.combo_extra2_prof_d3d_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_d3d_2.Dock")));
      this.combo_extra2_prof_d3d_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_d3d_2.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_d3d_2.Enabled")));
      this.combo_extra2_prof_d3d_2.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_d3d_2.Font")));
      this.combo_extra2_prof_d3d_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_d3d_2.ImeMode")));
      this.combo_extra2_prof_d3d_2.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_d3d_2.IntegralHeight")));
      this.combo_extra2_prof_d3d_2.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_d3d_2.ItemHeight")));
      this.combo_extra2_prof_d3d_2.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_d3d_2.Location")));
      this.combo_extra2_prof_d3d_2.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_d3d_2.MaxDropDownItems")));
      this.combo_extra2_prof_d3d_2.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_d3d_2.MaxLength")));
      this.combo_extra2_prof_d3d_2.Name = "combo_extra2_prof_d3d_2";
      this.combo_extra2_prof_d3d_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_d3d_2.RightToLeft")));
      this.combo_extra2_prof_d3d_2.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_d3d_2.Size")));
      this.combo_extra2_prof_d3d_2.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_d3d_2.TabIndex")));
      this.combo_extra2_prof_d3d_2.Text = resources.GetString("combo_extra2_prof_d3d_2.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_d3d_2, resources.GetString("combo_extra2_prof_d3d_2.ToolTip"));
      this.combo_extra2_prof_d3d_2.Visible = ((bool)(resources.GetObject("combo_extra2_prof_d3d_2.Visible")));
      // 
      // combo_extra2_prof_d3d_3
      // 
      this.combo_extra2_prof_d3d_3.AccessibleDescription = resources.GetString("combo_extra2_prof_d3d_3.AccessibleDescription");
      this.combo_extra2_prof_d3d_3.AccessibleName = resources.GetString("combo_extra2_prof_d3d_3.AccessibleName");
      this.combo_extra2_prof_d3d_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_d3d_3.Anchor")));
      this.combo_extra2_prof_d3d_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_d3d_3.BackgroundImage")));
      this.combo_extra2_prof_d3d_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_d3d_3.Dock")));
      this.combo_extra2_prof_d3d_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_d3d_3.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_d3d_3.Enabled")));
      this.combo_extra2_prof_d3d_3.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_d3d_3.Font")));
      this.combo_extra2_prof_d3d_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_d3d_3.ImeMode")));
      this.combo_extra2_prof_d3d_3.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_d3d_3.IntegralHeight")));
      this.combo_extra2_prof_d3d_3.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_d3d_3.ItemHeight")));
      this.combo_extra2_prof_d3d_3.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_d3d_3.Location")));
      this.combo_extra2_prof_d3d_3.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_d3d_3.MaxDropDownItems")));
      this.combo_extra2_prof_d3d_3.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_d3d_3.MaxLength")));
      this.combo_extra2_prof_d3d_3.Name = "combo_extra2_prof_d3d_3";
      this.combo_extra2_prof_d3d_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_d3d_3.RightToLeft")));
      this.combo_extra2_prof_d3d_3.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_d3d_3.Size")));
      this.combo_extra2_prof_d3d_3.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_d3d_3.TabIndex")));
      this.combo_extra2_prof_d3d_3.Text = resources.GetString("combo_extra2_prof_d3d_3.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_d3d_3, resources.GetString("combo_extra2_prof_d3d_3.ToolTip"));
      this.combo_extra2_prof_d3d_3.Visible = ((bool)(resources.GetObject("combo_extra2_prof_d3d_3.Visible")));
      // 
      // combo_extra2_prof_d3d_4
      // 
      this.combo_extra2_prof_d3d_4.AccessibleDescription = resources.GetString("combo_extra2_prof_d3d_4.AccessibleDescription");
      this.combo_extra2_prof_d3d_4.AccessibleName = resources.GetString("combo_extra2_prof_d3d_4.AccessibleName");
      this.combo_extra2_prof_d3d_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_d3d_4.Anchor")));
      this.combo_extra2_prof_d3d_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_d3d_4.BackgroundImage")));
      this.combo_extra2_prof_d3d_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_d3d_4.Dock")));
      this.combo_extra2_prof_d3d_4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_d3d_4.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_d3d_4.Enabled")));
      this.combo_extra2_prof_d3d_4.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_d3d_4.Font")));
      this.combo_extra2_prof_d3d_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_d3d_4.ImeMode")));
      this.combo_extra2_prof_d3d_4.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_d3d_4.IntegralHeight")));
      this.combo_extra2_prof_d3d_4.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_d3d_4.ItemHeight")));
      this.combo_extra2_prof_d3d_4.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_d3d_4.Location")));
      this.combo_extra2_prof_d3d_4.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_d3d_4.MaxDropDownItems")));
      this.combo_extra2_prof_d3d_4.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_d3d_4.MaxLength")));
      this.combo_extra2_prof_d3d_4.Name = "combo_extra2_prof_d3d_4";
      this.combo_extra2_prof_d3d_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_d3d_4.RightToLeft")));
      this.combo_extra2_prof_d3d_4.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_d3d_4.Size")));
      this.combo_extra2_prof_d3d_4.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_d3d_4.TabIndex")));
      this.combo_extra2_prof_d3d_4.Text = resources.GetString("combo_extra2_prof_d3d_4.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_d3d_4, resources.GetString("combo_extra2_prof_d3d_4.ToolTip"));
      this.combo_extra2_prof_d3d_4.Visible = ((bool)(resources.GetObject("combo_extra2_prof_d3d_4.Visible")));
      // 
      // combo_extra2_prof_d3d_5
      // 
      this.combo_extra2_prof_d3d_5.AccessibleDescription = resources.GetString("combo_extra2_prof_d3d_5.AccessibleDescription");
      this.combo_extra2_prof_d3d_5.AccessibleName = resources.GetString("combo_extra2_prof_d3d_5.AccessibleName");
      this.combo_extra2_prof_d3d_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_d3d_5.Anchor")));
      this.combo_extra2_prof_d3d_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_d3d_5.BackgroundImage")));
      this.combo_extra2_prof_d3d_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_d3d_5.Dock")));
      this.combo_extra2_prof_d3d_5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_d3d_5.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_d3d_5.Enabled")));
      this.combo_extra2_prof_d3d_5.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_d3d_5.Font")));
      this.combo_extra2_prof_d3d_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_d3d_5.ImeMode")));
      this.combo_extra2_prof_d3d_5.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_d3d_5.IntegralHeight")));
      this.combo_extra2_prof_d3d_5.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_d3d_5.ItemHeight")));
      this.combo_extra2_prof_d3d_5.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_d3d_5.Location")));
      this.combo_extra2_prof_d3d_5.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_d3d_5.MaxDropDownItems")));
      this.combo_extra2_prof_d3d_5.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_d3d_5.MaxLength")));
      this.combo_extra2_prof_d3d_5.Name = "combo_extra2_prof_d3d_5";
      this.combo_extra2_prof_d3d_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_d3d_5.RightToLeft")));
      this.combo_extra2_prof_d3d_5.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_d3d_5.Size")));
      this.combo_extra2_prof_d3d_5.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_d3d_5.TabIndex")));
      this.combo_extra2_prof_d3d_5.Text = resources.GetString("combo_extra2_prof_d3d_5.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_d3d_5, resources.GetString("combo_extra2_prof_d3d_5.ToolTip"));
      this.combo_extra2_prof_d3d_5.Visible = ((bool)(resources.GetObject("combo_extra2_prof_d3d_5.Visible")));
      // 
      // combo_extra2_prof_d3d_6
      // 
      this.combo_extra2_prof_d3d_6.AccessibleDescription = resources.GetString("combo_extra2_prof_d3d_6.AccessibleDescription");
      this.combo_extra2_prof_d3d_6.AccessibleName = resources.GetString("combo_extra2_prof_d3d_6.AccessibleName");
      this.combo_extra2_prof_d3d_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_d3d_6.Anchor")));
      this.combo_extra2_prof_d3d_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_d3d_6.BackgroundImage")));
      this.combo_extra2_prof_d3d_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_d3d_6.Dock")));
      this.combo_extra2_prof_d3d_6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_d3d_6.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_d3d_6.Enabled")));
      this.combo_extra2_prof_d3d_6.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_d3d_6.Font")));
      this.combo_extra2_prof_d3d_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_d3d_6.ImeMode")));
      this.combo_extra2_prof_d3d_6.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_d3d_6.IntegralHeight")));
      this.combo_extra2_prof_d3d_6.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_d3d_6.ItemHeight")));
      this.combo_extra2_prof_d3d_6.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_d3d_6.Location")));
      this.combo_extra2_prof_d3d_6.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_d3d_6.MaxDropDownItems")));
      this.combo_extra2_prof_d3d_6.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_d3d_6.MaxLength")));
      this.combo_extra2_prof_d3d_6.Name = "combo_extra2_prof_d3d_6";
      this.combo_extra2_prof_d3d_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_d3d_6.RightToLeft")));
      this.combo_extra2_prof_d3d_6.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_d3d_6.Size")));
      this.combo_extra2_prof_d3d_6.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_d3d_6.TabIndex")));
      this.combo_extra2_prof_d3d_6.Text = resources.GetString("combo_extra2_prof_d3d_6.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_d3d_6, resources.GetString("combo_extra2_prof_d3d_6.ToolTip"));
      this.combo_extra2_prof_d3d_6.Visible = ((bool)(resources.GetObject("combo_extra2_prof_d3d_6.Visible")));
      // 
      // combo_extra2_prof_ogl_1
      // 
      this.combo_extra2_prof_ogl_1.AccessibleDescription = resources.GetString("combo_extra2_prof_ogl_1.AccessibleDescription");
      this.combo_extra2_prof_ogl_1.AccessibleName = resources.GetString("combo_extra2_prof_ogl_1.AccessibleName");
      this.combo_extra2_prof_ogl_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_ogl_1.Anchor")));
      this.combo_extra2_prof_ogl_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_ogl_1.BackgroundImage")));
      this.combo_extra2_prof_ogl_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_ogl_1.Dock")));
      this.combo_extra2_prof_ogl_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_ogl_1.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_ogl_1.Enabled")));
      this.combo_extra2_prof_ogl_1.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_ogl_1.Font")));
      this.combo_extra2_prof_ogl_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_ogl_1.ImeMode")));
      this.combo_extra2_prof_ogl_1.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_ogl_1.IntegralHeight")));
      this.combo_extra2_prof_ogl_1.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_ogl_1.ItemHeight")));
      this.combo_extra2_prof_ogl_1.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_ogl_1.Location")));
      this.combo_extra2_prof_ogl_1.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_ogl_1.MaxDropDownItems")));
      this.combo_extra2_prof_ogl_1.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_ogl_1.MaxLength")));
      this.combo_extra2_prof_ogl_1.Name = "combo_extra2_prof_ogl_1";
      this.combo_extra2_prof_ogl_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_ogl_1.RightToLeft")));
      this.combo_extra2_prof_ogl_1.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_ogl_1.Size")));
      this.combo_extra2_prof_ogl_1.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_ogl_1.TabIndex")));
      this.combo_extra2_prof_ogl_1.Text = resources.GetString("combo_extra2_prof_ogl_1.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_ogl_1, resources.GetString("combo_extra2_prof_ogl_1.ToolTip"));
      this.combo_extra2_prof_ogl_1.Visible = ((bool)(resources.GetObject("combo_extra2_prof_ogl_1.Visible")));
      // 
      // combo_extra2_prof_ogl_2
      // 
      this.combo_extra2_prof_ogl_2.AccessibleDescription = resources.GetString("combo_extra2_prof_ogl_2.AccessibleDescription");
      this.combo_extra2_prof_ogl_2.AccessibleName = resources.GetString("combo_extra2_prof_ogl_2.AccessibleName");
      this.combo_extra2_prof_ogl_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_ogl_2.Anchor")));
      this.combo_extra2_prof_ogl_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_ogl_2.BackgroundImage")));
      this.combo_extra2_prof_ogl_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_ogl_2.Dock")));
      this.combo_extra2_prof_ogl_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_ogl_2.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_ogl_2.Enabled")));
      this.combo_extra2_prof_ogl_2.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_ogl_2.Font")));
      this.combo_extra2_prof_ogl_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_ogl_2.ImeMode")));
      this.combo_extra2_prof_ogl_2.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_ogl_2.IntegralHeight")));
      this.combo_extra2_prof_ogl_2.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_ogl_2.ItemHeight")));
      this.combo_extra2_prof_ogl_2.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_ogl_2.Location")));
      this.combo_extra2_prof_ogl_2.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_ogl_2.MaxDropDownItems")));
      this.combo_extra2_prof_ogl_2.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_ogl_2.MaxLength")));
      this.combo_extra2_prof_ogl_2.Name = "combo_extra2_prof_ogl_2";
      this.combo_extra2_prof_ogl_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_ogl_2.RightToLeft")));
      this.combo_extra2_prof_ogl_2.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_ogl_2.Size")));
      this.combo_extra2_prof_ogl_2.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_ogl_2.TabIndex")));
      this.combo_extra2_prof_ogl_2.Text = resources.GetString("combo_extra2_prof_ogl_2.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_ogl_2, resources.GetString("combo_extra2_prof_ogl_2.ToolTip"));
      this.combo_extra2_prof_ogl_2.Visible = ((bool)(resources.GetObject("combo_extra2_prof_ogl_2.Visible")));
      // 
      // combo_extra2_prof_ogl_3
      // 
      this.combo_extra2_prof_ogl_3.AccessibleDescription = resources.GetString("combo_extra2_prof_ogl_3.AccessibleDescription");
      this.combo_extra2_prof_ogl_3.AccessibleName = resources.GetString("combo_extra2_prof_ogl_3.AccessibleName");
      this.combo_extra2_prof_ogl_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_ogl_3.Anchor")));
      this.combo_extra2_prof_ogl_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_ogl_3.BackgroundImage")));
      this.combo_extra2_prof_ogl_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_ogl_3.Dock")));
      this.combo_extra2_prof_ogl_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_ogl_3.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_ogl_3.Enabled")));
      this.combo_extra2_prof_ogl_3.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_ogl_3.Font")));
      this.combo_extra2_prof_ogl_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_ogl_3.ImeMode")));
      this.combo_extra2_prof_ogl_3.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_ogl_3.IntegralHeight")));
      this.combo_extra2_prof_ogl_3.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_ogl_3.ItemHeight")));
      this.combo_extra2_prof_ogl_3.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_ogl_3.Location")));
      this.combo_extra2_prof_ogl_3.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_ogl_3.MaxDropDownItems")));
      this.combo_extra2_prof_ogl_3.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_ogl_3.MaxLength")));
      this.combo_extra2_prof_ogl_3.Name = "combo_extra2_prof_ogl_3";
      this.combo_extra2_prof_ogl_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_ogl_3.RightToLeft")));
      this.combo_extra2_prof_ogl_3.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_ogl_3.Size")));
      this.combo_extra2_prof_ogl_3.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_ogl_3.TabIndex")));
      this.combo_extra2_prof_ogl_3.Text = resources.GetString("combo_extra2_prof_ogl_3.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_ogl_3, resources.GetString("combo_extra2_prof_ogl_3.ToolTip"));
      this.combo_extra2_prof_ogl_3.Visible = ((bool)(resources.GetObject("combo_extra2_prof_ogl_3.Visible")));
      // 
      // combo_extra2_prof_ogl_4
      // 
      this.combo_extra2_prof_ogl_4.AccessibleDescription = resources.GetString("combo_extra2_prof_ogl_4.AccessibleDescription");
      this.combo_extra2_prof_ogl_4.AccessibleName = resources.GetString("combo_extra2_prof_ogl_4.AccessibleName");
      this.combo_extra2_prof_ogl_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_ogl_4.Anchor")));
      this.combo_extra2_prof_ogl_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_ogl_4.BackgroundImage")));
      this.combo_extra2_prof_ogl_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_ogl_4.Dock")));
      this.combo_extra2_prof_ogl_4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_ogl_4.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_ogl_4.Enabled")));
      this.combo_extra2_prof_ogl_4.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_ogl_4.Font")));
      this.combo_extra2_prof_ogl_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_ogl_4.ImeMode")));
      this.combo_extra2_prof_ogl_4.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_ogl_4.IntegralHeight")));
      this.combo_extra2_prof_ogl_4.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_ogl_4.ItemHeight")));
      this.combo_extra2_prof_ogl_4.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_ogl_4.Location")));
      this.combo_extra2_prof_ogl_4.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_ogl_4.MaxDropDownItems")));
      this.combo_extra2_prof_ogl_4.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_ogl_4.MaxLength")));
      this.combo_extra2_prof_ogl_4.Name = "combo_extra2_prof_ogl_4";
      this.combo_extra2_prof_ogl_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_ogl_4.RightToLeft")));
      this.combo_extra2_prof_ogl_4.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_ogl_4.Size")));
      this.combo_extra2_prof_ogl_4.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_ogl_4.TabIndex")));
      this.combo_extra2_prof_ogl_4.Text = resources.GetString("combo_extra2_prof_ogl_4.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_ogl_4, resources.GetString("combo_extra2_prof_ogl_4.ToolTip"));
      this.combo_extra2_prof_ogl_4.Visible = ((bool)(resources.GetObject("combo_extra2_prof_ogl_4.Visible")));
      // 
      // combo_extra2_prof_ogl_5
      // 
      this.combo_extra2_prof_ogl_5.AccessibleDescription = resources.GetString("combo_extra2_prof_ogl_5.AccessibleDescription");
      this.combo_extra2_prof_ogl_5.AccessibleName = resources.GetString("combo_extra2_prof_ogl_5.AccessibleName");
      this.combo_extra2_prof_ogl_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_ogl_5.Anchor")));
      this.combo_extra2_prof_ogl_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_ogl_5.BackgroundImage")));
      this.combo_extra2_prof_ogl_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_ogl_5.Dock")));
      this.combo_extra2_prof_ogl_5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_ogl_5.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_ogl_5.Enabled")));
      this.combo_extra2_prof_ogl_5.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_ogl_5.Font")));
      this.combo_extra2_prof_ogl_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_ogl_5.ImeMode")));
      this.combo_extra2_prof_ogl_5.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_ogl_5.IntegralHeight")));
      this.combo_extra2_prof_ogl_5.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_ogl_5.ItemHeight")));
      this.combo_extra2_prof_ogl_5.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_ogl_5.Location")));
      this.combo_extra2_prof_ogl_5.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_ogl_5.MaxDropDownItems")));
      this.combo_extra2_prof_ogl_5.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_ogl_5.MaxLength")));
      this.combo_extra2_prof_ogl_5.Name = "combo_extra2_prof_ogl_5";
      this.combo_extra2_prof_ogl_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_ogl_5.RightToLeft")));
      this.combo_extra2_prof_ogl_5.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_ogl_5.Size")));
      this.combo_extra2_prof_ogl_5.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_ogl_5.TabIndex")));
      this.combo_extra2_prof_ogl_5.Text = resources.GetString("combo_extra2_prof_ogl_5.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_ogl_5, resources.GetString("combo_extra2_prof_ogl_5.ToolTip"));
      this.combo_extra2_prof_ogl_5.Visible = ((bool)(resources.GetObject("combo_extra2_prof_ogl_5.Visible")));
      // 
      // combo_extra2_prof_ogl_6
      // 
      this.combo_extra2_prof_ogl_6.AccessibleDescription = resources.GetString("combo_extra2_prof_ogl_6.AccessibleDescription");
      this.combo_extra2_prof_ogl_6.AccessibleName = resources.GetString("combo_extra2_prof_ogl_6.AccessibleName");
      this.combo_extra2_prof_ogl_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_ogl_6.Anchor")));
      this.combo_extra2_prof_ogl_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_ogl_6.BackgroundImage")));
      this.combo_extra2_prof_ogl_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_ogl_6.Dock")));
      this.combo_extra2_prof_ogl_6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_ogl_6.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_ogl_6.Enabled")));
      this.combo_extra2_prof_ogl_6.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_ogl_6.Font")));
      this.combo_extra2_prof_ogl_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_ogl_6.ImeMode")));
      this.combo_extra2_prof_ogl_6.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_ogl_6.IntegralHeight")));
      this.combo_extra2_prof_ogl_6.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_ogl_6.ItemHeight")));
      this.combo_extra2_prof_ogl_6.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_ogl_6.Location")));
      this.combo_extra2_prof_ogl_6.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_ogl_6.MaxDropDownItems")));
      this.combo_extra2_prof_ogl_6.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_ogl_6.MaxLength")));
      this.combo_extra2_prof_ogl_6.Name = "combo_extra2_prof_ogl_6";
      this.combo_extra2_prof_ogl_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_ogl_6.RightToLeft")));
      this.combo_extra2_prof_ogl_6.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_ogl_6.Size")));
      this.combo_extra2_prof_ogl_6.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_ogl_6.TabIndex")));
      this.combo_extra2_prof_ogl_6.Text = resources.GetString("combo_extra2_prof_ogl_6.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_ogl_6, resources.GetString("combo_extra2_prof_ogl_6.ToolTip"));
      this.combo_extra2_prof_ogl_6.Visible = ((bool)(resources.GetObject("combo_extra2_prof_ogl_6.Visible")));
      // 
      // combo_extra_curr_d3d_1
      // 
      this.combo_extra_curr_d3d_1.AccessibleDescription = resources.GetString("combo_extra_curr_d3d_1.AccessibleDescription");
      this.combo_extra_curr_d3d_1.AccessibleName = resources.GetString("combo_extra_curr_d3d_1.AccessibleName");
      this.combo_extra_curr_d3d_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_d3d_1.Anchor")));
      this.combo_extra_curr_d3d_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_d3d_1.BackgroundImage")));
      this.combo_extra_curr_d3d_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_d3d_1.Dock")));
      this.combo_extra_curr_d3d_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_d3d_1.Enabled = ((bool)(resources.GetObject("combo_extra_curr_d3d_1.Enabled")));
      this.combo_extra_curr_d3d_1.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_d3d_1.Font")));
      this.combo_extra_curr_d3d_1.ForeColor = System.Drawing.SystemColors.WindowText;
      this.combo_extra_curr_d3d_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_d3d_1.ImeMode")));
      this.combo_extra_curr_d3d_1.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_d3d_1.IntegralHeight")));
      this.combo_extra_curr_d3d_1.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_d3d_1.ItemHeight")));
      this.combo_extra_curr_d3d_1.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_d3d_1.Location")));
      this.combo_extra_curr_d3d_1.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_d3d_1.MaxDropDownItems")));
      this.combo_extra_curr_d3d_1.MaxLength = ((int)(resources.GetObject("combo_extra_curr_d3d_1.MaxLength")));
      this.combo_extra_curr_d3d_1.Name = "combo_extra_curr_d3d_1";
      this.combo_extra_curr_d3d_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_d3d_1.RightToLeft")));
      this.combo_extra_curr_d3d_1.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_d3d_1.Size")));
      this.combo_extra_curr_d3d_1.TabIndex = ((int)(resources.GetObject("combo_extra_curr_d3d_1.TabIndex")));
      this.combo_extra_curr_d3d_1.Text = resources.GetString("combo_extra_curr_d3d_1.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_d3d_1, resources.GetString("combo_extra_curr_d3d_1.ToolTip"));
      this.combo_extra_curr_d3d_1.Visible = ((bool)(resources.GetObject("combo_extra_curr_d3d_1.Visible")));
      // 
      // combo_extra_curr_d3d_2
      // 
      this.combo_extra_curr_d3d_2.AccessibleDescription = resources.GetString("combo_extra_curr_d3d_2.AccessibleDescription");
      this.combo_extra_curr_d3d_2.AccessibleName = resources.GetString("combo_extra_curr_d3d_2.AccessibleName");
      this.combo_extra_curr_d3d_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_d3d_2.Anchor")));
      this.combo_extra_curr_d3d_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_d3d_2.BackgroundImage")));
      this.combo_extra_curr_d3d_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_d3d_2.Dock")));
      this.combo_extra_curr_d3d_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_d3d_2.Enabled = ((bool)(resources.GetObject("combo_extra_curr_d3d_2.Enabled")));
      this.combo_extra_curr_d3d_2.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_d3d_2.Font")));
      this.combo_extra_curr_d3d_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_d3d_2.ImeMode")));
      this.combo_extra_curr_d3d_2.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_d3d_2.IntegralHeight")));
      this.combo_extra_curr_d3d_2.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_d3d_2.ItemHeight")));
      this.combo_extra_curr_d3d_2.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_d3d_2.Location")));
      this.combo_extra_curr_d3d_2.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_d3d_2.MaxDropDownItems")));
      this.combo_extra_curr_d3d_2.MaxLength = ((int)(resources.GetObject("combo_extra_curr_d3d_2.MaxLength")));
      this.combo_extra_curr_d3d_2.Name = "combo_extra_curr_d3d_2";
      this.combo_extra_curr_d3d_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_d3d_2.RightToLeft")));
      this.combo_extra_curr_d3d_2.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_d3d_2.Size")));
      this.combo_extra_curr_d3d_2.TabIndex = ((int)(resources.GetObject("combo_extra_curr_d3d_2.TabIndex")));
      this.combo_extra_curr_d3d_2.Text = resources.GetString("combo_extra_curr_d3d_2.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_d3d_2, resources.GetString("combo_extra_curr_d3d_2.ToolTip"));
      this.combo_extra_curr_d3d_2.Visible = ((bool)(resources.GetObject("combo_extra_curr_d3d_2.Visible")));
      // 
      // combo_extra_curr_d3d_3
      // 
      this.combo_extra_curr_d3d_3.AccessibleDescription = resources.GetString("combo_extra_curr_d3d_3.AccessibleDescription");
      this.combo_extra_curr_d3d_3.AccessibleName = resources.GetString("combo_extra_curr_d3d_3.AccessibleName");
      this.combo_extra_curr_d3d_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_d3d_3.Anchor")));
      this.combo_extra_curr_d3d_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_d3d_3.BackgroundImage")));
      this.combo_extra_curr_d3d_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_d3d_3.Dock")));
      this.combo_extra_curr_d3d_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_d3d_3.Enabled = ((bool)(resources.GetObject("combo_extra_curr_d3d_3.Enabled")));
      this.combo_extra_curr_d3d_3.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_d3d_3.Font")));
      this.combo_extra_curr_d3d_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_d3d_3.ImeMode")));
      this.combo_extra_curr_d3d_3.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_d3d_3.IntegralHeight")));
      this.combo_extra_curr_d3d_3.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_d3d_3.ItemHeight")));
      this.combo_extra_curr_d3d_3.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_d3d_3.Location")));
      this.combo_extra_curr_d3d_3.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_d3d_3.MaxDropDownItems")));
      this.combo_extra_curr_d3d_3.MaxLength = ((int)(resources.GetObject("combo_extra_curr_d3d_3.MaxLength")));
      this.combo_extra_curr_d3d_3.Name = "combo_extra_curr_d3d_3";
      this.combo_extra_curr_d3d_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_d3d_3.RightToLeft")));
      this.combo_extra_curr_d3d_3.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_d3d_3.Size")));
      this.combo_extra_curr_d3d_3.TabIndex = ((int)(resources.GetObject("combo_extra_curr_d3d_3.TabIndex")));
      this.combo_extra_curr_d3d_3.Text = resources.GetString("combo_extra_curr_d3d_3.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_d3d_3, resources.GetString("combo_extra_curr_d3d_3.ToolTip"));
      this.combo_extra_curr_d3d_3.Visible = ((bool)(resources.GetObject("combo_extra_curr_d3d_3.Visible")));
      // 
      // combo_extra_curr_d3d_4
      // 
      this.combo_extra_curr_d3d_4.AccessibleDescription = resources.GetString("combo_extra_curr_d3d_4.AccessibleDescription");
      this.combo_extra_curr_d3d_4.AccessibleName = resources.GetString("combo_extra_curr_d3d_4.AccessibleName");
      this.combo_extra_curr_d3d_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_d3d_4.Anchor")));
      this.combo_extra_curr_d3d_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_d3d_4.BackgroundImage")));
      this.combo_extra_curr_d3d_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_d3d_4.Dock")));
      this.combo_extra_curr_d3d_4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_d3d_4.Enabled = ((bool)(resources.GetObject("combo_extra_curr_d3d_4.Enabled")));
      this.combo_extra_curr_d3d_4.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_d3d_4.Font")));
      this.combo_extra_curr_d3d_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_d3d_4.ImeMode")));
      this.combo_extra_curr_d3d_4.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_d3d_4.IntegralHeight")));
      this.combo_extra_curr_d3d_4.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_d3d_4.ItemHeight")));
      this.combo_extra_curr_d3d_4.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_d3d_4.Location")));
      this.combo_extra_curr_d3d_4.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_d3d_4.MaxDropDownItems")));
      this.combo_extra_curr_d3d_4.MaxLength = ((int)(resources.GetObject("combo_extra_curr_d3d_4.MaxLength")));
      this.combo_extra_curr_d3d_4.Name = "combo_extra_curr_d3d_4";
      this.combo_extra_curr_d3d_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_d3d_4.RightToLeft")));
      this.combo_extra_curr_d3d_4.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_d3d_4.Size")));
      this.combo_extra_curr_d3d_4.TabIndex = ((int)(resources.GetObject("combo_extra_curr_d3d_4.TabIndex")));
      this.combo_extra_curr_d3d_4.Text = resources.GetString("combo_extra_curr_d3d_4.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_d3d_4, resources.GetString("combo_extra_curr_d3d_4.ToolTip"));
      this.combo_extra_curr_d3d_4.Visible = ((bool)(resources.GetObject("combo_extra_curr_d3d_4.Visible")));
      // 
      // combo_extra_curr_d3d_5
      // 
      this.combo_extra_curr_d3d_5.AccessibleDescription = resources.GetString("combo_extra_curr_d3d_5.AccessibleDescription");
      this.combo_extra_curr_d3d_5.AccessibleName = resources.GetString("combo_extra_curr_d3d_5.AccessibleName");
      this.combo_extra_curr_d3d_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_d3d_5.Anchor")));
      this.combo_extra_curr_d3d_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_d3d_5.BackgroundImage")));
      this.combo_extra_curr_d3d_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_d3d_5.Dock")));
      this.combo_extra_curr_d3d_5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_d3d_5.Enabled = ((bool)(resources.GetObject("combo_extra_curr_d3d_5.Enabled")));
      this.combo_extra_curr_d3d_5.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_d3d_5.Font")));
      this.combo_extra_curr_d3d_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_d3d_5.ImeMode")));
      this.combo_extra_curr_d3d_5.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_d3d_5.IntegralHeight")));
      this.combo_extra_curr_d3d_5.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_d3d_5.ItemHeight")));
      this.combo_extra_curr_d3d_5.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_d3d_5.Location")));
      this.combo_extra_curr_d3d_5.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_d3d_5.MaxDropDownItems")));
      this.combo_extra_curr_d3d_5.MaxLength = ((int)(resources.GetObject("combo_extra_curr_d3d_5.MaxLength")));
      this.combo_extra_curr_d3d_5.Name = "combo_extra_curr_d3d_5";
      this.combo_extra_curr_d3d_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_d3d_5.RightToLeft")));
      this.combo_extra_curr_d3d_5.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_d3d_5.Size")));
      this.combo_extra_curr_d3d_5.TabIndex = ((int)(resources.GetObject("combo_extra_curr_d3d_5.TabIndex")));
      this.combo_extra_curr_d3d_5.Text = resources.GetString("combo_extra_curr_d3d_5.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_d3d_5, resources.GetString("combo_extra_curr_d3d_5.ToolTip"));
      this.combo_extra_curr_d3d_5.Visible = ((bool)(resources.GetObject("combo_extra_curr_d3d_5.Visible")));
      // 
      // combo_extra_curr_d3d_6
      // 
      this.combo_extra_curr_d3d_6.AccessibleDescription = resources.GetString("combo_extra_curr_d3d_6.AccessibleDescription");
      this.combo_extra_curr_d3d_6.AccessibleName = resources.GetString("combo_extra_curr_d3d_6.AccessibleName");
      this.combo_extra_curr_d3d_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_d3d_6.Anchor")));
      this.combo_extra_curr_d3d_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_d3d_6.BackgroundImage")));
      this.combo_extra_curr_d3d_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_d3d_6.Dock")));
      this.combo_extra_curr_d3d_6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_d3d_6.Enabled = ((bool)(resources.GetObject("combo_extra_curr_d3d_6.Enabled")));
      this.combo_extra_curr_d3d_6.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_d3d_6.Font")));
      this.combo_extra_curr_d3d_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_d3d_6.ImeMode")));
      this.combo_extra_curr_d3d_6.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_d3d_6.IntegralHeight")));
      this.combo_extra_curr_d3d_6.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_d3d_6.ItemHeight")));
      this.combo_extra_curr_d3d_6.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_d3d_6.Location")));
      this.combo_extra_curr_d3d_6.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_d3d_6.MaxDropDownItems")));
      this.combo_extra_curr_d3d_6.MaxLength = ((int)(resources.GetObject("combo_extra_curr_d3d_6.MaxLength")));
      this.combo_extra_curr_d3d_6.Name = "combo_extra_curr_d3d_6";
      this.combo_extra_curr_d3d_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_d3d_6.RightToLeft")));
      this.combo_extra_curr_d3d_6.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_d3d_6.Size")));
      this.combo_extra_curr_d3d_6.TabIndex = ((int)(resources.GetObject("combo_extra_curr_d3d_6.TabIndex")));
      this.combo_extra_curr_d3d_6.Text = resources.GetString("combo_extra_curr_d3d_6.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_d3d_6, resources.GetString("combo_extra_curr_d3d_6.ToolTip"));
      this.combo_extra_curr_d3d_6.Visible = ((bool)(resources.GetObject("combo_extra_curr_d3d_6.Visible")));
      // 
      // combo_extra_curr_ogl_1
      // 
      this.combo_extra_curr_ogl_1.AccessibleDescription = resources.GetString("combo_extra_curr_ogl_1.AccessibleDescription");
      this.combo_extra_curr_ogl_1.AccessibleName = resources.GetString("combo_extra_curr_ogl_1.AccessibleName");
      this.combo_extra_curr_ogl_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_ogl_1.Anchor")));
      this.combo_extra_curr_ogl_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_ogl_1.BackgroundImage")));
      this.combo_extra_curr_ogl_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_ogl_1.Dock")));
      this.combo_extra_curr_ogl_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_ogl_1.Enabled = ((bool)(resources.GetObject("combo_extra_curr_ogl_1.Enabled")));
      this.combo_extra_curr_ogl_1.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_ogl_1.Font")));
      this.combo_extra_curr_ogl_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_ogl_1.ImeMode")));
      this.combo_extra_curr_ogl_1.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_ogl_1.IntegralHeight")));
      this.combo_extra_curr_ogl_1.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_ogl_1.ItemHeight")));
      this.combo_extra_curr_ogl_1.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_ogl_1.Location")));
      this.combo_extra_curr_ogl_1.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_ogl_1.MaxDropDownItems")));
      this.combo_extra_curr_ogl_1.MaxLength = ((int)(resources.GetObject("combo_extra_curr_ogl_1.MaxLength")));
      this.combo_extra_curr_ogl_1.Name = "combo_extra_curr_ogl_1";
      this.combo_extra_curr_ogl_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_ogl_1.RightToLeft")));
      this.combo_extra_curr_ogl_1.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_ogl_1.Size")));
      this.combo_extra_curr_ogl_1.TabIndex = ((int)(resources.GetObject("combo_extra_curr_ogl_1.TabIndex")));
      this.combo_extra_curr_ogl_1.Text = resources.GetString("combo_extra_curr_ogl_1.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_ogl_1, resources.GetString("combo_extra_curr_ogl_1.ToolTip"));
      this.combo_extra_curr_ogl_1.Visible = ((bool)(resources.GetObject("combo_extra_curr_ogl_1.Visible")));
      // 
      // combo_extra_curr_ogl_2
      // 
      this.combo_extra_curr_ogl_2.AccessibleDescription = resources.GetString("combo_extra_curr_ogl_2.AccessibleDescription");
      this.combo_extra_curr_ogl_2.AccessibleName = resources.GetString("combo_extra_curr_ogl_2.AccessibleName");
      this.combo_extra_curr_ogl_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_ogl_2.Anchor")));
      this.combo_extra_curr_ogl_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_ogl_2.BackgroundImage")));
      this.combo_extra_curr_ogl_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_ogl_2.Dock")));
      this.combo_extra_curr_ogl_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_ogl_2.Enabled = ((bool)(resources.GetObject("combo_extra_curr_ogl_2.Enabled")));
      this.combo_extra_curr_ogl_2.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_ogl_2.Font")));
      this.combo_extra_curr_ogl_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_ogl_2.ImeMode")));
      this.combo_extra_curr_ogl_2.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_ogl_2.IntegralHeight")));
      this.combo_extra_curr_ogl_2.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_ogl_2.ItemHeight")));
      this.combo_extra_curr_ogl_2.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_ogl_2.Location")));
      this.combo_extra_curr_ogl_2.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_ogl_2.MaxDropDownItems")));
      this.combo_extra_curr_ogl_2.MaxLength = ((int)(resources.GetObject("combo_extra_curr_ogl_2.MaxLength")));
      this.combo_extra_curr_ogl_2.Name = "combo_extra_curr_ogl_2";
      this.combo_extra_curr_ogl_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_ogl_2.RightToLeft")));
      this.combo_extra_curr_ogl_2.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_ogl_2.Size")));
      this.combo_extra_curr_ogl_2.TabIndex = ((int)(resources.GetObject("combo_extra_curr_ogl_2.TabIndex")));
      this.combo_extra_curr_ogl_2.Text = resources.GetString("combo_extra_curr_ogl_2.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_ogl_2, resources.GetString("combo_extra_curr_ogl_2.ToolTip"));
      this.combo_extra_curr_ogl_2.Visible = ((bool)(resources.GetObject("combo_extra_curr_ogl_2.Visible")));
      // 
      // combo_extra_curr_ogl_3
      // 
      this.combo_extra_curr_ogl_3.AccessibleDescription = resources.GetString("combo_extra_curr_ogl_3.AccessibleDescription");
      this.combo_extra_curr_ogl_3.AccessibleName = resources.GetString("combo_extra_curr_ogl_3.AccessibleName");
      this.combo_extra_curr_ogl_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_ogl_3.Anchor")));
      this.combo_extra_curr_ogl_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_ogl_3.BackgroundImage")));
      this.combo_extra_curr_ogl_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_ogl_3.Dock")));
      this.combo_extra_curr_ogl_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_ogl_3.Enabled = ((bool)(resources.GetObject("combo_extra_curr_ogl_3.Enabled")));
      this.combo_extra_curr_ogl_3.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_ogl_3.Font")));
      this.combo_extra_curr_ogl_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_ogl_3.ImeMode")));
      this.combo_extra_curr_ogl_3.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_ogl_3.IntegralHeight")));
      this.combo_extra_curr_ogl_3.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_ogl_3.ItemHeight")));
      this.combo_extra_curr_ogl_3.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_ogl_3.Location")));
      this.combo_extra_curr_ogl_3.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_ogl_3.MaxDropDownItems")));
      this.combo_extra_curr_ogl_3.MaxLength = ((int)(resources.GetObject("combo_extra_curr_ogl_3.MaxLength")));
      this.combo_extra_curr_ogl_3.Name = "combo_extra_curr_ogl_3";
      this.combo_extra_curr_ogl_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_ogl_3.RightToLeft")));
      this.combo_extra_curr_ogl_3.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_ogl_3.Size")));
      this.combo_extra_curr_ogl_3.TabIndex = ((int)(resources.GetObject("combo_extra_curr_ogl_3.TabIndex")));
      this.combo_extra_curr_ogl_3.Text = resources.GetString("combo_extra_curr_ogl_3.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_ogl_3, resources.GetString("combo_extra_curr_ogl_3.ToolTip"));
      this.combo_extra_curr_ogl_3.Visible = ((bool)(resources.GetObject("combo_extra_curr_ogl_3.Visible")));
      // 
      // combo_extra_curr_ogl_4
      // 
      this.combo_extra_curr_ogl_4.AccessibleDescription = resources.GetString("combo_extra_curr_ogl_4.AccessibleDescription");
      this.combo_extra_curr_ogl_4.AccessibleName = resources.GetString("combo_extra_curr_ogl_4.AccessibleName");
      this.combo_extra_curr_ogl_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_ogl_4.Anchor")));
      this.combo_extra_curr_ogl_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_ogl_4.BackgroundImage")));
      this.combo_extra_curr_ogl_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_ogl_4.Dock")));
      this.combo_extra_curr_ogl_4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_ogl_4.Enabled = ((bool)(resources.GetObject("combo_extra_curr_ogl_4.Enabled")));
      this.combo_extra_curr_ogl_4.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_ogl_4.Font")));
      this.combo_extra_curr_ogl_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_ogl_4.ImeMode")));
      this.combo_extra_curr_ogl_4.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_ogl_4.IntegralHeight")));
      this.combo_extra_curr_ogl_4.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_ogl_4.ItemHeight")));
      this.combo_extra_curr_ogl_4.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_ogl_4.Location")));
      this.combo_extra_curr_ogl_4.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_ogl_4.MaxDropDownItems")));
      this.combo_extra_curr_ogl_4.MaxLength = ((int)(resources.GetObject("combo_extra_curr_ogl_4.MaxLength")));
      this.combo_extra_curr_ogl_4.Name = "combo_extra_curr_ogl_4";
      this.combo_extra_curr_ogl_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_ogl_4.RightToLeft")));
      this.combo_extra_curr_ogl_4.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_ogl_4.Size")));
      this.combo_extra_curr_ogl_4.TabIndex = ((int)(resources.GetObject("combo_extra_curr_ogl_4.TabIndex")));
      this.combo_extra_curr_ogl_4.Text = resources.GetString("combo_extra_curr_ogl_4.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_ogl_4, resources.GetString("combo_extra_curr_ogl_4.ToolTip"));
      this.combo_extra_curr_ogl_4.Visible = ((bool)(resources.GetObject("combo_extra_curr_ogl_4.Visible")));
      // 
      // combo_extra_curr_ogl_5
      // 
      this.combo_extra_curr_ogl_5.AccessibleDescription = resources.GetString("combo_extra_curr_ogl_5.AccessibleDescription");
      this.combo_extra_curr_ogl_5.AccessibleName = resources.GetString("combo_extra_curr_ogl_5.AccessibleName");
      this.combo_extra_curr_ogl_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_ogl_5.Anchor")));
      this.combo_extra_curr_ogl_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_ogl_5.BackgroundImage")));
      this.combo_extra_curr_ogl_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_ogl_5.Dock")));
      this.combo_extra_curr_ogl_5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_ogl_5.Enabled = ((bool)(resources.GetObject("combo_extra_curr_ogl_5.Enabled")));
      this.combo_extra_curr_ogl_5.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_ogl_5.Font")));
      this.combo_extra_curr_ogl_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_ogl_5.ImeMode")));
      this.combo_extra_curr_ogl_5.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_ogl_5.IntegralHeight")));
      this.combo_extra_curr_ogl_5.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_ogl_5.ItemHeight")));
      this.combo_extra_curr_ogl_5.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_ogl_5.Location")));
      this.combo_extra_curr_ogl_5.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_ogl_5.MaxDropDownItems")));
      this.combo_extra_curr_ogl_5.MaxLength = ((int)(resources.GetObject("combo_extra_curr_ogl_5.MaxLength")));
      this.combo_extra_curr_ogl_5.Name = "combo_extra_curr_ogl_5";
      this.combo_extra_curr_ogl_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_ogl_5.RightToLeft")));
      this.combo_extra_curr_ogl_5.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_ogl_5.Size")));
      this.combo_extra_curr_ogl_5.TabIndex = ((int)(resources.GetObject("combo_extra_curr_ogl_5.TabIndex")));
      this.combo_extra_curr_ogl_5.Text = resources.GetString("combo_extra_curr_ogl_5.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_ogl_5, resources.GetString("combo_extra_curr_ogl_5.ToolTip"));
      this.combo_extra_curr_ogl_5.Visible = ((bool)(resources.GetObject("combo_extra_curr_ogl_5.Visible")));
      // 
      // combo_extra_curr_ogl_6
      // 
      this.combo_extra_curr_ogl_6.AccessibleDescription = resources.GetString("combo_extra_curr_ogl_6.AccessibleDescription");
      this.combo_extra_curr_ogl_6.AccessibleName = resources.GetString("combo_extra_curr_ogl_6.AccessibleName");
      this.combo_extra_curr_ogl_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_ogl_6.Anchor")));
      this.combo_extra_curr_ogl_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_ogl_6.BackgroundImage")));
      this.combo_extra_curr_ogl_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_ogl_6.Dock")));
      this.combo_extra_curr_ogl_6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_ogl_6.Enabled = ((bool)(resources.GetObject("combo_extra_curr_ogl_6.Enabled")));
      this.combo_extra_curr_ogl_6.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_ogl_6.Font")));
      this.combo_extra_curr_ogl_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_ogl_6.ImeMode")));
      this.combo_extra_curr_ogl_6.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_ogl_6.IntegralHeight")));
      this.combo_extra_curr_ogl_6.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_ogl_6.ItemHeight")));
      this.combo_extra_curr_ogl_6.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_ogl_6.Location")));
      this.combo_extra_curr_ogl_6.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_ogl_6.MaxDropDownItems")));
      this.combo_extra_curr_ogl_6.MaxLength = ((int)(resources.GetObject("combo_extra_curr_ogl_6.MaxLength")));
      this.combo_extra_curr_ogl_6.Name = "combo_extra_curr_ogl_6";
      this.combo_extra_curr_ogl_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_ogl_6.RightToLeft")));
      this.combo_extra_curr_ogl_6.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_ogl_6.Size")));
      this.combo_extra_curr_ogl_6.TabIndex = ((int)(resources.GetObject("combo_extra_curr_ogl_6.TabIndex")));
      this.combo_extra_curr_ogl_6.Text = resources.GetString("combo_extra_curr_ogl_6.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_ogl_6, resources.GetString("combo_extra_curr_ogl_6.ToolTip"));
      this.combo_extra_curr_ogl_6.Visible = ((bool)(resources.GetObject("combo_extra_curr_ogl_6.Visible")));
      // 
      // combo_extra_prof_d3d_1
      // 
      this.combo_extra_prof_d3d_1.AccessibleDescription = resources.GetString("combo_extra_prof_d3d_1.AccessibleDescription");
      this.combo_extra_prof_d3d_1.AccessibleName = resources.GetString("combo_extra_prof_d3d_1.AccessibleName");
      this.combo_extra_prof_d3d_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_d3d_1.Anchor")));
      this.combo_extra_prof_d3d_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_d3d_1.BackgroundImage")));
      this.combo_extra_prof_d3d_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_d3d_1.Dock")));
      this.combo_extra_prof_d3d_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_d3d_1.Enabled = ((bool)(resources.GetObject("combo_extra_prof_d3d_1.Enabled")));
      this.combo_extra_prof_d3d_1.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_d3d_1.Font")));
      this.combo_extra_prof_d3d_1.ForeColor = System.Drawing.SystemColors.WindowText;
      this.combo_extra_prof_d3d_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_d3d_1.ImeMode")));
      this.combo_extra_prof_d3d_1.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_d3d_1.IntegralHeight")));
      this.combo_extra_prof_d3d_1.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_d3d_1.ItemHeight")));
      this.combo_extra_prof_d3d_1.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_d3d_1.Location")));
      this.combo_extra_prof_d3d_1.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_d3d_1.MaxDropDownItems")));
      this.combo_extra_prof_d3d_1.MaxLength = ((int)(resources.GetObject("combo_extra_prof_d3d_1.MaxLength")));
      this.combo_extra_prof_d3d_1.Name = "combo_extra_prof_d3d_1";
      this.combo_extra_prof_d3d_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_d3d_1.RightToLeft")));
      this.combo_extra_prof_d3d_1.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_d3d_1.Size")));
      this.combo_extra_prof_d3d_1.TabIndex = ((int)(resources.GetObject("combo_extra_prof_d3d_1.TabIndex")));
      this.combo_extra_prof_d3d_1.Text = resources.GetString("combo_extra_prof_d3d_1.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_d3d_1, resources.GetString("combo_extra_prof_d3d_1.ToolTip"));
      this.combo_extra_prof_d3d_1.Visible = ((bool)(resources.GetObject("combo_extra_prof_d3d_1.Visible")));
      // 
      // combo_extra_prof_d3d_2
      // 
      this.combo_extra_prof_d3d_2.AccessibleDescription = resources.GetString("combo_extra_prof_d3d_2.AccessibleDescription");
      this.combo_extra_prof_d3d_2.AccessibleName = resources.GetString("combo_extra_prof_d3d_2.AccessibleName");
      this.combo_extra_prof_d3d_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_d3d_2.Anchor")));
      this.combo_extra_prof_d3d_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_d3d_2.BackgroundImage")));
      this.combo_extra_prof_d3d_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_d3d_2.Dock")));
      this.combo_extra_prof_d3d_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_d3d_2.Enabled = ((bool)(resources.GetObject("combo_extra_prof_d3d_2.Enabled")));
      this.combo_extra_prof_d3d_2.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_d3d_2.Font")));
      this.combo_extra_prof_d3d_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_d3d_2.ImeMode")));
      this.combo_extra_prof_d3d_2.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_d3d_2.IntegralHeight")));
      this.combo_extra_prof_d3d_2.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_d3d_2.ItemHeight")));
      this.combo_extra_prof_d3d_2.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_d3d_2.Location")));
      this.combo_extra_prof_d3d_2.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_d3d_2.MaxDropDownItems")));
      this.combo_extra_prof_d3d_2.MaxLength = ((int)(resources.GetObject("combo_extra_prof_d3d_2.MaxLength")));
      this.combo_extra_prof_d3d_2.Name = "combo_extra_prof_d3d_2";
      this.combo_extra_prof_d3d_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_d3d_2.RightToLeft")));
      this.combo_extra_prof_d3d_2.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_d3d_2.Size")));
      this.combo_extra_prof_d3d_2.TabIndex = ((int)(resources.GetObject("combo_extra_prof_d3d_2.TabIndex")));
      this.combo_extra_prof_d3d_2.Text = resources.GetString("combo_extra_prof_d3d_2.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_d3d_2, resources.GetString("combo_extra_prof_d3d_2.ToolTip"));
      this.combo_extra_prof_d3d_2.Visible = ((bool)(resources.GetObject("combo_extra_prof_d3d_2.Visible")));
      // 
      // combo_extra_prof_d3d_3
      // 
      this.combo_extra_prof_d3d_3.AccessibleDescription = resources.GetString("combo_extra_prof_d3d_3.AccessibleDescription");
      this.combo_extra_prof_d3d_3.AccessibleName = resources.GetString("combo_extra_prof_d3d_3.AccessibleName");
      this.combo_extra_prof_d3d_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_d3d_3.Anchor")));
      this.combo_extra_prof_d3d_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_d3d_3.BackgroundImage")));
      this.combo_extra_prof_d3d_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_d3d_3.Dock")));
      this.combo_extra_prof_d3d_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_d3d_3.Enabled = ((bool)(resources.GetObject("combo_extra_prof_d3d_3.Enabled")));
      this.combo_extra_prof_d3d_3.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_d3d_3.Font")));
      this.combo_extra_prof_d3d_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_d3d_3.ImeMode")));
      this.combo_extra_prof_d3d_3.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_d3d_3.IntegralHeight")));
      this.combo_extra_prof_d3d_3.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_d3d_3.ItemHeight")));
      this.combo_extra_prof_d3d_3.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_d3d_3.Location")));
      this.combo_extra_prof_d3d_3.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_d3d_3.MaxDropDownItems")));
      this.combo_extra_prof_d3d_3.MaxLength = ((int)(resources.GetObject("combo_extra_prof_d3d_3.MaxLength")));
      this.combo_extra_prof_d3d_3.Name = "combo_extra_prof_d3d_3";
      this.combo_extra_prof_d3d_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_d3d_3.RightToLeft")));
      this.combo_extra_prof_d3d_3.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_d3d_3.Size")));
      this.combo_extra_prof_d3d_3.TabIndex = ((int)(resources.GetObject("combo_extra_prof_d3d_3.TabIndex")));
      this.combo_extra_prof_d3d_3.Text = resources.GetString("combo_extra_prof_d3d_3.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_d3d_3, resources.GetString("combo_extra_prof_d3d_3.ToolTip"));
      this.combo_extra_prof_d3d_3.Visible = ((bool)(resources.GetObject("combo_extra_prof_d3d_3.Visible")));
      // 
      // combo_extra_prof_d3d_4
      // 
      this.combo_extra_prof_d3d_4.AccessibleDescription = resources.GetString("combo_extra_prof_d3d_4.AccessibleDescription");
      this.combo_extra_prof_d3d_4.AccessibleName = resources.GetString("combo_extra_prof_d3d_4.AccessibleName");
      this.combo_extra_prof_d3d_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_d3d_4.Anchor")));
      this.combo_extra_prof_d3d_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_d3d_4.BackgroundImage")));
      this.combo_extra_prof_d3d_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_d3d_4.Dock")));
      this.combo_extra_prof_d3d_4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_d3d_4.Enabled = ((bool)(resources.GetObject("combo_extra_prof_d3d_4.Enabled")));
      this.combo_extra_prof_d3d_4.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_d3d_4.Font")));
      this.combo_extra_prof_d3d_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_d3d_4.ImeMode")));
      this.combo_extra_prof_d3d_4.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_d3d_4.IntegralHeight")));
      this.combo_extra_prof_d3d_4.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_d3d_4.ItemHeight")));
      this.combo_extra_prof_d3d_4.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_d3d_4.Location")));
      this.combo_extra_prof_d3d_4.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_d3d_4.MaxDropDownItems")));
      this.combo_extra_prof_d3d_4.MaxLength = ((int)(resources.GetObject("combo_extra_prof_d3d_4.MaxLength")));
      this.combo_extra_prof_d3d_4.Name = "combo_extra_prof_d3d_4";
      this.combo_extra_prof_d3d_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_d3d_4.RightToLeft")));
      this.combo_extra_prof_d3d_4.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_d3d_4.Size")));
      this.combo_extra_prof_d3d_4.TabIndex = ((int)(resources.GetObject("combo_extra_prof_d3d_4.TabIndex")));
      this.combo_extra_prof_d3d_4.Text = resources.GetString("combo_extra_prof_d3d_4.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_d3d_4, resources.GetString("combo_extra_prof_d3d_4.ToolTip"));
      this.combo_extra_prof_d3d_4.Visible = ((bool)(resources.GetObject("combo_extra_prof_d3d_4.Visible")));
      // 
      // combo_extra_prof_d3d_5
      // 
      this.combo_extra_prof_d3d_5.AccessibleDescription = resources.GetString("combo_extra_prof_d3d_5.AccessibleDescription");
      this.combo_extra_prof_d3d_5.AccessibleName = resources.GetString("combo_extra_prof_d3d_5.AccessibleName");
      this.combo_extra_prof_d3d_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_d3d_5.Anchor")));
      this.combo_extra_prof_d3d_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_d3d_5.BackgroundImage")));
      this.combo_extra_prof_d3d_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_d3d_5.Dock")));
      this.combo_extra_prof_d3d_5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_d3d_5.Enabled = ((bool)(resources.GetObject("combo_extra_prof_d3d_5.Enabled")));
      this.combo_extra_prof_d3d_5.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_d3d_5.Font")));
      this.combo_extra_prof_d3d_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_d3d_5.ImeMode")));
      this.combo_extra_prof_d3d_5.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_d3d_5.IntegralHeight")));
      this.combo_extra_prof_d3d_5.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_d3d_5.ItemHeight")));
      this.combo_extra_prof_d3d_5.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_d3d_5.Location")));
      this.combo_extra_prof_d3d_5.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_d3d_5.MaxDropDownItems")));
      this.combo_extra_prof_d3d_5.MaxLength = ((int)(resources.GetObject("combo_extra_prof_d3d_5.MaxLength")));
      this.combo_extra_prof_d3d_5.Name = "combo_extra_prof_d3d_5";
      this.combo_extra_prof_d3d_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_d3d_5.RightToLeft")));
      this.combo_extra_prof_d3d_5.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_d3d_5.Size")));
      this.combo_extra_prof_d3d_5.TabIndex = ((int)(resources.GetObject("combo_extra_prof_d3d_5.TabIndex")));
      this.combo_extra_prof_d3d_5.Text = resources.GetString("combo_extra_prof_d3d_5.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_d3d_5, resources.GetString("combo_extra_prof_d3d_5.ToolTip"));
      this.combo_extra_prof_d3d_5.Visible = ((bool)(resources.GetObject("combo_extra_prof_d3d_5.Visible")));
      // 
      // combo_extra_prof_d3d_6
      // 
      this.combo_extra_prof_d3d_6.AccessibleDescription = resources.GetString("combo_extra_prof_d3d_6.AccessibleDescription");
      this.combo_extra_prof_d3d_6.AccessibleName = resources.GetString("combo_extra_prof_d3d_6.AccessibleName");
      this.combo_extra_prof_d3d_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_d3d_6.Anchor")));
      this.combo_extra_prof_d3d_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_d3d_6.BackgroundImage")));
      this.combo_extra_prof_d3d_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_d3d_6.Dock")));
      this.combo_extra_prof_d3d_6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_d3d_6.Enabled = ((bool)(resources.GetObject("combo_extra_prof_d3d_6.Enabled")));
      this.combo_extra_prof_d3d_6.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_d3d_6.Font")));
      this.combo_extra_prof_d3d_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_d3d_6.ImeMode")));
      this.combo_extra_prof_d3d_6.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_d3d_6.IntegralHeight")));
      this.combo_extra_prof_d3d_6.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_d3d_6.ItemHeight")));
      this.combo_extra_prof_d3d_6.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_d3d_6.Location")));
      this.combo_extra_prof_d3d_6.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_d3d_6.MaxDropDownItems")));
      this.combo_extra_prof_d3d_6.MaxLength = ((int)(resources.GetObject("combo_extra_prof_d3d_6.MaxLength")));
      this.combo_extra_prof_d3d_6.Name = "combo_extra_prof_d3d_6";
      this.combo_extra_prof_d3d_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_d3d_6.RightToLeft")));
      this.combo_extra_prof_d3d_6.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_d3d_6.Size")));
      this.combo_extra_prof_d3d_6.TabIndex = ((int)(resources.GetObject("combo_extra_prof_d3d_6.TabIndex")));
      this.combo_extra_prof_d3d_6.Text = resources.GetString("combo_extra_prof_d3d_6.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_d3d_6, resources.GetString("combo_extra_prof_d3d_6.ToolTip"));
      this.combo_extra_prof_d3d_6.Visible = ((bool)(resources.GetObject("combo_extra_prof_d3d_6.Visible")));
      // 
      // combo_extra_prof_ogl_1
      // 
      this.combo_extra_prof_ogl_1.AccessibleDescription = resources.GetString("combo_extra_prof_ogl_1.AccessibleDescription");
      this.combo_extra_prof_ogl_1.AccessibleName = resources.GetString("combo_extra_prof_ogl_1.AccessibleName");
      this.combo_extra_prof_ogl_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_ogl_1.Anchor")));
      this.combo_extra_prof_ogl_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_ogl_1.BackgroundImage")));
      this.combo_extra_prof_ogl_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_ogl_1.Dock")));
      this.combo_extra_prof_ogl_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_ogl_1.Enabled = ((bool)(resources.GetObject("combo_extra_prof_ogl_1.Enabled")));
      this.combo_extra_prof_ogl_1.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_ogl_1.Font")));
      this.combo_extra_prof_ogl_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_ogl_1.ImeMode")));
      this.combo_extra_prof_ogl_1.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_ogl_1.IntegralHeight")));
      this.combo_extra_prof_ogl_1.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_ogl_1.ItemHeight")));
      this.combo_extra_prof_ogl_1.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_ogl_1.Location")));
      this.combo_extra_prof_ogl_1.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_ogl_1.MaxDropDownItems")));
      this.combo_extra_prof_ogl_1.MaxLength = ((int)(resources.GetObject("combo_extra_prof_ogl_1.MaxLength")));
      this.combo_extra_prof_ogl_1.Name = "combo_extra_prof_ogl_1";
      this.combo_extra_prof_ogl_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_ogl_1.RightToLeft")));
      this.combo_extra_prof_ogl_1.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_ogl_1.Size")));
      this.combo_extra_prof_ogl_1.TabIndex = ((int)(resources.GetObject("combo_extra_prof_ogl_1.TabIndex")));
      this.combo_extra_prof_ogl_1.Text = resources.GetString("combo_extra_prof_ogl_1.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_ogl_1, resources.GetString("combo_extra_prof_ogl_1.ToolTip"));
      this.combo_extra_prof_ogl_1.Visible = ((bool)(resources.GetObject("combo_extra_prof_ogl_1.Visible")));
      // 
      // combo_extra_prof_ogl_2
      // 
      this.combo_extra_prof_ogl_2.AccessibleDescription = resources.GetString("combo_extra_prof_ogl_2.AccessibleDescription");
      this.combo_extra_prof_ogl_2.AccessibleName = resources.GetString("combo_extra_prof_ogl_2.AccessibleName");
      this.combo_extra_prof_ogl_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_ogl_2.Anchor")));
      this.combo_extra_prof_ogl_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_ogl_2.BackgroundImage")));
      this.combo_extra_prof_ogl_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_ogl_2.Dock")));
      this.combo_extra_prof_ogl_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_ogl_2.Enabled = ((bool)(resources.GetObject("combo_extra_prof_ogl_2.Enabled")));
      this.combo_extra_prof_ogl_2.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_ogl_2.Font")));
      this.combo_extra_prof_ogl_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_ogl_2.ImeMode")));
      this.combo_extra_prof_ogl_2.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_ogl_2.IntegralHeight")));
      this.combo_extra_prof_ogl_2.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_ogl_2.ItemHeight")));
      this.combo_extra_prof_ogl_2.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_ogl_2.Location")));
      this.combo_extra_prof_ogl_2.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_ogl_2.MaxDropDownItems")));
      this.combo_extra_prof_ogl_2.MaxLength = ((int)(resources.GetObject("combo_extra_prof_ogl_2.MaxLength")));
      this.combo_extra_prof_ogl_2.Name = "combo_extra_prof_ogl_2";
      this.combo_extra_prof_ogl_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_ogl_2.RightToLeft")));
      this.combo_extra_prof_ogl_2.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_ogl_2.Size")));
      this.combo_extra_prof_ogl_2.TabIndex = ((int)(resources.GetObject("combo_extra_prof_ogl_2.TabIndex")));
      this.combo_extra_prof_ogl_2.Text = resources.GetString("combo_extra_prof_ogl_2.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_ogl_2, resources.GetString("combo_extra_prof_ogl_2.ToolTip"));
      this.combo_extra_prof_ogl_2.Visible = ((bool)(resources.GetObject("combo_extra_prof_ogl_2.Visible")));
      // 
      // combo_extra_prof_ogl_3
      // 
      this.combo_extra_prof_ogl_3.AccessibleDescription = resources.GetString("combo_extra_prof_ogl_3.AccessibleDescription");
      this.combo_extra_prof_ogl_3.AccessibleName = resources.GetString("combo_extra_prof_ogl_3.AccessibleName");
      this.combo_extra_prof_ogl_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_ogl_3.Anchor")));
      this.combo_extra_prof_ogl_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_ogl_3.BackgroundImage")));
      this.combo_extra_prof_ogl_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_ogl_3.Dock")));
      this.combo_extra_prof_ogl_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_ogl_3.Enabled = ((bool)(resources.GetObject("combo_extra_prof_ogl_3.Enabled")));
      this.combo_extra_prof_ogl_3.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_ogl_3.Font")));
      this.combo_extra_prof_ogl_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_ogl_3.ImeMode")));
      this.combo_extra_prof_ogl_3.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_ogl_3.IntegralHeight")));
      this.combo_extra_prof_ogl_3.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_ogl_3.ItemHeight")));
      this.combo_extra_prof_ogl_3.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_ogl_3.Location")));
      this.combo_extra_prof_ogl_3.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_ogl_3.MaxDropDownItems")));
      this.combo_extra_prof_ogl_3.MaxLength = ((int)(resources.GetObject("combo_extra_prof_ogl_3.MaxLength")));
      this.combo_extra_prof_ogl_3.Name = "combo_extra_prof_ogl_3";
      this.combo_extra_prof_ogl_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_ogl_3.RightToLeft")));
      this.combo_extra_prof_ogl_3.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_ogl_3.Size")));
      this.combo_extra_prof_ogl_3.TabIndex = ((int)(resources.GetObject("combo_extra_prof_ogl_3.TabIndex")));
      this.combo_extra_prof_ogl_3.Text = resources.GetString("combo_extra_prof_ogl_3.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_ogl_3, resources.GetString("combo_extra_prof_ogl_3.ToolTip"));
      this.combo_extra_prof_ogl_3.Visible = ((bool)(resources.GetObject("combo_extra_prof_ogl_3.Visible")));
      // 
      // combo_extra_prof_ogl_4
      // 
      this.combo_extra_prof_ogl_4.AccessibleDescription = resources.GetString("combo_extra_prof_ogl_4.AccessibleDescription");
      this.combo_extra_prof_ogl_4.AccessibleName = resources.GetString("combo_extra_prof_ogl_4.AccessibleName");
      this.combo_extra_prof_ogl_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_ogl_4.Anchor")));
      this.combo_extra_prof_ogl_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_ogl_4.BackgroundImage")));
      this.combo_extra_prof_ogl_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_ogl_4.Dock")));
      this.combo_extra_prof_ogl_4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_ogl_4.Enabled = ((bool)(resources.GetObject("combo_extra_prof_ogl_4.Enabled")));
      this.combo_extra_prof_ogl_4.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_ogl_4.Font")));
      this.combo_extra_prof_ogl_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_ogl_4.ImeMode")));
      this.combo_extra_prof_ogl_4.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_ogl_4.IntegralHeight")));
      this.combo_extra_prof_ogl_4.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_ogl_4.ItemHeight")));
      this.combo_extra_prof_ogl_4.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_ogl_4.Location")));
      this.combo_extra_prof_ogl_4.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_ogl_4.MaxDropDownItems")));
      this.combo_extra_prof_ogl_4.MaxLength = ((int)(resources.GetObject("combo_extra_prof_ogl_4.MaxLength")));
      this.combo_extra_prof_ogl_4.Name = "combo_extra_prof_ogl_4";
      this.combo_extra_prof_ogl_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_ogl_4.RightToLeft")));
      this.combo_extra_prof_ogl_4.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_ogl_4.Size")));
      this.combo_extra_prof_ogl_4.TabIndex = ((int)(resources.GetObject("combo_extra_prof_ogl_4.TabIndex")));
      this.combo_extra_prof_ogl_4.Text = resources.GetString("combo_extra_prof_ogl_4.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_ogl_4, resources.GetString("combo_extra_prof_ogl_4.ToolTip"));
      this.combo_extra_prof_ogl_4.Visible = ((bool)(resources.GetObject("combo_extra_prof_ogl_4.Visible")));
      // 
      // combo_extra_prof_ogl_5
      // 
      this.combo_extra_prof_ogl_5.AccessibleDescription = resources.GetString("combo_extra_prof_ogl_5.AccessibleDescription");
      this.combo_extra_prof_ogl_5.AccessibleName = resources.GetString("combo_extra_prof_ogl_5.AccessibleName");
      this.combo_extra_prof_ogl_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_ogl_5.Anchor")));
      this.combo_extra_prof_ogl_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_ogl_5.BackgroundImage")));
      this.combo_extra_prof_ogl_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_ogl_5.Dock")));
      this.combo_extra_prof_ogl_5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_ogl_5.Enabled = ((bool)(resources.GetObject("combo_extra_prof_ogl_5.Enabled")));
      this.combo_extra_prof_ogl_5.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_ogl_5.Font")));
      this.combo_extra_prof_ogl_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_ogl_5.ImeMode")));
      this.combo_extra_prof_ogl_5.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_ogl_5.IntegralHeight")));
      this.combo_extra_prof_ogl_5.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_ogl_5.ItemHeight")));
      this.combo_extra_prof_ogl_5.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_ogl_5.Location")));
      this.combo_extra_prof_ogl_5.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_ogl_5.MaxDropDownItems")));
      this.combo_extra_prof_ogl_5.MaxLength = ((int)(resources.GetObject("combo_extra_prof_ogl_5.MaxLength")));
      this.combo_extra_prof_ogl_5.Name = "combo_extra_prof_ogl_5";
      this.combo_extra_prof_ogl_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_ogl_5.RightToLeft")));
      this.combo_extra_prof_ogl_5.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_ogl_5.Size")));
      this.combo_extra_prof_ogl_5.TabIndex = ((int)(resources.GetObject("combo_extra_prof_ogl_5.TabIndex")));
      this.combo_extra_prof_ogl_5.Text = resources.GetString("combo_extra_prof_ogl_5.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_ogl_5, resources.GetString("combo_extra_prof_ogl_5.ToolTip"));
      this.combo_extra_prof_ogl_5.Visible = ((bool)(resources.GetObject("combo_extra_prof_ogl_5.Visible")));
      // 
      // combo_extra_prof_ogl_6
      // 
      this.combo_extra_prof_ogl_6.AccessibleDescription = resources.GetString("combo_extra_prof_ogl_6.AccessibleDescription");
      this.combo_extra_prof_ogl_6.AccessibleName = resources.GetString("combo_extra_prof_ogl_6.AccessibleName");
      this.combo_extra_prof_ogl_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_ogl_6.Anchor")));
      this.combo_extra_prof_ogl_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_ogl_6.BackgroundImage")));
      this.combo_extra_prof_ogl_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_ogl_6.Dock")));
      this.combo_extra_prof_ogl_6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_ogl_6.Enabled = ((bool)(resources.GetObject("combo_extra_prof_ogl_6.Enabled")));
      this.combo_extra_prof_ogl_6.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_ogl_6.Font")));
      this.combo_extra_prof_ogl_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_ogl_6.ImeMode")));
      this.combo_extra_prof_ogl_6.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_ogl_6.IntegralHeight")));
      this.combo_extra_prof_ogl_6.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_ogl_6.ItemHeight")));
      this.combo_extra_prof_ogl_6.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_ogl_6.Location")));
      this.combo_extra_prof_ogl_6.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_ogl_6.MaxDropDownItems")));
      this.combo_extra_prof_ogl_6.MaxLength = ((int)(resources.GetObject("combo_extra_prof_ogl_6.MaxLength")));
      this.combo_extra_prof_ogl_6.Name = "combo_extra_prof_ogl_6";
      this.combo_extra_prof_ogl_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_ogl_6.RightToLeft")));
      this.combo_extra_prof_ogl_6.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_ogl_6.Size")));
      this.combo_extra_prof_ogl_6.TabIndex = ((int)(resources.GetObject("combo_extra_prof_ogl_6.TabIndex")));
      this.combo_extra_prof_ogl_6.Text = resources.GetString("combo_extra_prof_ogl_6.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_ogl_6, resources.GetString("combo_extra_prof_ogl_6.ToolTip"));
      this.combo_extra_prof_ogl_6.Visible = ((bool)(resources.GetObject("combo_extra_prof_ogl_6.Visible")));
      // 
      // combo_ogl_aniso_mode
      // 
      this.combo_ogl_aniso_mode.AccessibleDescription = resources.GetString("combo_ogl_aniso_mode.AccessibleDescription");
      this.combo_ogl_aniso_mode.AccessibleName = resources.GetString("combo_ogl_aniso_mode.AccessibleName");
      this.combo_ogl_aniso_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_ogl_aniso_mode.Anchor")));
      this.combo_ogl_aniso_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_ogl_aniso_mode.BackgroundImage")));
      this.combo_ogl_aniso_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_ogl_aniso_mode.Dock")));
      this.combo_ogl_aniso_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_ogl_aniso_mode.Enabled = ((bool)(resources.GetObject("combo_ogl_aniso_mode.Enabled")));
      this.combo_ogl_aniso_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_ogl_aniso_mode.Font")));
      this.combo_ogl_aniso_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_ogl_aniso_mode.ImeMode")));
      this.combo_ogl_aniso_mode.IntegralHeight = ((bool)(resources.GetObject("combo_ogl_aniso_mode.IntegralHeight")));
      this.combo_ogl_aniso_mode.ItemHeight = ((int)(resources.GetObject("combo_ogl_aniso_mode.ItemHeight")));
      this.combo_ogl_aniso_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_ogl_aniso_mode.Location")));
      this.combo_ogl_aniso_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_ogl_aniso_mode.MaxDropDownItems")));
      this.combo_ogl_aniso_mode.MaxLength = ((int)(resources.GetObject("combo_ogl_aniso_mode.MaxLength")));
      this.combo_ogl_aniso_mode.Name = "combo_ogl_aniso_mode";
      this.combo_ogl_aniso_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_ogl_aniso_mode.RightToLeft")));
      this.combo_ogl_aniso_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_ogl_aniso_mode.Size")));
      this.combo_ogl_aniso_mode.TabIndex = ((int)(resources.GetObject("combo_ogl_aniso_mode.TabIndex")));
      this.combo_ogl_aniso_mode.Text = resources.GetString("combo_ogl_aniso_mode.Text");
      this.toolTip.SetToolTip(this.combo_ogl_aniso_mode, resources.GetString("combo_ogl_aniso_mode.ToolTip"));
      this.combo_ogl_aniso_mode.Visible = ((bool)(resources.GetObject("combo_ogl_aniso_mode.Visible")));
      // 
      // combo_ogl_fsaa_mode
      // 
      this.combo_ogl_fsaa_mode.AccessibleDescription = resources.GetString("combo_ogl_fsaa_mode.AccessibleDescription");
      this.combo_ogl_fsaa_mode.AccessibleName = resources.GetString("combo_ogl_fsaa_mode.AccessibleName");
      this.combo_ogl_fsaa_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_ogl_fsaa_mode.Anchor")));
      this.combo_ogl_fsaa_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_ogl_fsaa_mode.BackgroundImage")));
      this.combo_ogl_fsaa_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_ogl_fsaa_mode.Dock")));
      this.combo_ogl_fsaa_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_ogl_fsaa_mode.Enabled = ((bool)(resources.GetObject("combo_ogl_fsaa_mode.Enabled")));
      this.combo_ogl_fsaa_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_ogl_fsaa_mode.Font")));
      this.combo_ogl_fsaa_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_ogl_fsaa_mode.ImeMode")));
      this.combo_ogl_fsaa_mode.IntegralHeight = ((bool)(resources.GetObject("combo_ogl_fsaa_mode.IntegralHeight")));
      this.combo_ogl_fsaa_mode.ItemHeight = ((int)(resources.GetObject("combo_ogl_fsaa_mode.ItemHeight")));
      this.combo_ogl_fsaa_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_ogl_fsaa_mode.Location")));
      this.combo_ogl_fsaa_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_ogl_fsaa_mode.MaxDropDownItems")));
      this.combo_ogl_fsaa_mode.MaxLength = ((int)(resources.GetObject("combo_ogl_fsaa_mode.MaxLength")));
      this.combo_ogl_fsaa_mode.Name = "combo_ogl_fsaa_mode";
      this.combo_ogl_fsaa_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_ogl_fsaa_mode.RightToLeft")));
      this.combo_ogl_fsaa_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_ogl_fsaa_mode.Size")));
      this.combo_ogl_fsaa_mode.TabIndex = ((int)(resources.GetObject("combo_ogl_fsaa_mode.TabIndex")));
      this.combo_ogl_fsaa_mode.Text = resources.GetString("combo_ogl_fsaa_mode.Text");
      this.toolTip.SetToolTip(this.combo_ogl_fsaa_mode, resources.GetString("combo_ogl_fsaa_mode.ToolTip"));
      this.combo_ogl_fsaa_mode.Visible = ((bool)(resources.GetObject("combo_ogl_fsaa_mode.Visible")));
      // 
      // combo_ogl_lod_bias
      // 
      this.combo_ogl_lod_bias.AccessibleDescription = resources.GetString("combo_ogl_lod_bias.AccessibleDescription");
      this.combo_ogl_lod_bias.AccessibleName = resources.GetString("combo_ogl_lod_bias.AccessibleName");
      this.combo_ogl_lod_bias.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_ogl_lod_bias.Anchor")));
      this.combo_ogl_lod_bias.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_ogl_lod_bias.BackgroundImage")));
      this.combo_ogl_lod_bias.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_ogl_lod_bias.Dock")));
      this.combo_ogl_lod_bias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_ogl_lod_bias.Enabled = ((bool)(resources.GetObject("combo_ogl_lod_bias.Enabled")));
      this.combo_ogl_lod_bias.Font = ((System.Drawing.Font)(resources.GetObject("combo_ogl_lod_bias.Font")));
      this.combo_ogl_lod_bias.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_ogl_lod_bias.ImeMode")));
      this.combo_ogl_lod_bias.IntegralHeight = ((bool)(resources.GetObject("combo_ogl_lod_bias.IntegralHeight")));
      this.combo_ogl_lod_bias.ItemHeight = ((int)(resources.GetObject("combo_ogl_lod_bias.ItemHeight")));
      this.combo_ogl_lod_bias.Location = ((System.Drawing.Point)(resources.GetObject("combo_ogl_lod_bias.Location")));
      this.combo_ogl_lod_bias.MaxDropDownItems = ((int)(resources.GetObject("combo_ogl_lod_bias.MaxDropDownItems")));
      this.combo_ogl_lod_bias.MaxLength = ((int)(resources.GetObject("combo_ogl_lod_bias.MaxLength")));
      this.combo_ogl_lod_bias.Name = "combo_ogl_lod_bias";
      this.combo_ogl_lod_bias.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_ogl_lod_bias.RightToLeft")));
      this.combo_ogl_lod_bias.Size = ((System.Drawing.Size)(resources.GetObject("combo_ogl_lod_bias.Size")));
      this.combo_ogl_lod_bias.TabIndex = ((int)(resources.GetObject("combo_ogl_lod_bias.TabIndex")));
      this.combo_ogl_lod_bias.Text = resources.GetString("combo_ogl_lod_bias.Text");
      this.toolTip.SetToolTip(this.combo_ogl_lod_bias, resources.GetString("combo_ogl_lod_bias.ToolTip"));
      this.combo_ogl_lod_bias.Visible = ((bool)(resources.GetObject("combo_ogl_lod_bias.Visible")));
      // 
      // combo_ogl_prerender_frames
      // 
      this.combo_ogl_prerender_frames.AccessibleDescription = resources.GetString("combo_ogl_prerender_frames.AccessibleDescription");
      this.combo_ogl_prerender_frames.AccessibleName = resources.GetString("combo_ogl_prerender_frames.AccessibleName");
      this.combo_ogl_prerender_frames.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_ogl_prerender_frames.Anchor")));
      this.combo_ogl_prerender_frames.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_ogl_prerender_frames.BackgroundImage")));
      this.combo_ogl_prerender_frames.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_ogl_prerender_frames.Dock")));
      this.combo_ogl_prerender_frames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_ogl_prerender_frames.Enabled = ((bool)(resources.GetObject("combo_ogl_prerender_frames.Enabled")));
      this.combo_ogl_prerender_frames.Font = ((System.Drawing.Font)(resources.GetObject("combo_ogl_prerender_frames.Font")));
      this.combo_ogl_prerender_frames.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_ogl_prerender_frames.ImeMode")));
      this.combo_ogl_prerender_frames.IntegralHeight = ((bool)(resources.GetObject("combo_ogl_prerender_frames.IntegralHeight")));
      this.combo_ogl_prerender_frames.ItemHeight = ((int)(resources.GetObject("combo_ogl_prerender_frames.ItemHeight")));
      this.combo_ogl_prerender_frames.Location = ((System.Drawing.Point)(resources.GetObject("combo_ogl_prerender_frames.Location")));
      this.combo_ogl_prerender_frames.MaxDropDownItems = ((int)(resources.GetObject("combo_ogl_prerender_frames.MaxDropDownItems")));
      this.combo_ogl_prerender_frames.MaxLength = ((int)(resources.GetObject("combo_ogl_prerender_frames.MaxLength")));
      this.combo_ogl_prerender_frames.Name = "combo_ogl_prerender_frames";
      this.combo_ogl_prerender_frames.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_ogl_prerender_frames.RightToLeft")));
      this.combo_ogl_prerender_frames.Size = ((System.Drawing.Size)(resources.GetObject("combo_ogl_prerender_frames.Size")));
      this.combo_ogl_prerender_frames.TabIndex = ((int)(resources.GetObject("combo_ogl_prerender_frames.TabIndex")));
      this.combo_ogl_prerender_frames.Text = resources.GetString("combo_ogl_prerender_frames.Text");
      this.toolTip.SetToolTip(this.combo_ogl_prerender_frames, resources.GetString("combo_ogl_prerender_frames.ToolTip"));
      this.combo_ogl_prerender_frames.Visible = ((bool)(resources.GetObject("combo_ogl_prerender_frames.Visible")));
      // 
      // combo_ogl_qe_mode
      // 
      this.combo_ogl_qe_mode.AccessibleDescription = resources.GetString("combo_ogl_qe_mode.AccessibleDescription");
      this.combo_ogl_qe_mode.AccessibleName = resources.GetString("combo_ogl_qe_mode.AccessibleName");
      this.combo_ogl_qe_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_ogl_qe_mode.Anchor")));
      this.combo_ogl_qe_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_ogl_qe_mode.BackgroundImage")));
      this.combo_ogl_qe_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_ogl_qe_mode.Dock")));
      this.combo_ogl_qe_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_ogl_qe_mode.Enabled = ((bool)(resources.GetObject("combo_ogl_qe_mode.Enabled")));
      this.combo_ogl_qe_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_ogl_qe_mode.Font")));
      this.combo_ogl_qe_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_ogl_qe_mode.ImeMode")));
      this.combo_ogl_qe_mode.IntegralHeight = ((bool)(resources.GetObject("combo_ogl_qe_mode.IntegralHeight")));
      this.combo_ogl_qe_mode.ItemHeight = ((int)(resources.GetObject("combo_ogl_qe_mode.ItemHeight")));
      this.combo_ogl_qe_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_ogl_qe_mode.Location")));
      this.combo_ogl_qe_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_ogl_qe_mode.MaxDropDownItems")));
      this.combo_ogl_qe_mode.MaxLength = ((int)(resources.GetObject("combo_ogl_qe_mode.MaxLength")));
      this.combo_ogl_qe_mode.Name = "combo_ogl_qe_mode";
      this.combo_ogl_qe_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_ogl_qe_mode.RightToLeft")));
      this.combo_ogl_qe_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_ogl_qe_mode.Size")));
      this.combo_ogl_qe_mode.TabIndex = ((int)(resources.GetObject("combo_ogl_qe_mode.TabIndex")));
      this.combo_ogl_qe_mode.Text = resources.GetString("combo_ogl_qe_mode.Text");
      this.toolTip.SetToolTip(this.combo_ogl_qe_mode, resources.GetString("combo_ogl_qe_mode.ToolTip"));
      this.combo_ogl_qe_mode.Visible = ((bool)(resources.GetObject("combo_ogl_qe_mode.Visible")));
      // 
      // combo_ogl_vsync_mode
      // 
      this.combo_ogl_vsync_mode.AccessibleDescription = resources.GetString("combo_ogl_vsync_mode.AccessibleDescription");
      this.combo_ogl_vsync_mode.AccessibleName = resources.GetString("combo_ogl_vsync_mode.AccessibleName");
      this.combo_ogl_vsync_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_ogl_vsync_mode.Anchor")));
      this.combo_ogl_vsync_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_ogl_vsync_mode.BackgroundImage")));
      this.combo_ogl_vsync_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_ogl_vsync_mode.Dock")));
      this.combo_ogl_vsync_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_ogl_vsync_mode.Enabled = ((bool)(resources.GetObject("combo_ogl_vsync_mode.Enabled")));
      this.combo_ogl_vsync_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_ogl_vsync_mode.Font")));
      this.combo_ogl_vsync_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_ogl_vsync_mode.ImeMode")));
      this.combo_ogl_vsync_mode.IntegralHeight = ((bool)(resources.GetObject("combo_ogl_vsync_mode.IntegralHeight")));
      this.combo_ogl_vsync_mode.ItemHeight = ((int)(resources.GetObject("combo_ogl_vsync_mode.ItemHeight")));
      this.combo_ogl_vsync_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_ogl_vsync_mode.Location")));
      this.combo_ogl_vsync_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_ogl_vsync_mode.MaxDropDownItems")));
      this.combo_ogl_vsync_mode.MaxLength = ((int)(resources.GetObject("combo_ogl_vsync_mode.MaxLength")));
      this.combo_ogl_vsync_mode.Name = "combo_ogl_vsync_mode";
      this.combo_ogl_vsync_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_ogl_vsync_mode.RightToLeft")));
      this.combo_ogl_vsync_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_ogl_vsync_mode.Size")));
      this.combo_ogl_vsync_mode.TabIndex = ((int)(resources.GetObject("combo_ogl_vsync_mode.TabIndex")));
      this.combo_ogl_vsync_mode.Text = resources.GetString("combo_ogl_vsync_mode.Text");
      this.toolTip.SetToolTip(this.combo_ogl_vsync_mode, resources.GetString("combo_ogl_vsync_mode.ToolTip"));
      this.combo_ogl_vsync_mode.Visible = ((bool)(resources.GetObject("combo_ogl_vsync_mode.Visible")));
      // 
      // combo_prof_d3d_aniso_mode
      // 
      this.combo_prof_d3d_aniso_mode.AccessibleDescription = resources.GetString("combo_prof_d3d_aniso_mode.AccessibleDescription");
      this.combo_prof_d3d_aniso_mode.AccessibleName = resources.GetString("combo_prof_d3d_aniso_mode.AccessibleName");
      this.combo_prof_d3d_aniso_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_d3d_aniso_mode.Anchor")));
      this.combo_prof_d3d_aniso_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_d3d_aniso_mode.BackgroundImage")));
      this.combo_prof_d3d_aniso_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_d3d_aniso_mode.Dock")));
      this.combo_prof_d3d_aniso_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_d3d_aniso_mode.Enabled = ((bool)(resources.GetObject("combo_prof_d3d_aniso_mode.Enabled")));
      this.combo_prof_d3d_aniso_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_d3d_aniso_mode.Font")));
      this.combo_prof_d3d_aniso_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_d3d_aniso_mode.ImeMode")));
      this.combo_prof_d3d_aniso_mode.IntegralHeight = ((bool)(resources.GetObject("combo_prof_d3d_aniso_mode.IntegralHeight")));
      this.combo_prof_d3d_aniso_mode.ItemHeight = ((int)(resources.GetObject("combo_prof_d3d_aniso_mode.ItemHeight")));
      this.combo_prof_d3d_aniso_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_d3d_aniso_mode.Location")));
      this.combo_prof_d3d_aniso_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_d3d_aniso_mode.MaxDropDownItems")));
      this.combo_prof_d3d_aniso_mode.MaxLength = ((int)(resources.GetObject("combo_prof_d3d_aniso_mode.MaxLength")));
      this.combo_prof_d3d_aniso_mode.Name = "combo_prof_d3d_aniso_mode";
      this.combo_prof_d3d_aniso_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_d3d_aniso_mode.RightToLeft")));
      this.combo_prof_d3d_aniso_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_d3d_aniso_mode.Size")));
      this.combo_prof_d3d_aniso_mode.TabIndex = ((int)(resources.GetObject("combo_prof_d3d_aniso_mode.TabIndex")));
      this.combo_prof_d3d_aniso_mode.Text = resources.GetString("combo_prof_d3d_aniso_mode.Text");
      this.toolTip.SetToolTip(this.combo_prof_d3d_aniso_mode, resources.GetString("combo_prof_d3d_aniso_mode.ToolTip"));
      this.combo_prof_d3d_aniso_mode.Visible = ((bool)(resources.GetObject("combo_prof_d3d_aniso_mode.Visible")));
      // 
      // combo_prof_d3d_fsaa_mode
      // 
      this.combo_prof_d3d_fsaa_mode.AccessibleDescription = resources.GetString("combo_prof_d3d_fsaa_mode.AccessibleDescription");
      this.combo_prof_d3d_fsaa_mode.AccessibleName = resources.GetString("combo_prof_d3d_fsaa_mode.AccessibleName");
      this.combo_prof_d3d_fsaa_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_d3d_fsaa_mode.Anchor")));
      this.combo_prof_d3d_fsaa_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_d3d_fsaa_mode.BackgroundImage")));
      this.combo_prof_d3d_fsaa_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_d3d_fsaa_mode.Dock")));
      this.combo_prof_d3d_fsaa_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_d3d_fsaa_mode.Enabled = ((bool)(resources.GetObject("combo_prof_d3d_fsaa_mode.Enabled")));
      this.combo_prof_d3d_fsaa_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_d3d_fsaa_mode.Font")));
      this.combo_prof_d3d_fsaa_mode.ForeColor = System.Drawing.SystemColors.WindowText;
      this.combo_prof_d3d_fsaa_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_d3d_fsaa_mode.ImeMode")));
      this.combo_prof_d3d_fsaa_mode.IntegralHeight = ((bool)(resources.GetObject("combo_prof_d3d_fsaa_mode.IntegralHeight")));
      this.combo_prof_d3d_fsaa_mode.ItemHeight = ((int)(resources.GetObject("combo_prof_d3d_fsaa_mode.ItemHeight")));
      this.combo_prof_d3d_fsaa_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_d3d_fsaa_mode.Location")));
      this.combo_prof_d3d_fsaa_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_d3d_fsaa_mode.MaxDropDownItems")));
      this.combo_prof_d3d_fsaa_mode.MaxLength = ((int)(resources.GetObject("combo_prof_d3d_fsaa_mode.MaxLength")));
      this.combo_prof_d3d_fsaa_mode.Name = "combo_prof_d3d_fsaa_mode";
      this.combo_prof_d3d_fsaa_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_d3d_fsaa_mode.RightToLeft")));
      this.combo_prof_d3d_fsaa_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_d3d_fsaa_mode.Size")));
      this.combo_prof_d3d_fsaa_mode.TabIndex = ((int)(resources.GetObject("combo_prof_d3d_fsaa_mode.TabIndex")));
      this.combo_prof_d3d_fsaa_mode.Text = resources.GetString("combo_prof_d3d_fsaa_mode.Text");
      this.toolTip.SetToolTip(this.combo_prof_d3d_fsaa_mode, resources.GetString("combo_prof_d3d_fsaa_mode.ToolTip"));
      this.combo_prof_d3d_fsaa_mode.Visible = ((bool)(resources.GetObject("combo_prof_d3d_fsaa_mode.Visible")));
      // 
      // combo_prof_d3d_lod_bias
      // 
      this.combo_prof_d3d_lod_bias.AccessibleDescription = resources.GetString("combo_prof_d3d_lod_bias.AccessibleDescription");
      this.combo_prof_d3d_lod_bias.AccessibleName = resources.GetString("combo_prof_d3d_lod_bias.AccessibleName");
      this.combo_prof_d3d_lod_bias.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_d3d_lod_bias.Anchor")));
      this.combo_prof_d3d_lod_bias.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_d3d_lod_bias.BackgroundImage")));
      this.combo_prof_d3d_lod_bias.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_d3d_lod_bias.Dock")));
      this.combo_prof_d3d_lod_bias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_d3d_lod_bias.Enabled = ((bool)(resources.GetObject("combo_prof_d3d_lod_bias.Enabled")));
      this.combo_prof_d3d_lod_bias.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_d3d_lod_bias.Font")));
      this.combo_prof_d3d_lod_bias.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_d3d_lod_bias.ImeMode")));
      this.combo_prof_d3d_lod_bias.IntegralHeight = ((bool)(resources.GetObject("combo_prof_d3d_lod_bias.IntegralHeight")));
      this.combo_prof_d3d_lod_bias.ItemHeight = ((int)(resources.GetObject("combo_prof_d3d_lod_bias.ItemHeight")));
      this.combo_prof_d3d_lod_bias.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_d3d_lod_bias.Location")));
      this.combo_prof_d3d_lod_bias.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_d3d_lod_bias.MaxDropDownItems")));
      this.combo_prof_d3d_lod_bias.MaxLength = ((int)(resources.GetObject("combo_prof_d3d_lod_bias.MaxLength")));
      this.combo_prof_d3d_lod_bias.Name = "combo_prof_d3d_lod_bias";
      this.combo_prof_d3d_lod_bias.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_d3d_lod_bias.RightToLeft")));
      this.combo_prof_d3d_lod_bias.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_d3d_lod_bias.Size")));
      this.combo_prof_d3d_lod_bias.TabIndex = ((int)(resources.GetObject("combo_prof_d3d_lod_bias.TabIndex")));
      this.combo_prof_d3d_lod_bias.Text = resources.GetString("combo_prof_d3d_lod_bias.Text");
      this.toolTip.SetToolTip(this.combo_prof_d3d_lod_bias, resources.GetString("combo_prof_d3d_lod_bias.ToolTip"));
      this.combo_prof_d3d_lod_bias.Visible = ((bool)(resources.GetObject("combo_prof_d3d_lod_bias.Visible")));
      // 
      // combo_prof_d3d_prerender_frames
      // 
      this.combo_prof_d3d_prerender_frames.AccessibleDescription = resources.GetString("combo_prof_d3d_prerender_frames.AccessibleDescription");
      this.combo_prof_d3d_prerender_frames.AccessibleName = resources.GetString("combo_prof_d3d_prerender_frames.AccessibleName");
      this.combo_prof_d3d_prerender_frames.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_d3d_prerender_frames.Anchor")));
      this.combo_prof_d3d_prerender_frames.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_d3d_prerender_frames.BackgroundImage")));
      this.combo_prof_d3d_prerender_frames.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_d3d_prerender_frames.Dock")));
      this.combo_prof_d3d_prerender_frames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_d3d_prerender_frames.Enabled = ((bool)(resources.GetObject("combo_prof_d3d_prerender_frames.Enabled")));
      this.combo_prof_d3d_prerender_frames.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_d3d_prerender_frames.Font")));
      this.combo_prof_d3d_prerender_frames.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_d3d_prerender_frames.ImeMode")));
      this.combo_prof_d3d_prerender_frames.IntegralHeight = ((bool)(resources.GetObject("combo_prof_d3d_prerender_frames.IntegralHeight")));
      this.combo_prof_d3d_prerender_frames.ItemHeight = ((int)(resources.GetObject("combo_prof_d3d_prerender_frames.ItemHeight")));
      this.combo_prof_d3d_prerender_frames.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_d3d_prerender_frames.Location")));
      this.combo_prof_d3d_prerender_frames.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_d3d_prerender_frames.MaxDropDownItems")));
      this.combo_prof_d3d_prerender_frames.MaxLength = ((int)(resources.GetObject("combo_prof_d3d_prerender_frames.MaxLength")));
      this.combo_prof_d3d_prerender_frames.Name = "combo_prof_d3d_prerender_frames";
      this.combo_prof_d3d_prerender_frames.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_d3d_prerender_frames.RightToLeft")));
      this.combo_prof_d3d_prerender_frames.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_d3d_prerender_frames.Size")));
      this.combo_prof_d3d_prerender_frames.TabIndex = ((int)(resources.GetObject("combo_prof_d3d_prerender_frames.TabIndex")));
      this.combo_prof_d3d_prerender_frames.Text = resources.GetString("combo_prof_d3d_prerender_frames.Text");
      this.toolTip.SetToolTip(this.combo_prof_d3d_prerender_frames, resources.GetString("combo_prof_d3d_prerender_frames.ToolTip"));
      this.combo_prof_d3d_prerender_frames.Visible = ((bool)(resources.GetObject("combo_prof_d3d_prerender_frames.Visible")));
      // 
      // combo_prof_d3d_qe_mode
      // 
      this.combo_prof_d3d_qe_mode.AccessibleDescription = resources.GetString("combo_prof_d3d_qe_mode.AccessibleDescription");
      this.combo_prof_d3d_qe_mode.AccessibleName = resources.GetString("combo_prof_d3d_qe_mode.AccessibleName");
      this.combo_prof_d3d_qe_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_d3d_qe_mode.Anchor")));
      this.combo_prof_d3d_qe_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_d3d_qe_mode.BackgroundImage")));
      this.combo_prof_d3d_qe_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_d3d_qe_mode.Dock")));
      this.combo_prof_d3d_qe_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_d3d_qe_mode.Enabled = ((bool)(resources.GetObject("combo_prof_d3d_qe_mode.Enabled")));
      this.combo_prof_d3d_qe_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_d3d_qe_mode.Font")));
      this.combo_prof_d3d_qe_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_d3d_qe_mode.ImeMode")));
      this.combo_prof_d3d_qe_mode.IntegralHeight = ((bool)(resources.GetObject("combo_prof_d3d_qe_mode.IntegralHeight")));
      this.combo_prof_d3d_qe_mode.ItemHeight = ((int)(resources.GetObject("combo_prof_d3d_qe_mode.ItemHeight")));
      this.combo_prof_d3d_qe_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_d3d_qe_mode.Location")));
      this.combo_prof_d3d_qe_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_d3d_qe_mode.MaxDropDownItems")));
      this.combo_prof_d3d_qe_mode.MaxLength = ((int)(resources.GetObject("combo_prof_d3d_qe_mode.MaxLength")));
      this.combo_prof_d3d_qe_mode.Name = "combo_prof_d3d_qe_mode";
      this.combo_prof_d3d_qe_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_d3d_qe_mode.RightToLeft")));
      this.combo_prof_d3d_qe_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_d3d_qe_mode.Size")));
      this.combo_prof_d3d_qe_mode.TabIndex = ((int)(resources.GetObject("combo_prof_d3d_qe_mode.TabIndex")));
      this.combo_prof_d3d_qe_mode.Text = resources.GetString("combo_prof_d3d_qe_mode.Text");
      this.toolTip.SetToolTip(this.combo_prof_d3d_qe_mode, resources.GetString("combo_prof_d3d_qe_mode.ToolTip"));
      this.combo_prof_d3d_qe_mode.Visible = ((bool)(resources.GetObject("combo_prof_d3d_qe_mode.Visible")));
      // 
      // combo_prof_d3d_vsync_mode
      // 
      this.combo_prof_d3d_vsync_mode.AccessibleDescription = resources.GetString("combo_prof_d3d_vsync_mode.AccessibleDescription");
      this.combo_prof_d3d_vsync_mode.AccessibleName = resources.GetString("combo_prof_d3d_vsync_mode.AccessibleName");
      this.combo_prof_d3d_vsync_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_d3d_vsync_mode.Anchor")));
      this.combo_prof_d3d_vsync_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_d3d_vsync_mode.BackgroundImage")));
      this.combo_prof_d3d_vsync_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_d3d_vsync_mode.Dock")));
      this.combo_prof_d3d_vsync_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_d3d_vsync_mode.Enabled = ((bool)(resources.GetObject("combo_prof_d3d_vsync_mode.Enabled")));
      this.combo_prof_d3d_vsync_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_d3d_vsync_mode.Font")));
      this.combo_prof_d3d_vsync_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_d3d_vsync_mode.ImeMode")));
      this.combo_prof_d3d_vsync_mode.IntegralHeight = ((bool)(resources.GetObject("combo_prof_d3d_vsync_mode.IntegralHeight")));
      this.combo_prof_d3d_vsync_mode.ItemHeight = ((int)(resources.GetObject("combo_prof_d3d_vsync_mode.ItemHeight")));
      this.combo_prof_d3d_vsync_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_d3d_vsync_mode.Location")));
      this.combo_prof_d3d_vsync_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_d3d_vsync_mode.MaxDropDownItems")));
      this.combo_prof_d3d_vsync_mode.MaxLength = ((int)(resources.GetObject("combo_prof_d3d_vsync_mode.MaxLength")));
      this.combo_prof_d3d_vsync_mode.Name = "combo_prof_d3d_vsync_mode";
      this.combo_prof_d3d_vsync_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_d3d_vsync_mode.RightToLeft")));
      this.combo_prof_d3d_vsync_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_d3d_vsync_mode.Size")));
      this.combo_prof_d3d_vsync_mode.TabIndex = ((int)(resources.GetObject("combo_prof_d3d_vsync_mode.TabIndex")));
      this.combo_prof_d3d_vsync_mode.Text = resources.GetString("combo_prof_d3d_vsync_mode.Text");
      this.toolTip.SetToolTip(this.combo_prof_d3d_vsync_mode, resources.GetString("combo_prof_d3d_vsync_mode.ToolTip"));
      this.combo_prof_d3d_vsync_mode.Visible = ((bool)(resources.GetObject("combo_prof_d3d_vsync_mode.Visible")));
      // 
      // combo_prof_names
      // 
      this.combo_prof_names.AccessibleDescription = resources.GetString("combo_prof_names.AccessibleDescription");
      this.combo_prof_names.AccessibleName = resources.GetString("combo_prof_names.AccessibleName");
      this.combo_prof_names.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_names.Anchor")));
      this.combo_prof_names.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_names.BackgroundImage")));
      this.combo_prof_names.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_names.Dock")));
      this.combo_prof_names.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_names.DropDownWidth = 224;
      this.combo_prof_names.Enabled = ((bool)(resources.GetObject("combo_prof_names.Enabled")));
      this.combo_prof_names.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_names.Font")));
      this.combo_prof_names.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_names.ImeMode")));
      this.combo_prof_names.IntegralHeight = ((bool)(resources.GetObject("combo_prof_names.IntegralHeight")));
      this.combo_prof_names.ItemHeight = ((int)(resources.GetObject("combo_prof_names.ItemHeight")));
      this.combo_prof_names.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_names.Location")));
      this.combo_prof_names.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_names.MaxDropDownItems")));
      this.combo_prof_names.MaxLength = ((int)(resources.GetObject("combo_prof_names.MaxLength")));
      this.combo_prof_names.Name = "combo_prof_names";
      this.combo_prof_names.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_names.RightToLeft")));
      this.combo_prof_names.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_names.Size")));
      this.combo_prof_names.TabIndex = ((int)(resources.GetObject("combo_prof_names.TabIndex")));
      this.combo_prof_names.Text = resources.GetString("combo_prof_names.Text");
      this.toolTip.SetToolTip(this.combo_prof_names, resources.GetString("combo_prof_names.ToolTip"));
      this.combo_prof_names.Visible = ((bool)(resources.GetObject("combo_prof_names.Visible")));
      this.combo_prof_names.KeyDown += new System.Windows.Forms.KeyEventHandler(this.combo_prof_names_KeyDown);
      this.combo_prof_names.TextChanged += new System.EventHandler(this.combo_prof_names_TextChanged);
      this.combo_prof_names.SelectedIndexChanged += new System.EventHandler(this.combo_prof_names_SelectedIndexChanged);
      // 
      // combo_prof_ogl_aniso_mode
      // 
      this.combo_prof_ogl_aniso_mode.AccessibleDescription = resources.GetString("combo_prof_ogl_aniso_mode.AccessibleDescription");
      this.combo_prof_ogl_aniso_mode.AccessibleName = resources.GetString("combo_prof_ogl_aniso_mode.AccessibleName");
      this.combo_prof_ogl_aniso_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_ogl_aniso_mode.Anchor")));
      this.combo_prof_ogl_aniso_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_ogl_aniso_mode.BackgroundImage")));
      this.combo_prof_ogl_aniso_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_ogl_aniso_mode.Dock")));
      this.combo_prof_ogl_aniso_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_ogl_aniso_mode.Enabled = ((bool)(resources.GetObject("combo_prof_ogl_aniso_mode.Enabled")));
      this.combo_prof_ogl_aniso_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_ogl_aniso_mode.Font")));
      this.combo_prof_ogl_aniso_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_ogl_aniso_mode.ImeMode")));
      this.combo_prof_ogl_aniso_mode.IntegralHeight = ((bool)(resources.GetObject("combo_prof_ogl_aniso_mode.IntegralHeight")));
      this.combo_prof_ogl_aniso_mode.ItemHeight = ((int)(resources.GetObject("combo_prof_ogl_aniso_mode.ItemHeight")));
      this.combo_prof_ogl_aniso_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_ogl_aniso_mode.Location")));
      this.combo_prof_ogl_aniso_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_ogl_aniso_mode.MaxDropDownItems")));
      this.combo_prof_ogl_aniso_mode.MaxLength = ((int)(resources.GetObject("combo_prof_ogl_aniso_mode.MaxLength")));
      this.combo_prof_ogl_aniso_mode.Name = "combo_prof_ogl_aniso_mode";
      this.combo_prof_ogl_aniso_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_ogl_aniso_mode.RightToLeft")));
      this.combo_prof_ogl_aniso_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_ogl_aniso_mode.Size")));
      this.combo_prof_ogl_aniso_mode.TabIndex = ((int)(resources.GetObject("combo_prof_ogl_aniso_mode.TabIndex")));
      this.combo_prof_ogl_aniso_mode.Text = resources.GetString("combo_prof_ogl_aniso_mode.Text");
      this.toolTip.SetToolTip(this.combo_prof_ogl_aniso_mode, resources.GetString("combo_prof_ogl_aniso_mode.ToolTip"));
      this.combo_prof_ogl_aniso_mode.Visible = ((bool)(resources.GetObject("combo_prof_ogl_aniso_mode.Visible")));
      // 
      // combo_prof_ogl_fsaa_mode
      // 
      this.combo_prof_ogl_fsaa_mode.AccessibleDescription = resources.GetString("combo_prof_ogl_fsaa_mode.AccessibleDescription");
      this.combo_prof_ogl_fsaa_mode.AccessibleName = resources.GetString("combo_prof_ogl_fsaa_mode.AccessibleName");
      this.combo_prof_ogl_fsaa_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_ogl_fsaa_mode.Anchor")));
      this.combo_prof_ogl_fsaa_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_ogl_fsaa_mode.BackgroundImage")));
      this.combo_prof_ogl_fsaa_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_ogl_fsaa_mode.Dock")));
      this.combo_prof_ogl_fsaa_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_ogl_fsaa_mode.Enabled = ((bool)(resources.GetObject("combo_prof_ogl_fsaa_mode.Enabled")));
      this.combo_prof_ogl_fsaa_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_ogl_fsaa_mode.Font")));
      this.combo_prof_ogl_fsaa_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_ogl_fsaa_mode.ImeMode")));
      this.combo_prof_ogl_fsaa_mode.IntegralHeight = ((bool)(resources.GetObject("combo_prof_ogl_fsaa_mode.IntegralHeight")));
      this.combo_prof_ogl_fsaa_mode.ItemHeight = ((int)(resources.GetObject("combo_prof_ogl_fsaa_mode.ItemHeight")));
      this.combo_prof_ogl_fsaa_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_ogl_fsaa_mode.Location")));
      this.combo_prof_ogl_fsaa_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_ogl_fsaa_mode.MaxDropDownItems")));
      this.combo_prof_ogl_fsaa_mode.MaxLength = ((int)(resources.GetObject("combo_prof_ogl_fsaa_mode.MaxLength")));
      this.combo_prof_ogl_fsaa_mode.Name = "combo_prof_ogl_fsaa_mode";
      this.combo_prof_ogl_fsaa_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_ogl_fsaa_mode.RightToLeft")));
      this.combo_prof_ogl_fsaa_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_ogl_fsaa_mode.Size")));
      this.combo_prof_ogl_fsaa_mode.TabIndex = ((int)(resources.GetObject("combo_prof_ogl_fsaa_mode.TabIndex")));
      this.combo_prof_ogl_fsaa_mode.Text = resources.GetString("combo_prof_ogl_fsaa_mode.Text");
      this.toolTip.SetToolTip(this.combo_prof_ogl_fsaa_mode, resources.GetString("combo_prof_ogl_fsaa_mode.ToolTip"));
      this.combo_prof_ogl_fsaa_mode.Visible = ((bool)(resources.GetObject("combo_prof_ogl_fsaa_mode.Visible")));
      // 
      // combo_prof_ogl_lod_bias
      // 
      this.combo_prof_ogl_lod_bias.AccessibleDescription = resources.GetString("combo_prof_ogl_lod_bias.AccessibleDescription");
      this.combo_prof_ogl_lod_bias.AccessibleName = resources.GetString("combo_prof_ogl_lod_bias.AccessibleName");
      this.combo_prof_ogl_lod_bias.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_ogl_lod_bias.Anchor")));
      this.combo_prof_ogl_lod_bias.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_ogl_lod_bias.BackgroundImage")));
      this.combo_prof_ogl_lod_bias.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_ogl_lod_bias.Dock")));
      this.combo_prof_ogl_lod_bias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_ogl_lod_bias.Enabled = ((bool)(resources.GetObject("combo_prof_ogl_lod_bias.Enabled")));
      this.combo_prof_ogl_lod_bias.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_ogl_lod_bias.Font")));
      this.combo_prof_ogl_lod_bias.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_ogl_lod_bias.ImeMode")));
      this.combo_prof_ogl_lod_bias.IntegralHeight = ((bool)(resources.GetObject("combo_prof_ogl_lod_bias.IntegralHeight")));
      this.combo_prof_ogl_lod_bias.ItemHeight = ((int)(resources.GetObject("combo_prof_ogl_lod_bias.ItemHeight")));
      this.combo_prof_ogl_lod_bias.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_ogl_lod_bias.Location")));
      this.combo_prof_ogl_lod_bias.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_ogl_lod_bias.MaxDropDownItems")));
      this.combo_prof_ogl_lod_bias.MaxLength = ((int)(resources.GetObject("combo_prof_ogl_lod_bias.MaxLength")));
      this.combo_prof_ogl_lod_bias.Name = "combo_prof_ogl_lod_bias";
      this.combo_prof_ogl_lod_bias.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_ogl_lod_bias.RightToLeft")));
      this.combo_prof_ogl_lod_bias.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_ogl_lod_bias.Size")));
      this.combo_prof_ogl_lod_bias.TabIndex = ((int)(resources.GetObject("combo_prof_ogl_lod_bias.TabIndex")));
      this.combo_prof_ogl_lod_bias.Text = resources.GetString("combo_prof_ogl_lod_bias.Text");
      this.toolTip.SetToolTip(this.combo_prof_ogl_lod_bias, resources.GetString("combo_prof_ogl_lod_bias.ToolTip"));
      this.combo_prof_ogl_lod_bias.Visible = ((bool)(resources.GetObject("combo_prof_ogl_lod_bias.Visible")));
      // 
      // combo_prof_ogl_prerender_frames
      // 
      this.combo_prof_ogl_prerender_frames.AccessibleDescription = resources.GetString("combo_prof_ogl_prerender_frames.AccessibleDescription");
      this.combo_prof_ogl_prerender_frames.AccessibleName = resources.GetString("combo_prof_ogl_prerender_frames.AccessibleName");
      this.combo_prof_ogl_prerender_frames.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_ogl_prerender_frames.Anchor")));
      this.combo_prof_ogl_prerender_frames.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_ogl_prerender_frames.BackgroundImage")));
      this.combo_prof_ogl_prerender_frames.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_ogl_prerender_frames.Dock")));
      this.combo_prof_ogl_prerender_frames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_ogl_prerender_frames.Enabled = ((bool)(resources.GetObject("combo_prof_ogl_prerender_frames.Enabled")));
      this.combo_prof_ogl_prerender_frames.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_ogl_prerender_frames.Font")));
      this.combo_prof_ogl_prerender_frames.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_ogl_prerender_frames.ImeMode")));
      this.combo_prof_ogl_prerender_frames.IntegralHeight = ((bool)(resources.GetObject("combo_prof_ogl_prerender_frames.IntegralHeight")));
      this.combo_prof_ogl_prerender_frames.ItemHeight = ((int)(resources.GetObject("combo_prof_ogl_prerender_frames.ItemHeight")));
      this.combo_prof_ogl_prerender_frames.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_ogl_prerender_frames.Location")));
      this.combo_prof_ogl_prerender_frames.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_ogl_prerender_frames.MaxDropDownItems")));
      this.combo_prof_ogl_prerender_frames.MaxLength = ((int)(resources.GetObject("combo_prof_ogl_prerender_frames.MaxLength")));
      this.combo_prof_ogl_prerender_frames.Name = "combo_prof_ogl_prerender_frames";
      this.combo_prof_ogl_prerender_frames.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_ogl_prerender_frames.RightToLeft")));
      this.combo_prof_ogl_prerender_frames.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_ogl_prerender_frames.Size")));
      this.combo_prof_ogl_prerender_frames.TabIndex = ((int)(resources.GetObject("combo_prof_ogl_prerender_frames.TabIndex")));
      this.combo_prof_ogl_prerender_frames.Text = resources.GetString("combo_prof_ogl_prerender_frames.Text");
      this.toolTip.SetToolTip(this.combo_prof_ogl_prerender_frames, resources.GetString("combo_prof_ogl_prerender_frames.ToolTip"));
      this.combo_prof_ogl_prerender_frames.Visible = ((bool)(resources.GetObject("combo_prof_ogl_prerender_frames.Visible")));
      // 
      // combo_prof_ogl_qe_mode
      // 
      this.combo_prof_ogl_qe_mode.AccessibleDescription = resources.GetString("combo_prof_ogl_qe_mode.AccessibleDescription");
      this.combo_prof_ogl_qe_mode.AccessibleName = resources.GetString("combo_prof_ogl_qe_mode.AccessibleName");
      this.combo_prof_ogl_qe_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_ogl_qe_mode.Anchor")));
      this.combo_prof_ogl_qe_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_ogl_qe_mode.BackgroundImage")));
      this.combo_prof_ogl_qe_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_ogl_qe_mode.Dock")));
      this.combo_prof_ogl_qe_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_ogl_qe_mode.Enabled = ((bool)(resources.GetObject("combo_prof_ogl_qe_mode.Enabled")));
      this.combo_prof_ogl_qe_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_ogl_qe_mode.Font")));
      this.combo_prof_ogl_qe_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_ogl_qe_mode.ImeMode")));
      this.combo_prof_ogl_qe_mode.IntegralHeight = ((bool)(resources.GetObject("combo_prof_ogl_qe_mode.IntegralHeight")));
      this.combo_prof_ogl_qe_mode.ItemHeight = ((int)(resources.GetObject("combo_prof_ogl_qe_mode.ItemHeight")));
      this.combo_prof_ogl_qe_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_ogl_qe_mode.Location")));
      this.combo_prof_ogl_qe_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_ogl_qe_mode.MaxDropDownItems")));
      this.combo_prof_ogl_qe_mode.MaxLength = ((int)(resources.GetObject("combo_prof_ogl_qe_mode.MaxLength")));
      this.combo_prof_ogl_qe_mode.Name = "combo_prof_ogl_qe_mode";
      this.combo_prof_ogl_qe_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_ogl_qe_mode.RightToLeft")));
      this.combo_prof_ogl_qe_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_ogl_qe_mode.Size")));
      this.combo_prof_ogl_qe_mode.TabIndex = ((int)(resources.GetObject("combo_prof_ogl_qe_mode.TabIndex")));
      this.combo_prof_ogl_qe_mode.Text = resources.GetString("combo_prof_ogl_qe_mode.Text");
      this.toolTip.SetToolTip(this.combo_prof_ogl_qe_mode, resources.GetString("combo_prof_ogl_qe_mode.ToolTip"));
      this.combo_prof_ogl_qe_mode.Visible = ((bool)(resources.GetObject("combo_prof_ogl_qe_mode.Visible")));
      // 
      // combo_prof_ogl_vsync_mode
      // 
      this.combo_prof_ogl_vsync_mode.AccessibleDescription = resources.GetString("combo_prof_ogl_vsync_mode.AccessibleDescription");
      this.combo_prof_ogl_vsync_mode.AccessibleName = resources.GetString("combo_prof_ogl_vsync_mode.AccessibleName");
      this.combo_prof_ogl_vsync_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_ogl_vsync_mode.Anchor")));
      this.combo_prof_ogl_vsync_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_ogl_vsync_mode.BackgroundImage")));
      this.combo_prof_ogl_vsync_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_ogl_vsync_mode.Dock")));
      this.combo_prof_ogl_vsync_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_ogl_vsync_mode.Enabled = ((bool)(resources.GetObject("combo_prof_ogl_vsync_mode.Enabled")));
      this.combo_prof_ogl_vsync_mode.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_ogl_vsync_mode.Font")));
      this.combo_prof_ogl_vsync_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_ogl_vsync_mode.ImeMode")));
      this.combo_prof_ogl_vsync_mode.IntegralHeight = ((bool)(resources.GetObject("combo_prof_ogl_vsync_mode.IntegralHeight")));
      this.combo_prof_ogl_vsync_mode.ItemHeight = ((int)(resources.GetObject("combo_prof_ogl_vsync_mode.ItemHeight")));
      this.combo_prof_ogl_vsync_mode.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_ogl_vsync_mode.Location")));
      this.combo_prof_ogl_vsync_mode.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_ogl_vsync_mode.MaxDropDownItems")));
      this.combo_prof_ogl_vsync_mode.MaxLength = ((int)(resources.GetObject("combo_prof_ogl_vsync_mode.MaxLength")));
      this.combo_prof_ogl_vsync_mode.Name = "combo_prof_ogl_vsync_mode";
      this.combo_prof_ogl_vsync_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_ogl_vsync_mode.RightToLeft")));
      this.combo_prof_ogl_vsync_mode.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_ogl_vsync_mode.Size")));
      this.combo_prof_ogl_vsync_mode.TabIndex = ((int)(resources.GetObject("combo_prof_ogl_vsync_mode.TabIndex")));
      this.combo_prof_ogl_vsync_mode.Text = resources.GetString("combo_prof_ogl_vsync_mode.Text");
      this.toolTip.SetToolTip(this.combo_prof_ogl_vsync_mode, resources.GetString("combo_prof_ogl_vsync_mode.ToolTip"));
      this.combo_prof_ogl_vsync_mode.Visible = ((bool)(resources.GetObject("combo_prof_ogl_vsync_mode.Visible")));
      // 
      // dialog_prof_choose_exec
      // 
      this.dialog_prof_choose_exec.Filter = resources.GetString("dialog_prof_choose_exec.Filter");
      this.dialog_prof_choose_exec.Title = resources.GetString("dialog_prof_choose_exec.Title");
      this.dialog_prof_choose_exec.FileOk += new System.ComponentModel.CancelEventHandler(this.dialog_prof_choose_exec_FileOk);
      // 
      // label_extra2_prof_ogl
      // 
      this.label_extra2_prof_ogl.AccessibleDescription = resources.GetString("label_extra2_prof_ogl.AccessibleDescription");
      this.label_extra2_prof_ogl.AccessibleName = resources.GetString("label_extra2_prof_ogl.AccessibleName");
      this.label_extra2_prof_ogl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_prof_ogl.Anchor")));
      this.label_extra2_prof_ogl.AutoSize = ((bool)(resources.GetObject("label_extra2_prof_ogl.AutoSize")));
      this.label_extra2_prof_ogl.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_prof_ogl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_prof_ogl.Dock")));
      this.label_extra2_prof_ogl.Enabled = ((bool)(resources.GetObject("label_extra2_prof_ogl.Enabled")));
      this.label_extra2_prof_ogl.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_prof_ogl.Font")));
      this.label_extra2_prof_ogl.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_prof_ogl.Image")));
      this.label_extra2_prof_ogl.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_prof_ogl.ImageAlign")));
      this.label_extra2_prof_ogl.ImageIndex = ((int)(resources.GetObject("label_extra2_prof_ogl.ImageIndex")));
      this.label_extra2_prof_ogl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_prof_ogl.ImeMode")));
      this.label_extra2_prof_ogl.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_prof_ogl.Location")));
      this.label_extra2_prof_ogl.Name = "label_extra2_prof_ogl";
      this.label_extra2_prof_ogl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_prof_ogl.RightToLeft")));
      this.label_extra2_prof_ogl.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_prof_ogl.Size")));
      this.label_extra2_prof_ogl.TabIndex = ((int)(resources.GetObject("label_extra2_prof_ogl.TabIndex")));
      this.label_extra2_prof_ogl.Text = resources.GetString("label_extra2_prof_ogl.Text");
      this.label_extra2_prof_ogl.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_prof_ogl.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_prof_ogl, resources.GetString("label_extra2_prof_ogl.ToolTip"));
      this.label_extra2_prof_ogl.Visible = ((bool)(resources.GetObject("label_extra2_prof_ogl.Visible")));
      this.label_extra2_prof_ogl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_ogl_MouseDown);
      // 
      // label_extra2_prof_d3d
      // 
      this.label_extra2_prof_d3d.AccessibleDescription = resources.GetString("label_extra2_prof_d3d.AccessibleDescription");
      this.label_extra2_prof_d3d.AccessibleName = resources.GetString("label_extra2_prof_d3d.AccessibleName");
      this.label_extra2_prof_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_prof_d3d.Anchor")));
      this.label_extra2_prof_d3d.AutoSize = ((bool)(resources.GetObject("label_extra2_prof_d3d.AutoSize")));
      this.label_extra2_prof_d3d.CausesValidation = false;
      this.label_extra2_prof_d3d.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_prof_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_prof_d3d.Dock")));
      this.label_extra2_prof_d3d.Enabled = ((bool)(resources.GetObject("label_extra2_prof_d3d.Enabled")));
      this.label_extra2_prof_d3d.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_prof_d3d.Font")));
      this.label_extra2_prof_d3d.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_prof_d3d.Image")));
      this.label_extra2_prof_d3d.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_prof_d3d.ImageAlign")));
      this.label_extra2_prof_d3d.ImageIndex = ((int)(resources.GetObject("label_extra2_prof_d3d.ImageIndex")));
      this.label_extra2_prof_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_prof_d3d.ImeMode")));
      this.label_extra2_prof_d3d.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_prof_d3d.Location")));
      this.label_extra2_prof_d3d.Name = "label_extra2_prof_d3d";
      this.label_extra2_prof_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_prof_d3d.RightToLeft")));
      this.label_extra2_prof_d3d.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_prof_d3d.Size")));
      this.label_extra2_prof_d3d.TabIndex = ((int)(resources.GetObject("label_extra2_prof_d3d.TabIndex")));
      this.label_extra2_prof_d3d.Text = resources.GetString("label_extra2_prof_d3d.Text");
      this.label_extra2_prof_d3d.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_prof_d3d.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_prof_d3d, resources.GetString("label_extra2_prof_d3d.ToolTip"));
      this.label_extra2_prof_d3d.Visible = ((bool)(resources.GetObject("label_extra2_prof_d3d.Visible")));
      this.label_extra2_prof_d3d.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_d3d_MouseDown);
      // 
      // combo_extra2_curr_d3d_6
      // 
      this.combo_extra2_curr_d3d_6.AccessibleDescription = resources.GetString("combo_extra2_curr_d3d_6.AccessibleDescription");
      this.combo_extra2_curr_d3d_6.AccessibleName = resources.GetString("combo_extra2_curr_d3d_6.AccessibleName");
      this.combo_extra2_curr_d3d_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_d3d_6.Anchor")));
      this.combo_extra2_curr_d3d_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_d3d_6.BackgroundImage")));
      this.combo_extra2_curr_d3d_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_d3d_6.Dock")));
      this.combo_extra2_curr_d3d_6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_d3d_6.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_d3d_6.Enabled")));
      this.combo_extra2_curr_d3d_6.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_d3d_6.Font")));
      this.combo_extra2_curr_d3d_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_d3d_6.ImeMode")));
      this.combo_extra2_curr_d3d_6.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_d3d_6.IntegralHeight")));
      this.combo_extra2_curr_d3d_6.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_d3d_6.ItemHeight")));
      this.combo_extra2_curr_d3d_6.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_d3d_6.Location")));
      this.combo_extra2_curr_d3d_6.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_d3d_6.MaxDropDownItems")));
      this.combo_extra2_curr_d3d_6.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_d3d_6.MaxLength")));
      this.combo_extra2_curr_d3d_6.Name = "combo_extra2_curr_d3d_6";
      this.combo_extra2_curr_d3d_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_d3d_6.RightToLeft")));
      this.combo_extra2_curr_d3d_6.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_d3d_6.Size")));
      this.combo_extra2_curr_d3d_6.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_d3d_6.TabIndex")));
      this.combo_extra2_curr_d3d_6.Text = resources.GetString("combo_extra2_curr_d3d_6.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_d3d_6, resources.GetString("combo_extra2_curr_d3d_6.ToolTip"));
      this.combo_extra2_curr_d3d_6.Visible = ((bool)(resources.GetObject("combo_extra2_curr_d3d_6.Visible")));
      // 
      // label_extra2_curr_ogl
      // 
      this.label_extra2_curr_ogl.AccessibleDescription = resources.GetString("label_extra2_curr_ogl.AccessibleDescription");
      this.label_extra2_curr_ogl.AccessibleName = resources.GetString("label_extra2_curr_ogl.AccessibleName");
      this.label_extra2_curr_ogl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_curr_ogl.Anchor")));
      this.label_extra2_curr_ogl.AutoSize = ((bool)(resources.GetObject("label_extra2_curr_ogl.AutoSize")));
      this.label_extra2_curr_ogl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_curr_ogl.Dock")));
      this.label_extra2_curr_ogl.Enabled = ((bool)(resources.GetObject("label_extra2_curr_ogl.Enabled")));
      this.label_extra2_curr_ogl.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_curr_ogl.Font")));
      this.label_extra2_curr_ogl.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_curr_ogl.Image")));
      this.label_extra2_curr_ogl.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_curr_ogl.ImageAlign")));
      this.label_extra2_curr_ogl.ImageIndex = ((int)(resources.GetObject("label_extra2_curr_ogl.ImageIndex")));
      this.label_extra2_curr_ogl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_curr_ogl.ImeMode")));
      this.label_extra2_curr_ogl.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_curr_ogl.Location")));
      this.label_extra2_curr_ogl.Name = "label_extra2_curr_ogl";
      this.label_extra2_curr_ogl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_curr_ogl.RightToLeft")));
      this.label_extra2_curr_ogl.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_curr_ogl.Size")));
      this.label_extra2_curr_ogl.TabIndex = ((int)(resources.GetObject("label_extra2_curr_ogl.TabIndex")));
      this.label_extra2_curr_ogl.Text = resources.GetString("label_extra2_curr_ogl.Text");
      this.label_extra2_curr_ogl.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_curr_ogl.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_curr_ogl, resources.GetString("label_extra2_curr_ogl.ToolTip"));
      this.label_extra2_curr_ogl.Visible = ((bool)(resources.GetObject("label_extra2_curr_ogl.Visible")));
      // 
      // label_extra2_curr_d3d
      // 
      this.label_extra2_curr_d3d.AccessibleDescription = resources.GetString("label_extra2_curr_d3d.AccessibleDescription");
      this.label_extra2_curr_d3d.AccessibleName = resources.GetString("label_extra2_curr_d3d.AccessibleName");
      this.label_extra2_curr_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_curr_d3d.Anchor")));
      this.label_extra2_curr_d3d.AutoSize = ((bool)(resources.GetObject("label_extra2_curr_d3d.AutoSize")));
      this.label_extra2_curr_d3d.CausesValidation = false;
      this.label_extra2_curr_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_curr_d3d.Dock")));
      this.label_extra2_curr_d3d.Enabled = ((bool)(resources.GetObject("label_extra2_curr_d3d.Enabled")));
      this.label_extra2_curr_d3d.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_curr_d3d.Font")));
      this.label_extra2_curr_d3d.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_curr_d3d.Image")));
      this.label_extra2_curr_d3d.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_curr_d3d.ImageAlign")));
      this.label_extra2_curr_d3d.ImageIndex = ((int)(resources.GetObject("label_extra2_curr_d3d.ImageIndex")));
      this.label_extra2_curr_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_curr_d3d.ImeMode")));
      this.label_extra2_curr_d3d.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_curr_d3d.Location")));
      this.label_extra2_curr_d3d.Name = "label_extra2_curr_d3d";
      this.label_extra2_curr_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_curr_d3d.RightToLeft")));
      this.label_extra2_curr_d3d.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_curr_d3d.Size")));
      this.label_extra2_curr_d3d.TabIndex = ((int)(resources.GetObject("label_extra2_curr_d3d.TabIndex")));
      this.label_extra2_curr_d3d.Text = resources.GetString("label_extra2_curr_d3d.Text");
      this.label_extra2_curr_d3d.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_curr_d3d.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_curr_d3d, resources.GetString("label_extra2_curr_d3d.ToolTip"));
      this.label_extra2_curr_d3d.Visible = ((bool)(resources.GetObject("label_extra2_curr_d3d.Visible")));
      // 
      // group_main_d3d
      // 
      this.group_main_d3d.AccessibleDescription = resources.GetString("group_main_d3d.AccessibleDescription");
      this.group_main_d3d.AccessibleName = resources.GetString("group_main_d3d.AccessibleName");
      this.group_main_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("group_main_d3d.Anchor")));
      this.group_main_d3d.BackColor = System.Drawing.SystemColors.Control;
      this.group_main_d3d.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("group_main_d3d.BackgroundImage")));
      this.group_main_d3d.Controls.Add(this.label4);
      this.group_main_d3d.Controls.Add(this.label2);
      this.group_main_d3d.Controls.Add(this.combo_d3d_prerender_frames);
      this.group_main_d3d.Controls.Add(this.combo_d3d_qe_mode);
      this.group_main_d3d.Controls.Add(this.combo_d3d_vsync_mode);
      this.group_main_d3d.Controls.Add(this.combo_d3d_fsaa_mode);
      this.group_main_d3d.Controls.Add(this.label_d3d);
      this.group_main_d3d.Controls.Add(this.combo_d3d_aniso_mode);
      this.group_main_d3d.Controls.Add(this.combo_d3d_lod_bias);
      this.group_main_d3d.Controls.Add(this.combo_prof_d3d_fsaa_mode);
      this.group_main_d3d.Controls.Add(this.combo_prof_d3d_prerender_frames);
      this.group_main_d3d.Controls.Add(this.label_prof_d3d);
      this.group_main_d3d.Controls.Add(this.combo_prof_d3d_lod_bias);
      this.group_main_d3d.Controls.Add(this.combo_prof_d3d_qe_mode);
      this.group_main_d3d.Controls.Add(this.combo_prof_d3d_aniso_mode);
      this.group_main_d3d.Controls.Add(this.combo_prof_d3d_vsync_mode);
      this.group_main_d3d.Controls.Add(this.label_prerender_frames);
      this.group_main_d3d.Controls.Add(this.label_fsaa_mode);
      this.group_main_d3d.Controls.Add(this.label_vsync_mode);
      this.group_main_d3d.Controls.Add(this.label_lod_bias);
      this.group_main_d3d.Controls.Add(this.label_aniso_mode);
      this.group_main_d3d.Controls.Add(this.label_quality);
      this.group_main_d3d.Controls.Add(this.combo_prof_ogl_vsync_mode);
      this.group_main_d3d.Controls.Add(this.combo_prof_ogl_qe_mode);
      this.group_main_d3d.Controls.Add(this.combo_ogl_vsync_mode);
      this.group_main_d3d.Controls.Add(this.combo_ogl_fsaa_mode);
      this.group_main_d3d.Controls.Add(this.combo_prof_ogl_aniso_mode);
      this.group_main_d3d.Controls.Add(this.label_ogl);
      this.group_main_d3d.Controls.Add(this.combo_ogl_aniso_mode);
      this.group_main_d3d.Controls.Add(this.combo_ogl_prerender_frames);
      this.group_main_d3d.Controls.Add(this.combo_ogl_lod_bias);
      this.group_main_d3d.Controls.Add(this.combo_ogl_qe_mode);
      this.group_main_d3d.Controls.Add(this.label_prof_ogl);
      this.group_main_d3d.Controls.Add(this.combo_prof_ogl_lod_bias);
      this.group_main_d3d.Controls.Add(this.combo_prof_ogl_fsaa_mode);
      this.group_main_d3d.Controls.Add(this.combo_prof_ogl_prerender_frames);
      this.group_main_d3d.Cursor = System.Windows.Forms.Cursors.Default;
      this.group_main_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("group_main_d3d.Dock")));
      this.group_main_d3d.Enabled = ((bool)(resources.GetObject("group_main_d3d.Enabled")));
      this.group_main_d3d.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.group_main_d3d.Font = ((System.Drawing.Font)(resources.GetObject("group_main_d3d.Font")));
      this.group_main_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("group_main_d3d.ImeMode")));
      this.group_main_d3d.Location = ((System.Drawing.Point)(resources.GetObject("group_main_d3d.Location")));
      this.group_main_d3d.Name = "group_main_d3d";
      this.group_main_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("group_main_d3d.RightToLeft")));
      this.group_main_d3d.Size = ((System.Drawing.Size)(resources.GetObject("group_main_d3d.Size")));
      this.group_main_d3d.TabIndex = ((int)(resources.GetObject("group_main_d3d.TabIndex")));
      this.group_main_d3d.TabStop = false;
      this.group_main_d3d.Text = resources.GetString("group_main_d3d.Text");
      this.toolTip.SetToolTip(this.group_main_d3d, resources.GetString("group_main_d3d.ToolTip"));
      this.group_main_d3d.Visible = ((bool)(resources.GetObject("group_main_d3d.Visible")));
      this.group_main_d3d.Enter += new System.EventHandler(this.group_current_Enter);
      // 
      // label4
      // 
      this.label4.AccessibleDescription = resources.GetString("label4.AccessibleDescription");
      this.label4.AccessibleName = resources.GetString("label4.AccessibleName");
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label4.Anchor")));
      this.label4.AutoSize = ((bool)(resources.GetObject("label4.AutoSize")));
      this.label4.BackColor = System.Drawing.SystemColors.Window;
      this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.label4.CausesValidation = false;
      this.label4.Cursor = System.Windows.Forms.Cursors.Default;
      this.label4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label4.Dock")));
      this.label4.Enabled = ((bool)(resources.GetObject("label4.Enabled")));
      this.label4.Font = ((System.Drawing.Font)(resources.GetObject("label4.Font")));
      this.label4.ForeColor = System.Drawing.SystemColors.ControlDark;
      this.label4.Image = ((System.Drawing.Image)(resources.GetObject("label4.Image")));
      this.label4.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label4.ImageAlign")));
      this.label4.ImageIndex = ((int)(resources.GetObject("label4.ImageIndex")));
      this.label4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label4.ImeMode")));
      this.label4.Location = ((System.Drawing.Point)(resources.GetObject("label4.Location")));
      this.label4.Name = "label4";
      this.label4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label4.RightToLeft")));
      this.label4.Size = ((System.Drawing.Size)(resources.GetObject("label4.Size")));
      this.label4.TabIndex = ((int)(resources.GetObject("label4.TabIndex")));
      this.label4.Text = resources.GetString("label4.Text");
      this.label4.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label4.TextAlign")));
      this.toolTip.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
      this.label4.Visible = ((bool)(resources.GetObject("label4.Visible")));
      // 
      // label2
      // 
      this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
      this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
      this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
      this.label2.BackColor = System.Drawing.SystemColors.Window;
      this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.label2.CausesValidation = false;
      this.label2.Cursor = System.Windows.Forms.Cursors.Default;
      this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
      this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
      this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
      this.label2.ForeColor = System.Drawing.SystemColors.ControlDark;
      this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
      this.label2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.ImageAlign")));
      this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
      this.label2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label2.ImeMode")));
      this.label2.Location = ((System.Drawing.Point)(resources.GetObject("label2.Location")));
      this.label2.Name = "label2";
      this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label2.RightToLeft")));
      this.label2.Size = ((System.Drawing.Size)(resources.GetObject("label2.Size")));
      this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
      this.label2.Text = resources.GetString("label2.Text");
      this.label2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.TextAlign")));
      this.toolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
      this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
      // 
      // label_d3d
      // 
      this.label_d3d.AccessibleDescription = resources.GetString("label_d3d.AccessibleDescription");
      this.label_d3d.AccessibleName = resources.GetString("label_d3d.AccessibleName");
      this.label_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_d3d.Anchor")));
      this.label_d3d.AutoSize = ((bool)(resources.GetObject("label_d3d.AutoSize")));
      this.label_d3d.CausesValidation = false;
      this.label_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_d3d.Dock")));
      this.label_d3d.Enabled = ((bool)(resources.GetObject("label_d3d.Enabled")));
      this.label_d3d.Font = ((System.Drawing.Font)(resources.GetObject("label_d3d.Font")));
      this.label_d3d.Image = ((System.Drawing.Image)(resources.GetObject("label_d3d.Image")));
      this.label_d3d.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_d3d.ImageAlign")));
      this.label_d3d.ImageIndex = ((int)(resources.GetObject("label_d3d.ImageIndex")));
      this.label_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_d3d.ImeMode")));
      this.label_d3d.Location = ((System.Drawing.Point)(resources.GetObject("label_d3d.Location")));
      this.label_d3d.Name = "label_d3d";
      this.label_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_d3d.RightToLeft")));
      this.label_d3d.Size = ((System.Drawing.Size)(resources.GetObject("label_d3d.Size")));
      this.label_d3d.TabIndex = ((int)(resources.GetObject("label_d3d.TabIndex")));
      this.label_d3d.Text = resources.GetString("label_d3d.Text");
      this.label_d3d.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_d3d.TextAlign")));
      this.toolTip.SetToolTip(this.label_d3d, resources.GetString("label_d3d.ToolTip"));
      this.label_d3d.Visible = ((bool)(resources.GetObject("label_d3d.Visible")));
      // 
      // label_prof_d3d
      // 
      this.label_prof_d3d.AccessibleDescription = resources.GetString("label_prof_d3d.AccessibleDescription");
      this.label_prof_d3d.AccessibleName = resources.GetString("label_prof_d3d.AccessibleName");
      this.label_prof_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_prof_d3d.Anchor")));
      this.label_prof_d3d.AutoSize = ((bool)(resources.GetObject("label_prof_d3d.AutoSize")));
      this.label_prof_d3d.CausesValidation = false;
      this.label_prof_d3d.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_prof_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_prof_d3d.Dock")));
      this.label_prof_d3d.Enabled = ((bool)(resources.GetObject("label_prof_d3d.Enabled")));
      this.label_prof_d3d.Font = ((System.Drawing.Font)(resources.GetObject("label_prof_d3d.Font")));
      this.label_prof_d3d.Image = ((System.Drawing.Image)(resources.GetObject("label_prof_d3d.Image")));
      this.label_prof_d3d.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_prof_d3d.ImageAlign")));
      this.label_prof_d3d.ImageIndex = ((int)(resources.GetObject("label_prof_d3d.ImageIndex")));
      this.label_prof_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_prof_d3d.ImeMode")));
      this.label_prof_d3d.Location = ((System.Drawing.Point)(resources.GetObject("label_prof_d3d.Location")));
      this.label_prof_d3d.Name = "label_prof_d3d";
      this.label_prof_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_prof_d3d.RightToLeft")));
      this.label_prof_d3d.Size = ((System.Drawing.Size)(resources.GetObject("label_prof_d3d.Size")));
      this.label_prof_d3d.TabIndex = ((int)(resources.GetObject("label_prof_d3d.TabIndex")));
      this.label_prof_d3d.Text = resources.GetString("label_prof_d3d.Text");
      this.label_prof_d3d.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_prof_d3d.TextAlign")));
      this.toolTip.SetToolTip(this.label_prof_d3d, resources.GetString("label_prof_d3d.ToolTip"));
      this.label_prof_d3d.Visible = ((bool)(resources.GetObject("label_prof_d3d.Visible")));
      this.label_prof_d3d.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_d3d_MouseDown);
      // 
      // label_prerender_frames
      // 
      this.label_prerender_frames.AccessibleDescription = resources.GetString("label_prerender_frames.AccessibleDescription");
      this.label_prerender_frames.AccessibleName = resources.GetString("label_prerender_frames.AccessibleName");
      this.label_prerender_frames.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_prerender_frames.Anchor")));
      this.label_prerender_frames.AutoSize = ((bool)(resources.GetObject("label_prerender_frames.AutoSize")));
      this.label_prerender_frames.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_prerender_frames.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_prerender_frames.Dock")));
      this.label_prerender_frames.Enabled = ((bool)(resources.GetObject("label_prerender_frames.Enabled")));
      this.label_prerender_frames.Font = ((System.Drawing.Font)(resources.GetObject("label_prerender_frames.Font")));
      this.label_prerender_frames.Image = ((System.Drawing.Image)(resources.GetObject("label_prerender_frames.Image")));
      this.label_prerender_frames.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_prerender_frames.ImageAlign")));
      this.label_prerender_frames.ImageIndex = ((int)(resources.GetObject("label_prerender_frames.ImageIndex")));
      this.label_prerender_frames.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_prerender_frames.ImeMode")));
      this.label_prerender_frames.Location = ((System.Drawing.Point)(resources.GetObject("label_prerender_frames.Location")));
      this.label_prerender_frames.Name = "label_prerender_frames";
      this.label_prerender_frames.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_prerender_frames.RightToLeft")));
      this.label_prerender_frames.Size = ((System.Drawing.Size)(resources.GetObject("label_prerender_frames.Size")));
      this.label_prerender_frames.TabIndex = ((int)(resources.GetObject("label_prerender_frames.TabIndex")));
      this.label_prerender_frames.Tag = "5";
      this.label_prerender_frames.Text = resources.GetString("label_prerender_frames.Text");
      this.label_prerender_frames.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_prerender_frames.TextAlign")));
      this.toolTip.SetToolTip(this.label_prerender_frames, resources.GetString("label_prerender_frames.ToolTip"));
      this.label_prerender_frames.Visible = ((bool)(resources.GetObject("label_prerender_frames.Visible")));
      this.label_prerender_frames.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_fsaa_mode
      // 
      this.label_fsaa_mode.AccessibleDescription = resources.GetString("label_fsaa_mode.AccessibleDescription");
      this.label_fsaa_mode.AccessibleName = resources.GetString("label_fsaa_mode.AccessibleName");
      this.label_fsaa_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_fsaa_mode.Anchor")));
      this.label_fsaa_mode.AutoSize = ((bool)(resources.GetObject("label_fsaa_mode.AutoSize")));
      this.label_fsaa_mode.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_fsaa_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_fsaa_mode.Dock")));
      this.label_fsaa_mode.Enabled = ((bool)(resources.GetObject("label_fsaa_mode.Enabled")));
      this.label_fsaa_mode.Font = ((System.Drawing.Font)(resources.GetObject("label_fsaa_mode.Font")));
      this.label_fsaa_mode.Image = ((System.Drawing.Image)(resources.GetObject("label_fsaa_mode.Image")));
      this.label_fsaa_mode.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_fsaa_mode.ImageAlign")));
      this.label_fsaa_mode.ImageIndex = ((int)(resources.GetObject("label_fsaa_mode.ImageIndex")));
      this.label_fsaa_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_fsaa_mode.ImeMode")));
      this.label_fsaa_mode.Location = ((System.Drawing.Point)(resources.GetObject("label_fsaa_mode.Location")));
      this.label_fsaa_mode.Name = "label_fsaa_mode";
      this.label_fsaa_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_fsaa_mode.RightToLeft")));
      this.label_fsaa_mode.Size = ((System.Drawing.Size)(resources.GetObject("label_fsaa_mode.Size")));
      this.label_fsaa_mode.TabIndex = ((int)(resources.GetObject("label_fsaa_mode.TabIndex")));
      this.label_fsaa_mode.Tag = "0";
      this.label_fsaa_mode.Text = resources.GetString("label_fsaa_mode.Text");
      this.label_fsaa_mode.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_fsaa_mode.TextAlign")));
      this.toolTip.SetToolTip(this.label_fsaa_mode, resources.GetString("label_fsaa_mode.ToolTip"));
      this.label_fsaa_mode.Visible = ((bool)(resources.GetObject("label_fsaa_mode.Visible")));
      this.label_fsaa_mode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_vsync_mode
      // 
      this.label_vsync_mode.AccessibleDescription = resources.GetString("label_vsync_mode.AccessibleDescription");
      this.label_vsync_mode.AccessibleName = resources.GetString("label_vsync_mode.AccessibleName");
      this.label_vsync_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_vsync_mode.Anchor")));
      this.label_vsync_mode.AutoSize = ((bool)(resources.GetObject("label_vsync_mode.AutoSize")));
      this.label_vsync_mode.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_vsync_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_vsync_mode.Dock")));
      this.label_vsync_mode.Enabled = ((bool)(resources.GetObject("label_vsync_mode.Enabled")));
      this.label_vsync_mode.Font = ((System.Drawing.Font)(resources.GetObject("label_vsync_mode.Font")));
      this.label_vsync_mode.Image = ((System.Drawing.Image)(resources.GetObject("label_vsync_mode.Image")));
      this.label_vsync_mode.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_vsync_mode.ImageAlign")));
      this.label_vsync_mode.ImageIndex = ((int)(resources.GetObject("label_vsync_mode.ImageIndex")));
      this.label_vsync_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_vsync_mode.ImeMode")));
      this.label_vsync_mode.Location = ((System.Drawing.Point)(resources.GetObject("label_vsync_mode.Location")));
      this.label_vsync_mode.Name = "label_vsync_mode";
      this.label_vsync_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_vsync_mode.RightToLeft")));
      this.label_vsync_mode.Size = ((System.Drawing.Size)(resources.GetObject("label_vsync_mode.Size")));
      this.label_vsync_mode.TabIndex = ((int)(resources.GetObject("label_vsync_mode.TabIndex")));
      this.label_vsync_mode.Tag = "2";
      this.label_vsync_mode.Text = resources.GetString("label_vsync_mode.Text");
      this.label_vsync_mode.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_vsync_mode.TextAlign")));
      this.toolTip.SetToolTip(this.label_vsync_mode, resources.GetString("label_vsync_mode.ToolTip"));
      this.label_vsync_mode.Visible = ((bool)(resources.GetObject("label_vsync_mode.Visible")));
      this.label_vsync_mode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_lod_bias
      // 
      this.label_lod_bias.AccessibleDescription = resources.GetString("label_lod_bias.AccessibleDescription");
      this.label_lod_bias.AccessibleName = resources.GetString("label_lod_bias.AccessibleName");
      this.label_lod_bias.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_lod_bias.Anchor")));
      this.label_lod_bias.AutoSize = ((bool)(resources.GetObject("label_lod_bias.AutoSize")));
      this.label_lod_bias.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_lod_bias.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_lod_bias.Dock")));
      this.label_lod_bias.Enabled = ((bool)(resources.GetObject("label_lod_bias.Enabled")));
      this.label_lod_bias.Font = ((System.Drawing.Font)(resources.GetObject("label_lod_bias.Font")));
      this.label_lod_bias.Image = ((System.Drawing.Image)(resources.GetObject("label_lod_bias.Image")));
      this.label_lod_bias.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_lod_bias.ImageAlign")));
      this.label_lod_bias.ImageIndex = ((int)(resources.GetObject("label_lod_bias.ImageIndex")));
      this.label_lod_bias.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_lod_bias.ImeMode")));
      this.label_lod_bias.Location = ((System.Drawing.Point)(resources.GetObject("label_lod_bias.Location")));
      this.label_lod_bias.Name = "label_lod_bias";
      this.label_lod_bias.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_lod_bias.RightToLeft")));
      this.label_lod_bias.Size = ((System.Drawing.Size)(resources.GetObject("label_lod_bias.Size")));
      this.label_lod_bias.TabIndex = ((int)(resources.GetObject("label_lod_bias.TabIndex")));
      this.label_lod_bias.Tag = "4";
      this.label_lod_bias.Text = resources.GetString("label_lod_bias.Text");
      this.label_lod_bias.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_lod_bias.TextAlign")));
      this.toolTip.SetToolTip(this.label_lod_bias, resources.GetString("label_lod_bias.ToolTip"));
      this.label_lod_bias.Visible = ((bool)(resources.GetObject("label_lod_bias.Visible")));
      this.label_lod_bias.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_aniso_mode
      // 
      this.label_aniso_mode.AccessibleDescription = resources.GetString("label_aniso_mode.AccessibleDescription");
      this.label_aniso_mode.AccessibleName = resources.GetString("label_aniso_mode.AccessibleName");
      this.label_aniso_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_aniso_mode.Anchor")));
      this.label_aniso_mode.AutoSize = ((bool)(resources.GetObject("label_aniso_mode.AutoSize")));
      this.label_aniso_mode.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_aniso_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_aniso_mode.Dock")));
      this.label_aniso_mode.Enabled = ((bool)(resources.GetObject("label_aniso_mode.Enabled")));
      this.label_aniso_mode.Font = ((System.Drawing.Font)(resources.GetObject("label_aniso_mode.Font")));
      this.label_aniso_mode.Image = ((System.Drawing.Image)(resources.GetObject("label_aniso_mode.Image")));
      this.label_aniso_mode.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_aniso_mode.ImageAlign")));
      this.label_aniso_mode.ImageIndex = ((int)(resources.GetObject("label_aniso_mode.ImageIndex")));
      this.label_aniso_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_aniso_mode.ImeMode")));
      this.label_aniso_mode.Location = ((System.Drawing.Point)(resources.GetObject("label_aniso_mode.Location")));
      this.label_aniso_mode.Name = "label_aniso_mode";
      this.label_aniso_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_aniso_mode.RightToLeft")));
      this.label_aniso_mode.Size = ((System.Drawing.Size)(resources.GetObject("label_aniso_mode.Size")));
      this.label_aniso_mode.TabIndex = ((int)(resources.GetObject("label_aniso_mode.TabIndex")));
      this.label_aniso_mode.Tag = "1";
      this.label_aniso_mode.Text = resources.GetString("label_aniso_mode.Text");
      this.label_aniso_mode.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_aniso_mode.TextAlign")));
      this.toolTip.SetToolTip(this.label_aniso_mode, resources.GetString("label_aniso_mode.ToolTip"));
      this.label_aniso_mode.Visible = ((bool)(resources.GetObject("label_aniso_mode.Visible")));
      this.label_aniso_mode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_quality
      // 
      this.label_quality.AccessibleDescription = resources.GetString("label_quality.AccessibleDescription");
      this.label_quality.AccessibleName = resources.GetString("label_quality.AccessibleName");
      this.label_quality.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_quality.Anchor")));
      this.label_quality.AutoSize = ((bool)(resources.GetObject("label_quality.AutoSize")));
      this.label_quality.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_quality.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_quality.Dock")));
      this.label_quality.Enabled = ((bool)(resources.GetObject("label_quality.Enabled")));
      this.label_quality.Font = ((System.Drawing.Font)(resources.GetObject("label_quality.Font")));
      this.label_quality.Image = ((System.Drawing.Image)(resources.GetObject("label_quality.Image")));
      this.label_quality.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_quality.ImageAlign")));
      this.label_quality.ImageIndex = ((int)(resources.GetObject("label_quality.ImageIndex")));
      this.label_quality.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_quality.ImeMode")));
      this.label_quality.Location = ((System.Drawing.Point)(resources.GetObject("label_quality.Location")));
      this.label_quality.Name = "label_quality";
      this.label_quality.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_quality.RightToLeft")));
      this.label_quality.Size = ((System.Drawing.Size)(resources.GetObject("label_quality.Size")));
      this.label_quality.TabIndex = ((int)(resources.GetObject("label_quality.TabIndex")));
      this.label_quality.Tag = "3";
      this.label_quality.Text = resources.GetString("label_quality.Text");
      this.label_quality.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_quality.TextAlign")));
      this.toolTip.SetToolTip(this.label_quality, resources.GetString("label_quality.ToolTip"));
      this.label_quality.Visible = ((bool)(resources.GetObject("label_quality.Visible")));
      this.label_quality.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_ogl
      // 
      this.label_ogl.AccessibleDescription = resources.GetString("label_ogl.AccessibleDescription");
      this.label_ogl.AccessibleName = resources.GetString("label_ogl.AccessibleName");
      this.label_ogl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_ogl.Anchor")));
      this.label_ogl.AutoSize = ((bool)(resources.GetObject("label_ogl.AutoSize")));
      this.label_ogl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_ogl.Dock")));
      this.label_ogl.Enabled = ((bool)(resources.GetObject("label_ogl.Enabled")));
      this.label_ogl.Font = ((System.Drawing.Font)(resources.GetObject("label_ogl.Font")));
      this.label_ogl.Image = ((System.Drawing.Image)(resources.GetObject("label_ogl.Image")));
      this.label_ogl.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_ogl.ImageAlign")));
      this.label_ogl.ImageIndex = ((int)(resources.GetObject("label_ogl.ImageIndex")));
      this.label_ogl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_ogl.ImeMode")));
      this.label_ogl.Location = ((System.Drawing.Point)(resources.GetObject("label_ogl.Location")));
      this.label_ogl.Name = "label_ogl";
      this.label_ogl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_ogl.RightToLeft")));
      this.label_ogl.Size = ((System.Drawing.Size)(resources.GetObject("label_ogl.Size")));
      this.label_ogl.TabIndex = ((int)(resources.GetObject("label_ogl.TabIndex")));
      this.label_ogl.Text = resources.GetString("label_ogl.Text");
      this.label_ogl.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_ogl.TextAlign")));
      this.toolTip.SetToolTip(this.label_ogl, resources.GetString("label_ogl.ToolTip"));
      this.label_ogl.Visible = ((bool)(resources.GetObject("label_ogl.Visible")));
      // 
      // label_prof_ogl
      // 
      this.label_prof_ogl.AccessibleDescription = resources.GetString("label_prof_ogl.AccessibleDescription");
      this.label_prof_ogl.AccessibleName = resources.GetString("label_prof_ogl.AccessibleName");
      this.label_prof_ogl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_prof_ogl.Anchor")));
      this.label_prof_ogl.AutoSize = ((bool)(resources.GetObject("label_prof_ogl.AutoSize")));
      this.label_prof_ogl.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_prof_ogl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_prof_ogl.Dock")));
      this.label_prof_ogl.Enabled = ((bool)(resources.GetObject("label_prof_ogl.Enabled")));
      this.label_prof_ogl.Font = ((System.Drawing.Font)(resources.GetObject("label_prof_ogl.Font")));
      this.label_prof_ogl.Image = ((System.Drawing.Image)(resources.GetObject("label_prof_ogl.Image")));
      this.label_prof_ogl.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_prof_ogl.ImageAlign")));
      this.label_prof_ogl.ImageIndex = ((int)(resources.GetObject("label_prof_ogl.ImageIndex")));
      this.label_prof_ogl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_prof_ogl.ImeMode")));
      this.label_prof_ogl.Location = ((System.Drawing.Point)(resources.GetObject("label_prof_ogl.Location")));
      this.label_prof_ogl.Name = "label_prof_ogl";
      this.label_prof_ogl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_prof_ogl.RightToLeft")));
      this.label_prof_ogl.Size = ((System.Drawing.Size)(resources.GetObject("label_prof_ogl.Size")));
      this.label_prof_ogl.TabIndex = ((int)(resources.GetObject("label_prof_ogl.TabIndex")));
      this.label_prof_ogl.Text = resources.GetString("label_prof_ogl.Text");
      this.label_prof_ogl.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_prof_ogl.TextAlign")));
      this.toolTip.SetToolTip(this.label_prof_ogl, resources.GetString("label_prof_ogl.ToolTip"));
      this.label_prof_ogl.Visible = ((bool)(resources.GetObject("label_prof_ogl.Visible")));
      this.label_prof_ogl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_ogl_MouseDown);
      // 
      // group_extra_d3d
      // 
      this.group_extra_d3d.AccessibleDescription = resources.GetString("group_extra_d3d.AccessibleDescription");
      this.group_extra_d3d.AccessibleName = resources.GetString("group_extra_d3d.AccessibleName");
      this.group_extra_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("group_extra_d3d.Anchor")));
      this.group_extra_d3d.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("group_extra_d3d.BackgroundImage")));
      this.group_extra_d3d.Controls.Add(this.label_extra_combo_d3d_8);
      this.group_extra_d3d.Controls.Add(this.combo_extra_curr_d3d_8);
      this.group_extra_d3d.Controls.Add(this.combo_extra_prof_d3d_8);
      this.group_extra_d3d.Controls.Add(this.label_extra_combo_d3d_7);
      this.group_extra_d3d.Controls.Add(this.combo_extra_curr_d3d_7);
      this.group_extra_d3d.Controls.Add(this.combo_extra_prof_d3d_7);
      this.group_extra_d3d.Controls.Add(this.label_extra_combo_d3d_1);
      this.group_extra_d3d.Controls.Add(this.label_extra_combo_d3d_4);
      this.group_extra_d3d.Controls.Add(this.label_extra_combo_d3d_5);
      this.group_extra_d3d.Controls.Add(this.label_extra_combo_d3d_3);
      this.group_extra_d3d.Controls.Add(this.label_extra_combo_d3d_2);
      this.group_extra_d3d.Controls.Add(this.label_extra_combo_d3d_6);
      this.group_extra_d3d.Controls.Add(this.combo_extra_curr_d3d_1);
      this.group_extra_d3d.Controls.Add(this.combo_extra_curr_d3d_2);
      this.group_extra_d3d.Controls.Add(this.combo_extra_curr_d3d_3);
      this.group_extra_d3d.Controls.Add(this.combo_extra_curr_d3d_4);
      this.group_extra_d3d.Controls.Add(this.combo_extra_curr_d3d_5);
      this.group_extra_d3d.Controls.Add(this.combo_extra_curr_d3d_6);
      this.group_extra_d3d.Controls.Add(this.label_extra_curr_d3d);
      this.group_extra_d3d.Controls.Add(this.combo_extra_prof_d3d_3);
      this.group_extra_d3d.Controls.Add(this.label_extra_prof_d3d);
      this.group_extra_d3d.Controls.Add(this.combo_extra_prof_d3d_4);
      this.group_extra_d3d.Controls.Add(this.combo_extra_prof_d3d_5);
      this.group_extra_d3d.Controls.Add(this.combo_extra_prof_d3d_1);
      this.group_extra_d3d.Controls.Add(this.combo_extra_prof_d3d_6);
      this.group_extra_d3d.Controls.Add(this.combo_extra_prof_d3d_2);
      this.group_extra_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("group_extra_d3d.Dock")));
      this.group_extra_d3d.Enabled = ((bool)(resources.GetObject("group_extra_d3d.Enabled")));
      this.group_extra_d3d.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.group_extra_d3d.Font = ((System.Drawing.Font)(resources.GetObject("group_extra_d3d.Font")));
      this.group_extra_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("group_extra_d3d.ImeMode")));
      this.group_extra_d3d.Location = ((System.Drawing.Point)(resources.GetObject("group_extra_d3d.Location")));
      this.group_extra_d3d.Name = "group_extra_d3d";
      this.group_extra_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("group_extra_d3d.RightToLeft")));
      this.group_extra_d3d.Size = ((System.Drawing.Size)(resources.GetObject("group_extra_d3d.Size")));
      this.group_extra_d3d.TabIndex = ((int)(resources.GetObject("group_extra_d3d.TabIndex")));
      this.group_extra_d3d.TabStop = false;
      this.group_extra_d3d.Text = resources.GetString("group_extra_d3d.Text");
      this.toolTip.SetToolTip(this.group_extra_d3d, resources.GetString("group_extra_d3d.ToolTip"));
      this.group_extra_d3d.Visible = ((bool)(resources.GetObject("group_extra_d3d.Visible")));
      // 
      // label_extra_combo_d3d_8
      // 
      this.label_extra_combo_d3d_8.AccessibleDescription = resources.GetString("label_extra_combo_d3d_8.AccessibleDescription");
      this.label_extra_combo_d3d_8.AccessibleName = resources.GetString("label_extra_combo_d3d_8.AccessibleName");
      this.label_extra_combo_d3d_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_d3d_8.Anchor")));
      this.label_extra_combo_d3d_8.AutoSize = ((bool)(resources.GetObject("label_extra_combo_d3d_8.AutoSize")));
      this.label_extra_combo_d3d_8.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_d3d_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_d3d_8.Dock")));
      this.label_extra_combo_d3d_8.Enabled = ((bool)(resources.GetObject("label_extra_combo_d3d_8.Enabled")));
      this.label_extra_combo_d3d_8.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_d3d_8.Font")));
      this.label_extra_combo_d3d_8.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_d3d_8.Image")));
      this.label_extra_combo_d3d_8.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_8.ImageAlign")));
      this.label_extra_combo_d3d_8.ImageIndex = ((int)(resources.GetObject("label_extra_combo_d3d_8.ImageIndex")));
      this.label_extra_combo_d3d_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_d3d_8.ImeMode")));
      this.label_extra_combo_d3d_8.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_d3d_8.Location")));
      this.label_extra_combo_d3d_8.Name = "label_extra_combo_d3d_8";
      this.label_extra_combo_d3d_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_d3d_8.RightToLeft")));
      this.label_extra_combo_d3d_8.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_d3d_8.Size")));
      this.label_extra_combo_d3d_8.TabIndex = ((int)(resources.GetObject("label_extra_combo_d3d_8.TabIndex")));
      this.label_extra_combo_d3d_8.Tag = "5";
      this.label_extra_combo_d3d_8.Text = resources.GetString("label_extra_combo_d3d_8.Text");
      this.label_extra_combo_d3d_8.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_8.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_d3d_8, resources.GetString("label_extra_combo_d3d_8.ToolTip"));
      this.label_extra_combo_d3d_8.Visible = ((bool)(resources.GetObject("label_extra_combo_d3d_8.Visible")));
      this.label_extra_combo_d3d_8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // combo_extra_curr_d3d_8
      // 
      this.combo_extra_curr_d3d_8.AccessibleDescription = resources.GetString("combo_extra_curr_d3d_8.AccessibleDescription");
      this.combo_extra_curr_d3d_8.AccessibleName = resources.GetString("combo_extra_curr_d3d_8.AccessibleName");
      this.combo_extra_curr_d3d_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_d3d_8.Anchor")));
      this.combo_extra_curr_d3d_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_d3d_8.BackgroundImage")));
      this.combo_extra_curr_d3d_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_d3d_8.Dock")));
      this.combo_extra_curr_d3d_8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_d3d_8.Enabled = ((bool)(resources.GetObject("combo_extra_curr_d3d_8.Enabled")));
      this.combo_extra_curr_d3d_8.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_d3d_8.Font")));
      this.combo_extra_curr_d3d_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_d3d_8.ImeMode")));
      this.combo_extra_curr_d3d_8.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_d3d_8.IntegralHeight")));
      this.combo_extra_curr_d3d_8.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_d3d_8.ItemHeight")));
      this.combo_extra_curr_d3d_8.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_d3d_8.Location")));
      this.combo_extra_curr_d3d_8.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_d3d_8.MaxDropDownItems")));
      this.combo_extra_curr_d3d_8.MaxLength = ((int)(resources.GetObject("combo_extra_curr_d3d_8.MaxLength")));
      this.combo_extra_curr_d3d_8.Name = "combo_extra_curr_d3d_8";
      this.combo_extra_curr_d3d_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_d3d_8.RightToLeft")));
      this.combo_extra_curr_d3d_8.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_d3d_8.Size")));
      this.combo_extra_curr_d3d_8.TabIndex = ((int)(resources.GetObject("combo_extra_curr_d3d_8.TabIndex")));
      this.combo_extra_curr_d3d_8.Text = resources.GetString("combo_extra_curr_d3d_8.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_d3d_8, resources.GetString("combo_extra_curr_d3d_8.ToolTip"));
      this.combo_extra_curr_d3d_8.Visible = ((bool)(resources.GetObject("combo_extra_curr_d3d_8.Visible")));
      // 
      // combo_extra_prof_d3d_8
      // 
      this.combo_extra_prof_d3d_8.AccessibleDescription = resources.GetString("combo_extra_prof_d3d_8.AccessibleDescription");
      this.combo_extra_prof_d3d_8.AccessibleName = resources.GetString("combo_extra_prof_d3d_8.AccessibleName");
      this.combo_extra_prof_d3d_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_d3d_8.Anchor")));
      this.combo_extra_prof_d3d_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_d3d_8.BackgroundImage")));
      this.combo_extra_prof_d3d_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_d3d_8.Dock")));
      this.combo_extra_prof_d3d_8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_d3d_8.Enabled = ((bool)(resources.GetObject("combo_extra_prof_d3d_8.Enabled")));
      this.combo_extra_prof_d3d_8.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_d3d_8.Font")));
      this.combo_extra_prof_d3d_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_d3d_8.ImeMode")));
      this.combo_extra_prof_d3d_8.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_d3d_8.IntegralHeight")));
      this.combo_extra_prof_d3d_8.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_d3d_8.ItemHeight")));
      this.combo_extra_prof_d3d_8.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_d3d_8.Location")));
      this.combo_extra_prof_d3d_8.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_d3d_8.MaxDropDownItems")));
      this.combo_extra_prof_d3d_8.MaxLength = ((int)(resources.GetObject("combo_extra_prof_d3d_8.MaxLength")));
      this.combo_extra_prof_d3d_8.Name = "combo_extra_prof_d3d_8";
      this.combo_extra_prof_d3d_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_d3d_8.RightToLeft")));
      this.combo_extra_prof_d3d_8.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_d3d_8.Size")));
      this.combo_extra_prof_d3d_8.TabIndex = ((int)(resources.GetObject("combo_extra_prof_d3d_8.TabIndex")));
      this.combo_extra_prof_d3d_8.Text = resources.GetString("combo_extra_prof_d3d_8.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_d3d_8, resources.GetString("combo_extra_prof_d3d_8.ToolTip"));
      this.combo_extra_prof_d3d_8.Visible = ((bool)(resources.GetObject("combo_extra_prof_d3d_8.Visible")));
      // 
      // label_extra_combo_d3d_7
      // 
      this.label_extra_combo_d3d_7.AccessibleDescription = resources.GetString("label_extra_combo_d3d_7.AccessibleDescription");
      this.label_extra_combo_d3d_7.AccessibleName = resources.GetString("label_extra_combo_d3d_7.AccessibleName");
      this.label_extra_combo_d3d_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_d3d_7.Anchor")));
      this.label_extra_combo_d3d_7.AutoSize = ((bool)(resources.GetObject("label_extra_combo_d3d_7.AutoSize")));
      this.label_extra_combo_d3d_7.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_d3d_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_d3d_7.Dock")));
      this.label_extra_combo_d3d_7.Enabled = ((bool)(resources.GetObject("label_extra_combo_d3d_7.Enabled")));
      this.label_extra_combo_d3d_7.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_d3d_7.Font")));
      this.label_extra_combo_d3d_7.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_d3d_7.Image")));
      this.label_extra_combo_d3d_7.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_7.ImageAlign")));
      this.label_extra_combo_d3d_7.ImageIndex = ((int)(resources.GetObject("label_extra_combo_d3d_7.ImageIndex")));
      this.label_extra_combo_d3d_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_d3d_7.ImeMode")));
      this.label_extra_combo_d3d_7.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_d3d_7.Location")));
      this.label_extra_combo_d3d_7.Name = "label_extra_combo_d3d_7";
      this.label_extra_combo_d3d_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_d3d_7.RightToLeft")));
      this.label_extra_combo_d3d_7.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_d3d_7.Size")));
      this.label_extra_combo_d3d_7.TabIndex = ((int)(resources.GetObject("label_extra_combo_d3d_7.TabIndex")));
      this.label_extra_combo_d3d_7.Tag = "5";
      this.label_extra_combo_d3d_7.Text = resources.GetString("label_extra_combo_d3d_7.Text");
      this.label_extra_combo_d3d_7.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_7.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_d3d_7, resources.GetString("label_extra_combo_d3d_7.ToolTip"));
      this.label_extra_combo_d3d_7.Visible = ((bool)(resources.GetObject("label_extra_combo_d3d_7.Visible")));
      this.label_extra_combo_d3d_7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // combo_extra_curr_d3d_7
      // 
      this.combo_extra_curr_d3d_7.AccessibleDescription = resources.GetString("combo_extra_curr_d3d_7.AccessibleDescription");
      this.combo_extra_curr_d3d_7.AccessibleName = resources.GetString("combo_extra_curr_d3d_7.AccessibleName");
      this.combo_extra_curr_d3d_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_d3d_7.Anchor")));
      this.combo_extra_curr_d3d_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_d3d_7.BackgroundImage")));
      this.combo_extra_curr_d3d_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_d3d_7.Dock")));
      this.combo_extra_curr_d3d_7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_d3d_7.Enabled = ((bool)(resources.GetObject("combo_extra_curr_d3d_7.Enabled")));
      this.combo_extra_curr_d3d_7.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_d3d_7.Font")));
      this.combo_extra_curr_d3d_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_d3d_7.ImeMode")));
      this.combo_extra_curr_d3d_7.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_d3d_7.IntegralHeight")));
      this.combo_extra_curr_d3d_7.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_d3d_7.ItemHeight")));
      this.combo_extra_curr_d3d_7.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_d3d_7.Location")));
      this.combo_extra_curr_d3d_7.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_d3d_7.MaxDropDownItems")));
      this.combo_extra_curr_d3d_7.MaxLength = ((int)(resources.GetObject("combo_extra_curr_d3d_7.MaxLength")));
      this.combo_extra_curr_d3d_7.Name = "combo_extra_curr_d3d_7";
      this.combo_extra_curr_d3d_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_d3d_7.RightToLeft")));
      this.combo_extra_curr_d3d_7.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_d3d_7.Size")));
      this.combo_extra_curr_d3d_7.TabIndex = ((int)(resources.GetObject("combo_extra_curr_d3d_7.TabIndex")));
      this.combo_extra_curr_d3d_7.Text = resources.GetString("combo_extra_curr_d3d_7.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_d3d_7, resources.GetString("combo_extra_curr_d3d_7.ToolTip"));
      this.combo_extra_curr_d3d_7.Visible = ((bool)(resources.GetObject("combo_extra_curr_d3d_7.Visible")));
      // 
      // combo_extra_prof_d3d_7
      // 
      this.combo_extra_prof_d3d_7.AccessibleDescription = resources.GetString("combo_extra_prof_d3d_7.AccessibleDescription");
      this.combo_extra_prof_d3d_7.AccessibleName = resources.GetString("combo_extra_prof_d3d_7.AccessibleName");
      this.combo_extra_prof_d3d_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_d3d_7.Anchor")));
      this.combo_extra_prof_d3d_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_d3d_7.BackgroundImage")));
      this.combo_extra_prof_d3d_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_d3d_7.Dock")));
      this.combo_extra_prof_d3d_7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_d3d_7.Enabled = ((bool)(resources.GetObject("combo_extra_prof_d3d_7.Enabled")));
      this.combo_extra_prof_d3d_7.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_d3d_7.Font")));
      this.combo_extra_prof_d3d_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_d3d_7.ImeMode")));
      this.combo_extra_prof_d3d_7.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_d3d_7.IntegralHeight")));
      this.combo_extra_prof_d3d_7.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_d3d_7.ItemHeight")));
      this.combo_extra_prof_d3d_7.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_d3d_7.Location")));
      this.combo_extra_prof_d3d_7.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_d3d_7.MaxDropDownItems")));
      this.combo_extra_prof_d3d_7.MaxLength = ((int)(resources.GetObject("combo_extra_prof_d3d_7.MaxLength")));
      this.combo_extra_prof_d3d_7.Name = "combo_extra_prof_d3d_7";
      this.combo_extra_prof_d3d_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_d3d_7.RightToLeft")));
      this.combo_extra_prof_d3d_7.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_d3d_7.Size")));
      this.combo_extra_prof_d3d_7.TabIndex = ((int)(resources.GetObject("combo_extra_prof_d3d_7.TabIndex")));
      this.combo_extra_prof_d3d_7.Text = resources.GetString("combo_extra_prof_d3d_7.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_d3d_7, resources.GetString("combo_extra_prof_d3d_7.ToolTip"));
      this.combo_extra_prof_d3d_7.Visible = ((bool)(resources.GetObject("combo_extra_prof_d3d_7.Visible")));
      // 
      // label_extra_combo_d3d_1
      // 
      this.label_extra_combo_d3d_1.AccessibleDescription = resources.GetString("label_extra_combo_d3d_1.AccessibleDescription");
      this.label_extra_combo_d3d_1.AccessibleName = resources.GetString("label_extra_combo_d3d_1.AccessibleName");
      this.label_extra_combo_d3d_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_d3d_1.Anchor")));
      this.label_extra_combo_d3d_1.AutoSize = ((bool)(resources.GetObject("label_extra_combo_d3d_1.AutoSize")));
      this.label_extra_combo_d3d_1.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_d3d_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_d3d_1.Dock")));
      this.label_extra_combo_d3d_1.Enabled = ((bool)(resources.GetObject("label_extra_combo_d3d_1.Enabled")));
      this.label_extra_combo_d3d_1.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_d3d_1.Font")));
      this.label_extra_combo_d3d_1.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_d3d_1.Image")));
      this.label_extra_combo_d3d_1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_1.ImageAlign")));
      this.label_extra_combo_d3d_1.ImageIndex = ((int)(resources.GetObject("label_extra_combo_d3d_1.ImageIndex")));
      this.label_extra_combo_d3d_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_d3d_1.ImeMode")));
      this.label_extra_combo_d3d_1.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_d3d_1.Location")));
      this.label_extra_combo_d3d_1.Name = "label_extra_combo_d3d_1";
      this.label_extra_combo_d3d_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_d3d_1.RightToLeft")));
      this.label_extra_combo_d3d_1.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_d3d_1.Size")));
      this.label_extra_combo_d3d_1.TabIndex = ((int)(resources.GetObject("label_extra_combo_d3d_1.TabIndex")));
      this.label_extra_combo_d3d_1.Text = resources.GetString("label_extra_combo_d3d_1.Text");
      this.label_extra_combo_d3d_1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_1.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_d3d_1, resources.GetString("label_extra_combo_d3d_1.ToolTip"));
      this.label_extra_combo_d3d_1.Visible = ((bool)(resources.GetObject("label_extra_combo_d3d_1.Visible")));
      this.label_extra_combo_d3d_1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_combo_d3d_4
      // 
      this.label_extra_combo_d3d_4.AccessibleDescription = resources.GetString("label_extra_combo_d3d_4.AccessibleDescription");
      this.label_extra_combo_d3d_4.AccessibleName = resources.GetString("label_extra_combo_d3d_4.AccessibleName");
      this.label_extra_combo_d3d_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_d3d_4.Anchor")));
      this.label_extra_combo_d3d_4.AutoSize = ((bool)(resources.GetObject("label_extra_combo_d3d_4.AutoSize")));
      this.label_extra_combo_d3d_4.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_d3d_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_d3d_4.Dock")));
      this.label_extra_combo_d3d_4.Enabled = ((bool)(resources.GetObject("label_extra_combo_d3d_4.Enabled")));
      this.label_extra_combo_d3d_4.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_d3d_4.Font")));
      this.label_extra_combo_d3d_4.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_d3d_4.Image")));
      this.label_extra_combo_d3d_4.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_4.ImageAlign")));
      this.label_extra_combo_d3d_4.ImageIndex = ((int)(resources.GetObject("label_extra_combo_d3d_4.ImageIndex")));
      this.label_extra_combo_d3d_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_d3d_4.ImeMode")));
      this.label_extra_combo_d3d_4.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_d3d_4.Location")));
      this.label_extra_combo_d3d_4.Name = "label_extra_combo_d3d_4";
      this.label_extra_combo_d3d_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_d3d_4.RightToLeft")));
      this.label_extra_combo_d3d_4.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_d3d_4.Size")));
      this.label_extra_combo_d3d_4.TabIndex = ((int)(resources.GetObject("label_extra_combo_d3d_4.TabIndex")));
      this.label_extra_combo_d3d_4.Tag = "3";
      this.label_extra_combo_d3d_4.Text = resources.GetString("label_extra_combo_d3d_4.Text");
      this.label_extra_combo_d3d_4.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_4.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_d3d_4, resources.GetString("label_extra_combo_d3d_4.ToolTip"));
      this.label_extra_combo_d3d_4.Visible = ((bool)(resources.GetObject("label_extra_combo_d3d_4.Visible")));
      this.label_extra_combo_d3d_4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_combo_d3d_5
      // 
      this.label_extra_combo_d3d_5.AccessibleDescription = resources.GetString("label_extra_combo_d3d_5.AccessibleDescription");
      this.label_extra_combo_d3d_5.AccessibleName = resources.GetString("label_extra_combo_d3d_5.AccessibleName");
      this.label_extra_combo_d3d_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_d3d_5.Anchor")));
      this.label_extra_combo_d3d_5.AutoSize = ((bool)(resources.GetObject("label_extra_combo_d3d_5.AutoSize")));
      this.label_extra_combo_d3d_5.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_d3d_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_d3d_5.Dock")));
      this.label_extra_combo_d3d_5.Enabled = ((bool)(resources.GetObject("label_extra_combo_d3d_5.Enabled")));
      this.label_extra_combo_d3d_5.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_d3d_5.Font")));
      this.label_extra_combo_d3d_5.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_d3d_5.Image")));
      this.label_extra_combo_d3d_5.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_5.ImageAlign")));
      this.label_extra_combo_d3d_5.ImageIndex = ((int)(resources.GetObject("label_extra_combo_d3d_5.ImageIndex")));
      this.label_extra_combo_d3d_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_d3d_5.ImeMode")));
      this.label_extra_combo_d3d_5.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_d3d_5.Location")));
      this.label_extra_combo_d3d_5.Name = "label_extra_combo_d3d_5";
      this.label_extra_combo_d3d_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_d3d_5.RightToLeft")));
      this.label_extra_combo_d3d_5.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_d3d_5.Size")));
      this.label_extra_combo_d3d_5.TabIndex = ((int)(resources.GetObject("label_extra_combo_d3d_5.TabIndex")));
      this.label_extra_combo_d3d_5.Tag = "4";
      this.label_extra_combo_d3d_5.Text = resources.GetString("label_extra_combo_d3d_5.Text");
      this.label_extra_combo_d3d_5.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_5.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_d3d_5, resources.GetString("label_extra_combo_d3d_5.ToolTip"));
      this.label_extra_combo_d3d_5.Visible = ((bool)(resources.GetObject("label_extra_combo_d3d_5.Visible")));
      this.label_extra_combo_d3d_5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_combo_d3d_3
      // 
      this.label_extra_combo_d3d_3.AccessibleDescription = resources.GetString("label_extra_combo_d3d_3.AccessibleDescription");
      this.label_extra_combo_d3d_3.AccessibleName = resources.GetString("label_extra_combo_d3d_3.AccessibleName");
      this.label_extra_combo_d3d_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_d3d_3.Anchor")));
      this.label_extra_combo_d3d_3.AutoSize = ((bool)(resources.GetObject("label_extra_combo_d3d_3.AutoSize")));
      this.label_extra_combo_d3d_3.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_d3d_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_d3d_3.Dock")));
      this.label_extra_combo_d3d_3.Enabled = ((bool)(resources.GetObject("label_extra_combo_d3d_3.Enabled")));
      this.label_extra_combo_d3d_3.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_d3d_3.Font")));
      this.label_extra_combo_d3d_3.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_d3d_3.Image")));
      this.label_extra_combo_d3d_3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_3.ImageAlign")));
      this.label_extra_combo_d3d_3.ImageIndex = ((int)(resources.GetObject("label_extra_combo_d3d_3.ImageIndex")));
      this.label_extra_combo_d3d_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_d3d_3.ImeMode")));
      this.label_extra_combo_d3d_3.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_d3d_3.Location")));
      this.label_extra_combo_d3d_3.Name = "label_extra_combo_d3d_3";
      this.label_extra_combo_d3d_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_d3d_3.RightToLeft")));
      this.label_extra_combo_d3d_3.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_d3d_3.Size")));
      this.label_extra_combo_d3d_3.TabIndex = ((int)(resources.GetObject("label_extra_combo_d3d_3.TabIndex")));
      this.label_extra_combo_d3d_3.Tag = "2";
      this.label_extra_combo_d3d_3.Text = resources.GetString("label_extra_combo_d3d_3.Text");
      this.label_extra_combo_d3d_3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_3.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_d3d_3, resources.GetString("label_extra_combo_d3d_3.ToolTip"));
      this.label_extra_combo_d3d_3.Visible = ((bool)(resources.GetObject("label_extra_combo_d3d_3.Visible")));
      this.label_extra_combo_d3d_3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_combo_d3d_2
      // 
      this.label_extra_combo_d3d_2.AccessibleDescription = resources.GetString("label_extra_combo_d3d_2.AccessibleDescription");
      this.label_extra_combo_d3d_2.AccessibleName = resources.GetString("label_extra_combo_d3d_2.AccessibleName");
      this.label_extra_combo_d3d_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_d3d_2.Anchor")));
      this.label_extra_combo_d3d_2.AutoSize = ((bool)(resources.GetObject("label_extra_combo_d3d_2.AutoSize")));
      this.label_extra_combo_d3d_2.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_d3d_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_d3d_2.Dock")));
      this.label_extra_combo_d3d_2.Enabled = ((bool)(resources.GetObject("label_extra_combo_d3d_2.Enabled")));
      this.label_extra_combo_d3d_2.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_d3d_2.Font")));
      this.label_extra_combo_d3d_2.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_d3d_2.Image")));
      this.label_extra_combo_d3d_2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_2.ImageAlign")));
      this.label_extra_combo_d3d_2.ImageIndex = ((int)(resources.GetObject("label_extra_combo_d3d_2.ImageIndex")));
      this.label_extra_combo_d3d_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_d3d_2.ImeMode")));
      this.label_extra_combo_d3d_2.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_d3d_2.Location")));
      this.label_extra_combo_d3d_2.Name = "label_extra_combo_d3d_2";
      this.label_extra_combo_d3d_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_d3d_2.RightToLeft")));
      this.label_extra_combo_d3d_2.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_d3d_2.Size")));
      this.label_extra_combo_d3d_2.TabIndex = ((int)(resources.GetObject("label_extra_combo_d3d_2.TabIndex")));
      this.label_extra_combo_d3d_2.Tag = "1";
      this.label_extra_combo_d3d_2.Text = resources.GetString("label_extra_combo_d3d_2.Text");
      this.label_extra_combo_d3d_2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_2.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_d3d_2, resources.GetString("label_extra_combo_d3d_2.ToolTip"));
      this.label_extra_combo_d3d_2.Visible = ((bool)(resources.GetObject("label_extra_combo_d3d_2.Visible")));
      this.label_extra_combo_d3d_2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_combo_d3d_6
      // 
      this.label_extra_combo_d3d_6.AccessibleDescription = resources.GetString("label_extra_combo_d3d_6.AccessibleDescription");
      this.label_extra_combo_d3d_6.AccessibleName = resources.GetString("label_extra_combo_d3d_6.AccessibleName");
      this.label_extra_combo_d3d_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_d3d_6.Anchor")));
      this.label_extra_combo_d3d_6.AutoSize = ((bool)(resources.GetObject("label_extra_combo_d3d_6.AutoSize")));
      this.label_extra_combo_d3d_6.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_d3d_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_d3d_6.Dock")));
      this.label_extra_combo_d3d_6.Enabled = ((bool)(resources.GetObject("label_extra_combo_d3d_6.Enabled")));
      this.label_extra_combo_d3d_6.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_d3d_6.Font")));
      this.label_extra_combo_d3d_6.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_d3d_6.Image")));
      this.label_extra_combo_d3d_6.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_6.ImageAlign")));
      this.label_extra_combo_d3d_6.ImageIndex = ((int)(resources.GetObject("label_extra_combo_d3d_6.ImageIndex")));
      this.label_extra_combo_d3d_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_d3d_6.ImeMode")));
      this.label_extra_combo_d3d_6.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_d3d_6.Location")));
      this.label_extra_combo_d3d_6.Name = "label_extra_combo_d3d_6";
      this.label_extra_combo_d3d_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_d3d_6.RightToLeft")));
      this.label_extra_combo_d3d_6.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_d3d_6.Size")));
      this.label_extra_combo_d3d_6.TabIndex = ((int)(resources.GetObject("label_extra_combo_d3d_6.TabIndex")));
      this.label_extra_combo_d3d_6.Tag = "5";
      this.label_extra_combo_d3d_6.Text = resources.GetString("label_extra_combo_d3d_6.Text");
      this.label_extra_combo_d3d_6.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_d3d_6.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_d3d_6, resources.GetString("label_extra_combo_d3d_6.ToolTip"));
      this.label_extra_combo_d3d_6.Visible = ((bool)(resources.GetObject("label_extra_combo_d3d_6.Visible")));
      this.label_extra_combo_d3d_6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_curr_d3d
      // 
      this.label_extra_curr_d3d.AccessibleDescription = resources.GetString("label_extra_curr_d3d.AccessibleDescription");
      this.label_extra_curr_d3d.AccessibleName = resources.GetString("label_extra_curr_d3d.AccessibleName");
      this.label_extra_curr_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_curr_d3d.Anchor")));
      this.label_extra_curr_d3d.AutoSize = ((bool)(resources.GetObject("label_extra_curr_d3d.AutoSize")));
      this.label_extra_curr_d3d.CausesValidation = false;
      this.label_extra_curr_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_curr_d3d.Dock")));
      this.label_extra_curr_d3d.Enabled = ((bool)(resources.GetObject("label_extra_curr_d3d.Enabled")));
      this.label_extra_curr_d3d.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_curr_d3d.Font")));
      this.label_extra_curr_d3d.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_curr_d3d.Image")));
      this.label_extra_curr_d3d.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_curr_d3d.ImageAlign")));
      this.label_extra_curr_d3d.ImageIndex = ((int)(resources.GetObject("label_extra_curr_d3d.ImageIndex")));
      this.label_extra_curr_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_curr_d3d.ImeMode")));
      this.label_extra_curr_d3d.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_curr_d3d.Location")));
      this.label_extra_curr_d3d.Name = "label_extra_curr_d3d";
      this.label_extra_curr_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_curr_d3d.RightToLeft")));
      this.label_extra_curr_d3d.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_curr_d3d.Size")));
      this.label_extra_curr_d3d.TabIndex = ((int)(resources.GetObject("label_extra_curr_d3d.TabIndex")));
      this.label_extra_curr_d3d.Text = resources.GetString("label_extra_curr_d3d.Text");
      this.label_extra_curr_d3d.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_curr_d3d.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_curr_d3d, resources.GetString("label_extra_curr_d3d.ToolTip"));
      this.label_extra_curr_d3d.Visible = ((bool)(resources.GetObject("label_extra_curr_d3d.Visible")));
      // 
      // label_extra_prof_d3d
      // 
      this.label_extra_prof_d3d.AccessibleDescription = resources.GetString("label_extra_prof_d3d.AccessibleDescription");
      this.label_extra_prof_d3d.AccessibleName = resources.GetString("label_extra_prof_d3d.AccessibleName");
      this.label_extra_prof_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_prof_d3d.Anchor")));
      this.label_extra_prof_d3d.AutoSize = ((bool)(resources.GetObject("label_extra_prof_d3d.AutoSize")));
      this.label_extra_prof_d3d.CausesValidation = false;
      this.label_extra_prof_d3d.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_prof_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_prof_d3d.Dock")));
      this.label_extra_prof_d3d.Enabled = ((bool)(resources.GetObject("label_extra_prof_d3d.Enabled")));
      this.label_extra_prof_d3d.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_prof_d3d.Font")));
      this.label_extra_prof_d3d.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_prof_d3d.Image")));
      this.label_extra_prof_d3d.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_prof_d3d.ImageAlign")));
      this.label_extra_prof_d3d.ImageIndex = ((int)(resources.GetObject("label_extra_prof_d3d.ImageIndex")));
      this.label_extra_prof_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_prof_d3d.ImeMode")));
      this.label_extra_prof_d3d.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_prof_d3d.Location")));
      this.label_extra_prof_d3d.Name = "label_extra_prof_d3d";
      this.label_extra_prof_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_prof_d3d.RightToLeft")));
      this.label_extra_prof_d3d.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_prof_d3d.Size")));
      this.label_extra_prof_d3d.TabIndex = ((int)(resources.GetObject("label_extra_prof_d3d.TabIndex")));
      this.label_extra_prof_d3d.Text = resources.GetString("label_extra_prof_d3d.Text");
      this.label_extra_prof_d3d.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_prof_d3d.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_prof_d3d, resources.GetString("label_extra_prof_d3d.ToolTip"));
      this.label_extra_prof_d3d.Visible = ((bool)(resources.GetObject("label_extra_prof_d3d.Visible")));
      this.label_extra_prof_d3d.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_d3d_MouseDown);
      // 
      // combo_extra2_curr_d3d_8
      // 
      this.combo_extra2_curr_d3d_8.AccessibleDescription = resources.GetString("combo_extra2_curr_d3d_8.AccessibleDescription");
      this.combo_extra2_curr_d3d_8.AccessibleName = resources.GetString("combo_extra2_curr_d3d_8.AccessibleName");
      this.combo_extra2_curr_d3d_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_d3d_8.Anchor")));
      this.combo_extra2_curr_d3d_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_d3d_8.BackgroundImage")));
      this.combo_extra2_curr_d3d_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_d3d_8.Dock")));
      this.combo_extra2_curr_d3d_8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_d3d_8.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_d3d_8.Enabled")));
      this.combo_extra2_curr_d3d_8.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_d3d_8.Font")));
      this.combo_extra2_curr_d3d_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_d3d_8.ImeMode")));
      this.combo_extra2_curr_d3d_8.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_d3d_8.IntegralHeight")));
      this.combo_extra2_curr_d3d_8.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_d3d_8.ItemHeight")));
      this.combo_extra2_curr_d3d_8.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_d3d_8.Location")));
      this.combo_extra2_curr_d3d_8.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_d3d_8.MaxDropDownItems")));
      this.combo_extra2_curr_d3d_8.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_d3d_8.MaxLength")));
      this.combo_extra2_curr_d3d_8.Name = "combo_extra2_curr_d3d_8";
      this.combo_extra2_curr_d3d_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_d3d_8.RightToLeft")));
      this.combo_extra2_curr_d3d_8.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_d3d_8.Size")));
      this.combo_extra2_curr_d3d_8.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_d3d_8.TabIndex")));
      this.combo_extra2_curr_d3d_8.Text = resources.GetString("combo_extra2_curr_d3d_8.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_d3d_8, resources.GetString("combo_extra2_curr_d3d_8.ToolTip"));
      this.combo_extra2_curr_d3d_8.Visible = ((bool)(resources.GetObject("combo_extra2_curr_d3d_8.Visible")));
      // 
      // combo_extra2_prof_d3d_8
      // 
      this.combo_extra2_prof_d3d_8.AccessibleDescription = resources.GetString("combo_extra2_prof_d3d_8.AccessibleDescription");
      this.combo_extra2_prof_d3d_8.AccessibleName = resources.GetString("combo_extra2_prof_d3d_8.AccessibleName");
      this.combo_extra2_prof_d3d_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_d3d_8.Anchor")));
      this.combo_extra2_prof_d3d_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_d3d_8.BackgroundImage")));
      this.combo_extra2_prof_d3d_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_d3d_8.Dock")));
      this.combo_extra2_prof_d3d_8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_d3d_8.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_d3d_8.Enabled")));
      this.combo_extra2_prof_d3d_8.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_d3d_8.Font")));
      this.combo_extra2_prof_d3d_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_d3d_8.ImeMode")));
      this.combo_extra2_prof_d3d_8.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_d3d_8.IntegralHeight")));
      this.combo_extra2_prof_d3d_8.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_d3d_8.ItemHeight")));
      this.combo_extra2_prof_d3d_8.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_d3d_8.Location")));
      this.combo_extra2_prof_d3d_8.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_d3d_8.MaxDropDownItems")));
      this.combo_extra2_prof_d3d_8.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_d3d_8.MaxLength")));
      this.combo_extra2_prof_d3d_8.Name = "combo_extra2_prof_d3d_8";
      this.combo_extra2_prof_d3d_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_d3d_8.RightToLeft")));
      this.combo_extra2_prof_d3d_8.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_d3d_8.Size")));
      this.combo_extra2_prof_d3d_8.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_d3d_8.TabIndex")));
      this.combo_extra2_prof_d3d_8.Text = resources.GetString("combo_extra2_prof_d3d_8.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_d3d_8, resources.GetString("combo_extra2_prof_d3d_8.ToolTip"));
      this.combo_extra2_prof_d3d_8.Visible = ((bool)(resources.GetObject("combo_extra2_prof_d3d_8.Visible")));
      // 
      // label_extra2_combo_d3d_8
      // 
      this.label_extra2_combo_d3d_8.AccessibleDescription = resources.GetString("label_extra2_combo_d3d_8.AccessibleDescription");
      this.label_extra2_combo_d3d_8.AccessibleName = resources.GetString("label_extra2_combo_d3d_8.AccessibleName");
      this.label_extra2_combo_d3d_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_d3d_8.Anchor")));
      this.label_extra2_combo_d3d_8.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_d3d_8.AutoSize")));
      this.label_extra2_combo_d3d_8.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_d3d_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_d3d_8.Dock")));
      this.label_extra2_combo_d3d_8.Enabled = ((bool)(resources.GetObject("label_extra2_combo_d3d_8.Enabled")));
      this.label_extra2_combo_d3d_8.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_d3d_8.Font")));
      this.label_extra2_combo_d3d_8.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_d3d_8.Image")));
      this.label_extra2_combo_d3d_8.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_8.ImageAlign")));
      this.label_extra2_combo_d3d_8.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_8.ImageIndex")));
      this.label_extra2_combo_d3d_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_d3d_8.ImeMode")));
      this.label_extra2_combo_d3d_8.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_d3d_8.Location")));
      this.label_extra2_combo_d3d_8.Name = "label_extra2_combo_d3d_8";
      this.label_extra2_combo_d3d_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_d3d_8.RightToLeft")));
      this.label_extra2_combo_d3d_8.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_d3d_8.Size")));
      this.label_extra2_combo_d3d_8.TabIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_8.TabIndex")));
      this.label_extra2_combo_d3d_8.Tag = "5";
      this.label_extra2_combo_d3d_8.Text = resources.GetString("label_extra2_combo_d3d_8.Text");
      this.label_extra2_combo_d3d_8.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_8.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_d3d_8, resources.GetString("label_extra2_combo_d3d_8.ToolTip"));
      this.label_extra2_combo_d3d_8.Visible = ((bool)(resources.GetObject("label_extra2_combo_d3d_8.Visible")));
      this.label_extra2_combo_d3d_8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // combo_extra2_curr_d3d_7
      // 
      this.combo_extra2_curr_d3d_7.AccessibleDescription = resources.GetString("combo_extra2_curr_d3d_7.AccessibleDescription");
      this.combo_extra2_curr_d3d_7.AccessibleName = resources.GetString("combo_extra2_curr_d3d_7.AccessibleName");
      this.combo_extra2_curr_d3d_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_d3d_7.Anchor")));
      this.combo_extra2_curr_d3d_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_d3d_7.BackgroundImage")));
      this.combo_extra2_curr_d3d_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_d3d_7.Dock")));
      this.combo_extra2_curr_d3d_7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_d3d_7.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_d3d_7.Enabled")));
      this.combo_extra2_curr_d3d_7.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_d3d_7.Font")));
      this.combo_extra2_curr_d3d_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_d3d_7.ImeMode")));
      this.combo_extra2_curr_d3d_7.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_d3d_7.IntegralHeight")));
      this.combo_extra2_curr_d3d_7.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_d3d_7.ItemHeight")));
      this.combo_extra2_curr_d3d_7.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_d3d_7.Location")));
      this.combo_extra2_curr_d3d_7.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_d3d_7.MaxDropDownItems")));
      this.combo_extra2_curr_d3d_7.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_d3d_7.MaxLength")));
      this.combo_extra2_curr_d3d_7.Name = "combo_extra2_curr_d3d_7";
      this.combo_extra2_curr_d3d_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_d3d_7.RightToLeft")));
      this.combo_extra2_curr_d3d_7.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_d3d_7.Size")));
      this.combo_extra2_curr_d3d_7.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_d3d_7.TabIndex")));
      this.combo_extra2_curr_d3d_7.Text = resources.GetString("combo_extra2_curr_d3d_7.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_d3d_7, resources.GetString("combo_extra2_curr_d3d_7.ToolTip"));
      this.combo_extra2_curr_d3d_7.Visible = ((bool)(resources.GetObject("combo_extra2_curr_d3d_7.Visible")));
      // 
      // combo_extra2_prof_d3d_7
      // 
      this.combo_extra2_prof_d3d_7.AccessibleDescription = resources.GetString("combo_extra2_prof_d3d_7.AccessibleDescription");
      this.combo_extra2_prof_d3d_7.AccessibleName = resources.GetString("combo_extra2_prof_d3d_7.AccessibleName");
      this.combo_extra2_prof_d3d_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_d3d_7.Anchor")));
      this.combo_extra2_prof_d3d_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_d3d_7.BackgroundImage")));
      this.combo_extra2_prof_d3d_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_d3d_7.Dock")));
      this.combo_extra2_prof_d3d_7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_d3d_7.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_d3d_7.Enabled")));
      this.combo_extra2_prof_d3d_7.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_d3d_7.Font")));
      this.combo_extra2_prof_d3d_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_d3d_7.ImeMode")));
      this.combo_extra2_prof_d3d_7.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_d3d_7.IntegralHeight")));
      this.combo_extra2_prof_d3d_7.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_d3d_7.ItemHeight")));
      this.combo_extra2_prof_d3d_7.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_d3d_7.Location")));
      this.combo_extra2_prof_d3d_7.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_d3d_7.MaxDropDownItems")));
      this.combo_extra2_prof_d3d_7.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_d3d_7.MaxLength")));
      this.combo_extra2_prof_d3d_7.Name = "combo_extra2_prof_d3d_7";
      this.combo_extra2_prof_d3d_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_d3d_7.RightToLeft")));
      this.combo_extra2_prof_d3d_7.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_d3d_7.Size")));
      this.combo_extra2_prof_d3d_7.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_d3d_7.TabIndex")));
      this.combo_extra2_prof_d3d_7.Text = resources.GetString("combo_extra2_prof_d3d_7.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_d3d_7, resources.GetString("combo_extra2_prof_d3d_7.ToolTip"));
      this.combo_extra2_prof_d3d_7.Visible = ((bool)(resources.GetObject("combo_extra2_prof_d3d_7.Visible")));
      // 
      // label_extra2_combo_d3d_7
      // 
      this.label_extra2_combo_d3d_7.AccessibleDescription = resources.GetString("label_extra2_combo_d3d_7.AccessibleDescription");
      this.label_extra2_combo_d3d_7.AccessibleName = resources.GetString("label_extra2_combo_d3d_7.AccessibleName");
      this.label_extra2_combo_d3d_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_d3d_7.Anchor")));
      this.label_extra2_combo_d3d_7.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_d3d_7.AutoSize")));
      this.label_extra2_combo_d3d_7.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_d3d_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_d3d_7.Dock")));
      this.label_extra2_combo_d3d_7.Enabled = ((bool)(resources.GetObject("label_extra2_combo_d3d_7.Enabled")));
      this.label_extra2_combo_d3d_7.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_d3d_7.Font")));
      this.label_extra2_combo_d3d_7.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_d3d_7.Image")));
      this.label_extra2_combo_d3d_7.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_7.ImageAlign")));
      this.label_extra2_combo_d3d_7.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_7.ImageIndex")));
      this.label_extra2_combo_d3d_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_d3d_7.ImeMode")));
      this.label_extra2_combo_d3d_7.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_d3d_7.Location")));
      this.label_extra2_combo_d3d_7.Name = "label_extra2_combo_d3d_7";
      this.label_extra2_combo_d3d_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_d3d_7.RightToLeft")));
      this.label_extra2_combo_d3d_7.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_d3d_7.Size")));
      this.label_extra2_combo_d3d_7.TabIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_7.TabIndex")));
      this.label_extra2_combo_d3d_7.Tag = "5";
      this.label_extra2_combo_d3d_7.Text = resources.GetString("label_extra2_combo_d3d_7.Text");
      this.label_extra2_combo_d3d_7.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_7.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_d3d_7, resources.GetString("label_extra2_combo_d3d_7.ToolTip"));
      this.label_extra2_combo_d3d_7.Visible = ((bool)(resources.GetObject("label_extra2_combo_d3d_7.Visible")));
      this.label_extra2_combo_d3d_7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_d3d_6
      // 
      this.label_extra2_combo_d3d_6.AccessibleDescription = resources.GetString("label_extra2_combo_d3d_6.AccessibleDescription");
      this.label_extra2_combo_d3d_6.AccessibleName = resources.GetString("label_extra2_combo_d3d_6.AccessibleName");
      this.label_extra2_combo_d3d_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_d3d_6.Anchor")));
      this.label_extra2_combo_d3d_6.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_d3d_6.AutoSize")));
      this.label_extra2_combo_d3d_6.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_d3d_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_d3d_6.Dock")));
      this.label_extra2_combo_d3d_6.Enabled = ((bool)(resources.GetObject("label_extra2_combo_d3d_6.Enabled")));
      this.label_extra2_combo_d3d_6.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_d3d_6.Font")));
      this.label_extra2_combo_d3d_6.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_d3d_6.Image")));
      this.label_extra2_combo_d3d_6.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_6.ImageAlign")));
      this.label_extra2_combo_d3d_6.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_6.ImageIndex")));
      this.label_extra2_combo_d3d_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_d3d_6.ImeMode")));
      this.label_extra2_combo_d3d_6.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_d3d_6.Location")));
      this.label_extra2_combo_d3d_6.Name = "label_extra2_combo_d3d_6";
      this.label_extra2_combo_d3d_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_d3d_6.RightToLeft")));
      this.label_extra2_combo_d3d_6.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_d3d_6.Size")));
      this.label_extra2_combo_d3d_6.TabIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_6.TabIndex")));
      this.label_extra2_combo_d3d_6.Tag = "5";
      this.label_extra2_combo_d3d_6.Text = resources.GetString("label_extra2_combo_d3d_6.Text");
      this.label_extra2_combo_d3d_6.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_6.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_d3d_6, resources.GetString("label_extra2_combo_d3d_6.ToolTip"));
      this.label_extra2_combo_d3d_6.Visible = ((bool)(resources.GetObject("label_extra2_combo_d3d_6.Visible")));
      this.label_extra2_combo_d3d_6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_d3d_5
      // 
      this.label_extra2_combo_d3d_5.AccessibleDescription = resources.GetString("label_extra2_combo_d3d_5.AccessibleDescription");
      this.label_extra2_combo_d3d_5.AccessibleName = resources.GetString("label_extra2_combo_d3d_5.AccessibleName");
      this.label_extra2_combo_d3d_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_d3d_5.Anchor")));
      this.label_extra2_combo_d3d_5.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_d3d_5.AutoSize")));
      this.label_extra2_combo_d3d_5.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_d3d_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_d3d_5.Dock")));
      this.label_extra2_combo_d3d_5.Enabled = ((bool)(resources.GetObject("label_extra2_combo_d3d_5.Enabled")));
      this.label_extra2_combo_d3d_5.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_d3d_5.Font")));
      this.label_extra2_combo_d3d_5.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_d3d_5.Image")));
      this.label_extra2_combo_d3d_5.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_5.ImageAlign")));
      this.label_extra2_combo_d3d_5.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_5.ImageIndex")));
      this.label_extra2_combo_d3d_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_d3d_5.ImeMode")));
      this.label_extra2_combo_d3d_5.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_d3d_5.Location")));
      this.label_extra2_combo_d3d_5.Name = "label_extra2_combo_d3d_5";
      this.label_extra2_combo_d3d_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_d3d_5.RightToLeft")));
      this.label_extra2_combo_d3d_5.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_d3d_5.Size")));
      this.label_extra2_combo_d3d_5.TabIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_5.TabIndex")));
      this.label_extra2_combo_d3d_5.Tag = "4";
      this.label_extra2_combo_d3d_5.Text = resources.GetString("label_extra2_combo_d3d_5.Text");
      this.label_extra2_combo_d3d_5.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_5.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_d3d_5, resources.GetString("label_extra2_combo_d3d_5.ToolTip"));
      this.label_extra2_combo_d3d_5.Visible = ((bool)(resources.GetObject("label_extra2_combo_d3d_5.Visible")));
      this.label_extra2_combo_d3d_5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_d3d_2
      // 
      this.label_extra2_combo_d3d_2.AccessibleDescription = resources.GetString("label_extra2_combo_d3d_2.AccessibleDescription");
      this.label_extra2_combo_d3d_2.AccessibleName = resources.GetString("label_extra2_combo_d3d_2.AccessibleName");
      this.label_extra2_combo_d3d_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_d3d_2.Anchor")));
      this.label_extra2_combo_d3d_2.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_d3d_2.AutoSize")));
      this.label_extra2_combo_d3d_2.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_d3d_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_d3d_2.Dock")));
      this.label_extra2_combo_d3d_2.Enabled = ((bool)(resources.GetObject("label_extra2_combo_d3d_2.Enabled")));
      this.label_extra2_combo_d3d_2.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_d3d_2.Font")));
      this.label_extra2_combo_d3d_2.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_d3d_2.Image")));
      this.label_extra2_combo_d3d_2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_2.ImageAlign")));
      this.label_extra2_combo_d3d_2.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_2.ImageIndex")));
      this.label_extra2_combo_d3d_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_d3d_2.ImeMode")));
      this.label_extra2_combo_d3d_2.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_d3d_2.Location")));
      this.label_extra2_combo_d3d_2.Name = "label_extra2_combo_d3d_2";
      this.label_extra2_combo_d3d_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_d3d_2.RightToLeft")));
      this.label_extra2_combo_d3d_2.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_d3d_2.Size")));
      this.label_extra2_combo_d3d_2.TabIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_2.TabIndex")));
      this.label_extra2_combo_d3d_2.Tag = "1";
      this.label_extra2_combo_d3d_2.Text = resources.GetString("label_extra2_combo_d3d_2.Text");
      this.label_extra2_combo_d3d_2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_2.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_d3d_2, resources.GetString("label_extra2_combo_d3d_2.ToolTip"));
      this.label_extra2_combo_d3d_2.Visible = ((bool)(resources.GetObject("label_extra2_combo_d3d_2.Visible")));
      this.label_extra2_combo_d3d_2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_d3d_3
      // 
      this.label_extra2_combo_d3d_3.AccessibleDescription = resources.GetString("label_extra2_combo_d3d_3.AccessibleDescription");
      this.label_extra2_combo_d3d_3.AccessibleName = resources.GetString("label_extra2_combo_d3d_3.AccessibleName");
      this.label_extra2_combo_d3d_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_d3d_3.Anchor")));
      this.label_extra2_combo_d3d_3.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_d3d_3.AutoSize")));
      this.label_extra2_combo_d3d_3.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_d3d_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_d3d_3.Dock")));
      this.label_extra2_combo_d3d_3.Enabled = ((bool)(resources.GetObject("label_extra2_combo_d3d_3.Enabled")));
      this.label_extra2_combo_d3d_3.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_d3d_3.Font")));
      this.label_extra2_combo_d3d_3.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_d3d_3.Image")));
      this.label_extra2_combo_d3d_3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_3.ImageAlign")));
      this.label_extra2_combo_d3d_3.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_3.ImageIndex")));
      this.label_extra2_combo_d3d_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_d3d_3.ImeMode")));
      this.label_extra2_combo_d3d_3.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_d3d_3.Location")));
      this.label_extra2_combo_d3d_3.Name = "label_extra2_combo_d3d_3";
      this.label_extra2_combo_d3d_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_d3d_3.RightToLeft")));
      this.label_extra2_combo_d3d_3.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_d3d_3.Size")));
      this.label_extra2_combo_d3d_3.TabIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_3.TabIndex")));
      this.label_extra2_combo_d3d_3.Tag = "2";
      this.label_extra2_combo_d3d_3.Text = resources.GetString("label_extra2_combo_d3d_3.Text");
      this.label_extra2_combo_d3d_3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_3.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_d3d_3, resources.GetString("label_extra2_combo_d3d_3.ToolTip"));
      this.label_extra2_combo_d3d_3.Visible = ((bool)(resources.GetObject("label_extra2_combo_d3d_3.Visible")));
      this.label_extra2_combo_d3d_3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_d3d_4
      // 
      this.label_extra2_combo_d3d_4.AccessibleDescription = resources.GetString("label_extra2_combo_d3d_4.AccessibleDescription");
      this.label_extra2_combo_d3d_4.AccessibleName = resources.GetString("label_extra2_combo_d3d_4.AccessibleName");
      this.label_extra2_combo_d3d_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_d3d_4.Anchor")));
      this.label_extra2_combo_d3d_4.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_d3d_4.AutoSize")));
      this.label_extra2_combo_d3d_4.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_d3d_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_d3d_4.Dock")));
      this.label_extra2_combo_d3d_4.Enabled = ((bool)(resources.GetObject("label_extra2_combo_d3d_4.Enabled")));
      this.label_extra2_combo_d3d_4.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_d3d_4.Font")));
      this.label_extra2_combo_d3d_4.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_d3d_4.Image")));
      this.label_extra2_combo_d3d_4.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_4.ImageAlign")));
      this.label_extra2_combo_d3d_4.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_4.ImageIndex")));
      this.label_extra2_combo_d3d_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_d3d_4.ImeMode")));
      this.label_extra2_combo_d3d_4.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_d3d_4.Location")));
      this.label_extra2_combo_d3d_4.Name = "label_extra2_combo_d3d_4";
      this.label_extra2_combo_d3d_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_d3d_4.RightToLeft")));
      this.label_extra2_combo_d3d_4.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_d3d_4.Size")));
      this.label_extra2_combo_d3d_4.TabIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_4.TabIndex")));
      this.label_extra2_combo_d3d_4.Tag = "3";
      this.label_extra2_combo_d3d_4.Text = resources.GetString("label_extra2_combo_d3d_4.Text");
      this.label_extra2_combo_d3d_4.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_4.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_d3d_4, resources.GetString("label_extra2_combo_d3d_4.ToolTip"));
      this.label_extra2_combo_d3d_4.Visible = ((bool)(resources.GetObject("label_extra2_combo_d3d_4.Visible")));
      this.label_extra2_combo_d3d_4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_d3d_1
      // 
      this.label_extra2_combo_d3d_1.AccessibleDescription = resources.GetString("label_extra2_combo_d3d_1.AccessibleDescription");
      this.label_extra2_combo_d3d_1.AccessibleName = resources.GetString("label_extra2_combo_d3d_1.AccessibleName");
      this.label_extra2_combo_d3d_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_d3d_1.Anchor")));
      this.label_extra2_combo_d3d_1.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_d3d_1.AutoSize")));
      this.label_extra2_combo_d3d_1.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_d3d_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_d3d_1.Dock")));
      this.label_extra2_combo_d3d_1.Enabled = ((bool)(resources.GetObject("label_extra2_combo_d3d_1.Enabled")));
      this.label_extra2_combo_d3d_1.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_d3d_1.Font")));
      this.label_extra2_combo_d3d_1.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_d3d_1.Image")));
      this.label_extra2_combo_d3d_1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_1.ImageAlign")));
      this.label_extra2_combo_d3d_1.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_1.ImageIndex")));
      this.label_extra2_combo_d3d_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_d3d_1.ImeMode")));
      this.label_extra2_combo_d3d_1.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_d3d_1.Location")));
      this.label_extra2_combo_d3d_1.Name = "label_extra2_combo_d3d_1";
      this.label_extra2_combo_d3d_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_d3d_1.RightToLeft")));
      this.label_extra2_combo_d3d_1.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_d3d_1.Size")));
      this.label_extra2_combo_d3d_1.TabIndex = ((int)(resources.GetObject("label_extra2_combo_d3d_1.TabIndex")));
      this.label_extra2_combo_d3d_1.Text = resources.GetString("label_extra2_combo_d3d_1.Text");
      this.label_extra2_combo_d3d_1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_d3d_1.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_d3d_1, resources.GetString("label_extra2_combo_d3d_1.ToolTip"));
      this.label_extra2_combo_d3d_1.Visible = ((bool)(resources.GetObject("label_extra2_combo_d3d_1.Visible")));
      this.label_extra2_combo_d3d_1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_curr_ogl
      // 
      this.label_extra_curr_ogl.AccessibleDescription = resources.GetString("label_extra_curr_ogl.AccessibleDescription");
      this.label_extra_curr_ogl.AccessibleName = resources.GetString("label_extra_curr_ogl.AccessibleName");
      this.label_extra_curr_ogl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_curr_ogl.Anchor")));
      this.label_extra_curr_ogl.AutoSize = ((bool)(resources.GetObject("label_extra_curr_ogl.AutoSize")));
      this.label_extra_curr_ogl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_curr_ogl.Dock")));
      this.label_extra_curr_ogl.Enabled = ((bool)(resources.GetObject("label_extra_curr_ogl.Enabled")));
      this.label_extra_curr_ogl.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_curr_ogl.Font")));
      this.label_extra_curr_ogl.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_curr_ogl.Image")));
      this.label_extra_curr_ogl.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_curr_ogl.ImageAlign")));
      this.label_extra_curr_ogl.ImageIndex = ((int)(resources.GetObject("label_extra_curr_ogl.ImageIndex")));
      this.label_extra_curr_ogl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_curr_ogl.ImeMode")));
      this.label_extra_curr_ogl.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_curr_ogl.Location")));
      this.label_extra_curr_ogl.Name = "label_extra_curr_ogl";
      this.label_extra_curr_ogl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_curr_ogl.RightToLeft")));
      this.label_extra_curr_ogl.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_curr_ogl.Size")));
      this.label_extra_curr_ogl.TabIndex = ((int)(resources.GetObject("label_extra_curr_ogl.TabIndex")));
      this.label_extra_curr_ogl.Text = resources.GetString("label_extra_curr_ogl.Text");
      this.label_extra_curr_ogl.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_curr_ogl.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_curr_ogl, resources.GetString("label_extra_curr_ogl.ToolTip"));
      this.label_extra_curr_ogl.Visible = ((bool)(resources.GetObject("label_extra_curr_ogl.Visible")));
      // 
      // group_extra_ogl
      // 
      this.group_extra_ogl.AccessibleDescription = resources.GetString("group_extra_ogl.AccessibleDescription");
      this.group_extra_ogl.AccessibleName = resources.GetString("group_extra_ogl.AccessibleName");
      this.group_extra_ogl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("group_extra_ogl.Anchor")));
      this.group_extra_ogl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("group_extra_ogl.BackgroundImage")));
      this.group_extra_ogl.Controls.Add(this.combo_extra_prof_ogl_8);
      this.group_extra_ogl.Controls.Add(this.label_extra_combo_ogl_8);
      this.group_extra_ogl.Controls.Add(this.combo_extra_curr_ogl_8);
      this.group_extra_ogl.Controls.Add(this.combo_extra_prof_ogl_7);
      this.group_extra_ogl.Controls.Add(this.label_extra_combo_ogl_7);
      this.group_extra_ogl.Controls.Add(this.combo_extra_curr_ogl_7);
      this.group_extra_ogl.Controls.Add(this.combo_extra_prof_ogl_6);
      this.group_extra_ogl.Controls.Add(this.combo_extra_prof_ogl_4);
      this.group_extra_ogl.Controls.Add(this.combo_extra_prof_ogl_2);
      this.group_extra_ogl.Controls.Add(this.combo_extra_prof_ogl_1);
      this.group_extra_ogl.Controls.Add(this.combo_extra_prof_ogl_3);
      this.group_extra_ogl.Controls.Add(this.combo_extra_prof_ogl_5);
      this.group_extra_ogl.Controls.Add(this.label_extra_prof_ogl);
      this.group_extra_ogl.Controls.Add(this.label_extra_combo_ogl_1);
      this.group_extra_ogl.Controls.Add(this.label_extra_combo_ogl_4);
      this.group_extra_ogl.Controls.Add(this.label_extra_combo_ogl_5);
      this.group_extra_ogl.Controls.Add(this.label_extra_combo_ogl_3);
      this.group_extra_ogl.Controls.Add(this.label_extra_combo_ogl_2);
      this.group_extra_ogl.Controls.Add(this.label_extra_combo_ogl_6);
      this.group_extra_ogl.Controls.Add(this.combo_extra_curr_ogl_3);
      this.group_extra_ogl.Controls.Add(this.combo_extra_curr_ogl_4);
      this.group_extra_ogl.Controls.Add(this.label_extra_curr_ogl);
      this.group_extra_ogl.Controls.Add(this.combo_extra_curr_ogl_5);
      this.group_extra_ogl.Controls.Add(this.combo_extra_curr_ogl_1);
      this.group_extra_ogl.Controls.Add(this.combo_extra_curr_ogl_6);
      this.group_extra_ogl.Controls.Add(this.combo_extra_curr_ogl_2);
      this.group_extra_ogl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("group_extra_ogl.Dock")));
      this.group_extra_ogl.Enabled = ((bool)(resources.GetObject("group_extra_ogl.Enabled")));
      this.group_extra_ogl.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.group_extra_ogl.Font = ((System.Drawing.Font)(resources.GetObject("group_extra_ogl.Font")));
      this.group_extra_ogl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("group_extra_ogl.ImeMode")));
      this.group_extra_ogl.Location = ((System.Drawing.Point)(resources.GetObject("group_extra_ogl.Location")));
      this.group_extra_ogl.Name = "group_extra_ogl";
      this.group_extra_ogl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("group_extra_ogl.RightToLeft")));
      this.group_extra_ogl.Size = ((System.Drawing.Size)(resources.GetObject("group_extra_ogl.Size")));
      this.group_extra_ogl.TabIndex = ((int)(resources.GetObject("group_extra_ogl.TabIndex")));
      this.group_extra_ogl.TabStop = false;
      this.group_extra_ogl.Text = resources.GetString("group_extra_ogl.Text");
      this.toolTip.SetToolTip(this.group_extra_ogl, resources.GetString("group_extra_ogl.ToolTip"));
      this.group_extra_ogl.Visible = ((bool)(resources.GetObject("group_extra_ogl.Visible")));
      // 
      // combo_extra_prof_ogl_8
      // 
      this.combo_extra_prof_ogl_8.AccessibleDescription = resources.GetString("combo_extra_prof_ogl_8.AccessibleDescription");
      this.combo_extra_prof_ogl_8.AccessibleName = resources.GetString("combo_extra_prof_ogl_8.AccessibleName");
      this.combo_extra_prof_ogl_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_ogl_8.Anchor")));
      this.combo_extra_prof_ogl_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_ogl_8.BackgroundImage")));
      this.combo_extra_prof_ogl_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_ogl_8.Dock")));
      this.combo_extra_prof_ogl_8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_ogl_8.Enabled = ((bool)(resources.GetObject("combo_extra_prof_ogl_8.Enabled")));
      this.combo_extra_prof_ogl_8.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_ogl_8.Font")));
      this.combo_extra_prof_ogl_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_ogl_8.ImeMode")));
      this.combo_extra_prof_ogl_8.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_ogl_8.IntegralHeight")));
      this.combo_extra_prof_ogl_8.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_ogl_8.ItemHeight")));
      this.combo_extra_prof_ogl_8.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_ogl_8.Location")));
      this.combo_extra_prof_ogl_8.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_ogl_8.MaxDropDownItems")));
      this.combo_extra_prof_ogl_8.MaxLength = ((int)(resources.GetObject("combo_extra_prof_ogl_8.MaxLength")));
      this.combo_extra_prof_ogl_8.Name = "combo_extra_prof_ogl_8";
      this.combo_extra_prof_ogl_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_ogl_8.RightToLeft")));
      this.combo_extra_prof_ogl_8.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_ogl_8.Size")));
      this.combo_extra_prof_ogl_8.TabIndex = ((int)(resources.GetObject("combo_extra_prof_ogl_8.TabIndex")));
      this.combo_extra_prof_ogl_8.Text = resources.GetString("combo_extra_prof_ogl_8.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_ogl_8, resources.GetString("combo_extra_prof_ogl_8.ToolTip"));
      this.combo_extra_prof_ogl_8.Visible = ((bool)(resources.GetObject("combo_extra_prof_ogl_8.Visible")));
      // 
      // label_extra_combo_ogl_8
      // 
      this.label_extra_combo_ogl_8.AccessibleDescription = resources.GetString("label_extra_combo_ogl_8.AccessibleDescription");
      this.label_extra_combo_ogl_8.AccessibleName = resources.GetString("label_extra_combo_ogl_8.AccessibleName");
      this.label_extra_combo_ogl_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_ogl_8.Anchor")));
      this.label_extra_combo_ogl_8.AutoSize = ((bool)(resources.GetObject("label_extra_combo_ogl_8.AutoSize")));
      this.label_extra_combo_ogl_8.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_ogl_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_ogl_8.Dock")));
      this.label_extra_combo_ogl_8.Enabled = ((bool)(resources.GetObject("label_extra_combo_ogl_8.Enabled")));
      this.label_extra_combo_ogl_8.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_ogl_8.Font")));
      this.label_extra_combo_ogl_8.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_ogl_8.Image")));
      this.label_extra_combo_ogl_8.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_8.ImageAlign")));
      this.label_extra_combo_ogl_8.ImageIndex = ((int)(resources.GetObject("label_extra_combo_ogl_8.ImageIndex")));
      this.label_extra_combo_ogl_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_ogl_8.ImeMode")));
      this.label_extra_combo_ogl_8.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_ogl_8.Location")));
      this.label_extra_combo_ogl_8.Name = "label_extra_combo_ogl_8";
      this.label_extra_combo_ogl_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_ogl_8.RightToLeft")));
      this.label_extra_combo_ogl_8.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_ogl_8.Size")));
      this.label_extra_combo_ogl_8.TabIndex = ((int)(resources.GetObject("label_extra_combo_ogl_8.TabIndex")));
      this.label_extra_combo_ogl_8.Tag = "5";
      this.label_extra_combo_ogl_8.Text = resources.GetString("label_extra_combo_ogl_8.Text");
      this.label_extra_combo_ogl_8.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_8.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_ogl_8, resources.GetString("label_extra_combo_ogl_8.ToolTip"));
      this.label_extra_combo_ogl_8.Visible = ((bool)(resources.GetObject("label_extra_combo_ogl_8.Visible")));
      this.label_extra_combo_ogl_8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // combo_extra_curr_ogl_8
      // 
      this.combo_extra_curr_ogl_8.AccessibleDescription = resources.GetString("combo_extra_curr_ogl_8.AccessibleDescription");
      this.combo_extra_curr_ogl_8.AccessibleName = resources.GetString("combo_extra_curr_ogl_8.AccessibleName");
      this.combo_extra_curr_ogl_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_ogl_8.Anchor")));
      this.combo_extra_curr_ogl_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_ogl_8.BackgroundImage")));
      this.combo_extra_curr_ogl_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_ogl_8.Dock")));
      this.combo_extra_curr_ogl_8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_ogl_8.Enabled = ((bool)(resources.GetObject("combo_extra_curr_ogl_8.Enabled")));
      this.combo_extra_curr_ogl_8.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_ogl_8.Font")));
      this.combo_extra_curr_ogl_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_ogl_8.ImeMode")));
      this.combo_extra_curr_ogl_8.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_ogl_8.IntegralHeight")));
      this.combo_extra_curr_ogl_8.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_ogl_8.ItemHeight")));
      this.combo_extra_curr_ogl_8.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_ogl_8.Location")));
      this.combo_extra_curr_ogl_8.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_ogl_8.MaxDropDownItems")));
      this.combo_extra_curr_ogl_8.MaxLength = ((int)(resources.GetObject("combo_extra_curr_ogl_8.MaxLength")));
      this.combo_extra_curr_ogl_8.Name = "combo_extra_curr_ogl_8";
      this.combo_extra_curr_ogl_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_ogl_8.RightToLeft")));
      this.combo_extra_curr_ogl_8.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_ogl_8.Size")));
      this.combo_extra_curr_ogl_8.TabIndex = ((int)(resources.GetObject("combo_extra_curr_ogl_8.TabIndex")));
      this.combo_extra_curr_ogl_8.Text = resources.GetString("combo_extra_curr_ogl_8.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_ogl_8, resources.GetString("combo_extra_curr_ogl_8.ToolTip"));
      this.combo_extra_curr_ogl_8.Visible = ((bool)(resources.GetObject("combo_extra_curr_ogl_8.Visible")));
      // 
      // combo_extra_prof_ogl_7
      // 
      this.combo_extra_prof_ogl_7.AccessibleDescription = resources.GetString("combo_extra_prof_ogl_7.AccessibleDescription");
      this.combo_extra_prof_ogl_7.AccessibleName = resources.GetString("combo_extra_prof_ogl_7.AccessibleName");
      this.combo_extra_prof_ogl_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_prof_ogl_7.Anchor")));
      this.combo_extra_prof_ogl_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_prof_ogl_7.BackgroundImage")));
      this.combo_extra_prof_ogl_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_prof_ogl_7.Dock")));
      this.combo_extra_prof_ogl_7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_prof_ogl_7.Enabled = ((bool)(resources.GetObject("combo_extra_prof_ogl_7.Enabled")));
      this.combo_extra_prof_ogl_7.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_prof_ogl_7.Font")));
      this.combo_extra_prof_ogl_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_prof_ogl_7.ImeMode")));
      this.combo_extra_prof_ogl_7.IntegralHeight = ((bool)(resources.GetObject("combo_extra_prof_ogl_7.IntegralHeight")));
      this.combo_extra_prof_ogl_7.ItemHeight = ((int)(resources.GetObject("combo_extra_prof_ogl_7.ItemHeight")));
      this.combo_extra_prof_ogl_7.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_prof_ogl_7.Location")));
      this.combo_extra_prof_ogl_7.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_prof_ogl_7.MaxDropDownItems")));
      this.combo_extra_prof_ogl_7.MaxLength = ((int)(resources.GetObject("combo_extra_prof_ogl_7.MaxLength")));
      this.combo_extra_prof_ogl_7.Name = "combo_extra_prof_ogl_7";
      this.combo_extra_prof_ogl_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_prof_ogl_7.RightToLeft")));
      this.combo_extra_prof_ogl_7.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_prof_ogl_7.Size")));
      this.combo_extra_prof_ogl_7.TabIndex = ((int)(resources.GetObject("combo_extra_prof_ogl_7.TabIndex")));
      this.combo_extra_prof_ogl_7.Text = resources.GetString("combo_extra_prof_ogl_7.Text");
      this.toolTip.SetToolTip(this.combo_extra_prof_ogl_7, resources.GetString("combo_extra_prof_ogl_7.ToolTip"));
      this.combo_extra_prof_ogl_7.Visible = ((bool)(resources.GetObject("combo_extra_prof_ogl_7.Visible")));
      // 
      // label_extra_combo_ogl_7
      // 
      this.label_extra_combo_ogl_7.AccessibleDescription = resources.GetString("label_extra_combo_ogl_7.AccessibleDescription");
      this.label_extra_combo_ogl_7.AccessibleName = resources.GetString("label_extra_combo_ogl_7.AccessibleName");
      this.label_extra_combo_ogl_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_ogl_7.Anchor")));
      this.label_extra_combo_ogl_7.AutoSize = ((bool)(resources.GetObject("label_extra_combo_ogl_7.AutoSize")));
      this.label_extra_combo_ogl_7.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_ogl_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_ogl_7.Dock")));
      this.label_extra_combo_ogl_7.Enabled = ((bool)(resources.GetObject("label_extra_combo_ogl_7.Enabled")));
      this.label_extra_combo_ogl_7.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_ogl_7.Font")));
      this.label_extra_combo_ogl_7.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_ogl_7.Image")));
      this.label_extra_combo_ogl_7.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_7.ImageAlign")));
      this.label_extra_combo_ogl_7.ImageIndex = ((int)(resources.GetObject("label_extra_combo_ogl_7.ImageIndex")));
      this.label_extra_combo_ogl_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_ogl_7.ImeMode")));
      this.label_extra_combo_ogl_7.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_ogl_7.Location")));
      this.label_extra_combo_ogl_7.Name = "label_extra_combo_ogl_7";
      this.label_extra_combo_ogl_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_ogl_7.RightToLeft")));
      this.label_extra_combo_ogl_7.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_ogl_7.Size")));
      this.label_extra_combo_ogl_7.TabIndex = ((int)(resources.GetObject("label_extra_combo_ogl_7.TabIndex")));
      this.label_extra_combo_ogl_7.Tag = "5";
      this.label_extra_combo_ogl_7.Text = resources.GetString("label_extra_combo_ogl_7.Text");
      this.label_extra_combo_ogl_7.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_7.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_ogl_7, resources.GetString("label_extra_combo_ogl_7.ToolTip"));
      this.label_extra_combo_ogl_7.Visible = ((bool)(resources.GetObject("label_extra_combo_ogl_7.Visible")));
      this.label_extra_combo_ogl_7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // combo_extra_curr_ogl_7
      // 
      this.combo_extra_curr_ogl_7.AccessibleDescription = resources.GetString("combo_extra_curr_ogl_7.AccessibleDescription");
      this.combo_extra_curr_ogl_7.AccessibleName = resources.GetString("combo_extra_curr_ogl_7.AccessibleName");
      this.combo_extra_curr_ogl_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra_curr_ogl_7.Anchor")));
      this.combo_extra_curr_ogl_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra_curr_ogl_7.BackgroundImage")));
      this.combo_extra_curr_ogl_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra_curr_ogl_7.Dock")));
      this.combo_extra_curr_ogl_7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra_curr_ogl_7.Enabled = ((bool)(resources.GetObject("combo_extra_curr_ogl_7.Enabled")));
      this.combo_extra_curr_ogl_7.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra_curr_ogl_7.Font")));
      this.combo_extra_curr_ogl_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra_curr_ogl_7.ImeMode")));
      this.combo_extra_curr_ogl_7.IntegralHeight = ((bool)(resources.GetObject("combo_extra_curr_ogl_7.IntegralHeight")));
      this.combo_extra_curr_ogl_7.ItemHeight = ((int)(resources.GetObject("combo_extra_curr_ogl_7.ItemHeight")));
      this.combo_extra_curr_ogl_7.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra_curr_ogl_7.Location")));
      this.combo_extra_curr_ogl_7.MaxDropDownItems = ((int)(resources.GetObject("combo_extra_curr_ogl_7.MaxDropDownItems")));
      this.combo_extra_curr_ogl_7.MaxLength = ((int)(resources.GetObject("combo_extra_curr_ogl_7.MaxLength")));
      this.combo_extra_curr_ogl_7.Name = "combo_extra_curr_ogl_7";
      this.combo_extra_curr_ogl_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra_curr_ogl_7.RightToLeft")));
      this.combo_extra_curr_ogl_7.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra_curr_ogl_7.Size")));
      this.combo_extra_curr_ogl_7.TabIndex = ((int)(resources.GetObject("combo_extra_curr_ogl_7.TabIndex")));
      this.combo_extra_curr_ogl_7.Text = resources.GetString("combo_extra_curr_ogl_7.Text");
      this.toolTip.SetToolTip(this.combo_extra_curr_ogl_7, resources.GetString("combo_extra_curr_ogl_7.ToolTip"));
      this.combo_extra_curr_ogl_7.Visible = ((bool)(resources.GetObject("combo_extra_curr_ogl_7.Visible")));
      // 
      // label_extra_prof_ogl
      // 
      this.label_extra_prof_ogl.AccessibleDescription = resources.GetString("label_extra_prof_ogl.AccessibleDescription");
      this.label_extra_prof_ogl.AccessibleName = resources.GetString("label_extra_prof_ogl.AccessibleName");
      this.label_extra_prof_ogl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_prof_ogl.Anchor")));
      this.label_extra_prof_ogl.AutoSize = ((bool)(resources.GetObject("label_extra_prof_ogl.AutoSize")));
      this.label_extra_prof_ogl.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_prof_ogl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_prof_ogl.Dock")));
      this.label_extra_prof_ogl.Enabled = ((bool)(resources.GetObject("label_extra_prof_ogl.Enabled")));
      this.label_extra_prof_ogl.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_prof_ogl.Font")));
      this.label_extra_prof_ogl.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_prof_ogl.Image")));
      this.label_extra_prof_ogl.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_prof_ogl.ImageAlign")));
      this.label_extra_prof_ogl.ImageIndex = ((int)(resources.GetObject("label_extra_prof_ogl.ImageIndex")));
      this.label_extra_prof_ogl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_prof_ogl.ImeMode")));
      this.label_extra_prof_ogl.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_prof_ogl.Location")));
      this.label_extra_prof_ogl.Name = "label_extra_prof_ogl";
      this.label_extra_prof_ogl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_prof_ogl.RightToLeft")));
      this.label_extra_prof_ogl.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_prof_ogl.Size")));
      this.label_extra_prof_ogl.TabIndex = ((int)(resources.GetObject("label_extra_prof_ogl.TabIndex")));
      this.label_extra_prof_ogl.Text = resources.GetString("label_extra_prof_ogl.Text");
      this.label_extra_prof_ogl.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_prof_ogl.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_prof_ogl, resources.GetString("label_extra_prof_ogl.ToolTip"));
      this.label_extra_prof_ogl.Visible = ((bool)(resources.GetObject("label_extra_prof_ogl.Visible")));
      this.label_extra_prof_ogl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_ogl_MouseDown);
      // 
      // label_extra_combo_ogl_1
      // 
      this.label_extra_combo_ogl_1.AccessibleDescription = resources.GetString("label_extra_combo_ogl_1.AccessibleDescription");
      this.label_extra_combo_ogl_1.AccessibleName = resources.GetString("label_extra_combo_ogl_1.AccessibleName");
      this.label_extra_combo_ogl_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_ogl_1.Anchor")));
      this.label_extra_combo_ogl_1.AutoSize = ((bool)(resources.GetObject("label_extra_combo_ogl_1.AutoSize")));
      this.label_extra_combo_ogl_1.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_ogl_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_ogl_1.Dock")));
      this.label_extra_combo_ogl_1.Enabled = ((bool)(resources.GetObject("label_extra_combo_ogl_1.Enabled")));
      this.label_extra_combo_ogl_1.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_ogl_1.Font")));
      this.label_extra_combo_ogl_1.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_ogl_1.Image")));
      this.label_extra_combo_ogl_1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_1.ImageAlign")));
      this.label_extra_combo_ogl_1.ImageIndex = ((int)(resources.GetObject("label_extra_combo_ogl_1.ImageIndex")));
      this.label_extra_combo_ogl_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_ogl_1.ImeMode")));
      this.label_extra_combo_ogl_1.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_ogl_1.Location")));
      this.label_extra_combo_ogl_1.Name = "label_extra_combo_ogl_1";
      this.label_extra_combo_ogl_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_ogl_1.RightToLeft")));
      this.label_extra_combo_ogl_1.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_ogl_1.Size")));
      this.label_extra_combo_ogl_1.TabIndex = ((int)(resources.GetObject("label_extra_combo_ogl_1.TabIndex")));
      this.label_extra_combo_ogl_1.Text = resources.GetString("label_extra_combo_ogl_1.Text");
      this.label_extra_combo_ogl_1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_1.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_ogl_1, resources.GetString("label_extra_combo_ogl_1.ToolTip"));
      this.label_extra_combo_ogl_1.Visible = ((bool)(resources.GetObject("label_extra_combo_ogl_1.Visible")));
      this.label_extra_combo_ogl_1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_combo_ogl_4
      // 
      this.label_extra_combo_ogl_4.AccessibleDescription = resources.GetString("label_extra_combo_ogl_4.AccessibleDescription");
      this.label_extra_combo_ogl_4.AccessibleName = resources.GetString("label_extra_combo_ogl_4.AccessibleName");
      this.label_extra_combo_ogl_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_ogl_4.Anchor")));
      this.label_extra_combo_ogl_4.AutoSize = ((bool)(resources.GetObject("label_extra_combo_ogl_4.AutoSize")));
      this.label_extra_combo_ogl_4.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_ogl_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_ogl_4.Dock")));
      this.label_extra_combo_ogl_4.Enabled = ((bool)(resources.GetObject("label_extra_combo_ogl_4.Enabled")));
      this.label_extra_combo_ogl_4.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_ogl_4.Font")));
      this.label_extra_combo_ogl_4.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_ogl_4.Image")));
      this.label_extra_combo_ogl_4.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_4.ImageAlign")));
      this.label_extra_combo_ogl_4.ImageIndex = ((int)(resources.GetObject("label_extra_combo_ogl_4.ImageIndex")));
      this.label_extra_combo_ogl_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_ogl_4.ImeMode")));
      this.label_extra_combo_ogl_4.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_ogl_4.Location")));
      this.label_extra_combo_ogl_4.Name = "label_extra_combo_ogl_4";
      this.label_extra_combo_ogl_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_ogl_4.RightToLeft")));
      this.label_extra_combo_ogl_4.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_ogl_4.Size")));
      this.label_extra_combo_ogl_4.TabIndex = ((int)(resources.GetObject("label_extra_combo_ogl_4.TabIndex")));
      this.label_extra_combo_ogl_4.Tag = "3";
      this.label_extra_combo_ogl_4.Text = resources.GetString("label_extra_combo_ogl_4.Text");
      this.label_extra_combo_ogl_4.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_4.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_ogl_4, resources.GetString("label_extra_combo_ogl_4.ToolTip"));
      this.label_extra_combo_ogl_4.Visible = ((bool)(resources.GetObject("label_extra_combo_ogl_4.Visible")));
      this.label_extra_combo_ogl_4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_combo_ogl_5
      // 
      this.label_extra_combo_ogl_5.AccessibleDescription = resources.GetString("label_extra_combo_ogl_5.AccessibleDescription");
      this.label_extra_combo_ogl_5.AccessibleName = resources.GetString("label_extra_combo_ogl_5.AccessibleName");
      this.label_extra_combo_ogl_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_ogl_5.Anchor")));
      this.label_extra_combo_ogl_5.AutoSize = ((bool)(resources.GetObject("label_extra_combo_ogl_5.AutoSize")));
      this.label_extra_combo_ogl_5.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_ogl_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_ogl_5.Dock")));
      this.label_extra_combo_ogl_5.Enabled = ((bool)(resources.GetObject("label_extra_combo_ogl_5.Enabled")));
      this.label_extra_combo_ogl_5.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_ogl_5.Font")));
      this.label_extra_combo_ogl_5.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_ogl_5.Image")));
      this.label_extra_combo_ogl_5.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_5.ImageAlign")));
      this.label_extra_combo_ogl_5.ImageIndex = ((int)(resources.GetObject("label_extra_combo_ogl_5.ImageIndex")));
      this.label_extra_combo_ogl_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_ogl_5.ImeMode")));
      this.label_extra_combo_ogl_5.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_ogl_5.Location")));
      this.label_extra_combo_ogl_5.Name = "label_extra_combo_ogl_5";
      this.label_extra_combo_ogl_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_ogl_5.RightToLeft")));
      this.label_extra_combo_ogl_5.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_ogl_5.Size")));
      this.label_extra_combo_ogl_5.TabIndex = ((int)(resources.GetObject("label_extra_combo_ogl_5.TabIndex")));
      this.label_extra_combo_ogl_5.Tag = "4";
      this.label_extra_combo_ogl_5.Text = resources.GetString("label_extra_combo_ogl_5.Text");
      this.label_extra_combo_ogl_5.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_5.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_ogl_5, resources.GetString("label_extra_combo_ogl_5.ToolTip"));
      this.label_extra_combo_ogl_5.Visible = ((bool)(resources.GetObject("label_extra_combo_ogl_5.Visible")));
      this.label_extra_combo_ogl_5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_combo_ogl_3
      // 
      this.label_extra_combo_ogl_3.AccessibleDescription = resources.GetString("label_extra_combo_ogl_3.AccessibleDescription");
      this.label_extra_combo_ogl_3.AccessibleName = resources.GetString("label_extra_combo_ogl_3.AccessibleName");
      this.label_extra_combo_ogl_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_ogl_3.Anchor")));
      this.label_extra_combo_ogl_3.AutoSize = ((bool)(resources.GetObject("label_extra_combo_ogl_3.AutoSize")));
      this.label_extra_combo_ogl_3.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_ogl_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_ogl_3.Dock")));
      this.label_extra_combo_ogl_3.Enabled = ((bool)(resources.GetObject("label_extra_combo_ogl_3.Enabled")));
      this.label_extra_combo_ogl_3.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_ogl_3.Font")));
      this.label_extra_combo_ogl_3.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_ogl_3.Image")));
      this.label_extra_combo_ogl_3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_3.ImageAlign")));
      this.label_extra_combo_ogl_3.ImageIndex = ((int)(resources.GetObject("label_extra_combo_ogl_3.ImageIndex")));
      this.label_extra_combo_ogl_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_ogl_3.ImeMode")));
      this.label_extra_combo_ogl_3.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_ogl_3.Location")));
      this.label_extra_combo_ogl_3.Name = "label_extra_combo_ogl_3";
      this.label_extra_combo_ogl_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_ogl_3.RightToLeft")));
      this.label_extra_combo_ogl_3.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_ogl_3.Size")));
      this.label_extra_combo_ogl_3.TabIndex = ((int)(resources.GetObject("label_extra_combo_ogl_3.TabIndex")));
      this.label_extra_combo_ogl_3.Tag = "2";
      this.label_extra_combo_ogl_3.Text = resources.GetString("label_extra_combo_ogl_3.Text");
      this.label_extra_combo_ogl_3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_3.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_ogl_3, resources.GetString("label_extra_combo_ogl_3.ToolTip"));
      this.label_extra_combo_ogl_3.Visible = ((bool)(resources.GetObject("label_extra_combo_ogl_3.Visible")));
      this.label_extra_combo_ogl_3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_combo_ogl_2
      // 
      this.label_extra_combo_ogl_2.AccessibleDescription = resources.GetString("label_extra_combo_ogl_2.AccessibleDescription");
      this.label_extra_combo_ogl_2.AccessibleName = resources.GetString("label_extra_combo_ogl_2.AccessibleName");
      this.label_extra_combo_ogl_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_ogl_2.Anchor")));
      this.label_extra_combo_ogl_2.AutoSize = ((bool)(resources.GetObject("label_extra_combo_ogl_2.AutoSize")));
      this.label_extra_combo_ogl_2.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_ogl_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_ogl_2.Dock")));
      this.label_extra_combo_ogl_2.Enabled = ((bool)(resources.GetObject("label_extra_combo_ogl_2.Enabled")));
      this.label_extra_combo_ogl_2.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_ogl_2.Font")));
      this.label_extra_combo_ogl_2.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_ogl_2.Image")));
      this.label_extra_combo_ogl_2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_2.ImageAlign")));
      this.label_extra_combo_ogl_2.ImageIndex = ((int)(resources.GetObject("label_extra_combo_ogl_2.ImageIndex")));
      this.label_extra_combo_ogl_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_ogl_2.ImeMode")));
      this.label_extra_combo_ogl_2.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_ogl_2.Location")));
      this.label_extra_combo_ogl_2.Name = "label_extra_combo_ogl_2";
      this.label_extra_combo_ogl_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_ogl_2.RightToLeft")));
      this.label_extra_combo_ogl_2.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_ogl_2.Size")));
      this.label_extra_combo_ogl_2.TabIndex = ((int)(resources.GetObject("label_extra_combo_ogl_2.TabIndex")));
      this.label_extra_combo_ogl_2.Tag = "1";
      this.label_extra_combo_ogl_2.Text = resources.GetString("label_extra_combo_ogl_2.Text");
      this.label_extra_combo_ogl_2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_2.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_ogl_2, resources.GetString("label_extra_combo_ogl_2.ToolTip"));
      this.label_extra_combo_ogl_2.Visible = ((bool)(resources.GetObject("label_extra_combo_ogl_2.Visible")));
      this.label_extra_combo_ogl_2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra_combo_ogl_6
      // 
      this.label_extra_combo_ogl_6.AccessibleDescription = resources.GetString("label_extra_combo_ogl_6.AccessibleDescription");
      this.label_extra_combo_ogl_6.AccessibleName = resources.GetString("label_extra_combo_ogl_6.AccessibleName");
      this.label_extra_combo_ogl_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra_combo_ogl_6.Anchor")));
      this.label_extra_combo_ogl_6.AutoSize = ((bool)(resources.GetObject("label_extra_combo_ogl_6.AutoSize")));
      this.label_extra_combo_ogl_6.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra_combo_ogl_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra_combo_ogl_6.Dock")));
      this.label_extra_combo_ogl_6.Enabled = ((bool)(resources.GetObject("label_extra_combo_ogl_6.Enabled")));
      this.label_extra_combo_ogl_6.Font = ((System.Drawing.Font)(resources.GetObject("label_extra_combo_ogl_6.Font")));
      this.label_extra_combo_ogl_6.Image = ((System.Drawing.Image)(resources.GetObject("label_extra_combo_ogl_6.Image")));
      this.label_extra_combo_ogl_6.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_6.ImageAlign")));
      this.label_extra_combo_ogl_6.ImageIndex = ((int)(resources.GetObject("label_extra_combo_ogl_6.ImageIndex")));
      this.label_extra_combo_ogl_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra_combo_ogl_6.ImeMode")));
      this.label_extra_combo_ogl_6.Location = ((System.Drawing.Point)(resources.GetObject("label_extra_combo_ogl_6.Location")));
      this.label_extra_combo_ogl_6.Name = "label_extra_combo_ogl_6";
      this.label_extra_combo_ogl_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra_combo_ogl_6.RightToLeft")));
      this.label_extra_combo_ogl_6.Size = ((System.Drawing.Size)(resources.GetObject("label_extra_combo_ogl_6.Size")));
      this.label_extra_combo_ogl_6.TabIndex = ((int)(resources.GetObject("label_extra_combo_ogl_6.TabIndex")));
      this.label_extra_combo_ogl_6.Tag = "5";
      this.label_extra_combo_ogl_6.Text = resources.GetString("label_extra_combo_ogl_6.Text");
      this.label_extra_combo_ogl_6.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra_combo_ogl_6.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra_combo_ogl_6, resources.GetString("label_extra_combo_ogl_6.ToolTip"));
      this.label_extra_combo_ogl_6.Visible = ((bool)(resources.GetObject("label_extra_combo_ogl_6.Visible")));
      this.label_extra_combo_ogl_6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_ogl_8
      // 
      this.label_extra2_combo_ogl_8.AccessibleDescription = resources.GetString("label_extra2_combo_ogl_8.AccessibleDescription");
      this.label_extra2_combo_ogl_8.AccessibleName = resources.GetString("label_extra2_combo_ogl_8.AccessibleName");
      this.label_extra2_combo_ogl_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_ogl_8.Anchor")));
      this.label_extra2_combo_ogl_8.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_ogl_8.AutoSize")));
      this.label_extra2_combo_ogl_8.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_ogl_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_ogl_8.Dock")));
      this.label_extra2_combo_ogl_8.Enabled = ((bool)(resources.GetObject("label_extra2_combo_ogl_8.Enabled")));
      this.label_extra2_combo_ogl_8.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_ogl_8.Font")));
      this.label_extra2_combo_ogl_8.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_ogl_8.Image")));
      this.label_extra2_combo_ogl_8.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_8.ImageAlign")));
      this.label_extra2_combo_ogl_8.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_8.ImageIndex")));
      this.label_extra2_combo_ogl_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_ogl_8.ImeMode")));
      this.label_extra2_combo_ogl_8.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_ogl_8.Location")));
      this.label_extra2_combo_ogl_8.Name = "label_extra2_combo_ogl_8";
      this.label_extra2_combo_ogl_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_ogl_8.RightToLeft")));
      this.label_extra2_combo_ogl_8.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_ogl_8.Size")));
      this.label_extra2_combo_ogl_8.TabIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_8.TabIndex")));
      this.label_extra2_combo_ogl_8.Tag = "5";
      this.label_extra2_combo_ogl_8.Text = resources.GetString("label_extra2_combo_ogl_8.Text");
      this.label_extra2_combo_ogl_8.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_8.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_ogl_8, resources.GetString("label_extra2_combo_ogl_8.ToolTip"));
      this.label_extra2_combo_ogl_8.Visible = ((bool)(resources.GetObject("label_extra2_combo_ogl_8.Visible")));
      this.label_extra2_combo_ogl_8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // combo_extra2_curr_ogl_8
      // 
      this.combo_extra2_curr_ogl_8.AccessibleDescription = resources.GetString("combo_extra2_curr_ogl_8.AccessibleDescription");
      this.combo_extra2_curr_ogl_8.AccessibleName = resources.GetString("combo_extra2_curr_ogl_8.AccessibleName");
      this.combo_extra2_curr_ogl_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_ogl_8.Anchor")));
      this.combo_extra2_curr_ogl_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_ogl_8.BackgroundImage")));
      this.combo_extra2_curr_ogl_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_ogl_8.Dock")));
      this.combo_extra2_curr_ogl_8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_ogl_8.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_ogl_8.Enabled")));
      this.combo_extra2_curr_ogl_8.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_ogl_8.Font")));
      this.combo_extra2_curr_ogl_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_ogl_8.ImeMode")));
      this.combo_extra2_curr_ogl_8.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_ogl_8.IntegralHeight")));
      this.combo_extra2_curr_ogl_8.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_ogl_8.ItemHeight")));
      this.combo_extra2_curr_ogl_8.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_ogl_8.Location")));
      this.combo_extra2_curr_ogl_8.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_ogl_8.MaxDropDownItems")));
      this.combo_extra2_curr_ogl_8.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_ogl_8.MaxLength")));
      this.combo_extra2_curr_ogl_8.Name = "combo_extra2_curr_ogl_8";
      this.combo_extra2_curr_ogl_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_ogl_8.RightToLeft")));
      this.combo_extra2_curr_ogl_8.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_ogl_8.Size")));
      this.combo_extra2_curr_ogl_8.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_ogl_8.TabIndex")));
      this.combo_extra2_curr_ogl_8.Text = resources.GetString("combo_extra2_curr_ogl_8.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_ogl_8, resources.GetString("combo_extra2_curr_ogl_8.ToolTip"));
      this.combo_extra2_curr_ogl_8.Visible = ((bool)(resources.GetObject("combo_extra2_curr_ogl_8.Visible")));
      // 
      // combo_extra2_prof_ogl_8
      // 
      this.combo_extra2_prof_ogl_8.AccessibleDescription = resources.GetString("combo_extra2_prof_ogl_8.AccessibleDescription");
      this.combo_extra2_prof_ogl_8.AccessibleName = resources.GetString("combo_extra2_prof_ogl_8.AccessibleName");
      this.combo_extra2_prof_ogl_8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_ogl_8.Anchor")));
      this.combo_extra2_prof_ogl_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_ogl_8.BackgroundImage")));
      this.combo_extra2_prof_ogl_8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_ogl_8.Dock")));
      this.combo_extra2_prof_ogl_8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_ogl_8.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_ogl_8.Enabled")));
      this.combo_extra2_prof_ogl_8.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_ogl_8.Font")));
      this.combo_extra2_prof_ogl_8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_ogl_8.ImeMode")));
      this.combo_extra2_prof_ogl_8.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_ogl_8.IntegralHeight")));
      this.combo_extra2_prof_ogl_8.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_ogl_8.ItemHeight")));
      this.combo_extra2_prof_ogl_8.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_ogl_8.Location")));
      this.combo_extra2_prof_ogl_8.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_ogl_8.MaxDropDownItems")));
      this.combo_extra2_prof_ogl_8.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_ogl_8.MaxLength")));
      this.combo_extra2_prof_ogl_8.Name = "combo_extra2_prof_ogl_8";
      this.combo_extra2_prof_ogl_8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_ogl_8.RightToLeft")));
      this.combo_extra2_prof_ogl_8.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_ogl_8.Size")));
      this.combo_extra2_prof_ogl_8.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_ogl_8.TabIndex")));
      this.combo_extra2_prof_ogl_8.Text = resources.GetString("combo_extra2_prof_ogl_8.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_ogl_8, resources.GetString("combo_extra2_prof_ogl_8.ToolTip"));
      this.combo_extra2_prof_ogl_8.Visible = ((bool)(resources.GetObject("combo_extra2_prof_ogl_8.Visible")));
      // 
      // label_extra2_combo_ogl_7
      // 
      this.label_extra2_combo_ogl_7.AccessibleDescription = resources.GetString("label_extra2_combo_ogl_7.AccessibleDescription");
      this.label_extra2_combo_ogl_7.AccessibleName = resources.GetString("label_extra2_combo_ogl_7.AccessibleName");
      this.label_extra2_combo_ogl_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_ogl_7.Anchor")));
      this.label_extra2_combo_ogl_7.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_ogl_7.AutoSize")));
      this.label_extra2_combo_ogl_7.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_ogl_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_ogl_7.Dock")));
      this.label_extra2_combo_ogl_7.Enabled = ((bool)(resources.GetObject("label_extra2_combo_ogl_7.Enabled")));
      this.label_extra2_combo_ogl_7.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_ogl_7.Font")));
      this.label_extra2_combo_ogl_7.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_ogl_7.Image")));
      this.label_extra2_combo_ogl_7.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_7.ImageAlign")));
      this.label_extra2_combo_ogl_7.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_7.ImageIndex")));
      this.label_extra2_combo_ogl_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_ogl_7.ImeMode")));
      this.label_extra2_combo_ogl_7.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_ogl_7.Location")));
      this.label_extra2_combo_ogl_7.Name = "label_extra2_combo_ogl_7";
      this.label_extra2_combo_ogl_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_ogl_7.RightToLeft")));
      this.label_extra2_combo_ogl_7.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_ogl_7.Size")));
      this.label_extra2_combo_ogl_7.TabIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_7.TabIndex")));
      this.label_extra2_combo_ogl_7.Tag = "5";
      this.label_extra2_combo_ogl_7.Text = resources.GetString("label_extra2_combo_ogl_7.Text");
      this.label_extra2_combo_ogl_7.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_7.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_ogl_7, resources.GetString("label_extra2_combo_ogl_7.ToolTip"));
      this.label_extra2_combo_ogl_7.Visible = ((bool)(resources.GetObject("label_extra2_combo_ogl_7.Visible")));
      this.label_extra2_combo_ogl_7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // combo_extra2_curr_ogl_7
      // 
      this.combo_extra2_curr_ogl_7.AccessibleDescription = resources.GetString("combo_extra2_curr_ogl_7.AccessibleDescription");
      this.combo_extra2_curr_ogl_7.AccessibleName = resources.GetString("combo_extra2_curr_ogl_7.AccessibleName");
      this.combo_extra2_curr_ogl_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_curr_ogl_7.Anchor")));
      this.combo_extra2_curr_ogl_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_curr_ogl_7.BackgroundImage")));
      this.combo_extra2_curr_ogl_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_curr_ogl_7.Dock")));
      this.combo_extra2_curr_ogl_7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_curr_ogl_7.Enabled = ((bool)(resources.GetObject("combo_extra2_curr_ogl_7.Enabled")));
      this.combo_extra2_curr_ogl_7.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_curr_ogl_7.Font")));
      this.combo_extra2_curr_ogl_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_curr_ogl_7.ImeMode")));
      this.combo_extra2_curr_ogl_7.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_curr_ogl_7.IntegralHeight")));
      this.combo_extra2_curr_ogl_7.ItemHeight = ((int)(resources.GetObject("combo_extra2_curr_ogl_7.ItemHeight")));
      this.combo_extra2_curr_ogl_7.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_curr_ogl_7.Location")));
      this.combo_extra2_curr_ogl_7.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_curr_ogl_7.MaxDropDownItems")));
      this.combo_extra2_curr_ogl_7.MaxLength = ((int)(resources.GetObject("combo_extra2_curr_ogl_7.MaxLength")));
      this.combo_extra2_curr_ogl_7.Name = "combo_extra2_curr_ogl_7";
      this.combo_extra2_curr_ogl_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_curr_ogl_7.RightToLeft")));
      this.combo_extra2_curr_ogl_7.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_curr_ogl_7.Size")));
      this.combo_extra2_curr_ogl_7.TabIndex = ((int)(resources.GetObject("combo_extra2_curr_ogl_7.TabIndex")));
      this.combo_extra2_curr_ogl_7.Text = resources.GetString("combo_extra2_curr_ogl_7.Text");
      this.toolTip.SetToolTip(this.combo_extra2_curr_ogl_7, resources.GetString("combo_extra2_curr_ogl_7.ToolTip"));
      this.combo_extra2_curr_ogl_7.Visible = ((bool)(resources.GetObject("combo_extra2_curr_ogl_7.Visible")));
      // 
      // combo_extra2_prof_ogl_7
      // 
      this.combo_extra2_prof_ogl_7.AccessibleDescription = resources.GetString("combo_extra2_prof_ogl_7.AccessibleDescription");
      this.combo_extra2_prof_ogl_7.AccessibleName = resources.GetString("combo_extra2_prof_ogl_7.AccessibleName");
      this.combo_extra2_prof_ogl_7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_extra2_prof_ogl_7.Anchor")));
      this.combo_extra2_prof_ogl_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_extra2_prof_ogl_7.BackgroundImage")));
      this.combo_extra2_prof_ogl_7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_extra2_prof_ogl_7.Dock")));
      this.combo_extra2_prof_ogl_7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_extra2_prof_ogl_7.Enabled = ((bool)(resources.GetObject("combo_extra2_prof_ogl_7.Enabled")));
      this.combo_extra2_prof_ogl_7.Font = ((System.Drawing.Font)(resources.GetObject("combo_extra2_prof_ogl_7.Font")));
      this.combo_extra2_prof_ogl_7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_extra2_prof_ogl_7.ImeMode")));
      this.combo_extra2_prof_ogl_7.IntegralHeight = ((bool)(resources.GetObject("combo_extra2_prof_ogl_7.IntegralHeight")));
      this.combo_extra2_prof_ogl_7.ItemHeight = ((int)(resources.GetObject("combo_extra2_prof_ogl_7.ItemHeight")));
      this.combo_extra2_prof_ogl_7.Location = ((System.Drawing.Point)(resources.GetObject("combo_extra2_prof_ogl_7.Location")));
      this.combo_extra2_prof_ogl_7.MaxDropDownItems = ((int)(resources.GetObject("combo_extra2_prof_ogl_7.MaxDropDownItems")));
      this.combo_extra2_prof_ogl_7.MaxLength = ((int)(resources.GetObject("combo_extra2_prof_ogl_7.MaxLength")));
      this.combo_extra2_prof_ogl_7.Name = "combo_extra2_prof_ogl_7";
      this.combo_extra2_prof_ogl_7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_extra2_prof_ogl_7.RightToLeft")));
      this.combo_extra2_prof_ogl_7.Size = ((System.Drawing.Size)(resources.GetObject("combo_extra2_prof_ogl_7.Size")));
      this.combo_extra2_prof_ogl_7.TabIndex = ((int)(resources.GetObject("combo_extra2_prof_ogl_7.TabIndex")));
      this.combo_extra2_prof_ogl_7.Text = resources.GetString("combo_extra2_prof_ogl_7.Text");
      this.toolTip.SetToolTip(this.combo_extra2_prof_ogl_7, resources.GetString("combo_extra2_prof_ogl_7.ToolTip"));
      this.combo_extra2_prof_ogl_7.Visible = ((bool)(resources.GetObject("combo_extra2_prof_ogl_7.Visible")));
      // 
      // label_extra2_combo_ogl_1
      // 
      this.label_extra2_combo_ogl_1.AccessibleDescription = resources.GetString("label_extra2_combo_ogl_1.AccessibleDescription");
      this.label_extra2_combo_ogl_1.AccessibleName = resources.GetString("label_extra2_combo_ogl_1.AccessibleName");
      this.label_extra2_combo_ogl_1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_ogl_1.Anchor")));
      this.label_extra2_combo_ogl_1.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_ogl_1.AutoSize")));
      this.label_extra2_combo_ogl_1.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_ogl_1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_ogl_1.Dock")));
      this.label_extra2_combo_ogl_1.Enabled = ((bool)(resources.GetObject("label_extra2_combo_ogl_1.Enabled")));
      this.label_extra2_combo_ogl_1.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_ogl_1.Font")));
      this.label_extra2_combo_ogl_1.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_ogl_1.Image")));
      this.label_extra2_combo_ogl_1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_1.ImageAlign")));
      this.label_extra2_combo_ogl_1.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_1.ImageIndex")));
      this.label_extra2_combo_ogl_1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_ogl_1.ImeMode")));
      this.label_extra2_combo_ogl_1.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_ogl_1.Location")));
      this.label_extra2_combo_ogl_1.Name = "label_extra2_combo_ogl_1";
      this.label_extra2_combo_ogl_1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_ogl_1.RightToLeft")));
      this.label_extra2_combo_ogl_1.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_ogl_1.Size")));
      this.label_extra2_combo_ogl_1.TabIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_1.TabIndex")));
      this.label_extra2_combo_ogl_1.Text = resources.GetString("label_extra2_combo_ogl_1.Text");
      this.label_extra2_combo_ogl_1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_1.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_ogl_1, resources.GetString("label_extra2_combo_ogl_1.ToolTip"));
      this.label_extra2_combo_ogl_1.Visible = ((bool)(resources.GetObject("label_extra2_combo_ogl_1.Visible")));
      this.label_extra2_combo_ogl_1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_ogl_4
      // 
      this.label_extra2_combo_ogl_4.AccessibleDescription = resources.GetString("label_extra2_combo_ogl_4.AccessibleDescription");
      this.label_extra2_combo_ogl_4.AccessibleName = resources.GetString("label_extra2_combo_ogl_4.AccessibleName");
      this.label_extra2_combo_ogl_4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_ogl_4.Anchor")));
      this.label_extra2_combo_ogl_4.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_ogl_4.AutoSize")));
      this.label_extra2_combo_ogl_4.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_ogl_4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_ogl_4.Dock")));
      this.label_extra2_combo_ogl_4.Enabled = ((bool)(resources.GetObject("label_extra2_combo_ogl_4.Enabled")));
      this.label_extra2_combo_ogl_4.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_ogl_4.Font")));
      this.label_extra2_combo_ogl_4.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_ogl_4.Image")));
      this.label_extra2_combo_ogl_4.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_4.ImageAlign")));
      this.label_extra2_combo_ogl_4.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_4.ImageIndex")));
      this.label_extra2_combo_ogl_4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_ogl_4.ImeMode")));
      this.label_extra2_combo_ogl_4.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_ogl_4.Location")));
      this.label_extra2_combo_ogl_4.Name = "label_extra2_combo_ogl_4";
      this.label_extra2_combo_ogl_4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_ogl_4.RightToLeft")));
      this.label_extra2_combo_ogl_4.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_ogl_4.Size")));
      this.label_extra2_combo_ogl_4.TabIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_4.TabIndex")));
      this.label_extra2_combo_ogl_4.Tag = "3";
      this.label_extra2_combo_ogl_4.Text = resources.GetString("label_extra2_combo_ogl_4.Text");
      this.label_extra2_combo_ogl_4.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_4.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_ogl_4, resources.GetString("label_extra2_combo_ogl_4.ToolTip"));
      this.label_extra2_combo_ogl_4.Visible = ((bool)(resources.GetObject("label_extra2_combo_ogl_4.Visible")));
      this.label_extra2_combo_ogl_4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_ogl_5
      // 
      this.label_extra2_combo_ogl_5.AccessibleDescription = resources.GetString("label_extra2_combo_ogl_5.AccessibleDescription");
      this.label_extra2_combo_ogl_5.AccessibleName = resources.GetString("label_extra2_combo_ogl_5.AccessibleName");
      this.label_extra2_combo_ogl_5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_ogl_5.Anchor")));
      this.label_extra2_combo_ogl_5.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_ogl_5.AutoSize")));
      this.label_extra2_combo_ogl_5.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_ogl_5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_ogl_5.Dock")));
      this.label_extra2_combo_ogl_5.Enabled = ((bool)(resources.GetObject("label_extra2_combo_ogl_5.Enabled")));
      this.label_extra2_combo_ogl_5.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_ogl_5.Font")));
      this.label_extra2_combo_ogl_5.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_ogl_5.Image")));
      this.label_extra2_combo_ogl_5.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_5.ImageAlign")));
      this.label_extra2_combo_ogl_5.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_5.ImageIndex")));
      this.label_extra2_combo_ogl_5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_ogl_5.ImeMode")));
      this.label_extra2_combo_ogl_5.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_ogl_5.Location")));
      this.label_extra2_combo_ogl_5.Name = "label_extra2_combo_ogl_5";
      this.label_extra2_combo_ogl_5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_ogl_5.RightToLeft")));
      this.label_extra2_combo_ogl_5.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_ogl_5.Size")));
      this.label_extra2_combo_ogl_5.TabIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_5.TabIndex")));
      this.label_extra2_combo_ogl_5.Tag = "4";
      this.label_extra2_combo_ogl_5.Text = resources.GetString("label_extra2_combo_ogl_5.Text");
      this.label_extra2_combo_ogl_5.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_5.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_ogl_5, resources.GetString("label_extra2_combo_ogl_5.ToolTip"));
      this.label_extra2_combo_ogl_5.Visible = ((bool)(resources.GetObject("label_extra2_combo_ogl_5.Visible")));
      this.label_extra2_combo_ogl_5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_ogl_3
      // 
      this.label_extra2_combo_ogl_3.AccessibleDescription = resources.GetString("label_extra2_combo_ogl_3.AccessibleDescription");
      this.label_extra2_combo_ogl_3.AccessibleName = resources.GetString("label_extra2_combo_ogl_3.AccessibleName");
      this.label_extra2_combo_ogl_3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_ogl_3.Anchor")));
      this.label_extra2_combo_ogl_3.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_ogl_3.AutoSize")));
      this.label_extra2_combo_ogl_3.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_ogl_3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_ogl_3.Dock")));
      this.label_extra2_combo_ogl_3.Enabled = ((bool)(resources.GetObject("label_extra2_combo_ogl_3.Enabled")));
      this.label_extra2_combo_ogl_3.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_ogl_3.Font")));
      this.label_extra2_combo_ogl_3.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_ogl_3.Image")));
      this.label_extra2_combo_ogl_3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_3.ImageAlign")));
      this.label_extra2_combo_ogl_3.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_3.ImageIndex")));
      this.label_extra2_combo_ogl_3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_ogl_3.ImeMode")));
      this.label_extra2_combo_ogl_3.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_ogl_3.Location")));
      this.label_extra2_combo_ogl_3.Name = "label_extra2_combo_ogl_3";
      this.label_extra2_combo_ogl_3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_ogl_3.RightToLeft")));
      this.label_extra2_combo_ogl_3.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_ogl_3.Size")));
      this.label_extra2_combo_ogl_3.TabIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_3.TabIndex")));
      this.label_extra2_combo_ogl_3.Tag = "2";
      this.label_extra2_combo_ogl_3.Text = resources.GetString("label_extra2_combo_ogl_3.Text");
      this.label_extra2_combo_ogl_3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_3.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_ogl_3, resources.GetString("label_extra2_combo_ogl_3.ToolTip"));
      this.label_extra2_combo_ogl_3.Visible = ((bool)(resources.GetObject("label_extra2_combo_ogl_3.Visible")));
      this.label_extra2_combo_ogl_3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_ogl_2
      // 
      this.label_extra2_combo_ogl_2.AccessibleDescription = resources.GetString("label_extra2_combo_ogl_2.AccessibleDescription");
      this.label_extra2_combo_ogl_2.AccessibleName = resources.GetString("label_extra2_combo_ogl_2.AccessibleName");
      this.label_extra2_combo_ogl_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_ogl_2.Anchor")));
      this.label_extra2_combo_ogl_2.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_ogl_2.AutoSize")));
      this.label_extra2_combo_ogl_2.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_ogl_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_ogl_2.Dock")));
      this.label_extra2_combo_ogl_2.Enabled = ((bool)(resources.GetObject("label_extra2_combo_ogl_2.Enabled")));
      this.label_extra2_combo_ogl_2.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_ogl_2.Font")));
      this.label_extra2_combo_ogl_2.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_ogl_2.Image")));
      this.label_extra2_combo_ogl_2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_2.ImageAlign")));
      this.label_extra2_combo_ogl_2.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_2.ImageIndex")));
      this.label_extra2_combo_ogl_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_ogl_2.ImeMode")));
      this.label_extra2_combo_ogl_2.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_ogl_2.Location")));
      this.label_extra2_combo_ogl_2.Name = "label_extra2_combo_ogl_2";
      this.label_extra2_combo_ogl_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_ogl_2.RightToLeft")));
      this.label_extra2_combo_ogl_2.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_ogl_2.Size")));
      this.label_extra2_combo_ogl_2.TabIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_2.TabIndex")));
      this.label_extra2_combo_ogl_2.Tag = "1";
      this.label_extra2_combo_ogl_2.Text = resources.GetString("label_extra2_combo_ogl_2.Text");
      this.label_extra2_combo_ogl_2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_2.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_ogl_2, resources.GetString("label_extra2_combo_ogl_2.ToolTip"));
      this.label_extra2_combo_ogl_2.Visible = ((bool)(resources.GetObject("label_extra2_combo_ogl_2.Visible")));
      this.label_extra2_combo_ogl_2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // label_extra2_combo_ogl_6
      // 
      this.label_extra2_combo_ogl_6.AccessibleDescription = resources.GetString("label_extra2_combo_ogl_6.AccessibleDescription");
      this.label_extra2_combo_ogl_6.AccessibleName = resources.GetString("label_extra2_combo_ogl_6.AccessibleName");
      this.label_extra2_combo_ogl_6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_extra2_combo_ogl_6.Anchor")));
      this.label_extra2_combo_ogl_6.AutoSize = ((bool)(resources.GetObject("label_extra2_combo_ogl_6.AutoSize")));
      this.label_extra2_combo_ogl_6.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_extra2_combo_ogl_6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_extra2_combo_ogl_6.Dock")));
      this.label_extra2_combo_ogl_6.Enabled = ((bool)(resources.GetObject("label_extra2_combo_ogl_6.Enabled")));
      this.label_extra2_combo_ogl_6.Font = ((System.Drawing.Font)(resources.GetObject("label_extra2_combo_ogl_6.Font")));
      this.label_extra2_combo_ogl_6.Image = ((System.Drawing.Image)(resources.GetObject("label_extra2_combo_ogl_6.Image")));
      this.label_extra2_combo_ogl_6.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_6.ImageAlign")));
      this.label_extra2_combo_ogl_6.ImageIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_6.ImageIndex")));
      this.label_extra2_combo_ogl_6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_extra2_combo_ogl_6.ImeMode")));
      this.label_extra2_combo_ogl_6.Location = ((System.Drawing.Point)(resources.GetObject("label_extra2_combo_ogl_6.Location")));
      this.label_extra2_combo_ogl_6.Name = "label_extra2_combo_ogl_6";
      this.label_extra2_combo_ogl_6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_extra2_combo_ogl_6.RightToLeft")));
      this.label_extra2_combo_ogl_6.Size = ((System.Drawing.Size)(resources.GetObject("label_extra2_combo_ogl_6.Size")));
      this.label_extra2_combo_ogl_6.TabIndex = ((int)(resources.GetObject("label_extra2_combo_ogl_6.TabIndex")));
      this.label_extra2_combo_ogl_6.Tag = "5";
      this.label_extra2_combo_ogl_6.Text = resources.GetString("label_extra2_combo_ogl_6.Text");
      this.label_extra2_combo_ogl_6.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_extra2_combo_ogl_6.TextAlign")));
      this.toolTip.SetToolTip(this.label_extra2_combo_ogl_6, resources.GetString("label_extra2_combo_ogl_6.ToolTip"));
      this.label_extra2_combo_ogl_6.Visible = ((bool)(resources.GetObject("label_extra2_combo_ogl_6.Visible")));
      this.label_extra2_combo_ogl_6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
      // 
      // combo_prof_img
      // 
      this.combo_prof_img.AccessibleDescription = resources.GetString("combo_prof_img.AccessibleDescription");
      this.combo_prof_img.AccessibleName = resources.GetString("combo_prof_img.AccessibleName");
      this.combo_prof_img.AllowDrop = true;
      this.combo_prof_img.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_prof_img.Anchor")));
      this.combo_prof_img.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_prof_img.BackgroundImage")));
      this.combo_prof_img.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_prof_img.Dock")));
      this.combo_prof_img.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_prof_img.DropDownWidth = 450;
      this.combo_prof_img.Enabled = ((bool)(resources.GetObject("combo_prof_img.Enabled")));
      this.combo_prof_img.Font = ((System.Drawing.Font)(resources.GetObject("combo_prof_img.Font")));
      this.combo_prof_img.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_prof_img.ImeMode")));
      this.combo_prof_img.IntegralHeight = ((bool)(resources.GetObject("combo_prof_img.IntegralHeight")));
      this.combo_prof_img.ItemHeight = ((int)(resources.GetObject("combo_prof_img.ItemHeight")));
      this.combo_prof_img.Location = ((System.Drawing.Point)(resources.GetObject("combo_prof_img.Location")));
      this.combo_prof_img.MaxDropDownItems = ((int)(resources.GetObject("combo_prof_img.MaxDropDownItems")));
      this.combo_prof_img.MaxLength = ((int)(resources.GetObject("combo_prof_img.MaxLength")));
      this.combo_prof_img.Name = "combo_prof_img";
      this.combo_prof_img.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_prof_img.RightToLeft")));
      this.combo_prof_img.Size = ((System.Drawing.Size)(resources.GetObject("combo_prof_img.Size")));
      this.combo_prof_img.TabIndex = ((int)(resources.GetObject("combo_prof_img.TabIndex")));
      this.combo_prof_img.Text = resources.GetString("combo_prof_img.Text");
      this.toolTip.SetToolTip(this.combo_prof_img, resources.GetString("combo_prof_img.ToolTip"));
      this.combo_prof_img.Visible = ((bool)(resources.GetObject("combo_prof_img.Visible")));
      this.combo_prof_img.DragDrop += new System.Windows.Forms.DragEventHandler(this.combo_prof_img_DragDrop);
      this.combo_prof_img.TextChanged += new System.EventHandler(this.combo_prof_img_TextChanged);
      this.combo_prof_img.SelectedIndexChanged += new System.EventHandler(this.combo_prof_img_SelectedIndexChanged);
      this.combo_prof_img.Leave += new System.EventHandler(this.text_prof_exe_args_Leave);
      this.combo_prof_img.DragEnter += new System.Windows.Forms.DragEventHandler(this.combo_prof_img_DragEnter);
      this.combo_prof_img.Enter += new System.EventHandler(this.text_prof_exe_args_Enter);
      // 
      // text_prof_exe_args
      // 
      this.text_prof_exe_args.AccessibleDescription = resources.GetString("text_prof_exe_args.AccessibleDescription");
      this.text_prof_exe_args.AccessibleName = resources.GetString("text_prof_exe_args.AccessibleName");
      this.text_prof_exe_args.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_prof_exe_args.Anchor")));
      this.text_prof_exe_args.AutoSize = ((bool)(resources.GetObject("text_prof_exe_args.AutoSize")));
      this.text_prof_exe_args.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_prof_exe_args.BackgroundImage")));
      this.text_prof_exe_args.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_prof_exe_args.Dock")));
      this.text_prof_exe_args.Enabled = ((bool)(resources.GetObject("text_prof_exe_args.Enabled")));
      this.text_prof_exe_args.Font = ((System.Drawing.Font)(resources.GetObject("text_prof_exe_args.Font")));
      this.text_prof_exe_args.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_prof_exe_args.ImeMode")));
      this.text_prof_exe_args.Location = ((System.Drawing.Point)(resources.GetObject("text_prof_exe_args.Location")));
      this.text_prof_exe_args.MaxLength = ((int)(resources.GetObject("text_prof_exe_args.MaxLength")));
      this.text_prof_exe_args.Multiline = ((bool)(resources.GetObject("text_prof_exe_args.Multiline")));
      this.text_prof_exe_args.Name = "text_prof_exe_args";
      this.text_prof_exe_args.PasswordChar = ((char)(resources.GetObject("text_prof_exe_args.PasswordChar")));
      this.text_prof_exe_args.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_prof_exe_args.RightToLeft")));
      this.text_prof_exe_args.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_prof_exe_args.ScrollBars")));
      this.text_prof_exe_args.Size = ((System.Drawing.Size)(resources.GetObject("text_prof_exe_args.Size")));
      this.text_prof_exe_args.TabIndex = ((int)(resources.GetObject("text_prof_exe_args.TabIndex")));
      this.text_prof_exe_args.Text = resources.GetString("text_prof_exe_args.Text");
      this.text_prof_exe_args.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_prof_exe_args.TextAlign")));
      this.toolTip.SetToolTip(this.text_prof_exe_args, resources.GetString("text_prof_exe_args.ToolTip"));
      this.text_prof_exe_args.Visible = ((bool)(resources.GetObject("text_prof_exe_args.Visible")));
      this.text_prof_exe_args.WordWrap = ((bool)(resources.GetObject("text_prof_exe_args.WordWrap")));
      this.text_prof_exe_args.TextChanged += new System.EventHandler(this.text_prof_exe_args_TextChanged);
      this.text_prof_exe_args.Leave += new System.EventHandler(this.text_prof_exe_args_Leave);
      this.text_prof_exe_args.Enter += new System.EventHandler(this.text_prof_exe_args_Enter);
      // 
      // text_prof_exe_path
      // 
      this.text_prof_exe_path.AccessibleDescription = resources.GetString("text_prof_exe_path.AccessibleDescription");
      this.text_prof_exe_path.AccessibleName = resources.GetString("text_prof_exe_path.AccessibleName");
      this.text_prof_exe_path.AllowDrop = true;
      this.text_prof_exe_path.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_prof_exe_path.Anchor")));
      this.text_prof_exe_path.AutoSize = ((bool)(resources.GetObject("text_prof_exe_path.AutoSize")));
      this.text_prof_exe_path.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_prof_exe_path.BackgroundImage")));
      this.text_prof_exe_path.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_prof_exe_path.Dock")));
      this.text_prof_exe_path.Enabled = ((bool)(resources.GetObject("text_prof_exe_path.Enabled")));
      this.text_prof_exe_path.Font = ((System.Drawing.Font)(resources.GetObject("text_prof_exe_path.Font")));
      this.text_prof_exe_path.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_prof_exe_path.ImeMode")));
      this.text_prof_exe_path.Location = ((System.Drawing.Point)(resources.GetObject("text_prof_exe_path.Location")));
      this.text_prof_exe_path.MaxLength = ((int)(resources.GetObject("text_prof_exe_path.MaxLength")));
      this.text_prof_exe_path.Multiline = ((bool)(resources.GetObject("text_prof_exe_path.Multiline")));
      this.text_prof_exe_path.Name = "text_prof_exe_path";
      this.text_prof_exe_path.PasswordChar = ((char)(resources.GetObject("text_prof_exe_path.PasswordChar")));
      this.text_prof_exe_path.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_prof_exe_path.RightToLeft")));
      this.text_prof_exe_path.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_prof_exe_path.ScrollBars")));
      this.text_prof_exe_path.Size = ((System.Drawing.Size)(resources.GetObject("text_prof_exe_path.Size")));
      this.text_prof_exe_path.TabIndex = ((int)(resources.GetObject("text_prof_exe_path.TabIndex")));
      this.text_prof_exe_path.Text = resources.GetString("text_prof_exe_path.Text");
      this.text_prof_exe_path.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_prof_exe_path.TextAlign")));
      this.toolTip.SetToolTip(this.text_prof_exe_path, resources.GetString("text_prof_exe_path.ToolTip"));
      this.text_prof_exe_path.Visible = ((bool)(resources.GetObject("text_prof_exe_path.Visible")));
      this.text_prof_exe_path.WordWrap = ((bool)(resources.GetObject("text_prof_exe_path.WordWrap")));
      this.text_prof_exe_path.DragDrop += new System.Windows.Forms.DragEventHandler(this.text_prof_exe_path_DragDrop);
      this.text_prof_exe_path.TextChanged += new System.EventHandler(this.text_prof_exe_path_TextChanged);
      this.text_prof_exe_path.DragEnter += new System.Windows.Forms.DragEventHandler(this.text_prof_exe_path_DragEnter);
      this.text_prof_exe_path.Leave += new System.EventHandler(this.text_prof_exe_args_Leave);
      this.text_prof_exe_path.Enter += new System.EventHandler(this.text_prof_exe_args_Enter);
      // 
      // mainMenu
      // 
      this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                             this.menu_file,
                                                                             this.menu_options,
                                                                             this.menu_tools,
                                                                             this.menu_profile,
                                                                             this.menu_profs,
                                                                             this.menu_help,
                                                                             this.menu_exp});
      this.mainMenu.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("mainMenu.RightToLeft")));
      // 
      // menu_file
      // 
      this.menu_file.Enabled = ((bool)(resources.GetObject("menu_file.Enabled")));
      this.menu_file.Index = 0;
      this.menu_file.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                              this.menu_file_loadprofs,
                                                                              this.menu_file_reloadCurrDriverSettings,
                                                                              this.menuItem16,
                                                                              this.menu_file_iconifyTray,
                                                                              this.menu_file_quit});
      this.menu_file.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file.Shortcut")));
      this.menu_file.ShowShortcut = ((bool)(resources.GetObject("menu_file.ShowShortcut")));
      this.menu_file.Text = resources.GetString("menu_file.Text");
      this.menu_file.Visible = ((bool)(resources.GetObject("menu_file.Visible")));
      // 
      // menu_file_loadprofs
      // 
      this.menu_file_loadprofs.Enabled = ((bool)(resources.GetObject("menu_file_loadprofs.Enabled")));
      this.menu_file_loadprofs.Index = 0;
      this.menu_file_loadprofs.OwnerDraw = true;
      this.menu_file_loadprofs.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_loadprofs.Shortcut")));
      this.menu_file_loadprofs.ShowShortcut = ((bool)(resources.GetObject("menu_file_loadprofs.ShowShortcut")));
      this.menu_file_loadprofs.Text = resources.GetString("menu_file_loadprofs.Text");
      this.menu_file_loadprofs.Visible = ((bool)(resources.GetObject("menu_file_loadprofs.Visible")));
      this.menu_file_loadprofs.Click += new System.EventHandler(this.menu_file_loadprofs_Click);
      // 
      // menu_file_reloadCurrDriverSettings
      // 
      this.menu_file_reloadCurrDriverSettings.Enabled = ((bool)(resources.GetObject("menu_file_reloadCurrDriverSettings.Enabled")));
      this.menu_file_reloadCurrDriverSettings.Index = 1;
      this.menu_file_reloadCurrDriverSettings.OwnerDraw = true;
      this.menu_file_reloadCurrDriverSettings.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_reloadCurrDriverSettings.Shortcut")));
      this.menu_file_reloadCurrDriverSettings.ShowShortcut = ((bool)(resources.GetObject("menu_file_reloadCurrDriverSettings.ShowShortcut")));
      this.menu_file_reloadCurrDriverSettings.Text = resources.GetString("menu_file_reloadCurrDriverSettings.Text");
      this.menu_file_reloadCurrDriverSettings.Visible = ((bool)(resources.GetObject("menu_file_reloadCurrDriverSettings.Visible")));
      this.menu_file_reloadCurrDriverSettings.Click += new System.EventHandler(this.menu_file_reloadCurrDriverSettings_Click);
      // 
      // menuItem16
      // 
      this.menuItem16.Enabled = ((bool)(resources.GetObject("menuItem16.Enabled")));
      this.menuItem16.Index = 2;
      this.menuItem16.OwnerDraw = true;
      this.menuItem16.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem16.Shortcut")));
      this.menuItem16.ShowShortcut = ((bool)(resources.GetObject("menuItem16.ShowShortcut")));
      this.menuItem16.Text = resources.GetString("menuItem16.Text");
      this.menuItem16.Visible = ((bool)(resources.GetObject("menuItem16.Visible")));
      // 
      // menu_file_iconifyTray
      // 
      this.menu_file_iconifyTray.Enabled = ((bool)(resources.GetObject("menu_file_iconifyTray.Enabled")));
      this.menu_file_iconifyTray.Index = 3;
      this.menu_file_iconifyTray.OwnerDraw = true;
      this.menu_file_iconifyTray.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_iconifyTray.Shortcut")));
      this.menu_file_iconifyTray.ShowShortcut = ((bool)(resources.GetObject("menu_file_iconifyTray.ShowShortcut")));
      this.menu_file_iconifyTray.Text = resources.GetString("menu_file_iconifyTray.Text");
      this.menu_file_iconifyTray.Visible = ((bool)(resources.GetObject("menu_file_iconifyTray.Visible")));
      this.menu_file_iconifyTray.Click += new System.EventHandler(this.menu_file_iconifyTray_Click);
      // 
      // menu_file_quit
      // 
      this.menu_file_quit.Enabled = ((bool)(resources.GetObject("menu_file_quit.Enabled")));
      this.menu_file_quit.Index = 4;
      this.menu_file_quit.OwnerDraw = true;
      this.menu_file_quit.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_quit.Shortcut")));
      this.menu_file_quit.ShowShortcut = ((bool)(resources.GetObject("menu_file_quit.ShowShortcut")));
      this.menu_file_quit.Text = resources.GetString("menu_file_quit.Text");
      this.menu_file_quit.Visible = ((bool)(resources.GetObject("menu_file_quit.Visible")));
      this.menu_file_quit.Click += new System.EventHandler(this.menu_file_quit_Click);
      // 
      // menu_options
      // 
      this.menu_options.Enabled = ((bool)(resources.GetObject("menu_options.Enabled")));
      this.menu_options.Index = 1;
      this.menu_options.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                 this.menu_opts_lang,
                                                                                 this.menuItem1,
                                                                                 this.menu_opts_regreadonly,
                                                                                 this.menuItem17,
                                                                                 this.menu_opts_menuIcons,
                                                                                 this.menu_opt_3DCheckBoxes,
                                                                                 this.menuItem8,
                                                                                 this.menu_opts_multiUser,
                                                                                 this.menu_opts_autoStart,
                                                                                 this.menuItem15,
                                                                                 this.menu_opts_hotkeys,
                                                                                 this.menuItem27,
                                                                                 this.menu_opts_settings});
      this.menu_options.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_options.Shortcut")));
      this.menu_options.ShowShortcut = ((bool)(resources.GetObject("menu_options.ShowShortcut")));
      this.menu_options.Text = resources.GetString("menu_options.Text");
      this.menu_options.Visible = ((bool)(resources.GetObject("menu_options.Visible")));
      // 
      // menu_opts_lang
      // 
      this.menu_opts_lang.Enabled = ((bool)(resources.GetObject("menu_opts_lang.Enabled")));
      this.menu_opts_lang.Index = 0;
      this.menu_opts_lang.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                   this.menu_opt_lang_auto,
                                                                                   this.menu_opt_lang_en,
                                                                                   this.menu_opt_lang_de});
      this.menu_opts_lang.OwnerDraw = true;
      this.menu_opts_lang.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opts_lang.Shortcut")));
      this.menu_opts_lang.ShowShortcut = ((bool)(resources.GetObject("menu_opts_lang.ShowShortcut")));
      this.menu_opts_lang.Text = resources.GetString("menu_opts_lang.Text");
      this.menu_opts_lang.Visible = ((bool)(resources.GetObject("menu_opts_lang.Visible")));
      // 
      // menu_opt_lang_auto
      // 
      this.menu_opt_lang_auto.Enabled = ((bool)(resources.GetObject("menu_opt_lang_auto.Enabled")));
      this.menu_opt_lang_auto.Index = 0;
      this.menu_opt_lang_auto.OwnerDraw = true;
      this.menu_opt_lang_auto.RadioCheck = true;
      this.menu_opt_lang_auto.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opt_lang_auto.Shortcut")));
      this.menu_opt_lang_auto.ShowShortcut = ((bool)(resources.GetObject("menu_opt_lang_auto.ShowShortcut")));
      this.menu_opt_lang_auto.Text = resources.GetString("menu_opt_lang_auto.Text");
      this.menu_opt_lang_auto.Visible = ((bool)(resources.GetObject("menu_opt_lang_auto.Visible")));
      this.menu_opt_lang_auto.Click += new System.EventHandler(this.menu_opt_lang_Click);
      // 
      // menu_opt_lang_en
      // 
      this.menu_opt_lang_en.Enabled = ((bool)(resources.GetObject("menu_opt_lang_en.Enabled")));
      this.menu_opt_lang_en.Index = 1;
      this.menu_opt_lang_en.OwnerDraw = true;
      this.menu_opt_lang_en.RadioCheck = true;
      this.menu_opt_lang_en.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opt_lang_en.Shortcut")));
      this.menu_opt_lang_en.ShowShortcut = ((bool)(resources.GetObject("menu_opt_lang_en.ShowShortcut")));
      this.menu_opt_lang_en.Text = resources.GetString("menu_opt_lang_en.Text");
      this.menu_opt_lang_en.Visible = ((bool)(resources.GetObject("menu_opt_lang_en.Visible")));
      this.menu_opt_lang_en.Click += new System.EventHandler(this.menu_opt_lang_Click);
      // 
      // menu_opt_lang_de
      // 
      this.menu_opt_lang_de.Enabled = ((bool)(resources.GetObject("menu_opt_lang_de.Enabled")));
      this.menu_opt_lang_de.Index = 2;
      this.menu_opt_lang_de.OwnerDraw = true;
      this.menu_opt_lang_de.RadioCheck = true;
      this.menu_opt_lang_de.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opt_lang_de.Shortcut")));
      this.menu_opt_lang_de.ShowShortcut = ((bool)(resources.GetObject("menu_opt_lang_de.ShowShortcut")));
      this.menu_opt_lang_de.Text = resources.GetString("menu_opt_lang_de.Text");
      this.menu_opt_lang_de.Visible = ((bool)(resources.GetObject("menu_opt_lang_de.Visible")));
      this.menu_opt_lang_de.Click += new System.EventHandler(this.menu_opt_lang_Click);
      // 
      // menuItem1
      // 
      this.menuItem1.Enabled = ((bool)(resources.GetObject("menuItem1.Enabled")));
      this.menuItem1.Index = 1;
      this.menuItem1.OwnerDraw = true;
      this.menuItem1.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem1.Shortcut")));
      this.menuItem1.ShowShortcut = ((bool)(resources.GetObject("menuItem1.ShowShortcut")));
      this.menuItem1.Text = resources.GetString("menuItem1.Text");
      this.menuItem1.Visible = ((bool)(resources.GetObject("menuItem1.Visible")));
      // 
      // menu_opts_regreadonly
      // 
      this.menu_opts_regreadonly.Checked = true;
      this.menu_opts_regreadonly.Enabled = ((bool)(resources.GetObject("menu_opts_regreadonly.Enabled")));
      this.menu_opts_regreadonly.Index = 2;
      this.menu_opts_regreadonly.OwnerDraw = true;
      this.menu_opts_regreadonly.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opts_regreadonly.Shortcut")));
      this.menu_opts_regreadonly.ShowShortcut = ((bool)(resources.GetObject("menu_opts_regreadonly.ShowShortcut")));
      this.menu_opts_regreadonly.Text = resources.GetString("menu_opts_regreadonly.Text");
      this.menu_opts_regreadonly.Visible = ((bool)(resources.GetObject("menu_opts_regreadonly.Visible")));
      this.menu_opts_regreadonly.Click += new System.EventHandler(this.menu_opts_regreadonly_Click);
      // 
      // menuItem17
      // 
      this.menuItem17.Enabled = ((bool)(resources.GetObject("menuItem17.Enabled")));
      this.menuItem17.Index = 3;
      this.menuItem17.OwnerDraw = true;
      this.menuItem17.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem17.Shortcut")));
      this.menuItem17.ShowShortcut = ((bool)(resources.GetObject("menuItem17.ShowShortcut")));
      this.menuItem17.Text = resources.GetString("menuItem17.Text");
      this.menuItem17.Visible = ((bool)(resources.GetObject("menuItem17.Visible")));
      // 
      // menu_opts_menuIcons
      // 
      this.menu_opts_menuIcons.Enabled = ((bool)(resources.GetObject("menu_opts_menuIcons.Enabled")));
      this.menu_opts_menuIcons.Index = 4;
      this.menu_opts_menuIcons.OwnerDraw = true;
      this.menu_opts_menuIcons.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opts_menuIcons.Shortcut")));
      this.menu_opts_menuIcons.ShowShortcut = ((bool)(resources.GetObject("menu_opts_menuIcons.ShowShortcut")));
      this.menu_opts_menuIcons.Text = resources.GetString("menu_opts_menuIcons.Text");
      this.menu_opts_menuIcons.Visible = ((bool)(resources.GetObject("menu_opts_menuIcons.Visible")));
      this.menu_opts_menuIcons.Click += new System.EventHandler(this.menuItem13_Click);
      // 
      // menu_opt_3DCheckBoxes
      // 
      this.menu_opt_3DCheckBoxes.Enabled = ((bool)(resources.GetObject("menu_opt_3DCheckBoxes.Enabled")));
      this.menu_opt_3DCheckBoxes.Index = 5;
      this.menu_opt_3DCheckBoxes.OwnerDraw = true;
      this.menu_opt_3DCheckBoxes.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opt_3DCheckBoxes.Shortcut")));
      this.menu_opt_3DCheckBoxes.ShowShortcut = ((bool)(resources.GetObject("menu_opt_3DCheckBoxes.ShowShortcut")));
      this.menu_opt_3DCheckBoxes.Text = resources.GetString("menu_opt_3DCheckBoxes.Text");
      this.menu_opt_3DCheckBoxes.Visible = ((bool)(resources.GetObject("menu_opt_3DCheckBoxes.Visible")));
      this.menu_opt_3DCheckBoxes.Click += new System.EventHandler(this.menu_opt_3DCheckBoxes_Click);
      // 
      // menuItem8
      // 
      this.menuItem8.Enabled = ((bool)(resources.GetObject("menuItem8.Enabled")));
      this.menuItem8.Index = 6;
      this.menuItem8.OwnerDraw = true;
      this.menuItem8.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem8.Shortcut")));
      this.menuItem8.ShowShortcut = ((bool)(resources.GetObject("menuItem8.ShowShortcut")));
      this.menuItem8.Text = resources.GetString("menuItem8.Text");
      this.menuItem8.Visible = ((bool)(resources.GetObject("menuItem8.Visible")));
      // 
      // menu_opts_multiUser
      // 
      this.menu_opts_multiUser.Enabled = ((bool)(resources.GetObject("menu_opts_multiUser.Enabled")));
      this.menu_opts_multiUser.Index = 7;
      this.menu_opts_multiUser.OwnerDraw = true;
      this.menu_opts_multiUser.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opts_multiUser.Shortcut")));
      this.menu_opts_multiUser.ShowShortcut = ((bool)(resources.GetObject("menu_opts_multiUser.ShowShortcut")));
      this.menu_opts_multiUser.Text = resources.GetString("menu_opts_multiUser.Text");
      this.menu_opts_multiUser.Visible = ((bool)(resources.GetObject("menu_opts_multiUser.Visible")));
      this.menu_opts_multiUser.Click += new System.EventHandler(this.menu_opts_multiUser_Click);
      // 
      // menu_opts_autoStart
      // 
      this.menu_opts_autoStart.Enabled = ((bool)(resources.GetObject("menu_opts_autoStart.Enabled")));
      this.menu_opts_autoStart.Index = 8;
      this.menu_opts_autoStart.OwnerDraw = true;
      this.menu_opts_autoStart.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opts_autoStart.Shortcut")));
      this.menu_opts_autoStart.ShowShortcut = ((bool)(resources.GetObject("menu_opts_autoStart.ShowShortcut")));
      this.menu_opts_autoStart.Text = resources.GetString("menu_opts_autoStart.Text");
      this.menu_opts_autoStart.Visible = ((bool)(resources.GetObject("menu_opts_autoStart.Visible")));
      this.menu_opts_autoStart.Click += new System.EventHandler(this.menu_opts_autoStart_Click);
      // 
      // menuItem15
      // 
      this.menuItem15.Enabled = ((bool)(resources.GetObject("menuItem15.Enabled")));
      this.menuItem15.Index = 9;
      this.menuItem15.OwnerDraw = true;
      this.menuItem15.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem15.Shortcut")));
      this.menuItem15.ShowShortcut = ((bool)(resources.GetObject("menuItem15.ShowShortcut")));
      this.menuItem15.Text = resources.GetString("menuItem15.Text");
      this.menuItem15.Visible = ((bool)(resources.GetObject("menuItem15.Visible")));
      // 
      // menu_opts_hotkeys
      // 
      this.menu_opts_hotkeys.Enabled = ((bool)(resources.GetObject("menu_opts_hotkeys.Enabled")));
      this.menu_opts_hotkeys.Index = 10;
      this.menu_opts_hotkeys.OwnerDraw = true;
      this.menu_opts_hotkeys.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opts_hotkeys.Shortcut")));
      this.menu_opts_hotkeys.ShowShortcut = ((bool)(resources.GetObject("menu_opts_hotkeys.ShowShortcut")));
      this.menu_opts_hotkeys.Text = resources.GetString("menu_opts_hotkeys.Text");
      this.menu_opts_hotkeys.Visible = ((bool)(resources.GetObject("menu_opts_hotkeys.Visible")));
      this.menu_opts_hotkeys.Click += new System.EventHandler(this.menu_prof_hotkeys_Click);
      // 
      // menuItem27
      // 
      this.menuItem27.Enabled = ((bool)(resources.GetObject("menuItem27.Enabled")));
      this.menuItem27.Index = 11;
      this.menuItem27.OwnerDraw = true;
      this.menuItem27.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem27.Shortcut")));
      this.menuItem27.ShowShortcut = ((bool)(resources.GetObject("menuItem27.ShowShortcut")));
      this.menuItem27.Text = resources.GetString("menuItem27.Text");
      this.menuItem27.Visible = ((bool)(resources.GetObject("menuItem27.Visible")));
      // 
      // menu_opts_settings
      // 
      this.menu_opts_settings.Enabled = ((bool)(resources.GetObject("menu_opts_settings.Enabled")));
      this.menu_opts_settings.Index = 12;
      this.menu_opts_settings.OwnerDraw = true;
      this.menu_opts_settings.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_opts_settings.Shortcut")));
      this.menu_opts_settings.ShowShortcut = ((bool)(resources.GetObject("menu_opts_settings.ShowShortcut")));
      this.menu_opts_settings.Text = resources.GetString("menu_opts_settings.Text");
      this.menu_opts_settings.Visible = ((bool)(resources.GetObject("menu_opts_settings.Visible")));
      this.menu_opts_settings.Click += new System.EventHandler(this.menu_opts_settings_Click);
      // 
      // menu_tools
      // 
      this.menu_tools.Enabled = ((bool)(resources.GetObject("menu_tools.Enabled")));
      this.menu_tools.Index = 2;
      this.menu_tools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                               this.menu_tools_openRegedit,
                                                                               this.menu_exp_testRdKey,
                                                                               this.menu_tools_regdiff_ati,
                                                                               this.menuItem5,
                                                                               this.menu_tools_nvclock,
                                                                               this.menu_nvCoolBits,
                                                                               this.menu_ati_D3DApply,
                                                                               this.menu_ati_open_cpl,
                                                                               this.menu_ati_open_oldCpl,
                                                                               this.menu_winTweaks_Separator,
                                                                               this.menu_winTweaks,
                                                                               this.menuItem21,
                                                                               this.menu_tools_undoApply});
      this.menu_tools.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tools.Shortcut")));
      this.menu_tools.ShowShortcut = ((bool)(resources.GetObject("menu_tools.ShowShortcut")));
      this.menu_tools.Text = resources.GetString("menu_tools.Text");
      this.menu_tools.Visible = ((bool)(resources.GetObject("menu_tools.Visible")));
      // 
      // menu_tools_openRegedit
      // 
      this.menu_tools_openRegedit.Enabled = ((bool)(resources.GetObject("menu_tools_openRegedit.Enabled")));
      this.menu_tools_openRegedit.Index = 0;
      this.menu_tools_openRegedit.OwnerDraw = true;
      this.menu_tools_openRegedit.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tools_openRegedit.Shortcut")));
      this.menu_tools_openRegedit.ShowShortcut = ((bool)(resources.GetObject("menu_tools_openRegedit.ShowShortcut")));
      this.menu_tools_openRegedit.Text = resources.GetString("menu_tools_openRegedit.Text");
      this.menu_tools_openRegedit.Visible = ((bool)(resources.GetObject("menu_tools_openRegedit.Visible")));
      this.menu_tools_openRegedit.Click += new System.EventHandler(this.menu_tools_openRegedit_Click);
      // 
      // menu_exp_testRdKey
      // 
      this.menu_exp_testRdKey.Enabled = ((bool)(resources.GetObject("menu_exp_testRdKey.Enabled")));
      this.menu_exp_testRdKey.Index = 1;
      this.menu_exp_testRdKey.OwnerDraw = true;
      this.menu_exp_testRdKey.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_exp_testRdKey.Shortcut")));
      this.menu_exp_testRdKey.ShowShortcut = ((bool)(resources.GetObject("menu_exp_testRdKey.ShowShortcut")));
      this.menu_exp_testRdKey.Text = resources.GetString("menu_exp_testRdKey.Text");
      this.menu_exp_testRdKey.Visible = ((bool)(resources.GetObject("menu_exp_testRdKey.Visible")));
      this.menu_exp_testRdKey.Click += new System.EventHandler(this.menu_exp_testRdKey_Click);
      // 
      // menu_tools_regdiff_ati
      // 
      this.menu_tools_regdiff_ati.Enabled = ((bool)(resources.GetObject("menu_tools_regdiff_ati.Enabled")));
      this.menu_tools_regdiff_ati.Index = 2;
      this.menu_tools_regdiff_ati.OwnerDraw = true;
      this.menu_tools_regdiff_ati.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tools_regdiff_ati.Shortcut")));
      this.menu_tools_regdiff_ati.ShowShortcut = ((bool)(resources.GetObject("menu_tools_regdiff_ati.ShowShortcut")));
      this.menu_tools_regdiff_ati.Text = resources.GetString("menu_tools_regdiff_ati.Text");
      this.menu_tools_regdiff_ati.Visible = ((bool)(resources.GetObject("menu_tools_regdiff_ati.Visible")));
      this.menu_tools_regdiff_ati.Click += new System.EventHandler(this.menu_tools_regdiff_ati_Click);
      // 
      // menuItem5
      // 
      this.menuItem5.Enabled = ((bool)(resources.GetObject("menuItem5.Enabled")));
      this.menuItem5.Index = 3;
      this.menuItem5.OwnerDraw = true;
      this.menuItem5.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem5.Shortcut")));
      this.menuItem5.ShowShortcut = ((bool)(resources.GetObject("menuItem5.ShowShortcut")));
      this.menuItem5.Text = resources.GetString("menuItem5.Text");
      this.menuItem5.Visible = ((bool)(resources.GetObject("menuItem5.Visible")));
      // 
      // menu_tools_nvclock
      // 
      this.menu_tools_nvclock.Enabled = ((bool)(resources.GetObject("menu_tools_nvclock.Enabled")));
      this.menu_tools_nvclock.Index = 4;
      this.menu_tools_nvclock.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                       this.menu_tools_nvclock_log,
                                                                                       this.menuItem19,
                                                                                       this.menuItem20});
      this.menu_tools_nvclock.OwnerDraw = true;
      this.menu_tools_nvclock.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tools_nvclock.Shortcut")));
      this.menu_tools_nvclock.ShowShortcut = ((bool)(resources.GetObject("menu_tools_nvclock.ShowShortcut")));
      this.menu_tools_nvclock.Text = resources.GetString("menu_tools_nvclock.Text");
      this.menu_tools_nvclock.Visible = ((bool)(resources.GetObject("menu_tools_nvclock.Visible")));
      // 
      // menu_tools_nvclock_log
      // 
      this.menu_tools_nvclock_log.Enabled = ((bool)(resources.GetObject("menu_tools_nvclock_log.Enabled")));
      this.menu_tools_nvclock_log.Index = 0;
      this.menu_tools_nvclock_log.OwnerDraw = true;
      this.menu_tools_nvclock_log.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tools_nvclock_log.Shortcut")));
      this.menu_tools_nvclock_log.ShowShortcut = ((bool)(resources.GetObject("menu_tools_nvclock_log.ShowShortcut")));
      this.menu_tools_nvclock_log.Text = resources.GetString("menu_tools_nvclock_log.Text");
      this.menu_tools_nvclock_log.Visible = ((bool)(resources.GetObject("menu_tools_nvclock_log.Visible")));
      this.menu_tools_nvclock_log.Click += new System.EventHandler(this.menu_tools_nvclock_log_Click);
      // 
      // menuItem19
      // 
      this.menuItem19.Enabled = ((bool)(resources.GetObject("menuItem19.Enabled")));
      this.menuItem19.Index = 1;
      this.menuItem19.OwnerDraw = true;
      this.menuItem19.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem19.Shortcut")));
      this.menuItem19.ShowShortcut = ((bool)(resources.GetObject("menuItem19.ShowShortcut")));
      this.menuItem19.Text = resources.GetString("menuItem19.Text");
      this.menuItem19.Visible = ((bool)(resources.GetObject("menuItem19.Visible")));
      this.menuItem19.Click += new System.EventHandler(this.menu_tools_nvclock_log_Click);
      // 
      // menuItem20
      // 
      this.menuItem20.Enabled = ((bool)(resources.GetObject("menuItem20.Enabled")));
      this.menuItem20.Index = 2;
      this.menuItem20.OwnerDraw = true;
      this.menuItem20.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem20.Shortcut")));
      this.menuItem20.ShowShortcut = ((bool)(resources.GetObject("menuItem20.ShowShortcut")));
      this.menuItem20.Text = resources.GetString("menuItem20.Text");
      this.menuItem20.Visible = ((bool)(resources.GetObject("menuItem20.Visible")));
      this.menuItem20.Click += new System.EventHandler(this.menu_tools_nvclock_log_Click);
      // 
      // menu_nvCoolBits
      // 
      this.menu_nvCoolBits.Enabled = ((bool)(resources.GetObject("menu_nvCoolBits.Enabled")));
      this.menu_nvCoolBits.Index = 5;
      this.menu_nvCoolBits.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                    this.menu_nvCoolBits_clocking});
      this.menu_nvCoolBits.OwnerDraw = true;
      this.menu_nvCoolBits.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_nvCoolBits.Shortcut")));
      this.menu_nvCoolBits.ShowShortcut = ((bool)(resources.GetObject("menu_nvCoolBits.ShowShortcut")));
      this.menu_nvCoolBits.Text = resources.GetString("menu_nvCoolBits.Text");
      this.menu_nvCoolBits.Visible = ((bool)(resources.GetObject("menu_nvCoolBits.Visible")));
      this.menu_nvCoolBits.Popup += new System.EventHandler(this.menu_nvCoolBits_Popup);
      // 
      // menu_nvCoolBits_clocking
      // 
      this.menu_nvCoolBits_clocking.Enabled = ((bool)(resources.GetObject("menu_nvCoolBits_clocking.Enabled")));
      this.menu_nvCoolBits_clocking.Index = 0;
      this.menu_nvCoolBits_clocking.OwnerDraw = true;
      this.menu_nvCoolBits_clocking.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_nvCoolBits_clocking.Shortcut")));
      this.menu_nvCoolBits_clocking.ShowShortcut = ((bool)(resources.GetObject("menu_nvCoolBits_clocking.ShowShortcut")));
      this.menu_nvCoolBits_clocking.Text = resources.GetString("menu_nvCoolBits_clocking.Text");
      this.menu_nvCoolBits_clocking.Visible = ((bool)(resources.GetObject("menu_nvCoolBits_clocking.Visible")));
      this.menu_nvCoolBits_clocking.Click += new System.EventHandler(this.menu_nvCoolBits_clocking_Click);
      // 
      // menu_ati_D3DApply
      // 
      this.menu_ati_D3DApply.Enabled = ((bool)(resources.GetObject("menu_ati_D3DApply.Enabled")));
      this.menu_ati_D3DApply.Index = 6;
      this.menu_ati_D3DApply.OwnerDraw = true;
      this.menu_ati_D3DApply.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_ati_D3DApply.Shortcut")));
      this.menu_ati_D3DApply.ShowShortcut = ((bool)(resources.GetObject("menu_ati_D3DApply.ShowShortcut")));
      this.menu_ati_D3DApply.Text = resources.GetString("menu_ati_D3DApply.Text");
      this.menu_ati_D3DApply.Visible = ((bool)(resources.GetObject("menu_ati_D3DApply.Visible")));
      this.menu_ati_D3DApply.Click += new System.EventHandler(this.menu_ati_D3DApply_Click);
      // 
      // menu_ati_open_cpl
      // 
      this.menu_ati_open_cpl.Enabled = ((bool)(resources.GetObject("menu_ati_open_cpl.Enabled")));
      this.menu_ati_open_cpl.Index = 7;
      this.menu_ati_open_cpl.OwnerDraw = true;
      this.menu_ati_open_cpl.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_ati_open_cpl.Shortcut")));
      this.menu_ati_open_cpl.ShowShortcut = ((bool)(resources.GetObject("menu_ati_open_cpl.ShowShortcut")));
      this.menu_ati_open_cpl.Text = resources.GetString("menu_ati_open_cpl.Text");
      this.menu_ati_open_cpl.Visible = ((bool)(resources.GetObject("menu_ati_open_cpl.Visible")));
      this.menu_ati_open_cpl.Click += new System.EventHandler(this.menu_ati_open_cpl_Click);
      // 
      // menu_ati_open_oldCpl
      // 
      this.menu_ati_open_oldCpl.Enabled = ((bool)(resources.GetObject("menu_ati_open_oldCpl.Enabled")));
      this.menu_ati_open_oldCpl.Index = 8;
      this.menu_ati_open_oldCpl.OwnerDraw = true;
      this.menu_ati_open_oldCpl.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_ati_open_oldCpl.Shortcut")));
      this.menu_ati_open_oldCpl.ShowShortcut = ((bool)(resources.GetObject("menu_ati_open_oldCpl.ShowShortcut")));
      this.menu_ati_open_oldCpl.Text = resources.GetString("menu_ati_open_oldCpl.Text");
      this.menu_ati_open_oldCpl.Visible = ((bool)(resources.GetObject("menu_ati_open_oldCpl.Visible")));
      this.menu_ati_open_oldCpl.Click += new System.EventHandler(this.menu_ati_open_oldCpl_Click);
      // 
      // menu_winTweaks_Separator
      // 
      this.menu_winTweaks_Separator.Enabled = ((bool)(resources.GetObject("menu_winTweaks_Separator.Enabled")));
      this.menu_winTweaks_Separator.Index = 9;
      this.menu_winTweaks_Separator.OwnerDraw = true;
      this.menu_winTweaks_Separator.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_winTweaks_Separator.Shortcut")));
      this.menu_winTweaks_Separator.ShowShortcut = ((bool)(resources.GetObject("menu_winTweaks_Separator.ShowShortcut")));
      this.menu_winTweaks_Separator.Text = resources.GetString("menu_winTweaks_Separator.Text");
      this.menu_winTweaks_Separator.Visible = ((bool)(resources.GetObject("menu_winTweaks_Separator.Visible")));
      // 
      // menu_winTweaks
      // 
      this.menu_winTweaks.Enabled = ((bool)(resources.GetObject("menu_winTweaks.Enabled")));
      this.menu_winTweaks.Index = 10;
      this.menu_winTweaks.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                   this.menu_winTweak_disablePageExecutive,
                                                                                   this.menu_ati_lsc});
      this.menu_winTweaks.OwnerDraw = true;
      this.menu_winTweaks.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_winTweaks.Shortcut")));
      this.menu_winTweaks.ShowShortcut = ((bool)(resources.GetObject("menu_winTweaks.ShowShortcut")));
      this.menu_winTweaks.Text = resources.GetString("menu_winTweaks.Text");
      this.menu_winTweaks.Visible = ((bool)(resources.GetObject("menu_winTweaks.Visible")));
      this.menu_winTweaks.Popup += new System.EventHandler(this.menu_winTweaks_Popup);
      // 
      // menu_winTweak_disablePageExecutive
      // 
      this.menu_winTweak_disablePageExecutive.Enabled = ((bool)(resources.GetObject("menu_winTweak_disablePageExecutive.Enabled")));
      this.menu_winTweak_disablePageExecutive.Index = 0;
      this.menu_winTweak_disablePageExecutive.OwnerDraw = true;
      this.menu_winTweak_disablePageExecutive.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_winTweak_disablePageExecutive.Shortcut")));
      this.menu_winTweak_disablePageExecutive.ShowShortcut = ((bool)(resources.GetObject("menu_winTweak_disablePageExecutive.ShowShortcut")));
      this.menu_winTweak_disablePageExecutive.Text = resources.GetString("menu_winTweak_disablePageExecutive.Text");
      this.menu_winTweak_disablePageExecutive.Visible = ((bool)(resources.GetObject("menu_winTweak_disablePageExecutive.Visible")));
      this.menu_winTweak_disablePageExecutive.Click += new System.EventHandler(this.menu_winTweak_disablePageExecutive_Click);
      // 
      // menu_ati_lsc
      // 
      this.menu_ati_lsc.Enabled = ((bool)(resources.GetObject("menu_ati_lsc.Enabled")));
      this.menu_ati_lsc.Index = 1;
      this.menu_ati_lsc.OwnerDraw = true;
      this.menu_ati_lsc.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_ati_lsc.Shortcut")));
      this.menu_ati_lsc.ShowShortcut = ((bool)(resources.GetObject("menu_ati_lsc.ShowShortcut")));
      this.menu_ati_lsc.Text = resources.GetString("menu_ati_lsc.Text");
      this.menu_ati_lsc.Visible = ((bool)(resources.GetObject("menu_ati_lsc.Visible")));
      this.menu_ati_lsc.Click += new System.EventHandler(this.menu_ati_lsc_Click);
      // 
      // menuItem21
      // 
      this.menuItem21.Enabled = ((bool)(resources.GetObject("menuItem21.Enabled")));
      this.menuItem21.Index = 11;
      this.menuItem21.OwnerDraw = true;
      this.menuItem21.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem21.Shortcut")));
      this.menuItem21.ShowShortcut = ((bool)(resources.GetObject("menuItem21.ShowShortcut")));
      this.menuItem21.Text = resources.GetString("menuItem21.Text");
      this.menuItem21.Visible = ((bool)(resources.GetObject("menuItem21.Visible")));
      // 
      // menu_tools_undoApply
      // 
      this.menu_tools_undoApply.Enabled = ((bool)(resources.GetObject("menu_tools_undoApply.Enabled")));
      this.menu_tools_undoApply.Index = 12;
      this.menu_tools_undoApply.OwnerDraw = true;
      this.menu_tools_undoApply.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tools_undoApply.Shortcut")));
      this.menu_tools_undoApply.ShowShortcut = ((bool)(resources.GetObject("menu_tools_undoApply.ShowShortcut")));
      this.menu_tools_undoApply.Text = resources.GetString("menu_tools_undoApply.Text");
      this.menu_tools_undoApply.Visible = ((bool)(resources.GetObject("menu_tools_undoApply.Visible")));
      this.menu_tools_undoApply.Click += new System.EventHandler(this.menu_tools_undoApply_Click);
      // 
      // menu_profile
      // 
      this.menu_profile.Enabled = ((bool)(resources.GetObject("menu_profile.Enabled")));
      this.menu_profile.Index = 3;
      this.menu_profile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                 this.menuItem14,
                                                                                 this.menuItem3,
                                                                                 this.menuItem12,
                                                                                 this.menu_prof_prio,
                                                                                 this.menu_prof_freeMem,
                                                                                 this.menu_prof_autoRestore,
                                                                                 this.menuItem6,
                                                                                 this.menu_prof_detectAPI,
                                                                                 this.menu_prof_tdprofGD,
                                                                                 this.menu_prof_mouseWare,
                                                                                 this.menuItem9,
                                                                                 this.menu_prof_setSpecName,
                                                                                 this.menu_prof_importProfile,
                                                                                 this.menuItem24,
                                                                                 this.menuItem28});
      this.menu_profile.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profile.Shortcut")));
      this.menu_profile.ShowShortcut = ((bool)(resources.GetObject("menu_profile.ShowShortcut")));
      this.menu_profile.Text = resources.GetString("menu_profile.Text");
      this.menu_profile.Visible = ((bool)(resources.GetObject("menu_profile.Visible")));
      this.menu_profile.Popup += new System.EventHandler(this.menu_profile_Popup);
      // 
      // menuItem14
      // 
      this.menuItem14.Enabled = ((bool)(resources.GetObject("menuItem14.Enabled")));
      this.menuItem14.Index = 0;
      this.menuItem14.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                               this.menu_prof_exploreExePath,
                                                                               this.menu_profile_ini});
      this.menuItem14.OwnerDraw = true;
      this.menuItem14.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem14.Shortcut")));
      this.menuItem14.ShowShortcut = ((bool)(resources.GetObject("menuItem14.ShowShortcut")));
      this.menuItem14.Text = resources.GetString("menuItem14.Text");
      this.menuItem14.Visible = ((bool)(resources.GetObject("menuItem14.Visible")));
      // 
      // menu_prof_exploreExePath
      // 
      this.menu_prof_exploreExePath.Enabled = ((bool)(resources.GetObject("menu_prof_exploreExePath.Enabled")));
      this.menu_prof_exploreExePath.Index = 0;
      this.menu_prof_exploreExePath.OwnerDraw = true;
      this.menu_prof_exploreExePath.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_exploreExePath.Shortcut")));
      this.menu_prof_exploreExePath.ShowShortcut = ((bool)(resources.GetObject("menu_prof_exploreExePath.ShowShortcut")));
      this.menu_prof_exploreExePath.Text = resources.GetString("menu_prof_exploreExePath.Text");
      this.menu_prof_exploreExePath.Visible = ((bool)(resources.GetObject("menu_prof_exploreExePath.Visible")));
      this.menu_prof_exploreExePath.Click += new System.EventHandler(this.menu_prof_exploreExePath_Click);
      // 
      // menu_profile_ini
      // 
      this.menu_profile_ini.Enabled = ((bool)(resources.GetObject("menu_profile_ini.Enabled")));
      this.menu_profile_ini.Index = 1;
      this.menu_profile_ini.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                     this.menu_profile_ini_edit,
                                                                                     this.menu_profile_ini_find});
      this.menu_profile_ini.OwnerDraw = true;
      this.menu_profile_ini.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profile_ini.Shortcut")));
      this.menu_profile_ini.ShowShortcut = ((bool)(resources.GetObject("menu_profile_ini.ShowShortcut")));
      this.menu_profile_ini.Text = resources.GetString("menu_profile_ini.Text");
      this.menu_profile_ini.Visible = ((bool)(resources.GetObject("menu_profile_ini.Visible")));
      // 
      // menu_profile_ini_edit
      // 
      this.menu_profile_ini_edit.Enabled = ((bool)(resources.GetObject("menu_profile_ini_edit.Enabled")));
      this.menu_profile_ini_edit.Index = 0;
      this.menu_profile_ini_edit.OwnerDraw = true;
      this.menu_profile_ini_edit.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profile_ini_edit.Shortcut")));
      this.menu_profile_ini_edit.ShowShortcut = ((bool)(resources.GetObject("menu_profile_ini_edit.ShowShortcut")));
      this.menu_profile_ini_edit.Text = resources.GetString("menu_profile_ini_edit.Text");
      this.menu_profile_ini_edit.Visible = ((bool)(resources.GetObject("menu_profile_ini_edit.Visible")));
      this.menu_profile_ini_edit.Click += new System.EventHandler(this.menu_profile_ini_edit_Click);
      // 
      // menu_profile_ini_find
      // 
      this.menu_profile_ini_find.Enabled = ((bool)(resources.GetObject("menu_profile_ini_find.Enabled")));
      this.menu_profile_ini_find.Index = 1;
      this.menu_profile_ini_find.OwnerDraw = true;
      this.menu_profile_ini_find.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profile_ini_find.Shortcut")));
      this.menu_profile_ini_find.ShowShortcut = ((bool)(resources.GetObject("menu_profile_ini_find.ShowShortcut")));
      this.menu_profile_ini_find.Text = resources.GetString("menu_profile_ini_find.Text");
      this.menu_profile_ini_find.Visible = ((bool)(resources.GetObject("menu_profile_ini_find.Visible")));
      this.menu_profile_ini_find.Click += new System.EventHandler(this.menu_profile_ini_find_Click);
      // 
      // menuItem3
      // 
      this.menuItem3.Enabled = ((bool)(resources.GetObject("menuItem3.Enabled")));
      this.menuItem3.Index = 1;
      this.menuItem3.OwnerDraw = true;
      this.menuItem3.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem3.Shortcut")));
      this.menuItem3.ShowShortcut = ((bool)(resources.GetObject("menuItem3.ShowShortcut")));
      this.menuItem3.Text = resources.GetString("menuItem3.Text");
      this.menuItem3.Visible = ((bool)(resources.GetObject("menuItem3.Visible")));
      // 
      // menuItem12
      // 
      this.menuItem12.Enabled = ((bool)(resources.GetObject("menuItem12.Enabled")));
      this.menuItem12.Index = 2;
      this.menuItem12.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                               this.menu_prof_daemon_drive_nmb,
                                                                               this.menu_prof_imageFiles});
      this.menuItem12.OwnerDraw = true;
      this.menuItem12.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem12.Shortcut")));
      this.menuItem12.ShowShortcut = ((bool)(resources.GetObject("menuItem12.ShowShortcut")));
      this.menuItem12.Text = resources.GetString("menuItem12.Text");
      this.menuItem12.Visible = ((bool)(resources.GetObject("menuItem12.Visible")));
      // 
      // menu_prof_daemon_drive_nmb
      // 
      this.menu_prof_daemon_drive_nmb.Enabled = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb.Enabled")));
      this.menu_prof_daemon_drive_nmb.Index = 0;
      this.menu_prof_daemon_drive_nmb.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                               this.menu_prof_daemon_drive_nmb_0,
                                                                                               this.menu_prof_daemon_drive_nmb_1,
                                                                                               this.menu_prof_daemon_drive_nmb_2,
                                                                                               this.menu_prof_daemon_drive_nmb_3});
      this.menu_prof_daemon_drive_nmb.OwnerDraw = true;
      this.menu_prof_daemon_drive_nmb.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_daemon_drive_nmb.Shortcut")));
      this.menu_prof_daemon_drive_nmb.ShowShortcut = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb.ShowShortcut")));
      this.menu_prof_daemon_drive_nmb.Text = resources.GetString("menu_prof_daemon_drive_nmb.Text");
      this.menu_prof_daemon_drive_nmb.Visible = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb.Visible")));
      // 
      // menu_prof_daemon_drive_nmb_0
      // 
      this.menu_prof_daemon_drive_nmb_0.Checked = true;
      this.menu_prof_daemon_drive_nmb_0.Enabled = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_0.Enabled")));
      this.menu_prof_daemon_drive_nmb_0.Index = 0;
      this.menu_prof_daemon_drive_nmb_0.OwnerDraw = true;
      this.menu_prof_daemon_drive_nmb_0.RadioCheck = true;
      this.menu_prof_daemon_drive_nmb_0.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_daemon_drive_nmb_0.Shortcut")));
      this.menu_prof_daemon_drive_nmb_0.ShowShortcut = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_0.ShowShortcut")));
      this.menu_prof_daemon_drive_nmb_0.Text = resources.GetString("menu_prof_daemon_drive_nmb_0.Text");
      this.menu_prof_daemon_drive_nmb_0.Visible = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_0.Visible")));
      this.menu_prof_daemon_drive_nmb_0.Click += new System.EventHandler(this.menu_prof_daemon_drive_nmb_N_Click);
      // 
      // menu_prof_daemon_drive_nmb_1
      // 
      this.menu_prof_daemon_drive_nmb_1.Enabled = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_1.Enabled")));
      this.menu_prof_daemon_drive_nmb_1.Index = 1;
      this.menu_prof_daemon_drive_nmb_1.OwnerDraw = true;
      this.menu_prof_daemon_drive_nmb_1.RadioCheck = true;
      this.menu_prof_daemon_drive_nmb_1.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_daemon_drive_nmb_1.Shortcut")));
      this.menu_prof_daemon_drive_nmb_1.ShowShortcut = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_1.ShowShortcut")));
      this.menu_prof_daemon_drive_nmb_1.Text = resources.GetString("menu_prof_daemon_drive_nmb_1.Text");
      this.menu_prof_daemon_drive_nmb_1.Visible = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_1.Visible")));
      this.menu_prof_daemon_drive_nmb_1.Click += new System.EventHandler(this.menu_prof_daemon_drive_nmb_N_Click);
      // 
      // menu_prof_daemon_drive_nmb_2
      // 
      this.menu_prof_daemon_drive_nmb_2.Enabled = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_2.Enabled")));
      this.menu_prof_daemon_drive_nmb_2.Index = 2;
      this.menu_prof_daemon_drive_nmb_2.OwnerDraw = true;
      this.menu_prof_daemon_drive_nmb_2.RadioCheck = true;
      this.menu_prof_daemon_drive_nmb_2.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_daemon_drive_nmb_2.Shortcut")));
      this.menu_prof_daemon_drive_nmb_2.ShowShortcut = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_2.ShowShortcut")));
      this.menu_prof_daemon_drive_nmb_2.Text = resources.GetString("menu_prof_daemon_drive_nmb_2.Text");
      this.menu_prof_daemon_drive_nmb_2.Visible = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_2.Visible")));
      this.menu_prof_daemon_drive_nmb_2.Click += new System.EventHandler(this.menu_prof_daemon_drive_nmb_N_Click);
      // 
      // menu_prof_daemon_drive_nmb_3
      // 
      this.menu_prof_daemon_drive_nmb_3.Enabled = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_3.Enabled")));
      this.menu_prof_daemon_drive_nmb_3.Index = 3;
      this.menu_prof_daemon_drive_nmb_3.OwnerDraw = true;
      this.menu_prof_daemon_drive_nmb_3.RadioCheck = true;
      this.menu_prof_daemon_drive_nmb_3.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_daemon_drive_nmb_3.Shortcut")));
      this.menu_prof_daemon_drive_nmb_3.ShowShortcut = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_3.ShowShortcut")));
      this.menu_prof_daemon_drive_nmb_3.Text = resources.GetString("menu_prof_daemon_drive_nmb_3.Text");
      this.menu_prof_daemon_drive_nmb_3.Visible = ((bool)(resources.GetObject("menu_prof_daemon_drive_nmb_3.Visible")));
      this.menu_prof_daemon_drive_nmb_3.Click += new System.EventHandler(this.menu_prof_daemon_drive_nmb_N_Click);
      // 
      // menu_prof_imageFiles
      // 
      this.menu_prof_imageFiles.Enabled = ((bool)(resources.GetObject("menu_prof_imageFiles.Enabled")));
      this.menu_prof_imageFiles.Index = 1;
      this.menu_prof_imageFiles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                         this.menu_prof_img_file_replace,
                                                                                         this.menu_prof_img_file_replaceAll,
                                                                                         this.menu_prof_img_file_add,
                                                                                         this.menu_prof_img_file_remove,
                                                                                         this.menu_prof_img_file_removeAll});
      this.menu_prof_imageFiles.OwnerDraw = true;
      this.menu_prof_imageFiles.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_imageFiles.Shortcut")));
      this.menu_prof_imageFiles.ShowShortcut = ((bool)(resources.GetObject("menu_prof_imageFiles.ShowShortcut")));
      this.menu_prof_imageFiles.Text = resources.GetString("menu_prof_imageFiles.Text");
      this.menu_prof_imageFiles.Visible = ((bool)(resources.GetObject("menu_prof_imageFiles.Visible")));
      this.menu_prof_imageFiles.Popup += new System.EventHandler(this.menu_prof_imageFiles_Popup);
      // 
      // menu_prof_img_file_replace
      // 
      this.menu_prof_img_file_replace.Enabled = ((bool)(resources.GetObject("menu_prof_img_file_replace.Enabled")));
      this.menu_prof_img_file_replace.Index = 0;
      this.menu_prof_img_file_replace.OwnerDraw = true;
      this.menu_prof_img_file_replace.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_img_file_replace.Shortcut")));
      this.menu_prof_img_file_replace.ShowShortcut = ((bool)(resources.GetObject("menu_prof_img_file_replace.ShowShortcut")));
      this.menu_prof_img_file_replace.Text = resources.GetString("menu_prof_img_file_replace.Text");
      this.menu_prof_img_file_replace.Visible = ((bool)(resources.GetObject("menu_prof_img_file_replace.Visible")));
      this.menu_prof_img_file_replace.Click += new System.EventHandler(this.menu_prof_img_file_replace_Click);
      // 
      // menu_prof_img_file_replaceAll
      // 
      this.menu_prof_img_file_replaceAll.Enabled = ((bool)(resources.GetObject("menu_prof_img_file_replaceAll.Enabled")));
      this.menu_prof_img_file_replaceAll.Index = 1;
      this.menu_prof_img_file_replaceAll.OwnerDraw = true;
      this.menu_prof_img_file_replaceAll.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_img_file_replaceAll.Shortcut")));
      this.menu_prof_img_file_replaceAll.ShowShortcut = ((bool)(resources.GetObject("menu_prof_img_file_replaceAll.ShowShortcut")));
      this.menu_prof_img_file_replaceAll.Text = resources.GetString("menu_prof_img_file_replaceAll.Text");
      this.menu_prof_img_file_replaceAll.Visible = ((bool)(resources.GetObject("menu_prof_img_file_replaceAll.Visible")));
      this.menu_prof_img_file_replaceAll.Click += new System.EventHandler(this.menu_prof_img_file_replaceAll_Click);
      // 
      // menu_prof_img_file_add
      // 
      this.menu_prof_img_file_add.Enabled = ((bool)(resources.GetObject("menu_prof_img_file_add.Enabled")));
      this.menu_prof_img_file_add.Index = 2;
      this.menu_prof_img_file_add.OwnerDraw = true;
      this.menu_prof_img_file_add.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_img_file_add.Shortcut")));
      this.menu_prof_img_file_add.ShowShortcut = ((bool)(resources.GetObject("menu_prof_img_file_add.ShowShortcut")));
      this.menu_prof_img_file_add.Text = resources.GetString("menu_prof_img_file_add.Text");
      this.menu_prof_img_file_add.Visible = ((bool)(resources.GetObject("menu_prof_img_file_add.Visible")));
      this.menu_prof_img_file_add.Click += new System.EventHandler(this.menu_prof_img_file_add_Click);
      // 
      // menu_prof_img_file_remove
      // 
      this.menu_prof_img_file_remove.Enabled = ((bool)(resources.GetObject("menu_prof_img_file_remove.Enabled")));
      this.menu_prof_img_file_remove.Index = 3;
      this.menu_prof_img_file_remove.OwnerDraw = true;
      this.menu_prof_img_file_remove.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_img_file_remove.Shortcut")));
      this.menu_prof_img_file_remove.ShowShortcut = ((bool)(resources.GetObject("menu_prof_img_file_remove.ShowShortcut")));
      this.menu_prof_img_file_remove.Text = resources.GetString("menu_prof_img_file_remove.Text");
      this.menu_prof_img_file_remove.Visible = ((bool)(resources.GetObject("menu_prof_img_file_remove.Visible")));
      this.menu_prof_img_file_remove.Click += new System.EventHandler(this.menu_prof_img_file_remove_Click);
      // 
      // menu_prof_img_file_removeAll
      // 
      this.menu_prof_img_file_removeAll.Enabled = ((bool)(resources.GetObject("menu_prof_img_file_removeAll.Enabled")));
      this.menu_prof_img_file_removeAll.Index = 4;
      this.menu_prof_img_file_removeAll.OwnerDraw = true;
      this.menu_prof_img_file_removeAll.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_img_file_removeAll.Shortcut")));
      this.menu_prof_img_file_removeAll.ShowShortcut = ((bool)(resources.GetObject("menu_prof_img_file_removeAll.ShowShortcut")));
      this.menu_prof_img_file_removeAll.Text = resources.GetString("menu_prof_img_file_removeAll.Text");
      this.menu_prof_img_file_removeAll.Visible = ((bool)(resources.GetObject("menu_prof_img_file_removeAll.Visible")));
      this.menu_prof_img_file_removeAll.Click += new System.EventHandler(this.menu_prof_img_file_removeAll_Click);
      // 
      // menu_prof_prio
      // 
      this.menu_prof_prio.Enabled = ((bool)(resources.GetObject("menu_prof_prio.Enabled")));
      this.menu_prof_prio.Index = 3;
      this.menu_prof_prio.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                   this.menu_prof_prio_high,
                                                                                   this.menu_prof_prio_aboveNormal,
                                                                                   this.menu_prof_prio_normal,
                                                                                   this.menu_prof_prio_belowNormal,
                                                                                   this.menu_prof_prio_idle});
      this.menu_prof_prio.OwnerDraw = true;
      this.menu_prof_prio.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_prio.Shortcut")));
      this.menu_prof_prio.ShowShortcut = ((bool)(resources.GetObject("menu_prof_prio.ShowShortcut")));
      this.menu_prof_prio.Text = resources.GetString("menu_prof_prio.Text");
      this.menu_prof_prio.Visible = ((bool)(resources.GetObject("menu_prof_prio.Visible")));
      this.menu_prof_prio.Popup += new System.EventHandler(this.menu_prof_prio_Popup);
      // 
      // menu_prof_prio_high
      // 
      this.menu_prof_prio_high.Enabled = ((bool)(resources.GetObject("menu_prof_prio_high.Enabled")));
      this.menu_prof_prio_high.Index = 0;
      this.menu_prof_prio_high.OwnerDraw = true;
      this.menu_prof_prio_high.RadioCheck = true;
      this.menu_prof_prio_high.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_prio_high.Shortcut")));
      this.menu_prof_prio_high.ShowShortcut = ((bool)(resources.GetObject("menu_prof_prio_high.ShowShortcut")));
      this.menu_prof_prio_high.Text = resources.GetString("menu_prof_prio_high.Text");
      this.menu_prof_prio_high.Visible = ((bool)(resources.GetObject("menu_prof_prio_high.Visible")));
      this.menu_prof_prio_high.Click += new System.EventHandler(this.menu_prof_prio_any_Click);
      // 
      // menu_prof_prio_aboveNormal
      // 
      this.menu_prof_prio_aboveNormal.Enabled = ((bool)(resources.GetObject("menu_prof_prio_aboveNormal.Enabled")));
      this.menu_prof_prio_aboveNormal.Index = 1;
      this.menu_prof_prio_aboveNormal.OwnerDraw = true;
      this.menu_prof_prio_aboveNormal.RadioCheck = true;
      this.menu_prof_prio_aboveNormal.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_prio_aboveNormal.Shortcut")));
      this.menu_prof_prio_aboveNormal.ShowShortcut = ((bool)(resources.GetObject("menu_prof_prio_aboveNormal.ShowShortcut")));
      this.menu_prof_prio_aboveNormal.Text = resources.GetString("menu_prof_prio_aboveNormal.Text");
      this.menu_prof_prio_aboveNormal.Visible = ((bool)(resources.GetObject("menu_prof_prio_aboveNormal.Visible")));
      this.menu_prof_prio_aboveNormal.Click += new System.EventHandler(this.menu_prof_prio_any_Click);
      // 
      // menu_prof_prio_normal
      // 
      this.menu_prof_prio_normal.Checked = true;
      this.menu_prof_prio_normal.Enabled = ((bool)(resources.GetObject("menu_prof_prio_normal.Enabled")));
      this.menu_prof_prio_normal.Index = 2;
      this.menu_prof_prio_normal.OwnerDraw = true;
      this.menu_prof_prio_normal.RadioCheck = true;
      this.menu_prof_prio_normal.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_prio_normal.Shortcut")));
      this.menu_prof_prio_normal.ShowShortcut = ((bool)(resources.GetObject("menu_prof_prio_normal.ShowShortcut")));
      this.menu_prof_prio_normal.Text = resources.GetString("menu_prof_prio_normal.Text");
      this.menu_prof_prio_normal.Visible = ((bool)(resources.GetObject("menu_prof_prio_normal.Visible")));
      this.menu_prof_prio_normal.Click += new System.EventHandler(this.menu_prof_prio_any_Click);
      // 
      // menu_prof_prio_belowNormal
      // 
      this.menu_prof_prio_belowNormal.Enabled = ((bool)(resources.GetObject("menu_prof_prio_belowNormal.Enabled")));
      this.menu_prof_prio_belowNormal.Index = 3;
      this.menu_prof_prio_belowNormal.OwnerDraw = true;
      this.menu_prof_prio_belowNormal.RadioCheck = true;
      this.menu_prof_prio_belowNormal.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_prio_belowNormal.Shortcut")));
      this.menu_prof_prio_belowNormal.ShowShortcut = ((bool)(resources.GetObject("menu_prof_prio_belowNormal.ShowShortcut")));
      this.menu_prof_prio_belowNormal.Text = resources.GetString("menu_prof_prio_belowNormal.Text");
      this.menu_prof_prio_belowNormal.Visible = ((bool)(resources.GetObject("menu_prof_prio_belowNormal.Visible")));
      this.menu_prof_prio_belowNormal.Click += new System.EventHandler(this.menu_prof_prio_any_Click);
      // 
      // menu_prof_prio_idle
      // 
      this.menu_prof_prio_idle.Enabled = ((bool)(resources.GetObject("menu_prof_prio_idle.Enabled")));
      this.menu_prof_prio_idle.Index = 4;
      this.menu_prof_prio_idle.OwnerDraw = true;
      this.menu_prof_prio_idle.RadioCheck = true;
      this.menu_prof_prio_idle.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_prio_idle.Shortcut")));
      this.menu_prof_prio_idle.ShowShortcut = ((bool)(resources.GetObject("menu_prof_prio_idle.ShowShortcut")));
      this.menu_prof_prio_idle.Text = resources.GetString("menu_prof_prio_idle.Text");
      this.menu_prof_prio_idle.Visible = ((bool)(resources.GetObject("menu_prof_prio_idle.Visible")));
      this.menu_prof_prio_idle.Click += new System.EventHandler(this.menu_prof_prio_any_Click);
      // 
      // menu_prof_freeMem
      // 
      this.menu_prof_freeMem.Enabled = ((bool)(resources.GetObject("menu_prof_freeMem.Enabled")));
      this.menu_prof_freeMem.Index = 4;
      this.menu_prof_freeMem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.menu_prof_freeMem_none,
                                                                                      this.menu_prof_freeMem_64mb,
                                                                                      this.menu_prof_freeMem_128mb,
                                                                                      this.menu_prof_freeMem_256mb,
                                                                                      this.menu_prof_freeMem_384mb,
                                                                                      this.menu_prof_freeMem_512mb,
                                                                                      this.menu_prof_freeMem_max});
      this.menu_prof_freeMem.OwnerDraw = true;
      this.menu_prof_freeMem.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_freeMem.Shortcut")));
      this.menu_prof_freeMem.ShowShortcut = ((bool)(resources.GetObject("menu_prof_freeMem.ShowShortcut")));
      this.menu_prof_freeMem.Text = resources.GetString("menu_prof_freeMem.Text");
      this.menu_prof_freeMem.Visible = ((bool)(resources.GetObject("menu_prof_freeMem.Visible")));
      this.menu_prof_freeMem.Popup += new System.EventHandler(this.menu_prof_freeMem_Popup);
      // 
      // menu_prof_freeMem_none
      // 
      this.menu_prof_freeMem_none.Enabled = ((bool)(resources.GetObject("menu_prof_freeMem_none.Enabled")));
      this.menu_prof_freeMem_none.Index = 0;
      this.menu_prof_freeMem_none.OwnerDraw = true;
      this.menu_prof_freeMem_none.RadioCheck = true;
      this.menu_prof_freeMem_none.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_freeMem_none.Shortcut")));
      this.menu_prof_freeMem_none.ShowShortcut = ((bool)(resources.GetObject("menu_prof_freeMem_none.ShowShortcut")));
      this.menu_prof_freeMem_none.Text = resources.GetString("menu_prof_freeMem_none.Text");
      this.menu_prof_freeMem_none.Visible = ((bool)(resources.GetObject("menu_prof_freeMem_none.Visible")));
      this.menu_prof_freeMem_none.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
      // 
      // menu_prof_freeMem_64mb
      // 
      this.menu_prof_freeMem_64mb.Enabled = ((bool)(resources.GetObject("menu_prof_freeMem_64mb.Enabled")));
      this.menu_prof_freeMem_64mb.Index = 1;
      this.menu_prof_freeMem_64mb.OwnerDraw = true;
      this.menu_prof_freeMem_64mb.RadioCheck = true;
      this.menu_prof_freeMem_64mb.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_freeMem_64mb.Shortcut")));
      this.menu_prof_freeMem_64mb.ShowShortcut = ((bool)(resources.GetObject("menu_prof_freeMem_64mb.ShowShortcut")));
      this.menu_prof_freeMem_64mb.Text = resources.GetString("menu_prof_freeMem_64mb.Text");
      this.menu_prof_freeMem_64mb.Visible = ((bool)(resources.GetObject("menu_prof_freeMem_64mb.Visible")));
      this.menu_prof_freeMem_64mb.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
      // 
      // menu_prof_freeMem_128mb
      // 
      this.menu_prof_freeMem_128mb.Enabled = ((bool)(resources.GetObject("menu_prof_freeMem_128mb.Enabled")));
      this.menu_prof_freeMem_128mb.Index = 2;
      this.menu_prof_freeMem_128mb.OwnerDraw = true;
      this.menu_prof_freeMem_128mb.RadioCheck = true;
      this.menu_prof_freeMem_128mb.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_freeMem_128mb.Shortcut")));
      this.menu_prof_freeMem_128mb.ShowShortcut = ((bool)(resources.GetObject("menu_prof_freeMem_128mb.ShowShortcut")));
      this.menu_prof_freeMem_128mb.Text = resources.GetString("menu_prof_freeMem_128mb.Text");
      this.menu_prof_freeMem_128mb.Visible = ((bool)(resources.GetObject("menu_prof_freeMem_128mb.Visible")));
      this.menu_prof_freeMem_128mb.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
      // 
      // menu_prof_freeMem_256mb
      // 
      this.menu_prof_freeMem_256mb.Enabled = ((bool)(resources.GetObject("menu_prof_freeMem_256mb.Enabled")));
      this.menu_prof_freeMem_256mb.Index = 3;
      this.menu_prof_freeMem_256mb.OwnerDraw = true;
      this.menu_prof_freeMem_256mb.RadioCheck = true;
      this.menu_prof_freeMem_256mb.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_freeMem_256mb.Shortcut")));
      this.menu_prof_freeMem_256mb.ShowShortcut = ((bool)(resources.GetObject("menu_prof_freeMem_256mb.ShowShortcut")));
      this.menu_prof_freeMem_256mb.Text = resources.GetString("menu_prof_freeMem_256mb.Text");
      this.menu_prof_freeMem_256mb.Visible = ((bool)(resources.GetObject("menu_prof_freeMem_256mb.Visible")));
      this.menu_prof_freeMem_256mb.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
      // 
      // menu_prof_freeMem_384mb
      // 
      this.menu_prof_freeMem_384mb.Enabled = ((bool)(resources.GetObject("menu_prof_freeMem_384mb.Enabled")));
      this.menu_prof_freeMem_384mb.Index = 4;
      this.menu_prof_freeMem_384mb.OwnerDraw = true;
      this.menu_prof_freeMem_384mb.RadioCheck = true;
      this.menu_prof_freeMem_384mb.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_freeMem_384mb.Shortcut")));
      this.menu_prof_freeMem_384mb.ShowShortcut = ((bool)(resources.GetObject("menu_prof_freeMem_384mb.ShowShortcut")));
      this.menu_prof_freeMem_384mb.Text = resources.GetString("menu_prof_freeMem_384mb.Text");
      this.menu_prof_freeMem_384mb.Visible = ((bool)(resources.GetObject("menu_prof_freeMem_384mb.Visible")));
      this.menu_prof_freeMem_384mb.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
      // 
      // menu_prof_freeMem_512mb
      // 
      this.menu_prof_freeMem_512mb.Enabled = ((bool)(resources.GetObject("menu_prof_freeMem_512mb.Enabled")));
      this.menu_prof_freeMem_512mb.Index = 5;
      this.menu_prof_freeMem_512mb.OwnerDraw = true;
      this.menu_prof_freeMem_512mb.RadioCheck = true;
      this.menu_prof_freeMem_512mb.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_freeMem_512mb.Shortcut")));
      this.menu_prof_freeMem_512mb.ShowShortcut = ((bool)(resources.GetObject("menu_prof_freeMem_512mb.ShowShortcut")));
      this.menu_prof_freeMem_512mb.Text = resources.GetString("menu_prof_freeMem_512mb.Text");
      this.menu_prof_freeMem_512mb.Visible = ((bool)(resources.GetObject("menu_prof_freeMem_512mb.Visible")));
      this.menu_prof_freeMem_512mb.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
      // 
      // menu_prof_freeMem_max
      // 
      this.menu_prof_freeMem_max.Enabled = ((bool)(resources.GetObject("menu_prof_freeMem_max.Enabled")));
      this.menu_prof_freeMem_max.Index = 6;
      this.menu_prof_freeMem_max.OwnerDraw = true;
      this.menu_prof_freeMem_max.RadioCheck = true;
      this.menu_prof_freeMem_max.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_freeMem_max.Shortcut")));
      this.menu_prof_freeMem_max.ShowShortcut = ((bool)(resources.GetObject("menu_prof_freeMem_max.ShowShortcut")));
      this.menu_prof_freeMem_max.Text = resources.GetString("menu_prof_freeMem_max.Text");
      this.menu_prof_freeMem_max.Visible = ((bool)(resources.GetObject("menu_prof_freeMem_max.Visible")));
      this.menu_prof_freeMem_max.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
      // 
      // menu_prof_autoRestore
      // 
      this.menu_prof_autoRestore.Enabled = ((bool)(resources.GetObject("menu_prof_autoRestore.Enabled")));
      this.menu_prof_autoRestore.Index = 5;
      this.menu_prof_autoRestore.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                          this.menu_prof_autoRestore_default,
                                                                                          this.menu_prof_autoRestore_forceOff,
                                                                                          this.menu_prof_autoRestore_forceDialog,
                                                                                          this.menu_prof_autoRestore_disableDialog});
      this.menu_prof_autoRestore.OwnerDraw = true;
      this.menu_prof_autoRestore.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_autoRestore.Shortcut")));
      this.menu_prof_autoRestore.ShowShortcut = ((bool)(resources.GetObject("menu_prof_autoRestore.ShowShortcut")));
      this.menu_prof_autoRestore.Text = resources.GetString("menu_prof_autoRestore.Text");
      this.menu_prof_autoRestore.Visible = ((bool)(resources.GetObject("menu_prof_autoRestore.Visible")));
      this.menu_prof_autoRestore.Popup += new System.EventHandler(this.menu_prof_autoRestore_Popup);
      // 
      // menu_prof_autoRestore_default
      // 
      this.menu_prof_autoRestore_default.Enabled = ((bool)(resources.GetObject("menu_prof_autoRestore_default.Enabled")));
      this.menu_prof_autoRestore_default.Index = 0;
      this.menu_prof_autoRestore_default.OwnerDraw = true;
      this.menu_prof_autoRestore_default.RadioCheck = true;
      this.menu_prof_autoRestore_default.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_autoRestore_default.Shortcut")));
      this.menu_prof_autoRestore_default.ShowShortcut = ((bool)(resources.GetObject("menu_prof_autoRestore_default.ShowShortcut")));
      this.menu_prof_autoRestore_default.Text = resources.GetString("menu_prof_autoRestore_default.Text");
      this.menu_prof_autoRestore_default.Visible = ((bool)(resources.GetObject("menu_prof_autoRestore_default.Visible")));
      this.menu_prof_autoRestore_default.Click += new System.EventHandler(this.menu_prof_autoRestore_Any_Click);
      // 
      // menu_prof_autoRestore_forceOff
      // 
      this.menu_prof_autoRestore_forceOff.Enabled = ((bool)(resources.GetObject("menu_prof_autoRestore_forceOff.Enabled")));
      this.menu_prof_autoRestore_forceOff.Index = 1;
      this.menu_prof_autoRestore_forceOff.OwnerDraw = true;
      this.menu_prof_autoRestore_forceOff.RadioCheck = true;
      this.menu_prof_autoRestore_forceOff.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_autoRestore_forceOff.Shortcut")));
      this.menu_prof_autoRestore_forceOff.ShowShortcut = ((bool)(resources.GetObject("menu_prof_autoRestore_forceOff.ShowShortcut")));
      this.menu_prof_autoRestore_forceOff.Text = resources.GetString("menu_prof_autoRestore_forceOff.Text");
      this.menu_prof_autoRestore_forceOff.Visible = ((bool)(resources.GetObject("menu_prof_autoRestore_forceOff.Visible")));
      this.menu_prof_autoRestore_forceOff.Click += new System.EventHandler(this.menu_prof_autoRestore_Any_Click);
      // 
      // menu_prof_autoRestore_forceDialog
      // 
      this.menu_prof_autoRestore_forceDialog.Enabled = ((bool)(resources.GetObject("menu_prof_autoRestore_forceDialog.Enabled")));
      this.menu_prof_autoRestore_forceDialog.Index = 2;
      this.menu_prof_autoRestore_forceDialog.OwnerDraw = true;
      this.menu_prof_autoRestore_forceDialog.RadioCheck = true;
      this.menu_prof_autoRestore_forceDialog.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_autoRestore_forceDialog.Shortcut")));
      this.menu_prof_autoRestore_forceDialog.ShowShortcut = ((bool)(resources.GetObject("menu_prof_autoRestore_forceDialog.ShowShortcut")));
      this.menu_prof_autoRestore_forceDialog.Text = resources.GetString("menu_prof_autoRestore_forceDialog.Text");
      this.menu_prof_autoRestore_forceDialog.Visible = ((bool)(resources.GetObject("menu_prof_autoRestore_forceDialog.Visible")));
      this.menu_prof_autoRestore_forceDialog.Click += new System.EventHandler(this.menu_prof_autoRestore_Any_Click);
      // 
      // menu_prof_autoRestore_disableDialog
      // 
      this.menu_prof_autoRestore_disableDialog.Enabled = ((bool)(resources.GetObject("menu_prof_autoRestore_disableDialog.Enabled")));
      this.menu_prof_autoRestore_disableDialog.Index = 3;
      this.menu_prof_autoRestore_disableDialog.OwnerDraw = true;
      this.menu_prof_autoRestore_disableDialog.RadioCheck = true;
      this.menu_prof_autoRestore_disableDialog.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_autoRestore_disableDialog.Shortcut")));
      this.menu_prof_autoRestore_disableDialog.ShowShortcut = ((bool)(resources.GetObject("menu_prof_autoRestore_disableDialog.ShowShortcut")));
      this.menu_prof_autoRestore_disableDialog.Text = resources.GetString("menu_prof_autoRestore_disableDialog.Text");
      this.menu_prof_autoRestore_disableDialog.Visible = ((bool)(resources.GetObject("menu_prof_autoRestore_disableDialog.Visible")));
      this.menu_prof_autoRestore_disableDialog.Click += new System.EventHandler(this.menu_prof_autoRestore_Any_Click);
      // 
      // menuItem6
      // 
      this.menuItem6.Enabled = ((bool)(resources.GetObject("menuItem6.Enabled")));
      this.menuItem6.Index = 6;
      this.menuItem6.OwnerDraw = true;
      this.menuItem6.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem6.Shortcut")));
      this.menuItem6.ShowShortcut = ((bool)(resources.GetObject("menuItem6.ShowShortcut")));
      this.menuItem6.Text = resources.GetString("menuItem6.Text");
      this.menuItem6.Visible = ((bool)(resources.GetObject("menuItem6.Visible")));
      // 
      // menu_prof_detectAPI
      // 
      this.menu_prof_detectAPI.Enabled = ((bool)(resources.GetObject("menu_prof_detectAPI.Enabled")));
      this.menu_prof_detectAPI.Index = 7;
      this.menu_prof_detectAPI.OwnerDraw = true;
      this.menu_prof_detectAPI.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_detectAPI.Shortcut")));
      this.menu_prof_detectAPI.ShowShortcut = ((bool)(resources.GetObject("menu_prof_detectAPI.ShowShortcut")));
      this.menu_prof_detectAPI.Text = resources.GetString("menu_prof_detectAPI.Text");
      this.menu_prof_detectAPI.Visible = ((bool)(resources.GetObject("menu_prof_detectAPI.Visible")));
      this.menu_prof_detectAPI.Click += new System.EventHandler(this.menu_prof_detectAPI_Click);
      // 
      // menu_prof_tdprofGD
      // 
      this.menu_prof_tdprofGD.Enabled = ((bool)(resources.GetObject("menu_prof_tdprofGD.Enabled")));
      this.menu_prof_tdprofGD.Index = 8;
      this.menu_prof_tdprofGD.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                       this.menu_prof_tdprofGD_create,
                                                                                       this.menu_prof_tdprofGD_remove,
                                                                                       this.menu_prof_tdprofGD_help});
      this.menu_prof_tdprofGD.OwnerDraw = true;
      this.menu_prof_tdprofGD.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_tdprofGD.Shortcut")));
      this.menu_prof_tdprofGD.ShowShortcut = ((bool)(resources.GetObject("menu_prof_tdprofGD.ShowShortcut")));
      this.menu_prof_tdprofGD.Text = resources.GetString("menu_prof_tdprofGD.Text");
      this.menu_prof_tdprofGD.Visible = ((bool)(resources.GetObject("menu_prof_tdprofGD.Visible")));
      this.menu_prof_tdprofGD.Popup += new System.EventHandler(this.menu_prof_tdprofGD_Popup);
      // 
      // menu_prof_tdprofGD_create
      // 
      this.menu_prof_tdprofGD_create.Enabled = ((bool)(resources.GetObject("menu_prof_tdprofGD_create.Enabled")));
      this.menu_prof_tdprofGD_create.Index = 0;
      this.menu_prof_tdprofGD_create.OwnerDraw = true;
      this.menu_prof_tdprofGD_create.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_tdprofGD_create.Shortcut")));
      this.menu_prof_tdprofGD_create.ShowShortcut = ((bool)(resources.GetObject("menu_prof_tdprofGD_create.ShowShortcut")));
      this.menu_prof_tdprofGD_create.Text = resources.GetString("menu_prof_tdprofGD_create.Text");
      this.menu_prof_tdprofGD_create.Visible = ((bool)(resources.GetObject("menu_prof_tdprofGD_create.Visible")));
      this.menu_prof_tdprofGD_create.Click += new System.EventHandler(this.menu_prof_tdprofGD_create_Click);
      // 
      // menu_prof_tdprofGD_remove
      // 
      this.menu_prof_tdprofGD_remove.Enabled = ((bool)(resources.GetObject("menu_prof_tdprofGD_remove.Enabled")));
      this.menu_prof_tdprofGD_remove.Index = 1;
      this.menu_prof_tdprofGD_remove.OwnerDraw = true;
      this.menu_prof_tdprofGD_remove.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_tdprofGD_remove.Shortcut")));
      this.menu_prof_tdprofGD_remove.ShowShortcut = ((bool)(resources.GetObject("menu_prof_tdprofGD_remove.ShowShortcut")));
      this.menu_prof_tdprofGD_remove.Text = resources.GetString("menu_prof_tdprofGD_remove.Text");
      this.menu_prof_tdprofGD_remove.Visible = ((bool)(resources.GetObject("menu_prof_tdprofGD_remove.Visible")));
      this.menu_prof_tdprofGD_remove.Click += new System.EventHandler(this.menu_prof_tdprofGD_remove_Click);
      // 
      // menu_prof_tdprofGD_help
      // 
      this.menu_prof_tdprofGD_help.Enabled = ((bool)(resources.GetObject("menu_prof_tdprofGD_help.Enabled")));
      this.menu_prof_tdprofGD_help.Index = 2;
      this.menu_prof_tdprofGD_help.OwnerDraw = true;
      this.menu_prof_tdprofGD_help.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_tdprofGD_help.Shortcut")));
      this.menu_prof_tdprofGD_help.ShowShortcut = ((bool)(resources.GetObject("menu_prof_tdprofGD_help.ShowShortcut")));
      this.menu_prof_tdprofGD_help.Text = resources.GetString("menu_prof_tdprofGD_help.Text");
      this.menu_prof_tdprofGD_help.Visible = ((bool)(resources.GetObject("menu_prof_tdprofGD_help.Visible")));
      this.menu_prof_tdprofGD_help.Click += new System.EventHandler(this.menu_prof_tdprofGD_help_Click);
      // 
      // menu_prof_mouseWare
      // 
      this.menu_prof_mouseWare.Enabled = ((bool)(resources.GetObject("menu_prof_mouseWare.Enabled")));
      this.menu_prof_mouseWare.Index = 9;
      this.menu_prof_mouseWare.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                        this.menu_prof_mouseWare_noAccel});
      this.menu_prof_mouseWare.OwnerDraw = true;
      this.menu_prof_mouseWare.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_mouseWare.Shortcut")));
      this.menu_prof_mouseWare.ShowShortcut = ((bool)(resources.GetObject("menu_prof_mouseWare.ShowShortcut")));
      this.menu_prof_mouseWare.Text = resources.GetString("menu_prof_mouseWare.Text");
      this.menu_prof_mouseWare.Visible = ((bool)(resources.GetObject("menu_prof_mouseWare.Visible")));
      this.menu_prof_mouseWare.Popup += new System.EventHandler(this.menu_prof_mouseWare_Popup);
      // 
      // menu_prof_mouseWare_noAccel
      // 
      this.menu_prof_mouseWare_noAccel.Enabled = ((bool)(resources.GetObject("menu_prof_mouseWare_noAccel.Enabled")));
      this.menu_prof_mouseWare_noAccel.Index = 0;
      this.menu_prof_mouseWare_noAccel.OwnerDraw = true;
      this.menu_prof_mouseWare_noAccel.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_mouseWare_noAccel.Shortcut")));
      this.menu_prof_mouseWare_noAccel.ShowShortcut = ((bool)(resources.GetObject("menu_prof_mouseWare_noAccel.ShowShortcut")));
      this.menu_prof_mouseWare_noAccel.Text = resources.GetString("menu_prof_mouseWare_noAccel.Text");
      this.menu_prof_mouseWare_noAccel.Visible = ((bool)(resources.GetObject("menu_prof_mouseWare_noAccel.Visible")));
      this.menu_prof_mouseWare_noAccel.Click += new System.EventHandler(this.menu_prof_mouseWare_noAccel_Click);
      // 
      // menuItem9
      // 
      this.menuItem9.Enabled = ((bool)(resources.GetObject("menuItem9.Enabled")));
      this.menuItem9.Index = 10;
      this.menuItem9.OwnerDraw = true;
      this.menuItem9.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem9.Shortcut")));
      this.menuItem9.ShowShortcut = ((bool)(resources.GetObject("menuItem9.ShowShortcut")));
      this.menuItem9.Text = resources.GetString("menuItem9.Text");
      this.menuItem9.Visible = ((bool)(resources.GetObject("menuItem9.Visible")));
      // 
      // menu_prof_setSpecName
      // 
      this.menu_prof_setSpecName.Enabled = ((bool)(resources.GetObject("menu_prof_setSpecName.Enabled")));
      this.menu_prof_setSpecName.Index = 11;
      this.menu_prof_setSpecName.OwnerDraw = true;
      this.menu_prof_setSpecName.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_setSpecName.Shortcut")));
      this.menu_prof_setSpecName.ShowShortcut = ((bool)(resources.GetObject("menu_prof_setSpecName.ShowShortcut")));
      this.menu_prof_setSpecName.Text = resources.GetString("menu_prof_setSpecName.Text");
      this.menu_prof_setSpecName.Visible = ((bool)(resources.GetObject("menu_prof_setSpecName.Visible")));
      this.menu_prof_setSpecName.Click += new System.EventHandler(this.menu_prof_setSpecName_Click);
      // 
      // menu_prof_importProfile
      // 
      this.menu_prof_importProfile.Enabled = ((bool)(resources.GetObject("menu_prof_importProfile.Enabled")));
      this.menu_prof_importProfile.Index = 12;
      this.menu_prof_importProfile.OwnerDraw = true;
      this.menu_prof_importProfile.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_prof_importProfile.Shortcut")));
      this.menu_prof_importProfile.ShowShortcut = ((bool)(resources.GetObject("menu_prof_importProfile.ShowShortcut")));
      this.menu_prof_importProfile.Text = resources.GetString("menu_prof_importProfile.Text");
      this.menu_prof_importProfile.Visible = ((bool)(resources.GetObject("menu_prof_importProfile.Visible")));
      // 
      // menuItem24
      // 
      this.menuItem24.Enabled = ((bool)(resources.GetObject("menuItem24.Enabled")));
      this.menuItem24.Index = 13;
      this.menuItem24.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                               this.menu_profs_templates_turnTo2D,
                                                                               this.menu_profs_templates_clockOnly});
      this.menuItem24.OwnerDraw = true;
      this.menuItem24.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem24.Shortcut")));
      this.menuItem24.ShowShortcut = ((bool)(resources.GetObject("menuItem24.ShowShortcut")));
      this.menuItem24.Text = resources.GetString("menuItem24.Text");
      this.menuItem24.Visible = ((bool)(resources.GetObject("menuItem24.Visible")));
      // 
      // menu_profs_templates_turnTo2D
      // 
      this.menu_profs_templates_turnTo2D.Enabled = ((bool)(resources.GetObject("menu_profs_templates_turnTo2D.Enabled")));
      this.menu_profs_templates_turnTo2D.Index = 0;
      this.menu_profs_templates_turnTo2D.OwnerDraw = true;
      this.menu_profs_templates_turnTo2D.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profs_templates_turnTo2D.Shortcut")));
      this.menu_profs_templates_turnTo2D.ShowShortcut = ((bool)(resources.GetObject("menu_profs_templates_turnTo2D.ShowShortcut")));
      this.menu_profs_templates_turnTo2D.Text = resources.GetString("menu_profs_templates_turnTo2D.Text");
      this.menu_profs_templates_turnTo2D.Visible = ((bool)(resources.GetObject("menu_profs_templates_turnTo2D.Visible")));
      this.menu_profs_templates_turnTo2D.Click += new System.EventHandler(this.menu_profs_templates_turnTo2D_Click);
      // 
      // menu_profs_templates_clockOnly
      // 
      this.menu_profs_templates_clockOnly.Enabled = ((bool)(resources.GetObject("menu_profs_templates_clockOnly.Enabled")));
      this.menu_profs_templates_clockOnly.Index = 1;
      this.menu_profs_templates_clockOnly.OwnerDraw = true;
      this.menu_profs_templates_clockOnly.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profs_templates_clockOnly.Shortcut")));
      this.menu_profs_templates_clockOnly.ShowShortcut = ((bool)(resources.GetObject("menu_profs_templates_clockOnly.ShowShortcut")));
      this.menu_profs_templates_clockOnly.Text = resources.GetString("menu_profs_templates_clockOnly.Text");
      this.menu_profs_templates_clockOnly.Visible = ((bool)(resources.GetObject("menu_profs_templates_clockOnly.Visible")));
      this.menu_profs_templates_clockOnly.Click += new System.EventHandler(this.menu_profs_templates_clockOnly_Click);
      // 
      // menuItem28
      // 
      this.menuItem28.Enabled = ((bool)(resources.GetObject("menuItem28.Enabled")));
      this.menuItem28.Index = 14;
      this.menuItem28.OwnerDraw = true;
      this.menuItem28.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem28.Shortcut")));
      this.menuItem28.ShowShortcut = ((bool)(resources.GetObject("menuItem28.ShowShortcut")));
      this.menuItem28.Text = resources.GetString("menuItem28.Text");
      this.menuItem28.Visible = ((bool)(resources.GetObject("menuItem28.Visible")));
      // 
      // menu_profs
      // 
      this.menu_profs.Enabled = ((bool)(resources.GetObject("menu_profs.Enabled")));
      this.menu_profs.Index = 4;
      this.menu_profs.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                               this.menu_profs_filterBySpecName,
                                                                               this.menu_profs_cmds,
                                                                               this.menu_profs_hotkeys,
                                                                               this.menu_profs_exim});
      this.menu_profs.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profs.Shortcut")));
      this.menu_profs.ShowShortcut = ((bool)(resources.GetObject("menu_profs.ShowShortcut")));
      this.menu_profs.Text = resources.GetString("menu_profs.Text");
      this.menu_profs.Visible = ((bool)(resources.GetObject("menu_profs.Visible")));
      // 
      // menu_profs_filterBySpecName
      // 
      this.menu_profs_filterBySpecName.Enabled = ((bool)(resources.GetObject("menu_profs_filterBySpecName.Enabled")));
      this.menu_profs_filterBySpecName.Index = 0;
      this.menu_profs_filterBySpecName.OwnerDraw = true;
      this.menu_profs_filterBySpecName.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profs_filterBySpecName.Shortcut")));
      this.menu_profs_filterBySpecName.ShowShortcut = ((bool)(resources.GetObject("menu_profs_filterBySpecName.ShowShortcut")));
      this.menu_profs_filterBySpecName.Text = resources.GetString("menu_profs_filterBySpecName.Text");
      this.menu_profs_filterBySpecName.Visible = ((bool)(resources.GetObject("menu_profs_filterBySpecName.Visible")));
      this.menu_profs_filterBySpecName.Click += new System.EventHandler(this.menu_prof_filterBySpecName_Click);
      // 
      // menu_profs_cmds
      // 
      this.menu_profs_cmds.Enabled = ((bool)(resources.GetObject("menu_profs_cmds.Enabled")));
      this.menu_profs_cmds.Index = 1;
      this.menu_profs_cmds.OwnerDraw = true;
      this.menu_profs_cmds.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profs_cmds.Shortcut")));
      this.menu_profs_cmds.ShowShortcut = ((bool)(resources.GetObject("menu_profs_cmds.ShowShortcut")));
      this.menu_profs_cmds.Text = resources.GetString("menu_profs_cmds.Text");
      this.menu_profs_cmds.Visible = ((bool)(resources.GetObject("menu_profs_cmds.Visible")));
      this.menu_profs_cmds.Click += new System.EventHandler(this.menu_profs_cmds_Click);
      // 
      // menu_profs_hotkeys
      // 
      this.menu_profs_hotkeys.Enabled = ((bool)(resources.GetObject("menu_profs_hotkeys.Enabled")));
      this.menu_profs_hotkeys.Index = 2;
      this.menu_profs_hotkeys.OwnerDraw = true;
      this.menu_profs_hotkeys.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profs_hotkeys.Shortcut")));
      this.menu_profs_hotkeys.ShowShortcut = ((bool)(resources.GetObject("menu_profs_hotkeys.ShowShortcut")));
      this.menu_profs_hotkeys.Text = resources.GetString("menu_profs_hotkeys.Text");
      this.menu_profs_hotkeys.Visible = ((bool)(resources.GetObject("menu_profs_hotkeys.Visible")));
      this.menu_profs_hotkeys.Click += new System.EventHandler(this.menu_prof_hotkeys_Click);
      // 
      // menu_profs_exim
      // 
      this.menu_profs_exim.Enabled = ((bool)(resources.GetObject("menu_profs_exim.Enabled")));
      this.menu_profs_exim.Index = 3;
      this.menu_profs_exim.OwnerDraw = true;
      this.menu_profs_exim.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_profs_exim.Shortcut")));
      this.menu_profs_exim.ShowShortcut = ((bool)(resources.GetObject("menu_profs_exim.ShowShortcut")));
      this.menu_profs_exim.Text = resources.GetString("menu_profs_exim.Text");
      this.menu_profs_exim.Visible = ((bool)(resources.GetObject("menu_profs_exim.Visible")));
      this.menu_profs_exim.Click += new System.EventHandler(this.menu_profs_exim_Click);
      // 
      // menu_help
      // 
      this.menu_help.Enabled = ((bool)(resources.GetObject("menu_help.Enabled")));
      this.menu_help.Index = 5;
      this.menu_help.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                              this.menu_help_tooltips,
                                                                              this.menuItem4,
                                                                              this.menu_help_intro,
                                                                              this.menu_help_news,
                                                                              this.menu_help_todo,
                                                                              this.menu_help_manual,
                                                                              this.menuItem18,
                                                                              this.menu_help_visit_home,
                                                                              this.menu_help_visit_thread,
                                                                              this.menu_help_ati_visit_radeonFAQ,
                                                                              this.menu_help_mailto_author,
                                                                              this.menuItem10,
                                                                              this.menu_help_about,
                                                                              this.menu_help_showLicense,
                                                                              this.menu_help_nonWarranty,
                                                                              this.menuItem2,
                                                                              this.menu_help_report,
                                                                              this.menu_help_log,
                                                                              this.menu_help_sysInfo,
                                                                              this.menu_sfEdit});
      this.menu_help.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help.Shortcut")));
      this.menu_help.ShowShortcut = ((bool)(resources.GetObject("menu_help.ShowShortcut")));
      this.menu_help.Text = resources.GetString("menu_help.Text");
      this.menu_help.Visible = ((bool)(resources.GetObject("menu_help.Visible")));
      // 
      // menu_help_tooltips
      // 
      this.menu_help_tooltips.Checked = true;
      this.menu_help_tooltips.Enabled = ((bool)(resources.GetObject("menu_help_tooltips.Enabled")));
      this.menu_help_tooltips.Index = 0;
      this.menu_help_tooltips.OwnerDraw = true;
      this.menu_help_tooltips.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_tooltips.Shortcut")));
      this.menu_help_tooltips.ShowShortcut = ((bool)(resources.GetObject("menu_help_tooltips.ShowShortcut")));
      this.menu_help_tooltips.Text = resources.GetString("menu_help_tooltips.Text");
      this.menu_help_tooltips.Visible = ((bool)(resources.GetObject("menu_help_tooltips.Visible")));
      this.menu_help_tooltips.Click += new System.EventHandler(this.menu_help_tooltips_Click);
      // 
      // menuItem4
      // 
      this.menuItem4.Enabled = ((bool)(resources.GetObject("menuItem4.Enabled")));
      this.menuItem4.Index = 1;
      this.menuItem4.OwnerDraw = true;
      this.menuItem4.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem4.Shortcut")));
      this.menuItem4.ShowShortcut = ((bool)(resources.GetObject("menuItem4.ShowShortcut")));
      this.menuItem4.Text = resources.GetString("menuItem4.Text");
      this.menuItem4.Visible = ((bool)(resources.GetObject("menuItem4.Visible")));
      // 
      // menu_help_intro
      // 
      this.menu_help_intro.Enabled = ((bool)(resources.GetObject("menu_help_intro.Enabled")));
      this.menu_help_intro.Index = 2;
      this.menu_help_intro.OwnerDraw = true;
      this.menu_help_intro.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_intro.Shortcut")));
      this.menu_help_intro.ShowShortcut = ((bool)(resources.GetObject("menu_help_intro.ShowShortcut")));
      this.menu_help_intro.Text = resources.GetString("menu_help_intro.Text");
      this.menu_help_intro.Visible = ((bool)(resources.GetObject("menu_help_intro.Visible")));
      this.menu_help_intro.Click += new System.EventHandler(this.menu_help_intro_Click);
      // 
      // menu_help_news
      // 
      this.menu_help_news.Enabled = ((bool)(resources.GetObject("menu_help_news.Enabled")));
      this.menu_help_news.Index = 3;
      this.menu_help_news.OwnerDraw = true;
      this.menu_help_news.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_news.Shortcut")));
      this.menu_help_news.ShowShortcut = ((bool)(resources.GetObject("menu_help_news.ShowShortcut")));
      this.menu_help_news.Text = resources.GetString("menu_help_news.Text");
      this.menu_help_news.Visible = ((bool)(resources.GetObject("menu_help_news.Visible")));
      this.menu_help_news.Click += new System.EventHandler(this.menu_help_news_Click);
      // 
      // menu_help_todo
      // 
      this.menu_help_todo.Enabled = ((bool)(resources.GetObject("menu_help_todo.Enabled")));
      this.menu_help_todo.Index = 4;
      this.menu_help_todo.OwnerDraw = true;
      this.menu_help_todo.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_todo.Shortcut")));
      this.menu_help_todo.ShowShortcut = ((bool)(resources.GetObject("menu_help_todo.ShowShortcut")));
      this.menu_help_todo.Text = resources.GetString("menu_help_todo.Text");
      this.menu_help_todo.Visible = ((bool)(resources.GetObject("menu_help_todo.Visible")));
      this.menu_help_todo.Click += new System.EventHandler(this.menu_help_todo_Click);
      // 
      // menu_help_manual
      // 
      this.menu_help_manual.Enabled = ((bool)(resources.GetObject("menu_help_manual.Enabled")));
      this.menu_help_manual.Index = 5;
      this.menu_help_manual.OwnerDraw = true;
      this.menu_help_manual.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_manual.Shortcut")));
      this.menu_help_manual.ShowShortcut = ((bool)(resources.GetObject("menu_help_manual.ShowShortcut")));
      this.menu_help_manual.Text = resources.GetString("menu_help_manual.Text");
      this.menu_help_manual.Visible = ((bool)(resources.GetObject("menu_help_manual.Visible")));
      this.menu_help_manual.Click += new System.EventHandler(this.menu_help_manual_Click);
      // 
      // menuItem18
      // 
      this.menuItem18.Enabled = ((bool)(resources.GetObject("menuItem18.Enabled")));
      this.menuItem18.Index = 6;
      this.menuItem18.OwnerDraw = true;
      this.menuItem18.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem18.Shortcut")));
      this.menuItem18.ShowShortcut = ((bool)(resources.GetObject("menuItem18.ShowShortcut")));
      this.menuItem18.Text = resources.GetString("menuItem18.Text");
      this.menuItem18.Visible = ((bool)(resources.GetObject("menuItem18.Visible")));
      // 
      // menu_help_visit_home
      // 
      this.menu_help_visit_home.Enabled = ((bool)(resources.GetObject("menu_help_visit_home.Enabled")));
      this.menu_help_visit_home.Index = 7;
      this.menu_help_visit_home.OwnerDraw = true;
      this.menu_help_visit_home.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_visit_home.Shortcut")));
      this.menu_help_visit_home.ShowShortcut = ((bool)(resources.GetObject("menu_help_visit_home.ShowShortcut")));
      this.menu_help_visit_home.Text = resources.GetString("menu_help_visit_home.Text");
      this.menu_help_visit_home.Visible = ((bool)(resources.GetObject("menu_help_visit_home.Visible")));
      this.menu_help_visit_home.Click += new System.EventHandler(this.menu_help_visit_home_Click);
      // 
      // menu_help_visit_thread
      // 
      this.menu_help_visit_thread.Enabled = ((bool)(resources.GetObject("menu_help_visit_thread.Enabled")));
      this.menu_help_visit_thread.Index = 8;
      this.menu_help_visit_thread.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                           this.menu_help_visit_thread_3DC,
                                                                                           this.menu_help_visit_thread_R3D});
      this.menu_help_visit_thread.OwnerDraw = true;
      this.menu_help_visit_thread.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_visit_thread.Shortcut")));
      this.menu_help_visit_thread.ShowShortcut = ((bool)(resources.GetObject("menu_help_visit_thread.ShowShortcut")));
      this.menu_help_visit_thread.Text = resources.GetString("menu_help_visit_thread.Text");
      this.menu_help_visit_thread.Visible = ((bool)(resources.GetObject("menu_help_visit_thread.Visible")));
      // 
      // menu_help_visit_thread_3DC
      // 
      this.menu_help_visit_thread_3DC.Enabled = ((bool)(resources.GetObject("menu_help_visit_thread_3DC.Enabled")));
      this.menu_help_visit_thread_3DC.Index = 0;
      this.menu_help_visit_thread_3DC.OwnerDraw = true;
      this.menu_help_visit_thread_3DC.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_visit_thread_3DC.Shortcut")));
      this.menu_help_visit_thread_3DC.ShowShortcut = ((bool)(resources.GetObject("menu_help_visit_thread_3DC.ShowShortcut")));
      this.menu_help_visit_thread_3DC.Text = resources.GetString("menu_help_visit_thread_3DC.Text");
      this.menu_help_visit_thread_3DC.Visible = ((bool)(resources.GetObject("menu_help_visit_thread_3DC.Visible")));
      this.menu_help_visit_thread_3DC.Click += new System.EventHandler(this.menu_help_visit_thread_3DC_Click);
      // 
      // menu_help_visit_thread_R3D
      // 
      this.menu_help_visit_thread_R3D.Enabled = ((bool)(resources.GetObject("menu_help_visit_thread_R3D.Enabled")));
      this.menu_help_visit_thread_R3D.Index = 1;
      this.menu_help_visit_thread_R3D.OwnerDraw = true;
      this.menu_help_visit_thread_R3D.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_visit_thread_R3D.Shortcut")));
      this.menu_help_visit_thread_R3D.ShowShortcut = ((bool)(resources.GetObject("menu_help_visit_thread_R3D.ShowShortcut")));
      this.menu_help_visit_thread_R3D.Text = resources.GetString("menu_help_visit_thread_R3D.Text");
      this.menu_help_visit_thread_R3D.Visible = ((bool)(resources.GetObject("menu_help_visit_thread_R3D.Visible")));
      this.menu_help_visit_thread_R3D.Click += new System.EventHandler(this.menu_help_visit_thread_R3D_Click);
      // 
      // menu_help_ati_visit_radeonFAQ
      // 
      this.menu_help_ati_visit_radeonFAQ.Enabled = ((bool)(resources.GetObject("menu_help_ati_visit_radeonFAQ.Enabled")));
      this.menu_help_ati_visit_radeonFAQ.Index = 9;
      this.menu_help_ati_visit_radeonFAQ.OwnerDraw = true;
      this.menu_help_ati_visit_radeonFAQ.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_ati_visit_radeonFAQ.Shortcut")));
      this.menu_help_ati_visit_radeonFAQ.ShowShortcut = ((bool)(resources.GetObject("menu_help_ati_visit_radeonFAQ.ShowShortcut")));
      this.menu_help_ati_visit_radeonFAQ.Text = resources.GetString("menu_help_ati_visit_radeonFAQ.Text");
      this.menu_help_ati_visit_radeonFAQ.Visible = ((bool)(resources.GetObject("menu_help_ati_visit_radeonFAQ.Visible")));
      this.menu_help_ati_visit_radeonFAQ.Click += new System.EventHandler(this.menu_help_ati_visit_radeonFAQ_Click);
      // 
      // menu_help_mailto_author
      // 
      this.menu_help_mailto_author.Enabled = ((bool)(resources.GetObject("menu_help_mailto_author.Enabled")));
      this.menu_help_mailto_author.Index = 10;
      this.menu_help_mailto_author.OwnerDraw = true;
      this.menu_help_mailto_author.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_mailto_author.Shortcut")));
      this.menu_help_mailto_author.ShowShortcut = ((bool)(resources.GetObject("menu_help_mailto_author.ShowShortcut")));
      this.menu_help_mailto_author.Text = resources.GetString("menu_help_mailto_author.Text");
      this.menu_help_mailto_author.Visible = ((bool)(resources.GetObject("menu_help_mailto_author.Visible")));
      this.menu_help_mailto_author.Click += new System.EventHandler(this.menu_help_mailto_author_Click);
      // 
      // menuItem10
      // 
      this.menuItem10.Enabled = ((bool)(resources.GetObject("menuItem10.Enabled")));
      this.menuItem10.Index = 11;
      this.menuItem10.OwnerDraw = true;
      this.menuItem10.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem10.Shortcut")));
      this.menuItem10.ShowShortcut = ((bool)(resources.GetObject("menuItem10.ShowShortcut")));
      this.menuItem10.Text = resources.GetString("menuItem10.Text");
      this.menuItem10.Visible = ((bool)(resources.GetObject("menuItem10.Visible")));
      // 
      // menu_help_about
      // 
      this.menu_help_about.Enabled = ((bool)(resources.GetObject("menu_help_about.Enabled")));
      this.menu_help_about.Index = 12;
      this.menu_help_about.OwnerDraw = true;
      this.menu_help_about.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_about.Shortcut")));
      this.menu_help_about.ShowShortcut = ((bool)(resources.GetObject("menu_help_about.ShowShortcut")));
      this.menu_help_about.Text = resources.GetString("menu_help_about.Text");
      this.menu_help_about.Visible = ((bool)(resources.GetObject("menu_help_about.Visible")));
      this.menu_help_about.Click += new System.EventHandler(this.menu_help_about_Click);
      // 
      // menu_help_showLicense
      // 
      this.menu_help_showLicense.Enabled = ((bool)(resources.GetObject("menu_help_showLicense.Enabled")));
      this.menu_help_showLicense.Index = 13;
      this.menu_help_showLicense.OwnerDraw = true;
      this.menu_help_showLicense.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_showLicense.Shortcut")));
      this.menu_help_showLicense.ShowShortcut = ((bool)(resources.GetObject("menu_help_showLicense.ShowShortcut")));
      this.menu_help_showLicense.Text = resources.GetString("menu_help_showLicense.Text");
      this.menu_help_showLicense.Visible = ((bool)(resources.GetObject("menu_help_showLicense.Visible")));
      this.menu_help_showLicense.Click += new System.EventHandler(this.menu_help_showLicense_Click);
      // 
      // menu_help_nonWarranty
      // 
      this.menu_help_nonWarranty.Enabled = ((bool)(resources.GetObject("menu_help_nonWarranty.Enabled")));
      this.menu_help_nonWarranty.Index = 14;
      this.menu_help_nonWarranty.OwnerDraw = true;
      this.menu_help_nonWarranty.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_nonWarranty.Shortcut")));
      this.menu_help_nonWarranty.ShowShortcut = ((bool)(resources.GetObject("menu_help_nonWarranty.ShowShortcut")));
      this.menu_help_nonWarranty.Text = resources.GetString("menu_help_nonWarranty.Text");
      this.menu_help_nonWarranty.Visible = ((bool)(resources.GetObject("menu_help_nonWarranty.Visible")));
      this.menu_help_nonWarranty.Click += new System.EventHandler(this.menu_help_nonWarranty_Click);
      // 
      // menuItem2
      // 
      this.menuItem2.Enabled = ((bool)(resources.GetObject("menuItem2.Enabled")));
      this.menuItem2.Index = 15;
      this.menuItem2.OwnerDraw = true;
      this.menuItem2.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem2.Shortcut")));
      this.menuItem2.ShowShortcut = ((bool)(resources.GetObject("menuItem2.ShowShortcut")));
      this.menuItem2.Text = resources.GetString("menuItem2.Text");
      this.menuItem2.Visible = ((bool)(resources.GetObject("menuItem2.Visible")));
      // 
      // menu_help_report
      // 
      this.menu_help_report.Enabled = ((bool)(resources.GetObject("menu_help_report.Enabled")));
      this.menu_help_report.Index = 16;
      this.menu_help_report.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                     this.menu_help_report_nvclockDebug,
                                                                                     this.menu_help_report_r6clockDebug,
                                                                                     this.menu_help_report_displayInfo});
      this.menu_help_report.OwnerDraw = true;
      this.menu_help_report.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_report.Shortcut")));
      this.menu_help_report.ShowShortcut = ((bool)(resources.GetObject("menu_help_report.ShowShortcut")));
      this.menu_help_report.Text = resources.GetString("menu_help_report.Text");
      this.menu_help_report.Visible = ((bool)(resources.GetObject("menu_help_report.Visible")));
      // 
      // menu_help_report_nvclockDebug
      // 
      this.menu_help_report_nvclockDebug.Enabled = ((bool)(resources.GetObject("menu_help_report_nvclockDebug.Enabled")));
      this.menu_help_report_nvclockDebug.Index = 0;
      this.menu_help_report_nvclockDebug.OwnerDraw = true;
      this.menu_help_report_nvclockDebug.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_report_nvclockDebug.Shortcut")));
      this.menu_help_report_nvclockDebug.ShowShortcut = ((bool)(resources.GetObject("menu_help_report_nvclockDebug.ShowShortcut")));
      this.menu_help_report_nvclockDebug.Text = resources.GetString("menu_help_report_nvclockDebug.Text");
      this.menu_help_report_nvclockDebug.Visible = ((bool)(resources.GetObject("menu_help_report_nvclockDebug.Visible")));
      this.menu_help_report_nvclockDebug.Click += new System.EventHandler(this.menu_help_report_nvclockDebug_Click);
      // 
      // menu_help_report_r6clockDebug
      // 
      this.menu_help_report_r6clockDebug.Enabled = ((bool)(resources.GetObject("menu_help_report_r6clockDebug.Enabled")));
      this.menu_help_report_r6clockDebug.Index = 1;
      this.menu_help_report_r6clockDebug.OwnerDraw = true;
      this.menu_help_report_r6clockDebug.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_report_r6clockDebug.Shortcut")));
      this.menu_help_report_r6clockDebug.ShowShortcut = ((bool)(resources.GetObject("menu_help_report_r6clockDebug.ShowShortcut")));
      this.menu_help_report_r6clockDebug.Text = resources.GetString("menu_help_report_r6clockDebug.Text");
      this.menu_help_report_r6clockDebug.Visible = ((bool)(resources.GetObject("menu_help_report_r6clockDebug.Visible")));
      this.menu_help_report_r6clockDebug.Click += new System.EventHandler(this.menu_help_report_r6clockDebug_Click);
      // 
      // menu_help_report_displayInfo
      // 
      this.menu_help_report_displayInfo.Enabled = ((bool)(resources.GetObject("menu_help_report_displayInfo.Enabled")));
      this.menu_help_report_displayInfo.Index = 2;
      this.menu_help_report_displayInfo.OwnerDraw = true;
      this.menu_help_report_displayInfo.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_report_displayInfo.Shortcut")));
      this.menu_help_report_displayInfo.ShowShortcut = ((bool)(resources.GetObject("menu_help_report_displayInfo.ShowShortcut")));
      this.menu_help_report_displayInfo.Text = resources.GetString("menu_help_report_displayInfo.Text");
      this.menu_help_report_displayInfo.Visible = ((bool)(resources.GetObject("menu_help_report_displayInfo.Visible")));
      this.menu_help_report_displayInfo.Click += new System.EventHandler(this.menu_help_report_displayInfo_Click);
      // 
      // menu_help_log
      // 
      this.menu_help_log.Enabled = ((bool)(resources.GetObject("menu_help_log.Enabled")));
      this.menu_help_log.Index = 17;
      this.menu_help_log.OwnerDraw = true;
      this.menu_help_log.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_log.Shortcut")));
      this.menu_help_log.ShowShortcut = ((bool)(resources.GetObject("menu_help_log.ShowShortcut")));
      this.menu_help_log.Text = resources.GetString("menu_help_log.Text");
      this.menu_help_log.Visible = ((bool)(resources.GetObject("menu_help_log.Visible")));
      this.menu_help_log.Click += new System.EventHandler(this.menu_help_log_Click);
      // 
      // menu_help_sysInfo
      // 
      this.menu_help_sysInfo.Enabled = ((bool)(resources.GetObject("menu_help_sysInfo.Enabled")));
      this.menu_help_sysInfo.Index = 18;
      this.menu_help_sysInfo.OwnerDraw = true;
      this.menu_help_sysInfo.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_sysInfo.Shortcut")));
      this.menu_help_sysInfo.ShowShortcut = ((bool)(resources.GetObject("menu_help_sysInfo.ShowShortcut")));
      this.menu_help_sysInfo.Text = resources.GetString("menu_help_sysInfo.Text");
      this.menu_help_sysInfo.Visible = ((bool)(resources.GetObject("menu_help_sysInfo.Visible")));
      this.menu_help_sysInfo.Click += new System.EventHandler(this.menu_help_sysInfo_Click);
      // 
      // menu_sfEdit
      // 
      this.menu_sfEdit.Enabled = ((bool)(resources.GetObject("menu_sfEdit.Enabled")));
      this.menu_sfEdit.Index = 19;
      this.menu_sfEdit.OwnerDraw = true;
      this.menu_sfEdit.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_sfEdit.Shortcut")));
      this.menu_sfEdit.ShowShortcut = ((bool)(resources.GetObject("menu_sfEdit.ShowShortcut")));
      this.menu_sfEdit.Text = resources.GetString("menu_sfEdit.Text");
      this.menu_sfEdit.Visible = ((bool)(resources.GetObject("menu_sfEdit.Visible")));
      this.menu_sfEdit.Click += new System.EventHandler(this.menu_sfEdit_Click);
      // 
      // menu_exp
      // 
      this.menu_exp.Enabled = ((bool)(resources.GetObject("menu_exp.Enabled")));
      this.menu_exp.Index = 6;
      this.menu_exp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                             this.menuI_exp_findProc,
                                                                             this.menu_help_helpButton,
                                                                             this.menu_file_debugThrow,
                                                                             this.menu_file_debugCrashMe,
                                                                             this.menu_file_restore3d,
                                                                             this.menu_exp_test1,
                                                                             this.menuItem13});
      this.menu_exp.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_exp.Shortcut")));
      this.menu_exp.ShowShortcut = ((bool)(resources.GetObject("menu_exp.ShowShortcut")));
      this.menu_exp.Text = resources.GetString("menu_exp.Text");
      this.menu_exp.Visible = ((bool)(resources.GetObject("menu_exp.Visible")));
      // 
      // menuI_exp_findProc
      // 
      this.menuI_exp_findProc.Enabled = ((bool)(resources.GetObject("menuI_exp_findProc.Enabled")));
      this.menuI_exp_findProc.Index = 0;
      this.menuI_exp_findProc.OwnerDraw = true;
      this.menuI_exp_findProc.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuI_exp_findProc.Shortcut")));
      this.menuI_exp_findProc.ShowShortcut = ((bool)(resources.GetObject("menuI_exp_findProc.ShowShortcut")));
      this.menuI_exp_findProc.Text = resources.GetString("menuI_exp_findProc.Text");
      this.menuI_exp_findProc.Visible = ((bool)(resources.GetObject("menuI_exp_findProc.Visible")));
      this.menuI_exp_findProc.Click += new System.EventHandler(this.menuI_exp_findProc_Click);
      // 
      // menu_help_helpButton
      // 
      this.menu_help_helpButton.Enabled = ((bool)(resources.GetObject("menu_help_helpButton.Enabled")));
      this.menu_help_helpButton.Index = 1;
      this.menu_help_helpButton.OwnerDraw = true;
      this.menu_help_helpButton.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_help_helpButton.Shortcut")));
      this.menu_help_helpButton.ShowShortcut = ((bool)(resources.GetObject("menu_help_helpButton.ShowShortcut")));
      this.menu_help_helpButton.Text = resources.GetString("menu_help_helpButton.Text");
      this.menu_help_helpButton.Visible = ((bool)(resources.GetObject("menu_help_helpButton.Visible")));
      this.menu_help_helpButton.Click += new System.EventHandler(this.menu_help_helpButton_Click);
      // 
      // menu_file_debugThrow
      // 
      this.menu_file_debugThrow.Enabled = ((bool)(resources.GetObject("menu_file_debugThrow.Enabled")));
      this.menu_file_debugThrow.Index = 2;
      this.menu_file_debugThrow.OwnerDraw = true;
      this.menu_file_debugThrow.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_debugThrow.Shortcut")));
      this.menu_file_debugThrow.ShowShortcut = ((bool)(resources.GetObject("menu_file_debugThrow.ShowShortcut")));
      this.menu_file_debugThrow.Text = resources.GetString("menu_file_debugThrow.Text");
      this.menu_file_debugThrow.Visible = ((bool)(resources.GetObject("menu_file_debugThrow.Visible")));
      this.menu_file_debugThrow.Click += new System.EventHandler(this.menu_file_debugThrow_Click);
      // 
      // menu_file_debugCrashMe
      // 
      this.menu_file_debugCrashMe.Enabled = ((bool)(resources.GetObject("menu_file_debugCrashMe.Enabled")));
      this.menu_file_debugCrashMe.Index = 3;
      this.menu_file_debugCrashMe.OwnerDraw = true;
      this.menu_file_debugCrashMe.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_debugCrashMe.Shortcut")));
      this.menu_file_debugCrashMe.ShowShortcut = ((bool)(resources.GetObject("menu_file_debugCrashMe.ShowShortcut")));
      this.menu_file_debugCrashMe.Text = resources.GetString("menu_file_debugCrashMe.Text");
      this.menu_file_debugCrashMe.Visible = ((bool)(resources.GetObject("menu_file_debugCrashMe.Visible")));
      this.menu_file_debugCrashMe.Click += new System.EventHandler(this.menu_file_debugCrashMe_Click);
      // 
      // menu_file_restore3d
      // 
      this.menu_file_restore3d.Enabled = ((bool)(resources.GetObject("menu_file_restore3d.Enabled")));
      this.menu_file_restore3d.Index = 4;
      this.menu_file_restore3d.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                        this.menu_file_restore3d_saveToFile,
                                                                                        this.menu_file_restore3d_loadLastSaved,
                                                                                        this.menu_file_restore3d_loadSavedByRun,
                                                                                        this.menu_file_restore3d_loadAutoSaved});
      this.menu_file_restore3d.OwnerDraw = true;
      this.menu_file_restore3d.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_restore3d.Shortcut")));
      this.menu_file_restore3d.ShowShortcut = ((bool)(resources.GetObject("menu_file_restore3d.ShowShortcut")));
      this.menu_file_restore3d.Text = resources.GetString("menu_file_restore3d.Text");
      this.menu_file_restore3d.Visible = ((bool)(resources.GetObject("menu_file_restore3d.Visible")));
      // 
      // menu_file_restore3d_saveToFile
      // 
      this.menu_file_restore3d_saveToFile.Enabled = ((bool)(resources.GetObject("menu_file_restore3d_saveToFile.Enabled")));
      this.menu_file_restore3d_saveToFile.Index = 0;
      this.menu_file_restore3d_saveToFile.OwnerDraw = true;
      this.menu_file_restore3d_saveToFile.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_restore3d_saveToFile.Shortcut")));
      this.menu_file_restore3d_saveToFile.ShowShortcut = ((bool)(resources.GetObject("menu_file_restore3d_saveToFile.ShowShortcut")));
      this.menu_file_restore3d_saveToFile.Text = resources.GetString("menu_file_restore3d_saveToFile.Text");
      this.menu_file_restore3d_saveToFile.Visible = ((bool)(resources.GetObject("menu_file_restore3d_saveToFile.Visible")));
      this.menu_file_restore3d_saveToFile.Click += new System.EventHandler(this.menu_file_restore3d_saveToFile_Click);
      // 
      // menu_file_restore3d_loadLastSaved
      // 
      this.menu_file_restore3d_loadLastSaved.Enabled = ((bool)(resources.GetObject("menu_file_restore3d_loadLastSaved.Enabled")));
      this.menu_file_restore3d_loadLastSaved.Index = 1;
      this.menu_file_restore3d_loadLastSaved.OwnerDraw = true;
      this.menu_file_restore3d_loadLastSaved.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_restore3d_loadLastSaved.Shortcut")));
      this.menu_file_restore3d_loadLastSaved.ShowShortcut = ((bool)(resources.GetObject("menu_file_restore3d_loadLastSaved.ShowShortcut")));
      this.menu_file_restore3d_loadLastSaved.Text = resources.GetString("menu_file_restore3d_loadLastSaved.Text");
      this.menu_file_restore3d_loadLastSaved.Visible = ((bool)(resources.GetObject("menu_file_restore3d_loadLastSaved.Visible")));
      this.menu_file_restore3d_loadLastSaved.Click += new System.EventHandler(this.menu_file_restore3d_loadLastSaved_Click);
      // 
      // menu_file_restore3d_loadSavedByRun
      // 
      this.menu_file_restore3d_loadSavedByRun.Enabled = ((bool)(resources.GetObject("menu_file_restore3d_loadSavedByRun.Enabled")));
      this.menu_file_restore3d_loadSavedByRun.Index = 2;
      this.menu_file_restore3d_loadSavedByRun.OwnerDraw = true;
      this.menu_file_restore3d_loadSavedByRun.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_restore3d_loadSavedByRun.Shortcut")));
      this.menu_file_restore3d_loadSavedByRun.ShowShortcut = ((bool)(resources.GetObject("menu_file_restore3d_loadSavedByRun.ShowShortcut")));
      this.menu_file_restore3d_loadSavedByRun.Text = resources.GetString("menu_file_restore3d_loadSavedByRun.Text");
      this.menu_file_restore3d_loadSavedByRun.Visible = ((bool)(resources.GetObject("menu_file_restore3d_loadSavedByRun.Visible")));
      this.menu_file_restore3d_loadSavedByRun.Click += new System.EventHandler(this.menu_file_restore3d_loadSavedByRun_Click);
      // 
      // menu_file_restore3d_loadAutoSaved
      // 
      this.menu_file_restore3d_loadAutoSaved.Enabled = ((bool)(resources.GetObject("menu_file_restore3d_loadAutoSaved.Enabled")));
      this.menu_file_restore3d_loadAutoSaved.Index = 3;
      this.menu_file_restore3d_loadAutoSaved.OwnerDraw = true;
      this.menu_file_restore3d_loadAutoSaved.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_file_restore3d_loadAutoSaved.Shortcut")));
      this.menu_file_restore3d_loadAutoSaved.ShowShortcut = ((bool)(resources.GetObject("menu_file_restore3d_loadAutoSaved.ShowShortcut")));
      this.menu_file_restore3d_loadAutoSaved.Text = resources.GetString("menu_file_restore3d_loadAutoSaved.Text");
      this.menu_file_restore3d_loadAutoSaved.Visible = ((bool)(resources.GetObject("menu_file_restore3d_loadAutoSaved.Visible")));
      this.menu_file_restore3d_loadAutoSaved.Click += new System.EventHandler(this.menu_file_restore3d_loadAutoSaved_Click);
      // 
      // menu_exp_test1
      // 
      this.menu_exp_test1.Enabled = ((bool)(resources.GetObject("menu_exp_test1.Enabled")));
      this.menu_exp_test1.Index = 5;
      this.menu_exp_test1.OwnerDraw = true;
      this.menu_exp_test1.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_exp_test1.Shortcut")));
      this.menu_exp_test1.ShowShortcut = ((bool)(resources.GetObject("menu_exp_test1.ShowShortcut")));
      this.menu_exp_test1.Text = resources.GetString("menu_exp_test1.Text");
      this.menu_exp_test1.Visible = ((bool)(resources.GetObject("menu_exp_test1.Visible")));
      this.menu_exp_test1.Click += new System.EventHandler(this.menu_exp_test1_Click);
      // 
      // menuItem13
      // 
      this.menuItem13.Enabled = ((bool)(resources.GetObject("menuItem13.Enabled")));
      this.menuItem13.Index = 6;
      this.menuItem13.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                               this.menuItem22});
      this.menuItem13.OwnerDraw = true;
      this.menuItem13.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem13.Shortcut")));
      this.menuItem13.ShowShortcut = ((bool)(resources.GetObject("menuItem13.ShowShortcut")));
      this.menuItem13.Text = resources.GetString("menuItem13.Text");
      this.menuItem13.Visible = ((bool)(resources.GetObject("menuItem13.Visible")));
      // 
      // menuItem22
      // 
      this.menuItem22.Enabled = ((bool)(resources.GetObject("menuItem22.Enabled")));
      this.menuItem22.Index = 0;
      this.menuItem22.OwnerDraw = true;
      this.menuItem22.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem22.Shortcut")));
      this.menuItem22.ShowShortcut = ((bool)(resources.GetObject("menuItem22.ShowShortcut")));
      this.menuItem22.Text = resources.GetString("menuItem22.Text");
      this.menuItem22.Visible = ((bool)(resources.GetObject("menuItem22.Visible")));
      this.menuItem22.Click += new System.EventHandler(this.menuItem22_Click);
      // 
      // tabCtrl
      // 
      this.tabCtrl.AccessibleDescription = resources.GetString("tabCtrl.AccessibleDescription");
      this.tabCtrl.AccessibleName = resources.GetString("tabCtrl.AccessibleName");
      this.tabCtrl.Alignment = ((System.Windows.Forms.TabAlignment)(resources.GetObject("tabCtrl.Alignment")));
      this.tabCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabCtrl.Anchor")));
      this.tabCtrl.Appearance = ((System.Windows.Forms.TabAppearance)(resources.GetObject("tabCtrl.Appearance")));
      this.tabCtrl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabCtrl.BackgroundImage")));
      this.tabCtrl.Controls.Add(this.tab_main);
      this.tabCtrl.Controls.Add(this.tab_files);
      this.tabCtrl.Controls.Add(this.tab_extra_d3d);
      this.tabCtrl.Controls.Add(this.tab_extra_ogl);
      this.tabCtrl.Controls.Add(this.tab_summary);
      this.tabCtrl.Controls.Add(this.tab_clocking);
      this.tabCtrl.Controls.Add(this.tab_exp);
      this.tabCtrl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabCtrl.Dock")));
      this.tabCtrl.Enabled = ((bool)(resources.GetObject("tabCtrl.Enabled")));
      this.tabCtrl.Font = ((System.Drawing.Font)(resources.GetObject("tabCtrl.Font")));
      this.tabCtrl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabCtrl.ImeMode")));
      this.tabCtrl.ItemSize = ((System.Drawing.Size)(resources.GetObject("tabCtrl.ItemSize")));
      this.tabCtrl.Location = ((System.Drawing.Point)(resources.GetObject("tabCtrl.Location")));
      this.tabCtrl.Name = "tabCtrl";
      this.tabCtrl.Padding = ((System.Drawing.Point)(resources.GetObject("tabCtrl.Padding")));
      this.tabCtrl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabCtrl.RightToLeft")));
      this.tabCtrl.SelectedIndex = 0;
      this.tabCtrl.ShowToolTips = ((bool)(resources.GetObject("tabCtrl.ShowToolTips")));
      this.tabCtrl.Size = ((System.Drawing.Size)(resources.GetObject("tabCtrl.Size")));
      this.tabCtrl.TabIndex = ((int)(resources.GetObject("tabCtrl.TabIndex")));
      this.tabCtrl.Text = resources.GetString("tabCtrl.Text");
      this.toolTip.SetToolTip(this.tabCtrl, resources.GetString("tabCtrl.ToolTip"));
      this.tabCtrl.Visible = ((bool)(resources.GetObject("tabCtrl.Visible")));
      this.tabCtrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabCtrl_KeyDown);
      // 
      // tab_main
      // 
      this.tab_main.AccessibleDescription = resources.GetString("tab_main.AccessibleDescription");
      this.tab_main.AccessibleName = resources.GetString("tab_main.AccessibleName");
      this.tab_main.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab_main.Anchor")));
      this.tab_main.AutoScroll = ((bool)(resources.GetObject("tab_main.AutoScroll")));
      this.tab_main.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tab_main.AutoScrollMargin")));
      this.tab_main.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tab_main.AutoScrollMinSize")));
      this.tab_main.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab_main.BackgroundImage")));
      this.tab_main.Controls.Add(this.group_main_d3d);
      this.tab_main.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab_main.Dock")));
      this.tab_main.Enabled = ((bool)(resources.GetObject("tab_main.Enabled")));
      this.tab_main.Font = ((System.Drawing.Font)(resources.GetObject("tab_main.Font")));
      this.tab_main.ImageIndex = ((int)(resources.GetObject("tab_main.ImageIndex")));
      this.tab_main.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab_main.ImeMode")));
      this.tab_main.Location = ((System.Drawing.Point)(resources.GetObject("tab_main.Location")));
      this.tab_main.Name = "tab_main";
      this.tab_main.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab_main.RightToLeft")));
      this.tab_main.Size = ((System.Drawing.Size)(resources.GetObject("tab_main.Size")));
      this.tab_main.TabIndex = ((int)(resources.GetObject("tab_main.TabIndex")));
      this.tab_main.Text = resources.GetString("tab_main.Text");
      this.toolTip.SetToolTip(this.tab_main, resources.GetString("tab_main.ToolTip"));
      this.tab_main.ToolTipText = resources.GetString("tab_main.ToolTipText");
      this.tab_main.Visible = ((bool)(resources.GetObject("tab_main.Visible")));
      // 
      // tab_files
      // 
      this.tab_files.AccessibleDescription = resources.GetString("tab_files.AccessibleDescription");
      this.tab_files.AccessibleName = resources.GetString("tab_files.AccessibleName");
      this.tab_files.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab_files.Anchor")));
      this.tab_files.AutoScroll = ((bool)(resources.GetObject("tab_files.AutoScroll")));
      this.tab_files.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tab_files.AutoScrollMargin")));
      this.tab_files.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tab_files.AutoScrollMinSize")));
      this.tab_files.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab_files.BackgroundImage")));
      this.tab_files.Controls.Add(this.panel_prof_files);
      this.tab_files.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab_files.Dock")));
      this.tab_files.Enabled = ((bool)(resources.GetObject("tab_files.Enabled")));
      this.tab_files.Font = ((System.Drawing.Font)(resources.GetObject("tab_files.Font")));
      this.tab_files.ImageIndex = ((int)(resources.GetObject("tab_files.ImageIndex")));
      this.tab_files.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab_files.ImeMode")));
      this.tab_files.Location = ((System.Drawing.Point)(resources.GetObject("tab_files.Location")));
      this.tab_files.Name = "tab_files";
      this.tab_files.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab_files.RightToLeft")));
      this.tab_files.Size = ((System.Drawing.Size)(resources.GetObject("tab_files.Size")));
      this.tab_files.TabIndex = ((int)(resources.GetObject("tab_files.TabIndex")));
      this.tab_files.Text = resources.GetString("tab_files.Text");
      this.toolTip.SetToolTip(this.tab_files, resources.GetString("tab_files.ToolTip"));
      this.tab_files.ToolTipText = resources.GetString("tab_files.ToolTipText");
      this.tab_files.Visible = ((bool)(resources.GetObject("tab_files.Visible")));
      // 
      // panel_prof_files
      // 
      this.panel_prof_files.AccessibleDescription = resources.GetString("panel_prof_files.AccessibleDescription");
      this.panel_prof_files.AccessibleName = resources.GetString("panel_prof_files.AccessibleName");
      this.panel_prof_files.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panel_prof_files.Anchor")));
      this.panel_prof_files.AutoScroll = ((bool)(resources.GetObject("panel_prof_files.AutoScroll")));
      this.panel_prof_files.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panel_prof_files.AutoScrollMargin")));
      this.panel_prof_files.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panel_prof_files.AutoScrollMinSize")));
      this.panel_prof_files.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel_prof_files.BackgroundImage")));
      this.panel_prof_files.Controls.Add(this.button_prof_choose_img);
      this.panel_prof_files.Controls.Add(this.button_prof_choose_exe);
      this.panel_prof_files.Controls.Add(this.panel_gameExe);
      this.panel_prof_files.Controls.Add(this.combo_prof_img);
      this.panel_prof_files.Controls.Add(this.check_prof_shellLink);
      this.panel_prof_files.Controls.Add(this.button_prof_mount_img);
      this.panel_prof_files.Controls.Add(this.button_prof_make_link);
      this.panel_prof_files.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panel_prof_files.Dock")));
      this.panel_prof_files.Enabled = ((bool)(resources.GetObject("panel_prof_files.Enabled")));
      this.panel_prof_files.Font = ((System.Drawing.Font)(resources.GetObject("panel_prof_files.Font")));
      this.panel_prof_files.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panel_prof_files.ImeMode")));
      this.panel_prof_files.Location = ((System.Drawing.Point)(resources.GetObject("panel_prof_files.Location")));
      this.panel_prof_files.Name = "panel_prof_files";
      this.panel_prof_files.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panel_prof_files.RightToLeft")));
      this.panel_prof_files.Size = ((System.Drawing.Size)(resources.GetObject("panel_prof_files.Size")));
      this.panel_prof_files.TabIndex = ((int)(resources.GetObject("panel_prof_files.TabIndex")));
      this.panel_prof_files.Text = resources.GetString("panel_prof_files.Text");
      this.toolTip.SetToolTip(this.panel_prof_files, resources.GetString("panel_prof_files.ToolTip"));
      this.panel_prof_files.Visible = ((bool)(resources.GetObject("panel_prof_files.Visible")));
      // 
      // panel_gameExe
      // 
      this.panel_gameExe.AccessibleDescription = resources.GetString("panel_gameExe.AccessibleDescription");
      this.panel_gameExe.AccessibleName = resources.GetString("panel_gameExe.AccessibleName");
      this.panel_gameExe.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panel_gameExe.Anchor")));
      this.panel_gameExe.AutoScroll = ((bool)(resources.GetObject("panel_gameExe.AutoScroll")));
      this.panel_gameExe.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panel_gameExe.AutoScrollMargin")));
      this.panel_gameExe.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panel_gameExe.AutoScrollMinSize")));
      this.panel_gameExe.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel_gameExe.BackgroundImage")));
      this.panel_gameExe.Controls.Add(this.text_prof_exe_args);
      this.panel_gameExe.Controls.Add(this.splitter_prof_gameExe);
      this.panel_gameExe.Controls.Add(this.text_prof_exe_path);
      this.panel_gameExe.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panel_gameExe.Dock")));
      this.panel_gameExe.Enabled = ((bool)(resources.GetObject("panel_gameExe.Enabled")));
      this.panel_gameExe.Font = ((System.Drawing.Font)(resources.GetObject("panel_gameExe.Font")));
      this.panel_gameExe.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panel_gameExe.ImeMode")));
      this.panel_gameExe.Location = ((System.Drawing.Point)(resources.GetObject("panel_gameExe.Location")));
      this.panel_gameExe.Name = "panel_gameExe";
      this.panel_gameExe.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panel_gameExe.RightToLeft")));
      this.panel_gameExe.Size = ((System.Drawing.Size)(resources.GetObject("panel_gameExe.Size")));
      this.panel_gameExe.TabIndex = ((int)(resources.GetObject("panel_gameExe.TabIndex")));
      this.panel_gameExe.Text = resources.GetString("panel_gameExe.Text");
      this.toolTip.SetToolTip(this.panel_gameExe, resources.GetString("panel_gameExe.ToolTip"));
      this.panel_gameExe.Visible = ((bool)(resources.GetObject("panel_gameExe.Visible")));
      // 
      // splitter_prof_gameExe
      // 
      this.splitter_prof_gameExe.AccessibleDescription = resources.GetString("splitter_prof_gameExe.AccessibleDescription");
      this.splitter_prof_gameExe.AccessibleName = resources.GetString("splitter_prof_gameExe.AccessibleName");
      this.splitter_prof_gameExe.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("splitter_prof_gameExe.Anchor")));
      this.splitter_prof_gameExe.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splitter_prof_gameExe.BackgroundImage")));
      this.splitter_prof_gameExe.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("splitter_prof_gameExe.Dock")));
      this.splitter_prof_gameExe.Enabled = ((bool)(resources.GetObject("splitter_prof_gameExe.Enabled")));
      this.splitter_prof_gameExe.Font = ((System.Drawing.Font)(resources.GetObject("splitter_prof_gameExe.Font")));
      this.splitter_prof_gameExe.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("splitter_prof_gameExe.ImeMode")));
      this.splitter_prof_gameExe.Location = ((System.Drawing.Point)(resources.GetObject("splitter_prof_gameExe.Location")));
      this.splitter_prof_gameExe.MinExtra = ((int)(resources.GetObject("splitter_prof_gameExe.MinExtra")));
      this.splitter_prof_gameExe.MinSize = ((int)(resources.GetObject("splitter_prof_gameExe.MinSize")));
      this.splitter_prof_gameExe.Name = "splitter_prof_gameExe";
      this.splitter_prof_gameExe.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("splitter_prof_gameExe.RightToLeft")));
      this.splitter_prof_gameExe.Size = ((System.Drawing.Size)(resources.GetObject("splitter_prof_gameExe.Size")));
      this.splitter_prof_gameExe.TabIndex = ((int)(resources.GetObject("splitter_prof_gameExe.TabIndex")));
      this.splitter_prof_gameExe.TabStop = false;
      this.toolTip.SetToolTip(this.splitter_prof_gameExe, resources.GetString("splitter_prof_gameExe.ToolTip"));
      this.splitter_prof_gameExe.Visible = ((bool)(resources.GetObject("splitter_prof_gameExe.Visible")));
      // 
      // check_prof_shellLink
      // 
      this.check_prof_shellLink.AccessibleDescription = resources.GetString("check_prof_shellLink.AccessibleDescription");
      this.check_prof_shellLink.AccessibleName = resources.GetString("check_prof_shellLink.AccessibleName");
      this.check_prof_shellLink.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_prof_shellLink.Anchor")));
      this.check_prof_shellLink.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_prof_shellLink.Appearance")));
      this.check_prof_shellLink.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_prof_shellLink.BackgroundImage")));
      this.check_prof_shellLink.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_shellLink.CheckAlign")));
      this.check_prof_shellLink.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_prof_shellLink.Dock")));
      this.check_prof_shellLink.Enabled = ((bool)(resources.GetObject("check_prof_shellLink.Enabled")));
      this.check_prof_shellLink.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_prof_shellLink.FlatStyle")));
      this.check_prof_shellLink.Font = ((System.Drawing.Font)(resources.GetObject("check_prof_shellLink.Font")));
      this.check_prof_shellLink.Image = ((System.Drawing.Image)(resources.GetObject("check_prof_shellLink.Image")));
      this.check_prof_shellLink.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_shellLink.ImageAlign")));
      this.check_prof_shellLink.ImageIndex = ((int)(resources.GetObject("check_prof_shellLink.ImageIndex")));
      this.check_prof_shellLink.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_prof_shellLink.ImeMode")));
      this.check_prof_shellLink.Location = ((System.Drawing.Point)(resources.GetObject("check_prof_shellLink.Location")));
      this.check_prof_shellLink.Name = "check_prof_shellLink";
      this.check_prof_shellLink.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_prof_shellLink.RightToLeft")));
      this.check_prof_shellLink.Size = ((System.Drawing.Size)(resources.GetObject("check_prof_shellLink.Size")));
      this.check_prof_shellLink.TabIndex = ((int)(resources.GetObject("check_prof_shellLink.TabIndex")));
      this.check_prof_shellLink.Text = resources.GetString("check_prof_shellLink.Text");
      this.check_prof_shellLink.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_shellLink.TextAlign")));
      this.toolTip.SetToolTip(this.check_prof_shellLink, resources.GetString("check_prof_shellLink.ToolTip"));
      this.check_prof_shellLink.Visible = ((bool)(resources.GetObject("check_prof_shellLink.Visible")));
      this.check_prof_shellLink.CheckedChanged += new System.EventHandler(this.check_prof_shellLink_CheckedChanged);
      // 
      // tab_extra_d3d
      // 
      this.tab_extra_d3d.AccessibleDescription = resources.GetString("tab_extra_d3d.AccessibleDescription");
      this.tab_extra_d3d.AccessibleName = resources.GetString("tab_extra_d3d.AccessibleName");
      this.tab_extra_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab_extra_d3d.Anchor")));
      this.tab_extra_d3d.AutoScroll = ((bool)(resources.GetObject("tab_extra_d3d.AutoScroll")));
      this.tab_extra_d3d.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tab_extra_d3d.AutoScrollMargin")));
      this.tab_extra_d3d.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tab_extra_d3d.AutoScrollMinSize")));
      this.tab_extra_d3d.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab_extra_d3d.BackgroundImage")));
      this.tab_extra_d3d.Controls.Add(this.group_extra_d3d);
      this.tab_extra_d3d.Controls.Add(this.group_extra_d3d_2);
      this.tab_extra_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab_extra_d3d.Dock")));
      this.tab_extra_d3d.Enabled = ((bool)(resources.GetObject("tab_extra_d3d.Enabled")));
      this.tab_extra_d3d.Font = ((System.Drawing.Font)(resources.GetObject("tab_extra_d3d.Font")));
      this.tab_extra_d3d.ImageIndex = ((int)(resources.GetObject("tab_extra_d3d.ImageIndex")));
      this.tab_extra_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab_extra_d3d.ImeMode")));
      this.tab_extra_d3d.Location = ((System.Drawing.Point)(resources.GetObject("tab_extra_d3d.Location")));
      this.tab_extra_d3d.Name = "tab_extra_d3d";
      this.tab_extra_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab_extra_d3d.RightToLeft")));
      this.tab_extra_d3d.Size = ((System.Drawing.Size)(resources.GetObject("tab_extra_d3d.Size")));
      this.tab_extra_d3d.TabIndex = ((int)(resources.GetObject("tab_extra_d3d.TabIndex")));
      this.tab_extra_d3d.Text = resources.GetString("tab_extra_d3d.Text");
      this.toolTip.SetToolTip(this.tab_extra_d3d, resources.GetString("tab_extra_d3d.ToolTip"));
      this.tab_extra_d3d.ToolTipText = resources.GetString("tab_extra_d3d.ToolTipText");
      this.tab_extra_d3d.Visible = ((bool)(resources.GetObject("tab_extra_d3d.Visible")));
      // 
      // group_extra_d3d_2
      // 
      this.group_extra_d3d_2.AccessibleDescription = resources.GetString("group_extra_d3d_2.AccessibleDescription");
      this.group_extra_d3d_2.AccessibleName = resources.GetString("group_extra_d3d_2.AccessibleName");
      this.group_extra_d3d_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("group_extra_d3d_2.Anchor")));
      this.group_extra_d3d_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("group_extra_d3d_2.BackgroundImage")));
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_prof_d3d_5);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_prof_d3d_7);
      this.group_extra_d3d_2.Controls.Add(this.label_extra2_combo_d3d_2);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_curr_d3d_1);
      this.group_extra_d3d_2.Controls.Add(this.label_extra2_combo_d3d_3);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_curr_d3d_3);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_prof_d3d_3);
      this.group_extra_d3d_2.Controls.Add(this.label_extra2_combo_d3d_6);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_curr_d3d_7);
      this.group_extra_d3d_2.Controls.Add(this.label_extra2_combo_d3d_1);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_prof_d3d_4);
      this.group_extra_d3d_2.Controls.Add(this.label_extra2_combo_d3d_4);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_prof_d3d_6);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_prof_d3d_2);
      this.group_extra_d3d_2.Controls.Add(this.label_extra2_prof_d3d);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_curr_d3d_8);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_prof_d3d_8);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_curr_d3d_4);
      this.group_extra_d3d_2.Controls.Add(this.label_extra2_combo_d3d_5);
      this.group_extra_d3d_2.Controls.Add(this.label_extra2_curr_d3d);
      this.group_extra_d3d_2.Controls.Add(this.label_extra2_combo_d3d_8);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_prof_d3d_1);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_curr_d3d_5);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_curr_d3d_2);
      this.group_extra_d3d_2.Controls.Add(this.combo_extra2_curr_d3d_6);
      this.group_extra_d3d_2.Controls.Add(this.label_extra2_combo_d3d_7);
      this.group_extra_d3d_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("group_extra_d3d_2.Dock")));
      this.group_extra_d3d_2.Enabled = ((bool)(resources.GetObject("group_extra_d3d_2.Enabled")));
      this.group_extra_d3d_2.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.group_extra_d3d_2.Font = ((System.Drawing.Font)(resources.GetObject("group_extra_d3d_2.Font")));
      this.group_extra_d3d_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("group_extra_d3d_2.ImeMode")));
      this.group_extra_d3d_2.Location = ((System.Drawing.Point)(resources.GetObject("group_extra_d3d_2.Location")));
      this.group_extra_d3d_2.Name = "group_extra_d3d_2";
      this.group_extra_d3d_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("group_extra_d3d_2.RightToLeft")));
      this.group_extra_d3d_2.Size = ((System.Drawing.Size)(resources.GetObject("group_extra_d3d_2.Size")));
      this.group_extra_d3d_2.TabIndex = ((int)(resources.GetObject("group_extra_d3d_2.TabIndex")));
      this.group_extra_d3d_2.TabStop = false;
      this.group_extra_d3d_2.Text = resources.GetString("group_extra_d3d_2.Text");
      this.toolTip.SetToolTip(this.group_extra_d3d_2, resources.GetString("group_extra_d3d_2.ToolTip"));
      this.group_extra_d3d_2.Visible = ((bool)(resources.GetObject("group_extra_d3d_2.Visible")));
      // 
      // tab_extra_ogl
      // 
      this.tab_extra_ogl.AccessibleDescription = resources.GetString("tab_extra_ogl.AccessibleDescription");
      this.tab_extra_ogl.AccessibleName = resources.GetString("tab_extra_ogl.AccessibleName");
      this.tab_extra_ogl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab_extra_ogl.Anchor")));
      this.tab_extra_ogl.AutoScroll = ((bool)(resources.GetObject("tab_extra_ogl.AutoScroll")));
      this.tab_extra_ogl.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tab_extra_ogl.AutoScrollMargin")));
      this.tab_extra_ogl.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tab_extra_ogl.AutoScrollMinSize")));
      this.tab_extra_ogl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab_extra_ogl.BackgroundImage")));
      this.tab_extra_ogl.Controls.Add(this.group_extra_ogl);
      this.tab_extra_ogl.Controls.Add(this.group_extra_ogl_2);
      this.tab_extra_ogl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab_extra_ogl.Dock")));
      this.tab_extra_ogl.Enabled = ((bool)(resources.GetObject("tab_extra_ogl.Enabled")));
      this.tab_extra_ogl.Font = ((System.Drawing.Font)(resources.GetObject("tab_extra_ogl.Font")));
      this.tab_extra_ogl.ImageIndex = ((int)(resources.GetObject("tab_extra_ogl.ImageIndex")));
      this.tab_extra_ogl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab_extra_ogl.ImeMode")));
      this.tab_extra_ogl.Location = ((System.Drawing.Point)(resources.GetObject("tab_extra_ogl.Location")));
      this.tab_extra_ogl.Name = "tab_extra_ogl";
      this.tab_extra_ogl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab_extra_ogl.RightToLeft")));
      this.tab_extra_ogl.Size = ((System.Drawing.Size)(resources.GetObject("tab_extra_ogl.Size")));
      this.tab_extra_ogl.TabIndex = ((int)(resources.GetObject("tab_extra_ogl.TabIndex")));
      this.tab_extra_ogl.Text = resources.GetString("tab_extra_ogl.Text");
      this.toolTip.SetToolTip(this.tab_extra_ogl, resources.GetString("tab_extra_ogl.ToolTip"));
      this.tab_extra_ogl.ToolTipText = resources.GetString("tab_extra_ogl.ToolTipText");
      this.tab_extra_ogl.Visible = ((bool)(resources.GetObject("tab_extra_ogl.Visible")));
      // 
      // group_extra_ogl_2
      // 
      this.group_extra_ogl_2.AccessibleDescription = resources.GetString("group_extra_ogl_2.AccessibleDescription");
      this.group_extra_ogl_2.AccessibleName = resources.GetString("group_extra_ogl_2.AccessibleName");
      this.group_extra_ogl_2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("group_extra_ogl_2.Anchor")));
      this.group_extra_ogl_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("group_extra_ogl_2.BackgroundImage")));
      this.group_extra_ogl_2.Controls.Add(this.label_extra2_combo_ogl_1);
      this.group_extra_ogl_2.Controls.Add(this.label_extra2_combo_ogl_4);
      this.group_extra_ogl_2.Controls.Add(this.label_extra2_combo_ogl_7);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_curr_ogl_3);
      this.group_extra_ogl_2.Controls.Add(this.label_extra2_combo_ogl_6);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_curr_ogl_1);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_prof_ogl_8);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_curr_ogl_5);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_prof_ogl_1);
      this.group_extra_ogl_2.Controls.Add(this.label_extra2_combo_ogl_8);
      this.group_extra_ogl_2.Controls.Add(this.label_extra2_curr_ogl);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_curr_ogl_8);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_prof_ogl_5);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_curr_ogl_2);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_curr_ogl_4);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_curr_ogl_7);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_curr_ogl_6);
      this.group_extra_ogl_2.Controls.Add(this.label_extra2_prof_ogl);
      this.group_extra_ogl_2.Controls.Add(this.label_extra2_combo_ogl_2);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_prof_ogl_7);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_prof_ogl_3);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_prof_ogl_2);
      this.group_extra_ogl_2.Controls.Add(this.label_extra2_combo_ogl_3);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_prof_ogl_6);
      this.group_extra_ogl_2.Controls.Add(this.label_extra2_combo_ogl_5);
      this.group_extra_ogl_2.Controls.Add(this.combo_extra2_prof_ogl_4);
      this.group_extra_ogl_2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("group_extra_ogl_2.Dock")));
      this.group_extra_ogl_2.Enabled = ((bool)(resources.GetObject("group_extra_ogl_2.Enabled")));
      this.group_extra_ogl_2.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.group_extra_ogl_2.Font = ((System.Drawing.Font)(resources.GetObject("group_extra_ogl_2.Font")));
      this.group_extra_ogl_2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("group_extra_ogl_2.ImeMode")));
      this.group_extra_ogl_2.Location = ((System.Drawing.Point)(resources.GetObject("group_extra_ogl_2.Location")));
      this.group_extra_ogl_2.Name = "group_extra_ogl_2";
      this.group_extra_ogl_2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("group_extra_ogl_2.RightToLeft")));
      this.group_extra_ogl_2.Size = ((System.Drawing.Size)(resources.GetObject("group_extra_ogl_2.Size")));
      this.group_extra_ogl_2.TabIndex = ((int)(resources.GetObject("group_extra_ogl_2.TabIndex")));
      this.group_extra_ogl_2.TabStop = false;
      this.group_extra_ogl_2.Text = resources.GetString("group_extra_ogl_2.Text");
      this.toolTip.SetToolTip(this.group_extra_ogl_2, resources.GetString("group_extra_ogl_2.ToolTip"));
      this.group_extra_ogl_2.Visible = ((bool)(resources.GetObject("group_extra_ogl_2.Visible")));
      // 
      // tab_summary
      // 
      this.tab_summary.AccessibleDescription = resources.GetString("tab_summary.AccessibleDescription");
      this.tab_summary.AccessibleName = resources.GetString("tab_summary.AccessibleName");
      this.tab_summary.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab_summary.Anchor")));
      this.tab_summary.AutoScroll = ((bool)(resources.GetObject("tab_summary.AutoScroll")));
      this.tab_summary.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tab_summary.AutoScrollMargin")));
      this.tab_summary.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tab_summary.AutoScrollMinSize")));
      this.tab_summary.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab_summary.BackgroundImage")));
      this.tab_summary.Controls.Add(this.text_summary);
      this.tab_summary.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab_summary.Dock")));
      this.tab_summary.Enabled = ((bool)(resources.GetObject("tab_summary.Enabled")));
      this.tab_summary.Font = ((System.Drawing.Font)(resources.GetObject("tab_summary.Font")));
      this.tab_summary.ImageIndex = ((int)(resources.GetObject("tab_summary.ImageIndex")));
      this.tab_summary.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab_summary.ImeMode")));
      this.tab_summary.Location = ((System.Drawing.Point)(resources.GetObject("tab_summary.Location")));
      this.tab_summary.Name = "tab_summary";
      this.tab_summary.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab_summary.RightToLeft")));
      this.tab_summary.Size = ((System.Drawing.Size)(resources.GetObject("tab_summary.Size")));
      this.tab_summary.TabIndex = ((int)(resources.GetObject("tab_summary.TabIndex")));
      this.tab_summary.Text = resources.GetString("tab_summary.Text");
      this.toolTip.SetToolTip(this.tab_summary, resources.GetString("tab_summary.ToolTip"));
      this.tab_summary.ToolTipText = resources.GetString("tab_summary.ToolTipText");
      this.tab_summary.Visible = ((bool)(resources.GetObject("tab_summary.Visible")));
      // 
      // text_summary
      // 
      this.text_summary.AccessibleDescription = resources.GetString("text_summary.AccessibleDescription");
      this.text_summary.AccessibleName = resources.GetString("text_summary.AccessibleName");
      this.text_summary.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_summary.Anchor")));
      this.text_summary.AutoSize = ((bool)(resources.GetObject("text_summary.AutoSize")));
      this.text_summary.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_summary.BackgroundImage")));
      this.text_summary.BulletIndent = ((int)(resources.GetObject("text_summary.BulletIndent")));
      this.text_summary.DetectUrls = false;
      this.text_summary.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_summary.Dock")));
      this.text_summary.Enabled = ((bool)(resources.GetObject("text_summary.Enabled")));
      this.text_summary.Font = ((System.Drawing.Font)(resources.GetObject("text_summary.Font")));
      this.text_summary.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_summary.ImeMode")));
      this.text_summary.Location = ((System.Drawing.Point)(resources.GetObject("text_summary.Location")));
      this.text_summary.MaxLength = ((int)(resources.GetObject("text_summary.MaxLength")));
      this.text_summary.Multiline = ((bool)(resources.GetObject("text_summary.Multiline")));
      this.text_summary.Name = "text_summary";
      this.text_summary.ReadOnly = true;
      this.text_summary.RightMargin = ((int)(resources.GetObject("text_summary.RightMargin")));
      this.text_summary.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_summary.RightToLeft")));
      this.text_summary.ScrollBars = ((System.Windows.Forms.RichTextBoxScrollBars)(resources.GetObject("text_summary.ScrollBars")));
      this.text_summary.Size = ((System.Drawing.Size)(resources.GetObject("text_summary.Size")));
      this.text_summary.TabIndex = ((int)(resources.GetObject("text_summary.TabIndex")));
      this.text_summary.TabStop = false;
      this.text_summary.Text = resources.GetString("text_summary.Text");
      this.toolTip.SetToolTip(this.text_summary, resources.GetString("text_summary.ToolTip"));
      this.text_summary.Visible = ((bool)(resources.GetObject("text_summary.Visible")));
      this.text_summary.WordWrap = ((bool)(resources.GetObject("text_summary.WordWrap")));
      this.text_summary.ZoomFactor = ((System.Single)(resources.GetObject("text_summary.ZoomFactor")));
      this.text_summary.VisibleChanged += new System.EventHandler(this.text_summary_VisibleChanged);
      // 
      // tab_clocking
      // 
      this.tab_clocking.AccessibleDescription = resources.GetString("tab_clocking.AccessibleDescription");
      this.tab_clocking.AccessibleName = resources.GetString("tab_clocking.AccessibleName");
      this.tab_clocking.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab_clocking.Anchor")));
      this.tab_clocking.AutoScroll = ((bool)(resources.GetObject("tab_clocking.AutoScroll")));
      this.tab_clocking.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tab_clocking.AutoScrollMargin")));
      this.tab_clocking.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tab_clocking.AutoScrollMinSize")));
      this.tab_clocking.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab_clocking.BackgroundImage")));
      this.tab_clocking.Controls.Add(this.group_clocking_curr);
      this.tab_clocking.Controls.Add(this.splitter_clocking);
      this.tab_clocking.Controls.Add(this.group_clocking_prof);
      this.tab_clocking.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab_clocking.Dock")));
      this.tab_clocking.Enabled = ((bool)(resources.GetObject("tab_clocking.Enabled")));
      this.tab_clocking.Font = ((System.Drawing.Font)(resources.GetObject("tab_clocking.Font")));
      this.tab_clocking.ImageIndex = ((int)(resources.GetObject("tab_clocking.ImageIndex")));
      this.tab_clocking.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab_clocking.ImeMode")));
      this.tab_clocking.Location = ((System.Drawing.Point)(resources.GetObject("tab_clocking.Location")));
      this.tab_clocking.Name = "tab_clocking";
      this.tab_clocking.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab_clocking.RightToLeft")));
      this.tab_clocking.Size = ((System.Drawing.Size)(resources.GetObject("tab_clocking.Size")));
      this.tab_clocking.TabIndex = ((int)(resources.GetObject("tab_clocking.TabIndex")));
      this.tab_clocking.Text = resources.GetString("tab_clocking.Text");
      this.toolTip.SetToolTip(this.tab_clocking, resources.GetString("tab_clocking.ToolTip"));
      this.tab_clocking.ToolTipText = resources.GetString("tab_clocking.ToolTipText");
      this.tab_clocking.Visible = ((bool)(resources.GetObject("tab_clocking.Visible")));
      // 
      // group_clocking_curr
      // 
      this.group_clocking_curr.AccessibleDescription = resources.GetString("group_clocking_curr.AccessibleDescription");
      this.group_clocking_curr.AccessibleName = resources.GetString("group_clocking_curr.AccessibleName");
      this.group_clocking_curr.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("group_clocking_curr.Anchor")));
      this.group_clocking_curr.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("group_clocking_curr.BackgroundImage")));
      this.group_clocking_curr.Controls.Add(this.button_clocking_set);
      this.group_clocking_curr.Controls.Add(this.button_clocking_reset);
      this.group_clocking_curr.Controls.Add(this.text_clocking_curr_core);
      this.group_clocking_curr.Controls.Add(this.label3);
      this.group_clocking_curr.Controls.Add(this.track_clocking_curr_mem);
      this.group_clocking_curr.Controls.Add(this.track_clocking_curr_core);
      this.group_clocking_curr.Controls.Add(this.label1);
      this.group_clocking_curr.Controls.Add(this.text_clocking_curr_mem);
      this.group_clocking_curr.Controls.Add(this.button_clocking_refresh);
      this.group_clocking_curr.Controls.Add(this.group_clocking_current_presets);
      this.group_clocking_curr.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("group_clocking_curr.Dock")));
      this.group_clocking_curr.Enabled = ((bool)(resources.GetObject("group_clocking_curr.Enabled")));
      this.group_clocking_curr.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.group_clocking_curr.Font = ((System.Drawing.Font)(resources.GetObject("group_clocking_curr.Font")));
      this.group_clocking_curr.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("group_clocking_curr.ImeMode")));
      this.group_clocking_curr.Location = ((System.Drawing.Point)(resources.GetObject("group_clocking_curr.Location")));
      this.group_clocking_curr.Name = "group_clocking_curr";
      this.group_clocking_curr.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("group_clocking_curr.RightToLeft")));
      this.group_clocking_curr.Size = ((System.Drawing.Size)(resources.GetObject("group_clocking_curr.Size")));
      this.group_clocking_curr.TabIndex = ((int)(resources.GetObject("group_clocking_curr.TabIndex")));
      this.group_clocking_curr.TabStop = false;
      this.group_clocking_curr.Text = resources.GetString("group_clocking_curr.Text");
      this.toolTip.SetToolTip(this.group_clocking_curr, resources.GetString("group_clocking_curr.ToolTip"));
      this.group_clocking_curr.Visible = ((bool)(resources.GetObject("group_clocking_curr.Visible")));
      // 
      // button_clocking_set
      // 
      this.button_clocking_set.AccessibleDescription = resources.GetString("button_clocking_set.AccessibleDescription");
      this.button_clocking_set.AccessibleName = resources.GetString("button_clocking_set.AccessibleName");
      this.button_clocking_set.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_clocking_set.Anchor")));
      this.button_clocking_set.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_clocking_set.BackgroundImage")));
      this.button_clocking_set.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_clocking_set.Dock")));
      this.button_clocking_set.Enabled = ((bool)(resources.GetObject("button_clocking_set.Enabled")));
      this.button_clocking_set.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_clocking_set.FlatStyle")));
      this.button_clocking_set.Font = ((System.Drawing.Font)(resources.GetObject("button_clocking_set.Font")));
      this.button_clocking_set.Image = ((System.Drawing.Image)(resources.GetObject("button_clocking_set.Image")));
      this.button_clocking_set.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_set.ImageAlign")));
      this.button_clocking_set.ImageIndex = ((int)(resources.GetObject("button_clocking_set.ImageIndex")));
      this.button_clocking_set.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_clocking_set.ImeMode")));
      this.button_clocking_set.Location = ((System.Drawing.Point)(resources.GetObject("button_clocking_set.Location")));
      this.button_clocking_set.Name = "button_clocking_set";
      this.button_clocking_set.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_clocking_set.RightToLeft")));
      this.button_clocking_set.Size = ((System.Drawing.Size)(resources.GetObject("button_clocking_set.Size")));
      this.button_clocking_set.TabIndex = ((int)(resources.GetObject("button_clocking_set.TabIndex")));
      this.button_clocking_set.Text = resources.GetString("button_clocking_set.Text");
      this.button_clocking_set.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_set.TextAlign")));
      this.toolTip.SetToolTip(this.button_clocking_set, resources.GetString("button_clocking_set.ToolTip"));
      this.button_clocking_set.Visible = ((bool)(resources.GetObject("button_clocking_set.Visible")));
      this.button_clocking_set.Click += new System.EventHandler(this.button_clocking_set_Click);
      // 
      // button_clocking_reset
      // 
      this.button_clocking_reset.AccessibleDescription = resources.GetString("button_clocking_reset.AccessibleDescription");
      this.button_clocking_reset.AccessibleName = resources.GetString("button_clocking_reset.AccessibleName");
      this.button_clocking_reset.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_clocking_reset.Anchor")));
      this.button_clocking_reset.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_clocking_reset.BackgroundImage")));
      this.button_clocking_reset.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_clocking_reset.Dock")));
      this.button_clocking_reset.Enabled = ((bool)(resources.GetObject("button_clocking_reset.Enabled")));
      this.button_clocking_reset.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_clocking_reset.FlatStyle")));
      this.button_clocking_reset.Font = ((System.Drawing.Font)(resources.GetObject("button_clocking_reset.Font")));
      this.button_clocking_reset.Image = ((System.Drawing.Image)(resources.GetObject("button_clocking_reset.Image")));
      this.button_clocking_reset.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_reset.ImageAlign")));
      this.button_clocking_reset.ImageIndex = ((int)(resources.GetObject("button_clocking_reset.ImageIndex")));
      this.button_clocking_reset.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_clocking_reset.ImeMode")));
      this.button_clocking_reset.Location = ((System.Drawing.Point)(resources.GetObject("button_clocking_reset.Location")));
      this.button_clocking_reset.Name = "button_clocking_reset";
      this.button_clocking_reset.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_clocking_reset.RightToLeft")));
      this.button_clocking_reset.Size = ((System.Drawing.Size)(resources.GetObject("button_clocking_reset.Size")));
      this.button_clocking_reset.TabIndex = ((int)(resources.GetObject("button_clocking_reset.TabIndex")));
      this.button_clocking_reset.Text = resources.GetString("button_clocking_reset.Text");
      this.button_clocking_reset.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_reset.TextAlign")));
      this.toolTip.SetToolTip(this.button_clocking_reset, resources.GetString("button_clocking_reset.ToolTip"));
      this.button_clocking_reset.Visible = ((bool)(resources.GetObject("button_clocking_reset.Visible")));
      this.button_clocking_reset.Click += new System.EventHandler(this.button_clocking_reset_Click);
      // 
      // text_clocking_curr_core
      // 
      this.text_clocking_curr_core.AccessibleDescription = resources.GetString("text_clocking_curr_core.AccessibleDescription");
      this.text_clocking_curr_core.AccessibleName = resources.GetString("text_clocking_curr_core.AccessibleName");
      this.text_clocking_curr_core.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_clocking_curr_core.Anchor")));
      this.text_clocking_curr_core.AutoSize = ((bool)(resources.GetObject("text_clocking_curr_core.AutoSize")));
      this.text_clocking_curr_core.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_clocking_curr_core.BackgroundImage")));
      this.text_clocking_curr_core.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.text_clocking_curr_core.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_clocking_curr_core.Dock")));
      this.text_clocking_curr_core.Enabled = ((bool)(resources.GetObject("text_clocking_curr_core.Enabled")));
      this.text_clocking_curr_core.Font = ((System.Drawing.Font)(resources.GetObject("text_clocking_curr_core.Font")));
      this.text_clocking_curr_core.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_clocking_curr_core.ImeMode")));
      this.text_clocking_curr_core.Location = ((System.Drawing.Point)(resources.GetObject("text_clocking_curr_core.Location")));
      this.text_clocking_curr_core.MaxLength = ((int)(resources.GetObject("text_clocking_curr_core.MaxLength")));
      this.text_clocking_curr_core.Multiline = ((bool)(resources.GetObject("text_clocking_curr_core.Multiline")));
      this.text_clocking_curr_core.Name = "text_clocking_curr_core";
      this.text_clocking_curr_core.PasswordChar = ((char)(resources.GetObject("text_clocking_curr_core.PasswordChar")));
      this.text_clocking_curr_core.ReadOnly = true;
      this.text_clocking_curr_core.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_clocking_curr_core.RightToLeft")));
      this.text_clocking_curr_core.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_clocking_curr_core.ScrollBars")));
      this.text_clocking_curr_core.Size = ((System.Drawing.Size)(resources.GetObject("text_clocking_curr_core.Size")));
      this.text_clocking_curr_core.TabIndex = ((int)(resources.GetObject("text_clocking_curr_core.TabIndex")));
      this.text_clocking_curr_core.TabStop = false;
      this.text_clocking_curr_core.Text = resources.GetString("text_clocking_curr_core.Text");
      this.text_clocking_curr_core.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_clocking_curr_core.TextAlign")));
      this.toolTip.SetToolTip(this.text_clocking_curr_core, resources.GetString("text_clocking_curr_core.ToolTip"));
      this.text_clocking_curr_core.Visible = ((bool)(resources.GetObject("text_clocking_curr_core.Visible")));
      this.text_clocking_curr_core.WordWrap = ((bool)(resources.GetObject("text_clocking_curr_core.WordWrap")));
      this.text_clocking_curr_core.TextChanged += new System.EventHandler(this.text_clocking_curr_core_TextChanged);
      // 
      // label3
      // 
      this.label3.AccessibleDescription = resources.GetString("label3.AccessibleDescription");
      this.label3.AccessibleName = resources.GetString("label3.AccessibleName");
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label3.Anchor")));
      this.label3.AutoSize = ((bool)(resources.GetObject("label3.AutoSize")));
      this.label3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label3.Dock")));
      this.label3.Enabled = ((bool)(resources.GetObject("label3.Enabled")));
      this.label3.Font = ((System.Drawing.Font)(resources.GetObject("label3.Font")));
      this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
      this.label3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.ImageAlign")));
      this.label3.ImageIndex = ((int)(resources.GetObject("label3.ImageIndex")));
      this.label3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label3.ImeMode")));
      this.label3.Location = ((System.Drawing.Point)(resources.GetObject("label3.Location")));
      this.label3.Name = "label3";
      this.label3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label3.RightToLeft")));
      this.label3.Size = ((System.Drawing.Size)(resources.GetObject("label3.Size")));
      this.label3.TabIndex = ((int)(resources.GetObject("label3.TabIndex")));
      this.label3.Text = resources.GetString("label3.Text");
      this.label3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.TextAlign")));
      this.toolTip.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
      this.label3.Visible = ((bool)(resources.GetObject("label3.Visible")));
      // 
      // track_clocking_curr_mem
      // 
      this.track_clocking_curr_mem.AccessibleDescription = resources.GetString("track_clocking_curr_mem.AccessibleDescription");
      this.track_clocking_curr_mem.AccessibleName = resources.GetString("track_clocking_curr_mem.AccessibleName");
      this.track_clocking_curr_mem.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("track_clocking_curr_mem.Anchor")));
      this.track_clocking_curr_mem.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("track_clocking_curr_mem.BackgroundImage")));
      this.track_clocking_curr_mem.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("track_clocking_curr_mem.Dock")));
      this.track_clocking_curr_mem.Enabled = ((bool)(resources.GetObject("track_clocking_curr_mem.Enabled")));
      this.track_clocking_curr_mem.Font = ((System.Drawing.Font)(resources.GetObject("track_clocking_curr_mem.Font")));
      this.track_clocking_curr_mem.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("track_clocking_curr_mem.ImeMode")));
      this.track_clocking_curr_mem.Location = ((System.Drawing.Point)(resources.GetObject("track_clocking_curr_mem.Location")));
      this.track_clocking_curr_mem.Maximum = 320;
      this.track_clocking_curr_mem.Minimum = 200;
      this.track_clocking_curr_mem.Name = "track_clocking_curr_mem";
      this.track_clocking_curr_mem.Orientation = ((System.Windows.Forms.Orientation)(resources.GetObject("track_clocking_curr_mem.Orientation")));
      this.track_clocking_curr_mem.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("track_clocking_curr_mem.RightToLeft")));
      this.track_clocking_curr_mem.Size = ((System.Drawing.Size)(resources.GetObject("track_clocking_curr_mem.Size")));
      this.track_clocking_curr_mem.TabIndex = ((int)(resources.GetObject("track_clocking_curr_mem.TabIndex")));
      this.track_clocking_curr_mem.Text = resources.GetString("track_clocking_curr_mem.Text");
      this.track_clocking_curr_mem.TickFrequency = 10;
      this.track_clocking_curr_mem.TickStyle = System.Windows.Forms.TickStyle.None;
      this.toolTip.SetToolTip(this.track_clocking_curr_mem, resources.GetString("track_clocking_curr_mem.ToolTip"));
      this.track_clocking_curr_mem.Value = 200;
      this.track_clocking_curr_mem.Visible = ((bool)(resources.GetObject("track_clocking_curr_mem.Visible")));
      this.track_clocking_curr_mem.Scroll += new System.EventHandler(this.track_clocking_curr_mem_Scroll);
      // 
      // track_clocking_curr_core
      // 
      this.track_clocking_curr_core.AccessibleDescription = resources.GetString("track_clocking_curr_core.AccessibleDescription");
      this.track_clocking_curr_core.AccessibleName = resources.GetString("track_clocking_curr_core.AccessibleName");
      this.track_clocking_curr_core.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("track_clocking_curr_core.Anchor")));
      this.track_clocking_curr_core.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("track_clocking_curr_core.BackgroundImage")));
      this.track_clocking_curr_core.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("track_clocking_curr_core.Dock")));
      this.track_clocking_curr_core.Enabled = ((bool)(resources.GetObject("track_clocking_curr_core.Enabled")));
      this.track_clocking_curr_core.Font = ((System.Drawing.Font)(resources.GetObject("track_clocking_curr_core.Font")));
      this.track_clocking_curr_core.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("track_clocking_curr_core.ImeMode")));
      this.track_clocking_curr_core.Location = ((System.Drawing.Point)(resources.GetObject("track_clocking_curr_core.Location")));
      this.track_clocking_curr_core.Maximum = 400;
      this.track_clocking_curr_core.Minimum = 200;
      this.track_clocking_curr_core.Name = "track_clocking_curr_core";
      this.track_clocking_curr_core.Orientation = ((System.Windows.Forms.Orientation)(resources.GetObject("track_clocking_curr_core.Orientation")));
      this.track_clocking_curr_core.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("track_clocking_curr_core.RightToLeft")));
      this.track_clocking_curr_core.Size = ((System.Drawing.Size)(resources.GetObject("track_clocking_curr_core.Size")));
      this.track_clocking_curr_core.TabIndex = ((int)(resources.GetObject("track_clocking_curr_core.TabIndex")));
      this.track_clocking_curr_core.Text = resources.GetString("track_clocking_curr_core.Text");
      this.track_clocking_curr_core.TickFrequency = 10;
      this.track_clocking_curr_core.TickStyle = System.Windows.Forms.TickStyle.None;
      this.toolTip.SetToolTip(this.track_clocking_curr_core, resources.GetString("track_clocking_curr_core.ToolTip"));
      this.track_clocking_curr_core.Value = 200;
      this.track_clocking_curr_core.Visible = ((bool)(resources.GetObject("track_clocking_curr_core.Visible")));
      this.track_clocking_curr_core.VisibleChanged += new System.EventHandler(this.track_clocking_curr_core_VisibleChanged);
      this.track_clocking_curr_core.Scroll += new System.EventHandler(this.track_clocking_core_Scroll);
      // 
      // label1
      // 
      this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
      this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
      this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
      this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
      this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
      this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
      this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
      this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
      this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
      this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
      this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
      this.label1.Name = "label1";
      this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
      this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
      this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
      this.label1.Text = resources.GetString("label1.Text");
      this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
      this.toolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
      this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
      // 
      // text_clocking_curr_mem
      // 
      this.text_clocking_curr_mem.AccessibleDescription = resources.GetString("text_clocking_curr_mem.AccessibleDescription");
      this.text_clocking_curr_mem.AccessibleName = resources.GetString("text_clocking_curr_mem.AccessibleName");
      this.text_clocking_curr_mem.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_clocking_curr_mem.Anchor")));
      this.text_clocking_curr_mem.AutoSize = ((bool)(resources.GetObject("text_clocking_curr_mem.AutoSize")));
      this.text_clocking_curr_mem.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_clocking_curr_mem.BackgroundImage")));
      this.text_clocking_curr_mem.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.text_clocking_curr_mem.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_clocking_curr_mem.Dock")));
      this.text_clocking_curr_mem.Enabled = ((bool)(resources.GetObject("text_clocking_curr_mem.Enabled")));
      this.text_clocking_curr_mem.Font = ((System.Drawing.Font)(resources.GetObject("text_clocking_curr_mem.Font")));
      this.text_clocking_curr_mem.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_clocking_curr_mem.ImeMode")));
      this.text_clocking_curr_mem.Location = ((System.Drawing.Point)(resources.GetObject("text_clocking_curr_mem.Location")));
      this.text_clocking_curr_mem.MaxLength = ((int)(resources.GetObject("text_clocking_curr_mem.MaxLength")));
      this.text_clocking_curr_mem.Multiline = ((bool)(resources.GetObject("text_clocking_curr_mem.Multiline")));
      this.text_clocking_curr_mem.Name = "text_clocking_curr_mem";
      this.text_clocking_curr_mem.PasswordChar = ((char)(resources.GetObject("text_clocking_curr_mem.PasswordChar")));
      this.text_clocking_curr_mem.ReadOnly = true;
      this.text_clocking_curr_mem.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_clocking_curr_mem.RightToLeft")));
      this.text_clocking_curr_mem.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_clocking_curr_mem.ScrollBars")));
      this.text_clocking_curr_mem.Size = ((System.Drawing.Size)(resources.GetObject("text_clocking_curr_mem.Size")));
      this.text_clocking_curr_mem.TabIndex = ((int)(resources.GetObject("text_clocking_curr_mem.TabIndex")));
      this.text_clocking_curr_mem.TabStop = false;
      this.text_clocking_curr_mem.Text = resources.GetString("text_clocking_curr_mem.Text");
      this.text_clocking_curr_mem.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_clocking_curr_mem.TextAlign")));
      this.toolTip.SetToolTip(this.text_clocking_curr_mem, resources.GetString("text_clocking_curr_mem.ToolTip"));
      this.text_clocking_curr_mem.Visible = ((bool)(resources.GetObject("text_clocking_curr_mem.Visible")));
      this.text_clocking_curr_mem.WordWrap = ((bool)(resources.GetObject("text_clocking_curr_mem.WordWrap")));
      this.text_clocking_curr_mem.TextChanged += new System.EventHandler(this.text_clocking_curr_mem_TextChanged);
      // 
      // button_clocking_refresh
      // 
      this.button_clocking_refresh.AccessibleDescription = resources.GetString("button_clocking_refresh.AccessibleDescription");
      this.button_clocking_refresh.AccessibleName = resources.GetString("button_clocking_refresh.AccessibleName");
      this.button_clocking_refresh.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_clocking_refresh.Anchor")));
      this.button_clocking_refresh.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_clocking_refresh.BackgroundImage")));
      this.button_clocking_refresh.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_clocking_refresh.Dock")));
      this.button_clocking_refresh.Enabled = ((bool)(resources.GetObject("button_clocking_refresh.Enabled")));
      this.button_clocking_refresh.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_clocking_refresh.FlatStyle")));
      this.button_clocking_refresh.Font = ((System.Drawing.Font)(resources.GetObject("button_clocking_refresh.Font")));
      this.button_clocking_refresh.Image = ((System.Drawing.Image)(resources.GetObject("button_clocking_refresh.Image")));
      this.button_clocking_refresh.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_refresh.ImageAlign")));
      this.button_clocking_refresh.ImageIndex = ((int)(resources.GetObject("button_clocking_refresh.ImageIndex")));
      this.button_clocking_refresh.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_clocking_refresh.ImeMode")));
      this.button_clocking_refresh.Location = ((System.Drawing.Point)(resources.GetObject("button_clocking_refresh.Location")));
      this.button_clocking_refresh.Name = "button_clocking_refresh";
      this.button_clocking_refresh.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_clocking_refresh.RightToLeft")));
      this.button_clocking_refresh.Size = ((System.Drawing.Size)(resources.GetObject("button_clocking_refresh.Size")));
      this.button_clocking_refresh.TabIndex = ((int)(resources.GetObject("button_clocking_refresh.TabIndex")));
      this.button_clocking_refresh.Text = resources.GetString("button_clocking_refresh.Text");
      this.button_clocking_refresh.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_refresh.TextAlign")));
      this.toolTip.SetToolTip(this.button_clocking_refresh, resources.GetString("button_clocking_refresh.ToolTip"));
      this.button_clocking_refresh.Visible = ((bool)(resources.GetObject("button_clocking_refresh.Visible")));
      this.button_clocking_refresh.Click += new System.EventHandler(this.button_clocking_refresh_Click);
      // 
      // group_clocking_current_presets
      // 
      this.group_clocking_current_presets.AccessibleDescription = resources.GetString("group_clocking_current_presets.AccessibleDescription");
      this.group_clocking_current_presets.AccessibleName = resources.GetString("group_clocking_current_presets.AccessibleName");
      this.group_clocking_current_presets.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("group_clocking_current_presets.Anchor")));
      this.group_clocking_current_presets.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("group_clocking_current_presets.BackgroundImage")));
      this.group_clocking_current_presets.Controls.Add(this.button_clocking_curr_preFast);
      this.group_clocking_current_presets.Controls.Add(this.button_clocking_curr_preUltra);
      this.group_clocking_current_presets.Controls.Add(this.button_clocking_curr_preNormal);
      this.group_clocking_current_presets.Controls.Add(this.button_clocking_curr_preSlow);
      this.group_clocking_current_presets.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("group_clocking_current_presets.Dock")));
      this.group_clocking_current_presets.Enabled = ((bool)(resources.GetObject("group_clocking_current_presets.Enabled")));
      this.group_clocking_current_presets.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.group_clocking_current_presets.Font = ((System.Drawing.Font)(resources.GetObject("group_clocking_current_presets.Font")));
      this.group_clocking_current_presets.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("group_clocking_current_presets.ImeMode")));
      this.group_clocking_current_presets.Location = ((System.Drawing.Point)(resources.GetObject("group_clocking_current_presets.Location")));
      this.group_clocking_current_presets.Name = "group_clocking_current_presets";
      this.group_clocking_current_presets.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("group_clocking_current_presets.RightToLeft")));
      this.group_clocking_current_presets.Size = ((System.Drawing.Size)(resources.GetObject("group_clocking_current_presets.Size")));
      this.group_clocking_current_presets.TabIndex = ((int)(resources.GetObject("group_clocking_current_presets.TabIndex")));
      this.group_clocking_current_presets.TabStop = false;
      this.group_clocking_current_presets.Text = resources.GetString("group_clocking_current_presets.Text");
      this.toolTip.SetToolTip(this.group_clocking_current_presets, resources.GetString("group_clocking_current_presets.ToolTip"));
      this.group_clocking_current_presets.Visible = ((bool)(resources.GetObject("group_clocking_current_presets.Visible")));
      // 
      // button_clocking_curr_preFast
      // 
      this.button_clocking_curr_preFast.AccessibleDescription = resources.GetString("button_clocking_curr_preFast.AccessibleDescription");
      this.button_clocking_curr_preFast.AccessibleName = resources.GetString("button_clocking_curr_preFast.AccessibleName");
      this.button_clocking_curr_preFast.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_clocking_curr_preFast.Anchor")));
      this.button_clocking_curr_preFast.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_clocking_curr_preFast.BackgroundImage")));
      this.button_clocking_curr_preFast.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_clocking_curr_preFast.Dock")));
      this.button_clocking_curr_preFast.Enabled = ((bool)(resources.GetObject("button_clocking_curr_preFast.Enabled")));
      this.button_clocking_curr_preFast.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_clocking_curr_preFast.FlatStyle")));
      this.button_clocking_curr_preFast.Font = ((System.Drawing.Font)(resources.GetObject("button_clocking_curr_preFast.Font")));
      this.button_clocking_curr_preFast.Image = ((System.Drawing.Image)(resources.GetObject("button_clocking_curr_preFast.Image")));
      this.button_clocking_curr_preFast.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_curr_preFast.ImageAlign")));
      this.button_clocking_curr_preFast.ImageIndex = ((int)(resources.GetObject("button_clocking_curr_preFast.ImageIndex")));
      this.button_clocking_curr_preFast.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_clocking_curr_preFast.ImeMode")));
      this.button_clocking_curr_preFast.Location = ((System.Drawing.Point)(resources.GetObject("button_clocking_curr_preFast.Location")));
      this.button_clocking_curr_preFast.Name = "button_clocking_curr_preFast";
      this.button_clocking_curr_preFast.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_clocking_curr_preFast.RightToLeft")));
      this.button_clocking_curr_preFast.Size = ((System.Drawing.Size)(resources.GetObject("button_clocking_curr_preFast.Size")));
      this.button_clocking_curr_preFast.TabIndex = ((int)(resources.GetObject("button_clocking_curr_preFast.TabIndex")));
      this.button_clocking_curr_preFast.Tag = "3";
      this.button_clocking_curr_preFast.Text = resources.GetString("button_clocking_curr_preFast.Text");
      this.button_clocking_curr_preFast.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_curr_preFast.TextAlign")));
      this.toolTip.SetToolTip(this.button_clocking_curr_preFast, resources.GetString("button_clocking_curr_preFast.ToolTip"));
      this.button_clocking_curr_preFast.Visible = ((bool)(resources.GetObject("button_clocking_curr_preFast.Visible")));
      this.button_clocking_curr_preFast.Click += new System.EventHandler(this.button_clocking_curr_preFast_Click);
      // 
      // button_clocking_curr_preUltra
      // 
      this.button_clocking_curr_preUltra.AccessibleDescription = resources.GetString("button_clocking_curr_preUltra.AccessibleDescription");
      this.button_clocking_curr_preUltra.AccessibleName = resources.GetString("button_clocking_curr_preUltra.AccessibleName");
      this.button_clocking_curr_preUltra.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_clocking_curr_preUltra.Anchor")));
      this.button_clocking_curr_preUltra.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_clocking_curr_preUltra.BackgroundImage")));
      this.button_clocking_curr_preUltra.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_clocking_curr_preUltra.Dock")));
      this.button_clocking_curr_preUltra.Enabled = ((bool)(resources.GetObject("button_clocking_curr_preUltra.Enabled")));
      this.button_clocking_curr_preUltra.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_clocking_curr_preUltra.FlatStyle")));
      this.button_clocking_curr_preUltra.Font = ((System.Drawing.Font)(resources.GetObject("button_clocking_curr_preUltra.Font")));
      this.button_clocking_curr_preUltra.Image = ((System.Drawing.Image)(resources.GetObject("button_clocking_curr_preUltra.Image")));
      this.button_clocking_curr_preUltra.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_curr_preUltra.ImageAlign")));
      this.button_clocking_curr_preUltra.ImageIndex = ((int)(resources.GetObject("button_clocking_curr_preUltra.ImageIndex")));
      this.button_clocking_curr_preUltra.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_clocking_curr_preUltra.ImeMode")));
      this.button_clocking_curr_preUltra.Location = ((System.Drawing.Point)(resources.GetObject("button_clocking_curr_preUltra.Location")));
      this.button_clocking_curr_preUltra.Name = "button_clocking_curr_preUltra";
      this.button_clocking_curr_preUltra.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_clocking_curr_preUltra.RightToLeft")));
      this.button_clocking_curr_preUltra.Size = ((System.Drawing.Size)(resources.GetObject("button_clocking_curr_preUltra.Size")));
      this.button_clocking_curr_preUltra.TabIndex = ((int)(resources.GetObject("button_clocking_curr_preUltra.TabIndex")));
      this.button_clocking_curr_preUltra.Tag = "4";
      this.button_clocking_curr_preUltra.Text = resources.GetString("button_clocking_curr_preUltra.Text");
      this.button_clocking_curr_preUltra.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_curr_preUltra.TextAlign")));
      this.toolTip.SetToolTip(this.button_clocking_curr_preUltra, resources.GetString("button_clocking_curr_preUltra.ToolTip"));
      this.button_clocking_curr_preUltra.Visible = ((bool)(resources.GetObject("button_clocking_curr_preUltra.Visible")));
      this.button_clocking_curr_preUltra.Click += new System.EventHandler(this.button_clocking_curr_preUltra_Click);
      // 
      // button_clocking_curr_preNormal
      // 
      this.button_clocking_curr_preNormal.AccessibleDescription = resources.GetString("button_clocking_curr_preNormal.AccessibleDescription");
      this.button_clocking_curr_preNormal.AccessibleName = resources.GetString("button_clocking_curr_preNormal.AccessibleName");
      this.button_clocking_curr_preNormal.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_clocking_curr_preNormal.Anchor")));
      this.button_clocking_curr_preNormal.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_clocking_curr_preNormal.BackgroundImage")));
      this.button_clocking_curr_preNormal.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_clocking_curr_preNormal.Dock")));
      this.button_clocking_curr_preNormal.Enabled = ((bool)(resources.GetObject("button_clocking_curr_preNormal.Enabled")));
      this.button_clocking_curr_preNormal.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_clocking_curr_preNormal.FlatStyle")));
      this.button_clocking_curr_preNormal.Font = ((System.Drawing.Font)(resources.GetObject("button_clocking_curr_preNormal.Font")));
      this.button_clocking_curr_preNormal.Image = ((System.Drawing.Image)(resources.GetObject("button_clocking_curr_preNormal.Image")));
      this.button_clocking_curr_preNormal.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_curr_preNormal.ImageAlign")));
      this.button_clocking_curr_preNormal.ImageIndex = ((int)(resources.GetObject("button_clocking_curr_preNormal.ImageIndex")));
      this.button_clocking_curr_preNormal.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_clocking_curr_preNormal.ImeMode")));
      this.button_clocking_curr_preNormal.Location = ((System.Drawing.Point)(resources.GetObject("button_clocking_curr_preNormal.Location")));
      this.button_clocking_curr_preNormal.Name = "button_clocking_curr_preNormal";
      this.button_clocking_curr_preNormal.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_clocking_curr_preNormal.RightToLeft")));
      this.button_clocking_curr_preNormal.Size = ((System.Drawing.Size)(resources.GetObject("button_clocking_curr_preNormal.Size")));
      this.button_clocking_curr_preNormal.TabIndex = ((int)(resources.GetObject("button_clocking_curr_preNormal.TabIndex")));
      this.button_clocking_curr_preNormal.Tag = "2";
      this.button_clocking_curr_preNormal.Text = resources.GetString("button_clocking_curr_preNormal.Text");
      this.button_clocking_curr_preNormal.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_curr_preNormal.TextAlign")));
      this.toolTip.SetToolTip(this.button_clocking_curr_preNormal, resources.GetString("button_clocking_curr_preNormal.ToolTip"));
      this.button_clocking_curr_preNormal.Visible = ((bool)(resources.GetObject("button_clocking_curr_preNormal.Visible")));
      this.button_clocking_curr_preNormal.Click += new System.EventHandler(this.button_clocking_curr_preNormal_Click);
      // 
      // button_clocking_curr_preSlow
      // 
      this.button_clocking_curr_preSlow.AccessibleDescription = resources.GetString("button_clocking_curr_preSlow.AccessibleDescription");
      this.button_clocking_curr_preSlow.AccessibleName = resources.GetString("button_clocking_curr_preSlow.AccessibleName");
      this.button_clocking_curr_preSlow.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_clocking_curr_preSlow.Anchor")));
      this.button_clocking_curr_preSlow.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_clocking_curr_preSlow.BackgroundImage")));
      this.button_clocking_curr_preSlow.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_clocking_curr_preSlow.Dock")));
      this.button_clocking_curr_preSlow.Enabled = ((bool)(resources.GetObject("button_clocking_curr_preSlow.Enabled")));
      this.button_clocking_curr_preSlow.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_clocking_curr_preSlow.FlatStyle")));
      this.button_clocking_curr_preSlow.Font = ((System.Drawing.Font)(resources.GetObject("button_clocking_curr_preSlow.Font")));
      this.button_clocking_curr_preSlow.Image = ((System.Drawing.Image)(resources.GetObject("button_clocking_curr_preSlow.Image")));
      this.button_clocking_curr_preSlow.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_curr_preSlow.ImageAlign")));
      this.button_clocking_curr_preSlow.ImageIndex = ((int)(resources.GetObject("button_clocking_curr_preSlow.ImageIndex")));
      this.button_clocking_curr_preSlow.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_clocking_curr_preSlow.ImeMode")));
      this.button_clocking_curr_preSlow.Location = ((System.Drawing.Point)(resources.GetObject("button_clocking_curr_preSlow.Location")));
      this.button_clocking_curr_preSlow.Name = "button_clocking_curr_preSlow";
      this.button_clocking_curr_preSlow.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_clocking_curr_preSlow.RightToLeft")));
      this.button_clocking_curr_preSlow.Size = ((System.Drawing.Size)(resources.GetObject("button_clocking_curr_preSlow.Size")));
      this.button_clocking_curr_preSlow.TabIndex = ((int)(resources.GetObject("button_clocking_curr_preSlow.TabIndex")));
      this.button_clocking_curr_preSlow.Tag = "";
      this.button_clocking_curr_preSlow.Text = resources.GetString("button_clocking_curr_preSlow.Text");
      this.button_clocking_curr_preSlow.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_curr_preSlow.TextAlign")));
      this.toolTip.SetToolTip(this.button_clocking_curr_preSlow, resources.GetString("button_clocking_curr_preSlow.ToolTip"));
      this.button_clocking_curr_preSlow.Visible = ((bool)(resources.GetObject("button_clocking_curr_preSlow.Visible")));
      this.button_clocking_curr_preSlow.Click += new System.EventHandler(this.button_clocking_curr_preSlow_Click);
      // 
      // splitter_clocking
      // 
      this.splitter_clocking.AccessibleDescription = resources.GetString("splitter_clocking.AccessibleDescription");
      this.splitter_clocking.AccessibleName = resources.GetString("splitter_clocking.AccessibleName");
      this.splitter_clocking.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("splitter_clocking.Anchor")));
      this.splitter_clocking.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splitter_clocking.BackgroundImage")));
      this.splitter_clocking.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("splitter_clocking.Dock")));
      this.splitter_clocking.Enabled = ((bool)(resources.GetObject("splitter_clocking.Enabled")));
      this.splitter_clocking.Font = ((System.Drawing.Font)(resources.GetObject("splitter_clocking.Font")));
      this.splitter_clocking.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("splitter_clocking.ImeMode")));
      this.splitter_clocking.Location = ((System.Drawing.Point)(resources.GetObject("splitter_clocking.Location")));
      this.splitter_clocking.MinExtra = ((int)(resources.GetObject("splitter_clocking.MinExtra")));
      this.splitter_clocking.MinSize = ((int)(resources.GetObject("splitter_clocking.MinSize")));
      this.splitter_clocking.Name = "splitter_clocking";
      this.splitter_clocking.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("splitter_clocking.RightToLeft")));
      this.splitter_clocking.Size = ((System.Drawing.Size)(resources.GetObject("splitter_clocking.Size")));
      this.splitter_clocking.TabIndex = ((int)(resources.GetObject("splitter_clocking.TabIndex")));
      this.splitter_clocking.TabStop = false;
      this.toolTip.SetToolTip(this.splitter_clocking, resources.GetString("splitter_clocking.ToolTip"));
      this.splitter_clocking.Visible = ((bool)(resources.GetObject("splitter_clocking.Visible")));
      // 
      // group_clocking_prof
      // 
      this.group_clocking_prof.AccessibleDescription = resources.GetString("group_clocking_prof.AccessibleDescription");
      this.group_clocking_prof.AccessibleName = resources.GetString("group_clocking_prof.AccessibleName");
      this.group_clocking_prof.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("group_clocking_prof.Anchor")));
      this.group_clocking_prof.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("group_clocking_prof.BackgroundImage")));
      this.group_clocking_prof.Controls.Add(this.panel_clocking_prof_clocks);
      this.group_clocking_prof.Controls.Add(this.label5);
      this.group_clocking_prof.Controls.Add(this.combo_clocking_prof_presets);
      this.group_clocking_prof.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("group_clocking_prof.Dock")));
      this.group_clocking_prof.Enabled = ((bool)(resources.GetObject("group_clocking_prof.Enabled")));
      this.group_clocking_prof.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.group_clocking_prof.Font = ((System.Drawing.Font)(resources.GetObject("group_clocking_prof.Font")));
      this.group_clocking_prof.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("group_clocking_prof.ImeMode")));
      this.group_clocking_prof.Location = ((System.Drawing.Point)(resources.GetObject("group_clocking_prof.Location")));
      this.group_clocking_prof.Name = "group_clocking_prof";
      this.group_clocking_prof.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("group_clocking_prof.RightToLeft")));
      this.group_clocking_prof.Size = ((System.Drawing.Size)(resources.GetObject("group_clocking_prof.Size")));
      this.group_clocking_prof.TabIndex = ((int)(resources.GetObject("group_clocking_prof.TabIndex")));
      this.group_clocking_prof.TabStop = false;
      this.group_clocking_prof.Text = resources.GetString("group_clocking_prof.Text");
      this.toolTip.SetToolTip(this.group_clocking_prof, resources.GetString("group_clocking_prof.ToolTip"));
      this.group_clocking_prof.Visible = ((bool)(resources.GetObject("group_clocking_prof.Visible")));
      // 
      // panel_clocking_prof_clocks
      // 
      this.panel_clocking_prof_clocks.AccessibleDescription = resources.GetString("panel_clocking_prof_clocks.AccessibleDescription");
      this.panel_clocking_prof_clocks.AccessibleName = resources.GetString("panel_clocking_prof_clocks.AccessibleName");
      this.panel_clocking_prof_clocks.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panel_clocking_prof_clocks.Anchor")));
      this.panel_clocking_prof_clocks.AutoScroll = ((bool)(resources.GetObject("panel_clocking_prof_clocks.AutoScroll")));
      this.panel_clocking_prof_clocks.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panel_clocking_prof_clocks.AutoScrollMargin")));
      this.panel_clocking_prof_clocks.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panel_clocking_prof_clocks.AutoScrollMinSize")));
      this.panel_clocking_prof_clocks.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel_clocking_prof_clocks.BackgroundImage")));
      this.panel_clocking_prof_clocks.Controls.Add(this.check_clocking_prof_mem);
      this.panel_clocking_prof_clocks.Controls.Add(this.text_clocking_prof_mem);
      this.panel_clocking_prof_clocks.Controls.Add(this.track_clocking_prof_mem);
      this.panel_clocking_prof_clocks.Controls.Add(this.text_clocking_prof_core);
      this.panel_clocking_prof_clocks.Controls.Add(this.label_clocking_prof_mem);
      this.panel_clocking_prof_clocks.Controls.Add(this.track_clocking_prof_core);
      this.panel_clocking_prof_clocks.Controls.Add(this.check_clocking_prof_core);
      this.panel_clocking_prof_clocks.Controls.Add(this.button_clocking_disable);
      this.panel_clocking_prof_clocks.Controls.Add(this.label_clocking_prof_core);
      this.panel_clocking_prof_clocks.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panel_clocking_prof_clocks.Dock")));
      this.panel_clocking_prof_clocks.Enabled = ((bool)(resources.GetObject("panel_clocking_prof_clocks.Enabled")));
      this.panel_clocking_prof_clocks.Font = ((System.Drawing.Font)(resources.GetObject("panel_clocking_prof_clocks.Font")));
      this.panel_clocking_prof_clocks.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panel_clocking_prof_clocks.ImeMode")));
      this.panel_clocking_prof_clocks.Location = ((System.Drawing.Point)(resources.GetObject("panel_clocking_prof_clocks.Location")));
      this.panel_clocking_prof_clocks.Name = "panel_clocking_prof_clocks";
      this.panel_clocking_prof_clocks.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panel_clocking_prof_clocks.RightToLeft")));
      this.panel_clocking_prof_clocks.Size = ((System.Drawing.Size)(resources.GetObject("panel_clocking_prof_clocks.Size")));
      this.panel_clocking_prof_clocks.TabIndex = ((int)(resources.GetObject("panel_clocking_prof_clocks.TabIndex")));
      this.panel_clocking_prof_clocks.Text = resources.GetString("panel_clocking_prof_clocks.Text");
      this.toolTip.SetToolTip(this.panel_clocking_prof_clocks, resources.GetString("panel_clocking_prof_clocks.ToolTip"));
      this.panel_clocking_prof_clocks.Visible = ((bool)(resources.GetObject("panel_clocking_prof_clocks.Visible")));
      // 
      // check_clocking_prof_mem
      // 
      this.check_clocking_prof_mem.AccessibleDescription = resources.GetString("check_clocking_prof_mem.AccessibleDescription");
      this.check_clocking_prof_mem.AccessibleName = resources.GetString("check_clocking_prof_mem.AccessibleName");
      this.check_clocking_prof_mem.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_clocking_prof_mem.Anchor")));
      this.check_clocking_prof_mem.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_clocking_prof_mem.Appearance")));
      this.check_clocking_prof_mem.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_clocking_prof_mem.BackgroundImage")));
      this.check_clocking_prof_mem.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clocking_prof_mem.CheckAlign")));
      this.check_clocking_prof_mem.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_clocking_prof_mem.Dock")));
      this.check_clocking_prof_mem.Enabled = ((bool)(resources.GetObject("check_clocking_prof_mem.Enabled")));
      this.check_clocking_prof_mem.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_clocking_prof_mem.FlatStyle")));
      this.check_clocking_prof_mem.Font = ((System.Drawing.Font)(resources.GetObject("check_clocking_prof_mem.Font")));
      this.check_clocking_prof_mem.Image = ((System.Drawing.Image)(resources.GetObject("check_clocking_prof_mem.Image")));
      this.check_clocking_prof_mem.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clocking_prof_mem.ImageAlign")));
      this.check_clocking_prof_mem.ImageIndex = ((int)(resources.GetObject("check_clocking_prof_mem.ImageIndex")));
      this.check_clocking_prof_mem.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_clocking_prof_mem.ImeMode")));
      this.check_clocking_prof_mem.Location = ((System.Drawing.Point)(resources.GetObject("check_clocking_prof_mem.Location")));
      this.check_clocking_prof_mem.Name = "check_clocking_prof_mem";
      this.check_clocking_prof_mem.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_clocking_prof_mem.RightToLeft")));
      this.check_clocking_prof_mem.Size = ((System.Drawing.Size)(resources.GetObject("check_clocking_prof_mem.Size")));
      this.check_clocking_prof_mem.TabIndex = ((int)(resources.GetObject("check_clocking_prof_mem.TabIndex")));
      this.check_clocking_prof_mem.Text = resources.GetString("check_clocking_prof_mem.Text");
      this.check_clocking_prof_mem.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clocking_prof_mem.TextAlign")));
      this.toolTip.SetToolTip(this.check_clocking_prof_mem, resources.GetString("check_clocking_prof_mem.ToolTip"));
      this.check_clocking_prof_mem.Visible = ((bool)(resources.GetObject("check_clocking_prof_mem.Visible")));
      this.check_clocking_prof_mem.CheckedChanged += new System.EventHandler(this.check_clocking_prof_mem_CheckedChanged);
      // 
      // text_clocking_prof_mem
      // 
      this.text_clocking_prof_mem.AccessibleDescription = resources.GetString("text_clocking_prof_mem.AccessibleDescription");
      this.text_clocking_prof_mem.AccessibleName = resources.GetString("text_clocking_prof_mem.AccessibleName");
      this.text_clocking_prof_mem.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_clocking_prof_mem.Anchor")));
      this.text_clocking_prof_mem.AutoSize = ((bool)(resources.GetObject("text_clocking_prof_mem.AutoSize")));
      this.text_clocking_prof_mem.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_clocking_prof_mem.BackgroundImage")));
      this.text_clocking_prof_mem.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.text_clocking_prof_mem.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_clocking_prof_mem.Dock")));
      this.text_clocking_prof_mem.Enabled = ((bool)(resources.GetObject("text_clocking_prof_mem.Enabled")));
      this.text_clocking_prof_mem.Font = ((System.Drawing.Font)(resources.GetObject("text_clocking_prof_mem.Font")));
      this.text_clocking_prof_mem.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_clocking_prof_mem.ImeMode")));
      this.text_clocking_prof_mem.Location = ((System.Drawing.Point)(resources.GetObject("text_clocking_prof_mem.Location")));
      this.text_clocking_prof_mem.MaxLength = ((int)(resources.GetObject("text_clocking_prof_mem.MaxLength")));
      this.text_clocking_prof_mem.Multiline = ((bool)(resources.GetObject("text_clocking_prof_mem.Multiline")));
      this.text_clocking_prof_mem.Name = "text_clocking_prof_mem";
      this.text_clocking_prof_mem.PasswordChar = ((char)(resources.GetObject("text_clocking_prof_mem.PasswordChar")));
      this.text_clocking_prof_mem.ReadOnly = true;
      this.text_clocking_prof_mem.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_clocking_prof_mem.RightToLeft")));
      this.text_clocking_prof_mem.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_clocking_prof_mem.ScrollBars")));
      this.text_clocking_prof_mem.Size = ((System.Drawing.Size)(resources.GetObject("text_clocking_prof_mem.Size")));
      this.text_clocking_prof_mem.TabIndex = ((int)(resources.GetObject("text_clocking_prof_mem.TabIndex")));
      this.text_clocking_prof_mem.TabStop = false;
      this.text_clocking_prof_mem.Text = resources.GetString("text_clocking_prof_mem.Text");
      this.text_clocking_prof_mem.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_clocking_prof_mem.TextAlign")));
      this.toolTip.SetToolTip(this.text_clocking_prof_mem, resources.GetString("text_clocking_prof_mem.ToolTip"));
      this.text_clocking_prof_mem.Visible = ((bool)(resources.GetObject("text_clocking_prof_mem.Visible")));
      this.text_clocking_prof_mem.WordWrap = ((bool)(resources.GetObject("text_clocking_prof_mem.WordWrap")));
      this.text_clocking_prof_mem.TextChanged += new System.EventHandler(this.text_clocking_prof_mem_TextChanged);
      // 
      // track_clocking_prof_mem
      // 
      this.track_clocking_prof_mem.AccessibleDescription = resources.GetString("track_clocking_prof_mem.AccessibleDescription");
      this.track_clocking_prof_mem.AccessibleName = resources.GetString("track_clocking_prof_mem.AccessibleName");
      this.track_clocking_prof_mem.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("track_clocking_prof_mem.Anchor")));
      this.track_clocking_prof_mem.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("track_clocking_prof_mem.BackgroundImage")));
      this.track_clocking_prof_mem.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("track_clocking_prof_mem.Dock")));
      this.track_clocking_prof_mem.Enabled = ((bool)(resources.GetObject("track_clocking_prof_mem.Enabled")));
      this.track_clocking_prof_mem.Font = ((System.Drawing.Font)(resources.GetObject("track_clocking_prof_mem.Font")));
      this.track_clocking_prof_mem.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("track_clocking_prof_mem.ImeMode")));
      this.track_clocking_prof_mem.Location = ((System.Drawing.Point)(resources.GetObject("track_clocking_prof_mem.Location")));
      this.track_clocking_prof_mem.Maximum = 320;
      this.track_clocking_prof_mem.Minimum = 200;
      this.track_clocking_prof_mem.Name = "track_clocking_prof_mem";
      this.track_clocking_prof_mem.Orientation = ((System.Windows.Forms.Orientation)(resources.GetObject("track_clocking_prof_mem.Orientation")));
      this.track_clocking_prof_mem.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("track_clocking_prof_mem.RightToLeft")));
      this.track_clocking_prof_mem.Size = ((System.Drawing.Size)(resources.GetObject("track_clocking_prof_mem.Size")));
      this.track_clocking_prof_mem.TabIndex = ((int)(resources.GetObject("track_clocking_prof_mem.TabIndex")));
      this.track_clocking_prof_mem.Text = resources.GetString("track_clocking_prof_mem.Text");
      this.track_clocking_prof_mem.TickFrequency = 10;
      this.track_clocking_prof_mem.TickStyle = System.Windows.Forms.TickStyle.None;
      this.toolTip.SetToolTip(this.track_clocking_prof_mem, resources.GetString("track_clocking_prof_mem.ToolTip"));
      this.track_clocking_prof_mem.Value = 200;
      this.track_clocking_prof_mem.Visible = ((bool)(resources.GetObject("track_clocking_prof_mem.Visible")));
      this.track_clocking_prof_mem.Scroll += new System.EventHandler(this.track_clocking_prof_mem_ValueChanged);
      // 
      // text_clocking_prof_core
      // 
      this.text_clocking_prof_core.AccessibleDescription = resources.GetString("text_clocking_prof_core.AccessibleDescription");
      this.text_clocking_prof_core.AccessibleName = resources.GetString("text_clocking_prof_core.AccessibleName");
      this.text_clocking_prof_core.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_clocking_prof_core.Anchor")));
      this.text_clocking_prof_core.AutoSize = ((bool)(resources.GetObject("text_clocking_prof_core.AutoSize")));
      this.text_clocking_prof_core.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_clocking_prof_core.BackgroundImage")));
      this.text_clocking_prof_core.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.text_clocking_prof_core.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_clocking_prof_core.Dock")));
      this.text_clocking_prof_core.Enabled = ((bool)(resources.GetObject("text_clocking_prof_core.Enabled")));
      this.text_clocking_prof_core.Font = ((System.Drawing.Font)(resources.GetObject("text_clocking_prof_core.Font")));
      this.text_clocking_prof_core.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_clocking_prof_core.ImeMode")));
      this.text_clocking_prof_core.Location = ((System.Drawing.Point)(resources.GetObject("text_clocking_prof_core.Location")));
      this.text_clocking_prof_core.MaxLength = ((int)(resources.GetObject("text_clocking_prof_core.MaxLength")));
      this.text_clocking_prof_core.Multiline = ((bool)(resources.GetObject("text_clocking_prof_core.Multiline")));
      this.text_clocking_prof_core.Name = "text_clocking_prof_core";
      this.text_clocking_prof_core.PasswordChar = ((char)(resources.GetObject("text_clocking_prof_core.PasswordChar")));
      this.text_clocking_prof_core.ReadOnly = true;
      this.text_clocking_prof_core.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_clocking_prof_core.RightToLeft")));
      this.text_clocking_prof_core.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_clocking_prof_core.ScrollBars")));
      this.text_clocking_prof_core.Size = ((System.Drawing.Size)(resources.GetObject("text_clocking_prof_core.Size")));
      this.text_clocking_prof_core.TabIndex = ((int)(resources.GetObject("text_clocking_prof_core.TabIndex")));
      this.text_clocking_prof_core.TabStop = false;
      this.text_clocking_prof_core.Text = resources.GetString("text_clocking_prof_core.Text");
      this.text_clocking_prof_core.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_clocking_prof_core.TextAlign")));
      this.toolTip.SetToolTip(this.text_clocking_prof_core, resources.GetString("text_clocking_prof_core.ToolTip"));
      this.text_clocking_prof_core.Visible = ((bool)(resources.GetObject("text_clocking_prof_core.Visible")));
      this.text_clocking_prof_core.WordWrap = ((bool)(resources.GetObject("text_clocking_prof_core.WordWrap")));
      this.text_clocking_prof_core.TextChanged += new System.EventHandler(this.text_clocking_prof_core_TextChanged);
      // 
      // label_clocking_prof_mem
      // 
      this.label_clocking_prof_mem.AccessibleDescription = resources.GetString("label_clocking_prof_mem.AccessibleDescription");
      this.label_clocking_prof_mem.AccessibleName = resources.GetString("label_clocking_prof_mem.AccessibleName");
      this.label_clocking_prof_mem.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_clocking_prof_mem.Anchor")));
      this.label_clocking_prof_mem.AutoSize = ((bool)(resources.GetObject("label_clocking_prof_mem.AutoSize")));
      this.label_clocking_prof_mem.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_clocking_prof_mem.Dock")));
      this.label_clocking_prof_mem.Enabled = ((bool)(resources.GetObject("label_clocking_prof_mem.Enabled")));
      this.label_clocking_prof_mem.Font = ((System.Drawing.Font)(resources.GetObject("label_clocking_prof_mem.Font")));
      this.label_clocking_prof_mem.Image = ((System.Drawing.Image)(resources.GetObject("label_clocking_prof_mem.Image")));
      this.label_clocking_prof_mem.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_clocking_prof_mem.ImageAlign")));
      this.label_clocking_prof_mem.ImageIndex = ((int)(resources.GetObject("label_clocking_prof_mem.ImageIndex")));
      this.label_clocking_prof_mem.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_clocking_prof_mem.ImeMode")));
      this.label_clocking_prof_mem.Location = ((System.Drawing.Point)(resources.GetObject("label_clocking_prof_mem.Location")));
      this.label_clocking_prof_mem.Name = "label_clocking_prof_mem";
      this.label_clocking_prof_mem.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_clocking_prof_mem.RightToLeft")));
      this.label_clocking_prof_mem.Size = ((System.Drawing.Size)(resources.GetObject("label_clocking_prof_mem.Size")));
      this.label_clocking_prof_mem.TabIndex = ((int)(resources.GetObject("label_clocking_prof_mem.TabIndex")));
      this.label_clocking_prof_mem.Text = resources.GetString("label_clocking_prof_mem.Text");
      this.label_clocking_prof_mem.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_clocking_prof_mem.TextAlign")));
      this.toolTip.SetToolTip(this.label_clocking_prof_mem, resources.GetString("label_clocking_prof_mem.ToolTip"));
      this.label_clocking_prof_mem.Visible = ((bool)(resources.GetObject("label_clocking_prof_mem.Visible")));
      // 
      // track_clocking_prof_core
      // 
      this.track_clocking_prof_core.AccessibleDescription = resources.GetString("track_clocking_prof_core.AccessibleDescription");
      this.track_clocking_prof_core.AccessibleName = resources.GetString("track_clocking_prof_core.AccessibleName");
      this.track_clocking_prof_core.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("track_clocking_prof_core.Anchor")));
      this.track_clocking_prof_core.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("track_clocking_prof_core.BackgroundImage")));
      this.track_clocking_prof_core.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("track_clocking_prof_core.Dock")));
      this.track_clocking_prof_core.Enabled = ((bool)(resources.GetObject("track_clocking_prof_core.Enabled")));
      this.track_clocking_prof_core.Font = ((System.Drawing.Font)(resources.GetObject("track_clocking_prof_core.Font")));
      this.track_clocking_prof_core.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("track_clocking_prof_core.ImeMode")));
      this.track_clocking_prof_core.Location = ((System.Drawing.Point)(resources.GetObject("track_clocking_prof_core.Location")));
      this.track_clocking_prof_core.Maximum = 400;
      this.track_clocking_prof_core.Minimum = 200;
      this.track_clocking_prof_core.Name = "track_clocking_prof_core";
      this.track_clocking_prof_core.Orientation = ((System.Windows.Forms.Orientation)(resources.GetObject("track_clocking_prof_core.Orientation")));
      this.track_clocking_prof_core.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("track_clocking_prof_core.RightToLeft")));
      this.track_clocking_prof_core.Size = ((System.Drawing.Size)(resources.GetObject("track_clocking_prof_core.Size")));
      this.track_clocking_prof_core.TabIndex = ((int)(resources.GetObject("track_clocking_prof_core.TabIndex")));
      this.track_clocking_prof_core.Text = resources.GetString("track_clocking_prof_core.Text");
      this.track_clocking_prof_core.TickFrequency = 10;
      this.track_clocking_prof_core.TickStyle = System.Windows.Forms.TickStyle.None;
      this.toolTip.SetToolTip(this.track_clocking_prof_core, resources.GetString("track_clocking_prof_core.ToolTip"));
      this.track_clocking_prof_core.Value = 200;
      this.track_clocking_prof_core.Visible = ((bool)(resources.GetObject("track_clocking_prof_core.Visible")));
      this.track_clocking_prof_core.Scroll += new System.EventHandler(this.track_clocking_prof_core_ValueChanged);
      // 
      // check_clocking_prof_core
      // 
      this.check_clocking_prof_core.AccessibleDescription = resources.GetString("check_clocking_prof_core.AccessibleDescription");
      this.check_clocking_prof_core.AccessibleName = resources.GetString("check_clocking_prof_core.AccessibleName");
      this.check_clocking_prof_core.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_clocking_prof_core.Anchor")));
      this.check_clocking_prof_core.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_clocking_prof_core.Appearance")));
      this.check_clocking_prof_core.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_clocking_prof_core.BackgroundImage")));
      this.check_clocking_prof_core.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clocking_prof_core.CheckAlign")));
      this.check_clocking_prof_core.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_clocking_prof_core.Dock")));
      this.check_clocking_prof_core.Enabled = ((bool)(resources.GetObject("check_clocking_prof_core.Enabled")));
      this.check_clocking_prof_core.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_clocking_prof_core.FlatStyle")));
      this.check_clocking_prof_core.Font = ((System.Drawing.Font)(resources.GetObject("check_clocking_prof_core.Font")));
      this.check_clocking_prof_core.Image = ((System.Drawing.Image)(resources.GetObject("check_clocking_prof_core.Image")));
      this.check_clocking_prof_core.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clocking_prof_core.ImageAlign")));
      this.check_clocking_prof_core.ImageIndex = ((int)(resources.GetObject("check_clocking_prof_core.ImageIndex")));
      this.check_clocking_prof_core.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_clocking_prof_core.ImeMode")));
      this.check_clocking_prof_core.Location = ((System.Drawing.Point)(resources.GetObject("check_clocking_prof_core.Location")));
      this.check_clocking_prof_core.Name = "check_clocking_prof_core";
      this.check_clocking_prof_core.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_clocking_prof_core.RightToLeft")));
      this.check_clocking_prof_core.Size = ((System.Drawing.Size)(resources.GetObject("check_clocking_prof_core.Size")));
      this.check_clocking_prof_core.TabIndex = ((int)(resources.GetObject("check_clocking_prof_core.TabIndex")));
      this.check_clocking_prof_core.Text = resources.GetString("check_clocking_prof_core.Text");
      this.check_clocking_prof_core.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clocking_prof_core.TextAlign")));
      this.toolTip.SetToolTip(this.check_clocking_prof_core, resources.GetString("check_clocking_prof_core.ToolTip"));
      this.check_clocking_prof_core.Visible = ((bool)(resources.GetObject("check_clocking_prof_core.Visible")));
      this.check_clocking_prof_core.CheckedChanged += new System.EventHandler(this.check_clocking_prof_core_CheckedChanged);
      // 
      // button_clocking_disable
      // 
      this.button_clocking_disable.AccessibleDescription = resources.GetString("button_clocking_disable.AccessibleDescription");
      this.button_clocking_disable.AccessibleName = resources.GetString("button_clocking_disable.AccessibleName");
      this.button_clocking_disable.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_clocking_disable.Anchor")));
      this.button_clocking_disable.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_clocking_disable.BackgroundImage")));
      this.button_clocking_disable.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_clocking_disable.Dock")));
      this.button_clocking_disable.Enabled = ((bool)(resources.GetObject("button_clocking_disable.Enabled")));
      this.button_clocking_disable.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_clocking_disable.FlatStyle")));
      this.button_clocking_disable.Font = ((System.Drawing.Font)(resources.GetObject("button_clocking_disable.Font")));
      this.button_clocking_disable.Image = ((System.Drawing.Image)(resources.GetObject("button_clocking_disable.Image")));
      this.button_clocking_disable.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_disable.ImageAlign")));
      this.button_clocking_disable.ImageIndex = ((int)(resources.GetObject("button_clocking_disable.ImageIndex")));
      this.button_clocking_disable.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_clocking_disable.ImeMode")));
      this.button_clocking_disable.Location = ((System.Drawing.Point)(resources.GetObject("button_clocking_disable.Location")));
      this.button_clocking_disable.Name = "button_clocking_disable";
      this.button_clocking_disable.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_clocking_disable.RightToLeft")));
      this.button_clocking_disable.Size = ((System.Drawing.Size)(resources.GetObject("button_clocking_disable.Size")));
      this.button_clocking_disable.TabIndex = ((int)(resources.GetObject("button_clocking_disable.TabIndex")));
      this.button_clocking_disable.Text = resources.GetString("button_clocking_disable.Text");
      this.button_clocking_disable.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_clocking_disable.TextAlign")));
      this.toolTip.SetToolTip(this.button_clocking_disable, resources.GetString("button_clocking_disable.ToolTip"));
      this.button_clocking_disable.Visible = ((bool)(resources.GetObject("button_clocking_disable.Visible")));
      this.button_clocking_disable.Click += new System.EventHandler(this.button_clocking_disable_Click);
      // 
      // label_clocking_prof_core
      // 
      this.label_clocking_prof_core.AccessibleDescription = resources.GetString("label_clocking_prof_core.AccessibleDescription");
      this.label_clocking_prof_core.AccessibleName = resources.GetString("label_clocking_prof_core.AccessibleName");
      this.label_clocking_prof_core.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_clocking_prof_core.Anchor")));
      this.label_clocking_prof_core.AutoSize = ((bool)(resources.GetObject("label_clocking_prof_core.AutoSize")));
      this.label_clocking_prof_core.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_clocking_prof_core.Dock")));
      this.label_clocking_prof_core.Enabled = ((bool)(resources.GetObject("label_clocking_prof_core.Enabled")));
      this.label_clocking_prof_core.Font = ((System.Drawing.Font)(resources.GetObject("label_clocking_prof_core.Font")));
      this.label_clocking_prof_core.Image = ((System.Drawing.Image)(resources.GetObject("label_clocking_prof_core.Image")));
      this.label_clocking_prof_core.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_clocking_prof_core.ImageAlign")));
      this.label_clocking_prof_core.ImageIndex = ((int)(resources.GetObject("label_clocking_prof_core.ImageIndex")));
      this.label_clocking_prof_core.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_clocking_prof_core.ImeMode")));
      this.label_clocking_prof_core.Location = ((System.Drawing.Point)(resources.GetObject("label_clocking_prof_core.Location")));
      this.label_clocking_prof_core.Name = "label_clocking_prof_core";
      this.label_clocking_prof_core.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_clocking_prof_core.RightToLeft")));
      this.label_clocking_prof_core.Size = ((System.Drawing.Size)(resources.GetObject("label_clocking_prof_core.Size")));
      this.label_clocking_prof_core.TabIndex = ((int)(resources.GetObject("label_clocking_prof_core.TabIndex")));
      this.label_clocking_prof_core.Text = resources.GetString("label_clocking_prof_core.Text");
      this.label_clocking_prof_core.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_clocking_prof_core.TextAlign")));
      this.toolTip.SetToolTip(this.label_clocking_prof_core, resources.GetString("label_clocking_prof_core.ToolTip"));
      this.label_clocking_prof_core.Visible = ((bool)(resources.GetObject("label_clocking_prof_core.Visible")));
      // 
      // label5
      // 
      this.label5.AccessibleDescription = resources.GetString("label5.AccessibleDescription");
      this.label5.AccessibleName = resources.GetString("label5.AccessibleName");
      this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label5.Anchor")));
      this.label5.AutoSize = ((bool)(resources.GetObject("label5.AutoSize")));
      this.label5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label5.Dock")));
      this.label5.Enabled = ((bool)(resources.GetObject("label5.Enabled")));
      this.label5.Font = ((System.Drawing.Font)(resources.GetObject("label5.Font")));
      this.label5.Image = ((System.Drawing.Image)(resources.GetObject("label5.Image")));
      this.label5.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label5.ImageAlign")));
      this.label5.ImageIndex = ((int)(resources.GetObject("label5.ImageIndex")));
      this.label5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label5.ImeMode")));
      this.label5.Location = ((System.Drawing.Point)(resources.GetObject("label5.Location")));
      this.label5.Name = "label5";
      this.label5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label5.RightToLeft")));
      this.label5.Size = ((System.Drawing.Size)(resources.GetObject("label5.Size")));
      this.label5.TabIndex = ((int)(resources.GetObject("label5.TabIndex")));
      this.label5.Text = resources.GetString("label5.Text");
      this.label5.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label5.TextAlign")));
      this.toolTip.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
      this.label5.Visible = ((bool)(resources.GetObject("label5.Visible")));
      // 
      // combo_clocking_prof_presets
      // 
      this.combo_clocking_prof_presets.AccessibleDescription = resources.GetString("combo_clocking_prof_presets.AccessibleDescription");
      this.combo_clocking_prof_presets.AccessibleName = resources.GetString("combo_clocking_prof_presets.AccessibleName");
      this.combo_clocking_prof_presets.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_clocking_prof_presets.Anchor")));
      this.combo_clocking_prof_presets.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_clocking_prof_presets.BackgroundImage")));
      this.combo_clocking_prof_presets.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_clocking_prof_presets.Dock")));
      this.combo_clocking_prof_presets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_clocking_prof_presets.Enabled = ((bool)(resources.GetObject("combo_clocking_prof_presets.Enabled")));
      this.combo_clocking_prof_presets.Font = ((System.Drawing.Font)(resources.GetObject("combo_clocking_prof_presets.Font")));
      this.combo_clocking_prof_presets.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_clocking_prof_presets.ImeMode")));
      this.combo_clocking_prof_presets.IntegralHeight = ((bool)(resources.GetObject("combo_clocking_prof_presets.IntegralHeight")));
      this.combo_clocking_prof_presets.ItemHeight = ((int)(resources.GetObject("combo_clocking_prof_presets.ItemHeight")));
      this.combo_clocking_prof_presets.Items.AddRange(new object[] {
                                                                     resources.GetString("combo_clocking_prof_presets.Items"),
                                                                     resources.GetString("combo_clocking_prof_presets.Items1"),
                                                                     resources.GetString("combo_clocking_prof_presets.Items2"),
                                                                     resources.GetString("combo_clocking_prof_presets.Items3"),
                                                                     resources.GetString("combo_clocking_prof_presets.Items4")});
      this.combo_clocking_prof_presets.Location = ((System.Drawing.Point)(resources.GetObject("combo_clocking_prof_presets.Location")));
      this.combo_clocking_prof_presets.MaxDropDownItems = ((int)(resources.GetObject("combo_clocking_prof_presets.MaxDropDownItems")));
      this.combo_clocking_prof_presets.MaxLength = ((int)(resources.GetObject("combo_clocking_prof_presets.MaxLength")));
      this.combo_clocking_prof_presets.Name = "combo_clocking_prof_presets";
      this.combo_clocking_prof_presets.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_clocking_prof_presets.RightToLeft")));
      this.combo_clocking_prof_presets.Size = ((System.Drawing.Size)(resources.GetObject("combo_clocking_prof_presets.Size")));
      this.combo_clocking_prof_presets.TabIndex = ((int)(resources.GetObject("combo_clocking_prof_presets.TabIndex")));
      this.combo_clocking_prof_presets.Text = resources.GetString("combo_clocking_prof_presets.Text");
      this.toolTip.SetToolTip(this.combo_clocking_prof_presets, resources.GetString("combo_clocking_prof_presets.ToolTip"));
      this.combo_clocking_prof_presets.Visible = ((bool)(resources.GetObject("combo_clocking_prof_presets.Visible")));
      this.combo_clocking_prof_presets.SelectedIndexChanged += new System.EventHandler(this.combo_clocking_prof_presets_SelectedIndexChanged);
      // 
      // tab_exp
      // 
      this.tab_exp.AccessibleDescription = resources.GetString("tab_exp.AccessibleDescription");
      this.tab_exp.AccessibleName = resources.GetString("tab_exp.AccessibleName");
      this.tab_exp.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab_exp.Anchor")));
      this.tab_exp.AutoScroll = ((bool)(resources.GetObject("tab_exp.AutoScroll")));
      this.tab_exp.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tab_exp.AutoScrollMargin")));
      this.tab_exp.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tab_exp.AutoScrollMinSize")));
      this.tab_exp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab_exp.BackgroundImage")));
      this.tab_exp.Controls.Add(this.picture_ind_d3d);
      this.tab_exp.Controls.Add(this.check_ind_ogl);
      this.tab_exp.Controls.Add(this.check_ind_d3d);
      this.tab_exp.Controls.Add(this.label_ind_mode);
      this.tab_exp.Controls.Add(this.combo_ind_modeVal);
      this.tab_exp.Controls.Add(this.list_3d);
      this.tab_exp.Controls.Add(this.picture_ind_ogl);
      this.tab_exp.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab_exp.Dock")));
      this.tab_exp.Enabled = ((bool)(resources.GetObject("tab_exp.Enabled")));
      this.tab_exp.Font = ((System.Drawing.Font)(resources.GetObject("tab_exp.Font")));
      this.tab_exp.ImageIndex = ((int)(resources.GetObject("tab_exp.ImageIndex")));
      this.tab_exp.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab_exp.ImeMode")));
      this.tab_exp.Location = ((System.Drawing.Point)(resources.GetObject("tab_exp.Location")));
      this.tab_exp.Name = "tab_exp";
      this.tab_exp.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab_exp.RightToLeft")));
      this.tab_exp.Size = ((System.Drawing.Size)(resources.GetObject("tab_exp.Size")));
      this.tab_exp.TabIndex = ((int)(resources.GetObject("tab_exp.TabIndex")));
      this.tab_exp.Text = resources.GetString("tab_exp.Text");
      this.toolTip.SetToolTip(this.tab_exp, resources.GetString("tab_exp.ToolTip"));
      this.tab_exp.ToolTipText = resources.GetString("tab_exp.ToolTipText");
      this.tab_exp.Visible = ((bool)(resources.GetObject("tab_exp.Visible")));
      // 
      // picture_ind_d3d
      // 
      this.picture_ind_d3d.AccessibleDescription = resources.GetString("picture_ind_d3d.AccessibleDescription");
      this.picture_ind_d3d.AccessibleName = resources.GetString("picture_ind_d3d.AccessibleName");
      this.picture_ind_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("picture_ind_d3d.Anchor")));
      this.picture_ind_d3d.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picture_ind_d3d.BackgroundImage")));
      this.picture_ind_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("picture_ind_d3d.Dock")));
      this.picture_ind_d3d.Enabled = ((bool)(resources.GetObject("picture_ind_d3d.Enabled")));
      this.picture_ind_d3d.Font = ((System.Drawing.Font)(resources.GetObject("picture_ind_d3d.Font")));
      this.picture_ind_d3d.Image = ((System.Drawing.Image)(resources.GetObject("picture_ind_d3d.Image")));
      this.picture_ind_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("picture_ind_d3d.ImeMode")));
      this.picture_ind_d3d.Location = ((System.Drawing.Point)(resources.GetObject("picture_ind_d3d.Location")));
      this.picture_ind_d3d.Name = "picture_ind_d3d";
      this.picture_ind_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("picture_ind_d3d.RightToLeft")));
      this.picture_ind_d3d.Size = ((System.Drawing.Size)(resources.GetObject("picture_ind_d3d.Size")));
      this.picture_ind_d3d.SizeMode = ((System.Windows.Forms.PictureBoxSizeMode)(resources.GetObject("picture_ind_d3d.SizeMode")));
      this.picture_ind_d3d.TabIndex = ((int)(resources.GetObject("picture_ind_d3d.TabIndex")));
      this.picture_ind_d3d.TabStop = false;
      this.picture_ind_d3d.Text = resources.GetString("picture_ind_d3d.Text");
      this.toolTip.SetToolTip(this.picture_ind_d3d, resources.GetString("picture_ind_d3d.ToolTip"));
      this.picture_ind_d3d.Visible = ((bool)(resources.GetObject("picture_ind_d3d.Visible")));
      // 
      // check_ind_ogl
      // 
      this.check_ind_ogl.AccessibleDescription = resources.GetString("check_ind_ogl.AccessibleDescription");
      this.check_ind_ogl.AccessibleName = resources.GetString("check_ind_ogl.AccessibleName");
      this.check_ind_ogl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_ind_ogl.Anchor")));
      this.check_ind_ogl.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_ind_ogl.Appearance")));
      this.check_ind_ogl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_ind_ogl.BackgroundImage")));
      this.check_ind_ogl.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_ind_ogl.CheckAlign")));
      this.check_ind_ogl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_ind_ogl.Dock")));
      this.check_ind_ogl.Enabled = ((bool)(resources.GetObject("check_ind_ogl.Enabled")));
      this.check_ind_ogl.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_ind_ogl.FlatStyle")));
      this.check_ind_ogl.Font = ((System.Drawing.Font)(resources.GetObject("check_ind_ogl.Font")));
      this.check_ind_ogl.Image = ((System.Drawing.Image)(resources.GetObject("check_ind_ogl.Image")));
      this.check_ind_ogl.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_ind_ogl.ImageAlign")));
      this.check_ind_ogl.ImageIndex = ((int)(resources.GetObject("check_ind_ogl.ImageIndex")));
      this.check_ind_ogl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_ind_ogl.ImeMode")));
      this.check_ind_ogl.Location = ((System.Drawing.Point)(resources.GetObject("check_ind_ogl.Location")));
      this.check_ind_ogl.Name = "check_ind_ogl";
      this.check_ind_ogl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_ind_ogl.RightToLeft")));
      this.check_ind_ogl.Size = ((System.Drawing.Size)(resources.GetObject("check_ind_ogl.Size")));
      this.check_ind_ogl.TabIndex = ((int)(resources.GetObject("check_ind_ogl.TabIndex")));
      this.check_ind_ogl.Text = resources.GetString("check_ind_ogl.Text");
      this.check_ind_ogl.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_ind_ogl.TextAlign")));
      this.toolTip.SetToolTip(this.check_ind_ogl, resources.GetString("check_ind_ogl.ToolTip"));
      this.check_ind_ogl.Visible = ((bool)(resources.GetObject("check_ind_ogl.Visible")));
      this.check_ind_ogl.CheckedChanged += new System.EventHandler(this.check_ind_ogl_CheckedChanged);
      // 
      // check_ind_d3d
      // 
      this.check_ind_d3d.AccessibleDescription = resources.GetString("check_ind_d3d.AccessibleDescription");
      this.check_ind_d3d.AccessibleName = resources.GetString("check_ind_d3d.AccessibleName");
      this.check_ind_d3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_ind_d3d.Anchor")));
      this.check_ind_d3d.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_ind_d3d.Appearance")));
      this.check_ind_d3d.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_ind_d3d.BackgroundImage")));
      this.check_ind_d3d.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_ind_d3d.CheckAlign")));
      this.check_ind_d3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_ind_d3d.Dock")));
      this.check_ind_d3d.Enabled = ((bool)(resources.GetObject("check_ind_d3d.Enabled")));
      this.check_ind_d3d.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_ind_d3d.FlatStyle")));
      this.check_ind_d3d.Font = ((System.Drawing.Font)(resources.GetObject("check_ind_d3d.Font")));
      this.check_ind_d3d.Image = ((System.Drawing.Image)(resources.GetObject("check_ind_d3d.Image")));
      this.check_ind_d3d.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_ind_d3d.ImageAlign")));
      this.check_ind_d3d.ImageIndex = ((int)(resources.GetObject("check_ind_d3d.ImageIndex")));
      this.check_ind_d3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_ind_d3d.ImeMode")));
      this.check_ind_d3d.Location = ((System.Drawing.Point)(resources.GetObject("check_ind_d3d.Location")));
      this.check_ind_d3d.Name = "check_ind_d3d";
      this.check_ind_d3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_ind_d3d.RightToLeft")));
      this.check_ind_d3d.Size = ((System.Drawing.Size)(resources.GetObject("check_ind_d3d.Size")));
      this.check_ind_d3d.TabIndex = ((int)(resources.GetObject("check_ind_d3d.TabIndex")));
      this.check_ind_d3d.Text = resources.GetString("check_ind_d3d.Text");
      this.check_ind_d3d.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_ind_d3d.TextAlign")));
      this.toolTip.SetToolTip(this.check_ind_d3d, resources.GetString("check_ind_d3d.ToolTip"));
      this.check_ind_d3d.Visible = ((bool)(resources.GetObject("check_ind_d3d.Visible")));
      this.check_ind_d3d.CheckedChanged += new System.EventHandler(this.check_ind_d3d_CheckedChanged);
      // 
      // label_ind_mode
      // 
      this.label_ind_mode.AccessibleDescription = resources.GetString("label_ind_mode.AccessibleDescription");
      this.label_ind_mode.AccessibleName = resources.GetString("label_ind_mode.AccessibleName");
      this.label_ind_mode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_ind_mode.Anchor")));
      this.label_ind_mode.AutoSize = ((bool)(resources.GetObject("label_ind_mode.AutoSize")));
      this.label_ind_mode.Cursor = System.Windows.Forms.Cursors.Hand;
      this.label_ind_mode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_ind_mode.Dock")));
      this.label_ind_mode.Enabled = ((bool)(resources.GetObject("label_ind_mode.Enabled")));
      this.label_ind_mode.Font = ((System.Drawing.Font)(resources.GetObject("label_ind_mode.Font")));
      this.label_ind_mode.Image = ((System.Drawing.Image)(resources.GetObject("label_ind_mode.Image")));
      this.label_ind_mode.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_ind_mode.ImageAlign")));
      this.label_ind_mode.ImageIndex = ((int)(resources.GetObject("label_ind_mode.ImageIndex")));
      this.label_ind_mode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_ind_mode.ImeMode")));
      this.label_ind_mode.Location = ((System.Drawing.Point)(resources.GetObject("label_ind_mode.Location")));
      this.label_ind_mode.Name = "label_ind_mode";
      this.label_ind_mode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_ind_mode.RightToLeft")));
      this.label_ind_mode.Size = ((System.Drawing.Size)(resources.GetObject("label_ind_mode.Size")));
      this.label_ind_mode.TabIndex = ((int)(resources.GetObject("label_ind_mode.TabIndex")));
      this.label_ind_mode.Tag = "0";
      this.label_ind_mode.Text = resources.GetString("label_ind_mode.Text");
      this.label_ind_mode.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_ind_mode.TextAlign")));
      this.toolTip.SetToolTip(this.label_ind_mode, resources.GetString("label_ind_mode.ToolTip"));
      this.label_ind_mode.Visible = ((bool)(resources.GetObject("label_ind_mode.Visible")));
      // 
      // combo_ind_modeVal
      // 
      this.combo_ind_modeVal.AccessibleDescription = resources.GetString("combo_ind_modeVal.AccessibleDescription");
      this.combo_ind_modeVal.AccessibleName = resources.GetString("combo_ind_modeVal.AccessibleName");
      this.combo_ind_modeVal.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_ind_modeVal.Anchor")));
      this.combo_ind_modeVal.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_ind_modeVal.BackgroundImage")));
      this.combo_ind_modeVal.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_ind_modeVal.Dock")));
      this.combo_ind_modeVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_ind_modeVal.Enabled = ((bool)(resources.GetObject("combo_ind_modeVal.Enabled")));
      this.combo_ind_modeVal.Font = ((System.Drawing.Font)(resources.GetObject("combo_ind_modeVal.Font")));
      this.combo_ind_modeVal.ForeColor = System.Drawing.SystemColors.WindowText;
      this.combo_ind_modeVal.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_ind_modeVal.ImeMode")));
      this.combo_ind_modeVal.IntegralHeight = ((bool)(resources.GetObject("combo_ind_modeVal.IntegralHeight")));
      this.combo_ind_modeVal.ItemHeight = ((int)(resources.GetObject("combo_ind_modeVal.ItemHeight")));
      this.combo_ind_modeVal.Location = ((System.Drawing.Point)(resources.GetObject("combo_ind_modeVal.Location")));
      this.combo_ind_modeVal.MaxDropDownItems = ((int)(resources.GetObject("combo_ind_modeVal.MaxDropDownItems")));
      this.combo_ind_modeVal.MaxLength = ((int)(resources.GetObject("combo_ind_modeVal.MaxLength")));
      this.combo_ind_modeVal.Name = "combo_ind_modeVal";
      this.combo_ind_modeVal.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_ind_modeVal.RightToLeft")));
      this.combo_ind_modeVal.Size = ((System.Drawing.Size)(resources.GetObject("combo_ind_modeVal.Size")));
      this.combo_ind_modeVal.TabIndex = ((int)(resources.GetObject("combo_ind_modeVal.TabIndex")));
      this.combo_ind_modeVal.Text = resources.GetString("combo_ind_modeVal.Text");
      this.toolTip.SetToolTip(this.combo_ind_modeVal, resources.GetString("combo_ind_modeVal.ToolTip"));
      this.combo_ind_modeVal.Visible = ((bool)(resources.GetObject("combo_ind_modeVal.Visible")));
      this.combo_ind_modeVal.SelectedIndexChanged += new System.EventHandler(this.combo_ind_modeVal_SelectedIndexChanged);
      // 
      // list_3d
      // 
      this.list_3d.AccessibleDescription = resources.GetString("list_3d.AccessibleDescription");
      this.list_3d.AccessibleName = resources.GetString("list_3d.AccessibleName");
      this.list_3d.Alignment = ((System.Windows.Forms.ListViewAlignment)(resources.GetObject("list_3d.Alignment")));
      this.list_3d.AllowColumnReorder = true;
      this.list_3d.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("list_3d.Anchor")));
      this.list_3d.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("list_3d.BackgroundImage")));
      this.list_3d.CheckBoxes = true;
      this.list_3d.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                              this.columnHeader_name,
                                                                              this.columnHeader_profile,
                                                                              this.columnHeader_driver});
      this.list_3d.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("list_3d.Dock")));
      this.list_3d.Enabled = ((bool)(resources.GetObject("list_3d.Enabled")));
      this.list_3d.Font = ((System.Drawing.Font)(resources.GetObject("list_3d.Font")));
      this.list_3d.FullRowSelect = true;
      this.list_3d.GridLines = true;
      this.list_3d.HideSelection = false;
      this.list_3d.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("list_3d.ImeMode")));
      this.list_3d.LabelWrap = ((bool)(resources.GetObject("list_3d.LabelWrap")));
      this.list_3d.Location = ((System.Drawing.Point)(resources.GetObject("list_3d.Location")));
      this.list_3d.MultiSelect = false;
      this.list_3d.Name = "list_3d";
      this.list_3d.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("list_3d.RightToLeft")));
      this.list_3d.Size = ((System.Drawing.Size)(resources.GetObject("list_3d.Size")));
      this.list_3d.TabIndex = ((int)(resources.GetObject("list_3d.TabIndex")));
      this.list_3d.Text = resources.GetString("list_3d.Text");
      this.toolTip.SetToolTip(this.list_3d, resources.GetString("list_3d.ToolTip"));
      this.list_3d.View = System.Windows.Forms.View.Details;
      this.list_3d.Visible = ((bool)(resources.GetObject("list_3d.Visible")));
      this.list_3d.SelectedIndexChanged += new System.EventHandler(this.list_3d_SelectedIndexChanged);
      this.list_3d.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.list_3d_ItemCheck);
      // 
      // columnHeader_name
      // 
      this.columnHeader_name.Text = resources.GetString("columnHeader_name.Text");
      this.columnHeader_name.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader_name.TextAlign")));
      this.columnHeader_name.Width = ((int)(resources.GetObject("columnHeader_name.Width")));
      // 
      // columnHeader_profile
      // 
      this.columnHeader_profile.Text = resources.GetString("columnHeader_profile.Text");
      this.columnHeader_profile.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader_profile.TextAlign")));
      this.columnHeader_profile.Width = ((int)(resources.GetObject("columnHeader_profile.Width")));
      // 
      // columnHeader_driver
      // 
      this.columnHeader_driver.Text = resources.GetString("columnHeader_driver.Text");
      this.columnHeader_driver.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader_driver.TextAlign")));
      this.columnHeader_driver.Width = ((int)(resources.GetObject("columnHeader_driver.Width")));
      // 
      // picture_ind_ogl
      // 
      this.picture_ind_ogl.AccessibleDescription = resources.GetString("picture_ind_ogl.AccessibleDescription");
      this.picture_ind_ogl.AccessibleName = resources.GetString("picture_ind_ogl.AccessibleName");
      this.picture_ind_ogl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("picture_ind_ogl.Anchor")));
      this.picture_ind_ogl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picture_ind_ogl.BackgroundImage")));
      this.picture_ind_ogl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("picture_ind_ogl.Dock")));
      this.picture_ind_ogl.Enabled = ((bool)(resources.GetObject("picture_ind_ogl.Enabled")));
      this.picture_ind_ogl.Font = ((System.Drawing.Font)(resources.GetObject("picture_ind_ogl.Font")));
      this.picture_ind_ogl.Image = ((System.Drawing.Image)(resources.GetObject("picture_ind_ogl.Image")));
      this.picture_ind_ogl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("picture_ind_ogl.ImeMode")));
      this.picture_ind_ogl.Location = ((System.Drawing.Point)(resources.GetObject("picture_ind_ogl.Location")));
      this.picture_ind_ogl.Name = "picture_ind_ogl";
      this.picture_ind_ogl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("picture_ind_ogl.RightToLeft")));
      this.picture_ind_ogl.Size = ((System.Drawing.Size)(resources.GetObject("picture_ind_ogl.Size")));
      this.picture_ind_ogl.SizeMode = ((System.Windows.Forms.PictureBoxSizeMode)(resources.GetObject("picture_ind_ogl.SizeMode")));
      this.picture_ind_ogl.TabIndex = ((int)(resources.GetObject("picture_ind_ogl.TabIndex")));
      this.picture_ind_ogl.TabStop = false;
      this.picture_ind_ogl.Text = resources.GetString("picture_ind_ogl.Text");
      this.toolTip.SetToolTip(this.picture_ind_ogl, resources.GetString("picture_ind_ogl.ToolTip"));
      this.picture_ind_ogl.Visible = ((bool)(resources.GetObject("picture_ind_ogl.Visible")));
      // 
      // toolTip
      // 
      this.toolTip.ShowAlways = true;
      // 
      // button_prof_new
      // 
      this.button_prof_new.AccessibleDescription = resources.GetString("button_prof_new.AccessibleDescription");
      this.button_prof_new.AccessibleName = resources.GetString("button_prof_new.AccessibleName");
      this.button_prof_new.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_new.Anchor")));
      this.button_prof_new.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_new.BackgroundImage")));
      this.button_prof_new.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_new.Dock")));
      this.button_prof_new.Enabled = ((bool)(resources.GetObject("button_prof_new.Enabled")));
      this.button_prof_new.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_new.FlatStyle")));
      this.button_prof_new.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_new.Font")));
      this.button_prof_new.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_new.Image")));
      this.button_prof_new.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_new.ImageAlign")));
      this.button_prof_new.ImageIndex = ((int)(resources.GetObject("button_prof_new.ImageIndex")));
      this.button_prof_new.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_new.ImeMode")));
      this.button_prof_new.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_new.Location")));
      this.button_prof_new.Name = "button_prof_new";
      this.button_prof_new.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_new.RightToLeft")));
      this.button_prof_new.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_new.Size")));
      this.button_prof_new.TabIndex = ((int)(resources.GetObject("button_prof_new.TabIndex")));
      this.button_prof_new.Text = resources.GetString("button_prof_new.Text");
      this.button_prof_new.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_new.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_new, resources.GetString("button_prof_new.ToolTip"));
      this.button_prof_new.Visible = ((bool)(resources.GetObject("button_prof_new.Visible")));
      this.button_prof_new.Click += new System.EventHandler(this.button_prof_new_Click);
      // 
      // button_prof_clone
      // 
      this.button_prof_clone.AccessibleDescription = resources.GetString("button_prof_clone.AccessibleDescription");
      this.button_prof_clone.AccessibleName = resources.GetString("button_prof_clone.AccessibleName");
      this.button_prof_clone.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_clone.Anchor")));
      this.button_prof_clone.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_clone.BackgroundImage")));
      this.button_prof_clone.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_clone.Dock")));
      this.button_prof_clone.Enabled = ((bool)(resources.GetObject("button_prof_clone.Enabled")));
      this.button_prof_clone.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_clone.FlatStyle")));
      this.button_prof_clone.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_clone.Font")));
      this.button_prof_clone.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_clone.Image")));
      this.button_prof_clone.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_clone.ImageAlign")));
      this.button_prof_clone.ImageIndex = ((int)(resources.GetObject("button_prof_clone.ImageIndex")));
      this.button_prof_clone.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_clone.ImeMode")));
      this.button_prof_clone.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_clone.Location")));
      this.button_prof_clone.Name = "button_prof_clone";
      this.button_prof_clone.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_clone.RightToLeft")));
      this.button_prof_clone.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_clone.Size")));
      this.button_prof_clone.TabIndex = ((int)(resources.GetObject("button_prof_clone.TabIndex")));
      this.button_prof_clone.Text = resources.GetString("button_prof_clone.Text");
      this.button_prof_clone.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_clone.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_clone, resources.GetString("button_prof_clone.ToolTip"));
      this.button_prof_clone.Visible = ((bool)(resources.GetObject("button_prof_clone.Visible")));
      this.button_prof_clone.Click += new System.EventHandler(this.button_prof_clone_Click);
      // 
      // text_prof_name
      // 
      this.text_prof_name.AccessibleDescription = resources.GetString("text_prof_name.AccessibleDescription");
      this.text_prof_name.AccessibleName = resources.GetString("text_prof_name.AccessibleName");
      this.text_prof_name.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_prof_name.Anchor")));
      this.text_prof_name.AutoSize = ((bool)(resources.GetObject("text_prof_name.AutoSize")));
      this.text_prof_name.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_prof_name.BackgroundImage")));
      this.text_prof_name.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_prof_name.Dock")));
      this.text_prof_name.Enabled = ((bool)(resources.GetObject("text_prof_name.Enabled")));
      this.text_prof_name.Font = ((System.Drawing.Font)(resources.GetObject("text_prof_name.Font")));
      this.text_prof_name.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_prof_name.ImeMode")));
      this.text_prof_name.Location = ((System.Drawing.Point)(resources.GetObject("text_prof_name.Location")));
      this.text_prof_name.MaxLength = ((int)(resources.GetObject("text_prof_name.MaxLength")));
      this.text_prof_name.Multiline = ((bool)(resources.GetObject("text_prof_name.Multiline")));
      this.text_prof_name.Name = "text_prof_name";
      this.text_prof_name.PasswordChar = ((char)(resources.GetObject("text_prof_name.PasswordChar")));
      this.text_prof_name.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_prof_name.RightToLeft")));
      this.text_prof_name.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_prof_name.ScrollBars")));
      this.text_prof_name.Size = ((System.Drawing.Size)(resources.GetObject("text_prof_name.Size")));
      this.text_prof_name.TabIndex = ((int)(resources.GetObject("text_prof_name.TabIndex")));
      this.text_prof_name.Text = resources.GetString("text_prof_name.Text");
      this.text_prof_name.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_prof_name.TextAlign")));
      this.toolTip.SetToolTip(this.text_prof_name, resources.GetString("text_prof_name.ToolTip"));
      this.text_prof_name.Visible = ((bool)(resources.GetObject("text_prof_name.Visible")));
      this.text_prof_name.WordWrap = ((bool)(resources.GetObject("text_prof_name.WordWrap")));
      this.text_prof_name.TextChanged += new System.EventHandler(this.text_prof_name_TextChanged);
      // 
      // button_prof_cancel
      // 
      this.button_prof_cancel.AccessibleDescription = resources.GetString("button_prof_cancel.AccessibleDescription");
      this.button_prof_cancel.AccessibleName = resources.GetString("button_prof_cancel.AccessibleName");
      this.button_prof_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_cancel.Anchor")));
      this.button_prof_cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_cancel.BackgroundImage")));
      this.button_prof_cancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_cancel.Dock")));
      this.button_prof_cancel.Enabled = ((bool)(resources.GetObject("button_prof_cancel.Enabled")));
      this.button_prof_cancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_cancel.FlatStyle")));
      this.button_prof_cancel.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_cancel.Font")));
      this.button_prof_cancel.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_cancel.Image")));
      this.button_prof_cancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_cancel.ImageAlign")));
      this.button_prof_cancel.ImageIndex = ((int)(resources.GetObject("button_prof_cancel.ImageIndex")));
      this.button_prof_cancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_cancel.ImeMode")));
      this.button_prof_cancel.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_cancel.Location")));
      this.button_prof_cancel.Name = "button_prof_cancel";
      this.button_prof_cancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_cancel.RightToLeft")));
      this.button_prof_cancel.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_cancel.Size")));
      this.button_prof_cancel.TabIndex = ((int)(resources.GetObject("button_prof_cancel.TabIndex")));
      this.button_prof_cancel.Text = resources.GetString("button_prof_cancel.Text");
      this.button_prof_cancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_cancel.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_cancel, resources.GetString("button_prof_cancel.ToolTip"));
      this.button_prof_cancel.Visible = ((bool)(resources.GetObject("button_prof_cancel.Visible")));
      this.button_prof_cancel.Click += new System.EventHandler(this.button_prof_cancel_Click);
      // 
      // button_prof_ok
      // 
      this.button_prof_ok.AccessibleDescription = resources.GetString("button_prof_ok.AccessibleDescription");
      this.button_prof_ok.AccessibleName = resources.GetString("button_prof_ok.AccessibleName");
      this.button_prof_ok.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_ok.Anchor")));
      this.button_prof_ok.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_ok.BackgroundImage")));
      this.button_prof_ok.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_ok.Dock")));
      this.button_prof_ok.Enabled = ((bool)(resources.GetObject("button_prof_ok.Enabled")));
      this.button_prof_ok.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_ok.FlatStyle")));
      this.button_prof_ok.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_ok.Font")));
      this.button_prof_ok.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_ok.Image")));
      this.button_prof_ok.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_ok.ImageAlign")));
      this.button_prof_ok.ImageIndex = ((int)(resources.GetObject("button_prof_ok.ImageIndex")));
      this.button_prof_ok.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_ok.ImeMode")));
      this.button_prof_ok.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_ok.Location")));
      this.button_prof_ok.Name = "button_prof_ok";
      this.button_prof_ok.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_ok.RightToLeft")));
      this.button_prof_ok.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_ok.Size")));
      this.button_prof_ok.TabIndex = ((int)(resources.GetObject("button_prof_ok.TabIndex")));
      this.button_prof_ok.Text = resources.GetString("button_prof_ok.Text");
      this.button_prof_ok.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_ok.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_ok, resources.GetString("button_prof_ok.ToolTip"));
      this.button_prof_ok.Visible = ((bool)(resources.GetObject("button_prof_ok.Visible")));
      this.button_prof_ok.Click += new System.EventHandler(this.button_prof_ok_Click);
      // 
      // panel_prof_apply
      // 
      this.panel_prof_apply.AccessibleDescription = resources.GetString("panel_prof_apply.AccessibleDescription");
      this.panel_prof_apply.AccessibleName = resources.GetString("panel_prof_apply.AccessibleName");
      this.panel_prof_apply.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panel_prof_apply.Anchor")));
      this.panel_prof_apply.AutoScroll = ((bool)(resources.GetObject("panel_prof_apply.AutoScroll")));
      this.panel_prof_apply.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panel_prof_apply.AutoScrollMargin")));
      this.panel_prof_apply.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panel_prof_apply.AutoScrollMinSize")));
      this.panel_prof_apply.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel_prof_apply.BackgroundImage")));
      this.panel_prof_apply.Controls.Add(this.button_prof_restore);
      this.panel_prof_apply.Controls.Add(this.button_prof_apply);
      this.panel_prof_apply.Controls.Add(this.button_prof_run_exe);
      this.panel_prof_apply.Controls.Add(this.button_prof_apply_and_run);
      this.panel_prof_apply.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panel_prof_apply.Dock")));
      this.panel_prof_apply.Enabled = ((bool)(resources.GetObject("panel_prof_apply.Enabled")));
      this.panel_prof_apply.Font = ((System.Drawing.Font)(resources.GetObject("panel_prof_apply.Font")));
      this.panel_prof_apply.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panel_prof_apply.ImeMode")));
      this.panel_prof_apply.Location = ((System.Drawing.Point)(resources.GetObject("panel_prof_apply.Location")));
      this.panel_prof_apply.Name = "panel_prof_apply";
      this.panel_prof_apply.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panel_prof_apply.RightToLeft")));
      this.panel_prof_apply.Size = ((System.Drawing.Size)(resources.GetObject("panel_prof_apply.Size")));
      this.panel_prof_apply.TabIndex = ((int)(resources.GetObject("panel_prof_apply.TabIndex")));
      this.panel_prof_apply.Text = resources.GetString("panel_prof_apply.Text");
      this.toolTip.SetToolTip(this.panel_prof_apply, resources.GetString("panel_prof_apply.ToolTip"));
      this.panel_prof_apply.Visible = ((bool)(resources.GetObject("panel_prof_apply.Visible")));
      // 
      // button_prof_restore
      // 
      this.button_prof_restore.AccessibleDescription = resources.GetString("button_prof_restore.AccessibleDescription");
      this.button_prof_restore.AccessibleName = resources.GetString("button_prof_restore.AccessibleName");
      this.button_prof_restore.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_restore.Anchor")));
      this.button_prof_restore.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_restore.BackgroundImage")));
      this.button_prof_restore.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_restore.Dock")));
      this.button_prof_restore.Enabled = ((bool)(resources.GetObject("button_prof_restore.Enabled")));
      this.button_prof_restore.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_restore.FlatStyle")));
      this.button_prof_restore.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_restore.Font")));
      this.button_prof_restore.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_restore.Image")));
      this.button_prof_restore.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_restore.ImageAlign")));
      this.button_prof_restore.ImageIndex = ((int)(resources.GetObject("button_prof_restore.ImageIndex")));
      this.button_prof_restore.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_restore.ImeMode")));
      this.button_prof_restore.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_restore.Location")));
      this.button_prof_restore.Name = "button_prof_restore";
      this.button_prof_restore.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_restore.RightToLeft")));
      this.button_prof_restore.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_restore.Size")));
      this.button_prof_restore.TabIndex = ((int)(resources.GetObject("button_prof_restore.TabIndex")));
      this.button_prof_restore.Text = resources.GetString("button_prof_restore.Text");
      this.button_prof_restore.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_restore.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_restore, resources.GetString("button_prof_restore.ToolTip"));
      this.button_prof_restore.Visible = ((bool)(resources.GetObject("button_prof_restore.Visible")));
      this.button_prof_restore.Click += new System.EventHandler(this.button_prof_restore_Click);
      // 
      // group_prof
      // 
      this.group_prof.AccessibleDescription = resources.GetString("group_prof.AccessibleDescription");
      this.group_prof.AccessibleName = resources.GetString("group_prof.AccessibleName");
      this.group_prof.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("group_prof.Anchor")));
      this.group_prof.AutoScroll = ((bool)(resources.GetObject("group_prof.AutoScroll")));
      this.group_prof.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("group_prof.AutoScrollMargin")));
      this.group_prof.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("group_prof.AutoScrollMinSize")));
      this.group_prof.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("group_prof.BackgroundImage")));
      this.group_prof.Controls.Add(this.button_prof_discard);
      this.group_prof.Controls.Add(this.check_prof_quit);
      this.group_prof.Controls.Add(this.panel_prof_apply);
      this.group_prof.Controls.Add(this.combo_prof_names);
      this.group_prof.Controls.Add(this.text_prof_name);
      this.group_prof.Controls.Add(this.button_prof_save);
      this.group_prof.Controls.Add(this.button_prof_delete);
      this.group_prof.Controls.Add(this.button_prof_rename);
      this.group_prof.Controls.Add(this.button_prof_clone);
      this.group_prof.Controls.Add(this.button_prof_new);
      this.group_prof.Controls.Add(this.button_prof_cancel);
      this.group_prof.Controls.Add(this.button_prof_ok);
      this.group_prof.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("group_prof.Dock")));
      this.group_prof.Enabled = ((bool)(resources.GetObject("group_prof.Enabled")));
      this.group_prof.Font = ((System.Drawing.Font)(resources.GetObject("group_prof.Font")));
      this.group_prof.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("group_prof.ImeMode")));
      this.group_prof.Location = ((System.Drawing.Point)(resources.GetObject("group_prof.Location")));
      this.group_prof.Name = "group_prof";
      this.group_prof.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("group_prof.RightToLeft")));
      this.group_prof.Size = ((System.Drawing.Size)(resources.GetObject("group_prof.Size")));
      this.group_prof.TabIndex = ((int)(resources.GetObject("group_prof.TabIndex")));
      this.group_prof.Text = resources.GetString("group_prof.Text");
      this.toolTip.SetToolTip(this.group_prof, resources.GetString("group_prof.ToolTip"));
      this.group_prof.Visible = ((bool)(resources.GetObject("group_prof.Visible")));
      // 
      // button_prof_discard
      // 
      this.button_prof_discard.AccessibleDescription = resources.GetString("button_prof_discard.AccessibleDescription");
      this.button_prof_discard.AccessibleName = resources.GetString("button_prof_discard.AccessibleName");
      this.button_prof_discard.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_discard.Anchor")));
      this.button_prof_discard.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_discard.BackgroundImage")));
      this.button_prof_discard.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_discard.Dock")));
      this.button_prof_discard.Enabled = ((bool)(resources.GetObject("button_prof_discard.Enabled")));
      this.button_prof_discard.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_discard.FlatStyle")));
      this.button_prof_discard.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_discard.Font")));
      this.button_prof_discard.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_discard.Image")));
      this.button_prof_discard.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_discard.ImageAlign")));
      this.button_prof_discard.ImageIndex = ((int)(resources.GetObject("button_prof_discard.ImageIndex")));
      this.button_prof_discard.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_discard.ImeMode")));
      this.button_prof_discard.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_discard.Location")));
      this.button_prof_discard.Name = "button_prof_discard";
      this.button_prof_discard.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_discard.RightToLeft")));
      this.button_prof_discard.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_discard.Size")));
      this.button_prof_discard.TabIndex = ((int)(resources.GetObject("button_prof_discard.TabIndex")));
      this.button_prof_discard.Text = resources.GetString("button_prof_discard.Text");
      this.button_prof_discard.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_discard.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_discard, resources.GetString("button_prof_discard.ToolTip"));
      this.button_prof_discard.Visible = ((bool)(resources.GetObject("button_prof_discard.Visible")));
      this.button_prof_discard.Click += new System.EventHandler(this.button_prof_discard_Click);
      // 
      // button_prof_rename
      // 
      this.button_prof_rename.AccessibleDescription = resources.GetString("button_prof_rename.AccessibleDescription");
      this.button_prof_rename.AccessibleName = resources.GetString("button_prof_rename.AccessibleName");
      this.button_prof_rename.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_prof_rename.Anchor")));
      this.button_prof_rename.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_prof_rename.BackgroundImage")));
      this.button_prof_rename.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_prof_rename.Dock")));
      this.button_prof_rename.Enabled = ((bool)(resources.GetObject("button_prof_rename.Enabled")));
      this.button_prof_rename.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_prof_rename.FlatStyle")));
      this.button_prof_rename.Font = ((System.Drawing.Font)(resources.GetObject("button_prof_rename.Font")));
      this.button_prof_rename.Image = ((System.Drawing.Image)(resources.GetObject("button_prof_rename.Image")));
      this.button_prof_rename.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_rename.ImageAlign")));
      this.button_prof_rename.ImageIndex = ((int)(resources.GetObject("button_prof_rename.ImageIndex")));
      this.button_prof_rename.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_prof_rename.ImeMode")));
      this.button_prof_rename.Location = ((System.Drawing.Point)(resources.GetObject("button_prof_rename.Location")));
      this.button_prof_rename.Name = "button_prof_rename";
      this.button_prof_rename.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_prof_rename.RightToLeft")));
      this.button_prof_rename.Size = ((System.Drawing.Size)(resources.GetObject("button_prof_rename.Size")));
      this.button_prof_rename.TabIndex = ((int)(resources.GetObject("button_prof_rename.TabIndex")));
      this.button_prof_rename.Text = resources.GetString("button_prof_rename.Text");
      this.button_prof_rename.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_prof_rename.TextAlign")));
      this.toolTip.SetToolTip(this.button_prof_rename, resources.GetString("button_prof_rename.ToolTip"));
      this.button_prof_rename.Visible = ((bool)(resources.GetObject("button_prof_rename.Visible")));
      this.button_prof_rename.Click += new System.EventHandler(this.button_prof_rename_Click);
      // 
      // toolBar1
      // 
      this.toolBar1.AccessibleDescription = resources.GetString("toolBar1.AccessibleDescription");
      this.toolBar1.AccessibleName = resources.GetString("toolBar1.AccessibleName");
      this.toolBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("toolBar1.Anchor")));
      this.toolBar1.Appearance = ((System.Windows.Forms.ToolBarAppearance)(resources.GetObject("toolBar1.Appearance")));
      this.toolBar1.AutoSize = ((bool)(resources.GetObject("toolBar1.AutoSize")));
      this.toolBar1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolBar1.BackgroundImage")));
      this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                this.toolButton_exploreGameFolder,
                                                                                this.toolButton_editGameCfg,
                                                                                this.toolBarButton1,
                                                                                this.toolButton_tools_regEdit,
                                                                                this.toolBarButton4,
                                                                                this.toolButton_prof_commands,
                                                                                this.toolButton_hotkeys,
                                                                                this.toolBarButton2,
                                                                                this.toolButton_settings,
                                                                                this.toolBarButton3,
                                                                                this.toolButton_help_onlineManual,
                                                                                this.toolBarButton5,
                                                                                this.toolButton_compact});
      this.toolBar1.ButtonSize = ((System.Drawing.Size)(resources.GetObject("toolBar1.ButtonSize")));
      this.toolBar1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("toolBar1.Dock")));
      this.toolBar1.DropDownArrows = ((bool)(resources.GetObject("toolBar1.DropDownArrows")));
      this.toolBar1.Enabled = ((bool)(resources.GetObject("toolBar1.Enabled")));
      this.toolBar1.Font = ((System.Drawing.Font)(resources.GetObject("toolBar1.Font")));
      this.toolBar1.ImageList = this.imageList;
      this.toolBar1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("toolBar1.ImeMode")));
      this.toolBar1.Location = ((System.Drawing.Point)(resources.GetObject("toolBar1.Location")));
      this.toolBar1.Name = "toolBar1";
      this.toolBar1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("toolBar1.RightToLeft")));
      this.toolBar1.ShowToolTips = ((bool)(resources.GetObject("toolBar1.ShowToolTips")));
      this.toolBar1.Size = ((System.Drawing.Size)(resources.GetObject("toolBar1.Size")));
      this.toolBar1.TabIndex = ((int)(resources.GetObject("toolBar1.TabIndex")));
      this.toolBar1.TextAlign = ((System.Windows.Forms.ToolBarTextAlign)(resources.GetObject("toolBar1.TextAlign")));
      this.toolTip.SetToolTip(this.toolBar1, resources.GetString("toolBar1.ToolTip"));
      this.toolBar1.Visible = ((bool)(resources.GetObject("toolBar1.Visible")));
      this.toolBar1.Wrappable = ((bool)(resources.GetObject("toolBar1.Wrappable")));
      this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
      // 
      // toolButton_exploreGameFolder
      // 
      this.toolButton_exploreGameFolder.Enabled = ((bool)(resources.GetObject("toolButton_exploreGameFolder.Enabled")));
      this.toolButton_exploreGameFolder.ImageIndex = ((int)(resources.GetObject("toolButton_exploreGameFolder.ImageIndex")));
      this.toolButton_exploreGameFolder.Text = resources.GetString("toolButton_exploreGameFolder.Text");
      this.toolButton_exploreGameFolder.ToolTipText = resources.GetString("toolButton_exploreGameFolder.ToolTipText");
      this.toolButton_exploreGameFolder.Visible = ((bool)(resources.GetObject("toolButton_exploreGameFolder.Visible")));
      // 
      // toolButton_editGameCfg
      // 
      this.toolButton_editGameCfg.Enabled = ((bool)(resources.GetObject("toolButton_editGameCfg.Enabled")));
      this.toolButton_editGameCfg.ImageIndex = ((int)(resources.GetObject("toolButton_editGameCfg.ImageIndex")));
      this.toolButton_editGameCfg.Text = resources.GetString("toolButton_editGameCfg.Text");
      this.toolButton_editGameCfg.ToolTipText = resources.GetString("toolButton_editGameCfg.ToolTipText");
      this.toolButton_editGameCfg.Visible = ((bool)(resources.GetObject("toolButton_editGameCfg.Visible")));
      // 
      // toolBarButton1
      // 
      this.toolBarButton1.Enabled = ((bool)(resources.GetObject("toolBarButton1.Enabled")));
      this.toolBarButton1.ImageIndex = ((int)(resources.GetObject("toolBarButton1.ImageIndex")));
      this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      this.toolBarButton1.Text = resources.GetString("toolBarButton1.Text");
      this.toolBarButton1.ToolTipText = resources.GetString("toolBarButton1.ToolTipText");
      this.toolBarButton1.Visible = ((bool)(resources.GetObject("toolBarButton1.Visible")));
      // 
      // toolButton_tools_regEdit
      // 
      this.toolButton_tools_regEdit.Enabled = ((bool)(resources.GetObject("toolButton_tools_regEdit.Enabled")));
      this.toolButton_tools_regEdit.ImageIndex = ((int)(resources.GetObject("toolButton_tools_regEdit.ImageIndex")));
      this.toolButton_tools_regEdit.Text = resources.GetString("toolButton_tools_regEdit.Text");
      this.toolButton_tools_regEdit.ToolTipText = resources.GetString("toolButton_tools_regEdit.ToolTipText");
      this.toolButton_tools_regEdit.Visible = ((bool)(resources.GetObject("toolButton_tools_regEdit.Visible")));
      // 
      // toolBarButton4
      // 
      this.toolBarButton4.Enabled = ((bool)(resources.GetObject("toolBarButton4.Enabled")));
      this.toolBarButton4.ImageIndex = ((int)(resources.GetObject("toolBarButton4.ImageIndex")));
      this.toolBarButton4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      this.toolBarButton4.Text = resources.GetString("toolBarButton4.Text");
      this.toolBarButton4.ToolTipText = resources.GetString("toolBarButton4.ToolTipText");
      this.toolBarButton4.Visible = ((bool)(resources.GetObject("toolBarButton4.Visible")));
      // 
      // toolButton_prof_commands
      // 
      this.toolButton_prof_commands.Enabled = ((bool)(resources.GetObject("toolButton_prof_commands.Enabled")));
      this.toolButton_prof_commands.ImageIndex = ((int)(resources.GetObject("toolButton_prof_commands.ImageIndex")));
      this.toolButton_prof_commands.Text = resources.GetString("toolButton_prof_commands.Text");
      this.toolButton_prof_commands.ToolTipText = resources.GetString("toolButton_prof_commands.ToolTipText");
      this.toolButton_prof_commands.Visible = ((bool)(resources.GetObject("toolButton_prof_commands.Visible")));
      // 
      // toolButton_hotkeys
      // 
      this.toolButton_hotkeys.Enabled = ((bool)(resources.GetObject("toolButton_hotkeys.Enabled")));
      this.toolButton_hotkeys.ImageIndex = ((int)(resources.GetObject("toolButton_hotkeys.ImageIndex")));
      this.toolButton_hotkeys.Text = resources.GetString("toolButton_hotkeys.Text");
      this.toolButton_hotkeys.ToolTipText = resources.GetString("toolButton_hotkeys.ToolTipText");
      this.toolButton_hotkeys.Visible = ((bool)(resources.GetObject("toolButton_hotkeys.Visible")));
      // 
      // toolBarButton2
      // 
      this.toolBarButton2.Enabled = ((bool)(resources.GetObject("toolBarButton2.Enabled")));
      this.toolBarButton2.ImageIndex = ((int)(resources.GetObject("toolBarButton2.ImageIndex")));
      this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      this.toolBarButton2.Text = resources.GetString("toolBarButton2.Text");
      this.toolBarButton2.ToolTipText = resources.GetString("toolBarButton2.ToolTipText");
      this.toolBarButton2.Visible = ((bool)(resources.GetObject("toolBarButton2.Visible")));
      // 
      // toolButton_settings
      // 
      this.toolButton_settings.Enabled = ((bool)(resources.GetObject("toolButton_settings.Enabled")));
      this.toolButton_settings.ImageIndex = ((int)(resources.GetObject("toolButton_settings.ImageIndex")));
      this.toolButton_settings.Text = resources.GetString("toolButton_settings.Text");
      this.toolButton_settings.ToolTipText = resources.GetString("toolButton_settings.ToolTipText");
      this.toolButton_settings.Visible = ((bool)(resources.GetObject("toolButton_settings.Visible")));
      // 
      // toolBarButton3
      // 
      this.toolBarButton3.Enabled = ((bool)(resources.GetObject("toolBarButton3.Enabled")));
      this.toolBarButton3.ImageIndex = ((int)(resources.GetObject("toolBarButton3.ImageIndex")));
      this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      this.toolBarButton3.Text = resources.GetString("toolBarButton3.Text");
      this.toolBarButton3.ToolTipText = resources.GetString("toolBarButton3.ToolTipText");
      this.toolBarButton3.Visible = ((bool)(resources.GetObject("toolBarButton3.Visible")));
      // 
      // toolButton_help_onlineManual
      // 
      this.toolButton_help_onlineManual.Enabled = ((bool)(resources.GetObject("toolButton_help_onlineManual.Enabled")));
      this.toolButton_help_onlineManual.ImageIndex = ((int)(resources.GetObject("toolButton_help_onlineManual.ImageIndex")));
      this.toolButton_help_onlineManual.Text = resources.GetString("toolButton_help_onlineManual.Text");
      this.toolButton_help_onlineManual.ToolTipText = resources.GetString("toolButton_help_onlineManual.ToolTipText");
      this.toolButton_help_onlineManual.Visible = ((bool)(resources.GetObject("toolButton_help_onlineManual.Visible")));
      // 
      // toolBarButton5
      // 
      this.toolBarButton5.Enabled = ((bool)(resources.GetObject("toolBarButton5.Enabled")));
      this.toolBarButton5.ImageIndex = ((int)(resources.GetObject("toolBarButton5.ImageIndex")));
      this.toolBarButton5.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      this.toolBarButton5.Text = resources.GetString("toolBarButton5.Text");
      this.toolBarButton5.ToolTipText = resources.GetString("toolBarButton5.ToolTipText");
      this.toolBarButton5.Visible = ((bool)(resources.GetObject("toolBarButton5.Visible")));
      // 
      // toolButton_compact
      // 
      this.toolButton_compact.Enabled = ((bool)(resources.GetObject("toolButton_compact.Enabled")));
      this.toolButton_compact.ImageIndex = ((int)(resources.GetObject("toolButton_compact.ImageIndex")));
      this.toolButton_compact.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
      this.toolButton_compact.Text = resources.GetString("toolButton_compact.Text");
      this.toolButton_compact.ToolTipText = resources.GetString("toolButton_compact.ToolTipText");
      this.toolButton_compact.Visible = ((bool)(resources.GetObject("toolButton_compact.Visible")));
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageList.ImageSize = ((System.Drawing.Size)(resources.GetObject("imageList.ImageSize")));
      this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // timer_updateCr
      // 
      this.timer_updateCr.Interval = 750;
      this.timer_updateCr.Tick += new System.EventHandler(this.timer_updateCr_Tick);
      // 
      // notifyIcon_tray
      // 
      this.notifyIcon_tray.ContextMenu = this.context_tray;
      this.notifyIcon_tray.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon_tray.Icon")));
      this.notifyIcon_tray.Text = resources.GetString("notifyIcon_tray.Text");
      this.notifyIcon_tray.Visible = ((bool)(resources.GetObject("notifyIcon_tray.Visible")));
      this.notifyIcon_tray.DoubleClick += new System.EventHandler(this.notifyIcon_tray_DoubleClick);
      // 
      // context_tray
      // 
      this.context_tray.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                 this.menu_tray_profs,
                                                                                 this.menuItem26,
                                                                                 this.menuItem11,
                                                                                 this.menuItem25,
                                                                                 this.menu_tray_tools,
                                                                                 this.menuItem23,
                                                                                 this.menu_tray_stayInTray,
                                                                                 this.menu_tray_hideOnCloseBox,
                                                                                 this.menu_tray_sep,
                                                                                 this.menu_tray_quit});
      this.context_tray.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("context_tray.RightToLeft")));
      this.context_tray.Popup += new System.EventHandler(this.context_tray_Popup);
      // 
      // menu_tray_profs
      // 
      this.menu_tray_profs.Enabled = ((bool)(resources.GetObject("menu_tray_profs.Enabled")));
      this.menu_tray_profs.Index = 0;
      this.menu_tray_profs.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                    this.menu_tray_apply_exe,
                                                                                    this.menu_tray_apply,
                                                                                    this.menu_tray_profs_editGameIni,
                                                                                    this.menuItem7,
                                                                                    this.menu_tray_profs_makeLink});
      this.menu_tray_profs.OwnerDraw = true;
      this.menu_tray_profs.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_profs.Shortcut")));
      this.menu_tray_profs.ShowShortcut = ((bool)(resources.GetObject("menu_tray_profs.ShowShortcut")));
      this.menu_tray_profs.Text = resources.GetString("menu_tray_profs.Text");
      this.menu_tray_profs.Visible = ((bool)(resources.GetObject("menu_tray_profs.Visible")));
      // 
      // menu_tray_apply_exe
      // 
      this.menu_tray_apply_exe.Enabled = ((bool)(resources.GetObject("menu_tray_apply_exe.Enabled")));
      this.menu_tray_apply_exe.Index = 0;
      this.menu_tray_apply_exe.OwnerDraw = true;
      this.menu_tray_apply_exe.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_apply_exe.Shortcut")));
      this.menu_tray_apply_exe.ShowShortcut = ((bool)(resources.GetObject("menu_tray_apply_exe.ShowShortcut")));
      this.menu_tray_apply_exe.Text = resources.GetString("menu_tray_apply_exe.Text");
      this.menu_tray_apply_exe.Visible = ((bool)(resources.GetObject("menu_tray_apply_exe.Visible")));
      // 
      // menu_tray_apply
      // 
      this.menu_tray_apply.Enabled = ((bool)(resources.GetObject("menu_tray_apply.Enabled")));
      this.menu_tray_apply.Index = 1;
      this.menu_tray_apply.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                    this.menu_tray_applyExe});
      this.menu_tray_apply.OwnerDraw = true;
      this.menu_tray_apply.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_apply.Shortcut")));
      this.menu_tray_apply.ShowShortcut = ((bool)(resources.GetObject("menu_tray_apply.ShowShortcut")));
      this.menu_tray_apply.Text = resources.GetString("menu_tray_apply.Text");
      this.menu_tray_apply.Visible = ((bool)(resources.GetObject("menu_tray_apply.Visible")));
      // 
      // menu_tray_applyExe
      // 
      this.menu_tray_applyExe.Enabled = ((bool)(resources.GetObject("menu_tray_applyExe.Enabled")));
      this.menu_tray_applyExe.Index = 0;
      this.menu_tray_applyExe.OwnerDraw = true;
      this.menu_tray_applyExe.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_applyExe.Shortcut")));
      this.menu_tray_applyExe.ShowShortcut = ((bool)(resources.GetObject("menu_tray_applyExe.ShowShortcut")));
      this.menu_tray_applyExe.Text = resources.GetString("menu_tray_applyExe.Text");
      this.menu_tray_applyExe.Visible = ((bool)(resources.GetObject("menu_tray_applyExe.Visible")));
      // 
      // menu_tray_profs_editGameIni
      // 
      this.menu_tray_profs_editGameIni.Enabled = ((bool)(resources.GetObject("menu_tray_profs_editGameIni.Enabled")));
      this.menu_tray_profs_editGameIni.Index = 2;
      this.menu_tray_profs_editGameIni.OwnerDraw = true;
      this.menu_tray_profs_editGameIni.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_profs_editGameIni.Shortcut")));
      this.menu_tray_profs_editGameIni.ShowShortcut = ((bool)(resources.GetObject("menu_tray_profs_editGameIni.ShowShortcut")));
      this.menu_tray_profs_editGameIni.Text = resources.GetString("menu_tray_profs_editGameIni.Text");
      this.menu_tray_profs_editGameIni.Visible = ((bool)(resources.GetObject("menu_tray_profs_editGameIni.Visible")));
      this.menu_tray_profs_editGameIni.Popup += new System.EventHandler(this.menu_tray_profs_editGameIni_Popup);
      // 
      // menuItem7
      // 
      this.menuItem7.Enabled = ((bool)(resources.GetObject("menuItem7.Enabled")));
      this.menuItem7.Index = 3;
      this.menuItem7.OwnerDraw = true;
      this.menuItem7.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem7.Shortcut")));
      this.menuItem7.ShowShortcut = ((bool)(resources.GetObject("menuItem7.ShowShortcut")));
      this.menuItem7.Text = resources.GetString("menuItem7.Text");
      this.menuItem7.Visible = ((bool)(resources.GetObject("menuItem7.Visible")));
      // 
      // menu_tray_profs_makeLink
      // 
      this.menu_tray_profs_makeLink.Enabled = ((bool)(resources.GetObject("menu_tray_profs_makeLink.Enabled")));
      this.menu_tray_profs_makeLink.Index = 4;
      this.menu_tray_profs_makeLink.OwnerDraw = true;
      this.menu_tray_profs_makeLink.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_profs_makeLink.Shortcut")));
      this.menu_tray_profs_makeLink.ShowShortcut = ((bool)(resources.GetObject("menu_tray_profs_makeLink.ShowShortcut")));
      this.menu_tray_profs_makeLink.Text = resources.GetString("menu_tray_profs_makeLink.Text");
      this.menu_tray_profs_makeLink.Visible = ((bool)(resources.GetObject("menu_tray_profs_makeLink.Visible")));
      // 
      // menuItem26
      // 
      this.menuItem26.Enabled = ((bool)(resources.GetObject("menuItem26.Enabled")));
      this.menuItem26.Index = 1;
      this.menuItem26.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                               this.menu_tray_clocking_pre_slow,
                                                                               this.menu_tray_clocking_pre_normal,
                                                                               this.menu_tray_clocking_pre_fast,
                                                                               this.menu_tray_clocking_pre_ultra});
      this.menuItem26.OwnerDraw = true;
      this.menuItem26.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem26.Shortcut")));
      this.menuItem26.ShowShortcut = ((bool)(resources.GetObject("menuItem26.ShowShortcut")));
      this.menuItem26.Text = resources.GetString("menuItem26.Text");
      this.menuItem26.Visible = ((bool)(resources.GetObject("menuItem26.Visible")));
      // 
      // menu_tray_clocking_pre_slow
      // 
      this.menu_tray_clocking_pre_slow.Enabled = ((bool)(resources.GetObject("menu_tray_clocking_pre_slow.Enabled")));
      this.menu_tray_clocking_pre_slow.Index = 0;
      this.menu_tray_clocking_pre_slow.OwnerDraw = true;
      this.menu_tray_clocking_pre_slow.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_clocking_pre_slow.Shortcut")));
      this.menu_tray_clocking_pre_slow.ShowShortcut = ((bool)(resources.GetObject("menu_tray_clocking_pre_slow.ShowShortcut")));
      this.menu_tray_clocking_pre_slow.Text = resources.GetString("menu_tray_clocking_pre_slow.Text");
      this.menu_tray_clocking_pre_slow.Visible = ((bool)(resources.GetObject("menu_tray_clocking_pre_slow.Visible")));
      this.menu_tray_clocking_pre_slow.Click += new System.EventHandler(this.menu_tray_clocking_pre_slow_Click);
      // 
      // menu_tray_clocking_pre_normal
      // 
      this.menu_tray_clocking_pre_normal.Enabled = ((bool)(resources.GetObject("menu_tray_clocking_pre_normal.Enabled")));
      this.menu_tray_clocking_pre_normal.Index = 1;
      this.menu_tray_clocking_pre_normal.OwnerDraw = true;
      this.menu_tray_clocking_pre_normal.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_clocking_pre_normal.Shortcut")));
      this.menu_tray_clocking_pre_normal.ShowShortcut = ((bool)(resources.GetObject("menu_tray_clocking_pre_normal.ShowShortcut")));
      this.menu_tray_clocking_pre_normal.Text = resources.GetString("menu_tray_clocking_pre_normal.Text");
      this.menu_tray_clocking_pre_normal.Visible = ((bool)(resources.GetObject("menu_tray_clocking_pre_normal.Visible")));
      this.menu_tray_clocking_pre_normal.Click += new System.EventHandler(this.menu_tray_clocking_pre_normal_Click);
      // 
      // menu_tray_clocking_pre_fast
      // 
      this.menu_tray_clocking_pre_fast.Enabled = ((bool)(resources.GetObject("menu_tray_clocking_pre_fast.Enabled")));
      this.menu_tray_clocking_pre_fast.Index = 2;
      this.menu_tray_clocking_pre_fast.OwnerDraw = true;
      this.menu_tray_clocking_pre_fast.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_clocking_pre_fast.Shortcut")));
      this.menu_tray_clocking_pre_fast.ShowShortcut = ((bool)(resources.GetObject("menu_tray_clocking_pre_fast.ShowShortcut")));
      this.menu_tray_clocking_pre_fast.Text = resources.GetString("menu_tray_clocking_pre_fast.Text");
      this.menu_tray_clocking_pre_fast.Visible = ((bool)(resources.GetObject("menu_tray_clocking_pre_fast.Visible")));
      this.menu_tray_clocking_pre_fast.Click += new System.EventHandler(this.menu_tray_clocking_pre_fast_Click);
      // 
      // menu_tray_clocking_pre_ultra
      // 
      this.menu_tray_clocking_pre_ultra.Enabled = ((bool)(resources.GetObject("menu_tray_clocking_pre_ultra.Enabled")));
      this.menu_tray_clocking_pre_ultra.Index = 3;
      this.menu_tray_clocking_pre_ultra.OwnerDraw = true;
      this.menu_tray_clocking_pre_ultra.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_clocking_pre_ultra.Shortcut")));
      this.menu_tray_clocking_pre_ultra.ShowShortcut = ((bool)(resources.GetObject("menu_tray_clocking_pre_ultra.ShowShortcut")));
      this.menu_tray_clocking_pre_ultra.Text = resources.GetString("menu_tray_clocking_pre_ultra.Text");
      this.menu_tray_clocking_pre_ultra.Visible = ((bool)(resources.GetObject("menu_tray_clocking_pre_ultra.Visible")));
      this.menu_tray_clocking_pre_ultra.Click += new System.EventHandler(this.menu_tray_clocking_pre_ultra_Click);
      // 
      // menuItem11
      // 
      this.menuItem11.Enabled = ((bool)(resources.GetObject("menuItem11.Enabled")));
      this.menuItem11.Index = 2;
      this.menuItem11.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                               this.menu_tray_img_mountCurr,
                                                                               this.menu_tray_img_mountAnImgAtD0});
      this.menuItem11.OwnerDraw = true;
      this.menuItem11.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem11.Shortcut")));
      this.menuItem11.ShowShortcut = ((bool)(resources.GetObject("menuItem11.ShowShortcut")));
      this.menuItem11.Text = resources.GetString("menuItem11.Text");
      this.menuItem11.Visible = ((bool)(resources.GetObject("menuItem11.Visible")));
      // 
      // menu_tray_img_mountCurr
      // 
      this.menu_tray_img_mountCurr.Enabled = ((bool)(resources.GetObject("menu_tray_img_mountCurr.Enabled")));
      this.menu_tray_img_mountCurr.Index = 0;
      this.menu_tray_img_mountCurr.OwnerDraw = true;
      this.menu_tray_img_mountCurr.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_img_mountCurr.Shortcut")));
      this.menu_tray_img_mountCurr.ShowShortcut = ((bool)(resources.GetObject("menu_tray_img_mountCurr.ShowShortcut")));
      this.menu_tray_img_mountCurr.Text = resources.GetString("menu_tray_img_mountCurr.Text");
      this.menu_tray_img_mountCurr.Visible = ((bool)(resources.GetObject("menu_tray_img_mountCurr.Visible")));
      this.menu_tray_img_mountCurr.Click += new System.EventHandler(this.menu_tray_img_mountCurr_Click);
      // 
      // menu_tray_img_mountAnImgAtD0
      // 
      this.menu_tray_img_mountAnImgAtD0.Enabled = ((bool)(resources.GetObject("menu_tray_img_mountAnImgAtD0.Enabled")));
      this.menu_tray_img_mountAnImgAtD0.Index = 1;
      this.menu_tray_img_mountAnImgAtD0.OwnerDraw = true;
      this.menu_tray_img_mountAnImgAtD0.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_img_mountAnImgAtD0.Shortcut")));
      this.menu_tray_img_mountAnImgAtD0.ShowShortcut = ((bool)(resources.GetObject("menu_tray_img_mountAnImgAtD0.ShowShortcut")));
      this.menu_tray_img_mountAnImgAtD0.Text = resources.GetString("menu_tray_img_mountAnImgAtD0.Text");
      this.menu_tray_img_mountAnImgAtD0.Visible = ((bool)(resources.GetObject("menu_tray_img_mountAnImgAtD0.Visible")));
      // 
      // menuItem25
      // 
      this.menuItem25.Enabled = ((bool)(resources.GetObject("menuItem25.Enabled")));
      this.menuItem25.Index = 3;
      this.menuItem25.OwnerDraw = true;
      this.menuItem25.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem25.Shortcut")));
      this.menuItem25.ShowShortcut = ((bool)(resources.GetObject("menuItem25.ShowShortcut")));
      this.menuItem25.Text = resources.GetString("menuItem25.Text");
      this.menuItem25.Visible = ((bool)(resources.GetObject("menuItem25.Visible")));
      // 
      // menu_tray_tools
      // 
      this.menu_tray_tools.Enabled = ((bool)(resources.GetObject("menu_tray_tools.Enabled")));
      this.menu_tray_tools.Index = 4;
      this.menu_tray_tools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                    this.menu_tray_tools_regEdit,
                                                                                    this.menu_tray_tools_regDiff});
      this.menu_tray_tools.OwnerDraw = true;
      this.menu_tray_tools.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_tools.Shortcut")));
      this.menu_tray_tools.ShowShortcut = ((bool)(resources.GetObject("menu_tray_tools.ShowShortcut")));
      this.menu_tray_tools.Text = resources.GetString("menu_tray_tools.Text");
      this.menu_tray_tools.Visible = ((bool)(resources.GetObject("menu_tray_tools.Visible")));
      // 
      // menu_tray_tools_regEdit
      // 
      this.menu_tray_tools_regEdit.Enabled = ((bool)(resources.GetObject("menu_tray_tools_regEdit.Enabled")));
      this.menu_tray_tools_regEdit.Index = 0;
      this.menu_tray_tools_regEdit.OwnerDraw = true;
      this.menu_tray_tools_regEdit.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_tools_regEdit.Shortcut")));
      this.menu_tray_tools_regEdit.ShowShortcut = ((bool)(resources.GetObject("menu_tray_tools_regEdit.ShowShortcut")));
      this.menu_tray_tools_regEdit.Text = resources.GetString("menu_tray_tools_regEdit.Text");
      this.menu_tray_tools_regEdit.Visible = ((bool)(resources.GetObject("menu_tray_tools_regEdit.Visible")));
      this.menu_tray_tools_regEdit.Click += new System.EventHandler(this.menu_tools_openRegedit_Click);
      // 
      // menu_tray_tools_regDiff
      // 
      this.menu_tray_tools_regDiff.Enabled = ((bool)(resources.GetObject("menu_tray_tools_regDiff.Enabled")));
      this.menu_tray_tools_regDiff.Index = 1;
      this.menu_tray_tools_regDiff.OwnerDraw = true;
      this.menu_tray_tools_regDiff.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_tools_regDiff.Shortcut")));
      this.menu_tray_tools_regDiff.ShowShortcut = ((bool)(resources.GetObject("menu_tray_tools_regDiff.ShowShortcut")));
      this.menu_tray_tools_regDiff.Text = resources.GetString("menu_tray_tools_regDiff.Text");
      this.menu_tray_tools_regDiff.Visible = ((bool)(resources.GetObject("menu_tray_tools_regDiff.Visible")));
      this.menu_tray_tools_regDiff.Click += new System.EventHandler(this.menu_exp_testRdKey_Click);
      // 
      // menuItem23
      // 
      this.menuItem23.Enabled = ((bool)(resources.GetObject("menuItem23.Enabled")));
      this.menuItem23.Index = 5;
      this.menuItem23.OwnerDraw = true;
      this.menuItem23.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem23.Shortcut")));
      this.menuItem23.ShowShortcut = ((bool)(resources.GetObject("menuItem23.ShowShortcut")));
      this.menuItem23.Text = resources.GetString("menuItem23.Text");
      this.menuItem23.Visible = ((bool)(resources.GetObject("menuItem23.Visible")));
      // 
      // menu_tray_stayInTray
      // 
      this.menu_tray_stayInTray.Enabled = ((bool)(resources.GetObject("menu_tray_stayInTray.Enabled")));
      this.menu_tray_stayInTray.Index = 6;
      this.menu_tray_stayInTray.OwnerDraw = true;
      this.menu_tray_stayInTray.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_stayInTray.Shortcut")));
      this.menu_tray_stayInTray.ShowShortcut = ((bool)(resources.GetObject("menu_tray_stayInTray.ShowShortcut")));
      this.menu_tray_stayInTray.Text = resources.GetString("menu_tray_stayInTray.Text");
      this.menu_tray_stayInTray.Visible = ((bool)(resources.GetObject("menu_tray_stayInTray.Visible")));
      this.menu_tray_stayInTray.Click += new System.EventHandler(this.menu_tray_stayInTray_Click);
      // 
      // menu_tray_hideOnCloseBox
      // 
      this.menu_tray_hideOnCloseBox.Enabled = ((bool)(resources.GetObject("menu_tray_hideOnCloseBox.Enabled")));
      this.menu_tray_hideOnCloseBox.Index = 7;
      this.menu_tray_hideOnCloseBox.OwnerDraw = true;
      this.menu_tray_hideOnCloseBox.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_hideOnCloseBox.Shortcut")));
      this.menu_tray_hideOnCloseBox.ShowShortcut = ((bool)(resources.GetObject("menu_tray_hideOnCloseBox.ShowShortcut")));
      this.menu_tray_hideOnCloseBox.Text = resources.GetString("menu_tray_hideOnCloseBox.Text");
      this.menu_tray_hideOnCloseBox.Visible = ((bool)(resources.GetObject("menu_tray_hideOnCloseBox.Visible")));
      this.menu_tray_hideOnCloseBox.Click += new System.EventHandler(this.menu_tray_hideOnCloseBox_Click);
      // 
      // menu_tray_sep
      // 
      this.menu_tray_sep.Enabled = ((bool)(resources.GetObject("menu_tray_sep.Enabled")));
      this.menu_tray_sep.Index = 8;
      this.menu_tray_sep.OwnerDraw = true;
      this.menu_tray_sep.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_sep.Shortcut")));
      this.menu_tray_sep.ShowShortcut = ((bool)(resources.GetObject("menu_tray_sep.ShowShortcut")));
      this.menu_tray_sep.Text = resources.GetString("menu_tray_sep.Text");
      this.menu_tray_sep.Visible = ((bool)(resources.GetObject("menu_tray_sep.Visible")));
      // 
      // menu_tray_quit
      // 
      this.menu_tray_quit.Enabled = ((bool)(resources.GetObject("menu_tray_quit.Enabled")));
      this.menu_tray_quit.Index = 9;
      this.menu_tray_quit.OwnerDraw = true;
      this.menu_tray_quit.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menu_tray_quit.Shortcut")));
      this.menu_tray_quit.ShowShortcut = ((bool)(resources.GetObject("menu_tray_quit.ShowShortcut")));
      this.menu_tray_quit.Text = resources.GetString("menu_tray_quit.Text");
      this.menu_tray_quit.Visible = ((bool)(resources.GetObject("menu_tray_quit.Visible")));
      this.menu_tray_quit.Click += new System.EventHandler(this.menu_file_quit_Click);
      // 
      // FormMain
      // 
      this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
      this.AccessibleName = resources.GetString("$this.AccessibleName");
      this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
      this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
      this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
      this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
      this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
      this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
      this.Controls.Add(this.group_prof);
      this.Controls.Add(this.tabCtrl);
      this.Controls.Add(this.toolBar1);
      this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
      this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
      this.KeyPreview = true;
      this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
      this.MaximizeBox = false;
      this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
      this.Menu = this.mainMenu;
      this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
      this.Name = "FormMain";
      this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
      this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
      this.Text = resources.GetString("$this.Text");
      this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
      this.Closing += new System.ComponentModel.CancelEventHandler(this.FormMain_Closing);
      this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
      this.Load += new System.EventHandler(this.FormMain_Load);
      this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyUp);
      this.Closed += new System.EventHandler(this.FormMain_Closed);
      this.Activated += new System.EventHandler(this.FormMain_Activated);
      this.group_main_d3d.ResumeLayout(false);
      this.group_extra_d3d.ResumeLayout(false);
      this.group_extra_ogl.ResumeLayout(false);
      this.tabCtrl.ResumeLayout(false);
      this.tab_main.ResumeLayout(false);
      this.tab_files.ResumeLayout(false);
      this.panel_prof_files.ResumeLayout(false);
      this.panel_gameExe.ResumeLayout(false);
      this.tab_extra_d3d.ResumeLayout(false);
      this.group_extra_d3d_2.ResumeLayout(false);
      this.tab_extra_ogl.ResumeLayout(false);
      this.group_extra_ogl_2.ResumeLayout(false);
      this.tab_summary.ResumeLayout(false);
      this.tab_clocking.ResumeLayout(false);
      this.group_clocking_curr.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.track_clocking_curr_mem)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.track_clocking_curr_core)).EndInit();
      this.group_clocking_current_presets.ResumeLayout(false);
      this.group_clocking_prof.ResumeLayout(false);
      this.panel_clocking_prof_clocks.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.track_clocking_prof_mem)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.track_clocking_prof_core)).EndInit();
      this.tab_exp.ResumeLayout(false);
      this.panel_prof_apply.ResumeLayout(false);
      this.group_prof.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion
    #region *** Widget Defintions ***
    private System.Windows.Forms.Label label_clocking_prof_core;
    private System.Windows.Forms.Label label_clocking_prof_mem;
    private System.Windows.Forms.CheckBox check_clocking_prof_core;
    private System.Windows.Forms.CheckBox check_clocking_prof_mem;
    private System.Windows.Forms.GroupBox group_clocking_prof;
    private System.Windows.Forms.GroupBox group_clocking_curr;
    private System.Windows.Forms.Button button_clocking_reset;
    private System.Windows.Forms.MenuItem menu_prof_prio;
    private System.Windows.Forms.MenuItem menu_prof_prio_idle;
    private System.Windows.Forms.MenuItem menu_prof_prio_belowNormal;
    private System.Windows.Forms.MenuItem menu_prof_prio_normal;
    private System.Windows.Forms.MenuItem menu_prof_prio_aboveNormal;
    private System.Windows.Forms.MenuItem menu_prof_prio_high;
    private System.Windows.Forms.TabPage tab_summary;
    private System.Windows.Forms.Label label_extra2_prof_ogl;
    private System.Windows.Forms.Label label_extra2_prof_d3d;
    private System.Windows.Forms.Label label_extra2_curr_ogl;
    private System.Windows.Forms.Label label_extra2_curr_d3d;
    private System.Windows.Forms.Label label_extra_combo_ogl_1;
    private System.Windows.Forms.Label label_extra_combo_ogl_2;
    private System.Windows.Forms.Label label_extra_combo_ogl_3;
    private System.Windows.Forms.Label label_extra_combo_ogl_4;
    private System.Windows.Forms.Label label_extra_combo_ogl_5;
    private System.Windows.Forms.Label label_extra_combo_ogl_6;
    private System.Windows.Forms.Label label_extra_combo_d3d_1;
    private System.Windows.Forms.Label label_extra_combo_d3d_4;
    private System.Windows.Forms.Label label_extra_combo_d3d_5;
    private System.Windows.Forms.Label label_extra_combo_d3d_3;
    private System.Windows.Forms.Label label_extra_combo_d3d_2;
    private System.Windows.Forms.Label label_extra_combo_d3d_6;
    private System.ComponentModel.IContainer components;
    private System.Windows.Forms.Button button_prof_apply;
    private System.Windows.Forms.Button button_prof_apply_and_run;
    private System.Windows.Forms.Button button_prof_choose_exe;
    private System.Windows.Forms.Button button_prof_choose_img;
    private System.Windows.Forms.Button button_prof_delete;
    private System.Windows.Forms.Button button_prof_make_link;
    private System.Windows.Forms.Button button_prof_mount_img;
    private System.Windows.Forms.Button button_prof_run_exe;
    private System.Windows.Forms.Button button_prof_save;
    private System.Windows.Forms.CheckBox check_prof_quit;
    private System.Windows.Forms.ComboBox combo_d3d_aniso_mode;
    private System.Windows.Forms.ComboBox combo_d3d_fsaa_mode;
    private System.Windows.Forms.ComboBox combo_d3d_lod_bias;
    private System.Windows.Forms.ComboBox combo_d3d_prerender_frames;
    private System.Windows.Forms.ComboBox combo_d3d_qe_mode;
    private System.Windows.Forms.ComboBox combo_d3d_vsync_mode;
    private System.Windows.Forms.ComboBox combo_extra2_curr_d3d_1;
    private System.Windows.Forms.ComboBox combo_extra2_curr_d3d_2;
    private System.Windows.Forms.ComboBox combo_extra2_curr_d3d_3;
    private System.Windows.Forms.ComboBox combo_extra2_curr_d3d_4;
    private System.Windows.Forms.ComboBox combo_extra2_curr_d3d_5;
    private System.Windows.Forms.ComboBox combo_extra2_curr_d3d_6;
    private System.Windows.Forms.ComboBox combo_extra2_curr_ogl_1;
    private System.Windows.Forms.ComboBox combo_extra2_curr_ogl_2;
    private System.Windows.Forms.ComboBox combo_extra2_curr_ogl_3;
    private System.Windows.Forms.ComboBox combo_extra2_curr_ogl_4;
    private System.Windows.Forms.ComboBox combo_extra2_curr_ogl_5;
    private System.Windows.Forms.ComboBox combo_extra2_curr_ogl_6;
    private System.Windows.Forms.ComboBox combo_extra2_prof_d3d_1;
    private System.Windows.Forms.ComboBox combo_extra2_prof_d3d_2;
    private System.Windows.Forms.ComboBox combo_extra2_prof_d3d_3;
    private System.Windows.Forms.ComboBox combo_extra2_prof_d3d_4;
    private System.Windows.Forms.ComboBox combo_extra2_prof_d3d_5;
    private System.Windows.Forms.ComboBox combo_extra2_prof_d3d_6;
    private System.Windows.Forms.ComboBox combo_extra2_prof_ogl_1;
    private System.Windows.Forms.ComboBox combo_extra2_prof_ogl_2;
    private System.Windows.Forms.ComboBox combo_extra2_prof_ogl_3;
    private System.Windows.Forms.ComboBox combo_extra2_prof_ogl_4;
    private System.Windows.Forms.ComboBox combo_extra2_prof_ogl_5;
    private System.Windows.Forms.ComboBox combo_extra2_prof_ogl_6;
    private System.Windows.Forms.ComboBox combo_extra_curr_d3d_1;
    private System.Windows.Forms.ComboBox combo_extra_curr_d3d_2;
    private System.Windows.Forms.ComboBox combo_extra_curr_d3d_3;
    private System.Windows.Forms.ComboBox combo_extra_curr_d3d_4;
    private System.Windows.Forms.ComboBox combo_extra_curr_d3d_5;
    private System.Windows.Forms.ComboBox combo_extra_curr_d3d_6;
    private System.Windows.Forms.ComboBox combo_extra_curr_ogl_1;
    private System.Windows.Forms.ComboBox combo_extra_curr_ogl_2;
    private System.Windows.Forms.ComboBox combo_extra_curr_ogl_3;
    private System.Windows.Forms.ComboBox combo_extra_curr_ogl_4;
    private System.Windows.Forms.ComboBox combo_extra_curr_ogl_5;
    private System.Windows.Forms.ComboBox combo_extra_curr_ogl_6;
    private System.Windows.Forms.ComboBox combo_extra_prof_d3d_1;
    private System.Windows.Forms.ComboBox combo_extra_prof_d3d_2;
    private System.Windows.Forms.ComboBox combo_extra_prof_d3d_3;
    private System.Windows.Forms.ComboBox combo_extra_prof_d3d_4;
    private System.Windows.Forms.ComboBox combo_extra_prof_d3d_5;
    private System.Windows.Forms.ComboBox combo_extra_prof_d3d_6;
    private System.Windows.Forms.ComboBox combo_extra_prof_ogl_1;
    private System.Windows.Forms.ComboBox combo_extra_prof_ogl_2;
    private System.Windows.Forms.ComboBox combo_extra_prof_ogl_3;
    private System.Windows.Forms.ComboBox combo_extra_prof_ogl_4;
    private System.Windows.Forms.ComboBox combo_extra_prof_ogl_5;
    private System.Windows.Forms.ComboBox combo_extra_prof_ogl_6;
    private System.Windows.Forms.ComboBox combo_ogl_aniso_mode;
    private System.Windows.Forms.ComboBox combo_ogl_fsaa_mode;
    private System.Windows.Forms.ComboBox combo_ogl_lod_bias;
    private System.Windows.Forms.ComboBox combo_ogl_prerender_frames;
    private System.Windows.Forms.ComboBox combo_ogl_qe_mode;
    private System.Windows.Forms.ComboBox combo_ogl_vsync_mode;
    private System.Windows.Forms.ComboBox combo_prof_d3d_aniso_mode;
    private System.Windows.Forms.ComboBox combo_prof_d3d_fsaa_mode;
    private System.Windows.Forms.ComboBox combo_prof_d3d_lod_bias;
    private System.Windows.Forms.ComboBox combo_prof_d3d_prerender_frames;
    private System.Windows.Forms.ComboBox combo_prof_d3d_qe_mode;
    private System.Windows.Forms.ComboBox combo_prof_d3d_vsync_mode;
    private System.Windows.Forms.ComboBox combo_prof_names;
    private System.Windows.Forms.ComboBox combo_prof_ogl_aniso_mode;
    private System.Windows.Forms.ComboBox combo_prof_ogl_fsaa_mode;
    private System.Windows.Forms.ComboBox combo_prof_ogl_lod_bias;
    private System.Windows.Forms.ComboBox combo_prof_ogl_prerender_frames;
    private System.Windows.Forms.ComboBox combo_prof_ogl_qe_mode;
    private System.Windows.Forms.ComboBox combo_prof_ogl_vsync_mode;
    private System.Windows.Forms.Label label_aniso_mode;
    private System.Windows.Forms.Label label_d3d;
    private System.Windows.Forms.Label label_extra_curr_d3d;
    private System.Windows.Forms.Label label_extra_curr_ogl;
    private System.Windows.Forms.Label label_extra_prof_d3d;
    private System.Windows.Forms.Label label_extra_prof_ogl;
    private System.Windows.Forms.Label label_fsaa_mode;
    private System.Windows.Forms.Label label_lod_bias;
    private System.Windows.Forms.Label label_ogl;
    private System.Windows.Forms.Label label_prerender_frames;
    private System.Windows.Forms.Label label_prof_d3d;
    private System.Windows.Forms.Label label_prof_ogl;
    private System.Windows.Forms.Label label_quality;
    private System.Windows.Forms.Label label_vsync_mode;
    private System.Windows.Forms.MainMenu mainMenu;
    private System.Windows.Forms.MenuItem menuItem2;
    private System.Windows.Forms.MenuItem menu_ati_D3DApply;
    private System.Windows.Forms.MenuItem menu_file;
    private System.Windows.Forms.MenuItem menu_file_debugCrashMe;
    private System.Windows.Forms.MenuItem menu_file_loadprofs;
    private System.Windows.Forms.MenuItem menu_file_quit;
    private System.Windows.Forms.MenuItem menu_file_reloadCurrDriverSettings;
    private System.Windows.Forms.MenuItem menu_file_restore3d;
    private System.Windows.Forms.MenuItem menu_file_restore3d_loadAutoSaved;
    private System.Windows.Forms.MenuItem menu_file_restore3d_loadLastSaved;
    private System.Windows.Forms.MenuItem menu_file_restore3d_loadSavedByRun;
    private System.Windows.Forms.MenuItem menu_file_restore3d_saveToFile;
    private System.Windows.Forms.MenuItem menu_help;
    private System.Windows.Forms.MenuItem menu_help_about;
    private System.Windows.Forms.MenuItem menu_help_nonWarranty;
    private System.Windows.Forms.MenuItem menu_help_showLicense;
    private System.Windows.Forms.MenuItem menu_help_tooltips;
    private System.Windows.Forms.MenuItem menu_opt_lang_de;
    private System.Windows.Forms.MenuItem menu_opt_lang_en;
    private System.Windows.Forms.MenuItem menu_options;
    private System.Windows.Forms.MenuItem menu_opts_regreadonly;
    private System.Windows.Forms.MenuItem menu_opts_settings;
    private System.Windows.Forms.MenuItem menu_prof_daemon_drive_nmb;
    private System.Windows.Forms.MenuItem menu_prof_daemon_drive_nmb_0;
    private System.Windows.Forms.MenuItem menu_prof_daemon_drive_nmb_1;
    private System.Windows.Forms.MenuItem menu_prof_daemon_drive_nmb_2;
    private System.Windows.Forms.MenuItem menu_prof_daemon_drive_nmb_3;
    private System.Windows.Forms.MenuItem menu_prof_exploreExePath;
    private System.Windows.Forms.MenuItem menu_prof_setSpecName;
    private System.Windows.Forms.MenuItem menu_profile;
    private System.Windows.Forms.MenuItem menu_profile_ini;
    private System.Windows.Forms.MenuItem menu_profile_ini_edit;
    private System.Windows.Forms.MenuItem menu_profile_ini_find;
    private System.Windows.Forms.MenuItem menu_tools;
    private System.Windows.Forms.MenuItem menu_tools_openRegedit;
    private System.Windows.Forms.MenuItem menu_tools_undoApply;
    private System.Windows.Forms.OpenFileDialog dialog_prof_choose_exec;
    private System.Windows.Forms.TabControl tabCtrl;
    private System.Windows.Forms.TabPage tab_main;
    private System.Windows.Forms.TextBox text_prof_exe_args;
    private System.Windows.Forms.TextBox text_prof_exe_path;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Label label_extra2_combo_d3d_2;
    private System.Windows.Forms.Label label_extra2_combo_d3d_3;
    private System.Windows.Forms.Label label_extra2_combo_d3d_5;
    private System.Windows.Forms.Label label_extra2_combo_d3d_4;
    private System.Windows.Forms.Label label_extra2_combo_d3d_6;
    private System.Windows.Forms.Label label_extra2_combo_d3d_1;
    private System.Windows.Forms.Label label_extra2_combo_ogl_1;
    private System.Windows.Forms.Label label_extra2_combo_ogl_4;
    private System.Windows.Forms.Label label_extra2_combo_ogl_5;
    private System.Windows.Forms.Label label_extra2_combo_ogl_3;
    private System.Windows.Forms.Label label_extra2_combo_ogl_2;
    private System.Windows.Forms.Label label_extra2_combo_ogl_6;
    private System.Windows.Forms.TabPage tab_extra_d3d;
    private System.Windows.Forms.TabPage tab_extra_ogl;
    private System.Windows.Forms.Label label_extra_combo_d3d_7;
    private System.Windows.Forms.ComboBox combo_extra_curr_d3d_7;
    private System.Windows.Forms.ComboBox combo_extra_prof_d3d_7;
    private System.Windows.Forms.ComboBox combo_extra2_curr_d3d_7;
    private System.Windows.Forms.ComboBox combo_extra2_prof_d3d_7;
    private System.Windows.Forms.Label label_extra2_combo_d3d_7;
    private System.Windows.Forms.Label label_extra_combo_d3d_8;
    private System.Windows.Forms.ComboBox combo_extra_curr_d3d_8;
    private System.Windows.Forms.ComboBox combo_extra_prof_d3d_8;
    private System.Windows.Forms.ComboBox combo_extra2_curr_d3d_8;
    private System.Windows.Forms.ComboBox combo_extra2_prof_d3d_8;
    private System.Windows.Forms.Label label_extra2_combo_d3d_8;
    private System.Windows.Forms.Label label_extra2_combo_ogl_7;
    private System.Windows.Forms.ComboBox combo_extra_prof_ogl_7;
    private System.Windows.Forms.Label label_extra_combo_ogl_7;
    private System.Windows.Forms.ComboBox combo_extra_curr_ogl_7;
    private System.Windows.Forms.ComboBox combo_extra2_curr_ogl_7;
    private System.Windows.Forms.ComboBox combo_extra2_prof_ogl_7;
    private System.Windows.Forms.Label label_extra2_combo_ogl_8;
    private System.Windows.Forms.ComboBox combo_extra_prof_ogl_8;
    private System.Windows.Forms.Label label_extra_combo_ogl_8;
    private System.Windows.Forms.ComboBox combo_extra_curr_ogl_8;
    private System.Windows.Forms.ComboBox combo_extra2_curr_ogl_8;
    private System.Windows.Forms.ComboBox combo_extra2_prof_ogl_8;
    private System.Windows.Forms.MenuItem menuItem3;
    private System.Windows.Forms.MenuItem menuItem5;
    private System.Windows.Forms.Timer timer_updateCr;
    private System.Windows.Forms.Button button_prof_new;
    private System.Windows.Forms.Button button_prof_cancel;
    private System.Windows.Forms.Button button_prof_ok;
    private System.Windows.Forms.Button button_prof_clone;
    private System.Windows.Forms.TextBox text_prof_name;
    private System.Windows.Forms.Button button_clocking_refresh;
    private System.Windows.Forms.Button button_clocking_set;
    private System.Windows.Forms.TextBox text_clocking_prof_mem;
    private System.Windows.Forms.TextBox text_clocking_prof_core;
    private System.Windows.Forms.TextBox text_clocking_curr_mem;
    private System.Windows.Forms.TextBox text_clocking_curr_core;
    private System.Windows.Forms.TrackBar track_clocking_curr_core;
    private System.Windows.Forms.TrackBar track_clocking_curr_mem;
    private System.Windows.Forms.TrackBar track_clocking_prof_core;
    private System.Windows.Forms.TrackBar track_clocking_prof_mem;
    private System.Windows.Forms.TabPage tab_clocking;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.RichTextBox text_summary;
    private System.Windows.Forms.MenuItem menu_prof_detectAPI;
    private System.Windows.Forms.MenuItem menu_prof_freeMem;
    private System.Windows.Forms.MenuItem menu_prof_freeMem_none;
    private System.Windows.Forms.MenuItem menu_prof_freeMem_64mb;
    private System.Windows.Forms.MenuItem menu_prof_freeMem_128mb;
    private System.Windows.Forms.MenuItem menu_prof_freeMem_256mb;
    private System.Windows.Forms.MenuItem menu_prof_freeMem_384mb;
    private System.Windows.Forms.MenuItem menu_prof_freeMem_512mb;
    private System.Windows.Forms.MenuItem menu_prof_freeMem_max;
    private System.Windows.Forms.MenuItem menuItem6;
    private System.Windows.Forms.MenuItem menu_prof_tdprofGD;
    private System.Windows.Forms.MenuItem menu_prof_tdprofGD_create;
    private System.Windows.Forms.MenuItem menu_prof_tdprofGD_remove;
    private System.Windows.Forms.MenuItem menu_prof_tdprofGD_help;
    private System.Windows.Forms.NotifyIcon notifyIcon_tray;
    private System.Windows.Forms.ContextMenu context_tray;
    private System.Windows.Forms.MenuItem menu_tray_apply_exe;
    private System.Windows.Forms.MenuItem menu_tray_tools;
    private System.Windows.Forms.MenuItem menu_tray_tools_regEdit;
    private System.Windows.Forms.MenuItem menu_tray_tools_regDiff;
    private System.Windows.Forms.MenuItem menu_tray_quit;
    private System.Windows.Forms.MenuItem menu_tray_sep;
    private System.Windows.Forms.MenuItem menu_tray_profs;
    private System.Windows.Forms.MenuItem menu_tray_apply;
    private System.Windows.Forms.MenuItem menu_tray_applyExe;
    private System.Windows.Forms.MenuItem menu_opts_multiUser;
    private System.Windows.Forms.MenuItem menuItem8;
    private System.Windows.Forms.MenuItem menu_tray_profs_makeLink;
    private System.Windows.Forms.MenuItem menuItem7;
    private System.Windows.Forms.MenuItem menu_opts_autoStart;
    private System.Windows.Forms.MenuItem menu_help_sysInfo;
    private System.Windows.Forms.ComboBox combo_prof_img;
    private System.Windows.Forms.MenuItem menuItem9;
    private System.Windows.Forms.MenuItem menuItem10;
    private System.Windows.Forms.MenuItem menu_exp;
    private System.Windows.Forms.MenuItem menuI_exp_findProc;
    private System.Windows.Forms.MenuItem menuItem11;
    private System.Windows.Forms.MenuItem menu_tray_img_mountAnImgAtD0;
    private System.Windows.Forms.MenuItem menu_tray_img_mountCurr;
    private System.Windows.Forms.MenuItem menuItem12;
    private System.Windows.Forms.MenuItem menu_prof_img_file_replace;
    private System.Windows.Forms.MenuItem menu_prof_img_file_add;
    private System.Windows.Forms.MenuItem menu_prof_img_file_remove;
    private System.Windows.Forms.MenuItem menu_prof_img_file_replaceAll;
    private System.Windows.Forms.MenuItem menu_prof_img_file_removeAll;
    private System.Windows.Forms.MenuItem menuItem14;
    private System.Windows.Forms.MenuItem menu_help_helpButton;
    private System.Windows.Forms.MenuItem menuItem16;
    private System.Windows.Forms.MenuItem menu_file_iconifyTray;
    private System.Windows.Forms.MenuItem menu_exp_test1;
    private System.Windows.Forms.MenuItem menu_sfEdit;
    private System.Windows.Forms.MenuItem menu_tray_profs_editGameIni;
    private System.Windows.Forms.MenuItem menuItem15;
    private System.Windows.Forms.GroupBox group_main_d3d;
    private System.Windows.Forms.GroupBox group_extra_d3d;
    private System.Windows.Forms.GroupBox group_extra_ogl;
    private System.Windows.Forms.MenuItem menu_tools_nvclock_log;
    private System.Windows.Forms.MenuItem menuItem19;
    private System.Windows.Forms.MenuItem menuItem20;
    private System.Windows.Forms.MenuItem menu_tools_nvclock;
    private System.Windows.Forms.MenuItem menu_profs_cmds;
    private System.Windows.Forms.MenuItem menu_opt_lang_auto;
    private System.Windows.Forms.MenuItem menu_profs;
    private System.Windows.Forms.MenuItem menu_profs_filterBySpecName;
    private System.Windows.Forms.MenuItem menuItem1;
    private System.Windows.Forms.MenuItem menu_tools_regdiff_ati;
    private System.Windows.Forms.MenuItem menu_prof_autoRestore;
    private System.Windows.Forms.MenuItem menu_prof_autoRestore_forceDialog;
    private System.Windows.Forms.MenuItem menu_prof_autoRestore_disableDialog;
    private System.Windows.Forms.MenuItem menuItem18;
    private System.Windows.Forms.MenuItem menu_help_visit_home;
    private System.Windows.Forms.MenuItem menu_help_visit_thread;
    private System.Windows.Forms.MenuItem menu_help_visit_thread_3DC;
    private System.Windows.Forms.MenuItem menu_help_visit_thread_R3D;
    private System.Windows.Forms.MenuItem menu_help_mailto_author;
    private System.Windows.Forms.MenuItem menu_prof_imageFiles;
    private System.Windows.Forms.ImageList imageList;
    private Chris.Beckett.MenuImageLib.MenuImage menuImage;
    private System.Windows.Forms.MenuItem menu_opts_menuIcons;
    private System.Windows.Forms.MenuItem menu_help_news;
    private System.Windows.Forms.MenuItem menu_opts_lang;
    private System.Windows.Forms.MenuItem menuItem4;
    private System.Windows.Forms.MenuItem menu_help_todo;
    private System.Windows.Forms.MenuItem menu_help_intro;
    private System.Windows.Forms.MenuItem menu_help_ati_visit_radeonFAQ;
    private System.Windows.Forms.MenuItem menu_prof_importProfile;
    private System.Windows.Forms.MenuItem menuItem13;
    private System.Windows.Forms.MenuItem menuItem22;
    private System.Windows.Forms.MenuItem menu_ati_open_cpl;
    private System.Windows.Forms.MenuItem menu_ati_open_oldCpl;
    private System.Windows.Forms.MenuItem menu_opt_3DCheckBoxes;
    private System.Windows.Forms.MenuItem menuItem17;
    private System.Windows.Forms.MenuItem menu_prof_autoRestore_forceOff;
    private System.Windows.Forms.MenuItem menu_prof_autoRestore_default;
    private System.Windows.Forms.MenuItem menu_ati_lsc;
    private System.Windows.Forms.MenuItem menu_winTweaks;
    private System.Windows.Forms.MenuItem menu_winTweak_disablePageExecutive;
    private System.Windows.Forms.MenuItem menuItem21;
    private System.Windows.Forms.MenuItem menu_prof_mouseWare;
    private System.Windows.Forms.MenuItem menu_prof_mouseWare_noAccel;
    private System.Windows.Forms.MenuItem menu_winTweaks_Separator;
    private System.Windows.Forms.MenuItem menu_help_report;
    private System.Windows.Forms.MenuItem menu_help_report_nvclockDebug;
    private System.Windows.Forms.MenuItem menu_help_report_r6clockDebug;
    private System.Windows.Forms.MenuItem menu_help_report_displayInfo;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.MenuItem menuItem23;
    private System.Windows.Forms.MenuItem menu_tray_stayInTray;
    private System.Windows.Forms.MenuItem menu_tray_hideOnCloseBox;
    private System.Windows.Forms.Panel panel_prof_apply;
    private System.Windows.Forms.Panel group_prof;
    private System.Windows.Forms.MenuItem menu_profs_hotkeys;
    private System.Windows.Forms.Panel panel_gameExe;
    private System.Windows.Forms.Panel panel_prof_files;
    private System.Windows.Forms.MenuItem menu_exp_testRdKey;

   #endregion WIDGET_DEFS
    #region *** Widget Callbacks ***
    #region Tray Icon Callbacks
    private void notifyIcon_tray_DoubleClick(object sender, System.EventArgs e) {
      toggle_minimize_tray();
    }
    #endregion
      #region Tab Callbacks
    private void tabCtrl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
      if (m_textbox_focus) // XXX
        return;

      if (combo_prof_names.Visible) {
        if (e.Modifiers == Keys.Alt) {


          if (e.KeyCode == Keys.Up) {
            e.Handled = true;
            if (0 < combo_prof_names.SelectedIndex) {
              combo_prof_names.SelectedIndex -= 1;
            }
          } else if (e.KeyCode == Keys.Down) {
            e.Handled = true;
            if (combo_prof_names.SelectedIndex + 1 < combo_prof_names.Items.Count)
              combo_prof_names.SelectedIndex += 1;
          } else for (int i=0; i < keys_tabCtrl.Length; ++i)
                   if (e.KeyCode == keys_tabCtrl[i]) {
                     tabCtrl.SelectedIndex = i;
                     e.Handled = true;
                     return;
                   }
        }
        if (e.KeyCode == Keys.P || e.KeyCode == Keys.Escape) {
          combo_prof_names.Focus();
          e.Handled = true;
        }
      }
    }

    #endregion
      #region Form Callbacks
    private void FormMain_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
#if TRAY_ON_CLOSEBUTTON
      System.Drawing.Point pos = System.Windows.Forms.Cursor.Position;
      System.Drawing.Rectangle bounds = this.Bounds;
      System.Drawing.Rectangle cbounds = this.ClientRectangle;
      int capHeight = System.Windows.Forms.SystemInformation.CaptionHeight;
      bounds.Height = capHeight;
      bool window_open = WindowState == System.Windows.Forms.FormWindowState.Normal;
      bool cursor_in_bounds = bounds.Contains(pos);
      bool cursor_in_client_rec = cbounds.Contains(this.PointToClient(pos));

      if (window_open && cursor_in_bounds && !cursor_in_client_rec) {
        if (m_ignore_close_just_hide && !m_shift_pressed) {
          e.Cancel = true;
          this.form_iconify_or_deiconify(true, true);
          return;
        }
      }
#endif
      if (GameProfiles.change_count > 0 && !G.ax.cl.opt_selftest)
        e.Cancel = showDialog_save_profiles();
    }

    private void FormMain_SizeChanged(object sender, System.EventArgs e) {
      bool is_minimized = (WindowState == System.Windows.Forms.FormWindowState.Minimized);
      init_update_timer(is_minimized);
      if (!is_minimized)
        gui_compact(ax.ac.gui_compact);
    }
    private void FormMain_Closed(object sender, System.EventArgs e) {
      notifyIcon_tray.Visible = false; 
    }

    private void FormMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
      // KeyPreview property needs to be true, otherwise we will not get any events here!
#if false
      // does not work, because KeyUp event will not received if Window is hidden (needs atleast a workaround)
      if (e.KeyCode == Keys.ShiftKey)
        m_shift_pressed = true;
#endif
#if false
      if (e.KeyCode == Keys.F1 && File.Exists(G.ManualIndexHTML)) {
        Process.Start(G.ManualIndexHTML);
      }
#endif
    }
    private void FormMain_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
      if (e.KeyCode == Keys.ShiftKey)
        m_shift_pressed = false;
    }

    #endregion Form Callbacks
      #region Misc Callbacks
    Keys[] keys_tabCtrl = { Keys.M, Keys.D, Keys.G, Keys.C, Keys.S };

    private void timer_updateCr_Tick(object sender, System.EventArgs e) {
      update_current_combos_from_cr();
    }

    private void check_prof_quit_CheckedChanged(object sender, System.EventArgs e) {
      ax.ac.run_and_quit = check_prof_quit.Checked;
      ax.ac.save_config();
    }
    private void dialog_prof_choose_exec_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {
      OpenFileDialog ofd = (OpenFileDialog)sender;
      if (ofd.Title == "Choose exe file")
        text_prof_exe_path.Text = ofd.FileName;
      else if (dialog_prof_choose_exec.Title.IndexOf("image file") != -1) {
        bool replace = false;
        bool all = false;
        switch (dialog_prof_choose_exec.Title) {
          case "Choose new image file(s)":
            replace = true; all = true;
            break;
          case "Choose new image file":
            replace = true;
            break;
        }

        if (replace && all)
          combo_prof_img.Items.Clear();

        if (replace && !all) {
          if (combo_prof_img.SelectedIndex != -1) {
            combo_prof_img.Items[combo_prof_img.SelectedIndex]
              = (combo_prof_img.Items.Count > 1 ? "0: " : "")
              + ofd.FileName;
          }
        } else if ((replace && all) || (!replace && combo_prof_img.Items.Count == 0)) {
          if (dialog_prof_choose_exec.Title == "Choose new image file(s)") 
            combo_prof_img.Items.Clear();
          if (ofd.FileNames.Length == 1) {
            combo_prof_img.DropDownStyle = ComboBoxStyle.Simple;
            combo_prof_img.Items.AddRange(ofd.FileNames);
          } else if (ofd.FileNames.Length > 1) {
            combo_prof_img.DropDownStyle = ComboBoxStyle.DropDown;
            int drv_nmb = 0;
            foreach(string img in ofd.FileNames) {
              if (combo_prof_img.Items.Count >= 4) //TODO:literal max_images
                break;
              combo_prof_img.Items.Add(drv_nmb.ToString() + ": " + img);
              if (drv_nmb < 3)
                ++drv_nmb;
            }
          }
          combo_prof_img.SelectedIndex = 0;
        } else if (!replace && combo_prof_img.Items.Count > 0) {
          if (combo_prof_img.Items.Count == 1) {
            combo_prof_img.Items[0] = sel_gpd.img_drive_number[0] + ": " + combo_prof_img.Items[0];
            combo_prof_img.DropDownStyle = ComboBoxStyle.DropDown;
          }
          int drv_nmb = 0;
          foreach(string img in ofd.FileNames) {
            if (combo_prof_img.Items.Count >= 4) //TODO:literal max_images
              break;
            combo_prof_img.Items.Add(drv_nmb.ToString() + ": " + img);
            if (drv_nmb < 3)
              ++drv_nmb;
          }


          combo_prof_img_TextChanged(combo_prof_img, null); //TODO:XXX add items to gpd
        }
      }
      else if (ofd.Title == "Choose game config file") {
        if (sel_gpd == null)
          return;
        sel_gpd.game_ini_path = dialog_prof_choose_exec.FileName;
      }
      m_tray_menu_updated = m_main_menu_updated = false;
    }

    #endregion
      #region Button Callbacks
    private void button_clocking_refresh_Click(object sender, System.EventArgs e) {
      clocking_get_clock(false);
    }

    private void button_prof_save_Click(object sender, System.EventArgs e) {
      if (sel_gpi >= 0)
        store_gui_profile(ax.gp.get_profile(sel_gpi));
      ax.gp.save_profiles("profiles.cfg");
      button_prof_save.Enabled = button_prof_discard.Enabled = false;
      m_tray_menu_updated = m_main_menu_updated = false;
    }

    private void button_prof_discard_Click(object sender, System.EventArgs e) {
      ax.gp = GameProfiles.create_from_file("profiles.cfg");
      init_from_gp();
      button_prof_save.Enabled = button_prof_discard.Enabled = false;
      m_tray_menu_updated = m_main_menu_updated = false;
    }

    private void button_prof_apply_Click(object sender, System.EventArgs e) {
      ar_apply.save_state(ax.cr);
      menu_tools_undoApply.Enabled = button_prof_restore.Enabled = true;

      apply_prof();
      gui_update();

      // mounting
      if (sel_gpd == null) 
        return;
      sel_gpd.mount_img(ax.ac.img_daemon_exe_path);	

    }

    private void button_prof_restore_Click(object sender, System.EventArgs e) {
      ar_apply.restore_state(ax.cr);
      ax.cr.ati_apply_d3d(false);
      menu_tools_undoApply.Enabled = button_prof_restore.Enabled = false;

      gui_update();

      // unmounting
      if (sel_gpd == null) 
        return;
      sel_gpd.unmount_img(ax.ac.img_daemon_exe_path);	

    }

    private void button_prof_choose_exe_Click(object sender, System.EventArgs e) {
      Debug.Assert(sel_gpd != null);
      if (sel_gpd == null)
        return;

      dialog_prof_choose_exec.FileName = sel_gpd.exe_path.Length > 0 ? sel_gpd.exe_path : "";
      dialog_prof_choose_exec.Filter = "Exe Files|*.exe;*.com|All Files|*.*";
      dialog_prof_choose_exec.ReadOnlyChecked = true;
      dialog_prof_choose_exec.Title = "Choose exe file";
      dialog_prof_choose_exec.ShowDialog();
      dialog_prof_choose_exec.Filter = "";
      //MessageBox.Show(openFileDialog1.FileName);
    }

    private void button_prof_run_exe_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      ax.cr.ati_apply_d3d(false);
      m_gps.run_exec(sel_gpd, false, null);
      // Quit App
      if (check_prof_quit.Checked) {
        close_me();
      }
    }
    
    private void button_prof_mount_img_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null) 
        return;
      if (!check_daemon_exe())
        return;

      sel_gpd.mount_img(ax.ac.img_daemon_exe_path);	
    }

    private void button_prof_delete_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null) {
        combo_prof_names.Text = ""; // just clear editet text if not selected item.
        return;
      }

      ax.gp.remove_profile(sel_gpi);
      combo_prof_names.Items.RemoveAt(sel_gpi);
      m_gpi = -1; m_gpd = null; // make sel_gpi and sel_gpd invalid
      button_prof_delete.Enabled = button_prof_rename.Enabled = false;

      // clear remaining text if that was the last item
      if (combo_prof_names.Items.Count == 0)
        combo_prof_names.Text = "";
    }

    private void button_prof_apply_and_run_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null) 
        return;

      m_gps.run_profile(sel_gpi, false);
    }


    private void button_prof_choose_img_Click(object sender, System.EventArgs e) {
      img_file_browse(true, true);
    }

    private void button_prof_ok_Click(object sender, System.EventArgs e) {

      if (text_prof_name.Text.Length > 0) {
        if ((string)button_prof_ok.Tag == "rename") {
          if (ax.gp.rename_profile(sel_gpd.name, text_prof_name.Text) > 0) {
            init_from_gp();
            combo_prof_names.SelectedIndex = ax.gp.get_profile_index(sel_gpd.name);
          }
        } else {
          prof_add(text_prof_name.Text, (string)button_prof_ok.Tag == "new");
        }

      }
      state_enter_new_profile(ProfNameState.Choose);
    }

    private void button_prof_cancel_Click(object sender, System.EventArgs e) {
      state_enter_new_profile(ProfNameState.Choose);
    }

    private void button_prof_new_Click(object sender, System.EventArgs e) {
      state_enter_new_profile(ProfNameState.New);
      text_prof_name.Text = G.loc.text_prof_enter_new_name_here;
      text_prof_name.SelectAll();
      button_prof_ok.Enabled = false; // enforce new name
      button_prof_ok.Tag = button_prof_cancel.Tag = "new";
    }

    private void button_prof_clone_Click(object sender, System.EventArgs e) {
      state_enter_new_profile(ProfNameState.New);
      string old_name = combo_prof_names.Text;
      Match match = Regex.Match(old_name, @"^(.+) \((\d+)\)$");
      int i=1;
      if (match.Success) {
        old_name = match.Groups[1].Value;
        i = int.Parse(match.Groups[2].Value);
      }
      string new_name = old_name;
      for (;;++i) {
        if (i > 1) new_name = old_name + " (" + i.ToString() + ")";
        int idx = ax.gp.get_profile_index(new_name);
        if (idx == -1 || ax.gp.get_profile(idx).spec_name != G.spec_name)
          break;
      }
      text_prof_name.Text = new_name;
      text_prof_name.SelectAll();
      button_prof_ok.Tag = button_prof_cancel.Tag = "clone";
    }

    private void button_prof_rename_Click(object sender, System.EventArgs e) {
      state_enter_new_profile(ProfNameState.Rename);
      text_prof_name.Text = sel_gpd.name;
      button_prof_ok.Tag = button_prof_cancel.Tag = "rename";
    }


    void clocking_set_clock_fromGUI() {
      if (text_clocking_curr_core.Text == "" || text_clocking_curr_mem.Text == "")
        return;
      clocking_set_clock(
        float.Parse(text_clocking_curr_core.Text, System.Globalization.NumberStyles.AllowDecimalPoint),
        float.Parse(text_clocking_curr_mem.Text, System.Globalization.NumberStyles.AllowDecimalPoint),
        false);
    }


    private void button_clocking_set_Click(object sender, System.EventArgs e) {
      clocking_set_clock_fromGUI();
    }

    private void button_prof_make_link_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null) 
        return;

      Link.create_link(sel_gpd, ax.ac.sl_name_prefix, ax.ac.sl_name_suffix);
    }

    private void button_prof_apply_and_run_MouseEnter(object sender, System.EventArgs e) {
      if (ax.ac.gui_mover_feedback)
        foreach (Button but in  new Button[] {button_prof_apply, button_prof_mount_img, button_prof_run_exe })
          button_color_effect(but, true);
    }

    private void button_prof_apply_and_run_MouseLeave(object sender, System.EventArgs e) {
      if (ax.ac.gui_mover_feedback)
        foreach (Button but in  new Button[] {button_prof_apply, button_prof_mount_img, button_prof_run_exe })
          button_color_effect(but, false);
    }

    private void button_MouseEnter(object sender, System.EventArgs e) {
      if (ax.ac.gui_mover_feedback)
        button_mover_effect((Button)sender, true);
    }

    private void button_MouseLeave(object sender, System.EventArgs e) {
      if (ax.ac.gui_mover_feedback)
        button_mover_effect((Button)sender, false);
    }

    private void button_clocking_reset_Click(object sender, System.EventArgs e) {
      clocking_set_clock(1.0f, 1.0f, false);
    }

    #endregion
      #region Label Callbacks
 
    private void label_prof_names_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
      if (combo_prof_names.Visible)
        combo_prof_names.Focus();
      else
        text_prof_name.Focus();
    }

    private void label_any_mode_Click(object sender, System.EventArgs e) {
      Label this_label = (Label)sender;

      for (int i=0; i < labels_modes.Length; ++i)
        if (labels_modes[i] == this_label) {
          bool enabled = (combos_prof_modes[i].Enabled ^= true);
          if (sel_gpd != null)
            sel_gpd.val(G.gp_parms[i]).Enabled = enabled;
        }

    
    }

    private void label_any_mode_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
      Label this_label = (Label)sender;
      bool enabled = true;
      if (e.Button == MouseButtons.Right)
        enabled = false;
      else if (e.Button == MouseButtons.Left)
        enabled = true;
      else
        return;

      for (int i=0; i < labels_modes.Length; ++i)
        if (labels_modes[i] == this_label) {
          util_prof_enable_combo(i, enabled);
        }
      
    }

    private void label_prof_d3d_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
      bool enabled = true;
      if (e.Button == MouseButtons.Right)
        enabled = false;
      else if (e.Button == MouseButtons.Left)
        enabled = true;
      else
        return;

      for (int i=0; i < combos_prof_modes.Length; i+=2) {
        util_prof_enable_combo(i, enabled);
      }
       
    }

    private void label_prof_ogl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
      bool enabled = true;
      if (e.Button == MouseButtons.Right)
        enabled = false;
      else if (e.Button == MouseButtons.Left)
        enabled = true;
      else
        return;

      for (int i=1; i < combos_prof_modes.Length; i+=2) {
        util_prof_enable_combo(i, enabled);
      }
	    
    }

    #endregion Label Callbacks    
      #region Text Callbacks
    private void text_prof_exe_path_TextChanged(object sender, System.EventArgs e) {
      toolTip.SetToolTip(text_prof_exe_path,
        (text_prof_exe_path.Text.Length > 0) ? text_prof_exe_path.Text : toolTip_orgs.text_prof_exe_path);

      button_prof_run_exe.Enabled
        = check_prof_quit.Enabled
        = text_prof_exe_args.Enabled
        = ((TextBox)sender).Text != "";

      if (sel_gpd == null)
        return;

      sel_gpd.exe_path = text_prof_exe_path.Text;
      button_prof_set_enable_state();
    }

    private void text_prof_exe_args_TextChanged(object sender, System.EventArgs e) {
      toolTip.SetToolTip(text_prof_exe_args,
        (text_prof_exe_args.Text.Length > 0) ? text_prof_exe_args.Text : toolTip_orgs.text_prof_exe_args);

      int idx = combo_prof_names.SelectedIndex;
      if (idx < 0)
        return;
      GameProfileData gpd = ax.gp.get_profile(idx);
      gpd.exe_args = text_prof_exe_args.Text;

      
    }

    private void text_prof_exe_path_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
      System.Windows.Forms.TextBox this_text = (System.Windows.Forms.TextBox)sender;

      // Handle FileDrop data.
      if(e.Data.GetDataPresent(DataFormats.FileDrop) ) {
        // Assign the file names to a string array, in 
        // case the user has selected multiple files.
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        this_text.Text = files[0];
      }

    }

    private void text_prof_exe_path_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) {
      if (e.Data.GetDataPresent(DataFormats.FileDrop) ) {
        e.Effect = DragDropEffects.Link;
      }
      else {
        e.Effect = DragDropEffects.None;
      }

    }

    private void combo_prof_img_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
      System.Windows.Forms.ComboBox this_text = (System.Windows.Forms.ComboBox)sender;

      // Handle FileDrop data.
      if(e.Data.GetDataPresent(DataFormats.FileDrop) ) {
        // Assign the file names to a string array, in 
        // case the user has selected multiple files.
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        this_text.Text = files[0];
        combo_prof_img.Items.Clear();
        if (files.Length == 1) {
          combo_prof_img.DropDownStyle = ComboBoxStyle.Simple;
          combo_prof_img.Items.AddRange(files);
        } else if (files.Length > 1) {
          combo_prof_img.DropDownStyle = ComboBoxStyle.DropDown;
          int drv_nmb = 0;
          foreach(string img in files) {
            if (combo_prof_img.Items.Count >= 4) //TODO:literal max_images
              break;
            combo_prof_img.Items.Add(drv_nmb.ToString() + ": " + img);
            if (drv_nmb < 3)
              ++drv_nmb;
          }
        }
        combo_prof_img.SelectedIndex = 0;

      }

    }

    private void combo_prof_img_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) {
      if (e.Data.GetDataPresent(DataFormats.FileDrop) ) {
        e.Effect = DragDropEffects.Link;
      }
      else {
        e.Effect = DragDropEffects.None;
      }

    }

    private void text_prof_name_TextChanged(object sender, System.EventArgs e) {
      string text = ((TextBox)sender).Text;
      int idx = ax.gp.get_profile_index(text);
      button_prof_ok.Enabled = (text.Length > 0 && (idx == -1 || ax.gp.get_profile(idx).spec_name != G.spec_name));
    
    }


    private void text_clocking_curr_core_TextChanged(object sender, System.EventArgs e) {
      string s = ((TextBox)sender).Text;
      if (s == "")
        return;
      int int_len = s.IndexOf(System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator);
      if (int_len != -1)
        s = s.Substring(0, int_len);
      track_clocking_curr_core.Value
        = Utils.limited(int.Parse(s, System.Globalization.NumberStyles.AllowDecimalPoint),
        track_clocking_curr_core.Minimum, track_clocking_curr_core.Maximum);
    }

    private void text_clocking_curr_mem_TextChanged(object sender, System.EventArgs e) {
      string s = ((TextBox)sender).Text;
      if (s == "")
        return;

      int int_len = s.IndexOf(System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator);
      if (int_len != -1)
        s = s.Substring(0, int_len);
      track_clocking_curr_mem.Value
        = Utils.limited(int.Parse(s, System.Globalization.NumberStyles.AllowDecimalPoint),
        track_clocking_curr_mem.Minimum, track_clocking_curr_mem.Maximum);    
    }

    private void text_clocking_prof_core_TextChanged(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      sel_gpd.clocking_core_clock = float.Parse(((TextBox)sender).Text, System.Globalization.NumberStyles.AllowDecimalPoint);
      int slider_val = (int)sel_gpd.clocking_core_clock + 1; //TODO: possible endless loop;
      track_clocking_prof_core.Value
        = Utils.limited(slider_val, track_clocking_prof_core.Minimum, track_clocking_prof_core.Maximum);
    }

    private void text_clocking_prof_mem_TextChanged(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;
      sel_gpd.clocking_mem_clock = float.Parse(((TextBox)sender).Text, System.Globalization.NumberStyles.AllowDecimalPoint);   
      int slider_val = (int)sel_gpd.clocking_mem_clock + 1; //TODO: possible endless loop;
      track_clocking_prof_mem.Value
        = Utils.limited(slider_val, track_clocking_prof_mem.Minimum, track_clocking_prof_mem.Maximum);      
    }

    private void text_summary_VisibleChanged(object sender, System.EventArgs e) {
      summary_update();
    }

    private void text_prof_exe_args_Enter(object sender, System.EventArgs e) {
      m_textbox_focus = true;
    }

    private void text_prof_exe_args_Leave(object sender, System.EventArgs e) {
      m_textbox_focus = false;
    }

    #endregion
      #region CheckBox Callbacks
    private void check_clocking_prof_core_CheckedChanged(object sender, System.EventArgs e) {
      clocking_enable_prof_core(((CheckBox)sender).Checked);
    }

    private void check_clocking_prof_mem_CheckedChanged(object sender, System.EventArgs e) {
      clocking_enable_prof_mem(((CheckBox)sender).Checked);
    }

    private void checks_update_combo_CheckStateChanged(object sender, System.EventArgs e) {
      CheckBox chb = (CheckBox)sender;
      combo_user_data cud = (combo_user_data)chb.Tag;
      ComboBox cob = ((cud.kind == combo_user_data.Kind.CURR) ? combos_curr_modes[cud.idx] : combos_prof_modes[cud.idx]);
      set_combo_by_checkbox(chb, cob);
    }

      #endregion
      #region TrackBar Callbacks
    void clocking_set_slider_limits() {
      int[] limits = ax.ac.clocking_limits;
      track_clocking_curr_mem.Minimum  = limits[0];
      track_clocking_curr_mem.Maximum  = limits[1];
      track_clocking_curr_core.Minimum = limits[2];
      track_clocking_curr_core.Maximum = limits[3];
      track_clocking_prof_mem.Minimum  = limits[0];
      track_clocking_prof_mem.Maximum  = limits[1];
      track_clocking_prof_core.Minimum = limits[2];
      track_clocking_prof_core.Maximum = limits[3];
    }
   
    private void track_clocking_curr_core_VisibleChanged(object sender, System.EventArgs e) {
      if (track_clocking_curr_core.Visible) {
        clocking_set_slider_limits();
        clocking_get_clock(true); // Ignore errors, because user may just want to look into profile settings
      }
    }
    private void track_clocking_core_Scroll(object sender, System.EventArgs e) {
      text_clocking_curr_core.Text = track_clocking_curr_core.Value.ToString();   
    }
    private void track_clocking_curr_core_MouseWheel(object sender, MouseEventArgs e) {
      text_clocking_curr_core.Text = track_clocking_curr_core.Value.ToString();   
    }

    private void track_clocking_curr_mem_Scroll(object sender, System.EventArgs e) {
      text_clocking_curr_mem.Text = track_clocking_curr_mem.Value.ToString();   
    }
    private void track_clocking_curr_mem_MouseWheel(object sender, MouseEventArgs e) {
      text_clocking_curr_mem.Text = track_clocking_curr_mem.Value.ToString();   
    }

    private void track_clocking_prof_mem_ValueChanged(object sender, System.EventArgs e) {
      text_clocking_prof_mem.Text = track_clocking_prof_mem.Value.ToString();       
    }

    private void track_clocking_prof_mem_MouseWheel(object sender, MouseEventArgs e) {
      text_clocking_prof_mem.Text = track_clocking_prof_mem.Value.ToString();       
    }
    private void track_clocking_prof_core_ValueChanged(object sender, System.EventArgs e) {
      text_clocking_prof_core.Text = track_clocking_prof_core.Value.ToString();    
    
    }
    private void track_clocking_prof_core_MouseWheel(object sender, MouseEventArgs e) {
      text_clocking_prof_core.Text = track_clocking_prof_core.Value.ToString();       
    }
           
    #endregion TrackBar Callbacks
      #region Combo Callbacks
    private void combo_prof_names_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
      if (e.KeyCode == Keys.Escape) {
        tabCtrl.Focus();
      }
    }

    private void combo_prof_names_TextChanged(object sender, System.EventArgs e) {
      button_prof_set_enable_state();
    }


    private void combos_prof_modes_EnabledChanged(object sender, System.EventArgs e) {
      update_from_gpd(sel_gpd);
    }

    private void combos_prof_modes_SelectedIndexChanged(object sender, System.EventArgs e) {
      System.Windows.Forms.ComboBox this_combo = (System.Windows.Forms.ComboBox)sender;
      int idx = ((combo_user_data)this_combo.Tag).idx;

      //restore color and style for unkown values if necessary
      if (this_combo.DropDownStyle != System.Windows.Forms.ComboBoxStyle.DropDownList)
        set_index_or_text_in_profile_comboBox(idx, this_combo.SelectedIndex, null);

      if (combos_prof_update_in_progress)
        return;

      if (sel_gpd != null && combos_prof_modes[idx].SelectedIndex >= 0) {
        // lookup config-name for selected index in the config
        // store the config-name in the related profile field
        if (!ax.cr.test_modeval_name_or_alias(
          G.cr_modes[idx],
          combos_prof_modes[idx].SelectedIndex,
          sel_gpd.val(G.gp_parms[idx]).Data)) {

          string modeval_name = ax.cr.get_modeval_name(G.cr_modes[idx], combos_prof_modes[idx].SelectedIndex);
          sel_gpd.val(G.gp_parms[idx]).Data = modeval_name;
        }
      }
      combos_mouse_leaves(sender, e); //TODO: workaround. See comment of combos_mouse_leaves()

      update_from_gpd(sel_gpd);
    }

    private void combos_SelectedIndexChanged(object sender, System.EventArgs e) {
      System.Windows.Forms.ComboBox this_combo = (System.Windows.Forms.ComboBox)sender;
      int idx = ((combo_user_data)this_combo.Tag).idx;
      combo_user_data cud = (combo_user_data)this_combo.Tag;

      if (cud.disable_updating == false && 0 <= idx && 0 <= this_combo.SelectedIndex)
        ax.cr.enable_modeval_by_index(G.cr_modes[idx], this_combo.SelectedIndex);

      if (!combos_prof_update_in_progress) //XXX:test
        update_current_combos_from_cr();

      combos_mouse_leaves(sender, e); //TODO: workaround. See comment of combos_mouse_leaves()
    }

    private void combo_prof_names_SelectedIndexChanged(object sender, System.EventArgs e) {
      m_gpi = combo_prof_names.SelectedIndex;

      if (sel_gpi != -1)
        m_gpd = ax.gp.get_profile(sel_gpi);
      update_from_gpd(sel_gpd);

      panel_prof_files.Enabled
        //	= text_prof_exe_args.Enabled 
        = combo_prof_img.Enabled 
        = button_prof_choose_exe.Enabled 
        = button_prof_choose_img.Enabled 
        = button_prof_apply.Enabled
        = button_prof_apply_and_run.Enabled
        //	= check_prof_quit.Enabled
        //	= button_prof_run_exe.Enabled
        //	= button_prof_mount_img.Enabled
        = (sel_gpd != null);

      button_prof_set_enable_state();
      if (sel_gpd != null)
        ax.ac.prof_last_selected = combo_prof_names.Text;

      if (text_summary.Visible)
        summary_update();
    }

    void highlight_button(Button but, bool highlight) {
      but.ForeColor = (highlight ? System.Drawing.Color.Blue : System.Drawing.SystemColors.WindowText);
      //label.Font.Bold = highlight;
    }
    void highlight_3d_label(Label label, bool highlight) {
      label.ForeColor = (highlight ? System.Drawing.Color.Blue : System.Drawing.SystemColors.WindowText);
      //label.Font.Bold = highlight;
    }
    Control m_last_control_entered = null;
    /// <summary>
    /// called by both combo_ and combo_prof_
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void combos_mouse_enter(object sender, System.EventArgs e) {
      combo_user_data cud;

      if (ax.ac.gui_mover_feedback) {

        // compensate for unreliable mouse leave event
        if (m_last_control_entered != null) {
          cud = ((combo_user_data)m_last_control_entered.Tag);
          highlight_3d_label(cud.column_label, false);
          highlight_3d_label(cud.row_label, false);
        }


        Control ctrl = m_last_control_entered = (Control)sender;
        cud = ((combo_user_data)ctrl.Tag);

        highlight_3d_label(cud.column_label, true);
        highlight_3d_label(cud.row_label, true);
      }
    }
    /// <summary>
    /// called by both combo_ and combo_prof_.
    /// there is some bug: Leaving event not always received when leaving while the drop down list is open.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void combos_mouse_leaves(object sender, System.EventArgs e) {
      if (ax.ac.gui_mover_feedback) {
        m_last_control_entered = null;
        Control ctrl = (Control)sender;
        combo_user_data cud = ((combo_user_data)ctrl.Tag);

        highlight_3d_label(cud.column_label, false);
        highlight_3d_label(cud.row_label, false);
      }
		
    }
    private void combo_d3d_qe_mode_SelectedIndexChanged(object sender, System.EventArgs e) {
      ax.cr.enable_modeval_by_index(ConfigRecord.Mode.D3D_QE, combo_d3d_qe_mode.SelectedIndex);		
    }

    private void combo_ogl_qe_mode_SelectedIndexChanged(object sender, System.EventArgs e) {
      ax.cr.enable_modeval_by_index(ConfigRecord.Mode.OGL_QE, combo_ogl_qe_mode.SelectedIndex);		
    }

    private void combos_prof_mouse_down(object sender, System.Windows.Forms.MouseEventArgs e) {
      Control this_control = (Control)sender;
      bool enabled = true;
      if (e.Button == MouseButtons.Right)
        enabled = false;
      else if (e.Button == MouseButtons.Left)
        enabled = true;
      else
        return;

      if (e.Button == MouseButtons.Right) {
        // enable/disable box and related profile parm at right click
        int idx = ((combo_user_data)this_control.Tag).idx;
        util_prof_enable_combo(idx, enabled);
      }
      
    }
    private void combo_prof_names_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
      char c = (char)((char)e.KeyChar + (char)'A' - (char)1);

      string s = c.ToString();

      for (int i=0; i < combo_prof_names.Items.Count; ++i) {
        if (ax.gp.get_profile(i).name.ToUpper().StartsWith(s)) {
          combo_prof_names.SelectedIndex = i;
          break;
        }
      }

      e.Handled = true;
    }

    private void combos_update_checkbox_ForeColorChanged(object sender, System.EventArgs e) {
      ComboBox cob = (ComboBox)sender;
      combo_user_data cud = (combo_user_data)cob.Tag;
      CheckBox chb = ((cud.kind == combo_user_data.Kind.CURR) ? checks_curr_modes[cud.idx] : checks_prof_modes[cud.idx]);
      if (chb == null)
        return;
      chb.ForeColor = cob.ForeColor; // does not work? no color change in CheckBox can be seen.
    }

    private void combos_update_checkbox_EnabledChanged(object sender, System.EventArgs e) {
      ComboBox cob = (ComboBox)sender;
      combo_user_data cud = (combo_user_data)cob.Tag;
      CheckBox chb = ((cud.kind == combo_user_data.Kind.CURR) ? checks_curr_modes[cud.idx] : checks_prof_modes[cud.idx]);
      chb.Enabled = cob.Enabled;
    }
    private void combos_update_checkbox_SelectedIndexChanged(object sender, System.EventArgs e) {
      ComboBox cob = (ComboBox)sender;
      combo_user_data cud = (combo_user_data)cob.Tag;
      CheckBox chb = ((cud.kind == combo_user_data.Kind.CURR) ? checks_curr_modes[cud.idx] : checks_prof_modes[cud.idx]);
      set_checkbox_by_combo(chb, cob);
      // if profile-value is not in list, switch to combo-box, which is capable of showing the unknown value (red-text mode)
      chb.Visible = !(cob.Visible = (cob.SelectedIndex == -1)); 
    }

    private void combo_prof_img_SelectedIndexChanged(object sender, System.EventArgs e) {
      ComboBox obj = (ComboBox)sender;
      int idx = obj.SelectedIndex;
      if (idx < 0)
        return;

      if (Regex.Match(obj.Text, "^[0-9]:").Success) {
        int drv_nmb = int.Parse(obj.Text.Substring(0, 1));
        menu_prof_daemon_drive_nmb_0.Checked = (drv_nmb == 0);
        menu_prof_daemon_drive_nmb_1.Checked = (drv_nmb == 1);
        menu_prof_daemon_drive_nmb_2.Checked = (drv_nmb == 2);
        menu_prof_daemon_drive_nmb_3.Checked = (drv_nmb == 3);
      }
    }

    private void combo_prof_img_TextChanged(object sender, System.EventArgs e) {
      // dynamic multi line tool tip
      if (combo_prof_img.Text.Length == 0)
        toolTip.SetToolTip(combo_prof_img, toolTip_orgs.combo_prof_img);
      else if (combo_prof_img.SelectedIndex == -1) {
        toolTip.SetToolTip(combo_prof_img, combo_prof_img.Text);
      } else {
        string s = "";
        for (int i=0; i < combo_prof_img.Items.Count; ++i) {
          if (s != "") s += "\r\n";
          s += combo_prof_img.Items[i];
        }
        toolTip.SetToolTip(combo_prof_img, s);
      }

      string txt = ((ComboBox)sender).Text;

      button_prof_mount_img.Enabled 
        = this.menu_prof_daemon_drive_nmb.Enabled
        = menu_prof_img_file_replace.Enabled
        = menu_prof_img_file_replaceAll.Enabled
        = menu_prof_img_file_remove.Enabled
        = menu_prof_img_file_removeAll.Enabled
        = txt != "";

      if (sel_gpd == null)
        return;

      if (combo_prof_img.Items.Count > 1 && combo_prof_img.SelectedIndex >= 0) {
        string[] sa = {"", "", "", ""};
        int[] dn = {0, 0, 0, 0};
        for (int i=0; i < sa.Length && i < combo_prof_img.Items.Count; ++i) {
          string mi_text = combo_prof_img.Items[i].ToString();
          Debug.Assert (Regex.Match(mi_text, "^[0-9]: ").Success);
          sa[i] = mi_text.Substring(3);
          dn[i] = int.Parse(mi_text.Substring(0, 1));
        }
        sel_gpd.img_path = sa;
        sel_gpd.img_drive_number = dn;
      } else {
        sel_gpd.img_path = new string[] { txt }; //TODO:image
      }
      
    }

     #endregion Combo Callbacks
      #region Menu Callbacks

        #region File
    private void menu_file_reloadCurrDriverSettings_Click(object sender, System.EventArgs e) {
      update_current_combos_from_cr();    
    }
    private void menu_file_restore3d_loadSavedByRun_Click(object sender, System.EventArgs e) {
      auto_restore_to_before_nongui_run();
    }

    private void menu_file_debugCrashMe_Click(object sender, System.EventArgs e) {
      Application.Exit();
      Environment.Exit(1);
    }

    private void menu_file_quit_Click(object sender, System.EventArgs e) {
      close_me();
    }

    private void menu_file_loadprofs_Click(object sender, System.EventArgs e) {
      ax.gp.append_from_file("profiles.cfg");
    }

    private void menu_file_restore3d_loadLastSaved_Click(object sender, System.EventArgs e) {
      try {
        AutoRestore ar = AutoRestore.create_from_file("saved_3d_settings.bin");
        ar.restore_state(ax.cr);
        update_current_combos_from_cr();
      } catch {
      }
    }
    private void menu_file_restore3d_loadAutoSaved_Click(object sender, System.EventArgs e) {
      try {
        AutoRestore ar = AutoRestore.create_from_file("3d_settings_saved_by_run_exec.bin");
        ar.restore_state(ax.cr);
        update_current_combos_from_cr();
      } catch {
      }
    }

    private void menu_file_restore3d_saveToFile_Click(object sender, System.EventArgs e) {
      try {
        AutoRestore ar = new AutoRestore();
        ar.save_state(ax.cr);
        ar.store_to_file("saved_3d_settings.bin");
      } catch {
      }

    }

    private void menu_file_iconifyTray_Click(object sender, System.EventArgs e) {
      form_iconify_or_deiconify(true, true);
    }

       #endregion
        #region Options
    private void menu_opt_lang_Click(object sender, System.EventArgs e) {
      menu_opt_lang_en.Checked
        = menu_opt_lang_de.Checked
        = menu_opt_lang_auto.Checked
        = false;
      MenuItem this_mi = (MenuItem)sender;
      this_mi.Checked = true;
      ax.app_change_lang(this_mi.Text);
      ax.ac.save_config();
      init_from_cr(true);
    }

    private void menu_opts_autoStart_Click(object sender, System.EventArgs e) {
      menu_opts_autoStart.Checked = auto_start(false, !menu_opts_autoStart.Checked);
    }

    private void menu_opts_multiUser_Click(object sender, System.EventArgs e) {
      if (G.config_in_isolated_storage = (menu_opts_multiUser.Checked ^= true)) {
        File.Create("multi_user_config").Close();
      } else {
        File.Delete("multi_user_config");
      }
      init_from_gp();
      m_appHotkeys.DeactivateHotkeys();
      m_profHotkeys.DeactivateHotkeys();
      init_hotkeys();
    }

    private void menu_opts_regreadonly_Click(object sender, System.EventArgs e) {
      regreadonly_change_for_session(!((MenuItem)sender).Checked);
      ax.ac.reg_readonly = ((MenuItem)sender).Checked;
      ax.ac.save_config();
    }

    private void edit_settings(Form_Settings.TabEn initial_tab) {
      bool old_feature_clocking = ax.ac.feature_clocking;
      if (true) {
        Form_Settings form_opts = new Form_Settings(this, this.ax, initial_tab);
        form_opts.ShowDialog();
        form_opts.Dispose();
      }
      init_update_timer(false);

      Clocking.init();
      if (old_feature_clocking != ax.ac.feature_clocking) {
        if (ax.ac.feature_clocking) {
          if (Clocking.clocking_ability() && !tabCtrl.Controls.Contains(tab_clocking)) {
            tabCtrl.Controls.Add(tab_clocking);
            tabCtrl.SelectedTab = tab_main;
          }
        } else {
          if (tabCtrl.Controls.Contains(tab_clocking)) {
            tabCtrl.Controls.Remove(tab_clocking);
            tabCtrl.SelectedTab = tab_main;
          }
        }
      } else {
        if (tabCtrl.SelectedTab == tab_clocking) {
          clocking_set_slider_limits();
        }
      }
    }

    private void menu_opts_settings_Click(object sender_void, System.EventArgs e_void) {
      edit_settings(Form_Settings.TabEn.Main);
    }

    private void menuItem13_Click(object sender, System.EventArgs e) {
      bool menuIcons = ax.ac.gui_show_menu_icons = (((MenuItem)sender).Checked ^= true);
      ax.ac.save_config();
      foreach(MenuItem mi in mainMenu.MenuItems)
        remove_menuImage_recursively(mi.MenuItems, menuIcons);
    }

    private void menu_opt_3DCheckBoxes_Click(object sender, System.EventArgs e) {
      bool menuIcons = ax.ac.gui_3d_checkboxes = (((MenuItem)sender).Checked ^= true);
      ax.ac.save_config();
      MessageBox.Show("Restarting of 3DProf required.", "3DProf");
    }

        #endregion
        #region Tools
    private void menu_tools_regdiff_ati_Click(object sender, System.EventArgs e) {
      RegDiffForm rdf = new RegDiffForm(Microsoft.Win32.Registry.LocalMachine, @"Software\ATI Technologies", 500);
      rdf.Show();

    }

    private void menu_tools_nvclock_log_Click(object sender, System.EventArgs e) {
      MenuItem mi = (MenuItem)sender;
      Match m = Regex.Match(mi.Text, @"^(\d+) ([ms])");
      if (m.Success) {
        string log_interval = m.Groups[1].Value;
        string unit = m.Groups[2].Value;
        if (unit == "s")
          log_interval += "000";

        Process.Start("etc/nvclock-log.exe", "--log-gpu-clk --log-mem-clk --log-interval " + log_interval);
      }
    }

    private void menu_ati_D3DApply_Click(object sender, System.EventArgs e) {
      ax.cr.ati_apply_d3d(true);
    }

    private void menu_tools_undoApply_Click(object sender, System.EventArgs e) {
      ar_apply.restore_state(ax.cr);
      ax.cr.ati_apply_d3d(false);
      menu_tools_undoApply.Enabled = button_prof_restore.Enabled = false;

      gui_update();
    }

    private void menu_ati_open_cpl_Click(object sender, System.EventArgs e) {
      ati_open_cpl(false);
    }

    private void menu_ati_open_oldCpl_Click(object sender, System.EventArgs e) {
      ati_open_cpl(true);
    }

    private void menu_ati_lsc_Click(object sender, System.EventArgs e) {
      bool lsc_on = (((MenuItem)sender).Checked ^= true);
      using (RegistryKey rek = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management")) {
        rek.SetValue("SystemPages", (Int32)(lsc_on ? 0xffffffff : 0x183000));
        rek.SetValue("LargeSystemCache", (Int32)(lsc_on ? 0x1 : 0x0));
      }
    }

    private void menu_winTweak_disablePageExecutive_Click(object sender, System.EventArgs e) {
      bool on = (((MenuItem)sender).Checked ^= true);
      using (RegistryKey rek = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management")) {
        rek.SetValue("DisablePagingExecutive", (Int32)(on ? 0x1 : 0x0));
      }

    }

    private void menu_winTweaks_Popup(object sender, System.EventArgs e) {
      check_lsc(false);
    }

    const int nvCoolBits_clocking = 0x3;

    private void menu_nvCoolBits_Popup(object sender, System.EventArgs e) {
      using (RegistryKey rek = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\NVIDIA Corporation\Global\NvTweak")) {
        object val_obj = rek.GetValue("CoolBits");
        Int32 val = (val_obj == null) ? 0 : (Int32)val_obj;

        menu_nvCoolBits_clocking.Checked = (val & nvCoolBits_clocking) == nvCoolBits_clocking;


      }

    }

    private void menu_nvCoolBits_clocking_Click(object sender, System.EventArgs e) {
      bool on = (((MenuItem)sender).Checked ^= true);
      using (RegistryKey rek = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\NVIDIA Corporation\Global\NvTweak")) {
        object val_obj = rek.GetValue("CoolBits");
        Int32 val = (val_obj == null) ? 0 : (Int32)val_obj;

        rek.SetValue("CoolBits", (Int32)(on ? (val | nvCoolBits_clocking) : (val & ~nvCoolBits_clocking)));
      }


    }
     #endregion Tools
        #region  Profile
    private void menu_prof_freeMem_Any_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      menu_prof_prio_idle.Checked
        = menu_prof_freeMem_none.Checked
        = menu_prof_freeMem_64mb.Checked
        = menu_prof_freeMem_128mb.Checked
        = menu_prof_freeMem_256mb.Checked
        = menu_prof_freeMem_384mb.Checked
        = menu_prof_freeMem_512mb.Checked
        = menu_prof_freeMem_max.Checked
        = false;

      MenuItem this_mi = (MenuItem)sender;
      this_mi.Checked = true;

      sel_gpd.exe_free_mem
        = ((this_mi == menu_prof_freeMem_none)  ? 0
        : ((this_mi == menu_prof_freeMem_64mb)  ? 64
        : ((this_mi == menu_prof_freeMem_128mb) ? 128
        : ((this_mi == menu_prof_freeMem_256mb) ? 256
        : ((this_mi == menu_prof_freeMem_384mb) ? 384
        : ((this_mi == menu_prof_freeMem_512mb) ? 512
        : ((this_mi == menu_prof_freeMem_max)   ? -1
        : 666)))))));                                                                                 
    }

    private void menu_prof_setSpecName_Click(object sender, System.EventArgs e) {
      prof_set_spec_name(!(menu_prof_setSpecName.Checked ^= true));
    }

    private void menu_prof_filterBySpecName_Click(object sender, System.EventArgs e) {
      m_tray_menu_updated = m_main_menu_updated = false;

      ax.ac.gui_filter_by_spec_name = (menu_profs_filterBySpecName.Checked ^= true);
      int old_selected_idx = combo_prof_names.SelectedIndex;
      init_from_gp();
      if (combo_prof_names.Items.Count > 0)
        combo_prof_names.SelectedIndex
          = (old_selected_idx < combo_prof_names.Items.Count) ? old_selected_idx : 0;
    }

    private void menu_prof_exploreExePath_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;
      System.Diagnostics.Process.Start("explorer", "/e,/select," + sel_gpd.exe_path);    
    }

    private void menu_prof_daemon_drive_nmb_N_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      menu_prof_daemon_drive_nmb_0.Checked
        = menu_prof_daemon_drive_nmb_1.Checked
        = menu_prof_daemon_drive_nmb_2.Checked
        = menu_prof_daemon_drive_nmb_3.Checked
        = false;

      MenuItem this_mi = (MenuItem)sender;
      this_mi.Checked = true;
      sel_gpd.img_drive_number = new int[] { int.Parse(this_mi.Text) };

      int idx = combo_prof_img.SelectedIndex;
      if (0 <= idx) {
        string img = combo_prof_img.Text.Substring(3);
        int drv_nmb = int.Parse(combo_prof_img.Text.Substring(0, 1));
        combo_prof_img.Items.RemoveAt(idx);
        combo_prof_img.Items.Insert(idx, this_mi.Text + ": " + img);
        combo_prof_img.SelectedIndex = idx;
      }
    }

    private void menu_prof_prio_any_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      menu_prof_prio_idle.Checked
        = menu_prof_prio_belowNormal.Checked
        = menu_prof_prio_normal.Checked
        = menu_prof_prio_aboveNormal.Checked
        = menu_prof_prio_high.Checked
        = false;
      MenuItem this_mi = (MenuItem)sender;
      this_mi.Checked = true;
      if (this_mi == menu_prof_prio_idle)
        sel_gpd.exe_process_prio = ProcessPriorityClass.Idle;
      else if (this_mi == menu_prof_prio_belowNormal)
        sel_gpd.exe_process_prio = ProcessPriorityClass.BelowNormal;
      else if (this_mi == menu_prof_prio_normal)
        sel_gpd.exe_process_prio = ProcessPriorityClass.Normal;
      else if (this_mi == menu_prof_prio_aboveNormal)
        sel_gpd.exe_process_prio = ProcessPriorityClass.AboveNormal;
      else if (this_mi == menu_prof_prio_high)
        sel_gpd.exe_process_prio = ProcessPriorityClass.High;
      
    }

    private void menu_prof_detectAPI_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      string dir = Util.Utils.extract_directory(sel_gpd.exe_path);

      int found_d3d = 0, found_ogl = 0;

      Process p = new Process();
      p.StartInfo.FileName = "etc/detect_3d_api.exe";
      p.StartInfo.Arguments = "\"" + dir + "\"";
      p.StartInfo.UseShellExecute = false;
      p.StartInfo.RedirectStandardOutput = true;
      //p.StartInfo.RedirectStandardError = true;
      //p.StartInfo.RedirectStandardInput = true;
      p.StartInfo.CreateNoWindow = true;

      this.Enabled = false;
      Cursor cursor = this.Cursor;
      this.Cursor = Cursors.WaitCursor;

      p.Start();
      p.WaitForExit();
      Cursor = cursor;
      this.Enabled = true;

      string line;
      if (p.ExitCode != 0) {
        return;
      }
      while((line = p.StandardOutput.ReadLine()) != null) {
        if (line.IndexOf("d3d") != -1)
          found_d3d = 1;
        if (line.IndexOf("ogl") != -1)
          found_ogl = 1;
      }



      for (int i=1; i < combos_prof_modes.Length; i+=2) {
        util_prof_enable_combo(i, found_ogl > 0);
      }
      for (int i=0; i < combos_prof_modes.Length; i+=2) {
        util_prof_enable_combo(i, found_d3d > 0);
      }

#if false 
      if (found_ogl > 0 || found_d3d > 0) {
        if (found_ogl > 0)
          MessageBox.Show("OGL Found", G.app_name + " - Experimental");

        if (found_d3d > 0) {
          MessageBox.Show("d3d Found", G.app_name + " - Experimental");
        }
      }
#endif
    
    }

    private void menu_prof_img_file_remove_Click(object sender, System.EventArgs e) {
      int idx = combo_prof_img.SelectedIndex;
      if (idx == -1)
        return;
      img_file_remove(idx);
    }

    private void menu_prof_img_file_replaceAll_Click(object sender, System.EventArgs e) {
      img_file_browse(true, true);
    }

    private void menu_prof_img_file_replace_Click(object sender, System.EventArgs e) {
      img_file_browse(true, false);
    }

    private void menu_prof_img_file_removeAll_Click(object sender, System.EventArgs e) {
      combo_prof_img.Items.Clear();
      combo_prof_img.Text = "";
      combo_prof_img.DropDownStyle = ComboBoxStyle.Simple;
    }

    private void menu_prof_img_file_add_Click(object sender, System.EventArgs e) {
      img_file_browse(false, true);
    }

    private void menu_prof_tdprofGD_help_Click(object sender, System.EventArgs e) {
      System.Windows.Forms.MessageBox.Show(G.loc.msgbox_txt_help_tdprofgd, G.loc.msgbox_title_help_tdprofgd);
    }

    private void menu_prof_tdprofGD_enable(GameProfileData gpd) {
      string lnch_path = null;
      if (gpd != null && gpd.exe_path == "") {
        menu_prof_tdprofGD.Enabled = false;
        return;
      }
      menu_prof_tdprofGD.Enabled = true;

      if (gpd != null && lnch_path == null) {

        string exe_path = gpd.exe_path;
        if (exe_path != "") {
          string exe_dir = Utils.extract_directory(exe_path);
          lnch_path = exe_dir + "/TDProfGD.exe";
        }
      }
      bool file_exists = File.Exists(lnch_path);
      menu_prof_tdprofGD_remove.Enabled = file_exists;
      menu_prof_tdprofGD_create.Enabled = !file_exists;
    }

    private void menu_prof_tdprofGD_remove_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null) 
        return;
      if (sel_gpd.exe_path == "")
        return;

      string exe_dir = Utils.extract_directory(sel_gpd.exe_path);
      
      File.Delete(exe_dir + "/TDProfGD.exe");
      File.Delete(exe_dir + "/TDProfGD.ini");
    }

    private void menu_prof_tdprofGD_create_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null) 
        return;
      if (sel_gpd.exe_path == "")
        return;
      string exe_path = sel_gpd.exe_path;
      string exe_dir = Utils.extract_directory(exe_path);
      string wdir = Directory.GetCurrentDirectory();
      string lnch_path = exe_dir + "/TDProfGD.exe";
      File.Copy("etc/TDProfGD.exe", lnch_path, true);
      if (Environment.OSVersion.Platform == System.PlatformID.Win32NT)
        try {
          ProcessStartInfo si = new ProcessStartInfo("etc/ReplaceIcon.exe", "\"" + lnch_path + "\" \"" + exe_path + "\"");
          si.CreateNoWindow = true;
          si.RedirectStandardOutput = true;
          si.RedirectStandardError = true;
          si.UseShellExecute = false;

          Process pr = Process.Start(si);
          
          
        } catch { }
      StreamWriter sw = new StreamWriter(exe_dir + "/TDProfGD.ini");
      sw.WriteLine("tdprof_dir=" + wdir);
      sw.WriteLine("prof_name=\"" + sel_gpd.name + "\"");
      sw.Close();
    }

    private void menu_prof_tdprofGD_Popup(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      menu_prof_tdprofGD_enable(sel_gpd);
    }

    private void menu_prof_imageFiles_Popup(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      int nmb_of_files = sel_gpd.img_path.Length;
      menu_prof_img_file_add.Enabled = nmb_of_files < 4;
      menu_prof_img_file_remove.Enabled = nmb_of_files > 0;
      menu_prof_img_file_removeAll.Visible = nmb_of_files > 1;
      menu_prof_img_file_replace.Enabled = nmb_of_files > 0;
      menu_prof_img_file_replaceAll.Visible = nmb_of_files > 1;
    }

    private void menu_prof_autoRestore_Any_Click(object sender, System.EventArgs e) {
      MenuItem mi = (MenuItem)sender;
      
      if (sel_gpd == null)
        return;

      sel_gpd.auto_restore_mode
        = ((mi == menu_prof_autoRestore_default)       ? GameProfileData.AutoRestoreMode.NORMAL
        : ((mi == menu_prof_autoRestore_forceOff)      ? GameProfileData.AutoRestoreMode.OFF
        : ((mi == menu_prof_autoRestore_disableDialog) ? GameProfileData.AutoRestoreMode.NO_DIALOG
        : ((mi == menu_prof_autoRestore_forceDialog)   ? GameProfileData.AutoRestoreMode.FORCE_DIALOG
        : GameProfileData.AutoRestoreMode.NORMAL))));                                                                                 
    }

    private void menu_prof_autoRestore_Popup(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      GameProfileData.AutoRestoreMode arm = sel_gpd.auto_restore_mode;

      menu_prof_autoRestore_default.Checked       = (arm == GameProfileData.AutoRestoreMode.NORMAL);
      menu_prof_autoRestore_forceOff.Checked      = (arm == GameProfileData.AutoRestoreMode.OFF);
      menu_prof_autoRestore_forceDialog.Checked   = (arm == GameProfileData.AutoRestoreMode.FORCE_DIALOG);
      menu_prof_autoRestore_disableDialog.Checked = (arm == GameProfileData.AutoRestoreMode.NO_DIALOG);
    }

    private void menu_prof_mouseWare_noAccel_Click(object sender, System.EventArgs e) {
      bool on = (((MenuItem)sender).Checked ^= true);
      if (sel_gpd == null)
        return;

      string exe_file = System.IO.Path.GetFileName(sel_gpd.exe_path).ToLower();
      using (RegistryKey rek = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Logitech\MouseWare\CurrentVersion\GamingCompatibility")) {
        // if a value already exists, just keep it or delete it. 
        foreach(string val in rek.GetValueNames()) {
          string data = rek.GetValue(val) as string;
          if (data == null || data.ToLower() != exe_file)
            continue;
          if (!on) {
            rek.DeleteValue(val);
            mouseWare_restart_emExec();
          }
          return; // all done
        }

        // if a value does not exist create it or do nothing
        if (!on)
          return; // do nothing
        // make a unique value name
        string val_name = exe_file;
        for (int i=0; i < 99; ++i) {
          if (rek.GetValue(val_name) == null)
            break;
          val_name = exe_file + "-" + i.ToString();
        }
        exe_file = System.IO.Path.GetFileName(sel_gpd.exe_path); // XXX case sensitive?
        rek.SetValue(val_name, exe_file);
#if true
        Application.DoEvents();
        Thread thread = new Thread(new ThreadStart(mouseWare_restart_emExec));
        //thread.Priority = System.Threading.ThreadPriority.Lowest;
        thread.Start();
#else
        mouseWare_restart_emExec();
#endif
      }
    }

    private void menu_prof_mouseWare_Popup(object sender, System.EventArgs e) {
      check_mw(false);
    }

    private void menu_profile_ini_edit_Click(object sender, System.EventArgs e) {
      string ini_file_name = m_gps.find_ini_file(true);
      run_game_ini_file(ini_file_name);
    }

    private void menu_profile_ini_find_Click(object sender, System.EventArgs e) {
      string ini_file_name = m_gps.find_ini_file(false);
      if (ini_file_name != null) {
        System.Diagnostics.Process.Start(ini_file_name);
      }
    }

    private void menu_profile_Popup(object sender, System.EventArgs e) {
      if (!m_main_menu_updated) {
        init_main_menu_from_gp();
        m_main_menu_updated = true;
        if (sel_gpd != null)
          update_from_gpd(sel_gpd);
      }
    }

    private void menu_prof_prio_Popup(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;
      GameProfileData gpd = sel_gpd;

      // process priorities
      ProcessPriorityClass prio = gpd.exe_process_prio;
      menu_prof_prio_idle.Checked = (prio == ProcessPriorityClass.Idle);
      menu_prof_prio_belowNormal.Checked = (prio == ProcessPriorityClass.BelowNormal);
      menu_prof_prio_normal.Checked = (prio == ProcessPriorityClass.Normal);
      menu_prof_prio_aboveNormal.Checked = (prio == ProcessPriorityClass.AboveNormal);
      menu_prof_prio_high.Checked = (prio == ProcessPriorityClass.High);
    }

    private void menu_prof_freeMem_Popup(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;
      GameProfileData gpd = sel_gpd;
   
      // free ram menu
      int mb = gpd.exe_free_mem;
      menu_prof_freeMem_none.Checked = (mb == 0);
      menu_prof_freeMem_64mb.Checked = (mb == 64);
      menu_prof_freeMem_128mb.Checked = (mb == 128);
      menu_prof_freeMem_256mb.Checked = (mb == 256);
      menu_prof_freeMem_384mb.Checked = (mb == 384);
      menu_prof_freeMem_512mb.Checked = (mb == 512);
      menu_prof_freeMem_max.Checked = (mb == -1);
    }

    private void menu_prof_importProfile_Any_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      MenuItem mi = (MenuItem)sender;

      foreach(MenuItem mii in mi.Parent.MenuItems) {
        bool was_checked = mii.Checked;
        bool will_checked = (mii == mi) && !was_checked;
        mii.Checked = will_checked;
      }

      sel_gpd.include_other_profile = (mi.Checked ? mi.Text : "");
    }


        #endregion
        #region  Profiles
    private void menu_profs_cmds_Click(object sender, System.EventArgs e) {
      FormCommands fc = new FormCommands(combo_prof_names.SelectedIndex);
      fc.ShowDialog();
      if (G.prof_change_count == 0)
        button_prof_save.Enabled = button_prof_discard.Enabled = button_prof_discard.Enabled = false;
    }

    private void menu_prof_hotkeys_Click(object sender, System.EventArgs e) {
    
      if (m_profHotkeys == null) {
        m_profHotkeys = new ProfileHotkeys(G.sys_hotkeys, new Hotkeys.SystemHotkey.callback_function(hotkey_callback_run_profile));
        try {
          m_profHotkeys.Load("hotkeys.xml");
        } catch {}
        m_profHotkeys.ActivateHotkeys();
      }

      Forms.FormHotkeys fhk = new Forms.FormHotkeys(ax.gp, m_profHotkeys, m_appHotkeys);
      fhk.ShowDialog();
    }
 
    private void menu_profs_exim_Click(object sender, System.EventArgs e) {
      FormProfileExport frm = new FormProfileExport();
      frm.ShowDialog();
    }

    private void menu_profs_templates_turnTo2D_Click(object sender, System.EventArgs e) {
      templates_turnTo2D();
    }

    private void menu_profs_templates_clockOnly_Click(object sender, System.EventArgs e) {
      templates_turnClockOnly();
      try { tabCtrl.SelectedTab = tab_clocking; } catch {}
    }
        #endregion
        #region Help
    private void menu_help_showLicense_Click(object sender, System.EventArgs e) {
      string file_name = "copying.txt";
      System.Diagnostics.Process.Start("notepad", file_name);
   
    }

    private void menu_help_about_Click(object sender, System.EventArgs e) {
#if true
      new AboutForm().ShowDialog();
#else
      System.Windows.Forms.MessageBox
        .Show(
        G.app_name + " version 1.1, Copyright (C) 2003 Bert Winkelmann <Tom.Servo@gmx.net>\r\n"
        +G.app_name + " comes with ABSOLUTELY NO WARRANTY; for details click menu 'Help | No Warranty'.\r\n"
        +"This is free software, and you are welcome to redistribute it under certain\r\n"
        +"conditions; click menu 'Help | Copying Conditions' for details.\r\n",
        G.app_name + " - About"
        );
#endif
    }

    private void menu_help_nonWarranty_Click(object sender, System.EventArgs e) {
      System.Windows.Forms.MessageBox
        .Show(
        "			    NO WARRANTY\r\n"
        +"\r\n"
        +"  11. BECAUSE THE PROGRAM IS LICENSED FREE OF CHARGE, THERE IS NO WARRANTY\r\n"
        +"FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW.  EXCEPT WHEN\r\n"
        +"OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR OTHER PARTIES\r\n"
        +"PROVIDE THE PROGRAM \"AS IS\" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED\r\n"
        +"OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF\r\n"
        +"MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.  THE ENTIRE RISK AS\r\n"
        +"TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU.  SHOULD THE\r\n"
        +"PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING,\r\n"
        +"REPAIR OR CORRECTION.\r\n"
        +"\r\n"
        +"  12. IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING\r\n"
        +"WILL ANY COPYRIGHT HOLDER, OR ANY OTHER PARTY WHO MAY MODIFY AND/OR\r\n"
        +"REDISTRIBUTE THE PROGRAM AS PERMITTED ABOVE, BE LIABLE TO YOU FOR DAMAGES,\r\n"
        +"INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING\r\n"
        +"OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED\r\n"
        +"TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY\r\n"
        +"YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER\r\n"
        +"PROGRAMS), EVEN IF SUCH HOLDER OR OTHER PARTY HAS BEEN ADVISED OF THE\r\n"
        +"POSSIBILITY OF SUCH DAMAGES.\r\n",
        G.app_name + " - No Warranty");
    }

    private void menu_help_sysInfo_Click(object sender, System.EventArgs e) {
      MessageBox.Show(@"
Device Name: " + ax.di.get_device_string() + @"
Driver Version: " + ax.di.get_driver_version() + @"
PCI Device ID: " + ax.di.get_device_id() + @"

--- 3DProf Specific Info ---
Spec-Name: " + G.spec_name + @"
", G.app_name + " - System Info");
    }
    private void menu_tools_openRegedit_Click(object sender, System.EventArgs e) {
      string keyName_lastKey = @"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit";
      //HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit
      string valName_lastKey = "LastKey";
      string valData_lastKey = @"HKEY_LOCAL_MACHINE\" + ax.di.get_driver_key();
      Microsoft.Win32.RegistryKey regkey = null;
      try {
        regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(keyName_lastKey);
        regkey.SetValue(valName_lastKey, valData_lastKey);
        System.Diagnostics.Process.Start("regedit");
      } 
      catch { throw; }

    
    }

    private void menu_help_tooltips_Click(object sender, System.EventArgs e) {
      toolTip.Active 
        = ax.ac.gui_show_tooltips
        = (((MenuItem)sender).Checked ^= true);
    }

    private void menu_help_report_nvclockDebug_Click(object sender, System.EventArgs e) {
      MessageForm.open_dialog(report_nvclock(), "NVClock").resize(600, 500).Show();
    }

    private void menu_help_report_r6clockDebug_Click(object sender, System.EventArgs e) {
      MessageForm.open_dialog(report_r6clock(), "R6Clock").resize(600, 500).Show();  
    }

    private void menu_help_report_displayInfo_Click(object sender, System.EventArgs e) {
      MessageForm.open_dialog(ax.di.report(), "Display Info").resize(600, 500).Show();
    }

    private void menu_help_visit_thread_3DC_Click(object sender, System.EventArgs e) {
      Process.Start("http://www.forum-3dcenter.de/vbulletin/showthread.php?s=&threadid=77278");
    }

    private void menu_help_visit_thread_R3D_Click(object sender, System.EventArgs e) {
      Process.Start("http://www.rage3d.com/board/showthread.php?s=&threadid=33695883");
    }

    private void menu_help_mailto_author_Click(object sender, System.EventArgs e) {
      Process.Start("mailto:3DProf@bertw.de");
    }

    private void menu_help_visit_home_Click(object sender, System.EventArgs e) {
      Process.Start("http://home.tiscalinet.de/bertw/proj/tdprof/3dprof_usr.html");
    }

    private void menu_help_helpButton_Click(object sender, System.EventArgs e) {
      bool help_button = (((MenuItem)sender).Checked ^= true);
      this.MinimizeBox = !help_button;
      this.HelpButton  = help_button;
    }

    private void menu_help_news_Click(object sender, System.EventArgs e) {
      string file_name = "news.txt";
      System.Diagnostics.Process.Start("notepad", file_name);
    }

    private void menu_help_todo_Click(object sender, System.EventArgs e) {
      string file_name = @"etc\TODO.txt";
      System.Diagnostics.Process.Start("notepad", file_name);    
    }

    private void menu_help_visit_thread_3DC_en_Click(object sender, System.EventArgs e) {
      Process.Start("http://www.forum-3dcenter.de/vbulletin/showthread.php?s=&threadid=105737");
    }

    private void menu_help_intro_Click(object sender, System.EventArgs e) {
      MessageForm.open_dialog(G.loc.help_text_intro, "3DProf - Intro").resize(430, 500).Show();
      //MessageBox.Show(G.loc.help_text_intro, "3DProf - Intro");
    }

    private void menu_help_ati_visit_radeonFAQ_Click(object sender, System.EventArgs e) {
      Process.Start("http://www.rage3d.com/radeon/reg/index.shtml");
    }

    private void menu_sfEdit_Click(object sender, System.EventArgs e) {
      FormSpecFileEdit frm = new FormSpecFileEdit();
      frm.ShowDialog();
      frm.Dispose();
    }

    private void menu_help_manual_Click(object sender, System.EventArgs e) {

      if (!File.Exists(G.ManualIndexHTML))
        return;

      // Creating Screenshots
      string sample_screenshot = G.ManualDirectory + @"\img\en\tdprof_shot_main.png";
      if (!File.Exists(sample_screenshot)
        || File.GetLastWriteTime(@".\tdprof.exe").CompareTo(File.GetLastWriteTime(sample_screenshot)) > 0) {
        Process.Start(@".\tdprof.exe", "-screenshots -lang en").WaitForExit();
        Process.Start(@".\tdprof.exe", "-screenshots -lang de").WaitForExit();
      }

      if (!File.Exists(sample_screenshot))
        return; // Error: Screenshots not created

      Process.Start(G.ManualIndexHTML);    
    }


        #endregion
        #region Experimental
    private void menu_exp_testRdKey_Click(object sender, System.EventArgs e) {
      RegDiffForm rdf = new RegDiffForm(Microsoft.Win32.Registry.LocalMachine, G.di.get_driver_key(), 500);
      rdf.Show();
    }

    private void menu_exp_test1_Click(object sender, System.EventArgs e) {
      dev_testing_self(); return;

      Icon ico = new Icon(@"etc\TDProfGD.exe");
      Image myIconImage = new Bitmap(ico.Width, ico.Height);
      Graphics g = Graphics.FromImage(myIconImage);
      g.DrawIcon(ico, 0, 0);//draw your icon into the bitmap
      g.Dispose();
      // this.pictureBox1.Image = myIconImage;
      return;


      Modules.App.AppAutoUpdate.is_online();
      return;
      if (m_profHotkeys == null) {
        m_profHotkeys = new ProfileHotkeys(G.sys_hotkeys, new Hotkeys.SystemHotkey.callback_function(hotkey_callback_run_profile));
        try {
          m_profHotkeys.Load("hotkeys.xml");
        } catch {}
        m_profHotkeys.ActivateHotkeys();
      }

      Forms.FormHotkeys fhk = new Forms.FormHotkeys(ax.gp, m_profHotkeys, m_appHotkeys);
      fhk.Show();
      return;
      m_profHotkeys.AddHotkey("UT2003", (System.Windows.Forms.Keys.F9 | System.Windows.Forms.Keys.Alt));
      m_profHotkeys.AddHotkey("No One Lives Forever", (System.Windows.Forms.Keys.F10 | System.Windows.Forms.Keys.Alt));
      m_profHotkeys.Save("hotkeys.xml");
      return;



    }

    private void menuI_exp_findProc_Click(object sender, System.EventArgs e) {
      GameProfileData gpd = detect_game_process();
      if (gpd == null)
        return;
      MessageBox.Show("Found process: " + gpd.name, G.app_name + " - Experimental");

    }

    private void menuItem22_Click(object sender, System.EventArgs e) {
      ((MenuItem)sender).Checked = (menu_prof_importProfile.Visible ^= true);
    }

        #endregion
        #region Tray
    private void menu_tray_apply_exe_Any_Click(object sender, System.EventArgs e) {
      MenuItem mi = (MenuItem)sender;
      combo_prof_names.SelectedIndex = mi.Index;

      int idx = mi.Index;
      if (idx < 0)
        return;

      m_gps.run_profile(idx, true);
    }

    private void menu_tray_apply_Any_Click(object sender, System.EventArgs e) {
      MenuItem mi = (MenuItem)sender;
      combo_prof_names.SelectedIndex = mi.Index
        + ((mi.Parent.MenuItems[0].MenuItems.Count == 0) ? 0 : -1); // if having a submenu
      ar_apply.save_state(ax.cr);
      menu_tools_undoApply.Enabled = button_prof_restore.Enabled = true;
      apply_prof();
      gui_update();

    }
    
    private void menu_tray_profs_makeLink_Any_Click(object sender, System.EventArgs e) {
      MenuItem mi = (MenuItem)sender;
      GameProfileData gpd = ax.gp.get_profile(mi.Index);
      if (gpd == null)
        return;

      mi.Checked = Link.exists_link(gpd, ax.ac.sl_name_prefix, ax.ac.sl_name_suffix);
      mi.Checked ^= true;
      if (mi.Checked)
        mi.Checked = Link.create_link(gpd, ax.ac.sl_name_prefix, ax.ac.sl_name_suffix);
      else
        Link.delete_link(gpd, ax.ac.sl_name_prefix, ax.ac.sl_name_suffix);

      update_from_gpd(gpd);
    }

    void run_game_ini_file(string ini_file_name) {

      if (ini_file_name == null)
        return;

      try {
        System.Diagnostics.Process.Start(ini_file_name);
      } catch (System.ComponentModel.Win32Exception ex) { 

        switch (ex.NativeErrorCode) {
          case 1155: // no app for this file. Use Notepad
            System.Diagnostics.Process.Start("Notepad", ini_file_name);
            break;

          case 2: // file not found
            MessageBox.Show("Error while trying to edit game config\n"
              + "\nCode:\t" + ex.NativeErrorCode.ToString()
              + "\nText:\t" + ex.Message 
              + "\nFile:\t\"" + ini_file_name + "\"",
              G.app_name + " - User Error");
            break;

          default:
            MessageBox.Show("Error while trying to edit game config\n"
              + "\nCode:\t" + ex.NativeErrorCode.ToString()
              + "\nText:\t" + ex.Message 
              + "\nFile:\t\"" + ini_file_name + "\"",
              G.app_name + " - Error");
            break;
        }
      }
    }

    private void menu_tray_profs_editGameIni_Any_Click(object sender, System.EventArgs e) {
      MenuItem mi = (MenuItem)sender;
      GameProfileData gpd = ax.gp.get_profile(mi.Index);
      run_game_ini_file(gpd.game_ini_path);
    }

    private void menu_tray_img_mountAnImgAtD0_Any_Click(object sender, System.EventArgs e) {
      MenuItem mi = (MenuItem)sender;
      string img_path = mi.Text;
      int drv_nmb = 0;
      string daemon_exe_path = ax.ac.img_daemon_exe_path;

      if (!Utils.file_exists(daemon_exe_path)) {
        System.Windows.Forms.MessageBox.Show("Daemon.exe could not be found at \""
          + daemon_exe_path
          + "\".\r\n"
          + "\r\nHint: Configure the correct path in menu Options=>Settings and try again.",
          G.app_name + " - User Error");
        return;
      }


      string args = "-mount " + drv_nmb + "," + "\"" + img_path + "\"";
      System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
      psi.FileName = daemon_exe_path;
      psi.Arguments = args;
      psi.CreateNoWindow = true;
      System.Diagnostics.Process process = new System.Diagnostics.Process();
      process.StartInfo = psi;
      if (process.Start()) {
        process.WaitForExit(10 * 1000); //TODO: make timeout configurable (?)
        if (!process.HasExited) {
          return; 
        }
      }
      
    }

    private void menu_tray_stayInTray_Click(object sender, System.EventArgs e) {
      ax.ac.gui_show_tray_icon = (((MenuItem)sender).Checked ^= true);
      ax.ac.save_config();
      form_iconify_or_deiconify(this.WindowState == FormWindowState.Minimized, !this.Visible);
    }

    private void menu_tray_hideOnCloseBox_Click(object sender, System.EventArgs e) {
      m_ignore_close_just_hide = ax.ac.gui_hide_on_closeBox = (((MenuItem)sender).Checked ^= true);
      ax.ac.save_config(); 
    }

    private void menu_tray_img_mountCurr_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;
      sel_gpd.mount_img(ax.ac.img_daemon_exe_path);
    }

    private void context_tray_Popup(object sender, System.EventArgs e) {
      if (!m_tray_menu_updated) {
        init_tray_menu_from_gp();
        m_tray_menu_updated = true;
      }
    }

    private void menu_tray_profs_editGameIni_Popup(object sender, System.EventArgs e) {
      
      bool filter_by_videocard = ax.ac.gui_filter_by_spec_name;
      
      for (int i = 0, end = ax.gp.nmb_of_profiles; i < end;  ++i) {

        GameProfileData gpd = ax.gp.get_profile(i);

        if (filter_by_videocard && gpd.spec_name != G.spec_name)
          break;

        menu_tray_profs_editGameIni.MenuItems[i].Visible = (gpd.game_ini_path != "");
      }
    }
    #endregion


 


  #endregion Menu Callbacks


    int m_height_full = 0;  // store form height to help with resizing GUI (compact => normal)
    bool gui_compact(bool compact) {
      
      if (G.ax.cl.opt_screenshots)
        return false;

      // -iconic safe saving of Height in normal state
      if (m_height_full < tabCtrl.Height && (m_height_full = Height) < tabCtrl.Height)
        return false;

      // check if requested state is different from current state
      if (compact != tabCtrl.Visible)
        return compact;

      tabCtrl.Visible = !compact;

      if (compact)
        this.Height -= tabCtrl.Height;
      else
        this.Height = m_height_full;

      return compact;
    }


    #endregion Callbacks

    //////////////////////////////////////////////////////////////////////////////////////   

    private void button_clocking_disable_Click(object sender, System.EventArgs e) {
      check_clocking_prof_core.Checked ^= true;
      check_clocking_prof_mem.Checked  ^= true;   
    }

    void guifb_clocking_prof_set_clocking_kind(GameProfileData.EClockKinds kind) {
      panel_clocking_prof_clocks.Enabled = kind == GameProfileData.EClockKinds.PARENT;
    }

    private void combo_clocking_prof_presets_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;
      int idx = ((ComboBox)sender).SelectedIndex;
      if (idx < 0)
        return;
      sel_gpd.clocking_kind = (GameProfileData.EClockKinds)idx;
      guifb_clocking_prof_set_clocking_kind(sel_gpd.clocking_kind);
    }

    private void combo_clocking_prof_restorePresets_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;
      int idx = ((ComboBox)sender).SelectedIndex;
      if (idx < 0)
        return;
      sel_gpd.clocking_restore_kind = (GameProfileData.EClockKinds)idx;
    
    }

    private void button_clocking_curr_preSlow_Click(object sender, System.EventArgs e) {
      clocking_pre_slow();
    }

    private void button_clocking_curr_preNormal_Click(object sender, System.EventArgs e) {
      clocking_pre_normal();
    }

    private void button_clocking_curr_preFast_Click(object sender, System.EventArgs e) {
      clocking_pre_fast();
    }

    private void button_clocking_curr_preUltra_Click(object sender, System.EventArgs e) {
      clocking_pre_ultra();
    }


    private void menu_tray_clocking_pre_slow_Click(object sender, System.EventArgs e) {
      clocking_pre_slow();
    }

    private void menu_tray_clocking_pre_normal_Click(object sender, System.EventArgs e) {
      clocking_pre_normal();
    }

    private void menu_tray_clocking_pre_fast_Click(object sender, System.EventArgs e) {
      clocking_pre_fast();
    }

    private void menu_tray_clocking_pre_ultra_Click(object sender, System.EventArgs e) {
      clocking_pre_ultra();
    }

    bool check_valid_preset(int[] clocks) {
      int[] clock_limits = ax.ac.clocking_limits;

      if (clocks[0] != 0 && clocks[1] != 0
        && clock_limits[0] <= clocks[1] && clocks[1] <= clock_limits[1]
        && clock_limits[2] <= clocks[0] && clocks[0] <= clock_limits[3])
        return true;

      MessageBox.Show("At first, you have to configure some clock related settings.\r\nSettings Dialog will appear after this message.", "3DProf - Help");
      edit_settings(Form_Settings.TabEn.Clocking);
      return false;
    }

    private void clocking_pre_slow() {
      int[] clocks = ax.ac.clocking_preset_slow;
      if (!check_valid_preset(clocks))
        return;
      clocking_set_clock((float)clocks[0], (float)clocks[1], true);
    }

    private void clocking_pre_normal() {
      int[] clocks = ax.ac.clocking_preset_normal;
      if (!check_valid_preset(clocks))
        return;
      clocking_set_clock((float)clocks[0], (float)clocks[1], true);
    }

    private void clocking_pre_fast() {
      int[] clocks = ax.ac.clocking_preset_fast;
      if (!check_valid_preset(clocks))
        return;
      clocking_set_clock((float)clocks[0], (float)clocks[1], true);
    }

    private void clocking_pre_ultra() {
      int[] clocks = ax.ac.clocking_preset_ultra;
      if (!check_valid_preset(clocks))
        return;
      clocking_set_clock((float)clocks[0], (float)clocks[1], true);
    }

    private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e) {
      if (e.Button == toolButton_editGameCfg) {
        menu_profile_ini_edit_Click (null, null);
      } else if (e.Button == toolButton_hotkeys) {
        menu_prof_hotkeys_Click(null, null);
      } else if (e.Button == toolButton_settings) {
        menu_opts_settings_Click(null, null);
      } else if (e.Button == toolButton_exploreGameFolder) {
        menu_prof_exploreExePath_Click(null, null);
      } else if (e.Button == toolButton_prof_commands) {
        menu_profs_cmds_Click(null, null);
      } else if (e.Button == toolButton_tools_regEdit) {
        menu_tools_openRegedit_Click(null, null);
      } else if (e.Button == toolButton_help_onlineManual) {
        menu_help_manual_Click(null, null);
      } else if (e.Button == toolButton_compact) {
        ax.ac.gui_compact = e.Button.Pushed;
        ax.ac.save_config();
        gui_compact(ax.ac.gui_compact);

      }
    }

    private void check_prof_shellLink_CheckedChanged(object sender, System.EventArgs e) {
      if (sel_gpd == null) 
        return;

      CheckBox chb = (CheckBox)sender;
      if (!chb.Enabled)
        return;

      if (chb.Checked)
        chb.Checked = Link.create_link(sel_gpd, ax.ac.sl_name_prefix, ax.ac.sl_name_suffix);
      else
        Link.delete_link(sel_gpd, ax.ac.sl_name_prefix, ax.ac.sl_name_suffix);
    }

    bool m_activated = false;

    private void FormMain_Activated(object sender, System.EventArgs e) {
      if (m_activated)
        return;
      m_activated = true;
      form_iconify_or_deiconify((m_iconify_me || m_send_me_to_tray), m_send_me_to_tray);

      if (G.ax.cl.opt_screenshots) {
        dev_make_screenshots();
        this.Close();
      }

      if (G.ax.cl.opt_selftest) {
        dev_testing_self();
        this.Close();
      }  
    }

    private void menu_help_log_Click(object sender, System.EventArgs e) {
      try {
        Process.Start("app.log");
      } catch {}
    }

    private void menu_file_debugThrow_Click(object sender, System.EventArgs e) {
       string s = null;
       s.ToLower();
    }

 







  }

}
