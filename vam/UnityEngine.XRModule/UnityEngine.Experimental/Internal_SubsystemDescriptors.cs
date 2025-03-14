using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental;

internal static class Internal_SubsystemDescriptors
{
	internal static List<ISubsystemDescriptorImpl> s_SubsystemDescriptors = new List<ISubsystemDescriptorImpl>();

	[RequiredByNativeCode]
	internal static void Internal_InitializeManagedDescriptor(IntPtr ptr, ISubsystemDescriptorImpl desc)
	{
		desc.ptr = ptr;
		s_SubsystemDescriptors.Add(desc);
	}

	[RequiredByNativeCode]
	internal static void Internal_ClearManagedDescriptors()
	{
		foreach (ISubsystemDescriptorImpl s_SubsystemDescriptor in s_SubsystemDescriptors)
		{
			s_SubsystemDescriptor.ptr = IntPtr.Zero;
		}
		s_SubsystemDescriptors.Clear();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern IntPtr Create(IntPtr descriptorPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern string GetId(IntPtr descriptorPtr);
}
