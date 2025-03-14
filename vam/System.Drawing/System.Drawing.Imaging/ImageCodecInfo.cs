using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

public sealed class ImageCodecInfo
{
	private Guid clsid;

	private string codecName;

	private string dllName;

	private string filenameExtension;

	private ImageCodecFlags flags;

	private string formatDescription;

	private Guid formatID;

	private string mimeType;

	private byte[][] signatureMasks;

	private byte[][] signaturePatterns;

	private int version;

	public Guid Clsid
	{
		get
		{
			return clsid;
		}
		set
		{
			clsid = value;
		}
	}

	public string CodecName
	{
		get
		{
			return codecName;
		}
		set
		{
			codecName = value;
		}
	}

	public string DllName
	{
		get
		{
			return dllName;
		}
		set
		{
			dllName = value;
		}
	}

	public string FilenameExtension
	{
		get
		{
			return filenameExtension;
		}
		set
		{
			filenameExtension = value;
		}
	}

	public ImageCodecFlags Flags
	{
		get
		{
			return flags;
		}
		set
		{
			flags = value;
		}
	}

	public string FormatDescription
	{
		get
		{
			return formatDescription;
		}
		set
		{
			formatDescription = value;
		}
	}

	public Guid FormatID
	{
		get
		{
			return formatID;
		}
		set
		{
			formatID = value;
		}
	}

	public string MimeType
	{
		get
		{
			return mimeType;
		}
		set
		{
			mimeType = value;
		}
	}

	[CLSCompliant(false)]
	public byte[][] SignatureMasks
	{
		get
		{
			return signatureMasks;
		}
		set
		{
			signatureMasks = value;
		}
	}

	[CLSCompliant(false)]
	public byte[][] SignaturePatterns
	{
		get
		{
			return signaturePatterns;
		}
		set
		{
			signaturePatterns = value;
		}
	}

	public int Version
	{
		get
		{
			return version;
		}
		set
		{
			version = value;
		}
	}

	internal ImageCodecInfo()
	{
	}

	public static ImageCodecInfo[] GetImageDecoders()
	{
		GdipImageCodecInfo gdipImageCodecInfo = default(GdipImageCodecInfo);
		Status status = GDIPlus.GdipGetImageDecodersSize(out var decoderNums, out var arraySize);
		GDIPlus.CheckStatus(status);
		ImageCodecInfo[] array = new ImageCodecInfo[decoderNums];
		if (decoderNums == 0)
		{
			return array;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(arraySize);
		try
		{
			status = GDIPlus.GdipGetImageDecoders(decoderNums, arraySize, intPtr);
			GDIPlus.CheckStatus(status);
			int num = Marshal.SizeOf(gdipImageCodecInfo);
			IntPtr ptr = intPtr;
			int num2 = 0;
			while (num2 < decoderNums)
			{
				gdipImageCodecInfo = (GdipImageCodecInfo)Marshal.PtrToStructure(ptr, typeof(GdipImageCodecInfo));
				array[num2] = new ImageCodecInfo();
				GdipImageCodecInfo.MarshalTo(gdipImageCodecInfo, array[num2]);
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

	public static ImageCodecInfo[] GetImageEncoders()
	{
		GdipImageCodecInfo gdipImageCodecInfo = default(GdipImageCodecInfo);
		Status status = GDIPlus.GdipGetImageEncodersSize(out var encoderNums, out var arraySize);
		GDIPlus.CheckStatus(status);
		ImageCodecInfo[] array = new ImageCodecInfo[encoderNums];
		if (encoderNums == 0)
		{
			return array;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(arraySize);
		try
		{
			status = GDIPlus.GdipGetImageEncoders(encoderNums, arraySize, intPtr);
			GDIPlus.CheckStatus(status);
			int num = Marshal.SizeOf(gdipImageCodecInfo);
			IntPtr ptr = intPtr;
			int num2 = 0;
			while (num2 < encoderNums)
			{
				gdipImageCodecInfo = (GdipImageCodecInfo)Marshal.PtrToStructure(ptr, typeof(GdipImageCodecInfo));
				array[num2] = new ImageCodecInfo();
				GdipImageCodecInfo.MarshalTo(gdipImageCodecInfo, array[num2]);
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
