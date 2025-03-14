using System;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal class StyleSheet : ScriptableObject
{
	[SerializeField]
	private StyleRule[] m_Rules;

	[SerializeField]
	private StyleComplexSelector[] m_ComplexSelectors;

	[SerializeField]
	internal float[] floats;

	[SerializeField]
	internal Color[] colors;

	[SerializeField]
	internal string[] strings;

	public StyleRule[] rules
	{
		get
		{
			return m_Rules;
		}
		internal set
		{
			m_Rules = value;
			SetupReferences();
		}
	}

	public StyleComplexSelector[] complexSelectors
	{
		get
		{
			return m_ComplexSelectors;
		}
		internal set
		{
			m_ComplexSelectors = value;
			SetupReferences();
		}
	}

	private static bool TryCheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle[] handles, int index, out T value)
	{
		bool result = false;
		value = default(T);
		if (index < handles.Length)
		{
			StyleValueHandle styleValueHandle = handles[index];
			if (styleValueHandle.valueType == type && styleValueHandle.valueIndex >= 0 && styleValueHandle.valueIndex < list.Length)
			{
				value = list[styleValueHandle.valueIndex];
				result = true;
			}
			else
			{
				Debug.LogErrorFormat("Trying to read value of type {0} while reading a value of type {1}", type, styleValueHandle.valueType);
			}
		}
		return result;
	}

	private static T CheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle handle)
	{
		T result = default(T);
		if (handle.valueType != type)
		{
			Debug.LogErrorFormat("Trying to read value of type {0} while reading a value of type {1}", type, handle.valueType);
		}
		else if (handle.valueIndex < 0 && handle.valueIndex >= list.Length)
		{
			Debug.LogError("Accessing invalid property");
		}
		else
		{
			result = list[handle.valueIndex];
		}
		return result;
	}

	private void OnEnable()
	{
		SetupReferences();
	}

	private void SetupReferences()
	{
		if (complexSelectors == null || rules == null)
		{
			return;
		}
		for (int i = 0; i < complexSelectors.Length; i++)
		{
			StyleComplexSelector styleComplexSelector = complexSelectors[i];
			if (styleComplexSelector.ruleIndex < rules.Length)
			{
				styleComplexSelector.rule = rules[styleComplexSelector.ruleIndex];
			}
		}
	}

	public StyleValueKeyword ReadKeyword(StyleValueHandle handle)
	{
		return (StyleValueKeyword)handle.valueIndex;
	}

	public float ReadFloat(StyleValueHandle handle)
	{
		return CheckAccess(floats, StyleValueType.Float, handle);
	}

	public bool TryReadFloat(StyleValueHandle[] handles, int index, out float value)
	{
		return TryCheckAccess(floats, StyleValueType.Float, handles, index, out value);
	}

	public Color ReadColor(StyleValueHandle handle)
	{
		return CheckAccess(colors, StyleValueType.Color, handle);
	}

	public bool TryReadColor(StyleValueHandle[] handles, int index, out Color value)
	{
		return TryCheckAccess(colors, StyleValueType.Color, handles, index, out value);
	}

	public string ReadString(StyleValueHandle handle)
	{
		return CheckAccess(strings, StyleValueType.String, handle);
	}

	public bool TryReadString(StyleValueHandle[] handles, int index, out string value)
	{
		return TryCheckAccess(strings, StyleValueType.String, handles, index, out value);
	}

	public string ReadEnum(StyleValueHandle handle)
	{
		return CheckAccess(strings, StyleValueType.Enum, handle);
	}

	public bool TryReadEnum(StyleValueHandle[] handles, int index, out string value)
	{
		return TryCheckAccess(strings, StyleValueType.Enum, handles, index, out value);
	}

	public string ReadResourcePath(StyleValueHandle handle)
	{
		return CheckAccess(strings, StyleValueType.ResourcePath, handle);
	}

	public bool TryReadResourcePath(StyleValueHandle[] handles, int index, out string value)
	{
		return TryCheckAccess(strings, StyleValueType.ResourcePath, handles, index, out value);
	}
}
