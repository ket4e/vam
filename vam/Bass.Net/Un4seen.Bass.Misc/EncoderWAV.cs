using System;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderWAV : BaseEncoder
{
	private int _wavBitsPerSample = 16;

	private bool _wavUse32BitInteger;

	private bool _wavAddRiffInfo;

	private bool _wavUseAIFF;

	private bool _bwfUseRF64;

	private bool _bwfAddBWF;

	private bool _bwfAddCART;

	public BASSChannelType WAV_EncoderType = BASSChannelType.BASS_CTYPE_STREAM_WAV;

	public string WAV_DefaultOutputExtension = ".wav";

	public override BASSChannelType EncoderType => WAV_EncoderType;

	public override string DefaultOutputExtension => WAV_DefaultOutputExtension;

	public override bool SupportsSTDOUT => false;

	public override string EncoderCommandLine => base.OutputFile;

	public override int EffectiveBitrate => base.ChannelSampleRate * WAV_BitsPerSample * base.ChannelNumChans / 1000;

	public new bool Force16Bit => false;

	public int WAV_BitsPerSample
	{
		get
		{
			return _wavBitsPerSample;
		}
		set
		{
			if (value == 8 || value == 16 || value == 24 || value == 32)
			{
				_wavBitsPerSample = value;
			}
		}
	}

	public bool WAV_Use32BitInteger
	{
		get
		{
			return _wavUse32BitInteger;
		}
		set
		{
			_wavUse32BitInteger = value;
		}
	}

	public bool WAV_AddRiffInfo
	{
		get
		{
			return _wavAddRiffInfo;
		}
		set
		{
			_wavAddRiffInfo = value;
		}
	}

	public bool WAV_UseAIFF
	{
		get
		{
			return _wavUseAIFF;
		}
		set
		{
			_wavUseAIFF = value;
			if (_wavUseAIFF)
			{
				WAV_EncoderType = BASSChannelType.BASS_CTYPE_STREAM_AIFF;
				WAV_DefaultOutputExtension = ".aif";
			}
			else
			{
				WAV_EncoderType = BASSChannelType.BASS_CTYPE_STREAM_WAV;
				WAV_DefaultOutputExtension = ".wav";
			}
		}
	}

	public bool BWF_UseRF64
	{
		get
		{
			return _bwfUseRF64;
		}
		set
		{
			_bwfUseRF64 = value;
		}
	}

	public bool BWF_AddBEXT
	{
		get
		{
			return _bwfAddBWF;
		}
		set
		{
			_bwfAddBWF = value;
		}
	}

	public bool BWF_AddCART
	{
		get
		{
			return _bwfAddCART;
		}
		set
		{
			_bwfAddCART = value;
		}
	}

	public EncoderWAV(int channel)
		: base(channel)
	{
		if (channel != 0)
		{
			_wavBitsPerSample = base.ChannelBitwidth;
		}
	}

	public override string ToString()
	{
		if (_wavUseAIFF)
		{
			return "AIFF Encoder (.aif)";
		}
		return "RIFF WAVE (.wav)";
	}

	public override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		if (base.OutputFile == null)
		{
			return false;
		}
		int num = base.ChannelHandle;
		if (base.InputFile != null)
		{
			num = Bass.BASS_StreamCreateFile(base.InputFile, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE);
			if (num == 0)
			{
				return false;
			}
		}
		BASSEncode bASSEncode = BASSEncode.BASS_ENCODE_PCM;
		if (_wavUseAIFF)
		{
			bASSEncode = BASSEncode.BASS_ENCODE_AIFF;
		}
		if (_wavBitsPerSample == 16)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_16BIT;
		}
		else if (_wavBitsPerSample == 24)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_24BIT;
		}
		else if (_wavBitsPerSample == 8)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_8BIT;
		}
		else if (WAV_Use32BitInteger)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_32BIT;
		}
		if (BWF_UseRF64)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_RF64;
		}
		if (paused || (base.TAGs != null && !_wavUseAIFF))
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_PAUSE;
		}
		if (base.NoLimit)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_CAST_NOLIMIT;
		}
		if (base.UseAsyncQueue)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_QUEUE;
		}
		if (_wavBitsPerSample > 16 || base.ChannelSampleRate > 48000 || base.ChannelNumChans > 2)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_WFEXT;
		}
		base.EncoderHandle = BassEnc.BASS_Encode_Start(num, base.OutputFile, bASSEncode, null, IntPtr.Zero);
		if (base.TAGs != null && !_wavUseAIFF)
		{
			if (BWF_AddBEXT)
			{
				byte[] array = base.TAGs.ConvertToRiffBEXT(fromNativeTags: true);
				if (array != null && array.Length != 0)
				{
					BassEnc.BASS_Encode_AddChunk(base.EncoderHandle, "bext", array, array.Length);
				}
			}
			if (BWF_AddCART)
			{
				byte[] array2 = base.TAGs.ConvertToRiffCART(fromNativeTags: true);
				if (array2 != null && array2.Length != 0)
				{
					BassEnc.BASS_Encode_AddChunk(base.EncoderHandle, "cart", array2, array2.Length);
				}
			}
			if (WAV_AddRiffInfo)
			{
				byte[] array3 = base.TAGs.ConvertToRiffINFO(fromNativeTags: false);
				if (array3 != null && array3.Length != 0)
				{
					BassEnc.BASS_Encode_AddChunk(base.EncoderHandle, "LIST", array3, array3.Length);
				}
			}
		}
		if (base.TAGs != null && !_wavUseAIFF)
		{
			BassEnc.BASS_Encode_SetPaused(base.EncoderHandle, paused);
		}
		if (base.InputFile != null)
		{
			Utils.DecodeAllData(num, autoFree: true);
		}
		if (base.EncoderHandle == 0)
		{
			return false;
		}
		return true;
	}

	public override string SettingsString()
	{
		return string.Format("{0} kbps, {1}-bit {2}", EffectiveBitrate, WAV_BitsPerSample, (WAV_BitsPerSample != 32) ? "" : (WAV_Use32BitInteger ? "Linear" : "Float")).Trim();
	}
}
