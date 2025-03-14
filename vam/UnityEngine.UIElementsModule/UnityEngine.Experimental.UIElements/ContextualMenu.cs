using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>A contextual menu.</para>
/// </summary>
public class ContextualMenu
{
	/// <summary>
	///   <para>An item in a contextual menu.</para>
	/// </summary>
	public abstract class MenuItem
	{
	}

	/// <summary>
	///   <para>A separator menu item.</para>
	/// </summary>
	public class Separator : MenuItem
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public Separator()
		{
		}
	}

	/// <summary>
	///   <para>A menu action item.</para>
	/// </summary>
	public class MenuAction : MenuItem
	{
		/// <summary>
		///   <para>Status of the menu item.</para>
		/// </summary>
		[Flags]
		public enum StatusFlags
		{
			/// <summary>
			///   <para>The item is displayed normally.</para>
			/// </summary>
			Normal = 0,
			/// <summary>
			///   <para>The item is disabled and is not be selectable by the user.</para>
			/// </summary>
			Disabled = 1,
			/// <summary>
			///   <para>The item is displayed with a checkmark.</para>
			/// </summary>
			Checked = 2,
			/// <summary>
			///   <para>The item is not displayed.</para>
			/// </summary>
			Hidden = 4
		}

		/// <summary>
		///   <para>The name of the item.</para>
		/// </summary>
		public string name;

		private Action<EventBase> actionCallback;

		private Func<EventBase, StatusFlags> actionStatusCallback;

		/// <summary>
		///   <para>The status of the item.</para>
		/// </summary>
		public StatusFlags status { get; private set; }

		public MenuAction(string actionName, Action<EventBase> actionCallback, Func<EventBase, StatusFlags> actionStatusCallback)
		{
			name = actionName;
			this.actionCallback = actionCallback;
			this.actionStatusCallback = actionStatusCallback;
		}

		/// <summary>
		///   <para>Status callback that always returns StatusFlags.Enabled.</para>
		/// </summary>
		/// <param name="e">The event that triggered the display of the context menu.</param>
		/// <returns>
		///   <para>Always return StatusFlags.Enabled.</para>
		/// </returns>
		public static StatusFlags AlwaysEnabled(EventBase e)
		{
			return StatusFlags.Normal;
		}

		/// <summary>
		///   <para>Status callback that always returns StatusFlags.Disabled.</para>
		/// </summary>
		/// <param name="e">The event that triggered the display of the context menu.</param>
		/// <returns>
		///   <para>Always return StatusFlags.Disabled.</para>
		/// </returns>
		public static StatusFlags AlwaysDisabled(EventBase e)
		{
			return StatusFlags.Disabled;
		}

		/// <summary>
		///   <para>Update the status flag of this item by calling the item status callback.</para>
		/// </summary>
		/// <param name="e">The event that triggered the display of the context menu.</param>
		public void UpdateActionStatus(EventBase e)
		{
			status = ((actionStatusCallback == null) ? StatusFlags.Hidden : actionStatusCallback(e));
		}

		/// <summary>
		///   <para>Execute the callback associated with this item.</para>
		/// </summary>
		/// <param name="e">The event that triggered the display of the context menu.</param>
		public void Execute(EventBase e)
		{
			if (actionCallback != null)
			{
				actionCallback(e);
			}
		}
	}

	private List<MenuItem> menuItems = new List<MenuItem>();

	/// <summary>
	///   <para>Constructor.</para>
	/// </summary>
	public ContextualMenu()
	{
	}

	/// <summary>
	///   <para>Get the list of menu items.</para>
	/// </summary>
	/// <returns>
	///   <para>The list of items in the menu.</para>
	/// </returns>
	public List<MenuItem> MenuItems()
	{
		return menuItems;
	}

	public void AppendAction(string actionName, Action<EventBase> action, Func<EventBase, MenuAction.StatusFlags> actionStatusCallback)
	{
		MenuAction item = new MenuAction(actionName, action, actionStatusCallback);
		menuItems.Add(item);
	}

	public void InsertAction(string actionName, Action<EventBase> action, Func<EventBase, MenuAction.StatusFlags> actionStatusCallback, int atIndex)
	{
		MenuAction item = new MenuAction(actionName, action, actionStatusCallback);
		menuItems.Insert(atIndex, item);
	}

	/// <summary>
	///   <para>Add a separator line in the menu. The separator is added at the end of the current item list.</para>
	/// </summary>
	public void AppendSeparator()
	{
		if (menuItems.Count > 0 && !(menuItems[menuItems.Count - 1] is Separator))
		{
			Separator item = new Separator();
			menuItems.Add(item);
		}
	}

	/// <summary>
	///   <para>Add a separator line in the menu. The separator is added at the end of the specified index in the list.</para>
	/// </summary>
	/// <param name="atIndex">Index where the separator should be inserted.</param>
	public void InsertSeparator(int atIndex)
	{
		if (atIndex > 0 && atIndex <= menuItems.Count && !(menuItems[atIndex - 1] is Separator))
		{
			Separator item = new Separator();
			menuItems.Insert(atIndex, item);
		}
	}

	/// <summary>
	///   <para>Update the status of all items by calling their status callback and remove the separators in excess. This is called just before displaying the menu.</para>
	/// </summary>
	/// <param name="e"></param>
	public void PrepareForDisplay(EventBase e)
	{
		foreach (MenuItem menuItem in menuItems)
		{
			if (menuItem is MenuAction menuAction)
			{
				menuAction.UpdateActionStatus(e);
			}
		}
		if (menuItems[menuItems.Count - 1] is Separator)
		{
			menuItems.RemoveAt(menuItems.Count - 1);
		}
	}
}
