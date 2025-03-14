using System;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match;

internal class MatchDirectConnectInfo : ResponseBase
{
	public NodeID nodeId { get; set; }

	public string publicAddress { get; set; }

	public string privateAddress { get; set; }

	public HostPriority hostPriority { get; set; }

	public override string ToString()
	{
		return UnityString.Format("[{0}]-nodeId:{1},publicAddress:{2},privateAddress:{3},hostPriority:{4}", base.ToString(), nodeId, publicAddress, privateAddress, hostPriority);
	}

	public override void Parse(object obj)
	{
		if (obj is IDictionary<string, object> dictJsonObj)
		{
			nodeId = (NodeID)ParseJSONUInt16("nodeId", obj, dictJsonObj);
			publicAddress = ParseJSONString("publicAddress", obj, dictJsonObj);
			privateAddress = ParseJSONString("privateAddress", obj, dictJsonObj);
			hostPriority = (HostPriority)ParseJSONInt32("hostPriority", obj, dictJsonObj);
			return;
		}
		throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
	}
}
