using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class DSP_BufferStream : BaseDSP
{
	private byte[] _buffer;

	private int _configBuffer = 500;

	private int _bufferLength;

	private STREAMPROC _streamProc;

	private int _bufferStream;

	private bool _isOutputBuffered = true;

	private BASSFlag _bufferStreamFlags = BASSFlag.BASS_STREAM_DECODE;

	private int _outputHandle;

	private volatile int _lastPos;

	public int BufferStream => _bufferStream;

	public BASSFlag BufferStreamFlags => _bufferStreamFlags;

	public int OutputHandle
	{
		get
		{
			return _outputHandle;
		}
		set
		{
			_outputHandle = value;
		}
	}

	public bool IsOutputBuffered
	{
		get
		{
			return _isOutputBuffered;
		}
		set
		{
			lock (this)
			{
				_isOutputBuffered = value;
				_lastPos = ((!_isOutputBuffered) ? _bufferLength : 0);
			}
		}
	}

	public int ConfigBuffer
	{
		get
		{
			return _configBuffer;
		}
		set
		{
			if (value > 5000)
			{
				_configBuffer = 5000;
			}
			else if (value < 1)
			{
				_configBuffer = 1;
			}
			else
			{
				_configBuffer = value;
			}
			OnStopped();
			if ((base.ChannelInfo.flags & BASSFlag.BASS_SAMPLE_FLOAT) != 0)
			{
				_bufferLength = (int)Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, (double)_configBuffer / 1000.0);
			}
			else if ((base.ChannelInfo.flags & BASSFlag.BASS_SAMPLE_8BITS) != 0 && base.ChannelBitwidth == 32)
			{
				_bufferLength = (int)Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, (double)_configBuffer / 1000.0) * 4;
			}
			else if (base.ChannelBitwidth == 32)
			{
				_bufferLength = (int)Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, (double)_configBuffer / 1000.0) * 2;
			}
			if (base.IsAssigned)
			{
				OnStarted();
			}
		}
	}

	public int ConfigBufferLength => _bufferLength;

	public int BufferPosition
	{
		get
		{
			return _lastPos;
		}
		set
		{
			_lastPos = value;
		}
	}

	public DSP_BufferStream()
	{
		ConfigBuffer = Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_BUFFER);
		_streamProc = BassStreamProc;
	}

	public DSP_BufferStream(int channel, int priority)
		: base(channel, priority, IntPtr.Zero)
	{
		ConfigBuffer = Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_BUFFER);
		_streamProc = BassStreamProc;
	}

	public void ClearBuffer()
	{
		if (_buffer == null)
		{
			return;
		}
		lock (this)
		{
			Array.Clear(_buffer, 0, _bufferLength);
			_lastPos = ((!_isOutputBuffered) ? _bufferLength : 0);
		}
	}

	public override void OnChannelChanged()
	{
		OnStopped();
		if ((base.ChannelInfo.flags & BASSFlag.BASS_SAMPLE_FLOAT) != 0)
		{
			_bufferLength = (int)Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, (double)_configBuffer / 1000.0);
		}
		else if ((base.ChannelInfo.flags & BASSFlag.BASS_SAMPLE_8BITS) != 0 && base.ChannelBitwidth == 32)
		{
			_bufferLength = (int)Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, (double)_configBuffer / 1000.0) * 4;
		}
		else if (base.ChannelBitwidth == 32)
		{
			_bufferLength = (int)Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, (double)_configBuffer / 1000.0) * 2;
		}
		_bufferStreamFlags = base.ChannelInfo.flags | BASSFlag.BASS_STREAM_DECODE;
		_bufferStreamFlags &= ~BASSFlag.BASS_STREAM_AUTOFREE;
		if (base.ChannelBitwidth == 32)
		{
			_bufferStreamFlags &= ~BASSFlag.BASS_SAMPLE_8BITS;
			_bufferStreamFlags |= BASSFlag.BASS_SAMPLE_FLOAT;
		}
		else if (base.ChannelBitwidth == 8)
		{
			_bufferStreamFlags &= ~BASSFlag.BASS_SAMPLE_FLOAT;
			_bufferStreamFlags |= BASSFlag.BASS_SAMPLE_8BITS;
		}
		else
		{
			_bufferStreamFlags &= ~BASSFlag.BASS_SAMPLE_FLOAT;
			_bufferStreamFlags &= ~BASSFlag.BASS_SAMPLE_8BITS;
		}
		if (base.IsAssigned)
		{
			OnStarted();
		}
	}

	public override void OnBypassChanged()
	{
		ClearBuffer();
	}

	public override void OnStarted()
	{
		_buffer = new byte[_bufferLength];
		lock (this)
		{
			_bufferStream = Bass.BASS_StreamCreate(base.ChannelSampleRate, base.ChannelNumChans, _bufferStreamFlags, _streamProc, IntPtr.Zero);
			_lastPos = ((!_isOutputBuffered) ? _bufferLength : 0);
		}
	}

	public override void OnStopped()
	{
		if (_bufferStream != 0)
		{
			Bass.BASS_StreamFree(_bufferStream);
			_bufferStream = 0;
		}
		_buffer = null;
	}

	public override void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (base.IsBypassed)
		{
			return;
		}
		if (length > _bufferLength)
		{
			length = _bufferLength;
		}
		lock (this)
		{
			Array.Copy(_buffer, length, _buffer, 0, _bufferLength - length);
			Marshal.Copy(buffer, _buffer, _bufferLength - length, length);
			_lastPos -= length;
			if (_lastPos < 0)
			{
				_lastPos = 0;
			}
		}
	}

	private int BassStreamProc(int handle, IntPtr buffer, int length, IntPtr user)
	{
		if (base.IsBypassed)
		{
			return 0;
		}
		if (OutputHandle != 0)
		{
			int num = Bass.BASS_ChannelGetData(OutputHandle, IntPtr.Zero, 0);
			num = (int)Bass.BASS_ChannelSeconds2Bytes(handle, Bass.BASS_ChannelBytes2Seconds(OutputHandle, num));
			if (num > _bufferLength)
			{
				num = _bufferLength;
			}
			else if (num < 0)
			{
				num = 0;
			}
			if (length > num)
			{
				length = num;
			}
			lock (this)
			{
				Marshal.Copy(_buffer, _bufferLength - num, buffer, length);
			}
		}
		else
		{
			if (_lastPos + length > _bufferLength)
			{
				length = _bufferLength - _lastPos;
			}
			lock (this)
			{
				Marshal.Copy(_buffer, _lastPos, buffer, length);
				_lastPos += length;
				if (_lastPos > _bufferLength)
				{
					_lastPos = _bufferLength;
				}
			}
		}
		return length;
	}

	public override string ToString()
	{
		return "Buffer Stream DSP";
	}
}
