namespace IKVM.Reflection;

public enum AssemblyComparisonResult
{
	Unknown,
	EquivalentFullMatch,
	EquivalentWeakNamed,
	EquivalentFXUnified,
	EquivalentUnified,
	NonEquivalentVersion,
	NonEquivalent,
	EquivalentPartialMatch,
	EquivalentPartialWeakNamed,
	EquivalentPartialUnified,
	EquivalentPartialFXUnified,
	NonEquivalentPartialVersion
}
