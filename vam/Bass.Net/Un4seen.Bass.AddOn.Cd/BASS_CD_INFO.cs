using System;

namespace Un4seen.Bass.AddOn.Cd;

[Serializable]
public sealed class BASS_CD_INFO
{
	internal BASS_CD_INFO_INTERNAL _internal;

	public string vendor = string.Empty;

	public string product = string.Empty;

	public string rev = string.Empty;

	public int letter = -1;

	public BASSCDRWFlags rwflags;

	public bool canopen;

	public bool canlock;

	public int maxspeed;

	public int cache;

	public bool cdtext;

	public char DriveLetter => (char)(65 + letter);

	public override string ToString()
	{
		return $"{DriveLetter}:\\ {product} ({vendor})";
	}
}
