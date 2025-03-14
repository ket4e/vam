namespace System.Drawing;

internal struct GdiplusStartupInput
{
	internal uint GdiplusVersion;

	internal IntPtr DebugEventCallback;

	internal int SuppressBackgroundThread;

	internal int SuppressExternalCodecs;

	internal static GdiplusStartupInput MakeGdiplusStartupInput()
	{
		GdiplusStartupInput result = default(GdiplusStartupInput);
		result.GdiplusVersion = 1u;
		result.DebugEventCallback = IntPtr.Zero;
		result.SuppressBackgroundThread = 0;
		result.SuppressExternalCodecs = 0;
		return result;
	}
}
