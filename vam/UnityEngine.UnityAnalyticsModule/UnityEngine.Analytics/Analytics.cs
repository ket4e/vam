using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UnityEngine.Analytics;

/// <summary>
///   <para>Unity Analytics provides insight into your game users e.g. DAU, MAU.</para>
/// </summary>
public static class Analytics
{
	private static UnityAnalyticsHandler s_UnityAnalyticsHandler;

	/// <summary>
	///   <para>Controls whether to limit user tracking at runtime.</para>
	/// </summary>
	public static bool limitUserTracking
	{
		get
		{
			return UnityAnalyticsHandler.limitUserTracking;
		}
		set
		{
			UnityAnalyticsHandler.limitUserTracking = value;
		}
	}

	/// <summary>
	///   <para>Controls whether the sending of device stats at runtime is enabled.</para>
	/// </summary>
	public static bool deviceStatsEnabled
	{
		get
		{
			return UnityAnalyticsHandler.deviceStatsEnabled;
		}
		set
		{
			UnityAnalyticsHandler.deviceStatsEnabled = value;
		}
	}

	/// <summary>
	///   <para>Controls whether the Analytics service is enabled at runtime.</para>
	/// </summary>
	public static bool enabled
	{
		get
		{
			return GetUnityAnalyticsHandler()?.enabled ?? false;
		}
		set
		{
			UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
			if (unityAnalyticsHandler != null)
			{
				unityAnalyticsHandler.enabled = value;
			}
		}
	}

	internal static UnityAnalyticsHandler GetUnityAnalyticsHandler()
	{
		if (s_UnityAnalyticsHandler == null)
		{
			s_UnityAnalyticsHandler = new UnityAnalyticsHandler();
		}
		if (s_UnityAnalyticsHandler.IsInitialized())
		{
			return s_UnityAnalyticsHandler;
		}
		return null;
	}

	/// <summary>
	///   <para>Attempts to flush immediately all queued analytics events to the network and filesystem cache if possible (optional).</para>
	/// </summary>
	public static AnalyticsResult FlushEvents()
	{
		UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
		if (unityAnalyticsHandler == null)
		{
			return AnalyticsResult.NotInitialized;
		}
		return (!unityAnalyticsHandler.FlushEvents()) ? AnalyticsResult.NotInitialized : AnalyticsResult.Ok;
	}

	/// <summary>
	///   <para>User Demographics (optional).</para>
	/// </summary>
	/// <param name="userId">User id.</param>
	public static AnalyticsResult SetUserId(string userId)
	{
		if (string.IsNullOrEmpty(userId))
		{
			throw new ArgumentException("Cannot set userId to an empty or null string");
		}
		return GetUnityAnalyticsHandler()?.SetUserId(userId) ?? AnalyticsResult.NotInitialized;
	}

	/// <summary>
	///   <para>User Demographics (optional).</para>
	/// </summary>
	/// <param name="gender">Gender of user can be "Female", "Male", or "Unknown".</param>
	public static AnalyticsResult SetUserGender(Gender gender)
	{
		return GetUnityAnalyticsHandler()?.SetUserGender(gender) ?? AnalyticsResult.NotInitialized;
	}

	/// <summary>
	///   <para>User Demographics (optional).</para>
	/// </summary>
	/// <param name="birthYear">Birth year of user. Must be 4-digit year format, only.</param>
	public static AnalyticsResult SetUserBirthYear(int birthYear)
	{
		UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
		if (s_UnityAnalyticsHandler == null)
		{
			return AnalyticsResult.NotInitialized;
		}
		return unityAnalyticsHandler.SetUserBirthYear(birthYear);
	}

	/// <summary>
	///   <para>Tracking Monetization (optional).</para>
	/// </summary>
	/// <param name="productId">The id of the purchased item.</param>
	/// <param name="amount">The price of the item.</param>
	/// <param name="currency">Abbreviation of the currency used for the transaction. For example “USD” (United States Dollars). See http:en.wikipedia.orgwikiISO_4217 for a standardized list of currency abbreviations.</param>
	/// <param name="receiptPurchaseData">Receipt data (iOS)  receipt ID (android)  for in-app purchases to verify purchases with Apple iTunes / Google Play. Use null in the absence of receipts.</param>
	/// <param name="signature">Android receipt signature. If using native Android use the INAPP_DATA_SIGNATURE string containing the signature of the purchase data that was signed with the private key of the developer. The data signature uses the RSASSA-PKCS1-v1_5 scheme. Pass in null in absence of a signature.</param>
	/// <param name="usingIAPService">Set to true when using UnityIAP.</param>
	public static AnalyticsResult Transaction(string productId, decimal amount, string currency)
	{
		return Transaction(productId, amount, currency, null, null, usingIAPService: false);
	}

