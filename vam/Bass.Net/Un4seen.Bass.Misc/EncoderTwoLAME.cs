using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderTwoLAME : BaseEncoder
{
	public enum TWOMode
	{
		Auto,
		Stereo,
		JointStereo,
		DualChannel,
		Mono
	}

	public enum TWOPsycModel
	{
		Default = -2,
		None,
		Fixed,
		ISO_PAM1,
		ISO_PAM2,
		PAM3,
		PAM4
	}

	public enum TWODeEmphMode
	{
		None,
		CCIT_J17,
		Five
	}

	public bool TWO_UseCustomOptionsOnly;

	public string TWO_CustomOptions = string.Empty;

	public TWOMode TWO_Mode;

	public float TWO_Scale = 1f;

	public bool TWO_Downmix;

	public int TWO_Bitrate = 256;

	public int TWO_MaxBitrate;

	public bool TWO_UseVBR;

	public float TWO_VBRLevel = -100f;

	public TWOPsycModel TWO_PsycModel = TWOPsycModel.Default;

	public float TWO_ATH;

	public int TWO_Quick;

	public bool TWO_Copyright;

	public bool TWO_NonOriginal;

	public bool TWO_Protect;

	public bool TWO_Padding;

	public int TWO_Reserve;

	public TWODeEmphMode TWO_DeEmphasis;

	public bool TWO_Energy;

	public override bool EncoderExists
	{
		get
		{
			if (File.Exists(Path.Combine(base.EncoderDirectory, "twolame.exe")))
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

	public override int EffectiveBitrate => TWO_Bitrate;

	public EncoderTwoLAME(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "TwoLAME Encoder (.mp2)";
	}

	public override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		BASSEncode bASSEncode = BASSEncode.BASS_ENCODE_NOHEAD;
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
		return string.Format("{0}-{1} kbps, {2} {3} {4}", TWO_UseVBR ? "VBR" : "CBR", EffectiveBitrate, TWO_Downmix ? "Mono" : "Stereo", TWO_Mode, TWO_PsycModel).Trim();
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "twolame.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("twolame.exe");
		}
		if (base.ChannelBitwidth > 24)
		{
			stringBuilder.Append(string.Format(provider, " -r -s {0} --samplefloat -N {1}", base.ChannelSampleRate, base.ChannelNumChans));
		}
		else
		{
			stringBuilder.Append(string.Format(provider, " -r -s {0} --samplesize {1} -N {2}", base.ChannelSampleRate, (base.ChannelBitwidth <= 16) ? base.ChannelBitwidth : (base.Force16Bit ? 16 : 32), base.ChannelNumChans));
		}
		if (TWO_UseCustomOptionsOnly)
		{
			if (TWO_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(TWO_CustomOptions.Trim());
			}
		}
		else
		{
			if (TWO_Scale != 1f)
			{
				stringBuilder.Append(string.Format(provider, " --scale {0:0.0####}", TWO_Scale));
			}
			if (base.ChannelNumChans == 1)
			{
				TWO_Mode = TWOMode.Mono;
			}
			if (base.ChannelNumChans > 1 && TWO_Mode == TWOMode.Mono)
			{
				stringBuilder.Append(" -m s -a");
			}
			else
			{
				switch (TWO_Mode)
				{
				case TWOMode.Stereo:
					stringBuilder.Append(" -m s");
					break;
				case TWOMode.JointStereo:
					stringBuilder.Append(" -m j");
					break;
				case TWOMode.DualChannel:
					stringBuilder.Append(" -m d");
					break;
				case TWOMode.Mono:
					stringBuilder.Append(" -m m");
					break;
				}
			}
			stringBuilder.Append(string.Format(provider, " -b {0}", TWO_Bitrate));
			if (TWO_PsycModel != TWOPsycModel.Default)
			{
				stringBuilder.Append(string.Format(provider, " -P {0}", (int)TWO_PsycModel));
			}
			if (TWO_UseVBR)
			{
				stringBuilder.Append(" -v");
				if (TWO_VBRLevel >= -50f && TWO_VBRLevel <= 50f)
				{
					stringBuilder.Append(string.Format(provider, " -V {0:#0.0####}", TWO_VBRLevel));
				}
				if (TWO_MaxBitrate > 0)
				{
					stringBuilder.Append(string.Format(provider, " -B {0}", TWO_MaxBitrate));
				}
			}
			if (TWO_ATH != 0f)
			{
				stringBuilder.Append(string.Format(provider, " -l {0:#0.0####}", TWO_ATH));
			}
			if (TWO_Quick > 0)
			{
				stringBuilder.Append(string.Format(provider, " -q {0}", TWO_Quick));
			}
			if (TWO_Copyright)
			{
				stringBuilder.Append(" -c");
			}
			if (TWO_NonOriginal)
			{
				stringBuilder.Append(" -o");
			}
			if (TWO_Protect)
			{
				stringBuilder.Append(" -p");
			}
			if (TWO_Padding)
			{
				stringBuilder.Append(" -d");
			}
			if (TWO_Reserve > 0)
			{
				stringBuilder.Append(string.Format(provider, " -R {0}", TWO_Reserve / 8 * 8));
			}
			switch (TWO_DeEmphasis)
			{
			case TWODeEmphMode.CCIT_J17:
				stringBuilder.Append(" -e c");
				break;
			case TWODeEmphMode.Five:
				stringBuilder.Append(" -e 5");
				break;
			}
			if (TWO_Energy)
			{
				stringBuilder.Append(" -E");
			}
			if (TWO_CustomOptions != null && TWO_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(TWO_CustomOptions.Trim());
			}
		}
		if (base.OutputFile == null)
		{
			stringBuilder.Append(" --quiet");
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
