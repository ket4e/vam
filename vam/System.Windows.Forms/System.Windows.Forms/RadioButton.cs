using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DefaultProperty("Checked")]
[DefaultBindingProperty("Checked")]
[ToolboxItem("System.Windows.Forms.Design.AutoSizeToolboxItem,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[Designer("System.Windows.Forms.Design.RadioButtonDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent("CheckedChanged")]
[ComVisible(true)]
public class RadioButton : ButtonBase
{
	[ComVisible(true)]
	public class RadioButtonAccessibleObject : ButtonBaseAccessibleObject
	{
		private new RadioButton owner;

		public override string DefaultAction => "Select";

		public override AccessibleRole Role => AccessibleRole.RadioButton;

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

		public RadioButtonAccessibleObject(RadioButton owner)
			: base(owner)
		{
			this.owner = owner;
		}

		public override void DoDefaultAction()
		{
			owner.PerformClick();
		}
	}

	internal Appearance appearance;

	internal bool auto_check;

	internal ContentAlignment radiobutton_alignment;

	internal CheckState check_state;

	private static object AppearanceChangedEvent;

	private static object CheckedChangedEvent;

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
				((EventHandler)base.Events[AppearanceChanged])?.Invoke(this, EventArgs.Empty);
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

	[DefaultValue(ContentAlignment.MiddleLeft)]
	[Localizable(true)]
	public ContentAlignment CheckAlign
	{
		get
		{
			return radiobutton_alignment;
		}
		set
		{
			if (value != radiobutton_alignment)
			{
				radiobutton_alignment = value;
				Invalidate();
			}
		}
	}

	[Bindable(true, BindingDirection.OneWay)]
	[DefaultValue(false)]
	[SettingsBindable(true)]
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
				UpdateSiblings();
				OnCheckedChanged(EventArgs.Empty);
			}
			else if (!value && check_state != 0)
			{
				TabStop = false;
				check_state = CheckState.Unchecked;
				Invalidate();
				OnCheckedChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(false)]
	public new bool TabStop
	{
		get
		{
			return base.TabStop;
		}
		set
		{
			base.TabStop = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(ContentAlignment.MiddleLeft)]
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

	protected override CreateParams CreateParams
	{
		get
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			return base.CreateParams;
		}
	}

	protected override Size DefaultSize => ThemeEngine.Current.RadioButtonDefaultSize;

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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler DoubleClick
	{
		add
		{
			base.DoubleClick += value;
		}
		remove
		{
			base.DoubleClick -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	public RadioButton()
	{
		appearance = Appearance.Normal;
		auto_check = true;
		radiobutton_alignment = ContentAlignment.MiddleLeft;
		TextAlign = ContentAlignment.MiddleLeft;
		TabStop = false;
	}

	static RadioButton()
	{
		AppearanceChanged = new object();
		CheckedChanged = new object();
	}

	private void PerformDefaultCheck()
	{
		if (!auto_check || Checked)
		{
			return;
		}
		bool flag = false;
		Control control = base.Parent;
		if (control != null)
		{
			for (int i = 0; i < control.Controls.Count; i++)
			{
				if (control.Controls[i] is RadioButton radioButton && radioButton.auto_check && radioButton.check_state == CheckState.Checked)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			Checked = true;
		}
	}

	private void UpdateSiblings()
	{
		if (!auto_check)
		{
			return;
		}
		Control control = base.Parent;
		if (control != null)
		{
			for (int i = 0; i < control.Controls.Count; i++)
			{
				if (this != control.Controls[i] && control.Controls[i] is RadioButton && ((RadioButton)control.Controls[i]).auto_check)
				{
					control.Controls[i].TabStop = false;
					((RadioButton)control.Controls[i]).Checked = false;
				}
			}
		}
		TabStop = true;
	}

	internal override void Draw(PaintEventArgs pe)
	{
		ThemeEngine.Current.CalculateRadioButtonTextAndImageLayout(this, Point.Empty, out var glyphArea, out var textRectangle, out var imageRectangle);
		if (base.FlatStyle != FlatStyle.System && Appearance != Appearance.Button)
		{
			ThemeEngine.Current.DrawRadioButton(pe.Graphics, this, glyphArea, textRectangle, imageRectangle, pe.ClipRectangle);
		}
		else
		{
			ThemeEngine.Current.DrawRadioButton(pe.Graphics, base.ClientRectangle, this);
		}
	}

	internal override Size GetPreferredSizeCore(Size proposedSize)
	{
		if (AutoSize)
		{
			return ThemeEngine.Current.CalculateRadioButtonAutoSize(this);
		}
		return base.GetPreferredSizeCore(proposedSize);
	}

	public void PerformClick()
	{
		OnClick(EventArgs.Empty);
	}

	public override string ToString()
	{
		return base.ToString() + ", Checked: " + Checked;
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		AccessibleObject accessibleObject = base.CreateAccessibilityInstance();
		accessibleObject.role = AccessibleRole.RadioButton;
		return accessibleObject;
	}

	protected virtual void OnCheckedChanged(EventArgs e)
	{
		((EventHandler)base.Events[CheckedChanged])?.Invoke(this, e);
	}

	protected override void OnClick(EventArgs e)
	{
		if (auto_check)
		{
			if (!Checked)
			{
				Checked = true;
			}
		}
		else
		{
			Checked = !Checked;
		}
		base.OnClick(e);
	}

	protected override void OnEnter(EventArgs e)
	{
		PerformDefaultCheck();
		base.OnEnter(e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
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
			PerformClick();
			return true;
		}
		return base.ProcessMnemonic(charCode);
	}
}
