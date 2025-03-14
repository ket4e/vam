using System;

namespace Un4seen.Bass.AddOn.Midi;

[Serializable]
public sealed class BASS_MIDI_DEVICEINFO
{
	internal BASS_MIDI_DEVICEINFO_INTERNAL _internal;

	public string name = string.Empty;

	public int id;

	public BASSDeviceInfo flags;

	public bool IsEnabled => (flags & BASSDeviceInfo.BASS_DEVICE_ENABLED) != 0;

	public bool IsInitialized => (flags & BASSDeviceInfo.BASS_DEVICE_INIT) != 0;

	public override string ToString()
	{
		return name;
	}
}
