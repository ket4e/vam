using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

/// <summary>
///   <para>An implementation of IPlayable that controls an animation mixer.</para>
/// </summary>
[NativeHeader("Runtime/Animation/ScriptBindings/AnimationMixerPlayable.bindings.h")]
[NativeHeader("Runtime/Animation/Director/AnimationMixerPlayable.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[RequiredByNativeCode]
[StaticAccessor("AnimationMixerPlayableBindings", StaticAccessorType.DoubleColon)]
public struct AnimationMixerPlayable : IPlayable, IEquatable<AnimationMixerPlayable>
{
	private PlayableHandle m_Handle;

	internal AnimationMixerPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<AnimationMixerPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AnimationMixerPlayable.");
		}
		m_Handle = handle;
	}

	/// <summary>
	///   <para>Creates an AnimationMixerPlayable in the PlayableGraph.</para>
	/// </summary>
	/// <param name="graph">The PlayableGraph that will contain the new AnimationMixerPlayable.</param>
	/// <param name="inputCount">The number of inputs that the mixer will update.</param>
	/// <param name="normalizeWeights">True to force a weight normalization of the inputs.</param>
	/// <returns>
	///   <para>A new AnimationMixerPlayable linked to the PlayableGraph.</para>
	/// </returns>
	public static AnimationMixerPlayable Create(PlayableGraph graph, int inputCount = 0, bool normalizeWeights = false)
	{
		PlayableHandle handle = CreateHandle(graph, inputCount, normalizeWeights);
		return new AnimationMixerPlayable(handle);
	}

	private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount = 0, bool normalizeWeights = false)
	{
		PlayableHandle handle = PlayableHandle.Null;
		if (!CreateHandleInternal(graph, inputCount, normalizeWeights, ref handle))
		{
			return PlayableHandle.Null;
		}
		handle.SetInputCount(inputCount);
		return handle;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator Playable(AnimationMixerPlayable playable)
	{
		return new Playable(playable.GetHandle());
	}

	public static explicit operator AnimationMixerPlayable(Playable playable)
	{
		return new AnimationMixerPlayable(playable.GetHandle());
	}

	public bool Equals(AnimationMixerPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}

	private static bool CreateHandleInternal(PlayableGraph graph, int inputCount, bool normalizeWeights, ref PlayableHandle handle)
	{
		return CreateHandleInternal_Injected(ref graph, inputCount, normalizeWeights, ref handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, int inputCount, bool normalizeWeights, ref PlayableHandle handle);
}
