using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

/// <summary>
///   <para>A Playable that controls an AnimationClip.</para>
/// </summary>
[NativeHeader("Runtime/Animation/ScriptBindings/AnimationClipPlayable.bindings.h")]
[NativeHeader("Runtime/Animation/Director/AnimationClipPlayable.h")]
[StaticAccessor("AnimationClipPlayableBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
public struct AnimationClipPlayable : IPlayable, IEquatable<AnimationClipPlayable>
{
	private PlayableHandle m_Handle;

	internal AnimationClipPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<AnimationClipPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AnimationClipPlayable.");
		}
		m_Handle = handle;
	}

	/// <summary>
	///   <para>Creates an AnimationClipPlayable in the PlayableGraph.</para>
	/// </summary>
	/// <param name="graph">The PlayableGraph object that will own the AnimationClipPlayable.</param>
	/// <param name="clip">The AnimationClip that will be added in the PlayableGraph.</param>
	/// <returns>
	///   <para>A AnimationClipPlayable linked to the PlayableGraph.</para>
	/// </returns>
	public static AnimationClipPlayable Create(PlayableGraph graph, AnimationClip clip)
	{
		PlayableHandle handle = CreateHandle(graph, clip);
		return new AnimationClipPlayable(handle);
	}

	private static PlayableHandle CreateHandle(PlayableGraph graph, AnimationClip clip)
	{
		PlayableHandle handle = PlayableHandle.Null;
		if (!CreateHandleInternal(graph, clip, ref handle))
		{
			return PlayableHandle.Null;
		}
		return handle;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator Playable(AnimationClipPlayable playable)
	{
		return new Playable(playable.GetHandle());
	}

	public static explicit operator AnimationClipPlayable(Playable playable)
	{
		return new AnimationClipPlayable(playable.GetHandle());
	}

	public bool Equals(AnimationClipPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}

	/// <summary>
	///   <para>Returns the AnimationClip stored in the AnimationClipPlayable.</para>
	/// </summary>
	public AnimationClip GetAnimationClip()
	{
		return GetAnimationClipInternal(ref m_Handle);
	}

	/// <summary>
	///   <para>Returns the state of the ApplyFootIK flag.</para>
	/// </summary>
	public bool GetApplyFootIK()
	{
		return GetApplyFootIKInternal(ref m_Handle);
	}

	/// <summary>
	///   <para>Sets the value of the ApplyFootIK flag.</para>
	/// </summary>
	/// <param name="value">The new value of the ApplyFootIK flag.</param>
	public void SetApplyFootIK(bool value)
	{
		SetApplyFootIKInternal(ref m_Handle, value);
	}

	/// <summary>
	///   <para>Returns the state of the ApplyPlayableIK flag.</para>
	/// </summary>
	public bool GetApplyPlayableIK()
	{
		return GetApplyPlayableIKInternal(ref m_Handle);
	}

	/// <summary>
	///   <para>Requests OnAnimatorIK to be called on the animated GameObject.</para>
	/// </summary>
	/// <param name="value"></param>
	public void SetApplyPlayableIK(bool value)
	{
		SetApplyPlayableIKInternal(ref m_Handle, value);
	}

	internal bool GetRemoveStartOffset()
	{
		return GetRemoveStartOffsetInternal(ref m_Handle);
	}

	internal void SetRemoveStartOffset(bool value)
	{
		SetRemoveStartOffsetInternal(ref m_Handle, value);
	}

	private static bool CreateHandleInternal(PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle)
	{
		return CreateHandleInternal_Injected(ref graph, clip, ref handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimationClip GetAnimationClipInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetApplyFootIKInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetApplyFootIKInternal(ref PlayableHandle handle, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetApplyPlayableIKInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetApplyPlayableIKInternal(ref PlayableHandle handle, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetRemoveStartOffsetInternal(ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetRemoveStartOffsetInternal(ref PlayableHandle handle, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle);
}
