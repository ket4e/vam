using System;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match;

internal class MatchDesc : ResponseBase
{
	public NetworkID networkId { get; set; }

	public string name { get; set; }

	public int averageEloScore { get; set; }

	public int maxSize { get; set; }

	public int currentSize { get; set; }

	public bool isPrivate { get; set; }

	public Dictionary<string, long> matchAttributes { get; set; }

	public NodeID hostNodeId { get; set; }

	public List<MatchDirectConnectInfo> directConnectInfos { get; set; }

	public override string ToString()
	{
		return UnityString.Format("[{0}]-networkId:0x{1},name:{2},averageEloScore:{3},maxSize:{4},currentSize:{5},isPrivate:{6},matchAttributes.Count:{7},hostNodeId:{8},directConnectInfos.Count:{9}", base.ToString(), networkId.ToString("X"), name, averageEloScore, maxSize, currentSize, isPrivate, (matchAttributes != null) ? matchAttributes.Count : 0, hostNodeId, directConnectInfos.Count);
	}

	public override void Parse(object obj)
	{
		if (obj is IDictionary<string, object> dictJsonObj)
		{
			networkId = (NetworkID)ParseJSONUInt64("networkId", obj, dictJsonObj);
			name = ParseJSONString("name", obj, dictJsonObj);
			averageEloScore = ParseJSONInt32("averageEloScore", obj, dictJsonObj);
			maxSize = ParseJSONInt32("maxSize", obj, dictJsonObj);
			currentSize = ParseJSONInt32("currentSize", obj, dictJsonObj);
			isPrivate = ParseJSONBool("isPrivate", obj, dictJsonObj);
			hostNodeId = (NodeID)ParseJSONUInt16("hostNodeId", obj, dictJsonObj);
			directConnectInfos = ParseJSONList<MatchDirectConnectInfo>("directConnectInfos", obj, dictJsonObj);
			return;
		}
		throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
	}
}
