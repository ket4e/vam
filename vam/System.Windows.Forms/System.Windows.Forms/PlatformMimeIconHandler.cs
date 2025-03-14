namespace System.Windows.Forms;

internal abstract class PlatformMimeIconHandler
{
	protected MimeExtensionHandlerStatus mimeExtensionHandlerStatus;

	public MimeExtensionHandlerStatus MimeExtensionHandlerStatus => mimeExtensionHandlerStatus;

	public abstract MimeExtensionHandlerStatus Start();

	public virtual object AddAndGetIconIndex(string filename, string mime_type)
	{
		return null;
	}

	public virtual object AddAndGetIconIndex(string mime_type)
	{
		return null;
	}
}
