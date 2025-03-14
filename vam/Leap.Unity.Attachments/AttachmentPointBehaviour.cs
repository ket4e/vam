using Leap.Unity.Attributes;
using UnityEngine;

namespace Leap.Unity.Attachments;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class AttachmentPointBehaviour : MonoBehaviour
{
	[Tooltip("The AttachmentHand associated with this AttachmentPointBehaviour. AttachmentPointBehaviours should be beneath their AttachmentHand object in the hierarchy.")]
	[Disable]
	public AttachmentHand attachmentHand;

	[Tooltip("To change which attachment points are available on an AttachmentHand, refer to the inspector for the parent AttachmentHands object.")]
	[Disable]
	public AttachmentPointFlags attachmentPoint;

	private void OnValidate()
	{
		if (!attachmentPoint.IsSinglePoint() && attachmentPoint != 0)
		{
			Debug.LogError("AttachmentPointBehaviours should refer to a single attachmentPoint flag.", base.gameObject);
			attachmentPoint = AttachmentPointFlags.None;
		}
	}

	private void OnDestroy()
	{
		if (attachmentHand != null)
		{
			attachmentHand.notifyPointBehaviourDeleted(this);
		}
	}

	public static implicit operator AttachmentPointFlags(AttachmentPointBehaviour p)
	{
		if (p == null)
		{
			return AttachmentPointFlags.None;
		}
		return p.attachmentPoint;
	}

	public void SetTransformUsingHand(Hand hand)
	{
		if (hand != null)
		{
			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			GetLeapHandPointData(hand, attachmentPoint, out position, out rotation);
			base.transform.position = position;
			base.transform.rotation = rotation;
		}
	}

	public static void GetLeapHandPointData(Hand hand, AttachmentPointFlags singlePoint, out Vector3 position, out Quaternion rotation)
	{
		position = Vector3.zero;
		rotation = Quaternion.identity;
		if (singlePoint != 0 && !singlePoint.IsSinglePoint())
		{
			Debug.LogError("Cannot get attachment point data for an AttachmentPointFlags argument consisting of more than one set flag.");
			return;
		}
		switch (singlePoint)
		{
		case AttachmentPointFlags.Wrist:
			position = hand.WristPosition.ToVector3();
			rotation = hand.Arm.Basis.rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.Palm:
			position = hand.PalmPosition.ToVector3();
			rotation = hand.Basis.rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.ThumbProximalJoint:
			position = hand.Fingers[0].bones[1].NextJoint.ToVector3();
			rotation = hand.Fingers[0].bones[2].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.ThumbDistalJoint:
			position = hand.Fingers[0].bones[2].NextJoint.ToVector3();
			rotation = hand.Fingers[0].bones[3].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.ThumbTip:
			position = hand.Fingers[0].bones[3].NextJoint.ToVector3();
			rotation = hand.Fingers[0].bones[3].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.IndexKnuckle:
			position = hand.Fingers[1].bones[0].NextJoint.ToVector3();
			rotation = hand.Fingers[1].bones[1].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.IndexMiddleJoint:
			position = hand.Fingers[1].bones[1].NextJoint.ToVector3();
			rotation = hand.Fingers[1].bones[2].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.IndexDistalJoint:
			position = hand.Fingers[1].bones[2].NextJoint.ToVector3();
			rotation = hand.Fingers[1].bones[3].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.IndexTip:
			position = hand.Fingers[1].bones[3].NextJoint.ToVector3();
			rotation = hand.Fingers[1].bones[3].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.MiddleKnuckle:
			position = hand.Fingers[2].bones[0].NextJoint.ToVector3();
			rotation = hand.Fingers[2].bones[1].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.MiddleMiddleJoint:
			position = hand.Fingers[2].bones[1].NextJoint.ToVector3();
			rotation = hand.Fingers[2].bones[2].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.MiddleDistalJoint:
			position = hand.Fingers[2].bones[2].NextJoint.ToVector3();
			rotation = hand.Fingers[2].bones[3].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.MiddleTip:
			position = hand.Fingers[2].bones[3].NextJoint.ToVector3();
			rotation = hand.Fingers[2].bones[3].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.RingKnuckle:
			position = hand.Fingers[3].bones[0].NextJoint.ToVector3();
			rotation = hand.Fingers[3].bones[1].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.RingMiddleJoint:
			position = hand.Fingers[3].bones[1].NextJoint.ToVector3();
			rotation = hand.Fingers[3].bones[2].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.RingDistalJoint:
			position = hand.Fingers[3].bones[2].NextJoint.ToVector3();
			rotation = hand.Fingers[3].bones[3].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.RingTip:
			position = hand.Fingers[3].bones[3].NextJoint.ToVector3();
			rotation = hand.Fingers[3].bones[3].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.PinkyKnuckle:
			position = hand.Fingers[4].bones[0].NextJoint.ToVector3();
			rotation = hand.Fingers[4].bones[1].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.PinkyMiddleJoint:
			position = hand.Fingers[4].bones[1].NextJoint.ToVector3();
			rotation = hand.Fingers[4].bones[2].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.PinkyDistalJoint:
			position = hand.Fingers[4].bones[2].NextJoint.ToVector3();
			rotation = hand.Fingers[4].bones[3].Rotation.ToQuaternion();
			break;
		case AttachmentPointFlags.PinkyTip:
			position = hand.Fingers[4].bones[3].NextJoint.ToVector3();
			rotation = hand.Fingers[4].bones[3].Rotation.ToQuaternion();
			break;
		}
	}
}
