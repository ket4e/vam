namespace System.Diagnostics;

public class EventTypeFilter : TraceFilter
{
	private SourceLevels event_type;

	public SourceLevels EventType
	{
		get
		{
			return event_type;
		}
		set
		{
			event_type = value;
		}
	}

	public EventTypeFilter(SourceLevels eventType)
	{
		event_type = eventType;
	}

	public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
	{
		switch (eventType)
		{
		case TraceEventType.Critical:
			return (event_type & SourceLevels.Critical) != 0;
		case TraceEventType.Error:
			return (event_type & SourceLevels.Error) != 0;
		case TraceEventType.Information:
			return (event_type & SourceLevels.Information) != 0;
		case TraceEventType.Verbose:
			return (event_type & SourceLevels.Verbose) != 0;
		case TraceEventType.Warning:
			return (event_type & SourceLevels.Warning) != 0;
		case TraceEventType.Start:
		case TraceEventType.Stop:
		case TraceEventType.Suspend:
		case TraceEventType.Resume:
		case TraceEventType.Transfer:
			return (event_type & SourceLevels.ActivityTracing) != 0;
		default:
			return event_type != SourceLevels.Off;
		}
	}
}
