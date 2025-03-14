using System;
using System.Linq;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility;

/// <summary>
///   <para>A class containing methods to assist with accessibility for users with different vision capabilities.</para>
/// </summary>
[UsedByNativeCode]
public static class VisionUtility
{
	private static readonly Color[] s_ColorBlindSafePalette = new Color[11]
	{
		new Color32(0, 0, 0, byte.MaxValue),
		new Color32(73, 0, 146, byte.MaxValue),
		new Color32(7, 71, 81, byte.MaxValue),
		new Color32(0, 146, 146, byte.MaxValue),
		new Color32(182, 109, byte.MaxValue, byte.MaxValue),
		new Color32(byte.MaxValue, 109, 182, byte.MaxValue),
		new Color32(109, 182, byte.MaxValue, byte.MaxValue),
		new Color32(36, byte.MaxValue, 36, byte.MaxValue),
		new Color32(byte.MaxValue, 182, 219, byte.MaxValue),
		new Color32(182, 219, byte.MaxValue, byte.MaxValue),
		new Color32(byte.MaxValue, byte.MaxValue, 109, byte.MaxValue)
	};

	private static readonly float[] s_ColorBlindSafePaletteLuminanceValues = s_ColorBlindSafePalette.Select((Color c) => ComputePerceivedLuminance(c)).ToArray();

	internal static float ComputePerceivedLuminance(Color color)
	{
		color = color.linear;
		return Mathf.LinearToGammaSpace(0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b);
	}

	/// <summary>
	///   <para>Gets a palette of colors that should be distinguishable for normal vision, deuteranopia, protanopia, and tritanopia.</para>
	/// </summary>
	/// <param name="palette">An array of colors to populate with a palette.</param>
	/// <param name="minimumLuminance">Minimum allowable perceived luminance from 0 to 1. A value of 0.2 or greater is recommended for dark backgrounds.</param>
	/// <param name="maximumLuminance">Maximum allowable perceived luminance from 0 to 1. A value of 0.8 or less is recommended for light backgrounds.</param>
	/// <returns>
	///   <para>The number of unambiguous colors in the palette.</para>
	/// </returns>
	public static int GetColorBlindSafePalette(Color[] palette, float minimumLuminance, float maximumLuminance)
	{
		if (palette == null)
		{
			throw new ArgumentNullException("palette");
		}
		Color[] array = (from i in Enumerable.Range(0, s_ColorBlindSafePalette.Length)
			where s_ColorBlindSafePaletteLuminanceValues[i] >= minimumLuminance && s_ColorBlindSafePaletteLuminanceValues[i] <= maximumLuminance
			select s_ColorBlindSafePalette[i]).ToArray();
		int num = Mathf.Min(palette.Length, array.Length);
		if (num > 0)
		{
			int j = 0;
			for (int num2 = palette.Length; j < num2; j++)
			{
				ref Color reference = ref palette[j];
				reference = array[j % num];
			}
		}
		else
		{
			int k = 0;
			for (int num3 = palette.Length; k < num3; k++)
			{
				palette[k] = default(Color);
			}
		}
		return num;
	}
}
