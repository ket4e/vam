using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A pair of SphereColliders used to define shapes for Cloth objects to collide against.</para>
/// </summary>
[UsedByNativeCode]
public struct ClothSphereColliderPair
{
	private SphereCollider m_First;

	private SphereCollider m_Second;

	/// <summary>
	///   <para>The first SphereCollider of a ClothSphereColliderPair.</para>
	/// </summary>
	public SphereCollider first
	{
		get
		{
			return m_First;
		}
		set
		{
			m_First = value;
		}
	}

	/// <summary>
	///   <para>The second SphereCollider of a ClothSphereColliderPair.</para>
	/// </summary>
	public SphereCollider second
	{
		get
		{
			return m_Second;
		}
		set
		{
			m_Second = value;
		}
	}

	/// <summary>
	///   <para>Creates a ClothSphereColliderPair. If only one SphereCollider is given, the ClothSphereColliderPair will define a simple sphere. If two SphereColliders are given, the ClothSphereColliderPair defines a conic capsule shape, composed of the two spheres and the cone connecting the two.</para>
	/// </summary>
	/// <param name="a">The first SphereCollider of a ClothSphereColliderPair.</param>
	/// <param name="b">The second SphereCollider of a ClothSphereColliderPair.</param>
	public ClothSphereColliderPair(SphereCollider a)
	{
		m_First = null;
		m_Second = null;
		first = a;
		second = null;
	}

	/// <summary>
	///   <para>Creates a ClothSphereColliderPair. If only one SphereCollider is given, the ClothSphereColliderPair will define a simple sphere. If two SphereColliders are given, the ClothSphereColliderPair defines a conic capsule shape, composed of the two spheres and the cone connecting the two.</para>
	/// </summary>
	/// <param name="a">The first SphereCollider of a ClothSphereColliderPair.</param>
	/// <param name="b">The second SphereCollider of a ClothSphereColliderPair.</param>
	public ClothSphereColliderPair(SphereCollider a, SphereCollider b)
	{
		m_First = null;
		m_Second = null;
		first = a;
		second = b;
	}
}
