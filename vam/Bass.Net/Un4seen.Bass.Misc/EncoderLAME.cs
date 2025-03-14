using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public class EncoderLAME : BaseEncoder
{
	public enum LAMEATH
	{
		Default,
		Only,
		Disable,
		OnlyShortBlocks
	}

	public enum LAMENOASM
	{
		Default,
		NO_MMX,
		NO_3DNOW,
		NO_SSE
	}

	public enum LAMEVBRQuality
	{
		VBR_Q0,
		VBR_Q1,
		VBR_Q2,
		VBR_Q3,
		VBR_Q4,
		VBR_Q5,
		VBR_Q6,
		VBR_Q7,
		VBR_Q8,
		VBR_Q9
	}

	public enum LAMEReplayGain
	{
		Default,
		Fast,
		Accurate,
		None
	}

	public enum LAMEQuality
	{
		Q0,
		Q1,
		Q2,
		Q3,
		Q4,
		Q5,
		Q6,
		Q7,
		Q8,
		Q9,
		None,
		Speed,
		Quality
	}

	public enum LAMEMode
	{
		Default,
		Stereo,
		JointStereo,
		ForcedJointStereo,
		Mono,
		DualMono
	}

	public bool LAME_UseCustomOptionsOnly;

	public string LAME_CustomOptions = string.Empty;

	public LAMEMode LAME_Mode;

	public float LAME_Scale = 1f;

	public string LAME_PresetName = string.Empty;

	public LAMEQuality LAME_Quality = LAMEQuality.Quality;

	public LAMEReplayGain LAME_ReplayGain = LAMEReplayGain.None;

	public bool LAME_FreeFormat;

	public int LAME_Bitrate = 128;

	public int LAME_TargetSampleRate;

	public bool LAME_EnforceCBR;

	public int LAME_ABRBitrate;

	public bool LAME_UseVBR;

	public bool LAME_LimitVBR;

	public LAMEVBRQuality LAME_VBRQuality = LAMEVBRQuality.VBR_Q4;

	public int LAME_VBRMaxBitrate = 320;

	public bool LAME_VBRDisableTag;

	public bool LAME_VBREnforceMinBitrate;

	public bool LAME_Copyright;

	public bool LAME_NonOriginal;

	public bool LAME_Protect;

	public bool LAME_DisableBitReservoir;

	public bool LAME_EnforceISO;

	public bool LAME_DisableAllFilters;

	public bool LAME_PSYuseShortBlocks;

	public bool LAME_PSYnoShortBlocks;

	public bool LAME_PSYallShortBlocks;

	public bool LAME_PSYnoTemp;

	public bool LAME_PSYnsSafeJoint;

	public LAMENOASM LAME_NoASM;

	public int LAME_HighPassFreq;

	public int LAME_HighPassFreqWidth;

	public int LAME_LowPassFreq;

	public int LAME_LowPassFreqWidth;

	public LAMEATH LAME_ATHControl;

	public override bool EncoderExists
	{
		get
		{
			if (File.Exists(Path.Combine(base.EncoderDirectory, "lame.exe")))
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

	public override int EffectiveBitrate
	{
		get
		{
			if (LAME_UseVBR)
			{
				if (LAME_ABRBitrate > 0)
				{
					return LAME_ABRBitrate;
				}
				return (LAME_VBRMaxBitrate + LAME_Bitrate) / 2;
			}
			return LAME_Bitrate;
		}
	}

	public EncoderLAME(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "LAME Encoder (.mp3)";
	}

	public override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		BASSEncode bASSEncode = BASSEncode.BASS_ENCODE_NOHEAD;
		bASSEncode = ((!base.Force16Bit) ? (bASSEncode | BASSEncode.BASS_ENCODE_FP_32BIT) : (bASSEncode | BASSEncode.BASS_ENCODE_FP_16BIT));
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
		return string.Format("{0}-{1} kbps, {2} {3}", LAME_UseVBR ? "VBR" : "CBR", EffectiveBitrate, LAME_Mode, LAME_UseVBR ? LAME_VBRQuality.ToString() : "").Trim();
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "lame.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("lame.exe");
		}
		stringBuilder.Append(string.Format(provider, " -r -s {0:##0.0##} --bitwidth {1}", (float)base.ChannelSampleRate / 1000f, (base.ChannelBitwidth <= 16) ? base.ChannelBitwidth : (base.Force16Bit ? 16 : 32)));
		if (LAME_UseCustomOptionsOnly)
		{
			if (LAME_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(LAME_CustomOptions.Trim());
			}
			if (base.OutputFile == null)
			{
				stringBuilder.Append(" --quiet");
			}
		}
		else
		{
			if (LAME_Scale != 1f)
			{
				stringBuilder.Append(string.Format(provider, " --scale {0:#0.0####}", LAME_Scale));
			}
			if (base.ChannelNumChans == 1)
			{
				LAME_Mode = LAMEMode.Mono;
			}
			if (base.ChannelNumChans > 1 && LAME_Mode == LAMEMode.Mono)
			{
				stringBuilder.Append(" -a");
			}
			else
			{
				switch (LAME_Mode)
				{
				case LAMEMode.Stereo:
					stringBuilder.Append(" -m s");
					break;
				case LAMEMode.JointStereo:
					stringBuilder.Append(" -m j");
					break;
				case LAMEMode.ForcedJointStereo:
					stringBuilder.Append(" -m f");
					break;
				case LAMEMode.DualMono:
					stringBuilder.Append(" -m d");
					break;
				case LAMEMode.Mono:
					stringBuilder.Append(" -m m");
					break;
				}
			}
			if (base.OutputFile == null)
			{
				stringBuilder.Append(" --quiet");
			}
			if (LAME_PresetName != null && LAME_PresetName.Length > 0)
			{
				stringBuilder.Append(string.Format(provider, " --preset {0}", LAME_PresetName.Trim()));
			}
			else
			{
				switch (LAME_Quality)
				{
				case LAMEQuality.Q0:
				case LAMEQuality.Q1:
				case LAMEQuality.Q2:
				case LAMEQuality.Q3:
				case LAMEQuality.Q4:
				case LAMEQuality.Q5:
				case LAMEQuality.Q6:
				case LAMEQuality.Q7:
				case LAMEQuality.Q8:
				case LAMEQuality.Q9:
					stringBuilder.Append(string.Format(provider, " -q {0}", (int)LAME_Quality));
					break;
				case LAMEQuality.Speed:
					stringBuilder.Append(" -f");
					break;
				case LAMEQuality.Quality:
					stringBuilder.Append(" -h");
					break;
				}
				switch (LAME_ReplayGain)
				{
				case LAMEReplayGain.None:
					stringBuilder.Append(" --noreplaygain");
					break;
				case LAMEReplayGain.Fast:
					stringBuilder.Append(" --replaygain-fast");
					break;
				case LAMEReplayGain.Accurate:
					stringBuilder.Append(" --replaygain-accurate");
					break;
				}
				if (LAME_FreeFormat)
				{
					stringBuilder.Append(" --freeformat");
				}
				if (LAME_UseVBR)
				{
					if (LAME_ABRBitrate > 0)
					{
						stringBuilder.Append(string.Format(provider, " --abr {0}", LAME_ABRBitrate));
					}
					else
					{
						stringBuilder.Append(string.Format(provider, " -V {0}", (int)LAME_VBRQuality));
						if (LAME_LimitVBR)
						{
							stringBuilder.Append(string.Format(provider, " -b {0}", LAME_Bitrate));
							stringBuilder.Append($" -B {LAME_VBRMaxBitrate}");
							if (LAME_VBREnforceMinBitrate)
							{
								stringBuilder.Append(" -F");
							}
						}
					}
					if (LAME_VBRDisableTag)
					{
						stringBuilder.Append(" -t");
					}
				}
				else
				{
					stringBuilder.Append(string.Format(provider, " -b {0}", LAME_Bitrate));
					if (LAME_EnforceCBR)
					{
						stringBuilder.Append(" --cbr");
					}
				}
			}
			if (LAME_Copyright)
			{
				stringBuilder.Append(" -c");
			}
			if (LAME_NonOriginal)
			{
				stringBuilder.Append(" -o");
			}
			if (LAME_Protect)
			{
				stringBuilder.Append(" -p");
			}
			if (LAME_DisableBitReservoir)
			{
				stringBuilder.Append(" --nores");
			}
			if (LAME_EnforceISO)
			{
				stringBuilder.Append(" --strictly-enforce-ISO");
			}
			if (LAME_DisableAllFilters)
			{
				stringBuilder.Append(" -k");
			}
			if (LAME_PSYuseShortBlocks)
			{
				stringBuilder.Append(" --short");
			}
			if (LAME_PSYnoShortBlocks)
			{
				stringBuilder.Append(" --noshort");
			}
			if (LAME_PSYallShortBlocks)
			{
				stringBuilder.Append(" --allshort");
			}
			if (LAME_PSYnoTemp)
			{
				stringBuilder.Append(" --notemp");
			}
			if (LAME_PSYnsSafeJoint)
			{
				stringBuilder.Append(" --nssafejoint");
			}
			switch (LAME_ATHControl)
			{
			case LAMEATH.Disable:
				stringBuilder.Append(" --noath");
				break;
			case LAMEATH.Only:
				stringBuilder.Append(" --athonly");
				break;
			case LAMEATH.OnlyShortBlocks:
				stringBuilder.Append(" --athshort");
				break;
			}
			if (LAME_TargetSampleRate > 0)
			{
				stringBuilder.Append(string.Format(provider, " --resample {0:##0.0##}", (float)LAME_TargetSampleRate / 1000f));
			}
			if (LAME_HighPassFreq > 0)
			{
				stringBuilder.Append(string.Format(provider, " --highpass {0}", LAME_HighPassFreq));
				if (LAME_HighPassFreqWidth > 0)
				{
					stringBuilder.Append(string.Format(provider, " --highpass-width {0}", LAME_HighPassFreqWidth));
				}
			}
			if (LAME_LowPassFreq > 0)
			{
				stringBuilder.Append(string.Format(provider, " --lowpass {0}", LAME_LowPassFreq));
				if (LAME_LowPassFreqWidth > 0)
				{
					stringBuilder.Append(string.Format(provider, " --lowpass-width {0}", LAME_LowPassFreqWidth));
				}
			}
			switch (LAME_NoASM)
			{
			case LAMENOASM.NO_MMX:
				stringBuilder.Append(" --noasm mmx");
				break;
			case LAMENOASM.NO_3DNOW:
				stringBuilder.Append(" --noasm 3dnow");
				break;
			case LAMENOASM.NO_SSE:
				stringBuilder.Append(" --noasm sse");
				break;
			}
			if (base.TAGs != null)
			{
				stringBuilder.Append(" --ignore-tag-errors");
				if (!string.IsNullOrEmpty(base.TAGs.title))
				{
					stringBuilder.Append(" --tt \"" + base.TAGs.title.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.artist))
				{
					stringBuilder.Append(" --ta \"" + base.TAGs.artist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.album))
				{
					stringBuilder.Append(" --tl \"" + base.TAGs.album.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.year))
				{
					stringBuilder.Append(" --ty \"" + base.TAGs.year.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.comment))
				{
					stringBuilder.Append(" --tc \"" + base.TAGs.comment.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.track))
				{
					stringBuilder.Append(" --tn \"" + base.TAGs.track.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.genre))
				{
					stringBuilder.Append(" --tg \"" + base.TAGs.genre.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
			}
			if (LAME_CustomOptions != null && LAME_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(LAME_CustomOptions.Trim());
			}
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
