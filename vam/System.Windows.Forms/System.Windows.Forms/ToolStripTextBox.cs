using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
public class ToolStripTextBox : ToolStripControlHost
{
	private class ToolStripTextBoxControl : TextBox
	{
		private BorderStyle border;

		private Timer tooltip_timer;

		private ToolTip tooltip_window;

		private ToolStripItem owner_item;

		internal BorderStyle Border
		{
			set
			{
				border = value;
				Invalidate();
			}
		}

		internal ToolStripItem OwnerItem
		{
			set
			{
				owner_item = value;
			}
		}

		private bool ShowToolTips
		{
			get
			{
				if (base.Parent == null)
				{
					return false;
				}
				return (base.Parent as ToolStrip).ShowItemToolTips;
			}
		}

		private Timer ToolTipTimer
		{
			get
			{
				if (tooltip_timer == null)
				{
					tooltip_timer = new Timer();
					tooltip_timer.Enabled = false;
					tooltip_timer.Interval = 500;
					tooltip_timer.Tick += ToolTipTimer_Tick;
				}
				return tooltip_timer;
			}
		}

		private ToolTip ToolTipWindow
		{
			get
			{
				if (tooltip_window == null)
				{
					tooltip_window = new ToolTip();
				}
				return tooltip_window;
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			Invalidate();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			Invalidate();
			if (ShowToolTips)
			{
				ToolTipTimer.Start();
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			Invalidate();
			ToolTipTimer.Stop();
			ToolTipWindow.Hide(this);
		}

		internal override void OnPaintInternal(PaintEventArgs e)
		{
			base.OnPaintInternal(e);
			if ((!Focused && !base.Entered && border != BorderStyle.FixedSingle) || border == BorderStyle.None)
			{
				return;
			}
			ToolStripRenderer renderer = (base.Parent as ToolStrip).Renderer;
			if (renderer is ToolStripProfessionalRenderer)
			{
				using (Pen pen = new Pen((renderer as ToolStripProfessionalRenderer).ColorTable.ButtonSelectedBorder))
				{
					e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, base.Width - 1, base.Height - 1));
				}
			}
		}

		private void ToolTipTimer_Tick(object o, EventArgs args)
		{
			string toolTip = owner_item.GetToolTip();
			if (!string.IsNullOrEmpty(toolTip))
			{
				ToolTipWindow.Present(this, toolTip);
			}
			ToolTipTimer.Stop();
		}
	}

	private BorderStyle border_style;

	private static object AcceptsTabChangedEvent;

	private static object BorderStyleChangedEvent;

	private static object HideSelectionChangedEvent;

	private static object ModifiedChangedEvent;

	private static object MultilineChangedEvent;

	private static object ReadOnlyChangedEvent;

	private static object TextBoxTextAlignChangedEvent;

	[DefaultValue(false)]
	public bool AcceptsReturn
	{
		get
		{
			return TextBox.AcceptsReturn;
		}
		set
		{
			TextBox.AcceptsReturn = value;
		}
	}

