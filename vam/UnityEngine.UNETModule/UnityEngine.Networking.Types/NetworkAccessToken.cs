using System;

namespace UnityEngine.Networking.Types;

/// <summary>
///   <para>Access token used to authenticate a client session for the purposes of allowing or disallowing match operations requested by that client.</para>
/// </summary>
public class NetworkAccessToken
{
	private const int NETWORK_ACCESS_TOKEN_SIZE = 64;

	/// <summary>
	///   <para>Binary field for the actual token.</para>
	/// </summary>
	public byte[] array;

	public NetworkAccessToken()
	{
		array = new byte[64];
	}

	public NetworkAccessToken(byte[] array)
	{
		this.array = array;
	}

	public NetworkAccessToken(string strArray)
	{
		try
		{
			array = Convert.FromBase64String(strArray);
		}
		catch (Exception)
		{
			array = new byte[64];
		}
	}

	/// <summary>
	///   <para>Accessor to get an encoded string from the m_array data.</para>
	/// </summary>
	public string GetByteString()
	{
		return Convert.ToBase64String(array);
	}

	/// <summary>
	///   <para>Checks if the token is a valid set of data with respect to default values (returns true if the values are not default, does not validate the token is a current legitimate token with respect to the server's auth framework).</para>
	/// </summary>
	public bool IsValid()
	{
		if (this.array == null || this.array.Length != 64)
		{
			return false;
		}
		bool result = false;
		byte[] array = this.array;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != 0)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
