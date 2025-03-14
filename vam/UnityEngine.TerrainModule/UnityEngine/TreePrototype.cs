using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Simple class that contains a pointer to a tree prototype.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[UsedByNativeCode]
public sealed class TreePrototype
{
	internal GameObject m_Prefab;

	internal float m_BendFactor;

	/// <summary>
	///   <para>Retrieves the actual GameObject used by the tree.</para>
	/// </summary>
	public GameObject prefab
	{
		get
		{
			return m_Prefab;
		}
		set
		{
			m_Prefab = value;
		}
	}

	/// <summary>
	///   <para>Bend factor of the tree prototype.</para>
	/// </summary>
	public float bendFactor
	{
		get
		{
			return m_BendFactor;
		}
		set
		{
			m_BendFactor = value;
		}
	}
}
