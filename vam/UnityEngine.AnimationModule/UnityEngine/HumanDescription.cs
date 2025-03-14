using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Class that holds humanoid avatar parameters to pass to the AvatarBuilder.BuildHumanAvatar function.</para>
/// </summary>
[NativeHeader("Runtime/Animation/ScriptBindings/AvatarBuilder.bindings.h")]
[NativeHeader("Runtime/Animation/HumanDescription.h")]
public struct HumanDescription
{
	/// <summary>
	///   <para>Mapping between Mecanim bone names and bone names in the rig.</para>
	/// </summary>
	[NativeName("m_Human")]
	public HumanBone[] human;

	/// <summary>
	///   <para>List of bone Transforms to include in the model.</para>
	/// </summary>
	[NativeName("m_Skeleton")]
	public SkeletonBone[] skeleton;

	internal float m_ArmTwist;

	internal float m_ForeArmTwist;

	internal float m_UpperLegTwist;

	internal float m_LegTwist;

	internal float m_ArmStretch;

	internal float m_LegStretch;

	internal float m_FeetSpacing;

	internal string m_RootMotionBoneName;

	internal Quaternion m_RootMotionBoneRotation;

	internal bool m_HasTranslationDoF;

	internal bool m_HasExtraRoot;

	internal bool m_SkeletonHasParents;

	/// <summary>
	///   <para>Defines how the upper arm's roll/twisting is distributed between the shoulder and elbow joints.</para>
	/// </summary>
	public float upperArmTwist
	{
		get
		{
			return m_ArmTwist;
		}
		set
		{
			m_ArmTwist = value;
		}
	}

	/// <summary>
	///   <para>Defines how the lower arm's roll/twisting is distributed between the elbow and wrist joints.</para>
	/// </summary>
	public float lowerArmTwist
	{
		get
		{
			return m_ForeArmTwist;
		}
		set
		{
			m_ForeArmTwist = value;
		}
	}

	/// <summary>
	///   <para>Defines how the upper leg's roll/twisting is distributed between the thigh and knee joints.</para>
	/// </summary>
	public float upperLegTwist
	{
		get
		{
			return m_UpperLegTwist;
		}
		set
		{
			m_UpperLegTwist = value;
		}
	}

	/// <summary>
	///   <para>Defines how the lower leg's roll/twisting is distributed between the knee and ankle.</para>
	/// </summary>
	public float lowerLegTwist
	{
		get
		{
			return m_LegTwist;
		}
		set
		{
			m_LegTwist = value;
		}
	}

	/// <summary>
	///   <para>Amount by which the arm's length is allowed to stretch when using IK.</para>
	/// </summary>
	public float armStretch
	{
		get
		{
			return m_ArmStretch;
		}
		set
		{
			m_ArmStretch = value;
		}
	}

	/// <summary>
	///   <para>Amount by which the leg's length is allowed to stretch when using IK.</para>
	/// </summary>
	public float legStretch
	{
		get
		{
			return m_LegStretch;
		}
		set
		{
			m_LegStretch = value;
		}
	}

	/// <summary>
	///   <para>Modification to the minimum distance between the feet of a humanoid model.</para>
	/// </summary>
	public float feetSpacing
	{
		get
		{
			return m_FeetSpacing;
		}
		set
		{
			m_FeetSpacing = value;
		}
	}

	/// <summary>
	///   <para>True for any human that has a translation Degree of Freedom (DoF). It is set to false by default.</para>
	/// </summary>
	public bool hasTranslationDoF
	{
		get
		{
			return m_HasTranslationDoF;
		}
		set
		{
			m_HasTranslationDoF = value;
		}
	}
}
