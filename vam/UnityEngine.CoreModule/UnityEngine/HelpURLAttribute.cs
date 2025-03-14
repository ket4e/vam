using System;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Provide a custom documentation URL for a class.</para>
/// </summary>
[UsedByNativeCode]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class HelpURLAttribute : Attribute
{
	internal readonly string m_Url;

	/// <summary>
	///   <para>The documentation URL specified for this class.</para>
	/// </summary>
	public string URL => m_Url;

	/// <summary>
	///   <para>Initialize the HelpURL attribute with a documentation url.</para>
	/// </summary>
	/// <param name="url">The custom documentation URL for this class.</param>
	public HelpURLAttribute(string url)
	{
		m_Url = url;
	}
}
