using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Advertisements;

internal class UnityAdsSettings
{
	[ThreadAndSerializationSafe]
	public static extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public static extern bool initializeOnStartup
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public static extern bool testMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("No longer supported and will always return true")]
	[GeneratedByOldBindingsGenerator]
	public static extern bool IsPlatformEnabled(RuntimePlatform platform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("No longer supported and will do nothing")]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetPlatformEnabled(RuntimePlatform platform, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern string GetGameId(RuntimePlatform platform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetGameId(RuntimePlatform platform, string gameId);
}
