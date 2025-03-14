using System.Drawing;

namespace System.Windows.Forms;

public sealed class ListViewInsertionMark
{
	private ListView listview_owner;

	private bool appears_after_item;

	private Rectangle bounds;

	private Color? color;

	private int index;

	public bool AppearsAfterItem
	{
		get
		{
			return appears_after_item;
		}
		set
		{
			if (value != appears_after_item)
			{
				appears_after_item = value;
				listview_owner.item_control.Invalidate(bounds);
				UpdateBounds();
				listview_owner.item_control.Invalidate(bounds);
			}
		}
	}

	public Rectangle Bounds => bounds;

	public Color Color
	{
		get
		{
			Color? color = this.color;
			return color.HasValue ? this.color.Value : listview_owner.ForeColor;
		}
		set
		{
			color = value;
		}
	}

	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			if (value != index)
			{
				index = value;
				listview_owner.item_control.Invalidate(bounds);
				UpdateBounds();
				listview_owner.item_control.Invalidate(bounds);
			}
		}
	}

	internal PointF[] TopTriangle
	{
		get
		{
			PointF pointF = new PointF(bounds.X, bounds.Y);
			PointF pointF2 = new PointF(bounds.Right, bounds.Y);
			PointF pointF3 = new PointF(bounds.X + (bounds.Right - bounds.X) / 2, bounds.Y + 5);
			return new PointF[3] { pointF, pointF2, pointF3 };
		}
	}

	internal PointF[] BottomTriangle
	{
		get
		{
			PointF pointF = new PointF(bounds.X, bounds.Bottom);
			PointF pointF2 = new PointF(bounds.Right, bounds.Bottom);
			PointF pointF3 = new PointF(bounds.X + (bounds.Right - bounds.X) / 2, bounds.Bottom - 5);
			return new PointF[3] { pointF, pointF2, pointF3 };
		}
	}

	internal Rectangle Line => new Rectangle(bounds.X + 2, bounds.Y + 2, 2, bounds.Height - 5);

	internal ListViewInsertionMark(ListView listview)
	{
		listview_owner = listview;
	}

	private void UpdateBounds()
	{
		if (index < 0 || index >= listview_owner.Items.Count)
		{
			bounds = Rectangle.Empty;
			return;
		}
		Rectangle rectangle = listview_owner.Items[index].Bounds;
		int x = ((!appears_after_item) ? rectangle.Left : rectangle.Right) - 2;
		int height = rectangle.Height + ThemeEngine.Current.ListViewVerticalSpacing;
		bounds = new Rectangle(x, rectangle.Top, 7, height);
	}

	public int NearestIndex(Point pt)
	{
		double num = double.MaxValue;
		int num2 = -1;
		for (int i = 0; i < listview_owner.Items.Count; i++)
		{
			Point itemLocation = listview_owner.GetItemLocation(i);
			double num3 = Math.Pow(itemLocation.X - pt.X, 2.0) + Math.Pow(itemLocation.Y - pt.Y, 2.0);
			if (num3 < num)
			{
				num = num3;
				num2 = i;
			}
		}
		if (listview_owner.item_control.dragged_item_index == num2)
		{
			return -1;
		}
		return num2;
	}
}
