using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public sealed class TrackBarRenderer
{
	public static bool IsSupported => VisualStyleInformation.IsEnabledByUser && (Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled || Application.VisualStyleState == VisualStyleState.ClientAreaEnabled);

	private TrackBarRenderer()
	{
	}

	public static void DrawBottomPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			TrackBarThumbState.Disabled => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbBottom.Disabled), 
			TrackBarThumbState.Hot => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbBottom.Hot), 
			TrackBarThumbState.Pressed => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbBottom.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbBottom.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawHorizontalThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			TrackBarThumbState.Disabled => new VisualStyleRenderer(VisualStyleElement.TrackBar.Thumb.Disabled), 
			TrackBarThumbState.Hot => new VisualStyleRenderer(VisualStyleElement.TrackBar.Thumb.Hot), 
			TrackBarThumbState.Pressed => new VisualStyleRenderer(VisualStyleElement.TrackBar.Thumb.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.TrackBar.Thumb.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawHorizontalTicks(Graphics g, Rectangle bounds, int numTicks, EdgeStyle edgeStyle)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		if (bounds.Height > 0 && bounds.Width > 0 && numTicks > 0)
		{
			VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.TrackBar.Ticks.Normal);
			double num = bounds.Left;
			double num2 = (double)(bounds.Width - 2) / (double)(numTicks - 1);
			for (int i = 0; i < numTicks; i++)
			{
				visualStyleRenderer.DrawEdge(g, new Rectangle((int)Math.Round(num), bounds.Top, 5, bounds.Height), Edges.Left, edgeStyle, EdgeEffects.None);
				num += num2;
			}
		}
	}

	public static void DrawHorizontalTrack(Graphics g, Rectangle bounds)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.TrackBar.Track.Normal);
		visualStyleRenderer.DrawBackground(g, bounds);
	}

	public static void DrawLeftPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			TrackBarThumbState.Disabled => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbLeft.Disabled), 
			TrackBarThumbState.Hot => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbLeft.Hot), 
			TrackBarThumbState.Pressed => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbLeft.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbLeft.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawRightPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			TrackBarThumbState.Disabled => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbRight.Disabled), 
			TrackBarThumbState.Hot => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbRight.Hot), 
			TrackBarThumbState.Pressed => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbRight.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbRight.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawTopPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			TrackBarThumbState.Disabled => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbTop.Disabled), 
			TrackBarThumbState.Hot => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbTop.Hot), 
			TrackBarThumbState.Pressed => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbTop.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbTop.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawVerticalThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			TrackBarThumbState.Disabled => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbVertical.Disabled), 
			TrackBarThumbState.Hot => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbVertical.Hot), 
			TrackBarThumbState.Pressed => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbVertical.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbVertical.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawVerticalTicks(Graphics g, Rectangle bounds, int numTicks, EdgeStyle edgeStyle)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		if (bounds.Height > 0 && bounds.Width > 0 && numTicks > 0)
		{
			VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.TrackBar.TicksVertical.Normal);
			double num = bounds.Top;
			double num2 = (double)(bounds.Height - 2) / (double)(numTicks - 1);
			for (int i = 0; i < numTicks; i++)
			{
				visualStyleRenderer.DrawEdge(g, new Rectangle(bounds.Left, (int)Math.Round(num), bounds.Width, 5), Edges.Top, edgeStyle, EdgeEffects.None);
				num += num2;
			}
		}
	}

	public static void DrawVerticalTrack(Graphics g, Rectangle bounds)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.TrackBar.Track.Normal);
		visualStyleRenderer.DrawBackground(g, bounds);
	}

	public static Size GetBottomPointingThumbSize(Graphics g, TrackBarThumbState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		return (state switch
		{
			TrackBarThumbState.Disabled => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbBottom.Disabled), 
			TrackBarThumbState.Hot => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbBottom.Hot), 
			TrackBarThumbState.Pressed => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbBottom.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbBottom.Normal), 
		}).GetPartSize(g, ThemeSizeType.Draw);
	}

	public static Size GetLeftPointingThumbSize(Graphics g, TrackBarThumbState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		return (state switch
		{
			TrackBarThumbState.Disabled => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbLeft.Disabled), 
			TrackBarThumbState.Hot => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbLeft.Hot), 
			TrackBarThumbState.Pressed => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbLeft.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbLeft.Normal), 
		}).GetPartSize(g, ThemeSizeType.Draw);
	}

	public static Size GetRightPointingThumbSize(Graphics g, TrackBarThumbState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		return (state switch
		{
			TrackBarThumbState.Disabled => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbRight.Disabled), 
			TrackBarThumbState.Hot => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbRight.Hot), 
			TrackBarThumbState.Pressed => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbRight.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbRight.Normal), 
		}).GetPartSize(g, ThemeSizeType.Draw);
	}

	public static Size GetTopPointingThumbSize(Graphics g, TrackBarThumbState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		return (state switch
		{
			TrackBarThumbState.Disabled => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbTop.Disabled), 
			TrackBarThumbState.Hot => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbTop.Hot), 
			TrackBarThumbState.Pressed => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbTop.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.TrackBar.ThumbTop.Normal), 
		}).GetPartSize(g, ThemeSizeType.Draw);
	}
}
