using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.CarbonInternal;

internal class Dnd
{
	internal const uint kEventParamDragRef = 1685217639u;

	internal const uint typeDragRef = 1685217639u;

	internal const uint typeMono = 1836019311u;

	internal const uint typeMonoSerializedObject = 1836279154u;

	private static DragDropEffects effects;

	private static DragTrackingDelegate DragTrackingHandler;

	internal Dnd()
	{
	}

	static Dnd()
	{
		effects = DragDropEffects.None;
		DragTrackingHandler = TrackingCallback;
		InstallTrackingHandler(DragTrackingHandler, IntPtr.Zero, IntPtr.Zero);
	}

	internal static void TrackingCallback(short message, IntPtr window, IntPtr data, IntPtr dragref)
	{
		XplatUICarbon.GetInstance().FlushQueue();
	}

	internal static DragDropEffects DragActionsToEffects(uint actions)
	{
		DragDropEffects dragDropEffects = DragDropEffects.None;
		if ((actions & (true ? 1u : 0u)) != 0)
		{
			dragDropEffects |= DragDropEffects.Copy;
		}
		if ((actions & 0x10u) != 0)
		{
			dragDropEffects |= DragDropEffects.Move;
		}
		if ((actions & 0xFFFFFFFFu) != 0)
		{
			dragDropEffects |= DragDropEffects.All;
		}
		return dragDropEffects;
	}

	internal static DataObject DragToDataObject(IntPtr dragref)
	{
		uint count = 0u;
		ArrayList arrayList = new ArrayList();
		CountDragItems(dragref, ref count);
		for (uint num = 1u; num <= count; num++)
		{
			IntPtr itemref = IntPtr.Zero;
			uint count2 = 0u;
			GetDragItemReferenceNumber(dragref, num, ref itemref);
			CountDragItemFlavors(dragref, itemref, ref count2);
			for (uint num2 = 1u; num2 <= count2; num2++)
			{
				FlavorHandler flavorHandler = new FlavorHandler(dragref, itemref, num2);
				if (flavorHandler.Supported)
				{
					arrayList.Add(flavorHandler);
				}
			}
		}
		if (arrayList.Count > 0)
		{
			return ((FlavorHandler)arrayList[0]).Convert(arrayList);
		}
		return new DataObject();
	}

	internal static bool HandleEvent(IntPtr callref, IntPtr eventref, IntPtr handle, uint kind, ref MSG msg)
	{
		QDPoint outData = default(QDPoint);
		uint actions = 0u;
		IntPtr data = IntPtr.Zero;
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd == null || hwnd.Handle != handle)
		{
			return false;
		}
		GetEventParameter(eventref, 1685217639u, 1685217639u, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(IntPtr)), IntPtr.Zero, ref data);
		XplatUICarbon.GetGlobalMouse(ref outData);
		GetDragAllowableActions(data, ref actions);
		Control control = Control.FromHandle(hwnd.Handle);
		DragDropEffects allowedEffect = DragActionsToEffects(actions);
		DataObject data2 = DragToDataObject(data);
		DragEventArgs dragEventArgs = new DragEventArgs(data2, 0, outData.x, outData.y, allowedEffect, DragDropEffects.None);
		switch (kind)
		{
		case 18u:
		{
			bool data3 = control.AllowDrop;
			SetEventParameter(eventref, 1668047975u, 1651470188u, (uint)Marshal.SizeOf(typeof(bool)), ref data3);
			control.DndEnter(dragEventArgs);
			effects = dragEventArgs.Effect;
			return false;
		}
		case 19u:
			control.DndOver(dragEventArgs);
			effects = dragEventArgs.Effect;
			break;
		case 20u:
			control.DndLeave(dragEventArgs);
			break;
		case 21u:
			control.DndDrop(dragEventArgs);
			break;
		}
		return true;
	}

	public void SetAllowDrop(Hwnd hwnd, bool allow)
	{
		if (hwnd.allow_drop != allow)
		{
			hwnd.allow_drop = allow;
			SetControlDragTrackingEnabled(hwnd.whole_window, enabled: true);
			SetControlDragTrackingEnabled(hwnd.client_window, enabled: true);
		}
	}

	public void SendDrop(IntPtr handle, IntPtr from, IntPtr time)
	{
	}

	public DragDropEffects StartDrag(IntPtr handle, object data, DragDropEffects allowed_effects)
	{
		IntPtr dragref = IntPtr.Zero;
		EventRecord eventrecord = default(EventRecord);
		effects = DragDropEffects.None;
		NewDrag(ref dragref);
		XplatUICarbon.GetGlobalMouse(ref eventrecord.mouse);
		StoreObjectInDrag(handle, dragref, data);
		TrackDrag(dragref, ref eventrecord, IntPtr.Zero);
		DisposeDrag(dragref);
		return effects;
	}

	public void StoreObjectInDrag(IntPtr handle, IntPtr dragref, object data)
	{
		IntPtr zero = IntPtr.Zero;
		IntPtr zero2 = IntPtr.Zero;
		int num = 0;
		if (data is string)
		{
			throw new NotSupportedException("Implement me.");
		}
		if (data is ISerializable)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, data);
			zero2 = Marshal.AllocHGlobal((int)memoryStream.Length);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			for (int i = 0; i < memoryStream.Length; i++)
			{
				Marshal.WriteByte(zero2, i, (byte)memoryStream.ReadByte());
			}
			zero = (IntPtr)1836279154L;
			num = (int)memoryStream.Length;
		}
		else
		{
			zero2 = (IntPtr)GCHandle.Alloc(data);
			zero = (IntPtr)1836019311L;
			num = Marshal.SizeOf(typeof(IntPtr));
		}
		AddDragItemFlavor(dragref, handle, zero, ref zero2, num, 1u);
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int InstallTrackingHandler(DragTrackingDelegate callback, IntPtr window, IntPtr data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetEventParameter(IntPtr eventref, uint name, uint type, IntPtr outtype, uint size, IntPtr outsize, ref IntPtr data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int SetEventParameter(IntPtr eventref, uint name, uint type, uint size, ref bool data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int SetControlDragTrackingEnabled(IntPtr view, bool enabled);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int AddDragItemFlavor(IntPtr dragref, IntPtr itemref, IntPtr flavortype, ref IntPtr data, int size, uint flags);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int CountDragItems(IntPtr dragref, ref uint count);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int CountDragItemFlavors(IntPtr dragref, IntPtr itemref, ref uint count);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetDragItemReferenceNumber(IntPtr dragref, uint index, ref IntPtr itemref);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int NewDrag(ref IntPtr dragref);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int TrackDrag(IntPtr dragref, ref EventRecord eventrecord, IntPtr region);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int DisposeDrag(IntPtr dragref);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetDragAllowableActions(IntPtr dragref, ref uint actions);
}
