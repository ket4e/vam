using System;

namespace NAudio.Mixer;

public class ListTextMixerControl : MixerControl
{
	internal ListTextMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
	{
		base.mixerControl = mixerControl;
		base.mixerHandle = mixerHandle;
		base.mixerHandleType = mixerHandleType;
		base.nChannels = nChannels;
		mixerControlDetails = default(MixerInterop.MIXERCONTROLDETAILS);
		GetControlDetails();
	}

	protected override void GetDetails(IntPtr pDetails)
	{
	}
}
