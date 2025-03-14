using System;

namespace Un4seen.Bass.AddOn.DShow;

[Serializable]
public sealed class BASS_DSHOW_PLUGININFO
{
	internal BASS_DSHOW_PLUGININFO_INTERNAL _internal;

	public int version;

	public int decoderType;

	public string plgdescription;

	public bool IsAudio => decoderType == 1;

	public bool IsVideo => decoderType == 2;

	public override string ToString()
	{
		return plgdescription;
	}
}
