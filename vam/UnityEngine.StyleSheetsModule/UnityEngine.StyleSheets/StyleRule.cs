using System;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal class StyleRule
{
	[SerializeField]
	private StyleProperty[] m_Properties;

	[SerializeField]
	internal int line;

	public StyleProperty[] properties
	{
		get
		{
			return m_Properties;
		}
		internal set
		{
			m_Properties = value;
		}
	}
}
