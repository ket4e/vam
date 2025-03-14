using System;
using System.Security;

namespace radio42.Multimedia.Midi;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class MidiShortMessage : IFormattable
{
	private byte _status;

	private byte _data1;

	private byte _data2;

	private byte _data3;

	private double _timestamp;

	private MidiShortMessage _previous;

	private static long _id;

	private long _myid;

	private bool _thisIsMSB;

	private bool _previousIsMSB;

	public long ID => _myid;

	public MidiShortMessage PreviousShortMessage
	{
		get
		{
			return _previous;
		}
		set
		{
			_previous = value;
		}
	}

	public byte Status
	{
		get
		{
			return _status;
		}
		set
		{
			_status = value;
			if (StatusType == MIDIStatus.None)
			{
				_data3 = 0;
			}
		}
	}

	public byte Data1
	{
		get
		{
			return _data1;
		}
		set
		{
			_data1 = value;
		}
	}

	public byte Data2
	{
		get
		{
			return _data2;
		}
		set
		{
			_data2 = value;
		}
	}

	public short PairedData2
	{
		get
		{
			if (IsSetAsContinuousController)
			{
				if (_thisIsMSB)
				{
					return GetPairedData(_data2, PreviousShortMessage.Data2);
				}
				return GetPairedData(PreviousShortMessage.Data2, _data2);
			}
			return (short)(_data2 & 0x7F);
		}
	}

	public byte Data3
	{
		get
		{
			return _data3;
		}
		set
		{
			if (StatusType == MIDIStatus.None)
			{
				_data3 = value;
			}
			else
			{
				_data3 = 0;
			}
		}
	}

	public TimeSpan Timespan
	{
		get
		{
			return TimeSpan.FromMilliseconds(_timestamp);
		}
		set
		{
			_timestamp = value.TotalMilliseconds;
		}
	}

	public long Timestamp
	{
		get
		{
			return (long)_timestamp;
		}
		set
		{
			_timestamp = value;
		}
	}

	public IntPtr TimestampAsIntPtr
	{
		get
		{
			return new IntPtr((int)Timestamp);
		}
		set
		{
			Timestamp = value.ToInt32();
		}
	}

	public int Message
	{
		get
		{
			int num = 0;
			num |= _status;
			num |= _data1 << 8;
			num |= _data2 << 16;
			if (StatusType == MIDIStatus.None)
			{
				num |= _data3 << 24;
			}
			return num;
		}
		set
		{
			_status = (byte)((uint)value & 0xFFu);
			_data1 = (byte)((uint)(value >> 8) & 0xFFu);
			_data2 = (byte)((uint)(value >> 16) & 0xFFu);
			if (StatusType == MIDIStatus.None)
			{
				_data3 = (byte)((uint)(value >> 24) & 0xFFu);
			}
			else
			{
				_data3 = 0;
			}
		}
	}

	public IntPtr MessageAsIntPtr
	{
		get
		{
			return new IntPtr(Message);
		}
		set
		{
			Message = value.ToInt32();
		}
	}

	public MIDIMessageType MessageType
	{
		get
		{
			if (_status >= 128 && _status <= 239)
			{
				return MIDIMessageType.Channel;
			}
			if (_status == 240 || _status == 241 || _status == 242 || _status == 243 || _status == 246 || _status == 247)
			{
				return MIDIMessageType.SystemCommon;
			}
			if (_status == 248 || _status == 249 || _status == 250 || _status == 251 || _status == 252 || _status == 254 || _status == byte.MaxValue)
			{
				return MIDIMessageType.SystemRealtime;
			}
			return MIDIMessageType.Unknown;
		}
		set
		{
			_status |= (byte)value;
		}
	}

