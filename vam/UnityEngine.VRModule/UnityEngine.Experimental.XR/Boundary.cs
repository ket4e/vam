using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Experimental.XR;

[MovedFrom("UnityEngine.Experimental.VR")]
public static class Boundary
{
	public enum Type
	{
		PlayArea,
		TrackedArea
	}

	public static extern bool visible
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public static extern bool configured
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	[ExcludeFromDocs]
	public static bool TryGetDimensions(out Vector3 dimensionsOut)
	{
		Type boundaryType = Type.PlayArea;
		return TryGetDimensions(out dimensionsOut, boundaryType);
	}

	public static bool TryGetDimensions(out Vector3 dimensionsOut, [DefaultValue("Type.PlayArea")] Type boundaryType)
	{
		return TryGetDimensionsInternal(out dimensionsOut, (int)boundaryType);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool TryGetDimensionsInternal(out Vector3 dimensionsOut, int boundaryType);

	[ExcludeFromDocs]
	public static bool TryGetGeometry(List<Vector3> geometry)
	{
		Type boundaryType = Type.PlayArea;
		return TryGetGeometry(geometry, boundaryType);
	}

	public static bool TryGetGeometry(List<Vector3> geometry, [DefaultValue("Type.PlayArea")] Type boundaryType)
	{
		if (geometry == null)
		{
			throw new ArgumentNullException("geometry");
		}
		geometry.Clear();
		return TryGetGeometryInternal(geometry, (int)boundaryType);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool TryGetGeometryInternal(object geometryOut, int boundaryType);
}
