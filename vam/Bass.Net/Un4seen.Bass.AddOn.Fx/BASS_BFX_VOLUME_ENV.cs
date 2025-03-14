using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_BFX_VOLUME_ENV : IDisposable
{
	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public int lNodeCount;

	private IntPtr ptr = IntPtr.Zero;

	[MarshalAs(UnmanagedType.Bool)]
	public bool bFollow = true;

	private GCHandle hgc;

	public BASS_BFX_ENV_NODE[] pNodes;

	public BASS_BFX_VOLUME_ENV()
	{
	}

	public BASS_BFX_VOLUME_ENV(int nodeCount)
	{
		lNodeCount = nodeCount;
		pNodes = new BASS_BFX_ENV_NODE[lNodeCount];
		for (int i = 0; i < lNodeCount; i++)
		{
			pNodes[i].pos = 0.0;
			pNodes[i].val = 1f;
		}
	}

	public BASS_BFX_VOLUME_ENV(params BASS_BFX_ENV_NODE[] nodes)
	{
		if (nodes == null)
		{
			lNodeCount = 0;
			pNodes = null;
			return;
		}
		lNodeCount = nodes.Length;
		pNodes = new BASS_BFX_ENV_NODE[lNodeCount];
		for (int i = 0; i < lNodeCount; i++)
		{
			pNodes[i].pos = nodes[i].pos;
			pNodes[i].val = nodes[i].val;
		}
	}

	~BASS_BFX_VOLUME_ENV()
	{
		Dispose();
	}

	internal void Set()
	{
		if (lNodeCount > 0)
		{
			if (hgc.IsAllocated)
			{
				hgc.Free();
				ptr = IntPtr.Zero;
			}
			hgc = GCHandle.Alloc(pNodes, GCHandleType.Pinned);
			ptr = hgc.AddrOfPinnedObject();
		}
		else
		{
			if (hgc.IsAllocated)
			{
				hgc.Free();
			}
			ptr = IntPtr.Zero;
		}
	}

	internal void Get()
	{
		if (ptr != IntPtr.Zero && lNodeCount > 0)
		{
			pNodes = new BASS_BFX_ENV_NODE[lNodeCount];
			ReadArrayStructure(lNodeCount, ptr);
		}
		else
		{
			pNodes = null;
		}
	}

	private unsafe void ReadArrayStructure(int count, IntPtr p)
	{
		for (int i = 0; i < count; i++)
		{
			pNodes[i] = (BASS_BFX_ENV_NODE)Marshal.PtrToStructure(p, typeof(BASS_BFX_ENV_NODE));
			p = new IntPtr((byte*)p.ToPointer() + Marshal.SizeOf(pNodes[i]));
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
