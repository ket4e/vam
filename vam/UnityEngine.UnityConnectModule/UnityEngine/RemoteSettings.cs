using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Provides access to your remote settings.</para>
/// </summary>
[NativeHeader("UnityConnectScriptingClasses.h")]
[NativeHeader("Modules/UnityConnect/RemoteSettings.h")]
public static class RemoteSettings
{
	/// <summary>
	///   <para>Defines the delegate signature for handling RemoteSettings.Updated events.</para>
	/// </summary>
	public delegate void UpdatedEventHandler();

	public static event UpdatedEventHandler Updated;

	public static event Action BeforeFetchFromServer;

	public static event Action<bool, bool, int> Completed;

	[RequiredByNativeCode]
	internal static void RemoteSettingsUpdated(bool wasLastUpdatedFromServer)
	{
		RemoteSettings.Updated?.Invoke();
	}

	[RequiredByNativeCode]
	internal static void RemoteSettingsBeforeFetchFromServer()
	{
		RemoteSettings.BeforeFetchFromServer?.Invoke();
	}

	[RequiredByNativeCode]
	internal static void RemoteSettingsUpdateCompleted(bool wasLastUpdatedFromServer, bool settingsChanged, int response)
	{
		RemoteSettings.Completed?.Invoke(wasLastUpdatedFromServer, settingsChanged, response);
	}

	[Obsolete("Calling CallOnUpdate() is not necessary any more and should be removed. Use RemoteSettingsUpdated instead", true)]
	public static void CallOnUpdate()
	{
		throw new NotSupportedException("Calling CallOnUpdate() is not necessary any more and should be removed.");
	}

	/// <summary>
	///   <para>Forces the game to download the newest settings from the server and update its values.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void ForceUpdate();

	/// <summary>
	///   <para>Reports whether or not the settings available from the RemoteSettings object were received from the Analytics Service during the current session.</para>
	/// </summary>
	/// <returns>
	///   <para>True, if the remote settings file was received from the Analytics Service in the current session. False, if the remote settings file was received during an earlier session and cached.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool WasLastUpdatedFromServer();

	/// <summary>
	///   <para>Gets the value corresponding to remote setting identified by key, if it exists.</para>
	/// </summary>
	/// <param name="key">The key identifying the setting.</param>
	/// <param name="defaultValue">The default value to use if the setting identified by the key parameter cannot be found or is unavailable.</param>
	/// <returns>
	///   <para>The current value of the setting identified by key, or the default value.</para>
	/// </returns>
	[ExcludeFromDocs]
	public static int GetInt(string key)
	{
		return GetInt(key, 0);
	}

	/// <summary>
	///   <para>Gets the value corresponding to remote setting identified by key, if it exists.</para>
	/// </summary>
	/// <param name="key">The key identifying the setting.</param>
	/// <param name="defaultValue">The default value to use if the setting identified by the key parameter cannot be found or is unavailable.</param>
	/// <returns>
	///   <para>The current value of the setting identified by key, or the default value.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern int GetInt(string key, [DefaultValue("0")] int defaultValue);

	[ExcludeFromDocs]
	public static long GetLong(string key)
	{
		return GetLong(key, 0L);
	}

	/// <summary>
	///   <para>Gets the value corresponding to remote setting identified by key, if it exists.</para>
	/// </summary>
	/// <param name="key">The key identifying the setting.</param>
	/// <param name="defaultValue">The default value to use if the setting identified by the key parameter cannot be found or is unavailable.</param>
	/// <returns>
	///   <para>The current value of the setting identified by key, or the default value.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern long GetLong(string key, [DefaultValue("0")] long defaultValue);

	/// <summary>
	///   <para>Gets the value corresponding to remote setting identified by key, if it exists.</para>
	/// </summary>
	/// <param name="key">The key identifying the setting.</param>
	/// <param name="defaultValue">The default value to use if the setting identified by the key parameter cannot be found or is unavailable.</param>
	/// <returns>
	///   <para>The current value of the setting identified by key, or the default value.</para>
	/// </returns>
	[ExcludeFromDocs]
	public static float GetFloat(string key)
	{
		return GetFloat(key, 0f);
	}

	/// <summary>
	///   <para>Gets the value corresponding to remote setting identified by key, if it exists.</para>
	/// </summary>
	/// <param name="key">The key identifying the setting.</param>
	/// <param name="defaultValue">The default value to use if the setting identified by the key parameter cannot be found or is unavailable.</param>
	/// <returns>
	///   <para>The current value of the setting identified by key, or the default value.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue);

	/// <summary>
	///   <para>Gets the value corresponding to remote setting identified by key, if it exists.</para>
	/// </summary>
	/// <param name="key">The key identifying the setting.</param>
	/// <param name="defaultValue">The default value to use if the setting identified by the key parameter cannot be found or is unavailable.</param>
	/// <returns>
	///   <para>The current value of the setting identified by key, or the default value.</para>
	/// </returns>
	[ExcludeFromDocs]
	public static string GetString(string key)
	{
		return GetString(key, "");
	}

	/// <summary>
	///   <para>Gets the value corresponding to remote setting identified by key, if it exists.</para>
	/// </summary>
	/// <param name="key">The key identifying the setting.</param>
	/// <param name="defaultValue">The default value to use if the setting identified by the key parameter cannot be found or is unavailable.</param>
	/// <returns>
	///   <para>The current value of the setting identified by key, or the default value.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern string GetString(string key, [DefaultValue("\"\"")] string defaultValue);

	/// <summary>
	///   <para>Gets the value corresponding to remote setting identified by key, if it exists.</para>
	/// </summary>
	/// <param name="key">The key identifying the setting.</param>
	/// <param name="defaultValue">The default value to use if the setting identified by the key parameter cannot be found or is unavailable.</param>
	/// <returns>
	///   <para>The current value of the setting identified by key, or the default value.</para>
	/// </returns>
	[ExcludeFromDocs]
	public static bool GetBool(string key)
	{
		return GetBool(key, defaultValue: false);
	}

	/// <summary>
	///   <para>Gets the value corresponding to remote setting identified by key, if it exists.</para>
	/// </summary>
	/// <param name="key">The key identifying the setting.</param>
	/// <param name="defaultValue">The default value to use if the setting identified by the key parameter cannot be found or is unavailable.</param>
	/// <returns>
	///   <para>The current value of the setting identified by key, or the default value.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool GetBool(string key, [DefaultValue("false")] bool defaultValue);

	/// <summary>
	///   <para>Reports whether the specified key exists in the remote settings configuration.</para>
	/// </summary>
	/// <param name="key">The key identifying the setting.</param>
	/// <returns>
	///   <para>True, if the key exists.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool HasKey(string key);

	/// <summary>
	///   <para>Gets the number of keys in the remote settings configuration.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern int GetCount();

	/// <summary>
	///   <para>Gets an array containing all the keys in the remote settings configuration.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern string[] GetKeys();
}
