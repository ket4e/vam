namespace radio42.Multimedia.Midi;

public enum MIDITimeType
{
	TIME_MS = 1,
	TIME_SAMPLES = 2,
	TIME_BYTES = 4,
	TIME_SMPTE = 8,
	TIME_MIDI = 0x10,
	TIME_TICKS = 0x20
}
