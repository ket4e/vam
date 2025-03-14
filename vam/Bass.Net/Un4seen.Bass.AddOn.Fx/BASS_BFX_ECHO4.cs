using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_BFX_ECHO4
{
	public float fDryMix;

	public float fWetMix;

	public float fFeedback;

	public float fDelay;

	[MarshalAs(UnmanagedType.Bool)]
	public bool bStereo;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_ECHO4()
	{
	}

	public BASS_BFX_ECHO4(float DryMix, float WetMix, float Delay)
	{
		fDryMix = DryMix;
		fWetMix = WetMix;
		fDelay = Delay;
	}

	public BASS_BFX_ECHO4(float DryMix, float WetMix, float Delay, float Feedback)
	{
		fDryMix = DryMix;
		fWetMix = WetMix;
		fDelay = Delay;
		fFeedback = Feedback;
	}

	public BASS_BFX_ECHO4(float DryMix, float WetMix, float Delay, float Feedback, bool Stereo)
	{
		fDryMix = DryMix;
		fWetMix = WetMix;
		fDelay = Delay;
		fFeedback = Feedback;
		bStereo = Stereo;
	}

	public BASS_BFX_ECHO4(float DryMix, float WetMix, float Delay, float Feedback, bool Stereo, BASSFXChan chans)
	{
		fDryMix = DryMix;
		fWetMix = WetMix;
		fDelay = Delay;
		fFeedback = Feedback;
		bStereo = Stereo;
		lChannel = chans;
	}

	public void Preset_SmallEcho()
	{
		fDryMix = 0.999f;
		fWetMix = 0.999f;
		fDelay = 0.2f;
	}

	public void Preset_DoubleKick()
	{
		fDryMix = 0.5f;
		fWetMix = 0.599f;
		fDelay = 0.5f;
	}

	public void Preset_LongEcho()
	{
		fDryMix = 0.999f;
		fWetMix = 0.699f;
		fDelay = 0.9f;
	}
}
