using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[Obsolete("This effect is obsolete; use BASS_FX_BFX_BQF with BASS_BFX_BQF_ALLPASS filter instead")]
public sealed class BASS_BFX_APF
{
	public float fGain;

	public float fDelay;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_APF()
	{
	}

	public BASS_BFX_APF(float Gain, float Delay)
	{
		fGain = Gain;
		fDelay = Delay;
	}

	public BASS_BFX_APF(float Gain, float Delay, BASSFXChan chans)
	{
		fGain = Gain;
		fDelay = Delay;
		lChannel = chans;
	}

	public void Preset_Default()
	{
		fGain = -0.5f;
		fDelay = 0.5f;
	}

	public void Preset_SmallRever()
	{
		fGain = 0.799f;
		fDelay = 0.2f;
	}

	public void Preset_RobotVoice()
	{
		fGain = 0.6f;
		fDelay = 0.05f;
	}

	public void Preset_LongReverberation()
	{
		fGain = 0.599f;
		fDelay = 1.3f;
	}
}
