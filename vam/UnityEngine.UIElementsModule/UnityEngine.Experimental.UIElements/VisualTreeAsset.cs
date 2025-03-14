#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements;

[Serializable]
public class VisualTreeAsset : ScriptableObject
{
	[Serializable]
	internal struct UsingEntry
	{
		internal static readonly IComparer<UsingEntry> comparer = new UsingEntryComparer();

		[SerializeField]
		public string alias;

		[SerializeField]
		public string path;

		public UsingEntry(string alias, string path)
		{
			this.alias = alias;
			this.path = path;
		}
	}

	private class UsingEntryComparer : IComparer<UsingEntry>
	{
		public int Compare(UsingEntry x, UsingEntry y)
		{
			return Comparer<string>.Default.Compare(x.alias, y.alias);
		}
	}

	[Serializable]
	internal struct SlotDefinition
	{
		[SerializeField]
		public string name;

		[SerializeField]
		public int insertionPointId;
	}

	[Serializable]
	internal struct SlotUsageEntry
	{
		[SerializeField]
		public string slotName;

		[SerializeField]
		public int assetId;

		public SlotUsageEntry(string slotName, int assetId)
		{
			this.slotName = slotName;
			this.assetId = assetId;
		}
	}

	[SerializeField]
	private List<UsingEntry> m_Usings;

	[SerializeField]
	internal StyleSheet inlineSheet;

	[SerializeField]
	private List<VisualElementAsset> m_VisualElementAssets;

	[SerializeField]
	private List<TemplateAsset> m_TemplateAssets;

	[SerializeField]
	private List<SlotDefinition> m_Slots;

	[SerializeField]
	private int m_ContentContainerId;

	internal List<VisualElementAsset> visualElementAssets
	{
		get
		{
			return m_VisualElementAssets;
		}
		set
		{
			m_VisualElementAssets = value;
		}
	}

	internal List<TemplateAsset> templateAssets
	{
		get
		{
			return m_TemplateAssets;
		}
		set
		{
			m_TemplateAssets = value;
		}
	}

	internal List<SlotDefinition> slots
	{
		get
		{
			return m_Slots;
		}
		set
		{
			m_Slots = value;
		}
	}

	internal int contentContainerId
	{
		get
		{
			return m_ContentContainerId;
		}
		set
		{
			m_ContentContainerId = value;
		}
	}

	public VisualElement CloneTree(Dictionary<string, VisualElement> slotInsertionPoints)
	{
		TemplateContainer templateContainer = new TemplateContainer(base.name);
		CloneTree(templateContainer, slotInsertionPoints ?? new Dictionary<string, VisualElement>());
		return templateContainer;
	}

