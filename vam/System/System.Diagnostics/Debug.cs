namespace System.Diagnostics;

public sealed class Debug
{
	public static bool AutoFlush
	{
		get
		{
			return System.Diagnostics.TraceImpl.AutoFlush;
		}
		set
		{
			System.Diagnostics.TraceImpl.AutoFlush = value;
		}
	}

	public static int IndentLevel
	{
		get
		{
			return System.Diagnostics.TraceImpl.IndentLevel;
		}
		set
		{
			System.Diagnostics.TraceImpl.IndentLevel = value;
		}
	}

	public static int IndentSize
	{
		get
		{
			return System.Diagnostics.TraceImpl.IndentSize;
		}
		set
		{
			System.Diagnostics.TraceImpl.IndentSize = value;
		}
	}

	public static TraceListenerCollection Listeners => System.Diagnostics.TraceImpl.Listeners;

	private Debug()
	{
	}

	[Conditional("DEBUG")]
	public static void Assert(bool condition)
	{
		System.Diagnostics.TraceImpl.Assert(condition);
	}

	[Conditional("DEBUG")]
	public static void Assert(bool condition, string message)
	{
		System.Diagnostics.TraceImpl.Assert(condition, message);
	}

	[Conditional("DEBUG")]
	public static void Assert(bool condition, string message, string detailMessage)
	{
		System.Diagnostics.TraceImpl.Assert(condition, message, detailMessage);
	}

	[Conditional("DEBUG")]
	public static void Close()
	{
		System.Diagnostics.TraceImpl.Close();
	}

	[Conditional("DEBUG")]
	public static void Fail(string message)
	{
		System.Diagnostics.TraceImpl.Fail(message);
	}

	[Conditional("DEBUG")]
	public static void Fail(string message, string detailMessage)
	{
		System.Diagnostics.TraceImpl.Fail(message, detailMessage);
	}

	[Conditional("DEBUG")]
	public static void Flush()
	{
		System.Diagnostics.TraceImpl.Flush();
	}

	[Conditional("DEBUG")]
	public static void Indent()
	{
		System.Diagnostics.TraceImpl.Indent();
	}

	[Conditional("DEBUG")]
	public static void Unindent()
	{
		System.Diagnostics.TraceImpl.Unindent();
	}

	[Conditional("DEBUG")]
	public static void Write(object value)
	{
		System.Diagnostics.TraceImpl.Write(value);
	}

	[Conditional("DEBUG")]
	public static void Write(string message)
	{
		System.Diagnostics.TraceImpl.Write(message);
	}

	[Conditional("DEBUG")]
	public static void Write(object value, string category)
	{
		System.Diagnostics.TraceImpl.Write(value, category);
	}

	[Conditional("DEBUG")]
	public static void Write(string message, string category)
	{
		System.Diagnostics.TraceImpl.Write(message, category);
	}

	[Conditional("DEBUG")]
	public static void WriteIf(bool condition, object value)
	{
		System.Diagnostics.TraceImpl.WriteIf(condition, value);
	}

	[Conditional("DEBUG")]
	public static void WriteIf(bool condition, string message)
	{
		System.Diagnostics.TraceImpl.WriteIf(condition, message);
	}

	[Conditional("DEBUG")]
	public static void WriteIf(bool condition, object value, string category)
	{
		System.Diagnostics.TraceImpl.WriteIf(condition, value, category);
	}

	[Conditional("DEBUG")]
	public static void WriteIf(bool condition, string message, string category)
	{
		System.Diagnostics.TraceImpl.WriteIf(condition, message, category);
	}

	[Conditional("DEBUG")]
	public static void WriteLine(object value)
	{
		System.Diagnostics.TraceImpl.WriteLine(value);
	}

	[Conditional("DEBUG")]
	public static void WriteLine(string message)
	{
		System.Diagnostics.TraceImpl.WriteLine(message);
	}

	[Conditional("DEBUG")]
	public static void WriteLine(object value, string category)
	{
		System.Diagnostics.TraceImpl.WriteLine(value, category);
	}

	[Conditional("DEBUG")]
	public static void WriteLine(string message, string category)
	{
		System.Diagnostics.TraceImpl.WriteLine(message, category);
	}

	[Conditional("DEBUG")]
	public static void WriteLineIf(bool condition, object value)
	{
		System.Diagnostics.TraceImpl.WriteLineIf(condition, value);
	}

	[Conditional("DEBUG")]
	public static void WriteLineIf(bool condition, string message)
	{
		System.Diagnostics.TraceImpl.WriteLineIf(condition, message);
	}

	[Conditional("DEBUG")]
	public static void WriteLineIf(bool condition, object value, string category)
	{
		System.Diagnostics.TraceImpl.WriteLineIf(condition, value, category);
	}

	[Conditional("DEBUG")]
	public static void WriteLineIf(bool condition, string message, string category)
	{
		System.Diagnostics.TraceImpl.WriteLineIf(condition, message, category);
	}

	[Conditional("DEBUG")]
	public static void Print(string message)
	{
		System.Diagnostics.TraceImpl.WriteLine(message);
	}

	[Conditional("DEBUG")]
	public static void Print(string format, params object[] args)
	{
		System.Diagnostics.TraceImpl.WriteLine(string.Format(format, args));
	}
}