	/// <summary>
	///   <para>Tracking Monetization (optional).</para>
	/// </summary>
	/// <param name="productId">The id of the purchased item.</param>
	/// <param name="amount">The price of the item.</param>
	/// <param name="currency">Abbreviation of the currency used for the transaction. For example “USD” (United States Dollars). See http:en.wikipedia.orgwikiISO_4217 for a standardized list of currency abbreviations.</param>
	/// <param name="receiptPurchaseData">Receipt data (iOS)  receipt ID (android)  for in-app purchases to verify purchases with Apple iTunes / Google Play. Use null in the absence of receipts.</param>
	/// <param name="signature">Android receipt signature. If using native Android use the INAPP_DATA_SIGNATURE string containing the signature of the purchase data that was signed with the private key of the developer. The data signature uses the RSASSA-PKCS1-v1_5 scheme. Pass in null in absence of a signature.</param>
	/// <param name="usingIAPService">Set to true when using UnityIAP.</param>
	public static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature)
	{
		return Transaction(productId, amount, currency, receiptPurchaseData, signature, usingIAPService: false);
	}

	/// <summary>
	///   <para>Tracking Monetization (optional).</para>
	/// </summary>
	/// <param name="productId">The id of the purchased item.</param>
	/// <param name="amount">The price of the item.</param>
	/// <param name="currency">Abbreviation of the currency used for the transaction. For example “USD” (United States Dollars). See http:en.wikipedia.orgwikiISO_4217 for a standardized list of currency abbreviations.</param>
	/// <param name="receiptPurchaseData">Receipt data (iOS)  receipt ID (android)  for in-app purchases to verify purchases with Apple iTunes / Google Play. Use null in the absence of receipts.</param>
	/// <param name="signature">Android receipt signature. If using native Android use the INAPP_DATA_SIGNATURE string containing the signature of the purchase data that was signed with the private key of the developer. The data signature uses the RSASSA-PKCS1-v1_5 scheme. Pass in null in absence of a signature.</param>
	/// <param name="usingIAPService">Set to true when using UnityIAP.</param>
	public static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService)
	{
		if (string.IsNullOrEmpty(productId))
		{
			throw new ArgumentException("Cannot set productId to an empty or null string");
		}
		if (string.IsNullOrEmpty(currency))
		{
			throw new ArgumentException("Cannot set currency to an empty or null string");
		}
		UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
		if (unityAnalyticsHandler == null)
		{
			return AnalyticsResult.NotInitialized;
		}
		if (receiptPurchaseData == null)
		{
			receiptPurchaseData = string.Empty;
		}
		if (signature == null)
		{
			signature = string.Empty;
		}
		return unityAnalyticsHandler.Transaction(productId, Convert.ToDouble(amount), currency, receiptPurchaseData, signature, usingIAPService);
	}

	/// <summary>
	///   <para>Custom Events (optional).</para>
	/// </summary>
	/// <param name="customEventName"></param>
	public static AnalyticsResult CustomEvent(string customEventName)
	{
		if (string.IsNullOrEmpty(customEventName))
		{
			throw new ArgumentException("Cannot set custom event name to an empty or null string");
		}
		return GetUnityAnalyticsHandler()?.SendCustomEventName(customEventName) ?? AnalyticsResult.NotInitialized;
	}

	/// <summary>
	///   <para>Custom Events (optional).</para>
	/// </summary>
	/// <param name="customEventName"></param>
	/// <param name="position"></param>
	public static AnalyticsResult CustomEvent(string customEventName, Vector3 position)
	{
		if (string.IsNullOrEmpty(customEventName))
		{
			throw new ArgumentException("Cannot set custom event name to an empty or null string");
		}
		UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
		if (unityAnalyticsHandler == null)
		{
			return AnalyticsResult.NotInitialized;
		}
		CustomEventData customEventData = new CustomEventData(customEventName);
		customEventData.AddDouble("x", (double)Convert.ToDecimal(position.x));
		customEventData.AddDouble("y", (double)Convert.ToDecimal(position.y));
		customEventData.AddDouble("z", (double)Convert.ToDecimal(position.z));
		return unityAnalyticsHandler.SendCustomEvent(customEventData);
	}

	public static AnalyticsResult CustomEvent(string customEventName, IDictionary<string, object> eventData)
	{
		if (string.IsNullOrEmpty(customEventName))
		{
			throw new ArgumentException("Cannot set custom event name to an empty or null string");
		}
		UnityAnalyticsHandler unityAnalyticsHandler = GetUnityAnalyticsHandler();
		if (unityAnalyticsHandler == null)
		{
			return AnalyticsResult.NotInitialized;
		}
		if (eventData == null)
		{
			return unityAnalyticsHandler.SendCustomEventName(customEventName);
		}
		CustomEventData customEventData = new CustomEventData(customEventName);
		customEventData.AddDictionary(eventData);
		return unityAnalyticsHandler.SendCustomEvent(customEventData);
	}

	/// <summary>
	///   <para>This API is used for registering a Runtime Analytics event. It is meant for internal use only and is likely to change in the future. User code should never use this API.</para>
	/// </summary>
	/// <param name="eventName">Name of the event.</param>
	/// <param name="maxEventPerHour">Hourly limit for this event name.</param>
	/// <param name="maxItems">Maximum number of items in this event.</param>
	/// <param name="vendorKey">Vendor key name.</param>
	/// <param name="prefix">Optional event name prefix value.</param>
	/// <param name="ver">Event version number.</param>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static AnalyticsResult RegisterEvent(string eventName, int maxEventPerHour, int maxItems, string vendorKey = "", string prefix = "")
	{
		string empty = string.Empty;
		empty = Assembly.GetCallingAssembly().FullName;
		return RegisterEvent(eventName, maxEventPerHour, maxItems, vendorKey, 1, prefix, empty);
	}

	/// <summary>
	///   <para>This API is used for registering a Runtime Analytics event. It is meant for internal use only and is likely to change in the future. User code should never use this API.</para>
	/// </summary>
	/// <param name="eventName">Name of the event.</param>
	/// <param name="maxEventPerHour">Hourly limit for this event name.</param>
	/// <param name="maxItems">Maximum number of items in this event.</param>
	/// <param name="vendorKey">Vendor key name.</param>
	/// <param name="prefix">Optional event name prefix value.</param>
	/// <param name="ver">Event version number.</param>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static AnalyticsResult RegisterEvent(string eventName, int maxEventPerHour, int maxItems, string vendorKey, int ver, string prefix = "")
	{
		string empty = string.Empty;
		empty = Assembly.GetCallingAssembly().FullName;
		return RegisterEvent(eventName, maxEventPerHour, maxItems, vendorKey, ver, prefix, empty);
	}

	private static AnalyticsResult RegisterEvent(string eventName, int maxEventPerHour, int maxItems, string vendorKey, int ver, string prefix, string assemblyInfo)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			throw new ArgumentException("Cannot set event name to an empty or null string");
		}
		return GetUnityAnalyticsHandler()?.RegisterEvent(eventName, maxEventPerHour, maxItems, vendorKey, ver, prefix, assemblyInfo) ?? AnalyticsResult.NotInitialized;
	}

	/// <summary>
	///   <para>This API is used to send a Runtime Analytics event. It is meant for internal use only and is likely to change in the future. User code should never use this API.</para>
	/// </summary>
	/// <param name="eventName">Name of the event.</param>
	/// <param name="ver">Event version number.</param>
	/// <param name="prefix">Optional event name prefix value.</param>
	/// <param name="parameters">Additional event data.</param>
	public static AnalyticsResult SendEvent(string eventName, object parameters, int ver = 1, string prefix = "")
	{
		if (string.IsNullOrEmpty(eventName))
		{
			throw new ArgumentException("Cannot set event name to an empty or null string");
		}
		if (parameters == null)
		{
			throw new ArgumentException("Cannot set parameters to null");
		}
		return GetUnityAnalyticsHandler()?.SendEvent(eventName, parameters, ver, prefix) ?? AnalyticsResult.NotInitialized;
	}
}
