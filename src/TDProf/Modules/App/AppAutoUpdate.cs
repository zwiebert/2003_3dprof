using System;
using System.Runtime.InteropServices;
using Win32;

namespace TDProf.Modules.App
{
	/// <summary>
	/// Summary description for AppAutoUpdate.
	/// </summary>
	public class AppAutoUpdate
	{
		public AppAutoUpdate()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    public static bool is_online() { return InternetQueryOptionTest.is_online(); } 
	}



  class InternetQueryOptionTest {
    // For INTERNET_OPTION_PROXY

    public static bool is_online() {
      int cb = 0;
      uint state = 0;
      // get size into cb
      WinInet.InternetQueryOption(IntPtr.Zero, WinInet.INTERNET_OPTION_CONNECTED_STATE, IntPtr.Zero, ref cb);
      IntPtr buffer = IntPtr.Zero;
      try {
        buffer = Marshal.AllocHGlobal(cb);
        if (WinInet.InternetQueryOption(IntPtr.Zero, WinInet.INTERNET_OPTION_CONNECTED_STATE, buffer, ref cb)) {
          state = (uint)Marshal.ReadInt32(buffer);
        }
        else
          throw new System.ComponentModel.Win32Exception();
      }
      finally {
        if (buffer != IntPtr.Zero)
          Marshal.FreeHGlobal(buffer);
      }
      return (state == WinInet.INTERNET_STATE_CONNECTED);
    }

#if false
    const uint INTERNET_OPTION_PROXY = 38;

    enum InternetOpenType {
      Preconfig = 0,
      Direct = 1,
      Proxy = 3,
      PreconfigWithNoAutoProxy = 4
    }


    struct INTERNET_PROXY_INFO {
      public InternetOpenType dwAccessType;
      public string lpszProxy, lpszProxyBypass;
    }

    public static void test2() {
      int cb = 0;
      InternetQueryOption(IntPtr.Zero, INTERNET_OPTION_PROXY, IntPtr.Zero, ref cb);
      IntPtr buffer = IntPtr.Zero;
      try {
        buffer = Marshal.AllocHGlobal(cb);
        if (InternetQueryOption(IntPtr.Zero, INTERNET_OPTION_PROXY, buffer, ref cb)) {
          INTERNET_PROXY_INFO ipi = (INTERNET_PROXY_INFO)
            Marshal.PtrToStructure(buffer, typeof(INTERNET_PROXY_INFO));
          Console.WriteLine( ipi.dwAccessType );
          Console.WriteLine( ipi.lpszProxy );
          Console.WriteLine( ipi.lpszProxyBypass );
        }
        // else
        //  throw new Win32Exception();
      }
      finally {
        if ( buffer != IntPtr.Zero ) Marshal.FreeHGlobal( buffer );
      }
    }
#endif
  }
}
