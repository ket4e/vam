using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace UnityEngine;

/// <summary>
///   <para>Simple access to web pages.</para>
/// </summary>
public class WWW : CustomYieldInstruction, IDisposable
{
	private UnityWebRequest _uwr;

	private AssetBundle _assetBundle;

	private Dictionary<string, string> _responseHeaders;

	/// <summary>
	///   <para>Streams an AssetBundle that can contain any kind of asset from the project folder.</para>
	/// </summary>
	public AssetBundle assetBundle
	{
		get
		{
			if (_assetBundle == null)
			{
				if (!WaitUntilDoneIfPossible())
				{
					return null;
				}
				if (_uwr.isNetworkError)
				{
					return null;
				}
				if (_uwr.downloadHandler is DownloadHandlerAssetBundle downloadHandlerAssetBundle)
				{
					_assetBundle = downloadHandlerAssetBundle.assetBundle;
				}
				else
				{
					byte[] array = bytes;
					if (array == null)
					{
						return null;
					}
					_assetBundle = AssetBundle.LoadFromMemory(array);
				}
			}
			return _assetBundle;
		}
	}

	/// <summary>
	///   <para>Returns a AudioClip generated from the downloaded data (Read Only).</para>
	/// </summary>
	[Obsolete("Obsolete msg (UnityUpgradable) -> * UnityEngine.WWW.GetAudioClip()", true)]
	public Object audioClip => null;

	/// <summary>
	///   <para>Returns the contents of the fetched web page as a byte array (Read Only).</para>
	/// </summary>
	public byte[] bytes
	{
		get
		{
			if (!WaitUntilDoneIfPossible())
			{
				return new byte[0];
			}
			if (_uwr.isNetworkError)
			{
				return new byte[0];
			}
			DownloadHandler downloadHandler = _uwr.downloadHandler;
			if (downloadHandler == null)
			{
				return new byte[0];
			}
			return downloadHandler.data;
		}
	}

	/// <summary>
	///   <para>Returns a MovieTexture generated from the downloaded data (Read Only).</para>
	/// </summary>
	[Obsolete("Obsolete msg (UnityUpgradable) -> * UnityEngine.WWW.GetMovieTexture()", true)]
	public Object movie => null;

	[Obsolete("WWW.size is obsolete. Please use WWW.bytesDownloaded instead")]
	public int size => bytesDownloaded;

	/// <summary>
	///   <para>The number of bytes downloaded by this WWW query (read only).</para>
	/// </summary>
	public int bytesDownloaded => (int)_uwr.downloadedBytes;

	/// <summary>
	///   <para>Returns an error message if there was an error during the download (Read Only).</para>
	/// </summary>
	public string error
	{
		get
		{
			if (!_uwr.isDone)
			{
				return null;
			}
			if (_uwr.isNetworkError)
			{
				return _uwr.error;
			}
			if (_uwr.responseCode >= 400)
			{
				return $"{_uwr.responseCode} {GetStatusCodeName(_uwr.responseCode)}";
			}
			return null;
		}
	}

	/// <summary>
	///   <para>Is the download already finished? (Read Only)</para>
	/// </summary>
	public bool isDone => _uwr.isDone;

	/// <summary>
	///   <para>How far has the download progressed (Read Only).</para>
	/// </summary>
	public float progress
	{
		get
		{
			float num = _uwr.downloadProgress;
			if (num < 0f)
			{
				num = 0f;
			}
			return num;
		}
	}

	/// <summary>
	///   <para>Dictionary of headers returned by the request.</para>
	/// </summary>
	public Dictionary<string, string> responseHeaders
	{
		get
		{
			if (!isDone)
			{
				return new Dictionary<string, string>();
			}
			if (_responseHeaders == null)
			{
				_responseHeaders = _uwr.GetResponseHeaders();
				if (_responseHeaders != null)
				{
					_responseHeaders["STATUS"] = $"HTTP/1.1 {_uwr.responseCode} {GetStatusCodeName(_uwr.responseCode)}";
				}
				else
				{
					_responseHeaders = new Dictionary<string, string>();
				}
			}
			return _responseHeaders;
		}
	}

	[Obsolete("Please use WWW.text instead. (UnityUpgradable) -> text", true)]
	public string data => text;

	/// <summary>
	///   <para>Returns the contents of the fetched web page as a string (Read Only).</para>
	/// </summary>
	public string text
	{
		get
		{
			if (!WaitUntilDoneIfPossible())
			{
				return "";
			}
			if (_uwr.isNetworkError)
			{
				return "";
			}
			DownloadHandler downloadHandler = _uwr.downloadHandler;
			if (downloadHandler == null)
			{
				return "";
			}
			return downloadHandler.text;
		}
	}

