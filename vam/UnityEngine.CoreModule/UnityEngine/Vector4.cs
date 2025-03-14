using System;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Representation of four-dimensional vectors.</para>
/// </summary>
[UsedByNativeCode]
public struct Vector4
{
	public const float kEpsilon = 1E-05f;

	/// <summary>
	///   <para>X component of the vector.</para>
	/// </summary>
	public float x;

	/// <summary>
	///   <para>Y component of the vector.</para>
	/// </summary>
	public float y;

	/// <summary>
	///   <para>Z component of the vector.</para>
	/// </summary>
	public float z;

	/// <summary>
	///   <para>W component of the vector.</para>
	/// </summary>
	public float w;

	private static readonly Vector4 zeroVector = new Vector4(0f, 0f, 0f, 0f);

	private static readonly Vector4 oneVector = new Vector4(1f, 1f, 1f, 1f);

	private static readonly Vector4 positiveInfinityVector = new Vector4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

	private static readonly Vector4 negativeInfinityVector = new Vector4(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

	public float this[int index]
	{
		get
		{
			return index switch
			{
				0 => x, 
				1 => y, 
				2 => z, 
				3 => w, 
				_ => throw new IndexOutOfRangeException("Invalid Vector4 index!"), 
			};
		}
		set
		{
			switch (index)
			{
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
			case 2:
				z = value;
				break;
			case 3:
				w = value;
				break;
			default:
				throw new IndexOutOfRangeException("Invalid Vector4 index!");
			}
		}
	}

	/// <summary>
	///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
	/// </summary>
	public Vector4 normalized => Normalize(this);

	/// <summary>
	///   <para>Returns the length of this vector (Read Only).</para>
	/// </summary>
	public float magnitude => Mathf.Sqrt(Dot(this, this));

	/// <summary>
	///   <para>Returns the squared length of this vector (Read Only).</para>
	/// </summary>
	public float sqrMagnitude => Dot(this, this);

	/// <summary>
	///   <para>Shorthand for writing Vector4(0,0,0,0).</para>
	/// </summary>
	public static Vector4 zero => zeroVector;

	/// <summary>
	///   <para>Shorthand for writing Vector4(1,1,1,1).</para>
	/// </summary>
	public static Vector4 one => oneVector;

	/// <summary>
	///   <para>Shorthand for writing Vector4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity).</para>
	/// </summary>
	public static Vector4 positiveInfinity => positiveInfinityVector;

	/// <summary>
	///   <para>Shorthand for writing Vector4(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity).</para>
	/// </summary>
	public static Vector4 negativeInfinity => negativeInfinityVector;

	/// <summary>
	///   <para>Creates a new vector with given x, y, z, w components.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <param name="w"></param>
	public Vector4(float x, float y, float z, float w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	/// <summary>
	///   <para>Creates a new vector with given x, y, z components and sets w to zero.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public Vector4(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		w = 0f;
	}

	/// <summary>
	///   <para>Creates a new vector with given x, y components and sets z and w to zero.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public Vector4(float x, float y)
	{
		this.x = x;
		this.y = y;
		z = 0f;
		w = 0f;
	}

	/// <summary>
	///   <para>Set x, y, z and w components of an existing Vector4.</para>
	/// </summary>
	/// <param name="newX"></param>
	/// <param name="newY"></param>
	/// <param name="newZ"></param>
	/// <param name="newW"></param>
	public void Set(float newX, float newY, float newZ, float newW)
	{
		x = newX;
		y = newY;
		z = newZ;
		w = newW;
	}

	/// <summary>
	///   <para>Linearly interpolates between two vectors.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="t"></param>
	public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
	{
		t = Mathf.Clamp01(t);
		return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
	}

	/// <summary>
	///   <para>Linearly interpolates between two vectors.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="t"></param>
	public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t)
	{
		return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
	}

	/// <summary>
	///   <para>Moves a point current towards target.</para>
	/// </summary>
	/// <param name="current"></param>
	/// <param name="target"></param>
	/// <param name="maxDistanceDelta"></param>
	public static Vector4 MoveTowards(Vector4 current, Vector4 target, float maxDistanceDelta)
	{
		Vector4 vector = target - current;
		float num = vector.magnitude;
		if (num <= maxDistanceDelta || num == 0f)
		{
			return target;
		}
		return current + vector / num * maxDistanceDelta;
	}

	/// <summary>
	///   <para>Multiplies two vectors component-wise.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public static Vector4 Scale(Vector4 a, Vector4 b)
	{
		return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
	}

