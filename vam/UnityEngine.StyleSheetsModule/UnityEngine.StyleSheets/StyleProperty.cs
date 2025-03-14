using System;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal class StyleProperty
{
	[SerializeField]
	private string m_Name;

	[SerializeField]
	private StyleValueHandle[] m_Values;

	public string name
	{
		get
		{
			return m_Name;
		}
		internal set
		{
			m_Name = value;
		}
	}

	public StyleValueHandle[] values
	{
		get
		{
			return m_Values;
		}
		internal set
		{
			m_Values = value;
		}
	}
}
