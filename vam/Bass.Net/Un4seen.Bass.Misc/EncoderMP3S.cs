using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public class EncoderMP3S : BaseEncoder
{
	public enum MP3SVBRQuality
	{
		Highest = 1,
		High,
		Intermediate,
		Medium,
		Low
	}

	public bool MP3S_UseCustomOptionsOnly;

	public string MP3S_CustomOptions = string.Empty;

	public bool MP3S_Mono;

	public bool MP3S_Quality;

	public int MP3S_Bitrate = 128;

	public bool MP3S_UseVBR;

	public MP3SVBRQuality MP3S_VBRQuality = MP3SVBRQuality.High;

	public bool MP3S_WriteVBRHeader;

	public override bool EncoderExists
	{
		get
		{
			if (File.Exists(Path.Combine(base.EncoderDirectory, "mp3sEncoder.exe")))
			{
				return true;
			}
			return false;
		}
	}

	public override BASSChannelType EncoderType => BASSChannelType.BASS_CTYPE_STREAM_MP3;

	public override string DefaultOutputExtension => ".mp3";

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate => MP3S_Bitrate;

	public EncoderMP3S(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "Fraunhofer IIS MP3 Surround Encoder (.mp3)";
	}

	public override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		BASSEncode bASSEncode = BASSEncode.BASS_ENCODE_NOHEAD;
		bASSEncode = ((!base.Force16Bit) ? (bASSEncode | BASSEncode.BASS_ENCODE_FP_24BIT) : (bASSEncode | BASSEncode.BASS_ENCODE_FP_16BIT));
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
		base.EncoderHandle = BassEnc.BASS_Encode_Start(base.ChannelHandle, EncoderCommandLine, bASSEncode, proc, user);
		if (base.EncoderHandle == 0)
		{
			return false;
		}
		return true;
	}

	public override string SettingsString()
	{
		return string.Format("{0}-{1} kbps, {2} {3} {4}", MP3S_UseVBR ? "VBR" : "CBR", EffectiveBitrate, MP3S_Mono ? "Mono" : "Stereo", MP3S_Quality ? "HQ" : "Fast", MP3S_UseVBR ? MP3S_VBRQuality.ToString() : "").Trim();
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "mp3sEncoder.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("mp3sEncoder.exe");
		}
		stringBuilder.Append(string.Format(provider, " -raw -sr {0} -c {1} -res {2}", base.ChannelSampleRate, base.ChannelNumChans, (base.ChannelBitwidth <= 16) ? base.ChannelBitwidth : (base.Force16Bit ? 16 : 24)));
		if (MP3S_UseCustomOptionsOnly)
		{
			if (MP3S_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(MP3S_CustomOptions.Trim());
			}
		}
		else
		{
			if (base.ChannelNumChans > 1 && MP3S_Mono)
			{
				stringBuilder.Append(" -mono");
			}
			if (MP3S_Quality)
			{
				stringBuilder.Append(" -q 1");
			}
			if (MP3S_UseVBR)
			{
				stringBuilder.Append(string.Format(provider, " -m {0}", (int)MP3S_VBRQuality));
			}
			else
			{
				stringBuilder.Append(string.Format(provider, " -br {0}", MP3S_Bitrate * 1000));
			}
			if (MP3S_UseVBR && MP3S_WriteVBRHeader)
			{
				stringBuilder.Append(" -vbri");
			}
			if (MP3S_CustomOptions != null && MP3S_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(MP3S_CustomOptions.Trim());
			}
		}
		if (base.InputFile != null)
		{
			stringBuilder.Append(" -if \"" + base.InputFile + "\"");
		}
		else
		{
			stringBuilder.Append(" -if -");
		}
		if (base.OutputFile != null)
		{
			stringBuilder.Append(" -of \"" + base.OutputFile + "\"");
		}
		else
		{
			stringBuilder.Append(" -of -");
		}
		return stringBuilder.ToString();
	}
}
