using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ToolboxBitmap("")]
public class DataGridViewLinkColumn : DataGridViewColumn
{
	private string text = string.Empty;

	public Color ActiveLinkColor
	{
		get
		{
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			return dataGridViewLinkCell.ActiveLinkColor;
		}
		set
		{
			if (ActiveLinkColor == value)
			{
				return;
			}
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			dataGridViewLinkCell.ActiveLinkColor = value;
			foreach (DataGridViewRow item in (IEnumerable)base.DataGridView.Rows)
			{
				if (item.Cells[base.Index] is DataGridViewLinkCell dataGridViewLinkCell2)
				{
					dataGridViewLinkCell2.ActiveLinkColor = value;
				}
			}
			base.DataGridView.InvalidateColumn(base.Index);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override DataGridViewCell CellTemplate
	{
		get
		{
			return base.CellTemplate;
		}
		set
		{
			base.CellTemplate = value as DataGridViewLinkCell;
		}
	}

	[DefaultValue(LinkBehavior.SystemDefault)]
	public LinkBehavior LinkBehavior
	{
		get
		{
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			return dataGridViewLinkCell.LinkBehavior;
		}
		set
		{
			if (LinkBehavior == value)
			{
				return;
			}
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			dataGridViewLinkCell.LinkBehavior = value;
			foreach (DataGridViewRow item in (IEnumerable)base.DataGridView.Rows)
			{
				if (item.Cells[base.Index] is DataGridViewLinkCell dataGridViewLinkCell2)
				{
					dataGridViewLinkCell2.LinkBehavior = value;
				}
			}
			base.DataGridView.InvalidateColumn(base.Index);
		}
	}

	public Color LinkColor
	{
		get
		{
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			return dataGridViewLinkCell.LinkColor;
		}
		set
		{
			if (LinkColor == value)
			{
				return;
			}
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			dataGridViewLinkCell.LinkColor = value;
			foreach (DataGridViewRow item in (IEnumerable)base.DataGridView.Rows)
			{
				if (item.Cells[base.Index] is DataGridViewLinkCell dataGridViewLinkCell2)
				{
					dataGridViewLinkCell2.LinkColor = value;
				}
			}
			base.DataGridView.InvalidateColumn(base.Index);
		}
	}

	[System.MonoInternalNote("")]
	[DefaultValue(null)]
	public string Text
	{
		get
		{
			if (!(CellTemplate is DataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			return text;
		}
		set
		{
			if (!(Text == value))
			{
				if (!(CellTemplate is DataGridViewLinkCell))
				{
					throw new InvalidOperationException("CellTemplate is null when getting this property.");
				}
				text = value;
				base.DataGridView.InvalidateColumn(base.Index);
			}
		}
	}

	[DefaultValue(true)]
	public bool TrackVisitedState
	{
		get
		{
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			return dataGridViewLinkCell.TrackVisitedState;
		}
		set
		{
			if (TrackVisitedState == value)
			{
				return;
			}
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			dataGridViewLinkCell.TrackVisitedState = value;
			foreach (DataGridViewRow item in (IEnumerable)base.DataGridView.Rows)
			{
				if (item.Cells[base.Index] is DataGridViewLinkCell dataGridViewLinkCell2)
				{
					dataGridViewLinkCell2.TrackVisitedState = value;
				}
			}
			base.DataGridView.InvalidateColumn(base.Index);
		}
	}

	[DefaultValue(false)]
	public bool UseColumnTextForLinkValue
	{
		get
		{
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			return dataGridViewLinkCell.UseColumnTextForLinkValue;
		}
		set
		{
			if (UseColumnTextForLinkValue == value)
			{
				return;
			}
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			dataGridViewLinkCell.UseColumnTextForLinkValue = value;
			foreach (DataGridViewRow item in (IEnumerable)base.DataGridView.Rows)
			{
				if (item.Cells[base.Index] is DataGridViewLinkCell dataGridViewLinkCell2)
				{
					dataGridViewLinkCell2.UseColumnTextForLinkValue = value;
				}
			}
			base.DataGridView.InvalidateColumn(base.Index);
		}
	}

	public Color VisitedLinkColor
	{
		get
		{
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			return dataGridViewLinkCell.VisitedLinkColor;
		}
		set
		{
			if (VisitedLinkColor == value)
			{
				return;
			}
			if (!(CellTemplate is DataGridViewLinkCell dataGridViewLinkCell))
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			dataGridViewLinkCell.VisitedLinkColor = value;
			foreach (DataGridViewRow item in (IEnumerable)base.DataGridView.Rows)
			{
				if (item.Cells[base.Index] is DataGridViewLinkCell dataGridViewLinkCell2)
				{
					dataGridViewLinkCell2.VisitedLinkColor = value;
				}
			}
			base.DataGridView.InvalidateColumn(base.Index);
		}
	}

	public DataGridViewLinkColumn()
	{
		base.CellTemplate = new DataGridViewLinkCell();
	}

	public override object Clone()
	{
		DataGridViewLinkColumn dataGridViewLinkColumn = (DataGridViewLinkColumn)base.Clone();
		dataGridViewLinkColumn.CellTemplate = (DataGridViewCell)CellTemplate.Clone();
		return dataGridViewLinkColumn;
	}

	public override string ToString()
	{
		return base.ToString();
	}
}
