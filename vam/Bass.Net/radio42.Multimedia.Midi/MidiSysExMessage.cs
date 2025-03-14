using System;
using System.Security;
using System.Text;

namespace radio42.Multimedia.Midi;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class MidiSysExMessage
{
	private bool _input = true;

	private IntPtr _device = IntPtr.Zero;

	private IntPtr _headerPtr = IntPtr.Zero;

	private IntPtr _user = IntPtr.Zero;

	private byte[] _data;

	private static long _id;

	private long _myid;

	public long ID => _myid;

	public bool IsInput => _input;

	public IntPtr Device => _device;

	public bool IsPrepared
	{
		get
		{
			if (_headerPtr != IntPtr.Zero)
			{
				return true;
			}
			return false;
		}
	}

	public MIDIMessageType MessageType => MIDIMessageType.SystemExclusive;

	public IntPtr MessageAsIntPtr => _headerPtr;

	public byte[] Message => _data;

	public int MessageLength
	{
		get
		{
			if (_data == null)
			{
				return 0;
			}
			return _data.Length;
		}
	}

	public bool IsDone
	{
		get
		{
			if (_data == null || MessageLength < 2)
			{
				return false;
			}
			if (_data[0] == 240)
			{
				return _data[MessageLength - 1] == 247;
			}
			return false;
		}
	}

	public IntPtr User => _user;

	public MIDIStatus StatusType
	{
		get
		{
			MIDIStatus mIDIStatus = MIDIStatus.None;
			try
			{
				return (MIDIStatus)_data[0];
			}
			catch
			{
				return MIDIStatus.None;
			}
		}
	}

	public short Manufacturer
	{
		get
		{
			short num = short.MinValue;
			try
			{
				int offset = 1;
				num = Read8(ref _data, ref offset);
				if (num == 0)
				{
					num = (short)(-1 * Read16(ref _data, ref offset));
				}
			}
			catch
			{
				num = short.MinValue;
			}
			return num;
		}
	}

	public bool IsUniversalRealtime => Manufacturer == 127;

	public bool IsUniversalNonRealtime => Manufacturer == 126;

	public byte UniversalChannel
	{
		get
		{
			byte result = byte.MaxValue;
			try
			{
				int offset = 1;
				byte b = Read8(ref _data, ref offset);
				if (b == 126 || b == 127)
				{
					result = Read8(ref _data, ref offset);
				}
			}
			catch
			{
				result = byte.MaxValue;
			}
			return result;
		}
	}

	public byte UniversalSubID
	{
		get
		{
			byte result = byte.MaxValue;
			try
			{
				int offset = 1;
				byte b = Read8(ref _data, ref offset);
				if (b == 126 || b == 127)
				{
					offset = 3;
					result = Read8(ref _data, ref offset);
				}
			}
			catch
			{
				result = byte.MaxValue;
			}
			return result;
		}
	}

	public byte UniversalSubID2
	{
		get
		{
			byte result = byte.MaxValue;
			try
			{
				int offset = 1;
				byte b = Read8(ref _data, ref offset);
				if (b == 126 || b == 127)
				{
					offset = 4;
					result = Read8(ref _data, ref offset);
				}
			}
			catch
			{
				result = byte.MaxValue;
			}
			return result;
		}
	}

	public byte MMCDeviceID
	{
		get
		{
			byte result = byte.MaxValue;
			try
			{
				int offset = 1;
				if (Read8(ref _data, ref offset) == 127)
				{
					result = Read8(ref _data, ref offset);
				}
			}
			catch
			{
				result = byte.MaxValue;
			}
			return result;
		}
	}

	public byte MMCCommand
	{
		get
		{
			byte result = byte.MaxValue;
			try
			{
				int offset = 1;
				if (Read8(ref _data, ref offset) == 127)
				{
					offset = 4;
					result = Read8(ref _data, ref offset);
				}
			}
			catch
			{
				result = byte.MaxValue;
			}
			return result;
		}
	}

	public MidiSysExMessage(bool input, IntPtr handle)
	{
		InitID();
		_input = input;
		_device = handle;
	}

	public MidiSysExMessage(bool input, IntPtr handle, IntPtr headerPtr)
	{
		InitID();
		_input = input;
		_device = handle;
		MIDI_HEADER mIDI_HEADER = new MIDI_HEADER(headerPtr);
		if (mIDI_HEADER.IsDone)
		{
			_data = mIDI_HEADER.Data;
			_user = mIDI_HEADER.User;
		}
		mIDI_HEADER.Unprepare(input, handle);
	}

