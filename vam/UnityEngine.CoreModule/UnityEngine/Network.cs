using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

/// <summary>
///   <para>The network class is at the heart of the network implementation and provides the core functions.</para>
/// </summary>
public sealed class Network
{
	/// <summary>
	///   <para>Set the password for the server (for incoming connections).</para>
	/// </summary>
	public static extern string incomingPassword
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Set the log level for network messages (default is Off).</para>
	/// </summary>
	public static extern NetworkLogLevel logLevel
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>All connected players.</para>
	/// </summary>
	public static extern NetworkPlayer[] connections
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Get the local NetworkPlayer instance.</para>
	/// </summary>
	public static NetworkPlayer player
	{
		get
		{
			NetworkPlayer result = default(NetworkPlayer);
			result.index = Internal_GetPlayer();
			return result;
		}
	}

	/// <summary>
	///   <para>Returns true if your peer type is client.</para>
	/// </summary>
	public static extern bool isClient
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Returns true if your peer type is server.</para>
	/// </summary>
	public static extern bool isServer
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The status of the peer type, i.e. if it is disconnected, connecting, server or client.</para>
	/// </summary>
	public static extern NetworkPeerType peerType
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The default send rate of network updates for all Network Views.</para>
	/// </summary>
	public static extern float sendRate
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Enable or disable the processing of network messages.</para>
	/// </summary>
	public static extern bool isMessageQueueRunning
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Get the current network time (seconds).</para>
	/// </summary>
	public static double time
	{
		get
		{
			Internal_GetTime(out var t);
			return t;
		}
	}

