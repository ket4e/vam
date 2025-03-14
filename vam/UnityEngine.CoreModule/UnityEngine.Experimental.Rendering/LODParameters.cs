using System;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>LODGroup culling parameters.</para>
/// </summary>
public struct LODParameters
{
	private int m_IsOrthographic;

	private Vector3 m_CameraPosition;

	private float m_FieldOfView;

	private float m_OrthoSize;

	private int m_CameraPixelHeight;

	/// <summary>
	///   <para>Indicates whether camera is orthographic.</para>
	/// </summary>
	public bool isOrthographic
	{
		get
		{
			return Convert.ToBoolean(m_IsOrthographic);
		}
		set
		{
			m_IsOrthographic = Convert.ToInt32(value);
		}
	}

	/// <summary>
	///   <para>Camera position.</para>
	/// </summary>
	public Vector3 cameraPosition
	{
		get
		{
			return m_CameraPosition;
		}
		set
		{
			m_CameraPosition = value;
		}
	}

	/// <summary>
	///   <para>Camera's field of view.</para>
	/// </summary>
	public float fieldOfView
	{
		get
		{
			return m_FieldOfView;
		}
		set
		{
			m_FieldOfView = value;
		}
	}

	/// <summary>
	///   <para>Orhographic camera size.</para>
	/// </summary>
	public float orthoSize
	{
		get
		{
			return m_OrthoSize;
		}
		set
		{
			m_OrthoSize = value;
		}
	}

	/// <summary>
	///   <para>Rendering view height in pixels.</para>
	/// </summary>
	public int cameraPixelHeight
	{
		get
		{
			return m_CameraPixelHeight;
		}
		set
		{
			m_CameraPixelHeight = value;
		}
	}
}
