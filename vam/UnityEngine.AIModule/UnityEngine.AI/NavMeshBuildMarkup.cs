using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.AI;

/// <summary>
///   <para>The NavMesh build markup allows you to control how certain objects are treated during the NavMesh build process, specifically when collecting sources for building.</para>
/// </summary>
public struct NavMeshBuildMarkup
{
	private int m_OverrideArea;

	private int m_Area;

	private int m_IgnoreFromBuild;

	private int m_InstanceID;

	/// <summary>
	///   <para>Use this to specify whether the area type of the GameObject and its children should be overridden by the area type specified in this struct.</para>
	/// </summary>
	public bool overrideArea
	{
		get
		{
			return m_OverrideArea != 0;
		}
		set
		{
			m_OverrideArea = (value ? 1 : 0);
		}
	}

	/// <summary>
	///   <para>The area type to use when override area is enabled.</para>
	/// </summary>
	public int area
	{
		get
		{
			return m_Area;
		}
		set
		{
			m_Area = value;
		}
	}

	/// <summary>
	///   <para>Use this to specify whether the GameObject and its children should be ignored.</para>
	/// </summary>
	public bool ignoreFromBuild
	{
		get
		{
			return m_IgnoreFromBuild != 0;
		}
		set
		{
			m_IgnoreFromBuild = (value ? 1 : 0);
		}
	}

	/// <summary>
	///   <para>Use this to specify which GameObject (including the GameObject’s children) the markup should be applied to.</para>
	/// </summary>
	public Transform root
	{
		get
		{
			return InternalGetRootGO(m_InstanceID);
		}
		set
		{
			m_InstanceID = ((value != null) ? value.GetInstanceID() : 0);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern Transform InternalGetRootGO(int instanceID);
}