	/// <summary>
	///   <para>Get or set the minimum number of ViewID numbers in the ViewID pool given to clients by the server.</para>
	/// </summary>
	public static extern int minimumAllocatableViewIDs
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("No longer needed. This is now explicitly set in the InitializeServer function call. It is implicitly set when calling Connect depending on if an IP/port combination is used (useNat=false) or a GUID is used(useNat=true).")]
	public static extern bool useNat
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The IP address of the NAT punchthrough facilitator.</para>
	/// </summary>
	public static extern string natFacilitatorIP
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The port of the NAT punchthrough facilitator.</para>
	/// </summary>
	public static extern int natFacilitatorPort
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The IP address of the connection tester used in Network.TestConnection.</para>
	/// </summary>
	public static extern string connectionTesterIP
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The port of the connection tester used in Network.TestConnection.</para>
	/// </summary>
	public static extern int connectionTesterPort
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Set the maximum amount of connections/players allowed.</para>
	/// </summary>
	public static extern int maxConnections
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The IP address of the proxy server.</para>
	/// </summary>
	public static extern string proxyIP
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The port of the proxy server.</para>
	/// </summary>
	public static extern int proxyPort
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Indicate if proxy support is needed, in which case traffic is relayed through the proxy server.</para>
	/// </summary>
	public static extern bool useProxy
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Set the proxy server password.</para>
	/// </summary>
	public static extern string proxyPassword
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Initialize the server.</para>
	/// </summary>
	/// <param name="connections"></param>
	/// <param name="listenPort"></param>
	/// <param name="useNat"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern NetworkConnectionError InitializeServer(int connections, int listenPort, bool useNat);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern NetworkConnectionError Internal_InitializeServerDeprecated(int connections, int listenPort);

	/// <summary>
	///   <para>Initialize the server.</para>
	/// </summary>
	/// <param name="connections"></param>
	/// <param name="listenPort"></param>
	/// <param name="useNat"></param>
	[Obsolete("Use the IntializeServer(connections, listenPort, useNat) function instead")]
	public static NetworkConnectionError InitializeServer(int connections, int listenPort)
	{
		return Internal_InitializeServerDeprecated(connections, listenPort);
	}

	/// <summary>
	///   <para>Initializes security layer.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void InitializeSecurity();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort, [DefaultValue("\"\"")] string password);

	[ExcludeFromDocs]
	private static NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort)
	{
		string password = "";
		return Internal_ConnectToSingleIP(IP, remotePort, localPort, password);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern NetworkConnectionError Internal_ConnectToGuid(string guid, string password);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern NetworkConnectionError Internal_ConnectToIPs(string[] IP, int remotePort, int localPort, [DefaultValue("\"\"")] string password);

	[ExcludeFromDocs]
	private static NetworkConnectionError Internal_ConnectToIPs(string[] IP, int remotePort, int localPort)
	{
		string password = "";
		return Internal_ConnectToIPs(IP, remotePort, localPort, password);
	}

	/// <summary>
	///   <para>Connect to the specified host (ip or domain name) and server port.</para>
	/// </summary>
	/// <param name="IP"></param>
	/// <param name="remotePort"></param>
	/// <param name="password"></param>
	[ExcludeFromDocs]
	public static NetworkConnectionError Connect(string IP, int remotePort)
	{
		string password = "";
		return Connect(IP, remotePort, password);
	}

	/// <summary>
	///   <para>Connect to the specified host (ip or domain name) and server port.</para>
	/// </summary>
	/// <param name="IP"></param>
	/// <param name="remotePort"></param>
	/// <param name="password"></param>
	public static NetworkConnectionError Connect(string IP, int remotePort, [DefaultValue("\"\"")] string password)
	{
		return Internal_ConnectToSingleIP(IP, remotePort, 0, password);
	}

	/// <summary>
	///   <para>This function is exactly like Network.Connect but can accept an array of IP addresses.</para>
	/// </summary>
	/// <param name="IPs"></param>
	/// <param name="remotePort"></param>
	/// <param name="password"></param>
	[ExcludeFromDocs]
	public static NetworkConnectionError Connect(string[] IPs, int remotePort)
	{
		string password = "";
		return Connect(IPs, remotePort, password);
	}

	/// <summary>
	///   <para>This function is exactly like Network.Connect but can accept an array of IP addresses.</para>
	/// </summary>
	/// <param name="IPs"></param>
	/// <param name="remotePort"></param>
	/// <param name="password"></param>
	public static NetworkConnectionError Connect(string[] IPs, int remotePort, [DefaultValue("\"\"")] string password)
	{
		return Internal_ConnectToIPs(IPs, remotePort, 0, password);
	}

	/// <summary>
	///   <para>Connect to a server GUID. NAT punchthrough can only be performed this way.</para>
	/// </summary>
	/// <param name="GUID"></param>
	/// <param name="password"></param>
	[ExcludeFromDocs]
	public static NetworkConnectionError Connect(string GUID)
	{
		string password = "";
		return Connect(GUID, password);
	}

	/// <summary>
	///   <para>Connect to a server GUID. NAT punchthrough can only be performed this way.</para>
	/// </summary>
	/// <param name="GUID"></param>
	/// <param name="password"></param>
	public static NetworkConnectionError Connect(string GUID, [DefaultValue("\"\"")] string password)
	{
		return Internal_ConnectToGuid(GUID, password);
	}

	/// <summary>
	///   <para>Connect to the host represented by a HostData structure returned by the Master Server.</para>
	/// </summary>
	/// <param name="hostData"></param>
	/// <param name="password"></param>
	[ExcludeFromDocs]
	public static NetworkConnectionError Connect(HostData hostData)
	{
		string password = "";
		return Connect(hostData, password);
	}

	/// <summary>
	///   <para>Connect to the host represented by a HostData structure returned by the Master Server.</para>
	/// </summary>
	/// <param name="hostData"></param>
	/// <param name="password"></param>
	public static NetworkConnectionError Connect(HostData hostData, [DefaultValue("\"\"")] string password)
	{
		if (hostData == null)
		{
			throw new NullReferenceException();
		}
		if (hostData.guid.Length > 0 && hostData.useNat)
		{
			return Connect(hostData.guid, password);
		}
		return Connect(hostData.ip, hostData.port, password);
	}

	/// <summary>
	///   <para>Close all open connections and shuts down the network interface.</para>
	/// </summary>
	/// <param name="timeout"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void Disconnect([DefaultValue("200")] int timeout);

	[ExcludeFromDocs]
	public static void Disconnect()
	{
		int timeout = 200;
		Disconnect(timeout);
	}

	/// <summary>
	///   <para>Close the connection to another system.</para>
	/// </summary>
	/// <param name="target"></param>
	/// <param name="sendDisconnectionNotification"></param>
	public static void CloseConnection(NetworkPlayer target, bool sendDisconnectionNotification)
	{
		INTERNAL_CALL_CloseConnection(ref target, sendDisconnectionNotification);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_CloseConnection(ref NetworkPlayer target, bool sendDisconnectionNotification);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int Internal_GetPlayer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_AllocateViewID(out NetworkViewID viewID);

	/// <summary>
	///   <para>Query for the next available network view ID number and allocate it (reserve).</para>
	/// </summary>
	public static NetworkViewID AllocateViewID()
	{
		Internal_AllocateViewID(out var viewID);
		return viewID;
	}

	/// <summary>
	///   <para>Network instantiate a prefab.</para>
	/// </summary>
	/// <param name="prefab"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <param name="group"></param>
	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object prefab, Vector3 position, Quaternion rotation, int group)
	{
		return INTERNAL_CALL_Instantiate(prefab, ref position, ref rotation, group);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern Object INTERNAL_CALL_Instantiate(Object prefab, ref Vector3 position, ref Quaternion rotation, int group);

	/// <summary>
	///   <para>Destroy the object associated with this view ID across the network.</para>
	/// </summary>
	/// <param name="viewID"></param>
	public static void Destroy(NetworkViewID viewID)
	{
		INTERNAL_CALL_Destroy(ref viewID);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Destroy(ref NetworkViewID viewID);

	/// <summary>
	///   <para>Destroy the object across the network.</para>
	/// </summary>
	/// <param name="gameObject"></param>
	public static void Destroy(GameObject gameObject)
	{
		if (gameObject != null)
		{
			NetworkView component = gameObject.GetComponent<NetworkView>();
			if (component != null)
			{
				Destroy(component.viewID);
			}
			else
			{
				Debug.LogError("Couldn't destroy game object because no network view is attached to it.", gameObject);
			}
		}
	}

	/// <summary>
	///   <para>Destroy all the objects based on view IDs belonging to this player.</para>
	/// </summary>
	/// <param name="playerID"></param>
	public static void DestroyPlayerObjects(NetworkPlayer playerID)
	{
		INTERNAL_CALL_DestroyPlayerObjects(ref playerID);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DestroyPlayerObjects(ref NetworkPlayer playerID);

	private static void Internal_RemoveRPCs(NetworkPlayer playerID, NetworkViewID viewID, uint channelMask)
	{
		INTERNAL_CALL_Internal_RemoveRPCs(ref playerID, ref viewID, channelMask);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_RemoveRPCs(ref NetworkPlayer playerID, ref NetworkViewID viewID, uint channelMask);

	/// <summary>
	///   <para>Remove all RPC functions which belong to this player ID.</para>
	/// </summary>
	/// <param name="playerID"></param>
	public static void RemoveRPCs(NetworkPlayer playerID)
	{
		Internal_RemoveRPCs(playerID, NetworkViewID.unassigned, uint.MaxValue);
	}

	/// <summary>
	///   <para>Remove all RPC functions which belong to this player ID and were sent based on the given group.</para>
	/// </summary>
	/// <param name="playerID"></param>
	/// <param name="group"></param>
	public static void RemoveRPCs(NetworkPlayer playerID, int group)
	{
		Internal_RemoveRPCs(playerID, NetworkViewID.unassigned, (uint)(1 << group));
	}

	/// <summary>
	///   <para>Remove the RPC function calls accociated with this view ID number.</para>
	/// </summary>
	/// <param name="viewID"></param>
	public static void RemoveRPCs(NetworkViewID viewID)
	{
		Internal_RemoveRPCs(NetworkPlayer.unassigned, viewID, uint.MaxValue);
	}

	/// <summary>
	///   <para>Remove all RPC functions which belong to given group number.</para>
	/// </summary>
	/// <param name="group"></param>
	public static void RemoveRPCsInGroup(int group)
	{
		Internal_RemoveRPCs(NetworkPlayer.unassigned, NetworkViewID.unassigned, (uint)(1 << group));
	}

	/// <summary>
	///   <para>Set the level prefix which will then be prefixed to all network ViewID numbers.</para>
	/// </summary>
	/// <param name="prefix"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetLevelPrefix(int prefix);

	/// <summary>
	///   <para>The last ping time to the given player in milliseconds.</para>
	/// </summary>
	/// <param name="player"></param>
	public static int GetLastPing(NetworkPlayer player)
	{
		return INTERNAL_CALL_GetLastPing(ref player);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int INTERNAL_CALL_GetLastPing(ref NetworkPlayer player);

	/// <summary>
	///   <para>The last average ping time to the given player in milliseconds.</para>
	/// </summary>
	/// <param name="player"></param>
	public static int GetAveragePing(NetworkPlayer player)
	{
		return INTERNAL_CALL_GetAveragePing(ref player);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int INTERNAL_CALL_GetAveragePing(ref NetworkPlayer player);

	/// <summary>
	///   <para>Enable or disables the reception of messages in a specific group number from a specific player.</para>
	/// </summary>
	/// <param name="player"></param>
	/// <param name="group"></param>
	/// <param name="enabled"></param>
	public static void SetReceivingEnabled(NetworkPlayer player, int group, bool enabled)
	{
		INTERNAL_CALL_SetReceivingEnabled(ref player, group, enabled);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_SetReceivingEnabled(ref NetworkPlayer player, int group, bool enabled);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetSendingGlobal(int group, bool enabled);

	private static void Internal_SetSendingSpecific(NetworkPlayer player, int group, bool enabled)
	{
		INTERNAL_CALL_Internal_SetSendingSpecific(ref player, group, enabled);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_SetSendingSpecific(ref NetworkPlayer player, int group, bool enabled);

	/// <summary>
	///   <para>Enables or disables transmission of messages and RPC calls on a specific network group number.</para>
	/// </summary>
	/// <param name="group"></param>
	/// <param name="enabled"></param>
	public static void SetSendingEnabled(int group, bool enabled)
	{
		Internal_SetSendingGlobal(group, enabled);
	}

	/// <summary>
	///   <para>Enable or disable transmission of messages and RPC calls based on target network player as well as the network group.</para>
	/// </summary>
	/// <param name="player"></param>
	/// <param name="group"></param>
	/// <param name="enabled"></param>
	public static void SetSendingEnabled(NetworkPlayer player, int group, bool enabled)
	{
		Internal_SetSendingSpecific(player, group, enabled);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_GetTime(out double t);

	/// <summary>
	///   <para>Test this machines network connection.</para>
	/// </summary>
	/// <param name="forceTest"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern ConnectionTesterStatus TestConnection([DefaultValue("false")] bool forceTest);

	[ExcludeFromDocs]
	public static ConnectionTesterStatus TestConnection()
	{
		bool forceTest = false;
		return TestConnection(forceTest);
	}

	/// <summary>
	///   <para>Test the connection specifically for NAT punch-through connectivity.</para>
	/// </summary>
	/// <param name="forceTest"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern ConnectionTesterStatus TestConnectionNAT([DefaultValue("false")] bool forceTest);

	[ExcludeFromDocs]
	public static ConnectionTesterStatus TestConnectionNAT()
	{
		bool forceTest = false;
		return TestConnectionNAT(forceTest);
	}

	/// <summary>
	///   <para>Check if this machine has a public IP address.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool HavePublicAddress();
}
