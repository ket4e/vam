namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Effects/Extensions/UILinearDodgeEffect")]
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UILinearDodgeEffect : MonoBehaviour
{
	private MaskableGraphic mGraphic;

	private void Start()
	{
		SetMaterial();
	}

	public void SetMaterial()
	{
		mGraphic = GetComponent<MaskableGraphic>();
		if (mGraphic != null)
		{
			if (mGraphic.material == null || mGraphic.material.name == "Default UI Material")
			{
				mGraphic.material = new Material(Shader.Find("UI Extensions/UILinearDodge"));
			}
		}
		else
		{
			Debug.LogError("Please attach component to a Graphical UI component");
		}
	}

	public void OnValidate()
	{
		SetMaterial();
	}
}
