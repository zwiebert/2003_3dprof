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
      public ConfigRecord.EMode cr_mode;
      //public combo_user_data(int index) { idx = index; row_label = null; column_label = null; }
      public combo_user_data(Kind k, int index, Label row, Label column, ConfigRecord.EMode crm) {
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
    private System.Windows.Forms.NumericUpDown num_prof_imgDrive;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TrackBar track_ind_modeVal;
    private System.Windows.Forms.Panel panel_ind_modeVal;
    private System.Windows.Forms.GroupBox group_ind_modeVal;
    private System.Windows.Forms.ColumnHeader columnHeader_help;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Splitter splitter_ind;
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
      GameProfiles.ListChanged += new System.EventHandler(game_profile_list_has_changed);
      GameProfiles.DataChanged += new System.EventHandler(game_profile_data_has_changed);
 
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
      combos_curr = new ComboBox[52];
      combos_prof = new ComboBox[52];
      labels      = new Label[52];


      int x_offset = 44, x_space = 1;
      int combo_curr_width = 55;
      int combo_curr_height = 21;
      int combo_prof_width = 55;
      int combo_prof_height = 21;
      int label_width = 55;
      int label_height = 16;

      // LUT for row containers (like groupBoxes or panels)
      Control[][] conts = { new Control[] {group_extra_d3d, group_extra_d3d_2}, new Control[] {group_extra_ogl, group_extra_ogl_2} };
      // LUT for vertical positions inside a row container
      int[][] row_y = { new int[] {12, 36, 60 }, new int[] { 32, 56, 16} }; // [row][n] n: 1=CurrCombo, 2=ProfCombo, 3=Label

      for (int tab = 0; tab < 2; ++tab) 
        for (int row = 0; row < 2; ++row) {
          Control cont = conts[tab][row];
          for (int col = 0; col < 10; ++col) {
            int idx = 12 + (col * 2) + (row * 20) + tab;

            ComboBox cb = combos_curr[idx] = new ComboBox();
            cb.SetBounds(x_offset + col * (combo_curr_width + x_space), row_y[row][0], combo_curr_width, combo_curr_height);
            cont.Controls.Add(cb);

            cb = combos_prof[idx] = new ComboBox();
            cb.SetBounds(x_offset + col * (combo_prof_width + x_space), row_y[row][1], combo_prof_width, combo_prof_height);
            cont.Controls.Add(cb);

            Label lab = labels[idx] = new Label();
            lab.SetBounds(x_offset + col * (label_width + x_space), row_y[row][2], label_width, label_height);
            lab.TextAlign = ContentAlignment.MiddleCenter;
            lab.MouseDown += new System.Windows.Forms.MouseEventHandler(label_any_mode_MouseDown);
            cont.Controls.Add(lab);
          }
        }






      // add existing controls to lookup table

      int i=0;
      combos_curr[i++] = combo_d3d_fsaa_mode;
      combos_curr[i++] = combo_ogl_fsaa_mode;
      combos_curr[i++] = combo_d3d_aniso_mode;
      combos_curr[i++] = combo_ogl_aniso_mode;
      combos_curr[i++] = combo_d3d_vsync_mode;
      combos_curr[i++] = combo_ogl_vsync_mode;
      combos_curr[i++] = combo_d3d_qe_mode;
      combos_curr[i++] = combo_ogl_qe_mode;
      combos_curr[i++] = combo_d3d_lod_bias;
      combos_curr[i++] = combo_ogl_lod_bias;
      combos_curr[i++] = combo_d3d_prerender_frames;
      combos_curr[i++] = combo_ogl_prerender_frames;
      i=0;
      combos_prof[i++] = combo_prof_d3d_fsaa_mode;
      combos_prof[i++] = combo_prof_ogl_fsaa_mode;
      combos_prof[i++] = combo_prof_d3d_aniso_mode;
      combos_prof[i++] = combo_prof_ogl_aniso_mode;
      combos_prof[i++] = combo_prof_d3d_vsync_mode;
      combos_prof[i++] = combo_prof_ogl_vsync_mode;
      combos_prof[i++] = combo_prof_d3d_qe_mode;
      combos_prof[i++] = combo_prof_ogl_qe_mode;
      combos_prof[i++] = combo_prof_d3d_lod_bias;
      combos_prof[i++] = combo_prof_ogl_lod_bias;
      combos_prof[i++] = combo_prof_d3d_prerender_frames;
      combos_prof[i++] = combo_prof_ogl_prerender_frames;
      i=0;
      labels[i++] = label_fsaa_mode;
      labels[i++] = label_fsaa_mode;
      labels[i++] = label_aniso_mode;
      labels[i++] = label_aniso_mode;
      labels[i++] = label_vsync_mode;
      labels[i++] = label_vsync_mode;
      labels[i++] = label_quality;
      labels[i++] = label_quality;
      labels[i++] = label_lod_bias;
      labels[i++] = label_lod_bias;
      labels[i++] = label_prerender_frames;
      labels[i++] = label_prerender_frames;

      #endregion
 
      // set common eventhandler for ComboBoxes

      foreach (ComboBox cb in combos_curr) {
        cb.MouseEnter           += new System.EventHandler(this.combos_mouse_enter);
        cb.MouseLeave           += new System.EventHandler(this.combos_mouse_leaves);
        cb.DropDown             += new System.EventHandler(this.combos_DropDown);
        cb.SelectedIndexChanged += new System.EventHandler(this.combos_SelectedIndexChanged);
      }

      foreach (ComboBox cb in combos_prof) {
        cb.MouseEnter           += new System.EventHandler(this.combos_mouse_enter);
        cb.MouseLeave           += new System.EventHandler(this.combos_mouse_leaves);
        cb.DropDown             += new System.EventHandler(this.combos_DropDown);
        cb.SelectedIndexChanged += new System.EventHandler(this.combos_prof_modes_SelectedIndexChanged);
        cb.MouseDown            += new System.Windows.Forms.MouseEventHandler(this.combos_prof_mouse_down);
        cb.EnabledChanged       += new System.EventHandler(this.combos_prof_modes_EnabledChanged);
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
      gui_recall_sizes();
    }

    void gui_recall_sizes() {
      int ratio = ax.ac.gui_clock_tab_split_ratio;
      int width = ((group_clocking_prof.Width + group_clocking_curr.Width) * ratio) / 100;
      group_clocking_prof.Width = width;
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
      if (sel_gpd != null)
        sel_gpd.val(G.gp_parms[idx]).Enabled = enabled;
      combos_prof_modes[idx].Enabled = enabled;
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
        combo_prof_img.Items[0] = combo_prof_img.Items[0].ToString();
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
      ConfigRecord.EMode cr_mode = G.cr_modes[cud.idx];
      string[] names = ax.cr[cr_mode].names;
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
      ConfigRecord.EMode cr_mode = G.cr_modes[cud.idx];
      string[] names = ax.cr[cr_mode].names;

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
        ConfigRecord.EMode cr_mode = G.cr_modes[cud.idx];
        string[] names = ax.cr[cr_mode].names;

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
          = ax.cr.mode_exists(G.cr_modes[i]);

        // set combo labels and tooltypes by cr (defined in specfile)
        string label = ax.cr[G.cr_modes[i]].gui_label;
        string tooltip = ax.cr[G.cr_modes[i]].gui_tooltip;
        int width_mult = ax.cr[G.cr_modes[i]].gui_width_mult;
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
            cob.Text = ax.cr[cud.cr_mode].get_text_by_index(idx);
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
    private void init_combo(ComboBox box, ConfigRecord.EMode label) {
      box.BeginUpdate();
      string[] new_items = ax.cr[label].texts;

      if (box.Items.Count == new_items.Length) {
        for(int i=0, end = new_items.Length; i < end; ++i)
          box.Items[i] = new_items[i];

      } else {
        box.Items.Clear();
        box.Items.AddRange(ax.cr[label].texts);
      }
      box.EndUpdate();
    }
    #endregion

    #endregion

    #region Game Profiles
    int flags_prof_need_update = ~0;
    enum EProfUpdate { TAB_3D, TAB_D3D, TAB_OGL, TAB_FILE, TAB_EXP, TAB_CLOCKING, MENU };

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

    bool update_from_gpd_in_progress = false;
    /// <summary>
    /// Update profile widgets states by GameProfileData
    /// </summary>
    /// <param name="gpd">profile data</param>
    private void update_from_gpd(GameProfileData gpd) {
      if (gpd == null)
        return; // XXX
      if (combos_prof_update_in_progress)
        return;
      if (update_from_gpd_in_progress)
        return;
      bool force = true;

      this.SuspendLayout();
      update_from_gpd_in_progress = true;

      // update 3D profile combo boxes (let them show an item or write text directly if no item matches)

      if (force || tabCtrl.SelectedTab == tab_main || tabCtrl.SelectedTab == tab_extra_d3d || tabCtrl.SelectedTab == tab_extra_ogl) {
        combos_prof_update_in_progress = true;
        for (int i=0, l=G.cr_modes.Length; i < l; ++i) {
          if (force || combos_prof_modes[i].Visible || (checks_prof_modes != null && checks_prof_modes[i] != null && checks_prof_modes[i].Visible)) {

            string gp_mode_name = gpd.val(G.gp_parms[i]).Data;
            int mode_idx        = ax.cr[G.cr_modes[i]].get_index_by_name(gp_mode_name);
            bool disabled       = !gpd.val(G.gp_parms[i]).Enabled;

            set_index_or_text_in_profile_comboBox(i, mode_idx, gp_mode_name);

            combos_prof_modes[i].Enabled = !disabled;
          }
        }
        combos_prof_update_in_progress = false;
      }




      if (force || (tabCtrl.SelectedTab == tab_files && 0 != (flags_prof_need_update & (1 << (int)EProfUpdate.TAB_FILE)))) {
        // update exe file/args controls
        if (!text_prof_exe_path.Focused) {
          text_prof_exe_path.Text           = gpd.exe_path;
          text_prof_exe_path.SelectionStart = text_prof_exe_path.Text.Length; // show end of text
        }
        if (!text_prof_exe_args.Focused) {
          text_prof_exe_args.Text           = gpd.exe_args;
          text_prof_exe_args.SelectionStart = text_prof_exe_args.Text.Length; // show end of text
        }

        
        check_prof_shellLink.Enabled = false;
        check_prof_shellLink.Checked = Link.exists_link(gpd, ax.ac.sl_name_prefix, ax.ac.sl_name_suffix);
        check_prof_shellLink.Enabled = true;

        // image combo box
        if (!combo_prof_img.Focused) {

          combo_prof_img.Items.Clear();
          combo_prof_img.DropDownStyle = (gpd.img_path.Length > 1) ? ComboBoxStyle.DropDown : ComboBoxStyle.Simple;


          if (gpd.img_path.Length > 0) {
            combo_prof_img.Items.AddRange(gpd.img_path);
          }

          if (combo_prof_img.Items.Count > 0) {
            combo_prof_img.SelectedIndex = 0;
            // daemon.exe
            int idx = combo_prof_img.SelectedIndex;
            int drive_number = gpd.img_drive_number[idx];
            num_prof_imgDrive.Value = drive_number;
          } else {
            combo_prof_img.Text = "";
          }
        
          if (!combo_prof_img.Focused)
            combo_prof_img.SelectionStart = combo_prof_img.Text.Length; // show text end

        }
      }

      if (force || (tabCtrl.SelectedTab == tab_clocking && 0 != (flags_prof_need_update & (1 << (int)EProfUpdate.TAB_CLOCKING)))) {

        // clocking
        text_clocking_prof_core.Text = gpd.clocking_core_clock.ToString();
        text_clocking_prof_mem.Text = gpd.clocking_mem_clock.ToString();
        combo_clocking_prof_presets.SelectedIndex = (int)gpd.clocking_kind;

        clocking_enable_prof_core(gpd.val(GameProfileData.Parms.CLOCKING_CORE_CLK).Enabled);
        clocking_enable_prof_mem(gpd.val(GameProfileData.Parms.CLOCKING_MEM_CLK).Enabled);

      }

      // include master profile
      foreach (MenuItem mi in menu_prof_importProfile.MenuItems) {
        mi.Checked = (mi.Text == gpd.include_other_profile);
      }
        

#if TAB_EXP
      if (force || (tabCtrl.SelectedTab == tab_exp && 0 != (flags_prof_need_update & (1 << (int)EProfUpdate.TAB_EXP)))) {
        tab_exp_update_from_gpd(gpd);
      }
#endif

      update_from_gpd_in_progress = false;
      this.ResumeLayout();
    }


    string menu_prof_tdprofGD_create_Text = null;
    string menu_prof_tdprofGD_remove_Text = null;


    void game_profile_list_has_changed(Object sender, EventArgs e) {
      button_prof_set_enable_state ();
    }

    void game_profile_data_has_changed(Object sender, EventArgs e) {
      button_prof_set_enable_state ();
      if (text_summary.Visible)
        summary_update();
      if (sel_gpd != null)
        update_from_gpd(sel_gpd);
    }


    #endregion




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
          string modeval_name = ax.cr[G.cr_modes[i]].get_name_by_index(combos_prof_modes[i].SelectedIndex);
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
            string modeval_name = ax.cr[G.cr_modes[i]].get_name_by_index(combos_curr_modes[i].SelectedIndex);
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
    #region DATA 
    Font font_lvsi_bold;
    Font font_lvsi_regular;
    Label[] labels_track_ind_modeVal = null;
    #endregion
    #region INIT/UPDATE
    void resize_tab_exp() {
      Label[] labs = labels_track_ind_modeVal;
      if (labs == null) return;
      int count = labs.Length;
      int lab_width = panel_ind_modeVal.Width / (count - 1);
      int lab2_width = (lab_width / 2);

      for (int i=0; i < count; ++i) {
        if (i == 0) {
          labs[i].SetBounds(0, 0, lab2_width, panel_ind_modeVal.Height);
        } else if (i == count - 1) {
          labs[i].SetBounds(lab2_width + (i - 1) * lab_width, 0, lab2_width, panel_ind_modeVal.Height);
        } else {
          labs[i].SetBounds(lab2_width + (i - 1) * lab_width, 0, lab_width, panel_ind_modeVal.Height);
        }
      }
    }
    void init_tab_exp() {

      // Build Direct3D Rows
      for (int i=0, e=G.cr_modes.Length; i < e; ++i) {

        ConfigRecord.EMode cr_mode = G.cr_modes[i];
        GameProfileData.Parms gp_parm = G.gp_parms[i];

        if (!ax.cr.mode_exists(cr_mode))
          continue;
        if (!ConfigRecord.is_mode_d3d(cr_mode))
          continue;

        string row_label = ax.cr[cr_mode].gui_label;
        string tooltip = ax.cr[cr_mode].gui_tooltip; if (tooltip == null) tooltip = "";

        ListViewItem lvi = list_3d.Items.Add(row_label.Replace("&&", "&"));
        lvi.Tag = new option_3d(i);
        lvi.UseItemStyleForSubItems = false; // color support for sub items
        lvi.SubItems.Add("");
        lvi.SubItems.Add("");
        lvi.SubItems.Add(tooltip.Replace("&&", "&"));
      }

      // Build OpenGL Rows
      for (int i=0, e=G.cr_modes.Length; i < e; ++i) {

        ConfigRecord.EMode cr_mode = G.cr_modes[i];
        GameProfileData.Parms gp_parm = G.gp_parms[i];

        if (!ax.cr.mode_exists(cr_mode))
          continue;
        if (!ConfigRecord.is_mode_ogl(cr_mode))
          continue;

        ListViewItem lvi = list_3d.Items.Add("[ogl]  " + ax.cr[cr_mode].gui_label.Replace("&&", "&"));
        lvi.Tag = new option_3d(i);
        lvi.UseItemStyleForSubItems = false; // color support for sub items
        lvi.SubItems.Add(""); 
        lvi.SubItems.Add("");
      }

      // Build Clocking Rows
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
          string text = ax.cr[o3d.cr_mode].get_text_by_index(idx);
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
          string txt = ax.cr[o3d.cr_mode].get_text_by_index(ax.cr[o3d.cr_mode].get_index_by_name(gpd.val(o3d.gp_parm).Data));

          bool is_default = gpd.val(o3d.gp_parm).Data == ax.cr[o3d.cr_mode].get_name_by_index(0);

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
      track_ind_modeVal.Enabled = cbx.Enabled;

      check_ind_d3d.CheckState = (nmb_d3d_enabled == 0) ? CheckState.Unchecked
        : ((nmb_d3d - nmb_d3d_enabled) == 0) ? CheckState.Checked
        : CheckState.Indeterminate;
      check_ind_ogl.CheckState = (nmb_ogl_enabled == 0) ? CheckState.Unchecked
        : ((nmb_ogl - nmb_ogl_enabled) == 0) ? CheckState.Checked
        : CheckState.Indeterminate;  

      list_3d.EndUpdate();
      list_3d.Tag = gpd;
      //XXX// list_3d_SelectedIndexChanged(list_3d, null);
      tab_exp_update_editor();

    }

    #endregion
    #region Widget Events
    void tab_exp_update_editor() {
      ListView.SelectedListViewItemCollection lvis = list_3d.SelectedItems;
      if (lvis.Count == 0)
        return;

      ListViewItem lvi              = lvis[0];
      option_3d o3d                 = lvi.Tag as option_3d;
      option_clock ock              = lvi.Tag as option_clock;
      GameProfileData gpd           = list_3d.Tag as GameProfileData;


      if (o3d != null) {


#if true
        group_ind_modeVal.Text = ax.cr[o3d.cr_mode].gui_label;
        toolTip.SetToolTip(combo_ind_modeVal, ax.cr[o3d.cr_mode].gui_tooltip);
        toolTip.SetToolTip(track_ind_modeVal, ax.cr[o3d.cr_mode].gui_tooltip);
#else
        label_ind_mode.Text = ax.cr.get_gui_label(o3d.cr_mode);
        toolTip.SetToolTip(label_ind_mode, ax.cr.get_gui_tooltip(o3d.cr_mode));
#endif

        ComboBox cbx = combo_ind_modeVal;
        cbx.Items.Clear();
        cbx.Items.AddRange(ax.cr[o3d.cr_mode].texts);
        cbx.Tag = null;
        int count = cbx.Items.Count;
        TrackBar tbr = track_ind_modeVal;
        tbr.Maximum = count - 1;

        if (labels_track_ind_modeVal != null) 
          foreach (Label lab in labels_track_ind_modeVal)
            panel_ind_modeVal.Controls.Remove(lab);

        if (count < 10) {
          Label[] labs = labels_track_ind_modeVal = new Label[count];
          for (int i=0; i < count; ++i) {
            labs[i] = new Label();
            labs[i].Text = cbx.Items[i].ToString();
            if (i == 0) {
              labs[i].TextAlign = ContentAlignment.TopLeft;
            } else if (i == count - 1) {
              labs[i].TextAlign = ContentAlignment.TopRight;
            } else {
              labs[i].TextAlign = ContentAlignment.TopCenter;
            }
          }
          panel_ind_modeVal.Controls.AddRange(labs);
          resize_tab_exp();
        }



     
        if (gpd != null) track_ind_modeVal.Enabled = cbx.Enabled = gpd.val(o3d.gp_parm).Enabled;
        picture_ind_d3d.Visible = ConfigRecord.is_mode_d3d(o3d.cr_mode);
        picture_ind_ogl.Visible = ConfigRecord.is_mode_ogl(o3d.cr_mode);
      
        if (gpd != null) {
          string gp_mode_name = gpd.val(o3d.gp_parm).Data;
          int mode_idx        = ax.cr[o3d.cr_mode].get_index_by_name(gp_mode_name);
          if (mode_idx != -1) {
            cbx.SelectedIndex = mode_idx;
            track_ind_modeVal.Value = mode_idx;
          }
        }
        cbx.Tag = lvi.Tag;
      } else {
        // combo_ind_modeVal.Enabled = combo_ind_modeVal.Visible = false;
      }    
    }

    private void list_3d_SelectedIndexChanged(object sender, System.EventArgs e) {
      tab_exp_update_editor();
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
       //XXX// update_from_gpd(gpd);
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

      gpd.val(o3d.gp_parm).Data = ax.cr[o3d.cr_mode].get_name_by_index(idx);
      if (idx <= track_ind_modeVal.Maximum)
        track_ind_modeVal.Value = idx;
    }

    private void check_ind_d3d_CheckedChanged(object sender, System.EventArgs e) {
      if (list_3d.Tag == null)
        return; // event caused by program code
      GameProfileData gpd = list_3d.Tag as GameProfileData;

      bool enabled = (check_ind_d3d.CheckState == CheckState.Checked);

      list_3d.BeginUpdate();
      update_from_gpd_in_progress = true;
      foreach(ListViewItem lvi in list_3d.Items) {
        option_3d o3d = lvi.Tag as option_3d;
        if (o3d != null && ConfigRecord.is_mode_d3d(o3d.cr_mode)) {
          gpd.val(o3d.gp_parm).Enabled = enabled;
        }
      }
      update_from_gpd_in_progress = false;
      list_3d.EndUpdate();
      update_from_gpd(gpd);
    }

    private void check_ind_ogl_CheckedChanged(object sender, System.EventArgs e) {
      if (list_3d.Tag == null)
        return;
      GameProfileData gpd = list_3d.Tag as GameProfileData;

      bool enabled = (check_ind_ogl.CheckState == CheckState.Checked);

      list_3d.BeginUpdate();
      update_from_gpd_in_progress = true;
      foreach(ListViewItem lvi in list_3d.Items) {
        option_3d o3d = lvi.Tag as option_3d;
        if (o3d != null && ConfigRecord.is_mode_ogl(o3d.cr_mode)) {
          gpd.val(o3d.gp_parm).Enabled = enabled;
        }
      }
      update_from_gpd_in_progress = false;
      list_3d.EndUpdate();
      update_from_gpd(gpd);
    }

    #endregion
    #endregion

    //////////////////////////////////////////////////////////////////////////////////////
    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
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
        this.label_extra_curr_d3d = new System.Windows.Forms.Label();
        this.label_extra_prof_d3d = new System.Windows.Forms.Label();
        this.label_extra_curr_ogl = new System.Windows.Forms.Label();
        this.group_extra_ogl = new System.Windows.Forms.GroupBox();
        this.label_extra_prof_ogl = new System.Windows.Forms.Label();
        this.combo_prof_img = new System.Windows.Forms.ComboBox();
        this.text_prof_exe_args = new System.Windows.Forms.TextBox();
        this.text_prof_exe_path = new System.Windows.Forms.TextBox();
        this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
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
        this.label6 = new System.Windows.Forms.Label();
        this.num_prof_imgDrive = new System.Windows.Forms.NumericUpDown();
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
        this.panel1 = new System.Windows.Forms.Panel();
        this.group_ind_modeVal = new System.Windows.Forms.GroupBox();
        this.combo_ind_modeVal = new System.Windows.Forms.ComboBox();
        this.track_ind_modeVal = new System.Windows.Forms.TrackBar();
        this.panel_ind_modeVal = new System.Windows.Forms.Panel();
        this.picture_ind_d3d = new System.Windows.Forms.PictureBox();
        this.picture_ind_ogl = new System.Windows.Forms.PictureBox();
        this.check_ind_ogl = new System.Windows.Forms.CheckBox();
        this.check_ind_d3d = new System.Windows.Forms.CheckBox();
        this.splitter_ind = new System.Windows.Forms.Splitter();
        this.list_3d = new System.Windows.Forms.ListView();
        this.columnHeader_name = new System.Windows.Forms.ColumnHeader();
        this.columnHeader_profile = new System.Windows.Forms.ColumnHeader();
        this.columnHeader_driver = new System.Windows.Forms.ColumnHeader();
        this.columnHeader_help = new System.Windows.Forms.ColumnHeader();
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
        this.menuImage = new Chris.Beckett.MenuImageLib.MenuImage(this.components);
        this.group_main_d3d.SuspendLayout();
        this.group_extra_d3d.SuspendLayout();
        this.group_extra_ogl.SuspendLayout();
        this.tabCtrl.SuspendLayout();
        this.tab_main.SuspendLayout();
        this.tab_files.SuspendLayout();
        this.panel_prof_files.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.num_prof_imgDrive)).BeginInit();
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
        this.panel1.SuspendLayout();
        this.group_ind_modeVal.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.track_ind_modeVal)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.picture_ind_d3d)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.picture_ind_ogl)).BeginInit();
        this.panel_prof_apply.SuspendLayout();
        this.group_prof.SuspendLayout();
        this.SuspendLayout();
        // 
        // button_prof_apply
        // 
        resources.ApplyResources(this.button_prof_apply, "button_prof_apply");
        this.button_prof_apply.Name = "button_prof_apply";
        this.toolTip.SetToolTip(this.button_prof_apply, resources.GetString("button_prof_apply.ToolTip"));
        this.button_prof_apply.MouseLeave += new System.EventHandler(this.button_MouseLeave);
        this.button_prof_apply.Click += new System.EventHandler(this.button_prof_apply_Click);
        this.button_prof_apply.MouseEnter += new System.EventHandler(this.button_MouseEnter);
        // 
        // button_prof_apply_and_run
        // 
        resources.ApplyResources(this.button_prof_apply_and_run, "button_prof_apply_and_run");
        this.button_prof_apply_and_run.Name = "button_prof_apply_and_run";
        this.toolTip.SetToolTip(this.button_prof_apply_and_run, resources.GetString("button_prof_apply_and_run.ToolTip"));
        this.button_prof_apply_and_run.MouseLeave += new System.EventHandler(this.button_prof_apply_and_run_MouseLeave);
        this.button_prof_apply_and_run.Click += new System.EventHandler(this.button_prof_apply_and_run_Click);
        this.button_prof_apply_and_run.MouseEnter += new System.EventHandler(this.button_prof_apply_and_run_MouseEnter);
        // 
        // button_prof_choose_exe
        // 
        resources.ApplyResources(this.button_prof_choose_exe, "button_prof_choose_exe");
        this.button_prof_choose_exe.Name = "button_prof_choose_exe";
        this.toolTip.SetToolTip(this.button_prof_choose_exe, resources.GetString("button_prof_choose_exe.ToolTip"));
        this.button_prof_choose_exe.Click += new System.EventHandler(this.button_prof_choose_exe_Click);
        // 
        // button_prof_choose_img
        // 
        resources.ApplyResources(this.button_prof_choose_img, "button_prof_choose_img");
        this.button_prof_choose_img.Name = "button_prof_choose_img";
        this.toolTip.SetToolTip(this.button_prof_choose_img, resources.GetString("button_prof_choose_img.ToolTip"));
        this.button_prof_choose_img.Click += new System.EventHandler(this.button_prof_choose_img_Click);
        // 
        // button_prof_delete
        // 
        resources.ApplyResources(this.button_prof_delete, "button_prof_delete");
        this.button_prof_delete.Name = "button_prof_delete";
        this.toolTip.SetToolTip(this.button_prof_delete, resources.GetString("button_prof_delete.ToolTip"));
        this.button_prof_delete.MouseLeave += new System.EventHandler(this.button_MouseLeave);
        this.button_prof_delete.Click += new System.EventHandler(this.button_prof_delete_Click);
        this.button_prof_delete.MouseEnter += new System.EventHandler(this.button_MouseEnter);
        // 
        // button_prof_make_link
        // 
        resources.ApplyResources(this.button_prof_make_link, "button_prof_make_link");
        this.button_prof_make_link.Name = "button_prof_make_link";
        this.toolTip.SetToolTip(this.button_prof_make_link, resources.GetString("button_prof_make_link.ToolTip"));
        this.button_prof_make_link.MouseLeave += new System.EventHandler(this.button_MouseLeave);
        this.button_prof_make_link.Click += new System.EventHandler(this.button_prof_make_link_Click);
        this.button_prof_make_link.MouseEnter += new System.EventHandler(this.button_MouseEnter);
        // 
        // button_prof_mount_img
        // 
        resources.ApplyResources(this.button_prof_mount_img, "button_prof_mount_img");
        this.button_prof_mount_img.Name = "button_prof_mount_img";
        this.toolTip.SetToolTip(this.button_prof_mount_img, resources.GetString("button_prof_mount_img.ToolTip"));
        this.button_prof_mount_img.MouseLeave += new System.EventHandler(this.button_MouseLeave);
        this.button_prof_mount_img.Click += new System.EventHandler(this.button_prof_mount_img_Click);
        this.button_prof_mount_img.MouseEnter += new System.EventHandler(this.button_MouseEnter);
        // 
        // button_prof_run_exe
        // 
        resources.ApplyResources(this.button_prof_run_exe, "button_prof_run_exe");
        this.button_prof_run_exe.Name = "button_prof_run_exe";
        this.toolTip.SetToolTip(this.button_prof_run_exe, resources.GetString("button_prof_run_exe.ToolTip"));
        this.button_prof_run_exe.MouseLeave += new System.EventHandler(this.button_MouseLeave);
        this.button_prof_run_exe.Click += new System.EventHandler(this.button_prof_run_exe_Click);
        this.button_prof_run_exe.MouseEnter += new System.EventHandler(this.button_MouseEnter);
        // 
        // button_prof_save
        // 
        resources.ApplyResources(this.button_prof_save, "button_prof_save");
        this.button_prof_save.Name = "button_prof_save";
        this.toolTip.SetToolTip(this.button_prof_save, resources.GetString("button_prof_save.ToolTip"));
        this.button_prof_save.MouseLeave += new System.EventHandler(this.button_MouseLeave);
        this.button_prof_save.Click += new System.EventHandler(this.button_prof_save_Click);
        this.button_prof_save.MouseEnter += new System.EventHandler(this.button_MouseEnter);
        // 
        // check_prof_quit
        // 
        resources.ApplyResources(this.check_prof_quit, "check_prof_quit");
        this.check_prof_quit.Name = "check_prof_quit";
        this.toolTip.SetToolTip(this.check_prof_quit, resources.GetString("check_prof_quit.ToolTip"));
        this.check_prof_quit.CheckedChanged += new System.EventHandler(this.check_prof_quit_CheckedChanged);
        // 
        // combo_d3d_aniso_mode
        // 
        this.combo_d3d_aniso_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_d3d_aniso_mode, "combo_d3d_aniso_mode");
        this.combo_d3d_aniso_mode.Name = "combo_d3d_aniso_mode";
        // 
        // combo_d3d_fsaa_mode
        // 
        this.combo_d3d_fsaa_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_d3d_fsaa_mode, "combo_d3d_fsaa_mode");
        this.combo_d3d_fsaa_mode.Name = "combo_d3d_fsaa_mode";
        // 
        // combo_d3d_lod_bias
        // 
        this.combo_d3d_lod_bias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_d3d_lod_bias, "combo_d3d_lod_bias");
        this.combo_d3d_lod_bias.Name = "combo_d3d_lod_bias";
        // 
        // combo_d3d_prerender_frames
        // 
        this.combo_d3d_prerender_frames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_d3d_prerender_frames, "combo_d3d_prerender_frames");
        this.combo_d3d_prerender_frames.Name = "combo_d3d_prerender_frames";
        // 
        // combo_d3d_qe_mode
        // 
        this.combo_d3d_qe_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_d3d_qe_mode, "combo_d3d_qe_mode");
        this.combo_d3d_qe_mode.Name = "combo_d3d_qe_mode";
        // 
        // combo_d3d_vsync_mode
        // 
        this.combo_d3d_vsync_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_d3d_vsync_mode, "combo_d3d_vsync_mode");
        this.combo_d3d_vsync_mode.Name = "combo_d3d_vsync_mode";
        // 
        // combo_ogl_aniso_mode
        // 
        resources.ApplyResources(this.combo_ogl_aniso_mode, "combo_ogl_aniso_mode");
        this.combo_ogl_aniso_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_ogl_aniso_mode.Name = "combo_ogl_aniso_mode";
        // 
        // combo_ogl_fsaa_mode
        // 
        resources.ApplyResources(this.combo_ogl_fsaa_mode, "combo_ogl_fsaa_mode");
        this.combo_ogl_fsaa_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_ogl_fsaa_mode.Name = "combo_ogl_fsaa_mode";
        // 
        // combo_ogl_lod_bias
        // 
        resources.ApplyResources(this.combo_ogl_lod_bias, "combo_ogl_lod_bias");
        this.combo_ogl_lod_bias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_ogl_lod_bias.Name = "combo_ogl_lod_bias";
        // 
        // combo_ogl_prerender_frames
        // 
        resources.ApplyResources(this.combo_ogl_prerender_frames, "combo_ogl_prerender_frames");
        this.combo_ogl_prerender_frames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_ogl_prerender_frames.Name = "combo_ogl_prerender_frames";
        // 
        // combo_ogl_qe_mode
        // 
        resources.ApplyResources(this.combo_ogl_qe_mode, "combo_ogl_qe_mode");
        this.combo_ogl_qe_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_ogl_qe_mode.Name = "combo_ogl_qe_mode";
        // 
        // combo_ogl_vsync_mode
        // 
        resources.ApplyResources(this.combo_ogl_vsync_mode, "combo_ogl_vsync_mode");
        this.combo_ogl_vsync_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_ogl_vsync_mode.Name = "combo_ogl_vsync_mode";
        // 
        // combo_prof_d3d_aniso_mode
        // 
        this.combo_prof_d3d_aniso_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_prof_d3d_aniso_mode, "combo_prof_d3d_aniso_mode");
        this.combo_prof_d3d_aniso_mode.Name = "combo_prof_d3d_aniso_mode";
        // 
        // combo_prof_d3d_fsaa_mode
        // 
        this.combo_prof_d3d_fsaa_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_prof_d3d_fsaa_mode.ForeColor = System.Drawing.SystemColors.WindowText;
        resources.ApplyResources(this.combo_prof_d3d_fsaa_mode, "combo_prof_d3d_fsaa_mode");
        this.combo_prof_d3d_fsaa_mode.Name = "combo_prof_d3d_fsaa_mode";
        // 
        // combo_prof_d3d_lod_bias
        // 
        this.combo_prof_d3d_lod_bias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_prof_d3d_lod_bias, "combo_prof_d3d_lod_bias");
        this.combo_prof_d3d_lod_bias.Name = "combo_prof_d3d_lod_bias";
        // 
        // combo_prof_d3d_prerender_frames
        // 
        this.combo_prof_d3d_prerender_frames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_prof_d3d_prerender_frames, "combo_prof_d3d_prerender_frames");
        this.combo_prof_d3d_prerender_frames.Name = "combo_prof_d3d_prerender_frames";
        // 
        // combo_prof_d3d_qe_mode
        // 
        this.combo_prof_d3d_qe_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_prof_d3d_qe_mode, "combo_prof_d3d_qe_mode");
        this.combo_prof_d3d_qe_mode.Name = "combo_prof_d3d_qe_mode";
        // 
        // combo_prof_d3d_vsync_mode
        // 
        this.combo_prof_d3d_vsync_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources(this.combo_prof_d3d_vsync_mode, "combo_prof_d3d_vsync_mode");
        this.combo_prof_d3d_vsync_mode.Name = "combo_prof_d3d_vsync_mode";
        // 
        // combo_prof_names
        // 
        resources.ApplyResources(this.combo_prof_names, "combo_prof_names");
        this.combo_prof_names.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_prof_names.DropDownWidth = 224;
        this.combo_prof_names.Name = "combo_prof_names";
        this.toolTip.SetToolTip(this.combo_prof_names, resources.GetString("combo_prof_names.ToolTip"));
        this.combo_prof_names.SelectedIndexChanged += new System.EventHandler(this.combo_prof_names_SelectedIndexChanged);
        this.combo_prof_names.KeyDown += new System.Windows.Forms.KeyEventHandler(this.combo_prof_names_KeyDown);
        this.combo_prof_names.TextChanged += new System.EventHandler(this.combo_prof_names_TextChanged);
        // 
        // combo_prof_ogl_aniso_mode
        // 
        resources.ApplyResources(this.combo_prof_ogl_aniso_mode, "combo_prof_ogl_aniso_mode");
        this.combo_prof_ogl_aniso_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_prof_ogl_aniso_mode.Name = "combo_prof_ogl_aniso_mode";
        // 
        // combo_prof_ogl_fsaa_mode
        // 
        resources.ApplyResources(this.combo_prof_ogl_fsaa_mode, "combo_prof_ogl_fsaa_mode");
        this.combo_prof_ogl_fsaa_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_prof_ogl_fsaa_mode.Name = "combo_prof_ogl_fsaa_mode";
        // 
        // combo_prof_ogl_lod_bias
        // 
        resources.ApplyResources(this.combo_prof_ogl_lod_bias, "combo_prof_ogl_lod_bias");
        this.combo_prof_ogl_lod_bias.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_prof_ogl_lod_bias.Name = "combo_prof_ogl_lod_bias";
        // 
        // combo_prof_ogl_prerender_frames
        // 
        resources.ApplyResources(this.combo_prof_ogl_prerender_frames, "combo_prof_ogl_prerender_frames");
        this.combo_prof_ogl_prerender_frames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_prof_ogl_prerender_frames.Name = "combo_prof_ogl_prerender_frames";
        // 
        // combo_prof_ogl_qe_mode
        // 
        resources.ApplyResources(this.combo_prof_ogl_qe_mode, "combo_prof_ogl_qe_mode");
        this.combo_prof_ogl_qe_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_prof_ogl_qe_mode.Name = "combo_prof_ogl_qe_mode";
        // 
        // combo_prof_ogl_vsync_mode
        // 
        resources.ApplyResources(this.combo_prof_ogl_vsync_mode, "combo_prof_ogl_vsync_mode");
        this.combo_prof_ogl_vsync_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_prof_ogl_vsync_mode.Name = "combo_prof_ogl_vsync_mode";
        // 
        // dialog_prof_choose_exec
        // 
        this.dialog_prof_choose_exec.FileOk += new System.ComponentModel.CancelEventHandler(this.dialog_prof_choose_exec_FileOk);
        // 
        // label_extra2_prof_ogl
        // 
        resources.ApplyResources(this.label_extra2_prof_ogl, "label_extra2_prof_ogl");
        this.label_extra2_prof_ogl.Cursor = System.Windows.Forms.Cursors.Hand;
        this.label_extra2_prof_ogl.Name = "label_extra2_prof_ogl";
        this.toolTip.SetToolTip(this.label_extra2_prof_ogl, resources.GetString("label_extra2_prof_ogl.ToolTip"));
        this.label_extra2_prof_ogl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_ogl_MouseDown);
        // 
        // label_extra2_prof_d3d
        // 
        resources.ApplyResources(this.label_extra2_prof_d3d, "label_extra2_prof_d3d");
        this.label_extra2_prof_d3d.CausesValidation = false;
        this.label_extra2_prof_d3d.Cursor = System.Windows.Forms.Cursors.Hand;
        this.label_extra2_prof_d3d.Name = "label_extra2_prof_d3d";
        this.toolTip.SetToolTip(this.label_extra2_prof_d3d, resources.GetString("label_extra2_prof_d3d.ToolTip"));
        this.label_extra2_prof_d3d.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_d3d_MouseDown);
        // 
        // label_extra2_curr_ogl
        // 
        resources.ApplyResources(this.label_extra2_curr_ogl, "label_extra2_curr_ogl");
        this.label_extra2_curr_ogl.Name = "label_extra2_curr_ogl";
        this.toolTip.SetToolTip(this.label_extra2_curr_ogl, resources.GetString("label_extra2_curr_ogl.ToolTip"));
        // 
        // label_extra2_curr_d3d
        // 
        resources.ApplyResources(this.label_extra2_curr_d3d, "label_extra2_curr_d3d");
        this.label_extra2_curr_d3d.CausesValidation = false;
        this.label_extra2_curr_d3d.Name = "label_extra2_curr_d3d";
        this.toolTip.SetToolTip(this.label_extra2_curr_d3d, resources.GetString("label_extra2_curr_d3d.ToolTip"));
        // 
        // group_main_d3d
        // 
        this.group_main_d3d.BackColor = System.Drawing.SystemColors.Control;
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
        this.group_main_d3d.Controls.Add(this.label_prof_d3d);
        this.group_main_d3d.Controls.Add(this.combo_prof_d3d_aniso_mode);
        this.group_main_d3d.Controls.Add(this.combo_prof_d3d_vsync_mode);
        this.group_main_d3d.Controls.Add(this.label_prerender_frames);
        this.group_main_d3d.Controls.Add(this.label_fsaa_mode);
        this.group_main_d3d.Controls.Add(this.label_vsync_mode);
        this.group_main_d3d.Controls.Add(this.label_lod_bias);
        this.group_main_d3d.Controls.Add(this.label_aniso_mode);
        this.group_main_d3d.Controls.Add(this.label_quality);
        this.group_main_d3d.Controls.Add(this.combo_prof_ogl_vsync_mode);
        this.group_main_d3d.Controls.Add(this.label_ogl);
        this.group_main_d3d.Controls.Add(this.label_prof_ogl);
        this.group_main_d3d.Controls.Add(this.combo_ogl_vsync_mode);
        this.group_main_d3d.Controls.Add(this.combo_prof_ogl_aniso_mode);
        this.group_main_d3d.Controls.Add(this.combo_prof_ogl_fsaa_mode);
        this.group_main_d3d.Controls.Add(this.combo_ogl_fsaa_mode);
        this.group_main_d3d.Controls.Add(this.combo_prof_ogl_qe_mode);
        this.group_main_d3d.Controls.Add(this.combo_ogl_qe_mode);
        this.group_main_d3d.Controls.Add(this.combo_ogl_lod_bias);
        this.group_main_d3d.Controls.Add(this.combo_prof_ogl_lod_bias);
        this.group_main_d3d.Controls.Add(this.combo_prof_ogl_prerender_frames);
        this.group_main_d3d.Controls.Add(this.combo_ogl_prerender_frames);
        this.group_main_d3d.Controls.Add(this.combo_prof_d3d_prerender_frames);
        this.group_main_d3d.Controls.Add(this.combo_prof_d3d_lod_bias);
        this.group_main_d3d.Controls.Add(this.combo_prof_d3d_qe_mode);
        this.group_main_d3d.Controls.Add(this.combo_ogl_aniso_mode);
        this.group_main_d3d.Cursor = System.Windows.Forms.Cursors.Default;
        resources.ApplyResources(this.group_main_d3d, "group_main_d3d");
        this.group_main_d3d.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.group_main_d3d.Name = "group_main_d3d";
        this.group_main_d3d.TabStop = false;
        this.toolTip.SetToolTip(this.group_main_d3d, resources.GetString("group_main_d3d.ToolTip"));
        this.group_main_d3d.Enter += new System.EventHandler(this.group_current_Enter);
        // 
        // label4
        // 
        resources.ApplyResources(this.label4, "label4");
        this.label4.BackColor = System.Drawing.SystemColors.Window;
        this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.label4.CausesValidation = false;
        this.label4.Cursor = System.Windows.Forms.Cursors.Default;
        this.label4.ForeColor = System.Drawing.SystemColors.ControlDark;
        this.label4.Name = "label4";
        // 
        // label2
        // 
        this.label2.BackColor = System.Drawing.SystemColors.Window;
        this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.label2.CausesValidation = false;
        this.label2.Cursor = System.Windows.Forms.Cursors.Default;
        resources.ApplyResources(this.label2, "label2");
        this.label2.ForeColor = System.Drawing.SystemColors.ControlDark;
        this.label2.Name = "label2";
        // 
        // label_d3d
        // 
        this.label_d3d.CausesValidation = false;
        resources.ApplyResources(this.label_d3d, "label_d3d");
        this.label_d3d.Name = "label_d3d";
        this.toolTip.SetToolTip(this.label_d3d, resources.GetString("label_d3d.ToolTip"));
        // 
        // label_prof_d3d
        // 
        this.label_prof_d3d.CausesValidation = false;
        this.label_prof_d3d.Cursor = System.Windows.Forms.Cursors.Hand;
        resources.ApplyResources(this.label_prof_d3d, "label_prof_d3d");
        this.label_prof_d3d.Name = "label_prof_d3d";
        this.toolTip.SetToolTip(this.label_prof_d3d, resources.GetString("label_prof_d3d.ToolTip"));
        this.label_prof_d3d.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_d3d_MouseDown);
        // 
        // label_prerender_frames
        // 
        resources.ApplyResources(this.label_prerender_frames, "label_prerender_frames");
        this.label_prerender_frames.Cursor = System.Windows.Forms.Cursors.Hand;
        this.label_prerender_frames.Name = "label_prerender_frames";
        this.label_prerender_frames.Tag = "5";
        this.toolTip.SetToolTip(this.label_prerender_frames, resources.GetString("label_prerender_frames.ToolTip"));
        this.label_prerender_frames.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
        // 
        // label_fsaa_mode
        // 
        resources.ApplyResources(this.label_fsaa_mode, "label_fsaa_mode");
        this.label_fsaa_mode.Cursor = System.Windows.Forms.Cursors.Hand;
        this.label_fsaa_mode.Name = "label_fsaa_mode";
        this.label_fsaa_mode.Tag = "0";
        this.toolTip.SetToolTip(this.label_fsaa_mode, resources.GetString("label_fsaa_mode.ToolTip"));
        this.label_fsaa_mode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
        // 
        // label_vsync_mode
        // 
        resources.ApplyResources(this.label_vsync_mode, "label_vsync_mode");
        this.label_vsync_mode.Cursor = System.Windows.Forms.Cursors.Hand;
        this.label_vsync_mode.Name = "label_vsync_mode";
        this.label_vsync_mode.Tag = "2";
        this.toolTip.SetToolTip(this.label_vsync_mode, resources.GetString("label_vsync_mode.ToolTip"));
        this.label_vsync_mode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
        // 
        // label_lod_bias
        // 
        resources.ApplyResources(this.label_lod_bias, "label_lod_bias");
        this.label_lod_bias.Cursor = System.Windows.Forms.Cursors.Hand;
        this.label_lod_bias.Name = "label_lod_bias";
        this.label_lod_bias.Tag = "4";
        this.toolTip.SetToolTip(this.label_lod_bias, resources.GetString("label_lod_bias.ToolTip"));
        this.label_lod_bias.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
        // 
        // label_aniso_mode
        // 
        resources.ApplyResources(this.label_aniso_mode, "label_aniso_mode");
        this.label_aniso_mode.Cursor = System.Windows.Forms.Cursors.Hand;
        this.label_aniso_mode.Name = "label_aniso_mode";
        this.label_aniso_mode.Tag = "1";
        this.toolTip.SetToolTip(this.label_aniso_mode, resources.GetString("label_aniso_mode.ToolTip"));
        this.label_aniso_mode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
        // 
        // label_quality
        // 
        resources.ApplyResources(this.label_quality, "label_quality");
        this.label_quality.Cursor = System.Windows.Forms.Cursors.Hand;
        this.label_quality.Name = "label_quality";
        this.label_quality.Tag = "3";
        this.toolTip.SetToolTip(this.label_quality, resources.GetString("label_quality.ToolTip"));
        this.label_quality.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_any_mode_MouseDown);
        // 
        // label_ogl
        // 
        resources.ApplyResources(this.label_ogl, "label_ogl");
        this.label_ogl.Name = "label_ogl";
        this.toolTip.SetToolTip(this.label_ogl, resources.GetString("label_ogl.ToolTip"));
        // 
        // label_prof_ogl
        // 
        resources.ApplyResources(this.label_prof_ogl, "label_prof_ogl");
        this.label_prof_ogl.Cursor = System.Windows.Forms.Cursors.Hand;
        this.label_prof_ogl.Name = "label_prof_ogl";
        this.toolTip.SetToolTip(this.label_prof_ogl, resources.GetString("label_prof_ogl.ToolTip"));
        this.label_prof_ogl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_ogl_MouseDown);
        // 
        // group_extra_d3d
        // 
        this.group_extra_d3d.Controls.Add(this.label_extra_curr_d3d);
        this.group_extra_d3d.Controls.Add(this.label_extra_prof_d3d);
        this.group_extra_d3d.FlatStyle = System.Windows.Forms.FlatStyle.System;
        resources.ApplyResources(this.group_extra_d3d, "group_extra_d3d");
        this.group_extra_d3d.Name = "group_extra_d3d";
        this.group_extra_d3d.TabStop = false;
        // 
        // label_extra_curr_d3d
        // 
        this.label_extra_curr_d3d.CausesValidation = false;
        resources.ApplyResources(this.label_extra_curr_d3d, "label_extra_curr_d3d");
        this.label_extra_curr_d3d.Name = "label_extra_curr_d3d";
        this.toolTip.SetToolTip(this.label_extra_curr_d3d, resources.GetString("label_extra_curr_d3d.ToolTip"));
        // 
        // label_extra_prof_d3d
        // 
        this.label_extra_prof_d3d.CausesValidation = false;
        this.label_extra_prof_d3d.Cursor = System.Windows.Forms.Cursors.Hand;
        resources.ApplyResources(this.label_extra_prof_d3d, "label_extra_prof_d3d");
        this.label_extra_prof_d3d.Name = "label_extra_prof_d3d";
        this.toolTip.SetToolTip(this.label_extra_prof_d3d, resources.GetString("label_extra_prof_d3d.ToolTip"));
        this.label_extra_prof_d3d.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_d3d_MouseDown);
        // 
        // label_extra_curr_ogl
        // 
        resources.ApplyResources(this.label_extra_curr_ogl, "label_extra_curr_ogl");
        this.label_extra_curr_ogl.Name = "label_extra_curr_ogl";
        this.toolTip.SetToolTip(this.label_extra_curr_ogl, resources.GetString("label_extra_curr_ogl.ToolTip"));
        // 
        // group_extra_ogl
        // 
        this.group_extra_ogl.Controls.Add(this.label_extra_prof_ogl);
        this.group_extra_ogl.Controls.Add(this.label_extra_curr_ogl);
        this.group_extra_ogl.FlatStyle = System.Windows.Forms.FlatStyle.System;
        resources.ApplyResources(this.group_extra_ogl, "group_extra_ogl");
        this.group_extra_ogl.Name = "group_extra_ogl";
        this.group_extra_ogl.TabStop = false;
        // 
        // label_extra_prof_ogl
        // 
        this.label_extra_prof_ogl.Cursor = System.Windows.Forms.Cursors.Hand;
        resources.ApplyResources(this.label_extra_prof_ogl, "label_extra_prof_ogl");
        this.label_extra_prof_ogl.Name = "label_extra_prof_ogl";
        this.toolTip.SetToolTip(this.label_extra_prof_ogl, resources.GetString("label_extra_prof_ogl.ToolTip"));
        this.label_extra_prof_ogl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_prof_ogl_MouseDown);
        // 
        // combo_prof_img
        // 
        this.combo_prof_img.AllowDrop = true;
        resources.ApplyResources(this.combo_prof_img, "combo_prof_img");
        this.combo_prof_img.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_prof_img.DropDownWidth = 450;
        this.combo_prof_img.Name = "combo_prof_img";
        this.toolTip.SetToolTip(this.combo_prof_img, resources.GetString("combo_prof_img.ToolTip"));
        this.combo_prof_img.SelectedIndexChanged += new System.EventHandler(this.combo_prof_img_SelectedIndexChanged);
        this.combo_prof_img.Leave += new System.EventHandler(this.text_prof_exe_args_Leave);
        this.combo_prof_img.Enter += new System.EventHandler(this.text_prof_exe_args_Enter);
        this.combo_prof_img.DragDrop += new System.Windows.Forms.DragEventHandler(this.combo_prof_img_DragDrop);
        this.combo_prof_img.DragEnter += new System.Windows.Forms.DragEventHandler(this.combo_prof_img_DragEnter);
        this.combo_prof_img.TextChanged += new System.EventHandler(this.combo_prof_img_TextChanged);
        // 
        // text_prof_exe_args
        // 
        resources.ApplyResources(this.text_prof_exe_args, "text_prof_exe_args");
        this.text_prof_exe_args.Name = "text_prof_exe_args";
        this.toolTip.SetToolTip(this.text_prof_exe_args, resources.GetString("text_prof_exe_args.ToolTip"));
        this.text_prof_exe_args.TextChanged += new System.EventHandler(this.text_prof_exe_args_TextChanged);
        this.text_prof_exe_args.Leave += new System.EventHandler(this.text_prof_exe_args_Leave);
        this.text_prof_exe_args.Enter += new System.EventHandler(this.text_prof_exe_args_Enter);
        // 
        // text_prof_exe_path
        // 
        this.text_prof_exe_path.AllowDrop = true;
        resources.ApplyResources(this.text_prof_exe_path, "text_prof_exe_path");
        this.text_prof_exe_path.Name = "text_prof_exe_path";
        this.toolTip.SetToolTip(this.text_prof_exe_path, resources.GetString("text_prof_exe_path.ToolTip"));
        this.text_prof_exe_path.TextChanged += new System.EventHandler(this.text_prof_exe_path_TextChanged);
        this.text_prof_exe_path.DragDrop += new System.Windows.Forms.DragEventHandler(this.text_prof_exe_path_DragDrop);
        this.text_prof_exe_path.Leave += new System.EventHandler(this.text_prof_exe_args_Leave);
        this.text_prof_exe_path.Enter += new System.EventHandler(this.text_prof_exe_args_Enter);
        this.text_prof_exe_path.DragEnter += new System.Windows.Forms.DragEventHandler(this.text_prof_exe_path_DragEnter);
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
        // 
        // menu_file
        // 
        this.menu_file.Index = 0;
        this.menu_file.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_file_loadprofs,
            this.menu_file_reloadCurrDriverSettings,
            this.menuItem16,
            this.menu_file_iconifyTray,
            this.menu_file_quit});
        resources.ApplyResources(this.menu_file, "menu_file");
        // 
        // menu_file_loadprofs
        // 
        this.menu_file_loadprofs.Index = 0;
        this.menuImage.SetMenuImage(this.menu_file_loadprofs, null);
        this.menu_file_loadprofs.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_loadprofs, "menu_file_loadprofs");
        this.menu_file_loadprofs.Click += new System.EventHandler(this.menu_file_loadprofs_Click);
        // 
        // menu_file_reloadCurrDriverSettings
        // 
        this.menu_file_reloadCurrDriverSettings.Index = 1;
        this.menuImage.SetMenuImage(this.menu_file_reloadCurrDriverSettings, null);
        this.menu_file_reloadCurrDriverSettings.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_reloadCurrDriverSettings, "menu_file_reloadCurrDriverSettings");
        this.menu_file_reloadCurrDriverSettings.Click += new System.EventHandler(this.menu_file_reloadCurrDriverSettings_Click);
        // 
        // menuItem16
        // 
        this.menuItem16.Index = 2;
        this.menuImage.SetMenuImage(this.menuItem16, null);
        this.menuItem16.OwnerDraw = true;
        resources.ApplyResources(this.menuItem16, "menuItem16");
        // 
        // menu_file_iconifyTray
        // 
        this.menu_file_iconifyTray.Index = 3;
        this.menuImage.SetMenuImage(this.menu_file_iconifyTray, null);
        this.menu_file_iconifyTray.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_iconifyTray, "menu_file_iconifyTray");
        this.menu_file_iconifyTray.Click += new System.EventHandler(this.menu_file_iconifyTray_Click);
        // 
        // menu_file_quit
        // 
        this.menu_file_quit.Index = 4;
        this.menuImage.SetMenuImage(this.menu_file_quit, null);
        this.menu_file_quit.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_quit, "menu_file_quit");
        this.menu_file_quit.Click += new System.EventHandler(this.menu_file_quit_Click);
        // 
        // menu_options
        // 
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
        resources.ApplyResources(this.menu_options, "menu_options");
        // 
        // menu_opts_lang
        // 
        this.menu_opts_lang.Index = 0;
        this.menuImage.SetMenuImage(this.menu_opts_lang, "6");
        this.menu_opts_lang.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_opt_lang_auto,
            this.menu_opt_lang_en,
            this.menu_opt_lang_de});
        this.menu_opts_lang.OwnerDraw = true;
        resources.ApplyResources(this.menu_opts_lang, "menu_opts_lang");
        // 
        // menu_opt_lang_auto
        // 
        this.menu_opt_lang_auto.Index = 0;
        this.menuImage.SetMenuImage(this.menu_opt_lang_auto, null);
        this.menu_opt_lang_auto.OwnerDraw = true;
        this.menu_opt_lang_auto.RadioCheck = true;
        resources.ApplyResources(this.menu_opt_lang_auto, "menu_opt_lang_auto");
        this.menu_opt_lang_auto.Click += new System.EventHandler(this.menu_opt_lang_Click);
        // 
        // menu_opt_lang_en
        // 
        this.menu_opt_lang_en.Index = 1;
        this.menuImage.SetMenuImage(this.menu_opt_lang_en, "1");
        this.menu_opt_lang_en.OwnerDraw = true;
        this.menu_opt_lang_en.RadioCheck = true;
        resources.ApplyResources(this.menu_opt_lang_en, "menu_opt_lang_en");
        this.menu_opt_lang_en.Click += new System.EventHandler(this.menu_opt_lang_Click);
        // 
        // menu_opt_lang_de
        // 
        this.menu_opt_lang_de.Index = 2;
        this.menuImage.SetMenuImage(this.menu_opt_lang_de, "0");
        this.menu_opt_lang_de.OwnerDraw = true;
        this.menu_opt_lang_de.RadioCheck = true;
        resources.ApplyResources(this.menu_opt_lang_de, "menu_opt_lang_de");
        this.menu_opt_lang_de.Click += new System.EventHandler(this.menu_opt_lang_Click);
        // 
        // menuItem1
        // 
        this.menuItem1.Index = 1;
        this.menuImage.SetMenuImage(this.menuItem1, null);
        this.menuItem1.OwnerDraw = true;
        resources.ApplyResources(this.menuItem1, "menuItem1");
        // 
        // menu_opts_regreadonly
        // 
        this.menu_opts_regreadonly.Checked = true;
        this.menu_opts_regreadonly.Index = 2;
        this.menuImage.SetMenuImage(this.menu_opts_regreadonly, null);
        this.menu_opts_regreadonly.OwnerDraw = true;
        resources.ApplyResources(this.menu_opts_regreadonly, "menu_opts_regreadonly");
        this.menu_opts_regreadonly.Click += new System.EventHandler(this.menu_opts_regreadonly_Click);
        // 
        // menuItem17
        // 
        this.menuItem17.Index = 3;
        this.menuImage.SetMenuImage(this.menuItem17, null);
        this.menuItem17.OwnerDraw = true;
        resources.ApplyResources(this.menuItem17, "menuItem17");
        // 
        // menu_opts_menuIcons
        // 
        this.menu_opts_menuIcons.Index = 4;
        this.menuImage.SetMenuImage(this.menu_opts_menuIcons, null);
        this.menu_opts_menuIcons.OwnerDraw = true;
        resources.ApplyResources(this.menu_opts_menuIcons, "menu_opts_menuIcons");
        this.menu_opts_menuIcons.Click += new System.EventHandler(this.menuItem13_Click);
        // 
        // menu_opt_3DCheckBoxes
        // 
        this.menu_opt_3DCheckBoxes.Index = 5;
        this.menuImage.SetMenuImage(this.menu_opt_3DCheckBoxes, null);
        this.menu_opt_3DCheckBoxes.OwnerDraw = true;
        resources.ApplyResources(this.menu_opt_3DCheckBoxes, "menu_opt_3DCheckBoxes");
        this.menu_opt_3DCheckBoxes.Click += new System.EventHandler(this.menu_opt_3DCheckBoxes_Click);
        // 
        // menuItem8
        // 
        this.menuItem8.Index = 6;
        this.menuImage.SetMenuImage(this.menuItem8, null);
        this.menuItem8.OwnerDraw = true;
        resources.ApplyResources(this.menuItem8, "menuItem8");
        // 
        // menu_opts_multiUser
        // 
        this.menu_opts_multiUser.Index = 7;
        this.menuImage.SetMenuImage(this.menu_opts_multiUser, null);
        this.menu_opts_multiUser.OwnerDraw = true;
        resources.ApplyResources(this.menu_opts_multiUser, "menu_opts_multiUser");
        this.menu_opts_multiUser.Click += new System.EventHandler(this.menu_opts_multiUser_Click);
        // 
        // menu_opts_autoStart
        // 
        this.menu_opts_autoStart.Index = 8;
        this.menuImage.SetMenuImage(this.menu_opts_autoStart, null);
        this.menu_opts_autoStart.OwnerDraw = true;
        resources.ApplyResources(this.menu_opts_autoStart, "menu_opts_autoStart");
        this.menu_opts_autoStart.Click += new System.EventHandler(this.menu_opts_autoStart_Click);
        // 
        // menuItem15
        // 
        this.menuItem15.Index = 9;
        this.menuImage.SetMenuImage(this.menuItem15, null);
        this.menuItem15.OwnerDraw = true;
        resources.ApplyResources(this.menuItem15, "menuItem15");
        // 
        // menu_opts_hotkeys
        // 
        this.menu_opts_hotkeys.Index = 10;
        this.menuImage.SetMenuImage(this.menu_opts_hotkeys, "24");
        this.menu_opts_hotkeys.OwnerDraw = true;
        resources.ApplyResources(this.menu_opts_hotkeys, "menu_opts_hotkeys");
        this.menu_opts_hotkeys.Click += new System.EventHandler(this.menu_prof_hotkeys_Click);
        // 
        // menuItem27
        // 
        this.menuItem27.Index = 11;
        this.menuImage.SetMenuImage(this.menuItem27, null);
        this.menuItem27.OwnerDraw = true;
        resources.ApplyResources(this.menuItem27, "menuItem27");
        // 
        // menu_opts_settings
        // 
        this.menu_opts_settings.Index = 12;
        this.menuImage.SetMenuImage(this.menu_opts_settings, "9");
        this.menu_opts_settings.OwnerDraw = true;
        resources.ApplyResources(this.menu_opts_settings, "menu_opts_settings");
        this.menu_opts_settings.Click += new System.EventHandler(this.menu_opts_settings_Click);
        // 
        // menu_tools
        // 
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
        resources.ApplyResources(this.menu_tools, "menu_tools");
        // 
        // menu_tools_openRegedit
        // 
        this.menu_tools_openRegedit.Index = 0;
        this.menuImage.SetMenuImage(this.menu_tools_openRegedit, "23");
        this.menu_tools_openRegedit.OwnerDraw = true;
        resources.ApplyResources(this.menu_tools_openRegedit, "menu_tools_openRegedit");
        this.menu_tools_openRegedit.Click += new System.EventHandler(this.menu_tools_openRegedit_Click);
        // 
        // menu_exp_testRdKey
        // 
        this.menu_exp_testRdKey.Index = 1;
        this.menuImage.SetMenuImage(this.menu_exp_testRdKey, null);
        this.menu_exp_testRdKey.OwnerDraw = true;
        resources.ApplyResources(this.menu_exp_testRdKey, "menu_exp_testRdKey");
        this.menu_exp_testRdKey.Click += new System.EventHandler(this.menu_exp_testRdKey_Click);
        // 
        // menu_tools_regdiff_ati
        // 
        this.menu_tools_regdiff_ati.Index = 2;
        this.menuImage.SetMenuImage(this.menu_tools_regdiff_ati, null);
        this.menu_tools_regdiff_ati.OwnerDraw = true;
        resources.ApplyResources(this.menu_tools_regdiff_ati, "menu_tools_regdiff_ati");
        this.menu_tools_regdiff_ati.Click += new System.EventHandler(this.menu_tools_regdiff_ati_Click);
        // 
        // menuItem5
        // 
        this.menuItem5.Index = 3;
        this.menuImage.SetMenuImage(this.menuItem5, null);
        this.menuItem5.OwnerDraw = true;
        resources.ApplyResources(this.menuItem5, "menuItem5");
        // 
        // menu_tools_nvclock
        // 
        this.menu_tools_nvclock.Index = 4;
        this.menuImage.SetMenuImage(this.menu_tools_nvclock, null);
        this.menu_tools_nvclock.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_tools_nvclock_log,
            this.menuItem19,
            this.menuItem20});
        this.menu_tools_nvclock.OwnerDraw = true;
        resources.ApplyResources(this.menu_tools_nvclock, "menu_tools_nvclock");
        // 
        // menu_tools_nvclock_log
        // 
        this.menu_tools_nvclock_log.Index = 0;
        this.menuImage.SetMenuImage(this.menu_tools_nvclock_log, null);
        this.menu_tools_nvclock_log.OwnerDraw = true;
        resources.ApplyResources(this.menu_tools_nvclock_log, "menu_tools_nvclock_log");
        this.menu_tools_nvclock_log.Click += new System.EventHandler(this.menu_tools_nvclock_log_Click);
        // 
        // menuItem19
        // 
        this.menuItem19.Index = 1;
        this.menuImage.SetMenuImage(this.menuItem19, null);
        this.menuItem19.OwnerDraw = true;
        resources.ApplyResources(this.menuItem19, "menuItem19");
        this.menuItem19.Click += new System.EventHandler(this.menu_tools_nvclock_log_Click);
        // 
        // menuItem20
        // 
        this.menuItem20.Index = 2;
        this.menuImage.SetMenuImage(this.menuItem20, null);
        this.menuItem20.OwnerDraw = true;
        resources.ApplyResources(this.menuItem20, "menuItem20");
        this.menuItem20.Click += new System.EventHandler(this.menu_tools_nvclock_log_Click);
        // 
        // menu_nvCoolBits
        // 
        this.menu_nvCoolBits.Index = 5;
        this.menuImage.SetMenuImage(this.menu_nvCoolBits, null);
        this.menu_nvCoolBits.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_nvCoolBits_clocking});
        this.menu_nvCoolBits.OwnerDraw = true;
        resources.ApplyResources(this.menu_nvCoolBits, "menu_nvCoolBits");
        this.menu_nvCoolBits.Popup += new System.EventHandler(this.menu_nvCoolBits_Popup);
        // 
        // menu_nvCoolBits_clocking
        // 
        this.menu_nvCoolBits_clocking.Index = 0;
        this.menuImage.SetMenuImage(this.menu_nvCoolBits_clocking, null);
        this.menu_nvCoolBits_clocking.OwnerDraw = true;
        resources.ApplyResources(this.menu_nvCoolBits_clocking, "menu_nvCoolBits_clocking");
        this.menu_nvCoolBits_clocking.Click += new System.EventHandler(this.menu_nvCoolBits_clocking_Click);
        // 
        // menu_ati_D3DApply
        // 
        this.menu_ati_D3DApply.Index = 6;
        this.menuImage.SetMenuImage(this.menu_ati_D3DApply, null);
        this.menu_ati_D3DApply.OwnerDraw = true;
        resources.ApplyResources(this.menu_ati_D3DApply, "menu_ati_D3DApply");
        this.menu_ati_D3DApply.Click += new System.EventHandler(this.menu_ati_D3DApply_Click);
        // 
        // menu_ati_open_cpl
        // 
        this.menu_ati_open_cpl.Index = 7;
        this.menuImage.SetMenuImage(this.menu_ati_open_cpl, null);
        this.menu_ati_open_cpl.OwnerDraw = true;
        resources.ApplyResources(this.menu_ati_open_cpl, "menu_ati_open_cpl");
        this.menu_ati_open_cpl.Click += new System.EventHandler(this.menu_ati_open_cpl_Click);
        // 
        // menu_ati_open_oldCpl
        // 
        this.menu_ati_open_oldCpl.Index = 8;
        this.menuImage.SetMenuImage(this.menu_ati_open_oldCpl, null);
        this.menu_ati_open_oldCpl.OwnerDraw = true;
        resources.ApplyResources(this.menu_ati_open_oldCpl, "menu_ati_open_oldCpl");
        this.menu_ati_open_oldCpl.Click += new System.EventHandler(this.menu_ati_open_oldCpl_Click);
        // 
        // menu_winTweaks_Separator
        // 
        this.menu_winTweaks_Separator.Index = 9;
        this.menuImage.SetMenuImage(this.menu_winTweaks_Separator, null);
        this.menu_winTweaks_Separator.OwnerDraw = true;
        resources.ApplyResources(this.menu_winTweaks_Separator, "menu_winTweaks_Separator");
        // 
        // menu_winTweaks
        // 
        this.menu_winTweaks.Index = 10;
        this.menuImage.SetMenuImage(this.menu_winTweaks, null);
        this.menu_winTweaks.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_winTweak_disablePageExecutive,
            this.menu_ati_lsc});
        this.menu_winTweaks.OwnerDraw = true;
        resources.ApplyResources(this.menu_winTweaks, "menu_winTweaks");
        this.menu_winTweaks.Popup += new System.EventHandler(this.menu_winTweaks_Popup);
        // 
        // menu_winTweak_disablePageExecutive
        // 
        this.menu_winTweak_disablePageExecutive.Index = 0;
        this.menuImage.SetMenuImage(this.menu_winTweak_disablePageExecutive, null);
        this.menu_winTweak_disablePageExecutive.OwnerDraw = true;
        resources.ApplyResources(this.menu_winTweak_disablePageExecutive, "menu_winTweak_disablePageExecutive");
        this.menu_winTweak_disablePageExecutive.Click += new System.EventHandler(this.menu_winTweak_disablePageExecutive_Click);
        // 
        // menu_ati_lsc
        // 
        this.menu_ati_lsc.Index = 1;
        this.menuImage.SetMenuImage(this.menu_ati_lsc, null);
        this.menu_ati_lsc.OwnerDraw = true;
        resources.ApplyResources(this.menu_ati_lsc, "menu_ati_lsc");
        this.menu_ati_lsc.Click += new System.EventHandler(this.menu_ati_lsc_Click);
        // 
        // menuItem21
        // 
        this.menuItem21.Index = 11;
        this.menuImage.SetMenuImage(this.menuItem21, null);
        this.menuItem21.OwnerDraw = true;
        resources.ApplyResources(this.menuItem21, "menuItem21");
        // 
        // menu_tools_undoApply
        // 
        resources.ApplyResources(this.menu_tools_undoApply, "menu_tools_undoApply");
        this.menu_tools_undoApply.Index = 12;
        this.menuImage.SetMenuImage(this.menu_tools_undoApply, null);
        this.menu_tools_undoApply.OwnerDraw = true;
        this.menu_tools_undoApply.Click += new System.EventHandler(this.menu_tools_undoApply_Click);
        // 
        // menu_profile
        // 
        this.menu_profile.Index = 3;
        this.menu_profile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem14,
            this.menuItem3,
            this.menu_prof_imageFiles,
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
        resources.ApplyResources(this.menu_profile, "menu_profile");
        this.menu_profile.Popup += new System.EventHandler(this.menu_profile_Popup);
        // 
        // menuItem14
        // 
        this.menuItem14.Index = 0;
        this.menuImage.SetMenuImage(this.menuItem14, "8");
        this.menuItem14.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_prof_exploreExePath,
            this.menu_profile_ini});
        this.menuItem14.OwnerDraw = true;
        resources.ApplyResources(this.menuItem14, "menuItem14");
        // 
        // menu_prof_exploreExePath
        // 
        this.menu_prof_exploreExePath.Index = 0;
        this.menuImage.SetMenuImage(this.menu_prof_exploreExePath, "16");
        this.menu_prof_exploreExePath.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_exploreExePath, "menu_prof_exploreExePath");
        this.menu_prof_exploreExePath.Click += new System.EventHandler(this.menu_prof_exploreExePath_Click);
        // 
        // menu_profile_ini
        // 
        this.menu_profile_ini.Index = 1;
        this.menuImage.SetMenuImage(this.menu_profile_ini, "25");
        this.menu_profile_ini.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_profile_ini_edit,
            this.menu_profile_ini_find});
        this.menu_profile_ini.OwnerDraw = true;
        resources.ApplyResources(this.menu_profile_ini, "menu_profile_ini");
        // 
        // menu_profile_ini_edit
        // 
        this.menu_profile_ini_edit.Index = 0;
        this.menuImage.SetMenuImage(this.menu_profile_ini_edit, null);
        this.menu_profile_ini_edit.OwnerDraw = true;
        resources.ApplyResources(this.menu_profile_ini_edit, "menu_profile_ini_edit");
        this.menu_profile_ini_edit.Click += new System.EventHandler(this.menu_profile_ini_edit_Click);
        // 
        // menu_profile_ini_find
        // 
        this.menu_profile_ini_find.Index = 1;
        this.menuImage.SetMenuImage(this.menu_profile_ini_find, null);
        this.menu_profile_ini_find.OwnerDraw = true;
        resources.ApplyResources(this.menu_profile_ini_find, "menu_profile_ini_find");
        this.menu_profile_ini_find.Click += new System.EventHandler(this.menu_profile_ini_find_Click);
        // 
        // menuItem3
        // 
        this.menuItem3.Index = 1;
        this.menuImage.SetMenuImage(this.menuItem3, null);
        this.menuItem3.OwnerDraw = true;
        resources.ApplyResources(this.menuItem3, "menuItem3");
        // 
        // menu_prof_imageFiles
        // 
        this.menu_prof_imageFiles.Index = 2;
        this.menuImage.SetMenuImage(this.menu_prof_imageFiles, null);
        this.menu_prof_imageFiles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_prof_img_file_replace,
            this.menu_prof_img_file_replaceAll,
            this.menu_prof_img_file_add,
            this.menu_prof_img_file_remove,
            this.menu_prof_img_file_removeAll});
        this.menu_prof_imageFiles.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_imageFiles, "menu_prof_imageFiles");
        this.menu_prof_imageFiles.Popup += new System.EventHandler(this.menu_prof_imageFiles_Popup);
        // 
        // menu_prof_img_file_replace
        // 
        this.menu_prof_img_file_replace.Index = 0;
        this.menuImage.SetMenuImage(this.menu_prof_img_file_replace, null);
        this.menu_prof_img_file_replace.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_img_file_replace, "menu_prof_img_file_replace");
        this.menu_prof_img_file_replace.Click += new System.EventHandler(this.menu_prof_img_file_replace_Click);
        // 
        // menu_prof_img_file_replaceAll
        // 
        this.menu_prof_img_file_replaceAll.Index = 1;
        this.menuImage.SetMenuImage(this.menu_prof_img_file_replaceAll, null);
        this.menu_prof_img_file_replaceAll.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_img_file_replaceAll, "menu_prof_img_file_replaceAll");
        this.menu_prof_img_file_replaceAll.Click += new System.EventHandler(this.menu_prof_img_file_replaceAll_Click);
        // 
        // menu_prof_img_file_add
        // 
        this.menu_prof_img_file_add.Index = 2;
        this.menuImage.SetMenuImage(this.menu_prof_img_file_add, null);
        this.menu_prof_img_file_add.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_img_file_add, "menu_prof_img_file_add");
        this.menu_prof_img_file_add.Click += new System.EventHandler(this.menu_prof_img_file_add_Click);
        // 
        // menu_prof_img_file_remove
        // 
        this.menu_prof_img_file_remove.Index = 3;
        this.menuImage.SetMenuImage(this.menu_prof_img_file_remove, null);
        this.menu_prof_img_file_remove.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_img_file_remove, "menu_prof_img_file_remove");
        this.menu_prof_img_file_remove.Click += new System.EventHandler(this.menu_prof_img_file_remove_Click);
        // 
        // menu_prof_img_file_removeAll
        // 
        this.menu_prof_img_file_removeAll.Index = 4;
        this.menuImage.SetMenuImage(this.menu_prof_img_file_removeAll, null);
        this.menu_prof_img_file_removeAll.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_img_file_removeAll, "menu_prof_img_file_removeAll");
        this.menu_prof_img_file_removeAll.Click += new System.EventHandler(this.menu_prof_img_file_removeAll_Click);
        // 
        // menu_prof_prio
        // 
        this.menu_prof_prio.Index = 3;
        this.menuImage.SetMenuImage(this.menu_prof_prio, "4");
        this.menu_prof_prio.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_prof_prio_high,
            this.menu_prof_prio_aboveNormal,
            this.menu_prof_prio_normal,
            this.menu_prof_prio_belowNormal,
            this.menu_prof_prio_idle});
        this.menu_prof_prio.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_prio, "menu_prof_prio");
        this.menu_prof_prio.Popup += new System.EventHandler(this.menu_prof_prio_Popup);
        // 
        // menu_prof_prio_high
        // 
        this.menu_prof_prio_high.Index = 0;
        this.menuImage.SetMenuImage(this.menu_prof_prio_high, null);
        this.menu_prof_prio_high.OwnerDraw = true;
        this.menu_prof_prio_high.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_prio_high, "menu_prof_prio_high");
        this.menu_prof_prio_high.Click += new System.EventHandler(this.menu_prof_prio_any_Click);
        // 
        // menu_prof_prio_aboveNormal
        // 
        this.menu_prof_prio_aboveNormal.Index = 1;
        this.menuImage.SetMenuImage(this.menu_prof_prio_aboveNormal, null);
        this.menu_prof_prio_aboveNormal.OwnerDraw = true;
        this.menu_prof_prio_aboveNormal.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_prio_aboveNormal, "menu_prof_prio_aboveNormal");
        this.menu_prof_prio_aboveNormal.Click += new System.EventHandler(this.menu_prof_prio_any_Click);
        // 
        // menu_prof_prio_normal
        // 
        this.menu_prof_prio_normal.Checked = true;
        this.menu_prof_prio_normal.Index = 2;
        this.menuImage.SetMenuImage(this.menu_prof_prio_normal, null);
        this.menu_prof_prio_normal.OwnerDraw = true;
        this.menu_prof_prio_normal.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_prio_normal, "menu_prof_prio_normal");
        this.menu_prof_prio_normal.Click += new System.EventHandler(this.menu_prof_prio_any_Click);
        // 
        // menu_prof_prio_belowNormal
        // 
        this.menu_prof_prio_belowNormal.Index = 3;
        this.menuImage.SetMenuImage(this.menu_prof_prio_belowNormal, null);
        this.menu_prof_prio_belowNormal.OwnerDraw = true;
        this.menu_prof_prio_belowNormal.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_prio_belowNormal, "menu_prof_prio_belowNormal");
        this.menu_prof_prio_belowNormal.Click += new System.EventHandler(this.menu_prof_prio_any_Click);
        // 
        // menu_prof_prio_idle
        // 
        this.menu_prof_prio_idle.Index = 4;
        this.menuImage.SetMenuImage(this.menu_prof_prio_idle, null);
        this.menu_prof_prio_idle.OwnerDraw = true;
        this.menu_prof_prio_idle.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_prio_idle, "menu_prof_prio_idle");
        this.menu_prof_prio_idle.Click += new System.EventHandler(this.menu_prof_prio_any_Click);
        // 
        // menu_prof_freeMem
        // 
        this.menu_prof_freeMem.Index = 4;
        this.menuImage.SetMenuImage(this.menu_prof_freeMem, "3");
        this.menu_prof_freeMem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_prof_freeMem_none,
            this.menu_prof_freeMem_64mb,
            this.menu_prof_freeMem_128mb,
            this.menu_prof_freeMem_256mb,
            this.menu_prof_freeMem_384mb,
            this.menu_prof_freeMem_512mb,
            this.menu_prof_freeMem_max});
        this.menu_prof_freeMem.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_freeMem, "menu_prof_freeMem");
        this.menu_prof_freeMem.Popup += new System.EventHandler(this.menu_prof_freeMem_Popup);
        // 
        // menu_prof_freeMem_none
        // 
        this.menu_prof_freeMem_none.Index = 0;
        this.menuImage.SetMenuImage(this.menu_prof_freeMem_none, null);
        this.menu_prof_freeMem_none.OwnerDraw = true;
        this.menu_prof_freeMem_none.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_freeMem_none, "menu_prof_freeMem_none");
        this.menu_prof_freeMem_none.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
        // 
        // menu_prof_freeMem_64mb
        // 
        this.menu_prof_freeMem_64mb.Index = 1;
        this.menuImage.SetMenuImage(this.menu_prof_freeMem_64mb, null);
        this.menu_prof_freeMem_64mb.OwnerDraw = true;
        this.menu_prof_freeMem_64mb.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_freeMem_64mb, "menu_prof_freeMem_64mb");
        this.menu_prof_freeMem_64mb.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
        // 
        // menu_prof_freeMem_128mb
        // 
        this.menu_prof_freeMem_128mb.Index = 2;
        this.menuImage.SetMenuImage(this.menu_prof_freeMem_128mb, null);
        this.menu_prof_freeMem_128mb.OwnerDraw = true;
        this.menu_prof_freeMem_128mb.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_freeMem_128mb, "menu_prof_freeMem_128mb");
        this.menu_prof_freeMem_128mb.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
        // 
        // menu_prof_freeMem_256mb
        // 
        this.menu_prof_freeMem_256mb.Index = 3;
        this.menuImage.SetMenuImage(this.menu_prof_freeMem_256mb, null);
        this.menu_prof_freeMem_256mb.OwnerDraw = true;
        this.menu_prof_freeMem_256mb.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_freeMem_256mb, "menu_prof_freeMem_256mb");
        this.menu_prof_freeMem_256mb.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
        // 
        // menu_prof_freeMem_384mb
        // 
        this.menu_prof_freeMem_384mb.Index = 4;
        this.menuImage.SetMenuImage(this.menu_prof_freeMem_384mb, null);
        this.menu_prof_freeMem_384mb.OwnerDraw = true;
        this.menu_prof_freeMem_384mb.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_freeMem_384mb, "menu_prof_freeMem_384mb");
        this.menu_prof_freeMem_384mb.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
        // 
        // menu_prof_freeMem_512mb
        // 
        this.menu_prof_freeMem_512mb.Index = 5;
        this.menuImage.SetMenuImage(this.menu_prof_freeMem_512mb, null);
        this.menu_prof_freeMem_512mb.OwnerDraw = true;
        this.menu_prof_freeMem_512mb.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_freeMem_512mb, "menu_prof_freeMem_512mb");
        this.menu_prof_freeMem_512mb.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
        // 
        // menu_prof_freeMem_max
        // 
        this.menu_prof_freeMem_max.Index = 6;
        this.menuImage.SetMenuImage(this.menu_prof_freeMem_max, null);
        this.menu_prof_freeMem_max.OwnerDraw = true;
        this.menu_prof_freeMem_max.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_freeMem_max, "menu_prof_freeMem_max");
        this.menu_prof_freeMem_max.Click += new System.EventHandler(this.menu_prof_freeMem_Any_Click);
        // 
        // menu_prof_autoRestore
        // 
        this.menu_prof_autoRestore.Index = 5;
        this.menuImage.SetMenuImage(this.menu_prof_autoRestore, "19");
        this.menu_prof_autoRestore.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_prof_autoRestore_default,
            this.menu_prof_autoRestore_forceOff,
            this.menu_prof_autoRestore_forceDialog,
            this.menu_prof_autoRestore_disableDialog});
        this.menu_prof_autoRestore.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_autoRestore, "menu_prof_autoRestore");
        this.menu_prof_autoRestore.Popup += new System.EventHandler(this.menu_prof_autoRestore_Popup);
        // 
        // menu_prof_autoRestore_default
        // 
        this.menu_prof_autoRestore_default.Index = 0;
        this.menuImage.SetMenuImage(this.menu_prof_autoRestore_default, null);
        this.menu_prof_autoRestore_default.OwnerDraw = true;
        this.menu_prof_autoRestore_default.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_autoRestore_default, "menu_prof_autoRestore_default");
        this.menu_prof_autoRestore_default.Click += new System.EventHandler(this.menu_prof_autoRestore_Any_Click);
        // 
        // menu_prof_autoRestore_forceOff
        // 
        this.menu_prof_autoRestore_forceOff.Index = 1;
        this.menuImage.SetMenuImage(this.menu_prof_autoRestore_forceOff, null);
        this.menu_prof_autoRestore_forceOff.OwnerDraw = true;
        this.menu_prof_autoRestore_forceOff.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_autoRestore_forceOff, "menu_prof_autoRestore_forceOff");
        this.menu_prof_autoRestore_forceOff.Click += new System.EventHandler(this.menu_prof_autoRestore_Any_Click);
        // 
        // menu_prof_autoRestore_forceDialog
        // 
        this.menu_prof_autoRestore_forceDialog.Index = 2;
        this.menuImage.SetMenuImage(this.menu_prof_autoRestore_forceDialog, null);
        this.menu_prof_autoRestore_forceDialog.OwnerDraw = true;
        this.menu_prof_autoRestore_forceDialog.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_autoRestore_forceDialog, "menu_prof_autoRestore_forceDialog");
        this.menu_prof_autoRestore_forceDialog.Click += new System.EventHandler(this.menu_prof_autoRestore_Any_Click);
        // 
        // menu_prof_autoRestore_disableDialog
        // 
        this.menu_prof_autoRestore_disableDialog.Index = 3;
        this.menuImage.SetMenuImage(this.menu_prof_autoRestore_disableDialog, null);
        this.menu_prof_autoRestore_disableDialog.OwnerDraw = true;
        this.menu_prof_autoRestore_disableDialog.RadioCheck = true;
        resources.ApplyResources(this.menu_prof_autoRestore_disableDialog, "menu_prof_autoRestore_disableDialog");
        this.menu_prof_autoRestore_disableDialog.Click += new System.EventHandler(this.menu_prof_autoRestore_Any_Click);
        // 
        // menuItem6
        // 
        this.menuItem6.Index = 6;
        this.menuImage.SetMenuImage(this.menuItem6, null);
        this.menuItem6.OwnerDraw = true;
        resources.ApplyResources(this.menuItem6, "menuItem6");
        // 
        // menu_prof_detectAPI
        // 
        this.menu_prof_detectAPI.Index = 7;
        this.menuImage.SetMenuImage(this.menu_prof_detectAPI, "5");
        this.menu_prof_detectAPI.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_detectAPI, "menu_prof_detectAPI");
        this.menu_prof_detectAPI.Click += new System.EventHandler(this.menu_prof_detectAPI_Click);
        // 
        // menu_prof_tdprofGD
        // 
        this.menu_prof_tdprofGD.Index = 8;
        this.menuImage.SetMenuImage(this.menu_prof_tdprofGD, null);
        this.menu_prof_tdprofGD.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_prof_tdprofGD_create,
            this.menu_prof_tdprofGD_remove,
            this.menu_prof_tdprofGD_help});
        this.menu_prof_tdprofGD.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_tdprofGD, "menu_prof_tdprofGD");
        this.menu_prof_tdprofGD.Popup += new System.EventHandler(this.menu_prof_tdprofGD_Popup);
        // 
        // menu_prof_tdprofGD_create
        // 
        this.menu_prof_tdprofGD_create.Index = 0;
        this.menuImage.SetMenuImage(this.menu_prof_tdprofGD_create, "12");
        this.menu_prof_tdprofGD_create.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_tdprofGD_create, "menu_prof_tdprofGD_create");
        this.menu_prof_tdprofGD_create.Click += new System.EventHandler(this.menu_prof_tdprofGD_create_Click);
        // 
        // menu_prof_tdprofGD_remove
        // 
        this.menu_prof_tdprofGD_remove.Index = 1;
        this.menuImage.SetMenuImage(this.menu_prof_tdprofGD_remove, "11");
        this.menu_prof_tdprofGD_remove.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_tdprofGD_remove, "menu_prof_tdprofGD_remove");
        this.menu_prof_tdprofGD_remove.Click += new System.EventHandler(this.menu_prof_tdprofGD_remove_Click);
        // 
        // menu_prof_tdprofGD_help
        // 
        this.menu_prof_tdprofGD_help.Index = 2;
        this.menuImage.SetMenuImage(this.menu_prof_tdprofGD_help, "10");
        this.menu_prof_tdprofGD_help.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_tdprofGD_help, "menu_prof_tdprofGD_help");
        this.menu_prof_tdprofGD_help.Click += new System.EventHandler(this.menu_prof_tdprofGD_help_Click);
        // 
        // menu_prof_mouseWare
        // 
        this.menu_prof_mouseWare.Index = 9;
        this.menuImage.SetMenuImage(this.menu_prof_mouseWare, null);
        this.menu_prof_mouseWare.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_prof_mouseWare_noAccel});
        this.menu_prof_mouseWare.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_mouseWare, "menu_prof_mouseWare");
        this.menu_prof_mouseWare.Popup += new System.EventHandler(this.menu_prof_mouseWare_Popup);
        // 
        // menu_prof_mouseWare_noAccel
        // 
        this.menu_prof_mouseWare_noAccel.Index = 0;
        this.menuImage.SetMenuImage(this.menu_prof_mouseWare_noAccel, null);
        this.menu_prof_mouseWare_noAccel.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_mouseWare_noAccel, "menu_prof_mouseWare_noAccel");
        this.menu_prof_mouseWare_noAccel.Click += new System.EventHandler(this.menu_prof_mouseWare_noAccel_Click);
        // 
        // menuItem9
        // 
        this.menuItem9.Index = 10;
        this.menuImage.SetMenuImage(this.menuItem9, null);
        this.menuItem9.OwnerDraw = true;
        resources.ApplyResources(this.menuItem9, "menuItem9");
        // 
        // menu_prof_setSpecName
        // 
        this.menu_prof_setSpecName.Index = 11;
        this.menuImage.SetMenuImage(this.menu_prof_setSpecName, null);
        this.menu_prof_setSpecName.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_setSpecName, "menu_prof_setSpecName");
        this.menu_prof_setSpecName.Click += new System.EventHandler(this.menu_prof_setSpecName_Click);
        // 
        // menu_prof_importProfile
        // 
        this.menu_prof_importProfile.Index = 12;
        this.menuImage.SetMenuImage(this.menu_prof_importProfile, null);
        this.menu_prof_importProfile.OwnerDraw = true;
        resources.ApplyResources(this.menu_prof_importProfile, "menu_prof_importProfile");
        // 
        // menuItem24
        // 
        this.menuItem24.Index = 13;
        this.menuImage.SetMenuImage(this.menuItem24, null);
        this.menuItem24.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_profs_templates_turnTo2D,
            this.menu_profs_templates_clockOnly});
        this.menuItem24.OwnerDraw = true;
        resources.ApplyResources(this.menuItem24, "menuItem24");
        // 
        // menu_profs_templates_turnTo2D
        // 
        this.menu_profs_templates_turnTo2D.Index = 0;
        this.menuImage.SetMenuImage(this.menu_profs_templates_turnTo2D, null);
        this.menu_profs_templates_turnTo2D.OwnerDraw = true;
        resources.ApplyResources(this.menu_profs_templates_turnTo2D, "menu_profs_templates_turnTo2D");
        this.menu_profs_templates_turnTo2D.Click += new System.EventHandler(this.menu_profs_templates_turnTo2D_Click);
        // 
        // menu_profs_templates_clockOnly
        // 
        this.menu_profs_templates_clockOnly.Index = 1;
        this.menuImage.SetMenuImage(this.menu_profs_templates_clockOnly, null);
        this.menu_profs_templates_clockOnly.OwnerDraw = true;
        resources.ApplyResources(this.menu_profs_templates_clockOnly, "menu_profs_templates_clockOnly");
        this.menu_profs_templates_clockOnly.Click += new System.EventHandler(this.menu_profs_templates_clockOnly_Click);
        // 
        // menuItem28
        // 
        this.menuItem28.Index = 14;
        this.menuImage.SetMenuImage(this.menuItem28, null);
        this.menuItem28.OwnerDraw = true;
        resources.ApplyResources(this.menuItem28, "menuItem28");
        // 
        // menu_profs
        // 
        this.menu_profs.Index = 4;
        this.menu_profs.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_profs_filterBySpecName,
            this.menu_profs_cmds,
            this.menu_profs_hotkeys,
            this.menu_profs_exim});
        resources.ApplyResources(this.menu_profs, "menu_profs");
        // 
        // menu_profs_filterBySpecName
        // 
        this.menu_profs_filterBySpecName.Index = 0;
        this.menuImage.SetMenuImage(this.menu_profs_filterBySpecName, null);
        this.menu_profs_filterBySpecName.OwnerDraw = true;
        resources.ApplyResources(this.menu_profs_filterBySpecName, "menu_profs_filterBySpecName");
        this.menu_profs_filterBySpecName.Click += new System.EventHandler(this.menu_prof_filterBySpecName_Click);
        // 
        // menu_profs_cmds
        // 
        this.menu_profs_cmds.Index = 1;
        this.menuImage.SetMenuImage(this.menu_profs_cmds, "26");
        this.menu_profs_cmds.OwnerDraw = true;
        resources.ApplyResources(this.menu_profs_cmds, "menu_profs_cmds");
        this.menu_profs_cmds.Click += new System.EventHandler(this.menu_profs_cmds_Click);
        // 
        // menu_profs_hotkeys
        // 
        this.menu_profs_hotkeys.Index = 2;
        this.menuImage.SetMenuImage(this.menu_profs_hotkeys, "24");
        this.menu_profs_hotkeys.OwnerDraw = true;
        resources.ApplyResources(this.menu_profs_hotkeys, "menu_profs_hotkeys");
        this.menu_profs_hotkeys.Click += new System.EventHandler(this.menu_prof_hotkeys_Click);
        // 
        // menu_profs_exim
        // 
        this.menu_profs_exim.Index = 3;
        this.menuImage.SetMenuImage(this.menu_profs_exim, null);
        this.menu_profs_exim.OwnerDraw = true;
        resources.ApplyResources(this.menu_profs_exim, "menu_profs_exim");
        this.menu_profs_exim.Click += new System.EventHandler(this.menu_profs_exim_Click);
        // 
        // menu_help
        // 
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
        resources.ApplyResources(this.menu_help, "menu_help");
        // 
        // menu_help_tooltips
        // 
        this.menu_help_tooltips.Checked = true;
        this.menu_help_tooltips.Index = 0;
        this.menuImage.SetMenuImage(this.menu_help_tooltips, null);
        this.menu_help_tooltips.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_tooltips, "menu_help_tooltips");
        this.menu_help_tooltips.Click += new System.EventHandler(this.menu_help_tooltips_Click);
        // 
        // menuItem4
        // 
        this.menuItem4.Index = 1;
        this.menuImage.SetMenuImage(this.menuItem4, null);
        this.menuItem4.OwnerDraw = true;
        resources.ApplyResources(this.menuItem4, "menuItem4");
        // 
        // menu_help_intro
        // 
        this.menu_help_intro.Index = 2;
        this.menuImage.SetMenuImage(this.menu_help_intro, null);
        this.menu_help_intro.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_intro, "menu_help_intro");
        this.menu_help_intro.Click += new System.EventHandler(this.menu_help_intro_Click);
        // 
        // menu_help_news
        // 
        this.menu_help_news.Index = 3;
        this.menuImage.SetMenuImage(this.menu_help_news, null);
        this.menu_help_news.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_news, "menu_help_news");
        this.menu_help_news.Click += new System.EventHandler(this.menu_help_news_Click);
        // 
        // menu_help_todo
        // 
        this.menu_help_todo.Index = 4;
        this.menuImage.SetMenuImage(this.menu_help_todo, null);
        this.menu_help_todo.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_todo, "menu_help_todo");
        this.menu_help_todo.Click += new System.EventHandler(this.menu_help_todo_Click);
        // 
        // menu_help_manual
        // 
        this.menu_help_manual.Index = 5;
        this.menuImage.SetMenuImage(this.menu_help_manual, "27");
        this.menu_help_manual.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_manual, "menu_help_manual");
        this.menu_help_manual.Click += new System.EventHandler(this.menu_help_manual_Click);
        // 
        // menuItem18
        // 
        this.menuItem18.Index = 6;
        this.menuImage.SetMenuImage(this.menuItem18, null);
        this.menuItem18.OwnerDraw = true;
        resources.ApplyResources(this.menuItem18, "menuItem18");
        // 
        // menu_help_visit_home
        // 
        this.menu_help_visit_home.Index = 7;
        this.menuImage.SetMenuImage(this.menu_help_visit_home, "13");
        this.menu_help_visit_home.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_visit_home, "menu_help_visit_home");
        this.menu_help_visit_home.Click += new System.EventHandler(this.menu_help_visit_home_Click);
        // 
        // menu_help_visit_thread
        // 
        this.menu_help_visit_thread.Index = 8;
        this.menuImage.SetMenuImage(this.menu_help_visit_thread, "14");
        this.menu_help_visit_thread.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_help_visit_thread_3DC,
            this.menu_help_visit_thread_R3D});
        this.menu_help_visit_thread.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_visit_thread, "menu_help_visit_thread");
        // 
        // menu_help_visit_thread_3DC
        // 
        this.menu_help_visit_thread_3DC.Index = 0;
        this.menuImage.SetMenuImage(this.menu_help_visit_thread_3DC, "20");
        this.menu_help_visit_thread_3DC.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_visit_thread_3DC, "menu_help_visit_thread_3DC");
        this.menu_help_visit_thread_3DC.Click += new System.EventHandler(this.menu_help_visit_thread_3DC_Click);
        // 
        // menu_help_visit_thread_R3D
        // 
        this.menu_help_visit_thread_R3D.Index = 1;
        this.menuImage.SetMenuImage(this.menu_help_visit_thread_R3D, "21");
        this.menu_help_visit_thread_R3D.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_visit_thread_R3D, "menu_help_visit_thread_R3D");
        this.menu_help_visit_thread_R3D.Click += new System.EventHandler(this.menu_help_visit_thread_R3D_Click);
        // 
        // menu_help_ati_visit_radeonFAQ
        // 
        this.menu_help_ati_visit_radeonFAQ.Index = 9;
        this.menuImage.SetMenuImage(this.menu_help_ati_visit_radeonFAQ, null);
        this.menu_help_ati_visit_radeonFAQ.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_ati_visit_radeonFAQ, "menu_help_ati_visit_radeonFAQ");
        this.menu_help_ati_visit_radeonFAQ.Click += new System.EventHandler(this.menu_help_ati_visit_radeonFAQ_Click);
        // 
        // menu_help_mailto_author
        // 
        this.menu_help_mailto_author.Index = 10;
        this.menuImage.SetMenuImage(this.menu_help_mailto_author, "7");
        this.menu_help_mailto_author.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_mailto_author, "menu_help_mailto_author");
        this.menu_help_mailto_author.Click += new System.EventHandler(this.menu_help_mailto_author_Click);
        // 
        // menuItem10
        // 
        this.menuItem10.Index = 11;
        this.menuImage.SetMenuImage(this.menuItem10, null);
        this.menuItem10.OwnerDraw = true;
        resources.ApplyResources(this.menuItem10, "menuItem10");
        // 
        // menu_help_about
        // 
        this.menu_help_about.Index = 12;
        this.menuImage.SetMenuImage(this.menu_help_about, "18");
        this.menu_help_about.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_about, "menu_help_about");
        this.menu_help_about.Click += new System.EventHandler(this.menu_help_about_Click);
        // 
        // menu_help_showLicense
        // 
        this.menu_help_showLicense.Index = 13;
        this.menuImage.SetMenuImage(this.menu_help_showLicense, null);
        this.menu_help_showLicense.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_showLicense, "menu_help_showLicense");
        this.menu_help_showLicense.Click += new System.EventHandler(this.menu_help_showLicense_Click);
        // 
        // menu_help_nonWarranty
        // 
        this.menu_help_nonWarranty.Index = 14;
        this.menuImage.SetMenuImage(this.menu_help_nonWarranty, null);
        this.menu_help_nonWarranty.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_nonWarranty, "menu_help_nonWarranty");
        this.menu_help_nonWarranty.Click += new System.EventHandler(this.menu_help_nonWarranty_Click);
        // 
        // menuItem2
        // 
        this.menuItem2.Index = 15;
        this.menuImage.SetMenuImage(this.menuItem2, null);
        this.menuItem2.OwnerDraw = true;
        resources.ApplyResources(this.menuItem2, "menuItem2");
        // 
        // menu_help_report
        // 
        this.menu_help_report.Index = 16;
        this.menuImage.SetMenuImage(this.menu_help_report, null);
        this.menu_help_report.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_help_report_nvclockDebug,
            this.menu_help_report_r6clockDebug,
            this.menu_help_report_displayInfo});
        this.menu_help_report.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_report, "menu_help_report");
        // 
        // menu_help_report_nvclockDebug
        // 
        this.menu_help_report_nvclockDebug.Index = 0;
        this.menuImage.SetMenuImage(this.menu_help_report_nvclockDebug, null);
        this.menu_help_report_nvclockDebug.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_report_nvclockDebug, "menu_help_report_nvclockDebug");
        this.menu_help_report_nvclockDebug.Click += new System.EventHandler(this.menu_help_report_nvclockDebug_Click);
        // 
        // menu_help_report_r6clockDebug
        // 
        this.menu_help_report_r6clockDebug.Index = 1;
        this.menuImage.SetMenuImage(this.menu_help_report_r6clockDebug, null);
        this.menu_help_report_r6clockDebug.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_report_r6clockDebug, "menu_help_report_r6clockDebug");
        this.menu_help_report_r6clockDebug.Click += new System.EventHandler(this.menu_help_report_r6clockDebug_Click);
        // 
        // menu_help_report_displayInfo
        // 
        this.menu_help_report_displayInfo.Index = 2;
        this.menuImage.SetMenuImage(this.menu_help_report_displayInfo, null);
        this.menu_help_report_displayInfo.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_report_displayInfo, "menu_help_report_displayInfo");
        this.menu_help_report_displayInfo.Click += new System.EventHandler(this.menu_help_report_displayInfo_Click);
        // 
        // menu_help_log
        // 
        this.menu_help_log.Index = 17;
        this.menuImage.SetMenuImage(this.menu_help_log, null);
        this.menu_help_log.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_log, "menu_help_log");
        this.menu_help_log.Click += new System.EventHandler(this.menu_help_log_Click);
        // 
        // menu_help_sysInfo
        // 
        this.menu_help_sysInfo.Index = 18;
        this.menuImage.SetMenuImage(this.menu_help_sysInfo, "22");
        this.menu_help_sysInfo.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_sysInfo, "menu_help_sysInfo");
        this.menu_help_sysInfo.Click += new System.EventHandler(this.menu_help_sysInfo_Click);
        // 
        // menu_sfEdit
        // 
        this.menu_sfEdit.Index = 19;
        this.menuImage.SetMenuImage(this.menu_sfEdit, null);
        this.menu_sfEdit.OwnerDraw = true;
        resources.ApplyResources(this.menu_sfEdit, "menu_sfEdit");
        this.menu_sfEdit.Click += new System.EventHandler(this.menu_sfEdit_Click);
        // 
        // menu_exp
        // 
        this.menu_exp.Index = 6;
        this.menu_exp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuI_exp_findProc,
            this.menu_help_helpButton,
            this.menu_file_debugThrow,
            this.menu_file_debugCrashMe,
            this.menu_file_restore3d,
            this.menu_exp_test1,
            this.menuItem13});
        resources.ApplyResources(this.menu_exp, "menu_exp");
        // 
        // menuI_exp_findProc
        // 
        this.menuI_exp_findProc.Index = 0;
        this.menuImage.SetMenuImage(this.menuI_exp_findProc, null);
        this.menuI_exp_findProc.OwnerDraw = true;
        resources.ApplyResources(this.menuI_exp_findProc, "menuI_exp_findProc");
        this.menuI_exp_findProc.Click += new System.EventHandler(this.menuI_exp_findProc_Click);
        // 
        // menu_help_helpButton
        // 
        this.menu_help_helpButton.Index = 1;
        this.menuImage.SetMenuImage(this.menu_help_helpButton, null);
        this.menu_help_helpButton.OwnerDraw = true;
        resources.ApplyResources(this.menu_help_helpButton, "menu_help_helpButton");
        this.menu_help_helpButton.Click += new System.EventHandler(this.menu_help_helpButton_Click);
        // 
        // menu_file_debugThrow
        // 
        this.menu_file_debugThrow.Index = 2;
        this.menuImage.SetMenuImage(this.menu_file_debugThrow, null);
        this.menu_file_debugThrow.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_debugThrow, "menu_file_debugThrow");
        this.menu_file_debugThrow.Click += new System.EventHandler(this.menu_file_debugThrow_Click);
        // 
        // menu_file_debugCrashMe
        // 
        this.menu_file_debugCrashMe.Index = 3;
        this.menuImage.SetMenuImage(this.menu_file_debugCrashMe, null);
        this.menu_file_debugCrashMe.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_debugCrashMe, "menu_file_debugCrashMe");
        this.menu_file_debugCrashMe.Click += new System.EventHandler(this.menu_file_debugCrashMe_Click);
        // 
        // menu_file_restore3d
        // 
        this.menu_file_restore3d.Index = 4;
        this.menuImage.SetMenuImage(this.menu_file_restore3d, null);
        this.menu_file_restore3d.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_file_restore3d_saveToFile,
            this.menu_file_restore3d_loadLastSaved,
            this.menu_file_restore3d_loadSavedByRun,
            this.menu_file_restore3d_loadAutoSaved});
        this.menu_file_restore3d.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_restore3d, "menu_file_restore3d");
        // 
        // menu_file_restore3d_saveToFile
        // 
        this.menu_file_restore3d_saveToFile.Index = 0;
        this.menuImage.SetMenuImage(this.menu_file_restore3d_saveToFile, null);
        this.menu_file_restore3d_saveToFile.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_restore3d_saveToFile, "menu_file_restore3d_saveToFile");
        this.menu_file_restore3d_saveToFile.Click += new System.EventHandler(this.menu_file_restore3d_saveToFile_Click);
        // 
        // menu_file_restore3d_loadLastSaved
        // 
        this.menu_file_restore3d_loadLastSaved.Index = 1;
        this.menuImage.SetMenuImage(this.menu_file_restore3d_loadLastSaved, null);
        this.menu_file_restore3d_loadLastSaved.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_restore3d_loadLastSaved, "menu_file_restore3d_loadLastSaved");
        this.menu_file_restore3d_loadLastSaved.Click += new System.EventHandler(this.menu_file_restore3d_loadLastSaved_Click);
        // 
        // menu_file_restore3d_loadSavedByRun
        // 
        this.menu_file_restore3d_loadSavedByRun.Index = 2;
        this.menuImage.SetMenuImage(this.menu_file_restore3d_loadSavedByRun, null);
        this.menu_file_restore3d_loadSavedByRun.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_restore3d_loadSavedByRun, "menu_file_restore3d_loadSavedByRun");
        this.menu_file_restore3d_loadSavedByRun.Click += new System.EventHandler(this.menu_file_restore3d_loadSavedByRun_Click);
        // 
        // menu_file_restore3d_loadAutoSaved
        // 
        this.menu_file_restore3d_loadAutoSaved.Index = 3;
        this.menuImage.SetMenuImage(this.menu_file_restore3d_loadAutoSaved, null);
        this.menu_file_restore3d_loadAutoSaved.OwnerDraw = true;
        resources.ApplyResources(this.menu_file_restore3d_loadAutoSaved, "menu_file_restore3d_loadAutoSaved");
        this.menu_file_restore3d_loadAutoSaved.Click += new System.EventHandler(this.menu_file_restore3d_loadAutoSaved_Click);
        // 
        // menu_exp_test1
        // 
        this.menu_exp_test1.Index = 5;
        this.menuImage.SetMenuImage(this.menu_exp_test1, null);
        this.menu_exp_test1.OwnerDraw = true;
        resources.ApplyResources(this.menu_exp_test1, "menu_exp_test1");
        this.menu_exp_test1.Click += new System.EventHandler(this.menu_exp_test1_Click);
        // 
        // menuItem13
        // 
        this.menuItem13.Index = 6;
        this.menuImage.SetMenuImage(this.menuItem13, null);
        this.menuItem13.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem22});
        this.menuItem13.OwnerDraw = true;
        resources.ApplyResources(this.menuItem13, "menuItem13");
        // 
        // menuItem22
        // 
        this.menuItem22.Index = 0;
        this.menuImage.SetMenuImage(this.menuItem22, null);
        this.menuItem22.OwnerDraw = true;
        resources.ApplyResources(this.menuItem22, "menuItem22");
        this.menuItem22.Click += new System.EventHandler(this.menuItem22_Click);
        // 
        // tabCtrl
        // 
        this.tabCtrl.Controls.Add(this.tab_main);
        this.tabCtrl.Controls.Add(this.tab_files);
        this.tabCtrl.Controls.Add(this.tab_extra_d3d);
        this.tabCtrl.Controls.Add(this.tab_extra_ogl);
        this.tabCtrl.Controls.Add(this.tab_summary);
        this.tabCtrl.Controls.Add(this.tab_clocking);
        this.tabCtrl.Controls.Add(this.tab_exp);
        resources.ApplyResources(this.tabCtrl, "tabCtrl");
        this.tabCtrl.Name = "tabCtrl";
        this.tabCtrl.SelectedIndex = 0;
        this.tabCtrl.SelectedIndexChanged += new System.EventHandler(this.tabCtrl_SelectedIndexChanged);
        this.tabCtrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabCtrl_KeyDown);
        // 
        // tab_main
        // 
        this.tab_main.Controls.Add(this.group_main_d3d);
        resources.ApplyResources(this.tab_main, "tab_main");
        this.tab_main.Name = "tab_main";
        // 
        // tab_files
        // 
        this.tab_files.Controls.Add(this.panel_prof_files);
        resources.ApplyResources(this.tab_files, "tab_files");
        this.tab_files.Name = "tab_files";
        // 
        // panel_prof_files
        // 
        resources.ApplyResources(this.panel_prof_files, "panel_prof_files");
        this.panel_prof_files.Controls.Add(this.label6);
        this.panel_prof_files.Controls.Add(this.num_prof_imgDrive);
        this.panel_prof_files.Controls.Add(this.button_prof_choose_img);
        this.panel_prof_files.Controls.Add(this.button_prof_choose_exe);
        this.panel_prof_files.Controls.Add(this.panel_gameExe);
        this.panel_prof_files.Controls.Add(this.combo_prof_img);
        this.panel_prof_files.Controls.Add(this.check_prof_shellLink);
        this.panel_prof_files.Controls.Add(this.button_prof_mount_img);
        this.panel_prof_files.Controls.Add(this.button_prof_make_link);
        this.panel_prof_files.Name = "panel_prof_files";
        // 
        // label6
        // 
        resources.ApplyResources(this.label6, "label6");
        this.label6.Name = "label6";
        // 
        // num_prof_imgDrive
        // 
        resources.ApplyResources(this.num_prof_imgDrive, "num_prof_imgDrive");
        this.num_prof_imgDrive.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
        this.num_prof_imgDrive.Name = "num_prof_imgDrive";
        this.num_prof_imgDrive.ValueChanged += new System.EventHandler(this.num_prof_imgDrive_ValueChanged);
        // 
        // panel_gameExe
        // 
        resources.ApplyResources(this.panel_gameExe, "panel_gameExe");
        this.panel_gameExe.Controls.Add(this.text_prof_exe_args);
        this.panel_gameExe.Controls.Add(this.splitter_prof_gameExe);
        this.panel_gameExe.Controls.Add(this.text_prof_exe_path);
        this.panel_gameExe.Name = "panel_gameExe";
        // 
        // splitter_prof_gameExe
        // 
        resources.ApplyResources(this.splitter_prof_gameExe, "splitter_prof_gameExe");
        this.splitter_prof_gameExe.Name = "splitter_prof_gameExe";
        this.splitter_prof_gameExe.TabStop = false;
        // 
        // check_prof_shellLink
        // 
        resources.ApplyResources(this.check_prof_shellLink, "check_prof_shellLink");
        this.check_prof_shellLink.Name = "check_prof_shellLink";
        this.toolTip.SetToolTip(this.check_prof_shellLink, resources.GetString("check_prof_shellLink.ToolTip"));
        this.check_prof_shellLink.CheckedChanged += new System.EventHandler(this.check_prof_shellLink_CheckedChanged);
        // 
        // tab_extra_d3d
        // 
        this.tab_extra_d3d.Controls.Add(this.group_extra_d3d);
        this.tab_extra_d3d.Controls.Add(this.group_extra_d3d_2);
        resources.ApplyResources(this.tab_extra_d3d, "tab_extra_d3d");
        this.tab_extra_d3d.Name = "tab_extra_d3d";
        this.toolTip.SetToolTip(this.tab_extra_d3d, resources.GetString("tab_extra_d3d.ToolTip"));
        // 
        // group_extra_d3d_2
        // 
        this.group_extra_d3d_2.Controls.Add(this.label_extra2_prof_d3d);
        this.group_extra_d3d_2.Controls.Add(this.label_extra2_curr_d3d);
        this.group_extra_d3d_2.FlatStyle = System.Windows.Forms.FlatStyle.System;
        resources.ApplyResources(this.group_extra_d3d_2, "group_extra_d3d_2");
        this.group_extra_d3d_2.Name = "group_extra_d3d_2";
        this.group_extra_d3d_2.TabStop = false;
        // 
        // tab_extra_ogl
        // 
        this.tab_extra_ogl.Controls.Add(this.group_extra_ogl);
        this.tab_extra_ogl.Controls.Add(this.group_extra_ogl_2);
        resources.ApplyResources(this.tab_extra_ogl, "tab_extra_ogl");
        this.tab_extra_ogl.Name = "tab_extra_ogl";
        this.toolTip.SetToolTip(this.tab_extra_ogl, resources.GetString("tab_extra_ogl.ToolTip"));
        // 
        // group_extra_ogl_2
        // 
        this.group_extra_ogl_2.Controls.Add(this.label_extra2_curr_ogl);
        this.group_extra_ogl_2.Controls.Add(this.label_extra2_prof_ogl);
        this.group_extra_ogl_2.FlatStyle = System.Windows.Forms.FlatStyle.System;
        resources.ApplyResources(this.group_extra_ogl_2, "group_extra_ogl_2");
        this.group_extra_ogl_2.Name = "group_extra_ogl_2";
        this.group_extra_ogl_2.TabStop = false;
        // 
        // tab_summary
        // 
        this.tab_summary.Controls.Add(this.text_summary);
        resources.ApplyResources(this.tab_summary, "tab_summary");
        this.tab_summary.Name = "tab_summary";
        this.toolTip.SetToolTip(this.tab_summary, resources.GetString("tab_summary.ToolTip"));
        // 
        // text_summary
        // 
        this.text_summary.DetectUrls = false;
        resources.ApplyResources(this.text_summary, "text_summary");
        this.text_summary.Name = "text_summary";
        this.text_summary.ReadOnly = true;
        this.text_summary.TabStop = false;
        this.text_summary.VisibleChanged += new System.EventHandler(this.text_summary_VisibleChanged);
        // 
        // tab_clocking
        // 
        this.tab_clocking.Controls.Add(this.group_clocking_curr);
        this.tab_clocking.Controls.Add(this.splitter_clocking);
        this.tab_clocking.Controls.Add(this.group_clocking_prof);
        resources.ApplyResources(this.tab_clocking, "tab_clocking");
        this.tab_clocking.Name = "tab_clocking";
        this.toolTip.SetToolTip(this.tab_clocking, resources.GetString("tab_clocking.ToolTip"));
        // 
        // group_clocking_curr
        // 
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
        resources.ApplyResources(this.group_clocking_curr, "group_clocking_curr");
        this.group_clocking_curr.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.group_clocking_curr.Name = "group_clocking_curr";
        this.group_clocking_curr.TabStop = false;
        // 
        // button_clocking_set
        // 
        resources.ApplyResources(this.button_clocking_set, "button_clocking_set");
        this.button_clocking_set.Name = "button_clocking_set";
        this.toolTip.SetToolTip(this.button_clocking_set, resources.GetString("button_clocking_set.ToolTip"));
        this.button_clocking_set.Click += new System.EventHandler(this.button_clocking_set_Click);
        // 
        // button_clocking_reset
        // 
        resources.ApplyResources(this.button_clocking_reset, "button_clocking_reset");
        this.button_clocking_reset.Name = "button_clocking_reset";
        this.toolTip.SetToolTip(this.button_clocking_reset, resources.GetString("button_clocking_reset.ToolTip"));
        this.button_clocking_reset.Click += new System.EventHandler(this.button_clocking_reset_Click);
        // 
        // text_clocking_curr_core
        // 
        this.text_clocking_curr_core.BorderStyle = System.Windows.Forms.BorderStyle.None;
        resources.ApplyResources(this.text_clocking_curr_core, "text_clocking_curr_core");
        this.text_clocking_curr_core.Name = "text_clocking_curr_core";
        this.text_clocking_curr_core.ReadOnly = true;
        this.text_clocking_curr_core.TabStop = false;
        this.text_clocking_curr_core.TextChanged += new System.EventHandler(this.text_clocking_curr_core_TextChanged);
        // 
        // label3
        // 
        resources.ApplyResources(this.label3, "label3");
        this.label3.Name = "label3";
        this.toolTip.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
        // 
        // track_clocking_curr_mem
        // 
        resources.ApplyResources(this.track_clocking_curr_mem, "track_clocking_curr_mem");
        this.track_clocking_curr_mem.Maximum = 320;
        this.track_clocking_curr_mem.Minimum = 200;
        this.track_clocking_curr_mem.Name = "track_clocking_curr_mem";
        this.track_clocking_curr_mem.TickFrequency = 10;
        this.track_clocking_curr_mem.TickStyle = System.Windows.Forms.TickStyle.None;
        this.track_clocking_curr_mem.Value = 200;
        this.track_clocking_curr_mem.Scroll += new System.EventHandler(this.track_clocking_curr_mem_Scroll);
        // 
        // track_clocking_curr_core
        // 
        resources.ApplyResources(this.track_clocking_curr_core, "track_clocking_curr_core");
        this.track_clocking_curr_core.Maximum = 400;
        this.track_clocking_curr_core.Minimum = 200;
        this.track_clocking_curr_core.Name = "track_clocking_curr_core";
        this.track_clocking_curr_core.TickFrequency = 10;
        this.track_clocking_curr_core.TickStyle = System.Windows.Forms.TickStyle.None;
        this.track_clocking_curr_core.Value = 200;
        this.track_clocking_curr_core.VisibleChanged += new System.EventHandler(this.track_clocking_curr_core_VisibleChanged);
        this.track_clocking_curr_core.Scroll += new System.EventHandler(this.track_clocking_core_Scroll);
        // 
        // label1
        // 
        resources.ApplyResources(this.label1, "label1");
        this.label1.Name = "label1";
        this.toolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
        // 
        // text_clocking_curr_mem
        // 
        this.text_clocking_curr_mem.BorderStyle = System.Windows.Forms.BorderStyle.None;
        resources.ApplyResources(this.text_clocking_curr_mem, "text_clocking_curr_mem");
        this.text_clocking_curr_mem.Name = "text_clocking_curr_mem";
        this.text_clocking_curr_mem.ReadOnly = true;
        this.text_clocking_curr_mem.TabStop = false;
        this.text_clocking_curr_mem.TextChanged += new System.EventHandler(this.text_clocking_curr_mem_TextChanged);
        // 
        // button_clocking_refresh
        // 
        resources.ApplyResources(this.button_clocking_refresh, "button_clocking_refresh");
        this.button_clocking_refresh.Name = "button_clocking_refresh";
        this.toolTip.SetToolTip(this.button_clocking_refresh, resources.GetString("button_clocking_refresh.ToolTip"));
        this.button_clocking_refresh.Click += new System.EventHandler(this.button_clocking_refresh_Click);
        // 
        // group_clocking_current_presets
        // 
        resources.ApplyResources(this.group_clocking_current_presets, "group_clocking_current_presets");
        this.group_clocking_current_presets.Controls.Add(this.button_clocking_curr_preFast);
        this.group_clocking_current_presets.Controls.Add(this.button_clocking_curr_preUltra);
        this.group_clocking_current_presets.Controls.Add(this.button_clocking_curr_preNormal);
        this.group_clocking_current_presets.Controls.Add(this.button_clocking_curr_preSlow);
        this.group_clocking_current_presets.Name = "group_clocking_current_presets";
        this.group_clocking_current_presets.TabStop = false;
        // 
        // button_clocking_curr_preFast
        // 
        resources.ApplyResources(this.button_clocking_curr_preFast, "button_clocking_curr_preFast");
        this.button_clocking_curr_preFast.Name = "button_clocking_curr_preFast";
        this.button_clocking_curr_preFast.Tag = "3";
        this.toolTip.SetToolTip(this.button_clocking_curr_preFast, resources.GetString("button_clocking_curr_preFast.ToolTip"));
        this.button_clocking_curr_preFast.Click += new System.EventHandler(this.button_clocking_curr_preFast_Click);
        // 
        // button_clocking_curr_preUltra
        // 
        resources.ApplyResources(this.button_clocking_curr_preUltra, "button_clocking_curr_preUltra");
        this.button_clocking_curr_preUltra.Name = "button_clocking_curr_preUltra";
        this.button_clocking_curr_preUltra.Tag = "4";
        this.toolTip.SetToolTip(this.button_clocking_curr_preUltra, resources.GetString("button_clocking_curr_preUltra.ToolTip"));
        this.button_clocking_curr_preUltra.Click += new System.EventHandler(this.button_clocking_curr_preUltra_Click);
        // 
        // button_clocking_curr_preNormal
        // 
        resources.ApplyResources(this.button_clocking_curr_preNormal, "button_clocking_curr_preNormal");
        this.button_clocking_curr_preNormal.Name = "button_clocking_curr_preNormal";
        this.button_clocking_curr_preNormal.Tag = "2";
        this.toolTip.SetToolTip(this.button_clocking_curr_preNormal, resources.GetString("button_clocking_curr_preNormal.ToolTip"));
        this.button_clocking_curr_preNormal.Click += new System.EventHandler(this.button_clocking_curr_preNormal_Click);
        // 
        // button_clocking_curr_preSlow
        // 
        resources.ApplyResources(this.button_clocking_curr_preSlow, "button_clocking_curr_preSlow");
        this.button_clocking_curr_preSlow.Name = "button_clocking_curr_preSlow";
        this.button_clocking_curr_preSlow.Tag = "";
        this.toolTip.SetToolTip(this.button_clocking_curr_preSlow, resources.GetString("button_clocking_curr_preSlow.ToolTip"));
        this.button_clocking_curr_preSlow.Click += new System.EventHandler(this.button_clocking_curr_preSlow_Click);
        // 
        // splitter_clocking
        // 
        resources.ApplyResources(this.splitter_clocking, "splitter_clocking");
        this.splitter_clocking.Name = "splitter_clocking";
        this.splitter_clocking.TabStop = false;
        this.splitter_clocking.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter_clocking_SplitterMoved);
        // 
        // group_clocking_prof
        // 
        this.group_clocking_prof.Controls.Add(this.panel_clocking_prof_clocks);
        this.group_clocking_prof.Controls.Add(this.label5);
        this.group_clocking_prof.Controls.Add(this.combo_clocking_prof_presets);
        resources.ApplyResources(this.group_clocking_prof, "group_clocking_prof");
        this.group_clocking_prof.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.group_clocking_prof.Name = "group_clocking_prof";
        this.group_clocking_prof.TabStop = false;
        // 
        // panel_clocking_prof_clocks
        // 
        resources.ApplyResources(this.panel_clocking_prof_clocks, "panel_clocking_prof_clocks");
        this.panel_clocking_prof_clocks.Controls.Add(this.check_clocking_prof_mem);
        this.panel_clocking_prof_clocks.Controls.Add(this.text_clocking_prof_mem);
        this.panel_clocking_prof_clocks.Controls.Add(this.track_clocking_prof_mem);
        this.panel_clocking_prof_clocks.Controls.Add(this.text_clocking_prof_core);
        this.panel_clocking_prof_clocks.Controls.Add(this.label_clocking_prof_mem);
        this.panel_clocking_prof_clocks.Controls.Add(this.track_clocking_prof_core);
        this.panel_clocking_prof_clocks.Controls.Add(this.check_clocking_prof_core);
        this.panel_clocking_prof_clocks.Controls.Add(this.button_clocking_disable);
        this.panel_clocking_prof_clocks.Controls.Add(this.label_clocking_prof_core);
        this.panel_clocking_prof_clocks.Name = "panel_clocking_prof_clocks";
        // 
        // check_clocking_prof_mem
        // 
        resources.ApplyResources(this.check_clocking_prof_mem, "check_clocking_prof_mem");
        this.check_clocking_prof_mem.Name = "check_clocking_prof_mem";
        this.toolTip.SetToolTip(this.check_clocking_prof_mem, resources.GetString("check_clocking_prof_mem.ToolTip"));
        this.check_clocking_prof_mem.CheckedChanged += new System.EventHandler(this.check_clocking_prof_mem_CheckedChanged);
        // 
        // text_clocking_prof_mem
        // 
        resources.ApplyResources(this.text_clocking_prof_mem, "text_clocking_prof_mem");
        this.text_clocking_prof_mem.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.text_clocking_prof_mem.Name = "text_clocking_prof_mem";
        this.text_clocking_prof_mem.ReadOnly = true;
        this.text_clocking_prof_mem.TabStop = false;
        this.text_clocking_prof_mem.TextChanged += new System.EventHandler(this.text_clocking_prof_mem_TextChanged);
        // 
        // track_clocking_prof_mem
        // 
        resources.ApplyResources(this.track_clocking_prof_mem, "track_clocking_prof_mem");
        this.track_clocking_prof_mem.Maximum = 320;
        this.track_clocking_prof_mem.Minimum = 200;
        this.track_clocking_prof_mem.Name = "track_clocking_prof_mem";
        this.track_clocking_prof_mem.TickFrequency = 10;
        this.track_clocking_prof_mem.TickStyle = System.Windows.Forms.TickStyle.None;
        this.track_clocking_prof_mem.Value = 200;
        this.track_clocking_prof_mem.Scroll += new System.EventHandler(this.track_clocking_prof_mem_ValueChanged);
        // 
        // text_clocking_prof_core
        // 
        resources.ApplyResources(this.text_clocking_prof_core, "text_clocking_prof_core");
        this.text_clocking_prof_core.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.text_clocking_prof_core.Name = "text_clocking_prof_core";
        this.text_clocking_prof_core.ReadOnly = true;
        this.text_clocking_prof_core.TabStop = false;
        this.text_clocking_prof_core.TextChanged += new System.EventHandler(this.text_clocking_prof_core_TextChanged);
        // 
        // label_clocking_prof_mem
        // 
        resources.ApplyResources(this.label_clocking_prof_mem, "label_clocking_prof_mem");
        this.label_clocking_prof_mem.Name = "label_clocking_prof_mem";
        this.toolTip.SetToolTip(this.label_clocking_prof_mem, resources.GetString("label_clocking_prof_mem.ToolTip"));
        // 
        // track_clocking_prof_core
        // 
        resources.ApplyResources(this.track_clocking_prof_core, "track_clocking_prof_core");
        this.track_clocking_prof_core.Maximum = 400;
        this.track_clocking_prof_core.Minimum = 200;
        this.track_clocking_prof_core.Name = "track_clocking_prof_core";
        this.track_clocking_prof_core.TickFrequency = 10;
        this.track_clocking_prof_core.TickStyle = System.Windows.Forms.TickStyle.None;
        this.track_clocking_prof_core.Value = 200;
        this.track_clocking_prof_core.Scroll += new System.EventHandler(this.track_clocking_prof_core_ValueChanged);
        // 
        // check_clocking_prof_core
        // 
        resources.ApplyResources(this.check_clocking_prof_core, "check_clocking_prof_core");
        this.check_clocking_prof_core.Name = "check_clocking_prof_core";
        this.toolTip.SetToolTip(this.check_clocking_prof_core, resources.GetString("check_clocking_prof_core.ToolTip"));
        this.check_clocking_prof_core.CheckedChanged += new System.EventHandler(this.check_clocking_prof_core_CheckedChanged);
        // 
        // button_clocking_disable
        // 
        resources.ApplyResources(this.button_clocking_disable, "button_clocking_disable");
        this.button_clocking_disable.Name = "button_clocking_disable";
        this.toolTip.SetToolTip(this.button_clocking_disable, resources.GetString("button_clocking_disable.ToolTip"));
        this.button_clocking_disable.Click += new System.EventHandler(this.button_clocking_disable_Click);
        // 
        // label_clocking_prof_core
        // 
        resources.ApplyResources(this.label_clocking_prof_core, "label_clocking_prof_core");
        this.label_clocking_prof_core.Name = "label_clocking_prof_core";
        this.toolTip.SetToolTip(this.label_clocking_prof_core, resources.GetString("label_clocking_prof_core.ToolTip"));
        // 
        // label5
        // 
        resources.ApplyResources(this.label5, "label5");
        this.label5.Name = "label5";
        this.toolTip.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
        // 
        // combo_clocking_prof_presets
        // 
        resources.ApplyResources(this.combo_clocking_prof_presets, "combo_clocking_prof_presets");
        this.combo_clocking_prof_presets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_clocking_prof_presets.Items.AddRange(new object[] {
            resources.GetString("combo_clocking_prof_presets.Items"),
            resources.GetString("combo_clocking_prof_presets.Items1"),
            resources.GetString("combo_clocking_prof_presets.Items2"),
            resources.GetString("combo_clocking_prof_presets.Items3"),
            resources.GetString("combo_clocking_prof_presets.Items4")});
        this.combo_clocking_prof_presets.Name = "combo_clocking_prof_presets";
        this.combo_clocking_prof_presets.SelectedIndexChanged += new System.EventHandler(this.combo_clocking_prof_presets_SelectedIndexChanged);
        // 
        // tab_exp
        // 
        this.tab_exp.Controls.Add(this.panel1);
        this.tab_exp.Controls.Add(this.splitter_ind);
        this.tab_exp.Controls.Add(this.list_3d);
        resources.ApplyResources(this.tab_exp, "tab_exp");
        this.tab_exp.Name = "tab_exp";
        // 
        // panel1
        // 
        this.panel1.Controls.Add(this.group_ind_modeVal);
        this.panel1.Controls.Add(this.check_ind_ogl);
        this.panel1.Controls.Add(this.check_ind_d3d);
        resources.ApplyResources(this.panel1, "panel1");
        this.panel1.Name = "panel1";
        // 
        // group_ind_modeVal
        // 
        resources.ApplyResources(this.group_ind_modeVal, "group_ind_modeVal");
        this.group_ind_modeVal.Controls.Add(this.combo_ind_modeVal);
        this.group_ind_modeVal.Controls.Add(this.track_ind_modeVal);
        this.group_ind_modeVal.Controls.Add(this.panel_ind_modeVal);
        this.group_ind_modeVal.Controls.Add(this.picture_ind_d3d);
        this.group_ind_modeVal.Controls.Add(this.picture_ind_ogl);
        this.group_ind_modeVal.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.group_ind_modeVal.Name = "group_ind_modeVal";
        this.group_ind_modeVal.TabStop = false;
        // 
        // combo_ind_modeVal
        // 
        resources.ApplyResources(this.combo_ind_modeVal, "combo_ind_modeVal");
        this.combo_ind_modeVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.combo_ind_modeVal.ForeColor = System.Drawing.SystemColors.WindowText;
        this.combo_ind_modeVal.Name = "combo_ind_modeVal";
        this.toolTip.SetToolTip(this.combo_ind_modeVal, resources.GetString("combo_ind_modeVal.ToolTip"));
        this.combo_ind_modeVal.SelectedIndexChanged += new System.EventHandler(this.combo_ind_modeVal_SelectedIndexChanged);
        // 
        // track_ind_modeVal
        // 
        resources.ApplyResources(this.track_ind_modeVal, "track_ind_modeVal");
        this.track_ind_modeVal.LargeChange = 1;
        this.track_ind_modeVal.Name = "track_ind_modeVal";
        this.toolTip.SetToolTip(this.track_ind_modeVal, resources.GetString("track_ind_modeVal.ToolTip"));
        this.track_ind_modeVal.Scroll += new System.EventHandler(this.track_ind_modeVal_Scroll);
        // 
        // panel_ind_modeVal
        // 
        resources.ApplyResources(this.panel_ind_modeVal, "panel_ind_modeVal");
        this.panel_ind_modeVal.Name = "panel_ind_modeVal";
        this.panel_ind_modeVal.Resize += new System.EventHandler(this.panel_ind_modeVal_Resize);
        // 
        // picture_ind_d3d
        // 
        resources.ApplyResources(this.picture_ind_d3d, "picture_ind_d3d");
        this.picture_ind_d3d.Name = "picture_ind_d3d";
        this.picture_ind_d3d.TabStop = false;
        // 
        // picture_ind_ogl
        // 
        resources.ApplyResources(this.picture_ind_ogl, "picture_ind_ogl");
        this.picture_ind_ogl.Name = "picture_ind_ogl";
        this.picture_ind_ogl.TabStop = false;
        // 
        // check_ind_ogl
        // 
        resources.ApplyResources(this.check_ind_ogl, "check_ind_ogl");
        this.check_ind_ogl.Name = "check_ind_ogl";
        this.toolTip.SetToolTip(this.check_ind_ogl, resources.GetString("check_ind_ogl.ToolTip"));
        this.check_ind_ogl.CheckedChanged += new System.EventHandler(this.check_ind_ogl_CheckedChanged);
        // 
        // check_ind_d3d
        // 
        resources.ApplyResources(this.check_ind_d3d, "check_ind_d3d");
        this.check_ind_d3d.Name = "check_ind_d3d";
        this.toolTip.SetToolTip(this.check_ind_d3d, resources.GetString("check_ind_d3d.ToolTip"));
        this.check_ind_d3d.CheckedChanged += new System.EventHandler(this.check_ind_d3d_CheckedChanged);
        // 
        // splitter_ind
        // 
        this.splitter_ind.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        resources.ApplyResources(this.splitter_ind, "splitter_ind");
        this.splitter_ind.Name = "splitter_ind";
        this.splitter_ind.TabStop = false;
        // 
        // list_3d
        // 
        this.list_3d.AllowColumnReorder = true;
        this.list_3d.CheckBoxes = true;
        this.list_3d.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_name,
            this.columnHeader_profile,
            this.columnHeader_driver,
            this.columnHeader_help});
        resources.ApplyResources(this.list_3d, "list_3d");
        this.list_3d.FullRowSelect = true;
        this.list_3d.GridLines = true;
        this.list_3d.HideSelection = false;
        this.list_3d.MultiSelect = false;
        this.list_3d.Name = "list_3d";
        this.list_3d.UseCompatibleStateImageBehavior = false;
        this.list_3d.View = System.Windows.Forms.View.Details;
        this.list_3d.SelectedIndexChanged += new System.EventHandler(this.list_3d_SelectedIndexChanged);
        this.list_3d.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.list_3d_ItemCheck);
        this.list_3d.MouseDown += new System.Windows.Forms.MouseEventHandler(this.list_3d_MouseDown);
        // 
        // columnHeader_name
        // 
        resources.ApplyResources(this.columnHeader_name, "columnHeader_name");
        // 
        // columnHeader_profile
        // 
        resources.ApplyResources(this.columnHeader_profile, "columnHeader_profile");
        // 
        // columnHeader_driver
        // 
        resources.ApplyResources(this.columnHeader_driver, "columnHeader_driver");
        // 
        // columnHeader_help
        // 
        resources.ApplyResources(this.columnHeader_help, "columnHeader_help");
        // 
        // toolTip
        // 
        this.toolTip.ShowAlways = true;
        // 
        // button_prof_new
        // 
        resources.ApplyResources(this.button_prof_new, "button_prof_new");
        this.button_prof_new.Name = "button_prof_new";
        this.toolTip.SetToolTip(this.button_prof_new, resources.GetString("button_prof_new.ToolTip"));
        this.button_prof_new.Click += new System.EventHandler(this.button_prof_new_Click);
        // 
        // button_prof_clone
        // 
        resources.ApplyResources(this.button_prof_clone, "button_prof_clone");
        this.button_prof_clone.Name = "button_prof_clone";
        this.toolTip.SetToolTip(this.button_prof_clone, resources.GetString("button_prof_clone.ToolTip"));
        this.button_prof_clone.Click += new System.EventHandler(this.button_prof_clone_Click);
        // 
        // text_prof_name
        // 
        resources.ApplyResources(this.text_prof_name, "text_prof_name");
        this.text_prof_name.Name = "text_prof_name";
        this.toolTip.SetToolTip(this.text_prof_name, resources.GetString("text_prof_name.ToolTip"));
        this.text_prof_name.TextChanged += new System.EventHandler(this.text_prof_name_TextChanged);
        // 
        // button_prof_cancel
        // 
        resources.ApplyResources(this.button_prof_cancel, "button_prof_cancel");
        this.button_prof_cancel.Name = "button_prof_cancel";
        this.button_prof_cancel.Click += new System.EventHandler(this.button_prof_cancel_Click);
        // 
        // button_prof_ok
        // 
        resources.ApplyResources(this.button_prof_ok, "button_prof_ok");
        this.button_prof_ok.Name = "button_prof_ok";
        this.button_prof_ok.Click += new System.EventHandler(this.button_prof_ok_Click);
        // 
        // panel_prof_apply
        // 
        resources.ApplyResources(this.panel_prof_apply, "panel_prof_apply");
        this.panel_prof_apply.Controls.Add(this.button_prof_restore);
        this.panel_prof_apply.Controls.Add(this.button_prof_apply);
        this.panel_prof_apply.Controls.Add(this.button_prof_run_exe);
        this.panel_prof_apply.Controls.Add(this.button_prof_apply_and_run);
        this.panel_prof_apply.Name = "panel_prof_apply";
        // 
        // button_prof_restore
        // 
        resources.ApplyResources(this.button_prof_restore, "button_prof_restore");
        this.button_prof_restore.Name = "button_prof_restore";
        this.toolTip.SetToolTip(this.button_prof_restore, resources.GetString("button_prof_restore.ToolTip"));
        this.button_prof_restore.Click += new System.EventHandler(this.button_prof_restore_Click);
        // 
        // group_prof
        // 
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
        resources.ApplyResources(this.group_prof, "group_prof");
        this.group_prof.Name = "group_prof";
        // 
        // button_prof_discard
        // 
        resources.ApplyResources(this.button_prof_discard, "button_prof_discard");
        this.button_prof_discard.Name = "button_prof_discard";
        this.toolTip.SetToolTip(this.button_prof_discard, resources.GetString("button_prof_discard.ToolTip"));
        this.button_prof_discard.Click += new System.EventHandler(this.button_prof_discard_Click);
        // 
        // button_prof_rename
        // 
        resources.ApplyResources(this.button_prof_rename, "button_prof_rename");
        this.button_prof_rename.Name = "button_prof_rename";
        this.toolTip.SetToolTip(this.button_prof_rename, resources.GetString("button_prof_rename.ToolTip"));
        this.button_prof_rename.Click += new System.EventHandler(this.button_prof_rename_Click);
        // 
        // toolBar1
        // 
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
        resources.ApplyResources(this.toolBar1, "toolBar1");
        this.toolBar1.ImageList = this.imageList;
        this.toolBar1.Name = "toolBar1";
        this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
        // 
        // toolButton_exploreGameFolder
        // 
        resources.ApplyResources(this.toolButton_exploreGameFolder, "toolButton_exploreGameFolder");
        this.toolButton_exploreGameFolder.Name = "toolButton_exploreGameFolder";
        // 
        // toolButton_editGameCfg
        // 
        resources.ApplyResources(this.toolButton_editGameCfg, "toolButton_editGameCfg");
        this.toolButton_editGameCfg.Name = "toolButton_editGameCfg";
        // 
        // toolBarButton1
        // 
        this.toolBarButton1.Name = "toolBarButton1";
        this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
        // 
        // toolButton_tools_regEdit
        // 
        resources.ApplyResources(this.toolButton_tools_regEdit, "toolButton_tools_regEdit");
        this.toolButton_tools_regEdit.Name = "toolButton_tools_regEdit";
        // 
        // toolBarButton4
        // 
        this.toolBarButton4.Name = "toolBarButton4";
        this.toolBarButton4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
        // 
        // toolButton_prof_commands
        // 
        resources.ApplyResources(this.toolButton_prof_commands, "toolButton_prof_commands");
        this.toolButton_prof_commands.Name = "toolButton_prof_commands";
        // 
        // toolButton_hotkeys
        // 
        resources.ApplyResources(this.toolButton_hotkeys, "toolButton_hotkeys");
        this.toolButton_hotkeys.Name = "toolButton_hotkeys";
        // 
        // toolBarButton2
        // 
        this.toolBarButton2.Name = "toolBarButton2";
        this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
        // 
        // toolButton_settings
        // 
        resources.ApplyResources(this.toolButton_settings, "toolButton_settings");
        this.toolButton_settings.Name = "toolButton_settings";
        // 
        // toolBarButton3
        // 
        this.toolBarButton3.Name = "toolBarButton3";
        this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
        // 
        // toolButton_help_onlineManual
        // 
        resources.ApplyResources(this.toolButton_help_onlineManual, "toolButton_help_onlineManual");
        this.toolButton_help_onlineManual.Name = "toolButton_help_onlineManual";
        // 
        // toolBarButton5
        // 
        this.toolBarButton5.Name = "toolBarButton5";
        this.toolBarButton5.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
        // 
        // toolButton_compact
        // 
        resources.ApplyResources(this.toolButton_compact, "toolButton_compact");
        this.toolButton_compact.Name = "toolButton_compact";
        this.toolButton_compact.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
        // 
        // imageList
        // 
        this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
        this.imageList.TransparentColor = System.Drawing.Color.Transparent;
        this.imageList.Images.SetKeyName(0, "");
        this.imageList.Images.SetKeyName(1, "");
        this.imageList.Images.SetKeyName(2, "");
        this.imageList.Images.SetKeyName(3, "");
        this.imageList.Images.SetKeyName(4, "");
        this.imageList.Images.SetKeyName(5, "");
        this.imageList.Images.SetKeyName(6, "");
        this.imageList.Images.SetKeyName(7, "");
        this.imageList.Images.SetKeyName(8, "");
        this.imageList.Images.SetKeyName(9, "");
        this.imageList.Images.SetKeyName(10, "");
        this.imageList.Images.SetKeyName(11, "");
        this.imageList.Images.SetKeyName(12, "");
        this.imageList.Images.SetKeyName(13, "");
        this.imageList.Images.SetKeyName(14, "");
        this.imageList.Images.SetKeyName(15, "");
        this.imageList.Images.SetKeyName(16, "");
        this.imageList.Images.SetKeyName(17, "");
        this.imageList.Images.SetKeyName(18, "");
        this.imageList.Images.SetKeyName(19, "");
        this.imageList.Images.SetKeyName(20, "");
        this.imageList.Images.SetKeyName(21, "");
        this.imageList.Images.SetKeyName(22, "");
        this.imageList.Images.SetKeyName(23, "");
        this.imageList.Images.SetKeyName(24, "");
        this.imageList.Images.SetKeyName(25, "");
        this.imageList.Images.SetKeyName(26, "");
        this.imageList.Images.SetKeyName(27, "");
        this.imageList.Images.SetKeyName(28, "");
        // 
        // timer_updateCr
        // 
        this.timer_updateCr.Interval = 750;
        this.timer_updateCr.Tick += new System.EventHandler(this.timer_updateCr_Tick);
        // 
        // notifyIcon_tray
        // 
        this.notifyIcon_tray.ContextMenu = this.context_tray;
        resources.ApplyResources(this.notifyIcon_tray, "notifyIcon_tray");
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
        this.context_tray.Popup += new System.EventHandler(this.context_tray_Popup);
        // 
        // menu_tray_profs
        // 
        this.menu_tray_profs.Index = 0;
        this.menuImage.SetMenuImage(this.menu_tray_profs, "0");
        this.menu_tray_profs.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_tray_apply_exe,
            this.menu_tray_apply,
            this.menu_tray_profs_editGameIni,
            this.menuItem7,
            this.menu_tray_profs_makeLink});
        this.menu_tray_profs.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_profs, "menu_tray_profs");
        // 
        // menu_tray_apply_exe
        // 
        this.menu_tray_apply_exe.Index = 0;
        this.menuImage.SetMenuImage(this.menu_tray_apply_exe, "0");
        this.menu_tray_apply_exe.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_apply_exe, "menu_tray_apply_exe");
        // 
        // menu_tray_apply
        // 
        this.menu_tray_apply.Index = 1;
        this.menuImage.SetMenuImage(this.menu_tray_apply, "0");
        this.menu_tray_apply.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_tray_applyExe});
        this.menu_tray_apply.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_apply, "menu_tray_apply");
        // 
        // menu_tray_applyExe
        // 
        this.menu_tray_applyExe.Index = 0;
        this.menuImage.SetMenuImage(this.menu_tray_applyExe, "0");
        this.menu_tray_applyExe.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_applyExe, "menu_tray_applyExe");
        // 
        // menu_tray_profs_editGameIni
        // 
        this.menu_tray_profs_editGameIni.Index = 2;
        this.menuImage.SetMenuImage(this.menu_tray_profs_editGameIni, "0");
        this.menu_tray_profs_editGameIni.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_profs_editGameIni, "menu_tray_profs_editGameIni");
        this.menu_tray_profs_editGameIni.Popup += new System.EventHandler(this.menu_tray_profs_editGameIni_Popup);
        // 
        // menuItem7
        // 
        this.menuItem7.Index = 3;
        this.menuImage.SetMenuImage(this.menuItem7, "0");
        this.menuItem7.OwnerDraw = true;
        resources.ApplyResources(this.menuItem7, "menuItem7");
        // 
        // menu_tray_profs_makeLink
        // 
        this.menu_tray_profs_makeLink.Index = 4;
        this.menuImage.SetMenuImage(this.menu_tray_profs_makeLink, "0");
        this.menu_tray_profs_makeLink.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_profs_makeLink, "menu_tray_profs_makeLink");
        // 
        // menuItem26
        // 
        this.menuItem26.Index = 1;
        this.menuImage.SetMenuImage(this.menuItem26, null);
        this.menuItem26.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_tray_clocking_pre_slow,
            this.menu_tray_clocking_pre_normal,
            this.menu_tray_clocking_pre_fast,
            this.menu_tray_clocking_pre_ultra});
        this.menuItem26.OwnerDraw = true;
        resources.ApplyResources(this.menuItem26, "menuItem26");
        // 
        // menu_tray_clocking_pre_slow
        // 
        this.menu_tray_clocking_pre_slow.Index = 0;
        this.menuImage.SetMenuImage(this.menu_tray_clocking_pre_slow, null);
        this.menu_tray_clocking_pre_slow.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_clocking_pre_slow, "menu_tray_clocking_pre_slow");
        this.menu_tray_clocking_pre_slow.Click += new System.EventHandler(this.menu_tray_clocking_pre_slow_Click);
        // 
        // menu_tray_clocking_pre_normal
        // 
        this.menu_tray_clocking_pre_normal.Index = 1;
        this.menuImage.SetMenuImage(this.menu_tray_clocking_pre_normal, null);
        this.menu_tray_clocking_pre_normal.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_clocking_pre_normal, "menu_tray_clocking_pre_normal");
        this.menu_tray_clocking_pre_normal.Click += new System.EventHandler(this.menu_tray_clocking_pre_normal_Click);
        // 
        // menu_tray_clocking_pre_fast
        // 
        this.menu_tray_clocking_pre_fast.Index = 2;
        this.menuImage.SetMenuImage(this.menu_tray_clocking_pre_fast, null);
        this.menu_tray_clocking_pre_fast.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_clocking_pre_fast, "menu_tray_clocking_pre_fast");
        this.menu_tray_clocking_pre_fast.Click += new System.EventHandler(this.menu_tray_clocking_pre_fast_Click);
        // 
        // menu_tray_clocking_pre_ultra
        // 
        this.menu_tray_clocking_pre_ultra.Index = 3;
        this.menuImage.SetMenuImage(this.menu_tray_clocking_pre_ultra, null);
        this.menu_tray_clocking_pre_ultra.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_clocking_pre_ultra, "menu_tray_clocking_pre_ultra");
        this.menu_tray_clocking_pre_ultra.Click += new System.EventHandler(this.menu_tray_clocking_pre_ultra_Click);
        // 
        // menuItem11
        // 
        this.menuItem11.Index = 2;
        this.menuImage.SetMenuImage(this.menuItem11, "0");
        this.menuItem11.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_tray_img_mountCurr,
            this.menu_tray_img_mountAnImgAtD0});
        this.menuItem11.OwnerDraw = true;
        resources.ApplyResources(this.menuItem11, "menuItem11");
        // 
        // menu_tray_img_mountCurr
        // 
        this.menu_tray_img_mountCurr.Index = 0;
        this.menuImage.SetMenuImage(this.menu_tray_img_mountCurr, "0");
        this.menu_tray_img_mountCurr.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_img_mountCurr, "menu_tray_img_mountCurr");
        this.menu_tray_img_mountCurr.Click += new System.EventHandler(this.menu_tray_img_mountCurr_Click);
        // 
        // menu_tray_img_mountAnImgAtD0
        // 
        this.menu_tray_img_mountAnImgAtD0.Index = 1;
        this.menuImage.SetMenuImage(this.menu_tray_img_mountAnImgAtD0, "0");
        this.menu_tray_img_mountAnImgAtD0.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_img_mountAnImgAtD0, "menu_tray_img_mountAnImgAtD0");
        // 
        // menuItem25
        // 
        this.menuItem25.Index = 3;
        this.menuImage.SetMenuImage(this.menuItem25, null);
        this.menuItem25.OwnerDraw = true;
        resources.ApplyResources(this.menuItem25, "menuItem25");
        // 
        // menu_tray_tools
        // 
        this.menu_tray_tools.Index = 4;
        this.menuImage.SetMenuImage(this.menu_tray_tools, "0");
        this.menu_tray_tools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_tray_tools_regEdit,
            this.menu_tray_tools_regDiff});
        this.menu_tray_tools.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_tools, "menu_tray_tools");
        // 
        // menu_tray_tools_regEdit
        // 
        this.menu_tray_tools_regEdit.Index = 0;
        this.menuImage.SetMenuImage(this.menu_tray_tools_regEdit, "0");
        this.menu_tray_tools_regEdit.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_tools_regEdit, "menu_tray_tools_regEdit");
        this.menu_tray_tools_regEdit.Click += new System.EventHandler(this.menu_tools_openRegedit_Click);
        // 
        // menu_tray_tools_regDiff
        // 
        this.menu_tray_tools_regDiff.Index = 1;
        this.menuImage.SetMenuImage(this.menu_tray_tools_regDiff, "0");
        this.menu_tray_tools_regDiff.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_tools_regDiff, "menu_tray_tools_regDiff");
        this.menu_tray_tools_regDiff.Click += new System.EventHandler(this.menu_exp_testRdKey_Click);
        // 
        // menuItem23
        // 
        this.menuItem23.Index = 5;
        this.menuImage.SetMenuImage(this.menuItem23, null);
        this.menuItem23.OwnerDraw = true;
        resources.ApplyResources(this.menuItem23, "menuItem23");
        // 
        // menu_tray_stayInTray
        // 
        this.menu_tray_stayInTray.Index = 6;
        this.menuImage.SetMenuImage(this.menu_tray_stayInTray, null);
        this.menu_tray_stayInTray.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_stayInTray, "menu_tray_stayInTray");
        this.menu_tray_stayInTray.Click += new System.EventHandler(this.menu_tray_stayInTray_Click);
        // 
        // menu_tray_hideOnCloseBox
        // 
        this.menu_tray_hideOnCloseBox.Index = 7;
        this.menuImage.SetMenuImage(this.menu_tray_hideOnCloseBox, null);
        this.menu_tray_hideOnCloseBox.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_hideOnCloseBox, "menu_tray_hideOnCloseBox");
        this.menu_tray_hideOnCloseBox.Click += new System.EventHandler(this.menu_tray_hideOnCloseBox_Click);
        // 
        // menu_tray_sep
        // 
        this.menu_tray_sep.Index = 8;
        this.menuImage.SetMenuImage(this.menu_tray_sep, "0");
        this.menu_tray_sep.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_sep, "menu_tray_sep");
        // 
        // menu_tray_quit
        // 
        this.menu_tray_quit.Index = 9;
        this.menuImage.SetMenuImage(this.menu_tray_quit, "0");
        this.menu_tray_quit.OwnerDraw = true;
        resources.ApplyResources(this.menu_tray_quit, "menu_tray_quit");
        this.menu_tray_quit.Click += new System.EventHandler(this.menu_file_quit_Click);
        // 
        // menuImage
        // 
        this.menuImage.ImageList = this.imageList;
        // 
        // FormMain
        // 
        resources.ApplyResources(this, "$this");
        this.Controls.Add(this.group_prof);
        this.Controls.Add(this.tabCtrl);
        this.Controls.Add(this.toolBar1);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
        this.KeyPreview = true;
        this.MaximizeBox = false;
        this.Menu = this.mainMenu;
        this.Name = "FormMain";
        this.Load += new System.EventHandler(this.FormMain_Load);
        this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
        this.Closed += new System.EventHandler(this.FormMain_Closed);
        this.Activated += new System.EventHandler(this.FormMain_Activated);
        this.Closing += new System.ComponentModel.CancelEventHandler(this.FormMain_Closing);
        this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyUp);
        this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
        this.group_main_d3d.ResumeLayout(false);
        this.group_extra_d3d.ResumeLayout(false);
        this.group_extra_ogl.ResumeLayout(false);
        this.tabCtrl.ResumeLayout(false);
        this.tab_main.ResumeLayout(false);
        this.tab_files.ResumeLayout(false);
        this.panel_prof_files.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.num_prof_imgDrive)).EndInit();
        this.panel_gameExe.ResumeLayout(false);
        this.panel_gameExe.PerformLayout();
        this.tab_extra_d3d.ResumeLayout(false);
        this.group_extra_d3d_2.ResumeLayout(false);
        this.tab_extra_ogl.ResumeLayout(false);
        this.group_extra_ogl_2.ResumeLayout(false);
        this.tab_summary.ResumeLayout(false);
        this.tab_clocking.ResumeLayout(false);
        this.group_clocking_curr.ResumeLayout(false);
        this.group_clocking_curr.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.track_clocking_curr_mem)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.track_clocking_curr_core)).EndInit();
        this.group_clocking_current_presets.ResumeLayout(false);
        this.group_clocking_prof.ResumeLayout(false);
        this.panel_clocking_prof_clocks.ResumeLayout(false);
        this.panel_clocking_prof_clocks.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.track_clocking_prof_mem)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.track_clocking_prof_core)).EndInit();
        this.tab_exp.ResumeLayout(false);
        this.panel1.ResumeLayout(false);
        this.group_ind_modeVal.ResumeLayout(false);
        this.group_ind_modeVal.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.track_ind_modeVal)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.picture_ind_d3d)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.picture_ind_ogl)).EndInit();
        this.panel_prof_apply.ResumeLayout(false);
        this.group_prof.ResumeLayout(false);
        this.group_prof.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();

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
    private System.Windows.Forms.TabPage tab_extra_d3d;
    private System.Windows.Forms.TabPage tab_extra_ogl;
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
              combo_prof_img.Items.Add(img);
              if (drv_nmb < 3)
                ++drv_nmb;
            }
          }
          combo_prof_img.SelectedIndex = 0;
        } else if (!replace && combo_prof_img.Items.Count > 0) {
          if (combo_prof_img.Items.Count == 1) {
            combo_prof_img.Items[0] = combo_prof_img.Items[0];
            combo_prof_img.DropDownStyle = ComboBoxStyle.DropDown;
          }
          int drv_nmb = 0;
          foreach(string img in ofd.FileNames) {
            if (combo_prof_img.Items.Count >= 4) //TODO:literal max_images
              break;
            combo_prof_img.Items.Add(img);
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

    private void button_clocking_disable_Click(object sender, System.EventArgs e) {
      check_clocking_prof_core.Checked ^= true;
      check_clocking_prof_mem.Checked  ^= true;   
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
      //XXX// update_from_gpd(sel_gpd);
    }

    private void combos_prof_modes_SelectedIndexChanged(object sender, System.EventArgs e) {
      //XXX// if (!((Control)sender).Focused) return;
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
        if (!ax.cr.modeval_name_or_alias_exists(
          G.cr_modes[idx],
          combos_prof_modes[idx].SelectedIndex,
          sel_gpd.val(G.gp_parms[idx]).Data)) {

          string modeval_name = ax.cr[G.cr_modes[idx]].get_name_by_index(combos_prof_modes[idx].SelectedIndex);
          sel_gpd.val(G.gp_parms[idx]).Data = modeval_name;
        }
      }
      combos_mouse_leaves(sender, e); //TODO: workaround. See comment of combos_mouse_leaves()

      //XXX// update_from_gpd(sel_gpd);
    }

    private void combos_SelectedIndexChanged(object sender, System.EventArgs e) {
      //XXX// if (!((Control)sender).Focused) return;
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
      ax.cr.enable_modeval_by_index(ConfigRecord.EMode.D3D_QE, combo_d3d_qe_mode.SelectedIndex);		
    }

    private void combo_ogl_qe_mode_SelectedIndexChanged(object sender, System.EventArgs e) {
      ax.cr.enable_modeval_by_index(ConfigRecord.EMode.OGL_QE, combo_ogl_qe_mode.SelectedIndex);		
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

      int drv_nmb = sel_gpd.img_drive_number[idx];
      num_prof_imgDrive.Value = drv_nmb;
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
        = menu_prof_img_file_replace.Enabled
        = menu_prof_img_file_replaceAll.Enabled
        = menu_prof_img_file_remove.Enabled
        = menu_prof_img_file_removeAll.Enabled
        = txt != "";

      if (sel_gpd == null)
        return;

      if (combo_prof_img.Items.Count > 1 && combo_prof_img.SelectedIndex >= 0) {
        string[] sa = {"", "", "", ""};
        for (int i=0; i < sa.Length && i < combo_prof_img.Items.Count; ++i) {
          sa[i] = combo_prof_img.Items[i].ToString();
        }
        sel_gpd.img_path = sa;
      } else {
        sel_gpd.img_path = new string[] { txt }; //TODO:image
      }
      
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
      System.Windows.Forms.MessageBox.Show(G.loc.msgbox_txt_help_tdprofgd.Replace("TDProfGD", tdprofGD_basename(sel_gpd, false)),
        G.loc.msgbox_title_help_tdprofgd.Replace("TDProfGD", tdprofGD_basename(sel_gpd, false)));
    }



    string tdprofGD_basename(GameProfileData gpd, bool add_path) {
        string exe_path = gpd.exe_path;
        if (exe_path == "")
          return null;

      string dir_name = System.IO.Path.GetDirectoryName(exe_path);
      string base_name = System.IO.Path.GetFileNameWithoutExtension(exe_path);

      return (add_path ? dir_name + @"\" + base_name : base_name) + ".3DProf";
    }

    private void menu_prof_tdprofGD_enable(GameProfileData gpd) {
      string lnch_path = null;

      if (gpd != null && gpd.exe_path == "") {
        menu_prof_tdprofGD.Enabled = false;
        return;
      }

      menu_prof_tdprofGD.Enabled = true;

      if (gpd != null && lnch_path == null) {
        lnch_path = tdprofGD_basename(gpd, true) + ".exe";
      }

      bool file_exists = File.Exists(lnch_path);
      bool dir_exists =  Directory.Exists(System.IO.Path.GetDirectoryName(lnch_path));
      menu_prof_tdprofGD_remove.Enabled = file_exists;
      menu_prof_tdprofGD_create.Enabled = !file_exists && dir_exists;
    }

    private void menu_prof_tdprofGD_remove_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null) 
        return;
      if (sel_gpd.exe_path == "")
        return;
      
      File.Delete(tdprofGD_basename(sel_gpd, true) + ".exe");
      File.Delete(tdprofGD_basename(sel_gpd, true) + ".ini");
    }

    private void menu_prof_tdprofGD_create_Click(object sender, System.EventArgs e) {
      if (sel_gpd == null) 
        return;
      if (sel_gpd.exe_path == "")
        return;
      string exe_path = sel_gpd.exe_path;
      string exe_dir = Utils.extract_directory(exe_path);
      string wdir = Directory.GetCurrentDirectory();
      string lnch_path = tdprofGD_basename(sel_gpd, true) + ".exe";
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
      StreamWriter sw = new StreamWriter(tdprofGD_basename(sel_gpd, true) + ".ini");
      sw.WriteLine("tdprof_dir=" + wdir);
      sw.WriteLine("prof_name=\"" + sel_gpd.name + "\"");
      sw.Close();
    }

    private void menu_prof_tdprofGD_Popup(object sender, System.EventArgs e) {
      if (sel_gpd == null)
        return;

      if (menu_prof_tdprofGD_create_Text == null) {
        menu_prof_tdprofGD_create_Text = menu_prof_tdprofGD_create.Text;
        menu_prof_tdprofGD_remove_Text = menu_prof_tdprofGD_remove.Text;
      }
      menu_prof_tdprofGD_create.Text = menu_prof_tdprofGD_create_Text.Replace("TDProfGD", tdprofGD_basename(sel_gpd, false));
      menu_prof_tdprofGD_remove.Text = menu_prof_tdprofGD_remove_Text.Replace("TDProfGD", tdprofGD_basename(sel_gpd, false));

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
      }
      if (sel_gpd != null) {
        menu_prof_setSpecName.Checked = (sel_gpd.spec_name == G.spec_name);
        menu_prof_tdprofGD_enable(sel_gpd);
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

    private void menu_help_log_Click(object sender, System.EventArgs e) {
      try {
        Process.Start("app.log");
      } catch {}
    }


        #endregion
        #region Experimental
    private void menu_exp_testRdKey_Click(object sender, System.EventArgs e) {
      RegDiffForm rdf = new RegDiffForm(Microsoft.Win32.Registry.LocalMachine, G.di.get_driver_key(), 500);
      rdf.Show();
    }

    private void menu_exp_test1_Click(object sender, System.EventArgs e) {
      group_clocking_prof.Width = group_clocking_curr.Width; return;
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

    void guifb_clocking_prof_set_clocking_kind(GameProfileData.EClockKinds kind) {
      panel_clocking_prof_clocks.Enabled = kind == GameProfileData.EClockKinds.PARENT;
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

    private void menu_file_debugThrow_Click(object sender, System.EventArgs e) {
       string s = null;
       s.ToLower();
    }

    private void tabCtrl_SelectedIndexChanged(object sender, System.EventArgs e) {
      update_from_gpd(sel_gpd);
    }


    private void num_prof_imgDrive_ValueChanged(object sender, System.EventArgs e) {
      int drive_number = (int)num_prof_imgDrive.Value;
      int img_number = combo_prof_img.SelectedIndex;
      GameProfileData gpd = sel_gpd;

      if (gpd == null || img_number < 0)
        return;
      
      int[] tem = gpd.img_drive_number;
      tem[img_number] = drive_number;

      gpd.img_drive_number = tem;
    }

    private void track_ind_modeVal_Scroll(object sender, System.EventArgs e) {
      //XXX// if (!((Control)sender).Focused) return;

      int idx = (int)track_ind_modeVal.Value;
      if (combo_ind_modeVal.Items.Count > idx)
        combo_ind_modeVal.SelectedIndex = idx;
    }

    private void list_3d_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
      int idx = track_ind_modeVal.Value;
      int count = combo_ind_modeVal.Items.Count;

      if (e.Button == MouseButtons.Right || e.Button == MouseButtons.XButton2) {
        combo_ind_modeVal.SelectedIndex = (idx + 1) % count;
      } else if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.XButton1) {
        combo_ind_modeVal.SelectedIndex = (idx + count - 1) % count;
      }
    }

    private void splitter_clocking_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e) {
      int ratio = (group_clocking_prof.Width * 100) / (group_clocking_prof.Width + group_clocking_curr.Width);
      ax.ac.gui_clock_tab_split_ratio = ratio;
      ax.ac.save_config();
    }

    private void panel_ind_modeVal_Resize(object sender, System.EventArgs e) {
      resize_tab_exp();
    }

 







  }

}
