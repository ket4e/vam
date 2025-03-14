using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;

namespace System.Windows.Forms;

[DefaultProperty("Document")]
[Designer("System.Windows.Forms.Design.PrintDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
public sealed class PrintDialog : CommonDialog
{
	private class CollatePreview : UserControl
	{
		private bool collate;

		private new Font font;

		public bool Collate
		{
			get
			{
				return collate;
			}
			set
			{
				if (collate != value)
				{
					collate = value;
					Invalidate();
				}
			}
		}

		public CollatePreview()
		{
			font = new Font(FontFamily.GenericSansSerif, 10f);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (collate)
			{
				DrawCollate(e.Graphics);
			}
			else
			{
				DrawNoCollate(e.Graphics);
			}
			base.OnPaint(e);
		}

		private void DrawCollate(Graphics g)
		{
			int num = 0;
			int num2 = 12;
			int num3 = 14;
			int num4 = 6;
			int num5 = 26;
			int num6 = 0;
			for (int i = 0; i < 2; i++)
			{
				g.FillRectangle(Brushes.White, num5 + i * 18, num6, 18, 24);
				ControlPaint.DrawBorder(g, new Rectangle(num5 + i * 18, num6, 18, 24), SystemColors.ControlDark, ButtonBorderStyle.Solid);
				g.DrawString((i + 1).ToString(), font, SystemBrushes.ControlDarkDark, num5 + i * 18 + 5, num6 + 5, StringFormat.GenericTypographic);
				g.FillRectangle(Brushes.White, num3 + i * 18, num4, 18, 24);
				ControlPaint.DrawBorder(g, new Rectangle(num3 + i * 18, num4, 18, 24), SystemColors.ControlDark, ButtonBorderStyle.Solid);
				g.DrawString((i + 1).ToString(), font, SystemBrushes.ControlDarkDark, num3 + i * 18 + 5, num4 + 5, StringFormat.GenericTypographic);
				g.FillRectangle(Brushes.White, num + i * 18, num2, 18, 24);
				ControlPaint.DrawBorder(g, new Rectangle(num + i * 18, num2, 18, 24), SystemColors.ControlDark, ButtonBorderStyle.Solid);
				g.DrawString((i + 1).ToString(), font, SystemBrushes.ControlDarkDark, num + i * 18 + 5, num2 + 5, StringFormat.GenericTypographic);
				num += 28;
				num3 += 28;
				num5 += 28;
			}
		}

		private void DrawNoCollate(Graphics g)
		{
			int num = 0;
			int num2 = 12;
			int num3 = 13;
			int num4 = 4;
			for (int i = 0; i < 3; i++)
			{
				g.FillRectangle(Brushes.White, num3 + i * 18, num4, 18, 24);
				ControlPaint.DrawBorder(g, new Rectangle(num3 + i * 18, num4, 18, 24), SystemColors.ControlDark, ButtonBorderStyle.Solid);
				g.DrawString((i + 1).ToString(), font, SystemBrushes.ControlDarkDark, num3 + i * 18 + 5, num4 + 5, StringFormat.GenericTypographic);
				g.FillRectangle(Brushes.White, num + i * 18, num2, 18, 24);
				ControlPaint.DrawBorder(g, new Rectangle(num + i * 18, num2, 18, 24), SystemColors.ControlDark, ButtonBorderStyle.Solid);
				g.DrawString((i + 1).ToString(), font, SystemBrushes.ControlDarkDark, num + i * 18 + 5, num2 + 5, StringFormat.GenericTypographic);
				num += 15;
				num3 += 15;
			}
		}
	}

	private PrintDocument document;

	private bool allow_current_page;

	private bool allow_print_to_file;

	private bool allow_selection;

	private bool allow_some_pages;

	private bool show_help;

	private bool show_network;

	private bool print_to_file;

	private PrinterSettings current_settings;

	private Button cancel_button;

	private Button accept_button;

	private Button help_button;

	private ComboBox printer_combo;

	private RadioButton radio_all;

	private RadioButton radio_pages;

	private RadioButton radio_sel;

	private PrinterSettings.StringCollection installed_printers;

	private PrinterSettings default_printer_settings;

	private TextBox txtFrom;

	private TextBox txtTo;

	private Label labelTo;

	private Label labelFrom;

	private CheckBox chkbox_print;

	private NumericUpDown updown_copies;

	private CheckBox chkbox_collate;

	private Label label_status;

	private Label label_type;

	private Label label_where;

	private Label label_comment;

	private CollatePreview collate;

	private bool use_ex_dialog;

	[DefaultValue(false)]
	public bool AllowCurrentPage
	{
		get
		{
			return allow_current_page;
		}
		set
		{
			allow_current_page = value;
			radio_pages.Enabled = value;
		}
	}

	[DefaultValue(true)]
	public bool AllowPrintToFile
	{
		get
		{
			return allow_print_to_file;
		}
		set
		{
			allow_print_to_file = value;
			chkbox_print.Enabled = value;
		}
	}

	[DefaultValue(false)]
	public bool AllowSelection
	{
		get
		{
			return allow_selection;
		}
		set
		{
			allow_selection = value;
			radio_sel.Enabled = value;
		}
	}

	[DefaultValue(false)]
	public bool AllowSomePages
	{
		get
		{
			return allow_some_pages;
		}
		set
		{
			allow_some_pages = value;
			radio_pages.Enabled = value;
			txtFrom.Enabled = value;
			txtTo.Enabled = value;
			labelTo.Enabled = value;
			labelFrom.Enabled = value;
			if (PrinterSettings != null)
			{
				txtFrom.Text = PrinterSettings.FromPage.ToString();
				txtTo.Text = PrinterSettings.ToPage.ToString();
			}
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
			current_settings = ((value != null) ? value.PrinterSettings : new PrinterSettings());
		}
	}

	[DefaultValue(null)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public PrinterSettings PrinterSettings
	{
		get
		{
			if (current_settings == null)
			{
				current_settings = new PrinterSettings();
			}
			return current_settings;
		}
		set
		{
			if (value == null || value != current_settings)
			{
				current_settings = ((value != null) ? value : new PrinterSettings());
				document = null;
			}
		}
	}

	[DefaultValue(false)]
	public bool PrintToFile
	{
		get
		{
			return print_to_file;
		}
		set
		{
			print_to_file = value;
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

	[DefaultValue(false)]
	public bool ShowHelp
	{
		get
		{
			return show_help;
		}
		set
		{
			show_help = value;
			ShowHelpButton();
		}
	}

	[DefaultValue(false)]
	[System.MonoTODO("Stub, not implemented, will always use default dialog")]
	public bool UseEXDialog
	{
		get
		{
			return use_ex_dialog;
		}
		set
		{
			use_ex_dialog = value;
		}
	}

	public PrintDialog()
	{
		form = new DialogForm(this);
		help_button = null;
		installed_printers = PrinterSettings.InstalledPrinters;
		form.Text = "Print";
		CreateFormControls();
		Reset();
	}

	public override void Reset()
	{
		current_settings = null;
		AllowPrintToFile = true;
		AllowSelection = false;
		AllowSomePages = false;
		PrintToFile = false;
		ShowHelp = false;
		ShowNetwork = true;
	}

	protected override bool RunDialog(IntPtr hwndOwner)
	{
		if (allow_some_pages && PrinterSettings.FromPage > PrinterSettings.ToPage)
		{
			throw new ArgumentException("FromPage out of range");
		}
		if (allow_some_pages)
		{
			txtFrom.Text = PrinterSettings.FromPage.ToString();
			txtTo.Text = PrinterSettings.ToPage.ToString();
		}
		if (PrinterSettings.PrintRange == PrintRange.SomePages && allow_some_pages)
		{
			radio_pages.Checked = true;
		}
		else if (PrinterSettings.PrintRange == PrintRange.Selection && allow_selection)
		{
			radio_sel.Checked = true;
		}
		else
		{
			radio_all.Checked = true;
		}
		updown_copies.Value = ((PrinterSettings.Copies == 0) ? 1 : PrinterSettings.Copies);
		chkbox_collate.Checked = PrinterSettings.Collate;
		chkbox_collate.Enabled = ((updown_copies.Value > 1m) ? true : false);
		if (show_help)
		{
			ShowHelpButton();
		}
		SetPrinterDetails();
		return true;
	}

	private void OnClickCancelButton(object sender, EventArgs e)
	{
		form.DialogResult = DialogResult.Cancel;
	}

	private void ShowErrorMessage(string message, Control control_to_focus)
	{
		MessageBox.Show(message, "Print", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		control_to_focus?.Focus();
	}

	private void OnClickOkButton(object sender, EventArgs e)
	{
		if (updown_copies.Text.Length < 1)
		{
			ShowErrorMessage("The 'Copies' value cannot be empty and must be a positive value.", updown_copies);
			return;
		}
		int num = -1;
		int num2 = -1;
		if (allow_some_pages && radio_pages.Checked)
		{
			if (txtFrom.Text.Length < 1)
			{
				ShowErrorMessage("The 'From' value cannot be empty and must be a positive value.", txtFrom);
				return;
			}
			try
			{
				num = int.Parse(txtFrom.Text);
				num2 = int.Parse(txtTo.Text);
			}
			catch
			{
				ShowErrorMessage("From/To values should be numeric", txtFrom);
				return;
			}
			if (num > num2)
			{
				ShowErrorMessage("'From' value cannot be greater than 'To' value.", txtFrom);
				return;
			}
			if (num2 < PrinterSettings.MinimumPage || num2 > PrinterSettings.MaximumPage)
			{
				ShowErrorMessage("'To' value is not within the page range\nEnter a number between " + PrinterSettings.MinimumPage + " and " + PrinterSettings.MaximumPage + ".", txtTo);
				return;
			}
			if (num < PrinterSettings.MinimumPage || num > PrinterSettings.MaximumPage)
			{
				ShowErrorMessage("'From' value is not within the page range\nEnter a number between " + PrinterSettings.MinimumPage + " and " + PrinterSettings.MaximumPage + ".", txtFrom);
				return;
			}
		}
		if (radio_all.Checked)
		{
			PrinterSettings.PrintRange = PrintRange.AllPages;
		}
		else if (radio_pages.Checked)
		{
			PrinterSettings.PrintRange = PrintRange.SomePages;
		}
		else
		{
			PrinterSettings.PrintRange = PrintRange.Selection;
		}
		PrinterSettings.Copies = (short)updown_copies.Value;
		if (PrinterSettings.PrintRange == PrintRange.SomePages)
		{
			PrinterSettings.FromPage = num;
			PrinterSettings.ToPage = num2;
		}
		PrinterSettings.Collate = chkbox_collate.Checked;
		if (allow_print_to_file)
		{
			PrinterSettings.PrintToFile = chkbox_print.Checked;
		}
		form.DialogResult = DialogResult.OK;
		if (printer_combo.SelectedItem != null)
		{
			PrinterSettings.PrinterName = (string)printer_combo.SelectedItem;
		}
		if (document != null)
		{
			document.PrintController = new PrintControllerWithStatusDialog(document.PrintController);
			document.PrinterSettings = PrinterSettings;
		}
	}

	private void ShowHelpButton()
	{
		if (help_button == null)
		{
			help_button = new Button();
			help_button.TabIndex = 60;
			help_button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			help_button.FlatStyle = FlatStyle.System;
			help_button.Location = new Point(20, 270);
			help_button.Text = "&Help";
			help_button.FlatStyle = FlatStyle.System;
			form.Controls.Add(help_button);
		}
		help_button.Visible = show_help;
	}

	private void OnUpDownValueChanged(object sender, EventArgs e)
	{
		chkbox_collate.Enabled = ((updown_copies.Value > 1m) ? true : false);
	}

	private void OnPagesCheckedChanged(object obj, EventArgs args)
	{
		if (radio_pages.Checked && !txtTo.Focused)
		{
			txtFrom.Focus();
		}
	}

	private void CreateFormControls()
	{
		form.SuspendLayout();
		GroupBox groupBox = new GroupBox();
		groupBox.Location = new Point(10, 8);
		groupBox.Text = "Printer";
		groupBox.Size = new Size(420, 145);
		GroupBox groupBox2 = new GroupBox();
		groupBox2.Location = new Point(10, 155);
		groupBox2.Text = "Print range";
		groupBox2.Size = new Size(240, 100);
		GroupBox groupBox3 = new GroupBox();
		groupBox3.Location = new Point(265, 155);
		groupBox3.Text = "Copies";
		groupBox3.Size = new Size(165, 100);
		accept_button = new Button();
		form.AcceptButton = accept_button;
		accept_button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		accept_button.FlatStyle = FlatStyle.System;
		accept_button.Location = new Point(265, 270);
		accept_button.Text = "OK";
		accept_button.FlatStyle = FlatStyle.System;
		accept_button.Click += OnClickOkButton;
		cancel_button = new Button();
		form.CancelButton = cancel_button;
		cancel_button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		cancel_button.FlatStyle = FlatStyle.System;
		cancel_button.Location = new Point(350, 270);
		cancel_button.Text = "Cancel";
		cancel_button.FlatStyle = FlatStyle.System;
		cancel_button.Click += OnClickCancelButton;
		Label label = new Label();
		label.AutoSize = true;
		label.Text = "&Name:";
		label.Location = new Point(20, 33);
		groupBox.Controls.Add(label);
		label = new Label();
		label.Text = "Status:";
		label.AutoSize = true;
		label.Location = new Point(20, 60);
		groupBox.Controls.Add(label);
		label_status = new Label();
		label_status.AutoSize = true;
		label_status.Location = new Point(80, 60);
		groupBox.Controls.Add(label_status);
		label = new Label();
		label.Text = "Type:";
		label.AutoSize = true;
		label.Location = new Point(20, 80);
		groupBox.Controls.Add(label);
		label_type = new Label();
		label_type.AutoSize = true;
		label_type.Location = new Point(80, 80);
		groupBox.Controls.Add(label_type);
		label = new Label();
		label.Text = "Where:";
		label.AutoSize = true;
		label.Location = new Point(20, 100);
		groupBox.Controls.Add(label);
		label_where = new Label();
		label_where.AutoSize = true;
		label_where.Location = new Point(80, 100);
		groupBox.Controls.Add(label_where);
		label = new Label();
		label.Text = "Comment:";
		label.AutoSize = true;
		label.Location = new Point(20, 120);
		groupBox.Controls.Add(label);
		label_comment = new Label();
		label_comment.AutoSize = true;
		label_comment.Location = new Point(80, 120);
		groupBox.Controls.Add(label_comment);
		radio_all = new RadioButton();
		radio_all.TabIndex = 21;
		radio_all.Location = new Point(20, 20);
		radio_all.Text = "&All";
		radio_all.Checked = true;
		groupBox2.Controls.Add(radio_all);
		radio_pages = new RadioButton();
		radio_pages.TabIndex = 22;
		radio_pages.Location = new Point(20, 46);
		radio_pages.Text = "Pa&ges";
		radio_pages.Width = 60;
		radio_pages.CheckedChanged += OnPagesCheckedChanged;
		groupBox2.Controls.Add(radio_pages);
		radio_sel = new RadioButton();
		radio_sel.TabIndex = 23;
		radio_sel.Location = new Point(20, 72);
		radio_sel.Text = "&Selection";
		groupBox2.Controls.Add(radio_sel);
		labelFrom = new Label();
		labelFrom.Text = "&from:";
		labelFrom.TabIndex = 24;
		labelFrom.AutoSize = true;
		labelFrom.Location = new Point(80, 50);
		groupBox2.Controls.Add(labelFrom);
		txtFrom = new TextBox();
		txtFrom.TabIndex = 25;
		txtFrom.Location = new Point(120, 50);
		txtFrom.Width = 40;
		txtFrom.TextChanged += OnPagesTextChanged;
		groupBox2.Controls.Add(txtFrom);
		labelTo = new Label();
		labelTo.Text = "&to:";
		labelTo.TabIndex = 26;
		labelTo.AutoSize = true;
		labelTo.Location = new Point(170, 50);
		groupBox2.Controls.Add(labelTo);
		txtTo = new TextBox();
		txtTo.TabIndex = 27;
		txtTo.Location = new Point(190, 50);
		txtTo.Width = 40;
		txtTo.TextChanged += OnPagesTextChanged;
		groupBox2.Controls.Add(txtTo);
		chkbox_print = new CheckBox();
		chkbox_print.Location = new Point(305, 115);
		chkbox_print.Text = "Print to fil&e";
		updown_copies = new NumericUpDown();
		updown_copies.TabIndex = 31;
		updown_copies.Location = new Point(105, 18);
		updown_copies.Minimum = 1m;
		groupBox3.Controls.Add(updown_copies);
		updown_copies.ValueChanged += OnUpDownValueChanged;
		updown_copies.Size = new Size(40, 20);
		label = new Label();
		label.Text = "Number of &copies:";
		label.AutoSize = true;
		label.Location = new Point(10, 20);
		groupBox3.Controls.Add(label);
		chkbox_collate = new CheckBox();
		chkbox_collate.TabIndex = 32;
		chkbox_collate.Location = new Point(105, 55);
		chkbox_collate.Text = "C&ollate";
		chkbox_collate.Width = 58;
		chkbox_collate.CheckedChanged += chkbox_collate_CheckedChanged;
		groupBox3.Controls.Add(chkbox_collate);
		collate = new CollatePreview();
		collate.Location = new Point(6, 50);
		collate.Size = new Size(100, 45);
		groupBox3.Controls.Add(collate);
		printer_combo = new ComboBox();
		printer_combo.DropDownStyle = ComboBoxStyle.DropDownList;
		printer_combo.Location = new Point(80, 32);
		printer_combo.Width = 220;
		printer_combo.SelectedIndexChanged += OnPrinterSelectedIndexChanged;
		default_printer_settings = new PrinterSettings();
		for (int i = 0; i < installed_printers.Count; i++)
		{
			printer_combo.Items.Add(installed_printers[i]);
			if (installed_printers[i] == default_printer_settings.PrinterName)
			{
				printer_combo.SelectedItem = installed_printers[i];
			}
		}
		printer_combo.TabIndex = 11;
		chkbox_print.TabIndex = 12;
		groupBox.Controls.Add(printer_combo);
		groupBox.Controls.Add(chkbox_print);
		form.Size = new Size(450, 327);
		form.FormBorderStyle = FormBorderStyle.FixedDialog;
		form.MaximizeBox = false;
		groupBox.TabIndex = 10;
		groupBox2.TabIndex = 20;
		groupBox3.TabIndex = 30;
		accept_button.TabIndex = 40;
		cancel_button.TabIndex = 50;
		form.Controls.Add(groupBox);
		form.Controls.Add(groupBox2);
		form.Controls.Add(groupBox3);
		form.Controls.Add(accept_button);
		form.Controls.Add(cancel_button);
		form.ResumeLayout(performLayout: false);
	}

	private void OnPagesTextChanged(object sender, EventArgs args)
	{
		radio_pages.Checked = true;
	}

	private void OnPrinterSelectedIndexChanged(object sender, EventArgs e)
	{
		SetPrinterDetails();
	}

	private void SetPrinterDetails()
	{
		try
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			Type type = Type.GetType("System.Drawing.Printing.SysPrn, System.Drawing");
			MethodInfo method = type.GetMethod("GetPrintDialogInfo", BindingFlags.Static | BindingFlags.NonPublic);
			string text5 = (string)printer_combo.SelectedItem;
			if (text5 != null)
			{
				object[] array = new object[5] { text5, text, text2, text3, text4 };
				method.Invoke(null, array);
				text = (string)array[1];
				text2 = (string)array[2];
				text3 = (string)array[3];
				text4 = (string)array[4];
			}
			label_status.Text = text3;
			label_type.Text = text2;
			label_where.Text = text;
			label_comment.Text = text4;
			accept_button.Enabled = true;
		}
		catch
		{
			accept_button.Enabled = false;
		}
	}

	private void chkbox_collate_CheckedChanged(object sender, EventArgs e)
	{
		collate.Collate = chkbox_collate.Checked;
	}
}
