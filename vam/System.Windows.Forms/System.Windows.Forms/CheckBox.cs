using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DefaultBindingProperty("CheckState")]
[ToolboxItem("System.Windows.Forms.Design.AutoSizeToolboxItem,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
[DefaultEvent("CheckedChanged")]
[DefaultProperty("Checked")]
public class CheckBox : ButtonBase
{
	[ComVisible(true)]
	public class CheckBoxAccessibleObject : ButtonBaseAccessibleObject
	{
		private new CheckBox owner;

		public override string DefaultAction => "Select";

		public override AccessibleRole Role => AccessibleRole.CheckButton;

		public override AccessibleStates State
		{
			get
			{
				AccessibleStates accessibleStates = AccessibleStates.Default;
				if (owner.check_state == CheckState.Checked)
				{
					accessibleStates |= AccessibleStates.Checked;
				}
				if (owner.Focused)
				{
					accessibleStates |= AccessibleStates.Focused;
				}
				if (owner.CanFocus)
				{
					accessibleStates |= AccessibleStates.Focusable;
				}
				return accessibleStates;
			}
		}

		public CheckBoxAccessibleObject(Control owner)
			: base(owner)
		{
			this.owner = (CheckBox)owner;
		}

		public override void DoDefaultAction()
		{
			owner.Checked = !owner.Checked;
		}
	}

	internal Appearance appearance;

	internal bool auto_check;

	internal ContentAlignment check_alignment;

	internal CheckState check_state;

	internal bool three_state;

	private static object AppearanceChangedEvent;

	private static object CheckedChangedEvent;

	private static object CheckStateChangedEvent;

	[DefaultValue(Appearance.Normal)]
	[Localizable(true)]
	public Appearance Appearance
	{
		get
		{
			return appearance;
		}
		set
		{
			if (value != appearance)
			{
				appearance = value;
				OnAppearanceChanged(EventArgs.Empty);
				if (base.Parent != null)
				{
					base.Parent.PerformLayout(this, "Appearance");
				}
				Invalidate();
			}
		}
	}

	[DefaultValue(true)]
	public bool AutoCheck
	{
		get
		{
			return auto_check;
		}
		set
		{
			auto_check = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(ContentAlignment.MiddleLeft)]
	[Bindable(true)]
	public ContentAlignment CheckAlign
	{
		get
		{
			return check_alignment;
		}
		set
		{
			if (value != check_alignment)
			{
				check_alignment = value;
				if (base.Parent != null)
				{
					base.Parent.PerformLayout(this, "CheckAlign");
				}
				Invalidate();
			}
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	[DefaultValue(false)]
	[SettingsBindable(true)]
	[Bindable(true)]
	public bool Checked
	{
		get
		{
			if (check_state != 0)
			{
				return true;
			}
			return false;
		}
		set
		{
			if (value && check_state != CheckState.Checked)
			{
				check_state = CheckState.Checked;
				Invalidate();
				OnCheckedChanged(EventArgs.Empty);
			}
			else if (!value && check_state != 0)
			{
				check_state = CheckState.Unchecked;
				Invalidate();
				OnCheckedChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(CheckState.Unchecked)]
	[Bindable(true)]
	[RefreshProperties(RefreshProperties.All)]
	public CheckState CheckState
	{
		get
		{
			return check_state;
		}
		set
		{
			if (value != check_state)
			{
				bool flag = check_state != CheckState.Unchecked;
				check_state = value;
				if (flag != (check_state != CheckState.Unchecked))
				{
					OnCheckedChanged(EventArgs.Empty);
				}
				OnCheckStateChanged(EventArgs.Empty);
				Invalidate();
			}
		}
	}

	[DefaultValue(ContentAlignment.MiddleLeft)]
	[Localizable(true)]
	public override ContentAlignment TextAlign
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

	[DefaultValue(false)]
	public bool ThreeState
	{
		get
		{
			return three_state;
		}
		set
		{
			three_state = value;
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Size DefaultSize => new Size(104, 24);

	public event EventHandler AppearanceChanged
	{
		add
		{
			base.Events.AddHandler(AppearanceChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AppearanceChangedEvent, value);
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event MouseEventHandler MouseDoubleClick
	{
		add
		{
			base.MouseDoubleClick += value;
		}
		remove
		{
			base.MouseDoubleClick -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler DoubleClick;

	public CheckBox()
	{
		appearance = Appearance.Normal;
		auto_check = true;
		check_alignment = ContentAlignment.MiddleLeft;
		TextAlign = ContentAlignment.MiddleLeft;
		SetStyle(ControlStyles.StandardDoubleClick, value: false);
		SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
	}

	static CheckBox()
	{
		AppearanceChanged = new object();
		CheckedChanged = new object();
		CheckStateChanged = new object();
	}

	internal override void Draw(PaintEventArgs pe)
	{
		ThemeEngine.Current.CalculateCheckBoxTextAndImageLayout(this, Point.Empty, out var glyphArea, out var textRectangle, out var imageRectangle);
		if (base.FlatStyle != FlatStyle.System)
		{
			ThemeEngine.Current.DrawCheckBox(pe.Graphics, this, glyphArea, textRectangle, imageRectangle, pe.ClipRectangle);
		}
		else
		{
			ThemeEngine.Current.DrawCheckBox(pe.Graphics, base.ClientRectangle, this);
		}
	}

	internal override Size GetPreferredSizeCore(Size proposedSize)
	{
		if (AutoSize)
		{
			return ThemeEngine.Current.CalculateCheckBoxAutoSize(this);
		}
		return base.GetPreferredSizeCore(proposedSize);
	}

	internal override void HaveDoubleClick()
	{
		if (this.DoubleClick != null)
		{
			this.DoubleClick(this, EventArgs.Empty);
		}
	}

	public override string ToString()
	{
		return base.ToString() + ", CheckState: " + (int)check_state;
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		AccessibleObject accessibleObject = base.CreateAccessibilityInstance();
		accessibleObject.role = AccessibleRole.CheckButton;
		return accessibleObject;
	}

	protected virtual void OnAppearanceChanged(EventArgs e)
	{
		((EventHandler)base.Events[AppearanceChanged])?.Invoke(this, e);
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
		if (auto_check)
		{
			switch (check_state)
			{
			case CheckState.Unchecked:
				if (three_state)
				{
					CheckState = CheckState.Indeterminate;
				}
				else
				{
					CheckState = CheckState.Checked;
				}
				break;
			case CheckState.Indeterminate:
				CheckState = CheckState.Checked;
				break;
			case CheckState.Checked:
				CheckState = CheckState.Unchecked;
				break;
			}
		}
		base.OnClick(e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
	}

	protected override void OnMouseUp(MouseEventArgs mevent)
	{
		base.OnMouseUp(mevent);
	}

	protected override bool ProcessMnemonic(char charCode)
	{
		if (Control.IsMnemonic(charCode, Text))
		{
			Select();
			OnClick(EventArgs.Empty);
			return true;
		}
		return base.ProcessMnemonic(charCode);
	}
}
