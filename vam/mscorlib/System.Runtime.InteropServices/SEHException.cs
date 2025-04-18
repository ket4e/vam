using System.Runtime.Serialization;

namespace System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
public class SEHException : ExternalException
{
	public SEHException()
	{
	}

	public SEHException(string message)
		: base(message)
	{
	}

	public SEHException(string message, Exception inner)
		: base(message, inner)
	{
	}

	protected SEHException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public virtual bool CanResume()
	{
		return false;
	}
}
