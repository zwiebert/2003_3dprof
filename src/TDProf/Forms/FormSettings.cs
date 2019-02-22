using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using TDProf.App;

namespace TDProf.GUI {

  /// <summary>
  /// Summary description for Form2.
  /// </summary>
  public class Form_Settings : System.Windows.Forms.Form {
    AppConfig ac;
    FormMain form_root;
    AppContext ax;
    TabPage m_initialTabPage;

    #region *** Widgets ***
    private System.Windows.Forms.TextBox text_daemon_exe_path;
    private System.Windows.Forms.Button button__choose_daemon_exe_path;
    private System.Windows.Forms.Label label_daemon_exe_path;
    private System.Windows.Forms.Button button_ok;
    private System.Windows.Forms.Button button_cancel;
    private System.ComponentModel.IContainer components;
    private System.Windows.Forms.CheckBox check_gui_mover_feedback;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.OpenFileDialog openFile;
    private System.Windows.Forms.TabControl tab;
    private System.Windows.Forms.TabPage tab_sl;
    private System.Windows.Forms.TabPage tab_main;
    private System.Windows.Forms.Label label_sl_prefix;
    private System.Windows.Forms.TextBox text_SlNamePrefix;
    private System.Windows.Forms.TextBox text_SlNameSuffix;
    private System.Windows.Forms.CheckBox check_sl_auto_restore;
    private System.Windows.Forms.Label label_sl_suffix;
    private System.Windows.Forms.CheckBox check_applyDefaultFirst;
    private System.Windows.Forms.CheckBox check_restoreToDefault;
    private System.Windows.Forms.CheckBox check_auto_restore;
    private System.Windows.Forms.TextBox text_update_timer;
    private System.Windows.Forms.Label label_update_timer;
    private System.Windows.Forms.CheckBox check_update_timer;
    private System.Windows.Forms.ComboBox combo_defaultProfile;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox combo_qualityProfile;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox combo_performanceProfile;
    private System.Windows.Forms.CheckBox check_features_enableClocking;
    private System.Windows.Forms.TabPage tab_restore;
    private System.Windows.Forms.GroupBox group_autorestore;
    private System.Windows.Forms.TabPage tab_clocking;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.CheckBox check_gui_show_tray_icon;
    private System.Windows.Forms.CheckBox check_clock_atiSmallSteps;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.TabPage tab_extended;
    private System.Windows.Forms.Label label_SlPreview;
    private System.Windows.Forms.NumericUpDown num_clock_chip_min;
    private System.Windows.Forms.NumericUpDown num_clock_chip_pre_slow;
    private System.Windows.Forms.NumericUpDown num_clock_chip_pre_normal;
    private System.Windows.Forms.NumericUpDown num_clock_chip_pre_fast;
    private System.Windows.Forms.NumericUpDown num_clock_chip_pre_ultra;
    private System.Windows.Forms.NumericUpDown num_clock_mem_pre_fast;
    private System.Windows.Forms.NumericUpDown num_clock_mem_pre_normal;
    private System.Windows.Forms.NumericUpDown num_clock_mem_pre_slow;
    private System.Windows.Forms.NumericUpDown num_clock_mem_min;
    private System.Windows.Forms.NumericUpDown num_clock_chip_max;
    private System.Windows.Forms.NumericUpDown num_clock_mem_max;
    private System.Windows.Forms.NumericUpDown num_clock_mem_pre_ultra;
    private System.Windows.Forms.CheckBox check_clock_enableDLL;
    private System.Windows.Forms.ComboBox combo_clocking_prof_restorePresets;
    #endregion

