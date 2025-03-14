using System;

namespace Un4seen.Bass.AddOn.DShow;

[Serializable]
public sealed class BASS_DSHOW_VIDEOCOLORS
{
	public float Contrast;

	public float Brightness;

	public float Hue;

	public float Saturation;

	public override string ToString()
	{
		return $"Contrast={Contrast:F}, Brightness={Brightness:F}, Hue={Hue:F}, Saturation={Saturation:F}";
	}
}
