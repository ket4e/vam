using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Win32;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderWinampAACplus : BaseEncoder
{
	public int AACPlus_Bitrate = 128;

	public bool AACPlus_EnableParametricStereo;

	public bool AACPlus_IndependedStereo;

	public bool AACPlus_PreferDualChannel;

	public bool AACPlus_Mono;

	public bool AACPlus_Mpeg2Aac;

	public bool AACPlus_Mpeg4Aac;

	public bool AACPlus_LC;

	public bool AACPlus_HE;

	public bool AACPlus_High;

	public bool AACPlus_Speech;

	public bool AACPlus_PNS;

	public bool AACPlus_UseMp4Output;

	public override bool EncoderExists
	{
		get
		{
			if (AACPlus_UseMp4Output)
			{
				if (File.Exists(Path.Combine(base.EncoderDirectory, "enc_aacPlus.exe")) && File.Exists(Path.Combine(base.EncoderDirectory, "enc_aacPlus.dll")) && File.Exists(Path.Combine(base.EncoderDirectory, "libmp4v2.dll")))
				{
					return true;
				}
				return false;
			}
			if (File.Exists(Path.Combine(base.EncoderDirectory, "enc_aacPlus.exe")) && File.Exists(Path.Combine(base.EncoderDirectory, "enc_aacPlus.dll")))
			{
				return true;
			}
			return false;
		}
	}

	public override BASSChannelType EncoderType => BASSChannelType.BASS_CTYPE_STREAM_AAC;

	public override string DefaultOutputExtension
	{
		get
		{
			if (AACPlus_UseMp4Output)
			{
				return ".m4a";
			}
			return ".aac";
		}
	}

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate => AACPlus_Bitrate;

	public new bool Force16Bit => true;

	public EncoderWinampAACplus(int channel)
		: base(channel)
	{
		if (base.ChannelBitwidth < 16)
		{
			throw new ArgumentException("8-bit channels are not supported by the Winamp AACPlus encoder!");
		}
	}

	public override string ToString()
	{
		return "Winamp AACplus v2 Encoder (" + DefaultOutputExtension + ")";
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
		return string.Format("CBR-{0} kbps, {1} {2}", EffectiveBitrate, AACPlus_High ? "HE-AAC+" : (AACPlus_LC ? "LC-AAC" : "HE-AAC"), AACPlus_EnableParametricStereo ? "Parametric" : (AACPlus_IndependedStereo ? "Independent" : (AACPlus_Mono ? "Mono" : "Stereo")));
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "enc_aacPlus.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("enc_aacPlus.exe");
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
		stringBuilder.Append(string.Format(provider, " --br {0}", AACPlus_Bitrate * 1000));
		if (AACPlus_EnableParametricStereo)
		{
			stringBuilder.Append(" --ps");
		}
		if (AACPlus_IndependedStereo)
		{
			stringBuilder.Append(" --is");
		}
		if (AACPlus_PreferDualChannel)
		{
			stringBuilder.Append(" --dc");
		}
		if (AACPlus_Mono)
		{
			stringBuilder.Append(" --mono");
		}
		if (AACPlus_Speech)
		{
			stringBuilder.Append(" --speech");
		}
		if (AACPlus_PNS)
		{
			stringBuilder.Append(" --pns");
		}
		if (AACPlus_Mpeg2Aac)
		{
			stringBuilder.Append(" --mpeg2aac");
		}
		if (AACPlus_Mpeg4Aac)
		{
			stringBuilder.Append(" --mpeg4aac");
		}
		if (AACPlus_LC)
		{
			stringBuilder.Append(" --lc");
		}
		if (AACPlus_HE && AACPlus_Bitrate <= 128)
		{
			stringBuilder.Append(" --he");
		}
		if (AACPlus_High && AACPlus_Bitrate <= 256 && base.ChannelNumChans <= 2)
		{
			stringBuilder.Append(" --high");
		}
		if (base.OutputFile == null)
		{
			stringBuilder.Append(" --silent");
		}
		if (base.InputFile == null)
		{
			stringBuilder.Append(string.Format(provider, " --rawpcm {0} {1} {2}", base.ChannelSampleRate, base.ChannelNumChans, 16));
		}
		return stringBuilder.ToString();
	}

	public static bool IsWinampInstalled()
	{
		bool flag = false;
		try
		{
			using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Nullsoft\\Winamp");
			if (registryKey != null)
			{
				flag = true;
			}
		}
		catch
		{
		}
		if (!flag)
		{
			try
			{
				using RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
				string[] subKeyNames = registryKey2.GetSubKeyNames();
				foreach (string name in subKeyNames)
				{
					try
					{
						using RegistryKey registryKey3 = registryKey2.OpenSubKey(name);
						if (registryKey3 != null && registryKey3.GetValue("DisplayName") as string == "Winamp")
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
