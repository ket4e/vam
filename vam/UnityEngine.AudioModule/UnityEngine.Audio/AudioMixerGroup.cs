using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

/// <summary>
///   <para>Object representing a group in the mixer.</para>
/// </summary>
public class AudioMixerGroup : Object, ISubAssetNotDuplicatable
{
	public extern AudioMixer audioMixer
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	internal AudioMixerGroup()
	{
	}
}
