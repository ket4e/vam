using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms;

internal class SystemResPool
{
	private Hashtable pens = new Hashtable();

	private Hashtable dashpens = new Hashtable();

	private Hashtable sizedpens = new Hashtable();

	private Hashtable solidbrushes = new Hashtable();

	private Hashtable hatchbrushes = new Hashtable();

	private Hashtable uiImages = new Hashtable();

	private Hashtable cpcolors = new Hashtable();

	public Pen GetPen(Color color)
	{
		int num = color.ToArgb();
		lock (pens)
		{
			if (pens[num] is Pen result)
			{
				return result;
			}
			Pen pen = new Pen(color);
			pens.Add(num, pen);
			return pen;
		}
	}

	public Pen GetDashPen(Color color, DashStyle dashStyle)
	{
		string key = color.ToString() + dashStyle;
		lock (dashpens)
		{
			if (dashpens[key] is Pen result)
			{
				return result;
			}
			Pen pen = new Pen(color);
			pen.DashStyle = dashStyle;
			dashpens[key] = pen;
			return pen;
		}
	}

	public Pen GetSizedPen(Color color, int size)
	{
		string key = color.ToString() + size;
		lock (sizedpens)
		{
			if (sizedpens[key] is Pen result)
			{
				return result;
			}
			Pen pen = new Pen(color, size);
			sizedpens[key] = pen;
			return pen;
		}
	}

	public SolidBrush GetSolidBrush(Color color)
	{
		int num = color.ToArgb();
		lock (solidbrushes)
		{
			if (solidbrushes[num] is SolidBrush result)
			{
				return result;
			}
			SolidBrush solidBrush = new SolidBrush(color);
			solidbrushes.Add(num, solidBrush);
			return solidBrush;
		}
	}

	public HatchBrush GetHatchBrush(HatchStyle hatchStyle, Color foreColor, Color backColor)
	{
		int num = (int)hatchStyle;
		string key = num + foreColor.ToString() + backColor.ToString();
		lock (hatchbrushes)
		{
			HatchBrush hatchBrush = (HatchBrush)hatchbrushes[key];
			if (hatchBrush == null)
			{
				hatchBrush = new HatchBrush(hatchStyle, foreColor, backColor);
				hatchbrushes.Add(key, hatchBrush);
			}
			return hatchBrush;
		}
	}

	public void AddUIImage(Image image, string name, int size)
	{
		string key = name + size;
		lock (uiImages)
		{
			if (!uiImages.Contains(key))
			{
				uiImages.Add(key, image);
			}
		}
	}

	public Image GetUIImage(string name, int size)
	{
		string key = name + size;
		return uiImages[key] as Image;
	}

	public CPColor GetCPColor(Color color)
	{
		lock (cpcolors)
		{
			object obj = cpcolors[color];
			if (obj == null)
			{
				CPColor cPColor = default(CPColor);
				cPColor.Dark = ControlPaint.Dark(color);
				cPColor.DarkDark = ControlPaint.DarkDark(color);
				cPColor.Light = ControlPaint.Light(color);
				cPColor.LightLight = ControlPaint.LightLight(color);
				cpcolors.Add(color, cPColor);
				return cPColor;
			}
			return (CPColor)obj;
		}
	}
}
