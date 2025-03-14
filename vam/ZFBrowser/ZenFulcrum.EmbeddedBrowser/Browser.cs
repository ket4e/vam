using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using AOT;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZenFulcrum.EmbeddedBrowser;

public class Browser : MonoBehaviour
{
	[Flags]
	public enum NewWindowAction
	{
		Ignore = 1,
		Redirect = 2,
		NewBrowser = 3,
		NewWindow = 4
	}

	public delegate void JSCallback(JSONNode args);

	protected delegate void JSResultFunc(JSONNode value, bool isError);

	private static int lastUpdateFrame;

	protected IBrowserUI _uiHandler;

	protected bool uiHandlerAssigned;

	[Tooltip("Initial URL to load.\n\nTo change at runtime use browser.Url to load a page.")]
	[SerializeField]
	private string _url = string.Empty;

	[Tooltip("Initial size.\n\nTo change at runtime use browser.Resize.")]
	[SerializeField]
	private int _width = 512;

	[Tooltip("Initial size.\n\nTo change at runtime use browser.Resize.")]
	[SerializeField]
	private int _height = 512;

	[Tooltip("Generate mipmaps?\n\nGenerating mipmaps tends to be somewhat expensive, especially when updating a large texture every frame. Instead of\ngenerating mipmaps, try using one of the \"emulate mipmap\" shader variants.\n\nTo change at runtime modify this value and call browser.Resize.")]
	public bool generateMipmap;

	[Tooltip("Base background color to use for pages.\n\nThe texture will be cleared to this color until the page has rendered. Additionally, if baseColor.a is not\nfully opaque the browser will render transparently. (Don't forget to use an appropriate material for transparency.)\n\nDon't change this after the first Update() tick. (You can still tweak a page via EvalJS and CSS.)")]
	[FormerlySerializedAs("backgroundColor")]
	public Color32 baseColor = new Color32(0, 0, 0, 0);

	[Tooltip("Initial browser \"zoom level\". Negative numbers are smaller, zero is normal, positive numbers are larger.\nThe size roughly doubles/halves for every four units added/removed.\nNote that zoom level is shared by all pages on the some domain.\nAlso note that this zoom level may be persisted across runs.\n\nTo change at runtime use browser.Zoom.")]
	[SerializeField]
	private float _zoom;

	[Tooltip("Allow right-clicking to show a context menu on what parts of the page?\n\nMay be changed at any time.\n")]
	[FlagsField]
	public BrowserNative.ContextMenuOrigin allowContextMenuOn = BrowserNative.ContextMenuOrigin.Editable;

	[Tooltip("What should we do when a user/the page tries to open a new window?\n\nFor \"New Browser\" to work, you need to assign NewWindowHandler to a handler of your creation.\n\nDon't use \"New Window\" outside debugging and testing.\n\nUse SetNewWindowHandler to adjust at runtime.\n")]
	[SerializeField]
	private NewWindowAction newWindowAction = NewWindowAction.Redirect;

	[NonSerialized]
	protected internal int browserId;

	private int unsafeBrowserId;

	protected bool browserIdRequested;

	protected Texture2D texture;

	protected bool textureIsOurs;

	protected bool forceNextRender = true;

	protected bool isPopup;

	protected List<Action> thingsToDo = new List<Action>();

	protected List<Action> onloadActions = new List<Action>();

	protected List<object> thingsToRemember = new List<object>();

	protected static Dictionary<int, List<object>> allThingsToRemember = new Dictionary<int, List<object>>();

	private int nextCallbackId = 1;

	protected Dictionary<int, JSResultFunc> registeredCallbacks = new Dictionary<int, JSResultFunc>();

	public Action<int, JSONNode> onDownloadStarted;

	[HideInInspector]
	public readonly BrowserFocusState focusState = new BrowserFocusState();

	private BrowserInput browserInput;

	private Browser overlay;

	private bool skipNextLoad;

	private bool loadPending;

	private BrowserNavState navState = new BrowserNavState();

	private bool newWindowHandlerSet;

	private INewWindowHandler newWindowHandler;

	protected DialogHandler dialogHandler;

	private Action pageReplacer;

	private float pageReplacerPriority;

	protected List<Action> thingsToDoClone = new List<Action>();

	private Color32[] colorBuffer;

	internal static Dictionary<int, Browser> allBrowsers;

	public static string LocalUrlPrefix => BrowserNative.LocalUrlPrefix;

	public IBrowserUI UIHandler
	{
		get
		{
			return _uiHandler;
		}
		set
		{
			uiHandlerAssigned = true;
			_uiHandler = value;
		}
	}

	[Obsolete("Use SetNewWindowHandler", true)]
	public INewWindowHandler NewWindowHandler { get; set; }

