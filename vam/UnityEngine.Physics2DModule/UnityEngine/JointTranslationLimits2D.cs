namespace UnityEngine;

/// <summary>
///   <para>Motion limits of a Rigidbody2D object along a SliderJoint2D.</para>
/// </summary>
public struct JointTranslationLimits2D
{
	private float m_LowerTranslation;

	private float m_UpperTranslation;

	/// <summary>
	///   <para>Minimum distance the Rigidbody2D object can move from the Slider Joint's anchor.</para>
	/// </summary>
	public float min
	{
		get
		{
			return m_LowerTranslation;
		}
		set
		{
			m_LowerTranslation = value;
		}
	}

	/// <summary>
	///   <para>Maximum distance the Rigidbody2D object can move from the Slider Joint's anchor.</para>
	/// </summary>
	public float max
	{
		get
		{
			return m_UpperTranslation;
		}
		set
		{
			m_UpperTranslation = value;
		}
	}
}
