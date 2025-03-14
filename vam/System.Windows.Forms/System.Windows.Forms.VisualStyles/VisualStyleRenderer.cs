using System.ComponentModel;
using System.Drawing;
using System.Security;

namespace System.Windows.Forms.VisualStyles;

public sealed class VisualStyleRenderer
{
	private class ThemeHandleManager
	{
		public VisualStyleRenderer VisualStyleRenderer;

		~ThemeHandleManager()
		{
			if (!(VisualStyleRenderer.theme == IntPtr.Zero))
			{
				VisualStyles.UxThemeCloseThemeData(VisualStyleRenderer.theme);
			}
		}
	}

	private string class_name;

	private int part;

	private int state;

	private IntPtr theme;

	private int last_hresult;

	private ThemeHandleManager theme_handle_manager = new ThemeHandleManager();

	public string Class => class_name;

	public IntPtr Handle => theme;

	public int LastHResult => last_hresult;

	public int Part => part;

	public int State => state;

	public static bool IsSupported
	{
		get
		{
			if (!VisualStyleInformation.IsEnabledByUser)
			{
				return false;
			}
			if (Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled || Application.VisualStyleState == VisualStyleState.ClientAreaEnabled)
			{
				return true;
			}
			return false;
		}
	}

	internal static IVisualStyles VisualStyles => VisualStylesEngine.Instance;

	public VisualStyleRenderer(string className, int part, int state)
	{
		theme_handle_manager.VisualStyleRenderer = this;
		SetParameters(className, part, state);
	}

	public VisualStyleRenderer(VisualStyleElement element)
		: this(element.ClassName, element.Part, element.State)
	{
	}

