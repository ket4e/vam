using System;

namespace UnityEngine;

/// <summary>
///   <para>Retargetable humanoid pose.</para>
/// </summary>
public struct HumanPose
{
	/// <summary>
	///   <para>The human body position for that pose.</para>
	/// </summary>
	public Vector3 bodyPosition;

	/// <summary>
	///   <para>The human body orientation for that pose.</para>
	/// </summary>
	public Quaternion bodyRotation;

	/// <summary>
	///   <para>The array of muscle values for that pose.</para>
	/// </summary>
	public float[] muscles;

	internal void Init()
	{
		if (muscles != null && muscles.Length != HumanTrait.MuscleCount)
		{
			throw new InvalidOperationException("Bad array size for HumanPose.muscles. Size must equal HumanTrait.MuscleCount");
		}
		if (muscles == null)
		{
			muscles = new float[HumanTrait.MuscleCount];
			if (bodyRotation.x == 0f && bodyRotation.y == 0f && bodyRotation.z == 0f && bodyRotation.w == 0f)
			{
				bodyRotation.w = 1f;
			}
		}
	}
}
