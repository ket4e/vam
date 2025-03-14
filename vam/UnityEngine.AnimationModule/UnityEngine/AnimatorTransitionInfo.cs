using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Information about the current transition.</para>
/// </summary>
[NativeHeader("Runtime/Animation/AnimatorInfo.h")]
[RequiredByNativeCode]
public struct AnimatorTransitionInfo
{
	[NativeName("fullPathHash")]
	private int m_FullPath;

	[NativeName("userNameHash")]
	private int m_UserName;

	[NativeName("nameHash")]
	private int m_Name;

	[NativeName("hasFixedDuration")]
	private bool m_HasFixedDuration;

	[NativeName("duration")]
	private float m_Duration;

	[NativeName("normalizedTime")]
	private float m_NormalizedTime;

	[NativeName("anyState")]
	private bool m_AnyState;

	[NativeName("transitionType")]
	private int m_TransitionType;

	/// <summary>
	///   <para>The hash name of the Transition.</para>
	/// </summary>
	public int fullPathHash => m_FullPath;

	/// <summary>
	///   <para>The simplified name of the Transition.</para>
	/// </summary>
	public int nameHash => m_Name;

	/// <summary>
	///   <para>The user-specified name of the Transition.</para>
	/// </summary>
	public int userNameHash => m_UserName;

	/// <summary>
	///   <para>The unit of the transition duration.</para>
	/// </summary>
	public DurationUnit durationUnit => (!m_HasFixedDuration) ? DurationUnit.Normalized : DurationUnit.Fixed;

	/// <summary>
	///   <para>Duration of the transition.</para>
	/// </summary>
	public float duration => m_Duration;

	/// <summary>
	///   <para>Normalized time of the Transition.</para>
	/// </summary>
	public float normalizedTime => m_NormalizedTime;

	/// <summary>
	///   <para>Returns true if the transition is from an AnyState node, or from Animator.CrossFade.</para>
	/// </summary>
	public bool anyState => m_AnyState;

	internal bool entry => (m_TransitionType & 2) != 0;

	internal bool exit => (m_TransitionType & 4) != 0;

	/// <summary>
	///   <para>Does name match the name of the active Transition.</para>
	/// </summary>
	/// <param name="name"></param>
	public bool IsName(string name)
	{
		return Animator.StringToHash(name) == m_Name || Animator.StringToHash(name) == m_FullPath;
	}

	/// <summary>
	///   <para>Does userName match the name of the active Transition.</para>
	/// </summary>
	/// <param name="name"></param>
	public bool IsUserName(string name)
	{
		return Animator.StringToHash(name) == m_UserName;
	}
}
