using System;
using System.Collections.Generic;
using System.Linq;

namespace MHLab.PATCH;

public sealed class Version : IComparable
{
	private int m_majorReleaseNumber;

	private int m_minorReleaseNumber;

	private int m_maintenanceReleaseNumber;

	private int m_buildNumber;

	public int Major => m_majorReleaseNumber;

	public int Minor => m_minorReleaseNumber;

	public int Revision => m_maintenanceReleaseNumber;

	public int Build => m_buildNumber;

	public Version()
	{
		m_majorReleaseNumber = 0;
		m_minorReleaseNumber = 0;
		m_maintenanceReleaseNumber = 0;
		m_buildNumber = 0;
	}

	public Version(string version)
	{
		Version buildNumber = null;
		TryParse(version, out buildNumber);
		if (buildNumber != null)
		{
			m_majorReleaseNumber = buildNumber.m_majorReleaseNumber;
			m_minorReleaseNumber = buildNumber.m_minorReleaseNumber;
			m_maintenanceReleaseNumber = buildNumber.m_maintenanceReleaseNumber;
			m_buildNumber = buildNumber.m_buildNumber;
			return;
		}
		throw new ArgumentException("Version string isn't valid");
	}

	public static bool TryParse(string input, out Version buildNumber)
	{
		try
		{
			buildNumber = Parse(input);
			return true;
		}
		catch
		{
			buildNumber = null;
			return false;
		}
	}

	public static Version Parse(string buildNumber)
	{
		if (buildNumber == null)
		{
			throw new ArgumentNullException("buildNumber");
		}
		List<string> list = (from v in buildNumber.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries)
			select v.Trim()).ToList();
		if (list.Count < 2)
		{
			throw new ArgumentException("BuildNumber string was too short");
		}
		if (list.Count > 4)
		{
			throw new ArgumentException("BuildNumber string was too long");
		}
		return new Version
		{
			m_majorReleaseNumber = ParseVersion(list[0]),
			m_minorReleaseNumber = ParseVersion(list[1]),
			m_maintenanceReleaseNumber = ((list.Count > 2) ? ParseVersion(list[2]) : (-1)),
			m_buildNumber = ((list.Count > 3) ? ParseVersion(list[3]) : (-1))
		};
	}

	private static int ParseVersion(string input)
	{
		if (!int.TryParse(input, out var result))
		{
			throw new FormatException("Version string was not in a correct format");
		}
		if (result < 0)
		{
			throw new ArgumentOutOfRangeException("Version", "Versions must be greater than or equal to zero");
		}
		return result;
	}

	public override string ToString()
	{
		return string.Format("{0}.{1}{2}{3}", m_majorReleaseNumber, m_minorReleaseNumber, (m_maintenanceReleaseNumber < 0) ? "" : ("." + m_maintenanceReleaseNumber), (m_buildNumber < 0) ? "" : ("." + m_buildNumber));
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (!(obj is Version version))
		{
			return 1;
		}
		if (this == version)
		{
			return 0;
		}
		if (m_majorReleaseNumber != version.m_majorReleaseNumber)
		{
			return m_majorReleaseNumber.CompareTo(version.m_majorReleaseNumber);
		}
		if (m_minorReleaseNumber != version.m_minorReleaseNumber)
		{
			return m_minorReleaseNumber.CompareTo(version.m_minorReleaseNumber);
		}
		if (m_maintenanceReleaseNumber != version.m_maintenanceReleaseNumber)
		{
			return m_maintenanceReleaseNumber.CompareTo(version.m_maintenanceReleaseNumber);
		}
		return m_buildNumber.CompareTo(version.m_buildNumber);
	}

	public static bool operator >(Version first, Version second)
	{
		return first.CompareTo(second) > 0;
	}

	public static bool operator <(Version first, Version second)
	{
		return first.CompareTo(second) < 0;
	}

	public override bool Equals(object obj)
	{
		return CompareTo(obj) == 0;
	}

	public override int GetHashCode()
	{
		return (((17 * 23 + m_majorReleaseNumber.GetHashCode()) * 23 + m_minorReleaseNumber.GetHashCode()) * 23 + m_maintenanceReleaseNumber.GetHashCode()) * 23 + m_buildNumber.GetHashCode();
	}
}
