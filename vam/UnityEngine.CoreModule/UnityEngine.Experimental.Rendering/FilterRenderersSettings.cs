namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Filter settings for ScriptableRenderContext.DrawRenderers.</para>
/// </summary>
public struct FilterRenderersSettings
{
	private RenderQueueRange m_RenderQueueRange;

	private int m_LayerMask;

	private uint m_RenderingLayerMask;

	/// <summary>
	///   <para>Render objects whose material render queue in inside this range.</para>
	/// </summary>
	public RenderQueueRange renderQueueRange
	{
		get
		{
			return m_RenderQueueRange;
		}
		set
		{
			m_RenderQueueRange = value;
		}
	}

	/// <summary>
	///   <para>Only render objects in the given layer mask.</para>
	/// </summary>
	public int layerMask
	{
		get
		{
			return m_LayerMask;
		}
		set
		{
			m_LayerMask = value;
		}
	}

	/// <summary>
	///   <para>The rendering layer mask to use when filtering available renderers for drawing.</para>
	/// </summary>
	public uint renderingLayerMask
	{
		get
		{
			return m_RenderingLayerMask;
		}
		set
		{
			m_RenderingLayerMask = value;
		}
	}

	/// <summary>
	///   <para></para>
	/// </summary>
	/// <param name="initializeValues">Specifies whether the values of the struct should be initialized.</param>
	public FilterRenderersSettings(bool initializeValues = false)
	{
		this = default(FilterRenderersSettings);
		if (initializeValues)
		{
			m_RenderQueueRange = RenderQueueRange.all;
			m_LayerMask = -1;
			m_RenderingLayerMask = uint.MaxValue;
		}
	}
}
