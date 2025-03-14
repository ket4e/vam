using System;
using UnityEngine.Rendering;

namespace UnityEngine.XR;

/// <summary>
///   <para>Class used to override a camera's default background rendering path to instead render a given Texture and/or Material. This will typically be used with images from the color camera for rendering the AR background on mobile devices.</para>
/// </summary>
public class ARBackgroundRenderer
{
	protected Camera m_Camera = null;

	protected Material m_BackgroundMaterial = null;

	protected Texture m_BackgroundTexture = null;

	private ARRenderMode m_RenderMode = ARRenderMode.StandardBackground;

	private CommandBuffer m_CommandBuffer = null;

	private CameraClearFlags m_CameraClearFlags = CameraClearFlags.Skybox;

	/// <summary>
	///   <para>The Material used for AR rendering.</para>
	/// </summary>
	public Material backgroundMaterial
	{
		get
		{
			return m_BackgroundMaterial;
		}
		set
		{
			if (!(m_BackgroundMaterial == value))
			{
				RemoveCommandBuffersIfNeeded();
				m_BackgroundMaterial = value;
				if (this.backgroundRendererChanged != null)
				{
					this.backgroundRendererChanged();
				}
				ReapplyCommandBuffersIfNeeded();
			}
		}
	}

	/// <summary>
	///   <para>An optional Texture used for AR rendering. If this property is not set then the texture set in XR.ARBackgroundRenderer._backgroundMaterial as "_MainTex" is used.</para>
	/// </summary>
	public Texture backgroundTexture
	{
		get
		{
			return m_BackgroundTexture;
		}
		set
		{
			if (!(m_BackgroundTexture = value))
			{
				RemoveCommandBuffersIfNeeded();
				m_BackgroundTexture = value;
				if (this.backgroundRendererChanged != null)
				{
					this.backgroundRendererChanged();
				}
				ReapplyCommandBuffersIfNeeded();
			}
		}
	}

	/// <summary>
	///   <para>An optional Camera whose background rendering will be overridden by this class. If this property is not set then the main Camera in the scene is used.</para>
	/// </summary>
	public Camera camera
	{
		get
		{
			return (!(m_Camera != null)) ? Camera.main : m_Camera;
		}
		set
		{
			if (!(m_Camera == value))
			{
				RemoveCommandBuffersIfNeeded();
				m_Camera = value;
				if (this.backgroundRendererChanged != null)
				{
					this.backgroundRendererChanged();
				}
				ReapplyCommandBuffersIfNeeded();
			}
		}
	}

	/// <summary>
	///   <para>When set to XR.ARRenderMode.StandardBackground (default) the camera is not overridden to display the background image. Setting this property to XR.ARRenderMode.MaterialAsBackground will render the texture specified by XR.ARBackgroundRenderer._backgroundMaterial and or XR.ARBackgroundRenderer._backgroundTexture as the background.</para>
	/// </summary>
	public ARRenderMode mode
	{
		get
		{
			return m_RenderMode;
		}
		set
		{
			if (value != m_RenderMode)
			{
				m_RenderMode = value;
				switch (m_RenderMode)
				{
				case ARRenderMode.StandardBackground:
					DisableARBackgroundRendering();
					break;
				case ARRenderMode.MaterialAsBackground:
					EnableARBackgroundRendering();
					break;
				default:
					throw new Exception("Unhandled render mode.");
				}
				if (this.backgroundRendererChanged != null)
				{
					this.backgroundRendererChanged();
				}
			}
		}
	}

	public event Action backgroundRendererChanged = null;

	protected bool EnableARBackgroundRendering()
	{
		if (m_BackgroundMaterial == null)
		{
			return false;
		}
		Camera camera = ((!(m_Camera != null)) ? Camera.main : m_Camera);
		if (camera == null)
		{
			return false;
		}
		m_CameraClearFlags = camera.clearFlags;
		camera.clearFlags = CameraClearFlags.Depth;
		m_CommandBuffer = new CommandBuffer();
		Texture texture = m_BackgroundTexture;
		if (texture == null && m_BackgroundMaterial.HasProperty("_MainTex"))
		{
			texture = m_BackgroundMaterial.GetTexture("_MainTex");
		}
		m_CommandBuffer.Blit(texture, BuiltinRenderTextureType.CameraTarget, m_BackgroundMaterial);
		camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_CommandBuffer);
		camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, m_CommandBuffer);
		return true;
	}

	/// <summary>
	///   <para>Disables AR background rendering. This method is called internally but can be overridden by users who wish to subclass XR.ARBackgroundRenderer to customize handling of AR background rendering.</para>
	/// </summary>
	protected void DisableARBackgroundRendering()
	{
		if (m_CommandBuffer != null)
		{
			Camera camera = ((!(m_Camera != null)) ? Camera.main : m_Camera);
			if (camera != null)
			{
				camera.clearFlags = m_CameraClearFlags;
			}
			camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_CommandBuffer);
			camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, m_CommandBuffer);
		}
	}

	private bool ReapplyCommandBuffersIfNeeded()
	{
		if (m_RenderMode != ARRenderMode.MaterialAsBackground)
		{
			return false;
		}
		EnableARBackgroundRendering();
		return true;
	}

	private bool RemoveCommandBuffersIfNeeded()
	{
		if (m_RenderMode != ARRenderMode.MaterialAsBackground)
		{
			return false;
		}
		DisableARBackgroundRendering();
		return true;
	}
}
