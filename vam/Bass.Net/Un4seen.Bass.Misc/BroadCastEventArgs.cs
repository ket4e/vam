using System;

namespace Un4seen.Bass.Misc;

public class BroadCastEventArgs : EventArgs
{
	private readonly BroadCastEventType _eventType;

	private readonly object _data;

	private readonly DateTime _now;

	public BroadCastEventType EventType => _eventType;

	public object Data => _data;

	public DateTime DateTime => _now;

	public BroadCastEventArgs(BroadCastEventType pEventType, object pData)
	{
		_eventType = pEventType;
		_data = pData;
		_now = DateTime.Now;
	}
}
