using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Diagnostics;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public sealed class EventLogTraceListener : TraceListener
{
	private EventLog event_log;

	private string name;

	public EventLog EventLog
	{
		get
		{
			return event_log;
		}
		set
		{
			event_log = value;
		}
	}

	public override string Name
	{
		get
		{
			return (name == null) ? event_log.Source : name;
		}
		set
		{
			name = value;
		}
	}

	public EventLogTraceListener()
	{
	}

	public EventLogTraceListener(EventLog eventLog)
	{
		if (eventLog == null)
		{
			throw new ArgumentNullException("eventLog");
		}
		event_log = eventLog;
	}

	public EventLogTraceListener(string source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		event_log = new EventLog();
		event_log.Source = source;
	}

	public override void Close()
	{
		event_log.Close();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			event_log.Dispose();
		}
	}

	public override void Write(string message)
	{
		TraceData(new TraceEventCache(), event_log.Source, TraceEventType.Information, 0, message);
	}

	public override void WriteLine(string message)
	{
		Write(message);
	}

	[ComVisible(false)]
	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
	{
		EventLogEntryType type;
		switch (eventType)
		{
		case TraceEventType.Critical:
		case TraceEventType.Error:
			type = EventLogEntryType.Error;
			break;
		case TraceEventType.Warning:
			type = EventLogEntryType.Warning;
			break;
		default:
			type = EventLogEntryType.Information;
			break;
		}
		event_log.WriteEntry((data == null) ? string.Empty : data.ToString(), type, id, 0);
	}

	[ComVisible(false)]
	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
	{
		string data2 = string.Empty;
		if (data != null)
		{
			string[] array = new string[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				array[i] = ((data[i] == null) ? string.Empty : data[i].ToString());
			}
			data2 = string.Join(", ", array);
		}
		TraceData(eventCache, source, eventType, id, data2);
	}

	[ComVisible(false)]
	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
	{
		TraceData(eventCache, source, eventType, id, message);
	}

	[ComVisible(false)]
	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
	{
		TraceEvent(eventCache, source, eventType, id, (format == null) ? null : string.Format(format, args));
	}
}
