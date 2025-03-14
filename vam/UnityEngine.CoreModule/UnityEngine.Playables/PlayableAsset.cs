using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

/// <summary>
///   <para>An base class for assets that can be used to instatiate a Playable at runtime.</para>
/// </summary>
[Serializable]
[RequiredByNativeCode]
[AssetFileNameExtension("playable", new string[] { })]
public abstract class PlayableAsset : ScriptableObject, IPlayableAsset
{
	/// <summary>
	///   <para>The playback duration in seconds of the instantiated Playable.</para>
	/// </summary>
	public virtual double duration => PlayableBinding.DefaultDuration;

	/// <summary>
	///   <para>A description of the outputs of the instantiated Playable.</para>
	/// </summary>
	public virtual IEnumerable<PlayableBinding> outputs => PlayableBinding.None;

	/// <summary>
	///   <para>Implement this method to have your asset inject playables into the given graph.</para>
	/// </summary>
	/// <param name="graph">The graph to inject playables into.</param>
	/// <param name="owner">The game object which initiated the build.</param>
	/// <returns>
	///   <para>The playable injected into the graph, or the root playable if multiple playables are injected.</para>
	/// </returns>
	public abstract Playable CreatePlayable(PlayableGraph graph, GameObject owner);

	[RequiredByNativeCode]
	internal unsafe static void Internal_CreatePlayable(PlayableAsset asset, PlayableGraph graph, GameObject go, IntPtr ptr)
	{
		Playable playable = ((!(asset == null)) ? asset.CreatePlayable(graph, go) : Playable.Null);
		Playable* ptr2 = (Playable*)ptr.ToPointer();
		*ptr2 = playable;
	}

	[RequiredByNativeCode]
	internal unsafe static void Internal_GetPlayableAssetDuration(PlayableAsset asset, IntPtr ptrToDouble)
	{
		double num = asset.duration;
		double* ptr = (double*)ptrToDouble.ToPointer();
		*ptr = num;
	}
}
