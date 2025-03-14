using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Linq;

public abstract class JContainer : JToken, IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable, IList, ICollection
{
	private object _syncRoot;

	private bool _busy;

	protected abstract IList<JToken> ChildrenTokens { get; }

	public override bool HasValues => ChildrenTokens.Count > 0;

	public override JToken First
	{
		get
		{
			IList<JToken> childrenTokens = ChildrenTokens;
			if (childrenTokens.Count <= 0)
			{
				return null;
			}
			return childrenTokens[0];
		}
	}

	public override JToken Last
	{
		get
		{
			IList<JToken> childrenTokens = ChildrenTokens;
			int count = childrenTokens.Count;
			if (count <= 0)
			{
				return null;
			}
			return childrenTokens[count - 1];
		}
	}

	JToken IList<JToken>.this[int index]
	{
		get
		{
			return GetItem(index);
		}
		set
		{
			SetItem(index, value);
		}
	}

	bool ICollection<JToken>.IsReadOnly => false;

	bool IList.IsFixedSize => false;

	bool IList.IsReadOnly => false;

	object IList.this[int index]
	{
		get
		{
			return GetItem(index);
		}
		set
		{
			SetItem(index, EnsureValue(value));
		}
	}

	public int Count => ChildrenTokens.Count;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
	{
		get
		{
			if (_syncRoot == null)
			{
				Interlocked.CompareExchange(ref _syncRoot, new object(), null);
			}
			return _syncRoot;
		}
	}

	internal JContainer()
	{
	}

	internal JContainer(JContainer other)
		: this()
	{
		ValidationUtils.ArgumentNotNull(other, "other");
		int num = 0;
		foreach (JToken item in (IEnumerable<JToken>)other)
		{
			AddInternal(num, item, skipParentCheck: false);
			num++;
		}
	}

	internal void CheckReentrancy()
	{
		if (_busy)
		{
			throw new InvalidOperationException("Cannot change {0} during a collection change event.".FormatWith(CultureInfo.InvariantCulture, GetType()));
		}
	}

	internal virtual IList<JToken> CreateChildrenCollection()
	{
		return new List<JToken>();
	}

	internal bool ContentsEqual(JContainer container)
	{
		if (container == this)
		{
			return true;
		}
		IList<JToken> childrenTokens = ChildrenTokens;
		IList<JToken> childrenTokens2 = container.ChildrenTokens;
		if (childrenTokens.Count != childrenTokens2.Count)
		{
			return false;
		}
		for (int i = 0; i < childrenTokens.Count; i++)
		{
			if (!childrenTokens[i].DeepEquals(childrenTokens2[i]))
			{
				return false;
			}
		}
		return true;
	}

	public override JEnumerable<JToken> Children()
	{
		return new JEnumerable<JToken>(ChildrenTokens);
	}

	public override IEnumerable<T> Values<T>()
	{
		return ChildrenTokens.Convert<JToken, T>();
	}

	public IEnumerable<JToken> Descendants()
	{
		return GetDescendants(self: false);
	}

	public IEnumerable<JToken> DescendantsAndSelf()
	{
		return GetDescendants(self: true);
	}

	internal IEnumerable<JToken> GetDescendants(bool self)
	{
		if (self)
		{
			yield return this;
		}
		foreach (JToken o in ChildrenTokens)
		{
			yield return o;
			if (!(o is JContainer jContainer))
			{
				continue;
			}
			foreach (JToken item in jContainer.Descendants())
			{
				yield return item;
			}
		}
	}

	internal bool IsMultiContent(object content)
	{
		if (content is IEnumerable && !(content is string) && !(content is JToken))
		{
			return !(content is byte[]);
		}
		return false;
	}

	internal JToken EnsureParentToken(JToken item, bool skipParentCheck)
	{
		if (item == null)
		{
			return JValue.CreateNull();
		}
		if (skipParentCheck)
		{
			return item;
		}
		if (item.Parent != null || item == this || (item.HasValues && base.Root == item))
		{
			item = item.CloneToken();
		}
		return item;
	}

	internal abstract int IndexOfItem(JToken item);