	[DefaultValue(false)]
	public bool AcceptsTab
	{
		get
		{
			return TextBox.AcceptsTab;
		}
		set
		{
			TextBox.AcceptsTab = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[System.MonoTODO("AutoCompletion algorithm is currently not implemented.")]
	[Browsable(true)]
	[Localizable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public AutoCompleteStringCollection AutoCompleteCustomSource
	{
		get
		{
			return TextBox.AutoCompleteCustomSource;
		}
		set
		{
			TextBox.AutoCompleteCustomSource = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[DefaultValue(AutoCompleteMode.None)]
	[Browsable(true)]
	[System.MonoTODO("AutoCompletion algorithm is currently not implemented.")]
	public AutoCompleteMode AutoCompleteMode
	{
		get
		{
			return TextBox.AutoCompleteMode;
		}
		set
		{
			TextBox.AutoCompleteMode = value;
		}
	}

	[DefaultValue(AutoCompleteSource.None)]
	[System.MonoTODO("AutoCompletion algorithm is currently not implemented.")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public AutoCompleteSource AutoCompleteSource
	{
		get
		{
			return TextBox.AutoCompleteSource;
		}
		set
		{
			TextBox.AutoCompleteSource = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override Image BackgroundImage
	{
		get
		{
			return base.BackgroundImage;
		}
		set
		{
			base.BackgroundImage = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override ImageLayout BackgroundImageLayout
	{
		get
		{
			return base.BackgroundImageLayout;
		}
		set
		{
			base.BackgroundImageLayout = value;
		}
	}

	[DispId(-504)]
	[DefaultValue(BorderStyle.Fixed3D)]
	public BorderStyle BorderStyle
	{
		get
		{
			return border_style;
		}
		set
		{
			if (border_style != value)
			{
				border_style = value;
				(base.Control as ToolStripTextBoxControl).Border = value;
				OnBorderStyleChanged(EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CanUndo => TextBox.CanUndo;

	[DefaultValue(CharacterCasing.Normal)]
	public CharacterCasing CharacterCasing
	{
		get
		{
			return TextBox.CharacterCasing;
		}
		set
		{
			TextBox.CharacterCasing = value;
		}
	}

	[DefaultValue(true)]
	public bool HideSelection
	{
		get
		{
			return TextBox.HideSelection;
		}
		set
		{
			TextBox.HideSelection = value;
		}
	}

	[Localizable(true)]
	[Editor("System.Windows.Forms.Design.StringArrayEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string[] Lines
	{
		get
		{
			return TextBox.Lines;
		}
		set
		{
			TextBox.Lines = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(32767)]
	public int MaxLength
	{
		get
		{
			return TextBox.MaxLength;
		}
		set
		{
			TextBox.MaxLength = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool Modified
	{
		get
		{
			return TextBox.Modified;
		}
		set
		{
			TextBox.Modified = value;
		}
	}

	[Browsable(false)]
	[DefaultValue(false)]
	[RefreshProperties(RefreshProperties.All)]
	[Localizable(true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool Multiline
	{
		get
		{
			return TextBox.Multiline;
		}
		set
		{
			TextBox.Multiline = value;
		}
	}

	[DefaultValue(false)]
	public bool ReadOnly
	{
		get
		{
			return TextBox.ReadOnly;
		}
		set
		{
			TextBox.ReadOnly = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string SelectedText
	{
		get
		{
			return TextBox.SelectedText;
		}
		set
		{
			TextBox.SelectedText = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int SelectionLength
	{
		get
		{
			return (TextBox.SelectionLength != -1) ? TextBox.SelectionLength : 0;
		}
		set
		{
			TextBox.SelectionLength = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int SelectionStart
	{
		get
		{
			return TextBox.SelectionStart;
		}
		set
		{
			TextBox.SelectionStart = value;
		}
	}

	[DefaultValue(true)]
	public bool ShortcutsEnabled
	{
		get
		{
			return TextBox.ShortcutsEnabled;
		}
		set
		{
			TextBox.ShortcutsEnabled = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public TextBox TextBox => (TextBox)base.Control;

	[Localizable(true)]
	[DefaultValue(HorizontalAlignment.Left)]
	public HorizontalAlignment TextBoxTextAlign
	{
		get
		{
			return TextBox.TextAlign;
		}
		set
		{
			TextBox.TextAlign = value;
		}
	}

	[Browsable(false)]
	public int TextLength => TextBox.TextLength;

	[DefaultValue(true)]
	[Localizable(true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public bool WordWrap
	{
		get
		{
			return TextBox.WordWrap;
		}
		set
		{
			TextBox.WordWrap = value;
		}
	}

	protected internal override Padding DefaultMargin => new Padding(1, 0, 1, 0);

	protected override Size DefaultSize => new Size(100, 22);

	public event EventHandler AcceptsTabChanged
	{
		add
		{
			base.Events.AddHandler(AcceptsTabChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AcceptsTabChangedEvent, value);
		}
	}

	public event EventHandler BorderStyleChanged
	{
		add
		{
			base.Events.AddHandler(BorderStyleChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BorderStyleChangedEvent, value);
		}
	}

	public event EventHandler HideSelectionChanged
	{
		add
		{
			base.Events.AddHandler(HideSelectionChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(HideSelectionChangedEvent, value);
		}
	}

	public event EventHandler ModifiedChanged
	{
		add
		{
			base.Events.AddHandler(ModifiedChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ModifiedChangedEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public event EventHandler MultilineChanged
	{
		add
		{
			base.Events.AddHandler(MultilineChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MultilineChangedEvent, value);
		}
	}

	public event EventHandler ReadOnlyChanged
	{
		add
		{
			base.Events.AddHandler(ReadOnlyChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ReadOnlyChangedEvent, value);
		}
	}

	public event EventHandler TextBoxTextAlignChanged
	{
		add
		{
			base.Events.AddHandler(TextBoxTextAlignChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(TextBoxTextAlignChangedEvent, value);
		}
	}

	public ToolStripTextBox()
		: base(new ToolStripTextBoxControl())
	{
		ToolStripTextBoxControl toolStripTextBoxControl = TextBox as ToolStripTextBoxControl;
		toolStripTextBoxControl.OwnerItem = this;
		toolStripTextBoxControl.border_style = BorderStyle.None;
		toolStripTextBoxControl.TopMargin = 3;
		toolStripTextBoxControl.Border = BorderStyle.Fixed3D;
		border_style = BorderStyle.Fixed3D;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public ToolStripTextBox(Control c)
		: base(c)
	{
		throw new NotSupportedException("This construtor cannot be used.");
	}

	public ToolStripTextBox(string name)
		: this()
	{
		base.Name = name;
	}

	static ToolStripTextBox()
	{
		AcceptsTabChanged = new object();
		BorderStyleChanged = new object();
		HideSelectionChanged = new object();
		ModifiedChanged = new object();
		MultilineChanged = new object();
		ReadOnlyChanged = new object();
		TextBoxTextAlignChanged = new object();
	}

	public void AppendText(string text)
	{
		TextBox.AppendText(text);
	}

	public void Clear()
	{
		TextBox.Clear();
	}

	public void ClearUndo()
	{
		TextBox.ClearUndo();
	}

	public void Copy()
	{
		TextBox.Copy();
	}

	public void Cut()
	{
		TextBox.Cut();
	}

	public void DeselectAll()
	{
		TextBox.DeselectAll();
	}

	public char GetCharFromPosition(Point pt)
	{
		return TextBox.GetCharFromPosition(pt);
	}

	public int GetCharIndexFromPosition(Point pt)
	{
		return TextBox.GetCharIndexFromPosition(pt);
	}

	public int GetFirstCharIndexFromLine(int lineNumber)
	{
		return TextBox.GetFirstCharIndexFromLine(lineNumber);
	}

	public int GetFirstCharIndexOfCurrentLine()
	{
		return TextBox.GetFirstCharIndexOfCurrentLine();
	}

	public int GetLineFromCharIndex(int index)
	{
		return TextBox.GetLineFromCharIndex(index);
	}

	public Point GetPositionFromCharIndex(int index)
	{
		return TextBox.GetPositionFromCharIndex(index);
	}

	public override Size GetPreferredSize(Size constrainingSize)
	{
		return base.GetPreferredSize(constrainingSize);
	}

	public void Paste()
	{
		TextBox.Paste();
	}

	public void ScrollToCaret()
	{
		TextBox.ScrollToCaret();
	}

	public void Select(int start, int length)
	{
		TextBox.Select(start, length);
	}

	public void SelectAll()
	{
		TextBox.SelectAll();
	}

	public void Undo()
	{
		TextBox.Undo();
	}

	protected virtual void OnAcceptsTabChanged(EventArgs e)
	{
		((EventHandler)base.Events[AcceptsTabChanged])?.Invoke(this, e);
	}

	protected virtual void OnBorderStyleChanged(EventArgs e)
	{
		((EventHandler)base.Events[BorderStyleChanged])?.Invoke(this, e);
	}

	protected virtual void OnHideSelectionChanged(EventArgs e)
	{
		((EventHandler)base.Events[HideSelectionChanged])?.Invoke(this, e);
	}

	protected virtual void OnModifiedChanged(EventArgs e)
	{
		((EventHandler)base.Events[ModifiedChanged])?.Invoke(this, e);
	}

	protected virtual void OnMultilineChanged(EventArgs e)
	{
		((EventHandler)base.Events[MultilineChanged])?.Invoke(this, e);
	}

	protected virtual void OnReadOnlyChanged(EventArgs e)
	{
		((EventHandler)base.Events[ReadOnlyChanged])?.Invoke(this, e);
	}

	protected override void OnSubscribeControlEvents(Control control)
	{
		base.OnSubscribeControlEvents(control);
		TextBox.AcceptsTabChanged += HandleAcceptsTabChanged;
		TextBox.HideSelectionChanged += HandleHideSelectionChanged;
		TextBox.ModifiedChanged += HandleModifiedChanged;
		TextBox.MultilineChanged += HandleMultilineChanged;
		TextBox.ReadOnlyChanged += HandleReadOnlyChanged;
		TextBox.TextAlignChanged += HandleTextAlignChanged;
		TextBox.TextChanged += HandleTextChanged;
	}

	protected override void OnUnsubscribeControlEvents(Control control)
	{
		base.OnUnsubscribeControlEvents(control);
	}

	private void HandleTextAlignChanged(object sender, EventArgs e)
	{
		((EventHandler)base.Events[TextBoxTextAlignChanged])?.Invoke(this, e);
	}

	private void HandleReadOnlyChanged(object sender, EventArgs e)
	{
		OnReadOnlyChanged(e);
	}

	private void HandleMultilineChanged(object sender, EventArgs e)
	{
		OnMultilineChanged(e);
	}

	private void HandleModifiedChanged(object sender, EventArgs e)
	{
		OnModifiedChanged(e);
	}

	private void HandleHideSelectionChanged(object sender, EventArgs e)
	{
		OnHideSelectionChanged(e);
	}

	private void HandleAcceptsTabChanged(object sender, EventArgs e)
	{
		OnAcceptsTabChanged(e);
	}

	private void HandleTextChanged(object sender, EventArgs e)
	{
		OnTextChanged(e);
	}
}
