using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match;

/// <summary>
///   <para>Details about a UNET MatchMaker match.</para>
/// </summary>
public class MatchInfo
{
	/// <summary>
	///   <para>IP address of the host of the match,.</para>
	/// </summary>
	public string address { get; private set; }

	/// <summary>
	///   <para>Port of the host of the match.</para>
	/// </summary>
	public int port { get; private set; }

	/// <summary>
	///   <para>The numeric domain for the match.</para>
	/// </summary>
	public int domain { get; private set; }

	/// <summary>
	///   <para>The unique ID of this match.</para>
	/// </summary>
	public NetworkID networkId { get; private set; }

	/// <summary>
	///   <para>The binary access token this client uses to authenticate its session for future commands.</para>
	/// </summary>
	public NetworkAccessToken accessToken { get; private set; }

	/// <summary>
	///   <para>NodeID for this member client in the match.</para>
	/// </summary>
	public NodeID nodeId { get; private set; }

	/// <summary>
	///   <para>This flag indicates whether or not the match is using a Relay server.</para>
	/// </summary>
	public bool usingRelay { get; private set; }

	public MatchInfo()
	{
	}

	internal MatchInfo(CreateMatchResponse matchResponse)
	{
		address = matchResponse.address;
		port = matchResponse.port;
		domain = matchResponse.domain;
		networkId = matchResponse.networkId;
		accessToken = new NetworkAccessToken(matchResponse.accessTokenString);
		nodeId = matchResponse.nodeId;
		usingRelay = matchResponse.usingRelay;
	}

	internal MatchInfo(JoinMatchResponse matchResponse)
	{
		address = matchResponse.address;
		port = matchResponse.port;
		domain = matchResponse.domain;
		networkId = matchResponse.networkId;
		accessToken = new NetworkAccessToken(matchResponse.accessTokenString);
		nodeId = matchResponse.nodeId;
		usingRelay = matchResponse.usingRelay;
	}

	public override string ToString()
	{
		return UnityString.Format("{0} @ {1}:{2} [{3},{4}]", networkId, address, port, nodeId, usingRelay);
	}
}
