namespace System.Windows.Forms;

internal class LabelEditTextBox : FixedSizeTextBox
{
	private static object EditingCancelledEvent;

	private static object EditingFinishedEvent;

	public event EventHandler EditingCancelled
	{
		add
		{
			base.Events.AddHandler(EditingCancelledEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(EditingCancelledEvent, value);
		}
	}

	public event EventHandler EditingFinished
	{
		add
		{
			base.Events.AddHandler(EditingFinishedEvent, value);
		}
		remove
		{
			base.Events.AddHandler(EditingFinishedEvent, value);
		}
	}

	public LabelEditTextBox()
		: base(fixed_horz: true, fixed_vert: true)
	{
	}

	static LabelEditTextBox()
	{
		EditingCancelled = new object();
		EditingFinished = new object();
	}

	protected override bool IsInputKey(Keys key_data)
	{
		if ((key_data & Keys.Alt) == 0)
		{
			switch (key_data & Keys.KeyCode)
			{
			case Keys.Return:
				return true;
			case Keys.Escape:
				return true;
			}
		}
		return base.IsInputKey(key_data);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (base.Visible)
		{
			switch (e.KeyCode)
			{
			case Keys.Return:
				base.Visible = false;
				base.Parent.Focus();
				e.Handled = true;
				OnEditingFinished(e);
				break;
			case Keys.Escape:
				base.Visible = false;
				base.Parent.Focus();
				e.Handled = true;
				OnEditingCancelled(e);
				break;
			}
		}
	}

	protected override void OnLostFocus(EventArgs e)
	{
		if (base.Visible)
		{
			OnEditingFinished(e);
		}
	}

	protected void OnEditingCancelled(EventArgs e)
	{
		((EventHandler)base.Events[EditingCancelled])?.Invoke(this, e);
	}

	protected void OnEditingFinished(EventArgs e)
	{
		((EventHandler)base.Events[EditingFinished])?.Invoke(this, e);
	}
}
