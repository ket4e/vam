namespace System.Drawing.Drawing2D;

public sealed class HatchBrush : Brush
{
	public Color BackgroundColor
	{
		get
		{
			int backColor;
			Status status = GDIPlus.GdipGetHatchBackgroundColor(nativeObject, out backColor);
			GDIPlus.CheckStatus(status);
			return Color.FromArgb(backColor);
		}
	}

	public Color ForegroundColor
	{
		get
		{
			int foreColor;
			Status status = GDIPlus.GdipGetHatchForegroundColor(nativeObject, out foreColor);
			GDIPlus.CheckStatus(status);
			return Color.FromArgb(foreColor);
		}
	}

	public HatchStyle HatchStyle
	{
		get
		{
			HatchStyle hatchstyle;
			Status status = GDIPlus.GdipGetHatchStyle(nativeObject, out hatchstyle);
			GDIPlus.CheckStatus(status);
			return hatchstyle;
		}
	}

	internal HatchBrush(IntPtr ptr)
		: base(ptr)
	{
	}

	public HatchBrush(HatchStyle hatchStyle, Color foreColor)
		: this(hatchStyle, foreColor, Color.Black)
	{
	}

	public HatchBrush(HatchStyle hatchStyle, Color foreColor, Color backColor)
	{
		Status status = GDIPlus.GdipCreateHatchBrush(hatchStyle, foreColor.ToArgb(), backColor.ToArgb(), out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public override object Clone()
	{
		IntPtr clonedBrush;
		Status status = GDIPlus.GdipCloneBrush(nativeObject, out clonedBrush);
		GDIPlus.CheckStatus(status);
		return new HatchBrush(clonedBrush);
	}
}
