#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.UIElements;

internal class Panel : BaseVisualElementPanel
{
	private StyleContext m_StyleContext;

	private VisualElement m_RootContainer;

	private IDataWatchService m_DataWatch;

	private TimerEventScheduler m_Scheduler;

	internal static LoadResourceFunction loadResourceFunc = null;

	private static TimeMsFunction s_TimeSinceStartup;

	private bool m_KeepPixelCacheOnWorldBoundChange;

	private const int kMaxValidatePersistentDataCount = 5;

	private const int kMaxValidateLayoutCount = 5;

	public override VisualElement visualTree => m_RootContainer;

	public override IEventDispatcher dispatcher { get; protected set; }

	internal override IDataWatchService dataWatch => m_DataWatch;

	public TimerEventScheduler timerEventScheduler => m_Scheduler ?? (m_Scheduler = new TimerEventScheduler());

	internal override IScheduler scheduler => timerEventScheduler;

	internal StyleContext styleContext => m_StyleContext;

	public override ScriptableObject ownerObject { get; protected set; }

	public bool allowPixelCaching { get; set; }

	public override ContextType contextType { get; protected set; }

	public override SavePersistentViewData savePersistentViewData { get; set; }

	public override GetViewDataDictionary getViewDataDictionary { get; set; }

	public override FocusController focusController { get; set; }

	public override EventInterests IMGUIEventInterests { get; set; }

	internal static TimeMsFunction TimeSinceStartup
	{
		get
		{
			return s_TimeSinceStartup;
		}
		set
		{
			if (value == null)
			{
				value = DefaultTimeSinceStartupMs;
			}
			s_TimeSinceStartup = value;
		}
	}

	public override bool keepPixelCacheOnWorldBoundChange
	{
		get
		{
			return m_KeepPixelCacheOnWorldBoundChange;
		}
		set
		{
			if (m_KeepPixelCacheOnWorldBoundChange != value)
			{
				m_KeepPixelCacheOnWorldBoundChange = value;
				if (!value)
				{
					m_RootContainer.Dirty(ChangeType.Transform | ChangeType.Repaint);
				}
			}
		}
	}

	public override int IMGUIContainersCount { get; set; }

	public Panel(ScriptableObject ownerObject, ContextType contextType, IDataWatchService dataWatch = null, IEventDispatcher dispatcher = null)
	{
		this.ownerObject = ownerObject;
		this.contextType = contextType;
		m_DataWatch = dataWatch;
		this.dispatcher = dispatcher;
		stylePainter = new StylePainter();
		cursorManager = new CursorManager();
		contextualMenuManager = null;
		m_RootContainer = new VisualElement();
		m_RootContainer.name = VisualElementUtils.GetUniqueName("PanelContainer");
		m_RootContainer.persistenceKey = "PanelContainer";
		visualTree.ChangePanel(this);
		focusController = new FocusController(new VisualElementFocusRing(visualTree));
		m_StyleContext = new StyleContext(m_RootContainer);
		allowPixelCaching = true;
	}

	public static long TimeSinceStartupMs()
	{
		return (s_TimeSinceStartup != null) ? s_TimeSinceStartup() : DefaultTimeSinceStartupMs();
	}

	internal static long DefaultTimeSinceStartupMs()
	{
		return (long)(Time.realtimeSinceStartup * 1000f);
	}

	private VisualElement PickAll(VisualElement root, Vector2 point, List<VisualElement> picked = null)
	{
		if ((root.pseudoStates & PseudoStates.Invisible) == PseudoStates.Invisible)
		{
			return null;
		}
		Vector3 vector = root.WorldToLocal(point);
		bool flag = root.ContainsPoint(vector);
		if (!flag && root.clippingOptions != VisualElement.ClippingOptions.NoClipping)
		{
			return null;
		}
		if (picked != null && root.enabledInHierarchy && root.pickingMode == PickingMode.Position)
		{
			picked.Add(root);
		}
		VisualElement visualElement = null;
		for (int num = root.shadow.childCount - 1; num >= 0; num--)
		{
			VisualElement root2 = root.shadow[num];
			VisualElement visualElement2 = PickAll(root2, point, picked);
			if (visualElement == null && visualElement2 != null)
			{
				visualElement = visualElement2;
			}
		}
		if (visualElement != null)
		{
			return visualElement;
		}
		switch (root.pickingMode)
		{
		case PickingMode.Position:
			if (flag && root.enabledInHierarchy)
			{
				return root;
			}
			break;
		}
		return null;
	}

