using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Fx;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_BFX_FREEVERB
{
	public float fDryMix;

	public float fWetMix = 1f;

	public float fRoomSize = 0.5f;

	public float fDamp = 0.5f;

	public float fWidth = 1f;

	private int lMode;

	public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;

	public BASS_BFX_FREEVERB()
	{
	}

	public BASS_BFX_FREEVERB(float DryMix, float WetMix, float RoomSize, float Damp, float Width, int Mode)
	{
		fDryMix = DryMix;
		fWetMix = WetMix;
		fRoomSize = RoomSize;
		fDamp = Damp;
		fWidth = Width;
		lMode = Mode;
	}
}
