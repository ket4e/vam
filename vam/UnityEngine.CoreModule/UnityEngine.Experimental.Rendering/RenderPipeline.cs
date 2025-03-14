using System;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Defines a series of commands and settings that describes how Unity renders a frame.</para>
/// </summary>
public abstract class RenderPipeline : IRenderPipeline, IDisposable
{
	/// <summary>
	///   <para>When the IRenderPipeline is invalid or destroyed this returns true.</para>
	/// </summary>
	public bool disposed { get; private set; }

	public static event Action<Camera[]> beginFrameRendering;

	public static event Action<Camera> beginCameraRendering;

	/// <summary>
	///   <para>Defines custom rendering for this RenderPipeline.</para>
	/// </summary>
	/// <param name="renderContext"></param>
	/// <param name="cameras"></param>
	public virtual void Render(ScriptableRenderContext renderContext, Camera[] cameras)
	{
		if (disposed)
		{
			throw new ObjectDisposedException($"{this} has been disposed. Do not call Render on disposed RenderLoops.");
		}
	}

	/// <summary>
	///   <para>Dispose the Renderpipeline destroying all internal state.</para>
	/// </summary>
	public virtual void Dispose()
	{
		disposed = true;
	}

	/// <summary>
	///   <para>Call the delegate used during SRP rendering before a render begins.</para>
	/// </summary>
	/// <param name="cameras"></param>
	public static void BeginFrameRendering(Camera[] cameras)
	{
		if (RenderPipeline.beginFrameRendering != null)
		{
			RenderPipeline.beginFrameRendering(cameras);
		}
	}

	/// <summary>
	///   <para>Call the delegate used during SRP rendering before a single camera starts rendering.</para>
	/// </summary>
	/// <param name="camera"></param>
	public static void BeginCameraRendering(Camera camera)
	{
		if (RenderPipeline.beginCameraRendering != null)
		{
			RenderPipeline.beginCameraRendering(camera);
		}
	}
}
