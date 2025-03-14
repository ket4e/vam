using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Structure used to get information back from a raycast.</para>
/// </summary>
[UsedByNativeCode]
public struct RaycastHit
{
	private Vector3 m_Point;

	private Vector3 m_Normal;

	private int m_FaceID;

	private float m_Distance;

	private Vector2 m_UV;

	private int m_Collider;

	/// <summary>
	///   <para>The impact point in world space where the ray hit the collider.</para>
	/// </summary>
	public Vector3 point
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
	///   <para>The normal of the surface the ray hit.</para>
	/// </summary>
	public Vector3 normal
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
	///   <para>The barycentric coordinate of the triangle we hit.</para>
	/// </summary>
	public Vector3 barycentricCoordinate
	{
		get
		{
			return new Vector3(1f - (m_UV.y + m_UV.x), m_UV.x, m_UV.y);
		}
		set
		{
			m_UV = value;
		}
	}

	/// <summary>
	///   <para>The distance from the ray's origin to the impact point.</para>
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
	///   <para>The index of the triangle that was hit.</para>
	/// </summary>
	public int triangleIndex => m_FaceID;

	/// <summary>
	///   <para>The uv texture coordinate at the collision location.</para>
	/// </summary>
	public Vector2 textureCoord
	{
		get
		{
			CalculateRaycastTexCoord(out var output, collider, m_UV, m_Point, m_FaceID, 0);
			return output;
		}
	}

	/// <summary>
	///   <para>The secondary uv texture coordinate at the impact point.</para>
	/// </summary>
	public Vector2 textureCoord2
	{
		get
		{
			CalculateRaycastTexCoord(out var output, collider, m_UV, m_Point, m_FaceID, 1);
			return output;
		}
	}

	[Obsolete("Use textureCoord2 instead")]
	public Vector2 textureCoord1
	{
		get
		{
			CalculateRaycastTexCoord(out var output, collider, m_UV, m_Point, m_FaceID, 1);
			return output;
		}
	}

	/// <summary>
	///   <para>The uv lightmap coordinate at the impact point.</para>
	/// </summary>
	public Vector2 lightmapCoord
	{
		get
		{
			CalculateRaycastTexCoord(out var output, collider, m_UV, m_Point, m_FaceID, 1);
			if (collider.GetComponent<Renderer>() != null)
			{
				Vector4 lightmapScaleOffset = collider.GetComponent<Renderer>().lightmapScaleOffset;
				output.x = output.x * lightmapScaleOffset.x + lightmapScaleOffset.z;
				output.y = output.y * lightmapScaleOffset.y + lightmapScaleOffset.w;
			}
			return output;
		}
	}

	/// <summary>
	///   <para>The Collider that was hit.</para>
	/// </summary>
	public Collider collider => Object.FindObjectFromInstanceID(m_Collider) as Collider;

	/// <summary>
	///   <para>The Rigidbody of the collider that was hit. If the collider is not attached to a rigidbody then it is null.</para>
	/// </summary>
	public Rigidbody rigidbody => (!(collider != null)) ? null : collider.attachedRigidbody;

	/// <summary>
	///   <para>The Transform of the rigidbody or collider that was hit.</para>
	/// </summary>
	public Transform transform
	{
		get
		{
			Rigidbody rigidbody = this.rigidbody;
			if (rigidbody != null)
			{
				return rigidbody.transform;
			}
			if (collider != null)
			{
				return collider.transform;
			}
			return null;
		}
	}

	private static void CalculateRaycastTexCoord(out Vector2 output, Collider col, Vector2 uv, Vector3 point, int face, int index)
	{
		INTERNAL_CALL_CalculateRaycastTexCoord(out output, col, ref uv, ref point, face, index);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_CalculateRaycastTexCoord(out Vector2 output, Collider col, ref Vector2 uv, ref Vector3 point, int face, int index);
}
