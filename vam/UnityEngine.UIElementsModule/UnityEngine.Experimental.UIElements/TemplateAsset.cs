#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.Experimental.UIElements;

[Serializable]
internal class TemplateAsset : VisualElementAsset
{
	[SerializeField]
	private string m_TemplateAlias;

	[SerializeField]
	private List<VisualTreeAsset.SlotUsageEntry> m_SlotUsages;

	public string templateAlias
	{
		get
		{
			return m_TemplateAlias;
		}
		set
		{
			m_TemplateAlias = value;
		}
	}

	internal List<VisualTreeAsset.SlotUsageEntry> slotUsages
	{
		get
		{
			return m_SlotUsages;
		}
		set
		{
			m_SlotUsages = value;
		}
	}

	public TemplateAsset(string templateAlias)
		: base(typeof(TemplateContainer).FullName)
	{
		Assert.IsFalse(string.IsNullOrEmpty(templateAlias), "Template alias must not be null or empty");
		m_TemplateAlias = templateAlias;
	}

	public void AddSlotUsage(string slotName, int resId)
	{
		if (m_SlotUsages == null)
		{
			m_SlotUsages = new List<VisualTreeAsset.SlotUsageEntry>();
		}
		m_SlotUsages.Add(new VisualTreeAsset.SlotUsageEntry(slotName, resId));
	}
}
