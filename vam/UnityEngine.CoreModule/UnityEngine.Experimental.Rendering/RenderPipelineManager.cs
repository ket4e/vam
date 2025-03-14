using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Render Pipeline manager.</para>
/// </summary>
public static class RenderPipelineManager
{
	private static IRenderPipelineAsset s_CurrentPipelineAsset;

	/// <summary>
	///   <para>Returns the instance of the currently used Render Pipeline.</para>
	/// </summary>
	public static IRenderPipeline currentPipeline { get; private set; }

	[RequiredByNativeCode]
	internal static void CleanupRenderPipeline()
	{
		if (s_CurrentPipelineAsset != null)
		{
			s_CurrentPipelineAsset.DestroyCreatedInstances();
		}
		s_CurrentPipelineAsset = null;
		currentPipeline = null;
	}

	[RequiredByNativeCode]
	private static void DoRenderLoop_Internal(IRenderPipelineAsset pipe, Camera[] cameras, IntPtr loopPtr)
	{
		PrepareRenderPipeline(pipe);
		if (currentPipeline != null)
		{
			ScriptableRenderContext renderContext = new ScriptableRenderContext(loopPtr);
			currentPipeline.Render(renderContext, cameras);
		}
	}

	private static void PrepareRenderPipeline(IRenderPipelineAsset pipe)
	{
		if (s_CurrentPipelineAsset != pipe)
		{
			if (s_CurrentPipelineAsset != null)
			{
				CleanupRenderPipeline();
			}
			s_CurrentPipelineAsset = pipe;
		}
		if (s_CurrentPipelineAsset != null && (currentPipeline == null || currentPipeline.disposed))
		{
			currentPipeline = s_CurrentPipelineAsset.CreatePipeline();
		}
	}
}
