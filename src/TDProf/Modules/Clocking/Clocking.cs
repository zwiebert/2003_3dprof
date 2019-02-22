#if DEBUG
#endif
#define ATI_DLL

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Globalization;

using TDProf.DriverSettings;
using TDProf.Profiles;
using TDProf.Util;
using TDProf.App;


namespace TDProf {

  /// <summary>
  /// Summary description for Clocking.
  /// </summary>
  public class Clocking {

    #region Public Interface
    public static bool init() {
      card = null;
      return clocking_ability();
    }

    public static bool clocking_ability() {

      if (card != null)
        return true;

      try {
#if ATI_DLL
        if (G.ax.ac.exp_clocking_dll && clocking_atidll.is_supported()) {
          card = new clocking_atidll();
          return true;
        }
#endif
        if (clocking_radeon.is_supported()) {
          card = new clocking_radeon();
          return true;
        }

        if (clocking_nvidia.is_supported()) {
          card = new clocking_nvidia();
          return true;
        }

      } catch (FatalError) {}

      return false;
    }

    public static float[] clocking_set_clock(float core_clock, float mem_clock, bool ignore_errors) {
      if (!G.ax.ac.feature_clocking || card == null)
        return null;

      int[] limits = G.ax.ac.clocking_limits;

      if (1.0f < core_clock)
        core_clock = clamp(core_clock, (float)limits[2], (float)limits[3]);
      if (1.0f < mem_clock)
        mem_clock = clamp(mem_clock, (float)limits[0], (float)limits[1]);

      try {
        AppEventLog.log_clock(string.Format("Clocking::card.set_clock({0}, {1})",
          core_clock.ToString(NumberFormatInfo.InvariantInfo),
          mem_clock.ToString(NumberFormatInfo.InvariantInfo)));
        return card.set_clock(core_clock, mem_clock, ignore_errors);

      } catch (Exception e) {
        if (!ignore_errors) {
          MessageForm.open_dialog("Error: " + e.ToString(), "3DProf - Error in Clocking").ShowDialog();
        }
      }
      return null;
    }

    public static float[] clocking_get_clock(bool ignore_errors) {
      if (!G.ax.ac.feature_clocking || card == null)
        return null;

      try {
        return card.get_clock(ignore_errors);
      } catch (Exception e) {
        if (!ignore_errors) {
          string msg = e.ToString() + error_msg();
          System.Windows.Forms.Clipboard.SetDataObject(msg);
          MessageForm.open_dialog(msg, "3DProf - Error in Clocking").ShowDialog();
        }
      }
      return null;
    }
    
    #endregion

    #region Private
    private Clocking() {
    }

    static string error_msg() {
      string msg 
        = "\r\n======================================================"
        + "\r\nCurrent Directory: " + Directory.GetCurrentDirectory()
        + "\r\nInstall Directory: " + G.app_install_directory
        + "\r\nAssembly Location: " + System.Reflection.Assembly.GetCallingAssembly().Location
        + "\r\nSpec-Name: "         + G.spec_name
        + "\r\nDevice Name: "       + G.ax.di.get_device_string()
        + "\r\nDriver Version: "    + G.ax.di.get_driver_version()
        + "\r\nPCI Device ID: "     + G.ax.di.get_device_id()
        + "\r\n======================================================\r\n"
        ;
      return msg;
    }
  
    static float clamp(float val, float min, float max) {
      if (val < min)      return min;
      else if (val > max) return max;
      else                return val;
    }

    #region Card Implemetations
    static clocking_card card;

    interface clocking_card {
      float[] set_clock(float core_clock, float mem_clock, bool ignore_errors);
      float[] get_clock(bool ignore_errors);
    }

    class clocking_atidll : clocking_card {
      public clocking_atidll() {
        if (!is_supported())
          throw new FatalError("unsupported card");
      }

