using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop;

/// <summary>
///   <para>Update phase in the native player loop.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[RequiredByNativeCode]
public struct Initialization
{
	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct PlayerUpdateTime
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct AsyncUploadTimeSlicedUpdate
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct SynchronizeState
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct SynchronizeInputs
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct XREarlyUpdate
	{
	}
}
