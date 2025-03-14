using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>LODGroup lets you group multiple Renderers into LOD levels.</para>
/// </summary>
public sealed class LODGroup : Component
{
	/// <summary>
	///   <para>The local reference point against which the LOD distance is calculated.</para>
	/// </summary>
	public Vector3 localReferencePoint
	{
		get
		{
			INTERNAL_get_localReferencePoint(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_localReferencePoint(ref value);
		}
	}

	/// <summary>
	///   <para>The size of the LOD object in local space.</para>
	/// </summary>
	public extern float size
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The number of LOD levels.</para>
	/// </summary>
	public extern int lodCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The LOD fade mode used.</para>
	/// </summary>
	public extern LODFadeMode fadeMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Specify if the cross-fading should be animated by time. The animation duration is specified globally as crossFadeAnimationDuration.</para>
	/// </summary>
	public extern bool animateCrossFading
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Enable / Disable the LODGroup - Disabling will turn off all renderers.</para>
	/// </summary>
	public extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The cross-fading animation duration in seconds. ArgumentException will be thrown if it is set to zero or a negative value.</para>
	/// </summary>
	public static extern float crossFadeAnimationDuration
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_localReferencePoint(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_localReferencePoint(ref Vector3 value);

	/// <summary>
	///   <para>Recalculate the bounding region for the LODGroup (Relatively slow, do not call often).</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void RecalculateBounds();

	/// <summary>
	///   <para>Returns the array of LODs.</para>
	/// </summary>
	/// <returns>
	///   <para>The LOD array.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern LOD[] GetLODs();

	[Obsolete("Use SetLODs instead.")]
	public void SetLODS(LOD[] lods)
	{
		SetLODs(lods);
	}

	/// <summary>
	///   <para>Set the LODs for the LOD group. This will remove any existing LODs configured on the LODGroup.</para>
	/// </summary>
	/// <param name="lods">The LODs to use for this group.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetLODs(LOD[] lods);

	/// <summary>
	///   <para></para>
	/// </summary>
	/// <param name="index">The LOD level to use. Passing index &lt; 0 will return to standard LOD processing.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void ForceLOD(int index);
}
