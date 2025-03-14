using System;
using UnityEngine.Scripting;

namespace UnityEngine.Serialization;

/// <summary>
///   <para>Use this attribute to rename a field without losing its serialized value.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
[RequiredByNativeCode]
public class FormerlySerializedAsAttribute : Attribute
{
	private string m_oldName;

	/// <summary>
	///   <para>The name of the field before the rename.</para>
	/// </summary>
	public string oldName => m_oldName;

	/// <summary>
	///   <para></para>
	/// </summary>
	/// <param name="oldName">The name of the field before renaming.</param>
	public FormerlySerializedAsAttribute(string oldName)
	{
		m_oldName = oldName;
	}
}
