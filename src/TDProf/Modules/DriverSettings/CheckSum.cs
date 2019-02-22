using System;

namespace TDProf.Util
{
	/// <summary>
	/// Compute a checksum for a bytestream.
	/// </summary>
	public class CheckSum
	{
      int m_cs = 0;

		public CheckSum()
		{
		}

      public void push_bytes(string s) {
        foreach (byte c in s) {
          m_cs += c;
        }
      }
      public int get_checksum() { return m_cs; }

	}
}