	public MIDIStatus StatusType
	{
		get
		{
			MIDIStatus mIDIStatus = MIDIStatus.None;
			try
			{
				mIDIStatus = ((_status < 240) ? ((MIDIStatus)(_status & 0xF0u)) : ((MIDIStatus)_status));
				if (mIDIStatus == MIDIStatus.NoteOn && Velocity == 0)
				{
					mIDIStatus = MIDIStatus.NoteOff;
				}
			}
			catch
			{
				mIDIStatus = MIDIStatus.None;
			}
			return mIDIStatus;
		}
		set
		{
			if ((int)value >= 240)
			{
				_status = (byte)value;
			}
			else
			{
				_status = (byte)((uint)(value & MIDIStatus.SystemMsgs) | (_status & 0xFu));
			}
			if (value == MIDIStatus.None)
			{
				_data3 = 0;
			}
		}
	}

	public byte Channel
	{
		get
		{
			return (byte)(_status & 0xFu);
		}
		set
		{
			if (value <= 15)
			{
				_status = (byte)((_status & 0xF0u) | (value & 0xFu));
			}
		}
	}

	public byte Note
	{
		get
		{
			return (byte)(_data1 & 0x7Fu);
		}
		set
		{
			_data1 = (byte)(value & 0x7Fu);
		}
	}

	public string NoteString => $"{(MIDINote)(Note % 12)}{(byte)(Note / 12) - 2}";

	public byte Velocity
	{
		get
		{
			return (byte)(_data2 & 0x7Fu);
		}
		set
		{
			_data2 = (byte)(value & 0x7Fu);
		}
	}

	public byte Aftertouch
	{
		get
		{
			return (byte)(_data2 & 0x7Fu);
		}
		set
		{
			_data2 = (byte)(value & 0x7Fu);
		}
	}

	public byte ChannelPressure
	{
		get
		{
			return (byte)(_data1 & 0x7Fu);
		}
		set
		{
			_data1 = (byte)(value & 0x7Fu);
		}
	}

	public byte Program
	{
		get
		{
			return (byte)(_data1 & 0x7Fu);
		}
		set
		{
			_data1 = (byte)(value & 0x7Fu);
		}
	}

	public short PitchBend
	{
		get
		{
			return (short)((_data2 & 0x7F) * 128 + (_data1 & 0x7F));
		}
		set
		{
			if (value >= 0 && value <= 16383)
			{
				_data1 = (byte)(value % 128);
				_data2 = (byte)(value / 128);
			}
		}
	}

	public byte Controller
	{
		get
		{
			return (byte)(_data1 & 0x7Fu);
		}
		set
		{
			_data1 = (byte)(value & 0x7Fu);
		}
	}

	public MIDIControllerType ControllerType
	{
		get
		{
			return (MIDIControllerType)(_data1 & 0x7Fu);
		}
		set
		{
			_data1 = (byte)(value & MIDIControllerType.PolyOperation);
		}
	}

	public bool ThisIsMSB => _thisIsMSB;

	public bool PreviousIsMSB => _previousIsMSB;

	public bool IsSetAsContinuousController
	{
		get
		{
			if (StatusType != MIDIStatus.ControlChange || PreviousShortMessage == null || PreviousShortMessage.StatusType != MIDIStatus.ControlChange || PreviousShortMessage.Channel != Channel)
			{
				return false;
			}
			return _thisIsMSB != _previousIsMSB;
		}
	}

	public short ControllerValue
	{
		get
		{
			if (IsSetAsContinuousController)
			{
				if (_thisIsMSB)
				{
					return GetPairedData(_data2, PreviousShortMessage.Data2);
				}
				return GetPairedData(PreviousShortMessage.Data2, _data2);
			}
			return (short)(_data2 & 0x7F);
		}
		set
		{
			if (IsSetAsContinuousController)
			{
				if (_thisIsMSB)
				{
					_data2 = (byte)((byte)(value >> 7) & 0x7Fu);
					PreviousShortMessage.Data2 = (byte)((uint)value & 0x7Fu);
				}
				else
				{
					_data2 = (byte)((uint)value & 0x7Fu);
					PreviousShortMessage.Data2 = (byte)((byte)(value >> 7) & 0x7Fu);
				}
			}
			else
			{
				_data2 = (byte)((uint)value & 0x7Fu);
			}
		}
	}

	public MIDIMTCType TimeCodeType
	{
		get
		{
			return (MIDIMTCType)((uint)(_data1 >> 4) & 7u);
		}
		set
		{
			_data1 = (byte)((uint)(value & (MIDIMTCType)240) | (_data1 & 0xFu));
		}
	}

