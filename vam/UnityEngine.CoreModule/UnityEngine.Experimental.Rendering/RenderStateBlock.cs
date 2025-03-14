namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>A set of values used to override the render state. Note that it is not enough to set e.g. blendState, but that mask must also include RenderStateMask.Blend for the override to occur.</para>
/// </summary>
public struct RenderStateBlock
{
	private BlendState m_BlendState;

	private RasterState m_RasterState;

	private DepthState m_DepthState;

	private StencilState m_StencilState;

	private int m_StencilReference;

	private RenderStateMask m_Mask;

	/// <summary>
	///   <para>Specifies the new blend state.</para>
	/// </summary>
	public BlendState blendState
	{
		get
		{
			return m_BlendState;
		}
		set
		{
			m_BlendState = value;
		}
	}

	/// <summary>
	///   <para>Specifies the new raster state.</para>
	/// </summary>
	public RasterState rasterState
	{
		get
		{
			return m_RasterState;
		}
		set
		{
			m_RasterState = value;
		}
	}

	/// <summary>
	///   <para>Specifies the new depth state.</para>
	/// </summary>
	public DepthState depthState
	{
		get
		{
			return m_DepthState;
		}
		set
		{
			m_DepthState = value;
		}
	}

	/// <summary>
	///   <para>Specifies the new stencil state.</para>
	/// </summary>
	public StencilState stencilState
	{
		get
		{
			return m_StencilState;
		}
		set
		{
			m_StencilState = value;
		}
	}

	/// <summary>
	///   <para>The value to be compared against and/or the value to be written to the buffer based on the stencil state.</para>
	/// </summary>
	public int stencilReference
	{
		get
		{
			return m_StencilReference;
		}
		set
		{
			m_StencilReference = value;
		}
	}

	/// <summary>
	///   <para>Specifies which parts of the render state that is overriden.</para>
	/// </summary>
	public RenderStateMask mask
	{
		get
		{
			return m_Mask;
		}
		set
		{
			m_Mask = value;
		}
	}

	/// <summary>
	///   <para>Creates a new render state block with the specified mask.</para>
	/// </summary>
	/// <param name="mask">Specifies which parts of the render state that is overriden.</param>
	public RenderStateBlock(RenderStateMask mask)
	{
		m_BlendState = BlendState.Default;
		m_RasterState = RasterState.Default;
		m_DepthState = DepthState.Default;
		m_StencilState = StencilState.Default;
		m_StencilReference = 0;
		m_Mask = mask;
	}
}
