using System;

namespace Un4seen.BassWasapi;

[Serializable]
public sealed class BASS_WASAPI_DEVICEINFO
{
	internal BASS_WASAPI_DEVICEINFO_INTERNAL _internal;

	public string name = string.Empty;

	public string id = string.Empty;

	public BASSWASAPIDeviceType type = BASSWASAPIDeviceType.BASS_WASAPI_TYPE_UNKNOWN;

	public BASSWASAPIDeviceInfo flags;

	public float minperiod;

	public float defperiod;

	public int mixfreq;

	public int mixchans;

	public bool IsEnabled => (flags & BASSWASAPIDeviceInfo.BASS_DEVICE_ENABLED) != 0;

	public bool IsDisabled => (flags & BASSWASAPIDeviceInfo.BASS_DEVICE_DISABLED) != 0;

	public bool IsUnplugged => (flags & BASSWASAPIDeviceInfo.BASS_DEVICE_UNPLUGGED) != 0;

	public bool IsDefault => (flags & BASSWASAPIDeviceInfo.BASS_DEVICE_DEFAULT) != 0;

	public bool IsInitialized => (flags & BASSWASAPIDeviceInfo.BASS_DEVICE_INIT) != 0;

	public bool IsLoopback => (flags & BASSWASAPIDeviceInfo.BASS_DEVICE_LOOPBACK) != 0;

	public bool IsInput => (flags & BASSWASAPIDeviceInfo.BASS_DEVICE_INPUT) != 0;

	public bool SupportsRecording
	{
		get
		{
			if ((flags & BASSWASAPIDeviceInfo.BASS_DEVICE_INPUT) == 0)
			{
				return (flags & BASSWASAPIDeviceInfo.BASS_DEVICE_LOOPBACK) != 0;
			}
			return true;
		}
	}

	public override string ToString()
	{
		return name;
	}
}
