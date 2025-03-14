using System.ComponentModel;

namespace System.Drawing.Printing;

[ToolboxItemFilter("System.Drawing.Printing", ToolboxItemFilterType.Allow)]
[DefaultProperty("DocumentName")]
[DefaultEvent("PrintPage")]
public class PrintDocument : Component
{
	private PageSettings defaultpagesettings;

	private PrinterSettings printersettings;

	private PrintController printcontroller;

	private string documentname;

	private bool originAtMargins;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[SRDescription("The settings for the current page.")]
	[Browsable(false)]
	public PageSettings DefaultPageSettings
	{
		get
		{
			return defaultpagesettings;
		}
		set
		{
			defaultpagesettings = value;
		}
	}

	[SRDescription("The name of the document.")]
	[DefaultValue("document")]
	public string DocumentName
	{
		get
		{
			return documentname;
		}
		set
		{
			documentname = value;
		}
	}

	[Browsable(false)]
	[SRDescription("The print controller object.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public PrintController PrintController
	{
		get
		{
			return printcontroller;
		}
		set
		{
			printcontroller = value;
		}
	}

	[SRDescription("The current settings for the active printer.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public PrinterSettings PrinterSettings
	{
		get
		{
			return printersettings;
		}
		set
		{
			printersettings = ((value != null) ? value : new PrinterSettings());
		}
	}

	[DefaultValue(false)]
	[SRDescription("Determines if the origin is set at the specified margins.")]
	public bool OriginAtMargins
	{
		get
		{
			return originAtMargins;
		}
		set
		{
			originAtMargins = value;
		}
	}

	[SRDescription("Raised when printing begins")]
	public event PrintEventHandler BeginPrint;

	[SRDescription("Raised when printing ends")]
	public event PrintEventHandler EndPrint;

	[SRDescription("Raised when printing of a new page begins")]
	public event PrintPageEventHandler PrintPage;

	[SRDescription("Raised before printing of a new page begins")]
	public event QueryPageSettingsEventHandler QueryPageSettings;

	public PrintDocument()
	{
		documentname = "document";
		printersettings = new PrinterSettings();
		defaultpagesettings = (PageSettings)printersettings.DefaultPageSettings.Clone();
		printcontroller = new StandardPrintController();
	}

	public void Print()
	{
		PrintEventArgs printEventArgs = new PrintEventArgs();
		OnBeginPrint(printEventArgs);
		if (printEventArgs.Cancel)
		{
			return;
		}
		PrintController.OnStartPrint(this, printEventArgs);
		if (printEventArgs.Cancel)
		{
			return;
		}
		Graphics graphics = null;
		if (printEventArgs.GraphicsContext != null)
		{
			graphics = Graphics.FromHdc(printEventArgs.GraphicsContext.Hdc);
			printEventArgs.GraphicsContext.Graphics = graphics;
		}
		PrintPageEventArgs printPageEventArgs;
		do
		{
			QueryPageSettingsEventArgs queryPageSettingsEventArgs = new QueryPageSettingsEventArgs(DefaultPageSettings.Clone() as PageSettings);
			OnQueryPageSettings(queryPageSettingsEventArgs);
			PageSettings pageSettings = queryPageSettingsEventArgs.PageSettings;
			printPageEventArgs = new PrintPageEventArgs(graphics, pageSettings.Bounds, new Rectangle(0, 0, pageSettings.PaperSize.Width, pageSettings.PaperSize.Height), pageSettings);
			printPageEventArgs.GraphicsContext = printEventArgs.GraphicsContext;
			Graphics graphics2 = PrintController.OnStartPage(this, printPageEventArgs);
			printPageEventArgs.SetGraphics(graphics2);
			if (!printPageEventArgs.Cancel)
			{
				OnPrintPage(printPageEventArgs);
			}
			PrintController.OnEndPage(this, printPageEventArgs);
		}
		while (!printPageEventArgs.Cancel && printPageEventArgs.HasMorePages);
		OnEndPrint(printEventArgs);
		PrintController.OnEndPrint(this, printEventArgs);
	}

	public override string ToString()
	{
		return "[PrintDocument " + DocumentName + "]";
	}

	protected virtual void OnBeginPrint(PrintEventArgs e)
	{
		if (this.BeginPrint != null)
		{
			this.BeginPrint(this, e);
		}
	}

	protected virtual void OnEndPrint(PrintEventArgs e)
	{
		if (this.EndPrint != null)
		{
			this.EndPrint(this, e);
		}
	}

	protected virtual void OnPrintPage(PrintPageEventArgs e)
	{
		if (this.PrintPage != null)
		{
			this.PrintPage(this, e);
		}
	}

	protected virtual void OnQueryPageSettings(QueryPageSettingsEventArgs e)
	{
		if (this.QueryPageSettings != null)
		{
			this.QueryPageSettings(this, e);
		}
	}
}
