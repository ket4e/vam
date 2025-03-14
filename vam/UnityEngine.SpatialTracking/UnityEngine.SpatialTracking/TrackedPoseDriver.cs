using System;
using UnityEngine.XR;
using UnityEngine.XR.Tango;

namespace UnityEngine.SpatialTracking;

[Serializable]
[DefaultExecutionOrder(-30000)]
[AddComponentMenu("XR/Tracked Pose Driver")]
public class TrackedPoseDriver : MonoBehaviour
{
	public enum DeviceType
	{
		GenericXRDevice,
		GenericXRController,
		GenericXRRemote
	}

	public enum TrackedPose
	{
		LeftEye,
		RightEye,
		Center,
		Head,
		LeftPose,
		RightPose,
		ColorCamera,
		DepthCamera,
		FisheyeCamera,
		Device,
		RemotePose
	}

	public enum TrackingType
	{
		RotationAndPosition,
		RotationOnly,
		PositionOnly
	}

	public enum UpdateType
	{
		UpdateAndBeforeRender,
		Update,
		BeforeRender
	}

	[SerializeField]
	private DeviceType m_Device;

	[SerializeField]
	private TrackedPose m_PoseSource;

	[SerializeField]
	private TrackingType m_TrackingType;

	[SerializeField]
	private UpdateType m_UpdateType = UpdateType.UpdateAndBeforeRender;

	[SerializeField]
	private bool m_UseRelativeTransform = true;

	protected Pose m_OriginPose;

	public DeviceType deviceType
	{
		get
		{
			return m_Device;
		}
		internal set
		{
			m_Device = value;
		}
	}

	public TrackedPose poseSource
	{
		get
		{
			return m_PoseSource;
		}
		internal set
		{
			m_PoseSource = value;
		}
	}

	public TrackingType trackingType
	{
		get
		{
			return m_TrackingType;
		}
		set
		{
			m_TrackingType = value;
		}
	}

	public UpdateType updateType
	{
		get
		{
			return m_UpdateType;
		}
		set
		{
			m_UpdateType = value;
		}
	}

	public bool UseRelativeTransform
	{
		get
		{
			return m_UseRelativeTransform;
		}
		set
		{
			m_UseRelativeTransform = value;
		}
	}

	public Pose originPose
	{
		get
		{
			return m_OriginPose;
		}
		set
		{
			m_OriginPose = value;
		}
	}

	public bool SetPoseSource(DeviceType deviceType, TrackedPose pose)
	{
		if ((int)deviceType < TrackedPoseDriverDataDescription.DeviceData.Count)
		{
			TrackedPoseDriverDataDescription.PoseData poseData = TrackedPoseDriverDataDescription.DeviceData[(int)deviceType];
			for (int i = 0; i < poseData.Poses.Count; i++)
			{
				if (poseData.Poses[i] == pose)
				{
					poseSource = pose;
					return true;
				}
			}
		}
		return false;
	}

	private bool TryGetTangoPose(CoordinateFrame frame, out Pose pose)
	{
		if (TangoInputTracking.TryGetPoseAtTime(out var pose2, TangoDevice.baseCoordinateFrame, frame) && pose2.statusCode == PoseStatus.Valid)
		{
			pose.position = pose2.position;
			pose.rotation = pose2.rotation;
			return true;
		}
		pose = Pose.identity;
		return false;
	}

