using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Information about a particle collision.</para>
/// </summary>
[RequiredByNativeCode(Optional = true)]
public struct ParticleCollisionEvent
{
	private Vector3 m_Intersection;

	private Vector3 m_Normal;

	private Vector3 m_Velocity;

	private int m_ColliderInstanceID;

	/// <summary>
	///   <para>Intersection point of the collision in world coordinates.</para>
	/// </summary>
	public Vector3 intersection => m_Intersection;

	/// <summary>
	///   <para>Geometry normal at the intersection point of the collision.</para>
	/// </summary>
	public Vector3 normal => m_Normal;

	/// <summary>
	///   <para>Incident velocity at the intersection point of the collision.</para>
	/// </summary>
	public Vector3 velocity => m_Velocity;

	/// <summary>
	///   <para>The Collider or Collider2D for the GameObject struck by the particles.</para>
	/// </summary>
	public Component colliderComponent => InstanceIDToColliderComponent(m_ColliderInstanceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern Component InstanceIDToColliderComponent(int instanceID);
}