	public void CloneTree(VisualElement target, Dictionary<string, VisualElement> slotInsertionPoints)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target", "Cannot clone a Visual Tree in a null target");
		}
		if ((m_VisualElementAssets == null || m_VisualElementAssets.Count <= 0) && (m_TemplateAssets == null || m_TemplateAssets.Count <= 0))
		{
			return;
		}
		Dictionary<int, List<VisualElementAsset>> dictionary = new Dictionary<int, List<VisualElementAsset>>();
		int num = ((m_VisualElementAssets != null) ? m_VisualElementAssets.Count : 0);
		int num2 = ((m_TemplateAssets != null) ? m_TemplateAssets.Count : 0);
		for (int i = 0; i < num + num2; i++)
		{
			VisualElementAsset visualElementAsset = ((i >= num) ? m_TemplateAssets[i - num] : m_VisualElementAssets[i]);
			if (!dictionary.TryGetValue(visualElementAsset.parentId, out var value))
			{
				value = new List<VisualElementAsset>();
				dictionary.Add(visualElementAsset.parentId, value);
			}
			value.Add(visualElementAsset);
		}
		if (!dictionary.TryGetValue(0, out var value2) || value2 == null)
		{
			return;
		}
		foreach (VisualElementAsset item in value2)
		{
			Assert.IsNotNull(item);
			VisualElement child = CloneSetupRecursively(item, dictionary, new CreationContext(slotInsertionPoints, this, target));
			target.shadow.Add(child);
		}
	}

	private VisualElement CloneSetupRecursively(VisualElementAsset root, Dictionary<int, List<VisualElementAsset>> idToChildren, CreationContext context)
	{
		VisualElement visualElement = root.Create(context);
		if (root.id == context.visualTreeAsset.contentContainerId)
		{
			if (context.target is TemplateContainer)
			{
				((TemplateContainer)context.target).SetContentContainer(visualElement);
			}
			else
			{
				Debug.LogError("Trying to clone a VisualTreeAsset with a custom content container into a element which is not a template container");
			}
		}
		visualElement.name = root.name;
		if (context.slotInsertionPoints != null && TryGetSlotInsertionPoint(root.id, out var slotName))
		{
			context.slotInsertionPoints.Add(slotName, visualElement);
		}
		if (root.classes != null)
		{
			for (int i = 0; i < root.classes.Length; i++)
			{
				visualElement.AddToClassList(root.classes[i]);
			}
		}
		if (root.ruleIndex != -1)
		{
			if (inlineSheet == null)
			{
				Debug.LogWarning("VisualElementAsset has a RuleIndex but no inlineStyleSheet");
			}
			else
			{
				StyleRule rule = inlineSheet.rules[root.ruleIndex];
				VisualElementStylesData visualElementStylesData = new VisualElementStylesData(isShared: false);
				visualElement.SetInlineStyles(visualElementStylesData);
				visualElementStylesData.ApplyRule(inlineSheet, int.MaxValue, rule, StyleSheetCache.GetPropertyIDs(inlineSheet, root.ruleIndex));
			}
		}
		TemplateAsset templateAsset = root as TemplateAsset;
		if (idToChildren.TryGetValue(root.id, out var value))
		{
			foreach (VisualElementAsset childVea in value)
			{
				VisualElement visualElement2 = CloneSetupRecursively(childVea, idToChildren, context);
				if (visualElement2 == null)
				{
					continue;
				}
				if (templateAsset == null)
				{
					visualElement.Add(visualElement2);
					continue;
				}
				int num = ((templateAsset.slotUsages != null) ? templateAsset.slotUsages.FindIndex((SlotUsageEntry u) => u.assetId == childVea.id) : (-1));
				if (num != -1)
				{
					string slotName2 = templateAsset.slotUsages[num].slotName;
					Assert.IsFalse(string.IsNullOrEmpty(slotName2), "a lost name should not be null or empty, this probably points to an importer or serialization bug");
					if (context.slotInsertionPoints == null || !context.slotInsertionPoints.TryGetValue(slotName2, out var value2))
					{
						Debug.LogErrorFormat("Slot '{0}' was not found. Existing slots: {1}", slotName2, (context.slotInsertionPoints != null) ? string.Join(", ", context.slotInsertionPoints.Keys.ToArray()) : string.Empty);
						visualElement.Add(visualElement2);
					}
					else
					{
						value2.Add(visualElement2);
					}
				}
				else
				{
					visualElement.Add(visualElement2);
				}
			}
		}
		if (templateAsset != null && context.slotInsertionPoints != null)
		{
			context.slotInsertionPoints.Clear();
		}
		return visualElement;
	}

	internal bool TryGetSlotInsertionPoint(int insertionPointId, out string slotName)
	{
		if (m_Slots == null)
		{
			slotName = null;
			return false;
		}
		for (int i = 0; i < m_Slots.Count; i++)
		{
			SlotDefinition slotDefinition = m_Slots[i];
			if (slotDefinition.insertionPointId == insertionPointId)
			{
				slotName = slotDefinition.name;
				return true;
			}
		}
		slotName = null;
		return false;
	}

	internal VisualTreeAsset ResolveUsing(string templateAlias)
	{
		if (m_Usings == null || m_Usings.Count == 0)
		{
			return null;
		}
		int num = m_Usings.BinarySearch(new UsingEntry(templateAlias, null), UsingEntry.comparer);
		if (num < 0)
		{
			return null;
		}
		string path = m_Usings[num].path;
		return (Panel.loadResourceFunc != null) ? (Panel.loadResourceFunc(path, typeof(VisualTreeAsset)) as VisualTreeAsset) : null;
	}
}
