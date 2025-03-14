using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Defines state and drawing commands used in a custom render pipelines.</para>
/// </summary>
[NativeType("Runtime/Graphics/ScriptableRenderLoop/ScriptableRenderContext.h")]
public struct ScriptableRenderContext
{
	private IntPtr m_Ptr;

	internal ScriptableRenderContext(IntPtr ptr)
	{
		m_Ptr = ptr;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Submit_Internal();

	private void DrawRenderers_Internal(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings)
	{
		INTERNAL_CALL_DrawRenderers_Internal(ref this, ref renderers, ref drawSettings, ref filterSettings);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DrawRenderers_Internal(ref ScriptableRenderContext self, ref FilterResults renderers, ref DrawRendererSettings drawSettings, ref FilterRenderersSettings filterSettings);

	private void DrawRenderers_StateBlock_Internal(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings, RenderStateBlock stateBlock)
	{
		INTERNAL_CALL_DrawRenderers_StateBlock_Internal(ref this, ref renderers, ref drawSettings, ref filterSettings, ref stateBlock);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DrawRenderers_StateBlock_Internal(ref ScriptableRenderContext self, ref FilterResults renderers, ref DrawRendererSettings drawSettings, ref FilterRenderersSettings filterSettings, ref RenderStateBlock stateBlock);

	private void DrawRenderers_StateMap_Internal(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings, Array stateMap, int stateMapLength)
	{
		INTERNAL_CALL_DrawRenderers_StateMap_Internal(ref this, ref renderers, ref drawSettings, ref filterSettings, stateMap, stateMapLength);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DrawRenderers_StateMap_Internal(ref ScriptableRenderContext self, ref FilterResults renderers, ref DrawRendererSettings drawSettings, ref FilterRenderersSettings filterSettings, Array stateMap, int stateMapLength);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void DrawShadows_Internal(ref DrawShadowsSettings settings);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void ExecuteCommandBuffer_Internal(CommandBuffer commandBuffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void ExecuteCommandBufferAsync_Internal(CommandBuffer commandBuffer, ComputeQueueType queueType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetupCameraProperties_Internal(Camera camera, bool stereoSetup);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void StereoEndRender_Internal(Camera camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void StartMultiEye_Internal(Camera camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void StopMultiEye_Internal(Camera camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void DrawSkybox_Internal(Camera camera);

	internal IntPtr Internal_GetPtr()
	{
		return m_Ptr;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("BeginRenderPass")]
	public static extern void BeginRenderPassInternal(IntPtr _self, int w, int h, int samples, RenderPassAttachment[] colors, RenderPassAttachment depth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("BeginSubPass")]
	public static extern void BeginSubPassInternal(IntPtr _self, RenderPassAttachment[] colors, RenderPassAttachment[] inputs, bool readOnlyDepth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("EndRenderPass")]
	public static extern void EndRenderPassInternal(IntPtr _self);

	/// <summary>
	///   <para>Submit rendering loop for execution.</para>
	/// </summary>
	public void Submit()
	{
		CheckValid();
		Submit_Internal();
	}

	public void DrawRenderers(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings)
	{
		CheckValid();
		DrawRenderers_Internal(renderers, ref drawSettings, filterSettings);
	}

	public void DrawRenderers(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings, RenderStateBlock stateBlock)
	{
		CheckValid();
		DrawRenderers_StateBlock_Internal(renderers, ref drawSettings, filterSettings, stateBlock);
	}

	public void DrawRenderers(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings, List<RenderStateMapping> stateMap)
	{
		CheckValid();
		DrawRenderers_StateMap_Internal(renderers, ref drawSettings, filterSettings, NoAllocHelpers.ExtractArrayFromList(stateMap), stateMap.Count);
	}

	public void DrawShadows(ref DrawShadowsSettings settings)
	{
		CheckValid();
		DrawShadows_Internal(ref settings);
	}

	/// <summary>
	///   <para>Execute a custom graphics command buffer.</para>
	/// </summary>
	/// <param name="commandBuffer">Command buffer to execute.</param>
	public void ExecuteCommandBuffer(CommandBuffer commandBuffer)
	{
		if (commandBuffer == null)
		{
			throw new ArgumentNullException("commandBuffer");
		}
		CheckValid();
		ExecuteCommandBuffer_Internal(commandBuffer);
	}

	/// <summary>
	///   <para>Executes a command buffer on an async compute queue with the queue selected based on the ComputeQueueType parameter passed.
	///
	/// It is required that all of the commands within the command buffer be of a type suitable for execution on the async compute queues. If the buffer contains any commands that are not appropriate then an error will be logged and displayed in the editor window.  Specifically the following commands are permitted in a CommandBuffer intended for async execution:
	///
	/// CommandBuffer.BeginSample
	///
	/// CommandBuffer.CopyCounterValue
	///
	/// CommandBuffer.CopyTexture
	///
	/// CommandBuffer.CreateGPUFence
	///
	/// CommandBuffer.DispatchCompute
	///
	/// CommandBuffer.EndSample
	///
	/// CommandBuffer.IssuePluginEvent
	///
	/// CommandBuffer.SetComputeBufferParam
	///
	/// CommandBuffer.SetComputeFloatParam
	///
	/// CommandBuffer.SetComputeFloatParams
	///
	/// CommandBuffer.SetComputeTextureParam
	///
	/// CommandBuffer.SetComputeVectorParam
	///
	/// CommandBuffer.WaitOnGPUFence
	///
	/// All of the commands within the buffer are guaranteed to be executed on the same queue. If the target platform does not support async compute queues then the work is dispatched on the graphics queue.</para>
	/// </summary>
	/// <param name="commandBuffer">The CommandBuffer to be executed.</param>
	/// <param name="queueType">Describes the desired async compute queue the supplied CommandBuffer should be executed on.</param>
	public void ExecuteCommandBufferAsync(CommandBuffer commandBuffer, ComputeQueueType queueType)
	{
		if (commandBuffer == null)
		{
			throw new ArgumentNullException("commandBuffer");
		}
		CheckValid();
		ExecuteCommandBufferAsync_Internal(commandBuffer, queueType);
	}

	/// <summary>
	///   <para>Setup camera specific global shader variables.</para>
	/// </summary>
	/// <param name="camera">Camera to setup shader variables for.</param>
	/// <param name="stereoSetup">Set up the stereo shader variables and state.</param>
	public void SetupCameraProperties(Camera camera)
	{
		CheckValid();
		SetupCameraProperties_Internal(camera, stereoSetup: false);
	}

	/// <summary>
	///   <para>Setup camera specific global shader variables.</para>
	/// </summary>
	/// <param name="camera">Camera to setup shader variables for.</param>
	/// <param name="stereoSetup">Set up the stereo shader variables and state.</param>
	public void SetupCameraProperties(Camera camera, bool stereoSetup)
	{
		CheckValid();
		SetupCameraProperties_Internal(camera, stereoSetup);
	}

	/// <summary>
	///   <para>Indicate completion of stereo rendering on a single frame.</para>
	/// </summary>
	/// <param name="camera">Camera to indicate completion of stereo rendering.</param>
	public void StereoEndRender(Camera camera)
	{
		CheckValid();
		StereoEndRender_Internal(camera);
	}

	/// <summary>
	///   <para>Fine-grain control to begin stereo rendering on the scriptable render context.</para>
	/// </summary>
	/// <param name="camera">Camera to enable stereo rendering on.</param>
	public void StartMultiEye(Camera camera)
	{
		CheckValid();
		StartMultiEye_Internal(camera);
	}

	/// <summary>
	///   <para>Stop stereo rendering on the scriptable render context.</para>
	/// </summary>
	/// <param name="camera">Camera to disable stereo rendering on.</param>
	public void StopMultiEye(Camera camera)
	{
		CheckValid();
		StopMultiEye_Internal(camera);
	}

	/// <summary>
	///   <para>Draw skybox.</para>
	/// </summary>
	/// <param name="camera">Camera to draw the skybox for.</param>
	public void DrawSkybox(Camera camera)
	{
		CheckValid();
		DrawSkybox_Internal(camera);
	}

	internal void CheckValid()
	{
		if (m_Ptr.ToInt64() == 0)
		{
			throw new ArgumentException("Invalid ScriptableRenderContext.  This can be caused by allocating a context in user code.");
		}
	}
}
