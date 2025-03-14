using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A component can be designed to drive a RectTransform. The DrivenRectTransformTracker struct is used to specify which RectTransforms it is driving.</para>
/// </summary>
public struct DrivenRectTransformTracker
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern bool CanRecordModifications();

	/// <summary>
	///   <para>Add a RectTransform to be driven.</para>
	/// </summary>
	/// <param name="driver">The object to drive properties.</param>
	/// <param name="rectTransform">The RectTransform to be driven.</param>
	/// <param name="drivenProperties">The properties to be driven.</param>
	public void Add(Object driver, RectTransform rectTransform, DrivenTransformProperties drivenProperties)
	{
	}

	[Obsolete("revertValues parameter is ignored. Please use Clear() instead.")]
	public void Clear(bool revertValues)
	{
		Clear();
	}

	/// <summary>
	///   <para>Clear the list of RectTransforms being driven.</para>
	/// </summary>
	public void Clear()
	{
	}
}
