using System;

namespace Un4seen.Bass.AddOn.Midi;

[Serializable]
public struct BASS_MIDI_FONT
{
	public int font;

	public int preset;

	public int bank;

	public BASS_MIDI_FONT(int Font, int Preset, int Bank)
	{
		font = Font;
		preset = Preset;
		bank = Bank;
	}

	public override string ToString()
	{
		return $"Font={font}, Preset={preset}, Bank={bank}";
	}
}
