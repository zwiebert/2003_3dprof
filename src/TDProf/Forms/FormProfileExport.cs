using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TDProf
{
	/// <summary>
	/// Summary description for FormProfileExport.
	/// </summary>
	public class FormProfileExport : System.Windows.Forms.Form
	{
    #region GUI Controls
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.ListBox list_profs_vidcard;
    private System.Windows.Forms.Splitter splitter1;
    private System.Windows.Forms.CheckedListBox list_profs_names;
    #endregion

    #region Private Data
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
    private System.Windows.Forms.TreeView treeView1;

    Hashtable m_prof_vidcards = new Hashtable();
    #endregion

    #region Init / Destroy
		public FormProfileExport()
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

    #endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormProfileExport));
      this.panel1 = new System.Windows.Forms.Panel();
      this.list_profs_vidcard = new System.Windows.Forms.ListBox();
      this.splitter1 = new System.Windows.Forms.Splitter();
      this.list_profs_names = new System.Windows.Forms.CheckedListBox();
      this.treeView1 = new System.Windows.Forms.TreeView();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                         this.list_profs_names,
                                                                         this.splitter1,
                                                                         this.list_profs_vidcard});
      this.panel1.Location = new System.Drawing.Point(24, 16);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(232, 200);
      this.panel1.TabIndex = 0;
      // 
      // list_profs_vidcard
      // 
      this.list_profs_vidcard.Dock = System.Windows.Forms.DockStyle.Left;
      this.list_profs_vidcard.Name = "list_profs_vidcard";
      this.list_profs_vidcard.Size = new System.Drawing.Size(112, 199);
      this.list_profs_vidcard.TabIndex = 0;
      // 
      // splitter1
      // 
      this.splitter1.Location = new System.Drawing.Point(112, 0);
      this.splitter1.Name = "splitter1";
      this.splitter1.Size = new System.Drawing.Size(3, 200);
      this.splitter1.TabIndex = 1;
      this.splitter1.TabStop = false;
      // 
      // list_profs_names
      // 
      this.list_profs_names.Dock = System.Windows.Forms.DockStyle.Fill;
      this.list_profs_names.Location = new System.Drawing.Point(115, 0);
      this.list_profs_names.Name = "list_profs_names";
      this.list_profs_names.Size = new System.Drawing.Size(117, 199);
      this.list_profs_names.TabIndex = 2;
      // 
      // treeView1
      // 
      this.treeView1.ImageIndex = -1;
      this.treeView1.Location = new System.Drawing.Point(32, 240);
      this.treeView1.Name = "treeView1";
      this.treeView1.SelectedImageIndex = -1;
      this.treeView1.Size = new System.Drawing.Size(224, 312);
      this.treeView1.TabIndex = 1;
      // 
      // FormProfileExport
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(432, 589);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.treeView1,
                                                                  this.panel1});
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormProfileExport";
      this.Text = "3DProf - Profile Export and Import";
      this.Load += new System.EventHandler(this.FormProfileExport_Load);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion

    private void FormProfileExport_Load(object sender, System.EventArgs e) {
    
    }
	}
}
