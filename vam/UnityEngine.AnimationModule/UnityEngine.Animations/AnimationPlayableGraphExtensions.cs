using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;

namespace UnityEngine.Animations;

[NativeHeader("Runtime/Animation/ScriptBindings/AnimationPlayableGraphExtensions.bindings.h")]
[NativeHeader("Runtime/Animation/Animator.h")]
[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[StaticAccessor("AnimationPlayableGraphExtensionsBindings", StaticAccessorType.DoubleColon)]
internal static class AnimationPlayableGraphExtensions
{
	internal static void SyncUpdateAndTimeMode(this PlayableGraph graph, Animator animator)
	{
		InternalSyncUpdateAndTimeMode(ref graph, animator);
	}

	internal static void DestroyOutput(this PlayableGraph graph, PlayableOutputHandle handle)
	{
		InternalDestroyOutput(ref graph, ref handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool InternalCreateAnimationOutput(ref PlayableGraph graph, string name, out PlayableOutputHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void InternalSyncUpdateAndTimeMode(ref PlayableGraph graph, Animator animator);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalDestroyOutput(ref PlayableGraph graph, ref PlayableOutputHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int InternalAnimationOutputCount(ref PlayableGraph graph);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool InternalGetAnimationOutput(ref PlayableGraph graph, int index, out PlayableOutputHandle handle);
}