	internal virtual void InsertItem(int index, JToken item, bool skipParentCheck)
	{
		IList<JToken> childrenTokens = ChildrenTokens;
		if (index > childrenTokens.Count)
		{
			throw new ArgumentOutOfRangeException("index", "Index must be within the bounds of the List.");
		}
		CheckReentrancy();
		item = EnsureParentToken(item, skipParentCheck);
		JToken jToken = ((index == 0) ? null : childrenTokens[index - 1]);
		JToken jToken2 = ((index == childrenTokens.Count) ? null : childrenTokens[index]);
		ValidateToken(item, null);
		item.Parent = this;
		item.Previous = jToken;
		if (jToken != null)
		{
			jToken.Next = item;
		}
		item.Next = jToken2;
		if (jToken2 != null)
		{
			jToken2.Previous = item;
		}
		childrenTokens.Insert(index, item);
	}

	internal virtual void RemoveItemAt(int index)
	{
		IList<JToken> childrenTokens = ChildrenTokens;
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
		}
		if (index >= childrenTokens.Count)
		{
			throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
		}
		CheckReentrancy();
		JToken jToken = childrenTokens[index];
		JToken jToken2 = ((index == 0) ? null : childrenTokens[index - 1]);
		JToken jToken3 = ((index == childrenTokens.Count - 1) ? null : childrenTokens[index + 1]);
		if (jToken2 != null)
		{
			jToken2.Next = jToken3;
		}
		if (jToken3 != null)
		{
			jToken3.Previous = jToken2;
		}
		jToken.Parent = null;
		jToken.Previous = null;
		jToken.Next = null;
		childrenTokens.RemoveAt(index);
	}

	internal virtual bool RemoveItem(JToken item)
	{
		int num = IndexOfItem(item);
		if (num >= 0)
		{
			RemoveItemAt(num);
			return true;
		}
		return false;
	}

	internal virtual JToken GetItem(int index)
	{
		return ChildrenTokens[index];
	}

	internal virtual void SetItem(int index, JToken item)
	{
		IList<JToken> childrenTokens = ChildrenTokens;
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
		}
		if (index >= childrenTokens.Count)
		{
			throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
		}
		JToken jToken = childrenTokens[index];
		if (!IsTokenUnchanged(jToken, item))
		{
			CheckReentrancy();
			item = EnsureParentToken(item, skipParentCheck: false);
			ValidateToken(item, jToken);
			JToken jToken2 = ((index == 0) ? null : childrenTokens[index - 1]);
			JToken jToken3 = ((index == childrenTokens.Count - 1) ? null : childrenTokens[index + 1]);
			item.Parent = this;
			item.Previous = jToken2;
			if (jToken2 != null)
			{
				jToken2.Next = item;
			}
			item.Next = jToken3;
			if (jToken3 != null)
			{
				jToken3.Previous = item;
			}
			childrenTokens[index] = item;
			jToken.Parent = null;
			jToken.Previous = null;
			jToken.Next = null;
		}
	}

	internal virtual void ClearItems()
	{
		CheckReentrancy();
		IList<JToken> childrenTokens = ChildrenTokens;
		foreach (JToken item in childrenTokens)
		{
			item.Parent = null;
			item.Previous = null;
			item.Next = null;
		}
		childrenTokens.Clear();
	}

	internal virtual void ReplaceItem(JToken existing, JToken replacement)
	{
		if (existing != null && existing.Parent == this)
		{
			int index = IndexOfItem(existing);
			SetItem(index, replacement);
		}
	}

	internal virtual bool ContainsItem(JToken item)
	{
		return IndexOfItem(item) != -1;
	}

	internal virtual void CopyItemsTo(Array array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
		}
		if (arrayIndex >= array.Length && arrayIndex != 0)
		{
			throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
		}
		if (Count > array.Length - arrayIndex)
		{
			throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
		}
		int num = 0;
		foreach (JToken childrenToken in ChildrenTokens)
		{
			array.SetValue(childrenToken, arrayIndex + num);
			num++;
		}
	}

	internal static bool IsTokenUnchanged(JToken currentValue, JToken newValue)
	{
		if (currentValue is JValue jValue)
		{
			if (jValue.Type == JTokenType.Null && newValue == null)
			{
				return true;
			}
			return jValue.Equals(newValue);
		}
		return false;
	}

	internal virtual void ValidateToken(JToken o, JToken existing)
	{
		ValidationUtils.ArgumentNotNull(o, "o");
		if (o.Type == JTokenType.Property)
		{
			throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), GetType()));
		}
	}

	public virtual void Add(object content)
	{
		AddInternal(ChildrenTokens.Count, content, skipParentCheck: false);
	}

	internal void AddAndSkipParentCheck(JToken token)
	{
		AddInternal(ChildrenTokens.Count, token, skipParentCheck: true);
	}

	public void AddFirst(object content)
	{
		AddInternal(0, content, skipParentCheck: false);
	}

	internal void AddInternal(int index, object content, bool skipParentCheck)
	{
		if (IsMultiContent(content))
		{
			IEnumerable obj = (IEnumerable)content;
			int num = index;
			{
				foreach (object item2 in obj)
				{
					AddInternal(num, item2, skipParentCheck);
					num++;
				}
				return;
			}
		}
		JToken item = CreateFromContent(content);
		InsertItem(index, item, skipParentCheck);
	}

	internal static JToken CreateFromContent(object content)
	{
		if (content is JToken result)
		{
			return result;
		}
		return new JValue(content);
	}

	public JsonWriter CreateWriter()
	{
		return new JTokenWriter(this);
	}

	public void ReplaceAll(object content)
	{
		ClearItems();
		Add(content);
	}

	public void RemoveAll()
	{
		ClearItems();
	}

	internal abstract void MergeItem(object content, JsonMergeSettings settings);

	public void Merge(object content)
	{
		MergeItem(content, new JsonMergeSettings());
	}

	public void Merge(object content, JsonMergeSettings settings)
	{
		MergeItem(content, settings);
	}

	internal void ReadTokenFrom(JsonReader reader, JsonLoadSettings options)
	{
		int depth = reader.Depth;
		if (!reader.Read())
		{
			throw JsonReaderException.Create(reader, "Error reading {0} from JsonReader.".FormatWith(CultureInfo.InvariantCulture, GetType().Name));
		}
		ReadContentFrom(reader, options);
		if (reader.Depth > depth)
		{
			throw JsonReaderException.Create(reader, "Unexpected end of content while loading {0}.".FormatWith(CultureInfo.InvariantCulture, GetType().Name));
		}
	}

	internal void ReadContentFrom(JsonReader r, JsonLoadSettings settings)
	{
		ValidationUtils.ArgumentNotNull(r, "r");
		IJsonLineInfo lineInfo = r as IJsonLineInfo;
		JContainer jContainer = this;
		do
		{
			if ((jContainer as JProperty)?.Value != null)
			{
				if (jContainer == this)
				{
					break;
				}
				jContainer = jContainer.Parent;
			}
			switch (r.TokenType)
			{
			case JsonToken.StartArray:
			{
				JArray jArray = new JArray();
				jArray.SetLineInfo(lineInfo, settings);
				jContainer.Add(jArray);
				jContainer = jArray;
				break;
			}
			case JsonToken.EndArray:
				if (jContainer == this)
				{
					return;
				}
				jContainer = jContainer.Parent;
				break;
			case JsonToken.StartObject:
			{
				JObject jObject = new JObject();
				jObject.SetLineInfo(lineInfo, settings);
				jContainer.Add(jObject);
				jContainer = jObject;
				break;
			}
			case JsonToken.EndObject:
				if (jContainer == this)
				{
					return;
				}
				jContainer = jContainer.Parent;
				break;
			case JsonToken.StartConstructor:
			{
				JConstructor jConstructor = new JConstructor(r.Value.ToString());
				jConstructor.SetLineInfo(lineInfo, settings);
				jContainer.Add(jConstructor);
				jContainer = jConstructor;
				break;
			}
			case JsonToken.EndConstructor:
				if (jContainer == this)
				{
					return;
				}
				jContainer = jContainer.Parent;
				break;
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Date:
			case JsonToken.Bytes:
			{
				JValue jValue = new JValue(r.Value);
				jValue.SetLineInfo(lineInfo, settings);
				jContainer.Add(jValue);
				break;
			}
			case JsonToken.Comment:
				if (settings != null && settings.CommentHandling == CommentHandling.Load)
				{
					JValue jValue = JValue.CreateComment(r.Value.ToString());
					jValue.SetLineInfo(lineInfo, settings);
					jContainer.Add(jValue);
				}
				break;
			case JsonToken.Null:
			{
				JValue jValue = JValue.CreateNull();
				jValue.SetLineInfo(lineInfo, settings);
				jContainer.Add(jValue);
				break;
			}
			case JsonToken.Undefined:
			{
				JValue jValue = JValue.CreateUndefined();
				jValue.SetLineInfo(lineInfo, settings);
				jContainer.Add(jValue);
				break;
			}
			case JsonToken.PropertyName:
			{
				string name = r.Value.ToString();
				JProperty jProperty = new JProperty(name);
				jProperty.SetLineInfo(lineInfo, settings);
				JProperty jProperty2 = ((JObject)jContainer).Property(name);
				if (jProperty2 == null)
				{
					jContainer.Add(jProperty);
				}
				else
				{
					jProperty2.Replace(jProperty);
				}
				jContainer = jProperty;
				break;
			}
			default:
				throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith(CultureInfo.InvariantCulture, r.TokenType));
			case JsonToken.None:
				break;
			}
		}
		while (r.Read());
	}

	internal int ContentsHashCode()
	{
		int num = 0;
		foreach (JToken childrenToken in ChildrenTokens)
		{
			num ^= childrenToken.GetDeepHashCode();
		}
		return num;
	}

	int IList<JToken>.IndexOf(JToken item)
	{
		return IndexOfItem(item);
	}

	void IList<JToken>.Insert(int index, JToken item)
	{
		InsertItem(index, item, skipParentCheck: false);
	}

	void IList<JToken>.RemoveAt(int index)
	{
		RemoveItemAt(index);
	}

	void ICollection<JToken>.Add(JToken item)
	{
		Add(item);
	}

	void ICollection<JToken>.Clear()
	{
		ClearItems();
	}

	bool ICollection<JToken>.Contains(JToken item)
	{
		return ContainsItem(item);
	}

	void ICollection<JToken>.CopyTo(JToken[] array, int arrayIndex)
	{
		CopyItemsTo(array, arrayIndex);
	}

	bool ICollection<JToken>.Remove(JToken item)
	{
		return RemoveItem(item);
	}

	private JToken EnsureValue(object value)
	{
		if (value == null)
		{
			return null;
		}
		if (value is JToken result)
		{
			return result;
		}
		throw new ArgumentException("Argument is not a JToken.");
	}

	int IList.Add(object value)
	{
		Add(EnsureValue(value));
		return Count - 1;
	}

	void IList.Clear()
	{
		ClearItems();
	}

	bool IList.Contains(object value)
	{
		return ContainsItem(EnsureValue(value));
	}

	int IList.IndexOf(object value)
	{
		return IndexOfItem(EnsureValue(value));
	}

	void IList.Insert(int index, object value)
	{
		InsertItem(index, EnsureValue(value), skipParentCheck: false);
	}

	void IList.Remove(object value)
	{
		RemoveItem(EnsureValue(value));
	}

	void IList.RemoveAt(int index)
	{
		RemoveItemAt(index);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		CopyItemsTo(array, index);
	}

	internal static void MergeEnumerableContent(JContainer target, IEnumerable content, JsonMergeSettings settings)
	{
		switch (settings.MergeArrayHandling)
		{
		case MergeArrayHandling.Concat:
		{
			foreach (JToken item in content)
			{
				target.Add(item);
			}
			break;
		}
		case MergeArrayHandling.Union:
		{
			HashSet<JToken> hashSet = new HashSet<JToken>(target, JToken.EqualityComparer);
			{
				foreach (JToken item2 in content)
				{
					if (hashSet.Add(item2))
					{
						target.Add(item2);
					}
				}
				break;
			}
		}
		case MergeArrayHandling.Replace:
			target.ClearItems();
			{
				foreach (JToken item3 in content)
				{
					target.Add(item3);
				}
				break;
			}
		case MergeArrayHandling.Merge:
		{
			int num = 0;
			{
				foreach (object item4 in content)
				{
					if (num < target.Count)
					{
						if (target[num] is JContainer jContainer)
						{
							jContainer.Merge(item4, settings);
						}
						else if (item4 != null)
						{
							JToken jToken = CreateFromContent(item4);
							if (jToken.Type != JTokenType.Null)
							{
								target[num] = jToken;
							}
						}
					}
					else
					{
						target.Add(item4);
					}
					num++;
				}
				break;
			}
		}
		default:
			throw new ArgumentOutOfRangeException("settings", "Unexpected merge array handling when merging JSON.");
		}
	}
}
