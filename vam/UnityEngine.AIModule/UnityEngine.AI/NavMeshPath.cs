using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI;

/// <summary>
///   <para>A path as calculated by the navigation system.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[MovedFrom("UnityEngine")]
public sealed class NavMeshPath
{
	internal IntPtr m_Ptr;

	internal Vector3[] m_corners;

	/// <summary>
	///   <para>Corner points of the path. (Read Only)</para>
	/// </summary>
	public Vector3[] corners
	{
		get
		{
			CalculateCorners();
			return m_corners;
		}
	}

	/// <summary>
	///   <para>Status of the path. (Read Only)</para>
	/// </summary>
	public extern NavMeshPathStatus status
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>NavMeshPath constructor.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern NavMeshPath();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	private extern void DestroyNavMeshPath();

	~NavMeshPath()
	{
		DestroyNavMeshPath();
		m_Ptr = IntPtr.Zero;
	}

	/// <summary>
	///   <para>Calculate the corners for the path.</para>
	/// </summary>
	/// <param name="results">Array to store path corners.</param>
	/// <returns>
	///   <para>The number of corners along the path - including start and end points.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern int GetCornersNonAlloc(Vector3[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern Vector3[] CalculateCornersInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void ClearCornersInternal();

	/// <summary>
	///   <para>Erase all corner points from path.</para>
	/// </summary>
	public void ClearCorners()
	{
		ClearCornersInternal();
		m_corners = null;
	}

	private void CalculateCorners()
	{
		if (m_corners == null)
		{
			m_corners = CalculateCornersInternal();
		}
	}
}
