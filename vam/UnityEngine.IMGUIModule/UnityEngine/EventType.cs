using System;

namespace UnityEngine;

/// <summary>
///   <para>Types of UnityGUI input and processing events.</para>
/// </summary>
public enum EventType
{
	/// <summary>
	///   <para>Mouse button was pressed.</para>
	/// </summary>
	MouseDown = 0,
	/// <summary>
	///   <para>Mouse button was released.</para>
	/// </summary>
	MouseUp = 1,
	/// <summary>
	///   <para>Mouse was moved (Editor views only).</para>
	/// </summary>
	MouseMove = 2,
	/// <summary>
	///   <para>Mouse was dragged.</para>
	/// </summary>
	MouseDrag = 3,
	/// <summary>
	///   <para>A keyboard key was pressed.</para>
	/// </summary>
	KeyDown = 4,
	/// <summary>
	///   <para>A keyboard key was released.</para>
	/// </summary>
	KeyUp = 5,
	/// <summary>
	///   <para>The scroll wheel was moved.</para>
	/// </summary>
	ScrollWheel = 6,
	/// <summary>
	///   <para>A repaint event. One is sent every frame.</para>
	/// </summary>
	Repaint = 7,
	/// <summary>
	///   <para>A layout event.</para>
	/// </summary>
	Layout = 8,
	/// <summary>
	///   <para>Editor only: drag &amp; drop operation updated.</para>
	/// </summary>
	DragUpdated = 9,
	/// <summary>
	///   <para>Editor only: drag &amp; drop operation performed.</para>
	/// </summary>
	DragPerform = 10,
	/// <summary>
	///   <para>Editor only: drag &amp; drop operation exited.</para>
	/// </summary>
	DragExited = 15,
	/// <summary>
	///   <para>Event should be ignored.</para>
	/// </summary>
	Ignore = 11,
	/// <summary>
	///   <para>Already processed event.</para>
	/// </summary>
	Used = 12,
	/// <summary>
	///   <para>Validates a special command (e.g. copy &amp; paste).</para>
	/// </summary>
	ValidateCommand = 13,
	/// <summary>
	///   <para>Execute a special command (eg. copy &amp; paste).</para>
	/// </summary>
	ExecuteCommand = 14,
	/// <summary>
	///   <para>User has right-clicked (or control-clicked on the mac).</para>
	/// </summary>
	ContextClick = 16,
	/// <summary>
	///   <para>Mouse entered a window (Editor views only).</para>
	/// </summary>
	MouseEnterWindow = 20,
	/// <summary>
	///   <para>Mouse left a window (Editor views only).</para>
	/// </summary>
	MouseLeaveWindow = 21,
	/// <summary>
	///   <para>An event that is called when the mouse is clicked.</para>
	/// </summary>
	[Obsolete("Use MouseDown instead (UnityUpgradable) -> MouseDown", true)]
	mouseDown = 0,
	/// <summary>
	///   <para>An event that is called when the mouse is no longer being clicked.</para>
	/// </summary>
	[Obsolete("Use MouseUp instead (UnityUpgradable) -> MouseUp", true)]
	mouseUp = 1,
	[Obsolete("Use MouseMove instead (UnityUpgradable) -> MouseMove", true)]
	mouseMove = 2,
	/// <summary>
	///   <para>An event that is called when the mouse is clicked and dragged.</para>
	/// </summary>
	[Obsolete("Use MouseDrag instead (UnityUpgradable) -> MouseDrag", true)]
	mouseDrag = 3,
	[Obsolete("Use KeyDown instead (UnityUpgradable) -> KeyDown", true)]
	keyDown = 4,
	[Obsolete("Use KeyUp instead (UnityUpgradable) -> KeyUp", true)]
	keyUp = 5,
	[Obsolete("Use ScrollWheel instead (UnityUpgradable) -> ScrollWheel", true)]
	scrollWheel = 6,
	[Obsolete("Use Repaint instead (UnityUpgradable) -> Repaint", true)]
	repaint = 7,
	[Obsolete("Use Layout instead (UnityUpgradable) -> Layout", true)]
	layout = 8,
	[Obsolete("Use DragUpdated instead (UnityUpgradable) -> DragUpdated", true)]
	dragUpdated = 9,
	[Obsolete("Use DragPerform instead (UnityUpgradable) -> DragPerform", true)]
	dragPerform = 10,
	[Obsolete("Use Ignore instead (UnityUpgradable) -> Ignore", true)]
	ignore = 11,
	[Obsolete("Use Used instead (UnityUpgradable) -> Used", true)]
	used = 12
}
