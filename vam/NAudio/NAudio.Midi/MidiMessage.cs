using System;

namespace NAudio.Midi;

public class MidiMessage
{
	private int rawData;

	public int RawData => rawData;

	public MidiMessage(int status, int data1, int data2)
	{
		rawData = status + (data1 << 8) + (data2 << 16);
	}

	public MidiMessage(int rawData)
	{
		this.rawData = rawData;
	}

	public static MidiMessage StartNote(int note, int volume, int channel)
	{
		ValidateNoteParameters(note, volume, channel);
		return new MidiMessage(144 + channel - 1, note, volume);
	}

	private static void ValidateNoteParameters(int note, int volume, int channel)
	{
		ValidateChannel(channel);
		if (note < 0 || note > 127)
		{
			throw new ArgumentOutOfRangeException("note", "Note number must be in the range 0-127");
		}
		if (volume < 0 || volume > 127)
		{
			throw new ArgumentOutOfRangeException("volume", "Velocity must be in the range 0-127");
		}
	}

	private static void ValidateChannel(int channel)
	{
		if (channel < 1 || channel > 16)
		{
			throw new ArgumentOutOfRangeException("channel", channel, $"Channel must be 1-16 (Got {channel})");
		}
	}

	public static MidiMessage StopNote(int note, int volume, int channel)
	{
		ValidateNoteParameters(note, volume, channel);
		return new MidiMessage(128 + channel - 1, note, volume);
	}

	public static MidiMessage ChangePatch(int patch, int channel)
	{
		ValidateChannel(channel);
		return new MidiMessage(192 + channel - 1, patch, 0);
	}

	public static MidiMessage ChangeControl(int controller, int value, int channel)
	{
		ValidateChannel(channel);
		return new MidiMessage(176 + channel - 1, controller, value);
	}
}
