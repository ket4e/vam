using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Mono.WebBrowser;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[Docking(DockingBehavior.AutoDock)]
[DefaultEvent("DocumentCompleted")]
[DefaultProperty("Url")]
[ComVisible(true)]
[Designer("System.Windows.Forms.Design.WebBrowserDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
public class WebBrowser : WebBrowserBase
{
	[System.MonoTODO("Stub, not implemented")]
	[ComVisible(false)]
	protected class WebBrowserSite : WebBrowserSiteBase
	{
		[System.MonoTODO("Stub, not implemented")]
		public WebBrowserSite(WebBrowser host)
		{
		}
	}

	private bool allowNavigation;

	private bool allowWebBrowserDrop = true;

	private bool isWebBrowserContextMenuEnabled;

	private object objectForScripting;

	private bool webBrowserShortcutsEnabled;

	private bool scrollbarsEnabled = true;

	private WebBrowserReadyState readyState;

	private HtmlDocument document;

	private WebBrowserEncryptionLevel securityLevel;

	private Stream data;

	private bool isStreamSet;

	private string url;

	[DefaultValue(true)]
	public bool AllowNavigation
	{
		get
		{
			return allowNavigation;
		}
		set
		{
			allowNavigation = value;
		}
	}

	[DefaultValue(true)]
	public bool AllowWebBrowserDrop
	{
		get
		{
			return allowWebBrowserDrop;
		}
		set
		{
			allowWebBrowserDrop = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CanGoBack => base.WebHost.Navigation.CanGoBack;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CanGoForward => base.WebHost.Navigation.CanGoForward;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public HtmlDocument Document
	{
		get
		{
			if (document == null && documentReady)
			{
				document = new HtmlDocument(this, base.WebHost);
			}
			return document;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public Stream DocumentStream
	{
		get
		{
			if (base.WebHost.Document == null || base.WebHost.Document.DocumentElement == null)
			{
				return null;
			}
			return null;
		}
		set
		{
			if (!allowNavigation)
			{
				Url = new Uri("about:blank");
				data = value;
				isStreamSet = true;
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string DocumentText
	{
		get
		{
			if (base.WebHost.Document == null || base.WebHost.Document.DocumentElement == null)
			{
				return string.Empty;
			}
			return base.WebHost.Document.DocumentElement.OuterHTML;
		}
		set
		{
			if (base.WebHost.Document != null && base.WebHost.Document.DocumentElement != null)
			{
				base.WebHost.Document.DocumentElement.OuterHTML = value;
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public string DocumentTitle
	{
		get
		{
			if (document != null)
			{
				return document.Title;
			}
			return string.Empty;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public string DocumentType
	{
		get
		{
			if (document != null)
			{
				return document.DocType;
			}
			return string.Empty;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public WebBrowserEncryptionLevel EncryptionLevel => securityLevel;

	public override bool Focused => base.Focused;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool IsBusy => !documentReady;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool IsOffline => base.WebHost.Offline;

	[DefaultValue(true)]
	[System.MonoTODO("Stub, not implemented")]
	public bool IsWebBrowserContextMenuEnabled
	{
		get
		{
			return isWebBrowserContextMenuEnabled;
		}
		set
		{
			isWebBrowserContextMenuEnabled = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[System.MonoTODO("Stub, not implemented")]
	public object ObjectForScripting
	{
		get
		{
			return objectForScripting;
		}
		set
		{
			objectForScripting = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public WebBrowserReadyState ReadyState => readyState;

	[DefaultValue(false)]
	public bool ScriptErrorsSuppressed
	{
		get
		{
			return base.SuppressDialogs;
		}
		set
		{
			base.SuppressDialogs = value;
		}
	}

	[DefaultValue(true)]
	public bool ScrollBarsEnabled
	{
		get
		{
			return scrollbarsEnabled;
		}
		set
		{
			scrollbarsEnabled = value;
			if (document != null)
			{
				SetScrollbars();
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual string StatusText => status;

	[TypeConverter(typeof(WebBrowserUriTypeConverter))]
	[DefaultValue(null)]
	[Bindable(true)]
	public Uri Url
	{
		get
		{
			if (url != null)
			{
				return new Uri(url);
			}
			if (base.WebHost.Document != null && base.WebHost.Document.Url != null)
			{
				return new Uri(base.WebHost.Document.Url);
			}
			return null;
		}
		set
		{
			url = null;
			Navigate(value);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public Version Version
	{
		get
		{
			Assembly assembly = base.WebHost.GetType().Assembly;
			return assembly.GetName().Version;
		}
	}

	[System.MonoTODO("Stub, not implemented")]
	[DefaultValue(true)]
	public bool WebBrowserShortcutsEnabled
	{
		get
		{
			return webBrowserShortcutsEnabled;
		}
		set
		{
			webBrowserShortcutsEnabled = value;
		}
	}

	protected override Size DefaultSize => base.DefaultSize;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new Padding Padding
	{
		get
		{
			return base.Padding;
		}
		set
		{
			base.Padding = value;
		}
	}

	[Browsable(false)]
	public event EventHandler CanGoBackChanged;

	[Browsable(false)]
	public event EventHandler CanGoForwardChanged;

	public event WebBrowserDocumentCompletedEventHandler DocumentCompleted;

	[Browsable(false)]
	public event EventHandler DocumentTitleChanged;

	[Browsable(false)]
	public event EventHandler EncryptionLevelChanged;

	public event EventHandler FileDownload;

	public event WebBrowserNavigatedEventHandler Navigated;

	public event WebBrowserNavigatingEventHandler Navigating;

	public event CancelEventHandler NewWindow;

	public event WebBrowserProgressChangedEventHandler ProgressChanged;

	[Browsable(false)]
	public event EventHandler StatusTextChanged;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new event EventHandler PaddingChanged;

	[System.MonoTODO("WebBrowser control is only supported on Linux/Windows. No support for OSX.")]
	public WebBrowser()
	{
	}

	public bool GoBack()
	{
		documentReady = false;
		document = null;
		return base.WebHost.Navigation.Back();
	}

	public bool GoForward()
	{
		documentReady = false;
		document = null;
		return base.WebHost.Navigation.Forward();
	}

	public void GoHome()
	{
		documentReady = false;
		document = null;
		base.WebHost.Navigation.Home();
	}

	public void Navigate(string urlString)
	{
		documentReady = false;
		document = null;
		base.WebHost.Navigation.Go(urlString);
	}

	public void Navigate(Uri url)
	{
		documentReady = false;
		document = null;
		base.WebHost.Navigation.Go(url.ToString());
	}

	public void Navigate(string urlString, bool newWindow)
	{
		documentReady = false;
		document = null;
		base.WebHost.Navigation.Go(urlString);
	}

	public void Navigate(string urlString, string targetFrameName)
	{
		documentReady = false;
		document = null;
		base.WebHost.Navigation.Go(urlString);
	}

	public void Navigate(Uri url, bool newWindow)
	{
		documentReady = false;
		document = null;
		base.WebHost.Navigation.Go(url.ToString());
	}

	public void Navigate(Uri url, string targetFrameName)
	{
		documentReady = false;
		document = null;
		base.WebHost.Navigation.Go(url.ToString());
	}

	public void Navigate(string urlString, string targetFrameName, byte[] postData, string additionalHeaders)
	{
		documentReady = false;
		document = null;
		base.WebHost.Navigation.Go(urlString);
	}

	public void Navigate(Uri url, string targetFrameName, byte[] postData, string additionalHeaders)
	{
		documentReady = false;
		document = null;
		base.WebHost.Navigation.Go(url.ToString());
	}

	public override void Refresh()
	{
		Refresh(WebBrowserRefreshOption.IfExpired);
	}

	public void Refresh(WebBrowserRefreshOption opt)
	{
		documentReady = false;
		document = null;
		switch (opt)
		{
		case WebBrowserRefreshOption.Normal:
			base.WebHost.Navigation.Reload(ReloadOption.Proxy);
			break;
		case WebBrowserRefreshOption.IfExpired:
			base.WebHost.Navigation.Reload(ReloadOption.None);
			break;
		case WebBrowserRefreshOption.Completely:
			base.WebHost.Navigation.Reload(ReloadOption.Full);
			break;
		case WebBrowserRefreshOption.Continue:
			break;
		}
	}

	public void Stop()
	{
		base.WebHost.Navigation.Stop();
	}

	public void GoSearch()
	{
		string urlString = "http://www.google.com";
		try
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Internet Explorer\\Main\\Search Page");
			if (registryKey != null)
			{
				object value = registryKey.GetValue("Default_Search_URL");
				if (value != null && value is string && Uri.TryCreate(value as string, UriKind.Absolute, out var result))
				{
					urlString = result.ToString();
				}
			}
		}
		catch
		{
		}
		Navigate(urlString);
	}

	public void Print()
	{
		throw new NotImplementedException();
	}

	public void ShowPageSetupDialog()
	{
		throw new NotImplementedException();
	}

	public void ShowPrintDialog()
	{
		throw new NotImplementedException();
	}

	public void ShowPrintPreviewDialog()
	{
		throw new NotImplementedException();
	}

	public void ShowPropertiesDialog()
	{
		throw new NotImplementedException();
	}

	public void ShowSaveAsDialog()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Stub, not implemented")]
	protected override void AttachInterfaces(object nativeActiveXObject)
	{
		base.AttachInterfaces(nativeActiveXObject);
	}

	[System.MonoTODO("Stub, not implemented")]
	protected override void CreateSink()
	{
		base.CreateSink();
	}

	[System.MonoTODO("Stub, not implemented")]
	protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
	{
		return base.CreateWebBrowserSiteBase();
	}

	[System.MonoTODO("Stub, not implemented")]
	protected override void DetachInterfaces()
	{
		base.DetachInterfaces();
	}

	[System.MonoTODO("Stub, not implemented")]
	protected override void DetachSink()
	{
		base.DetachSink();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	protected virtual void OnCanGoBackChanged(EventArgs e)
	{
		if (this.CanGoBackChanged != null)
		{
			this.CanGoBackChanged(this, e);
		}
	}

	protected virtual void OnCanGoForwardChanged(EventArgs e)
	{
		if (this.CanGoForwardChanged != null)
		{
			this.CanGoForwardChanged(this, e);
		}
	}

	protected virtual void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
	{
		if (this.DocumentCompleted != null)
		{
			this.DocumentCompleted(this, e);
		}
	}

	protected virtual void OnDocumentTitleChanged(EventArgs e)
	{
		if (this.DocumentTitleChanged != null)
		{
			this.DocumentTitleChanged(this, e);
		}
	}

	protected virtual void OnEncryptionLevelChanged(EventArgs e)
	{
		if (this.EncryptionLevelChanged != null)
		{
			this.EncryptionLevelChanged(this, e);
		}
	}

	protected virtual void OnFileDownload(EventArgs e)
	{
		if (this.FileDownload != null)
		{
			this.FileDownload(this, e);
		}
	}

	protected virtual void OnNavigated(WebBrowserNavigatedEventArgs e)
	{
		if (this.Navigated != null)
		{
			this.Navigated(this, e);
		}
	}

	protected virtual void OnNavigating(WebBrowserNavigatingEventArgs e)
	{
		if (this.Navigating != null)
		{
			this.Navigating(this, e);
		}
	}

	protected virtual void OnNewWindow(CancelEventArgs e)
	{
		if (this.NewWindow != null)
		{
			this.NewWindow(this, e);
		}
	}

	protected virtual void OnProgressChanged(WebBrowserProgressChangedEventArgs e)
	{
		if (this.ProgressChanged != null)
		{
			this.ProgressChanged(this, e);
		}
	}

	protected virtual void OnStatusTextChanged(EventArgs e)
	{
		if (this.StatusTextChanged != null)
		{
			this.StatusTextChanged(this, e);
		}
	}

	internal override bool OnNewWindowInternal()
	{
		CancelEventArgs cancelEventArgs = new CancelEventArgs();
		OnNewWindow(cancelEventArgs);
		return cancelEventArgs.Cancel;
	}

	internal override void OnWebHostLoadStarted(object sender, LoadStartedEventArgs e)
	{
		documentReady = false;
		document = null;
		readyState = WebBrowserReadyState.Loading;
		WebBrowserNavigatingEventArgs e2 = new WebBrowserNavigatingEventArgs(new Uri(e.Uri), e.FrameName);
		OnNavigating(e2);
	}

	internal override void OnWebHostLoadCommited(object sender, LoadCommitedEventArgs e)
	{
		readyState = WebBrowserReadyState.Loaded;
		url = e.Uri;
		SetScrollbars();
		WebBrowserNavigatedEventArgs e2 = new WebBrowserNavigatedEventArgs(new Uri(e.Uri));
		OnNavigated(e2);
	}

	internal override void OnWebHostProgressChanged(object sender, Mono.WebBrowser.ProgressChangedEventArgs e)
	{
		readyState = WebBrowserReadyState.Interactive;
		WebBrowserProgressChangedEventArgs e2 = new WebBrowserProgressChangedEventArgs(e.Progress, e.MaxProgress);
		OnProgressChanged(e2);
	}

	internal override void OnWebHostLoadFinished(object sender, LoadFinishedEventArgs e)
	{
		url = null;
		documentReady = true;
		readyState = WebBrowserReadyState.Complete;
		if (isStreamSet)
		{
			byte[] buffer = new byte[data.Length];
			long length = data.Length;
			int num = 0;
			data.Position = 0L;
			do
			{
				num = data.Read(buffer, (int)data.Position, (int)(length - data.Position));
			}
			while (num > 0);
			base.WebHost.Render(buffer);
			data = null;
			isStreamSet = false;
		}
		SetScrollbars();
		WebBrowserDocumentCompletedEventArgs e2 = new WebBrowserDocumentCompletedEventArgs(new Uri(e.Uri));
		OnDocumentCompleted(e2);
	}

	internal override void OnWebHostSecurityChanged(object sender, SecurityChangedEventArgs e)
	{
		switch (e.State)
		{
		case SecurityLevel.Insecure:
			securityLevel = WebBrowserEncryptionLevel.Insecure;
			break;
		case SecurityLevel.Mixed:
			securityLevel = WebBrowserEncryptionLevel.Mixed;
			break;
		case SecurityLevel.Secure:
			securityLevel = WebBrowserEncryptionLevel.Bit56;
			break;
		}
	}

	internal override void OnWebHostContextMenuShown(object sender, ContextMenuEventArgs e)
	{
		if (isWebBrowserContextMenuEnabled)
		{
			ContextMenu contextMenu = new ContextMenu();
			MenuItem menuItem = new MenuItem("Back", delegate
			{
				GoBack();
			});
			menuItem.Enabled = CanGoBack;
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Forward", delegate
			{
				GoForward();
			});
			menuItem.Enabled = CanGoForward;
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Refresh", delegate
			{
				Refresh();
			});
			contextMenu.MenuItems.Add(menuItem);
			contextMenu.MenuItems.Add(new MenuItem("-"));
			contextMenu.Show(this, PointToClient(Control.MousePosition));
		}
	}

	internal override void OnWebHostStatusChanged(object sender, StatusChangedEventArgs e)
	{
		status = e.Message;
		OnStatusTextChanged(null);
	}

	private void SetScrollbars()
	{
	}
}
