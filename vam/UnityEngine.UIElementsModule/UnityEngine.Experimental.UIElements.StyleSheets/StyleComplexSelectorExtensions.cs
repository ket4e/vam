using System.Collections.Generic;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets;

internal static class StyleComplexSelectorExtensions
{
	private struct PseudoStateData
	{
		public readonly PseudoStates state;

		public readonly bool negate;

		public PseudoStateData(PseudoStates state, bool negate)
		{
			this.state = state;
			this.negate = negate;
		}
	}

	private static Dictionary<string, PseudoStateData> s_PseudoStates;

	public static void CachePseudoStateMasks(this StyleComplexSelector complexSelector)
	{
		if (complexSelector.selectors[0].pseudoStateMask != -1)
		{
			return;
		}
		if (s_PseudoStates == null)
		{
			s_PseudoStates = new Dictionary<string, PseudoStateData>();
			s_PseudoStates["active"] = new PseudoStateData(PseudoStates.Active, negate: false);
			s_PseudoStates["hover"] = new PseudoStateData(PseudoStates.Hover, negate: false);
			s_PseudoStates["checked"] = new PseudoStateData(PseudoStates.Checked, negate: false);
			s_PseudoStates["selected"] = new PseudoStateData(PseudoStates.Selected, negate: false);
			s_PseudoStates["disabled"] = new PseudoStateData(PseudoStates.Disabled, negate: false);
			s_PseudoStates["focus"] = new PseudoStateData(PseudoStates.Focus, negate: false);
			s_PseudoStates["inactive"] = new PseudoStateData(PseudoStates.Active, negate: true);
			s_PseudoStates["enabled"] = new PseudoStateData(PseudoStates.Disabled, negate: true);
		}
		int i = 0;
		for (int num = complexSelector.selectors.Length; i < num; i++)
		{
			StyleSelector styleSelector = complexSelector.selectors[i];
			StyleSelectorPart[] parts = styleSelector.parts;
			PseudoStates pseudoStates = (PseudoStates)0;
			PseudoStates pseudoStates2 = (PseudoStates)0;
			for (int j = 0; j < styleSelector.parts.Length; j++)
			{
				if (styleSelector.parts[j].type != StyleSelectorType.PseudoClass)
				{
					continue;
				}
				if (s_PseudoStates.TryGetValue(parts[j].value, out var value))
				{
					if (!value.negate)
					{
						pseudoStates |= value.state;
					}
					else
					{
						pseudoStates2 |= value.state;
					}
				}
				else
				{
					Debug.LogWarningFormat("Unknown pseudo class \"{0}\"", parts[j].value);
				}
			}
			styleSelector.pseudoStateMask = (int)pseudoStates;
			styleSelector.negatedPseudoStateMask = (int)pseudoStates2;
		}
	}
}
