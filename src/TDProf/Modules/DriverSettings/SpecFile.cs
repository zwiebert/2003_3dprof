using System;
using System.IO;
using TDProf.Util;


namespace TDProf.DriverSettings {
  /// <summary>
  /// Summary description for SpecFile.
  /// </summary>
  public class SpecFile {
    DisplayInfo di_;
    public readonly string config_file_name;
    public string chip_name_;
    public readonly string spec_name;


    public SpecFile(DisplayInfo di) {
      di_ = di;
      config_file_name = this.create_cr_config_file_name();
      if (config_file_name != null)
          spec_name = config_file_name.Replace("-win4.cfg", "").Replace("-win5.cfg", "").Replace("-win6.cfg", "").Replace("-win.cfg", "").Replace("specfiles/", "").Replace("usr-specfiles/", "");
    }

    /// <summary>
    /// get valid config file name for current hardware.
    /// For supported vendors we use the vendor name in filename.
    /// For currently unsupported vendors we use the vendor number like in v1002-win5.cfg.
    /// The user can use its own vNNNN-winN.cfg file until the program does support the card.
    ///
    /// TODO:
    /// If a file VENDOR-DEVID-WINVER.cfg exist, we use it instead VENDOR-WINVER.cfg.
    /// We should have an additional map file which defines groups of device number
    /// who have the same features:
    /// ;VEN: DEVID ... : Alias ...
    /// 1002: 4E44 4E45 : R300
    /// 
    /// </summary>
    /// <returns></returns>
    string create_cr_config_file_name() {
      string dev_id = di_.get_device_id();
      string ven_id = di_.get_vendor_id().ToLower();

      string prefix = "specfiles/";
      string suffix = ".cfg";
      string[] win_parts = {
                             "win" + Environment.OSVersion.Version.Major.ToString(),
                             "win" };
	  
      string ven_part = ((ven_id == "10de") ? "nvidia"
        : ((ven_id == "1002") ? "ati"
        : ven_id.ToString()));
      string file_name;

      string[] aliases = this.find_devpart_alias(ven_id, dev_id);
      chip_name_ = aliases[0];

      // before we try aliases, let's see if we have an exact match by device ID
      foreach (string win_part in win_parts) {
        file_name = prefix + ven_part + "-" + dev_id + "-" + win_part + suffix;
        if (Utils.file_exists("usr-" + file_name))
          return "usr-" + file_name;
        else if (Utils.file_exists(file_name))
          return file_name;
          //FIXME: If the file is named with the ID instead the vendor name (nvidia, ati) do the following. This should be cleaned up
        if (ven_id != ven_part)
        {
            file_name = prefix + ven_id + "-" + dev_id + "-" + win_part + suffix;
            if (Utils.file_exists("usr-" + file_name))
                return "usr-" + file_name;
            else if (Utils.file_exists(file_name))
                return file_name;
        }
      }



      // try if we have a match by chip name (index 0) line nv17 or a different
      // device ID used for the same chip (e.g. 0170, 0171, 0172 are all nv17 chips),
      // so we can use a nv-0172-win5.cfg file, if we have detected a 0170.
      foreach (string dev in aliases) {
        foreach (string win_part in win_parts) {
          file_name = prefix + ven_part + "-" + dev + "-" + win_part + suffix;
          if (Utils.file_exists("usr-" + file_name))
            return "usr-" + file_name;
          else if (Utils.file_exists(file_name))
            return file_name;
        }
      }

      // try if we have a match of a chip name with same capabilities (like NV17, NV18)
      aliases = this.find_devpart_alias(ven_id, aliases[0]);
      foreach (string dev in aliases) {
        foreach (string win_part in win_parts) {
          file_name = prefix + ven_part + "-" + dev + "-" + win_part + suffix;
          if (Utils.file_exists("usr-" + file_name))
            return "usr-" + file_name;
          else if (Utils.file_exists(file_name))
            return file_name;
        }
      }

      /* TODO: here we should process an alias array or file */
      foreach (string win_part in win_parts) {
        file_name = prefix + ven_part + "-" + win_part + suffix;
        if (Utils.file_exists("usr-" + file_name))
          return "usr-" + file_name;
        else if (Utils.file_exists(file_name))
          return file_name;
      }

      return null;
    }

