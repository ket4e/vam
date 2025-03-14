using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngineInternal;

namespace UnityEngine.Networking;

/// <summary>
///   <para>The UnityWebRequest object is used to communicate with web servers.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/UnityWebRequest/Public/UnityWebRequest.h")]
public class UnityWebRequest : IDisposable
{
	internal enum UnityWebRequestMethod
	{
		Get,
		Post,
		Put,
		Head,
		Custom
	}

	internal enum UnityWebRequestError
	{
		OK,
		Unknown,
		SDKError,
		UnsupportedProtocol,
		MalformattedUrl,
		CannotResolveProxy,
		CannotResolveHost,
		CannotConnectToHost,
		AccessDenied,
		GenericHttpError,
		WriteError,
		ReadError,
		OutOfMemory,
		Timeout,
		HTTPPostError,
		SSLCannotConnect,
		Aborted,
		TooManyRedirects,
		ReceivedNoData,
		SSLNotSupported,
		FailedToSendData,
		FailedToReceiveData,
		SSLCertificateError,
		SSLCipherNotAvailable,
		SSLCACertError,
		UnrecognizedContentEncoding,
		LoginFailed,
		SSLShutdownFailed,
		NoInternetConnection
	}

	[NonSerialized]
	internal IntPtr m_Ptr;

	[NonSerialized]
	internal DownloadHandler m_DownloadHandler;

	[NonSerialized]
	internal UploadHandler m_UploadHandler;

	[NonSerialized]
	internal CertificateHandler m_CertificateHandler;

	[NonSerialized]
	internal Uri m_Uri;

	/// <summary>
	///   <para>The string "GET", commonly used as the verb for an HTTP GET request.</para>
	/// </summary>
	public const string kHttpVerbGET = "GET";

	/// <summary>
	///   <para>The string "HEAD", commonly used as the verb for an HTTP HEAD request.</para>
	/// </summary>
	public const string kHttpVerbHEAD = "HEAD";

	/// <summary>
	///   <para>The string "POST", commonly used as the verb for an HTTP POST request.</para>
	/// </summary>
	public const string kHttpVerbPOST = "POST";

	/// <summary>
	///   <para>The string "PUT", commonly used as the verb for an HTTP PUT request.</para>
	/// </summary>
	public const string kHttpVerbPUT = "PUT";

	/// <summary>
	///   <para>The string "CREATE", commonly used as the verb for an HTTP CREATE request.</para>
	/// </summary>
	public const string kHttpVerbCREATE = "CREATE";

	/// <summary>
	///   <para>The string "DELETE", commonly used as the verb for an HTTP DELETE request.</para>
	/// </summary>
	public const string kHttpVerbDELETE = "DELETE";

	/// <summary>
	///   <para>If true, any CertificateHandler attached to this UnityWebRequest will have CertificateHandler.Dispose called automatically when UnityWebRequest.Dispose is called.</para>
	/// </summary>
	public bool disposeCertificateHandlerOnDispose { get; set; }

	/// <summary>
	///   <para>If true, any DownloadHandler attached to this UnityWebRequest will have DownloadHandler.Dispose called automatically when UnityWebRequest.Dispose is called.</para>
	/// </summary>
	public bool disposeDownloadHandlerOnDispose { get; set; }

	/// <summary>
	///   <para>If true, any UploadHandler attached to this UnityWebRequest will have UploadHandler.Dispose called automatically when UnityWebRequest.Dispose is called.</para>
	/// </summary>
	public bool disposeUploadHandlerOnDispose { get; set; }

