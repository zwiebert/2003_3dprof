using System;
using IWshRuntimeLibrary;

namespace TDProf
{
	/// <summary>
	/// Summary description for shell_link.
	/// </summary>
	public class Link
	{
		WshShell shell_ = new WshShell();

		public Link()
		{
		}



		/// <summary>
		/// create a link on the Desktop
		/// </summary>
		/// <param name="link_path"></param>
		/// <returns></returns>
		public bool create_link(GameProfileData gpd)
		{
			string link_path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory)
				+ @"\" + gpd.profile_descriptor + ".lnk";

			string target_dir = System.Environment.CurrentDirectory;
			string target_path = target_dir + @"\tdprof.exe";
			string target_args = " -run \"" + gpd.profile_descriptor + "\"";

			IWshShortcut link = (IWshShortcut)shell_.CreateShortcut(link_path);
			link.TargetPath = target_path;
			link.Arguments = target_args;
			link.WorkingDirectory = target_dir;
			// if we have an exe, let's use its icron
			string icon_file = ((gpd.exe_path != null && gpd.exe_path.Length > 0)
				? gpd.exe_path 
				: target_path);

			link.IconLocation = icon_file;
			link.Save();

			return true;
		}
	}
}
