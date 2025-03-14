using System;
using System.Reflection;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Allows to control for which display the OnGUI is called.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class GUITargetAttribute : Attribute
{
	internal int displayMask;

	/// <summary>
	///   <para>Default constructor initializes the attribute for OnGUI to be called for all available displays.</para>
	/// </summary>
	/// <param name="displayIndex">Display index.</param>
	/// <param name="displayIndex1">Display index.</param>
	/// <param name="displayIndexList">Display index list.</param>
	public GUITargetAttribute()
	{
		displayMask = -1;
	}

	/// <summary>
	///   <para>Default constructor initializes the attribute for OnGUI to be called for all available displays.</para>
	/// </summary>
	/// <param name="displayIndex">Display index.</param>
	/// <param name="displayIndex1">Display index.</param>
	/// <param name="displayIndexList">Display index list.</param>
	public GUITargetAttribute(int displayIndex)
	{
		displayMask = 1 << displayIndex;
	}

	/// <summary>
	///   <para>Default constructor initializes the attribute for OnGUI to be called for all available displays.</para>
	/// </summary>
	/// <param name="displayIndex">Display index.</param>
	/// <param name="displayIndex1">Display index.</param>
	/// <param name="displayIndexList">Display index list.</param>
	public GUITargetAttribute(int displayIndex, int displayIndex1)
	{
		displayMask = (1 << displayIndex) | (1 << displayIndex1);
	}

	/// <summary>
	///   <para>Default constructor initializes the attribute for OnGUI to be called for all available displays.</para>
	/// </summary>
	/// <param name="displayIndex">Display index.</param>
	/// <param name="displayIndex1">Display index.</param>
	/// <param name="displayIndexList">Display index list.</param>
	public GUITargetAttribute(int displayIndex, int displayIndex1, params int[] displayIndexList)
	{
		displayMask = (1 << displayIndex) | (1 << displayIndex1);
		for (int i = 0; i < displayIndexList.Length; i++)
		{
			displayMask |= 1 << displayIndexList[i];
		}
	}

	[RequiredByNativeCode]
	private static int GetGUITargetAttrValue(Type klass, string methodName)
	{
		MethodInfo method = klass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (method != null)
		{
			object[] customAttributes = method.GetCustomAttributes(inherit: true);
			if (customAttributes != null)
			{
				for (int i = 0; i < customAttributes.Length; i++)
				{
					if (customAttributes[i].GetType() == typeof(GUITargetAttribute))
					{
						GUITargetAttribute gUITargetAttribute = customAttributes[i] as GUITargetAttribute;
						return gUITargetAttribute.displayMask;
					}
				}
			}
		}
		return -1;
	}
}
