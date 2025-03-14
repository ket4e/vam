using System;

namespace UnityEngine.Experimental.GlobalIllumination;

/// <summary>
///   <para>Utility class for converting Unity Lights to light types recognized by the baking backends.</para>
/// </summary>
public static class LightmapperUtils
{
	/// <summary>
	///   <para>Extracts informations from Lights.</para>
	/// </summary>
	/// <param name="baketype">The lights baketype.</param>
	/// <returns>
	///   <para>Returns the light's light mode.</para>
	/// </returns>
	public static LightMode Extract(LightmapBakeType baketype)
	{
		return baketype switch
		{
			LightmapBakeType.Realtime => LightMode.Realtime, 
			LightmapBakeType.Mixed => LightMode.Mixed, 
			_ => LightMode.Baked, 
		};
	}

	/// <summary>
	///   <para>Extracts the indirect color from a light.</para>
	/// </summary>
	/// <param name="l"></param>
	public static LinearColor ExtractIndirect(Light l)
	{
		return LinearColor.Convert(l.color, l.intensity * l.bounceIntensity);
	}

	/// <summary>
	///   <para>Extracts the inner cone angle of spot lights.</para>
	/// </summary>
	/// <param name="l"></param>
	public static float ExtractInnerCone(Light l)
	{
		return 2f * Mathf.Atan(Mathf.Tan(l.spotAngle * 0.5f * ((float)Math.PI / 180f)) * 46f / 64f);
	}

	public static void Extract(Light l, ref DirectionalLight dir)
	{
		dir.instanceID = l.GetInstanceID();
		dir.mode = LightMode.Realtime;
		dir.shadow = l.shadows != LightShadows.None;
		dir.direction = l.transform.forward;
		dir.color = LinearColor.Convert(l.color, l.intensity);
		dir.indirectColor = ExtractIndirect(l);
		dir.penumbraWidthRadian = 0f;
	}

	public static void Extract(Light l, ref PointLight point)
	{
		point.instanceID = l.GetInstanceID();
		point.mode = LightMode.Realtime;
		point.shadow = l.shadows != LightShadows.None;
		point.position = l.transform.position;
		point.color = LinearColor.Convert(l.color, l.intensity);
		point.indirectColor = ExtractIndirect(l);
		point.range = l.range;
		point.sphereRadius = 0f;
		point.falloff = FalloffType.Legacy;
	}

	public static void Extract(Light l, ref SpotLight spot)
	{
		spot.instanceID = l.GetInstanceID();
		spot.mode = LightMode.Realtime;
		spot.shadow = l.shadows != LightShadows.None;
		spot.position = l.transform.position;
		spot.orientation = l.transform.rotation;
		spot.color = LinearColor.Convert(l.color, l.intensity);
		spot.indirectColor = ExtractIndirect(l);
		spot.range = l.range;
		spot.sphereRadius = 0f;
		spot.coneAngle = l.spotAngle * ((float)Math.PI / 180f);
		spot.innerConeAngle = ExtractInnerCone(l);
		spot.falloff = FalloffType.Legacy;
	}

	public static void Extract(Light l, ref RectangleLight rect)
	{
		rect.instanceID = l.GetInstanceID();
		rect.mode = LightMode.Realtime;
		rect.shadow = l.shadows != LightShadows.None;
		rect.position = l.transform.position;
		rect.orientation = l.transform.rotation;
		rect.color = LinearColor.Convert(l.color, l.intensity);
		rect.indirectColor = ExtractIndirect(l);
		rect.range = l.range;
		rect.width = 0f;
		rect.height = 0f;
	}
}
