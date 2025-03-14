using System;

namespace Un4seen.BassWasapi;

[Flags]
public enum BASSWASAPIDeviceType
{
	BASS_WASAPI_TYPE_NETWORKDEVICE = 0,
	BASS_WASAPI_TYPE_SPEAKERS = 1,
	BASS_WASAPI_TYPE_LINELEVEL = 2,
	BASS_WASAPI_TYPE_HEADPHONES = 3,
	BASS_WASAPI_TYPE_MICROPHONE = 4,
	BASS_WASAPI_TYPE_HEADSET = 5,
	BASS_WASAPI_TYPE_HANDSET = 6,
	BASS_WASAPI_TYPE_DIGITAL = 7,
	BASS_WASAPI_TYPE_SPDIF = 8,
	BASS_WASAPI_TYPE_HDMI = 9,
	BASS_WASAPI_TYPE_UNKNOWN = 0xA
}
