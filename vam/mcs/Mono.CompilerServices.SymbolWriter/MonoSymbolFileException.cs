using System;

namespace Mono.CompilerServices.SymbolWriter;

public class MonoSymbolFileException : Exception
{
	public MonoSymbolFileException()
	{
	}

	public MonoSymbolFileException(string message, params object[] args)
		: base(string.Format(message, args))
	{
	}

	public MonoSymbolFileException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
