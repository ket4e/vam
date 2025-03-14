using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental;

[StructLayout(LayoutKind.Sequential)]
[UsedByNativeCode("XRSubsystemDescriptorBase")]
public class SubsystemDescriptorBase : ISubsystemDescriptor, ISubsystemDescriptorImpl
{
	internal IntPtr m_Ptr;

	IntPtr ISubsystemDescriptorImpl.ptr
	{
		get
		{
			return m_Ptr;
		}
		set
		{
			m_Ptr = value;
		}
	}

	public string id => Internal_SubsystemDescriptors.GetId(m_Ptr);
}
