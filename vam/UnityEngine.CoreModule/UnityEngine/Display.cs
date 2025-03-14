using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Provides access to a display / screen for rendering operations.</para>
/// </summary>
[UsedByNativeCode]
public sealed class Display
{
	public delegate void DisplaysUpdatedDelegate();

	internal IntPtr nativeDisplay;

	/// <summary>
	///   <para>The list of currently connected Displays. Contains at least one (main) display.</para>
	/// </summary>
	public static Display[] displays = new Display[1]
	{
		new Display()
	};

	private static Display _mainDisplay = displays[0];

	/// <summary>
	///   <para>Horizontal resolution that the display is rendering at.</para>
	/// </summary>
	public int renderingWidth
	{
		get
		{
			int w = 0;
			int h = 0;
			GetRenderingExtImpl(nativeDisplay, out w, out h);
			return w;
		}
	}

	/// <summary>
	///   <para>Vertical resolution that the display is rendering at.</para>
	/// </summary>
	public int renderingHeight
	{
		get
		{
			int w = 0;
			int h = 0;
			GetRenderingExtImpl(nativeDisplay, out w, out h);
			return h;
		}
	}

	/// <summary>
	///   <para>Horizontal native display resolution.</para>
	/// </summary>
	public int systemWidth
	{
		get
		{
			int w = 0;
			int h = 0;
			GetSystemExtImpl(nativeDisplay, out w, out h);
			return w;
		}
	}

	/// <summary>
	///   <para>Vertical native display resolution.</para>
	/// </summary>
	public int systemHeight
	{
		get
		{
			int w = 0;
			int h = 0;
			GetSystemExtImpl(nativeDisplay, out w, out h);
			return h;
		}
	}

	/// <summary>
	///   <para>Color RenderBuffer.</para>
	/// </summary>
	public RenderBuffer colorBuffer
	{
		get
		{
			GetRenderingBuffersImpl(nativeDisplay, out var color, out var _);
			return color;
		}
	}

	/// <summary>
	///   <para>Depth RenderBuffer.</para>
	/// </summary>
	public RenderBuffer depthBuffer
	{
		get
		{
			GetRenderingBuffersImpl(nativeDisplay, out var _, out var depth);
			return depth;
		}
	}

	/// <summary>
	///   <para>Gets the state of the display and returns true if the display is active and false if otherwise.</para>
	/// </summary>
	public bool active => GetActiveImp(nativeDisplay);

	/// <summary>
	///   <para>Main Display.</para>
	/// </summary>
	public static Display main => _mainDisplay;

	public static event DisplaysUpdatedDelegate onDisplaysUpdated;

	internal Display()
	{
		nativeDisplay = new IntPtr(0);
	}

	internal Display(IntPtr nativeDisplay)
	{
		this.nativeDisplay = nativeDisplay;
	}

	/// <summary>
	///   <para>Activate an external display. Eg. Secondary Monitors connected to the System.</para>
	/// </summary>
	public void Activate()
	{
		ActivateDisplayImpl(nativeDisplay, 0, 0, 60);
	}

	/// <summary>
	///   <para>This overloaded function available for Windows allows specifying desired Window Width, Height and Refresh Rate.</para>
	/// </summary>
	/// <param name="width">Desired Width of the Window (for Windows only. On Linux and Mac uses Screen Width).</param>
	/// <param name="height">Desired Height of the Window (for Windows only. On Linux and Mac uses Screen Height).</param>
	/// <param name="refreshRate">Desired Refresh Rate.</param>
	public void Activate(int width, int height, int refreshRate)
	{
		ActivateDisplayImpl(nativeDisplay, width, height, refreshRate);
	}

	/// <summary>
	///   <para>Set rendering size and position on screen (Windows only).</para>
	/// </summary>
	/// <param name="width">Change Window Width (Windows Only).</param>
	/// <param name="height">Change Window Height (Windows Only).</param>
	/// <param name="x">Change Window Position X (Windows Only).</param>
	/// <param name="y">Change Window Position Y (Windows Only).</param>
	public void SetParams(int width, int height, int x, int y)
	{
		SetParamsImpl(nativeDisplay, width, height, x, y);
	}

	/// <summary>
	///   <para>Sets rendering resolution for the display.</para>
	/// </summary>
	/// <param name="w">Rendering width in pixels.</param>
	/// <param name="h">Rendering height in pixels.</param>
	public void SetRenderingResolution(int w, int h)
	{
		SetRenderingResolutionImpl(nativeDisplay, w, h);
	}

	[Obsolete("MultiDisplayLicense has been deprecated.", false)]
	public static bool MultiDisplayLicense()
	{
		return true;
	}

	/// <summary>
	///   <para>Query relative mouse coordinates.</para>
	/// </summary>
	/// <param name="inputMouseCoordinates">Mouse Input Position as Coordinates.</param>
	public static Vector3 RelativeMouseAt(Vector3 inputMouseCoordinates)
	{
		int rx = 0;
		int ry = 0;
		int x = (int)inputMouseCoordinates.x;
		int y = (int)inputMouseCoordinates.y;
		Vector3 result = default(Vector3);
		result.z = RelativeMouseAtImpl(x, y, out rx, out ry);
		result.x = rx;
		result.y = ry;
		return result;
	}

	[RequiredByNativeCode]
	private static void RecreateDisplayList(IntPtr[] nativeDisplay)
	{
		if (nativeDisplay.Length != 0)
		{
			displays = new Display[nativeDisplay.Length];
			for (int i = 0; i < nativeDisplay.Length; i++)
			{
				displays[i] = new Display(nativeDisplay[i]);
			}
			_mainDisplay = displays[0];
		}
	}

	[RequiredByNativeCode]
	private static void FireDisplaysUpdated()
	{
		if (Display.onDisplaysUpdated != null)
		{
			Display.onDisplaysUpdated();
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void GetSystemExtImpl(IntPtr nativeDisplay, out int w, out int h);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void GetRenderingExtImpl(IntPtr nativeDisplay, out int w, out int h);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void GetRenderingBuffersImpl(IntPtr nativeDisplay, out RenderBuffer color, out RenderBuffer depth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SetRenderingResolutionImpl(IntPtr nativeDisplay, int w, int h);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void ActivateDisplayImpl(IntPtr nativeDisplay, int width, int height, int refreshRate);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SetParamsImpl(IntPtr nativeDisplay, int width, int height, int x, int y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int RelativeMouseAtImpl(int x, int y, out int rx, out int ry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool GetActiveImp(IntPtr nativeDisplay);

	static Display()
	{
		Display.onDisplaysUpdated = null;
	}
}
