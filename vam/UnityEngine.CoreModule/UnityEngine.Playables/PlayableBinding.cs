using System;

namespace UnityEngine.Playables;

/// <summary>
///   <para>Struct that holds information regarding an output of a PlayableAsset.</para>
/// </summary>
public struct PlayableBinding
{
	/// <summary>
	///   <para>A constant to represent a PlayableAsset has no bindings.</para>
	/// </summary>
	public static readonly PlayableBinding[] None = new PlayableBinding[0];

	/// <summary>
	///   <para>The default duration used when a PlayableOutput has no fixed duration.</para>
	/// </summary>
	public static readonly double DefaultDuration = double.PositiveInfinity;

	/// <summary>
	///   <para>The name of the output or input stream.</para>
	/// </summary>
	public string streamName { get; set; }

	/// <summary>
	///   <para>The type of the output or input stream.</para>
	/// </summary>
	public DataStreamType streamType { get; set; }

	/// <summary>
	///   <para>A reference to a UnityEngine.Object that acts a key for this binding.</para>
	/// </summary>
	public Object sourceObject { get; set; }

	/// <summary>
	///   <para>When the StreamType is set to None, a binding can be represented using System.Type.</para>
	/// </summary>
	public Type sourceBindingType { get; set; }
}
