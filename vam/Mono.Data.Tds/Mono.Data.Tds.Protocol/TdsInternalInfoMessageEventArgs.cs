using System;

namespace Mono.Data.Tds.Protocol;

public class TdsInternalInfoMessageEventArgs : EventArgs
{
	private TdsInternalErrorCollection errors;

	public TdsInternalErrorCollection Errors => errors;

	public byte Class => errors[0].Class;

	public int LineNumber => errors[0].LineNumber;

	public string Message => errors[0].Message;

	public int Number => errors[0].Number;

	public string Procedure => errors[0].Procedure;

	public string Server => errors[0].Server;

	public string Source => errors[0].Source;

	public byte State => errors[0].State;

	public TdsInternalInfoMessageEventArgs(TdsInternalErrorCollection errors)
	{
		this.errors = errors;
	}

	public TdsInternalInfoMessageEventArgs(TdsInternalError error)
	{
		errors = new TdsInternalErrorCollection();
		errors.Add(error);
	}

	public int Add(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
	{
		return errors.Add(new TdsInternalError(theClass, lineNumber, message, number, procedure, server, source, state));
	}
}
