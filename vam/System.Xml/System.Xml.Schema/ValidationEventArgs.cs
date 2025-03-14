namespace System.Xml.Schema;

public class ValidationEventArgs : EventArgs
{
	private XmlSchemaException exception;

	private string message;

	private XmlSeverityType severity;

	public XmlSchemaException Exception => exception;

	public string Message => message;

	public XmlSeverityType Severity => severity;

	private ValidationEventArgs()
	{
	}

	internal ValidationEventArgs(XmlSchemaException ex, string message, XmlSeverityType severity)
	{
		exception = ex;
		this.message = message;
		this.severity = severity;
	}
}
