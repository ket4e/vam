using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing;

[Serializable]
[TypeConverter(typeof(ImageConverter))]
[ComVisible(true)]
[Editor("System.Drawing.Design.ImageEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
[ImmutableObject(true)]
public abstract class Image : MarshalByRefObject, IDisposable, ICloneable, ISerializable
{
	public delegate bool GetThumbnailImageAbort();

	private object tag;

	internal IntPtr nativeObject = IntPtr.Zero;

	internal Stream stream;

	[Browsable(false)]
	public int Flags
	{
		get
		{
			int flag;
			Status status = GDIPlus.GdipGetImageFlags(nativeObject, out flag);
			GDIPlus.CheckStatus(status);
			return flag;
		}
	}

	[Browsable(false)]
	public Guid[] FrameDimensionsList
	{
		get
		{
			Status status = GDIPlus.GdipImageGetFrameDimensionsCount(nativeObject, out var count);
			GDIPlus.CheckStatus(status);
			Guid[] array = new Guid[count];
			status = GDIPlus.GdipImageGetFrameDimensionsList(nativeObject, array, count);
			GDIPlus.CheckStatus(status);
			return array;
		}
	}

	[Browsable(false)]
	[DefaultValue(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int Height
	{
		get
		{
			uint height;
			Status status = GDIPlus.GdipGetImageHeight(nativeObject, out height);
			GDIPlus.CheckStatus(status);
			return (int)height;
		}
	}

	public float HorizontalResolution
	{
		get
		{
			float resolution;
			Status status = GDIPlus.GdipGetImageHorizontalResolution(nativeObject, out resolution);
			GDIPlus.CheckStatus(status);
			return resolution;
		}
	}

	[Browsable(false)]
	public ColorPalette Palette
	{
		get
		{
			return retrieveGDIPalette();
		}
		set
		{
			storeGDIPalette(value);
		}
	}

	public SizeF PhysicalDimension
	{
		get
		{
			float width;
			float height;
			Status status = GDIPlus.GdipGetImageDimension(nativeObject, out width, out height);
			GDIPlus.CheckStatus(status);
			return new SizeF(width, height);
		}
	}

	public PixelFormat PixelFormat
	{
		get
		{
			PixelFormat format;
			Status status = GDIPlus.GdipGetImagePixelFormat(nativeObject, out format);
			GDIPlus.CheckStatus(status);
			return format;
		}
	}

	[Browsable(false)]
	public int[] PropertyIdList
	{
		get
		{
			Status status = GDIPlus.GdipGetPropertyCount(nativeObject, out var propNumbers);
			GDIPlus.CheckStatus(status);
			int[] array = new int[propNumbers];
			status = GDIPlus.GdipGetPropertyIdList(nativeObject, propNumbers, array);
			GDIPlus.CheckStatus(status);
			return array;
		}
	}

	[Browsable(false)]
	public PropertyItem[] PropertyItems
	{
		get
		{
			GdipPropertyItem gdipPropertyItem = default(GdipPropertyItem);
			Status status = GDIPlus.GdipGetPropertySize(nativeObject, out var bufferSize, out var propNumbers);
			GDIPlus.CheckStatus(status);
			PropertyItem[] array = new PropertyItem[propNumbers];
			if (propNumbers == 0)
			{
				return array;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(bufferSize * propNumbers);
			try
			{
				status = GDIPlus.GdipGetAllPropertyItems(nativeObject, bufferSize, propNumbers, intPtr);
				GDIPlus.CheckStatus(status);
				int num = Marshal.SizeOf(gdipPropertyItem);
				IntPtr ptr = intPtr;
				int num2 = 0;
				while (num2 < propNumbers)
				{
					gdipPropertyItem = (GdipPropertyItem)Marshal.PtrToStructure(ptr, typeof(GdipPropertyItem));
					array[num2] = new PropertyItem();
					GdipPropertyItem.MarshalTo(gdipPropertyItem, array[num2]);
					num2++;
					ptr = new IntPtr(ptr.ToInt64() + num);
				}
				return array;
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public ImageFormat RawFormat
	{
		get
		{
			Guid format;
			Status status = GDIPlus.GdipGetImageRawFormat(nativeObject, out format);
			GDIPlus.CheckStatus(status);
			return new ImageFormat(format);
		}
	}

	public Size Size => new Size(Width, Height);

	[TypeConverter(typeof(StringConverter))]
	[Bindable(true)]
	[DefaultValue(null)]
	[Localizable(false)]
	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	public float VerticalResolution
	{
		get
		{
			float resolution;
			Status status = GDIPlus.GdipGetImageVerticalResolution(nativeObject, out resolution);
			GDIPlus.CheckStatus(status);
			return resolution;
		}
	}

	[DefaultValue(false)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int Width
	{
		get
		{
			uint width;
			Status status = GDIPlus.GdipGetImageWidth(nativeObject, out width);
			GDIPlus.CheckStatus(status);
			return (int)width;
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

	internal Image()
	{
	}

	internal Image(SerializationInfo info, StreamingContext context)
	{
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SerializationEntry current = enumerator.Current;
			if (string.Compare(current.Name, "Data", ignoreCase: true) != 0)
			{
				continue;
			}
			byte[] array = (byte[])current.Value;
			if (array != null)
			{
				MemoryStream memoryStream = new MemoryStream(array);
				nativeObject = InitFromStream(memoryStream);
				if (GDIPlus.RunningOnWindows())
				{
					stream = memoryStream;
				}
			}
		}
	}

	void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
	{
		using MemoryStream memoryStream = new MemoryStream();
		if (RawFormat.Equals(ImageFormat.Icon))
		{
			Save(memoryStream, ImageFormat.Png);
		}
		else
		{
			Save(memoryStream, RawFormat);
		}
		si.AddValue("Data", memoryStream.ToArray());
	}

	public static Image FromFile(string filename)
	{
		return FromFile(filename, useEmbeddedColorManagement: false);
	}

	public static Image FromFile(string filename, bool useEmbeddedColorManagement)
	{
		if (!File.Exists(filename))
		{
			throw new FileNotFoundException(filename);
		}
		IntPtr image;
		Status status = ((!useEmbeddedColorManagement) ? GDIPlus.GdipLoadImageFromFile(filename, out image) : GDIPlus.GdipLoadImageFromFileICM(filename, out image));
		GDIPlus.CheckStatus(status);
		return CreateFromHandle(image);
	}

	public static Bitmap FromHbitmap(IntPtr hbitmap)
	{
		return FromHbitmap(hbitmap, IntPtr.Zero);
	}

	public static Bitmap FromHbitmap(IntPtr hbitmap, IntPtr hpalette)
	{
		IntPtr image;
		Status status = GDIPlus.GdipCreateBitmapFromHBITMAP(hbitmap, hpalette, out image);
		GDIPlus.CheckStatus(status);
		return new Bitmap(image);
	}

	public static Image FromStream(Stream stream)
	{
		return LoadFromStream(stream, keepAlive: false);
	}

	[System.MonoLimitation("useEmbeddedColorManagement  isn't supported.")]
	public static Image FromStream(Stream stream, bool useEmbeddedColorManagement)
	{
		return LoadFromStream(stream, keepAlive: false);
	}

	[System.MonoLimitation("useEmbeddedColorManagement  and validateImageData aren't supported.")]
	public static Image FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
	{
		return LoadFromStream(stream, keepAlive: false);
	}

	internal static Image LoadFromStream(Stream stream, bool keepAlive)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		Image image = CreateFromHandle(InitFromStream(stream));
		if (keepAlive && GDIPlus.RunningOnWindows())
		{
			image.stream = stream;
		}
		return image;
	}

	internal static Image CreateFromHandle(IntPtr handle)
	{
		GDIPlus.CheckStatus(GDIPlus.GdipGetImageType(handle, out var type));
		return type switch
		{
			ImageType.Bitmap => new Bitmap(handle), 
			ImageType.Metafile => new Metafile(handle), 
			_ => throw new NotSupportedException(global::Locale.GetText("Unknown image type.")), 
		};
	}

	public static int GetPixelFormatSize(PixelFormat pixfmt)
	{
		int result = 0;
		switch (pixfmt)
		{
		case PixelFormat.Format16bppRgb555:
		case PixelFormat.Format16bppRgb565:
		case PixelFormat.Format16bppArgb1555:
		case PixelFormat.Format16bppGrayScale:
			result = 16;
			break;
		case PixelFormat.Format1bppIndexed:
			result = 1;
			break;
		case PixelFormat.Format24bppRgb:
			result = 24;
			break;
		case PixelFormat.Format32bppRgb:
		case PixelFormat.Format32bppPArgb:
		case PixelFormat.Format32bppArgb:
			result = 32;
			break;
		case PixelFormat.Format48bppRgb:
			result = 48;
			break;
		case PixelFormat.Format4bppIndexed:
			result = 4;
			break;
		case PixelFormat.Format64bppPArgb:
		case PixelFormat.Format64bppArgb:
			result = 64;
			break;
		case PixelFormat.Format8bppIndexed:
			result = 8;
			break;
		}
		return result;
	}

	public static bool IsAlphaPixelFormat(PixelFormat pixfmt)
	{
		bool result = false;
		switch (pixfmt)
		{
		case PixelFormat.Format16bppArgb1555:
		case PixelFormat.Format32bppPArgb:
		case PixelFormat.Format64bppPArgb:
		case PixelFormat.Format32bppArgb:
		case PixelFormat.Format64bppArgb:
			result = true;
			break;
		case PixelFormat.Format16bppRgb555:
		case PixelFormat.Format16bppRgb565:
		case PixelFormat.Format24bppRgb:
		case PixelFormat.Format32bppRgb:
		case PixelFormat.Format1bppIndexed:
		case PixelFormat.Format4bppIndexed:
		case PixelFormat.Format8bppIndexed:
		case PixelFormat.Format16bppGrayScale:
		case PixelFormat.Format48bppRgb:
			result = false;
			break;
		}
		return result;
	}

	public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
	{
		return (pixfmt & PixelFormat.Canonical) != 0;
	}

	public static bool IsExtendedPixelFormat(PixelFormat pixfmt)
	{
		return (pixfmt & PixelFormat.Extended) != 0;
	}

	internal static IntPtr InitFromStream(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentException("stream");
		}
		if (!stream.CanSeek)
		{
			byte[] array = new byte[256];
			int num = 0;
			int num2;
			do
			{
				if (array.Length < num + 256)
				{
					byte[] array2 = new byte[array.Length * 2];
					Array.Copy(array, array2, array.Length);
					array = array2;
				}
				num2 = stream.Read(array, num, 256);
				num += num2;
			}
			while (num2 != 0);
			stream = new MemoryStream(array, 0, num);
		}
		Status status;
		IntPtr image;
		if (GDIPlus.RunningOnUnix())
		{
			GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, seekToOrigin: true);
			status = GDIPlus.GdipLoadImageFromDelegate_linux(gdiPlusStreamHelper.GetHeaderDelegate, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, out image);
		}
		else
		{
			status = GDIPlus.GdipLoadImageFromStream(new ComIStreamWrapper(stream), out image);
		}
		GDIPlus.CheckStatus(status);
		return image;
	}

	public RectangleF GetBounds(ref GraphicsUnit pageUnit)
	{
		RectangleF source;
		Status status = GDIPlus.GdipGetImageBounds(nativeObject, out source, ref pageUnit);
		GDIPlus.CheckStatus(status);
		return source;
	}

	public EncoderParameters GetEncoderParameterList(Guid encoder)
	{
		Status status = GDIPlus.GdipGetEncoderParameterListSize(nativeObject, ref encoder, out var size);
		GDIPlus.CheckStatus(status);
		IntPtr intPtr = Marshal.AllocHGlobal((int)size);
		try
		{
			status = GDIPlus.GdipGetEncoderParameterList(nativeObject, ref encoder, size, intPtr);
			EncoderParameters result = EncoderParameters.FromNativePtr(intPtr);
			GDIPlus.CheckStatus(status);
			return result;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public int GetFrameCount(FrameDimension dimension)
	{
		Guid guidDimension = dimension.Guid;
		uint count;
		Status status = GDIPlus.GdipImageGetFrameCount(nativeObject, ref guidDimension, out count);
		GDIPlus.CheckStatus(status);
		return (int)count;
	}

	public PropertyItem GetPropertyItem(int propid)
	{
		PropertyItem propertyItem = new PropertyItem();
		GdipPropertyItem gdipPropertyItem = default(GdipPropertyItem);
		Status status = GDIPlus.GdipGetPropertyItemSize(nativeObject, propid, out var propertySize);
		GDIPlus.CheckStatus(status);
		IntPtr intPtr = Marshal.AllocHGlobal(propertySize);
		try
		{
			status = GDIPlus.GdipGetPropertyItem(nativeObject, propid, propertySize, intPtr);
			GDIPlus.CheckStatus(status);
			gdipPropertyItem = (GdipPropertyItem)Marshal.PtrToStructure(intPtr, typeof(GdipPropertyItem));
			GdipPropertyItem.MarshalTo(gdipPropertyItem, propertyItem);
			return propertyItem;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public Image GetThumbnailImage(int thumbWidth, int thumbHeight, GetThumbnailImageAbort callback, IntPtr callbackData)
	{
		if (thumbWidth <= 0 || thumbHeight <= 0)
		{
			throw new OutOfMemoryException("Invalid thumbnail size");
		}
		Image image = new Bitmap(thumbWidth, thumbHeight);
		using Graphics graphics = Graphics.FromImage(image);
		Status status = GDIPlus.GdipDrawImageRectRectI(graphics.nativeObject, nativeObject, 0, 0, thumbWidth, thumbHeight, 0, 0, Width, Height, GraphicsUnit.Pixel, IntPtr.Zero, null, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
		return image;
	}

	public void RemovePropertyItem(int propid)
	{
		Status status = GDIPlus.GdipRemovePropertyItem(nativeObject, propid);
		GDIPlus.CheckStatus(status);
	}

	public void RotateFlip(RotateFlipType rotateFlipType)
	{
		Status status = GDIPlus.GdipImageRotateFlip(nativeObject, rotateFlipType);
		GDIPlus.CheckStatus(status);
	}

	internal ImageCodecInfo findEncoderForFormat(ImageFormat format)
	{
		ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
		ImageCodecInfo result = null;
		if (format.Guid.Equals(ImageFormat.MemoryBmp.Guid))
		{
			format = ImageFormat.Png;
		}
		for (int i = 0; i < imageEncoders.Length; i++)
		{
			if (imageEncoders[i].FormatID.Equals(format.Guid))
			{
				result = imageEncoders[i];
				break;
			}
		}
		return result;
	}

	public void Save(string filename)
	{
		Save(filename, RawFormat);
	}

	public void Save(string filename, ImageFormat format)
	{
		ImageCodecInfo imageCodecInfo = findEncoderForFormat(format);
		if (imageCodecInfo == null)
		{
			imageCodecInfo = findEncoderForFormat(RawFormat);
			if (imageCodecInfo == null)
			{
				string text = global::Locale.GetText("No codec available for saving format '{0}'.", format.Guid);
				throw new ArgumentException(text, "format");
			}
		}
		Save(filename, imageCodecInfo, null);
	}

	public void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams)
	{
		Guid encoderClsID = encoder.Clsid;
		Status status;
		if (encoderParams == null)
		{
			status = GDIPlus.GdipSaveImageToFile(nativeObject, filename, ref encoderClsID, IntPtr.Zero);
		}
		else
		{
			IntPtr intPtr = encoderParams.ToNativePtr();
			status = GDIPlus.GdipSaveImageToFile(nativeObject, filename, ref encoderClsID, intPtr);
			Marshal.FreeHGlobal(intPtr);
		}
		GDIPlus.CheckStatus(status);
	}

	public void Save(Stream stream, ImageFormat format)
	{
		ImageCodecInfo imageCodecInfo = findEncoderForFormat(format);
		if (imageCodecInfo == null)
		{
			throw new ArgumentException("No codec available for format:" + format.Guid);
		}
		Save(stream, imageCodecInfo, null);
	}

	public void Save(Stream stream, ImageCodecInfo encoder, EncoderParameters encoderParams)
	{
		Guid encoderClsID = encoder.Clsid;
		IntPtr intPtr = encoderParams?.ToNativePtr() ?? IntPtr.Zero;
		Status status;
		try
		{
			if (GDIPlus.RunningOnUnix())
			{
				GDIPlus.GdiPlusStreamHelper gdiPlusStreamHelper = new GDIPlus.GdiPlusStreamHelper(stream, seekToOrigin: false);
				status = GDIPlus.GdipSaveImageToDelegate_linux(nativeObject, gdiPlusStreamHelper.GetBytesDelegate, gdiPlusStreamHelper.PutBytesDelegate, gdiPlusStreamHelper.SeekDelegate, gdiPlusStreamHelper.CloseDelegate, gdiPlusStreamHelper.SizeDelegate, ref encoderClsID, intPtr);
			}
			else
			{
				status = GDIPlus.GdipSaveImageToStream(new HandleRef(this, nativeObject), new ComIStreamWrapper(stream), ref encoderClsID, new HandleRef(encoderParams, intPtr));
			}
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
		GDIPlus.CheckStatus(status);
	}

	public void SaveAdd(EncoderParameters encoderParams)
	{
		IntPtr intPtr = encoderParams.ToNativePtr();
		Status status = GDIPlus.GdipSaveAdd(nativeObject, intPtr);
		Marshal.FreeHGlobal(intPtr);
		GDIPlus.CheckStatus(status);
	}

	public void SaveAdd(Image image, EncoderParameters encoderParams)
	{
		IntPtr intPtr = encoderParams.ToNativePtr();
		Status status = GDIPlus.GdipSaveAddImage(nativeObject, image.NativeObject, intPtr);
		Marshal.FreeHGlobal(intPtr);
		GDIPlus.CheckStatus(status);
	}

	public int SelectActiveFrame(FrameDimension dimension, int frameIndex)
	{
		Guid guidDimension = dimension.Guid;
		Status status = GDIPlus.GdipImageSelectActiveFrame(nativeObject, ref guidDimension, frameIndex);
		GDIPlus.CheckStatus(status);
		return frameIndex;
	}

	public void SetPropertyItem(PropertyItem propitem)
	{
		throw new NotImplementedException();
	}

	internal ColorPalette retrieveGDIPalette()
	{
		ColorPalette colorPalette = new ColorPalette();
		Status status = GDIPlus.GdipGetImagePaletteSize(nativeObject, out var size);
		GDIPlus.CheckStatus(status);
		IntPtr intPtr = Marshal.AllocHGlobal(size);
		try
		{
			status = GDIPlus.GdipGetImagePalette(nativeObject, intPtr, size);
			GDIPlus.CheckStatus(status);
			colorPalette.setFromGDIPalette(intPtr);
			return colorPalette;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	internal void storeGDIPalette(ColorPalette palette)
	{
		if (palette == null)
		{
			throw new ArgumentNullException("palette");
		}
		IntPtr gDIPalette = palette.getGDIPalette();
		if (gDIPalette == IntPtr.Zero)
		{
			return;
		}
		try
		{
			Status status = GDIPlus.GdipSetImagePalette(nativeObject, gDIPalette);
			GDIPlus.CheckStatus(status);
		}
		finally
		{
			Marshal.FreeHGlobal(gDIPalette);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~Image()
	{
		Dispose(disposing: false);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (GDIPlus.GdiPlusToken != 0L && nativeObject != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDisposeImage(nativeObject);
			if (stream != null)
			{
				stream.Close();
				stream = null;
			}
			nativeObject = IntPtr.Zero;
			GDIPlus.CheckStatus(status);
		}
	}

	public object Clone()
	{
		if (GDIPlus.RunningOnWindows() && stream != null)
		{
			return CloneFromStream();
		}
		IntPtr imageclone = IntPtr.Zero;
		Status status = GDIPlus.GdipCloneImage(NativeObject, out imageclone);
		GDIPlus.CheckStatus(status);
		if (this is Bitmap)
		{
			return new Bitmap(imageclone);
		}
		return new Metafile(imageclone);
	}

	private object CloneFromStream()
	{
		byte[] buffer = new byte[stream.Length];
		MemoryStream memoryStream = new MemoryStream(buffer);
		int num = (int)((stream.Length >= 4096) ? 4096 : stream.Length);
		byte[] buffer2 = new byte[num];
		stream.Position = 0L;
		do
		{
			num = stream.Read(buffer2, 0, num);
			memoryStream.Write(buffer2, 0, num);
		}
		while (num == 4096);
		IntPtr zero = IntPtr.Zero;
		zero = InitFromStream(memoryStream);
		if (this is Bitmap)
		{
			return new Bitmap(zero, memoryStream);
		}
		return new Metafile(zero, memoryStream);
	}
}
