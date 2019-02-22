using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using TDProf.DriverSettings;
using TDProf.Profiles;
using TDProf.Util;
using TDProf.App;


namespace Dev
{
	/// <summary>
	/// Summary description for DevScreenShots.
	/// </summary>
	public class ScreenShots
	{
		public ScreenShots()
		{
			//
			// TODO: Add constructor logic here
			//
		}

      static public Image GetScreenShot(Control ctrl) {
        Graphics graphics =null;;
        Graphics graphics_new=null;
        try {
          int width = ctrl.ClientRectangle.Width;
          int height = ctrl.ClientRectangle.Height;

          graphics = ctrl.CreateGraphics();
          Image screenShot_ = new Bitmap(width, height, graphics);
          graphics_new= Graphics.FromImage(screenShot_);
          IntPtr handle_1 = graphics.GetHdc();
          IntPtr handle_2 = graphics_new.GetHdc();
          BitBlt(handle_2, 0, 0, width, height, handle_1, 0, 0, 13369376);
          graphics.ReleaseHdc(handle_1);
          graphics_new.ReleaseHdc(handle_2);
          return screenShot_;
        }
        finally {
          if(null!=graphics) graphics.Dispose();
          if(null!=graphics_new) graphics_new.Dispose();
        }
      }

      [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
      private static extern bool BitBlt(
        IntPtr hdcDest,  
        int nXDest,  
        int nYDest,  
        int nWidth,  
        int nHeight,  
        IntPtr hdcSrc,
        int nXSrc,    
        int nYSrc,          
        System.Int32 dwRop  
        );

      public static void dev_make_and_save_screenshot(Control ctrl, string file) {
        Application.DoEvents();
        Image img = GetScreenShot(ctrl);
        img.Save(file, System.Drawing.Imaging.ImageFormat.Png);
        img.Dispose();
      }

	}
}
