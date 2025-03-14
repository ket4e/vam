using System;
using System.Runtime.InteropServices;
using System.Text;

namespace radio42.Multimedia.Midi;

[Serializable]
public sealed class MIDI_HEADER
{
	private byte[] _data;

	private IntPtr _user = IntPtr.Zero;

	private MIDIHeader _flags;

	private IntPtr _headerPtr = IntPtr.Zero;

	public byte[] Data
	{
		get
		{
			return _data;
		}
		set
		{
			if (!IsPrepared)
			{
				_data = value;
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
			if (!IsPrepared)
			{
				_user = value;
			}
		}
	}

	public MIDIHeader Flags => _flags;

	public IntPtr HeaderPtr => _headerPtr;

	public bool IsDone => (_flags & MIDIHeader.MHDR_DONE) != 0;

	public bool IsPrepared => (_flags & MIDIHeader.MHDR_PREPARED) != 0;

	public bool IsStreamBuffer => (_flags & MIDIHeader.MHDR_ISSTRM) != 0;

	public MIDI_HEADER(int size)
	{
		if (size >= 2 && size <= 65536)
		{
			_data = new byte[size];
		}
		else
		{
			_data = new byte[256];
		}
	}

	public MIDI_HEADER(byte[] data)
	{
		_data = data;
	}

	public MIDI_HEADER(string data)
	{
		int length = data.Length;
		_data = new byte[length + 1];
		for (int i = 0; i < length; i++)
		{
			_data[i] = (byte)data[i];
		}
		_data[length] = 0;
	}

	public MIDI_HEADER(IntPtr headerPtr)
	{
		if (!(headerPtr == IntPtr.Zero))
		{
			_headerPtr = headerPtr;
			MIDIHDR mIDIHDR = null;
			try
			{
				mIDIHDR = (MIDIHDR)Marshal.PtrToStructure(headerPtr, typeof(MIDIHDR));
			}
			catch
			{
				mIDIHDR = null;
			}
			if (mIDIHDR != null)
			{
				_data = mIDIHDR.GetData();
				_flags = mIDIHDR.flags;
				_user = mIDIHDR.user;
			}
		}
	}

	public override string ToString()
	{
		if (Data == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(Data.Length);
		byte[] data = Data;
		foreach (byte b in data)
		{
			stringBuilder.Append($"{b:X2} ");
		}
		return stringBuilder.ToString();
	}

	public bool Prepare(bool input, IntPtr handle)
	{
		if (_headerPtr != IntPtr.Zero)
		{
			Unprepare(input, handle);
		}
		IntPtr headerPtr = IntPtr.Zero;
		MIDIHDR header = new MIDIHDR();
		if (InitData(_data, header))
		{
			headerPtr = AllocHeader(header);
			if (!PrepareHeader(input, handle, headerPtr))
			{
				headerPtr = IntPtr.Zero;
			}
		}
		_headerPtr = headerPtr;
		return _headerPtr != IntPtr.Zero;
	}

	public void Unprepare(bool input, IntPtr handle)
	{
		UnprepareHeader(input, handle, _headerPtr);
	}

	private IntPtr AllocHeader(MIDIHDR header)
	{
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MIDIHDR)));
		}
		catch (Exception)
		{
			try
			{
				if (header.data != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(header.data);
				}
			}
			catch
			{
			}
			header.data = IntPtr.Zero;
		}
		try
		{
			Marshal.StructureToPtr(header, intPtr, fDeleteOld: true);
		}
		catch (Exception)
		{
			try
			{
				if (header.data != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(header.data);
				}
			}
			catch
			{
			}
			header.data = IntPtr.Zero;
			try
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			catch
			{
			}
			intPtr = IntPtr.Zero;
		}
		return intPtr;
	}

	private bool InitData(byte[] buffer, MIDIHDR header)
	{
		if (buffer == null || header == null)
		{
			return false;
		}
		try
		{
			header.Reset();
			header.bufferLength = buffer.Length;
			header.data = Marshal.AllocHGlobal(buffer.Length);
			if (header.data != IntPtr.Zero)
			{
				for (int i = 0; i < buffer.Length; i++)
				{
					Marshal.WriteByte(header.data, i, buffer[i]);
				}
			}
		}
		catch
		{
			header.Reset();
		}
		return header.data != IntPtr.Zero;
	}

	private bool PrepareHeader(bool input, IntPtr handle, IntPtr headerPtr)
	{
		MIDIError mIDIError = MIDIError.MIDI_OK;
		if (headerPtr != IntPtr.Zero)
		{
			mIDIError = ((!input) ? Midi.MIDI_OutPrepareHeader(handle, headerPtr) : Midi.MIDI_InPrepareHeader(handle, headerPtr));
		}
		if (mIDIError != 0)
		{
			MIDIHDR mIDIHDR = null;
			try
			{
				mIDIHDR = (MIDIHDR)Marshal.PtrToStructure(headerPtr, typeof(MIDIHDR));
			}
			catch
			{
			}
			if (mIDIHDR != null)
			{
				try
				{
					if (mIDIHDR.data != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(mIDIHDR.data);
					}
				}
				catch
				{
				}
				mIDIHDR.data = IntPtr.Zero;
				try
				{
					if (headerPtr != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(headerPtr);
					}
				}
				catch
				{
				}
			}
		}
		return mIDIError == MIDIError.MIDI_OK;
	}

	private void UnprepareHeader(bool input, IntPtr handle, IntPtr headerPtr)
	{
		if (headerPtr == IntPtr.Zero)
		{
			return;
		}
		MIDIHDR mIDIHDR = null;
		try
		{
			mIDIHDR = (MIDIHDR)Marshal.PtrToStructure(headerPtr, typeof(MIDIHDR));
		}
		catch
		{
		}
		if (mIDIHDR == null)
		{
			return;
		}
		try
		{
			if (input)
			{
				Midi.MIDI_InUnprepareHeader(handle, headerPtr);
			}
			else
			{
				Midi.MIDI_OutUnprepareHeader(handle, headerPtr);
			}
		}
		catch
		{
		}
		try
		{
			if (mIDIHDR.data != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(mIDIHDR.data);
			}
		}
		catch
		{
		}
		try
		{
			Marshal.FreeHGlobal(headerPtr);
		}
		catch
		{
		}
	}
}
