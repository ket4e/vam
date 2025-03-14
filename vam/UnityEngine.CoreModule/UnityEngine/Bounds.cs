using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Represents an axis aligned bounding box.</para>
/// </summary>
[UsedByNativeCode]
[NativeType(Header = "Runtime/Geometry/AABB.h")]
[NativeHeader("Runtime/Geometry/Ray.h")]
[NativeHeader("Runtime/Geometry/Intersection.h")]
[ThreadAndSerializationSafe]
[NativeHeader("Runtime/Math/MathScripting.h")]
public struct Bounds
{
	private Vector3 m_Center;

	private Vector3 m_Extents;

	/// <summary>
	///   <para>The center of the bounding box.</para>
	/// </summary>
	public Vector3 center
	{
		get
		{
			return m_Center;
		}
		set
		{
			m_Center = value;
		}
	}

	/// <summary>
	///   <para>The total size of the box. This is always twice as large as the extents.</para>
	/// </summary>
	public Vector3 size
	{
		get
		{
			return m_Extents * 2f;
		}
		set
		{
			m_Extents = value * 0.5f;
		}
	}

	/// <summary>
	///   <para>The extents of the Bounding Box. This is always half of the size of the Bounds.</para>
	/// </summary>
	public Vector3 extents
	{
		get
		{
			return m_Extents;
		}
		set
		{
			m_Extents = value;
		}
	}

	/// <summary>
	///   <para>The minimal point of the box. This is always equal to center-extents.</para>
	/// </summary>
	public Vector3 min
	{
		get
		{
			return center - extents;
		}
		set
		{
			SetMinMax(value, max);
		}
	}

	/// <summary>
	///   <para>The maximal point of the box. This is always equal to center+extents.</para>
	/// </summary>
	public Vector3 max
	{
		get
		{
			return center + extents;
		}
		set
		{
			SetMinMax(min, value);
		}
	}

	/// <summary>
	///   <para>Creates a new Bounds.</para>
	/// </summary>
	/// <param name="center">The location of the origin of the Bounds.</param>
	/// <param name="size">The dimensions of the Bounds.</param>
	public Bounds(Vector3 center, Vector3 size)
	{
		m_Center = center;
		m_Extents = size * 0.5f;
	}

	public override int GetHashCode()
	{
		return center.GetHashCode() ^ (extents.GetHashCode() << 2);
	}

	public override bool Equals(object other)
	{
		if (!(other is Bounds bounds))
		{
			return false;
		}
		return center.Equals(bounds.center) && extents.Equals(bounds.extents);
	}

	public static bool operator ==(Bounds lhs, Bounds rhs)
	{
		return lhs.center == rhs.center && lhs.extents == rhs.extents;
	}

	public static bool operator !=(Bounds lhs, Bounds rhs)
	{
		return !(lhs == rhs);
	}

	/// <summary>
	///   <para>Sets the bounds to the min and max value of the box.</para>
	/// </summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	public void SetMinMax(Vector3 min, Vector3 max)
	{
		extents = (max - min) * 0.5f;
		center = min + extents;
	}

	/// <summary>
	///   <para>Grows the Bounds to include the point.</para>
	/// </summary>
	/// <param name="point"></param>
	public void Encapsulate(Vector3 point)
	{
		SetMinMax(Vector3.Min(min, point), Vector3.Max(max, point));
	}

	/// <summary>
	///   <para>Grow the bounds to encapsulate the bounds.</para>
	/// </summary>
	/// <param name="bounds"></param>
	public void Encapsulate(Bounds bounds)
	{
		Encapsulate(bounds.center - bounds.extents);
		Encapsulate(bounds.center + bounds.extents);
	}

	/// <summary>
	///   <para>Expand the bounds by increasing its size by amount along each side.</para>
	/// </summary>
	/// <param name="amount"></param>
	public void Expand(float amount)
	{
		amount *= 0.5f;
		extents += new Vector3(amount, amount, amount);
	}

	/// <summary>
	///   <para>Expand the bounds by increasing its size by amount along each side.</para>
	/// </summary>
	/// <param name="amount"></param>
	public void Expand(Vector3 amount)
	{
		extents += amount * 0.5f;
	}

	/// <summary>
	///   <para>Does another bounding box intersect with this bounding box?</para>
	/// </summary>
	/// <param name="bounds"></param>
	public bool Intersects(Bounds bounds)
	{
		return min.x <= bounds.max.x && max.x >= bounds.min.x && min.y <= bounds.max.y && max.y >= bounds.min.y && min.z <= bounds.max.z && max.z >= bounds.min.z;
	}

	/// <summary>
	///   <para>Does ray intersect this bounding box?</para>
	/// </summary>
	/// <param name="ray"></param>
	public bool IntersectRay(Ray ray)
	{
		float dist;
		return IntersectRayAABB(ray, this, out dist);
	}

	public bool IntersectRay(Ray ray, out float distance)
	{
		return IntersectRayAABB(ray, this, out distance);
	}

	/// <summary>
	///   <para>Returns a nicely formatted string for the bounds.</para>
	/// </summary>
	/// <param name="format"></param>
	public override string ToString()
	{
		return UnityString.Format("Center: {0}, Extents: {1}", m_Center, m_Extents);
	}

	/// <summary>
	///   <para>Returns a nicely formatted string for the bounds.</para>
	/// </summary>
	/// <param name="format"></param>
	public string ToString(string format)
	{
		return UnityString.Format("Center: {0}, Extents: {1}", m_Center.ToString(format), m_Extents.ToString(format));
	}

	/// <summary>
	///   <para>Is point contained in the bounding box?</para>
	/// </summary>
	/// <param name="point"></param>
	[NativeMethod("IsInside", IsThreadSafe = true)]
	public bool Contains(Vector3 point)
	{
		return Contains_Injected(ref this, ref point);
	}

	/// <summary>
	///   <para>The smallest squared distance between the point and this bounding box.</para>
	/// </summary>
	/// <param name="point"></param>
	[FreeFunction("BoundsScripting::SqrDistance", HasExplicitThis = true, IsThreadSafe = true)]
	public float SqrDistance(Vector3 point)
	{
		return SqrDistance_Injected(ref this, ref point);
	}

	[FreeFunction("IntersectRayAABB", IsThreadSafe = true)]
	private static bool IntersectRayAABB(Ray ray, Bounds bounds, out float dist)
	{
		return IntersectRayAABB_Injected(ref ray, ref bounds, out dist);
	}

	/// <summary>
	///   <para>The closest point on the bounding box.</para>
	/// </summary>
	/// <param name="point">Arbitrary point.</param>
	/// <returns>
	///   <para>The point on the bounding box or inside the bounding box.</para>
	/// </returns>
	[FreeFunction("BoundsScripting::ClosestPoint", HasExplicitThis = true, IsThreadSafe = true)]
	public Vector3 ClosestPoint(Vector3 point)
	{
		ClosestPoint_Injected(ref this, ref point, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Contains_Injected(ref Bounds _unity_self, ref Vector3 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SqrDistance_Injected(ref Bounds _unity_self, ref Vector3 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IntersectRayAABB_Injected(ref Ray ray, ref Bounds bounds, out float dist);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClosestPoint_Injected(ref Bounds _unity_self, ref Vector3 point, out Vector3 ret);
}