	/// <summary>
	///   <para>Returns a Texture2D generated from the downloaded data (Read Only).</para>
	/// </summary>
	public Texture2D texture => CreateTextureFromDownloadedData(markNonReadable: false);

	/// <summary>
	///   <para>Returns a non-readable Texture2D generated from the downloaded data (Read Only).</para>
	/// </summary>
	public Texture2D textureNonReadable => CreateTextureFromDownloadedData(markNonReadable: true);

	/// <summary>
	///   <para>Obsolete, has no effect.</para>
	/// </summary>
	public ThreadPriority threadPriority { get; set; }

	/// <summary>
	///   <para>How far has the upload progressed (Read Only).</para>
	/// </summary>
	public float uploadProgress
	{
		get
		{
			float num = _uwr.uploadProgress;
			if (num < 0f)
			{
				num = 0f;
			}
			return num;
		}
	}

	/// <summary>
	///   <para>The URL of this WWW request (Read Only).</para>
	/// </summary>
	public string url => _uwr.url;

	public override bool keepWaiting => !_uwr.isDone;

	/// <summary>
	///   <para>Creates a WWW request with the given URL.</para>
	/// </summary>
	/// <param name="url">The url to download. Must be '%' escaped.</param>
	/// <returns>
	///   <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
	/// </returns>
	public WWW(string url)
	{
		_uwr = UnityWebRequest.Get(url);
		_uwr.SendWebRequest();
	}

	/// <summary>
	///   <para>Creates a WWW request with the given URL.</para>
	/// </summary>
	/// <param name="url">The url to download. Must be '%' escaped.</param>
	/// <param name="form">A WWWForm instance containing the form data to post.</param>
	/// <returns>
	///   <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
	/// </returns>
	public WWW(string url, WWWForm form)
	{
		_uwr = UnityWebRequest.Post(url, form);
		_uwr.chunkedTransfer = false;
		_uwr.SendWebRequest();
	}

	/// <summary>
	///   <para>Creates a WWW request with the given URL.</para>
	/// </summary>
	/// <param name="url">The url to download. Must be '%' escaped.</param>
	/// <param name="postData">A byte array of data to be posted to the url.</param>
	/// <returns>
	///   <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
	/// </returns>
	public WWW(string url, byte[] postData)
	{
		_uwr = new UnityWebRequest(url, "POST");
		_uwr.chunkedTransfer = false;
		UploadHandler uploadHandler = new UploadHandlerRaw(postData)
		{
			contentType = "application/x-www-form-urlencoded"
		};
		_uwr.uploadHandler = uploadHandler;
		_uwr.downloadHandler = new DownloadHandlerBuffer();
		_uwr.SendWebRequest();
	}

	/// <summary>
	///   <para>Creates a WWW request with the given URL.</para>
	/// </summary>
	/// <param name="url">The url to download. Must be '%' escaped.</param>
	/// <param name="postData">A byte array of data to be posted to the url.</param>
	/// <param name="headers">A hash table of custom headers to send with the request.</param>
	/// <returns>
	///   <para>A new WWW object. When it has been downloaded, the results can be fetched from the returned object.</para>
	/// </returns>
	[Obsolete("This overload is deprecated. Use UnityEngine.WWW.WWW(string, byte[], System.Collections.Generic.Dictionary<string, string>) instead.")]
	public WWW(string url, byte[] postData, Hashtable headers)
	{
		_uwr = new UnityWebRequest(url, (postData != null) ? "POST" : "GET");
		_uwr.chunkedTransfer = false;
		UploadHandler uploadHandler = new UploadHandlerRaw(postData)
		{
			contentType = "application/x-www-form-urlencoded"
		};
		_uwr.uploadHandler = uploadHandler;
		_uwr.downloadHandler = new DownloadHandlerBuffer();
		foreach (object key in headers.Keys)
		{
			_uwr.SetRequestHeader((string)key, (string)headers[key]);
		}
		_uwr.SendWebRequest();
	}

	public WWW(string url, byte[] postData, Dictionary<string, string> headers)
	{
		_uwr = new UnityWebRequest(url, (postData != null) ? "POST" : "GET");
		_uwr.chunkedTransfer = false;
		UploadHandler uploadHandler = new UploadHandlerRaw(postData)
		{
			contentType = "application/x-www-form-urlencoded"
		};
		_uwr.uploadHandler = uploadHandler;
		_uwr.downloadHandler = new DownloadHandlerBuffer();
		foreach (KeyValuePair<string, string> header in headers)
		{
			_uwr.SetRequestHeader(header.Key, header.Value);
		}
		_uwr.SendWebRequest();
	}

