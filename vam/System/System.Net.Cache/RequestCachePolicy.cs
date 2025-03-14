namespace System.Net.Cache;

public class RequestCachePolicy
{
	private RequestCacheLevel level;

	public RequestCacheLevel Level => level;

	public RequestCachePolicy()
	{
	}

	public RequestCachePolicy(RequestCacheLevel level)
	{
		this.level = level;
	}

	[System.MonoTODO]
	public override string ToString()
	{
		throw new NotImplementedException();
	}
}
