using System.Collections.Generic;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

[RequireComponent(typeof(MeshCollider))]
public class PointerUIMesh : PointerUIBase
{
	protected MeshCollider meshCollider;

	protected Dictionary<int, RaycastHit> rayHits = new Dictionary<int, RaycastHit>();

	[Tooltip("Which layers should UI rays collide with (and be able to hit)?")]
	public LayerMask layerMask = -1;

	public override void Awake()
	{
		base.Awake();
		meshCollider = GetComponent<MeshCollider>();
	}

	protected override Vector2 MapPointerToBrowser(Vector2 screenPosition, int pointerId)
	{
		Camera camera = ((!viewCamera) ? Camera.main : viewCamera);
		if (!camera)
		{
			Debug.LogError("No main camera and no viewCamera specified. We can't map screen-space mouse clicks to the browser without a camera.", this);
			enableMouseInput = false;
			return new Vector2(float.NaN, float.NaN);
		}
		return MapRayToBrowser(camera.ScreenPointToRay(screenPosition), pointerId);
	}

	protected override Vector2 MapRayToBrowser(Ray worldRay, int pointerId)
	{
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(worldRay, out hitInfo, maxDistance, layerMask);
		rayHits[pointerId] = hitInfo;
		if (!flag || hitInfo.collider.transform != meshCollider.transform)
		{
			return new Vector3(float.NaN, float.NaN);
		}
		return hitInfo.textureCoord;
	}

	public override void GetCurrentHitLocation(out Vector3 pos, out Quaternion rot)
	{
		if (currentPointerId == 0)
		{
			pos = new Vector3(float.NaN, float.NaN, float.NaN);
			rot = Quaternion.identity;
			return;
		}
		RaycastHit raycastHit = rayHits[currentPointerId];
		Vector3 up = raycastHit.collider.transform.up;
		pos = raycastHit.point;
		rot = Quaternion.LookRotation(-raycastHit.normal, up);
	}
}
