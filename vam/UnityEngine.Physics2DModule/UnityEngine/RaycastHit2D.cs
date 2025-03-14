using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Information returned about an object detected by a raycast in 2D physics.</para>
/// </summary>
[UsedByNativeCode]
public struct RaycastHit2D
{
	private Vector2 m_Centroid;

	private Vector2 m_Point;

	private Vector2 m_Normal;

	private float m_Distance;

	private float m_Fraction;

	private int m_Collider;

	/// <summary>
	///   <para>The centroid of the primitive used to perform the cast.</para>
	/// </summary>
	public Vector2 centroid
	{
		get
		{
			return m_Centroid;
		}
		set
		{
			m_Centroid = value;
		}
	}

	/// <summary>
	///   <para>The point in world space where the ray hit the collider's surface.</para>
	/// </summary>
	public Vector2 point
	{
		get
		{
			return m_Point;
		}
		set
		{
			m_Point = value;
		}
	}

	/// <summary>
	///   <para>The normal vector of the surface hit by the ray.</para>
	/// </summary>
	public Vector2 normal
	{
		get
		{
			return m_Normal;
		}
		set
		{
			m_Normal = value;
		}
	}

	/// <summary>
	///   <para>The distance from the ray origin to the impact point.</para>
	/// </summary>
	public float distance
	{
		get
		{
			return m_Distance;
		}
		set
		{
			m_Distance = value;
		}
	}

	/// <summary>
	///   <para>Fraction of the distance along the ray that the hit occurred.</para>
	/// </summary>
	public float fraction
	{
		get
		{
			return m_Fraction;
		}
		set
		{
			m_Fraction = value;
		}
	}

	/// <summary>
	///   <para>The collider hit by the ray.</para>
	/// </summary>
	public Collider2D collider => Object.FindObjectFromInstanceID(m_Collider) as Collider2D;

	/// <summary>
	///   <para>The Rigidbody2D attached to the object that was hit.</para>
	/// </summary>
	public Rigidbody2D rigidbody => (!(collider != null)) ? null : collider.attachedRigidbody;

	/// <summary>
	///   <para>The Transform of the object that was hit.</para>
	/// </summary>
	public Transform transform
	{
		get
		{
			Rigidbody2D rigidbody2D = rigidbody;
			if (rigidbody2D != null)
			{
				return rigidbody2D.transform;
			}
			if (collider != null)
			{
				return collider.transform;
			}
			return null;
		}
	}

	public static implicit operator bool(RaycastHit2D hit)
	{
		return hit.collider != null;
	}

	public int CompareTo(RaycastHit2D other)
	{
		if (collider == null)
		{
			return 1;
		}
		if (other.collider == null)
		{
			return -1;
		}
		return fraction.CompareTo(other.fraction);
	}
}
