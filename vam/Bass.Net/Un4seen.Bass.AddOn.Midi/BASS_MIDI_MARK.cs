using System;

namespace Un4seen.Bass.AddOn.Midi;

[Serializable]
public sealed class BASS_MIDI_MARK
{
	internal MIDI_MARK_INTERNAL _internal;

	public int track;

	public int pos;

	public string text = string.Empty;

	public BASS_MIDI_MARK()
	{
	}

	public BASS_MIDI_MARK(int Track, int Pos, string Text)
	{
		track = Track;
		pos = Pos;
		text = Text;
	}

	public override string ToString()
	{
		return text;
	}
}
