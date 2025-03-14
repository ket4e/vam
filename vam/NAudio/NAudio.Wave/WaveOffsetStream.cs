using System;

namespace NAudio.Wave;

public class WaveOffsetStream : WaveStream
{
	private WaveStream sourceStream;

	private long audioStartPosition;

	private long sourceOffsetBytes;

	private long sourceLengthBytes;

	private long length;

	private readonly int bytesPerSample;

	private long position;

	private TimeSpan startTime;

	private TimeSpan sourceOffset;

	private TimeSpan sourceLength;

	private readonly object lockObject = new object();

	public TimeSpan StartTime
	{
		get
		{
			return startTime;
		}
		set
		{
			lock (lockObject)
			{
				startTime = value;
				audioStartPosition = (long)(startTime.TotalSeconds * (double)sourceStream.WaveFormat.SampleRate) * bytesPerSample;
				length = audioStartPosition + sourceLengthBytes;
				Position = Position;
			}
		}
	}

	public TimeSpan SourceOffset
	{
		get
		{
			return sourceOffset;
		}
		set
		{
			lock (lockObject)
			{
				sourceOffset = value;
				sourceOffsetBytes = (long)(sourceOffset.TotalSeconds * (double)sourceStream.WaveFormat.SampleRate) * bytesPerSample;
				Position = Position;
			}
		}
	}

	public TimeSpan SourceLength
	{
		get
		{
			return sourceLength;
		}
		set
		{
			lock (lockObject)
			{
				sourceLength = value;
				sourceLengthBytes = (long)(sourceLength.TotalSeconds * (double)sourceStream.WaveFormat.SampleRate) * bytesPerSample;
				length = audioStartPosition + sourceLengthBytes;
				Position = Position;
			}
		}
	}

	public override int BlockAlign => sourceStream.BlockAlign;

	public override long Length => length;

	public override long Position
	{
		get
		{
			return position;
		}
		set
		{
			lock (lockObject)
			{
				value -= value % BlockAlign;
				if (value < audioStartPosition)
				{
					sourceStream.Position = sourceOffsetBytes;
				}
				else
				{
					sourceStream.Position = sourceOffsetBytes + (value - audioStartPosition);
				}
				position = value;
			}
		}
	}

	public override WaveFormat WaveFormat => sourceStream.WaveFormat;

	public WaveOffsetStream(WaveStream sourceStream, TimeSpan startTime, TimeSpan sourceOffset, TimeSpan sourceLength)
	{
		if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
		{
			throw new ArgumentException("Only PCM supported");
		}
		this.sourceStream = sourceStream;
		position = 0L;
		bytesPerSample = sourceStream.WaveFormat.BitsPerSample / 8 * sourceStream.WaveFormat.Channels;
		StartTime = startTime;
		SourceOffset = sourceOffset;
		SourceLength = sourceLength;
	}

	public WaveOffsetStream(WaveStream sourceStream)
		: this(sourceStream, TimeSpan.Zero, TimeSpan.Zero, sourceStream.TotalTime)
	{
	}

	public override int Read(byte[] destBuffer, int offset, int numBytes)
	{
		lock (lockObject)
		{
			int num = 0;
			if (position < audioStartPosition)
			{
				num = (int)Math.Min(numBytes, audioStartPosition - position);
				for (int i = 0; i < num; i++)
				{
					destBuffer[i + offset] = 0;
				}
			}
			if (num < numBytes)
			{
				int count = (int)Math.Min(numBytes - num, sourceLengthBytes + sourceOffsetBytes - sourceStream.Position);
				int num2 = sourceStream.Read(destBuffer, num + offset, count);
				num += num2;
			}
			for (int j = num; j < numBytes; j++)
			{
				destBuffer[offset + j] = 0;
			}
			position += numBytes;
			return numBytes;
		}
	}

	public override bool HasData(int count)
	{
		if (position + count < audioStartPosition)
		{
			return false;
		}
		if (position >= length)
		{
			return false;
		}
		return sourceStream.HasData(count);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && sourceStream != null)
		{
			sourceStream.Dispose();
			sourceStream = null;
		}
		base.Dispose(disposing);
	}
}
