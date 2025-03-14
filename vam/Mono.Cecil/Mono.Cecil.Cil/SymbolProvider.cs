using System;
using System.IO;
using System.Reflection;

namespace Mono.Cecil.Cil;

internal static class SymbolProvider
{
	private static AssemblyName GetSymbolAssemblyName(SymbolKind kind)
	{
		if (kind == SymbolKind.PortablePdb)
		{
			throw new ArgumentException();
		}
		string symbolNamespace = GetSymbolNamespace(kind);
		AssemblyName name = typeof(SymbolProvider).Assembly().GetName();
		AssemblyName assemblyName = new AssemblyName();
		assemblyName.Name = name.Name + "." + symbolNamespace;
		assemblyName.Version = name.Version;
		assemblyName.CultureInfo = name.CultureInfo;
		assemblyName.SetPublicKeyToken(name.GetPublicKeyToken());
		return assemblyName;
	}

	private static Type GetSymbolType(SymbolKind kind, string fullname)
	{
		Type type = Type.GetType(fullname);
		if (type != null)
		{
			return type;
		}
		AssemblyName symbolAssemblyName = GetSymbolAssemblyName(kind);
		type = Type.GetType(fullname + ", " + symbolAssemblyName.FullName);
		if (type != null)
		{
			return type;
		}
		try
		{
			Assembly assembly = Assembly.Load(symbolAssemblyName);
			if (assembly != null)
			{
				return assembly.GetType(fullname);
			}
		}
		catch (FileNotFoundException)
		{
		}
		catch (FileLoadException)
		{
		}
		return null;
	}

	public static ISymbolReaderProvider GetReaderProvider(SymbolKind kind)
	{
		switch (kind)
		{
		case SymbolKind.PortablePdb:
			return new PortablePdbReaderProvider();
		case SymbolKind.EmbeddedPortablePdb:
			return new EmbeddedPortablePdbReaderProvider();
		default:
		{
			string symbolTypeName = GetSymbolTypeName(kind, "ReaderProvider");
			return (ISymbolReaderProvider)Activator.CreateInstance(GetSymbolType(kind, symbolTypeName) ?? throw new TypeLoadException("Could not find symbol provider type " + symbolTypeName));
		}
		}
	}

	private static string GetSymbolTypeName(SymbolKind kind, string name)
	{
		return string.Concat("Mono.Cecil.", GetSymbolNamespace(kind), ".", kind, name);
	}

	private static string GetSymbolNamespace(SymbolKind kind)
	{
		switch (kind)
		{
		case SymbolKind.PortablePdb:
		case SymbolKind.EmbeddedPortablePdb:
			return "Cil";
		case SymbolKind.NativePdb:
			return "Pdb";
		case SymbolKind.Mdb:
			return "Mdb";
		default:
			throw new ArgumentException();
		}
	}
}
