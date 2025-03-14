using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.CarbonInternal;

internal struct CarbonCursor
{
	private Bitmap bmp;

	private Bitmap mask;

	private Color cursor_color;

	private Color mask_color;

	private int hot_x;

	private int hot_y;

	private StdCursor id;

	private bool standard;

	public StdCursor StdCursor => id;

	public Bitmap Bitmap => bmp;

	public Bitmap Mask => mask;

	public Color CursorColor => cursor_color;

	public Color MaskColor => mask_color;

	public int HotSpotX => hot_x;

	public int HotSpotY => hot_y;

	public CarbonCursor(Bitmap bitmap, Bitmap mask, Color cursor_pixel, Color mask_pixel, int xHotSpot, int yHotSpot)
	{
		id = StdCursor.Default;
		bmp = bitmap;
		this.mask = mask;
		cursor_color = cursor_pixel;
		mask_color = mask_pixel;
		hot_x = xHotSpot;
		hot_y = yHotSpot;
		standard = true;
	}

	public CarbonCursor(StdCursor id)
	{
		this.id = id;
		bmp = null;
		mask = null;
		cursor_color = Color.Black;
		mask_color = Color.Black;
		hot_x = 0;
		hot_y = 0;
		standard = true;
	}

	public void SetCursor()
	{
		if (standard)
		{
			SetStandardCursor();
		}
		else
		{
			SetCustomCursor();
		}
	}

	public void SetCustomCursor()
	{
		throw new NotImplementedException("We dont support custom cursors yet");
	}

	public void SetStandardCursor()
	{
		switch (id)
		{
		case StdCursor.AppStarting:
			SetThemeCursor(ThemeCursor.kThemeSpinningCursor);
			break;
		case StdCursor.Arrow:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.Cross:
			SetThemeCursor(ThemeCursor.kThemeCrossCursor);
			break;
		case StdCursor.Default:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.Hand:
			SetThemeCursor(ThemeCursor.kThemeOpenHandCursor);
			break;
		case StdCursor.Help:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.HSplit:
			SetThemeCursor(ThemeCursor.kThemeResizeLeftRightCursor);
			break;
		case StdCursor.IBeam:
			SetThemeCursor(ThemeCursor.kThemeIBeamCursor);
			break;
		case StdCursor.No:
			SetThemeCursor(ThemeCursor.kThemeNotAllowedCursor);
			break;
		case StdCursor.NoMove2D:
			SetThemeCursor(ThemeCursor.kThemeNotAllowedCursor);
			break;
		case StdCursor.NoMoveHoriz:
			SetThemeCursor(ThemeCursor.kThemeNotAllowedCursor);
			break;
		case StdCursor.NoMoveVert:
			SetThemeCursor(ThemeCursor.kThemeNotAllowedCursor);
			break;
		case StdCursor.PanEast:
			SetThemeCursor(ThemeCursor.kThemeResizeRightCursor);
			break;
		case StdCursor.PanNE:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.PanNorth:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.PanNW:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.PanSE:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.PanSouth:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.PanSW:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.PanWest:
			SetThemeCursor(ThemeCursor.kThemeResizeLeftCursor);
			break;
		case StdCursor.SizeAll:
			SetThemeCursor(ThemeCursor.kThemeResizeLeftRightCursor);
			break;
		case StdCursor.SizeNESW:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.SizeNS:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.SizeNWSE:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.SizeWE:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.UpArrow:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.VSplit:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		case StdCursor.WaitCursor:
			SetThemeCursor(ThemeCursor.kThemeSpinningCursor);
			break;
		default:
			SetThemeCursor(ThemeCursor.kThemeArrowCursor);
			break;
		}
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int SetThemeCursor(ThemeCursor cursor);
}
