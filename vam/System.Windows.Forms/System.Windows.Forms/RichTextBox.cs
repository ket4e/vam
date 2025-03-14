using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.RTF;

namespace System.Windows.Forms;

[Docking(DockingBehavior.Ask)]
[Designer("System.Windows.Forms.Design.RichTextBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class RichTextBox : TextBoxBase
{
	private class RtfSectionStyle : ICloneable
	{
		internal System.Drawing.Color rtf_color;

		internal System.Windows.Forms.RTF.Font rtf_rtffont;

		internal int rtf_rtffont_size;

		internal FontStyle rtf_rtfstyle;

		internal HorizontalAlignment rtf_rtfalign;

		internal int rtf_par_line_left_indent;

		internal bool rtf_visible;

		internal int rtf_skip_width;

		public object Clone()
		{
			RtfSectionStyle rtfSectionStyle = new RtfSectionStyle();
			rtfSectionStyle.rtf_color = rtf_color;
			rtfSectionStyle.rtf_par_line_left_indent = rtf_par_line_left_indent;
			rtfSectionStyle.rtf_rtfalign = rtf_rtfalign;
			rtfSectionStyle.rtf_rtffont = rtf_rtffont;
			rtfSectionStyle.rtf_rtffont_size = rtf_rtffont_size;
			rtfSectionStyle.rtf_rtfstyle = rtf_rtfstyle;
			rtfSectionStyle.rtf_visible = rtf_visible;
			rtfSectionStyle.rtf_skip_width = rtf_skip_width;
			return rtfSectionStyle;
		}
	}

	internal bool auto_word_select;

	internal int bullet_indent;

	internal bool detect_urls;

	private bool reuse_line;

	internal int margin_right;

	internal float zoom;

	private StringBuilder rtf_line;

	private RtfSectionStyle rtf_style;

	private Stack rtf_section_stack;

	private TextMap rtf_text_map;

	private int rtf_skip_count;

	private int rtf_cursor_x;

	private int rtf_cursor_y;

	private int rtf_chars;

	private bool enable_auto_drag_drop;

	private RichTextBoxLanguageOptions language_option;

	private bool rich_text_shortcuts_enabled;

	private System.Drawing.Color selection_back_color;

	private static object ContentsResizedEvent;

	private static object HScrollEvent;

	private static object ImeChangeEvent;

	private static object LinkClickedEvent;

	private static object ProtectedEvent;

	private static object SelectionChangedEvent;

	private static object VScrollEvent;

	private static readonly char[] ReservedRTFChars;

	[Browsable(false)]
	public override bool AllowDrop
	{
		get
		{
			return base.AllowDrop;
		}
		set
		{
			base.AllowDrop = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DefaultValue(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[RefreshProperties(RefreshProperties.Repaint)]
	[Browsable(false)]
	public override bool AutoSize
	{
		get
		{
			return auto_size;
		}
		set
		{
			base.AutoSize = value;
		}
	}

	[System.MonoTODO("Value not respected, always true")]
	[DefaultValue(false)]
	public bool AutoWordSelection
	{
		get
		{
			return auto_word_select;
		}
		set
		{
			auto_word_select = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[DefaultValue(0)]
	[Localizable(true)]
	public int BulletIndent
	{
		get
		{
			return bullet_indent;
		}
		set
		{
			bullet_indent = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CanRedo => document.undo.CanRedo;

	[DefaultValue(true)]
	public bool DetectUrls
	{
		get
		{
			return base.EnableLinks;
		}
		set
		{
			base.EnableLinks = value;
		}
	}

	[DefaultValue(false)]
	[System.MonoTODO("Stub, does nothing")]
	public bool EnableAutoDragDrop
	{
		get
		{
			return enable_auto_drag_drop;
		}
		set
		{
			enable_auto_drag_drop = value;
		}
	}

	public override System.Drawing.Font Font
	{
		get
		{
			return base.Font;
		}
		set
		{
			if (font != value)
			{
				if (auto_size && base.PreferredHeight != base.Height)
				{
					base.Height = base.PreferredHeight;
				}
				base.Font = value;
				Line line = document.GetLine(1);
				Line line2 = document.GetLine(document.Lines);
				document.FormatText(line, 1, line2, line2.text.Length + 1, base.Font, System.Drawing.Color.Empty, System.Drawing.Color.Empty, FormatSpecified.Font);
			}
		}
	}

	public override System.Drawing.Color ForeColor
	{
		get
		{
			return base.ForeColor;
		}
		set
		{
			base.ForeColor = value;
		}
	}

	[System.MonoTODO("Stub, does nothing")]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public RichTextBoxLanguageOptions LanguageOption
	{
		get
		{
			return language_option;
		}
		set
		{
			language_option = value;
		}
	}

	[DefaultValue(int.MaxValue)]
	public override int MaxLength
	{
		get
		{
			return base.MaxLength;
		}
		set
		{
			base.MaxLength = value;
		}
	}

	[DefaultValue(true)]
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

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string RedoActionName => document.undo.RedoActionName;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[System.MonoTODO("Stub, does nothing")]
	[Browsable(false)]
	[DefaultValue(true)]
	public bool RichTextShortcutsEnabled
	{
		get
		{
			return rich_text_shortcuts_enabled;
		}
		set
		{
			rich_text_shortcuts_enabled = value;
		}
	}

	[System.MonoInternalNote("Teach TextControl.RecalculateLine to consider the right margin as well")]
	[System.MonoTODO("Stub, does nothing")]
	[Localizable(true)]
	[DefaultValue(0)]
	public int RightMargin
	{
		get
		{
			return margin_right;
		}
		set
		{
			margin_right = value;
		}
	}

	[Browsable(false)]
	[RefreshProperties(RefreshProperties.All)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string Rtf
	{
		get
		{
			Line line = document.GetLine(1);
			Line line2 = document.GetLine(document.Lines);
			return GenerateRTF(line, 0, line2, line2.text.Length).ToString();
		}
		set
		{
			document.Empty();
			MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(value), writable: false);
			InsertRTFFromStream(memoryStream, 0, 1);
			memoryStream.Close();
			Invalidate();
		}
	}

	[Localizable(true)]
	[DefaultValue(RichTextBoxScrollBars.Both)]
	public RichTextBoxScrollBars ScrollBars
	{
		get
		{
			return scrollbars;
		}
		set
		{
			if (!Enum.IsDefined(typeof(RichTextBoxScrollBars), value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(RichTextBoxScrollBars));
			}
			if (value != scrollbars)
			{
				scrollbars = value;
				CalculateDocument();
			}
		}
	}

	[Browsable(false)]
	[DefaultValue("")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string SelectedRtf
	{
		get
		{
			return GenerateRTF(document.selection_start.line, document.selection_start.pos, document.selection_end.line, document.selection_end.pos).ToString();
		}
		set
		{
			if (document.selection_visible)
			{
				document.ReplaceSelection(string.Empty, select_new: false);
			}
			int pos = document.LineTagToCharIndex(document.selection_start.line, document.selection_start.pos);
			MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(value), writable: false);
			int pos2 = document.selection_start.pos;
			int line_no = document.selection_start.line.line_no;
			if (pos2 == 0)
			{
				reuse_line = true;
			}
			InsertRTFFromStream(memoryStream, pos2, line_no, out var _, out var to_y, out var chars);
			memoryStream.Close();
			int num = document.LineEndingLength((!XplatUI.RunningOnUnix) ? LineEnding.Hard : LineEnding.Rich);
			document.CharIndexToLineTag(pos + chars + (to_y - document.selection_start.line.line_no) * num, out var line_out, out var _, out pos);
			if (pos >= line_out.text.Length)
			{
				pos = line_out.text.Length - 1;
			}
			document.SetSelection(line_out, pos);
			document.PositionCaret(line_out, pos);
			document.DisplayCaret();
			ScrollToCaret();
			OnTextChanged(EventArgs.Empty);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DefaultValue("")]
	public override string SelectedText
	{
		get
		{
			return base.SelectedText;
		}
		set
		{
			base.Modified = true;
			base.SelectedText = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DefaultValue(HorizontalAlignment.Left)]
	public HorizontalAlignment SelectionAlignment
	{
		get
		{
			Line line = document.ParagraphStart(document.selection_start.line);
			HorizontalAlignment horizontalAlignment = line.alignment;
			Line line2 = document.ParagraphEnd(document.selection_end.line);
			Line line3 = line;
			while (true)
			{
				if (line3.alignment != horizontalAlignment)
				{
					return HorizontalAlignment.Left;
				}
				if (line3 == line2)
				{
					break;
				}
				line3 = document.GetLine(line3.line_no + 1);
			}
			return horizontalAlignment;
		}
		set
		{
			Line line = document.ParagraphStart(document.selection_start.line);
			Line line2 = document.ParagraphEnd(document.selection_end.line);
			Line line3 = line;
			while (true)
			{
				line3.alignment = value;
				if (line3 == line2)
				{
					break;
				}
				line3 = document.GetLine(line3.line_no + 1);
			}
			CalculateDocument();
		}
	}

	[System.MonoTODO("Stub, does nothing")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public System.Drawing.Color SelectionBackColor
	{
		get
		{
			return selection_back_color;
		}
		set
		{
			selection_back_color = value;
		}
	}

	[DefaultValue(false)]
	[Browsable(false)]
	[System.MonoTODO("Stub, does nothing")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool SelectionBullet
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[DefaultValue(0)]
	[Browsable(false)]
	[System.MonoTODO("Stub, does nothing")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int SelectionCharOffset
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public System.Drawing.Color SelectionColor
	{
		get
		{
			LineTag lineTag;
			LineTag lineTag2;
			if (selection_length > 0)
			{
				lineTag = document.selection_start.line.FindTag(document.selection_start.pos + 1);
				lineTag2 = document.selection_start.line.FindTag(document.selection_end.pos);
			}
			else
			{
				lineTag = document.selection_start.line.FindTag(document.selection_start.pos);
				lineTag2 = lineTag;
			}
			System.Drawing.Color color = lineTag.Color;
			for (LineTag lineTag3 = lineTag; lineTag3 != null; lineTag3 = document.NextTag(lineTag3))
			{
				if (!color.Equals(lineTag3.Color))
				{
					return System.Drawing.Color.Empty;
				}
				if (lineTag3 == lineTag2)
				{
					break;
				}
			}
			return color;
		}
		set
		{
			if (value == System.Drawing.Color.Empty)
			{
				value = Control.DefaultForeColor;
			}
			int index = document.LineTagToCharIndex(document.selection_start.line, document.selection_start.pos);
			int index2 = document.LineTagToCharIndex(document.selection_end.line, document.selection_end.pos);
			document.FormatText(document.selection_start.line, document.selection_start.pos + 1, document.selection_end.line, document.selection_end.pos + 1, null, value, System.Drawing.Color.Empty, FormatSpecified.Color);
			document.CharIndexToLineTag(index, out document.selection_start.line, out document.selection_start.tag, out document.selection_start.pos);
			document.CharIndexToLineTag(index2, out document.selection_end.line, out document.selection_end.tag, out document.selection_end.pos);
			document.UpdateView(document.selection_start.line, 0);
			document.AlignCaret(changeCaretTag: false);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public System.Drawing.Font SelectionFont
	{
		get
		{
			LineTag lineTag;
			LineTag lineTag2;
			if (selection_length > 0)
			{
				lineTag = document.selection_start.line.FindTag(document.selection_start.pos + 1);
				lineTag2 = document.selection_start.line.FindTag(document.selection_end.pos);
			}
			else
			{
				lineTag = document.selection_start.line.FindTag(document.selection_start.pos);
				lineTag2 = lineTag;
			}
			System.Drawing.Font font = lineTag.Font;
			if (selection_length > 1)
			{
				for (LineTag lineTag3 = lineTag; lineTag3 != null; lineTag3 = document.NextTag(lineTag3))
				{
					if (!font.Equals(lineTag3.Font))
					{
						return null;
					}
					if (lineTag3 == lineTag2)
					{
						break;
					}
				}
			}
			return font;
		}
		set
		{
			int index = document.LineTagToCharIndex(document.selection_start.line, document.selection_start.pos);
			int index2 = document.LineTagToCharIndex(document.selection_end.line, document.selection_end.pos);
			document.FormatText(document.selection_start.line, document.selection_start.pos + 1, document.selection_end.line, document.selection_end.pos + 1, value, System.Drawing.Color.Empty, System.Drawing.Color.Empty, FormatSpecified.Font);
			document.CharIndexToLineTag(index, out document.selection_start.line, out document.selection_start.tag, out document.selection_start.pos);
			document.CharIndexToLineTag(index2, out document.selection_end.line, out document.selection_end.tag, out document.selection_end.pos);
			document.UpdateView(document.selection_start.line, 0);
			base.Document.AlignCaret(changeCaretTag: false);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[System.MonoTODO("Stub, does nothing")]
	[DefaultValue(0)]
	public int SelectionHangingIndent
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	[System.MonoTODO("Stub, does nothing")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DefaultValue(0)]
	[Browsable(false)]
	public int SelectionIndent
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override int SelectionLength
	{
		get
		{
			return base.SelectionLength;
		}
		set
		{
			base.SelectionLength = value;
		}
	}

	[DefaultValue(false)]
	[System.MonoTODO("Stub, does nothing")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool SelectionProtected
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[System.MonoTODO("Stub, does nothing")]
	[Browsable(false)]
	[DefaultValue(0)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int SelectionRightIndent
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	[Browsable(false)]
	[System.MonoTODO("Stub, does nothing")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int[] SelectionTabs
	{
		get
		{
			return new int[0];
		}
		set
		{
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public RichTextBoxSelectionTypes SelectionType
	{
		get
		{
			if (document.selection_start == document.selection_end)
			{
				return RichTextBoxSelectionTypes.Empty;
			}
			if (SelectedText.Length > 1)
			{
				return RichTextBoxSelectionTypes.Text | RichTextBoxSelectionTypes.MultiChar;
			}
			return RichTextBoxSelectionTypes.Text;
		}
	}

	[DefaultValue(false)]
	[System.MonoTODO("Stub, does nothing")]
	public bool ShowSelectionMargin
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[Localizable(true)]
	[RefreshProperties(RefreshProperties.All)]
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

	[Browsable(false)]
	public override int TextLength => base.TextLength;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string UndoActionName => document.undo.UndoActionName;

	[Localizable(true)]
	[DefaultValue(1)]
	public float ZoomFactor
	{
		get
		{
			return zoom;
		}
		set
		{
			zoom = value;
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Size DefaultSize => new Size(100, 96);

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler BackgroundImageChanged
	{
		add
		{
			base.BackgroundImageChanged += value;
		}
		remove
		{
			base.BackgroundImageChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler BackgroundImageLayoutChanged
	{
		add
		{
			base.BackgroundImageLayoutChanged += value;
		}
		remove
		{
			base.BackgroundImageLayoutChanged -= value;
		}
	}

	public event ContentsResizedEventHandler ContentsResized
	{
		add
		{
			base.Events.AddHandler(ContentsResizedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ContentsResizedEvent, value);
		}
	}

	[Browsable(false)]
	public new event DragEventHandler DragDrop
	{
		add
		{
			base.DragDrop += value;
		}
		remove
		{
			base.DragDrop -= value;
		}
	}

	[Browsable(false)]
	public new event DragEventHandler DragEnter
	{
		add
		{
			base.DragEnter += value;
		}
		remove
		{
			base.DragEnter -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler DragLeave
	{
		add
		{
			base.DragLeave += value;
		}
		remove
		{
			base.DragLeave -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event DragEventHandler DragOver
	{
		add
		{
			base.DragOver += value;
		}
		remove
		{
			base.DragOver -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event GiveFeedbackEventHandler GiveFeedback
	{
		add
		{
			base.GiveFeedback += value;
		}
		remove
		{
			base.GiveFeedback -= value;
		}
	}

	public event EventHandler HScroll
	{
		add
		{
			base.Events.AddHandler(HScrollEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(HScrollEvent, value);
		}
	}

	public event EventHandler ImeChange
	{
		add
		{
			base.Events.AddHandler(ImeChangeEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ImeChangeEvent, value);
		}
	}

	public event LinkClickedEventHandler LinkClicked
	{
		add
		{
			base.Events.AddHandler(LinkClickedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(LinkClickedEvent, value);
		}
	}

	public event EventHandler Protected
	{
		add
		{
			base.Events.AddHandler(ProtectedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ProtectedEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event QueryContinueDragEventHandler QueryContinueDrag
	{
		add
		{
			base.QueryContinueDrag += value;
		}
		remove
		{
			base.QueryContinueDrag -= value;
		}
	}

	[System.MonoTODO("Event never raised")]
	public event EventHandler SelectionChanged
	{
		add
		{
			base.Events.AddHandler(SelectionChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SelectionChangedEvent, value);
		}
	}

	public event EventHandler VScroll
	{
		add
		{
			base.Events.AddHandler(VScrollEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(VScrollEvent, value);
		}
	}

	public RichTextBox()
	{
		accepts_return = true;
		auto_size = false;
		auto_word_select = false;
		bullet_indent = 0;
		base.MaxLength = int.MaxValue;
		margin_right = 0;
		zoom = 1f;
		base.Multiline = true;
		document.CRLFSize = 1;
		shortcuts_enabled = true;
		base.EnableLinks = true;
		richtext = true;
		rtf_style = new RtfSectionStyle();
		rtf_section_stack = null;
		scrollbars = RichTextBoxScrollBars.Both;
		alignment = HorizontalAlignment.Left;
		base.LostFocus += RichTextBox_LostFocus;
		base.GotFocus += RichTextBox_GotFocus;
		BackColor = ThemeEngine.Current.ColorWindow;
		backcolor_set = false;
		language_option = RichTextBoxLanguageOptions.AutoFontSizeAdjust;
		rich_text_shortcuts_enabled = true;
		selection_back_color = Control.DefaultBackColor;
		ForeColor = ThemeEngine.Current.ColorWindowText;
		base.HScrolled += RichTextBox_HScrolled;
		base.VScrolled += RichTextBox_VScrolled;
		SetStyle(ControlStyles.StandardDoubleClick, value: false);
	}

	static RichTextBox()
	{
		ContentsResized = new object();
		HScroll = new object();
		ImeChange = new object();
		LinkClicked = new object();
		Protected = new object();
		SelectionChanged = new object();
		VScroll = new object();
		ReservedRTFChars = new char[3] { '\\', '{', '}' };
	}

	internal override void HandleLinkClicked(LinkRectangle link)
	{
		OnLinkClicked(new LinkClickedEventArgs(link.LinkTag.LinkText));
	}

	internal override System.Drawing.Color ChangeBackColor(System.Drawing.Color backColor)
	{
		if (backColor == System.Drawing.Color.Empty)
		{
			backcolor_set = false;
			if (!base.ReadOnly)
			{
				backColor = SystemColors.Window;
			}
		}
		return backColor;
	}

	internal override void RaiseSelectionChanged()
	{
		OnSelectionChanged(EventArgs.Empty);
	}

	private void RichTextBox_LostFocus(object sender, EventArgs e)
	{
		Invalidate();
	}

	private void RichTextBox_GotFocus(object sender, EventArgs e)
	{
		Invalidate();
	}

	public bool CanPaste(DataFormats.Format clipFormat)
	{
		if (clipFormat.Name == DataFormats.Rtf || clipFormat.Name == DataFormats.Text || clipFormat.Name == DataFormats.UnicodeText)
		{
			return true;
		}
		return false;
	}

	public int Find(char[] characterSet)
	{
		return Find(characterSet, -1, -1);
	}

	public int Find(char[] characterSet, int start)
	{
		return Find(characterSet, start, -1);
	}

	public int Find(char[] characterSet, int start, int end)
	{
		Document.Marker mark;
		if (start == -1)
		{
			document.GetMarker(out mark, start: true);
		}
		else
		{
			mark = default(Document.Marker);
			document.CharIndexToLineTag(start, out var line_out, out var tag_out, out var pos);
			mark.line = line_out;
			mark.tag = tag_out;
			mark.pos = pos;
		}
		Document.Marker mark2;
		if (end == -1)
		{
			document.GetMarker(out mark2, start: false);
		}
		else
		{
			mark2 = default(Document.Marker);
			document.CharIndexToLineTag(end, out var line_out2, out var tag_out2, out var pos2);
			mark2.line = line_out2;
			mark2.tag = tag_out2;
			mark2.pos = pos2;
		}
		if (document.FindChars(characterSet, mark, mark2, out var result))
		{
			return document.LineTagToCharIndex(result.line, result.pos);
		}
		return -1;
	}

	public int Find(string str)
	{
		return Find(str, -1, -1, RichTextBoxFinds.None);
	}

	public int Find(string str, int start, int end, RichTextBoxFinds options)
	{
		Document.Marker mark;
		if (start == -1)
		{
			document.GetMarker(out mark, start: true);
		}
		else
		{
			mark = default(Document.Marker);
			document.CharIndexToLineTag(start, out var line_out, out var tag_out, out var pos);
			mark.line = line_out;
			mark.tag = tag_out;
			mark.pos = pos;
		}
		Document.Marker mark2;
		if (end == -1)
		{
			document.GetMarker(out mark2, start: false);
		}
		else
		{
			mark2 = default(Document.Marker);
			document.CharIndexToLineTag(end, out var line_out2, out var tag_out2, out var pos2);
			mark2.line = line_out2;
			mark2.tag = tag_out2;
			mark2.pos = pos2;
		}
		if (document.Find(str, mark, mark2, out var result, options))
		{
			return document.LineTagToCharIndex(result.line, result.pos);
		}
		return -1;
	}

	public int Find(string str, int start, RichTextBoxFinds options)
	{
		return Find(str, start, -1, options);
	}

	public int Find(string str, RichTextBoxFinds options)
	{
		return Find(str, -1, -1, options);
	}

	internal override char GetCharFromPositionInternal(Point p)
	{
		PointToTagPos(p, out var tag, out var pos);
		if (pos >= tag.Line.text.Length)
		{
			return '\n';
		}
		return tag.Line.text[pos];
	}

	public override int GetCharIndexFromPosition(Point pt)
	{
		PointToTagPos(pt, out var tag, out var pos);
		return document.LineTagToCharIndex(tag.Line, pos);
	}

	public override int GetLineFromCharIndex(int index)
	{
		document.CharIndexToLineTag(index, out var line_out, out var _, out var _);
		return line_out.LineNo - 1;
	}

	public override Point GetPositionFromCharIndex(int index)
	{
		document.CharIndexToLineTag(index, out var line_out, out var _, out var pos);
		return new Point(line_out.X + (int)line_out.widths[pos] + document.OffsetX - document.ViewPortX, line_out.Y + document.OffsetY - document.ViewPortY);
	}

	public void LoadFile(Stream data, RichTextBoxStreamType fileType)
	{
		document.Empty();
		if (fileType == RichTextBoxStreamType.PlainText)
		{
			StringBuilder stringBuilder;
			char[] array;
			try
			{
				stringBuilder = new StringBuilder((int)data.Length);
				array = new char[1024];
			}
			catch
			{
				throw new IOException("Not enough memory to load document");
			}
			StreamReader streamReader = new StreamReader(data, Encoding.Default, detectEncodingFromByteOrderMarks: true);
			for (int num = streamReader.Read(array, 0, array.Length); num > 0; num = streamReader.Read(array, 0, array.Length))
			{
				stringBuilder.Append(array, 0, num);
			}
			if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '\n')
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			base.Text = stringBuilder.ToString();
		}
		else
		{
			InsertRTFFromStream(data, 0, 1);
			document.PositionCaret(document.GetLine(1), 0);
			document.SetSelectionToCaret(start: true);
			ScrollToCaret();
		}
	}

	public void LoadFile(string path)
	{
		LoadFile(path, RichTextBoxStreamType.RichText);
	}

	public void LoadFile(string path, RichTextBoxStreamType fileType)
	{
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024);
			LoadFile(fileStream, fileType);
		}
		catch (Exception innerException)
		{
			throw new IOException("Could not open file " + path, innerException);
		}
		finally
		{
			fileStream?.Close();
		}
	}

	public void Paste(DataFormats.Format clipFormat)
	{
		Paste(Clipboard.GetDataObject(), clipFormat, obey_length: false);
	}

	public void Redo()
	{
		if (document.undo.Redo())
		{
			OnTextChanged(EventArgs.Empty);
		}
	}

	public void SaveFile(Stream data, RichTextBoxStreamType fileType)
	{
		Encoding encoding = ((fileType != RichTextBoxStreamType.UnicodePlainText) ? Encoding.ASCII : Encoding.Unicode);
		byte[] bytes;
		switch (fileType)
		{
		case RichTextBoxStreamType.PlainText:
		case RichTextBoxStreamType.TextTextOleObjs:
		case RichTextBoxStreamType.UnicodePlainText:
		{
			if (!Multiline)
			{
				bytes = encoding.GetBytes(document.Root.text.ToString());
				data.Write(bytes, 0, bytes.Length);
				return;
			}
			for (int i = 1; i < document.Lines; i++)
			{
				string s = document.GetLine(i).TextWithoutEnding() + Environment.NewLine;
				bytes = encoding.GetBytes(s);
				data.Write(bytes, 0, bytes.Length);
			}
			bytes = encoding.GetBytes(document.GetLine(document.Lines).text.ToString());
			data.Write(bytes, 0, bytes.Length);
			return;
		}
		}
		Line line = document.GetLine(1);
		Line line2 = document.GetLine(document.Lines);
		StringBuilder stringBuilder = GenerateRTF(line, 0, line2, line2.text.Length);
		int length = stringBuilder.Length;
		bytes = new byte[4096];
		for (int i = 0; i < length; i += 1024)
		{
			int bytes2;
			if (i + 1024 < length)
			{
				bytes2 = encoding.GetBytes(stringBuilder.ToString(i, 1024), 0, 1024, bytes, 0);
			}
			else
			{
				bytes2 = length - i;
				bytes2 = encoding.GetBytes(stringBuilder.ToString(i, bytes2), 0, bytes2, bytes, 0);
			}
			data.Write(bytes, 0, bytes2);
		}
	}

	public void SaveFile(string path)
	{
		if (path.EndsWith(".rtf"))
		{
			SaveFile(path, RichTextBoxStreamType.RichText);
		}
		else
		{
			SaveFile(path, RichTextBoxStreamType.PlainText);
		}
	}

	public void SaveFile(string path, RichTextBoxStreamType fileType)
	{
		FileStream fileStream = null;
		fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024, useAsync: false);
		SaveFile(fileStream, fileType);
		fileStream?.Close();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
	{
		Graphics g = Graphics.FromImage(bitmap);
		Draw(g, targetBounds);
	}

	protected virtual object CreateRichEditOleCallback()
	{
		throw new NotImplementedException();
	}

	protected override void OnBackColorChanged(EventArgs e)
	{
		base.OnBackColorChanged(e);
	}

	protected virtual void OnContentsResized(ContentsResizedEventArgs e)
	{
		((ContentsResizedEventHandler)base.Events[ContentsResized])?.Invoke(this, e);
	}

	protected override void OnContextMenuChanged(EventArgs e)
	{
		base.OnContextMenuChanged(e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected virtual void OnHScroll(EventArgs e)
	{
		((EventHandler)base.Events[HScroll])?.Invoke(this, e);
	}

	[System.MonoTODO("Stub, never called")]
	protected virtual void OnImeChange(EventArgs e)
	{
		((EventHandler)base.Events[ImeChange])?.Invoke(this, e);
	}

	protected virtual void OnLinkClicked(LinkClickedEventArgs e)
	{
		((LinkClickedEventHandler)base.Events[LinkClicked])?.Invoke(this, e);
	}

	protected virtual void OnProtected(EventArgs e)
	{
		((EventHandler)base.Events[Protected])?.Invoke(this, e);
	}

	protected override void OnRightToLeftChanged(EventArgs e)
	{
		base.OnRightToLeftChanged(e);
	}

	protected virtual void OnSelectionChanged(EventArgs e)
	{
		((EventHandler)base.Events[SelectionChanged])?.Invoke(this, e);
	}

	protected virtual void OnVScroll(EventArgs e)
	{
		((EventHandler)base.Events[VScroll])?.Invoke(this, e);
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	protected override bool ProcessCmdKey(ref Message m, Keys keyData)
	{
		return base.ProcessCmdKey(ref m, keyData);
	}

	internal override void SelectWord()
	{
		document.ExpandSelection(CaretSelection.Word, to_caret: false);
	}

	private void HandleGroup(System.Windows.Forms.RTF.RTF rtf)
	{
		if (rtf_section_stack == null)
		{
			rtf_section_stack = new Stack();
		}
		if (rtf.Major == Major.BeginGroup)
		{
			rtf_section_stack.Push(rtf_style.Clone());
			rtf_skip_count = 0;
		}
		else if (rtf.Major == Major.EndGroup && rtf_section_stack.Count > 0)
		{
			FlushText(rtf, newline: false);
			rtf_style = (RtfSectionStyle)rtf_section_stack.Pop();
		}
	}

	[System.MonoInternalNote("Add QuadJust support for justified alignment")]
	private void HandleControl(System.Windows.Forms.RTF.RTF rtf)
	{
		switch (rtf.Major)
		{
		case Major.Unicode:
			switch (rtf.Minor)
			{
			case Minor.UnicodeCharBytes:
				rtf_style.rtf_skip_width = rtf.Param;
				break;
			case Minor.UnicodeChar:
				FlushText(rtf, newline: false);
				rtf_skip_count += rtf_style.rtf_skip_width;
				rtf_line.Append((char)rtf.Param);
				break;
			}
			break;
		case Major.Destination:
			rtf.SkipGroup();
			break;
		case Major.PictAttr:
			if (rtf.Picture != null && rtf.Picture.IsValid())
			{
				Line line = document.GetLine(rtf_cursor_y);
				document.InsertPicture(line, 0, rtf.Picture);
				rtf_cursor_x++;
				FlushText(rtf, newline: true);
				rtf.Picture = null;
			}
			break;
		case Major.CharAttr:
			switch (rtf.Minor)
			{
			case Minor.ForeColor:
			{
				System.Windows.Forms.RTF.Color color = System.Windows.Forms.RTF.Color.GetColor(rtf, rtf.Param);
				if (color != null)
				{
					FlushText(rtf, newline: false);
					if (color.Red == -1 && color.Green == -1 && color.Blue == -1)
					{
						rtf_style.rtf_color = ForeColor;
					}
					else
					{
						rtf_style.rtf_color = System.Drawing.Color.FromArgb(color.Red, color.Green, color.Blue);
					}
					FlushText(rtf, newline: false);
				}
				break;
			}
			case Minor.FontSize:
				FlushText(rtf, newline: false);
				rtf_style.rtf_rtffont_size = rtf.Param / 2;
				break;
			case Minor.FontNum:
			{
				System.Windows.Forms.RTF.Font font = System.Windows.Forms.RTF.Font.GetFont(rtf, rtf.Param);
				if (font != null)
				{
					FlushText(rtf, newline: false);
					rtf_style.rtf_rtffont = font;
				}
				break;
			}
			case Minor.Plain:
				FlushText(rtf, newline: false);
				rtf_style.rtf_rtfstyle = FontStyle.Regular;
				break;
			case Minor.Bold:
				FlushText(rtf, newline: false);
				if (rtf.Param == -1000000)
				{
					rtf_style.rtf_rtfstyle |= FontStyle.Bold;
				}
				else
				{
					rtf_style.rtf_rtfstyle &= ~FontStyle.Bold;
				}
				break;
			case Minor.Italic:
				FlushText(rtf, newline: false);
				if (rtf.Param == -1000000)
				{
					rtf_style.rtf_rtfstyle |= FontStyle.Italic;
				}
				else
				{
					rtf_style.rtf_rtfstyle &= ~FontStyle.Italic;
				}
				break;
			case Minor.StrikeThru:
				FlushText(rtf, newline: false);
				if (rtf.Param == -1000000)
				{
					rtf_style.rtf_rtfstyle |= FontStyle.Strikeout;
				}
				else
				{
					rtf_style.rtf_rtfstyle &= ~FontStyle.Strikeout;
				}
				break;
			case Minor.Underline:
				FlushText(rtf, newline: false);
				if (rtf.Param == -1000000)
				{
					rtf_style.rtf_rtfstyle |= FontStyle.Underline;
				}
				else
				{
					rtf_style.rtf_rtfstyle &= ~FontStyle.Underline;
				}
				break;
			case Minor.Invisible:
				FlushText(rtf, newline: false);
				rtf_style.rtf_visible = false;
				break;
			case Minor.NoUnderline:
				FlushText(rtf, newline: false);
				rtf_style.rtf_rtfstyle &= ~FontStyle.Underline;
				break;
			}
			break;
		case Major.ParAttr:
			switch (rtf.Minor)
			{
			case Minor.ParDef:
				FlushText(rtf, newline: false);
				rtf_style.rtf_par_line_left_indent = 0;
				rtf_style.rtf_rtfalign = HorizontalAlignment.Left;
				break;
			case Minor.LeftIndent:
				rtf_style.rtf_par_line_left_indent = (int)((float)rtf.Param / 1440f * CreateGraphics().DpiX + 0.5f);
				break;
			case Minor.QuadCenter:
				FlushText(rtf, newline: false);
				rtf_style.rtf_rtfalign = HorizontalAlignment.Center;
				break;
			case Minor.QuadJust:
				FlushText(rtf, newline: false);
				rtf_style.rtf_rtfalign = HorizontalAlignment.Center;
				break;
			case Minor.QuadLeft:
				FlushText(rtf, newline: false);
				rtf_style.rtf_rtfalign = HorizontalAlignment.Left;
				break;
			case Minor.QuadRight:
				FlushText(rtf, newline: false);
				rtf_style.rtf_rtfalign = HorizontalAlignment.Right;
				break;
			}
			break;
		case Major.SpecialChar:
			SpecialChar(rtf);
			break;
		}
	}

	private void SpecialChar(System.Windows.Forms.RTF.RTF rtf)
	{
		switch (rtf.Minor)
		{
		case Minor.Row:
		case Minor.Par:
		case Minor.Sect:
		case Minor.Page:
		case Minor.Line:
			FlushText(rtf, newline: true);
			break;
		case Minor.Cell:
			Console.Write(" ");
			break;
		case Minor.NoBrkSpace:
			Console.Write(" ");
			break;
		case Minor.Tab:
			rtf_line.Append("\t");
			break;
		case Minor.NoReqHyphen:
		case Minor.NoBrkHyphen:
			rtf_line.Append("-");
			break;
		case Minor.Bullet:
			Console.WriteLine("*");
			break;
		case Minor.EmDash:
			rtf_line.Append("—");
			break;
		case Minor.EnDash:
			rtf_line.Append("–");
			break;
		}
	}

	private void HandleText(System.Windows.Forms.RTF.RTF rtf)
	{
		string text = rtf.EncodedText;
		if (rtf_skip_count > 0 && text.Length > 0)
		{
			int num = Math.Min(rtf_skip_count, text.Length);
			text = text.Substring(num);
			rtf_skip_count -= num;
		}
		if (rtf_style.rtf_visible)
		{
			rtf_line.Append(text);
		}
	}

	private void FlushText(System.Windows.Forms.RTF.RTF rtf, bool newline)
	{
		int length = rtf_line.Length;
		if (!newline && length == 0)
		{
			return;
		}
		if (rtf_style.rtf_rtffont == null)
		{
			rtf_style.rtf_rtffont = System.Windows.Forms.RTF.Font.GetFont(rtf, 0);
		}
		System.Drawing.Font font = new System.Drawing.Font(rtf_style.rtf_rtffont.Name, rtf_style.rtf_rtffont_size, rtf_style.rtf_rtfstyle);
		if (rtf_style.rtf_color == System.Drawing.Color.Empty)
		{
			System.Windows.Forms.RTF.Color color = System.Windows.Forms.RTF.Color.GetColor(rtf, 0);
			if (color == null || (color.Red == -1 && color.Green == -1 && color.Blue == -1))
			{
				rtf_style.rtf_color = ForeColor;
			}
			else
			{
				rtf_style.rtf_color = System.Drawing.Color.FromArgb(color.Red, color.Green, color.Blue);
			}
		}
		rtf_chars += rtf_line.Length;
		if (rtf_cursor_x == 0 && !reuse_line)
		{
			if (newline && !rtf_line.ToString().EndsWith(Environment.NewLine))
			{
				rtf_line.Append(Environment.NewLine);
			}
			document.Add(rtf_cursor_y, rtf_line.ToString(), rtf_style.rtf_rtfalign, font, rtf_style.rtf_color, (!newline) ? LineEnding.Wrap : LineEnding.Rich);
			if (rtf_style.rtf_par_line_left_indent != 0)
			{
				Line line = document.GetLine(rtf_cursor_y);
				line.indent = rtf_style.rtf_par_line_left_indent;
			}
		}
		else
		{
			Line line2 = document.GetLine(rtf_cursor_y);
			line2.indent = rtf_style.rtf_par_line_left_indent;
			if (rtf_line.Length > 0)
			{
				document.InsertString(line2, rtf_cursor_x, rtf_line.ToString());
				document.FormatText(line2, rtf_cursor_x + 1, line2, rtf_cursor_x + 1 + length, font, rtf_style.rtf_color, System.Drawing.Color.Empty, FormatSpecified.Font | FormatSpecified.Color);
			}
			if (newline)
			{
				line2 = document.GetLine(rtf_cursor_y);
				line2.ending = LineEnding.Rich;
				if (!line2.Text.EndsWith(Environment.NewLine))
				{
					line2.Text += Environment.NewLine;
				}
			}
			reuse_line = false;
		}
		if (newline)
		{
			rtf_cursor_x = 0;
			rtf_cursor_y++;
		}
		else
		{
			rtf_cursor_x += length;
		}
		rtf_line.Length = 0;
	}

	private void InsertRTFFromStream(Stream data, int cursor_x, int cursor_y)
	{
		InsertRTFFromStream(data, cursor_x, cursor_y, out var _, out var _, out var _);
	}

	private void InsertRTFFromStream(Stream data, int cursor_x, int cursor_y, out int to_x, out int to_y, out int chars)
	{
		System.Windows.Forms.RTF.RTF rTF = new System.Windows.Forms.RTF.RTF(data);
		rTF.ClassCallback[TokenClass.Text] = HandleText;
		rTF.ClassCallback[TokenClass.Control] = HandleControl;
		rTF.ClassCallback[TokenClass.Group] = HandleGroup;
		rtf_skip_count = 0;
		rtf_line = new StringBuilder();
		rtf_style.rtf_color = System.Drawing.Color.Empty;
		rtf_style.rtf_rtffont_size = (int)Font.Size;
		rtf_style.rtf_rtfalign = HorizontalAlignment.Left;
		rtf_style.rtf_rtfstyle = FontStyle.Regular;
		rtf_style.rtf_rtffont = null;
		rtf_style.rtf_visible = true;
		rtf_style.rtf_skip_width = 1;
		rtf_cursor_x = cursor_x;
		rtf_cursor_y = cursor_y;
		rtf_chars = 0;
		rTF.DefaultFont(Font.Name);
		rtf_text_map = new TextMap();
		TextMap.SetupStandardTable(rtf_text_map.Table);
		document.SuspendRecalc();
		try
		{
			rTF.Read();
			FlushText(rTF, newline: false);
		}
		catch (RTFException ex)
		{
			Console.WriteLine("RTF Parsing failure: {0}", ex.Message);
		}
		to_x = rtf_cursor_x;
		to_y = rtf_cursor_y;
		chars = rtf_chars;
		if (rtf_section_stack != null)
		{
			rtf_section_stack.Clear();
		}
		document.RecalculateDocument(CreateGraphicsInternal(), cursor_y, document.Lines, optimize: false);
		document.ResumeRecalc(immediate_update: true);
		document.Invalidate(document.GetLine(cursor_y), 0, document.GetLine(document.Lines), -1);
	}

	private void RichTextBox_HScrolled(object sender, EventArgs e)
	{
		OnHScroll(e);
	}

	private void RichTextBox_VScrolled(object sender, EventArgs e)
	{
		OnVScroll(e);
	}

	private void PointToTagPos(Point pt, out LineTag tag, out int pos)
	{
		Point point = pt;
		if (point.X >= document.ViewPortWidth)
		{
			point.X = document.ViewPortWidth - 1;
		}
		else if (point.X < 0)
		{
			point.X = 0;
		}
		if (point.Y >= document.ViewPortHeight)
		{
			point.Y = document.ViewPortHeight - 1;
		}
		else if (point.Y < 0)
		{
			point.Y = 0;
		}
		tag = document.FindCursor(point.X + document.ViewPortX, point.Y + document.ViewPortY, out pos);
	}

	private void EmitRTFFontProperties(StringBuilder rtf, int prev_index, int font_index, System.Drawing.Font prev_font, System.Drawing.Font font)
	{
		if (prev_index != font_index)
		{
			rtf.Append($"\\f{font_index}");
		}
		if (prev_font == null || prev_font.Size != font.Size)
		{
			rtf.Append($"\\fs{(int)(font.Size * 2f)}");
		}
		if (prev_font == null || font.Bold != prev_font.Bold)
		{
			if (font.Bold)
			{
				rtf.Append("\\b");
			}
			else if (prev_font != null)
			{
				rtf.Append("\\b0");
			}
		}
		if (prev_font == null || font.Italic != prev_font.Italic)
		{
			if (font.Italic)
			{
				rtf.Append("\\i");
			}
			else if (prev_font != null)
			{
				rtf.Append("\\i0");
			}
		}
		if (prev_font == null || font.Strikeout != prev_font.Strikeout)
		{
			if (font.Strikeout)
			{
				rtf.Append("\\strike");
			}
			else if (prev_font != null)
			{
				rtf.Append("\\strike0");
			}
		}
		if (prev_font == null || font.Underline != prev_font.Underline)
		{
			if (font.Underline)
			{
				rtf.Append("\\ul");
			}
			else if (prev_font != null)
			{
				rtf.Append("\\ul0");
			}
		}
	}

	[System.MonoInternalNote("Emit unicode and other special characters properly")]
	private void EmitRTFText(StringBuilder rtf, string text)
	{
		int length = rtf.Length;
		int length2 = text.Length;
		rtf.Append(text);
		if (text.IndexOfAny(ReservedRTFChars) > -1)
		{
			rtf.Replace("\\", "\\\\", length, length2);
			rtf.Replace("{", "\\{", length, length2);
			rtf.Replace("}", "\\}", length, length2);
		}
	}

	private StringBuilder GenerateRTF(Line start_line, int start_pos, Line end_line, int end_pos)
	{
		StringBuilder stringBuilder = new StringBuilder();
		ArrayList arrayList = new ArrayList(10);
		ArrayList arrayList2 = new ArrayList(10);
		Line line = start_line;
		int i = start_line.line_no;
		int num = start_pos;
		LineTag lineTag = LineTag.FindTag(start_line, num);
		System.Drawing.Font font = lineTag.Font;
		System.Drawing.Color color = lineTag.Color;
		arrayList.Add(font.Name);
		arrayList2.Add(color);
		for (; i <= end_line.line_no; i++)
		{
			line = document.GetLine(i);
			lineTag = LineTag.FindTag(line, num);
			int num2 = ((i == end_line.line_no) ? end_pos : line.text.Length);
			while (num < num2)
			{
				if (lineTag.Font.Name != font.Name)
				{
					font = lineTag.Font;
					if (!arrayList.Contains(font.Name))
					{
						arrayList.Add(font.Name);
					}
				}
				if (lineTag.Color != color)
				{
					color = lineTag.Color;
					if (!arrayList2.Contains(color))
					{
						arrayList2.Add(color);
					}
				}
				num = lineTag.Start + lineTag.Length - 1;
				lineTag = lineTag.Next;
			}
			num = 0;
		}
		stringBuilder.Append("{\\rtf1\\ansi");
		stringBuilder.Append("\\ansicpg1252");
		stringBuilder.Append($"\\deff{arrayList.IndexOf(Font.Name)}");
		stringBuilder.Append("\\deflang1033" + Environment.NewLine);
		stringBuilder.Append("{\\fonttbl");
		for (int j = 0; j < arrayList.Count; j++)
		{
			stringBuilder.Append($"{{\\f{j}");
			stringBuilder.Append("\\fnil");
			stringBuilder.Append("\\fcharset0 ");
			stringBuilder.Append((string)arrayList[j]);
			stringBuilder.Append(";}");
		}
		stringBuilder.Append("}");
		stringBuilder.Append(Environment.NewLine);
		if (arrayList2.Count > 1 || ((System.Drawing.Color)arrayList2[0]).R != ForeColor.R || ((System.Drawing.Color)arrayList2[0]).G != ForeColor.G || ((System.Drawing.Color)arrayList2[0]).B != ForeColor.B)
		{
			stringBuilder.Append("{\\colortbl ");
			for (int j = 0; j < arrayList2.Count; j++)
			{
				stringBuilder.Append($"\\red{((System.Drawing.Color)arrayList2[j]).R}");
				stringBuilder.Append($"\\green{((System.Drawing.Color)arrayList2[j]).G}");
				stringBuilder.Append($"\\blue{((System.Drawing.Color)arrayList2[j]).B}");
				stringBuilder.Append(";");
			}
			stringBuilder.Append("}");
			stringBuilder.Append(Environment.NewLine);
		}
		stringBuilder.Append("{\\*\\generator Mono RichTextBox;}");
		lineTag = LineTag.FindTag(start_line, start_pos);
		stringBuilder.Append("\\pard");
		EmitRTFFontProperties(stringBuilder, -1, arrayList.IndexOf(lineTag.Font.Name), null, lineTag.Font);
		stringBuilder.Append(" ");
		font = lineTag.Font;
		color = (System.Drawing.Color)arrayList2[0];
		line = start_line;
		i = start_line.line_no;
		num = start_pos;
		for (; i <= end_line.line_no; i++)
		{
			line = document.GetLine(i);
			lineTag = LineTag.FindTag(line, num);
			int num2 = ((i == end_line.line_no) ? end_pos : line.text.Length);
			while (num < num2)
			{
				int length = stringBuilder.Length;
				if (lineTag.Font != font)
				{
					EmitRTFFontProperties(stringBuilder, arrayList.IndexOf(font.Name), arrayList.IndexOf(lineTag.Font.Name), font, lineTag.Font);
					font = lineTag.Font;
				}
				if (lineTag.Color != color)
				{
					color = lineTag.Color;
					stringBuilder.Append($"\\cf{arrayList2.IndexOf(color)}");
				}
				if (length != stringBuilder.Length)
				{
					stringBuilder.Append(" ");
				}
				if (i != end_line.line_no)
				{
					EmitRTFText(stringBuilder, lineTag.Line.text.ToString(num, lineTag.Start + lineTag.Length - num - 1));
				}
				else if (end_pos < lineTag.Start + lineTag.Length - 1)
				{
					EmitRTFText(stringBuilder, lineTag.Line.text.ToString(num, end_pos - num));
				}
				else
				{
					EmitRTFText(stringBuilder, lineTag.Line.text.ToString(num, lineTag.Start + lineTag.Length - num - 1));
				}
				num = lineTag.Start + lineTag.Length - 1;
				lineTag = lineTag.Next;
			}
			if (num >= line.text.Length && line.ending != LineEnding.Wrap)
			{
				stringBuilder.Append("\\par");
				stringBuilder.Append(Environment.NewLine);
			}
			num = 0;
		}
		stringBuilder.Append("}");
		stringBuilder.Append(Environment.NewLine);
		return stringBuilder;
	}
}
