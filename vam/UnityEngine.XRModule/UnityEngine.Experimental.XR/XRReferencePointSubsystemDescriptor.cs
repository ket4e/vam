using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>Class providing information about XRReferencePointSubsystem registration.</para>
/// </summary>
[NativeType(Header = "Modules/XR/Subsystems/ReferencePoints/XRReferencePointSubsystemDescriptor.h")]
[NativeHeader("Modules/XR/XRPrefix.h")]
[UsedByNativeCode]
public class XRReferencePointSubsystemDescriptor : SubsystemDescriptor<XRReferencePointSubsystem>
{
}
