using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Win32;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderQTAAC : BaseEncoder
{
	public enum QTMode
	{
		CBR,
		ABR,
		CVBR,
		TVBR
	}

	public enum QTQuality
	{
		Fastest,
		Fast,
		Normal,
		High,
		Highest
	}

	public bool QT_UseCustomOptionsOnly;

	public string QT_CustomOptions = string.Empty;

	public QTQuality QT_QualityMode = QTQuality.High;

	public QTMode QT_Mode = QTMode.TVBR;

	public int QT_Bitrate = 128;

	public int QT_Quality = 65;

	public bool QT_HE;

	public int QT_Samplerate;

	public override bool EncoderExists
	{
		get
		{
			if (File.Exists(Path.Combine(base.EncoderDirectory, "qtaacenc.exe")))
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

	public override int EffectiveBitrate => QT_Bitrate;

	public EncoderQTAAC(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "QuickTime AAC Encoder (.m4a)";
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
		return string.Format("{0}-{1} kbps, {2}", QT_Mode, EffectiveBitrate, QT_HE ? "HE" : "LC");
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "qtaacenc.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("qtaacenc.exe");
		}
		stringBuilder.Append(" --quiet");
		if (QT_UseCustomOptionsOnly)
		{
			if (QT_CustomOptions != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(QT_CustomOptions.Trim());
			}
		}
		else
		{
			switch (QT_Mode)
			{
			case QTMode.CBR:
				stringBuilder.Append(string.Format(provider, " --cbr {0}", QT_Bitrate));
				break;
			case QTMode.ABR:
				stringBuilder.Append(string.Format(provider, " --abr {0}", QT_Bitrate));
				break;
			case QTMode.CVBR:
				stringBuilder.Append(string.Format(provider, " --cvbr {0}", QT_Bitrate));
				break;
			case QTMode.TVBR:
				stringBuilder.Append(string.Format(provider, " --tvbr {0}", QT_Quality));
				break;
			}
			if (QT_Samplerate < 0)
			{
				stringBuilder.Append(" --samplerate auto");
			}
			else if (QT_Samplerate == 0)
			{
				stringBuilder.Append(" --samplerate keep");
			}
			else
			{
				switch (QT_Samplerate)
				{
				case 8000:
					stringBuilder.Append(" --samplerate 8000");
					break;
				case 11250:
					stringBuilder.Append(" --samplerate 11250");
					break;
				case 12000:
					stringBuilder.Append(" --samplerate 12000");
					break;
				case 16000:
					stringBuilder.Append(" --samplerate 16000");
					break;
				case 22050:
					stringBuilder.Append(" --samplerate 22050");
					break;
				case 24000:
					stringBuilder.Append(" --samplerate 24000");
					break;
				case 32000:
					stringBuilder.Append(" --samplerate 32000");
					break;
				case 44100:
					stringBuilder.Append(" --samplerate 44100");
					break;
				case 48000:
					stringBuilder.Append(" --samplerate 48000");
					break;
				default:
					stringBuilder.Append(" --samplerate auto");
					break;
				}
			}
			switch (QT_QualityMode)
			{
			case QTQuality.Fastest:
				stringBuilder.Append(" --fastest");
				break;
			case QTQuality.Fast:
				stringBuilder.Append(" --fast");
				break;
			case QTQuality.Normal:
				stringBuilder.Append(" --normal");
				break;
			case QTQuality.High:
				stringBuilder.Append(" --high");
				break;
			case QTQuality.Highest:
				stringBuilder.Append(" --highest");
				break;
			default:
				stringBuilder.Append(" --high");
				break;
			}
			if (QT_HE && QT_Mode != QTMode.TVBR)
			{
				stringBuilder.Append(" --he");
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
					stringBuilder.Append(" --album \"" + base.TAGs.album.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.albumartist))
				{
					stringBuilder.Append(" --albumartist \"" + base.TAGs.albumartist.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.genre))
				{
					stringBuilder.Append(" --genre \"" + base.TAGs.genre.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.year))
				{
					stringBuilder.Append(" --date \"" + base.TAGs.year.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.composer))
				{
					stringBuilder.Append(" --composer \"" + base.TAGs.composer.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.grouping))
				{
					stringBuilder.Append(" --grouping \"" + base.TAGs.grouping.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.comment))
				{
					stringBuilder.Append(" --comment \"" + base.TAGs.comment.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.track))
				{
					stringBuilder.Append(" --track \"" + base.TAGs.track.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
				if (!string.IsNullOrEmpty(base.TAGs.disc))
				{
					stringBuilder.Append(" --disc \"" + base.TAGs.disc.Replace("\"", "\\\"").Replace("\r", string.Empty).Replace("\n", string.Empty) + "\"");
				}
			}
			if (QT_CustomOptions != null && QT_CustomOptions.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(QT_CustomOptions.Trim());
			}
		}
		if (base.InputFile != null)
		{
			stringBuilder.Append(" \"" + base.InputFile + "\"");
		}
		else
		{
			stringBuilder.Append(" --ignorelength -");
		}
		if (base.OutputFile != null)
		{
			stringBuilder.Append(" \"" + base.OutputFile + "\"");
		}
		else
		{
			stringBuilder.Append(" \"" + Path.ChangeExtension(base.InputFile, DefaultOutputExtension) + "\"");
		}
		return stringBuilder.ToString();
	}

	public static bool IsQuickTimeInstalled()
	{
		bool flag = false;
		try
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Apple Computer, Inc.\\QuickTime"))
			{
				if (registryKey != null)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				using RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Apple Computer, Inc.\\QuickTime");
				if (registryKey2 != null)
				{
					flag = true;
				}
			}
		}
		catch
		{
		}
		if (!flag)
		{
			try
			{
				using RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
				string[] subKeyNames = registryKey3.GetSubKeyNames();
				foreach (string name in subKeyNames)
				{
					try
					{
						using RegistryKey registryKey4 = registryKey3.OpenSubKey(name);
						if (registryKey4 != null && registryKey4.GetValue("DisplayName") as string == "QuickTime")
						{
							flag = true;
							break;
						}
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}
		return flag;
	}
}
