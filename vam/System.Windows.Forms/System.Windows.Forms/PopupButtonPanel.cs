using System.Drawing;

namespace System.Windows.Forms;

internal class PopupButtonPanel : Control, IUpdateFolder
{
	internal class PopupButton : Control
	{
		internal enum PopupButtonState
		{
			Normal,
			Down,
			Up
		}

		private Image image;

		private PopupButtonState popupButtonState;

		private StringFormat text_format = new StringFormat();

		private Rectangle text_rect = Rectangle.Empty;

		public Image Image
		{
			get
			{
				return image;
			}
			set
			{
				image = value;
				Invalidate();
			}
		}

		public PopupButtonState ButtonState
		{
			get
			{
				return popupButtonState;
			}
			set
			{
				popupButtonState = value;
				Invalidate();
			}
		}

		public PopupButton()
		{
			text_format.Alignment = StringAlignment.Center;
			text_format.LineAlignment = StringAlignment.Near;
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
			SetStyle(ControlStyles.Selectable, value: false);
		}

		internal void PerformClick()
		{
			OnClick(EventArgs.Empty);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			Draw(pe);
			base.OnPaint(pe);
		}

		private void Draw(PaintEventArgs pe)
		{
			Graphics graphics = pe.Graphics;
			graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(BackColor), base.ClientRectangle);
			if (image != null)
			{
				int x = (base.ClientSize.Width - image.Width) / 2;
				int y = 4;
				graphics.DrawImage(image, x, y);
			}
			if (Text != string.Empty)
			{
				if (text_rect == Rectangle.Empty)
				{
					text_rect = new Rectangle(0, base.Height - 30, base.Width, base.Height - 30);
				}
				graphics.DrawString(Text, Font, Brushes.White, text_rect, text_format);
			}
			switch (popupButtonState)
			{
			case PopupButtonState.Up:
				graphics.DrawLine(ThemeEngine.Current.ResPool.GetPen(Color.White), 0, 0, base.ClientSize.Width - 1, 0);
				graphics.DrawLine(ThemeEngine.Current.ResPool.GetPen(Color.White), 0, 0, 0, base.ClientSize.Height - 1);
				graphics.DrawLine(ThemeEngine.Current.ResPool.GetPen(Color.Black), base.ClientSize.Width - 1, 0, base.ClientSize.Width - 1, base.ClientSize.Height - 1);
				graphics.DrawLine(ThemeEngine.Current.ResPool.GetPen(Color.Black), 0, base.ClientSize.Height - 1, base.ClientSize.Width - 1, base.ClientSize.Height - 1);
				break;
			case PopupButtonState.Down:
				graphics.DrawLine(ThemeEngine.Current.ResPool.GetPen(Color.Black), 0, 0, base.ClientSize.Width - 1, 0);
				graphics.DrawLine(ThemeEngine.Current.ResPool.GetPen(Color.Black), 0, 0, 0, base.ClientSize.Height - 1);
				graphics.DrawLine(ThemeEngine.Current.ResPool.GetPen(Color.White), base.ClientSize.Width - 1, 0, base.ClientSize.Width - 1, base.ClientSize.Height - 1);
				graphics.DrawLine(ThemeEngine.Current.ResPool.GetPen(Color.White), 0, base.ClientSize.Height - 1, base.ClientSize.Width - 1, base.ClientSize.Height - 1);
				break;
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			if (popupButtonState != PopupButtonState.Down)
			{
				popupButtonState = PopupButtonState.Up;
			}
			PopupButtonPanel popupButtonPanel = base.Parent as PopupButtonPanel;
			if (popupButtonPanel.focusButton != null && popupButtonPanel.focusButton.ButtonState == PopupButtonState.Up)
			{
				popupButtonPanel.focusButton.ButtonState = PopupButtonState.Normal;
				popupButtonPanel.SetFocusButton(null);
			}
			Invalidate();
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (popupButtonState == PopupButtonState.Up)
			{
				popupButtonState = PopupButtonState.Normal;
			}
			Invalidate();
			base.OnMouseLeave(e);
		}

		protected override void OnClick(EventArgs e)
		{
			popupButtonState = PopupButtonState.Down;
			Invalidate();
			base.OnClick(e);
		}
	}

	private PopupButton recentlyusedButton;

	private PopupButton desktopButton;

	private PopupButton personalButton;

	private PopupButton mycomputerButton;

	private PopupButton networkButton;

	private PopupButton lastPopupButton;

	private PopupButton focusButton;

	private string currentPath;

	private int currentFocusIndex;

	private static object UIAFocusedItemChangedEvent;

