using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[Obsolete("This effect is obsolete; use BASS_FX_BFX_COMPRESSOR2 instead")]
public class BASS_BFX_COMPRESSOR
{
	public float fThreshold;

	public float fAttacktime;

	public float fReleasetime;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_COMPRESSOR()
	{
	}

	public BASS_BFX_COMPRESSOR(float Threshold, float Attacktime, float Releasetime)
	{
		fThreshold = Threshold;
		fAttacktime = Attacktime;
		fReleasetime = Releasetime;
	}

	public BASS_BFX_COMPRESSOR(float Threshold, float Attacktime, float Releasetime, BASSFXChan chans)
	{
		fThreshold = Threshold;
		fAttacktime = Attacktime;
		fReleasetime = Releasetime;
		lChannel = chans;
	}

	public void Preset_50Attack15msRelease1sec()
	{
		fThreshold = 0.5f;
		fAttacktime = 15f;
		fReleasetime = 1000f;
	}

	public void Preset_80Attack1msRelease05sec()
	{
		fThreshold = 0.8f;
		fAttacktime = 1f;
		fReleasetime = 500f;
	}

	public void Preset_Soft()
	{
		fThreshold = 0.89f;
		fAttacktime = 20f;
		fReleasetime = 350f;
	}

	public void Preset_SoftHigh()
	{
		fThreshold = 0.7f;
		fAttacktime = 10f;
		fReleasetime = 200f;
	}

	public void Preset_Medium()
	{
		fThreshold = 0.5f;
		fAttacktime = 5f;
		fReleasetime = 250f;
	}

	public void Preset_Hard()
	{
		fThreshold = 0.25f;
		fAttacktime = 2.2f;
		fReleasetime = 400f;
	}
}
