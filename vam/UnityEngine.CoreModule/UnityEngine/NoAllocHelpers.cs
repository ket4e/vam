using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

internal sealed class NoAllocHelpers
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void Internal_ResizeList(object list, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern Array ExtractArrayFromList(object list);

	public static int SafeLength(Array values)
	{
		return values?.Length ?? 0;
	}

	public static int SafeLength<T>(List<T> values)
	{
		return values?.Count ?? 0;
	}

	public static void ResizeList<T>(List<T> list, int size)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (size < 0 || size > list.Capacity)
		{
			throw new ArgumentException("list", "invalid size to resize.");
		}
		if (size != list.Count)
		{
			Internal_ResizeList(list, size);
		}
	}

	public static T[] ExtractArrayFromListT<T>(List<T> list)
	{
		return (T[])ExtractArrayFromList(list);
	}

	public static void EnsureListElemCount<T>(List<T> list, int count)
	{
		list.Clear();
		if (list.Capacity < count)
		{
			list.Capacity = count;
		}
		ResizeList(list, count);
	}
}
