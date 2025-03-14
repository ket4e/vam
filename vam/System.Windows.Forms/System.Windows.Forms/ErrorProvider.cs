using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ProvideProperty("Error", "System.Windows.Forms.Control, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[ToolboxItemFilter("System.Windows.Forms")]
[ProvideProperty("IconAlignment", "System.Windows.Forms.Control, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[ProvideProperty("IconPadding", "System.Windows.Forms.Control, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[ComplexBindingProperties("DataSource", "DataMember")]
public class ErrorProvider : Component, ISupportInitialize, IExtenderProvider
{
	private class ErrorWindow : UserControl
	{
		public ErrorWindow()
		{
			SetStyle(ControlStyles.Selectable, value: false);
		}
	}

	private class ErrorProperty
	{
		public ErrorIconAlignment alignment;

		public int padding;

		public string text;

		public Control control;

		public ErrorProvider ep;

		private ErrorWindow window;

		private bool visible;

		private int blink_count;

		private EventHandler tick;

		private Timer timer;

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				bool flag = text != value;
				text = value;
				if (text != string.Empty)
				{
					window.Visible = true;
					if (flag || ep.blinkstyle == ErrorBlinkStyle.AlwaysBlink)
					{
						if (timer == null)
						{
							timer = new Timer();
							timer.Tick += tick;
						}
						timer.Interval = ep.blinkrate;
						blink_count = 0;
						timer.Enabled = true;
					}
				}
				else
				{
					window.Visible = false;
				}
			}
		}

		public ErrorIconAlignment Alignment
		{
			get
			{
				return alignment;
			}
			set
			{
				if (alignment != value)
				{
					alignment = value;
					CalculateAlignment();
				}
			}
		}

		public int Padding
		{
			get
			{
				return padding;
			}
			set
			{
				if (padding != value)
				{
					padding = value;
					CalculateAlignment();
				}
			}
		}

		public ErrorProperty(ErrorProvider ep, Control control)
		{
			ErrorProperty errorProperty = this;
			this.ep = ep;
			this.control = control;
			alignment = ErrorIconAlignment.MiddleRight;
			padding = 0;
			text = string.Empty;
			blink_count = 0;
			tick = window_Tick;
			window = new ErrorWindow();
			window.Visible = false;
			window.Width = ep.icon.Width;
			window.Height = ep.icon.Height;
			OnUIAErrorProviderHookUp(ep, new ControlEventArgs(control));
			window.VisibleChanged += delegate
			{
				if (errorProperty.window.Visible)
				{
					OnUIAControlHookUp(control, new ControlEventArgs(errorProperty.window));
				}
				else
				{
					OnUIAControlUnhookUp(control, new ControlEventArgs(errorProperty.window));
				}
			};
			if (control.Parent != null)
			{
				OnUIAControlHookUp(control, new ControlEventArgs(window));
				control.Parent.Controls.Add(window);
				control.Parent.Controls.SetChildIndex(window, control.Parent.Controls.IndexOf(control) + 1);
			}
			window.Paint += window_Paint;
			window.MouseEnter += window_MouseEnter;
			window.MouseLeave += window_MouseLeave;
			control.SizeChanged += control_SizeLocationChanged;
			control.LocationChanged += control_SizeLocationChanged;
			control.ParentChanged += control_ParentChanged;
			CalculateAlignment();
		}

		private void CalculateAlignment()
		{
			if (visible)
			{
				visible = false;
				ep.tooltip.Visible = false;
			}
			switch (alignment)
			{
			case ErrorIconAlignment.TopLeft:
				window.Left = control.Left - ep.icon.Width - padding;
				window.Top = control.Top;
				break;
			case ErrorIconAlignment.TopRight:
				window.Left = control.Left + control.Width + padding;
				window.Top = control.Top;
				break;
			case ErrorIconAlignment.MiddleLeft:
				window.Left = control.Left - ep.icon.Width - padding;
				window.Top = control.Top + (control.Height - ep.icon.Height) / 2;
				break;
			case ErrorIconAlignment.MiddleRight:
				window.Left = control.Left + control.Width + padding;
				window.Top = control.Top + (control.Height - ep.icon.Height) / 2;
				break;
			case ErrorIconAlignment.BottomLeft:
				window.Left = control.Left - ep.icon.Width - padding;
				window.Top = control.Top + control.Height - ep.icon.Height;
				break;
			case ErrorIconAlignment.BottomRight:
				window.Left = control.Left + control.Width + padding;
				window.Top = control.Top + control.Height - ep.icon.Height;
				break;
			}
		}

		private void window_Paint(object sender, PaintEventArgs e)
		{
			if (text != string.Empty)
			{
				e.Graphics.DrawIcon(ep.icon, 0, 0);
			}
		}

		private void window_MouseEnter(object sender, EventArgs e)
		{
			if (!visible)
			{
				visible = true;
				Point mousePosition = Control.MousePosition;
				Size size = ThemeEngine.Current.ToolTipSize(ep.tooltip, text);
				ep.tooltip.Width = size.Width;
				ep.tooltip.Height = size.Height;
				ep.tooltip.Text = text;
				if (mousePosition.X + size.Width < SystemInformation.WorkingArea.Width)
				{
					ep.tooltip.Left = mousePosition.X;
				}
				else
				{
					ep.tooltip.Left = mousePosition.X - size.Width;
				}
				if (mousePosition.Y + size.Height < SystemInformation.WorkingArea.Height - 16)
				{
					ep.tooltip.Top = mousePosition.Y + 16;
				}
				else
				{
					ep.tooltip.Top = mousePosition.Y - size.Height;
				}
				ep.UIAControl = control;
				ep.tooltip.Visible = true;
			}
		}

		private void window_MouseLeave(object sender, EventArgs e)
		{
			if (visible)
			{
				visible = false;
				ep.tooltip.Visible = false;
			}
		}

		private void control_SizeLocationChanged(object sender, EventArgs e)
		{
			if (visible)
			{
				visible = false;
				ep.tooltip.Visible = false;
			}
			CalculateAlignment();
		}

		private void control_ParentChanged(object sender, EventArgs e)
		{
			if (control.Parent != null)
			{
				OnUIAControlUnhookUp(control, new ControlEventArgs(window));
				control.Parent.Controls.Add(window);
				control.Parent.Controls.SetChildIndex(window, control.Parent.Controls.IndexOf(control) + 1);
				OnUIAControlHookUp(control, new ControlEventArgs(window));
			}
		}

		private void window_Tick(object sender, EventArgs e)
		{
			if (!timer.Enabled || !control.IsHandleCreated || !control.Visible)
			{
				return;
			}
			blink_count++;
			Graphics graphics = window.CreateGraphics();
			if (blink_count % 2 == 0)
			{
				graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(window.Parent.BackColor), window.ClientRectangle);
			}
			else
			{
				graphics.DrawIcon(ep.icon, 0, 0);
			}
			graphics.Dispose();
			switch (ep.blinkstyle)
			{
			case ErrorBlinkStyle.BlinkIfDifferentError:
				if (blink_count > 10)
				{
					timer.Stop();
				}
				break;
			case ErrorBlinkStyle.NeverBlink:
				timer.Stop();
				break;
			}
			if (blink_count == 11)
			{
				blink_count = 1;
			}
		}
	}

	private int blinkrate;

	private ErrorBlinkStyle blinkstyle;

	private string datamember;

	private object datasource;

	private ContainerControl container;

	private Icon icon;

	private Hashtable controls;

	private ToolTip.ToolTipWindow tooltip;

	private bool right_to_left;

	private object tag;

	private static object RightToLeftChangedEvent;

	private Control uia_control;

	[DefaultValue(250)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public int BlinkRate
	{
		get
		{
			return blinkrate;
		}
		set
		{
			blinkrate = value;
		}
	}

	[DefaultValue(ErrorBlinkStyle.BlinkIfDifferentError)]
	public ErrorBlinkStyle BlinkStyle
	{
		get
		{
			return blinkstyle;
		}
		set
		{
			blinkstyle = value;
		}
	}

	[DefaultValue(null)]
	public ContainerControl ContainerControl
	{
		get
		{
			return container;
		}
		set
		{
			container = value;
		}
	}

	[DefaultValue(null)]
	[System.MonoTODO("Stub, does nothing")]
	[Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public string DataMember
	{
		get
		{
			return datamember;
		}
		set
		{
			datamember = value;
		}
	}

	[DefaultValue(null)]
	[AttributeProvider(typeof(IListSource))]
	[System.MonoTODO("Stub, does nothing")]
	public object DataSource
	{
		get
		{
			return datasource;
		}
		set
		{
			datasource = value;
		}
	}

	[Localizable(true)]
	public Icon Icon
	{
		get
		{
			return icon;
		}
		set
		{
			if (value != null && (value.Height != 16 || value.Width != 16))
			{
				icon = new Icon(value, 16, 16);
			}
			else
			{
				icon = value;
			}
		}
	}

	public override ISite Site
	{
		set
		{
			base.Site = value;
		}
	}

	[System.MonoTODO("RTL not supported")]
	[Localizable(true)]
	[DefaultValue(false)]
	public virtual bool RightToLeft
	{
		get
		{
			return right_to_left;
		}
		set
		{
			right_to_left = value;
		}
	}

	[TypeConverter(typeof(StringConverter))]
	[Localizable(false)]
	[Bindable(true)]
	[MWFCategory("Data")]
	[DefaultValue(null)]
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

	internal Control UIAControl
	{
		get
		{
			return uia_control;
		}
		set
		{
			uia_control = value;
		}
	}

	internal Rectangle UIAToolTipRectangle => tooltip.Bounds;

	public event EventHandler RightToLeftChanged
	{
		add
		{
			base.Events.AddHandler(RightToLeftChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(RightToLeftChangedEvent, value);
		}
	}

	internal static event ControlEventHandler UIAControlHookUp;

	internal static event ControlEventHandler UIAControlUnhookUp;

	internal static event ControlEventHandler UIAErrorProviderHookUp;

	internal static event ControlEventHandler UIAErrorProviderUnhookUp;

	internal static event PopupEventHandler UIAPopup;

	internal static event PopupEventHandler UIAUnPopup;

	public ErrorProvider()
	{
		controls = new Hashtable();
		blinkrate = 250;
		blinkstyle = ErrorBlinkStyle.BlinkIfDifferentError;
		icon = ResourceImageLoader.GetIcon("errorProvider.ico");
		tooltip = new ToolTip.ToolTipWindow();
		tooltip.VisibleChanged += delegate
		{
			if (tooltip.Visible)
			{
				OnUIAPopup(this, new PopupEventArgs(UIAControl, UIAControl, isBalloon: false, Size.Empty));
			}
			else if (!tooltip.Visible)
			{
				OnUIAUnPopup(this, new PopupEventArgs(UIAControl, UIAControl, isBalloon: false, Size.Empty));
			}
		};
	}

	public ErrorProvider(ContainerControl parentControl)
		: this()
	{
		container = parentControl;
	}

	public ErrorProvider(IContainer container)
		: this()
	{
		container.Add(this);
	}

	static ErrorProvider()
	{
		RightToLeftChanged = new object();
	}

	void ISupportInitialize.BeginInit()
	{
	}

	void ISupportInitialize.EndInit()
	{
	}

	[System.MonoTODO("Stub, does nothing")]
	public void BindToDataAndErrors(object newDataSource, string newDataMember)
	{
		datasource = newDataSource;
		datamember = newDataMember;
	}

	public bool CanExtend(object extendee)
	{
		if (!(extendee is Control))
		{
			return false;
		}
		if (extendee is Form || extendee is ToolBar)
		{
			return false;
		}
		return true;
	}

	public void Clear()
	{
		foreach (ErrorProperty value in controls.Values)
		{
			value.Text = string.Empty;
		}
	}

	[DefaultValue("")]
	[Localizable(true)]
	public string GetError(Control control)
	{
		return GetErrorProperty(control).Text;
	}

	[DefaultValue(ErrorIconAlignment.MiddleRight)]
	[Localizable(true)]
	public ErrorIconAlignment GetIconAlignment(Control control)
	{
		return GetErrorProperty(control).Alignment;
	}

	[DefaultValue(0)]
	[Localizable(true)]
	public int GetIconPadding(Control control)
	{
		return GetErrorProperty(control).padding;
	}

	public void SetError(Control control, string value)
	{
		GetErrorProperty(control).Text = value;
	}

	public void SetIconAlignment(Control control, ErrorIconAlignment value)
	{
		GetErrorProperty(control).Alignment = value;
	}

	public void SetIconPadding(Control control, int padding)
	{
		GetErrorProperty(control).Padding = padding;
	}

	[System.MonoTODO("Stub, does nothing")]
	public void UpdateBinding()
	{
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnRightToLeftChanged(EventArgs e)
	{
		((EventHandler)base.Events[RightToLeftChanged])?.Invoke(this, e);
	}

	private ErrorProperty GetErrorProperty(Control control)
	{
		ErrorProperty errorProperty = (ErrorProperty)controls[control];
		if (errorProperty == null)
		{
			errorProperty = new ErrorProperty(this, control);
			controls[control] = errorProperty;
		}
		return errorProperty;
	}

	internal static void OnUIAPopup(ErrorProvider sender, PopupEventArgs args)
	{
		if (ErrorProvider.UIAPopup != null)
		{
			ErrorProvider.UIAPopup(sender, args);
		}
	}

	internal static void OnUIAUnPopup(ErrorProvider sender, PopupEventArgs args)
	{
		if (ErrorProvider.UIAUnPopup != null)
		{
			ErrorProvider.UIAUnPopup(sender, args);
		}
	}

	internal static void OnUIAControlHookUp(object sender, ControlEventArgs args)
	{
		if (ErrorProvider.UIAControlHookUp != null)
		{
			ErrorProvider.UIAControlHookUp(sender, args);
		}
	}

	internal static void OnUIAControlUnhookUp(object sender, ControlEventArgs args)
	{
		if (ErrorProvider.UIAControlUnhookUp != null)
		{
			ErrorProvider.UIAControlUnhookUp(sender, args);
		}
	}

	internal static void OnUIAErrorProviderHookUp(object sender, ControlEventArgs args)
	{
		if (ErrorProvider.UIAErrorProviderHookUp != null)
		{
			ErrorProvider.UIAErrorProviderHookUp(sender, args);
		}
	}

	internal static void OnUIAErrorProviderUnhookUp(object sender, ControlEventArgs args)
	{
		if (ErrorProvider.UIAErrorProviderUnhookUp != null)
		{
			ErrorProvider.UIAErrorProviderUnhookUp(sender, args);
		}
	}
}
