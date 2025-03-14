using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.U2D;

/// <summary>
///   <para>Sprite Atlas is an asset created within Unity. It is part of the built-in sprite packing solution.</para>
/// </summary>
public sealed class SpriteAtlas : Object
{
	/// <summary>
	///   <para>Return true if this SpriteAtlas is a variant.</para>
	/// </summary>
	public extern bool isVariant
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Get the tag of this SpriteAtlas.</para>
	/// </summary>
	public extern string tag
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Get the total number of Sprite packed into this atlas.</para>
	/// </summary>
	public extern int spriteCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Clone all the Sprite in this atlas and fill them into the supplied array.</para>
	/// </summary>
	/// <param name="sprites">Array of Sprite that will be filled.</param>
	/// <returns>
	///   <para>The size of the returned array.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern int GetSprites(Sprite[] sprites);

	/// <summary>
	///   <para>Clone all the Sprite matching the name in this atlas and fill them into the supplied array.</para>
	/// </summary>
	/// <param name="sprites">Array of Sprite that will be filled.</param>
	/// <param name="name">The name of the Sprite.</param>
	public int GetSprites(Sprite[] sprites, string name)
	{
		return GetSpritesByName(sprites, name);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern int GetSpritesByName(Sprite[] sprites, string name);

	/// <summary>
	///   <para>Clone the first Sprite in this atlas that matches the name packed in this atlas and return it.</para>
	/// </summary>
	/// <param name="name">The name of the Sprite.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Sprite GetSprite(string name);
}
