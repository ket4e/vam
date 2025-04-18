using System;
using System.Runtime.InteropServices;

namespace radio42.Multimedia.Midi;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class MIDI_OUTCAPS
{
	public short mid;

	public short pid;

	public int driverVersion;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
	public string name = string.Empty;

	public MIDIDevice technology;

	public short voices;

	public short notes;

	public short channelMask;

	public int support;

	public bool SupportsCache => (support & 4) != 0;

	public bool SupportsLRVolume => (support & 2) != 0;

	public bool SupportsVolume => (support & 1) != 0;

	public bool SupportsStream => (support & 8) != 0;

	public bool IsMidiPort => technology == MIDIDevice.MOD_MIDIPORT;

	public bool IsMidiMapper => technology == MIDIDevice.MOD_MAPPER;

	public override string ToString()
	{
		return name;
	}
}
