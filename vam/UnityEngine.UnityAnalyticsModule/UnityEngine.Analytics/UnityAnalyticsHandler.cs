using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Analytics;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/UnityAnalytics/Events/UserCustomEvent.h")]
[NativeHeader("Modules/UnityConnect/UnityConnectClient.h")]
[NativeHeader("Modules/UnityAnalytics/UnityAnalytics.h")]
internal class UnityAnalyticsHandler : IDisposable
{
	[NonSerialized]
	internal IntPtr m_Ptr;

	[StaticAccessor("GetUnityConnectClient()", StaticAccessorType.Dot)]
	public static extern bool limitUserTracking
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetUnityConnectClient()", StaticAccessorType.Dot)]
	public static extern bool deviceStatsEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public UnityAnalyticsHandler()
	{
		m_Ptr = Internal_Create(this);
	}

	~UnityAnalyticsHandler()
	{
		Destroy();
	}

	private void Destroy()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
	}

	public void Dispose()
	{
		Destroy();
		GC.SuppressFinalize(this);
	}

	public bool IsInitialized()
	{
		return m_Ptr != IntPtr.Zero;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern IntPtr Internal_Create(UnityAnalyticsHandler u);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	internal static extern void Internal_Destroy(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern bool FlushEvents();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern AnalyticsResult SetUserId(string userId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern AnalyticsResult SetUserGender(Gender gender);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern AnalyticsResult SetUserBirthYear(int birthYear);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern AnalyticsResult Transaction(string productId, double amount, string currency, string receiptPurchaseData, string signature, bool usingIAPService);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern AnalyticsResult SendCustomEventName(string customEventName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern AnalyticsResult SendCustomEvent(CustomEventData eventData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern AnalyticsResult RegisterEvent(string eventName, int maxEventPerHour, int maxItems, string vendorKey, int ver, string prefix, string assemblyInfo);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern AnalyticsResult SendEvent(string eventName, object parameters, int ver, string prefix);
}
