namespace System.Drawing;

public sealed class SolidBrush : Brush
{
	internal bool isModifiable = true;

	private Color color;

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			if (isModifiable)
			{
				color = value;
				Status status = GDIPlus.GdipSetSolidFillColor(nativeObject, value.ToArgb());
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This SolidBrush object can't be modified."));
		}
	}

	internal SolidBrush(IntPtr ptr)
		: base(ptr)
	{
		int argb;
		Status status = GDIPlus.GdipGetSolidFillColor(ptr, out argb);
		GDIPlus.CheckStatus(status);
		color = Color.FromArgb(argb);
	}

	public SolidBrush(Color color)
	{
		this.color = color;
		Status status = GDIPlus.GdipCreateSolidFill(color.ToArgb(), out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public override object Clone()
	{
		IntPtr clonedBrush;
		Status status = GDIPlus.GdipCloneBrush(nativeObject, out clonedBrush);
		GDIPlus.CheckStatus(status);
		return new SolidBrush(clonedBrush);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && !isModifiable)
		{
			throw new ArgumentException(global::Locale.GetText("This SolidBrush object can't be modified."));
		}
		base.Dispose(disposing);
	}
}
