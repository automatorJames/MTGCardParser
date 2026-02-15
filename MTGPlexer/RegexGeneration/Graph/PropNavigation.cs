namespace MTGPlexer.RegexGeneration.Graph;

public class PropNavigation : TypeNavigation
{
    public PropertyInfo Prop { get; }
    public Proptions Proptions { get; }

    public PropNavigation(PropertySnippet propertySnippet)
        : base(
            propertySnippet.Type, 
            propertySnippet.Name, 
            propertySnippet.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns)
    {
        Prop = propertySnippet.Prop;
        Proptions = propertySnippet.Proptions;
    }
}
