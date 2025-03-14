namespace System.Data;

public sealed class StateChangeEventArgs : EventArgs
{
	private ConnectionState originalState;

	private ConnectionState currentState;

	public ConnectionState CurrentState => currentState;

	public ConnectionState OriginalState => originalState;

	public StateChangeEventArgs(ConnectionState originalState, ConnectionState currentState)
	{
		this.originalState = originalState;
		this.currentState = currentState;
	}
}
