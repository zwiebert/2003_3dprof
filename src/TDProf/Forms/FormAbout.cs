using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TDProf
{
	/// <summary>
	/// Summary description for AboutForm.
	/// </summary>
	public class AboutForm : System.Windows.Forms.Form
	{
      private System.Windows.Forms.RichTextBox rText;
      private System.Windows.Forms.PictureBox pictureBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
      rText.Text = rText.Text.Replace("$version$", G.app_version);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
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
		private void InitializeComponent()
		{
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AboutForm));
      this.rText = new System.Windows.Forms.RichTextBox();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.SuspendLayout();
      // 
      // rText
      // 
      this.rText.AccessibleDescription = ((string)(resources.GetObject("rText.AccessibleDescription")));
      this.rText.AccessibleName = ((string)(resources.GetObject("rText.AccessibleName")));
      this.rText.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rText.Anchor")));
      this.rText.AutoSize = ((bool)(resources.GetObject("rText.AutoSize")));
      this.rText.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rText.BackgroundImage")));
      this.rText.BulletIndent = ((int)(resources.GetObject("rText.BulletIndent")));
      this.rText.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rText.Dock")));
      this.rText.Enabled = ((bool)(resources.GetObject("rText.Enabled")));
      this.rText.Font = ((System.Drawing.Font)(resources.GetObject("rText.Font")));
      this.rText.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rText.ImeMode")));
      this.rText.Location = ((System.Drawing.Point)(resources.GetObject("rText.Location")));
      this.rText.MaxLength = ((int)(resources.GetObject("rText.MaxLength")));
      this.rText.Multiline = ((bool)(resources.GetObject("rText.Multiline")));
      this.rText.Name = "rText";
      this.rText.ReadOnly = true;
      this.rText.RightMargin = ((int)(resources.GetObject("rText.RightMargin")));
      this.rText.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rText.RightToLeft")));
      this.rText.ScrollBars = ((System.Windows.Forms.RichTextBoxScrollBars)(resources.GetObject("rText.ScrollBars")));
      this.rText.Size = ((System.Drawing.Size)(resources.GetObject("rText.Size")));
      this.rText.TabIndex = ((int)(resources.GetObject("rText.TabIndex")));
      this.rText.Text = resources.GetString("rText.Text");
      this.rText.Visible = ((bool)(resources.GetObject("rText.Visible")));
      this.rText.WordWrap = ((bool)(resources.GetObject("rText.WordWrap")));
      this.rText.ZoomFactor = ((System.Single)(resources.GetObject("rText.ZoomFactor")));
      // 
      // pictureBox1
      // 
      this.pictureBox1.AccessibleDescription = ((string)(resources.GetObject("pictureBox1.AccessibleDescription")));
      this.pictureBox1.AccessibleName = ((string)(resources.GetObject("pictureBox1.AccessibleName")));
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("pictureBox1.Anchor")));
      this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
      this.pictureBox1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("pictureBox1.Dock")));
      this.pictureBox1.Enabled = ((bool)(resources.GetObject("pictureBox1.Enabled")));
      this.pictureBox1.Font = ((System.Drawing.Font)(resources.GetObject("pictureBox1.Font")));
      this.pictureBox1.Image = ((System.Drawing.Bitmap)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("pictureBox1.ImeMode")));
      this.pictureBox1.Location = ((System.Drawing.Point)(resources.GetObject("pictureBox1.Location")));
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("pictureBox1.RightToLeft")));
      this.pictureBox1.Size = ((System.Drawing.Size)(resources.GetObject("pictureBox1.Size")));
      this.pictureBox1.SizeMode = ((System.Windows.Forms.PictureBoxSizeMode)(resources.GetObject("pictureBox1.SizeMode")));
      this.pictureBox1.TabIndex = ((int)(resources.GetObject("pictureBox1.TabIndex")));
      this.pictureBox1.TabStop = false;
      this.pictureBox1.Text = resources.GetString("pictureBox1.Text");
      this.pictureBox1.Visible = ((bool)(resources.GetObject("pictureBox1.Visible")));
      // 
      // AboutForm
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
                                                                  this.pictureBox1,
                                                                  this.rText});
      this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
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
      this.Name = "AboutForm";
      this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
      this.ShowInTaskbar = false;
      this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
      this.Text = resources.GetString("$this.Text");
      this.TopMost = true;
      this.Visible = ((bool)(resources.GetObject("$this.Visible")));
      this.ResumeLayout(false);

    }
		#endregion

	}
}
