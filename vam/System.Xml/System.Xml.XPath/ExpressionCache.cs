using System.Collections;
using System.Xml.Xsl;

namespace System.Xml.XPath;

internal static class ExpressionCache
{
	private static readonly Hashtable table_per_ctx = new Hashtable();

	private static object dummy = new object();

	private static object cache_lock = new object();

	public static XPathExpression Get(string xpath, IStaticXsltContext ctx)
	{
		object key = ((ctx == null) ? dummy : ctx);
		lock (cache_lock)
		{
			if (!(table_per_ctx[key] is WeakReference weakReference))
			{
				return null;
			}
			if (!(weakReference.Target is Hashtable hashtable))
			{
				table_per_ctx[key] = null;
				return null;
			}
			if (hashtable[xpath] is WeakReference weakReference2)
			{
				if (weakReference2.Target is XPathExpression result)
				{
					return result;
				}
				hashtable[xpath] = null;
			}
		}
		return null;
	}

	public static void Set(string xpath, IStaticXsltContext ctx, XPathExpression exp)
	{
		object key = ((ctx == null) ? dummy : ctx);
		Hashtable hashtable = null;
		lock (cache_lock)
		{
			if (table_per_ctx[key] is WeakReference weakReference && weakReference.IsAlive)
			{
				hashtable = (Hashtable)weakReference.Target;
			}
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				table_per_ctx[key] = new WeakReference(hashtable);
			}
			hashtable[xpath] = new WeakReference(exp);
		}
	}
}
