using System.IO;
using System.Text;

namespace System.Diagnostics;

public class DelimitedListTraceListener : TextWriterTraceListener
{
	private static readonly string[] attributes = new string[1] { "delimiter" };

	private string delimiter = ";";

	public string Delimiter
	{
		get
		{
			return delimiter;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			delimiter = value;
		}
	}

	public DelimitedListTraceListener(string fileName)
		: base(fileName)
	{
	}

	public DelimitedListTraceListener(string fileName, string name)
		: base(fileName, name)
	{
	}

	public DelimitedListTraceListener(Stream stream)
		: base(stream)
	{
	}

	public DelimitedListTraceListener(Stream stream, string name)
		: base(stream, name)
	{
	}

	public DelimitedListTraceListener(TextWriter writer)
		: base(writer)
	{
	}

	public DelimitedListTraceListener(TextWriter writer, string name)
		: base(writer, name)
	{
	}

	protected internal override string[] GetSupportedAttributes()
	{
		return attributes;
	}

	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
	{
		TraceCore(eventCache, source, eventType, id, null, data);
	}

	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
	{
		TraceCore(eventCache, source, eventType, id, null, data);
	}

	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
	{
		TraceCore(eventCache, source, eventType, id, message);
	}

	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
	{
		TraceCore(eventCache, source, eventType, id, string.Format(format, args));
	}

	private void TraceCore(TraceEventCache c, string source, TraceEventType eventType, int id, string message, params object[] data)
	{
		Write(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{12}", delimiter, (source == null) ? null : ("\"" + source.Replace("\"", "\"\"") + "\""), eventType, id, (message == null) ? null : ("\"" + message.Replace("\"", "\"\"") + "\""), FormatData(data), (!IsTarget(c, TraceOptions.ProcessId)) ? null : c.ProcessId.ToString(), (!IsTarget(c, TraceOptions.LogicalOperationStack)) ? null : TraceListener.FormatArray(c.LogicalOperationStack, ", "), (!IsTarget(c, TraceOptions.ThreadId)) ? null : c.ThreadId, (!IsTarget(c, TraceOptions.DateTime)) ? null : c.DateTime.ToString("o"), (!IsTarget(c, TraceOptions.Timestamp)) ? null : c.Timestamp.ToString(), (!IsTarget(c, TraceOptions.Callstack)) ? null : c.Callstack, Environment.NewLine));
	}

	private bool IsTarget(TraceEventCache c, TraceOptions opt)
	{
		return c != null && (base.TraceOutputOptions & opt) != 0;
	}

	private string FormatData(object[] data)
	{
		if (data == null || data.Length == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < data.Length; i++)
		{
			if (data[i] != null)
			{
				stringBuilder.Append('"').Append(data[i].ToString().Replace("\"", "\"\"")).Append('"');
			}
			if (i + 1 < data.Length)
			{
				stringBuilder.Append(',');
			}
		}
		return stringBuilder.ToString();
	}
}
