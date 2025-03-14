using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Values for the depth state.</para>
/// </summary>
public struct DepthState
{
	private byte m_WriteEnabled;

	private sbyte m_CompareFunction;

	/// <summary>
	///   <para>Default values for the depth state.</para>
	/// </summary>
	public static DepthState Default => new DepthState(writeEnabled: true, CompareFunction.Less);

	/// <summary>
	///   <para>Controls whether pixels from this object are written to the depth buffer.</para>
	/// </summary>
	public bool writeEnabled
	{
		get
		{
			return Convert.ToBoolean(m_WriteEnabled);
		}
		set
		{
			m_WriteEnabled = Convert.ToByte(value);
		}
	}

	/// <summary>
	///   <para>How should depth testing be performed.</para>
	/// </summary>
	public CompareFunction compareFunction
	{
		get
		{
			return (CompareFunction)m_CompareFunction;
		}
		set
		{
			m_CompareFunction = (sbyte)value;
		}
	}

	/// <summary>
	///   <para>Creates a new depth state with the given values.</para>
	/// </summary>
	/// <param name="writeEnabled">Controls whether pixels from this object are written to the depth buffer.</param>
	/// <param name="compareFunction">How should depth testing be performed.</param>
	public DepthState(bool writeEnabled = true, CompareFunction compareFunction = CompareFunction.Less)
	{
		m_WriteEnabled = Convert.ToByte(writeEnabled);
		m_CompareFunction = (sbyte)compareFunction;
	}
}
