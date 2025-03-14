using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Drawing;

public sealed class Graphics : MarshalByRefObject, IDisposable, IDeviceContext
{
	public delegate bool EnumerateMetafileProc(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData);

	public delegate bool DrawImageAbort(IntPtr callbackData);

	private const string MetafileEnumeration = "Metafiles enumeration, for both WMF and EMF formats, isn't supported.";

	internal IntPtr nativeObject = IntPtr.Zero;

	internal CarbonContext context;

	private bool disposed;

	private static float defDpiX;

	private static float defDpiY;

	private IntPtr deviceContextHdc;

	internal static float systemDpiX
	{
		get
		{
			if (defDpiX == 0f)
			{
				Bitmap image = new Bitmap(1, 1);
				Graphics graphics = FromImage(image);
				defDpiX = graphics.DpiX;
				defDpiY = graphics.DpiY;
			}
			return defDpiX;
		}
	}

	internal static float systemDpiY
	{
		get
		{
			if (defDpiY == 0f)
			{
				Bitmap image = new Bitmap(1, 1);
				Graphics graphics = FromImage(image);
				defDpiX = graphics.DpiX;
				defDpiY = graphics.DpiY;
			}
			return defDpiY;
		}
	}

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

	public Region Clip
	{
		get
		{
			Region region = new Region();
			Status status = GDIPlus.GdipGetClip(nativeObject, region.NativeObject);
			GDIPlus.CheckStatus(status);
			return region;
		}
		set
		{
			SetClip(value, CombineMode.Replace);
		}
	}

	public RectangleF ClipBounds
	{
		get
		{
			RectangleF rect = default(RectangleF);
			Status status = GDIPlus.GdipGetClipBounds(nativeObject, out rect);
			GDIPlus.CheckStatus(status);
			return rect;
		}
	}

