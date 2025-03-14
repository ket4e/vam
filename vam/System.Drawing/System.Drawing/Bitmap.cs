using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Drawing;

[Serializable]
[ComVisible(true)]
[Editor("System.Drawing.Design.BitmapEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
public sealed class Bitmap : Image
{
	private Bitmap()
	{
	}

	internal Bitmap(IntPtr ptr)
	{
		nativeObject = ptr;
	}

	internal Bitmap(IntPtr ptr, Stream stream)
	{
		if (GDIPlus.RunningOnWindows())
		{
			base.stream = stream;
		}
		nativeObject = ptr;
	}

	public Bitmap(int width, int height)
		: this(width, height, PixelFormat.Format32bppArgb)
	{
	}

	public Bitmap(int width, int height, Graphics g)
	{
		if (g == null)
		{
			throw new ArgumentNullException("g");
		}
		IntPtr bitmap;
		Status status = GDIPlus.GdipCreateBitmapFromGraphics(width, height, g.nativeObject, out bitmap);
		GDIPlus.CheckStatus(status);
		nativeObject = bitmap;
	}

	public Bitmap(int width, int height, PixelFormat format)
	{
		IntPtr bmp;
		Status status = GDIPlus.GdipCreateBitmapFromScan0(width, height, 0, format, IntPtr.Zero, out bmp);
		GDIPlus.CheckStatus(status);
		nativeObject = bmp;
	}

	public Bitmap(Image original)
		: this(original, original.Width, original.Height)
	{
	}

	public Bitmap(Stream stream)
		: this(stream, useIcm: false)
	{
	}

	public Bitmap(string filename)
		: this(filename, useIcm: false)
	{
	}

	public Bitmap(Image original, Size newSize)
		: this(original, newSize.Width, newSize.Height)
	{
	}

	public Bitmap(Stream stream, bool useIcm)
	{
		nativeObject = Image.InitFromStream(stream);
	}

	public Bitmap(string filename, bool useIcm)
	{
		if (filename == null)
		{
			throw new ArgumentNullException("filename");
		}
		GDIPlus.CheckStatus((!useIcm) ? GDIPlus.GdipCreateBitmapFromFile(filename, out var bitmap) : GDIPlus.GdipCreateBitmapFromFileICM(filename, out bitmap));
		nativeObject = bitmap;
	}

	public Bitmap(Type type, string resource)
	{
		if (resource == null)
		{
			throw new ArgumentException("resource");
		}
		Stream manifestResourceStream = type.Assembly.GetManifestResourceStream(type, resource);
		if (manifestResourceStream == null)
		{
			string text = global::Locale.GetText("Resource '{0}' was not found.", resource);
			throw new FileNotFoundException(text);
		}
		nativeObject = Image.InitFromStream(manifestResourceStream);
		if (GDIPlus.RunningOnWindows())
		{
			stream = manifestResourceStream;
		}
	}

	public Bitmap(Image original, int width, int height)
		: this(width, height, PixelFormat.Format32bppArgb)
	{
		Graphics graphics = Graphics.FromImage(this);
		graphics.DrawImage(original, 0, 0, width, height);
		graphics.Dispose();
	}

	public Bitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
	{
		IntPtr bmp;
		Status status = GDIPlus.GdipCreateBitmapFromScan0(width, height, stride, format, scan0, out bmp);
		GDIPlus.CheckStatus(status);
		nativeObject = bmp;
	}

	private Bitmap(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public Color GetPixel(int x, int y)
	{
		int argb;
		Status status = GDIPlus.GdipBitmapGetPixel(nativeObject, x, y, out argb);
		GDIPlus.CheckStatus(status);
		return Color.FromArgb(argb);
	}

	public void SetPixel(int x, int y, Color color)
	{
		Status status = GDIPlus.GdipBitmapSetPixel(nativeObject, x, y, color.ToArgb());
		if (status == Status.InvalidParameter && (base.PixelFormat & PixelFormat.Indexed) != 0)
		{
			string text = global::Locale.GetText("SetPixel cannot be called on indexed bitmaps.");
			throw new InvalidOperationException(text);
		}
		GDIPlus.CheckStatus(status);
	}

	public Bitmap Clone(Rectangle rect, PixelFormat format)
	{
		IntPtr bitmap;
		Status status = GDIPlus.GdipCloneBitmapAreaI(rect.X, rect.Y, rect.Width, rect.Height, format, nativeObject, out bitmap);
		GDIPlus.CheckStatus(status);
		return new Bitmap(bitmap);
	}

	public Bitmap Clone(RectangleF rect, PixelFormat format)
	{
		IntPtr bitmap;
		Status status = GDIPlus.GdipCloneBitmapArea(rect.X, rect.Y, rect.Width, rect.Height, format, nativeObject, out bitmap);
		GDIPlus.CheckStatus(status);
		return new Bitmap(bitmap);
	}

	public static Bitmap FromHicon(IntPtr hicon)
	{
		IntPtr bitmap;
		Status status = GDIPlus.GdipCreateBitmapFromHICON(hicon, out bitmap);
		GDIPlus.CheckStatus(status);
		return new Bitmap(bitmap);
	}

	public static Bitmap FromResource(IntPtr hinstance, string bitmapName)
	{
		IntPtr bitmap;
		Status status = GDIPlus.GdipCreateBitmapFromResource(hinstance, bitmapName, out bitmap);
		GDIPlus.CheckStatus(status);
		return new Bitmap(bitmap);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public IntPtr GetHbitmap()
	{
		return GetHbitmap(Color.Gray);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public IntPtr GetHbitmap(Color background)
	{
		IntPtr HandleBmp;
		Status status = GDIPlus.GdipCreateHBITMAPFromBitmap(nativeObject, out HandleBmp, background.ToArgb());
		GDIPlus.CheckStatus(status);
		return HandleBmp;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public IntPtr GetHicon()
	{
		IntPtr HandleIcon;
		Status status = GDIPlus.GdipCreateHICONFromBitmap(nativeObject, out HandleIcon);
		GDIPlus.CheckStatus(status);
		return HandleIcon;
	}

	public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
	{
		BitmapData bitmapData = new BitmapData();
		return LockBits(rect, flags, format, bitmapData);
	}

	public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
	{
		Status status = GDIPlus.GdipBitmapLockBits(nativeObject, ref rect, flags, format, bitmapData);
		GDIPlus.CheckStatus(status);
		return bitmapData;
	}

	public void MakeTransparent()
	{
		Color pixel = GetPixel(0, 0);
		MakeTransparent(pixel);
	}

	public void MakeTransparent(Color transparentColor)
	{
		Bitmap bitmap = new Bitmap(base.Width, base.Height, PixelFormat.Format32bppArgb);
		Graphics graphics = Graphics.FromImage(bitmap);
		Rectangle destRect = new Rectangle(0, 0, base.Width, base.Height);
		ImageAttributes imageAttributes = new ImageAttributes();
		imageAttributes.SetColorKey(transparentColor, transparentColor);
		graphics.DrawImage(this, destRect, 0, 0, base.Width, base.Height, GraphicsUnit.Pixel, imageAttributes);
		IntPtr intPtr = nativeObject;
		nativeObject = bitmap.nativeObject;
		bitmap.nativeObject = intPtr;
		graphics.Dispose();
		bitmap.Dispose();
		imageAttributes.Dispose();
	}

	public void SetResolution(float xDpi, float yDpi)
	{
		Status status = GDIPlus.GdipBitmapSetResolution(nativeObject, xDpi, yDpi);
		GDIPlus.CheckStatus(status);
	}

	public void UnlockBits(BitmapData bitmapdata)
	{
		Status status = GDIPlus.GdipBitmapUnlockBits(nativeObject, bitmapdata);
		GDIPlus.CheckStatus(status);
	}
}
