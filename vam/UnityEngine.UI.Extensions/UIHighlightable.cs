using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/UI Highlightable Extension")]
[RequireComponent(typeof(RectTransform), typeof(Graphic))]
public class UIHighlightable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	[Serializable]
	public class InteractableChangedEvent : UnityEvent<bool>
	{
	}

	private Graphic m_Graphic;

	private bool m_Highlighted;

	private bool m_Pressed;

	[SerializeField]
	[Tooltip("Can this panel be interacted with or is it disabled? (does not affect child components)")]
	private bool m_Interactable = true;

	[SerializeField]
	[Tooltip("Does the panel remain in the pressed state when clicked? (default false)")]
	private bool m_ClickToHold;

	[Tooltip("The default color for the panel")]
	public Color NormalColor = Color.grey;

	[Tooltip("The color for the panel when a mouse is over it or it is in focus")]
	public Color HighlightedColor = Color.yellow;

	[Tooltip("The color for the panel when it is clicked/held")]
	public Color PressedColor = Color.green;

	[Tooltip("The color for the panel when it is not interactable (see Interactable)")]
	public Color DisabledColor = Color.gray;

	[Tooltip("Event for when the panel is enabled / disabled, to enable disabling / enabling of child or other gameobjects")]
	public InteractableChangedEvent OnInteractableChanged;

	public bool Interactable
	{
		get
		{
			return m_Interactable;
		}
		set
		{
			m_Interactable = value;
			HighlightInteractable(m_Graphic);
			OnInteractableChanged.Invoke(m_Interactable);
		}
	}

	public bool ClickToHold
	{
		get
		{
			return m_ClickToHold;
		}
		set
		{
			m_ClickToHold = value;
		}
	}

	private void Awake()
	{
		m_Graphic = GetComponent<Graphic>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (Interactable && !m_Pressed)
		{
			m_Highlighted = true;
			m_Graphic.color = HighlightedColor;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (Interactable && !m_Pressed)
		{
			m_Highlighted = false;
			m_Graphic.color = NormalColor;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (Interactable)
		{
			m_Graphic.color = PressedColor;
			if (ClickToHold)
			{
				m_Pressed = !m_Pressed;
			}
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!m_Pressed)
		{
			HighlightInteractable(m_Graphic);
		}
	}

	private void HighlightInteractable(Graphic graphic)
	{
		if (m_Interactable)
		{
			if (m_Highlighted)
			{
				graphic.color = HighlightedColor;
			}
			else
			{
				graphic.color = NormalColor;
			}
		}
		else
		{
			graphic.color = DisabledColor;
		}
	}
}
