using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>Provides methods, events, and properties that provides information about planes detected in the environment. </para>
/// </summary>
[NativeHeader("Modules/XR/XRPrefix.h")]
[NativeConditional("ENABLE_XR")]
[UsedByNativeCode]
[NativeHeader("Modules/XR/Subsystems/Planes/XRPlaneSubsystem.h")]
public class XRPlaneSubsystem : Subsystem<XRPlaneSubsystemDescriptor>
{
	/// <summary>
	///   <para>The frame during which the planes were last updated.</para>
	/// </summary>
	public extern int LastUpdatedFrame
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public event Action<PlaneAddedEventArgs> PlaneAdded;

	public event Action<PlaneUpdatedEventArgs> PlaneUpdated;

	public event Action<PlaneRemovedEventArgs> PlaneRemoved;

	public bool TryGetPlane(TrackableId planeId, out BoundedPlane plane)
	{
		return TryGetPlane_Injected(ref planeId, out plane);
	}

	public void GetAllPlanes(List<BoundedPlane> planesOut)
	{
		if (planesOut == null)
		{
			throw new ArgumentNullException("planesOut");
		}
		GetAllPlanesAsList(planesOut);
	}

	public bool TryGetPlaneBoundary(TrackableId planeId, List<Vector3> boundaryOut)
	{
		if (boundaryOut == null)
		{
			throw new ArgumentNullException("boundaryOut");
		}
		return Internal_GetBoundaryAsList(planeId, boundaryOut);
	}

	[RequiredByNativeCode]
	private void InvokePlaneAddedEvent(BoundedPlane plane)
	{
		if (this.PlaneAdded != null)
		{
			this.PlaneAdded(new PlaneAddedEventArgs
			{
				PlaneSubsystem = this,
				Plane = plane
			});
		}
	}

	[RequiredByNativeCode]
	private void InvokePlaneUpdatedEvent(BoundedPlane plane)
	{
		if (this.PlaneUpdated != null)
		{
			this.PlaneUpdated(new PlaneUpdatedEventArgs
			{
				PlaneSubsystem = this,
				Plane = plane
			});
		}
	}

	[RequiredByNativeCode]
	private void InvokePlaneRemovedEvent(BoundedPlane removedPlane)
	{
		if (this.PlaneRemoved != null)
		{
			this.PlaneRemoved(new PlaneRemovedEventArgs
			{
				PlaneSubsystem = this,
				Plane = removedPlane
			});
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern BoundedPlane[] GetAllPlanesAsFixedArray();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetAllPlanesAsList(List<BoundedPlane> planes);

	private bool Internal_GetBoundaryAsList(TrackableId planeId, List<Vector3> boundaryOut)
	{
		return Internal_GetBoundaryAsList_Injected(ref planeId, boundaryOut);
	}

	private Vector3[] Internal_GetBoundaryAsFixedArray(TrackableId planeId)
	{
		return Internal_GetBoundaryAsFixedArray_Injected(ref planeId);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool TryGetPlane_Injected(ref TrackableId planeId, out BoundedPlane plane);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool Internal_GetBoundaryAsList_Injected(ref TrackableId planeId, List<Vector3> boundaryOut);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Vector3[] Internal_GetBoundaryAsFixedArray_Injected(ref TrackableId planeId);
}