	public bool EnableRendering { get; set; }

	public bool EnableInput { get; set; }

	public CookieManager CookieManager { get; private set; }

	public Texture2D Texture => texture;

	public bool IsReady => browserId != 0;

	public string Url
	{
		get
		{
			return navState.url;
		}
		set
		{
			LoadURL(value, force: true);
		}
	}

	public bool CanGoBack => navState.canGoBack;

	public bool CanGoForward => navState.canGoForward;

	public bool IsLoadingRaw => navState.loading;

	public bool IsLoaded
	{
		get
		{
			if (!IsReady || loadPending)
			{
				return false;
			}
			if (navState.loading)
			{
				return false;
			}
			string url = Url;
			bool flag = string.IsNullOrEmpty(url) || url == "about:blank";
			return !flag;
		}
	}

	public Vector2 Size => new Vector2(_width, _height);

	public float Zoom
	{
		get
		{
			return _zoom;
		}
		set
		{
			if (!DeferUnready(delegate
			{
				Zoom = value;
			}))
			{
				BrowserNative.zfb_setZoom(browserId, value);
				_zoom = value;
			}
		}
	}

	public event Action<string, string> onConsoleMessage = delegate
	{
	};

	public event Action<Texture2D> afterResize = delegate
	{
	};

	protected event BrowserNative.ReadyFunc onNativeReady;

	public event Action<JSONNode> onLoad = delegate
	{
	};

	[Obsolete("Doesn't fire reliably due to its design. Consider using onLoad or onNavStateChange.")]
	public event Action<JSONNode> onFetch = delegate
	{
	};

	public event Action<JSONNode> onFetchError = delegate
	{
	};

	public event Action<JSONNode> onCertError = delegate
	{
	};

	public event Action onSadTab = delegate
	{
	};

	public event Action onTextureUpdated = delegate
	{
	};

	public event Action onNavStateChange = delegate
	{
	};

	public event Action<int, JSONNode> onDownloadStatus = delegate
	{
	};

	public event Action<string, bool, string> onNodeFocus = delegate
	{
	};

	public event Action<bool, bool> onBrowserFocus = delegate
	{
	};

	public static event Action<Browser> onAnyBrowserCreated;

	public static event Action<Browser> onAnyBrowserDestroyed;

	protected void Awake()
	{
		EnableRendering = true;
		EnableInput = true;
		CookieManager = new CookieManager(this);
		browserInput = new BrowserInput(this);
		if (!newWindowHandlerSet)
		{
			SetNewWindowHandler((newWindowAction == NewWindowAction.NewBrowser) ? NewWindowAction.Ignore : newWindowAction, null);
		}
		onNativeReady += delegate
		{
			if (!uiHandlerAssigned)
			{
				MeshCollider component = GetComponent<MeshCollider>();
				if ((bool)component)
				{
					PointerUIMesh uIHandler = base.gameObject.AddComponent<PointerUIMesh>();
					base.gameObject.AddComponent<CursorRendererOS>();
					UIHandler = uIHandler;
				}
			}
			Resize(_width, _height);
			Zoom = _zoom;
			if (!isPopup && !string.IsNullOrEmpty(_url))
			{
				Url = _url;
			}
		};
		onConsoleMessage += delegate(string message, string source)
		{
			string message2 = source + ": " + message;
			Debug.Log(message2, this);
		};
		onFetchError += delegate(JSONNode err)
		{
			if (!(err["error"] == "ERR_ABORTED"))
			{
				QueuePageReplacer(delegate
				{
					LoadDataURI(ErrorGenerator.GenerateFetchError(err));
				}, -1000f);
			}
		};
		onCertError += delegate(JSONNode err)
		{
			QueuePageReplacer(delegate
			{
				LoadHTML(ErrorGenerator.GenerateCertError(err), Url);
			}, -900f);
		};
		onSadTab += delegate
		{
			QueuePageReplacer(delegate
			{
				LoadDataURI(ErrorGenerator.GenerateSadTabError());
			}, -1000f);
		};
		Browser.onAnyBrowserCreated(this);
	}

