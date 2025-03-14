using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_BFX_DAMP
{
	public float fTarget = 1f;

	public float fQuiet;

	public float fRate;

	public float fGain;

	public float fDelay;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_DAMP()
	{
	}

	public BASS_BFX_DAMP(float Target, float Quiet, float Rate, float Gain, float Delay)
	{
		fTarget = Target;
		fQuiet = Quiet;
		fRate = Rate;
		fGain = Gain;
		fDelay = Delay;
	}

	public BASS_BFX_DAMP(float Target, float Quiet, float Rate, float Gain, float Delay, BASSFXChan chans)
	{
		fTarget = Target;
		fQuiet = Quiet;
		fRate = Rate;
		fGain = Gain;
		fDelay = Delay;
		lChannel = chans;
	}

	public void Preset_Soft()
	{
		fTarget = 0.92f;
		fQuiet = 0.02f;
		fRate = 0.01f;
		fGain = 1f;
		fDelay = 0.5f;
	}

	public void Preset_Medium()
	{
		fTarget = 0.94f;
		fQuiet = 0.03f;
		fRate = 0.01f;
		fGain = 1f;
		fDelay = 0.35f;
	}

	public void Preset_Hard()
	{
		fTarget = 0.98f;
		fQuiet = 0.04f;
		fRate = 0.02f;
		fGain = 2f;
		fDelay = 0.2f;
	}
}
