using System;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

/// <summary>
///   <para>A IPlayableOutput implementation that contains a script output for the a PlayableGraph.</para>
/// </summary>
[RequiredByNativeCode]
public struct ScriptPlayableOutput : IPlayableOutput
{
	private PlayableOutputHandle m_Handle;

	/// <summary>
	///   <para>Returns an invalid ScriptPlayableOutput.</para>
	/// </summary>
	public static ScriptPlayableOutput Null => new ScriptPlayableOutput(PlayableOutputHandle.Null);

	internal ScriptPlayableOutput(PlayableOutputHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOutputOfType<ScriptPlayableOutput>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not a ScriptPlayableOutput.");
		}
		m_Handle = handle;
	}

	/// <summary>
	///   <para>Creates a new ScriptPlayableOutput in the associated PlayableGraph.</para>
	/// </summary>
	/// <param name="graph">The PlayableGraph that will contain the ScriptPlayableOutput.</param>
	/// <param name="name">The name of this ScriptPlayableOutput.</param>
	/// <returns>
	///   <para>The created ScriptPlayableOutput.</para>
	/// </returns>
	public static ScriptPlayableOutput Create(PlayableGraph graph, string name)
	{
		if (!graph.CreateScriptOutputInternal(name, out var handle))
		{
			return Null;
		}
		return new ScriptPlayableOutput(handle);
	}

	public PlayableOutputHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator PlayableOutput(ScriptPlayableOutput output)
	{
		return new PlayableOutput(output.GetHandle());
	}

	public static explicit operator ScriptPlayableOutput(PlayableOutput output)
	{
		return new ScriptPlayableOutput(output.GetHandle());
	}
}
