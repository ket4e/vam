using System;
using System.Runtime.InteropServices;
using System.Security;
using Un4seen.Bass;

namespace Un4seen.BassAsio;

[SuppressUnmanagedCodeSecurity]
public class BassAsioHandler : IDisposable
{
	public delegate void BassAsioHandlerEventHandler(object sender, BassAsioHandlerEventArgs e);

	public static bool UseDedicatedThreads = true;

	private bool disposed;

	private bool _input;

	private bool _fullDuplex;

	private int _device = -1;

	private int _channel = -1;

	private int _fullDuplexDevice = -1;

	private int _fullDuplexChannel = -1;

	private int _mirrorChannel = -1;

	private double _samplerate = 48000.0;

	private int _numchans = 2;

	private BASSASIOFormat _format = BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT;

	private int _outputChannel;

	private BASS_CHANNELINFO _outputChannelInfo = new BASS_CHANNELINFO();

	private bool _useInputChannel;

	private int _inputChannel;

	private byte[] _fullDuplexBuffer;

	private int _bassSamplesNeeded = -1;

	private bool _bypassFullDuplex;

	private ASIOPROC _internalAsioProc;

	private bool _sourceStalled;

	private float _volume = 1f;

	private float _pan;

	private float _volumeMirror = 1f;

	private float _panMirror;

	public float DeviceVolume
	{
		get
		{
			int num = BassAsio.BASS_ASIO_GetDevice();
			if (num != _device)
			{
				BassAsio.BASS_ASIO_SetDevice(_device);
			}
			float result = BassAsio.BASS_ASIO_ChannelGetVolume(_input, -1);
			BassAsio.BASS_ASIO_SetDevice(num);
			return result;
		}
		set
		{
			if (value < 0f)
			{
				value = 0f;
			}
			else if (value > 1f)
			{
				value = 1f;
			}
			int num = BassAsio.BASS_ASIO_GetDevice();
			if (num != _device)
			{
				BassAsio.BASS_ASIO_SetDevice(_device);
			}
			BassAsio.BASS_ASIO_ChannelSetVolume(_input, -1, value);
			BassAsio.BASS_ASIO_SetDevice(num);
		}
	}

	public ASIOPROC InternalAsioProc => _internalAsioProc;

	public float Volume
	{
		get
		{
			return _volume;
		}
		set
		{
			if (_volume != value)
			{
				if (value < 0f)
				{
					_volume = 0f;
				}
				else
				{
					_volume = value;
				}
				int num = BassAsio.BASS_ASIO_GetDevice();
				if (num != _device)
				{
					BassAsio.BASS_ASIO_SetDevice(_device);
				}
				SetVolume(_volume, _pan);
				BassAsio.BASS_ASIO_SetDevice(num);
			}
		}
	}

	public float VolumeMirror
	{
		get
		{
			return _volumeMirror;
		}
		set
		{
			if (_volumeMirror != value)
			{
				if (value < 0f)
				{
					_volumeMirror = 0f;
				}
				else
				{
					_volumeMirror = value;
				}
				int num = BassAsio.BASS_ASIO_GetDevice();
				if (num != _device)
				{
					BassAsio.BASS_ASIO_SetDevice(_device);
				}
				SetVolumeMirror(_volumeMirror, _panMirror);
				BassAsio.BASS_ASIO_SetDevice(num);
			}
		}
	}

	public float Pan
	{
		get
		{
			return _pan;
		}
		set
		{
			if (_pan != value)
			{
				if (value < -1f)
				{
					_pan = -1f;
				}
				else if (value > 1f)
				{
					_pan = 1f;
				}
				else
				{
					_pan = value;
				}
				int num = BassAsio.BASS_ASIO_GetDevice();
				if (num != _device)
				{
					BassAsio.BASS_ASIO_SetDevice(_device);
				}
				SetVolume(_volume, _pan);
				BassAsio.BASS_ASIO_SetDevice(num);
			}
		}
	}

	public float PanMirror
	{
		get
		{
			return _panMirror;
		}
		set
		{
			if (_panMirror != value)
			{
				if (value < -1f)
				{
					_panMirror = -1f;
				}
				else if (value > 1f)
				{
					_panMirror = 1f;
				}
				else
				{
					_panMirror = value;
				}
				int num = BassAsio.BASS_ASIO_GetDevice();
				if (num != _device)
				{
					BassAsio.BASS_ASIO_SetDevice(_device);
				}
				SetVolumeMirror(_volumeMirror, _panMirror);
				BassAsio.BASS_ASIO_SetDevice(num);
			}
		}
	}

