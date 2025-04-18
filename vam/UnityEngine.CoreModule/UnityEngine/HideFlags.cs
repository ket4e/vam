using System;

namespace UnityEngine;

/// <summary>
///   <para>Bit mask that controls object destruction, saving and visibility in inspectors.</para>
/// </summary>
[Flags]
public enum HideFlags
{
	/// <summary>
	///   <para>A normal, visible object. This is the default.</para>
	/// </summary>
	None = 0,
	/// <summary>
	///   <para>The object will not appear in the hierarchy.</para>
	/// </summary>
	HideInHierarchy = 1,
	/// <summary>
	///   <para>It is not possible to view it in the inspector.</para>
	/// </summary>
	HideInInspector = 2,
	/// <summary>
	///   <para>The object will not be saved to the scene in the editor.</para>
	/// </summary>
	DontSaveInEditor = 4,
	/// <summary>
	///   <para>The object is not be editable in the inspector.</para>
	/// </summary>
	NotEditable = 8,
	/// <summary>
	///   <para>The object will not be saved when building a player.</para>
	/// </summary>
	DontSaveInBuild = 0x10,
	/// <summary>
	///   <para>The object will not be unloaded by Resources.UnloadUnusedAssets.</para>
	/// </summary>
	DontUnloadUnusedAsset = 0x20,
	/// <summary>
	///   <para>The object will not be saved to the scene. It will not be destroyed when a new scene is loaded. It is a shortcut for HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset.</para>
	/// </summary>
	DontSave = 0x34,
	/// <summary>
	///   <para>The GameObject is not shown in the Hierarchy, not saved to to Scenes, and not unloaded by Resources.UnloadUnusedAssets.</para>
	/// </summary>
	HideAndDontSave = 0x3D
}
