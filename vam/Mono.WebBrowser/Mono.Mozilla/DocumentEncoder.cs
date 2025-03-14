using System;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Mozilla.DOM;
using Mono.WebBrowser;
using Mono.WebBrowser.DOM;

namespace Mono.Mozilla;

internal class DocumentEncoder : DOMObject
{
	private nsIDocumentEncoder docEncoder;

	private string mimeType;

	private DocumentEncoderFlags flags;

	public string MimeType
	{
		get
		{
			if (mimeType == null)
			{
				mimeType = "text/html";
			}
			return mimeType;
		}
		set
		{
			mimeType = value;
		}
	}

	public DocumentEncoderFlags Flags
	{
		get
		{
			return flags;
		}
		set
		{
			flags = value;
		}
	}

	public DocumentEncoder(WebBrowser control)
		: base(control)
	{
		IntPtr ret = IntPtr.Zero;
		base.control.ServiceManager.getServiceByContractID("@mozilla.org/layout/documentEncoder;1?type=text/html", typeof(nsIDocumentEncoder).GUID, out ret);
		if (ret == IntPtr.Zero)
		{
			throw new Mono.WebBrowser.Exception(Mono.WebBrowser.Exception.ErrorCodes.DocumentEncoderService);
		}
		try
		{
			docEncoder = (nsIDocumentEncoder)Marshal.GetObjectForIUnknown(ret);
		}
		catch (System.Exception)
		{
			throw new Mono.WebBrowser.Exception(Mono.WebBrowser.Exception.ErrorCodes.DocumentEncoderService);
		}
		if (control.platform != control.enginePlatform)
		{
			docEncoder = nsDocumentEncoder.GetProxy(control, docEncoder);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				docEncoder = null;
			}
			disposed = true;
		}
	}

	private void Init(Document document, string mimeType, DocumentEncoderFlags flags)
	{
		UniString uniString = new UniString(mimeType);
		try
		{
			docEncoder.init((nsIDOMDocument)document.nodeNoProxy, uniString.Handle, (uint)flags);
		}
		catch (System.Exception innerException)
		{
			throw new Mono.WebBrowser.Exception(Mono.WebBrowser.Exception.ErrorCodes.DocumentEncoderService, innerException);
		}
	}

	public string EncodeToString(Document document)
	{
		Init(document, MimeType, Flags);
		docEncoder.encodeToString(storage);
		return Base.StringGet(storage);
	}

	public string EncodeToString(HTMLElement element)
	{
		Init((Document)element.Owner, MimeType, Flags);
		docEncoder.setNode(element.nodeNoProxy);
		docEncoder.encodeToString(storage);
		string text = Base.StringGet(storage);
		string tagName = element.TagName;
		string text2 = "<" + tagName;
		string text3;
		foreach (IAttribute attribute in element.Attributes)
		{
			text3 = text2;
			text2 = text3 + " " + attribute.Name + "=\"" + attribute.Value + "\"";
		}
		text3 = text2;
		return text3 + ">" + text + "</" + tagName + ">";
	}

	public System.IO.Stream EncodeToStream(Document document)
	{
		Init(document, MimeType, Flags);
		Stream stream = new Stream(new MemoryStream());
		docEncoder.encodeToStream(stream);
		return stream.BaseStream;
	}

	public System.IO.Stream EncodeToStream(HTMLElement element)
	{
		Init((Document)element.Owner, MimeType, Flags);
		docEncoder.setNode(element.nodeNoProxy);
		Stream stream = new Stream(new MemoryStream());
		docEncoder.encodeToStream(stream);
		return stream.BaseStream;
	}
}
