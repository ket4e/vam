#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets;

internal class StyleContext
{
	internal struct RuleRef
	{
		public StyleComplexSelector selector;

		public StyleSheet sheet;
	}

	internal class StyleContextHierarchyTraversal : HierarchyTraversal
	{
		private List<RuleRef> m_MatchedRules = new List<RuleRef>(0);

		private long m_MatchingRulesHash;

		public float currentPixelsPerPoint { get; set; }

		public override bool ShouldSkipElement(VisualElement element)
		{
			return !element.IsDirty(ChangeType.Styles) && !element.IsDirty(ChangeType.StylesPath);
		}

		public override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
		{
			StyleRule rule = matcher.complexSelector.rule;
			int specificity = matcher.complexSelector.specificity;
			m_MatchingRulesHash = (m_MatchingRulesHash * 397) ^ rule.GetHashCode();
			m_MatchingRulesHash = (m_MatchingRulesHash * 397) ^ specificity;
			m_MatchedRules.Add(new RuleRef
			{
				selector = matcher.complexSelector,
				sheet = matcher.sheet
			});
			return false;
		}

		public override void OnBeginElementTest(VisualElement element, List<RuleMatcher> ruleMatchers)
		{
			if (element.IsDirty(ChangeType.Styles))
			{
				element.triggerPseudoMask = (PseudoStates)0;
				element.dependencyPseudoMask = (PseudoStates)0;
			}
			if (element != null && element.styleSheets != null)
			{
				for (int i = 0; i < element.styleSheets.Count; i++)
				{
					StyleSheet styleSheet = element.styleSheets[i];
					StyleComplexSelector[] complexSelectors = styleSheet.complexSelectors;
					int val = ruleMatchers.Count + complexSelectors.Length;
					ruleMatchers.Capacity = Math.Max(ruleMatchers.Capacity, val);
					foreach (StyleComplexSelector complexSelector in complexSelectors)
					{
						ruleMatchers.Add(new RuleMatcher
						{
							sheet = styleSheet,
							complexSelector = complexSelector
						});
					}
				}
			}
			m_MatchedRules.Clear();
			string fullTypeName = element.fullTypeName;
			long num = fullTypeName.GetHashCode();
			m_MatchingRulesHash = (num * 397) ^ currentPixelsPerPoint.GetHashCode();
		}

		public override void OnProcessMatchResult(VisualElement element, ref RuleMatcher matcher, ref MatchResultInfo matchInfo)
		{
			element.triggerPseudoMask |= matchInfo.triggerPseudoMask;
			element.dependencyPseudoMask |= matchInfo.dependencyPseudoMask;
		}

		public override void ProcessMatchedRules(VisualElement element)
		{
			if (s_StyleCache.TryGetValue(m_MatchingRulesHash, out var value))
			{
				element.SetSharedStyles(value);
				return;
			}
			value = new VisualElementStylesData(isShared: true);
			int i = 0;
			for (int count = m_MatchedRules.Count; i < count; i++)
			{
				RuleRef ruleRef = m_MatchedRules[i];
				StylePropertyID[] propertyIDs = StyleSheetCache.GetPropertyIDs(ruleRef.sheet, ruleRef.selector.ruleIndex);
				value.ApplyRule(ruleRef.sheet, ruleRef.selector.specificity, ruleRef.selector.rule, propertyIDs);
			}
			s_StyleCache[m_MatchingRulesHash] = value;
			element.SetSharedStyles(value);
		}
	}

	private VisualElement m_VisualTree;

	private static Dictionary<long, VisualElementStylesData> s_StyleCache = new Dictionary<long, VisualElementStylesData>();

	internal static StyleContextHierarchyTraversal styleContextHierarchyTraversal = new StyleContextHierarchyTraversal();

	public float currentPixelsPerPoint { get; set; } = 1f;


	public StyleContext(VisualElement tree)
	{
		m_VisualTree = tree;
	}

	public void DirtyStyleSheets()
	{
		PropagateDirtyStyleSheets(m_VisualTree);
	}

	public void ApplyStyles()
	{
		Debug.Assert(m_VisualTree.panel != null);
		styleContextHierarchyTraversal.currentPixelsPerPoint = currentPixelsPerPoint;
		styleContextHierarchyTraversal.Traverse(m_VisualTree);
	}

	private static void PropagateDirtyStyleSheets(VisualElement element)
	{
		if (element == null)
		{
			return;
		}
		if (element.styleSheets != null)
		{
			element.LoadStyleSheetsFromPaths();
		}
		foreach (VisualElement item in element.shadow.Children())
		{
			PropagateDirtyStyleSheets(item);
		}
	}

	public static void ClearStyleCache()
	{
		s_StyleCache.Clear();
	}
}
