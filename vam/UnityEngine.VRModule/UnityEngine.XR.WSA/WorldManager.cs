using System;

namespace UnityEngine.XR.WSA;

/// <summary>
///   <para>This class represents the state of the real world tracking system.</para>
/// </summary>
public class WorldManager
{
	[Obsolete("The option for toggling latent frame presentation has been removed, and is on for performance reasons. This property will be removed in a future release.", false)]
	public static bool IsLatentFramePresentation => true;

	[Obsolete("The option for toggling latent frame presentation has been removed, and is on for performance reasons. This method will be removed in a future release.", false)]
	public static void ActivateLatentFramePresentation(bool activated)
	{
	}
}
