using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline;

[Serializable]
[TrackClipType(typeof(TrackAsset))]
[TrackMediaType(TimelineAsset.MediaType.Group)]
[SupportsChildTracks(null, int.MaxValue)]
public class GroupTrack : TrackAsset
{
	internal override bool compilable => false;

	public override IEnumerable<PlayableBinding> outputs => PlayableBinding.None;
}
