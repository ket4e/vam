using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;

namespace System.Windows.Forms;

public class DataGridViewLinkCell : DataGridViewCell
{
	protected class DataGridViewLinkCellAccessibleObject : DataGridViewCellAccessibleObject
	{
		public override string DefaultAction => "Click";

		public DataGridViewLinkCellAccessibleObject(DataGridViewCell owner)
			: base(owner)
		{
		}

		[System.MonoTODO("Stub, does nothing")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public override void DoDefaultAction()
		{
		}

		public override int GetChildCount()
		{
			return -1;
		}
	}

	private Color activeLinkColor;

	private LinkBehavior linkBehavior;

	private Color linkColor;

	private bool linkVisited;

	private Cursor parent_cursor;

	private bool trackVisitedState;

	private bool useColumnTextForLinkValue;

	private Color visited_link_color;

	private LinkState linkState;

	public Color ActiveLinkColor
	{
		get
		{
			return activeLinkColor;
		}
		set
		{
			activeLinkColor = value;
		}
	}

	[DefaultValue(LinkBehavior.SystemDefault)]
	public LinkBehavior LinkBehavior
	{
		get
		{
			return linkBehavior;
		}
		set
		{
			linkBehavior = value;
		}
	}

	public Color LinkColor
	{
		get
		{
			return linkColor;
		}
		set
		{
			linkColor = value;
		}
	}

	public bool LinkVisited
	{
		get
		{
			return linkVisited;
		}
		set
		{
			linkVisited = value;
		}
	}

	[DefaultValue(true)]
	public bool TrackVisitedState
	{
		get
		{
			return trackVisitedState;
		}
		set
		{
			trackVisitedState = value;
		}
	}

	[DefaultValue(false)]
	public bool UseColumnTextForLinkValue
	{
		get
		{
			return useColumnTextForLinkValue;
		}
		set
		{
			useColumnTextForLinkValue = value;
		}
	}

	public Color VisitedLinkColor
	{
		get
		{
			return visited_link_color;
		}
		set
		{
			visited_link_color = value;
		}
	}

	public override Type ValueType => (base.ValueType != null) ? base.ValueType : typeof(object);

	public override Type EditType => null;

	public override Type FormattedValueType => typeof(string);

	public DataGridViewLinkCell()
	{
		activeLinkColor = Color.Red;
		linkColor = Color.FromArgb(0, 0, 255);
		trackVisitedState = true;
		visited_link_color = Color.FromArgb(128, 0, 128);
	}

	public override object Clone()
	{
		DataGridViewLinkCell dataGridViewLinkCell = (DataGridViewLinkCell)base.Clone();
		dataGridViewLinkCell.activeLinkColor = activeLinkColor;
		dataGridViewLinkCell.linkColor = linkColor;
		dataGridViewLinkCell.linkVisited = linkVisited;
		dataGridViewLinkCell.linkBehavior = linkBehavior;
		dataGridViewLinkCell.visited_link_color = visited_link_color;
		dataGridViewLinkCell.trackVisitedState = trackVisitedState;
		return dataGridViewLinkCell;
	}

	public override string ToString()
	{
		return $"DataGridViewLinkCell {{ ColumnIndex={base.ColumnIndex}, RowIndex={base.RowIndex} }}";
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new DataGridViewLinkCellAccessibleObject(this);
	}

	protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return Rectangle.Empty;
		}
		object formattedValue = base.FormattedValue;
		Size empty = Size.Empty;
		if (formattedValue != null)
		{
			empty = DataGridViewCell.MeasureTextSize(graphics, formattedValue.ToString(), cellStyle.Font, TextFormatFlags.Left);
			empty.Height += 3;
			return new Rectangle(1, (base.OwningRow.Height - empty.Height) / 2 - 1, empty.Width, empty.Height);
		}
		return new Rectangle(1, 10, 0, 0);
	}

	protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null || string.IsNullOrEmpty(base.ErrorText))
		{
			return Rectangle.Empty;
		}
		Size size = new Size(12, 11);
		return new Rectangle(new Point(base.Size.Width - size.Width - 5, (base.Size.Height - size.Height) / 2), size);
	}

	protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
	{
		object formattedValue = base.FormattedValue;
		if (formattedValue != null)
		{
			Size result = DataGridViewCell.MeasureTextSize(graphics, formattedValue.ToString(), cellStyle.Font, TextFormatFlags.Left);
			result.Height = Math.Max(result.Height, 20);
			result.Width += 4;
			return result;
		}
		return new Size(21, 20);
	}

	protected override object GetValue(int rowIndex)
	{
		if (useColumnTextForLinkValue)
		{
			return (base.OwningColumn as DataGridViewLinkColumn).Text;
		}
		return base.GetValue(rowIndex);
	}

	protected override bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
	{
		if (e.KeyCode != Keys.Space && trackVisitedState && !linkVisited && !e.Shift && !e.Control && !e.Alt)
		{
			return true;
		}
		return false;
	}

	protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
	{
		return true;
	}

	protected override bool MouseLeaveUnsharesRow(int rowIndex)
	{
		return linkState != LinkState.Normal;
	}

	protected override bool MouseMoveUnsharesRow(DataGridViewCellMouseEventArgs e)
	{
		if (linkState == LinkState.Hover)
		{
			return true;
		}
		return false;
	}

	protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
	{
		return linkState == LinkState.Hover;
	}

	protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
	{
		if ((e.KeyData & Keys.Space) == Keys.Space)
		{
			linkState = LinkState.Normal;
			base.DataGridView.InvalidateCell(this);
		}
	}

	protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
	{
		base.OnMouseDown(e);
		linkState = LinkState.Active;
		base.DataGridView.InvalidateCell(this);
	}

	protected override void OnMouseLeave(int rowIndex)
	{
		base.OnMouseLeave(rowIndex);
		linkState = LinkState.Normal;
		base.DataGridView.InvalidateCell(this);
		base.DataGridView.Cursor = parent_cursor;
	}

	protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
	{
		base.OnMouseMove(e);
		if (linkState != LinkState.Hover)
		{
			linkState = LinkState.Hover;
			base.DataGridView.InvalidateCell(this);
			parent_cursor = base.DataGridView.Cursor;
			base.DataGridView.Cursor = Cursors.Hand;
		}
	}

	protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
	{
		base.OnMouseUp(e);
		linkState = LinkState.Hover;
		LinkVisited = true;
		base.DataGridView.InvalidateCell(this);
	}

	protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
	}

	internal override void PaintPartContent(Graphics graphics, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, DataGridViewCellStyle cellStyle, object formattedValue)
	{
		Font font = cellStyle.Font;
		switch (LinkBehavior)
		{
		case LinkBehavior.SystemDefault:
		case LinkBehavior.AlwaysUnderline:
			font = new Font(font, FontStyle.Underline);
			break;
		case LinkBehavior.HoverUnderline:
			if (linkState == LinkState.Hover)
			{
				font = new Font(font, FontStyle.Underline);
			}
			break;
		}
		Color foreColor = ((linkState == LinkState.Active) ? ActiveLinkColor : ((!linkVisited) ? LinkColor : VisitedLinkColor));
		TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis;
		cellBounds.Height -= 2;
		cellBounds.Width -= 2;
		if (formattedValue != null)
		{
			TextRenderer.DrawText(graphics, formattedValue.ToString(), font, cellBounds, foreColor, flags);
		}
	}
}
