using System.Collections;
using System.Collections.Specialized;
using System.Drawing;

namespace System.Windows.Forms;

internal class ClipboardData
{
	private ListDictionary source_data;

	private string plain_text_source;

	private Image image_source;

	internal object Item;

	internal ArrayList Formats;

	internal bool Retrieving;

	internal bool Enumerating;

	internal XplatUI.ObjectToClipboard Converter;

	public bool IsSourceText => plain_text_source != null;

	public bool IsSourceImage => image_source != null;

	public ClipboardData()
	{
		source_data = new ListDictionary();
	}

	public void ClearSources()
	{
		source_data.Clear();
		plain_text_source = null;
		image_source = null;
	}

	public void AddSource(int type, object source)
	{
		if (source is string && (type == DataFormats.GetFormat(DataFormats.Text).Id || type == -1))
		{
			plain_text_source = source as string;
		}
		else if (source is Image)
		{
			image_source = source as Image;
		}
		source_data[type] = source;
	}

	public object GetSource(int type)
	{
		return source_data[type];
	}

	public string GetPlainText()
	{
		return plain_text_source;
	}

	public string GetRtfText()
	{
		DataFormats.Format format = DataFormats.GetFormat(DataFormats.Rtf);
		if (format == null)
		{
			return null;
		}
		return (string)GetSource(format.Id);
	}

	public Image GetImage()
	{
		return image_source;
	}
}
