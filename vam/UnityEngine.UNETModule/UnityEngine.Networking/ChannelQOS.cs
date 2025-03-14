using System;

namespace UnityEngine.Networking;

/// <summary>
///   <para>Defines parameters of channels.</para>
/// </summary>
[Serializable]
public class ChannelQOS
{
	[SerializeField]
	internal QosType m_Type;

	[SerializeField]
	internal bool m_BelongsSharedOrderChannel;

	/// <summary>
	///   <para>Channel quality of service.</para>
	/// </summary>
	public QosType QOS => m_Type;

	/// <summary>
	///   <para>Returns true if the channel belongs to a shared group.</para>
	/// </summary>
	public bool BelongsToSharedOrderChannel => m_BelongsSharedOrderChannel;

	/// <summary>
	///   <para>UnderlyingModel.MemDoc.MemDocModel.</para>
	/// </summary>
	/// <param name="value">Requested type of quality of service (default Unreliable).</param>
	/// <param name="channel">Copy constructor.</param>
	public ChannelQOS(QosType value)
	{
		m_Type = value;
		m_BelongsSharedOrderChannel = false;
	}

	/// <summary>
	///   <para>UnderlyingModel.MemDoc.MemDocModel.</para>
	/// </summary>
	/// <param name="value">Requested type of quality of service (default Unreliable).</param>
	/// <param name="channel">Copy constructor.</param>
	public ChannelQOS()
	{
		m_Type = QosType.Unreliable;
		m_BelongsSharedOrderChannel = false;
	}

	/// <summary>
	///   <para>UnderlyingModel.MemDoc.MemDocModel.</para>
	/// </summary>
	/// <param name="value">Requested type of quality of service (default Unreliable).</param>
	/// <param name="channel">Copy constructor.</param>
	public ChannelQOS(ChannelQOS channel)
	{
		if (channel == null)
		{
			throw new NullReferenceException("channel is not defined");
		}
		m_Type = channel.m_Type;
		m_BelongsSharedOrderChannel = channel.m_BelongsSharedOrderChannel;
	}
}
