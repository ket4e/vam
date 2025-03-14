using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>ControllerColliderHit is used by CharacterController.OnControllerColliderHit to give detailed information about the collision and how to deal with it.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
public class ControllerColliderHit
{
	internal CharacterController m_Controller;

	internal Collider m_Collider;

	internal Vector3 m_Point;

	internal Vector3 m_Normal;

	internal Vector3 m_MoveDirection;

	internal float m_MoveLength;

	internal int m_Push;

	/// <summary>
	///   <para>The controller that hit the collider.</para>
	/// </summary>
	public CharacterController controller => m_Controller;

	/// <summary>
	///   <para>The collider that was hit by the controller.</para>
	/// </summary>
	public Collider collider => m_Collider;

	/// <summary>
	///   <para>The rigidbody that was hit by the controller.</para>
	/// </summary>
	public Rigidbody rigidbody => m_Collider.attachedRigidbody;

	/// <summary>
	///   <para>The game object that was hit by the controller.</para>
	/// </summary>
	public GameObject gameObject => m_Collider.gameObject;

	/// <summary>
	///   <para>The transform that was hit by the controller.</para>
	/// </summary>
	public Transform transform => m_Collider.transform;

	/// <summary>
	///   <para>The impact point in world space.</para>
	/// </summary>
	public Vector3 point => m_Point;

	/// <summary>
	///   <para>The normal of the surface we collided with in world space.</para>
	/// </summary>
	public Vector3 normal => m_Normal;

	/// <summary>
	///   <para>The direction the CharacterController was moving in when the collision occured.</para>
	/// </summary>
	public Vector3 moveDirection => m_MoveDirection;

	/// <summary>
	///   <para>How far the character has travelled until it hit the collider.</para>
	/// </summary>
	public float moveLength => m_MoveLength;

	private bool push
	{
		get
		{
			return m_Push != 0;
		}
		set
		{
			m_Push = (value ? 1 : 0);
		}
	}
}