	public byte TimeCodeValue
	{
		get
		{
			return (byte)(_data1 & 0xFu);
		}
		set
		{
			_data1 = (byte)((_data1 & 0xF0u) | (value & 0xFu));
		}
	}

	public short SongPosition
	{
		get
		{
			return (short)((_data2 & 0x7F) * 128 + (_data1 & 0x7F));
		}
		set
		{
			if (value >= 0 && value <= 16383)
			{
				_data1 = (byte)(value % 128);
				_data2 = (byte)(value / 128);
			}
		}
	}

	public byte Song
	{
		get
		{
			return (byte)(_data1 & 0x7Fu);
		}
		set
		{
			_data1 = (byte)(value & 0x7Fu);
		}
	}

	public int Data
	{
		get
		{
			if (StatusType == MIDIStatus.None)
			{
				return (_data3 & 0x7F) * 16384 + (_data2 & 0x7F) * 128 + (_data1 & 0x7F);
			}
			return (_data2 & 0x7F) * 128 + (_data1 & 0x7F);
		}
		set
		{
			if (value >= 0 && value <= 2097151)
			{
				_data1 = (byte)(value % 128);
				_data2 = (byte)(value % 16384 / 128);
				if (StatusType == MIDIStatus.None)
				{
					_data3 = (byte)(value / 16384);
				}
			}
		}
	}

	public MidiShortMessage()
	{
		InitID();
	}

	public MidiShortMessage(IntPtr param1, IntPtr param2)
	{
		InitID();
		InitData((uint)param1.ToInt32());
		InitTimestamp((uint)param2.ToInt32());
	}

	public MidiShortMessage(IntPtr param1, IntPtr param2, MidiShortMessage previous)
	{
		InitID();
		InitData((uint)param1.ToInt32());
		InitTimestamp((uint)param2.ToInt32());
		if (previous != null && _status < 247 && StatusType == previous.StatusType)
		{
			previous.PreviousShortMessage = null;
			_previous = previous;
		}
	}

	public MidiShortMessage(MIDIStatus status, byte channel, byte data1, byte data2, long timestamp)
	{
		InitID();
		BuildMessage(status, channel, data1, data2, timestamp);
	}

	public MidiShortMessage(byte status, byte data1, byte data2, byte data3, long timestamp)
	{
		InitID();
		BuildMessage(status, data1, data2, data3, timestamp);
	}

	public MidiShortMessage(MIDIStatus status, byte channel, int data, long timestamp)
	{
		InitID();
		BuildMessage(status, channel, data, timestamp);
	}

	public MidiShortMessage(byte status, int data, long timestamp)
	{
		InitID();
		BuildMessage(status, data, timestamp);
	}

	private void InitData(uint message)
	{
		_status = (byte)(message & 0xFFu);
		_data1 = (byte)((message >> 8) & 0xFFu);
		_data2 = (byte)((message >> 16) & 0xFFu);
		if (StatusType == MIDIStatus.None)
		{
			_data3 = (byte)((message >> 24) & 0xFFu);
		}
	}

	private void InitTimestamp(uint timestamp)
	{
		_timestamp = timestamp;
	}

	private void InitID()
	{
		_myid = _id;
		if (_id == long.MaxValue)
		{
			_id = 0L;
		}
		else
		{
			_id++;
		}
	}

	public void SetContinuousController(bool thisIsMSB, bool previousIsMSB)
	{
		if (StatusType != MIDIStatus.ControlChange || PreviousShortMessage == null || PreviousShortMessage.StatusType != MIDIStatus.ControlChange || PreviousShortMessage.Channel != Channel)
		{
			_thisIsMSB = false;
			_previousIsMSB = false;
		}
		else if (thisIsMSB != previousIsMSB)
		{
			_thisIsMSB = thisIsMSB;
			_previousIsMSB = previousIsMSB;
		}
		else
		{
			_thisIsMSB = false;
			_previousIsMSB = false;
		}
	}

