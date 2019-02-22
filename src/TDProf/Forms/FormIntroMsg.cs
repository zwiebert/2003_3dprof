using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TDProf.Forms {
  /// <summary>
  /// Summary description for FormIntroMsg.
  /// </summary>
  public class FormIntroMsg : System.Windows.Forms.Form {
    private System.Windows.Forms.Button button_close;
    private System.Windows.Forms.CheckBox check_hideNext;
    private System.Windows.Forms.Label label_message;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public FormIntroMsg() {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //
      // TODO: Add any constructor code after InitializeComponent call
      //
    }


    public FormIntroMsg resize(int width, int height) {
      this.Size = new System.Drawing.Size(width, height);
      return this;
    }

    public static FormIntroMsg open_dialog(string msg, string title, ref bool hideNext) {
      FormIntroMsg f = new FormIntroMsg();
      f.label_message.Text = msg;
      f.Text = /*G.app_name + " - " +*/ title;
      f.button_close.Focus();
      //f.button_ok.Text = "Abort";
      return f;
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
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormIntroMsg));
      this.button_close = new System.Windows.Forms.Button();
      this.check_hideNext = new System.Windows.Forms.CheckBox();
      this.label_message = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // button_close
      // 
      this.button_close.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.button_close.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.button_close.Location = new System.Drawing.Point(8, 144);
      this.button_close.Name = "button_close";
      this.button_close.Size = new System.Drawing.Size(224, 23);
      this.button_close.TabIndex = 32;
      this.button_close.Text = "Close";
      // 
      // check_hideNext
      // 
      this.check_hideNext.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.check_hideNext.Location = new System.Drawing.Point(8, 104);
      this.check_hideNext.Name = "check_hideNext";
      this.check_hideNext.Size = new System.Drawing.Size(220, 24);
      this.check_hideNext.TabIndex = 33;
      this.check_hideNext.Text = "Never Show This Window Again";
      // 
      // label_message
      // 
      this.label_message.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.label_message.Location = new System.Drawing.Point(8, 16);
      this.label_message.Name = "label_message";
      this.label_message.Size = new System.Drawing.Size(228, 80);
      this.label_message.TabIndex = 34;
      // 
      // FormIntroMsg
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(240, 173);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.label_message,
                                                                  this.check_hideNext,
                                                                  this.button_close});
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormIntroMsg";
      this.Text = "3DProf - Info";
      this.ResumeLayout(false);

    }
		#endregion
  }
}
