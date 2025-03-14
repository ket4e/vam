using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Use this struct to specify the position and rotation weight mask for Animator.MatchTarget.</para>
/// </summary>
[NativeHeader("Runtime/Animation/Animator.h")]
public struct MatchTargetWeightMask
{
	private Vector3 m_PositionXYZWeight;

	private float m_RotationWeight;

	/// <summary>
	///   <para>Position XYZ weight.</para>
	/// </summary>
	public Vector3 positionXYZWeight
	{
		get
		{
			return m_PositionXYZWeight;
		}
		set
		{
			m_PositionXYZWeight = value;
		}
	}

	/// <summary>
	///   <para>Rotation weight.</para>
	/// </summary>
	public float rotationWeight
	{
		get
		{
			return m_RotationWeight;
		}
		set
		{
			m_RotationWeight = value;
		}
	}

	/// <summary>
	///   <para>MatchTargetWeightMask contructor.</para>
	/// </summary>
	/// <param name="positionXYZWeight">Position XYZ weight.</param>
	/// <param name="rotationWeight">Rotation weight.</param>
	public MatchTargetWeightMask(Vector3 positionXYZWeight, float rotationWeight)
	{
		m_PositionXYZWeight = positionXYZWeight;
		m_RotationWeight = rotationWeight;
	}
}
