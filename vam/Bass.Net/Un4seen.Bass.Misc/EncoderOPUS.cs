using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderOPUS : BaseEncoder
{
	public enum OPUSDownmix
	{
		None,
		Mono,
		Stereo
	}

	public enum OPUSFramesize
	{
		f2_5ms,
		f5ms,
		f10ms,
		f20ms,
		f40ms,
		f60ms
	}

	public enum OPUSMode
	{
		VBR,
		CVBR,
		CBR
	}

	public bool OPUS_UseCustomOptionsOnly;

	public string OPUS_CustomOptions = string.Empty;

	public int OPUS_Bitrate = 128;

	public OPUSMode OPUS_Mode;

	public int OPUS_Complexity = 10;

	public OPUSFramesize OPUS_Framesize = OPUSFramesize.f20ms;

	public int OPUS_ExpectLoss;

	public OPUSDownmix OPUS_Downmix;

	public int OPUS_MaxDelay = 1000;

	public bool OPUS_Uncoupled;

	public override bool EncoderExists
	{
		get
		{
			if (File.Exists(Path.Combine(base.EncoderDirectory, "opusenc.exe")))
			{
				return true;
			}
			return false;
		}
	}

	public override BASSChannelType EncoderType => BASSChannelType.BASS_CTYPE_STREAM_OPUS;

	public override string DefaultOutputExtension => ".opus";

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate => OPUS_Bitrate;

	public EncoderOPUS(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "OPUS Encoder (.opus)";
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
		return string.Format("{0}-{1} kbps{2}", OPUS_Mode, EffectiveBitrate, (OPUS_Downmix == OPUSDownmix.None) ? "" : (", " + OPUS_Downmix));
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "opusenc.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("opusenc.exe");
		}
		stringBuilder.Append(string.Format(provider, " --raw --raw-bits {0} --raw-rate {1} --raw-chan {2} --ignorelength", (base.ChannelBitwidth <= 16) ? base.ChannelBitwidth : (base.Force16Bit ? 16 : 24), base.ChannelSampleRate, base.ChannelNumChans));
		if (OPUS_UseCustomOptionsOnly)
		{
			if (base.OutputFile == null)
			{
				stringBuilder.Append(" --quiet");
			}
			if (OPUS_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(OPUS_CustomOptions.Trim());
			}
		}
		else
		{
			if (base.OutputFile == null)
			{
				stringBuilder.Append(" --quiet");
			}
			stringBuilder.Append(string.Format(provider, " --bitrate {0}", OPUS_Bitrate));
			if (OPUS_Mode == OPUSMode.CVBR)
			{
				stringBuilder.Append(" --cvbr");
			}
			else if (OPUS_Mode == OPUSMode.CBR)
			{
				stringBuilder.Append(" --hard-cbr");
			}
			if (OPUS_Complexity < 10)
			{
				stringBuilder.AppendFormat(provider, " --comp {0}", OPUS_Complexity);
			}
			switch (OPUS_Framesize)
			{
			case OPUSFramesize.f2_5ms:
				stringBuilder.Append(" --framesize 2.5");
				break;
			case OPUSFramesize.f5ms:
				stringBuilder.Append(" --framesize 5");
				break;
			case OPUSFramesize.f10ms:
				stringBuilder.Append(" --framesize 10");
				break;
			case OPUSFramesize.f40ms:
				stringBuilder.Append(" --framesize 40");
				break;
			case OPUSFramesize.f60ms:
				stringBuilder.Append(" --framesize 60");
				break;
			}
			if (OPUS_ExpectLoss != 0)
			{
				stringBuilder.AppendFormat(provider, " --expect-loss {0}", OPUS_ExpectLoss);
			}
			if (OPUS_Downmix != 0)
			{
				if (OPUS_Downmix == OPUSDownmix.Stereo && base.ChannelNumChans > 2)
				{
					stringBuilder.Append(" --downmix-stereo");
				}
				else if (OPUS_Downmix == OPUSDownmix.Mono)
				{
					stringBuilder.Append(" --downmix-mono");
				}
			}
			if (OPUS_MaxDelay < 1000)
			{
				stringBuilder.AppendFormat(provider, " --max-delay {0}", OPUS_MaxDelay);
			}
			if (OPUS_Uncoupled && base.ChannelNumChans > 1)
			{
				stringBuilder.Append(" --uncoupled");
			}
			if (base.TAGs != null)
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
					stringBuilder.Append(" --comment \"ALBUM=" + base.TAGs.album.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.year))
				{
					stringBuilder.Append(" --comment \"DATE=" + base.TAGs.year.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.track))
				{
					stringBuilder.Append(" --comment \"TRACKNUMBER=" + base.TAGs.track.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.genre))
				{
					stringBuilder.Append(" --comment \"GENRE=" + base.TAGs.genre.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.disc))
				{
					stringBuilder.Append(" --comment \"DISCNUMBER=" + base.TAGs.disc.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.albumartist))
				{
					stringBuilder.Append(" --comment \"ALBUMARTIST=" + base.TAGs.albumartist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.comment))
				{
					stringBuilder.Append(" --comment \"COMMENT=" + base.TAGs.comment.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.composer))
				{
					stringBuilder.Append(" --comment \"COMPOSER=" + base.TAGs.composer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.conductor))
				{
					stringBuilder.Append(" --comment \"CONDUCTOR=" + base.TAGs.conductor.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.lyricist))
				{
					stringBuilder.Append(" --comment \"LYRICIST=" + base.TAGs.lyricist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.remixer))
				{
					stringBuilder.Append(" --comment \"REMIXER=" + base.TAGs.remixer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.producer))
				{
					stringBuilder.Append(" --comment \"PRODUCER=" + base.TAGs.producer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.encodedby))
				{
					stringBuilder.Append(" --comment \"ENCODEDBY=" + base.TAGs.encodedby.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.copyright))
				{
					stringBuilder.Append(" --comment \"COPYRIGHT=" + base.TAGs.copyright.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.publisher))
				{
					stringBuilder.Append(" --comment \"LABEL=" + base.TAGs.publisher.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.bpm))
				{
					stringBuilder.Append(" --comment \"BPM=" + base.TAGs.bpm.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.grouping))
				{
					stringBuilder.Append(" --comment \"GROUPING=" + base.TAGs.grouping.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.mood))
				{
					stringBuilder.Append(" --comment \"MOOD=" + base.TAGs.mood.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.rating))
				{
					stringBuilder.Append(" --comment \"RATING=" + base.TAGs.rating.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.isrc))
				{
					stringBuilder.Append(" --comment \"ISRC=" + base.TAGs.isrc.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (base.TAGs.replaygain_track_peak >= 0f)
				{
					stringBuilder.Append(" --comment \"replaygain_track_peak=" + base.TAGs.replaygain_track_peak.ToString("R", CultureInfo.InvariantCulture) + "\"");
				}
				if (base.TAGs.replaygain_track_gain >= -60f && base.TAGs.replaygain_track_gain <= 60f)
				{
					stringBuilder.Append(" --comment \"replaygain_track_gain=" + base.TAGs.replaygain_track_gain.ToString("R", CultureInfo.InvariantCulture) + " dB\"");
				}
			}
			if (OPUS_CustomOptions != null && OPUS_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(OPUS_CustomOptions.Trim());
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
