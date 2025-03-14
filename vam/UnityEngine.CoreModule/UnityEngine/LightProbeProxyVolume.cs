using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The Light Probe Proxy Volume component offers the possibility to use higher resolution lighting for large non-static GameObjects.</para>
/// </summary>
public sealed class LightProbeProxyVolume : Behaviour
{
	/// <summary>
	///   <para>The resolution mode for generating a grid of interpolated Light Probes.</para>
	/// </summary>
	public enum ResolutionMode
	{
		/// <summary>
		///   <para>The automatic mode uses a number of interpolated Light Probes per unit area, and uses the bounding volume size to compute the resolution. The final resolution value is a power of 2.</para>
		/// </summary>
		Automatic,
		/// <summary>
		///   <para>The custom mode allows you to specify the 3D grid resolution.</para>
		/// </summary>
		Custom
	}

	/// <summary>
	///   <para>The bounding box mode for generating a grid of interpolated Light Probes.</para>
	/// </summary>
	public enum BoundingBoxMode
	{
		/// <summary>
		///   <para>The bounding box encloses the current Renderer and all the relevant Renderers down the hierarchy, in local space.</para>
		/// </summary>
		AutomaticLocal,
		/// <summary>
		///   <para>The bounding box encloses the current Renderer and all the relevant Renderers down the hierarchy, in world space.</para>
		/// </summary>
		AutomaticWorld,
		/// <summary>
		///   <para>A custom local-space bounding box is used. The user is able to edit the bounding box.</para>
		/// </summary>
		Custom
	}

	/// <summary>
	///   <para>The mode in which the interpolated Light Probe positions are generated.</para>
	/// </summary>
	public enum ProbePositionMode
	{
		/// <summary>
		///   <para>Divide the volume in cells based on resolution, and generate interpolated Light Probes positions in the corner/edge of the cells.</para>
		/// </summary>
		CellCorner,
		/// <summary>
		///   <para>Divide the volume in cells based on resolution, and generate interpolated Light Probe positions in the center of the cells.</para>
		/// </summary>
		CellCenter
	}

	/// <summary>
	///   <para>An enum describing the way a Light Probe Proxy Volume refreshes in the Player.</para>
	/// </summary>
	public enum RefreshMode
	{
		/// <summary>
		///   <para>Automatically detects updates in Light Probes and triggers an update of the Light Probe volume.</para>
		/// </summary>
		Automatic,
		/// <summary>
		///   <para>Causes Unity to update the Light Probe Proxy Volume every frame.</para>
		/// </summary>
		EveryFrame,
		/// <summary>
		///   <para>Use this option to indicate that the Light Probe Proxy Volume is never to be automatically updated by Unity.</para>
		/// </summary>
		ViaScripting
	}

	/// <summary>
	///   <para>The world-space bounding box in which the 3D grid of interpolated Light Probes is generated.</para>
	/// </summary>
	public Bounds boundsGlobal
	{
		get
		{
			INTERNAL_get_boundsGlobal(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>The size of the bounding box in which the 3D grid of interpolated Light Probes is generated.</para>
	/// </summary>
	public Vector3 sizeCustom
	{
		get
		{
			INTERNAL_get_sizeCustom(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_sizeCustom(ref value);
		}
	}

	/// <summary>
	///   <para>The local-space origin of the bounding box in which the 3D grid of interpolated Light Probes is generated.</para>
	/// </summary>
	public Vector3 originCustom
	{
		get
		{
			INTERNAL_get_originCustom(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_originCustom(ref value);
		}
	}

	/// <summary>
	///   <para>The bounding box mode for generating the 3D grid of interpolated Light Probes.</para>
	/// </summary>
	public extern BoundingBoxMode boundingBoxMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The resolution mode for generating the grid of interpolated Light Probes.</para>
	/// </summary>
	public extern ResolutionMode resolutionMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The mode in which the interpolated Light Probe positions are generated.</para>
	/// </summary>
	public extern ProbePositionMode probePositionMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Sets the way the Light Probe Proxy Volume refreshes.</para>
	/// </summary>
	public extern RefreshMode refreshMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Interpolated Light Probe density.</para>
	/// </summary>
	public extern float probeDensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The 3D grid resolution on the z-axis.</para>
	/// </summary>
	public extern int gridResolutionX
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The 3D grid resolution on the y-axis.</para>
	/// </summary>
	public extern int gridResolutionY
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The 3D grid resolution on the z-axis.</para>
	/// </summary>
	public extern int gridResolutionZ
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Checks if Light Probe Proxy Volumes are supported.</para>
	/// </summary>
	public static extern bool isFeatureSupported
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_boundsGlobal(out Bounds value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_sizeCustom(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_sizeCustom(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_originCustom(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_originCustom(ref Vector3 value);

	/// <summary>
	///   <para>Triggers an update of the Light Probe Proxy Volume.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Update();
}
