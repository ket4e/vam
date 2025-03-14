namespace System.Windows.Forms;

internal sealed class Locale
{
	public static string GetText(string msg)
	{
		return msg;
	}

	public static string GetText(string msg, params object[] args)
	{
		return string.Format(GetText(msg), args);
	}
}
