using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public class CursorRendererWorldSpace : CursorRendererBase
{
	[Tooltip("How far to keep the cursor from the surface of the browser. Set it as low as you can without causing z-fighting. (Note: The default cursor material will always render on top of everything, this is more useful if you use a different material.")]
	public float zOffset = 0.005f;

	[Tooltip("How large should the cursor be when rendered? (meters)")]
	public float size = 0.1f;

	private GameObject cursorHolder;

	private GameObject cursorImage;

	private PointerUIBase pointerUI;

	private bool cursorVisible;

	public void Awake()
	{
		pointerUI = GetComponent<PointerUIBase>();
		cursorHolder = new GameObject("Cursor for " + base.name);
		cursorHolder.transform.localScale = new Vector3(size, size, size);
		cursorImage = GameObject.CreatePrimitive(PrimitiveType.Quad);
		cursorImage.name = "Cursor Image";
		cursorImage.transform.parent = cursorHolder.transform;
		MeshRenderer component = cursorImage.GetComponent<MeshRenderer>();
		component.sharedMaterial = Resources.Load<Material>("Browser/CursorDecal");
		Object.Destroy(cursorImage.GetComponent<Collider>());
		cursorImage.transform.SetParent(cursorHolder.transform, worldPositionStays: false);
		cursorImage.transform.localScale = new Vector3(1f, 1f, 1f);
		cursorHolder.SetActive(value: false);
	}

	protected override void CursorChange()
	{
		if (cursor.HasMouse && (bool)cursor.Texture)
		{
			cursorVisible = true;
			Renderer component = cursorImage.GetComponent<Renderer>();
			component.material.mainTexture = cursor.Texture;
			Vector2 hotspot = cursor.Hotspot;
			component.transform.localPosition = new Vector3(0.5f - hotspot.x / (float)cursor.Texture.width, -0.5f + hotspot.y / (float)cursor.Texture.height, 0f);
		}
		else
		{
			cursorVisible = false;
		}
	}

	public void LateUpdate()
	{
		pointerUI.GetCurrentHitLocation(out var pos, out var rot);
		if (float.IsNaN(pos.x))
		{
			cursorHolder.SetActive(value: false);
			return;
		}
		cursorHolder.SetActive(cursorVisible);
		cursorHolder.transform.position = pos + rot * new Vector3(0f, 0f, 0f - zOffset);
		cursorHolder.transform.rotation = rot;
	}

	public void OnDestroy()
	{
		Object.Destroy(cursorHolder.gameObject);
	}
}
