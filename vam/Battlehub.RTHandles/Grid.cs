using Battlehub.RTSaveLoad;
using UnityEngine;

namespace Battlehub.RTHandles;

[RequireComponent(typeof(PersistentIgnore))]
[RequireComponent(typeof(Camera))]
public class Grid : MonoBehaviour
{
	private Camera m_camera;

	public Camera SceneCamera;

	public float CamOffset;

	public bool AutoCamOffset = true;

	public Vector3 GridOffset;

	private void Start()
	{
		m_camera = GetComponent<Camera>();
		if (SceneCamera == null)
		{
			SceneCamera = Camera.main;
		}
		if (SceneCamera == null)
		{
			Debug.LogError("SceneCamera is null");
			base.enabled = false;
			return;
		}
		m_camera.clearFlags = CameraClearFlags.Nothing;
		m_camera.renderingPath = RenderingPath.Forward;
		m_camera.cullingMask = 0;
		SetupCamera();
		if ((double)m_camera.depth != 0.01)
		{
			m_camera.depth = 0.01f;
		}
	}

	private void OnPreRender()
	{
		m_camera.farClipPlane = RuntimeHandles.GetGridFarPlane();
	}

	private void OnPostRender()
	{
		if (AutoCamOffset)
		{
			RuntimeHandles.DrawGrid(GridOffset, Camera.current.transform.position.y);
		}
		else
		{
			RuntimeHandles.DrawGrid(GridOffset, CamOffset);
		}
	}

	private void Update()
	{
		SetupCamera();
	}

	private void SetupCamera()
	{
		m_camera.transform.position = SceneCamera.transform.position;
		m_camera.transform.rotation = SceneCamera.transform.rotation;
		m_camera.transform.localScale = SceneCamera.transform.localScale;
		if (m_camera.fieldOfView != SceneCamera.fieldOfView)
		{
			m_camera.fieldOfView = SceneCamera.fieldOfView;
		}
		if (m_camera.orthographic != SceneCamera.orthographic)
		{
			m_camera.orthographic = SceneCamera.orthographic;
		}
		if (m_camera.orthographicSize != SceneCamera.orthographicSize)
		{
			m_camera.orthographicSize = SceneCamera.orthographicSize;
		}
		if (m_camera.rect != SceneCamera.rect)
		{
			m_camera.rect = SceneCamera.rect;
		}
	}
}
