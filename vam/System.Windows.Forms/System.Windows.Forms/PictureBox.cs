using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
[DefaultProperty("Image")]
[Designer("System.Windows.Forms.Design.PictureBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[Docking(DockingBehavior.Ask)]
[DefaultBindingProperty("Image")]
public class PictureBox : Control, ISupportInitialize
{
	private Image image;

	private PictureBoxSizeMode size_mode;

	private Image error_image;

	private string image_location;

	private Image initial_image;

	private bool wait_on_load;

	private WebClient image_download;

	private bool image_from_url;

	private int no_update;

	private EventHandler frame_handler;

	private static object LoadCompletedEvent;

	private static object LoadProgressChangedEvent;

	private static object SizeModeChangedEvent;

	[DefaultValue(PictureBoxSizeMode.Normal)]
	[RefreshProperties(RefreshProperties.Repaint)]
	[Localizable(true)]
	public PictureBoxSizeMode SizeMode
	{
		get
		{
			return size_mode;
		}
		set
		{
			if (size_mode != value)
			{
				size_mode = value;
				if (size_mode == PictureBoxSizeMode.AutoSize)
				{
					AutoSize = true;
					SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
				}
				else
				{
					AutoSize = false;
					SetAutoSizeMode(AutoSizeMode.GrowOnly);
				}
				UpdateSize();
				if (no_update == 0)
				{
					Invalidate();
				}
				OnSizeModeChanged(EventArgs.Empty);
			}
		}
	}

	[Localizable(true)]
	[Bindable(true)]
	public Image Image
	{
		get
		{
			return image;
		}
		set
		{
			ChangeImage(value, from_url: false);
		}
	}

	[DispId(-504)]
	[DefaultValue(BorderStyle.None)]
	public BorderStyle BorderStyle
	{
		get
		{
			return base.InternalBorderStyle;
		}
		set
		{
			base.InternalBorderStyle = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool CausesValidation
	{
		get
		{
			return base.CausesValidation;
		}
		set
		{
			base.CausesValidation = value;
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	[Localizable(true)]
	public Image ErrorImage
	{
		get
		{
			return error_image;
		}
		set
		{
			error_image = value;
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	[Localizable(true)]
	public Image InitialImage
	{
		get
		{
			return initial_image;
		}
		set
		{
			initial_image = value;
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	[DefaultValue(null)]
	[Localizable(true)]
	public string ImageLocation
	{
		get
		{
			return image_location;
		}
		set
		{
			image_location = value;
			if (!string.IsNullOrEmpty(value))
			{
				if (WaitOnLoad)
				{
					Load(value);
				}
				else
				{
					LoadAsync(value);
				}
			}
			else if (image_from_url)
			{
				ChangeImage(null, from_url: true);
			}
		}
	}

	[DefaultValue(false)]
	[Localizable(true)]
	public bool WaitOnLoad
	{
		get
		{
			return wait_on_load;
		}
		set
		{
			wait_on_load = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ImeMode ImeMode
	{
		get
		{
			return base.ImeMode;
		}
		set
		{
			base.ImeMode = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new int TabIndex
	{
		get
		{
			return base.TabIndex;
		}
		set
		{
			base.TabIndex = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool TabStop
	{
		get
		{
			return base.TabStop;
		}
		set
		{
			base.TabStop = value;
		}
	}

	[Browsable(false)]
	[Bindable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	protected override CreateParams CreateParams => base.CreateParams;

	protected override ImeMode DefaultImeMode => base.DefaultImeMode;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Font Font
	{
		get
		{
			return base.Font;
		}
		set
		{
			base.Font = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Color ForeColor
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

	[EditorBrowsable(EditorBrowsableState.Never)]
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

	protected override Size DefaultSize => ThemeEngine.Current.PictureBoxDefaultSize;

	private WebClient ImageDownload
	{
		get
		{
			if (image_download == null)
			{
				image_download = new WebClient();
			}
			return image_download;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler CausesValidationChanged
	{
		add
		{
			base.CausesValidationChanged += value;
		}
		remove
		{
			base.CausesValidationChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler Enter
	{
		add
		{
			base.Enter += value;
		}
		remove
		{
			base.Enter -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler FontChanged
	{
		add
		{
			base.FontChanged += value;
		}
		remove
		{
			base.FontChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler ForeColorChanged
	{
		add
		{
			base.ForeColorChanged += value;
		}
		remove
		{
			base.ForeColorChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler ImeModeChanged
	{
		add
		{
			base.ImeModeChanged += value;
		}
		remove
		{
			base.ImeModeChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event KeyEventHandler KeyDown
	{
		add
		{
			base.KeyDown += value;
		}
		remove
		{
			base.KeyDown -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event KeyPressEventHandler KeyPress
	{
		add
		{
			base.KeyPress += value;
		}
		remove
		{
			base.KeyPress -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event KeyEventHandler KeyUp
	{
		add
		{
			base.KeyUp += value;
		}
		remove
		{
			base.KeyUp -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler Leave
	{
		add
		{
			base.Leave += value;
		}
		remove
		{
			base.Leave -= value;
		}
	}

	public event AsyncCompletedEventHandler LoadCompleted
	{
		add
		{
			base.Events.AddHandler(LoadCompletedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(LoadCompletedEvent, value);
		}
	}

	public event ProgressChangedEventHandler LoadProgressChanged
	{
		add
		{
			base.Events.AddHandler(LoadProgressChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(LoadProgressChangedEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler RightToLeftChanged
	{
		add
		{
			base.RightToLeftChanged += value;
		}
		remove
		{
			base.RightToLeftChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler TabIndexChanged
	{
		add
		{
			base.TabIndexChanged += value;
		}
		remove
		{
			base.TabIndexChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler TabStopChanged
	{
		add
		{
			base.TabStopChanged += value;
		}
		remove
		{
			base.TabStopChanged -= value;
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

	public event EventHandler SizeModeChanged
	{
		add
		{
			base.Events.AddHandler(SizeModeChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SizeModeChangedEvent, value);
		}
	}

	public PictureBox()
	{
		no_update = 0;
		SetStyle(ControlStyles.OptimizedDoubleBuffer, value: true);
		SetStyle(ControlStyles.Opaque, value: false);
		SetStyle(ControlStyles.Selectable, value: false);
		SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
		base.HandleCreated += PictureBox_HandleCreated;
		initial_image = ResourceImageLoader.Get("image-x-generic.png");
		error_image = ResourceImageLoader.Get("image-missing.png");
	}

	static PictureBox()
	{
		LoadCompleted = new object();
		LoadProgressChanged = new object();
		SizeModeChanged = new object();
	}

	void ISupportInitialize.BeginInit()
	{
		no_update++;
	}

	void ISupportInitialize.EndInit()
	{
		if (no_update > 0)
		{
			no_update--;
		}
		if (no_update == 0)
		{
			Invalidate();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (image != null)
		{
			StopAnimation();
			image = null;
		}
		initial_image = null;
		base.Dispose(disposing);
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		ThemeEngine.Current.DrawPictureBox(pe.Graphics, pe.ClipRectangle, this);
		base.OnPaint(pe);
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged(e);
	}

	protected virtual void OnSizeModeChanged(EventArgs e)
	{
		((EventHandler)base.Events[SizeModeChanged])?.Invoke(this, e);
	}

	protected override void OnEnabledChanged(EventArgs e)
	{
		base.OnEnabledChanged(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected virtual void OnLoadCompleted(AsyncCompletedEventArgs e)
	{
		((AsyncCompletedEventHandler)base.Events[LoadCompleted])?.Invoke(this, e);
	}

	protected virtual void OnLoadProgressChanged(ProgressChangedEventArgs e)
	{
		((ProgressChangedEventHandler)base.Events[LoadProgressChanged])?.Invoke(this, e);
	}

	protected override void OnParentChanged(EventArgs e)
	{
		base.OnParentChanged(e);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		Invalidate();
	}

	internal override Size GetPreferredSizeCore(Size proposedSize)
	{
		if (image == null)
		{
			return base.GetPreferredSizeCore(proposedSize);
		}
		return image.Size;
	}

	private void ChangeImage(Image value, bool from_url)
	{
		StopAnimation();
		image_from_url = from_url;
		image = value;
		if (base.IsHandleCreated)
		{
			UpdateSize();
			if (image != null && ImageAnimator.CanAnimate(image))
			{
				frame_handler = OnAnimateImage;
				ImageAnimator.Animate(image, frame_handler);
			}
			if (no_update == 0)
			{
				Invalidate();
			}
		}
	}

	private void StopAnimation()
	{
		if (frame_handler != null)
		{
			ImageAnimator.StopAnimate(image, frame_handler);
			frame_handler = null;
		}
	}

	private void UpdateSize()
	{
		if (image != null && base.Parent != null)
		{
			base.Parent.PerformLayout(this, "AutoSize");
		}
	}

	private void OnAnimateImage(object sender, EventArgs e)
	{
		if (base.IsHandleCreated)
		{
			BeginInvoke(new EventHandler(UpdateAnimatedImage), this, e);
		}
	}

	private void UpdateAnimatedImage(object sender, EventArgs e)
	{
		if (base.IsHandleCreated)
		{
			ImageAnimator.UpdateFrames(image);
			Refresh();
		}
	}

	private void PictureBox_HandleCreated(object sender, EventArgs e)
	{
		UpdateSize();
		if (image != null && ImageAnimator.CanAnimate(image))
		{
			frame_handler = OnAnimateImage;
			ImageAnimator.Animate(image, frame_handler);
		}
		if (no_update == 0)
		{
			Invalidate();
		}
	}

	private void ImageDownload_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
	{
		if (e.Error != null && !e.Cancelled)
		{
			Image = error_image;
		}
		else if (e.Error == null && !e.Cancelled)
		{
			using MemoryStream stream = new MemoryStream(e.Result);
			Image = Image.FromStream(stream);
		}
		ImageDownload.DownloadProgressChanged -= ImageDownload_DownloadProgressChanged;
		ImageDownload.DownloadDataCompleted -= ImageDownload_DownloadDataCompleted;
		image_download = null;
		OnLoadCompleted(e);
	}

	private void ImageDownload_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
	{
		OnLoadProgressChanged(new ProgressChangedEventArgs(e.ProgressPercentage, e.UserState));
	}

	public void CancelAsync()
	{
		if (image_download != null)
		{
			image_download.CancelAsync();
		}
	}

	public void Load()
	{
		Load(image_location);
	}

	public void Load(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new InvalidOperationException("ImageLocation not specified.");
		}
		image_location = url;
		if (url.Contains("://"))
		{
			using (Stream stream = ImageDownload.OpenRead(url))
			{
				ChangeImage(Image.FromStream(stream), from_url: true);
				return;
			}
		}
		ChangeImage(Image.FromFile(url), from_url: true);
	}

	public void LoadAsync()
	{
		LoadAsync(image_location);
	}

	public void LoadAsync(string url)
	{
		if (wait_on_load)
		{
			Load(url);
			return;
		}
		if (string.IsNullOrEmpty(url))
		{
			throw new InvalidOperationException("ImageLocation not specified.");
		}
		image_location = url;
		ChangeImage(InitialImage, from_url: true);
		if (ImageDownload.IsBusy)
		{
			ImageDownload.CancelAsync();
		}
		Uri uri = null;
		try
		{
			uri = new Uri(url);
		}
		catch (UriFormatException)
		{
			uri = new Uri(Path.GetFullPath(url));
		}
		ImageDownload.DownloadProgressChanged += ImageDownload_DownloadProgressChanged;
		ImageDownload.DownloadDataCompleted += ImageDownload_DownloadDataCompleted;
		ImageDownload.DownloadDataAsync(uri);
	}

	public override string ToString()
	{
		return $"{base.ToString()}, SizeMode: {SizeMode}";
	}
}
