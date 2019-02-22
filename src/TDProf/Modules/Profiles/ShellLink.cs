using System;
using MSjogren.Samples.ShellLink;

namespace TDProf.Profiles {
  /// <summary>
  /// Summary description for shell_link.
  /// </summary>
  public class Link {
    private Link() {
    }

    public static bool exists_link(Profiles.GameProfileData gpd, string prefix, string suffix) {
      string link_path = link_file_name(gpd, prefix, suffix);
      return (System.IO.File.Exists(link_path));
    }

    public static string link_file_name(Profiles.GameProfileData gpd, string prefix, string suffix) {
      string link_file = prefix + gpd.name + suffix + ".lnk";
      string link_path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory)
        + @"\" + link_file.Replace(@":", "=").Replace(@"\", ";").Replace(@";", "_");
      return link_path;
    }

    public static void delete_link(Profiles.GameProfileData gpd, string prefix, string suffix) {
      string link_path = link_file_name(gpd, prefix, suffix);
      System.IO.File.Delete(link_path);
    }

    public static bool create_link(Profiles.GameProfileData gpd, string prefix, string suffix) {
      string link_path = link_file_name(gpd, prefix, suffix);

      string target_dir = System.Environment.CurrentDirectory;
      string target_path = target_dir + @"\tdprof.exe";
      string target_args = " -run \"" + gpd.name + "\"";

      // if we have an exe, let's use its icon
      string icon_file = ((gpd.exe_path != null && gpd.exe_path.Length > 0)
        ? gpd.exe_path 
        : target_path);

      ShellShortcut link = new ShellShortcut(link_path);
      link.Path = target_path;
      link.Arguments = target_args;
      link.WorkingDirectory = target_dir;
      link.IconPath = icon_file;
      link.Save();
      return true;
    }

    /// <summary>
    /// create a link on the Desktop
    /// </summary>
    /// <param name="link_path"></param>
    /// <returns></returns>
    public static bool create_link(Profiles.GameProfileData gpd) {
      return create_link(gpd, "", "");
    }
  }
}
