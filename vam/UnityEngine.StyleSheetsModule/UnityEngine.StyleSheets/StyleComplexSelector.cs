using System;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal class StyleComplexSelector
{
	[SerializeField]
	private int m_Specificity;

	[SerializeField]
	private StyleSelector[] m_Selectors;

	[SerializeField]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal int ruleIndex;

	public int specificity
	{
		get
		{
			return m_Specificity;
		}
		internal set
		{
			m_Specificity = value;
		}
	}

	public StyleRule rule { get; internal set; }

	public bool isSimple => selectors.Length == 1;

	public StyleSelector[] selectors
	{
		get
		{
			return m_Selectors;
		}
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal set
		{
			m_Selectors = value;
		}
	}

	public override string ToString()
	{
		return string.Format("[{0}]", string.Join(", ", m_Selectors.Select((StyleSelector x) => x.ToString()).ToArray()));
	}
}
