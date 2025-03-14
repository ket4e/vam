using System.Drawing;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

[Serializable]
public class OwnerDrawPropertyBag : MarshalByRefObject, ISerializable
{
	private Color fore_color;

	private Color back_color;

	private Font font;

	public Color ForeColor
	{
		get
		{
			return fore_color;
		}
		set
		{
			fore_color = value;
		}
	}

	public Color BackColor
	{
		get
		{
			return back_color;
		}
		set
		{
			back_color = value;
		}
	}

	public Font Font
	{
		get
		{
			return font;
		}
		set
		{
			font = value;
		}
	}

	internal OwnerDrawPropertyBag()
	{
		fore_color = (back_color = Color.Empty);
	}

	private OwnerDrawPropertyBag(Color fore_color, Color back_color, Font font)
	{
		this.fore_color = fore_color;
		this.back_color = back_color;
		this.font = font;
	}

	protected OwnerDrawPropertyBag(SerializationInfo info, StreamingContext context)
	{
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SerializationEntry current = enumerator.Current;
			switch (current.Name)
			{
			case "Font":
				font = (Font)current.Value;
				break;
			case "ForeColor":
				fore_color = (Color)current.Value;
				break;
			case "BackColor":
				back_color = (Color)current.Value;
				break;
			}
		}
	}

	void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
	{
		si.AddValue("BackColor", BackColor);
		si.AddValue("ForeColor", ForeColor);
		si.AddValue("Font", Font);
	}

	public virtual bool IsEmpty()
	{
		return font == null && fore_color.IsEmpty && back_color.IsEmpty;
	}

	public static OwnerDrawPropertyBag Copy(OwnerDrawPropertyBag value)
	{
		return new OwnerDrawPropertyBag(value.ForeColor, value.BackColor, value.Font);
	}
}
