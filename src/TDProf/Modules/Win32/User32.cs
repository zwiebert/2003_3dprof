using System;
using System.Runtime.InteropServices;

namespace Win32 {
  /// <summary>
  /// Summary description for User32.
  /// </summary>
  public class User32 {

    #region functions
    [DllImport("User32.dll", CharSet=CharSet.Auto)]
    public static extern int EnumDisplayDevices(String lpDevice, int iDevNum, /*ref*/ [In, Out] DISPLAY_DEVICE lpDisplayDevice, int dwFlags);
    [DllImport("user32.dll", CharSet=CharSet.Auto)]
    public static extern bool EnumDisplaySettings(string lpszDeviceName, int lModeNum, /*ref*/ [Out] DEVMODE lpdm);
    [DllImport("user32.dll", CharSet=CharSet.Auto)]
    public static extern int ChangeDisplaySettings([In] /*ref*/ DEVMODE lpdm, int iFlags);

    [DllImport("user32.dll", CharSet=CharSet.Auto)]
    public static extern System.IntPtr GetDC(System.IntPtr hWnd);
    [DllImport("user32.dll", CharSet=CharSet.Auto)]
    public static extern int ReleaseDC(System.IntPtr hWnd, System.IntPtr hDC);

    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr hWnd,int id,int fsModifiers,int vlc);
    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    #endregion

    #region constants

    public const int DISPLAY_DEVICE_ATTACHED_TO_DESKTOP = 0x00000001;
    public const int DISPLAY_DEVICE_MULTI_DRIVER        = 0x00000002;
    public const int DISPLAY_DEVICE_PRIMARY_DEVICE      = 0x00000004;
    public const int DISPLAY_DEVICE_MIRRORING_DRIVER    = 0x00000008;
    public const int DISPLAY_DEVICE_VGA                 = 0x00000010;

	
    public const int ENUM_CURRENT_SETTINGS = -1;
    public const int ENUM_REGISTRY_SETTINGS = -2;

    // Return values for ChangeDisplaySettings
    public const int DISP_CHANGE_SUCCESSFUL   =    0;
    public const int DISP_CHANGE_RESTART      =    1;
    public const int DISP_CHANGE_FAILED       =   -1;
    public const int DISP_CHANGE_BADMODE      =   -2;
    public const int DISP_CHANGE_NOTUPDATED   =   -3;
    public const int DISP_CHANGE_BADFLAGS     =   -4;
    public const int DISP_CHANGE_BADPARAM     =   -5;

    // Flags for ChangeDisplaySettings
    public const int CDS_UPDATEREGISTRY = 0x00000001;
    public const int CDS_TEST           = 0x00000002;
    public const int CDS_FULLSCREEN   =   0x00000004;
    public const int CDS_GLOBAL       =   0x00000008;
    public const int CDS_SET_PRIMARY  =   0x00000010;
    public const int CDS_RESET        =   0x40000000;
    public const int CDS_SETRECT      =   0x20000000;
    public const int CDS_NORESET      =   0x10000000;
  

    /* field selection bits */
    public const long  DM_ORIENTATION      = 0x00000001L;
    public const long  DM_PAPERSIZE        = 0x00000002L;
    public const long  DM_PAPERLENGTH      = 0x00000004L;
    public const long  DM_PAPERWIDTH       = 0x00000008L;
    public const long  DM_SCALE            = 0x00000010L;
    public const long  DM_COPIES           = 0x00000100L;
    public const long  DM_DEFAULTSOURCE    = 0x00000200L;
    public const long  DM_PRINTQUALITY     = 0x00000400L;
    public const long  DM_COLOR            = 0x00000800L;
    public const long  DM_DUPLEX           = 0x00001000L;
    public const long  DM_YRESOLUTION      = 0x00002000L;
    public const long  DM_TTOPTION         = 0x00004000L;
    public const long  DM_COLLATE          = 0x00008000L;
    public const long  DM_FORMNAME         = 0x00010000L;
    public const long  DM_LOGPIXELS        = 0x00020000L;
    public const long  DM_BITSPERPEL       = 0x00040000L;
    public const long  DM_PELSWIDTH        = 0x00080000L;
    public const long  DM_PELSHEIGHT       = 0x00100000L;
    public const long  DM_DISPLAYFLAGS     = 0x00200000L;
    public const long  DM_DISPLAYFREQUENCY = 0x00400000L;
    //#if(WINVER >= 0x0400)
    public const long  DM_ICMMETHOD     = 0x00800000L;
    public const long  DM_ICMINTENT     = 0x01000000L;
    public const long  DM_MEDIATYPE     = 0x02000000L;
    public const long  DM_DITHERTYPE    = 0x04000000L;
    //#endif /* WINVER >= 0x0400 */

    public const long  DM_GRAYSCALE  = 1;
    public const long  DM_INTERLACED = 2;

  }
  #endregion

    #region structs
  [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
  public /* struct */ class DISPLAY_DEVICE {
    public int cb;
    [MarshalAs(UnmanagedType.ByValTStr , SizeConst=32)]
    public String DeviceName;
    [MarshalAs(UnmanagedType.ByValTStr , SizeConst=128)]
    public String DeviceString;
    public int StateFlags;
    [MarshalAs(UnmanagedType.ByValTStr , SizeConst=128)]
    public String DeviceID;
    [MarshalAs(UnmanagedType.ByValTStr , SizeConst=128)]
    public String DeviceKey;
    public DISPLAY_DEVICE () {
      cb = Marshal.SizeOf(this);
      DeviceName = DeviceString = DeviceID = DeviceKey = ""; //prevent warnings
      StateFlags = 0;
    }
  } //DISPLAY_DEVICE 



  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public /*struct*/ class DEVMODE { 
    // specify the size of the string arrays to 32
    public const int SA_SIZE = 32;

    // struct (class) members
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=SA_SIZE)]
    public string dmDeviceName; 

    public short dmSpecVersion;
    public short dmDriverVersion;
    public short dmSize;
    public short dmDriverExtra;
    public int   dmFields;
    public short dmOrientation;
    public short dmPaperSize;
    public short dmPaperLength;
    public short dmPaperWidth;
    public short dmScale;
    public short dmCopies;
    public short dmDefaultSource;
    public short dmPrintQuality;
    public short dmColor;
    public short dmDuplex;
    public short dmYRes;
    public short dmTTOption;
    public short dmCollate;
  
    // specify marshalling attrib appropriately for this type
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=SA_SIZE)]
    public string lpszFormName;

    public short dmLogPixels;
    public int dmBitsPerPel;
    public int dmPelsWidth;
    public int dmPelsHeight;
    public int dmDisplayFlags;
    public int dmDisplayFreq;
    public int dmICMMethod;
    public int dmICMIntent;
    public int dmMediaType;
    public int dmDitherType;
    public int dmReserved1;
    public int dmReserved2;
    public int dmPanWidth;
    public int dmPanHeight;

    public DEVMODE() {
      dmSize = (short)Marshal.SizeOf(this);
    }
  }//DEVMODE

  #endregion

}
