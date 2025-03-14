using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

/// <summary>
///   <para>An implementation of IPlayable that controls an animation layer mixer.</para>
/// </summary>
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[RequiredByNativeCode]
[NativeHeader("Runtime/Animation/ScriptBindings/AnimationLayerMixerPlayable.bindings.h")]
[NativeHeader("Runtime/Animation/Director/AnimationLayerMixerPlayable.h")]
[StaticAccessor("AnimationLayerMixerPlayableBindings", StaticAccessorType.DoubleColon)]
public struct AnimationLayerMixerPlayable : IPlayable, IEquatable<AnimationLayerMixerPlayable>
{
	private PlayableHandle m_Handle;

	private static readonly AnimationLayerMixerPlayable m_NullPlayable = new AnimationLayerMixerPlayable(PlayableHandle.Null);

	/// <summary>
	///   <para>Returns an invalid AnimationLayerMixerPlayable.</para>
	/// </summary>
	public static AnimationLayerMixerPlayable Null => m_NullPlayable;

	internal AnimationLayerMixerPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<AnimationLayerMixerPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AnimationLayerMixerPlayable.");
		}
		m_Handle = handle;
	}

	/// <summary>
	///   <para>Creates an AnimationLayerMixerPlayable in the PlayableGraph.</para>
	/// </summary>
	/// <param name="graph">The PlayableGraph that will contain the new AnimationLayerMixerPlayable.</param>
	/// <param name="inputCount">The number of layers.</param>
	/// <returns>
	///   <para>A new AnimationLayerMixerPlayable linked to the PlayableGraph.</para>
	/// </returns>
	public static AnimationLayerMixerPlayable Create(PlayableGraph graph, int inputCount = 0)
	{
		PlayableHandle handle = CreateHandle(graph, inputCount);
		return new AnimationLayerMixerPlayable(handle);
	}

	private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount = 0)
	{
		PlayableHandle handle = PlayableHandle.Null;
		if (!CreateHandleInternal(graph, ref handle))
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

	public static implicit operator Playable(AnimationLayerMixerPlayable playable)
	{
		return new Playable(playable.GetHandle());
	}

	public static explicit operator AnimationLayerMixerPlayable(Playable playable)
	{
		return new AnimationLayerMixerPlayable(playable.GetHandle());
	}

	public bool Equals(AnimationLayerMixerPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}

	/// <summary>
	///   <para>Returns true if the layer is additive, false otherwise.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <returns>
	///   <para>True if the layer is additive, false otherwise.</para>
	/// </returns>
	public bool IsLayerAdditive(uint layerIndex)
	{
		if (layerIndex >= m_Handle.GetInputCount())
		{
			throw new ArgumentOutOfRangeException("layerIndex", $"layerIndex {layerIndex} must be in the range of 0 to {m_Handle.GetInputCount() - 1}.");
		}
		return IsLayerAdditiveInternal(ref m_Handle, layerIndex);
	}

	/// <summary>
	///   <para>Specifies whether a layer is additive or not. Additive layers blend with previous layers.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <param name="value">Whether the layer is additive or not. Set to true for an additive blend, or false for a regular blend.</param>
	public void SetLayerAdditive(uint layerIndex, bool value)
	{
		if (layerIndex >= m_Handle.GetInputCount())
		{
			throw new ArgumentOutOfRangeException("layerIndex", $"layerIndex {layerIndex} must be in the range of 0 to {m_Handle.GetInputCount() - 1}.");
		}
		SetLayerAdditiveInternal(ref m_Handle, layerIndex, value);
	}

	/// <summary>
	///   <para>Sets the mask for the current layer.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <param name="mask">The AvatarMask used to create the new LayerMask.</param>
	public void SetLayerMaskFromAvatarMask(uint layerIndex, AvatarMask mask)
	{
		if (layerIndex >= m_Handle.GetInputCount())
		{
			throw new ArgumentOutOfRangeException("layerIndex", $"layerIndex {layerIndex} must be in the range of 0 to {m_Handle.GetInputCount() - 1}.");
		}
		if (mask == null)
		{
			throw new ArgumentNullException("mask");
		}
		SetLayerMaskFromAvatarMaskInternal(ref m_Handle, layerIndex, mask);
	}

	private static bool CreateHandleInternal(PlayableGraph graph, ref PlayableHandle handle)
	{
		return CreateHandleInternal_Injected(ref graph, ref handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLayerMaskFromAvatarMaskInternal(ref PlayableHandle handle, uint layerIndex, AvatarMask mask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, ref PlayableHandle handle);
}
