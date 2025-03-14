using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;

namespace radio42.Multimedia.Midi;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class MidiOutputDevice
{
	private bool _disposed;

	private bool _disposing;

	private int _deviceID = -1;

	private IntPtr _device = IntPtr.Zero;

	private MidiSysExMessage _sysexMsg;

	private MIDIOUTPROC _midiOutProc;

	private IntPtr _user = IntPtr.Zero;

	private MIDIError _lastError;

	public bool IsDisposed => _disposing;

	public IntPtr Device => _device;

	public int DeviceID => _deviceID;

	public IntPtr User
	{
		get
		{
			return _user;
		}
		set
		{
			_user = value;
		}
	}

	public bool IsOpened => _device != IntPtr.Zero;

	public MIDIError LastErrorCode => _lastError;

	public event MidiMessageEventHandler MessageReceived;

	public MidiOutputDevice(int deviceID)
	{
		_deviceID = deviceID;
		_midiOutProc = MidiOutProc;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		_disposing = true;
		if (!_disposed && IsOpened)
		{
			Close();
		}
		_disposed = true;
	}

	~MidiOutputDevice()
	{
		Dispose(disposing: false);
	}

	public static int GetDeviceCount()
	{
		return Midi.MIDI_OutGetNumDevs();
	}

	public static string[] GetDeviceDescriptions()
	{
		List<string> list = new List<string>();
		int num = Midi.MIDI_OutGetNumDevs();
		MIDI_OUTCAPS mIDI_OUTCAPS = new MIDI_OUTCAPS();
		for (int i = 0; i < num; i++)
		{
			if (Midi.MIDI_OutGetDevCaps(i, mIDI_OUTCAPS) == MIDIError.MIDI_OK)
			{
				list.Add(mIDI_OUTCAPS.name);
			}
		}
		return list.ToArray();
	}

	public static int[] GetMidiPorts()
	{
		List<int> list = new List<int>();
		int num = Midi.MIDI_OutGetNumDevs();
		MIDI_OUTCAPS mIDI_OUTCAPS = new MIDI_OUTCAPS();
		for (int i = 0; i < num; i++)
		{
			if (Midi.MIDI_OutGetDevCaps(i, mIDI_OUTCAPS) == MIDIError.MIDI_OK && mIDI_OUTCAPS.IsMidiPort)
			{
				list.Add(i);
			}
		}
		return list.ToArray();
	}

	public static string GetDeviceDescription(int deviceID)
	{
		MIDI_OUTCAPS mIDI_OUTCAPS = new MIDI_OUTCAPS();
		if (Midi.MIDI_OutGetDevCaps(deviceID, mIDI_OUTCAPS) == MIDIError.MIDI_OK)
		{
			return mIDI_OUTCAPS.name;
		}
		return null;
	}

	public static MIDI_OUTCAPS GetInfo(int deviceID)
	{
		MIDI_OUTCAPS mIDI_OUTCAPS = new MIDI_OUTCAPS();
		if (Midi.MIDI_OutGetDevCaps(deviceID, mIDI_OUTCAPS) == MIDIError.MIDI_OK)
		{
			return mIDI_OUTCAPS;
		}
		return null;
	}

	public bool Connect(IntPtr handleTo)
	{
		if (_disposing)
		{
			return false;
		}
		_lastError = Midi.MIDI_Connect(_device, handleTo);
		return _lastError == MIDIError.MIDI_OK;
	}

	public bool Disconnect(IntPtr handleFrom)
	{
		if (_disposing)
		{
			return false;
		}
		_lastError = Midi.MIDI_Disconnect(_device, handleFrom);
		return _lastError == MIDIError.MIDI_OK;
	}

	public bool Open()
	{
		if (_disposing)
		{
			return false;
		}
		if (_device != IntPtr.Zero)
		{
			return true;
		}
		_lastError = Midi.MIDI_OutOpen(ref _device, _deviceID, _midiOutProc, new IntPtr(_deviceID));
		if (_lastError != 0)
		{
			_device = IntPtr.Zero;
		}
		return _device != IntPtr.Zero;
	}

	public bool Close()
	{
		if (_device == IntPtr.Zero)
		{
			return true;
		}
		Midi.MIDI_OutReset(_device);
		_lastError = Midi.MIDI_OutClose(_device);
		if (_lastError == MIDIError.MIDI_OK)
		{
			_device = IntPtr.Zero;
		}
		return _device == IntPtr.Zero;
	}

