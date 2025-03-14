using System;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>UQuery is a set of extension methods allowing you to select individual or collection of visualElements inside a complex hierarchy.</para>
/// </summary>
public static class UQuery
{
	internal interface IVisualPredicateWrapper
	{
		bool Predicate(object e);
	}

	internal class IsOfType<T> : IVisualPredicateWrapper where T : VisualElement
	{
		public static IsOfType<T> s_Instance = new IsOfType<T>();

		public bool Predicate(object e)
		{
			return e is T;
		}
	}

	internal class PredicateWrapper<T> : IVisualPredicateWrapper where T : VisualElement
	{
		private Func<T, bool> predicate;

		public PredicateWrapper(Func<T, bool> p)
		{
			predicate = p;
		}

		public bool Predicate(object e)
		{
			if (e is T arg)
			{
				return predicate(arg);
			}
			return false;
		}
	}

	private abstract class UQueryMatcher : HierarchyTraversal
	{
		internal List<RuleMatcher> m_Matchers;

		public override bool ShouldSkipElement(VisualElement element)
		{
			return false;
		}

		protected override bool MatchSelectorPart(VisualElement element, StyleSelector selector, StyleSelectorPart part)
		{
			if (part.type == StyleSelectorType.Predicate)
			{
				return part.tempData is IVisualPredicateWrapper visualPredicateWrapper && visualPredicateWrapper.Predicate(element);
			}
			return base.MatchSelectorPart(element, selector, part);
		}

		public override void Traverse(VisualElement element)
		{
			TraverseRecursive(element, 0, new List<RuleMatcher>(m_Matchers));
		}

		public virtual void Run(VisualElement root, List<RuleMatcher> matchers)
		{
			m_Matchers = matchers;
			Traverse(root);
		}
	}

	private abstract class SingleQueryMatcher : UQueryMatcher
	{
		public VisualElement match { get; set; }

		public override void Run(VisualElement root, List<RuleMatcher> matchers)
		{
			match = null;
			base.Run(root, matchers);
		}
	}

	private class FirstQueryMatcher : SingleQueryMatcher
	{
		public override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
		{
			if (base.match == null)
			{
				base.match = element;
			}
			return true;
		}
	}

	private class LastQueryMatcher : SingleQueryMatcher
	{
		public override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
		{
			base.match = element;
			return false;
		}
	}

	private class IndexQueryMatcher : SingleQueryMatcher
	{
		private int matchCount = -1;

		private int _matchIndex;

		public int matchIndex
		{
			get
			{
				return _matchIndex;
			}
			set
			{
				matchCount = -1;
				_matchIndex = value;
			}
		}

		public override void Run(VisualElement root, List<RuleMatcher> matchers)
		{
			matchCount = -1;
			base.Run(root, matchers);
		}

		public override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
		{
			matchCount++;
			if (matchCount == _matchIndex)
			{
				base.match = element;
			}
			return matchCount >= _matchIndex;
		}
	}

	/// <summary>
	///   <para>Utility Object that contructs a set of selection rules to be ran on a root visual element.</para>
	/// </summary>
	public struct QueryBuilder<T> where T : VisualElement
	{
		private List<StyleSelector> m_StyleSelectors;

		private List<StyleSelectorPart> m_Parts;

		private VisualElement m_Element;

		private List<RuleMatcher> m_Matchers;

		private StyleSelectorRelationship m_Relationship;

		private int pseudoStatesMask;

		private int negatedPseudoStatesMask;

		private List<StyleSelector> styleSelectors => m_StyleSelectors ?? (m_StyleSelectors = new List<StyleSelector>());

		private List<StyleSelectorPart> parts => m_Parts ?? (m_Parts = new List<StyleSelectorPart>());

