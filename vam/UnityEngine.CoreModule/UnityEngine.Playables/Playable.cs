using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

/// <summary>
///   <para>Playables are customizable runtime objects that can be connected together and are contained in a PlayableGraph to create complex behaviours.</para>
/// </summary>
[RequiredByNativeCode]
public struct Playable : IPlayable, IEquatable<Playable>
{
	private PlayableHandle m_Handle;

	private static readonly Playable m_NullPlayable = new Playable(PlayableHandle.Null);

	/// <summary>
	///   <para>Returns an invalid Playable.</para>
	/// </summary>
	public static Playable Null => m_NullPlayable;

	[VisibleToOtherModules]
	internal Playable(PlayableHandle handle)
	{
		m_Handle = handle;
	}

	public static Playable Create(PlayableGraph graph, int inputCount = 0)
	{
		Playable playable = new Playable(graph.CreatePlayableHandle());
		playable.SetInputCount(inputCount);
		return playable;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public bool IsPlayableOfType<T>() where T : struct, IPlayable
	{
		return GetHandle().IsPlayableOfType<T>();
	}

	public Type GetPlayableType()
	{
		return GetHandle().GetPlayableType();
	}

	public bool Equals(Playable other)
	{
		return GetHandle() == other.GetHandle();
	}
}
