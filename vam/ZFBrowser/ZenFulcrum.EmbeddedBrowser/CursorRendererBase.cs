using System.Collections;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

[RequireComponent(typeof(PointerUIBase))]
public abstract class CursorRendererBase : MonoBehaviour
{
	protected BrowserCursor cursor;

	public virtual void OnEnable()
	{
		StartCoroutine(Setup());
	}

	private IEnumerator Setup()
	{
		if (cursor == null)
		{
			yield return null;
			cursor = GetComponent<Browser>().UIHandler.BrowserCursor;
			cursor.cursorChange += CursorChange;
		}
	}

	protected abstract void CursorChange();
}