      public static bool is_supported() {
        return (G.is_vendor_ati);
      }
      public float[] set_clock(float core_clock, float mem_clock, bool ignore_errors) {


        // Replace 1.0MHz (magic number) by default clock
        if (core_clock == 1.0 || mem_clock == 1.0) {
          Win32.R6Clock.r6clock_clocks def_clocks = Win32.R6Clock.r6clock_get_default_clock();
          if (core_clock == 1.0)
            core_clock = (float)def_clocks.core_khz / 1000.0f;
          if (mem_clock == 1.0)
            mem_clock = (float)def_clocks.ram_khz / 1000.0f;
        }


        uint core_khz = (uint)(core_clock * 1000.0f);
        uint ram_khz  = (uint)(mem_clock  * 1000.0f);
        Win32.R6Clock.r6clock_clocks clocks = Win32.R6Clock.r6clock_set_clock(core_khz, ram_khz);

        return new float[] { clocks.core_khz * 0.001f, clocks.ram_khz * 0.001f };

      }

      public float[] get_clock(bool ignore_errors) {
        Win32.R6Clock.r6clock_clocks clocks = Win32.R6Clock.r6clock_get_clock();

        return new float[] { clocks.core_khz * 0.001f, clocks.ram_khz * 0.001f };
      }
    }

    class clocking_radeon : clocking_card {

      public clocking_radeon() {
        if (!is_supported())
          throw new FatalError("unsupported card");
      }

      public static bool is_supported() {
        return (G.is_vendor_ati && File.Exists(oc_prog));
      }

      public float[] get_clock(bool ignore_errors) {
        Process p = new Process();
        p.StartInfo.FileName = oc_prog;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        //p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        p.WaitForExit();
        string line;

        if (p.ExitCode != 0) {
          if (!ignore_errors) {
            MessageForm.open_dialog("Error while executing etc/radeon_setclk.exe:\r\n\r\n"
              + p.StandardOutput.ReadToEnd() + "\r\n"
              + p.StandardError.ReadToEnd() + "\r\n"
              + "\r\nHint: There is an installer for r6probe.sys located in 3DProf's application directory\r\n"
              + "It can be installed from 3DProf startmenu too.\r\n"
              + r6probe_debug_infos()
              ,
              G.app_name + " - External Command Error").resize(500, 500).ShowDialog();
          }
          return null;
        }
        while((line = p.StandardOutput.ReadLine()) != null) {
          Match m = Regex.Match(line, @"Radeon @ ([0-9.]+)/([0-9.]+) \(core/mem\)");
          if (m.Success) {
            return new float[] {
                                 float.Parse(m.Groups[1].Value, System.Globalization.NumberFormatInfo.InvariantInfo),
                                 float.Parse(m.Groups[2].Value, System.Globalization.NumberFormatInfo.InvariantInfo)
                               };
          }

        }

        return null;
      }

      public float[] set_clock(float core_clock, float mem_clock, bool ignore_errors) {
        Process p = new Process();
        p.StartInfo.FileName = oc_prog;
        p.StartInfo.Arguments 
          = (G.exp_radeon_setclk_minimize_step || G.ax.ac.exp_radeon_small_clocksteps ? "--min_step " : "")
          + core_clock.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)
          + " "
          + mem_clock.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        if (core_clock == 1.0 && mem_clock == 1.0)
          p.StartInfo.Arguments = "--default";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        //p.StartInfo.RedirectStandardError = true;
        //p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        p.WaitForExit();
        if (p.ExitCode != 0) {
          if (!ignore_errors) {
            MessageForm.open_dialog("Error while executing etc/radeon_setclk.exe:\r\n\r\n"
              + p.StandardOutput.ReadToEnd() + "\r\n"
              + "\r\nHint: There is an installer for r6probe.sys located in 3DProf's application directory\r\n"
              + "It can be installed from 3DProf startmenu too.\r\n"
              , G.app_name + " - External Command Error").ShowDialog();
          }
          return null;
        }
        string line;
        while((line = p.StandardOutput.ReadLine()) != null) {
          Match m = Regex.Match(line, @"Radeon @ ([0-9.]+)/([0-9.]+) \(core/mem\)");
          if (m.Success) {
            return new float[] {
                                 float.Parse(m.Groups[1].Value, System.Globalization.NumberFormatInfo.InvariantInfo),
                                 float.Parse(m.Groups[2].Value, System.Globalization.NumberFormatInfo.InvariantInfo)
                               };
          }
        }
        return null;
      }

