using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>GPU data buffer, mostly for use with compute shaders.</para>
/// </summary>
[UsedByNativeCode]
public sealed class ComputeBuffer : IDisposable
{
	internal IntPtr m_Ptr;

	/// <summary>
	///   <para>Number of elements in the buffer (Read Only).</para>
	/// </summary>
	public extern int count
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Size of one element in the buffer (Read Only).</para>
	/// </summary>
	public extern int stride
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Create a Compute Buffer.</para>
	/// </summary>
	/// <param name="count">Number of elements in the buffer.</param>
	/// <param name="stride">Size of one element in the buffer. Has to match size of buffer type in the shader. See for cross-platform compatibility information.</param>
	/// <param name="type">Type of the buffer, default is ComputeBufferType.Default (structured buffer).</param>
	public ComputeBuffer(int count, int stride)
		: this(count, stride, ComputeBufferType.Default, 3)
	{
	}

	/// <summary>
	///   <para>Create a Compute Buffer.</para>
	/// </summary>
	/// <param name="count">Number of elements in the buffer.</param>
	/// <param name="stride">Size of one element in the buffer. Has to match size of buffer type in the shader. See for cross-platform compatibility information.</param>
	/// <param name="type">Type of the buffer, default is ComputeBufferType.Default (structured buffer).</param>
	public ComputeBuffer(int count, int stride, ComputeBufferType type)
		: this(count, stride, type, 3)
	{
	}

	internal ComputeBuffer(int count, int stride, ComputeBufferType type, int stackDepth)
	{
		if (count <= 0)
		{
			throw new ArgumentException("Attempting to create a zero length compute buffer", "count");
		}
		if (stride < 0)
		{
			throw new ArgumentException("Attempting to create a compute buffer with a negative stride", "stride");
		}
		m_Ptr = IntPtr.Zero;
		InitBuffer(this, count, stride, type);
	}

	~ComputeBuffer()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			DestroyBuffer(this);
		}
		else if (m_Ptr != IntPtr.Zero)
		{
			Debug.LogWarning("GarbageCollector disposing of ComputeBuffer. Please use ComputeBuffer.Release() or .Dispose() to manually release the buffer.");
		}
		m_Ptr = IntPtr.Zero;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void InitBuffer(ComputeBuffer buf, int count, int stride, ComputeBufferType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void DestroyBuffer(ComputeBuffer buf);

	/// <summary>
	///   <para>Release a Compute Buffer.</para>
	/// </summary>
	public void Release()
	{
		Dispose();
	}

	/// <summary>
	///   <para>Returns true if this compute buffer is valid and false otherwise.</para>
	/// </summary>
	public bool IsValid()
	{
		return m_Ptr != IntPtr.Zero;
	}

	/// <summary>
	///   <para>Set the buffer with values from an array.</para>
	/// </summary>
	/// <param name="data">Array of values to fill the buffer.</param>
	[SecuritySafeCritical]
	public void SetData(Array data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		InternalSetData(data, 0, 0, data.Length, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[SecuritySafeCritical]
	public void SetData<T>(List<T> data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), 0, 0, NoAllocHelpers.SafeLength(data), Marshal.SizeOf(typeof(T)));
	}

	[SecuritySafeCritical]
	public unsafe void SetData<T>(NativeArray<T> data) where T : struct
	{
		InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr(), 0, 0, data.Length, UnsafeUtility.SizeOf<T>());
	}

	/// <summary>
	///   <para>Partial copy of data values from an array into the buffer.</para>
	/// </summary>
	/// <param name="data">Array of values to fill the buffer.</param>
	/// <param name="managedBufferStartIndex">The first element index in data to copy to the compute buffer.</param>
	/// <param name="computeBufferStartIndex">The first element index in compute buffer to receive the data.</param>
	/// <param name="count">The number of elements to copy.</param>
	[SecuritySafeCritical]
	public void SetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (managedBufferStartIndex:{managedBufferStartIndex} computeBufferStartIndex:{computeBufferStartIndex} count:{count})");
		}
		InternalSetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[SecuritySafeCritical]
	public void SetData<T>(List<T> data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Count)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (managedBufferStartIndex:{managedBufferStartIndex} computeBufferStartIndex:{computeBufferStartIndex} count:{count})");
		}
		InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(typeof(T)));
	}

	[SecuritySafeCritical]
	public unsafe void SetData<T>(NativeArray<T> data, int nativeBufferStartIndex, int computeBufferStartIndex, int count) where T : struct
	{
		if (nativeBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || nativeBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (nativeBufferStartIndex:{nativeBufferStartIndex} computeBufferStartIndex:{computeBufferStartIndex} count:{count})");
		}
		InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr(), nativeBufferStartIndex, computeBufferStartIndex, count, UnsafeUtility.SizeOf<T>());
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SecurityCritical]
	[GeneratedByOldBindingsGenerator]
	private extern void InternalSetNativeData(IntPtr data, int nativeBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SecurityCritical]
	[GeneratedByOldBindingsGenerator]
	private extern void InternalSetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

	/// <summary>
	///   <para>Read data values from the buffer into an array.</para>
	/// </summary>
	/// <param name="data">An array to receive the data.</param>
	[SecurityCritical]
	public void GetData(Array data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		InternalGetData(data, 0, 0, data.Length, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	/// <summary>
	///   <para>Partial read of data values from the buffer into an array.</para>
	/// </summary>
	/// <param name="data">An array to receive the data.</param>
	/// <param name="managedBufferStartIndex">The first element index in data where retrieved elements are copied.</param>
	/// <param name="computeBufferStartIndex">The first element index of the compute buffer from which elements are read.</param>
	/// <param name="count">The number of elements to retrieve.</param>
	[SecurityCritical]
	public void GetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count argument (managedBufferStartIndex:{managedBufferStartIndex} computeBufferStartIndex:{computeBufferStartIndex} count:{count})");
		}
		InternalGetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SecurityCritical]
	[GeneratedByOldBindingsGenerator]
	private extern void InternalGetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

	/// <summary>
	///   <para>Sets counter value of append/consume buffer.</para>
	/// </summary>
	/// <param name="counterValue">Value of the append/consume counter.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetCounterValue(uint counterValue);

	/// <summary>
	///   <para>Copy counter value of append/consume buffer into another buffer.</para>
	/// </summary>
	/// <param name="src">Append/consume buffer to copy the counter from.</param>
	/// <param name="dst">A buffer to copy the counter to.</param>
	/// <param name="dstOffsetBytes">Target byte offset in dst.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes);

	/// <summary>
	///   <para>Retrieve a native (underlying graphics API) pointer to the buffer.</para>
	/// </summary>
	/// <returns>
	///   <para>Pointer to the underlying graphics API buffer.</para>
	/// </returns>
	public IntPtr GetNativeBufferPtr()
	{
		INTERNAL_CALL_GetNativeBufferPtr(this, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetNativeBufferPtr(ComputeBuffer self, out IntPtr value);
}
