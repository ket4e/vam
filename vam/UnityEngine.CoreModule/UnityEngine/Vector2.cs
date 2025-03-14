using System;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Representation of 2D vectors and points.</para>
/// </summary>
[UsedByNativeCode]
[GenerateManagedProxy("Vector2f")]
public struct Vector2
{
	/// <summary>
	///   <para>X component of the vector.</para>
	/// </summary>
	public float x;

	/// <summary>
	///   <para>Y component of the vector.</para>
	/// </summary>
	public float y;

	private static readonly Vector2 zeroVector = new Vector2(0f, 0f);

	private static readonly Vector2 oneVector = new Vector2(1f, 1f);

	private static readonly Vector2 upVector = new Vector2(0f, 1f);

	private static readonly Vector2 downVector = new Vector2(0f, -1f);

	private static readonly Vector2 leftVector = new Vector2(-1f, 0f);

	private static readonly Vector2 rightVector = new Vector2(1f, 0f);

	private static readonly Vector2 positiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

	private static readonly Vector2 negativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

	public const float kEpsilon = 1E-05f;

	public float this[int index]
	{
		get
		{
			return index switch
			{
				0 => x, 
				1 => y, 
				_ => throw new IndexOutOfRangeException("Invalid Vector2 index!"), 
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
			default:
				throw new IndexOutOfRangeException("Invalid Vector2 index!");
			}
		}
	}

	/// <summary>
	///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
	/// </summary>
	public Vector2 normalized
	{
		get
		{
			Vector2 result = new Vector2(x, y);
			result.Normalize();
			return result;
		}
	}

	/// <summary>
	///   <para>Returns the length of this vector (Read Only).</para>
	/// </summary>
	public float magnitude => Mathf.Sqrt(x * x + y * y);

	/// <summary>
	///   <para>Returns the squared length of this vector (Read Only).</para>
	/// </summary>
	public float sqrMagnitude => x * x + y * y;

	/// <summary>
	///   <para>Shorthand for writing Vector2(0, 0).</para>
	/// </summary>
	public static Vector2 zero => zeroVector;

	/// <summary>
	///   <para>Shorthand for writing Vector2(1, 1).</para>
	/// </summary>
	public static Vector2 one => oneVector;

	/// <summary>
	///   <para>Shorthand for writing Vector2(0, 1).</para>
	/// </summary>
	public static Vector2 up => upVector;

	/// <summary>
	///   <para>Shorthand for writing Vector2(0, -1).</para>
	/// </summary>
	public static Vector2 down => downVector;

	/// <summary>
	///   <para>Shorthand for writing Vector2(-1, 0).</para>
	/// </summary>
	public static Vector2 left => leftVector;

	/// <summary>
	///   <para>Shorthand for writing Vector2(1, 0).</para>
	/// </summary>
	public static Vector2 right => rightVector;

	/// <summary>
	///   <para>Shorthand for writing Vector2(float.PositiveInfinity, float.PositiveInfinity).</para>
	/// </summary>
	public static Vector2 positiveInfinity => positiveInfinityVector;

	/// <summary>
	///   <para>Shorthand for writing Vector2(float.NegativeInfinity, float.NegativeInfinity).</para>
	/// </summary>
	public static Vector2 negativeInfinity => negativeInfinityVector;

	/// <summary>
	///   <para>Constructs a new vector with given x, y components.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public Vector2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	/// <summary>
	///   <para>Set x and y components of an existing Vector2.</para>
	/// </summary>
	/// <param name="newX"></param>
	/// <param name="newY"></param>
	public void Set(float newX, float newY)
	{
		x = newX;
		y = newY;
	}

	/// <summary>
	///   <para>Linearly interpolates between vectors a and b by t.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="t"></param>
	public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
	{
		t = Mathf.Clamp01(t);
		return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
	}

	/// <summary>
	///   <para>Linearly interpolates between vectors a and b by t.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="t"></param>
	public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
	{
		return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
	}

	/// <summary>
	///   <para>Moves a point current towards target.</para>
	/// </summary>
	/// <param name="current"></param>
	/// <param name="target"></param>
	/// <param name="maxDistanceDelta"></param>
	public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
	{
		Vector2 vector = target - current;
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
	public static Vector2 Scale(Vector2 a, Vector2 b)
	{
		return new Vector2(a.x * b.x, a.y * b.y);
	}

	/// <summary>
	///   <para>Multiplies every component of this vector by the same component of scale.</para>
	/// </summary>
	/// <param name="scale"></param>
	public void Scale(Vector2 scale)
	{
		x *= scale.x;
		y *= scale.y;
	}

	/// <summary>
	///   <para>Makes this vector have a magnitude of 1.</para>
	/// </summary>
	public void Normalize()
	{
		float num = magnitude;
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
	///   <para>Returns a nicely formatted string for this vector.</para>
	/// </summary>
	/// <param name="format"></param>
	public override string ToString()
	{
		return UnityString.Format("({0:F1}, {1:F1})", x, y);
	}

	/// <summary>
	///   <para>Returns a nicely formatted string for this vector.</para>
	/// </summary>
	/// <param name="format"></param>
	public string ToString(string format)
	{
		return UnityString.Format("({0}, {1})", x.ToString(format), y.ToString(format));
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2);
	}

	/// <summary>
	///   <para>Returns true if the given vector is exactly equal to this vector.</para>
	/// </summary>
	/// <param name="other"></param>
	public override bool Equals(object other)
	{
		if (!(other is Vector2 vector))
		{
			return false;
		}
		return x.Equals(vector.x) && y.Equals(vector.y);
	}

	/// <summary>
	///   <para>Reflects a vector off the vector defined by a normal.</para>
	/// </summary>
	/// <param name="inDirection"></param>
	/// <param name="inNormal"></param>
	public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal)
	{
		return -2f * Dot(inNormal, inDirection) * inNormal + inDirection;
	}

