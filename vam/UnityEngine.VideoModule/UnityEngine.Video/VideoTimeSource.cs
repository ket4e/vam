namespace UnityEngine.Video;

/// <summary>
///   <para>Time source followed by the Video.VideoPlayer when reading content.</para>
/// </summary>
public enum VideoTimeSource
{
	/// <summary>
	///   <para>The audio hardware clock.</para>
	/// </summary>
	AudioDSPTimeSource,
	/// <summary>
	///   <para>The unscaled game time as defined by Time.realtimeSinceStartup.</para>
	/// </summary>
	GameTimeSource
}