    public enum TabEn { Main, Clocking };
    /// <summary>
    /// user options
    /// </summary>
    public Form_Settings(FormMain parent, AppContext  appContext,  TabEn starting_tab) {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //
      // TODO: Add any constructor code after InitializeComponent call
      //
      form_root = parent;
      ax = appContext;
      ac = ax.ac;

      
      try {
        //TODO: Security exceptions can occur on Win9x
        this.openFile.RestoreDirectory = true;
      } catch {}

      switch(starting_tab) {
        case TabEn.Clocking: m_initialTabPage = tab_clocking; break;
        default: m_initialTabPage = tab_main; break;
      }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if(components != null) {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

		#region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form_Settings));
      this.text_daemon_exe_path = new System.Windows.Forms.TextBox();
      this.button__choose_daemon_exe_path = new System.Windows.Forms.Button();
      this.label_daemon_exe_path = new System.Windows.Forms.Label();
      this.button_ok = new System.Windows.Forms.Button();
      this.button_cancel = new System.Windows.Forms.Button();
      this.check_gui_mover_feedback = new System.Windows.Forms.CheckBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.text_SlNamePrefix = new System.Windows.Forms.TextBox();
      this.text_SlNameSuffix = new System.Windows.Forms.TextBox();
      this.check_sl_auto_restore = new System.Windows.Forms.CheckBox();
      this.check_auto_restore = new System.Windows.Forms.CheckBox();
      this.check_update_timer = new System.Windows.Forms.CheckBox();
      this.check_restoreToDefault = new System.Windows.Forms.CheckBox();
      this.check_features_enableClocking = new System.Windows.Forms.CheckBox();
      this.tab = new System.Windows.Forms.TabControl();
      this.tab_main = new System.Windows.Forms.TabPage();
      this.check_gui_show_tray_icon = new System.Windows.Forms.CheckBox();
      this.label_update_timer = new System.Windows.Forms.Label();
      this.text_update_timer = new System.Windows.Forms.TextBox();
      this.tab_restore = new System.Windows.Forms.TabPage();
      this.group_autorestore = new System.Windows.Forms.GroupBox();
      this.combo_defaultProfile = new System.Windows.Forms.ComboBox();
      this.tab_sl = new System.Windows.Forms.TabPage();
      this.label_SlPreview = new System.Windows.Forms.Label();
      this.label_sl_prefix = new System.Windows.Forms.Label();
      this.label_sl_suffix = new System.Windows.Forms.Label();
      this.tab_clocking = new System.Windows.Forms.TabPage();
      this.label14 = new System.Windows.Forms.Label();
      this.combo_clocking_prof_restorePresets = new System.Windows.Forms.ComboBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.num_clock_mem_max = new System.Windows.Forms.NumericUpDown();
      this.num_clock_mem_pre_ultra = new System.Windows.Forms.NumericUpDown();
      this.num_clock_mem_pre_fast = new System.Windows.Forms.NumericUpDown();
      this.num_clock_mem_pre_normal = new System.Windows.Forms.NumericUpDown();
      this.num_clock_mem_pre_slow = new System.Windows.Forms.NumericUpDown();
      this.num_clock_mem_min = new System.Windows.Forms.NumericUpDown();
      this.num_clock_chip_max = new System.Windows.Forms.NumericUpDown();
      this.num_clock_chip_pre_ultra = new System.Windows.Forms.NumericUpDown();
      this.num_clock_chip_pre_fast = new System.Windows.Forms.NumericUpDown();
      this.num_clock_chip_pre_normal = new System.Windows.Forms.NumericUpDown();
      this.num_clock_chip_pre_slow = new System.Windows.Forms.NumericUpDown();
      this.label10 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.label13 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.num_clock_chip_min = new System.Windows.Forms.NumericUpDown();
      this.tab_extended = new System.Windows.Forms.TabPage();
      this.check_clock_enableDLL = new System.Windows.Forms.CheckBox();
      this.check_clock_atiSmallSteps = new System.Windows.Forms.CheckBox();
      this.label3 = new System.Windows.Forms.Label();
      this.combo_performanceProfile = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.combo_qualityProfile = new System.Windows.Forms.ComboBox();
      this.check_applyDefaultFirst = new System.Windows.Forms.CheckBox();
      this.openFile = new System.Windows.Forms.OpenFileDialog();
      this.tab.SuspendLayout();
      this.tab_main.SuspendLayout();
      this.tab_restore.SuspendLayout();
      this.group_autorestore.SuspendLayout();
      this.tab_sl.SuspendLayout();
      this.tab_clocking.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_max)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_pre_ultra)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_pre_fast)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_pre_normal)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_pre_slow)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_min)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_max)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_pre_ultra)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_pre_fast)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_pre_normal)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_pre_slow)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_min)).BeginInit();
      this.tab_extended.SuspendLayout();
      this.SuspendLayout();
      // 
      // text_daemon_exe_path
      // 
      this.text_daemon_exe_path.AccessibleDescription = resources.GetString("text_daemon_exe_path.AccessibleDescription");
      this.text_daemon_exe_path.AccessibleName = resources.GetString("text_daemon_exe_path.AccessibleName");
      this.text_daemon_exe_path.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_daemon_exe_path.Anchor")));
      this.text_daemon_exe_path.AutoSize = ((bool)(resources.GetObject("text_daemon_exe_path.AutoSize")));
      this.text_daemon_exe_path.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_daemon_exe_path.BackgroundImage")));
      this.text_daemon_exe_path.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_daemon_exe_path.Dock")));
      this.text_daemon_exe_path.Enabled = ((bool)(resources.GetObject("text_daemon_exe_path.Enabled")));
      this.text_daemon_exe_path.Font = ((System.Drawing.Font)(resources.GetObject("text_daemon_exe_path.Font")));
      this.text_daemon_exe_path.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_daemon_exe_path.ImeMode")));
      this.text_daemon_exe_path.Location = ((System.Drawing.Point)(resources.GetObject("text_daemon_exe_path.Location")));
      this.text_daemon_exe_path.MaxLength = ((int)(resources.GetObject("text_daemon_exe_path.MaxLength")));
      this.text_daemon_exe_path.Multiline = ((bool)(resources.GetObject("text_daemon_exe_path.Multiline")));
      this.text_daemon_exe_path.Name = "text_daemon_exe_path";
      this.text_daemon_exe_path.PasswordChar = ((char)(resources.GetObject("text_daemon_exe_path.PasswordChar")));
      this.text_daemon_exe_path.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_daemon_exe_path.RightToLeft")));
      this.text_daemon_exe_path.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_daemon_exe_path.ScrollBars")));
      this.text_daemon_exe_path.Size = ((System.Drawing.Size)(resources.GetObject("text_daemon_exe_path.Size")));
      this.text_daemon_exe_path.TabIndex = ((int)(resources.GetObject("text_daemon_exe_path.TabIndex")));
      this.text_daemon_exe_path.Text = resources.GetString("text_daemon_exe_path.Text");
      this.text_daemon_exe_path.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_daemon_exe_path.TextAlign")));
      this.toolTip.SetToolTip(this.text_daemon_exe_path, resources.GetString("text_daemon_exe_path.ToolTip"));
      this.text_daemon_exe_path.Visible = ((bool)(resources.GetObject("text_daemon_exe_path.Visible")));
      this.text_daemon_exe_path.WordWrap = ((bool)(resources.GetObject("text_daemon_exe_path.WordWrap")));
      // 
      // button__choose_daemon_exe_path
      // 
      this.button__choose_daemon_exe_path.AccessibleDescription = resources.GetString("button__choose_daemon_exe_path.AccessibleDescription");
      this.button__choose_daemon_exe_path.AccessibleName = resources.GetString("button__choose_daemon_exe_path.AccessibleName");
      this.button__choose_daemon_exe_path.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button__choose_daemon_exe_path.Anchor")));
      this.button__choose_daemon_exe_path.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button__choose_daemon_exe_path.BackgroundImage")));
      this.button__choose_daemon_exe_path.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button__choose_daemon_exe_path.Dock")));
      this.button__choose_daemon_exe_path.Enabled = ((bool)(resources.GetObject("button__choose_daemon_exe_path.Enabled")));
      this.button__choose_daemon_exe_path.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button__choose_daemon_exe_path.FlatStyle")));
      this.button__choose_daemon_exe_path.Font = ((System.Drawing.Font)(resources.GetObject("button__choose_daemon_exe_path.Font")));
      this.button__choose_daemon_exe_path.Image = ((System.Drawing.Image)(resources.GetObject("button__choose_daemon_exe_path.Image")));
      this.button__choose_daemon_exe_path.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button__choose_daemon_exe_path.ImageAlign")));
      this.button__choose_daemon_exe_path.ImageIndex = ((int)(resources.GetObject("button__choose_daemon_exe_path.ImageIndex")));
      this.button__choose_daemon_exe_path.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button__choose_daemon_exe_path.ImeMode")));
      this.button__choose_daemon_exe_path.Location = ((System.Drawing.Point)(resources.GetObject("button__choose_daemon_exe_path.Location")));
      this.button__choose_daemon_exe_path.Name = "button__choose_daemon_exe_path";
      this.button__choose_daemon_exe_path.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button__choose_daemon_exe_path.RightToLeft")));
      this.button__choose_daemon_exe_path.Size = ((System.Drawing.Size)(resources.GetObject("button__choose_daemon_exe_path.Size")));
      this.button__choose_daemon_exe_path.TabIndex = ((int)(resources.GetObject("button__choose_daemon_exe_path.TabIndex")));
      this.button__choose_daemon_exe_path.Text = resources.GetString("button__choose_daemon_exe_path.Text");
      this.button__choose_daemon_exe_path.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button__choose_daemon_exe_path.TextAlign")));
      this.toolTip.SetToolTip(this.button__choose_daemon_exe_path, resources.GetString("button__choose_daemon_exe_path.ToolTip"));
      this.button__choose_daemon_exe_path.Visible = ((bool)(resources.GetObject("button__choose_daemon_exe_path.Visible")));
      this.button__choose_daemon_exe_path.Click += new System.EventHandler(this.button__choose_daemon_exe_path_Click);
      // 
      // label_daemon_exe_path
      // 
      this.label_daemon_exe_path.AccessibleDescription = resources.GetString("label_daemon_exe_path.AccessibleDescription");
      this.label_daemon_exe_path.AccessibleName = resources.GetString("label_daemon_exe_path.AccessibleName");
      this.label_daemon_exe_path.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_daemon_exe_path.Anchor")));
      this.label_daemon_exe_path.AutoSize = ((bool)(resources.GetObject("label_daemon_exe_path.AutoSize")));
      this.label_daemon_exe_path.CausesValidation = false;
      this.label_daemon_exe_path.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_daemon_exe_path.Dock")));
      this.label_daemon_exe_path.Enabled = ((bool)(resources.GetObject("label_daemon_exe_path.Enabled")));
      this.label_daemon_exe_path.Font = ((System.Drawing.Font)(resources.GetObject("label_daemon_exe_path.Font")));
      this.label_daemon_exe_path.Image = ((System.Drawing.Image)(resources.GetObject("label_daemon_exe_path.Image")));
      this.label_daemon_exe_path.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_daemon_exe_path.ImageAlign")));
      this.label_daemon_exe_path.ImageIndex = ((int)(resources.GetObject("label_daemon_exe_path.ImageIndex")));
      this.label_daemon_exe_path.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_daemon_exe_path.ImeMode")));
      this.label_daemon_exe_path.Location = ((System.Drawing.Point)(resources.GetObject("label_daemon_exe_path.Location")));
      this.label_daemon_exe_path.Name = "label_daemon_exe_path";
      this.label_daemon_exe_path.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_daemon_exe_path.RightToLeft")));
      this.label_daemon_exe_path.Size = ((System.Drawing.Size)(resources.GetObject("label_daemon_exe_path.Size")));
      this.label_daemon_exe_path.TabIndex = ((int)(resources.GetObject("label_daemon_exe_path.TabIndex")));
      this.label_daemon_exe_path.Text = resources.GetString("label_daemon_exe_path.Text");
      this.label_daemon_exe_path.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_daemon_exe_path.TextAlign")));
      this.toolTip.SetToolTip(this.label_daemon_exe_path, resources.GetString("label_daemon_exe_path.ToolTip"));
      this.label_daemon_exe_path.Visible = ((bool)(resources.GetObject("label_daemon_exe_path.Visible")));
      // 
      // button_ok
      // 
      this.button_ok.AccessibleDescription = resources.GetString("button_ok.AccessibleDescription");
      this.button_ok.AccessibleName = resources.GetString("button_ok.AccessibleName");
      this.button_ok.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_ok.Anchor")));
      this.button_ok.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_ok.BackgroundImage")));
      this.button_ok.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_ok.Dock")));
      this.button_ok.Enabled = ((bool)(resources.GetObject("button_ok.Enabled")));
      this.button_ok.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_ok.FlatStyle")));
      this.button_ok.Font = ((System.Drawing.Font)(resources.GetObject("button_ok.Font")));
      this.button_ok.Image = ((System.Drawing.Image)(resources.GetObject("button_ok.Image")));
      this.button_ok.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_ok.ImageAlign")));
      this.button_ok.ImageIndex = ((int)(resources.GetObject("button_ok.ImageIndex")));
      this.button_ok.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_ok.ImeMode")));
      this.button_ok.Location = ((System.Drawing.Point)(resources.GetObject("button_ok.Location")));
      this.button_ok.Name = "button_ok";
      this.button_ok.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_ok.RightToLeft")));
      this.button_ok.Size = ((System.Drawing.Size)(resources.GetObject("button_ok.Size")));
      this.button_ok.TabIndex = ((int)(resources.GetObject("button_ok.TabIndex")));
      this.button_ok.Text = resources.GetString("button_ok.Text");
      this.button_ok.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_ok.TextAlign")));
      this.toolTip.SetToolTip(this.button_ok, resources.GetString("button_ok.ToolTip"));
      this.button_ok.Visible = ((bool)(resources.GetObject("button_ok.Visible")));
      this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
      // 
      // button_cancel
      // 
      this.button_cancel.AccessibleDescription = resources.GetString("button_cancel.AccessibleDescription");
      this.button_cancel.AccessibleName = resources.GetString("button_cancel.AccessibleName");
      this.button_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_cancel.Anchor")));
      this.button_cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_cancel.BackgroundImage")));
      this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button_cancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_cancel.Dock")));
      this.button_cancel.Enabled = ((bool)(resources.GetObject("button_cancel.Enabled")));
      this.button_cancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_cancel.FlatStyle")));
      this.button_cancel.Font = ((System.Drawing.Font)(resources.GetObject("button_cancel.Font")));
      this.button_cancel.Image = ((System.Drawing.Image)(resources.GetObject("button_cancel.Image")));
      this.button_cancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_cancel.ImageAlign")));
      this.button_cancel.ImageIndex = ((int)(resources.GetObject("button_cancel.ImageIndex")));
      this.button_cancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_cancel.ImeMode")));
      this.button_cancel.Location = ((System.Drawing.Point)(resources.GetObject("button_cancel.Location")));
      this.button_cancel.Name = "button_cancel";
      this.button_cancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_cancel.RightToLeft")));
      this.button_cancel.Size = ((System.Drawing.Size)(resources.GetObject("button_cancel.Size")));
      this.button_cancel.TabIndex = ((int)(resources.GetObject("button_cancel.TabIndex")));
      this.button_cancel.Text = resources.GetString("button_cancel.Text");
      this.button_cancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_cancel.TextAlign")));
      this.toolTip.SetToolTip(this.button_cancel, resources.GetString("button_cancel.ToolTip"));
      this.button_cancel.Visible = ((bool)(resources.GetObject("button_cancel.Visible")));
      // 
      // check_gui_mover_feedback
      // 
      this.check_gui_mover_feedback.AccessibleDescription = resources.GetString("check_gui_mover_feedback.AccessibleDescription");
      this.check_gui_mover_feedback.AccessibleName = resources.GetString("check_gui_mover_feedback.AccessibleName");
      this.check_gui_mover_feedback.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_gui_mover_feedback.Anchor")));
      this.check_gui_mover_feedback.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_gui_mover_feedback.Appearance")));
      this.check_gui_mover_feedback.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_gui_mover_feedback.BackgroundImage")));
      this.check_gui_mover_feedback.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_gui_mover_feedback.CheckAlign")));
      this.check_gui_mover_feedback.Checked = true;
      this.check_gui_mover_feedback.CheckState = System.Windows.Forms.CheckState.Checked;
      this.check_gui_mover_feedback.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_gui_mover_feedback.Dock")));
      this.check_gui_mover_feedback.Enabled = ((bool)(resources.GetObject("check_gui_mover_feedback.Enabled")));
      this.check_gui_mover_feedback.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_gui_mover_feedback.FlatStyle")));
      this.check_gui_mover_feedback.Font = ((System.Drawing.Font)(resources.GetObject("check_gui_mover_feedback.Font")));
      this.check_gui_mover_feedback.Image = ((System.Drawing.Image)(resources.GetObject("check_gui_mover_feedback.Image")));
      this.check_gui_mover_feedback.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_gui_mover_feedback.ImageAlign")));
      this.check_gui_mover_feedback.ImageIndex = ((int)(resources.GetObject("check_gui_mover_feedback.ImageIndex")));
      this.check_gui_mover_feedback.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_gui_mover_feedback.ImeMode")));
      this.check_gui_mover_feedback.Location = ((System.Drawing.Point)(resources.GetObject("check_gui_mover_feedback.Location")));
      this.check_gui_mover_feedback.Name = "check_gui_mover_feedback";
      this.check_gui_mover_feedback.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_gui_mover_feedback.RightToLeft")));
      this.check_gui_mover_feedback.Size = ((System.Drawing.Size)(resources.GetObject("check_gui_mover_feedback.Size")));
      this.check_gui_mover_feedback.TabIndex = ((int)(resources.GetObject("check_gui_mover_feedback.TabIndex")));
      this.check_gui_mover_feedback.Text = resources.GetString("check_gui_mover_feedback.Text");
      this.check_gui_mover_feedback.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_gui_mover_feedback.TextAlign")));
      this.toolTip.SetToolTip(this.check_gui_mover_feedback, resources.GetString("check_gui_mover_feedback.ToolTip"));
      this.check_gui_mover_feedback.Visible = ((bool)(resources.GetObject("check_gui_mover_feedback.Visible")));
      // 
      // text_SlNamePrefix
      // 
      this.text_SlNamePrefix.AccessibleDescription = resources.GetString("text_SlNamePrefix.AccessibleDescription");
      this.text_SlNamePrefix.AccessibleName = resources.GetString("text_SlNamePrefix.AccessibleName");
      this.text_SlNamePrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_SlNamePrefix.Anchor")));
      this.text_SlNamePrefix.AutoSize = ((bool)(resources.GetObject("text_SlNamePrefix.AutoSize")));
      this.text_SlNamePrefix.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_SlNamePrefix.BackgroundImage")));
      this.text_SlNamePrefix.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_SlNamePrefix.Dock")));
      this.text_SlNamePrefix.Enabled = ((bool)(resources.GetObject("text_SlNamePrefix.Enabled")));
      this.text_SlNamePrefix.Font = ((System.Drawing.Font)(resources.GetObject("text_SlNamePrefix.Font")));
      this.text_SlNamePrefix.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_SlNamePrefix.ImeMode")));
      this.text_SlNamePrefix.Location = ((System.Drawing.Point)(resources.GetObject("text_SlNamePrefix.Location")));
      this.text_SlNamePrefix.MaxLength = ((int)(resources.GetObject("text_SlNamePrefix.MaxLength")));
      this.text_SlNamePrefix.Multiline = ((bool)(resources.GetObject("text_SlNamePrefix.Multiline")));
      this.text_SlNamePrefix.Name = "text_SlNamePrefix";
      this.text_SlNamePrefix.PasswordChar = ((char)(resources.GetObject("text_SlNamePrefix.PasswordChar")));
      this.text_SlNamePrefix.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_SlNamePrefix.RightToLeft")));
      this.text_SlNamePrefix.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_SlNamePrefix.ScrollBars")));
      this.text_SlNamePrefix.Size = ((System.Drawing.Size)(resources.GetObject("text_SlNamePrefix.Size")));
      this.text_SlNamePrefix.TabIndex = ((int)(resources.GetObject("text_SlNamePrefix.TabIndex")));
      this.text_SlNamePrefix.Text = resources.GetString("text_SlNamePrefix.Text");
      this.text_SlNamePrefix.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_SlNamePrefix.TextAlign")));
      this.toolTip.SetToolTip(this.text_SlNamePrefix, resources.GetString("text_SlNamePrefix.ToolTip"));
      this.text_SlNamePrefix.Visible = ((bool)(resources.GetObject("text_SlNamePrefix.Visible")));
      this.text_SlNamePrefix.WordWrap = ((bool)(resources.GetObject("text_SlNamePrefix.WordWrap")));
      this.text_SlNamePrefix.TextChanged += new System.EventHandler(this.text_SlName_TextChanged);
      // 
      // text_SlNameSuffix
      // 
      this.text_SlNameSuffix.AccessibleDescription = resources.GetString("text_SlNameSuffix.AccessibleDescription");
      this.text_SlNameSuffix.AccessibleName = resources.GetString("text_SlNameSuffix.AccessibleName");
      this.text_SlNameSuffix.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_SlNameSuffix.Anchor")));
      this.text_SlNameSuffix.AutoSize = ((bool)(resources.GetObject("text_SlNameSuffix.AutoSize")));
      this.text_SlNameSuffix.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_SlNameSuffix.BackgroundImage")));
      this.text_SlNameSuffix.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_SlNameSuffix.Dock")));
      this.text_SlNameSuffix.Enabled = ((bool)(resources.GetObject("text_SlNameSuffix.Enabled")));
      this.text_SlNameSuffix.Font = ((System.Drawing.Font)(resources.GetObject("text_SlNameSuffix.Font")));
      this.text_SlNameSuffix.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_SlNameSuffix.ImeMode")));
      this.text_SlNameSuffix.Location = ((System.Drawing.Point)(resources.GetObject("text_SlNameSuffix.Location")));
      this.text_SlNameSuffix.MaxLength = ((int)(resources.GetObject("text_SlNameSuffix.MaxLength")));
      this.text_SlNameSuffix.Multiline = ((bool)(resources.GetObject("text_SlNameSuffix.Multiline")));
      this.text_SlNameSuffix.Name = "text_SlNameSuffix";
      this.text_SlNameSuffix.PasswordChar = ((char)(resources.GetObject("text_SlNameSuffix.PasswordChar")));
      this.text_SlNameSuffix.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_SlNameSuffix.RightToLeft")));
      this.text_SlNameSuffix.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_SlNameSuffix.ScrollBars")));
      this.text_SlNameSuffix.Size = ((System.Drawing.Size)(resources.GetObject("text_SlNameSuffix.Size")));
      this.text_SlNameSuffix.TabIndex = ((int)(resources.GetObject("text_SlNameSuffix.TabIndex")));
      this.text_SlNameSuffix.Text = resources.GetString("text_SlNameSuffix.Text");
      this.text_SlNameSuffix.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_SlNameSuffix.TextAlign")));
      this.toolTip.SetToolTip(this.text_SlNameSuffix, resources.GetString("text_SlNameSuffix.ToolTip"));
      this.text_SlNameSuffix.Visible = ((bool)(resources.GetObject("text_SlNameSuffix.Visible")));
      this.text_SlNameSuffix.WordWrap = ((bool)(resources.GetObject("text_SlNameSuffix.WordWrap")));
      this.text_SlNameSuffix.TextChanged += new System.EventHandler(this.text_SlName_TextChanged);
      // 
      // check_sl_auto_restore
      // 
      this.check_sl_auto_restore.AccessibleDescription = resources.GetString("check_sl_auto_restore.AccessibleDescription");
      this.check_sl_auto_restore.AccessibleName = resources.GetString("check_sl_auto_restore.AccessibleName");
      this.check_sl_auto_restore.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_sl_auto_restore.Anchor")));
      this.check_sl_auto_restore.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_sl_auto_restore.Appearance")));
      this.check_sl_auto_restore.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_sl_auto_restore.BackgroundImage")));
      this.check_sl_auto_restore.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_sl_auto_restore.CheckAlign")));
      this.check_sl_auto_restore.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_sl_auto_restore.Dock")));
      this.check_sl_auto_restore.Enabled = ((bool)(resources.GetObject("check_sl_auto_restore.Enabled")));
      this.check_sl_auto_restore.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_sl_auto_restore.FlatStyle")));
      this.check_sl_auto_restore.Font = ((System.Drawing.Font)(resources.GetObject("check_sl_auto_restore.Font")));
      this.check_sl_auto_restore.Image = ((System.Drawing.Image)(resources.GetObject("check_sl_auto_restore.Image")));
      this.check_sl_auto_restore.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_sl_auto_restore.ImageAlign")));
      this.check_sl_auto_restore.ImageIndex = ((int)(resources.GetObject("check_sl_auto_restore.ImageIndex")));
      this.check_sl_auto_restore.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_sl_auto_restore.ImeMode")));
      this.check_sl_auto_restore.Location = ((System.Drawing.Point)(resources.GetObject("check_sl_auto_restore.Location")));
      this.check_sl_auto_restore.Name = "check_sl_auto_restore";
      this.check_sl_auto_restore.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_sl_auto_restore.RightToLeft")));
      this.check_sl_auto_restore.Size = ((System.Drawing.Size)(resources.GetObject("check_sl_auto_restore.Size")));
      this.check_sl_auto_restore.TabIndex = ((int)(resources.GetObject("check_sl_auto_restore.TabIndex")));
      this.check_sl_auto_restore.Text = resources.GetString("check_sl_auto_restore.Text");
      this.check_sl_auto_restore.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_sl_auto_restore.TextAlign")));
      this.toolTip.SetToolTip(this.check_sl_auto_restore, resources.GetString("check_sl_auto_restore.ToolTip"));
      this.check_sl_auto_restore.Visible = ((bool)(resources.GetObject("check_sl_auto_restore.Visible")));
      // 
      // check_auto_restore
      // 
      this.check_auto_restore.AccessibleDescription = resources.GetString("check_auto_restore.AccessibleDescription");
      this.check_auto_restore.AccessibleName = resources.GetString("check_auto_restore.AccessibleName");
      this.check_auto_restore.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_auto_restore.Anchor")));
      this.check_auto_restore.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_auto_restore.Appearance")));
      this.check_auto_restore.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_auto_restore.BackgroundImage")));
      this.check_auto_restore.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_auto_restore.CheckAlign")));
      this.check_auto_restore.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_auto_restore.Dock")));
      this.check_auto_restore.Enabled = ((bool)(resources.GetObject("check_auto_restore.Enabled")));
      this.check_auto_restore.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_auto_restore.FlatStyle")));
      this.check_auto_restore.Font = ((System.Drawing.Font)(resources.GetObject("check_auto_restore.Font")));
      this.check_auto_restore.Image = ((System.Drawing.Image)(resources.GetObject("check_auto_restore.Image")));
      this.check_auto_restore.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_auto_restore.ImageAlign")));
      this.check_auto_restore.ImageIndex = ((int)(resources.GetObject("check_auto_restore.ImageIndex")));
      this.check_auto_restore.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_auto_restore.ImeMode")));
      this.check_auto_restore.Location = ((System.Drawing.Point)(resources.GetObject("check_auto_restore.Location")));
      this.check_auto_restore.Name = "check_auto_restore";
      this.check_auto_restore.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_auto_restore.RightToLeft")));
      this.check_auto_restore.Size = ((System.Drawing.Size)(resources.GetObject("check_auto_restore.Size")));
      this.check_auto_restore.TabIndex = ((int)(resources.GetObject("check_auto_restore.TabIndex")));
      this.check_auto_restore.Text = resources.GetString("check_auto_restore.Text");
      this.check_auto_restore.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_auto_restore.TextAlign")));
      this.toolTip.SetToolTip(this.check_auto_restore, resources.GetString("check_auto_restore.ToolTip"));
      this.check_auto_restore.Visible = ((bool)(resources.GetObject("check_auto_restore.Visible")));
      // 
      // check_update_timer
      // 
      this.check_update_timer.AccessibleDescription = resources.GetString("check_update_timer.AccessibleDescription");
      this.check_update_timer.AccessibleName = resources.GetString("check_update_timer.AccessibleName");
      this.check_update_timer.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_update_timer.Anchor")));
      this.check_update_timer.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_update_timer.Appearance")));
      this.check_update_timer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_update_timer.BackgroundImage")));
      this.check_update_timer.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_update_timer.CheckAlign")));
      this.check_update_timer.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_update_timer.Dock")));
      this.check_update_timer.Enabled = ((bool)(resources.GetObject("check_update_timer.Enabled")));
      this.check_update_timer.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_update_timer.FlatStyle")));
      this.check_update_timer.Font = ((System.Drawing.Font)(resources.GetObject("check_update_timer.Font")));
      this.check_update_timer.Image = ((System.Drawing.Image)(resources.GetObject("check_update_timer.Image")));
      this.check_update_timer.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_update_timer.ImageAlign")));
      this.check_update_timer.ImageIndex = ((int)(resources.GetObject("check_update_timer.ImageIndex")));
      this.check_update_timer.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_update_timer.ImeMode")));
      this.check_update_timer.Location = ((System.Drawing.Point)(resources.GetObject("check_update_timer.Location")));
      this.check_update_timer.Name = "check_update_timer";
      this.check_update_timer.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_update_timer.RightToLeft")));
      this.check_update_timer.Size = ((System.Drawing.Size)(resources.GetObject("check_update_timer.Size")));
      this.check_update_timer.TabIndex = ((int)(resources.GetObject("check_update_timer.TabIndex")));
      this.check_update_timer.Text = resources.GetString("check_update_timer.Text");
      this.check_update_timer.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_update_timer.TextAlign")));
      this.toolTip.SetToolTip(this.check_update_timer, resources.GetString("check_update_timer.ToolTip"));
      this.check_update_timer.Visible = ((bool)(resources.GetObject("check_update_timer.Visible")));
      this.check_update_timer.CheckedChanged += new System.EventHandler(this.check_update_timer_CheckedChanged);
      // 
      // check_restoreToDefault
      // 
      this.check_restoreToDefault.AccessibleDescription = resources.GetString("check_restoreToDefault.AccessibleDescription");
      this.check_restoreToDefault.AccessibleName = resources.GetString("check_restoreToDefault.AccessibleName");
      this.check_restoreToDefault.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_restoreToDefault.Anchor")));
      this.check_restoreToDefault.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_restoreToDefault.Appearance")));
      this.check_restoreToDefault.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_restoreToDefault.BackgroundImage")));
      this.check_restoreToDefault.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_restoreToDefault.CheckAlign")));
      this.check_restoreToDefault.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_restoreToDefault.Dock")));
      this.check_restoreToDefault.Enabled = ((bool)(resources.GetObject("check_restoreToDefault.Enabled")));
      this.check_restoreToDefault.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_restoreToDefault.FlatStyle")));
      this.check_restoreToDefault.Font = ((System.Drawing.Font)(resources.GetObject("check_restoreToDefault.Font")));
      this.check_restoreToDefault.Image = ((System.Drawing.Image)(resources.GetObject("check_restoreToDefault.Image")));
      this.check_restoreToDefault.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_restoreToDefault.ImageAlign")));
      this.check_restoreToDefault.ImageIndex = ((int)(resources.GetObject("check_restoreToDefault.ImageIndex")));
      this.check_restoreToDefault.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_restoreToDefault.ImeMode")));
      this.check_restoreToDefault.Location = ((System.Drawing.Point)(resources.GetObject("check_restoreToDefault.Location")));
      this.check_restoreToDefault.Name = "check_restoreToDefault";
      this.check_restoreToDefault.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_restoreToDefault.RightToLeft")));
      this.check_restoreToDefault.Size = ((System.Drawing.Size)(resources.GetObject("check_restoreToDefault.Size")));
      this.check_restoreToDefault.TabIndex = ((int)(resources.GetObject("check_restoreToDefault.TabIndex")));
      this.check_restoreToDefault.Text = resources.GetString("check_restoreToDefault.Text");
      this.check_restoreToDefault.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_restoreToDefault.TextAlign")));
      this.toolTip.SetToolTip(this.check_restoreToDefault, resources.GetString("check_restoreToDefault.ToolTip"));
      this.check_restoreToDefault.Visible = ((bool)(resources.GetObject("check_restoreToDefault.Visible")));
      // 
      // check_features_enableClocking
      // 
      this.check_features_enableClocking.AccessibleDescription = resources.GetString("check_features_enableClocking.AccessibleDescription");
      this.check_features_enableClocking.AccessibleName = resources.GetString("check_features_enableClocking.AccessibleName");
      this.check_features_enableClocking.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_features_enableClocking.Anchor")));
      this.check_features_enableClocking.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_features_enableClocking.Appearance")));
      this.check_features_enableClocking.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_features_enableClocking.BackgroundImage")));
      this.check_features_enableClocking.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_features_enableClocking.CheckAlign")));
      this.check_features_enableClocking.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_features_enableClocking.Dock")));
      this.check_features_enableClocking.Enabled = ((bool)(resources.GetObject("check_features_enableClocking.Enabled")));
      this.check_features_enableClocking.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_features_enableClocking.FlatStyle")));
      this.check_features_enableClocking.Font = ((System.Drawing.Font)(resources.GetObject("check_features_enableClocking.Font")));
      this.check_features_enableClocking.Image = ((System.Drawing.Image)(resources.GetObject("check_features_enableClocking.Image")));
      this.check_features_enableClocking.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_features_enableClocking.ImageAlign")));
      this.check_features_enableClocking.ImageIndex = ((int)(resources.GetObject("check_features_enableClocking.ImageIndex")));
      this.check_features_enableClocking.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_features_enableClocking.ImeMode")));
      this.check_features_enableClocking.Location = ((System.Drawing.Point)(resources.GetObject("check_features_enableClocking.Location")));
      this.check_features_enableClocking.Name = "check_features_enableClocking";
      this.check_features_enableClocking.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_features_enableClocking.RightToLeft")));
      this.check_features_enableClocking.Size = ((System.Drawing.Size)(resources.GetObject("check_features_enableClocking.Size")));
      this.check_features_enableClocking.TabIndex = ((int)(resources.GetObject("check_features_enableClocking.TabIndex")));
      this.check_features_enableClocking.Text = resources.GetString("check_features_enableClocking.Text");
      this.check_features_enableClocking.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_features_enableClocking.TextAlign")));
      this.toolTip.SetToolTip(this.check_features_enableClocking, resources.GetString("check_features_enableClocking.ToolTip"));
      this.check_features_enableClocking.Visible = ((bool)(resources.GetObject("check_features_enableClocking.Visible")));
      // 
      // tab
      // 
      this.tab.AccessibleDescription = resources.GetString("tab.AccessibleDescription");
      this.tab.AccessibleName = resources.GetString("tab.AccessibleName");
      this.tab.Alignment = ((System.Windows.Forms.TabAlignment)(resources.GetObject("tab.Alignment")));
      this.tab.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab.Anchor")));
      this.tab.Appearance = ((System.Windows.Forms.TabAppearance)(resources.GetObject("tab.Appearance")));
      this.tab.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab.BackgroundImage")));
      this.tab.Controls.Add(this.tab_main);
      this.tab.Controls.Add(this.tab_restore);
      this.tab.Controls.Add(this.tab_sl);
      this.tab.Controls.Add(this.tab_clocking);
      this.tab.Controls.Add(this.tab_extended);
      this.tab.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab.Dock")));
      this.tab.Enabled = ((bool)(resources.GetObject("tab.Enabled")));
      this.tab.Font = ((System.Drawing.Font)(resources.GetObject("tab.Font")));
      this.tab.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab.ImeMode")));
      this.tab.ItemSize = ((System.Drawing.Size)(resources.GetObject("tab.ItemSize")));
      this.tab.Location = ((System.Drawing.Point)(resources.GetObject("tab.Location")));
      this.tab.Name = "tab";
      this.tab.Padding = ((System.Drawing.Point)(resources.GetObject("tab.Padding")));
      this.tab.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab.RightToLeft")));
      this.tab.SelectedIndex = 0;
      this.tab.ShowToolTips = ((bool)(resources.GetObject("tab.ShowToolTips")));
      this.tab.Size = ((System.Drawing.Size)(resources.GetObject("tab.Size")));
      this.tab.TabIndex = ((int)(resources.GetObject("tab.TabIndex")));
      this.tab.Text = resources.GetString("tab.Text");
      this.toolTip.SetToolTip(this.tab, resources.GetString("tab.ToolTip"));
      this.tab.Visible = ((bool)(resources.GetObject("tab.Visible")));
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
      this.tab_main.Controls.Add(this.check_gui_show_tray_icon);
      this.tab_main.Controls.Add(this.check_update_timer);
      this.tab_main.Controls.Add(this.label_update_timer);
      this.tab_main.Controls.Add(this.text_update_timer);
      this.tab_main.Controls.Add(this.label_daemon_exe_path);
      this.tab_main.Controls.Add(this.button__choose_daemon_exe_path);
      this.tab_main.Controls.Add(this.text_daemon_exe_path);
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
      // check_gui_show_tray_icon
      // 
      this.check_gui_show_tray_icon.AccessibleDescription = resources.GetString("check_gui_show_tray_icon.AccessibleDescription");
      this.check_gui_show_tray_icon.AccessibleName = resources.GetString("check_gui_show_tray_icon.AccessibleName");
      this.check_gui_show_tray_icon.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_gui_show_tray_icon.Anchor")));
      this.check_gui_show_tray_icon.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_gui_show_tray_icon.Appearance")));
      this.check_gui_show_tray_icon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_gui_show_tray_icon.BackgroundImage")));
      this.check_gui_show_tray_icon.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_gui_show_tray_icon.CheckAlign")));
      this.check_gui_show_tray_icon.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_gui_show_tray_icon.Dock")));
      this.check_gui_show_tray_icon.Enabled = ((bool)(resources.GetObject("check_gui_show_tray_icon.Enabled")));
      this.check_gui_show_tray_icon.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_gui_show_tray_icon.FlatStyle")));
      this.check_gui_show_tray_icon.Font = ((System.Drawing.Font)(resources.GetObject("check_gui_show_tray_icon.Font")));
      this.check_gui_show_tray_icon.Image = ((System.Drawing.Image)(resources.GetObject("check_gui_show_tray_icon.Image")));
      this.check_gui_show_tray_icon.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_gui_show_tray_icon.ImageAlign")));
      this.check_gui_show_tray_icon.ImageIndex = ((int)(resources.GetObject("check_gui_show_tray_icon.ImageIndex")));
      this.check_gui_show_tray_icon.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_gui_show_tray_icon.ImeMode")));
      this.check_gui_show_tray_icon.Location = ((System.Drawing.Point)(resources.GetObject("check_gui_show_tray_icon.Location")));
      this.check_gui_show_tray_icon.Name = "check_gui_show_tray_icon";
      this.check_gui_show_tray_icon.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_gui_show_tray_icon.RightToLeft")));
      this.check_gui_show_tray_icon.Size = ((System.Drawing.Size)(resources.GetObject("check_gui_show_tray_icon.Size")));
      this.check_gui_show_tray_icon.TabIndex = ((int)(resources.GetObject("check_gui_show_tray_icon.TabIndex")));
      this.check_gui_show_tray_icon.Text = resources.GetString("check_gui_show_tray_icon.Text");
      this.check_gui_show_tray_icon.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_gui_show_tray_icon.TextAlign")));
      this.toolTip.SetToolTip(this.check_gui_show_tray_icon, resources.GetString("check_gui_show_tray_icon.ToolTip"));
      this.check_gui_show_tray_icon.Visible = ((bool)(resources.GetObject("check_gui_show_tray_icon.Visible")));
      // 
      // label_update_timer
      // 
      this.label_update_timer.AccessibleDescription = resources.GetString("label_update_timer.AccessibleDescription");
      this.label_update_timer.AccessibleName = resources.GetString("label_update_timer.AccessibleName");
      this.label_update_timer.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_update_timer.Anchor")));
      this.label_update_timer.AutoSize = ((bool)(resources.GetObject("label_update_timer.AutoSize")));
      this.label_update_timer.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_update_timer.Dock")));
      this.label_update_timer.Enabled = ((bool)(resources.GetObject("label_update_timer.Enabled")));
      this.label_update_timer.Font = ((System.Drawing.Font)(resources.GetObject("label_update_timer.Font")));
      this.label_update_timer.Image = ((System.Drawing.Image)(resources.GetObject("label_update_timer.Image")));
      this.label_update_timer.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_update_timer.ImageAlign")));
      this.label_update_timer.ImageIndex = ((int)(resources.GetObject("label_update_timer.ImageIndex")));
      this.label_update_timer.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_update_timer.ImeMode")));
      this.label_update_timer.Location = ((System.Drawing.Point)(resources.GetObject("label_update_timer.Location")));
      this.label_update_timer.Name = "label_update_timer";
      this.label_update_timer.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_update_timer.RightToLeft")));
      this.label_update_timer.Size = ((System.Drawing.Size)(resources.GetObject("label_update_timer.Size")));
      this.label_update_timer.TabIndex = ((int)(resources.GetObject("label_update_timer.TabIndex")));
      this.label_update_timer.Text = resources.GetString("label_update_timer.Text");
      this.label_update_timer.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_update_timer.TextAlign")));
      this.toolTip.SetToolTip(this.label_update_timer, resources.GetString("label_update_timer.ToolTip"));
      this.label_update_timer.Visible = ((bool)(resources.GetObject("label_update_timer.Visible")));
      // 
      // text_update_timer
      // 
      this.text_update_timer.AccessibleDescription = resources.GetString("text_update_timer.AccessibleDescription");
      this.text_update_timer.AccessibleName = resources.GetString("text_update_timer.AccessibleName");
      this.text_update_timer.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_update_timer.Anchor")));
      this.text_update_timer.AutoSize = ((bool)(resources.GetObject("text_update_timer.AutoSize")));
      this.text_update_timer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_update_timer.BackgroundImage")));
      this.text_update_timer.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_update_timer.Dock")));
      this.text_update_timer.Enabled = ((bool)(resources.GetObject("text_update_timer.Enabled")));
      this.text_update_timer.Font = ((System.Drawing.Font)(resources.GetObject("text_update_timer.Font")));
      this.text_update_timer.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_update_timer.ImeMode")));
      this.text_update_timer.Location = ((System.Drawing.Point)(resources.GetObject("text_update_timer.Location")));
      this.text_update_timer.MaxLength = ((int)(resources.GetObject("text_update_timer.MaxLength")));
      this.text_update_timer.Multiline = ((bool)(resources.GetObject("text_update_timer.Multiline")));
      this.text_update_timer.Name = "text_update_timer";
      this.text_update_timer.PasswordChar = ((char)(resources.GetObject("text_update_timer.PasswordChar")));
      this.text_update_timer.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_update_timer.RightToLeft")));
      this.text_update_timer.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_update_timer.ScrollBars")));
      this.text_update_timer.Size = ((System.Drawing.Size)(resources.GetObject("text_update_timer.Size")));
      this.text_update_timer.TabIndex = ((int)(resources.GetObject("text_update_timer.TabIndex")));
      this.text_update_timer.Tag = "0";
      this.text_update_timer.Text = resources.GetString("text_update_timer.Text");
      this.text_update_timer.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_update_timer.TextAlign")));
      this.toolTip.SetToolTip(this.text_update_timer, resources.GetString("text_update_timer.ToolTip"));
      this.text_update_timer.Visible = ((bool)(resources.GetObject("text_update_timer.Visible")));
      this.text_update_timer.WordWrap = ((bool)(resources.GetObject("text_update_timer.WordWrap")));
      // 
      // tab_restore
      // 
      this.tab_restore.AccessibleDescription = resources.GetString("tab_restore.AccessibleDescription");
      this.tab_restore.AccessibleName = resources.GetString("tab_restore.AccessibleName");
      this.tab_restore.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab_restore.Anchor")));
      this.tab_restore.AutoScroll = ((bool)(resources.GetObject("tab_restore.AutoScroll")));
      this.tab_restore.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tab_restore.AutoScrollMargin")));
      this.tab_restore.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tab_restore.AutoScrollMinSize")));
      this.tab_restore.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab_restore.BackgroundImage")));
      this.tab_restore.Controls.Add(this.group_autorestore);
      this.tab_restore.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab_restore.Dock")));
      this.tab_restore.Enabled = ((bool)(resources.GetObject("tab_restore.Enabled")));
      this.tab_restore.Font = ((System.Drawing.Font)(resources.GetObject("tab_restore.Font")));
      this.tab_restore.ImageIndex = ((int)(resources.GetObject("tab_restore.ImageIndex")));
      this.tab_restore.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab_restore.ImeMode")));
      this.tab_restore.Location = ((System.Drawing.Point)(resources.GetObject("tab_restore.Location")));
      this.tab_restore.Name = "tab_restore";
      this.tab_restore.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab_restore.RightToLeft")));
      this.tab_restore.Size = ((System.Drawing.Size)(resources.GetObject("tab_restore.Size")));
      this.tab_restore.TabIndex = ((int)(resources.GetObject("tab_restore.TabIndex")));
      this.tab_restore.Text = resources.GetString("tab_restore.Text");
      this.toolTip.SetToolTip(this.tab_restore, resources.GetString("tab_restore.ToolTip"));
      this.tab_restore.ToolTipText = resources.GetString("tab_restore.ToolTipText");
      this.tab_restore.Visible = ((bool)(resources.GetObject("tab_restore.Visible")));
      // 
      // group_autorestore
      // 
      this.group_autorestore.AccessibleDescription = resources.GetString("group_autorestore.AccessibleDescription");
      this.group_autorestore.AccessibleName = resources.GetString("group_autorestore.AccessibleName");
      this.group_autorestore.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("group_autorestore.Anchor")));
      this.group_autorestore.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("group_autorestore.BackgroundImage")));
      this.group_autorestore.Controls.Add(this.check_auto_restore);
      this.group_autorestore.Controls.Add(this.check_sl_auto_restore);
      this.group_autorestore.Controls.Add(this.check_restoreToDefault);
      this.group_autorestore.Controls.Add(this.combo_defaultProfile);
      this.group_autorestore.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("group_autorestore.Dock")));
      this.group_autorestore.Enabled = ((bool)(resources.GetObject("group_autorestore.Enabled")));
      this.group_autorestore.Font = ((System.Drawing.Font)(resources.GetObject("group_autorestore.Font")));
      this.group_autorestore.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("group_autorestore.ImeMode")));
      this.group_autorestore.Location = ((System.Drawing.Point)(resources.GetObject("group_autorestore.Location")));
      this.group_autorestore.Name = "group_autorestore";
      this.group_autorestore.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("group_autorestore.RightToLeft")));
      this.group_autorestore.Size = ((System.Drawing.Size)(resources.GetObject("group_autorestore.Size")));
      this.group_autorestore.TabIndex = ((int)(resources.GetObject("group_autorestore.TabIndex")));
      this.group_autorestore.TabStop = false;
      this.group_autorestore.Text = resources.GetString("group_autorestore.Text");
      this.toolTip.SetToolTip(this.group_autorestore, resources.GetString("group_autorestore.ToolTip"));
      this.group_autorestore.Visible = ((bool)(resources.GetObject("group_autorestore.Visible")));
      // 
      // combo_defaultProfile
      // 
      this.combo_defaultProfile.AccessibleDescription = resources.GetString("combo_defaultProfile.AccessibleDescription");
      this.combo_defaultProfile.AccessibleName = resources.GetString("combo_defaultProfile.AccessibleName");
      this.combo_defaultProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_defaultProfile.Anchor")));
      this.combo_defaultProfile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_defaultProfile.BackgroundImage")));
      this.combo_defaultProfile.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_defaultProfile.Dock")));
      this.combo_defaultProfile.Enabled = ((bool)(resources.GetObject("combo_defaultProfile.Enabled")));
      this.combo_defaultProfile.Font = ((System.Drawing.Font)(resources.GetObject("combo_defaultProfile.Font")));
      this.combo_defaultProfile.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_defaultProfile.ImeMode")));
      this.combo_defaultProfile.IntegralHeight = ((bool)(resources.GetObject("combo_defaultProfile.IntegralHeight")));
      this.combo_defaultProfile.ItemHeight = ((int)(resources.GetObject("combo_defaultProfile.ItemHeight")));
      this.combo_defaultProfile.Location = ((System.Drawing.Point)(resources.GetObject("combo_defaultProfile.Location")));
      this.combo_defaultProfile.MaxDropDownItems = ((int)(resources.GetObject("combo_defaultProfile.MaxDropDownItems")));
      this.combo_defaultProfile.MaxLength = ((int)(resources.GetObject("combo_defaultProfile.MaxLength")));
      this.combo_defaultProfile.Name = "combo_defaultProfile";
      this.combo_defaultProfile.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_defaultProfile.RightToLeft")));
      this.combo_defaultProfile.Size = ((System.Drawing.Size)(resources.GetObject("combo_defaultProfile.Size")));
      this.combo_defaultProfile.TabIndex = ((int)(resources.GetObject("combo_defaultProfile.TabIndex")));
      this.combo_defaultProfile.Text = resources.GetString("combo_defaultProfile.Text");
      this.toolTip.SetToolTip(this.combo_defaultProfile, resources.GetString("combo_defaultProfile.ToolTip"));
      this.combo_defaultProfile.Visible = ((bool)(resources.GetObject("combo_defaultProfile.Visible")));
      // 
      // tab_sl
      // 
      this.tab_sl.AccessibleDescription = resources.GetString("tab_sl.AccessibleDescription");
      this.tab_sl.AccessibleName = resources.GetString("tab_sl.AccessibleName");
      this.tab_sl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab_sl.Anchor")));
      this.tab_sl.AutoScroll = ((bool)(resources.GetObject("tab_sl.AutoScroll")));
      this.tab_sl.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tab_sl.AutoScrollMargin")));
      this.tab_sl.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tab_sl.AutoScrollMinSize")));
      this.tab_sl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab_sl.BackgroundImage")));
      this.tab_sl.Controls.Add(this.label_SlPreview);
      this.tab_sl.Controls.Add(this.text_SlNameSuffix);
      this.tab_sl.Controls.Add(this.label_sl_prefix);
      this.tab_sl.Controls.Add(this.text_SlNamePrefix);
      this.tab_sl.Controls.Add(this.label_sl_suffix);
      this.tab_sl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab_sl.Dock")));
      this.tab_sl.Enabled = ((bool)(resources.GetObject("tab_sl.Enabled")));
      this.tab_sl.Font = ((System.Drawing.Font)(resources.GetObject("tab_sl.Font")));
      this.tab_sl.ImageIndex = ((int)(resources.GetObject("tab_sl.ImageIndex")));
      this.tab_sl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab_sl.ImeMode")));
      this.tab_sl.Location = ((System.Drawing.Point)(resources.GetObject("tab_sl.Location")));
      this.tab_sl.Name = "tab_sl";
      this.tab_sl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab_sl.RightToLeft")));
      this.tab_sl.Size = ((System.Drawing.Size)(resources.GetObject("tab_sl.Size")));
      this.tab_sl.TabIndex = ((int)(resources.GetObject("tab_sl.TabIndex")));
      this.tab_sl.Text = resources.GetString("tab_sl.Text");
      this.toolTip.SetToolTip(this.tab_sl, resources.GetString("tab_sl.ToolTip"));
      this.tab_sl.ToolTipText = resources.GetString("tab_sl.ToolTipText");
      this.tab_sl.Visible = ((bool)(resources.GetObject("tab_sl.Visible")));
      // 
      // label_SlPreview
      // 
      this.label_SlPreview.AccessibleDescription = resources.GetString("label_SlPreview.AccessibleDescription");
      this.label_SlPreview.AccessibleName = resources.GetString("label_SlPreview.AccessibleName");
      this.label_SlPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_SlPreview.Anchor")));
      this.label_SlPreview.AutoSize = ((bool)(resources.GetObject("label_SlPreview.AutoSize")));
      this.label_SlPreview.BackColor = System.Drawing.SystemColors.Control;
      this.label_SlPreview.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_SlPreview.Dock")));
      this.label_SlPreview.Enabled = ((bool)(resources.GetObject("label_SlPreview.Enabled")));
      this.label_SlPreview.Font = ((System.Drawing.Font)(resources.GetObject("label_SlPreview.Font")));
      this.label_SlPreview.ForeColor = System.Drawing.SystemColors.ControlText;
      this.label_SlPreview.Image = ((System.Drawing.Image)(resources.GetObject("label_SlPreview.Image")));
      this.label_SlPreview.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_SlPreview.ImageAlign")));
      this.label_SlPreview.ImageIndex = ((int)(resources.GetObject("label_SlPreview.ImageIndex")));
      this.label_SlPreview.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_SlPreview.ImeMode")));
      this.label_SlPreview.Location = ((System.Drawing.Point)(resources.GetObject("label_SlPreview.Location")));
      this.label_SlPreview.Name = "label_SlPreview";
      this.label_SlPreview.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_SlPreview.RightToLeft")));
      this.label_SlPreview.Size = ((System.Drawing.Size)(resources.GetObject("label_SlPreview.Size")));
      this.label_SlPreview.TabIndex = ((int)(resources.GetObject("label_SlPreview.TabIndex")));
      this.label_SlPreview.Text = resources.GetString("label_SlPreview.Text");
      this.label_SlPreview.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_SlPreview.TextAlign")));
      this.toolTip.SetToolTip(this.label_SlPreview, resources.GetString("label_SlPreview.ToolTip"));
      this.label_SlPreview.Visible = ((bool)(resources.GetObject("label_SlPreview.Visible")));
      // 
      // label_sl_prefix
      // 
      this.label_sl_prefix.AccessibleDescription = resources.GetString("label_sl_prefix.AccessibleDescription");
      this.label_sl_prefix.AccessibleName = resources.GetString("label_sl_prefix.AccessibleName");
      this.label_sl_prefix.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_sl_prefix.Anchor")));
      this.label_sl_prefix.AutoSize = ((bool)(resources.GetObject("label_sl_prefix.AutoSize")));
      this.label_sl_prefix.CausesValidation = false;
      this.label_sl_prefix.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_sl_prefix.Dock")));
      this.label_sl_prefix.Enabled = ((bool)(resources.GetObject("label_sl_prefix.Enabled")));
      this.label_sl_prefix.Font = ((System.Drawing.Font)(resources.GetObject("label_sl_prefix.Font")));
      this.label_sl_prefix.Image = ((System.Drawing.Image)(resources.GetObject("label_sl_prefix.Image")));
      this.label_sl_prefix.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_sl_prefix.ImageAlign")));
      this.label_sl_prefix.ImageIndex = ((int)(resources.GetObject("label_sl_prefix.ImageIndex")));
      this.label_sl_prefix.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_sl_prefix.ImeMode")));
      this.label_sl_prefix.Location = ((System.Drawing.Point)(resources.GetObject("label_sl_prefix.Location")));
      this.label_sl_prefix.Name = "label_sl_prefix";
      this.label_sl_prefix.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_sl_prefix.RightToLeft")));
      this.label_sl_prefix.Size = ((System.Drawing.Size)(resources.GetObject("label_sl_prefix.Size")));
      this.label_sl_prefix.TabIndex = ((int)(resources.GetObject("label_sl_prefix.TabIndex")));
      this.label_sl_prefix.Text = resources.GetString("label_sl_prefix.Text");
      this.label_sl_prefix.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_sl_prefix.TextAlign")));
      this.toolTip.SetToolTip(this.label_sl_prefix, resources.GetString("label_sl_prefix.ToolTip"));
      this.label_sl_prefix.Visible = ((bool)(resources.GetObject("label_sl_prefix.Visible")));
      // 
      // label_sl_suffix
      // 
      this.label_sl_suffix.AccessibleDescription = resources.GetString("label_sl_suffix.AccessibleDescription");
      this.label_sl_suffix.AccessibleName = resources.GetString("label_sl_suffix.AccessibleName");
      this.label_sl_suffix.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_sl_suffix.Anchor")));
      this.label_sl_suffix.AutoSize = ((bool)(resources.GetObject("label_sl_suffix.AutoSize")));
      this.label_sl_suffix.CausesValidation = false;
      this.label_sl_suffix.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_sl_suffix.Dock")));
      this.label_sl_suffix.Enabled = ((bool)(resources.GetObject("label_sl_suffix.Enabled")));
      this.label_sl_suffix.Font = ((System.Drawing.Font)(resources.GetObject("label_sl_suffix.Font")));
      this.label_sl_suffix.Image = ((System.Drawing.Image)(resources.GetObject("label_sl_suffix.Image")));
      this.label_sl_suffix.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_sl_suffix.ImageAlign")));
      this.label_sl_suffix.ImageIndex = ((int)(resources.GetObject("label_sl_suffix.ImageIndex")));
      this.label_sl_suffix.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_sl_suffix.ImeMode")));
      this.label_sl_suffix.Location = ((System.Drawing.Point)(resources.GetObject("label_sl_suffix.Location")));
      this.label_sl_suffix.Name = "label_sl_suffix";
      this.label_sl_suffix.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_sl_suffix.RightToLeft")));
      this.label_sl_suffix.Size = ((System.Drawing.Size)(resources.GetObject("label_sl_suffix.Size")));
      this.label_sl_suffix.TabIndex = ((int)(resources.GetObject("label_sl_suffix.TabIndex")));
      this.label_sl_suffix.Text = resources.GetString("label_sl_suffix.Text");
      this.label_sl_suffix.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_sl_suffix.TextAlign")));
      this.toolTip.SetToolTip(this.label_sl_suffix, resources.GetString("label_sl_suffix.ToolTip"));
      this.label_sl_suffix.Visible = ((bool)(resources.GetObject("label_sl_suffix.Visible")));
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
      this.tab_clocking.Controls.Add(this.label14);
      this.tab_clocking.Controls.Add(this.combo_clocking_prof_restorePresets);
      this.tab_clocking.Controls.Add(this.groupBox2);
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
      // label14
      // 
      this.label14.AccessibleDescription = resources.GetString("label14.AccessibleDescription");
      this.label14.AccessibleName = resources.GetString("label14.AccessibleName");
      this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label14.Anchor")));
      this.label14.AutoSize = ((bool)(resources.GetObject("label14.AutoSize")));
      this.label14.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label14.Dock")));
      this.label14.Enabled = ((bool)(resources.GetObject("label14.Enabled")));
      this.label14.Font = ((System.Drawing.Font)(resources.GetObject("label14.Font")));
      this.label14.Image = ((System.Drawing.Image)(resources.GetObject("label14.Image")));
      this.label14.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label14.ImageAlign")));
      this.label14.ImageIndex = ((int)(resources.GetObject("label14.ImageIndex")));
      this.label14.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label14.ImeMode")));
      this.label14.Location = ((System.Drawing.Point)(resources.GetObject("label14.Location")));
      this.label14.Name = "label14";
      this.label14.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label14.RightToLeft")));
      this.label14.Size = ((System.Drawing.Size)(resources.GetObject("label14.Size")));
      this.label14.TabIndex = ((int)(resources.GetObject("label14.TabIndex")));
      this.label14.Text = resources.GetString("label14.Text");
      this.label14.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label14.TextAlign")));
      this.toolTip.SetToolTip(this.label14, resources.GetString("label14.ToolTip"));
      this.label14.Visible = ((bool)(resources.GetObject("label14.Visible")));
      // 
      // combo_clocking_prof_restorePresets
      // 
      this.combo_clocking_prof_restorePresets.AccessibleDescription = resources.GetString("combo_clocking_prof_restorePresets.AccessibleDescription");
      this.combo_clocking_prof_restorePresets.AccessibleName = resources.GetString("combo_clocking_prof_restorePresets.AccessibleName");
      this.combo_clocking_prof_restorePresets.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_clocking_prof_restorePresets.Anchor")));
      this.combo_clocking_prof_restorePresets.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_clocking_prof_restorePresets.BackgroundImage")));
      this.combo_clocking_prof_restorePresets.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_clocking_prof_restorePresets.Dock")));
      this.combo_clocking_prof_restorePresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo_clocking_prof_restorePresets.Enabled = ((bool)(resources.GetObject("combo_clocking_prof_restorePresets.Enabled")));
      this.combo_clocking_prof_restorePresets.Font = ((System.Drawing.Font)(resources.GetObject("combo_clocking_prof_restorePresets.Font")));
      this.combo_clocking_prof_restorePresets.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_clocking_prof_restorePresets.ImeMode")));
      this.combo_clocking_prof_restorePresets.IntegralHeight = ((bool)(resources.GetObject("combo_clocking_prof_restorePresets.IntegralHeight")));
      this.combo_clocking_prof_restorePresets.ItemHeight = ((int)(resources.GetObject("combo_clocking_prof_restorePresets.ItemHeight")));
      this.combo_clocking_prof_restorePresets.Items.AddRange(new object[] {
                                                                            resources.GetString("combo_clocking_prof_restorePresets.Items"),
                                                                            resources.GetString("combo_clocking_prof_restorePresets.Items1"),
                                                                            resources.GetString("combo_clocking_prof_restorePresets.Items2"),
                                                                            resources.GetString("combo_clocking_prof_restorePresets.Items3"),
                                                                            resources.GetString("combo_clocking_prof_restorePresets.Items4")});
      this.combo_clocking_prof_restorePresets.Location = ((System.Drawing.Point)(resources.GetObject("combo_clocking_prof_restorePresets.Location")));
      this.combo_clocking_prof_restorePresets.MaxDropDownItems = ((int)(resources.GetObject("combo_clocking_prof_restorePresets.MaxDropDownItems")));
      this.combo_clocking_prof_restorePresets.MaxLength = ((int)(resources.GetObject("combo_clocking_prof_restorePresets.MaxLength")));
      this.combo_clocking_prof_restorePresets.Name = "combo_clocking_prof_restorePresets";
      this.combo_clocking_prof_restorePresets.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_clocking_prof_restorePresets.RightToLeft")));
      this.combo_clocking_prof_restorePresets.Size = ((System.Drawing.Size)(resources.GetObject("combo_clocking_prof_restorePresets.Size")));
      this.combo_clocking_prof_restorePresets.TabIndex = ((int)(resources.GetObject("combo_clocking_prof_restorePresets.TabIndex")));
      this.combo_clocking_prof_restorePresets.Text = resources.GetString("combo_clocking_prof_restorePresets.Text");
      this.toolTip.SetToolTip(this.combo_clocking_prof_restorePresets, resources.GetString("combo_clocking_prof_restorePresets.ToolTip"));
      this.combo_clocking_prof_restorePresets.Visible = ((bool)(resources.GetObject("combo_clocking_prof_restorePresets.Visible")));
      // 
      // groupBox2
      // 
      this.groupBox2.AccessibleDescription = resources.GetString("groupBox2.AccessibleDescription");
      this.groupBox2.AccessibleName = resources.GetString("groupBox2.AccessibleName");
      this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox2.Anchor")));
      this.groupBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox2.BackgroundImage")));
      this.groupBox2.Controls.Add(this.num_clock_mem_max);
      this.groupBox2.Controls.Add(this.num_clock_mem_pre_ultra);
      this.groupBox2.Controls.Add(this.num_clock_mem_pre_fast);
      this.groupBox2.Controls.Add(this.num_clock_mem_pre_normal);
      this.groupBox2.Controls.Add(this.num_clock_mem_pre_slow);
      this.groupBox2.Controls.Add(this.num_clock_mem_min);
      this.groupBox2.Controls.Add(this.num_clock_chip_max);
      this.groupBox2.Controls.Add(this.num_clock_chip_pre_ultra);
      this.groupBox2.Controls.Add(this.num_clock_chip_pre_fast);
      this.groupBox2.Controls.Add(this.num_clock_chip_pre_normal);
      this.groupBox2.Controls.Add(this.num_clock_chip_pre_slow);
      this.groupBox2.Controls.Add(this.label10);
      this.groupBox2.Controls.Add(this.label11);
      this.groupBox2.Controls.Add(this.label8);
      this.groupBox2.Controls.Add(this.label9);
      this.groupBox2.Controls.Add(this.label13);
      this.groupBox2.Controls.Add(this.label12);
      this.groupBox2.Controls.Add(this.label4);
      this.groupBox2.Controls.Add(this.label5);
      this.groupBox2.Controls.Add(this.num_clock_chip_min);
      this.groupBox2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox2.Dock")));
      this.groupBox2.Enabled = ((bool)(resources.GetObject("groupBox2.Enabled")));
      this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.groupBox2.Font = ((System.Drawing.Font)(resources.GetObject("groupBox2.Font")));
      this.groupBox2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox2.ImeMode")));
      this.groupBox2.Location = ((System.Drawing.Point)(resources.GetObject("groupBox2.Location")));
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox2.RightToLeft")));
      this.groupBox2.Size = ((System.Drawing.Size)(resources.GetObject("groupBox2.Size")));
      this.groupBox2.TabIndex = ((int)(resources.GetObject("groupBox2.TabIndex")));
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = resources.GetString("groupBox2.Text");
      this.toolTip.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
      this.groupBox2.Visible = ((bool)(resources.GetObject("groupBox2.Visible")));
      // 
      // num_clock_mem_max
      // 
      this.num_clock_mem_max.AccessibleDescription = resources.GetString("num_clock_mem_max.AccessibleDescription");
      this.num_clock_mem_max.AccessibleName = resources.GetString("num_clock_mem_max.AccessibleName");
      this.num_clock_mem_max.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_mem_max.Anchor")));
      this.num_clock_mem_max.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(128)), ((System.Byte)(128)));
      this.num_clock_mem_max.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_mem_max.Dock")));
      this.num_clock_mem_max.Enabled = ((bool)(resources.GetObject("num_clock_mem_max.Enabled")));
      this.num_clock_mem_max.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_mem_max.Font")));
      this.num_clock_mem_max.ForeColor = System.Drawing.Color.Black;
      this.num_clock_mem_max.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_mem_max.ImeMode")));
      this.num_clock_mem_max.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_mem_max.Location")));
      this.num_clock_mem_max.Maximum = new System.Decimal(new int[] {
                                                                      999,
                                                                      0,
                                                                      0,
                                                                      0});
      this.num_clock_mem_max.Name = "num_clock_mem_max";
      this.num_clock_mem_max.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_mem_max.RightToLeft")));
      this.num_clock_mem_max.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_mem_max.Size")));
      this.num_clock_mem_max.TabIndex = ((int)(resources.GetObject("num_clock_mem_max.TabIndex")));
      this.num_clock_mem_max.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_mem_max.TextAlign")));
      this.num_clock_mem_max.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_mem_max.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_mem_max, resources.GetString("num_clock_mem_max.ToolTip"));
      this.num_clock_mem_max.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_mem_max.UpDownAlign")));
      this.num_clock_mem_max.Visible = ((bool)(resources.GetObject("num_clock_mem_max.Visible")));
      // 
      // num_clock_mem_pre_ultra
      // 
      this.num_clock_mem_pre_ultra.AccessibleDescription = resources.GetString("num_clock_mem_pre_ultra.AccessibleDescription");
      this.num_clock_mem_pre_ultra.AccessibleName = resources.GetString("num_clock_mem_pre_ultra.AccessibleName");
      this.num_clock_mem_pre_ultra.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_mem_pre_ultra.Anchor")));
      this.num_clock_mem_pre_ultra.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(192)), ((System.Byte)(128)));
      this.num_clock_mem_pre_ultra.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_mem_pre_ultra.Dock")));
      this.num_clock_mem_pre_ultra.Enabled = ((bool)(resources.GetObject("num_clock_mem_pre_ultra.Enabled")));
      this.num_clock_mem_pre_ultra.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_mem_pre_ultra.Font")));
      this.num_clock_mem_pre_ultra.ForeColor = System.Drawing.Color.Black;
      this.num_clock_mem_pre_ultra.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_mem_pre_ultra.ImeMode")));
      this.num_clock_mem_pre_ultra.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_mem_pre_ultra.Location")));
      this.num_clock_mem_pre_ultra.Maximum = new System.Decimal(new int[] {
                                                                            999,
                                                                            0,
                                                                            0,
                                                                            0});
      this.num_clock_mem_pre_ultra.Name = "num_clock_mem_pre_ultra";
      this.num_clock_mem_pre_ultra.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_mem_pre_ultra.RightToLeft")));
      this.num_clock_mem_pre_ultra.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_mem_pre_ultra.Size")));
      this.num_clock_mem_pre_ultra.TabIndex = ((int)(resources.GetObject("num_clock_mem_pre_ultra.TabIndex")));
      this.num_clock_mem_pre_ultra.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_mem_pre_ultra.TextAlign")));
      this.num_clock_mem_pre_ultra.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_mem_pre_ultra.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_mem_pre_ultra, resources.GetString("num_clock_mem_pre_ultra.ToolTip"));
      this.num_clock_mem_pre_ultra.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_mem_pre_ultra.UpDownAlign")));
      this.num_clock_mem_pre_ultra.Visible = ((bool)(resources.GetObject("num_clock_mem_pre_ultra.Visible")));
      // 
      // num_clock_mem_pre_fast
      // 
      this.num_clock_mem_pre_fast.AccessibleDescription = resources.GetString("num_clock_mem_pre_fast.AccessibleDescription");
      this.num_clock_mem_pre_fast.AccessibleName = resources.GetString("num_clock_mem_pre_fast.AccessibleName");
      this.num_clock_mem_pre_fast.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_mem_pre_fast.Anchor")));
      this.num_clock_mem_pre_fast.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(255)), ((System.Byte)(128)));
      this.num_clock_mem_pre_fast.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_mem_pre_fast.Dock")));
      this.num_clock_mem_pre_fast.Enabled = ((bool)(resources.GetObject("num_clock_mem_pre_fast.Enabled")));
      this.num_clock_mem_pre_fast.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_mem_pre_fast.Font")));
      this.num_clock_mem_pre_fast.ForeColor = System.Drawing.Color.Black;
      this.num_clock_mem_pre_fast.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_mem_pre_fast.ImeMode")));
      this.num_clock_mem_pre_fast.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_mem_pre_fast.Location")));
      this.num_clock_mem_pre_fast.Maximum = new System.Decimal(new int[] {
                                                                           999,
                                                                           0,
                                                                           0,
                                                                           0});
      this.num_clock_mem_pre_fast.Name = "num_clock_mem_pre_fast";
      this.num_clock_mem_pre_fast.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_mem_pre_fast.RightToLeft")));
      this.num_clock_mem_pre_fast.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_mem_pre_fast.Size")));
      this.num_clock_mem_pre_fast.TabIndex = ((int)(resources.GetObject("num_clock_mem_pre_fast.TabIndex")));
      this.num_clock_mem_pre_fast.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_mem_pre_fast.TextAlign")));
      this.num_clock_mem_pre_fast.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_mem_pre_fast.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_mem_pre_fast, resources.GetString("num_clock_mem_pre_fast.ToolTip"));
      this.num_clock_mem_pre_fast.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_mem_pre_fast.UpDownAlign")));
      this.num_clock_mem_pre_fast.Visible = ((bool)(resources.GetObject("num_clock_mem_pre_fast.Visible")));
      // 
      // num_clock_mem_pre_normal
      // 
      this.num_clock_mem_pre_normal.AccessibleDescription = resources.GetString("num_clock_mem_pre_normal.AccessibleDescription");
      this.num_clock_mem_pre_normal.AccessibleName = resources.GetString("num_clock_mem_pre_normal.AccessibleName");
      this.num_clock_mem_pre_normal.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_mem_pre_normal.Anchor")));
      this.num_clock_mem_pre_normal.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(128)), ((System.Byte)(255)), ((System.Byte)(128)));
      this.num_clock_mem_pre_normal.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_mem_pre_normal.Dock")));
      this.num_clock_mem_pre_normal.Enabled = ((bool)(resources.GetObject("num_clock_mem_pre_normal.Enabled")));
      this.num_clock_mem_pre_normal.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_mem_pre_normal.Font")));
      this.num_clock_mem_pre_normal.ForeColor = System.Drawing.Color.Black;
      this.num_clock_mem_pre_normal.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_mem_pre_normal.ImeMode")));
      this.num_clock_mem_pre_normal.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_mem_pre_normal.Location")));
      this.num_clock_mem_pre_normal.Maximum = new System.Decimal(new int[] {
                                                                             999,
                                                                             0,
                                                                             0,
                                                                             0});
      this.num_clock_mem_pre_normal.Name = "num_clock_mem_pre_normal";
      this.num_clock_mem_pre_normal.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_mem_pre_normal.RightToLeft")));
      this.num_clock_mem_pre_normal.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_mem_pre_normal.Size")));
      this.num_clock_mem_pre_normal.TabIndex = ((int)(resources.GetObject("num_clock_mem_pre_normal.TabIndex")));
      this.num_clock_mem_pre_normal.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_mem_pre_normal.TextAlign")));
      this.num_clock_mem_pre_normal.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_mem_pre_normal.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_mem_pre_normal, resources.GetString("num_clock_mem_pre_normal.ToolTip"));
      this.num_clock_mem_pre_normal.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_mem_pre_normal.UpDownAlign")));
      this.num_clock_mem_pre_normal.Visible = ((bool)(resources.GetObject("num_clock_mem_pre_normal.Visible")));
      // 
      // num_clock_mem_pre_slow
      // 
      this.num_clock_mem_pre_slow.AccessibleDescription = resources.GetString("num_clock_mem_pre_slow.AccessibleDescription");
      this.num_clock_mem_pre_slow.AccessibleName = resources.GetString("num_clock_mem_pre_slow.AccessibleName");
      this.num_clock_mem_pre_slow.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_mem_pre_slow.Anchor")));
      this.num_clock_mem_pre_slow.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(128)), ((System.Byte)(255)), ((System.Byte)(255)));
      this.num_clock_mem_pre_slow.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_mem_pre_slow.Dock")));
      this.num_clock_mem_pre_slow.Enabled = ((bool)(resources.GetObject("num_clock_mem_pre_slow.Enabled")));
      this.num_clock_mem_pre_slow.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_mem_pre_slow.Font")));
      this.num_clock_mem_pre_slow.ForeColor = System.Drawing.Color.Black;
      this.num_clock_mem_pre_slow.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_mem_pre_slow.ImeMode")));
      this.num_clock_mem_pre_slow.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_mem_pre_slow.Location")));
      this.num_clock_mem_pre_slow.Maximum = new System.Decimal(new int[] {
                                                                           999,
                                                                           0,
                                                                           0,
                                                                           0});
      this.num_clock_mem_pre_slow.Name = "num_clock_mem_pre_slow";
      this.num_clock_mem_pre_slow.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_mem_pre_slow.RightToLeft")));
      this.num_clock_mem_pre_slow.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_mem_pre_slow.Size")));
      this.num_clock_mem_pre_slow.TabIndex = ((int)(resources.GetObject("num_clock_mem_pre_slow.TabIndex")));
      this.num_clock_mem_pre_slow.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_mem_pre_slow.TextAlign")));
      this.num_clock_mem_pre_slow.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_mem_pre_slow.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_mem_pre_slow, resources.GetString("num_clock_mem_pre_slow.ToolTip"));
      this.num_clock_mem_pre_slow.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_mem_pre_slow.UpDownAlign")));
      this.num_clock_mem_pre_slow.Visible = ((bool)(resources.GetObject("num_clock_mem_pre_slow.Visible")));
      // 
      // num_clock_mem_min
      // 
      this.num_clock_mem_min.AccessibleDescription = resources.GetString("num_clock_mem_min.AccessibleDescription");
      this.num_clock_mem_min.AccessibleName = resources.GetString("num_clock_mem_min.AccessibleName");
      this.num_clock_mem_min.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_mem_min.Anchor")));
      this.num_clock_mem_min.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(192)), ((System.Byte)(255)));
      this.num_clock_mem_min.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_mem_min.Dock")));
      this.num_clock_mem_min.Enabled = ((bool)(resources.GetObject("num_clock_mem_min.Enabled")));
      this.num_clock_mem_min.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_mem_min.Font")));
      this.num_clock_mem_min.ForeColor = System.Drawing.Color.Black;
      this.num_clock_mem_min.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_mem_min.ImeMode")));
      this.num_clock_mem_min.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_mem_min.Location")));
      this.num_clock_mem_min.Maximum = new System.Decimal(new int[] {
                                                                      999,
                                                                      0,
                                                                      0,
                                                                      0});
      this.num_clock_mem_min.Name = "num_clock_mem_min";
      this.num_clock_mem_min.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_mem_min.RightToLeft")));
      this.num_clock_mem_min.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_mem_min.Size")));
      this.num_clock_mem_min.TabIndex = ((int)(resources.GetObject("num_clock_mem_min.TabIndex")));
      this.num_clock_mem_min.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_mem_min.TextAlign")));
      this.num_clock_mem_min.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_mem_min.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_mem_min, resources.GetString("num_clock_mem_min.ToolTip"));
      this.num_clock_mem_min.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_mem_min.UpDownAlign")));
      this.num_clock_mem_min.Visible = ((bool)(resources.GetObject("num_clock_mem_min.Visible")));
      // 
      // num_clock_chip_max
      // 
      this.num_clock_chip_max.AccessibleDescription = resources.GetString("num_clock_chip_max.AccessibleDescription");
      this.num_clock_chip_max.AccessibleName = resources.GetString("num_clock_chip_max.AccessibleName");
      this.num_clock_chip_max.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_chip_max.Anchor")));
      this.num_clock_chip_max.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(128)), ((System.Byte)(128)));
      this.num_clock_chip_max.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_chip_max.Dock")));
      this.num_clock_chip_max.Enabled = ((bool)(resources.GetObject("num_clock_chip_max.Enabled")));
      this.num_clock_chip_max.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_chip_max.Font")));
      this.num_clock_chip_max.ForeColor = System.Drawing.Color.Black;
      this.num_clock_chip_max.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_chip_max.ImeMode")));
      this.num_clock_chip_max.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_chip_max.Location")));
      this.num_clock_chip_max.Maximum = new System.Decimal(new int[] {
                                                                       999,
                                                                       0,
                                                                       0,
                                                                       0});
      this.num_clock_chip_max.Name = "num_clock_chip_max";
      this.num_clock_chip_max.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_chip_max.RightToLeft")));
      this.num_clock_chip_max.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_chip_max.Size")));
      this.num_clock_chip_max.TabIndex = ((int)(resources.GetObject("num_clock_chip_max.TabIndex")));
      this.num_clock_chip_max.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_chip_max.TextAlign")));
      this.num_clock_chip_max.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_chip_max.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_chip_max, resources.GetString("num_clock_chip_max.ToolTip"));
      this.num_clock_chip_max.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_chip_max.UpDownAlign")));
      this.num_clock_chip_max.Visible = ((bool)(resources.GetObject("num_clock_chip_max.Visible")));
      // 
      // num_clock_chip_pre_ultra
      // 
      this.num_clock_chip_pre_ultra.AccessibleDescription = resources.GetString("num_clock_chip_pre_ultra.AccessibleDescription");
      this.num_clock_chip_pre_ultra.AccessibleName = resources.GetString("num_clock_chip_pre_ultra.AccessibleName");
      this.num_clock_chip_pre_ultra.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_chip_pre_ultra.Anchor")));
      this.num_clock_chip_pre_ultra.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(192)), ((System.Byte)(128)));
      this.num_clock_chip_pre_ultra.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_chip_pre_ultra.Dock")));
      this.num_clock_chip_pre_ultra.Enabled = ((bool)(resources.GetObject("num_clock_chip_pre_ultra.Enabled")));
      this.num_clock_chip_pre_ultra.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_chip_pre_ultra.Font")));
      this.num_clock_chip_pre_ultra.ForeColor = System.Drawing.Color.Black;
      this.num_clock_chip_pre_ultra.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_chip_pre_ultra.ImeMode")));
      this.num_clock_chip_pre_ultra.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_chip_pre_ultra.Location")));
      this.num_clock_chip_pre_ultra.Maximum = new System.Decimal(new int[] {
                                                                             999,
                                                                             0,
                                                                             0,
                                                                             0});
      this.num_clock_chip_pre_ultra.Name = "num_clock_chip_pre_ultra";
      this.num_clock_chip_pre_ultra.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_chip_pre_ultra.RightToLeft")));
      this.num_clock_chip_pre_ultra.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_chip_pre_ultra.Size")));
      this.num_clock_chip_pre_ultra.TabIndex = ((int)(resources.GetObject("num_clock_chip_pre_ultra.TabIndex")));
      this.num_clock_chip_pre_ultra.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_chip_pre_ultra.TextAlign")));
      this.num_clock_chip_pre_ultra.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_chip_pre_ultra.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_chip_pre_ultra, resources.GetString("num_clock_chip_pre_ultra.ToolTip"));
      this.num_clock_chip_pre_ultra.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_chip_pre_ultra.UpDownAlign")));
      this.num_clock_chip_pre_ultra.Visible = ((bool)(resources.GetObject("num_clock_chip_pre_ultra.Visible")));
      // 
      // num_clock_chip_pre_fast
      // 
      this.num_clock_chip_pre_fast.AccessibleDescription = resources.GetString("num_clock_chip_pre_fast.AccessibleDescription");
      this.num_clock_chip_pre_fast.AccessibleName = resources.GetString("num_clock_chip_pre_fast.AccessibleName");
      this.num_clock_chip_pre_fast.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_chip_pre_fast.Anchor")));
      this.num_clock_chip_pre_fast.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(255)), ((System.Byte)(128)));
      this.num_clock_chip_pre_fast.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_chip_pre_fast.Dock")));
      this.num_clock_chip_pre_fast.Enabled = ((bool)(resources.GetObject("num_clock_chip_pre_fast.Enabled")));
      this.num_clock_chip_pre_fast.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_chip_pre_fast.Font")));
      this.num_clock_chip_pre_fast.ForeColor = System.Drawing.Color.Black;
      this.num_clock_chip_pre_fast.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_chip_pre_fast.ImeMode")));
      this.num_clock_chip_pre_fast.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_chip_pre_fast.Location")));
      this.num_clock_chip_pre_fast.Maximum = new System.Decimal(new int[] {
                                                                            999,
                                                                            0,
                                                                            0,
                                                                            0});
      this.num_clock_chip_pre_fast.Name = "num_clock_chip_pre_fast";
      this.num_clock_chip_pre_fast.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_chip_pre_fast.RightToLeft")));
      this.num_clock_chip_pre_fast.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_chip_pre_fast.Size")));
      this.num_clock_chip_pre_fast.TabIndex = ((int)(resources.GetObject("num_clock_chip_pre_fast.TabIndex")));
      this.num_clock_chip_pre_fast.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_chip_pre_fast.TextAlign")));
      this.num_clock_chip_pre_fast.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_chip_pre_fast.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_chip_pre_fast, resources.GetString("num_clock_chip_pre_fast.ToolTip"));
      this.num_clock_chip_pre_fast.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_chip_pre_fast.UpDownAlign")));
      this.num_clock_chip_pre_fast.Visible = ((bool)(resources.GetObject("num_clock_chip_pre_fast.Visible")));
      // 
      // num_clock_chip_pre_normal
      // 
      this.num_clock_chip_pre_normal.AccessibleDescription = resources.GetString("num_clock_chip_pre_normal.AccessibleDescription");
      this.num_clock_chip_pre_normal.AccessibleName = resources.GetString("num_clock_chip_pre_normal.AccessibleName");
      this.num_clock_chip_pre_normal.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_chip_pre_normal.Anchor")));
      this.num_clock_chip_pre_normal.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(128)), ((System.Byte)(255)), ((System.Byte)(128)));
      this.num_clock_chip_pre_normal.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_chip_pre_normal.Dock")));
      this.num_clock_chip_pre_normal.Enabled = ((bool)(resources.GetObject("num_clock_chip_pre_normal.Enabled")));
      this.num_clock_chip_pre_normal.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_chip_pre_normal.Font")));
      this.num_clock_chip_pre_normal.ForeColor = System.Drawing.Color.Black;
      this.num_clock_chip_pre_normal.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_chip_pre_normal.ImeMode")));
      this.num_clock_chip_pre_normal.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_chip_pre_normal.Location")));
      this.num_clock_chip_pre_normal.Maximum = new System.Decimal(new int[] {
                                                                              999,
                                                                              0,
                                                                              0,
                                                                              0});
      this.num_clock_chip_pre_normal.Name = "num_clock_chip_pre_normal";
      this.num_clock_chip_pre_normal.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_chip_pre_normal.RightToLeft")));
      this.num_clock_chip_pre_normal.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_chip_pre_normal.Size")));
      this.num_clock_chip_pre_normal.TabIndex = ((int)(resources.GetObject("num_clock_chip_pre_normal.TabIndex")));
      this.num_clock_chip_pre_normal.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_chip_pre_normal.TextAlign")));
      this.num_clock_chip_pre_normal.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_chip_pre_normal.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_chip_pre_normal, resources.GetString("num_clock_chip_pre_normal.ToolTip"));
      this.num_clock_chip_pre_normal.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_chip_pre_normal.UpDownAlign")));
      this.num_clock_chip_pre_normal.Visible = ((bool)(resources.GetObject("num_clock_chip_pre_normal.Visible")));
      // 
      // num_clock_chip_pre_slow
      // 
      this.num_clock_chip_pre_slow.AccessibleDescription = resources.GetString("num_clock_chip_pre_slow.AccessibleDescription");
      this.num_clock_chip_pre_slow.AccessibleName = resources.GetString("num_clock_chip_pre_slow.AccessibleName");
      this.num_clock_chip_pre_slow.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_chip_pre_slow.Anchor")));
      this.num_clock_chip_pre_slow.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(128)), ((System.Byte)(255)), ((System.Byte)(255)));
      this.num_clock_chip_pre_slow.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_chip_pre_slow.Dock")));
      this.num_clock_chip_pre_slow.Enabled = ((bool)(resources.GetObject("num_clock_chip_pre_slow.Enabled")));
      this.num_clock_chip_pre_slow.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_chip_pre_slow.Font")));
      this.num_clock_chip_pre_slow.ForeColor = System.Drawing.Color.Black;
      this.num_clock_chip_pre_slow.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_chip_pre_slow.ImeMode")));
      this.num_clock_chip_pre_slow.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_chip_pre_slow.Location")));
      this.num_clock_chip_pre_slow.Maximum = new System.Decimal(new int[] {
                                                                            999,
                                                                            0,
                                                                            0,
                                                                            0});
      this.num_clock_chip_pre_slow.Name = "num_clock_chip_pre_slow";
      this.num_clock_chip_pre_slow.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_chip_pre_slow.RightToLeft")));
      this.num_clock_chip_pre_slow.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_chip_pre_slow.Size")));
      this.num_clock_chip_pre_slow.TabIndex = ((int)(resources.GetObject("num_clock_chip_pre_slow.TabIndex")));
      this.num_clock_chip_pre_slow.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_chip_pre_slow.TextAlign")));
      this.num_clock_chip_pre_slow.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_chip_pre_slow.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_chip_pre_slow, resources.GetString("num_clock_chip_pre_slow.ToolTip"));
      this.num_clock_chip_pre_slow.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_chip_pre_slow.UpDownAlign")));
      this.num_clock_chip_pre_slow.Visible = ((bool)(resources.GetObject("num_clock_chip_pre_slow.Visible")));
      // 
      // label10
      // 
      this.label10.AccessibleDescription = resources.GetString("label10.AccessibleDescription");
      this.label10.AccessibleName = resources.GetString("label10.AccessibleName");
      this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label10.Anchor")));
      this.label10.AutoSize = ((bool)(resources.GetObject("label10.AutoSize")));
      this.label10.CausesValidation = false;
      this.label10.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label10.Dock")));
      this.label10.Enabled = ((bool)(resources.GetObject("label10.Enabled")));
      this.label10.Font = ((System.Drawing.Font)(resources.GetObject("label10.Font")));
      this.label10.Image = ((System.Drawing.Image)(resources.GetObject("label10.Image")));
      this.label10.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label10.ImageAlign")));
      this.label10.ImageIndex = ((int)(resources.GetObject("label10.ImageIndex")));
      this.label10.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label10.ImeMode")));
      this.label10.Location = ((System.Drawing.Point)(resources.GetObject("label10.Location")));
      this.label10.Name = "label10";
      this.label10.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label10.RightToLeft")));
      this.label10.Size = ((System.Drawing.Size)(resources.GetObject("label10.Size")));
      this.label10.TabIndex = ((int)(resources.GetObject("label10.TabIndex")));
      this.label10.Text = resources.GetString("label10.Text");
      this.label10.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label10.TextAlign")));
      this.toolTip.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
      this.label10.Visible = ((bool)(resources.GetObject("label10.Visible")));
      // 
      // label11
      // 
      this.label11.AccessibleDescription = resources.GetString("label11.AccessibleDescription");
      this.label11.AccessibleName = resources.GetString("label11.AccessibleName");
      this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label11.Anchor")));
      this.label11.AutoSize = ((bool)(resources.GetObject("label11.AutoSize")));
      this.label11.CausesValidation = false;
      this.label11.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label11.Dock")));
      this.label11.Enabled = ((bool)(resources.GetObject("label11.Enabled")));
      this.label11.Font = ((System.Drawing.Font)(resources.GetObject("label11.Font")));
      this.label11.Image = ((System.Drawing.Image)(resources.GetObject("label11.Image")));
      this.label11.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label11.ImageAlign")));
      this.label11.ImageIndex = ((int)(resources.GetObject("label11.ImageIndex")));
      this.label11.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label11.ImeMode")));
      this.label11.Location = ((System.Drawing.Point)(resources.GetObject("label11.Location")));
      this.label11.Name = "label11";
      this.label11.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label11.RightToLeft")));
      this.label11.Size = ((System.Drawing.Size)(resources.GetObject("label11.Size")));
      this.label11.TabIndex = ((int)(resources.GetObject("label11.TabIndex")));
      this.label11.Text = resources.GetString("label11.Text");
      this.label11.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label11.TextAlign")));
      this.toolTip.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
      this.label11.Visible = ((bool)(resources.GetObject("label11.Visible")));
      // 
      // label8
      // 
      this.label8.AccessibleDescription = resources.GetString("label8.AccessibleDescription");
      this.label8.AccessibleName = resources.GetString("label8.AccessibleName");
      this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label8.Anchor")));
      this.label8.AutoSize = ((bool)(resources.GetObject("label8.AutoSize")));
      this.label8.CausesValidation = false;
      this.label8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label8.Dock")));
      this.label8.Enabled = ((bool)(resources.GetObject("label8.Enabled")));
      this.label8.Font = ((System.Drawing.Font)(resources.GetObject("label8.Font")));
      this.label8.Image = ((System.Drawing.Image)(resources.GetObject("label8.Image")));
      this.label8.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label8.ImageAlign")));
      this.label8.ImageIndex = ((int)(resources.GetObject("label8.ImageIndex")));
      this.label8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label8.ImeMode")));
      this.label8.Location = ((System.Drawing.Point)(resources.GetObject("label8.Location")));
      this.label8.Name = "label8";
      this.label8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label8.RightToLeft")));
      this.label8.Size = ((System.Drawing.Size)(resources.GetObject("label8.Size")));
      this.label8.TabIndex = ((int)(resources.GetObject("label8.TabIndex")));
      this.label8.Text = resources.GetString("label8.Text");
      this.label8.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label8.TextAlign")));
      this.toolTip.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
      this.label8.Visible = ((bool)(resources.GetObject("label8.Visible")));
      // 
      // label9
      // 
      this.label9.AccessibleDescription = resources.GetString("label9.AccessibleDescription");
      this.label9.AccessibleName = resources.GetString("label9.AccessibleName");
      this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label9.Anchor")));
      this.label9.AutoSize = ((bool)(resources.GetObject("label9.AutoSize")));
      this.label9.CausesValidation = false;
      this.label9.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label9.Dock")));
      this.label9.Enabled = ((bool)(resources.GetObject("label9.Enabled")));
      this.label9.Font = ((System.Drawing.Font)(resources.GetObject("label9.Font")));
      this.label9.Image = ((System.Drawing.Image)(resources.GetObject("label9.Image")));
      this.label9.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label9.ImageAlign")));
      this.label9.ImageIndex = ((int)(resources.GetObject("label9.ImageIndex")));
      this.label9.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label9.ImeMode")));
      this.label9.Location = ((System.Drawing.Point)(resources.GetObject("label9.Location")));
      this.label9.Name = "label9";
      this.label9.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label9.RightToLeft")));
      this.label9.Size = ((System.Drawing.Size)(resources.GetObject("label9.Size")));
      this.label9.TabIndex = ((int)(resources.GetObject("label9.TabIndex")));
      this.label9.Text = resources.GetString("label9.Text");
      this.label9.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label9.TextAlign")));
      this.toolTip.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
      this.label9.Visible = ((bool)(resources.GetObject("label9.Visible")));
      // 
      // label13
      // 
      this.label13.AccessibleDescription = resources.GetString("label13.AccessibleDescription");
      this.label13.AccessibleName = resources.GetString("label13.AccessibleName");
      this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label13.Anchor")));
      this.label13.AutoSize = ((bool)(resources.GetObject("label13.AutoSize")));
      this.label13.CausesValidation = false;
      this.label13.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label13.Dock")));
      this.label13.Enabled = ((bool)(resources.GetObject("label13.Enabled")));
      this.label13.Font = ((System.Drawing.Font)(resources.GetObject("label13.Font")));
      this.label13.Image = ((System.Drawing.Image)(resources.GetObject("label13.Image")));
      this.label13.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label13.ImageAlign")));
      this.label13.ImageIndex = ((int)(resources.GetObject("label13.ImageIndex")));
      this.label13.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label13.ImeMode")));
      this.label13.Location = ((System.Drawing.Point)(resources.GetObject("label13.Location")));
      this.label13.Name = "label13";
      this.label13.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label13.RightToLeft")));
      this.label13.Size = ((System.Drawing.Size)(resources.GetObject("label13.Size")));
      this.label13.TabIndex = ((int)(resources.GetObject("label13.TabIndex")));
      this.label13.Text = resources.GetString("label13.Text");
      this.label13.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label13.TextAlign")));
      this.toolTip.SetToolTip(this.label13, resources.GetString("label13.ToolTip"));
      this.label13.Visible = ((bool)(resources.GetObject("label13.Visible")));
      // 
      // label12
      // 
      this.label12.AccessibleDescription = resources.GetString("label12.AccessibleDescription");
      this.label12.AccessibleName = resources.GetString("label12.AccessibleName");
      this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label12.Anchor")));
      this.label12.AutoSize = ((bool)(resources.GetObject("label12.AutoSize")));
      this.label12.CausesValidation = false;
      this.label12.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label12.Dock")));
      this.label12.Enabled = ((bool)(resources.GetObject("label12.Enabled")));
      this.label12.Font = ((System.Drawing.Font)(resources.GetObject("label12.Font")));
      this.label12.Image = ((System.Drawing.Image)(resources.GetObject("label12.Image")));
      this.label12.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label12.ImageAlign")));
      this.label12.ImageIndex = ((int)(resources.GetObject("label12.ImageIndex")));
      this.label12.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label12.ImeMode")));
      this.label12.Location = ((System.Drawing.Point)(resources.GetObject("label12.Location")));
      this.label12.Name = "label12";
      this.label12.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label12.RightToLeft")));
      this.label12.Size = ((System.Drawing.Size)(resources.GetObject("label12.Size")));
      this.label12.TabIndex = ((int)(resources.GetObject("label12.TabIndex")));
      this.label12.Text = resources.GetString("label12.Text");
      this.label12.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label12.TextAlign")));
      this.toolTip.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
      this.label12.Visible = ((bool)(resources.GetObject("label12.Visible")));
      // 
      // label4
      // 
      this.label4.AccessibleDescription = resources.GetString("label4.AccessibleDescription");
      this.label4.AccessibleName = resources.GetString("label4.AccessibleName");
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label4.Anchor")));
      this.label4.AutoSize = ((bool)(resources.GetObject("label4.AutoSize")));
      this.label4.CausesValidation = false;
      this.label4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label4.Dock")));
      this.label4.Enabled = ((bool)(resources.GetObject("label4.Enabled")));
      this.label4.Font = ((System.Drawing.Font)(resources.GetObject("label4.Font")));
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
      // label5
      // 
      this.label5.AccessibleDescription = resources.GetString("label5.AccessibleDescription");
      this.label5.AccessibleName = resources.GetString("label5.AccessibleName");
      this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label5.Anchor")));
      this.label5.AutoSize = ((bool)(resources.GetObject("label5.AutoSize")));
      this.label5.CausesValidation = false;
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
      // num_clock_chip_min
      // 
      this.num_clock_chip_min.AccessibleDescription = resources.GetString("num_clock_chip_min.AccessibleDescription");
      this.num_clock_chip_min.AccessibleName = resources.GetString("num_clock_chip_min.AccessibleName");
      this.num_clock_chip_min.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("num_clock_chip_min.Anchor")));
      this.num_clock_chip_min.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(192)), ((System.Byte)(255)));
      this.num_clock_chip_min.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("num_clock_chip_min.Dock")));
      this.num_clock_chip_min.Enabled = ((bool)(resources.GetObject("num_clock_chip_min.Enabled")));
      this.num_clock_chip_min.Font = ((System.Drawing.Font)(resources.GetObject("num_clock_chip_min.Font")));
      this.num_clock_chip_min.ForeColor = System.Drawing.Color.Black;
      this.num_clock_chip_min.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("num_clock_chip_min.ImeMode")));
      this.num_clock_chip_min.Location = ((System.Drawing.Point)(resources.GetObject("num_clock_chip_min.Location")));
      this.num_clock_chip_min.Maximum = new System.Decimal(new int[] {
                                                                       999,
                                                                       0,
                                                                       0,
                                                                       0});
      this.num_clock_chip_min.Name = "num_clock_chip_min";
      this.num_clock_chip_min.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("num_clock_chip_min.RightToLeft")));
      this.num_clock_chip_min.Size = ((System.Drawing.Size)(resources.GetObject("num_clock_chip_min.Size")));
      this.num_clock_chip_min.TabIndex = ((int)(resources.GetObject("num_clock_chip_min.TabIndex")));
      this.num_clock_chip_min.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("num_clock_chip_min.TextAlign")));
      this.num_clock_chip_min.ThousandsSeparator = ((bool)(resources.GetObject("num_clock_chip_min.ThousandsSeparator")));
      this.toolTip.SetToolTip(this.num_clock_chip_min, resources.GetString("num_clock_chip_min.ToolTip"));
      this.num_clock_chip_min.UpDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("num_clock_chip_min.UpDownAlign")));
      this.num_clock_chip_min.Visible = ((bool)(resources.GetObject("num_clock_chip_min.Visible")));
      // 
      // tab_extended
      // 
      this.tab_extended.AccessibleDescription = resources.GetString("tab_extended.AccessibleDescription");
      this.tab_extended.AccessibleName = resources.GetString("tab_extended.AccessibleName");
      this.tab_extended.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tab_extended.Anchor")));
      this.tab_extended.AutoScroll = ((bool)(resources.GetObject("tab_extended.AutoScroll")));
      this.tab_extended.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tab_extended.AutoScrollMargin")));
      this.tab_extended.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tab_extended.AutoScrollMinSize")));
      this.tab_extended.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tab_extended.BackgroundImage")));
      this.tab_extended.Controls.Add(this.check_clock_enableDLL);
      this.tab_extended.Controls.Add(this.check_clock_atiSmallSteps);
      this.tab_extended.Controls.Add(this.check_gui_mover_feedback);
      this.tab_extended.Controls.Add(this.check_features_enableClocking);
      this.tab_extended.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tab_extended.Dock")));
      this.tab_extended.Enabled = ((bool)(resources.GetObject("tab_extended.Enabled")));
      this.tab_extended.Font = ((System.Drawing.Font)(resources.GetObject("tab_extended.Font")));
      this.tab_extended.ImageIndex = ((int)(resources.GetObject("tab_extended.ImageIndex")));
      this.tab_extended.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tab_extended.ImeMode")));
      this.tab_extended.Location = ((System.Drawing.Point)(resources.GetObject("tab_extended.Location")));
      this.tab_extended.Name = "tab_extended";
      this.tab_extended.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tab_extended.RightToLeft")));
      this.tab_extended.Size = ((System.Drawing.Size)(resources.GetObject("tab_extended.Size")));
      this.tab_extended.TabIndex = ((int)(resources.GetObject("tab_extended.TabIndex")));
      this.tab_extended.Text = resources.GetString("tab_extended.Text");
      this.toolTip.SetToolTip(this.tab_extended, resources.GetString("tab_extended.ToolTip"));
      this.tab_extended.ToolTipText = resources.GetString("tab_extended.ToolTipText");
      this.tab_extended.Visible = ((bool)(resources.GetObject("tab_extended.Visible")));
      // 
      // check_clock_enableDLL
      // 
      this.check_clock_enableDLL.AccessibleDescription = resources.GetString("check_clock_enableDLL.AccessibleDescription");
      this.check_clock_enableDLL.AccessibleName = resources.GetString("check_clock_enableDLL.AccessibleName");
      this.check_clock_enableDLL.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_clock_enableDLL.Anchor")));
      this.check_clock_enableDLL.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_clock_enableDLL.Appearance")));
      this.check_clock_enableDLL.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_clock_enableDLL.BackgroundImage")));
      this.check_clock_enableDLL.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clock_enableDLL.CheckAlign")));
      this.check_clock_enableDLL.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_clock_enableDLL.Dock")));
      this.check_clock_enableDLL.Enabled = ((bool)(resources.GetObject("check_clock_enableDLL.Enabled")));
      this.check_clock_enableDLL.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_clock_enableDLL.FlatStyle")));
      this.check_clock_enableDLL.Font = ((System.Drawing.Font)(resources.GetObject("check_clock_enableDLL.Font")));
      this.check_clock_enableDLL.Image = ((System.Drawing.Image)(resources.GetObject("check_clock_enableDLL.Image")));
      this.check_clock_enableDLL.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clock_enableDLL.ImageAlign")));
      this.check_clock_enableDLL.ImageIndex = ((int)(resources.GetObject("check_clock_enableDLL.ImageIndex")));
      this.check_clock_enableDLL.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_clock_enableDLL.ImeMode")));
      this.check_clock_enableDLL.Location = ((System.Drawing.Point)(resources.GetObject("check_clock_enableDLL.Location")));
      this.check_clock_enableDLL.Name = "check_clock_enableDLL";
      this.check_clock_enableDLL.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_clock_enableDLL.RightToLeft")));
      this.check_clock_enableDLL.Size = ((System.Drawing.Size)(resources.GetObject("check_clock_enableDLL.Size")));
      this.check_clock_enableDLL.TabIndex = ((int)(resources.GetObject("check_clock_enableDLL.TabIndex")));
      this.check_clock_enableDLL.Text = resources.GetString("check_clock_enableDLL.Text");
      this.check_clock_enableDLL.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clock_enableDLL.TextAlign")));
      this.toolTip.SetToolTip(this.check_clock_enableDLL, resources.GetString("check_clock_enableDLL.ToolTip"));
      this.check_clock_enableDLL.Visible = ((bool)(resources.GetObject("check_clock_enableDLL.Visible")));
      // 
      // check_clock_atiSmallSteps
      // 
      this.check_clock_atiSmallSteps.AccessibleDescription = resources.GetString("check_clock_atiSmallSteps.AccessibleDescription");
      this.check_clock_atiSmallSteps.AccessibleName = resources.GetString("check_clock_atiSmallSteps.AccessibleName");
      this.check_clock_atiSmallSteps.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_clock_atiSmallSteps.Anchor")));
      this.check_clock_atiSmallSteps.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_clock_atiSmallSteps.Appearance")));
      this.check_clock_atiSmallSteps.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_clock_atiSmallSteps.BackgroundImage")));
      this.check_clock_atiSmallSteps.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clock_atiSmallSteps.CheckAlign")));
      this.check_clock_atiSmallSteps.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_clock_atiSmallSteps.Dock")));
      this.check_clock_atiSmallSteps.Enabled = ((bool)(resources.GetObject("check_clock_atiSmallSteps.Enabled")));
      this.check_clock_atiSmallSteps.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_clock_atiSmallSteps.FlatStyle")));
      this.check_clock_atiSmallSteps.Font = ((System.Drawing.Font)(resources.GetObject("check_clock_atiSmallSteps.Font")));
      this.check_clock_atiSmallSteps.Image = ((System.Drawing.Image)(resources.GetObject("check_clock_atiSmallSteps.Image")));
      this.check_clock_atiSmallSteps.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clock_atiSmallSteps.ImageAlign")));
      this.check_clock_atiSmallSteps.ImageIndex = ((int)(resources.GetObject("check_clock_atiSmallSteps.ImageIndex")));
      this.check_clock_atiSmallSteps.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_clock_atiSmallSteps.ImeMode")));
      this.check_clock_atiSmallSteps.Location = ((System.Drawing.Point)(resources.GetObject("check_clock_atiSmallSteps.Location")));
      this.check_clock_atiSmallSteps.Name = "check_clock_atiSmallSteps";
      this.check_clock_atiSmallSteps.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_clock_atiSmallSteps.RightToLeft")));
      this.check_clock_atiSmallSteps.Size = ((System.Drawing.Size)(resources.GetObject("check_clock_atiSmallSteps.Size")));
      this.check_clock_atiSmallSteps.TabIndex = ((int)(resources.GetObject("check_clock_atiSmallSteps.TabIndex")));
      this.check_clock_atiSmallSteps.Text = resources.GetString("check_clock_atiSmallSteps.Text");
      this.check_clock_atiSmallSteps.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_clock_atiSmallSteps.TextAlign")));
      this.toolTip.SetToolTip(this.check_clock_atiSmallSteps, resources.GetString("check_clock_atiSmallSteps.ToolTip"));
      this.check_clock_atiSmallSteps.Visible = ((bool)(resources.GetObject("check_clock_atiSmallSteps.Visible")));
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
      // combo_performanceProfile
      // 
      this.combo_performanceProfile.AccessibleDescription = resources.GetString("combo_performanceProfile.AccessibleDescription");
      this.combo_performanceProfile.AccessibleName = resources.GetString("combo_performanceProfile.AccessibleName");
      this.combo_performanceProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_performanceProfile.Anchor")));
      this.combo_performanceProfile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_performanceProfile.BackgroundImage")));
      this.combo_performanceProfile.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_performanceProfile.Dock")));
      this.combo_performanceProfile.Enabled = ((bool)(resources.GetObject("combo_performanceProfile.Enabled")));
      this.combo_performanceProfile.Font = ((System.Drawing.Font)(resources.GetObject("combo_performanceProfile.Font")));
      this.combo_performanceProfile.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_performanceProfile.ImeMode")));
      this.combo_performanceProfile.IntegralHeight = ((bool)(resources.GetObject("combo_performanceProfile.IntegralHeight")));
      this.combo_performanceProfile.ItemHeight = ((int)(resources.GetObject("combo_performanceProfile.ItemHeight")));
      this.combo_performanceProfile.Location = ((System.Drawing.Point)(resources.GetObject("combo_performanceProfile.Location")));
      this.combo_performanceProfile.MaxDropDownItems = ((int)(resources.GetObject("combo_performanceProfile.MaxDropDownItems")));
      this.combo_performanceProfile.MaxLength = ((int)(resources.GetObject("combo_performanceProfile.MaxLength")));
      this.combo_performanceProfile.Name = "combo_performanceProfile";
      this.combo_performanceProfile.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_performanceProfile.RightToLeft")));
      this.combo_performanceProfile.Size = ((System.Drawing.Size)(resources.GetObject("combo_performanceProfile.Size")));
      this.combo_performanceProfile.TabIndex = ((int)(resources.GetObject("combo_performanceProfile.TabIndex")));
      this.combo_performanceProfile.Text = resources.GetString("combo_performanceProfile.Text");
      this.toolTip.SetToolTip(this.combo_performanceProfile, resources.GetString("combo_performanceProfile.ToolTip"));
      this.combo_performanceProfile.Visible = ((bool)(resources.GetObject("combo_performanceProfile.Visible")));
      // 
      // label2
      // 
      this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
      this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
      this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
      this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
      this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
      this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
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
      // combo_qualityProfile
      // 
      this.combo_qualityProfile.AccessibleDescription = resources.GetString("combo_qualityProfile.AccessibleDescription");
      this.combo_qualityProfile.AccessibleName = resources.GetString("combo_qualityProfile.AccessibleName");
      this.combo_qualityProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_qualityProfile.Anchor")));
      this.combo_qualityProfile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_qualityProfile.BackgroundImage")));
      this.combo_qualityProfile.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_qualityProfile.Dock")));
      this.combo_qualityProfile.Enabled = ((bool)(resources.GetObject("combo_qualityProfile.Enabled")));
      this.combo_qualityProfile.Font = ((System.Drawing.Font)(resources.GetObject("combo_qualityProfile.Font")));
      this.combo_qualityProfile.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_qualityProfile.ImeMode")));
      this.combo_qualityProfile.IntegralHeight = ((bool)(resources.GetObject("combo_qualityProfile.IntegralHeight")));
      this.combo_qualityProfile.ItemHeight = ((int)(resources.GetObject("combo_qualityProfile.ItemHeight")));
      this.combo_qualityProfile.Location = ((System.Drawing.Point)(resources.GetObject("combo_qualityProfile.Location")));
      this.combo_qualityProfile.MaxDropDownItems = ((int)(resources.GetObject("combo_qualityProfile.MaxDropDownItems")));
      this.combo_qualityProfile.MaxLength = ((int)(resources.GetObject("combo_qualityProfile.MaxLength")));
      this.combo_qualityProfile.Name = "combo_qualityProfile";
      this.combo_qualityProfile.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_qualityProfile.RightToLeft")));
      this.combo_qualityProfile.Size = ((System.Drawing.Size)(resources.GetObject("combo_qualityProfile.Size")));
      this.combo_qualityProfile.TabIndex = ((int)(resources.GetObject("combo_qualityProfile.TabIndex")));
      this.combo_qualityProfile.Text = resources.GetString("combo_qualityProfile.Text");
      this.toolTip.SetToolTip(this.combo_qualityProfile, resources.GetString("combo_qualityProfile.ToolTip"));
      this.combo_qualityProfile.Visible = ((bool)(resources.GetObject("combo_qualityProfile.Visible")));
      // 
      // check_applyDefaultFirst
      // 
      this.check_applyDefaultFirst.AccessibleDescription = resources.GetString("check_applyDefaultFirst.AccessibleDescription");
      this.check_applyDefaultFirst.AccessibleName = resources.GetString("check_applyDefaultFirst.AccessibleName");
      this.check_applyDefaultFirst.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_applyDefaultFirst.Anchor")));
      this.check_applyDefaultFirst.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_applyDefaultFirst.Appearance")));
      this.check_applyDefaultFirst.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_applyDefaultFirst.BackgroundImage")));
      this.check_applyDefaultFirst.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_applyDefaultFirst.CheckAlign")));
      this.check_applyDefaultFirst.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_applyDefaultFirst.Dock")));
      this.check_applyDefaultFirst.Enabled = ((bool)(resources.GetObject("check_applyDefaultFirst.Enabled")));
      this.check_applyDefaultFirst.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_applyDefaultFirst.FlatStyle")));
      this.check_applyDefaultFirst.Font = ((System.Drawing.Font)(resources.GetObject("check_applyDefaultFirst.Font")));
      this.check_applyDefaultFirst.Image = ((System.Drawing.Image)(resources.GetObject("check_applyDefaultFirst.Image")));
      this.check_applyDefaultFirst.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_applyDefaultFirst.ImageAlign")));
      this.check_applyDefaultFirst.ImageIndex = ((int)(resources.GetObject("check_applyDefaultFirst.ImageIndex")));
      this.check_applyDefaultFirst.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_applyDefaultFirst.ImeMode")));
      this.check_applyDefaultFirst.Location = ((System.Drawing.Point)(resources.GetObject("check_applyDefaultFirst.Location")));
      this.check_applyDefaultFirst.Name = "check_applyDefaultFirst";
      this.check_applyDefaultFirst.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_applyDefaultFirst.RightToLeft")));
      this.check_applyDefaultFirst.Size = ((System.Drawing.Size)(resources.GetObject("check_applyDefaultFirst.Size")));
      this.check_applyDefaultFirst.TabIndex = ((int)(resources.GetObject("check_applyDefaultFirst.TabIndex")));
      this.check_applyDefaultFirst.Text = resources.GetString("check_applyDefaultFirst.Text");
      this.check_applyDefaultFirst.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_applyDefaultFirst.TextAlign")));
      this.toolTip.SetToolTip(this.check_applyDefaultFirst, resources.GetString("check_applyDefaultFirst.ToolTip"));
      this.check_applyDefaultFirst.Visible = ((bool)(resources.GetObject("check_applyDefaultFirst.Visible")));
      // 
      // openFile
      // 
      this.openFile.Filter = resources.GetString("openFile.Filter");
      this.openFile.Title = resources.GetString("openFile.Title");
      this.openFile.FileOk += new System.ComponentModel.CancelEventHandler(this.openFile_FileOk);
      // 
      // Form_Settings
      // 
      this.AcceptButton = this.button_ok;
      this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
      this.AccessibleName = resources.GetString("$this.AccessibleName");
      this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
      this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
      this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
      this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
      this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
      this.CancelButton = this.button_cancel;
      this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
      this.Controls.Add(this.tab);
      this.Controls.Add(this.button_cancel);
      this.Controls.Add(this.button_ok);
      this.Controls.Add(this.combo_qualityProfile);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.combo_performanceProfile);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.check_applyDefaultFirst);
      this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
      this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
      this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
      this.MaximizeBox = false;
      this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
      this.MinimizeBox = false;
      this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
      this.Name = "Form_Settings";
      this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
      this.ShowInTaskbar = false;
      this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
      this.Text = resources.GetString("$this.Text");
      this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
      this.Closing += new System.ComponentModel.CancelEventHandler(this.Form_Settings_Closing);
      this.Load += new System.EventHandler(this.Form_Settings_Load);
      this.tab.ResumeLayout(false);
      this.tab_main.ResumeLayout(false);
      this.tab_restore.ResumeLayout(false);
      this.group_autorestore.ResumeLayout(false);
      this.tab_sl.ResumeLayout(false);
      this.tab_clocking.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_max)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_pre_ultra)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_pre_fast)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_pre_normal)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_pre_slow)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_mem_min)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_max)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_pre_ultra)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_pre_fast)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_pre_normal)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_pre_slow)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.num_clock_chip_min)).EndInit();
      this.tab_extended.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion

    private void Form_Settings_Load(object sender, System.EventArgs e) {
      init_main();
      init_auto_restore();
      init_clocking();

      // Shell Links Tab
      text_SlNameSuffix.Text = ac.sl_name_suffix;
      text_SlNamePrefix.Text = ac.sl_name_prefix;

      // Special Profiles Tab


      for (int i=0; i < ax.gp.nmb_of_profiles; ++i) {
        if (ax.gp[i].exe_path != "" || (ax.gp[i].spec_name != "" && ax.gp[i].spec_name != G.spec_name))
          continue;
        else {
          combo_defaultProfile.Items.Add(ax.gp[i].name);
          if (ax.gp[i].name == ac.prof_default)
            combo_defaultProfile.SelectedIndex = combo_defaultProfile.Items.Count - 1;

          combo_qualityProfile.Items.Add(ax.gp[i].name);
          if (ax.gp[i].name == ac.prof_quality)
            combo_qualityProfile.SelectedIndex = combo_qualityProfile.Items.Count - 1;

          combo_performanceProfile.Items.Add(ax.gp[i].name);
          if (ax.gp[i].name == ac.prof_performance)
            combo_performanceProfile.SelectedIndex = combo_performanceProfile.Items.Count - 1;
        }
      }

      // Features Tab
      check_features_enableClocking.Checked = ac.feature_clocking;

      tab.SelectedTab = m_initialTabPage;

      label_SlPreview.BackColor = Color.FromArgb(SystemColors.Desktop.ToArgb());
      label_SlPreview.ForeColor = ((label_SlPreview.BackColor.GetBrightness() <= Color.Gray.GetBrightness())
        ? Color.White
        : Color.Black);
    }

    
    public void dev_make_screenshots() {
      string screenshots_path = G.ManualDirectory + @"\img\" + System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
      string prefix = screenshots_path + @"\" + "tdprof_shot_settings_";
      tab.SelectedTab = tab_main;
      Dev.ScreenShots.dev_make_and_save_screenshot(this, prefix + "main.png");
      tab.SelectedTab = tab_sl;
      Dev.ScreenShots.dev_make_and_save_screenshot(this, prefix + "links.png");
      tab.SelectedTab = tab_restore;
      Dev.ScreenShots.dev_make_and_save_screenshot(this, prefix + "restore.png");
      tab.SelectedTab = tab_clocking;
      Dev.ScreenShots.dev_make_and_save_screenshot(this, prefix + "clocking.png");
      tab.SelectedTab = tab_extended;
      Dev.ScreenShots.dev_make_and_save_screenshot(this, prefix + "extended.png");
    }

    void init_main() {
      text_daemon_exe_path.Text = ac.img_daemon_exe_path;
      check_gui_mover_feedback.Checked = ac.gui_mover_feedback;
      check_gui_show_tray_icon.Checked = ac.gui_show_tray_icon;

      int ms = ac.cr_timer_update;
      this.text_update_timer.Enabled 
        = this.check_update_timer.Checked = ms > 0;
      this.text_update_timer.Text = (ms < 0 ? ms * -1 : ms).ToString();
    }

    void ok_main() {
      ac.img_daemon_exe_path = text_daemon_exe_path.Text;
      ac.gui_mover_feedback = check_gui_mover_feedback.Checked;
      ac.gui_show_tray_icon = check_gui_show_tray_icon.Checked;

      int ms = int.Parse(text_update_timer.Text);
      if (!check_update_timer.Checked) ms *= -1;
      ac.cr_timer_update = ms;

    }

    void init_auto_restore() {
      check_sl_auto_restore.Checked   = ac.auto_restore_after_exit;
      check_auto_restore.Checked      = ac.auto_restore_after_exit_in_gui;
      check_restoreToDefault.Checked  = ac.auto_restore_to_default_profile;
    }

    void ok_auto_restore() {
      ac.auto_restore_after_exit         = check_sl_auto_restore.Checked;
      ac.auto_restore_after_exit_in_gui  = check_auto_restore.Checked;
      ac.auto_restore_to_default_profile = check_restoreToDefault.Checked;
      ac.prof_default                    = combo_defaultProfile.Text;
    }

    void init_clocking() {

      int idx = 0;
      int[] limits = ac.clocking_limits;
      foreach(NumericUpDown teb in new NumericUpDown[] {num_clock_mem_min, num_clock_mem_max, num_clock_chip_min, num_clock_chip_max}) {
        teb.Value = limits[idx++];
      }

      int[] preset;
      preset = ac.clocking_preset_slow;
      num_clock_chip_pre_slow.Value = preset[0];
      num_clock_mem_pre_slow.Value  = preset[1];

      preset = ac.clocking_preset_normal;
      num_clock_chip_pre_normal.Value = preset[0];
      num_clock_mem_pre_normal.Value  = preset[1];

      preset = ac.clocking_preset_fast;
      num_clock_chip_pre_fast.Value = preset[0];
      num_clock_mem_pre_fast.Value  = preset[1];

      preset = ac.clocking_preset_ultra;
      num_clock_chip_pre_ultra.Value = preset[0];
      num_clock_mem_pre_ultra.Value  = preset[1];

      combo_clocking_prof_restorePresets.SelectedIndex = (int)ac.clocking_restore_kind;

      check_clock_atiSmallSteps.Checked = ac.exp_radeon_small_clocksteps;
      check_clock_enableDLL.Checked     = ac.exp_clocking_dll;                                                                                                         
                                                                                                           
    }

    void ok_clocking() {
      int idx = 0;
      int[] limits = new int[4];
      foreach(NumericUpDown teb in new NumericUpDown[] {num_clock_mem_min, num_clock_mem_max, num_clock_chip_min, num_clock_chip_max}) {
        limits[idx++] = (int)teb.Value;
      } 
      ac.clocking_limits = limits;

      int[] preset = new int[2];

      preset[0] = (int)num_clock_chip_pre_slow.Value;
      preset[1] = (int)num_clock_mem_pre_slow.Value;
      ac.clocking_preset_slow = preset;

      preset[0] = (int)num_clock_chip_pre_normal.Value;
      preset[1] = (int)num_clock_mem_pre_normal.Value;
      ac.clocking_preset_normal = preset;

      preset[0] = (int)num_clock_chip_pre_fast.Value;
      preset[1] = (int)num_clock_mem_pre_fast.Value;
      ac.clocking_preset_fast = preset;

      preset[0] = (int)num_clock_chip_pre_ultra.Value;
      preset[1] = (int)num_clock_mem_pre_ultra.Value;
      ac.clocking_preset_ultra = preset;


      ac.clocking_restore_kind = (Profiles.GameProfileData.EClockKinds)combo_clocking_prof_restorePresets.SelectedIndex;
      
  
      ac.exp_radeon_small_clocksteps = check_clock_atiSmallSteps.Checked;
      ac.exp_clocking_dll            = check_clock_enableDLL.Checked;                                                                                                         
    }

    private void button_ok_Click(object sender, System.EventArgs e) {

      ok_main();
      ok_auto_restore();

      ok_clocking();

      // Shell Link Tab
      ac.sl_name_suffix = text_SlNameSuffix.Text;
      ac.sl_name_prefix = text_SlNamePrefix.Text;

      // Special Profiles Tab
      ac.prof_quality = combo_qualityProfile.Text;
      ac.prof_performance = combo_performanceProfile.Text;

      // Features Tab
      ac.feature_clocking = check_features_enableClocking.Checked;

    
      ac.save_config();
      Hide();
      form_root.Activate();
    }

    private void button_cancel_Click(object sender, System.EventArgs e) {
      Hide();
      form_root.Activate();
      GC.Collect();
    }

    private void button__choose_daemon_exe_path_Click(object sender, System.EventArgs e) {

      openFile.FileName = text_daemon_exe_path.Text;
      openFile.Filter = "Daeom.exe|Daemon.exe|All Files|*.*";
      openFile.ReadOnlyChecked = true;
      openFile.Title = "Find Daemon.exe file";
      openFile.ShowDialog();
      openFile.Filter = "";
      openFile.Multiselect = false;

    }

    private void openFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {
      if (openFile.Title == "Find Daemon.exe file")
        text_daemon_exe_path.Text = openFile.FileName;
    }

    private void check_update_timer_CheckedChanged(object sender, System.EventArgs e) {
      text_update_timer.Enabled = check_update_timer.Checked;
    }



    private void Form_Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
      if (G.ax.cl.opt_screenshots) {
      //  dev_make_screenshots();
      }

    }

    private void text_SlName_TextChanged(object sender, System.EventArgs e) {
      text_SlNamePrefix.Text = text_SlNamePrefix.Text.Replace(":", "").Replace("/", "/");
      text_SlNameSuffix.Text = text_SlNameSuffix.Text.Replace(":", "").Replace("/", "/");
      label_SlPreview.Text = text_SlNamePrefix.Text + "Demo" + text_SlNameSuffix.Text;
    }







  }
}