	private static object PDirectoryChangedEvent;

	internal PopupButton UIAFocusButton => focusButton;

	public string CurrentFolder
	{
		get
		{
			return currentPath;
		}
		set
		{
			if (value == MWFVFS.RecentlyUsedPrefix)
			{
				if (lastPopupButton != recentlyusedButton)
				{
					if (lastPopupButton != null)
					{
						lastPopupButton.ButtonState = PopupButton.PopupButtonState.Normal;
					}
					recentlyusedButton.ButtonState = PopupButton.PopupButtonState.Down;
					lastPopupButton = recentlyusedButton;
				}
			}
			else if (value == MWFVFS.DesktopPrefix)
			{
				if (lastPopupButton != desktopButton)
				{
					if (lastPopupButton != null)
					{
						lastPopupButton.ButtonState = PopupButton.PopupButtonState.Normal;
					}
					desktopButton.ButtonState = PopupButton.PopupButtonState.Down;
					lastPopupButton = desktopButton;
				}
			}
			else if (value == MWFVFS.PersonalPrefix)
			{
				if (lastPopupButton != personalButton)
				{
					if (lastPopupButton != null)
					{
						lastPopupButton.ButtonState = PopupButton.PopupButtonState.Normal;
					}
					personalButton.ButtonState = PopupButton.PopupButtonState.Down;
					lastPopupButton = personalButton;
				}
			}
			else if (value == MWFVFS.MyComputerPrefix)
			{
				if (lastPopupButton != mycomputerButton)
				{
					if (lastPopupButton != null)
					{
						lastPopupButton.ButtonState = PopupButton.PopupButtonState.Normal;
					}
					mycomputerButton.ButtonState = PopupButton.PopupButtonState.Down;
					lastPopupButton = mycomputerButton;
				}
			}
			else if (value == MWFVFS.MyNetworkPrefix)
			{
				if (lastPopupButton != networkButton)
				{
					if (lastPopupButton != null)
					{
						lastPopupButton.ButtonState = PopupButton.PopupButtonState.Normal;
					}
					networkButton.ButtonState = PopupButton.PopupButtonState.Down;
					lastPopupButton = networkButton;
				}
			}
			else if (lastPopupButton != null)
			{
				lastPopupButton.ButtonState = PopupButton.PopupButtonState.Normal;
				lastPopupButton = null;
			}
		}
	}

	internal event EventHandler UIAFocusedItemChanged
	{
		add
		{
			base.Events.AddHandler(UIAFocusedItemChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAFocusedItemChangedEvent, value);
		}
	}

	public event EventHandler DirectoryChanged
	{
		add
		{
			base.Events.AddHandler(PDirectoryChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PDirectoryChangedEvent, value);
		}
	}

	public PopupButtonPanel()
	{
		SuspendLayout();
		BackColor = Color.FromArgb(128, 128, 128);
		base.Size = new Size(85, 336);
		base.InternalBorderStyle = BorderStyle.Fixed3D;
		recentlyusedButton = new PopupButton();
		desktopButton = new PopupButton();
		personalButton = new PopupButton();
		mycomputerButton = new PopupButton();
		networkButton = new PopupButton();
		recentlyusedButton.Size = new Size(81, 64);
		recentlyusedButton.Image = ThemeEngine.Current.Images(UIIcon.PlacesRecentDocuments, 32);
		recentlyusedButton.BackColor = BackColor;
		recentlyusedButton.ForeColor = Color.Black;
		recentlyusedButton.Location = new Point(2, 2);
		recentlyusedButton.Text = "Recently\nused";
		recentlyusedButton.Click += OnClickButton;
		desktopButton.Image = ThemeEngine.Current.Images(UIIcon.PlacesDesktop, 32);
		desktopButton.BackColor = BackColor;
		desktopButton.ForeColor = Color.Black;
		desktopButton.Size = new Size(81, 64);
		desktopButton.Location = new Point(2, 66);
		desktopButton.Text = "Desktop";
		desktopButton.Click += OnClickButton;
		personalButton.Image = ThemeEngine.Current.Images(UIIcon.PlacesPersonal, 32);
		personalButton.BackColor = BackColor;
		personalButton.ForeColor = Color.Black;
		personalButton.Size = new Size(81, 64);
		personalButton.Location = new Point(2, 130);
		personalButton.Text = "Personal";
		personalButton.Click += OnClickButton;
		mycomputerButton.Image = ThemeEngine.Current.Images(UIIcon.PlacesMyComputer, 32);
		mycomputerButton.BackColor = BackColor;
		mycomputerButton.ForeColor = Color.Black;
		mycomputerButton.Size = new Size(81, 64);
		mycomputerButton.Location = new Point(2, 194);
		mycomputerButton.Text = "My Computer";
		mycomputerButton.Click += OnClickButton;
		networkButton.Image = ThemeEngine.Current.Images(UIIcon.PlacesMyNetwork, 32);
		networkButton.BackColor = BackColor;
		networkButton.ForeColor = Color.Black;
		networkButton.Size = new Size(81, 64);
		networkButton.Location = new Point(2, 258);
		networkButton.Text = "My Network";
		networkButton.Click += OnClickButton;
		base.Controls.Add(recentlyusedButton);
		base.Controls.Add(desktopButton);
		base.Controls.Add(personalButton);
		base.Controls.Add(mycomputerButton);
		base.Controls.Add(networkButton);
		ResumeLayout(performLayout: false);
		base.KeyDown += Key_Down;
		SetStyle(ControlStyles.StandardClick, value: false);
	}

