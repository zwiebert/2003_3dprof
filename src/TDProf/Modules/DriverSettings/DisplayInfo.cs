#define RESTORE_GAMMA

using System;
using System.IO; // for debugging
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;
using Win32;


namespace TDProf {


  /// <summary>
  /// Summary description for DisplayInfo.
  /// </summary>
  public class DisplayInfo {

    //-------------------------------------------------------------------------

    private DISPLAY_DEVICE dd_ = new DISPLAY_DEVICE();
    string driver_key = null;
    string driver_dll = null;
    FileVersionInfo driver_dll_version_info = null;
    string driver_version = null;

    public bool is_ati_radeon = false; //to indicate if we need reset screenmode for D3D changes
	
    // no properties, because if we need more than one display
    // we can just overload with get_...(int idx)
    public string get_driver_key() { return driver_key; }
    public string get_device_id__() { return dd_.DeviceID; }
    public string get_device_string() { return dd_.DeviceString; }
    public string get_vendor_id() {
      Match match = Regex.Match(dd_.DeviceID, @"VEN_(....)");
      string vendor_id = match.Groups[1].ToString();
      return vendor_id;
    }

    public string get_guid() {
      Match m = Regex.Match(driver_key, @"(\{........-....-....-....-............\})");
      if (m.Success)
        return m.Groups[1].Value;
      return null;
    }

    public string get_device_id() {
      Match match = Regex.Match(dd_.DeviceID, @"DEV_(....)");
      return (match.Groups[1].Success ? match.Groups[1].Value : "");
    }
    public string get_driver_version() { return driver_version; }

    private void debug() {
      string env_debug = Environment.GetEnvironmentVariable("TDPROF_DEBUG");
      if (env_debug == null || env_debug.IndexOf("(di)") == -1)
        return;

      string link_path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
      Stream stream = File.Open(link_path + @"\tdprof-log.txt", FileMode.Create);
      System.IO.StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.ASCII);

      for (int i = 0; Win32.User32.EnumDisplayDevices(null, i, /*ref*/ dd_, 0) != 0; ++i) {
        if (i > 0)
          sw.WriteLine("\r\n-----------------------------------------------------------\r\n");
        sw.WriteLine("dd.DeviceName=\"" + dd_.DeviceName + "\"");
        sw.WriteLine("dd.DeviceString=\"" + dd_.DeviceString + "\"");
        sw.WriteLine("dd.DeviceKey=\"" + dd_.DeviceKey + "\"");
        sw.WriteLine("dd.DeviceID=\"" + dd_.DeviceID + "\"");
        sw.WriteLine("dd.StateFlags=\"" + String.Format("{0:x8}" + "\"", dd_.StateFlags));
      }
      sw.Close();
      stream.Close();
    }

    public string report() {
      StringBuilder sb = new StringBuilder(1024);
      DISPLAY_DEVICE dd = new DISPLAY_DEVICE();
      for (int i = 0; Win32.User32.EnumDisplayDevices(null, i, /*ref*/ dd, 0) != 0; ++i) {
        if (i > 0)
          sb.Append("\r\n-----------------------------------------------------------\r\n\r\n");
        sb.Append("dd.DeviceName=\"" + dd.DeviceName + "\"\r\n");
        sb.Append("dd.DeviceString=\"" + dd.DeviceString + "\"\r\n");
        sb.Append("dd.DeviceKey=\"" + dd.DeviceKey + "\"\r\n");
        sb.Append("dd.DeviceID=\"" + dd.DeviceID + "\"\r\n");
        sb.Append("dd.StateFlags=\"" + String.Format("{0:x8}" + "\"\r\n", dd.StateFlags));
      }
      sb.Append("\r\n====================================================\r\n\r\n");
      sb.AppendFormat("get_driver_key() => {0}\r\n", get_driver_key());
      sb.AppendFormat("get_vendor_id() => {0}\r\n", get_vendor_id());
      sb.AppendFormat("get_guid() => {0}\r\n", get_guid());
      return sb.ToString();
    }

		