	public bool IsInput => _input;

	public int Device => _device;

	public int Channel => _channel;

	public bool IsInputFullDuplex
	{
		get
		{
			if (_input)
			{
				return _fullDuplex;
			}
			return false;
		}
	}

	public int FullDuplexDevice => _fullDuplexDevice;

	public int FullDuplexChannel => _fullDuplexChannel;

	public bool BypassFullDuplex
	{
		get
		{
			return _bypassFullDuplex;
		}
		set
		{
			_bypassFullDuplex = value;
		}
	}

	public bool IsMirrored => _mirrorChannel != -1;

	public int MirrorChannel => _mirrorChannel;

	public double SampleRate
	{
		get
		{
			return _samplerate;
		}
		set
		{
			if (_samplerate != value)
			{
				if (value > 0.0)
				{
					_samplerate = value;
				}
				BassAsio.BASS_ASIO_ChannelSetRate(_input, _channel, _samplerate);
			}
		}
	}

	public int ChannelNumChans => _numchans;

	public BASSASIOFormat Format
	{
		get
		{
			return _format;
		}
		set
		{
			_format = value;
			BassAsio.BASS_ASIO_ChannelSetFormat(_input, _channel, _format);
		}
	}

	public int OutputChannel
	{
		get
		{
			return _outputChannel;
		}
		set
		{
			_outputChannel = value;
		}
	}

	public bool IsResampling
	{
		get
		{
			bool flag = false;
			int num = BassAsio.BASS_ASIO_GetDevice();
			if (num != _device)
			{
				BassAsio.BASS_ASIO_SetDevice(_device);
			}
			double num2 = BassAsio.BASS_ASIO_ChannelGetRate(_input, _channel);
			if (num2 != 0.0)
			{
				flag = BassAsio.BASS_ASIO_GetRate() != num2;
			}
			if (!flag && IsInputFullDuplex)
			{
				if (_device != _fullDuplexDevice)
				{
					BassAsio.BASS_ASIO_SetDevice(_fullDuplexDevice);
				}
				double num3 = BassAsio.BASS_ASIO_ChannelGetRate(input: false, _fullDuplexChannel);
				if (num3 != 0.0)
				{
					double num4 = BassAsio.BASS_ASIO_GetRate();
					flag = flag && num4 != num3;
				}
			}
			BassAsio.BASS_ASIO_SetDevice(num);
			return flag;
		}
	}

	public int InputChannel => _inputChannel;

	public bool UseInput
	{
		get
		{
			return _useInputChannel;
		}
		set
		{
			if (!IsInput)
			{
				_useInputChannel = false;
				return;
			}
			_useInputChannel = value;
			if (_inputChannel != 0)
			{
				Un4seen.Bass.Bass.BASS_StreamFree(_inputChannel);
				_inputChannel = 0;
			}
			if (_useInputChannel)
			{
				int num = BassAsio.BASS_ASIO_GetDevice();
				if (num != _device)
				{
					BassAsio.BASS_ASIO_SetDevice(_device);
				}
				_inputChannel = Un4seen.Bass.Bass.BASS_StreamCreateDummy((int)_samplerate, _numchans, ((_format == BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT) ? BASSFlag.BASS_SAMPLE_FLOAT : BASSFlag.BASS_DEFAULT) | BASSFlag.BASS_STREAM_DECODE, IntPtr.Zero);
				BassAsio.BASS_ASIO_SetDevice(num);
			}
		}
	}

	public event BassAsioHandlerEventHandler Notification;

	public BassAsioHandler()
	{
	}

