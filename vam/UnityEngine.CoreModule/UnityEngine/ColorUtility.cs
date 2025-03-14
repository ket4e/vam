using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A collection of common color functions.</para>
/// </summary>
public sealed class ColorUtility
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern bool DoTryParseHtmlColor(string htmlString, out Color32 color);

	public static bool TryParseHtmlString(string htmlString, out Color color)
	{
		Color32 color2;
		bool result = DoTryParseHtmlColor(htmlString, out color2);
		color = color2;
		return result;
	}

	/// <summary>
	///   <para>Returns the color as a hexadecimal string in the format "RRGGBB".</para>
	/// </summary>
	/// <param name="color">The color to be converted.</param>
	/// <returns>
	///   <para>Hexadecimal string representing the color.</para>
	/// </returns>
	public static string ToHtmlStringRGB(Color color)
	{
		Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), 1);
		return $"{color2.r:X2}{color2.g:X2}{color2.b:X2}";
	}

	/// <summary>
	///   <para>Returns the color as a hexadecimal string in the format "RRGGBBAA".</para>
	/// </summary>
	/// <param name="color">The color to be converted.</param>
	/// <returns>
	///   <para>Hexadecimal string representing the color.</para>
	/// </returns>
	public static string ToHtmlStringRGBA(Color color)
	{
		Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * 255f), 0, 255));
		return $"{color2.r:X2}{color2.g:X2}{color2.b:X2}{color2.a:X2}";
	}
}
