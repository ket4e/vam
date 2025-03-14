using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

[ExecuteInEditMode]
public class MirrorReflection : JSONStorable
{
	public MirrorReflection slaveReflection;

	protected JSONStorableBool disablePixelLightsJSON;

	[SerializeField]
	protected bool _disablePixelLights;

	protected JSONStorableStringChooser textureSizeJSON;

	protected int _oldReflectionTextureSize;

	[SerializeField]
	protected int _textureSize = 1024;

	protected JSONStorableStringChooser antiAliasingJSON;

	protected int _oldAntiAliasing;

	[SerializeField]
	protected int _antiAliasing = 8;

	protected JSONStorableFloat reflectionOpacityJSON;

	[SerializeField]
	protected float _reflectionOpacity = 0.5f;

	protected JSONStorableFloat reflectionBlendJSON;

	[SerializeField]
	protected float _reflectionBlend = 1f;

	protected JSONStorableFloat surfaceTexturePowerJSON;

	[SerializeField]
	protected float _surfaceTexturePower = 1f;

	protected JSONStorableFloat specularIntensityJSON;

	[SerializeField]
	protected float _specularIntensity = 1f;

	protected JSONStorableColor reflectionColorJSON;

	protected HSVColor _currentReflectionHSVColor;

	protected Color _currentReflectionColor;

	protected JSONStorableFloat renderQueueJSON;

	public static bool globalEnabled = true;

	public Transform altObjectWhenMirrorDisabled;

	public bool useSameMaterialWhenMirrorDisabled;

	public float m_ClipPlaneOffset;

	public LayerMask m_ReflectLayers = -1;

	public bool m_UseObliqueClip = true;

	public bool renderBackside;

	protected Hashtable m_ReflectionCameras = new Hashtable();

	protected RenderTexture m_ReflectionTextureLeft;

	protected RenderTexture m_ReflectionTextureRight;

	protected static bool s_InsideRendering;

	public bool disablePixelLights
	{
		get
		{
			return _disablePixelLights;
		}
		set
		{
			if (disablePixelLightsJSON != null)
			{
				disablePixelLightsJSON.val = value;
			}
			else if (_disablePixelLights != value)
			{
				SyncDisablePixelLights(value);
			}
		}
	}

	public int textureSize
	{
		get
		{
			return _textureSize;
		}
		set
		{
			if (textureSizeJSON != null)
			{
				textureSizeJSON.val = value.ToString();
			}
			else if (_textureSize != value && (value == 512 || value == 1024 || value == 2048 || value == 4096))
			{
				SetTextureSizeFromString(value.ToString());
			}
		}
	}

	public int antiAliasing
	{
		get
		{
			return _antiAliasing;
		}
		set
		{
			if (antiAliasingJSON != null)
			{
				antiAliasingJSON.val = value.ToString();
			}
			else if (_antiAliasing != value && (value == 1 || value == 2 || value == 4 || value == 8))
			{
				SetAntialiasingFromString(value.ToString());
			}
		}
	}

	public float reflectionOpacity
	{
		get
		{
			return _reflectionOpacity;
		}
		set
		{
			if (reflectionOpacityJSON != null)
			{
				reflectionOpacityJSON.val = value;
			}
			else if (_reflectionOpacity != value)
			{
				SyncReflectionOpacity(value);
			}
		}
	}

	public float reflectionBlend
	{
		get
		{
			return _reflectionBlend;
		}
		set
		{
			if (reflectionBlendJSON != null)
			{
				reflectionBlendJSON.val = value;
			}
			else if (_reflectionBlend != value)
			{
				SyncReflectionBlend(value);
			}
		}
	}

	public float surfaceTexturePower
	{
		get
		{
			return _surfaceTexturePower;
		}
		set
		{
			if (surfaceTexturePowerJSON != null)
			{
				surfaceTexturePowerJSON.val = value;
			}
			else if (_surfaceTexturePower != value)
			{
				SyncSurfaceTexturePower(value);
			}
		}
	}

