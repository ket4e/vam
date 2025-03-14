using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderTooLAME : BaseEncoder
{
	public enum TOOMode
	{
		Auto,
		Stereo,
		JointStereo,
		DualChannel,
		Mono
	}

	public enum TOOPsycModel
	{
		Default = -2,
		None,
		Fixed,
		ISO_PAM1,
		ISO_PAM2,
		PAM3,
		PAM4
	}

	public enum TOODeEmphMode
	{
		None,
		CCIT_J17,
		Five
	}

	public bool TOO_UseCustomOptionsOnly;

	public string TOO_CustomOptions = string.Empty;

	public TOOMode TOO_Mode;

	public bool TOO_Downmix;

	public int TOO_Bitrate = 192;

	public bool TOO_UseVBR;

	public float TOO_VBRLevel = 5f;

	public TOOPsycModel TOO_PsycModel = TOOPsycModel.Default;

	public float TOO_ATH;

	public int TOO_Quick;

	public bool TOO_Copyright;

	public bool TOO_Original;

	public bool TOO_Protect;

	public bool TOO_Padding;

	public TOODeEmphMode TOO_DeEmphasis;

	public int TOO_DABExtension;

	public override bool EncoderExists
	{
		get
		{
			if (File.Exists(Path.Combine(base.EncoderDirectory, "tooLAME.exe")))
			{
				return true;
			}
			return false;
		}
	}

	public override BASSChannelType EncoderType => BASSChannelType.BASS_CTYPE_STREAM_MP2;

	public override string DefaultOutputExtension => ".mp2";

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate => TOO_Bitrate;

	public new bool Force16Bit => true;

	public EncoderTooLAME(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "tooLAME Encoder (.mp2)";
	}

	public override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		BASSEncode bASSEncode = BASSEncode.BASS_ENCODE_NOHEAD | BASSEncode.BASS_ENCODE_FP_16BIT;
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
		return string.Format("{0}-{1} kbps, {2} {3} {4}", TOO_UseVBR ? "VBR" : "CBR", EffectiveBitrate, TOO_Downmix ? "Mono" : "Stereo", TOO_Mode, TOO_PsycModel).Trim();
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "tooLAME.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("tooLAME.exe");
		}
		stringBuilder.Append(string.Format(provider, " -s {0:#0.0##}", (float)base.ChannelSampleRate / 1000f));
		if (TOO_UseCustomOptionsOnly)
		{
			if (TOO_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(TOO_CustomOptions.Trim());
			}
		}
		else
		{
			if (TOO_Bitrate > 192)
			{
				TOO_Mode = TOOMode.Stereo;
			}
			if (base.ChannelNumChans == 1)
			{
				TOO_Mode = TOOMode.Mono;
			}
			if (base.ChannelNumChans > 1 && TOO_Mode == TOOMode.Mono)
			{
				stringBuilder.Append(" -m s -a");
			}
			else
			{
				switch (TOO_Mode)
				{
				case TOOMode.Stereo:
					stringBuilder.Append(" -m s");
					break;
				case TOOMode.JointStereo:
					stringBuilder.Append(" -m j");
					break;
				case TOOMode.DualChannel:
					stringBuilder.Append(" -m d");
					break;
				case TOOMode.Mono:
					stringBuilder.Append(" -m m");
					break;
				}
			}
			stringBuilder.Append(string.Format(provider, " -b {0}", TOO_Bitrate));
			if (TOO_PsycModel != TOOPsycModel.Default)
			{
				stringBuilder.Append(string.Format(provider, " -P {0}", (int)TOO_PsycModel));
			}
			if (TOO_UseVBR)
			{
				stringBuilder.Append(string.Format(provider, " -v {0:#0.0####}", TOO_VBRLevel));
			}
			if (TOO_ATH != 0f)
			{
				stringBuilder.Append(string.Format(provider, " -l {0:#0.0####}", TOO_ATH));
			}
			if (TOO_Quick > 0)
			{
				stringBuilder.Append(string.Format(provider, " -q {0}", TOO_Quick));
			}
			switch (TOO_DeEmphasis)
			{
			case TOODeEmphMode.CCIT_J17:
				stringBuilder.Append(" -e c");
				break;
			case TOODeEmphMode.Five:
				stringBuilder.Append(" -e 5");
				break;
			}
			if (TOO_Copyright)
			{
				stringBuilder.Append(" -c");
			}
			if (TOO_Original)
			{
				stringBuilder.Append(" -o");
			}
			if (TOO_Protect)
			{
				stringBuilder.Append(" -e");
			}
			if (TOO_Padding)
			{
				stringBuilder.Append(" -r");
			}
			if (TOO_DABExtension > 0)
			{
				stringBuilder.Append(string.Format(provider, " -D {0", TOO_DABExtension));
			}
			if (TOO_CustomOptions != null && TOO_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(TOO_CustomOptions.Trim());
			}
		}
		if (base.OutputFile == null)
		{
			stringBuilder.Append(" -t 0");
		}
		if (base.InputFile != null)
		{
			stringBuilder.Append(" \"" + base.InputFile + "\"");
		}
		else
		{
			stringBuilder.Append(" -");
		}
		if (base.OutputFile != null)
		{
			stringBuilder.Append(" \"" + base.OutputFile + "\"");
		}
		else
		{
			stringBuilder.Append(" -");
		}
		return stringBuilder.ToString();
	}
}
