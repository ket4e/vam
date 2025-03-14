using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class InteractableHoverEvents : MonoBehaviour
{
	public UnityEvent onHandHoverBegin;

	public UnityEvent onHandHoverEnd;

	public UnityEvent onAttachedToHand;

	public UnityEvent onDetachedFromHand;

	private void OnHandHoverBegin()
	{
		onHandHoverBegin.Invoke();
	}

	private void OnHandHoverEnd()
	{
		onHandHoverEnd.Invoke();
	}

	private void OnAttachedToHand(Hand hand)
	{
		onAttachedToHand.Invoke();
	}

	private void OnDetachedFromHand(Hand hand)
	{
		onDetachedFromHand.Invoke();
	}
}