	public BassAsioHandler(bool input, int asioDevice, int asioChannel, int asioNumChans, BASSASIOFormat asioFormat, double asioSamplerate)
	{
		_input = input;
		_device = asioDevice;
		_channel = asioChannel;
		_numchans = asioNumChans;
		_format = asioFormat;
		BassAsio.BASS_ASIO_Init(asioDevice, UseDedicatedThreads ? BASSASIOInit.BASS_ASIO_THREAD : BASSASIOInit.BASS_ASIO_DEFAULT);
		BassAsio.BASS_ASIO_SetDevice(asioDevice);
		if (asioSamplerate <= 0.0)
		{
			_samplerate = BassAsio.BASS_ASIO_GetRate();
		}
		else
		{
			BassAsio.BASS_ASIO_SetRate(asioSamplerate);
			_samplerate = BassAsio.BASS_ASIO_GetRate();
		}
		if (_input)
		{
			_internalAsioProc = AsioInputCallback;
			UseInput = true;
		}
		else
		{
			_internalAsioProc = AsioOutputCallback;
		}
		bool num = BassAsio.BASS_ASIO_IsStarted();
		BASSASIOActive bASSASIOActive = BassAsio.BASS_ASIO_ChannelIsActive(_input, _channel);
		if (num && bASSASIOActive == BASSASIOActive.BASS_ASIO_ACTIVE_DISABLED)
		{
			BassAsio.BASS_ASIO_Stop();
		}
		EnableAndJoin(_input, _channel, _numchans, _internalAsioProc, _format);
		if (num)
		{
			BassAsio.BASS_ASIO_Start(0, 0);
		}
	}

	public BassAsioHandler(int asioDevice, int asioChannel, int outputChannel, BASSASIOFormat format = BASSASIOFormat.BASS_ASIO_FORMAT_UNKNOWN)
	{
		_input = false;
		_device = asioDevice;
		_channel = asioChannel;
		_outputChannel = outputChannel;
		BassAsio.BASS_ASIO_Init(asioDevice, UseDedicatedThreads ? BASSASIOInit.BASS_ASIO_THREAD : BASSASIOInit.BASS_ASIO_DEFAULT);
		BassAsio.BASS_ASIO_SetDevice(asioDevice);
		GetChannelInfo(outputChannel);
		_numchans = _outputChannelInfo.chans;
		switch (format)
		{
		case BASSASIOFormat.BASS_ASIO_FORMAT_UNKNOWN:
			_format = BASSASIOFormat.BASS_ASIO_FORMAT_16BIT;
			if ((_outputChannelInfo.flags & BASSFlag.BASS_SAMPLE_FLOAT) != 0)
			{
				_format = BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT;
			}
			break;
		case BASSASIOFormat.BASS_ASIO_FORMAT_DITHER:
			_format = BASSASIOFormat.BASS_ASIO_FORMAT_16BIT;
			if ((_outputChannelInfo.flags & BASSFlag.BASS_SAMPLE_FLOAT) != 0)
			{
				_format = (BASSASIOFormat)275;
			}
			break;
		default:
			_format = format;
			break;
		}
		_samplerate = _outputChannelInfo.freq;
		BassAsio.BASS_ASIO_SetRate(_samplerate);
		_internalAsioProc = AsioOutputCallback;
		bool num = BassAsio.BASS_ASIO_IsStarted();
		BASSASIOActive bASSASIOActive = BassAsio.BASS_ASIO_ChannelIsActive(_input, _channel);
		if (num && bASSASIOActive == BASSASIOActive.BASS_ASIO_ACTIVE_DISABLED)
		{
			BassAsio.BASS_ASIO_Stop();
		}
		EnableAndJoin(_input, _channel, _numchans, _internalAsioProc, _format);
		if (num)
		{
			BassAsio.BASS_ASIO_Start(0, 0);
		}
	}

