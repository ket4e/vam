using System;

namespace UnityEngine;

/// <summary>
///   <para>The platform application is running. Returned by Application.platform.</para>
/// </summary>
public enum RuntimePlatform
{
	/// <summary>
	///   <para>In the Unity editor on macOS.</para>
	/// </summary>
	OSXEditor = 0,
	/// <summary>
	///   <para>In the player on macOS.</para>
	/// </summary>
	OSXPlayer = 1,
	/// <summary>
	///   <para>In the player on Windows.</para>
	/// </summary>
	WindowsPlayer = 2,
	/// <summary>
	///   <para>In the web player on macOS.</para>
	/// </summary>
	[Obsolete("WebPlayer export is no longer supported in Unity 5.4+.", true)]
	OSXWebPlayer = 3,
	/// <summary>
	///   <para>In the Dashboard widget on macOS.</para>
	/// </summary>
	[Obsolete("Dashboard widget on Mac OS X export is no longer supported in Unity 5.4+.", true)]
	OSXDashboardPlayer = 4,
	/// <summary>
	///   <para>In the web player on Windows.</para>
	/// </summary>
	[Obsolete("WebPlayer export is no longer supported in Unity 5.4+.", true)]
	WindowsWebPlayer = 5,
	/// <summary>
	///   <para>In the Unity editor on Windows.</para>
	/// </summary>
	WindowsEditor = 7,
	/// <summary>
	///   <para>In the player on the iPhone.</para>
	/// </summary>
	IPhonePlayer = 8,
	[Obsolete("Xbox360 export is no longer supported in Unity 5.5+.")]
	XBOX360 = 10,
	[Obsolete("PS3 export is no longer supported in Unity >=5.5.")]
	PS3 = 9,
	/// <summary>
	///   <para>In the player on Android devices.</para>
	/// </summary>
	Android = 11,
	[Obsolete("NaCl export is no longer supported in Unity 5.0+.")]
	NaCl = 12,
	[Obsolete("FlashPlayer export is no longer supported in Unity 5.0+.")]
	FlashPlayer = 15,
	/// <summary>
	///   <para>In the player on Linux.</para>
	/// </summary>
	LinuxPlayer = 13,
	/// <summary>
	///   <para>In the Unity editor on Linux.</para>
	/// </summary>
	LinuxEditor = 16,
	/// <summary>
	///   <para>In the player on WebGL</para>
	/// </summary>
	WebGLPlayer = 17,
	[Obsolete("Use WSAPlayerX86 instead")]
	MetroPlayerX86 = 18,
	/// <summary>
	///   <para>In the player on Windows Store Apps when CPU architecture is X86.</para>
	/// </summary>
	WSAPlayerX86 = 18,
	[Obsolete("Use WSAPlayerX64 instead")]
	MetroPlayerX64 = 19,
	/// <summary>
	///   <para>In the player on Windows Store Apps when CPU architecture is X64.</para>
	/// </summary>
	WSAPlayerX64 = 19,
	[Obsolete("Use WSAPlayerARM instead")]
	MetroPlayerARM = 20,
	/// <summary>
	///   <para>In the player on Windows Store Apps when CPU architecture is ARM.</para>
	/// </summary>
	WSAPlayerARM = 20,
	[Obsolete("Windows Phone 8 was removed in 5.3")]
	WP8Player = 21,
	[Obsolete("BlackBerryPlayer export is no longer supported in Unity 5.4+.")]
	BlackBerryPlayer = 22,
	TizenPlayer = 23,
	/// <summary>
	///   <para>In the player on the PS Vita.</para>
	/// </summary>
	PSP2 = 24,
	/// <summary>
	///   <para>In the player on the Playstation 4.</para>
	/// </summary>
	PS4 = 25,
	[Obsolete("PSM export is no longer supported in Unity >= 5.3")]
	PSM = 26,
	/// <summary>
	///   <para>In the player on Xbox One.</para>
	/// </summary>
	XboxOne = 27,
	[Obsolete("SamsungTVPlayer export is no longer supported in Unity 2017.3+.")]
	SamsungTVPlayer = 28,
	[Obsolete("Wii U is no longer supported in Unity 2018.1+.")]
	WiiU = 30,
	/// <summary>
	///   <para>In the player on the Apple's tvOS.</para>
	/// </summary>
	tvOS = 31,
	/// <summary>
	///   <para>In the player on Nintendo Switch.</para>
	/// </summary>
	Switch = 32
}
