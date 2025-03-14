namespace UnityEngine.AI;

/// <summary>
///   <para>An instance representing a link available for pathfinding.</para>
/// </summary>
public struct NavMeshLinkInstance
{
	private int m_Handle;

	/// <summary>
	///   <para>True if the NavMesh link is added to the navigation system - otherwise false (Read Only).</para>
	/// </summary>
	public bool valid => m_Handle != 0 && NavMesh.IsValidLinkHandle(m_Handle);

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
			return NavMesh.InternalGetLinkOwner(id);
		}
		set
		{
			int ownerID = ((value != null) ? value.GetInstanceID() : 0);
			if (!NavMesh.InternalSetLinkOwner(id, ownerID))
			{
				Debug.LogError("Cannot set 'owner' on an invalid NavMeshLinkInstance");
			}
		}
	}

	/// <summary>
	///   <para>Removes this instance from the game.</para>
	/// </summary>
	public void Remove()
	{
		NavMesh.RemoveLinkInternal(id);
	}
}
