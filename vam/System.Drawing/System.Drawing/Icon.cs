using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Drawing;

[Serializable]
[TypeConverter(typeof(IconConverter))]
[Editor("System.Drawing.Design.IconEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
public sealed class Icon : MarshalByRefObject, IDisposable, ICloneable, ISerializable
{
	internal struct IconDirEntry
	{
		internal byte width;

		internal byte height;

		internal byte colorCount;

		internal byte reserved;

		internal ushort planes;

		internal ushort bitCount;

		internal uint bytesInRes;

		internal uint imageOffset;
	}

	internal struct IconDir
	{
		internal ushort idReserved;

		internal ushort idType;

		internal ushort idCount;

		internal IconDirEntry[] idEntries;
	}

	internal struct BitmapInfoHeader
	{
		internal uint biSize;

		internal int biWidth;

		internal int biHeight;

		internal ushort biPlanes;

		internal ushort biBitCount;

		internal uint biCompression;

		internal uint biSizeImage;

		internal int biXPelsPerMeter;

		internal int biYPelsPerMeter;

		internal uint biClrUsed;

		internal uint biClrImportant;
	}

	internal struct IconImage
	{
		internal BitmapInfoHeader iconHeader;

		internal uint[] iconColors;

		internal byte[] iconXOR;

		internal byte[] iconAND;
	}

	private Size iconSize;

	private IntPtr handle = IntPtr.Zero;

	private IconDir iconDir;

	private ushort id;

	private IconImage[] imageData;

	private bool undisposable;

	private bool disposed;

	private Bitmap bitmap;

	[Browsable(false)]
	public IntPtr Handle
	{
		get
		{
			if (!disposed && handle == IntPtr.Zero)
			{
				if (GDIPlus.RunningOnUnix())
				{
					handle = GetInternalBitmap().NativeObject;
				}
				else
				{
					IconInfo piconinfo = default(IconInfo);
					piconinfo.IsIcon = true;
					piconinfo.hbmColor = ToBitmap().GetHbitmap();
					piconinfo.hbmMask = piconinfo.hbmColor;
					handle = GDIPlus.CreateIconIndirect(ref piconinfo);
				}
			}
			return handle;
		}
	}

	[Browsable(false)]
	public int Height => iconSize.Height;

	public Size Size => iconSize;

	[Browsable(false)]
	public int Width => iconSize.Width;

	private Icon()
	{
	}

	private Icon(IntPtr handle)
	{
		this.handle = handle;
		if (GDIPlus.RunningOnUnix())
		{
			bitmap = Bitmap.FromHicon(handle);
			iconSize = new Size(bitmap.Width, bitmap.Height);
		}
		else
		{
			GDIPlus.GetIconInfo(handle, out var iconinfo);
			if (!iconinfo.IsIcon)
			{
				throw new NotImplementedException(global::Locale.GetText("Handle doesn't represent an ICON."));
			}
			iconSize = new Size(iconinfo.xHotspot * 2, iconinfo.yHotspot * 2);
			bitmap = Image.FromHbitmap(iconinfo.hbmColor);
		}
		undisposable = true;
	}

	public Icon(Icon original, int width, int height)
		: this(original, new Size(width, height))
	{
	}

	public Icon(Icon original, Size size)
	{
		if (original == null)
		{
			throw new ArgumentException("original");
		}
		iconSize = size;
		iconDir = original.iconDir;
		int idCount = iconDir.idCount;
		if (idCount > 0)
		{
			imageData = original.imageData;
			id = ushort.MaxValue;
			for (ushort num = 0; num < idCount; num++)
			{
				IconDirEntry iconDirEntry = iconDir.idEntries[num];
				if (iconDirEntry.height == size.Height || iconDirEntry.width == size.Width)
				{
					id = num;
					break;
				}
			}
			if (id == ushort.MaxValue)
			{
				int num2 = Math.Min(size.Height, size.Width);
				IconDirEntry iconDirEntry2 = iconDir.idEntries[0];
				for (ushort num3 = 1; num3 < idCount; num3++)
				{
					IconDirEntry iconDirEntry3 = iconDir.idEntries[num3];
					if ((iconDirEntry3.height < num2 || iconDirEntry3.width < num2) && (iconDirEntry3.height > iconDirEntry2.height || iconDirEntry3.width > iconDirEntry2.width))
					{
						id = num3;
					}
				}
			}
			if (id == ushort.MaxValue)
			{
				id = (ushort)(idCount - 1);
			}
			iconSize.Height = iconDir.idEntries[id].height;
			iconSize.Width = iconDir.idEntries[id].width;
		}
		else
		{
			iconSize.Height = size.Height;
			iconSize.Width = size.Width;
		}
		if (original.bitmap != null)
		{
			bitmap = (Bitmap)original.bitmap.Clone();
		}
	}

	public Icon(Stream stream)
		: this(stream, 32, 32)
	{
	}

	public Icon(Stream stream, int width, int height)
	{
		InitFromStreamWithSize(stream, width, height);
	}

	public Icon(string fileName)
	{
		using FileStream stream = File.OpenRead(fileName);
		InitFromStreamWithSize(stream, 32, 32);
	}

	public Icon(Type type, string resource)
	{
		if (resource == null)
		{
			throw new ArgumentException("resource");
		}
		using Stream stream = type.Assembly.GetManifestResourceStream(type, resource);
		if (stream == null)
		{
			string text = global::Locale.GetText("Resource '{0}' was not found.", resource);
			throw new FileNotFoundException(text);
		}
		InitFromStreamWithSize(stream, 32, 32);
	}

	private Icon(SerializationInfo info, StreamingContext context)
	{
		MemoryStream memoryStream = null;
		int num = 0;
		int num2 = 0;
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SerializationEntry current = enumerator.Current;
			if (string.Compare(current.Name, "IconData", ignoreCase: true) == 0)
			{
				memoryStream = new MemoryStream((byte[])current.Value);
			}
			if (string.Compare(current.Name, "IconSize", ignoreCase: true) == 0)
			{
				Size size = (Size)current.Value;
				num = size.Width;
				num2 = size.Height;
			}
		}
		if (memoryStream != null && num == num2)
		{
			memoryStream.Seek(0L, SeekOrigin.Begin);
			InitFromStreamWithSize(memoryStream, num, num2);
		}
	}

	internal Icon(string resourceName, bool undisposable)
	{
		using (Stream stream = typeof(Icon).Assembly.GetManifestResourceStream(resourceName))
		{
			if (stream == null)
			{
				string text = global::Locale.GetText("Resource '{0}' was not found.", resourceName);
				throw new FileNotFoundException(text);
			}
			InitFromStreamWithSize(stream, 32, 32);
		}
		this.undisposable = true;
	}

	public Icon(Stream stream, Size size)
		: this(stream, size.Width, size.Height)
	{
	}

	public Icon(string fileName, int width, int height)
	{
		using FileStream stream = File.OpenRead(fileName);
		InitFromStreamWithSize(stream, width, height);
	}

	public Icon(string fileName, Size size)
	{
		using FileStream stream = File.OpenRead(fileName);
		InitFromStreamWithSize(stream, size.Width, size.Height);
	}

	void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
	{
		MemoryStream memoryStream = new MemoryStream();
		Save(memoryStream);
		si.AddValue("IconSize", Size, typeof(Size));
		si.AddValue("IconData", memoryStream.ToArray());
	}

	[System.MonoLimitation("The same icon, SystemIcons.WinLogo, is returned for all file types.")]
	public static Icon ExtractAssociatedIcon(string filePath)
	{
		if (string.IsNullOrEmpty(filePath))
		{
			throw new ArgumentException(global::Locale.GetText("Null or empty path."), "filePath");
		}
		if (!File.Exists(filePath))
		{
			throw new FileNotFoundException(global::Locale.GetText("Couldn't find specified file."), filePath);
		}
		return SystemIcons.WinLogo;
	}

	public void Dispose()
	{
		if (undisposable)
		{
			return;
		}
		if (!disposed)
		{
			if (GDIPlus.RunningOnWindows() && handle != IntPtr.Zero)
			{
				GDIPlus.DestroyIcon(handle);
				handle = IntPtr.Zero;
			}
			if (bitmap != null)
			{
				bitmap.Dispose();
				bitmap = null;
			}
			GC.SuppressFinalize(this);
		}
		disposed = true;
	}

	public object Clone()
	{
		return new Icon(this, Size);
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static Icon FromHandle(IntPtr handle)
	{
		if (handle == IntPtr.Zero)
		{
			throw new ArgumentException("handle");
		}
		return new Icon(handle);
	}

	private void SaveIconImage(BinaryWriter writer, IconImage ii)
	{
		BitmapInfoHeader iconHeader = ii.iconHeader;
		writer.Write(iconHeader.biSize);
		writer.Write(iconHeader.biWidth);
		writer.Write(iconHeader.biHeight);
		writer.Write(iconHeader.biPlanes);
		writer.Write(iconHeader.biBitCount);
		writer.Write(iconHeader.biCompression);
		writer.Write(iconHeader.biSizeImage);
		writer.Write(iconHeader.biXPelsPerMeter);
		writer.Write(iconHeader.biYPelsPerMeter);
		writer.Write(iconHeader.biClrUsed);
		writer.Write(iconHeader.biClrImportant);
		int num = ii.iconColors.Length;
		for (int i = 0; i < num; i++)
		{
			writer.Write(ii.iconColors[i]);
		}
		writer.Write(ii.iconXOR);
		writer.Write(ii.iconAND);
	}

	private void SaveIconDirEntry(BinaryWriter writer, IconDirEntry ide, uint offset)
	{
		writer.Write(ide.width);
		writer.Write(ide.height);
		writer.Write(ide.colorCount);
		writer.Write(ide.reserved);
		writer.Write(ide.planes);
		writer.Write(ide.bitCount);
		writer.Write(ide.bytesInRes);
		writer.Write((offset != uint.MaxValue) ? offset : ide.imageOffset);
	}

	private void SaveAll(BinaryWriter writer)
	{
		writer.Write(iconDir.idReserved);
		writer.Write(iconDir.idType);
		ushort idCount = iconDir.idCount;
		writer.Write(idCount);
		for (int i = 0; i < idCount; i++)
		{
			SaveIconDirEntry(writer, iconDir.idEntries[i], uint.MaxValue);
		}
		for (int j = 0; j < idCount; j++)
		{
			SaveIconImage(writer, imageData[j]);
		}
	}

	private void SaveBestSingleIcon(BinaryWriter writer, int width, int height)
	{
		writer.Write(iconDir.idReserved);
		writer.Write(iconDir.idType);
		writer.Write((ushort)1);
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < iconDir.idCount; i++)
		{
			IconDirEntry iconDirEntry = iconDir.idEntries[i];
			if (width == iconDirEntry.width && height == iconDirEntry.height && iconDirEntry.bitCount >= num2)
			{
				num2 = iconDirEntry.bitCount;
				num = i;
			}
		}
		SaveIconDirEntry(writer, iconDir.idEntries[num], 22u);
		SaveIconImage(writer, imageData[num]);
	}

	private void SaveBitmapAsIcon(BinaryWriter writer)
	{
		writer.Write((ushort)0);
		writer.Write((ushort)1);
		writer.Write((ushort)1);
		IconDirEntry ide = default(IconDirEntry);
		ide.width = (byte)bitmap.Width;
		ide.height = (byte)bitmap.Height;
		ide.colorCount = 0;
		ide.reserved = 0;
		ide.planes = 0;
		ide.bitCount = 32;
		ide.imageOffset = 22u;
		BitmapInfoHeader iconHeader = default(BitmapInfoHeader);
		iconHeader.biSize = (uint)Marshal.SizeOf(typeof(BitmapInfoHeader));
		iconHeader.biWidth = bitmap.Width;
		iconHeader.biHeight = 2 * bitmap.Height;
		iconHeader.biPlanes = 1;
		iconHeader.biBitCount = 32;
		iconHeader.biCompression = 0u;
		iconHeader.biSizeImage = 0u;
		iconHeader.biXPelsPerMeter = 0;
		iconHeader.biYPelsPerMeter = 0;
		iconHeader.biClrUsed = 0u;
		iconHeader.biClrImportant = 0u;
		IconImage ii = default(IconImage);
		ii.iconHeader = iconHeader;
		ii.iconColors = new uint[0];
		int num = (((iconHeader.biBitCount * bitmap.Width + 31) & -32) >> 3) * bitmap.Height;
		ii.iconXOR = new byte[num];
		int num2 = 0;
		for (int num3 = bitmap.Height - 1; num3 >= 0; num3--)
		{
			for (int i = 0; i < bitmap.Width; i++)
			{
				Color pixel = bitmap.GetPixel(i, num3);
				ii.iconXOR[num2++] = pixel.B;
				ii.iconXOR[num2++] = pixel.G;
				ii.iconXOR[num2++] = pixel.R;
				ii.iconXOR[num2++] = pixel.A;
			}
		}
		int num4 = ((Width + 31) & -32) >> 3;
		int num5 = num4 * bitmap.Height;
		ii.iconAND = new byte[num5];
		ide.bytesInRes = (uint)(iconHeader.biSize + num + num5);
		SaveIconDirEntry(writer, ide, uint.MaxValue);
		SaveIconImage(writer, ii);
	}

	private void Save(Stream outputStream, int width, int height)
	{
		BinaryWriter binaryWriter = new BinaryWriter(outputStream);
		if (iconDir.idEntries != null)
		{
			if (width == -1 && height == -1)
			{
				SaveAll(binaryWriter);
			}
			else
			{
				SaveBestSingleIcon(binaryWriter, width, height);
			}
		}
		else if (bitmap != null)
		{
			SaveBitmapAsIcon(binaryWriter);
		}
		binaryWriter.Flush();
	}

	public void Save(Stream outputStream)
	{
		if (outputStream == null)
		{
			throw new NullReferenceException("outputStream");
		}
		Save(outputStream, -1, -1);
	}

	internal Bitmap BuildBitmapOnWin32()
	{
		if (imageData == null)
		{
			return new Bitmap(32, 32);
		}
		IconImage iconImage = imageData[id];
		BitmapInfoHeader iconHeader = iconImage.iconHeader;
		int num = iconHeader.biHeight / 2;
		if (iconHeader.biClrUsed == 0 && iconHeader.biBitCount < 24)
		{
			int num2 = 1 << (int)iconHeader.biBitCount;
		}
		Bitmap bitmap;
		switch (iconHeader.biBitCount)
		{
		case 1:
			bitmap = new Bitmap(iconHeader.biWidth, num, PixelFormat.Format1bppIndexed);
			break;
		case 4:
			bitmap = new Bitmap(iconHeader.biWidth, num, PixelFormat.Format4bppIndexed);
			break;
		case 8:
			bitmap = new Bitmap(iconHeader.biWidth, num, PixelFormat.Format8bppIndexed);
			break;
		case 24:
			bitmap = new Bitmap(iconHeader.biWidth, num, PixelFormat.Format24bppRgb);
			break;
		case 32:
			bitmap = new Bitmap(iconHeader.biWidth, num, PixelFormat.Format32bppArgb);
			break;
		default:
		{
			string text = global::Locale.GetText("Unexpected number of bits: {0}", iconHeader.biBitCount);
			throw new Exception(text);
		}
		}
		if (iconHeader.biBitCount < 24)
		{
			ColorPalette palette = bitmap.Palette;
			for (int i = 0; i < iconImage.iconColors.Length; i++)
			{
				ref Color reference = ref palette.Entries[i];
				reference = Color.FromArgb((int)iconImage.iconColors[i] | -16777216);
			}
			bitmap.Palette = palette;
		}
		int num3 = ((iconHeader.biWidth * iconHeader.biBitCount + 31) & -32) >> 3;
		BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
		for (int j = 0; j < num; j++)
		{
			Marshal.Copy(iconImage.iconXOR, num3 * j, (IntPtr)(bitmapData.Scan0.ToInt64() + bitmapData.Stride * (num - 1 - j)), num3);
		}
		bitmap.UnlockBits(bitmapData);
		bitmap = new Bitmap(bitmap);
		num3 = ((iconHeader.biWidth + 31) & -32) >> 3;
		for (int k = 0; k < num; k++)
		{
			for (int l = 0; l < iconHeader.biWidth / 8; l++)
			{
				for (int num4 = 7; num4 >= 0; num4--)
				{
					if (((uint)(iconImage.iconAND[k * num3 + l] >> num4) & (true ? 1u : 0u)) != 0)
					{
						bitmap.SetPixel(l * 8 + 7 - num4, num - k - 1, Color.Transparent);
					}
				}
			}
		}
		return bitmap;
	}

	internal Bitmap GetInternalBitmap()
	{
		if (bitmap == null)
		{
			if (GDIPlus.RunningOnUnix())
			{
				using MemoryStream memoryStream = new MemoryStream();
				Save(memoryStream, Width, Height);
				memoryStream.Position = 0L;
				bitmap = (Bitmap)Image.LoadFromStream(memoryStream, keepAlive: false);
			}
			else
			{
				bitmap = BuildBitmapOnWin32();
			}
		}
		return bitmap;
	}

	public Bitmap ToBitmap()
	{
		if (disposed)
		{
			throw new ObjectDisposedException(global::Locale.GetText("Icon instance was disposed."));
		}
		return new Bitmap(GetInternalBitmap());
	}

	public override string ToString()
	{
		return "<Icon>";
	}

	~Icon()
	{
		Dispose();
	}

	private void InitFromStreamWithSize(Stream stream, int width, int height)
	{
		if (stream == null || stream.Length == 0L)
		{
			throw new ArgumentException("The argument 'stream' must be a picture that can be used as a Icon", "stream");
		}
		BinaryReader binaryReader = new BinaryReader(stream);
		iconDir.idReserved = binaryReader.ReadUInt16();
		if (iconDir.idReserved != 0)
		{
			throw new ArgumentException("Invalid Argument", "stream");
		}
		iconDir.idType = binaryReader.ReadUInt16();
		if (iconDir.idType != 1)
		{
			throw new ArgumentException("Invalid Argument", "stream");
		}
		ushort num = binaryReader.ReadUInt16();
		ArrayList arrayList = new ArrayList(num);
		bool flag = false;
		IconDirEntry iconDirEntry = default(IconDirEntry);
		for (int i = 0; i < num; i++)
		{
			iconDirEntry.width = binaryReader.ReadByte();
			iconDirEntry.height = binaryReader.ReadByte();
			iconDirEntry.colorCount = binaryReader.ReadByte();
			iconDirEntry.reserved = binaryReader.ReadByte();
			iconDirEntry.planes = binaryReader.ReadUInt16();
			iconDirEntry.bitCount = binaryReader.ReadUInt16();
			iconDirEntry.bytesInRes = binaryReader.ReadUInt32();
			iconDirEntry.imageOffset = binaryReader.ReadUInt32();
			if (iconDirEntry.width != 0 || iconDirEntry.height != 0)
			{
				int num2 = arrayList.Add(iconDirEntry);
				if (!flag && (iconDirEntry.height == height || iconDirEntry.width == width))
				{
					id = (ushort)num2;
					flag = true;
					iconSize.Height = iconDirEntry.height;
					iconSize.Width = iconDirEntry.width;
				}
			}
		}
		num = (ushort)arrayList.Count;
		if (num == 0)
		{
			throw new Win32Exception(0, "No valid icon entry were found.");
		}
		iconDir.idCount = num;
		imageData = new IconImage[num];
		iconDir.idEntries = new IconDirEntry[num];
		arrayList.CopyTo(iconDir.idEntries);
		if (!flag)
		{
			uint num3 = 0u;
			for (int j = 0; j < num; j++)
			{
				if (iconDir.idEntries[j].bytesInRes >= num3)
				{
					num3 = iconDir.idEntries[j].bytesInRes;
					id = (ushort)j;
					iconSize.Height = iconDir.idEntries[j].height;
					iconSize.Width = iconDir.idEntries[j].width;
				}
			}
		}
		for (int k = 0; k < num; k++)
		{
			IconImage iconImage = default(IconImage);
			BitmapInfoHeader iconHeader = default(BitmapInfoHeader);
			stream.Seek(iconDir.idEntries[k].imageOffset, SeekOrigin.Begin);
			byte[] array = new byte[iconDir.idEntries[k].bytesInRes];
			stream.Read(array, 0, array.Length);
			BinaryReader binaryReader2 = new BinaryReader(new MemoryStream(array));
			iconHeader.biSize = binaryReader2.ReadUInt32();
			iconHeader.biWidth = binaryReader2.ReadInt32();
			iconHeader.biHeight = binaryReader2.ReadInt32();
			iconHeader.biPlanes = binaryReader2.ReadUInt16();
			iconHeader.biBitCount = binaryReader2.ReadUInt16();
			iconHeader.biCompression = binaryReader2.ReadUInt32();
			iconHeader.biSizeImage = binaryReader2.ReadUInt32();
			iconHeader.biXPelsPerMeter = binaryReader2.ReadInt32();
			iconHeader.biYPelsPerMeter = binaryReader2.ReadInt32();
			iconHeader.biClrUsed = binaryReader2.ReadUInt32();
			iconHeader.biClrImportant = binaryReader2.ReadUInt32();
			iconImage.iconHeader = iconHeader;
			int num4 = iconHeader.biBitCount switch
			{
				1 => 2, 
				4 => 16, 
				8 => 256, 
				_ => 0, 
			};
			iconImage.iconColors = new uint[num4];
			for (int l = 0; l < num4; l++)
			{
				iconImage.iconColors[l] = binaryReader2.ReadUInt32();
			}
			int num5 = iconHeader.biHeight / 2;
			int num6 = iconHeader.biWidth * iconHeader.biPlanes * iconHeader.biBitCount + 31 >> 5 << 2;
			int num7 = num6 * num5;
			iconImage.iconXOR = new byte[num7];
			int num8 = binaryReader2.Read(iconImage.iconXOR, 0, num7);
			if (num8 != num7)
			{
				string text = global::Locale.GetText("{0} data length expected {1}, read {2}", "XOR", num7, num8);
				throw new ArgumentException(text, "stream");
			}
			num6 = ((iconHeader.biWidth + 31) & -32) >> 3;
			int num9 = num6 * num5;
			iconImage.iconAND = new byte[num9];
			num8 = binaryReader2.Read(iconImage.iconAND, 0, num9);
			if (num8 != num9)
			{
				string text2 = global::Locale.GetText("{0} data length expected {1}, read {2}", "AND", num9, num8);
				throw new ArgumentException(text2, "stream");
			}
			imageData[k] = iconImage;
			binaryReader2.Close();
		}
		binaryReader.Close();
	}
}
