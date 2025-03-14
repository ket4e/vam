using System.Drawing;
using System.IO;

namespace System.Windows.Forms.RTF;

internal class Picture
{
	private Minor image_type;

	private Image image;

	private MemoryStream data;

	private float width = -1f;

	private float height = -1f;

	private static readonly float dpix;

	public Minor ImageType
	{
		get
		{
			return image_type;
		}
		set
		{
			image_type = value;
		}
	}

	public MemoryStream Data
	{
		get
		{
			if (data == null)
			{
				data = new MemoryStream();
			}
			return data;
		}
	}

	public float Width
	{
		get
		{
			float num = width;
			if (num == -1f)
			{
				if (image == null)
				{
					image = ToImage();
				}
				num = image.Width;
			}
			return num;
		}
	}

	public float Height
	{
		get
		{
			float num = height;
			if (num == -1f)
			{
				if (image == null)
				{
					image = ToImage();
				}
				num = image.Height;
			}
			return num;
		}
	}

	public SizeF Size => new SizeF(Width, Height);

	static Picture()
	{
		dpix = TextRenderer.GetDpi().Width;
	}

	public void SetWidthFromTwips(int twips)
	{
		width = (int)((float)twips / 1440f * dpix + 0.5f);
	}

	public void SetHeightFromTwips(int twips)
	{
		height = (int)((float)twips / 1440f * dpix + 0.5f);
	}

	public bool IsValid()
	{
		if (data == null)
		{
			return false;
		}
		switch (image_type)
		{
		default:
			return false;
		case Minor.WinMetafile:
		case Minor.PngBlip:
			return true;
		}
	}

	public void DrawImage(Graphics dc, float x, float y, bool selected)
	{
		if (image == null)
		{
			image = ToImage();
		}
		float num = height;
		float num2 = width;
		if (num == -1f)
		{
			num = image.Height;
		}
		if (num2 == -1f)
		{
			num2 = image.Width;
		}
		dc.DrawImage(image, x, y, num2, num);
	}

	public Image ToImage()
	{
		data.Position = 0L;
		return Image.FromStream(data);
	}
}
