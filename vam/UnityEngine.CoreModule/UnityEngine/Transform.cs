using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Position, rotation and scale of an object.</para>
/// </summary>
public class Transform : Component, IEnumerable
{
	private sealed class Enumerator : IEnumerator
	{
		private Transform outer;

		private int currentIndex = -1;

		public object Current => outer.GetChild(currentIndex);

		internal Enumerator(Transform outer)
		{
			this.outer = outer;
		}

		public bool MoveNext()
		{
			int childCount = outer.childCount;
			return ++currentIndex < childCount;
		}

		public void Reset()
		{
			currentIndex = -1;
		}
	}

	/// <summary>
	///   <para>The position of the transform in world space.</para>
	/// </summary>
	public Vector3 position
	{
		get
		{
			INTERNAL_get_position(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_position(ref value);
		}
	}

	/// <summary>
	///   <para>Position of the transform relative to the parent transform.</para>
	/// </summary>
	public Vector3 localPosition
	{
		get
		{
			INTERNAL_get_localPosition(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_localPosition(ref value);
		}
	}

	/// <summary>
	///   <para>The rotation as Euler angles in degrees.</para>
	/// </summary>
	public Vector3 eulerAngles
	{
		get
		{
			return rotation.eulerAngles;
		}
		set
		{
			rotation = Quaternion.Euler(value);
		}
	}

	/// <summary>
	///   <para>The rotation as Euler angles in degrees relative to the parent transform's rotation.</para>
	/// </summary>
	public Vector3 localEulerAngles
	{
		get
		{
			return localRotation.eulerAngles;
		}
		set
		{
			localRotation = Quaternion.Euler(value);
		}
	}

	/// <summary>
	///   <para>The red axis of the transform in world space.</para>
	/// </summary>
	public Vector3 right
	{
		get
		{
			return rotation * Vector3.right;
		}
		set
		{
			rotation = Quaternion.FromToRotation(Vector3.right, value);
		}
	}

	/// <summary>
	///   <para>The green axis of the transform in world space.</para>
	/// </summary>
	public Vector3 up
	{
		get
		{
			return rotation * Vector3.up;
		}
		set
		{
			rotation = Quaternion.FromToRotation(Vector3.up, value);
		}
	}

	/// <summary>
	///   <para>The blue axis of the transform in world space.</para>
	/// </summary>
	public Vector3 forward
	{
		get
		{
			return rotation * Vector3.forward;
		}
		set
		{
			rotation = Quaternion.LookRotation(value);
		}
	}

	/// <summary>
	///   <para>The rotation of the transform in world space stored as a Quaternion.</para>
	/// </summary>
	public Quaternion rotation
	{
		get
		{
			INTERNAL_get_rotation(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_rotation(ref value);
		}
	}

	/// <summary>
	///   <para>The rotation of the transform relative to the parent transform's rotation.</para>
	/// </summary>
	public Quaternion localRotation
	{
		get
		{
			INTERNAL_get_localRotation(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_localRotation(ref value);
		}
	}

	/// <summary>
	///   <para>The scale of the transform relative to the parent.</para>
	/// </summary>
	public Vector3 localScale
	{
		get
		{
			INTERNAL_get_localScale(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_localScale(ref value);
		}
	}

	/// <summary>
	///   <para>The parent of the transform.</para>
	/// </summary>
	public Transform parent
	{
		get
		{
			return parentInternal;
		}
		set
		{
			if (this is RectTransform)
			{
				Debug.LogWarning("Parent of RectTransform is being set with parent property. Consider using the SetParent method instead, with the worldPositionStays argument set to false. This will retain local orientation and scale rather than world orientation and scale, which can prevent common UI scaling issues.", this);
			}
			parentInternal = value;
		}
	}

	internal extern Transform parentInternal
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Matrix that transforms a point from world space into local space (Read Only).</para>
	/// </summary>
	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			INTERNAL_get_worldToLocalMatrix(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>Matrix that transforms a point from local space into world space (Read Only).</para>
	/// </summary>
	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			INTERNAL_get_localToWorldMatrix(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>Returns the topmost transform in the hierarchy.</para>
	/// </summary>
	public extern Transform root
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The number of children the Transform has.</para>
	/// </summary>
	public extern int childCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The global scale of the object (Read Only).</para>
	/// </summary>
	public Vector3 lossyScale
	{
		get
		{
			INTERNAL_get_lossyScale(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>Has the transform changed since the last time the flag was set to 'false'?</para>
	/// </summary>
	public extern bool hasChanged
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The transform capacity of the transform's hierarchy data structure.</para>
	/// </summary>
	public extern int hierarchyCapacity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The number of transforms in the transform's hierarchy data structure.</para>
	/// </summary>
	public extern int hierarchyCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	protected Transform()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_position(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_position(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_localPosition(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_localPosition(ref Vector3 value);

	internal Vector3 GetLocalEulerAngles(RotationOrder order)
	{
		INTERNAL_CALL_GetLocalEulerAngles(this, order, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetLocalEulerAngles(Transform self, RotationOrder order, out Vector3 value);

	internal void SetLocalEulerAngles(Vector3 euler, RotationOrder order)
	{
		INTERNAL_CALL_SetLocalEulerAngles(this, ref euler, order);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_SetLocalEulerAngles(Transform self, ref Vector3 euler, RotationOrder order);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_rotation(out Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_rotation(ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_localRotation(out Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_localRotation(ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_localScale(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_localScale(ref Vector3 value);

	/// <summary>
	///   <para>Set the parent of the transform.</para>
	/// </summary>
	/// <param name="parent">The parent Transform to use.</param>
	/// <param name="worldPositionStays">If true, the parent-relative position, scale and
	///   rotation are modified such that the object keeps the same world space position,
	///   rotation and scale as before.</param>
	public void SetParent(Transform parent)
	{
		SetParent(parent, worldPositionStays: true);
	}

	/// <summary>
	///   <para>Set the parent of the transform.</para>
	/// </summary>
	/// <param name="parent">The parent Transform to use.</param>
	/// <param name="worldPositionStays">If true, the parent-relative position, scale and
	///   rotation are modified such that the object keeps the same world space position,
	///   rotation and scale as before.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetParent(Transform parent, bool worldPositionStays);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_worldToLocalMatrix(out Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_localToWorldMatrix(out Matrix4x4 value);

	/// <summary>
	///   <para>Sets the world space position and rotation of the Transform component.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		INTERNAL_CALL_SetPositionAndRotation(this, ref position, ref rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_SetPositionAndRotation(Transform self, ref Vector3 position, ref Quaternion rotation);

	/// <summary>
	///   <para>Moves the transform in the direction and distance of translation.</para>
	/// </summary>
	/// <param name="translation"></param>
	/// <param name="relativeTo"></param>
	[ExcludeFromDocs]
	public void Translate(Vector3 translation)
	{
		Space relativeTo = Space.Self;
		Translate(translation, relativeTo);
	}

	/// <summary>
	///   <para>Moves the transform in the direction and distance of translation.</para>
	/// </summary>
	/// <param name="translation"></param>
	/// <param name="relativeTo"></param>
	public void Translate(Vector3 translation, [DefaultValue("Space.Self")] Space relativeTo)
	{
		if (relativeTo == Space.World)
		{
			position += translation;
		}
		else
		{
			position += TransformDirection(translation);
		}
	}

	/// <summary>
	///   <para>Moves the transform by x along the x axis, y along the y axis, and z along the z axis.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <param name="relativeTo"></param>
	[ExcludeFromDocs]
	public void Translate(float x, float y, float z)
	{
		Space relativeTo = Space.Self;
		Translate(x, y, z, relativeTo);
	}

	/// <summary>
	///   <para>Moves the transform by x along the x axis, y along the y axis, and z along the z axis.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <param name="relativeTo"></param>
	public void Translate(float x, float y, float z, [DefaultValue("Space.Self")] Space relativeTo)
	{
		Translate(new Vector3(x, y, z), relativeTo);
	}

	/// <summary>
	///   <para>Moves the transform in the direction and distance of translation.</para>
	/// </summary>
	/// <param name="translation"></param>
	/// <param name="relativeTo"></param>
	public void Translate(Vector3 translation, Transform relativeTo)
	{
		if ((bool)relativeTo)
		{
			position += relativeTo.TransformDirection(translation);
		}
		else
		{
			position += translation;
		}
	}

	/// <summary>
	///   <para>Moves the transform by x along the x axis, y along the y axis, and z along the z axis.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <param name="relativeTo"></param>
	public void Translate(float x, float y, float z, Transform relativeTo)
	{
		Translate(new Vector3(x, y, z), relativeTo);
	}

	[ExcludeFromDocs]
	public void Rotate(Vector3 eulerAngles)
	{
		Space relativeTo = Space.Self;
		Rotate(eulerAngles, relativeTo);
	}

	/// <summary>
	///   <para>Applies a rotation of eulerAngles.z degrees around the z axis, eulerAngles.x degrees around the x axis, and eulerAngles.y degrees around the y axis (in that order).</para>
	/// </summary>
	/// <param name="relativeTo">Rotation is local to object or World.</param>
	/// <param name="eulers">Rotation to apply.</param>
	/// <param name="eulerAngles"></param>
	public void Rotate(Vector3 eulerAngles, [DefaultValue("Space.Self")] Space relativeTo)
	{
		Quaternion quaternion = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
		if (relativeTo == Space.Self)
		{
			localRotation *= quaternion;
		}
		else
		{
			rotation *= Quaternion.Inverse(rotation) * quaternion * rotation;
		}
	}

	[ExcludeFromDocs]
	public void Rotate(float xAngle, float yAngle, float zAngle)
	{
		Space relativeTo = Space.Self;
		Rotate(xAngle, yAngle, zAngle, relativeTo);
	}

	/// <summary>
	///   <para>Applies a rotation of zAngle degrees around the z axis, xAngle degrees around the x axis, and yAngle degrees around the y axis (in that order).</para>
	/// </summary>
	/// <param name="xAngle">Degrees to rotate around the X axis.</param>
	/// <param name="yAngle">Degrees to rotate around the Y axis.</param>
	/// <param name="zAngle">Degrees to rotate around the Z axis.</param>
	/// <param name="relativeTo">Rotation is local to object or World.</param>
	public void Rotate(float xAngle, float yAngle, float zAngle, [DefaultValue("Space.Self")] Space relativeTo)
	{
		Rotate(new Vector3(xAngle, yAngle, zAngle), relativeTo);
	}

	internal void RotateAroundInternal(Vector3 axis, float angle)
	{
		INTERNAL_CALL_RotateAroundInternal(this, ref axis, angle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_RotateAroundInternal(Transform self, ref Vector3 axis, float angle);

	[ExcludeFromDocs]
	public void Rotate(Vector3 axis, float angle)
	{
		Space relativeTo = Space.Self;
		Rotate(axis, angle, relativeTo);
	}

	/// <summary>
	///   <para>Rotates the object around axis by angle degrees.</para>
	/// </summary>
	/// <param name="axis">Axis to apply rotation to.</param>
	/// <param name="angle">Degrees to rotation to apply.</param>
	/// <param name="relativeTo">Rotation is local to object or World.</param>
	public void Rotate(Vector3 axis, float angle, [DefaultValue("Space.Self")] Space relativeTo)
	{
		if (relativeTo == Space.Self)
		{
			RotateAroundInternal(base.transform.TransformDirection(axis), angle * ((float)Math.PI / 180f));
		}
		else
		{
			RotateAroundInternal(axis, angle * ((float)Math.PI / 180f));
		}
	}

	/// <summary>
	///   <para>Rotates the transform about axis passing through point in world coordinates by angle degrees.</para>
	/// </summary>
	/// <param name="point"></param>
	/// <param name="axis"></param>
	/// <param name="angle"></param>
	public void RotateAround(Vector3 point, Vector3 axis, float angle)
	{
		Vector3 vector = position;
		Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
		Vector3 vector2 = vector - point;
		vector2 = quaternion * vector2;
		vector = point + vector2;
		position = vector;
		RotateAroundInternal(axis, angle * ((float)Math.PI / 180f));
	}

	/// <summary>
	///   <para>Rotates the transform so the forward vector points at target's current position.</para>
	/// </summary>
	/// <param name="target">Object to point towards.</param>
	/// <param name="worldUp">Vector specifying the upward direction.</param>
	[ExcludeFromDocs]
	public void LookAt(Transform target)
	{
		Vector3 worldUp = Vector3.up;
		LookAt(target, worldUp);
	}

	/// <summary>
	///   <para>Rotates the transform so the forward vector points at target's current position.</para>
	/// </summary>
	/// <param name="target">Object to point towards.</param>
	/// <param name="worldUp">Vector specifying the upward direction.</param>
	public void LookAt(Transform target, [DefaultValue("Vector3.up")] Vector3 worldUp)
	{
		if ((bool)target)
		{
			LookAt(target.position, worldUp);
		}
	}

	/// <summary>
	///   <para>Rotates the transform so the forward vector points at worldPosition.</para>
	/// </summary>
	/// <param name="worldPosition">Point to look at.</param>
	/// <param name="worldUp">Vector specifying the upward direction.</param>
	public void LookAt(Vector3 worldPosition, [DefaultValue("Vector3.up")] Vector3 worldUp)
	{
		INTERNAL_CALL_LookAt(this, ref worldPosition, ref worldUp);
	}

	/// <summary>
	///   <para>Rotates the transform so the forward vector points at worldPosition.</para>
	/// </summary>
	/// <param name="worldPosition">Point to look at.</param>
	/// <param name="worldUp">Vector specifying the upward direction.</param>
	[ExcludeFromDocs]
	public void LookAt(Vector3 worldPosition)
	{
		Vector3 worldUp = Vector3.up;
		INTERNAL_CALL_LookAt(this, ref worldPosition, ref worldUp);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_LookAt(Transform self, ref Vector3 worldPosition, ref Vector3 worldUp);

	/// <summary>
	///   <para>Transforms direction from local space to world space.</para>
	/// </summary>
	/// <param name="direction"></param>
	public Vector3 TransformDirection(Vector3 direction)
	{
		INTERNAL_CALL_TransformDirection(this, ref direction, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_TransformDirection(Transform self, ref Vector3 direction, out Vector3 value);

	/// <summary>
	///   <para>Transforms direction x, y, z from local space to world space.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public Vector3 TransformDirection(float x, float y, float z)
	{
		return TransformDirection(new Vector3(x, y, z));
	}

	/// <summary>
	///   <para>Transforms a direction from world space to local space. The opposite of Transform.TransformDirection.</para>
	/// </summary>
	/// <param name="direction"></param>
	public Vector3 InverseTransformDirection(Vector3 direction)
	{
		INTERNAL_CALL_InverseTransformDirection(this, ref direction, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_InverseTransformDirection(Transform self, ref Vector3 direction, out Vector3 value);

	/// <summary>
	///   <para>Transforms the direction x, y, z from world space to local space. The opposite of Transform.TransformDirection.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public Vector3 InverseTransformDirection(float x, float y, float z)
	{
		return InverseTransformDirection(new Vector3(x, y, z));
	}

	/// <summary>
	///   <para>Transforms vector from local space to world space.</para>
	/// </summary>
	/// <param name="vector"></param>
	public Vector3 TransformVector(Vector3 vector)
	{
		INTERNAL_CALL_TransformVector(this, ref vector, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_TransformVector(Transform self, ref Vector3 vector, out Vector3 value);

	/// <summary>
	///   <para>Transforms vector x, y, z from local space to world space.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public Vector3 TransformVector(float x, float y, float z)
	{
		return TransformVector(new Vector3(x, y, z));
	}

	/// <summary>
	///   <para>Transforms a vector from world space to local space. The opposite of Transform.TransformVector.</para>
	/// </summary>
	/// <param name="vector"></param>
	public Vector3 InverseTransformVector(Vector3 vector)
	{
		INTERNAL_CALL_InverseTransformVector(this, ref vector, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_InverseTransformVector(Transform self, ref Vector3 vector, out Vector3 value);

	/// <summary>
	///   <para>Transforms the vector x, y, z from world space to local space. The opposite of Transform.TransformVector.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public Vector3 InverseTransformVector(float x, float y, float z)
	{
		return InverseTransformVector(new Vector3(x, y, z));
	}

	/// <summary>
	///   <para>Transforms position from local space to world space.</para>
	/// </summary>
	/// <param name="position"></param>
	public Vector3 TransformPoint(Vector3 position)
	{
		INTERNAL_CALL_TransformPoint(this, ref position, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_TransformPoint(Transform self, ref Vector3 position, out Vector3 value);

	/// <summary>
	///   <para>Transforms the position x, y, z from local space to world space.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public Vector3 TransformPoint(float x, float y, float z)
	{
		return TransformPoint(new Vector3(x, y, z));
	}

	/// <summary>
	///   <para>Transforms position from world space to local space.</para>
	/// </summary>
	/// <param name="position"></param>
	public Vector3 InverseTransformPoint(Vector3 position)
	{
		INTERNAL_CALL_InverseTransformPoint(this, ref position, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_InverseTransformPoint(Transform self, ref Vector3 position, out Vector3 value);

	/// <summary>
	///   <para>Transforms the position x, y, z from world space to local space. The opposite of Transform.TransformPoint.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public Vector3 InverseTransformPoint(float x, float y, float z)
	{
		return InverseTransformPoint(new Vector3(x, y, z));
	}

	/// <summary>
	///   <para>Unparents all children.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void DetachChildren();

	/// <summary>
	///   <para>Move the transform to the start of the local transform list.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetAsFirstSibling();

	/// <summary>
	///   <para>Move the transform to the end of the local transform list.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetAsLastSibling();

	/// <summary>
	///   <para>Sets the sibling index.</para>
	/// </summary>
	/// <param name="index">Index to set.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetSiblingIndex(int index);

	/// <summary>
	///   <para>Gets the sibling index.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern int GetSiblingIndex();

	/// <summary>
	///   <para>Finds a child by n and returns it.</para>
	/// </summary>
	/// <param name="n">Name of child to be found.</param>
	/// <param name="name"></param>
	/// <returns>
	///   <para>The returned child transform or null if no child is found.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Transform Find(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_lossyScale(out Vector3 value);

	/// <summary>
	///   <para>Is this transform a child of parent?</para>
	/// </summary>
	/// <param name="parent"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool IsChildOf(Transform parent);

	[Obsolete("FindChild has been deprecated. Use Find instead (UnityUpgradable) -> Find([mscorlib] System.String)", false)]
	public Transform FindChild(string name)
	{
		return Find(name);
	}

	public IEnumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	/// <summary>
	///   <para></para>
	/// </summary>
	/// <param name="axis"></param>
	/// <param name="angle"></param>
	[Obsolete("use Transform.Rotate instead.")]
	public void RotateAround(Vector3 axis, float angle)
	{
		INTERNAL_CALL_RotateAround(this, ref axis, angle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_RotateAround(Transform self, ref Vector3 axis, float angle);

	[Obsolete("use Transform.Rotate instead.")]
	public void RotateAroundLocal(Vector3 axis, float angle)
	{
		INTERNAL_CALL_RotateAroundLocal(this, ref axis, angle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_RotateAroundLocal(Transform self, ref Vector3 axis, float angle);

	/// <summary>
	///   <para>Returns a transform child by index.</para>
	/// </summary>
	/// <param name="index">Index of the child transform to return. Must be smaller than Transform.childCount.</param>
	/// <returns>
	///   <para>Transform child by index.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Transform GetChild(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("use Transform.childCount instead.")]
	[GeneratedByOldBindingsGenerator]
	public extern int GetChildCount();
}
