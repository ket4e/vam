using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Un4seen.Bass.AddOn.Tags;

[Serializable]
public class TagPicture
{
	public enum PICTURE_TYPE : byte
	{
		Unknown,
		Icon32,
		OtherIcon,
		FrontAlbumCover,
		BackAlbumCover,
		LeafletPage,
		Media,
		LeadArtist,
		Artists,
		Conductor,
		Orchestra,
		Composer,
		Writer,
		Location,
		RecordingSession,
		Performance,
		VideoCapture,
		ColoredFish,
		Illustration,
		BandLogo,
		PublisherLogo
	}

	public enum PICTURE_STORAGE : byte
	{
		Internal,
		External
	}

	public string MIMEType;

	public PICTURE_TYPE PictureType;

	public string Description;

	public byte[] Data;

	public int AttributeIndex = -1;

	public PICTURE_STORAGE PictureStorage;

	public Image PictureImage
	{
		get
		{
			if (PictureStorage == PICTURE_STORAGE.Internal)
			{
				try
				{
					return new ImageConverter().ConvertFrom(Data) as Image;
				}
				catch
				{
					return null;
				}
			}
			try
			{
				string @string = Encoding.UTF8.GetString(Data);
				if (!string.IsNullOrEmpty(@string))
				{
					return LoadImageFromFile(@string);
				}
				return null;
			}
			catch
			{
				return null;
			}
		}
	}

	public TagPicture(int attribIndex, string mimeType, PICTURE_TYPE pictureType, string description, byte[] data)
	{
		AttributeIndex = attribIndex;
		MIMEType = mimeType;
		PictureType = pictureType;
		Description = description;
		Data = data;
		PictureStorage = PICTURE_STORAGE.Internal;
	}

	public TagPicture(Image image, PICTURE_TYPE pictureType, string description)
	{
		PictureType = pictureType;
		Description = description;
		ImageConverter imageConverter = new ImageConverter();
		Data = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
		MIMEType = GetMimeTypeFromImage(image);
		PictureStorage = PICTURE_STORAGE.Internal;
	}

