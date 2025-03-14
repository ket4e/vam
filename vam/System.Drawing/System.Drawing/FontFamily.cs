using System.Drawing.Text;
using System.Text;

namespace System.Drawing;

public sealed class FontFamily : MarshalByRefObject, IDisposable
{
	private string name;

	private IntPtr nativeFontFamily = IntPtr.Zero;

	internal IntPtr NativeObject => nativeFontFamily;

	public string Name
	{
		get
		{
			if (nativeFontFamily == IntPtr.Zero)
			{
				throw new ArgumentException("Name", global::Locale.GetText("Object was disposed."));
			}
			if (name == null)
			{
				refreshName();
			}
			return name;
		}
	}

	public static FontFamily GenericMonospace => new FontFamily(GenericFontFamilies.Monospace);

	public static FontFamily GenericSansSerif => new FontFamily(GenericFontFamilies.SansSerif);

	public static FontFamily GenericSerif => new FontFamily(GenericFontFamilies.Serif);

	public static FontFamily[] Families => new InstalledFontCollection().Families;

	internal FontFamily(IntPtr fntfamily)
	{
		nativeFontFamily = fntfamily;
	}

	public FontFamily(GenericFontFamilies genericFamily)
	{
		GDIPlus.CheckStatus(genericFamily switch
		{
			GenericFontFamilies.SansSerif => GDIPlus.GdipGetGenericFontFamilySansSerif(out nativeFontFamily), 
			GenericFontFamilies.Serif => GDIPlus.GdipGetGenericFontFamilySerif(out nativeFontFamily), 
			_ => GDIPlus.GdipGetGenericFontFamilyMonospace(out nativeFontFamily), 
		});
	}

	public FontFamily(string name)
		: this(name, null)
	{
	}

	public FontFamily(string name, FontCollection fontCollection)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipCreateFontFamilyFromName(name, fontCollection?.nativeFontCollection ?? IntPtr.Zero, out nativeFontFamily));
	}

	internal void refreshName()
	{
		if (!(nativeFontFamily == IntPtr.Zero))
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			Status status = GDIPlus.GdipGetFamilyName(nativeFontFamily, stringBuilder, 0);
			GDIPlus.CheckStatus(status);
			name = stringBuilder.ToString();
		}
	}

	~FontFamily()
	{
		Dispose();
	}

	public int GetCellAscent(FontStyle style)
	{
		short ascent;
		Status status = GDIPlus.GdipGetCellAscent(nativeFontFamily, (int)style, out ascent);
		GDIPlus.CheckStatus(status);
		return ascent;
	}

	public int GetCellDescent(FontStyle style)
	{
		short descent;
		Status status = GDIPlus.GdipGetCellDescent(nativeFontFamily, (int)style, out descent);
		GDIPlus.CheckStatus(status);
		return descent;
	}

	public int GetEmHeight(FontStyle style)
	{
		short emHeight;
		Status status = GDIPlus.GdipGetEmHeight(nativeFontFamily, (int)style, out emHeight);
		GDIPlus.CheckStatus(status);
		return emHeight;
	}

	public int GetLineSpacing(FontStyle style)
	{
		short spacing;
		Status status = GDIPlus.GdipGetLineSpacing(nativeFontFamily, (int)style, out spacing);
		GDIPlus.CheckStatus(status);
		return spacing;
	}

	[System.MonoDocumentationNote("When used with libgdiplus this method always return true (styles are created on demand).")]
	public bool IsStyleAvailable(FontStyle style)
	{
		bool styleAvailable;
		Status status = GDIPlus.GdipIsStyleAvailable(nativeFontFamily, (int)style, out styleAvailable);
		GDIPlus.CheckStatus(status);
		return styleAvailable;
	}

	public void Dispose()
	{
		if (nativeFontFamily != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeleteFontFamily(nativeFontFamily);
			nativeFontFamily = IntPtr.Zero;
			GC.SuppressFinalize(this);
			GDIPlus.CheckStatus(status);
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is FontFamily fontFamily))
		{
			return false;
		}
		return Name == fontFamily.Name;
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}

	public static FontFamily[] GetFamilies(Graphics graphics)
	{
		if (graphics == null)
		{
			throw new ArgumentNullException("graphics");
		}
		InstalledFontCollection installedFontCollection = new InstalledFontCollection();
		return installedFontCollection.Families;
	}

	[System.MonoLimitation("The language parameter is ignored. We always return the name using the default system language.")]
	public string GetName(int language)
	{
		return Name;
	}

	public override string ToString()
	{
		return "[FontFamily: Name=" + Name + "]";
	}
}
