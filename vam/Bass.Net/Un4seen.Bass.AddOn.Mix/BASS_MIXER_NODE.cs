using System;

namespace Un4seen.Bass.AddOn.Mix;

[Serializable]
public struct BASS_MIXER_NODE
{
	public long pos;

	public float val;

	public BASS_MIXER_NODE(long Pos, float Val)
	{
		pos = Pos;
		val = Val;
	}

	public override string ToString()
	{
		return $"Pos={pos}, Val={val}";
	}
}
