using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;
using UnityEngine.Video;

namespace UnityEngine.Experimental.Video;

/// <summary>
///   <para>An implementation of IPlayable that controls playback of a VideoClip.</para>
/// </summary>
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[StaticAccessor("VideoClipPlayableBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
[NativeHeader("Modules/Video/Public/VideoClip.h")]
[NativeHeader("Modules/Video/Public/ScriptBindings/VideoClipPlayable.bindings.h")]
[NativeHeader("Modules/Video/Public/Director/VideoClipPlayable.h")]
public struct VideoClipPlayable : IPlayable, IEquatable<VideoClipPlayable>
{
	private PlayableHandle m_Handle;

	internal VideoClipPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<VideoClipPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an VideoClipPlayable.");
		}
		m_Handle = handle;
	}

	/// <summary>
	///   <para>Creates a VideoClipPlayable in the PlayableGraph.</para>
	/// </summary>
	/// <param name="graph">The PlayableGraph object that will own the VideoClipPlayable.</param>
	/// <param name="looping">Indicates if VideoClip loops when it reaches the end.</param>
	/// <param name="clip">VideoClip used to produce textures in the PlayableGraph.</param>
	/// <returns>
	///   <para>A VideoClipPlayable linked to the PlayableGraph.</para>
	/// </returns>
	public static VideoClipPlayable Create(PlayableGraph graph, VideoClip clip, bool looping)
	{
		PlayableHandle handle = CreateHandle(graph, clip, looping);
		VideoClipPlayable videoClipPlayable = new VideoClipPlayable(handle);
		if (clip != null)
		{
			videoClipPlayable.SetDuration(clip.length);
		}
		return videoClipPlayable;
	}

	private static PlayableHandle CreateHandle(PlayableGraph graph, VideoClip clip, bool looping)
	{
		PlayableHandle handle = PlayableHandle.Null;
		if (!InternalCreateVideoClipPlayable(ref graph, clip, looping, ref handle))
		{
			return PlayableHandle.Null;
		}
		return handle;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator Playable(VideoClipPlayable playable)
	{
		return new Playable(playable.GetHandle());
	}

	public static explicit operator VideoClipPlayable(Playable playable)
	{
		return new VideoClipPlayable(playable.GetHandle());
	}

	public bool Equals(VideoClipPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}

	public VideoClip GetClip()
	{
		return GetClipInternal(ref m_Handle);
	}

	public void SetClip(VideoClip value)
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

	public bool IsPlaying()
	{
		return GetIsPlayingInternal(ref m_Handle);
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
			throw new ArgumentException("VideoClipPlayable.pauseDelay: Setting new delay when existing delay is too small or 0.0 (" + pauseDelayInternal + "), Video system will not be able to change in time");
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
		if (IsPlaying() && (startDelay < 0.05 || (startDelayInternal >= 1E-05 && startDelayInternal < 0.05)))
		{
			Debug.LogWarning("VideoClipPlayable.StartDelay: Setting new delay when existing delay is too small or 0.0 (" + startDelayInternal + "), Video system will not be able to change in time");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern VideoClip GetClipInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetClipInternal(ref PlayableHandle hdl, VideoClip clip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetLoopedInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLoopedInternal(ref PlayableHandle hdl, bool looped);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetIsPlayingInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double GetStartDelayInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetStartDelayInternal(ref PlayableHandle hdl, double delay);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double GetPauseDelayInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPauseDelayInternal(ref PlayableHandle hdl, double delay);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool InternalCreateVideoClipPlayable(ref PlayableGraph graph, VideoClip clip, bool looping, ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ValidateType(ref PlayableHandle hdl);
}
