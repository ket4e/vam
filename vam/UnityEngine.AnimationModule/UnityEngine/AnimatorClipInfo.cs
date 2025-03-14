using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Information about clip being played and blended by the Animator.</para>
/// </summary>
[UsedByNativeCode]
[NativeHeader("Runtime/Animation/AnimatorInfo.h")]
[NativeHeader("Runtime/Animation/ScriptBindings/Animation.bindings.h")]
public struct AnimatorClipInfo
{
	private int m_ClipInstanceID;

	private float m_Weight;

	/// <summary>
	///   <para>Returns the animation clip played by the Animator.</para>
	/// </summary>
	public AnimationClip clip => (m_ClipInstanceID == 0) ? null : InstanceIDToAnimationClipPPtr(m_ClipInstanceID);

	/// <summary>
	///   <para>Returns the blending weight used by the Animator to blend this clip.</para>
	/// </summary>
	public float weight => m_Weight;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("AnimationBindings::InstanceIDToAnimationClipPPtr")]
	private static extern AnimationClip InstanceIDToAnimationClipPPtr(int instanceID);
}