	public override VisualElement LoadTemplate(string path, Dictionary<string, VisualElement> slots = null)
	{
		VisualTreeAsset visualTreeAsset = loadResourceFunc(path, typeof(VisualTreeAsset)) as VisualTreeAsset;
		if (visualTreeAsset == null)
		{
			return null;
		}
		return visualTreeAsset.CloneTree(slots);
	}

	public override VisualElement PickAll(Vector2 point, List<VisualElement> picked)
	{
		ValidateLayout();
		picked?.Clear();
		return PickAll(visualTree, point, picked);
	}

	public override VisualElement Pick(Vector2 point)
	{
		ValidateLayout();
		return PickAll(visualTree, point);
	}

	private void ValidatePersistentData()
	{
		int num = 0;
		while (visualTree.AnyDirty(ChangeType.PersistentData | ChangeType.PersistentDataPath))
		{
			ValidatePersistentDataOnSubTree(visualTree, enablePersistence: true);
			num++;
			if (num > 5)
			{
				Debug.LogError("UIElements: Too many children recursively added that rely on persistent data: " + visualTree);
				break;
			}
		}
	}

	private void ValidatePersistentDataOnSubTree(VisualElement root, bool enablePersistence)
	{
		if (!root.IsPersitenceSupportedOnChildren())
		{
			enablePersistence = false;
		}
		if (root.IsDirty(ChangeType.PersistentData))
		{
			root.OnPersistentDataReady(enablePersistence);
			root.ClearDirty(ChangeType.PersistentData);
		}
		if (root.IsDirty(ChangeType.PersistentDataPath))
		{
			for (int i = 0; i < root.shadow.childCount; i++)
			{
				ValidatePersistentDataOnSubTree(root.shadow[i], enablePersistence);
			}
			root.ClearDirty(ChangeType.PersistentDataPath);
		}
	}

	private void ValidateStyling()
	{
		if (m_RootContainer.AnyDirty(ChangeType.Styles | ChangeType.StylesPath))
		{
			m_StyleContext.ApplyStyles();
		}
	}

	public override void ValidateLayout()
	{
		ValidateStyling();
		int num = 0;
		while (visualTree.cssNode.IsDirty)
		{
			visualTree.cssNode.CalculateLayout();
			ValidateSubTree(visualTree);
			if (num++ >= 5)
			{
				Debug.LogError("ValidateLayout is struggling to process current layout (consider simplifying to avoid recursive layout): " + visualTree);
				break;
			}
		}
	}

	private void ValidateSubTree(VisualElement root)
	{
		Rect rect = new Rect(root.cssNode.LayoutX, root.cssNode.LayoutY, root.cssNode.LayoutWidth, root.cssNode.LayoutHeight);
		Rect lastLayout = root.renderData.lastLayout;
		bool flag = lastLayout != rect;
		if (flag)
		{
			if (lastLayout.position != rect.position)
			{
				root.Dirty(ChangeType.Transform);
			}
			root.renderData.lastLayout = rect;
		}
		bool hasNewLayout = root.cssNode.HasNewLayout;
		if (hasNewLayout)
		{
			for (int i = 0; i < root.shadow.childCount; i++)
			{
				ValidateSubTree(root.shadow[i]);
			}
		}
		if (flag)
		{
			using PostLayoutEvent postLayoutEvent = PostLayoutEvent.GetPooled(hasNewLayout, lastLayout, rect);
			postLayoutEvent.target = root;
			UIElementsUtility.eventDispatcher.DispatchEvent(postLayoutEvent, this);
		}
		root.ClearDirty(ChangeType.Layout);
		if (hasNewLayout)
		{
			root.cssNode.MarkLayoutSeen();
		}
	}

