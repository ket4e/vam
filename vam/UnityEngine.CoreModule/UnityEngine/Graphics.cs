using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Raw interface to Unity's drawing functions.</para>
/// </summary>
[NativeHeader("Runtime/Camera/LightProbeProxyVolume.h")]
[NativeHeader("Runtime/Graphics/CopyTexture.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Shaders/ComputeShader.h")]
public sealed class Graphics
{
	internal static readonly int kMaxDrawMeshInstanceCount = Internal_GetMaxDrawMeshInstanceCount();

	/// <summary>
	///   <para>Currently active color buffer (Read Only).</para>
	/// </summary>
	public static RenderBuffer activeColorBuffer
	{
		get
		{
			GetActiveColorBuffer(out var res);
			return res;
		}
	}

	/// <summary>
	///   <para>Currently active depth/stencil buffer (Read Only).</para>
	/// </summary>
	public static RenderBuffer activeDepthBuffer
	{
		get
		{
			GetActiveDepthBuffer(out var res);
			return res;
		}
	}

	/// <summary>
	///   <para>Graphics Tier classification for current device.
	/// Changing this value affects any subsequently loaded shaders. Initially this value is auto-detected from the hardware in use.</para>
	/// </summary>
	public static extern GraphicsTier activeTier
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Returns the currently active color gamut.</para>
	/// </summary>
	public static extern ColorGamut activeColorGamut
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	private static void Internal_DrawMeshNow1(Mesh mesh, int subsetIndex, Vector3 position, Quaternion rotation)
	{
		INTERNAL_CALL_Internal_DrawMeshNow1(mesh, subsetIndex, ref position, ref rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_DrawMeshNow1(Mesh mesh, int subsetIndex, ref Vector3 position, ref Quaternion rotation);

	private static void Internal_DrawMeshNow2(Mesh mesh, int subsetIndex, Matrix4x4 matrix)
	{
		INTERNAL_CALL_Internal_DrawMeshNow2(mesh, subsetIndex, ref matrix);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_DrawMeshNow2(Mesh mesh, int subsetIndex, ref Matrix4x4 matrix);

	/// <summary>
	///   <para>Draws a fully procedural geometry on the GPU.</para>
	/// </summary>
	/// <param name="topology"></param>
	/// <param name="vertexCount"></param>
	/// <param name="instanceCount"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void DrawProcedural(MeshTopology topology, int vertexCount, [DefaultValue("1")] int instanceCount);

	[ExcludeFromDocs]
	public static void DrawProcedural(MeshTopology topology, int vertexCount)
	{
		int instanceCount = 1;
		DrawProcedural(topology, vertexCount, instanceCount);
	}

	/// <summary>
	///   <para>Draws a fully procedural geometry on the GPU.</para>
	/// </summary>
	/// <param name="topology">Topology of the procedural geometry.</param>
	/// <param name="bufferWithArgs">Buffer with draw arguments.</param>
	/// <param name="argsOffset">Byte offset where in the buffer the draw arguments are.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset);

	[ExcludeFromDocs]
	public static void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs)
	{
		int argsOffset = 0;
		DrawProceduralIndirect(topology, bufferWithArgs, argsOffset);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int Internal_GetMaxDrawMeshInstanceCount();

	/// <summary>
	///   <para>Draw a texture in screen coordinates.</para>
	/// </summary>
	/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
	/// <param name="texture">Texture to draw.</param>
	/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
	/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
	/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
	/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
	/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
	/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
	/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, Material mat)
	{
		int pass = -1;
		DrawTexture(screenRect, texture, mat, pass);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture)
	{
		int pass = -1;
		Material mat = null;
		DrawTexture(screenRect, texture, mat, pass);
	}

	/// <summary>
	///   <para>Draw a texture in screen coordinates.</para>
	/// </summary>
	/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
	/// <param name="texture">Texture to draw.</param>
	/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
	/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
	/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
	/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
	/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
	/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
	/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	public static void DrawTexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
	{
		DrawTexture(screenRect, texture, 0, 0, 0, 0, mat, pass);
	}

	/// <summary>
	///   <para>Draw a texture in screen coordinates.</para>
	/// </summary>
	/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
	/// <param name="texture">Texture to draw.</param>
	/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
	/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
	/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
	/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
	/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
	/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
	/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
	{
		int pass = -1;
		DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
	{
		int pass = -1;
		Material mat = null;
		DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
	}

	/// <summary>
	///   <para>Draw a texture in screen coordinates.</para>
	/// </summary>
	/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
	/// <param name="texture">Texture to draw.</param>
	/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
	/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
	/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
	/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
	/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
	/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
	/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
	{
		DrawTexture(screenRect, texture, new Rect(0f, 0f, 1f, 1f), leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
	}

	/// <summary>
	///   <para>Draw a texture in screen coordinates.</para>
	/// </summary>
	/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
	/// <param name="texture">Texture to draw.</param>
	/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
	/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
	/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
	/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
	/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
	/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
	/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
	{
		int pass = -1;
		DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
	{
		int pass = -1;
		Material mat = null;
		DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
	}

	/// <summary>
	///   <para>Draw a texture in screen coordinates.</para>
	/// </summary>
	/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
	/// <param name="texture">Texture to draw.</param>
	/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
	/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
	/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
	/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
	/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
	/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
	/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
	{
		Color32 color = new Color32(128, 128, 128, 128);
		DrawTextureImpl(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
	}

	/// <summary>
	///   <para>Draw a texture in screen coordinates.</para>
	/// </summary>
	/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
	/// <param name="texture">Texture to draw.</param>
	/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
	/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
	/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
	/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
	/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
	/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
	/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat)
	{
		int pass = -1;
		DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
	}

	[ExcludeFromDocs]
	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color)
	{
		int pass = -1;
		Material mat = null;
		DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
	}

	/// <summary>
	///   <para>Draw a texture in screen coordinates.</para>
	/// </summary>
	/// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
	/// <param name="texture">Texture to draw.</param>
	/// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
	/// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
	/// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
	/// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
	/// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
	/// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
	/// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
	{
		DrawTextureImpl(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
	[GeneratedByOldBindingsGenerator]
	internal static extern void Internal_DrawTexture(ref Internal_DrawTextureArguments args);

	[ExcludeFromDocs]
	public static GPUFence CreateGPUFence()
	{
		SynchronisationStage stage = SynchronisationStage.PixelProcessing;
		return CreateGPUFence(stage);
	}

	/// <summary>
	///   <para>Creates a GPUFence which will be passed after the last Blit, Clear, Draw, Dispatch or Texture Copy command prior to this call has been completed on the GPU.</para>
	/// </summary>
	/// <param name="stage">On some platforms there is a significant gap between the vertex processing completing and the pixel processing begining for a given draw call. This parameter allows for the fence to be passed after either the vertex or pixel processing for the proceeding draw has completed. If a compute shader dispatch was the last task submitted then this parameter is ignored.</param>
	/// <returns>
	///   <para>Returns a new GPUFence.</para>
	/// </returns>
	public static GPUFence CreateGPUFence([DefaultValue("SynchronisationStage.PixelProcessing")] SynchronisationStage stage)
	{
		GPUFence result = default(GPUFence);
		result.m_Ptr = Internal_CreateGPUFence(stage);
		result.InitPostAllocation();
		result.Validate();
		return result;
	}

	private static IntPtr Internal_CreateGPUFence(SynchronisationStage stage)
	{
		INTERNAL_CALL_Internal_CreateGPUFence(stage, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_CreateGPUFence(SynchronisationStage stage, out IntPtr value);

	[ExcludeFromDocs]
	public static void WaitOnGPUFence(GPUFence fence)
	{
		SynchronisationStage stage = SynchronisationStage.VertexProcessing;
		WaitOnGPUFence(fence, stage);
	}

	/// <summary>
	///   <para>Instructs the GPU's processing of the graphics queue to wait until the given GPUFence is passed.</para>
	/// </summary>
	/// <param name="fence">The GPUFence that the GPU will be instructed to wait upon before proceeding with its processing of the graphics queue.</param>
	/// <param name="stage">On some platforms there is a significant gap between the vertex processing completing and the pixel processing begining for a given draw call. This parameter allows for requested wait to be before the next items vertex or pixel processing begins. If a compute shader dispatch is the next item to be submitted then this parameter is ignored.</param>
	public static void WaitOnGPUFence(GPUFence fence, [DefaultValue("SynchronisationStage.VertexProcessing")] SynchronisationStage stage)
	{
		fence.Validate();
		if (fence.IsFencePending())
		{
			WaitOnGPUFence_Internal(fence.m_Ptr, stage);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void WaitOnGPUFence_Internal(IntPtr fencePtr, SynchronisationStage stage);

	/// <summary>
	///   <para>Execute a command buffer.</para>
	/// </summary>
	/// <param name="buffer">The buffer to execute.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void ExecuteCommandBuffer(CommandBuffer buffer);

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
	/// <param name="buffer">The CommandBuffer to be executed.</param>
	/// <param name="queueType">Describes the desired async compute queue the suuplied CommandBuffer should be executed on.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void ExecuteCommandBufferAsync(CommandBuffer buffer, ComputeQueueType queueType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetNullRT();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetRTSimple(out RenderBuffer color, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetMRTFullSetup(RenderBuffer[] colorSA, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice, RenderBufferLoadAction[] colorLoadSA, RenderBufferStoreAction[] colorStoreSA, RenderBufferLoadAction depthLoad, RenderBufferStoreAction depthStore);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetMRTSimple(RenderBuffer[] colorSA, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void GetActiveColorBuffer(out RenderBuffer res);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void GetActiveDepthBuffer(out RenderBuffer res);

	/// <summary>
	///   <para>Set random write target for level pixel shaders.</para>
	/// </summary>
	/// <param name="index">Index of the random write target in the shader.</param>
	/// <param name="uav">RenderTexture to set as write target.</param>
	/// <param name="preserveCounterValue">Whether to leave the append/consume counter value unchanged.</param>
	public static void SetRandomWriteTarget(int index, RenderTexture uav)
	{
		Internal_SetRandomWriteTargetRT(index, uav);
	}

	[ExcludeFromDocs]
	public static void SetRandomWriteTarget(int index, ComputeBuffer uav)
	{
		bool preserveCounterValue = false;
		SetRandomWriteTarget(index, uav, preserveCounterValue);
	}

	/// <summary>
	///   <para>Set random write target for level pixel shaders.</para>
	/// </summary>
	/// <param name="index">Index of the random write target in the shader.</param>
	/// <param name="uav">RenderTexture to set as write target.</param>
	/// <param name="preserveCounterValue">Whether to leave the append/consume counter value unchanged.</param>
	public static void SetRandomWriteTarget(int index, ComputeBuffer uav, [DefaultValue("false")] bool preserveCounterValue)
	{
		if (uav == null)
		{
			throw new ArgumentNullException("uav");
		}
		if (uav.m_Ptr == IntPtr.Zero)
		{
			throw new ObjectDisposedException("uav");
		}
		Internal_SetRandomWriteTargetBuffer(index, uav, preserveCounterValue);
	}

	/// <summary>
	///   <para>Clear random write targets for level pixel shaders.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void ClearRandomWriteTargets();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetRandomWriteTargetRT(int index, RenderTexture uav);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetRandomWriteTargetBuffer(int index, ComputeBuffer uav, bool preserveCounterValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CopyTexture")]
	private static extern void CopyTexture_Full(Texture src, Texture dst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CopyTexture")]
	private static extern void CopyTexture_Slice_AllMips(Texture src, int srcElement, Texture dst, int dstElement);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CopyTexture")]
	private static extern void CopyTexture_Slice(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CopyTexture")]
	private static extern void CopyTexture_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ConvertTexture")]
	private static extern bool ConvertTexture_Full(Texture src, Texture dst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ConvertTexture")]
	private static extern bool ConvertTexture_Slice(Texture src, int srcElement, Texture dst, int dstElement);

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, null, 0, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, 0, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows, null, LightProbeUsage.BlendProbes, null);
	}

	/// <summary>
	///   <para>Draw a mesh.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="position">Position of the mesh.</param>
	/// <param name="rotation">Rotation of the mesh.</param>
	/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
	/// <param name="material">Material to use.</param>
	/// <param name="layer"> to use.</param>
	/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
	/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
	/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
	/// <param name="castShadows">Should the mesh cast shadows?</param>
	/// <param name="receiveShadows">Should the mesh receive shadows?</param>
	/// <param name="useLightProbes">Should the mesh use light probes?</param>
	/// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
	/// <param name="lightProbeUsage">LightProbeUsage for the mesh.</param>
	/// <param name="lightProbeProxyVolume"></param>
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("true")] bool castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("true")] bool useLightProbes)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows, null, useLightProbes ? LightProbeUsage.BlendProbes : LightProbeUsage.Off, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, LightProbeUsage.BlendProbes, null);
	}

	/// <summary>
	///   <para>Draw a mesh.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="position">Position of the mesh.</param>
	/// <param name="rotation">Rotation of the mesh.</param>
	/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
	/// <param name="material">Material to use.</param>
	/// <param name="layer"> to use.</param>
	/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
	/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
	/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
	/// <param name="castShadows">Should the mesh cast shadows?</param>
	/// <param name="receiveShadows">Should the mesh receive shadows?</param>
	/// <param name="useLightProbes">Should the mesh use light probes?</param>
	/// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
	/// <param name="lightProbeUsage">LightProbeUsage for the mesh.</param>
	/// <param name="lightProbeProxyVolume"></param>
	public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("null")] Transform probeAnchor, [DefaultValue("true")] bool useLightProbes)
	{
		DrawMesh(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes ? LightProbeUsage.BlendProbes : LightProbeUsage.Off, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer)
	{
		DrawMesh(mesh, matrix, material, layer, null, 0, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera)
	{
		DrawMesh(mesh, matrix, material, layer, camera, 0, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, null, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, ShadowCastingMode.On, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows, null, LightProbeUsage.BlendProbes, null);
	}

	/// <summary>
	///   <para>Draw a mesh.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="position">Position of the mesh.</param>
	/// <param name="rotation">Rotation of the mesh.</param>
	/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
	/// <param name="material">Material to use.</param>
	/// <param name="layer"> to use.</param>
	/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
	/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
	/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
	/// <param name="castShadows">Should the mesh cast shadows?</param>
	/// <param name="receiveShadows">Should the mesh receive shadows?</param>
	/// <param name="useLightProbes">Should the mesh use light probes?</param>
	/// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
	/// <param name="lightProbeUsage">LightProbeUsage for the mesh.</param>
	/// <param name="lightProbeProxyVolume"></param>
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("true")] bool castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("true")] bool useLightProbes)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows, null, useLightProbes ? LightProbeUsage.BlendProbes : LightProbeUsage.Off, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows: true, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, LightProbeUsage.BlendProbes, null);
	}

	/// <summary>
	///   <para>Draw a mesh.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="position">Position of the mesh.</param>
	/// <param name="rotation">Rotation of the mesh.</param>
	/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
	/// <param name="material">Material to use.</param>
	/// <param name="layer"> to use.</param>
	/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
	/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
	/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
	/// <param name="castShadows">Should the mesh cast shadows?</param>
	/// <param name="receiveShadows">Should the mesh receive shadows?</param>
	/// <param name="useLightProbes">Should the mesh use light probes?</param>
	/// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
	/// <param name="lightProbeUsage">LightProbeUsage for the mesh.</param>
	/// <param name="lightProbeProxyVolume"></param>
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("null")] Transform probeAnchor, [DefaultValue("true")] bool useLightProbes)
	{
		DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes ? LightProbeUsage.BlendProbes : LightProbeUsage.Off, null);
	}

	/// <summary>
	///   <para>Draw a mesh.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="position">Position of the mesh.</param>
	/// <param name="rotation">Rotation of the mesh.</param>
	/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
	/// <param name="material">Material to use.</param>
	/// <param name="layer"> to use.</param>
	/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
	/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
	/// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
	/// <param name="castShadows">Should the mesh cast shadows?</param>
	/// <param name="receiveShadows">Should the mesh receive shadows?</param>
	/// <param name="useLightProbes">Should the mesh use light probes?</param>
	/// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
	/// <param name="lightProbeUsage">LightProbeUsage for the mesh.</param>
	/// <param name="lightProbeProxyVolume"></param>
	[ExcludeFromDocs]
	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage)
	{
		Internal_DrawMesh(mesh, submeshIndex, matrix, material, layer, camera, properties, castShadows, receiveShadows, probeAnchor, lightProbeUsage, null);
	}

	public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
	{
		if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
		{
			throw new ArgumentException("lightProbeProxyVolume", "Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.");
		}
		Internal_DrawMesh(mesh, submeshIndex, matrix, material, layer, camera, properties, castShadows, receiveShadows, probeAnchor, lightProbeUsage, lightProbeProxyVolume);
	}

	[FreeFunction("DrawMeshMatrixFromScript")]
	internal static void Internal_DrawMesh(Mesh mesh, int submeshIndex, Matrix4x4 matrix, Material material, int layer, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume)
	{
		Internal_DrawMesh_Injected(mesh, submeshIndex, ref matrix, material, layer, camera, properties, castShadows, receiveShadows, probeAnchor, lightProbeUsage, lightProbeProxyVolume);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, matrices.Length, null, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, null, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera, LightProbeUsage.BlendProbes, null);
	}

	/// <summary>
	///   <para>Draw the same mesh multiple times using GPU instancing.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
	/// <param name="material">Material to use.</param>
	/// <param name="matrices">The array of object transformation matrices.</param>
	/// <param name="count">The number of instances to be drawn.</param>
	/// <param name="properties">Additional material properties to apply. See MaterialPropertyBlock.</param>
	/// <param name="castShadows">Should the meshes cast shadows?</param>
	/// <param name="receiveShadows">Should the meshes receive shadows?</param>
	/// <param name="layer"> to use.</param>
	/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be drawn in the given camera only.</param>
	/// <param name="lightProbeUsage">LightProbeUsage for the instances.</param>
	/// <param name="lightProbeProxyVolume"></param>
	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, null);
	}

	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, [DefaultValue("matrices.Length")] int count, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera, [DefaultValue("LightProbeUsage.BlendProbes")] LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
		}
		if (material == null)
		{
			throw new ArgumentNullException("material");
		}
		if (!material.enableInstancing)
		{
			throw new InvalidOperationException("Material needs to enable instancing for use with DrawMeshInstanced.");
		}
		if (matrices == null)
		{
			throw new ArgumentNullException("matrices");
		}
		if (count < 0 || count > Mathf.Min(kMaxDrawMeshInstanceCount, matrices.Length))
		{
			throw new ArgumentOutOfRangeException("count", $"Count must be in the range of 0 to {Mathf.Min(kMaxDrawMeshInstanceCount, matrices.Length)}.");
		}
		if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
		{
			throw new ArgumentException("lightProbeProxyVolume", "Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.");
		}
		if (count > 0)
		{
			Internal_DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
		}
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, null, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage)
	{
		DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, null);
	}

	public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera, [DefaultValue("LightProbeUsage.BlendProbes")] LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
	{
		if (matrices == null)
		{
			throw new ArgumentNullException("matrices");
		}
		DrawMeshInstanced(mesh, submeshIndex, material, NoAllocHelpers.ExtractArrayFromListT(matrices), matrices.Count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("DrawMeshInstancedFromScript")]
	internal static extern void Internal_DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume);

	[ExcludeFromDocs]
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs)
	{
		DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, 0, null, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset)
	{
		DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, null, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
	{
		DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, ShadowCastingMode.On, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
	{
		DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows: true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
	{
		DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, 0, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
	{
		DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, null, LightProbeUsage.BlendProbes, null);
	}

	[ExcludeFromDocs]
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
	{
		DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, LightProbeUsage.BlendProbes, null);
	}

	/// <summary>
	///   <para>Draw the same mesh multiple times using GPU instancing.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
	/// <param name="material">Material to use.</param>
	/// <param name="bounds">The bounding volume surrounding the instances you intend to draw.</param>
	/// <param name="bufferWithArgs">The GPU buffer containing the arguments for how many instances of this mesh to draw.</param>
	/// <param name="argsOffset">The byte offset into the buffer, where the draw arguments start.</param>
	/// <param name="properties">Additional material properties to apply. See MaterialPropertyBlock.</param>
	/// <param name="castShadows">Should the mesh cast shadows?</param>
	/// <param name="receiveShadows">Should the mesh receive shadows?</param>
	/// <param name="layer"> to use.</param>
	/// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be drawn in the given camera only.</param>
	/// <param name="lightProbeUsage">LightProbeUsage for the instances.</param>
	/// <param name="lightProbeProxyVolume"></param>
	[ExcludeFromDocs]
	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage)
	{
		DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, null);
	}

	public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera, [DefaultValue("LightProbeUsage.BlendProbes")] LightProbeUsage lightProbeUsage, [DefaultValue("null")] LightProbeProxyVolume lightProbeProxyVolume)
	{
		if (!SystemInfo.supportsInstancing)
		{
			throw new InvalidOperationException("Instancing is not supported.");
		}
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
		}
		if (material == null)
		{
			throw new ArgumentNullException("material");
		}
		if (bufferWithArgs == null)
		{
			throw new ArgumentNullException("bufferWithArgs");
		}
		if (lightProbeUsage == LightProbeUsage.UseProxyVolume && lightProbeProxyVolume == null)
		{
			throw new ArgumentException("lightProbeProxyVolume", "Argument lightProbeProxyVolume must not be null if lightProbeUsage is set to UseProxyVolume.");
		}
		Internal_DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
	}

	[FreeFunction("DrawMeshInstancedIndirectFromScript")]
	internal static void Internal_DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume)
	{
		Internal_DrawMeshInstancedIndirect_Injected(mesh, submeshIndex, material, ref bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsScripting::BlitMaterial")]
	private static extern void Internal_BlitMaterial(Texture source, RenderTexture dest, [NotNull] Material mat, int pass, bool setRT);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsScripting::BlitMultitap")]
	private static extern void Internal_BlitMultiTap(Texture source, RenderTexture dest, [NotNull] Material mat, [NotNull] Vector2[] offsets);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsScripting::Blit")]
	private static extern void Blit2(Texture source, RenderTexture dest);

	[FreeFunction("GraphicsScripting::Blit")]
	private static void Blit4(Texture source, RenderTexture dest, Vector2 scale, Vector2 offset)
	{
		Blit4_Injected(source, dest, ref scale, ref offset);
	}

	internal static void CheckLoadActionValid(RenderBufferLoadAction load, string bufferType)
	{
		if (load != 0 && load != RenderBufferLoadAction.DontCare)
		{
			throw new ArgumentException(UnityString.Format("Bad {0} LoadAction provided.", bufferType));
		}
	}

	internal static void CheckStoreActionValid(RenderBufferStoreAction store, string bufferType)
	{
		if (store != 0 && store != RenderBufferStoreAction.DontCare)
		{
			throw new ArgumentException(UnityString.Format("Bad {0} StoreAction provided.", bufferType));
		}
	}

	internal static void SetRenderTargetImpl(RenderTargetSetup setup)
	{
		if (setup.color.Length == 0)
		{
			throw new ArgumentException("Invalid color buffer count for SetRenderTarget");
		}
		if (setup.color.Length != setup.colorLoad.Length)
		{
			throw new ArgumentException("Color LoadAction and Buffer arrays have different sizes");
		}
		if (setup.color.Length != setup.colorStore.Length)
		{
			throw new ArgumentException("Color StoreAction and Buffer arrays have different sizes");
		}
		RenderBufferLoadAction[] colorLoad = setup.colorLoad;
		foreach (RenderBufferLoadAction load in colorLoad)
		{
			CheckLoadActionValid(load, "Color");
		}
		RenderBufferStoreAction[] colorStore = setup.colorStore;
		foreach (RenderBufferStoreAction store in colorStore)
		{
			CheckStoreActionValid(store, "Color");
		}
		CheckLoadActionValid(setup.depthLoad, "Depth");
		CheckStoreActionValid(setup.depthStore, "Depth");
		if (setup.cubemapFace < CubemapFace.Unknown || setup.cubemapFace > CubemapFace.NegativeZ)
		{
			throw new ArgumentException("Bad CubemapFace provided");
		}
		Internal_SetMRTFullSetup(setup.color, out setup.depth, setup.mipLevel, setup.cubemapFace, setup.depthSlice, setup.colorLoad, setup.colorStore, setup.depthLoad, setup.depthStore);
	}

	internal static void SetRenderTargetImpl(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
	{
		RenderBuffer color = colorBuffer;
		RenderBuffer depth = depthBuffer;
		Internal_SetRTSimple(out color, out depth, mipLevel, face, depthSlice);
	}

	internal static void SetRenderTargetImpl(RenderTexture rt, int mipLevel, CubemapFace face, int depthSlice)
	{
		if ((bool)rt)
		{
			SetRenderTargetImpl(rt.colorBuffer, rt.depthBuffer, mipLevel, face, depthSlice);
		}
		else
		{
			Internal_SetNullRT();
		}
	}

	internal static void SetRenderTargetImpl(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
	{
		RenderBuffer depth = depthBuffer;
		Internal_SetMRTSimple(colorBuffers, out depth, mipLevel, face, depthSlice);
	}

	/// <summary>
	///   <para>Sets current render target.</para>
	/// </summary>
	/// <param name="rt">RenderTexture to set as active render target.</param>
	/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
	/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
	/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
	/// <param name="colorBuffer">Color buffer to render into.</param>
	/// <param name="depthBuffer">Depth buffer to render into.</param>
	/// <param name="colorBuffers">
	/// Color buffers to render into (for multiple render target effects).</param>
	/// <param name="setup">Full render target setup information.</param>
	public static void SetRenderTarget(RenderTexture rt)
	{
		SetRenderTargetImpl(rt, 0, CubemapFace.Unknown, 0);
	}

	/// <summary>
	///   <para>Sets current render target.</para>
	/// </summary>
	/// <param name="rt">RenderTexture to set as active render target.</param>
	/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
	/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
	/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
	/// <param name="colorBuffer">Color buffer to render into.</param>
	/// <param name="depthBuffer">Depth buffer to render into.</param>
	/// <param name="colorBuffers">
	/// Color buffers to render into (for multiple render target effects).</param>
	/// <param name="setup">Full render target setup information.</param>
	public static void SetRenderTarget(RenderTexture rt, int mipLevel)
	{
		SetRenderTargetImpl(rt, mipLevel, CubemapFace.Unknown, 0);
	}

	/// <summary>
	///   <para>Sets current render target.</para>
	/// </summary>
	/// <param name="rt">RenderTexture to set as active render target.</param>
	/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
	/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
	/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
	/// <param name="colorBuffer">Color buffer to render into.</param>
	/// <param name="depthBuffer">Depth buffer to render into.</param>
	/// <param name="colorBuffers">
	/// Color buffers to render into (for multiple render target effects).</param>
	/// <param name="setup">Full render target setup information.</param>
	public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face)
	{
		SetRenderTargetImpl(rt, mipLevel, face, 0);
	}

	/// <summary>
	///   <para>Sets current render target.</para>
	/// </summary>
	/// <param name="rt">RenderTexture to set as active render target.</param>
	/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
	/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
	/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
	/// <param name="colorBuffer">Color buffer to render into.</param>
	/// <param name="depthBuffer">Depth buffer to render into.</param>
	/// <param name="colorBuffers">
	/// Color buffers to render into (for multiple render target effects).</param>
	/// <param name="setup">Full render target setup information.</param>
	public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face, int depthSlice)
	{
		SetRenderTargetImpl(rt, mipLevel, face, depthSlice);
	}

	/// <summary>
	///   <para>Sets current render target.</para>
	/// </summary>
	/// <param name="rt">RenderTexture to set as active render target.</param>
	/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
	/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
	/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
	/// <param name="colorBuffer">Color buffer to render into.</param>
	/// <param name="depthBuffer">Depth buffer to render into.</param>
	/// <param name="colorBuffers">
	/// Color buffers to render into (for multiple render target effects).</param>
	/// <param name="setup">Full render target setup information.</param>
	public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
	{
		SetRenderTargetImpl(colorBuffer, depthBuffer, 0, CubemapFace.Unknown, 0);
	}

	/// <summary>
	///   <para>Sets current render target.</para>
	/// </summary>
	/// <param name="rt">RenderTexture to set as active render target.</param>
	/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
	/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
	/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
	/// <param name="colorBuffer">Color buffer to render into.</param>
	/// <param name="depthBuffer">Depth buffer to render into.</param>
	/// <param name="colorBuffers">
	/// Color buffers to render into (for multiple render target effects).</param>
	/// <param name="setup">Full render target setup information.</param>
	public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel)
	{
		SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, CubemapFace.Unknown, 0);
	}

	/// <summary>
	///   <para>Sets current render target.</para>
	/// </summary>
	/// <param name="rt">RenderTexture to set as active render target.</param>
	/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
	/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
	/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
	/// <param name="colorBuffer">Color buffer to render into.</param>
	/// <param name="depthBuffer">Depth buffer to render into.</param>
	/// <param name="colorBuffers">
	/// Color buffers to render into (for multiple render target effects).</param>
	/// <param name="setup">Full render target setup information.</param>
	public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face)
	{
		SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face, 0);
	}

	/// <summary>
	///   <para>Sets current render target.</para>
	/// </summary>
	/// <param name="rt">RenderTexture to set as active render target.</param>
	/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
	/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
	/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
	/// <param name="colorBuffer">Color buffer to render into.</param>
	/// <param name="depthBuffer">Depth buffer to render into.</param>
	/// <param name="colorBuffers">
	/// Color buffers to render into (for multiple render target effects).</param>
	/// <param name="setup">Full render target setup information.</param>
	public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
	{
		SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face, depthSlice);
	}

	/// <summary>
	///   <para>Sets current render target.</para>
	/// </summary>
	/// <param name="rt">RenderTexture to set as active render target.</param>
	/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
	/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
	/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
	/// <param name="colorBuffer">Color buffer to render into.</param>
	/// <param name="depthBuffer">Depth buffer to render into.</param>
	/// <param name="colorBuffers">
	/// Color buffers to render into (for multiple render target effects).</param>
	/// <param name="setup">Full render target setup information.</param>
	public static void SetRenderTarget(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer)
	{
		SetRenderTargetImpl(colorBuffers, depthBuffer, 0, CubemapFace.Unknown, 0);
	}

	/// <summary>
	///   <para>Sets current render target.</para>
	/// </summary>
	/// <param name="rt">RenderTexture to set as active render target.</param>
	/// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
	/// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
	/// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
	/// <param name="colorBuffer">Color buffer to render into.</param>
	/// <param name="depthBuffer">Depth buffer to render into.</param>
	/// <param name="colorBuffers">
	/// Color buffers to render into (for multiple render target effects).</param>
	/// <param name="setup">Full render target setup information.</param>
	public static void SetRenderTarget(RenderTargetSetup setup)
	{
		SetRenderTargetImpl(setup);
	}

	/// <summary>
	///   <para>Copy texture contents.</para>
	/// </summary>
	/// <param name="src">Source texture.</param>
	/// <param name="dst">Destination texture.</param>
	/// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
	/// <param name="srcMip">Source texture mipmap level.</param>
	/// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
	/// <param name="dstMip">Destination texture mipmap level.</param>
	/// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
	/// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
	/// <param name="srcWidth">Width of source texture region to copy.</param>
	/// <param name="srcHeight">Height of source texture region to copy.</param>
	/// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
	/// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
	public static void CopyTexture(Texture src, Texture dst)
	{
		CopyTexture_Full(src, dst);
	}

	public static void CopyTexture(Texture src, int srcElement, Texture dst, int dstElement)
	{
		CopyTexture_Slice_AllMips(src, srcElement, dst, dstElement);
	}

	/// <summary>
	///   <para>Copy texture contents.</para>
	/// </summary>
	/// <param name="src">Source texture.</param>
	/// <param name="dst">Destination texture.</param>
	/// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
	/// <param name="srcMip">Source texture mipmap level.</param>
	/// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
	/// <param name="dstMip">Destination texture mipmap level.</param>
	/// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
	/// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
	/// <param name="srcWidth">Width of source texture region to copy.</param>
	/// <param name="srcHeight">Height of source texture region to copy.</param>
	/// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
	/// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
	public static void CopyTexture(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip)
	{
		CopyTexture_Slice(src, srcElement, srcMip, dst, dstElement, dstMip);
	}

	/// <summary>
	///   <para>Copy texture contents.</para>
	/// </summary>
	/// <param name="src">Source texture.</param>
	/// <param name="dst">Destination texture.</param>
	/// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
	/// <param name="srcMip">Source texture mipmap level.</param>
	/// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
	/// <param name="dstMip">Destination texture mipmap level.</param>
	/// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
	/// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
	/// <param name="srcWidth">Width of source texture region to copy.</param>
	/// <param name="srcHeight">Height of source texture region to copy.</param>
	/// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
	/// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
	public static void CopyTexture(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY)
	{
		CopyTexture_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dst, dstElement, dstMip, dstX, dstY);
	}

	/// <summary>
	///   <para>This function provides an efficient way to convert between textures of different formats and dimensions.
	/// The destination texture format should be uncompressed and correspond to a supported RenderTextureFormat.</para>
	/// </summary>
	/// <param name="src">Source texture.</param>
	/// <param name="dst">Destination texture.</param>
	/// <param name="srcElement">Source element (e.g. cubemap face).  Set this to 0 for 2d source textures.</param>
	/// <param name="dstElement">Destination element (e.g. cubemap face or texture array element).</param>
	/// <returns>
	///   <para>True if the call succeeded.</para>
	/// </returns>
	public static bool ConvertTexture(Texture src, Texture dst)
	{
		return ConvertTexture_Full(src, dst);
	}

	/// <summary>
	///   <para>This function provides an efficient way to convert between textures of different formats and dimensions.
	/// The destination texture format should be uncompressed and correspond to a supported RenderTextureFormat.</para>
	/// </summary>
	/// <param name="src">Source texture.</param>
	/// <param name="dst">Destination texture.</param>
	/// <param name="srcElement">Source element (e.g. cubemap face).  Set this to 0 for 2d source textures.</param>
	/// <param name="dstElement">Destination element (e.g. cubemap face or texture array element).</param>
	/// <returns>
	///   <para>True if the call succeeded.</para>
	/// </returns>
	public static bool ConvertTexture(Texture src, int srcElement, Texture dst, int dstElement)
	{
		return ConvertTexture_Slice(src, srcElement, dst, dstElement);
	}

	private static void DrawTextureImpl(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat, int pass)
	{
		Internal_DrawTextureArguments args = default(Internal_DrawTextureArguments);
		args.screenRect = screenRect;
		args.sourceRect = sourceRect;
		args.leftBorder = leftBorder;
		args.rightBorder = rightBorder;
		args.topBorder = topBorder;
		args.bottomBorder = bottomBorder;
		args.color = color;
		args.pass = pass;
		args.texture = texture;
		args.mat = mat;
		Internal_DrawTexture(ref args);
	}

	/// <summary>
	///   <para>Draw a mesh immediately.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="position">Position of the mesh.</param>
	/// <param name="rotation">Rotation of the mesh.</param>
	/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
	/// <param name="materialIndex">Subset of the mesh to draw.</param>
	public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation)
	{
		DrawMeshNow(mesh, position, rotation, -1);
	}

	/// <summary>
	///   <para>Draw a mesh immediately.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="position">Position of the mesh.</param>
	/// <param name="rotation">Rotation of the mesh.</param>
	/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
	/// <param name="materialIndex">Subset of the mesh to draw.</param>
	public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
	{
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		Internal_DrawMeshNow1(mesh, materialIndex, position, rotation);
	}

	/// <summary>
	///   <para>Draw a mesh immediately.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="position">Position of the mesh.</param>
	/// <param name="rotation">Rotation of the mesh.</param>
	/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
	/// <param name="materialIndex">Subset of the mesh to draw.</param>
	public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix)
	{
		DrawMeshNow(mesh, matrix, -1);
	}

	/// <summary>
	///   <para>Draw a mesh immediately.</para>
	/// </summary>
	/// <param name="mesh">The Mesh to draw.</param>
	/// <param name="position">Position of the mesh.</param>
	/// <param name="rotation">Rotation of the mesh.</param>
	/// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
	/// <param name="materialIndex">Subset of the mesh to draw.</param>
	public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix, int materialIndex)
	{
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		Internal_DrawMeshNow2(mesh, materialIndex, matrix);
	}

	/// <summary>
	///   <para>Copies source texture into destination render texture with a shader.</para>
	/// </summary>
	/// <param name="source">Source texture.</param>
	/// <param name="dest">The destination RenderTexture. Set this to null to blit directly to screen. See description for more information.</param>
	/// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	/// <param name="offset">Offset applied to the source texture coordinate.</param>
	/// <param name="scale">Scale applied to the source texture coordinate.</param>
	public static void Blit(Texture source, RenderTexture dest)
	{
		Blit2(source, dest);
	}

	/// <summary>
	///   <para>Copies source texture into destination render texture with a shader.</para>
	/// </summary>
	/// <param name="source">Source texture.</param>
	/// <param name="dest">The destination RenderTexture. Set this to null to blit directly to screen. See description for more information.</param>
	/// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	/// <param name="offset">Offset applied to the source texture coordinate.</param>
	/// <param name="scale">Scale applied to the source texture coordinate.</param>
	public static void Blit(Texture source, RenderTexture dest, Vector2 scale, Vector2 offset)
	{
		Blit4(source, dest, scale, offset);
	}

	/// <summary>
	///   <para>Copies source texture into destination render texture with a shader.</para>
	/// </summary>
	/// <param name="source">Source texture.</param>
	/// <param name="dest">The destination RenderTexture. Set this to null to blit directly to screen. See description for more information.</param>
	/// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	/// <param name="offset">Offset applied to the source texture coordinate.</param>
	/// <param name="scale">Scale applied to the source texture coordinate.</param>
	public static void Blit(Texture source, RenderTexture dest, Material mat, [DefaultValue("-1")] int pass)
	{
		Internal_BlitMaterial(source, dest, mat, pass, setRT: true);
	}

	public static void Blit(Texture source, RenderTexture dest, Material mat)
	{
		Blit(source, dest, mat, -1);
	}

	/// <summary>
	///   <para>Copies source texture into destination render texture with a shader.</para>
	/// </summary>
	/// <param name="source">Source texture.</param>
	/// <param name="dest">The destination RenderTexture. Set this to null to blit directly to screen. See description for more information.</param>
	/// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
	/// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
	/// <param name="offset">Offset applied to the source texture coordinate.</param>
	/// <param name="scale">Scale applied to the source texture coordinate.</param>
	public static void Blit(Texture source, Material mat, [DefaultValue("-1")] int pass)
	{
		Internal_BlitMaterial(source, null, mat, pass, setRT: false);
	}

	public static void Blit(Texture source, Material mat)
	{
		Blit(source, mat, -1);
	}

	/// <summary>
	///   <para>Copies source texture into destination, for multi-tap shader.</para>
	/// </summary>
	/// <param name="source">Source texture.</param>
	/// <param name="dest">Destination RenderTexture, or null to blit directly to screen.</param>
	/// <param name="mat">Material to use for copying. Material's shader should do some post-processing effect.</param>
	/// <param name="offsets">Variable number of filtering offsets. Offsets are given in pixels.</param>
	public static void BlitMultiTap(Texture source, RenderTexture dest, Material mat, params Vector2[] offsets)
	{
		if (offsets.Length == 0)
		{
			throw new ArgumentException("empty offsets list passed.", "offsets");
		}
		Internal_BlitMultiTap(source, dest, mat, offsets);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawMesh_Injected(Mesh mesh, int submeshIndex, ref Matrix4x4 matrix, Material material, int layer, Camera camera, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawMeshInstancedIndirect_Injected(Mesh mesh, int submeshIndex, Material material, ref Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera, LightProbeUsage lightProbeUsage, LightProbeProxyVolume lightProbeProxyVolume);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Blit4_Injected(Texture source, RenderTexture dest, ref Vector2 scale, ref Vector2 offset);
}
