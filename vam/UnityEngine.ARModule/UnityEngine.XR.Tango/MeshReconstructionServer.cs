using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Tango;

[NativeConditional("PLATFORM_ANDROID")]
[NativeHeader("PhysicsScriptingClasses.h")]
[NativeHeader("Runtime/AR/Tango/TangoScriptApi.h")]
[UsedByNativeCode]
internal class MeshReconstructionServer
{
	internal delegate void SegmentChangedDelegate(GridIndex gridIndex, SegmentChange changeType, double updateTime);

	internal delegate void SegmentReadyDelegate(SegmentGenerationResult generatedSegmentData);

	internal enum Status
	{
		UnsupportedPlatform,
		Ok,
		MissingMeshReconstructionLibrary,
		FailedToCreateMeshReconstructionContext,
		FailedToSetDepthCalibration
	}

	internal IntPtr m_ServerPtr = IntPtr.Zero;

	private Status m_Status = Status.UnsupportedPlatform;

	internal Status status => m_Status;

	internal int generationRequests => Internal_GetNumGenerationRequests(m_ServerPtr);

	internal bool enabled
	{
		get
		{
			return Internal_GetEnabled(m_ServerPtr);
		}
		set
		{
			Internal_SetEnabled(m_ServerPtr, value);
		}
	}

	internal MeshReconstructionServer(MeshReconstructionConfig config)
	{
		int num = 0;
		m_ServerPtr = Internal_Create(this, config, out num);
		m_Status = (Status)num;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_ClearMeshes(IntPtr server);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_GetEnabled(IntPtr server);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetEnabled(IntPtr server, bool enabled);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_GetNativeReconstructionContextPtr(IntPtr server);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int Internal_GetNumGenerationRequests(IntPtr server);

	internal void Dispose()
	{
		if (m_ServerPtr != IntPtr.Zero)
		{
			Destroy(m_ServerPtr);
			m_ServerPtr = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	private static IntPtr Internal_Create(MeshReconstructionServer self, MeshReconstructionConfig config, out int status)
	{
		return Internal_Create_Injected(self, ref config, out status);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void Destroy(IntPtr server);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	internal static extern void DestroyThreaded(IntPtr server);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetChangedSegments(IntPtr serverPtr, SegmentChangedDelegate onSegmentChanged);

	private static void Internal_GenerateSegmentAsync(IntPtr serverPtr, GridIndex gridIndex, MeshFilter destinationMeshFilter, MeshCollider destinationMeshCollider, SegmentReadyDelegate onSegmentReady, bool provideNormals, bool provideColors, bool providePhysics)
	{
		Internal_GenerateSegmentAsync_Injected(serverPtr, ref gridIndex, destinationMeshFilter, destinationMeshCollider, onSegmentReady, provideNormals, provideColors, providePhysics);
	}

	~MeshReconstructionServer()
	{
		if (m_ServerPtr != IntPtr.Zero)
		{
			DestroyThreaded(m_ServerPtr);
			m_ServerPtr = IntPtr.Zero;
			GC.SuppressFinalize(this);
		}
	}

	internal void ClearMeshes()
	{
		Internal_ClearMeshes(m_ServerPtr);
	}

	internal IntPtr GetNativeReconstructionContext()
	{
		return Internal_GetNativeReconstructionContextPtr(m_ServerPtr);
	}

	internal void GetChangedSegments(SegmentChangedDelegate onSegmentChanged)
	{
		if (onSegmentChanged == null)
		{
			throw new ArgumentNullException("onSegmentChanged");
		}
		Internal_GetChangedSegments(m_ServerPtr, onSegmentChanged);
	}

	internal void GenerateSegmentAsync(SegmentGenerationRequest request, SegmentReadyDelegate onSegmentReady)
	{
		if (onSegmentReady == null)
		{
			throw new ArgumentNullException("onSegmentRead");
		}
		Internal_GenerateSegmentAsync(m_ServerPtr, request.gridIndex, request.destinationMeshFilter, request.destinationMeshCollider, onSegmentReady, request.provideNormals, request.provideColors, request.providePhysics);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_Create_Injected(MeshReconstructionServer self, ref MeshReconstructionConfig config, out int status);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GenerateSegmentAsync_Injected(IntPtr serverPtr, ref GridIndex gridIndex, MeshFilter destinationMeshFilter, MeshCollider destinationMeshCollider, SegmentReadyDelegate onSegmentReady, bool provideNormals, bool provideColors, bool providePhysics);
}