      static string r6probe_debug_infos() {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
        try {
          if (Environment.OSVersion.Platform == System.PlatformID.Win32NT) {

            string r6probe_file = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\drivers\r6probe.sys";
            sb.Append("\r\n=== Some R6Probe related infos ===\r\n");
            sb.Append("r6probe.sys standard location: ");
            sb.Append(r6probe_file);
            sb.Append("\r\n");

            sb.Append("r6probe.sys file date: ");
            try {
              sb.Append(File.GetCreationTime(r6probe_file).ToShortDateString());
            } catch(Exception e) {
              sb.Append(" could not be retrieved (" + e.Message + ")");
            }
            sb.Append("\r\n");
            sb.Append("r6probe.sys file version: ");
            try {
              sb.Append(FileVersionInfo.GetVersionInfo(r6probe_file).FileVersion);
            } catch(Exception e) {
              sb.Append(" could not be retrieved (" + e.Message + ")");
            }
            sb.Append("\r\n");
            using(RegistryKey rek = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\r6probe")) {
              sb.Append("R6Probe standard registry key :");
              sb.Append(rek != null ? rek.Name : "not found");
              sb.Append("\r\n");
              sb.Append("r6probe.sys image path: ");
              try {
                sb.Append(rek.GetValue("ImagePath"));
              } catch(Exception e) {
                sb.Append(" could not be retrieved (" + e.Message + ")");
              }
              sb.Append("\r\n");
            }
            sb.Append("\r\n### Now searching for installed R6Probe drivers:\r\n");
            int found_drivers = 0;
            using (RegistryKey base_rek = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services")) {
              foreach(string rek_name in base_rek.GetSubKeyNames()) {
                using (RegistryKey rek = base_rek.OpenSubKey(rek_name)) {
                  if (rek == null) continue;
                  string imagePath = (string)rek.GetValue("ImagePath");
                  if (imagePath == null) continue;
                  if (imagePath.ToLower().IndexOf("r6probe.sys") == -1) continue;

                  ++found_drivers;
                  sb.Append("  ### Found one:\r\n");
                  sb.Append("    Key Name: ");
                  sb.Append(rek.Name);
                  sb.Append("\r\n");
                  sb.Append("    ImagePath: ");
                  sb.Append(imagePath);
                  sb.Append("\r\n");

                  string winroot = Environment.GetEnvironmentVariable("SystemRoot");
                  winroot = (winroot != null) ? winroot + @"\" : "";
                  bool absolute_imagPath = (imagePath[1] == ':');
                  r6probe_file = absolute_imagPath ? imagePath
                    : (winroot != null && File.Exists(winroot + imagePath)) ? winroot + imagePath
                    : null;
                
                  if (r6probe_file != null) {
                    if (absolute_imagPath) {
                      sb.Append("    Full Path (guessed): ");
                      sb.Append(r6probe_file);
                      sb.Append("\r\n");
                    }
                    sb.Append("    File Date: ");
                    try {
                      sb.Append(File.GetCreationTime(r6probe_file).ToShortDateString());
                    } catch(Exception e) {
                      sb.Append(" could not be retrieved (" + e.Message + ")");
                    }
                    sb.Append("\r\n");
                    sb.Append("    File version: ");
                    try {
                      sb.Append(FileVersionInfo.GetVersionInfo(r6probe_file).FileVersion);
                    } catch(Exception e) {
                      sb.Append(" could not be retrieved (" + e.Message + ")");
                    }
                    sb.Append("\r\n");
  
                  }
            
                }
              }
            }

            sb.AppendFormat("### Number of installed drivers found: {0} ===\r\n", found_drivers);

          }
        } catch {
#if DEBUG
          throw;
#endif
        }

        return sb.ToString();
      }