	public TagPicture(string file)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		Data = uTF8Encoding.GetBytes(file);
		MIMEType = GetMimeTypeFromFile(file);
		PictureType = PICTURE_TYPE.Unknown;
		Description = file;
		PictureStorage = PICTURE_STORAGE.External;
	}

	public TagPicture(string file, PICTURE_TYPE type, string description)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		Data = uTF8Encoding.GetBytes(file);
		MIMEType = GetMimeTypeFromFile(file);
		PictureType = type;
		Description = description;
		PictureStorage = PICTURE_STORAGE.External;
	}

	internal TagPicture(Tag pTag)
	{
		AttributeIndex = pTag.Index;
		MIMEType = "Unknown";
		PictureType = PICTURE_TYPE.Unknown;
		Description = "";
		MemoryStream memoryStream = null;
		BinaryReader binaryReader = null;
		if (!(pTag.Name == "WM/Picture"))
		{
			return;
		}
		try
		{
			memoryStream = new MemoryStream((byte[])pTag);
			binaryReader = new BinaryReader(memoryStream);
			if (Utils.Is64Bit)
			{
				MIMEType = Marshal.PtrToStringUni(new IntPtr(binaryReader.ReadInt64()));
			}
			else
			{
				MIMEType = Marshal.PtrToStringUni(new IntPtr(binaryReader.ReadInt32()));
			}
			byte pictureType = binaryReader.ReadByte();
			try
			{
				PictureType = (PICTURE_TYPE)pictureType;
			}
			catch
			{
				PictureType = PICTURE_TYPE.Unknown;
			}
			if (Utils.Is64Bit)
			{
				Description = Marshal.PtrToStringUni(new IntPtr(binaryReader.ReadInt64()));
			}
			else
			{
				Description = Marshal.PtrToStringUni(new IntPtr(binaryReader.ReadInt32()));
			}
			int num = binaryReader.ReadInt32();
			Data = new byte[num];
			if (Utils.Is64Bit)
			{
				Marshal.Copy(new IntPtr(binaryReader.ReadInt64()), Data, 0, num);
			}
			else
			{
				Marshal.Copy(new IntPtr(binaryReader.ReadInt32()), Data, 0, num);
			}
		}
		catch
		{
		}
		finally
		{
			try
			{
				binaryReader?.Close();
				if (memoryStream != null)
				{
					memoryStream.Close();
					memoryStream.Dispose();
				}
			}
			catch
			{
			}
		}
	}

	internal TagPicture(byte[] pData, int pType)
	{
		PictureStorage = PICTURE_STORAGE.Internal;
		switch (pType)
		{
		case 1:
		{
			AttributeIndex = -1;
			MIMEType = "Unknown";
			PictureType = PICTURE_TYPE.Unknown;
			Description = "";
			MemoryStream memoryStream = null;
			BinaryReader binaryReader = null;
			if (pData == null || pData.Length <= 32)
			{
				break;
			}
			try
			{
				memoryStream = new MemoryStream(pData);
				binaryReader = new BinaryReader(memoryStream);
				PictureType = (PICTURE_TYPE)ReadInt32(binaryReader);
				int count = ReadInt32(binaryReader);
				MIMEType = Encoding.UTF8.GetString(binaryReader.ReadBytes(count));
				int count2 = ReadInt32(binaryReader);
				Description = Encoding.UTF8.GetString(binaryReader.ReadBytes(count2));
				binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				int count3 = ReadInt32(binaryReader);
				Data = binaryReader.ReadBytes(count3);
				break;
			}
			catch
			{
				break;
			}
			finally
			{
				try
				{
					binaryReader?.Close();
					if (memoryStream != null)
					{
						memoryStream.Close();
						memoryStream.Dispose();
					}
				}
				catch
				{
				}
			}
		}
		case 2:
			AttributeIndex = -1;
			MIMEType = "Unknown";
			PictureType = PICTURE_TYPE.Unknown;
			Description = "";
			if (pData == null || pData.Length == 0)
			{
				break;
			}
			try
			{
				int i;
				for (i = 0; pData[i] != 0; i++)
				{
				}
				Description = Encoding.UTF8.GetString(pData, 0, i);
				i++;
				Data = new byte[pData.Length - i];
				Array.Copy(pData, i, Data, 0, Data.Length);
				if (PictureImage != null)
				{
					MIMEType = GetMimeTypeFromImage(PictureImage);
				}
				break;
			}
			catch
			{
				break;
			}
		default:
			AttributeIndex = -1;
			MIMEType = "Unknown";
			PictureType = PICTURE_TYPE.FrontAlbumCover;
			Description = "CoverArt";
			Data = pData;
			if (PictureImage != null)
			{
				MIMEType = GetMimeTypeFromImage(PictureImage);
			}
			break;
		}
	}

	public TagPicture(TagPicture pic, int size)
	{
		Image pictureImage = pic.PictureImage;
		int num = size;
		int num2 = size;
		if (pictureImage.Width > pictureImage.Height)
		{
			num2 = (int)((double)num2 / ((double)pictureImage.Width / (double)pictureImage.Height));
		}
		else
		{
			num = (int)((double)num / ((double)pictureImage.Height / (double)pictureImage.Width));
		}
		try
		{
			using Bitmap bitmap = new Bitmap(num, num2);
			using Graphics graphics = Graphics.FromImage(bitmap);
			graphics.DrawImage(pictureImage, 0, 0, num, num2);
			using MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, ImageFormat.Jpeg);
			Data = memoryStream.ToArray();
			memoryStream.Close();
		}
		catch
		{
		}
		PictureType = pic.PictureType;
		Description = pic.Description;
		MIMEType = "image/jpeg";
		PictureStorage = PICTURE_STORAGE.Internal;
	}

	private static int ReadInt32(BinaryReader stream)
	{
		byte[] array = new byte[4];
		stream.Read(array, 0, 4);
		return (array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
	}

	public override string ToString()
	{
		return $"{Description} [{PictureType}, {MIMEType}]";
	}

	public static string GetMimeTypeFromImage(Image pImage)
	{
		string result = "Unknown";
		if (pImage == null)
		{
			return result;
		}
		ImageFormat rawFormat = pImage.RawFormat;
		if (rawFormat.Guid == ImageFormat.Jpeg.Guid)
		{
			return "image/jpeg";
		}
		if (rawFormat.Guid == ImageFormat.Gif.Guid)
		{
			return "image/gif";
		}
		if (rawFormat.Guid == ImageFormat.MemoryBmp.Guid)
		{
			return "image/bmp";
		}
		if (rawFormat.Guid == ImageFormat.Bmp.Guid)
		{
			return "image/bmp";
		}
		if (rawFormat.Guid == ImageFormat.Png.Guid)
		{
			return "image/png";
		}
		if (rawFormat.Guid == ImageFormat.Icon.Guid)
		{
			return "image/x-icon";
		}
		if (rawFormat.Guid == ImageFormat.Tiff.Guid)
		{
			return "image/tiff";
		}
		if (rawFormat.Guid == ImageFormat.Emf.Guid)
		{
			return "image/x-emf";
		}
		if (rawFormat.Guid == ImageFormat.Wmf.Guid)
		{
			return "image/x-wmf";
		}
		return "image/jpeg";
	}

	public static string GetMimeTypeFromFile(string pFile)
	{
		string result = "image/jpeg";
		switch (Path.GetExtension(pFile).ToLower())
		{
		case ".jpg":
		case ".jpeg":
			result = "image/jpeg";
			break;
		case ".gif":
			result = "image/gif";
			break;
		case ".bmp":
			result = "image/bmp";
			break;
		case ".png":
			result = "image/png";
			break;
		case ".ico":
			result = "image/x-icon";
			break;
		case ".tif":
		case ".tiff":
			result = "image/tiff";
			break;
		case ".emf":
			result = "image/x-emf";
			break;
		case ".wmf":
			result = "image/x-wmf";
			break;
		}
		return result;
	}

	public static Image LoadImageFromFile(string filename)
	{
		Image result = null;
		try
		{
			using Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, (int)stream.Length);
			result = new ImageConverter().ConvertFrom(array) as Image;
			stream.Close();
		}
		catch
		{
		}
		return result;
	}

	public static bool SaveImageToFile(Image img, string filename, ImageFormat format)
	{
		bool flag = false;
		bool flag2 = false;
		try
		{
			byte[] array = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (Image image = (Image)img.Clone())
				{
					image.Save(memoryStream, format);
				}
				array = memoryStream.ToArray();
				memoryStream.Close();
			}
			using FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
			flag2 = true;
			try
			{
				fileStream.Write(array, 0, array.Length);
				return true;
			}
			finally
			{
				fileStream.Close();
			}
		}
		catch
		{
			if (flag2 && File.Exists(filename))
			{
				File.Delete(filename);
			}
			throw;
		}
	}

	private static bool _ThumbnailCallback()
	{
		return true;
	}
}
