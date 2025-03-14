using System.Xml;

namespace System.Security.Cryptography.Xml;

public class XmlLicenseTransform : Transform
{
	private IRelDecryptor _decryptor;

	private Type[] inputTypes;

	private Type[] outputTypes;

	public IRelDecryptor Decryptor
	{
		get
		{
			return _decryptor;
		}
		set
		{
			_decryptor = value;
		}
	}

	public override Type[] InputTypes
	{
		get
		{
			if (inputTypes == null)
			{
				inputTypes = new Type[1] { typeof(XmlDocument) };
			}
			return inputTypes;
		}
	}

	public override Type[] OutputTypes
	{
		get
		{
			if (outputTypes == null)
			{
				outputTypes = new Type[1] { typeof(XmlDocument) };
			}
			return outputTypes;
		}
	}

	public XmlLicenseTransform()
	{
		base.Algorithm = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";
	}

	[System.MonoTODO]
	protected override XmlNodeList GetInnerXml()
	{
		return null;
	}

	[System.MonoTODO]
	public override object GetOutput()
	{
		return null;
	}

	public override object GetOutput(Type type)
	{
		if (type != typeof(XmlDocument))
		{
			throw new ArgumentException("type");
		}
		return GetOutput();
	}

	public override void LoadInnerXml(XmlNodeList nodeList)
	{
	}

	[System.MonoTODO]
	public override void LoadInput(object obj)
	{
		if (obj != typeof(XmlDocument))
		{
			throw new ArgumentException("obj");
		}
		if (_decryptor == null)
		{
			throw new CryptographicException(global::Locale.GetText("missing decryptor"));
		}
	}
}
