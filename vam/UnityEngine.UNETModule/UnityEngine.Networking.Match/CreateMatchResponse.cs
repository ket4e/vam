using System;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match;

internal class CreateMatchResponse : BasicResponse
{
	public string address { get; set; }

	public int port { get; set; }

	public int domain { get; set; }

	public NetworkID networkId { get; set; }

	public string accessTokenString { get; set; }

	public NodeID nodeId { get; set; }

	public bool usingRelay { get; set; }

	public override string ToString()
	{
		return UnityString.Format("[{0}]-address:{1},port:{2},networkId:0x{3},accessTokenString.IsEmpty:{4},nodeId:0x{5},usingRelay:{6}", base.ToString(), address, port, networkId.ToString("X"), string.IsNullOrEmpty(accessTokenString), nodeId.ToString("X"), usingRelay);
	}

	public override void Parse(object obj)
	{
		base.Parse(obj);
		if (obj is IDictionary<string, object> dictJsonObj)
		{
			address = ParseJSONString("address", obj, dictJsonObj);
			port = ParseJSONInt32("port", obj, dictJsonObj);
			networkId = (NetworkID)ParseJSONUInt64("networkId", obj, dictJsonObj);
			accessTokenString = ParseJSONString("accessTokenString", obj, dictJsonObj);
			nodeId = (NodeID)ParseJSONUInt16("nodeId", obj, dictJsonObj);
			usingRelay = ParseJSONBool("usingRelay", obj, dictJsonObj);
			return;
		}
		throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
	}
}
