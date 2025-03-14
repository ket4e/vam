using System;

namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Enum which describes the various types of changes that can occur on a VisualElement.</para>
/// </summary>
[Flags]
public enum ChangeType
{
	/// <summary>
	///   <para>Persistence key or parent has changed on the current VisualElement.</para>
	/// </summary>
	PersistentData = 0x40,
	/// <summary>
	///   <para>Persistence key or parent has changed on some child of the current VisualElement.</para>
	/// </summary>
	PersistentDataPath = 0x20,
	Layout = 0x10,
	Styles = 8,
	Transform = 4,
	StylesPath = 2,
	Repaint = 1,
	/// <summary>
	///   <para>All change types have been flagged.</para>
	/// </summary>
	All = 0x7F
}
