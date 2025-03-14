using System;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Values for the blend state.</para>
/// </summary>
public struct BlendState
{
	private RenderTargetBlendState m_BlendState0;

	private RenderTargetBlendState m_BlendState1;

	private RenderTargetBlendState m_BlendState2;

	private RenderTargetBlendState m_BlendState3;

	private RenderTargetBlendState m_BlendState4;

	private RenderTargetBlendState m_BlendState5;

	private RenderTargetBlendState m_BlendState6;

	private RenderTargetBlendState m_BlendState7;

	private byte m_SeparateMRTBlendStates;

	private byte m_AlphaToMask;

	private short m_Padding;

	/// <summary>
	///   <para>Default values for the blend state.</para>
	/// </summary>
	public static BlendState Default => new BlendState(separateMRTBlend: false, alphaToMask: false);

	/// <summary>
	///   <para>Determines whether each render target uses a separate blend state.</para>
	/// </summary>
	public bool separateMRTBlendStates
	{
		get
		{
			return Convert.ToBoolean(m_SeparateMRTBlendStates);
		}
		set
		{
			m_SeparateMRTBlendStates = Convert.ToByte(value);
		}
	}

	/// <summary>
	///   <para>Turns on alpha-to-coverage.</para>
	/// </summary>
	public bool alphaToMask
	{
		get
		{
			return Convert.ToBoolean(m_AlphaToMask);
		}
		set
		{
			m_AlphaToMask = Convert.ToByte(value);
		}
	}

	/// <summary>
	///   <para>Blend state for render target 0.</para>
	/// </summary>
	public RenderTargetBlendState blendState0
	{
		get
		{
			return m_BlendState0;
		}
		set
		{
			m_BlendState0 = value;
		}
	}

	/// <summary>
	///   <para>Blend state for render target 1.</para>
	/// </summary>
	public RenderTargetBlendState blendState1
	{
		get
		{
			return m_BlendState1;
		}
		set
		{
			m_BlendState1 = value;
		}
	}

	/// <summary>
	///   <para>Blend state for render target 2.</para>
	/// </summary>
	public RenderTargetBlendState blendState2
	{
		get
		{
			return m_BlendState2;
		}
		set
		{
			m_BlendState2 = value;
		}
	}

	/// <summary>
	///   <para>Blend state for render target 3.</para>
	/// </summary>
	public RenderTargetBlendState blendState3
	{
		get
		{
			return m_BlendState3;
		}
		set
		{
			m_BlendState3 = value;
		}
	}

	/// <summary>
	///   <para>Blend state for render target 4.</para>
	/// </summary>
	public RenderTargetBlendState blendState4
	{
		get
		{
			return m_BlendState4;
		}
		set
		{
			m_BlendState4 = value;
		}
	}

	/// <summary>
	///   <para>Blend state for render target 5.</para>
	/// </summary>
	public RenderTargetBlendState blendState5
	{
		get
		{
			return m_BlendState5;
		}
		set
		{
			m_BlendState5 = value;
		}
	}

	/// <summary>
	///   <para>Blend state for render target 6.</para>
	/// </summary>
	public RenderTargetBlendState blendState6
	{
		get
		{
			return m_BlendState6;
		}
		set
		{
			m_BlendState6 = value;
		}
	}

	/// <summary>
	///   <para>Blend state for render target 7.</para>
	/// </summary>
	public RenderTargetBlendState blendState7
	{
		get
		{
			return m_BlendState7;
		}
		set
		{
			m_BlendState7 = value;
		}
	}

	/// <summary>
	///   <para>Creates a new blend state with the specified values.</para>
	/// </summary>
	/// <param name="separateMRTBlend">Determines whether each render target uses a separate blend state.</param>
	/// <param name="alphaToMask">Turns on alpha-to-coverage.</param>
	public BlendState(bool separateMRTBlend = false, bool alphaToMask = false)
	{
		m_BlendState0 = RenderTargetBlendState.Default;
		m_BlendState1 = RenderTargetBlendState.Default;
		m_BlendState2 = RenderTargetBlendState.Default;
		m_BlendState3 = RenderTargetBlendState.Default;
		m_BlendState4 = RenderTargetBlendState.Default;
		m_BlendState5 = RenderTargetBlendState.Default;
		m_BlendState6 = RenderTargetBlendState.Default;
		m_BlendState7 = RenderTargetBlendState.Default;
		m_SeparateMRTBlendStates = Convert.ToByte(separateMRTBlend);
		m_AlphaToMask = Convert.ToByte(alphaToMask);
		m_Padding = 0;
	}
}
