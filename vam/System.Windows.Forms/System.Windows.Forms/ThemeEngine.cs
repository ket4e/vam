namespace System.Windows.Forms;

internal class ThemeEngine
{
	private static Theme theme;

	public static Theme Current => theme;

	static ThemeEngine()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("MONO_THEME");
		if (environmentVariable == null)
		{
			environmentVariable = "win32";
		}
		else
		{
			environmentVariable = environmentVariable.ToLower();
		}
		if (Application.VisualStylesEnabled)
		{
			theme = new ThemeVisualStyles();
		}
		else
		{
			theme = new ThemeWin32Classic();
		}
	}
}
