using UnityEngine.Scripting;

namespace UnityEngine.Experimental.GlobalIllumination;

/// <summary>
///   <para>The interop structure to pass light information to the light baking backends. There are helper structures for Directional, Point, Spot and Rectangle lights to correctly initialize this structure.</para>
/// </summary>
[UsedByNativeCode]
public struct LightDataGI
{
	/// <summary>
	///   <para>The light's instanceID.</para>
	/// </summary>
	public int instanceID;

	/// <summary>
	///   <para>The color of the light.</para>
	/// </summary>
	public LinearColor color;

	/// <summary>
	///   <para>The indirect color of the light.</para>
	/// </summary>
	public LinearColor indirectColor;

	/// <summary>
	///   <para>The orientation of the light.</para>
	/// </summary>
	public Quaternion orientation;

	/// <summary>
	///   <para>The position of the light.</para>
	/// </summary>
	public Vector3 position;

	/// <summary>
	///   <para>The range of the light. Unused for directional lights.</para>
	/// </summary>
	public float range;

	/// <summary>
	///   <para>The cone angle for spot lights.</para>
	/// </summary>
	public float coneAngle;

	/// <summary>
	///   <para>The inner cone angle for spot lights.</para>
	/// </summary>
	public float innerConeAngle;

	/// <summary>
	///   <para>The light's sphere radius for point and spot lights, or the width for rectangle lights.</para>
	/// </summary>
	public float shape0;

	/// <summary>
	///   <para>The height for rectangle lights.</para>
	/// </summary>
	public float shape1;

	/// <summary>
	///   <para>The type of the light.</para>
	/// </summary>
	public LightType type;

	/// <summary>
	///   <para>The lightmap mode for the light.</para>
	/// </summary>
	public LightMode mode;

	/// <summary>
	///   <para>Set to 1 for shadow casting lights, 0 otherwise.</para>
	/// </summary>
	public byte shadow;

	/// <summary>
	///   <para>The falloff model to use for baking point and spot lights.</para>
	/// </summary>
	public FalloffType falloff;

	public void Init(ref DirectionalLight light)
	{
		instanceID = light.instanceID;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation.SetLookRotation(light.direction, Vector3.up);
		position = Vector3.zero;
		range = 0f;
		coneAngle = 0f;
		innerConeAngle = 0f;
		shape0 = light.penumbraWidthRadian;
		shape1 = 0f;
		type = LightType.Directional;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = FalloffType.Undefined;
	}

	public void Init(ref PointLight light)
	{
		instanceID = light.instanceID;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation = Quaternion.identity;
		position = light.position;
		range = light.range;
		coneAngle = 0f;
		innerConeAngle = 0f;
		shape0 = light.sphereRadius;
		shape1 = 0f;
		type = LightType.Point;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = light.falloff;
	}

	public void Init(ref SpotLight light)
	{
		instanceID = light.instanceID;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation = light.orientation;
		position = light.position;
		range = light.range;
		coneAngle = light.coneAngle;
		innerConeAngle = light.innerConeAngle;
		shape0 = light.sphereRadius;
		shape1 = 0f;
		type = LightType.Spot;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = light.falloff;
	}

	public void Init(ref RectangleLight light)
	{
		instanceID = light.instanceID;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation = light.orientation;
		position = light.position;
		range = light.range;
		coneAngle = 0f;
		innerConeAngle = 0f;
		shape0 = light.width;
		shape1 = light.height;
		type = LightType.Rectangle;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = FalloffType.Undefined;
	}

	/// <summary>
	///   <para>Initialize a light so that the baking backends ignore it.</para>
	/// </summary>
	/// <param name="lightInstanceID"></param>
	public void InitNoBake(int lightInstanceID)
	{
		instanceID = lightInstanceID;
		mode = LightMode.Unknown;
	}
}
