using System;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>A set of parameters for filtering contact results.</para>
/// </summary>
[Serializable]
public struct ContactFilter2D
{
	/// <summary>
	///   <para>Sets to filter contact results based on trigger collider involvement.</para>
	/// </summary>
	[NativeName("m_UseTriggers")]
	public bool useTriggers;

	/// <summary>
	///   <para>Sets the contact filter to filter results by layer mask.</para>
	/// </summary>
	[NativeName("m_UseLayerMask")]
	public bool useLayerMask;

	/// <summary>
	///   <para>Sets the contact filter to filter the results by depth using minDepth and maxDepth.</para>
	/// </summary>
	[NativeName("m_UseDepth")]
	public bool useDepth;

	/// <summary>
	///   <para>Sets the contact filter to filter within the minDepth and maxDepth range, or outside that range.</para>
	/// </summary>
	[NativeName("m_UseOutsideDepth")]
	public bool useOutsideDepth;

	/// <summary>
	///   <para>Sets the contact filter to filter the results by the collision's normal angle using minNormalAngle and maxNormalAngle.</para>
	/// </summary>
	[NativeName("m_UseNormalAngle")]
	public bool useNormalAngle;

	/// <summary>
	///   <para>Sets the contact filter to filter within the minNormalAngle and maxNormalAngle range, or outside that range.</para>
	/// </summary>
	[NativeName("m_UseOutsideNormalAngle")]
	public bool useOutsideNormalAngle;

	/// <summary>
	///   <para>Sets the contact filter to filter the results that only include Collider2D on the layers defined by the layer mask.</para>
	/// </summary>
	[NativeName("m_LayerMask")]
	public LayerMask layerMask;

	/// <summary>
	///   <para>Sets the contact filter to filter the results to only include Collider2D with a Z coordinate (depth) greater than this value.</para>
	/// </summary>
	[NativeName("m_MinDepth")]
	public float minDepth;

	/// <summary>
	///   <para>Sets the contact filter to filter the results to only include Collider2D with a Z coordinate (depth) less than this value.</para>
	/// </summary>
	[NativeName("m_MaxDepth")]
	public float maxDepth;

	/// <summary>
	///   <para>Sets the contact filter to filter the results to only include contacts with collision normal angles that are greater than this angle.</para>
	/// </summary>
	[NativeName("m_MinNormalAngle")]
	public float minNormalAngle;

	/// <summary>
	///   <para>Sets the contact filter to filter the results to only include contacts with collision normal angles that are less than this angle.</para>
	/// </summary>
	[NativeName("m_MaxNormalAngle")]
	public float maxNormalAngle;

	public const float NormalAngleUpperLimit = 359.9999f;

	/// <summary>
	///   <para>Given the current state of the contact filter, determine whether it would filter anything.</para>
	/// </summary>
	public bool isFiltering => !useTriggers || useLayerMask || useDepth || useNormalAngle;

	/// <summary>
	///   <para>Sets the contact filter to not filter any ContactPoint2D.</para>
	/// </summary>
	/// <returns>
	///   <para>A copy of the contact filter set to not filter any ContactPoint2D.</para>
	/// </returns>
	public ContactFilter2D NoFilter()
	{
		useTriggers = true;
		useLayerMask = false;
		layerMask = -1;
		useDepth = false;
		useOutsideDepth = false;
		minDepth = float.NegativeInfinity;
		maxDepth = float.PositiveInfinity;
		useNormalAngle = false;
		useOutsideNormalAngle = false;
		minNormalAngle = 0f;
		maxNormalAngle = 359.9999f;
		return this;
	}

	private void CheckConsistency()
	{
		minDepth = ((minDepth != float.NegativeInfinity && minDepth != float.PositiveInfinity && !float.IsNaN(minDepth)) ? minDepth : float.MinValue);
		maxDepth = ((maxDepth != float.NegativeInfinity && maxDepth != float.PositiveInfinity && !float.IsNaN(maxDepth)) ? maxDepth : float.MaxValue);
		if (minDepth > maxDepth)
		{
			float num = minDepth;
			minDepth = maxDepth;
			maxDepth = num;
		}
		minNormalAngle = ((!float.IsNaN(minNormalAngle)) ? Mathf.Clamp(minNormalAngle, 0f, 359.9999f) : 0f);
		maxNormalAngle = ((!float.IsNaN(maxNormalAngle)) ? Mathf.Clamp(maxNormalAngle, 0f, 359.9999f) : 359.9999f);
		if (minNormalAngle > maxNormalAngle)
		{
			float num2 = minNormalAngle;
			minNormalAngle = maxNormalAngle;
			maxNormalAngle = num2;
		}
	}

	/// <summary>
	///   <para>Turns off layer mask filtering by setting useLayerMask to false.  The associated value of layerMask is not changed.</para>
	/// </summary>
	public void ClearLayerMask()
	{
		useLayerMask = false;
	}

	/// <summary>
	///   <para>Sets the layerMask filter property using the layerMask parameter provided and also enables layer mask filtering by setting useLayerMask to true.</para>
	/// </summary>
	/// <param name="layerMask">The value used to set the layerMask.</param>
	public void SetLayerMask(LayerMask layerMask)
	{
		this.layerMask = layerMask;
		useLayerMask = true;
	}

