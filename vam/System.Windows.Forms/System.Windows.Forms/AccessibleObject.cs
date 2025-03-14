using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Accessibility;

namespace System.Windows.Forms;

[ComVisible(true)]
public class AccessibleObject : StandardOleMarshalObject, IReflect, IAccessible
{
	internal string name;

	internal string value;

	internal Control owner;

	internal AccessibleRole role;

	internal AccessibleStates state;

	internal string default_action;

	internal string description;

	internal string help;

	internal string keyboard_shortcut;

	Type IReflect.UnderlyingSystemType
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	int IAccessible.accChildCount
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	object IAccessible.accFocus
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	object IAccessible.accParent
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	object IAccessible.accSelection
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public virtual Rectangle Bounds => owner.Bounds;

	public virtual string DefaultAction => default_action;

	public virtual string Description => description;

	public virtual string Help => help;

	public virtual string KeyboardShortcut => keyboard_shortcut;

	public virtual string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public virtual AccessibleObject Parent
	{
		get
		{
			if (owner != null && owner.Parent != null)
			{
				return owner.Parent.AccessibilityObject;
			}
			return null;
		}
	}

	public virtual AccessibleRole Role => role;

	public virtual AccessibleStates State => state;

	public virtual string Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public AccessibleObject()
	{
		owner = null;
		value = null;
		name = null;
		role = AccessibleRole.Default;
		default_action = null;
		description = null;
		help = null;
		keyboard_shortcut = null;
		state = AccessibleStates.None;
	}

	internal AccessibleObject(Control owner)
		: this()
	{
		this.owner = owner;
	}

	FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
	{
		throw new NotImplementedException();
	}

	FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
	{
		throw new NotImplementedException();
	}

	MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
	{
		throw new NotImplementedException();
	}

	MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
	{
		throw new NotImplementedException();
	}

	MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
	{
		throw new NotImplementedException();
	}

	MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
	{
		throw new NotImplementedException();
	}

	MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
	{
		throw new NotImplementedException();
	}

	PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
	{
		throw new NotImplementedException();
	}

	PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		throw new NotImplementedException();
	}

	PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
	{
		throw new NotImplementedException();
	}

	object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
	{
		throw new NotImplementedException();
	}

	void IAccessible.accDoDefaultAction(object childID)
	{
		throw new NotImplementedException();
	}

	object IAccessible.accHitTest(int xLeft, int yTop)
	{
		throw new NotImplementedException();
	}

	void IAccessible.accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, object childID)
	{
		throw new NotImplementedException();
	}

	object IAccessible.accNavigate(int navDir, object childID)
	{
		throw new NotImplementedException();
	}

	void IAccessible.accSelect(int flagsSelect, object childID)
	{
		throw new NotImplementedException();
	}

	object IAccessible.get_accChild(object childID)
	{
		throw new NotImplementedException();
	}

	string IAccessible.get_accDefaultAction(object childID)
	{
		throw new NotImplementedException();
	}

	string IAccessible.get_accDescription(object childID)
	{
		throw new NotImplementedException();
	}

	string IAccessible.get_accHelp(object childID)
	{
		throw new NotImplementedException();
	}

	int IAccessible.get_accHelpTopic(out string pszHelpFile, object childID)
	{
		throw new NotImplementedException();
	}

	string IAccessible.get_accKeyboardShortcut(object childID)
	{
		throw new NotImplementedException();
	}

	string IAccessible.get_accName(object childID)
	{
		throw new NotImplementedException();
	}

	object IAccessible.get_accRole(object childID)
	{
		throw new NotImplementedException();
	}

	object IAccessible.get_accState(object childID)
	{
		throw new NotImplementedException();
	}

	string IAccessible.get_accValue(object childID)
	{
		throw new NotImplementedException();
	}

	void IAccessible.set_accName(object childID, string newName)
	{
		throw new NotImplementedException();
	}

	void IAccessible.set_accValue(object childID, string newValue)
	{
		throw new NotImplementedException();
	}

	public virtual void DoDefaultAction()
	{
		if (owner != null)
		{
			owner.DoDefaultAction();
		}
	}

	public virtual AccessibleObject GetChild(int index)
	{
		if (owner != null && index < owner.Controls.Count)
		{
			return owner.Controls[index].AccessibilityObject;
		}
		return null;
	}

	public virtual int GetChildCount()
	{
		if (owner != null)
		{
			return owner.Controls.Count;
		}
		return -1;
	}

	public virtual AccessibleObject GetFocused()
	{
		if (owner.has_focus)
		{
			return owner.AccessibilityObject;
		}
		return FindFocusControl(owner);
	}

	public virtual int GetHelpTopic(out string fileName)
	{
		fileName = null;
		return -1;
	}

	public virtual AccessibleObject GetSelected()
	{
		if ((state & AccessibleStates.Selected) != 0)
		{
			return this;
		}
		return FindSelectedControl(owner);
	}

	public virtual AccessibleObject HitTest(int x, int y)
	{
		return FindHittestControl(owner, x, y)?.AccessibilityObject;
	}

	public virtual AccessibleObject Navigate(AccessibleNavigation navdir)
	{
		int num = ((owner.Parent == null) ? (-1) : owner.Parent.Controls.IndexOf(owner));
		switch (navdir)
		{
		case AccessibleNavigation.Up:
			if (owner.Parent != null)
			{
				for (int k = 0; k < owner.Parent.Controls.Count; k++)
				{
					if (owner != owner.Parent.Controls[k] && owner.Parent.Controls[k].Top < owner.Top)
					{
						return owner.Parent.Controls[k].AccessibilityObject;
					}
				}
			}
			return owner.AccessibilityObject;
		case AccessibleNavigation.Down:
			if (owner.Parent != null)
			{
				for (int j = 0; j < owner.Parent.Controls.Count; j++)
				{
					if (owner != owner.Parent.Controls[j] && owner.Parent.Controls[j].Top > owner.Bottom)
					{
						return owner.Parent.Controls[j].AccessibilityObject;
					}
				}
			}
			return owner.AccessibilityObject;
		case AccessibleNavigation.Left:
			if (owner.Parent != null)
			{
				for (int i = 0; i < owner.Parent.Controls.Count; i++)
				{
					if (owner != owner.Parent.Controls[i] && owner.Parent.Controls[i].Left < owner.Left)
					{
						return owner.Parent.Controls[i].AccessibilityObject;
					}
				}
			}
			return owner.AccessibilityObject;
		case AccessibleNavigation.Right:
			if (owner.Parent != null)
			{
				for (int l = 0; l < owner.Parent.Controls.Count; l++)
				{
					if (owner != owner.Parent.Controls[l] && owner.Parent.Controls[l].Left > owner.Right)
					{
						return owner.Parent.Controls[l].AccessibilityObject;
					}
				}
			}
			return owner.AccessibilityObject;
		case AccessibleNavigation.Next:
			if (owner.Parent != null)
			{
				if (num + 1 < owner.Parent.Controls.Count)
				{
					return owner.Parent.Controls[num + 1].AccessibilityObject;
				}
				return owner.Parent.Controls[0].AccessibilityObject;
			}
			return owner.AccessibilityObject;
		case AccessibleNavigation.Previous:
			if (owner.Parent != null)
			{
				if (num > 0)
				{
					return owner.Parent.Controls[num - 1].AccessibilityObject;
				}
				return owner.Parent.Controls[owner.Parent.Controls.Count - 1].AccessibilityObject;
			}
			return owner.AccessibilityObject;
		case AccessibleNavigation.FirstChild:
			if (owner.Controls.Count > 0)
			{
				return owner.Controls[0].AccessibilityObject;
			}
			return owner.AccessibilityObject;
		case AccessibleNavigation.LastChild:
			if (owner.Controls.Count > 0)
			{
				return owner.Controls[owner.Controls.Count - 1].AccessibilityObject;
			}
			return owner.AccessibilityObject;
		default:
			return owner.AccessibilityObject;
		}
	}

	public virtual void Select(AccessibleSelection flags)
	{
		if ((flags & AccessibleSelection.TakeFocus) != 0)
		{
			owner.Focus();
		}
	}

	protected void UseStdAccessibleObjects(IntPtr handle)
	{
	}

	protected void UseStdAccessibleObjects(IntPtr handle, int objid)
	{
		UseStdAccessibleObjects(handle, 0);
	}

	internal static AccessibleObject FindFocusControl(Control parent)
	{
		if (parent != null)
		{
			for (int i = 0; i < parent.Controls.Count; i++)
			{
				Control control = parent.Controls[i];
				if ((control.AccessibilityObject.state & AccessibleStates.Focused) != 0)
				{
					return control.AccessibilityObject;
				}
				if (control.Controls.Count > 0)
				{
					AccessibleObject accessibleObject = FindFocusControl(control);
					if (accessibleObject != null)
					{
						return accessibleObject;
					}
				}
			}
		}
		return null;
	}

	internal static AccessibleObject FindSelectedControl(Control parent)
	{
		if (parent != null)
		{
			for (int i = 0; i < parent.Controls.Count; i++)
			{
				Control control = parent.Controls[i];
				if ((control.AccessibilityObject.state & AccessibleStates.Selected) != 0)
				{
					return control.AccessibilityObject;
				}
				if (control.Controls.Count > 0)
				{
					AccessibleObject accessibleObject = FindSelectedControl(control);
					if (accessibleObject != null)
					{
						return accessibleObject;
					}
				}
			}
		}
		return null;
	}

	internal static Control FindHittestControl(Control parent, int x, int y)
	{
		Point p = new Point(x, y);
		Point pt = parent.PointToClient(p);
		if (parent.ClientRectangle.Contains(pt))
		{
			return parent;
		}
		for (int i = 0; i < parent.Controls.Count; i++)
		{
			Control control = parent.Controls[i];
			pt = control.PointToClient(p);
			if (control.ClientRectangle.Contains(pt))
			{
				return control;
			}
			if (control.Controls.Count > 0)
			{
				Control control2 = FindHittestControl(control, x, y);
				if (control2 != null)
				{
					return control2;
				}
			}
		}
		return null;
	}
}
