using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

[Serializable]
public sealed class ImageListStreamer : ISerializable
{
	private readonly ImageList.ImageCollection imageCollection;

	private Image[] images;

	private Size image_size;

	private Color back_color;

	private static byte[] header = new byte[8] { 77, 83, 70, 116, 73, 76, 3, 0 };

	internal Image[] Images => images;

	internal Size ImageSize => image_size;

	internal ColorDepth ColorDepth => ColorDepth.Depth32Bit;

	internal Color BackColor => back_color;

	internal ImageListStreamer(ImageList.ImageCollection imageCollection)
	{
		this.imageCollection = imageCollection;
	}

	private ImageListStreamer(SerializationInfo info, StreamingContext context)
	{
		byte[] array = (byte[])info.GetValue("Data", typeof(byte[]));
		if (array == null || array.Length <= 4 || array[0] != 77 || array[1] != 83 || array[2] != 70 || array[3] != 116)
		{
			return;
		}
		MemoryStream decodedStream = GetDecodedStream(array, 4, array.Length - 4);
		decodedStream.Position = 4L;
		BinaryReader binaryReader = new BinaryReader(decodedStream);
		ushort num = binaryReader.ReadUInt16();
		binaryReader.ReadUInt16();
		ushort num2 = binaryReader.ReadUInt16();
		ushort num3 = binaryReader.ReadUInt16();
		ushort num4 = binaryReader.ReadUInt16();
		uint num5 = binaryReader.ReadUInt32();
		back_color = Color.FromArgb((int)num5);
		binaryReader.ReadUInt16();
		short[] array2 = new short[4];
		for (int i = 0; i < 4; i++)
		{
			array2[i] = binaryReader.ReadInt16();
		}
		byte[] buffer = decodedStream.GetBuffer();
		int num6 = 28;
		int num7 = buffer[num6 + 2] + (buffer[num6 + 3] << 8) + (buffer[num6 + 4] << 16) + (buffer[num6 + 5] << 24);
		int num8 = buffer[num6 + 34] + (buffer[num6 + 35] << 8) + (buffer[num6 + 36] << 16) + (buffer[num6 + 37] << 24);
		int num9 = num8 + num7;
		MemoryStream stream = new MemoryStream(buffer, num6, num9);
		Bitmap bitmap = null;
		Bitmap bitmap2 = null;
		bitmap = new Bitmap(stream);
		MemoryStream memoryStream = new MemoryStream(buffer, num6 + num9, (int)(decodedStream.Length - num6 - num9));
		if (memoryStream.Length > 0)
		{
			bitmap2 = new Bitmap(memoryStream);
		}
		if (num5 == uint.MaxValue)
		{
			back_color = bitmap.GetPixel(0, 0);
		}
		if (bitmap2 != null)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;
			Bitmap bitmap3 = new Bitmap(bitmap);
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < width; k++)
				{
					if (bitmap2.GetPixel(k, j).B != 0)
					{
						bitmap3.SetPixel(k, j, Color.Transparent);
					}
				}
			}
			bitmap.Dispose();
			bitmap = bitmap3;
			bitmap2.Dispose();
			bitmap2 = null;
		}
		images = new Image[num];
		image_size = new Size(num3, num4);
		Rectangle destRect = new Rectangle(0, 0, num3, num4);
		if (num2 * bitmap.Width > num3)
		{
			num2 = (ushort)(bitmap.Width / num3);
		}
		for (int l = 0; l < num; l++)
		{
			int num10 = l % num2;
			int num11 = l / num2;
			Rectangle srcRect = new Rectangle(num10 * num3, num11 * num4, num3, num4);
			Bitmap bitmap4 = new Bitmap(num3, num4);
			using (Graphics graphics = Graphics.FromImage(bitmap4))
			{
				graphics.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
			}
			images[l] = bitmap4;
		}
		bitmap.Dispose();
	}

	public void GetObjectData(SerializationInfo si, StreamingContext context)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(header);
		Image[] array = ((imageCollection == null) ? images : imageCollection.ToArray());
		int num = 4;
		int num2 = array.Length / num;
		if (array.Length % num > 0)
		{
			num2++;
		}
		binaryWriter.Write((ushort)array.Length);
		binaryWriter.Write((ushort)array.Length);
		binaryWriter.Write((ushort)4);
		binaryWriter.Write((ushort)array[0].Width);
		binaryWriter.Write((ushort)array[0].Height);
		binaryWriter.Write(uint.MaxValue);
		binaryWriter.Write((ushort)4105);
		for (int i = 0; i < 4; i++)
		{
			binaryWriter.Write((short)(-1));
		}
		Bitmap bitmap = new Bitmap(num * ImageSize.Width, num2 * ImageSize.Height);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(BackColor), 0, 0, bitmap.Width, bitmap.Height);
			for (int j = 0; j < array.Length; j++)
			{
				graphics.DrawImage(array[j], j % num * ImageSize.Width, j / num * ImageSize.Height);
			}
		}
		MemoryStream memoryStream2 = new MemoryStream();
		bitmap.Save(memoryStream2, ImageFormat.Bmp);
		memoryStream2.WriteTo(memoryStream);
		Bitmap bitmap2 = Get1bppMask(bitmap);
		bitmap.Dispose();
		bitmap = null;
		memoryStream2 = new MemoryStream();
		bitmap2.Save(memoryStream2, ImageFormat.Bmp);
		memoryStream2.WriteTo(memoryStream);
		bitmap2.Dispose();
		memoryStream = GetRLEStream(memoryStream, 4);
		si.AddValue("Data", memoryStream.ToArray(), typeof(byte[]));
	}

	private unsafe Bitmap Get1bppMask(Bitmap main)
	{
		Rectangle rect = new Rectangle(0, 0, main.Width, main.Height);
		Bitmap bitmap = new Bitmap(main.Width, main.Height, PixelFormat.Format1bppIndexed);
		BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
		int width = images[0].Width;
		int height = images[0].Height;
		byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
		int stride = bitmapData.Stride;
		Bitmap bitmap2 = null;
		for (int i = 0; i < images.Length; i++)
		{
			bitmap2 = (Bitmap)images[i];
			Color pixel = bitmap2.GetPixel(0, 0);
			if (pixel.A != 0 && pixel == back_color)
			{
				bitmap2.MakeTransparent(back_color);
			}
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		for (int j = 0; j < main.Height; j++)
		{
			if (num3 == height)
			{
				num3 = 0;
				num5 += 4;
			}
			num6 = 0;
			num4 = 0;
			for (int k = 0; k < main.Width; k++)
			{
				if (num4 == width)
				{
					num4 = 0;
					num6++;
				}
				num2 = num5 + num6;
				if (num2 >= images.Length)
				{
					break;
				}
				bitmap2 = (Bitmap)images[num2];
				if (bitmap2.GetPixel(num4, num3).A == 0)
				{
					int num7 = num + (k >> 3);
					byte* num8 = ptr + num7;
					*num8 |= (byte)(128 >> (k & 7));
				}
				num4++;
			}
			if (num2 >= images.Length)
			{
				break;
			}
			num += stride;
			num3++;
		}
		bitmap.UnlockBits(bitmapData);
		return bitmap;
	}

	private static MemoryStream GetDecodedStream(byte[] bytes, int offset, int size)
	{
		byte[] array = new byte[512];
		int num = 0;
		MemoryStream memoryStream = new MemoryStream();
		while (size > 0)
		{
			int num2 = bytes[offset++];
			int num3 = bytes[offset++];
			if (512 - num2 < num)
			{
				memoryStream.Write(array, 0, num);
				num = 0;
			}
			for (int i = 0; i < num2; i++)
			{
				array[num++] = (byte)num3;
			}
			size -= 2;
		}
		if (num > 0)
		{
			memoryStream.Write(array, 0, num);
		}
		memoryStream.Position = 0L;
		return memoryStream;
	}

	private static MemoryStream GetRLEStream(MemoryStream input, int start)
	{
		MemoryStream memoryStream = new MemoryStream();
		byte[] buffer = input.GetBuffer();
		memoryStream.Write(buffer, 0, start);
		input.Position = start;
		int num = -1;
		int num2 = 0;
		int num3;
		while ((num3 = input.ReadByte()) != -1)
		{
			if (num != num3 || num2 == 255)
			{
				if (num != -1)
				{
					memoryStream.WriteByte((byte)num2);
					memoryStream.WriteByte((byte)num);
				}
				num = num3;
				num2 = 0;
			}
			num2++;
		}
		if (num2 > 0)
		{
			memoryStream.WriteByte((byte)num2);
			memoryStream.WriteByte((byte)num3);
		}
		return memoryStream;
	}
}
