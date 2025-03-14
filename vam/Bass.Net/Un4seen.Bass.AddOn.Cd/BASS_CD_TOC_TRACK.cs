using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Cd;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BASS_CD_TOC_TRACK
{
	public byte res1;

	public byte adrcon;

	public byte track;

	public byte res2;

	public int lba;

	public byte hour => (byte)((uint)(lba >> 24) & 0xFu);

	public byte minute => (byte)((uint)(lba >> 16) & 0xFu);

	public byte second => (byte)((uint)(lba >> 8) & 0xFu);

	public byte frame => (byte)((uint)lba & 0xFu);

	public byte ADR => (byte)((uint)(adrcon >> 4) & 0xFu);

	public BASSCDTOCFlags Control => (BASSCDTOCFlags)(adrcon & 0xFu);

	public override string ToString()
	{
		if (track == 170)
		{
			return $"Lead-Out: adr={ADR}, con={Control}, start={lba} [{minute:00}:{second:00}:{frame:00}]";
		}
		return $"Track {track}: adr={ADR}, con={Control}, start={lba} [{minute:00}:{second:00}:{frame:00}]";
	}
}
