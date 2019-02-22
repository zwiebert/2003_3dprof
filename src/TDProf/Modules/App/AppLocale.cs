using System;
using System.Resources;

namespace TDProf
{
	/// <summary>
	/// Summary description for AppLocale.
	/// </summary>
	public class AppLocale
	{
      static System.Resources.ResourceManager res;
      public readonly string test = (string)res.GetObject("test");
      public readonly string msgbox_txt_help_tdprofgd = ((string)res.GetObject("msgbox_txt_help_tdprofgd"));
      public readonly string msgbox_title_help_tdprofgd = ((string)res.GetObject("msgbox_title_help_tdprofgd"));
      public readonly string em_profile_does_not_exist = ((string)res.GetObject("em_profile_does_not_exist"));
      public readonly string em_matching_specfile_does_not_found = ((string)res.GetObject("em_matching_specfile_does_not_found"));
      public readonly string em_safe_mode_enabled_nongui = ((string)res.GetObject("em_safe_mode_enabled_nongui"));
      public readonly string em_safe_mode_enabled_gui_forced_specfile  = ((string)res.GetObject("em_safe_mode_enabled_gui_forced_specfile"));
      public readonly string em_safe_mode_enabled_gui = ((string)res.GetObject("em_safe_mode_enabled_gui"));
      public readonly string em_safe_mode_enabled_gui_new = ((string)res.GetObject("em_safe_mode_enabled_gui_new"));
      public readonly string em_fmt_provided_specfile_0_not_exists = ((string)res.GetObject("em_fmt_provided_specfile_0_not_exists"));

      public readonly string fmt_summary_of_profile_0 = ((string)res.GetObject("fmt_summary_of_profile_0"));
      public readonly string fmt_summary_of_profile_0_attached_to_display_adapter_1 = ((string)res.GetObject("fmt_summary_of_profile_0_attached_to_display_adapter_1"));
      public readonly string msgbox_txt_autorestore_exit_to_soon = ((string)res.GetObject("msgbox_txt_autorestore_exit_to_soon"));
      public readonly string msgbox_title_autorestore_exit_to_soon = ((string)res.GetObject("msgbox_title_autorestore_exit_to_soon"));
      public readonly string msgbox_txt_safe_mode_reenabled = (string)res.GetObject("msgbox_txt_safe_mode_reenabled");
      public readonly string msgbox_title_safe_mode_reenabled = (string)res.GetObject("msgbox_title_safe_mode_reenabled");
      public readonly string msgbox_title_err_prepostcommand = ((string)res.GetObject("msgbox_title_err_prepostcommand"));
      public readonly string em_fmt_cannot_execute_file_0 = ((string)res.GetObject("em_fmt_cannot_execute_file_0"));
      public readonly string msgbox_title_user_error = ((string)res.GetObject("msgbox_title_user_error"));
      public readonly string em_file_not_found = ((string)res.GetObject("em_file_not_found"));
      public readonly string em_cant_continue = ((string)res.GetObject("em_cant_continue"));
      public readonly string msgbox_title_unrecoverable_error = (string)res.GetObject("msgbox_title_unrecoverable_error");
      public readonly string msgbox_title_unhandled_exception = (string)res.GetObject("msgbox_title_unhandled_exception");
      public readonly string em_fmt_unhandled_execption_0_occured = ((string)res.GetObject("em_fmt_unhandled_execption_0_occured"));
      public readonly string msgbox_txt_autorestore_dialog_forced = ((string)res.GetObject("msgbox_txt_autorestore_dialog_forced"));
      public readonly string help_text_intro = ((string)res.GetObject("help_text_intro"));
      public readonly string text_prof_enter_new_name_here = ((string)res.GetObject("text_prof_enter_new_name_here"));
#if false
      public readonly string help_text_intro = ((string)res.GetObject("help_text_intro"));
      public readonly string msgbox_title_unhandled_exception = (string)res.GetObject("msgbox_title_unhandled_exception");
      public readonly string s = (string)res.GetObject("");
      public readonly string s = (string)res.GetObject("");
      public string s = (string)res.GetObject("");
      public string s = (string)res.GetObject("");
#endif
		private AppLocale()
		{
			//
			// TODO: Add constructor logic here
			//
          res = null;
		}
      static public AppLocale create() {
        res = new System.Resources.ResourceManager(typeof(AppLocale));
        AppLocale obj = new AppLocale();
        res = null;
        return obj;
      }
	}
}