	public void WhenReady(Action callback)
	{
		if (IsReady)
		{
			lock (thingsToDo)
			{
				thingsToDo.Add(callback);
				return;
			}
		}
		BrowserNative.ReadyFunc func = null;
		func = delegate
		{
			try
			{
				callback();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			onNativeReady -= func;
		};
		onNativeReady += func;
	}

	public void RunOnMainThread(Action callback)
	{
		lock (thingsToDo)
		{
			thingsToDo.Add(callback);
		}
	}

	public void WhenLoaded(Action callback)
	{
		onloadActions.Add(callback);
	}

	internal void RequestNativeBrowser(int newBrowserId = 0)
	{
		if (browserId == 0 && !browserIdRequested)
		{
			browserIdRequested = true;
			try
			{
				BrowserNative.LoadNative();
			}
			catch
			{
				base.gameObject.SetActive(value: false);
				throw;
			}
			int num;
			if (newBrowserId == 0)
			{
				BrowserNative.ZFBSettings zFBSettings = default(BrowserNative.ZFBSettings);
				zFBSettings.bgR = baseColor.r;
				zFBSettings.bgG = baseColor.g;
				zFBSettings.bgB = baseColor.b;
				zFBSettings.bgA = baseColor.a;
				zFBSettings.offscreen = 1;
				BrowserNative.ZFBSettings settings = zFBSettings;
				num = BrowserNative.zfb_createBrowser(settings);
			}
			else
			{
				num = newBrowserId;
				isPopup = true;
			}
			unsafeBrowserId = num;
			allBrowsers[unsafeBrowserId] = this;
			lock (allThingsToRemember)
			{
				allThingsToRemember[num] = thingsToRemember;
			}
			BrowserNative.ForwardJSCallFunc forwardJSCallFunc = CB_ForwardJSCallFunc;
			thingsToRemember.Add(forwardJSCallFunc);
			BrowserNative.zfb_registerJSCallback(num, forwardJSCallFunc);
			BrowserNative.ChangeFunc changeFunc = CB_ChangeFunc;
			thingsToRemember.Add(changeFunc);
			BrowserNative.zfb_registerChangeCallback(num, changeFunc);
			BrowserNative.DisplayDialogFunc displayDialogFunc = CB_DisplayDialogFunc;
			thingsToRemember.Add(displayDialogFunc);
			BrowserNative.zfb_registerDialogCallback(num, displayDialogFunc);
			BrowserNative.ShowContextMenuFunc showContextMenuFunc = CB_ShowContextMenuFunc;
			thingsToRemember.Add(showContextMenuFunc);
			BrowserNative.zfb_registerContextMenuCallback(num, showContextMenuFunc);
			BrowserNative.ConsoleFunc consoleFunc = CB_ConsoleFunc;
			thingsToRemember.Add(consoleFunc);
			BrowserNative.zfb_registerConsoleCallback(num, consoleFunc);
			BrowserNative.ReadyFunc readyFunc = CB_ReadyFunc;
			thingsToRemember.Add(readyFunc);
			BrowserNative.zfb_setReadyCallback(num, readyFunc);
			BrowserNative.NavStateFunc navStateFunc = CB_NavStateFunc;
			thingsToRemember.Add(navStateFunc);
			BrowserNative.zfb_registerNavStateCallback(num, navStateFunc);
		}
	}

	protected void OnItemChange(BrowserNative.ChangeType type, string arg1)
	{
		switch (type)
		{
		case BrowserNative.ChangeType.CHT_CURSOR:
			UpdateCursor();
			break;
		case BrowserNative.ChangeType.CHT_BROWSER_CLOSE:
			break;
		case BrowserNative.ChangeType.CHT_FETCH_FINISHED:
			this.onFetch(JSONNode.Parse(arg1));
			break;
		case BrowserNative.ChangeType.CHT_FETCH_FAILED:
			this.onFetchError(JSONNode.Parse(arg1));
			break;
		case BrowserNative.ChangeType.CHT_LOAD_FINISHED:
			if (skipNextLoad)
			{
				skipNextLoad = false;
				break;
			}
			loadPending = false;
			navState.loading = false;
			if (onloadActions.Count != 0)
			{
				foreach (Action onloadAction in onloadActions)
				{
					onloadAction();
				}
				onloadActions.Clear();
			}
			this.onLoad(JSONNode.Parse(arg1));
			break;
		case BrowserNative.ChangeType.CHT_CERT_ERROR:
			this.onCertError(JSONNode.Parse(arg1));
			break;
		case BrowserNative.ChangeType.CHT_SAD_TAB:
			this.onSadTab();
			break;
		case BrowserNative.ChangeType.CHT_DOWNLOAD_STARTED:
		{
			JSONNode jSONNode3 = JSONNode.Parse(arg1);
			if (onDownloadStarted != null)
			{
				onDownloadStarted(jSONNode3["id"], jSONNode3);
			}
			else
			{
				DownloadCommand(jSONNode3["id"], BrowserNative.DownloadAction.Cancel);
			}
			break;
		}
		case BrowserNative.ChangeType.CHT_DOWNLOAD_STATUS:
		{
			JSONNode jSONNode2 = JSONNode.Parse(arg1);
			this.onDownloadStatus(jSONNode2["id"], jSONNode2);
			break;
		}
		case BrowserNative.ChangeType.CHT_FOCUSED_NODE:
		{
			JSONNode jSONNode = JSONNode.Parse(arg1);
			focusState.focusedTagName = jSONNode["TagName"];
			focusState.focusedNodeEditable = jSONNode["editable"];
			this.onNodeFocus(jSONNode["tagName"], jSONNode["editable"], jSONNode["value"]);
			break;
		}
		}
	}

	protected void CreateDialogHandler()
	{
		if (!(dialogHandler != null))
		{
			DialogHandler.DialogCallback dialogCallback = delegate(bool affirm, string text1, string text2)
			{
				CheckSanity();
				BrowserNative.zfb_sendDialogResults(browserId, affirm, text1, text2);
			};
			DialogHandler.MenuCallback contextCallback = delegate(int commandId)
			{
				CheckSanity();
				BrowserNative.zfb_sendContextMenuResults(browserId, commandId);
			};
			dialogHandler = DialogHandler.Create(this, dialogCallback, contextCallback);
		}
	}

	protected void CheckSanity()
	{
		if (browserId == 0)
		{
			throw new InvalidOperationException("No native browser allocated");
		}
		if (!BrowserNative.SymbolsLoaded)
		{
			throw new InvalidOperationException("Browser .dll not loaded");
		}
	}

	internal bool DeferUnready(Action ifNotReady)
	{
		if (browserId == 0)
		{
			WhenReady(ifNotReady);
			return true;
		}
		CheckSanity();
		return false;
	}

	protected void OnDisable()
	{
	}

	protected void OnDestroy()
	{
		Browser.onAnyBrowserDestroyed(this);
		if (browserId != 0)
		{
			if ((bool)dialogHandler)
			{
				UnityEngine.Object.DestroyImmediate(dialogHandler.gameObject);
			}
			dialogHandler = null;
			if (BrowserNative.SymbolsLoaded)
			{
				BrowserNative.zfb_destroyBrowser(browserId);
			}
			if (textureIsOurs)
			{
				UnityEngine.Object.Destroy(texture);
			}
			browserId = 0;
			texture = null;
		}
	}

	public void LoadURL(string url, bool force)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new ArgumentException("URL must be non-empty", "value");
		}
		if (!DeferUnready(delegate
		{
			LoadURL(url, force);
		}))
		{
			if (url.StartsWith("localGame://"))
			{
				url = LocalUrlPrefix + url.Substring("localGame://".Length);
			}
			loadPending = true;
			BrowserNative.zfb_goToURL(browserId, url, force);
		}
	}

	public void LoadHTML(string html, string url = null)
	{
		if (!DeferUnready(delegate
		{
			LoadHTML(html, url);
		}))
		{
			loadPending = true;
			if (string.IsNullOrEmpty(url))
			{
				url = LocalUrlPrefix + "custom";
			}
			if (string.IsNullOrEmpty(Url))
			{
				Url = "about:blank";
				skipNextLoad = true;
			}
			BrowserNative.zfb_goToHTML(browserId, html, url);
		}
	}

	public void LoadDataURI(string text, string mimeType = "text/html")
	{
		if (mimeType.StartsWith("text/") && !mimeType.Contains(";"))
		{
			mimeType += ";charset=UTF-8";
		}
		LoadDataURI(Encoding.UTF8.GetBytes(text), mimeType);
	}

	public void LoadDataURI(byte[] data, string mimeType)
	{
		string text = Convert.ToBase64String(data);
		Url = "data:" + mimeType + ";base64," + text;
	}

	public void SetNewWindowHandler(NewWindowAction action, INewWindowHandler newWindowHandler)
	{
		newWindowHandlerSet = true;
		if (action == NewWindowAction.NewBrowser && newWindowHandler == null)
		{
			throw new Exception("No new window handler supplied for NewBrowser action");
		}
		if (!DeferUnready(delegate
		{
			SetNewWindowHandler(action, newWindowHandler);
		}))
		{
			BrowserNative.ZFBSettings zFBSettings = default(BrowserNative.ZFBSettings);
			zFBSettings.bgR = baseColor.r;
			zFBSettings.bgG = baseColor.g;
			zFBSettings.bgB = baseColor.b;
			zFBSettings.bgA = baseColor.a;
			BrowserNative.ZFBSettings baseSettings = zFBSettings;
			this.newWindowHandler = newWindowHandler;
			newWindowAction = action;
			BrowserNative.NewWindowFunc newWindowFunc = CB_NewWindowFunc;
			thingsToRemember.Add(newWindowFunc);
			BrowserNative.zfb_registerPopupCallback(browserId, (BrowserNative.NewWindowAction)action, baseSettings, newWindowFunc);
		}
	}

	public void SendFrameCommand(BrowserNative.FrameCommand command)
	{
		if (!DeferUnready(delegate
		{
			SendFrameCommand(command);
		}))
		{
			BrowserNative.zfb_sendCommandToFocusedFrame(browserId, command);
		}
	}

	public void QueuePageReplacer(Action replacePage, float priority)
	{
		if (pageReplacer == null || priority >= pageReplacerPriority)
		{
			pageReplacer = replacePage;
			pageReplacerPriority = priority;
		}
	}

	public void GoBack()
	{
		if (IsReady)
		{
			CheckSanity();
			BrowserNative.zfb_doNav(browserId, -1);
		}
	}

	public void GoForward()
	{
		if (IsReady)
		{
			CheckSanity();
			BrowserNative.zfb_doNav(browserId, 1);
		}
	}

	public void Stop()
	{
		if (IsReady)
		{
			CheckSanity();
			BrowserNative.zfb_changeLoading(browserId, BrowserNative.LoadChange.LC_STOP);
		}
	}

	public void Reload(bool force = false)
	{
		if (IsReady)
		{
			CheckSanity();
			if (force)
			{
				BrowserNative.zfb_changeLoading(browserId, BrowserNative.LoadChange.LC_FORCE_RELOAD);
			}
			else
			{
				BrowserNative.zfb_changeLoading(browserId, BrowserNative.LoadChange.LC_RELOAD);
			}
		}
	}

	public void ShowDevTools(bool show = true)
	{
		if (!DeferUnready(delegate
		{
			ShowDevTools(show);
		}))
		{
			BrowserNative.zfb_showDevTools(browserId, show);
		}
	}

	protected void _Resize(Texture2D newTexture, bool newTextureIsOurs)
	{
		int width = newTexture.width;
		int height = newTexture.height;
		if (textureIsOurs && (bool)texture && newTexture != texture)
		{
			UnityEngine.Object.Destroy(texture);
		}
		_width = width;
		_height = height;
		if (IsReady)
		{
			BrowserNative.zfb_resize(browserId, width, height);
		}
		else
		{
			WhenReady(delegate
			{
				BrowserNative.zfb_resize(browserId, width, height);
			});
		}
		texture = newTexture;
		textureIsOurs = newTextureIsOurs;
		Renderer component = GetComponent<Renderer>();
		if ((bool)component)
		{
			component.material.mainTexture = texture;
		}
		this.afterResize(texture);
		if ((bool)overlay)
		{
			overlay.Resize(Texture);
		}
		forceNextRender = true;
	}

	public void Resize(int width, int height)
	{
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, generateMipmap);
		if (generateMipmap)
		{
			texture2D.filterMode = FilterMode.Trilinear;
		}
		texture2D.wrapMode = TextureWrapMode.Clamp;
		int num = width * height;
		if (texture2D.mipmapCount > 1)
		{
			for (int i = 1; i < texture2D.mipmapCount; i++)
			{
				num += texture2D.GetPixels32(i).Length;
			}
		}
		BrowserNative.LoadSymbols();
		IntPtr intPtr = BrowserNative.zfb_flatColorTexture(num, baseColor.r, baseColor.g, baseColor.b, baseColor.a);
		texture2D.LoadRawTextureData(intPtr, num * 4);
		texture2D.Apply();
		BrowserNative.zfb_free(intPtr);
		_Resize(texture2D, newTextureIsOurs: true);
	}

	public void Resize(Texture2D newTexture)
	{
		_Resize(newTexture, newTextureIsOurs: false);
	}

	public IPromise<JSONNode> EvalJS(string script, string scriptURL = "scripted command")
	{
		Promise<JSONNode> promise = new Promise<JSONNode>();
		int id = nextCallbackId++;
		string asJSON = new JSONNode(script).AsJSON;
		string resultJS = "try {_zfb_event(" + id + ", JSON.stringify(eval(" + asJSON + " )) || 'null');} catch(ex) {_zfb_event(" + id + ", 'fail-' + (JSON.stringify(ex.stack) || 'null'));}";
		registeredCallbacks.Add(id, delegate(JSONNode val, bool isError)
		{
			registeredCallbacks.Remove(id);
			if (isError)
			{
				promise.Reject(new JSException(val));
			}
			else
			{
				promise.Resolve(val);
			}
		});
		if (!IsLoaded)
		{
			WhenLoaded(delegate
			{
				_EvalJS(resultJS, scriptURL);
			});
		}
		else
		{
			_EvalJS(resultJS, scriptURL);
		}
		return promise;
	}

	public IPromise<JSONNode> EvalJSCSP(string script, string scriptURL = "scripted command")
	{
		Promise<JSONNode> promise = new Promise<JSONNode>();
		int id = nextCallbackId++;
		string resultJS = "try {_zfb_event(" + id + ", JSON.stringify( (function() {" + script + "})() ) || 'null');} catch(ex) {_zfb_event(" + id + ", 'fail-' + (JSON.stringify(ex.stack) || 'null'));}";
		registeredCallbacks.Add(id, delegate(JSONNode val, bool isError)
		{
			registeredCallbacks.Remove(id);
			if (isError)
			{
				promise.Reject(new JSException(val));
			}
			else
			{
				promise.Resolve(val);
			}
		});
		if (!IsLoaded)
		{
			WhenLoaded(delegate
			{
				_EvalJS(resultJS, scriptURL);
			});
		}
		else
		{
			_EvalJS(resultJS, scriptURL);
		}
		return promise;
	}

	protected void _EvalJS(string script, string scriptURL)
	{
		BrowserNative.zfb_evalJS(browserId, script, scriptURL);
	}

	public IPromise<JSONNode> CallFunction(string name, params JSONNode[] arguments)
	{
		string text = name + "(";
		string text2 = string.Empty;
		foreach (JSONNode jSONNode in arguments)
		{
			text = text + text2 + (jSONNode ?? JSONNode.NullNode).AsJSON;
			text2 = ", ";
		}
		text += ");";
		return EvalJS(text);
	}

	public void RegisterFunction(string name, JSCallback callback)
	{
		int num = nextCallbackId++;
		registeredCallbacks.Add(num, delegate(JSONNode value, bool error)
		{
			callback(value);
		});
		string script = name + " = function() { _zfb_event(" + num + ", JSON.stringify(Array.prototype.slice.call(arguments))); };";
		EvalJS(script);
	}

	protected void ProcessCallbacks()
	{
		while (thingsToDo.Count != 0)
		{
			lock (thingsToDo)
			{
				thingsToDoClone.AddRange(thingsToDo);
				thingsToDo.Clear();
			}
			foreach (Action item in thingsToDoClone)
			{
				item();
			}
			thingsToDoClone.Clear();
		}
	}

	protected void Update()
	{
		ProcessCallbacks();
		if (browserId == 0)
		{
			RequestNativeBrowser();
		}
		else if (BrowserNative.SymbolsLoaded)
		{
			HandleInput();
		}
	}

	protected void LateUpdate()
	{
		if (lastUpdateFrame != Time.frameCount && BrowserNative.NativeLoaded)
		{
			BrowserNative.zfb_tick();
			lastUpdateFrame = Time.frameCount;
		}
		if (browserId != 0)
		{
			ProcessCallbacks();
			if (pageReplacer != null)
			{
				pageReplacer();
				pageReplacer = null;
			}
			if (browserId != 0 && EnableRendering)
			{
				Render();
			}
		}
	}

	protected void Render()
	{
		if (!BrowserNative.SymbolsLoaded)
		{
			return;
		}
		CheckSanity();
		BrowserNative.RenderData renderData;
		try
		{
			renderData = BrowserNative.zfb_getImage(browserId, forceNextRender);
			forceNextRender = false;
			if (renderData.pixels == IntPtr.Zero || renderData.w != texture.width || renderData.h != texture.height)
			{
				return;
			}
		}
		finally
		{
		}
		if (texture.mipmapCount == 1)
		{
			texture.LoadRawTextureData(renderData.pixels, renderData.w * renderData.h * 4);
		}
		else
		{
			if (colorBuffer == null || colorBuffer.Length != renderData.w * renderData.h)
			{
				colorBuffer = new Color32[renderData.w * renderData.h];
			}
			GCHandle gCHandle = GCHandle.Alloc(colorBuffer, GCHandleType.Pinned);
			BrowserNative.zfb_copyToColor32(renderData.pixels, gCHandle.AddrOfPinnedObject(), renderData.w * renderData.h);
			gCHandle.Free();
			texture.SetPixels32(colorBuffer);
		}
		texture.Apply(updateMipmaps: true);
		this.onTextureUpdated();
	}

	public void SetOverlay(Browser overlayBrowser)
	{
		if (DeferUnready(delegate
		{
			SetOverlay(overlayBrowser);
		}) || ((bool)overlayBrowser && overlayBrowser.DeferUnready(delegate
		{
			SetOverlay(overlayBrowser);
		})))
		{
			return;
		}
		if (!overlayBrowser)
		{
			BrowserNative.zfb_setOverlay(browserId, 0);
			overlay = null;
			return;
		}
		overlay = overlayBrowser;
		if (!overlay.Texture || overlay.Texture.width != Texture.width || overlay.Texture.height != Texture.height)
		{
			overlay.Resize(Texture);
		}
		BrowserNative.zfb_setOverlay(browserId, overlayBrowser.browserId);
	}

	protected void HandleInput()
	{
		if (_uiHandler != null && EnableInput)
		{
			CheckSanity();
			browserInput.HandleInput();
		}
	}

	protected void OnApplicationFocus(bool focus)
	{
		if (!focus && browserInput != null)
		{
			browserInput.HandleFocusLoss();
		}
	}

	protected void OnApplicationPause(bool paused)
	{
		if (paused && browserInput != null)
		{
			browserInput.HandleFocusLoss();
		}
	}

	public void UpdateCursor()
	{
		if (UIHandler != null && !DeferUnready(UpdateCursor))
		{
			int width;
			int height;
			BrowserNative.CursorType cursorType = BrowserNative.zfb_getMouseCursor(browserId, out width, out height);
			if (cursorType != BrowserNative.CursorType.Custom)
			{
				UIHandler.BrowserCursor.SetActiveCursor(cursorType);
				return;
			}
			if (width == 0 && height == 0)
			{
				UIHandler.BrowserCursor.SetActiveCursor(BrowserNative.CursorType.None);
				return;
			}
			Color32[] array = new Color32[width * height];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			BrowserNative.zfb_getMouseCustomCursor(browserId, gCHandle.AddrOfPinnedObject(), width, height, out var hotX, out var hotY);
			gCHandle.Free();
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, mipmap: false);
			texture2D.SetPixels32(array);
			UIHandler.BrowserCursor.SetCustomCursor(texture2D, new Vector2(hotX, hotY));
			UnityEngine.Object.DestroyImmediate(texture2D);
		}
	}

	public void DownloadCommand(int downloadId, BrowserNative.DownloadAction action, string fileName = null)
	{
		CheckSanity();
		BrowserNative.zfb_downloadCommand(browserId, downloadId, action, fileName);
	}

	public void TypeText(string text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			Event @event = new Event();
			@event.type = EventType.KeyDown;
			@event.keyCode = KeyCode.None;
			@event.character = text[i];
			Event item = @event;
			browserInput.extraEventsToInject.Add(item);
		}
	}

	public void PressKey(KeyCode key, KeyAction action = KeyAction.PressAndRelease)
	{
		if (action == KeyAction.Press || action == KeyAction.PressAndRelease)
		{
			Event @event = new Event();
			@event.type = EventType.KeyDown;
			@event.keyCode = key;
			@event.character = '\0';
			Event item = @event;
			browserInput.extraEventsToInject.Add(item);
		}
		if (action == KeyAction.Release || action == KeyAction.PressAndRelease)
		{
			Event @event = new Event();
			@event.type = EventType.KeyUp;
			@event.keyCode = key;
			@event.character = '\0';
			Event item2 = @event;
			browserInput.extraEventsToInject.Add(item2);
		}
	}

	internal void _RaiseFocusEvent(bool mouseIsFocused, bool keyboardIsFocused)
	{
		focusState.hasMouseFocus = mouseIsFocused;
		focusState.hasKeyboardFocus = keyboardIsFocused;
		this.onBrowserFocus(mouseIsFocused, keyboardIsFocused);
	}

	private static Browser GetBrowser(int browserId)
	{
		lock (allBrowsers)
		{
			if (allBrowsers.TryGetValue(browserId, out var value))
			{
				return value;
			}
		}
		Debug.LogWarning("Got a callback for brower id " + browserId + " which doesn't exist.");
		return null;
	}

	[MonoPInvokeCallback(typeof(BrowserNative.ForwardJSCallFunc))]
	private static void CB_ForwardJSCallFunc(int browserId, int callbackId, string data, int size)
	{
		Browser browser = GetBrowser(browserId);
		if (!browser)
		{
			return;
		}
		lock (browser.thingsToDo)
		{
			browser.thingsToDo.Add(delegate
			{
				if (!browser.registeredCallbacks.TryGetValue(callbackId, out var value))
				{
					Debug.LogWarning("Got a JS callback for event " + callbackId + ", but no such event is registered.");
					return;
				}
				bool isError = false;
				if (data.StartsWith("fail-"))
				{
					isError = true;
					data = data.Substring(5);
				}
				JSONNode value2;
				try
				{
					value2 = JSONNode.Parse(data);
				}
				catch (SerializationException)
				{
					Debug.LogWarning("Invalid JSON sent from browser: " + data);
					return;
				}
				try
				{
					value(value2, isError);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			});
		}
	}

	[MonoPInvokeCallback(typeof(BrowserNative.ChangeFunc))]
	private static void CB_ChangeFunc(int browserId, BrowserNative.ChangeType changeType, string arg1)
	{
		Browser browser;
		lock (allBrowsers)
		{
			if (!allBrowsers.TryGetValue(browserId, out browser))
			{
				return;
			}
		}
		if (changeType == BrowserNative.ChangeType.CHT_BROWSER_CLOSE)
		{
			if ((bool)browser)
			{
				lock (browser.thingsToDo)
				{
					browser.thingsToDo.Add(delegate
					{
						UnityEngine.Object.Destroy(browser.gameObject);
					});
				}
			}
			lock (allThingsToRemember)
			{
				allThingsToRemember.Remove(browser.unsafeBrowserId);
			}
			lock (allBrowsers)
			{
				allBrowsers.Remove(browserId);
			}
			browser.browserId = 0;
		}
		else
		{
			if (!browser)
			{
				return;
			}
			lock (browser.thingsToDo)
			{
				browser.thingsToDo.Add(delegate
				{
					browser.OnItemChange(changeType, arg1);
				});
			}
		}
	}

	[MonoPInvokeCallback(typeof(BrowserNative.DisplayDialogFunc))]
	private static void CB_DisplayDialogFunc(int browserId, BrowserNative.DialogType dialogType, IntPtr textPtr, IntPtr promptTextPtr, IntPtr sourceURL)
	{
		Browser browser = GetBrowser(browserId);
		if (!browser)
		{
			return;
		}
		string text = Util.PtrToStringUTF8(textPtr);
		string promptText = Util.PtrToStringUTF8(promptTextPtr);
		lock (browser.thingsToDo)
		{
			browser.thingsToDo.Add(delegate
			{
				browser.CreateDialogHandler();
				browser.dialogHandler.HandleDialog(dialogType, text, promptText);
			});
		}
	}

	[MonoPInvokeCallback(typeof(BrowserNative.ShowContextMenuFunc))]
	private static void CB_ShowContextMenuFunc(int browserId, string json, int x, int y, BrowserNative.ContextMenuOrigin origin)
	{
		Browser browser = GetBrowser(browserId);
		if (!browser)
		{
			return;
		}
		if (json != null && (browser.allowContextMenuOn & origin) == 0)
		{
			BrowserNative.zfb_sendContextMenuResults(browserId, -1);
			return;
		}
		lock (browser.thingsToDo)
		{
			browser.thingsToDo.Add(delegate
			{
				if (json != null)
				{
					browser.CreateDialogHandler();
				}
				if (browser.dialogHandler != null)
				{
					browser.dialogHandler.HandleContextMenu(json, x, y);
				}
			});
		}
	}

	[MonoPInvokeCallback(typeof(BrowserNative.ConsoleFunc))]
	private static void CB_ConsoleFunc(int browserId, string message, string source, int line)
	{
		Browser browser = GetBrowser(browserId);
		if (!browser)
		{
			return;
		}
		lock (browser.thingsToDo)
		{
			browser.thingsToDo.Add(delegate
			{
				browser.onConsoleMessage(message, source + ":" + line);
			});
		}
	}

	[MonoPInvokeCallback(typeof(BrowserNative.ReadyFunc))]
	private static void CB_ReadyFunc(int browserId)
	{
		Browser browser = GetBrowser(browserId);
		if (!browser)
		{
			return;
		}
		lock (browser.thingsToDo)
		{
			browser.thingsToDo.Add(delegate
			{
				browser.browserId = browserId;
				browser.onNativeReady(browserId);
			});
		}
	}

	[MonoPInvokeCallback(typeof(BrowserNative.NavStateFunc))]
	private static void CB_NavStateFunc(int browserId, bool canGoBack, bool canGoForward, bool lodaing, IntPtr urlRaw)
	{
		Browser browser = GetBrowser(browserId);
		if (!browser)
		{
			return;
		}
		string url = Util.PtrToStringUTF8(urlRaw);
		lock (browser.thingsToDo)
		{
			browser.thingsToDo.Add(delegate
			{
				browser.navState.canGoBack = canGoBack;
				browser.navState.canGoForward = canGoForward;
				browser.navState.loading = lodaing;
				browser.navState.url = url;
				browser._url = url;
				browser.onNavStateChange();
			});
		}
	}

	[MonoPInvokeCallback(typeof(BrowserNative.NewWindowFunc))]
	private static void CB_NewWindowFunc(int creatorBrowserId, int newBrowserId, IntPtr urlPtr)
	{
		Browser browser = GetBrowser(creatorBrowserId);
		if (!browser)
		{
			return;
		}
		lock (browser.thingsToDo)
		{
			browser.thingsToDo.Add(delegate
			{
				Browser browser2 = browser.newWindowHandler.CreateBrowser(browser);
				browser2.RequestNativeBrowser(newBrowserId);
			});
		}
	}

	static Browser()
	{
		Browser.onAnyBrowserCreated = delegate
		{
		};
		Browser.onAnyBrowserDestroyed = delegate
		{
		};
		allBrowsers = new Dictionary<int, Browser>();
	}
}
