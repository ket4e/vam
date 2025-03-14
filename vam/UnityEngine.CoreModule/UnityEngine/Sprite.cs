using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Represents a Sprite object for use in 2D gameplay.</para>
/// </summary>
[NativeHeader("Runtime/2D/Common/ScriptBindings/SpritesMarshalling.h")]
[NativeHeader("Runtime/Graphics/SpriteUtility.h")]
[NativeType("Runtime/Graphics/SpriteFrame.h")]
public sealed class Sprite : Object
{
	/// <summary>
	///   <para>Bounds of the Sprite, specified by its center and extents in world space units.</para>
	/// </summary>
	public Bounds bounds
	{
		get
		{
			INTERNAL_get_bounds(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>Location of the Sprite on the original Texture, specified in pixels.</para>
	/// </summary>
	public Rect rect
	{
		get
		{
			INTERNAL_get_rect(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>Get the reference to the used texture. If packed this will point to the atlas, if not packed will point to the source sprite.</para>
	/// </summary>
	public extern Texture2D texture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Returns the texture that contains the alpha channel from the source texture. Unity generates this texture under the hood for sprites that have alpha in the source, and need to be compressed using techniques like ETC1.
	///
	/// Returns NULL if there is no associated alpha texture for the source sprite. This is the case if the sprite has not been setup to use ETC1 compression.</para>
	/// </summary>
	public extern Texture2D associatedAlphaSplitTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Get the rectangle this sprite uses on its texture. Raises an exception if this sprite is tightly packed in an atlas.</para>
	/// </summary>
	public Rect textureRect
	{
		get
		{
			INTERNAL_get_textureRect(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>Gets the offset of the rectangle this sprite uses on its texture to the original sprite bounds. If sprite mesh type is FullRect, offset is zero.</para>
	/// </summary>
	public Vector2 textureRectOffset
	{
		get
		{
			Internal_GetTextureRectOffset(this, out var output);
			return output;
		}
	}

	/// <summary>
	///   <para>Returns true if this Sprite is packed in an atlas.</para>
	/// </summary>
	public extern bool packed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>If Sprite is packed (see Sprite.packed), returns its SpritePackingMode.</para>
	/// </summary>
	public extern SpritePackingMode packingMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>If Sprite is packed (see Sprite.packed), returns its SpritePackingRotation.</para>
	/// </summary>
	public extern SpritePackingRotation packingRotation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Location of the Sprite's center point in the Rect on the original Texture, specified in pixels.</para>
	/// </summary>
	public Vector2 pivot
	{
		get
		{
			Internal_GetPivot(this, out var output);
			return output;
		}
	}

	/// <summary>
	///   <para>Returns the border sizes of the sprite.</para>
	/// </summary>
	public Vector4 border
	{
		get
		{
			INTERNAL_get_border(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>Returns a copy of the array containing sprite mesh vertex positions.</para>
	/// </summary>
	public extern Vector2[] vertices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Returns a copy of the array containing sprite mesh triangles.</para>
	/// </summary>
	public extern ushort[] triangles
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The base texture coordinates of the sprite mesh.</para>
	/// </summary>
	public extern Vector2[] uv
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The number of pixels in the sprite that correspond to one unit in world space. (Read Only)</para>
	/// </summary>
	public extern float pixelsPerUnit
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("GetPixelsToUnits")]
		get;
	}

	private Sprite()
	{
	}

	/// <summary>
	///   <para>Create a new Sprite object.</para>
	/// </summary>
	/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
	/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
	/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
	/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
	/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
	/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
	/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
	/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, [DefaultValue("100.0f")] float pixelsPerUnit, [DefaultValue("0")] uint extrude, [DefaultValue("SpriteMeshType.Tight")] SpriteMeshType meshType, [DefaultValue("Vector4.zero")] Vector4 border, [DefaultValue("false")] bool generateFallbackPhysicsShape)
	{
		return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref border, generateFallbackPhysicsShape);
	}

	/// <summary>
	///   <para>Create a new Sprite object.</para>
	/// </summary>
	/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
	/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
	/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
	/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
	/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
	/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
	/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
	/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
	[ExcludeFromDocs]
	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border)
	{
		bool generateFallbackPhysicsShape = false;
		return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref border, generateFallbackPhysicsShape);
	}

	/// <summary>
	///   <para>Create a new Sprite object.</para>
	/// </summary>
	/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
	/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
	/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
	/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
	/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
	/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
	/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
	/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
	[ExcludeFromDocs]
	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType)
	{
		bool generateFallbackPhysicsShape = false;
		Vector4 zero = Vector4.zero;
		return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref zero, generateFallbackPhysicsShape);
	}

	/// <summary>
	///   <para>Create a new Sprite object.</para>
	/// </summary>
	/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
	/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
	/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
	/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
	/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
	/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
	/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
	/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
	[ExcludeFromDocs]
	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude)
	{
		bool generateFallbackPhysicsShape = false;
		Vector4 zero = Vector4.zero;
		SpriteMeshType meshType = SpriteMeshType.Tight;
		return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref zero, generateFallbackPhysicsShape);
	}

	/// <summary>
	///   <para>Create a new Sprite object.</para>
	/// </summary>
	/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
	/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
	/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
	/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
	/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
	/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
	/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
	/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
	[ExcludeFromDocs]
	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit)
	{
		bool generateFallbackPhysicsShape = false;
		Vector4 zero = Vector4.zero;
		SpriteMeshType meshType = SpriteMeshType.Tight;
		uint extrude = 0u;
		return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref zero, generateFallbackPhysicsShape);
	}

