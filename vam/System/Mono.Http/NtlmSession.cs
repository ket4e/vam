using System;
using System.Net;
using Mono.Security.Protocol.Ntlm;

namespace Mono.Http;

internal class NtlmSession
{
	private MessageBase message;

	public Authorization Authenticate(string challenge, WebRequest webRequest, ICredentials credentials)
	{
		if (!(webRequest is HttpWebRequest httpWebRequest))
		{
			return null;
		}
		NetworkCredential credential = credentials.GetCredential(httpWebRequest.RequestUri, "NTLM");
		if (credential == null)
		{
			return null;
		}
		string userName = credential.UserName;
		string domain = credential.Domain;
		string text = credential.Password;
		if (userName == null || userName == string.Empty)
		{
			return null;
		}
		domain = ((domain == null || domain.Length <= 0) ? httpWebRequest.Headers["Host"] : domain);
		bool complete = false;
		if (message == null)
		{
			Type1Message type1Message = new Type1Message();
			type1Message.Domain = domain;
			message = type1Message;
		}
		else if (message.Type == 1)
		{
			if (challenge == null)
			{
				message = null;
				return null;
			}
			Type2Message type2Message = new Type2Message(Convert.FromBase64String(challenge));
			if (text == null)
			{
				text = string.Empty;
			}
			Type3Message type3Message = new Type3Message();
			type3Message.Domain = domain;
			type3Message.Username = userName;
			type3Message.Challenge = type2Message.Nonce;
			type3Message.Password = text;
			message = type3Message;
			complete = true;
		}
		else if (challenge == null || challenge == string.Empty)
		{
			Type1Message type1Message2 = new Type1Message();
			type1Message2.Domain = domain;
			message = type1Message2;
		}
		else
		{
			complete = true;
		}
		string token = "NTLM " + Convert.ToBase64String(message.GetBytes());
		return new Authorization(token, complete);
	}
}
