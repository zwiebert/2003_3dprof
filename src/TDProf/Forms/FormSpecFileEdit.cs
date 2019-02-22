using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TDProf
{
	/// <summary>
	/// Summary description for FormSpecFileEdit.
	/// </summary>
	public class FormSpecFileEdit : System.Windows.Forms.Form
	{
      private System.Windows.Forms.TreeView tree_sf;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormSpecFileEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

          string[][][] hmr = G.ax.cr.hmr_format();

          TreeNode[] tn_o = new TreeNode[hmr.Length];
          for (int i=0; i < hmr.Length; ++i) {
            TreeNode[] tn_m = new TreeNode[hmr[i].Length - 1];
            for (int ii=1; ii < hmr[i].Length; ++ii) {
              TreeNode[] tn_i = new TreeNode[hmr[i][ii].Length - 1];
              for (int iii=1; iii < hmr[i][ii].Length; ++iii) {
                tn_i[iii-1] = new TreeNode(hmr[i][ii][iii]);
              }
              tn_m[ii-1] = new TreeNode(hmr[i][ii][0], tn_i);
            }
              tn_o[i] = new TreeNode(hmr[i][0][0], tn_m);
          }
          this.tree_sf.Nodes.AddRange(tn_o);
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
          System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormSpecFileEdit));
          this.tree_sf = new System.Windows.Forms.TreeView();
          this.SuspendLayout();
          // 
          // tree_sf
          // 
          this.tree_sf.AccessibleDescription = ((string)(resources.GetObject("tree_sf.AccessibleDescription")));
          this.tree_sf.AccessibleName = ((string)(resources.GetObject("tree_sf.AccessibleName")));
          this.tree_sf.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tree_sf.Anchor")));
          this.tree_sf.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tree_sf.BackgroundImage")));
          this.tree_sf.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tree_sf.Dock")));
          this.tree_sf.Enabled = ((bool)(resources.GetObject("tree_sf.Enabled")));
          this.tree_sf.Font = ((System.Drawing.Font)(resources.GetObject("tree_sf.Font")));
          this.tree_sf.ImageIndex = ((int)(resources.GetObject("tree_sf.ImageIndex")));
          this.tree_sf.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tree_sf.ImeMode")));
          this.tree_sf.Indent = ((int)(resources.GetObject("tree_sf.Indent")));
          this.tree_sf.ItemHeight = ((int)(resources.GetObject("tree_sf.ItemHeight")));
          this.tree_sf.Location = ((System.Drawing.Point)(resources.GetObject("tree_sf.Location")));
          this.tree_sf.Name = "tree_sf";
          this.tree_sf.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tree_sf.RightToLeft")));
          this.tree_sf.SelectedImageIndex = ((int)(resources.GetObject("tree_sf.SelectedImageIndex")));
          this.tree_sf.Size = ((System.Drawing.Size)(resources.GetObject("tree_sf.Size")));
          this.tree_sf.TabIndex = ((int)(resources.GetObject("tree_sf.TabIndex")));
          this.tree_sf.Text = resources.GetString("tree_sf.Text");
          this.tree_sf.Visible = ((bool)(resources.GetObject("tree_sf.Visible")));
          // 
          // FormSpecFileEdit
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
                                                                      this.tree_sf});
          this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
          this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
          this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
          this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
          this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
          this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
          this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
          this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
          this.Name = "FormSpecFileEdit";
          this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
          this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
          this.Text = resources.GetString("$this.Text");
          this.Visible = ((bool)(resources.GetObject("$this.Visible")));
          this.ResumeLayout(false);

        }
		#endregion
	}
}
