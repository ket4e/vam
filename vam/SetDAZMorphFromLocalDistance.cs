using UnityEngine;

public class SetDAZMorphFromLocalDistance : SetDAZMorph
{
	public enum axis
	{
		X,
		Y,
		Z
	}

	public Transform movingTransform;

	public bool updateEnabled = true;

	public axis distanceAxis;

	public float distanceLow;

	public float distanceHigh = 0.1f;

	public float currentDistance;

	private Vector3 startingLocalPosition;

	public float _multiplier = 1f;

	public bool clampMorphValue = true;

	public float multiplier
	{
		get
		{
			return _multiplier;
		}
		set
		{
			_multiplier = value;
		}
	}

	public void DoUpdate()
	{
		if (!isOn || !(movingTransform != null) || morph1 == null)
		{
			return;
		}
		float f = distanceAxis switch
		{
			axis.X => movingTransform.localPosition.x - startingLocalPosition.x, 
			axis.Y => movingTransform.localPosition.y - startingLocalPosition.y, 
			axis.Z => movingTransform.localPosition.z - startingLocalPosition.z, 
			_ => 0f, 
		};
		if (float.IsNaN(f))
		{
			Debug.LogError("Detected NaN value during distance calculation for SetDAZMorphFromLocalDistance " + base.name);
			return;
		}
		currentDistance = f;
		float num = (currentDistance * _multiplier - distanceLow) / (distanceHigh - distanceLow);
		float num2 = num;
		if (clampMorphValue)
		{
			num2 = Mathf.Clamp(num, 0f, 1f);
		}
		currentMorph1Value = morph1Low + (morph1High - morph1Low) * num2;
		morph1.SetValueThreadSafe(currentMorph1Value);
	}

	protected void SyncMorphJSON()
	{
		if (morph1 != null)
		{
			morph1.SyncJSON();
		}
	}

	private void Update()
	{
		if (updateEnabled)
		{
			DoUpdate();
			SyncMorphJSON();
		}
		else
		{
			SyncMorphJSON();
		}
	}

	private void Start()
	{
		startingLocalPosition = movingTransform.localPosition;
	}
}
