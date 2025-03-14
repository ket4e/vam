namespace System;

internal class UriData : System.IUriData
{
	private Uri uri;

	private UriParser parser;

	private string absolute_path;

	private string absolute_uri;

	private string absolute_uri_unescaped;

	private string authority;

	private string fragment;

	private string host;

	private string path_and_query;

	private string strong_port;

	private string query;

	private string user_info;

	public string AbsolutePath => Lookup(ref absolute_path, UriComponents.Path | UriComponents.KeepDelimiter);

	public string AbsoluteUri => Lookup(ref absolute_uri, UriComponents.AbsoluteUri);

	public string AbsoluteUri_SafeUnescaped => Lookup(ref absolute_uri_unescaped, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);

	public string Authority => Lookup(ref authority, UriComponents.Host | UriComponents.Port);

	public string Fragment => Lookup(ref fragment, UriComponents.Fragment | UriComponents.KeepDelimiter);

	public string Host => Lookup(ref host, UriComponents.Host);

	public string PathAndQuery => Lookup(ref path_and_query, UriComponents.PathAndQuery);

	public string StrongPort => Lookup(ref strong_port, UriComponents.StrongPort);

	public string Query => Lookup(ref query, UriComponents.Query | UriComponents.KeepDelimiter);

	public string UserInfo => Lookup(ref user_info, UriComponents.UserInfo);

	public UriData(Uri uri, UriParser parser)
	{
		this.uri = uri;
		this.parser = parser;
	}

	private string Lookup(ref string cache, UriComponents components)
	{
		return Lookup(ref cache, components, (!uri.UserEscaped) ? UriFormat.UriEscaped : UriFormat.Unescaped);
	}

	private string Lookup(ref string cache, UriComponents components, UriFormat format)
	{
		if (cache == null)
		{
			cache = parser.GetComponents(uri, components, format);
		}
		return cache;
	}
}
