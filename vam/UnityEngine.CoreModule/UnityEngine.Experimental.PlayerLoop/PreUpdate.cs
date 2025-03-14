using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop;

/// <summary>
///   <para>Update phase in the native player loop.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[RequiredByNativeCode]
public struct PreUpdate
{
	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct PhysicsUpdate
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct Physics2DUpdate
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct CheckTexFieldInput
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct IMGUISendQueuedEvents
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct SendMouseEvents
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct AIUpdate
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct WindUpdate
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateVideo
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct NewInputUpdate
	{
	}
}
