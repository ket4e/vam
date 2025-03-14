using System;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderAIFF : BaseEncoder
{
	private int _aiffBitsPerSample = 16;

	private bool _aiffUse32BitInteger;

	public BASSChannelType AIFF_EncoderType = BASSChannelType.BASS_CTYPE_STREAM_AIFF;

	public string AIFF_DefaultOutputExtension = ".aif";

	public override BASSChannelType EncoderType => AIFF_EncoderType;

	public override string DefaultOutputExtension => AIFF_DefaultOutputExtension;

	public override bool SupportsSTDOUT => false;

	public override string EncoderCommandLine => base.OutputFile;

	public override int EffectiveBitrate => base.ChannelSampleRate * AIFF_BitsPerSample * base.ChannelNumChans / 1000;

	public new bool Force16Bit => false;

	public int AIFF_BitsPerSample
	{
		get
		{
			return _aiffBitsPerSample;
		}
		set
		{
			if (value == 8 || value == 16 || value == 24 || value == 32)
			{
				_aiffBitsPerSample = value;
			}
		}
	}

	public bool AIFF_Use32BitInteger
	{
		get
		{
			return _aiffUse32BitInteger;
		}
		set
		{
			_aiffUse32BitInteger = value;
		}
	}

	public EncoderAIFF(int channel)
		: base(channel)
	{
		if (channel != 0)
		{
			_aiffBitsPerSample = base.ChannelBitwidth;
		}
	}

	public override string ToString()
	{
		return "AIFF Encoder (.aif)";
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
		BASSEncode bASSEncode = BASSEncode.BASS_ENCODE_AIFF;
		if (_aiffBitsPerSample == 16)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_16BIT;
		}
		else if (_aiffBitsPerSample == 24)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_24BIT;
		}
		else if (_aiffBitsPerSample == 8)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_8BIT;
		}
		else if (AIFF_Use32BitInteger)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_32BIT;
		}
		if (paused)
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
		base.EncoderHandle = BassEnc.BASS_Encode_Start(num, base.OutputFile, bASSEncode, null, IntPtr.Zero);
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
		return string.Format("{0} kbps, {1}-bit {2}", EffectiveBitrate, AIFF_BitsPerSample, (AIFF_BitsPerSample != 32) ? "" : (AIFF_Use32BitInteger ? "Linear" : "Float")).Trim();
	}
}
