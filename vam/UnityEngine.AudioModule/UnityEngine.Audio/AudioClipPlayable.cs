using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

/// <summary>
///   <para>An implementation of IPlayable that controls an AudioClip.</para>
/// </summary>
[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioClipPlayable.bindings.h")]
[NativeHeader("Modules/Audio/Public/Director/AudioClipPlayable.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[StaticAccessor("AudioClipPlayableBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
public struct AudioClipPlayable : IPlayable, IEquatable<AudioClipPlayable>
{
	private PlayableHandle m_Handle;

	internal AudioClipPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<AudioClipPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AudioClipPlayable.");
		}
		m_Handle = handle;
	}

	/// <summary>
	///   <para>Creates an AudioClipPlayable in the PlayableGraph.</para>
	/// </summary>
	/// <param name="graph">The PlayableGraph that will contain the new AnimationLayerMixerPlayable.</param>
	/// <param name="clip">The AudioClip that will be added in the PlayableGraph.</param>
	/// <param name="looping">True if the clip should loop, false otherwise.</param>
	/// <returns>
	///   <para>A AudioClipPlayable linked to the PlayableGraph.</para>
	/// </returns>
	public static AudioClipPlayable Create(PlayableGraph graph, AudioClip clip, bool looping)
	{
		PlayableHandle handle = CreateHandle(graph, clip, looping);
		AudioClipPlayable audioClipPlayable = new AudioClipPlayable(handle);
		if (clip != null)
		{
			audioClipPlayable.SetDuration(clip.length);
		}
		return audioClipPlayable;
	}

	private static PlayableHandle CreateHandle(PlayableGraph graph, AudioClip clip, bool looping)
	{
		PlayableHandle handle = PlayableHandle.Null;
		if (!InternalCreateAudioClipPlayable(ref graph, clip, looping, ref handle))
		{
			return PlayableHandle.Null;
		}
		return handle;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator Playable(AudioClipPlayable playable)
	{
		return new Playable(playable.GetHandle());
	}

	public static explicit operator AudioClipPlayable(Playable playable)
	{
		return new AudioClipPlayable(playable.GetHandle());
	}

	public bool Equals(AudioClipPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}

	public AudioClip GetClip()
	{
		return GetClipInternal(ref m_Handle);
	}

	public void SetClip(AudioClip value)
	{
		SetClipInternal(ref m_Handle, value);
	}

	public bool GetLooped()
	{
		return GetLoopedInternal(ref m_Handle);
	}

	public void SetLooped(bool value)
	{
		SetLoopedInternal(ref m_Handle, value);
	}

	[Obsolete("IsPlaying() has been deprecated. Use IsChannelPlaying() instead (UnityUpgradable) -> IsChannelPlaying()", true)]
	public bool IsPlaying()
	{
		return IsChannelPlaying();
	}

	public bool IsChannelPlaying()
	{
		return GetIsChannelPlayingInternal(ref m_Handle);
	}

	public double GetStartDelay()
	{
		return GetStartDelayInternal(ref m_Handle);
	}

	internal void SetStartDelay(double value)
	{
		ValidateStartDelayInternal(value);
		SetStartDelayInternal(ref m_Handle, value);
	}

	public double GetPauseDelay()
	{
		return GetPauseDelayInternal(ref m_Handle);
	}

	internal void GetPauseDelay(double value)
	{
		double pauseDelayInternal = GetPauseDelayInternal(ref m_Handle);
		if (m_Handle.GetPlayState() == PlayState.Playing && (value < 0.05 || (pauseDelayInternal != 0.0 && pauseDelayInternal < 0.05)))
		{
			throw new ArgumentException("AudioClipPlayable.pauseDelay: Setting new delay when existing delay is too small or 0.0 (" + pauseDelayInternal + "), audio system will not be able to change in time");
		}
		SetPauseDelayInternal(ref m_Handle, value);
	}

	public void Seek(double startTime, double startDelay)
	{
		Seek(startTime, startDelay, 0.0);
	}

	public void Seek(double startTime, double startDelay, [DefaultValue("0")] double duration)
	{
		ValidateStartDelayInternal(startDelay);
		SetStartDelayInternal(ref m_Handle, startDelay);
		if (duration > 0.0)
		{
			m_Handle.SetDuration(duration + startTime);
			SetPauseDelayInternal(ref m_Handle, startDelay + duration);
		}
		else
		{
			m_Handle.SetDuration(double.MaxValue);
			SetPauseDelayInternal(ref m_Handle, 0.0);
		}
		m_Handle.SetTime(startTime);
		m_Handle.Play();
	}

	private void ValidateStartDelayInternal(double startDelay)
	{
		double startDelayInternal = GetStartDelayInternal(ref m_Handle);
		if (IsChannelPlaying() && (startDelay < 0.05 || (startDelayInternal >= 1E-05 && startDelayInternal < 0.05)))
		{
			Debug.LogWarning("AudioClipPlayable.StartDelay: Setting new delay when existing delay is too small or 0.0 (" + startDelayInternal + "), audio system will not be able to change in time");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AudioClip GetClipInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetClipInternal(ref PlayableHandle hdl, AudioClip clip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetLoopedInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLoopedInternal(ref PlayableHandle hdl, bool looped);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetIsChannelPlayingInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double GetStartDelayInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetStartDelayInternal(ref PlayableHandle hdl, double delay);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double GetPauseDelayInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPauseDelayInternal(ref PlayableHandle hdl, double delay);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool InternalCreateAudioClipPlayable(ref PlayableGraph graph, AudioClip clip, bool looping, ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ValidateType(ref PlayableHandle hdl);
}
