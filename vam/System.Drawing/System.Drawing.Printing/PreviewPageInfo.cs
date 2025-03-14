namespace System.Drawing.Printing;

public sealed class PreviewPageInfo
{
	private Image image;

	private Size physicalSize;

	public Image Image => image;

	public Size PhysicalSize => physicalSize;

	public PreviewPageInfo(Image image, Size physicalSize)
	{
		this.image = image;
		this.physicalSize = physicalSize;
	}
}
