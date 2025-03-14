using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

/// <summary>
///   <para>Access to display information.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/ScreenManager.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[StaticAccessor("GetScreenManager()", StaticAccessorType.Dot)]
public sealed class Screen
{
	/// <summary>
	///   <para>The current width of the screen window in pixels (Read Only).</para>
	/// </summary>
	public static extern int width
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(Name = "GetWidth", IsThreadSafe = true)]
		get;
	}

	/// <summary>
	///   <para>The current height of the screen window in pixels (Read Only).</para>
	/// </summary>
	public static extern int height
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(Name = "GetHeight", IsThreadSafe = true)]
		get;
	}

	/// <summary>
	///   <para>The current DPI of the screen / device (Read Only).</para>
	/// </summary>
	public static extern float dpi
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetDPI")]
		get;
	}

	/// <summary>
	///   <para>Specifies logical orientation of the screen.</para>
	/// </summary>
	public static ScreenOrientation orientation
	{
		get
		{
			return GetScreenOrientation();
		}
		set
		{
			if (value == ScreenOrientation.Unknown)
			{
				Debug.Log("ScreenOrientation.Unknown is deprecated. Please use ScreenOrientation.AutoRotation");
				value = ScreenOrientation.AutoRotation;
			}
			RequestOrientation(value);
		}
	}

	/// <summary>
	///   <para>A power saving setting, allowing the screen to dim some time after the last active user interaction.</para>
	/// </summary>
	[NativeProperty("ScreenTimeout")]
	public static extern int sleepTimeout
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Allow auto-rotation to portrait?</para>
	/// </summary>
	public static bool autorotateToPortrait
	{
		get
		{
			return IsOrientationEnabled(EnabledOrientation.kAutorotateToPortrait);
		}
		set
		{
			SetOrientationEnabled(EnabledOrientation.kAutorotateToPortrait, value);
		}
	}

	/// <summary>
	///   <para>Allow auto-rotation to portrait, upside down?</para>
	/// </summary>
	public static bool autorotateToPortraitUpsideDown
	{
		get
		{
			return IsOrientationEnabled(EnabledOrientation.kAutorotateToPortraitUpsideDown);
		}
		set
		{
			SetOrientationEnabled(EnabledOrientation.kAutorotateToPortraitUpsideDown, value);
		}
	}

	/// <summary>
	///   <para>Allow auto-rotation to landscape left?</para>
	/// </summary>
	public static bool autorotateToLandscapeLeft
	{
		get
		{
			return IsOrientationEnabled(EnabledOrientation.kAutorotateToLandscapeLeft);
		}
		set
		{
			SetOrientationEnabled(EnabledOrientation.kAutorotateToLandscapeLeft, value);
		}
	}

	/// <summary>
	///   <para>Allow auto-rotation to landscape right?</para>
	/// </summary>
	public static bool autorotateToLandscapeRight
	{
		get
		{
			return IsOrientationEnabled(EnabledOrientation.kAutorotateToLandscapeRight);
		}
		set
		{
			SetOrientationEnabled(EnabledOrientation.kAutorotateToLandscapeRight, value);
		}
	}

	/// <summary>
	///   <para>The current screen resolution (Read Only).</para>
	/// </summary>
	public static Resolution currentResolution
	{
		get
		{
			get_currentResolution_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Is the game running fullscreen?</para>
	/// </summary>
	public static extern bool fullScreen
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("IsFullscreen")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("RequestSetFullscreenFromScript")]
		set;
	}

	/// <summary>
	///   <para>Set this property to one of the values in FullScreenMode to change the display mode of your application.</para>
	/// </summary>
	public static extern FullScreenMode fullScreenMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetFullscreenMode")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("RequestSetFullscreenModeFromScript")]
		set;
	}

	/// <summary>
	///   <para>Returns the safe area of the screen in pixels (Read Only).</para>
	/// </summary>
	public static Rect safeArea
	{
		get
		{
			get_safeArea_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>All fullscreen resolutions supported by the monitor (Read Only).</para>
	/// </summary>
	public static extern Resolution[] resolutions
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("ScreenScripting::GetResolutions")]
		get;
	}

	/// <summary>
	///   <para>Should the cursor be locked?</para>
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use Cursor.lockState and Cursor.visible instead.", false)]
	public static bool lockCursor
	{
		get
		{
			return CursorLockMode.Locked == Cursor.lockState;
		}
		set
		{
			if (value)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RequestOrientation(ScreenOrientation orient);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ScreenOrientation GetScreenOrientation();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetIsOrientationEnabled")]
	private static extern bool IsOrientationEnabled(EnabledOrientation orient);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetIsOrientationEnabled")]
	private static extern void SetOrientationEnabled(EnabledOrientation orient, bool enabled);

	/// <summary>
	///   <para>Switches the screen resolution.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="fullscreen"></param>
	/// <param name="preferredRefreshRate"></param>
	/// <param name="fullscreenMode"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("RequestResolution")]
	public static extern void SetResolution(int width, int height, FullScreenMode fullscreenMode, [UnityEngine.Internal.DefaultValue("0")] int preferredRefreshRate);

	public static void SetResolution(int width, int height, FullScreenMode fullscreenMode)
	{
		SetResolution(width, height, fullscreenMode, 0);
	}

	/// <summary>
	///   <para>Switches the screen resolution.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="fullscreen"></param>
	/// <param name="preferredRefreshRate"></param>
	/// <param name="fullscreenMode"></param>
	public static void SetResolution(int width, int height, bool fullscreen, [UnityEngine.Internal.DefaultValue("0")] int preferredRefreshRate)
	{
		SetResolution(width, height, fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, preferredRefreshRate);
	}

	/// <summary>
	///   <para>Switches the screen resolution.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="fullscreen"></param>
	/// <param name="preferredRefreshRate"></param>
	/// <param name="fullscreenMode"></param>
	public static void SetResolution(int width, int height, bool fullscreen)
	{
		SetResolution(width, height, fullscreen, 0);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_currentResolution_Injected(out Resolution ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_safeArea_Injected(out Rect ret);
}
