namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Maps a RenderType to a specific render state override.</para>
/// </summary>
public struct RenderStateMapping
{
	private int m_RenderTypeID;

	private RenderStateBlock m_StateBlock;

	/// <summary>
	///   <para>Specifices the RenderType to override the render state for.</para>
	/// </summary>
	public string renderType
	{
		get
		{
			return Shader.IDToTag(m_RenderTypeID);
		}
		set
		{
			m_RenderTypeID = Shader.TagToID(value);
		}
	}

	/// <summary>
	///   <para>Specifies the values to override the render state with.</para>
	/// </summary>
	public RenderStateBlock stateBlock
	{
		get
		{
			return m_StateBlock;
		}
		set
		{
			m_StateBlock = value;
		}
	}

	/// <summary>
	///   <para>Creates a new render state mapping with the specified values.</para>
	/// </summary>
	/// <param name="renderType">Specifices the RenderType to override the render state for.</param>
	/// <param name="stateBlock">Specifies the values to override the render state with.</param>
	public RenderStateMapping(string renderType, RenderStateBlock stateBlock)
	{
		m_RenderTypeID = Shader.TagToID(renderType);
		m_StateBlock = stateBlock;
	}

	/// <summary>
	///   <para>Creates a new render state mapping with the specified values.</para>
	/// </summary>
	/// <param name="renderType">Specifices the RenderType to override the render state for.</param>
	/// <param name="stateBlock">Specifies the values to override the render state with.</param>
	public RenderStateMapping(RenderStateBlock stateBlock)
		: this(null, stateBlock)
	{
	}
}
