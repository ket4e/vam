using System.Drawing;
using System.Drawing.Printing;

namespace System.Windows.Forms;

public class PrintControllerWithStatusDialog : PrintController
{
	private class PrintingDialog : Form
	{
		private Button buttonCancel;

		private Label label;

		public string LabelText
		{
			get
			{
				return label.Text;
			}
			set
			{
				label.Text = value;
			}
		}

		public PrintingDialog()
		{
			buttonCancel = new Button();
			label = new Label();
			SuspendLayout();
			buttonCancel.Location = new Point(88, 88);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.TabIndex = 0;
			buttonCancel.Text = "Cancel";
			label.Location = new Point(0, 40);
			label.Name = "label";
			label.Size = new Size(257, 23);
			label.TabIndex = 1;
			label.Text = "Page 1 of document";
			label.TextAlign = ContentAlignment.MiddleCenter;
			AutoScaleBaseSize = new Size(5, 13);
			base.CancelButton = buttonCancel;
			base.ClientSize = new Size(258, 124);
			base.ControlBox = false;
			base.Controls.Add(label);
			base.Controls.Add(buttonCancel);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.Name = "PrintingDialog";
			base.ShowInTaskbar = false;
			Text = "Printing";
			ResumeLayout(performLayout: false);
		}
	}

	private PrintController underlyingController;

	private PrintingDialog dialog;

	private int currentPage;

	public override bool IsPreview => underlyingController.IsPreview;

	public PrintControllerWithStatusDialog(PrintController underlyingController)
	{
		this.underlyingController = underlyingController;
		dialog = new PrintingDialog();
		dialog.Text = "Printing";
	}

	public PrintControllerWithStatusDialog(PrintController underlyingController, string dialogTitle)
		: this(underlyingController)
	{
		dialog.Text = dialogTitle;
	}

	public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
	{
		if (dialog.DialogResult == DialogResult.Cancel)
		{
			e.Cancel = true;
			dialog.Hide();
		}
		else
		{
			underlyingController.OnEndPage(document, e);
		}
	}

	public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
	{
		dialog.Hide();
		underlyingController.OnEndPrint(document, e);
	}

	public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
	{
		if (dialog.DialogResult == DialogResult.Cancel)
		{
			e.Cancel = true;
			dialog.Hide();
			return null;
		}
		dialog.LabelText = $"Page {++currentPage} of document";
		return underlyingController.OnStartPage(document, e);
	}

	private void Set_PrinterSettings_PrintFileName(PrinterSettings settings, string filename)
	{
		settings.PrintFileName = filename;
	}

	public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
	{
		try
		{
			currentPage = 0;
			dialog.Show();
			if (document.PrinterSettings.PrintToFile)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				if (saveFileDialog.ShowDialog() != DialogResult.OK)
				{
					throw new Exception("The operation was canceled by the user");
				}
				Set_PrinterSettings_PrintFileName(document.PrinterSettings, saveFileDialog.FileName);
			}
			underlyingController.OnStartPrint(document, e);
		}
		catch
		{
			dialog.Hide();
			throw;
		}
	}
}
