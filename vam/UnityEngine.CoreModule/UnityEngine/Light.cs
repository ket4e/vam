using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Script interface for.</para>
/// </summary>
[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Camera/Light.h")]
public sealed class Light : Behaviour
{
	private int m_BakedIndex;

	/// <summary>
	///   <para>How this light casts shadows</para>
	/// </summary>
	public extern LightShadows shadows
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Strength of light's shadows.</para>
	/// </summary>
	public extern float shadowStrength
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The resolution of the shadow map.</para>
	/// </summary>
	public extern LightShadowResolution shadowResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("Shadow softness is removed in Unity 5.0+")]
	public extern float shadowSoftness
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("Shadow softness is removed in Unity 5.0+")]
	public extern float shadowSoftnessFade
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The size of a directional light's cookie.</para>
	/// </summary>
	public extern float cookieSize
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The cookie texture projected by the light.</para>
	/// </summary>
	public extern Texture cookie
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>How to render the light.</para>
	/// </summary>
	public extern LightRenderMode renderMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("bakedIndex has been removed please use bakingOutput.isBaked instead.")]
	public int bakedIndex
	{
		get
		{
			return m_BakedIndex;
		}
		set
		{
			m_BakedIndex = value;
		}
	}

	/// <summary>
	///   <para>Number of command buffers set up on this light (Read Only).</para>
	/// </summary>
	public extern int commandBufferCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern int pixelLightCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("light.shadowConstantBias was removed, use light.shadowBias", true)]
	public float shadowConstantBias
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	[Obsolete("light.shadowObjectSizeBias was removed, use light.shadowBias", true)]
	public float shadowObjectSizeBias
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	[Obsolete("light.attenuate was removed; all lights always attenuate now", true)]
	public bool attenuate
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>The type of the light.</para>
	/// </summary>
	[NativeProperty("LightType")]
	public extern LightType type
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The angle of the light's spotlight cone in degrees.</para>
	/// </summary>
	public extern float spotAngle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The color of the light.</para>
	/// </summary>
	public Color color
	{
		get
		{
			get_color_Injected(out var ret);
			return ret;
		}
		set
		{
			set_color_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>
	///     The color temperature of the light.
	///     Correlated Color Temperature (abbreviated as CCT) is multiplied with the color filter when calculating the final color of a light source. The color temperature of the electromagnetic radiation emitted from an ideal black body is defined as its surface temperature in Kelvin. White is 6500K according to the D65 standard. Candle light is 1800K.
	///     If you want to use lightsUseCCT, lightsUseLinearIntensity has to be enabled to ensure physically correct output.
	///     See Also: GraphicsSettings.lightsUseLinearIntensity, GraphicsSettings.lightsUseCCT.
	///   </para>
	/// </summary>
	public extern float colorTemperature
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The Intensity of a light is multiplied with the Light color.</para>
	/// </summary>
	public extern float intensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The multiplier that defines the strength of the bounce lighting.</para>
	/// </summary>
	public extern float bounceIntensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The custom resolution of the shadow map.</para>
	/// </summary>
	public extern int shadowCustomResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Shadow mapping constant bias.</para>
	/// </summary>
	public extern float shadowBias
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Shadow mapping normal-based bias.</para>
	/// </summary>
	public extern float shadowNormalBias
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Near plane value to use for shadow frustums.</para>
	/// </summary>
	public extern float shadowNearPlane
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The range of the light.</para>
	/// </summary>
	public extern float range
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The to use for this light.</para>
	/// </summary>
	public extern Flare flare
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>This property describes the output of the last Global Illumination bake.</para>
	/// </summary>
	public LightBakingOutput bakingOutput
	{
		get
		{
			get_bakingOutput_Injected(out var ret);
			return ret;
		}
		set
		{
			set_bakingOutput_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>This is used to light certain objects in the scene selectively.</para>
	/// </summary>
	public extern int cullingMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetFalloffTable(float[] input);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SetAllLightsFalloffToInverseSquared();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SetAllLightsFalloffToUnityLegacy();

	/// <summary>
	///   <para>Sets a light dirty to notify the light baking backends to update their internal light representation.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetLightDirty();

	/// <summary>
	///   <para>Add a command buffer to be executed at a specified place.</para>
	/// </summary>
	/// <param name="evt">When to execute the command buffer during rendering.</param>
	/// <param name="buffer">The buffer to execute.</param>
	/// <param name="shadowPassMask">A mask specifying which shadow passes to execute the buffer for.</param>
	public void AddCommandBuffer(LightEvent evt, CommandBuffer buffer)
	{
		AddCommandBuffer(evt, buffer, ShadowMapPass.All);
	}

	/// <summary>
	///   <para>Add a command buffer to be executed at a specified place.</para>
	/// </summary>
	/// <param name="evt">When to execute the command buffer during rendering.</param>
	/// <param name="buffer">The buffer to execute.</param>
	/// <param name="shadowPassMask">A mask specifying which shadow passes to execute the buffer for.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void AddCommandBuffer(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask);

	/// <summary>
	///   <para>Adds a command buffer to the GPU's async compute queues and executes that command buffer when graphics processing reaches a given point.</para>
	/// </summary>
	/// <param name="evt">The point during the graphics processing at which this command buffer should commence on the GPU.</param>
	/// <param name="buffer">The buffer to execute.</param>
	/// <param name="queueType">The desired async compute queue type to execute the buffer on.</param>
	/// <param name="shadowPassMask">A mask specifying which shadow passes to execute the buffer for.</param>
	public void AddCommandBufferAsync(LightEvent evt, CommandBuffer buffer, ComputeQueueType queueType)
	{
		AddCommandBufferAsync(evt, buffer, ShadowMapPass.All, queueType);
	}

	/// <summary>
	///   <para>Adds a command buffer to the GPU's async compute queues and executes that command buffer when graphics processing reaches a given point.</para>
	/// </summary>
	/// <param name="evt">The point during the graphics processing at which this command buffer should commence on the GPU.</param>
	/// <param name="buffer">The buffer to execute.</param>
	/// <param name="queueType">The desired async compute queue type to execute the buffer on.</param>
	/// <param name="shadowPassMask">A mask specifying which shadow passes to execute the buffer for.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void AddCommandBufferAsync(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask, ComputeQueueType queueType);

	/// <summary>
	///   <para>Remove command buffer from execution at a specified place.</para>
	/// </summary>
	/// <param name="evt">When to execute the command buffer during rendering.</param>
	/// <param name="buffer">The buffer to execute.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void RemoveCommandBuffer(LightEvent evt, CommandBuffer buffer);

	/// <summary>
	///   <para>Remove command buffers from execution at a specified place.</para>
	/// </summary>
	/// <param name="evt">When to execute the command buffer during rendering.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void RemoveCommandBuffers(LightEvent evt);

	/// <summary>
	///   <para>Remove all command buffers set on this light.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void RemoveAllCommandBuffers();

	/// <summary>
	///   <para>Get command buffers to be executed at a specified place.</para>
	/// </summary>
	/// <param name="evt">When to execute the command buffer during rendering.</param>
	/// <returns>
	///   <para>Array of command buffers.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern CommandBuffer[] GetCommandBuffers(LightEvent evt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern Light[] GetLights(LightType type, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_color_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_color_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_bakingOutput_Injected(out LightBakingOutput ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_bakingOutput_Injected(ref LightBakingOutput value);
}
