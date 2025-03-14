namespace UnityEngine.AI;

/// <summary>
///   <para>Specify which of the temporary data generated while building the NavMesh should be retained in memory after the process has completed.</para>
/// </summary>
public struct NavMeshBuildDebugSettings
{
	private byte m_Flags;

	/// <summary>
	///   <para>Specify which types of debug data to collect when building the NavMesh.</para>
	/// </summary>
	public NavMeshBuildDebugFlags flags
	{
		get
		{
			return (NavMeshBuildDebugFlags)m_Flags;
		}
		set
		{
			m_Flags = (byte)value;
		}
	}
}
