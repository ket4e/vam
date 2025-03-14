namespace System.Net.Cache;

public class HttpRequestCachePolicy : RequestCachePolicy
{
	private DateTime cacheSyncDate;

	private HttpRequestCacheLevel level;

	private TimeSpan maxAge;

	private TimeSpan maxStale;

	private TimeSpan minFresh;

	public DateTime CacheSyncDate => cacheSyncDate;

	public new HttpRequestCacheLevel Level => level;

	public TimeSpan MaxAge => maxAge;

	public TimeSpan MaxStale => maxStale;

	public TimeSpan MinFresh => minFresh;

	public HttpRequestCachePolicy()
	{
	}

	public HttpRequestCachePolicy(DateTime cacheSyncDate)
	{
		this.cacheSyncDate = cacheSyncDate;
	}

	public HttpRequestCachePolicy(HttpRequestCacheLevel level)
	{
		this.level = level;
	}

	public HttpRequestCachePolicy(HttpCacheAgeControl cacheAgeControl, TimeSpan ageOrFreshOrStale)
	{
		switch (cacheAgeControl)
		{
		case HttpCacheAgeControl.MaxAge:
			maxAge = ageOrFreshOrStale;
			break;
		case HttpCacheAgeControl.MaxStale:
			maxStale = ageOrFreshOrStale;
			break;
		case HttpCacheAgeControl.MinFresh:
			minFresh = ageOrFreshOrStale;
			break;
		default:
			throw new ArgumentException("ageOrFreshOrStale");
		}
	}

	public HttpRequestCachePolicy(HttpCacheAgeControl cacheAgeControl, TimeSpan maxAge, TimeSpan freshOrStale)
	{
		this.maxAge = maxAge;
		switch (cacheAgeControl)
		{
		case HttpCacheAgeControl.MaxStale:
			maxStale = freshOrStale;
			break;
		case HttpCacheAgeControl.MinFresh:
			minFresh = freshOrStale;
			break;
		default:
			throw new ArgumentException("freshOrStale");
		}
	}

	public HttpRequestCachePolicy(HttpCacheAgeControl cacheAgeControl, TimeSpan maxAge, TimeSpan freshOrStale, DateTime cacheSyncDate)
		: this(cacheAgeControl, maxAge, freshOrStale)
	{
		this.cacheSyncDate = cacheSyncDate;
	}

	[System.MonoTODO]
	public override string ToString()
	{
		throw new NotImplementedException();
	}
}
