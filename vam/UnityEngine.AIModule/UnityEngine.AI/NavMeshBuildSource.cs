using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.AI;

/// <summary>
///   <para>The input to the NavMesh builder is a list of NavMesh build sources.</para>
/// </summary>
[UsedByNativeCode]
public struct NavMeshBuildSource
{
	private Matrix4x4 m_Transform;

	private Vector3 m_Size;

	private NavMeshBuildSourceShape m_Shape;

	private int m_Area;

	private int m_InstanceID;

	private int m_ComponentID;

	/// <summary>
	///   <para>Describes the local to world transformation matrix of the build source. That is, position and orientation and scale of the shape.</para>
	/// </summary>
	public Matrix4x4 transform
	{
		get
		{
			return m_Transform;
		}
		set
		{
			m_Transform = value;
		}
	}

	/// <summary>
	///   <para>Describes the dimensions of the shape.</para>
	/// </summary>
	public Vector3 size
	{
		get
		{
			return m_Size;
		}
		set
		{
			m_Size = value;
		}
	}

	/// <summary>
	///   <para>The type of the shape this source describes. See Also: NavMeshBuildSourceShape.</para>
	/// </summary>
	public NavMeshBuildSourceShape shape
	{
		get
		{
			return m_Shape;
		}
		set
		{
			m_Shape = value;
		}
	}

	/// <summary>
	///   <para>Describes the area type of the NavMesh surface for this object.</para>
	/// </summary>
	public int area
	{
		get
		{
			return m_Area;
		}
		set
		{
			m_Area = value;
		}
	}

	/// <summary>
	///   <para>Describes the object referenced for Mesh and Terrain types of input sources.</para>
	/// </summary>
	public Object sourceObject
	{
		get
		{
			return InternalGetObject(m_InstanceID);
		}
		set
		{
			m_InstanceID = ((value != null) ? value.GetInstanceID() : 0);
		}
	}

	/// <summary>
	///   <para>Points to the owning component - if available, otherwise null.</para>
	/// </summary>
	public Component component
	{
		get
		{
			return InternalGetComponent(m_ComponentID);
		}
		set
		{
			m_ComponentID = ((value != null) ? value.GetInstanceID() : 0);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern Component InternalGetComponent(int instanceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern Object InternalGetObject(int instanceID);
}
