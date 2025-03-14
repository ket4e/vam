using System.Runtime.InteropServices;

namespace System.Windows.Forms.CarbonInternal;

internal class Pasteboard
{
	private static IntPtr primary_pbref;

	private static IntPtr app_pbref;

	private static IntPtr internal_format;

	internal static IntPtr Primary => primary_pbref;

	internal static IntPtr Application => app_pbref;

	static Pasteboard()
	{
		PasteboardCreate(XplatUICarbon.__CFStringMakeConstantString("com.apple.pasteboard.clipboard"), ref primary_pbref);
		PasteboardCreate(IntPtr.Zero, ref app_pbref);
		internal_format = XplatUICarbon.__CFStringMakeConstantString("com.novell.mono.mwf.pasteboard");
	}

	internal static object Retrieve(IntPtr pbref, int key)
	{
		uint count = 0u;
		key = (int)internal_format;
		PasteboardGetItemCount(pbref, ref count);
		for (int i = 1; i <= count; i++)
		{
			uint itemid = 0u;
			PasteboardGetItemIdentifier(pbref, (uint)i, ref itemid);
			if (itemid == 64206)
			{
				IntPtr data = IntPtr.Zero;
				PasteboardCopyItemFlavorData(pbref, 64206u, (uint)key, ref data);
				if (data != IntPtr.Zero)
				{
					return ((GCHandle)Marshal.ReadIntPtr(CFDataGetBytePtr(data))).Target;
				}
			}
		}
		return null;
	}

	internal static void Store(IntPtr pbref, object data, int key)
	{
		IntPtr buf = (IntPtr)GCHandle.Alloc(data);
		IntPtr data2 = CFDataCreate(IntPtr.Zero, ref buf, Marshal.SizeOf(typeof(IntPtr)));
		key = (int)internal_format;
		PasteboardClear(pbref);
		PasteboardPutItemFlavor(pbref, 64206u, (uint)key, data2, 0u);
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr CFDataCreate(IntPtr allocator, ref IntPtr buf, int length);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr CFDataGetBytePtr(IntPtr data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int PasteboardClear(IntPtr pbref);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int PasteboardCreate(IntPtr str, ref IntPtr pbref);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int PasteboardCopyItemFlavorData(IntPtr pbref, uint itemid, uint key, ref IntPtr data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int PasteboardGetItemCount(IntPtr pbref, ref uint count);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int PasteboardGetItemIdentifier(IntPtr pbref, uint itemindex, ref uint itemid);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int PasteboardPutItemFlavor(IntPtr pbref, uint itemid, uint key, IntPtr data, uint flags);
}
