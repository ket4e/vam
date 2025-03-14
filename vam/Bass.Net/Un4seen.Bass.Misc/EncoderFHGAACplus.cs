using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Win32;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderFHGAACplus : BaseEncoder
{
	public enum FHGProfile
	{
		Auto,
		LC,
		HE,
		HEv2
	}

	public int AACPlus_ConstantBitrate = 128;

	public int AACPlus_VariableBitrate;

	public FHGProfile AACPlus_Profile;

	public bool AACPlus_UseADTS;

	public override bool EncoderExists
	{
		get
		{
			if (AACPlus_UseADTS)
			{
				if (File.Exists(Path.Combine(base.EncoderDirectory, "fhgaacenc.exe")) && File.Exists(Path.Combine(base.EncoderDirectory, "libsndfile-1.dll")) && File.Exists(Path.Combine(base.EncoderDirectory, "nsutil.dll")) && File.Exists(Path.Combine(base.EncoderDirectory, "enc_fhgaac.dll")))
				{
					return true;
				}
				return false;
			}
			if (File.Exists(Path.Combine(base.EncoderDirectory, "fhgaacenc.exe")) && File.Exists(Path.Combine(base.EncoderDirectory, "enc_fhgaac.dll")) && File.Exists(Path.Combine(base.EncoderDirectory, "libsndfile-1.dll")) && File.Exists(Path.Combine(base.EncoderDirectory, "nsutil.dll")) && File.Exists(Path.Combine(base.EncoderDirectory, "libmp4v2.dll")))
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
			if (AACPlus_UseADTS)
			{
				return ".aac";
			}
			return ".m4a";
		}
	}

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => BuildEncoderCommandLine();

	public override int EffectiveBitrate => AACPlus_ConstantBitrate;

	public EncoderFHGAACplus(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "FHG Winamp AACplus v2 Encoder (" + DefaultOutputExtension + ")";
	}

	public override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		BASSEncode bASSEncode = BASSEncode.BASS_ENCODE_DEFAULT;
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
		if (AACPlus_VariableBitrate > 0)
		{
			return string.Format("VBR-{0}, {1} {2}", AACPlus_VariableBitrate, AACPlus_Profile, AACPlus_UseADTS ? "ADTS" : "MPEG-4");
		}
		return string.Format("CBR-{0} kbps, {1} {2}", AACPlus_ConstantBitrate, AACPlus_Profile, AACPlus_UseADTS ? "ADTS" : "MPEG-4");
	}

	private string BuildEncoderCommandLine()
	{
		CultureInfo provider = new CultureInfo("en-US", useUserOverride: false);
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(base.EncoderDirectory))
		{
			stringBuilder.Append("\"" + Path.Combine(base.EncoderDirectory, "fhgaacenc.exe") + "\"");
		}
		else
		{
			stringBuilder.Append("fhgaacenc.exe");
		}
		if (AACPlus_VariableBitrate > 0)
		{
			stringBuilder.Append(string.Format(provider, " --vbr {0}", AACPlus_VariableBitrate));
		}
		else
		{
			stringBuilder.Append(string.Format(provider, " --cbr {0}", AACPlus_ConstantBitrate));
			if (AACPlus_Profile != 0)
			{
				stringBuilder.Append(string.Format(provider, " --profile {0}", AACPlus_Profile));
			}
		}
		if (AACPlus_UseADTS || base.OutputFile == null)
		{
			stringBuilder.Append(" --adts");
		}
		if (base.InputFile == null)
		{
			stringBuilder.Append(" --ignorelength");
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

	public static bool IsWinampInstalled()
	{
		try
		{
			using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Winamp");
			if (registryKey != null)
			{
				return true;
			}
		}
		catch
		{
		}
		try
		{
			using RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Nullsoft\\Winamp");
			if (registryKey2 != null)
			{
				return true;
			}
		}
		catch
		{
		}
		try
		{
			using RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Nullsoft\\Winamp");
			if (registryKey3 != null)
			{
				return true;
			}
		}
		catch
		{
		}
		try
		{
			using RegistryKey registryKey4 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
			string[] subKeyNames = registryKey4.GetSubKeyNames();
			foreach (string name in subKeyNames)
			{
				try
				{
					using RegistryKey registryKey5 = registryKey4.OpenSubKey(name);
					if (registryKey5 != null && registryKey5.GetValue("DisplayName") as string == "Winamp")
					{
						return true;
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
		return false;
	}
}
