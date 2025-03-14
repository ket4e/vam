using System;

namespace Un4seen.Bass.AddOn.DShow;

[Serializable]
public sealed class BASS_DSHOW_STREAMS
{
	internal BASS_DSHOW_STREAMS_INTERNAL _internal;

	public int format;

	public string name;

	public int index;

	public bool enabled;

	public bool IsAudio => format == 2;

	public bool IsVideo => format == 1;

	public override string ToString()
	{
		return name;
	}
}
