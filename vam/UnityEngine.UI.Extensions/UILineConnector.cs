namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/UI Line Connector")]
[RequireComponent(typeof(UILineRenderer))]
[ExecuteInEditMode]
public class UILineConnector : MonoBehaviour
{
	public RectTransform[] transforms;

	private Vector2[] previousPositions;

	private RectTransform canvas;

	private RectTransform rt;

	private UILineRenderer lr;

	private void Awake()
	{
		canvas = GetComponentInParent<RectTransform>().GetParentCanvas().GetComponent<RectTransform>();
		rt = GetComponent<RectTransform>();
		lr = GetComponent<UILineRenderer>();
	}

	private void Update()
	{
		if (transforms == null || transforms.Length < 1)
		{
			return;
		}
		if (previousPositions != null && previousPositions.Length == transforms.Length)
		{
			bool flag = false;
			for (int i = 0; i < transforms.Length; i++)
			{
				if (!flag && previousPositions[i] != transforms[i].anchoredPosition)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
		}
		Vector2 pivot = rt.pivot;
		Vector2 pivot2 = canvas.pivot;
		Vector3[] array = new Vector3[transforms.Length];
		Vector3[] array2 = new Vector3[transforms.Length];
		Vector2[] array3 = new Vector2[transforms.Length];
		for (int j = 0; j < transforms.Length; j++)
		{
			ref Vector3 reference = ref array[j];
			reference = transforms[j].TransformPoint(pivot);
		}
		for (int k = 0; k < transforms.Length; k++)
		{
			ref Vector3 reference2 = ref array2[k];
			reference2 = canvas.InverseTransformPoint(array[k]);
		}
		for (int l = 0; l < transforms.Length; l++)
		{
			ref Vector2 reference3 = ref array3[l];
			reference3 = new Vector2(array2[l].x, array2[l].y);
		}
		lr.Points = array3;
		lr.RelativeSize = false;
		lr.drivenExternally = true;
		previousPositions = new Vector2[transforms.Length];
		for (int m = 0; m < transforms.Length; m++)
		{
			ref Vector2 reference4 = ref previousPositions[m];
			reference4 = transforms[m].anchoredPosition;
		}
	}
}
