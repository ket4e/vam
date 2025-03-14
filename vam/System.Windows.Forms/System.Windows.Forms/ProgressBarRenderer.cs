using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public sealed class ProgressBarRenderer
{
	public static bool IsSupported => VisualStyleInformation.IsEnabledByUser && (Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled || Application.VisualStyleState == VisualStyleState.ClientAreaEnabled);

	public static int ChunkSpaceThickness
	{
		get
		{
			if (!IsSupported)
			{
				throw new InvalidOperationException();
			}
			VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ProgressBar.Chunk.Normal);
			return visualStyleRenderer.GetInteger(IntegerProperty.ProgressSpaceSize);
		}
	}

	public static int ChunkThickness
	{
		get
		{
			if (!IsSupported)
			{
				throw new InvalidOperationException();
			}
			VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ProgressBar.Chunk.Normal);
			return visualStyleRenderer.GetInteger(IntegerProperty.ProgressChunkSize);
		}
	}

	private ProgressBarRenderer()
	{
	}

	public static void DrawHorizontalBar(Graphics g, Rectangle bounds)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ProgressBar.Bar.Normal);
		visualStyleRenderer.DrawBackground(g, bounds);
	}

	public static void DrawHorizontalChunks(Graphics g, Rectangle bounds)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ProgressBar.Chunk.Normal);
		visualStyleRenderer.DrawBackground(g, bounds);
	}

	public static void DrawVerticalBar(Graphics g, Rectangle bounds)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ProgressBar.BarVertical.Normal);
		visualStyleRenderer.DrawBackground(g, bounds);
	}

	public static void DrawVerticalChunks(Graphics g, Rectangle bounds)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ProgressBar.ChunkVertical.Normal);
		visualStyleRenderer.DrawBackground(g, bounds);
	}
}
