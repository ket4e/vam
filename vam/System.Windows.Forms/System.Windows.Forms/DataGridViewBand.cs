using System.ComponentModel;

namespace System.Windows.Forms;

public class DataGridViewBand : DataGridViewElement, IDisposable, ICloneable
{
	private ContextMenuStrip contextMenuStrip;

	private DataGridViewCellStyle defaultCellStyle;

	private Type defaultHeaderCellType;

	private bool displayed;

	private bool frozen;

	private int index = -1;

	private bool readOnly;

	private DataGridViewTriState resizable;

	private bool selected;

	private object tag;

	private bool visible = true;

	private DataGridViewHeaderCell headerCellCore;

	private bool isRow;

	private DataGridViewCellStyle inheritedStyle;

	[DefaultValue(null)]
	public virtual ContextMenuStrip ContextMenuStrip
	{
		get
		{
			return contextMenuStrip;
		}
		set
		{
			contextMenuStrip = value;
		}
	}

	[Browsable(false)]
	public virtual DataGridViewCellStyle DefaultCellStyle
	{
		get
		{
			if (defaultCellStyle == null)
			{
				defaultCellStyle = new DataGridViewCellStyle();
			}
			return defaultCellStyle;
		}
		set
		{
			defaultCellStyle = value;
		}
	}

	[Browsable(false)]
	public Type DefaultHeaderCellType
	{
		get
		{
			return defaultHeaderCellType;
		}
		set
		{
			if (!value.IsSubclassOf(typeof(DataGridViewHeaderCell)))
			{
				throw new ArgumentException("Type is not DataGridViewHeaderCell or a derived type.");
			}
			defaultHeaderCellType = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public virtual bool Displayed => displayed;

	[DefaultValue(false)]
	public virtual bool Frozen
	{
		get
		{
			return frozen;
		}
		set
		{
			if (frozen != value)
			{
				frozen = value;
				if (frozen)
				{
					SetState(State | DataGridViewElementStates.Frozen);
				}
				else
				{
					SetState(State & ~DataGridViewElementStates.Frozen);
				}
			}
		}
	}

	[Browsable(false)]
	public bool HasDefaultCellStyle => defaultCellStyle != null;

	[Browsable(false)]
	public int Index => index;

	[Browsable(false)]
	public virtual DataGridViewCellStyle InheritedStyle => inheritedStyle;

	[DefaultValue(false)]
	public virtual bool ReadOnly
	{
		get
		{
			return readOnly;
		}
		set
		{
			if (readOnly != value)
			{
				readOnly = value;
				if (readOnly)
				{
					SetState(State | DataGridViewElementStates.ReadOnly);
				}
				else
				{
					SetState(State & ~DataGridViewElementStates.ReadOnly);
				}
			}
		}
	}

	[Browsable(true)]
	public virtual DataGridViewTriState Resizable
	{
		get
		{
			if (resizable == DataGridViewTriState.NotSet && base.DataGridView != null)
			{
				return base.DataGridView.AllowUserToResizeColumns ? DataGridViewTriState.True : DataGridViewTriState.False;
			}
			return resizable;
		}
		set
		{
			if (value != resizable)
			{
				resizable = value;
				if (resizable == DataGridViewTriState.True)
				{
					SetState(State | DataGridViewElementStates.Resizable);
				}
				else
				{
					SetState(State & ~DataGridViewElementStates.Resizable);
				}
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			if (base.DataGridView == null)
			{
				throw new InvalidOperationException("Cant select a row non associated with a DataGridView.");
			}
			if (isRow)
			{
				base.DataGridView.SetSelectedRowCoreInternal(Index, value);
			}
			else
			{
				base.DataGridView.SetSelectedColumnCoreInternal(Index, value);
			}
		}
	}

	internal bool SelectedInternal
	{
		get
		{
			return selected;
		}
		set
		{
			if (selected != value)
			{
				selected = value;
				if (selected)
				{
					SetState(State | DataGridViewElementStates.Selected);
				}
				else
				{
					SetState(State & ~DataGridViewElementStates.Selected);
				}
			}
		}
	}

	internal bool DisplayedInternal
	{
		get
		{
			return displayed;
		}
		set
		{
			if (value != displayed)
			{
				displayed = value;
				if (displayed)
				{
					SetState(State | DataGridViewElementStates.Displayed);
				}
				else
				{
					SetState(State & ~DataGridViewElementStates.Displayed);
				}
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	[DefaultValue(true)]
	public virtual bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			if (visible != value)
			{
				visible = value;
				if (visible)
				{
					SetState(State | DataGridViewElementStates.Visible);
				}
				else
				{
					SetState(State & ~DataGridViewElementStates.Visible);
				}
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	protected DataGridViewHeaderCell HeaderCellCore
	{
		get
		{
			return headerCellCore;
		}
		set
		{
			headerCellCore = value;
		}
	}

	protected bool IsRow => isRow;

	internal DataGridViewBand()
	{
		defaultHeaderCellType = typeof(DataGridViewHeaderCell);
		isRow = this is DataGridViewRow;
	}

	~DataGridViewBand()
	{
		Dispose();
	}

	public virtual object Clone()
	{
		return new DataGridViewBand();
	}

	public void Dispose()
	{
	}

	public override string ToString()
	{
		return GetType().Name + ": " + index + ".";
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	protected override void OnDataGridViewChanged()
	{
	}

	internal virtual void SetIndex(int index)
	{
		this.index = index;
	}
}
