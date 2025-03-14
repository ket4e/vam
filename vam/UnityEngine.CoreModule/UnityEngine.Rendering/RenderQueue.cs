namespace UnityEngine.Rendering;

/// <summary>
///   <para>Determine in which order objects are renderered.</para>
/// </summary>
public enum RenderQueue
{
	/// <summary>
	///   <para>This render queue is rendered before any others.</para>
	/// </summary>
	Background = 1000,
	/// <summary>
	///   <para>Opaque geometry uses this queue.</para>
	/// </summary>
	Geometry = 2000,
	/// <summary>
	///   <para>Alpha tested geometry uses this queue.</para>
	/// </summary>
	AlphaTest = 2450,
	/// <summary>
	///   <para>Last render queue that is considered "opaque".</para>
	/// </summary>
	GeometryLast = 2500,
	/// <summary>
	///   <para>This render queue is rendered after Geometry and AlphaTest, in back-to-front order.</para>
	/// </summary>
	Transparent = 3000,
	/// <summary>
	///   <para>This render queue is meant for overlay effects.</para>
	/// </summary>
	Overlay = 4000
}
