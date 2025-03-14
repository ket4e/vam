using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement;

/// <summary>
///   <para>Run-time data structure for *.unity file.</para>
/// </summary>
public struct Scene
{
	internal enum LoadingState
	{
		NotLoaded,
		Loading,
		Loaded
	}

	private int m_Handle;

	internal int handle => m_Handle;

	internal LoadingState loadingState => GetLoadingStateInternal(handle);

	/// <summary>
	///   <para>Returns the relative path of the scene. Like: "AssetsMyScenesMyScene.unity".</para>
	/// </summary>
	public string path => GetPathInternal(handle);

	/// <summary>
	///   <para>Returns the name of the scene.</para>
	/// </summary>
	public string name
	{
		get
		{
			return GetNameInternal(handle);
		}
		internal set
		{
			SetNameInternal(handle, value);
		}
	}

	internal string guid => GetGUIDInternal(handle);

	/// <summary>
	///   <para>Returns true if the scene is loaded.</para>
	/// </summary>
	public bool isLoaded => GetIsLoadedInternal(handle);

	/// <summary>
	///   <para>Returns the index of the scene in the Build Settings. Always returns -1 if the scene was loaded through an AssetBundle.</para>
	/// </summary>
	public int buildIndex => GetBuildIndexInternal(handle);

	/// <summary>
	///   <para>Returns true if the scene is modifed.</para>
	/// </summary>
	public bool isDirty => GetIsDirtyInternal(handle);

	/// <summary>
	///   <para>The number of root transforms of this scene.</para>
	/// </summary>
	public int rootCount => GetRootCountInternal(handle);

	/// <summary>
	///   <para>Whether this is a valid scene.
	/// A scene may be invalid if, for example, you tried to open a scene that does not exist. In this case, the scene returned from EditorSceneManager.OpenScene would return False for IsValid.</para>
	/// </summary>
	/// <returns>
	///   <para>Whether this is a valid scene.</para>
	/// </returns>
	public bool IsValid()
	{
		return IsValidInternal(handle);
	}

	/// <summary>
	///   <para>Returns all the root game objects in the scene.</para>
	/// </summary>
	/// <returns>
	///   <para>An array of game objects.</para>
	/// </returns>
	public GameObject[] GetRootGameObjects()
	{
		List<GameObject> list = new List<GameObject>(rootCount);
		GetRootGameObjects(list);
		return list.ToArray();
	}

	public void GetRootGameObjects(List<GameObject> rootGameObjects)
	{
		if (rootGameObjects.Capacity < rootCount)
		{
			rootGameObjects.Capacity = rootCount;
		}
		rootGameObjects.Clear();
		if (!IsValid())
		{
			throw new ArgumentException("The scene is invalid.");
		}
		if (!Application.isPlaying && !isLoaded)
		{
			throw new ArgumentException("The scene is not loaded.");
		}
		if (rootCount != 0)
		{
			GetRootGameObjectsInternal(handle, rootGameObjects);
		}
	}

	public static bool operator ==(Scene lhs, Scene rhs)
	{
		return lhs.handle == rhs.handle;
	}

	public static bool operator !=(Scene lhs, Scene rhs)
	{
		return lhs.handle != rhs.handle;
	}

	public override int GetHashCode()
	{
		return m_Handle;
	}

	public override bool Equals(object other)
	{
		if (!(other is Scene scene))
		{
			return false;
		}
		return handle == scene.handle;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool IsValidInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string GetPathInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string GetNameInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SetNameInternal(int sceneHandle, string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string GetGUIDInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool GetIsLoadedInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern LoadingState GetLoadingStateInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool GetIsDirtyInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int GetBuildIndexInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int GetRootCountInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void GetRootGameObjectsInternal(int sceneHandle, object resultRootList);
}
