using System;
using System.Runtime.InteropServices;
using Un4seen.Bass;

namespace Un4seen.BassWasapi;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_WASAPI_INFO
{
	public BASSWASAPIInit initflags;

	public int freq;

	public int chans;

	public BASSWASAPIFormat format;

	public int buflen;

	private float volmax;

	private float volmin;

	private float volstep;

	public bool IsExclusive => (initflags & BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE) != 0;

	public bool IsEventDriven => (initflags & BASSWASAPIInit.BASS_WASAPI_EVENT) != 0;

	public override string ToString()
	{
		return string.Format("{0}, {1}Hz, {2}", (format == BASSWASAPIFormat.BASS_WASAPI_FORMAT_FLOAT || format == BASSWASAPIFormat.BASS_WASAPI_FORMAT_32BIT) ? "32-bit" : ((format == BASSWASAPIFormat.BASS_WASAPI_FORMAT_24BIT) ? "24-bit" : ((format == BASSWASAPIFormat.BASS_WASAPI_FORMAT_16BIT) ? "16-bit" : ((format == BASSWASAPIFormat.BASS_WASAPI_FORMAT_8BIT) ? "8-bit" : "Unknown"))), freq, Utils.ChannelNumberToString(chans));
	}
}
