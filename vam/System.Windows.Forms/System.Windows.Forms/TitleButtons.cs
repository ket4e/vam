using System.Collections;

namespace System.Windows.Forms;

internal class TitleButtons : IEnumerable
{
	private const int tooltip_hide_interval = 3000;

	private const int tooltip_show_interval = 1000;

	public TitleButton MinimizeButton;

	public TitleButton MaximizeButton;

	public TitleButton RestoreButton;

	public TitleButton CloseButton;

	public TitleButton HelpButton;

	public TitleButton[] AllButtons;

	public bool Visible;

	private ToolTip.ToolTipWindow tooltip;

	private Timer tooltip_timer;

	private TitleButton tooltip_hovered_button;

	private TitleButton tooltip_hidden_button;

	private Form form;

	public bool AnyPushedTitleButtons
	{
		get
		{
			if (!Visible)
			{
				return false;
			}
			TitleButton[] allButtons = AllButtons;
			foreach (TitleButton titleButton in allButtons)
			{
				if (titleButton.Visible && titleButton.State == ButtonState.Pushed)
				{
					return true;
				}
			}
			return false;
		}
	}

	public TitleButtons(Form frm)
	{
		form = frm;
		Visible = true;
		MinimizeButton = new TitleButton(CaptionButton.Minimize, ClickHandler);
		MaximizeButton = new TitleButton(CaptionButton.Maximize, ClickHandler);
		RestoreButton = new TitleButton(CaptionButton.Restore, ClickHandler);
		CloseButton = new TitleButton(CaptionButton.Close, ClickHandler);
		HelpButton = new TitleButton(CaptionButton.Help, ClickHandler);
		AllButtons = new TitleButton[5] { MinimizeButton, MaximizeButton, RestoreButton, CloseButton, HelpButton };
	}

	private void ClickHandler(object sender, EventArgs e)
	{
		if (Visible)
		{
			TitleButton titleButton = (TitleButton)sender;
			switch (titleButton.Caption)
			{
			case CaptionButton.Close:
				form.Close();
				break;
			case CaptionButton.Help:
				Console.WriteLine("Help not implemented.");
				break;
			case CaptionButton.Maximize:
				form.WindowState = FormWindowState.Maximized;
				break;
			case CaptionButton.Minimize:
				form.WindowState = FormWindowState.Minimized;
				break;
			case CaptionButton.Restore:
				form.WindowState = FormWindowState.Normal;
				break;
			}
		}
	}

	public TitleButton FindButton(int x, int y)
	{
		if (!Visible)
		{
			return null;
		}
		TitleButton[] allButtons = AllButtons;
		foreach (TitleButton titleButton in allButtons)
		{
			if (titleButton.Visible && titleButton.Rectangle.Contains(x, y))
			{
				return titleButton;
			}
		}
		return null;
	}

	public IEnumerator GetEnumerator()
	{
		return AllButtons.GetEnumerator();
	}

	public void ToolTipStart(TitleButton button)
	{
		tooltip_hovered_button = button;
		if (tooltip_hovered_button != tooltip_hidden_button)
		{
			tooltip_hidden_button = null;
			if (tooltip != null && tooltip.Visible)
			{
				ToolTipShow(only_refresh: true);
			}
			if (tooltip_timer == null)
			{
				tooltip_timer = new Timer();
				tooltip_timer.Tick += ToolTipTimerTick;
			}
			tooltip_timer.Interval = 1000;
			tooltip_timer.Start();
			tooltip_hovered_button = button;
		}
	}

	public void ToolTipTimerTick(object sender, EventArgs e)
	{
		if (tooltip_timer.Interval == 3000)
		{
			tooltip_hidden_button = tooltip_hovered_button;
			ToolTipHide(reset_hidden_button: false);
		}
		else
		{
			ToolTipShow(only_refresh: false);
		}
	}

