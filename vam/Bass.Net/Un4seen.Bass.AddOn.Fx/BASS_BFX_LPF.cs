using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[Obsolete("This effect is obsolete; use 2x BASS_FX_BFX_BQF with BASS_BFX_BQF_LOWPASS filter and appropriate fQ values instead")]
public sealed class BASS_BFX_LPF
{
	public float fResonance = 2f;

	public float fCutOffFreq = 200f;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_LPF()
	{
	}

	public BASS_BFX_LPF(float Resonance, float CutOffFreq)
	{
		fResonance = Resonance;
		fCutOffFreq = CutOffFreq;
	}

	public BASS_BFX_LPF(float Resonance, float CutOffFreq, BASSFXChan chans)
	{
		fResonance = Resonance;
		fCutOffFreq = CutOffFreq;
		lChannel = chans;
	}
}
