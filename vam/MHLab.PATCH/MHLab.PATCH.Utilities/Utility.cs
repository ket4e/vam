using System;
using System.Reflection;

namespace MHLab.PATCH.Utilities;

public class Utility
{
	public static string GetVersion()
	{
		return Assembly.GetExecutingAssembly().GetName().Version.ToString();
	}

	public static string FormatSizeBinary(long size, int decimals)
	{
		string[] array = new string[9] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };
		double num = size;
		int num2 = 0;
		while (num >= 1024.0 && num2 < array.Length)
		{
			num /= 1024.0;
			num2++;
		}
		return Math.Round(num, decimals) + array[num2];
	}

	public static string FormatSizeDecimal(long size, int decimals)
	{
		string[] array = new string[9] { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
		double num = size;
		int num2 = 0;
		while (num >= 1000.0 && num2 < array.Length)
		{
			num /= 1000.0;
			num2++;
		}
		return Math.Round(num, decimals) + array[num2];
	}

	public static bool IsRemoteServiceAvailable(string url)
	{
		try
		{
			return true;
		}
		catch
		{
			return false;
		}
	}
}
