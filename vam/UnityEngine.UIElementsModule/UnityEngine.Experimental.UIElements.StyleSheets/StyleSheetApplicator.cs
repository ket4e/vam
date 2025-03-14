using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets;

internal static class StyleSheetApplicator
{
	internal delegate CursorStyle CreateDefaultCursorStyleFunction(StyleSheet sheet, StyleValueHandle handle);

	public static class Shorthand
	{
		private static void ReadFourSidesArea(StyleSheet sheet, StyleValueHandle[] handles, out float top, out float right, out float bottom, out float left)
		{
			top = 0f;
			right = 0f;
			bottom = 0f;
			left = 0f;
			switch (handles.Length)
			{
			case 0:
				break;
			case 1:
				top = (right = (bottom = (left = sheet.ReadFloat(handles[0]))));
				break;
			case 2:
				top = (bottom = sheet.ReadFloat(handles[0]));
				left = (right = sheet.ReadFloat(handles[1]));
				break;
			case 3:
				top = sheet.ReadFloat(handles[0]);
				left = (right = sheet.ReadFloat(handles[1]));
				bottom = sheet.ReadFloat(handles[2]);
				break;
			default:
				top = sheet.ReadFloat(handles[0]);
				right = sheet.ReadFloat(handles[1]);
				bottom = sheet.ReadFloat(handles[2]);
				left = sheet.ReadFloat(handles[3]);
				break;
			}
		}

		public static void ApplyBorderRadius(StyleSheet sheet, StyleValueHandle[] handles, int specificity, VisualElementStylesData styleData)
		{
			ReadFourSidesArea(sheet, handles, out var top, out var right, out var bottom, out var left);
			Apply(top, specificity, ref styleData.borderTopLeftRadius);
			Apply(right, specificity, ref styleData.borderTopRightRadius);
			Apply(left, specificity, ref styleData.borderBottomLeftRadius);
			Apply(bottom, specificity, ref styleData.borderBottomRightRadius);
		}

		public static void ApplyMargin(StyleSheet sheet, StyleValueHandle[] handles, int specificity, VisualElementStylesData styleData)
		{
			ReadFourSidesArea(sheet, handles, out var top, out var right, out var bottom, out var left);
			Apply(top, specificity, ref styleData.marginTop);
			Apply(right, specificity, ref styleData.marginRight);
			Apply(bottom, specificity, ref styleData.marginBottom);
			Apply(left, specificity, ref styleData.marginLeft);
		}

		public static void ApplyPadding(StyleSheet sheet, StyleValueHandle[] handles, int specificity, VisualElementStylesData styleData)
		{
			ReadFourSidesArea(sheet, handles, out var top, out var right, out var bottom, out var left);
			Apply(top, specificity, ref styleData.paddingTop);
			Apply(right, specificity, ref styleData.paddingRight);
			Apply(bottom, specificity, ref styleData.paddingBottom);
			Apply(left, specificity, ref styleData.paddingLeft);
		}
	}

	internal static CreateDefaultCursorStyleFunction createDefaultCursorStyleFunc = null;

	private static void Apply<T>(T val, int specificity, ref StyleValue<T> property)
	{
		property.Apply(new StyleValue<T>(val, specificity), StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity);
	}

	public static void ApplyDefault<T>(int specificity, ref StyleValue<T> property)
	{
		Apply(default(T), specificity, ref property);
	}

	public static void ApplyBool(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<bool> property)
	{
		bool val = sheet.ReadKeyword(handles[0]) == StyleValueKeyword.True;
		Apply(val, specificity, ref property);
	}

	public static void ApplyFloat(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<float> property)
	{
		float val = sheet.ReadFloat(handles[0]);
		Apply(val, specificity, ref property);
	}

	public static void ApplyInt(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<int> property)
	{
		int val = (int)sheet.ReadFloat(handles[0]);
		Apply(val, specificity, ref property);
	}

	public static void ApplyEnum<T>(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<int> property)
	{
		int enumValue = StyleSheetCache.GetEnumValue<T>(sheet, handles[0]);
		Apply(enumValue, specificity, ref property);
	}

	public static void ApplyColor(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<Color> property)
	{
		Color val = sheet.ReadColor(handles[0]);
		Apply(val, specificity, ref property);
	}

	public static void ApplyCursor(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<CursorStyle> property)
	{
		StyleValueHandle handle = handles[0];
		if (handle.valueType == StyleValueType.ResourcePath)
		{
			string pathName = sheet.ReadResourcePath(handles[0]);
			Texture2D texture2D = Panel.loadResourceFunc(pathName, typeof(Texture2D)) as Texture2D;
			if (texture2D != null)
			{
				Vector2 zero = Vector2.zero;
				sheet.TryReadFloat(handles, 1, out zero.x);
				sheet.TryReadFloat(handles, 2, out zero.y);
				CursorStyle cursorStyle = default(CursorStyle);
				cursorStyle.texture = texture2D;
				cursorStyle.hotspot = zero;
				CursorStyle val = cursorStyle;
				Apply(val, specificity, ref property);
			}
		}
		else if (createDefaultCursorStyleFunc != null)
		{
			CursorStyle val2 = createDefaultCursorStyleFunc(sheet, handle);
			Apply(val2, specificity, ref property);
		}
	}

	public static void ApplyResource<T>(StyleSheet sheet, StyleValueHandle[] handles, int specificity, ref StyleValue<T> property) where T : Object
	{
		StyleValueHandle handle = handles[0];
		if (handle.valueType == StyleValueType.Keyword && handle.valueIndex == 5)
		{
			Apply((T)null, specificity, ref property);
			return;
		}
		T val = (T)null;
		string text = sheet.ReadResourcePath(handle);
		if (!string.IsNullOrEmpty(text))
		{
			val = Panel.loadResourceFunc(text, typeof(T)) as T;
			if ((Object)val != (Object)null)
			{
				Apply(val, specificity, ref property);
			}
			else
			{
				Debug.LogWarning($"{typeof(T).Name} resource/file not found for path: {text}");
			}
		}
	}
}
