using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>A session-unique identifier for trackables in the environment, e.g., planes and feature points.</para>
/// </summary>
[NativeHeader("Modules/XR/XRManagedBindings.h")]
[UsedByNativeCode]
public struct TrackableId
{
	private static TrackableId s_InvalidId = default(TrackableId);

	private ulong m_SubId1;

	private ulong m_SubId2;

	/// <summary>
	///   <para>Represents an invalid id.</para>
	/// </summary>
	public static TrackableId InvalidId => s_InvalidId;

	/// <summary>
	///   <para>Generates a nicely formatted version of the id.</para>
	/// </summary>
	/// <returns>
	///   <para>A string unique to this id</para>
	/// </returns>
	public override string ToString()
	{
		return string.Format("{0}-{1}", m_SubId1.ToString("X16"), m_SubId2.ToString("X16"));
	}

	public override int GetHashCode()
	{
		return m_SubId1.GetHashCode() ^ m_SubId2.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj is TrackableId trackableId)
		{
			return m_SubId1 == trackableId.m_SubId1 && m_SubId2 == trackableId.m_SubId2;
		}
		return false;
	}

	public static bool operator ==(TrackableId id1, TrackableId id2)
	{
		return id1.m_SubId1 == id2.m_SubId1 && id1.m_SubId2 == id2.m_SubId2;
	}

	public static bool operator !=(TrackableId id1, TrackableId id2)
	{
		return id1.m_SubId1 != id2.m_SubId1 || id1.m_SubId2 != id2.m_SubId2;
	}
}
