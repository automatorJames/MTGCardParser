namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record TerminalNode : Node
{
    public PropertyInfo Prop { get; set; }
    public ScalarAlternateSet ScalarAlternativeSet { get; protected set; }

    public TerminalNode(PropertyInfo prop)
    {
        Prop = prop;
        SetScalarAlternativeSet(prop);
    }

    protected virtual void SetScalarAlternativeSet(PropertyInfo prop)
    {
        if (TokenTypeRegistry.PropScalarAlternativeSets.TryGetValue(new TemplatePropInfo(prop), out var scalarAlternativeSet))
            ScalarAlternativeSet = scalarAlternativeSet;
        else
        {
            var captureAlternatives = (prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [prop.Name])
                .OrderByDescending(s => s.Length).ToList();

            ScalarAlternativeSet = new(captureAlternatives);
        }
    }
}