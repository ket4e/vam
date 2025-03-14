using System;

namespace Un4seen.BassAsio;

[Serializable]
public sealed class BASS_ASIO_DEVICEINFO
{
	internal BASS_ASIO_DEVICEINFO_INTERNAL _internal;

	public string name = string.Empty;

	public string driver = string.Empty;

	public override string ToString()
	{
		return name;
	}
}
