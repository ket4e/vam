using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Cd;

[Serializable]
internal struct BASS_CD_TOC_INTERNAL
{
	public short size;

	public byte first;

	public byte last;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
	public BASS_CD_TOC_TRACK[] tracks;

	public int NumberOfTracks => size / Marshal.SizeOf(typeof(BASS_CD_TOC_TRACK));
}
