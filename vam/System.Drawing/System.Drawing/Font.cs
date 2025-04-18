using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Drawing;

[Serializable]
[ComVisible(true)]
[Editor("System.Drawing.Design.FontEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
[TypeConverter(typeof(FontConverter))]
public sealed class Font : MarshalByRefObject, IDisposable, ICloneable, ISerializable
{
	private const byte DefaultCharSet = 1;

	private IntPtr fontObject = IntPtr.Zero;

	private string systemFontName;

	private string originalFontName;

	private float _size;

	private object olf;

	private static int CharSetOffset = -1;

	private bool _bold;

	private FontFamily _fontFamily;

	private byte _gdiCharSet;

	private bool _gdiVerticalFont;

	private bool _italic;

	private string _name;

	private float _sizeInPoints;

	private bool _strikeout;

	private FontStyle _style;

	private bool _underline;

	private GraphicsUnit _unit;

	internal IntPtr NativeObject => fontObject;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool Bold => _bold;

	[Browsable(false)]
	public FontFamily FontFamily => _fontFamily;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public byte GdiCharSet => _gdiCharSet;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool GdiVerticalFont => _gdiVerticalFont;

	[Browsable(false)]
	public int Height => (int)Math.Ceiling(GetHeight());

	[Browsable(false)]
	public bool IsSystemFont
	{
		get
		{
			if (systemFontName == null)
			{
				return false;
			}
			return StringComparer.InvariantCulture.Compare(systemFontName, string.Empty) != 0;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool Italic => _italic;

	[Editor("System.Drawing.Design.FontNameEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[TypeConverter(typeof(FontConverter.FontNameConverter))]
	public string Name => _name;

	public float Size => _size;

	[Browsable(false)]
	public float SizeInPoints => _sizeInPoints;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool Strikeout => _strikeout;

	[Browsable(false)]
	public FontStyle Style => _style;

	[Browsable(false)]
	public string SystemFontName => systemFontName;

	[Browsable(false)]
	public string OriginalFontName => originalFontName;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool Underline => _underline;

	[TypeConverter(typeof(FontConverter.FontUnitConverter))]
	public GraphicsUnit Unit => _unit;

	private Font(SerializationInfo info, StreamingContext context)
	{
		string familyName = (string)info.GetValue("Name", typeof(string));
		float emSize = (float)info.GetValue("Size", typeof(float));
		FontStyle style = (FontStyle)(int)info.GetValue("Style", typeof(FontStyle));
		GraphicsUnit unit = (GraphicsUnit)(int)info.GetValue("Unit", typeof(GraphicsUnit));
		CreateFont(familyName, emSize, style, unit, 1, isVertical: false);
	}

	internal Font(IntPtr newFontObject, string familyName, FontStyle style, float size)
	{
		FontFamily family;
		try
		{
			family = new FontFamily(familyName);
		}
		catch (Exception)
		{
			family = FontFamily.GenericSansSerif;
		}
		setProperties(family, size, style, GraphicsUnit.Pixel, 0, isVertical: false);
		fontObject = newFontObject;
	}

	public Font(Font prototype, FontStyle newStyle)
	{
		setProperties(prototype.FontFamily, prototype.Size, newStyle, prototype.Unit, prototype.GdiCharSet, prototype.GdiVerticalFont);
		Status status = GDIPlus.GdipCreateFont(_fontFamily.NativeObject, Size, Style, Unit, out fontObject);
		GDIPlus.CheckStatus(status);
	}

	public Font(FontFamily family, float emSize, GraphicsUnit unit)
		: this(family, emSize, FontStyle.Regular, unit, 1, gdiVerticalFont: false)
	{
	}

	public Font(string familyName, float emSize, GraphicsUnit unit)
		: this(new FontFamily(familyName), emSize, FontStyle.Regular, unit, 1, gdiVerticalFont: false)
	{
	}

	public Font(FontFamily family, float emSize)
		: this(family, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, gdiVerticalFont: false)
	{
	}

	public Font(FontFamily family, float emSize, FontStyle style)
		: this(family, emSize, style, GraphicsUnit.Point, 1, gdiVerticalFont: false)
	{
	}

	public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit)
		: this(family, emSize, style, unit, 1, gdiVerticalFont: false)
	{
	}

	public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
		: this(family, emSize, style, unit, gdiCharSet, gdiVerticalFont: false)
	{
	}

	public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
	{
		if (family == null)
		{
			throw new ArgumentNullException("family");
		}
		setProperties(family, emSize, style, unit, gdiCharSet, gdiVerticalFont);
		Status status = GDIPlus.GdipCreateFont(family.NativeObject, emSize, style, unit, out fontObject);
		GDIPlus.CheckStatus(status);
	}

	public Font(string familyName, float emSize)
		: this(familyName, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, gdiVerticalFont: false)
	{
	}

	public Font(string familyName, float emSize, FontStyle style)
		: this(familyName, emSize, style, GraphicsUnit.Point, 1, gdiVerticalFont: false)
	{
	}

	public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit)
		: this(familyName, emSize, style, unit, 1, gdiVerticalFont: false)
	{
	}

	public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
		: this(familyName, emSize, style, unit, gdiCharSet, gdiVerticalFont: false)
	{
	}

	public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
	{
		CreateFont(familyName, emSize, style, unit, gdiCharSet, gdiVerticalFont);
	}

	internal Font(string familyName, float emSize, string systemName)
		: this(familyName, emSize, FontStyle.Regular, GraphicsUnit.Point, 1, gdiVerticalFont: false)
	{
		systemFontName = systemName;
	}

	void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
	{
		si.AddValue("Name", Name);
		si.AddValue("Size", Size);
		si.AddValue("Style", Style);
		si.AddValue("Unit", Unit);
	}

	private void CreateFont(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte charSet, bool isVertical)
	{
		originalFontName = familyName;
		FontFamily fontFamily;
		try
		{
			fontFamily = new FontFamily(familyName);
		}
		catch (Exception)
		{
			fontFamily = FontFamily.GenericSansSerif;
		}
		setProperties(fontFamily, emSize, style, unit, charSet, isVertical);
		Status status = GDIPlus.GdipCreateFont(fontFamily.NativeObject, emSize, style, unit, out fontObject);
		if (status == Status.FontStyleNotFound)
		{
			throw new ArgumentException(global::Locale.GetText("Style {0} isn't supported by font {1}.", style.ToString(), familyName));
		}
		GDIPlus.CheckStatus(status);
	}

	~Font()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (fontObject != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeleteFont(fontObject);
			fontObject = IntPtr.Zero;
			GC.SuppressFinalize(this);
			GDIPlus.CheckStatus(status);
		}
	}

	internal void unitConversion(GraphicsUnit fromUnit, GraphicsUnit toUnit, float nSrc, out float nTrg)
	{
		float num = 0f;
		nTrg = 0f;
		switch (fromUnit)
		{
		case GraphicsUnit.Display:
			num = nSrc / 75f;
			break;
		case GraphicsUnit.Document:
			num = nSrc / 300f;
			break;
		case GraphicsUnit.Inch:
			num = nSrc;
			break;
		case GraphicsUnit.Millimeter:
			num = nSrc / 25.4f;
			break;
		case GraphicsUnit.World:
		case GraphicsUnit.Pixel:
			num = nSrc / Graphics.systemDpiX;
			break;
		case GraphicsUnit.Point:
			num = nSrc / 72f;
			break;
		default:
			throw new ArgumentException("Invalid GraphicsUnit");
		}
		switch (toUnit)
		{
		case GraphicsUnit.Display:
			nTrg = num * 75f;
			break;
		case GraphicsUnit.Document:
			nTrg = num * 300f;
			break;
		case GraphicsUnit.Inch:
			nTrg = num;
			break;
		case GraphicsUnit.Millimeter:
			nTrg = num * 25.4f;
			break;
		case GraphicsUnit.World:
		case GraphicsUnit.Pixel:
			nTrg = num * Graphics.systemDpiX;
			break;
		case GraphicsUnit.Point:
			nTrg = num * 72f;
			break;
		default:
			throw new ArgumentException("Invalid GraphicsUnit");
		}
	}

	internal void setProperties(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte charSet, bool isVertical)
	{
		_name = family.Name;
		_fontFamily = family;
		_size = emSize;
		_unit = unit;
		_style = style;
		_gdiCharSet = charSet;
		_gdiVerticalFont = isVertical;
		unitConversion(unit, GraphicsUnit.Point, emSize, out _sizeInPoints);
		_bold = (_italic = (_strikeout = (_underline = false)));
		if ((style & FontStyle.Bold) == FontStyle.Bold)
		{
			_bold = true;
		}
		if ((style & FontStyle.Italic) == FontStyle.Italic)
		{
			_italic = true;
		}
		if ((style & FontStyle.Strikeout) == FontStyle.Strikeout)
		{
			_strikeout = true;
		}
		if ((style & FontStyle.Underline) == FontStyle.Underline)
		{
			_underline = true;
		}
	}

	public static Font FromHfont(IntPtr hfont)
	{
		FontStyle fontStyle = FontStyle.Regular;
		LOGFONT lf = default(LOGFONT);
		if (hfont == IntPtr.Zero)
		{
			return new Font("Arial", 10f, FontStyle.Regular);
		}
		if (GDIPlus.RunningOnUnix())
		{
			IntPtr font;
			Status status = GDIPlus.GdipCreateFontFromHfont(hfont, out font, ref lf);
			GDIPlus.CheckStatus(status);
			if (lf.lfItalic != 0)
			{
				fontStyle |= FontStyle.Italic;
			}
			if (lf.lfUnderline != 0)
			{
				fontStyle |= FontStyle.Underline;
			}
			if (lf.lfStrikeOut != 0)
			{
				fontStyle |= FontStyle.Strikeout;
			}
			if (lf.lfWeight > 400)
			{
				fontStyle |= FontStyle.Bold;
			}
			return new Font(size: (lf.lfHeight >= 0) ? ((float)lf.lfHeight) : ((float)(lf.lfHeight * -1)), newFontObject: font, familyName: lf.lfFaceName, style: fontStyle);
		}
		fontStyle = FontStyle.Regular;
		IntPtr dC = GDIPlus.GetDC(IntPtr.Zero);
		try
		{
			return FromLogFont(lf, dC);
		}
		finally
		{
			GDIPlus.ReleaseDC(IntPtr.Zero, dC);
		}
	}

	public IntPtr ToHfont()
	{
		if (fontObject == IntPtr.Zero)
		{
			throw new ArgumentException(global::Locale.GetText("Object has been disposed."));
		}
		if (GDIPlus.RunningOnUnix())
		{
			return fontObject;
		}
		if (olf == null)
		{
			olf = default(LOGFONT);
			ToLogFont(olf);
		}
		LOGFONT logfont = (LOGFONT)olf;
		return GDIPlus.CreateFontIndirect(ref logfont);
	}

	public object Clone()
	{
		return new Font(this, Style);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Font font))
		{
			return false;
		}
		if (font.FontFamily.Equals(FontFamily) && font.Size == Size && font.Style == Style && font.Unit == Unit && font.GdiCharSet == GdiCharSet && font.GdiVerticalFont == GdiVerticalFont)
		{
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _name.GetHashCode() ^ FontFamily.GetHashCode() ^ _size.GetHashCode() ^ _style.GetHashCode() ^ _gdiCharSet ^ _gdiVerticalFont.GetHashCode();
	}

	[System.MonoTODO("The hdc parameter has no direct equivalent in libgdiplus.")]
	public static Font FromHdc(IntPtr hdc)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("The returned font may not have all it's properties initialized correctly.")]
	public static Font FromLogFont(object lf, IntPtr hdc)
	{
		LOGFONT lf2 = (LOGFONT)lf;
		IntPtr ptr;
		Status status = GDIPlus.GdipCreateFontFromLogfont(hdc, ref lf2, out ptr);
		GDIPlus.CheckStatus(status);
		return new Font(ptr, "Microsoft Sans Serif", FontStyle.Regular, 10f);
	}

	public float GetHeight()
	{
		return GetHeight(Graphics.systemDpiY);
	}

	public static Font FromLogFont(object lf)
	{
		if (GDIPlus.RunningOnUnix())
		{
			return FromLogFont(lf, IntPtr.Zero);
		}
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = GDIPlus.GetDC(IntPtr.Zero);
			return FromLogFont(lf, intPtr);
		}
		finally
		{
			GDIPlus.ReleaseDC(IntPtr.Zero, intPtr);
		}
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public void ToLogFont(object logFont)
	{
		if (GDIPlus.RunningOnUnix())
		{
			using (Bitmap image = new Bitmap(1, 1, PixelFormat.Format32bppArgb))
			{
				using Graphics graphics = Graphics.FromImage(image);
				ToLogFont(logFont, graphics);
				return;
			}
		}
		IntPtr dC = GDIPlus.GetDC(IntPtr.Zero);
		try
		{
			using Graphics graphics2 = Graphics.FromHdc(dC);
			ToLogFont(logFont, graphics2);
		}
		finally
		{
			GDIPlus.ReleaseDC(IntPtr.Zero, dC);
		}
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public void ToLogFont(object logFont, Graphics graphics)
	{
		if (graphics == null)
		{
			throw new ArgumentNullException("graphics");
		}
		if (logFont == null)
		{
			throw new AccessViolationException("logFont");
		}
		Type type = logFont.GetType();
		if (!type.IsLayoutSequential)
		{
			throw new ArgumentException("logFont", global::Locale.GetText("Layout must be sequential."));
		}
		Type typeFromHandle = typeof(LOGFONT);
		int num = Marshal.SizeOf(logFont);
		if (num < Marshal.SizeOf(typeFromHandle))
		{
			return;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Status status;
		try
		{
			Marshal.StructureToPtr(logFont, intPtr, fDeleteOld: false);
			status = GDIPlus.GdipGetLogFont(NativeObject, graphics.NativeObject, logFont);
			if (status != 0)
			{
				Marshal.PtrToStructure(intPtr, logFont);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		if (CharSetOffset == -1)
		{
			CharSetOffset = (int)Marshal.OffsetOf(typeFromHandle, "lfCharSet");
		}
		GCHandle gCHandle = GCHandle.Alloc(logFont, GCHandleType.Pinned);
		try
		{
			IntPtr ptr = gCHandle.AddrOfPinnedObject();
			if (Marshal.ReadByte(ptr, CharSetOffset) == 0)
			{
				Marshal.WriteByte(ptr, CharSetOffset, 1);
			}
		}
		finally
		{
			gCHandle.Free();
		}
		GDIPlus.CheckStatus(status);
	}

	public float GetHeight(Graphics graphics)
	{
		if (graphics == null)
		{
			throw new ArgumentNullException("graphics");
		}
		float height;
		Status status = GDIPlus.GdipGetFontHeight(fontObject, graphics.NativeObject, out height);
		GDIPlus.CheckStatus(status);
		return height;
	}

	public float GetHeight(float dpi)
	{
		float height;
		Status status = GDIPlus.GdipGetFontHeightGivenDPI(fontObject, dpi, out height);
		GDIPlus.CheckStatus(status);
		return height;
	}

	public override string ToString()
	{
		return $"[Font: Name={_name}, Size={Size}, Units={(int)_unit}, GdiCharSet={_gdiCharSet}, GdiVerticalFont={_gdiVerticalFont}]";
	}
}
