using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.DShow;

[Serializable]
public sealed class BASS_DSHOW_CHANNELINFO
{
	public float AvgTimePerFrame;

	public int Height;

	public int Width;

	public int nChannels;

	public int freq;

	public int wBits;

	[MarshalAs(UnmanagedType.Bool)]
	public bool floatingpoint;

	public override string ToString()
	{
		if (Height == 0 && Width == 0 && nChannels != 0)
		{
			return $"{freq} Hz, {Utils.ChannelNumberToString(nChannels)}, {wBits} bits";
		}
		return $"{Width}x{Height}";
	}
}
