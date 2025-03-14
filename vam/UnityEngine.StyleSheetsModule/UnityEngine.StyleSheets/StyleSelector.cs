using System;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal class StyleSelector
{
	[SerializeField]
	private StyleSelectorPart[] m_Parts;

	[SerializeField]
	private StyleSelectorRelationship m_PreviousRelationship;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal int pseudoStateMask = -1;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal int negatedPseudoStateMask = -1;

	public StyleSelectorPart[] parts
	{
		get
		{
			return m_Parts;
		}
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal set
		{
			m_Parts = value;
		}
	}

	public StyleSelectorRelationship previousRelationship
	{
		get
		{
			return m_PreviousRelationship;
		}
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal set
		{
			m_PreviousRelationship = value;
		}
	}

	public override string ToString()
	{
		return string.Join(", ", parts.Select((StyleSelectorPart p) => p.ToString()).ToArray());
	}
}
