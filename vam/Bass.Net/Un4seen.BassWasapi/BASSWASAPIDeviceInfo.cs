using System;

namespace Un4seen.BassWasapi;

[Flags]
public enum BASSWASAPIDeviceInfo
{
	BASS_DEVICE_UNKNOWN = 0,
	BASS_DEVICE_ENABLED = 1,
	BASS_DEVICE_DEFAULT = 2,
	BASS_DEVICE_INIT = 4,
	BASS_DEVICE_LOOPBACK = 8,
	BASS_DEVICE_INPUT = 0x10,
	BASS_DEVICE_UNPLUGGED = 0x20,
	BASS_DEVICE_DISABLED = 0x40
}
