using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngineInternal.Input;

[NativeConditional("ENABLE_NEW_INPUT_SYSTEM")]
[NativeHeader("Modules/Input/Private/InputInternal.h")]
[NativeHeader("Modules/Input/Private/InputModuleBindings.h")]
public class NativeInputSystem
{
	public static NativeUpdateCallback onUpdate;

	public static NativeEventCallback onEvents;

	private static NativeDeviceDiscoveredCallback s_OnDeviceDiscoveredCallback;

	public static extern double zeroEventTime
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern bool hasDeviceDiscoveredCallback
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static event NativeDeviceDiscoveredCallback onDeviceDiscovered
	{
		add
		{
			s_OnDeviceDiscoveredCallback = (NativeDeviceDiscoveredCallback)Delegate.Combine(s_OnDeviceDiscoveredCallback, value);
			hasDeviceDiscoveredCallback = s_OnDeviceDiscoveredCallback != null;
		}
		remove
		{
			s_OnDeviceDiscoveredCallback = (NativeDeviceDiscoveredCallback)Delegate.Remove(s_OnDeviceDiscoveredCallback, value);
			hasDeviceDiscoveredCallback = s_OnDeviceDiscoveredCallback != null;
		}
	}

	static NativeInputSystem()
	{
		hasDeviceDiscoveredCallback = false;
	}

	[RequiredByNativeCode]
	internal static void NotifyUpdate(NativeInputUpdateType updateType)
	{
		onUpdate?.Invoke(updateType);
	}

	[RequiredByNativeCode]
	internal static void NotifyEvents(int eventCount, IntPtr eventData)
	{
		onEvents?.Invoke(eventCount, eventData);
	}

	[RequiredByNativeCode]
	internal static void NotifyDeviceDiscovered(NativeInputDeviceInfo deviceInfo)
	{
		s_OnDeviceDiscoveredCallback?.Invoke(deviceInfo);
	}

	public unsafe static void SendInput<TInputEvent>(TInputEvent inputEvent) where TInputEvent : struct
	{
		SendInput((IntPtr)UnsafeUtility.AddressOf(ref inputEvent));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void SendInput(IntPtr inputEvent);

	public unsafe static bool SendOutput<TOutputEvent>(int deviceId, int type, TOutputEvent outputEvent) where TOutputEvent : struct
	{
		return SendOutput(deviceId, type, UnsafeUtility.SizeOf<TOutputEvent>(), (IntPtr)UnsafeUtility.AddressOf(ref outputEvent));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool SendOutput(int deviceId, int type, int sizeInBytes, IntPtr data);

	public static string GetDeviceConfiguration(int deviceId)
	{
		return null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern string GetControlConfiguration(int deviceId, int controlIndex);

	public static void SetPollingFrequency(float hertz)
	{
		if (hertz < 1f)
		{
			throw new ArgumentException("Polling frequency cannot be less than 1Hz");
		}
		SetPollingFrequencyInternal(hertz);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPollingFrequencyInternal(float hertz);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendEvents();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Update(NativeInputUpdateType updateType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int ReportNewInputDevice(string descriptor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ReportInputDeviceDisconnect(int nativeDeviceId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ReportInputDeviceReconnect(int nativeDeviceId);
}
