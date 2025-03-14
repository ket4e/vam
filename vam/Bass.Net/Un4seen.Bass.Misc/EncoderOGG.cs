using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderOGG : BaseEncoder
{
	public enum OGGConverter
	{
		Best,
		Medium,
		Fast
	}

	public bool OGG_UseCustomOptionsOnly;

	public string OGG_CustomOptions = string.Empty;

	public bool OGG_Downmix;

	public float OGG_Scale = 1f;

	public bool OGG_UseQualityMode = true;

	public float OGG_Quality = 4f;

	public int OGG_Bitrate = 128;

	public bool OGG_UseManagedBitrate;

	public int OGG_MinBitrate;

	public int OGG_MaxBitrate;

	public int OGG_TargetSampleRate;

	public OGGConverter OGG_Converter = OGGConverter.Medium;

	public override bool EncoderExists
	{
		get
		{
			if (File.Exists(Path.Combine(base.EncoderDirectory, "oggenc2.exe")))
			{
				return true;
			}
			return false;
		}
	}

	public override BASSChannelType EncoderType => BASSChannelType.BASS_CTYPE_STREAM_OGG;

	public override string DefaultOutputExtension => ".ogg";

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate
	{
		get
		{
			if (OGG_UseQualityMode)
			{
				return Quality2Kbps(OGG_Quality);
			}
			return OGG_Bitrate;
		}
	}

	public EncoderOGG(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "OGG Encoder (.ogg)";
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
		return string.Format("{0}-{1} kbps, {2} {3}", (OGG_Bitrate == OGG_MinBitrate && OGG_Bitrate == OGG_MaxBitrate) ? "CBR" : "VBR", EffectiveBitrate, OGG_Downmix ? "Mono" : "Stereo", OGG_Converter);
	}

	public int Quality2Kbps(float q)
	{
		if (q < 4f)
		{
			return (int)Math.Round(64f + q * 16f);
		}
		if (q < 8f)
		{
			return (int)Math.Round(128f + (q - 4f) * 32f);
		}
		if (q < 9f)
		{
			return (int)Math.Round(256f + (q - 8f) * 64f);
		}
		if (q <= 10f)
		{
			return (int)Math.Round(320f + (q - 9f) * 178f);
		}
		return 128;
	}

	public float Kbps2Quality(int kbps)
	{
		if (kbps <= 128)
		{
			return (float)Math.Round((double)(kbps - 64) / 16.0, 2);
		}
		if (kbps <= 256)
		{
			return (float)Math.Round((double)(kbps - 128) / 32.0, 2) + 4f;
		}
		if (kbps <= 320)
		{
			return (float)Math.Round((double)(kbps - 256) / 64.0, 2) + 8f;
		}
		if (kbps <= 498)
		{
			return (float)Math.Round((double)(kbps - 320) / 178.0, 2) + 9f;
		}
		return 4f;
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "oggenc2.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("oggenc2.exe");
		}
		if (base.ChannelBitwidth > 24)
		{
			stringBuilder.Append(string.Format(provider, " -r -F 3 -C {0} -R {1}", base.ChannelNumChans, base.ChannelSampleRate));
		}
		else
		{
			stringBuilder.Append(string.Format(provider, " -r -F 1 -B {0} -C {1} -R {2}", (base.ChannelBitwidth <= 16) ? base.ChannelBitwidth : (base.Force16Bit ? 16 : base.ChannelBitwidth), base.ChannelNumChans, base.ChannelSampleRate));
		}
		if (OGG_UseCustomOptionsOnly)
		{
			if (base.OutputFile == null)
			{
				stringBuilder.Append(" -Q");
			}
			if (OGG_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(OGG_CustomOptions.Trim());
			}
		}
		else
		{
			if (base.OutputFile == null)
			{
				stringBuilder.Append(" -Q");
			}
			if (OGG_UseQualityMode)
			{
				stringBuilder.Append(string.Format(provider, " -q {0:0.0####}", OGG_Quality));
			}
			else
			{
				stringBuilder.Append(string.Format(provider, " -b {0}", OGG_Bitrate));
				if (OGG_UseManagedBitrate)
				{
					stringBuilder.Append(" --managed");
				}
				if (OGG_MinBitrate > 0)
				{
					stringBuilder.Append(string.Format(provider, " -m {0}", OGG_MinBitrate));
				}
				if (OGG_MaxBitrate > 0)
				{
					stringBuilder.Append(string.Format(provider, " -M {0}", OGG_MaxBitrate));
				}
			}
			if (OGG_TargetSampleRate > 0)
			{
				stringBuilder.Append(string.Format(provider, " --resample {0} -S {1}", OGG_TargetSampleRate, (int)OGG_Converter));
			}
			if (base.ChannelNumChans == 2 && OGG_Downmix)
			{
				stringBuilder.Append(" --downmix");
			}
			if (OGG_Scale < 1f && OGG_Scale >= 0f)
			{
				stringBuilder.Append(string.Format(provider, " --scale {0:0.0####}", OGG_Scale));
			}
			if (base.TAGs != null)
			{
				stringBuilder.Append(" --utf8");
				if (!string.IsNullOrEmpty(base.TAGs.title))
				{
					stringBuilder.Append(" -t \"" + base.TAGs.title.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.artist))
				{
					stringBuilder.Append(" -a \"" + base.TAGs.artist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.album))
				{
					stringBuilder.Append(" -l \"" + base.TAGs.album.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.year))
				{
					stringBuilder.Append(" -d \"" + base.TAGs.year.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.track))
				{
					stringBuilder.Append(" -N \"" + base.TAGs.track.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.genre))
				{
					stringBuilder.Append(" -G \"" + base.TAGs.genre.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.disc))
				{
					stringBuilder.Append(" -c \"DISCNUMBER=" + base.TAGs.disc.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.albumartist))
				{
					stringBuilder.Append(" -c \"ALBUMARTIST=" + base.TAGs.albumartist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.comment))
				{
					stringBuilder.Append(" -c \"COMMENT=" + base.TAGs.comment.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.composer))
				{
					stringBuilder.Append(" -c \"COMPOSER=" + base.TAGs.composer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.conductor))
				{
					stringBuilder.Append(" -c \"CONDUCTOR=" + base.TAGs.conductor.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.lyricist))
				{
					stringBuilder.Append(" -c \"LYRICIST=" + base.TAGs.lyricist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.remixer))
				{
					stringBuilder.Append(" -c \"REMIXER=" + base.TAGs.remixer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.producer))
				{
					stringBuilder.Append(" -c \"PRODUCER=" + base.TAGs.producer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.encodedby))
				{
					stringBuilder.Append(" -c \"ENCODEDBY=" + base.TAGs.encodedby.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.copyright))
				{
					stringBuilder.Append(" -c \"COPYRIGHT=" + base.TAGs.copyright.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.publisher))
				{
					stringBuilder.Append(" -c \"LABEL=" + base.TAGs.publisher.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.bpm))
				{
					stringBuilder.Append(" -c \"BPM=" + base.TAGs.bpm.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.grouping))
				{
					stringBuilder.Append(" -c \"GROUPING=" + base.TAGs.grouping.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.mood))
				{
					stringBuilder.Append(" -c \"MOOD=" + base.TAGs.mood.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.rating))
				{
					stringBuilder.Append(" -c \"RATING=" + base.TAGs.rating.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.isrc))
				{
					stringBuilder.Append(" -c \"ISRC=" + base.TAGs.isrc.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (base.TAGs.replaygain_track_peak >= 0f)
				{
					stringBuilder.Append(" -c \"replaygain_track_peak=" + base.TAGs.replaygain_track_peak.ToString("R", CultureInfo.InvariantCulture) + "\"");
				}
				if (base.TAGs.replaygain_track_gain >= -60f && base.TAGs.replaygain_track_gain <= 60f)
				{
					stringBuilder.Append(" -c \"replaygain_track_gain=" + base.TAGs.replaygain_track_gain.ToString("R", CultureInfo.InvariantCulture) + " dB\"");
				}
			}
			if (OGG_CustomOptions != null && OGG_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(OGG_CustomOptions.Trim());
			}
		}
		if (base.OutputFile != null)
		{
			stringBuilder.Append(" -o \"" + base.OutputFile + "\"");
		}
		else
		{
			stringBuilder.Append(" -o -");
		}
		if (base.InputFile != null)
		{
			stringBuilder.Append(" \"" + base.InputFile + "\"");
		}
		else
		{
			stringBuilder.Append(" -");
		}
		if (base.TAGs != null)
		{
			Encoding unicode = Encoding.Unicode;
			Encoding uTF = Encoding.UTF8;
			byte[] bytes = unicode.GetBytes(stringBuilder.ToString());
			byte[] array = Encoding.Convert(unicode, uTF, bytes);
			char[] array2 = new char[uTF.GetCharCount(array, 0, array.Length)];
			uTF.GetChars(array, 0, array.Length, array2, 0);
			return new string(array2);
		}
		return stringBuilder.ToString();
	}
}
