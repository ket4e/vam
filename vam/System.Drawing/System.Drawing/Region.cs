using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Drawing;

public sealed class Region : MarshalByRefObject, IDisposable
{
	private IntPtr nativeRegion = IntPtr.Zero;

	internal IntPtr NativeObject
	{
		get
		{
			return nativeRegion;
		}
		set
		{
			nativeRegion = value;
		}
	}

	public Region()
	{
		Status status = GDIPlus.GdipCreateRegion(out nativeRegion);
		GDIPlus.CheckStatus(status);
	}

	internal Region(IntPtr native)
	{
		nativeRegion = native;
	}

	public Region(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		Status status = GDIPlus.GdipCreateRegionPath(path.NativeObject, out nativeRegion);
		GDIPlus.CheckStatus(status);
	}

	public Region(Rectangle rect)
	{
		Status status = GDIPlus.GdipCreateRegionRectI(ref rect, out nativeRegion);
		GDIPlus.CheckStatus(status);
	}

	public Region(RectangleF rect)
	{
		Status status = GDIPlus.GdipCreateRegionRect(ref rect, out nativeRegion);
		GDIPlus.CheckStatus(status);
	}

	public Region(RegionData rgnData)
	{
		if (rgnData == null)
		{
			throw new ArgumentNullException("rgnData");
		}
		if (rgnData.Data.Length == 0)
		{
			throw new ArgumentException("rgnData");
		}
		Status status = GDIPlus.GdipCreateRegionRgnData(rgnData.Data, rgnData.Data.Length, out nativeRegion);
		GDIPlus.CheckStatus(status);
	}

	public void Union(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		Status status = GDIPlus.GdipCombineRegionPath(nativeRegion, path.NativeObject, CombineMode.Union);
		GDIPlus.CheckStatus(status);
	}

	public void Union(Rectangle rect)
	{
		Status status = GDIPlus.GdipCombineRegionRectI(nativeRegion, ref rect, CombineMode.Union);
		GDIPlus.CheckStatus(status);
	}

	public void Union(RectangleF rect)
	{
		Status status = GDIPlus.GdipCombineRegionRect(nativeRegion, ref rect, CombineMode.Union);
		GDIPlus.CheckStatus(status);
	}

	public void Union(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		Status status = GDIPlus.GdipCombineRegionRegion(nativeRegion, region.NativeObject, CombineMode.Union);
		GDIPlus.CheckStatus(status);
	}

	public void Intersect(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		Status status = GDIPlus.GdipCombineRegionPath(nativeRegion, path.NativeObject, CombineMode.Intersect);
		GDIPlus.CheckStatus(status);
	}

	public void Intersect(Rectangle rect)
	{
		Status status = GDIPlus.GdipCombineRegionRectI(nativeRegion, ref rect, CombineMode.Intersect);
		GDIPlus.CheckStatus(status);
	}

	public void Intersect(RectangleF rect)
	{
		Status status = GDIPlus.GdipCombineRegionRect(nativeRegion, ref rect, CombineMode.Intersect);
		GDIPlus.CheckStatus(status);
	}

	public void Intersect(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		Status status = GDIPlus.GdipCombineRegionRegion(nativeRegion, region.NativeObject, CombineMode.Intersect);
		GDIPlus.CheckStatus(status);
	}

	public void Complement(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		Status status = GDIPlus.GdipCombineRegionPath(nativeRegion, path.NativeObject, CombineMode.Complement);
		GDIPlus.CheckStatus(status);
	}

	public void Complement(Rectangle rect)
	{
		Status status = GDIPlus.GdipCombineRegionRectI(nativeRegion, ref rect, CombineMode.Complement);
		GDIPlus.CheckStatus(status);
	}

	public void Complement(RectangleF rect)
	{
		Status status = GDIPlus.GdipCombineRegionRect(nativeRegion, ref rect, CombineMode.Complement);
		GDIPlus.CheckStatus(status);
	}

	public void Complement(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		Status status = GDIPlus.GdipCombineRegionRegion(nativeRegion, region.NativeObject, CombineMode.Complement);
		GDIPlus.CheckStatus(status);
	}

