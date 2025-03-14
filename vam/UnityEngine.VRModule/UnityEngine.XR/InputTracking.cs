using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.XR;

/// <summary>
///   <para>A collection of methods and properties for interacting with the XR tracking system.</para>
/// </summary>
[RequiredByNativeCode]
public static class InputTracking
{
	private enum TrackingStateEventType
	{
		NodeAdded,
		NodeRemoved,
		TrackingAcquired,
		TrackingLost
	}

	/// <summary>
	///   <para>Disables positional tracking in XR. This takes effect the next time the head pose is sampled.  If set to true the camera only tracks headset rotation state.</para>
	/// </summary>
	public static extern bool disablePositionalTracking
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public static event Action<XRNodeState> trackingAcquired;

	public static event Action<XRNodeState> trackingLost;

	public static event Action<XRNodeState> nodeAdded;

	public static event Action<XRNodeState> nodeRemoved;

	/// <summary>
	///   <para>Gets the position of a specific node.</para>
	/// </summary>
	/// <param name="node">Specifies which node's position should be returned.</param>
	/// <returns>
	///   <para>The position of the node in its local tracking space.</para>
	/// </returns>
	public static Vector3 GetLocalPosition(XRNode node)
	{
		INTERNAL_CALL_GetLocalPosition(node, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetLocalPosition(XRNode node, out Vector3 value);

	/// <summary>
	///   <para>Gets the rotation of a specific node.</para>
	/// </summary>
	/// <param name="node">Specifies which node's rotation should be returned.</param>
	/// <returns>
	///   <para>The rotation of the node in its local tracking space.</para>
	/// </returns>
	public static Quaternion GetLocalRotation(XRNode node)
	{
		INTERNAL_CALL_GetLocalRotation(node, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetLocalRotation(XRNode node, out Quaternion value);

	/// <summary>
	///   <para>Center tracking to the current position and orientation of the HMD.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void Recenter();

	/// <summary>
	///   <para>Accepts the unique identifier for a tracked node and returns a friendly name for it.</para>
	/// </summary>
	/// <param name="uniqueID">The unique identifier for the Node index.</param>
	/// <returns>
	///   <para>The name of the tracked node if the given 64-bit identifier maps to a currently tracked node. Empty string otherwise.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern string GetNodeName(ulong uniqueID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void GetNodeStatesInternal(object nodeStates);

	public static void GetNodeStates(List<XRNodeState> nodeStates)
	{
		if (nodeStates == null)
		{
			throw new ArgumentNullException("nodeStates");
		}
		nodeStates.Clear();
		GetNodeStatesInternal(nodeStates);
	}

	[RequiredByNativeCode]
	private static void InvokeTrackingEvent(TrackingStateEventType eventType, XRNode nodeType, long uniqueID, bool tracked)
	{
		Action<XRNodeState> action = null;
		XRNodeState obj = default(XRNodeState);
		obj.uniqueID = (ulong)uniqueID;
		obj.nodeType = nodeType;
		obj.tracked = tracked;
		((Action<XRNodeState>)(eventType switch
		{
			TrackingStateEventType.TrackingAcquired => InputTracking.trackingAcquired, 
			TrackingStateEventType.TrackingLost => InputTracking.trackingLost, 
			TrackingStateEventType.NodeAdded => InputTracking.nodeAdded, 
			TrackingStateEventType.NodeRemoved => InputTracking.nodeRemoved, 
			_ => throw new ArgumentException("TrackingEventHandler - Invalid EventType: " + eventType), 
		}))?.Invoke(obj);
	}

	static InputTracking()
	{
		InputTracking.trackingAcquired = null;
		InputTracking.trackingLost = null;
		InputTracking.nodeAdded = null;
		InputTracking.nodeRemoved = null;
	}
}
