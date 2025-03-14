using System.IO;

namespace System.ComponentModel;

public static class SyntaxCheck
{
	public static bool CheckMachineName(string value)
	{
		if (value == null || value.Trim().Length == 0)
		{
			return false;
		}
		return value.IndexOf('\\') == -1;
	}

	public static bool CheckPath(string value)
	{
		if (value == null || value.Trim().Length == 0)
		{
			return false;
		}
		return value.StartsWith("\\\\");
	}

	public static bool CheckRootedPath(string value)
	{
		if (value == null || value.Trim().Length == 0)
		{
			return false;
		}
		return Path.IsPathRooted(value);
	}
}