		public QueryBuilder(VisualElement visualElement)
		{
			this = default(QueryBuilder<T>);
			m_Element = visualElement;
			m_Parts = null;
			m_StyleSelectors = null;
			m_Relationship = StyleSelectorRelationship.None;
			m_Matchers = new List<RuleMatcher>();
			pseudoStatesMask = (negatedPseudoStatesMask = 0);
		}

		public QueryBuilder<T> Class(string classname)
		{
			AddClass(classname);
			return this;
		}

		public QueryBuilder<T> Name(string id)
		{
			AddName(id);
			return this;
		}

		public QueryBuilder<T2> Descendents<T2>(string name = null, params string[] classNames) where T2 : VisualElement
		{
			FinishCurrentSelector();
			AddType<T2>();
			AddName(name);
			AddClasses(classNames);
			return AddRelationship<T2>(StyleSelectorRelationship.Descendent);
		}

		public QueryBuilder<T2> Descendents<T2>(string name = null, string classname = null) where T2 : VisualElement
		{
			FinishCurrentSelector();
			AddType<T2>();
			AddName(name);
			AddClass(classname);
			return AddRelationship<T2>(StyleSelectorRelationship.Descendent);
		}

		public QueryBuilder<T2> Children<T2>(string name = null, params string[] classes) where T2 : VisualElement
		{
			FinishCurrentSelector();
			AddType<T2>();
			AddName(name);
			AddClasses(classes);
			return AddRelationship<T2>(StyleSelectorRelationship.Child);
		}

		public QueryBuilder<T2> Children<T2>(string name = null, string className = null) where T2 : VisualElement
		{
			FinishCurrentSelector();
			AddType<T2>();
			AddName(name);
			AddClass(className);
			return AddRelationship<T2>(StyleSelectorRelationship.Child);
		}

		public QueryBuilder<T2> OfType<T2>(string name = null, params string[] classes) where T2 : VisualElement
		{
			AddType<T2>();
			AddName(name);
			AddClasses(classes);
			return AddRelationship<T2>(StyleSelectorRelationship.None);
		}

		public QueryBuilder<T2> OfType<T2>(string name = null, string className = null) where T2 : VisualElement
		{
			AddType<T2>();
			AddName(name);
			AddClass(className);
			return AddRelationship<T2>(StyleSelectorRelationship.None);
		}

		public QueryBuilder<T> Where(Func<T, bool> selectorPredicate)
		{
			parts.Add(StyleSelectorPart.CreatePredicate(new PredicateWrapper<T>(selectorPredicate)));
			return this;
		}

		private void AddClass(string c)
		{
			if (c != null)
			{
				parts.Add(StyleSelectorPart.CreateClass(c));
			}
		}

		private void AddClasses(params string[] classes)
		{
			if (classes != null)
			{
				for (int i = 0; i < classes.Length; i++)
				{
					AddClass(classes[i]);
				}
			}
		}

		private void AddName(string id)
		{
			if (id != null)
			{
				parts.Add(StyleSelectorPart.CreateId(id));
			}
		}

		private void AddType<T2>() where T2 : VisualElement
		{
			if (typeof(T2) != typeof(VisualElement))
			{
				parts.Add(StyleSelectorPart.CreatePredicate(IsOfType<T2>.s_Instance));
			}
		}

		private QueryBuilder<T> AddPseudoState(PseudoStates s)
		{
			pseudoStatesMask |= (int)s;
			return this;
		}

		private QueryBuilder<T> AddNegativePseudoState(PseudoStates s)
		{
			negatedPseudoStatesMask |= (int)s;
			return this;
		}

		public QueryBuilder<T> Active()
		{
			return AddPseudoState(PseudoStates.Active);
		}

		public QueryBuilder<T> NotActive()
		{
			return AddNegativePseudoState(PseudoStates.Active);
		}

		public QueryBuilder<T> Visible()
		{
			return AddNegativePseudoState(PseudoStates.Invisible);
		}

		public QueryBuilder<T> NotVisible()
		{
			return AddPseudoState(PseudoStates.Invisible);
		}

