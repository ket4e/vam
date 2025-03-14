using System.ComponentModel;

namespace System.Windows.Forms;

internal class ImageListConverter : ComponentConverter
{
	public ImageListConverter()
		: base(typeof(ImageList))
	{
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
