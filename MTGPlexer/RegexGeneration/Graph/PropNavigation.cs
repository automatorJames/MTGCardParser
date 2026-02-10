namespace MTGPlexer.RegexGeneration.Graph;

public class PropNavigation : TypeNavigation
{
    public PropertyInfo Prop { get; }
    public Proptions Proptions { get; }
    public string[] RegexPatterns { get; }

    public PropNavigation(PropertySnippet propertySnippet)
        : base(propertySnippet.Type, propertySnippet.Name)
    {
        Prop = propertySnippet.Prop;
        Proptions = propertySnippet.Proptions;
        RegexPatterns = Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;
    }
}
