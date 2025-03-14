using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
[DesignerSerializer("System.Windows.Forms.Design.ToolStripMenuItemCodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public class ToolStripMenuItem : ToolStripDropDownItem
{
	private class ToolStripMenuItemAccessibleObject : AccessibleObject
	{
	}

	private CheckState checked_state;

	private bool check_on_click;

	private bool close_on_mouse_release;

	private string shortcut_display_string;

	private Keys shortcut_keys;

	private bool show_shortcut_keys = true;

	private Form mdi_client_form;

	private static object CheckedChangedEvent;

	private static object CheckStateChangedEvent;

	private static object UIACheckOnClickChangedEvent;

	[Bindable(true)]
	[DefaultValue(false)]
	[RefreshProperties(RefreshProperties.All)]
	public bool Checked
	{
		get
		{
			switch (checked_state)
			{
			default:
				return false;
			case CheckState.Checked:
			case CheckState.Indeterminate:
				return true;
			}
		}
		set
		{
			CheckState = (value ? CheckState.Checked : CheckState.Unchecked);
		}
	}

	[DefaultValue(false)]
	public bool CheckOnClick
	{
		get
		{
			return check_on_click;
		}
		set
		{
			if (check_on_click != value)
			{
				check_on_click = value;
				OnUIACheckOnClickChangedEvent(EventArgs.Empty);
			}
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	[Bindable(true)]
	[DefaultValue(CheckState.Unchecked)]
	public CheckState CheckState
	{
		get
		{
			return checked_state;
		}
		set
		{
			if (!Enum.IsDefined(typeof(CheckState), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for CheckState");
			}
			if (value != checked_state)
			{
				checked_state = value;
				Invalidate();
				OnCheckedChanged(EventArgs.Empty);
				OnCheckStateChanged(EventArgs.Empty);
			}
		}
	}

	public override bool Enabled
	{
		get
		{
			return base.Enabled;
		}
		set
		{
			base.Enabled = value;
		}
	}

	[Browsable(false)]
	public bool IsMdiWindowListEntry => mdi_client_form != null;

	[DefaultValue(ToolStripItemOverflow.Never)]
	public new ToolStripItemOverflow Overflow
	{
		get
		{
			return base.Overflow;
		}
		set
		{
			base.Overflow = value;
		}
	}

	[DefaultValue(true)]
	[Localizable(true)]
	public bool ShowShortcutKeys
	{
		get
		{
			return show_shortcut_keys;
		}
		set
		{
			show_shortcut_keys = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(null)]
	public string ShortcutKeyDisplayString
	{
		get
		{
			return shortcut_display_string;
		}
		set
		{
			shortcut_display_string = value;
		}
	}

	[DefaultValue(Keys.None)]
	[Localizable(true)]
	public Keys ShortcutKeys
	{
		get
		{
			return shortcut_keys;
		}
		set
		{
			if (shortcut_keys != value)
			{
				shortcut_keys = value;
				if (base.Parent != null)
				{
					ToolStripManager.AddToolStripMenuItem(this);
				}
			}
		}
	}

	protected internal override Padding DefaultMargin => new Padding(0);

	protected override Padding DefaultPadding => new Padding(4, 0, 4, 0);

	protected override Size DefaultSize => new Size(32, 19);

	internal Form MdiClientForm
	{
		get
		{
			return mdi_client_form;
		}
		set
		{
			mdi_client_form = value;
		}
	}

	public event EventHandler CheckedChanged
	{
		add
		{
			base.Events.AddHandler(CheckedChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CheckedChangedEvent, value);
		}
	}

	public event EventHandler CheckStateChanged
	{
		add
		{
			base.Events.AddHandler(CheckStateChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CheckStateChangedEvent, value);
		}
	}

	internal event EventHandler UIACheckOnClickChanged
	{
		add
		{
			base.Events.AddHandler(UIACheckOnClickChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIACheckOnClickChangedEvent, value);
		}
	}

	public ToolStripMenuItem()
		: this(null, null, null, string.Empty)
	{
	}

	public ToolStripMenuItem(Image image)
		: this(null, image, null, string.Empty)
	{
	}

	public ToolStripMenuItem(string text)
		: this(text, null, null, string.Empty)
	{
	}

	public ToolStripMenuItem(string text, Image image)
		: this(text, image, null, string.Empty)
	{
	}

	public ToolStripMenuItem(string text, Image image, EventHandler onClick)
		: this(text, image, onClick, string.Empty)
	{
	}

	public ToolStripMenuItem(string text, Image image, params ToolStripItem[] dropDownItems)
		: this(text, image, null, string.Empty)
	{
		if (dropDownItems != null)
		{
			foreach (ToolStripItem value in dropDownItems)
			{
				base.DropDownItems.Add(value);
			}
		}
	}

	public ToolStripMenuItem(string text, Image image, EventHandler onClick, Keys shortcutKeys)
		: this(text, image, onClick, string.Empty)
	{
	}

	public ToolStripMenuItem(string text, Image image, EventHandler onClick, string name)
		: base(text, image, onClick, name)
	{
		base.Overflow = ToolStripItemOverflow.Never;
	}

	static ToolStripMenuItem()
	{
		CheckedChanged = new object();
		CheckStateChanged = new object();
		UIACheckOnClickChanged = new object();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new ToolStripMenuItemAccessibleObject();
	}

	protected override ToolStripDropDown CreateDefaultDropDown()
	{
		ToolStripDropDownMenu toolStripDropDownMenu = new ToolStripDropDownMenu();
		toolStripDropDownMenu.OwnerItem = this;
		return toolStripDropDownMenu;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected virtual void OnCheckedChanged(EventArgs e)
	{
		((EventHandler)base.Events[CheckedChanged])?.Invoke(this, e);
	}

	protected virtual void OnCheckStateChanged(EventArgs e)
	{
		((EventHandler)base.Events[CheckStateChanged])?.Invoke(this, e);
	}

	protected override void OnClick(EventArgs e)
	{
		if (!Enabled)
		{
			return;
		}
		if (HasDropDownItems)
		{
			base.OnClick(e);
			return;
		}
		if (base.OwnerItem is ToolStripDropDownItem)
		{
			(base.OwnerItem as ToolStripDropDownItem).OnDropDownItemClicked(new ToolStripItemClickedEventArgs(this));
		}
		if (base.IsOnDropDown)
		{
			GetTopLevelToolStrip()?.Dismiss(ToolStripDropDownCloseReason.ItemClicked);
		}
		if (IsMdiWindowListEntry)
		{
			mdi_client_form.MdiParent.MdiContainer.ActivateChild(mdi_client_form);
			return;
		}
		if (check_on_click)
		{
			Checked = !Checked;
		}
		base.OnClick(e);
		if (!base.IsOnDropDown && !HasDropDownItems)
		{
			GetTopLevelToolStrip()?.Dismiss(ToolStripDropDownCloseReason.ItemClicked);
		}
	}

	protected override void OnDropDownHide(EventArgs e)
	{
		base.OnDropDownHide(e);
	}

	protected override void OnDropDownShow(EventArgs e)
	{
		base.OnDropDownShow(e);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (!base.IsOnDropDown && HasDropDownItems && base.DropDown.Visible)
		{
			close_on_mouse_release = true;
		}
		if (Enabled && !base.DropDown.Visible)
		{
			ShowDropDown();
		}
		base.OnMouseDown(e);
	}

	protected override void OnMouseEnter(EventArgs e)
	{
		if (base.IsOnDropDown && HasDropDownItems && Enabled)
		{
			ShowDropDown();
		}
		base.OnMouseEnter(e);
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		if (close_on_mouse_release)
		{
			base.DropDown.Dismiss(ToolStripDropDownCloseReason.ItemClicked);
			Invalidate();
			close_on_mouse_release = false;
			if (!base.IsOnDropDown && base.Parent is MenuStrip)
			{
				(base.Parent as MenuStrip).MenuDroppedDown = false;
			}
		}
		if (!HasDropDownItems && Enabled)
		{
			base.OnMouseUp(e);
		}
	}

	protected override void OnOwnerChanged(EventArgs e)
	{
		base.OnOwnerChanged(e);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (base.Owner == null)
		{
			return;
		}
		Image image = ((!base.UseImageMargin) ? null : Image);
		Color color = ForeColor;
		if ((Selected || Pressed) && base.IsOnDropDown && color == SystemColors.MenuText)
		{
			color = SystemColors.HighlightText;
		}
		if (!Enabled && ForeColor == SystemColors.ControlText)
		{
			color = SystemColors.GrayText;
		}
		image = ((!Enabled) ? ToolStripRenderer.CreateDisabledImage(image) : image);
		base.Owner.Renderer.DrawMenuItemBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
		CalculateTextAndImageRectangles(out var text_rect, out var image_rect);
		if (base.IsOnDropDown)
		{
			if (!base.UseImageMargin)
			{
				image_rect = Rectangle.Empty;
				text_rect = new Rectangle(8, text_rect.Top, text_rect.Width, text_rect.Height);
			}
			else
			{
				text_rect = new Rectangle(35, text_rect.Top, text_rect.Width, text_rect.Height);
				if (image_rect != Rectangle.Empty)
				{
					image_rect = new Rectangle(new Point(4, 3), GetImageSize());
				}
			}
			if (Checked && base.ShowMargin)
			{
				base.Owner.Renderer.DrawItemCheck(new ToolStripItemImageRenderEventArgs(e.Graphics, this, new Rectangle(2, 1, 19, 19)));
			}
		}
		if (text_rect != Rectangle.Empty)
		{
			base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, color, Font, TextAlign));
		}
		string shortcutDisplayString = GetShortcutDisplayString();
		if (!string.IsNullOrEmpty(shortcutDisplayString) && !HasDropDownItems)
		{
			int num = 15;
			Size size = TextRenderer.MeasureText(shortcutDisplayString, Font);
			Rectangle textRectangle = new Rectangle(base.ContentRectangle.Right - size.Width - num, text_rect.Top, size.Width, text_rect.Height);
			base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, shortcutDisplayString, textRectangle, color, Font, TextAlign));
		}
		if (image_rect != Rectangle.Empty)
		{
			base.Owner.Renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, image, image_rect));
		}
		if (base.IsOnDropDown && HasDropDownItems && base.Parent is ToolStripDropDownMenu)
		{
			base.Owner.Renderer.DrawArrow(new ToolStripArrowRenderEventArgs(e.Graphics, this, new Rectangle(Bounds.Width - 17, 2, 10, 20), Color.Black, ArrowDirection.Right));
		}
	}

	protected internal override bool ProcessCmdKey(ref Message m, Keys keyData)
	{
		Control control = Control.FromHandle(m.HWnd);
		Form form = ((control != null) ? ((Form)control.TopLevelControl) : null);
		if (Enabled && keyData == shortcut_keys && GetTopLevelControl() == form)
		{
			FireEvent(EventArgs.Empty, ToolStripItemEventType.Click);
			return true;
		}
		return base.ProcessCmdKey(ref m, keyData);
	}

	private Control GetTopLevelControl()
	{
		ToolStripItem toolStripItem = this;
		while (toolStripItem.OwnerItem != null)
		{
			toolStripItem = toolStripItem.OwnerItem;
		}
		if (toolStripItem.Owner == null)
		{
			return null;
		}
		if (toolStripItem.Owner is ContextMenuStrip)
		{
			return ((ContextMenuStrip)toolStripItem.Owner).container?.TopLevelControl;
		}
		return toolStripItem.Owner.TopLevelControl;
	}

	protected internal override bool ProcessMnemonic(char charCode)
	{
		if (!Selected)
		{
			base.Parent.ChangeSelection(this);
		}
		if (HasDropDownItems)
		{
			ToolStripManager.SetActiveToolStrip(base.Parent, keyboard: true);
			ShowDropDown();
			base.DropDown.SelectNextToolStripItem(null, forward: true);
		}
		else
		{
			PerformClick();
		}
		return true;
	}

	protected internal override void SetBounds(Rectangle rect)
	{
		base.SetBounds(rect);
	}

	internal void OnUIACheckOnClickChangedEvent(EventArgs args)
	{
		((EventHandler)base.Events[UIACheckOnClickChanged])?.Invoke(this, args);
	}

	internal override Size CalculatePreferredSize(Size constrainingSize)
	{
		Size result = base.CalculatePreferredSize(constrainingSize);
		string shortcutDisplayString = GetShortcutDisplayString();
		if (string.IsNullOrEmpty(shortcutDisplayString))
		{
			return result;
		}
		Size size = TextRenderer.MeasureText(shortcutDisplayString, Font);
		return new Size(result.Width + size.Width - 25, result.Height);
	}

	internal string GetShortcutDisplayString()
	{
		if (!show_shortcut_keys)
		{
			return string.Empty;
		}
		if (base.Parent == null || !(base.Parent is ToolStripDropDownMenu))
		{
			return string.Empty;
		}
		string result = string.Empty;
		if (!string.IsNullOrEmpty(shortcut_display_string))
		{
			result = shortcut_display_string;
		}
		else if (shortcut_keys != 0)
		{
			KeysConverter keysConverter = new KeysConverter();
			result = keysConverter.ConvertToString(shortcut_keys);
		}
		return result;
	}

	internal void HandleAutoExpansion()
	{
		if (HasDropDownItems)
		{
			ShowDropDown();
			base.DropDown.SelectNextToolStripItem(null, forward: true);
		}
	}

	internal override void HandleClick(EventArgs e)
	{
		OnClick(e);
		if (base.Parent != null)
		{
			base.Parent.Invalidate();
		}
	}
}
