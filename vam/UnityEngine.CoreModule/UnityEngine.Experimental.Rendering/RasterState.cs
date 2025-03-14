using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Values for the raster state.</para>
/// </summary>
public struct RasterState
{
	/// <summary>
	///   <para>Default values for the raster state.</para>
	/// </summary>
	public static readonly RasterState Default = new RasterState(CullMode.Back, 0, 0f, depthClip: true);

	private CullMode m_CullingMode;

	private int m_OffsetUnits;

	private float m_OffsetFactor;

	private byte m_DepthClip;

	/// <summary>
	///   <para>Controls which sides of polygons should be culled (not drawn).</para>
	/// </summary>
	public CullMode cullingMode
	{
		get
		{
			return m_CullingMode;
		}
		set
		{
			m_CullingMode = value;
		}
	}

	/// <summary>
	///   <para>Enable clipping based on depth.</para>
	/// </summary>
	public bool depthClip
	{
		get
		{
			return Convert.ToBoolean(m_DepthClip);
		}
		set
		{
			m_DepthClip = Convert.ToByte(value);
		}
	}

	/// <summary>
	///   <para>Scales the minimum resolvable depth buffer value.</para>
	/// </summary>
	public int offsetUnits
	{
		get
		{
			return m_OffsetUnits;
		}
		set
		{
			m_OffsetUnits = value;
		}
	}

	/// <summary>
	///   <para>Scales the maximum Z slope.</para>
	/// </summary>
	public float offsetFactor
	{
		get
		{
			return m_OffsetFactor;
		}
		set
		{
			m_OffsetFactor = value;
		}
	}

	public RasterState(CullMode cullingMode = CullMode.Back, int offsetUnits = 0, float offsetFactor = 0f, bool depthClip = true)
	{
		m_CullingMode = cullingMode;
		m_OffsetUnits = offsetUnits;
		m_OffsetFactor = offsetFactor;
		m_DepthClip = Convert.ToByte(depthClip);
	}
}
