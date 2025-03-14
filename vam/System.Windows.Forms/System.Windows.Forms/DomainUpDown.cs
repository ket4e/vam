using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultProperty("Items")]
[DefaultEvent("SelectedItemChanged")]
[ComVisible(true)]
[DefaultBindingProperty("SelectedItem")]
public class DomainUpDown : UpDownBase
{
	[ComVisible(true)]
	public class DomainItemAccessibleObject : AccessibleObject
	{
		private AccessibleObject parent;

		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		public override AccessibleObject Parent => parent;

		public override AccessibleRole Role => base.Role;

		public override AccessibleStates State => base.State;

		public override string Value => base.Value;

		public DomainItemAccessibleObject(string name, AccessibleObject parent)
		{
			base.name = name;
			this.parent = parent;
		}
	}

	[ComVisible(true)]
	public class DomainUpDownAccessibleObject : ControlAccessibleObject
	{
		public override AccessibleRole Role => base.Role;

		public DomainUpDownAccessibleObject(Control owner)
			: base(owner)
		{
		}

		public override AccessibleObject GetChild(int index)
		{
			return base.GetChild(index);
		}

		public override int GetChildCount()
		{
			return base.GetChildCount();
		}
	}

	public class DomainUpDownItemCollection : ArrayList
	{
		private class ToStringSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				return string.Compare(x.ToString(), y.ToString());
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Cannot add null values to a DomainUpDownItemCollection");
				}
				base[index] = value;
				OnCollectionChanged(index, 0);
			}
		}

		internal event CollectionChangedEventHandler CollectionChanged;

		internal DomainUpDownItemCollection()
		{
		}

		public override int Add(object item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("value", "Cannot add null values to a DomainUpDownItemCollection");
			}
			int result = base.Add(item);
			OnCollectionChanged(Count - 1, 1);
			return result;
		}

		public override void Insert(int index, object item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("value", "Cannot add null values to a DomainUpDownItemCollection");
			}
			base.Insert(index, item);
			OnCollectionChanged(index, 1);
		}

		public override void Remove(object item)
		{
			int num = IndexOf(item);
			if (num >= 0)
			{
				RemoveAt(num);
			}
		}

		public override void RemoveAt(int item)
		{
			base.RemoveAt(item);
			OnCollectionChanged(item, -1);
		}

		internal void OnCollectionChanged(int index, int size_delta)
		{
			this.CollectionChanged?.Invoke(index, size_delta);
		}

		internal void PrivSort()
		{
			base.Sort(new ToStringSorter());
		}
	}

	internal delegate void CollectionChangedEventHandler(int index, int size_delta);

	private DomainUpDownItemCollection items;

	private int selected_index = -1;

	private bool sorted;

	private bool wrap;

	private int typed_to_index = -1;

	private static object SelectedItemChangedEvent;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Localizable(true)]
	public DomainUpDownItemCollection Items => items;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new Padding Padding
	{
		get
		{
			return Padding.Empty;
		}
		set
		{
		}
	}

	[Browsable(false)]
	[DefaultValue(-1)]
	public int SelectedIndex
	{
		get
		{
			return selected_index;
		}
		set
		{
			object objA = ((selected_index < 0) ? null : items[selected_index]);
			selected_index = value;
			UpdateEditText();
			object objB = ((selected_index < 0) ? null : items[selected_index]);
			if (!object.ReferenceEquals(objA, objB))
			{
				OnSelectedItemChanged(this, EventArgs.Empty);
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public object SelectedItem
	{
		get
		{
			if (selected_index >= 0)
			{
				return items[selected_index];
			}
			return null;
		}
		set
		{
			SelectedIndex = items.IndexOf(value);
		}
	}

	[DefaultValue(false)]
	public bool Sorted
	{
		get
		{
			return sorted;
		}
		set
		{
			sorted = value;
			if (sorted)
			{
				items.PrivSort();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(false)]
	public bool Wrap
	{
		get
		{
			return wrap;
		}
		set
		{
			wrap = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler PaddingChanged
	{
		add
		{
			base.PaddingChanged += value;
		}
		remove
		{
			base.PaddingChanged -= value;
		}
	}

	public event EventHandler SelectedItemChanged
	{
		add
		{
			base.Events.AddHandler(SelectedItemChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SelectedItemChangedEvent, value);
		}
	}

	public DomainUpDown()
	{
		selected_index = -1;
		sorted = false;
		wrap = false;
		typed_to_index = -1;
		items = new DomainUpDownItemCollection();
		items.CollectionChanged += items_CollectionChanged;
		txtView.LostFocus += TextBoxLostFocus;
		txtView.KeyPress += TextBoxKeyDown;
		UpdateEditText();
	}

	static DomainUpDown()
	{
		SelectedItemChanged = new object();
	}

	internal void items_CollectionChanged(int index, int size_delta)
	{
		bool flag = false;
		if (index == selected_index && size_delta <= 0)
		{
			flag = true;
		}
		else if (index <= selected_index)
		{
			selected_index += size_delta;
		}
		if (sorted && index >= 0)
		{
			items.PrivSort();
		}
		UpdateEditText();
		if (flag)
		{
			OnSelectedItemChanged(this, EventArgs.Empty);
		}
	}

	private void go_to_user_input()
	{
		base.UserEdit = false;
		if (typed_to_index >= 0)
		{
			selected_index = typed_to_index;
			OnSelectedItemChanged(this, EventArgs.Empty);
		}
	}

	private void TextBoxLostFocus(object source, EventArgs e)
	{
		Select(txtView.SelectionStart + txtView.SelectionLength, 0);
	}

	private int SearchTextWithPrefix(char key_char)
	{
		string strA = key_char.ToString();
		int num = ((selected_index != -1) ? selected_index : 0);
		int num2 = ((selected_index != -1 && selected_index + 1 < items.Count) ? (num + 1) : 0);
		do
		{
			string strB = items[num2].ToString();
			if (string.Compare(strA, 0, strB, 0, 1, ignoreCase: true) == 0)
			{
				return num2;
			}
			num2 = ((num2 + 1 < items.Count) ? (num2 + 1) : 0);
		}
		while (num2 != num);
		return -1;
	}

	private bool IsValidInput(char key_char)
	{
		return char.IsLetterOrDigit(key_char) || char.IsNumber(key_char) || char.IsPunctuation(key_char) || char.IsSymbol(key_char) || char.IsWhiteSpace(key_char);
	}

	private void TextBoxKeyDown(object source, KeyPressEventArgs e)
	{
		if (base.ReadOnly)
		{
			char keyChar = e.KeyChar;
			if (IsValidInput(keyChar) && items.Count > 0)
			{
				int num = SearchTextWithPrefix(keyChar);
				if (num > -1)
				{
					SelectedIndex = num;
					e.Handled = true;
				}
			}
			return;
		}
		if (!base.UserEdit)
		{
			txtView.SelectionLength = 0;
			typed_to_index = -1;
		}
		if (txtView.SelectionLength == 0)
		{
			txtView.SelectionStart = 0;
		}
		if (txtView.SelectionStart != 0)
		{
			return;
		}
		if (e.KeyChar == '\b')
		{
			if (txtView.SelectionLength <= 0)
			{
				return;
			}
			string text = txtView.SelectedText.Substring(0, txtView.SelectionLength - 1);
			bool flag = false;
			if (typed_to_index < 0)
			{
				typed_to_index = 0;
			}
			if (sorted)
			{
				for (int num2 = typed_to_index; num2 >= 0; num2--)
				{
					int num3 = string.Compare(text, 0, items[num2].ToString(), 0, text.Length, ignoreCase: true);
					if (num3 == 0)
					{
						flag = true;
						typed_to_index = num2;
					}
					if (num3 > 0)
					{
						break;
					}
				}
			}
			else
			{
				for (int i = 0; i < items.Count; i++)
				{
					if (string.Compare(text, 0, items[i].ToString(), 0, text.Length, ignoreCase: true) == 0)
					{
						flag = true;
						typed_to_index = i;
						break;
					}
				}
			}
			base.ChangingText = true;
			if (flag)
			{
				Text = items[typed_to_index].ToString();
			}
			else
			{
				Text = text;
			}
			Select(0, text.Length);
			base.UserEdit = true;
			e.Handled = true;
			return;
		}
		char keyChar2 = e.KeyChar;
		if (!IsValidInput(keyChar2))
		{
			return;
		}
		string text2 = txtView.SelectedText + keyChar2;
		bool flag2 = false;
		if (typed_to_index < 0)
		{
			typed_to_index = 0;
		}
		if (sorted)
		{
			for (int j = typed_to_index; j < items.Count; j++)
			{
				int num4 = string.Compare(text2, 0, items[j].ToString(), 0, text2.Length, ignoreCase: true);
				if (num4 == 0)
				{
					flag2 = true;
					typed_to_index = j;
				}
				if (num4 <= 0)
				{
					break;
				}
			}
		}
		else
		{
			for (int k = 0; k < items.Count; k++)
			{
				if (string.Compare(text2, 0, items[k].ToString(), 0, text2.Length, ignoreCase: true) == 0)
				{
					flag2 = true;
					typed_to_index = k;
					break;
				}
			}
		}
		base.ChangingText = true;
		if (flag2)
		{
			Text = items[typed_to_index].ToString();
		}
		else
		{
			Text = text2;
		}
		Select(0, text2.Length);
		base.UserEdit = true;
		e.Handled = true;
	}

	public override void DownButton()
	{
		if (base.UserEdit)
		{
			go_to_user_input();
		}
		int num = selected_index + 1;
		if (num >= items.Count)
		{
			if (!wrap)
			{
				return;
			}
			num = 0;
		}
		SelectedIndex = num;
		OnUIADownButtonClick(EventArgs.Empty);
	}

	public override string ToString()
	{
		return base.ToString() + ", Items.Count: " + items.Count + ", SelectedIndex: " + selected_index;
	}

	public override void UpButton()
	{
		if (base.UserEdit)
		{
			go_to_user_input();
		}
		int num = selected_index - 1;
		if (num < 0)
		{
			if (!wrap)
			{
				return;
			}
			num = items.Count - 1;
		}
		SelectedIndex = num;
		OnUIAUpButtonClick(EventArgs.Empty);
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		AccessibleObject accessibleObject = new AccessibleObject(this);
		accessibleObject.role = AccessibleRole.SpinButton;
		return accessibleObject;
	}

	protected override void OnChanged(object source, EventArgs e)
	{
		base.OnChanged(source, e);
	}

	protected void OnSelectedItemChanged(object source, EventArgs e)
	{
		((EventHandler)base.Events[SelectedItemChanged])?.Invoke(this, e);
	}

	protected override void UpdateEditText()
	{
		if (selected_index >= 0 && selected_index < items.Count)
		{
			base.ChangingText = true;
			Text = items[selected_index].ToString();
		}
	}

	protected override void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
	{
		base.OnTextBoxKeyPress(source, e);
	}
}
