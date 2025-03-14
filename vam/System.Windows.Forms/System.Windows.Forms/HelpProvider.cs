using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ProvideProperty("ShowHelp", "System.Windows.Forms.Control, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[ToolboxItemFilter("System.Windows.Forms")]
[ProvideProperty("HelpString", "System.Windows.Forms.Control, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[ProvideProperty("HelpKeyword", "System.Windows.Forms.Control, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[ProvideProperty("HelpNavigator", "System.Windows.Forms.Control, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public class HelpProvider : Component, IExtenderProvider
{
	private class HelpProperty
	{
		internal string keyword;

		internal HelpNavigator navigator;

		internal string text;

		internal bool show;

		internal Control control;

		internal HelpProvider hp;

		public string Keyword
		{
			get
			{
				return keyword;
			}
			set
			{
				keyword = value;
			}
		}

		public HelpNavigator Navigator
		{
			get
			{
				return navigator;
			}
			set
			{
				navigator = value;
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public bool Show
		{
			get
			{
				return show;
			}
			set
			{
				show = value;
			}
		}

		public HelpProperty(HelpProvider hp, Control control)
		{
			this.control = control;
			this.hp = hp;
			keyword = null;
			navigator = HelpNavigator.AssociateIndex;
			text = null;
			show = false;
			control.HelpRequested += hp.HelpRequestHandler;
		}
	}

	private string helpnamespace;

	private Hashtable controls;

	private ToolTip.ToolTipWindow tooltip;

	private EventHandler HideToolTipHandler;

	private KeyPressEventHandler HideToolTipKeyHandler;

	private MouseEventHandler HideToolTipMouseHandler;

	private HelpEventHandler HelpRequestHandler;

	private object tag;

	private Control uia_control;

	[Localizable(true)]
	[Editor("System.Windows.Forms.Design.HelpNamespaceEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultValue(null)]
	public virtual string HelpNamespace
	{
		get
		{
			return helpnamespace;
		}
		set
		{
			helpnamespace = value;
		}
	}

	[Bindable(true)]
	[MWFCategory("Data")]
	[DefaultValue(null)]
	[Localizable(false)]
	[TypeConverter(typeof(StringConverter))]
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

	private Control UIAControl
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

	internal static event ControlEventHandler UIAHelpRequested;

	internal static event ControlEventHandler UIAHelpUnRequested;

	public HelpProvider()
	{
		controls = new Hashtable();
		tooltip = new ToolTip.ToolTipWindow();
		tooltip.VisibleChanged += delegate
		{
			if (tooltip.Visible)
			{
				OnUIAHelpRequested(this, new ControlEventArgs(UIAControl));
			}
			else
			{
				OnUIAHelpUnRequested(this, new ControlEventArgs(UIAControl));
			}
		};
		HideToolTipHandler = HideToolTip;
		HideToolTipKeyHandler = HideToolTipKey;
		HideToolTipMouseHandler = HideToolTipMouse;
		HelpRequestHandler = HelpRequested;
	}

	public virtual bool CanExtend(object target)
	{
		if (!(target is Control))
		{
			return false;
		}
		if (target is Form || target is ToolBar)
		{
			return false;
		}
		return true;
	}

	[Localizable(true)]
	[DefaultValue(null)]
	public virtual string GetHelpKeyword(Control ctl)
	{
		return GetHelpProperty(ctl).Keyword;
	}

	[DefaultValue(HelpNavigator.AssociateIndex)]
	[Localizable(true)]
	public virtual HelpNavigator GetHelpNavigator(Control ctl)
	{
		return GetHelpProperty(ctl).Navigator;
	}

	[DefaultValue(null)]
	[Localizable(true)]
	public virtual string GetHelpString(Control ctl)
	{
		return GetHelpProperty(ctl).Text;
	}

	[Localizable(true)]
	public virtual bool GetShowHelp(Control ctl)
	{
		return GetHelpProperty(ctl).Show;
	}

	public virtual void ResetShowHelp(Control ctl)
	{
		HelpProperty helpProperty = GetHelpProperty(ctl);
		if (helpProperty.Keyword != null || helpProperty.Text != null)
		{
			helpProperty.Show = true;
		}
		else
		{
			helpProperty.Show = false;
		}
	}

	public virtual void SetHelpKeyword(Control ctl, string keyword)
	{
		GetHelpProperty(ctl).Keyword = keyword;
	}

	public virtual void SetHelpNavigator(Control ctl, HelpNavigator navigator)
	{
		GetHelpProperty(ctl).Navigator = navigator;
	}

	public virtual void SetHelpString(Control ctl, string helpString)
	{
		GetHelpProperty(ctl).Text = helpString;
	}

	public virtual void SetShowHelp(Control ctl, bool value)
	{
		GetHelpProperty(ctl).Show = value;
	}

	public override string ToString()
	{
		return base.ToString() + ", HelpNameSpace: " + helpnamespace;
	}

	private HelpProperty GetHelpProperty(Control control)
	{
		HelpProperty helpProperty = (HelpProperty)controls[control];
		if (helpProperty == null)
		{
			helpProperty = new HelpProperty(this, control);
			controls[control] = helpProperty;
		}
		return helpProperty;
	}

	private void HideToolTip(object Sender, EventArgs e)
	{
		Control control = (Control)Sender;
		control.LostFocus -= HideToolTipHandler;
		tooltip.Visible = false;
	}

	private void HideToolTipKey(object Sender, KeyPressEventArgs e)
	{
		Control control = (Control)Sender;
		control.KeyPress -= HideToolTipKeyHandler;
		tooltip.Visible = false;
	}

	private void HideToolTipMouse(object Sender, MouseEventArgs e)
	{
		Control control = (Control)Sender;
		control.MouseDown -= HideToolTipMouseHandler;
		tooltip.Visible = false;
	}

	private void HelpRequested(object sender, HelpEventArgs e)
	{
		Control control2 = (UIAControl = (Control)sender);
		if (GetHelpProperty(control2).Text != null)
		{
			Point mousePos = e.MousePos;
			tooltip.Text = GetHelpProperty(control2).Text;
			Size size = ThemeEngine.Current.ToolTipSize(tooltip, tooltip.Text);
			tooltip.Width = size.Width;
			tooltip.Height = size.Height;
			mousePos.X -= size.Width / 2;
			if (mousePos.X < 0)
			{
				mousePos.X += size.Width / 2;
			}
			if (mousePos.X + size.Width < SystemInformation.WorkingArea.Width)
			{
				tooltip.Left = mousePos.X;
			}
			else
			{
				tooltip.Left = mousePos.X - size.Width;
			}
			if (mousePos.Y + size.Height < SystemInformation.WorkingArea.Height - 16)
			{
				tooltip.Top = mousePos.Y;
			}
			else
			{
				tooltip.Top = mousePos.Y - size.Height;
			}
			tooltip.Visible = true;
			control2.KeyPress += HideToolTipKeyHandler;
			control2.MouseDown += HideToolTipMouseHandler;
			control2.LostFocus += HideToolTipHandler;
			e.Handled = true;
		}
	}

	internal static void OnUIAHelpRequested(HelpProvider provider, ControlEventArgs args)
	{
		if (HelpProvider.UIAHelpRequested != null)
		{
			HelpProvider.UIAHelpRequested(provider, args);
		}
	}

	internal static void OnUIAHelpUnRequested(HelpProvider provider, ControlEventArgs args)
	{
		if (HelpProvider.UIAHelpUnRequested != null)
		{
			HelpProvider.UIAHelpUnRequested(provider, args);
		}
	}
}
