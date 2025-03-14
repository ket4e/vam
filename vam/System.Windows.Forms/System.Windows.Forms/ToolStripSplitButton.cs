using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
[DefaultEvent("ButtonClick")]
public class ToolStripSplitButton : ToolStripDropDownItem
{
	public class ToolStripSplitButtonAccessibleObject : ToolStripItemAccessibleObject
	{
		public ToolStripSplitButtonAccessibleObject(ToolStripSplitButton item)
			: base(item)
		{
		}

		public override void DoDefaultAction()
		{
			(owner_item as ToolStripSplitButton).PerformButtonClick();
		}
	}

	private bool button_pressed;

	private ToolStripItem default_item;

	private bool drop_down_button_selected;

	private int drop_down_button_width;

	private static object ButtonClickEvent;

	private static object ButtonDoubleClickEvent;

	private static object DefaultItemChangedEvent;

	[DefaultValue(true)]
	public new bool AutoToolTip
	{
		get
		{
			return base.AutoToolTip;
		}
		set
		{
			base.AutoToolTip = value;
		}
	}

	[Browsable(false)]
	public Rectangle ButtonBounds => new Rectangle(Bounds.Left, Bounds.Top, Bounds.Width - drop_down_button_width - 1, base.Height);

	[Browsable(false)]
	public bool ButtonPressed => button_pressed;

	[Browsable(false)]
	public bool ButtonSelected => base.Selected;