	/// <summary>
	///   <para>Multiplies every component of this vector by the same component of scale.</para>
	/// </summary>
	/// <param name="scale"></param>
	public void Scale(Vector4 scale)
	{
		x *= scale.x;
		y *= scale.y;
		z *= scale.z;
		w *= scale.w;
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
	}

	/// <summary>
	///   <para>Returns true if the given vector is exactly equal to this vector.</para>
	/// </summary>
	/// <param name="other"></param>
	public override bool Equals(object other)
	{
		if (!(other is Vector4 vector))
		{
			return false;
		}
		return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z) && w.Equals(vector.w);
	}

	/// <summary>
	///   <para></para>
	/// </summary>
	/// <param name="a"></param>
	public static Vector4 Normalize(Vector4 a)
	{
		float num = Magnitude(a);
		if (num > 1E-05f)
		{
			return a / num;
		}
		return zero;
	}

	/// <summary>
	///   <para>Makes this vector have a magnitude of 1.</para>
	/// </summary>
	public void Normalize()
	{
		float num = Magnitude(this);
		if (num > 1E-05f)
		{
			this /= num;
		}
		else
		{
			this = zero;
		}
	}

	/// <summary>
	///   <para>Dot Product of two vectors.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public static float Dot(Vector4 a, Vector4 b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
	}

	/// <summary>
	///   <para>Projects a vector onto another vector.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public static Vector4 Project(Vector4 a, Vector4 b)
	{
		return b * Dot(a, b) / Dot(b, b);
	}

	/// <summary>
	///   <para>Returns the distance between a and b.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public static float Distance(Vector4 a, Vector4 b)
	{
		return Magnitude(a - b);
	}

	public static float Magnitude(Vector4 a)
	{
		return Mathf.Sqrt(Dot(a, a));
	}

	/// <summary>
	///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
	/// </summary>
	/// <param name="lhs"></param>
	/// <param name="rhs"></param>
	public static Vector4 Min(Vector4 lhs, Vector4 rhs)
	{
		return new Vector4(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z), Mathf.Min(lhs.w, rhs.w));
	}

	/// <summary>
	///   <para>Returns a vector that is made from the largest components of two vectors.</para>
	/// </summary>
	/// <param name="lhs"></param>
	/// <param name="rhs"></param>
	public static Vector4 Max(Vector4 lhs, Vector4 rhs)
	{
		return new Vector4(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z), Mathf.Max(lhs.w, rhs.w));
	}

	public static Vector4 operator +(Vector4 a, Vector4 b)
	{
		return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
	}

	public static Vector4 operator -(Vector4 a, Vector4 b)
	{
		return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
	}

	public static Vector4 operator -(Vector4 a)
	{
		return new Vector4(0f - a.x, 0f - a.y, 0f - a.z, 0f - a.w);
	}

	public static Vector4 operator *(Vector4 a, float d)
	{
		return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
	}

	public static Vector4 operator *(float d, Vector4 a)
	{
		return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
	}

	public static Vector4 operator /(Vector4 a, float d)
	{
		return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
	}

	public static bool operator ==(Vector4 lhs, Vector4 rhs)
	{
		return SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
	}

	public static bool operator !=(Vector4 lhs, Vector4 rhs)
	{
		return !(lhs == rhs);
	}

	public static implicit operator Vector4(Vector3 v)
	{
		return new Vector4(v.x, v.y, v.z, 0f);
	}

	public static implicit operator Vector3(Vector4 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	public static implicit operator Vector4(Vector2 v)
	{
		return new Vector4(v.x, v.y, 0f, 0f);
	}

	public static implicit operator Vector2(Vector4 v)
	{
		return new Vector2(v.x, v.y);
	}

	/// <summary>
	///   <para>Returns a nicely formatted string for this vector.</para>
	/// </summary>
	/// <param name="format"></param>
	public override string ToString()
	{
		return UnityString.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", x, y, z, w);
	}

	/// <summary>
	///   <para>Returns a nicely formatted string for this vector.</para>
	/// </summary>
	/// <param name="format"></param>
	public string ToString(string format)
	{
		return UnityString.Format("({0}, {1}, {2}, {3})", x.ToString(format), y.ToString(format), z.ToString(format), w.ToString(format));
	}

	public static float SqrMagnitude(Vector4 a)
	{
		return Dot(a, a);
	}

	public float SqrMagnitude()
	{
		return Dot(this, this);
	}
}
