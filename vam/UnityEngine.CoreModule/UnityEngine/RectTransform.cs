using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Position, size, anchor and pivot information for a rectangle.</para>
/// </summary>
[NativeClass("UI::RectTransform")]
public sealed class RectTransform : Transform
{
	/// <summary>
	///   <para>Delegate used for the reapplyDrivenProperties event.</para>
	/// </summary>
	/// <param name="driven"></param>
	public delegate void ReapplyDrivenProperties(RectTransform driven);

	/// <summary>
	///   <para>Enum used to specify one edge of a rectangle.</para>
	/// </summary>
	public enum Edge
	{
		/// <summary>
		///   <para>The left edge.</para>
		/// </summary>
		Left,
		/// <summary>
		///   <para>The right edge.</para>
		/// </summary>
		Right,
		/// <summary>
		///   <para>The top edge.</para>
		/// </summary>
		Top,
		/// <summary>
		///   <para>The bottom edge.</para>
		/// </summary>
		Bottom
	}

	/// <summary>
	///   <para>An axis that can be horizontal or vertical.</para>
	/// </summary>
	public enum Axis
	{
		/// <summary>
		///   <para>Horizontal.</para>
		/// </summary>
		Horizontal,
		/// <summary>
		///   <para>Vertical.</para>
		/// </summary>
		Vertical
	}

