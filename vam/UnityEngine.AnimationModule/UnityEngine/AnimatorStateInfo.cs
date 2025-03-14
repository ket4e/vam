using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Information about the current or next state.</para>
/// </summary>
[NativeHeader("Runtime/Animation/AnimatorInfo.h")]
[RequiredByNativeCode]
public struct AnimatorStateInfo
{
	private int m_Name;

	private int m_Path;

	private int m_FullPath;

	private float m_NormalizedTime;

	private float m_Length;

	private float m_Speed;

	private float m_SpeedMultiplier;

	private int m_Tag;

	private int m_Loop;

	/// <summary>
	///   <para>The full path hash for this state.</para>
	/// </summary>
	public int fullPathHash => m_FullPath;

	/// <summary>
	///   <para>The hashed name of the State.</para>
	/// </summary>
	[Obsolete("AnimatorStateInfo.nameHash has been deprecated. Use AnimatorStateInfo.fullPathHash instead.")]
	public int nameHash => m_Path;

	/// <summary>
	///   <para>The hash is generated using Animator.StringToHash. The hash does not include the name of the parent layer.</para>
	/// </summary>
	public int shortNameHash => m_Name;

	/// <summary>
	///   <para>Normalized time of the State.</para>
	/// </summary>
	public float normalizedTime => m_NormalizedTime;

	/// <summary>
	///   <para>Current duration of the state.</para>
	/// </summary>
	public float length => m_Length;

	/// <summary>
	///   <para>The playback speed of the animation. 1 is the normal playback speed.</para>
	/// </summary>
	public float speed => m_Speed;

	/// <summary>
	///   <para>The speed multiplier for this state.</para>
	/// </summary>
	public float speedMultiplier => m_SpeedMultiplier;

	/// <summary>
	///   <para>The Tag of the State.</para>
	/// </summary>
	public int tagHash => m_Tag;

	/// <summary>
	///   <para>Is the state looping.</para>
	/// </summary>
	public bool loop => m_Loop != 0;

	/// <summary>
	///   <para>Does name match the name of the active state in the statemachine?</para>
	/// </summary>
	/// <param name="name"></param>
	public bool IsName(string name)
	{
		int num = Animator.StringToHash(name);
		return num == m_FullPath || num == m_Name || num == m_Path;
	}

	/// <summary>
	///   <para>Does tag match the tag of the active state in the statemachine.</para>
	/// </summary>
	/// <param name="tag"></param>
	public bool IsTag(string tag)
	{
		return Animator.StringToHash(tag) == m_Tag;
	}
}
