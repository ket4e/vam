using System;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A single keyframe that can be injected into an animation curve.</para>
/// </summary>
[RequiredByNativeCode]
public struct Keyframe
{
	private float m_Time;

	private float m_Value;

	private float m_InTangent;

	private float m_OutTangent;

	private int m_WeightedMode;

	private float m_InWeight;

	private float m_OutWeight;

	/// <summary>
	///   <para>The time of the keyframe.</para>
	/// </summary>
	public float time
	{
		get
		{
			return m_Time;
		}
		set
		{
			m_Time = value;
		}
	}

	/// <summary>
	///   <para>The value of the curve at keyframe.</para>
	/// </summary>
	public float value
	{
		get
		{
			return m_Value;
		}
		set
		{
			m_Value = value;
		}
	}

	/// <summary>
	///   <para>Sets the incoming tangent for this key. The incoming tangent affects the slope of the curve from the previous key to this key.</para>
	/// </summary>
	public float inTangent
	{
		get
		{
			return m_InTangent;
		}
		set
		{
			m_InTangent = value;
		}
	}

	/// <summary>
	///   <para>Sets the outgoing tangent for this key. The outgoing tangent affects the slope of the curve from this key to the next key.</para>
	/// </summary>
	public float outTangent
	{
		get
		{
			return m_OutTangent;
		}
		set
		{
			m_OutTangent = value;
		}
	}

	/// <summary>
	///   <para>Sets the incoming weight for this key. The incoming weight affects the slope of the curve from the previous key to this key.</para>
	/// </summary>
	public float inWeight
	{
		get
		{
			return m_InWeight;
		}
		set
		{
			m_InWeight = value;
		}
	}

	/// <summary>
	///   <para>Sets the outgoing weight for this key. The outgoing weight affects the slope of the curve from this key to the next key.</para>
	/// </summary>
	public float outWeight
	{
		get
		{
			return m_OutWeight;
		}
		set
		{
			m_OutWeight = value;
		}
	}

	/// <summary>
	///   <para>Weighted mode for the keyframe.</para>
	/// </summary>
	public WeightedMode weightedMode
	{
		get
		{
			return (WeightedMode)m_WeightedMode;
		}
		set
		{
			m_WeightedMode = (int)value;
		}
	}

	/// <summary>
	///   <para>TangentMode is deprecated.  Use AnimationUtility.SetKeyLeftTangentMode or AnimationUtility.SetKeyRightTangentMode instead.</para>
	/// </summary>
	[Obsolete("Use AnimationUtility.SetLeftTangentMode, AnimationUtility.SetRightTangentMode, AnimationUtility.GetLeftTangentMode or AnimationUtility.GetRightTangentMode instead.")]
	public int tangentMode
	{
		get
		{
			return tangentModeInternal;
		}
		set
		{
			tangentModeInternal = value;
		}
	}

	internal int tangentModeInternal
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>Create a keyframe.</para>
	/// </summary>
	/// <param name="time"></param>
	/// <param name="value"></param>
	public Keyframe(float time, float value)
	{
		m_Time = time;
		m_Value = value;
		m_InTangent = 0f;
		m_OutTangent = 0f;
		m_WeightedMode = 0;
		m_InWeight = 0f;
		m_OutWeight = 0f;
	}

	/// <summary>
	///   <para>Create a keyframe.</para>
	/// </summary>
	/// <param name="time"></param>
	/// <param name="value"></param>
	/// <param name="inTangent"></param>
	/// <param name="outTangent"></param>
	public Keyframe(float time, float value, float inTangent, float outTangent)
	{
		m_Time = time;
		m_Value = value;
		m_InTangent = inTangent;
		m_OutTangent = outTangent;
		m_WeightedMode = 0;
		m_InWeight = 0f;
		m_OutWeight = 0f;
	}

	/// <summary>
	///   <para>Create a keyframe.</para>
	/// </summary>
	/// <param name="time"></param>
	/// <param name="value"></param>
	/// <param name="inTangent"></param>
	/// <param name="outTangent"></param>
	/// <param name="inWeight"></param>
	/// <param name="outWeight"></param>
	public Keyframe(float time, float value, float inTangent, float outTangent, float inWeight, float outWeight)
	{
		m_Time = time;
		m_Value = value;
		m_InTangent = inTangent;
		m_OutTangent = outTangent;
		m_WeightedMode = 3;
		m_InWeight = inWeight;
		m_OutWeight = outWeight;
	}
}
