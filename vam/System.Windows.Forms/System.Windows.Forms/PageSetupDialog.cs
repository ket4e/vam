using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms;

[DefaultProperty("Document")]
public sealed class PageSetupDialog : CommonDialog
{
	private class PrinterForm : Form
	{
		private GroupBox groupbox_printer;

		private ComboBox combobox_printers;

		private Label label_name;

		private Label label_status;

		private Button button_properties;

		private Button button_network;

		private Button button_cancel;

		private Button button_ok;

		private Label label_status_text;

		private Label label_type;

		private Label label_where;

		private Label label_where_text;

		private Label label_type_text;

		private Label label_comment;

		private Label label_comment_text;

		private PageSetupDialog page_setup_dialog;

		public string SelectedPrinter
		{
			get
			{
				return (string)combobox_printers.SelectedItem;
			}
			set
			{
				combobox_printers.SelectedItem = value;
				label_type_text.Text = value;
			}
		}

		public PrinterForm(PageSetupDialog page_setup_dialog)
		{
			InitializeComponent();
			this.page_setup_dialog = page_setup_dialog;
		}

		public void UpdateValues()
		{
			combobox_printers.Items.Clear();
			foreach (string installedPrinter in PrinterSettings.InstalledPrinters)
			{
				combobox_printers.Items.Add(installedPrinter);
			}
			SelectedPrinter = page_setup_dialog.PrinterSettings.PrinterName;
			button_network.Enabled = page_setup_dialog.ShowNetwork;
		}

