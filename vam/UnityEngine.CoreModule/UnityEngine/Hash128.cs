using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Represent the hash value.</para>
/// </summary>
[UsedByNativeCode]
[NativeHeader("Runtime/Utilities/Hash128.h")]
public struct Hash128
{
	private uint m_u32_0;

	private uint m_u32_1;

	private uint m_u32_2;

	private uint m_u32_3;

	/// <summary>
	///   <para>Get if the hash value is valid or not. (Read Only)</para>
	/// </summary>
	public bool isValid => m_u32_0 != 0 || m_u32_1 != 0 || m_u32_2 != 0 || m_u32_3 != 0;

	/// <summary>
	///   <para>Construct the Hash128.</para>
	/// </summary>
	/// <param name="u32_0"></param>
	/// <param name="u32_1"></param>
	/// <param name="u32_2"></param>
	/// <param name="u32_3"></param>
	public Hash128(uint u32_0, uint u32_1, uint u32_2, uint u32_3)
	{
		m_u32_0 = u32_0;
		m_u32_1 = u32_1;
		m_u32_2 = u32_2;
		m_u32_3 = u32_3;
	}

	/// <summary>
	///   <para>Convert Hash128 to string.</para>
	/// </summary>
	public override string ToString()
	{
		return Internal_Hash128ToString(this);
	}

	/// <summary>
	///   <para>Convert the input string to Hash128.</para>
	/// </summary>
	/// <param name="hashString"></param>
	[FreeFunction("StringToHash128")]
	public static Hash128 Parse(string hashString)
	{
		Parse_Injected(hashString, out var ret);
		return ret;
	}

	[FreeFunction("Hash128ToString")]
	internal static string Internal_Hash128ToString(Hash128 hash128)
	{
		return Internal_Hash128ToString_Injected(ref hash128);
	}

	public override bool Equals(object obj)
	{
		return obj is Hash128 && this == (Hash128)obj;
	}

	public override int GetHashCode()
	{
		return m_u32_0.GetHashCode() ^ m_u32_1.GetHashCode() ^ m_u32_2.GetHashCode() ^ m_u32_3.GetHashCode();
	}

	public static bool operator ==(Hash128 hash1, Hash128 hash2)
	{
		return hash1.m_u32_0 == hash2.m_u32_0 && hash1.m_u32_1 == hash2.m_u32_1 && hash1.m_u32_2 == hash2.m_u32_2 && hash1.m_u32_3 == hash2.m_u32_3;
	}

	public static bool operator !=(Hash128 hash1, Hash128 hash2)
	{
		return !(hash1 == hash2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Parse_Injected(string hashString, out Hash128 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string Internal_Hash128ToString_Injected(ref Hash128 hash128);
}
