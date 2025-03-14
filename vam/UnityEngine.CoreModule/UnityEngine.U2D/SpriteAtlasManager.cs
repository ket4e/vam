using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.U2D;

/// <summary>
///   <para>Manages SpriteAtlas during runtime.</para>
/// </summary>
public sealed class SpriteAtlasManager
{
	/// <summary>
	///   <para>Delegate type for atlas request callback.</para>
	/// </summary>
	/// <param name="tag">Tag of SpriteAtlas that needs to be provided by user.</param>
	/// <param name="action">An Action that takes user loaded SpriteAtlas.</param>
	public delegate void RequestAtlasCallback(string tag, Action<SpriteAtlas> action);

	public static event RequestAtlasCallback atlasRequested;

	[RequiredByNativeCode]
	private static bool RequestAtlas(string tag)
	{
		if (SpriteAtlasManager.atlasRequested != null)
		{
			SpriteAtlasManager.atlasRequested(tag, Register);
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void Register(SpriteAtlas spriteAtlas);

	static SpriteAtlasManager()
	{
		SpriteAtlasManager.atlasRequested = null;
	}
}