	[DefaultValue(null)]
	[Browsable(false)]
	public ToolStripItem DefaultItem
	{
		get
		{
			return default_item;
		}
		set
		{
			if (default_item != value)
			{
				default_item = value;
				OnDefaultItemChanged(EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	public Rectangle DropDownButtonBounds => new Rectangle(Bounds.Right - drop_down_button_width, 0, drop_down_button_width, Bounds.Height);

	[Browsable(false)]
	public bool DropDownButtonPressed => drop_down_button_selected || (HasDropDownItems && base.DropDown.Visible);

	[Browsable(false)]
	public bool DropDownButtonSelected => base.Selected;

	public int DropDownButtonWidth
	{
		get
		{
			return drop_down_button_width;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (drop_down_button_width != value)
			{
				drop_down_button_width = value;
				CalculateAutoSize();
			}
		}
	}

	[Browsable(false)]
	public Rectangle SplitterBounds => new Rectangle(Bounds.Width - drop_down_button_width - 1, 0, 1, base.Height);

	protected override bool DefaultAutoToolTip => true;

	protected internal override bool DismissWhenClicked => true;

	public event EventHandler ButtonClick
	{
		add
		{
			base.Events.AddHandler(ButtonClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ButtonClickEvent, value);
		}
	}

	public event EventHandler ButtonDoubleClick
	{
		add
		{
			base.Events.AddHandler(ButtonDoubleClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ButtonDoubleClickEvent, value);
		}
	}

	public event EventHandler DefaultItemChanged
	{
		add
		{
			base.Events.AddHandler(DefaultItemChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DefaultItemChangedEvent, value);
		}
	}

	public ToolStripSplitButton()
		: this(string.Empty, null, null, string.Empty)
	{
	}

	public ToolStripSplitButton(Image image)
		: this(string.Empty, image, null, string.Empty)
	{
	}

	public ToolStripSplitButton(string text)
		: this(text, null, null, string.Empty)
	{
	}

	public ToolStripSplitButton(string text, Image image)
		: this(text, image, null, string.Empty)
	{
	}

	public ToolStripSplitButton(string text, Image image, EventHandler onClick)
		: this(text, image, onClick, string.Empty)
	{
	}

	public ToolStripSplitButton(string text, Image image, params ToolStripItem[] dropDownItems)
		: base(text, image, dropDownItems)
	{
		ResetDropDownButtonWidth();
	}

	public ToolStripSplitButton(string text, Image image, EventHandler onClick, string name)
		: base(text, image, onClick, name)
	{
		ResetDropDownButtonWidth();
	}

	static ToolStripSplitButton()
	{
		ButtonClick = new object();
		ButtonDoubleClick = new object();
		DefaultItemChanged = new object();
	}

	public override Size GetPreferredSize(Size constrainingSize)
	{
		Size preferredSize = base.GetPreferredSize(constrainingSize);
		if (preferredSize.Width < 23)
		{
			preferredSize.Width = 23;
		}
		if (base.AutoSize)
		{
			preferredSize.Width += drop_down_button_width - 2;
		}
		return preferredSize;
	}

	public virtual void OnButtonDoubleClick(EventArgs e)
	{
		((EventHandler)base.Events[ButtonDoubleClick])?.Invoke(this, e);
	}

	public void PerformButtonClick()
	{
		if (Enabled)
		{
			OnButtonClick(EventArgs.Empty);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void ResetDropDownButtonWidth()
	{
		DropDownButtonWidth = 11;
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new ToolStripSplitButtonAccessibleObject(this);
	}

	protected override ToolStripDropDown CreateDefaultDropDown()
	{
		ToolStripDropDownMenu toolStripDropDownMenu = new ToolStripDropDownMenu();
		toolStripDropDownMenu.OwnerItem = this;
		return toolStripDropDownMenu;
	}

	protected virtual void OnButtonClick(EventArgs e)
	{
		((EventHandler)base.Events[ButtonClick])?.Invoke(this, e);
	}

	protected virtual void OnDefaultItemChanged(EventArgs e)
	{
		((EventHandler)base.Events[DefaultItemChanged])?.Invoke(this, e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (ButtonBounds.Contains(e.Location))
		{
			button_pressed = true;
			Invalidate();
			base.OnMouseDown(e);
		}
		else if (DropDownButtonBounds.Contains(e.Location))
		{
			if (base.DropDown.Visible)
			{
				HideDropDown(ToolStripDropDownCloseReason.ItemClicked);
			}
			else
			{
				ShowDropDown();
			}
			Invalidate();
			base.OnMouseDown(e);
		}
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		drop_down_button_selected = false;
		button_pressed = false;
		Invalidate();
		base.OnMouseLeave(e);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		button_pressed = false;
		Invalidate();
		base.OnMouseUp(e);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (base.Owner != null)
		{
			Color textColor = ((!Enabled) ? SystemColors.GrayText : ForeColor);
			Image image = ((!Enabled) ? ToolStripRenderer.CreateDisabledImage(Image) : Image);
			base.Owner.Renderer.DrawSplitButton(new ToolStripItemRenderEventArgs(e.Graphics, this));
			Rectangle contentRectangle = base.ContentRectangle;
			contentRectangle.Width -= drop_down_button_width + 1;
			CalculateTextAndImageRectangles(contentRectangle, out var text_rect, out var image_rect);
			if (text_rect != Rectangle.Empty)
			{
				base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, textColor, Font, TextAlign));
			}
			if (image_rect != Rectangle.Empty)
			{
				base.Owner.Renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, image, image_rect));
			}
			base.Owner.Renderer.DrawArrow(new ToolStripArrowRenderEventArgs(e.Graphics, this, new Rectangle(base.Width - 9, 1, 6, base.Height), Color.Black, ArrowDirection.Down));
		}
	}

	protected override void OnRightToLeftChanged(EventArgs e)
	{
		base.OnRightToLeftChanged(e);
	}

	protected internal override bool ProcessDialogKey(Keys keyData)
	{
		if (Selected && keyData == Keys.Return && DefaultItem != null)
		{
			DefaultItem.FireEvent(EventArgs.Empty, ToolStripItemEventType.Click);
			return true;
		}
		return base.ProcessDialogKey(keyData);
	}

	protected internal override bool ProcessMnemonic(char charCode)
	{
		if (!Selected)
		{
			base.Parent.ChangeSelection(this);
		}
		if (HasDropDownItems)
		{
			ShowDropDown();
		}
		else
		{
			PerformClick();
		}
		return true;
	}

	internal override void HandleClick(EventArgs e)
	{
		base.HandleClick(e);
		if (e is MouseEventArgs mouseEventArgs && ButtonBounds.Contains(mouseEventArgs.Location))
		{
			OnButtonClick(EventArgs.Empty);
		}
	}
}
