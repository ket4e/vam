using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

[Serializable]
internal class VisualElementAsset : IUxmlAttributes
{
	[SerializeField]
	private string m_Name;

	[SerializeField]
	private int m_Id;

	[SerializeField]
	private int m_ParentId;

	[SerializeField]
	private int m_RuleIndex;

	[SerializeField]
	private string m_Text;

	[SerializeField]
	private PickingMode m_PickingMode;

	[SerializeField]
	private string m_FullTypeName;

	[SerializeField]
	private string[] m_Classes;

	[SerializeField]
	private List<string> m_Stylesheets;

	[SerializeField]
	private List<string> m_Properties;

	public string name
	{
		get
		{
			return m_Name;
		}
		set
		{
			m_Name = value;
		}
	}

	public int id
	{
		get
		{
			return m_Id;
		}
		set
		{
			m_Id = value;
		}
	}

	public int parentId
	{
		get
		{
			return m_ParentId;
		}
		set
		{
			m_ParentId = value;
		}
	}

	public int ruleIndex
	{
		get
		{
			return m_RuleIndex;
		}
		set
		{
			m_RuleIndex = value;
		}
	}

	public string text
	{
		get
		{
			return m_Text;
		}
		set
		{
			m_Text = value;
		}
	}

	public PickingMode pickingMode
	{
		get
		{
			return m_PickingMode;
		}
		set
		{
			m_PickingMode = value;
		}
	}

	public string fullTypeName
	{
		get
		{
			return m_FullTypeName;
		}
		set
		{
			m_FullTypeName = value;
		}
	}

	public string[] classes
	{
		get
		{
			return m_Classes;
		}
		set
		{
			m_Classes = value;
		}
	}

	public List<string> stylesheets
	{
		get
		{
			return (m_Stylesheets != null) ? m_Stylesheets : (m_Stylesheets = new List<string>());
		}
		set
		{
			m_Stylesheets = value;
		}
	}

	public VisualElementAsset(string fullTypeName)
	{
		m_FullTypeName = fullTypeName;
	}

	public VisualElement Create(CreationContext ctx)
	{
		if (!Factories.TryGetValue(fullTypeName, out var factory))
		{
			Debug.LogErrorFormat("Visual Element Type '{0}' has no factory method.", fullTypeName);
			return new Label($"Unknown type: '{fullTypeName}'");
		}
		if (factory == null)
		{
			Debug.LogErrorFormat("Visual Element Type '{0}' has a null factory method.", fullTypeName);
			return new Label($"Type with no factory method: '{fullTypeName}'");
		}
		VisualElement visualElement = factory(this, ctx);
		if (visualElement == null)
		{
			Debug.LogErrorFormat("The factory of Visual Element Type '{0}' has returned a null object", fullTypeName);
			return new Label($"The factory of Visual Element Type '{fullTypeName}' has returned a null object");
		}
		visualElement.name = name;
		if (classes != null)
		{
			for (int i = 0; i < classes.Length; i++)
			{
				visualElement.AddToClassList(classes[i]);
			}
		}
		if (stylesheets != null)
		{
			for (int j = 0; j < stylesheets.Count; j++)
			{
				visualElement.AddStyleSheetPath(stylesheets[j]);
			}
		}
		if (visualElement is BaseTextElement baseTextElement && !string.IsNullOrEmpty(text))
		{
			baseTextElement.text = text;
		}
		visualElement.pickingMode = pickingMode;
		return visualElement;
	}

	public void AddProperty(string propertyName, string propertyValue)
	{
		if (m_Properties == null)
		{
			m_Properties = new List<string>();
		}
		m_Properties.Add(propertyName);
		m_Properties.Add(propertyValue);
	}

	public string GetPropertyString(string propertyName)
	{
		if (m_Properties == null)
		{
			return null;
		}
		for (int i = 0; i < m_Properties.Count - 1; i += 2)
		{
			if (m_Properties[i] == propertyName)
			{
				return m_Properties[i + 1];
			}
		}
		return null;
	}

	public int GetPropertyInt(string propertyName, int defaultValue)
	{
		string propertyString = GetPropertyString(propertyName);
		if (propertyString == null || !int.TryParse(propertyString, out var result))
		{
			return defaultValue;
		}
		return result;
	}

	public bool GetPropertyBool(string propertyName, bool defaultValue)
	{
		string propertyString = GetPropertyString(propertyName);
		if (propertyString == null || !bool.TryParse(propertyString, out var result))
		{
			return defaultValue;
		}
		return result;
	}

	public Color GetPropertyColor(string propertyName, Color defaultValue)
	{
		string propertyString = GetPropertyString(propertyName);
		if (propertyString == null || !ColorUtility.TryParseHtmlString(propertyString, out var color))
		{
			return defaultValue;
		}
		return color;
	}

	public long GetPropertyLong(string propertyName, long defaultValue)
	{
		string propertyString = GetPropertyString(propertyName);
		if (propertyString == null || !long.TryParse(propertyString, out var result))
		{
			return defaultValue;
		}
		return result;
	}

	public float GetPropertyFloat(string propertyName, float def)
	{
		string propertyString = GetPropertyString(propertyName);
		if (propertyString == null || !float.TryParse(propertyString, out var result))
		{
			return def;
		}
		return result;
	}

	public T GetPropertyEnum<T>(string propertyName, T def)
	{
		string propertyString = GetPropertyString(propertyName);
		if (propertyString == null || !Enum.IsDefined(typeof(T), propertyString))
		{
			return def;
		}
		return (T)Enum.Parse(typeof(T), propertyString);
	}
}
