using System.Collections.Generic;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets;

internal abstract class HierarchyTraversal : IHierarchyTraversal
{
	public struct MatchResultInfo
	{
		public bool success;

		public PseudoStates triggerPseudoMask;

		public PseudoStates dependencyPseudoMask;
	}

	private List<RuleMatcher> m_ruleMatchers = new List<RuleMatcher>();

	public abstract bool ShouldSkipElement(VisualElement element);

	public abstract bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element);

	public virtual void OnBeginElementTest(VisualElement element, List<RuleMatcher> ruleMatchers)
	{
	}

	public void BeginElementTest(VisualElement element, List<RuleMatcher> ruleMatchers)
	{
		OnBeginElementTest(element, ruleMatchers);
	}

	public virtual void ProcessMatchedRules(VisualElement element)
	{
	}

	public virtual void OnProcessMatchResult(VisualElement element, ref RuleMatcher matcher, ref MatchResultInfo matchInfo)
	{
	}

	public virtual void Traverse(VisualElement element)
	{
		TraverseRecursive(element, 0, m_ruleMatchers);
		m_ruleMatchers.Clear();
	}

	public virtual void TraverseRecursive(VisualElement element, int depth, List<RuleMatcher> ruleMatchers)
	{
		if (ShouldSkipElement(element))
		{
			return;
		}
		int count = ruleMatchers.Count;
		BeginElementTest(element, ruleMatchers);
		int count2 = ruleMatchers.Count;
		for (int i = 0; i < count2; i++)
		{
			RuleMatcher matcher = ruleMatchers[i];
			if (MatchRightToLeft(element, ref matcher))
			{
				return;
			}
		}
		ProcessMatchedRules(element);
		Recurse(element, depth, ruleMatchers);
		if (ruleMatchers.Count > count)
		{
			ruleMatchers.RemoveRange(count, ruleMatchers.Count - count);
		}
	}

	private bool MatchRightToLeft(VisualElement element, ref RuleMatcher matcher)
	{
		VisualElement visualElement = element;
		int num = matcher.complexSelector.selectors.Length - 1;
		VisualElement visualElement2 = null;
		int num2 = -1;
		while (num >= 0 && visualElement != null)
		{
			MatchResultInfo matchInfo = Match(visualElement, ref matcher, num);
			OnProcessMatchResult(visualElement, ref matcher, ref matchInfo);
			if (!matchInfo.success)
			{
				if (num < matcher.complexSelector.selectors.Length - 1 && matcher.complexSelector.selectors[num + 1].previousRelationship == StyleSelectorRelationship.Descendent)
				{
					visualElement = visualElement.parent;
					continue;
				}
				if (visualElement2 != null)
				{
					visualElement = visualElement2;
					num = num2;
					continue;
				}
				break;
			}
			if (num < matcher.complexSelector.selectors.Length - 1 && matcher.complexSelector.selectors[num + 1].previousRelationship == StyleSelectorRelationship.Descendent)
			{
				visualElement2 = visualElement.parent;
				num2 = num;
			}
			if (--num < 0 && OnRuleMatchedElement(matcher, element))
			{
				return true;
			}
			visualElement = visualElement.parent;
		}
		return false;
	}

	protected virtual void Recurse(VisualElement element, int depth, List<RuleMatcher> ruleMatchers)
	{
		int num = 0;
		while (num < element.shadow.childCount)
		{
			VisualElement visualElement = element.shadow[num];
			TraverseRecursive(visualElement, depth + 1, ruleMatchers);
			if (visualElement.shadow.parent == element)
			{
				num++;
			}
		}
	}

	protected virtual bool MatchSelectorPart(VisualElement element, StyleSelector selector, StyleSelectorPart part)
	{
		bool result = true;
		switch (part.type)
		{
		case StyleSelectorType.Class:
			result = element.ClassListContains(part.value);
			break;
		case StyleSelectorType.ID:
			result = element.name == part.value;
			break;
		case StyleSelectorType.Type:
			result = element.typeName == part.value;
			break;
		case StyleSelectorType.PseudoClass:
		{
			int pseudoStates = (int)element.pseudoStates;
			result = (selector.pseudoStateMask & pseudoStates) == selector.pseudoStateMask;
			result &= (selector.negatedPseudoStateMask & ~pseudoStates) == selector.negatedPseudoStateMask;
			break;
		}
		default:
			result = false;
			break;
		case StyleSelectorType.Wildcard:
			break;
		}
		return result;
	}

	public virtual MatchResultInfo Match(VisualElement element, ref RuleMatcher matcher, int selectorIndex)
	{
		if (element == null)
		{
			return default(MatchResultInfo);
		}
		bool flag = true;
		StyleSelector styleSelector = matcher.complexSelector.selectors[selectorIndex];
		int num = styleSelector.parts.Length;
		int num2 = 0;
		int num3 = 0;
		bool flag2 = true;
		for (int i = 0; i < num; i++)
		{
			bool flag3 = MatchSelectorPart(element, styleSelector, styleSelector.parts[i]);
			if (!flag3)
			{
				if (styleSelector.parts[i].type == StyleSelectorType.PseudoClass)
				{
					num2 |= styleSelector.pseudoStateMask;
					num3 |= styleSelector.negatedPseudoStateMask;
				}
				else
				{
					flag2 = false;
				}
			}
			else if (styleSelector.parts[i].type == StyleSelectorType.PseudoClass)
			{
				num3 |= styleSelector.pseudoStateMask;
				num2 |= styleSelector.negatedPseudoStateMask;
			}
			flag = flag && flag3;
		}
		MatchResultInfo matchResultInfo = default(MatchResultInfo);
		matchResultInfo.success = flag;
		MatchResultInfo result = matchResultInfo;
		if (flag || flag2)
		{
			result.triggerPseudoMask = (PseudoStates)num2;
			result.dependencyPseudoMask = (PseudoStates)num3;
		}
		return result;
	}
}
