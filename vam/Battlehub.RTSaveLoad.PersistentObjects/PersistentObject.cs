using System;
using System.Collections.Generic;
using Battlehub.RTSaveLoad.PersistentObjects.AI;
using Battlehub.RTSaveLoad.PersistentObjects.Audio;
using Battlehub.RTSaveLoad.PersistentObjects.Rendering;
using Battlehub.RTSaveLoad.PersistentObjects.Video;
using ProtoBuf;
using UnityEngine;

namespace Battlehub.RTSaveLoad.PersistentObjects;

[Serializable]
[ProtoContract(AsReferenceDefault = true, ImplicitFields = ImplicitFields.AllFields)]
[ProtoInclude(1159, typeof(PersistentAssetBundle))]
[ProtoInclude(1160, typeof(PersistentAssetBundleManifest))]
[ProtoInclude(1161, typeof(PersistentScriptableObject))]
[ProtoInclude(1162, typeof(PersistentBillboardAsset))]
[ProtoInclude(1163, typeof(PersistentComponent))]
[ProtoInclude(1164, typeof(PersistentComputeShader))]
[ProtoInclude(1165, typeof(PersistentGameObject))]
[ProtoInclude(1166, typeof(PersistentRenderSettings))]
[ProtoInclude(1167, typeof(PersistentQualitySettings))]
[ProtoInclude(1168, typeof(PersistentFlare))]
[ProtoInclude(1169, typeof(PersistentLightProbes))]
[ProtoInclude(1170, typeof(PersistentLightmapSettings))]
[ProtoInclude(1171, typeof(PersistentMesh))]
[ProtoInclude(1172, typeof(PersistentGraphicsSettings))]
[ProtoInclude(1173, typeof(PersistentShader))]
[ProtoInclude(1174, typeof(PersistentMaterial))]
[ProtoInclude(1175, typeof(PersistentShaderVariantCollection))]
[ProtoInclude(1176, typeof(PersistentSprite))]
[ProtoInclude(1177, typeof(PersistentTextAsset))]
[ProtoInclude(1178, typeof(PersistentTexture))]
[ProtoInclude(1179, typeof(PersistentPhysicMaterial))]
[ProtoInclude(1180, typeof(PersistentPhysicsMaterial2D))]
[ProtoInclude(1181, typeof(PersistentNavMeshData))]
[ProtoInclude(1182, typeof(PersistentAudioClip))]
[ProtoInclude(1183, typeof(PersistentAudioMixer))]
[ProtoInclude(1184, typeof(PersistentAudioMixerSnapshot))]
[ProtoInclude(1185, typeof(PersistentAudioMixerGroup))]
[ProtoInclude(1186, typeof(PersistentRuntimeAnimatorController))]
[ProtoInclude(1187, typeof(PersistentAvatar))]
[ProtoInclude(1188, typeof(PersistentAvatarMask))]
[ProtoInclude(1189, typeof(PersistentMotion))]
[ProtoInclude(1190, typeof(PersistentTerrainData))]
[ProtoInclude(1191, typeof(PersistentFont))]
[ProtoInclude(1192, typeof(PersistentVideoClip))]
[ProtoInclude(99999, typeof(PersistentScript))]
public class PersistentObject : PersistentData
{
	public string name;

	public uint hideFlags;

	public override object WriteTo(object obj, Dictionary<long, UnityEngine.Object> objects)
	{
		obj = base.WriteTo(obj, objects);
		if (obj == null)
		{
			return null;
		}
		UnityEngine.Object @object = (UnityEngine.Object)obj;
		@object.name = name;
		@object.hideFlags = (HideFlags)hideFlags;
		return @object;
	}

	public override void ReadFrom(object obj)
	{
		base.ReadFrom(obj);
		if (obj != null)
		{
			UnityEngine.Object @object = (UnityEngine.Object)obj;
			name = @object.name;
			hideFlags = (uint)@object.hideFlags;
		}
	}

	public override void FindDependencies<T>(Dictionary<long, T> dependencies, Dictionary<long, T> objects, bool allowNulls)
	{
		base.FindDependencies(dependencies, objects, allowNulls);
	}
}
