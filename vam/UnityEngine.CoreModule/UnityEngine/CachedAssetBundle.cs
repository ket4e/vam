using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Data structure for downloading AssetBundles to a customized cache path. See Also:UnityWebRequestAssetBundle.GetAssetBundle for more information.</para>
/// </summary>
[UsedByNativeCode]
public struct CachedAssetBundle
{
	private string m_Name;

	private Hash128 m_Hash;

	/// <summary>
	///   <para>AssetBundle name which is used as the customized cache path.</para>
	/// </summary>
	public string name
	{
		get
		{
			return m_Name;
		}
		set
		{
			m_Name = value;
		}
	}

	/// <summary>
	///   <para>Hash128 which is used as the version of the AssetBundle.</para>
	/// </summary>
	public Hash128 hash
	{
		get
		{
			return m_Hash;
		}
		set
		{
			m_Hash = value;
		}
	}

	public CachedAssetBundle(string name, Hash128 hash)
	{
		m_Name = name;
		m_Hash = hash;
	}
}