	/// <summary>
	///   <para>Turns off depth filtering by setting useDepth to false.  The associated values of minDepth and maxDepth are not changed.</para>
	/// </summary>
	public void ClearDepth()
	{
		useDepth = false;
	}

	/// <summary>
	///   <para>Sets the minDepth and maxDepth filter properties and turns on depth filtering by setting useDepth to true.</para>
	/// </summary>
	/// <param name="minDepth">The value used to set minDepth.</param>
	/// <param name="maxDepth">The value used to set maxDepth.</param>
	public void SetDepth(float minDepth, float maxDepth)
	{
		this.minDepth = minDepth;
		this.maxDepth = maxDepth;
		useDepth = true;
		CheckConsistency();
	}

	/// <summary>
	///   <para>Turns off normal angle filtering by setting useNormalAngle to false. The associated values of minNormalAngle and maxNormalAngle are not changed.</para>
	/// </summary>
	public void ClearNormalAngle()
	{
		useNormalAngle = false;
	}

	/// <summary>
	///   <para>Sets the minNormalAngle and maxNormalAngle filter properties and turns on normal angle filtering by setting useNormalAngle to true.</para>
	/// </summary>
	/// <param name="minNormalAngle">The value used to set the minNormalAngle.</param>
	/// <param name="maxNormalAngle">The value used to set the maxNormalAngle.</param>
	public void SetNormalAngle(float minNormalAngle, float maxNormalAngle)
	{
		this.minNormalAngle = minNormalAngle;
		this.maxNormalAngle = maxNormalAngle;
		useNormalAngle = true;
		CheckConsistency();
	}

	/// <summary>
	///   <para>Checks if the collider is a trigger and should be filtered by the useTriggers to be filtered.</para>
	/// </summary>
	/// <param name="collider">The Collider2D used to check for a trigger.</param>
	/// <returns>
	///   <para>Returns true when collider is excluded by the filter and false if otherwise.</para>
	/// </returns>
	public bool IsFilteringTrigger([Writable] Collider2D collider)
	{
		return !useTriggers && collider.isTrigger;
	}

	/// <summary>
	///   <para>Checks if the GameObject.layer for obj is included in the layerMask to be filtered.</para>
	/// </summary>
	/// <param name="obj">The GameObject used to check the GameObject.layer.</param>
	/// <returns>
	///   <para>Returns true when obj is excluded by the filter and false if otherwise.</para>
	/// </returns>
	public bool IsFilteringLayerMask(GameObject obj)
	{
		return useLayerMask && ((int)layerMask & (1 << obj.layer)) == 0;
	}

	/// <summary>
	///   <para>Checks if the Transform for obj is within the depth range to be filtered.</para>
	/// </summary>
	/// <param name="obj">The GameObject used to check the z-position (depth) of Transform.position.</param>
	/// <returns>
	///   <para>Returns true when obj is excluded by the filter and false if otherwise.</para>
	/// </returns>
	public bool IsFilteringDepth(GameObject obj)
	{
		if (!useDepth)
		{
			return false;
		}
		if (minDepth > maxDepth)
		{
			float num = minDepth;
			minDepth = maxDepth;
			maxDepth = num;
		}
		float z = obj.transform.position.z;
		bool flag = z < minDepth || z > maxDepth;
		if (useOutsideDepth)
		{
			return !flag;
		}
		return flag;
	}

	/// <summary>
	///   <para>Checks if the angle of normal is within the normal angle range to be filtered.</para>
	/// </summary>
	/// <param name="normal">The normal used to calculate an angle.</param>
	/// <returns>
	///   <para>Returns true when normal is excluded by the filter and false if otherwise.</para>
	/// </returns>
	public bool IsFilteringNormalAngle(Vector2 normal)
	{
		float angle = Mathf.Atan2(normal.y, normal.x) * 57.29578f;
		return IsFilteringNormalAngle(angle);
	}

	/// <summary>
	///   <para>Checks if the angle is within the normal angle range to be filtered.</para>
	/// </summary>
	/// <param name="angle">The angle used for comparison in the filter.</param>
	/// <returns>
	///   <para>Returns true when angle is excluded by the filter and false if otherwise.</para>
	/// </returns>
	public bool IsFilteringNormalAngle(float angle)
	{
		angle -= Mathf.Floor(angle / 359.9999f) * 359.9999f;
		float num = Mathf.Clamp(minNormalAngle, 0f, 359.9999f);
		float num2 = Mathf.Clamp(maxNormalAngle, 0f, 359.9999f);
		if (num > num2)
		{
			float num3 = num;
			num = num2;
			num2 = num3;
		}
		bool flag = angle < num || angle > num2;
		if (useOutsideNormalAngle)
		{
			return !flag;
		}
		return flag;
	}

	internal static ContactFilter2D CreateLegacyFilter(int layerMask, float minDepth, float maxDepth)
	{
		ContactFilter2D result = default(ContactFilter2D);
		result.useTriggers = Physics2D.queriesHitTriggers;
		result.SetLayerMask(layerMask);
		result.SetDepth(minDepth, maxDepth);
		return result;
	}
}
