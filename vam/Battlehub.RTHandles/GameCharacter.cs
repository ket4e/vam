using Battlehub.Cubeman;
using Battlehub.RTCommon;
using UnityEngine;

namespace Battlehub.RTHandles;

[DisallowMultipleComponent]
public class GameCharacter : MonoBehaviour
{
	public CubemenGame Game;

	private Rigidbody m_rigidBody;

	private CubemanUserControl m_userControl;

	private Transform m_soul;

	private SkinnedMeshRenderer m_skinnedMeshRenderer;

	public Transform Camera
	{
		get
		{
			return m_userControl.Cam;
		}
		set
		{
			m_userControl.Cam = value;
		}
	}

	public bool IsActive
	{
		get
		{
			return m_userControl.HandleInput;
		}
		set
		{
			m_userControl.HandleInput = value;
		}
	}

	private void Awake()
	{
		m_userControl = GetComponent<CubemanUserControl>();
		m_soul = base.transform.Find("Soul");
		m_skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		m_rigidBody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (base.transform.position.y < -10f)
		{
			Die();
		}
		if (InputController.GetKeyDown(KeyCode.K))
		{
			Die();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Finish")
		{
			Game.OnPlayerFinish(this);
			if (m_skinnedMeshRenderer != null)
			{
				m_skinnedMeshRenderer.enabled = false;
			}
			if (m_soul != null)
			{
				m_soul.gameObject.SetActive(value: true);
			}
			Object.Destroy(base.gameObject, 2f);
		}
	}

	private void Die()
	{
		Game.OnPlayerDie(this);
		if (m_skinnedMeshRenderer != null)
		{
			m_skinnedMeshRenderer.enabled = false;
		}
		if (m_rigidBody != null)
		{
			m_rigidBody.isKinematic = true;
		}
		if (m_userControl != null)
		{
			m_userControl.HandleInput = false;
		}
		if (m_soul != null)
		{
			m_soul.gameObject.SetActive(value: true);
		}
		Object.Destroy(base.gameObject, 2f);
	}
}
