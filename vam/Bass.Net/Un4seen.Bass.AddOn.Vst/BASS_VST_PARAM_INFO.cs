using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Vst;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_VST_PARAM_INFO
{
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
	public string name = string.Empty;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
	public string unit = string.Empty;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
	public string display = string.Empty;

	public float defaultValue;

	public override string ToString()
	{
		return $"{name} = {display}";
	}
}