	public void ToolTipShow(bool only_refresh)
	{
		if (!form.Visible)
		{
			return;
		}
		string text = Locale.GetText(tooltip_hovered_button.Caption.ToString());
		tooltip_timer.Interval = 3000;
		tooltip_timer.Enabled = true;
		if (only_refresh && (tooltip == null || !tooltip.Visible))
		{
			return;
		}
		if (tooltip == null)
		{
			tooltip = new ToolTip.ToolTipWindow();
		}
		else
		{
			if (tooltip.Text == text && tooltip.Visible)
			{
				return;
			}
			if (tooltip.Visible)
			{
				tooltip.Visible = false;
			}
		}
		if (form.WindowState == FormWindowState.Maximized && form.MdiParent != null)
		{
			tooltip.Present(form.MdiParent, text);
		}
		else
		{
			tooltip.Present(form, text);
		}
	}

	public void ToolTipHide(bool reset_hidden_button)
	{
		if (tooltip_timer != null)
		{
			tooltip_timer.Enabled = false;
		}
		if (tooltip != null && tooltip.Visible)
		{
			tooltip.Visible = false;
		}
		if (reset_hidden_button)
		{
			tooltip_hidden_button = null;
		}
	}

	public bool MouseMove(int x, int y)
	{
		if (!Visible)
		{
			return false;
		}
		bool flag = false;
		bool anyPushedTitleButtons = AnyPushedTitleButtons;
		bool flag2 = false;
		TitleButton titleButton = FindButton(x, y);
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TitleButton titleButton2 = (TitleButton)enumerator.Current;
				if (titleButton2 == null || titleButton2.State == ButtonState.Inactive)
				{
					continue;
				}
				if (titleButton2 == titleButton)
				{
					if (anyPushedTitleButtons)
					{
						flag |= titleButton2.State != ButtonState.Pushed;
						titleButton2.State = ButtonState.Pushed;
					}
					ToolTipStart(titleButton2);
					flag2 = true;
					if (!titleButton2.Entered)
					{
						titleButton2.Entered = true;
						if (ThemeEngine.Current.ManagedWindowTitleButtonHasHotElementStyle(titleButton2, form))
						{
							flag = true;
						}
					}
					continue;
				}
				if (anyPushedTitleButtons)
				{
					flag |= titleButton2.State != ButtonState.Normal;
					titleButton2.State = ButtonState.Normal;
				}
				if (titleButton2.Entered)
				{
					titleButton2.Entered = false;
					if (ThemeEngine.Current.ManagedWindowTitleButtonHasHotElementStyle(titleButton2, form))
					{
						flag = true;
					}
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		if (!flag2)
		{
			ToolTipHide(reset_hidden_button: false);
		}
		return flag;
	}

	public void MouseDown(int x, int y)
	{
		if (!Visible)
		{
			return;
		}
		ToolTipHide(reset_hidden_button: false);
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TitleButton titleButton = (TitleButton)enumerator.Current;
				if (titleButton != null && titleButton.State != ButtonState.Inactive)
				{
					titleButton.State = ButtonState.Normal;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		TitleButton titleButton2 = FindButton(x, y);
		if (titleButton2 != null && titleButton2.State != ButtonState.Inactive)
		{
			titleButton2.State = ButtonState.Pushed;
		}
	}

	public void MouseUp(int x, int y)
	{
		if (!Visible)
		{
			return;
		}
		TitleButton titleButton = FindButton(x, y);
		if (titleButton != null && titleButton.State != ButtonState.Inactive)
		{
			titleButton.OnClick();
		}
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TitleButton titleButton2 = (TitleButton)enumerator.Current;
				if (titleButton2 != null && titleButton2.State != ButtonState.Inactive)
				{
					titleButton2.State = ButtonState.Normal;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		if (titleButton == CloseButton && !form.closing)
		{
			XplatUI.InvalidateNC(form.Handle);
		}
		ToolTipHide(reset_hidden_button: true);
	}

	internal void MouseLeave(int x, int y)
	{
		if (!Visible)
		{
			return;
		}
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TitleButton titleButton = (TitleButton)enumerator.Current;
				if (titleButton != null && titleButton.State != ButtonState.Inactive)
				{
					titleButton.State = ButtonState.Normal;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		ToolTipHide(reset_hidden_button: true);
	}
}
