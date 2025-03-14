using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
[Designer("System.Windows.Forms.Design.TextBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
public class TextBox : TextBoxBase
{
	private class AutoCompleteListBox : Control
	{
		private const int DefaultDropDownItems = 7;

		private TextBox owner;

		private VScrollBar vscroll;

		private int top_item;

		private int last_item;

		internal int page_size;

		private int item_height;

		private int highlighted_index = -1;

		private bool user_defined_size;

		private bool resizing;

		private Rectangle resizer_bounds;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.Style ^= 1073741824;
				createParams.Style ^= 268435456;
				createParams.Style |= int.MinValue;
				createParams.ExStyle |= 136;
				return createParams;
			}
		}

		public int HighlightedIndex
		{
			get
			{
				return highlighted_index;
			}
			set
			{
				if (value != highlighted_index)
				{
					if (highlighted_index != -1)
					{
						Invalidate(GetItemBounds(highlighted_index));
					}
					highlighted_index = value;
					if (highlighted_index != -1)
					{
						Invalidate(GetItemBounds(highlighted_index));
					}
					if (highlighted_index != -1)
					{
						EnsureVisible(highlighted_index);
					}
				}
			}
		}

		internal override bool ActivateOnShow => false;

		public AutoCompleteListBox(TextBox tb)
		{
			owner = tb;
			item_height = base.FontHeight + 2;
			vscroll = new VScrollBar();
			vscroll.ValueChanged += VScrollValueChanged;
			base.Controls.Add(vscroll);
			is_visible = false;
			base.InternalBorderStyle = BorderStyle.FixedSingle;
		}

		public void Scroll(int lines)
		{
			int num = vscroll.Maximum - page_size + 1;
			int num2 = vscroll.Value + lines;
			if (num2 > num)
			{
				num2 = num;
			}
			else if (num2 < vscroll.Minimum)
			{
				num2 = vscroll.Minimum;
			}
			vscroll.Value = num2;
		}

		public void EnsureVisible(int index)
		{
			if (index < top_item)
			{
				vscroll.Value = index;
				return;
			}
			int num = vscroll.Maximum - page_size + 1;
			int num2 = base.Height / item_height;
			if (index > top_item + num2 - 1)
			{
				index = index - num2 + 1;
				vscroll.Value = ((index <= num) ? index : num);
			}
		}

		private void VScrollValueChanged(object o, EventArgs args)
		{
			if (top_item != vscroll.Value)
			{
				top_item = vscroll.Value;
				last_item = GetLastVisibleItem();
				Invalidate();
			}
		}

		private int GetLastVisibleItem()
		{
			int height = base.Height;
			for (int i = top_item; i < owner.auto_complete_matches.Count; i++)
			{
				int num = i - top_item;
				if (num * item_height + item_height >= height)
				{
					return i;
				}
			}
			return owner.auto_complete_matches.Count - 1;
		}

		private Rectangle GetItemBounds(int index)
		{
			int num = index - top_item;
			Rectangle result = new Rectangle(0, num * item_height, base.Width, item_height);
			if (vscroll.Visible)
			{
				result.Width -= vscroll.Width;
			}
			return result;
		}

		private int GetItemAt(Point loc)
		{
			if (loc.Y > (last_item - top_item) * item_height + item_height)
			{
				return -1;
			}
			int num = loc.Y / item_height;
			return num + top_item;
		}

		private void LayoutListBox()
		{
			int num = owner.auto_complete_matches.Count * item_height;
			page_size = Math.Max(base.Height / item_height, 1);
			last_item = GetLastVisibleItem();
			if (base.Height < num)
			{
				vscroll.Visible = true;
				vscroll.Maximum = owner.auto_complete_matches.Count - 1;
				vscroll.LargeChange = page_size;
				vscroll.Location = new Point(base.Width - vscroll.Width, 0);
				vscroll.Height = base.Height - item_height;
			}
			else
			{
				vscroll.Visible = false;
			}
			resizer_bounds = new Rectangle(base.Width - item_height, base.Height - item_height, item_height, item_height);
		}

		public void HideListBox(bool set_text)
		{
			if (set_text)
			{
				owner.Text = owner.auto_complete_matches[HighlightedIndex];
			}
			base.Capture = false;
			Hide();
		}

		public void ShowListBox()
		{
			if (!user_defined_size)
			{
				int height = ((owner.auto_complete_matches.Count <= 7) ? ((owner.auto_complete_matches.Count + 1) * item_height) : (7 * item_height));
				base.Size = new Size(owner.Width, height);
			}
			else
			{
				LayoutListBox();
			}
			vscroll.Value = 0;
			HighlightedIndex = -1;
			Show();
			XplatUI.SetZOrder(Handle, IntPtr.Zero, Top: true, Bottom: false);
			Invalidate();
		}

		protected override void OnResize(EventArgs args)
		{
			base.OnResize(args);
			LayoutListBox();
			Refresh();
		}

		protected override void OnMouseDown(MouseEventArgs args)
		{
			base.OnMouseDown(args);
			if (resizer_bounds.Contains(args.Location))
			{
				user_defined_size = true;
				resizing = true;
				base.Capture = true;
			}
		}

		protected override void OnMouseMove(MouseEventArgs args)
		{
			base.OnMouseMove(args);
			if (resizing)
			{
				Point mousePosition = Control.MousePosition;
				Point point = PointToScreen(Point.Empty);
				Size size = new Size(mousePosition.X - point.X, mousePosition.Y - point.Y);
				if (size.Height < item_height)
				{
					size.Height = item_height;
				}
				if (size.Width < item_height)
				{
					size.Width = item_height;
				}
				base.Size = size;
			}
			else
			{
				Cursor = ((!resizer_bounds.Contains(args.Location)) ? Cursors.Default : Cursors.SizeNWSE);
				int itemAt = GetItemAt(args.Location);
				if (itemAt != -1)
				{
					HighlightedIndex = itemAt;
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs args)
		{
			base.OnMouseUp(args);
			int itemAt = GetItemAt(args.Location);
			if (itemAt != -1 && !resizing)
			{
				HideListBox(set_text: true);
			}
			owner.OnAutoCompleteValueSelected(EventArgs.Empty);
			resizing = false;
			base.Capture = false;
		}

		internal override void OnPaintInternal(PaintEventArgs args)
		{
			Graphics graphics = args.Graphics;
			Brush solidBrush = ThemeEngine.Current.ResPool.GetSolidBrush(ForeColor);
			int highlightedIndex = HighlightedIndex;
			int num = 0;
			int lastVisibleItem = GetLastVisibleItem();
			for (int i = top_item; i <= lastVisibleItem; i++)
			{
				Rectangle itemBounds = GetItemBounds(i);
				if (itemBounds.IntersectsWith(args.ClipRectangle))
				{
					if (i == highlightedIndex)
					{
						graphics.FillRectangle(SystemBrushes.Highlight, itemBounds);
						graphics.DrawString(owner.auto_complete_matches[i], Font, SystemBrushes.HighlightText, itemBounds);
					}
					else
					{
						graphics.DrawString(owner.auto_complete_matches[i], Font, solidBrush, itemBounds);
					}
					num += item_height;
				}
			}
			ThemeEngine.Current.CPDrawSizeGrip(graphics, SystemColors.Control, resizer_bounds);
		}
	}

	private ContextMenu menu;

	private MenuItem undo;

	private MenuItem cut;

	private MenuItem copy;

	private MenuItem paste;

	private MenuItem delete;

	private MenuItem select_all;

	private bool use_system_password_char;

	private AutoCompleteStringCollection auto_complete_custom_source;

	private AutoCompleteMode auto_complete_mode;

	private AutoCompleteSource auto_complete_source = AutoCompleteSource.None;

	private AutoCompleteListBox auto_complete_listbox;

	private string auto_complete_original_text;

	private int auto_complete_selected_index = -1;

	private List<string> auto_complete_matches;

	private ComboBox auto_complete_cb_source;

	private static object TextAlignChangedEvent;

	internal bool IsAutoCompleteAvailable
	{
		get
		{
			if (auto_complete_source == AutoCompleteSource.None || auto_complete_mode == AutoCompleteMode.None)
			{
				return false;
			}
			if (auto_complete_source != AutoCompleteSource.CustomSource)
			{
				return false;
			}
			object obj;
			if (auto_complete_cb_source == null)
			{
				IList list = auto_complete_custom_source;
				obj = list;
			}
			else
			{
				obj = auto_complete_cb_source.Items;
			}
			IList list2 = (IList)obj;
			if (list2 == null || list2.Count == 0)
			{
				return false;
			}
			return true;
		}
	}

	internal ComboBox AutoCompleteInternalSource
	{
		get
		{
			return auto_complete_cb_source;
		}
		set
		{
			auto_complete_cb_source = value;
		}
	}

	internal bool CanNavigateAutoCompleteList
	{
		get
		{
			if (auto_complete_mode == AutoCompleteMode.None)
			{
				return false;
			}
			if (auto_complete_matches == null || auto_complete_matches.Count == 0)
			{
				return false;
			}
			bool flag = auto_complete_listbox != null && auto_complete_listbox.Visible;
			if (auto_complete_mode == AutoCompleteMode.Suggest && !flag)
			{
				return false;
			}
			return true;
		}
	}

	[Localizable(true)]
	[System.MonoTODO("AutoCompletion algorithm is currently not implemented.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public AutoCompleteStringCollection AutoCompleteCustomSource
	{
		get
		{
			if (auto_complete_custom_source == null)
			{
				auto_complete_custom_source = new AutoCompleteStringCollection();
				auto_complete_custom_source.CollectionChanged += OnAutoCompleteCustomSourceChanged;
			}
			return auto_complete_custom_source;
		}
		set
		{
			if (auto_complete_custom_source != value)
			{
				if (auto_complete_custom_source != null)
				{
					auto_complete_custom_source.CollectionChanged -= OnAutoCompleteCustomSourceChanged;
				}
				auto_complete_custom_source = value;
				if (auto_complete_custom_source != null)
				{
					auto_complete_custom_source.CollectionChanged += OnAutoCompleteCustomSourceChanged;
				}
			}
		}
	}

	[DefaultValue(AutoCompleteMode.None)]
	[System.MonoTODO("AutoCompletion algorithm is currently not implemented.")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public AutoCompleteMode AutoCompleteMode
	{
		get
		{
			return auto_complete_mode;
		}
		set
		{
			if (auto_complete_mode != value)
			{
				if (value < AutoCompleteMode.None || value > AutoCompleteMode.SuggestAppend)
				{
					throw new InvalidEnumArgumentException(Locale.GetText("Enum argument value '{0}' is not valid for AutoCompleteMode", value));
				}
				auto_complete_mode = value;
			}
		}
	}

	[DefaultValue(AutoCompleteSource.None)]
	[System.MonoTODO("AutoCompletion algorithm is currently not implemented.")]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[TypeConverter(typeof(TextBoxAutoCompleteSourceConverter))]
	[Browsable(true)]
	public AutoCompleteSource AutoCompleteSource
	{
		get
		{
			return auto_complete_source;
		}
		set
		{
			if (auto_complete_source != value)
			{
				if (!Enum.IsDefined(typeof(AutoCompleteSource), value))
				{
					throw new InvalidEnumArgumentException(Locale.GetText("Enum argument value '{0}' is not valid for AutoCompleteSource", value));
				}
				auto_complete_source = value;
			}
		}
	}

	[DefaultValue(false)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public bool UseSystemPasswordChar
	{
		get
		{
			return use_system_password_char;
		}
		set
		{
			if (use_system_password_char != value)
			{
				use_system_password_char = value;
				if (!Multiline)
				{
					document.PasswordChar = PasswordChar.ToString();
				}
				else
				{
					document.PasswordChar = string.Empty;
				}
				Invalidate();
			}
		}
	}

	[MWFCategory("Behavior")]
	[DefaultValue(false)]
	public bool AcceptsReturn
	{
		get
		{
			return accepts_return;
		}
		set
		{
			if (value != accepts_return)
			{
				accepts_return = value;
			}
		}
	}

	[DefaultValue(CharacterCasing.Normal)]
	[MWFCategory("Behavior")]
	public CharacterCasing CharacterCasing
	{
		get
		{
			return character_casing;
		}
		set
		{
			if (value != character_casing)
			{
				character_casing = value;
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[Localizable(true)]
	[DefaultValue('\0')]
	[MWFCategory("Behavior")]
	public char PasswordChar
	{
		get
		{
			if (use_system_password_char)
			{
				return '*';
			}
			return password_char;
		}
		set
		{
			if (value != password_char)
			{
				password_char = value;
				if (!Multiline)
				{
					document.PasswordChar = PasswordChar.ToString();
				}
				else
				{
					document.PasswordChar = string.Empty;
				}
				CalculateDocument();
			}
		}
	}

	[MWFCategory("Appearance")]
	[Localizable(true)]
	[DefaultValue(ScrollBars.None)]
	public ScrollBars ScrollBars
	{
		get
		{
			return (ScrollBars)scrollbars;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ScrollBars), value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(ScrollBars));
			}
			if (value != (ScrollBars)scrollbars)
			{
				scrollbars = (RichTextBoxScrollBars)value;
				CalculateScrollBars();
			}
		}
	}

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	[DefaultValue(HorizontalAlignment.Left)]
	[Localizable(true)]
	[MWFCategory("Appearance")]
	public HorizontalAlignment TextAlign
	{
		get
		{
			return alignment;
		}
		set
		{
			if (value != alignment)
			{
				alignment = value;
				UpdateAlignment();
				OnTextAlignChanged(EventArgs.Empty);
			}
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	internal override ContextMenu ContextMenuInternal
	{
		get
		{
			ContextMenu contextMenuInternal = base.ContextMenuInternal;
			if (contextMenuInternal == menu)
			{
				return null;
			}
			return contextMenuInternal;
		}
		set
		{
			base.ContextMenuInternal = value;
		}
	}

	public override bool Multiline
	{
		get
		{
			return base.Multiline;
		}
		set
		{
			base.Multiline = value;
		}
	}

	public event EventHandler TextAlignChanged
	{
		add
		{
			base.Events.AddHandler(TextAlignChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(TextAlignChangedEvent, value);
		}
	}

	public TextBox()
	{
		scrollbars = RichTextBoxScrollBars.None;
		alignment = HorizontalAlignment.Left;
		base.LostFocus += TextBox_LostFocus;
		base.RightToLeftChanged += TextBox_RightToLeftChanged;
		base.MouseWheel += TextBox_MouseWheel;
		BackColor = SystemColors.Window;
		ForeColor = SystemColors.WindowText;
		backcolor_set = false;
		SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, value: false);
		SetStyle(ControlStyles.FixedHeight, value: true);
		undo = new MenuItem(Locale.GetText("&Undo"));
		cut = new MenuItem(Locale.GetText("Cu&t"));
		copy = new MenuItem(Locale.GetText("&Copy"));
		paste = new MenuItem(Locale.GetText("&Paste"));
		delete = new MenuItem(Locale.GetText("&Delete"));
		select_all = new MenuItem(Locale.GetText("Select &All"));
		menu = new ContextMenu(new MenuItem[8]
		{
			undo,
			new MenuItem("-"),
			cut,
			copy,
			paste,
			delete,
			new MenuItem("-"),
			select_all
		});
		ContextMenu = menu;
		menu.Popup += menu_Popup;
		undo.Click += undo_Click;
		cut.Click += cut_Click;
		copy.Click += copy_Click;
		paste.Click += paste_Click;
		delete.Click += delete_Click;
		select_all.Click += select_all_Click;
		document.multiline = false;
	}

	static TextBox()
	{
		TextAlignChanged = new object();
	}

	private void TextBox_RightToLeftChanged(object sender, EventArgs e)
	{
		UpdateAlignment();
	}

	private void TextBox_LostFocus(object sender, EventArgs e)
	{
		if (hide_selection)
		{
			document.InvalidateSelectionArea();
		}
		if (auto_complete_listbox != null && auto_complete_listbox.Visible)
		{
			auto_complete_listbox.HideListBox(set_text: false);
		}
	}

	private void TextBox_MouseWheel(object o, MouseEventArgs args)
	{
		if (auto_complete_listbox != null && auto_complete_listbox.Visible)
		{
			int num = args.Delta / 120;
			auto_complete_listbox.Scroll(-num);
		}
	}

	private void ProcessAutoCompleteInput(ref Message m, bool deleting_chars)
	{
		base.WndProc(ref m);
		auto_complete_original_text = Text;
		ShowAutoCompleteListBox(deleting_chars);
	}

	private void ShowAutoCompleteListBox(bool deleting_chars)
	{
		object obj;
		if (auto_complete_cb_source == null)
		{
			IList list = auto_complete_custom_source;
			obj = list;
		}
		else
		{
			obj = auto_complete_cb_source.Items;
		}
		IList list2 = (IList)obj;
		bool flag = auto_complete_mode == AutoCompleteMode.Append || auto_complete_mode == AutoCompleteMode.SuggestAppend;
		bool flag2 = auto_complete_mode == AutoCompleteMode.Suggest || auto_complete_mode == AutoCompleteMode.SuggestAppend;
		if (Text.Length == 0)
		{
			if (auto_complete_listbox != null)
			{
				auto_complete_listbox.HideListBox(set_text: false);
			}
			return;
		}
		if (auto_complete_matches == null)
		{
			auto_complete_matches = new List<string>();
		}
		string value = Text;
		auto_complete_matches.Clear();
		for (int i = 0; i < list2.Count; i++)
		{
			string text = ((auto_complete_cb_source != null) ? auto_complete_cb_source.GetItemText(auto_complete_cb_source.Items[i]) : auto_complete_custom_source[i]);
			if (text.StartsWith(value, StringComparison.CurrentCultureIgnoreCase))
			{
				auto_complete_matches.Add(text);
			}
		}
		auto_complete_matches.Sort();
		if (auto_complete_matches.Count == 0 || (auto_complete_matches.Count == 1 && auto_complete_matches[0].Equals(value, StringComparison.CurrentCultureIgnoreCase)))
		{
			if (auto_complete_listbox != null && auto_complete_listbox.Visible)
			{
				auto_complete_listbox.HideListBox(set_text: false);
			}
			return;
		}
		auto_complete_selected_index = (flag2 ? (-1) : 0);
		if (flag2)
		{
			if (auto_complete_listbox == null)
			{
				auto_complete_listbox = new AutoCompleteListBox(this);
			}
			auto_complete_listbox.Location = PointToScreen(new Point(0, base.Height));
			auto_complete_listbox.ShowListBox();
		}
		if (flag && !deleting_chars)
		{
			AppendAutoCompleteMatch(0);
		}
		document.MoveCaret(CaretDirection.End);
	}

	internal void HideAutoCompleteList()
	{
		if (auto_complete_listbox != null)
		{
			auto_complete_listbox.HideListBox(set_text: false);
		}
	}

	private bool NavigateAutoCompleteList(Keys key)
	{
		if (auto_complete_matches == null || auto_complete_matches.Count == 0)
		{
			return false;
		}
		bool flag = auto_complete_listbox != null && auto_complete_listbox.Visible;
		if (!flag && auto_complete_mode == AutoCompleteMode.Suggest)
		{
			return false;
		}
		int num = auto_complete_selected_index;
		switch (key)
		{
		case Keys.Up:
			num--;
			if (num < -1)
			{
				num = auto_complete_matches.Count - 1;
			}
			break;
		case Keys.Down:
			num++;
			if (num >= auto_complete_matches.Count)
			{
				num = -1;
			}
			break;
		case Keys.PageUp:
			if (auto_complete_mode == AutoCompleteMode.Append || !flag)
			{
				goto case Keys.Up;
			}
			switch (num)
			{
			case -1:
				num = auto_complete_matches.Count - 1;
				break;
			case 0:
				num = -1;
				break;
			default:
				num -= auto_complete_listbox.page_size - 1;
				if (num < 0)
				{
					num = 0;
				}
				break;
			}
			break;
		case Keys.PageDown:
			if (auto_complete_mode == AutoCompleteMode.Append || !flag)
			{
				goto case Keys.Down;
			}
			if (num == -1)
			{
				num = 0;
				break;
			}
			if (num == auto_complete_matches.Count - 1)
			{
				num = -1;
				break;
			}
			num += auto_complete_listbox.page_size - 1;
			if (num >= auto_complete_matches.Count)
			{
				num = auto_complete_matches.Count - 1;
			}
			break;
		}
		if ((auto_complete_mode == AutoCompleteMode.Suggest || auto_complete_mode == AutoCompleteMode.SuggestAppend) && flag)
		{
			Text = ((num != -1) ? auto_complete_matches[num] : auto_complete_original_text);
			auto_complete_listbox.HighlightedIndex = num;
		}
		else
		{
			AppendAutoCompleteMatch((num >= 0) ? num : 0);
		}
		auto_complete_selected_index = num;
		document.MoveCaret(CaretDirection.End);
		return true;
	}

	private void AppendAutoCompleteMatch(int index)
	{
		Text = auto_complete_original_text + auto_complete_matches[index].Substring(auto_complete_original_text.Length);
		base.SelectionStart = auto_complete_original_text.Length;
		SelectionLength = auto_complete_matches[index].Length - auto_complete_original_text.Length;
	}

	internal virtual void OnAutoCompleteValueSelected(EventArgs args)
	{
	}

	private void UpdateAlignment()
	{
		HorizontalAlignment horizontalAlignment = alignment;
		RightToLeft inheritedRtoL = GetInheritedRtoL();
		if (inheritedRtoL == RightToLeft.Yes)
		{
			switch (horizontalAlignment)
			{
			case HorizontalAlignment.Left:
				horizontalAlignment = HorizontalAlignment.Right;
				break;
			case HorizontalAlignment.Right:
				horizontalAlignment = HorizontalAlignment.Left;
				break;
			}
		}
		document.alignment = horizontalAlignment;
		if (Multiline)
		{
			if (alignment != 0)
			{
				document.Wrap = true;
			}
			else
			{
				document.Wrap = word_wrap;
			}
		}
		for (int i = 1; i <= document.Lines; i++)
		{
			document.GetLine(i).Alignment = horizontalAlignment;
		}
		document.RecalculateDocument(CreateGraphicsInternal());
		Invalidate();
	}

	internal override Color ChangeBackColor(Color backColor)
	{
		if (backColor == Color.Empty)
		{
			if (!base.ReadOnly)
			{
				backColor = SystemColors.Window;
			}
			backcolor_set = false;
		}
		return backColor;
	}

	private void OnAutoCompleteCustomSourceChanged(object sender, CollectionChangeEventArgs e)
	{
		if (auto_complete_source != AutoCompleteSource.CustomSource)
		{
		}
	}

	public void Paste(string text)
	{
		document.ReplaceSelection(CaseAdjust(text), select_new: false);
		ScrollToCaret();
		OnTextChanged(EventArgs.Empty);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected override bool IsInputKey(Keys keyData)
	{
		return base.IsInputKey(keyData);
	}

	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		if (selection_length == -1 && !has_been_focused)
		{
			SelectAllNoScroll();
		}
		has_been_focused = true;
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	protected virtual void OnTextAlignChanged(EventArgs e)
	{
		((EventHandler)base.Events[TextAlignChanged])?.Invoke(this, e);
	}

	protected override void WndProc(ref Message m)
	{
		switch ((Msg)m.Msg)
		{
		case Msg.WM_KEYDOWN:
		{
			if (!IsAutoCompleteAvailable)
			{
				break;
			}
			Keys keys = (Keys)m.WParam.ToInt32();
			switch (keys)
			{
			case Keys.PageUp:
			case Keys.PageDown:
			case Keys.Up:
			case Keys.Down:
				if (NavigateAutoCompleteList(keys))
				{
					m.Result = IntPtr.Zero;
					return;
				}
				break;
			case Keys.Return:
				if (auto_complete_listbox != null && auto_complete_listbox.Visible)
				{
					auto_complete_listbox.HideListBox(set_text: false);
				}
				SelectAll();
				break;
			case Keys.Escape:
				if (auto_complete_listbox != null && auto_complete_listbox.Visible)
				{
					auto_complete_listbox.HideListBox(set_text: false);
				}
				break;
			case Keys.Delete:
				ProcessAutoCompleteInput(ref m, deleting_chars: true);
				return;
			}
			break;
		}
		case Msg.WM_CHAR:
			if (IsAutoCompleteAvailable)
			{
				int num = m.WParam.ToInt32();
				if (num != 13 && num != 27)
				{
					ProcessAutoCompleteInput(ref m, num == 8);
					return;
				}
			}
			break;
		case Msg.WM_LBUTTONDOWN:
			has_been_focused = true;
			FocusInternal(skip_check: true);
			break;
		}
		base.WndProc(ref m);
	}

	internal void RestoreContextMenu()
	{
		ContextMenuInternal = menu;
	}

	private void menu_Popup(object sender, EventArgs e)
	{
		if (SelectionLength == 0)
		{
			cut.Enabled = false;
			copy.Enabled = false;
		}
		else
		{
			cut.Enabled = true;
			copy.Enabled = true;
		}
		if (SelectionLength == TextLength)
		{
			select_all.Enabled = false;
		}
		else
		{
			select_all.Enabled = true;
		}
		if (!base.CanUndo)
		{
			undo.Enabled = false;
		}
		else
		{
			undo.Enabled = true;
		}
		if (base.ReadOnly)
		{
			MenuItem menuItem = undo;
			bool flag = false;
			delete.Enabled = flag;
			flag = flag;
			paste.Enabled = flag;
			flag = flag;
			cut.Enabled = flag;
			menuItem.Enabled = flag;
		}
	}

	private void undo_Click(object sender, EventArgs e)
	{
		Undo();
	}

	private void cut_Click(object sender, EventArgs e)
	{
		Cut();
	}

	private void copy_Click(object sender, EventArgs e)
	{
		Copy();
	}

	private void paste_Click(object sender, EventArgs e)
	{
		Paste();
	}

	private void delete_Click(object sender, EventArgs e)
	{
		SelectedText = string.Empty;
	}

	private void select_all_Click(object sender, EventArgs e)
	{
		SelectAll();
	}

	protected override void OnBackColorChanged(EventArgs e)
	{
		base.OnBackColorChanged(e);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}
}
