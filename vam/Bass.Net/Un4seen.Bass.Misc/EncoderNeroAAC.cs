using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderNeroAAC : BaseEncoder
{
	public bool NERO_UseSSE;

	public bool NERO_UseCustomOptionsOnly;

	public string NERO_CustomOptions = string.Empty;

	public bool NERO_UseQualityMode;

	public float NERO_Quality = 0.4f;

	public int NERO_Bitrate = 128;

	public bool NERO_UseCBR;

	public bool NERO_2Pass;

	public int NERO_2PassPeriod;

	public bool NERO_LC;

	public bool NERO_HE;

	public bool NERO_HEv2;

	public bool NERO_HintTrack;

	public override bool EncoderExists
	{
		get
		{
			if (NERO_UseSSE)
			{
				if (File.Exists(Path.Combine(base.EncoderDirectory, "neroAacEnc_sse2.exe")))
				{
					return true;
				}
				return false;
			}
			if (File.Exists(Path.Combine(base.EncoderDirectory, "neroAacEnc.exe")))
			{
				return true;
			}
			return false;
		}
	}

	public override BASSChannelType EncoderType => BASSChannelType.BASS_CTYPE_STREAM_AAC;

	public override string DefaultOutputExtension => ".m4a";

	public override bool SupportsSTDOUT => false;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate => NERO_Bitrate;

	public EncoderNeroAAC(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "Nero Digital Aac Encoder (.m4a)";
	}

	public override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		BASSEncode bASSEncode = BASSEncode.BASS_ENCODE_DEFAULT;
		if (base.Force16Bit)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_16BIT;
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
		base.EncoderHandle = BassEnc.BASS_Encode_Start(base.ChannelHandle, EncoderCommandLine, bASSEncode, proc, user);
		if (base.EncoderHandle == 0)
		{
			return false;
		}
		return true;
	}

	public override string SettingsString()
	{
		return string.Format("{0}-{1} kbps, {2}", NERO_UseCBR ? "CBR" : "VBR", EffectiveBitrate, NERO_LC ? "LC" : (NERO_HE ? "HE" : (NERO_HEv2 ? "HEv2" : "Auto")));
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (NERO_UseSSE)
		{
			if (!string.IsNullOrEmpty(base.EncoderDirectory))
			{
				stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "neroAacEnc_sse2.exe") + "\"");
			}
			else
			{
				stringBuilder.Append("neroAacEnc_sse2.exe");
			}
		}
		else if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "neroAacEnc.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("neroAacEnc.exe");
		}
		if (NERO_UseCustomOptionsOnly)
		{
			if (NERO_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(NERO_CustomOptions.Trim());
			}
		}
		else
		{
			if (NERO_UseQualityMode)
			{
				stringBuilder.Append(string.Format(provider, " -q {0:0.0####}", NERO_Quality));
			}
			else if (NERO_UseCBR)
			{
				stringBuilder.Append(string.Format(provider, " -cbr {0}", NERO_Bitrate * 1000));
			}
			else
			{
				stringBuilder.Append(string.Format(provider, " -br {0}", NERO_Bitrate * 1000));
			}
			if (base.InputFile != null && NERO_2Pass)
			{
				stringBuilder.Append(" -2pass");
				if (NERO_2PassPeriod > 0)
				{
					stringBuilder.Append(string.Format(provider, " -2passperiod {0}", NERO_2PassPeriod));
				}
			}
			if (NERO_LC)
			{
				stringBuilder.Append(" -lc");
			}
			if (NERO_HE)
			{
				stringBuilder.Append(" -he");
			}
			if (NERO_HEv2)
			{
				stringBuilder.Append(" -hev2");
			}
			if (NERO_HintTrack)
			{
				stringBuilder.Append(" -hinttrack");
			}
			if (base.ChannelInfo.ctype == BASSChannelType.BASS_CTYPE_RECORD)
			{
				stringBuilder.Append(" -ignorelength");
			}
			if (NERO_CustomOptions != null && NERO_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(NERO_CustomOptions.Trim());
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
			stringBuilder.Append(" -of \"" + Path.ChangeExtension(base.InputFile, DefaultOutputExtension) + "\"");
		}
		return stringBuilder.ToString();
	}
}
