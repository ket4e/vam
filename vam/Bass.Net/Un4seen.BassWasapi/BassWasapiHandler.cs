using System;
using System.Security;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;

namespace Un4seen.BassWasapi;

[SuppressUnmanagedCodeSecurity]
public class BassWasapiHandler : IDisposable
{
	public delegate void BassWasapiHandlerEventHandler(object sender, BassWasapiHandlerEventArgs e);

	private bool disposed;

	private BASS_WASAPI_DEVICEINFO _wasapiDeviceInfo = new BASS_WASAPI_DEVICEINFO();

	private WASAPIPROC _internalWasapiProc;

	private bool _mixerStalled;

	private volatile float _volL = 1f;

	private volatile float _volR = 1f;

	private int _bassSamplesNeeded = -1;

	private bool _bypassFullDuplex;

	private bool _exclusive = true;

	private bool _eventSystem;

	private int _device = -1;

	private int _samplerate = 48000;

	private int _numchans = 2;

	private float _bufferLength;

	private float _updatePeriod;

	private int _internalMixer;

	private int _outputChannel;

	private float _volume = 1f;

	private float _pan;

	private int _inputChannel;

	private bool _useInputChannel;

	private volatile bool _fullDuplex;

	public WASAPIPROC InternalWasapiProc => _internalWasapiProc;

	public bool IsInput => _wasapiDeviceInfo.SupportsRecording;

	public bool Exclusive => _exclusive;

	public bool EventSystem => _eventSystem;

	public int Device => _device;

	public double SampleRate => _samplerate;

	public int NumChans => _numchans;

	public float BufferLength => _bufferLength;

	public float UpdatePeriod => _updatePeriod;

	public int InternalMixer => _internalMixer;

