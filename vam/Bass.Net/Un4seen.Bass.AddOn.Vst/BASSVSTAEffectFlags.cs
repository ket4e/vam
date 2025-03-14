using System;

namespace Un4seen.Bass.AddOn.Vst;

[Flags]
public enum BASSVSTAEffectFlags
{
	effFlagsHasEditor = 1,
	effFlagsHasClip = 2,
	effFlagsHasVu = 4,
	effFlagsCanMono = 8,
	effFlagsCanReplacing = 0x10,
	effFlagsProgramChunks = 0x20,
	effFlagsIsSynth = 0x100,
	effFlagsNoSoundInStop = 0x200,
	effFlagsExtIsAsync = 0x400,
	effFlagsExtHasBuffer = 0x800,
	effFlagsCanDoubleReplacing = 0x1000
}
