using UnityEngine;

namespace Leap.Unity;

public class LeapTestProvider : LeapProvider
{
	public Frame frame;

	[Header("Runtime Basis Transforms")]
	[Tooltip("At runtime, if this Transform is non-null, the LeapTestProvider will create a test-pose left hand at this transform's position and rotation.Setting this binding to null at runtime will cause the hand to disappear from Frame data, as if it stopped tracking.")]
	public Transform leftHandBasis;

	private Hand _leftHand;

	private Hand _cachedLeftHand;

	[Tooltip("At runtime, if this Transform is non-null, the LeapTestProvider will create a test-pose right hand at this transform's position and rotation.Setting this binding to null at runtime will cause the hand to disappear from Frame data, as if it stopped tracking.")]
	public Transform rightHandBasis;

	private Hand _rightHand;

	private Hand _cachedRightHand;

	public override Frame CurrentFrame => frame;

	public override Frame CurrentFixedFrame => frame;

	private void Awake()
	{
		_cachedLeftHand = TestHandFactory.MakeTestHand(isLeft: true, 0, 0, TestHandFactory.UnitType.UnityUnits);
		_cachedRightHand = TestHandFactory.MakeTestHand(isLeft: false, 0, 0, TestHandFactory.UnitType.UnityUnits);
	}

	private void Update()
	{
		if (_leftHand == null && leftHandBasis != null)
		{
			_leftHand = _cachedLeftHand;
			frame.Hands.Add(_leftHand);
		}
		if (_leftHand != null && leftHandBasis == null)
		{
			frame.Hands.Remove(_leftHand);
			_leftHand = null;
		}
		if (_leftHand != null)
		{
			_leftHand.SetTransform(leftHandBasis.position, leftHandBasis.rotation);
		}
		if (_rightHand == null && rightHandBasis != null)
		{
			_rightHand = _cachedRightHand;
			frame.Hands.Add(_rightHand);
		}
		if (_rightHand != null && rightHandBasis == null)
		{
			frame.Hands.Remove(_rightHand);
			_rightHand = null;
		}
		if (_rightHand != null)
		{
			_rightHand.SetTransform(rightHandBasis.position, rightHandBasis.rotation);
		}
		DispatchUpdateFrameEvent(frame);
	}

	private void FixedUpdate()
	{
		DispatchFixedFrameEvent(frame);
	}
}
