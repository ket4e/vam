using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

/// <summary>
///   <para>Constrains the orientation of an object relative to the position of one or more source objects, such that the object is facing the average position of the sources.</para>
/// </summary>
[NativeHeader("Runtime/Animation/Constraints/Constraint.bindings.h")]
[RequireComponent(typeof(Transform))]
[UsedByNativeCode]
[NativeHeader("Runtime/Animation/Constraints/AimConstraint.h")]
public sealed class AimConstraint : Behaviour, IConstraint, IConstraintInternal
{
	/// <summary>
	///   <para>Specifies how the world up vector used by the aim constraint is defined.</para>
	/// </summary>
	public enum WorldUpType
	{
		/// <summary>
		///   <para>Uses and defines the world up vector as the Unity scene up vector (the Y axis).</para>
		/// </summary>
		SceneUp,
		/// <summary>
		///   <para>Uses and defines the world up vector as a vector from the constrained object, in the direction of the up object.</para>
		/// </summary>
		ObjectUp,
		/// <summary>
		///   <para>Uses and defines the world up vector as relative to the local space of the object.</para>
		/// </summary>
		ObjectRotationUp,
		/// <summary>
		///   <para>Uses and defines the world up vector as a vector specified by the user.</para>
		/// </summary>
		Vector,
		/// <summary>
		///   <para>Neither defines nor uses a world up vector.</para>
		/// </summary>
		None
	}

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
	///   <para>Locks the offset and rotation at rest.</para>
	/// </summary>
	public extern bool locked
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
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
	///   <para>Represents an offset from the constrained orientation.</para>
	/// </summary>
	public Vector3 rotationOffset
	{
		get
		{
			get_rotationOffset_Injected(out var ret);
			return ret;
		}
		set
		{
			set_rotationOffset_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The axes affected by the AimConstraint.</para>
	/// </summary>
	public extern Axis rotationAxis
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The axis towards which the constrained object orients.</para>
	/// </summary>
	public Vector3 aimVector
	{
		get
		{
			get_aimVector_Injected(out var ret);
			return ret;
		}
		set
		{
			set_aimVector_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The up vector.</para>
	/// </summary>
	public Vector3 upVector
	{
		get
		{
			get_upVector_Injected(out var ret);
			return ret;
		}
		set
		{
			set_upVector_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The world up Vector used when the world up type is AimConstraint.WorldUpType.Vector or AimConstraint.WorldUpType.ObjectRotationUp.</para>
	/// </summary>
	public Vector3 worldUpVector
	{
		get
		{
			get_worldUpVector_Injected(out var ret);
			return ret;
		}
		set
		{
			set_worldUpVector_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The world up object, used to calculate the world up vector when the world up Type is AimConstraint.WorldUpType.ObjectUp or AimConstraint.WorldUpType.ObjectRotationUp.</para>
	/// </summary>
	public extern Transform worldUpObject
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The type of the world up vector.</para>
	/// </summary>
	public extern WorldUpType worldUpType
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

	private AimConstraint()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Create([Writable] AimConstraint self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ConstraintBindings::GetSourceCount")]
	private static extern int GetSourceCountInternal(AimConstraint self);

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
	private static extern void SetSourcesInternal(AimConstraint self, List<ConstraintSource> sources);

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
			throw new InvalidOperationException("The AimConstraint component has no sources.");
		}
		if (index < 0 || index >= sourceCount)
		{
			throw new ArgumentOutOfRangeException("index", $"Constraint source index {index} is out of bounds (0-{sourceCount}).");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_rotationAtRest_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_rotationAtRest_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_rotationOffset_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_rotationOffset_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_aimVector_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_aimVector_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_upVector_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_upVector_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_worldUpVector_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_worldUpVector_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int AddSource_Injected(ref ConstraintSource source);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);
}
