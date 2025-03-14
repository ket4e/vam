namespace System.Windows.Forms;

public class RowStyle : TableLayoutStyle
{
	private float height;

	public float Height
	{
		get
		{
			return height;
		}
		set
		{
			if (value < 0f)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (height != value)
			{
				height = value;
				if (base.Owner != null)
				{
					base.Owner.PerformLayout();
				}
			}
		}
	}

	public RowStyle()
	{
		height = 0f;
	}

	public RowStyle(SizeType sizeType)
	{
		height = 0f;
		base.SizeType = sizeType;
	}

	public RowStyle(SizeType sizeType, float height)
	{
		if (height < 0f)
		{
			throw new ArgumentOutOfRangeException("height");
		}
		base.SizeType = sizeType;
		this.height = height;
	}
}
