using System;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets;

internal static class StyleSheetExtensions
{
	public static void Apply<T>(this StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<T> property, HandlesApplicatorFunction<T> applicatorFunc)
	{
		if (handles[0].valueType == StyleValueType.Keyword && handles[0].valueIndex == 2)
		{
			StyleSheetApplicator.ApplyDefault(specificity, ref property);
		}
		else
		{
			applicatorFunc(sheet, handles, specificity, ref property);
		}
	}

	public static void ApplyShorthand(this StyleSheet sheet, StyleValueHandle[] handles, int specificity, VisualElementStylesData styleData, ShorthandApplicatorFunction applicatorFunc)
	{
		if (handles[0].valueType != 0 && handles[0].valueIndex != 2)
		{
			applicatorFunc(sheet, handles, specificity, styleData);
		}
	}

	public static string ReadAsString(this StyleSheet sheet, StyleValueHandle handle)
	{
		string empty = string.Empty;
		return handle.valueType switch
		{
			StyleValueType.Float => sheet.ReadFloat(handle).ToString(), 
			StyleValueType.Color => sheet.ReadColor(handle).ToString(), 
			StyleValueType.ResourcePath => sheet.ReadResourcePath(handle), 
			StyleValueType.String => sheet.ReadString(handle), 
			StyleValueType.Enum => sheet.ReadEnum(handle), 
			StyleValueType.Keyword => sheet.ReadKeyword(handle).ToString(), 
			_ => throw new ArgumentException("Unhandled type " + handle.valueType), 
		};
	}
}
