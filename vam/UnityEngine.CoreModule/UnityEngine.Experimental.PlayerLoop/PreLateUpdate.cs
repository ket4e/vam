using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop;

/// <summary>
///   <para>Update phase in the native player loop.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
[RequiredByNativeCode]
public struct PreLateUpdate
{
	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct AIUpdatePostScript
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct DirectorUpdateAnimationBegin
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct LegacyAnimationUpdate
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct DirectorUpdateAnimationEnd
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct DirectorDeferredEvaluate
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateNetworkManager
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct UpdateMasterServerInterface
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct UNetUpdate
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct EndGraphicsJobsLate
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct ParticleSystemBeginUpdateAll
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct ScriptRunBehaviourLateUpdate
	{
	}

	/// <summary>
	///   <para>Native engine system updated by the native player loop.</para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[RequiredByNativeCode]
	public struct ConstraintManagerUpdate
	{
	}
}
