namespace System.Data.Odbc;

public sealed class OdbcInfoMessageEventArgs : EventArgs
{
	private OdbcErrorCollection errors = new OdbcErrorCollection();

	public OdbcErrorCollection Errors => errors;

	public string Message => errors[0].Message;

	internal OdbcInfoMessageEventArgs(OdbcErrorCollection errors)
	{
		foreach (OdbcError error in errors)
		{
			this.errors.Add(error);
		}
	}

	public override string ToString()
	{
		return Message;
	}
}
