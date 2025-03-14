using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Values for the stencil state.</para>
/// </summary>
public struct StencilState
{
	private byte m_Enabled;

	private byte m_ReadMask;

	private byte m_WriteMask;

	private byte m_Padding;

	private byte m_CompareFunctionFront;

	private byte m_PassOperationFront;

	private byte m_FailOperationFront;

	private byte m_ZFailOperationFront;

	private byte m_CompareFunctionBack;

	private byte m_PassOperationBack;

	private byte m_FailOperationBack;

	private byte m_ZFailOperationBack;

	/// <summary>
	///   <para>Default values for the stencil state.</para>
	/// </summary>
	public static StencilState Default => new StencilState(enabled: false, byte.MaxValue, byte.MaxValue, CompareFunction.Always, StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);

	/// <summary>
	///   <para>Controls whether the stencil buffer is enabled.</para>
	/// </summary>
	public bool enabled
	{
		get
		{
			return Convert.ToBoolean(m_Enabled);
		}
		set
		{
			m_Enabled = Convert.ToByte(value);
		}
	}

	/// <summary>
	///   <para>An 8 bit mask as an 0–255 integer, used when comparing the reference value with the contents of the buffer.</para>
	/// </summary>
	public byte readMask
	{
		get
		{
			return m_ReadMask;
		}
		set
		{
			m_ReadMask = value;
		}
	}

	/// <summary>
	///   <para>An 8 bit mask as an 0–255 integer, used when writing to the buffer.</para>
	/// </summary>
	public byte writeMask
	{
		get
		{
			return m_WriteMask;
		}
		set
		{
			m_WriteMask = value;
		}
	}

	/// <summary>
	///   <para>The function used to compare the reference value to the current contents of the buffer.</para>
	/// </summary>
	public CompareFunction compareFunction
	{
		set
		{
			compareFunctionFront = value;
			compareFunctionBack = value;
		}
	}

	/// <summary>
	///   <para>What to do with the contents of the buffer if the stencil test (and the depth test) passes.</para>
	/// </summary>
	public StencilOp passOperation
	{
		set
		{
			passOperationFront = value;
			passOperationBack = value;
		}
	}

	/// <summary>
	///   <para>What to do with the contents of the buffer if the stencil test fails.</para>
	/// </summary>
	public StencilOp failOperation
	{
		set
		{
			failOperationFront = value;
			failOperationBack = value;
		}
	}

	/// <summary>
	///   <para>What to do with the contents of the buffer if the stencil test passes, but the depth test fails.</para>
	/// </summary>
	public StencilOp zFailOperation
	{
		set
		{
			zFailOperationFront = value;
			zFailOperationBack = value;
		}
	}

	/// <summary>
	///   <para>The function used to compare the reference value to the current contents of the buffer for front-facing geometry.</para>
	/// </summary>
	public CompareFunction compareFunctionFront
	{
		get
		{
			return (CompareFunction)m_CompareFunctionFront;
		}
		set
		{
			m_CompareFunctionFront = (byte)value;
		}
	}

	/// <summary>
	///   <para>What to do with the contents of the buffer if the stencil test (and the depth test) passes for front-facing geometry.</para>
	/// </summary>
	public StencilOp passOperationFront
	{
		get
		{
			return (StencilOp)m_PassOperationFront;
		}
		set
		{
			m_PassOperationFront = (byte)value;
		}
	}

	/// <summary>
	///   <para>What to do with the contents of the buffer if the stencil test fails for front-facing geometry.</para>
	/// </summary>
	public StencilOp failOperationFront
	{
		get
		{
			return (StencilOp)m_FailOperationFront;
		}
		set
		{
			m_FailOperationFront = (byte)value;
		}
	}

	/// <summary>
	///   <para>What to do with the contents of the buffer if the stencil test passes, but the depth test fails for front-facing geometry.</para>
	/// </summary>
	public StencilOp zFailOperationFront
	{
		get
		{
			return (StencilOp)m_ZFailOperationFront;
		}
		set
		{
			m_ZFailOperationFront = (byte)value;
		}
	}

	/// <summary>
	///   <para>The function used to compare the reference value to the current contents of the buffer for back-facing geometry.</para>
	/// </summary>
	public CompareFunction compareFunctionBack
	{
		get
		{
			return (CompareFunction)m_CompareFunctionBack;
		}
		set
		{
			m_CompareFunctionBack = (byte)value;
		}
	}

	/// <summary>
	///   <para>What to do with the contents of the buffer if the stencil test (and the depth test) passes for back-facing geometry.</para>
	/// </summary>
	public StencilOp passOperationBack
	{
		get
		{
			return (StencilOp)m_PassOperationBack;
		}
		set
		{
			m_PassOperationBack = (byte)value;
		}
	}

	/// <summary>
	///   <para>What to do with the contents of the buffer if the stencil test fails for back-facing geometry.</para>
	/// </summary>
	public StencilOp failOperationBack
	{
		get
		{
			return (StencilOp)m_FailOperationBack;
		}
		set
		{
			m_FailOperationBack = (byte)value;
		}
	}

	/// <summary>
	///   <para>What to do with the contents of the buffer if the stencil test passes, but the depth test fails for back-facing geometry.</para>
	/// </summary>
	public StencilOp zFailOperationBack
	{
		get
		{
			return (StencilOp)m_ZFailOperationBack;
		}
		set
		{
			m_ZFailOperationBack = (byte)value;
		}
	}