		public QueryBuilder<T> Hovered()
		{
			return AddPseudoState(PseudoStates.Hover);
		}

		public QueryBuilder<T> NotHovered()
		{
			return AddNegativePseudoState(PseudoStates.Hover);
		}

		public QueryBuilder<T> Checked()
		{
			return AddPseudoState(PseudoStates.Checked);
		}

		public QueryBuilder<T> NotChecked()
		{
			return AddNegativePseudoState(PseudoStates.Checked);
		}

		public QueryBuilder<T> Selected()
		{
			return AddPseudoState(PseudoStates.Selected);
		}

		public QueryBuilder<T> NotSelected()
		{
			return AddNegativePseudoState(PseudoStates.Selected);
		}

		public QueryBuilder<T> Enabled()
		{
			return AddNegativePseudoState(PseudoStates.Disabled);
		}

		public QueryBuilder<T> NotEnabled()
		{
			return AddPseudoState(PseudoStates.Disabled);
		}

		public QueryBuilder<T> Focused()
		{
			return AddPseudoState(PseudoStates.Focus);
		}

		public QueryBuilder<T> NotFocused()
		{
			return AddNegativePseudoState(PseudoStates.Focus);
		}

		private QueryBuilder<T2> AddRelationship<T2>(StyleSelectorRelationship relationship) where T2 : VisualElement
		{
			QueryBuilder<T2> result = new QueryBuilder<T2>(m_Element);
			result.m_Matchers = m_Matchers;
			result.m_Parts = m_Parts;
			result.m_StyleSelectors = m_StyleSelectors;
			result.m_Relationship = ((relationship != 0) ? relationship : m_Relationship);
			result.pseudoStatesMask = pseudoStatesMask;
			result.negatedPseudoStatesMask = negatedPseudoStatesMask;
			return result;
		}

		private void AddPseudoStatesRuleIfNecessasy()
		{
			if (pseudoStatesMask != 0 || negatedPseudoStatesMask != 0)
			{
				parts.Add(new StyleSelectorPart
				{
					type = StyleSelectorType.PseudoClass
				});
			}
		}

		private void FinishSelector()
		{
			FinishCurrentSelector();
			if (styleSelectors.Count > 0)
			{
				StyleComplexSelector styleComplexSelector = new StyleComplexSelector();
				styleComplexSelector.selectors = styleSelectors.ToArray();
				styleSelectors.Clear();
				m_Matchers.Add(new RuleMatcher
				{
					complexSelector = styleComplexSelector
				});
			}
		}

		private bool CurrentSelectorEmpty()
		{
			return parts.Count == 0 && m_Relationship == StyleSelectorRelationship.None && pseudoStatesMask == 0 && negatedPseudoStatesMask == 0;
		}

		private void FinishCurrentSelector()
		{
			if (!CurrentSelectorEmpty())
			{
				StyleSelector styleSelector = new StyleSelector();
				styleSelector.previousRelationship = m_Relationship;
				AddPseudoStatesRuleIfNecessasy();
				styleSelector.parts = m_Parts.ToArray();
				styleSelector.pseudoStateMask = pseudoStatesMask;
				styleSelector.negatedPseudoStateMask = negatedPseudoStatesMask;
				styleSelectors.Add(styleSelector);
				m_Parts.Clear();
				pseudoStatesMask = (negatedPseudoStatesMask = 0);
			}
		}

		public QueryState<T> Build()
		{
			FinishSelector();
			return new QueryState<T>(m_Element, m_Matchers);
		}

		public static implicit operator T(QueryBuilder<T> s)
		{
			return s.First();
		}

		public T First()
		{
			return Build().First();
		}

		public T Last()
		{
			return Build().Last();
		}

		public List<T> ToList()
		{
			return Build().ToList();
		}

		public void ToList(List<T> results)
		{
			Build().ToList(results);
		}

		public T AtIndex(int index)
		{
			return Build().AtIndex(index);
		}

