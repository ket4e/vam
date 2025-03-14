using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public class BrowserCursor
{
	public class CursorInfo
	{
		public int atlasOffset;

		public Vector2 hotspot;
	}

	private static Dictionary<BrowserNative.CursorType, CursorInfo> mapping = new Dictionary<BrowserNative.CursorType, CursorInfo>();

	private static bool loaded = false;

	private static int size;

	private static Texture2D allCursors;

	private bool _hasMouse;

	protected Texture2D normalTexture;

	protected Texture2D customTexture;

	public virtual Texture2D Texture { get; protected set; }

	public virtual Vector2 Hotspot { get; protected set; }

	public bool HasMouse
	{
		get
		{
			return _hasMouse;
		}
		set
		{
			_hasMouse = value;
			this.cursorChange();
		}
	}

	public event Action cursorChange = delegate
	{
	};

	public BrowserCursor()
	{
		Load();
		normalTexture = CreateTexture(size, size);
		SetActiveCursor(BrowserNative.CursorType.Pointer);
	}

	private static void Load()
	{
		if (!loaded)
		{
			allCursors = Resources.Load<Texture2D>("Browser/Cursors");
			if (!allCursors)
			{
				throw new Exception("Failed to find browser allCursors");
			}
			size = allCursors.height;
			TextAsset textAsset = Resources.Load<TextAsset>("Browser/Cursors");
			string[] array = textAsset.text.Split('\n');
			foreach (string text in array)
			{
				string[] array2 = text.Split(',');
				BrowserNative.CursorType key = (BrowserNative.CursorType)Enum.Parse(typeof(BrowserNative.CursorType), array2[0]);
				CursorInfo cursorInfo = new CursorInfo();
				cursorInfo.atlasOffset = int.Parse(array2[1]);
				cursorInfo.hotspot = new Vector2(int.Parse(array2[2]), int.Parse(array2[3]));
				CursorInfo value = cursorInfo;
				mapping[key] = value;
			}
			loaded = true;
		}
	}

	private Texture2D CreateTexture(int w, int h)
	{
		return new Texture2D(w, h, TextureFormat.ARGB32, mipmap: false);
	}

	public virtual void SetActiveCursor(BrowserNative.CursorType type)
	{
		switch (type)
		{
		case BrowserNative.CursorType.Custom:
			throw new ArgumentException("Use SetCustomCursor to set custom cursors.", "type");
		case BrowserNative.CursorType.None:
			Texture = null;
			this.cursorChange();
			return;
		}
		CursorInfo cursorInfo = mapping[type];
		Color[] pixels = allCursors.GetPixels(cursorInfo.atlasOffset * size, 0, size, size);
		Hotspot = cursorInfo.hotspot;
		normalTexture.SetPixels(pixels);
		normalTexture.Apply(updateMipmaps: true);
		Texture = normalTexture;
		this.cursorChange();
	}

	public virtual void SetCustomCursor(Texture2D cursor, Vector2 hotspot)
	{
		Color32[] pixels = cursor.GetPixels32();
		bool flag = false;
		for (int i = 0; i < pixels.Length; i++)
		{
			if (pixels[i].a != 0)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			SetActiveCursor(BrowserNative.CursorType.None);
			return;
		}
		if (!customTexture || customTexture.width != cursor.width || customTexture.height != cursor.height)
		{
			UnityEngine.Object.Destroy(customTexture);
			customTexture = CreateTexture(cursor.width, cursor.height);
		}
		customTexture.SetPixels32(pixels);
		customTexture.Apply(updateMipmaps: true);
		Hotspot = hotspot;
		Texture = customTexture;
		this.cursorChange();
	}
}
