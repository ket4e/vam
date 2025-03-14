using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>Class providing information about XRSessionSubsystem registration.</para>
/// </summary>
[NativeType(Header = "Modules/XR/Subsystems/Session/XRSessionSubsystemDescriptor.h")]
[NativeHeader("Modules/XR/XRPrefix.h")]
[UsedByNativeCode]
public class XRSessionSubsystemDescriptor : SubsystemDescriptor<XRSessionSubsystem>
{
}
