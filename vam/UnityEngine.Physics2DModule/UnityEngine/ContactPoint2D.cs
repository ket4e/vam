using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Details about a specific point of contact involved in a 2D physics collision.</para>
/// </summary>
[UsedByNativeCode]
public struct ContactPoint2D
{
	private Vector2 m_Point;

	private Vector2 m_Normal;

	private Vector2 m_RelativeVelocity;

	private float m_Separation;

	private float m_NormalImpulse;

	private float m_TangentImpulse;

	private int m_Collider;

	private int m_OtherCollider;

	private int m_Rigidbody;

	private int m_OtherRigidbody;

	private int m_Enabled;

	/// <summary>
	///   <para>The point of contact between the two colliders in world space.</para>
	/// </summary>
	public Vector2 point => m_Point;

	/// <summary>
	///   <para>Surface normal at the contact point.</para>
	/// </summary>
	public Vector2 normal => m_Normal;

	/// <summary>
	///   <para>Gets the distance between the colliders at the contact point.</para>
	/// </summary>
	public float separation => m_Separation;

	/// <summary>
	///   <para>Gets the impulse force applied at the contact point along the ContactPoint2D.normal.</para>
	/// </summary>
	public float normalImpulse => m_NormalImpulse;

	/// <summary>
	///   <para>Gets the impulse force applied at the contact point which is perpendicular to the ContactPoint2D.normal.</para>
	/// </summary>
	public float tangentImpulse => m_TangentImpulse;

	/// <summary>
	///   <para>Gets the relative velocity of the two colliders at the contact point (Read Only).</para>
	/// </summary>
	public Vector2 relativeVelocity => m_RelativeVelocity;

	/// <summary>
	///   <para>The incoming Collider2D involved in the collision with the otherCollider.</para>
	/// </summary>
	public Collider2D collider => Object.FindObjectFromInstanceID(m_Collider) as Collider2D;

	/// <summary>
	///   <para>The other Collider2D involved in the collision with the collider.</para>
	/// </summary>
	public Collider2D otherCollider => Object.FindObjectFromInstanceID(m_OtherCollider) as Collider2D;

	/// <summary>
	///   <para>The incoming Rigidbody2D involved in the collision with the otherRigidbody.</para>
	/// </summary>
	public Rigidbody2D rigidbody => Object.FindObjectFromInstanceID(m_Rigidbody) as Rigidbody2D;

	/// <summary>
	///   <para>The other Rigidbody2D involved in the collision with the rigidbody.</para>
	/// </summary>
	public Rigidbody2D otherRigidbody => Object.FindObjectFromInstanceID(m_OtherRigidbody) as Rigidbody2D;

	/// <summary>
	///   <para>Indicates whether the collision response or reaction is enabled or disabled.</para>
	/// </summary>
	public bool enabled => m_Enabled == 1;
}
