using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

/// <summary>
///   <para>Constrains the orientation and translation of an object to one or more source objects. The constrained object behaves as if it is in the hierarchy of the sources.</para>
/// </summary>
[UsedByNativeCode]
[NativeHeader("Runtime/Animation/Constraints/Constraint.bindings.h")]
[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Animation/Constraints/ParentConstraint.h")]
public sealed class ParentConstraint : Behaviour, IConstraint, IConstraintInternal
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
	///   <para>Locks the offsets and position (translation and rotation) at rest.</para>
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

	/// <summary>
	///   <para>The position of the object in local space, used when the sources have a total weight of 0.</para>
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
	///   <para>The rotation used when the sources have a total weight of 0.</para>
	/// </summary>
	public Vector3 rotationAtRest
	{
		get
		{
			get_rotationAtRest_Injected(out var ret);
			return ret;
		}
		set
		{
			set_rotationAtRest_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The translation offsets from the constrained orientation.</para>
	/// </summary>
	public extern Vector3[] translationOffsets
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The rotation offsets from the constrained orientation.</para>
	/// </summary>
	public extern Vector3[] rotationOffsets
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The translation axes affected by the ParentConstraint.</para>
	/// </summary>
	public extern Axis translationAxis
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The rotation axes affected by the ParentConstraint.</para>
	/// </summary>
	public extern Axis rotationAxis
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	private ParentConstraint()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Create([Writable] ParentConstraint self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ConstraintBindings::GetSourceCount")]
	private static extern int GetSourceCountInternal(ParentConstraint self);

	/// <summary>
	///   <para>Gets the rotation offset associated with a source by index.</para>
	/// </summary>
	/// <param name="index">The index of the constraint source.</param>
	/// <returns>
	///   <para>The translation offset.</para>
	/// </returns>
	public Vector3 GetTranslationOffset(int index)
	{
		ValidateSourceIndex(index);
		return GetTranslationOffsetInternal(index);
	}

	/// <summary>
	///   <para>Sets the translation offset associated with a source by index.</para>
	/// </summary>
	/// <param name="index">The index of the constraint source.</param>
	/// <param name="value">The new translation offset.</param>
	public void SetTranslationOffset(int index, Vector3 value)
	{
		ValidateSourceIndex(index);
		SetTranslationOffsetInternal(index, value);
	}

	[NativeName("GetTranslationOffset")]
	private Vector3 GetTranslationOffsetInternal(int index)
	{
		GetTranslationOffsetInternal_Injected(index, out var ret);
		return ret;
	}

	[NativeName("SetTranslationOffset")]
	private void SetTranslationOffsetInternal(int index, Vector3 value)
	{
		SetTranslationOffsetInternal_Injected(index, ref value);
	}

	/// <summary>
	///   <para>Gets the rotation offset associated with a source by index.</para>
	/// </summary>
	/// <param name="index">The index of the constraint source.</param>
	/// <returns>
	///   <para>The rotation offset, as Euler angles.</para>
	/// </returns>
	public Vector3 GetRotationOffset(int index)
	{
		ValidateSourceIndex(index);
		return GetRotationOffsetInternal(index);
	}

	/// <summary>
	///   <para>Sets the rotation offset associated with a source by index.</para>
	/// </summary>
	/// <param name="index">The index of the constraint source.</param>
	/// <param name="value">The new rotation offset.</param>
	public void SetRotationOffset(int index, Vector3 value)
	{
		ValidateSourceIndex(index);
		SetRotationOffsetInternal(index, value);
	}

	[NativeName("GetRotationOffset")]
	private Vector3 GetRotationOffsetInternal(int index)
	{
		GetRotationOffsetInternal_Injected(index, out var ret);
		return ret;
	}

	[NativeName("SetRotationOffset")]
	private void SetRotationOffsetInternal(int index, Vector3 value)
	{
		SetRotationOffsetInternal_Injected(index, ref value);
	}

	private void ValidateSourceIndex(int index)
	{
		if (sourceCount == 0)
		{
			throw new InvalidOperationException("The ParentConstraint component has no sources.");
		}
		if (index < 0 || index >= sourceCount)
		{
			throw new ArgumentOutOfRangeException("index", $"Constraint source index {index} is out of bounds (0-{sourceCount}).");
		}
	}

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
	private static extern void SetSourcesInternal(ParentConstraint self, List<ConstraintSource> sources);

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

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_translationAtRest_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_translationAtRest_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_rotationAtRest_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_rotationAtRest_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetTranslationOffsetInternal_Injected(int index, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTranslationOffsetInternal_Injected(int index, ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetRotationOffsetInternal_Injected(int index, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetRotationOffsetInternal_Injected(int index, ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int AddSource_Injected(ref ConstraintSource source);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);
}
