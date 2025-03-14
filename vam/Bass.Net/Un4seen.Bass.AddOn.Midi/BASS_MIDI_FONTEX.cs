using System;

namespace Un4seen.Bass.AddOn.Midi;

[Serializable]
public struct BASS_MIDI_FONTEX
{
	public int font;

	public int spreset;

	public int sbank;

	public int dpreset;

	public int dbank;

	public int dbanklsb;

	public BASS_MIDI_FONTEX(int Font, int SPreset, int SBank, int DPreset, int DBank, int DBanklsb)
	{
		font = Font;
		spreset = SPreset;
		sbank = SBank;
		dpreset = DPreset;
		dbank = DBank;
		dbanklsb = DBanklsb;
	}

	public override string ToString()
	{
		return $"Font={font}, Preset={spreset}, Bank={sbank}";
	}
}
