using System;
using System.ComponentModel;
using System.Security;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public abstract class BaseDSP : IDisposable
{
	private bool disposed;

	private int _channel;

	private int _bitwidth = 16;

	private int _samplerate = 44100;

	private int _numchans = 2;

	private BASS_CHANNELINFO _channelInfo = new BASS_CHANNELINFO();

	private IntPtr _user = IntPtr.Zero;

	private int _dspPriority;

	private int _dspHandle;

	private DSPPROC _dspProc;

	private bool _bypass;

	public int ChannelHandle
	{
		get
		{
			return _channel;
		}
		set
		{
			if (_channel != value)
			{
				GetChannelInfo(value);
				if (_dspHandle != 0)
				{
					ReAssign(_channel, value);
				}
				_channel = value;
				OnChannelChanged();
			}
		}
	}

	public BASS_CHANNELINFO ChannelInfo => _channelInfo;

	public int ChannelBitwidth => _bitwidth;

	public int ChannelSampleRate => _samplerate;

	public int ChannelNumChans => _numchans;

	public int DSPPriority
	{
		get
		{
			return _dspPriority;
		}
		set
		{
			if (_dspPriority != value)
			{
				_dspPriority = value;
				if (_dspHandle != 0)
				{
					ReAssign(_channel, _channel);
				}
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

	public int DSPHandle => _dspHandle;

	public DSPPROC DSPProc => _dspProc;

	public bool IsBypassed => _bypass;

	public bool IsAssigned
	{
		get
		{
			if (_dspHandle == 0 || _channel == 0)
			{
				return false;
			}
			if (Bass.BASS_ChannelFlags(_channel, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_DEFAULT) == (BASSFlag.BASS_UNICODE | BASSFlag.BASS_SPEAKER_PAIR15 | BASSFlag.BASS_SAMPLE_OVER_DIST | BASSFlag.BASS_AC3_DOWNMIX_DOLBY | BASSFlag.BASS_SAMPLE_8BITS | BASSFlag.BASS_SAMPLE_MONO | BASSFlag.BASS_SAMPLE_LOOP | BASSFlag.BASS_SAMPLE_3D | BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_SAMPLE_MUTEMAX | BASSFlag.BASS_SAMPLE_VAM | BASSFlag.BASS_SAMPLE_FX | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_RECORD_PAUSE | BASSFlag.BASS_RECORD_ECHOCANCEL | BASSFlag.BASS_RECORD_AGC | BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_STREAM_RESTRATE | BASSFlag.BASS_STREAM_BLOCK | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_STATUS | BASSFlag.BASS_SPEAKER_LEFT | BASSFlag.BASS_SPEAKER_RIGHT | BASSFlag.BASS_ASYNCFILE | BASSFlag.BASS_WV_STEREO | BASSFlag.BASS_AC3_DYNAMIC_RANGE | BASSFlag.BASS_AAC_FRAME960))
			{
				_dspHandle = 0;
				_channel = 0;
				return false;
			}
			return true;
		}
	}

	public event EventHandler Notification;

	public BaseDSP()
	{
		_dspProc = DSPCallback;
		if (Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_FLOATDSP) == 1)
		{
			_bitwidth = 32;
		}
	}

	public BaseDSP(int channel, int priority, IntPtr user)
		: this()
	{
		_channel = channel;
		_dspPriority = priority;
		_user = user;
		GetChannelInfo(channel);
		Start();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!disposed)
		{
			try
			{
				Stop();
			}
			catch
			{
			}
		}
		disposed = true;
	}

	~BaseDSP()
	{
		Dispose(disposing: false);
	}

	public bool Start()
	{
		if (IsAssigned)
		{
			return true;
		}
		_dspHandle = Bass.BASS_ChannelSetDSP(_channel, _dspProc, _user, _dspPriority);
		OnStarted();
		return _dspHandle != 0;
	}

	public bool Stop()
	{
		bool result = Bass.BASS_ChannelRemoveDSP(_channel, _dspHandle);
		_dspHandle = 0;
		OnStopped();
		return result;
	}

	public void SetBypass(bool bypass)
	{
		_bypass = bypass;
		OnBypassChanged();
	}

	public virtual void OnChannelChanged()
	{
	}

	public virtual void OnStarted()
	{
	}

	public virtual void OnStopped()
	{
	}

	public virtual void OnBypassChanged()
	{
	}

	public abstract void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user);

	public void RaiseNotification()
	{
		if (this.Notification != null)
		{
			ProcessDelegate(this.Notification, this, EventArgs.Empty);
		}
	}

	public abstract override string ToString();

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

	private void GetChannelInfo(int channel)
	{
		if (channel != 0)
		{
			if (!Bass.BASS_ChannelGetInfo(channel, _channelInfo))
			{
				throw new ArgumentException("Invalid channel: " + Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode()));
			}
			_samplerate = _channelInfo.freq;
			_numchans = _channelInfo.chans;
			if ((_channelInfo.flags & BASSFlag.BASS_SAMPLE_MONO) != 0)
			{
				_numchans = 1;
			}
			_bitwidth = 16;
			bool flag = Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_FLOATDSP) == 1;
			if ((_channelInfo.flags & BASSFlag.BASS_SAMPLE_FLOAT) != 0 || flag)
			{
				_bitwidth = 32;
			}
			else if ((_channelInfo.flags & BASSFlag.BASS_SAMPLE_8BITS) != 0)
			{
				_bitwidth = 8;
			}
		}
	}

	private void ReAssign(int oldChannel, int newChannel)
	{
		Bass.BASS_ChannelRemoveDSP(oldChannel, _dspHandle);
		_dspHandle = Bass.BASS_ChannelSetDSP(newChannel, _dspProc, _user, _dspPriority);
	}
}
