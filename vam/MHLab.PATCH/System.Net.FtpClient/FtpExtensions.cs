using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Net.FtpClient;

public static class FtpExtensions
{
	public static string GetFtpPath(this string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return "./";
		}
		path = Regex.Replace(path.Replace('\\', '/'), "[/]+", "/").TrimEnd('/');
		if (path.Length == 0)
		{
			path = "/";
		}
		return path;
	}

	public static string GetFtpPath(this string path, params string[] segments)
	{
		if (string.IsNullOrEmpty(path))
		{
			path = "./";
		}
		foreach (string text in segments)
		{
			if (text != null)
			{
				if (path.Length > 0 && !path.EndsWith("/"))
				{
					path += "/";
				}
				path += Regex.Replace(text.Replace('\\', '/'), "[/]+", "/").TrimEnd('/');
			}
		}
		path = Regex.Replace(path.Replace('\\', '/'), "[/]+", "/").TrimEnd('/');
		if (path.Length == 0)
		{
			path = "/";
		}
		return path;
	}

	public static string GetFtpDirectoryName(this string path)
	{
		string text = ((path == null) ? "" : path.GetFtpPath());
		int num = -1;
		if (text.Length == 0 || text == "/")
		{
			return "/";
		}
		num = text.LastIndexOf('/');
		if (num < 0)
		{
			return ".";
		}
		return text.Substring(0, num);
	}

	public static string GetFtpFileName(this string path)
	{
		string text = ((path == null) ? null : path);
		int num = -1;
		if (text == null)
		{
			return null;
		}
		num = text.LastIndexOf('/');
		if (num < 0)
		{
			return text;
		}
		num++;
		if (num >= text.Length)
		{
			return text;
		}
		return text.Substring(num, text.Length - num);
	}

	public static DateTime GetFtpDate(this string date, DateTimeStyles style)
	{
		string[] formats = new string[6] { "yyyyMMddHHmmss", "yyyyMMddHHmmss.fff", "MMM dd  yyyy", "MMM  d  yyyy", "MMM dd HH:mm", "MMM  d HH:mm" };
		if (DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, style, out var result))
		{
			return result;
		}
		return DateTime.MinValue;
	}
}
