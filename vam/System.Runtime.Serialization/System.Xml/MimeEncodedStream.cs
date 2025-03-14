using System.IO;
using System.Text;

namespace System.Xml;

internal class MimeEncodedStream
{
	public string Id { get; set; }

	public string ContentEncoding { get; set; }

	public string EncodedString { get; set; }

	public string DecodedBase64String => Convert.ToBase64String(Encoding.ASCII.GetBytes(EncodedString));

	public MimeEncodedStream(string id, string contentEncoding, string value)
	{
		Id = id;
		ContentEncoding = contentEncoding;
		EncodedString = value;
	}

	public TextReader CreateTextReader()
	{
		switch (ContentEncoding)
		{
		case "7bit":
		case "8bit":
			return new StringReader(EncodedString);
		default:
			return new StringReader(DecodedBase64String);
		}
	}
}