    public bool obtain_device_info(string force_driver_key, string force_device_key) {
      G.applog("** entering DisplayInfo.obtain_device_info(...)");


      bool success;
			
      if (force_device_key != null) {
        dd_.DeviceKey = force_device_key;
        success = true;
      }
      else
        success = (Win32.User32.EnumDisplayDevices(null, 0, /*ref*/ dd_, 0) != 0);

      if (success) {
        driver_key = (dd_.DeviceKey.ToLower().StartsWith(@"\registry\machine\") 
          ? dd_.DeviceKey.Substring(18) // On Windows 5
          : dd_.DeviceKey); // On Windows 4 or if force_device_key was used w/o prefix
        is_ati_radeon = (dd_.DeviceID.ToLower().IndexOf("ven_1002") != -1);
        string four_digits = driver_key.Substring(driver_key.Length - 4);

        if (is_ati_radeon && Environment.OSVersion.Platform != System.PlatformID.Win32NT)
          driver_key = @"Software\ATI Technologies\Driver\" + four_digits;
      }

      if (force_device_key == null) {
        dd_.DeviceKey = (dd_.DeviceKey.ToLower().StartsWith(@"\registry\machine\") 
          ? dd_.DeviceKey.Substring(18) // On Windows 5
          : dd_.DeviceKey); // On Windows 4 or if force_device_key was used w/o prefix
      }

      if (force_driver_key != null)
        driver_key = force_driver_key;

      try {
        Microsoft.Win32.RegistryKey dk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(dd_.DeviceKey);
        if (is_ati_radeon) {
          string rel_ver = (string)dk.GetValue("ReleaseVersion");
          if (rel_ver != null) {
#if false
            driver_version = rel_ver.Substring(0, rel_ver.IndexOf("-"));
#else
            driver_version = rel_ver.Substring(0, 1) + "." + rel_ver.Substring(2,2);
#endif
          }
        } else if (Environment.OSVersion.Platform == System.PlatformID.Win32NT) {
          string[] driver_dll_arr = (string[])dk.GetValue("InstalledDisplayDrivers");
          driver_dll = driver_dll_arr[0] + ".dll";
          FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\" + driver_dll);
          driver_dll_version_info = fvi;
          driver_version = driver_dll_version_info.FileVersion; 
        } else if (Environment.OSVersion.Platform == System.PlatformID.Win32Windows) {
          Microsoft.Win32.RegistryKey dk_def = dk.OpenSubKey("DEFAULT");
          driver_dll = (string)dk_def.GetValue(@"drv");
          FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\" + driver_dll);
          driver_dll_version_info = fvi;
          driver_version = driver_dll_version_info.FileVersion; 
          dk_def.Close();
        }
        dk.Close();
      }     
#if DEBUG
      catch (Exception e) {
        throw new FatalError("Error while getting display driver version info :" + e.Message);
      }
#else
      catch { } 
#endif

      G.applog("** leaving DisplayInfo.obtain_device_info(...)");

      return success;		
    }
		
    public DisplayInfo(string force_driver_key, string force_device_key) {
      debug();
      obtain_device_info(force_driver_key, force_device_key);
    }


    static bool driver_cmp_lt(string lhs, string rhs) {
      string[] lhs_split = lhs.Split('.');
      string[] rhs_split = rhs.Split('.');
      for (int i=0; i < lhs_split.Length && i < rhs_split.Length; ++i) {
        if (int.Parse(lhs_split[i]) < int.Parse(rhs_split[i]))
          return true;
      }
      return false;
    }

    public bool is_ati_driver_needing_apply_d3d() {
      return (is_ati_radeon
        && Environment.OSVersion.Platform == System.PlatformID.Win32NT
#if false
        && driver_cmp_lt(get_driver_version(), "7.98")
#endif     
        );        
      }

    public void ati_apply_d3d(bool force) {
      if (!is_ati_driver_needing_apply_d3d() && !force)
        return;

      DEVMODE dm = new DEVMODE();

      if (Win32.User32.EnumDisplaySettings(null, Win32.User32.ENUM_CURRENT_SETTINGS, dm)) {
        dm.dmBitsPerPel = (dm.dmBitsPerPel != 16 ? 16 : 32);
        dm.dmFields = (int)(Win32.User32.DM_BITSPERPEL);
        Win32.User32.ChangeDisplaySettings(dm, Win32.User32.CDS_RESET);
        Win32.User32.ChangeDisplaySettings(null, 0);
      }
    }


    DEVMODE m_desktopDM = null;

    public void save_resolution() {
      if (m_desktopDM == null)
        m_desktopDM = new DEVMODE();
      DEVMODE dm = m_desktopDM;

      if (Win32.User32.EnumDisplaySettings(null, Win32.User32.ENUM_CURRENT_SETTINGS, dm)) {
        ;
      }
#if RESTORE_GAMMA
      save_gamma_ramp();
#endif
    }

    static public void set_resolution(int display, int hres, int vres, int vfreq, int bpp) {
      DEVMODE dm = new DEVMODE();
      dm.dmDeviceName  = @"\\.\DISPLAY" + display.ToString();
      dm.dmFields      = (int)(Win32.User32.DM_PELSWIDTH | Win32.User32.DM_PELSHEIGHT | Win32.User32.DM_DISPLAYFREQUENCY);
      if (bpp != 0)
        dm.dmFields   |= (int)Win32.User32.DM_BITSPERPEL;
      dm.dmPelsWidth   = hres;
      dm.dmPelsHeight  = vres;
      dm.dmDisplayFreq = vfreq;
      dm.dmBitsPerPel  = bpp;

      Win32.User32.ChangeDisplaySettings(dm, 0); //Win32.User32.CDS_UPDATEREGISTRY);
    }
    public void restore_resolution() {
      if (m_desktopDM == null)
        return;
      DEVMODE dm;

#if false
      dm = new DEVMODE();
      if (! Win32.User32.EnumDisplaySettings(null, Win32.User32.ENUM_CURRENT_SETTINGS, dm))
        return;
#endif

      dm = m_desktopDM;
      dm.dmFields = (int)(Win32.User32.DM_PELSWIDTH | Win32.User32.DM_PELSHEIGHT | Win32.User32.DM_BITSPERPEL | Win32.User32.DM_DISPLAYFREQUENCY);
      Win32.User32.ChangeDisplaySettings(dm, 0);

#if RESTORE_GAMMA
      restore_gamma_ramp();
#endif

    }


    GDI32.GammaRamp m_gammaRamp = null;
    void save_gamma_ramp() {
      if (m_gammaRamp == null)
        m_gammaRamp = new Win32.GDI32.GammaRamp();
      System.IntPtr dc = Win32.User32.GetDC(IntPtr.Zero);
      if (dc != IntPtr.Zero) {
        if (!Win32.GDI32.GetDeviceGammaRamp(dc, m_gammaRamp)) {
          m_gammaRamp = null;
        }
        if (Win32.User32.ReleaseDC(IntPtr.Zero, dc) != 1) {
          //int n = 0;
        }
      }

    }
  
    void restore_gamma_ramp() {
      if (m_gammaRamp == null)
        return;
      System.IntPtr dc = Win32.User32.GetDC(IntPtr.Zero);
      if (dc != IntPtr.Zero) {
        Win32.GDI32.SetDeviceGammaRamp(dc, m_gammaRamp);
        if (Win32.User32.ReleaseDC(IntPtr.Zero, dc) != 1) {
          //int n = 0;
        }
      }
    }

    void set_gamma_ramp(double gamma) {
      Win32.GDI32.GammaRamp gr = new Win32.GDI32.GammaRamp(gamma);
      System.IntPtr dc = Win32.User32.GetDC(IntPtr.Zero);
      if (dc != IntPtr.Zero) {
        Win32.GDI32.SetDeviceGammaRamp(dc, gr);
        if (Win32.User32.ReleaseDC(IntPtr.Zero, dc) != 1) {
          //int n = 0;
        }
      }

    }
  
  }
}