		#region CHIPDATA
    /// <summary>
    /// Array of device ids and aliases grouped by same capabilities.
    /// If a device is in the wrong group, don't panic. The user can always create a new config
    /// file using the exact device id in name (e.g. ".\specfiles\nv-01f0-win5.cfg")
    // NOTE: all strings must be lower case
    /// </summary>
    struct DevIDMap {
      public string ven_id;
      public string[][] dev_ids; // array of device id groups
      public DevIDMap(string vendor, string[][] devices) { ven_id = vendor; dev_ids = devices; }
    };
    static DevIDMap[] dev_id_maps
      = { new DevIDMap("10de", new string[][] {
	 //------------- Groups of chip names with same caps -----------------
                                                new string[] {"gf4mx", "nv17", "nv18", "cr17", },
                                                new string[] {"gf4ti", "nv25", "nv28", },
                                                new string[] {"gffx", "nv30", "nv35", "nv34", "nv31", },
                                                new string[] {"gf6", "nv40", "nv41", "nv43", "nv45", },
                                                //--------------------------------------------------------------------
                                                //------------- Map chip name to device IDs --------------------------
                                                //--------------------------------------------------------------------
                                                new string[] {"nv15", "0150", "0151", "0152", "0153",},
                                                //--------------------------------------------------------------------
                                                new string[] {"nv17", "0170", "0171", "0172", "0173", "0178", "017a",},
                                                new string[] {"nv18", "0181", "0182", "0183", "0188", "018a", "018b",},
                                                new string[] {"cr17", "01f0",},
                                                new string[] {"nv20", "0200", "0201", "0202", "0203",},
                                                //--------------------------------------------------------------------
                                                new string[] {"nv25", "0250", "0251", "0253", "0258", "0259", "025b",},
                                                new string[] {"nv28", "0280", "0281", "0282", "0281", "0282", "0288", "0289",},
                                                //--------------------------------------------------------------------
                                                new string[] {"nv31", "0311", "0312", "0313",},
                                                //--------------------------------------------------------------------
                                                new string[] {"nv34", "0321", "0322", "0323", "032a", "032b", "032f",},
                                                //--------------------------------------------------------------------
                                                new string[] {"nv30", "0301", "0302", "0308", "0309"},
                                                //--------------------------------------------------------------------
                                                new string[] {"nv35", "0330", "0331", "0338"},
                                                //--------------------------------------------------------------------
                                                new string[] {"nv40", "0040", "0041", "0042", "0045", "004E", },
                                                new string[] {"nv41", "00C1", "00CE", },
                                                new string[] {"nv43", "0140", "0141", "0145", "014E", "014F", } 


        }), new DevIDMap("1002", new string [][] {
                                                   //------------- Groups of chip names with same caps -----------------
                                                   new string[] {"r300", "r350", "r360", "r420"},
                                                   //--------------------------------------------------------------------
                                                   //--------------------------------------------------------------------
                                                   new string[] {"r200", "514c", "514d", },
                                                   new string[] {"rv200", "5157", },
                                                   new string[] {"r300", "4144", "4e45", "4e44", "4e46"},
                                                   new string[] {"r350", "4e49", "4e48", },
                                                   new string[] {"r360", "4e4a",},
                                                   new string[] {"rv350", "4152", "4150", },
                                                   new string[] {"rv280", "5961", "5960", },
                                                   new string[] {"rv420", "4a49", "4a4b", "4a50", }

        }),};
		#endregion CHIPDATA

    /// <summary>
    /// return an array of valid aliases for given device
    /// (NOTE: in current implementation we return only one aliase per device)
    /// </summary>
    /// <param name="vendor"></param>
    /// <param name="device"></param>
    /// <returns></returns>
    string[] find_devpart_alias(string vendor, string device) {
      device = device.ToLower();
      vendor = vendor.ToLower();

      //DEBUG	if (device == "0302") return new string[] {"nv18", "0170", "0171", "0172", "0173", "0178", "017a",};

      foreach (DevIDMap dim in dev_id_maps)
        if (dim.ven_id == vendor)
          foreach (string[] devs in dim.dev_ids)
            foreach (string dev in devs)
              if (dev == device)
                return devs;

      return new string[] { device };
    }

  }
}
