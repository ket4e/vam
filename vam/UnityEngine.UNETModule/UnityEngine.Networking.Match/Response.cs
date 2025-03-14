using System;
using System.Collections.Generic;

namespace UnityEngine.Networking.Match;

internal abstract class Response : ResponseBase, IResponse
{
	public bool success { get; private set; }

	public string extendedInfo { get; private set; }

	public void SetSuccess()
	{
		success = true;
		extendedInfo = "";
	}

	public void SetFailure(string info)
	{
		success = false;
		extendedInfo += info;
	}

	public override string ToString()
	{
		return UnityString.Format("[{0}]-success:{1}-extendedInfo:{2}", base.ToString(), success, extendedInfo);
	}

	public override void Parse(object obj)
	{
		if (obj is IDictionary<string, object> dictJsonObj)
		{
			success = ParseJSONBool("success", obj, dictJsonObj);
			extendedInfo = ParseJSONString("extendedInfo", obj, dictJsonObj);
			if (!success)
			{
				throw new FormatException("FAILURE Returned from server: " + extendedInfo);
			}
		}
	}
}
