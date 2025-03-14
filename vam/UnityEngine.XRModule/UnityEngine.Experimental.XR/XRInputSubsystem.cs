using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>XRInputSubsystem
/// Instance is used to enable and disable the inputs coming from a specific plugin.</para>
/// </summary>
[UsedByNativeCode]
[NativeType(Header = "Modules/XR/Subsystems/Input/XRInputSubsystem")]
public class XRInputSubsystem : Subsystem<XRInputSubsystemDescriptor>
{
}
