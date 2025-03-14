using System;
using System.Collections.Generic;
using AOT;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public class CookieManager
{
	private class CookieFetch
	{
		public BrowserNative.GetCookieFunc nativeCB;

		public Promise<List<Cookie>> promise;

		public CookieManager manager;

		public List<Cookie> result;
	}

	internal readonly Browser browser;

	private static CookieFetch currentFetch;

	public CookieManager(Browser browser)
	{
		this.browser = browser;
	}

	public IPromise<List<Cookie>> GetCookies()
	{
		if (currentFetch != null)
		{
			throw new InvalidOperationException("GetCookies is not reentrant");
		}
		Cookie.Init();
		List<Cookie> list = new List<Cookie>();
		if (!browser.IsReady || !browser.enabled)
		{
			return Promise<List<Cookie>>.Resolved(list);
		}
		Promise<List<Cookie>> promise = new Promise<List<Cookie>>();
		BrowserNative.GetCookieFunc getCookieFunc = CB_GetCookieFunc;
		BrowserNative.zfb_getCookies(browser.browserId, getCookieFunc);
		CookieFetch cookieFetch = new CookieFetch();
		cookieFetch.promise = promise;
		cookieFetch.nativeCB = getCookieFunc;
		cookieFetch.manager = this;
		cookieFetch.result = list;
		currentFetch = cookieFetch;
		return promise;
	}

	[MonoPInvokeCallback(typeof(BrowserNative.GetCookieFunc))]
	private static void CB_GetCookieFunc(BrowserNative.NativeCookie cookie)
	{
		try
		{
			if (cookie == null)
			{
				List<Cookie> result = currentFetch.result;
				Promise<List<Cookie>> promise = currentFetch.promise;
				currentFetch.manager.browser.RunOnMainThread(delegate
				{
					promise.Resolve(result);
				});
				currentFetch = null;
			}
			else
			{
				currentFetch.result.Add(new Cookie(currentFetch.manager, cookie));
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void ClearAll()
	{
		if (!browser.DeferUnready(ClearAll))
		{
			BrowserNative.zfb_clearCookies(browser.browserId);
		}
	}
}
