namespace System.Drawing;

public abstract class Brush : MarshalByRefObject, IDisposable, ICloneable
{
	internal IntPtr nativeObject;

	internal IntPtr NativeObject
	{
		get
		{
			return nativeObject;
		}
		set
		{
			nativeObject = value;
		}
	}

	internal Brush(IntPtr ptr)
	{
		nativeObject = ptr;
	}

	protected Brush()
	{
	}

	public abstract object Clone();

	protected internal void SetNativeBrush(IntPtr brush)
	{
		nativeObject = brush;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (nativeObject != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeleteBrush(nativeObject);
			nativeObject = IntPtr.Zero;
			GDIPlus.CheckStatus(status);
		}
	}

	~Brush()
	{
		Dispose(disposing: false);
	}
}
