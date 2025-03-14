namespace System.Drawing.Drawing2D;

public class CustomLineCap : MarshalByRefObject, IDisposable, ICloneable
{
	private bool disposed;

	internal IntPtr nativeObject;

	public LineCap BaseCap
	{
		get
		{
			LineCap baseCap;
			Status status = GDIPlus.GdipGetCustomLineCapBaseCap(nativeObject, out baseCap);
			GDIPlus.CheckStatus(status);
			return baseCap;
		}
		set
		{
			Status status = GDIPlus.GdipSetCustomLineCapBaseCap(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public LineJoin StrokeJoin
	{
		get
		{
			LineJoin lineJoin;
			Status status = GDIPlus.GdipGetCustomLineCapStrokeJoin(nativeObject, out lineJoin);
			GDIPlus.CheckStatus(status);
			return lineJoin;
		}
		set
		{
			Status status = GDIPlus.GdipSetCustomLineCapStrokeJoin(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public float BaseInset
	{
		get
		{
			float inset;
			Status status = GDIPlus.GdipGetCustomLineCapBaseInset(nativeObject, out inset);
			GDIPlus.CheckStatus(status);
			return inset;
		}
		set
		{
			Status status = GDIPlus.GdipSetCustomLineCapBaseInset(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public float WidthScale
	{
		get
		{
			float widthScale;
			Status status = GDIPlus.GdipGetCustomLineCapWidthScale(nativeObject, out widthScale);
			GDIPlus.CheckStatus(status);
			return widthScale;
		}
		set
		{
			Status status = GDIPlus.GdipSetCustomLineCapWidthScale(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	internal CustomLineCap()
	{
	}

	internal CustomLineCap(IntPtr ptr)
	{
		nativeObject = ptr;
	}

	public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath)
		: this(fillPath, strokePath, LineCap.Flat, 0f)
	{
	}

	public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap)
		: this(fillPath, strokePath, baseCap, 0f)
	{
	}

	public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap, float baseInset)
	{
		IntPtr fillPath2 = IntPtr.Zero;
		IntPtr strokePath2 = IntPtr.Zero;
		if (fillPath != null)
		{
			fillPath2 = fillPath.nativePath;
		}
		if (strokePath != null)
		{
			strokePath2 = strokePath.nativePath;
		}
		Status status = GDIPlus.GdipCreateCustomLineCap(fillPath2, strokePath2, baseCap, baseInset, out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public object Clone()
	{
		IntPtr clonedCap;
		Status status = GDIPlus.GdipCloneCustomLineCap(nativeObject, out clonedCap);
		GDIPlus.CheckStatus(status);
		return new CustomLineCap(clonedCap);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			Status status = GDIPlus.GdipDeleteCustomLineCap(nativeObject);
			GDIPlus.CheckStatus(status);
			disposed = true;
			nativeObject = IntPtr.Zero;
		}
	}

	~CustomLineCap()
	{
		Dispose(disposing: false);
	}

	public void GetStrokeCaps(out LineCap startCap, out LineCap endCap)
	{
		Status status = GDIPlus.GdipGetCustomLineCapStrokeCaps(nativeObject, out startCap, out endCap);
		GDIPlus.CheckStatus(status);
	}

	public void SetStrokeCaps(LineCap startCap, LineCap endCap)
	{
		Status status = GDIPlus.GdipSetCustomLineCapStrokeCaps(nativeObject, startCap, endCap);
		GDIPlus.CheckStatus(status);
	}
}
