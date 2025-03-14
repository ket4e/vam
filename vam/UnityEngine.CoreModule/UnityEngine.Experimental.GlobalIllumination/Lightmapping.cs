using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.GlobalIllumination;

/// <summary>
///   <para>Interface to the light baking backends.</para>
/// </summary>
public static class Lightmapping
{
	/// <summary>
	///   <para>Delegate called when converting lights into a form that the baking backends understand.</para>
	/// </summary>
	/// <param name="requests">The list of lights to be converted.</param>
	/// <param name="lightsOutput">The output generated by the delegate function. Lights that should be skipped must be added to the output, initialized with InitNoBake on the LightDataGI structure.</param>
	public delegate void RequestLightsDelegate(Light[] requests, NativeArray<LightDataGI> lightsOutput);

	private static readonly RequestLightsDelegate s_DefaultDelegate = delegate(Light[] requests, NativeArray<LightDataGI> lightsOutput)
	{
		DirectionalLight dir = default(DirectionalLight);
		PointLight point = default(PointLight);
		SpotLight spot = default(SpotLight);
		RectangleLight rect = default(RectangleLight);
		LightDataGI value = default(LightDataGI);
		for (int i = 0; i < requests.Length; i++)
		{
			Light light = requests[i];
			switch (light.type)
			{
			case UnityEngine.LightType.Directional:
				LightmapperUtils.Extract(light, ref dir);
				value.Init(ref dir);
				break;
			case UnityEngine.LightType.Point:
				LightmapperUtils.Extract(light, ref point);
				value.Init(ref point);
				break;
			case UnityEngine.LightType.Spot:
				LightmapperUtils.Extract(light, ref spot);
				value.Init(ref spot);
				break;
			case UnityEngine.LightType.Area:
				LightmapperUtils.Extract(light, ref rect);
				value.Init(ref rect);
				break;
			default:
				value.InitNoBake(light.GetInstanceID());
				break;
			}
			lightsOutput[i] = value;
		}
	};

	private static RequestLightsDelegate s_RequestLightsDelegate = s_DefaultDelegate;

	public static void SetDelegate(RequestLightsDelegate del)
	{
		s_RequestLightsDelegate = ((del == null) ? s_DefaultDelegate : del);
	}

	/// <summary>
	///   <para>Resets the light conversion delegate to Unity's default conversion function.</para>
	/// </summary>
	public static void ResetDelegate()
	{
		s_RequestLightsDelegate = s_DefaultDelegate;
	}

	[UsedByNativeCode]
	internal unsafe static void RequestLights(Light[] lights, IntPtr outLightsPtr, int outLightsCount)
	{
		NativeArray<LightDataGI> lightsOutput = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<LightDataGI>((void*)outLightsPtr, outLightsCount, Allocator.None);
		s_RequestLightsDelegate(lights, lightsOutput);
	}
}
