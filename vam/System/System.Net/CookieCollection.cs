using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace System.Net;

[Serializable]
public class CookieCollection : ICollection, IEnumerable
{
	private sealed class CookieCollectionComparer : IComparer<Cookie>
	{
		public int Compare(Cookie x, Cookie y)
		{
			if (x == null || y == null)
			{
				return 0;
			}
			int num = x.Name.Length + x.Value.Length;
			int num2 = y.Name.Length + y.Value.Length;
			return num - num2;
		}
	}

	private List<Cookie> list = new List<Cookie>();

	private static CookieCollectionComparer Comparer = new CookieCollectionComparer();

	internal IList<Cookie> List => list;

	public int Count => list.Count;

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	public bool IsReadOnly => true;

	public Cookie this[int index]
	{
		get
		{
			if (index < 0 || index >= list.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return list[index];
		}
	}

	public Cookie this[string name]
	{
		get
		{
			foreach (Cookie item in list)
			{
				if (string.Compare(item.Name, name, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
				{
					return item;
				}
			}
			return null;
		}
	}

	public void CopyTo(Array array, int index)
	{
		((ICollection)list).CopyTo(array, index);
	}

	public void CopyTo(Cookie[] array, int index)
	{
		list.CopyTo(array, index);
	}

	public IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public void Add(Cookie cookie)
	{
		if (cookie == null)
		{
			throw new ArgumentNullException("cookie");
		}
		int num = SearchCookie(cookie);
		if (num == -1)
		{
			list.Add(cookie);
		}
		else
		{
			list[num] = cookie;
		}
	}

	internal void Sort()
	{
		if (list.Count > 0)
		{
			list.Sort(Comparer);
		}
	}

	private int SearchCookie(Cookie cookie)
	{
		string name = cookie.Name;
		string domain = cookie.Domain;
		string path = cookie.Path;
		for (int num = list.Count - 1; num >= 0; num--)
		{
			Cookie cookie2 = list[num];
			if (cookie2.Version == cookie.Version && string.Compare(domain, cookie2.Domain, ignoreCase: true, CultureInfo.InvariantCulture) == 0 && string.Compare(name, cookie2.Name, ignoreCase: true, CultureInfo.InvariantCulture) == 0 && string.Compare(path, cookie2.Path, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
			{
				return num;
			}
		}
		return -1;
	}

	public void Add(CookieCollection cookies)
	{
		if (cookies == null)
		{
			throw new ArgumentNullException("cookies");
		}
		foreach (Cookie cookie in cookies)
		{
			Add(cookie);
		}
	}
}
