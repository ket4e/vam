using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Mono.WebBrowser;

namespace Mono.Mozilla;

internal class Base
{
	private class BindingInfo
	{
		public CallbackBinder callback;

		public IntPtr gluezilla;
	}

	private static Hashtable boundControls;

	private static bool initialized;

	private static object initLock;

	private static string monoMozDir;

	static Base()
	{
		initLock = new object();
		boundControls = new Hashtable();
	}

	private static bool isInitialized()
	{
		if (!initialized)
		{
			return false;
		}
		return true;
	}

	private static BindingInfo getBinding(IWebBrowser control)
	{
		if (!boundControls.ContainsKey(control))
		{
			return null;
		}
		return boundControls[control] as BindingInfo;
	}

	public static void Debug(int signal)
	{
		gluezilla_debug(signal);
	}

	public static bool Init(WebBrowser control, Platform platform)
	{
		lock (initLock)
		{
			if (!initialized)
			{
				Platform mozPlatform;
				try
				{
					short num = gluezilla_init(platform, out mozPlatform);
					monoMozDir = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ".mono"), "mozilla-" + num);
					if (!Directory.Exists(monoMozDir))
					{
						Directory.CreateDirectory(monoMozDir);
					}
				}
				catch (DllNotFoundException)
				{
					Console.WriteLine("libgluezilla not found. To have webbrowser support, you need libgluezilla installed");
					initialized = false;
					return false;
				}
				control.enginePlatform = mozPlatform;
				initialized = true;
			}
		}
		return initialized;
	}

	public static bool Bind(WebBrowser control, IntPtr handle, int width, int height)
	{
		if (!isInitialized())
		{
			return false;
		}
		BindingInfo bindingInfo = new BindingInfo();
		bindingInfo.callback = new CallbackBinder(control.callbacks);
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(bindingInfo.callback));
		Marshal.StructureToPtr(bindingInfo.callback, intPtr, fDeleteOld: true);
		bindingInfo.gluezilla = gluezilla_bind(intPtr, handle, width, height, Environment.CurrentDirectory, monoMozDir, control.platform);
		lock (initLock)
		{
			if (bindingInfo.gluezilla == IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
				bindingInfo = null;
				initialized = false;
				return false;
			}
		}
		boundControls.Add(control, bindingInfo);
		return true;
	}

	public static bool Create(IWebBrowser control)
	{
		if (!isInitialized())
		{
			return false;
		}
		BindingInfo binding = getBinding(control);
		gluezilla_createBrowserWindow(binding.gluezilla);
		return true;
	}

	public static void Shutdown(IWebBrowser control)
	{
		lock (initLock)
		{
			if (initialized)
			{
				BindingInfo binding = getBinding(control);
				gluezilla_shutdown(binding.gluezilla);
				boundControls.Remove(control);
				if (boundControls.Count == 0)
				{
					binding = null;
					initialized = false;
				}
			}
		}
	}

	public static void Focus(IWebBrowser control, FocusOption focus)
	{
		if (isInitialized())
		{
			BindingInfo binding = getBinding(control);
			gluezilla_focus(binding.gluezilla, focus);
		}
	}

	public static void Blur(IWebBrowser control)
	{
		if (isInitialized())
		{
			BindingInfo binding = getBinding(control);
			gluezilla_blur(binding.gluezilla);
		}
	}

	public static void Activate(IWebBrowser control)
	{
		if (isInitialized())
		{
			BindingInfo binding = getBinding(control);
			gluezilla_activate(binding.gluezilla);
		}
	}

	public static void Deactivate(IWebBrowser control)
	{
		if (isInitialized())
		{
			BindingInfo binding = getBinding(control);
			gluezilla_deactivate(binding.gluezilla);
		}
	}

	public static void Resize(IWebBrowser control, int width, int height)
	{
		if (isInitialized())
		{
			BindingInfo binding = getBinding(control);
			gluezilla_resize(binding.gluezilla, width, height);
		}
	}

	public static void Home(IWebBrowser control)
	{
		if (isInitialized())
		{
			BindingInfo binding = getBinding(control);
			gluezilla_home(binding.gluezilla);
		}
	}

	public static nsIWebNavigation GetWebNavigation(IWebBrowser control)
	{
		if (!isInitialized())
		{
			return null;
		}
		BindingInfo binding = getBinding(control);
		return gluezilla_getWebNavigation(binding.gluezilla);
	}

	public static IntPtr StringInit()
	{
		if (!isInitialized())
		{
			return IntPtr.Zero;
		}
		return gluezilla_stringInit();
	}

	public static void StringFinish(HandleRef str)
	{
		if (isInitialized())
		{
			gluezilla_stringFinish(str);
		}
	}

	public static string StringGet(HandleRef str)
	{
		if (!isInitialized())
		{
			return string.Empty;
		}
		IntPtr ptr = gluezilla_stringGet(str);
		return Marshal.PtrToStringUni(ptr);
	}

	public static void StringSet(HandleRef str, string text)
	{
		if (isInitialized())
		{
			gluezilla_stringSet(str, text);
		}
	}

	public static object GetProxyForObject(IWebBrowser control, Guid iid, object obj)
	{
		if (!isInitialized())
		{
			return null;
		}
		BindingInfo binding = getBinding(control);
		gluezilla_getProxyForObject(binding.gluezilla, iid, obj, out var ret);
		return Marshal.GetObjectForIUnknown(ret);
	}

	public static nsIServiceManager GetServiceManager(IWebBrowser control)
	{
		if (!isInitialized())
		{
			return null;
		}
		BindingInfo binding = getBinding(control);
		return gluezilla_getServiceManager2(binding.gluezilla);
	}

	public static string EvalScript(IWebBrowser control, string script)
	{
		if (!isInitialized())
		{
			return null;
		}
		BindingInfo binding = getBinding(control);
		IntPtr ptr = gluezilla_evalScript(binding.gluezilla, script);
		return Marshal.PtrToStringAuto(ptr);
	}

	[DllImport("gluezilla")]
	private static extern void gluezilla_debug(int signal);

	[DllImport("gluezilla")]
	private static extern short gluezilla_init(Platform platform, out Platform mozPlatform);

	[DllImport("gluezilla")]
	private static extern IntPtr gluezilla_bind(IntPtr events, IntPtr hwnd, int width, int height, string startDir, string dataDir, Platform platform);

	[DllImport("gluezilla")]
	private static extern int gluezilla_createBrowserWindow(IntPtr instance);

	[DllImport("gluezilla")]
	private static extern IntPtr gluezilla_shutdown(IntPtr instance);

	[DllImport("gluezilla")]
	private static extern int gluezilla_focus(IntPtr instance, FocusOption focus);

	[DllImport("gluezilla")]
	private static extern int gluezilla_blur(IntPtr instance);

	[DllImport("gluezilla")]
	private static extern int gluezilla_activate(IntPtr instance);

	[DllImport("gluezilla")]
	private static extern int gluezilla_deactivate(IntPtr instance);

	[DllImport("gluezilla")]
	private static extern int gluezilla_resize(IntPtr instance, int width, int height);

	[DllImport("gluezilla")]
	private static extern int gluezilla_home(IntPtr instance);

	[DllImport("gluezilla")]
	[return: MarshalAs(UnmanagedType.Interface)]
	private static extern nsIWebNavigation gluezilla_getWebNavigation(IntPtr instance);

	[DllImport("gluezilla")]
	private static extern IntPtr gluezilla_stringInit();

	[DllImport("gluezilla")]
	private static extern int gluezilla_stringFinish(HandleRef str);

	[DllImport("gluezilla")]
	private static extern IntPtr gluezilla_stringGet(HandleRef str);

	[DllImport("gluezilla")]
	private static extern void gluezilla_stringSet(HandleRef str, [MarshalAs(UnmanagedType.LPWStr)] string text);

	[DllImport("gluezilla")]
	private static extern void gluezilla_getProxyForObject(IntPtr instance, [MarshalAs(UnmanagedType.LPStruct)] Guid iid, [MarshalAs(UnmanagedType.Interface)] object obj, out IntPtr ret);

	[DllImport("gluezilla")]
	public static extern uint gluezilla_StringContainerInit(HandleRef aStr);

	[DllImport("gluezilla")]
	public static extern void gluezilla_StringContainerFinish(HandleRef aStr);

	[DllImport("gluezilla")]
	public static extern uint gluezilla_StringGetData(HandleRef aStr, out IntPtr aBuf, out bool aTerm);

	[DllImport("gluezilla")]
	public static extern uint gluezilla_StringSetData(HandleRef aStr, [MarshalAs(UnmanagedType.LPWStr)] string aBuf, uint aCount);

	[DllImport("gluezilla")]
	public static extern uint gluezilla_CStringContainerInit(HandleRef aStr);

	[DllImport("gluezilla")]
	public static extern void gluezilla_CStringContainerFinish(HandleRef aStr);

	[DllImport("gluezilla")]
	public static extern uint gluezilla_CStringGetData(HandleRef aStr, out IntPtr aBuf, out bool aTerm);

	[DllImport("gluezilla")]
	public static extern uint gluezilla_CStringSetData(HandleRef aStr, string aBuf, uint aCount);

	[DllImport("gluezilla")]
	[return: MarshalAs(UnmanagedType.Interface)]
	public static extern nsIServiceManager gluezilla_getServiceManager2(IntPtr instance);

	[DllImport("gluezilla")]
	private static extern IntPtr gluezilla_evalScript(IntPtr instance, string script);
}
