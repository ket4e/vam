using System;
using System.Collections.Generic;

namespace Un4seen.Bass.AddOn.Cd;

[Serializable]
public sealed class BASS_CD_TOC
{
	public byte first;

	public byte last;

	public List<BASS_CD_TOC_TRACK> tracks = new List<BASS_CD_TOC_TRACK>();

	public override string ToString()
	{
		return $"{tracks.Count} tracks ({first} - {last})";
	}
}
