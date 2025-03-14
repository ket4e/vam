using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_BFX_AUTOWAH
{
	public float fDryMix;

	public float fWetMix;

	public float fFeedback;

	public float fRate;

	public float fRange;

	public float fFreq;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_AUTOWAH()
	{
	}

	public BASS_BFX_AUTOWAH(float DryMix, float WetMix, float Feedback, float Rate, float Range, float Freq)
	{
		fDryMix = DryMix;
		fWetMix = WetMix;
		fFeedback = Feedback;
		fRate = Rate;
		fRange = Range;
		fFreq = Freq;
	}

	public BASS_BFX_AUTOWAH(float DryMix, float WetMix, float Feedback, float Rate, float Range, float Freq, BASSFXChan chans)
	{
		fDryMix = DryMix;
		fWetMix = WetMix;
		fFeedback = Feedback;
		fRate = Rate;
		fRange = Range;
		fFreq = Freq;
		lChannel = chans;
	}

	public void Preset_Default()
	{
		fDryMix = -1f;
		fWetMix = 1f;
		fFeedback = 0.06f;
		fRate = 0.2f;
		fRange = 6f;
		fFreq = 100f;
	}

	public void Preset_SlowAutoWah()
	{
		fDryMix = 0.5f;
		fWetMix = 1.5f;
		fFeedback = 0.5f;
		fRate = 2f;
		fRange = 4.3f;
		fFreq = 50f;
	}

	public void Preset_FastAutoWah()
	{
		fDryMix = 0.5f;
		fWetMix = 1.5f;
		fFeedback = 0.5f;
		fRate = 5f;
		fRange = 5.3f;
		fFreq = 50f;
	}

	public void Preset_HiFastAutoWah()
	{
		fDryMix = 0.5f;
		fWetMix = 1.5f;
		fFeedback = 0.5f;
		fRate = 5f;
		fRange = 4.3f;
		fFreq = 500f;
	}
}
