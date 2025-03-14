using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/AssetBundle/Public/AssetBundlePatching.h")]
internal static class AssetBundleUtility
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	internal static extern void PatchAssetBundles(AssetBundle[] bundles, string[] filenames);
}