	/// <summary>
	///   <para>Returns the 2D vector perpendicular to this 2D vector. The result is always rotated 90-degrees in a counter-clockwise direction for a 2D coordinate system where the positive Y axis goes up.</para>
	/// </summary>
	/// <param name="inDirection">The input direction.</param>
	/// <returns>
	///   <para>The perpendicular direction.</para>
	/// </returns>
	public static Vector2 Perpendicular(Vector2 inDirection)
	{
		return new Vector2(0f - inDirection.y, inDirection.x);
	}

	/// <summary>
	///   <para>Dot Product of two vectors.</para>
	/// </summary>
	/// <param name="lhs"></param>
	/// <param name="rhs"></param>
	public static float Dot(Vector2 lhs, Vector2 rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y;
	}

	/// <summary>
	///   <para>Returns the unsigned angle in degrees between from and to.</para>
	/// </summary>
	/// <param name="from">The vector from which the angular difference is measured.</param>
	/// <param name="to">The vector to which the angular difference is measured.</param>
	public static float Angle(Vector2 from, Vector2 to)
	{
		return Mathf.Acos(Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
	}

	/// <summary>
	///   <para>Returns the signed angle in degrees between from and to.</para>
	/// </summary>
	/// <param name="from">The vector from which the angular difference is measured.</param>
	/// <param name="to">The vector to which the angular difference is measured.</param>
	public static float SignedAngle(Vector2 from, Vector2 to)
	{
		Vector2 lhs = from.normalized;
		Vector2 rhs = to.normalized;
		float num = Mathf.Acos(Mathf.Clamp(Dot(lhs, rhs), -1f, 1f)) * 57.29578f;
		float num2 = Mathf.Sign(lhs.x * rhs.y - lhs.y * rhs.x);
		return num * num2;
	}

	/// <summary>
	///   <para>Returns the distance between a and b.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public static float Distance(Vector2 a, Vector2 b)
	{
		return (a - b).magnitude;
	}

	/// <summary>
	///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="maxLength"></param>
	public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
	{
		if (vector.sqrMagnitude > maxLength * maxLength)
		{
			return vector.normalized * maxLength;
		}
		return vector;
	}

	public static float SqrMagnitude(Vector2 a)
	{
		return a.x * a.x + a.y * a.y;
	}

	public float SqrMagnitude()
	{
		return x * x + y * y;
	}

	/// <summary>
	///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
	/// </summary>
	/// <param name="lhs"></param>
	/// <param name="rhs"></param>
	public static Vector2 Min(Vector2 lhs, Vector2 rhs)
	{
		return new Vector2(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
	}

	/// <summary>
	///   <para>Returns a vector that is made from the largest components of two vectors.</para>
	/// </summary>
	/// <param name="lhs"></param>
	/// <param name="rhs"></param>
	public static Vector2 Max(Vector2 lhs, Vector2 rhs)
	{
		return new Vector2(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
	}

	[ExcludeFromDocs]
	public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed)
	{
		float deltaTime = Time.deltaTime;
		return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}

	[ExcludeFromDocs]
	public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime)
	{
		float deltaTime = Time.deltaTime;
		float maxSpeed = float.PositiveInfinity;
		return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}

	public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
	{
		smoothTime = Mathf.Max(0.0001f, smoothTime);
		float num = 2f / smoothTime;
		float num2 = num * deltaTime;
		float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
		Vector2 vector = current - target;
		Vector2 vector2 = target;
		float maxLength = maxSpeed * smoothTime;
		vector = ClampMagnitude(vector, maxLength);
		target = current - vector;
		Vector2 vector3 = (currentVelocity + num * vector) * deltaTime;
		currentVelocity = (currentVelocity - num * vector3) * num3;
		Vector2 vector4 = target + (vector + vector3) * num3;
		if (Dot(vector2 - current, vector4 - vector2) > 0f)
		{
			vector4 = vector2;
			currentVelocity = (vector4 - vector2) / deltaTime;
		}
		return vector4;
	}

	public static Vector2 operator +(Vector2 a, Vector2 b)
	{
		return new Vector2(a.x + b.x, a.y + b.y);
	}

	public static Vector2 operator -(Vector2 a, Vector2 b)
	{
		return new Vector2(a.x - b.x, a.y - b.y);
	}

	public static Vector2 operator *(Vector2 a, Vector2 b)
	{
		return new Vector2(a.x * b.x, a.y * b.y);
	}

	public static Vector2 operator /(Vector2 a, Vector2 b)
	{
		return new Vector2(a.x / b.x, a.y / b.y);
	}

	public static Vector2 operator -(Vector2 a)
	{
		return new Vector2(0f - a.x, 0f - a.y);
	}

	public static Vector2 operator *(Vector2 a, float d)
	{
		return new Vector2(a.x * d, a.y * d);
	}

	public static Vector2 operator *(float d, Vector2 a)
	{
		return new Vector2(a.x * d, a.y * d);
	}

	public static Vector2 operator /(Vector2 a, float d)
	{
		return new Vector2(a.x / d, a.y / d);
	}

	public static bool operator ==(Vector2 lhs, Vector2 rhs)
	{
		return (lhs - rhs).sqrMagnitude < 9.9999994E-11f;
	}

	public static bool operator !=(Vector2 lhs, Vector2 rhs)
	{
		return !(lhs == rhs);
	}

	public static implicit operator Vector2(Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static implicit operator Vector3(Vector2 v)
	{
		return new Vector3(v.x, v.y, 0f);
	}
}
