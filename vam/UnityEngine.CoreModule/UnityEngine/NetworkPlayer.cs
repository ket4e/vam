using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The NetworkPlayer is a data structure with which you can locate another player over the network.</para>
/// </summary>
[RequiredByNativeCode(Optional = true)]
public struct NetworkPlayer
{
	internal int index;

	/// <summary>
	///   <para>The IP address of this player.</para>
	/// </summary>
	public string ipAddress
	{
		get
		{
			if (index == Internal_GetPlayerIndex())
			{
				return Internal_GetLocalIP();
			}
			return Internal_GetIPAddress(index);
		}
	}

	/// <summary>
	///   <para>The port of this player.</para>
	/// </summary>
	public int port
	{
		get
		{
			if (index == Internal_GetPlayerIndex())
			{
				return Internal_GetLocalPort();
			}
			return Internal_GetPort(index);
		}
	}

	/// <summary>
	///   <para>The GUID for this player, used when connecting with NAT punchthrough.</para>
	/// </summary>
	public string guid
	{
		get
		{
			if (index == Internal_GetPlayerIndex())
			{
				return Internal_GetLocalGUID();
			}
			return Internal_GetGUID(index);
		}
	}

	/// <summary>
	///   <para>Returns the external IP address of the network interface.</para>
	/// </summary>
	public string externalIP => Internal_GetExternalIP();

	/// <summary>
	///   <para>Returns the external port of the network interface.</para>
	/// </summary>
	public int externalPort => Internal_GetExternalPort();

	internal static NetworkPlayer unassigned
	{
		get
		{
			NetworkPlayer result = default(NetworkPlayer);
			result.index = -1;
			return result;
		}
	}

	public NetworkPlayer(string ip, int port)
	{
		Debug.LogError("Not yet implemented");
		index = 0;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string Internal_GetIPAddress(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int Internal_GetPort(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string Internal_GetExternalIP();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int Internal_GetExternalPort();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string Internal_GetLocalIP();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int Internal_GetLocalPort();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int Internal_GetPlayerIndex();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string Internal_GetGUID(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string Internal_GetLocalGUID();

	public static bool operator ==(NetworkPlayer lhs, NetworkPlayer rhs)
	{
		return lhs.index == rhs.index;
	}

	public static bool operator !=(NetworkPlayer lhs, NetworkPlayer rhs)
	{
		return lhs.index != rhs.index;
	}

	public override int GetHashCode()
	{
		return index.GetHashCode();
	}

	public override bool Equals(object other)
	{
		if (!(other is NetworkPlayer networkPlayer))
		{
			return false;
		}
		return networkPlayer.index == index;
	}

	/// <summary>
	///   <para>Returns the index number for this network player.</para>
	/// </summary>
	public override string ToString()
	{
		return index.ToString();
	}
}
