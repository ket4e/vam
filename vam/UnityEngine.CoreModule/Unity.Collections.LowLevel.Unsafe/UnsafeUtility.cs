using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace Unity.Collections.LowLevel.Unsafe;

/// <summary>
///   <para>Unsafe utility class.</para>
/// </summary>
[VisibleToOtherModules]
[NativeHeader("Runtime/Export/Unsafe/UnsafeUtility.bindings.h")]
[StaticAccessor("UnsafeUtility", StaticAccessorType.DoubleColon)]
public static class UnsafeUtility
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private static extern int GetFieldOffsetInStruct(FieldInfo field);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private static extern int GetFieldOffsetInClass(FieldInfo field);

	/// <summary>
	///   <para>Returns the offset of the field relative struct or class it is contained in.</para>
	/// </summary>
	/// <param name="field"></param>
	public static int GetFieldOffset(FieldInfo field)
	{
		if (field.DeclaringType.IsValueType)
		{
			return GetFieldOffsetInStruct(field);
		}
		if (field.DeclaringType.IsClass)
		{
			return GetFieldOffsetInClass(field);
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void* PinGCObjectAndGetAddress(object target, out ulong gcHandle);

	/// <summary>
	///   <para>Releases a GC Object Handle, previously aquired by UnsafeUtility.PinGCObjectAndGetAddress.</para>
	/// </summary>
	/// <param name="gcHandle"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void ReleaseGCObject(ulong gcHandle);

	/// <summary>
	///   <para>Assigns an Object reference to a struct or pinned class. See Also: UnsafeUtility.PinGCObjectAndGetAddress.</para>
	/// </summary>
	/// <param name="target"></param>
	/// <param name="dstPtr"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void CopyObjectAddressToPtr(object target, void* dstPtr);

	public static bool IsBlittable<T>() where T : struct
	{
		return IsBlittable(typeof(T));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void* Malloc(long size, int alignment, Allocator allocator);

	/// <summary>
	///   <para>Free memory.</para>
	/// </summary>
	/// <param name="memory">Memory pointer.</param>
	/// <param name="allocator">Allocator.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void Free(void* memory, Allocator allocator);

	/// <summary>
	///   <para>Returns true if the allocator label is valid and can be used to allocate or deallocate memory.</para>
	/// </summary>
	/// <param name="allocator"></param>
	public static bool IsValidAllocator(Allocator allocator)
	{
		return allocator > Allocator.None;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void MemCpy(void* destination, void* source, long size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void MemCpyReplicate(void* destination, void* source, int size, int count);

	/// <summary>
	///   <para>Similar to UnsafeUtility.MemCpy but can skip bytes via desinationStride and sourceStride.</para>
	/// </summary>
	/// <param name="destination"></param>
	/// <param name="destinationStride"></param>
	/// <param name="source"></param>
	/// <param name="sourceStride"></param>
	/// <param name="elementSize"></param>
	/// <param name="count"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void MemCpyStride(void* destination, int destinationStride, void* source, int sourceStride, int elementSize, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void MemMove(void* destination, void* source, long size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void MemClear(void* destination, long size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int SizeOf(Type type);

	/// <summary>
	///   <para>Returns whether the struct is blittable.</para>
	/// </summary>
	/// <param name="type">The System.Type of a struct.</param>
	/// <returns>
	///   <para>True if struct is blittable, otherwise false.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool IsBlittable(Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	internal static extern void LogError(string msg, string filename, int linenumber);

	public unsafe static void CopyPtrToStructure<T>(void* ptr, out T output) where T : struct
	{
		output = *(T*)ptr;
	}

	public unsafe static void CopyStructureToPtr<T>(ref T input, void* ptr) where T : struct
	{
		*(T*)ptr = input;
	}

	public unsafe static T ReadArrayElement<T>(void* source, int index)
	{
		return *(T*)((byte*)source + index * System.Runtime.CompilerServices.Unsafe.SizeOf<T>());
	}

	public unsafe static T ReadArrayElementWithStride<T>(void* source, int index, int stride)
	{
		return *(T*)((byte*)source + index * stride);
	}

	public unsafe static void WriteArrayElement<T>(void* destination, int index, T value)
	{
		*(T*)((byte*)destination + index * System.Runtime.CompilerServices.Unsafe.SizeOf<T>()) = value;
	}

	public unsafe static void WriteArrayElementWithStride<T>(void* destination, int index, int stride, T value)
	{
		*(T*)((byte*)destination + index * stride) = value;
	}

	public unsafe static void* AddressOf<T>(ref T output) where T : struct
	{
		return System.Runtime.CompilerServices.Unsafe.AsPointer(ref output);
	}

	public static int SizeOf<T>() where T : struct
	{
		return System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
	}

	public static int AlignOf<T>() where T : struct
	{
		return 4;
	}
}
