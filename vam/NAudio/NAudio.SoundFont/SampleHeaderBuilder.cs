using System.IO;
using NAudio.Utils;

namespace NAudio.SoundFont;

internal class SampleHeaderBuilder : StructureBuilder<SampleHeader>
{
	public override int Length => 46;

	public SampleHeader[] SampleHeaders => data.ToArray();

	public override SampleHeader Read(BinaryReader br)
	{
		SampleHeader sampleHeader = new SampleHeader();
		byte[] array = br.ReadBytes(20);
		sampleHeader.SampleName = ByteEncoding.Instance.GetString(array, 0, array.Length);
		sampleHeader.Start = br.ReadUInt32();
		sampleHeader.End = br.ReadUInt32();
		sampleHeader.StartLoop = br.ReadUInt32();
		sampleHeader.EndLoop = br.ReadUInt32();
		sampleHeader.SampleRate = br.ReadUInt32();
		sampleHeader.OriginalPitch = br.ReadByte();
		sampleHeader.PitchCorrection = br.ReadSByte();
		sampleHeader.SampleLink = br.ReadUInt16();
		sampleHeader.SFSampleLink = (SFSampleLink)br.ReadUInt16();
		data.Add(sampleHeader);
		return sampleHeader;
	}

	public override void Write(BinaryWriter bw, SampleHeader sampleHeader)
	{
	}

	internal void RemoveEOS()
	{
		data.RemoveAt(data.Count - 1);
	}
}
