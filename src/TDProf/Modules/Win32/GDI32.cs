using System;
using System.Runtime.InteropServices;

namespace Win32
{
	/// <summary>
	/// Summary description for GDI32.
	/// </summary>
	public class GDI32
	{

    [DllImport("gdi32.dll", CharSet=CharSet.Auto)]    
    public static extern bool GetDeviceGammaRamp([In] System.IntPtr hDC, [Out] GammaRamp lpRamp);
    [DllImport("gdi32.dll", CharSet=CharSet.Auto)]    
    public static extern bool SetDeviceGammaRamp([In] System.IntPtr hDC, [In]  GammaRamp lpRamp);


    [StructLayout(LayoutKind.Sequential,Pack=1)]
      public class GammaRamp {
      // MarshalAs tells how big it already is, but does not allocate memory.
      [MarshalAs(UnmanagedType.ByValArray, SizeConst=768)]
      ushort[] ramps = new ushort[768];

      public GammaRamp() {
      }

      public GammaRamp(double clut_gamma) {
        set_gamma(clut_gamma);
      }

      public void set_gamma(double clut_gamma) {
        for (int i = 0; i < 256; i++) {
          ushort val = (ushort)(65535.0 * Math.Pow((double)i / 255.0, 1.0 / clut_gamma));
          ramps[i]       = val;
          ramps[i + 256] = val;
          ramps[i + 512] = val;
        }
      }

    }


	}
}