		private void InitializeComponent()
		{
			this.groupbox_printer = new System.Windows.Forms.GroupBox();
			this.combobox_printers = new System.Windows.Forms.ComboBox();
			this.button_network = new System.Windows.Forms.Button();
			this.button_cancel = new System.Windows.Forms.Button();
			this.button_ok = new System.Windows.Forms.Button();
			this.label_name = new System.Windows.Forms.Label();
			this.label_status = new System.Windows.Forms.Label();
			this.label_status_text = new System.Windows.Forms.Label();
			this.label_type = new System.Windows.Forms.Label();
			this.label_type_text = new System.Windows.Forms.Label();
			this.label_where = new System.Windows.Forms.Label();
			this.label_comment = new System.Windows.Forms.Label();
			this.label_where_text = new System.Windows.Forms.Label();
			this.label_comment_text = new System.Windows.Forms.Label();
			this.button_properties = new System.Windows.Forms.Button();
			this.groupbox_printer.SuspendLayout();
			base.SuspendLayout();
			this.groupbox_printer.Controls.AddRange(new System.Windows.Forms.Control[11]
			{
				this.button_properties, this.label_comment_text, this.label_where_text, this.label_comment, this.label_where, this.label_type_text, this.label_type, this.label_status_text, this.label_status, this.label_name,
				this.combobox_printers
			});
			this.groupbox_printer.Location = new System.Drawing.Point(12, 8);
			this.groupbox_printer.Name = "groupbox_printer";
			this.groupbox_printer.Size = new System.Drawing.Size(438, 136);
			this.groupbox_printer.Text = "Printer";
			this.combobox_printers.Location = new System.Drawing.Point(64, 24);
			this.combobox_printers.Name = "combobox_printers";
			this.combobox_printers.SelectedValueChanged += new System.EventHandler(OnSelectedValueChangedPrinters);
			this.combobox_printers.Size = new System.Drawing.Size(232, 21);
			this.combobox_printers.TabIndex = 1;
			this.button_network.Location = new System.Drawing.Point(16, 160);
			this.button_network.Name = "button_network";
			this.button_network.Size = new System.Drawing.Size(68, 22);
			this.button_network.TabIndex = 5;
			this.button_network.Text = "Network...";
			this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_cancel.Location = new System.Drawing.Point(376, 160);
			this.button_cancel.Name = "button_cancel";
			this.button_cancel.Size = new System.Drawing.Size(68, 22);
			this.button_cancel.TabIndex = 4;
			this.button_cancel.Text = "Cancel";
			this.button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_ok.Location = new System.Drawing.Point(300, 160);
			this.button_ok.Name = "button_ok";
			this.button_ok.Size = new System.Drawing.Size(68, 22);
			this.button_ok.TabIndex = 3;
			this.button_ok.Text = "OK";
			this.label_name.Location = new System.Drawing.Point(12, 28);
			this.label_name.Name = "label_name";
			this.label_name.Size = new System.Drawing.Size(48, 20);
			this.label_name.Text = "Name:";
			this.label_status.Location = new System.Drawing.Point(6, 52);
			this.label_status.Name = "label_status";
			this.label_status.Size = new System.Drawing.Size(58, 14);
			this.label_status.Text = "Status:";
			this.label_status_text.Location = new System.Drawing.Point(64, 52);
			this.label_status_text.Name = "label_status_text";
			this.label_status_text.Size = new System.Drawing.Size(64, 14);
			this.label_status_text.Text = string.Empty;
			this.label_type.Location = new System.Drawing.Point(6, 72);
			this.label_type.Name = "label_type";
			this.label_type.Size = new System.Drawing.Size(58, 14);
			this.label_type.Text = "Type:";
			this.label_type_text.Location = new System.Drawing.Point(64, 72);
			this.label_type_text.Name = "label_type_text";
			this.label_type_text.Size = new System.Drawing.Size(232, 14);
			this.label_type_text.TabIndex = 5;
			this.label_type_text.Text = string.Empty;
			this.label_where.Location = new System.Drawing.Point(6, 92);
			this.label_where.Name = "label_where";
			this.label_where.Size = new System.Drawing.Size(58, 16);
			this.label_where.TabIndex = 6;
			this.label_where.Text = "Where:";
			this.label_comment.Location = new System.Drawing.Point(6, 112);
			this.label_comment.Name = "label_comment";
			this.label_comment.Size = new System.Drawing.Size(56, 16);
			this.label_comment.Text = "Comment:";
			this.label_where_text.Location = new System.Drawing.Point(64, 92);
			this.label_where_text.Name = "label_where_text";
			this.label_where_text.Size = new System.Drawing.Size(232, 16);
			this.label_where_text.Text = string.Empty;
			this.label_comment_text.Location = new System.Drawing.Point(64, 112);
			this.label_comment_text.Name = "label_comment_text";
			this.label_comment_text.Size = new System.Drawing.Size(232, 16);
			this.label_comment_text.Text = string.Empty;
			this.button_properties.Location = new System.Drawing.Point(308, 22);
			this.button_properties.Name = "button_properties";
			this.button_properties.Size = new System.Drawing.Size(92, 22);
			this.button_properties.TabIndex = 2;
			this.button_properties.Text = "Properties...";
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			base.AcceptButton = this.button_ok;
			base.CancelButton = this.button_cancel;
			base.ClientSize = new System.Drawing.Size(456, 194);
			base.Controls.AddRange(new System.Windows.Forms.Control[4] { this.button_ok, this.button_cancel, this.button_network, this.groupbox_printer });
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.HelpButton = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PrinterForm";
			base.ShowInTaskbar = false;
			this.Text = "Configure page";
			this.groupbox_printer.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void OnSelectedValueChangedPrinters(object sender, EventArgs args)
		{
			SelectedPrinter = (string)combobox_printers.SelectedItem;
		}
	}

	private class PagePreview : UserControl
	{
		private int width;

		private int height;

		private int marginBottom;

		private int marginTop;

		private int marginLeft;

		private int marginRight;

		private bool landscape;

		private bool loaded;

		private StringBuilder sb;

		private float displayHeight;

		private new Font font;

		public bool Landscape
		{
			get
			{
				return landscape;
			}
			set
			{
				if (landscape != value)
				{
					landscape = value;
					Invalidate();
				}
			}
		}

		public new float Height
		{
			get
			{
				return displayHeight;
			}
			set
			{
				if (displayHeight != value)
				{
					displayHeight = value;
					Invalidate();
				}
			}
		}

