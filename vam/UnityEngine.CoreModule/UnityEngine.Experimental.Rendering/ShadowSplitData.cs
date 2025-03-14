using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Describes the culling information for a given shadow split (e.g. directional cascade).</para>
/// </summary>
[UsedByNativeCode]
public struct ShadowSplitData
{
	[StructLayout(LayoutKind.Sequential, Size = 160)]
	[UnsafeValueType]
	[CompilerGenerated]
	public struct _003C_cullingPlanes_003E__FixedBuffer6
	{
		public float FixedElementField;
	}

	/// <summary>
	///   <para>The number of culling planes.</para>
	/// </summary>
	public int cullingPlaneCount;

	private _003C_cullingPlanes_003E__FixedBuffer6 _cullingPlanes;

	/// <summary>
	///   <para>The culling sphere.  The first three components of the vector describe the sphere center, and the last component specifies the radius.</para>
	/// </summary>
	public Vector4 cullingSphere;

	/// <summary>
	///   <para>Gets a culling plane.</para>
	/// </summary>
	/// <param name="index">The culling plane index.</param>
	/// <returns>
	///   <para>The culling plane.</para>
	/// </returns>
	public unsafe Plane GetCullingPlane(int index)
	{
		if (index < 0 || index >= cullingPlaneCount || index >= 10)
		{
			throw new IndexOutOfRangeException("Invalid plane index");
		}
		fixed (float* ptr = &_cullingPlanes.FixedElementField)
		{
			return new Plane(new Vector3(System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4), System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 1), System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 2)), System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 3));
		}
	}

	/// <summary>
	///   <para>Sets a culling plane.</para>
	/// </summary>
	/// <param name="index">The index of the culling plane to set.</param>
	/// <param name="plane">The culling plane.</param>
	public unsafe void SetCullingPlane(int index, Plane plane)
	{
		if (index < 0 || index >= cullingPlaneCount || index >= 10)
		{
			throw new IndexOutOfRangeException("Invalid plane index");
		}
		fixed (float* ptr = &_cullingPlanes.FixedElementField)
		{
			System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4) = plane.normal.x;
			System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 1) = plane.normal.y;
			System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 2) = plane.normal.z;
			System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 3) = plane.distance;
		}
	}
}