	public int OutputChannel => _outputChannel;

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
				SetVolume(_volume, _pan);
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
				SetVolume(_volume, _pan);
			}
		}
	}

	public float SessionVolume
	{
		get
		{
			int num = BassWasapi.BASS_WASAPI_GetDevice();
			if (num != _device)
			{
				BassWasapi.BASS_WASAPI_SetDevice(_device);
			}
			float result = BassWasapi.BASS_WASAPI_GetVolume(BASSWASAPIVolume.BASS_WASAPI_VOL_SESSION);
			BassWasapi.BASS_WASAPI_SetDevice(num);
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
			int num = BassWasapi.BASS_WASAPI_GetDevice();
			if (num != _device)
			{
				BassWasapi.BASS_WASAPI_SetDevice(_device);
			}
			BassWasapi.BASS_WASAPI_SetVolume(BASSWASAPIVolume.BASS_WASAPI_VOL_SESSION, value);
			BassWasapi.BASS_WASAPI_SetDevice(num);
		}
	}

	public bool SessionMute
	{
		get
		{
			int num = BassWasapi.BASS_WASAPI_GetDevice();
			if (num != _device)
			{
				BassWasapi.BASS_WASAPI_SetDevice(_device);
			}
			bool result = BassWasapi.BASS_WASAPI_GetMute(BASSWASAPIVolume.BASS_WASAPI_VOL_SESSION);
			BassWasapi.BASS_WASAPI_SetDevice(num);
			return result;
		}
		set
		{
			int num = BassWasapi.BASS_WASAPI_GetDevice();
			if (num != _device)
			{
				BassWasapi.BASS_WASAPI_SetDevice(_device);
			}
			BassWasapi.BASS_WASAPI_SetMute(BASSWASAPIVolume.BASS_WASAPI_VOL_SESSION, value);
			BassWasapi.BASS_WASAPI_SetDevice(num);
		}
	}

	public float DeviceVolume
	{
		get
		{
			int num = BassWasapi.BASS_WASAPI_GetDevice();
			if (num != _device)
			{
				BassWasapi.BASS_WASAPI_SetDevice(_device);
			}
			float result = BassWasapi.BASS_WASAPI_GetVolume(BASSWASAPIVolume.BASS_WASAPI_CURVE_LINEAR);
			BassWasapi.BASS_WASAPI_SetDevice(num);
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
			int num = BassWasapi.BASS_WASAPI_GetDevice();
			if (num != _device)
			{
				BassWasapi.BASS_WASAPI_SetDevice(_device);
			}
			BassWasapi.BASS_WASAPI_SetVolume(BASSWASAPIVolume.BASS_WASAPI_CURVE_LINEAR, value);
			BassWasapi.BASS_WASAPI_SetDevice(num);
		}
	}

	public bool DeviceMute
	{
		get
		{
			int num = BassWasapi.BASS_WASAPI_GetDevice();
			if (num != _device)
			{
				BassWasapi.BASS_WASAPI_SetDevice(_device);
			}
			bool result = BassWasapi.BASS_WASAPI_GetMute(BASSWASAPIVolume.BASS_WASAPI_CURVE_DB);
			BassWasapi.BASS_WASAPI_SetDevice(num);
			return result;
		}
		set
		{
			int num = BassWasapi.BASS_WASAPI_GetDevice();
			if (num != _device)
			{
				BassWasapi.BASS_WASAPI_SetDevice(_device);
			}
			BassWasapi.BASS_WASAPI_SetMute(BASSWASAPIVolume.BASS_WASAPI_CURVE_DB, value);
			BassWasapi.BASS_WASAPI_SetDevice(num);
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
				_inputChannel = Un4seen.Bass.Bass.BASS_StreamCreateDummy(_samplerate, _numchans, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, IntPtr.Zero);
			}
		}
	}

	public bool IsInputFullDuplex
	{
		get
		{
			if (IsInput)
			{
				return _fullDuplex;
			}
			return false;
		}
	}

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

	public event BassWasapiHandlerEventHandler Notification;

	public BassWasapiHandler(int device, bool exclusive, int freq, int chans, float buffer, float period)
	{
		_device = device;
		_exclusive = exclusive;
		if (!BassWasapi.BASS_WASAPI_GetDeviceInfo(device, _wasapiDeviceInfo))
		{
			throw new ArgumentException("Invalid device: " + Enum.GetName(typeof(BASSError), Un4seen.Bass.Bass.BASS_ErrorGetCode()));
		}
		if (exclusive)
		{
			_numchans = chans;
		}
		else
		{
			_numchans = _wasapiDeviceInfo.mixchans;
		}
		if (exclusive)
		{
			_samplerate = freq;
		}
		else
		{
			_samplerate = _wasapiDeviceInfo.mixfreq;
		}
		_updatePeriod = period;
		if (buffer == 0f)
		{
			_bufferLength = ((_updatePeriod == 0f) ? _wasapiDeviceInfo.defperiod : _updatePeriod) * 4f;
		}
		else
		{
			_bufferLength = buffer;
		}
		if (IsInput)
		{
			UseInput = true;
			return;
		}
		_internalMixer = BassMix.BASS_Mixer_StreamCreate(_samplerate, _numchans, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_AAC_FRAME960);
		if (_internalMixer != 0)
		{
			return;
		}
		throw new NotSupportedException("Internal Mixer: " + Enum.GetName(typeof(BASSError), Un4seen.Bass.Bass.BASS_ErrorGetCode()));
	}

	public BassWasapiHandler(int device, bool exclusive, bool eventSystem, int freq, int chans, float buffer, float period)
	{
		_device = device;
		_exclusive = exclusive;
		_eventSystem = eventSystem;
		if (!BassWasapi.BASS_WASAPI_GetDeviceInfo(device, _wasapiDeviceInfo))
		{
			throw new ArgumentException("Invalid device: " + Enum.GetName(typeof(BASSError), Un4seen.Bass.Bass.BASS_ErrorGetCode()));
		}
		if (exclusive)
		{
			_numchans = chans;
		}
		else
		{
			_numchans = _wasapiDeviceInfo.mixchans;
		}
		if (exclusive)
		{
			_samplerate = freq;
		}
		else
		{
			_samplerate = _wasapiDeviceInfo.mixfreq;
		}
		_updatePeriod = period;
		if (buffer == 0f)
		{
			_bufferLength = ((_updatePeriod == 0f) ? _wasapiDeviceInfo.defperiod : _updatePeriod) * 4f;
		}
		else
		{
			_bufferLength = buffer;
		}
		if (IsInput)
		{
			UseInput = true;
			return;
		}
		_internalMixer = BassMix.BASS_Mixer_StreamCreate(_samplerate, _numchans, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_AAC_FRAME960);
		if (_internalMixer != 0)
		{
			return;
		}
		throw new NotSupportedException("Internal Mixer: " + Enum.GetName(typeof(BASSError), Un4seen.Bass.Bass.BASS_ErrorGetCode()));
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
				int num = BassWasapi.BASS_WASAPI_GetDevice();
				if (num != _device)
				{
					BassWasapi.BASS_WASAPI_SetDevice(_device);
				}
				RemoveFullDuplex();
				BassWasapi.BASS_WASAPI_Stop(reset: true);
				Un4seen.Bass.Bass.BASS_StreamFree(_internalMixer);
				_internalMixer = 0;
				BassWasapi.BASS_WASAPI_Free();
				BassWasapi.BASS_WASAPI_SetDevice(num);
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

	~BassWasapiHandler()
	{
		Dispose(disposing: false);
	}

	public bool Init(bool buffered = false)
	{
		BASSWASAPIInit bASSWASAPIInit = BASSWASAPIInit.BASS_WASAPI_SHARED;
		if (_exclusive)
		{
			bASSWASAPIInit |= BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE;
		}
		if (_eventSystem)
		{
			bASSWASAPIInit |= BASSWASAPIInit.BASS_WASAPI_EVENT;
		}
		if (buffered)
		{
			bASSWASAPIInit |= BASSWASAPIInit.BASS_WASAPI_BUFFER;
		}
		if (IsInput)
		{
			_internalWasapiProc = WasapiInputCallback;
			return BassWasapi.BASS_WASAPI_Init(_device, _samplerate, _numchans, bASSWASAPIInit, _bufferLength, _updatePeriod, _internalWasapiProc, IntPtr.Zero);
		}
		_internalWasapiProc = WasapiOutputCallback;
		return BassWasapi.BASS_WASAPI_Init(_device, _samplerate, _numchans, bASSWASAPIInit, _bufferLength, _updatePeriod, _internalWasapiProc, IntPtr.Zero);
	}

	public bool Start()
	{
		bool flag = true;
		int num = BassWasapi.BASS_WASAPI_GetDevice();
		if (num != _device)
		{
			flag &= BassWasapi.BASS_WASAPI_SetDevice(_device);
		}
		if (flag && !BassWasapi.BASS_WASAPI_IsStarted())
		{
			flag &= BassWasapi.BASS_WASAPI_Start();
		}
		BassWasapi.BASS_WASAPI_SetDevice(num);
		return flag;
	}

	public bool Stop()
	{
		bool flag = true;
		int num = BassWasapi.BASS_WASAPI_GetDevice();
		if (num != _device)
		{
			flag &= BassWasapi.BASS_WASAPI_SetDevice(_device);
		}
		if (flag && BassWasapi.BASS_WASAPI_IsStarted())
		{
			flag &= BassWasapi.BASS_WASAPI_Stop(reset: true);
		}
		BassWasapi.BASS_WASAPI_SetDevice(num);
		return flag;
	}

	public bool Pause(bool pause)
	{
		bool flag = true;
		int num = BassWasapi.BASS_WASAPI_GetDevice();
		if (pause)
		{
			if (num != _device)
			{
				flag &= BassWasapi.BASS_WASAPI_SetDevice(_device);
			}
			if (flag)
			{
				flag &= BassWasapi.BASS_WASAPI_Stop(reset: false);
			}
		}
		else if (flag)
		{
			flag &= BassWasapi.BASS_WASAPI_SetDevice(_device);
			if (flag)
			{
				flag &= BassWasapi.BASS_WASAPI_Start();
			}
		}
		BassWasapi.BASS_WASAPI_SetDevice(num);
		return flag;
	}

	public bool AddOutputSource(int channel, BASSFlag flags)
	{
		BASS_CHANNELINFO bASS_CHANNELINFO = Un4seen.Bass.Bass.BASS_ChannelGetInfo(channel);
		if (bASS_CHANNELINFO == null)
		{
			return false;
		}
		if (!bASS_CHANNELINFO.IsDecodingChannel && bASS_CHANNELINFO.ctype != BASSChannelType.BASS_CTYPE_RECORD)
		{
			return false;
		}
		if (flags < BASSFlag.BASS_SPEAKER_FRONT)
		{
			flags |= BASSFlag.BASS_WV_STEREO;
		}
		return BassMix.BASS_Mixer_StreamAddChannel(_internalMixer, channel, flags);
	}

	public bool SetFullDuplex(int bassDevice, BASSFlag flags, bool buffered)
	{
		if (!IsInput)
		{
			return false;
		}
		bool flag = true;
		int num = Un4seen.Bass.Bass.BASS_GetDevice();
		if (num != bassDevice)
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
			flags |= BASSFlag.BASS_SAMPLE_FLOAT;
			_outputChannel = Un4seen.Bass.Bass.BASS_StreamCreatePush(_samplerate, _numchans, flags, IntPtr.Zero);
			if (_outputChannel != 0)
			{
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
				_fullDuplex = true;
			}
		}
		Un4seen.Bass.Bass.BASS_SetDevice(num);
		return flag;
	}

	public bool RemoveFullDuplex()
	{
		if (!IsInputFullDuplex)
		{
			return false;
		}
		_fullDuplex = false;
		Un4seen.Bass.Bass.BASS_StreamFree(_outputChannel);
		_outputChannel = 0;
		BypassFullDuplex = false;
		return true;
	}

	public unsafe virtual int WasapiOutputCallback(IntPtr buffer, int length, IntPtr user)
	{
		if (_internalMixer == 0)
		{
			return 0;
		}
		int num = Un4seen.Bass.Bass.BASS_ChannelGetData(_internalMixer, buffer, length);
		if (num <= 0)
		{
			num = 0;
			if (!_mixerStalled)
			{
				_mixerStalled = true;
				RaiseNotification(BassWasapiHandlerSyncType.SourceStalled, _internalMixer);
			}
		}
		else if (_mixerStalled)
		{
			_mixerStalled = false;
			RaiseNotification(BassWasapiHandlerSyncType.SourceResumed, _internalMixer);
		}
		if (num > 0 && (_volL != 1f || _volR != 1f))
		{
			float* ptr = (float*)(void*)buffer;
			for (int i = 0; i < length / 4; i++)
			{
				if (i % 2 == 0)
				{
					ptr[i] *= _volL;
				}
				else
				{
					ptr[i] *= _volR;
				}
			}
		}
		return num;
	}

	public unsafe virtual int WasapiInputCallback(IntPtr buffer, int length, IntPtr user)
	{
		if (length > 0 && (_volL != 1f || _volR != 1f))
		{
			float* ptr = (float*)(void*)buffer;
			for (int i = 0; i < length / 4; i++)
			{
				if (i % 2 == 0)
				{
					ptr[i] *= _volL;
				}
				else
				{
					ptr[i] *= _volR;
				}
			}
		}
		if (_inputChannel != 0)
		{
			Un4seen.Bass.Bass.BASS_ChannelGetData(_inputChannel, buffer, length);
		}
		if (_fullDuplex && !_bypassFullDuplex)
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
		return 1;
	}

	private void SetVolume(float volume, float pan)
	{
		_volL = volume;
		_volR = volume;
		if (_numchans > 1)
		{
			if (pan < 0f)
			{
				_volR = (1f + pan) * volume;
			}
			else if (pan > 0f)
			{
				_volL = (1f - pan) * volume;
			}
		}
	}

	private void RaiseNotification(BassWasapiHandlerSyncType syncType, int data)
	{
		if (this.Notification != null)
		{
			this.Notification(this, new BassWasapiHandlerEventArgs(syncType, data));
		}
	}
}
