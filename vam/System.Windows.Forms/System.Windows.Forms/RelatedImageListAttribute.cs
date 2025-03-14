namespace System.Windows.Forms;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RelatedImageListAttribute : Attribute
{
	private string related_image_list;

	public string RelatedImageList => related_image_list;

	public RelatedImageListAttribute(string relatedImageList)
	{
		related_image_list = relatedImageList;
	}
}