	private Rect ComputeAAAlignedBound(Rect position, Matrix4x4 mat)
	{
		Rect rect = position;
		Vector3 vector = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y, 0f));
		Vector3 vector2 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y, 0f));
		Vector3 vector3 = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y + rect.height, 0f));
		Vector3 vector4 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y + rect.height, 0f));
		return Rect.MinMaxRect(Mathf.Min(vector.x, Mathf.Min(vector2.x, Mathf.Min(vector3.x, vector4.x))), Mathf.Min(vector.y, Mathf.Min(vector2.y, Mathf.Min(vector3.y, vector4.y))), Mathf.Max(vector.x, Mathf.Max(vector2.x, Mathf.Max(vector3.x, vector4.x))), Mathf.Max(vector.y, Mathf.Max(vector2.y, Mathf.Max(vector3.y, vector4.y))));
	}

	private bool ShouldUsePixelCache(VisualElement root)
	{
		return allowPixelCaching && root.clippingOptions == VisualElement.ClippingOptions.ClipAndCacheContents && root.worldBound.size.magnitude > Mathf.Epsilon;
	}

	private void PaintSubTree(Event e, VisualElement root, Matrix4x4 offset, Rect currentGlobalClip)
	{
		if (root == null || root.panel != this || (root.pseudoStates & PseudoStates.Invisible) == PseudoStates.Invisible || root.style.opacity.GetSpecifiedValueOrDefault(1f) < Mathf.Epsilon)
		{
			return;
		}
		if (root.clippingOptions != VisualElement.ClippingOptions.NoClipping)
		{
			Rect rect = ComputeAAAlignedBound(root.rect, offset * root.worldTransform);
			if (!rect.Overlaps(currentGlobalClip))
			{
				return;
			}
			float num = Mathf.Max(rect.x, currentGlobalClip.x);
			float num2 = Mathf.Min(rect.x + rect.width, currentGlobalClip.x + currentGlobalClip.width);
			float num3 = Mathf.Max(rect.y, currentGlobalClip.y);
			float num4 = Mathf.Min(rect.y + rect.height, currentGlobalClip.y + currentGlobalClip.height);
			currentGlobalClip = new Rect(num, num3, num2 - num, num4 - num3);
		}
		if (ShouldUsePixelCache(root))
		{
			IStylePainter stylePainter = this.stylePainter;
			Rect worldBound = root.worldBound;
			int num5 = (int)GUIUtility.Internal_Roundf(worldBound.xMax) - (int)GUIUtility.Internal_Roundf(worldBound.xMin);
			int num6 = (int)GUIUtility.Internal_Roundf(worldBound.yMax) - (int)GUIUtility.Internal_Roundf(worldBound.yMin);
			int val = (int)GUIUtility.Internal_Roundf((float)num5 * GUIUtility.pixelsPerPoint);
			int val2 = (int)GUIUtility.Internal_Roundf((float)num6 * GUIUtility.pixelsPerPoint);
			val = Math.Max(val, 1);
			val2 = Math.Max(val2, 1);
			RenderTexture renderTexture = root.renderData.pixelCache;
			if (renderTexture != null && (renderTexture.width != val || renderTexture.height != val2) && (!keepPixelCacheOnWorldBoundChange || root.IsDirty(ChangeType.Repaint)))
			{
				Object.DestroyImmediate(renderTexture);
				renderTexture = (root.renderData.pixelCache = null);
			}
			if (root.IsDirty(ChangeType.Repaint) || root.renderData.pixelCache == null || !root.renderData.pixelCache.IsCreated())
			{
				if (renderTexture == null)
				{
					renderTexture = (root.renderData.pixelCache = new RenderTexture(val, val2, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB));
				}
				bool flag = (float)root.style.borderTopLeftRadius > 0f || (float)root.style.borderTopRightRadius > 0f || (float)root.style.borderBottomLeftRadius > 0f || (float)root.style.borderBottomRightRadius > 0f;
				RenderTexture renderTexture2 = null;
				RenderTexture active = RenderTexture.active;
				try
				{
					if (flag)
					{
						renderTexture2 = (renderTexture = RenderTexture.GetTemporary(val, val2, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB));
					}
					RenderTexture.active = renderTexture;
					GL.Clear(clearDepth: true, clearColor: true, new Color(0f, 0f, 0f, 0f));
					Matrix4x4 matrix4x = Matrix4x4.Translate(new Vector3(0f - GUIUtility.Internal_Roundf(worldBound.x), 0f - GUIUtility.Internal_Roundf(worldBound.y), 0f));
					Matrix4x4 matrix4x2 = matrix4x * root.worldTransform;
					Rect rect2 = new Rect(0f, 0f, num5, num6);
					stylePainter.currentTransform = matrix4x2;
					bool enabled = SystemInfo.graphicsDeviceType != GraphicsDeviceType.Metal;
					using (new GUIUtility.ManualTex2SRGBScope(enabled))
					{
						using (new GUIClip.ParentClipScope(stylePainter.currentTransform, rect2))
						{
							stylePainter.currentWorldClip = rect2;
							root.DoRepaint(stylePainter);
							root.ClearDirty(ChangeType.Repaint);
							PaintSubTreeChildren(e, root, matrix4x, rect2);
						}
					}
					if (flag)
					{
						RenderTexture.active = root.renderData.pixelCache;
						stylePainter.currentTransform = Matrix4x4.identity;
						using (new GUIUtility.ManualTex2SRGBScope(enabled))
						{
							using (new GUIClip.ParentClipScope(stylePainter.currentTransform, rect2))
							{
								GL.Clear(clearDepth: true, clearColor: true, new Color(0f, 0f, 0f, 0f));
								TextureStylePainterParameters defaultTextureParameters = stylePainter.GetDefaultTextureParameters(root);
								defaultTextureParameters.texture = renderTexture;
								defaultTextureParameters.scaleMode = ScaleMode.StretchToFill;
								defaultTextureParameters.rect = rect2;
								defaultTextureParameters.border.SetWidth(0f);
								Vector4 vector = new Vector4(1f, 0f, 0f, 0f);
								float x = (matrix4x2 * vector).x;
								defaultTextureParameters.border.SetRadius(defaultTextureParameters.border.topLeftRadius * x, defaultTextureParameters.border.topRightRadius * x, defaultTextureParameters.border.bottomRightRadius * x, defaultTextureParameters.border.bottomLeftRadius * x);
								defaultTextureParameters.usePremultiplyAlpha = true;
								stylePainter.DrawTexture(defaultTextureParameters);
							}
						}
						stylePainter.currentTransform = matrix4x2;
						using (new GUIUtility.ManualTex2SRGBScope(enabled))
						{
							using (new GUIClip.ParentClipScope(stylePainter.currentTransform, rect2))
							{
								stylePainter.DrawBorder(root);
							}
						}
					}
				}
				finally
				{
					renderTexture = null;
					if (renderTexture2 != null)
					{
						RenderTexture.ReleaseTemporary(renderTexture2);
					}
					RenderTexture.active = active;
				}
			}
			stylePainter.currentWorldClip = currentGlobalClip;
			stylePainter.currentTransform = offset * root.worldTransform;
			TextureStylePainterParameters textureStylePainterParameters = default(TextureStylePainterParameters);
			textureStylePainterParameters.rect = root.alignedRect;
			textureStylePainterParameters.uv = new Rect(0f, 0f, 1f, 1f);
			textureStylePainterParameters.texture = root.renderData.pixelCache;
			textureStylePainterParameters.color = Color.white;
			textureStylePainterParameters.scaleMode = ScaleMode.StretchToFill;
			textureStylePainterParameters.usePremultiplyAlpha = true;
			TextureStylePainterParameters painterParams = textureStylePainterParameters;
			using (new GUIClip.ParentClipScope(stylePainter.currentTransform, currentGlobalClip))
			{
				stylePainter.DrawTexture(painterParams);
				return;
			}
		}
		this.stylePainter.currentTransform = offset * root.worldTransform;
		using (new GUIClip.ParentClipScope(this.stylePainter.currentTransform, currentGlobalClip))
		{
			this.stylePainter.currentWorldClip = currentGlobalClip;
			this.stylePainter.mousePosition = root.worldTransform.inverse.MultiplyPoint3x4(e.mousePosition);
			this.stylePainter.opacity = root.style.opacity.GetSpecifiedValueOrDefault(1f);
			root.DoRepaint(this.stylePainter);
			this.stylePainter.opacity = 1f;
			root.ClearDirty(ChangeType.Repaint);
			PaintSubTreeChildren(e, root, offset, currentGlobalClip);
		}
	}

	private void PaintSubTreeChildren(Event e, VisualElement root, Matrix4x4 offset, Rect textureClip)
	{
		int childCount = root.shadow.childCount;
		for (int i = 0; i < childCount; i++)
		{
			VisualElement root2 = root.shadow[i];
			PaintSubTree(e, root2, offset, textureClip);
			if (childCount != root.shadow.childCount)
			{
				throw new NotImplementedException("Visual tree is read-only during repaint");
			}
		}
	}

	public override void Repaint(Event e)
	{
		Debug.Assert(GUIClip.Internal_GetCount() == 0, "UIElement is not compatible with IMGUI GUIClips, only GUIClip.ParentClipScope");
		if (!Mathf.Approximately(m_StyleContext.currentPixelsPerPoint, GUIUtility.pixelsPerPoint))
		{
			m_RootContainer.Dirty(ChangeType.Styles);
			m_StyleContext.currentPixelsPerPoint = GUIUtility.pixelsPerPoint;
		}
		ValidatePersistentData();
		ValidateLayout();
		stylePainter.repaintEvent = e;
		Rect currentGlobalClip = ((visualTree.clippingOptions == VisualElement.ClippingOptions.NoClipping) ? GUIClip.topmostRect : visualTree.layout);
		PaintSubTree(e, visualTree, Matrix4x4.identity, currentGlobalClip);
	}
}
