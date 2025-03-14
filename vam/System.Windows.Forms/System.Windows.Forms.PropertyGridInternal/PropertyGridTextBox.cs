using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal;

internal class PropertyGridTextBox : UserControl, IMessageFilter
{
	private PGTextBox textbox;

	private Button dialog_button;

	private Button dropdown_button;

	private bool validating;

	private bool filtering;

	private static object DropDownButtonClickedEvent;

	private static object DialogButtonClickedEvent;

	private static object ToggleValueEvent;

	private static object KeyDownEvent;

	private static object ValidateEvent;

	public bool DialogButtonVisible
	{
		get
		{
			return dialog_button.Visible;
		}
		set
		{
			dialog_button.Visible = value;
		}
	}

	public bool DropDownButtonVisible
	{
		get
		{
			return dropdown_button.Visible;
		}
		set
		{
			dropdown_button.Visible = value;
		}
	}

	public new Color ForeColor
	{
		get
		{
			return base.ForeColor;
		}
		set
		{
			textbox.ForeColor = value;
			dropdown_button.ForeColor = value;
			dialog_button.ForeColor = value;
			base.ForeColor = value;
		}
	}

	public new Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			textbox.BackColor = value;
			base.BackColor = value;
		}
	}

	public bool ReadOnly
	{
		get
		{
			return textbox.ReadOnly;
		}
		set
		{
			textbox.ReadOnly = value;
		}
	}

	public new string Text
	{
		get
		{
			return textbox.Text;
		}
		set
		{
			textbox.Text = value;
		}
	}

	public char PasswordChar
	{
		set
		{
			textbox.PasswordChar = value;
		}
	}

	public event EventHandler DropDownButtonClicked
	{
		add
		{
			base.Events.AddHandler(DropDownButtonClickedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DropDownButtonClickedEvent, value);
		}
	}

	public event EventHandler DialogButtonClicked
	{
		add
		{
			base.Events.AddHandler(DialogButtonClickedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DialogButtonClickedEvent, value);
		}
	}

	public event EventHandler ToggleValue
	{
		add
		{
			base.Events.AddHandler(ToggleValueEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ToggleValueEvent, value);
		}
	}

	public new event KeyEventHandler KeyDown
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

	public new event CancelEventHandler Validate
	{
		add
		{
			base.Events.AddHandler(ValidateEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ValidateEvent, value);
		}
	}

	public PropertyGridTextBox()
	{
		dialog_button = new Button();
		dropdown_button = new Button();
		textbox = new PGTextBox();
		SuspendLayout();
		dialog_button.Dock = DockStyle.Right;
		dialog_button.BackColor = SystemColors.Control;
		dialog_button.Size = new Size(16, 16);
		dialog_button.TabIndex = 1;
		dialog_button.Visible = false;
		dialog_button.Click += dialog_button_Click;
		dropdown_button.Dock = DockStyle.Right;
		dropdown_button.BackColor = SystemColors.Control;
		dropdown_button.Size = new Size(16, 16);
		dropdown_button.TabIndex = 2;
		dropdown_button.Visible = false;
		dropdown_button.Click += dropdown_button_Click;
		textbox.AutoSize = false;
		textbox.BorderStyle = BorderStyle.None;
		textbox.Dock = DockStyle.Fill;
		textbox.TabIndex = 3;
		base.Controls.Add(textbox);
		base.Controls.Add(dropdown_button);
		base.Controls.Add(dialog_button);
		SetStyle(ControlStyles.Selectable, value: true);
		ResumeLayout(performLayout: false);
		dropdown_button.Paint += dropdown_button_Paint;
		dialog_button.Paint += dialog_button_Paint;
		textbox.DoubleClick += textbox_DoubleClick;
		textbox.KeyDown += textbox_KeyDown;
		textbox.GotFocus += textbox_GotFocus;
	}

	static PropertyGridTextBox()
	{
		DropDownButtonClicked = new object();
		DialogButtonClicked = new object();
		ToggleValue = new object();
		KeyDown = new object();
		Validate = new object();
	}

	bool IMessageFilter.PreFilterMessage(ref Message m)
	{
		if (!validating && m.HWnd != textbox.Handle && textbox.Focused && (m.Msg == 513 || m.Msg == 519 || m.Msg == 516 || m.Msg == 161 || m.Msg == 167 || m.Msg == 164))
		{
			CancelEventHandler cancelEventHandler = (CancelEventHandler)base.Events[Validate];
			if (cancelEventHandler != null)
			{
				CancelEventArgs cancelEventArgs = new CancelEventArgs();
				validating = true;
				cancelEventHandler(this, cancelEventArgs);
				validating = false;
				if (!cancelEventArgs.Cancel)
				{
					Application.RemoveMessageFilter(this);
					filtering = false;
				}
				return cancelEventArgs.Cancel;
			}
		}
		return false;
	}

	protected override void OnGotFocus(EventArgs args)
	{
		base.OnGotFocus(args);
		textbox.has_been_focused = true;
		textbox.Focus();
		textbox.SelectionLength = 0;
	}

	private void dropdown_button_Paint(object sender, PaintEventArgs e)
	{
		ThemeEngine.Current.CPDrawComboButton(e.Graphics, dropdown_button.ClientRectangle, dropdown_button.ButtonState);
	}

	private void dialog_button_Paint(object sender, PaintEventArgs e)
	{
		e.Graphics.DrawString("...", new Font(Font, FontStyle.Bold), Brushes.Black, 0f, 0f);
	}

	private void dropdown_button_Click(object sender, EventArgs e)
	{
		((EventHandler)base.Events[DropDownButtonClicked])?.Invoke(this, e);
	}

	private void dialog_button_Click(object sender, EventArgs e)
	{
		((EventHandler)base.Events[DialogButtonClicked])?.Invoke(this, e);
	}

	internal void SendMouseDown(Point screenLocation)
	{
		Point point = PointToClient(screenLocation);
		XplatUI.SendMessage(Handle, Msg.WM_LBUTTONDOWN, new IntPtr(1), Control.MakeParam(point.X, point.Y));
		textbox.FocusAt(screenLocation);
	}

	private void textbox_DoubleClick(object sender, EventArgs e)
	{
		((EventHandler)base.Events[ToggleValue])?.Invoke(this, e);
	}

	private void textbox_KeyDown(object sender, KeyEventArgs e)
	{
		((KeyEventHandler)base.Events[KeyDown])?.Invoke(this, e);
	}

	private void textbox_GotFocus(object sender, EventArgs e)
	{
		if (!filtering)
		{
			filtering = true;
			Application.AddMessageFilter(this);
		}
	}

	protected override void DestroyHandle()
	{
		Application.RemoveMessageFilter(this);
		filtering = false;
		base.DestroyHandle();
	}
}
