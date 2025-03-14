using System;

namespace Un4seen.Bass.Misc;

[Serializable]
public class EncoderMP3 : EncoderCMDLN
{
	public EncoderMP3(int channel)
		: base(channel)
	{
		CMDLN_UseNOHEAD = true;
		CMDLN_UseFP_32BIT = false;
		CMDLN_UseFP_24BIT = true;
		CMDLN_EncoderType = BASSChannelType.BASS_CTYPE_STREAM_MP3;
		CMDLN_DefaultOutputExtension = ".mp3";
		CMDLN_SupportsSTDOUT = true;
		CMDLN_Option = "1";
		CMDLN_Quality = "2";
		CMDLN_UseA = true;
		CMDLN_UserA = "";
		CMDLN_UserB = "-mono";
		CMDLN_Executable = "mp3sEncoder.exe";
		CMDLN_CBRString = "${user} -raw -sr ${Hz} -c ${chan} -res ${res} -q ${option} -br ${bps} -if ${input} -of ${output}";
		CMDLN_VBRString = "${user} -raw -sr ${Hz} -c ${chan} -res ${res} -q ${option} -m ${quality} -if ${input} -of ${output}";
	}

	public override string SettingsString()
	{
		return string.Format("{0}-{1} kbps, {2} {3} {4}", CMDLN_UseVBR ? "VBR" : "CBR", EffectiveBitrate, CMDLN_Option, CMDLN_Quality, CMDLN_Mode).Trim();
	}

	public override string ToString()
	{
		return "Generic MP3 Encoder (" + DefaultOutputExtension + ")";
	}
}
