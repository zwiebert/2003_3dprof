using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TDProf {
  /// <summary>
  /// Summary description for FormImageChooser.
  /// </summary>
  public class FormImageChooser : System.Windows.Forms.Form {
    #region Private Data
    string[] m_result;
    ComboBox[] combos_dn;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;
    #endregion

    public FormImageChooser(string[] result, string[][] images) {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //
      // TODO: Add any constructor code after InitializeComponent call
      //
      m_result = result;
      combos_dn = new ComboBox[] { combo_img_dn0, combo_img_dn1, combo_img_dn2, combo_img_dn3 };

      for (int i=0; i < 4; ++i) {
        ComboBox cb = combos_dn[i];
        if (images[i] == null) {
          cb.Enabled = false;
          cb.DropDownStyle = ComboBoxStyle.Simple;
        } else {
          cb.Items.AddRange(images[i]);
          cb.SelectedIndex = 0;
          if (images[i].Length == 1) {
            cb.DropDownStyle = ComboBoxStyle.Simple;
            cb.Enabled = false;
          } else if (images[i].Length > 1)  {
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
          }
        }
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
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormImageChooser));
      this.combo_img_dn0 = new System.Windows.Forms.ComboBox();
      this.label_img_dn0 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.combo_img_dn1 = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.combo_img_dn2 = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.combo_img_dn3 = new System.Windows.Forms.ComboBox();
      this.button_ok = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // combo_img_dn0
      // 
      this.combo_img_dn0.AccessibleDescription = ((string)(resources.GetObject("combo_img_dn0.AccessibleDescription")));
      this.combo_img_dn0.AccessibleName = ((string)(resources.GetObject("combo_img_dn0.AccessibleName")));
      this.combo_img_dn0.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_img_dn0.Anchor")));
      this.combo_img_dn0.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_img_dn0.BackgroundImage")));
      this.combo_img_dn0.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_img_dn0.Dock")));
      this.combo_img_dn0.Enabled = ((bool)(resources.GetObject("combo_img_dn0.Enabled")));
      this.combo_img_dn0.Font = ((System.Drawing.Font)(resources.GetObject("combo_img_dn0.Font")));
      this.combo_img_dn0.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_img_dn0.ImeMode")));
      this.combo_img_dn0.IntegralHeight = ((bool)(resources.GetObject("combo_img_dn0.IntegralHeight")));
      this.combo_img_dn0.ItemHeight = ((int)(resources.GetObject("combo_img_dn0.ItemHeight")));
      this.combo_img_dn0.Location = ((System.Drawing.Point)(resources.GetObject("combo_img_dn0.Location")));
      this.combo_img_dn0.MaxDropDownItems = ((int)(resources.GetObject("combo_img_dn0.MaxDropDownItems")));
      this.combo_img_dn0.MaxLength = ((int)(resources.GetObject("combo_img_dn0.MaxLength")));
      this.combo_img_dn0.Name = "combo_img_dn0";
      this.combo_img_dn0.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_img_dn0.RightToLeft")));
      this.combo_img_dn0.Size = ((System.Drawing.Size)(resources.GetObject("combo_img_dn0.Size")));
      this.combo_img_dn0.TabIndex = ((int)(resources.GetObject("combo_img_dn0.TabIndex")));
      this.combo_img_dn0.Text = resources.GetString("combo_img_dn0.Text");
      this.combo_img_dn0.Visible = ((bool)(resources.GetObject("combo_img_dn0.Visible")));
      // 
      // label_img_dn0
      // 
      this.label_img_dn0.AccessibleDescription = ((string)(resources.GetObject("label_img_dn0.AccessibleDescription")));
      this.label_img_dn0.AccessibleName = ((string)(resources.GetObject("label_img_dn0.AccessibleName")));
      this.label_img_dn0.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label_img_dn0.Anchor")));
      this.label_img_dn0.AutoSize = ((bool)(resources.GetObject("label_img_dn0.AutoSize")));
      this.label_img_dn0.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label_img_dn0.Dock")));
      this.label_img_dn0.Enabled = ((bool)(resources.GetObject("label_img_dn0.Enabled")));
      this.label_img_dn0.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.label_img_dn0.Font = ((System.Drawing.Font)(resources.GetObject("label_img_dn0.Font")));
      this.label_img_dn0.Image = ((System.Drawing.Image)(resources.GetObject("label_img_dn0.Image")));
      this.label_img_dn0.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_img_dn0.ImageAlign")));
      this.label_img_dn0.ImageIndex = ((int)(resources.GetObject("label_img_dn0.ImageIndex")));
      this.label_img_dn0.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label_img_dn0.ImeMode")));
      this.label_img_dn0.Location = ((System.Drawing.Point)(resources.GetObject("label_img_dn0.Location")));
      this.label_img_dn0.Name = "label_img_dn0";
      this.label_img_dn0.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label_img_dn0.RightToLeft")));
      this.label_img_dn0.Size = ((System.Drawing.Size)(resources.GetObject("label_img_dn0.Size")));
      this.label_img_dn0.TabIndex = ((int)(resources.GetObject("label_img_dn0.TabIndex")));
      this.label_img_dn0.Text = resources.GetString("label_img_dn0.Text");
      this.label_img_dn0.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label_img_dn0.TextAlign")));
      this.label_img_dn0.Visible = ((bool)(resources.GetObject("label_img_dn0.Visible")));
      // 
      // label1
      // 
      this.label1.AccessibleDescription = ((string)(resources.GetObject("label1.AccessibleDescription")));
      this.label1.AccessibleName = ((string)(resources.GetObject("label1.AccessibleName")));
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
      this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
      this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
      this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
      this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
      this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
      // 
      // combo_img_dn1
      // 
      this.combo_img_dn1.AccessibleDescription = ((string)(resources.GetObject("combo_img_dn1.AccessibleDescription")));
      this.combo_img_dn1.AccessibleName = ((string)(resources.GetObject("combo_img_dn1.AccessibleName")));
      this.combo_img_dn1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_img_dn1.Anchor")));
      this.combo_img_dn1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_img_dn1.BackgroundImage")));
      this.combo_img_dn1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_img_dn1.Dock")));
      this.combo_img_dn1.Enabled = ((bool)(resources.GetObject("combo_img_dn1.Enabled")));
      this.combo_img_dn1.Font = ((System.Drawing.Font)(resources.GetObject("combo_img_dn1.Font")));
      this.combo_img_dn1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_img_dn1.ImeMode")));
      this.combo_img_dn1.IntegralHeight = ((bool)(resources.GetObject("combo_img_dn1.IntegralHeight")));
      this.combo_img_dn1.ItemHeight = ((int)(resources.GetObject("combo_img_dn1.ItemHeight")));
      this.combo_img_dn1.Location = ((System.Drawing.Point)(resources.GetObject("combo_img_dn1.Location")));
      this.combo_img_dn1.MaxDropDownItems = ((int)(resources.GetObject("combo_img_dn1.MaxDropDownItems")));
      this.combo_img_dn1.MaxLength = ((int)(resources.GetObject("combo_img_dn1.MaxLength")));
      this.combo_img_dn1.Name = "combo_img_dn1";
      this.combo_img_dn1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_img_dn1.RightToLeft")));
      this.combo_img_dn1.Size = ((System.Drawing.Size)(resources.GetObject("combo_img_dn1.Size")));
      this.combo_img_dn1.TabIndex = ((int)(resources.GetObject("combo_img_dn1.TabIndex")));
      this.combo_img_dn1.Text = resources.GetString("combo_img_dn1.Text");
      this.combo_img_dn1.Visible = ((bool)(resources.GetObject("combo_img_dn1.Visible")));
      // 
      // label2
      // 
      this.label2.AccessibleDescription = ((string)(resources.GetObject("label2.AccessibleDescription")));
      this.label2.AccessibleName = ((string)(resources.GetObject("label2.AccessibleName")));
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
      this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
      this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
      this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
      this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
      this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
      // 
      // combo_img_dn2
      // 
      this.combo_img_dn2.AccessibleDescription = ((string)(resources.GetObject("combo_img_dn2.AccessibleDescription")));
      this.combo_img_dn2.AccessibleName = ((string)(resources.GetObject("combo_img_dn2.AccessibleName")));
      this.combo_img_dn2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_img_dn2.Anchor")));
      this.combo_img_dn2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_img_dn2.BackgroundImage")));
      this.combo_img_dn2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_img_dn2.Dock")));
      this.combo_img_dn2.Enabled = ((bool)(resources.GetObject("combo_img_dn2.Enabled")));
      this.combo_img_dn2.Font = ((System.Drawing.Font)(resources.GetObject("combo_img_dn2.Font")));
      this.combo_img_dn2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_img_dn2.ImeMode")));
      this.combo_img_dn2.IntegralHeight = ((bool)(resources.GetObject("combo_img_dn2.IntegralHeight")));
      this.combo_img_dn2.ItemHeight = ((int)(resources.GetObject("combo_img_dn2.ItemHeight")));
      this.combo_img_dn2.Location = ((System.Drawing.Point)(resources.GetObject("combo_img_dn2.Location")));
      this.combo_img_dn2.MaxDropDownItems = ((int)(resources.GetObject("combo_img_dn2.MaxDropDownItems")));
      this.combo_img_dn2.MaxLength = ((int)(resources.GetObject("combo_img_dn2.MaxLength")));
      this.combo_img_dn2.Name = "combo_img_dn2";
      this.combo_img_dn2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_img_dn2.RightToLeft")));
      this.combo_img_dn2.Size = ((System.Drawing.Size)(resources.GetObject("combo_img_dn2.Size")));
      this.combo_img_dn2.TabIndex = ((int)(resources.GetObject("combo_img_dn2.TabIndex")));
      this.combo_img_dn2.Text = resources.GetString("combo_img_dn2.Text");
      this.combo_img_dn2.Visible = ((bool)(resources.GetObject("combo_img_dn2.Visible")));
      // 
      // label3
      // 
      this.label3.AccessibleDescription = ((string)(resources.GetObject("label3.AccessibleDescription")));
      this.label3.AccessibleName = ((string)(resources.GetObject("label3.AccessibleName")));
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label3.Anchor")));
      this.label3.AutoSize = ((bool)(resources.GetObject("label3.AutoSize")));
      this.label3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label3.Dock")));
      this.label3.Enabled = ((bool)(resources.GetObject("label3.Enabled")));
      this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
      this.label3.Visible = ((bool)(resources.GetObject("label3.Visible")));
      // 
      // combo_img_dn3
      // 
      this.combo_img_dn3.AccessibleDescription = ((string)(resources.GetObject("combo_img_dn3.AccessibleDescription")));
      this.combo_img_dn3.AccessibleName = ((string)(resources.GetObject("combo_img_dn3.AccessibleName")));
      this.combo_img_dn3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("combo_img_dn3.Anchor")));
      this.combo_img_dn3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("combo_img_dn3.BackgroundImage")));
      this.combo_img_dn3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("combo_img_dn3.Dock")));
      this.combo_img_dn3.Enabled = ((bool)(resources.GetObject("combo_img_dn3.Enabled")));
      this.combo_img_dn3.Font = ((System.Drawing.Font)(resources.GetObject("combo_img_dn3.Font")));
      this.combo_img_dn3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("combo_img_dn3.ImeMode")));
      this.combo_img_dn3.IntegralHeight = ((bool)(resources.GetObject("combo_img_dn3.IntegralHeight")));
      this.combo_img_dn3.ItemHeight = ((int)(resources.GetObject("combo_img_dn3.ItemHeight")));
      this.combo_img_dn3.Location = ((System.Drawing.Point)(resources.GetObject("combo_img_dn3.Location")));
      this.combo_img_dn3.MaxDropDownItems = ((int)(resources.GetObject("combo_img_dn3.MaxDropDownItems")));
      this.combo_img_dn3.MaxLength = ((int)(resources.GetObject("combo_img_dn3.MaxLength")));
      this.combo_img_dn3.Name = "combo_img_dn3";
      this.combo_img_dn3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("combo_img_dn3.RightToLeft")));
      this.combo_img_dn3.Size = ((System.Drawing.Size)(resources.GetObject("combo_img_dn3.Size")));
      this.combo_img_dn3.TabIndex = ((int)(resources.GetObject("combo_img_dn3.TabIndex")));
      this.combo_img_dn3.Text = resources.GetString("combo_img_dn3.Text");
      this.combo_img_dn3.Visible = ((bool)(resources.GetObject("combo_img_dn3.Visible")));
      // 
      // button_ok
      // 
      this.button_ok.AccessibleDescription = ((string)(resources.GetObject("button_ok.AccessibleDescription")));
      this.button_ok.AccessibleName = ((string)(resources.GetObject("button_ok.AccessibleName")));
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
      this.button_ok.Visible = ((bool)(resources.GetObject("button_ok.Visible")));
      this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
      // 
      // FormImageChooser
      // 
      this.AcceptButton = this.button_ok;
      this.AccessibleDescription = ((string)(resources.GetObject("$this.AccessibleDescription")));
      this.AccessibleName = ((string)(resources.GetObject("$this.AccessibleName")));
      this.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("$this.Anchor")));
      this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
      this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
      this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
      this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
      this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
      this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
      this.ControlBox = false;
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.button_ok,
                                                                  this.label3,
                                                                  this.combo_img_dn3,
                                                                  this.label2,
                                                                  this.combo_img_dn2,
                                                                  this.label1,
                                                                  this.combo_img_dn1,
                                                                  this.label_img_dn0,
                                                                  this.combo_img_dn0});
      this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
      this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
      this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
      this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
      this.MaximizeBox = false;
      this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
      this.MinimizeBox = false;
      this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
      this.Name = "FormImageChooser";
      this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
      this.Text = resources.GetString("$this.Text");
      this.Visible = ((bool)(resources.GetObject("$this.Visible")));
      this.Load += new System.EventHandler(this.FormImageChooser_Load);
      this.ResumeLayout(false);

    }
		#endregion
    #region *** Widget Defintions ***
    private System.Windows.Forms.ComboBox combo_img_dn0;
    private System.Windows.Forms.Label label_img_dn0;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button button_ok;
    private System.Windows.Forms.ComboBox combo_img_dn1;
    private System.Windows.Forms.ComboBox combo_img_dn2;
    private System.Windows.Forms.ComboBox combo_img_dn3;
    #endregion WIDGET_DEFS
    #region *** Widget Callbacks ***
    private void FormImageChooser_Load(object sender, System.EventArgs e) {
      
    }

    private void button_ok_Click(object sender, System.EventArgs e) {
      for (int i=0; i < 4; ++i) {
        ComboBox cb = combos_dn[i];
        if (cb.SelectedIndex != -1)
          m_result[i] = cb.Text;
      }
      this.Close();
    }

    #endregion

  }
}
