using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

/// <summary>
///   <para>Script interface for.</para>
/// </summary>
public sealed class GraphicsSettings : Object
{
	/// <summary>
	///   <para>The RenderPipelineAsset that describes how the Scene should be rendered.</para>
	/// </summary>
	public static RenderPipelineAsset renderPipelineAsset
	{
		get
		{
			return INTERNAL_renderPipelineAsset as RenderPipelineAsset;
		}
		set
		{
			INTERNAL_renderPipelineAsset = value;
		}
	}

	private static extern ScriptableObject INTERNAL_renderPipelineAsset
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Transparent object sorting mode.</para>
	/// </summary>
	public static extern TransparencySortMode transparencySortMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>An axis that describes the direction along which the distances of objects are measured for the purpose of sorting.</para>
	/// </summary>
	public static Vector3 transparencySortAxis
	{
		get
		{
			INTERNAL_get_transparencySortAxis(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_transparencySortAxis(ref value);
		}
	}

	/// <summary>
	///   <para>If this is true, Light intensity is multiplied against linear color values. If it is false, gamma color values are used.</para>
	/// </summary>
	public static extern bool lightsUseLinearIntensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Whether to use a Light's color temperature when calculating the final color of that Light."</para>
	/// </summary>
	public static extern bool lightsUseColorTemperature
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	private GraphicsSettings()
	{
	}

	/// <summary>
	///   <para>Set built-in shader mode.</para>
	/// </summary>
	/// <param name="type">Built-in shader type to change.</param>
	/// <param name="mode">Mode to use for built-in shader.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetShaderMode(BuiltinShaderType type, BuiltinShaderMode mode);

	/// <summary>
	///   <para>Get built-in shader mode.</para>
	/// </summary>
	/// <param name="type">Built-in shader type to query.</param>
	/// <returns>
	///   <para>Mode used for built-in shader.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern BuiltinShaderMode GetShaderMode(BuiltinShaderType type);

	/// <summary>
	///   <para>Set custom shader to use instead of a built-in shader.</para>
	/// </summary>
	/// <param name="type">Built-in shader type to set custom shader to.</param>
	/// <param name="shader">The shader to use.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetCustomShader(BuiltinShaderType type, Shader shader);

	/// <summary>
	///   <para>Get custom shader used instead of a built-in shader.</para>
	/// </summary>
	/// <param name="type">Built-in shader type to query custom shader for.</param>
	/// <returns>
	///   <para>The shader used.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern Shader GetCustomShader(BuiltinShaderType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern Object GetGraphicsSettings();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_transparencySortAxis(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_set_transparencySortAxis(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool HasShaderDefineImpl(GraphicsTier tier, BuiltinShaderDefine defineHash);

	/// <summary>
	///   <para>Returns true if shader define was set when compiling shaders for current GraphicsTier.</para>
	/// </summary>
	/// <param name="tier"></param>
	/// <param name="defineHash"></param>
	public static bool HasShaderDefine(GraphicsTier tier, BuiltinShaderDefine defineHash)
	{
		return HasShaderDefineImpl(tier, defineHash);
	}

	/// <summary>
	///   <para>Returns true if shader define was set when compiling shaders for given tier.</para>
	/// </summary>
	/// <param name="defineHash"></param>
	public static bool HasShaderDefine(BuiltinShaderDefine defineHash)
	{
		return HasShaderDefine(Graphics.activeTier, defineHash);
	}
}
