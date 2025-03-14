namespace UnityEngine.UI.Extensions.Examples;

public class Example01ScrollViewCell : FancyScrollViewCell<Example01CellDto>
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Text message;

	private readonly int scrollTriggerHash = Animator.StringToHash("scroll");

	private void Start()
	{
		RectTransform rectTransform = base.transform as RectTransform;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchoredPosition3D = Vector3.zero;
		UpdatePosition(0f);
	}

	public override void UpdateContent(Example01CellDto itemData)
	{
		message.text = itemData.Message;
	}

	public override void UpdatePosition(float position)
	{
		animator.Play(scrollTriggerHash, -1, position);
		animator.speed = 0f;
	}
}
