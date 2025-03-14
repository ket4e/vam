using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace UnityEngine.Timeline;

internal class AnimationOutputWeightProcessor : ITimelineEvaluateCallback
{
	private struct WeightInfo
	{
		public Playable mixer;

		public Playable parentMixer;

		public int port;

		public bool modulate;
	}

	private AnimationPlayableOutput m_Output;

	private AnimationLayerMixerPlayable m_LayerMixer;

	private readonly List<WeightInfo> m_Mixers = new List<WeightInfo>();

	public AnimationOutputWeightProcessor(AnimationPlayableOutput output)
	{
		m_Output = output;
		FindMixers();
	}

	private void FindMixers()
	{
		m_Mixers.Clear();
		m_LayerMixer = AnimationLayerMixerPlayable.Null;
		Playable sourcePlayable = m_Output.GetSourcePlayable();
		int sourceInputPort = m_Output.GetSourceInputPort();
		if (!sourcePlayable.IsValid() || sourceInputPort < 0 || sourceInputPort >= sourcePlayable.GetInputCount())
		{
			return;
		}
		Playable input = sourcePlayable.GetInput(sourceInputPort).GetInput(0);
		if (input.IsValid() && input.IsPlayableOfType<AnimationLayerMixerPlayable>())
		{
			m_LayerMixer = (AnimationLayerMixerPlayable)input;
			int inputCount = m_LayerMixer.GetInputCount();
			for (int i = 0; i < inputCount; i++)
			{
				FindMixers(m_LayerMixer, i, m_LayerMixer.GetInput(i));
			}
		}
	}

	private void FindMixers(Playable parent, int port, Playable node)
	{
		if (!node.IsValid())
		{
			return;
		}
		Type playableType = node.GetPlayableType();
		if (playableType == typeof(AnimationMixerPlayable) || playableType == typeof(AnimationLayerMixerPlayable))
		{
			int inputCount = node.GetInputCount();
			for (int i = 0; i < inputCount; i++)
			{
				FindMixers(node, i, node.GetInput(i));
			}
			WeightInfo weightInfo = default(WeightInfo);
			weightInfo.parentMixer = parent;
			weightInfo.mixer = node;
			weightInfo.port = port;
			weightInfo.modulate = playableType == typeof(AnimationLayerMixerPlayable);
			WeightInfo item = weightInfo;
			m_Mixers.Add(item);
		}
		else
		{
			int inputCount2 = node.GetInputCount();
			for (int j = 0; j < inputCount2; j++)
			{
				FindMixers(parent, port, node.GetInput(j));
			}
		}
	}

	public void Evaluate()
	{
		for (int i = 0; i < m_Mixers.Count; i++)
		{
			WeightInfo weightInfo = m_Mixers[i];
			float num = ((!weightInfo.modulate) ? 1f : weightInfo.parentMixer.GetInputWeight(weightInfo.port));
			weightInfo.parentMixer.SetInputWeight(weightInfo.port, num * WeightUtility.NormalizeMixer(weightInfo.mixer));
		}
		m_Output.SetWeight(WeightUtility.NormalizeMixer(m_LayerMixer));
	}
}
