using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultBindingProperty("Text")]
[Designer("System.Windows.Forms.Design.MaskedTextBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[DefaultEvent("MaskInputRejected")]
[DefaultProperty("Mask")]
[ComVisible(true)]
public class MaskedTextBox : TextBoxBase
{
	private MaskedTextProvider provider;

	private bool beep_on_error;

	private IFormatProvider format_provider;

	private bool hide_prompt_on_leave;

	private InsertKeyMode insert_key_mode;

	private bool insert_key_overwriting;

	private bool reject_input_on_first_failure;

	private HorizontalAlignment text_align;

	private MaskFormat cut_copy_mask_format;

	private bool use_system_password_char;

	private Type validating_type;

	private bool is_empty_mask;

	private bool setting_text;

	private static object AcceptsTabChangedEvent;

	private static object IsOverwriteModeChangedEvent;

	private static object MaskChangedEvent;

	private static object MaskInputRejectedEvent;

	private static object MultilineChangedEvent;

	private static object TextAlignChangedEvent;

	private static object TypeValidationCompletedEvent;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool AcceptsTab
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[DefaultValue(true)]
	public bool AllowPromptAsInput
	{
		get
		{
			return provider.AllowPromptAsInput;
		}
		set
		{
			provider = new MaskedTextProvider(provider.Mask, provider.Culture, value, provider.PromptChar, provider.PasswordChar, provider.AsciiOnly);
			UpdateVisibleText();
		}
	}

	[DefaultValue(false)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public bool AsciiOnly
	{
		get
		{
			return provider.AsciiOnly;
		}
		set
		{
			provider = new MaskedTextProvider(provider.Mask, provider.Culture, provider.AllowPromptAsInput, provider.PromptChar, provider.PasswordChar, value);
			UpdateVisibleText();
		}
	}

	[DefaultValue(false)]
	public bool BeepOnError
	{
		get
		{
			return beep_on_error;
		}
		set
		{
			beep_on_error = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool CanUndo => false;

	protected override CreateParams CreateParams => base.CreateParams;

	[RefreshProperties(RefreshProperties.Repaint)]
	public CultureInfo Culture
	{
		get
		{
			return provider.Culture;
		}
		set
		{
			provider = new MaskedTextProvider(provider.Mask, value, provider.AllowPromptAsInput, provider.PromptChar, provider.PasswordChar, provider.AsciiOnly);
			UpdateVisibleText();
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(MaskFormat.IncludeLiterals)]
	public MaskFormat CutCopyMaskFormat
	{
		get
		{
			return cut_copy_mask_format;
		}
		set
		{
			if (!Enum.IsDefined(typeof(MaskFormat), value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(MaskFormat));
			}
			cut_copy_mask_format = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public IFormatProvider FormatProvider
	{
		get
		{
			return format_provider;
		}
		set
		{
			format_provider = value;
		}
	}

	[DefaultValue(false)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public bool HidePromptOnLeave
	{
		get
		{
			return hide_prompt_on_leave;
		}
		set
		{
			hide_prompt_on_leave = value;
		}
	}

	[DefaultValue(InsertKeyMode.Default)]
	public InsertKeyMode InsertKeyMode
	{
		get
		{
			return insert_key_mode;
		}
		set
		{
			if (!Enum.IsDefined(typeof(InsertKeyMode), value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(InsertKeyMode));
			}
			insert_key_mode = value;
		}
	}

	[Browsable(false)]
	public bool IsOverwriteMode
	{
		get
		{
			if (insert_key_mode == InsertKeyMode.Default)
			{
				return insert_key_overwriting;
			}
			return insert_key_mode == InsertKeyMode.Overwrite;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new string[] Lines
	{
		get
		{
			string text = Text;
			if (text == null || text == string.Empty)
			{
				return new string[0];
			}
			return Text.Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		}
		set
		{
		}
	}

	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
	[Editor("System.Windows.Forms.Design.MaskPropertyEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DefaultValue("")]
	[MergableProperty(false)]
	public string Mask
	{
		get
		{
			if (is_empty_mask)
			{
				return string.Empty;
			}
			return provider.Mask;
		}
		set
		{
			is_empty_mask = value == string.Empty || value == null;
			if (is_empty_mask)
			{
				value = "<>";
			}
			provider = new MaskedTextProvider(value, provider.Culture, provider.AllowPromptAsInput, provider.PromptChar, provider.PasswordChar, provider.AsciiOnly);
			ReCalculatePasswordChar();
			UpdateVisibleText();
		}
	}

	[Browsable(false)]
	public bool MaskCompleted => provider.MaskCompleted;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public MaskedTextProvider MaskedTextProvider
	{
		get
		{
			if (is_empty_mask)
			{
				return null;
			}
			return provider.Clone() as MaskedTextProvider;
		}
	}

	[Browsable(false)]
	public bool MaskFull => provider.MaskFull;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override int MaxLength
	{
		get
		{
			return base.MaxLength;
		}
		set
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override bool Multiline
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue('\0')]
	public char PasswordChar
	{
		get
		{
			if (use_system_password_char)
			{
				return '*';
			}
			return provider.PasswordChar;
		}
		set
		{
			provider.PasswordChar = value;
			if (value != 0)
			{
				provider.IsPassword = true;
			}
			else
			{
				provider.IsPassword = false;
			}
			ReCalculatePasswordChar(using_password: true);
			CalculateDocument();
			UpdateVisibleText();
		}
	}

	[DefaultValue('_')]
	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public char PromptChar
	{
		get
		{
			return provider.PromptChar;
		}
		set
		{
			provider.PromptChar = value;
			UpdateVisibleText();
		}
	}

	public new bool ReadOnly
	{
		get
		{
			return base.ReadOnly;
		}
		set
		{
			base.ReadOnly = value;
		}
	}

	[DefaultValue(false)]
	public bool RejectInputOnFirstFailure
	{
		get
		{
			return reject_input_on_first_failure;
		}
		set
		{
			reject_input_on_first_failure = value;
		}
	}

	[DefaultValue(true)]
	public bool ResetOnPrompt
	{
		get
		{
			return provider.ResetOnPrompt;
		}
		set
		{
			provider.ResetOnPrompt = value;
		}
	}

	[DefaultValue(true)]
	public bool ResetOnSpace
	{
		get
		{
			return provider.ResetOnSpace;
		}
		set
		{
			provider.ResetOnSpace = value;
		}
	}

	public override string SelectedText
	{
		get
		{
			return base.SelectedText;
		}
		set
		{
			base.SelectedText = value;
			UpdateVisibleText();
		}
	}

	[DefaultValue(true)]
	public bool SkipLiterals
	{
		get
		{
			return provider.SkipLiterals;
		}
		set
		{
			provider.SkipLiterals = value;
		}
	}

	[Editor("System.Windows.Forms.Design.MaskedTextBoxTextEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DefaultValue("")]
	[RefreshProperties(RefreshProperties.Repaint)]
	[Localizable(true)]
	[Bindable(true)]
	public override string Text
	{
		get
		{
			if (is_empty_mask || setting_text)
			{
				return base.Text;
			}
			if (provider == null)
			{
				return string.Empty;
			}
			return provider.ToString();
		}
		set
		{
			if (is_empty_mask)
			{
				setting_text = true;
				base.Text = value;
				setting_text = false;
			}
			else
			{
				InputText(value);
			}
			UpdateVisibleText();
		}
	}

	[DefaultValue(HorizontalAlignment.Left)]
	[Localizable(true)]
	public HorizontalAlignment TextAlign
	{
		get
		{
			return text_align;
		}
		set
		{
			if (text_align != value)
			{
				if (!Enum.IsDefined(typeof(HorizontalAlignment), value))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(HorizontalAlignment));
				}
				text_align = value;
				OnTextAlignChanged(EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	public override int TextLength => Text.Length;

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(MaskFormat.IncludeLiterals)]
	public MaskFormat TextMaskFormat
	{
		get
		{
			if (provider.IncludePrompt && provider.IncludeLiterals)
			{
				return MaskFormat.IncludePromptAndLiterals;
			}
			if (provider.IncludeLiterals)
			{
				return MaskFormat.IncludeLiterals;
			}
			if (provider.IncludePrompt)
			{
				return MaskFormat.IncludePrompt;
			}
			return MaskFormat.ExcludePromptAndLiterals;
		}
		set
		{
			if (!Enum.IsDefined(typeof(MaskFormat), value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(MaskFormat));
			}
			provider.IncludeLiterals = (value & MaskFormat.IncludeLiterals) == MaskFormat.IncludeLiterals;
			provider.IncludePrompt = (value & MaskFormat.IncludePrompt) == MaskFormat.IncludePrompt;
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(false)]
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
				if (use_system_password_char)
				{
					PasswordChar = PasswordChar;
				}
				else
				{
					PasswordChar = '\0';
				}
			}
		}
	}

	[DefaultValue(null)]
	[Browsable(false)]
	public Type ValidatingType
	{
		get
		{
			return validating_type;
		}
		set
		{
			validating_type = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool WordWrap
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler AcceptsTabChanged
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

	public event EventHandler IsOverwriteModeChanged
	{
		add
		{
			base.Events.AddHandler(IsOverwriteModeChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(IsOverwriteModeChangedEvent, value);
		}
	}

	public event EventHandler MaskChanged
	{
		add
		{
			base.Events.AddHandler(MaskChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MaskChangedEvent, value);
		}
	}

	public event MaskInputRejectedEventHandler MaskInputRejected
	{
		add
		{
			base.Events.AddHandler(MaskInputRejectedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MaskInputRejectedEvent, value);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler MultilineChanged
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

	public event TypeValidationEventHandler TypeValidationCompleted
	{
		add
		{
			base.Events.AddHandler(TypeValidationCompletedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(TypeValidationCompletedEvent, value);
		}
	}

	public MaskedTextBox()
	{
		provider = new MaskedTextProvider("<>", CultureInfo.CurrentCulture);
		is_empty_mask = true;
		Init();
	}

	public MaskedTextBox(MaskedTextProvider maskedTextProvider)
	{
		if (maskedTextProvider == null)
		{
			throw new ArgumentNullException();
		}
		provider = maskedTextProvider;
		is_empty_mask = false;
		Init();
	}

	public MaskedTextBox(string mask)
	{
		if (mask == null)
		{
			throw new ArgumentNullException();
		}
		provider = new MaskedTextProvider(mask, CultureInfo.CurrentCulture);
		is_empty_mask = false;
		Init();
	}

	static MaskedTextBox()
	{
		AcceptsTabChanged = new object();
		IsOverwriteModeChanged = new object();
		MaskChanged = new object();
		MaskInputRejected = new object();
		MultilineChanged = new object();
		TextAlignChanged = new object();
		TypeValidationCompleted = new object();
	}

	private void Init()
	{
		BackColor = SystemColors.Window;
		cut_copy_mask_format = MaskFormat.IncludeLiterals;
		insert_key_overwriting = false;
		UpdateVisibleText();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new void ClearUndo()
	{
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[PermissionSet(SecurityAction.InheritanceDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.UIPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nWindow=\"AllWindows\"/>\n</PermissionSet>\n")]
	protected override void CreateHandle()
	{
		base.CreateHandle();
	}

	public override char GetCharFromPosition(Point pt)
	{
		return base.GetCharFromPosition(pt);
	}

	public override int GetCharIndexFromPosition(Point pt)
	{
		return base.GetCharIndexFromPosition(pt);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new int GetFirstCharIndexFromLine(int lineNumber)
	{
		return 0;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new int GetFirstCharIndexOfCurrentLine()
	{
		return 0;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override int GetLineFromCharIndex(int index)
	{
		return 0;
	}

	public override Point GetPositionFromCharIndex(int index)
	{
		return base.GetPositionFromCharIndex(index);
	}

	protected override bool IsInputKey(Keys keyData)
	{
		return base.IsInputKey(keyData);
	}

	protected override void OnBackColorChanged(EventArgs e)
	{
		base.OnBackColorChanged(e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnIsOverwriteModeChanged(EventArgs e)
	{
		((EventHandler)base.Events[IsOverwriteModeChanged])?.Invoke(this, e);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Insert && insert_key_mode == InsertKeyMode.Default)
		{
			insert_key_overwriting = !insert_key_overwriting;
			OnIsOverwriteModeChanged(EventArgs.Empty);
			e.Handled = true;
		}
		else if (e.KeyCode != Keys.Delete || is_empty_mask)
		{
			base.OnKeyDown(e);
		}
		else
		{
			int endPosition = ((SelectionLength != 0) ? (base.SelectionStart + SelectionLength - 1) : base.SelectionStart);
			int testPosition;
			MaskedTextResultHint resultHint;
			bool result = provider.RemoveAt(base.SelectionStart, endPosition, out testPosition, out resultHint);
			PostprocessKeyboardInput(result, testPosition, testPosition, resultHint);
			e.Handled = true;
		}
	}

	protected override void OnKeyPress(KeyPressEventArgs e)
	{
		if (is_empty_mask)
		{
			base.OnKeyPress(e);
			return;
		}
		bool result;
		int testPosition;
		MaskedTextResultHint resultHint;
		int newPosition;
		if (e.KeyChar == '\b')
		{
			result = ((SelectionLength != 0) ? provider.RemoveAt(base.SelectionStart, base.SelectionStart + SelectionLength - 1, out testPosition, out resultHint) : provider.RemoveAt(base.SelectionStart - 1, base.SelectionStart - 1, out testPosition, out resultHint));
			newPosition = testPosition;
		}
		else if (IsOverwriteMode || SelectionLength > 0)
		{
			int num = provider.FindEditPositionFrom(base.SelectionStart, direction: true);
			int endPosition = ((SelectionLength <= 0) ? num : (base.SelectionStart + SelectionLength - 1));
			result = provider.Replace(e.KeyChar, num, endPosition, out testPosition, out resultHint);
			newPosition = testPosition + 1;
		}
		else
		{
			result = provider.InsertAt(e.KeyChar, base.SelectionStart, out testPosition, out resultHint);
			newPosition = testPosition + 1;
		}
		PostprocessKeyboardInput(result, newPosition, testPosition, resultHint);
		e.Handled = true;
	}

	private void PostprocessKeyboardInput(bool result, int newPosition, int testPosition, MaskedTextResultHint resultHint)
	{
		if (!result)
		{
			OnMaskInputRejected(new MaskInputRejectedEventArgs(testPosition, resultHint));
			return;
		}
		if (newPosition != MaskedTextProvider.InvalidIndex)
		{
			base.SelectionStart = newPosition;
		}
		else
		{
			base.SelectionStart = provider.Length;
		}
		UpdateVisibleText();
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		base.OnKeyUp(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnMaskChanged(EventArgs e)
	{
		((EventHandler)base.Events[MaskChanged])?.Invoke(this, e);
	}

	private void OnMaskInputRejected(MaskInputRejectedEventArgs e)
	{
		((MaskInputRejectedEventHandler)base.Events[MaskInputRejected])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void OnMultilineChanged(EventArgs e)
	{
		((EventHandler)base.Events[MultilineChanged])?.Invoke(this, e);
	}

	protected virtual void OnTextAlignChanged(EventArgs e)
	{
		((EventHandler)base.Events[TextAlignChanged])?.Invoke(this, e);
	}

	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnValidating(CancelEventArgs e)
	{
		base.OnValidating(e);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		return base.ProcessCmdKey(ref msg, keyData);
	}

	protected internal override bool ProcessKeyMessage(ref Message m)
	{
		return base.ProcessKeyMessage(ref m);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new void ScrollToCaret()
	{
	}

	public override string ToString()
	{
		return base.ToString() + ", Text: " + provider.ToString(includePrompt: false, includeLiterals: false);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new void Undo()
	{
	}

	public object ValidateText()
	{
		throw new NotImplementedException();
	}

	protected override void WndProc(ref Message m)
	{
		Msg msg = (Msg)m.Msg;
		base.WndProc(ref m);
	}

	private void ReCalculatePasswordChar()
	{
		ReCalculatePasswordChar(PasswordChar != '\0');
	}

	private void ReCalculatePasswordChar(bool using_password)
	{
		if (using_password)
		{
			if (is_empty_mask)
			{
				document.PasswordChar = PasswordChar.ToString();
			}
			else
			{
				document.PasswordChar = string.Empty;
			}
		}
	}

	internal override void OnPaintInternal(PaintEventArgs pevent)
	{
		base.OnPaintInternal(pevent);
	}

	internal override Color ChangeBackColor(Color backColor)
	{
		return backColor;
	}

	private void UpdateVisibleText()
	{
		string text = null;
		text = ((is_empty_mask || setting_text) ? base.Text : ((provider != null) ? provider.ToDisplayString() : string.Empty));
		setting_text = true;
		if (base.Text != text)
		{
			int selectionStart = base.SelectionStart;
			base.Text = text;
			base.SelectionStart = selectionStart;
		}
		setting_text = false;
	}

	private void InputText(string text)
	{
		MaskedTextResultHint resultHint;
		int testPosition;
		if (RejectInputOnFirstFailure)
		{
			if (!provider.Set(text, out testPosition, out resultHint))
			{
				OnMaskInputRejected(new MaskInputRejectedEventArgs(testPosition, resultHint));
			}
			return;
		}
		provider.Clear();
		testPosition = 0;
		foreach (char input in text)
		{
			if (provider.InsertAt(input, testPosition, out testPosition, out resultHint))
			{
				testPosition++;
			}
			else
			{
				OnMaskInputRejected(new MaskInputRejectedEventArgs(testPosition, resultHint));
			}
		}
	}
}
