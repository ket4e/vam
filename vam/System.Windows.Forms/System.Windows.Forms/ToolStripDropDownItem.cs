using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[DefaultProperty("DropDownItems")]
[Designer("System.Windows.Forms.Design.ToolStripMenuItemDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
public abstract class ToolStripDropDownItem : ToolStripItem
{
	internal ToolStripDropDown drop_down;

	private ToolStripDropDownDirection drop_down_direction;

	private static object DropDownClosedEvent;

	private static object DropDownItemClickedEvent;

	private static object DropDownOpenedEvent;

	private static object DropDownOpeningEvent;

	[TypeConverter(typeof(ReferenceConverter))]
	public ToolStripDropDown DropDown
	{
		get
		{
			if (drop_down == null)
			{
				drop_down = CreateDefaultDropDown();
				drop_down.ItemAdded += DropDown_ItemAdded;
			}
			return drop_down;
		}
		set
		{
			drop_down = value;
			drop_down.OwnerItem = this;
		}
	}

	[Browsable(false)]
	public ToolStripDropDownDirection DropDownDirection
	{
		get
		{
			return drop_down_direction;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ToolStripDropDownDirection), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ToolStripDropDownDirection");
			}
			drop_down_direction = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public ToolStripItemCollection DropDownItems => DropDown.Items;

	[Browsable(false)]
	public virtual bool HasDropDownItems => drop_down != null && DropDown.Items.Count != 0;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override bool Pressed => base.Pressed || (drop_down != null && DropDown.Visible);

	protected internal virtual Point DropDownLocation
	{
		get
		{
			Point result;
			if (base.IsOnDropDown)
			{
				result = base.Parent.PointToScreen(new Point(Bounds.Left, Bounds.Top - 1));
				result.X += Bounds.Width;
				result.Y += Bounds.Left;
				return result;
			}
			result = new Point(Bounds.Left, Bounds.Bottom - 1);
			return base.Parent.PointToScreen(result);
		}
	}

	public event EventHandler DropDownClosed
	{
		add
		{
			base.Events.AddHandler(DropDownClosedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DropDownClosedEvent, value);
		}
	}

	public event ToolStripItemClickedEventHandler DropDownItemClicked
	{
		add
		{
			base.Events.AddHandler(DropDownItemClickedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DropDownItemClickedEvent, value);
		}
	}

	public event EventHandler DropDownOpened
	{
		add
		{
			base.Events.AddHandler(DropDownOpenedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DropDownOpenedEvent, value);
		}
	}

	public event EventHandler DropDownOpening
	{
		add
		{
			base.Events.AddHandler(DropDownOpeningEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DropDownOpeningEvent, value);
		}
	}

	protected ToolStripDropDownItem()
		: this(string.Empty, null, null, string.Empty)
	{
	}

	protected ToolStripDropDownItem(string text, Image image, EventHandler onClick)
		: this(text, image, onClick, string.Empty)
	{
	}

	protected ToolStripDropDownItem(string text, Image image, params ToolStripItem[] dropDownItems)
		: this(text, image, null, string.Empty)
	{
	}

	protected ToolStripDropDownItem(string text, Image image, EventHandler onClick, string name)
		: base(text, image, onClick, name)
	{
	}

	static ToolStripDropDownItem()
	{
		DropDownClosed = new object();
		DropDownItemClicked = new object();
		DropDownOpened = new object();
		DropDownOpening = new object();
	}

	public void HideDropDown()
	{
		if (drop_down != null && DropDown.Visible)
		{
			OnDropDownHide(EventArgs.Empty);
			DropDown.Close(ToolStripDropDownCloseReason.CloseCalled);
			is_pressed = false;
			Invalidate();
		}
	}

	public void ShowDropDown()
	{
		if (!DropDown.Visible)
		{
			OnDropDownShow(EventArgs.Empty);
			if (HasDropDownItems)
			{
				Invalidate();
				DropDown.Show(DropDownLocation);
			}
		}
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new ToolStripDropDownItemAccessibleObject(this);
	}

	protected virtual ToolStripDropDown CreateDefaultDropDown()
	{
		ToolStripDropDown toolStripDropDown = new ToolStripDropDown();
		toolStripDropDown.OwnerItem = this;
		return toolStripDropDown;
	}

	protected override void Dispose(bool disposing)
	{
		if (base.IsDisposed)
		{
			return;
		}
		if (HasDropDownItems)
		{
			foreach (ToolStripItem dropDownItem in DropDownItems)
			{
				if (dropDownItem is ToolStripMenuItem)
				{
					ToolStripManager.RemoveToolStripMenuItem((ToolStripMenuItem)dropDownItem);
				}
			}
		}
		if (drop_down != null)
		{
			ToolStripManager.RemoveToolStrip(drop_down);
		}
		base.Dispose(disposing);
	}

	protected override void OnBoundsChanged()
	{
		base.OnBoundsChanged();
	}

	protected internal virtual void OnDropDownClosed(EventArgs e)
	{
		((EventHandler)base.Events[DropDownClosed])?.Invoke(this, e);
	}

	protected virtual void OnDropDownHide(EventArgs e)
	{
	}

	protected internal virtual void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
	{
		((ToolStripItemClickedEventHandler)base.Events[DropDownItemClicked])?.Invoke(this, e);
	}

	protected internal virtual void OnDropDownOpened(EventArgs e)
	{
		((EventHandler)base.Events[DropDownOpened])?.Invoke(this, e);
	}

	protected virtual void OnDropDownShow(EventArgs e)
	{
		((EventHandler)base.Events[DropDownOpening])?.Invoke(this, e);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		if (drop_down != null)
		{
			drop_down.Font = Font;
		}
	}

	protected override void OnRightToLeftChanged(EventArgs e)
	{
		base.OnRightToLeftChanged(e);
	}

	protected internal override bool ProcessCmdKey(ref Message m, Keys keyData)
	{
		if (HasDropDownItems)
		{
			foreach (ToolStripItem dropDownItem in DropDownItems)
			{
				if (dropDownItem.ProcessCmdKey(ref m, keyData))
				{
					return true;
				}
			}
		}
		return base.ProcessCmdKey(ref m, keyData);
	}

	protected internal override bool ProcessDialogKey(Keys keyData)
	{
		if (!Selected || !HasDropDownItems)
		{
			return base.ProcessDialogKey(keyData);
		}
		if (!base.IsOnDropDown)
		{
			if (base.Parent.Orientation == Orientation.Horizontal)
			{
				if (keyData == Keys.Down || keyData == Keys.Return)
				{
					if (base.Parent is MenuStrip)
					{
						(base.Parent as MenuStrip).MenuDroppedDown = true;
					}
					ShowDropDown();
					DropDown.SelectNextToolStripItem(null, forward: true);
					return true;
				}
			}
			else if (keyData == Keys.Right || keyData == Keys.Return)
			{
				if (base.Parent is MenuStrip)
				{
					(base.Parent as MenuStrip).MenuDroppedDown = true;
				}
				ShowDropDown();
				DropDown.SelectNextToolStripItem(null, forward: true);
				return true;
			}
		}
		else if ((keyData == Keys.Right || keyData == Keys.Return) && HasDropDownItems)
		{
			ShowDropDown();
			DropDown.SelectNextToolStripItem(null, forward: true);
			return true;
		}
		return base.ProcessDialogKey(keyData);
	}

	internal override void Dismiss(ToolStripDropDownCloseReason reason)
	{
		if (HasDropDownItems && DropDown.Visible)
		{
			DropDown.Dismiss(reason);
		}
		base.Dismiss(reason);
	}

	internal override void HandleClick(EventArgs e)
	{
		OnClick(e);
	}

	internal void HideDropDown(ToolStripDropDownCloseReason reason)
	{
		if (drop_down != null && DropDown.Visible)
		{
			OnDropDownHide(EventArgs.Empty);
			DropDown.Close(reason);
			is_pressed = false;
			Invalidate();
		}
	}

	private void DropDown_ItemAdded(object sender, ToolStripItemEventArgs e)
	{
		e.Item.owner_item = this;
	}
}
