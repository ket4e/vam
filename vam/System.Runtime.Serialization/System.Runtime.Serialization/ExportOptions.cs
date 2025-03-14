using System.Collections.ObjectModel;

namespace System.Runtime.Serialization;

public class ExportOptions
{
	private IDataContractSurrogate surrogate;

	private KnownTypeCollection known_types;

	public IDataContractSurrogate DataContractSurrogate
	{
		get
		{
			return surrogate;
		}
		set
		{
			surrogate = value;
		}
	}

	public Collection<Type> KnownTypes => known_types;
}
