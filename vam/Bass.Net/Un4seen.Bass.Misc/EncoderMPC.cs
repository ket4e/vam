using System;
using System.Globalization;
using System.IO;
using System.Text;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderMPC : BaseEncoder
{
	public enum MPCPreset
	{
		telephone,
		thumb,
		radio,
		standard,
		xtreme,
		insane,
		braindead
	}

	public bool MPC_UseVersion7;

	public bool MPC_UseCustomOptionsOnly;

	public string MPC_CustomOptions = string.Empty;

	public MPCPreset MPC_Preset = MPCPreset.standard;

	public float MPC_Scale = 1f;

	public int MPC_Quality = 5;

	public float MPC_NMT = 6.5f;

	public float MPC_TMN = 18f;

	public float MPC_PNS;

	public override bool EncoderExists
	{
		get
		{
			if (MPC_UseVersion7 ? File.Exists(Path.Combine(base.EncoderDirectory, "mppenc.exe")) : File.Exists(Path.Combine(base.EncoderDirectory, "mpcenc.exe")))
			{
				return true;
			}
			return false;
		}
	}

	public override BASSChannelType EncoderType => BASSChannelType.BASS_CTYPE_STREAM_MPC;

	public override string DefaultOutputExtension => ".mpc";

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate
	{
		get
		{
			int result = 180;
			if (MPC_Quality != 5)
			{
				result = MPC_Quality switch
				{
					0 => 20, 
					1 => 30, 
					2 => 60, 
					3 => 90, 
					4 => 130, 
					5 => 180, 
					6 => 210, 
					7 => 240, 
					8 => 270, 
					9 => 300, 
					10 => 350, 
					_ => 180, 
				};
			}
			else if (MPC_Preset != MPCPreset.standard)
			{
				result = MPC_Preset switch
				{
					MPCPreset.xtreme => 210, 
					MPCPreset.radio => 130, 
					MPCPreset.insane => 240, 
					MPCPreset.thumb => 90, 
					MPCPreset.braindead => 270, 
					MPCPreset.telephone => 60, 
					_ => 180, 
				};
			}
			return result;
		}
	}

	public EncoderMPC(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "MusePack Encoder (.mpc)";
	}

	public override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		BASSEncode bASSEncode = BASSEncode.BASS_ENCODE_DEFAULT;
		bASSEncode = ((!base.Force16Bit) ? BASSEncode.BASS_ENCODE_FP_32BIT : (bASSEncode | BASSEncode.BASS_ENCODE_FP_16BIT));
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
		return $"{EffectiveBitrate} kbps, Quality {MPC_Quality}";
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (MPC_UseVersion7)
		{
			if (!string.IsNullOrEmpty(base.EncoderDirectory))
			{
				stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "mppenc.exe") + "\" --overwrite");
			}
			else
			{
				stringBuilder.Append("mppenc.exe --overwrite");
			}
		}
		else if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "mpcenc.exe") + "\" --overwrite");
		}
		else
		{
			stringBuilder.Append("mpcenc.exe --overwrite");
		}
		if (MPC_UseCustomOptionsOnly)
		{
			if (MPC_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(MPC_CustomOptions.Trim());
			}
		}
		else
		{
			if (MPC_Preset != MPCPreset.standard)
			{
				stringBuilder.Append(" --" + MPC_Preset);
			}
			if (MPC_Scale != 1f)
			{
				stringBuilder.Append(string.Format(provider, " --scale {0:0.0####}", MPC_Scale));
			}
			if (MPC_Quality != 5)
			{
				stringBuilder.Append(string.Format(provider, " --quality {0}", MPC_Quality));
			}
			if (MPC_NMT != 6.5f)
			{
				stringBuilder.Append(string.Format(provider, " --nmt {0:0.0####}", MPC_NMT));
			}
			if (MPC_TMN != 18f)
			{
				stringBuilder.Append(string.Format(provider, " --tmn {0:0.0####}", MPC_TMN));
			}
			if (MPC_PNS != 0f)
			{
				stringBuilder.Append(string.Format(provider, " --pns {0:0.0####}", MPC_PNS));
			}
			if (base.TAGs != null)
			{
				if (!string.IsNullOrEmpty(base.TAGs.title))
				{
					stringBuilder.Append(" --tag \"Title=" + base.TAGs.title.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.artist))
				{
					stringBuilder.Append(" --tag \"Artist=" + base.TAGs.artist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.album))
				{
					stringBuilder.Append(" --tag \"Album=" + base.TAGs.album.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.albumartist))
				{
					stringBuilder.Append(" --tag \"Album Artist=" + base.TAGs.albumartist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.year))
				{
					stringBuilder.Append(" --tag \"Year=" + base.TAGs.year.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.track))
				{
					stringBuilder.Append(" --tag \"Track=" + base.TAGs.track.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.disc))
				{
					stringBuilder.Append(" --tag \"Disc=" + base.TAGs.disc.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.genre))
				{
					stringBuilder.Append(" --tag \"Genre=" + base.TAGs.genre.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.comment))
				{
					stringBuilder.Append(" --tag \"Comment=" + base.TAGs.comment.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.composer))
				{
					stringBuilder.Append(" --tag \"Composer=" + base.TAGs.composer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.conductor))
				{
					stringBuilder.Append(" --tag \"Conductor=" + base.TAGs.conductor.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.lyricist))
				{
					stringBuilder.Append(" --tag \"Lyricist=" + base.TAGs.lyricist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.remixer))
				{
					stringBuilder.Append(" --tag \"MixArtist=" + base.TAGs.remixer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.producer))
				{
					stringBuilder.Append(" --tag \"Producer=" + base.TAGs.producer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.encodedby))
				{
					stringBuilder.Append(" --tag \"EncodedBy=" + base.TAGs.encodedby.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.copyright))
				{
					stringBuilder.Append(" --tag \"Copyright=" + base.TAGs.copyright.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.publisher))
				{
					stringBuilder.Append(" --tag \"Label=" + base.TAGs.publisher.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.bpm))
				{
					stringBuilder.Append(" --tag \"BPM=" + base.TAGs.bpm.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.grouping))
				{
					stringBuilder.Append(" --tag \"Grouping=" + base.TAGs.grouping.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.mood))
				{
					stringBuilder.Append(" --tag \"Mood=" + base.TAGs.mood.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.rating))
				{
					stringBuilder.Append(" --tag \"Rating=" + base.TAGs.rating.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.isrc))
				{
					stringBuilder.Append(" --tag \"ISRC=" + base.TAGs.isrc.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (base.TAGs.replaygain_track_peak >= 0f)
				{
					stringBuilder.Append(" --tag \"replaygain_track_peak=" + base.TAGs.replaygain_track_peak.ToString("R", CultureInfo.InvariantCulture) + "\"");
				}
				if (base.TAGs.replaygain_track_gain >= -60f && base.TAGs.replaygain_track_gain <= 60f)
				{
					stringBuilder.Append(" --tag \"replaygain_track_gain=" + base.TAGs.replaygain_track_gain.ToString("R", CultureInfo.InvariantCulture) + " dB\"");
				}
			}
			if (MPC_CustomOptions != null && MPC_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(MPC_CustomOptions.Trim());
			}
		}
		if (base.OutputFile == null)
		{
			stringBuilder.Append(" --silent");
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
