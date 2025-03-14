using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Stores light probes for the scene.</para>
/// </summary>
public sealed class LightProbes : Object
{
	/// <summary>
	///   <para>Positions of the baked light probes (Read Only).</para>
	/// </summary>
	public extern Vector3[] positions
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Coefficients of baked light probes.</para>
	/// </summary>
	public extern SphericalHarmonicsL2[] bakedProbes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The number of light probes (Read Only).</para>
	/// </summary>
	public extern int count
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The number of cells space is divided into (Read Only).</para>
	/// </summary>
	public extern int cellCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	[Obsolete("Use bakedProbes instead.", true)]
	public float[] coefficients
	{
		get
		{
			return new float[0];
		}
		set
		{
		}
	}

	private LightProbes()
	{
	}

	public static void GetInterpolatedProbe(Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe)
	{
		INTERNAL_CALL_GetInterpolatedProbe(ref position, renderer, out probe);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetInterpolatedProbe(ref Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe);

	/// <summary>
	///   <para>Calculate light probes and occlusion probes at the given world space positions.</para>
	/// </summary>
	/// <param name="positions">The array of world space positions used to evaluate the probes.</param>
	/// <param name="lightProbes">The array where the resulting light probes are written to.</param>
	/// <param name="occlusionProbes">The array where the resulting occlusion probes are written to.</param>
	public static void CalculateInterpolatedLightAndOcclusionProbes(Vector3[] positions, SphericalHarmonicsL2[] lightProbes, Vector4[] occlusionProbes)
	{
		if (positions == null)
		{
			throw new ArgumentNullException("positions");
		}
		if (lightProbes == null && occlusionProbes == null)
		{
			throw new ArgumentException("Argument lightProbes and occlusionProbes cannot both be null.");
		}
		if (lightProbes != null && lightProbes.Length < positions.Length)
		{
			throw new ArgumentException("lightProbes", "Argument lightProbes has less elements than positions");
		}
		if (occlusionProbes != null && occlusionProbes.Length < positions.Length)
		{
			throw new ArgumentException("occlusionProbes", "Argument occlusionProbes has less elements than positions");
		}
		Internal_CalculateInterpolatedLightAndOcclusionProbes(positions, positions.Length, lightProbes, occlusionProbes);
	}

	public static void CalculateInterpolatedLightAndOcclusionProbes(List<Vector3> positions, List<SphericalHarmonicsL2> lightProbes, List<Vector4> occlusionProbes)
	{
		if (positions == null)
		{
			throw new ArgumentNullException("positions");
		}
		if (lightProbes == null && occlusionProbes == null)
		{
			throw new ArgumentException("Argument lightProbes and occlusionProbes cannot both be null.");
		}
		if (lightProbes != null)
		{
			if (lightProbes.Capacity < positions.Count)
			{
				lightProbes.Capacity = positions.Count;
			}
			if (lightProbes.Count < positions.Count)
			{
				NoAllocHelpers.ResizeList(lightProbes, positions.Count);
			}
		}
		if (occlusionProbes != null)
		{
			if (occlusionProbes.Capacity < positions.Count)
			{
				occlusionProbes.Capacity = positions.Count;
			}
			if (occlusionProbes.Count < positions.Count)
			{
				NoAllocHelpers.ResizeList(occlusionProbes, positions.Count);
			}
		}
		Internal_CalculateInterpolatedLightAndOcclusionProbes(NoAllocHelpers.ExtractArrayFromListT(positions), positions.Count, NoAllocHelpers.ExtractArrayFromListT(lightProbes), NoAllocHelpers.ExtractArrayFromListT(occlusionProbes));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void Internal_CalculateInterpolatedLightAndOcclusionProbes(Vector3[] positions, int count, SphericalHarmonicsL2[] lightProbes, Vector4[] occlusionProbes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern bool AreLightProbesAllowed(Renderer renderer);

	[Obsolete("Use GetInterpolatedProbe instead.", true)]
	public void GetInterpolatedLightProbe(Vector3 position, Renderer renderer, float[] coefficients)
	{
	}
}
