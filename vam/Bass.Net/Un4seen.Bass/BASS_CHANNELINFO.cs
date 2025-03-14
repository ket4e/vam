using System;

namespace Un4seen.Bass;

[Serializable]
public sealed class BASS_CHANNELINFO
{
	internal BASS_CHANNELINFO_INTERNAL _internal;

	public int freq;

	public int chans;

	public BASSFlag flags;

	public BASSChannelType ctype;

	public int origres;

	public int plugin;

	public int sample;

	public string filename = string.Empty;

	public bool IsDecodingChannel => (flags & BASSFlag.BASS_STREAM_DECODE) != 0;

	public bool Is32bit => (flags & BASSFlag.BASS_SAMPLE_FLOAT) != 0;

	public bool Is8bit => (flags & BASSFlag.BASS_SAMPLE_8BITS) != 0;

	public override string ToString()
	{
		return $"{Utils.BASSChannelTypeToString(ctype)}, {freq}Hz, {Utils.ChannelNumberToString(chans)}, {((origres != 0) ? origres : (Is32bit ? 32 : (Is8bit ? 8 : 16)))}bit";
	}
}
