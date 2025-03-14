using UnityEngine;

namespace Leap.Unity;

public static class TransformUtil
{
	public static Quaternion TransformRotation(this Transform transform, Quaternion rotation)
	{
		return transform.rotation * rotation;
	}

	public static Quaternion InverseTransformRotation(this Transform transform, Quaternion rotation)
	{
		return Quaternion.Inverse(transform.rotation) * rotation;
	}

	public static void SetLocalX(this Transform transform, float localX)
	{
		transform.setLocalAxis(localX, 0);
	}

	public static void SetLocalY(this Transform transform, float localY)
	{
		transform.setLocalAxis(localY, 1);
	}

	public static void SetLocalZ(this Transform transform, float localZ)
	{
		transform.setLocalAxis(localZ, 2);
	}

	private static void setLocalAxis(this Transform transform, float value, int axis)
	{
		Vector3 localPosition = transform.localPosition;
		localPosition[axis] = value;
		transform.localPosition = localPosition;
	}
}