	/// <summary>
	///   <para>The calculated rectangle in the local space of the Transform.</para>
	/// </summary>
	public Rect rect
	{
		get
		{
			INTERNAL_get_rect(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>The normalized position in the parent RectTransform that the lower left corner is anchored to.</para>
	/// </summary>
	public Vector2 anchorMin
	{
		get
		{
			INTERNAL_get_anchorMin(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_anchorMin(ref value);
		}
	}

	/// <summary>
	///   <para>The normalized position in the parent RectTransform that the upper right corner is anchored to.</para>
	/// </summary>
	public Vector2 anchorMax
	{
		get
		{
			INTERNAL_get_anchorMax(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_anchorMax(ref value);
		}
	}

	/// <summary>
	///   <para>The 3D position of the pivot of this RectTransform relative to the anchor reference point.</para>
	/// </summary>
	public Vector3 anchoredPosition3D
	{
		get
		{
			Vector2 vector = anchoredPosition;
			return new Vector3(vector.x, vector.y, base.localPosition.z);
		}
		set
		{
			anchoredPosition = new Vector2(value.x, value.y);
			Vector3 vector = base.localPosition;
			vector.z = value.z;
			base.localPosition = vector;
		}
	}

	/// <summary>
	///   <para>The position of the pivot of this RectTransform relative to the anchor reference point.</para>
	/// </summary>
	public Vector2 anchoredPosition
	{
		get
		{
			INTERNAL_get_anchoredPosition(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_anchoredPosition(ref value);
		}
	}

	/// <summary>
	///   <para>The size of this RectTransform relative to the distances between the anchors.</para>
	/// </summary>
	public Vector2 sizeDelta
	{
		get
		{
			INTERNAL_get_sizeDelta(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_sizeDelta(ref value);
		}
	}

	/// <summary>
	///   <para>The normalized position in this RectTransform that it rotates around.</para>
	/// </summary>
	public Vector2 pivot
	{
		get
		{
			INTERNAL_get_pivot(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_pivot(ref value);
		}
	}

	internal extern Object drivenByObject
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	internal extern DrivenTransformProperties drivenProperties
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The offset of the lower left corner of the rectangle relative to the lower left anchor.</para>
	/// </summary>
	public Vector2 offsetMin
	{
		get
		{
			return anchoredPosition - Vector2.Scale(sizeDelta, pivot);
		}
		set
		{
			Vector2 vector = value - (anchoredPosition - Vector2.Scale(sizeDelta, pivot));
			sizeDelta -= vector;
			anchoredPosition += Vector2.Scale(vector, Vector2.one - pivot);
		}
	}

	/// <summary>
	///   <para>The offset of the upper right corner of the rectangle relative to the upper right anchor.</para>
	/// </summary>
	public Vector2 offsetMax
	{
		get
		{
			return anchoredPosition + Vector2.Scale(sizeDelta, Vector2.one - pivot);
		}
		set
		{
			Vector2 vector = value - (anchoredPosition + Vector2.Scale(sizeDelta, Vector2.one - pivot));
			sizeDelta += vector;
			anchoredPosition += Vector2.Scale(vector, pivot);
		}
	}

	public static event ReapplyDrivenProperties reapplyDrivenProperties;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_rect(out Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_anchorMin(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_anchorMin(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_anchorMax(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_anchorMax(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_anchoredPosition(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_anchoredPosition(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_sizeDelta(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_sizeDelta(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_pivot(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_pivot(ref Vector2 value);

	[RequiredByNativeCode]
	internal static void SendReapplyDrivenProperties(RectTransform driven)
	{
		if (RectTransform.reapplyDrivenProperties != null)
		{
			RectTransform.reapplyDrivenProperties(driven);
		}
	}

	/// <summary>
	///   <para>Get the corners of the calculated rectangle in the local space of its Transform.</para>
	/// </summary>
	/// <param name="fourCornersArray">The array that corners are filled into.</param>
	public void GetLocalCorners(Vector3[] fourCornersArray)
	{
		if (fourCornersArray == null || fourCornersArray.Length < 4)
		{
			Debug.LogError("Calling GetLocalCorners with an array that is null or has less than 4 elements.");
			return;
		}
		Rect rect = this.rect;
		float x = rect.x;
		float y = rect.y;
		float xMax = rect.xMax;
		float yMax = rect.yMax;
		ref Vector3 reference = ref fourCornersArray[0];
		reference = new Vector3(x, y, 0f);
		ref Vector3 reference2 = ref fourCornersArray[1];
		reference2 = new Vector3(x, yMax, 0f);
		ref Vector3 reference3 = ref fourCornersArray[2];
		reference3 = new Vector3(xMax, yMax, 0f);
		ref Vector3 reference4 = ref fourCornersArray[3];
		reference4 = new Vector3(xMax, y, 0f);
	}

	/// <summary>
	///   <para>Get the corners of the calculated rectangle in world space.</para>
	/// </summary>
	/// <param name="fourCornersArray">The ray that corners are filled into.</param>
	public void GetWorldCorners(Vector3[] fourCornersArray)
	{
		if (fourCornersArray == null || fourCornersArray.Length < 4)
		{
			Debug.LogError("Calling GetWorldCorners with an array that is null or has less than 4 elements.");
			return;
		}
		GetLocalCorners(fourCornersArray);
		Matrix4x4 matrix4x = base.transform.localToWorldMatrix;
		for (int i = 0; i < 4; i++)
		{
			ref Vector3 reference = ref fourCornersArray[i];
			reference = matrix4x.MultiplyPoint(fourCornersArray[i]);
		}
	}

	internal Rect GetRectInParentSpace()
	{
		Rect result = rect;
		Vector2 vector = offsetMin + Vector2.Scale(pivot, result.size);
		Transform transform = base.transform.parent;
		if ((bool)transform)
		{
			RectTransform component = transform.GetComponent<RectTransform>();
			if ((bool)component)
			{
				vector += Vector2.Scale(anchorMin, component.rect.size);
			}
		}
		result.x += vector.x;
		result.y += vector.y;
		return result;
	}

	public void SetInsetAndSizeFromParentEdge(Edge edge, float inset, float size)
	{
		int index = ((edge == Edge.Top || edge == Edge.Bottom) ? 1 : 0);
		bool flag = edge == Edge.Top || edge == Edge.Right;
		float value = (flag ? 1 : 0);
		Vector2 vector = anchorMin;
		vector[index] = value;
		anchorMin = vector;
		vector = anchorMax;
		vector[index] = value;
		anchorMax = vector;
		Vector2 vector2 = sizeDelta;
		vector2[index] = size;
		sizeDelta = vector2;
		Vector2 vector3 = anchoredPosition;
		vector3[index] = ((!flag) ? (inset + size * pivot[index]) : (0f - inset - size * (1f - pivot[index])));
		anchoredPosition = vector3;
	}

	public void SetSizeWithCurrentAnchors(Axis axis, float size)
	{
		Vector2 vector = sizeDelta;
		vector[(int)axis] = size - GetParentSize()[(int)axis] * (anchorMax[(int)axis] - anchorMin[(int)axis]);
		sizeDelta = vector;
	}

	private Vector2 GetParentSize()
	{
		RectTransform rectTransform = base.parent as RectTransform;
		if (!rectTransform)
		{
			return Vector2.zero;
		}
		return rectTransform.rect.size;
	}

	/// <summary>
	///   <para>Force the recalculation of RectTransforms internal data.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void ForceUpdateRectTransforms();
}