		public void ForEach<T2>(List<T2> result, Func<T, T2> funcCall)
		{
			Build().ForEach(result, funcCall);
		}

		public List<T2> ForEach<T2>(Func<T, T2> funcCall)
		{
			return Build().ForEach(funcCall);
		}

		public void ForEach(Action<T> funcCall)
		{
			Build().ForEach(funcCall);
		}
	}

	/// <summary>
	///   <para>Query object containing all the selection rules. Can be saved and rerun later without re-allocating memory.</para>
	/// </summary>
	public struct QueryState<T> where T : VisualElement
	{
		private class ListQueryMatcher : UQueryMatcher
		{
			public List<T> matches { get; set; }

			public override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
			{
				matches.Add(element as T);
				return false;
			}

			public void Reset()
			{
				matches = null;
			}
		}

		private class ActionQueryMatcher : UQueryMatcher
		{
			internal Action<T> callBack { get; set; }

			public override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
			{
				if (element is T obj)
				{
					callBack(obj);
				}
				return false;
			}
		}

		private class DelegateQueryMatcher<TReturnType> : UQueryMatcher
		{
			public static DelegateQueryMatcher<TReturnType> s_Instance = new DelegateQueryMatcher<TReturnType>();

			public Func<T, TReturnType> callBack { get; set; }

			public List<TReturnType> result { get; set; }

			public override bool OnRuleMatchedElement(RuleMatcher matcher, VisualElement element)
			{
				if (element is T arg)
				{
					result.Add(callBack(arg));
				}
				return false;
			}
		}

		private readonly VisualElement m_Element;

		private readonly List<RuleMatcher> m_Matchers;

		private static readonly ListQueryMatcher s_List = new ListQueryMatcher();

		private static ActionQueryMatcher s_Action = new ActionQueryMatcher();

		internal QueryState(VisualElement element, List<RuleMatcher> matchers)
		{
			m_Element = element;
			m_Matchers = matchers;
		}

		public QueryState<T> RebuildOn(VisualElement element)
		{
			return new QueryState<T>(element, m_Matchers);
		}

		public T First()
		{
			s_First.Run(m_Element, m_Matchers);
			T result = s_First.match as T;
			s_First.match = null;
			return result;
		}

		public T Last()
		{
			s_Last.Run(m_Element, m_Matchers);
			T result = s_Last.match as T;
			s_Last.match = null;
			return result;
		}

		public void ToList(List<T> results)
		{
			s_List.matches = results;
			s_List.Run(m_Element, m_Matchers);
			s_List.Reset();
		}

		public List<T> ToList()
		{
			List<T> list = new List<T>();
			ToList(list);
			return list;
		}

		public T AtIndex(int index)
		{
			s_Index.matchIndex = index;
			s_Index.Run(m_Element, m_Matchers);
			T result = s_Index.match as T;
			s_Index.match = null;
			return result;
		}

		public void ForEach(Action<T> funcCall)
		{
			s_Action.callBack = funcCall;
			s_Action.Run(m_Element, m_Matchers);
			s_Action.callBack = null;
		}

		public void ForEach<T2>(List<T2> result, Func<T, T2> funcCall)
		{
			DelegateQueryMatcher<T2> s_Instance = DelegateQueryMatcher<T2>.s_Instance;
			s_Instance.callBack = funcCall;
			s_Instance.result = result;
			s_Instance.Run(m_Element, m_Matchers);
			s_Instance.callBack = null;
			s_Instance.result = null;
		}

		public List<T2> ForEach<T2>(Func<T, T2> funcCall)
		{
			List<T2> result = new List<T2>();
			ForEach(result, funcCall);
			return result;
		}
	}

	private static FirstQueryMatcher s_First = new FirstQueryMatcher();

	private static LastQueryMatcher s_Last = new LastQueryMatcher();

	private static IndexQueryMatcher s_Index = new IndexQueryMatcher();
}
