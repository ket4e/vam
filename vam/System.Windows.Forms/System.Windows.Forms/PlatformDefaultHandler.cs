namespace System.Windows.Forms;

internal class PlatformDefaultHandler : PlatformMimeIconHandler
{
	public override MimeExtensionHandlerStatus Start()
	{
		MimeIconEngine.AddIconByImage("inode/directory", ResourceImageLoader.Get("folder.png"));
		MimeIconEngine.AddIconByImage("unknown/unknown", ResourceImageLoader.Get("text-x-generic.png"));
		MimeIconEngine.AddIconByImage("desktop/desktop", ResourceImageLoader.Get("user-desktop.png"));
		MimeIconEngine.AddIconByImage("directory/home", ResourceImageLoader.Get("user-home.png"));
		MimeIconEngine.AddIconByImage("network/network", ResourceImageLoader.Get("folder-remote.png"));
		MimeIconEngine.AddIconByImage("recently/recently", ResourceImageLoader.Get("document-open.png"));
		MimeIconEngine.AddIconByImage("workplace/workplace", ResourceImageLoader.Get("computer.png"));
		return MimeExtensionHandlerStatus.OK;
	}
}
