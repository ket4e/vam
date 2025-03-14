namespace System.Windows.Forms;

public class OSFeature : FeatureSupport
{
	private static OSFeature feature = new OSFeature();

	public static readonly object LayeredWindows;

	public static readonly object Themes;

	public static OSFeature Feature => feature;

	protected OSFeature()
	{
	}

	public static bool IsPresent(SystemParameter enumVal)
	{
		switch (enumVal)
		{
		case SystemParameter.DropShadow:
			try
			{
				object obj = SystemInformation.IsDropShadowEnabled;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		case SystemParameter.FlatMenu:
			try
			{
				object obj = SystemInformation.IsFlatMenuEnabled;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		case SystemParameter.FontSmoothingContrastMetric:
			try
			{
				object obj = SystemInformation.FontSmoothingContrast;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		case SystemParameter.FontSmoothingTypeMetric:
			try
			{
				object obj = SystemInformation.FontSmoothingType;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		case SystemParameter.MenuFadeEnabled:
			try
			{
				object obj = SystemInformation.IsMenuFadeEnabled;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		case SystemParameter.SelectionFade:
			try
			{
				object obj = SystemInformation.IsSelectionFadeEnabled;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		case SystemParameter.ToolTipAnimationMetric:
			try
			{
				object obj = SystemInformation.IsToolTipAnimationEnabled;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		case SystemParameter.UIEffects:
			try
			{
				object obj = SystemInformation.UIEffectsEnabled;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		case SystemParameter.CaretWidthMetric:
			try
			{
				object obj = SystemInformation.CaretWidth;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		case SystemParameter.VerticalFocusThicknessMetric:
			try
			{
				object obj = SystemInformation.VerticalFocusThickness;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		case SystemParameter.HorizontalFocusThicknessMetric:
			try
			{
				object obj = SystemInformation.HorizontalFocusThickness;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		default:
			return false;
		}
	}

	public override Version GetVersionPresent(object feature)
	{
		if (feature == Themes)
		{
			return ThemeEngine.Current.Version;
		}
		return null;
	}
}