	public MidiSysExMessage(bool input, IntPtr handle, IntPtr headerPtr, MidiSysExMessage previous)
	{
		InitID();
		_input = input;
		_device = handle;
		MIDI_HEADER mIDI_HEADER = new MIDI_HEADER(headerPtr);
		if (mIDI_HEADER.IsDone)
		{
			_user = mIDI_HEADER.User;
			if (previous == null || previous.IsDone)
			{
				_data = mIDI_HEADER.Data;
			}
			else
			{
				byte[] data = mIDI_HEADER.Data;
				if (data == null && previous.Message != null)
				{
					_data = new byte[previous.Message.Length];
					Array.Copy(previous.Message, 0, _data, 0, previous.Message.Length);
				}
				else if (previous.Message != null)
				{
					_data = new byte[data.Length + previous.Message.Length];
					Array.Copy(previous.Message, 0, _data, 0, previous.Message.Length);
					Array.Copy(data, 0, _data, previous.Message.Length, data.Length);
				}
				else
				{
					_data = data;
				}
			}
		}
		mIDI_HEADER.Unprepare(input, handle);
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

	public bool CreateBuffer(int size)
	{
		if (size < 2 || size > 65536 || IsPrepared)
		{
			return false;
		}
		_data = new byte[size];
		return true;
	}

	public bool CreateBuffer(byte[] data)
	{
		if (data == null || data.Length < 2)
		{
			return false;
		}
		_data = new byte[data.Length];
		data.CopyTo(_data, 0);
		return true;
	}

	public bool Prepare()
	{
		return Prepare(IntPtr.Zero);
	}

	public bool Prepare(IntPtr user)
	{
		_user = user;
		MIDI_HEADER mIDI_HEADER = new MIDI_HEADER(_data);
		mIDI_HEADER.User = _user;
		if (mIDI_HEADER.Prepare(_input, _device))
		{
			_headerPtr = mIDI_HEADER.HeaderPtr;
		}
		return _headerPtr != IntPtr.Zero;
	}

	public bool Send()
	{
		if (_headerPtr == IntPtr.Zero)
		{
			return false;
		}
		MIDIError mIDIError = MIDIError.MIDI_OK;
		mIDIError = ((!_input) ? Midi.MIDI_OutLongMsg(_device, _headerPtr) : Midi.MIDI_InAddBuffer(_device, _headerPtr));
		return mIDIError == MIDIError.MIDI_OK;
	}

	public bool Validate()
	{
		return Validate(_data);
	}

	public override string ToString()
	{
		if (Message == null || MessageLength <= 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(MessageLength);
		byte[] message = Message;
		foreach (byte b in message)
		{
			stringBuilder.Append($"{b:X2} ");
		}
		return stringBuilder.ToString();
	}

	public byte MessageRead(ref int offset)
	{
		byte result = _data[offset];
		offset++;
		return result;
	}

	public void MessageWrite(ref int offset, byte value)
	{
		if (!(_headerPtr != IntPtr.Zero))
		{
			_data[offset] = value;
			offset++;
		}
	}

	public void MessageWriteSoX()
	{
		if (!(_headerPtr != IntPtr.Zero))
		{
			_data[0] = 240;
		}
	}

	public void MessageWriteEoX()
	{
		if (!(_headerPtr != IntPtr.Zero))
		{
			_data[_data.Length - 1] = 247;
		}
	}

	public byte MessageRead8(ref int offset)
	{
		return Read8(ref _data, ref offset);
	}

	public void MessageWrite8(ref int offset, byte value)
	{
		if (!(_headerPtr != IntPtr.Zero))
		{
			Write8(ref _data, ref offset, value);
		}
	}

	public byte MessageRead8Wave(ref int offset)
	{
		return Read8Wave(ref _data, ref offset);
	}

	public void MessageWrite8Wave(ref int offset, byte value)
	{
		if (!(_headerPtr != IntPtr.Zero))
		{
			Write8Wave(ref _data, ref offset, value);
		}
	}

	public short MessageRead16(ref int offset)
	{
		return Read16(ref _data, ref offset);
	}

	public void MessageWrite16(ref int offset, short value)
	{
		if (!(_headerPtr != IntPtr.Zero))
		{
			Write16(ref _data, ref offset, value);
		}
	}

	public short MessageRead16Wave(ref int offset)
	{
		return Read16Wave(ref _data, ref offset);
	}

	public void MessageWrite16Wave(ref int offset, short value)
	{
		if (!(_headerPtr != IntPtr.Zero))
		{
			Write16Wave(ref _data, ref offset, value);
		}
	}

	public int MessageRead24(ref int offset)
	{
		return Read16(ref _data, ref offset);
	}

	public void MessageWrite24(ref int offset, int value)
	{
		if (!(_headerPtr != IntPtr.Zero))
		{
			Write24(ref _data, ref offset, value);
		}
	}

	public int MessageRead24Wave(ref int offset)
	{
		return Read24Wave(ref _data, ref offset);
	}

	public void MessageWrite16Wave(ref int offset, int value)
	{
		if (!(_headerPtr != IntPtr.Zero))
		{
			Write24Wave(ref _data, ref offset, value);
		}
	}

	public int MessageRead32(ref int offset)
	{
		return Read16(ref _data, ref offset);
	}

	public void MessageWrite32(ref int offset, int value)
	{
		if (!(_headerPtr != IntPtr.Zero))
		{
			Write32(ref _data, ref offset, value);
		}
	}

	private byte Read8(ref byte[] data, ref int offset)
	{
		byte result = (byte)(data[offset] & 0x7F);
		offset++;
		return result;
	}

	private void Write8(ref byte[] data, ref int offset, byte value)
	{
		data[offset] = (byte)(value & 0x7Fu);
		offset++;
	}

	private short Read16(ref byte[] data, ref int offset)
	{
		short result = (short)((data[offset] & 0x7F) * 128 + (_data[offset + 1] & 0x7F));
		offset += 2;
		return result;
	}

	private void Write16(ref byte[] data, ref int offset, short value)
	{
		data[offset] = (byte)((uint)(value >> 7) & 0x7Fu);
		data[offset + 1] = (byte)((uint)value & 0x7Fu);
		offset += 2;
	}

	private int Read24(ref byte[] data, ref int offset)
	{
		int result = (data[offset] & 0x7F) * 16384 + (data[offset + 1] & 0x7F) * 128 + (_data[offset + 2] & 0x7F);
		offset += 3;
		return result;
	}

	private void Write24(ref byte[] data, ref int offset, int value)
	{
		data[offset] = (byte)((uint)(value >> 14) & 0x7Fu);
		data[offset + 1] = (byte)((uint)(value >> 7) & 0x7Fu);
		data[offset + 2] = (byte)((uint)value & 0x7Fu);
		offset += 3;
	}

	private int Read32(ref byte[] data, ref int offset)
	{
		int result = (data[offset] & 0x7F) * 2097152 + (data[offset + 1] & 0x7F) * 16384 + (data[offset + 2] & 0x7F) * 128 + (_data[offset + 3] & 0x7F);
		offset += 4;
		return result;
	}

	private void Write32(ref byte[] data, ref int offset, int value)
	{
		data[offset] = (byte)((uint)(value >> 21) & 0x7Fu);
		data[offset + 1] = (byte)((uint)(value >> 14) & 0x7Fu);
		data[offset + 2] = (byte)((uint)(value >> 7) & 0x7Fu);
		data[offset + 3] = (byte)((uint)value & 0x7Fu);
		offset += 4;
	}

	private byte Read8Wave(ref byte[] data, ref int offset)
	{
		int num = (data[offset] << 1) | (data[offset + 1] >> 6);
		offset += 2;
		return (byte)num;
	}

	private void Write8Wave(ref byte[] data, ref int offset, byte value)
	{
		data[offset] = (byte)((uint)(value >> 1) & 0x7Fu);
		data[offset + 1] = (byte)((uint)(value << 6) & 0x7Fu);
		offset += 2;
	}

	private short Read16Wave(ref byte[] data, ref int offset)
	{
		int num = ((data[offset] << 9) | (data[offset + 1] << 2) | (data[offset] >> 5)) - 32768;
		offset += 3;
		return (short)num;
	}

	private void Write16Wave(ref byte[] data, ref int offset, short value)
	{
		data[offset] = (byte)((uint)(value >> 9) & 0x7Fu);
		data[offset + 1] = (byte)((uint)(value >> 2) & 0x7Fu);
		data[offset + 2] = (byte)((uint)(value << 5) & 0x7Fu);
		offset += 3;
	}

	private int Read24Wave(ref byte[] data, ref int offset)
	{
		int result = ((data[offset] << 17) | (data[offset + 1] << 10) | (data[offset] << 3) | (data[offset] >> 4)) - 8388608;
		offset += 4;
		return result;
	}

	private void Write24Wave(ref byte[] data, ref int offset, int value)
	{
		data[offset] = (byte)((uint)(value >> 17) & 0x7Fu);
		data[offset + 1] = (byte)((uint)(value >> 10) & 0x7Fu);
		data[offset + 2] = (byte)((uint)(value >> 3) & 0x7Fu);
		data[offset + 3] = (byte)((uint)(value << 4) & 0x7Fu);
		offset += 4;
	}

	private bool Validate(byte[] data)
	{
		if (data == null)
		{
			return false;
		}
		int num = data.Length;
		if (data[0] != 240 || data[num - 1] != 247)
		{
			return false;
		}
		num--;
		bool result = true;
		for (int num2 = 1; num2 < num; num2--)
		{
			if ((data[num2] & 0x80u) != 0)
			{
				result = false;
				break;
			}
		}
		return result;
	}
}
