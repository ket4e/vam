using System.IO;
using System.Text;

namespace Un4seen.Bass.Misc;

internal class StringWriterWithEncoding : StringWriter
{
	private Encoding _encoding;

	public override Encoding Encoding => _encoding;

	public StringWriterWithEncoding(Encoding encoding)
	{
		_encoding = encoding;
	}
}
