using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline;

[Serializable]
[TrackClipType(typeof(IPlayableAsset))]
[TrackMediaType(TimelineAsset.MediaType.Script)]
public class PlayableTrack : TrackAsset
{
	internal override void OnCreateClipFromAsset(Object asset, TimelineClip newClip)
	{
		base.OnCreateClipFromAsset(asset, newClip);
		if (newClip != null)
		{
			newClip.displayName = asset.GetType().Name;
		}
	}
}
