using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>Class providing information about XRPlaneSubsystem registration.</para>
/// </summary>
[NativeHeader("Modules/XR/XRPrefix.h")]
[NativeType(Header = "Modules/XR/Subsystems/Planes/XRPlaneSubsystemDescriptor.h")]
[UsedByNativeCode]
public class XRPlaneSubsystemDescriptor : SubsystemDescriptor<XRPlaneSubsystem>
{
}
