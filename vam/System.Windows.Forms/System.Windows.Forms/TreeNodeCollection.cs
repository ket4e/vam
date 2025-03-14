using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;

namespace System.Windows.Forms;

[Editor("System.Windows.Forms.Design.TreeNodeCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
public class TreeNodeCollection : ICollection, IEnumerable, IList
{
	internal class TreeNodeEnumerator : IEnumerator
	{
		private TreeNodeCollection collection;

		private int index = -1;

		public object Current
		{
			get
			{
				if (index == -1)
				{
					return null;
				}
				return collection[index];
			}
		}

		public TreeNodeEnumerator(TreeNodeCollection collection)
		{
			this.collection = collection;
		}

		public bool MoveNext()
		{
			if (index + 1 >= collection.Count)
			{
				return false;
			}
			index++;
			return true;
		}

		public void Reset()
		{
			index = -1;
		}
	}

	private class TreeNodeComparer : IComparer
	{
		private CompareInfo compare;

		public TreeNodeComparer(CompareInfo compare)
		{
			this.compare = compare;
		}

		public int Compare(object x, object y)
		{
			TreeNode treeNode = (TreeNode)x;
			TreeNode treeNode2 = (TreeNode)y;
			int num = compare.Compare(treeNode.Text, treeNode2.Text);
			return (num != 0) ? num : (treeNode.Index - treeNode2.Index);
		}
	}

	private static readonly int OrigSize = 50;

	private TreeNode owner;

	private int count;

	private TreeNode[] nodes;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	bool IList.IsFixedSize => false;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			if (!(value is TreeNode))
			{
				throw new ArgumentException("Parameter must be of type TreeNode.", "value");
			}
			this[index] = (TreeNode)value;
		}
	}

	[Browsable(false)]
	public int Count => count;

	public bool IsReadOnly => false;

	public virtual TreeNode this[int index]
	{
		get
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return nodes[index];
		}
		set
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			SetupNode(value);
			nodes[index] = value;
		}
	}

	public virtual TreeNode this[string key]
	{
		get
		{
			for (int i = 0; i < count; i++)
			{
				if (string.Compare(key, nodes[i].Name, ignoreCase: true) == 0)
				{
					return nodes[i];
				}
			}
			return null;
		}
	}

	private TreeNodeCollection()
	{
	}

	internal TreeNodeCollection(TreeNode owner)
	{
		this.owner = owner;
		nodes = new TreeNode[OrigSize];
	}

	int IList.Add(object node)
	{
		return Add((TreeNode)node);
	}

	bool IList.Contains(object node)
	{
		return Contains((TreeNode)node);
	}

	int IList.IndexOf(object node)
	{
		return IndexOf((TreeNode)node);
	}

	void IList.Insert(int index, object node)
	{
		Insert(index, (TreeNode)node);
	}

	void IList.Remove(object node)
	{
		Remove((TreeNode)node);
	}

	public virtual TreeNode Add(string text)
	{
		TreeNode treeNode = new TreeNode(text);
		Add(treeNode);
		return treeNode;
	}

	public virtual int Add(TreeNode node)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		TreeView treeView = null;
		if (owner != null)
		{
			treeView = owner.TreeView;
		}
		int result;
		if (treeView != null && treeView.Sorted)
		{
			result = AddSorted(node);
		}
		else
		{
			if (count >= nodes.Length)
			{
				Grow();
			}
			nodes[count] = node;
			result = count;
			count++;
		}
		SetupNode(node);
		treeView?.OnUIACollectionChanged(owner, new CollectionChangeEventArgs(CollectionChangeAction.Add, node));
		return result;
	}

	public virtual TreeNode Add(string key, string text)
	{
		TreeNode treeNode = new TreeNode(text);
		treeNode.Name = key;
		Add(treeNode);
		return treeNode;
	}

	public virtual TreeNode Add(string key, string text, int imageIndex)
	{
		TreeNode treeNode = Add(key, text);
		treeNode.ImageIndex = imageIndex;
		return treeNode;
	}

	public virtual TreeNode Add(string key, string text, string imageKey)
	{
		TreeNode treeNode = Add(key, text);
		treeNode.ImageKey = imageKey;
		return treeNode;
	}

	public virtual TreeNode Add(string key, string text, int imageIndex, int selectedImageIndex)
	{
		TreeNode treeNode = Add(key, text);
		treeNode.ImageIndex = imageIndex;
		treeNode.SelectedImageIndex = selectedImageIndex;
		return treeNode;
	}

	public virtual TreeNode Add(string key, string text, string imageKey, string selectedImageKey)
	{
		TreeNode treeNode = Add(key, text);
		treeNode.ImageKey = imageKey;
		treeNode.SelectedImageKey = selectedImageKey;
		return treeNode;
	}

	public virtual void AddRange(TreeNode[] nodes)
	{
		if (nodes == null)
		{
			throw new ArgumentNullException("nodes");
		}
		for (int i = 0; i < nodes.Length; i++)
		{
			Add(nodes[i]);
		}
	}

	public virtual void Clear()
	{
		while (count > 0)
		{
			RemoveAt(0, update: false);
		}
		Array.Clear(nodes, 0, count);
		count = 0;
		TreeView treeView = null;
		if (owner != null)
		{
			treeView = owner.TreeView;
			if (treeView != null)
			{
				treeView.UpdateBelow(owner);
				treeView.RecalculateVisibleOrder(owner);
				treeView.UpdateScrollBars(force: false);
			}
		}
	}

	public bool Contains(TreeNode node)
	{
		return Array.IndexOf(nodes, node, 0, count) != -1;
	}

	public virtual bool ContainsKey(string key)
	{
		for (int i = 0; i < count; i++)
		{
			if (string.Compare(nodes[i].Name, key, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
			{
				return true;
			}
		}
		return false;
	}

	public void CopyTo(Array dest, int index)
	{
		Array.Copy(nodes, index, dest, index, count);
	}

	public IEnumerator GetEnumerator()
	{
		return new TreeNodeEnumerator(this);
	}

	public int IndexOf(TreeNode node)
	{
		return Array.IndexOf(nodes, node);
	}

	public virtual int IndexOfKey(string key)
	{
		for (int i = 0; i < count; i++)
		{
			if (string.Compare(nodes[i].Name, key, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public virtual TreeNode Insert(int index, string text)
	{
		TreeNode treeNode = new TreeNode(text);
		Insert(index, treeNode);
		return treeNode;
	}

	public virtual void Insert(int index, TreeNode node)
	{
		if (count >= nodes.Length)
		{
			Grow();
		}
		Array.Copy(nodes, index, nodes, index + 1, count - index);
		nodes[index] = node;
		count++;
		SetupNode(node);
	}

	public virtual TreeNode Insert(int index, string key, string text)
	{
		TreeNode treeNode = new TreeNode(text);
		treeNode.Name = key;
		Insert(index, treeNode);
		return treeNode;
	}

	public virtual TreeNode Insert(int index, string key, string text, int imageIndex)
	{
		TreeNode treeNode = new TreeNode(text);
		treeNode.Name = key;
		treeNode.ImageIndex = imageIndex;
		Insert(index, treeNode);
		return treeNode;
	}

	public virtual TreeNode Insert(int index, string key, string text, string imageKey)
	{
		TreeNode treeNode = new TreeNode(text);
		treeNode.Name = key;
		treeNode.ImageKey = imageKey;
		Insert(index, treeNode);
		return treeNode;
	}

	public virtual TreeNode Insert(int index, string key, string text, int imageIndex, int selectedImageIndex)
	{
		TreeNode treeNode = new TreeNode(text, imageIndex, selectedImageIndex);
		treeNode.Name = key;
		Insert(index, treeNode);
		return treeNode;
	}

	public virtual TreeNode Insert(int index, string key, string text, string imageKey, string selectedImageKey)
	{
		TreeNode treeNode = new TreeNode(text);
		treeNode.Name = key;
		treeNode.ImageKey = imageKey;
		treeNode.SelectedImageKey = selectedImageKey;
		Insert(index, treeNode);
		return treeNode;
	}

	public void Remove(TreeNode node)
	{
		if (node == null)
		{
			throw new NullReferenceException();
		}
		int num = IndexOf(node);
		if (num != -1)
		{
			RemoveAt(num);
		}
	}

	public virtual void RemoveAt(int index)
	{
		RemoveAt(index, update: true);
	}

	private void RemoveAt(int index, bool update)
	{
		TreeNode treeNode = nodes[index];
		TreeNode prevNode = GetPrevNode(treeNode);
		TreeNode selectedNode = null;
		bool flag = false;
		bool isVisible = treeNode.IsVisible;
		TreeView treeView = null;
		if (owner != null)
		{
			treeView = owner.TreeView;
		}
		if (treeView != null)
		{
			treeView.RecalculateVisibleOrder(prevNode);
			if (treeNode == treeView.SelectedNode)
			{
				flag = true;
				OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(treeNode);
				if (openTreeNodeEnumerator.MoveNext() && openTreeNodeEnumerator.MoveNext())
				{
					selectedNode = openTreeNodeEnumerator.CurrentNode;
				}
				else
				{
					openTreeNodeEnumerator = new OpenTreeNodeEnumerator(treeNode);
					openTreeNodeEnumerator.MovePrevious();
					selectedNode = ((openTreeNodeEnumerator.CurrentNode != treeNode) ? openTreeNodeEnumerator.CurrentNode : null);
				}
			}
		}
		Array.Copy(nodes, index + 1, nodes, index, count - index - 1);
		count--;
		nodes[count] = null;
		if (nodes.Length > OrigSize && nodes.Length > count * 2)
		{
			Shrink();
		}
		if (treeView != null && flag)
		{
			treeView.SelectedNode = selectedNode;
		}
		TreeNode parent = treeNode.parent;
		treeNode.parent = null;
		if (update && treeView != null && isVisible)
		{
			treeView.RecalculateVisibleOrder(prevNode);
			treeView.UpdateScrollBars(force: false);
			treeView.UpdateBelow(parent);
		}
		treeView?.OnUIACollectionChanged(owner, new CollectionChangeEventArgs(CollectionChangeAction.Remove, treeNode));
	}

	public virtual void RemoveByKey(string key)
	{
		TreeNode treeNode = this[key];
		if (treeNode != null)
		{
			Remove(treeNode);
		}
	}

	private TreeNode GetPrevNode(TreeNode node)
	{
		OpenTreeNodeEnumerator openTreeNodeEnumerator = new OpenTreeNodeEnumerator(node);
		if (openTreeNodeEnumerator.MovePrevious() && openTreeNodeEnumerator.MovePrevious())
		{
			return openTreeNodeEnumerator.CurrentNode;
		}
		return null;
	}

	private void SetupNode(TreeNode node)
	{
		node.Remove();
		node.parent = owner;
		TreeView treeView = null;
		if (owner != null)
		{
			treeView = owner.TreeView;
		}
		if (treeView != null)
		{
			TreeNode prevNode = GetPrevNode(node);
			if (treeView.IsHandleCreated && node.ArePreviousNodesExpanded)
			{
				treeView.RecalculateVisibleOrder(prevNode);
			}
			if (owner == treeView.root_node || (node.Parent.IsVisible && node.Parent.IsExpanded))
			{
				treeView.UpdateScrollBars(force: false);
			}
		}
		if (owner != null && treeView != null && (owner.IsExpanded || owner.IsRoot))
		{
			treeView.UpdateBelow(owner);
		}
		else if (owner != null)
		{
			treeView?.UpdateBelow(owner);
		}
	}

	private int AddSorted(TreeNode node)
	{
		if (count >= nodes.Length)
		{
			Grow();
		}
		CompareInfo compareInfo = Application.CurrentCulture.CompareInfo;
		int num = 0;
		bool flag = false;
		for (int i = 0; i < count; i++)
		{
			num = i;
			int num2 = compareInfo.Compare(node.Text, nodes[i].Text);
			if (num2 < 0)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			num = count;
		}
		for (int num3 = count - 1; num3 >= num; num3--)
		{
			nodes[num3 + 1] = nodes[num3];
		}
		count++;
		nodes[num] = node;
		return count;
	}

	internal void Sort(IComparer sorter)
	{
		TreeNode[] array = nodes;
		int length = count;
		IComparer comparer2;
		if (sorter == null)
		{
			IComparer comparer = new TreeNodeComparer(Application.CurrentCulture.CompareInfo);
			comparer2 = comparer;
		}
		else
		{
			comparer2 = sorter;
		}
		Array.Sort(array, 0, length, comparer2);
		for (int i = 0; i < count; i++)
		{
			nodes[i].Nodes.Sort(sorter);
		}
	}

	private void Grow()
	{
		TreeNode[] destinationArray = new TreeNode[nodes.Length + 50];
		Array.Copy(nodes, destinationArray, nodes.Length);
		nodes = destinationArray;
	}

	private void Shrink()
	{
		int num = ((count + 1 <= OrigSize) ? OrigSize : (count + 1));
		TreeNode[] destinationArray = new TreeNode[num];
		Array.Copy(nodes, destinationArray, count);
		nodes = destinationArray;
	}

	public TreeNode[] Find(string key, bool searchAllChildren)
	{
		List<TreeNode> list = new List<TreeNode>(0);
		Find(key, searchAllChildren, this, list);
		return list.ToArray();
	}

	private static void Find(string key, bool searchAllChildren, TreeNodeCollection nodes, List<TreeNode> results)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			TreeNode treeNode = nodes[i];
			if (string.Compare(treeNode.Name, key, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
			{
				results.Add(treeNode);
			}
		}
		if (!searchAllChildren)
		{
			return;
		}
		for (int j = 0; j < nodes.Count; j++)
		{
			TreeNodeCollection treeNodeCollection = nodes[j].Nodes;
			if (treeNodeCollection.Count > 0)
			{
				Find(key, searchAllChildren, treeNodeCollection, results);
			}
		}
	}
}