	public static bool IsElementDefined(VisualStyleElement element)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException("Visual Styles are not enabled.");
		}
		if (IsElementKnownToBeSupported(element.ClassName, element.Part, element.State))
		{
			return true;
		}
		IntPtr intPtr = VisualStyles.UxThemeOpenThemeData(IntPtr.Zero, element.ClassName);
		if (intPtr == IntPtr.Zero)
		{
			return false;
		}
		bool result = VisualStyles.UxThemeIsThemePartDefined(intPtr, element.Part);
		VisualStyles.UxThemeCloseThemeData(intPtr);
		return result;
	}

	public void DrawBackground(IDeviceContext dc, Rectangle bounds)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeDrawThemeBackground(theme, dc, part, state, bounds);
	}

	public void DrawBackground(IDeviceContext dc, Rectangle bounds, Rectangle clipRectangle)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeDrawThemeBackground(theme, dc, part, state, bounds, clipRectangle);
	}

	public Rectangle DrawEdge(IDeviceContext dc, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeDrawThemeEdge(theme, dc, part, state, bounds, edges, style, effects, out var result);
		return result;
	}

	public void DrawImage(Graphics g, Rectangle bounds, ImageList imageList, int imageIndex)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		if (imageIndex < 0 || imageIndex > imageList.Images.Count - 1)
		{
			throw new ArgumentOutOfRangeException("imageIndex");
		}
		if (imageList.Images[imageIndex] == null)
		{
			throw new ArgumentNullException("imageIndex");
		}
		g.DrawImage(imageList.Images[imageIndex], bounds);
	}

	public void DrawImage(Graphics g, Rectangle bounds, Image image)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		g.DrawImage(image, bounds);
	}

	public void DrawParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeDrawThemeParentBackground(dc, bounds, childControl);
	}

	public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw, bool drawDisabled, TextFormatFlags flags)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeDrawThemeText(theme, dc, part, state, textToDraw, flags, bounds);
	}

	public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw, bool drawDisabled)
	{
		DrawText(dc, bounds, textToDraw, drawDisabled, TextFormatFlags.Left);
	}

	public void DrawText(IDeviceContext dc, Rectangle bounds, string textToDraw)
	{
		DrawText(dc, bounds, textToDraw, drawDisabled: false, TextFormatFlags.Left);
	}

	public Rectangle GetBackgroundContentRectangle(IDeviceContext dc, Rectangle bounds)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeGetThemeBackgroundContentRect(theme, dc, part, state, bounds, out var result);
		return result;
	}

	public Rectangle GetBackgroundExtent(IDeviceContext dc, Rectangle contentBounds)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeGetThemeBackgroundExtent(theme, dc, part, state, contentBounds, out var result);
		return result;
	}

	[SuppressUnmanagedCodeSecurity]
	public Region GetBackgroundRegion(IDeviceContext dc, Rectangle bounds)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeGetThemeBackgroundRegion(theme, dc, part, state, bounds, out var result);
		return result;
	}

	public bool GetBoolean(BooleanProperty prop)
	{
		if (!Enum.IsDefined(typeof(BooleanProperty), prop))
		{
			throw new InvalidEnumArgumentException("prop", (int)prop, typeof(BooleanProperty));
		}
		last_hresult = VisualStyles.UxThemeGetThemeBool(theme, part, state, prop, out var result);
		return result;
	}

	public Color GetColor(ColorProperty prop)
	{
		if (!Enum.IsDefined(typeof(ColorProperty), prop))
		{
			throw new InvalidEnumArgumentException("prop", (int)prop, typeof(ColorProperty));
		}
		last_hresult = VisualStyles.UxThemeGetThemeColor(theme, part, state, prop, out var result);
		return result;
	}

	public int GetEnumValue(EnumProperty prop)
	{
		if (!Enum.IsDefined(typeof(EnumProperty), prop))
		{
			throw new InvalidEnumArgumentException("prop", (int)prop, typeof(EnumProperty));
		}
		last_hresult = VisualStyles.UxThemeGetThemeEnumValue(theme, part, state, prop, out var result);
		return result;
	}

	public string GetFilename(FilenameProperty prop)
	{
		if (!Enum.IsDefined(typeof(FilenameProperty), prop))
		{
			throw new InvalidEnumArgumentException("prop", (int)prop, typeof(FilenameProperty));
		}
		last_hresult = VisualStyles.UxThemeGetThemeFilename(theme, part, state, prop, out var result);
		return result;
	}

	[System.MonoTODO("I can't get MS's to return anything but null, so I can't really get this one right")]
	public Font GetFont(IDeviceContext dc, FontProperty prop)
	{
		throw new NotImplementedException();
	}

	public int GetInteger(IntegerProperty prop)
	{
		if (!Enum.IsDefined(typeof(IntegerProperty), prop))
		{
			throw new InvalidEnumArgumentException("prop", (int)prop, typeof(IntegerProperty));
		}
		last_hresult = VisualStyles.UxThemeGetThemeInt(theme, part, state, prop, out var result);
		return result;
	}

	[System.MonoTODO("MS's causes a PInvokeStackUnbalance on me, so this is not verified against MS.")]
	public Padding GetMargins(IDeviceContext dc, MarginProperty prop)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		if (!Enum.IsDefined(typeof(MarginProperty), prop))
		{
			throw new InvalidEnumArgumentException("prop", (int)prop, typeof(MarginProperty));
		}
		last_hresult = VisualStyles.UxThemeGetThemeMargins(theme, dc, part, state, prop, out var result);
		return result;
	}

	public Size GetPartSize(IDeviceContext dc, Rectangle bounds, ThemeSizeType type)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		if (!Enum.IsDefined(typeof(ThemeSizeType), type))
		{
			throw new InvalidEnumArgumentException("prop", (int)type, typeof(ThemeSizeType));
		}
		last_hresult = VisualStyles.UxThemeGetThemePartSize(theme, dc, part, state, bounds, type, out var result);
		return result;
	}

	public Size GetPartSize(IDeviceContext dc, ThemeSizeType type)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		if (!Enum.IsDefined(typeof(ThemeSizeType), type))
		{
			throw new InvalidEnumArgumentException("prop", (int)type, typeof(ThemeSizeType));
		}
		last_hresult = VisualStyles.UxThemeGetThemePartSize(theme, dc, part, state, type, out var result);
		return result;
	}

	public Point GetPoint(PointProperty prop)
	{
		if (!Enum.IsDefined(typeof(PointProperty), prop))
		{
			throw new InvalidEnumArgumentException("prop", (int)prop, typeof(PointProperty));
		}
		last_hresult = VisualStyles.UxThemeGetThemePosition(theme, part, state, prop, out var result);
		return result;
	}

	[System.MonoTODO("Can't find any values that return anything on MS to test against")]
	public string GetString(StringProperty prop)
	{
		if (!Enum.IsDefined(typeof(StringProperty), prop))
		{
			throw new InvalidEnumArgumentException("prop", (int)prop, typeof(StringProperty));
		}
		last_hresult = VisualStyles.UxThemeGetThemeString(theme, part, state, prop, out var result);
		return result;
	}

	public Rectangle GetTextExtent(IDeviceContext dc, Rectangle bounds, string textToDraw, TextFormatFlags flags)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeGetThemeTextExtent(theme, dc, part, state, textToDraw, flags, bounds, out var result);
		return result;
	}

	public Rectangle GetTextExtent(IDeviceContext dc, string textToDraw, TextFormatFlags flags)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeGetThemeTextExtent(theme, dc, part, state, textToDraw, flags, out var result);
		return result;
	}

	public TextMetrics GetTextMetrics(IDeviceContext dc)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc", "dc cannot be null.");
		}
		last_hresult = VisualStyles.UxThemeGetThemeTextMetrics(theme, dc, part, state, out var result);
		return result;
	}

	public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, IntPtr hRgn, Point pt, HitTestOptions options)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		last_hresult = VisualStyles.UxThemeHitTestThemeBackground(theme, dc, part, state, options, backgroundRectangle, hRgn, pt, out var result);
		return result;
	}

	public HitTestCode HitTestBackground(Graphics g, Rectangle backgroundRectangle, Region region, Point pt, HitTestOptions options)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		IntPtr hrgn = region.GetHrgn(g);
		return HitTestBackground(g, backgroundRectangle, hrgn, pt, options);
	}

	public HitTestCode HitTestBackground(IDeviceContext dc, Rectangle backgroundRectangle, Point pt, HitTestOptions options)
	{
		return HitTestBackground(dc, backgroundRectangle, IntPtr.Zero, pt, options);
	}

	public bool IsBackgroundPartiallyTransparent()
	{
		return VisualStyles.UxThemeIsThemeBackgroundPartiallyTransparent(theme, part, state);
	}

	public void SetParameters(string className, int part, int state)
	{
		if (theme != IntPtr.Zero)
		{
			last_hresult = VisualStyles.UxThemeCloseThemeData(theme);
		}
		if (!IsSupported)
		{
			throw new InvalidOperationException("Visual Styles are not enabled.");
		}
		class_name = className;
		this.part = part;
		this.state = state;
		theme = VisualStyles.UxThemeOpenThemeData(IntPtr.Zero, class_name);
		if (IsElementKnownToBeSupported(className, part, state) || (!(theme == IntPtr.Zero) && VisualStyles.UxThemeIsThemePartDefined(theme, this.part)))
		{
			return;
		}
		throw new ArgumentException("This element is not supported by the current visual style.");
	}

	public void SetParameters(VisualStyleElement element)
	{
		SetParameters(element.ClassName, element.Part, element.State);
	}

	internal void DrawBackgroundExcludingArea(IDeviceContext dc, Rectangle bounds, Rectangle excludedArea)
	{
		VisualStyles.VisualStyleRendererDrawBackgroundExcludingArea(theme, dc, part, state, bounds, excludedArea);
	}

	private static bool IsElementKnownToBeSupported(string className, int part, int state)
	{
		return className == "STATUS" && part == 0 && state == 0;
	}
}
