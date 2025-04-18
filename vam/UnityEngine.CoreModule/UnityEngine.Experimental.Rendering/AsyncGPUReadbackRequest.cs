using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Represents an asynchronous request for a GPU resource.</para>
/// </summary>
[NativeHeader("Runtime/Shaders/ComputeShader.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/AsyncGPUReadbackManaged.h")]
[NativeHeader("Runtime/Graphics/Texture.h")]
public struct AsyncGPUReadbackRequest
{
	internal IntPtr m_Ptr;

	internal int m_Version;

	/// <summary>
	///   <para>Checks whether the request has been processed.</para>
	/// </summary>
	public bool done => IsDone();

	/// <summary>
	///   <para>This property is true if the request has encountered an error.</para>
	/// </summary>
	public bool hasError => HasError();

	/// <summary>
	///   <para>Number of layers in the current request.</para>
	/// </summary>
	public int layerCount => GetLayerCount();

	/// <summary>
	///   <para>Triggers an update of the request.</para>
	/// </summary>
	public void Update()
	{
		Update_Injected(ref this);
	}

	/// <summary>
	///   <para>Waits for completion of the request.</para>
	/// </summary>
	public void WaitForCompletion()
	{
		WaitForCompletion_Injected(ref this);
	}

	public unsafe NativeArray<T> GetData<T>(int layer = 0) where T : struct
	{
		if (!done || hasError)
		{
			throw new InvalidOperationException("Cannot access the data as it is not available");
		}
		if (layer < 0 || layer >= layerCount)
		{
			throw new ArgumentException($"Layer index is out of range {layer} / {layerCount}");
		}
		int num = UnsafeUtility.SizeOf<T>();
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)GetDataRaw(layer), GetLayerDataSize() / num, Allocator.None);
	}

	private bool IsDone()
	{
		return IsDone_Injected(ref this);
	}

	private bool HasError()
	{
		return HasError_Injected(ref this);
	}

	private int GetLayerCount()
	{
		return GetLayerCount_Injected(ref this);
	}

	private IntPtr GetDataRaw(int layer)
	{
		return GetDataRaw_Injected(ref this, layer);
	}

	private int GetLayerDataSize()
	{
		return GetLayerDataSize_Injected(ref this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Update_Injected(ref AsyncGPUReadbackRequest _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WaitForCompletion_Injected(ref AsyncGPUReadbackRequest _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsDone_Injected(ref AsyncGPUReadbackRequest _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasError_Injected(ref AsyncGPUReadbackRequest _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetLayerCount_Injected(ref AsyncGPUReadbackRequest _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetDataRaw_Injected(ref AsyncGPUReadbackRequest _unity_self, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetLayerDataSize_Injected(ref AsyncGPUReadbackRequest _unity_self);
}
