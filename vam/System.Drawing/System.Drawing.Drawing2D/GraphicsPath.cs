using System.ComponentModel;

namespace System.Drawing.Drawing2D;

public sealed class GraphicsPath : MarshalByRefObject, IDisposable, ICloneable
{
	private const float FlatnessDefault = 0.25f;

	internal IntPtr nativePath = IntPtr.Zero;

	public FillMode FillMode
	{
		get
		{
			FillMode fillMode;
			Status status = GDIPlus.GdipGetPathFillMode(nativePath, out fillMode);
			GDIPlus.CheckStatus(status);
			return fillMode;
		}
		set
		{
			if (value < FillMode.Alternate || value > FillMode.Winding)
			{
				throw new InvalidEnumArgumentException("FillMode", (int)value, typeof(FillMode));
			}
			Status status = GDIPlus.GdipSetPathFillMode(nativePath, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public PathData PathData
	{
		get
		{
			Status status = GDIPlus.GdipGetPointCount(nativePath, out var count);
			GDIPlus.CheckStatus(status);
			PointF[] points = new PointF[count];
			byte[] types = new byte[count];
			if (count > 0)
			{
				status = GDIPlus.GdipGetPathPoints(nativePath, points, count);
				GDIPlus.CheckStatus(status);
				status = GDIPlus.GdipGetPathTypes(nativePath, types, count);
				GDIPlus.CheckStatus(status);
			}
			PathData pathData = new PathData();
			pathData.Points = points;
			pathData.Types = types;
			return pathData;
		}
	}

	public PointF[] PathPoints
	{
		get
		{
			Status status = GDIPlus.GdipGetPointCount(nativePath, out var count);
			GDIPlus.CheckStatus(status);
			if (count == 0)
			{
				throw new ArgumentException("PathPoints");
			}
			PointF[] array = new PointF[count];
			status = GDIPlus.GdipGetPathPoints(nativePath, array, count);
			GDIPlus.CheckStatus(status);
			return array;
		}
	}

	public byte[] PathTypes
	{
		get
		{
			Status status = GDIPlus.GdipGetPointCount(nativePath, out var count);
			GDIPlus.CheckStatus(status);
			if (count == 0)
			{
				throw new ArgumentException("PathTypes");
			}
			byte[] array = new byte[count];
			status = GDIPlus.GdipGetPathTypes(nativePath, array, count);
			GDIPlus.CheckStatus(status);
			return array;
		}
	}

	public int PointCount
	{
		get
		{
			int count;
			Status status = GDIPlus.GdipGetPointCount(nativePath, out count);
			GDIPlus.CheckStatus(status);
			return count;
		}
	}

	internal IntPtr NativeObject
	{
		get
		{
			return nativePath;
		}
		set
		{
			nativePath = value;
		}
	}

	private GraphicsPath(IntPtr ptr)
	{
		nativePath = ptr;
	}

	public GraphicsPath()
	{
		Status status = GDIPlus.GdipCreatePath(FillMode.Alternate, out nativePath);
		GDIPlus.CheckStatus(status);
	}

	public GraphicsPath(FillMode fillMode)
	{
		Status status = GDIPlus.GdipCreatePath(fillMode, out nativePath);
		GDIPlus.CheckStatus(status);
	}

	public GraphicsPath(Point[] pts, byte[] types)
		: this(pts, types, FillMode.Alternate)
	{
	}

	public GraphicsPath(PointF[] pts, byte[] types)
		: this(pts, types, FillMode.Alternate)
	{
	}

	public GraphicsPath(Point[] pts, byte[] types, FillMode fillMode)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		if (pts.Length != types.Length)
		{
			throw new ArgumentException("Invalid parameter passed. Number of points and types must be same.");
		}
		Status status = GDIPlus.GdipCreatePath2I(pts, types, pts.Length, fillMode, out nativePath);
		GDIPlus.CheckStatus(status);
	}

	public GraphicsPath(PointF[] pts, byte[] types, FillMode fillMode)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		if (pts.Length != types.Length)
		{
			throw new ArgumentException("Invalid parameter passed. Number of points and types must be same.");
		}
		Status status = GDIPlus.GdipCreatePath2(pts, types, pts.Length, fillMode, out nativePath);
		GDIPlus.CheckStatus(status);
	}

	public object Clone()
	{
		IntPtr clonePath;
		Status status = GDIPlus.GdipClonePath(nativePath, out clonePath);
		GDIPlus.CheckStatus(status);
		return new GraphicsPath(clonePath);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~GraphicsPath()
	{
		Dispose(disposing: false);
	}

	private void Dispose(bool disposing)
	{
		if (nativePath != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeletePath(nativePath);
			GDIPlus.CheckStatus(status);
			nativePath = IntPtr.Zero;
		}
	}

	public void AddArc(Rectangle rect, float start_angle, float sweep_angle)
	{
		Status status = GDIPlus.GdipAddPathArcI(nativePath, rect.X, rect.Y, rect.Width, rect.Height, start_angle, sweep_angle);
		GDIPlus.CheckStatus(status);
	}

	public void AddArc(RectangleF rect, float start_angle, float sweep_angle)
	{
		Status status = GDIPlus.GdipAddPathArc(nativePath, rect.X, rect.Y, rect.Width, rect.Height, start_angle, sweep_angle);
		GDIPlus.CheckStatus(status);
	}

	public void AddArc(int x, int y, int width, int height, float start_angle, float sweep_angle)
	{
		Status status = GDIPlus.GdipAddPathArcI(nativePath, x, y, width, height, start_angle, sweep_angle);
		GDIPlus.CheckStatus(status);
	}

	public void AddArc(float x, float y, float width, float height, float start_angle, float sweep_angle)
	{
		Status status = GDIPlus.GdipAddPathArc(nativePath, x, y, width, height, start_angle, sweep_angle);
		GDIPlus.CheckStatus(status);
	}

	public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
	{
		Status status = GDIPlus.GdipAddPathBezierI(nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
		GDIPlus.CheckStatus(status);
	}

	public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
	{
		Status status = GDIPlus.GdipAddPathBezier(nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
		GDIPlus.CheckStatus(status);
	}

	public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
	{
		Status status = GDIPlus.GdipAddPathBezierI(nativePath, x1, y1, x2, y2, x3, y3, x4, y4);
		GDIPlus.CheckStatus(status);
	}

	public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
	{
		Status status = GDIPlus.GdipAddPathBezier(nativePath, x1, y1, x2, y2, x3, y3, x4, y4);
		GDIPlus.CheckStatus(status);
	}

	public void AddBeziers(params Point[] pts)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		Status status = GDIPlus.GdipAddPathBeziersI(nativePath, pts, pts.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddBeziers(PointF[] pts)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		Status status = GDIPlus.GdipAddPathBeziers(nativePath, pts, pts.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddEllipse(RectangleF r)
	{
		Status status = GDIPlus.GdipAddPathEllipse(nativePath, r.X, r.Y, r.Width, r.Height);
		GDIPlus.CheckStatus(status);
	}

	public void AddEllipse(float x, float y, float width, float height)
	{
		Status status = GDIPlus.GdipAddPathEllipse(nativePath, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void AddEllipse(Rectangle r)
	{
		Status status = GDIPlus.GdipAddPathEllipseI(nativePath, r.X, r.Y, r.Width, r.Height);
		GDIPlus.CheckStatus(status);
	}

	public void AddEllipse(int x, int y, int width, int height)
	{
		Status status = GDIPlus.GdipAddPathEllipseI(nativePath, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void AddLine(Point a, Point b)
	{
		Status status = GDIPlus.GdipAddPathLineI(nativePath, a.X, a.Y, b.X, b.Y);
		GDIPlus.CheckStatus(status);
	}

	public void AddLine(PointF a, PointF b)
	{
		Status status = GDIPlus.GdipAddPathLine(nativePath, a.X, a.Y, b.X, b.Y);
		GDIPlus.CheckStatus(status);
	}

	public void AddLine(int x1, int y1, int x2, int y2)
	{
		Status status = GDIPlus.GdipAddPathLineI(nativePath, x1, y1, x2, y2);
		GDIPlus.CheckStatus(status);
	}

	public void AddLine(float x1, float y1, float x2, float y2)
	{
		Status status = GDIPlus.GdipAddPathLine(nativePath, x1, y1, x2, y2);
		GDIPlus.CheckStatus(status);
	}

	public void AddLines(Point[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (points.Length == 0)
		{
			throw new ArgumentException("points");
		}
		Status status = GDIPlus.GdipAddPathLine2I(nativePath, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddLines(PointF[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (points.Length == 0)
		{
			throw new ArgumentException("points");
		}
		Status status = GDIPlus.GdipAddPathLine2(nativePath, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
	{
		Status status = GDIPlus.GdipAddPathPie(nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
		GDIPlus.CheckStatus(status);
	}

	public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle)
	{
		Status status = GDIPlus.GdipAddPathPieI(nativePath, x, y, width, height, startAngle, sweepAngle);
		GDIPlus.CheckStatus(status);
	}

	public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
	{
		Status status = GDIPlus.GdipAddPathPie(nativePath, x, y, width, height, startAngle, sweepAngle);
		GDIPlus.CheckStatus(status);
	}

	public void AddPolygon(Point[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathPolygonI(nativePath, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddPolygon(PointF[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathPolygon(nativePath, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddRectangle(Rectangle rect)
	{
		Status status = GDIPlus.GdipAddPathRectangleI(nativePath, rect.X, rect.Y, rect.Width, rect.Height);
		GDIPlus.CheckStatus(status);
	}

	public void AddRectangle(RectangleF rect)
	{
		Status status = GDIPlus.GdipAddPathRectangle(nativePath, rect.X, rect.Y, rect.Width, rect.Height);
		GDIPlus.CheckStatus(status);
	}

	public void AddRectangles(Rectangle[] rects)
	{
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		if (rects.Length == 0)
		{
			throw new ArgumentException("rects");
		}
		Status status = GDIPlus.GdipAddPathRectanglesI(nativePath, rects, rects.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddRectangles(RectangleF[] rects)
	{
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		if (rects.Length == 0)
		{
			throw new ArgumentException("rects");
		}
		Status status = GDIPlus.GdipAddPathRectangles(nativePath, rects, rects.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddPath(GraphicsPath addingPath, bool connect)
	{
		if (addingPath == null)
		{
			throw new ArgumentNullException("addingPath");
		}
		Status status = GDIPlus.GdipAddPathPath(nativePath, addingPath.nativePath, connect);
		GDIPlus.CheckStatus(status);
	}

	public PointF GetLastPoint()
	{
		PointF lastPoint;
		Status status = GDIPlus.GdipGetPathLastPoint(nativePath, out lastPoint);
		GDIPlus.CheckStatus(status);
		return lastPoint;
	}

	public void AddClosedCurve(Point[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathClosedCurveI(nativePath, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddClosedCurve(PointF[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathClosedCurve(nativePath, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddClosedCurve(Point[] points, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathClosedCurve2I(nativePath, points, points.Length, tension);
		GDIPlus.CheckStatus(status);
	}

	public void AddClosedCurve(PointF[] points, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathClosedCurve2(nativePath, points, points.Length, tension);
		GDIPlus.CheckStatus(status);
	}

	public void AddCurve(Point[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathCurveI(nativePath, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddCurve(PointF[] points)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathCurve(nativePath, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void AddCurve(Point[] points, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathCurve2I(nativePath, points, points.Length, tension);
		GDIPlus.CheckStatus(status);
	}

	public void AddCurve(PointF[] points, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathCurve2(nativePath, points, points.Length, tension);
		GDIPlus.CheckStatus(status);
	}

	public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathCurve3I(nativePath, points, points.Length, offset, numberOfSegments, tension);
		GDIPlus.CheckStatus(status);
	}

	public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipAddPathCurve3(nativePath, points, points.Length, offset, numberOfSegments, tension);
		GDIPlus.CheckStatus(status);
	}

	public void Reset()
	{
		Status status = GDIPlus.GdipResetPath(nativePath);
		GDIPlus.CheckStatus(status);
	}

	public void Reverse()
	{
		Status status = GDIPlus.GdipReversePath(nativePath);
		GDIPlus.CheckStatus(status);
	}

	public void Transform(Matrix matrix)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		Status status = GDIPlus.GdipTransformPath(nativePath, matrix.nativeMatrix);
		GDIPlus.CheckStatus(status);
	}

	[System.MonoTODO("The StringFormat parameter is ignored when using libgdiplus.")]
	public void AddString(string s, FontFamily family, int style, float emSize, Point origin, StringFormat format)
	{
		Rectangle layoutRect = default(Rectangle);
		layoutRect.X = origin.X;
		layoutRect.Y = origin.Y;
		AddString(s, family, style, emSize, layoutRect, format);
	}

	[System.MonoTODO("The StringFormat parameter is ignored when using libgdiplus.")]
	public void AddString(string s, FontFamily family, int style, float emSize, PointF origin, StringFormat format)
	{
		RectangleF layoutRect = default(RectangleF);
		layoutRect.X = origin.X;
		layoutRect.Y = origin.Y;
		AddString(s, family, style, emSize, layoutRect, format);
	}

	[System.MonoTODO("The layoutRect and StringFormat parameters are ignored when using libgdiplus.")]
	public void AddString(string s, FontFamily family, int style, float emSize, Rectangle layoutRect, StringFormat format)
	{
		if (family == null)
		{
			throw new ArgumentException("family");
		}
		IntPtr format2 = format?.NativeObject ?? IntPtr.Zero;
		Status status = GDIPlus.GdipAddPathStringI(nativePath, s, s.Length, family.NativeObject, style, emSize, ref layoutRect, format2);
		GDIPlus.CheckStatus(status);
	}

	[System.MonoTODO("The layoutRect and StringFormat parameters are ignored when using libgdiplus.")]
	public void AddString(string s, FontFamily family, int style, float emSize, RectangleF layoutRect, StringFormat format)
	{
		if (family == null)
		{
			throw new ArgumentException("family");
		}
		IntPtr format2 = format?.NativeObject ?? IntPtr.Zero;
		Status status = GDIPlus.GdipAddPathString(nativePath, s, s.Length, family.NativeObject, style, emSize, ref layoutRect, format2);
		GDIPlus.CheckStatus(status);
	}

	public void ClearMarkers()
	{
		Status status = GDIPlus.GdipClearPathMarkers(nativePath);
		GDIPlus.CheckStatus(status);
	}

	public void CloseAllFigures()
	{
		Status status = GDIPlus.GdipClosePathFigures(nativePath);
		GDIPlus.CheckStatus(status);
	}

	public void CloseFigure()
	{
		Status status = GDIPlus.GdipClosePathFigure(nativePath);
		GDIPlus.CheckStatus(status);
	}

	public void Flatten()
	{
		Flatten(null, 0.25f);
	}

	public void Flatten(Matrix matrix)
	{
		Flatten(matrix, 0.25f);
	}

	public void Flatten(Matrix matrix, float flatness)
	{
		IntPtr matrix2 = matrix?.nativeMatrix ?? IntPtr.Zero;
		Status status = GDIPlus.GdipFlattenPath(nativePath, matrix2, flatness);
		GDIPlus.CheckStatus(status);
	}

	public RectangleF GetBounds()
	{
		return GetBounds(null, null);
	}

	public RectangleF GetBounds(Matrix matrix)
	{
		return GetBounds(matrix, null);
	}

	public RectangleF GetBounds(Matrix matrix, Pen pen)
	{
		IntPtr matrix2 = matrix?.nativeMatrix ?? IntPtr.Zero;
		IntPtr pen2 = pen?.nativeObject ?? IntPtr.Zero;
		RectangleF bounds;
		Status status = GDIPlus.GdipGetPathWorldBounds(nativePath, out bounds, matrix2, pen2);
		GDIPlus.CheckStatus(status);
		return bounds;
	}

	public bool IsOutlineVisible(Point point, Pen pen)
	{
		return IsOutlineVisible(point.X, point.Y, pen, null);
	}

	public bool IsOutlineVisible(PointF point, Pen pen)
	{
		return IsOutlineVisible(point.X, point.Y, pen, null);
	}

	public bool IsOutlineVisible(int x, int y, Pen pen)
	{
		return IsOutlineVisible(x, y, pen, null);
	}

	public bool IsOutlineVisible(float x, float y, Pen pen)
	{
		return IsOutlineVisible(x, y, pen, null);
	}

	public bool IsOutlineVisible(Point pt, Pen pen, Graphics graphics)
	{
		return IsOutlineVisible(pt.X, pt.Y, pen, graphics);
	}

	public bool IsOutlineVisible(PointF pt, Pen pen, Graphics graphics)
	{
		return IsOutlineVisible(pt.X, pt.Y, pen, graphics);
	}

	public bool IsOutlineVisible(int x, int y, Pen pen, Graphics graphics)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		IntPtr graphics2 = graphics?.nativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsOutlineVisiblePathPointI(nativePath, x, y, pen.nativeObject, graphics2, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsOutlineVisible(float x, float y, Pen pen, Graphics graphics)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		IntPtr graphics2 = graphics?.nativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsOutlineVisiblePathPoint(nativePath, x, y, pen.nativeObject, graphics2, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(Point point)
	{
		return IsVisible(point.X, point.Y, null);
	}

	public bool IsVisible(PointF point)
	{
		return IsVisible(point.X, point.Y, null);
	}

	public bool IsVisible(int x, int y)
	{
		return IsVisible(x, y, null);
	}

	public bool IsVisible(float x, float y)
	{
		return IsVisible(x, y, null);
	}

	public bool IsVisible(Point pt, Graphics graphics)
	{
		return IsVisible(pt.X, pt.Y, graphics);
	}

	public bool IsVisible(PointF pt, Graphics graphics)
	{
		return IsVisible(pt.X, pt.Y, graphics);
	}

	public bool IsVisible(int x, int y, Graphics graphics)
	{
		IntPtr graphics2 = graphics?.nativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsVisiblePathPointI(nativePath, x, y, graphics2, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(float x, float y, Graphics graphics)
	{
		IntPtr graphics2 = graphics?.nativeObject ?? IntPtr.Zero;
		bool result;
		Status status = GDIPlus.GdipIsVisiblePathPoint(nativePath, x, y, graphics2, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public void SetMarkers()
	{
		Status status = GDIPlus.GdipSetPathMarker(nativePath);
		GDIPlus.CheckStatus(status);
	}

	public void StartFigure()
	{
		Status status = GDIPlus.GdipStartPathFigure(nativePath);
		GDIPlus.CheckStatus(status);
	}

	[System.MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
	public void Warp(PointF[] destPoints, RectangleF srcRect)
	{
		Warp(destPoints, srcRect, null, WarpMode.Perspective, 0.25f);
	}

	[System.MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
	public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix)
	{
		Warp(destPoints, srcRect, matrix, WarpMode.Perspective, 0.25f);
	}

	[System.MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
	public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode)
	{
		Warp(destPoints, srcRect, matrix, warpMode, 0.25f);
	}

	[System.MonoTODO("GdipWarpPath isn't implemented in libgdiplus")]
	public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode, float flatness)
	{
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		IntPtr matrix2 = matrix?.nativeMatrix ?? IntPtr.Zero;
		Status status = GDIPlus.GdipWarpPath(nativePath, matrix2, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, warpMode, flatness);
		GDIPlus.CheckStatus(status);
	}

	[System.MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
	public void Widen(Pen pen)
	{
		Widen(pen, null, 0.25f);
	}

	[System.MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
	public void Widen(Pen pen, Matrix matrix)
	{
		Widen(pen, matrix, 0.25f);
	}

	[System.MonoTODO("GdipWidenPath isn't implemented in libgdiplus")]
	public void Widen(Pen pen, Matrix matrix, float flatness)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (PointCount != 0)
		{
			IntPtr matrix2 = matrix?.nativeMatrix ?? IntPtr.Zero;
			Status status = GDIPlus.GdipWidenPath(nativePath, pen.nativeObject, matrix2, flatness);
			GDIPlus.CheckStatus(status);
		}
	}
}
