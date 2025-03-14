using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Jobs;

/// <summary>
///   <para>TransformAccessArray.</para>
/// </summary>
[NativeType(Header = "Runtime/Transform/ScriptBindings/TransformAccess.bindings.h", CodegenOptions = CodegenOptions.Custom)]
public struct TransformAccessArray : IDisposable
{
	private IntPtr m_TransformArray;

	/// <summary>
	///   <para>isCreated.</para>
	/// </summary>
	public bool isCreated => m_TransformArray != IntPtr.Zero;

	public Transform this[int index]
	{
		get
		{
			return GetTransform(m_TransformArray, index);
		}
		set
		{
			SetTransform(m_TransformArray, index, value);
		}
	}

	/// <summary>
	///   <para>Returns array capacity.</para>
	/// </summary>
	public int capacity
	{
		get
		{
			return GetCapacity(m_TransformArray);
		}
		set
		{
			SetCapacity(m_TransformArray, value);
		}
	}

	/// <summary>
	///   <para>Length.</para>
	/// </summary>
	public int length => GetLength(m_TransformArray);

	/// <summary>
	///   <para>Constructor.</para>
	/// </summary>
	/// <param name="transforms">Transforms.</param>
	/// <param name="desiredJobCount">Desired job count.</param>
	/// <param name="capacity">Capacity.</param>
	public TransformAccessArray(Transform[] transforms, int desiredJobCount = -1)
	{
		Allocate(transforms.Length, desiredJobCount, out this);
		SetTransforms(m_TransformArray, transforms);
	}

	/// <summary>
	///   <para>Constructor.</para>
	/// </summary>
	/// <param name="transforms">Transforms.</param>
	/// <param name="desiredJobCount">Desired job count.</param>
	/// <param name="capacity">Capacity.</param>
	public TransformAccessArray(int capacity, int desiredJobCount = -1)
	{
		Allocate(capacity, desiredJobCount, out this);
	}

	public static void Allocate(int capacity, int desiredJobCount, out TransformAccessArray array)
	{
		array.m_TransformArray = Create(capacity, desiredJobCount);
	}

	/// <summary>
	///   <para>Dispose.</para>
	/// </summary>
	public void Dispose()
	{
		DestroyTransformAccessArray(m_TransformArray);
		m_TransformArray = IntPtr.Zero;
	}

	internal IntPtr GetTransformAccessArrayForSchedule()
	{
		return m_TransformArray;
	}

	/// <summary>
	///   <para>Add.</para>
	/// </summary>
	/// <param name="transform">Transform.</param>
	public void Add(Transform transform)
	{
		Add(m_TransformArray, transform);
	}

	/// <summary>
	///   <para>Remove item at index.</para>
	/// </summary>
	/// <param name="index">Index.</param>
	public void RemoveAtSwapBack(int index)
	{
		RemoveAtSwapBack(m_TransformArray, index);
	}

	/// <summary>
	///   <para>Set transforms.</para>
	/// </summary>
	/// <param name="transforms">Transforms.</param>
	public void SetTransforms(Transform[] transforms)
	{
		SetTransforms(m_TransformArray, transforms);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::Create", IsFreeFunction = true)]
	private static extern IntPtr Create(int capacity, int desiredJobCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "DestroyTransformAccessArray", IsFreeFunction = true)]
	private static extern void DestroyTransformAccessArray(IntPtr transformArray);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::SetTransforms", IsFreeFunction = true)]
	private static extern void SetTransforms(IntPtr transformArrayIntPtr, Transform[] transforms);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::AddTransform", IsFreeFunction = true)]
	private static extern void Add(IntPtr transformArrayIntPtr, Transform transform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::RemoveAtSwapBack", IsFreeFunction = true)]
	private static extern void RemoveAtSwapBack(IntPtr transformArrayIntPtr, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::GetSortedTransformAccess", IsThreadSafe = true, IsFreeFunction = true)]
	internal static extern IntPtr GetSortedTransformAccess(IntPtr transformArrayIntPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::GetSortedToUserIndex", IsThreadSafe = true, IsFreeFunction = true)]
	internal static extern IntPtr GetSortedToUserIndex(IntPtr transformArrayIntPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::GetLength", IsFreeFunction = true)]
	internal static extern int GetLength(IntPtr transformArrayIntPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::GetCapacity", IsFreeFunction = true)]
	internal static extern int GetCapacity(IntPtr transformArrayIntPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::SetCapacity", IsFreeFunction = true)]
	internal static extern void SetCapacity(IntPtr transformArrayIntPtr, int capacity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::GetTransform", IsFreeFunction = true)]
	internal static extern Transform GetTransform(IntPtr transformArrayIntPtr, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TransformAccessArrayBindings::SetTransform", IsFreeFunction = true)]
	internal static extern void SetTransform(IntPtr transformArrayIntPtr, int index, Transform transform);
}
