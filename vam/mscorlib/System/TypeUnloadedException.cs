using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[ComVisible(true)]
public class TypeUnloadedException : SystemException
{
	public TypeUnloadedException()
		: base(Locale.GetText("Cannot access an unloaded class."))
	{
	}

	public TypeUnloadedException(string message)
		: base(message)
	{
	}

	protected TypeUnloadedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public TypeUnloadedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
