using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Values for the blend state.</para>
/// </summary>
public struct RenderTargetBlendState
{
	private byte m_WriteMask;

	private byte m_SourceColorBlendMode;

	private byte m_DestinationColorBlendMode;

	private byte m_SourceAlphaBlendMode;

	private byte m_DestinationAlphaBlendMode;

	private byte m_ColorBlendOperation;

	private byte m_AlphaBlendOperation;

	private byte m_Padding;

	/// <summary>
	///   <para>Default values for the blend state.</para>
	/// </summary>
	public static RenderTargetBlendState Default => new RenderTargetBlendState(ColorWriteMask.All, BlendMode.One, BlendMode.Zero, BlendMode.One, BlendMode.Zero, BlendOp.Add, BlendOp.Add);

	/// <summary>
	///   <para>Specifies which color components will get written into the target framebuffer.</para>
	/// </summary>
	public ColorWriteMask writeMask
	{
		get
		{
			return (ColorWriteMask)m_WriteMask;
		}
		set
		{
			m_WriteMask = (byte)value;
		}
	}

	/// <summary>
	///   <para>Blend factor used for the color (RGB) channel of the source.</para>
	/// </summary>
	public BlendMode sourceColorBlendMode
	{
		get
		{
			return (BlendMode)m_SourceColorBlendMode;
		}
		set
		{
			m_SourceColorBlendMode = (byte)value;
		}
	}

	/// <summary>
	///   <para>Blend factor used for the color (RGB) channel of the destination.</para>
	/// </summary>
	public BlendMode destinationColorBlendMode
	{
		get
		{
			return (BlendMode)m_DestinationColorBlendMode;
		}
		set
		{
			m_DestinationColorBlendMode = (byte)value;
		}
	}

	/// <summary>
	///   <para>Blend factor used for the alpha (A) channel of the source.</para>
	/// </summary>
	public BlendMode sourceAlphaBlendMode
	{
		get
		{
			return (BlendMode)m_SourceAlphaBlendMode;
		}
		set
		{
			m_SourceAlphaBlendMode = (byte)value;
		}
	}

	/// <summary>
	///   <para>Blend factor used for the alpha (A) channel of the destination.</para>
	/// </summary>
	public BlendMode destinationAlphaBlendMode
	{
		get
		{
			return (BlendMode)m_DestinationAlphaBlendMode;
		}
		set
		{
			m_DestinationAlphaBlendMode = (byte)value;
		}
	}

	/// <summary>
	///   <para>Operation used for blending the color (RGB) channel.</para>
	/// </summary>
	public BlendOp colorBlendOperation
	{
		get
		{
			return (BlendOp)m_ColorBlendOperation;
		}
		set
		{
			m_ColorBlendOperation = (byte)value;
		}
	}

	/// <summary>
	///   <para>Operation used for blending the alpha (A) channel.</para>
	/// </summary>
	public BlendOp alphaBlendOperation
	{
		get
		{
			return (BlendOp)m_AlphaBlendOperation;
		}
		set
		{
			m_AlphaBlendOperation = (byte)value;
		}
	}

	/// <summary>
	///   <para>Creates a new blend state with the given values.</para>
	/// </summary>
	/// <param name="writeMask">Specifies which color components will get written into the target framebuffer.</param>
	/// <param name="sourceColorBlendMode">Blend factor used for the color (RGB) channel of the source.</param>
	/// <param name="destinationColorBlendMode">Blend factor used for the color (RGB) channel of the destination.</param>
	/// <param name="sourceAlphaBlendMode">Blend factor used for the alpha (A) channel of the source.</param>
	/// <param name="destinationAlphaBlendMode">Blend factor used for the alpha (A) channel of the destination.</param>
	/// <param name="colorBlendOperation">Operation used for blending the color (RGB) channel.</param>
	/// <param name="alphaBlendOperation">Operation used for blending the alpha (A) channel.</param>
	public RenderTargetBlendState(ColorWriteMask writeMask = ColorWriteMask.All, BlendMode sourceColorBlendMode = BlendMode.One, BlendMode destinationColorBlendMode = BlendMode.Zero, BlendMode sourceAlphaBlendMode = BlendMode.One, BlendMode destinationAlphaBlendMode = BlendMode.Zero, BlendOp colorBlendOperation = BlendOp.Add, BlendOp alphaBlendOperation = BlendOp.Add)
	{
		m_WriteMask = (byte)writeMask;
		m_SourceColorBlendMode = (byte)sourceColorBlendMode;
		m_DestinationColorBlendMode = (byte)destinationColorBlendMode;
		m_SourceAlphaBlendMode = (byte)sourceAlphaBlendMode;
		m_DestinationAlphaBlendMode = (byte)destinationAlphaBlendMode;
		m_ColorBlendOperation = (byte)colorBlendOperation;
		m_AlphaBlendOperation = (byte)alphaBlendOperation;
		m_Padding = 0;
	}
}