	private bool EnableAndJoin(bool input, int channel, int numchans, ASIOPROC proc, BASSASIOFormat format)
	{
		bool flag = true;
		if (BassAsio.BASS_ASIO_ChannelIsActive(input, channel) == BASSASIOActive.BASS_ASIO_ACTIVE_DISABLED)
		{
			flag &= BassAsio.BASS_ASIO_ChannelEnable(input, channel, proc, IntPtr.Zero);
			for (int i = 1; i < numchans; i++)
			{
				BassAsio.BASS_ASIO_ChannelJoin(input, channel + i, channel);
			}
		}
		else
		{
			flag &= BassAsio.BASS_ASIO_ChannelEnable(input, channel, proc, IntPtr.Zero);
		}
		flag &= BassAsio.BASS_ASIO_ChannelSetFormat(input, channel, format);
		return flag & BassAsio.BASS_ASIO_ChannelSetRate(input, channel, _samplerate);
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
			if (disposing)
			{
				int num = BassAsio.BASS_ASIO_GetDevice();
				if (num != _device)
				{
					BassAsio.BASS_ASIO_SetDevice(_device);
				}
				bool flag = BassAsio.BASS_ASIO_Stop();
				RemoveFullDuplex(disableOutput: true);
				RemoveMirror();
				BassAsio.BASS_ASIO_ChannelEnable(_input, _channel, null, IntPtr.Zero);
				BassAsio.BASS_ASIO_ChannelReset(_input, _channel, BASSASIOReset.BASS_ASIO_RESET_ENABLE | BASSASIOReset.BASS_ASIO_RESET_FORMAT | BASSASIOReset.BASS_ASIO_RESET_RATE | BASSASIOReset.BASS_ASIO_RESET_VOLUME);
				for (int i = 1; i < _numchans; i++)
				{
					BassAsio.BASS_ASIO_ChannelReset(_input, _channel + i, BASSASIOReset.BASS_ASIO_RESET_JOIN);
				}
				if (flag)
				{
					BassAsio.BASS_ASIO_Start(0, 0);
				}
				BassAsio.BASS_ASIO_SetDevice(num);
			}
			if (_outputChannel != 0)
			{
				Un4seen.Bass.Bass.BASS_StreamFree(_outputChannel);
				_outputChannel = 0;
			}
			if (_inputChannel != 0)
			{
				Un4seen.Bass.Bass.BASS_StreamFree(_inputChannel);
				_inputChannel = 0;
			}
		}
		disposed = true;
	}

	~BassAsioHandler()
	{
		Dispose(disposing: false);
	}

	public bool Start(int buflen, int threads)
	{
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (num != _device)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_device);
		}
		if (flag && !BassAsio.BASS_ASIO_IsStarted())
		{
			flag &= BassAsio.BASS_ASIO_Start(buflen, threads);
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public bool Stop()
	{
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (num != _device)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_device);
		}
		if (flag && BassAsio.BASS_ASIO_IsStarted())
		{
			flag &= BassAsio.BASS_ASIO_Stop();
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public bool StartFullDuplex(int buflen)
	{
		if (_fullDuplexDevice < 0)
		{
			return false;
		}
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (num != _fullDuplexDevice)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_fullDuplexDevice);
		}
		if (flag && !BassAsio.BASS_ASIO_IsStarted())
		{
			flag &= BassAsio.BASS_ASIO_Start(buflen, 0);
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public bool StopFullDuplex()
	{
		if (_fullDuplexDevice < 0)
		{
			return false;
		}
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (num != _fullDuplexDevice)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_fullDuplexDevice);
		}
		if (flag && BassAsio.BASS_ASIO_IsStarted())
		{
			flag &= BassAsio.BASS_ASIO_Stop();
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public bool Pause(bool pause)
	{
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (pause)
		{
			if (num != _device)
			{
				flag &= BassAsio.BASS_ASIO_SetDevice(_device);
			}
			if (flag)
			{
				flag &= BassAsio.BASS_ASIO_ChannelPause(_input, _channel);
			}
			if (flag && IsInputFullDuplex)
			{
				if (_fullDuplexChannel != -1)
				{
					if (_device != _fullDuplexDevice)
					{
						BassAsio.BASS_ASIO_SetDevice(_fullDuplexDevice);
					}
					flag &= BassAsio.BASS_ASIO_ChannelPause(input: false, _fullDuplexChannel);
				}
				else
				{
					Un4seen.Bass.Bass.BASS_ChannelPause(_outputChannel);
					Un4seen.Bass.Bass.BASS_ChannelSetPosition(_outputChannel, 0L);
				}
			}
		}
		else
		{
			if (IsInputFullDuplex)
			{
				if (_fullDuplexChannel != -1)
				{
					if (num != _fullDuplexDevice)
					{
						flag &= BassAsio.BASS_ASIO_SetDevice(_fullDuplexDevice);
					}
					if (flag)
					{
						flag &= BassAsio.BASS_ASIO_ChannelReset(input: false, _fullDuplexChannel, BASSASIOReset.BASS_ASIO_RESET_PAUSE);
					}
				}
				else
				{
					Un4seen.Bass.Bass.BASS_ChannelPlay(_outputChannel, restart: false);
				}
			}
			if (flag)
			{
				flag &= BassAsio.BASS_ASIO_SetDevice(_device);
				if (flag)
				{
					flag &= BassAsio.BASS_ASIO_ChannelReset(_input, _channel, BASSASIOReset.BASS_ASIO_RESET_PAUSE);
				}
			}
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public bool PauseMirror(bool pause)
	{
		if (!IsMirrored)
		{
			return false;
		}
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (num != _device)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_device);
		}
		if (pause)
		{
			if (flag)
			{
				for (int i = 0; i < _numchans; i++)
				{
					flag &= BassAsio.BASS_ASIO_ChannelPause(input: false, _mirrorChannel + i);
				}
			}
		}
		else if (flag)
		{
			for (int j = 0; j < _numchans; j++)
			{
				flag &= BassAsio.BASS_ASIO_ChannelReset(input: false, _mirrorChannel + j, BASSASIOReset.BASS_ASIO_RESET_PAUSE);
			}
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public bool AssignOutputChannel(int outputChannel)
	{
		if (_input)
		{
			return false;
		}
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (num != _device)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_device);
		}
		if (flag)
		{
			BASSASIOActive num2 = BassAsio.BASS_ASIO_ChannelIsActive(_input, _channel);
			if (num2 == BASSASIOActive.BASS_ASIO_ACTIVE_ENABLED)
			{
				BassAsio.BASS_ASIO_ChannelPause(_input, _channel);
			}
			_outputChannel = outputChannel;
			if (_outputChannel == 0)
			{
				_samplerate = BassAsio.BASS_ASIO_GetRate();
				_numchans = 1;
				_format = BASSASIOFormat.BASS_ASIO_FORMAT_16BIT;
				_internalAsioProc = AsioOutputCallback;
			}
			else
			{
				GetChannelInfo(_outputChannel);
				_numchans = _outputChannelInfo.chans;
				_format = BASSASIOFormat.BASS_ASIO_FORMAT_16BIT;
				if ((_outputChannelInfo.flags & BASSFlag.BASS_SAMPLE_FLOAT) != 0)
				{
					_format = BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT;
				}
				_samplerate = _outputChannelInfo.freq;
			}
			BassAsio.BASS_ASIO_SetRate(_samplerate);
			if (num2 == BASSASIOActive.BASS_ASIO_ACTIVE_DISABLED)
			{
				_internalAsioProc = AsioOutputCallback;
			}
			flag &= EnableAndJoin(_input, _channel, _numchans, _internalAsioProc, _format);
			if (num2 == BASSASIOActive.BASS_ASIO_ACTIVE_ENABLED)
			{
				BassAsio.BASS_ASIO_ChannelReset(_input, _channel, BASSASIOReset.BASS_ASIO_RESET_PAUSE);
			}
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public bool SetFullDuplex(int asioDevice, int asioChannel)
	{
		if (!_input)
		{
			return false;
		}
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (num != _device)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_device);
		}
		if (flag)
		{
			if (_outputChannel != 0)
			{
				Un4seen.Bass.Bass.BASS_StreamFree(_outputChannel);
			}
			BASSFlag bASSFlag = BASSFlag.BASS_STREAM_DECODE;
			if (_format == BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT)
			{
				bASSFlag |= BASSFlag.BASS_SAMPLE_FLOAT;
			}
			_outputChannel = Un4seen.Bass.Bass.BASS_StreamCreateDummy((int)BassAsio.BASS_ASIO_GetRate(), _numchans, bASSFlag, IntPtr.Zero);
			BASS_ASIO_INFO bASS_ASIO_INFO = BassAsio.BASS_ASIO_GetInfo();
			_fullDuplexBuffer = new byte[bASS_ASIO_INFO.bufmax * _numchans * 4];
			_internalAsioProc = AsioToAsioFullDuplexCallback;
			flag &= EnableAndJoin(_input, _channel, _numchans, _internalAsioProc, _format);
			if (flag)
			{
				if (_device != asioDevice)
				{
					flag &= BassAsio.BASS_ASIO_SetDevice(asioDevice);
				}
				if (flag)
				{
					BassAsio.BASS_ASIO_SetRate(_samplerate);
					bool num2 = BassAsio.BASS_ASIO_IsStarted();
					BASSASIOActive bASSASIOActive = BassAsio.BASS_ASIO_ChannelIsActive(input: false, asioChannel);
					if (num2 && bASSASIOActive == BASSASIOActive.BASS_ASIO_ACTIVE_DISABLED)
					{
						BassAsio.BASS_ASIO_Stop();
					}
					flag &= EnableAndJoin(input: false, asioChannel, _numchans, _internalAsioProc, _format);
					if (flag)
					{
						_fullDuplexDevice = asioDevice;
						_fullDuplexChannel = asioChannel;
						_fullDuplex = true;
					}
					if (num2)
					{
						BassAsio.BASS_ASIO_Start(0, 0);
					}
				}
			}
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public bool SetFullDuplex(int bassDevice, BASSFlag flags, bool buffered)
	{
		if (!_input)
		{
			return false;
		}
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		int num2 = Un4seen.Bass.Bass.BASS_GetDevice();
		if (num != _device)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_device);
		}
		if (num2 != bassDevice)
		{
			flag &= Un4seen.Bass.Bass.BASS_SetDevice(bassDevice);
		}
		if (flag)
		{
			if (_outputChannel != 0)
			{
				Un4seen.Bass.Bass.BASS_StreamFree(_outputChannel);
			}
			flags &= ~BASSFlag.BASS_SAMPLE_8BITS;
			if (_format == BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT)
			{
				flags |= BASSFlag.BASS_SAMPLE_FLOAT;
			}
			else if (_format == BASSASIOFormat.BASS_ASIO_FORMAT_16BIT)
			{
				flags &= ~BASSFlag.BASS_SAMPLE_FLOAT;
			}
			_outputChannel = Un4seen.Bass.Bass.BASS_StreamCreatePush((int)BassAsio.BASS_ASIO_GetRate(), _numchans, flags, IntPtr.Zero);
			if (_outputChannel != 0)
			{
				Un4seen.Bass.Bass.BASS_ChannelGetInfo(_outputChannel, _outputChannelInfo);
				if (buffered)
				{
					_bassSamplesNeeded = (int)Un4seen.Bass.Bass.BASS_ChannelSeconds2Bytes(_outputChannel, (double)Un4seen.Bass.Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD) / 500.0);
				}
				else
				{
					_bassSamplesNeeded = -1;
				}
				if (!buffered && (flags & BASSFlag.BASS_STREAM_DECODE) == 0)
				{
					Un4seen.Bass.Bass.BASS_ChannelPlay(_outputChannel, restart: false);
				}
				_internalAsioProc = AsioToBassFullDuplexCallback;
				flag &= EnableAndJoin(_input, _channel, _numchans, _internalAsioProc, _format);
				if (flag)
				{
					_fullDuplexDevice = bassDevice;
					_fullDuplexChannel = -1;
					_fullDuplex = true;
				}
			}
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		Un4seen.Bass.Bass.BASS_SetDevice(num2);
		return flag;
	}

	public bool RemoveFullDuplex(bool disableOutput)
	{
		if (!IsInputFullDuplex)
		{
			return false;
		}
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (_fullDuplexChannel != -1 && num != _fullDuplexDevice)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_fullDuplexDevice);
		}
		if (flag)
		{
			bool flag2 = false;
			if (_fullDuplexChannel != -1)
			{
				flag2 = BassAsio.BASS_ASIO_IsStarted();
				BASSASIOActive bASSASIOActive = BassAsio.BASS_ASIO_ChannelIsActive(input: false, _fullDuplexChannel);
				if (flag2 && disableOutput && bASSASIOActive != 0)
				{
					BassAsio.BASS_ASIO_Stop();
					flag &= BassAsio.BASS_ASIO_ChannelEnable(input: false, _fullDuplexChannel, null, IntPtr.Zero);
					if (flag)
					{
						BassAsio.BASS_ASIO_ChannelReset(input: false, _fullDuplexChannel, BASSASIOReset.BASS_ASIO_RESET_ENABLE | BASSASIOReset.BASS_ASIO_RESET_FORMAT | BASSASIOReset.BASS_ASIO_RESET_RATE | BASSASIOReset.BASS_ASIO_RESET_VOLUME);
						for (int i = 1; i < _numchans; i++)
						{
							BassAsio.BASS_ASIO_ChannelReset(input: false, _fullDuplexChannel + i, BASSASIOReset.BASS_ASIO_RESET_JOIN);
						}
					}
				}
			}
			if (flag)
			{
				_fullDuplexChannel = -1;
				_fullDuplexDevice = -1;
				_fullDuplex = false;
			}
			if (flag2)
			{
				BassAsio.BASS_ASIO_Start(0, 0);
			}
			if (flag)
			{
				if (_fullDuplexDevice != _device)
				{
					flag &= BassAsio.BASS_ASIO_SetDevice(_device);
				}
				if (flag)
				{
					_internalAsioProc = AsioInputCallback;
					flag &= EnableAndJoin(_input, _channel, _numchans, _internalAsioProc, _format);
				}
				Un4seen.Bass.Bass.BASS_StreamFree(_outputChannel);
				_outputChannel = 0;
				_fullDuplexBuffer = null;
				BypassFullDuplex = false;
			}
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public bool SetMirror(int asioChannel)
	{
		if (!_input && _channel == asioChannel)
		{
			return false;
		}
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (num != _device)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_device);
		}
		if (flag)
		{
			bool flag2 = BassAsio.BASS_ASIO_IsStarted();
			BASSASIOActive bASSASIOActive = BassAsio.BASS_ASIO_ChannelIsActive(input: false, asioChannel);
			if (flag2)
			{
				BassAsio.BASS_ASIO_Stop();
			}
			for (int i = 0; i < _numchans; i++)
			{
				flag &= BassAsio.BASS_ASIO_ChannelEnableMirror(asioChannel + i, _input, _channel + i);
			}
			if (flag)
			{
				_mirrorChannel = asioChannel;
			}
			else if (bASSASIOActive == BASSASIOActive.BASS_ASIO_ACTIVE_DISABLED)
			{
				for (int j = 0; j < _numchans; j++)
				{
					BassAsio.BASS_ASIO_ChannelEnable(input: false, asioChannel + j, null, IntPtr.Zero);
					BassAsio.BASS_ASIO_ChannelReset(input: false, asioChannel + j, BASSASIOReset.BASS_ASIO_RESET_ENABLE | BASSASIOReset.BASS_ASIO_RESET_FORMAT | BASSASIOReset.BASS_ASIO_RESET_RATE | BASSASIOReset.BASS_ASIO_RESET_VOLUME);
				}
			}
			if (flag2)
			{
				BassAsio.BASS_ASIO_Start(0, 0);
			}
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public bool RemoveMirror()
	{
		if (!IsMirrored)
		{
			return false;
		}
		bool flag = true;
		int num = BassAsio.BASS_ASIO_GetDevice();
		if (num != _device)
		{
			flag &= BassAsio.BASS_ASIO_SetDevice(_device);
		}
		if (flag)
		{
			bool flag2 = BassAsio.BASS_ASIO_IsStarted();
			if (BassAsio.BASS_ASIO_ChannelIsActive(input: false, _mirrorChannel) != 0)
			{
				BassAsio.BASS_ASIO_Stop();
				for (int i = 0; i < _numchans; i++)
				{
					flag &= BassAsio.BASS_ASIO_ChannelEnable(input: false, _mirrorChannel + i, null, IntPtr.Zero);
					BassAsio.BASS_ASIO_ChannelReset(input: false, _mirrorChannel + i, BASSASIOReset.BASS_ASIO_RESET_ENABLE | BASSASIOReset.BASS_ASIO_RESET_FORMAT | BASSASIOReset.BASS_ASIO_RESET_RATE | BASSASIOReset.BASS_ASIO_RESET_VOLUME);
				}
			}
			if (flag)
			{
				_mirrorChannel = -1;
			}
			if (flag2)
			{
				BassAsio.BASS_ASIO_Start(0, 0);
			}
		}
		BassAsio.BASS_ASIO_SetDevice(num);
		return flag;
	}

	public virtual int AsioOutputCallback(bool input, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (input || _outputChannel == 0)
		{
			return 0;
		}
		int num = Un4seen.Bass.Bass.BASS_ChannelGetData(_outputChannel, buffer, length);
		if (num < length && !_sourceStalled)
		{
			RaiseNotification(BassAsioHandlerSyncType.BufferUnderrun, _outputChannel);
		}
		if (num <= 0)
		{
			num = 0;
			if (!_sourceStalled)
			{
				_sourceStalled = true;
				RaiseNotification(BassAsioHandlerSyncType.SourceStalled, _outputChannel);
			}
		}
		else if (_sourceStalled)
		{
			_sourceStalled = false;
			RaiseNotification(BassAsioHandlerSyncType.SourceResumed, _outputChannel);
		}
		return num;
	}

	public virtual int AsioInputCallback(bool input, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (!input)
		{
			return 0;
		}
		if (_inputChannel != 0)
		{
			Un4seen.Bass.Bass.BASS_ChannelGetData(_inputChannel, buffer, length);
		}
		return 0;
	}

	public virtual int AsioToAsioFullDuplexCallback(bool input, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (input)
		{
			if (_fullDuplexBuffer.Length < length)
			{
				_fullDuplexBuffer = new byte[length];
			}
			if (_inputChannel != 0)
			{
				Un4seen.Bass.Bass.BASS_ChannelGetData(_inputChannel, buffer, length);
			}
			Marshal.Copy(buffer, _fullDuplexBuffer, 0, length);
			return 0;
		}
		if (!_bypassFullDuplex)
		{
			if (length > _fullDuplexBuffer.Length)
			{
				length = _fullDuplexBuffer.Length;
			}
			Marshal.Copy(_fullDuplexBuffer, 0, buffer, length);
			Un4seen.Bass.Bass.BASS_ChannelGetData(_outputChannel, buffer, length);
			return length;
		}
		return 0;
	}

	public virtual int AsioToBassFullDuplexCallback(bool input, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (!input)
		{
			return 0;
		}
		if (_inputChannel != 0)
		{
			Un4seen.Bass.Bass.BASS_ChannelGetData(_inputChannel, buffer, length);
		}
		if (!_bypassFullDuplex)
		{
			Un4seen.Bass.Bass.BASS_ChannelLock(_outputChannel, state: true);
			int num = Un4seen.Bass.Bass.BASS_StreamPutData(_outputChannel, buffer, length);
			if (num > 1536000)
			{
				Un4seen.Bass.Bass.BASS_ChannelGetData(_outputChannel, buffer, num - 1536000);
			}
			Un4seen.Bass.Bass.BASS_ChannelLock(_outputChannel, state: false);
			if (_bassSamplesNeeded > 0 && Un4seen.Bass.Bass.BASS_StreamPutData(_outputChannel, IntPtr.Zero, 0) >= _bassSamplesNeeded)
			{
				Un4seen.Bass.Bass.BASS_ChannelPlay(_outputChannel, restart: false);
				_bassSamplesNeeded = -1;
			}
		}
		return 0;
	}

	private void GetChannelInfo(int channel)
	{
		if (channel == 0)
		{
			throw new ArgumentException("Invalid channel: must be a valid BASS channel!");
		}
		if (!Un4seen.Bass.Bass.BASS_ChannelGetInfo(channel, _outputChannelInfo))
		{
			throw new ArgumentException("Invalid channel: " + Enum.GetName(typeof(BASSError), Un4seen.Bass.Bass.BASS_ErrorGetCode()));
		}
		if (!_outputChannelInfo.IsDecodingChannel && _outputChannelInfo.ctype != BASSChannelType.BASS_CTYPE_RECORD)
		{
			throw new ArgumentException("Invalid channel: must be a decoding or recording channel!");
		}
	}

	private void SetVolume(float volume, float pan)
	{
		float num = volume;
		float num2 = volume;
		if (ChannelNumChans > 1)
		{
			if (pan < 0f)
			{
				num2 = (1f + pan) * volume;
			}
			else if (pan > 0f)
			{
				num = (1f - pan) * volume;
			}
		}
		float num3 = 1f;
		for (int i = 0; i < ChannelNumChans; i++)
		{
			BassAsio.BASS_ASIO_ChannelSetVolume(volume: (i % 2 != 0) ? num2 : num, input: _input, channel: i + Channel);
		}
	}

	private void SetVolumeMirror(float volume, float pan)
	{
		float num = volume;
		float num2 = volume;
		if (ChannelNumChans > 1)
		{
			if (pan < 0f)
			{
				num2 = (1f + pan) * volume;
			}
			else if (pan > 0f)
			{
				num = (1f - pan) * volume;
			}
		}
		float num3 = 1f;
		for (int i = 0; i < ChannelNumChans; i++)
		{
			BassAsio.BASS_ASIO_ChannelSetVolume(volume: (i % 2 != 0) ? num2 : num, input: false, channel: i + MirrorChannel);
		}
	}

	private void RaiseNotification(BassAsioHandlerSyncType syncType, int data)
	{
		if (this.Notification != null)
		{
			this.Notification(this, new BassAsioHandlerEventArgs(syncType, data));
		}
	}
}
