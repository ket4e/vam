using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public class EncoderCMDLN : BaseEncoder
{
	public bool CMDLN_UseNOHEAD = true;

	public bool CMDLN_UseFP_32BIT;

	public bool CMDLN_UseFP_24BIT;

	public string CMDLN_Executable = string.Empty;

	public string CMDLN_ParamSTDOUT = "-";

	public string CMDLN_ParamSTDIN = "-";

	public BASSChannelType CMDLN_EncoderType = BASSChannelType.BASS_CTYPE_STREAM_WAV;

	public string CMDLN_DefaultOutputExtension = ".wav";

	public bool CMDLN_SupportsSTDOUT;

	public bool CMDLN_UseVBR;

	public string CMDLN_CBRString = string.Empty;

	public string CMDLN_VBRString = string.Empty;

	public int CMDLN_Bitrate = 128;

	public string CMDLN_Quality = string.Empty;

	public string CMDLN_Mode = string.Empty;

	public string CMDLN_Option = string.Empty;

	public bool CMDLN_UseA = true;

	public string CMDLN_UserA = string.Empty;

	public string CMDLN_UserB = string.Empty;

	public override bool EncoderExists
	{
		get
		{
			if (File.Exists(Path.Combine(base.EncoderDirectory, CMDLN_Executable)))
			{
				return true;
			}
			return false;
		}
	}

	public override BASSChannelType EncoderType => CMDLN_EncoderType;

	public override string DefaultOutputExtension => CMDLN_DefaultOutputExtension;

	public override bool SupportsSTDOUT => CMDLN_SupportsSTDOUT;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate => CMDLN_Bitrate;

	public EncoderCMDLN(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "Generic Command-Line Encoder (" + DefaultOutputExtension + ")";
	}

	public override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		BASSEncode bASSEncode = (CMDLN_UseNOHEAD ? BASSEncode.BASS_ENCODE_NOHEAD : BASSEncode.BASS_ENCODE_DEFAULT);
		if (base.Force16Bit)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_16BIT;
		}
		else if (CMDLN_UseFP_32BIT)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_32BIT;
		}
		else if (CMDLN_UseFP_24BIT)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_FP_24BIT;
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
		return string.Format("{0}-{1} kbps, {2} {3} {4}", CMDLN_UseVBR ? "VBR" : "CBR", EffectiveBitrate, CMDLN_Option, CMDLN_Quality, CMDLN_Mode).Trim();
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo cultureInfo = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, CMDLN_Executable) + "\"");
		}
		else
		{
			stringBuilder.Append(CMDLN_Executable);
		}
		string text = CMDLN_CBRString;
		if (CMDLN_UseVBR)
		{
			text = CMDLN_VBRString;
		}
		text = text.Replace("${user}", CMDLN_UseA ? CMDLN_UserA : CMDLN_UserB).Replace("${Hz}", base.ChannelSampleRate.ToString(cultureInfo)).Replace("${kHz}", ((float)base.ChannelSampleRate / 1000f).ToString("##0.0##", cultureInfo))
			.Replace("${res}", ((base.ChannelBitwidth <= 16) ? base.ChannelBitwidth : (base.Force16Bit ? 16 : (CMDLN_UseFP_24BIT ? 24 : 32))).ToString(cultureInfo))
			.Replace("${chan}", base.ChannelNumChans.ToString(cultureInfo))
			.Replace("${mode}", CMDLN_Mode)
			.Replace("${quality}", CMDLN_Quality)
			.Replace("${option}", CMDLN_Option)
			.Replace("${kbps}", CMDLN_Bitrate.ToString(cultureInfo))
			.Replace("${bps}", (CMDLN_Bitrate * 1000).ToString(cultureInfo))
			.Replace("${input}", (base.InputFile == null) ? CMDLN_ParamSTDIN : ("\"" + base.InputFile + "\""))
			.Replace("${output}", (base.OutputFile == null) ? CMDLN_ParamSTDOUT : ("\"" + base.OutputFile + "\""))
			.Trim();
		stringBuilder.Append(" ");
		stringBuilder.Append(text);
		return stringBuilder.ToString();
	}
}
