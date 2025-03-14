using System;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match;

internal class DropConnectionResponse : Response
{
	public NetworkID networkId { get; set; }

	public NodeID nodeId { get; set; }

	public override string ToString()
	{
		return UnityString.Format("[{0}]-networkId:{1}", base.ToString(), networkId.ToString("X"));
	}

	public override void Parse(object obj)
	{
		base.Parse(obj);
		if (obj is IDictionary<string, object> dictJsonObj)
		{
			networkId = (NetworkID)ParseJSONUInt64("networkId", obj, dictJsonObj);
			nodeId = (NodeID)ParseJSONUInt16("nodeId", obj, dictJsonObj);
			return;
		}
		throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
	}
}
