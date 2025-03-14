using System.Drawing;
using System.Text;

namespace System.Windows.Forms;

public class MessageBox
{
	internal class MessageBoxForm : Form
	{
		private const int space_border = 10;

		private const int button_width = 86;

		private const int button_height = 23;

		private const int button_space = 5;

		private const int space_image_text = 10;

		private string msgbox_text;

		private bool size_known;

		private Icon icon_image;

		private RectangleF text_rect;

		private MessageBoxButtons msgbox_buttons;

		private MessageBoxDefaultButton msgbox_default;

		private bool buttons_placed;

		private int button_left;

		private Button[] buttons = new Button[4];

		private bool show_help;

		private string help_file_path;

		private string help_keyword;

		private HelpNavigator help_navigator;

		private object help_param;

		private AlertType alert_type;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.Style |= 113246208;
				if (!is_enabled)
				{
					createParams.Style |= 134217728;
				}
				return createParams;
			}
		}

		internal string HelpFilePath => help_file_path;

		internal string HelpKeyword => help_keyword;

		internal HelpNavigator HelpNavigator => help_navigator;

		internal object HelpParam => help_param;

		internal string UIAMessage => msgbox_text;

		internal Rectangle UIAMessageRectangle => new Rectangle((int)text_rect.X, (int)text_rect.Y, (int)text_rect.Width, (int)text_rect.Height);

		internal Rectangle UIAIconRectangle => new Rectangle(10, 10, (icon_image != null) ? icon_image.Width : (-1), (icon_image != null) ? icon_image.Height : (-1));

		public MessageBoxForm(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, bool displayHelpButton)
		{
			show_help = displayHelpButton;
			switch (icon)
			{
			case MessageBoxIcon.None:
				icon_image = null;
				alert_type = AlertType.Default;
				break;
			case MessageBoxIcon.Error:
				icon_image = SystemIcons.Error;
				alert_type = AlertType.Error;
				break;
			case MessageBoxIcon.Question:
				icon_image = SystemIcons.Question;
				alert_type = AlertType.Question;
				break;
			case MessageBoxIcon.Asterisk:
				icon_image = SystemIcons.Information;
				alert_type = AlertType.Information;
				break;
			case MessageBoxIcon.Exclamation:
				icon_image = SystemIcons.Warning;
				alert_type = AlertType.Warning;
				break;
			}
			msgbox_text = text;
			msgbox_buttons = buttons;
			msgbox_default = MessageBoxDefaultButton.Button1;
			if (owner != null)
			{
				base.Owner = Control.FromHandle(owner.Handle).FindForm();
			}
			else if (Application.MWFThread.Current.Context != null)
			{
				base.Owner = Application.MWFThread.Current.Context.MainForm;
			}
			Text = caption;
			base.ControlBox = true;
			base.MinimizeBox = false;
			base.MaximizeBox = false;
			base.ShowInTaskbar = base.Owner == null;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
		}

		public MessageBoxForm(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
			: this(owner, text, caption, buttons, icon, displayHelpButton)
		{
			msgbox_default = defaultButton;
		}

		public MessageBoxForm(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
			: this(owner, text, caption, buttons, icon, displayHelpButton: false)
		{
		}

		public void SetHelpData(string file_path, string keyword, HelpNavigator navigator, object param)
		{
			help_file_path = file_path;
			help_keyword = keyword;
			help_navigator = navigator;
			help_param = param;
		}

		public DialogResult RunDialog()
		{
			base.StartPosition = FormStartPosition.CenterScreen;
			if (!size_known)
			{
				InitFormsSize();
			}
			if (base.Owner != null)
			{
				base.TopMost = base.Owner.TopMost;
			}
			XplatUI.AudibleAlert(alert_type);
			ShowDialog();
			return base.DialogResult;
		}

		internal override void OnPaintInternal(PaintEventArgs e)
		{
			e.Graphics.DrawString(msgbox_text, Font, ThemeEngine.Current.ResPool.GetSolidBrush(Color.Black), text_rect);
			if (icon_image != null)
			{
				e.Graphics.DrawIcon(icon_image, 10, 10);
			}
		}

		private void InitFormsSize()
		{
			int num = 0;
			int num2 = (int)((double)Screen.GetWorkingArea(this).Width * 0.6);
			SizeF size = TextRenderer.MeasureString(msgbox_text, Font, num2);
			text_rect.Size = size;
			if (icon_image != null)
			{
				size.Width += icon_image.Width + 10;
				if ((float)icon_image.Height > size.Height)
				{
					text_rect.Location = new Point(icon_image.Width + 10 + 10, (int)((float)(icon_image.Height / 2) - size.Height / 2f) + 10);
				}
				else
				{
					text_rect.Location = new Point(icon_image.Width + 10 + 10, 12);
				}
				if (size.Height < (float)icon_image.Height)
				{
					size.Height = icon_image.Height;
				}
			}
			else
			{
				text_rect.Location = new Point(15, 10);
			}
			size.Height += 20f;
			int num3 = msgbox_buttons switch
			{
				MessageBoxButtons.OK => 1, 
				MessageBoxButtons.OKCancel => 2, 
				MessageBoxButtons.AbortRetryIgnore => 3, 
				MessageBoxButtons.YesNoCancel => 3, 
				MessageBoxButtons.YesNo => 2, 
				MessageBoxButtons.RetryCancel => 2, 
				_ => 0, 
			};
			if (show_help)
			{
				num3++;
			}
			num = 91 * num3;
			Size size2 = new SizeF(Math.Min(Math.Max(TextRenderer.MeasureString(Text, new Font(Control.DefaultFont, FontStyle.Bold)).Width + 40f, size.Width), num2), size.Height).ToSize();
			if (size2.Width > num)
			{
				int width = size2.Width + 20;
				int height = (base.Height = size2.Height + 40);
				base.ClientSize = new Size(width, height);
			}
			else
			{
				int width2 = num + 20;
				int height = (base.Height = size2.Height + 40);
				base.ClientSize = new Size(width2, height);
			}
			button_left = base.ClientSize.Width / 2 - num / 2 + 5;
			AddButtons();
			size_known = true;
			switch (msgbox_default)
			{
			case MessageBoxDefaultButton.Button2:
				if (buttons[1] != null)
				{
					ActiveControl = buttons[1];
				}
				break;
			case MessageBoxDefaultButton.Button3:
				if (buttons[2] != null)
				{
					ActiveControl = buttons[2];
				}
				break;
			}
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				CancelClick(this, null);
				return true;
			}
			if ((keyData & Keys.Modifiers) == Keys.Control && ((keyData & Keys.KeyCode) == Keys.C || (keyData & Keys.KeyCode) == Keys.Insert))
			{
				Copy();
			}
			return base.ProcessDialogKey(keyData);
		}

		protected override bool ProcessDialogChar(char charCode)
		{
			if ((charCode == 'N' || charCode == 'n') && base.CancelButton != null && (base.CancelButton as Button).Text == "No")
			{
				base.CancelButton.PerformClick();
			}
			else if ((charCode == 'Y' || charCode == 'y') && (base.AcceptButton as Button).Text == "Yes")
			{
				base.AcceptButton.PerformClick();
			}
			else if ((charCode == 'A' || charCode == 'a') && base.CancelButton != null && (base.CancelButton as Button).Text == "Abort")
			{
				base.CancelButton.PerformClick();
			}
			else if ((charCode == 'R' || charCode == 'r') && (base.AcceptButton as Button).Text == "Retry")
			{
				base.AcceptButton.PerformClick();
			}
			else if ((charCode == 'I' || charCode == 'i') && buttons.Length >= 3 && buttons[2].Text == "Ignore")
			{
				buttons[2].PerformClick();
			}
			return base.ProcessDialogChar(charCode);
		}

		private void Copy()
		{
			string value = "---------------------------" + Environment.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value);
			stringBuilder.Append(Text).Append(Environment.NewLine);
			stringBuilder.Append(value);
			stringBuilder.Append(msgbox_text).Append(Environment.NewLine);
			stringBuilder.Append(value);
			Button[] array = buttons;
			foreach (Button button in array)
			{
				if (button == null)
				{
					break;
				}
				stringBuilder.Append(button.Text).Append("   ");
			}
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(value);
			DataObject dataObject = new DataObject(DataFormats.Text, stringBuilder.ToString());
			Clipboard.SetDataObject(dataObject);
		}

		private void AddButtons()
		{
			if (buttons_placed)
			{
				return;
			}
			switch (msgbox_buttons)
			{
			case MessageBoxButtons.OK:
				buttons[0] = AddOkButton(0);
				break;
			case MessageBoxButtons.OKCancel:
				buttons[0] = AddOkButton(0);
				buttons[1] = AddCancelButton(1);
				break;
			case MessageBoxButtons.AbortRetryIgnore:
				buttons[0] = AddAbortButton(0);
				buttons[1] = AddRetryButton(1);
				buttons[2] = AddIgnoreButton(2);
				break;
			case MessageBoxButtons.YesNoCancel:
				buttons[0] = AddYesButton(0);
				buttons[1] = AddNoButton(1);
				buttons[2] = AddCancelButton(2);
				break;
			case MessageBoxButtons.YesNo:
				buttons[0] = AddYesButton(0);
				buttons[1] = AddNoButton(1);
				break;
			case MessageBoxButtons.RetryCancel:
				buttons[0] = AddRetryButton(0);
				buttons[1] = AddCancelButton(1);
				break;
			}
			if (show_help)
			{
				for (int i = 0; i <= 3; i++)
				{
					if (buttons[i] == null)
					{
						AddHelpButton(i);
						break;
					}
				}
			}
			buttons_placed = true;
		}

		private Button AddButton(string text, int left, EventHandler click_event)
		{
			Button button = new Button();
			button.Text = Locale.GetText(text);
			button.Width = 86;
			button.Height = 23;
			button.Top = base.ClientSize.Height - button.Height - 10;
			button.Left = 91 * left + button_left;
			if (click_event != null)
			{
				button.Click += click_event;
			}
			switch (text)
			{
			case "OK":
			case "Retry":
			case "Yes":
				base.AcceptButton = button;
				break;
			case "Cancel":
			case "Abort":
			case "No":
				base.CancelButton = button;
				break;
			}
			base.Controls.Add(button);
			return button;
		}

		private Button AddOkButton(int left)
		{
			return AddButton("OK", left, OkClick);
		}

		private Button AddCancelButton(int left)
		{
			return AddButton("Cancel", left, CancelClick);
		}

		private Button AddAbortButton(int left)
		{
			return AddButton("Abort", left, AbortClick);
		}

		private Button AddRetryButton(int left)
		{
			return AddButton("Retry", left, RetryClick);
		}

		private Button AddIgnoreButton(int left)
		{
			return AddButton("Ignore", left, IgnoreClick);
		}

		private Button AddYesButton(int left)
		{
			return AddButton("Yes", left, YesClick);
		}

		private Button AddNoButton(int left)
		{
			return AddButton("No", left, NoClick);
		}

		private Button AddHelpButton(int left)
		{
			Button button = AddButton("Help", left, null);
			button.Click += delegate
			{
				base.Owner.RaiseHelpRequested(new HelpEventArgs(base.Owner.Location));
			};
			return button;
		}

		private void OkClick(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}

		private void CancelClick(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void AbortClick(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Abort;
			Close();
		}

		private void RetryClick(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Retry;
			Close();
		}

		private void IgnoreClick(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Ignore;
			Close();
		}

		private void YesClick(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Yes;
			Close();
		}

		private void NoClick(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.No;
			Close();
		}
	}

	private MessageBox()
	{
	}

	public static DialogResult Show(string text)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(IWin32Window owner, string text)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(string text, string caption)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, caption, buttons, MessageBoxIcon.None);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(owner, text, caption, buttons, MessageBoxIcon.None);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(owner, text, caption, buttons, icon);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(IWin32Window owner, string text, string caption)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, caption, buttons, icon);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, caption, buttons, icon, defaultButton, MessageBoxOptions.DefaultDesktopOnly, displayHelpButton: false);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(owner, text, caption, buttons, icon, defaultButton, MessageBoxOptions.DefaultDesktopOnly, displayHelpButton: false);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, caption, buttons, icon, defaultButton, options, displayHelpButton: false);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(owner, text, caption, buttons, icon, defaultButton, options, displayHelpButton: false);
		return messageBoxForm.RunDialog();
	}

	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, caption, buttons, icon, defaultButton, options, displayHelpButton);
		return messageBoxForm.RunDialog();
	}

	[System.MonoTODO("Help is not implemented")]
	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, caption, buttons, icon, defaultButton, options, displayHelpButton: true);
		messageBoxForm.SetHelpData(helpFilePath, null, HelpNavigator.TableOfContents, null);
		return messageBoxForm.RunDialog();
	}

	[System.MonoTODO("Help is not implemented")]
	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, caption, buttons, icon, defaultButton, options, displayHelpButton: true);
		messageBoxForm.SetHelpData(helpFilePath, keyword, HelpNavigator.TableOfContents, null);
		return messageBoxForm.RunDialog();
	}

	[System.MonoTODO("Help is not implemented")]
	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, caption, buttons, icon, defaultButton, options, displayHelpButton: true);
		messageBoxForm.SetHelpData(helpFilePath, null, navigator, null);
		return messageBoxForm.RunDialog();
	}

	[System.MonoTODO("Help is not implemented")]
	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(null, text, caption, buttons, icon, defaultButton, options, displayHelpButton: true);
		messageBoxForm.SetHelpData(helpFilePath, null, navigator, param);
		return messageBoxForm.RunDialog();
	}

	[System.MonoTODO("Help is not implemented")]
	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(owner, text, caption, buttons, icon, defaultButton, options, displayHelpButton: true);
		messageBoxForm.SetHelpData(helpFilePath, null, HelpNavigator.TableOfContents, null);
		return messageBoxForm.RunDialog();
	}

	[System.MonoTODO("Help is not implemented")]
	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(owner, text, caption, buttons, icon, defaultButton, options, displayHelpButton: true);
		messageBoxForm.SetHelpData(helpFilePath, keyword, HelpNavigator.TableOfContents, null);
		return messageBoxForm.RunDialog();
	}

	[System.MonoTODO("Help is not implemented")]
	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(owner, text, caption, buttons, icon, defaultButton, options, displayHelpButton: true);
		messageBoxForm.SetHelpData(helpFilePath, null, navigator, null);
		return messageBoxForm.RunDialog();
	}

	[System.MonoTODO("Help is not implemented")]
	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
	{
		MessageBoxForm messageBoxForm = new MessageBoxForm(owner, text, caption, buttons, icon, defaultButton, options, displayHelpButton: true);
		messageBoxForm.SetHelpData(helpFilePath, null, navigator, param);
		return messageBoxForm.RunDialog();
	}
}
