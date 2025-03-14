using System;

namespace UnityEngine;

/// <summary>
///   <para>An enumeration of transform properties that can be driven on a RectTransform by an object.</para>
/// </summary>
[Flags]
public enum DrivenTransformProperties
{
	/// <summary>
	///   <para>Deselects all driven properties.</para>
	/// </summary>
	None = 0,
	/// <summary>
	///   <para>Selects all driven properties.</para>
	/// </summary>
	All = -1,
	/// <summary>
	///   <para>Selects driven property RectTransform.anchoredPosition.x.</para>
	/// </summary>
	AnchoredPositionX = 2,
	/// <summary>
	///   <para>Selects driven property RectTransform.anchoredPosition.y.</para>
	/// </summary>
	AnchoredPositionY = 4,
	/// <summary>
	///   <para>Selects driven property RectTransform.anchoredPosition3D.z.</para>
	/// </summary>
	AnchoredPositionZ = 8,
	/// <summary>
	///   <para>Selects driven property Transform.localRotation.</para>
	/// </summary>
	Rotation = 0x10,
	/// <summary>
	///   <para>Selects driven property Transform.localScale.x.</para>
	/// </summary>
	ScaleX = 0x20,
	/// <summary>
	///   <para>Selects driven property Transform.localScale.y.</para>
	/// </summary>
	ScaleY = 0x40,
	/// <summary>
	///   <para>Selects driven property Transform.localScale.z.</para>
	/// </summary>
	ScaleZ = 0x80,
	/// <summary>
	///   <para>Selects driven property RectTransform.anchorMin.x.</para>
	/// </summary>
	AnchorMinX = 0x100,
	/// <summary>
	///   <para>Selects driven property RectTransform.anchorMin.y.</para>
	/// </summary>
	AnchorMinY = 0x200,
	/// <summary>
	///   <para>Selects driven property RectTransform.anchorMax.x.</para>
	/// </summary>
	AnchorMaxX = 0x400,
	/// <summary>
	///   <para>Selects driven property RectTransform.anchorMax.y.</para>
	/// </summary>
	AnchorMaxY = 0x800,
	/// <summary>
	///   <para>Selects driven property RectTransform.sizeDelta.x.</para>
	/// </summary>
	SizeDeltaX = 0x1000,
	/// <summary>
	///   <para>Selects driven property RectTransform.sizeDelta.y.</para>
	/// </summary>
	SizeDeltaY = 0x2000,
	/// <summary>
	///   <para>Selects driven property RectTransform.pivot.x.</para>
	/// </summary>
	PivotX = 0x4000,
	/// <summary>
	///   <para>Selects driven property RectTransform.pivot.y.</para>
	/// </summary>
	PivotY = 0x8000,
	/// <summary>
	///   <para>Selects driven property RectTransform.anchoredPosition.</para>
	/// </summary>
	AnchoredPosition = 6,
	/// <summary>
	///   <para>Selects driven property RectTransform.anchoredPosition3D.</para>
	/// </summary>
	AnchoredPosition3D = 0xE,
	/// <summary>
	///   <para>Selects driven property combining ScaleX, ScaleY &amp;&amp; ScaleZ.</para>
	/// </summary>
	Scale = 0xE0,
	/// <summary>
	///   <para>Selects driven property combining AnchorMinX and AnchorMinY.</para>
	/// </summary>
	AnchorMin = 0x300,
	/// <summary>
	///   <para>Selects driven property combining AnchorMaxX and AnchorMaxY.</para>
	/// </summary>
	AnchorMax = 0xC00,
	/// <summary>
	///   <para>Selects driven property combining AnchorMinX, AnchorMinY, AnchorMaxX and AnchorMaxY.</para>
	/// </summary>
	Anchors = 0xF00,
	/// <summary>
	///   <para>Selects driven property combining SizeDeltaX and SizeDeltaY.</para>
	/// </summary>
	SizeDelta = 0x3000,
	/// <summary>
	///   <para>Selects driven property combining PivotX and PivotY.</para>
	/// </summary>
	Pivot = 0xC000
}
