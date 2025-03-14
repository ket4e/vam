using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>This is the data structure for holding individual host information.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode(Optional = true)]
public sealed class HostData
{
	private int m_Nat;

	private string m_GameType;

	private string m_GameName;

	private int m_ConnectedPlayers;

	private int m_PlayerLimit;

	private string[] m_IP;

	private int m_Port;

	private int m_PasswordProtected;

	private string m_Comment;

	private string m_GUID;

	/// <summary>
	///   <para>Does this server require NAT punchthrough?</para>
	/// </summary>
	public bool useNat
	{
		get
		{
			return m_Nat != 0;
		}
		set
		{
			m_Nat = (value ? 1 : 0);
		}
	}

	/// <summary>
	///   <para>The type of the game (like "MyUniqueGameType").</para>
	/// </summary>
	public string gameType
	{
		get
		{
			return m_GameType;
		}
		set
		{
			m_GameType = value;
		}
	}

	/// <summary>
	///   <para>The name of the game (like John Doe's Game).</para>
	/// </summary>
	public string gameName
	{
		get
		{
			return m_GameName;
		}
		set
		{
			m_GameName = value;
		}
	}

	/// <summary>
	///   <para>Currently connected players.</para>
	/// </summary>
	public int connectedPlayers
	{
		get
		{
			return m_ConnectedPlayers;
		}
		set
		{
			m_ConnectedPlayers = value;
		}
	}

	/// <summary>
	///   <para>Maximum players limit.</para>
	/// </summary>
	public int playerLimit
	{
		get
		{
			return m_PlayerLimit;
		}
		set
		{
			m_PlayerLimit = value;
		}
	}

	/// <summary>
	///   <para>Server IP address.</para>
	/// </summary>
	public string[] ip
	{
		get
		{
			return m_IP;
		}
		set
		{
			m_IP = value;
		}
	}

	/// <summary>
	///   <para>Server port.</para>
	/// </summary>
	public int port
	{
		get
		{
			return m_Port;
		}
		set
		{
			m_Port = value;
		}
	}

	/// <summary>
	///   <para>Does the server require a password?</para>
	/// </summary>
	public bool passwordProtected
	{
		get
		{
			return m_PasswordProtected != 0;
		}
		set
		{
			m_PasswordProtected = (value ? 1 : 0);
		}
	}

	/// <summary>
	///   <para>A miscellaneous comment (can hold data).</para>
	/// </summary>
	public string comment
	{
		get
		{
			return m_Comment;
		}
		set
		{
			m_Comment = value;
		}
	}

	/// <summary>
	///   <para>The GUID of the host, needed when connecting with NAT punchthrough.</para>
	/// </summary>
	public string guid
	{
		get
		{
			return m_GUID;
		}
		set
		{
			m_GUID = value;
		}
	}
}
