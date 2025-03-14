using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_BFX_PITCHSHIFT
{
	public float fPitchShift = 1f;

	public float fSemitones;

	public long lFFTsize = 2048L;

	public long lOsamp = 8L;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_PITCHSHIFT()
	{
	}

	public BASS_BFX_PITCHSHIFT(float PitchShift, float Semitones, long FFTsize, long Osamp)
	{
		fPitchShift = PitchShift;
		fSemitones = Semitones;
		lFFTsize = FFTsize;
		lOsamp = Osamp;
	}
}
