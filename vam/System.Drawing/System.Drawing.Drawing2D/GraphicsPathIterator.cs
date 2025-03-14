namespace System.Drawing.Drawing2D;

public sealed class GraphicsPathIterator : MarshalByRefObject, IDisposable
{
	private IntPtr nativeObject = IntPtr.Zero;

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

	public int Count
	{
		get
		{
			if (nativeObject == IntPtr.Zero)
			{
				return 0;
			}
			int count;
			Status status = GDIPlus.GdipPathIterGetCount(nativeObject, out count);
			GDIPlus.CheckStatus(status);
			return count;
		}
	}

	public int SubpathCount
	{
		get
		{
			int count;
			Status status = GDIPlus.GdipPathIterGetSubpathCount(nativeObject, out count);
			GDIPlus.CheckStatus(status);
			return count;
		}
	}

	internal GraphicsPathIterator(IntPtr native)
	{
		nativeObject = native;
	}

	public GraphicsPathIterator(GraphicsPath path)
	{
		if (path != null)
		{
			Status status = GDIPlus.GdipCreatePathIter(out nativeObject, path.NativeObject);
			GDIPlus.CheckStatus(status);
		}
	}

	internal void Dispose(bool disposing)
	{
		if (nativeObject != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeletePathIter(nativeObject);
			GDIPlus.CheckStatus(status);
			nativeObject = IntPtr.Zero;
		}
	}

	public int CopyData(ref PointF[] points, ref byte[] types, int startIndex, int endIndex)
	{
		if (points.Length != types.Length)
		{
			throw new ArgumentException("Invalid arguments passed. Both arrays should have the same length.");
		}
		int resultCount;
		Status status = GDIPlus.GdipPathIterCopyData(nativeObject, out resultCount, points, types, startIndex, endIndex);
		GDIPlus.CheckStatus(status);
		return resultCount;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~GraphicsPathIterator()
	{
		Dispose(disposing: false);
	}

	public int Enumerate(ref PointF[] points, ref byte[] types)
	{
		int num = points.Length;
		if (num != types.Length)
		{
			throw new ArgumentException("Invalid arguments passed. Both arrays should have the same length.");
		}
		int resultCount;
		Status status = GDIPlus.GdipPathIterEnumerate(nativeObject, out resultCount, points, types, num);
		GDIPlus.CheckStatus(status);
		return resultCount;
	}

	public bool HasCurve()
	{
		bool curve;
		Status status = GDIPlus.GdipPathIterHasCurve(nativeObject, out curve);
		GDIPlus.CheckStatus(status);
		return curve;
	}

	public int NextMarker(GraphicsPath path)
	{
		IntPtr path2 = path?.NativeObject ?? IntPtr.Zero;
		int resultCount;
		Status status = GDIPlus.GdipPathIterNextMarkerPath(nativeObject, out resultCount, path2);
		GDIPlus.CheckStatus(status);
		return resultCount;
	}

	public int NextMarker(out int startIndex, out int endIndex)
	{
		int resultCount;
		Status status = GDIPlus.GdipPathIterNextMarker(nativeObject, out resultCount, out startIndex, out endIndex);
		GDIPlus.CheckStatus(status);
		return resultCount;
	}

	public int NextPathType(out byte pathType, out int startIndex, out int endIndex)
	{
		int resultCount;
		Status status = GDIPlus.GdipPathIterNextPathType(nativeObject, out resultCount, out pathType, out startIndex, out endIndex);
		GDIPlus.CheckStatus(status);
		return resultCount;
	}

	public int NextSubpath(GraphicsPath path, out bool isClosed)
	{
		IntPtr path2 = path?.NativeObject ?? IntPtr.Zero;
		int resultCount;
		Status status = GDIPlus.GdipPathIterNextSubpathPath(nativeObject, out resultCount, path2, out isClosed);
		GDIPlus.CheckStatus(status);
		return resultCount;
	}

	public int NextSubpath(out int startIndex, out int endIndex, out bool isClosed)
	{
		int resultCount;
		Status status = GDIPlus.GdipPathIterNextSubpath(nativeObject, out resultCount, out startIndex, out endIndex, out isClosed);
		GDIPlus.CheckStatus(status);
		return resultCount;
	}

	public void Rewind()
	{
		Status status = GDIPlus.GdipPathIterRewind(nativeObject);
		GDIPlus.CheckStatus(status);
	}
}
