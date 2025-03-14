using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Shader pass name identifier.</para>
/// </summary>
public struct ShaderPassName
{
	private int m_NameIndex;

	internal int nameIndex => m_NameIndex;

	/// <summary>
	///   <para>Create shader pass name identifier.</para>
	/// </summary>
	/// <param name="name">Pass name.</param>
	public ShaderPassName(string name)
	{
		m_NameIndex = Init(name);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int Init(string name);
}
