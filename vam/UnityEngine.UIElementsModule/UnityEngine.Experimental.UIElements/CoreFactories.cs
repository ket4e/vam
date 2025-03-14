namespace UnityEngine.Experimental.UIElements;

internal static class CoreFactories
{
	internal static void RegisterAll()
	{
		Factories.RegisterFactory<Button>(CreateButton);
		Factories.RegisterFactory<IMGUIContainer>(CreateIMGUIContainer);
		Factories.RegisterFactory<Image>((IUxmlAttributes _, CreationContext __) => new Image());
		Factories.RegisterFactory<Label>((IUxmlAttributes _, CreationContext __) => new Label());
		Factories.RegisterFactory<RepeatButton>(CreateRepeatButton);
		Factories.RegisterFactory<ScrollerButton>(CreateScrollerButton);
		Factories.RegisterFactory<ScrollView>((IUxmlAttributes _, CreationContext __) => new ScrollView());
		Factories.RegisterFactory<Scroller>(CreateScroller);
		Factories.RegisterFactory<Slider>(CreateSlider);
		Factories.RegisterFactory<TextField>((IUxmlAttributes _, CreationContext __) => new TextField());
		Factories.RegisterFactory<Toggle>(CreateToggle);
		Factories.RegisterFactory<VisualContainer>((IUxmlAttributes _, CreationContext __) => new VisualContainer());
		Factories.RegisterFactory<VisualElement>((IUxmlAttributes _, CreationContext __) => new VisualElement());
		Factories.RegisterFactory<TemplateContainer>(CreateTemplate);
	}

	private static VisualElement CreateButton(IUxmlAttributes bag, CreationContext ctx)
	{
		return new Button(null);
	}

	private static VisualElement CreateTemplate(IUxmlAttributes bag, CreationContext ctx)
	{
		string templateAlias = ((TemplateAsset)bag).templateAlias;
		VisualTreeAsset visualTreeAsset = ctx.visualTreeAsset.ResolveUsing(templateAlias);
		TemplateContainer templateContainer = new TemplateContainer(templateAlias);
		if (visualTreeAsset == null)
		{
			templateContainer.Add(new Label($"Unknown Element: '{templateAlias}'"));
		}
		else
		{
			visualTreeAsset.CloneTree(templateContainer, ctx.slotInsertionPoints);
		}
		if (visualTreeAsset == null)
		{
			Debug.LogErrorFormat("Could not resolve template with alias '{0}'", templateAlias);
		}
		return templateContainer;
	}

	private static VisualElement CreateIMGUIContainer(IUxmlAttributes bag, CreationContext ctx)
	{
		return new IMGUIContainer(null);
	}

	private static VisualElement CreateRepeatButton(IUxmlAttributes bag, CreationContext ctx)
	{
		return new RepeatButton(null, bag.GetPropertyLong("delay", 0L), bag.GetPropertyLong("interval", 0L));
	}

	private static VisualElement CreateScrollerButton(IUxmlAttributes bag, CreationContext ctx)
	{
		return new ScrollerButton(null, bag.GetPropertyLong("delay", 0L), bag.GetPropertyLong("interval", 0L));
	}

	private static VisualElement CreateScroller(IUxmlAttributes bag, CreationContext ctx)
	{
		return new Scroller(bag.GetPropertyFloat("lowValue", 0f), bag.GetPropertyFloat("highValue", 0f), null, bag.GetPropertyEnum("direction", Slider.Direction.Horizontal));
	}

	private static VisualElement CreateSlider(IUxmlAttributes bag, CreationContext ctx)
	{
		return new Slider(bag.GetPropertyFloat("lowValue", 0f), bag.GetPropertyFloat("highValue", 0f), null, bag.GetPropertyEnum("direction", Slider.Direction.Horizontal));
	}

	private static VisualElement CreateToggle(IUxmlAttributes bag, CreationContext ctx)
	{
		return new Toggle(null);
	}
}
