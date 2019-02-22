using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TDProf.Forms
{
	/// <summary>
	/// Summary description for FormProfileMassEdit.
	/// </summary>
	public class FormProfileMassEdit : System.Windows.Forms.Form
	{
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox combo_act_moveToSpecName;
    private System.Windows.Forms.Button button_act_move;
    private System.Windows.Forms.Button button_act_copy;
    private System.Windows.Forms.ComboBox combo_act_copyToSpecName;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox combo_sel_specName;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox combo_sel_profName;
    private System.Windows.Forms.Button button_act_delete;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TextBox text_sel_count;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormProfileMassEdit()
		{
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
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormProfileMassEdit));
      this.label1 = new System.Windows.Forms.Label();
      this.combo_act_moveToSpecName = new System.Windows.Forms.ComboBox();
      this.button_act_move = new System.Windows.Forms.Button();
      this.button_act_copy = new System.Windows.Forms.Button();
      this.combo_act_copyToSpecName = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.combo_sel_specName = new System.Windows.Forms.ComboBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.combo_sel_profName = new System.Windows.Forms.ComboBox();
      this.button_act_delete = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.text_sel_count = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(8, 32);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(224, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Move selected profiles to graphic adapter";
      // 
      // combo_act_moveToSpecName
      // 
      this.combo_act_moveToSpecName.Location = new System.Drawing.Point(232, 32);
      this.combo_act_moveToSpecName.Name = "combo_act_moveToSpecName";
      this.combo_act_moveToSpecName.Size = new System.Drawing.Size(121, 21);
      this.combo_act_moveToSpecName.TabIndex = 1;
      // 
      // button_act_move
      // 
      this.button_act_move.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.button_act_move.Location = new System.Drawing.Point(360, 32);
      this.button_act_move.Name = "button_act_move";
      this.button_act_move.TabIndex = 2;
      this.button_act_move.Text = "Move";
      // 
      // button_act_copy
      // 
      this.button_act_copy.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.button_act_copy.Location = new System.Drawing.Point(360, 56);
      this.button_act_copy.Name = "button_act_copy";
      this.button_act_copy.TabIndex = 5;
      this.button_act_copy.Text = "Copy";
      // 
      // combo_act_copyToSpecName
      // 
      this.combo_act_copyToSpecName.Location = new System.Drawing.Point(232, 56);
      this.combo_act_copyToSpecName.Name = "combo_act_copyToSpecName";
      this.combo_act_copyToSpecName.Size = new System.Drawing.Size(121, 21);
      this.combo_act_copyToSpecName.TabIndex = 4;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(8, 56);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(224, 16);
      this.label2.TabIndex = 3;
      this.label2.Text = "Copy selected profiles to graphic adapter";
      // 
      // combo_sel_specName
      // 
      this.combo_sel_specName.Location = new System.Drawing.Point(232, 16);
      this.combo_sel_specName.Name = "combo_sel_specName";
      this.combo_sel_specName.Size = new System.Drawing.Size(121, 21);
      this.combo_sel_specName.TabIndex = 6;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.button2);
      this.groupBox1.Controls.Add(this.button1);
      this.groupBox1.Controls.Add(this.label6);
      this.groupBox1.Controls.Add(this.text_sel_count);
      this.groupBox1.Controls.Add(this.combo_sel_profName);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.combo_sel_specName);
      this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.groupBox1.Location = new System.Drawing.Point(8, 8);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(448, 144);
      this.groupBox1.TabIndex = 7;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "1) Select Profiles";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(8, 24);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(224, 16);
      this.label3.TabIndex = 7;
      this.label3.Text = "by related graphic adapter";
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(8, 48);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(224, 16);
      this.label4.TabIndex = 8;
      this.label4.Text = "by profile name (wildcards allowed)";
      // 
      // combo_sel_profName
      // 
      this.combo_sel_profName.Location = new System.Drawing.Point(232, 44);
      this.combo_sel_profName.Name = "combo_sel_profName";
      this.combo_sel_profName.Size = new System.Drawing.Size(121, 21);
      this.combo_sel_profName.TabIndex = 9;
      // 
      // button_act_delete
      // 
      this.button_act_delete.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.button_act_delete.Location = new System.Drawing.Point(360, 80);
      this.button_act_delete.Name = "button_act_delete";
      this.button_act_delete.TabIndex = 8;
      this.button_act_delete.Text = "Delete";
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(8, 80);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(224, 16);
      this.label5.TabIndex = 9;
      this.label5.Text = "Delete selected profiles";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.button_act_move);
      this.groupBox2.Controls.Add(this.combo_act_moveToSpecName);
      this.groupBox2.Controls.Add(this.button_act_copy);
      this.groupBox2.Controls.Add(this.combo_act_copyToSpecName);
      this.groupBox2.Controls.Add(this.label2);
      this.groupBox2.Controls.Add(this.button_act_delete);
      this.groupBox2.Controls.Add(this.label5);
      this.groupBox2.Controls.Add(this.label1);
      this.groupBox2.Location = new System.Drawing.Point(8, 176);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(448, 144);
      this.groupBox2.TabIndex = 10;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "2) Action";
      // 
      // text_sel_count
      // 
      this.text_sel_count.Location = new System.Drawing.Point(104, 112);
      this.text_sel_count.Name = "text_sel_count";
      this.text_sel_count.ReadOnly = true;
      this.text_sel_count.Size = new System.Drawing.Size(32, 20);
      this.text_sel_count.TabIndex = 10;
      this.text_sel_count.Text = "0";
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(136, 116);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(168, 16);
      this.label6.TabIndex = 11;
      this.label6.Text = "profiles currently selected";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(360, 16);
      this.button1.Name = "button1";
      this.button1.TabIndex = 12;
      this.button1.Text = "Select";
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(360, 44);
      this.button2.Name = "button2";
      this.button2.TabIndex = 13;
      this.button2.Text = "Select";
      // 
      // FormProfileMassEdit
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(464, 325);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormProfileMassEdit";
      this.Text = "Profile Mass Editor";
      this.groupBox1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion
	}
}
