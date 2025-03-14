using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

/// <summary>
///   <para>Object representing a snapshot in the mixer.</para>
/// </summary>
public class AudioMixerSnapshot : Object, ISubAssetNotDuplicatable
{
	public extern AudioMixer audioMixer
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	internal AudioMixerSnapshot()
	{
	}

	/// <summary>
	///   <para>Performs an interpolated transition towards this snapshot over the time interval specified.</para>
	/// </summary>
	/// <param name="timeToReach">Relative time after which this snapshot should be reached from any current state.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void TransitionTo(float timeToReach);
}