	public static short GetPairedData(byte dataMSB, byte dataLSB)
	{
		return (short)((dataMSB & 0x7F) * 128 + (dataLSB & 0x7F));
	}

	public static short GetPairedData1(MidiShortMessage msgMSB, MidiShortMessage msgLSB)
	{
		return (short)((msgMSB.Data1 & 0x7F) * 128 + (msgLSB.Data1 & 0x7F));
	}

	public static short GetPairedData2(MidiShortMessage msgMSB, MidiShortMessage msgLSB)
	{
		return (short)((msgMSB.Data2 & 0x7F) * 128 + (msgLSB.Data2 & 0x7F));
	}

	public void BuildMessage(MIDIStatus status, byte channel, byte data1, byte data2, long timestamp)
	{
		if (channel > 15)
		{
			channel = 15;
		}
		_status = (byte)(status + channel);
		_data1 = data1;
		_data2 = data2;
		_timestamp = timestamp;
	}

	public void BuildMessage(byte status, byte data1, byte data2, byte data3, long timestamp)
	{
		_status = status;
		_data1 = data1;
		_data2 = data2;
		_data3 = data3;
		_timestamp = timestamp;
	}

	public void BuildMessage(MIDIStatus status, byte channel, int data, long timestamp)
	{
		if (channel > 15)
		{
			channel = 15;
		}
		_status = (byte)(status + channel);
		Data = data;
		_timestamp = timestamp;
	}

	public void BuildMessage(byte status, int data, long timestamp)
	{
		_status = status;
		Data = data;
		_timestamp = timestamp;
	}

	public void BuildMessage(int message, long timestamp)
	{
		_status = (byte)((uint)message & 0xFFu);
		_data1 = (byte)((uint)(message >> 8) & 0xFFu);
		_data2 = (byte)((uint)(message >> 16) & 0xFFu);
		_data3 = (byte)((uint)(message >> 24) & 0xFFu);
		_timestamp = timestamp;
		if (StatusType != 0)
		{
			_data3 = 0;
		}
	}

	public void BuildMessage(long message, long timestamp)
	{
		_status = (byte)(message & 0xFF);
		_data1 = (byte)((message >> 8) & 0xFF);
		_data2 = (byte)((message >> 16) & 0xFF);
		_data3 = (byte)((message >> 24) & 0xFF);
		_timestamp = timestamp;
		if (StatusType != 0)
		{
			_data3 = 0;
		}
	}

	public override string ToString()
	{
		return ToString("G", null);
	}

	public string ToString(string format)
	{
		return ToString(format, null);
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		string text = format;
		if (format == null || format.Length == 0)
		{
			format = "G";
		}
		string text2 = format;
		if (format.Length == 1)
		{
			switch (format)
			{
			case "G":
				text2 = ((MessageType != MIDIMessageType.Channel) ? "{T}\t{M}\t{S}\t{D}" : "{T}\t{M} {C}\t{S}\t{D}");
				break;
			case "g":
				text2 = ((MessageType != MIDIMessageType.Channel) ? "{t}\t{m}\t{s}\t{d}" : "{t}\t{m} {C}\t{s}\t{d}");
				break;
			case "X":
				text2 = "{U} {A} {H}";
				break;
			case "x":
				text2 = "{u} {a} {h}";
				break;
			case "Y":
				text2 = "{A} {H}";
				break;
			case "y":
				text2 = "{a} {h}";
				break;
			case "T":
			case "t":
			case "U":
			case "u":
			case "M":
			case "m":
			case "S":
			case "s":
			case "A":
			case "a":
			case "C":
			case "D":
			case "d":
			case "H":
			case "h":
				text2 = format;
				break;
			default:
				text2 = string.Empty;
				break;
			}
		}
		else
		{
			text2 = format;
		}
		if (text2 != string.Empty)
		{
			text = FormatString(text2, formatProvider);
		}
		return text.Trim();
	}

