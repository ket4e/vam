using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace System.Configuration;

public sealed class RsaProtectedConfigurationProvider : ProtectedConfigurationProvider
{
	private string cspProviderName;

	private string keyContainerName;

	private bool useMachineContainer;

	private bool useOAEP;

	private RSACryptoServiceProvider rsa;

	public string CspProviderName => cspProviderName;

	public string KeyContainerName => keyContainerName;

	public RSAParameters RsaPublicKey
	{
		get
		{
			RSACryptoServiceProvider provider = GetProvider();
			return provider.ExportParameters(includePrivateParameters: false);
		}
	}

	public bool UseMachineContainer => useMachineContainer;

	public bool UseOAEP => useOAEP;

	private RSACryptoServiceProvider GetProvider()
	{
		if (rsa == null)
		{
			CspParameters cspParameters = new CspParameters();
			cspParameters.ProviderName = cspProviderName;
			cspParameters.KeyContainerName = keyContainerName;
			if (useMachineContainer)
			{
				cspParameters.Flags |= CspProviderFlags.UseMachineKeyStore;
			}
			rsa = new RSACryptoServiceProvider(cspParameters);
		}
		return rsa;
	}

	[System.MonoTODO]
	public override XmlNode Decrypt(XmlNode encrypted_node)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(new StringReader(encrypted_node.OuterXml));
		EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
		encryptedXml.AddKeyNameMapping("Rsa Key", GetProvider());
		encryptedXml.DecryptDocument();
		return xmlDocument.DocumentElement;
	}

	[System.MonoTODO]
	public override XmlNode Encrypt(XmlNode node)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(new StringReader(node.OuterXml));
		EncryptedXml encryptedXml = new EncryptedXml(xmlDocument);
		encryptedXml.AddKeyNameMapping("Rsa Key", GetProvider());
		EncryptedData encryptedData = encryptedXml.Encrypt(xmlDocument.DocumentElement, "Rsa Key");
		return encryptedData.GetXml();
	}

	[System.MonoTODO]
	public override void Initialize(string name, NameValueCollection configurationValues)
	{
		base.Initialize(name, configurationValues);
		keyContainerName = configurationValues["keyContainerName"];
		cspProviderName = configurationValues["cspProviderName"];
		string text = configurationValues["useMachineContainer"];
		if (text != null && text.ToLower() == "true")
		{
			useMachineContainer = true;
		}
		text = configurationValues["useOAEP"];
		if (text != null && text.ToLower() == "true")
		{
			useOAEP = true;
		}
	}

	[System.MonoTODO]
	public void AddKey(int keySize, bool exportable)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void DeleteKey()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void ExportKey(string xmlFileName, bool includePrivateParameters)
	{
		RSACryptoServiceProvider provider = GetProvider();
		string value = provider.ToXmlString(includePrivateParameters);
		FileStream stream = new FileStream(xmlFileName, FileMode.OpenOrCreate, FileAccess.Write);
		StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.Write(value);
		streamWriter.Close();
	}

	[System.MonoTODO]
	public void ImportKey(string xmlFileName, bool exportable)
	{
		throw new NotImplementedException();
	}
}
