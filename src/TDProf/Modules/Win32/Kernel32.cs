using System;
using System.Runtime.InteropServices;

namespace Win32
{
	/// <summary>
	/// Summary description for Kernel32.
	/// </summary>
	public class Kernel32
	{
    [DllImport("kernel32.dll")]
    public static extern int GlobalAddAtom(string Name);
    [DllImport("kernel32.dll")]
    public static extern int GlobalDeleteAtom(int atom);
    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalLock(IntPtr hMem);
    [DllImport("kernel32.dll")]
    public static extern bool GlobalUnlock(IntPtr hMem);
    [DllImport("kernel32.dll")]
    public static extern bool SetProcessWorkingSetSize( IntPtr proc, int min, int max );

	}
}