	/// <summary>
	///   <para>Creates a new stencil state with the given values.</para>
	/// </summary>
	/// <param name="readMask">An 8 bit mask as an 0–255 integer, used when comparing the reference value with the contents of the buffer.</param>
	/// <param name="writeMask">An 8 bit mask as an 0–255 integer, used when writing to the buffer.</param>
	/// <param name="enabled">Controls whether the stencil buffer is enabled.</param>
	/// <param name="compareFunctionFront">The function used to compare the reference value to the current contents of the buffer for front-facing geometry.</param>
	/// <param name="passOperationFront">What to do with the contents of the buffer if the stencil test (and the depth test) passes for front-facing geometry.</param>
	/// <param name="failOperationFront">What to do with the contents of the buffer if the stencil test fails for front-facing geometry.</param>
	/// <param name="zFailOperationFront">What to do with the contents of the buffer if the stencil test passes, but the depth test fails for front-facing geometry.</param>
	/// <param name="compareFunctionBack">The function used to compare the reference value to the current contents of the buffer for back-facing geometry.</param>
	/// <param name="passOperationBack">What to do with the contents of the buffer if the stencil test (and the depth test) passes for back-facing geometry.</param>
	/// <param name="failOperationBack">What to do with the contents of the buffer if the stencil test fails for back-facing geometry.</param>
	/// <param name="zFailOperationBack">What to do with the contents of the buffer if the stencil test passes, but the depth test fails for back-facing geometry.</param>
	/// <param name="compareFunction">The function used to compare the reference value to the current contents of the buffer.</param>
	/// <param name="passOperation">What to do with the contents of the buffer if the stencil test (and the depth test) passes.</param>
	/// <param name="failOperation">What to do with the contents of the buffer if the stencil test fails.</param>
	/// <param name="zFailOperation">What to do with the contents of the buffer if the stencil test passes, but the depth test.</param>
	public StencilState(bool enabled = false, byte readMask = byte.MaxValue, byte writeMask = byte.MaxValue, CompareFunction compareFunction = CompareFunction.Always, StencilOp passOperation = StencilOp.Keep, StencilOp failOperation = StencilOp.Keep, StencilOp zFailOperation = StencilOp.Keep)
		: this(enabled, readMask, writeMask, compareFunction, passOperation, failOperation, zFailOperation, compareFunction, passOperation, failOperation, zFailOperation)
	{
	}

	/// <summary>
	///   <para>Creates a new stencil state with the given values.</para>
	/// </summary>
	/// <param name="readMask">An 8 bit mask as an 0–255 integer, used when comparing the reference value with the contents of the buffer.</param>
	/// <param name="writeMask">An 8 bit mask as an 0–255 integer, used when writing to the buffer.</param>
	/// <param name="enabled">Controls whether the stencil buffer is enabled.</param>
	/// <param name="compareFunctionFront">The function used to compare the reference value to the current contents of the buffer for front-facing geometry.</param>
	/// <param name="passOperationFront">What to do with the contents of the buffer if the stencil test (and the depth test) passes for front-facing geometry.</param>
	/// <param name="failOperationFront">What to do with the contents of the buffer if the stencil test fails for front-facing geometry.</param>
	/// <param name="zFailOperationFront">What to do with the contents of the buffer if the stencil test passes, but the depth test fails for front-facing geometry.</param>
	/// <param name="compareFunctionBack">The function used to compare the reference value to the current contents of the buffer for back-facing geometry.</param>
	/// <param name="passOperationBack">What to do with the contents of the buffer if the stencil test (and the depth test) passes for back-facing geometry.</param>
	/// <param name="failOperationBack">What to do with the contents of the buffer if the stencil test fails for back-facing geometry.</param>
	/// <param name="zFailOperationBack">What to do with the contents of the buffer if the stencil test passes, but the depth test fails for back-facing geometry.</param>
	/// <param name="compareFunction">The function used to compare the reference value to the current contents of the buffer.</param>
	/// <param name="passOperation">What to do with the contents of the buffer if the stencil test (and the depth test) passes.</param>
	/// <param name="failOperation">What to do with the contents of the buffer if the stencil test fails.</param>
	/// <param name="zFailOperation">What to do with the contents of the buffer if the stencil test passes, but the depth test.</param>
	public StencilState(bool enabled, byte readMask, byte writeMask, CompareFunction compareFunctionFront, StencilOp passOperationFront, StencilOp failOperationFront, StencilOp zFailOperationFront, CompareFunction compareFunctionBack, StencilOp passOperationBack, StencilOp failOperationBack, StencilOp zFailOperationBack)
	{
		m_Enabled = Convert.ToByte(enabled);
		m_ReadMask = readMask;
		m_WriteMask = writeMask;
		m_Padding = 0;
		m_CompareFunctionFront = (byte)compareFunctionFront;
		m_PassOperationFront = (byte)passOperationFront;
		m_FailOperationFront = (byte)failOperationFront;
		m_ZFailOperationFront = (byte)zFailOperationFront;
		m_CompareFunctionBack = (byte)compareFunctionBack;
		m_PassOperationBack = (byte)passOperationBack;
		m_FailOperationBack = (byte)failOperationBack;
		m_ZFailOperationBack = (byte)zFailOperationBack;
	}
}
