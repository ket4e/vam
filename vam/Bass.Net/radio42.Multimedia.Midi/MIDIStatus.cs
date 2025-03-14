namespace radio42.Multimedia.Midi;

public enum MIDIStatus : byte
{
	None = 0,
	NoteOff = 128,
	NoteOn = 144,
	Aftertouch = 160,
	ControlChange = 176,
	ProgramChange = 192,
	ChannelPressure = 208,
	PitchBend = 224,
	SystemMsgs = 240,
	MidiTimeCode = 241,
	SongPosition = 242,
	SongSelect = 243,
	TuneRequest = 246,
	EOX = 247,
	Clock = 248,
	Tick = 249,
	Start = 250,
	Continue = 251,
	Stop = 252,
	ActiveSense = 254,
	Reset = byte.MaxValue
}
