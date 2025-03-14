using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderFAAC : BaseEncoder
{
	public bool FAAC_UseCustomOptionsOnly;

	public string FAAC_CustomOptions = string.Empty;

	public bool FAAC_UseQualityMode;

	public int FAAC_Quality = 100;

	public int FAAC_Bitrate = 120;

	public int FAAC_Bandwidth = -1;

	public bool FAAC_WrapMP4 = true;

	public bool FAAC_EnableTNS;

	public bool FAAC_NoMidSide;

	public int FAAC_MpegVersion = -1;

	public string FAAC_ObjectType = "LC";

	public int FAAC_BlockType;

	public bool FAAC_RawBitstream;

	public override bool EncoderExists
	{
		get
		{
			if (File.Exists(Path.Combine(base.EncoderDirectory, "faac.exe")))
			{
				return true;
			}
			return false;
		}
	}

	public override BASSChannelType EncoderType => BASSChannelType.BASS_CTYPE_STREAM_AAC;

	public override string DefaultOutputExtension => ".m4a";

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate
	{
		get
		{
			if (FAAC_UseQualityMode)
			{
				return Quality2Kbps(FAAC_Quality);
			}
			return FAAC_Bitrate;
		}
	}

	public EncoderFAAC(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "FAAC Encoder (.m4a)";
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
		return string.Format("{0}-{1} kbps, {2} {3}", FAAC_UseQualityMode ? "VBR" : "CBR", EffectiveBitrate, FAAC_ObjectType, FAAC_EnableTNS ? "TNS" : "").Trim();
	}

	public int Quality2Kbps(int q)
	{
		return (int)Math.Round((double)q * (1.2 * Math.Cos(q)) + 10.0);
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "faac.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("faac.exe");
		}
		stringBuilder.Append(string.Format(provider, " -P -R {0} {1} -C {2}", base.ChannelSampleRate, (base.ChannelBitwidth <= 16) ? ("-B " + base.ChannelBitwidth + " -X") : (base.Force16Bit ? "-B 16 -X" : "-F -B 32"), base.ChannelNumChans));
		if (FAAC_UseCustomOptionsOnly)
		{
			if (FAAC_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(FAAC_CustomOptions.Trim());
			}
		}
		else
		{
			if (FAAC_UseQualityMode)
			{
				stringBuilder.Append($" -q {FAAC_Quality}");
			}
			else
			{
				stringBuilder.Append($" -b {FAAC_Bitrate}");
			}
			if (FAAC_Bandwidth > 0)
			{
				stringBuilder.Append($" -c {FAAC_Bandwidth}");
			}
			if (FAAC_WrapMP4)
			{
				stringBuilder.Append(" -w");
			}
			if (FAAC_WrapMP4 && base.TAGs != null)
			{
				if (!string.IsNullOrEmpty(base.TAGs.title))
				{
					stringBuilder.Append(" --title \"" + base.TAGs.title.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.artist))
				{
					stringBuilder.Append(" --artist \"" + base.TAGs.artist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.album))
				{
					stringBuilder.Append(" --album \"" + base.TAGs.album.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.year))
				{
					stringBuilder.Append(" --year \"" + base.TAGs.year.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.track))
				{
					stringBuilder.Append(" --track \"" + base.TAGs.track.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.disc))
				{
					stringBuilder.Append(" --disc \"" + base.TAGs.disc.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.genre))
				{
					stringBuilder.Append(" --genre \"" + base.TAGs.genre.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.comment))
				{
					stringBuilder.Append(" --comment \"" + base.TAGs.comment.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.composer))
				{
					stringBuilder.Append(" --writer \"" + base.TAGs.composer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
			}
			if (FAAC_EnableTNS)
			{
				stringBuilder.Append(" --tns");
			}
			if (FAAC_NoMidSide)
			{
				stringBuilder.Append(" --no-midside");
			}
			if (FAAC_MpegVersion == 2 || FAAC_MpegVersion == 4)
			{
				stringBuilder.Append($" --mpeg-vers {FAAC_MpegVersion}");
			}
			if (FAAC_ObjectType == "Main" || FAAC_ObjectType == "LTP")
			{
				stringBuilder.Append($" --obj-type {FAAC_ObjectType}");
			}
			if (FAAC_BlockType == 1 || FAAC_BlockType == 2)
			{
				stringBuilder.Append($" --shortctl {FAAC_BlockType}");
			}
			if (FAAC_RawBitstream)
			{
				stringBuilder.Append(" -r");
			}
			if (FAAC_CustomOptions != null && FAAC_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(FAAC_CustomOptions.Trim());
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
			stringBuilder.Append(" -o \"" + base.OutputFile + "\"");
		}
		else
		{
			stringBuilder.Append(" -o -");
		}
		return stringBuilder.ToString();
	}
}