	public float specularIntensity
	{
		get
		{
			return _specularIntensity;
		}
		set
		{
			if (specularIntensityJSON != null)
			{
				specularIntensityJSON.val = value;
			}
			else if (_specularIntensity != value)
			{
				SyncSpecularIntensity(value);
			}
		}
	}

	public int renderQueue
	{
		get
		{
			if (renderQueueJSON != null)
			{
				return Mathf.FloorToInt(renderQueueJSON.val);
			}
			return 0;
		}
		set
		{
			if (renderQueueJSON != null)
			{
				renderQueueJSON.val = value;
			}
		}
	}

	protected void SyncDisablePixelLights(bool b)
	{
		_disablePixelLights = b;
		if (slaveReflection != null)
		{
			slaveReflection.disablePixelLights = b;
		}
	}

	protected void SetTextureSizeFromString(string size)
	{
		try
		{
			int num = int.Parse(size);
			if (num == 512 || num == 1024 || num == 2048 || num == 4096)
			{
				_textureSize = num;
				if (slaveReflection != null)
				{
					slaveReflection.textureSize = _textureSize;
				}
			}
			else
			{
				if (textureSizeJSON != null)
				{
					textureSizeJSON.valNoCallback = _textureSize.ToString();
				}
				Debug.LogError("Attempted to set texture size to " + size + " which is not a valid value of 512, 1024, 2048, 4096");
			}
		}
		catch (FormatException)
		{
			Debug.LogError("Attempted to set texture size to " + size + " which is not a valid integer");
		}
	}

	protected void SetAntialiasingFromString(string aa)
	{
		try
		{
			int num = int.Parse(aa);
			if (num == 1 || num == 2 || num == 4 || num == 8)
			{
				_antiAliasing = num;
				if (slaveReflection != null)
				{
					slaveReflection.antiAliasing = _antiAliasing;
				}
			}
			else
			{
				if (antiAliasingJSON != null)
				{
					antiAliasingJSON.valNoCallback = _antiAliasing.ToString();
				}
				Debug.LogError("Attempted to set antialiasing to " + aa + " which is not a valid value of 1, 2, 4, or 8");
			}
		}
		catch (FormatException)
		{
			Debug.LogError("Attempted to set antialiasing to " + aa + " which is not a valid integer");
		}
	}

