#define UNITY_ASSERTIONS
namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Class that manages capturing mouse events.</para>
/// </summary>
public static class MouseCaptureController
{
	internal static IEventHandler mouseCapture { get; private set; }

	/// <summary>
	///   <para>Checks if there is a handler assigned to capturing the mouse.</para>
	/// </summary>
	/// <returns>
	///   <para>Returns true if a handler is assigned to capture the mouse, false otherwise.</para>
	/// </returns>
	public static bool IsMouseCaptureTaken()
	{
		return mouseCapture != null;
	}

	/// <summary>
	///   <para>Checks if the event handler is capturing the mouse.</para>
	/// </summary>
	/// <param name="handler">Event handler to check.</param>
	/// <returns>
	///   <para>True if the handler captures the mouse.</para>
	/// </returns>
	public static bool HasMouseCapture(this IEventHandler handler)
	{
		return mouseCapture == handler;
	}

	/// <summary>
	///   <para>Assigns an event handler to capture the mouse.</para>
	/// </summary>
	/// <param name="handler">The event handler to capture the mouse.</param>
	public static void TakeMouseCapture(this IEventHandler handler)
	{
		if (mouseCapture == handler)
		{
			return;
		}
		if (GUIUtility.hotControl != 0)
		{
			Debug.Log("Should not be capturing when there is a hotcontrol");
			return;
		}
		ReleaseMouseCapture();
		mouseCapture = handler;
		using MouseCaptureEvent evt = MouseCaptureEventBase<MouseCaptureEvent>.GetPooled(mouseCapture);
		UIElementsUtility.eventDispatcher.DispatchEvent(evt, null);
	}

	/// <summary>
	///   <para>Stops an event handler from capturing the mouse.</para>
	/// </summary>
	/// <param name="handler">The event handler to stop capturing the mouse. If this handler is not assigned to capturing the mouse, nothing happens.</param>
	public static void ReleaseMouseCapture(this IEventHandler handler)
	{
		Debug.Assert(handler == mouseCapture, "Element releasing capture does not have capture");
		if (handler == mouseCapture)
		{
			ReleaseMouseCapture();
		}
	}

	/// <summary>
	///   <para>Stops an event handler from capturing the mouse.</para>
	/// </summary>
	/// <param name="handler">The event handler to stop capturing the mouse. If this handler is not assigned to capturing the mouse, nothing happens.</param>
	public static void ReleaseMouseCapture()
	{
		if (mouseCapture != null)
		{
			using MouseCaptureOutEvent evt = MouseCaptureEventBase<MouseCaptureOutEvent>.GetPooled(mouseCapture);
			UIElementsUtility.eventDispatcher.DispatchEvent(evt, null);
		}
		mouseCapture = null;
	}
}
