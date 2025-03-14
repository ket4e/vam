using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[DefaultEvent("Apply")]
[DefaultProperty("Font")]
public class FontDialog : CommonDialog
{
	internal class ColorComboBox : ComboBox
	{
		internal class ColorComboBoxItem
		{
			private Color color;

			private string name;

			public Color Color
			{
				get
				{
					return color;
				}
				set
				{
					color = value;
				}
			}

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}

			public ColorComboBoxItem(Color color, string name)
			{
				this.color = color;
				this.name = name;
			}

			public override string ToString()
			{
				return Name;
			}
		}

		private Color selectedColor;

		private FontDialog fontDialog;

		public ColorComboBox(FontDialog fontDialog)
		{
			this.fontDialog = fontDialog;
			base.DropDownStyle = ComboBoxStyle.DropDownList;
			base.DrawMode = DrawMode.OwnerDrawFixed;
			base.Items.AddRange(new object[16]
			{
				new ColorComboBoxItem(Color.Black, "Black"),
				new ColorComboBoxItem(Color.DarkRed, "Dark-Red"),
				new ColorComboBoxItem(Color.Green, "Green"),
				new ColorComboBoxItem(Color.Olive, "Olive-Green"),
				new ColorComboBoxItem(Color.Aquamarine, "Aquamarine"),
				new ColorComboBoxItem(Color.Crimson, "Crimson"),
				new ColorComboBoxItem(Color.Cyan, "Cyan"),
				new ColorComboBoxItem(Color.Gray, "Gray"),
				new ColorComboBoxItem(Color.Silver, "Silver"),
				new ColorComboBoxItem(Color.Red, "Red"),
				new ColorComboBoxItem(Color.YellowGreen, "Yellow-Green"),
				new ColorComboBoxItem(Color.Yellow, "Yellow"),
				new ColorComboBoxItem(Color.Blue, "Blue"),
				new ColorComboBoxItem(Color.Purple, "Purple"),
				new ColorComboBoxItem(Color.Aquamarine, "Aquamarine"),
				new ColorComboBoxItem(Color.White, "White")
			});
			SelectedIndex = 0;
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index != -1)
			{
				ColorComboBoxItem colorComboBoxItem = base.Items[e.Index] as ColorComboBoxItem;
				Rectangle rectangle = e.Bounds;
				rectangle.X += 24;
				if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
				{
					e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(Color.Blue), e.Bounds);
					e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(colorComboBoxItem.Color), e.Bounds.X + 3, e.Bounds.Y + 3, e.Bounds.X + 16, e.Bounds.Bottom - 3);
					e.Graphics.DrawRectangle(ThemeEngine.Current.ResPool.GetPen(Color.Black), e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.X + 17, e.Bounds.Bottom - 3);
					e.Graphics.DrawString(colorComboBoxItem.Name, Font, ThemeEngine.Current.ResPool.GetSolidBrush(Color.White), rectangle);
				}
				else
				{
					e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(Color.White), e.Bounds);
					e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(colorComboBoxItem.Color), e.Bounds.X + 3, e.Bounds.Y + 3, e.Bounds.X + 16, e.Bounds.Bottom - 3);
					e.Graphics.DrawRectangle(ThemeEngine.Current.ResPool.GetPen(Color.Black), e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.X + 17, e.Bounds.Bottom - 3);
					e.Graphics.DrawString(colorComboBoxItem.Name, Font, ThemeEngine.Current.ResPool.GetSolidBrush(Color.Black), rectangle);
				}
			}
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			ColorComboBoxItem colorComboBoxItem = base.Items[SelectedIndex] as ColorComboBoxItem;
			selectedColor = colorComboBoxItem.Color;
			fontDialog.Color = selectedColor;
		}
	}

	protected static readonly object EventApply = new object();

	private Font font;

	private Color color = Color.Black;

	private bool allowSimulations = true;

	private bool allowVectorFonts = true;

	private bool allowVerticalFonts = true;

	private bool allowScriptChange = true;

	private bool fixedPitchOnly;

	private int maxSize;

	private int minSize;

	private bool scriptsOnly;

	private bool showApply;

	private bool showColor;

	private bool showEffects = true;

	private bool showHelp;

	private bool fontMustExist;

	private Panel examplePanel;

	private Button okButton;

	private Button cancelButton;

	private Button applyButton;

	private Button helpButton;

	private TextBox fontTextBox;

	private TextBox fontstyleTextBox;

	private TextBox fontsizeTextBox;

	private MouseWheelListBox fontListBox;

	private MouseWheelListBox fontstyleListBox;

	private MouseWheelListBox fontsizeListBox;

	private GroupBox effectsGroupBox;

	private CheckBox strikethroughCheckBox;

	private CheckBox underlinedCheckBox;

	private ComboBox scriptComboBox;

	private Label fontLabel;

	private Label fontstyleLabel;

	private Label sizeLabel;

	private Label scriptLabel;

	private GroupBox exampleGroupBox;

	private ColorComboBox colorComboBox;

	private string currentFontName;

	private float currentSize;

	private FontFamily currentFamily;

	private FontStyle currentFontStyle;

	private bool underlined;

	private bool strikethrough;

	private Hashtable fontHash = new Hashtable();

	private int[] a_sizes = new int[18]
	{
		6, 7, 8, 9, 10, 11, 12, 14, 16, 18,
		20, 22, 24, 26, 28, 36, 48, 72
	};

	private string[] char_sets_names = new string[25]
	{
		"Western", "Symbol", "Shift Jis", "Hangul", "GB2312", "BIG5", "Greek", "Turkish", "Hebrew", "Arabic",
		"Baltic", "Vietname", "Cyrillic", "East European", "Thai", "Johab", "Mac", "OEM", "VISCII", "TCVN",
		"KOI-8", "ISO-8859-3", "ISO-8859-4", "ISO-8859-10", "Celtic"
	};

	private string[] char_sets = new string[25]
	{
		"AaBbYyZz",
		"Symbol",
		"Aa" + 'あ' + 'ぁ' + 'ア' + 'ァ' + '亜' + '宇',
		135036 + "AaBYyZz",
		new string(new char[6] { '微', '软', '中', '文', '软', '件' }),
		new string(new char[6] { '中', '文', '字', '型', '範', '例' }),
		"AaBb" + 'Α' + 'α' + 'Β' + 'β',
		"AaBb" + 'Ğ' + 'ğ' + 'Ş' + 'ş',
		"AaBb" + 'נ' + 'ס' + 'ש' + 'ת',
		"AaBb" + 'ا' + 'ب' + 'ج' + 'د' + 'ه' + 'و' + 'ز',
		"AaBbYyZz",
		"AaBb" + 'Ơ' + 'ơ' + 'Ư' + 'ư',
		"AaBb" + 'Б' + 'б' + 'Ф' + 'ф',
		"AaBb" + 'Á' + 'á' + 'Ô' + 'ô',
		"AaBb" + 'อ' + '\u0e31' + 'ก' + 'ษ' + 'ร' + 'ไ' + 'ท' + 'ย',
		135036 + "AaBYyZz",
		"AaBbYyZz",
		"AaBb" + 'ø' + 'ñ' + 'ý',
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty
	};

	private string example_panel_text;

	private bool internal_change;

	private bool internal_textbox_change;

	public Font Font
	{
		get
		{
			return font;
		}
		set
		{
			if (value != null)
			{
				font = new Font(value, value.Style);
				currentFontStyle = font.Style;
				currentSize = font.SizeInPoints;
				currentFontName = font.Name;
				strikethroughCheckBox.Checked = font.Strikeout;
				underlinedCheckBox.Checked = font.Underline;
				int num = fontListBox.FindString(currentFontName);
				if (num != -1)
				{
					fontListBox.SelectedIndex = num;
				}
				else
				{
					fontListBox.SelectedIndex = 0;
				}
				UpdateFontSizeListBox();
				UpdateFontStyleListBox();
				fontListBox.TopIndex = fontListBox.SelectedIndex;
			}
		}
	}

	[DefaultValue(false)]
	public bool FontMustExist
	{
		get
		{
			return fontMustExist;
		}
		set
		{
			fontMustExist = value;
		}
	}

	[DefaultValue("Color [Black]")]
	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
			examplePanel.Invalidate();
		}
	}

	[DefaultValue(true)]
	public bool AllowSimulations
	{
		get
		{
			return allowSimulations;
		}
		set
		{
			allowSimulations = value;
		}
	}

	[DefaultValue(true)]
	public bool AllowVectorFonts
	{
		get
		{
			return allowVectorFonts;
		}
		set
		{
			allowVectorFonts = value;
		}
	}

	[DefaultValue(true)]
	public bool AllowVerticalFonts
	{
		get
		{
			return allowVerticalFonts;
		}
		set
		{
			allowVerticalFonts = value;
		}
	}

	[DefaultValue(true)]
	public bool AllowScriptChange
	{
		get
		{
			return allowScriptChange;
		}
		set
		{
			allowScriptChange = value;
		}
	}

	[DefaultValue(false)]
	public bool FixedPitchOnly
	{
		get
		{
			return fixedPitchOnly;
		}
		set
		{
			if (fixedPitchOnly != value)
			{
				fixedPitchOnly = value;
				PopulateFontList();
			}
		}
	}

	[DefaultValue(0)]
	public int MaxSize
	{
		get
		{
			return maxSize;
		}
		set
		{
			maxSize = value;
			if (maxSize < 0)
			{
				maxSize = 0;
			}
			if (maxSize < minSize)
			{
				minSize = maxSize;
			}
			CreateFontSizeListBoxItems();
		}
	}

	[DefaultValue(0)]
	public int MinSize
	{
		get
		{
			return minSize;
		}
		set
		{
			minSize = value;
			if (minSize < 0)
			{
				minSize = 0;
			}
			if (minSize > maxSize)
			{
				maxSize = minSize;
			}
			CreateFontSizeListBoxItems();
			if ((float)minSize > currentSize && font != null)
			{
				font.Dispose();
				currentSize = minSize;
				font = new Font(currentFamily, currentSize, currentFontStyle);
				UpdateExamplePanel();
				fontsizeTextBox.Text = currentSize.ToString();
			}
		}
	}

	[DefaultValue(false)]
	public bool ScriptsOnly
	{
		get
		{
			return scriptsOnly;
		}
		set
		{
			scriptsOnly = value;
		}
	}

	[DefaultValue(false)]
	public bool ShowApply
	{
		get
		{
			return showApply;
		}
		set
		{
			if (value != showApply)
			{
				showApply = value;
				if (showApply)
				{
					applyButton.Show();
				}
				else
				{
					applyButton.Hide();
				}
				form.Refresh();
			}
		}
	}

	[DefaultValue(false)]
	public bool ShowColor
	{
		get
		{
			return showColor;
		}
		set
		{
			if (value != showColor)
			{
				showColor = value;
				if (showColor)
				{
					colorComboBox.Show();
				}
				else
				{
					colorComboBox.Hide();
				}
				form.Refresh();
			}
		}
	}

	[DefaultValue(true)]
	public bool ShowEffects
	{
		get
		{
			return showEffects;
		}
		set
		{
			if (value != showEffects)
			{
				showEffects = value;
				if (showEffects)
				{
					effectsGroupBox.Show();
				}
				else
				{
					effectsGroupBox.Hide();
				}
				form.Refresh();
			}
		}
	}

	[DefaultValue(false)]
	public bool ShowHelp
	{
		get
		{
			return showHelp;
		}
		set
		{
			if (value != showHelp)
			{
				showHelp = value;
				if (showHelp)
				{
					helpButton.Show();
				}
				else
				{
					helpButton.Hide();
				}
				form.Refresh();
			}
		}
	}

	protected int Options => 0;

	public event EventHandler Apply
	{
		add
		{
			base.Events.AddHandler(EventApply, value);
		}
		remove
		{
			base.Events.RemoveHandler(EventApply, value);
		}
	}

	public FontDialog()
	{
		form = new DialogForm(this);
		example_panel_text = char_sets[0];
		okButton = new Button();
		cancelButton = new Button();
		applyButton = new Button();
		helpButton = new Button();
		fontTextBox = new TextBox();
		fontstyleTextBox = new TextBox();
		fontsizeTextBox = new TextBox();
		fontListBox = new MouseWheelListBox();
		fontsizeListBox = new MouseWheelListBox();
		fontstyleListBox = new MouseWheelListBox();
		fontLabel = new Label();
		fontstyleLabel = new Label();
		sizeLabel = new Label();
		scriptLabel = new Label();
		exampleGroupBox = new GroupBox();
		effectsGroupBox = new GroupBox();
		underlinedCheckBox = new CheckBox();
		strikethroughCheckBox = new CheckBox();
		scriptComboBox = new ComboBox();
		examplePanel = new Panel();
		colorComboBox = new ColorComboBox(this);
		exampleGroupBox.SuspendLayout();
		effectsGroupBox.SuspendLayout();
		form.SuspendLayout();
		form.FormBorderStyle = FormBorderStyle.FixedDialog;
		form.MaximizeBox = false;
		fontsizeListBox.Location = new Point(284, 47);
		fontsizeListBox.Size = new Size(52, 95);
		fontsizeListBox.TabIndex = 10;
		fontListBox.Sorted = true;
		fontTextBox.Location = new Point(16, 26);
		fontTextBox.Size = new Size(140, 21);
		fontTextBox.TabIndex = 5;
		fontTextBox.Text = string.Empty;
		fontstyleLabel.Location = new Point(164, 10);
		fontstyleLabel.Size = new Size(100, 16);
		fontstyleLabel.TabIndex = 1;
		fontstyleLabel.Text = "Font Style:";
		fontsizeTextBox.Location = new Point(284, 26);
		fontsizeTextBox.Size = new Size(52, 21);
		fontsizeTextBox.TabIndex = 7;
		fontsizeTextBox.Text = string.Empty;
		fontsizeTextBox.MaxLength = 2;
		fontListBox.Location = new Point(16, 47);
		fontListBox.Size = new Size(140, 95);
		fontListBox.TabIndex = 8;
		fontListBox.Sorted = true;
		exampleGroupBox.Controls.Add(examplePanel);
		exampleGroupBox.FlatStyle = FlatStyle.System;
		exampleGroupBox.Location = new Point(164, 158);
		exampleGroupBox.Size = new Size(172, 70);
		exampleGroupBox.TabIndex = 12;
		exampleGroupBox.TabStop = false;
		exampleGroupBox.Text = "Example";
		fontstyleListBox.Location = new Point(164, 47);
		fontstyleListBox.Size = new Size(112, 95);
		fontstyleListBox.TabIndex = 9;
		fontLabel.Location = new Point(16, 10);
		fontLabel.Size = new Size(88, 16);
		fontLabel.TabIndex = 0;
		fontLabel.Text = "Font:";
		effectsGroupBox.Controls.Add(underlinedCheckBox);
		effectsGroupBox.Controls.Add(strikethroughCheckBox);
		effectsGroupBox.Controls.Add(colorComboBox);
		effectsGroupBox.FlatStyle = FlatStyle.System;
		effectsGroupBox.Location = new Point(16, 158);
		effectsGroupBox.Size = new Size(140, 116);
		effectsGroupBox.TabIndex = 11;
		effectsGroupBox.TabStop = false;
		effectsGroupBox.Text = "Effects";
		strikethroughCheckBox.FlatStyle = FlatStyle.System;
		strikethroughCheckBox.Location = new Point(8, 16);
		strikethroughCheckBox.TabIndex = 0;
		strikethroughCheckBox.Text = "Strikethrough";
		colorComboBox.Location = new Point(8, 70);
		colorComboBox.Size = new Size(130, 21);
		sizeLabel.Location = new Point(284, 10);
		sizeLabel.Size = new Size(100, 16);
		sizeLabel.TabIndex = 2;
		sizeLabel.Text = "Size:";
		scriptComboBox.Location = new Point(164, 253);
		scriptComboBox.Size = new Size(172, 21);
		scriptComboBox.TabIndex = 14;
		scriptComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		okButton.FlatStyle = FlatStyle.System;
		okButton.Location = new Point(352, 26);
		okButton.Size = new Size(70, 23);
		okButton.TabIndex = 3;
		okButton.Text = "OK";
		cancelButton.FlatStyle = FlatStyle.System;
		cancelButton.Location = new Point(352, 52);
		cancelButton.Size = new Size(70, 23);
		cancelButton.TabIndex = 4;
		cancelButton.Text = "Cancel";
		applyButton.FlatStyle = FlatStyle.System;
		applyButton.Location = new Point(352, 78);
		applyButton.Size = new Size(70, 23);
		applyButton.TabIndex = 5;
		applyButton.Text = "Apply";
		helpButton.FlatStyle = FlatStyle.System;
		helpButton.Location = new Point(352, 104);
		helpButton.Size = new Size(70, 23);
		helpButton.TabIndex = 6;
		helpButton.Text = "Help";
		underlinedCheckBox.FlatStyle = FlatStyle.System;
		underlinedCheckBox.Location = new Point(8, 36);
		underlinedCheckBox.TabIndex = 1;
		underlinedCheckBox.Text = "Underlined";
		fontstyleTextBox.Location = new Point(164, 26);
		fontstyleTextBox.Size = new Size(112, 21);
		fontstyleTextBox.TabIndex = 6;
		fontstyleTextBox.Text = string.Empty;
		scriptLabel.Location = new Point(164, 236);
		scriptLabel.Size = new Size(100, 16);
		scriptLabel.TabIndex = 13;
		scriptLabel.Text = "Script:";
		examplePanel.Location = new Point(8, 20);
		examplePanel.TabIndex = 0;
		examplePanel.Size = new Size(156, 40);
		examplePanel.BorderStyle = BorderStyle.Fixed3D;
		form.AcceptButton = okButton;
		form.CancelButton = cancelButton;
		form.Controls.Add(scriptComboBox);
		form.Controls.Add(scriptLabel);
		form.Controls.Add(exampleGroupBox);
		form.Controls.Add(effectsGroupBox);
		form.Controls.Add(fontsizeListBox);
		form.Controls.Add(fontstyleListBox);
		form.Controls.Add(fontListBox);
		form.Controls.Add(fontsizeTextBox);
		form.Controls.Add(fontstyleTextBox);
		form.Controls.Add(fontTextBox);
		form.Controls.Add(cancelButton);
		form.Controls.Add(okButton);
		form.Controls.Add(sizeLabel);
		form.Controls.Add(fontstyleLabel);
		form.Controls.Add(fontLabel);
		form.Controls.Add(applyButton);
		form.Controls.Add(helpButton);
		exampleGroupBox.ResumeLayout(performLayout: false);
		effectsGroupBox.ResumeLayout(performLayout: false);
		form.Size = new Size(430, 318);
		form.FormBorderStyle = FormBorderStyle.FixedDialog;
		form.MaximizeBox = false;
		form.Text = "Font";
		form.ResumeLayout(performLayout: false);
		scriptComboBox.BeginUpdate();
		scriptComboBox.Items.AddRange(char_sets_names);
		scriptComboBox.SelectedIndex = 0;
		scriptComboBox.EndUpdate();
		applyButton.Hide();
		helpButton.Hide();
		colorComboBox.Hide();
		cancelButton.Click += OnClickCancelButton;
		okButton.Click += OnClickOkButton;
		applyButton.Click += OnApplyButton;
		examplePanel.Paint += OnPaintExamplePanel;
		fontListBox.SelectedIndexChanged += OnSelectedIndexChangedFontListBox;
		fontsizeListBox.SelectedIndexChanged += OnSelectedIndexChangedSizeListBox;
		fontstyleListBox.SelectedIndexChanged += OnSelectedIndexChangedFontStyleListBox;
		underlinedCheckBox.CheckedChanged += OnCheckedChangedUnderlinedCheckBox;
		strikethroughCheckBox.CheckedChanged += OnCheckedChangedStrikethroughCheckBox;
		scriptComboBox.SelectedIndexChanged += OnSelectedIndexChangedScriptComboBox;
		fontTextBox.KeyPress += OnFontTextBoxKeyPress;
		fontstyleTextBox.KeyPress += OnFontStyleTextBoxKeyPress;
		fontsizeTextBox.KeyPress += OnFontSizeTextBoxKeyPress;
		fontTextBox.TextChanged += OnFontTextBoxTextChanged;
		fontstyleTextBox.TextChanged += OnFontStyleTextTextChanged;
		fontsizeTextBox.TextChanged += OnFontSizeTextBoxTextChanged;
		fontTextBox.KeyDown += OnFontTextBoxKeyDown;
		fontstyleTextBox.KeyDown += OnFontStyleTextBoxKeyDown;
		fontsizeTextBox.KeyDown += OnFontSizeTextBoxKeyDown;
		fontTextBox.MouseWheel += OnFontTextBoxMouseWheel;
		fontstyleTextBox.MouseWheel += OnFontStyleTextBoxMouseWheel;
		fontsizeTextBox.MouseWheel += OnFontSizeTextBoxMouseWheel;
		PopulateFontList();
	}

	public override void Reset()
	{
		color = Color.Black;
		allowSimulations = true;
		allowVectorFonts = true;
		allowVerticalFonts = true;
		allowScriptChange = true;
		fixedPitchOnly = false;
		maxSize = 0;
		minSize = 0;
		CreateFontSizeListBoxItems();
		scriptsOnly = false;
		showApply = false;
		applyButton.Hide();
		showColor = false;
		colorComboBox.Hide();
		showEffects = true;
		effectsGroupBox.Show();
		showHelp = false;
		helpButton.Hide();
		form.Refresh();
	}

	public override string ToString()
	{
		if (font == null)
		{
			return base.ToString();
		}
		return base.ToString() + ", Font: " + font.ToString();
	}

	protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
	{
		return base.HookProc(hWnd, msg, wparam, lparam);
	}

	protected override bool RunDialog(IntPtr hWndOwner)
	{
		form.Refresh();
		return true;
	}

	internal void OnApplyButton(object sender, EventArgs e)
	{
		OnApply(e);
	}

	protected virtual void OnApply(EventArgs e)
	{
		((EventHandler)base.Events[EventApply])?.Invoke(this, e);
	}

	private void OnClickCancelButton(object sender, EventArgs e)
	{
		form.DialogResult = DialogResult.Cancel;
	}

	private void OnClickOkButton(object sender, EventArgs e)
	{
		form.DialogResult = DialogResult.OK;
	}

	private void OnPaintExamplePanel(object sender, PaintEventArgs e)
	{
		SolidBrush solidBrush = ThemeEngine.Current.ResPool.GetSolidBrush(color);
		e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(SystemColors.Control), 0, 0, 156, 40);
		SizeF sizeF = e.Graphics.MeasureString(example_panel_text, font);
		int num = (int)sizeF.Width;
		int num2 = (int)sizeF.Height;
		int num3 = examplePanel.Width / 2 - num / 2;
		if (num3 < 0)
		{
			num3 = 0;
		}
		int y = examplePanel.Height / 2 - num2 / 2;
		e.Graphics.DrawString(example_panel_text, font, solidBrush, new Point(num3, y));
	}

	private void OnSelectedIndexChangedFontListBox(object sender, EventArgs e)
	{
		if (fontListBox.SelectedIndex != -1)
		{
			currentFamily = FindByName(fontListBox.Items[fontListBox.SelectedIndex].ToString());
			fontTextBox.Text = currentFamily.Name;
			internal_change = true;
			UpdateFontStyleListBox();
			UpdateFontSizeListBox();
			UpdateExamplePanel();
			form.Select(fontTextBox);
			internal_change = false;
		}
	}

	private void OnSelectedIndexChangedSizeListBox(object sender, EventArgs e)
	{
		if (fontsizeListBox.SelectedIndex != -1)
		{
			currentSize = (float)Convert.ToDouble(fontsizeListBox.Items[fontsizeListBox.SelectedIndex]);
			fontsizeTextBox.Text = currentSize.ToString();
			UpdateExamplePanel();
			if (!internal_change)
			{
				form.Select(fontsizeTextBox);
			}
		}
	}

	private void OnSelectedIndexChangedFontStyleListBox(object sender, EventArgs e)
	{
		if (fontstyleListBox.SelectedIndex != -1)
		{
			switch (fontstyleListBox.SelectedIndex)
			{
			case 0:
				currentFontStyle = FontStyle.Regular;
				break;
			case 1:
				currentFontStyle = FontStyle.Bold;
				break;
			case 2:
				currentFontStyle = FontStyle.Italic;
				break;
			case 3:
				currentFontStyle = FontStyle.Bold | FontStyle.Italic;
				break;
			default:
				currentFontStyle = FontStyle.Regular;
				break;
			}
			if (underlined)
			{
				currentFontStyle |= FontStyle.Underline;
			}
			if (strikethrough)
			{
				currentFontStyle |= FontStyle.Strikeout;
			}
			fontstyleTextBox.Text = fontstyleListBox.Items[fontstyleListBox.SelectedIndex].ToString();
			if (!internal_change)
			{
				UpdateExamplePanel();
				form.Select(fontstyleTextBox);
			}
		}
	}

	private void OnCheckedChangedUnderlinedCheckBox(object sender, EventArgs e)
	{
		if (underlinedCheckBox.Checked)
		{
			currentFontStyle |= FontStyle.Underline;
			underlined = true;
		}
		else
		{
			currentFontStyle ^= FontStyle.Underline;
			underlined = false;
		}
		UpdateExamplePanel();
	}

	private void OnCheckedChangedStrikethroughCheckBox(object sender, EventArgs e)
	{
		if (strikethroughCheckBox.Checked)
		{
			currentFontStyle |= FontStyle.Strikeout;
			strikethrough = true;
		}
		else
		{
			currentFontStyle ^= FontStyle.Strikeout;
			strikethrough = false;
		}
		UpdateExamplePanel();
	}

	private void OnFontTextBoxMouseWheel(object sender, MouseEventArgs e)
	{
		fontListBox.SendMouseWheelEvent(e);
	}

	private void OnFontStyleTextBoxMouseWheel(object sender, MouseEventArgs e)
	{
		fontstyleListBox.SendMouseWheelEvent(e);
	}

	private void OnFontSizeTextBoxMouseWheel(object sender, MouseEventArgs e)
	{
		fontsizeListBox.SendMouseWheelEvent(e);
	}

	private void OnFontTextBoxKeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.PageUp:
		case Keys.PageDown:
		case Keys.Up:
		case Keys.Down:
			fontListBox.HandleKeyDown(e.KeyCode);
			break;
		case Keys.End:
		case Keys.Home:
		case Keys.Left:
		case Keys.Right:
			break;
		}
	}

	private void OnFontStyleTextBoxKeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.PageUp:
		case Keys.PageDown:
		case Keys.Up:
		case Keys.Down:
			fontstyleListBox.HandleKeyDown(e.KeyCode);
			break;
		case Keys.End:
		case Keys.Home:
		case Keys.Left:
		case Keys.Right:
			break;
		}
	}

	private void OnFontSizeTextBoxKeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.PageUp:
		case Keys.PageDown:
		case Keys.Up:
		case Keys.Down:
			fontsizeListBox.HandleKeyDown(e.KeyCode);
			break;
		case Keys.End:
		case Keys.Home:
		case Keys.Left:
		case Keys.Right:
			break;
		}
	}

	private void OnFontTextBoxKeyPress(object sender, KeyPressEventArgs e)
	{
		internal_textbox_change = true;
		if (fontListBox.SelectedIndex > -1)
		{
			fontListBox.SelectedIndex = -1;
		}
	}

	private void OnFontStyleTextBoxKeyPress(object sender, KeyPressEventArgs e)
	{
		internal_textbox_change = true;
		if (fontstyleListBox.SelectedIndex > -1)
		{
			fontstyleListBox.SelectedIndex = -1;
		}
	}

	private void OnFontSizeTextBoxKeyPress(object sender, KeyPressEventArgs e)
	{
		if (char.IsLetter(e.KeyChar) || char.IsWhiteSpace(e.KeyChar) || char.IsPunctuation(e.KeyChar) || e.KeyChar == ',')
		{
			e.Handled = true;
		}
		else
		{
			internal_textbox_change = true;
		}
	}

	private void OnFontTextBoxTextChanged(object sender, EventArgs e)
	{
		if (!internal_textbox_change)
		{
			return;
		}
		internal_textbox_change = false;
		string text = fontTextBox.Text;
		int num = fontListBox.FindStringExact(text);
		if (num != -1)
		{
			fontListBox.SelectedIndex = num;
			return;
		}
		num = fontListBox.FindString(text);
		if (num != -1)
		{
			fontListBox.TopIndex = num;
		}
		else if (fontListBox.Items.Count > 0)
		{
			fontListBox.TopIndex = 0;
		}
	}

	private void OnFontStyleTextTextChanged(object sender, EventArgs e)
	{
		if (internal_textbox_change)
		{
			internal_textbox_change = false;
			int num = fontstyleListBox.FindStringExact(fontstyleTextBox.Text);
			if (num != -1)
			{
				fontstyleListBox.SelectedIndex = num;
			}
		}
	}

	private void OnFontSizeTextBoxTextChanged(object sender, EventArgs e)
	{
		if (!internal_textbox_change)
		{
			return;
		}
		internal_textbox_change = false;
		if (fontsizeTextBox.Text.Length == 0)
		{
			return;
		}
		for (int i = 0; i < fontsizeListBox.Items.Count; i++)
		{
			string text = fontsizeListBox.Items[i] as string;
			if (text.StartsWith(fontsizeTextBox.Text))
			{
				if (text == fontsizeTextBox.Text)
				{
					fontsizeListBox.SelectedIndex = i;
				}
				else
				{
					fontsizeListBox.TopIndex = i;
				}
				break;
			}
		}
	}

	private void OnSelectedIndexChangedScriptComboBox(object sender, EventArgs e)
	{
		string text = char_sets[scriptComboBox.SelectedIndex];
		if (text.Length > 0)
		{
			example_panel_text = text;
			UpdateExamplePanel();
		}
	}

	private void UpdateExamplePanel()
	{
		if (font != null)
		{
			font.Dispose();
		}
		font = new Font(currentFamily, currentSize, currentFontStyle);
		examplePanel.Invalidate();
	}

	private void UpdateFontSizeListBox()
	{
		int num = fontsizeListBox.FindString(((int)Math.Round(currentSize)).ToString());
		if (num != -1)
		{
			fontsizeListBox.SelectedIndex = num;
		}
		else
		{
			fontsizeListBox.SelectedIndex = 0;
		}
	}

	private void UpdateFontStyleListBox()
	{
		fontstyleListBox.BeginUpdate();
		fontstyleListBox.Items.Clear();
		int num = -1;
		int selectedIndex = 0;
		if (currentFamily.IsStyleAvailable(FontStyle.Regular))
		{
			num = fontstyleListBox.Items.Add("Regular");
			selectedIndex = num;
		}
		if (currentFamily.IsStyleAvailable(FontStyle.Bold))
		{
			num = fontstyleListBox.Items.Add("Bold");
			if ((currentFontStyle & FontStyle.Bold) == FontStyle.Bold)
			{
				selectedIndex = num;
			}
		}
		if (currentFamily.IsStyleAvailable(FontStyle.Italic))
		{
			num = fontstyleListBox.Items.Add("Italic");
			if ((currentFontStyle & FontStyle.Italic) == FontStyle.Italic)
			{
				selectedIndex = num;
			}
		}
		if (currentFamily.IsStyleAvailable(FontStyle.Bold) && currentFamily.IsStyleAvailable(FontStyle.Italic))
		{
			num = fontstyleListBox.Items.Add("Bold Italic");
			if ((currentFontStyle & (FontStyle.Bold | FontStyle.Italic)) == (FontStyle.Bold | FontStyle.Italic))
			{
				selectedIndex = num;
			}
		}
		if (fontstyleListBox.Items.Count > 0)
		{
			fontstyleListBox.SelectedIndex = selectedIndex;
			switch ((string)fontstyleListBox.SelectedItem)
			{
			case "Regular":
				currentFontStyle = FontStyle.Regular;
				break;
			case "Bold":
				currentFontStyle = FontStyle.Bold;
				break;
			case "Italic":
				currentFontStyle = FontStyle.Italic;
				break;
			case "Bold Italic":
				currentFontStyle = FontStyle.Bold | FontStyle.Italic;
				break;
			}
			if (strikethroughCheckBox.Checked)
			{
				currentFontStyle |= FontStyle.Strikeout;
			}
			if (underlinedCheckBox.Checked)
			{
				currentFontStyle |= FontStyle.Underline;
			}
		}
		fontstyleListBox.EndUpdate();
	}

	private FontFamily FindByName(string name)
	{
		return fontHash[name] as FontFamily;
	}

	private void CreateFontSizeListBoxItems()
	{
		fontsizeListBox.BeginUpdate();
		fontsizeListBox.Items.Clear();
		if (minSize == 0 && maxSize == 0)
		{
			int[] array = a_sizes;
			foreach (int num in array)
			{
				fontsizeListBox.Items.Add(num.ToString());
			}
		}
		else
		{
			int[] array2 = a_sizes;
			for (int j = 0; j < array2.Length; j++)
			{
				int num2 = array2[j];
				if (num2 >= minSize && num2 <= maxSize)
				{
					fontsizeListBox.Items.Add(num2.ToString());
				}
			}
		}
		fontsizeListBox.EndUpdate();
	}

	private void PopulateFontList()
	{
		fontListBox.Items.Clear();
		fontHash.Clear();
		fontListBox.BeginUpdate();
		FontFamily[] families = FontFamily.Families;
		foreach (FontFamily fontFamily in families)
		{
			if (!fontHash.ContainsKey(fontFamily.Name) && (!fixedPitchOnly || IsFontFamilyFixedPitch(fontFamily)))
			{
				fontListBox.Items.Add(fontFamily.Name);
				fontHash.Add(fontFamily.Name, fontFamily);
			}
		}
		fontListBox.EndUpdate();
		CreateFontSizeListBoxItems();
		if (fixedPitchOnly)
		{
			Font = new Font(FontFamily.GenericMonospace, 8.25f);
		}
		else
		{
			Font = form.Font;
		}
	}

	private bool IsFontFamilyFixedPitch(FontFamily family)
	{
		FontStyle style;
		if (family.IsStyleAvailable(FontStyle.Regular))
		{
			style = FontStyle.Regular;
		}
		else if (family.IsStyleAvailable(FontStyle.Bold))
		{
			style = FontStyle.Bold;
		}
		else if (family.IsStyleAvailable(FontStyle.Italic))
		{
			style = FontStyle.Italic;
		}
		else if (family.IsStyleAvailable(FontStyle.Strikeout))
		{
			style = FontStyle.Strikeout;
		}
		else
		{
			if (!family.IsStyleAvailable(FontStyle.Underline))
			{
				return false;
			}
			style = FontStyle.Underline;
		}
		Font font = new Font(family.Name, 10f, style);
		if (TextRenderer.MeasureString("i", font).Width == TextRenderer.MeasureString("w", font).Width)
		{
			return true;
		}
		return false;
	}
}
