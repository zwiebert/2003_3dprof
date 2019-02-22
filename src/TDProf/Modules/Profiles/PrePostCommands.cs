using System;
using System.IO;
using System.Diagnostics;
using TDProf.DriverSettings;
using TDProf.Profiles;
using TDProf.Util;
using TDProf.App;
using System.Text.RegularExpressions;

namespace TDProf
{
	/// <summary>
	/// Summary description for PrePostCommands.
	/// </summary>
	public class PrePostCommands
	{
      public static void run_pre_commands(GameProfileData gpd) {
        char[] nl_delim = {'\r', '\n' };

        if (gpd != null) {
          string[] cmds = gpd.command_pre_exe_run.Split(nl_delim);
          run_commands(cmds);
        }

        if (gpd == null || gpd.command_pre_exe_run_glob) {
          string[] cmds = G.ax.ac.command_pre_exe_run.Split(nl_delim);
          run_commands(cmds);
        }

      }

      public static void run_post_commands(GameProfileData gpd) {
        char[] nl_delim = {'\r', '\n' };

        if (gpd != null) {
          string[] cmds = gpd.command_post_exe_exit.Split(nl_delim);
          run_commands(cmds);
        }

        if (gpd == null || gpd.command_post_exe_exit_glob) {
          string[] cmds = G.ax.ac.command_post_exe_exit.Split(nl_delim);
          run_commands(cmds);
        }
      }

      static int run_commands(string[] cmds) {
        int executed_programms = 0;

        foreach(string s in cmds) {
          string cmd = s.Trim();
          if (cmd.Length == 0)
            continue;
          try {
            Process proc = null;
            string prog = null;
            string args = null;

            // internal commands
            if (cmd.StartsWith(":")) {
              Match m = Regex.Match(cmd, @":(?<prog>\S+)\s+(?<args>.*)$");
              switch (m.Groups["prog"].Value.ToLower()) {
                case "resolution":
                  MatchCollection mc = Regex.Matches(m.Groups["args"].Value, @"((?<nmb>\d):)?(?<hres>\d+)x(?<vres>\d+)@(?<vfreq>\d+)(Hz)?(,(?<bpp>\d+)bpp)?");
                  foreach(Match ma in mc) {
                    DisplayInfo.set_resolution(
                      ma.Groups["nmb"].Success ?  int.Parse(ma.Groups["nmb"].Value) : 1,
                      int.Parse(ma.Groups["hres"].Value),
                      int.Parse(ma.Groups["vres"].Value),
                      int.Parse(ma.Groups["vfreq"].Value),
                      ma.Groups["bpp"].Success ? int.Parse(ma.Groups["bpp"].Value) : 0
                      ); 
                  }
                  break;
                default: break;
              }
             
            } else if (cmd.StartsWith(@"""")) {
              Match m = Regex.Match(cmd, @"""(?<prog>[^""]+)""(?:\s+(?<args>.*))?$");
              if (m.Success) {
                prog = m.Groups["prog"].Value;
                if (m.Groups["args"].Success)
                  args = m.Groups["args"].Value; 
              }
            } else {
              Match m = Regex.Match(cmd, @"(?<prog>\S+)(?:\s+(?<args>.*))?$");
              if (m.Success) {
                prog = m.Groups["prog"].Value;
                if (m.Groups["args"].Success)
                  args = m.Groups["args"].Value; 
              }
            }

            if (prog != null)
              proc = (args == null) ? Process.Start(prog) : Process.Start(prog, args); 
            if (proc != null)
              proc.WaitForExit();
            ++executed_programms;
          } catch (Exception) {
            System.Windows.Forms.MessageBox.Show(string.Format(G.loc.em_fmt_cannot_execute_file_0, cmd), G.loc.msgbox_title_err_prepostcommand);
          }
        }

        return executed_programms;
      }

		public PrePostCommands()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
