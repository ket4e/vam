using System.IO;

namespace NAudio.SoundFont;

internal class ModulatorBuilder : StructureBuilder<Modulator>
{
	public override int Length => 10;

	public Modulator[] Modulators => data.ToArray();

	public override Modulator Read(BinaryReader br)
	{
		Modulator modulator = new Modulator();
		modulator.SourceModulationData = new ModulatorType(br.ReadUInt16());
		modulator.DestinationGenerator = (GeneratorEnum)br.ReadUInt16();
		modulator.Amount = br.ReadInt16();
		modulator.SourceModulationAmount = new ModulatorType(br.ReadUInt16());
		modulator.SourceTransform = (TransformEnum)br.ReadUInt16();
		data.Add(modulator);
		return modulator;
	}

	public override void Write(BinaryWriter bw, Modulator o)
	{
	}
}
