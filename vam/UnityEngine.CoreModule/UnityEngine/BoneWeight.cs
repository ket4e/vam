using System;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Skinning bone weights of a vertex in the mesh.</para>
/// </summary>
[Serializable]
[UsedByNativeCode]
public struct BoneWeight
{
	[SerializeField]
	private float m_Weight0;

	[SerializeField]
	private float m_Weight1;

	[SerializeField]
	private float m_Weight2;

	[SerializeField]
	private float m_Weight3;

	[SerializeField]
	private int m_BoneIndex0;

	[SerializeField]
	private int m_BoneIndex1;

	[SerializeField]
	private int m_BoneIndex2;

	[SerializeField]
	private int m_BoneIndex3;

	/// <summary>
	///   <para>Skinning weight for first bone.</para>
	/// </summary>
	public float weight0
	{
		get
		{
			return m_Weight0;
		}
		set
		{
			m_Weight0 = value;
		}
	}

	/// <summary>
	///   <para>Skinning weight for second bone.</para>
	/// </summary>
	public float weight1
	{
		get
		{
			return m_Weight1;
		}
		set
		{
			m_Weight1 = value;
		}
	}

	/// <summary>
	///   <para>Skinning weight for third bone.</para>
	/// </summary>
	public float weight2
	{
		get
		{
			return m_Weight2;
		}
		set
		{
			m_Weight2 = value;
		}
	}

	/// <summary>
	///   <para>Skinning weight for fourth bone.</para>
	/// </summary>
	public float weight3
	{
		get
		{
			return m_Weight3;
		}
		set
		{
			m_Weight3 = value;
		}
	}

	/// <summary>
	///   <para>Index of first bone.</para>
	/// </summary>
	public int boneIndex0
	{
		get
		{
			return m_BoneIndex0;
		}
		set
		{
			m_BoneIndex0 = value;
		}
	}

	/// <summary>
	///   <para>Index of second bone.</para>
	/// </summary>
	public int boneIndex1
	{
		get
		{
			return m_BoneIndex1;
		}
		set
		{
			m_BoneIndex1 = value;
		}
	}

	/// <summary>
	///   <para>Index of third bone.</para>
	/// </summary>
	public int boneIndex2
	{
		get
		{
			return m_BoneIndex2;
		}
		set
		{
			m_BoneIndex2 = value;
		}
	}

	/// <summary>
	///   <para>Index of fourth bone.</para>
	/// </summary>
	public int boneIndex3
	{
		get
		{
			return m_BoneIndex3;
		}
		set
		{
			m_BoneIndex3 = value;
		}
	}

	public override int GetHashCode()
	{
		return boneIndex0.GetHashCode() ^ (boneIndex1.GetHashCode() << 2) ^ (boneIndex2.GetHashCode() >> 2) ^ (boneIndex3.GetHashCode() >> 1) ^ (weight0.GetHashCode() << 5) ^ (weight1.GetHashCode() << 4) ^ (weight2.GetHashCode() >> 4) ^ (weight3.GetHashCode() >> 3);
	}

	public override bool Equals(object other)
	{
		if (!(other is BoneWeight boneWeight))
		{
			return false;
		}
		return boneIndex0.Equals(boneWeight.boneIndex0) && boneIndex1.Equals(boneWeight.boneIndex1) && boneIndex2.Equals(boneWeight.boneIndex2) && boneIndex3.Equals(boneWeight.boneIndex3) && new Vector4(weight0, weight1, weight2, weight3).Equals(new Vector4(boneWeight.weight0, boneWeight.weight1, boneWeight.weight2, boneWeight.weight3));
	}

	public static bool operator ==(BoneWeight lhs, BoneWeight rhs)
	{
		return lhs.boneIndex0 == rhs.boneIndex0 && lhs.boneIndex1 == rhs.boneIndex1 && lhs.boneIndex2 == rhs.boneIndex2 && lhs.boneIndex3 == rhs.boneIndex3 && new Vector4(lhs.weight0, lhs.weight1, lhs.weight2, lhs.weight3) == new Vector4(rhs.weight0, rhs.weight1, rhs.weight2, rhs.weight3);
	}

	public static bool operator !=(BoneWeight lhs, BoneWeight rhs)
	{
		return !(lhs == rhs);
	}
}
