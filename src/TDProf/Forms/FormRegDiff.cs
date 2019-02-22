using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace TDProf {
	/// <summary>
	/// Summary description for RegDiffForm.
	/// </summary>
	public class RegDiffForm : System.Windows.Forms.Form
	{
    #region Private Data

      RegDiff.RegDiff rdiff;// = new RegDiff.RegDiff(Registry.LocalMachine, G.di.get_driver_key());
      static StreamWriter sw = null;
      static FileStream os = null;
      static int instance_counter = 0;
      bool instance_disposed = false;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
    #endregion

		public RegDiffForm(RegistryKey base_key, string sub_key_name, int timer_interval_ms)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

      if (instance_counter++ == 0) {

        try {
          os = new FileStream("RegDiffOut.txt", FileMode.Append, FileAccess.Write);
          sw = new StreamWriter(os);
        } catch {
          if (os != null)
            os.Close();
        }
      }
          rdiff = new RegDiff.RegDiff(base_key, sub_key_name);
          if (timer_interval_ms > 0)
            timer_regDiff.Interval = timer_interval_ms;

          text_message.Text += "\r\nKey: " + base_key.Name + @"\" + sub_key_name + "\r\n"
            + "Logfile: " + (os == null ? "none" : os.Name) + "\r\n"
            + "\r\n";
          text_message.SelectionStart = text_message.TextLength;
          text_message.ScrollToCaret();
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

      if (!instance_disposed) {
        instance_disposed = true;
        if (--instance_counter == 0) {
          if (sw != null) { sw.Close(); sw = null; }
          if (os != null) { os.Close(); os = null; }
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
          System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(RegDiffForm));
          this.text_message = new System.Windows.Forms.TextBox();
          this.timer_regDiff = new System.Timers.Timer();
          ((System.ComponentModel.ISupportInitialize)(this.timer_regDiff)).BeginInit();
          this.SuspendLayout();
          // 
          // text_message
          // 
          this.text_message.AccessibleDescription = ((string)(resources.GetObject("text_message.AccessibleDescription")));
          this.text_message.AccessibleName = ((string)(resources.GetObject("text_message.AccessibleName")));
          this.text_message.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("text_message.Anchor")));
          this.text_message.AutoSize = ((bool)(resources.GetObject("text_message.AutoSize")));
          this.text_message.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("text_message.BackgroundImage")));
          this.text_message.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("text_message.Dock")));
          this.text_message.Enabled = ((bool)(resources.GetObject("text_message.Enabled")));
          this.text_message.Font = ((System.Drawing.Font)(resources.GetObject("text_message.Font")));
          this.text_message.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("text_message.ImeMode")));
          this.text_message.Location = ((System.Drawing.Point)(resources.GetObject("text_message.Location")));
          this.text_message.MaxLength = ((int)(resources.GetObject("text_message.MaxLength")));
          this.text_message.Multiline = ((bool)(resources.GetObject("text_message.Multiline")));
          this.text_message.Name = "text_message";
          this.text_message.PasswordChar = ((char)(resources.GetObject("text_message.PasswordChar")));
          this.text_message.ReadOnly = true;
          this.text_message.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("text_message.RightToLeft")));
          this.text_message.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("text_message.ScrollBars")));
          this.text_message.Size = ((System.Drawing.Size)(resources.GetObject("text_message.Size")));
          this.text_message.TabIndex = ((int)(resources.GetObject("text_message.TabIndex")));
          this.text_message.Text = resources.GetString("text_message.Text");
          this.text_message.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("text_message.TextAlign")));
          this.text_message.Visible = ((bool)(resources.GetObject("text_message.Visible")));
          this.text_message.WordWrap = ((bool)(resources.GetObject("text_message.WordWrap")));
          this.text_message.Resize += new System.EventHandler(this.text_message_Resize);
          this.text_message.MouseDown += new System.Windows.Forms.MouseEventHandler(this.text_message_MouseDown);
          // 
          // timer_regDiff
          // 
          this.timer_regDiff.Enabled = true;
          this.timer_regDiff.Interval = 1000;
          this.timer_regDiff.SynchronizingObject = this.text_message;
          this.timer_regDiff.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_regDiff_Elapsed);
          // 
          // RegDiffForm
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
                                                                      this.text_message});
          this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
          this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
          this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
          this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
          this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
          this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
          this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
          this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
          this.Name = "RegDiffForm";
          this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
          this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
          this.Text = resources.GetString("$this.Text");
          this.Visible = ((bool)(resources.GetObject("$this.Visible")));
          this.Closed += new System.EventHandler(this.RegDiffForm_Closed);
          ((System.ComponentModel.ISupportInitialize)(this.timer_regDiff)).EndInit();
          this.ResumeLayout(false);

        }
		#endregion
    #region *** Widget Defintions ***
    private System.Windows.Forms.TextBox text_message;
    private System.Timers.Timer timer_regDiff;
    #endregion WIDGET_DEFS
    #region *** Widget Callbacks ***
      private void timer_regDiff_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
        string msg = rdiff.test_rdkey();
        if (msg != null) {
          msg = "\r\n* Time: " + System.DateTime.Now.ToLongTimeString()
            + " Date: " + System.DateTime.Now.ToShortDateString()
            + "\r\n" + msg;
          text_message.Text += msg;
          text_message.SelectionStart = text_message.TextLength;
          text_message.ScrollToCaret();

          if (sw != null)
            sw.Write(msg);
         

        }
      }

      private void RegDiffForm_Closed(object sender, System.EventArgs e) {
        timer_regDiff.Dispose();
        if (sw != null) sw.Close();
        else if (os != null) os.Close();
        this.Dispose();
      }

      private void text_message_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
         timer_regDiff_Elapsed(null, null);
      }

      private void text_message_Resize(object sender, System.EventArgs e) {
        timer_regDiff_Elapsed(null, null); 
      }

    #endregion
	}
}
