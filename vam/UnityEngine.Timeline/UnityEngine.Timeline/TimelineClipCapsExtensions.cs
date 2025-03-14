namespace UnityEngine.Timeline;

internal static class TimelineClipCapsExtensions
{
	public static bool SupportsLooping(this TimelineClip clip)
	{
		return clip != null && (clip.clipCaps & ClipCaps.Looping) != 0;
	}

	public static bool SupportsExtrapolation(this TimelineClip clip)
	{
		return clip != null && (clip.clipCaps & ClipCaps.Extrapolation) != 0;
	}

	public static bool SupportsClipIn(this TimelineClip clip)
	{
		return clip != null && (clip.clipCaps & ClipCaps.ClipIn) != 0;
	}

	public static bool SupportsSpeedMultiplier(this TimelineClip clip)
	{
		return clip != null && (clip.clipCaps & ClipCaps.SpeedMultiplier) != 0;
	}

	public static bool SupportsBlending(this TimelineClip clip)
	{
		return clip != null && (clip.clipCaps & ClipCaps.Blending) != 0;
	}

	public static bool HasAll(this ClipCaps caps, ClipCaps flags)
	{
		return (caps & flags) == flags;
	}

	public static bool HasAny(this ClipCaps caps, ClipCaps flags)
	{
		return (caps & flags) != 0;
	}
}
