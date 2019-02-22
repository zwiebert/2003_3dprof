// C# version of r6clock-dll.h
using System;
using System.Runtime.InteropServices;

namespace Win32 {

  class R6Clock {

    [StructLayout(LayoutKind.Sequential)]
      public struct r6clock_clocks { 
      public UInt32 core_khz;
      public UInt32 ram_khz;
    }

    [DllImport("r6clock.dll")]
    public static extern r6clock_clocks r6clock_get_clock();

    [DllImport("r6clock.dll")]
    public static extern r6clock_clocks r6clock_get_default_clock();

    [DllImport("r6clock.dll")]
    public static extern r6clock_clocks r6clock_set_clock(UInt32 core_khz, UInt32 ram_khz);

    [DllImport("r6clock.dll")]
    public static extern UInt32 r6clock_get_id();


    [DllImport("r6clock.dll")]
    public static extern UInt32 r6clockdiag_get_info(System.Text.StringBuilder buf, UInt32 buf_len);

  }

}

