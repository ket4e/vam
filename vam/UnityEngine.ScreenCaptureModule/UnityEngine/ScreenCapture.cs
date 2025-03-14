using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

/// <summary>
///   <para>Functionality to take Screenshots.</para>
/// </summary>
[NativeHeader("Modules/ScreenCapture/Public/CaptureScreenshot.h")]
public static class ScreenCapture
{
	public static void CaptureScreenshot(string filename)
	{
		CaptureScreenshot(filename, 1);
	}

	/// <summary>
	///   <para>Captures a screenshot at path filename as a PNG file.</para>
	/// </summary>
	/// <param name="filename">Pathname to save the screenshot file to.</param>
	/// <param name="superSize">Factor by which to increase resolution.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void CaptureScreenshot(string filename, [DefaultValue("1")] int superSize);

	/// <summary>
	///   <para>Captures a screenshot of the game view into a Texture2D object.</para>
	/// </summary>
	/// <param name="superSize">Factor by which to increase resolution.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern Texture2D CaptureScreenshotAsTexture(int superSize = 1);
}