	internal WWW(string url, string name, Hash128 hash, uint crc)
	{
		_uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, new CachedAssetBundle(name, hash), crc);
		_uwr.SendWebRequest();
	}

	/// <summary>
	///   <para>Escapes characters in a string to ensure they are URL-friendly.</para>
	/// </summary>
	/// <param name="s">A string with characters to be escaped.</param>
	/// <param name="e">The text encoding to use.</param>
	public static string EscapeURL(string s)
	{
		return EscapeURL(s, Encoding.UTF8);
	}

	/// <summary>
	///   <para>Escapes characters in a string to ensure they are URL-friendly.</para>
	/// </summary>
	/// <param name="s">A string with characters to be escaped.</param>
	/// <param name="e">The text encoding to use.</param>
	public static string EscapeURL(string s, Encoding e)
	{
		return UnityWebRequest.EscapeURL(s, e);
	}

	/// <summary>
	///   <para>Converts URL-friendly escape sequences back to normal text.</para>
	/// </summary>
	/// <param name="s">A string containing escaped characters.</param>
	/// <param name="e">The text encoding to use.</param>
	public static string UnEscapeURL(string s)
	{
		return UnEscapeURL(s, Encoding.UTF8);
	}

	/// <summary>
	///   <para>Converts URL-friendly escape sequences back to normal text.</para>
	/// </summary>
	/// <param name="s">A string containing escaped characters.</param>
	/// <param name="e">The text encoding to use.</param>
	public static string UnEscapeURL(string s, Encoding e)
	{
		return UnityWebRequest.UnEscapeURL(s, e);
	}

	/// <summary>
	///   <para>Loads an AssetBundle with the specified version number from the cache. If the AssetBundle is not currently cached, it will automatically be downloaded and stored in the cache for future retrieval from local storage.</para>
	/// </summary>
	/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
	/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBundle from url.</param>
	/// <param name="hash">Hash128 which is used as the version of the AssetBundle.</param>
	/// <param name="cachedBundle">A structure used to download a given version of AssetBundle to a customized cache path.
	///
	/// Analogous to the cachedAssetBundle parameter for UnityWebRequestAssetBundle.GetAssetBundle.&lt;/param&gt;</param>
	/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
	/// <returns>
	///   <para>A WWW instance, which can be used to access the data once the load/download operation is completed.</para>
	/// </returns>
	public static WWW LoadFromCacheOrDownload(string url, int version)
	{
		return LoadFromCacheOrDownload(url, version, 0u);
	}

	/// <summary>
	///   <para>Loads an AssetBundle with the specified version number from the cache. If the AssetBundle is not currently cached, it will automatically be downloaded and stored in the cache for future retrieval from local storage.</para>
	/// </summary>
	/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
	/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBundle from url.</param>
	/// <param name="hash">Hash128 which is used as the version of the AssetBundle.</param>
	/// <param name="cachedBundle">A structure used to download a given version of AssetBundle to a customized cache path.
	///
	/// Analogous to the cachedAssetBundle parameter for UnityWebRequestAssetBundle.GetAssetBundle.&lt;/param&gt;</param>
	/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
	/// <returns>
	///   <para>A WWW instance, which can be used to access the data once the load/download operation is completed.</para>
	/// </returns>
	public static WWW LoadFromCacheOrDownload(string url, int version, uint crc)
	{
		Hash128 hash = new Hash128(0u, 0u, 0u, (uint)version);
		return LoadFromCacheOrDownload(url, hash, crc);
	}

	public static WWW LoadFromCacheOrDownload(string url, Hash128 hash)
	{
		return LoadFromCacheOrDownload(url, hash, 0u);
	}

	/// <summary>
	///   <para>Loads an AssetBundle with the specified version number from the cache. If the AssetBundle is not currently cached, it will automatically be downloaded and stored in the cache for future retrieval from local storage.</para>
	/// </summary>
	/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
	/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBundle from url.</param>
	/// <param name="hash">Hash128 which is used as the version of the AssetBundle.</param>
	/// <param name="cachedBundle">A structure used to download a given version of AssetBundle to a customized cache path.
	///
	/// Analogous to the cachedAssetBundle parameter for UnityWebRequestAssetBundle.GetAssetBundle.&lt;/param&gt;</param>
	/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
	/// <returns>
	///   <para>A WWW instance, which can be used to access the data once the load/download operation is completed.</para>
	/// </returns>
	public static WWW LoadFromCacheOrDownload(string url, Hash128 hash, uint crc)
	{
		return new WWW(url, "", hash, crc);
	}

	/// <summary>
	///   <para>Loads an AssetBundle with the specified version number from the cache. If the AssetBundle is not currently cached, it will automatically be downloaded and stored in the cache for future retrieval from local storage.</para>
	/// </summary>
	/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
	/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBundle from url.</param>
	/// <param name="hash">Hash128 which is used as the version of the AssetBundle.</param>
	/// <param name="cachedBundle">A structure used to download a given version of AssetBundle to a customized cache path.
	///
	/// Analogous to the cachedAssetBundle parameter for UnityWebRequestAssetBundle.GetAssetBundle.&lt;/param&gt;</param>
	/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
	/// <returns>
	///   <para>A WWW instance, which can be used to access the data once the load/download operation is completed.</para>
	/// </returns>
	public static WWW LoadFromCacheOrDownload(string url, CachedAssetBundle cachedBundle, uint crc = 0u)
	{
		return new WWW(url, cachedBundle.name, cachedBundle.hash, crc);
	}

	private Texture2D CreateTextureFromDownloadedData(bool markNonReadable)
	{
		if (!WaitUntilDoneIfPossible())
		{
			return new Texture2D(2, 2);
		}
		if (_uwr.isNetworkError)
		{
			return null;
		}
		DownloadHandler downloadHandler = _uwr.downloadHandler;
		if (downloadHandler == null)
		{
			return null;
		}
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(downloadHandler.data, markNonReadable);
		return texture2D;
	}

	/// <summary>
	///   <para>Replaces the contents of an existing Texture2D with an image from the downloaded data.</para>
	/// </summary>
	/// <param name="tex">An existing texture object to be overwritten with the image data.</param>
	/// <param name="texture"></param>
	public void LoadImageIntoTexture(Texture2D texture)
	{
		if (!WaitUntilDoneIfPossible())
		{
			return;
		}
		if (_uwr.isNetworkError)
		{
			Debug.LogError("Cannot load image: download failed");
			return;
		}
		DownloadHandler downloadHandler = _uwr.downloadHandler;
		if (downloadHandler == null)
		{
			Debug.LogError("Cannot load image: internal error");
		}
		else
		{
			texture.LoadImage(downloadHandler.data, markNonReadable: false);
		}
	}

	/// <summary>
	///   <para>Disposes of an existing WWW object.</para>
	/// </summary>
	public void Dispose()
	{
		_uwr.Dispose();
	}

	internal Object GetAudioClipInternal(bool threeD, bool stream, bool compressed, AudioType audioType)
	{
		return WebRequestWWW.InternalCreateAudioClipUsingDH(_uwr.downloadHandler, _uwr.url, stream, compressed, audioType);
	}

	internal object GetMovieTextureInternal()
	{
		return WebRequestWWW.InternalCreateMovieTextureUsingDH(_uwr.downloadHandler);
	}

	public AudioClip GetAudioClip()
	{
		return GetAudioClip(threeD: true, stream: false, AudioType.UNKNOWN);
	}

	/// <summary>
	///   <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
	/// </summary>
	/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip
	/// the .audioClip property defaults to 3D.</param>
	/// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false) or the stream can be played even if only part of the clip is downloaded (true).
	/// The latter will disable seeking on the clip (with .time and/or .timeSamples).</param>
	/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
	/// <returns>
	///   <para>The returned AudioClip.</para>
	/// </returns>
	public AudioClip GetAudioClip(bool threeD)
	{
		return GetAudioClip(threeD, stream: false, AudioType.UNKNOWN);
	}

	/// <summary>
	///   <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
	/// </summary>
	/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip
	/// the .audioClip property defaults to 3D.</param>
	/// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false) or the stream can be played even if only part of the clip is downloaded (true).
	/// The latter will disable seeking on the clip (with .time and/or .timeSamples).</param>
	/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
	/// <returns>
	///   <para>The returned AudioClip.</para>
	/// </returns>
	public AudioClip GetAudioClip(bool threeD, bool stream)
	{
		return GetAudioClip(threeD, stream, AudioType.UNKNOWN);
	}

	/// <summary>
	///   <para>Returns an AudioClip generated from the downloaded data (Read Only).</para>
	/// </summary>
	/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip
	/// the .audioClip property defaults to 3D.</param>
	/// <param name="stream">Sets whether the clip should be completely downloaded before it's ready to play (false) or the stream can be played even if only part of the clip is downloaded (true).
	/// The latter will disable seeking on the clip (with .time and/or .timeSamples).</param>
	/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
	/// <returns>
	///   <para>The returned AudioClip.</para>
	/// </returns>
	public AudioClip GetAudioClip(bool threeD, bool stream, AudioType audioType)
	{
		return (AudioClip)GetAudioClipInternal(threeD, stream, compressed: false, audioType);
	}

	/// <summary>
	///   <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
	/// </summary>
	/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
	/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
	/// <returns>
	///   <para>The returned AudioClip.</para>
	/// </returns>
	public AudioClip GetAudioClipCompressed()
	{
		return GetAudioClipCompressed(threeD: false, AudioType.UNKNOWN);
	}

	/// <summary>
	///   <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
	/// </summary>
	/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
	/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
	/// <returns>
	///   <para>The returned AudioClip.</para>
	/// </returns>
	public AudioClip GetAudioClipCompressed(bool threeD)
	{
		return GetAudioClipCompressed(threeD, AudioType.UNKNOWN);
	}

	/// <summary>
	///   <para>Returns an AudioClip generated from the downloaded data that is compressed in memory (Read Only).</para>
	/// </summary>
	/// <param name="threeD">Use this to specify whether the clip should be a 2D or 3D clip.</param>
	/// <param name="audioType">The AudioType of the content your downloading. If this is not set Unity will try to determine the type from URL.</param>
	/// <returns>
	///   <para>The returned AudioClip.</para>
	/// </returns>
	public AudioClip GetAudioClipCompressed(bool threeD, AudioType audioType)
	{
		return (AudioClip)GetAudioClipInternal(threeD, stream: false, compressed: true, audioType);
	}

	/// <summary>
	///   <para>Returns a MovieTexture generated from the downloaded data (Read Only).</para>
	/// </summary>
	public MovieTexture GetMovieTexture()
	{
		return (MovieTexture)GetMovieTextureInternal();
	}

	private bool WaitUntilDoneIfPossible()
	{
		if (_uwr.isDone)
		{
			return true;
		}
		if (url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
		{
			while (!_uwr.isDone)
			{
			}
			return true;
		}
		Debug.LogError("You are trying to load data from a www stream which has not completed the download yet.\nYou need to yield the download or wait until isDone returns true.");
		return false;
	}

	private string GetStatusCodeName(long statusCode)
	{
		if (statusCode >= 400 && statusCode <= 417)
		{
			switch (statusCode - 400)
			{
			case 0L:
				return "Bad Request";
			case 1L:
				return "Unauthorized";
			case 2L:
				return "Payment Required";
			case 3L:
				return "Forbidden";
			case 4L:
				return "Not Found";
			case 5L:
				return "Method Not Allowed";
			case 6L:
				return "Not Acceptable";
			case 7L:
				return "Proxy Authentication Required";
			case 8L:
				return "Request Timeout";
			case 9L:
				return "Conflict";
			case 10L:
				return "Gone";
			case 11L:
				return "Length Required";
			case 12L:
				return "Precondition Failed";
			case 13L:
				return "Request Entity Too Large";
			case 14L:
				return "Request-URI Too Long";
			case 15L:
				return "Unsupported Media Type";
			case 16L:
				return "Requested Range Not Satisfiable";
			case 17L:
				return "Expectation Failed";
			}
		}
		if (statusCode >= 200 && statusCode <= 206)
		{
			switch (statusCode - 200)
			{
			case 0L:
				return "OK";
			case 1L:
				return "Created";
			case 2L:
				return "Accepted";
			case 3L:
				return "Non-Authoritative Information";
			case 4L:
				return "No Content";
			case 5L:
				return "Reset Content";
			case 6L:
				return "Partial Content";
			}
		}
		if (statusCode >= 300 && statusCode <= 307)
		{
			switch (statusCode - 300)
			{
			case 0L:
				return "Multiple Choices";
			case 1L:
				return "Moved Permanently";
			case 2L:
				return "Found";
			case 3L:
				return "See Other";
			case 4L:
				return "Not Modified";
			case 5L:
				return "Use Proxy";
			case 7L:
				return "Temporary Redirect";
			}
		}
		if (statusCode >= 500 && statusCode <= 505)
		{
			switch (statusCode - 500)
			{
			case 0L:
				return "Internal Server Error";
			case 1L:
				return "Not Implemented";
			case 2L:
				return "Bad Gateway";
			case 3L:
				return "Service Unavailable";
			case 4L:
				return "Gateway Timeout";
			case 5L:
				return "HTTP Version Not Supported";
			}
		}
		return "";
	}
}
