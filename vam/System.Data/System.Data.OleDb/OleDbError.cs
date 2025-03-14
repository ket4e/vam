namespace System.Data.OleDb;

[Serializable]
public sealed class OleDbError
{
	private string message;

	private int nativeError;

	private string source;

	private string sqlState;

	public string Message => message;

	public int NativeError => nativeError;

	public string Source => source;

	public string SQLState => sqlState;

	internal OleDbError(string msg, int code, string source, string sql)
	{
		message = msg;
		nativeError = code;
		this.source = source;
		sqlState = sql;
	}

	[System.MonoTODO]
	public override string ToString()
	{
		string text = " <Stack Trace>";
		return "OleDbError:" + message + text;
	}
}
