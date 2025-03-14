using System;
using System.Collections.Generic;
using System.IO;

namespace IKVM.Reflection;

public abstract class Assembly : ICustomAttributeProvider
{
	internal readonly Universe universe;

	protected string fullName;

	protected List<ModuleResolveEventHandler> resolvers;

	public abstract string ImageRuntimeVersion { get; }

	public abstract Module ManifestModule { get; }

	public abstract MethodInfo EntryPoint { get; }

	public abstract string Location { get; }

	public string FullName => fullName ?? (fullName = GetName().FullName);

	public IEnumerable<Module> Modules => GetLoadedModules();

	public bool ReflectionOnly => true;

	public IEnumerable<Type> ExportedTypes => GetExportedTypes();

	public IEnumerable<TypeInfo> DefinedTypes
	{
		get
		{
			Type[] types = GetTypes();
			TypeInfo[] array = new TypeInfo[types.Length];
			for (int i = 0; i < types.Length; i++)
			{
				array[i] = types[i].GetTypeInfo();
			}
			return array;
		}
	}

	public IEnumerable<CustomAttributeData> CustomAttributes => GetCustomAttributesData();

	public string CodeBase
	{
		get
		{
			string text = Location.Replace(Path.DirectorySeparatorChar, '/');
			if (!text.StartsWith("/"))
			{
				text = "/" + text;
			}
			return "file://" + text;
		}
	}

	public virtual bool IsDynamic => false;

	public virtual bool __IsMissing => false;

	public AssemblyNameFlags __AssemblyFlags => GetAssemblyFlags();

	public event ModuleResolveEventHandler ModuleResolve
	{
		add
		{
			if (resolvers == null)
			{
				resolvers = new List<ModuleResolveEventHandler>();
			}
			resolvers.Add(value);
		}
		remove
		{
			resolvers.Remove(value);
		}
	}

	internal Assembly(Universe universe)
	{
		this.universe = universe;
	}

	public sealed override string ToString()
	{
		return FullName;
	}

	public abstract Type[] GetTypes();

	public abstract AssemblyName GetName();

	public abstract AssemblyName[] GetReferencedAssemblies();

	public abstract Module[] GetModules(bool getResourceModules);

	public abstract Module[] GetLoadedModules(bool getResourceModules);

	public abstract Module GetModule(string name);

	public abstract string[] GetManifestResourceNames();

	public abstract ManifestResourceInfo GetManifestResourceInfo(string resourceName);

	public abstract Stream GetManifestResourceStream(string name);

	internal abstract Type FindType(TypeName name);

	internal abstract Type FindTypeIgnoreCase(TypeName lowerCaseName);

	internal Type ResolveType(Module requester, TypeName typeName)
	{
		return FindType(typeName) ?? universe.GetMissingTypeOrThrow(requester, ManifestModule, null, typeName);
	}

	public Module[] GetModules()
	{
		return GetModules(getResourceModules: true);
	}

	public Module[] GetLoadedModules()
	{
		return GetLoadedModules(getResourceModules: true);
	}

	public AssemblyName GetName(bool copiedName)
	{
		return GetName();
	}

	public Type[] GetExportedTypes()
	{
		List<Type> list = new List<Type>();
		Type[] types = GetTypes();
		foreach (Type type in types)
		{
			if (type.IsVisible)
			{
				list.Add(type);
			}
		}
		return list.ToArray();
	}

	public Type GetType(string name)
	{
		return GetType(name, throwOnError: false);
	}

	public Type GetType(string name, bool throwOnError)
	{
		return GetType(name, throwOnError, ignoreCase: false);
	}

	public Type GetType(string name, bool throwOnError, bool ignoreCase)
	{
		TypeNameParser typeNameParser = TypeNameParser.Parse(name, throwOnError);
		if (typeNameParser.Error)
		{
			return null;
		}
		if (typeNameParser.AssemblyName != null)
		{
			if (throwOnError)
			{
				throw new ArgumentException("Type names passed to Assembly.GetType() must not specify an assembly.");
			}
			return null;
		}
		TypeName name2 = TypeName.Split(TypeNameParser.Unescape(typeNameParser.FirstNamePart));
		Type type = (ignoreCase ? FindTypeIgnoreCase(name2.ToLowerInvariant()) : FindType(name2));
		if (type == null && __IsMissing)
		{
			throw new MissingAssemblyException((MissingAssembly)this);
		}
		return typeNameParser.Expand(type, ManifestModule, throwOnError, name, resolve: false, ignoreCase);
	}

	public virtual Module LoadModule(string moduleName, byte[] rawModule)
	{
		throw new NotSupportedException();
	}

	public Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
	{
		return LoadModule(moduleName, rawModule);
	}

	public bool IsDefined(Type attributeType, bool inherit)
	{
		return CustomAttributeData.__GetCustomAttributes(this, attributeType, inherit).Count != 0;
	}

	public IList<CustomAttributeData> __GetCustomAttributes(Type attributeType, bool inherit)
	{
		return CustomAttributeData.__GetCustomAttributes(this, attributeType, inherit);
	}

	public IList<CustomAttributeData> GetCustomAttributesData()
	{
		return CustomAttributeData.GetCustomAttributes(this);
	}

	public static string CreateQualifiedName(string assemblyName, string typeName)
	{
		return typeName + ", " + assemblyName;
	}

	public static Assembly GetAssembly(Type type)
	{
		return type.Assembly;
	}

	protected virtual AssemblyNameFlags GetAssemblyFlags()
	{
		return GetName().Flags;
	}

	internal abstract IList<CustomAttributeData> GetCustomAttributesData(Type attributeType);
}
