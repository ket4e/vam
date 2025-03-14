using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml;

[Serializable]
public class XmlException : SystemException
{
	private const string Xml_DefaultException = "Xml_DefaultException";

	private const string Xml_UserException = "Xml_UserException";

	private int lineNumber;

	private int linePosition;

	private string sourceUri;

	private string res;

	private string[] messages;

	public int LineNumber => lineNumber;

	public int LinePosition => linePosition;

	public string SourceUri => sourceUri;

	public override string Message
	{
		get
		{
			if (lineNumber == 0)
			{
				return base.Message;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0} {3} Line {1}, position {2}.", base.Message, lineNumber, linePosition, sourceUri);
		}
	}

	public XmlException()
	{
		res = "Xml_DefaultException";
		messages = new string[1];
	}

	public XmlException(string message, Exception innerException)
		: base(message, innerException)
	{
		res = "Xml_UserException";
		messages = new string[1] { message };
	}

	protected XmlException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		lineNumber = info.GetInt32("lineNumber");
		linePosition = info.GetInt32("linePosition");
		res = info.GetString("res");
		messages = (string[])info.GetValue("args", typeof(string[]));
		sourceUri = info.GetString("sourceUri");
	}

	public XmlException(string message)
		: base(message)
	{
		res = "Xml_UserException";
		messages = new string[1] { message };
	}

	internal XmlException(IXmlLineInfo li, string sourceUri, string message)
		: this(li, null, sourceUri, message)
	{
	}

	internal XmlException(IXmlLineInfo li, Exception innerException, string sourceUri, string message)
		: this(message, innerException)
	{
		if (li != null)
		{
			lineNumber = li.LineNumber;
			linePosition = li.LinePosition;
		}
		this.sourceUri = sourceUri;
	}

	public XmlException(string message, Exception innerException, int lineNumber, int linePosition)
		: this(message, innerException)
	{
		this.lineNumber = lineNumber;
		this.linePosition = linePosition;
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("lineNumber", lineNumber);
		info.AddValue("linePosition", linePosition);
		info.AddValue("res", res);
		info.AddValue("args", messages);
		info.AddValue("sourceUri", sourceUri);
	}
}
