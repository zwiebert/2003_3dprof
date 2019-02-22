using System;
using System.Drawing;
using System.Net;
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
using Microsoft.Win32;

namespace TDProf.App
{
	/// <summary>
	/// Summary description for AppRunProfile.
	/// </summary>
	public class AppRunProfile
	{
    Thread m_dialog_thread, m_game_thread;
    AppContext m_ax = G.ax;
    GameProfileData gpd, def_gpd;
    AutoRestore ar = null;
    TDProf.GUI.FormMain m_frm;
    double m_proc_duration = 0;
    public delegate void notify_me();
    notify_me m_call_at_restore;
    bool apply_done = false; // if not set, dont wait nor open any dialogs (used by Run button)
    bool is_autorestore_enabled {
      get {
        return (gpd != null && !gpd.autorestore_force_disable)
          && ((G.is_gui_loaded && G.ax.ac.auto_restore_after_exit_in_gui)
          || (!G.is_gui_loaded && G.ax.ac.auto_restore_after_exit));
      }
    }


		public AppRunProfile(GameProfileData a_gpd, notify_me a_call_at_restore)
		{
      m_call_at_restore = a_call_at_restore;
      gpd = new GameProfileData(a_gpd); // clone it to modify from commandline parameters
      m_dialog_thread = new Thread(new ThreadStart(show_autorestore_dialog));
      m_game_thread = new Thread(new ThreadStart(run_game));
      def_gpd = m_ax.gp.get_profile(m_ax.ac.prof_default);
      ar = new AutoRestore();
      ar.save_state(m_ax.cr);
      // store to file, in case we don't want to wait for game-exit ourself
      ar.store_to_file("3d_settings_saved_by_run.bin");


      // handle option "-alt_exe"
      if (m_ax.cl.optarg_alt_exe != null) {
        gpd.exe_path = m_ax.cl.optarg_alt_exe;
        gpd.exe_args = "";
      }

      // passing parameters to game
      if (m_ax.cl.opt_run_and_pass)
        gpd.exe_args = " " + G.ax.cl.unkown_args;

    }

    void check_safe_mode() {
      // warn user about Safe Mode
      if (m_ax.cr.flag_registry_readonly)
        MessageBox.Show(G.loc.em_safe_mode_enabled_nongui, G.app_name + "");
    }

    void mount_images() {
      if (!gpd.mount_img(m_ax.ac.img_daemon_exe_path)) {
        throw new FatalError("CDROM image could not be mounted.");
      }
    }

    void unmount_images() {
      gpd.unmount_img(m_ax.ac.img_daemon_exe_path);
    }

    void process_commandline() {

      // handle option "-alt_exe"
      if (m_ax.cl.optarg_alt_exe != null) {
        gpd.exe_path = m_ax.cl.optarg_alt_exe;
        gpd.exe_args = "";
      }

      // passing parameters to game
      if (m_ax.cl.opt_run_and_pass)
        gpd.exe_args = " " + G.ax.cl.unkown_args;

    }

    void commands_before_game() {
      if (gpd.exe_path != "") {
        PrePostCommands.run_pre_commands(gpd);
      }
    }

    void commands_after_game() {
      if (gpd.exe_path != "") {
        PrePostCommands.run_post_commands(gpd);
      }
    }

    void show_autorestore_dialog() {
      MessageBox.Show((gpd.autorestore_force_dialog ? G.loc.msgbox_txt_autorestore_dialog_forced
        : G.loc.msgbox_txt_autorestore_exit_to_soon),
        G.loc.msgbox_title_autorestore_exit_to_soon);
    }


    public void restore_now() {
      if (m_ax.ac.auto_restore_to_default_profile) {
        if (def_gpd != null) {
          m_ax.cr.apply_prof(def_gpd, false);
          m_ax.cr.ati_apply_d3d(false);
        }
      } else {
        if (ar != null) {
          ar.restore_state(m_ax.cr); 
          m_ax.cr.ati_apply_d3d(false);
        }
      }

      unmount_images();
      commands_after_game();

      if (m_ax.ac.clocking_restore_kind != GameProfileData.EClockKinds.PARENT)
        m_ax.cr.apply_prof_clocking_only(gpd, true);

    }

    public void apply_profile() {
      m_ax.cr.apply_prof(gpd, false);
      mount_images();
      apply_done = true;
    }

    void run_game() {
      commands_before_game();

      if (gpd.autorestore_force_dialog)
        m_dialog_thread.Start();

      Process proc = gpd.run_exec();

      if (proc == null || !apply_done)
        return;

      if (gpd.autorestore_force_dialog) {
        m_dialog_thread.Join();

      } else if (gpd.autorestore_disable_dialog || !is_autorestore_enabled) {
        proc.WaitForExit();

      } else {
        bool need_dialog = true;

        try {
          DateTime start_time = proc.StartTime;
          proc.WaitForExit();
          TimeSpan time_elapsed = proc.ExitTime - start_time;
          m_proc_duration = time_elapsed.TotalSeconds;
          need_dialog = m_proc_duration < 120;
        } catch { }

        if (need_dialog)
            show_autorestore_dialog();
      }

      if (is_autorestore_enabled) {
        if (m_call_at_restore != null)
          m_call_at_restore();
        else
          restore_now();
      }
    }


    public void run_profile() {
      if (m_call_at_restore == null) {
        run_game();
      } else {
        m_game_thread.Start();
      }
    }

	}
}
