namespace System.Windows.Forms.CarbonInternal;

internal enum MouseTrackingResult : ushort
{
	kMouseTrackingMouseDown = 1,
	kMouseTrackingMouseUp,
	kMouseTrackingMouseExited,
	kMouseTrackingMouseEntered,
	kMouseTrackingMouseDragged,
	kMouseTrackingKeyModifiersChanged,
	kMouseTrackingUserCancelled,
	kMouseTrackingTimedOut,
	kMouseTrackingMouseMoved
}
