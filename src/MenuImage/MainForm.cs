using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Chris.Beckett.MenuImage
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu _mainMenu;
		private System.Windows.Forms.MenuItem _miFile;
		private System.Windows.Forms.MenuItem _mFileNew;
		private System.Windows.Forms.MenuItem _miFileOpen;
		private System.Windows.Forms.MenuItem _miFileSave;
		private System.Windows.Forms.MenuItem _miFileSep1;
		private System.Windows.Forms.MenuItem _miFileSaveOnClose;
		private System.Windows.Forms.MenuItem _miFileSep2;
		private System.Windows.Forms.MenuItem _miFileExit;
		private System.Windows.Forms.MenuItem _miHelp;
		private System.Windows.Forms.MenuItem _miHelpContents;
		private System.Windows.Forms.MenuItem _miHelpSep1;
		private System.Windows.Forms.MenuItem _miHelpAbout;
		private System.Windows.Forms.MenuItem _miFileNewItem1;
		private System.Windows.Forms.MenuItem _miFileNewItem2;
		private System.Windows.Forms.ImageList _menuIcons;
		private Chris.Beckett.MenuImageLib.MenuImage _menuImages;
		private System.ComponentModel.IContainer components;

		public MainForm()
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this._mainMenu = new System.Windows.Forms.MainMenu();
			this._miFile = new System.Windows.Forms.MenuItem();
			this._mFileNew = new System.Windows.Forms.MenuItem();
			this._miFileNewItem1 = new System.Windows.Forms.MenuItem();
			this._miFileNewItem2 = new System.Windows.Forms.MenuItem();
			this._miFileOpen = new System.Windows.Forms.MenuItem();
			this._miFileSave = new System.Windows.Forms.MenuItem();
			this._miFileSep1 = new System.Windows.Forms.MenuItem();
			this._miFileSaveOnClose = new System.Windows.Forms.MenuItem();
			this._miFileSep2 = new System.Windows.Forms.MenuItem();
			this._miFileExit = new System.Windows.Forms.MenuItem();
			this._miHelp = new System.Windows.Forms.MenuItem();
			this._miHelpContents = new System.Windows.Forms.MenuItem();
			this._miHelpSep1 = new System.Windows.Forms.MenuItem();
			this._miHelpAbout = new System.Windows.Forms.MenuItem();
			this._menuIcons = new System.Windows.Forms.ImageList(this.components);
			this._menuImages = new Chris.Beckett.MenuImageLib.MenuImage(this.components);
			// 
			// _mainMenu
			// 
			this._mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._miFile,
																					  this._miHelp});
			// 
			// _miFile
			// 
			this._miFile.Index = 0;
			this._miFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this._mFileNew,
																					this._miFileOpen,
																					this._miFileSave,
																					this._miFileSep1,
																					this._miFileSaveOnClose,
																					this._miFileSep2,
																					this._miFileExit});
			this._miFile.Text = "&File";
			// 
			// _mFileNew
			// 
			this._mFileNew.Index = 0;
			this._menuImages.SetMenuImage(this._mFileNew, "0");
			this._mFileNew.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._miFileNewItem1,
																					  this._miFileNewItem2});
			this._mFileNew.OwnerDraw = true;
			this._mFileNew.Text = "&New";
			// 
			// _miFileNewItem1
			// 
			this._miFileNewItem1.Index = 0;
			this._menuImages.SetMenuImage(this._miFileNewItem1, "1");
			this._miFileNewItem1.OwnerDraw = true;
			this._miFileNewItem1.Text = "Item &1";
			// 
			// _miFileNewItem2
			// 
			this._miFileNewItem2.Index = 1;
			this._menuImages.SetMenuImage(this._miFileNewItem2, null);
			this._miFileNewItem2.OwnerDraw = true;
			this._miFileNewItem2.Text = "Item &2";
			// 
			// _miFileOpen
			// 
			this._miFileOpen.Index = 1;
			this._menuImages.SetMenuImage(this._miFileOpen, "2");
			this._miFileOpen.OwnerDraw = true;
			this._miFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this._miFileOpen.Text = "&Open...";
			// 
			// _miFileSave
			// 
			this._miFileSave.Enabled = false;
			this._miFileSave.Index = 2;
			this._menuImages.SetMenuImage(this._miFileSave, "3");
			this._miFileSave.OwnerDraw = true;
			this._miFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this._miFileSave.Text = "&Save";
			// 
			// _miFileSep1
			// 
			this._miFileSep1.Index = 3;
			this._menuImages.SetMenuImage(this._miFileSep1, null);
			this._miFileSep1.OwnerDraw = true;
			this._miFileSep1.Text = "-";
			// 
			// _miFileSaveOnClose
			// 
			this._miFileSaveOnClose.Checked = true;
			this._miFileSaveOnClose.Index = 4;
			this._menuImages.SetMenuImage(this._miFileSaveOnClose, null);
			this._miFileSaveOnClose.OwnerDraw = true;
			this._miFileSaveOnClose.Text = "Save On Close";
			// 
			// _miFileSep2
			// 
			this._miFileSep2.Index = 5;
			this._menuImages.SetMenuImage(this._miFileSep2, null);
			this._miFileSep2.OwnerDraw = true;
			this._miFileSep2.Text = "-";
			// 
			// _miFileExit
			// 
			this._miFileExit.Index = 6;
			this._menuImages.SetMenuImage(this._miFileExit, null);
			this._miFileExit.OwnerDraw = true;
			this._miFileExit.Text = "E&xit";
			// 
			// _miHelp
			// 
			this._miHelp.Index = 1;
			this._miHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this._miHelpContents,
																					this._miHelpSep1,
																					this._miHelpAbout});
			this._miHelp.Text = "&Help";
			// 
			// _miHelpContents
			// 
			this._miHelpContents.Index = 0;
			this._menuImages.SetMenuImage(this._miHelpContents, "4");
			this._miHelpContents.OwnerDraw = true;
			this._miHelpContents.Text = "&Contents...";
			// 
			// _miHelpSep1
			// 
			this._miHelpSep1.Index = 1;
			this._menuImages.SetMenuImage(this._miHelpSep1, null);
			this._miHelpSep1.OwnerDraw = true;
			this._miHelpSep1.Text = "-";
			// 
			// _miHelpAbout
			// 
			this._miHelpAbout.Index = 2;
			this._menuImages.SetMenuImage(this._miHelpAbout, null);
			this._miHelpAbout.OwnerDraw = true;
			this._miHelpAbout.Text = "&About";
			// 
			// _menuIcons
			// 
			this._menuIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this._menuIcons.ImageSize = new System.Drawing.Size(16, 16);
			this._menuIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_menuIcons.ImageStream")));
			this._menuIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _menuImages
			// 
			this._menuImages.ImageList = this._menuIcons;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(312, 169);
			this.Menu = this._mainMenu;
			this.Name = "MainForm";
			this.Text = "MenuImage Sample";

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}
	}
}
