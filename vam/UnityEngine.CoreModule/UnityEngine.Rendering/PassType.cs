using System;

namespace UnityEngine.Rendering;

/// <summary>
///   <para>Shader pass type for Unity's lighting pipeline.</para>
/// </summary>
public enum PassType
{
	/// <summary>
	///   <para>Regular shader pass that does not interact with lighting.</para>
	/// </summary>
	Normal = 0,
	/// <summary>
	///   <para>Legacy vertex-lit shader pass.</para>
	/// </summary>
	Vertex = 1,
	/// <summary>
	///   <para>Legacy vertex-lit shader pass, with mobile lightmaps.</para>
	/// </summary>
	VertexLM = 2,
	/// <summary>
	///   <para>Legacy vertex-lit shader pass, with desktop (RGBM) lightmaps.</para>
	/// </summary>
	[Obsolete("VertexLMRGBM PassType is obsolete. Please use VertexLM PassType together with DecodeLightmap shader function.")]
	VertexLMRGBM = 3,
	/// <summary>
	///   <para>Forward rendering base pass.</para>
	/// </summary>
	ForwardBase = 4,
	/// <summary>
	///   <para>Forward rendering additive pixel light pass.</para>
	/// </summary>
	ForwardAdd = 5,
	/// <summary>
	///   <para>Legacy deferred lighting (light pre-pass) base pass.</para>
	/// </summary>
	LightPrePassBase = 6,
	/// <summary>
	///   <para>Legacy deferred lighting (light pre-pass) final pass.</para>
	/// </summary>
	LightPrePassFinal = 7,
	/// <summary>
	///   <para>Shadow caster &amp; depth texure shader pass.</para>
	/// </summary>
	ShadowCaster = 8,
	/// <summary>
	///   <para>Deferred Shading shader pass.</para>
	/// </summary>
	Deferred = 10,
	/// <summary>
	///   <para>Shader pass used to generate the albedo and emissive values used as input to lightmapping.</para>
	/// </summary>
	Meta = 11,
	/// <summary>
	///   <para>Motion vector render pass.</para>
	/// </summary>
	MotionVectors = 12
}
