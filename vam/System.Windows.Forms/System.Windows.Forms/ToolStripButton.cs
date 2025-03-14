using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
public class ToolStripButton : ToolStripItem
{
	private CheckState checked_state;

	private bool check_on_click;

	private static object CheckedChangedEvent;

	private static object CheckStateChangedEvent;

	private static object UIACheckOnClickChangedEvent;

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

	public override bool CanSelect => true;

	[DefaultValue(false)]
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
			if (checked_state != (CheckState)(value ? 1 : 0))
			{
				checked_state = (value ? CheckState.Checked : CheckState.Unchecked);
				OnCheckedChanged(EventArgs.Empty);
				OnCheckStateChanged(EventArgs.Empty);
				Invalidate();
			}
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

	[DefaultValue(CheckState.Unchecked)]
	public CheckState CheckState
	{
		get
		{
			return checked_state;
		}
		set
		{
			if (checked_state != value)
			{
				if (!Enum.IsDefined(typeof(CheckState), value))
				{
					throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for CheckState");
				}
				checked_state = value;
				OnCheckedChanged(EventArgs.Empty);
				OnCheckStateChanged(EventArgs.Empty);
				Invalidate();
			}
		}
	}

	protected override bool DefaultAutoToolTip => true;

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

	public ToolStripButton()
		: this(null, null, null, string.Empty)
	{
	}

	public ToolStripButton(Image image)
		: this(null, image, null, string.Empty)
	{
	}

	public ToolStripButton(string text)
		: this(text, null, null, string.Empty)
	{
	}

	public ToolStripButton(string text, Image image)
		: this(text, image, null, string.Empty)
	{
	}

	public ToolStripButton(string text, Image image, EventHandler onClick)
		: this(text, image, onClick, string.Empty)
	{
	}

	public ToolStripButton(string text, Image image, EventHandler onClick, string name)
		: base(text, image, onClick, name)
	{
		checked_state = CheckState.Unchecked;
		base.ToolTipText = string.Empty;
	}

	static ToolStripButton()
	{
		CheckedChanged = new object();
		CheckStateChanged = new object();
		UIACheckOnClickChanged = new object();
	}

	public override Size GetPreferredSize(Size constrainingSize)
	{
		Size preferredSize = base.GetPreferredSize(constrainingSize);
		if (preferredSize.Width < 23)
		{
			preferredSize.Width = 23;
		}
		return preferredSize;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		ToolStripItemAccessibleObject toolStripItemAccessibleObject = new ToolStripItemAccessibleObject(this);
		toolStripItemAccessibleObject.default_action = "Press";
		toolStripItemAccessibleObject.role = AccessibleRole.PushButton;
		toolStripItemAccessibleObject.state = AccessibleStates.Focusable;
		return toolStripItemAccessibleObject;
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
		if (check_on_click)
		{
			Checked = !Checked;
		}
		base.OnClick(e);
		GetTopLevelToolStrip()?.Dismiss(ToolStripDropDownCloseReason.ItemClicked);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (base.Owner != null)
		{
			Color textColor = ((!Enabled) ? SystemColors.GrayText : ForeColor);
			Image image = ((!Enabled) ? ToolStripRenderer.CreateDisabledImage(Image) : Image);
			base.Owner.Renderer.DrawButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
			CalculateTextAndImageRectangles(out var text_rect, out var image_rect);
			if (text_rect != Rectangle.Empty)
			{
				base.Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, text_rect, textColor, Font, TextAlign));
			}
			if (image_rect != Rectangle.Empty)
			{
				base.Owner.Renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, image, image_rect));
			}
		}
	}

	internal void OnUIACheckOnClickChangedEvent(EventArgs args)
	{
		((EventHandler)base.Events[UIACheckOnClickChanged])?.Invoke(this, args);
	}
}
