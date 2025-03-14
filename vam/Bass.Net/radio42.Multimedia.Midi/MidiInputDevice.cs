using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;

namespace radio42.Multimedia.Midi;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class MidiInputDevice : IDisposable
{
	private bool _disposed;

	private bool _disposing;

	private int _deviceID = -1;

	private IntPtr _device = IntPtr.Zero;

	private MidiShortMessage _shortMsg;

	private MidiSysExMessage _sysexMsg;

	private int _sysexBufferSize = 256;

	private MIDIINPROC _midiInProc;

	private IntPtr _user = IntPtr.Zero;

	private bool _started;

	private bool _closing;

	private MIDIError _lastError;

	private bool _autoPairController;

	private byte[,] _controllerPairs;

	private MIDIMessageType _messageFilter;

	private bool _processErrorMessages;

	private MidiShortMessage _shortMsgOnStack;

	private int _pairedResult;

	public bool IsDisposed => _disposing;

	public IntPtr Device => _device;

	public int DeviceID => _deviceID;

	public int SysExBufferSize
	{
		get
		{
			return _sysexBufferSize;
		}
		set
		{
			if (value >= 2 && value <= 65536)
			{
				_sysexBufferSize = value;
			}
		}
	}

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

	public MidiShortMessage ShortMessage => _shortMsg;

	public MidiSysExMessage SysExMessage => _sysexMsg;

	public bool IsOpened => _device != IntPtr.Zero;

	public bool IsStarted => _started;

	public MIDIError LastErrorCode => _lastError;

	public bool AutoPairController
	{
		get
		{
			return _autoPairController;
		}
		set
		{
			_autoPairController = value;
		}
	}

	public byte[,] ColtrollerPairMatrix
	{
		get
		{
			return _controllerPairs;
		}
		set
		{
			if (value == null || value.Rank == 3)
			{
				_controllerPairs = value;
			}
		}
	}

	public MIDIMessageType MessageFilter
	{
		get
		{
			return _messageFilter;
		}
		set
		{
			_messageFilter = value;
		}
	}

	public bool ProcessErrorMessages
	{
		get
		{
			return _processErrorMessages;
		}
		set
		{
			_processErrorMessages = value;
		}
	}

	public event MidiMessageEventHandler MessageReceived;

	public MidiInputDevice(int deviceID)
	{
		_deviceID = deviceID;
		_midiInProc = MidiInProc;
		InitControllerPairs();
	}