	static PopupButtonPanel()
	{
		UIAFocusedItemChanged = new object();
		PDirectoryChangedEvent = new object();
	}

	private void OnClickButton(object sender, EventArgs e)
	{
		if (lastPopupButton != null && lastPopupButton != sender as PopupButton)
		{
			lastPopupButton.ButtonState = PopupButton.PopupButtonState.Normal;
		}
		lastPopupButton = sender as PopupButton;
		if (sender == recentlyusedButton)
		{
			currentPath = MWFVFS.RecentlyUsedPrefix;
		}
		else if (sender == desktopButton)
		{
			currentPath = MWFVFS.DesktopPrefix;
		}
		else if (sender == personalButton)
		{
			currentPath = MWFVFS.PersonalPrefix;
		}
		else if (sender == mycomputerButton)
		{
			currentPath = MWFVFS.MyComputerPrefix;
		}
		else if (sender == networkButton)
		{
			currentPath = MWFVFS.MyNetworkPrefix;
		}
		((EventHandler)base.Events[PDirectoryChangedEvent])?.Invoke(this, EventArgs.Empty);
	}

	internal void OnUIAFocusedItemChanged()
	{
		((EventHandler)base.Events[UIAFocusedItemChanged])?.Invoke(this, EventArgs.Empty);
	}

	protected override void OnGotFocus(EventArgs e)
	{
		if (lastPopupButton != recentlyusedButton)
		{
			recentlyusedButton.ButtonState = PopupButton.PopupButtonState.Up;
			SetFocusButton(recentlyusedButton);
		}
		currentFocusIndex = 0;
		base.OnGotFocus(e);
	}

	protected override void OnLostFocus(EventArgs e)
	{
		if (focusButton != null && focusButton.ButtonState != PopupButton.PopupButtonState.Down)
		{
			focusButton.ButtonState = PopupButton.PopupButtonState.Normal;
		}
		base.OnLostFocus(e);
	}

	protected override bool IsInputKey(Keys key)
	{
		switch (key)
		{
		case Keys.Return:
		case Keys.Left:
		case Keys.Up:
		case Keys.Right:
		case Keys.Down:
			return true;
		default:
			return base.IsInputKey(key);
		}
	}

	private void Key_Down(object sender, KeyEventArgs e)
	{
		bool flag = false;
		if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
		{
			currentFocusIndex--;
			if (currentFocusIndex < 0)
			{
				currentFocusIndex = base.Controls.Count - 1;
			}
			flag = true;
		}
		else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right)
		{
			currentFocusIndex++;
			if (currentFocusIndex == base.Controls.Count)
			{
				currentFocusIndex = 0;
			}
			flag = true;
		}
		else if (e.KeyCode == Keys.Return)
		{
			focusButton.ButtonState = PopupButton.PopupButtonState.Down;
			OnClickButton(focusButton, EventArgs.Empty);
		}
		if (flag)
		{
			PopupButton popupButton = base.Controls[currentFocusIndex] as PopupButton;
			if (focusButton != null && focusButton.ButtonState != PopupButton.PopupButtonState.Down)
			{
				focusButton.ButtonState = PopupButton.PopupButtonState.Normal;
			}
			if (popupButton.ButtonState != PopupButton.PopupButtonState.Down)
			{
				popupButton.ButtonState = PopupButton.PopupButtonState.Up;
			}
			SetFocusButton(popupButton);
		}
		e.Handled = true;
	}

	internal void SetFocusButton(PopupButton button)
	{
		if (button != focusButton)
		{
			focusButton = button;
			OnUIAFocusedItemChanged();
		}
	}
}
