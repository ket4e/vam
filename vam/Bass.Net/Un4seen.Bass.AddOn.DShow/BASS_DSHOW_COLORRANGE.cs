using System;

namespace Un4seen.Bass.AddOn.DShow;

[Serializable]
public sealed class BASS_DSHOW_COLORRANGE
{
	public float MinValue;

	public float MaxValue;

	public float DefaultValue;

	public float StepSize;

	public BASSDSHOWColorControl type;

	public override string ToString()
	{
		return $"{type}: Min={MinValue:F}, Max={MaxValue:F}, Default={DefaultValue:F}, Step={StepSize:F}";
	}
}
