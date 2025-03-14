using System;

namespace Un4seen.Bass.AddOn.Midi;

[Serializable]
public struct BASS_MIDI_EVENT
{
	public BASSMIDIEvent eventtype;

	public int param;

	public int chan;

	public int tick;

	public int pos;

	public BASS_MIDI_EVENT(BASSMIDIEvent EventType, int Param, int Chan, int Tick, int Pos)
	{
		eventtype = EventType;
		param = Param;
		chan = Chan;
		tick = Tick;
		pos = Pos;
	}

	public override string ToString()
	{
		return $"Event={eventtype}, Param={param}, Chan={chan}, Tick={tick} ({pos})";
	}
}
