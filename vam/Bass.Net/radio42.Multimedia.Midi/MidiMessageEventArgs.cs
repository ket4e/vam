using System;

namespace radio42.Multimedia.Midi;

[Serializable]
public class MidiMessageEventArgs : EventArgs
{
	private readonly MidiMessageEventType _eventType;

	private readonly object _message;

	private readonly int _deviceID;

	private readonly IntPtr _device;

	public MidiMessageEventType EventType => _eventType;

	public int DeviceID => _deviceID;

	public IntPtr Device => _device;

	public object Message => _message;

	public MidiShortMessage ShortMessage
	{
		get
		{
			if (EventType == MidiMessageEventType.ShortMessage || EventType == MidiMessageEventType.ShortMessageError)
			{
				return _message as MidiShortMessage;
			}
			return null;
		}
	}

	public bool IsShortMessage
	{
		get
		{
			if (EventType == MidiMessageEventType.ShortMessage || EventType == MidiMessageEventType.ShortMessageError)
			{
				return true;
			}
			return false;
		}
	}

	public MidiSysExMessage SysExMessage
	{
		get
		{
			if (EventType == MidiMessageEventType.SystemExclusive || EventType == MidiMessageEventType.SystemExclusiveError || EventType == MidiMessageEventType.SystemExclusiveDone)
			{
				return _message as MidiSysExMessage;
			}
			return null;
		}
	}

	public bool IsSysExMessage
	{
		get
		{
			if (EventType == MidiMessageEventType.SystemExclusive || EventType == MidiMessageEventType.SystemExclusiveError || EventType == MidiMessageEventType.SystemExclusiveDone)
			{
				return true;
			}
			return false;
		}
	}

	public MidiMessageEventArgs(MidiMessageEventType pEventType, int pDeviceID, IntPtr pDevice, object pMessage)
	{
		_eventType = pEventType;
		_deviceID = pDeviceID;
		_device = pDevice;
		_message = pMessage;
	}
}