	public void Exclude(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		Status status = GDIPlus.GdipCombineRegionPath(nativeRegion, path.NativeObject, CombineMode.Exclude);
		GDIPlus.CheckStatus(status);
	}

	public void Exclude(Rectangle rect)
	{
		Status status = GDIPlus.GdipCombineRegionRectI(nativeRegion, ref rect, CombineMode.Exclude);
		GDIPlus.CheckStatus(status);
	}

	public void Exclude(RectangleF rect)
	{
		Status status = GDIPlus.GdipCombineRegionRect(nativeRegion, ref rect, CombineMode.Exclude);
		GDIPlus.CheckStatus(status);
	}

	public void Exclude(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		Status status = GDIPlus.GdipCombineRegionRegion(nativeRegion, region.NativeObject, CombineMode.Exclude);
		GDIPlus.CheckStatus(status);
	}

	public void Xor(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		Status status = GDIPlus.GdipCombineRegionPath(nativeRegion, path.NativeObject, CombineMode.Xor);
		GDIPlus.CheckStatus(status);
	}

	public void Xor(Rectangle rect)
	{
		Status status = GDIPlus.GdipCombineRegionRectI(nativeRegion, ref rect, CombineMode.Xor);
		GDIPlus.CheckStatus(status);
	}

	public void Xor(RectangleF rect)
	{
		Status status = GDIPlus.GdipCombineRegionRect(nativeRegion, ref rect, CombineMode.Xor);
		GDIPlus.CheckStatus(status);
	}

	public void Xor(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		Status status = GDIPlus.GdipCombineRegionRegion(nativeRegion, region.NativeObject, CombineMode.Xor);
		GDIPlus.CheckStatus(status);
	}

