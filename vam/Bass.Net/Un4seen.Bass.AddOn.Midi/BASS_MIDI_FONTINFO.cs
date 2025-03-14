using System;

namespace Un4seen.Bass.AddOn.Midi;

[Serializable]
public sealed class BASS_MIDI_FONTINFO
{
	internal BASS_MIDI_FONTINFO_INTERNAL _internal;

	public string name = string.Empty;

	public string copyright = string.Empty;

	public string comment = string.Empty;

	public int presets;

	public int samsize;

	public int samload;

	public int samtype = -1;

	public override string ToString()
	{
		return $"Name={((name == null) ? string.Empty : name)}, Copyright={((copyright == null) ? string.Empty : copyright)}, Comment={((comment == null) ? string.Empty : comment)}";
	}
}
