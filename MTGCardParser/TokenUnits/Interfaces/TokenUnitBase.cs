namespace MTGCardParser.TokenUnits.Interfaces;

public abstract class TokenUnitBase : ITokenUnit
{
    static string RegexTemplatePropName = "RegexTemplate";

    public ITokenUnit ParentToken { get; set; }
    public List<ITokenUnit> ChildTokens { get; set; } = new();
    public int RecursiveDepth { get; set; }
    public TextSpan MatchSpan { get; set; }
    public Dictionary<CaptureProp, TextSpan> PropMatches { get; set; } = new(new CapturePropComparer());

    public virtual void SetPropertiesFromMatchSpan()
    {
        var regexTemplate = GetRegexTemplate();
        regexTemplate.PropCaptureSegments.ForEach(x => x.SetValueFromMatchSpan(this, MatchSpan));

        foreach (var alternativeCaptureSet in regexTemplate.AlternativePropCaptureSets)
            if (!alternativeCaptureSet.Alternatives.Any(x => x.SetValueFromMatchSpan(this, MatchSpan)))
                throw new Exception($"Match string '{MatchSpan.ToStringValue()}' was passed to an alternative set, but no alternative was matched");
    }

    public RegexTemplate GetRegexTemplate()
    {
        var prop = GetType().GetProperty(RegexTemplatePropName);

        if (prop is null)
            throw new Exception($"{GetType().Name} doesn't contain a property named {RegexTemplatePropName})");

        return prop.GetValue(this) as RegexTemplate;
    }

    public static ITokenUnit InstantiateFromMatchString(Type type, TextSpan matchSpan, ITokenUnit parentToken = null)
    {
        if (!type.IsAssignableTo(typeof(ITokenUnit)))
            throw new Exception($"{type.Name} does not implement {nameof(ITokenUnit)}");

        var tokenInstance = (ITokenUnit)Activator.CreateInstance(type);
        tokenInstance.ParentToken = parentToken;
        tokenInstance.RecursiveDepth = parentToken is null ? 0 : parentToken.RecursiveDepth + 1;
        tokenInstance.MatchSpan = matchSpan;
        tokenInstance.SetPropertiesFromMatchSpan();

        return tokenInstance;
    }
}

