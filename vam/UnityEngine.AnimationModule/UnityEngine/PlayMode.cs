namespace UnityEngine;

/// <summary>
///   <para>Used by Animation.Play function.</para>
/// </summary>
public enum PlayMode
{
	/// <summary>
	///   <para>Will stop all animations that were started in the same layer. This is the default when playing animations.</para>
	/// </summary>
	StopSameLayer = 0,
	/// <summary>
	///   <para>Will stop all animations that were started with this component before playing.</para>
	/// </summary>
	StopAll = 4
}
