using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

/// <summary>
///   <para>An implementation of IPlayable that controls an audio mixer.</para>
/// </summary>
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[RequiredByNativeCode]
[StaticAccessor("AudioMixerPlayableBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Modules/Audio/Public/Director/AudioMixerPlayable.h")]
[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioMixerPlayable.bindings.h")]
public struct AudioMixerPlayable : IPlayable, IEquatable<AudioMixerPlayable>
{
	private PlayableHandle m_Handle;

	internal AudioMixerPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<AudioMixerPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AudioMixerPlayable.");
		}
		m_Handle = handle;
	}

	public static AudioMixerPlayable Create(PlayableGraph graph, int inputCount = 0, bool normalizeInputVolumes = false)
	{
		PlayableHandle handle = CreateHandle(graph, inputCount, normalizeInputVolumes);
		return new AudioMixerPlayable(handle);
	}

	private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount, bool normalizeInputVolumes)
	{
		PlayableHandle handle = PlayableHandle.Null;
		if (!CreateAudioMixerPlayableInternal(ref graph, inputCount, normalizeInputVolumes, ref handle))
		{
			return PlayableHandle.Null;
		}
		return handle;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator Playable(AudioMixerPlayable playable)
	{
		return new Playable(playable.GetHandle());
	}

	public static explicit operator AudioMixerPlayable(Playable playable)
	{
		return new AudioMixerPlayable(playable.GetHandle());
	}

	public bool Equals(AudioMixerPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CreateAudioMixerPlayableInternal(ref PlayableGraph graph, int inputCount, bool normalizeInputVolumes, ref PlayableHandle handle);
}
