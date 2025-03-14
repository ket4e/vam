using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[Obsolete("This effect is obsolete; use BASS_FX_BFX_ECHO4 with fFeedback enabled instead")]
public sealed class BASS_BFX_REVERB
{
	public float fLevel;

	public int lDelay = 1200;

	public BASS_BFX_REVERB()
	{
	}

	public BASS_BFX_REVERB(float Level, int Delay)
	{
		fLevel = Level;
		lDelay = Delay;
	}
}
