using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TDProf.Forms {
  /// <summary>
  /// Summary description for FormHotkeys.
  /// </summary>
  public class FormHotkeys : System.Windows.Forms.Form {
    #region Private Data

    Profiles.GameProfiles        m_gps;
    Profiles.ProfileHotkeys      m_phks;
    App.AppHotkeys               m_ahks;
    Hashtable                    m_profs;
    Keys[]                       m_appKeys;

    bool chooseHotkey_complete = true;
    private System.Windows.Forms.Label label1;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    #endregion

 
    public FormHotkeys(Profiles.GameProfiles gps, Profiles.ProfileHotkeys phks, App.AppHotkeys ahks) {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      m_gps     = gps;                             // GameProfiles
      m_phks    = phks;                            // Profile Hotkeys
      m_ahks    = ahks;                            // App Hotkeys
      m_profs   = new Hashtable();                 // Profiles
      m_appKeys = new Keys[list_app.Items.Count];  // AppKeys


      // init profile hotkeys
      /// Associative array GPD.NAME => Hotkey-Data to store key codes
      foreach (Profiles.GameProfileData gpd in m_gps) {
        m_profs[gpd.name] = m_phks.RetrieveHotkey(gpd.name);
      }
      
      /// name,hotkey pairs are used in listview
      foreach (DictionaryEntry den in m_profs) {
        string profName_text = (string)den.Key;
        string hotkeys_text  = ((Keys)den.Value).ToString();
        ListViewItem lvi = new ListViewItem(profName_text + "                                                                                                           ");
        lvi.Tag = profName_text;
        lvi.UseItemStyleForSubItems = false;
        lvi.SubItems.Add(hotkeys_text);
        list_profs.Items.Add(lvi);        
      }

      m_phks.DeactivateHotkeys();


      // init app hotkeys
      /// funcktion,hotkey pairs are used in listview
      for (int i=0, len=m_appKeys.Length; i < len; ++i) {
        m_appKeys[i] = m_ahks.GetHotkey((App.AppHotkeys.EAppKeys)i);

        ListViewItem lvi = list_app.Items[i];
        lvi.Text += "                                                                                                           ";
        lvi.UseItemStyleForSubItems = false; // color support for sub items
        lvi.SubItems[1].Text = m_appKeys[i].ToString();

        lvi.Tag = i.ToString(); // <----XXX/TODO: buggy GUI designer workaround (assigned tags in designer does not make it into code)
      }

      m_ahks.DeactivateHotkeys();
      //XXX: add code for adding not existing profile names from m_phks.
    }

    string print_hotkey(Keys key) {
      string result = "";
      result += chooseHotkey_print_modifiers((key & Keys.Alt) != 0, (key & Keys.Control) != 0, (key & Keys.Shift) != 0); 
      return result;
    }

    string chooseHotkey_print_modifiers(bool pressed_alt, bool pressed_control, bool pressed_shift) {
      string result = (pressed_alt ? Keys.Alt.ToString() + " + " : "")
        + (pressed_control ? Keys.Control.ToString() + " + " : "")
        + (pressed_shift ? Keys.Shift.ToString() + " + " : "");
      return result;
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


    void set_hotkey(ListView lvw, ListViewItem lvi, Keys hotkey, Keys view_hotkey) {
      bool hotkey_in_use = false;
      string text = view_hotkey.ToString();
      if (hotkey != Keys.None) {
        set_hotkey(lvw, lvi, Keys.None, view_hotkey);
        if (m_profs.ContainsValue(hotkey))
          hotkey_in_use = true;
        else foreach(Keys key in m_appKeys)
               if (key == hotkey) {
                 hotkey_in_use = true;
                 break;
               }
      }

      if (hotkey_in_use) {
        hotkey = Keys.None;
        text = "Already in use: " + text;
      }

      lvi.SubItems[1].Text = text;
      lvi.SubItems[1].ForeColor = (hotkey_in_use ? System.Drawing.Color.Red : System.Drawing.Color.Green);
      lvi.SubItems[1].BackColor = System.Drawing.Color.White;

      if (lvw == list_profs) {
        m_profs[(string)lvi.Tag] = hotkey;
      } else if (lvw == list_app) {
        m_appKeys[int.Parse((string)lvi.Tag)] = hotkey;
      }

    }

 
		#region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormHotkeys));
      this.list_profs = new System.Windows.Forms.ListView();
      this.colHead_names = new System.Windows.Forms.ColumnHeader();
      this.colHead_keys = new System.Windows.Forms.ColumnHeader();
      this.button_cancel = new System.Windows.Forms.Button();
      this.button_ok = new System.Windows.Forms.Button();
      this.list_app = new System.Windows.Forms.ListView();
      this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // list_profs
      // 
      this.list_profs.AccessibleDescription = ((string)(resources.GetObject("list_profs.AccessibleDescription")));
      this.list_profs.AccessibleName = ((string)(resources.GetObject("list_profs.AccessibleName")));
      this.list_profs.Alignment = ((System.Windows.Forms.ListViewAlignment)(resources.GetObject("list_profs.Alignment")));
      this.list_profs.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("list_profs.Anchor")));
      this.list_profs.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("list_profs.BackgroundImage")));
      this.list_profs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                 this.colHead_names,
                                                                                 this.colHead_keys});
      this.list_profs.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("list_profs.Dock")));
      this.list_profs.Enabled = ((bool)(resources.GetObject("list_profs.Enabled")));
      this.list_profs.Font = ((System.Drawing.Font)(resources.GetObject("list_profs.Font")));
      this.list_profs.GridLines = true;
      this.list_profs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.list_profs.HideSelection = false;
      this.list_profs.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("list_profs.ImeMode")));
      this.list_profs.LabelWrap = ((bool)(resources.GetObject("list_profs.LabelWrap")));
      this.list_profs.Location = ((System.Drawing.Point)(resources.GetObject("list_profs.Location")));
      this.list_profs.MultiSelect = false;
      this.list_profs.Name = "list_profs";
      this.list_profs.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("list_profs.RightToLeft")));
      this.list_profs.Size = ((System.Drawing.Size)(resources.GetObject("list_profs.Size")));
      this.list_profs.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.list_profs.TabIndex = ((int)(resources.GetObject("list_profs.TabIndex")));
      this.list_profs.Text = resources.GetString("list_profs.Text");
      this.list_profs.View = System.Windows.Forms.View.Details;
      this.list_profs.Visible = ((bool)(resources.GetObject("list_profs.Visible")));
      this.list_profs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.list_any_KeyDown);
      this.list_profs.KeyUp += new System.Windows.Forms.KeyEventHandler(this.list_any_KeyUp);
      // 
      // colHead_names
      // 
      this.colHead_names.Text = resources.GetString("colHead_names.Text");
      this.colHead_names.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("colHead_names.TextAlign")));
      this.colHead_names.Width = ((int)(resources.GetObject("colHead_names.Width")));
      // 
      // colHead_keys
      // 
      this.colHead_keys.Text = resources.GetString("colHead_keys.Text");
      this.colHead_keys.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("colHead_keys.TextAlign")));
      this.colHead_keys.Width = ((int)(resources.GetObject("colHead_keys.Width")));
      // 
      // button_cancel
      // 
      this.button_cancel.AccessibleDescription = ((string)(resources.GetObject("button_cancel.AccessibleDescription")));
      this.button_cancel.AccessibleName = ((string)(resources.GetObject("button_cancel.AccessibleName")));
      this.button_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_cancel.Anchor")));
      this.button_cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_cancel.BackgroundImage")));
      this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button_cancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button_cancel.Dock")));
      this.button_cancel.Enabled = ((bool)(resources.GetObject("button_cancel.Enabled")));
      this.button_cancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button_cancel.FlatStyle")));
      this.button_cancel.Font = ((System.Drawing.Font)(resources.GetObject("button_cancel.Font")));
      this.button_cancel.Image = ((System.Drawing.Image)(resources.GetObject("button_cancel.Image")));
      this.button_cancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_cancel.ImageAlign")));
      this.button_cancel.ImageIndex = ((int)(resources.GetObject("button_cancel.ImageIndex")));
      this.button_cancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button_cancel.ImeMode")));
      this.button_cancel.Location = ((System.Drawing.Point)(resources.GetObject("button_cancel.Location")));
      this.button_cancel.Name = "button_cancel";
      this.button_cancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button_cancel.RightToLeft")));
      this.button_cancel.Size = ((System.Drawing.Size)(resources.GetObject("button_cancel.Size")));
      this.button_cancel.TabIndex = ((int)(resources.GetObject("button_cancel.TabIndex")));
      this.button_cancel.Text = resources.GetString("button_cancel.Text");
      this.button_cancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button_cancel.TextAlign")));
      this.button_cancel.Visible = ((bool)(resources.GetObject("button_cancel.Visible")));
      this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
      // 
      // button_ok
      // 
      this.button_ok.AccessibleDescription = ((string)(resources.GetObject("button_ok.AccessibleDescription")));
      this.button_ok.AccessibleName = ((string)(resources.GetObject("button_ok.AccessibleName")));
      this.button_ok.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button_ok.Anchor")));
      this.button_ok.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_ok.BackgroundImage")));
      this.button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
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
      // list_app
      // 
      this.list_app.AccessibleDescription = ((string)(resources.GetObject("list_app.AccessibleDescription")));
      this.list_app.AccessibleName = ((string)(resources.GetObject("list_app.AccessibleName")));
      this.list_app.Alignment = ((System.Windows.Forms.ListViewAlignment)(resources.GetObject("list_app.Alignment")));
      this.list_app.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("list_app.Anchor")));
      this.list_app.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("list_app.BackgroundImage")));
      this.list_app.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                               this.columnHeader1,
                                                                               this.columnHeader2});
      this.list_app.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("list_app.Dock")));
      this.list_app.Enabled = ((bool)(resources.GetObject("list_app.Enabled")));
      this.list_app.Font = ((System.Drawing.Font)(resources.GetObject("list_app.Font")));
      this.list_app.GridLines = true;
      this.list_app.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.list_app.HideSelection = false;
      this.list_app.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("list_app.ImeMode")));
      this.list_app.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
                                                                             ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_app.Items.Items"))),
                                                                             ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_app.Items.Items1"))),
                                                                             ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_app.Items.Items2"))),
                                                                             ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_app.Items.Items3"))),
                                                                             ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_app.Items.Items4"))),
                                                                             ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_app.Items.Items5")))});
      this.list_app.LabelWrap = ((bool)(resources.GetObject("list_app.LabelWrap")));
      this.list_app.Location = ((System.Drawing.Point)(resources.GetObject("list_app.Location")));
      this.list_app.MultiSelect = false;
      this.list_app.Name = "list_app";
      this.list_app.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("list_app.RightToLeft")));
      this.list_app.Size = ((System.Drawing.Size)(resources.GetObject("list_app.Size")));
      this.list_app.TabIndex = ((int)(resources.GetObject("list_app.TabIndex")));
      this.list_app.Text = resources.GetString("list_app.Text");
      this.list_app.View = System.Windows.Forms.View.Details;
      this.list_app.Visible = ((bool)(resources.GetObject("list_app.Visible")));
      this.list_app.KeyDown += new System.Windows.Forms.KeyEventHandler(this.list_any_KeyDown);
      this.list_app.KeyUp += new System.Windows.Forms.KeyEventHandler(this.list_any_KeyUp);
      // 
      // columnHeader1
      // 
      this.columnHeader1.Text = resources.GetString("columnHeader1.Text");
      this.columnHeader1.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader1.TextAlign")));
      this.columnHeader1.Width = ((int)(resources.GetObject("columnHeader1.Width")));
      // 
      // columnHeader2
      // 
      this.columnHeader2.Text = resources.GetString("columnHeader2.Text");
      this.columnHeader2.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader2.TextAlign")));
      this.columnHeader2.Width = ((int)(resources.GetObject("columnHeader2.Width")));
      // 
      // label1
      // 
      this.label1.AccessibleDescription = ((string)(resources.GetObject("label1.AccessibleDescription")));
      this.label1.AccessibleName = ((string)(resources.GetObject("label1.AccessibleName")));
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
      this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
      this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
      this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
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
      // FormHotkeys
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
                                                                  this.label1,
                                                                  this.list_app,
                                                                  this.list_profs,
                                                                  this.button_cancel,
                                                                  this.button_ok});
      this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
      this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
      this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
      this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
      this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
      this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
      this.Name = "FormHotkeys";
      this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
      this.ShowInTaskbar = false;
      this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
      this.Text = resources.GetString("$this.Text");
      this.Visible = ((bool)(resources.GetObject("$this.Visible")));
      this.Closing += new System.ComponentModel.CancelEventHandler(this.FormHotkeys_Closing);
      this.ResumeLayout(false);

    }
		#endregion
    #region *** Widget Defintions ***
    private System.Windows.Forms.ListView list_profs;
    private System.Windows.Forms.ColumnHeader colHead_names;
    private System.Windows.Forms.ColumnHeader colHead_keys;
    private System.Windows.Forms.Button button_cancel;
    private System.Windows.Forms.Button button_ok;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.ListView list_app;
    #endregion
    #region *** Widget Callbacks ***

    private void list_any_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
      ListView lvw = (ListView)sender;
      chooseHotkey_complete = false;
      ListView.SelectedListViewItemCollection lvis = lvw.SelectedItems;
      if (lvis.Count == 0)
        return;
      ListViewItem lvi = lvis[0];

      if (e.KeyCode == Keys.Escape) {
        set_hotkey(lvw, lvi, Keys.None, Keys.None);
        chooseHotkey_complete = true;
        return;
      }

      if (e.Modifiers == 0)
        return;

      lvi.SubItems[1].Text = e.Modifiers.ToString();

      if (e.KeyCode != Keys.Menu && e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey) {
        if (e.Modifiers != 0) {
          set_hotkey(lvw, lvi, e.KeyData, e.KeyData);
        }
        chooseHotkey_complete = true;
      } else {
        lvi.SubItems[1].ForeColor = System.Drawing.Color.Blue;
      }
    }

    private void list_any_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
      ListView lvw = (ListView)sender;
      if (!chooseHotkey_complete) {
        ListView.SelectedListViewItemCollection lvis = lvw.SelectedItems;
        if (lvis.Count == 0)
          return;
        if (e.Modifiers == 0 && !(e.KeyCode == Keys.Menu || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey))
          return;
        ListViewItem lvi = lvis[0];
        set_hotkey(lvw, lvi, Keys.None, e.Modifiers);
        chooseHotkey_complete = (e.Modifiers == 0);
      }
    }

    private void button_ok_Click(object sender, System.EventArgs e) {

      m_phks.RemoveAllHotkeys();
      foreach (DictionaryEntry den in m_profs) {
        string profile_name = (string)den.Key;
        Keys hotkey         = (Keys)den.Value;
        m_phks.AddHotkey(profile_name, hotkey);
      }
      m_phks.Save("hotkeys.xml");


      for(int i=0, length=m_appKeys.Length; i < length; ++i) {
        m_ahks.SetHotkey((App.AppHotkeys.EAppKeys)i, m_appKeys[i]);
      }
      m_ahks.Save(G.ax.ac);

      this.Close();
     
    }

    private void FormHotkeys_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
      m_phks.ActivateHotkeys();
      m_ahks.ActivateHotkeys();
    }

    private void button_cancel_Click(object sender, System.EventArgs e) {
      this.Close();
    }
 


    #endregion



  }
}
