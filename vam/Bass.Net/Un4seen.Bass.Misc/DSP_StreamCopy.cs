using System;
using System.Security;
using Un4seen.Bass.AddOn.Mix;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class DSP_StreamCopy : BaseDSP
{
	private int _streamCopy;

	private int _sourceMixerStream;

	private bool _isSourceMixerNonstop;

	private int _targetMixerStream;

	private bool _isTargetMixerNonstop;

	private bool _isTargetMixerImmediate;

	private int _streamCopyDelay;

	private BASSFlag _streamCopyFlags;

	private int _streamCopyDevice = Bass.BASS_GetDevice();

	private bool _isOutputBuffered = true;

	private int _outputLatency;

	public int StreamCopy => _streamCopy;

	public BASSFlag StreamCopyFlags
	{
		get
		{
			return _streamCopyFlags;
		}
		set
		{
			OnStopped();
			_streamCopyFlags = value;
			if (base.IsAssigned)
			{
				OnStarted();
			}
		}
	}

	public int StreamCopyDevice
	{
		get
		{
			return _streamCopyDevice;
		}
		set
		{
			OnStopped();
			_streamCopyDevice = value;
			if (base.IsAssigned)
			{
				OnStarted();
			}
		}
	}

	public int OutputLatency
	{
		get
		{
			return _outputLatency;
		}
		set
		{
			if (value < 0)
			{
				_outputLatency = 0;
			}
			else
			{
				_outputLatency = value;
			}
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
			OnStopped();
			_isOutputBuffered = value;
			if (base.IsAssigned)
			{
				OnStarted();
			}
		}
	}

	public int SourceMixerStream
	{
		get
		{
			return _sourceMixerStream;
		}
		set
		{
			OnStopped();
			_sourceMixerStream = value;
			_isSourceMixerNonstop = (Bass.BASS_ChannelFlags(_sourceMixerStream, BASSFlag.BASS_STREAM_PRESCAN, BASSFlag.BASS_DEFAULT) & BASSFlag.BASS_STREAM_PRESCAN) != 0;
			if (base.IsAssigned)
			{
				OnStarted();
			}
		}
	}

	public int TargetMixerStream
	{
		get
		{
			return _targetMixerStream;
		}
		set
		{
			OnStopped();
			_targetMixerStream = value;
			BASSFlag bASSFlag = Bass.BASS_ChannelFlags(_targetMixerStream, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_DEFAULT);
			_isTargetMixerNonstop = (bASSFlag & BASSFlag.BASS_STREAM_PRESCAN) != 0;
			_isTargetMixerImmediate = (bASSFlag & BASSFlag.BASS_AAC_FRAME960) != 0;
			if (_targetMixerStream != 0)
			{
				_streamCopyFlags |= BASSFlag.BASS_STREAM_DECODE;
			}
			if (base.IsAssigned)
			{
				OnStarted();
			}
		}
	}

	public DSP_StreamCopy()
	{
	}

	public DSP_StreamCopy(int channel, int priority)
		: base(channel, priority, IntPtr.Zero)
	{
	}

	public void ReSync()
	{
		if (!base.IsAssigned || !_isOutputBuffered || TargetMixerStream == 0 || SourceMixerStream == 0)
		{
			return;
		}
		Bass.BASS_ChannelLock(SourceMixerStream, state: true);
		int num = Bass.BASS_ChannelGetData(TargetMixerStream, IntPtr.Zero, 0);
		if (num > 0)
		{
			num = (int)Bass.BASS_ChannelSeconds2Bytes(_streamCopy, Bass.BASS_ChannelBytes2Seconds(TargetMixerStream, num));
			if (!_isSourceMixerNonstop && _isTargetMixerNonstop)
			{
				_streamCopyDelay = num;
				BassMix.BASS_Mixer_ChannelSetPosition(_streamCopy, 0L);
			}
			else if (!_isTargetMixerNonstop)
			{
				int num2 = Bass.BASS_ChannelGetData(SourceMixerStream, IntPtr.Zero, 0);
				num2 = (int)Bass.BASS_ChannelSeconds2Bytes(_streamCopy, Bass.BASS_ChannelBytes2Seconds(SourceMixerStream, num2));
				if (num2 > num)
				{
					byte[] buffer = new byte[num2 - num];
					Bass.BASS_StreamPutData(_streamCopy, buffer, num2 - num);
				}
			}
		}
		else if (_isSourceMixerNonstop)
		{
			int num3 = Bass.BASS_ChannelGetData(SourceMixerStream, IntPtr.Zero, 0);
			num3 = (int)Bass.BASS_ChannelSeconds2Bytes(_streamCopy, Bass.BASS_ChannelBytes2Seconds(SourceMixerStream, num3));
			int num4 = (int)Bass.BASS_ChannelSeconds2Bytes(_streamCopy, (double)_outputLatency / 1000.0);
			if (num3 > num4)
			{
				BassMix.BASS_Mixer_ChannelSetPosition(_streamCopy, 0L);
				byte[] buffer2 = new byte[num3 - num4];
				Bass.BASS_StreamPutData(_streamCopy, buffer2, num3 - num4);
			}
		}
		if (_isTargetMixerImmediate && !_isTargetMixerNonstop)
		{
			Bass.BASS_ChannelUpdate(TargetMixerStream, 0);
		}
		Bass.BASS_ChannelLock(SourceMixerStream, state: false);
	}

	public override void OnChannelChanged()
	{
		OnStopped();
		if (base.IsAssigned)
		{
			OnStarted();
		}
	}

	public override void OnBypassChanged()
	{
		if (base.IsBypassed)
		{
			if (_isOutputBuffered && !base.ChannelInfo.IsDecodingChannel)
			{
				Bass.BASS_ChannelPause(_streamCopy);
				Bass.BASS_ChannelSetPosition(_streamCopy, 0L);
			}
			return;
		}
		int streamCopy = _streamCopy;
		_streamCopy = 0;
		if (SourceMixerStream == 0)
		{
			Bass.BASS_ChannelLock(base.ChannelHandle, state: true);
		}
		else
		{
			Bass.BASS_ChannelLock(SourceMixerStream, state: true);
		}
		if (_isOutputBuffered)
		{
			if (SourceMixerStream != 0)
			{
				if (TargetMixerStream != 0)
				{
					if (base.ChannelInfo.IsDecodingChannel && (BassMix.BASS_Mixer_ChannelFlags(base.ChannelHandle, BASSFlag.BASS_RECORD_ECHOCANCEL, BASSFlag.BASS_DEFAULT) & BASSFlag.BASS_RECORD_ECHOCANCEL) != 0)
					{
						int num = BassMix.BASS_Mixer_ChannelGetData(base.ChannelHandle, IntPtr.Zero, 0);
						int num2 = Bass.BASS_ChannelGetData(TargetMixerStream, IntPtr.Zero, 0);
						num2 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, Bass.BASS_ChannelBytes2Seconds(TargetMixerStream, num2));
						if (num2 > 0)
						{
							num -= num2;
						}
						if (num > 0)
						{
							int num3 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, (double)_outputLatency / 1000.0);
							if (num2 > 0)
							{
								num3 = 0;
							}
							byte[] array = new byte[num];
							num = BassMix.BASS_Mixer_ChannelGetData(base.ChannelHandle, array, num);
							if (num > num3)
							{
								if (num3 > 0)
								{
									Array.Copy(array, num3, array, 0, num - num3);
								}
								Bass.BASS_StreamPutData(streamCopy, array, num - num3);
							}
						}
					}
					else
					{
						int num4 = Bass.BASS_ChannelGetData(SourceMixerStream, IntPtr.Zero, 0);
						num4 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, Bass.BASS_ChannelBytes2Seconds(SourceMixerStream, num4));
						int num5 = Bass.BASS_ChannelGetData(TargetMixerStream, IntPtr.Zero, 0);
						num5 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, Bass.BASS_ChannelBytes2Seconds(TargetMixerStream, num5));
						num4 -= num5;
						if (num4 > 0)
						{
							int num6 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, (double)_outputLatency / 1000.0);
							if (num5 > 0)
							{
								num6 = 0;
							}
							byte[] array2 = new byte[num4];
							if (!base.ChannelInfo.IsDecodingChannel)
							{
								num4 = Bass.BASS_ChannelGetData(SourceMixerStream, array2, num4);
								num4 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, Bass.BASS_ChannelBytes2Seconds(SourceMixerStream, num4));
							}
							if (num4 > num6)
							{
								if (num6 > 0)
								{
									Array.Copy(array2, num6, array2, 0, num4 - num6);
								}
								Bass.BASS_StreamPutData(streamCopy, array2, num4 - num6);
							}
						}
					}
				}
				else if (base.ChannelInfo.IsDecodingChannel && (BassMix.BASS_Mixer_ChannelFlags(base.ChannelHandle, BASSFlag.BASS_RECORD_ECHOCANCEL, BASSFlag.BASS_DEFAULT) & BASSFlag.BASS_RECORD_ECHOCANCEL) != 0)
				{
					int num7 = BassMix.BASS_Mixer_ChannelGetData(base.ChannelHandle, IntPtr.Zero, 0);
					if (num7 > 0)
					{
						int num8 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, (double)_outputLatency / 1000.0);
						byte[] array3 = new byte[num7];
						num7 = BassMix.BASS_Mixer_ChannelGetData(base.ChannelHandle, array3, num7);
						if (num7 > num8)
						{
							if (num8 > 0)
							{
								Array.Copy(array3, num8, array3, 0, num7 - num8);
							}
							Bass.BASS_StreamPutData(streamCopy, array3, num7 - num8);
						}
					}
				}
				else
				{
					int num9 = Bass.BASS_ChannelGetData(SourceMixerStream, IntPtr.Zero, 0);
					num9 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, Bass.BASS_ChannelBytes2Seconds(SourceMixerStream, num9));
					if (num9 > 0)
					{
						int num10 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, (double)_outputLatency / 1000.0);
						byte[] array4 = new byte[num9];
						if (!base.ChannelInfo.IsDecodingChannel)
						{
							num9 = Bass.BASS_ChannelGetData(SourceMixerStream, array4, num9);
							num9 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, Bass.BASS_ChannelBytes2Seconds(SourceMixerStream, num9));
						}
						if (num9 > num10)
						{
							if (num10 > 0)
							{
								Array.Copy(array4, num10, array4, 0, num9 - num10);
							}
							Bass.BASS_StreamPutData(streamCopy, array4, num9 - num10);
						}
					}
				}
			}
			else if (!base.ChannelInfo.IsDecodingChannel)
			{
				int num11 = (int)Bass.BASS_ChannelSeconds2Bytes(streamCopy, (double)_outputLatency / 1000.0);
				int num12 = Bass.BASS_ChannelGetData(base.ChannelHandle, IntPtr.Zero, 0);
				byte[] array5 = new byte[num12];
				num12 = Bass.BASS_ChannelGetData(base.ChannelHandle, array5, num12);
				if (num12 > num11)
				{
					if (num11 > 0)
					{
						Array.Copy(array5, num11, array5, 0, num12 - num11);
					}
					Bass.BASS_StreamPutData(streamCopy, array5, num12 - num11);
				}
			}
		}
		_streamCopy = streamCopy;
		if (TargetMixerStream != 0 && !_isTargetMixerImmediate && !_isTargetMixerNonstop)
		{
			Bass.BASS_ChannelUpdate(TargetMixerStream, 0);
		}
		if ((!base.ChannelInfo.IsDecodingChannel || SourceMixerStream != 0) && Bass.BASS_ChannelIsActive(base.ChannelHandle) == BASSActive.BASS_ACTIVE_PLAYING)
		{
			Bass.BASS_ChannelPlay(_streamCopy, restart: false);
		}
		if (SourceMixerStream == 0)
		{
			Bass.BASS_ChannelLock(base.ChannelHandle, state: false);
		}
		else
		{
			Bass.BASS_ChannelLock(SourceMixerStream, state: false);
		}
	}

	public override void OnStarted()
	{
		int num = Bass.BASS_GetDevice();
		if (num != _streamCopyDevice)
		{
			Bass.BASS_SetDevice(_streamCopyDevice);
		}
		if (base.ChannelBitwidth == 32)
		{
			_streamCopyFlags &= ~BASSFlag.BASS_SAMPLE_8BITS;
			_streamCopyFlags |= BASSFlag.BASS_SAMPLE_FLOAT;
		}
		else if (base.ChannelBitwidth == 8)
		{
			_streamCopyFlags &= ~BASSFlag.BASS_SAMPLE_FLOAT;
			_streamCopyFlags |= BASSFlag.BASS_SAMPLE_8BITS;
		}
		else
		{
			_streamCopyFlags &= ~BASSFlag.BASS_SAMPLE_FLOAT;
			_streamCopyFlags &= ~BASSFlag.BASS_SAMPLE_8BITS;
		}
		int num2 = Bass.BASS_StreamCreatePush(base.ChannelSampleRate, base.ChannelNumChans, _streamCopyFlags, IntPtr.Zero);
		_streamCopyDelay = 0;
		if (SourceMixerStream == 0)
		{
			Bass.BASS_ChannelLock(base.ChannelHandle, state: true);
		}
		else
		{
			Bass.BASS_ChannelLock(SourceMixerStream, state: true);
		}
		if (_isOutputBuffered)
		{
			if (SourceMixerStream != 0)
			{
				if (TargetMixerStream != 0)
				{
					if (base.ChannelInfo.IsDecodingChannel && (BassMix.BASS_Mixer_ChannelFlags(base.ChannelHandle, BASSFlag.BASS_RECORD_ECHOCANCEL, BASSFlag.BASS_DEFAULT) & BASSFlag.BASS_RECORD_ECHOCANCEL) != 0)
					{
						int num3 = BassMix.BASS_Mixer_ChannelGetData(base.ChannelHandle, IntPtr.Zero, 0);
						int num4 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, Bass.BASS_ChannelBytes2Seconds(pos: Bass.BASS_ChannelGetData(TargetMixerStream, IntPtr.Zero, 0), handle: TargetMixerStream));
						if (num4 > 0)
						{
							num3 -= num4;
						}
						if (num3 > 0)
						{
							int num5 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, (double)_outputLatency / 1000.0);
							if (num4 > 0)
							{
								num5 = 0;
							}
							byte[] array = new byte[num3];
							num3 = BassMix.BASS_Mixer_ChannelGetData(base.ChannelHandle, array, num3);
							if (num3 > num5)
							{
								if (num5 > 0)
								{
									Array.Copy(array, num5, array, 0, num3 - num5);
								}
								Bass.BASS_StreamPutData(num2, array, num3 - num5);
							}
						}
					}
					else
					{
						int num6 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, Bass.BASS_ChannelBytes2Seconds(pos: Bass.BASS_ChannelGetData(SourceMixerStream, IntPtr.Zero, 0), handle: SourceMixerStream));
						int num7 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, Bass.BASS_ChannelBytes2Seconds(pos: Bass.BASS_ChannelGetData(TargetMixerStream, IntPtr.Zero, 0), handle: TargetMixerStream));
						num6 -= num7;
						if (num6 > 0)
						{
							int num8 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, (double)_outputLatency / 1000.0);
							if (num7 > 0)
							{
								num8 = 0;
							}
							byte[] array2 = new byte[num6];
							if (!base.ChannelInfo.IsDecodingChannel)
							{
								num6 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, Bass.BASS_ChannelBytes2Seconds(pos: Bass.BASS_ChannelGetData(SourceMixerStream, array2, num6), handle: SourceMixerStream));
							}
							if (num6 > num8)
							{
								if (num8 > 0)
								{
									Array.Copy(array2, num8, array2, 0, num6 - num8);
								}
								Bass.BASS_StreamPutData(num2, array2, num6 - num8);
							}
						}
					}
				}
				else if (base.ChannelInfo.IsDecodingChannel && (BassMix.BASS_Mixer_ChannelFlags(base.ChannelHandle, BASSFlag.BASS_RECORD_ECHOCANCEL, BASSFlag.BASS_DEFAULT) & BASSFlag.BASS_RECORD_ECHOCANCEL) != 0)
				{
					int num9 = BassMix.BASS_Mixer_ChannelGetData(base.ChannelHandle, IntPtr.Zero, 0);
					if (num9 > 0)
					{
						int num10 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, (double)_outputLatency / 1000.0);
						byte[] array3 = new byte[num9];
						num9 = BassMix.BASS_Mixer_ChannelGetData(base.ChannelHandle, array3, num9);
						if (num9 > num10)
						{
							if (num10 > 0)
							{
								Array.Copy(array3, num10, array3, 0, num9 - num10);
							}
							Bass.BASS_StreamPutData(num2, array3, num9 - num10);
						}
					}
				}
				else
				{
					int num11 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, Bass.BASS_ChannelBytes2Seconds(pos: Bass.BASS_ChannelGetData(SourceMixerStream, IntPtr.Zero, 0), handle: SourceMixerStream));
					if (num11 > 0)
					{
						int num12 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, (double)_outputLatency / 1000.0);
						byte[] array4 = new byte[num11];
						if (!base.ChannelInfo.IsDecodingChannel)
						{
							num11 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, Bass.BASS_ChannelBytes2Seconds(pos: Bass.BASS_ChannelGetData(SourceMixerStream, array4, num11), handle: SourceMixerStream));
						}
						if (num11 > num12)
						{
							if (num12 > 0)
							{
								Array.Copy(array4, num12, array4, 0, num11 - num12);
							}
							Bass.BASS_StreamPutData(num2, array4, num11 - num12);
						}
					}
				}
			}
			else if (!base.ChannelInfo.IsDecodingChannel)
			{
				int num13 = (int)Bass.BASS_ChannelSeconds2Bytes(num2, (double)_outputLatency / 1000.0);
				int num14 = Bass.BASS_ChannelGetData(base.ChannelHandle, IntPtr.Zero, 0);
				byte[] array5 = new byte[num14];
				num14 = Bass.BASS_ChannelGetData(base.ChannelHandle, array5, num14);
				if (num14 > num13)
				{
					if (num13 > 0)
					{
						Array.Copy(array5, num13, array5, 0, num14 - num13);
					}
					Bass.BASS_StreamPutData(num2, array5, num14 - num13);
				}
			}
		}
		_streamCopy = num2;
		if (TargetMixerStream != 0 && (_streamCopyFlags & BASSFlag.BASS_STREAM_DECODE) != 0)
		{
			BassMix.BASS_Mixer_StreamAddChannel(TargetMixerStream, _streamCopy, ((BassMix.BASS_Mixer_ChannelFlags(base.ChannelHandle, BASSFlag.BASS_RECORD_ECHOCANCEL, BASSFlag.BASS_DEFAULT) & BASSFlag.BASS_RECORD_ECHOCANCEL) != 0) ? BASSFlag.BASS_RECORD_ECHOCANCEL : BASSFlag.BASS_DEFAULT);
			if (!_isTargetMixerImmediate && !_isTargetMixerNonstop)
			{
				Bass.BASS_ChannelUpdate(TargetMixerStream, 0);
			}
		}
		if ((!base.ChannelInfo.IsDecodingChannel || SourceMixerStream != 0) && Bass.BASS_ChannelIsActive(base.ChannelHandle) == BASSActive.BASS_ACTIVE_PLAYING)
		{
			Bass.BASS_ChannelPlay(_streamCopy, restart: false);
		}
		if (SourceMixerStream == 0)
		{
			Bass.BASS_ChannelLock(base.ChannelHandle, state: false);
		}
		else
		{
			Bass.BASS_ChannelLock(SourceMixerStream, state: false);
		}
		if (!base.ChannelInfo.IsDecodingChannel)
		{
			Bass.BASS_ChannelSetLink(base.ChannelHandle, _streamCopy);
		}
		Bass.BASS_SetDevice(num);
	}

	public override void OnStopped()
	{
		if (_streamCopy != 0)
		{
			if (TargetMixerStream != 0)
			{
				BassMix.BASS_Mixer_ChannelRemove(_streamCopy);
			}
			Bass.BASS_ChannelRemoveLink(base.ChannelHandle, _streamCopy);
			Bass.BASS_StreamFree(_streamCopy);
			_streamCopy = 0;
		}
	}

	public override void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (!base.IsBypassed && _streamCopy != 0)
		{
			if (_streamCopyDelay > 0)
			{
				_streamCopyDelay -= length;
			}
			else
			{
				Bass.BASS_StreamPutData(_streamCopy, buffer, length);
			}
		}
	}

	public override string ToString()
	{
		return "Stream Copy DSP";
	}
}
