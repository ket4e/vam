using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

/// <summary>
///   <para>Constrains the position of an object relative to the position of one or more source objects.</para>
/// </summary>
[NativeHeader("Runtime/Animation/Constraints/Constraint.bindings.h")]
[RequireComponent(typeof(Transform))]
[UsedByNativeCode]
[NativeHeader("Runtime/Animation/Constraints/PositionConstraint.h")]
public sealed class PositionConstraint : Behaviour, IConstraint, IConstraintInternal
{
	/// <summary>
	///   <para>The weight of the constraint component.</para>
	/// </summary>
	public extern float weight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The translation used when the sources have a total weight of 0.</para>
	/// </summary>
	public Vector3 translationAtRest
	{
		get
		{
			get_translationAtRest_Injected(out var ret);
			return ret;
		}
		set
		{
			set_translationAtRest_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The offset from the constrained position.</para>
	/// </summary>
	public Vector3 translationOffset
	{
		get
		{
			get_translationOffset_Injected(out var ret);
			return ret;
		}
		set
		{
			set_translationOffset_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The axes affected by the PositionConstraint.</para>
	/// </summary>
	public extern Axis translationAxis
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Activates or deactivates the constraint.</para>
	/// </summary>
	public extern bool constraintActive
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Locks the offset and position at rest.</para>
	/// </summary>
	public extern bool locked
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The number of sources set on the component (read-only).</para>
	/// </summary>
	public int sourceCount => GetSourceCountInternal(this);

	private PositionConstraint()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Create([Writable] PositionConstraint self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ConstraintBindings::GetSourceCount")]
	private static extern int GetSourceCountInternal(PositionConstraint self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
	public extern void GetSources([NotNull] List<ConstraintSource> sources);

	public void SetSources(List<ConstraintSource> sources)
	{
		if (sources == null)
		{
			throw new ArgumentNullException("sources");
		}
		SetSourcesInternal(this, sources);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ConstraintBindings::SetSources")]
	private static extern void SetSourcesInternal(PositionConstraint self, List<ConstraintSource> sources);

	/// <summary>
	///   <para>Adds a constraint source.</para>
	/// </summary>
	/// <param name="source">The source object and its weight.</param>
	/// <returns>
	///   <para>Returns the index of the added source.</para>
	/// </returns>
	public int AddSource(ConstraintSource source)
	{
		return AddSource_Injected(ref source);
	}

	/// <summary>
	///   <para>Removes a source from the component.</para>
	/// </summary>
	/// <param name="index">The index of the source to remove.</param>
	public void RemoveSource(int index)
	{
		ValidateSourceIndex(index);
		RemoveSourceInternal(index);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("RemoveSource")]
	private extern void RemoveSourceInternal(int index);

	/// <summary>
	///   <para>Gets a constraint source by index.</para>
	/// </summary>
	/// <param name="index">The index of the source.</param>
	/// <returns>
	///   <para>The source object and its weight.</para>
	/// </returns>
	public ConstraintSource GetSource(int index)
	{
		ValidateSourceIndex(index);
		return GetSourceInternal(index);
	}

	[NativeName("GetSource")]
	private ConstraintSource GetSourceInternal(int index)
	{
		GetSourceInternal_Injected(index, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Sets a source at a specified index.</para>
	/// </summary>
	/// <param name="index">The index of the source to set.</param>
	/// <param name="source">The source object and its weight.</param>
	public void SetSource(int index, ConstraintSource source)
	{
		ValidateSourceIndex(index);
		SetSourceInternal(index, source);
	}

	[NativeName("SetSource")]
	private void SetSourceInternal(int index, ConstraintSource source)
	{
		SetSourceInternal_Injected(index, ref source);
	}

	private void ValidateSourceIndex(int index)
	{
		if (sourceCount == 0)
		{
			throw new InvalidOperationException("The PositionConstraint component has no sources.");
		}
		if (index < 0 || index >= sourceCount)
		{
			throw new ArgumentOutOfRangeException("index", $"Constraint source index {index} is out of bounds (0-{sourceCount}).");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_translationAtRest_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_translationAtRest_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_translationOffset_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_translationOffset_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int AddSource_Injected(ref ConstraintSource source);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);
}
