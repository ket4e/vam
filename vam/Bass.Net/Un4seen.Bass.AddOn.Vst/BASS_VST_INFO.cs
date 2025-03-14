using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Vst;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_VST_INFO
{
	public int channelHandle;

	public int uniqueID;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
	public string effectName = string.Empty;

	public int effectVersion;

	public int effectVstVersion;

	public int hostVstVersion;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
	public string productName = string.Empty;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
	public string vendorName = string.Empty;

	public int vendorVersion;

	public int chansIn;

	public int chansOut;

	public int initialDelay;

	[MarshalAs(UnmanagedType.Bool)]
	public bool hasEditor;

	public int editorWidth;

	public int editorHeight;

	public IntPtr aeffect = IntPtr.Zero;

	[MarshalAs(UnmanagedType.Bool)]
	public bool isInstrument;

	public int dspHandle;

	public override string ToString()
	{
		if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(vendorName))
		{
			return $"{effectName} ({productName}, {vendorName})";
		}
		if (string.IsNullOrEmpty(vendorName) && !string.IsNullOrEmpty(productName))
		{
			return $"{effectName} ({productName})";
		}
		if (string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(vendorName))
		{
			return $"{effectName} ({vendorName})";
		}
		return string.Format("{0}", string.IsNullOrEmpty(effectName) ? "Unknown" : effectName);
	}
}
