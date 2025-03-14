using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

/// <summary>
///   <para>The Render Settings contain values for a range of visual elements in your scene, like fog and ambient light.</para>
/// </summary>
[NativeHeader("Runtime/Camera/RenderSettings.h")]
[StaticAccessor("GetRenderSettings()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/Camera/RenderSettings.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
public sealed class RenderSettings : Object
{
	[Obsolete("Use RenderSettings.ambientIntensity instead (UnityUpgradable) -> ambientIntensity", false)]
	public static float ambientSkyboxAmount
	{
		get
		{
			return ambientIntensity;
		}
		set
		{
			ambientIntensity = value;
		}
	}

	/// <summary>
	///   <para>Is fog enabled?</para>
	/// </summary>
	[NativeProperty("UseFog")]
	public static extern bool fog
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The starting distance of linear fog.</para>
	/// </summary>
	[NativeProperty("LinearFogStart")]
	public static extern float fogStartDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The ending distance of linear fog.</para>
	/// </summary>
	[NativeProperty("LinearFogEnd")]
	public static extern float fogEndDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Fog mode to use.</para>
	/// </summary>
	public static extern FogMode fogMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The color of the fog.</para>
	/// </summary>
	public static Color fogColor
	{
		get
		{
			get_fogColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_fogColor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The density of the exponential fog.</para>
	/// </summary>
	public static extern float fogDensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Ambient lighting mode.</para>
	/// </summary>
	public static extern AmbientMode ambientMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Ambient lighting coming from above.</para>
	/// </summary>
	public static Color ambientSkyColor
	{
		get
		{
			get_ambientSkyColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_ambientSkyColor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Ambient lighting coming from the sides.</para>
	/// </summary>
	public static Color ambientEquatorColor
	{
		get
		{
			get_ambientEquatorColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_ambientEquatorColor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Ambient lighting coming from below.</para>
	/// </summary>
	public static Color ambientGroundColor
	{
		get
		{
			get_ambientGroundColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_ambientGroundColor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>How much the light from the Ambient Source affects the scene.</para>
	/// </summary>
	public static extern float ambientIntensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Flat ambient lighting color.</para>
	/// </summary>
	[NativeProperty("AmbientSkyColor")]
	public static Color ambientLight
	{
		get
		{
			get_ambientLight_Injected(out var ret);
			return ret;
		}
		set
		{
			set_ambientLight_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The color used for the sun shadows in the Subtractive lightmode.</para>
	/// </summary>
	public static Color subtractiveShadowColor
	{
		get
		{
			get_subtractiveShadowColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_subtractiveShadowColor_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The global skybox to use.</para>
	/// </summary>
	[NativeProperty("SkyboxMaterial")]
	public static extern Material skybox
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The light used by the procedural skybox.</para>
	/// </summary>
	public static extern Light sun
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Custom or skybox ambient lighting data.</para>
	/// </summary>
	public static SphericalHarmonicsL2 ambientProbe
	{
		get
		{
			get_ambientProbe_Injected(out var ret);
			return ret;
		}
		set
		{
			set_ambientProbe_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Custom specular reflection cubemap.</para>
	/// </summary>
	public static extern Cubemap customReflection
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>How much the skybox / custom cubemap reflection affects the scene.</para>
	/// </summary>
	public static extern float reflectionIntensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The number of times a reflection includes other reflections.</para>
	/// </summary>
	public static extern int reflectionBounces
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Default reflection mode.</para>
	/// </summary>
	public static extern DefaultReflectionMode defaultReflectionMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Cubemap resolution for default reflection.</para>
	/// </summary>
	public static extern int defaultReflectionResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Size of the Light halos.</para>
	/// </summary>
	public static extern float haloStrength
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The intensity of all flares in the scene.</para>
	/// </summary>
	public static extern float flareStrength
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The fade speed of all flares in the scene.</para>
	/// </summary>
	public static extern float flareFadeSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	private RenderSettings()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetRenderSettings")]
	internal static extern Object GetRenderSettings();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("RenderSettingsScripting", StaticAccessorType.DoubleColon)]
	internal static extern void Reset();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_fogColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_fogColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_ambientSkyColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_ambientSkyColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_ambientEquatorColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_ambientEquatorColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_ambientGroundColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_ambientGroundColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_ambientLight_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_ambientLight_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_subtractiveShadowColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_subtractiveShadowColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_ambientProbe_Injected(out SphericalHarmonicsL2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_ambientProbe_Injected(ref SphericalHarmonicsL2 value);
}
