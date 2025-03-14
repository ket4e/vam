using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_BFX_MIX : IDisposable
{
	private IntPtr ptr = IntPtr.Zero;

	private GCHandle hgc;

	public BASSFXChan[] lChannel;

	public BASS_BFX_MIX(int numChans)
	{
		lChannel = new BASSFXChan[numChans];
		for (int i = 0; i < numChans; i++)
		{
			lChannel[i] = (BASSFXChan)(1 << i);
		}
	}

	public BASS_BFX_MIX(params BASSFXChan[] channels)
	{
		lChannel = new BASSFXChan[channels.Length];
		for (int i = 0; i < channels.Length; i++)
		{
			lChannel[i] = channels[i];
		}
	}

	~BASS_BFX_MIX()
	{
		Dispose();
	}

	internal void Set()
	{
		if (hgc.IsAllocated)
		{
			hgc.Free();
			ptr = IntPtr.Zero;
		}
		int[] array = new int[lChannel.Length];
		for (int i = 0; i < lChannel.Length; i++)
		{
			array[i] = (int)lChannel[i];
		}
		hgc = GCHandle.Alloc(array, GCHandleType.Pinned);
		ptr = hgc.AddrOfPinnedObject();
	}

	internal void Get()
	{
		if (ptr != IntPtr.Zero)
		{
			int[] array = new int[lChannel.Length];
			Marshal.Copy(ptr, array, 0, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				lChannel[i] = (BASSFXChan)array[i];
			}
		}
	}

	public void Dispose()
	{
		if (hgc.IsAllocated)
		{
			hgc.Free();
			ptr = IntPtr.Zero;
		}
	}
}
