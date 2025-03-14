using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public sealed class ScrollBarRenderer
{
	public static bool IsSupported => VisualStyleInformation.IsEnabledByUser && (Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled || Application.VisualStyleState == VisualStyleState.ClientAreaEnabled);

	private ScrollBarRenderer()
	{
	}

	public static void DrawArrowButton(Graphics g, Rectangle bounds, ScrollBarArrowButtonState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			ScrollBarArrowButtonState.DownDisabled => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.DownDisabled), 
			ScrollBarArrowButtonState.DownHot => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.DownHot), 
			ScrollBarArrowButtonState.DownPressed => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.DownPressed), 
			ScrollBarArrowButtonState.LeftDisabled => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.LeftDisabled), 
			ScrollBarArrowButtonState.LeftHot => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.LeftHot), 
			ScrollBarArrowButtonState.LeftNormal => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.LeftNormal), 
			ScrollBarArrowButtonState.LeftPressed => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.LeftPressed), 
			ScrollBarArrowButtonState.RightDisabled => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.RightDisabled), 
			ScrollBarArrowButtonState.RightHot => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.RightHot), 
			ScrollBarArrowButtonState.RightNormal => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.RightNormal), 
			ScrollBarArrowButtonState.RightPressed => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.RightPressed), 
			ScrollBarArrowButtonState.UpDisabled => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.UpDisabled), 
			ScrollBarArrowButtonState.UpHot => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.UpHot), 
			ScrollBarArrowButtonState.UpNormal => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.UpNormal), 
			ScrollBarArrowButtonState.UpPressed => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.UpPressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ArrowButton.DownNormal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawHorizontalThumb(Graphics g, Rectangle bounds, ScrollBarState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			ScrollBarState.Disabled => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Disabled), 
			ScrollBarState.Hot => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Hot), 
			ScrollBarState.Pressed => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawHorizontalThumbGrip(Graphics g, Rectangle bounds, ScrollBarState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ScrollBar.GripperHorizontal.Normal);
		visualStyleRenderer.DrawBackground(g, bounds);
	}

	public static void DrawLeftHorizontalTrack(Graphics g, Rectangle bounds, ScrollBarState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			ScrollBarState.Disabled => new VisualStyleRenderer(VisualStyleElement.ScrollBar.LeftTrackHorizontal.Disabled), 
			ScrollBarState.Hot => new VisualStyleRenderer(VisualStyleElement.ScrollBar.LeftTrackHorizontal.Hot), 
			ScrollBarState.Pressed => new VisualStyleRenderer(VisualStyleElement.ScrollBar.LeftTrackHorizontal.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.ScrollBar.LeftTrackHorizontal.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawLowerVerticalTrack(Graphics g, Rectangle bounds, ScrollBarState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			ScrollBarState.Disabled => new VisualStyleRenderer(VisualStyleElement.ScrollBar.LowerTrackVertical.Disabled), 
			ScrollBarState.Hot => new VisualStyleRenderer(VisualStyleElement.ScrollBar.LowerTrackVertical.Hot), 
			ScrollBarState.Pressed => new VisualStyleRenderer(VisualStyleElement.ScrollBar.LowerTrackVertical.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.ScrollBar.LowerTrackVertical.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawRightHorizontalTrack(Graphics g, Rectangle bounds, ScrollBarState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			ScrollBarState.Disabled => new VisualStyleRenderer(VisualStyleElement.ScrollBar.RightTrackHorizontal.Disabled), 
			ScrollBarState.Hot => new VisualStyleRenderer(VisualStyleElement.ScrollBar.RightTrackHorizontal.Hot), 
			ScrollBarState.Pressed => new VisualStyleRenderer(VisualStyleElement.ScrollBar.RightTrackHorizontal.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.ScrollBar.RightTrackHorizontal.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawSizeBox(Graphics g, Rectangle bounds, ScrollBarSizeBoxState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			ScrollBarSizeBoxState.RightAlign => new VisualStyleRenderer(VisualStyleElement.ScrollBar.SizeBox.RightAlign), 
			_ => new VisualStyleRenderer(VisualStyleElement.ScrollBar.SizeBox.LeftAlign), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawUpperVerticalTrack(Graphics g, Rectangle bounds, ScrollBarState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			ScrollBarState.Disabled => new VisualStyleRenderer(VisualStyleElement.ScrollBar.UpperTrackVertical.Disabled), 
			ScrollBarState.Hot => new VisualStyleRenderer(VisualStyleElement.ScrollBar.UpperTrackVertical.Hot), 
			ScrollBarState.Pressed => new VisualStyleRenderer(VisualStyleElement.ScrollBar.UpperTrackVertical.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.ScrollBar.UpperTrackVertical.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawVerticalThumb(Graphics g, Rectangle bounds, ScrollBarState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			ScrollBarState.Disabled => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ThumbButtonVertical.Disabled), 
			ScrollBarState.Hot => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ThumbButtonVertical.Hot), 
			ScrollBarState.Pressed => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ThumbButtonVertical.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.ScrollBar.ThumbButtonVertical.Normal), 
		}).DrawBackground(g, bounds);
	}

	public static void DrawVerticalThumbGrip(Graphics g, Rectangle bounds, ScrollBarState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ScrollBar.GripperVertical.Normal);
		visualStyleRenderer.DrawBackground(g, bounds);
	}

	public static Size GetSizeBoxSize(Graphics g, ScrollBarState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ScrollBar.SizeBox.LeftAlign);
		return visualStyleRenderer.GetPartSize(g, ThemeSizeType.Draw);
	}

	public static Size GetThumbGripSize(Graphics g, ScrollBarState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ScrollBar.GripperVertical.Normal);
		return visualStyleRenderer.GetPartSize(g, ThemeSizeType.Draw);
	}
}