	private bool GetDataFromSource(DeviceType device, TrackedPose poseSource, out Pose resultPose)
	{
		switch (poseSource)
		{
		case TrackedPose.LeftEye:
			resultPose.position = InputTracking.GetLocalPosition(XRNode.LeftEye);
			resultPose.rotation = InputTracking.GetLocalRotation(XRNode.LeftEye);
			return true;
		case TrackedPose.RightEye:
			resultPose.position = InputTracking.GetLocalPosition(XRNode.RightEye);
			resultPose.rotation = InputTracking.GetLocalRotation(XRNode.RightEye);
			return true;
		case TrackedPose.Head:
			resultPose.position = InputTracking.GetLocalPosition(XRNode.Head);
			resultPose.rotation = InputTracking.GetLocalRotation(XRNode.Head);
			return true;
		case TrackedPose.Center:
			resultPose.position = InputTracking.GetLocalPosition(XRNode.CenterEye);
			resultPose.rotation = InputTracking.GetLocalRotation(XRNode.CenterEye);
			return true;
		case TrackedPose.LeftPose:
			resultPose.position = InputTracking.GetLocalPosition(XRNode.LeftHand);
			resultPose.rotation = InputTracking.GetLocalRotation(XRNode.LeftHand);
			return true;
		case TrackedPose.RightPose:
			resultPose.position = InputTracking.GetLocalPosition(XRNode.RightHand);
			resultPose.rotation = InputTracking.GetLocalRotation(XRNode.RightHand);
			return true;
		case TrackedPose.RemotePose:
			resultPose.position = InputTracking.GetLocalPosition(XRNode.RightHand);
			resultPose.rotation = InputTracking.GetLocalRotation(XRNode.RightHand);
			return true;
		case TrackedPose.ColorCamera:
			if (!TryGetTangoPose(CoordinateFrame.CameraColor, out resultPose))
			{
				resultPose.position = InputTracking.GetLocalPosition(XRNode.CenterEye);
				resultPose.rotation = InputTracking.GetLocalRotation(XRNode.CenterEye);
			}
			return true;
		case TrackedPose.DepthCamera:
			return TryGetTangoPose(CoordinateFrame.CameraDepth, out resultPose);
		case TrackedPose.FisheyeCamera:
			return TryGetTangoPose(CoordinateFrame.CameraFisheye, out resultPose);
		case TrackedPose.Device:
			return TryGetTangoPose(CoordinateFrame.Device, out resultPose);
		default:
			resultPose = Pose.identity;
			return false;
		}
	}

	private void CacheLocalPosition()
	{
		m_OriginPose.position = base.transform.localPosition;
		m_OriginPose.rotation = base.transform.localRotation;
	}

	private void ResetToCachedLocalPosition()
	{
		SetLocalTransform(m_OriginPose.position, m_OriginPose.rotation);
	}

	protected virtual void Awake()
	{
		CacheLocalPosition();
		if (HasStereoCamera())
		{
			XRDevice.DisableAutoXRCameraTracking(GetComponent<Camera>(), disabled: true);
		}
	}

	protected virtual void OnDestroy()
	{
		if (!HasStereoCamera())
		{
		}
	}

	protected virtual void OnEnable()
	{
		Application.onBeforeRender += OnBeforeRender;
	}

	protected virtual void OnDisable()
	{
		ResetToCachedLocalPosition();
		Application.onBeforeRender -= OnBeforeRender;
	}

	protected virtual void FixedUpdate()
	{
		if (m_UpdateType == UpdateType.Update || m_UpdateType == UpdateType.UpdateAndBeforeRender)
		{
			PerformUpdate();
		}
	}

	protected virtual void Update()
	{
		if (m_UpdateType == UpdateType.Update || m_UpdateType == UpdateType.UpdateAndBeforeRender)
		{
			PerformUpdate();
		}
	}

	protected virtual void OnBeforeRender()
	{
		if (m_UpdateType == UpdateType.BeforeRender || m_UpdateType == UpdateType.UpdateAndBeforeRender)
		{
			PerformUpdate();
		}
	}

	protected virtual void SetLocalTransform(Vector3 newPosition, Quaternion newRotation)
	{
		if (m_TrackingType == TrackingType.RotationAndPosition || m_TrackingType == TrackingType.RotationOnly)
		{
			base.transform.localRotation = newRotation;
		}
		if (m_TrackingType == TrackingType.RotationAndPosition || m_TrackingType == TrackingType.PositionOnly)
		{
			base.transform.localPosition = newPosition;
		}
	}

	protected Pose TransformPoseByOriginIfNeeded(Pose pose)
	{
		if (m_UseRelativeTransform)
		{
			return pose.GetTransformedBy(m_OriginPose);
		}
		return pose;
	}

	private bool HasStereoCamera()
	{
		Camera component = GetComponent<Camera>();
		return component != null && component.stereoEnabled;
	}

	protected virtual void PerformUpdate()
	{
		if (base.enabled)
		{
			Pose resultPose = default(Pose);
			if (GetDataFromSource(m_Device, m_PoseSource, out resultPose))
			{
				Pose pose = TransformPoseByOriginIfNeeded(resultPose);
				SetLocalTransform(pose.position, pose.rotation);
			}
		}
	}
}
