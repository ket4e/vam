using System;
using System.Runtime.Serialization;

namespace ICSharpCode.SharpZipLib.Tar;

[Serializable]
public class TarException : SharpZipBaseException
{
	protected TarException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public TarException()
	{
	}

	public TarException(string message)
		: base(message)
	{
	}

	public TarException(string message, Exception exception)
		: base(message, exception)
	{
	}
}