	public RectangleF GetBounds(Graphics g)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		RectangleF rect = default(Rectangle);
		Status status = GDIPlus.GdipGetRegionBounds(nativeRegion, g.NativeObject, ref rect);
		GDIPlus.CheckStatus(status);
		return rect;
	}

	public void Translate(int dx, int dy)
	{
		Status status = GDIPlus.GdipTranslateRegionI(nativeRegion, dx, dy);
		GDIPlus.CheckStatus(status);
	}

	public void Translate(float dx, float dy)
	{
		Status status = GDIPlus.GdipTranslateRegion(nativeRegion, dx, dy);
		GDIPlus.CheckStatus(status);
	}

	public bool IsVisible(int x, int y, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionPointI(nativeRegion, x, y, graphics, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(int x, int y, int width, int height)
	{
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionRectI(nativeRegion, x, y, width, height, IntPtr.Zero, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(int x, int y, int width, int height, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionRectI(nativeRegion, x, y, width, height, graphics, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(Point point)
	{
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionPointI(nativeRegion, point.X, point.Y, IntPtr.Zero, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(PointF point)
	{
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionPoint(nativeRegion, point.X, point.Y, IntPtr.Zero, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(Point point, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionPointI(nativeRegion, point.X, point.Y, graphics, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(PointF point, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionPoint(nativeRegion, point.X, point.Y, graphics, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(Rectangle rect)
	{
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionRectI(nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, IntPtr.Zero, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(RectangleF rect)
	{
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionRect(nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, IntPtr.Zero, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(Rectangle rect, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionRectI(nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, graphics, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(RectangleF rect, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionRect(nativeRegion, rect.X, rect.Y, rect.Width, rect.Height, graphics, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(float x, float y)
	{
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionPoint(nativeRegion, x, y, IntPtr.Zero, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(float x, float y, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionPoint(nativeRegion, x, y, graphics, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(float x, float y, float width, float height)
	{
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionRect(nativeRegion, x, y, width, height, IntPtr.Zero, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(float x, float y, float width, float height, Graphics g)
	{
		IntPtr graphics = g?.NativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsVisibleRegionRect(nativeRegion, x, y, width, height, graphics, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsEmpty(Graphics g)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		bool result;
		Status status = GDIPlus.GdipIsEmptyRegion(nativeRegion, g.NativeObject, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsInfinite(Graphics g)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		bool result;
		Status status = GDIPlus.GdipIsInfiniteRegion(nativeRegion, g.NativeObject, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public void MakeEmpty()
	{
		Status status = GDIPlus.GdipSetEmpty(nativeRegion);
		GDIPlus.CheckStatus(status);
	}

	public void MakeInfinite()
	{
		Status status = GDIPlus.GdipSetInfinite(nativeRegion);
		GDIPlus.CheckStatus(status);
	}

	public bool Equals(Region region, Graphics g)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		bool result;
		Status status = GDIPlus.GdipIsEqualRegion(nativeRegion, region.NativeObject, g.NativeObject, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static Region FromHrgn(IntPtr hrgn)
	{
		if (hrgn == IntPtr.Zero)
		{
			throw new ArgumentException("hrgn");
		}
		IntPtr region;
		Status status = GDIPlus.GdipCreateRegionHrgn(hrgn, out region);
		GDIPlus.CheckStatus(status);
		return new Region(region);
	}

	public IntPtr GetHrgn(Graphics g)
	{
		if (g == null)
		{
			return nativeRegion;
		}
		IntPtr hRgn = IntPtr.Zero;
		Status status = GDIPlus.GdipGetRegionHRgn(nativeRegion, g.NativeObject, ref hRgn);
		GDIPlus.CheckStatus(status);
		return hRgn;
	}

	public RegionData GetRegionData()
	{
		Status status = GDIPlus.GdipGetRegionDataSize(nativeRegion, out var bufferSize);
		GDIPlus.CheckStatus(status);
		byte[] array = new byte[bufferSize];
		status = GDIPlus.GdipGetRegionData(nativeRegion, array, bufferSize, out var _);
		GDIPlus.CheckStatus(status);
		RegionData regionData = new RegionData();
		regionData.Data = array;
		return regionData;
	}

	public RectangleF[] GetRegionScans(Matrix matrix)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		Status status = GDIPlus.GdipGetRegionScansCount(nativeRegion, out var count, matrix.NativeObject);
		GDIPlus.CheckStatus(status);
		if (count == 0)
		{
			return new RectangleF[0];
		}
		RectangleF[] array = new RectangleF[count];
		int num = Marshal.SizeOf(array[0]);
		IntPtr intPtr = Marshal.AllocHGlobal(num * count);
		try
		{
			status = GDIPlus.GdipGetRegionScans(nativeRegion, intPtr, out count, matrix.NativeObject);
			GDIPlus.CheckStatus(status);
		}
		finally
		{
			GDIPlus.FromUnManagedMemoryToRectangles(intPtr, array);
		}
		return array;
	}

	public void Transform(Matrix matrix)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		Status status = GDIPlus.GdipTransformRegion(nativeRegion, matrix.NativeObject);
		GDIPlus.CheckStatus(status);
	}

	public Region Clone()
	{
		IntPtr cloned;
		Status status = GDIPlus.GdipCloneRegion(nativeRegion, out cloned);
		GDIPlus.CheckStatus(status);
		return new Region(cloned);
	}

	public void Dispose()
	{
		DisposeHandle();
		GC.SuppressFinalize(this);
	}

	private void DisposeHandle()
	{
		if (nativeRegion != IntPtr.Zero)
		{
			GDIPlus.GdipDeleteRegion(nativeRegion);
			nativeRegion = IntPtr.Zero;
		}
	}

	~Region()
	{
		DisposeHandle();
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public void ReleaseHrgn(IntPtr regionHandle)
	{
		if (regionHandle == IntPtr.Zero)
		{
			throw new ArgumentNullException("regionHandle");
		}
		Status status = Status.Ok;
		if (GDIPlus.RunningOnUnix())
		{
			status = GDIPlus.GdipDeleteRegion(regionHandle);
		}
		else if (!GDIPlus.DeleteObject(regionHandle))
		{
			status = Status.InvalidParameter;
		}
		GDIPlus.CheckStatus(status);
	}
}
