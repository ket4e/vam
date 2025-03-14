using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.Threading;

[Serializable]
public sealed class CompressedStack : ISerializable
{
	private ArrayList _list;

	internal IList List => _list;

	internal CompressedStack(int length)
	{
		if (length > 0)
		{
			_list = new ArrayList(length);
		}
	}

	internal CompressedStack(CompressedStack cs)
	{
		if (cs != null && cs._list != null)
		{
			_list = (ArrayList)cs._list.Clone();
		}
	}

	[ComVisible(false)]
	public CompressedStack CreateCopy()
	{
		return new CompressedStack(this);
	}

	public static CompressedStack Capture()
	{
		CompressedStack compressedStack = new CompressedStack(0);
		compressedStack._list = SecurityFrame.GetStack(1);
		CompressedStack compressedStack2 = Thread.CurrentThread.GetCompressedStack();
		if (compressedStack2 != null)
		{
			for (int i = 0; i < compressedStack2._list.Count; i++)
			{
				compressedStack._list.Add(compressedStack2._list[i]);
			}
		}
		return compressedStack;
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n   <IPermission class=\"System.Security.Permissions.StrongNameIdentityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                PublicKeyBlob=\"00000000000000000400000000000000\"/>\n</PermissionSet>\n")]
	public static CompressedStack GetCompressedStack()
	{
		CompressedStack compressedStack = Thread.CurrentThread.GetCompressedStack();
		if (compressedStack == null)
		{
			compressedStack = Capture();
		}
		else
		{
			CompressedStack compressedStack2 = Capture();
			for (int i = 0; i < compressedStack2._list.Count; i++)
			{
				compressedStack._list.Add(compressedStack2._list[i]);
			}
		}
		return compressedStack;
	}

	[MonoTODO("incomplete")]
	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"MemberAccess\"/>\n</PermissionSet>\n")]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
	public static void Run(CompressedStack compressedStack, ContextCallback callback, object state)
	{
		if (compressedStack == null)
		{
			throw new ArgumentException("compressedStack");
		}
		Thread currentThread = Thread.CurrentThread;
		CompressedStack compressedStack2 = null;
		try
		{
			compressedStack2 = currentThread.GetCompressedStack();
			currentThread.SetCompressedStack(compressedStack);
			callback(state);
		}
		finally
		{
			if (compressedStack2 != null)
			{
				currentThread.SetCompressedStack(compressedStack2);
			}
		}
	}

	internal bool Equals(CompressedStack cs)
	{
		if (IsEmpty())
		{
			return cs.IsEmpty();
		}
		if (cs.IsEmpty())
		{
			return false;
		}
		if (_list.Count != cs._list.Count)
		{
			return false;
		}
		for (int i = 0; i < _list.Count; i++)
		{
			SecurityFrame securityFrame = (SecurityFrame)_list[i];
			SecurityFrame sf = (SecurityFrame)cs._list[i];
			if (!securityFrame.Equals(sf))
			{
				return false;
			}
		}
		return true;
	}

	internal bool IsEmpty()
	{
		return _list == null || _list.Count == 0;
	}
}
