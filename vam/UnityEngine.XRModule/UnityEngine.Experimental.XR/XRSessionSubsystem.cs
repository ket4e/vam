using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>A collection of methods and properties used to interact with and configure an XR session.</para>
/// </summary>
[NativeHeader("Modules/XR/Subsystems/Session/XRSessionSubsystem.h")]
[UsedByNativeCode]
[NativeConditional("ENABLE_XR")]
[NativeHeader("Modules/XR/XRPrefix.h")]
public class XRSessionSubsystem : Subsystem<XRSessionSubsystemDescriptor>
{
	/// <summary>
	///   <para>Get current tracking status of the device.</para>
	/// </summary>
	[NativeConditional("ENABLE_XR", StubReturnStatement = "kUnityXRTrackingStateUnknown")]
	public extern TrackingState TrackingState
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>The frame during which the tracking state was last updated.</para>
	/// </summary>
	public extern int LastUpdatedFrame
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public event Action<SessionTrackingStateChangedEventArgs> TrackingStateChanged;

	[RequiredByNativeCode]
	private void InvokeTrackingStateChangedEvent(TrackingState newState)
	{
		if (this.TrackingStateChanged != null)
		{
			this.TrackingStateChanged(new SessionTrackingStateChangedEventArgs
			{
				m_Session = this,
				NewState = newState
			});
		}
	}
}
