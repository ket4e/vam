using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics;

/// <summary>
///   <para>Accesses for Analytics session information (common for all game instances).</para>
/// </summary>
[RequiredByNativeCode]
[NativeHeader("UnityConnectScriptingClasses.h")]
[NativeHeader("Modules/UnityConnect/UnityConnectClient.h")]
public static class AnalyticsSessionInfo
{
	/// <summary>
	///   <para>This event occurs when a Analytics session state changes.</para>
	/// </summary>
	/// <param name="sessionState">Current session state.</param>
	/// <param name="sessionId">Current session id.</param>
	/// <param name="sessionElapsedTime">Game player current session time.</param>
	/// <param name="sessionChanged">Set to true when sessionId has changed.</param>
	public delegate void SessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged);

	/// <summary>
	///   <para>Session state.</para>
	/// </summary>
	public static extern AnalyticsSessionState sessionState
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("GetPlayerSessionState")]
		get;
	}

	/// <summary>
	///   <para>Session id is used for tracking player game session.</para>
	/// </summary>
	public static extern long sessionId
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("GetPlayerSessionId")]
		get;
	}

	/// <summary>
	///   <para>Session time since the begining of player game session.</para>
	/// </summary>
	public static extern long sessionElapsedTime
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("GetPlayerSessionElapsedTime")]
		get;
	}

	/// <summary>
	///   <para>UserId is random GUID to track a player and is persisted across game session.</para>
	/// </summary>
	public static extern string userId
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("GetUserId")]
		get;
	}

	public static event SessionStateChanged sessionStateChanged;

	[RequiredByNativeCode]
	internal static void CallSessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged)
	{
		AnalyticsSessionInfo.sessionStateChanged?.Invoke(sessionState, sessionId, sessionElapsedTime, sessionChanged);
	}
}
