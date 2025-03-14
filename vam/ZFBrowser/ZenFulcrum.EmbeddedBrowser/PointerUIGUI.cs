using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZenFulcrum.EmbeddedBrowser;

[RequireComponent(typeof(RawImage))]
public class PointerUIGUI : PointerUIBase, IBrowserUI, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IEventSystemHandler
{
	protected RawImage myImage;

	public bool enableInput = true;

	public bool automaticResize = true;

	protected BaseRaycaster raycaster;

	protected RectTransform rTransform;

	protected bool _mouseHasFocus;

	protected bool _keyboardHasFocus;

	public override bool MouseHasFocus
	{
		get
		{
			return _mouseHasFocus && enableInput;
		}
		protected set
		{
			_mouseHasFocus = value;
		}
	}

	public override bool KeyboardHasFocus
	{
		get
		{
			if (!enableInput)
			{
				return false;
			}
			return _keyboardHasFocus || focusForceCount > 0;
		}
	}

	public override void Awake()
	{
		base.Awake();
		myImage = GetComponent<RawImage>();
		browser.afterResize += UpdateTexture;
		rTransform = GetComponent<RectTransform>();
	}

	protected void OnEnable()
	{
		if (automaticResize)
		{
			StartCoroutine(WatchResize());
		}
	}

	private IEnumerator WatchResize()
	{
		Rect currentSize = default(Rect);
		while (base.enabled)
		{
			Rect rect = rTransform.rect;
			if (rect.size.x <= 0f || rect.size.y <= 0f)
			{
				rect.size = new Vector2(512f, 512f);
			}
			if (rect.size != currentSize.size)
			{
				browser.Resize((int)rect.size.x, (int)rect.size.y);
				currentSize = rect;
			}
			yield return null;
		}
	}

	protected void UpdateTexture(Texture2D texture)
	{
		myImage.texture = texture;
		myImage.uvRect = new Rect(0f, 0f, 1f, 1f);
	}

	protected override Vector2 MapPointerToBrowser(Vector2 screenPosition, int pointerId)
	{
		if (!raycaster)
		{
			raycaster = GetComponentInParent<BaseRaycaster>();
		}
		RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)base.transform, screenPosition, raycaster.eventCamera, out var localPoint);
		localPoint.x = localPoint.x / rTransform.rect.width + rTransform.pivot.x;
		localPoint.y = localPoint.y / rTransform.rect.height + rTransform.pivot.y;
		if (localPoint.x < 0f || localPoint.x > 1f)
		{
			localPoint.x = float.NaN;
		}
		if (localPoint.y < 0f || localPoint.y > 1f)
		{
			localPoint.x = float.NaN;
		}
		return localPoint;
	}

	protected override Vector2 MapRayToBrowser(Ray worldRay, int pointerId)
	{
		EventSystem current = EventSystem.current;
		if (!current)
		{
			return new Vector2(float.NaN, float.NaN);
		}
		return new Vector2(float.NaN, float.NaN);
	}

	public override void GetCurrentHitLocation(out Vector3 pos, out Quaternion rot)
	{
		pos = new Vector3(float.NaN, float.NaN, float.NaN);
		rot = Quaternion.identity;
	}

	public void OnSelect(BaseEventData eventData)
	{
		_keyboardHasFocus = true;
		Input.imeCompositionMode = IMECompositionMode.Off;
	}

	public void OnDeselect(BaseEventData eventData)
	{
		_keyboardHasFocus = false;
		Input.imeCompositionMode = IMECompositionMode.Auto;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_mouseHasFocus = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_mouseHasFocus = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		EventSystem.current.SetSelectedGameObject(base.gameObject);
	}
}
