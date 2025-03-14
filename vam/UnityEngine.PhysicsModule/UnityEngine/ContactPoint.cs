using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Describes a contact point where the collision occurs.</para>
/// </summary>
[UsedByNativeCode]
public struct ContactPoint
{
	internal Vector3 m_Point;

	internal Vector3 m_Normal;

	internal int m_ThisColliderInstanceID;

	internal int m_OtherColliderInstanceID;

	internal float m_Separation;

	/// <summary>
	///   <para>The point of contact.</para>
	/// </summary>
	public Vector3 point => m_Point;

	/// <summary>
	///   <para>Normal of the contact point.</para>
	/// </summary>
	public Vector3 normal => m_Normal;

	/// <summary>
	///   <para>The first collider in contact at the point.</para>
	/// </summary>
	public Collider thisCollider => Object.FindObjectFromInstanceID(m_ThisColliderInstanceID) as Collider;

	/// <summary>
	///   <para>The other collider in contact at the point.</para>
	/// </summary>
	public Collider otherCollider => Object.FindObjectFromInstanceID(m_OtherColliderInstanceID) as Collider;

	/// <summary>
	///   <para>The distance between the colliders at the contact point.</para>
	/// </summary>
	public float separation => m_Separation;
}
