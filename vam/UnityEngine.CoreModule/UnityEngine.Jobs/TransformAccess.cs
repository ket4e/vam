using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Jobs;

/// <summary>
///   <para>Position, rotation and scale of an object.</para>
/// </summary>
[NativeHeader("Runtime/Transform/ScriptBindings/TransformAccess.bindings.h")]
public struct TransformAccess
{
	private IntPtr hierarchy;

	private int index;

	/// <summary>
	///   <para>The position of the transform in world space.</para>
	/// </summary>
	public Vector3 position
	{
		get
		{
			GetPosition(ref this, out var p);
			return p;
		}
		set
		{
			SetPosition(ref this, ref value);
		}
	}

	/// <summary>
	///   <para>The rotation of the transform in world space stored as a Quaternion.</para>
	/// </summary>
	public Quaternion rotation
	{
		get
		{
			GetRotation(ref this, out var r);
			return r;
		}
		set
		{
			SetRotation(ref this, ref value);
		}
	}

	/// <summary>
	///   <para>The scale of the transform relative to the parent.</para>
	/// </summary>
	public Vector3 localPosition
	{
		get
		{
			GetLocalPosition(ref this, out var p);
			return p;
		}
		set
		{
			SetLocalPosition(ref this, ref value);
		}
	}

	/// <summary>
	///   <para>The rotation of the transform relative to the parent transform's rotation.</para>
	/// </summary>
	public Quaternion localRotation
	{
		get
		{
			GetLocalRotation(ref this, out var r);
			return r;
		}
		set
		{
			SetLocalRotation(ref this, ref value);
		}
	}

	/// <summary>
	///   <para>The scale of the transform relative to the parent.</para>
	/// </summary>
	public Vector3 localScale
	{
		get
		{
			GetLocalScale(ref this, out var r);
			return r;
		}
		set
		{
			SetLocalScale(ref this, ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessBindings::GetPosition", IsThreadSafe = true, IsFreeFunction = true)]
	private static extern void GetPosition(ref TransformAccess access, out Vector3 p);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessBindings::SetPosition", IsThreadSafe = true, IsFreeFunction = true)]
	private static extern void SetPosition(ref TransformAccess access, ref Vector3 p);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessBindings::GetRotation", IsThreadSafe = true, IsFreeFunction = true)]
	private static extern void GetRotation(ref TransformAccess access, out Quaternion r);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessBindings::SetRotation", IsThreadSafe = true, IsFreeFunction = true)]
	private static extern void SetRotation(ref TransformAccess access, ref Quaternion r);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessBindings::GetLocalPosition", IsThreadSafe = true, IsFreeFunction = true)]
	private static extern void GetLocalPosition(ref TransformAccess access, out Vector3 p);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessBindings::SetLocalPosition", IsThreadSafe = true, IsFreeFunction = true)]
	private static extern void SetLocalPosition(ref TransformAccess access, ref Vector3 p);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessBindings::GetLocalRotation", IsThreadSafe = true, IsFreeFunction = true)]
	private static extern void GetLocalRotation(ref TransformAccess access, out Quaternion r);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessBindings::SetLocalRotation", IsThreadSafe = true, IsFreeFunction = true)]
	private static extern void SetLocalRotation(ref TransformAccess access, ref Quaternion r);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessBindings::GetLocalScale", IsThreadSafe = true, IsFreeFunction = true)]
	private static extern void GetLocalScale(ref TransformAccess access, out Vector3 r);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessBindings::SetLocalScale", IsThreadSafe = true, IsFreeFunction = true)]
	private static extern void SetLocalScale(ref TransformAccess access, ref Vector3 r);
}
