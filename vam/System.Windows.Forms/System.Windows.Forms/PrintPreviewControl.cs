using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
[DefaultProperty("Document")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class PrintPreviewControl : Control
{
	private bool autozoom;

	private int columns;

	private int rows;

	private int startPage;

	private double zoom;

	private int padding = ThemeEngine.Current.PrintPreviewControlPadding;

	private PrintDocument document;

	internal PreviewPrintController controller;

	internal PreviewPageInfo[] page_infos;

	private VScrollBar vbar;

	private HScrollBar hbar;

	internal Rectangle ViewPort;

	internal Image[] image_cache;

	private Size image_size;

	private static object StartPageChangedEvent;

	internal int vbar_value;

	internal int hbar_value;

	[DefaultValue(true)]
	public bool AutoZoom
	{
		get
		{
			return autozoom;
		}
		set
		{
			autozoom = value;
			InvalidateLayout();
		}
	}

	[DefaultValue(1)]
	public int Columns
	{
		get
		{
			return columns;
		}
		set
		{
			columns = value;
			InvalidateLayout();
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
		}
	}

	[AmbientValue(RightToLeft.Inherit)]
	[Localizable(true)]
	public override RightToLeft RightToLeft
	{
		get
		{
			return base.RightToLeft;
		}
		set
		{
			base.RightToLeft = value;
		}
	}

	[DefaultValue(1)]
	public int Rows
	{
		get
		{
			return rows;
		}
		set
		{
			rows = value;
			InvalidateLayout();
		}
	}

	[DefaultValue(0)]
	public int StartPage
	{
		get
		{
			return startPage;
		}
		set
		{
			if (value < 1)
			{
				return;
			}
			if (document != null && value + (Rows + 1) * Columns > page_infos.Length + 1)
			{
				value = page_infos.Length + 1 - (Rows + 1) * Columns;
				if (value < 1)
				{
					value = 1;
				}
			}
			int num = StartPage;
			startPage = value;
			if (num != startPage)
			{
				InvalidateLayout();
				OnStartPageChanged(EventArgs.Empty);
			}
		}
	}

	[Bindable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[DefaultValue(false)]
	public bool UseAntiAlias
	{
		get
		{
			return controller.UseAntiAlias;
		}
		set
		{
			controller.UseAntiAlias = value;
		}
	}

	[DefaultValue(0.3)]
	public double Zoom
	{
		get
		{
			return zoom;
		}
		set
		{
			if (value <= 0.0)
			{
				throw new ArgumentException("zoom");
			}
			autozoom = false;
			zoom = value;
			InvalidateLayout();
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	internal ScrollBar UIAVScrollBar => vbar;

	internal ScrollBar UIAHScrollBar => hbar;

	public event EventHandler StartPageChanged
	{
		add
		{
			base.Events.AddHandler(StartPageChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(StartPageChangedEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler TextChanged
	{
		add
		{
			base.TextChanged += value;
		}
		remove
		{
			base.TextChanged -= value;
		}
	}

	public PrintPreviewControl()
	{
		autozoom = true;
		columns = 1;
		rows = 0;
		startPage = 1;
		BackColor = SystemColors.AppWorkspace;
		controller = new PreviewPrintController();
		vbar = new ImplicitVScrollBar();
		hbar = new ImplicitHScrollBar();
		vbar.Visible = false;
		hbar.Visible = false;
		vbar.ValueChanged += VScrollBarValueChanged;
		hbar.ValueChanged += HScrollBarValueChanged;
		SuspendLayout();
		base.Controls.AddImplicit(vbar);
		base.Controls.AddImplicit(hbar);
		ResumeLayout();
	}

	static PrintPreviewControl()
	{
		StartPageChanged = new object();
	}

	internal void GeneratePreview()
	{
		if (document == null)
		{
			return;
		}
		try
		{
			if (page_infos == null)
			{
				if (document.PrintController == null || !(document.PrintController is PrintControllerWithStatusDialog))
				{
					document.PrintController = new PrintControllerWithStatusDialog(controller);
				}
				document.Print();
				page_infos = controller.GetPreviewPageInfo();
			}
			if (image_cache == null)
			{
				image_cache = new Image[page_infos.Length];
				if (page_infos.Length > 0)
				{
					image_size = ThemeEngine.Current.PrintPreviewControlGetPageSize(this);
					if (image_size.Width >= 0 && image_size.Width < page_infos[0].Image.Width && image_size.Height >= 0 && image_size.Height < page_infos[0].Image.Height)
					{
						for (int i = 0; i < page_infos.Length; i++)
						{
							image_cache[i] = new Bitmap(image_size.Width, image_size.Height);
							Graphics graphics = Graphics.FromImage(image_cache[i]);
							graphics.DrawImage(page_infos[i].Image, new Rectangle(new Point(0, 0), image_size), 0, 0, page_infos[i].Image.Width, page_infos[i].Image.Height, GraphicsUnit.Pixel);
							graphics.Dispose();
						}
					}
				}
			}
			UpdateScrollBars();
		}
		catch (Exception ex)
		{
			page_infos = new PreviewPageInfo[0];
			image_cache = new Image[0];
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	public void InvalidatePreview()
	{
		if (page_infos != null)
		{
			for (int i = 0; i < page_infos.Length; i++)
			{
				if (page_infos[i].Image != null)
				{
					page_infos[i].Image.Dispose();
				}
			}
			page_infos = null;
		}
		InvalidateLayout();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override void ResetBackColor()
	{
		base.ResetBackColor();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override void ResetForeColor()
	{
		base.ResetForeColor();
	}

	protected override void OnPaint(PaintEventArgs pevent)
	{
		if (page_infos == null || image_cache == null)
		{
			GeneratePreview();
		}
		ThemeEngine.Current.PrintPreviewControlPaint(pevent, this, image_size);
	}

	protected override void OnResize(EventArgs eventargs)
	{
		InvalidateLayout();
		base.OnResize(eventargs);
	}

	protected virtual void OnStartPageChanged(EventArgs e)
	{
		((EventHandler)base.Events[StartPageChanged])?.Invoke(this, e);
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	private void VScrollBarValueChanged(object sender, EventArgs e)
	{
		int yAmount = ((vbar.Value <= vbar_value) ? (vbar_value - vbar.Value) : (-1 * (vbar.Value - vbar_value)));
		vbar_value = vbar.Value;
		XplatUI.ScrollWindow(Handle, ViewPort, 0, yAmount, with_children: false);
	}

	private void HScrollBarValueChanged(object sender, EventArgs e)
	{
		int xAmount = ((hbar.Value <= hbar_value) ? (hbar_value - hbar.Value) : (-1 * (hbar.Value - hbar_value)));
		hbar_value = hbar.Value;
		XplatUI.ScrollWindow(Handle, ViewPort, xAmount, 0, with_children: false);
	}

	private void UpdateScrollBars()
	{
		ViewPort = base.ClientRectangle;
		if (!AutoZoom)
		{
			int num = image_size.Width * Columns + (Columns + 1) * padding;
			int num2 = image_size.Height * (Rows + 1) + (Rows + 2) * padding;
			bool flag = false;
			bool flag2 = false;
			if (num > base.ClientRectangle.Width)
			{
				flag2 = true;
				ViewPort.Height -= hbar.Height;
			}
			if (num2 > ViewPort.Height)
			{
				flag = true;
				ViewPort.Width -= vbar.Width;
			}
			if (!flag2 && num > ViewPort.Width)
			{
				flag2 = true;
				ViewPort.Height -= hbar.Height;
			}
			SuspendLayout();
			if (flag)
			{
				vbar.SetValues(num2, ViewPort.Height);
				vbar.Bounds = new Rectangle(base.ClientRectangle.Width - vbar.Width, 0, vbar.Width, base.ClientRectangle.Height - (flag2 ? SystemInformation.VerticalScrollBarWidth : 0));
				vbar.Visible = true;
				vbar_value = vbar.Value;
			}
			else
			{
				vbar.Visible = false;
			}
			if (flag2)
			{
				hbar.SetValues(num, ViewPort.Width);
				hbar.Bounds = new Rectangle(0, base.ClientRectangle.Height - hbar.Height, base.ClientRectangle.Width - (flag ? SystemInformation.HorizontalScrollBarHeight : 0), hbar.Height);
				hbar.Visible = true;
				hbar_value = hbar.Value;
			}
			else
			{
				hbar.Visible = false;
			}
			ResumeLayout(performLayout: false);
		}
	}

	private void InvalidateLayout()
	{
		if (image_cache != null)
		{
			for (int i = 0; i < image_cache.Length; i++)
			{
				if (image_cache[i] != null)
				{
					image_cache[i].Dispose();
				}
			}
			image_cache = null;
		}
		Invalidate();
	}
}
