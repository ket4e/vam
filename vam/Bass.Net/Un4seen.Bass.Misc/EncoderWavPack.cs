using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderWavPack : BaseEncoder
{
	public bool WV_UseCustomOptionsOnly;

	public string WV_CustomOptions = string.Empty;

	public bool WV_UseHybrid;

	public int WV_HybridBitrate = 1024;

	public bool WV_CreateCorrectionFile;

	public bool WV_MaximumHybridCompression;

	public bool WV_FastMode;

	public bool WV_HighQuality;

	public bool WV_IgnoreLength;

	public bool WV_JointStereo = true;

	public bool WV_LowPriority;

	public bool WV_ComputeMD5;

	public bool WV_CalcAvgPeakQuant;

	public bool WV_PracticalFloat;

	public bool WV_NewRiffHeader;

	public float WV_NoiseShaping;

	public bool WV_CopyTimestamp;

	public int WV_ExtraProcessing;

	public override bool EncoderExists
	{
		get
		{
			if (File.Exists(Path.Combine(base.EncoderDirectory, "wavpack.exe")))
			{
				return true;
			}
			return false;
		}
	}

	public override BASSChannelType EncoderType => BASSChannelType.BASS_CTYPE_STREAM_WV;

	public override string DefaultOutputExtension => ".wv";

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate
	{
		get
		{
			if (WV_UseHybrid)
			{
				return WV_HybridBitrate;
			}
			return base.ChannelSampleRate * base.ChannelBitwidth * base.ChannelNumChans / 1000;
		}
	}

	public EncoderWavPack(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "WavPack Encoder (.wv)";
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
		base.EncoderHandle = BassEnc.BASS_Encode_Start(base.ChannelHandle, EncoderCommandLine, bASSEncode, null, IntPtr.Zero);
		if (base.EncoderHandle == 0)
		{
			return false;
		}
		return true;
	}

	public override string SettingsString()
	{
		return string.Format("{0}-{1} kbps, {2} {3}", WV_UseHybrid ? "Hybrid" : "Lossless", EffectiveBitrate, WV_JointStereo ? "JointStereo" : "Stereo", WV_HighQuality ? "High" : (WV_FastMode ? "Fast" : "Norm")).Trim();
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "wavpack.exe") + "\" -y");
		}
		else
		{
			stringBuilder.Append("wavpack.exe -y");
		}
		if (base.InputFile == null)
		{
			stringBuilder.Append(" -i");
		}
		if (WV_UseCustomOptionsOnly)
		{
			if (WV_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(WV_CustomOptions.Trim());
			}
		}
		else
		{
			if (WV_UseHybrid)
			{
				stringBuilder.Append(string.Format(provider, " -b{0}", WV_HybridBitrate));
				if (WV_CreateCorrectionFile)
				{
					stringBuilder.Append(" -c");
				}
				if (WV_MaximumHybridCompression)
				{
					stringBuilder.Append(" -cc");
				}
				if (WV_CalcAvgPeakQuant)
				{
					stringBuilder.Append(" -n");
				}
				if (WV_NoiseShaping != 0f && WV_NoiseShaping >= -1f && WV_NoiseShaping <= 1f)
				{
					stringBuilder.Append(string.Format(provider, " -s{0:0.0####}", WV_NoiseShaping));
				}
			}
			if (WV_FastMode)
			{
				stringBuilder.Append(" -f");
			}
			if (WV_HighQuality)
			{
				stringBuilder.Append(" -h");
			}
			if (WV_IgnoreLength && base.OutputFile != null)
			{
				stringBuilder.Append(" -i");
			}
			if (!WV_JointStereo)
			{
				stringBuilder.Append(" -j0");
			}
			if (WV_LowPriority)
			{
				stringBuilder.Append(" -l");
			}
			if (WV_ComputeMD5)
			{
				stringBuilder.Append(" -m");
			}
			if (WV_PracticalFloat)
			{
				stringBuilder.Append(" -p");
			}
			if (WV_NewRiffHeader)
			{
				stringBuilder.Append(" -r");
			}
			if (WV_CopyTimestamp && base.OutputFile != null)
			{
				stringBuilder.Append(" -t");
			}
			if (WV_ExtraProcessing > 0 && WV_ExtraProcessing <= 6)
			{
				stringBuilder.Append(string.Format(provider, " -x{0}", WV_ExtraProcessing));
			}
			if (base.TAGs != null)
			{
				if (!string.IsNullOrEmpty(base.TAGs.title))
				{
					stringBuilder.Append(" -w \"Title=" + base.TAGs.title.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.artist))
				{
					stringBuilder.Append(" -w \"Artist=" + base.TAGs.artist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.album))
				{
					stringBuilder.Append(" -w \"Album=" + base.TAGs.album.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.albumartist))
				{
					stringBuilder.Append(" -w \"Album Artist=" + base.TAGs.albumartist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.year))
				{
					stringBuilder.Append(" -w \"Year=" + base.TAGs.year.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.track))
				{
					stringBuilder.Append(" -w \"Track=" + base.TAGs.track.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.disc))
				{
					stringBuilder.Append(" -w \"Disc=" + base.TAGs.disc.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.genre))
				{
					stringBuilder.Append(" -w \"Genre=" + base.TAGs.genre.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.comment))
				{
					stringBuilder.Append(" -w \"Comment=" + base.TAGs.comment.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.composer))
				{
					stringBuilder.Append(" -w \"Composer=" + base.TAGs.composer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.conductor))
				{
					stringBuilder.Append(" -w \"Conductor=" + base.TAGs.conductor.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.lyricist))
				{
					stringBuilder.Append(" -w \"Lyricist=" + base.TAGs.lyricist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.remixer))
				{
					stringBuilder.Append(" -w \"MixArtist=" + base.TAGs.remixer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.producer))
				{
					stringBuilder.Append(" -w \"Producer=" + base.TAGs.producer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.encodedby))
				{
					stringBuilder.Append(" -w \"EncodedBy=" + base.TAGs.encodedby.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.copyright))
				{
					stringBuilder.Append(" -w \"Copyright=" + base.TAGs.copyright.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.publisher))
				{
					stringBuilder.Append(" -w \"Label=" + base.TAGs.publisher.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.bpm))
				{
					stringBuilder.Append(" -w \"BPM=" + base.TAGs.bpm.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.grouping))
				{
					stringBuilder.Append(" -w \"Grouping=" + base.TAGs.grouping.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.mood))
				{
					stringBuilder.Append(" -w \"Mood=" + base.TAGs.mood.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.rating))
				{
					stringBuilder.Append(" -w \"Rating=" + base.TAGs.rating.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.isrc))
				{
					stringBuilder.Append(" -w \"ISRC=" + base.TAGs.isrc.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (base.TAGs.replaygain_track_peak >= 0f)
				{
					stringBuilder.Append(" -w \"replaygain_track_peak=" + base.TAGs.replaygain_track_peak.ToString("R", CultureInfo.InvariantCulture) + "\"");
				}
				if (base.TAGs.replaygain_track_gain >= -60f && base.TAGs.replaygain_track_gain <= 60f)
				{
					stringBuilder.Append(" -w \"replaygain_track_gain=" + base.TAGs.replaygain_track_gain.ToString("R", CultureInfo.InvariantCulture) + " dB\"");
				}
			}
			if (WV_CustomOptions != null && WV_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(WV_CustomOptions.Trim());
			}
		}
		if (base.OutputFile == null)
		{
			stringBuilder.Append(" -q");
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