	public CompositingMode CompositingMode
	{
		get
		{
			CompositingMode compositingMode;
			Status status = GDIPlus.GdipGetCompositingMode(nativeObject, out compositingMode);
			GDIPlus.CheckStatus(status);
			return compositingMode;
		}
		set
		{
			Status status = GDIPlus.GdipSetCompositingMode(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public CompositingQuality CompositingQuality
	{
		get
		{
			CompositingQuality compositingQuality;
			Status status = GDIPlus.GdipGetCompositingQuality(nativeObject, out compositingQuality);
			GDIPlus.CheckStatus(status);
			return compositingQuality;
		}
		set
		{
			Status status = GDIPlus.GdipSetCompositingQuality(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public float DpiX
	{
		get
		{
			float dpi;
			Status status = GDIPlus.GdipGetDpiX(nativeObject, out dpi);
			GDIPlus.CheckStatus(status);
			return dpi;
		}
	}

	public float DpiY
	{
		get
		{
			float dpi;
			Status status = GDIPlus.GdipGetDpiY(nativeObject, out dpi);
			GDIPlus.CheckStatus(status);
			return dpi;
		}
	}

	public InterpolationMode InterpolationMode
	{
		get
		{
			InterpolationMode interpolationMode = InterpolationMode.Invalid;
			Status status = GDIPlus.GdipGetInterpolationMode(nativeObject, out interpolationMode);
			GDIPlus.CheckStatus(status);
			return interpolationMode;
		}
		set
		{
			Status status = GDIPlus.GdipSetInterpolationMode(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public bool IsClipEmpty
	{
		get
		{
			bool result = false;
			Status status = GDIPlus.GdipIsClipEmpty(nativeObject, out result);
			GDIPlus.CheckStatus(status);
			return result;
		}
	}

	public bool IsVisibleClipEmpty
	{
		get
		{
			bool result = false;
			Status status = GDIPlus.GdipIsVisibleClipEmpty(nativeObject, out result);
			GDIPlus.CheckStatus(status);
			return result;
		}
	}

	public float PageScale
	{
		get
		{
			float scale;
			Status status = GDIPlus.GdipGetPageScale(nativeObject, out scale);
			GDIPlus.CheckStatus(status);
			return scale;
		}
		set
		{
			Status status = GDIPlus.GdipSetPageScale(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public GraphicsUnit PageUnit
	{
		get
		{
			GraphicsUnit unit;
			Status status = GDIPlus.GdipGetPageUnit(nativeObject, out unit);
			GDIPlus.CheckStatus(status);
			return unit;
		}
		set
		{
			Status status = GDIPlus.GdipSetPageUnit(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	[System.MonoTODO("This property does not do anything when used with libgdiplus.")]
	public PixelOffsetMode PixelOffsetMode
	{
		get
		{
			PixelOffsetMode pixelOffsetMode = PixelOffsetMode.Invalid;
			Status status = GDIPlus.GdipGetPixelOffsetMode(nativeObject, out pixelOffsetMode);
			GDIPlus.CheckStatus(status);
			return pixelOffsetMode;
		}
		set
		{
			Status status = GDIPlus.GdipSetPixelOffsetMode(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public Point RenderingOrigin
	{
		get
		{
			int x;
			int y;
			Status status = GDIPlus.GdipGetRenderingOrigin(nativeObject, out x, out y);
			GDIPlus.CheckStatus(status);
			return new Point(x, y);
		}
		set
		{
			Status status = GDIPlus.GdipSetRenderingOrigin(nativeObject, value.X, value.Y);
			GDIPlus.CheckStatus(status);
		}
	}

	public SmoothingMode SmoothingMode
	{
		get
		{
			SmoothingMode smoothingMode = SmoothingMode.Invalid;
			Status status = GDIPlus.GdipGetSmoothingMode(nativeObject, out smoothingMode);
			GDIPlus.CheckStatus(status);
			return smoothingMode;
		}
		set
		{
			Status status = GDIPlus.GdipSetSmoothingMode(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	[System.MonoTODO("This property does not do anything when used with libgdiplus.")]
	public int TextContrast
	{
		get
		{
			int contrast;
			Status status = GDIPlus.GdipGetTextContrast(nativeObject, out contrast);
			GDIPlus.CheckStatus(status);
			return contrast;
		}
		set
		{
			Status status = GDIPlus.GdipSetTextContrast(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public TextRenderingHint TextRenderingHint
	{
		get
		{
			TextRenderingHint mode;
			Status status = GDIPlus.GdipGetTextRenderingHint(nativeObject, out mode);
			GDIPlus.CheckStatus(status);
			return mode;
		}
		set
		{
			Status status = GDIPlus.GdipSetTextRenderingHint(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public Matrix Transform
	{
		get
		{
			Matrix matrix = new Matrix();
			Status status = GDIPlus.GdipGetWorldTransform(nativeObject, matrix.nativeMatrix);
			GDIPlus.CheckStatus(status);
			return matrix;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Status status = GDIPlus.GdipSetWorldTransform(nativeObject, value.nativeMatrix);
			GDIPlus.CheckStatus(status);
		}
	}

	public RectangleF VisibleClipBounds
	{
		get
		{
			RectangleF rect;
			Status status = GDIPlus.GdipGetVisibleClipBounds(nativeObject, out rect);
			GDIPlus.CheckStatus(status);
			return rect;
		}
	}

	internal Graphics(IntPtr nativeGraphics)
	{
		nativeObject = nativeGraphics;
	}

	~Graphics()
	{
		Dispose();
	}

	[System.MonoTODO("Metafiles, both WMF and EMF formats, aren't supported.")]
	public void AddMetafileComment(byte[] data)
	{
		throw new NotImplementedException();
	}

	public GraphicsContainer BeginContainer()
	{
		uint state;
		Status status = GDIPlus.GdipBeginContainer2(nativeObject, out state);
		GDIPlus.CheckStatus(status);
		return new GraphicsContainer(state);
	}

	[System.MonoTODO("The rectangles and unit parameters aren't supported in libgdiplus")]
	public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
	{
		uint state;
		Status status = GDIPlus.GdipBeginContainerI(nativeObject, ref dstrect, ref srcrect, unit, out state);
		GDIPlus.CheckStatus(status);
		return new GraphicsContainer(state);
	}

	[System.MonoTODO("The rectangles and unit parameters aren't supported in libgdiplus")]
	public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
	{
		uint state;
		Status status = GDIPlus.GdipBeginContainer(nativeObject, ref dstrect, ref srcrect, unit, out state);
		GDIPlus.CheckStatus(status);
		return new GraphicsContainer(state);
	}

	public void Clear(Color color)
	{
		Status status = GDIPlus.GdipGraphicsClear(nativeObject, color.ToArgb());
		GDIPlus.CheckStatus(status);
	}

	[System.MonoLimitation("Works on Win32 and on X11 (but not on Cocoa and Quartz)")]
	public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize)
	{
		CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, CopyPixelOperation.SourceCopy);
	}

	[System.MonoLimitation("Works on Win32 and (for CopyPixelOperation.SourceCopy only) on X11 but not on Cocoa and Quartz")]
	public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
	{
		CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, copyPixelOperation);
	}

	[System.MonoLimitation("Works on Win32 and on X11 (but not on Cocoa and Quartz)")]
	public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
	{
		CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize, CopyPixelOperation.SourceCopy);
	}

	[System.MonoLimitation("Works on Win32 and (for CopyPixelOperation.SourceCopy only) on X11 but not on Cocoa and Quartz")]
	public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
	{
		if (!Enum.IsDefined(typeof(CopyPixelOperation), copyPixelOperation))
		{
			throw new InvalidEnumArgumentException(global::Locale.GetText("Enum argument value '{0}' is not valid for CopyPixelOperation", copyPixelOperation));
		}
		if (GDIPlus.UseX11Drawable)
		{
			CopyFromScreenX11(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
		}
		else if (GDIPlus.UseCarbonDrawable)
		{
			CopyFromScreenMac(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
		}
		else
		{
			CopyFromScreenWin32(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
		}
	}

	private void CopyFromScreenWin32(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
	{
		IntPtr desktopWindow = GDIPlus.GetDesktopWindow();
		IntPtr dC = GDIPlus.GetDC(desktopWindow);
		IntPtr hdc = GetHdc();
		GDIPlus.BitBlt(hdc, destinationX, destinationY, blockRegionSize.Width, blockRegionSize.Height, dC, sourceX, sourceY, (int)copyPixelOperation);
		GDIPlus.ReleaseDC(IntPtr.Zero, dC);
		ReleaseHdc(hdc);
	}

	private void CopyFromScreenMac(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
	{
		throw new NotImplementedException();
	}

	private void CopyFromScreenX11(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
	{
		int pane = -1;
		int nitems = 0;
		if (copyPixelOperation != CopyPixelOperation.SourceCopy)
		{
			throw new NotImplementedException("Operation not implemented under X11");
		}
		if (GDIPlus.Display == IntPtr.Zero)
		{
			GDIPlus.Display = GDIPlus.XOpenDisplay(IntPtr.Zero);
		}
		IntPtr drawable = GDIPlus.XRootWindow(GDIPlus.Display, 0);
		IntPtr visual = GDIPlus.XDefaultVisual(GDIPlus.Display, 0);
		XVisualInfo vinfo_template = default(XVisualInfo);
		vinfo_template.visualid = GDIPlus.XVisualIDFromVisual(visual);
		IntPtr intPtr = GDIPlus.XGetVisualInfo(GDIPlus.Display, 1, ref vinfo_template, ref nitems);
		vinfo_template = (XVisualInfo)Marshal.PtrToStructure(intPtr, typeof(XVisualInfo));
		IntPtr image = GDIPlus.XGetImage(GDIPlus.Display, drawable, sourceX, sourceY, blockRegionSize.Width, blockRegionSize.Height, pane, 2);
		Bitmap bitmap = new Bitmap(blockRegionSize.Width, blockRegionSize.Height);
		int num = (int)vinfo_template.red_mask;
		int num2 = (int)vinfo_template.blue_mask;
		int num3 = (int)vinfo_template.green_mask;
		for (int i = 0; i < blockRegionSize.Height; i++)
		{
			for (int j = 0; j < blockRegionSize.Width; j++)
			{
				int num4 = GDIPlus.XGetPixel(image, j, i);
				int red;
				int green;
				int blue;
				switch (vinfo_template.depth)
				{
				case 16u:
					red = ((num4 & num) >> 8) & 0xFF;
					green = ((num4 & num3) >> 3) & 0xFF;
					blue = ((num4 & num2) << 3) & 0xFF;
					break;
				case 24u:
				case 32u:
					red = ((num4 & num) >> 16) & 0xFF;
					green = ((num4 & num3) >> 8) & 0xFF;
					blue = num4 & num2 & 0xFF;
					break;
				default:
				{
					string text = global::Locale.GetText("{0}bbp depth not supported.", vinfo_template.depth);
					throw new NotImplementedException(text);
				}
				}
				bitmap.SetPixel(j, i, Color.FromArgb(255, red, green, blue));
			}
		}
		DrawImage(bitmap, destinationX, destinationY);
		bitmap.Dispose();
		GDIPlus.XDestroyImage(image);
		GDIPlus.XFree(intPtr);
	}

	public void Dispose()
	{
		if (!disposed)
		{
			if (GDIPlus.UseCarbonDrawable && context.ctx != IntPtr.Zero)
			{
				Flush();
				Carbon.CGContextSynchronize(context.ctx);
				Carbon.ReleaseContext(context.port, context.ctx);
			}
			Status status = GDIPlus.GdipDeleteGraphics(nativeObject);
			nativeObject = IntPtr.Zero;
			GDIPlus.CheckStatus(status);
			disposed = true;
		}
		GC.SuppressFinalize(this);
	}

	public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
	{
		DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
	}

	public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
	{
		DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
	}

	public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawArc(nativeObject, pen.nativeObject, x, y, width, height, startAngle, sweepAngle);
		GDIPlus.CheckStatus(status);
	}

	public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawArcI(nativeObject, pen.nativeObject, x, y, width, height, startAngle, sweepAngle);
		GDIPlus.CheckStatus(status);
	}

	public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawBezier(nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
		GDIPlus.CheckStatus(status);
	}

	public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawBezierI(nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
		GDIPlus.CheckStatus(status);
	}

	public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawBezier(nativeObject, pen.nativeObject, x1, y1, x2, y2, x3, y3, x4, y4);
		GDIPlus.CheckStatus(status);
	}

	public void DrawBeziers(Pen pen, Point[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		int num = points.Length;
		if (num >= 4)
		{
			for (int i = 0; i < num - 1; i += 3)
			{
				Point point = points[i];
				Point point2 = points[i + 1];
				Point point3 = points[i + 2];
				Point point4 = points[i + 3];
				Status status = GDIPlus.GdipDrawBezier(nativeObject, pen.nativeObject, point.X, point.Y, point2.X, point2.Y, point3.X, point3.Y, point4.X, point4.Y);
				GDIPlus.CheckStatus(status);
			}
		}
	}

	public void DrawBeziers(Pen pen, PointF[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		int num = points.Length;
		if (num >= 4)
		{
			for (int i = 0; i < num - 1; i += 3)
			{
				PointF pointF = points[i];
				PointF pointF2 = points[i + 1];
				PointF pointF3 = points[i + 2];
				PointF pointF4 = points[i + 3];
				Status status = GDIPlus.GdipDrawBezier(nativeObject, pen.nativeObject, pointF.X, pointF.Y, pointF2.X, pointF2.Y, pointF3.X, pointF3.Y, pointF4.X, pointF4.Y);
				GDIPlus.CheckStatus(status);
			}
		}
	}

	public void DrawClosedCurve(Pen pen, PointF[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawClosedCurve(nativeObject, pen.nativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawClosedCurve(Pen pen, Point[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawClosedCurveI(nativeObject, pen.nativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawClosedCurve2I(nativeObject, pen.nativeObject, points, points.Length, tension);
		GDIPlus.CheckStatus(status);
	}

	public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawClosedCurve2(nativeObject, pen.nativeObject, points, points.Length, tension);
		GDIPlus.CheckStatus(status);
	}

	public void DrawCurve(Pen pen, Point[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawCurveI(nativeObject, pen.nativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawCurve(Pen pen, PointF[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawCurve(nativeObject, pen.nativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawCurve(Pen pen, PointF[] points, float tension)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawCurve2(nativeObject, pen.nativeObject, points, points.Length, tension);
		GDIPlus.CheckStatus(status);
	}

	public void DrawCurve(Pen pen, Point[] points, float tension)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawCurve2I(nativeObject, pen.nativeObject, points, points.Length, tension);
		GDIPlus.CheckStatus(status);
	}

	public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawCurve3(nativeObject, pen.nativeObject, points, points.Length, offset, numberOfSegments, 0.5f);
		GDIPlus.CheckStatus(status);
	}

	public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawCurve3I(nativeObject, pen.nativeObject, points, points.Length, offset, numberOfSegments, tension);
		GDIPlus.CheckStatus(status);
	}

	public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawCurve3(nativeObject, pen.nativeObject, points, points.Length, offset, numberOfSegments, tension);
		GDIPlus.CheckStatus(status);
	}

	public void DrawEllipse(Pen pen, Rectangle rect)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void DrawEllipse(Pen pen, RectangleF rect)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void DrawEllipse(Pen pen, int x, int y, int width, int height)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawEllipseI(nativeObject, pen.nativeObject, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void DrawEllipse(Pen pen, float x, float y, float width, float height)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawEllipse(nativeObject, pen.nativeObject, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void DrawIcon(Icon icon, Rectangle targetRect)
	{
		if (icon == null)
		{
			throw new ArgumentNullException("icon");
		}
		DrawImage(icon.GetInternalBitmap(), targetRect);
	}

	public void DrawIcon(Icon icon, int x, int y)
	{
		if (icon == null)
		{
			throw new ArgumentNullException("icon");
		}
		DrawImage(icon.GetInternalBitmap(), x, y);
	}

	public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
	{
		if (icon == null)
		{
			throw new ArgumentNullException("icon");
		}
		DrawImageUnscaled(icon.GetInternalBitmap(), targetRect);
	}

	public void DrawImage(Image image, RectangleF rect)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRect(nativeObject, image.NativeObject, rect.X, rect.Y, rect.Width, rect.Height);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, PointF point)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImage(nativeObject, image.NativeObject, point.X, point.Y);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Point[] destPoints)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		Status status = GDIPlus.GdipDrawImagePointsI(nativeObject, image.NativeObject, destPoints, destPoints.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Point point)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		DrawImage(image, point.X, point.Y);
	}

	public void DrawImage(Image image, Rectangle rect)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void DrawImage(Image image, PointF[] destPoints)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		Status status = GDIPlus.GdipDrawImagePoints(nativeObject, image.NativeObject, destPoints, destPoints.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, int x, int y)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageI(nativeObject, image.NativeObject, x, y);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, float x, float y)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImage(nativeObject, image.NativeObject, x, y);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectRectI(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		Status status = GDIPlus.GdipDrawImagePointsRectI(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		Status status = GDIPlus.GdipDrawImagePointsRect(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		Status status = GDIPlus.GdipDrawImagePointsRectI(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.NativeObject ?? IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, float x, float y, float width, float height)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRect(nativeObject, image.NativeObject, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		Status status = GDIPlus.GdipDrawImagePointsRect(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.NativeObject ?? IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImagePointRectI(nativeObject, image.NativeObject, x, y, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, int x, int y, int width, int height)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectI(nativeObject, image.nativeObject, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImagePointRect(nativeObject, image.nativeObject, x, y, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		Status status = GDIPlus.GdipDrawImagePointsRect(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.NativeObject ?? IntPtr.Zero, callback, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		Status status = GDIPlus.GdipDrawImagePointsRectI(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.NativeObject ?? IntPtr.Zero, callback, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (destPoints == null)
		{
			throw new ArgumentNullException("destPoints");
		}
		Status status = GDIPlus.GdipDrawImagePointsRectI(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.NativeObject ?? IntPtr.Zero, callback, (IntPtr)callbackData);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
	{
		Status status = GDIPlus.GdipDrawImagePointsRect(nativeObject, image.NativeObject, destPoints, destPoints.Length, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, imageAttr?.NativeObject ?? IntPtr.Zero, callback, (IntPtr)callbackData);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectRectI(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs?.NativeObject ?? IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectRectI(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr?.NativeObject ?? IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectRectI(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr?.NativeObject ?? IntPtr.Zero, callback, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs?.NativeObject ?? IntPtr.Zero, callback, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs?.NativeObject ?? IntPtr.Zero, callback, callbackData);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Status status = GDIPlus.GdipDrawImageRectRect(nativeObject, image.NativeObject, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs?.NativeObject ?? IntPtr.Zero, callback, callbackData);
		GDIPlus.CheckStatus(status);
	}

	public void DrawImageUnscaled(Image image, Point point)
	{
		DrawImageUnscaled(image, point.X, point.Y);
	}

	public void DrawImageUnscaled(Image image, Rectangle rect)
	{
		DrawImageUnscaled(image, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void DrawImageUnscaled(Image image, int x, int y)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		DrawImage(image, x, y, image.Width, image.Height);
	}

	public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (width <= 0 || height <= 0)
		{
			return;
		}
		using Image image2 = new Bitmap(width, height);
		using Graphics graphics = FromImage(image2);
		graphics.DrawImage(image, 0, 0, image.Width, image.Height);
		DrawImage(image2, x, y, width, height);
	}

	public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		int width = ((image.Width <= rect.Width) ? image.Width : rect.Width);
		int height = ((image.Height <= rect.Height) ? image.Height : rect.Height);
		DrawImageUnscaled(image, rect.X, rect.Y, width, height);
	}

	public void DrawLine(Pen pen, PointF pt1, PointF pt2)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawLine(nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y);
		GDIPlus.CheckStatus(status);
	}

	public void DrawLine(Pen pen, Point pt1, Point pt2)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawLineI(nativeObject, pen.nativeObject, pt1.X, pt1.Y, pt2.X, pt2.Y);
		GDIPlus.CheckStatus(status);
	}

	public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawLineI(nativeObject, pen.nativeObject, x1, y1, x2, y2);
		GDIPlus.CheckStatus(status);
	}

	public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawLine(nativeObject, pen.nativeObject, x1, y1, x2, y2);
		GDIPlus.CheckStatus(status);
	}

	public void DrawLines(Pen pen, PointF[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawLines(nativeObject, pen.nativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawLines(Pen pen, Point[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawLinesI(nativeObject, pen.nativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawPath(Pen pen, GraphicsPath path)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		Status status = GDIPlus.GdipDrawPath(nativeObject, pen.nativeObject, path.nativePath);
		GDIPlus.CheckStatus(status);
	}

	public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
	}

	public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
	}

	public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawPie(nativeObject, pen.nativeObject, x, y, width, height, startAngle, sweepAngle);
		GDIPlus.CheckStatus(status);
	}

	public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawPieI(nativeObject, pen.nativeObject, x, y, width, height, startAngle, sweepAngle);
		GDIPlus.CheckStatus(status);
	}

	public void DrawPolygon(Pen pen, Point[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawPolygonI(nativeObject, pen.nativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawPolygon(Pen pen, PointF[] points)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipDrawPolygon(nativeObject, pen.nativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawRectangle(Pen pen, Rectangle rect)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
	}

	public void DrawRectangle(Pen pen, float x, float y, float width, float height)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawRectangle(nativeObject, pen.nativeObject, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void DrawRectangle(Pen pen, int x, int y, int width, int height)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("pen");
		}
		Status status = GDIPlus.GdipDrawRectangleI(nativeObject, pen.nativeObject, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void DrawRectangles(Pen pen, RectangleF[] rects)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("image");
		}
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		Status status = GDIPlus.GdipDrawRectangles(nativeObject, pen.nativeObject, rects, rects.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawRectangles(Pen pen, Rectangle[] rects)
	{
		if (pen == null)
		{
			throw new ArgumentNullException("image");
		}
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		Status status = GDIPlus.GdipDrawRectanglesI(nativeObject, pen.nativeObject, rects, rects.Length);
		GDIPlus.CheckStatus(status);
	}

	public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
	{
		DrawString(s, font, brush, layoutRectangle, null);
	}

	public void DrawString(string s, Font font, Brush brush, PointF point)
	{
		DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), null);
	}

	public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
	{
		DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), format);
	}

	public void DrawString(string s, Font font, Brush brush, float x, float y)
	{
		DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), null);
	}

	public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
	{
		DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), format);
	}

	public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
	{
		if (font == null)
		{
			throw new ArgumentNullException("font");
		}
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (s != null && s.Length != 0)
		{
			Status status = GDIPlus.GdipDrawString(nativeObject, s, s.Length, font.NativeObject, ref layoutRectangle, format?.NativeObject ?? IntPtr.Zero, brush.nativeObject);
			GDIPlus.CheckStatus(status);
		}
	}

	public void EndContainer(GraphicsContainer container)
	{
		if (container == null)
		{
			throw new ArgumentNullException("container");
		}
		Status status = GDIPlus.GdipEndContainer(nativeObject, container.NativeObject);
		GDIPlus.CheckStatus(status);
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Metafiles enumeration, for both WMF and EMF formats, isn't supported.")]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
	{
		throw new NotImplementedException();
	}

	public void ExcludeClip(Rectangle rect)
	{
		Status status = GDIPlus.GdipSetClipRectI(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Exclude);
		GDIPlus.CheckStatus(status);
	}

	public void ExcludeClip(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		Status status = GDIPlus.GdipSetClipRegion(nativeObject, region.NativeObject, CombineMode.Exclude);
		GDIPlus.CheckStatus(status);
	}

	public void FillClosedCurve(Brush brush, PointF[] points)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipFillClosedCurve(nativeObject, brush.NativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void FillClosedCurve(Brush brush, Point[] points)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipFillClosedCurveI(nativeObject, brush.NativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		FillClosedCurve(brush, points, fillmode, 0.5f);
	}

	public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		FillClosedCurve(brush, points, fillmode, 0.5f);
	}

	public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipFillClosedCurve2(nativeObject, brush.NativeObject, points, points.Length, tension, fillmode);
		GDIPlus.CheckStatus(status);
	}

	public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipFillClosedCurve2I(nativeObject, brush.NativeObject, points, points.Length, tension, fillmode);
		GDIPlus.CheckStatus(status);
	}

	public void FillEllipse(Brush brush, Rectangle rect)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void FillEllipse(Brush brush, RectangleF rect)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
	}

	public void FillEllipse(Brush brush, float x, float y, float width, float height)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		Status status = GDIPlus.GdipFillEllipse(nativeObject, brush.nativeObject, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void FillEllipse(Brush brush, int x, int y, int width, int height)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		Status status = GDIPlus.GdipFillEllipseI(nativeObject, brush.nativeObject, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void FillPath(Brush brush, GraphicsPath path)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		Status status = GDIPlus.GdipFillPath(nativeObject, brush.NativeObject, path.NativeObject);
		GDIPlus.CheckStatus(status);
	}

	public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		Status status = GDIPlus.GdipFillPie(nativeObject, brush.NativeObject, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
		GDIPlus.CheckStatus(status);
	}

	public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		Status status = GDIPlus.GdipFillPieI(nativeObject, brush.NativeObject, x, y, width, height, startAngle, sweepAngle);
		GDIPlus.CheckStatus(status);
	}

	public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		Status status = GDIPlus.GdipFillPie(nativeObject, brush.NativeObject, x, y, width, height, startAngle, sweepAngle);
		GDIPlus.CheckStatus(status);
	}

	public void FillPolygon(Brush brush, PointF[] points)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipFillPolygon2(nativeObject, brush.nativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void FillPolygon(Brush brush, Point[] points)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipFillPolygon2I(nativeObject, brush.nativeObject, points, points.Length);
		GDIPlus.CheckStatus(status);
	}

	public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipFillPolygonI(nativeObject, brush.nativeObject, points, points.Length, fillMode);
		GDIPlus.CheckStatus(status);
	}

	public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		Status status = GDIPlus.GdipFillPolygon(nativeObject, brush.nativeObject, points, points.Length, fillMode);
		GDIPlus.CheckStatus(status);
	}

	public void FillRectangle(Brush brush, RectangleF rect)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
	}

	public void FillRectangle(Brush brush, Rectangle rect)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
	}

	public void FillRectangle(Brush brush, int x, int y, int width, int height)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		Status status = GDIPlus.GdipFillRectangleI(nativeObject, brush.nativeObject, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void FillRectangle(Brush brush, float x, float y, float width, float height)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		Status status = GDIPlus.GdipFillRectangle(nativeObject, brush.nativeObject, x, y, width, height);
		GDIPlus.CheckStatus(status);
	}

	public void FillRectangles(Brush brush, Rectangle[] rects)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		Status status = GDIPlus.GdipFillRectanglesI(nativeObject, brush.nativeObject, rects, rects.Length);
		GDIPlus.CheckStatus(status);
	}

	public void FillRectangles(Brush brush, RectangleF[] rects)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (rects == null)
		{
			throw new ArgumentNullException("rects");
		}
		Status status = GDIPlus.GdipFillRectangles(nativeObject, brush.nativeObject, rects, rects.Length);
		GDIPlus.CheckStatus(status);
	}

	public void FillRegion(Brush brush, Region region)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		Status status = GDIPlus.GdipFillRegion(nativeObject, brush.NativeObject, region.NativeObject);
		GDIPlus.CheckStatus(status);
	}

	public void Flush()
	{
		Flush(FlushIntention.Flush);
	}

	public void Flush(FlushIntention intention)
	{
		if (!(nativeObject == IntPtr.Zero))
		{
			Status status = GDIPlus.GdipFlush(nativeObject, intention);
			GDIPlus.CheckStatus(status);
			if (GDIPlus.UseCarbonDrawable && context.ctx != IntPtr.Zero)
			{
				Carbon.CGContextSynchronize(context.ctx);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Graphics FromHdc(IntPtr hdc)
	{
		IntPtr graphics;
		Status status = GDIPlus.GdipCreateFromHDC(hdc, out graphics);
		GDIPlus.CheckStatus(status);
		return new Graphics(graphics);
	}

	[System.MonoTODO]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
	{
		throw new NotImplementedException();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static Graphics FromHdcInternal(IntPtr hdc)
	{
		GDIPlus.Display = hdc;
		return null;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Graphics FromHwnd(IntPtr hwnd)
	{
		IntPtr graphics;
		if (GDIPlus.UseCarbonDrawable)
		{
			CarbonContext cGContextForView = Carbon.GetCGContextForView(hwnd);
			GDIPlus.GdipCreateFromContext_macosx(cGContextForView.ctx, cGContextForView.width, cGContextForView.height, out graphics);
			Graphics graphics2 = new Graphics(graphics);
			graphics2.context = cGContextForView;
			return graphics2;
		}
		if (GDIPlus.UseX11Drawable)
		{
			if (GDIPlus.Display == IntPtr.Zero)
			{
				GDIPlus.Display = GDIPlus.XOpenDisplay(IntPtr.Zero);
				if (GDIPlus.Display == IntPtr.Zero)
				{
					throw new NotSupportedException("Could not open display (X-Server required. Check you DISPLAY environment variable)");
				}
			}
			if (hwnd == IntPtr.Zero)
			{
				hwnd = GDIPlus.XRootWindow(GDIPlus.Display, GDIPlus.XDefaultScreen(GDIPlus.Display));
			}
			return FromXDrawable(hwnd, GDIPlus.Display);
		}
		Status status = GDIPlus.GdipCreateFromHWND(hwnd, out graphics);
		GDIPlus.CheckStatus(status);
		return new Graphics(graphics);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static Graphics FromHwndInternal(IntPtr hwnd)
	{
		return FromHwnd(hwnd);
	}

	public static Graphics FromImage(Image image)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if ((image.PixelFormat & PixelFormat.Indexed) != 0)
		{
			throw new Exception(global::Locale.GetText("Cannot create Graphics from an indexed bitmap."));
		}
		IntPtr graphics;
		Status status = GDIPlus.GdipGetImageGraphicsContext(image.nativeObject, out graphics);
		GDIPlus.CheckStatus(status);
		Graphics graphics2 = new Graphics(graphics);
		if (GDIPlus.RunningOnUnix())
		{
			Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
			GDIPlus.GdipSetVisibleClip_linux(graphics2.NativeObject, ref rect);
		}
		return graphics2;
	}

	internal static Graphics FromXDrawable(IntPtr drawable, IntPtr display)
	{
		IntPtr graphics;
		Status status = GDIPlus.GdipCreateFromXDrawable_linux(drawable, display, out graphics);
		GDIPlus.CheckStatus(status);
		return new Graphics(graphics);
	}

	[System.MonoTODO]
	public static IntPtr GetHalftonePalette()
	{
		throw new NotImplementedException();
	}

	public IntPtr GetHdc()
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetDC(nativeObject, out deviceContextHdc));
		return deviceContextHdc;
	}

	public Color GetNearestColor(Color color)
	{
		int argb;
		Status status = GDIPlus.GdipGetNearestColor(nativeObject, out argb);
		GDIPlus.CheckStatus(status);
		return Color.FromArgb(argb);
	}

	public void IntersectClip(Region region)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		Status status = GDIPlus.GdipSetClipRegion(nativeObject, region.NativeObject, CombineMode.Intersect);
		GDIPlus.CheckStatus(status);
	}

	public void IntersectClip(RectangleF rect)
	{
		Status status = GDIPlus.GdipSetClipRect(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Intersect);
		GDIPlus.CheckStatus(status);
	}

	public void IntersectClip(Rectangle rect)
	{
		Status status = GDIPlus.GdipSetClipRectI(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, CombineMode.Intersect);
		GDIPlus.CheckStatus(status);
	}

	public bool IsVisible(Point point)
	{
		bool result = false;
		Status status = GDIPlus.GdipIsVisiblePointI(nativeObject, point.X, point.Y, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(RectangleF rect)
	{
		bool result = false;
		Status status = GDIPlus.GdipIsVisibleRect(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(PointF point)
	{
		bool result = false;
		Status status = GDIPlus.GdipIsVisiblePoint(nativeObject, point.X, point.Y, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(Rectangle rect)
	{
		bool result = false;
		Status status = GDIPlus.GdipIsVisibleRectI(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, out result);
		GDIPlus.CheckStatus(status);
		return result;
	}

	public bool IsVisible(float x, float y)
	{
		return IsVisible(new PointF(x, y));
	}

	public bool IsVisible(int x, int y)
	{
		return IsVisible(new Point(x, y));
	}

	public bool IsVisible(float x, float y, float width, float height)
	{
		return IsVisible(new RectangleF(x, y, width, height));
	}

	public bool IsVisible(int x, int y, int width, int height)
	{
		return IsVisible(new Rectangle(x, y, width, height));
	}

	public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
	{
		if (text == null || text.Length == 0)
		{
			return new Region[0];
		}
		if (font == null)
		{
			throw new ArgumentNullException("font");
		}
		if (stringFormat == null)
		{
			throw new ArgumentException("stringFormat");
		}
		int measurableCharacterRangeCount = stringFormat.GetMeasurableCharacterRangeCount();
		if (measurableCharacterRangeCount == 0)
		{
			return new Region[0];
		}
		IntPtr[] array = new IntPtr[measurableCharacterRangeCount];
		Region[] array2 = new Region[measurableCharacterRangeCount];
		for (int i = 0; i < measurableCharacterRangeCount; i++)
		{
			array2[i] = new Region();
			ref IntPtr reference = ref array[i];
			reference = array2[i].NativeObject;
		}
		Status status = GDIPlus.GdipMeasureCharacterRanges(nativeObject, text, text.Length, font.NativeObject, ref layoutRect, stringFormat.NativeObject, measurableCharacterRangeCount, out array[0]);
		GDIPlus.CheckStatus(status);
		return array2;
	}

	private unsafe SizeF GdipMeasureString(IntPtr graphics, string text, Font font, ref RectangleF layoutRect, IntPtr stringFormat)
	{
		if (text == null || text.Length == 0)
		{
			return SizeF.Empty;
		}
		if (font == null)
		{
			throw new ArgumentNullException("font");
		}
		RectangleF boundingBox = default(RectangleF);
		Status status = GDIPlus.GdipMeasureString(nativeObject, text, text.Length, font.NativeObject, ref layoutRect, stringFormat, out boundingBox, null, null);
		GDIPlus.CheckStatus(status);
		return new SizeF(boundingBox.Width, boundingBox.Height);
	}

	public SizeF MeasureString(string text, Font font)
	{
		return MeasureString(text, font, SizeF.Empty);
	}

	public SizeF MeasureString(string text, Font font, SizeF layoutArea)
	{
		RectangleF layoutRect = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
		return GdipMeasureString(nativeObject, text, font, ref layoutRect, IntPtr.Zero);
	}

	public SizeF MeasureString(string text, Font font, int width)
	{
		RectangleF layoutRect = new RectangleF(0f, 0f, width, 2.1474836E+09f);
		return GdipMeasureString(nativeObject, text, font, ref layoutRect, IntPtr.Zero);
	}

	public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
	{
		RectangleF layoutRect = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
		IntPtr stringFormat2 = stringFormat?.NativeObject ?? IntPtr.Zero;
		return GdipMeasureString(nativeObject, text, font, ref layoutRect, stringFormat2);
	}

	public SizeF MeasureString(string text, Font font, int width, StringFormat format)
	{
		RectangleF layoutRect = new RectangleF(0f, 0f, width, 2.1474836E+09f);
		IntPtr stringFormat = format?.NativeObject ?? IntPtr.Zero;
		return GdipMeasureString(nativeObject, text, font, ref layoutRect, stringFormat);
	}

	public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
	{
		RectangleF layoutRect = new RectangleF(origin.X, origin.Y, 0f, 0f);
		IntPtr stringFormat2 = stringFormat?.NativeObject ?? IntPtr.Zero;
		return GdipMeasureString(nativeObject, text, font, ref layoutRect, stringFormat2);
	}

	public unsafe SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
	{
		charactersFitted = 0;
		linesFilled = 0;
		if (text == null || text.Length == 0)
		{
			return SizeF.Empty;
		}
		if (font == null)
		{
			throw new ArgumentNullException("font");
		}
		RectangleF boundingBox = default(RectangleF);
		RectangleF layoutRect = new RectangleF(0f, 0f, layoutArea.Width, layoutArea.Height);
		IntPtr stringFormat2 = stringFormat?.NativeObject ?? IntPtr.Zero;
		fixed (int* codepointsFitted = &System.Runtime.CompilerServices.Unsafe.AsRef<int>((int*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref charactersFitted)))
		{
			fixed (int* linesFilled2 = &System.Runtime.CompilerServices.Unsafe.AsRef<int>((int*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref linesFilled)))
			{
				Status status = GDIPlus.GdipMeasureString(nativeObject, text, text.Length, font.NativeObject, ref layoutRect, stringFormat2, out boundingBox, codepointsFitted, linesFilled2);
				GDIPlus.CheckStatus(status);
			}
		}
		linesFilled2 = null;
		return new SizeF(boundingBox.Width, boundingBox.Height);
	}

	public void MultiplyTransform(Matrix matrix)
	{
		MultiplyTransform(matrix, MatrixOrder.Prepend);
	}

	public void MultiplyTransform(Matrix matrix, MatrixOrder order)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		Status status = GDIPlus.GdipMultiplyWorldTransform(nativeObject, matrix.nativeMatrix, order);
		GDIPlus.CheckStatus(status);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public void ReleaseHdc(IntPtr hdc)
	{
		ReleaseHdcInternal(hdc);
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public void ReleaseHdc()
	{
		ReleaseHdcInternal(deviceContextHdc);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[System.MonoLimitation("Can only be used when hdc was provided by Graphics.GetHdc() method")]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public void ReleaseHdcInternal(IntPtr hdc)
	{
		Status status = Status.InvalidParameter;
		if (hdc == deviceContextHdc)
		{
			status = GDIPlus.GdipReleaseDC(nativeObject, deviceContextHdc);
			deviceContextHdc = IntPtr.Zero;
		}
		GDIPlus.CheckStatus(status);
	}

	public void ResetClip()
	{
		Status status = GDIPlus.GdipResetClip(nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public void ResetTransform()
	{
		Status status = GDIPlus.GdipResetWorldTransform(nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public void Restore(GraphicsState gstate)
	{
		Status status = GDIPlus.GdipRestoreGraphics(nativeObject, gstate.nativeState);
		GDIPlus.CheckStatus(status);
	}

	public void RotateTransform(float angle)
	{
		RotateTransform(angle, MatrixOrder.Prepend);
	}

	public void RotateTransform(float angle, MatrixOrder order)
	{
		Status status = GDIPlus.GdipRotateWorldTransform(nativeObject, angle, order);
		GDIPlus.CheckStatus(status);
	}

	public GraphicsState Save()
	{
		uint state;
		Status status = GDIPlus.GdipSaveGraphics(nativeObject, out state);
		GDIPlus.CheckStatus(status);
		GraphicsState graphicsState = new GraphicsState();
		graphicsState.nativeState = state;
		return graphicsState;
	}

	public void ScaleTransform(float sx, float sy)
	{
		ScaleTransform(sx, sy, MatrixOrder.Prepend);
	}

	public void ScaleTransform(float sx, float sy, MatrixOrder order)
	{
		Status status = GDIPlus.GdipScaleWorldTransform(nativeObject, sx, sy, order);
		GDIPlus.CheckStatus(status);
	}

	public void SetClip(RectangleF rect)
	{
		SetClip(rect, CombineMode.Replace);
	}

	public void SetClip(GraphicsPath path)
	{
		SetClip(path, CombineMode.Replace);
	}

	public void SetClip(Rectangle rect)
	{
		SetClip(rect, CombineMode.Replace);
	}

	public void SetClip(Graphics g)
	{
		SetClip(g, CombineMode.Replace);
	}

	public void SetClip(Graphics g, CombineMode combineMode)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		Status status = GDIPlus.GdipSetClipGraphics(nativeObject, g.NativeObject, combineMode);
		GDIPlus.CheckStatus(status);
	}

	public void SetClip(Rectangle rect, CombineMode combineMode)
	{
		Status status = GDIPlus.GdipSetClipRectI(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, combineMode);
		GDIPlus.CheckStatus(status);
	}

	public void SetClip(RectangleF rect, CombineMode combineMode)
	{
		Status status = GDIPlus.GdipSetClipRect(nativeObject, rect.X, rect.Y, rect.Width, rect.Height, combineMode);
		GDIPlus.CheckStatus(status);
	}

	public void SetClip(Region region, CombineMode combineMode)
	{
		if (region == null)
		{
			throw new ArgumentNullException("region");
		}
		Status status = GDIPlus.GdipSetClipRegion(nativeObject, region.NativeObject, combineMode);
		GDIPlus.CheckStatus(status);
	}

	public void SetClip(GraphicsPath path, CombineMode combineMode)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		Status status = GDIPlus.GdipSetClipPath(nativeObject, path.NativeObject, combineMode);
		GDIPlus.CheckStatus(status);
	}

	public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		IntPtr intPtr = GDIPlus.FromPointToUnManagedMemory(pts);
		Status status = GDIPlus.GdipTransformPoints(nativeObject, destSpace, srcSpace, intPtr, pts.Length);
		GDIPlus.CheckStatus(status);
		GDIPlus.FromUnManagedMemoryToPoint(intPtr, pts);
	}

	public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		IntPtr intPtr = GDIPlus.FromPointToUnManagedMemoryI(pts);
		Status status = GDIPlus.GdipTransformPointsI(nativeObject, destSpace, srcSpace, intPtr, pts.Length);
		GDIPlus.CheckStatus(status);
		GDIPlus.FromUnManagedMemoryToPointI(intPtr, pts);
	}

	public void TranslateClip(int dx, int dy)
	{
		Status status = GDIPlus.GdipTranslateClipI(nativeObject, dx, dy);
		GDIPlus.CheckStatus(status);
	}

	public void TranslateClip(float dx, float dy)
	{
		Status status = GDIPlus.GdipTranslateClip(nativeObject, dx, dy);
		GDIPlus.CheckStatus(status);
	}

	public void TranslateTransform(float dx, float dy)
	{
		TranslateTransform(dx, dy, MatrixOrder.Prepend);
	}

	public void TranslateTransform(float dx, float dy, MatrixOrder order)
	{
		Status status = GDIPlus.GdipTranslateWorldTransform(nativeObject, dx, dy, order);
		GDIPlus.CheckStatus(status);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[System.MonoTODO]
	public object GetContextInfo()
	{
		throw new NotImplementedException();
	}
}