	public MidiInputDevice(int deviceID, MIDIINPROC proc)
	{
		_deviceID = deviceID;
		if (proc == null)
		{
			_midiInProc = MidiInProc;
		}
		else
		{
			_midiInProc = proc;
		}
		InitControllerPairs();
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

	~MidiInputDevice()
	{
		Dispose(disposing: false);
	}

	public static int GetDeviceCount()
	{
		return Midi.MIDI_InGetNumDevs();
	}

	public static string[] GetDeviceDescriptions()
	{
		List<string> list = new List<string>();
		int num = Midi.MIDI_InGetNumDevs();
		MIDI_INCAPS mIDI_INCAPS = new MIDI_INCAPS();
		for (int i = 0; i < num; i++)
		{
			if (Midi.MIDI_InGetDevCaps(i, mIDI_INCAPS) == MIDIError.MIDI_OK)
			{
				list.Add(mIDI_INCAPS.name);
			}
		}
		return list.ToArray();
	}

	public static int[] GetMidiPorts()
	{
		List<int> list = new List<int>();
		int num = Midi.MIDI_InGetNumDevs();
		MIDI_INCAPS caps = new MIDI_INCAPS();
		for (int i = 0; i < num; i++)
		{
			if (Midi.MIDI_InGetDevCaps(i, caps) == MIDIError.MIDI_OK)
			{
				list.Add(i);
			}
		}
		return list.ToArray();
	}

	public static string GetDeviceDescription(int deviceID)
	{
		MIDI_INCAPS mIDI_INCAPS = new MIDI_INCAPS();
		if (Midi.MIDI_InGetDevCaps(deviceID, mIDI_INCAPS) == MIDIError.MIDI_OK)
		{
			return mIDI_INCAPS.name;
		}
		return null;
	}

	public static MIDI_INCAPS GetInfo(int deviceID)
	{
		MIDI_INCAPS mIDI_INCAPS = new MIDI_INCAPS();
		if (Midi.MIDI_InGetDevCaps(deviceID, mIDI_INCAPS) == MIDIError.MIDI_OK)
		{
			return mIDI_INCAPS;
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
		_lastError = Midi.MIDI_InOpen(ref _device, _deviceID, _midiInProc, new IntPtr(_deviceID), MIDIFlags.MIDI_IO_STATUS);
		if (_lastError != 0)
		{
			_device = IntPtr.Zero;
		}
		else
		{
			_shortMsgOnStack = null;
		}
		return _device != IntPtr.Zero;
	}

	public bool Close()
	{
		if (_device == IntPtr.Zero)
		{
			return true;
		}
		_closing = true;
		bool started = _started;
		_lastError = Midi.MIDI_InReset(_device);
		if (_lastError == MIDIError.MIDI_OK)
		{
			_started = false;
			if (started)
			{
				RaiseMessageReceived(MidiMessageEventType.Stopped, null);
			}
		}
		_lastError = Midi.MIDI_InClose(_device);
		if (_lastError == MIDIError.MIDI_OK)
		{
			_device = IntPtr.Zero;
		}
		_closing = false;
		return _device == IntPtr.Zero;
	}

	public bool Start()
	{
		if (_disposing)
		{
			return false;
		}
		if (_started)
		{
			return true;
		}
		AddSysExBuffer();
		AddSysExBuffer();
		_lastError = Midi.MIDI_InStart(_device);
		if (_lastError == MIDIError.MIDI_OK)
		{
			_started = true;
			RaiseMessageReceived(MidiMessageEventType.Started, null);
		}
		else
		{
			Midi.MIDI_InReset(_device);
		}
		return _started;
	}

	public bool Stop()
	{
		if (_device == IntPtr.Zero)
		{
			return true;
		}
		if (!_started)
		{
			return true;
		}
		_closing = true;
		_lastError = Midi.MIDI_InStop(_device);
		if (_lastError == MIDIError.MIDI_OK)
		{
			_started = false;
			FlushShortMsgStack();
			RaiseMessageReceived(MidiMessageEventType.Stopped, null);
		}
		_closing = false;
		return !_started;
	}

	public bool AddSysExBuffer()
	{
		if (_closing)
		{
			return true;
		}
		if (_device == IntPtr.Zero || _disposing)
		{
			return false;
		}
		bool result = false;
		try
		{
			MidiSysExMessage midiSysExMessage = new MidiSysExMessage(input: true, _device);
			midiSysExMessage.CreateBuffer(_sysexBufferSize);
			if (midiSysExMessage.Prepare(_user))
			{
				result = midiSysExMessage.Send();
			}
		}
		catch
		{
		}
		return result;
	}

	private void InitControllerPairs()
	{
		_controllerPairs = new byte[34, 3]
		{
			{ 0, 32, 255 },
			{ 1, 33, 255 },
			{ 2, 34, 255 },
			{ 3, 35, 255 },
			{ 4, 36, 255 },
			{ 5, 37, 255 },
			{ 6, 38, 255 },
			{ 7, 39, 255 },
			{ 8, 40, 255 },
			{ 9, 41, 255 },
			{ 10, 42, 255 },
			{ 11, 43, 255 },
			{ 12, 44, 255 },
			{ 13, 45, 255 },
			{ 14, 46, 255 },
			{ 15, 47, 255 },
			{ 16, 48, 255 },
			{ 17, 49, 255 },
			{ 18, 50, 255 },
			{ 19, 51, 255 },
			{ 20, 52, 255 },
			{ 21, 53, 255 },
			{ 22, 54, 255 },
			{ 23, 55, 255 },
			{ 24, 56, 255 },
			{ 25, 57, 255 },
			{ 26, 58, 255 },
			{ 27, 59, 255 },
			{ 28, 60, 255 },
			{ 29, 61, 255 },
			{ 30, 62, 255 },
			{ 31, 63, 255 },
			{ 99, 98, 255 },
			{ 101, 100, 255 }
		};
	}

	private void FlushShortMsgStack()
	{
		if (_shortMsgOnStack != null)
		{
			RaiseMessageReceived(MidiMessageEventType.ShortMessage, _shortMsgOnStack);
			_shortMsgOnStack = null;
		}
	}

	public int IsPairedControllerMessage(MidiShortMessage msg)
	{
		if (!AutoPairController || msg == null || _controllerPairs == null || msg.StatusType != MIDIStatus.ControlChange)
		{
			return 0;
		}
		int num = 0;
		byte controller = msg.Controller;
		for (int i = 0; i < _controllerPairs.GetLength(0); i++)
		{
			byte b = _controllerPairs[i, 0];
			byte b2 = _controllerPairs[i, 1];
			if (controller == b || controller == b2)
			{
				num = -1;
				if (msg.PreviousShortMessage != null && msg.PreviousShortMessage.StatusType == MIDIStatus.ControlChange && msg.PreviousShortMessage.Channel == msg.Channel)
				{
					byte controller2 = msg.PreviousShortMessage.Controller;
					if (controller2 == b && controller == b2)
					{
						num = 2;
						msg.SetContinuousController(thisIsMSB: false, previousIsMSB: true);
					}
					else if (controller2 == b2 && controller == b)
					{
						num = 1;
						msg.SetContinuousController(thisIsMSB: true, previousIsMSB: false);
					}
				}
			}
			if (num != 0)
			{
				break;
			}
		}
		return num;
	}

	private void MidiInProc(IntPtr handle, MIDIMessage msg, IntPtr user, IntPtr param1, IntPtr param2)
	{
		switch (msg)
		{
		case MIDIMessage.MIM_OPEN:
			RaiseMessageReceived(MidiMessageEventType.Opened, null);
			break;
		case MIDIMessage.MIM_CLOSE:
			FlushShortMsgStack();
			RaiseMessageReceived(MidiMessageEventType.Closed, null);
			break;
		case MIDIMessage.MIM_DATA:
		case MIDIMessage.MIM_MOREDATA:
			_shortMsg = new MidiShortMessage(param1, param2, _shortMsg);
			if ((_shortMsg.MessageType & MessageFilter) == 0)
			{
				_pairedResult = IsPairedControllerMessage(_shortMsg);
				if (_pairedResult == 0)
				{
					FlushShortMsgStack();
					RaiseMessageReceived(MidiMessageEventType.ShortMessage, _shortMsg);
				}
				else if (_pairedResult == -1)
				{
					_shortMsgOnStack = _shortMsg;
				}
				else
				{
					_shortMsgOnStack = null;
					RaiseMessageReceived(MidiMessageEventType.ShortMessage, _shortMsg);
				}
			}
			break;
		case MIDIMessage.MIM_LONGDATA:
			FlushShortMsgStack();
			_sysexMsg = new MidiSysExMessage(input: true, handle, param1, _sysexMsg);
			if (_sysexMsg.IsDone && (_sysexMsg.MessageType & MessageFilter) == 0)
			{
				RaiseMessageReceived(MidiMessageEventType.SystemExclusive, _sysexMsg);
			}
			AddSysExBuffer();
			break;
		case MIDIMessage.MIM_ERROR:
			FlushShortMsgStack();
			if (ProcessErrorMessages)
			{
				MidiShortMessage midiShortMessage = new MidiShortMessage(param1, param2);
				if ((midiShortMessage.MessageType & MessageFilter) == 0)
				{
					RaiseMessageReceived(MidiMessageEventType.ShortMessageError, midiShortMessage);
				}
			}
			break;
		case MIDIMessage.MIM_LONGERROR:
		{
			FlushShortMsgStack();
			MidiSysExMessage midiSysExMessage = new MidiSysExMessage(input: true, handle, param1);
			if (midiSysExMessage.IsDone && ProcessErrorMessages && (midiSysExMessage.MessageType & MessageFilter) == 0)
			{
				RaiseMessageReceived(MidiMessageEventType.SystemExclusiveError, midiSysExMessage);
			}
			AddSysExBuffer();
			break;
		}
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
