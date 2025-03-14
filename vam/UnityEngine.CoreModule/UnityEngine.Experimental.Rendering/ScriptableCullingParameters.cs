using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Parameters controlling culling process in CullResults.</para>
/// </summary>
[UsedByNativeCode]
public struct ScriptableCullingParameters
{
	[StructLayout(LayoutKind.Sequential, Size = 160)]
	[UnsafeValueType]
	[CompilerGenerated]
	public struct _003Cm_CullingPlanes_003E__FixedBuffer4
	{
		public float FixedElementField;
	}

	[StructLayout(LayoutKind.Sequential, Size = 128)]
	[UnsafeValueType]
	[CompilerGenerated]
	public struct _003Cm_LayerFarCullDistances_003E__FixedBuffer5
	{
		public float FixedElementField;
	}

	private int m_IsOrthographic;

	private LODParameters m_LodParameters;

	private _003Cm_CullingPlanes_003E__FixedBuffer4 m_CullingPlanes;

	private int m_CullingPlaneCount;

	private int m_CullingMask;

	private long m_SceneMask;

	private _003Cm_LayerFarCullDistances_003E__FixedBuffer5 m_LayerFarCullDistances;

	private int m_LayerCull;

	private Matrix4x4 m_CullingMatrix;

	private Vector3 m_Position;

	private float m_shadowDistance;

	private int m_CullingFlags;

	private ReflectionProbeSortOptions m_ReflectionProbeSortOptions;

	private CameraProperties m_CameraProperties;

	/// <summary>
	///   <para>The view matrix generated for single-pass stereo culling.</para>
	/// </summary>
	public Matrix4x4 cullStereoView;

	/// <summary>
	///   <para>The projection matrix generated for single-pass stereo culling.</para>
	/// </summary>
	public Matrix4x4 cullStereoProj;

	/// <summary>
	///   <para>Distance between the virtual eyes.</para>
	/// </summary>
	public float cullStereoSeparation;

	private int padding2;

	/// <summary>
	///   <para>Number of culling planes to use.</para>
	/// </summary>
	public int cullingPlaneCount
	{
		get
		{
			return m_CullingPlaneCount;
		}
		set
		{
			if (value < 0 || value > 10)
			{
				throw new IndexOutOfRangeException("Invalid plane count (0 <= count <= 10)");
			}
			m_CullingPlaneCount = value;
		}
	}

	/// <summary>
	///   <para>Is the cull orthographic.</para>
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
	///   <para>LODParameters for culling.</para>
	/// </summary>
	public LODParameters lodParameters
	{
		get
		{
			return m_LodParameters;
		}
		set
		{
			m_LodParameters = value;
		}
	}

	/// <summary>
	///   <para>CullingMask used for culling.</para>
	/// </summary>
	public int cullingMask
	{
		get
		{
			return m_CullingMask;
		}
		set
		{
			m_CullingMask = value;
		}
	}

	/// <summary>
	///   <para>Scene Mask to use for the cull.</para>
	/// </summary>
	public long sceneMask
	{
		get
		{
			return m_SceneMask;
		}
		set
		{
			m_SceneMask = value;
		}
	}

	/// <summary>
	///   <para>Layers to cull.</para>
	/// </summary>
	public int layerCull
	{
		get
		{
			return m_LayerCull;
		}
		set
		{
			m_LayerCull = value;
		}
	}

	/// <summary>
	///   <para>CullingMatrix used for culling.</para>
	/// </summary>
	public Matrix4x4 cullingMatrix
	{
		get
		{
			return m_CullingMatrix;
		}
		set
		{
			m_CullingMatrix = value;
		}
	}

	/// <summary>
	///   <para>Position for the origin of th cull.</para>
	/// </summary>
	public Vector3 position
	{
		get
		{
			return m_Position;
		}
		set
		{
			m_Position = value;
		}
	}

	/// <summary>
	///   <para>Shadow distance to use for the cull.</para>
	/// </summary>
	public float shadowDistance
	{
		get
		{
			return m_shadowDistance;
		}
		set
		{
			m_shadowDistance = value;
		}
	}

	/// <summary>
	///   <para>Culling Flags for the culling.</para>
	/// </summary>
	public int cullingFlags
	{
		get
		{
			return m_CullingFlags;
		}
		set
		{
			m_CullingFlags = value;
		}
	}

	/// <summary>
	///   <para>Reflection Probe Sort options for the cull.</para>
	/// </summary>
	public ReflectionProbeSortOptions reflectionProbeSortOptions
	{
		get
		{
			return m_ReflectionProbeSortOptions;
		}
		set
		{
			m_ReflectionProbeSortOptions = value;
		}
	}

	/// <summary>
	///   <para>Camera Properties used for culling.</para>
	/// </summary>
	public CameraProperties cameraProperties
	{
		get
		{
			return m_CameraProperties;
		}
		set
		{
			m_CameraProperties = value;
		}
	}

	/// <summary>
	///   <para>Get the distance for the culling of a specific layer.</para>
	/// </summary>
	/// <param name="layerIndex"></param>
	public unsafe float GetLayerCullDistance(int layerIndex)
	{
		if (layerIndex < 0 || layerIndex >= 32)
		{
			throw new IndexOutOfRangeException("Invalid layer index");
		}
		fixed (float* ptr = &m_LayerFarCullDistances.FixedElementField)
		{
			return System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, layerIndex);
		}
	}

	/// <summary>
	///   <para>Set the distance for the culling of a specific layer.</para>
	/// </summary>
	/// <param name="layerIndex"></param>
	/// <param name="distance"></param>
	public unsafe void SetLayerCullDistance(int layerIndex, float distance)
	{
		if (layerIndex < 0 || layerIndex >= 32)
		{
			throw new IndexOutOfRangeException("Invalid layer index");
		}
		fixed (float* ptr = &m_LayerFarCullDistances.FixedElementField)
		{
			System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, layerIndex) = distance;
		}
	}

	/// <summary>
	///   <para>Fetch the culling plane at the given index.</para>
	/// </summary>
	/// <param name="index"></param>
	public unsafe Plane GetCullingPlane(int index)
	{
		if (index < 0 || index >= cullingPlaneCount || index >= 10)
		{
			throw new IndexOutOfRangeException("Invalid plane index");
		}
		fixed (float* ptr = &m_CullingPlanes.FixedElementField)
		{
			return new Plane(new Vector3(System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4), System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 1), System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 2)), System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 3));
		}
	}

	/// <summary>
	///   <para>Set the culling plane at a given index.</para>
	/// </summary>
	/// <param name="index"></param>
	/// <param name="plane"></param>
	public unsafe void SetCullingPlane(int index, Plane plane)
	{
		if (index < 0 || index >= cullingPlaneCount || index >= 10)
		{
			throw new IndexOutOfRangeException("Invalid plane index");
		}
		fixed (float* ptr = &m_CullingPlanes.FixedElementField)
		{
			System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4) = plane.normal.x;
			System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 1) = plane.normal.y;
			System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 2) = plane.normal.z;
			System.Runtime.CompilerServices.Unsafe.Add(ref *ptr, index * 4 + 3) = plane.distance;
		}
	}
}
