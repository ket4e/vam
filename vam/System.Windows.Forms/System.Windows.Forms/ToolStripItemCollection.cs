using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[ListBindable(false)]
[Editor("System.Windows.Forms.Design.ToolStripCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
public class ToolStripItemCollection : ArrangedElementCollection, ICollection, IEnumerable, IList
{
	private ToolStrip owner;

	private bool internal_created;

	bool IList.IsFixedSize => base.IsFixedSize;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public override bool IsReadOnly => base.IsReadOnly;

	public new virtual ToolStripItem this[int index] => (ToolStripItem)base[index];

	public virtual ToolStripItem this[string key]
	{
		get
		{
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ToolStripItem toolStripItem = (ToolStripItem)enumerator.Current;
					if (toolStripItem.Name == key)
					{
						return toolStripItem;
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return null;
		}
	}

	public ToolStripItemCollection(ToolStrip owner, ToolStripItem[] value)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (value == null)
		{
			throw new ArgumentNullException("toolStripItems");
		}
		this.owner = owner;
		foreach (ToolStripItem value2 in value)
		{
			AddNoOwnerOrLayout(value2);
		}
	}

	internal ToolStripItemCollection(ToolStrip owner, ToolStripItem[] value, bool internalcreated)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		internal_created = internalcreated;
		this.owner = owner;
		if (value != null)
		{
			foreach (ToolStripItem value2 in value)
			{
				AddNoOwnerOrLayout(value2);
			}
		}
	}

	int IList.Add(object value)
	{
		return Add((ToolStripItem)value);
	}

	void IList.Clear()
	{
		Clear();
	}

	bool IList.Contains(object value)
	{
		return Contains((ToolStripItem)value);
	}

	int IList.IndexOf(object value)
	{
		return IndexOf((ToolStripItem)value);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, (ToolStripItem)value);
	}

	void IList.Remove(object value)
	{
		Remove((ToolStripItem)value);
	}

	void IList.RemoveAt(int index)
	{
		RemoveAt(index);
	}

	public ToolStripItem Add(Image image)
	{
		ToolStripItem toolStripItem = owner.CreateDefaultItem(string.Empty, image, null);
		Add(toolStripItem);
		return toolStripItem;
	}

	public ToolStripItem Add(string text)
	{
		ToolStripItem toolStripItem = owner.CreateDefaultItem(text, null, null);
		Add(toolStripItem);
		return toolStripItem;
	}

	public int Add(ToolStripItem value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		value.InternalOwner = owner;
		if (value is ToolStripMenuItem && (value as ToolStripMenuItem).ShortcutKeys != 0)
		{
			ToolStripManager.AddToolStripMenuItem((ToolStripMenuItem)value);
		}
		int result = Add((object)value);
		if (internal_created)
		{
			owner.OnItemAdded(new ToolStripItemEventArgs(value));
		}
		return result;
	}

	public ToolStripItem Add(string text, Image image)
	{
		ToolStripItem toolStripItem = owner.CreateDefaultItem(text, image, null);
		Add(toolStripItem);
		return toolStripItem;
	}

	public ToolStripItem Add(string text, Image image, EventHandler onClick)
	{
		ToolStripItem toolStripItem = owner.CreateDefaultItem(text, image, onClick);
		Add(toolStripItem);
		return toolStripItem;
	}

	public void AddRange(ToolStripItem[] toolStripItems)
	{
		if (toolStripItems == null)
		{
			throw new ArgumentNullException("toolStripItems");
		}
		if (IsReadOnly)
		{
			throw new NotSupportedException("This collection is read-only");
		}
		owner.SuspendLayout();
		foreach (ToolStripItem value in toolStripItems)
		{
			Add(value);
		}
		owner.ResumeLayout();
	}

	public void AddRange(ToolStripItemCollection toolStripItems)
	{
		if (toolStripItems == null)
		{
			throw new ArgumentNullException("toolStripItems");
		}
		if (IsReadOnly)
		{
			throw new NotSupportedException("This collection is read-only");
		}
		owner.SuspendLayout();
		foreach (ToolStripItem toolStripItem in toolStripItems)
		{
			Add(toolStripItem);
		}
		owner.ResumeLayout();
	}

	public new virtual void Clear()
	{
		if (IsReadOnly)
		{
			throw new NotSupportedException("This collection is read-only");
		}
		base.Clear();
		owner.PerformLayout();
	}

	public bool Contains(ToolStripItem value)
	{
		return Contains((object)value);
	}

	public virtual bool ContainsKey(string key)
	{
		return this[key] != null;
	}

	public void CopyTo(ToolStripItem[] array, int index)
	{
		CopyTo((Array)array, index);
	}

	[System.MonoTODO("searchAllChildren parameter isn't used")]
	public ToolStripItem[] Find(string key, bool searchAllChildren)
	{
		if (key == null || key.Length == 0)
		{
			throw new ArgumentNullException("key");
		}
		List<ToolStripItem> list = new List<ToolStripItem>();
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ToolStripItem toolStripItem = (ToolStripItem)enumerator.Current;
				if (string.Compare(toolStripItem.Name, key, ignoreCase: true) == 0)
				{
					list.Add(toolStripItem);
					if (!searchAllChildren)
					{
					}
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return list.ToArray();
	}

	public int IndexOf(ToolStripItem value)
	{
		return IndexOf((object)value);
	}

	public virtual int IndexOfKey(string key)
	{
		ToolStripItem toolStripItem = this[key];
		if (toolStripItem == null)
		{
			return -1;
		}
		return IndexOf(toolStripItem);
	}

	public void Insert(int index, ToolStripItem value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value is ToolStripMenuItem && (value as ToolStripMenuItem).ShortcutKeys != 0)
		{
			ToolStripManager.AddToolStripMenuItem((ToolStripMenuItem)value);
		}
		if (value.Owner != null)
		{
			value.Owner.Items.Remove(value);
		}
		Insert(index, (object)value);
		if (internal_created)
		{
			value.InternalOwner = owner;
			owner.OnItemAdded(new ToolStripItemEventArgs(value));
		}
		if (owner.Created)
		{
			owner.PerformLayout();
		}
	}

	public void Remove(ToolStripItem value)
	{
		if (IsReadOnly)
		{
			throw new NotSupportedException("This collection is read-only");
		}
		Remove((object)value);
		if (value != null && internal_created)
		{
			value.InternalOwner = null;
			value.Parent = null;
		}
		if (internal_created)
		{
			owner.OnItemRemoved(new ToolStripItemEventArgs(value));
		}
		if (owner.Created)
		{
			owner.PerformLayout();
		}
	}

	public void RemoveAt(int index)
	{
		if (IsReadOnly)
		{
			throw new NotSupportedException("This collection is read-only");
		}
		ToolStripItem value = (ToolStripItem)base[index];
		Remove(value);
	}

	public virtual void RemoveByKey(string key)
	{
		if (IsReadOnly)
		{
			throw new NotSupportedException("This collection is read-only");
		}
		ToolStripItem toolStripItem = this[key];
		if (toolStripItem != null)
		{
			Remove(toolStripItem);
		}
	}

	internal int AddNoOwnerOrLayout(ToolStripItem value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		return Add((object)value);
	}

	internal void InsertNoOwnerOrLayout(int index, ToolStripItem value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (index > Count)
		{
			Add((object)value);
		}
		else
		{
			Insert(index, (object)value);
		}
	}

	internal void RemoveNoOwnerOrLayout(ToolStripItem value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		Remove((object)value);
	}
}
