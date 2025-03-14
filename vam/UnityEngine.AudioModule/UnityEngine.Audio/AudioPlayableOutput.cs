using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

/// <summary>
///   <para>A IPlayableOutput implementation that will be used to play audio.</para>
/// </summary>
[NativeHeader("Modules/Audio/Public/AudioSource.h")]
[NativeHeader("Modules/Audio/Public/Director/AudioPlayableOutput.h")]
[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioPlayableOutput.bindings.h")]
[StaticAccessor("AudioPlayableOutputBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
public struct AudioPlayableOutput : IPlayableOutput
{
	private PlayableOutputHandle m_Handle;

	/// <summary>
	///   <para>Returns an invalid AudioPlayableOutput.</para>
	/// </summary>
	public static AudioPlayableOutput Null => new AudioPlayableOutput(PlayableOutputHandle.Null);

	internal AudioPlayableOutput(PlayableOutputHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOutputOfType<AudioPlayableOutput>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AudioPlayableOutput.");
		}
		m_Handle = handle;
	}

	/// <summary>
	///   <para>Creates an AudioPlayableOutput in the PlayableGraph.</para>
	/// </summary>
	/// <param name="graph">The PlayableGraph that will contain the AnimationPlayableOutput.</param>
	/// <param name="name">The name of the output.</param>
	/// <param name="target">The AudioSource that will play the AudioPlayableOutput source Playable.</param>
	/// <returns>
	///   <para>A new AudioPlayableOutput attached to the PlayableGraph.</para>
	/// </returns>
	public static AudioPlayableOutput Create(PlayableGraph graph, string name, AudioSource target)
	{
		if (!AudioPlayableGraphExtensions.InternalCreateAudioOutput(ref graph, name, out var handle))
		{
			return Null;
		}
		AudioPlayableOutput result = new AudioPlayableOutput(handle);
		result.SetTarget(target);
		return result;
	}

	public PlayableOutputHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator PlayableOutput(AudioPlayableOutput output)
	{
		return new PlayableOutput(output.GetHandle());
	}

	public static explicit operator AudioPlayableOutput(PlayableOutput output)
	{
		return new AudioPlayableOutput(output.GetHandle());
	}

	public AudioSource GetTarget()
	{
		return InternalGetTarget(ref m_Handle);
	}

	public void SetTarget(AudioSource value)
	{
		InternalSetTarget(ref m_Handle, value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AudioSource InternalGetTarget(ref PlayableOutputHandle output);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetTarget(ref PlayableOutputHandle output, AudioSource target);
}
