using System;
using System.Runtime.InteropServices;

namespace Win32
{
	/// <summary>
	/// Summary description for WinInet.
	/// </summary>
	public class WinInet
	{
    [DllImport("wininet.dll", CharSet=CharSet.Auto, SetLastError=true)]
    public static extern bool InternetQueryOption(IntPtr hInternet, uint dwOption, IntPtr lpBuffer, ref int lpdwBufferLength);

    public const uint INTERNET_OPTION_CONNECTED_STATE = 50;
    public const uint INTERNET_STATE_CONNECTED = 1;

	}
}
