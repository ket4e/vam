namespace UnityEngine.SocialPlatforms;

/// <summary>
///   <para>User presence state.</para>
/// </summary>
public enum UserState
{
	/// <summary>
	///   <para>The user is online.</para>
	/// </summary>
	Online,
	/// <summary>
	///   <para>The user is online but away from their computer.</para>
	/// </summary>
	OnlineAndAway,
	/// <summary>
	///   <para>The user is online but set their status to busy.</para>
	/// </summary>
	OnlineAndBusy,
	/// <summary>
	///   <para>The user is offline.</para>
	/// </summary>
	Offline,
	/// <summary>
	///   <para>The user is playing a game.</para>
	/// </summary>
	Playing
}
