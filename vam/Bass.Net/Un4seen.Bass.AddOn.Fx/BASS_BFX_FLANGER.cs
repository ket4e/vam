using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[Obsolete("This effect is obsolete; use BASS_FX_BFX_CHORUS instead")]
public sealed class BASS_BFX_FLANGER
{
	public float fWetDry = 1f;

	public float fSpeed = 0.01f;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_FLANGER()
	{
	}

	public BASS_BFX_FLANGER(float WetDry, float Speed)
	{
		fWetDry = WetDry;
		fSpeed = Speed;
	}

	public BASS_BFX_FLANGER(float WetDry, float Speed, BASSFXChan chans)
	{
		fWetDry = WetDry;
		fSpeed = Speed;
		lChannel = chans;
	}

	public void Preset_Default()
	{
		fWetDry = 1f;
		fSpeed = 0.01f;
	}
}