		public PagePreview()
		{
			sb = new StringBuilder();
			for (int i = 0; i < 4; i++)
			{
				sb.Append("blabla piu piublapiu haha lai dlais dhl\ufffdai shd ");
				sb.Append("\ufffdoasd \ufffdlaj sd\ufffd\r\n lajsd l\ufffdaisdj l\ufffdillaisd lahs dli");
				sb.Append("laksjd liasjdliasdj blabla piu piublapiu haha ");
				sb.Append("lai dlais dhl\ufffdai shd \ufffdoasd \ufffdlaj sd\ufffd lajsd l\ufffdaisdj");
				sb.Append(" l\ufffdillaisd lahs dli laksjd liasjdliasdj\r\n\r\n");
			}
			font = new Font(FontFamily.GenericSansSerif, 4f);
			displayHeight = 130f;
		}

		public void SetSize(int width, int height)
		{
			this.width = width;
			this.height = height;
			Invalidate();
		}

		public void SetMargins(int left, int right, int top, int bottom)
		{
			marginBottom = bottom;
			marginTop = top;
			marginLeft = left;
			marginRight = right;
			Invalidate();
		}

		public void Setup(PageSettings pageSettings)
		{
			width = pageSettings.PaperSize.Width;
			height = pageSettings.PaperSize.Height;
			Margins margins = pageSettings.Margins;
			marginBottom = margins.Bottom;
			marginTop = margins.Top;
			marginLeft = margins.Left;
			marginRight = margins.Right;
			landscape = pageSettings.Landscape;
			loaded = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (!loaded)
			{
				base.OnPaint(e);
				return;
			}
			Graphics graphics = e.Graphics;
			float num = displayHeight;
			float num2 = (float)width * displayHeight / (float)height;
			float num3 = (float)marginTop * displayHeight / (float)height;
			float num4 = (float)marginLeft * displayHeight / (float)height;
			float num5 = (float)marginBottom * displayHeight / (float)height;
			float num6 = (float)marginRight * displayHeight / (float)height;
			if (landscape)
			{
				float num7 = num2;
				num2 = num;
				num = num7;
				num7 = num6;
				num6 = num3;
				num3 = num4;
				num4 = num5;
				num5 = num7;
			}
			graphics.FillRectangle(SystemBrushes.ControlDark, 4f, 4f, num2 + 4f, num + 4f);
			graphics.FillRectangle(Brushes.White, 0f, 0f, num2, num);
			RectangleF rectangleF = new RectangleF(0f, 0f, num2, num);
			RectangleF rectangleF2 = new RectangleF(num4, num3, num2 - num4 - num6, num - num3 - num5);
			ControlPaint.DrawBorder(graphics, rectangleF, Color.Black, ButtonBorderStyle.Solid);
			ControlPaint.DrawBorder(graphics, rectangleF2, SystemColors.ControlDark, ButtonBorderStyle.Dashed);
			graphics.DrawString(sb.ToString(), font, Brushes.Black, new RectangleF(rectangleF2.X + 2f, rectangleF2.Y + 2f, rectangleF2.Width - 4f, rectangleF2.Height - 4f));
			base.OnPaint(e);
		}
	}

	private PrintDocument document;

	private PageSettings page_settings;

	private PrinterSettings printer_settings;

	private Margins min_margins;

	private bool allow_margins;

	private bool allow_orientation;

	private bool allow_paper;

	private bool allow_printer;

	private bool show_help;

	private bool show_network;

	private bool enable_metric;

	private GroupBox groupbox_paper;

	private Label label_source;

	private Label label_size;

	private GroupBox groupbox_orientation;

	private RadioButton radio_landscape;

	private RadioButton radio_portrait;

	private GroupBox groupbox_margin;

	private Label label_left;

	private Button button_help;

	private Button button_ok;

	private Button button_cancel;

	private Button button_printer;

	private Label label_top;

	private Label label_right;

	private Label label_bottom;

	private NumericTextBox textbox_left;

	private NumericTextBox textbox_top;

	private NumericTextBox textbox_right;

	private NumericTextBox textbox_bottom;

	private ComboBox combobox_source;

	private ComboBox combobox_size;

	private PagePreview pagePreview;

	[DefaultValue(true)]
	public bool AllowMargins
	{
		get
		{
			return allow_margins;
		}
		set
		{
			allow_margins = value;
		}
	}

