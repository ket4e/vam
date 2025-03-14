using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Drawing;

[SuppressUnmanagedCodeSecurity]
internal static class Carbon
{
	internal static Hashtable contextReference;

	internal static object lockobj;

	internal static Delegate hwnd_delegate;

	static Carbon()
	{
		contextReference = new Hashtable();
		lockobj = new object();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			if (string.Equals(assembly.GetName().Name, "System.Windows.Forms"))
			{
				Type type = assembly.GetType("System.Windows.Forms.XplatUICarbon");
				if (type != null)
				{
					hwnd_delegate = (Delegate)type.GetField("HwndDelegate", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				}
			}
		}
	}

	internal static CarbonContext GetCGContextForView(IntPtr handle)
	{
		IntPtr context = IntPtr.Zero;
		IntPtr zero = IntPtr.Zero;
		IntPtr zero2 = IntPtr.Zero;
		zero2 = GetControlOwner(handle);
		if (handle == IntPtr.Zero || zero2 == IntPtr.Zero)
		{
			zero = GetQDGlobalsThePort();
			CreateCGContextForPort(zero, ref context);
			Rect rect = CGDisplayBounds(CGMainDisplayID());
			return new CarbonContext(zero, context, (int)rect.size.width, (int)rect.size.height);
		}
		QDRect rect2 = default(QDRect);
		Rect r = default(Rect);
		zero = GetWindowPort(zero2);
		context = GetContext(zero);
		GetWindowBounds(zero2, 32u, ref rect2);
		HIViewGetBounds(handle, ref r);
		HIViewConvertRect(ref r, handle, IntPtr.Zero);
		if (r.size.height < 0f)
		{
			r.size.height = 0f;
		}
		if (r.size.width < 0f)
		{
			r.size.width = 0f;
		}
		CGContextTranslateCTM(context, r.origin.x, (float)(rect2.bottom - rect2.top) - (r.origin.y + r.size.height));
		Rect rect3 = new Rect(0f, 0f, r.size.width, r.size.height);
		CGContextSaveGState(context);
		Rectangle[] array = (Rectangle[])hwnd_delegate.DynamicInvoke(handle);
		if (array != null && array.Length > 0)
		{
			int num = array.Length;
			CGContextBeginPath(context);
			CGContextAddRect(context, rect3);
			for (int i = 0; i < num; i++)
			{
				CGContextAddRect(context, new Rect(array[i].X, r.size.height - (float)array[i].Y - (float)array[i].Height, array[i].Width, array[i].Height));
			}
			CGContextClosePath(context);
			CGContextEOClip(context);
		}
		else
		{
			CGContextBeginPath(context);
			CGContextAddRect(context, rect3);
			CGContextClosePath(context);
			CGContextClip(context);
		}
		return new CarbonContext(zero, context, (int)r.size.width, (int)r.size.height);
	}

	internal static IntPtr GetContext(IntPtr port)
	{
		IntPtr context = IntPtr.Zero;
		lock (lockobj)
		{
			CreateCGContextForPort(port, ref context);
			return context;
		}
	}

	internal static void ReleaseContext(IntPtr port, IntPtr context)
	{
		CGContextRestoreGState(context);
		lock (lockobj)
		{
			CFRelease(context);
		}
	}

	[DllImport("libobjc.dylib")]
	public static extern IntPtr objc_getClass(string className);

	[DllImport("libobjc.dylib")]
	public static extern IntPtr objc_msgSend(IntPtr basePtr, IntPtr selector, string argument);

	[DllImport("libobjc.dylib")]
	public static extern IntPtr objc_msgSend(IntPtr basePtr, IntPtr selector);

	[DllImport("libobjc.dylib")]
	public static extern void objc_msgSend_stret(ref Rect arect, IntPtr basePtr, IntPtr selector);

	[DllImport("libobjc.dylib")]
	public static extern IntPtr sel_registerName(string selectorName);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern IntPtr CGMainDisplayID();

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern Rect CGDisplayBounds(IntPtr display);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int HIViewGetBounds(IntPtr vHnd, ref Rect r);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int HIViewConvertRect(ref Rect r, IntPtr a, IntPtr b);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern IntPtr GetControlOwner(IntPtr aView);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int GetWindowBounds(IntPtr wHnd, uint reg, ref QDRect rect);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern IntPtr GetWindowPort(IntPtr hWnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern IntPtr GetQDGlobalsThePort();

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CreateCGContextForPort(IntPtr port, ref IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CFRelease(IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void QDBeginCGContext(IntPtr port, ref IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void QDEndCGContext(IntPtr port, ref IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int CGContextClipToRect(IntPtr context, Rect clip);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int CGContextClipToRects(IntPtr context, Rect[] clip_rects, int count);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextTranslateCTM(IntPtr context, float tx, float ty);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextScaleCTM(IntPtr context, float x, float y);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextFlush(IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextSynchronize(IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern IntPtr CGPathCreateMutable();

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGPathAddRects(IntPtr path, IntPtr _void, Rect[] rects, int count);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGPathAddRect(IntPtr path, IntPtr _void, Rect rect);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextAddRects(IntPtr context, Rect[] rects, int count);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextAddRect(IntPtr context, Rect rect);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextBeginPath(IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextClosePath(IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextAddPath(IntPtr context, IntPtr path);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextClip(IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextEOClip(IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextEOFillPath(IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextSaveGState(IntPtr context);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern void CGContextRestoreGState(IntPtr context);
}
