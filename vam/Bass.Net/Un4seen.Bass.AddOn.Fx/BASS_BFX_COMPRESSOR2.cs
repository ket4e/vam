using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_BFX_COMPRESSOR2
{
	public float fGain = 5f;

	public float fThreshold = -15f;

	public float fRatio = 3f;

	public float fAttack = 20f;

	public float fRelease = 200f;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_COMPRESSOR2()
	{
	}

	public BASS_BFX_COMPRESSOR2(float Gain, float Threshold, float Ratio, float Attack, float Release, BASSFXChan chans)
	{
		fGain = Gain;
		fThreshold = Threshold;
		fRatio = Ratio;
		fAttack = Attack;
		fRelease = Release;
		lChannel = chans;
	}

	public void Calculate0dBGain()
	{
		fGain = fThreshold / 2f * (1f / fRatio - 1f);
	}

	public void Preset_Default()
	{
		fThreshold = -15f;
		fRatio = 3f;
		fGain = 5f;
		fAttack = 20f;
		fRelease = 200f;
	}

	public void Preset_Soft()
	{
		fThreshold = -15f;
		fRatio = 2f;
		fGain = 3.7f;
		fAttack = 24f;
		fRelease = 800f;
	}

	public void Preset_Soft2()
	{
		fThreshold = -18f;
		fRatio = 3f;
		fGain = 6f;
		fAttack = 24f;
		fRelease = 800f;
	}

	public void Preset_Medium()
	{
		fThreshold = -20f;
		fRatio = 4f;
		fGain = 7.5f;
		fAttack = 16f;
		fRelease = 500f;
	}

	public void Preset_Hard()
	{
		fThreshold = -23f;
		fRatio = 8f;
		fGain = 10f;
		fAttack = 12f;
		fRelease = 400f;
	}

	public void Preset_Hard2()
	{
		fThreshold = -18f;
		fRatio = 9f;
		fGain = 8f;
		fAttack = 12f;
		fRelease = 200f;
	}

	public void Preset_HardCommercial()
	{
		fThreshold = -20f;
		fRatio = 10f;
		fGain = 9f;
		fAttack = 8f;
		fRelease = 250f;
	}
}