	[DefaultValue(true)]
	public bool AllowOrientation
	{
		get
		{
			return allow_orientation;
		}
		set
		{
			allow_orientation = value;
		}
	}

	[DefaultValue(true)]
	public bool AllowPaper
	{
		get
		{
			return allow_paper;
		}
		set
		{
			allow_paper = value;
		}
	}

	[DefaultValue(true)]
	public bool AllowPrinter
	{
		get
		{
			return allow_printer;
		}
		set
		{
			allow_printer = value;
		}
	}

	[DefaultValue(null)]
	public PrintDocument Document
	{
		get
		{
			return document;
		}
		set
		{
			document = value;
			if (document != null)
			{
				printer_settings = document.PrinterSettings;
				page_settings = document.DefaultPageSettings;
			}
		}
	}

	[Browsable(true)]
	[DefaultValue(false)]
	[System.MonoTODO("Stubbed, not implemented")]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public bool EnableMetric
	{
		get
		{
			return enable_metric;
		}
		set
		{
			enable_metric = value;
		}
	}

	public Margins MinMargins
	{
		get
		{
			return min_margins;
		}
		set
		{
			min_margins = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[DefaultValue(null)]
	public PageSettings PageSettings
	{
		get
		{
			return page_settings;
		}
		set
		{
			page_settings = value;
			document = null;
		}
	}

	[DefaultValue(null)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public PrinterSettings PrinterSettings
	{
		get
		{
			return printer_settings;
		}
		set
		{
			printer_settings = value;
			document = null;
		}
	}

	[DefaultValue(false)]
	public bool ShowHelp
	{
		get
		{
			return show_help;
		}
		set
		{
			if (value != show_help)
			{
				show_help = value;
				ShowHelpButton();
			}
		}
	}

	[DefaultValue(true)]
	public bool ShowNetwork
	{
		get
		{
			return show_network;
		}
		set
		{
			show_network = value;
		}
	}

	private static bool UseYardPound => !RegionInfo.CurrentRegion.IsMetric;

	private PrinterSettings InternalPrinterSettings => (printer_settings != null) ? printer_settings : page_settings.PrinterSettings;

	public PageSetupDialog()
	{
		form = new DialogForm(this);
		InitializeComponent();
		Reset();
	}

	public override void Reset()
	{
		AllowMargins = true;
		AllowOrientation = true;
		AllowPaper = true;
		AllowPrinter = true;
		ShowHelp = false;
		ShowNetwork = true;
		MinMargins = new Margins(0, 0, 0, 0);
		PrinterSettings = null;
		PageSettings = null;
		Document = null;
	}

	protected override bool RunDialog(IntPtr hwndOwner)
	{
		try
		{
			SetPrinterDetails();
			return true;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}
	}

	private void InitializeComponent()
	{
		groupbox_paper = new GroupBox();
		combobox_source = new ComboBox();
		combobox_size = new ComboBox();
		label_source = new Label();
		label_size = new Label();
		groupbox_orientation = new GroupBox();
		radio_landscape = new RadioButton();
		radio_portrait = new RadioButton();
		groupbox_margin = new GroupBox();
		label_left = new Label();
		button_ok = new Button();
		button_cancel = new Button();
		button_printer = new Button();
		label_top = new Label();
		label_right = new Label();
		label_bottom = new Label();
		textbox_left = new NumericTextBox();
		textbox_top = new NumericTextBox();
		textbox_right = new NumericTextBox();
		textbox_bottom = new NumericTextBox();
		pagePreview = new PagePreview();
		groupbox_paper.SuspendLayout();
		groupbox_orientation.SuspendLayout();
		groupbox_margin.SuspendLayout();
		form.SuspendLayout();
		groupbox_paper.Controls.Add(combobox_source);
		groupbox_paper.Controls.Add(combobox_size);
		groupbox_paper.Controls.Add(label_source);
		groupbox_paper.Controls.Add(label_size);
		groupbox_paper.Location = new Point(12, 157);
		groupbox_paper.Name = "groupbox_paper";
		groupbox_paper.Size = new Size(336, 90);
		groupbox_paper.TabIndex = 0;
		groupbox_paper.TabStop = false;
		groupbox_paper.Text = "Paper";
		combobox_source.Location = new Point(84, 54);
		combobox_source.Name = "combobox_source";
		combobox_source.Size = new Size(240, 21);
		combobox_source.TabIndex = 3;
		combobox_size.ItemHeight = 13;
		combobox_size.Location = new Point(84, 22);
		combobox_size.Name = "combobox_size";
		combobox_size.Size = new Size(240, 21);
		combobox_size.TabIndex = 2;
		combobox_size.SelectedIndexChanged += OnPaperSizeChange;
		label_source.Location = new Point(13, 58);
		label_source.Name = "label_source";
		label_source.Size = new Size(48, 16);
		label_source.TabIndex = 1;
		label_source.Text = "&Source:";
		label_size.Location = new Point(13, 25);
		label_size.Name = "label_size";
		label_size.Size = new Size(52, 16);
		label_size.TabIndex = 0;
		label_size.Text = "Si&ze:";
		groupbox_orientation.Controls.Add(radio_landscape);
		groupbox_orientation.Controls.Add(radio_portrait);
		groupbox_orientation.Location = new Point(12, 255);
		groupbox_orientation.Name = "groupbox_orientation";
		groupbox_orientation.Size = new Size(96, 90);
		groupbox_orientation.TabIndex = 1;
		groupbox_orientation.TabStop = false;
		groupbox_orientation.Text = "Orientation";
		radio_landscape.Location = new Point(13, 52);
		radio_landscape.Name = "radio_landscape";
		radio_landscape.Size = new Size(80, 24);
		radio_landscape.TabIndex = 7;
		radio_landscape.Text = "L&andscape";
		radio_landscape.CheckedChanged += OnLandscapeChange;
		radio_portrait.Location = new Point(13, 19);
		radio_portrait.Name = "radio_portrait";
		radio_portrait.Size = new Size(72, 24);
		radio_portrait.TabIndex = 6;
		radio_portrait.Text = "P&ortrait";
		groupbox_margin.Controls.Add(textbox_bottom);
		groupbox_margin.Controls.Add(textbox_right);
		groupbox_margin.Controls.Add(textbox_top);
		groupbox_margin.Controls.Add(textbox_left);
		groupbox_margin.Controls.Add(label_bottom);
		groupbox_margin.Controls.Add(label_right);
		groupbox_margin.Controls.Add(label_top);
		groupbox_margin.Controls.Add(label_left);
		groupbox_margin.Location = new Point(120, 255);
		groupbox_margin.Name = "groupbox_margin";
		groupbox_margin.Size = new Size(228, 90);
		groupbox_margin.TabIndex = 2;
		groupbox_margin.TabStop = false;
		groupbox_margin.Text = LocalizedLengthUnit();
		label_left.Location = new Point(11, 25);
		label_left.Name = "label_left";
		label_left.Size = new Size(40, 23);
		label_left.TabIndex = 0;
		label_left.Text = "&Left:";
		button_ok.Location = new Point(120, 358);
		button_ok.Name = "button_ok";
		button_ok.Size = new Size(72, 23);
		button_ok.TabIndex = 3;
		button_ok.Text = "OK";
		button_ok.Click += OnClickOkButton;
		button_cancel.DialogResult = DialogResult.Cancel;
		button_cancel.Location = new Point(198, 358);
		button_cancel.Name = "button_cancel";
		button_cancel.Size = new Size(72, 23);
		button_cancel.TabIndex = 4;
		button_cancel.Text = "Cancel";
		button_printer.Location = new Point(276, 358);
		button_printer.Name = "button_printer";
		button_printer.Size = new Size(72, 23);
		button_printer.TabIndex = 5;
		button_printer.Text = "&Printer...";
		button_printer.Click += OnClickPrinterButton;
		label_top.Location = new Point(11, 57);
		label_top.Name = "label_top";
		label_top.Size = new Size(40, 23);
		label_top.TabIndex = 1;
		label_top.Text = "&Top:";
		label_right.Location = new Point(124, 25);
		label_right.Name = "label_right";
		label_right.Size = new Size(40, 23);
		label_right.TabIndex = 2;
		label_right.Text = "&Right:";
		label_bottom.Location = new Point(124, 57);
		label_bottom.Name = "label_bottom";
		label_bottom.Size = new Size(40, 23);
		label_bottom.TabIndex = 3;
		label_bottom.Text = "&Bottom:";
		textbox_left.Location = new Point(57, 21);
		textbox_left.Name = "textbox_left";
		textbox_left.Size = new Size(48, 20);
		textbox_left.TabIndex = 4;
		textbox_left.TextChanged += OnMarginChange;
		textbox_top.Location = new Point(57, 54);
		textbox_top.Name = "textbox_top";
		textbox_top.Size = new Size(48, 20);
		textbox_top.TabIndex = 5;
		textbox_top.TextChanged += OnMarginChange;
		textbox_right.Location = new Point(171, 21);
		textbox_right.Name = "textbox_right";
		textbox_right.Size = new Size(48, 20);
		textbox_right.TabIndex = 6;
		textbox_right.TextChanged += OnMarginChange;
		textbox_bottom.Location = new Point(171, 54);
		textbox_bottom.Name = "textbox_bottom";
		textbox_bottom.Size = new Size(48, 20);
		textbox_bottom.TabIndex = 7;
		textbox_bottom.TextChanged += OnMarginChange;
		pagePreview.Location = new Point(130, 10);
		pagePreview.Name = "pagePreview";
		pagePreview.Size = new Size(150, 150);
		pagePreview.TabIndex = 6;
		form.AcceptButton = button_ok;
		form.AutoScaleBaseSize = new Size(5, 13);
		form.CancelButton = button_cancel;
		form.ClientSize = new Size(360, 390);
		form.Controls.Add(pagePreview);
		form.Controls.Add(button_printer);
		form.Controls.Add(button_cancel);
		form.Controls.Add(button_ok);
		form.Controls.Add(groupbox_margin);
		form.Controls.Add(groupbox_orientation);
		form.Controls.Add(groupbox_paper);
		form.FormBorderStyle = FormBorderStyle.FixedDialog;
		form.HelpButton = true;
		form.MaximizeBox = false;
		form.MinimizeBox = false;
		form.Name = "Form3";
		form.ShowInTaskbar = false;
		form.Text = "Page Setup";
		groupbox_paper.ResumeLayout(performLayout: false);
		groupbox_orientation.ResumeLayout(performLayout: false);
		groupbox_margin.ResumeLayout(performLayout: false);
		form.ResumeLayout(performLayout: false);
	}

	private double ToLocalizedLength(int marginsUnit)
	{
		return (!UseYardPound) ? PrinterUnitConvert.Convert(marginsUnit, PrinterUnit.ThousandthsOfAnInch, PrinterUnit.TenthsOfAMillimeter) : PrinterUnitConvert.Convert(marginsUnit, PrinterUnit.ThousandthsOfAnInch, PrinterUnit.Display);
	}

	private int FromLocalizedLength(double marginsUnit)
	{
		return (int)((!UseYardPound) ? PrinterUnitConvert.Convert(marginsUnit, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.ThousandthsOfAnInch) : PrinterUnitConvert.Convert(marginsUnit, PrinterUnit.Display, PrinterUnit.ThousandthsOfAnInch));
	}

	private string LocalizedLengthUnit()
	{
		return (!UseYardPound) ? "Margins (millimeters)" : "Margins (inches)";
	}

	private void SetPrinterDetails()
	{
		if (PageSettings == null)
		{
			throw new ArgumentException("PageSettings");
		}
		combobox_size.Items.Clear();
		foreach (PaperSize paperSize in InternalPrinterSettings.PaperSizes)
		{
			combobox_size.Items.Add(paperSize.PaperName);
		}
		combobox_size.SelectedItem = page_settings.PaperSize.PaperName;
		combobox_source.Items.Clear();
		foreach (PaperSource paperSource in InternalPrinterSettings.PaperSources)
		{
			combobox_source.Items.Add(paperSource.SourceName);
		}
		combobox_source.SelectedItem = page_settings.PaperSource.SourceName;
		if (PageSettings.Landscape)
		{
			radio_landscape.Checked = true;
		}
		else
		{
			radio_portrait.Checked = true;
		}
		if (ShowHelp)
		{
			ShowHelpButton();
		}
		Margins margins = PageSettings.Margins;
		Margins minMargins = MinMargins;
		textbox_top.Text = ToLocalizedLength(margins.Top).ToString();
		textbox_bottom.Text = ToLocalizedLength(margins.Bottom).ToString();
		textbox_left.Text = ToLocalizedLength(margins.Left).ToString();
		textbox_right.Text = ToLocalizedLength(margins.Right).ToString();
		textbox_top.Min = ToLocalizedLength(minMargins.Top);
		textbox_bottom.Min = ToLocalizedLength(minMargins.Bottom);
		textbox_left.Min = ToLocalizedLength(minMargins.Left);
		textbox_right.Min = ToLocalizedLength(minMargins.Right);
		button_printer.Enabled = AllowPrinter && PrinterSettings != null;
		groupbox_orientation.Enabled = AllowOrientation;
		groupbox_paper.Enabled = AllowPaper;
		groupbox_margin.Enabled = AllowMargins;
		pagePreview.Setup(PageSettings);
	}

	private void OnClickOkButton(object sender, EventArgs e)
	{
		if (combobox_size.SelectedItem != null)
		{
			foreach (PaperSize paperSize in InternalPrinterSettings.PaperSizes)
			{
				if (paperSize.PaperName == (string)combobox_size.SelectedItem)
				{
					PageSettings.PaperSize = paperSize;
					break;
				}
			}
		}
		if (combobox_source.SelectedItem != null)
		{
			foreach (PaperSource paperSource in InternalPrinterSettings.PaperSources)
			{
				if (paperSource.SourceName == (string)combobox_source.SelectedItem)
				{
					PageSettings.PaperSource = paperSource;
					break;
				}
			}
		}
		Margins margins = new Margins();
		margins.Top = FromLocalizedLength(textbox_top.Value);
		margins.Bottom = FromLocalizedLength(textbox_bottom.Value);
		margins.Left = FromLocalizedLength(textbox_left.Value);
		margins.Right = FromLocalizedLength(textbox_right.Value);
		PageSettings.Margins = margins;
		PageSettings.Landscape = radio_landscape.Checked;
		form.DialogResult = DialogResult.OK;
	}

	private void ShowHelpButton()
	{
		if (button_help == null)
		{
			button_help = new Button();
			button_help.Location = new Point(12, 358);
			button_help.Name = "button_help";
			button_help.Size = new Size(72, 23);
			button_help.Text = "&Help";
			form.Controls.Add(button_help);
		}
		button_help.Visible = show_help;
	}

	private void OnClickPrinterButton(object sender, EventArgs args)
	{
		PrinterForm printerForm = new PrinterForm(this);
		printerForm.UpdateValues();
		if (printerForm.ShowDialog() == DialogResult.OK && printerForm.SelectedPrinter != PrinterSettings.PrinterName)
		{
			PrinterSettings.PrinterName = printerForm.SelectedPrinter;
		}
		PageSettings = PrinterSettings.DefaultPageSettings;
		SetPrinterDetails();
		button_ok.Select();
		printerForm.Dispose();
	}

	private void OnPaperSizeChange(object sender, EventArgs e)
	{
		if (combobox_size.SelectedItem == null)
		{
			return;
		}
		foreach (PaperSize paperSize in InternalPrinterSettings.PaperSizes)
		{
			if (paperSize.PaperName == (string)combobox_size.SelectedItem)
			{
				pagePreview.SetSize(paperSize.Width, paperSize.Height);
				break;
			}
		}
	}

	private void OnMarginChange(object sender, EventArgs e)
	{
		pagePreview.SetMargins(FromLocalizedLength(textbox_left.Value), FromLocalizedLength(textbox_right.Value), FromLocalizedLength(textbox_top.Value), FromLocalizedLength(textbox_bottom.Value));
	}

	private void OnLandscapeChange(object sender, EventArgs e)
	{
		pagePreview.Landscape = radio_landscape.Checked;
	}
}
