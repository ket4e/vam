using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Internal;

namespace Unity.Collections;

/// <summary>
///   <para>A NativeArray exposes a buffer of native memory to managed code, making it possible to share data between managed and native.</para>
/// </summary>
[NativeContainerSupportsMinMaxWriteRestriction]
[NativeContainer]
[NativeContainerSupportsDeallocateOnJobCompletion]
[NativeContainerSupportsDeferredConvertListToArray]
[DebuggerDisplay("Length = {Length}")]
[DebuggerTypeProxy(typeof(NativeArrayDebugView<>))]
public struct NativeArray<T> : IDisposable, IEnumerable<T>, IEnumerable where T : struct
{
	[ExcludeFromDocs]
	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private NativeArray<T> m_Array;

		private int m_Index;

		object IEnumerator.Current => Current;

		public T Current => m_Array[m_Index];

		public Enumerator(ref NativeArray<T> array)
		{
			m_Array = array;
			m_Index = -1;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			m_Index++;
			return m_Index < m_Array.Length;
		}

		public void Reset()
		{
			m_Index = -1;
		}
	}

	[NativeDisableUnsafePtrRestriction]
	internal unsafe void* m_Buffer;

	internal int m_Length;

	internal Allocator m_AllocatorLabel;

	public int Length => m_Length;

	public unsafe T this[int index]
	{
		get
		{
			return UnsafeUtility.ReadArrayElement<T>(m_Buffer, index);
		}
		[WriteAccessRequired]
		set
		{
			UnsafeUtility.WriteArrayElement(m_Buffer, index, value);
		}
	}

	public unsafe bool IsCreated => m_Buffer != null;

	public unsafe NativeArray(int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
	{
		Allocate(length, allocator, out this);
		if ((options & NativeArrayOptions.ClearMemory) == NativeArrayOptions.ClearMemory)
		{
			UnsafeUtility.MemClear(m_Buffer, (long)Length * (long)UnsafeUtility.SizeOf<T>());
		}
	}

	public NativeArray(T[] array, Allocator allocator)
	{
		Allocate(array.Length, allocator, out this);
		CopyFrom(array);
	}

	public NativeArray(NativeArray<T> array, Allocator allocator)
	{
		Allocate(array.Length, allocator, out this);
		CopyFrom(array);
	}

	private unsafe static void Allocate(int length, Allocator allocator, out NativeArray<T> array)
	{
		long size = (long)UnsafeUtility.SizeOf<T>() * (long)length;
		array.m_Buffer = UnsafeUtility.Malloc(size, UnsafeUtility.AlignOf<T>(), allocator);
		array.m_Length = length;
		array.m_AllocatorLabel = allocator;
	}

	[BurstDiscard]
	internal static void IsBlittableAndThrow()
	{
		if (!UnsafeUtility.IsBlittable<T>())
		{
			throw new ArgumentException(string.Format("{0} used in NativeArray<{0}> must be blittable", typeof(T)));
		}
	}

	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	private void CheckElementReadAccess(int index)
	{
	}

	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	private void CheckElementWriteAccess(int index)
	{
	}

	[WriteAccessRequired]
	public unsafe void Dispose()
	{
		UnsafeUtility.Free(m_Buffer, m_AllocatorLabel);
		m_Buffer = null;
		m_Length = 0;
	}

	[WriteAccessRequired]
	public unsafe void CopyFrom(T[] array)
	{
		for (int i = 0; i < Length; i++)
		{
			UnsafeUtility.WriteArrayElement(m_Buffer, i, array[i]);
		}
	}

	[WriteAccessRequired]
	public void CopyFrom(NativeArray<T> array)
	{
		array.CopyTo(this);
	}

	public unsafe void CopyTo(T[] array)
	{
		for (int i = 0; i < Length; i++)
		{
			array[i] = UnsafeUtility.ReadArrayElement<T>(m_Buffer, i);
		}
	}

	public unsafe void CopyTo(NativeArray<T> array)
	{
		UnsafeUtility.MemCpy(array.m_Buffer, m_Buffer, (long)Length * (long)UnsafeUtility.SizeOf<T>());
	}

	public T[] ToArray()
	{
		T[] array = new T[Length];
		CopyTo(array);
		return array;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(ref this);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new Enumerator(ref this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
