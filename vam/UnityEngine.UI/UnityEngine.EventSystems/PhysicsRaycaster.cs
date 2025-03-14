using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.EventSystems;

[AddComponentMenu("Event/Physics Raycaster")]
[RequireComponent(typeof(Camera))]
public class PhysicsRaycaster : BaseRaycaster
{
	protected const int kNoEventMaskSet = -1;

	protected Camera m_EventCamera;

	[SerializeField]
	protected LayerMask m_EventMask = -1;

	[SerializeField]
	protected int m_MaxRayIntersections = 0;

	protected int m_LastMaxRayIntersections = 0;

	private RaycastHit[] m_Hits;

	public override Camera eventCamera
	{
		get
		{
			if (m_EventCamera == null)
			{
				m_EventCamera = GetComponent<Camera>();
			}
			return m_EventCamera ?? Camera.main;
		}
	}

	public virtual int depth => (!(eventCamera != null)) ? 16777215 : ((int)eventCamera.depth);

	public int finalEventMask => (!(eventCamera != null)) ? (-1) : (eventCamera.cullingMask & (int)m_EventMask);

	public LayerMask eventMask
	{
		get
		{
			return m_EventMask;
		}
		set
		{
			m_EventMask = value;
		}
	}

	public int maxRayIntersections
	{
		get
		{
			return m_MaxRayIntersections;
		}
		set
		{
			m_MaxRayIntersections = value;
		}
	}

	protected PhysicsRaycaster()
	{
	}

	protected void ComputeRayAndDistance(PointerEventData eventData, out Ray ray, out float distanceToClipPlane)
	{
		ray = eventCamera.ScreenPointToRay(eventData.position);
		float z = ray.direction.z;
		distanceToClipPlane = ((!Mathf.Approximately(0f, z)) ? Mathf.Abs((eventCamera.farClipPlane - eventCamera.nearClipPlane) / z) : float.PositiveInfinity);
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		if (eventCamera == null || !eventCamera.pixelRect.Contains(eventData.position))
		{
			return;
		}
		ComputeRayAndDistance(eventData, out var ray, out var distanceToClipPlane);
		int num = 0;
		if (m_MaxRayIntersections == 0)
		{
			if (ReflectionMethodsCache.Singleton.raycast3DAll == null)
			{
				return;
			}
			m_Hits = ReflectionMethodsCache.Singleton.raycast3DAll(ray, distanceToClipPlane, finalEventMask);
			num = m_Hits.Length;
		}
		else
		{
			if (ReflectionMethodsCache.Singleton.getRaycastNonAlloc == null)
			{
				return;
			}
			if (m_LastMaxRayIntersections != m_MaxRayIntersections)
			{
				m_Hits = new RaycastHit[m_MaxRayIntersections];
				m_LastMaxRayIntersections = m_MaxRayIntersections;
			}
			num = ReflectionMethodsCache.Singleton.getRaycastNonAlloc(ray, m_Hits, distanceToClipPlane, finalEventMask);
		}
		if (num > 1)
		{
			Array.Sort(m_Hits, (RaycastHit r1, RaycastHit r2) => r1.distance.CompareTo(r2.distance));
		}
		if (num != 0)
		{
			int i = 0;
			for (int num2 = num; i < num2; i++)
			{
				RaycastResult raycastResult = default(RaycastResult);
				raycastResult.gameObject = m_Hits[i].collider.gameObject;
				raycastResult.module = this;
				raycastResult.distance = m_Hits[i].distance;
				raycastResult.worldPosition = m_Hits[i].point;
				raycastResult.worldNormal = m_Hits[i].normal;
				raycastResult.screenPosition = eventData.position;
				raycastResult.index = resultAppendList.Count;
				raycastResult.sortingLayer = 0;
				raycastResult.sortingOrder = 0;
				RaycastResult item = raycastResult;
				resultAppendList.Add(item);
			}
		}
	}
}
