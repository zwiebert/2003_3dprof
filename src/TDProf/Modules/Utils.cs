using System;
using System.IO;

namespace TDProf.Util {
  /// <summary>
  /// Some general static helper functions
  /// </summary>
  public class Utils {
    /// <summary>
    /// Provide a default string in case the actual string is null
    /// </summary>
    /// <param name="first_choice">string or null</param>
    /// <param name="second_choice">default string</param>
    /// <returns>return first_choice if non null, else return second choice.</returns>
    public static string alt_string(string first_choice, string second_choice) {
      return (first_choice != null) ? first_choice : second_choice;
    }

    /// <summary>
    /// Test if file exists.
    /// </summary>
    /// <param name="name">path to a file</param>
    /// <returns>true if file exists</returns>
    public static bool file_exists(string name) {
      if (name == null || name.Length == 0) return false;
      return File.Exists(name);
    }

    public static string extract_directory(string path_name) {
      return System.IO.Path.GetDirectoryName(path_name);
    }

    public static int max(int a, int b) { return (a > b) ? a : b; }
    public static int min(int a, int b) { return (a > b) ? a : b; }
    public static int limited(int a, int min, int max) {
      return ((a < min) ? min : (a > max) ? max : a);
    }

  }
}
