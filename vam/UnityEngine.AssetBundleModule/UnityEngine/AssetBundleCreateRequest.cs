using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Asynchronous create request for an AssetBundle.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
public sealed class AssetBundleCreateRequest : AsyncOperation
{
	/// <summary>
	///   <para>Asset object being loaded (Read Only).</para>
	/// </summary>
	public extern AssetBundle assetBundle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void DisableCompatibilityChecks();
}
