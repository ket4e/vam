using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Represents a display resolution.</para>
/// </summary>
[UsedByNativeCode]
public struct Resolution
{
	private int m_Width;

	private int m_Height;

	private int m_RefreshRate;

	/// <summary>
	///   <para>Resolution width in pixels.</para>
	/// </summary>
	public int width
	{
		get
		{
			return m_Width;
		}
		set
		{
			m_Width = value;
		}
	}

	/// <summary>
	///   <para>Resolution height in pixels.</para>
	/// </summary>
	public int height
	{
		get
		{
			return m_Height;
		}
		set
		{
			m_Height = value;
		}
	}

	/// <summary>
	///   <para>Resolution's vertical refresh rate in Hz.</para>
	/// </summary>
	public int refreshRate
	{
		get
		{
			return m_RefreshRate;
		}
		set
		{
			m_RefreshRate = value;
		}
	}

	/// <summary>
	///   <para>Returns a nicely formatted string of the resolution.</para>
	/// </summary>
	/// <returns>
	///   <para>A string with the format "width x height @ refreshRateHz".</para>
	/// </returns>
	public override string ToString()
	{
		return UnityString.Format("{0} x {1} @ {2}Hz", m_Width, m_Height, m_RefreshRate);
	}
}
