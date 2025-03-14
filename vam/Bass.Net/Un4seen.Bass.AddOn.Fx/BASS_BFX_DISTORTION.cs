using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_BFX_DISTORTION
{
	public float fDrive;

	public float fDryMix;

	public float fWetMix;

	public float fFeedback;

	public float fVolume;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_DISTORTION()
	{
	}

	public BASS_BFX_DISTORTION(float Drive, float DryMix, float WetMix, float Feedback, float Volume)
	{
		fDrive = Drive;
		fDryMix = DryMix;
		fWetMix = WetMix;
		fFeedback = Feedback;
		fVolume = Volume;
	}

	public BASS_BFX_DISTORTION(float Drive, float DryMix, float WetMix, float Feedback, float Volume, BASSFXChan chans)
	{
		fDrive = Drive;
		fDryMix = DryMix;
		fWetMix = WetMix;
		fFeedback = Feedback;
		fVolume = Volume;
		lChannel = chans;
	}

	public void Preset_HardDistortion()
	{
		fDrive = 1f;
		fDryMix = 0f;
		fWetMix = 1f;
		fFeedback = 0f;
		fVolume = 1f;
	}

	public void Preset_VeryHardDistortion()
	{
		fDrive = 5f;
		fDryMix = 0f;
		fWetMix = 1f;
		fFeedback = 0.1f;
		fVolume = 1f;
	}

	public void Preset_MediumDistortion()
	{
		fDrive = 0.2f;
		fDryMix = 1f;
		fWetMix = 1f;
		fFeedback = 0.1f;
		fVolume = 1f;
	}
}