	protected bool MaterialHasProp(string propName)
	{
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			Material material = ((!Application.isPlaying) ? component.sharedMaterial : component.material);
			if (material.HasProperty(propName))
			{
				return true;
			}
		}
		return false;
	}

	protected void SetMaterialProp(string propName, float propValue)
	{
		Renderer component = GetComponent<Renderer>();
		if (!(component != null))
		{
			return;
		}
		Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
		Material[] array2 = array;
		foreach (Material material in array2)
		{
			if (material.HasProperty(propName))
			{
				material.SetFloat(propName, propValue);
			}
		}
	}

	protected void SyncReflectionOpacity(float f)
	{
		_reflectionOpacity = f;
		SetMaterialProp("_ReflectionOpacity", _reflectionOpacity);
		if (slaveReflection != null)
		{
			slaveReflection.reflectionOpacity = f;
		}
	}

	protected void SyncReflectionBlend(float f)
	{
		_reflectionBlend = f;
		SetMaterialProp("_ReflectionBlendTexPower", _reflectionBlend);
		if (slaveReflection != null)
		{
			slaveReflection.reflectionBlend = f;
		}
	}

	protected void SyncSurfaceTexturePower(float f)
	{
		_surfaceTexturePower = f;
		SetMaterialProp("_MainTexPower", _surfaceTexturePower);
		if (slaveReflection != null)
		{
			slaveReflection.surfaceTexturePower = f;
		}
	}

	protected void SyncSpecularIntensity(float f)
	{
		_specularIntensity = f;
		SetMaterialProp("_SpecularIntensity", _specularIntensity);
		if (slaveReflection != null)
		{
			slaveReflection.specularIntensity = f;
		}
	}

	public void SetReflectionMaterialColor(Color c)
	{
		Renderer component = GetComponent<Renderer>();
		if (!(component != null))
		{
			return;
		}
		Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
		Material[] array2 = array;
		foreach (Material material in array2)
		{
			if (material.HasProperty("_ReflectionColor"))
			{
				material.SetColor("_ReflectionColor", c);
			}
		}
	}

	protected void SyncReflectionColor(float h, float s, float v)
	{
		_currentReflectionHSVColor.H = h;
		_currentReflectionHSVColor.S = s;
		_currentReflectionHSVColor.V = v;
		_currentReflectionColor = HSVColorPicker.HSVToRGB(h, s, v);
		SetReflectionMaterialColor(_currentReflectionColor);
		if (slaveReflection != null)
		{
			slaveReflection.SetReflectionColor(_currentReflectionHSVColor);
		}
	}

	public void SetReflectionColor(HSVColor hsvColor)
	{
		if (reflectionColorJSON != null)
		{
			reflectionColorJSON.val = hsvColor;
		}
		else
		{
			SyncReflectionColor(hsvColor.H, hsvColor.S, hsvColor.V);
		}
	}

	protected void SyncRenderQueue(float f)
	{
		int num = Mathf.FloorToInt(f);
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
			Material[] array2 = array;
			foreach (Material material in array2)
			{
				material.renderQueue = num;
			}
		}
		if (slaveReflection != null)
		{
			slaveReflection.renderQueue = num;
		}
	}

	private void RenderMirror(Camera reflectionCamera)
	{
		Vector3 position = base.transform.position;
		Vector3 up = base.transform.up;
		float w = 0f - Vector3.Dot(up, position) - m_ClipPlaneOffset;
		Vector4 plane = new Vector4(up.x, up.y, up.z, w);
		Matrix4x4 reflectionMat = Matrix4x4.zero;
		CalculateReflectionMatrix(ref reflectionMat, plane);
		reflectionCamera.worldToCameraMatrix *= reflectionMat;
		if (m_UseObliqueClip)
		{
			Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, up, 1f);
			reflectionCamera.projectionMatrix = reflectionCamera.CalculateObliqueMatrix(clipPlane);
		}
		reflectionCamera.cullingMask = -17 & m_ReflectLayers.value;
		GL.invertCulling = true;
		reflectionCamera.transform.position = reflectionCamera.cameraToWorldMatrix.GetPosition();
		reflectionCamera.transform.rotation = reflectionCamera.cameraToWorldMatrix.GetRotation();
		reflectionCamera.Render();
		GL.invertCulling = false;
	}

	public void OnWillRenderObject()
	{
		Renderer component = GetComponent<Renderer>();
		if (!base.enabled || !component || !component.sharedMaterial || !component.enabled || !globalEnabled)
		{
			return;
		}
		Camera current = Camera.current;
		if (!current)
		{
			return;
		}
		Vector3 rhs = current.transform.position - base.transform.position;
		if (!renderBackside)
		{
			float num = Vector3.Dot(base.transform.up, rhs);
			if (num <= 0.001f)
			{
				return;
			}
		}
		if (s_InsideRendering)
		{
			return;
		}
		s_InsideRendering = true;
		CreateMirrorObjects(current, out var reflectionCamera);
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (_disablePixelLights)
		{
			QualitySettings.pixelLightCount = 0;
		}
		UpdateCameraModes(current, reflectionCamera);
		Vector3 position = current.transform.position;
		if (current.stereoEnabled)
		{
			if (current.stereoTargetEye == StereoTargetEyeMask.Both)
			{
				reflectionCamera.ResetWorldToCameraMatrix();
				if (CameraTarget.rightTarget != null && CameraTarget.rightTarget.targetCamera != null && current.transform.parent != null)
				{
					reflectionCamera.transform.position = current.transform.parent.TransformPoint(InputTracking.GetLocalPosition(XRNode.RightEye));
					reflectionCamera.transform.rotation = current.transform.parent.rotation * InputTracking.GetLocalRotation(XRNode.RightEye);
					reflectionCamera.worldToCameraMatrix = CameraTarget.rightTarget.worldToCameraMatrix;
					reflectionCamera.projectionMatrix = CameraTarget.rightTarget.projectionMatrix;
				}
				else
				{
					reflectionCamera.transform.position = current.transform.parent.TransformPoint(InputTracking.GetLocalPosition(XRNode.RightEye));
					reflectionCamera.transform.rotation = current.transform.parent.rotation * InputTracking.GetLocalRotation(XRNode.RightEye);
					reflectionCamera.worldToCameraMatrix = current.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
					reflectionCamera.projectionMatrix = current.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
				}
				reflectionCamera.targetTexture = m_ReflectionTextureRight;
				RenderMirror(reflectionCamera);
				reflectionCamera.ResetWorldToCameraMatrix();
				if (CameraTarget.leftTarget != null && CameraTarget.leftTarget.targetCamera != null && current.transform.parent != null)
				{
					reflectionCamera.transform.position = current.transform.parent.TransformPoint(InputTracking.GetLocalPosition(XRNode.LeftEye));
					reflectionCamera.transform.rotation = current.transform.parent.rotation * InputTracking.GetLocalRotation(XRNode.LeftEye);
					reflectionCamera.worldToCameraMatrix = CameraTarget.leftTarget.worldToCameraMatrix;
					reflectionCamera.projectionMatrix = CameraTarget.leftTarget.projectionMatrix;
				}
				else
				{
					reflectionCamera.transform.position = current.transform.parent.TransformPoint(InputTracking.GetLocalPosition(XRNode.LeftEye));
					reflectionCamera.transform.rotation = current.transform.parent.rotation * InputTracking.GetLocalRotation(XRNode.LeftEye);
					reflectionCamera.worldToCameraMatrix = current.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
					reflectionCamera.projectionMatrix = current.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
				}
				position = reflectionCamera.transform.position;
				reflectionCamera.targetTexture = m_ReflectionTextureLeft;
				RenderMirror(reflectionCamera);
			}
			else if (current.stereoTargetEye == StereoTargetEyeMask.Left)
			{
				reflectionCamera.ResetWorldToCameraMatrix();
				reflectionCamera.transform.position = current.transform.position;
				reflectionCamera.transform.rotation = current.transform.rotation;
				reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix;
				reflectionCamera.projectionMatrix = current.projectionMatrix;
				reflectionCamera.targetTexture = m_ReflectionTextureLeft;
				RenderMirror(reflectionCamera);
			}
			else if (current.stereoTargetEye == StereoTargetEyeMask.Right)
			{
				reflectionCamera.ResetWorldToCameraMatrix();
				reflectionCamera.transform.position = current.transform.position;
				reflectionCamera.transform.rotation = current.transform.rotation;
				reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix;
				reflectionCamera.projectionMatrix = current.projectionMatrix;
				reflectionCamera.targetTexture = m_ReflectionTextureLeft;
				RenderMirror(reflectionCamera);
			}
		}
		else
		{
			reflectionCamera.ResetWorldToCameraMatrix();
			reflectionCamera.transform.position = current.transform.position;
			reflectionCamera.transform.rotation = current.transform.rotation;
			reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix;
			reflectionCamera.projectionMatrix = current.projectionMatrix;
			reflectionCamera.targetTexture = m_ReflectionTextureLeft;
			RenderMirror(reflectionCamera);
		}
		Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
		Vector4 value = default(Vector4);
		value.x = position.x;
		value.y = position.y;
		value.z = position.z;
		value.w = 0f;
		Material[] array2 = array;
		foreach (Material material in array2)
		{
			if (material.HasProperty("_ReflectionTex"))
			{
				material.SetTexture("_ReflectionTex", m_ReflectionTextureLeft);
			}
			if (material.HasProperty("_LeftReflectionTex"))
			{
				material.SetTexture("_LeftReflectionTex", m_ReflectionTextureLeft);
			}
			if (material.HasProperty("_RightReflectionTex"))
			{
				material.SetTexture("_RightReflectionTex", m_ReflectionTextureRight);
			}
			if (material.HasProperty("_LeftCameraPosition"))
			{
				material.SetVector("_LeftCameraPosition", value);
			}
		}
		if (_disablePixelLights)
		{
			QualitySettings.pixelLightCount = pixelLightCount;
		}
		s_InsideRendering = false;
	}

	protected void UpdateCameraModes(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox skybox = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox skybox2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!skybox || !skybox.material)
			{
				skybox2.enabled = false;
			}
			else
			{
				skybox2.enabled = true;
				skybox2.material = skybox.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	protected void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera)
	{
		reflectionCamera = null;
		if (!m_ReflectionTextureRight || !m_ReflectionTextureLeft || _oldReflectionTextureSize != _textureSize || _oldAntiAliasing != _antiAliasing)
		{
			if ((bool)m_ReflectionTextureLeft)
			{
				UnityEngine.Object.DestroyImmediate(m_ReflectionTextureLeft);
			}
			m_ReflectionTextureLeft = new RenderTexture(_textureSize, _textureSize, 24);
			m_ReflectionTextureLeft.name = "__MirrorReflectionLeft" + GetInstanceID();
			m_ReflectionTextureLeft.antiAliasing = _antiAliasing;
			m_ReflectionTextureLeft.isPowerOfTwo = true;
			m_ReflectionTextureLeft.hideFlags = HideFlags.DontSave;
			if ((bool)m_ReflectionTextureRight)
			{
				UnityEngine.Object.DestroyImmediate(m_ReflectionTextureRight);
			}
			m_ReflectionTextureRight = new RenderTexture(_textureSize, _textureSize, 24);
			m_ReflectionTextureRight.name = "__MirrorReflectionRight" + GetInstanceID();
			m_ReflectionTextureRight.antiAliasing = _antiAliasing;
			m_ReflectionTextureRight.isPowerOfTwo = true;
			m_ReflectionTextureRight.hideFlags = HideFlags.DontSave;
			_oldReflectionTextureSize = _textureSize;
			_oldAntiAliasing = _antiAliasing;
		}
		reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;
		if (!reflectionCamera)
		{
			GameObject gameObject = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
			reflectionCamera = gameObject.GetComponent<Camera>();
			reflectionCamera.enabled = false;
			reflectionCamera.transform.position = base.transform.position;
			reflectionCamera.transform.rotation = base.transform.rotation;
			reflectionCamera.gameObject.AddComponent<FlareLayer>();
			gameObject.hideFlags = HideFlags.DontSave;
			m_ReflectionCameras[currentCamera] = reflectionCamera;
		}
	}

	protected static float sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	protected Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
	}

	protected static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}

	protected void Init()
	{
		Material material = null;
		Renderer component = GetComponent<Renderer>();
		if (!(component != null))
		{
			return;
		}
		Material[] materials = component.materials;
		if (materials != null)
		{
			material = materials[0];
		}
		if (material != null && material.HasProperty("_ReflectionColor"))
		{
			Color color = material.GetColor("_ReflectionColor");
			_currentReflectionHSVColor = HSVColorPicker.RGBToHSV(color.r, color.g, color.b);
		}
		else
		{
			_currentReflectionHSVColor = default(HSVColor);
			_currentReflectionHSVColor.H = 1f;
			_currentReflectionHSVColor.S = 1f;
			_currentReflectionHSVColor.V = 1f;
		}
		SyncReflectionColor(_currentReflectionHSVColor.H, _currentReflectionHSVColor.S, _currentReflectionHSVColor.V);
		reflectionColorJSON = new JSONStorableColor("reflectionColor", _currentReflectionHSVColor, SyncReflectionColor);
		RegisterColor(reflectionColorJSON);
		if (material != null)
		{
			if (material.HasProperty("_ReflectionOpacity"))
			{
				SyncReflectionOpacity(material.GetFloat("_ReflectionOpacity"));
				reflectionOpacityJSON = new JSONStorableFloat("reflectionOpacity", _reflectionOpacity, SyncReflectionOpacity, 0f, 1f);
				RegisterFloat(reflectionOpacityJSON);
			}
			if (material.HasProperty("_ReflectionBlendTexPower"))
			{
				SyncReflectionBlend(material.GetFloat("_ReflectionBlendTexPower"));
				reflectionBlendJSON = new JSONStorableFloat("reflectionBlend", _reflectionBlend, SyncReflectionBlend, 0f, 2f);
				RegisterFloat(reflectionBlendJSON);
			}
			if (material.HasProperty("_MainTexPower"))
			{
				SyncSurfaceTexturePower(material.GetFloat("_MainTexPower"));
				surfaceTexturePowerJSON = new JSONStorableFloat("surfaceTexturePower", _surfaceTexturePower, SyncSurfaceTexturePower, 0f, 1f);
				RegisterFloat(surfaceTexturePowerJSON);
			}
			if (material.HasProperty("_SpecularIntensity"))
			{
				SyncSpecularIntensity(material.GetFloat("_SpecularIntensity"));
				specularIntensityJSON = new JSONStorableFloat("specularIntensity", _specularIntensity, SyncSpecularIntensity, 0f, 2f);
				RegisterFloat(specularIntensityJSON);
			}
			SyncRenderQueue(material.renderQueue);
			renderQueueJSON = new JSONStorableFloat("renderQueue", material.renderQueue, SyncRenderQueue, -1f, 5000f);
			RegisterFloat(renderQueueJSON);
		}
		List<string> list = new List<string>();
		list.Add("1");
		list.Add("2");
		list.Add("4");
		list.Add("8");
		antiAliasingJSON = new JSONStorableStringChooser("antiAliasing", list, _antiAliasing.ToString(), "Anti-aliasing", SetAntialiasingFromString);
		RegisterStringChooser(antiAliasingJSON);
		disablePixelLightsJSON = new JSONStorableBool("disablePixelLights", _disablePixelLights, SyncDisablePixelLights);
		RegisterBool(disablePixelLightsJSON);
		List<string> list2 = new List<string>();
		list2.Add("512");
		list2.Add("1024");
		list2.Add("2048");
		list2.Add("4096");
		textureSizeJSON = new JSONStorableStringChooser("textureSize", list2, _textureSize.ToString(), "Texture Size", SetTextureSizeFromString);
		RegisterStringChooser(textureSizeJSON);
	}

	public override void InitUI()
	{
		if (!(UITransform != null))
		{
			return;
		}
		MirrorReflectionUI componentInChildren = UITransform.GetComponentInChildren<MirrorReflectionUI>(includeInactive: true);
		if (componentInChildren != null)
		{
			disablePixelLightsJSON.toggle = componentInChildren.disablePixelLightsToggle;
			reflectionColorJSON.colorPicker = componentInChildren.reflectionColorPicker;
			if (reflectionOpacityJSON != null)
			{
				reflectionOpacityJSON.slider = componentInChildren.reflectionOpacitySlider;
			}
			else if (componentInChildren.reflectionOpacityContainer != null)
			{
				componentInChildren.reflectionOpacityContainer.gameObject.SetActive(value: false);
			}
			if (reflectionBlendJSON != null)
			{
				reflectionBlendJSON.slider = componentInChildren.reflectionBlendSlider;
			}
			else if (componentInChildren.reflectionBlendContainer != null)
			{
				componentInChildren.reflectionBlendContainer.gameObject.SetActive(value: false);
			}
			if (surfaceTexturePowerJSON != null)
			{
				surfaceTexturePowerJSON.slider = componentInChildren.surfaceTexturePowerSlider;
			}
			else if (componentInChildren.surfaceTexturePowerContainer != null)
			{
				componentInChildren.surfaceTexturePowerContainer.gameObject.SetActive(value: false);
			}
			if (specularIntensityJSON != null)
			{
				specularIntensityJSON.slider = componentInChildren.specularIntensitySlider;
			}
			else if (componentInChildren.specularIntensityContainer != null)
			{
				componentInChildren.specularIntensityContainer.gameObject.SetActive(value: false);
			}
			if (renderQueueJSON != null)
			{
				renderQueueJSON.slider = componentInChildren.renderQueueSlider;
			}
			antiAliasingJSON.popup = componentInChildren.antiAliasingPopup;
			textureSizeJSON.popup = componentInChildren.textureSizePopup;
		}
	}

	public override void InitUIAlt()
	{
		if (!(UITransformAlt != null))
		{
			return;
		}
		MirrorReflectionUI componentInChildren = UITransformAlt.GetComponentInChildren<MirrorReflectionUI>(includeInactive: true);
		if (componentInChildren != null)
		{
			disablePixelLightsJSON.toggleAlt = componentInChildren.disablePixelLightsToggle;
			reflectionColorJSON.colorPickerAlt = componentInChildren.reflectionColorPicker;
			if (reflectionOpacityJSON != null)
			{
				reflectionOpacityJSON.sliderAlt = componentInChildren.reflectionOpacitySlider;
			}
			else if (componentInChildren.reflectionOpacityContainer != null)
			{
				componentInChildren.reflectionOpacityContainer.gameObject.SetActive(value: false);
			}
			if (reflectionBlendJSON != null)
			{
				reflectionBlendJSON.sliderAlt = componentInChildren.reflectionBlendSlider;
			}
			else if (componentInChildren.reflectionBlendContainer != null)
			{
				componentInChildren.reflectionBlendContainer.gameObject.SetActive(value: false);
			}
			if (surfaceTexturePowerJSON != null)
			{
				surfaceTexturePowerJSON.sliderAlt = componentInChildren.surfaceTexturePowerSlider;
			}
			else if (componentInChildren.surfaceTexturePowerContainer != null)
			{
				componentInChildren.surfaceTexturePowerContainer.gameObject.SetActive(value: false);
			}
			if (specularIntensityJSON != null)
			{
				specularIntensityJSON.sliderAlt = componentInChildren.specularIntensitySlider;
			}
			else if (componentInChildren.specularIntensityContainer != null)
			{
				componentInChildren.specularIntensityContainer.gameObject.SetActive(value: false);
			}
			if (renderQueueJSON != null)
			{
				renderQueueJSON.sliderAlt = componentInChildren.renderQueueSlider;
			}
			antiAliasingJSON.popupAlt = componentInChildren.antiAliasingPopup;
			textureSizeJSON.popupAlt = componentInChildren.textureSizePopup;
		}
	}

	private void OnDisable()
	{
		if ((bool)m_ReflectionTextureRight)
		{
			UnityEngine.Object.DestroyImmediate(m_ReflectionTextureRight);
			m_ReflectionTextureRight = null;
		}
		if ((bool)m_ReflectionTextureLeft)
		{
			UnityEngine.Object.DestroyImmediate(m_ReflectionTextureLeft);
			m_ReflectionTextureLeft = null;
		}
		foreach (DictionaryEntry reflectionCamera in m_ReflectionCameras)
		{
			UnityEngine.Object.DestroyImmediate(((Camera)reflectionCamera.Value).gameObject);
		}
		m_ReflectionCameras.Clear();
	}

	protected override void Awake()
	{
		if (!awakecalled)
		{
			base.Awake();
			if (Application.isPlaying)
			{
				Init();
				InitUI();
				InitUIAlt();
			}
		}
	}

	private void Update()
	{
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			component.enabled = (globalEnabled || useSameMaterialWhenMirrorDisabled) && (containingAtom == null || (!containingAtom.globalDisableRender && !containingAtom.tempDisableRender));
		}
		if (useSameMaterialWhenMirrorDisabled)
		{
			if (globalEnabled)
			{
				return;
			}
			Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
			Material[] array2 = array;
			foreach (Material material in array2)
			{
				if (material.HasProperty("_ReflectionTex"))
				{
					material.SetTexture("_ReflectionTex", null);
				}
				if (material.HasProperty("_LeftReflectionTex"))
				{
					material.SetTexture("_LeftReflectionTex", null);
				}
				if (material.HasProperty("_RightReflectionTex"))
				{
					material.SetTexture("_RightReflectionTex", null);
				}
			}
		}
		else if (altObjectWhenMirrorDisabled != null)
		{
			altObjectWhenMirrorDisabled.gameObject.SetActive(!globalEnabled);
		}
	}
}
