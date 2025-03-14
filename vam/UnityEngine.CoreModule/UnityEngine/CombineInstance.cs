namespace UnityEngine;

/// <summary>
///   <para>Struct used to describe meshes to be combined using Mesh.CombineMeshes.</para>
/// </summary>
public struct CombineInstance
{
	private int m_MeshInstanceID;

	private int m_SubMeshIndex;

	private Matrix4x4 m_Transform;

	private Vector4 m_LightmapScaleOffset;

	private Vector4 m_RealtimeLightmapScaleOffset;

	/// <summary>
	///   <para>Mesh to combine.</para>
	/// </summary>
	public Mesh mesh
	{
		get
		{
			return Mesh.FromInstanceID(m_MeshInstanceID);
		}
		set
		{
			m_MeshInstanceID = ((value != null) ? value.GetInstanceID() : 0);
		}
	}

	/// <summary>
	///   <para>Sub-Mesh index of the Mesh.</para>
	/// </summary>
	public int subMeshIndex
	{
		get
		{
			return m_SubMeshIndex;
		}
		set
		{
			m_SubMeshIndex = value;
		}
	}

	/// <summary>
	///   <para>Matrix to transform the Mesh with before combining.</para>
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
	///   <para>The baked lightmap UV scale and offset applied to the Mesh.</para>
	/// </summary>
	public Vector4 lightmapScaleOffset
	{
		get
		{
			return m_LightmapScaleOffset;
		}
		set
		{
			m_LightmapScaleOffset = value;
		}
	}

	/// <summary>
	///   <para>The realtime lightmap UV scale and offset applied to the Mesh.</para>
	/// </summary>
	public Vector4 realtimeLightmapScaleOffset
	{
		get
		{
			return m_RealtimeLightmapScaleOffset;
		}
		set
		{
			m_RealtimeLightmapScaleOffset = value;
		}
	}
}