	public bool Send(MidiShortMessage shortMessage)
	{
		if (!IsOpened || shortMessage == null)
		{
			return false;
		}
		_lastError = Midi.MIDI_OutShortMsg(_device, shortMessage.Message);
		return _lastError == MIDIError.MIDI_OK;
	}

	public bool Send(int shortMessage)
	{
		if (!IsOpened)
		{
			return false;
		}
		_lastError = Midi.MIDI_OutShortMsg(_device, shortMessage);
		return _lastError == MIDIError.MIDI_OK;
	}

	public bool Send(MIDIStatus status, byte channel, byte data1, byte data2)
	{
		if (!IsOpened)
		{
			return false;
		}
		MidiShortMessage midiShortMessage = new MidiShortMessage(status, channel, data1, data2, 0L);
		_lastError = Midi.MIDI_OutShortMsg(_device, midiShortMessage.Message);
		return _lastError == MIDIError.MIDI_OK;
	}

	public bool Send(byte status, byte data1, byte data2)
	{
		if (!IsOpened)
		{
			return false;
		}
		MidiShortMessage midiShortMessage = new MidiShortMessage(status, data1, data2, 0, 0L);
		_lastError = Midi.MIDI_OutShortMsg(_device, midiShortMessage.Message);
		return _lastError == MIDIError.MIDI_OK;
	}

	public bool Send(MidiSysExMessage sysexMessage)
	{
		if (!IsOpened || sysexMessage.IsInput || sysexMessage.Device != Device || sysexMessage.IsPrepared)
		{
			return false;
		}
		if (sysexMessage.Prepare(_user))
		{
			_lastError = Midi.MIDI_OutLongMsg(_device, sysexMessage.MessageAsIntPtr);
			return _lastError == MIDIError.MIDI_OK;
		}
		return false;
	}

	public bool Send(byte[] sysexMessage)
	{
		if (!IsOpened || sysexMessage == null)
		{
			return false;
		}
		MidiSysExMessage midiSysExMessage = new MidiSysExMessage(input: false, _device);
		if (midiSysExMessage.CreateBuffer(sysexMessage))
		{
			if (midiSysExMessage.Prepare(_user))
			{
				_lastError = Midi.MIDI_OutLongMsg(_device, midiSysExMessage.MessageAsIntPtr);
				return _lastError == MIDIError.MIDI_OK;
			}
			return false;
		}
		return false;
	}

	private void MidiOutProc(IntPtr handle, MIDIMessage msg, IntPtr user, IntPtr param1, IntPtr param2)
	{
		switch (msg)
		{
		case MIDIMessage.MOM_OPEN:
			RaiseMessageReceived(MidiMessageEventType.Opened, null);
			break;
		case MIDIMessage.MOM_CLOSE:
			RaiseMessageReceived(MidiMessageEventType.Closed, null);
			break;
		case MIDIMessage.MOM_DONE:
			_sysexMsg = new MidiSysExMessage(input: false, handle, param1, _sysexMsg);
			if (_sysexMsg.IsDone)
			{
				RaiseMessageReceived(MidiMessageEventType.SystemExclusiveDone, _sysexMsg);
			}
			break;
		}
	}

	private void RaiseMessageReceived(MidiMessageEventType pEventType, object pData)
	{
		if (this.MessageReceived != null)
		{
			ProcessDelegate(this.MessageReceived, this, new MidiMessageEventArgs(pEventType, _deviceID, _device, pData));
		}
	}

	private void ProcessDelegate(Delegate del, params object[] args)
	{
		if ((object)del != null)
		{
			Delegate[] invocationList = del.GetInvocationList();
			foreach (Delegate del2 in invocationList)
			{
				InvokeDelegate(del2, args);
			}
		}
	}

	private void InvokeDelegate(Delegate del, object[] args)
	{
		if (del.Target is ISynchronizeInvoke synchronizeInvoke)
		{
			if (synchronizeInvoke.InvokeRequired)
			{
				try
				{
					synchronizeInvoke.BeginInvoke(del, args);
					return;
				}
				catch
				{
					return;
				}
			}
			del.DynamicInvoke(args);
		}
		else
		{
			del.DynamicInvoke(args);
		}
	}
}
