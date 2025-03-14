using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeChecksumPragma : CodeDirective
{
	private string fileName;

	private Guid checksumAlgorithmId;

	private byte[] checksumData;

	public Guid ChecksumAlgorithmId
	{
		get
		{
			return checksumAlgorithmId;
		}
		set
		{
			checksumAlgorithmId = value;
		}
	}

	public byte[] ChecksumData
	{
		get
		{
			return checksumData;
		}
		set
		{
			checksumData = value;
		}
	}

	public string FileName
	{
		get
		{
			if (fileName == null)
			{
				return string.Empty;
			}
			return fileName;
		}
		set
		{
			fileName = value;
		}
	}

	public CodeChecksumPragma()
	{
	}

	public CodeChecksumPragma(string fileName, Guid checksumAlgorithmId, byte[] checksumData)
	{
		this.fileName = fileName;
		this.checksumAlgorithmId = checksumAlgorithmId;
		this.checksumData = checksumData;
	}
}
