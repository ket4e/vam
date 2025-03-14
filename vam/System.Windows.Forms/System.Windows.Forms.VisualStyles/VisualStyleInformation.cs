using System.Drawing;

namespace System.Windows.Forms.VisualStyles;

public static class VisualStyleInformation
{
	public static string Author
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return string.Empty;
			}
			return VisualStyles.VisualStyleInformationAuthor;
		}
	}

	public static string ColorScheme
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return string.Empty;
			}
			return VisualStyles.VisualStyleInformationColorScheme;
		}
	}

	public static string Company
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return string.Empty;
			}
			return VisualStyles.VisualStyleInformationCompany;
		}
	}

	[System.MonoTODO("Cannot get this to return the same as MS's...")]
	public static Color ControlHighlightHot
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return SystemColors.ButtonHighlight;
			}
			return VisualStyles.VisualStyleInformationControlHighlightHot;
		}
	}

	public static string Copyright
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return string.Empty;
			}
			return VisualStyles.VisualStyleInformationCopyright;
		}
	}

	public static string Description
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return string.Empty;
			}
			return VisualStyles.VisualStyleInformationDescription;
		}
	}

	public static string DisplayName
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return string.Empty;
			}
			return VisualStyles.VisualStyleInformationDisplayName;
		}
	}

	public static bool IsEnabledByUser
	{
		get
		{
			if (!IsSupportedByOS)
			{
				return false;
			}
			return VisualStyles.UxThemeIsAppThemed() && VisualStyles.UxThemeIsThemeActive();
		}
	}

	public static bool IsSupportedByOS => VisualStyles.VisualStyleInformationIsSupportedByOS;

	public static int MinimumColorDepth
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return 0;
			}
			return VisualStyles.VisualStyleInformationMinimumColorDepth;
		}
	}

	public static string Size
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return string.Empty;
			}
			return VisualStyles.VisualStyleInformationSize;
		}
	}

	public static bool SupportsFlatMenus
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return false;
			}
			return VisualStyles.VisualStyleInformationSupportsFlatMenus;
		}
	}

	[System.MonoTODO("Cannot get this to return the same as MS's...")]
	public static Color TextControlBorder
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return SystemColors.ControlDarkDark;
			}
			return VisualStyles.VisualStyleInformationTextControlBorder;
		}
	}

	public static string Url
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return string.Empty;
			}
			return VisualStyles.VisualStyleInformationUrl;
		}
	}

	public static string Version
	{
		get
		{
			if (!VisualStyleRenderer.IsSupported)
			{
				return string.Empty;
			}
			return VisualStyles.VisualStyleInformationVersion;
		}
	}

	private static IVisualStyles VisualStyles => VisualStylesEngine.Instance;
}
