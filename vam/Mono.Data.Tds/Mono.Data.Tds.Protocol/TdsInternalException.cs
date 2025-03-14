using System;
using System.Runtime.Serialization;

namespace Mono.Data.Tds.Protocol;

public class TdsInternalException : SystemException
{
	private byte theClass;

	private int lineNumber;

	private int number;

	private string procedure;

	private string server;

	private string source;

	private byte state;

	public byte Class => theClass;

	public int LineNumber => lineNumber;

	public override string Message => base.Message;

	public int Number => number;

	public string Procedure => procedure;

	public string Server => server;

	public override string Source => source;

	public byte State => state;

	internal TdsInternalException()
		: base("a TDS Exception has occurred.")
	{
	}

	internal TdsInternalException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	internal TdsInternalException(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
		: base(message)
	{
		this.theClass = theClass;
		this.lineNumber = lineNumber;
		this.number = number;
		this.procedure = procedure;
		this.server = server;
		this.source = source;
		this.state = state;
	}

	[System.MonoTODO]
	public override void GetObjectData(SerializationInfo si, StreamingContext context)
	{
		throw new NotImplementedException();
	}
}