	/// <summary>
	///   <para>Defines the HTTP verb used by this UnityWebRequest, such as GET or POST.</para>
	/// </summary>
	public string method
	{
		get
		{
			return GetMethod() switch
			{
				UnityWebRequestMethod.Get => "GET", 
				UnityWebRequestMethod.Post => "POST", 
				UnityWebRequestMethod.Put => "PUT", 
				UnityWebRequestMethod.Head => "HEAD", 
				_ => GetCustomMethod(), 
			};
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Cannot set a UnityWebRequest's method to an empty or null string");
			}
			switch (value.ToUpper())
			{
			case "GET":
				InternalSetMethod(UnityWebRequestMethod.Get);
				break;
			case "POST":
				InternalSetMethod(UnityWebRequestMethod.Post);
				break;
			case "PUT":
				InternalSetMethod(UnityWebRequestMethod.Put);
				break;
			case "HEAD":
				InternalSetMethod(UnityWebRequestMethod.Head);
				break;
			default:
				InternalSetCustomMethod(value.ToUpper());
				break;
			}
		}
	}

	/// <summary>
	///   <para>A human-readable string describing any system errors encountered by this UnityWebRequest object while handling HTTP requests or responses. (Read Only)</para>
	/// </summary>
	public string error
	{
		get
		{
			if (!isNetworkError && !isHttpError)
			{
				return null;
			}
			return GetWebErrorString(GetError());
		}
	}

	private extern bool use100Continue
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Determines whether this UnityWebRequest will include Expect: 100-Continue in its outgoing request headers. (Default: true).</para>
	/// </summary>
	public bool useHttpContinue
	{
		get
		{
			return use100Continue;
		}
		set
		{
			if (!isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its 100-Continue setting cannot be altered");
			}
			use100Continue = value;
		}
	}

	/// <summary>
	///   <para>Defines the target URL for the UnityWebRequest to communicate with.</para>
	/// </summary>
	public string url
	{
		get
		{
			return GetUrl();
		}
		set
		{
			string localUrl = "http://localhost/";
			InternalSetUrl(WebRequestUtils.MakeInitialUrl(value, localUrl));
		}
	}

	/// <summary>
	///   <para>Defines the target URI for the UnityWebRequest to communicate with.</para>
	/// </summary>
	public Uri uri
	{
		get
		{
			return new Uri(GetUrl());
		}
		set
		{
			if (!value.IsAbsoluteUri)
			{
				throw new ArgumentException("URI must be absolute");
			}
			InternalSetUrl(WebRequestUtils.MakeUriString(value, value.OriginalString, prependProtocol: false));
			m_Uri = value;
		}
	}

	/// <summary>
	///   <para>The numeric HTTP response code returned by the server, such as 200, 404 or 500. (Read Only)</para>
	/// </summary>
	public extern long responseCode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Returns a floating-point value between 0.0 and 1.0, indicating the progress of uploading body data to the server.</para>
	/// </summary>
	public float uploadProgress
	{
		get
		{
			if (!IsExecuting() && !isDone)
			{
				return -1f;
			}
			return GetUploadProgress();
		}
	}

	/// <summary>
	///   <para>Returns true while a UnityWebRequest’s configuration properties can be altered. (Read Only)</para>
	/// </summary>
	public extern bool isModifiable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsModifiable")]
		get;
	}

	/// <summary>
	///   <para>Returns true after the UnityWebRequest has finished communicating with the remote server. (Read Only)</para>
	/// </summary>
	public extern bool isDone
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsDone")]
		get;
	}

	/// <summary>
	///   <para>Returns true after this UnityWebRequest encounters a system error. (Read Only)</para>
	/// </summary>
	public extern bool isNetworkError
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsNetworkError")]
		get;
	}

	/// <summary>
	///   <para>Returns true after this UnityWebRequest receives an HTTP response code indicating an error. (Read Only)</para>
	/// </summary>
	public extern bool isHttpError
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsHttpError")]
		get;
	}

	/// <summary>
	///   <para>Returns a floating-point value between 0.0 and 1.0, indicating the progress of downloading body data from the server. (Read Only)</para>
	/// </summary>
	public float downloadProgress
	{
		get
		{
			if (!IsExecuting() && !isDone)
			{
				return -1f;
			}
			return GetDownloadProgress();
		}
	}

	/// <summary>
	///   <para>Returns the number of bytes of body data the system has uploaded to the remote server. (Read Only)</para>
	/// </summary>
	public extern ulong uploadedBytes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Returns the number of bytes of body data the system has downloaded from the remote server. (Read Only)</para>
	/// </summary>
	public extern ulong downloadedBytes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Indicates the number of redirects which this UnityWebRequest will follow before halting with a “Redirect Limit Exceeded” system error.</para>
	/// </summary>
	public int redirectLimit
	{
		get
		{
			return GetRedirectLimit();
		}
		set
		{
			SetRedirectLimitFromScripting(value);
		}
	}

	/// <summary>
	///   <para>Indicates whether the UnityWebRequest system should employ the HTTP/1.1 chunked-transfer encoding method.</para>
	/// </summary>
	public bool chunkedTransfer
	{
		get
		{
			return GetChunked();
		}
		set
		{
			if (!isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent and its chunked transfer encoding setting cannot be altered");
			}
			UnityWebRequestError unityWebRequestError = SetChunked(value);
			if (unityWebRequestError != 0)
			{
				throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
			}
		}
	}

	/// <summary>
	///   <para>Holds a reference to the UploadHandler object which manages body data to be uploaded to the remote server.</para>
	/// </summary>
	public UploadHandler uploadHandler
	{
		get
		{
			return m_UploadHandler;
		}
		set
		{
			if (!isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the upload handler");
			}
			UnityWebRequestError unityWebRequestError = SetUploadHandler(value);
			if (unityWebRequestError != 0)
			{
				throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
			}
			m_UploadHandler = value;
		}
	}

	/// <summary>
	///   <para>Holds a reference to a DownloadHandler object, which manages body data received from the remote server by this UnityWebRequest.</para>
	/// </summary>
	public DownloadHandler downloadHandler
	{
		get
		{
			return m_DownloadHandler;
		}
		set
		{
			if (!isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the download handler");
			}
			UnityWebRequestError unityWebRequestError = SetDownloadHandler(value);
			if (unityWebRequestError != 0)
			{
				throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
			}
			m_DownloadHandler = value;
		}
	}

	/// <summary>
	///   <para>Holds a reference to a CertificateHandler object, which manages certificate validation for this UnityWebRequest.</para>
	/// </summary>
	public CertificateHandler certificateHandler
	{
		get
		{
			return m_CertificateHandler;
		}
		set
		{
			if (!isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the certificate handler");
			}
			UnityWebRequestError unityWebRequestError = SetCertificateHandler(value);
			if (unityWebRequestError != 0)
			{
				throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
			}
			m_CertificateHandler = value;
		}
	}

	/// <summary>
	///   <para>Sets UnityWebRequest to attempt to abort after the number of seconds in timeout have passed.</para>
	/// </summary>
	public int timeout
	{
		get
		{
			return GetTimeoutMsec() / 1000;
		}
		set
		{
			if (!isModifiable)
			{
				throw new InvalidOperationException("UnityWebRequest has already been sent; cannot modify the timeout");
			}
			value = Math.Max(value, 0);
			UnityWebRequestError unityWebRequestError = SetTimeoutMsec(value * 1000);
			if (unityWebRequestError != 0)
			{
				throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
			}
		}
	}

	/// <summary>
	///   <para>Creates a UnityWebRequest with the default options and no attached DownloadHandler or UploadHandler. Default method is GET.</para>
	/// </summary>
	/// <param name="url">The target URL with which this UnityWebRequest will communicate. Also accessible via the url property.</param>
	public UnityWebRequest()
	{
		m_Ptr = Create();
		InternalSetDefaults();
	}

	/// <summary>
	///   <para>Creates a UnityWebRequest with the default options and no attached DownloadHandler or UploadHandler. Default method is GET.</para>
	/// </summary>
	/// <param name="url">The target URL with which this UnityWebRequest will communicate. Also accessible via the url property.</param>
	public UnityWebRequest(string url)
	{
		m_Ptr = Create();
		InternalSetDefaults();
		this.url = url;
	}

	public UnityWebRequest(Uri uri)
	{
		m_Ptr = Create();
		InternalSetDefaults();
		this.uri = uri;
	}

	public UnityWebRequest(string url, string method)
	{
		m_Ptr = Create();
		InternalSetDefaults();
		this.url = url;
		this.method = method;
	}

	public UnityWebRequest(Uri uri, string method)
	{
		m_Ptr = Create();
		InternalSetDefaults();
		this.uri = uri;
		this.method = method;
	}

	public UnityWebRequest(string url, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
	{
		m_Ptr = Create();
		InternalSetDefaults();
		this.url = url;
		this.method = method;
		this.downloadHandler = downloadHandler;
		this.uploadHandler = uploadHandler;
	}

	public UnityWebRequest(Uri uri, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
	{
		m_Ptr = Create();
		InternalSetDefaults();
		this.uri = uri;
		this.method = method;
		this.downloadHandler = downloadHandler;
		this.uploadHandler = uploadHandler;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	[NativeConditional("ENABLE_UNITYWEBREQUEST")]
	private static extern string GetWebErrorString(UnityWebRequestError err);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern IntPtr Create();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern void Release();

	internal void InternalDestroy()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Abort();
			Release();
			m_Ptr = IntPtr.Zero;
		}
	}

	private void InternalSetDefaults()
	{
		disposeDownloadHandlerOnDispose = true;
		disposeUploadHandlerOnDispose = true;
		disposeCertificateHandlerOnDispose = true;
	}

	~UnityWebRequest()
	{
		DisposeHandlers();
		InternalDestroy();
	}

	/// <summary>
	///   <para>Signals that this [UnityWebRequest] is no longer being used, and should clean up any resources it is using.</para>
	/// </summary>
	public void Dispose()
	{
		DisposeHandlers();
		InternalDestroy();
		GC.SuppressFinalize(this);
	}

	private void DisposeHandlers()
	{
		if (disposeDownloadHandlerOnDispose)
		{
			downloadHandler?.Dispose();
		}
		if (disposeUploadHandlerOnDispose)
		{
			uploadHandler?.Dispose();
		}
		if (disposeCertificateHandlerOnDispose)
		{
			certificateHandler?.Dispose();
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	internal extern UnityWebRequestAsyncOperation BeginWebRequest();

	/// <summary>
	///   <para>Begin communicating with the remote server.</para>
	/// </summary>
	/// <returns>
	///   <para>An AsyncOperation indicating the progress/completion state of the UnityWebRequest. Yield this object to wait until the UnityWebRequest is done.</para>
	/// </returns>
	[Obsolete("Use SendWebRequest.  It returns a UnityWebRequestAsyncOperation which contains a reference to the WebRequest object.", false)]
	public AsyncOperation Send()
	{
		return SendWebRequest();
	}

	/// <summary>
	///   <para>Begin communicating with the remote server.
	///
	/// After calling this method, the UnityWebRequest will perform DNS resolution (if necessary), transmit an HTTP request to the remote server at the target URL and process the server’s response.
	///
	/// This method can only be called once on any given UnityWebRequest object. Once this method is called, you cannot change any of the UnityWebRequest’s properties.
	///
	/// This method returns a WebRequestAsyncOperation object. Yielding the WebRequestAsyncOperation inside a coroutine will cause the coroutine to pause until the UnityWebRequest encounters a system error or finishes communicating.</para>
	/// </summary>
	public UnityWebRequestAsyncOperation SendWebRequest()
	{
		UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = BeginWebRequest();
		if (unityWebRequestAsyncOperation != null)
		{
			unityWebRequestAsyncOperation.webRequest = this;
		}
		return unityWebRequestAsyncOperation;
	}

	/// <summary>
	///   <para>If in progress, halts the UnityWebRequest as soon as possible.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	public extern void Abort();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetMethod(UnityWebRequestMethod methodType);

	internal void InternalSetMethod(UnityWebRequestMethod methodType)
	{
		if (!isModifiable)
		{
			throw new InvalidOperationException("UnityWebRequest has already been sent and its request method can no longer be altered");
		}
		UnityWebRequestError unityWebRequestError = SetMethod(methodType);
		if (unityWebRequestError != 0)
		{
			throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetCustomMethod(string customMethodName);

	internal void InternalSetCustomMethod(string customMethodName)
	{
		if (!isModifiable)
		{
			throw new InvalidOperationException("UnityWebRequest has already been sent and its request method can no longer be altered");
		}
		UnityWebRequestError unityWebRequestError = SetCustomMethod(customMethodName);
		if (unityWebRequestError != 0)
		{
			throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern UnityWebRequestMethod GetMethod();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern string GetCustomMethod();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError GetError();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern string GetUrl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetUrl(string url);

	private void InternalSetUrl(string url)
	{
		if (!isModifiable)
		{
			throw new InvalidOperationException("UnityWebRequest has already been sent and its URL cannot be altered");
		}
		UnityWebRequestError unityWebRequestError = SetUrl(url);
		if (unityWebRequestError != 0)
		{
			throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern float GetUploadProgress();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool IsExecuting();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern float GetDownloadProgress();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetRedirectLimit();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private extern void SetRedirectLimitFromScripting(int limit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool GetChunked();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetChunked(bool chunked);

	/// <summary>
	///   <para>Retrieves the value of a custom request header.</para>
	/// </summary>
	/// <param name="name">Name of the custom request header. Case-insensitive.</param>
	/// <returns>
	///   <para>The value of the custom request header. If no custom header with a matching name has been set, returns an empty string.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern string GetRequestHeader(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("SetRequestHeader")]
	internal extern UnityWebRequestError InternalSetRequestHeader(string name, string value);

	/// <summary>
	///   <para>Set a HTTP request header to a custom value.</para>
	/// </summary>
	/// <param name="name">The key of the header to be set. Case-sensitive.</param>
	/// <param name="value">The header's intended value.</param>
	public void SetRequestHeader(string name, string value)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Cannot set a Request Header with a null or empty name");
		}
		if (value == null)
		{
			throw new ArgumentException("Cannot set a Request header with a null");
		}
		if (!isModifiable)
		{
			throw new InvalidOperationException("UnityWebRequest has already been sent and its request headers cannot be altered");
		}
		UnityWebRequestError unityWebRequestError = InternalSetRequestHeader(name, value);
		if (unityWebRequestError != 0)
		{
			throw new InvalidOperationException(GetWebErrorString(unityWebRequestError));
		}
	}

	/// <summary>
	///   <para>Retrieves the value of a response header from the latest HTTP response received.</para>
	/// </summary>
	/// <param name="name">The name of the HTTP header to retrieve. Case-insensitive.</param>
	/// <returns>
	///   <para>The value of the HTTP header from the latest HTTP response. If no header with a matching name has been received, or no responses have been received, returns null.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern string GetResponseHeader(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern string[] GetResponseHeaderKeys();

	/// <summary>
	///   <para>Retrieves a dictionary containing all the response headers received by this UnityWebRequest in the latest HTTP response.</para>
	/// </summary>
	/// <returns>
	///   <para>A dictionary containing all the response headers received in the latest HTTP response. If no responses have been received, returns null.</para>
	/// </returns>
	public Dictionary<string, string> GetResponseHeaders()
	{
		string[] responseHeaderKeys = GetResponseHeaderKeys();
		if (responseHeaderKeys == null || responseHeaderKeys.Length == 0)
		{
			return null;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>(responseHeaderKeys.Length, StringComparer.OrdinalIgnoreCase);
		for (int i = 0; i < responseHeaderKeys.Length; i++)
		{
			string responseHeader = GetResponseHeader(responseHeaderKeys[i]);
			dictionary.Add(responseHeaderKeys[i], responseHeader);
		}
		return dictionary;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetUploadHandler(UploadHandler uh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetDownloadHandler(DownloadHandler dh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetCertificateHandler(CertificateHandler ch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetTimeoutMsec();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnityWebRequestError SetTimeoutMsec(int timeout);

	/// <summary>
	///   <para>Creates a UnityWebRequest configured for HTTP GET.</para>
	/// </summary>
	/// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
	/// <returns>
	///   <para>A UnityWebRequest object configured to retrieve data from uri.</para>
	/// </returns>
	public static UnityWebRequest Get(string uri)
	{
		return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
	}

	public static UnityWebRequest Get(Uri uri)
	{
		return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
	}

	/// <summary>
	///   <para>Creates a UnityWebRequest configured for HTTP DELETE.</para>
	/// </summary>
	/// <param name="uri">The URI to which a DELETE request should be sent.</param>
	/// <returns>
	///   <para>A UnityWebRequest configured to send an HTTP DELETE request.</para>
	/// </returns>
	public static UnityWebRequest Delete(string uri)
	{
		return new UnityWebRequest(uri, "DELETE");
	}

	public static UnityWebRequest Delete(Uri uri)
	{
		return new UnityWebRequest(uri, "DELETE");
	}

	/// <summary>
	///   <para>Creates a UnityWebRequest configured to send a HTTP HEAD request.</para>
	/// </summary>
	/// <param name="uri">The URI to which to send a HTTP HEAD request.</param>
	/// <returns>
	///   <para>A UnityWebRequest configured to transmit a HTTP HEAD request.</para>
	/// </returns>
	public static UnityWebRequest Head(string uri)
	{
		return new UnityWebRequest(uri, "HEAD");
	}

	public static UnityWebRequest Head(Uri uri)
	{
		return new UnityWebRequest(uri, "HEAD");
	}

	/// <summary>
	///   <para>Creates a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data.</para>
	/// </summary>
	/// <param name="uri">The URI of the image to download.</param>
	/// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
	/// <returns>
	///   <para>A UnityWebRequest properly configured to download an image and convert it to a Texture.</para>
	/// </returns>
	[Obsolete("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestTexture.GetTexture(*)", true)]
	public static UnityWebRequest GetTexture(string uri)
	{
		throw new NotSupportedException("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead.");
	}

	/// <summary>
	///   <para>Creates a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data.</para>
	/// </summary>
	/// <param name="uri">The URI of the image to download.</param>
	/// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
	/// <returns>
	///   <para>A UnityWebRequest properly configured to download an image and convert it to a Texture.</para>
	/// </returns>
	[Obsolete("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestTexture.GetTexture(*)", true)]
	public static UnityWebRequest GetTexture(string uri, bool nonReadable)
	{
		throw new NotSupportedException("UnityWebRequest.GetTexture is obsolete. Use UnityWebRequestTexture.GetTexture instead.");
	}

	/// <summary>
	///   <para>OBSOLETE. Use UnityWebRequestMultimedia.GetAudioClip().</para>
	/// </summary>
	/// <param name="uri"></param>
	/// <param name="audioType"></param>
	[Obsolete("UnityWebRequest.GetAudioClip is obsolete. Use UnityWebRequestMultimedia.GetAudioClip instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestMultimedia.GetAudioClip(*)", true)]
	public static UnityWebRequest GetAudioClip(string uri, AudioType audioType)
	{
		return null;
	}

	[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
	public static UnityWebRequest GetAssetBundle(string uri)
	{
		return null;
	}

	/// <summary>
	///   <para>Deprecated. Replaced by UnityWebRequestAssetBundle.GetAssetBundle.</para>
	/// </summary>
	/// <param name="uri"></param>
	/// <param name="crc"></param>
	/// <param name="version"></param>
	/// <param name="hash"></param>
	/// <param name="cachedAssetBundle"></param>
	[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
	public static UnityWebRequest GetAssetBundle(string uri, uint crc)
	{
		return null;
	}

	/// <summary>
	///   <para>Deprecated. Replaced by UnityWebRequestAssetBundle.GetAssetBundle.</para>
	/// </summary>
	/// <param name="uri"></param>
	/// <param name="crc"></param>
	/// <param name="version"></param>
	/// <param name="hash"></param>
	/// <param name="cachedAssetBundle"></param>
	[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
	public static UnityWebRequest GetAssetBundle(string uri, uint version, uint crc)
	{
		return null;
	}

	/// <summary>
	///   <para>Deprecated. Replaced by UnityWebRequestAssetBundle.GetAssetBundle.</para>
	/// </summary>
	/// <param name="uri"></param>
	/// <param name="crc"></param>
	/// <param name="version"></param>
	/// <param name="hash"></param>
	/// <param name="cachedAssetBundle"></param>
	[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
	public static UnityWebRequest GetAssetBundle(string uri, Hash128 hash, uint crc)
	{
		return null;
	}

	/// <summary>
	///   <para>Deprecated. Replaced by UnityWebRequestAssetBundle.GetAssetBundle.</para>
	/// </summary>
	/// <param name="uri"></param>
	/// <param name="crc"></param>
	/// <param name="version"></param>
	/// <param name="hash"></param>
	/// <param name="cachedAssetBundle"></param>
	[Obsolete("UnityWebRequest.GetAssetBundle is obsolete. Use UnityWebRequestAssetBundle.GetAssetBundle instead (UnityUpgradable) -> [UnityEngine] UnityWebRequestAssetBundle.GetAssetBundle(*)", true)]
	public static UnityWebRequest GetAssetBundle(string uri, CachedAssetBundle cachedAssetBundle, uint crc)
	{
		return null;
	}

	/// <summary>
	///   <para>Creates a UnityWebRequest configured to upload raw data to a remote server via HTTP PUT.</para>
	/// </summary>
	/// <param name="uri">The URI to which the data will be sent.</param>
	/// <param name="bodyData">The data to transmit to the remote server.
	///
	/// If a string, the string will be converted to raw bytes via &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.text.encoding.utf8"&gt;System.Text.Encoding.UTF8&lt;a&gt;.</param>
	/// <returns>
	///   <para>A UnityWebRequest configured to transmit bodyData to uri via HTTP PUT.</para>
	/// </returns>
	public static UnityWebRequest Put(string uri, byte[] bodyData)
	{
		return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyData));
	}

	public static UnityWebRequest Put(Uri uri, byte[] bodyData)
	{
		return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyData));
	}

	/// <summary>
	///   <para>Creates a UnityWebRequest configured to upload raw data to a remote server via HTTP PUT.</para>
	/// </summary>
	/// <param name="uri">The URI to which the data will be sent.</param>
	/// <param name="bodyData">The data to transmit to the remote server.
	///
	/// If a string, the string will be converted to raw bytes via &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.text.encoding.utf8"&gt;System.Text.Encoding.UTF8&lt;a&gt;.</param>
	/// <returns>
	///   <para>A UnityWebRequest configured to transmit bodyData to uri via HTTP PUT.</para>
	/// </returns>
	public static UnityWebRequest Put(string uri, string bodyData)
	{
		return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData)));
	}

	public static UnityWebRequest Put(Uri uri, string bodyData)
	{
		return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData)));
	}

	/// <summary>
	///   <para>Creates a UnityWebRequest configured to send form data to a server via HTTP POST.</para>
	/// </summary>
	/// <param name="uri">The target URI to which form data will be transmitted.</param>
	/// <param name="postData">Form body data. Will be URLEncoded prior to transmission.</param>
	/// <returns>
	///   <para>A UnityWebRequest configured to send form data to uri via POST.</para>
	/// </returns>
	public static UnityWebRequest Post(string uri, string postData)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
		SetupPost(unityWebRequest, postData);
		return unityWebRequest;
	}

	public static UnityWebRequest Post(Uri uri, string postData)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
		SetupPost(unityWebRequest, postData);
		return unityWebRequest;
	}

	private static void SetupPost(UnityWebRequest request, string postData)
	{
		byte[] data = null;
		if (!string.IsNullOrEmpty(postData))
		{
			string s = WWWTranscoder.DataEncode(postData, Encoding.UTF8);
			data = Encoding.UTF8.GetBytes(s);
		}
		request.uploadHandler = new UploadHandlerRaw(data);
		request.uploadHandler.contentType = "application/x-www-form-urlencoded";
		request.downloadHandler = new DownloadHandlerBuffer();
	}

	/// <summary>
	///   <para>Create a UnityWebRequest configured to send form data to a server via HTTP POST.</para>
	/// </summary>
	/// <param name="uri">The target URI to which form data will be transmitted.</param>
	/// <param name="formData">Form fields or files encapsulated in a WWWForm object, for formatting and transmission to the remote server.</param>
	/// <returns>
	///   <para>A UnityWebRequest configured to send form data to uri via POST.</para>
	/// </returns>
	public static UnityWebRequest Post(string uri, WWWForm formData)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
		SetupPost(unityWebRequest, formData);
		return unityWebRequest;
	}

	public static UnityWebRequest Post(Uri uri, WWWForm formData)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
		SetupPost(unityWebRequest, formData);
		return unityWebRequest;
	}

	private static void SetupPost(UnityWebRequest request, WWWForm formData)
	{
		byte[] array = null;
		if (formData != null)
		{
			array = formData.data;
			if (array.Length == 0)
			{
				array = null;
			}
		}
		request.uploadHandler = new UploadHandlerRaw(array);
		request.downloadHandler = new DownloadHandlerBuffer();
		if (formData == null)
		{
			return;
		}
		Dictionary<string, string> headers = formData.headers;
		foreach (KeyValuePair<string, string> item in headers)
		{
			request.SetRequestHeader(item.Key, item.Value);
		}
	}

	public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections)
	{
		byte[] boundary = GenerateBoundary();
		return Post(uri, multipartFormSections, boundary);
	}

	public static UnityWebRequest Post(Uri uri, List<IMultipartFormSection> multipartFormSections)
	{
		byte[] boundary = GenerateBoundary();
		return Post(uri, multipartFormSections, boundary);
	}

	public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
		SetupPost(unityWebRequest, multipartFormSections, boundary);
		return unityWebRequest;
	}

	public static UnityWebRequest Post(Uri uri, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
		SetupPost(unityWebRequest, multipartFormSections, boundary);
		return unityWebRequest;
	}

	private static void SetupPost(UnityWebRequest request, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
	{
		byte[] data = null;
		if (multipartFormSections != null && multipartFormSections.Count != 0)
		{
			data = SerializeFormSections(multipartFormSections, boundary);
		}
		UploadHandler uploadHandler = new UploadHandlerRaw(data);
		uploadHandler.contentType = "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary, 0, boundary.Length);
		request.uploadHandler = uploadHandler;
		request.downloadHandler = new DownloadHandlerBuffer();
	}

	public static UnityWebRequest Post(string uri, Dictionary<string, string> formFields)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
		SetupPost(unityWebRequest, formFields);
		return unityWebRequest;
	}

	public static UnityWebRequest Post(Uri uri, Dictionary<string, string> formFields)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
		SetupPost(unityWebRequest, formFields);
		return unityWebRequest;
	}

	private static void SetupPost(UnityWebRequest request, Dictionary<string, string> formFields)
	{
		byte[] data = null;
		if (formFields != null && formFields.Count != 0)
		{
			data = SerializeSimpleForm(formFields);
		}
		UploadHandler uploadHandler = new UploadHandlerRaw(data);
		uploadHandler.contentType = "application/x-www-form-urlencoded";
		request.uploadHandler = uploadHandler;
		request.downloadHandler = new DownloadHandlerBuffer();
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
		if (s == null)
		{
			return null;
		}
		if (s == "")
		{
			return "";
		}
		if (e == null)
		{
			return null;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte[] bytes2 = WWWTranscoder.URLEncode(bytes);
		return e.GetString(bytes2);
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
		if (s == null)
		{
			return null;
		}
		if (s.IndexOf('%') == -1 && s.IndexOf('+') == -1)
		{
			return s;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte[] bytes2 = WWWTranscoder.URLDecode(bytes);
		return e.GetString(bytes2);
	}

	public static byte[] SerializeFormSections(List<IMultipartFormSection> multipartFormSections, byte[] boundary)
	{
		if (multipartFormSections == null || multipartFormSections.Count == 0)
		{
			return null;
		}
		byte[] bytes = Encoding.UTF8.GetBytes("\r\n");
		byte[] bytes2 = WWWForm.DefaultEncoding.GetBytes("--");
		int num = 0;
		foreach (IMultipartFormSection multipartFormSection in multipartFormSections)
		{
			num += 64 + multipartFormSection.sectionData.Length;
		}
		List<byte> list = new List<byte>(num);
		foreach (IMultipartFormSection multipartFormSection2 in multipartFormSections)
		{
			string text = "form-data";
			string sectionName = multipartFormSection2.sectionName;
			string fileName = multipartFormSection2.fileName;
			string text2 = "Content-Disposition: " + text;
			if (!string.IsNullOrEmpty(sectionName))
			{
				text2 = text2 + "; name=\"" + sectionName + "\"";
			}
			if (!string.IsNullOrEmpty(fileName))
			{
				text2 = text2 + "; filename=\"" + fileName + "\"";
			}
			text2 += "\r\n";
			string contentType = multipartFormSection2.contentType;
			if (!string.IsNullOrEmpty(contentType))
			{
				text2 = text2 + "Content-Type: " + contentType + "\r\n";
			}
			list.AddRange(bytes);
			list.AddRange(bytes2);
			list.AddRange(boundary);
			list.AddRange(bytes);
			list.AddRange(Encoding.UTF8.GetBytes(text2));
			list.AddRange(bytes);
			list.AddRange(multipartFormSection2.sectionData);
		}
		list.AddRange(bytes);
		list.AddRange(bytes2);
		list.AddRange(boundary);
		list.AddRange(bytes2);
		list.AddRange(bytes);
		return list.ToArray();
	}

	/// <summary>
	///   <para>Generate a random 40-byte array for use as a multipart form boundary.</para>
	/// </summary>
	/// <returns>
	///   <para>40 random bytes, guaranteed to contain only printable ASCII values.</para>
	/// </returns>
	public static byte[] GenerateBoundary()
	{
		byte[] array = new byte[40];
		for (int i = 0; i < 40; i++)
		{
			int num = Random.Range(48, 110);
			if (num > 57)
			{
				num += 7;
			}
			if (num > 90)
			{
				num += 6;
			}
			array[i] = (byte)num;
		}
		return array;
	}

	public static byte[] SerializeSimpleForm(Dictionary<string, string> formFields)
	{
		string text = "";
		foreach (KeyValuePair<string, string> formField in formFields)
		{
			if (text.Length > 0)
			{
				text += "&";
			}
			text = text + WWWTranscoder.DataEncode(formField.Key) + "=" + WWWTranscoder.DataEncode(formField.Value);
		}
		return Encoding.UTF8.GetBytes(text);
	}
}
