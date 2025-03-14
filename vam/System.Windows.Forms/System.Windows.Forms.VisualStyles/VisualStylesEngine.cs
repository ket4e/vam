namespace System.Windows.Forms.VisualStyles;

internal class VisualStylesEngine
{
	private static IVisualStyles instance = Initialize();

	public static IVisualStyles Instance => instance;

	private static IVisualStyles Initialize()
	{
		string text = Environment.GetEnvironmentVariable("MONO_VISUAL_STYLES");
		if (text != null)
		{
			text = text.ToLower();
		}
		if (text == "gtkplus" && VisualStylesGtkPlus.Initialize())
		{
			return new VisualStylesGtkPlus();
		}
		return new VisualStylesNative();
	}
}
