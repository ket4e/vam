using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps;

/// <summary>
///   <para>A Struct for the required data for animating a Tile.</para>
/// </summary>
[NativeType(Header = "Modules/Tilemap/TilemapScripting.h")]
[RequiredByNativeCode]
public struct TileAnimationData
{
	private Sprite[] m_AnimatedSprites;

	private float m_AnimationSpeed;

	private float m_AnimationStartTime;

	/// <summary>
	///   <para>The array of that are ordered by appearance in the animation.</para>
	/// </summary>
	public Sprite[] animatedSprites
	{
		get
		{
			return m_AnimatedSprites;
		}
		set
		{
			m_AnimatedSprites = value;
		}
	}

	/// <summary>
	///   <para>The animation speed.</para>
	/// </summary>
	public float animationSpeed
	{
		get
		{
			return m_AnimationSpeed;
		}
		set
		{
			m_AnimationSpeed = value;
		}
	}

	/// <summary>
	///   <para>The start time of the animation. The animation will begin at this time offset.</para>
	/// </summary>
	public float animationStartTime
	{
		get
		{
			return m_AnimationStartTime;
		}
		set
		{
			m_AnimationStartTime = value;
		}
	}
}