	/// <summary>
	///   <para>Create a new Sprite object.</para>
	/// </summary>
	/// <param name="texture">Texture from which to obtain the sprite graphic.</param>
	/// <param name="rect">Rectangular section of the texture to use for the sprite.</param>
	/// <param name="pivot">Sprite's pivot point relative to its graphic rectangle.</param>
	/// <param name="pixelsPerUnit">The number of pixels in the sprite that correspond to one unit in world space.</param>
	/// <param name="extrude">Amount by which the sprite mesh should be expanded outwards.</param>
	/// <param name="meshType">Controls the type of mesh generated for the sprite.</param>
	/// <param name="border">The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).</param>
	/// <param name="generateFallbackPhysicsShape">Generates a default physics shape for the sprite.</param>
	[ExcludeFromDocs]
	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot)
	{
		bool generateFallbackPhysicsShape = false;
		Vector4 zero = Vector4.zero;
		SpriteMeshType meshType = SpriteMeshType.Tight;
		uint extrude = 0u;
		float num = 100f;
		return INTERNAL_CALL_Create(texture, ref rect, ref pivot, num, extrude, meshType, ref zero, generateFallbackPhysicsShape);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern Sprite INTERNAL_CALL_Create(Texture2D texture, ref Rect rect, ref Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, ref Vector4 border, bool generateFallbackPhysicsShape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_bounds(out Bounds value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_rect(out Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_textureRect(out Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_GetTextureRectOffset(Sprite sprite, out Vector2 output);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_GetPivot(Sprite sprite, out Vector2 output);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_border(out Vector4 value);

	/// <summary>
	///   <para>Sets up new Sprite geometry.</para>
	/// </summary>
	/// <param name="vertices">Array of vertex positions in Sprite Rect space.</param>
	/// <param name="triangles">Array of sprite mesh triangle indices.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void OverrideGeometry(Vector2[] vertices, ushort[] triangles);

	/// <summary>
	///   <para>The number of physics shapes for the Sprite.</para>
	/// </summary>
	/// <returns>
	///   <para>The number of physics shapes for the Sprite.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern int GetPhysicsShapeCount();

	/// <summary>
	///   <para>The number of points in the selected physics shape for the Sprite.</para>
	/// </summary>
	/// <param name="shapeIdx">The index of the physics shape to retrieve the number of points from.</param>
	/// <returns>
	///   <para>The number of points in the selected physics shape for the Sprite.</para>
	/// </returns>
	public int GetPhysicsShapePointCount(int shapeIdx)
	{
		int physicsShapeCount = GetPhysicsShapeCount();
		if (shapeIdx < 0 || shapeIdx >= physicsShapeCount)
		{
			throw new IndexOutOfRangeException($"Index({shapeIdx}) is out of bounds(0 - {physicsShapeCount - 1})");
		}
		return Internal_GetPhysicsShapePointCount(shapeIdx);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("GetPhysicsShapePointCount")]
	private extern int Internal_GetPhysicsShapePointCount(int shapeIdx);

	public int GetPhysicsShape(int shapeIdx, List<Vector2> physicsShape)
	{
		int physicsShapeCount = GetPhysicsShapeCount();
		if (shapeIdx < 0 || shapeIdx >= physicsShapeCount)
		{
			throw new IndexOutOfRangeException($"Index({shapeIdx}) is out of bounds(0 - {physicsShapeCount - 1})");
		}
		GetPhysicsShapeImpl(this, shapeIdx, physicsShape);
		return physicsShape.Count;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("SpritesBindings::GetPhysicsShape", ThrowsException = true)]
	private static extern void GetPhysicsShapeImpl(Sprite sprite, int shapeIdx, List<Vector2> physicsShape);

	public void OverridePhysicsShape(IList<Vector2[]> physicsShapes)
	{
		for (int i = 0; i < physicsShapes.Count; i++)
		{
			Vector2[] array = physicsShapes[i];
			if (array == null)
			{
				throw new ArgumentNullException($"Physics Shape at {i} is null.");
			}
			if (array.Length < 3)
			{
				throw new ArgumentException($"Physics Shape at {i} has less than 3 vertices ({array.Length}).");
			}
		}
		OverridePhysicsShapeCount(this, physicsShapes.Count);
		for (int j = 0; j < physicsShapes.Count; j++)
		{
			OverridePhysicsShape(this, physicsShapes[j], j);
		}
	}

	[FreeFunction("CreateSpriteWithoutTextureScripting")]
	internal static Sprite Create(Rect rect, Vector2 pivot, float pixelsToUnits, Texture2D texture = null)
	{
		return Create_Injected(ref rect, ref pivot, pixelsToUnits, texture);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("SpritesBindings::OverridePhysicsShapeCount")]
	private static extern void OverridePhysicsShapeCount(Sprite sprite, int physicsShapeCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("SpritesBindings::OverridePhysicsShape", ThrowsException = true)]
	private static extern void OverridePhysicsShape(Sprite sprite, Vector2[] physicsShape, int idx);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Sprite Create_Injected(ref Rect rect, ref Vector2 pivot, float pixelsToUnits, Texture2D texture = null);
}
