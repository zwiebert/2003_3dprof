using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TDProf {
  /// <summary>
  /// Summary description for MessageForm.
  /// </summary>
  public class MessageForm : System.Windows.Forms.Form {
    private System.Windows.Forms.TextBox text_message;
    private System.Windows.Forms.Button button_ok;
    private System.Windows.Forms.Button button_cancel;
    private System.Windows.Forms.Button button_copy2clipboard;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public MessageForm() {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //
      // TODO: Add any constructor code after InitializeComponent call
      //
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
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MessageForm));
      this.text_message = new System.Windows.Forms.TextBox();
      this.button_ok = new System.Windows.Forms.Button();
      this.button_cancel = new System.Windows.Forms.Button();
      this.button_copy2clipboard = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // text_message
      // 
      this.text_message.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.text_message.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.text_message.Location = new System.Drawing.Point(8, 8);
      this.text_message.Multiline = true;
      this.text_message.Name = "text_message";
      this.text_message.ReadOnly = true;
      this.text_message.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.text_message.Size = new System.Drawing.Size(264, 128);
      this.text_message.TabIndex = 0;
      this.text_message.Text = "";
      // 
      // button_ok
      // 
      this.button_ok.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
      this.button_ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.button_ok.Location = new System.Drawing.Point(8, 144);
      this.button_ok.Name = "button_ok";
      this.button_ok.Size = new System.Drawing.Size(80, 23);
      this.button_ok.TabIndex = 31;
      this.button_ok.Text = "Ok";
      this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
      // 
      // button_cancel
      // 
      this.button_cancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
      this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button_cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.button_cancel.Location = new System.Drawing.Point(192, 144);
      this.button_cancel.Name = "button_cancel";
      this.button_cancel.Size = new System.Drawing.Size(80, 23);
      this.button_cancel.TabIndex = 32;
      this.button_cancel.Text = "Cancel";
      this.button_cancel.Visible = false;
      // 
      // button_copy2clipboard
      // 
      this.button_copy2clipboard.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.button_copy2clipboard.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.button_copy2clipboard.Location = new System.Drawing.Point(96, 144);
      this.button_copy2clipboard.Name = "button_copy2clipboard";
      this.button_copy2clipboard.Size = new System.Drawing.Size(88, 23);
      this.button_copy2clipboard.TabIndex = 33;
      this.button_copy2clipboard.Text = "To Clipobard";
      this.button_copy2clipboard.Click += new System.EventHandler(this.button_copy2clipboard_Click);
      // 
      // MessageForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(280, 182);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.button_ok,
                                                                  this.button_cancel,
                                                                  this.text_message,
                                                                  this.button_copy2clipboard});
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MessageForm";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "3DProf";
      this.ResumeLayout(false);

    }
		#endregion

    private void button_ok_Click(object sender, System.EventArgs e) {
      this.Close();
    }

    public MessageForm resize(int width, int height) {
      this.Size = new System.Drawing.Size(width, height);
      return this;
    }

    public static MessageForm open_dialog(string msg, string title) {
      MessageForm f = new MessageForm();
      f.text_message.Text = msg;
      f.text_message.Select(0,0);
      //f.text_message.
      f.Text = /*G.app_name + " - " +*/ title;
      f.button_ok.Focus();
      //f.button_ok.Text = "Abort";
      return f;
    }

    private void button_copy2clipboard_Click(object sender, System.EventArgs e) {
      System.Windows.Forms.Clipboard.SetDataObject("[code]\r\n" + text_message.Text + "[/code]\r\n");
    }
  }
}
