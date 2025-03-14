using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental;

/// <summary>
///   <para>Information about a subsystem that can be queried before creating a subsystem instance.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[UsedByNativeCode("XRSubsystemDescriptor")]
[NativeType(Header = "Modules/XR/XRSubsystemDescriptor.h")]
public class SubsystemDescriptor<TSubsystem> : SubsystemDescriptorBase where TSubsystem : Subsystem
{
	public TSubsystem Create()
	{
		IntPtr ptr = Internal_SubsystemDescriptors.Create(m_Ptr);
		TSubsystem val = (TSubsystem)Internal_SubsystemInstances.Internal_GetInstanceByPtr(ptr);
		val.m_subsystemDescriptor = this;
		return val;
	}
}
