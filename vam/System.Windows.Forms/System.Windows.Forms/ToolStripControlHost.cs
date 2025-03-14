using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripControlHost : ToolStripItem
{
	private Control control;

	private ContentAlignment control_align;

	private bool double_click_enabled;

	private static object EnterEvent;

	private static object GotFocusEvent;

	private static object KeyDownEvent;

	private static object KeyPressEvent;

	private static object KeyUpEvent;

	private static object LeaveEvent;

	private static object LostFocusEvent;

	private static object ValidatedEvent;

	private static object ValidatingEvent;

	public override Color BackColor
	{
		get
		{
			return control.BackColor;
		}
		set
		{
			control.BackColor = value;
		}
	}

	[DefaultValue(null)]
	[Localizable(true)]
	public override Image BackgroundImage
	{
		get
		{
			return base.BackgroundImage;
		}
		set
		{
			base.BackgroundImage = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(ImageLayout.Tile)]
	public override ImageLayout BackgroundImageLayout
	{
		get
		{
			return base.BackgroundImageLayout;
		}
		set
		{
			base.BackgroundImageLayout = value;
		}
	}

	public override bool CanSelect => control.CanSelect;

	[DefaultValue(true)]
	public bool CausesValidation
	{
		get
		{
			return control.CausesValidation;
		}
		set
		{
			control.CausesValidation = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Control Control => control;

	[Browsable(false)]
	[DefaultValue(ContentAlignment.MiddleCenter)]
	public ContentAlignment ControlAlign
	{
		get
		{
			return control_align;
		}
		set
		{
			if (control_align != value)
			{
				if (!Enum.IsDefined(typeof(ContentAlignment), value))
				{
					throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ContentAlignment");
				}
				control_align = value;
				if (control != null)
				{
					control.Bounds = AlignInRectangle(Bounds, control.Size, control_align);
				}
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new ToolStripItemDisplayStyle DisplayStyle
	{
		get
		{
			return base.DisplayStyle;
		}
		set
		{
			base.DisplayStyle = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DefaultValue(false)]
	public new bool DoubleClickEnabled
	{
		get
		{
			return double_click_enabled;
		}
		set
		{
			double_click_enabled = value;
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
			control.Enabled = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public virtual bool Focused => control.Focused;

	public override Font Font
	{
		get
		{
			return control.Font;
		}
		set
		{
			control.Font = value;
		}
	}

	public override Color ForeColor
	{
		get
		{
			return control.ForeColor;
		}
		set
		{
			control.ForeColor = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Image Image
	{
		get
		{
			return base.Image;
		}
		set
		{
			base.Image = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public new ContentAlignment ImageAlign
	{
		get
		{
			return base.ImageAlign;
		}
		set
		{
			base.ImageAlign = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new ToolStripItemImageScaling ImageScaling
	{
		get
		{
			return base.ImageScaling;
		}
		set
		{
			base.ImageScaling = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Color ImageTransparentColor
	{
		get
		{
			return base.ImageTransparentColor;
		}
		set
		{
			base.ImageTransparentColor = value;
		}
	}

	public override RightToLeft RightToLeft
	{
		get
		{
			return base.RightToLeft;
		}
		set
		{
			base.RightToLeft = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool RightToLeftAutoMirrorImage
	{
		get
		{
			return base.RightToLeftAutoMirrorImage;
		}
		set
		{
			base.RightToLeftAutoMirrorImage = value;
		}
	}

	public override bool Selected => base.Selected;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public override ISite Site
	{
		get
		{
			return control.Site;
		}
		set
		{
			control.Site = value;
		}
	}

	public override Size Size
	{
		get
		{
			return base.Size;
		}
		set
		{
			control.Size = value;
			base.Size = value;
			if (base.Owner != null)
			{
				base.Owner.PerformLayout();
			}
		}
	}

	[DefaultValue("")]
	public override string Text
	{
		get
		{
			return control.Text;
		}
		set
		{
			base.Text = value;
			control.Text = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ContentAlignment TextAlign
	{
		get
		{
			return base.TextAlign;
		}
		set
		{
			base.TextAlign = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DefaultValue(ToolStripTextDirection.Horizontal)]
	public override ToolStripTextDirection TextDirection
	{
		get
		{
			return base.TextDirection;
		}
		set
		{
			base.TextDirection = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new TextImageRelation TextImageRelation
	{
		get
		{
			return base.TextImageRelation;
		}
		set
		{
			base.TextImageRelation = value;
		}
	}

	protected override Size DefaultSize
	{
		get
		{
			if (control == null)
			{
				return new Size(23, 23);
			}
			return control.Size;
		}
	}

	internal override ToolStripTextDirection DefaultTextDirection => ToolStripTextDirection.Horizontal;

	internal override bool InternalVisible
	{
		get
		{
			return base.InternalVisible;
		}
		set
		{
			Control.Visible = value;
			base.InternalVisible = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler DisplayStyleChanged
	{
		add
		{
			base.DisplayStyleChanged += value;
		}
		remove
		{
			base.DisplayStyleChanged -= value;
		}
	}

	public event EventHandler Enter
	{
		add
		{
			base.Events.AddHandler(EnterEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(EnterEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public event EventHandler GotFocus
	{
		add
		{
			base.Events.AddHandler(GotFocusEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(GotFocusEvent, value);
		}
	}

	public event KeyEventHandler KeyDown
	{
		add
		{
			base.Events.AddHandler(KeyDownEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(KeyDownEvent, value);
		}
	}

	public event KeyPressEventHandler KeyPress
	{
		add
		{
			base.Events.AddHandler(KeyPressEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(KeyPressEvent, value);
		}
	}

	public event KeyEventHandler KeyUp
	{
		add
		{
			base.Events.AddHandler(KeyUpEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(KeyUpEvent, value);
		}
	}

	public event EventHandler Leave
	{
		add
		{
			base.Events.AddHandler(LeaveEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(LeaveEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public event EventHandler LostFocus
	{
		add
		{
			base.Events.AddHandler(LostFocusEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(LostFocusEvent, value);
		}
	}

	public event EventHandler Validated
	{
		add
		{
			base.Events.AddHandler(ValidatedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ValidatedEvent, value);
		}
	}

	public event CancelEventHandler Validating
	{
		add
		{
			base.Events.AddHandler(ValidatingEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ValidatingEvent, value);
		}
	}

	public ToolStripControlHost(Control c)
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		RightToLeft = RightToLeft.No;
		control = c;
		control_align = ContentAlignment.MiddleCenter;
		control.TabStop = false;
		control.Resize += ControlResizeHandler;
		Size = DefaultSize;
		OnSubscribeControlEvents(control);
	}

	public ToolStripControlHost(Control c, string name)
		: this(c)
	{
		base.Name = name;
	}

	static ToolStripControlHost()
	{
		Enter = new object();
		GotFocus = new object();
		KeyDown = new object();
		KeyPress = new object();
		KeyUp = new object();
		Leave = new object();
		LostFocus = new object();
		Validated = new object();
		Validating = new object();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void Focus()
	{
		control.Focus();
	}

	public override Size GetPreferredSize(Size constrainingSize)
	{
		return control.GetPreferredSize(constrainingSize);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override void ResetBackColor()
	{
		base.ResetBackColor();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override void ResetForeColor()
	{
		base.ResetForeColor();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return control.AccessibilityObject;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (control.Created && !control.IsDisposed)
		{
			control.Dispose();
		}
	}

	protected override void OnBoundsChanged()
	{
		if (control != null)
		{
			control.Bounds = AlignInRectangle(Bounds, control.Size, control_align);
		}
		base.OnBoundsChanged();
	}

	protected virtual void OnEnter(EventArgs e)
	{
		((EventHandler)base.Events[Enter])?.Invoke(this, e);
	}

	protected virtual void OnGotFocus(EventArgs e)
	{
		((EventHandler)base.Events[GotFocus])?.Invoke(this, e);
	}

	private void ControlResizeHandler(object obj, EventArgs args)
	{
		OnHostedControlResize(args);
	}

	protected virtual void OnHostedControlResize(EventArgs e)
	{
		if (control != null)
		{
			control.Location = AlignInRectangle(Bounds, control.Size, control_align).Location;
		}
	}

	protected virtual void OnKeyDown(KeyEventArgs e)
	{
		((KeyEventHandler)base.Events[KeyDown])?.Invoke(this, e);
	}

	protected virtual void OnKeyPress(KeyPressEventArgs e)
	{
		((KeyPressEventHandler)base.Events[KeyPress])?.Invoke(this, e);
	}

	protected virtual void OnKeyUp(KeyEventArgs e)
	{
		((KeyEventHandler)base.Events[KeyUp])?.Invoke(this, e);
	}

	protected override void OnLayout(LayoutEventArgs e)
	{
		base.OnLayout(e);
		if (control != null)
		{
			control.Bounds = AlignInRectangle(Bounds, control.Size, control_align);
		}
	}

	protected virtual void OnLeave(EventArgs e)
	{
		((EventHandler)base.Events[Leave])?.Invoke(this, e);
	}

	protected virtual void OnLostFocus(EventArgs e)
	{
		((EventHandler)base.Events[LostFocus])?.Invoke(this, e);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
	}

	protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
	{
		base.OnParentChanged(oldParent, newParent);
		oldParent?.Controls.Remove(control);
		newParent?.Controls.Add(control);
	}

	protected virtual void OnSubscribeControlEvents(Control control)
	{
		this.control.Enter += HandleEnter;
		this.control.GotFocus += HandleGotFocus;
		this.control.KeyDown += HandleKeyDown;
		this.control.KeyPress += HandleKeyPress;
		this.control.KeyUp += HandleKeyUp;
		this.control.Leave += HandleLeave;
		this.control.LostFocus += HandleLostFocus;
		this.control.Validated += HandleValidated;
		this.control.Validating += HandleValidating;
	}

	protected virtual void OnUnsubscribeControlEvents(Control control)
	{
	}

	protected virtual void OnValidated(EventArgs e)
	{
		((EventHandler)base.Events[Validated])?.Invoke(this, e);
	}

	protected virtual void OnValidating(CancelEventArgs e)
	{
		((CancelEventHandler)base.Events[Validating])?.Invoke(this, e);
	}

	protected internal override bool ProcessCmdKey(ref Message m, Keys keyData)
	{
		return base.ProcessCmdKey(ref m, keyData);
	}

	protected internal override bool ProcessDialogKey(Keys keyData)
	{
		return base.ProcessDialogKey(keyData);
	}

	protected override void SetVisibleCore(bool visible)
	{
		base.SetVisibleCore(visible);
		control.Visible = visible;
		if (control != null)
		{
			control.Bounds = AlignInRectangle(Bounds, control.Size, control_align);
		}
	}

	internal override void Dismiss(ToolStripDropDownCloseReason reason)
	{
		if (Selected)
		{
			base.Parent.Focus();
		}
		base.Dismiss(reason);
	}

	private void HandleEnter(object sender, EventArgs e)
	{
		OnEnter(e);
	}

	private void HandleGotFocus(object sender, EventArgs e)
	{
		OnGotFocus(e);
	}

	private void HandleKeyDown(object sender, KeyEventArgs e)
	{
		OnKeyDown(e);
	}

	private void HandleKeyPress(object sender, KeyPressEventArgs e)
	{
		OnKeyPress(e);
	}

	private void HandleKeyUp(object sender, KeyEventArgs e)
	{
		OnKeyUp(e);
	}

	private void HandleLeave(object sender, EventArgs e)
	{
		OnLeave(e);
	}

	private void HandleLostFocus(object sender, EventArgs e)
	{
		OnLostFocus(e);
	}

	private void HandleValidated(object sender, EventArgs e)
	{
		OnValidated(e);
	}

	private void HandleValidating(object sender, CancelEventArgs e)
	{
		OnValidating(e);
	}
}
