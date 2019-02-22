using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace r6probe_id_patcher
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form_Main : System.Windows.Forms.Form
	{
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.TextBox text_binary;
    private System.Windows.Forms.ListView list_pciIDs;
    private System.Windows.Forms.Button button_binary;
    private System.Windows.Forms.TextBox text_devid;
    private System.Windows.Forms.Button button_devid_remove;
    private System.Windows.Forms.Button button_devid_add;
    private System.Windows.Forms.Button button_devid_edit;
    private System.Windows.Forms.Button button_binary_save;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form_Main()
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
				if (components != null) 
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
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.button_binary = new System.Windows.Forms.Button();
      this.text_binary = new System.Windows.Forms.TextBox();
      this.list_pciIDs = new System.Windows.Forms.ListView();
      this.text_devid = new System.Windows.Forms.TextBox();
      this.button_devid_remove = new System.Windows.Forms.Button();
      this.button_devid_add = new System.Windows.Forms.Button();
      this.button_devid_edit = new System.Windows.Forms.Button();
      this.button_binary_save = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // button_binary
      // 
      this.button_binary.Location = new System.Drawing.Point(8, 24);
      this.button_binary.Name = "button_binary";
      this.button_binary.Size = new System.Drawing.Size(48, 23);
      this.button_binary.TabIndex = 0;
      this.button_binary.Text = "Binary";
      this.button_binary.Click += new System.EventHandler(this.button_binary_Click);
      // 
      // text_binary
      // 
      this.text_binary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.text_binary.Location = new System.Drawing.Point(56, 26);
      this.text_binary.Name = "text_binary";
      this.text_binary.Size = new System.Drawing.Size(672, 20);
      this.text_binary.TabIndex = 1;
      this.text_binary.Text = "";
      // 
      // list_pciIDs
      // 
      this.list_pciIDs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.list_pciIDs.FullRowSelect = true;
      this.list_pciIDs.HideSelection = false;
      this.list_pciIDs.LabelEdit = true;
      this.list_pciIDs.Location = new System.Drawing.Point(0, 80);
      this.list_pciIDs.Name = "list_pciIDs";
      this.list_pciIDs.Size = new System.Drawing.Size(728, 304);
      this.list_pciIDs.TabIndex = 2;
      this.list_pciIDs.View = System.Windows.Forms.View.List;
      this.list_pciIDs.SelectedIndexChanged += new System.EventHandler(this.list_pciIDs_SelectedIndexChanged);
      // 
      // text_devid
      // 
      this.text_devid.Location = new System.Drawing.Point(128, 392);
      this.text_devid.Name = "text_devid";
      this.text_devid.Size = new System.Drawing.Size(48, 20);
      this.text_devid.TabIndex = 3;
      this.text_devid.Text = "";
      // 
      // button_devid_remove
      // 
      this.button_devid_remove.Location = new System.Drawing.Point(8, 392);
      this.button_devid_remove.Name = "button_devid_remove";
      this.button_devid_remove.TabIndex = 4;
      this.button_devid_remove.Text = "Remove";
      this.button_devid_remove.Click += new System.EventHandler(this.button_devid_remove_Click);
      // 
      // button_devid_add
      // 
      this.button_devid_add.Location = new System.Drawing.Point(184, 392);
      this.button_devid_add.Name = "button_devid_add";
      this.button_devid_add.TabIndex = 5;
      this.button_devid_add.Text = "Add";
      this.button_devid_add.Click += new System.EventHandler(this.button_devid_add_Click);
      // 
      // button_devid_edit
      // 
      this.button_devid_edit.Location = new System.Drawing.Point(296, 392);
      this.button_devid_edit.Name = "button_devid_edit";
      this.button_devid_edit.TabIndex = 6;
      this.button_devid_edit.Text = "Edit";
      this.button_devid_edit.Click += new System.EventHandler(this.button_devid_edit_Click);
      // 
      // button_binary_save
      // 
      this.button_binary_save.Location = new System.Drawing.Point(80, 56);
      this.button_binary_save.Name = "button_binary_save";
      this.button_binary_save.TabIndex = 7;
      this.button_binary_save.Text = "Save";
      this.button_binary_save.Click += new System.EventHandler(this.button_binary_save_Click);
      // 
      // Form_Main
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(736, 453);
      this.Controls.Add(this.button_binary_save);
      this.Controls.Add(this.button_devid_edit);
      this.Controls.Add(this.button_devid_add);
      this.Controls.Add(this.button_devid_remove);
      this.Controls.Add(this.text_devid);
      this.Controls.Add(this.list_pciIDs);
      this.Controls.Add(this.text_binary);
      this.Controls.Add(this.button_binary);
      this.Name = "Form_Main";
      this.Text = "R6Probe PCI Device ID Patcher";
      this.ResumeLayout(false);

    }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form_Main());
		}

    private void button_binary_Click(object sender, System.EventArgs e) {
      openFileDialog.Filter = "R6Probe Driver |r6probe.sys;r6probe.vxd;|All files (*.*)|*.*";


      if(openFileDialog.ShowDialog() == DialogResult.OK) {
        text_binary.Text = openFileDialog.FileName;

        System.IO.Stream myStream;

        if((myStream = openFileDialog.OpenFile())!= null) {
          if (find_tables(myStream)) {
            read_tables(myStream);
            file_checksum = read_checksum(myStream);
            checksum = calc_checksum(myStream);
            highlight_doubles();


          }

          myStream.Close();
        }
      }

    }

    short checksum;
    short file_checksum;

    #region table_location

    const int MAGIC_BEG = 0x4213;
    const int MAGIC_MID = 0x1299;
    const int MAGIC_END = 0x6963;
    const int MAGIC_LENGTH = 8;

    short[] MAGIC_NMBS = { MAGIC_BEG, MAGIC_MID, MAGIC_END };
    long[] magic_pos = {0, 0, 0};
    long[] table_pos = {0, 0};
    long[] table_len = {0, 0};
    long cs_pos = 0;
    byte[] buf = new byte[2];


    bool find_tables(Stream sin) {

      int magic_run = 0;
      int found = 0;

      for (int i=0; i < 3; ++i) {
        while(sin.Read(buf, 0, 2) == 2) {
          short rn = (short)(buf[0] + (buf[1] << 8));
          magic_run = (rn == MAGIC_NMBS[i]) ? magic_run + 1 : 0;

          if (magic_run == MAGIC_LENGTH) {
            magic_pos[i] = sin.Position - 16;
            ++found;
            break;
          }
        }
      
      }
      if (found == 3) {
        table_pos[0] = magic_pos[0] + 16;
        table_pos[1] = magic_pos[1] + 16;
        table_len[0] = (magic_pos[1] - magic_pos[0]) / 2 - MAGIC_LENGTH;
        table_len[1] = (magic_pos[2] - magic_pos[1]) / 2 - MAGIC_LENGTH;
        cs_pos = magic_pos[2] + 16;
    }
      return found == 3;
    }

    #endregion

    void read_tables(Stream sin) {
      list_pciIDs.Clear();

      for (int i=0; i < 2; ++i) {
        sin.Position = table_pos[i];
        for (int k=0; k < table_len[i]; ++k) {
          sin.Read(buf, 0, 2);
          short rn = (short)(buf[0] + (buf[1] << 8));
          ListViewItem lvi = list_pciIDs.Items.Add(string.Format("{0:x}", rn));
          if (i == 1) lvi.ForeColor = System.Drawing.Color.Blue;
        }

      }

    }


    private void list_pciIDs_SelectedIndexChanged(object sender, System.EventArgs e) {
      ListView lv = sender as ListView;

      if (lv.SelectedItems.Count == 1) {
        text_devid.Text = lv.SelectedItems[0].Text;
      }
    
    }

    private void button_devid_remove_Click(object sender, System.EventArgs e) {
      ListView lv = list_pciIDs;
      if (lv.SelectedItems.Count > 0) {
        foreach (ListViewItem lvi in lv.SelectedItems) {
#if true
          lvi.Text = "0";
#else
          lvi.Remove();
#endif
        }
      }
    
    }

    private void button_devid_add_Click(object sender, System.EventArgs e) {
      ListView lv = list_pciIDs;
      foreach (ListViewItem lvi in lv.Items) {
        if (lvi.Text == "0") {
          lvi.Text = text_devid.Text;
          lvi.Selected = true;
          break;
        }
      }
    }

    private void button_devid_edit_Click(object sender, System.EventArgs e) {
      ListView lv = list_pciIDs;
      if (lv.SelectedItems.Count == 1) {
        lv.SelectedItems[0].BeginEdit();
      }

    }


    void highlight_doubles() {
      ListView lv = list_pciIDs;
      Hashtable ids = new Hashtable();

      foreach (ListViewItem lvi in lv.Items) {
        ids[lvi.Text] =  (ids.ContainsKey(lvi.Text) ?  (int)ids[lvi.Text] + 1 : 1);
      }

      foreach (ListViewItem lvi in lv.Items) {
        int nmb = (int)ids[lvi.Text];
        lvi.BackColor = (nmb == 1 || lvi.Text == "0") ? System.Drawing.Color.White : System.Drawing.Color.Red;
      }

    }


    short calc_checksum(Stream sin) {
      short cs = 0;
      for (int i=0; i < 2; ++i) {
        sin.Position = table_pos[i];
        for (int k=0; k < table_len[i]; ++k) {
          sin.Read(buf, 0, 2);
          short rn = (short)(buf[0] + (buf[1] << 8));
          cs += rn;
        }
      }
      return cs;
    }

    
    short calc_checksum(ListView lv) {
      short cs = 0;
      foreach (ListViewItem lvi in lv.Items) {
        string s = lvi.Text.PadLeft(4, '0');        
        short n = short.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
        cs += n;
      }

      return cs;
    }



    void write_binary(Stream sout) {
      ListView lv = list_pciIDs;
      int idx = 0;

      correct_checksum(sout);

      for(int i=0; i < 2; ++i) {
        sout.Position = table_pos[i];
        for(int k=0; k < table_len[i]; ++k) {
          string s = lv.Items[idx++].Text;
          s = s.PadLeft(4, '0');        
          buf[0] = byte.Parse(s.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
          buf[1] = byte.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
          sout.Write(buf, 0, 2);
        }
      }
    }

    short read_checksum(Stream sin) {
      sin.Position = cs_pos;
      sin.Read(buf, 0, 2);
      short rn = (short)(buf[0] + (buf[1] << 8));

      return rn;
    }

    void correct_checksum(Stream sout) {
      short new_cs = calc_checksum(list_pciIDs);
      int cs_diff = checksum - new_cs;
      short rn = file_checksum;
      rn = (short)(rn + cs_diff);
      sout.Position = cs_pos;
      buf[0] = (byte)((rn >> 8) & 0xff);
      buf[1] = (byte)(rn & 0xff);
      sout.Write(buf, 0, 2);
    }

    private void button_binary_save_Click(object sender, System.EventArgs e) {
      using (FileStream myStream = File.OpenWrite(text_binary.Text)) {
        write_binary(myStream);
      }
    }


    Hashtable hash_devid_map = new Hashtable();

    void init_hash_devide_map() {

    }



  }
}
