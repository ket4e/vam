using System;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement;

/// <summary>
///   <para>Scene management at run-time.</para>
/// </summary>
[RequiredByNativeCode]
public class SceneManager
{
	/// <summary>
	///   <para>The total number of currently loaded Scenes.</para>
	/// </summary>
	public static extern int sceneCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Number of Scenes in Build Settings.</para>
	/// </summary>
	public static extern int sceneCountInBuildSettings
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static event UnityAction<Scene, LoadSceneMode> sceneLoaded;

	public static event UnityAction<Scene> sceneUnloaded;

	public static event UnityAction<Scene, Scene> activeSceneChanged;

	/// <summary>
	///   <para>Gets the currently active Scene.</para>
	/// </summary>
	/// <returns>
	///   <para>The active Scene.</para>
	/// </returns>
	public static Scene GetActiveScene()
	{
		INTERNAL_CALL_GetActiveScene(out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetActiveScene(out Scene value);

	/// <summary>
	///   <para>Set the Scene to be active.</para>
	/// </summary>
	/// <param name="scene">The Scene to be set.</param>
	/// <returns>
	///   <para>Returns false if the Scene is not loaded yet.</para>
	/// </returns>
	public static bool SetActiveScene(Scene scene)
	{
		return INTERNAL_CALL_SetActiveScene(ref scene);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_SetActiveScene(ref Scene scene);

	/// <summary>
	///   <para>Searches all Scenes loaded for a Scene that has the given asset path.</para>
	/// </summary>
	/// <param name="scenePath">Path of the Scene. Should be relative to the project folder. Like: "AssetsMyScenesMyScene.unity".</param>
	/// <returns>
	///   <para>A reference to the Scene, if valid. If not, an invalid Scene is returned.</para>
	/// </returns>
	public static Scene GetSceneByPath(string scenePath)
	{
		INTERNAL_CALL_GetSceneByPath(scenePath, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetSceneByPath(string scenePath, out Scene value);

	/// <summary>
	///   <para>Searches through the Scenes loaded for a Scene with the given name.</para>
	/// </summary>
	/// <param name="name">Name of Scene to find.</param>
	/// <returns>
	///   <para>A reference to the Scene, if valid. If not, an invalid Scene is returned.</para>
	/// </returns>
	public static Scene GetSceneByName(string name)
	{
		INTERNAL_CALL_GetSceneByName(name, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetSceneByName(string name, out Scene value);

	/// <summary>
	///   <para>Get a Scene struct from a build index.</para>
	/// </summary>
	/// <param name="buildIndex">Build index as shown in the Build Settings window.</param>
	/// <returns>
	///   <para>A reference to the Scene, if valid. If not, an invalid Scene is returned.</para>
	/// </returns>
	public static Scene GetSceneByBuildIndex(int buildIndex)
	{
		INTERNAL_CALL_GetSceneByBuildIndex(buildIndex, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetSceneByBuildIndex(int buildIndex, out Scene value);

	/// <summary>
	///   <para>Get the Scene at index in the SceneManager's list of loaded Scenes.</para>
	/// </summary>
	/// <param name="index">Index of the Scene to get. Index must be greater than or equal to 0 and less than SceneManager.sceneCount.</param>
	/// <returns>
	///   <para>A reference to the Scene at the index specified.</para>
	/// </returns>
	public static Scene GetSceneAt(int index)
	{
		INTERNAL_CALL_GetSceneAt(index, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetSceneAt(int index, out Scene value);

	/// <summary>
	///   <para>Returns an array of all the Scenes currently open in the hierarchy.</para>
	/// </summary>
	/// <returns>
	///   <para>Array of Scenes in the Hierarchy.</para>
	/// </returns>
	[Obsolete("Use SceneManager.sceneCount and SceneManager.GetSceneAt(int index) to loop the all scenes instead.")]
	public static Scene[] GetAllScenes()
	{
		Scene[] array = new Scene[sceneCount];
		for (int i = 0; i < sceneCount; i++)
		{
			ref Scene reference = ref array[i];
			reference = GetSceneAt(i);
		}
		return array;
	}

	[ExcludeFromDocs]
	public static void LoadScene(string sceneName)
	{
		LoadSceneMode mode = LoadSceneMode.Single;
		LoadScene(sceneName, mode);
	}

	/// <summary>
	///   <para>Loads the Scene by its name or index in Build Settings.</para>
	/// </summary>
	/// <param name="sceneName">Name or path of the Scene to load.</param>
	/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
	/// <param name="mode">Allows you to specify whether or not to load the Scene additively.
	///   See SceneManagement.LoadSceneMode for more information about the options.</param>
	public static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
	{
		LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, mustCompleteNextFrame: true);
	}

	[ExcludeFromDocs]
	public static void LoadScene(int sceneBuildIndex)
	{
		LoadSceneMode mode = LoadSceneMode.Single;
		LoadScene(sceneBuildIndex, mode);
	}

	/// <summary>
	///   <para>Loads the Scene by its name or index in Build Settings.</para>
	/// </summary>
	/// <param name="sceneName">Name or path of the Scene to load.</param>
	/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
	/// <param name="mode">Allows you to specify whether or not to load the Scene additively.
	///   See SceneManagement.LoadSceneMode for more information about the options.</param>
	public static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
	{
		LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, mustCompleteNextFrame: true);
	}

	[ExcludeFromDocs]
	public static AsyncOperation LoadSceneAsync(string sceneName)
	{
		LoadSceneMode mode = LoadSceneMode.Single;
		return LoadSceneAsync(sceneName, mode);
	}

	/// <summary>
	///   <para>Loads the Scene asynchronously in the background.</para>
	/// </summary>
	/// <param name="sceneName">Name or path of the Scene to load.</param>
	/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
	/// <param name="mode">If LoadSceneMode.Single then all current Scenes will be unloaded before loading.</param>
	/// <returns>
	///   <para>Use the AsyncOperation to determine if the operation has completed.</para>
	/// </returns>
	public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
	{
		return LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, mustCompleteNextFrame: false);
	}

	[ExcludeFromDocs]
	public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
	{
		LoadSceneMode mode = LoadSceneMode.Single;
		return LoadSceneAsync(sceneBuildIndex, mode);
	}

	/// <summary>
	///   <para>Loads the Scene asynchronously in the background.</para>
	/// </summary>
	/// <param name="sceneName">Name or path of the Scene to load.</param>
	/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
	/// <param name="mode">If LoadSceneMode.Single then all current Scenes will be unloaded before loading.</param>
	/// <returns>
	///   <para>Use the AsyncOperation to determine if the operation has completed.</para>
	/// </returns>
	public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
	{
		return LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, mustCompleteNextFrame: false);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, bool isAdditive, bool mustCompleteNextFrame);

	/// <summary>
	///   <para>Create an empty new Scene at runtime with the given name.</para>
	/// </summary>
	/// <param name="sceneName">The name of the new Scene. It cannot be empty or null, or same as the name of the existing Scenes.</param>
	/// <returns>
	///   <para>A reference to the new Scene that was created, or an invalid Scene if creation failed.</para>
	/// </returns>
	public static Scene CreateScene(string sceneName)
	{
		INTERNAL_CALL_CreateScene(sceneName, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_CreateScene(string sceneName, out Scene value);

	/// <summary>
	///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
	/// </summary>
	/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to unload.</param>
	/// <param name="sceneName">Name or path of the Scene to unload.</param>
	/// <param name="scene">Scene to unload.</param>
	/// <returns>
	///   <para>Returns true if the Scene is unloaded.</para>
	/// </returns>
	[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
	public static bool UnloadScene(Scene scene)
	{
		return UnloadSceneInternal(scene);
	}

	private static bool UnloadSceneInternal(Scene scene)
	{
		return INTERNAL_CALL_UnloadSceneInternal(ref scene);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_UnloadSceneInternal(ref Scene scene);

	/// <summary>
	///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
	/// </summary>
	/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to unload.</param>
	/// <param name="sceneName">Name or path of the Scene to unload.</param>
	/// <param name="scene">Scene to unload.</param>
	/// <returns>
	///   <para>Returns true if the Scene is unloaded.</para>
	/// </returns>
	[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
	public static bool UnloadScene(int sceneBuildIndex)
	{
		UnloadSceneNameIndexInternal("", sceneBuildIndex, immediately: true, out var outSuccess);
		return outSuccess;
	}

	/// <summary>
	///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
	/// </summary>
	/// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to unload.</param>
	/// <param name="sceneName">Name or path of the Scene to unload.</param>
	/// <param name="scene">Scene to unload.</param>
	/// <returns>
	///   <para>Returns true if the Scene is unloaded.</para>
	/// </returns>
	[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
	public static bool UnloadScene(string sceneName)
	{
		UnloadSceneNameIndexInternal(sceneName, -1, immediately: true, out var outSuccess);
		return outSuccess;
	}

	/// <summary>
	///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
	/// </summary>
	/// <param name="sceneBuildIndex">Index of the Scene in BuildSettings.</param>
	/// <param name="sceneName">Name or path of the Scene to unload.</param>
	/// <param name="scene">Scene to unload.</param>
	/// <returns>
	///   <para>Use the AsyncOperation to determine if the operation has completed.</para>
	/// </returns>
	public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
	{
		bool outSuccess;
		return UnloadSceneNameIndexInternal("", sceneBuildIndex, immediately: false, out outSuccess);
	}

	/// <summary>
	///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
	/// </summary>
	/// <param name="sceneBuildIndex">Index of the Scene in BuildSettings.</param>
	/// <param name="sceneName">Name or path of the Scene to unload.</param>
	/// <param name="scene">Scene to unload.</param>
	/// <returns>
	///   <para>Use the AsyncOperation to determine if the operation has completed.</para>
	/// </returns>
	public static AsyncOperation UnloadSceneAsync(string sceneName)
	{
		bool outSuccess;
		return UnloadSceneNameIndexInternal(sceneName, -1, immediately: false, out outSuccess);
	}

	/// <summary>
	///   <para>Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager.</para>
	/// </summary>
	/// <param name="sceneBuildIndex">Index of the Scene in BuildSettings.</param>
	/// <param name="sceneName">Name or path of the Scene to unload.</param>
	/// <param name="scene">Scene to unload.</param>
	/// <returns>
	///   <para>Use the AsyncOperation to determine if the operation has completed.</para>
	/// </returns>
	public static AsyncOperation UnloadSceneAsync(Scene scene)
	{
		return UnloadSceneAsyncInternal(scene);
	}

	private static AsyncOperation UnloadSceneAsyncInternal(Scene scene)
	{
		return INTERNAL_CALL_UnloadSceneAsyncInternal(ref scene);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern AsyncOperation INTERNAL_CALL_UnloadSceneAsyncInternal(ref Scene scene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern AsyncOperation UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex, bool immediately, out bool outSuccess);

	/// <summary>
	///   <para>This will merge the source Scene into the destinationScene.</para>
	/// </summary>
	/// <param name="sourceScene">The Scene that will be merged into the destination Scene.</param>
	/// <param name="destinationScene">Existing Scene to merge the source Scene into.</param>
	public static void MergeScenes(Scene sourceScene, Scene destinationScene)
	{
		INTERNAL_CALL_MergeScenes(ref sourceScene, ref destinationScene);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_MergeScenes(ref Scene sourceScene, ref Scene destinationScene);

	/// <summary>
	///   <para>Move a GameObject from its current Scene to a new Scene.</para>
	/// </summary>
	/// <param name="go">GameObject to move.</param>
	/// <param name="scene">Scene to move into.</param>
	public static void MoveGameObjectToScene(GameObject go, Scene scene)
	{
		INTERNAL_CALL_MoveGameObjectToScene(go, ref scene);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_MoveGameObjectToScene(GameObject go, ref Scene scene);

	[RequiredByNativeCode]
	private static void Internal_SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (SceneManager.sceneLoaded != null)
		{
			SceneManager.sceneLoaded(scene, mode);
		}
	}

	[RequiredByNativeCode]
	private static void Internal_SceneUnloaded(Scene scene)
	{
		if (SceneManager.sceneUnloaded != null)
		{
			SceneManager.sceneUnloaded(scene);
		}
	}

	[RequiredByNativeCode]
	private static void Internal_ActiveSceneChanged(Scene previousActiveScene, Scene newActiveScene)
	{
		if (SceneManager.activeSceneChanged != null)
		{
			SceneManager.activeSceneChanged(previousActiveScene, newActiveScene);
		}
	}
}
