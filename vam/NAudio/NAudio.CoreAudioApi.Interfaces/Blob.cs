using System;

namespace NAudio.CoreAudioApi.Interfaces;

public struct Blob
{
	public int Length;

	public IntPtr Data;

	private void FixCS0649()
	{
		Length = 0;
		Data = IntPtr.Zero;
	}
}
