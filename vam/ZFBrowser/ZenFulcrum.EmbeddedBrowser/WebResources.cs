using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public abstract class WebResources
{
	public class ResponsePreamble
	{
		public int statusCode = 200;

		public string statusText;

		public string mimeType = "text/plain; charset=UTF-8";

		public int length = -1;

		public Dictionary<string, string> headers = new Dictionary<string, string>();
	}

	public static readonly Dictionary<string, string> extensionMimeTypes = new Dictionary<string, string>
	{
		{ "css", "text/css" },
		{ "gif", "image/gif" },
		{ "html", "text/html" },
		{ "htm", "text/html" },
		{ "jpeg", "image/jpeg" },
		{ "jpg", "image/jpeg" },
		{ "js", "application/javascript" },
		{ "mp3", "audio/mpeg" },
		{ "mpeg", "video/mpeg" },
		{ "ogg", "application/ogg" },
		{ "ogv", "video/ogg" },
		{ "webm", "video/webm" },
		{ "png", "image/png" },
		{ "svg", "image/svg+xml" },
		{ "txt", "text/plain" },
		{ "xml", "application/xml" },
		{ "*", "application/octet-stream" }
	};

	public static readonly Dictionary<int, string> statusTexts = new Dictionary<int, string>
	{
		{ 100, "Continue" },
		{ 101, "Switching Protocols" },
		{ 200, "OK" },
		{ 201, "Created" },
		{ 202, "Accepted" },
		{ 203, "Non-Authoritative Information" },
		{ 204, "No Content" },
		{ 205, "Reset Content" },
		{ 206, "Partial Content" },
		{ 300, "Multiple Choices" },
		{ 301, "Moved Permanently" },
		{ 302, "Found" },
		{ 303, "See Other" },
		{ 304, "Not Modified" },
		{ 305, "Use Proxy" },
		{ 307, "Temporary Redirect" },
		{ 400, "Bad Request" },
		{ 401, "Unauthorized" },
		{ 402, "Payment Required" },
		{ 403, "Forbidden" },
		{ 404, "Not Found" },
		{ 405, "Method Not Allowed" },
		{ 406, "Not Acceptable" },
		{ 407, "Proxy Authentication Required" },
		{ 408, "Request Timeout" },
		{ 409, "Conflict" },
		{ 410, "Gone" },
		{ 411, "Length Required" },
		{ 412, "Precondition Failed" },
		{ 413, "Request Entity Too Large" },
		{ 414, "Request-URI Too Long" },
		{ 415, "Unsupported Media Type" },
		{ 416, "Requested Range Not Satisfiable" },
		{ 417, "Expectation Failed" },
		{ 500, "Internal Server Error" },
		{ 501, "Not Implemented" },
		{ 502, "Bad Gateway" },
		{ 503, "Service Unavailable" },
		{ 504, "Gateway Timeout" },
		{ 505, "HTTP Version Not Supported" },
		{
			-1,
			string.Empty
		}
	};

	public abstract void HandleRequest(int id, string url);

	protected virtual void SendResponse(int id, byte[] data, string mimeType = "application/octet-stream")
	{
		ResponsePreamble responsePreamble = new ResponsePreamble();
		responsePreamble.headers = null;
		responsePreamble.length = data.Length;
		responsePreamble.mimeType = mimeType;
		responsePreamble.statusCode = 200;
		ResponsePreamble pre = responsePreamble;
		SendPreamble(id, pre);
		SendData(id, data);
		SendEnd(id);
	}

	protected virtual void SendResponse(int id, string text, string mimeType = "text/html; charset=UTF-8")
	{
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		ResponsePreamble responsePreamble = new ResponsePreamble();
		responsePreamble.headers = null;
		responsePreamble.length = bytes.Length;
		responsePreamble.mimeType = mimeType;
		responsePreamble.statusCode = 200;
		ResponsePreamble pre = responsePreamble;
		SendPreamble(id, pre);
		SendData(id, bytes);
		SendEnd(id);
	}

	protected virtual void SendError(int id, string html, int errorCode = 500)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(html);
		ResponsePreamble responsePreamble = new ResponsePreamble();
		responsePreamble.headers = null;
		responsePreamble.length = bytes.Length;
		responsePreamble.mimeType = "text/html; charset=UTF-8";
		responsePreamble.statusCode = errorCode;
		ResponsePreamble pre = responsePreamble;
		SendPreamble(id, pre);
		SendData(id, bytes);
		SendEnd(id);
	}

	protected virtual void SendFile(int id, FileInfo file, bool forceDownload = false)
	{
		new Thread((ThreadStart)delegate
		{
			try
			{
				if (!file.Exists)
				{
					SendError(id, "<h2>File not found</h2>", 404);
				}
				else
				{
					FileStream fileStream = null;
					try
					{
						fileStream = file.OpenRead();
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						SendError(id, "<h2>File unavailable</h2>");
						return;
					}
					string text = file.Extension;
					if (text.Length > 0)
					{
						text = text.Substring(1).ToLowerInvariant();
					}
					if (!extensionMimeTypes.TryGetValue(text, out var value))
					{
						value = extensionMimeTypes["*"];
					}
					ResponsePreamble responsePreamble = new ResponsePreamble
					{
						headers = new Dictionary<string, string>(),
						length = (int)file.Length,
						mimeType = value,
						statusCode = 200
					};
					if (forceDownload)
					{
						responsePreamble.headers["Content-Disposition"] = "attachment; filename=\"" + file.Name.Replace("\"", "\\\"") + "\"";
					}
					SendPreamble(id, responsePreamble);
					int num = -1;
					byte[] array = new byte[Math.Min(responsePreamble.length, 32768)];
					while (num != 0)
					{
						num = fileStream.Read(array, 0, array.Length);
						SendData(id, array, num);
					}
					SendEnd(id);
					fileStream.Close();
				}
			}
			catch (Exception exception2)
			{
				Debug.LogException(exception2);
			}
		}).Start();
	}

	protected void SendPreamble(int id, ResponsePreamble pre)
	{
		JSONNode jSONNode = new JSONNode(JSONNode.NodeType.Object);
		if (pre.headers != null)
		{
			foreach (KeyValuePair<string, string> header in pre.headers)
			{
				jSONNode[header.Key] = header.Value;
			}
		}
		if (pre.statusText == null && !statusTexts.TryGetValue(pre.statusCode, out pre.statusText))
		{
			pre.statusText = statusTexts[-1];
		}
		jSONNode[":status:"] = pre.statusCode.ToString();
		jSONNode[":statusText:"] = pre.statusText;
		jSONNode["Content-Type"] = pre.mimeType;
		lock (BrowserNative.symbolsLock)
		{
			BrowserNative.zfb_sendRequestHeaders(id, pre.length, jSONNode.AsJSON);
		}
	}

	protected void SendData(int id, byte[] data, int length = -1)
	{
		if (data != null && data.Length != 0 && length != 0)
		{
			if (length < 0)
			{
				length = data.Length;
			}
			if (length > data.Length)
			{
				throw new IndexOutOfRangeException();
			}
			GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
			lock (BrowserNative.symbolsLock)
			{
				BrowserNative.zfb_sendRequestData(id, gCHandle.AddrOfPinnedObject(), length);
			}
			gCHandle.Free();
		}
	}

	protected void SendEnd(int id)
	{
		lock (BrowserNative.symbolsLock)
		{
			BrowserNative.zfb_sendRequestData(id, IntPtr.Zero, 0);
		}
	}
}
