using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ZenFulcrum.EmbeddedBrowser;

public static class Util
{
	public static bool SafeStartsWith(this string check, string starter)
	{
		if (check == null || starter == null)
		{
			return false;
		}
		if (check.Length < starter.Length)
		{
			return false;
		}
		for (int i = 0; i < starter.Length; i++)
		{
			if (check[i] != starter[i])
			{
				return false;
			}
		}
		return true;
	}

	public static string PtrToStringUTF8(IntPtr strIn)
	{
		if (strIn == IntPtr.Zero)
		{
			return null;
		}
		int i;
		for (i = 0; Marshal.ReadByte(strIn, i) != 0; i++)
		{
		}
		byte[] array = new byte[i];
		Marshal.Copy(strIn, array, 0, i);
		return Encoding.UTF8.GetString(array);
	}
}
