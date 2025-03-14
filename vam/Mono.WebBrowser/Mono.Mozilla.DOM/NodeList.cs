using System;
using System.Collections;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class NodeList : DOMObject, IEnumerable, IList, ICollection, INodeList
{
	internal class NodeListEnumerator : IEnumerator
	{
		private NodeList collection;

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

		public NodeListEnumerator(NodeList collection)
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

	protected nsIDOMNodeList unmanagedNodes;

	protected INode[] nodes;

	protected int nodeCount;

	object ICollection.SyncRoot => this;

	bool ICollection.IsSynchronized => false;

	bool IList.IsFixedSize => false;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			this[index] = value as INode;
		}
	}

	public virtual int Count
	{
		get
		{
			if (unmanagedNodes != null && nodes == null)
			{
				Load();
			}
			return nodeCount;
		}
	}

	public bool IsReadOnly => false;

	public INode this[int index]
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
			nodes[index] = value;
		}
	}

	public NodeList(WebBrowser control, nsIDOMNodeList nodeList)
		: base(control)
	{
		if (control.platform != control.enginePlatform)
		{
			unmanagedNodes = nsDOMNodeList.GetProxy(control, nodeList);
		}
		else
		{
			unmanagedNodes = nodeList;
		}
	}

	public NodeList(WebBrowser control)
		: base(control)
	{
		nodes = new Node[0];
	}

	public NodeList(WebBrowser control, bool loaded)
		: base(control)
	{
	}

	void IList.RemoveAt(int index)
	{
		RemoveAt(index);
	}

	void IList.Remove(object node)
	{
		Remove(node as INode);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, value as INode);
	}

	int IList.IndexOf(object node)
	{
		return IndexOf(node as INode);
	}

	bool IList.Contains(object node)
	{
		return Contains(node as INode);
	}

	void IList.Clear()
	{
		Clear();
	}

	int IList.Add(object node)
	{
		return Add(node as INode);
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			Clear();
		}
		base.Dispose(disposing);
	}

	protected void Clear()
	{
		if (nodes != null)
		{
			for (int i = 0; i < nodeCount; i++)
			{
				nodes[i] = null;
			}
			nodeCount = 0;
			unmanagedNodes = null;
			nodes = null;
		}
	}

	internal virtual void Load()
	{
		if (unmanagedNodes != null)
		{
			Clear();
			unmanagedNodes.getLength(out var ret);
			nodeCount = (int)ret;
			nodes = new Node[nodeCount];
			for (int i = 0; i < nodeCount; i++)
			{
				unmanagedNodes.item((uint)i, out var ret2);
				ret2.getNodeType(out var _);
				nodes[i] = GetTypedNode(ret2);
			}
		}
	}

	public IEnumerator GetEnumerator()
	{
		return new NodeListEnumerator(this);
	}

	public void CopyTo(Array dest, int index)
	{
		if (nodes != null)
		{
			Array.Copy(nodes, 0, dest, index, Count);
		}
	}

	public void RemoveAt(int index)
	{
		if (index <= Count && index >= 0)
		{
			Array.Copy(nodes, index + 1, nodes, index, nodeCount - index - 1);
			nodeCount--;
			nodes[nodeCount] = null;
		}
	}

	public void Remove(INode node)
	{
		RemoveAt(IndexOf(node));
	}

	public void Insert(int index, INode value)
	{
		if (index > Count)
		{
			index = nodeCount;
		}
		INode[] array = new Node[nodeCount + 1];
		if (index > 0)
		{
			Array.Copy(nodes, 0, array, 0, index);
		}
		array[index] = value;
		if (index < nodeCount)
		{
			Array.Copy(nodes, index, array, index + 1, nodeCount - index);
		}
		nodes = array;
		nodeCount++;
	}

	public int IndexOf(INode node)
	{
		return Array.IndexOf(nodes, node);
	}

	public bool Contains(INode node)
	{
		return IndexOf(node) != -1;
	}

	public int Add(INode node)
	{
		Insert(Count + 1, node);
		return nodeCount - 1;
	}

	public override int GetHashCode()
	{
		if (unmanagedNodes != null)
		{
			return unmanagedNodes.GetHashCode();
		}
		return base.GetHashCode();
	}
}
