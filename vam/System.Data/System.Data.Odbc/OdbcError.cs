namespace System.Data.Odbc;

[Serializable]
public sealed class OdbcError
{
	private readonly string _message;

	private string _source;

	private readonly string _state;

	private readonly int _nativeerror;

	public string Message => _message;

	public int NativeError => _nativeerror;

	public string Source => _source;

	public string SQLState => _state;

	internal OdbcError(OdbcConnection connection)
	{
		_nativeerror = 1;
		_source = connection.SafeDriver;
		_message = "Error in " + _source;
		_state = string.Empty;
	}

	internal OdbcError(string message, string state, int nativeerror)
	{
		_message = message;
		_state = state;
		_nativeerror = nativeerror;
	}

	public override string ToString()
	{
		return Message;
	}

	internal void SetSource(string source)
	{
		_source = source;
	}
}
