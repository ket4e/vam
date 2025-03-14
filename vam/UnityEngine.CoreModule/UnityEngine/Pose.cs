using System;

namespace UnityEngine;

/// <summary>
///   <para>Representation of a Position, and a Rotation in 3D Space</para>
/// </summary>
[Serializable]
public struct Pose
{
	/// <summary>
	///   <para>The position component of the pose.</para>
	/// </summary>
	public Vector3 position;

	/// <summary>
	///   <para>The rotation component of the pose.</para>
	/// </summary>
	public Quaternion rotation;

	private static readonly Pose k_Identity = new Pose(Vector3.zero, Quaternion.identity);

	/// <summary>
	///   <para>Returns the forward vector of the pose.</para>
	/// </summary>
	public Vector3 forward => rotation * Vector3.forward;

	/// <summary>
	///   <para>Returns the right vector of the pose.</para>
	/// </summary>
	public Vector3 right => rotation * Vector3.right;

	/// <summary>
	///   <para>Returns the up vector of the pose.</para>
	/// </summary>
	public Vector3 up => rotation * Vector3.up;

	/// <summary>
	///   <para>Shorthand for pose which represents zero position, and an identity rotation.</para>
	/// </summary>
	public static Pose identity => k_Identity;

	public Pose(Vector3 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
	}

	public override string ToString()
	{
		return $"({position.ToString()}, {rotation.ToString()})";
	}

	public string ToString(string format)
	{
		return $"({position.ToString(format)}, {rotation.ToString(format)})";
	}

	/// <summary>
	///   <para>Transforms the current pose into the local space of the provided pose.</para>
	/// </summary>
	/// <param name="lhs"></param>
	public Pose GetTransformedBy(Pose lhs)
	{
		Pose result = default(Pose);
		result.position = lhs.position + lhs.rotation * position;
		result.rotation = lhs.rotation * rotation;
		return result;
	}

	/// <summary>
	///   <para>Transforms the current pose into the local space of the provided pose.</para>
	/// </summary>
	/// <param name="lhs"></param>
	public Pose GetTransformedBy(Transform lhs)
	{
		Pose result = default(Pose);
		result.position = lhs.TransformPoint(position);
		result.rotation = lhs.rotation * rotation;
		return result;
	}
}
