using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using ZenFulcrum.EmbeddedBrowser.VR;

namespace ZenFulcrum.EmbeddedBrowser;

public class VRBrowserHand : MonoBehaviour
{
	[Tooltip("Which hand we should look to track.")]
	public XRNode hand = XRNode.LeftHand;

	[Tooltip("Optional visualization of this hand. It should be a child of the VRHand object and will be set active when the controller is tracking.")]
	public GameObject visualization;

	[Tooltip("How much we must slide a finger/joystick before we start scrolling.")]
	public float scrollThreshold = 0.1f;

	[Tooltip("How fast the page moves as we move our finger across the touchpad.\r\nSet to a negative number to enable that infernal \"natural scrolling\" that's been making so many trackpads unusable lately.")]
	public float trackpadScrollSpeed = 0.05f;

	[Tooltip("How fast the page moves as we scroll with a joystick.")]
	public float joystickScrollSpeed = 75f;

	private Vector2 lastTouchPoint;

	private bool touchIsScrolling;

	private XRNodeState nodeState;

	private VRInput input;

	private int lastFrame;

	private List<XRNodeState> states = new List<XRNodeState>();

	private bool hasTouchpad;

	public bool Tracked { get; private set; }

	public MouseButton DepressedButtons { get; private set; }

	public Vector2 ScrollDelta { get; private set; }

	public void OnEnable()
	{
		VRInput.Init();
		input = VRInput.Impl;
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(UpdatePreCull));
		if ((bool)visualization)
		{
			visualization.SetActive(value: false);
		}
	}

	public void OnDisable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(UpdatePreCull));
	}

	public virtual void Update()
	{
		if (Time.frameCount >= 5)
		{
			ReadInput();
		}
	}

	protected virtual void ReadInput()
	{
		DepressedButtons = (MouseButton)0;
		ScrollDelta = Vector2.zero;
		if (nodeState.tracked)
		{
			float axis = input.GetAxis(nodeState, InputAxis.LeftClick);
			if (axis > 0.9f)
			{
				DepressedButtons |= MouseButton.Left;
			}
			float axis2 = input.GetAxis(nodeState, InputAxis.MiddleClick);
			if (axis2 > 0.5f)
			{
				DepressedButtons |= MouseButton.Middle;
			}
			float axis3 = input.GetAxis(nodeState, InputAxis.RightClick);
			if (axis3 > 0.5f)
			{
				DepressedButtons |= MouseButton.Right;
			}
			JoyPadType joypadTypes = input.GetJoypadTypes(nodeState);
			if ((joypadTypes & JoyPadType.Joystick) != 0)
			{
				ReadJoystick();
			}
			if ((joypadTypes & JoyPadType.TouchPad) != 0)
			{
				ReadTouchpad();
			}
		}
	}

	protected virtual void ReadTouchpad()
	{
		Vector2 vector = new Vector2(input.GetAxis(nodeState, InputAxis.TouchPadX), input.GetAxis(nodeState, InputAxis.TouchPadY));
		if (input.GetAxis(nodeState, InputAxis.TouchPadTouch) > 0.5f)
		{
			Vector2 vector2 = vector - lastTouchPoint;
			if (!touchIsScrolling)
			{
				if (vector2.magnitude * trackpadScrollSpeed > scrollThreshold)
				{
					touchIsScrolling = true;
					lastTouchPoint = vector;
				}
			}
			else
			{
				ScrollDelta += new Vector2(0f - vector2.x, vector2.y) * trackpadScrollSpeed;
				lastTouchPoint = vector;
			}
		}
		else
		{
			lastTouchPoint = vector;
			touchIsScrolling = false;
		}
	}

	protected virtual void ReadJoystick()
	{
		Vector2 vector = new Vector2(0f - input.GetAxis(nodeState, InputAxis.JoyStickX), input.GetAxis(nodeState, InputAxis.JoyStickY));
		vector.x = ((!(Mathf.Abs(vector.x) > scrollThreshold)) ? 0f : (vector.x - Mathf.Sign(vector.x) * scrollThreshold));
		vector.y = ((!(Mathf.Abs(vector.y) > scrollThreshold)) ? 0f : (vector.y - Mathf.Sign(vector.y) * scrollThreshold));
		vector = vector * vector.magnitude * joystickScrollSpeed * Time.deltaTime;
		ScrollDelta += vector;
	}

	private void UpdatePreCull(Camera cam)
	{
		if (lastFrame == Time.frameCount)
		{
			return;
		}
		lastFrame = Time.frameCount;
		InputTracking.GetNodeStates(states);
		for (int i = 0; i < states.Count; i++)
		{
			if (states[i].nodeType == hand)
			{
				nodeState = states[i];
				VRInput.Pose pose = input.GetPose(nodeState);
				base.transform.localPosition = pose.pos;
				base.transform.localRotation = pose.rot;
				if ((bool)visualization)
				{
					GameObject obj = visualization;
					bool active = (Tracked = nodeState.tracked);
					obj.SetActive(active);
				}
			}
		}
	}
}
