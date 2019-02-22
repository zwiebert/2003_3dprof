using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using TDProf.App;
using TDProf.DriverSettings;
using TDProf.Profiles;
using TDProf.Util;

namespace TDProf {

  /// <summary>
  /// Summary description for FormsCommands.
  /// </summary>
  public class FormCommands : System.Windows.Forms.Form {
    #region Private Data

    AppContext ax = G.ax;
    bool prof_unsaved = G.prof_change_count > 0;
    bool prof_changed = false;

    #endregion

    #region Init / Exit
    public FormCommands(int gpd_idx) {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //
      // TODO: Add any constructor code after InitializeComponent call
      //
      init_from_gp();
      init_from_ac();

      if (gpd_idx != -1 && gpd_idx < combo_prof_names.Items.Count)
        combo_prof_names.SelectedIndex = gpd_idx;
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

    private void init_from_ac() {
      text_cmdPreExeRun.Text = ax.ac.command_pre_exe_run;
      text_cmdPostExeExit.Text = ax.ac.command_post_exe_exit;
    }
    private void init_from_gp() {
      bool filter_by_videocard = ax.ac.gui_filter_by_spec_name;

      for (int i=0; i < ax.gp.nmb_of_profiles; ++i) {
        GameProfileData gpd = ax.gp.get_profile(i);
        if (filter_by_videocard && !(gpd.spec_name == G.spec_name || gpd.spec_name == ""))
          break;
        combo_prof_names.Items.Add(gpd.name);
      } // for

    }

    private void init_from_gpd(GameProfileData gpd) {
      text_prof_cmdPreExeRun.Text     = gpd.command_pre_exe_run;
      text_prof_cmdPostExeExit.Text   = gpd.command_post_exe_exit;
      check_prof_globPreCmds.Checked  = gpd.command_pre_exe_run_glob;
      check_prof_globPostCmds.Checked = gpd.command_post_exe_exit_glob;
    }

    #endregion

		#region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormCommands));
      this.label_cmdPostExeExit = new System.Windows.Forms.Label();
      this.label_cmdPreExeRun = new System.Windows.Forms.Label();
      this.text_cmdPostExeExit = new System.Windows.Forms.TextBox();
      this.text_cmdPreExeRun = new System.Windows.Forms.TextBox();
      this.text_prof_cmdPostExeExit = new System.Windows.Forms.TextBox();
      this.text_prof_cmdPreExeRun = new System.Windows.Forms.TextBox();
      this.combo_prof_names = new System.Windows.Forms.ComboBox();
      this.button_commands_ok = new System.Windows.Forms.Button();
      this.button_commands_cancel = new System.Windows.Forms.Button();
      this.button_commands_edit = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.check_prof_globPostCmds = new System.Windows.Forms.CheckBox();
      this.check_prof_globPreCmds = new System.Windows.Forms.CheckBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.button_fileDialog = new System.Windows.Forms.Button();
      this.ofd = new System.Windows.Forms.OpenFileDialog();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // label_cmdPostExeExit
      // 
      this.label_cmdPostExeExit.AccessibleDescription = ((string)(resources.GetObject("label_cmdPostExeExit.AccessibleDescription")));
      this.label_cmdPostExeExit.AccessibleName = ((string)(resources.GetObject("label_cmdPostExeExit.AccessibleName")));
      this.label_cmdPostExeExit.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_cmdPostExeExit.Anchor")));
      this.label_cmdPostExeExit.AutoSize = ((bool)(resources.GetObject("label_cmdPostExeExit.AutoSize")));
      this.label_cmdPostExeExit.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_cmdPostExeExit.Dock")));
      this.label_cmdPostExeExit.Enabled = ((bool)(resources.GetObject("label_cmdPostExeExit.Enabled")));
      this.label_cmdPostExeExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.label_cmdPostExeExit.Font = ((System.Drawing.Font)(resources.GetObject("label_cmdPostExeExit.Font")));
      this.label_cmdPostExeExit.Image = ((System.Drawing.Image)(resources.GetObject("label_cmdPostExeExit.Image")));
      this.label_cmdPostExeExit.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_cmdPostExeExit.ImageAlign")));
      this.label_cmdPostExeExit.ImageIndex = ((int)(resources.GetObject("label_cmdPostExeExit.ImageIndex")));
      this.label_cmdPostExeExit.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_cmdPostExeExit.ImeMode")));
      this.label_cmdPostExeExit.Location = ((System.Drawing.Point)(resources.GetObject("label_cmdPostExeExit.Location")));
      this.label_cmdPostExeExit.Name = "label_cmdPostExeExit";
      this.label_cmdPostExeExit.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_cmdPostExeExit.RightToLeft")));
      this.label_cmdPostExeExit.Size = ((System.Drawing.Size)(resources.GetObject("label_cmdPostExeExit.Size")));
      this.label_cmdPostExeExit.TabIndex = ((int)(resources.GetObject("label_cmdPostExeExit.TabIndex")));
      this.label_cmdPostExeExit.Text = resources.GetString("label_cmdPostExeExit.Text");
      this.label_cmdPostExeExit.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_cmdPostExeExit.TextAlign")));
      this.toolTip.SetToolTip(this.label_cmdPostExeExit, resources.GetString("label_cmdPostExeExit.ToolTip"));
      this.label_cmdPostExeExit.Visible = ((bool)(resources.GetObject("label_cmdPostExeExit.Visible")));
      // 
      // label_cmdPreExeRun
      // 
      this.label_cmdPreExeRun.AccessibleDescription = ((string)(resources.GetObject("label_cmdPreExeRun.AccessibleDescription")));
      this.label_cmdPreExeRun.AccessibleName = ((string)(resources.GetObject("label_cmdPreExeRun.AccessibleName")));
      this.label_cmdPreExeRun.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_cmdPreExeRun.Anchor")));
      this.label_cmdPreExeRun.AutoSize = ((bool)(resources.GetObject("label_cmdPreExeRun.AutoSize")));
      this.label_cmdPreExeRun.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_cmdPreExeRun.Dock")));
      this.label_cmdPreExeRun.Enabled = ((bool)(resources.GetObject("label_cmdPreExeRun.Enabled")));
      this.label_cmdPreExeRun.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.label_cmdPreExeRun.Font = ((System.Drawing.Font)(resources.GetObject("label_cmdPreExeRun.Font")));
      this.label_cmdPreExeRun.Image = ((System.Drawing.Image)(resources.GetObject("label_cmdPreExeRun.Image")));
      this.label_cmdPreExeRun.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_cmdPreExeRun.ImageAlign")));
      this.label_cmdPreExeRun.ImageIndex = ((int)(resources.GetObject("label_cmdPreExeRun.ImageIndex")));
      this.label_cmdPreExeRun.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_cmdPreExeRun.ImeMode")));
      this.label_cmdPreExeRun.Location = ((System.Drawing.Point)(resources.GetObject("label_cmdPreExeRun.Location")));
      this.label_cmdPreExeRun.Name = "label_cmdPreExeRun";
      this.label_cmdPreExeRun.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_cmdPreExeRun.RightToLeft")));
      this.label_cmdPreExeRun.Size = ((System.Drawing.Size)(resources.GetObject("label_cmdPreExeRun.Size")));
      this.label_cmdPreExeRun.TabIndex = ((int)(resources.GetObject("label_cmdPreExeRun.TabIndex")));
      this.label_cmdPreExeRun.Text = resources.GetString("label_cmdPreExeRun.Text");
      this.label_cmdPreExeRun.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_cmdPreExeRun.TextAlign")));
      this.toolTip.SetToolTip(this.label_cmdPreExeRun, resources.GetString("label_cmdPreExeRun.ToolTip"));
      this.label_cmdPreExeRun.Visible = ((bool)(resources.GetObject("label_cmdPreExeRun.Visible")));
      // 
      // text_cmdPostExeExit
      // 
      this.text_cmdPostExeExit.AcceptsReturn = true;
      this.text_cmdPostExeExit.AcceptsTab = true;
      this.text_cmdPostExeExit.AccessibleDescription = ((string)(resources.GetObject("text_cmdPostExeExit.AccessibleDescription")));
      this.text_cmdPostExeExit.AccessibleName = ((string)(resources.GetObject("text_cmdPostExeExit.AccessibleName")));
      this.text_cmdPostExeExit.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_cmdPostExeExit.Anchor")));
      this.text_cmdPostExeExit.AutoSize = ((bool)(resources.GetObject("text_cmdPostExeExit.AutoSize")));
      this.text_cmdPostExeExit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_cmdPostExeExit.BackgroundImage")));
      this.text_cmdPostExeExit.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_cmdPostExeExit.Dock")));
      this.text_cmdPostExeExit.Enabled = ((bool)(resources.GetObject("text_cmdPostExeExit.Enabled")));
      this.text_cmdPostExeExit.Font = ((System.Drawing.Font)(resources.GetObject("text_cmdPostExeExit.Font")));
      this.text_cmdPostExeExit.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_cmdPostExeExit.ImeMode")));
      this.text_cmdPostExeExit.Location = ((System.Drawing.Point)(resources.GetObject("text_cmdPostExeExit.Location")));
      this.text_cmdPostExeExit.MaxLength = ((int)(resources.GetObject("text_cmdPostExeExit.MaxLength")));
      this.text_cmdPostExeExit.Multiline = ((bool)(resources.GetObject("text_cmdPostExeExit.Multiline")));
      this.text_cmdPostExeExit.Name = "text_cmdPostExeExit";
      this.text_cmdPostExeExit.PasswordChar = ((char)(resources.GetObject("text_cmdPostExeExit.PasswordChar")));
      this.text_cmdPostExeExit.ReadOnly = true;
      this.text_cmdPostExeExit.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_cmdPostExeExit.RightToLeft")));
      this.text_cmdPostExeExit.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_cmdPostExeExit.ScrollBars")));
      this.text_cmdPostExeExit.Size = ((System.Drawing.Size)(resources.GetObject("text_cmdPostExeExit.Size")));
      this.text_cmdPostExeExit.TabIndex = ((int)(resources.GetObject("text_cmdPostExeExit.TabIndex")));
      this.text_cmdPostExeExit.Text = resources.GetString("text_cmdPostExeExit.Text");
      this.text_cmdPostExeExit.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_cmdPostExeExit.TextAlign")));
      this.toolTip.SetToolTip(this.text_cmdPostExeExit, resources.GetString("text_cmdPostExeExit.ToolTip"));
      this.text_cmdPostExeExit.Visible = ((bool)(resources.GetObject("text_cmdPostExeExit.Visible")));
      this.text_cmdPostExeExit.WordWrap = ((bool)(resources.GetObject("text_cmdPostExeExit.WordWrap")));
      this.text_cmdPostExeExit.Enter += new System.EventHandler(this.text_any_Enter);
      // 
      // text_cmdPreExeRun
      // 
      this.text_cmdPreExeRun.AcceptsReturn = true;
      this.text_cmdPreExeRun.AcceptsTab = true;
      this.text_cmdPreExeRun.AccessibleDescription = ((string)(resources.GetObject("text_cmdPreExeRun.AccessibleDescription")));
      this.text_cmdPreExeRun.AccessibleName = ((string)(resources.GetObject("text_cmdPreExeRun.AccessibleName")));
      this.text_cmdPreExeRun.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_cmdPreExeRun.Anchor")));
      this.text_cmdPreExeRun.AutoSize = ((bool)(resources.GetObject("text_cmdPreExeRun.AutoSize")));
      this.text_cmdPreExeRun.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_cmdPreExeRun.BackgroundImage")));
      this.text_cmdPreExeRun.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_cmdPreExeRun.Dock")));
      this.text_cmdPreExeRun.Enabled = ((bool)(resources.GetObject("text_cmdPreExeRun.Enabled")));
      this.text_cmdPreExeRun.Font = ((System.Drawing.Font)(resources.GetObject("text_cmdPreExeRun.Font")));
      this.text_cmdPreExeRun.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_cmdPreExeRun.ImeMode")));
      this.text_cmdPreExeRun.Location = ((System.Drawing.Point)(resources.GetObject("text_cmdPreExeRun.Location")));
      this.text_cmdPreExeRun.MaxLength = ((int)(resources.GetObject("text_cmdPreExeRun.MaxLength")));
      this.text_cmdPreExeRun.Multiline = ((bool)(resources.GetObject("text_cmdPreExeRun.Multiline")));
      this.text_cmdPreExeRun.Name = "text_cmdPreExeRun";
      this.text_cmdPreExeRun.PasswordChar = ((char)(resources.GetObject("text_cmdPreExeRun.PasswordChar")));
      this.text_cmdPreExeRun.ReadOnly = true;
      this.text_cmdPreExeRun.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_cmdPreExeRun.RightToLeft")));
      this.text_cmdPreExeRun.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_cmdPreExeRun.ScrollBars")));
      this.text_cmdPreExeRun.Size = ((System.Drawing.Size)(resources.GetObject("text_cmdPreExeRun.Size")));
      this.text_cmdPreExeRun.TabIndex = ((int)(resources.GetObject("text_cmdPreExeRun.TabIndex")));
      this.text_cmdPreExeRun.Text = resources.GetString("text_cmdPreExeRun.Text");
      this.text_cmdPreExeRun.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_cmdPreExeRun.TextAlign")));
      this.toolTip.SetToolTip(this.text_cmdPreExeRun, resources.GetString("text_cmdPreExeRun.ToolTip"));
      this.text_cmdPreExeRun.Visible = ((bool)(resources.GetObject("text_cmdPreExeRun.Visible")));
      this.text_cmdPreExeRun.WordWrap = ((bool)(resources.GetObject("text_cmdPreExeRun.WordWrap")));
      this.text_cmdPreExeRun.Enter += new System.EventHandler(this.text_any_Enter);
      // 
      // text_prof_cmdPostExeExit
      // 
      this.text_prof_cmdPostExeExit.AcceptsReturn = true;
      this.text_prof_cmdPostExeExit.AcceptsTab = true;
      this.text_prof_cmdPostExeExit.AccessibleDescription = ((string)(resources.GetObject("text_prof_cmdPostExeExit.AccessibleDescription")));
      this.text_prof_cmdPostExeExit.AccessibleName = ((string)(resources.GetObject("text_prof_cmdPostExeExit.AccessibleName")));
      this.text_prof_cmdPostExeExit.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_prof_cmdPostExeExit.Anchor")));
      this.text_prof_cmdPostExeExit.AutoSize = ((bool)(resources.GetObject("text_prof_cmdPostExeExit.AutoSize")));
      this.text_prof_cmdPostExeExit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_prof_cmdPostExeExit.BackgroundImage")));
      this.text_prof_cmdPostExeExit.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_prof_cmdPostExeExit.Dock")));
      this.text_prof_cmdPostExeExit.Enabled = ((bool)(resources.GetObject("text_prof_cmdPostExeExit.Enabled")));
      this.text_prof_cmdPostExeExit.Font = ((System.Drawing.Font)(resources.GetObject("text_prof_cmdPostExeExit.Font")));
      this.text_prof_cmdPostExeExit.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_prof_cmdPostExeExit.ImeMode")));
      this.text_prof_cmdPostExeExit.Location = ((System.Drawing.Point)(resources.GetObject("text_prof_cmdPostExeExit.Location")));
      this.text_prof_cmdPostExeExit.MaxLength = ((int)(resources.GetObject("text_prof_cmdPostExeExit.MaxLength")));
      this.text_prof_cmdPostExeExit.Multiline = ((bool)(resources.GetObject("text_prof_cmdPostExeExit.Multiline")));
      this.text_prof_cmdPostExeExit.Name = "text_prof_cmdPostExeExit";
      this.text_prof_cmdPostExeExit.PasswordChar = ((char)(resources.GetObject("text_prof_cmdPostExeExit.PasswordChar")));
      this.text_prof_cmdPostExeExit.ReadOnly = true;
      this.text_prof_cmdPostExeExit.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_prof_cmdPostExeExit.RightToLeft")));
      this.text_prof_cmdPostExeExit.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_prof_cmdPostExeExit.ScrollBars")));
      this.text_prof_cmdPostExeExit.Size = ((System.Drawing.Size)(resources.GetObject("text_prof_cmdPostExeExit.Size")));
      this.text_prof_cmdPostExeExit.TabIndex = ((int)(resources.GetObject("text_prof_cmdPostExeExit.TabIndex")));
      this.text_prof_cmdPostExeExit.Text = resources.GetString("text_prof_cmdPostExeExit.Text");
      this.text_prof_cmdPostExeExit.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_prof_cmdPostExeExit.TextAlign")));
      this.toolTip.SetToolTip(this.text_prof_cmdPostExeExit, resources.GetString("text_prof_cmdPostExeExit.ToolTip"));
      this.text_prof_cmdPostExeExit.Visible = ((bool)(resources.GetObject("text_prof_cmdPostExeExit.Visible")));
      this.text_prof_cmdPostExeExit.WordWrap = ((bool)(resources.GetObject("text_prof_cmdPostExeExit.WordWrap")));
      this.text_prof_cmdPostExeExit.Enter += new System.EventHandler(this.text_any_Enter);
      // 
      // text_prof_cmdPreExeRun
      // 
      this.text_prof_cmdPreExeRun.AcceptsReturn = true;
      this.text_prof_cmdPreExeRun.AcceptsTab = true;
      this.text_prof_cmdPreExeRun.AccessibleDescription = ((string)(resources.GetObject("text_prof_cmdPreExeRun.AccessibleDescription")));
      this.text_prof_cmdPreExeRun.AccessibleName = ((string)(resources.GetObject("text_prof_cmdPreExeRun.AccessibleName")));
      this.text_prof_cmdPreExeRun.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_prof_cmdPreExeRun.Anchor")));
      this.text_prof_cmdPreExeRun.AutoSize = ((bool)(resources.GetObject("text_prof_cmdPreExeRun.AutoSize")));
      this.text_prof_cmdPreExeRun.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_prof_cmdPreExeRun.BackgroundImage")));
      this.text_prof_cmdPreExeRun.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_prof_cmdPreExeRun.Dock")));
      this.text_prof_cmdPreExeRun.Enabled = ((bool)(resources.GetObject("text_prof_cmdPreExeRun.Enabled")));
      this.text_prof_cmdPreExeRun.Font = ((System.Drawing.Font)(resources.GetObject("text_prof_cmdPreExeRun.Font")));
      this.text_prof_cmdPreExeRun.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_prof_cmdPreExeRun.ImeMode")));
      this.text_prof_cmdPreExeRun.Location = ((System.Drawing.Point)(resources.GetObject("text_prof_cmdPreExeRun.Location")));
      this.text_prof_cmdPreExeRun.MaxLength = ((int)(resources.GetObject("text_prof_cmdPreExeRun.MaxLength")));
      this.text_prof_cmdPreExeRun.Multiline = ((bool)(resources.GetObject("text_prof_cmdPreExeRun.Multiline")));
      this.text_prof_cmdPreExeRun.Name = "text_prof_cmdPreExeRun";
      this.text_prof_cmdPreExeRun.PasswordChar = ((char)(resources.GetObject("text_prof_cmdPreExeRun.PasswordChar")));
      this.text_prof_cmdPreExeRun.ReadOnly = true;
      this.text_prof_cmdPreExeRun.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_prof_cmdPreExeRun.RightToLeft")));
      this.text_prof_cmdPreExeRun.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_prof_cmdPreExeRun.ScrollBars")));
      this.text_prof_cmdPreExeRun.Size = ((System.Drawing.Size)(resources.GetObject("text_prof_cmdPreExeRun.Size")));
      this.text_prof_cmdPreExeRun.TabIndex = ((int)(resources.GetObject("text_prof_cmdPreExeRun.TabIndex")));
      this.text_prof_cmdPreExeRun.Text = resources.GetString("text_prof_cmdPreExeRun.Text");
      this.text_prof_cmdPreExeRun.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_prof_cmdPreExeRun.TextAlign")));
      this.toolTip.SetToolTip(this.text_prof_cmdPreExeRun, resources.GetString("text_prof_cmdPreExeRun.ToolTip"));
      this.text_prof_cmdPreExeRun.Visible = ((bool)(resources.GetObject("text_prof_cmdPreExeRun.Visible")));
      this.text_prof_cmdPreExeRun.WordWrap = ((bool)(resources.GetObject("text_prof_cmdPreExeRun.WordWrap")));
      this.text_prof_cmdPreExeRun.Enter += new System.EventHandler(this.text_any_Enter);
      // 
      // combo_prof_names
      // 
      this.combo_prof_names.AccessibleDescription = ((string)(resources.GetObject("combo_prof_names.AccessibleDescription")));
      this.combo_prof_names.AccessibleName = ((string)(resources.GetObject("combo_prof_names.AccessibleName")));
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
      this.combo_prof_names.SelectedIndexChanged += new System.EventHandler(this.combo_prof_names_SelectedIndexChanged);
      this.combo_prof_names.Enter += new System.EventHandler(this.text_any_Leave);
      // 
      // button_commands_ok
      // 
      this.button_commands_ok.AccessibleDescription = ((string)(resources.GetObject("button_commands_ok.AccessibleDescription")));
      this.button_commands_ok.AccessibleName = ((string)(resources.GetObject("button_commands_ok.AccessibleName")));
      this.button_commands_ok.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_commands_ok.Anchor")));
      this.button_commands_ok.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_commands_ok.BackgroundImage")));
      this.button_commands_ok.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_commands_ok.Dock")));
      this.button_commands_ok.Enabled = ((bool)(resources.GetObject("button_commands_ok.Enabled")));
      this.button_commands_ok.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_commands_ok.FlatStyle")));
      this.button_commands_ok.Font = ((System.Drawing.Font)(resources.GetObject("button_commands_ok.Font")));
      this.button_commands_ok.Image = ((System.Drawing.Image)(resources.GetObject("button_commands_ok.Image")));
      this.button_commands_ok.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_commands_ok.ImageAlign")));
      this.button_commands_ok.ImageIndex = ((int)(resources.GetObject("button_commands_ok.ImageIndex")));
      this.button_commands_ok.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_commands_ok.ImeMode")));
      this.button_commands_ok.Location = ((System.Drawing.Point)(resources.GetObject("button_commands_ok.Location")));
      this.button_commands_ok.Name = "button_commands_ok";
      this.button_commands_ok.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_commands_ok.RightToLeft")));
      this.button_commands_ok.Size = ((System.Drawing.Size)(resources.GetObject("button_commands_ok.Size")));
      this.button_commands_ok.TabIndex = ((int)(resources.GetObject("button_commands_ok.TabIndex")));
      this.button_commands_ok.Text = resources.GetString("button_commands_ok.Text");
      this.button_commands_ok.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_commands_ok.TextAlign")));
      this.toolTip.SetToolTip(this.button_commands_ok, resources.GetString("button_commands_ok.ToolTip"));
      this.button_commands_ok.Visible = ((bool)(resources.GetObject("button_commands_ok.Visible")));
      this.button_commands_ok.Click += new System.EventHandler(this.button_commands_ok_Click);
      // 
      // button_commands_cancel
      // 
      this.button_commands_cancel.AccessibleDescription = ((string)(resources.GetObject("button_commands_cancel.AccessibleDescription")));
      this.button_commands_cancel.AccessibleName = ((string)(resources.GetObject("button_commands_cancel.AccessibleName")));
      this.button_commands_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_commands_cancel.Anchor")));
      this.button_commands_cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_commands_cancel.BackgroundImage")));
      this.button_commands_cancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_commands_cancel.Dock")));
      this.button_commands_cancel.Enabled = ((bool)(resources.GetObject("button_commands_cancel.Enabled")));
      this.button_commands_cancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_commands_cancel.FlatStyle")));
      this.button_commands_cancel.Font = ((System.Drawing.Font)(resources.GetObject("button_commands_cancel.Font")));
      this.button_commands_cancel.Image = ((System.Drawing.Image)(resources.GetObject("button_commands_cancel.Image")));
      this.button_commands_cancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_commands_cancel.ImageAlign")));
      this.button_commands_cancel.ImageIndex = ((int)(resources.GetObject("button_commands_cancel.ImageIndex")));
      this.button_commands_cancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_commands_cancel.ImeMode")));
      this.button_commands_cancel.Location = ((System.Drawing.Point)(resources.GetObject("button_commands_cancel.Location")));
      this.button_commands_cancel.Name = "button_commands_cancel";
      this.button_commands_cancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_commands_cancel.RightToLeft")));
      this.button_commands_cancel.Size = ((System.Drawing.Size)(resources.GetObject("button_commands_cancel.Size")));
      this.button_commands_cancel.TabIndex = ((int)(resources.GetObject("button_commands_cancel.TabIndex")));
      this.button_commands_cancel.Text = resources.GetString("button_commands_cancel.Text");
      this.button_commands_cancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_commands_cancel.TextAlign")));
      this.toolTip.SetToolTip(this.button_commands_cancel, resources.GetString("button_commands_cancel.ToolTip"));
      this.button_commands_cancel.Visible = ((bool)(resources.GetObject("button_commands_cancel.Visible")));
      this.button_commands_cancel.Click += new System.EventHandler(this.button_commands_cancel_Click);
      // 
      // button_commands_edit
      // 
      this.button_commands_edit.AccessibleDescription = ((string)(resources.GetObject("button_commands_edit.AccessibleDescription")));
      this.button_commands_edit.AccessibleName = ((string)(resources.GetObject("button_commands_edit.AccessibleName")));
      this.button_commands_edit.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_commands_edit.Anchor")));
      this.button_commands_edit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_commands_edit.BackgroundImage")));
      this.button_commands_edit.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_commands_edit.Dock")));
      this.button_commands_edit.Enabled = ((bool)(resources.GetObject("button_commands_edit.Enabled")));
      this.button_commands_edit.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_commands_edit.FlatStyle")));
      this.button_commands_edit.Font = ((System.Drawing.Font)(resources.GetObject("button_commands_edit.Font")));
      this.button_commands_edit.Image = ((System.Drawing.Image)(resources.GetObject("button_commands_edit.Image")));
      this.button_commands_edit.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_commands_edit.ImageAlign")));
      this.button_commands_edit.ImageIndex = ((int)(resources.GetObject("button_commands_edit.ImageIndex")));
      this.button_commands_edit.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_commands_edit.ImeMode")));
      this.button_commands_edit.Location = ((System.Drawing.Point)(resources.GetObject("button_commands_edit.Location")));
      this.button_commands_edit.Name = "button_commands_edit";
      this.button_commands_edit.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_commands_edit.RightToLeft")));
      this.button_commands_edit.Size = ((System.Drawing.Size)(resources.GetObject("button_commands_edit.Size")));
      this.button_commands_edit.TabIndex = ((int)(resources.GetObject("button_commands_edit.TabIndex")));
      this.button_commands_edit.Text = resources.GetString("button_commands_edit.Text");
      this.button_commands_edit.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_commands_edit.TextAlign")));
      this.toolTip.SetToolTip(this.button_commands_edit, resources.GetString("button_commands_edit.ToolTip"));
      this.button_commands_edit.Visible = ((bool)(resources.GetObject("button_commands_edit.Visible")));
      this.button_commands_edit.Click += new System.EventHandler(this.button_commands_edit_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.AccessibleDescription = ((string)(resources.GetObject("groupBox1.AccessibleDescription")));
      this.groupBox1.AccessibleName = ((string)(resources.GetObject("groupBox1.AccessibleName")));
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox1.Anchor")));
      this.groupBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox1.BackgroundImage")));
      this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                            this.text_cmdPreExeRun,
                                                                            this.text_cmdPostExeExit});
      this.groupBox1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox1.Dock")));
      this.groupBox1.Enabled = ((bool)(resources.GetObject("groupBox1.Enabled")));
      this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.groupBox1.Font = ((System.Drawing.Font)(resources.GetObject("groupBox1.Font")));
      this.groupBox1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox1.ImeMode")));
      this.groupBox1.Location = ((System.Drawing.Point)(resources.GetObject("groupBox1.Location")));
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox1.RightToLeft")));
      this.groupBox1.Size = ((System.Drawing.Size)(resources.GetObject("groupBox1.Size")));
      this.groupBox1.TabIndex = ((int)(resources.GetObject("groupBox1.TabIndex")));
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = resources.GetString("groupBox1.Text");
      this.toolTip.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
      this.groupBox1.Visible = ((bool)(resources.GetObject("groupBox1.Visible")));
      // 
      // groupBox2
      // 
      this.groupBox2.AccessibleDescription = ((string)(resources.GetObject("groupBox2.AccessibleDescription")));
      this.groupBox2.AccessibleName = ((string)(resources.GetObject("groupBox2.AccessibleName")));
      this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox2.Anchor")));
      this.groupBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox2.BackgroundImage")));
      this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                            this.check_prof_globPostCmds,
                                                                            this.check_prof_globPreCmds,
                                                                            this.text_prof_cmdPreExeRun,
                                                                            this.text_prof_cmdPostExeExit,
                                                                            this.combo_prof_names});
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
      // check_prof_globPostCmds
      // 
      this.check_prof_globPostCmds.AccessibleDescription = ((string)(resources.GetObject("check_prof_globPostCmds.AccessibleDescription")));
      this.check_prof_globPostCmds.AccessibleName = ((string)(resources.GetObject("check_prof_globPostCmds.AccessibleName")));
      this.check_prof_globPostCmds.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_prof_globPostCmds.Anchor")));
      this.check_prof_globPostCmds.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_prof_globPostCmds.Appearance")));
      this.check_prof_globPostCmds.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_prof_globPostCmds.BackgroundImage")));
      this.check_prof_globPostCmds.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_globPostCmds.CheckAlign")));
      this.check_prof_globPostCmds.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_prof_globPostCmds.Dock")));
      this.check_prof_globPostCmds.Enabled = ((bool)(resources.GetObject("check_prof_globPostCmds.Enabled")));
      this.check_prof_globPostCmds.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_prof_globPostCmds.FlatStyle")));
      this.check_prof_globPostCmds.Font = ((System.Drawing.Font)(resources.GetObject("check_prof_globPostCmds.Font")));
      this.check_prof_globPostCmds.Image = ((System.Drawing.Image)(resources.GetObject("check_prof_globPostCmds.Image")));
      this.check_prof_globPostCmds.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_globPostCmds.ImageAlign")));
      this.check_prof_globPostCmds.ImageIndex = ((int)(resources.GetObject("check_prof_globPostCmds.ImageIndex")));
      this.check_prof_globPostCmds.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_prof_globPostCmds.ImeMode")));
      this.check_prof_globPostCmds.Location = ((System.Drawing.Point)(resources.GetObject("check_prof_globPostCmds.Location")));
      this.check_prof_globPostCmds.Name = "check_prof_globPostCmds";
      this.check_prof_globPostCmds.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_prof_globPostCmds.RightToLeft")));
      this.check_prof_globPostCmds.Size = ((System.Drawing.Size)(resources.GetObject("check_prof_globPostCmds.Size")));
      this.check_prof_globPostCmds.TabIndex = ((int)(resources.GetObject("check_prof_globPostCmds.TabIndex")));
      this.check_prof_globPostCmds.Text = resources.GetString("check_prof_globPostCmds.Text");
      this.check_prof_globPostCmds.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_globPostCmds.TextAlign")));
      this.toolTip.SetToolTip(this.check_prof_globPostCmds, resources.GetString("check_prof_globPostCmds.ToolTip"));
      this.check_prof_globPostCmds.Visible = ((bool)(resources.GetObject("check_prof_globPostCmds.Visible")));
      this.check_prof_globPostCmds.Enter += new System.EventHandler(this.text_any_Leave);
      // 
      // check_prof_globPreCmds
      // 
      this.check_prof_globPreCmds.AccessibleDescription = ((string)(resources.GetObject("check_prof_globPreCmds.AccessibleDescription")));
      this.check_prof_globPreCmds.AccessibleName = ((string)(resources.GetObject("check_prof_globPreCmds.AccessibleName")));
      this.check_prof_globPreCmds.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("check_prof_globPreCmds.Anchor")));
      this.check_prof_globPreCmds.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("check_prof_globPreCmds.Appearance")));
      this.check_prof_globPreCmds.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("check_prof_globPreCmds.BackgroundImage")));
      this.check_prof_globPreCmds.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_globPreCmds.CheckAlign")));
      this.check_prof_globPreCmds.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("check_prof_globPreCmds.Dock")));
      this.check_prof_globPreCmds.Enabled = ((bool)(resources.GetObject("check_prof_globPreCmds.Enabled")));
      this.check_prof_globPreCmds.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("check_prof_globPreCmds.FlatStyle")));
      this.check_prof_globPreCmds.Font = ((System.Drawing.Font)(resources.GetObject("check_prof_globPreCmds.Font")));
      this.check_prof_globPreCmds.Image = ((System.Drawing.Image)(resources.GetObject("check_prof_globPreCmds.Image")));
      this.check_prof_globPreCmds.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_globPreCmds.ImageAlign")));
      this.check_prof_globPreCmds.ImageIndex = ((int)(resources.GetObject("check_prof_globPreCmds.ImageIndex")));
      this.check_prof_globPreCmds.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("check_prof_globPreCmds.ImeMode")));
      this.check_prof_globPreCmds.Location = ((System.Drawing.Point)(resources.GetObject("check_prof_globPreCmds.Location")));
      this.check_prof_globPreCmds.Name = "check_prof_globPreCmds";
      this.check_prof_globPreCmds.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("check_prof_globPreCmds.RightToLeft")));
      this.check_prof_globPreCmds.Size = ((System.Drawing.Size)(resources.GetObject("check_prof_globPreCmds.Size")));
      this.check_prof_globPreCmds.TabIndex = ((int)(resources.GetObject("check_prof_globPreCmds.TabIndex")));
      this.check_prof_globPreCmds.Text = resources.GetString("check_prof_globPreCmds.Text");
      this.check_prof_globPreCmds.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("check_prof_globPreCmds.TextAlign")));
      this.toolTip.SetToolTip(this.check_prof_globPreCmds, resources.GetString("check_prof_globPreCmds.ToolTip"));
      this.check_prof_globPreCmds.Visible = ((bool)(resources.GetObject("check_prof_globPreCmds.Visible")));
      this.check_prof_globPreCmds.Enter += new System.EventHandler(this.text_any_Leave);
      // 
      // button_fileDialog
      // 
      this.button_fileDialog.AccessibleDescription = ((string)(resources.GetObject("button_fileDialog.AccessibleDescription")));
      this.button_fileDialog.AccessibleName = ((string)(resources.GetObject("button_fileDialog.AccessibleName")));
      this.button_fileDialog.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_fileDialog.Anchor")));
      this.button_fileDialog.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_fileDialog.BackgroundImage")));
      this.button_fileDialog.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_fileDialog.Dock")));
      this.button_fileDialog.Enabled = ((bool)(resources.GetObject("button_fileDialog.Enabled")));
      this.button_fileDialog.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_fileDialog.FlatStyle")));
      this.button_fileDialog.Font = ((System.Drawing.Font)(resources.GetObject("button_fileDialog.Font")));
      this.button_fileDialog.Image = ((System.Drawing.Image)(resources.GetObject("button_fileDialog.Image")));
      this.button_fileDialog.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_fileDialog.ImageAlign")));
      this.button_fileDialog.ImageIndex = ((int)(resources.GetObject("button_fileDialog.ImageIndex")));
      this.button_fileDialog.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_fileDialog.ImeMode")));
      this.button_fileDialog.Location = ((System.Drawing.Point)(resources.GetObject("button_fileDialog.Location")));
      this.button_fileDialog.Name = "button_fileDialog";
      this.button_fileDialog.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_fileDialog.RightToLeft")));
      this.button_fileDialog.Size = ((System.Drawing.Size)(resources.GetObject("button_fileDialog.Size")));
      this.button_fileDialog.TabIndex = ((int)(resources.GetObject("button_fileDialog.TabIndex")));
      this.button_fileDialog.Text = resources.GetString("button_fileDialog.Text");
      this.button_fileDialog.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_fileDialog.TextAlign")));
      this.toolTip.SetToolTip(this.button_fileDialog, resources.GetString("button_fileDialog.ToolTip"));
      this.button_fileDialog.Visible = ((bool)(resources.GetObject("button_fileDialog.Visible")));
      this.button_fileDialog.Click += new System.EventHandler(this.button_fileDialog_Click);
      // 
      // ofd
      // 
      this.ofd.Filter = resources.GetString("ofd.Filter");
      this.ofd.Title = resources.GetString("ofd.Title");
      this.ofd.FileOk += new System.ComponentModel.CancelEventHandler(this.ofd_FileOk);
      // 
      // FormCommands
      // 
      this.AccessibleDescription = ((string)(resources.GetObject("$this.AccessibleDescription")));
      this.AccessibleName = ((string)(resources.GetObject("$this.AccessibleName")));
      this.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("$this.Anchor")));
      this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
      this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
      this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
      this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
      this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
      this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.button_fileDialog,
                                                                  this.groupBox2,
                                                                  this.groupBox1,
                                                                  this.button_commands_ok,
                                                                  this.button_commands_cancel,
                                                                  this.label_cmdPostExeExit,
                                                                  this.label_cmdPreExeRun,
                                                                  this.button_commands_edit});
      this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
      this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
      this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
      this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
      this.MaximizeBox = false;
      this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
      this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
      this.Name = "FormCommands";
      this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
      this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
      this.Text = resources.GetString("$this.Text");
      this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
      this.Visible = ((bool)(resources.GetObject("$this.Visible")));
      this.groupBox1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion
    #region *** Widget Defintions ***
    private System.Windows.Forms.Label label_cmdPostExeExit;
    private System.Windows.Forms.Label label_cmdPreExeRun;
    private System.Windows.Forms.TextBox text_cmdPostExeExit;
    private System.Windows.Forms.TextBox text_cmdPreExeRun;
    private System.Windows.Forms.ComboBox combo_prof_names;
    private System.Windows.Forms.TextBox text_prof_cmdPostExeExit;
    private System.Windows.Forms.TextBox text_prof_cmdPreExeRun;
    private System.Windows.Forms.Button button_commands_ok;
    private System.Windows.Forms.Button button_commands_cancel;
    private System.Windows.Forms.Button button_commands_edit;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.CheckBox check_prof_globPreCmds;
    private System.Windows.Forms.CheckBox check_prof_globPostCmds;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.OpenFileDialog ofd;
    private System.Windows.Forms.Button button_fileDialog;
    private System.ComponentModel.IContainer components;
    #endregion
    #region *** Widget Callbacks ***
    private void button_commands_edit_Click(object sender, System.EventArgs e) {
      state_commands_edit(true);
    }

    private void button_commands_cancel_Click(object sender, System.EventArgs e) {
      state_commands_edit(false);
      GameProfileData gpd = this.get_selected_profile_data();
      if (gpd == null)
        return;

      init_from_gpd(gpd);
      init_from_ac();
      text_cmdPreExeRun.Focus();
    }

    private void button_commands_ok_Click(object sender, System.EventArgs e) {
      state_commands_edit(false);

      // global commands
      ax.ac.command_pre_exe_run = text_cmdPreExeRun.Text;
      ax.ac.command_post_exe_exit = text_cmdPostExeExit.Text;
      ax.ac.save_config(); // save global config which contains global commands

      // profile commands
      GameProfileData gpd = this.get_selected_profile_data();
      if (gpd == null)
        return;

      // set prof_changed to true or keeep old value
      if (gpd.command_pre_exe_run         != text_prof_cmdPreExeRun.Text
        || gpd.command_post_exe_exit      != text_prof_cmdPostExeExit.Text
        || gpd.command_pre_exe_run_glob   != check_prof_globPreCmds.Checked
        || gpd.command_post_exe_exit_glob != check_prof_globPostCmds.Checked)
        prof_changed = true;

      gpd.command_pre_exe_run = text_prof_cmdPreExeRun.Text;
      gpd.command_post_exe_exit = text_prof_cmdPostExeExit.Text;
      gpd.command_pre_exe_run_glob = check_prof_globPreCmds.Checked;
      gpd.command_post_exe_exit_glob = check_prof_globPostCmds.Checked;

      if (prof_changed) {
        if (!prof_unsaved) {
          ax.gp.save_profiles("profiles.cfg");
          prof_unsaved = prof_changed = false;
        } else {
          System.Windows.Forms.DialogResult dr =
            System.Windows.Forms.MessageBox
            .Show("Do you want to save profiles? This will save all changes, including those made before entering the Commands dialog", G.app_name, System.Windows.Forms.MessageBoxButtons.YesNo);
          if (dr == System.Windows.Forms.DialogResult.Yes) {
            ax.gp.save_profiles("profiles.cfg");
            prof_unsaved = prof_changed = false;
          } else if (dr == System.Windows.Forms.DialogResult.No) {
          } else if (dr == System.Windows.Forms.DialogResult.Cancel) {
          }
        }
      }
    }

    private void button_fileDialog_Click(object sender, System.EventArgs e) {
      ofd.ShowDialog();
    }

    private void combo_prof_names_SelectedIndexChanged(object sender, System.EventArgs e) {
      GameProfileData gpd = get_selected_profile_data();
      init_from_gpd(gpd);
    }

    private void text_any_Enter(object sender, System.EventArgs e) {
      tb_with_focus = (TextBox)sender;
      button_fileDialog.Enabled = true;
    }

    private void text_any_Leave(object sender, System.EventArgs e) {
      tb_with_focus = null;
      button_fileDialog.Enabled = false;
    }
    private void ofd_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {
      TextBox tb = tb_with_focus;
      if (tb == null)
        return;
      string file_name = ofd.FileName;
      if (file_name.IndexOf(" ") != -1)
        file_name = "\"" + file_name + "\"";
      string text = tb.Text;
      if (text != "" && !text.EndsWith("\r\n"))
        text += "\r\n";
      text += file_name;
      tb.Text = text;

    }

    #endregion


    private GameProfileData get_selected_profile_data() {
      int idx = combo_prof_names.SelectedIndex;
      if (0 <= idx)
        return ax.gp.get_profile(idx);
      return null;
    }

    void state_commands_edit(bool p) {
      GameProfileData gpd = this.get_selected_profile_data();

      text_cmdPostExeExit.ReadOnly
        = text_cmdPreExeRun.ReadOnly
        = !p;
        text_prof_cmdPostExeExit.ReadOnly
        = text_prof_cmdPreExeRun.ReadOnly
        = !(p && gpd != null && gpd.exe_path.Length > 0);
      check_prof_globPreCmds.Enabled
        = check_prof_globPostCmds.Enabled
        = p;
      button_commands_edit.Visible = !p;
      button_commands_cancel.Visible
        = button_commands_ok.Visible
        = button_fileDialog.Visible
        = p;
      combo_prof_names.Enabled = !p;
    }

    TextBox tb_with_focus = null;
 
  }
}
