using System;
using System.IO;
using System.Security;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.Tags;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public abstract class BaseEncoder : IBaseEncoder, IDisposable
{
	public enum BITRATE
	{
		kbps_6 = 6,
		kbps_8 = 8,
		kbps_10 = 10,
		kbps_12 = 12,
		kbps_16 = 16,
		kbps_20 = 20,
		kbps_22 = 22,
		kbps_24 = 24,
		kbps_32 = 32,
		kbps_40 = 40,
		kbps_48 = 48,
		kbps_56 = 56,
		kbps_64 = 64,
		kbps_80 = 80,
		kbps_96 = 96,
		kbps_112 = 112,
		kbps_128 = 128,
		kbps_144 = 144,
		kbps_160 = 160,
		kbps_192 = 192,
		kbps_224 = 224,
		kbps_256 = 256,
		kbps_320 = 320
	}

	public enum SAMPLERATE
	{
		Hz_8000 = 8000,
		Hz_11025 = 11025,
		Hz_16000 = 16000,
		Hz_22050 = 22050,
		Hz_32000 = 32000,
		Hz_44100 = 44100,
		Hz_48000 = 48000,
		Hz_96000 = 96000,
		Hz_192000 = 192000
	}

	public delegate void ENCODEFILEPROC(long bytesTotal, long bytesDone);

	private bool disposed;

	private int _channel;

	private int _bitwidth = 16;

	private int _samplerate = 44100;

	private int _numchans = 2;

	private int _enc;

	private BASS_CHANNELINFO _channelInfo = new BASS_CHANNELINFO();

	private string _encDir = string.Empty;

	private string _inputFile;

	private bool _force16Bit;

	private bool _noLimit;

	private bool _useAsyncQueue;

	private string _outputFile;

	private TAG_INFO _tagInfo;

	public int ChannelHandle
	{
		get
		{
			return _channel;
		}
		set
		{
			if (value != 0 && value == BassEnc.BASS_Encode_GetChannel(_enc))
			{
				return;
			}
			if (IsActive && value != 0)
			{
				if (BassEnc.BASS_Encode_SetChannel(_enc, value))
				{
					InitChannel(value);
				}
			}
			else
			{
				InitChannel(value);
			}
		}
	}

	public BASS_CHANNELINFO ChannelInfo => _channelInfo;

	public int ChannelBitwidth => _bitwidth;

	public int ChannelSampleRate => _samplerate;

	public int ChannelNumChans => _numchans;

	public abstract bool SupportsSTDOUT { get; }

	public abstract BASSChannelType EncoderType { get; }

	public abstract string DefaultOutputExtension { get; }

	public int EncoderHandle
	{
		get
		{
			return _enc;
		}
		set
		{
			_enc = value;
		}
	}

	public virtual bool IsActive
	{
		get
		{
			if (EncoderHandle == 0)
			{
				return false;
			}
			return BassEnc.BASS_Encode_IsActive(EncoderHandle) != BASSActive.BASS_ACTIVE_STOPPED;
		}
	}

	public virtual bool IsPaused
	{
		get
		{
			if (EncoderHandle == 0)
			{
				return false;
			}
			return BassEnc.BASS_Encode_IsActive(EncoderHandle) == BASSActive.BASS_ACTIVE_PAUSED;
		}
	}

	public string EncoderDirectory
	{
		get
		{
			return _encDir;
		}
		set
		{
			_encDir = value;
		}
	}

	public abstract string EncoderCommandLine { get; }

	public virtual bool EncoderExists => true;

	public string InputFile
	{
		get
		{
			return _inputFile;
		}
		set
		{
			_inputFile = value;
		}
	}

	public bool Force16Bit
	{
		get
		{
			return _force16Bit;
		}
		set
		{
			_force16Bit = value;
		}
	}

	public bool NoLimit
	{
		get
		{
			return _noLimit;
		}
		set
		{
			_noLimit = value;
		}
	}

	public bool UseAsyncQueue
	{
		get
		{
			return _useAsyncQueue;
		}
		set
		{
			_useAsyncQueue = value;
		}
	}

	public string OutputFile
	{
		get
		{
			return _outputFile;
		}
		set
		{
			_outputFile = value;
		}
	}

	public virtual int EffectiveBitrate => 128;

	public TAG_INFO TAGs
	{
		get
		{
			return _tagInfo;
		}
		set
		{
			_tagInfo = value;
		}
	}

	public BaseEncoder(int channel)
	{
		InitChannel(channel);
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

	~BaseEncoder()
	{
		Dispose(disposing: false);
	}

	private void InitChannel(int channel)
	{
		_channel = channel;
		if (_channel != 0)
		{
			if (!Bass.BASS_ChannelGetInfo(channel, _channelInfo))
			{
				_channelInfo.chans = 2;
				_channelInfo.ctype = BASSChannelType.BASS_CTYPE_UNKNOWN;
				_channelInfo.freq = 44100;
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
		else
		{
			_channel = 0;
			_bitwidth = 16;
			_samplerate = 44100;
			_numchans = 2;
			_channelInfo = new BASS_CHANNELINFO();
		}
	}

	public abstract bool Start(ENCODEPROC proc, IntPtr user, bool paused);

	public virtual bool Stop()
	{
		if (EncoderHandle == 0)
		{
			return true;
		}
		if (_useAsyncQueue)
		{
			if (BassEnc.BASS_Encode_StopEx(EncoderHandle, queue: false))
			{
				EncoderHandle = 0;
				return true;
			}
			if (BassEnc.BASS_Encode_Stop(EncoderHandle))
			{
				EncoderHandle = 0;
				return true;
			}
		}
		else if (BassEnc.BASS_Encode_Stop(EncoderHandle))
		{
			EncoderHandle = 0;
			return true;
		}
		return false;
	}

	public virtual bool Stop(bool queue)
	{
		if (EncoderHandle == 0)
		{
			return true;
		}
		if (_useAsyncQueue)
		{
			if (BassEnc.BASS_Encode_StopEx(EncoderHandle, queue))
			{
				EncoderHandle = 0;
				return true;
			}
			return false;
		}
		return Stop();
	}

	public virtual bool Pause(bool paused)
	{
		if (EncoderHandle == 0)
		{
			return false;
		}
		return BassEnc.BASS_Encode_SetPaused(EncoderHandle, paused);
	}

	public virtual string SettingsString()
	{
		return $"{EffectiveBitrate} kbps";
	}

	public static bool EncodeFile(string inputFile, string outputFile, BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool deleteInput)
	{
		return EncodeFileInternal(inputFile, outputFile, encoder, proc, overwriteOutput, deleteInput, updateTags: false, -1L, -1L);
	}

	public static bool EncodeFile(BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool deleteInput)
	{
		return EncodeFileInternal(encoder.InputFile, encoder.OutputFile, encoder, proc, overwriteOutput, deleteInput, updateTags: false, -1L, -1L);
	}

	public static bool EncodeFile(string inputFile, string outputFile, BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool deleteInput, bool updateTags)
	{
		return EncodeFileInternal(inputFile, outputFile, encoder, proc, overwriteOutput, deleteInput, updateTags, -1L, -1L);
	}

	public static bool EncodeFile(BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool deleteInput, bool updateTags)
	{
		return EncodeFileInternal(encoder.InputFile, encoder.OutputFile, encoder, proc, overwriteOutput, deleteInput, updateTags, -1L, -1L);
	}

	public static bool EncodeFile(string inputFile, string outputFile, BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool deleteInput, bool updateTags, long fromPos, long toPos)
	{
		return EncodeFileInternal(inputFile, outputFile, encoder, proc, overwriteOutput, deleteInput, updateTags, fromPos, toPos);
	}

	public static bool EncodeFile(BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool deleteInput, bool updateTags, long fromPos, long toPos)
	{
		return EncodeFileInternal(encoder.InputFile, encoder.OutputFile, encoder, proc, overwriteOutput, deleteInput, updateTags, fromPos, toPos);
	}

	public static bool EncodeFile(string inputFile, string outputFile, BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool deleteInput, bool updateTags, double fromPos, double toPos)
	{
		return EncodeFileInternal(inputFile, outputFile, encoder, proc, overwriteOutput, deleteInput, updateTags, fromPos, toPos);
	}

	public static bool EncodeFile(BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool deleteInput, bool updateTags, double fromPos, double toPos)
	{
		return EncodeFileInternal(encoder.InputFile, encoder.OutputFile, encoder, proc, overwriteOutput, deleteInput, updateTags, fromPos, toPos);
	}

	private static bool EncodeFileInternal(string inputFile, string outputFile, BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool deleteInput, bool updateTags, long fromPos, long toPos)
	{
		if (encoder == null || (inputFile == null && encoder.ChannelHandle == 0))
		{
			return false;
		}
		if (toPos <= fromPos)
		{
			fromPos = -1L;
			toPos = -1L;
		}
		bool flag = false;
		string inputFile2 = encoder.InputFile;
		string outputFile2 = encoder.OutputFile;
		int channelHandle = encoder.ChannelHandle;
		int num = 0;
		try
		{
			if (inputFile == null)
			{
				if (encoder.ChannelHandle == 0 || encoder.ChannelInfo == null || string.IsNullOrEmpty(encoder.ChannelInfo.filename))
				{
					return false;
				}
				num = encoder.ChannelHandle;
				inputFile = encoder.ChannelInfo.filename;
			}
			else
			{
				num = Bass.BASS_StreamCreateFile(inputFile, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | ((fromPos >= 0 || toPos >= 0) ? BASSFlag.BASS_STREAM_PRESCAN : BASSFlag.BASS_DEFAULT));
			}
			flag = EncodeFileInternal2(num, inputFile, outputFile, encoder, proc, overwriteOutput, updateTags, fromPos, toPos);
		}
		catch
		{
			flag = false;
		}
		finally
		{
			if (num != 0)
			{
				Bass.BASS_StreamFree(num);
			}
			encoder.InputFile = inputFile2;
			encoder.OutputFile = outputFile2;
			encoder.ChannelHandle = channelHandle;
			if (flag && deleteInput)
			{
				try
				{
					File.Delete(inputFile);
				}
				catch
				{
				}
			}
		}
		return flag;
	}

	private static bool EncodeFileInternal(string inputFile, string outputFile, BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool deleteInput, bool updateTags, double fromPos, double toPos)
	{
		if (encoder == null || (inputFile == null && encoder.ChannelHandle == 0))
		{
			return false;
		}
		if (toPos <= fromPos)
		{
			fromPos = -1.0;
			toPos = -1.0;
		}
		bool flag = false;
		string inputFile2 = encoder.InputFile;
		string outputFile2 = encoder.OutputFile;
		int channelHandle = encoder.ChannelHandle;
		int num = 0;
		try
		{
			if (inputFile == null)
			{
				if (encoder.ChannelHandle == 0 || encoder.ChannelInfo == null || string.IsNullOrEmpty(encoder.ChannelInfo.filename))
				{
					return false;
				}
				num = encoder.ChannelHandle;
				inputFile = encoder.ChannelInfo.filename;
			}
			else
			{
				num = Bass.BASS_StreamCreateFile(inputFile, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | ((!(fromPos < 0.0) || !(toPos < 0.0)) ? BASSFlag.BASS_STREAM_PRESCAN : BASSFlag.BASS_DEFAULT));
			}
			flag = EncodeFileInternal2(num, inputFile, outputFile, encoder, proc, overwriteOutput, updateTags, Bass.BASS_ChannelSeconds2Bytes(num, fromPos), Bass.BASS_ChannelSeconds2Bytes(num, toPos));
		}
		catch
		{
			flag = false;
		}
		finally
		{
			if (num != 0)
			{
				Bass.BASS_StreamFree(num);
			}
			encoder.InputFile = inputFile2;
			encoder.OutputFile = outputFile2;
			encoder.ChannelHandle = channelHandle;
			if (flag && deleteInput)
			{
				try
				{
					File.Delete(inputFile);
				}
				catch
				{
				}
			}
		}
		return flag;
	}

	private static bool EncodeFileInternal2(int stream, string inputFile, string outputFile, BaseEncoder encoder, ENCODEFILEPROC proc, bool overwriteOutput, bool updateTags, long fromPos, long toPos)
	{
		bool result = false;
		if (stream != 0)
		{
			long num = 0L;
			long num2 = 0L;
			num = Bass.BASS_ChannelGetLength(stream);
			if (toPos > 0 && toPos > num)
			{
				toPos = num;
			}
			if (updateTags)
			{
				TAG_INFO tAG_INFO = new TAG_INFO(inputFile);
				if (BassTags.BASS_TAG_GetFromFile(stream, tAG_INFO))
				{
					encoder.TAGs = tAG_INFO;
				}
			}
			if (num > 0)
			{
				if (outputFile == null)
				{
					outputFile = Path.ChangeExtension(inputFile, encoder.DefaultOutputExtension);
					while (outputFile.Equals(inputFile))
					{
						outputFile += encoder.DefaultOutputExtension;
					}
				}
				if (File.Exists(outputFile))
				{
					if (!overwriteOutput)
					{
						throw new IOException($"The output file {outputFile} already exists!");
					}
					File.Delete(outputFile);
				}
				encoder.InputFile = null;
				encoder.OutputFile = outputFile;
				encoder.ChannelHandle = stream;
				if (toPos > 0)
				{
					num = toPos;
				}
				if (fromPos > 0)
				{
					Bass.BASS_ChannelSetPosition(stream, fromPos);
					num -= fromPos;
				}
				int num3 = 262144;
				int num4 = 0;
				long num5 = toPos - fromPos;
				bool wMA_UseNetwork = false;
				if (encoder is EncoderWMA)
				{
					wMA_UseNetwork = ((EncoderWMA)encoder).WMA_UseNetwork;
					((EncoderWMA)encoder).WMA_UseNetwork = false;
				}
				if (encoder.Start(null, IntPtr.Zero, paused: false))
				{
					byte[] buffer = new byte[num3];
					while (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
					{
						if (fromPos < 0 && toPos < 0)
						{
							num2 += Bass.BASS_ChannelGetData(stream, buffer, num3);
							proc?.Invoke(num, num2);
							continue;
						}
						num4 = ((num5 >= num3) ? Bass.BASS_ChannelGetData(stream, buffer, num3) : Bass.BASS_ChannelGetData(stream, buffer, (int)num5));
						num2 += num4;
						num5 -= num4;
						proc?.Invoke(num, num2);
						if (num5 <= 0)
						{
							break;
						}
					}
					if (encoder.Stop())
					{
						result = true;
					}
				}
				if (encoder is EncoderWMA)
				{
					((EncoderWMA)encoder).WMA_UseNetwork = wMA_UseNetwork;
				}
			}
		}
		return result;
	}
}
