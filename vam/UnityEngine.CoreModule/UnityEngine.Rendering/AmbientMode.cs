namespace UnityEngine.Rendering;

/// <summary>
///   <para>Ambient lighting mode.</para>
/// </summary>
public enum AmbientMode
{
	/// <summary>
	///   <para>Skybox-based or custom ambient lighting.</para>
	/// </summary>
	Skybox = 0,
	/// <summary>
	///   <para>Trilight ambient lighting.</para>
	/// </summary>
	Trilight = 1,
	/// <summary>
	///   <para>Flat ambient lighting.</para>
	/// </summary>
	Flat = 3,
	/// <summary>
	///   <para>Ambient lighting is defined by a custom cubemap.</para>
	/// </summary>
	Custom = 4
}
