using System.Runtime.Serialization;

namespace System.Transactions;

[Serializable]
public class TransactionAbortedException : TransactionException
{
	public TransactionAbortedException()
	{
	}

	public TransactionAbortedException(string message)
		: base(message)
	{
	}

	public TransactionAbortedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected TransactionAbortedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
