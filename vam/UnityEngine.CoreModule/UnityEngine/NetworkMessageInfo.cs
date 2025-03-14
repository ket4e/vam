using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>This data structure contains information on a message just received from the network.</para>
/// </summary>
[RequiredByNativeCode(Optional = true)]
public struct NetworkMessageInfo
{
	private double m_TimeStamp;

	private NetworkPlayer m_Sender;

	private NetworkViewID m_ViewID;

	/// <summary>
	///   <para>The time stamp when the Message was sent in seconds.</para>
	/// </summary>
	public double timestamp => m_TimeStamp;

	/// <summary>
	///   <para>The player who sent this network message (owner).</para>
	/// </summary>
	public NetworkPlayer sender => m_Sender;

	/// <summary>
	///   <para>The NetworkView who sent this message.</para>
	/// </summary>
	public NetworkView networkView
	{
		get
		{
			if (m_ViewID == NetworkViewID.unassigned)
			{
				Debug.LogError("No NetworkView is assigned to this NetworkMessageInfo object. Note that this is expected in OnNetworkInstantiate().");
				return NullNetworkView();
			}
			return NetworkView.Find(m_ViewID);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern NetworkView NullNetworkView();
}
