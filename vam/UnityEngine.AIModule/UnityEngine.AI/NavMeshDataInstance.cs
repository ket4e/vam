namespace UnityEngine.AI;

/// <summary>
///   <para>The instance is returned when adding NavMesh data.</para>
/// </summary>
public struct NavMeshDataInstance
{
	private int m_Handle;

	/// <summary>
	///   <para>True if the NavMesh data is added to the navigation system - otherwise false (Read Only).</para>
	/// </summary>
	public bool valid => m_Handle != 0 && NavMesh.IsValidNavMeshDataHandle(m_Handle);

	internal int id
	{
		get
		{
			return m_Handle;
		}
		set
		{
			m_Handle = value;
		}
	}

	/// <summary>
	///   <para>Get or set the owning Object.</para>
	/// </summary>
	public Object owner
	{
		get
		{
			return NavMesh.InternalGetOwner(id);
		}
		set
		{
			int ownerID = ((value != null) ? value.GetInstanceID() : 0);
			if (!NavMesh.InternalSetOwner(id, ownerID))
			{
				Debug.LogError("Cannot set 'owner' on an invalid NavMeshDataInstance");
			}
		}
	}

	/// <summary>
	///   <para>Removes this instance from the NavMesh system.</para>
	/// </summary>
	public void Remove()
	{
		NavMesh.RemoveNavMeshDataInternal(id);
	}
}