	private string FormatString(string format, IFormatProvider formatProvider)
	{
		return format.Replace("{G}", (MessageType == MIDIMessageType.Channel) ? "{T}\t{M} {C}\t{S}\t{D}" : "{T}\t{M}\t{S}\t{D}").Replace("{g}", (MessageType == MIDIMessageType.Channel) ? "{t}\t{m} {C}\t{s}\t{d}" : "{t}\t{m}\t{s}\t{d}").Replace("{X}", "{U} {A} {H}")
			.Replace("{x}", "{u} {a} {h}")
			.Replace("{Y}", "{A} {H}")
			.Replace("{y}", "{a} {h}")
			.Replace("{T}", FormatTimestamp("T", formatProvider))
			.Replace("{t}", FormatTimestamp("t", formatProvider))
			.Replace("{U}", FormatTimestamp("U", formatProvider))
			.Replace("{u}", FormatTimestamp("u", formatProvider))
			.Replace("{M}", FormatMessageType("M", formatProvider))
			.Replace("{m}", FormatMessageType("m", formatProvider))
			.Replace("{S}", FormatStatus("S", formatProvider))
			.Replace("{s}", FormatStatus("s", formatProvider))
			.Replace("{A}", FormatStatus("A", formatProvider))
			.Replace("{a}", FormatStatus("a", formatProvider))
			.Replace("{C}", FormatChannel("C", formatProvider))
			.Replace("{D}", FormatData("D", formatProvider))
			.Replace("{d}", FormatData("d", formatProvider))
			.Replace("{H}", FormatData("H", formatProvider))
			.Replace("{h}", FormatData("h", formatProvider));
	}

	private string FormatData(string format, IFormatProvider formatProvider)
	{
		string result = string.Empty;
		switch (format)
		{
		case "G":
		case "D":
			switch (MessageType)
			{
			case MIDIMessageType.Channel:
				switch (StatusType)
				{
				case MIDIStatus.NoteOn:
					result = string.Format(formatProvider, "Key={0}, Velocity={1}", NoteString, Velocity);
					break;
				case MIDIStatus.NoteOff:
					result = string.Format(formatProvider, "Key={0}, Velocity={1}", NoteString, Velocity);
					break;
				case MIDIStatus.Aftertouch:
					result = string.Format(formatProvider, "Key={0}, Pressure={1}", NoteString, Aftertouch);
					break;
				case MIDIStatus.ChannelPressure:
					result = string.Format(formatProvider, "Aftertouch={0}", ChannelPressure);
					break;
				case MIDIStatus.ProgramChange:
					result = string.Format(formatProvider, "Program={0}", Program);
					break;
				case MIDIStatus.PitchBend:
					result = string.Format(formatProvider, "PitchBend={0}", PitchBend);
					break;
				case MIDIStatus.ControlChange:
					result = string.Format(formatProvider, "Controller={0}, Value={1}", ControllerType, ControllerValue);
					break;
				}
				break;
			case MIDIMessageType.SystemCommon:
				switch (StatusType)
				{
				case MIDIStatus.MidiTimeCode:
					result = string.Format(formatProvider, "Type={0}, Value={1}", TimeCodeType, TimeCodeValue);
					break;
				case MIDIStatus.SongPosition:
					result = string.Format(formatProvider, "Position={0}", SongPosition);
					break;
				case MIDIStatus.SongSelect:
					result = string.Format(formatProvider, "Song={0}", Song);
					break;
				case MIDIStatus.TuneRequest:
					result = "";
					break;
				}
				break;
			case MIDIMessageType.SystemRealtime:
				result = "";
				break;
			case MIDIMessageType.Unknown:
				result = string.Format(formatProvider, "Data1={0}, Data2={1}", Data1, Data2);
				break;
			}
			break;
		case "g":
		case "d":
			switch (MessageType)
			{
			case MIDIMessageType.Channel:
				switch (StatusType)
				{
				case MIDIStatus.NoteOn:
					result = string.Format(formatProvider, "Key={0}, Velocity={1}", Note, Velocity);
					break;
				case MIDIStatus.NoteOff:
					result = string.Format(formatProvider, "Key={0}, Velocity={1}", Note, Velocity);
					break;
				case MIDIStatus.Aftertouch:
					result = string.Format(formatProvider, "Key={0}, Pressure={1}", Note, Aftertouch);
					break;
				case MIDIStatus.ChannelPressure:
					result = string.Format(formatProvider, "Aftertouch={0}", ChannelPressure);
					break;
				case MIDIStatus.ProgramChange:
					result = string.Format(formatProvider, "Program={0}", Program);
					break;
				case MIDIStatus.PitchBend:
					result = string.Format(formatProvider, "PitchBend={0}", PitchBend);
					break;
				case MIDIStatus.ControlChange:
					result = string.Format(formatProvider, "Controller={0}, Value={1}", Controller, ControllerValue);
					break;
				}
				break;
			case MIDIMessageType.SystemCommon:
				switch (StatusType)
				{
				case MIDIStatus.MidiTimeCode:
					result = string.Format(formatProvider, "Type={0}, Value={1}", (byte)TimeCodeType, TimeCodeValue);
					break;
				case MIDIStatus.SongPosition:
					result = string.Format(formatProvider, "Position={0}", SongPosition);
					break;
				case MIDIStatus.SongSelect:
					result = string.Format(formatProvider, "Song={0}", Song);
					break;
				case MIDIStatus.TuneRequest:
					result = "";
					break;
				}
				break;
			case MIDIMessageType.SystemRealtime:
				result = "";
				break;
			case MIDIMessageType.Unknown:
				result = string.Format(formatProvider, "Data1={0}, Data2={1}", Data1, Data2);
				break;
			}
			break;
		case "X":
		case "Y":
		case "H":
			result = ((StatusType != 0) ? string.Format(formatProvider, "0x{0:X02} 0x{1:X02}", Data1, Data2) : string.Format(formatProvider, "0x{0:X02} 0x{1:X02} 0x{2:X02}", Data1, Data2, Data3));
			break;
		case "x":
		case "y":
		case "h":
			result = ((StatusType != 0) ? string.Format(formatProvider, "0x{0:x02} 0x{1:x02}", Data1, Data2) : string.Format(formatProvider, "0x{0:x02} 0x{1:x02} 0x{2:x02}", Data1, Data2, Data3));
			break;
		}
		return result;
	}

