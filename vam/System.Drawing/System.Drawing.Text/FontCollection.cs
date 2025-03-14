namespace System.Drawing.Text;

public abstract class FontCollection : IDisposable
{
	internal IntPtr nativeFontCollection = IntPtr.Zero;

	public FontFamily[] Families
	{
		get
		{
			int retCount = 0;
			if (nativeFontCollection == IntPtr.Zero)
			{
				throw new ArgumentException(global::Locale.GetText("Collection was disposed."));
			}
			Status status = GDIPlus.GdipGetFontCollectionFamilyCount(nativeFontCollection, out var found);
			GDIPlus.CheckStatus(status);
			if (found == 0)
			{
				return new FontFamily[0];
			}
			IntPtr[] array = new IntPtr[found];
			status = GDIPlus.GdipGetFontCollectionFamilyList(nativeFontCollection, found, array, out retCount);
			FontFamily[] array2 = new FontFamily[retCount];
			for (int i = 0; i < retCount; i++)
			{
				array2[i] = new FontFamily(array[i]);
			}
			return array2;
		}
	}

	internal FontCollection()
	{
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(true);
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	~FontCollection()
	{
		Dispose(disposing: false);
	}
}
