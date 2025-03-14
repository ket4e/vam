using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Representation of RGBA colors in 32 bit format.</para>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
[UsedByNativeCode]
public struct Color32
{
	[FieldOffset(0)]
	private int rgba;

	/// <summary>
	///   <para>Red component of the color.</para>
	/// </summary>
	[FieldOffset(0)]
	public byte r;

	/// <summary>
	///   <para>Green component of the color.</para>
	/// </summary>
	[FieldOffset(1)]
	public byte g;

	/// <summary>
	///   <para>Blue component of the color.</para>
	/// </summary>
	[FieldOffset(2)]
	public byte b;

	/// <summary>
	///   <para>Alpha component of the color.</para>
	/// </summary>
	[FieldOffset(3)]
	public byte a;

	/// <summary>
	///   <para>Constructs a new Color32 with given r, g, b, a components.</para>
	/// </summary>
	/// <param name="r"></param>
	/// <param name="g"></param>
	/// <param name="b"></param>
	/// <param name="a"></param>
	public Color32(byte r, byte g, byte b, byte a)
	{
		rgba = 0;
		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;
	}

	public static implicit operator Color32(Color c)
	{
		return new Color32((byte)(Mathf.Clamp01(c.r) * 255f), (byte)(Mathf.Clamp01(c.g) * 255f), (byte)(Mathf.Clamp01(c.b) * 255f), (byte)(Mathf.Clamp01(c.a) * 255f));
	}

	public static implicit operator Color(Color32 c)
	{
		return new Color((float)(int)c.r / 255f, (float)(int)c.g / 255f, (float)(int)c.b / 255f, (float)(int)c.a / 255f);
	}

	/// <summary>
	///   <para>Linearly interpolates between colors a and b by t.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="t"></param>
	public static Color32 Lerp(Color32 a, Color32 b, float t)
	{
		t = Mathf.Clamp01(t);
		return new Color32((byte)((float)(int)a.r + (float)(b.r - a.r) * t), (byte)((float)(int)a.g + (float)(b.g - a.g) * t), (byte)((float)(int)a.b + (float)(b.b - a.b) * t), (byte)((float)(int)a.a + (float)(b.a - a.a) * t));
	}

	/// <summary>
	///   <para>Linearly interpolates between colors a and b by t.</para>
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="t"></param>
	public static Color32 LerpUnclamped(Color32 a, Color32 b, float t)
	{
		return new Color32((byte)((float)(int)a.r + (float)(b.r - a.r) * t), (byte)((float)(int)a.g + (float)(b.g - a.g) * t), (byte)((float)(int)a.b + (float)(b.b - a.b) * t), (byte)((float)(int)a.a + (float)(b.a - a.a) * t));
	}

	/// <summary>
	///   <para>Returns a nicely formatted string of this color.</para>
	/// </summary>
	/// <param name="format"></param>
	public override string ToString()
	{
		return UnityString.Format("RGBA({0}, {1}, {2}, {3})", r, g, b, a);
	}

	/// <summary>
	///   <para>Returns a nicely formatted string of this color.</para>
	/// </summary>
	/// <param name="format"></param>
	public string ToString(string format)
	{
		return UnityString.Format("RGBA({0}, {1}, {2}, {3})", r.ToString(format), g.ToString(format), b.ToString(format), a.ToString(format));
	}
}
