#define NEW_SHORT_FORMAT

using System;
using ArrayList=System.Collections.ArrayList;
using Microsoft.Win32;
using RegDiff;

namespace RegDiff {
  public class RegDiff {
    public RegDiff(RegistryKey base_key, string sub_key_name) {
      rk = base_key.OpenSubKey(sub_key_name);
    }

    public string test_rdkey() {
      RdKey prv_rdk = rdk;
      rdk = new RdKey(rk);
      return ((prv_rdk == null) ? null : rdk.hr_diff(prv_rdk));
    }

    private RdKey rdk = null;
    private RegistryKey rk;
  }


  //----------------------------------------------------------------

  class RdKey : Object {
    string name_m;
    RdKey[] subkeys_m = null;
    string[] vals_m = null;
    Object[] data_m = null;

    public RdKey(RegistryKey rk) {
      name_m = rk.Name;

      vals_m = rk.GetValueNames();
      if (vals_m != null)
        data_m = new Object[vals_m.Length];
      for (int i=0; i < vals_m.Length; ++i) {
        data_m[i] = rk.GetValue(vals_m[i]);
      }

      string[]subkey_names = rk.GetSubKeyNames();
      if (subkey_names != null) {
        subkeys_m = new RdKey[subkey_names.Length];
        for (int i=0; i < subkeys_m.Length; ++i) {
          RegistryKey sk = rk.OpenSubKey(subkey_names[i]);
          subkeys_m[i] = new RdKey(sk);
        }
      }
    }

    /// <summary>
    /// Test if two objects are equal or not 
    /// </summary>
    /// <param name="lhs">Object with one of types String, Byte[] or Int32</param>
    /// <param name="rhs">Object with the very same type as LHS</param>
    /// <returns>true if equal</returns>
    bool eq_object(Object lhs, Object rhs) {
      if (lhs.GetType() != rhs.GetType())
        return false;

      string type_name = lhs.GetType().Name;
      if (lhs.GetType().Name == "Byte[]") {
        byte[] l = (byte[])lhs;
        byte[] r = (byte[])rhs;
        if (l.Length != r.Length) return false;
        for (int i=0; i < l.Length; ++i)
          if (l[i] != r[i])
            return false;
        return true;
      } else if (lhs.GetType().Name == "Int32") {
        return (Int32)lhs == (Int32)rhs;
      } else {
        string s = lhs.ToString();
        return lhs.ToString() == rhs.ToString();
      }
    }
    /// <summary>
    /// Create human readable string showing content of an object
    /// </summary>
    /// <param name="o">Object with one of types String, Byte[] or Int32</param>
    /// <returns>formatted text</returns>
    string hr_object(Object o) {
      string res = "";
      string type_name = o.GetType().Name;
      if (type_name == "String") {
        res = "\"" + o.ToString() + "\"";
      } else if (type_name == "Byte[]") {
        res = "hex:";
        byte[] ba = (byte[])o;
        string sep = "";
        foreach (byte b in ba) {
          res += sep + String.Format("{0:x2}", b);
          sep = ",";
        }
      } else if (type_name == "Int32") 
        res = String.Format("dword:{0:x8}", (Int32)o);
  

      return res;
    }

    /// <summary>
    /// Return a human readable string containing differences between THIS and OTHER. Works recursively for sub keys contained in THIS.
    /// </summary>
    /// <param name="other">An object older than THIS. It has to be older, because statements as "changed from ... to are part of the written output.</param>
    /// <returns>formatted text</returns>
    public string hr_diff(RdKey other) {
      string msg = "";

      // do recursion on subkeys
      if (this.subkeys_m.Length > 0 && other.subkeys_m.Length > 0) {
        for (int i=0; i < subkeys_m.Length; ++i)
          for (int ii=0; ii < other.subkeys_m.Length; ++ii)
            if (this.subkeys_m[i].name_m == other.subkeys_m[ii].name_m) {
              string s = this.subkeys_m[i].hr_diff(other.subkeys_m[ii]);
              if (s != null)
                msg += s;
              break;
            }
      }

      // Find values whose data has changed
      // Find values existing in THIS but not in OTHER ("created in THIS")
      RdKey a = this, b = other;
      for (int i=0; i < a.vals_m.Length; ++i) {
        for (int ii=0; ii < b.vals_m.Length; ++ii) {
          if (b.vals_m[ii] == a.vals_m[i]) {
            if (!eq_object(b.data_m[ii], a.data_m[i])) {
#if NEW_SHORT_FORMAT
              msg += string.Format(@"*** Value Changed: ""{0}""={1}             [Old data was {2}]" + "\r\n",
                a.vals_m[i],
                hr_object(a.data_m[i]),
                hr_object(b.data_m[ii]));
#else
              msg += "*** Value: \"" + a.vals_m[i] + "\" changed"
                + " from " + hr_object(b.data_m[ii])
                + " to " + hr_object(a.data_m[i])
                + ".\r\n";
#endif
            }
            goto value_found;
          }
        }
#if NEW_SHORT_FORMAT
        msg += string.Format(@"*** Value Created: ""{0}""={1}" + "\r\n",
          a.vals_m[i],
          hr_object(a.data_m[i]));
#else
        msg += "*** Value: \"" + a.vals_m[i] + "\"=" + hr_object(a.data_m[i]) + " created" + "\r\n";
#endif
      value_found:
        continue;
      }

      // Find values existing in OTHER but not in THIS ("removed from THIS")
      a = other; b = this;
      for (int i=0; i < a.vals_m.Length; ++i) {
        for (int ii=0; ii < b.vals_m.Length; ++ii) {
          if (b.vals_m[ii] == a.vals_m[i]) {
            goto value_found;
          }
        }
#if NEW_SHORT_FORMAT
        msg += string.Format(@"*** Value Removed: ""{0}""" + "\r\n",
          a.vals_m[i],
          hr_object(a.data_m[i]));
#else
        msg += "*** Value: \"" + a.vals_m[i] + "\"=" + hr_object(a.data_m[i]) + " removed" + "\r\n";
#endif
      value_found:
        continue;
      }

      if (msg != "") {
        return "** Key: \"" + name_m + "\"\r\n" + msg;
      }

      return null;
    }  
  }
}
