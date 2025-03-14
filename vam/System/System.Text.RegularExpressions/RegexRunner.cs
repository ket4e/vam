using System.ComponentModel;

namespace System.Text.RegularExpressions;

[EditorBrowsable(EditorBrowsableState.Never)]
[System.MonoTODO("RegexRunner is not supported by Mono.")]
public abstract class RegexRunner
{
	[System.MonoTODO]
	protected internal int[] runcrawl;

	[System.MonoTODO]
	protected internal int runcrawlpos;

	[System.MonoTODO]
	protected internal Match runmatch;

	[System.MonoTODO]
	protected internal Regex runregex;

	[System.MonoTODO]
	protected internal int[] runstack;

	[System.MonoTODO]
	protected internal int runstackpos;

	[System.MonoTODO]
	protected internal string runtext;

	[System.MonoTODO]
	protected internal int runtextbeg;

	[System.MonoTODO]
	protected internal int runtextend;

	[System.MonoTODO]
	protected internal int runtextpos;

	[System.MonoTODO]
	protected internal int runtextstart;

	[System.MonoTODO]
	protected internal int[] runtrack;

	[System.MonoTODO]
	protected internal int runtrackcount;

	[System.MonoTODO]
	protected internal int runtrackpos;

	[System.MonoTODO]
	protected internal RegexRunner()
	{
	}

	protected abstract bool FindFirstChar();

	protected abstract void Go();

	protected abstract void InitTrackCount();

	[System.MonoTODO]
	protected void Capture(int capnum, int start, int end)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected static bool CharInClass(char ch, string charClass)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected static bool CharInSet(char ch, string set, string category)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected void Crawl(int i)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected int Crawlpos()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected void DoubleCrawl()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected void DoubleStack()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected void DoubleTrack()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected void EnsureStorage()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected bool IsBoundary(int index, int startpos, int endpos)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected bool IsECMABoundary(int index, int startpos, int endpos)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected bool IsMatched(int cap)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected int MatchIndex(int cap)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected int MatchLength(int cap)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected int Popcrawl()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected void TransferCapture(int capnum, int uncapnum, int start, int end)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected void Uncapture()
	{
		throw new NotImplementedException();
	}

	protected internal Match Scan(Regex regex, string text, int textbeg, int textend, int textstart, int prevlen, bool quick)
	{
		throw new NotImplementedException();
	}
}