      static readonly string oc_prog = "etc/radeon_setclk.exe";
    }

    class clocking_nvidia : clocking_card {

      public clocking_nvidia() {
        if (!is_supported())
          throw new FatalError("unsupported card");
      }

      public static bool is_supported() {
        return (G.is_vendor_nvidia && File.Exists(oc_prog)
          && (Environment.OSVersion.Platform == System.PlatformID.Win32NT));
      }

      public float[] get_clock(bool ignore_errors) {
        string external_clocker = oc_prog;
        if (!Utils.file_exists(external_clocker))
          return null;
        Process p = new Process();
        p.StartInfo.FileName = external_clocker;
        p.StartInfo.Arguments = "-1 --speeds";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        //p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        p.WaitForExit();
        string line;
        if (p.ExitCode != 0) {
          if (!ignore_errors) {
            MessageForm.open_dialog("Error while executing " + external_clocker + ":\r\n\r\n"
              + p.StandardOutput.ReadToEnd() + "\r\n"
              + p.StandardError.ReadToEnd() + "\r\n"
              ,
              G.app_name + " - External Command Error").ShowDialog();
          }
          return null;
        }
        string s_core = null;
        string s_mem = null;
        while((line = p.StandardOutput.ReadLine()) != null) {
          Match m = Regex.Match(line, @"^(Core|Memory) speed:\s+([0-9.]+)\s+MHz");
          if (m.Success) {
            if (m.Groups[1].Value == "Core")
              s_core = m.Groups[2].Value;
            else
              s_mem = m.Groups[2].Value;
          }
          if (s_core != null && s_mem != null)
            return new float[] { float.Parse(s_core, System.Globalization.NumberFormatInfo.InvariantInfo),
                                 float.Parse(s_mem, System.Globalization.NumberFormatInfo.InvariantInfo) };
        }
        return null;
      }

      public float[] set_clock(float core_clock, float mem_clock, bool ignore_errors) {
        string external_clocker = oc_prog;
        if (!Utils.file_exists(external_clocker))
          return null;
        Process p = new Process();
        p.StartInfo.FileName = external_clocker;
        p.StartInfo.Arguments = "-1 --force"
          + " -n " + core_clock.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)
          + " -m " + mem_clock.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        if (core_clock == 1.0 && mem_clock == 1.0)
          p.StartInfo.Arguments = "-1 --reset";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        //System.Convert.ToString(core_clock,
        p.StartInfo.RedirectStandardError = true;
        //p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        p.WaitForExit();
        if (p.ExitCode != 0) {
          if (!ignore_errors) {
            MessageForm.open_dialog("Error while executing " + external_clocker + ":\r\n\r\n"
              + p.StandardOutput.ReadToEnd() + "\r\n"
              + p.StandardError.ReadToEnd() + "\r\n"
              , G.app_name + " - External Command Error").ShowDialog();
          }
          return null;
        }
        string line;
        string s_core = null;
        string s_mem = null;
        while((line = p.StandardOutput.ReadLine()) != null) {
          Match m = Regex.Match(line, @"^(Core|Memory) speed:\s+([0-9.]+)\s+MHz");
          if (m.Success) {
            if (m.Groups[1].Value == "Core")
              s_core = m.Groups[2].Value;
            else
              s_mem = m.Groups[2].Value;
          }
          if (s_core != null && s_mem != null)
            return new float[] { float.Parse(s_core, System.Globalization.NumberFormatInfo.InvariantInfo),
                                 float.Parse(s_mem, System.Globalization.NumberFormatInfo.InvariantInfo) };
        }
        return null;
      }
      static readonly string oc_prog = "etc/nvclock.exe";
    }
    #endregion
    #endregion
  }

}