	private string FormatChannel(string format, IFormatProvider formatProvider)
	{
		string result = string.Empty;
		if (MessageType == MIDIMessageType.Channel)
		{
			switch (format)
			{
			case "G":
			case "M":
			case "C":
			case "g":
			case "m":
			case "c":
				result = string.Format(formatProvider, "{0}", Channel);
				break;
			}
		}
		return result;
	}

	private string FormatStatus(string format, IFormatProvider formatProvider)
	{
		string result = string.Empty;
		switch (format)
		{
		case "G":
		case "S":
			result = string.Format(formatProvider, "{0}", StatusType);
			break;
		case "g":
		case "s":
			result = string.Format(formatProvider, "{0}", (byte)StatusType);
			break;
		case "X":
		case "Y":
		case "A":
			result = string.Format(formatProvider, "0x{0:X02}", Status);
			break;
		case "x":
		case "y":
		case "a":
			result = string.Format(formatProvider, "0x{0:x02}", Status);
			break;
		}
		return result;
	}

	private string FormatMessageType(string format, IFormatProvider formatProvider)
	{
		string result = string.Empty;
		switch (format)
		{
		case "G":
		case "M":
			result = string.Format(formatProvider, "{0}", MessageType);
			break;
		case "g":
		case "m":
			result = string.Format(formatProvider, "{0}", (byte)MessageType);
			break;
		}
		return result;
	}

	private string FormatTimestamp(string format, IFormatProvider formatProvider)
	{
		string result = string.Empty;
		switch (format)
		{
		case "G":
		case "T":
			result = Timespan.ToString();
			break;
		case "g":
		case "t":
			result = Timestamp.ToString(formatProvider);
			break;
		case "X":
		case "U":
			result = "0x" + Timestamp.ToString("X08", formatProvider);
			break;
		case "x":
		case "u":
			result = "0x" + Timestamp.ToString("x08", formatProvider);
			break;
		}
		return result;
	}
}
