using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Collision details returned by 2D physics callback functions.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[UsedByNativeCode]
public class Collision2D
{
	internal int m_Collider;

	internal int m_OtherCollider;

	internal int m_Rigidbody;

	internal int m_OtherRigidbody;

	internal Vector2 m_RelativeVelocity;

	internal int m_Enabled;

	internal int m_ContactCount;

	internal CachedContactPoints2D m_CachedContactPoints;

	internal ContactPoint2D[] m_LegacyContactArray;

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
	///   <para>The Transform of the incoming object involved in the collision.</para>
	/// </summary>
	public Transform transform => (!(rigidbody != null)) ? collider.transform : rigidbody.transform;

	/// <summary>
	///   <para>The incoming GameObject involved in the collision.</para>
	/// </summary>
	public GameObject gameObject => (!(rigidbody != null)) ? collider.gameObject : rigidbody.gameObject;

	/// <summary>
	///   <para>The relative linear velocity of the two colliding objects (Read Only).</para>
	/// </summary>
	public Vector2 relativeVelocity => m_RelativeVelocity;

	/// <summary>
	///   <para>Indicates whether the collision response or reaction is enabled or disabled.</para>
	/// </summary>
	public bool enabled => m_Enabled == 1;

	/// <summary>
	///   <para>The specific points of contact with the incoming Collider2D.  You should avoid using this as it produces memory garbage.  Use GetContacts instead.</para>
	/// </summary>
	public ContactPoint2D[] contacts
	{
		get
		{
			if (m_LegacyContactArray == null)
			{
				m_LegacyContactArray = new ContactPoint2D[m_ContactCount];
				if (m_ContactCount > 0)
				{
					for (int i = 0; i < m_ContactCount; i++)
					{
						ref ContactPoint2D reference = ref m_LegacyContactArray[i];
						reference = m_CachedContactPoints[i];
					}
				}
			}
			return m_LegacyContactArray;
		}
	}

	/// <summary>
	///   <para>Retrieves all contact points in for contacts between collider and otherCollider.</para>
	/// </summary>
	/// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
	/// <returns>
	///   <para>Returns the number of contacts placed in the contacts array.</para>
	/// </returns>
	public int GetContacts(ContactPoint2D[] contacts)
	{
		if (contacts == null)
		{
			throw new ArgumentNullException("Cannot get contacts into a NULL array.");
		}
		int num = Mathf.Min(contacts.Length, m_ContactCount);
		if (num == 0)
		{
			return 0;
		}
		if (m_LegacyContactArray != null)
		{
			Array.Copy(m_LegacyContactArray, contacts, num);
			return num;
		}
		if (m_ContactCount > 0)
		{
			for (int i = 0; i < num; i++)
			{
				ref ContactPoint2D reference = ref contacts[i];
				reference = m_CachedContactPoints[i];
			}
		}
		return num;
	}
}
