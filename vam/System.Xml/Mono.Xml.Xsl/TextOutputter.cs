using System.IO;

namespace Mono.Xml.Xsl;

internal class TextOutputter : Outputter
{
	private TextWriter _writer;

	private int _depth;

	private bool _ignoreNestedText;

	public override bool CanProcessAttributes => false;

	public override bool InsideCDataSection
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public TextOutputter(TextWriter w, bool ignoreNestedText)
	{
		_writer = w;
		_ignoreNestedText = ignoreNestedText;
	}

	public override void WriteStartElement(string prefix, string localName, string nsURI)
	{
		if (_ignoreNestedText)
		{
			_depth++;
		}
	}

	public override void WriteEndElement()
	{
		if (_ignoreNestedText)
		{
			_depth--;
		}
	}

	public override void WriteAttributeString(string prefix, string localName, string nsURI, string value)
	{
	}

	public override void WriteNamespaceDecl(string prefix, string nsUri)
	{
	}

	public override void WriteComment(string text)
	{
	}

	public override void WriteProcessingInstruction(string name, string text)
	{
	}

	public override void WriteString(string text)
	{
		WriteImpl(text);
	}

	public override void WriteRaw(string data)
	{
		WriteImpl(data);
	}

	public override void WriteWhitespace(string text)
	{
		WriteImpl(text);
	}

	private void WriteImpl(string text)
	{
		if (!_ignoreNestedText || _depth == 0)
		{
			_writer.Write(text);
		}
	}

	public override void Done()
	{
		_writer.Flush();
	}
}
