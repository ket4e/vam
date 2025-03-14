using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public static class BrowserNative
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void MessageFunc(string message);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void NewRequestFunc(int requestId, string url);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ReadyFunc(int browserId);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ConsoleFunc(int browserId, string message, string source, int line);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ForwardJSCallFunc(int browserId, int callbackId, string data, int size);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void NewWindowFunc(int creatorBrowserId, int newBrowserId, IntPtr initialURL);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ChangeFunc(int browserId, ChangeType changeType, string arg1);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void DisplayDialogFunc(int browserId, DialogType dialogType, IntPtr dialogText, IntPtr initialPromptText, IntPtr sourceURL);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ShowContextMenuFunc(int browserId, string menuJSON, int x, int y, ContextMenuOrigin origin);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void GetCookieFunc(NativeCookie cookie);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void NavStateFunc(int browserId, bool canGoBack, bool canGoForward, bool lodaing, IntPtr url);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void WindowCallbackFunc(int windowId, IntPtr data);

	public enum LoadChange
	{
		LC_STOP = 1,
		LC_RELOAD,
		LC_FORCE_RELOAD
	}

	public enum MouseButton
	{
		MBT_LEFT,
		MBT_MIDDLE,
		MBT_RIGHT
	}

	public enum ChangeType
	{
		CHT_CURSOR,
		CHT_BROWSER_CLOSE,
		CHT_FETCH_FINISHED,
		CHT_FETCH_FAILED,
		CHT_LOAD_FINISHED,
		CHT_CERT_ERROR,
		CHT_SAD_TAB,
		CHT_DOWNLOAD_STARTED,
		CHT_DOWNLOAD_STATUS,
		CHT_FOCUSED_NODE
	}

	public enum DownloadAction
	{
		Begin,
		Cancel,
		Pause,
		Resume
	}

	public enum CursorType
	{
		Pointer,
		Cross,
		Hand,
		IBeam,
		Wait,
		Help,
		EastResize,
		NorthResize,
		NorthEastResize,
		NorthWestResize,
		SouthResize,
		SouthEastResize,
		SouthWestResize,
		WestResize,
		NorthSouthResize,
		EastWestResize,
		NorthEastSouthWestResize,
		NorthWestSouthEastResize,
		ColumnResize,
		RowResize,
		MiddlePanning,
		EastPanning,
		NorthPanning,
		NorthEastPanning,
		NorthWestPanning,
		SouthPanning,
		SouthEastPanning,
		SouthWestPanning,
		WestPanning,
		Move,
		VerticalText,
		Cell,
		ContextMenu,
		Alias,
		Progress,
		NoDrop,
		Copy,
		None,
		NotAllowed,
		ZoomIn,
		ZoomOut,
		Grab,
		Grabbing,
		Custom
	}

	public enum DialogType
	{
		DLT_HIDE,
		DLT_ALERT,
		DLT_CONFIRM,
		DLT_PROMPT,
		DLT_PAGE_UNLOAD,
		DLT_PAGE_RELOAD,
		DLT_GET_AUTH
	}

	public enum NewWindowAction
	{
		NWA_IGNORE = 1,
		NWA_REDIRECT,
		NWA_NEW_BROWSER,
		NWA_NEW_WINDOW
	}

	[Flags]
	public enum ContextMenuOrigin
	{
		Editable = 2,
		Image = 4,
		Selection = 8,
		Other = 1
	}

	public enum FrameCommand
	{
		Undo,
		Redo,
		Cut,
		Copy,
		Paste,
		Delete,
		SelectAll,
		ViewSource
	}

	public enum CookieAction
	{
		Delete,
		Create
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ZFBInitialSettings
	{
		public string cefPath;

		public string localePath;

		public string subprocessFile;

		public string userAgent;

		public string logFile;

		public string profilePath;

		public int debugPort;

		public int multiThreadedMessageLoop;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ZFBSettings
	{
		public int bgR;

		public int bgG;

		public int bgB;

		public int bgA;

		public int offscreen;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct RenderData
	{
		public IntPtr pixels;

		public int w;

		public int h;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class NativeCookie
	{
		public string name;

		public string value;

		public string domain;

		public string path;

		public string creation;

		public string lastAccess;

		public string expires;

		public byte secure;

		public byte httpOnly;
	}

	public delegate void Calltype_zfb_noop();

	public delegate IntPtr Calltype_zfb_flatColorTexture(int pixelCount, int r, int g, int b, int a);

	public delegate void Calltype_zfb_copyToColor32(IntPtr src, IntPtr dest, int pixelCount);

	public delegate void Calltype_zfb_free(IntPtr mem);

	public delegate void Calltype_zfb_memcpy(IntPtr dst, IntPtr src, int size);

	public delegate IntPtr Calltype_zfb_getVersion();

	public delegate void Calltype_zfb_setDebugFunc(MessageFunc debugFunc);

	public delegate void Calltype_zfb_setLocalRequestHandler(NewRequestFunc requestFunc);

	public delegate void Calltype_zfb_sendRequestHeaders(int requestId, int responseLength, string headersJSON);

	public delegate void Calltype_zfb_sendRequestData(int requestId, IntPtr data, int dataSize);

	public delegate void Calltype_zfb_setCallbacksEnabled(bool enabled);

	public delegate void Calltype_zfb_destroyAllBrowsers();

	public delegate void Calltype_zfb_addCLISwitch(string value);

	public delegate bool Calltype_zfb_init(ZFBInitialSettings settings);

	public delegate void Calltype_zfb_shutdown();

	public delegate int Calltype_zfb_createBrowser(ZFBSettings settings);

	public delegate int Calltype_zfb_numBrowsers();

	public delegate void Calltype_zfb_destroyBrowser(int id);

	public delegate void Calltype_zfb_tick();

	public delegate void Calltype_zfb_setReadyCallback(int id, ReadyFunc cb);

	public delegate void Calltype_zfb_resize(int id, int w, int h);

	public delegate void Calltype_zfb_setOverlay(int browserId, int overlayBrowserId);

	public delegate RenderData Calltype_zfb_getImage(int id, bool forceDirty);

	public delegate void Calltype_zfb_registerNavStateCallback(int id, NavStateFunc callback);

	public delegate void Calltype_zfb_goToURL(int id, string url, bool force);

	public delegate void Calltype_zfb_goToHTML(int id, string html, string url);

	public delegate void Calltype_zfb_doNav(int id, int direction);

	public delegate void Calltype_zfb_setZoom(int id, double zoom);

	public delegate void Calltype_zfb_changeLoading(int id, LoadChange what);

	public delegate void Calltype_zfb_showDevTools(int id, bool show);

	public delegate void Calltype_zfb_setFocused(int id, bool focused);

	public delegate void Calltype_zfb_mouseMove(int id, float x, float y);

	public delegate void Calltype_zfb_mouseButton(int id, MouseButton button, bool down, int clickCount);

	public delegate void Calltype_zfb_mouseScroll(int id, int deltaX, int deltaY);

	public delegate void Calltype_zfb_keyEvent(int id, bool down, int windowsKeyCode);

	public delegate void Calltype_zfb_characterEvent(int id, int character, int windowsKeyCode);

	public delegate void Calltype_zfb_registerConsoleCallback(int id, ConsoleFunc callback);

	public delegate void Calltype_zfb_evalJS(int id, string script, string scriptURL);

	public delegate void Calltype_zfb_registerJSCallback(int id, ForwardJSCallFunc cb);

	public delegate void Calltype_zfb_registerChangeCallback(int id, ChangeFunc cb);

	public delegate CursorType Calltype_zfb_getMouseCursor(int id, out int width, out int height);

	public delegate void Calltype_zfb_getMouseCustomCursor(int id, IntPtr buffer, int width, int height, out int hotX, out int hotY);

	public delegate void Calltype_zfb_registerDialogCallback(int id, DisplayDialogFunc cb);

	public delegate void Calltype_zfb_sendDialogResults(int id, bool affirmed, string text1, string text2);

	public delegate void Calltype_zfb_registerPopupCallback(int id, NewWindowAction windowAction, ZFBSettings baseSettings, NewWindowFunc cb);

	public delegate void Calltype_zfb_registerContextMenuCallback(int id, ShowContextMenuFunc cb);

	public delegate void Calltype_zfb_sendContextMenuResults(int id, int commandId);

	public delegate void Calltype_zfb_sendCommandToFocusedFrame(int id, FrameCommand command);

	public delegate void Calltype_zfb_getCookies(int id, GetCookieFunc cb);

	public delegate void Calltype_zfb_editCookie(int id, NativeCookie cookie, CookieAction action);

	public delegate void Calltype_zfb_clearCookies(int id);

	public delegate void Calltype_zfb_downloadCommand(int id, int downloadId, DownloadAction command, string fileName);

	public const int DebugPort = 9849;

	public static readonly object symbolsLock = new object();

	public const bool UsingAPIProxy = true;

	public static List<string> commandLineSwitches = new List<string> { "--disable-smooth-scrolling", "--enable-system-flash" };

	public static WebResources webResources;

	private static bool isAppDomainUnloading = false;

	private static string _profilePath = null;

	private static IntPtr moduleHandle;

	public static Calltype_zfb_noop zfb_noop;

	public static Calltype_zfb_flatColorTexture zfb_flatColorTexture;

	public static Calltype_zfb_copyToColor32 zfb_copyToColor32;

	public static Calltype_zfb_free zfb_free;

	public static Calltype_zfb_memcpy zfb_memcpy;

	public static Calltype_zfb_getVersion zfb_getVersion;

	public static Calltype_zfb_setDebugFunc zfb_setDebugFunc;

	public static Calltype_zfb_setLocalRequestHandler zfb_setLocalRequestHandler;

	public static Calltype_zfb_sendRequestHeaders zfb_sendRequestHeaders;

	public static Calltype_zfb_sendRequestData zfb_sendRequestData;

	public static Calltype_zfb_setCallbacksEnabled zfb_setCallbacksEnabled;

	public static Calltype_zfb_destroyAllBrowsers zfb_destroyAllBrowsers;

	public static Calltype_zfb_addCLISwitch zfb_addCLISwitch;

	public static Calltype_zfb_init zfb_init;

	public static Calltype_zfb_shutdown zfb_shutdown;

	public static Calltype_zfb_createBrowser zfb_createBrowser;

	public static Calltype_zfb_numBrowsers zfb_numBrowsers;

	public static Calltype_zfb_destroyBrowser zfb_destroyBrowser;

	public static Calltype_zfb_tick zfb_tick;

	public static Calltype_zfb_setReadyCallback zfb_setReadyCallback;

	public static Calltype_zfb_resize zfb_resize;

	public static Calltype_zfb_setOverlay zfb_setOverlay;

	public static Calltype_zfb_getImage zfb_getImage;

	public static Calltype_zfb_registerNavStateCallback zfb_registerNavStateCallback;

	public static Calltype_zfb_goToURL zfb_goToURL;

	public static Calltype_zfb_goToHTML zfb_goToHTML;

	public static Calltype_zfb_doNav zfb_doNav;

	public static Calltype_zfb_setZoom zfb_setZoom;

	public static Calltype_zfb_changeLoading zfb_changeLoading;

	public static Calltype_zfb_showDevTools zfb_showDevTools;

	public static Calltype_zfb_setFocused zfb_setFocused;

	public static Calltype_zfb_mouseMove zfb_mouseMove;

	public static Calltype_zfb_mouseButton zfb_mouseButton;

	public static Calltype_zfb_mouseScroll zfb_mouseScroll;

	public static Calltype_zfb_keyEvent zfb_keyEvent;

	public static Calltype_zfb_characterEvent zfb_characterEvent;

	public static Calltype_zfb_registerConsoleCallback zfb_registerConsoleCallback;

	public static Calltype_zfb_evalJS zfb_evalJS;

	public static Calltype_zfb_registerJSCallback zfb_registerJSCallback;

	public static Calltype_zfb_registerChangeCallback zfb_registerChangeCallback;

	public static Calltype_zfb_getMouseCursor zfb_getMouseCursor;

	public static Calltype_zfb_getMouseCustomCursor zfb_getMouseCustomCursor;

	public static Calltype_zfb_registerDialogCallback zfb_registerDialogCallback;

	public static Calltype_zfb_sendDialogResults zfb_sendDialogResults;

	public static Calltype_zfb_registerPopupCallback zfb_registerPopupCallback;

	public static Calltype_zfb_registerContextMenuCallback zfb_registerContextMenuCallback;

	public static Calltype_zfb_sendContextMenuResults zfb_sendContextMenuResults;

	public static Calltype_zfb_sendCommandToFocusedFrame zfb_sendCommandToFocusedFrame;

	public static Calltype_zfb_getCookies zfb_getCookies;

	public static Calltype_zfb_editCookie zfb_editCookie;

	public static Calltype_zfb_clearCookies zfb_clearCookies;

	public static Calltype_zfb_downloadCommand zfb_downloadCommand;

	public static bool NativeLoaded { get; private set; }

	public static bool SymbolsLoaded { get; private set; }

	public static string LocalUrlPrefix => "https://game.local/";

	public static string ProfilePath
	{
		get
		{
			return _profilePath;
		}
		set
		{
			if (NativeLoaded)
			{
				throw new InvalidOperationException("ProfilePath must be set before initializing the browser system.");
			}
			_profilePath = value;
		}
	}

	[MonoPInvokeCallback(typeof(MessageFunc))]
	private static void LogCallback(string message)
	{
		Debug.Log("ZFWeb: " + message);
	}

	public static void LoadSymbols()
	{
		if (!SymbolsLoaded)
		{
			if (isAppDomainUnloading)
			{
				throw new Exception("Tried to start up browser backend while unloading app domain.");
			}
			FileLocations.CEFDirs dirs = FileLocations.Dirs;
			HandLoadSymbols(dirs.binariesPath);
		}
	}

	public static void LoadNative()
	{
		if (NativeLoaded)
		{
			return;
		}
		if (webResources == null)
		{
			StandaloneWebResources standaloneWebResources = new StandaloneWebResources(Application.dataPath + "/Resources/browser_assets");
			standaloneWebResources.LoadIndex();
			webResources = standaloneWebResources;
		}
		int debugPort = (Debug.isDebugBuild ? 9849 : 0);
		FileLocations.CEFDirs dirs = FileLocations.Dirs;
		if (!dirs.logFileIsUnityLog)
		{
			FileInfo fileInfo = new FileInfo(dirs.logFile);
			try
			{
				if (fileInfo.Exists)
				{
					fileInfo.Delete();
				}
			}
			catch
			{
			}
		}
		string fullName = Directory.GetParent(Application.dataPath).FullName;
		string environmentVariable = Environment.GetEnvironmentVariable("PATH");
		environmentVariable = environmentVariable + ";" + fullName;
		Environment.SetEnvironmentVariable("PATH", environmentVariable);
		LoadSymbols();
		StandaloneShutdown.Create();
		zfb_destroyAllBrowsers();
		zfb_setDebugFunc(LogCallback);
		zfb_setLocalRequestHandler(NewRequestCallback);
		zfb_setCallbacksEnabled(enabled: true);
		ZFBInitialSettings zFBInitialSettings = default(ZFBInitialSettings);
		zFBInitialSettings.cefPath = dirs.resourcesPath;
		zFBInitialSettings.localePath = dirs.localesPath;
		zFBInitialSettings.subprocessFile = dirs.subprocessFile;
		zFBInitialSettings.userAgent = UserAgent.GetUserAgent();
		zFBInitialSettings.logFile = dirs.logFile;
		zFBInitialSettings.profilePath = _profilePath;
		zFBInitialSettings.debugPort = debugPort;
		zFBInitialSettings.multiThreadedMessageLoop = 0;
		ZFBInitialSettings settings = zFBInitialSettings;
		foreach (string commandLineSwitch in commandLineSwitches)
		{
			zfb_addCLISwitch(commandLineSwitch);
		}
		if (!zfb_init(settings))
		{
			throw new Exception("Failed to initialize browser system.");
		}
		NativeLoaded = true;
		AppDomain.CurrentDomain.DomainUnload += delegate
		{
			isAppDomainUnloading = true;
			UnloadNative();
		};
	}

	private static void FixProcessPermissions(FileLocations.CEFDirs dirs)
	{
		uint attributes = (uint)File.GetAttributes(dirs.subprocessFile);
		attributes |= 0x80000000u;
		File.SetAttributes(dirs.subprocessFile, (FileAttributes)attributes);
	}

	private static void HandLoadSymbols(string binariesPath)
	{
		string text = "ZFProxyWeb";
		string name = binariesPath + "/" + text + ".dll";
		moduleHandle = OpenLib(name);
		int num = 0;
		FieldInfo[] fields = typeof(BrowserNative).GetFields(BindingFlags.Static | BindingFlags.Public);
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.Name.StartsWith("zfb_"))
			{
				IntPtr func = GetFunc(moduleHandle, fieldInfo.Name);
				Delegate delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer(func, fieldInfo.FieldType);
				fieldInfo.SetValue(null, delegateForFunctionPointer);
				num++;
			}
		}
		SymbolsLoaded = true;
	}

	private static void ClearSymbols()
	{
		SymbolsLoaded = false;
		FieldInfo[] fields = typeof(BrowserNative).GetFields(BindingFlags.Static | BindingFlags.Public);
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.Name.StartsWith("zfb_"))
			{
				fieldInfo.SetValue(null, null);
			}
		}
	}

	private static string GetLibError()
	{
		return new Win32Exception(Marshal.GetLastWin32Error()).Message;
	}

	private static IntPtr OpenLib(string name)
	{
		IntPtr intPtr = LoadLibraryW(name);
		if (intPtr == IntPtr.Zero)
		{
			throw new DllNotFoundException("ZFBrowser failed to load " + name + ": " + new Win32Exception(Marshal.GetLastWin32Error()).Message);
		}
		return intPtr;
	}

	private static void CloseLib()
	{
		if (!(moduleHandle == IntPtr.Zero))
		{
			ClearSymbols();
			if (!FreeLibrary(moduleHandle))
			{
				throw new DllNotFoundException("Failed to unload library: " + GetLibError());
			}
			moduleHandle = IntPtr.Zero;
		}
	}

	private static IntPtr GetFunc(IntPtr libHandle, string fnName)
	{
		IntPtr procAddress = GetProcAddress(libHandle, fnName);
		if (procAddress == IntPtr.Zero)
		{
			throw new DllNotFoundException("ZFBrowser failed to load method " + fnName + ": " + Marshal.GetLastWin32Error());
		}
		return procAddress;
	}

	[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
	private static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

	[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool FreeLibrary(IntPtr hModule);

	[MonoPInvokeCallback(typeof(NewRequestFunc))]
	private static void NewRequestCallback(int requestId, string url)
	{
		webResources.HandleRequest(requestId, url);
	}

	public static void UnloadNative()
	{
		if (!NativeLoaded)
		{
			return;
		}
		lock (symbolsLock)
		{
			zfb_destroyAllBrowsers();
			zfb_shutdown();
			zfb_setCallbacksEnabled(enabled: false);
			NativeLoaded = false;
			CloseLib();
		}
	}
}
