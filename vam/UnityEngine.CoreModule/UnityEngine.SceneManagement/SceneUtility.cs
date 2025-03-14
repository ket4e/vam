using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement;

/// <summary>
///   <para>Scene and Build Settings related utilities.</para>
/// </summary>
public static class SceneUtility
{
	/// <summary>
	///   <para>Get the scene path from a build index.</para>
	/// </summary>
	/// <param name="buildIndex"></param>
	/// <returns>
	///   <para>Scene path (e.g "AssetsScenesScene1.unity").</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern string GetScenePathByBuildIndex(int buildIndex);

	/// <summary>
	///   <para>Get the build index from a scene path.</para>
	/// </summary>
	/// <param name="scenePath">Scene path (e.g: "AssetsScenesScene1.unity").</param>
	/// <returns>
	///   <para>Build index.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern int GetBuildIndexByScenePath(string scenePath);
}
