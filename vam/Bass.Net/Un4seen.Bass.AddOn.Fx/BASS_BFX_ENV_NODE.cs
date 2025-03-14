using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct BASS_BFX_ENV_NODE
{
	public double pos;

	public float val;

	public BASS_BFX_ENV_NODE(double Pos, float Val)
	{
		pos = Pos;
		val = Val;
	}

	public override string ToString()
	{
		return $"Pos={pos}, Val={val}";
	}
}
