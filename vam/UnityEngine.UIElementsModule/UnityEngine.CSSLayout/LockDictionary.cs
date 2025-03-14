using System.Collections.Generic;

namespace UnityEngine.CSSLayout;

internal class LockDictionary<TKey, TValue>
{
	private object _cacheLock = new object();

	private Dictionary<TKey, TValue> _cacheItemDictionary = new Dictionary<TKey, TValue>();

	public void Set(TKey key, TValue value)
	{
		lock (_cacheLock)
		{
			_cacheItemDictionary[key] = value;
		}
	}

	public bool TryGetValue(TKey key, out TValue cacheItem)
	{
		bool flag;
		lock (_cacheLock)
		{
			flag = _cacheItemDictionary.TryGetValue(key, out cacheItem);
		}
		if (!flag)
		{
			cacheItem = default(TValue);
		}
		return flag;
	}

	public bool ContainsKey(TKey key)
	{
		bool result = false;
		lock (_cacheLock)
		{
			result = _cacheItemDictionary.ContainsKey(key);
		}
		return result;
	}

	public void Remove(TKey key)
	{
		lock (_cacheLock)
		{
			_cacheItemDictionary.Remove(key);
		}
	}
}
